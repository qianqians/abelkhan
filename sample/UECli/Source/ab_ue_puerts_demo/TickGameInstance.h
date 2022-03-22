// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "Tickable.h"
#include "ArrayBuffer.h"
#include "CoreMinimal.h"
#include "HAL/Runnable.h"
#include "HAL/RunnableThread.h"
#include "Runtime/Core/Public/Containers/Map.h"
#include "Runtime/Core/Public/Delegates/DelegateCombinations.h"
#include "Runtime/NEtworking/Public/Networking.h"
#include "Engine/GameInstance.h"
#include "TickGameInstance.generated.h"

DECLARE_DYNAMIC_DELEGATE_TwoParams(FNotifyNetMsg, int32, socket, FArrayBuffer&, buffer);

class FReceiveThread : public FRunnable
{
private:
	FRunnableThread* thread = nullptr;
	bool threadRuning;
	UTickGameInstance* instance;

public:
	virtual uint32 Run() override;

	virtual void Stop() override;

public:
	FReceiveThread(UTickGameInstance* _instance);

};

/**
 * 
 */
UCLASS()
class AB_UE_PUERTS_DEMO_API UTickGameInstance : public UGameInstance
{
	GENERATED_BODY()

public:
	virtual void OnStart() override;

	virtual void Shutdown() override;

public:
	UFUNCTION(BlueprintCallable, Category = "ClientSocket")
	int32 Connect(FString ip, int32 port);

	UFUNCTION(BlueprintCallable, Category = "ClientSocket")
	bool Send(int32 socket, FArrayBuffer buffer);

	UPROPERTY()
	FNotifyNetMsg NotifyNetMsg;

private:
	struct SocketInfo {
		FIPv4Address addr;
		uint16 port;
		int32 index;
		FSocket* socket;
		FArrayBuffer buffer;
	};
	TMap<int32, SocketInfo> socketMap;

	TSharedPtr<FReceiveThread> receiveThread;

	friend class FReceiveThread;

};
