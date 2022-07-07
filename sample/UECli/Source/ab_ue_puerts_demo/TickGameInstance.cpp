// Fill out your copyright notice in the Description page of Project Settings.

#include "TickGameInstance.h"

#include "HAL/UnrealMemory.h"

FReceiveThread::FReceiveThread(UTickGameInstance* _instance, SocketInfo _socket)
{
	instance = _instance;
	socket = _socket;

	threadRuning = true;
	thread = FRunnableThread::Create(this, TEXT("ReceiveThread"), 0);
}

uint32 FReceiveThread::Run()
{
	int32 readlen = 0;
	while (threadRuning) {
		bool is_idel = true;
		
		TArray<uint8> buff;
		buff.Init(0, 1024u);
		readlen = 0;
		socket.socket->Recv(buff.GetData(), buff.Num(), readlen);
		if (readlen > 0) {
			instance->Recv(socket.index, (char*)buff.GetData(), readlen);
			is_idel = false;
		}

		if(is_idel) {
			FPlatformProcess::Sleep(0.001);
		}
	}
	return 1;
}

void FReceiveThread::Stop()
{
	threadRuning = false;
	if (thread) {
		thread->WaitForCompletion();
		delete thread;
	}
}

void UTickGameInstance::OnStart()
{
}

void UTickGameInstance::Shutdown()
{
	for (auto receiveThread : receiveThreads) {
		receiveThread->Stop();
	}
}


int32 UTickGameInstance::Connect(FString _ip, int32 _port)
{
	SocketInfo info;

	FIPv4Address::Parse(_ip, info.addr);
	TSharedPtr<FInternetAddr> addr = ISocketSubsystem::Get(PLATFORM_SOCKETSUBSYSTEM)->CreateInternetAddr();
	addr->SetIp(info.addr.Value);
	addr->SetPort(_port);

	info.socket = ISocketSubsystem::Get(PLATFORM_SOCKETSUBSYSTEM)->CreateSocket(NAME_Stream, TEXT("Default"), false);
	if (!info.socket->Connect(*addr))
	{
		UE_LOG(LogTemp, Error, TEXT("Connect Failed"));
		return -1;
	}

	info.index = socketMap.Num();
	socketMap.Add(info.index, info);

	auto receiveThread = MakeShared<FReceiveThread>(this, info);
	receiveThreads.Add(receiveThread);

	return info.index;
}

bool UTickGameInstance::Send(int32 socket, FArrayBuffer buffer)
{
	auto socket_info = socketMap.Find(socket);
	if (socket_info == nullptr) {
		UE_LOG(LogTemp, Error, TEXT("Message Send Failly undefined socket!"));
		return false;
	}

	int send = 0;
	if (!socket_info->socket->Send((uint8*)buffer.Data, buffer.Length, send))
	{
		UE_LOG(LogTemp, Error, TEXT("Message Send Failly"));
		return false;
	}

	return true;
}

void UTickGameInstance::Recv(int32 socketIndex, char* data, int32 len) {
	NetData netData;
	netData.index = socketIndex;
	FMemory::Memcpy(netData.data, data, len);
	netData.buffer.Data = netData.data;
	netData.buffer.Length = len;
	netDataQue.Enqueue(netData);
}

void UTickGameInstance::Tick(float DeltaTime) {
	NetData netData;
	while (netDataQue.Dequeue(netData)) {
		if (NotifyNetMsg.IsBound())
		{
			NotifyNetMsg.Execute(netData.index, netData.buffer);
		}
	}
}

TStatId UTickGameInstance::GetStatId() const {
	return Super::GetStatID();
}