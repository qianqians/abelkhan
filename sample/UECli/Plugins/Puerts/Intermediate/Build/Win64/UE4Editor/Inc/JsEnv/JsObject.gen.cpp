// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/GeneratedCppIncludes.h"
#include "JsEnv/Public/JsObject.h"
#ifdef _MSC_VER
#pragma warning (push)
#pragma warning (disable : 4883)
#endif
PRAGMA_DISABLE_DEPRECATION_WARNINGS
void EmptyLinkFunctionForGeneratedCodeJsObject() {}
// Cross Module References
	JSENV_API UScriptStruct* Z_Construct_UScriptStruct_FJsObject();
	UPackage* Z_Construct_UPackage__Script_JsEnv();
// End Cross Module References
class UScriptStruct* FJsObject::StaticStruct()
{
	static class UScriptStruct* Singleton = NULL;
	if (!Singleton)
	{
		extern JSENV_API uint32 Get_Z_Construct_UScriptStruct_FJsObject_Hash();
		Singleton = GetStaticStruct(Z_Construct_UScriptStruct_FJsObject, Z_Construct_UPackage__Script_JsEnv(), TEXT("JsObject"), sizeof(FJsObject), Get_Z_Construct_UScriptStruct_FJsObject_Hash());
	}
	return Singleton;
}
template<> JSENV_API UScriptStruct* StaticStruct<FJsObject>()
{
	return FJsObject::StaticStruct();
}
static FCompiledInDeferStruct Z_CompiledInDeferStruct_UScriptStruct_FJsObject(FJsObject::StaticStruct, TEXT("/Script/JsEnv"), TEXT("JsObject"), false, nullptr, nullptr);
static struct FScriptStruct_JsEnv_StaticRegisterNativesFJsObject
{
	FScriptStruct_JsEnv_StaticRegisterNativesFJsObject()
	{
		UScriptStruct::DeferCppStructOps<FJsObject>(FName(TEXT("JsObject")));
	}
} ScriptStruct_JsEnv_StaticRegisterNativesFJsObject;
	struct Z_Construct_UScriptStruct_FJsObject_Statics
	{
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Struct_MetaDataParams[];
#endif
		static void* NewStructOps();
		static const UE4CodeGen_Private::FStructParams ReturnStructParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UScriptStruct_FJsObject_Statics::Struct_MetaDataParams[] = {
		{ "BlueprintType", "true" },
		{ "ModuleRelativePath", "Public/JsObject.h" },
	};
#endif
	void* Z_Construct_UScriptStruct_FJsObject_Statics::NewStructOps()
	{
		return (UScriptStruct::ICppStructOps*)new UScriptStruct::TCppStructOps<FJsObject>();
	}
	const UE4CodeGen_Private::FStructParams Z_Construct_UScriptStruct_FJsObject_Statics::ReturnStructParams = {
		(UObject* (*)())Z_Construct_UPackage__Script_JsEnv,
		nullptr,
		&NewStructOps,
		"JsObject",
		sizeof(FJsObject),
		alignof(FJsObject),
		nullptr,
		0,
		RF_Public|RF_Transient|RF_MarkAsNative,
		EStructFlags(0x00000001),
		METADATA_PARAMS(Z_Construct_UScriptStruct_FJsObject_Statics::Struct_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UScriptStruct_FJsObject_Statics::Struct_MetaDataParams))
	};
	UScriptStruct* Z_Construct_UScriptStruct_FJsObject()
	{
#if WITH_HOT_RELOAD
		extern uint32 Get_Z_Construct_UScriptStruct_FJsObject_Hash();
		UPackage* Outer = Z_Construct_UPackage__Script_JsEnv();
		static UScriptStruct* ReturnStruct = FindExistingStructIfHotReloadOrDynamic(Outer, TEXT("JsObject"), sizeof(FJsObject), Get_Z_Construct_UScriptStruct_FJsObject_Hash(), false);
#else
		static UScriptStruct* ReturnStruct = nullptr;
#endif
		if (!ReturnStruct)
		{
			UE4CodeGen_Private::ConstructUScriptStruct(ReturnStruct, Z_Construct_UScriptStruct_FJsObject_Statics::ReturnStructParams);
		}
		return ReturnStruct;
	}
	uint32 Get_Z_Construct_UScriptStruct_FJsObject_Hash() { return 869385427U; }
PRAGMA_ENABLE_DEPRECATION_WARNINGS
#ifdef _MSC_VER
#pragma warning (pop)
#endif
