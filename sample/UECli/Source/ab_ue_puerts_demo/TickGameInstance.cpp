// Fill out your copyright notice in the Description page of Project Settings.


#include "TickGameInstance.h"

FReceiveThread::FReceiveThread(UTickGameInstance* _instance)
{
	instance = _instance;

	threadRuning = true;
	thread = FRunnableThread::Create(this, TEXT("ReceiveThread"), 0);
}

uint32 FReceiveThread::Run()
{
	TArray<uint8> buff;
	int32 readlen = 0;
	while (threadRuning) {
		for (auto s : instance->socketMap) {
			buff.Init(0, 1024u);
			s.Value.socket->Recv(buff.GetData(), buff.Num(), readlen);
			if (readlen > 0) {
				s.Value.buffer.Data = buff.GetData();
				s.Value.buffer.Length = readlen;
				if (instance->NotifyNetMsg.IsBound())
				{
					instance->NotifyNetMsg.Execute(s.Value.index, s.Value.buffer);
				}
			}
		}
	}
	return 1;
}

void FReceiveThread::Stop()
{
	threadRuning = false;
	if (thread) {
		thread->WaitForCompletion();
	}
}

FReceiveThread::~FReceiveThread()
{
	threadRuning = false;
	delete thread;
	thread = NULL;
}


void UTickGameInstance::OnStart()
{
	receiveThread = MakeShared<FReceiveThread>(this);
}

void UTickGameInstance::Shutdown()
{
	receiveThread->Stop();
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