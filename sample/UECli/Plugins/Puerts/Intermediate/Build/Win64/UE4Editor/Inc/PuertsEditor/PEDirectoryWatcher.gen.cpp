// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/GeneratedCppIncludes.h"
#include "PuertsEditor/Public/PEDirectoryWatcher.h"
#ifdef _MSC_VER
#pragma warning (push)
#pragma warning (disable : 4883)
#endif
PRAGMA_DISABLE_DEPRECATION_WARNINGS
void EmptyLinkFunctionForGeneratedCodePEDirectoryWatcher() {}
// Cross Module References
	PUERTSEDITOR_API UFunction* Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature();
	UPackage* Z_Construct_UPackage__Script_PuertsEditor();
	PUERTSEDITOR_API UClass* Z_Construct_UClass_UPEDirectoryWatcher_NoRegister();
	PUERTSEDITOR_API UClass* Z_Construct_UClass_UPEDirectoryWatcher();
	COREUOBJECT_API UClass* Z_Construct_UClass_UObject();
// End Cross Module References
	struct Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics
	{
		struct _Script_PuertsEditor_eventDirectoryWatcherCallback_Parms
		{
			TArray<FString> Added;
			TArray<FString> Modified;
			TArray<FString> Removed;
		};
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_Added_Inner;
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_Added_MetaData[];
#endif
		static const UE4CodeGen_Private::FArrayPropertyParams NewProp_Added;
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_Modified_Inner;
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_Modified_MetaData[];
#endif
		static const UE4CodeGen_Private::FArrayPropertyParams NewProp_Modified;
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_Removed_Inner;
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_Removed_MetaData[];
#endif
		static const UE4CodeGen_Private::FArrayPropertyParams NewProp_Removed;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::NewProp_Added_Inner = { "Added", nullptr, (EPropertyFlags)0x0000000000000000, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, 0, METADATA_PARAMS(nullptr, 0) };
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::NewProp_Added_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FArrayPropertyParams Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::NewProp_Added = { "Added", nullptr, (EPropertyFlags)0x0010000008000182, UE4CodeGen_Private::EPropertyGenFlags::Array, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(_Script_PuertsEditor_eventDirectoryWatcherCallback_Parms, Added), EArrayPropertyFlags::None, METADATA_PARAMS(Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::NewProp_Added_MetaData, UE_ARRAY_COUNT(Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::NewProp_Added_MetaData)) };
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::NewProp_Modified_Inner = { "Modified", nullptr, (EPropertyFlags)0x0000000000000000, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, 0, METADATA_PARAMS(nullptr, 0) };
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::NewProp_Modified_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FArrayPropertyParams Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::NewProp_Modified = { "Modified", nullptr, (EPropertyFlags)0x0010000008000182, UE4CodeGen_Private::EPropertyGenFlags::Array, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(_Script_PuertsEditor_eventDirectoryWatcherCallback_Parms, Modified), EArrayPropertyFlags::None, METADATA_PARAMS(Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::NewProp_Modified_MetaData, UE_ARRAY_COUNT(Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::NewProp_Modified_MetaData)) };
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::NewProp_Removed_Inner = { "Removed", nullptr, (EPropertyFlags)0x0000000000000000, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, 0, METADATA_PARAMS(nullptr, 0) };
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::NewProp_Removed_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FArrayPropertyParams Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::NewProp_Removed = { "Removed", nullptr, (EPropertyFlags)0x0010000008000182, UE4CodeGen_Private::EPropertyGenFlags::Array, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(_Script_PuertsEditor_eventDirectoryWatcherCallback_Parms, Removed), EArrayPropertyFlags::None, METADATA_PARAMS(Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::NewProp_Removed_MetaData, UE_ARRAY_COUNT(Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::NewProp_Removed_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::NewProp_Added_Inner,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::NewProp_Added,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::NewProp_Modified_Inner,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::NewProp_Modified,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::NewProp_Removed_Inner,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::NewProp_Removed,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::Function_MetaDataParams[] = {
		{ "ModuleRelativePath", "Public/PEDirectoryWatcher.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::FuncParams = { (UObject*(*)())Z_Construct_UPackage__Script_PuertsEditor, nullptr, "DirectoryWatcherCallback__DelegateSignature", nullptr, nullptr, sizeof(_Script_PuertsEditor_eventDirectoryWatcherCallback_Parms), Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00130000, 0, 0, METADATA_PARAMS(Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	DEFINE_FUNCTION(UPEDirectoryWatcher::execUnWatch)
	{
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->UnWatch();
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEDirectoryWatcher::execWatch)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_InDirectory);
		P_FINISH;
		P_NATIVE_BEGIN;
		*(bool*)Z_Param__Result=P_THIS->Watch(Z_Param_InDirectory);
		P_NATIVE_END;
	}
	void UPEDirectoryWatcher::StaticRegisterNativesUPEDirectoryWatcher()
	{
		UClass* Class = UPEDirectoryWatcher::StaticClass();
		static const FNameNativePtrPair Funcs[] = {
			{ "UnWatch", &UPEDirectoryWatcher::execUnWatch },
			{ "Watch", &UPEDirectoryWatcher::execWatch },
		};
		FNativeFunctionRegistrar::RegisterFunctions(Class, Funcs, UE_ARRAY_COUNT(Funcs));
	}
	struct Z_Construct_UFunction_UPEDirectoryWatcher_UnWatch_Statics
	{
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEDirectoryWatcher_UnWatch_Statics::Function_MetaDataParams[] = {
		{ "Category", "File" },
		{ "ModuleRelativePath", "Public/PEDirectoryWatcher.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEDirectoryWatcher_UnWatch_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEDirectoryWatcher, nullptr, "UnWatch", nullptr, nullptr, 0, nullptr, 0, RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEDirectoryWatcher_UnWatch_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEDirectoryWatcher_UnWatch_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEDirectoryWatcher_UnWatch()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEDirectoryWatcher_UnWatch_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEDirectoryWatcher_Watch_Statics
	{
		struct PEDirectoryWatcher_eventWatch_Parms
		{
			FString InDirectory;
			bool ReturnValue;
		};
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InDirectory_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InDirectory;
		static void NewProp_ReturnValue_SetBit(void* Obj);
		static const UE4CodeGen_Private::FBoolPropertyParams NewProp_ReturnValue;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEDirectoryWatcher_Watch_Statics::NewProp_InDirectory_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEDirectoryWatcher_Watch_Statics::NewProp_InDirectory = { "InDirectory", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEDirectoryWatcher_eventWatch_Parms, InDirectory), METADATA_PARAMS(Z_Construct_UFunction_UPEDirectoryWatcher_Watch_Statics::NewProp_InDirectory_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEDirectoryWatcher_Watch_Statics::NewProp_InDirectory_MetaData)) };
	void Z_Construct_UFunction_UPEDirectoryWatcher_Watch_Statics::NewProp_ReturnValue_SetBit(void* Obj)
	{
		((PEDirectoryWatcher_eventWatch_Parms*)Obj)->ReturnValue = 1;
	}
	const UE4CodeGen_Private::FBoolPropertyParams Z_Construct_UFunction_UPEDirectoryWatcher_Watch_Statics::NewProp_ReturnValue = { "ReturnValue", nullptr, (EPropertyFlags)0x0010000000000580, UE4CodeGen_Private::EPropertyGenFlags::Bool | UE4CodeGen_Private::EPropertyGenFlags::NativeBool, RF_Public|RF_Transient|RF_MarkAsNative, 1, sizeof(bool), sizeof(PEDirectoryWatcher_eventWatch_Parms), &Z_Construct_UFunction_UPEDirectoryWatcher_Watch_Statics::NewProp_ReturnValue_SetBit, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEDirectoryWatcher_Watch_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEDirectoryWatcher_Watch_Statics::NewProp_InDirectory,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEDirectoryWatcher_Watch_Statics::NewProp_ReturnValue,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEDirectoryWatcher_Watch_Statics::Function_MetaDataParams[] = {
		{ "Category", "File" },
		{ "ModuleRelativePath", "Public/PEDirectoryWatcher.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEDirectoryWatcher_Watch_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEDirectoryWatcher, nullptr, "Watch", nullptr, nullptr, sizeof(PEDirectoryWatcher_eventWatch_Parms), Z_Construct_UFunction_UPEDirectoryWatcher_Watch_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEDirectoryWatcher_Watch_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEDirectoryWatcher_Watch_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEDirectoryWatcher_Watch_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEDirectoryWatcher_Watch()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEDirectoryWatcher_Watch_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	UClass* Z_Construct_UClass_UPEDirectoryWatcher_NoRegister()
	{
		return UPEDirectoryWatcher::StaticClass();
	}
	struct Z_Construct_UClass_UPEDirectoryWatcher_Statics
	{
		static UObject* (*const DependentSingletons[])();
		static const FClassFunctionLinkInfo FuncInfo[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Class_MetaDataParams[];
#endif
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_OnChanged_MetaData[];
#endif
		static const UE4CodeGen_Private::FMulticastDelegatePropertyParams NewProp_OnChanged;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
		static const FCppClassTypeInfoStatic StaticCppClassTypeInfo;
		static const UE4CodeGen_Private::FClassParams ClassParams;
	};
	UObject* (*const Z_Construct_UClass_UPEDirectoryWatcher_Statics::DependentSingletons[])() = {
		(UObject* (*)())Z_Construct_UClass_UObject,
		(UObject* (*)())Z_Construct_UPackage__Script_PuertsEditor,
	};
	const FClassFunctionLinkInfo Z_Construct_UClass_UPEDirectoryWatcher_Statics::FuncInfo[] = {
		{ &Z_Construct_UFunction_UPEDirectoryWatcher_UnWatch, "UnWatch" }, // 4064989603
		{ &Z_Construct_UFunction_UPEDirectoryWatcher_Watch, "Watch" }, // 381597062
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UPEDirectoryWatcher_Statics::Class_MetaDataParams[] = {
		{ "Comment", "/**\n *\n */" },
		{ "IncludePath", "PEDirectoryWatcher.h" },
		{ "ModuleRelativePath", "Public/PEDirectoryWatcher.h" },
	};
#endif
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UPEDirectoryWatcher_Statics::NewProp_OnChanged_MetaData[] = {
		{ "ModuleRelativePath", "Public/PEDirectoryWatcher.h" },
	};
#endif
	const UE4CodeGen_Private::FMulticastDelegatePropertyParams Z_Construct_UClass_UPEDirectoryWatcher_Statics::NewProp_OnChanged = { "OnChanged", nullptr, (EPropertyFlags)0x0010000010080000, UE4CodeGen_Private::EPropertyGenFlags::InlineMulticastDelegate, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(UPEDirectoryWatcher, OnChanged), Z_Construct_UDelegateFunction_PuertsEditor_DirectoryWatcherCallback__DelegateSignature, METADATA_PARAMS(Z_Construct_UClass_UPEDirectoryWatcher_Statics::NewProp_OnChanged_MetaData, UE_ARRAY_COUNT(Z_Construct_UClass_UPEDirectoryWatcher_Statics::NewProp_OnChanged_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UClass_UPEDirectoryWatcher_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UClass_UPEDirectoryWatcher_Statics::NewProp_OnChanged,
	};
	const FCppClassTypeInfoStatic Z_Construct_UClass_UPEDirectoryWatcher_Statics::StaticCppClassTypeInfo = {
		TCppClassTypeTraits<UPEDirectoryWatcher>::IsAbstract,
	};
	const UE4CodeGen_Private::FClassParams Z_Construct_UClass_UPEDirectoryWatcher_Statics::ClassParams = {
		&UPEDirectoryWatcher::StaticClass,
		nullptr,
		&StaticCppClassTypeInfo,
		DependentSingletons,
		FuncInfo,
		Z_Construct_UClass_UPEDirectoryWatcher_Statics::PropPointers,
		nullptr,
		UE_ARRAY_COUNT(DependentSingletons),
		UE_ARRAY_COUNT(FuncInfo),
		UE_ARRAY_COUNT(Z_Construct_UClass_UPEDirectoryWatcher_Statics::PropPointers),
		0,
		0x009000A0u,
		METADATA_PARAMS(Z_Construct_UClass_UPEDirectoryWatcher_Statics::Class_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UClass_UPEDirectoryWatcher_Statics::Class_MetaDataParams))
	};
	UClass* Z_Construct_UClass_UPEDirectoryWatcher()
	{
		static UClass* OuterClass = nullptr;
		if (!OuterClass)
		{
			UE4CodeGen_Private::ConstructUClass(OuterClass, Z_Construct_UClass_UPEDirectoryWatcher_Statics::ClassParams);
		}
		return OuterClass;
	}
	IMPLEMENT_CLASS(UPEDirectoryWatcher, 4190462202);
	template<> PUERTSEDITOR_API UClass* StaticClass<UPEDirectoryWatcher>()
	{
		return UPEDirectoryWatcher::StaticClass();
	}
	static FCompiledInDefer Z_CompiledInDefer_UClass_UPEDirectoryWatcher(Z_Construct_UClass_UPEDirectoryWatcher, &UPEDirectoryWatcher::StaticClass, TEXT("/Script/PuertsEditor"), TEXT("UPEDirectoryWatcher"), false, nullptr, nullptr, nullptr);
	DEFINE_VTABLE_PTR_HELPER_CTOR(UPEDirectoryWatcher);
PRAGMA_ENABLE_DEPRECATION_WARNINGS
#ifdef _MSC_VER
#pragma warning (pop)
#endif
