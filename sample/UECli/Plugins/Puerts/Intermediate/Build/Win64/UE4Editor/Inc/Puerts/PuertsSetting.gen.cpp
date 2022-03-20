// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/GeneratedCppIncludes.h"
#include "Puerts/Private/PuertsSetting.h"
#ifdef _MSC_VER
#pragma warning (push)
#pragma warning (disable : 4883)
#endif
PRAGMA_DISABLE_DEPRECATION_WARNINGS
void EmptyLinkFunctionForGeneratedCodePuertsSetting() {}
// Cross Module References
	PUERTS_API UClass* Z_Construct_UClass_UPuertsSetting_NoRegister();
	PUERTS_API UClass* Z_Construct_UClass_UPuertsSetting();
	COREUOBJECT_API UClass* Z_Construct_UClass_UObject();
	UPackage* Z_Construct_UPackage__Script_Puerts();
// End Cross Module References
	void UPuertsSetting::StaticRegisterNativesUPuertsSetting()
	{
	}
	UClass* Z_Construct_UClass_UPuertsSetting_NoRegister()
	{
		return UPuertsSetting::StaticClass();
	}
	struct Z_Construct_UClass_UPuertsSetting_Statics
	{
		static UObject* (*const DependentSingletons[])();
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Class_MetaDataParams[];
#endif
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_AutoModeEnable_MetaData[];
#endif
		static void NewProp_AutoModeEnable_SetBit(void* Obj);
		static const UE4CodeGen_Private::FBoolPropertyParams NewProp_AutoModeEnable;
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_DebugEnable_MetaData[];
#endif
		static void NewProp_DebugEnable_SetBit(void* Obj);
		static const UE4CodeGen_Private::FBoolPropertyParams NewProp_DebugEnable;
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_DebugPort_MetaData[];
#endif
		static const UE4CodeGen_Private::FIntPropertyParams NewProp_DebugPort;
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_WaitDebugger_MetaData[];
#endif
		static void NewProp_WaitDebugger_SetBit(void* Obj);
		static const UE4CodeGen_Private::FBoolPropertyParams NewProp_WaitDebugger;
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_WaitDebuggerTimeout_MetaData[];
#endif
		static const UE4CodeGen_Private::FDoublePropertyParams NewProp_WaitDebuggerTimeout;
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_NumberOfJsEnv_MetaData[];
#endif
		static const UE4CodeGen_Private::FIntPropertyParams NewProp_NumberOfJsEnv;
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_WatchDisable_MetaData[];
#endif
		static void NewProp_WatchDisable_SetBit(void* Obj);
		static const UE4CodeGen_Private::FBoolPropertyParams NewProp_WatchDisable;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
		static const FCppClassTypeInfoStatic StaticCppClassTypeInfo;
		static const UE4CodeGen_Private::FClassParams ClassParams;
	};
	UObject* (*const Z_Construct_UClass_UPuertsSetting_Statics::DependentSingletons[])() = {
		(UObject* (*)())Z_Construct_UClass_UObject,
		(UObject* (*)())Z_Construct_UPackage__Script_Puerts,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UPuertsSetting_Statics::Class_MetaDataParams[] = {
		{ "DisplayName", "Puerts" },
		{ "IncludePath", "PuertsSetting.h" },
		{ "ModuleRelativePath", "Private/PuertsSetting.h" },
	};
#endif
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UPuertsSetting_Statics::NewProp_AutoModeEnable_MetaData[] = {
		{ "Category", "Engine Class Extends Mode" },
		{ "defaultValue", "FALSE" },
		{ "DisplayName", "Enable" },
		{ "ModuleRelativePath", "Private/PuertsSetting.h" },
	};
#endif
	void Z_Construct_UClass_UPuertsSetting_Statics::NewProp_AutoModeEnable_SetBit(void* Obj)
	{
		((UPuertsSetting*)Obj)->AutoModeEnable = 1;
	}
	const UE4CodeGen_Private::FBoolPropertyParams Z_Construct_UClass_UPuertsSetting_Statics::NewProp_AutoModeEnable = { "AutoModeEnable", nullptr, (EPropertyFlags)0x0010000000004001, UE4CodeGen_Private::EPropertyGenFlags::Bool | UE4CodeGen_Private::EPropertyGenFlags::NativeBool, RF_Public|RF_Transient|RF_MarkAsNative, 1, sizeof(bool), sizeof(UPuertsSetting), &Z_Construct_UClass_UPuertsSetting_Statics::NewProp_AutoModeEnable_SetBit, METADATA_PARAMS(Z_Construct_UClass_UPuertsSetting_Statics::NewProp_AutoModeEnable_MetaData, UE_ARRAY_COUNT(Z_Construct_UClass_UPuertsSetting_Statics::NewProp_AutoModeEnable_MetaData)) };
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UPuertsSetting_Statics::NewProp_DebugEnable_MetaData[] = {
		{ "Category", "Engine Class Extends Mode" },
		{ "defaultValue", "FALSE" },
		{ "DisplayName", "Debug Enable" },
		{ "ModuleRelativePath", "Private/PuertsSetting.h" },
	};
#endif
	void Z_Construct_UClass_UPuertsSetting_Statics::NewProp_DebugEnable_SetBit(void* Obj)
	{
		((UPuertsSetting*)Obj)->DebugEnable = 1;
	}
	const UE4CodeGen_Private::FBoolPropertyParams Z_Construct_UClass_UPuertsSetting_Statics::NewProp_DebugEnable = { "DebugEnable", nullptr, (EPropertyFlags)0x0010000000004001, UE4CodeGen_Private::EPropertyGenFlags::Bool | UE4CodeGen_Private::EPropertyGenFlags::NativeBool, RF_Public|RF_Transient|RF_MarkAsNative, 1, sizeof(bool), sizeof(UPuertsSetting), &Z_Construct_UClass_UPuertsSetting_Statics::NewProp_DebugEnable_SetBit, METADATA_PARAMS(Z_Construct_UClass_UPuertsSetting_Statics::NewProp_DebugEnable_MetaData, UE_ARRAY_COUNT(Z_Construct_UClass_UPuertsSetting_Statics::NewProp_DebugEnable_MetaData)) };
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UPuertsSetting_Statics::NewProp_DebugPort_MetaData[] = {
		{ "Category", "Engine Class Extends Mode" },
		{ "defaultValue", "8080" },
		{ "DisplayName", "Debug Port" },
		{ "ModuleRelativePath", "Private/PuertsSetting.h" },
	};
#endif
	const UE4CodeGen_Private::FIntPropertyParams Z_Construct_UClass_UPuertsSetting_Statics::NewProp_DebugPort = { "DebugPort", nullptr, (EPropertyFlags)0x0010000000004001, UE4CodeGen_Private::EPropertyGenFlags::Int, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(UPuertsSetting, DebugPort), METADATA_PARAMS(Z_Construct_UClass_UPuertsSetting_Statics::NewProp_DebugPort_MetaData, UE_ARRAY_COUNT(Z_Construct_UClass_UPuertsSetting_Statics::NewProp_DebugPort_MetaData)) };
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UPuertsSetting_Statics::NewProp_WaitDebugger_MetaData[] = {
		{ "Category", "Engine Class Extends Mode" },
		{ "defaultValue", "FALSE" },
		{ "DisplayName", "Wait Debugger" },
		{ "ModuleRelativePath", "Private/PuertsSetting.h" },
	};
#endif
	void Z_Construct_UClass_UPuertsSetting_Statics::NewProp_WaitDebugger_SetBit(void* Obj)
	{
		((UPuertsSetting*)Obj)->WaitDebugger = 1;
	}
	const UE4CodeGen_Private::FBoolPropertyParams Z_Construct_UClass_UPuertsSetting_Statics::NewProp_WaitDebugger = { "WaitDebugger", nullptr, (EPropertyFlags)0x0010000000004001, UE4CodeGen_Private::EPropertyGenFlags::Bool | UE4CodeGen_Private::EPropertyGenFlags::NativeBool, RF_Public|RF_Transient|RF_MarkAsNative, 1, sizeof(bool), sizeof(UPuertsSetting), &Z_Construct_UClass_UPuertsSetting_Statics::NewProp_WaitDebugger_SetBit, METADATA_PARAMS(Z_Construct_UClass_UPuertsSetting_Statics::NewProp_WaitDebugger_MetaData, UE_ARRAY_COUNT(Z_Construct_UClass_UPuertsSetting_Statics::NewProp_WaitDebugger_MetaData)) };
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UPuertsSetting_Statics::NewProp_WaitDebuggerTimeout_MetaData[] = {
		{ "Category", "Engine Class Extends Mode" },
		{ "defaultValue", "0" },
		{ "DisplayName", "Wait Debugger Timeout" },
		{ "ModuleRelativePath", "Private/PuertsSetting.h" },
	};
#endif
	const UE4CodeGen_Private::FDoublePropertyParams Z_Construct_UClass_UPuertsSetting_Statics::NewProp_WaitDebuggerTimeout = { "WaitDebuggerTimeout", nullptr, (EPropertyFlags)0x0010000000004001, UE4CodeGen_Private::EPropertyGenFlags::Double, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(UPuertsSetting, WaitDebuggerTimeout), METADATA_PARAMS(Z_Construct_UClass_UPuertsSetting_Statics::NewProp_WaitDebuggerTimeout_MetaData, UE_ARRAY_COUNT(Z_Construct_UClass_UPuertsSetting_Statics::NewProp_WaitDebuggerTimeout_MetaData)) };
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UPuertsSetting_Statics::NewProp_NumberOfJsEnv_MetaData[] = {
		{ "Category", "Engine Class Extends Mode" },
		{ "defaultValue", "1" },
		{ "DisplayName", "Number of JavaScript Env" },
		{ "ModuleRelativePath", "Private/PuertsSetting.h" },
	};
#endif
	const UE4CodeGen_Private::FIntPropertyParams Z_Construct_UClass_UPuertsSetting_Statics::NewProp_NumberOfJsEnv = { "NumberOfJsEnv", nullptr, (EPropertyFlags)0x0010000000004001, UE4CodeGen_Private::EPropertyGenFlags::Int, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(UPuertsSetting, NumberOfJsEnv), METADATA_PARAMS(Z_Construct_UClass_UPuertsSetting_Statics::NewProp_NumberOfJsEnv_MetaData, UE_ARRAY_COUNT(Z_Construct_UClass_UPuertsSetting_Statics::NewProp_NumberOfJsEnv_MetaData)) };
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UPuertsSetting_Statics::NewProp_WatchDisable_MetaData[] = {
		{ "Category", "Engine Class Extends Mode" },
		{ "defaultValue", "FALSE" },
		{ "DisplayName", "Disable TypeScript Watch" },
		{ "ModuleRelativePath", "Private/PuertsSetting.h" },
	};
#endif
	void Z_Construct_UClass_UPuertsSetting_Statics::NewProp_WatchDisable_SetBit(void* Obj)
	{
		((UPuertsSetting*)Obj)->WatchDisable = 1;
	}
	const UE4CodeGen_Private::FBoolPropertyParams Z_Construct_UClass_UPuertsSetting_Statics::NewProp_WatchDisable = { "WatchDisable", nullptr, (EPropertyFlags)0x0010000000004001, UE4CodeGen_Private::EPropertyGenFlags::Bool | UE4CodeGen_Private::EPropertyGenFlags::NativeBool, RF_Public|RF_Transient|RF_MarkAsNative, 1, sizeof(bool), sizeof(UPuertsSetting), &Z_Construct_UClass_UPuertsSetting_Statics::NewProp_WatchDisable_SetBit, METADATA_PARAMS(Z_Construct_UClass_UPuertsSetting_Statics::NewProp_WatchDisable_MetaData, UE_ARRAY_COUNT(Z_Construct_UClass_UPuertsSetting_Statics::NewProp_WatchDisable_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UClass_UPuertsSetting_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UClass_UPuertsSetting_Statics::NewProp_AutoModeEnable,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UClass_UPuertsSetting_Statics::NewProp_DebugEnable,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UClass_UPuertsSetting_Statics::NewProp_DebugPort,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UClass_UPuertsSetting_Statics::NewProp_WaitDebugger,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UClass_UPuertsSetting_Statics::NewProp_WaitDebuggerTimeout,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UClass_UPuertsSetting_Statics::NewProp_NumberOfJsEnv,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UClass_UPuertsSetting_Statics::NewProp_WatchDisable,
	};
	const FCppClassTypeInfoStatic Z_Construct_UClass_UPuertsSetting_Statics::StaticCppClassTypeInfo = {
		TCppClassTypeTraits<UPuertsSetting>::IsAbstract,
	};
	const UE4CodeGen_Private::FClassParams Z_Construct_UClass_UPuertsSetting_Statics::ClassParams = {
		&UPuertsSetting::StaticClass,
		"Puerts",
		&StaticCppClassTypeInfo,
		DependentSingletons,
		nullptr,
		Z_Construct_UClass_UPuertsSetting_Statics::PropPointers,
		nullptr,
		UE_ARRAY_COUNT(DependentSingletons),
		0,
		UE_ARRAY_COUNT(Z_Construct_UClass_UPuertsSetting_Statics::PropPointers),
		0,
		0x000000A6u,
		METADATA_PARAMS(Z_Construct_UClass_UPuertsSetting_Statics::Class_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UClass_UPuertsSetting_Statics::Class_MetaDataParams))
	};
	UClass* Z_Construct_UClass_UPuertsSetting()
	{
		static UClass* OuterClass = nullptr;
		if (!OuterClass)
		{
			UE4CodeGen_Private::ConstructUClass(OuterClass, Z_Construct_UClass_UPuertsSetting_Statics::ClassParams);
		}
		return OuterClass;
	}
	IMPLEMENT_CLASS(UPuertsSetting, 1374667471);
	template<> PUERTS_API UClass* StaticClass<UPuertsSetting>()
	{
		return UPuertsSetting::StaticClass();
	}
	static FCompiledInDefer Z_CompiledInDefer_UClass_UPuertsSetting(Z_Construct_UClass_UPuertsSetting, &UPuertsSetting::StaticClass, TEXT("/Script/Puerts"), TEXT("UPuertsSetting"), false, nullptr, nullptr, nullptr);
	DEFINE_VTABLE_PTR_HELPER_CTOR(UPuertsSetting);
PRAGMA_ENABLE_DEPRECATION_WARNINGS
#ifdef _MSC_VER
#pragma warning (pop)
#endif
