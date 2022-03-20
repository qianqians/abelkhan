// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/GeneratedCppIncludes.h"
#include "JsEnv/Private/ContainerMeta.h"
#ifdef _MSC_VER
#pragma warning (push)
#pragma warning (disable : 4883)
#endif
PRAGMA_DISABLE_DEPRECATION_WARNINGS
void EmptyLinkFunctionForGeneratedCodeContainerMeta() {}
// Cross Module References
	JSENV_API UScriptStruct* Z_Construct_UScriptStruct_FPropertyMetaRoot();
	UPackage* Z_Construct_UPackage__Script_JsEnv();
// End Cross Module References
	struct Z_Construct_UScriptStruct_FPropertyMetaRoot_Statics
	{
		struct FPropertyMetaRoot
		{
		};

#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Struct_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FStructParams ReturnStructParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UScriptStruct_FPropertyMetaRoot_Statics::Struct_MetaDataParams[] = {
		{ "ModuleRelativePath", "Private/ContainerMeta.h" },
	};
#endif
	const UE4CodeGen_Private::FStructParams Z_Construct_UScriptStruct_FPropertyMetaRoot_Statics::ReturnStructParams = {
		(UObject* (*)())Z_Construct_UPackage__Script_JsEnv,
		nullptr,
		nullptr,
		"PropertyMetaRoot",
		sizeof(FPropertyMetaRoot),
		alignof(FPropertyMetaRoot),
		nullptr,
		0,
		RF_Public|RF_Transient|RF_MarkAsNative,
		EStructFlags(0x00000008),
		METADATA_PARAMS(Z_Construct_UScriptStruct_FPropertyMetaRoot_Statics::Struct_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UScriptStruct_FPropertyMetaRoot_Statics::Struct_MetaDataParams))
	};
	UScriptStruct* Z_Construct_UScriptStruct_FPropertyMetaRoot()
	{
#if WITH_HOT_RELOAD
		extern uint32 Get_Z_Construct_UScriptStruct_FPropertyMetaRoot_Hash();
		UPackage* Outer = Z_Construct_UPackage__Script_JsEnv();
		static UScriptStruct* ReturnStruct = FindExistingStructIfHotReloadOrDynamic(Outer, TEXT("PropertyMetaRoot"), sizeof(Z_Construct_UScriptStruct_FPropertyMetaRoot_Statics::FPropertyMetaRoot), Get_Z_Construct_UScriptStruct_FPropertyMetaRoot_Hash(), false);
#else
		static UScriptStruct* ReturnStruct = nullptr;
#endif
		if (!ReturnStruct)
		{
			UE4CodeGen_Private::ConstructUScriptStruct(ReturnStruct, Z_Construct_UScriptStruct_FPropertyMetaRoot_Statics::ReturnStructParams);
		}
		return ReturnStruct;
	}
	uint32 Get_Z_Construct_UScriptStruct_FPropertyMetaRoot_Hash() { return 547572625U; }
PRAGMA_ENABLE_DEPRECATION_WARNINGS
#ifdef _MSC_VER
#pragma warning (pop)
#endif
