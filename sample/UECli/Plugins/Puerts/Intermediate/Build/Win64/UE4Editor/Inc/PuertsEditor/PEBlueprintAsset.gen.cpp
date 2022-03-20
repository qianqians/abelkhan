// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/GeneratedCppIncludes.h"
#include "PuertsEditor/Public/PEBlueprintAsset.h"
#ifdef _MSC_VER
#pragma warning (push)
#pragma warning (disable : 4883)
#endif
PRAGMA_DISABLE_DEPRECATION_WARNINGS
void EmptyLinkFunctionForGeneratedCodePEBlueprintAsset() {}
// Cross Module References
	PUERTSEDITOR_API UScriptStruct* Z_Construct_UScriptStruct_FPEGraphPinType();
	UPackage* Z_Construct_UPackage__Script_PuertsEditor();
	COREUOBJECT_API UClass* Z_Construct_UClass_UObject_NoRegister();
	PUERTSEDITOR_API UScriptStruct* Z_Construct_UScriptStruct_FPEGraphTerminalType();
	PUERTSEDITOR_API UClass* Z_Construct_UClass_UPEBlueprintAsset_NoRegister();
	PUERTSEDITOR_API UClass* Z_Construct_UClass_UPEBlueprintAsset();
	COREUOBJECT_API UClass* Z_Construct_UClass_UObject();
	PUERTSEDITOR_API UClass* Z_Construct_UClass_UPEFunctionMetaData_NoRegister();
	PUERTSEDITOR_API UClass* Z_Construct_UClass_UPEPropertyMetaData_NoRegister();
	PUERTSEDITOR_API UClass* Z_Construct_UClass_UPEParamMetaData_NoRegister();
	COREUOBJECT_API UClass* Z_Construct_UClass_UClass();
	PUERTSEDITOR_API UClass* Z_Construct_UClass_UPEClassMetaData_NoRegister();
	ENGINE_API UClass* Z_Construct_UClass_UBlueprint_NoRegister();
	COREUOBJECT_API UClass* Z_Construct_UClass_UPackage();
// End Cross Module References
class UScriptStruct* FPEGraphPinType::StaticStruct()
{
	static class UScriptStruct* Singleton = NULL;
	if (!Singleton)
	{
		extern PUERTSEDITOR_API uint32 Get_Z_Construct_UScriptStruct_FPEGraphPinType_Hash();
		Singleton = GetStaticStruct(Z_Construct_UScriptStruct_FPEGraphPinType, Z_Construct_UPackage__Script_PuertsEditor(), TEXT("PEGraphPinType"), sizeof(FPEGraphPinType), Get_Z_Construct_UScriptStruct_FPEGraphPinType_Hash());
	}
	return Singleton;
}
template<> PUERTSEDITOR_API UScriptStruct* StaticStruct<FPEGraphPinType>()
{
	return FPEGraphPinType::StaticStruct();
}
static FCompiledInDeferStruct Z_CompiledInDeferStruct_UScriptStruct_FPEGraphPinType(FPEGraphPinType::StaticStruct, TEXT("/Script/PuertsEditor"), TEXT("PEGraphPinType"), false, nullptr, nullptr);
static struct FScriptStruct_PuertsEditor_StaticRegisterNativesFPEGraphPinType
{
	FScriptStruct_PuertsEditor_StaticRegisterNativesFPEGraphPinType()
	{
		UScriptStruct::DeferCppStructOps<FPEGraphPinType>(FName(TEXT("PEGraphPinType")));
	}
} ScriptStruct_PuertsEditor_StaticRegisterNativesFPEGraphPinType;
	struct Z_Construct_UScriptStruct_FPEGraphPinType_Statics
	{
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Struct_MetaDataParams[];
#endif
		static void* NewStructOps();
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_PinCategory_MetaData[];
#endif
		static const UE4CodeGen_Private::FNamePropertyParams NewProp_PinCategory;
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_PinSubCategoryObject_MetaData[];
#endif
		static const UE4CodeGen_Private::FObjectPropertyParams NewProp_PinSubCategoryObject;
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_PinContainerType_MetaData[];
#endif
		static const UE4CodeGen_Private::FUnsizedIntPropertyParams NewProp_PinContainerType;
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_bIsReference_MetaData[];
#endif
		static void NewProp_bIsReference_SetBit(void* Obj);
		static const UE4CodeGen_Private::FBoolPropertyParams NewProp_bIsReference;
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_bIn_MetaData[];
#endif
		static void NewProp_bIn_SetBit(void* Obj);
		static const UE4CodeGen_Private::FBoolPropertyParams NewProp_bIn;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
		static const UE4CodeGen_Private::FStructParams ReturnStructParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UScriptStruct_FPEGraphPinType_Statics::Struct_MetaDataParams[] = {
		{ "BlueprintType", "true" },
		{ "ModuleRelativePath", "Public/PEBlueprintAsset.h" },
	};
#endif
	void* Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewStructOps()
	{
		return (UScriptStruct::ICppStructOps*)new UScriptStruct::TCppStructOps<FPEGraphPinType>();
	}
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_PinCategory_MetaData[] = {
		{ "Category", "PEBlueprintAsset" },
		{ "ModuleRelativePath", "Public/PEBlueprintAsset.h" },
	};
#endif
	const UE4CodeGen_Private::FNamePropertyParams Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_PinCategory = { "PinCategory", nullptr, (EPropertyFlags)0x0010000000000005, UE4CodeGen_Private::EPropertyGenFlags::Name, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(FPEGraphPinType, PinCategory), METADATA_PARAMS(Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_PinCategory_MetaData, UE_ARRAY_COUNT(Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_PinCategory_MetaData)) };
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_PinSubCategoryObject_MetaData[] = {
		{ "Category", "PEBlueprintAsset" },
		{ "ModuleRelativePath", "Public/PEBlueprintAsset.h" },
	};
#endif
	const UE4CodeGen_Private::FObjectPropertyParams Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_PinSubCategoryObject = { "PinSubCategoryObject", nullptr, (EPropertyFlags)0x0010000000000005, UE4CodeGen_Private::EPropertyGenFlags::Object, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(FPEGraphPinType, PinSubCategoryObject), Z_Construct_UClass_UObject_NoRegister, METADATA_PARAMS(Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_PinSubCategoryObject_MetaData, UE_ARRAY_COUNT(Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_PinSubCategoryObject_MetaData)) };
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_PinContainerType_MetaData[] = {
		{ "Category", "PEBlueprintAsset" },
		{ "ModuleRelativePath", "Public/PEBlueprintAsset.h" },
	};
#endif
	const UE4CodeGen_Private::FUnsizedIntPropertyParams Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_PinContainerType = { "PinContainerType", nullptr, (EPropertyFlags)0x0010000000000005, UE4CodeGen_Private::EPropertyGenFlags::Int, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(FPEGraphPinType, PinContainerType), METADATA_PARAMS(Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_PinContainerType_MetaData, UE_ARRAY_COUNT(Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_PinContainerType_MetaData)) };
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_bIsReference_MetaData[] = {
		{ "Category", "PEBlueprintAsset" },
		{ "ModuleRelativePath", "Public/PEBlueprintAsset.h" },
	};
#endif
	void Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_bIsReference_SetBit(void* Obj)
	{
		((FPEGraphPinType*)Obj)->bIsReference = 1;
	}
	const UE4CodeGen_Private::FBoolPropertyParams Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_bIsReference = { "bIsReference", nullptr, (EPropertyFlags)0x0010000000000005, UE4CodeGen_Private::EPropertyGenFlags::Bool | UE4CodeGen_Private::EPropertyGenFlags::NativeBool, RF_Public|RF_Transient|RF_MarkAsNative, 1, sizeof(bool), sizeof(FPEGraphPinType), &Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_bIsReference_SetBit, METADATA_PARAMS(Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_bIsReference_MetaData, UE_ARRAY_COUNT(Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_bIsReference_MetaData)) };
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_bIn_MetaData[] = {
		{ "Category", "PEBlueprintAsset" },
		{ "ModuleRelativePath", "Public/PEBlueprintAsset.h" },
	};
#endif
	void Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_bIn_SetBit(void* Obj)
	{
		((FPEGraphPinType*)Obj)->bIn = 1;
	}
	const UE4CodeGen_Private::FBoolPropertyParams Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_bIn = { "bIn", nullptr, (EPropertyFlags)0x0010000000000005, UE4CodeGen_Private::EPropertyGenFlags::Bool | UE4CodeGen_Private::EPropertyGenFlags::NativeBool, RF_Public|RF_Transient|RF_MarkAsNative, 1, sizeof(bool), sizeof(FPEGraphPinType), &Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_bIn_SetBit, METADATA_PARAMS(Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_bIn_MetaData, UE_ARRAY_COUNT(Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_bIn_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UScriptStruct_FPEGraphPinType_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_PinCategory,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_PinSubCategoryObject,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_PinContainerType,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_bIsReference,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UScriptStruct_FPEGraphPinType_Statics::NewProp_bIn,
	};
	const UE4CodeGen_Private::FStructParams Z_Construct_UScriptStruct_FPEGraphPinType_Statics::ReturnStructParams = {
		(UObject* (*)())Z_Construct_UPackage__Script_PuertsEditor,
		nullptr,
		&NewStructOps,
		"PEGraphPinType",
		sizeof(FPEGraphPinType),
		alignof(FPEGraphPinType),
		Z_Construct_UScriptStruct_FPEGraphPinType_Statics::PropPointers,
		UE_ARRAY_COUNT(Z_Construct_UScriptStruct_FPEGraphPinType_Statics::PropPointers),
		RF_Public|RF_Transient|RF_MarkAsNative,
		EStructFlags(0x00000001),
		METADATA_PARAMS(Z_Construct_UScriptStruct_FPEGraphPinType_Statics::Struct_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UScriptStruct_FPEGraphPinType_Statics::Struct_MetaDataParams))
	};
	UScriptStruct* Z_Construct_UScriptStruct_FPEGraphPinType()
	{
#if WITH_HOT_RELOAD
		extern uint32 Get_Z_Construct_UScriptStruct_FPEGraphPinType_Hash();
		UPackage* Outer = Z_Construct_UPackage__Script_PuertsEditor();
		static UScriptStruct* ReturnStruct = FindExistingStructIfHotReloadOrDynamic(Outer, TEXT("PEGraphPinType"), sizeof(FPEGraphPinType), Get_Z_Construct_UScriptStruct_FPEGraphPinType_Hash(), false);
#else
		static UScriptStruct* ReturnStruct = nullptr;
#endif
		if (!ReturnStruct)
		{
			UE4CodeGen_Private::ConstructUScriptStruct(ReturnStruct, Z_Construct_UScriptStruct_FPEGraphPinType_Statics::ReturnStructParams);
		}
		return ReturnStruct;
	}
	uint32 Get_Z_Construct_UScriptStruct_FPEGraphPinType_Hash() { return 247176218U; }
class UScriptStruct* FPEGraphTerminalType::StaticStruct()
{
	static class UScriptStruct* Singleton = NULL;
	if (!Singleton)
	{
		extern PUERTSEDITOR_API uint32 Get_Z_Construct_UScriptStruct_FPEGraphTerminalType_Hash();
		Singleton = GetStaticStruct(Z_Construct_UScriptStruct_FPEGraphTerminalType, Z_Construct_UPackage__Script_PuertsEditor(), TEXT("PEGraphTerminalType"), sizeof(FPEGraphTerminalType), Get_Z_Construct_UScriptStruct_FPEGraphTerminalType_Hash());
	}
	return Singleton;
}
template<> PUERTSEDITOR_API UScriptStruct* StaticStruct<FPEGraphTerminalType>()
{
	return FPEGraphTerminalType::StaticStruct();
}
static FCompiledInDeferStruct Z_CompiledInDeferStruct_UScriptStruct_FPEGraphTerminalType(FPEGraphTerminalType::StaticStruct, TEXT("/Script/PuertsEditor"), TEXT("PEGraphTerminalType"), false, nullptr, nullptr);
static struct FScriptStruct_PuertsEditor_StaticRegisterNativesFPEGraphTerminalType
{
	FScriptStruct_PuertsEditor_StaticRegisterNativesFPEGraphTerminalType()
	{
		UScriptStruct::DeferCppStructOps<FPEGraphTerminalType>(FName(TEXT("PEGraphTerminalType")));
	}
} ScriptStruct_PuertsEditor_StaticRegisterNativesFPEGraphTerminalType;
	struct Z_Construct_UScriptStruct_FPEGraphTerminalType_Statics
	{
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Struct_MetaDataParams[];
#endif
		static void* NewStructOps();
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_PinCategory_MetaData[];
#endif
		static const UE4CodeGen_Private::FNamePropertyParams NewProp_PinCategory;
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_PinSubCategoryObject_MetaData[];
#endif
		static const UE4CodeGen_Private::FObjectPropertyParams NewProp_PinSubCategoryObject;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
		static const UE4CodeGen_Private::FStructParams ReturnStructParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UScriptStruct_FPEGraphTerminalType_Statics::Struct_MetaDataParams[] = {
		{ "BlueprintType", "true" },
		{ "ModuleRelativePath", "Public/PEBlueprintAsset.h" },
	};
#endif
	void* Z_Construct_UScriptStruct_FPEGraphTerminalType_Statics::NewStructOps()
	{
		return (UScriptStruct::ICppStructOps*)new UScriptStruct::TCppStructOps<FPEGraphTerminalType>();
	}
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UScriptStruct_FPEGraphTerminalType_Statics::NewProp_PinCategory_MetaData[] = {
		{ "Category", "PEBlueprintAsset" },
		{ "ModuleRelativePath", "Public/PEBlueprintAsset.h" },
	};
#endif
	const UE4CodeGen_Private::FNamePropertyParams Z_Construct_UScriptStruct_FPEGraphTerminalType_Statics::NewProp_PinCategory = { "PinCategory", nullptr, (EPropertyFlags)0x0010000000000005, UE4CodeGen_Private::EPropertyGenFlags::Name, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(FPEGraphTerminalType, PinCategory), METADATA_PARAMS(Z_Construct_UScriptStruct_FPEGraphTerminalType_Statics::NewProp_PinCategory_MetaData, UE_ARRAY_COUNT(Z_Construct_UScriptStruct_FPEGraphTerminalType_Statics::NewProp_PinCategory_MetaData)) };
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UScriptStruct_FPEGraphTerminalType_Statics::NewProp_PinSubCategoryObject_MetaData[] = {
		{ "Category", "PEBlueprintAsset" },
		{ "ModuleRelativePath", "Public/PEBlueprintAsset.h" },
	};
#endif
	const UE4CodeGen_Private::FObjectPropertyParams Z_Construct_UScriptStruct_FPEGraphTerminalType_Statics::NewProp_PinSubCategoryObject = { "PinSubCategoryObject", nullptr, (EPropertyFlags)0x0010000000000005, UE4CodeGen_Private::EPropertyGenFlags::Object, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(FPEGraphTerminalType, PinSubCategoryObject), Z_Construct_UClass_UObject_NoRegister, METADATA_PARAMS(Z_Construct_UScriptStruct_FPEGraphTerminalType_Statics::NewProp_PinSubCategoryObject_MetaData, UE_ARRAY_COUNT(Z_Construct_UScriptStruct_FPEGraphTerminalType_Statics::NewProp_PinSubCategoryObject_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UScriptStruct_FPEGraphTerminalType_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UScriptStruct_FPEGraphTerminalType_Statics::NewProp_PinCategory,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UScriptStruct_FPEGraphTerminalType_Statics::NewProp_PinSubCategoryObject,
	};
	const UE4CodeGen_Private::FStructParams Z_Construct_UScriptStruct_FPEGraphTerminalType_Statics::ReturnStructParams = {
		(UObject* (*)())Z_Construct_UPackage__Script_PuertsEditor,
		nullptr,
		&NewStructOps,
		"PEGraphTerminalType",
		sizeof(FPEGraphTerminalType),
		alignof(FPEGraphTerminalType),
		Z_Construct_UScriptStruct_FPEGraphTerminalType_Statics::PropPointers,
		UE_ARRAY_COUNT(Z_Construct_UScriptStruct_FPEGraphTerminalType_Statics::PropPointers),
		RF_Public|RF_Transient|RF_MarkAsNative,
		EStructFlags(0x00000001),
		METADATA_PARAMS(Z_Construct_UScriptStruct_FPEGraphTerminalType_Statics::Struct_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UScriptStruct_FPEGraphTerminalType_Statics::Struct_MetaDataParams))
	};
	UScriptStruct* Z_Construct_UScriptStruct_FPEGraphTerminalType()
	{
#if WITH_HOT_RELOAD
		extern uint32 Get_Z_Construct_UScriptStruct_FPEGraphTerminalType_Hash();
		UPackage* Outer = Z_Construct_UPackage__Script_PuertsEditor();
		static UScriptStruct* ReturnStruct = FindExistingStructIfHotReloadOrDynamic(Outer, TEXT("PEGraphTerminalType"), sizeof(FPEGraphTerminalType), Get_Z_Construct_UScriptStruct_FPEGraphTerminalType_Hash(), false);
#else
		static UScriptStruct* ReturnStruct = nullptr;
#endif
		if (!ReturnStruct)
		{
			UE4CodeGen_Private::ConstructUScriptStruct(ReturnStruct, Z_Construct_UScriptStruct_FPEGraphTerminalType_Statics::ReturnStructParams);
		}
		return ReturnStruct;
	}
	uint32 Get_Z_Construct_UScriptStruct_FPEGraphTerminalType_Hash() { return 3002436076U; }
	DEFINE_FUNCTION(UPEBlueprintAsset::execSave)
	{
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->Save();
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEBlueprintAsset::execRemoveNotExistedMemberVariable)
	{
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->RemoveNotExistedMemberVariable();
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEBlueprintAsset::execAddMemberVariableWithMetaData)
	{
		P_GET_PROPERTY(FNameProperty,Z_Param_InNewVarName);
		P_GET_STRUCT(FPEGraphPinType,Z_Param_InGraphPinType);
		P_GET_STRUCT(FPEGraphTerminalType,Z_Param_InPinValueType);
		P_GET_PROPERTY(FIntProperty,Z_Param_InLFlags);
		P_GET_PROPERTY(FIntProperty,Z_Param_InHFLags);
		P_GET_PROPERTY(FIntProperty,Z_Param_InLifetimeCondition);
		P_GET_OBJECT(UPEPropertyMetaData,Z_Param_InMetaData);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->AddMemberVariableWithMetaData(Z_Param_InNewVarName,Z_Param_InGraphPinType,Z_Param_InPinValueType,Z_Param_InLFlags,Z_Param_InHFLags,Z_Param_InLifetimeCondition,Z_Param_InMetaData);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEBlueprintAsset::execAddMemberVariable)
	{
		P_GET_PROPERTY(FNameProperty,Z_Param_NewVarName);
		P_GET_STRUCT(FPEGraphPinType,Z_Param_InGraphPinType);
		P_GET_STRUCT(FPEGraphTerminalType,Z_Param_InPinValueType);
		P_GET_PROPERTY(FIntProperty,Z_Param_InLFlags);
		P_GET_PROPERTY(FIntProperty,Z_Param_InHFlags);
		P_GET_PROPERTY(FIntProperty,Z_Param_InLifetimeCondition);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->AddMemberVariable(Z_Param_NewVarName,Z_Param_InGraphPinType,Z_Param_InPinValueType,Z_Param_InLFlags,Z_Param_InHFlags,Z_Param_InLifetimeCondition);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEBlueprintAsset::execRemoveNotExistedFunction)
	{
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->RemoveNotExistedFunction();
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEBlueprintAsset::execAddFunctionWithMetaData)
	{
		P_GET_PROPERTY(FNameProperty,Z_Param_InName);
		P_GET_UBOOL(Z_Param_IsVoid);
		P_GET_STRUCT(FPEGraphPinType,Z_Param_InGraphPinType);
		P_GET_STRUCT(FPEGraphTerminalType,Z_Param_InPinValueType);
		P_GET_PROPERTY(FIntProperty,Z_Param_InSetFlags);
		P_GET_PROPERTY(FIntProperty,Z_Param_InClearFlags);
		P_GET_OBJECT(UPEFunctionMetaData,Z_Param_InMetaData);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->AddFunctionWithMetaData(Z_Param_InName,Z_Param_IsVoid,Z_Param_InGraphPinType,Z_Param_InPinValueType,Z_Param_InSetFlags,Z_Param_InClearFlags,Z_Param_InMetaData);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEBlueprintAsset::execAddFunction)
	{
		P_GET_PROPERTY(FNameProperty,Z_Param_InName);
		P_GET_UBOOL(Z_Param_IsVoid);
		P_GET_STRUCT(FPEGraphPinType,Z_Param_InGraphPinType);
		P_GET_STRUCT(FPEGraphTerminalType,Z_Param_InPinValueType);
		P_GET_PROPERTY(FIntProperty,Z_Param_InSetFlags);
		P_GET_PROPERTY(FIntProperty,Z_Param_InClearFlags);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->AddFunction(Z_Param_InName,Z_Param_IsVoid,Z_Param_InGraphPinType,Z_Param_InPinValueType,Z_Param_InSetFlags,Z_Param_InClearFlags);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEBlueprintAsset::execClearParameter)
	{
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->ClearParameter();
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEBlueprintAsset::execAddParameterWithMetaData)
	{
		P_GET_PROPERTY(FNameProperty,Z_Param_InParameterName);
		P_GET_STRUCT(FPEGraphPinType,Z_Param_InGraphPinType);
		P_GET_STRUCT(FPEGraphTerminalType,Z_Param_InPinValueType);
		P_GET_OBJECT(UPEParamMetaData,Z_Param_InMetaData);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->AddParameterWithMetaData(Z_Param_InParameterName,Z_Param_InGraphPinType,Z_Param_InPinValueType,Z_Param_InMetaData);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEBlueprintAsset::execAddParameter)
	{
		P_GET_PROPERTY(FNameProperty,Z_Param_InParameterName);
		P_GET_STRUCT(FPEGraphPinType,Z_Param_InGraphPinType);
		P_GET_STRUCT(FPEGraphTerminalType,Z_Param_InPinValueType);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->AddParameter(Z_Param_InParameterName,Z_Param_InGraphPinType,Z_Param_InPinValueType);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEBlueprintAsset::execLoadOrCreateWithMetaData)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_InName);
		P_GET_PROPERTY(FStrProperty,Z_Param_InPath);
		P_GET_OBJECT(UClass,Z_Param_InParentClass);
		P_GET_PROPERTY(FIntProperty,Z_Param_InSetFlags);
		P_GET_PROPERTY(FIntProperty,Z_Param_InClearFlags);
		P_GET_OBJECT(UPEClassMetaData,Z_Param_InMetaData);
		P_FINISH;
		P_NATIVE_BEGIN;
		*(bool*)Z_Param__Result=P_THIS->LoadOrCreateWithMetaData(Z_Param_InName,Z_Param_InPath,Z_Param_InParentClass,Z_Param_InSetFlags,Z_Param_InClearFlags,Z_Param_InMetaData);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEBlueprintAsset::execLoadOrCreate)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_InName);
		P_GET_PROPERTY(FStrProperty,Z_Param_InPath);
		P_GET_OBJECT(UClass,Z_Param_ParentClass);
		P_GET_PROPERTY(FIntProperty,Z_Param_InSetFlags);
		P_GET_PROPERTY(FIntProperty,Z_Param_InClearFlags);
		P_FINISH;
		P_NATIVE_BEGIN;
		*(bool*)Z_Param__Result=P_THIS->LoadOrCreate(Z_Param_InName,Z_Param_InPath,Z_Param_ParentClass,Z_Param_InSetFlags,Z_Param_InClearFlags);
		P_NATIVE_END;
	}
	void UPEBlueprintAsset::StaticRegisterNativesUPEBlueprintAsset()
	{
		UClass* Class = UPEBlueprintAsset::StaticClass();
		static const FNameNativePtrPair Funcs[] = {
			{ "AddFunction", &UPEBlueprintAsset::execAddFunction },
			{ "AddFunctionWithMetaData", &UPEBlueprintAsset::execAddFunctionWithMetaData },
			{ "AddMemberVariable", &UPEBlueprintAsset::execAddMemberVariable },
			{ "AddMemberVariableWithMetaData", &UPEBlueprintAsset::execAddMemberVariableWithMetaData },
			{ "AddParameter", &UPEBlueprintAsset::execAddParameter },
			{ "AddParameterWithMetaData", &UPEBlueprintAsset::execAddParameterWithMetaData },
			{ "ClearParameter", &UPEBlueprintAsset::execClearParameter },
			{ "LoadOrCreate", &UPEBlueprintAsset::execLoadOrCreate },
			{ "LoadOrCreateWithMetaData", &UPEBlueprintAsset::execLoadOrCreateWithMetaData },
			{ "RemoveNotExistedFunction", &UPEBlueprintAsset::execRemoveNotExistedFunction },
			{ "RemoveNotExistedMemberVariable", &UPEBlueprintAsset::execRemoveNotExistedMemberVariable },
			{ "Save", &UPEBlueprintAsset::execSave },
		};
		FNativeFunctionRegistrar::RegisterFunctions(Class, Funcs, UE_ARRAY_COUNT(Funcs));
	}
	struct Z_Construct_UFunction_UPEBlueprintAsset_AddFunction_Statics
	{
		struct PEBlueprintAsset_eventAddFunction_Parms
		{
			FName InName;
			bool IsVoid;
			FPEGraphPinType InGraphPinType;
			FPEGraphTerminalType InPinValueType;
			int32 InSetFlags;
			int32 InClearFlags;
		};
		static const UE4CodeGen_Private::FNamePropertyParams NewProp_InName;
		static void NewProp_IsVoid_SetBit(void* Obj);
		static const UE4CodeGen_Private::FBoolPropertyParams NewProp_IsVoid;
		static const UE4CodeGen_Private::FStructPropertyParams NewProp_InGraphPinType;
		static const UE4CodeGen_Private::FStructPropertyParams NewProp_InPinValueType;
		static const UE4CodeGen_Private::FIntPropertyParams NewProp_InSetFlags;
		static const UE4CodeGen_Private::FIntPropertyParams NewProp_InClearFlags;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
	const UE4CodeGen_Private::FNamePropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddFunction_Statics::NewProp_InName = { "InName", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Name, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddFunction_Parms, InName), METADATA_PARAMS(nullptr, 0) };
	void Z_Construct_UFunction_UPEBlueprintAsset_AddFunction_Statics::NewProp_IsVoid_SetBit(void* Obj)
	{
		((PEBlueprintAsset_eventAddFunction_Parms*)Obj)->IsVoid = 1;
	}
	const UE4CodeGen_Private::FBoolPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddFunction_Statics::NewProp_IsVoid = { "IsVoid", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Bool | UE4CodeGen_Private::EPropertyGenFlags::NativeBool, RF_Public|RF_Transient|RF_MarkAsNative, 1, sizeof(bool), sizeof(PEBlueprintAsset_eventAddFunction_Parms), &Z_Construct_UFunction_UPEBlueprintAsset_AddFunction_Statics::NewProp_IsVoid_SetBit, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FStructPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddFunction_Statics::NewProp_InGraphPinType = { "InGraphPinType", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Struct, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddFunction_Parms, InGraphPinType), Z_Construct_UScriptStruct_FPEGraphPinType, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FStructPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddFunction_Statics::NewProp_InPinValueType = { "InPinValueType", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Struct, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddFunction_Parms, InPinValueType), Z_Construct_UScriptStruct_FPEGraphTerminalType, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FIntPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddFunction_Statics::NewProp_InSetFlags = { "InSetFlags", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Int, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddFunction_Parms, InSetFlags), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FIntPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddFunction_Statics::NewProp_InClearFlags = { "InClearFlags", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Int, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddFunction_Parms, InClearFlags), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEBlueprintAsset_AddFunction_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddFunction_Statics::NewProp_InName,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddFunction_Statics::NewProp_IsVoid,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddFunction_Statics::NewProp_InGraphPinType,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddFunction_Statics::NewProp_InPinValueType,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddFunction_Statics::NewProp_InSetFlags,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddFunction_Statics::NewProp_InClearFlags,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEBlueprintAsset_AddFunction_Statics::Function_MetaDataParams[] = {
		{ "Category", "PEBlueprintAsset" },
		{ "ModuleRelativePath", "Public/PEBlueprintAsset.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEBlueprintAsset_AddFunction_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEBlueprintAsset, nullptr, "AddFunction", nullptr, nullptr, sizeof(PEBlueprintAsset_eventAddFunction_Parms), Z_Construct_UFunction_UPEBlueprintAsset_AddFunction_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEBlueprintAsset_AddFunction_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEBlueprintAsset_AddFunction_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEBlueprintAsset_AddFunction_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEBlueprintAsset_AddFunction()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEBlueprintAsset_AddFunction_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEBlueprintAsset_AddFunctionWithMetaData_Statics
	{
		struct PEBlueprintAsset_eventAddFunctionWithMetaData_Parms
		{
			FName InName;
			bool IsVoid;
			FPEGraphPinType InGraphPinType;
			FPEGraphTerminalType InPinValueType;
			int32 InSetFlags;
			int32 InClearFlags;
			UPEFunctionMetaData* InMetaData;
		};
		static const UE4CodeGen_Private::FNamePropertyParams NewProp_InName;
		static void NewProp_IsVoid_SetBit(void* Obj);
		static const UE4CodeGen_Private::FBoolPropertyParams NewProp_IsVoid;
		static const UE4CodeGen_Private::FStructPropertyParams NewProp_InGraphPinType;
		static const UE4CodeGen_Private::FStructPropertyParams NewProp_InPinValueType;
		static const UE4CodeGen_Private::FIntPropertyParams NewProp_InSetFlags;
		static const UE4CodeGen_Private::FIntPropertyParams NewProp_InClearFlags;
		static const UE4CodeGen_Private::FObjectPropertyParams NewProp_InMetaData;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
	const UE4CodeGen_Private::FNamePropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddFunctionWithMetaData_Statics::NewProp_InName = { "InName", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Name, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddFunctionWithMetaData_Parms, InName), METADATA_PARAMS(nullptr, 0) };
	void Z_Construct_UFunction_UPEBlueprintAsset_AddFunctionWithMetaData_Statics::NewProp_IsVoid_SetBit(void* Obj)
	{
		((PEBlueprintAsset_eventAddFunctionWithMetaData_Parms*)Obj)->IsVoid = 1;
	}
	const UE4CodeGen_Private::FBoolPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddFunctionWithMetaData_Statics::NewProp_IsVoid = { "IsVoid", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Bool | UE4CodeGen_Private::EPropertyGenFlags::NativeBool, RF_Public|RF_Transient|RF_MarkAsNative, 1, sizeof(bool), sizeof(PEBlueprintAsset_eventAddFunctionWithMetaData_Parms), &Z_Construct_UFunction_UPEBlueprintAsset_AddFunctionWithMetaData_Statics::NewProp_IsVoid_SetBit, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FStructPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddFunctionWithMetaData_Statics::NewProp_InGraphPinType = { "InGraphPinType", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Struct, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddFunctionWithMetaData_Parms, InGraphPinType), Z_Construct_UScriptStruct_FPEGraphPinType, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FStructPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddFunctionWithMetaData_Statics::NewProp_InPinValueType = { "InPinValueType", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Struct, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddFunctionWithMetaData_Parms, InPinValueType), Z_Construct_UScriptStruct_FPEGraphTerminalType, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FIntPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddFunctionWithMetaData_Statics::NewProp_InSetFlags = { "InSetFlags", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Int, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddFunctionWithMetaData_Parms, InSetFlags), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FIntPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddFunctionWithMetaData_Statics::NewProp_InClearFlags = { "InClearFlags", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Int, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddFunctionWithMetaData_Parms, InClearFlags), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FObjectPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddFunctionWithMetaData_Statics::NewProp_InMetaData = { "InMetaData", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Object, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddFunctionWithMetaData_Parms, InMetaData), Z_Construct_UClass_UPEFunctionMetaData_NoRegister, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEBlueprintAsset_AddFunctionWithMetaData_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddFunctionWithMetaData_Statics::NewProp_InName,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddFunctionWithMetaData_Statics::NewProp_IsVoid,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddFunctionWithMetaData_Statics::NewProp_InGraphPinType,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddFunctionWithMetaData_Statics::NewProp_InPinValueType,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddFunctionWithMetaData_Statics::NewProp_InSetFlags,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddFunctionWithMetaData_Statics::NewProp_InClearFlags,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddFunctionWithMetaData_Statics::NewProp_InMetaData,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEBlueprintAsset_AddFunctionWithMetaData_Statics::Function_MetaDataParams[] = {
		{ "Category", "PEBlueprintAsset" },
		{ "Comment", "/**\n     * @brief create the function with given meta data\n     * @param InName\n     * @param IsVoid\n     * @param InGraphPinType\n     * @param InPinValueType\n     * @param InSetFlags\n     * @param InClearFlags\n     * @param InMetaData\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintAsset.h" },
		{ "ToolTip", "@brief create the function with given meta data\n@param InName\n@param IsVoid\n@param InGraphPinType\n@param InPinValueType\n@param InSetFlags\n@param InClearFlags\n@param InMetaData" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEBlueprintAsset_AddFunctionWithMetaData_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEBlueprintAsset, nullptr, "AddFunctionWithMetaData", nullptr, nullptr, sizeof(PEBlueprintAsset_eventAddFunctionWithMetaData_Parms), Z_Construct_UFunction_UPEBlueprintAsset_AddFunctionWithMetaData_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEBlueprintAsset_AddFunctionWithMetaData_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEBlueprintAsset_AddFunctionWithMetaData_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEBlueprintAsset_AddFunctionWithMetaData_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEBlueprintAsset_AddFunctionWithMetaData()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEBlueprintAsset_AddFunctionWithMetaData_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariable_Statics
	{
		struct PEBlueprintAsset_eventAddMemberVariable_Parms
		{
			FName NewVarName;
			FPEGraphPinType InGraphPinType;
			FPEGraphTerminalType InPinValueType;
			int32 InLFlags;
			int32 InHFlags;
			int32 InLifetimeCondition;
		};
		static const UE4CodeGen_Private::FNamePropertyParams NewProp_NewVarName;
		static const UE4CodeGen_Private::FStructPropertyParams NewProp_InGraphPinType;
		static const UE4CodeGen_Private::FStructPropertyParams NewProp_InPinValueType;
		static const UE4CodeGen_Private::FIntPropertyParams NewProp_InLFlags;
		static const UE4CodeGen_Private::FIntPropertyParams NewProp_InHFlags;
		static const UE4CodeGen_Private::FIntPropertyParams NewProp_InLifetimeCondition;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
	const UE4CodeGen_Private::FNamePropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariable_Statics::NewProp_NewVarName = { "NewVarName", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Name, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddMemberVariable_Parms, NewVarName), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FStructPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariable_Statics::NewProp_InGraphPinType = { "InGraphPinType", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Struct, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddMemberVariable_Parms, InGraphPinType), Z_Construct_UScriptStruct_FPEGraphPinType, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FStructPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariable_Statics::NewProp_InPinValueType = { "InPinValueType", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Struct, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddMemberVariable_Parms, InPinValueType), Z_Construct_UScriptStruct_FPEGraphTerminalType, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FIntPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariable_Statics::NewProp_InLFlags = { "InLFlags", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Int, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddMemberVariable_Parms, InLFlags), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FIntPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariable_Statics::NewProp_InHFlags = { "InHFlags", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Int, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddMemberVariable_Parms, InHFlags), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FIntPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariable_Statics::NewProp_InLifetimeCondition = { "InLifetimeCondition", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Int, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddMemberVariable_Parms, InLifetimeCondition), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariable_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariable_Statics::NewProp_NewVarName,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariable_Statics::NewProp_InGraphPinType,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariable_Statics::NewProp_InPinValueType,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariable_Statics::NewProp_InLFlags,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariable_Statics::NewProp_InHFlags,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariable_Statics::NewProp_InLifetimeCondition,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariable_Statics::Function_MetaDataParams[] = {
		{ "Category", "PEBlueprintAsset" },
		{ "ModuleRelativePath", "Public/PEBlueprintAsset.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariable_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEBlueprintAsset, nullptr, "AddMemberVariable", nullptr, nullptr, sizeof(PEBlueprintAsset_eventAddMemberVariable_Parms), Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariable_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariable_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariable_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariable_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariable()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariable_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariableWithMetaData_Statics
	{
		struct PEBlueprintAsset_eventAddMemberVariableWithMetaData_Parms
		{
			FName InNewVarName;
			FPEGraphPinType InGraphPinType;
			FPEGraphTerminalType InPinValueType;
			int32 InLFlags;
			int32 InHFLags;
			int32 InLifetimeCondition;
			UPEPropertyMetaData* InMetaData;
		};
		static const UE4CodeGen_Private::FNamePropertyParams NewProp_InNewVarName;
		static const UE4CodeGen_Private::FStructPropertyParams NewProp_InGraphPinType;
		static const UE4CodeGen_Private::FStructPropertyParams NewProp_InPinValueType;
		static const UE4CodeGen_Private::FIntPropertyParams NewProp_InLFlags;
		static const UE4CodeGen_Private::FIntPropertyParams NewProp_InHFLags;
		static const UE4CodeGen_Private::FIntPropertyParams NewProp_InLifetimeCondition;
		static const UE4CodeGen_Private::FObjectPropertyParams NewProp_InMetaData;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
	const UE4CodeGen_Private::FNamePropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariableWithMetaData_Statics::NewProp_InNewVarName = { "InNewVarName", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Name, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddMemberVariableWithMetaData_Parms, InNewVarName), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FStructPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariableWithMetaData_Statics::NewProp_InGraphPinType = { "InGraphPinType", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Struct, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddMemberVariableWithMetaData_Parms, InGraphPinType), Z_Construct_UScriptStruct_FPEGraphPinType, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FStructPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariableWithMetaData_Statics::NewProp_InPinValueType = { "InPinValueType", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Struct, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddMemberVariableWithMetaData_Parms, InPinValueType), Z_Construct_UScriptStruct_FPEGraphTerminalType, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FIntPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariableWithMetaData_Statics::NewProp_InLFlags = { "InLFlags", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Int, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddMemberVariableWithMetaData_Parms, InLFlags), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FIntPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariableWithMetaData_Statics::NewProp_InHFLags = { "InHFLags", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Int, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddMemberVariableWithMetaData_Parms, InHFLags), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FIntPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariableWithMetaData_Statics::NewProp_InLifetimeCondition = { "InLifetimeCondition", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Int, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddMemberVariableWithMetaData_Parms, InLifetimeCondition), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FObjectPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariableWithMetaData_Statics::NewProp_InMetaData = { "InMetaData", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Object, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddMemberVariableWithMetaData_Parms, InMetaData), Z_Construct_UClass_UPEPropertyMetaData_NoRegister, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariableWithMetaData_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariableWithMetaData_Statics::NewProp_InNewVarName,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariableWithMetaData_Statics::NewProp_InGraphPinType,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariableWithMetaData_Statics::NewProp_InPinValueType,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariableWithMetaData_Statics::NewProp_InLFlags,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariableWithMetaData_Statics::NewProp_InHFLags,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariableWithMetaData_Statics::NewProp_InLifetimeCondition,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariableWithMetaData_Statics::NewProp_InMetaData,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariableWithMetaData_Statics::Function_MetaDataParams[] = {
		{ "Category", "PEBlueprintAsset" },
		{ "Comment", "/**\n     * @brief create the property with given meta data\n     * @param InNewVarName\n     * @param InGraphPinType\n     * @param InPinValueType\n     * @param InLFlags\n     * @param InHFLags\n     * @param InLifetimeCondition\n     * @param InMetaData\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintAsset.h" },
		{ "ToolTip", "@brief create the property with given meta data\n@param InNewVarName\n@param InGraphPinType\n@param InPinValueType\n@param InLFlags\n@param InHFLags\n@param InLifetimeCondition\n@param InMetaData" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariableWithMetaData_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEBlueprintAsset, nullptr, "AddMemberVariableWithMetaData", nullptr, nullptr, sizeof(PEBlueprintAsset_eventAddMemberVariableWithMetaData_Parms), Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariableWithMetaData_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariableWithMetaData_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariableWithMetaData_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariableWithMetaData_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariableWithMetaData()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariableWithMetaData_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEBlueprintAsset_AddParameter_Statics
	{
		struct PEBlueprintAsset_eventAddParameter_Parms
		{
			FName InParameterName;
			FPEGraphPinType InGraphPinType;
			FPEGraphTerminalType InPinValueType;
		};
		static const UE4CodeGen_Private::FNamePropertyParams NewProp_InParameterName;
		static const UE4CodeGen_Private::FStructPropertyParams NewProp_InGraphPinType;
		static const UE4CodeGen_Private::FStructPropertyParams NewProp_InPinValueType;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
	const UE4CodeGen_Private::FNamePropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddParameter_Statics::NewProp_InParameterName = { "InParameterName", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Name, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddParameter_Parms, InParameterName), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FStructPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddParameter_Statics::NewProp_InGraphPinType = { "InGraphPinType", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Struct, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddParameter_Parms, InGraphPinType), Z_Construct_UScriptStruct_FPEGraphPinType, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FStructPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddParameter_Statics::NewProp_InPinValueType = { "InPinValueType", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Struct, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddParameter_Parms, InPinValueType), Z_Construct_UScriptStruct_FPEGraphTerminalType, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEBlueprintAsset_AddParameter_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddParameter_Statics::NewProp_InParameterName,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddParameter_Statics::NewProp_InGraphPinType,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddParameter_Statics::NewProp_InPinValueType,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEBlueprintAsset_AddParameter_Statics::Function_MetaDataParams[] = {
		{ "Category", "PEBlueprintAsset" },
		{ "ModuleRelativePath", "Public/PEBlueprintAsset.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEBlueprintAsset_AddParameter_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEBlueprintAsset, nullptr, "AddParameter", nullptr, nullptr, sizeof(PEBlueprintAsset_eventAddParameter_Parms), Z_Construct_UFunction_UPEBlueprintAsset_AddParameter_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEBlueprintAsset_AddParameter_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEBlueprintAsset_AddParameter_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEBlueprintAsset_AddParameter_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEBlueprintAsset_AddParameter()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEBlueprintAsset_AddParameter_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEBlueprintAsset_AddParameterWithMetaData_Statics
	{
		struct PEBlueprintAsset_eventAddParameterWithMetaData_Parms
		{
			FName InParameterName;
			FPEGraphPinType InGraphPinType;
			FPEGraphTerminalType InPinValueType;
			UPEParamMetaData* InMetaData;
		};
		static const UE4CodeGen_Private::FNamePropertyParams NewProp_InParameterName;
		static const UE4CodeGen_Private::FStructPropertyParams NewProp_InGraphPinType;
		static const UE4CodeGen_Private::FStructPropertyParams NewProp_InPinValueType;
		static const UE4CodeGen_Private::FObjectPropertyParams NewProp_InMetaData;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
	const UE4CodeGen_Private::FNamePropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddParameterWithMetaData_Statics::NewProp_InParameterName = { "InParameterName", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Name, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddParameterWithMetaData_Parms, InParameterName), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FStructPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddParameterWithMetaData_Statics::NewProp_InGraphPinType = { "InGraphPinType", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Struct, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddParameterWithMetaData_Parms, InGraphPinType), Z_Construct_UScriptStruct_FPEGraphPinType, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FStructPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddParameterWithMetaData_Statics::NewProp_InPinValueType = { "InPinValueType", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Struct, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddParameterWithMetaData_Parms, InPinValueType), Z_Construct_UScriptStruct_FPEGraphTerminalType, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FObjectPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_AddParameterWithMetaData_Statics::NewProp_InMetaData = { "InMetaData", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Object, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventAddParameterWithMetaData_Parms, InMetaData), Z_Construct_UClass_UPEParamMetaData_NoRegister, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEBlueprintAsset_AddParameterWithMetaData_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddParameterWithMetaData_Statics::NewProp_InParameterName,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddParameterWithMetaData_Statics::NewProp_InGraphPinType,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddParameterWithMetaData_Statics::NewProp_InPinValueType,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_AddParameterWithMetaData_Statics::NewProp_InMetaData,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEBlueprintAsset_AddParameterWithMetaData_Statics::Function_MetaDataParams[] = {
		{ "Category", "PEBlueprintAsset" },
		{ "Comment", "/**\n     * @brief add parameter with given meta data\n     * @param InParameterName\n     * @param InGraphPinType\n     * @param InPinValueType\n     * @param InMetaData\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintAsset.h" },
		{ "ToolTip", "@brief add parameter with given meta data\n@param InParameterName\n@param InGraphPinType\n@param InPinValueType\n@param InMetaData" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEBlueprintAsset_AddParameterWithMetaData_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEBlueprintAsset, nullptr, "AddParameterWithMetaData", nullptr, nullptr, sizeof(PEBlueprintAsset_eventAddParameterWithMetaData_Parms), Z_Construct_UFunction_UPEBlueprintAsset_AddParameterWithMetaData_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEBlueprintAsset_AddParameterWithMetaData_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEBlueprintAsset_AddParameterWithMetaData_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEBlueprintAsset_AddParameterWithMetaData_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEBlueprintAsset_AddParameterWithMetaData()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEBlueprintAsset_AddParameterWithMetaData_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEBlueprintAsset_ClearParameter_Statics
	{
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEBlueprintAsset_ClearParameter_Statics::Function_MetaDataParams[] = {
		{ "Category", "PEBlueprintAsset" },
		{ "ModuleRelativePath", "Public/PEBlueprintAsset.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEBlueprintAsset_ClearParameter_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEBlueprintAsset, nullptr, "ClearParameter", nullptr, nullptr, 0, nullptr, 0, RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEBlueprintAsset_ClearParameter_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEBlueprintAsset_ClearParameter_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEBlueprintAsset_ClearParameter()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEBlueprintAsset_ClearParameter_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics
	{
		struct PEBlueprintAsset_eventLoadOrCreate_Parms
		{
			FString InName;
			FString InPath;
			UClass* ParentClass;
			int32 InSetFlags;
			int32 InClearFlags;
			bool ReturnValue;
		};
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InName_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InName;
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InPath_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InPath;
		static const UE4CodeGen_Private::FClassPropertyParams NewProp_ParentClass;
		static const UE4CodeGen_Private::FIntPropertyParams NewProp_InSetFlags;
		static const UE4CodeGen_Private::FIntPropertyParams NewProp_InClearFlags;
		static void NewProp_ReturnValue_SetBit(void* Obj);
		static const UE4CodeGen_Private::FBoolPropertyParams NewProp_ReturnValue;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::NewProp_InName_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::NewProp_InName = { "InName", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventLoadOrCreate_Parms, InName), METADATA_PARAMS(Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::NewProp_InName_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::NewProp_InName_MetaData)) };
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::NewProp_InPath_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::NewProp_InPath = { "InPath", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventLoadOrCreate_Parms, InPath), METADATA_PARAMS(Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::NewProp_InPath_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::NewProp_InPath_MetaData)) };
	const UE4CodeGen_Private::FClassPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::NewProp_ParentClass = { "ParentClass", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Class, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventLoadOrCreate_Parms, ParentClass), Z_Construct_UClass_UObject_NoRegister, Z_Construct_UClass_UClass, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FIntPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::NewProp_InSetFlags = { "InSetFlags", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Int, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventLoadOrCreate_Parms, InSetFlags), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FIntPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::NewProp_InClearFlags = { "InClearFlags", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Int, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventLoadOrCreate_Parms, InClearFlags), METADATA_PARAMS(nullptr, 0) };
	void Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::NewProp_ReturnValue_SetBit(void* Obj)
	{
		((PEBlueprintAsset_eventLoadOrCreate_Parms*)Obj)->ReturnValue = 1;
	}
	const UE4CodeGen_Private::FBoolPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::NewProp_ReturnValue = { "ReturnValue", nullptr, (EPropertyFlags)0x0010000000000580, UE4CodeGen_Private::EPropertyGenFlags::Bool | UE4CodeGen_Private::EPropertyGenFlags::NativeBool, RF_Public|RF_Transient|RF_MarkAsNative, 1, sizeof(bool), sizeof(PEBlueprintAsset_eventLoadOrCreate_Parms), &Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::NewProp_ReturnValue_SetBit, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::NewProp_InName,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::NewProp_InPath,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::NewProp_ParentClass,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::NewProp_InSetFlags,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::NewProp_InClearFlags,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::NewProp_ReturnValue,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::Function_MetaDataParams[] = {
		{ "Category", "PEBlueprintAsset" },
		{ "ModuleRelativePath", "Public/PEBlueprintAsset.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEBlueprintAsset, nullptr, "LoadOrCreate", nullptr, nullptr, sizeof(PEBlueprintAsset_eventLoadOrCreate_Parms), Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics
	{
		struct PEBlueprintAsset_eventLoadOrCreateWithMetaData_Parms
		{
			FString InName;
			FString InPath;
			UClass* InParentClass;
			int32 InSetFlags;
			int32 InClearFlags;
			UPEClassMetaData* InMetaData;
			bool ReturnValue;
		};
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InName_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InName;
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InPath_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InPath;
		static const UE4CodeGen_Private::FClassPropertyParams NewProp_InParentClass;
		static const UE4CodeGen_Private::FIntPropertyParams NewProp_InSetFlags;
		static const UE4CodeGen_Private::FIntPropertyParams NewProp_InClearFlags;
		static const UE4CodeGen_Private::FObjectPropertyParams NewProp_InMetaData;
		static void NewProp_ReturnValue_SetBit(void* Obj);
		static const UE4CodeGen_Private::FBoolPropertyParams NewProp_ReturnValue;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::NewProp_InName_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::NewProp_InName = { "InName", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventLoadOrCreateWithMetaData_Parms, InName), METADATA_PARAMS(Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::NewProp_InName_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::NewProp_InName_MetaData)) };
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::NewProp_InPath_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::NewProp_InPath = { "InPath", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventLoadOrCreateWithMetaData_Parms, InPath), METADATA_PARAMS(Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::NewProp_InPath_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::NewProp_InPath_MetaData)) };
	const UE4CodeGen_Private::FClassPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::NewProp_InParentClass = { "InParentClass", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Class, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventLoadOrCreateWithMetaData_Parms, InParentClass), Z_Construct_UClass_UObject_NoRegister, Z_Construct_UClass_UClass, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FIntPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::NewProp_InSetFlags = { "InSetFlags", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Int, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventLoadOrCreateWithMetaData_Parms, InSetFlags), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FIntPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::NewProp_InClearFlags = { "InClearFlags", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Int, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventLoadOrCreateWithMetaData_Parms, InClearFlags), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FObjectPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::NewProp_InMetaData = { "InMetaData", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Object, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEBlueprintAsset_eventLoadOrCreateWithMetaData_Parms, InMetaData), Z_Construct_UClass_UPEClassMetaData_NoRegister, METADATA_PARAMS(nullptr, 0) };
	void Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::NewProp_ReturnValue_SetBit(void* Obj)
	{
		((PEBlueprintAsset_eventLoadOrCreateWithMetaData_Parms*)Obj)->ReturnValue = 1;
	}
	const UE4CodeGen_Private::FBoolPropertyParams Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::NewProp_ReturnValue = { "ReturnValue", nullptr, (EPropertyFlags)0x0010000000000580, UE4CodeGen_Private::EPropertyGenFlags::Bool | UE4CodeGen_Private::EPropertyGenFlags::NativeBool, RF_Public|RF_Transient|RF_MarkAsNative, 1, sizeof(bool), sizeof(PEBlueprintAsset_eventLoadOrCreateWithMetaData_Parms), &Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::NewProp_ReturnValue_SetBit, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::NewProp_InName,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::NewProp_InPath,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::NewProp_InParentClass,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::NewProp_InSetFlags,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::NewProp_InClearFlags,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::NewProp_InMetaData,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::NewProp_ReturnValue,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::Function_MetaDataParams[] = {
		{ "Category", "PEBlueprintAsset" },
		{ "Comment", "/**\n     * @brief create the class with given meta data\n     * @param InName\n     * @param InPath\n     * @param InParentClass\n     * @param InSetFlags\n     * @param InClearFlags\n     * @param InMetaData\n     * @return\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintAsset.h" },
		{ "ToolTip", "@brief create the class with given meta data\n@param InName\n@param InPath\n@param InParentClass\n@param InSetFlags\n@param InClearFlags\n@param InMetaData\n@return" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEBlueprintAsset, nullptr, "LoadOrCreateWithMetaData", nullptr, nullptr, sizeof(PEBlueprintAsset_eventLoadOrCreateWithMetaData_Parms), Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEBlueprintAsset_RemoveNotExistedFunction_Statics
	{
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEBlueprintAsset_RemoveNotExistedFunction_Statics::Function_MetaDataParams[] = {
		{ "Category", "PEBlueprintAsset" },
		{ "ModuleRelativePath", "Public/PEBlueprintAsset.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEBlueprintAsset_RemoveNotExistedFunction_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEBlueprintAsset, nullptr, "RemoveNotExistedFunction", nullptr, nullptr, 0, nullptr, 0, RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEBlueprintAsset_RemoveNotExistedFunction_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEBlueprintAsset_RemoveNotExistedFunction_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEBlueprintAsset_RemoveNotExistedFunction()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEBlueprintAsset_RemoveNotExistedFunction_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEBlueprintAsset_RemoveNotExistedMemberVariable_Statics
	{
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEBlueprintAsset_RemoveNotExistedMemberVariable_Statics::Function_MetaDataParams[] = {
		{ "Category", "PEBlueprintAsset" },
		{ "ModuleRelativePath", "Public/PEBlueprintAsset.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEBlueprintAsset_RemoveNotExistedMemberVariable_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEBlueprintAsset, nullptr, "RemoveNotExistedMemberVariable", nullptr, nullptr, 0, nullptr, 0, RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEBlueprintAsset_RemoveNotExistedMemberVariable_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEBlueprintAsset_RemoveNotExistedMemberVariable_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEBlueprintAsset_RemoveNotExistedMemberVariable()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEBlueprintAsset_RemoveNotExistedMemberVariable_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEBlueprintAsset_Save_Statics
	{
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEBlueprintAsset_Save_Statics::Function_MetaDataParams[] = {
		{ "Category", "PEBlueprintAsset" },
		{ "ModuleRelativePath", "Public/PEBlueprintAsset.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEBlueprintAsset_Save_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEBlueprintAsset, nullptr, "Save", nullptr, nullptr, 0, nullptr, 0, RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEBlueprintAsset_Save_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEBlueprintAsset_Save_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEBlueprintAsset_Save()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEBlueprintAsset_Save_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	UClass* Z_Construct_UClass_UPEBlueprintAsset_NoRegister()
	{
		return UPEBlueprintAsset::StaticClass();
	}
	struct Z_Construct_UClass_UPEBlueprintAsset_Statics
	{
		static UObject* (*const DependentSingletons[])();
		static const FClassFunctionLinkInfo FuncInfo[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Class_MetaDataParams[];
#endif
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_GeneratedClass_MetaData[];
#endif
		static const UE4CodeGen_Private::FClassPropertyParams NewProp_GeneratedClass;
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_Blueprint_MetaData[];
#endif
		static const UE4CodeGen_Private::FObjectPropertyParams NewProp_Blueprint;
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_Package_MetaData[];
#endif
		static const UE4CodeGen_Private::FObjectPropertyParams NewProp_Package;
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_NeedSave_MetaData[];
#endif
		static void NewProp_NeedSave_SetBit(void* Obj);
		static const UE4CodeGen_Private::FBoolPropertyParams NewProp_NeedSave;
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_HasConstructor_MetaData[];
#endif
		static void NewProp_HasConstructor_SetBit(void* Obj);
		static const UE4CodeGen_Private::FBoolPropertyParams NewProp_HasConstructor;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
		static const FCppClassTypeInfoStatic StaticCppClassTypeInfo;
		static const UE4CodeGen_Private::FClassParams ClassParams;
	};
	UObject* (*const Z_Construct_UClass_UPEBlueprintAsset_Statics::DependentSingletons[])() = {
		(UObject* (*)())Z_Construct_UClass_UObject,
		(UObject* (*)())Z_Construct_UPackage__Script_PuertsEditor,
	};
	const FClassFunctionLinkInfo Z_Construct_UClass_UPEBlueprintAsset_Statics::FuncInfo[] = {
		{ &Z_Construct_UFunction_UPEBlueprintAsset_AddFunction, "AddFunction" }, // 2176478814
		{ &Z_Construct_UFunction_UPEBlueprintAsset_AddFunctionWithMetaData, "AddFunctionWithMetaData" }, // 1724128805
		{ &Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariable, "AddMemberVariable" }, // 870761804
		{ &Z_Construct_UFunction_UPEBlueprintAsset_AddMemberVariableWithMetaData, "AddMemberVariableWithMetaData" }, // 2238598493
		{ &Z_Construct_UFunction_UPEBlueprintAsset_AddParameter, "AddParameter" }, // 2205472800
		{ &Z_Construct_UFunction_UPEBlueprintAsset_AddParameterWithMetaData, "AddParameterWithMetaData" }, // 1677784431
		{ &Z_Construct_UFunction_UPEBlueprintAsset_ClearParameter, "ClearParameter" }, // 3905774890
		{ &Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreate, "LoadOrCreate" }, // 3854191280
		{ &Z_Construct_UFunction_UPEBlueprintAsset_LoadOrCreateWithMetaData, "LoadOrCreateWithMetaData" }, // 1976425387
		{ &Z_Construct_UFunction_UPEBlueprintAsset_RemoveNotExistedFunction, "RemoveNotExistedFunction" }, // 521526950
		{ &Z_Construct_UFunction_UPEBlueprintAsset_RemoveNotExistedMemberVariable, "RemoveNotExistedMemberVariable" }, // 1865258539
		{ &Z_Construct_UFunction_UPEBlueprintAsset_Save, "Save" }, // 1747793987
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UPEBlueprintAsset_Statics::Class_MetaDataParams[] = {
		{ "Comment", "/**\n *\n */" },
		{ "IncludePath", "PEBlueprintAsset.h" },
		{ "ModuleRelativePath", "Public/PEBlueprintAsset.h" },
	};
#endif
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_GeneratedClass_MetaData[] = {
		{ "Category", "PEBlueprintAsset" },
		{ "ModuleRelativePath", "Public/PEBlueprintAsset.h" },
	};
#endif
	const UE4CodeGen_Private::FClassPropertyParams Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_GeneratedClass = { "GeneratedClass", nullptr, (EPropertyFlags)0x0010000000000014, UE4CodeGen_Private::EPropertyGenFlags::Class, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(UPEBlueprintAsset, GeneratedClass), Z_Construct_UClass_UObject_NoRegister, Z_Construct_UClass_UClass, METADATA_PARAMS(Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_GeneratedClass_MetaData, UE_ARRAY_COUNT(Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_GeneratedClass_MetaData)) };
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_Blueprint_MetaData[] = {
		{ "Category", "PEBlueprintAsset" },
		{ "ModuleRelativePath", "Public/PEBlueprintAsset.h" },
	};
#endif
	const UE4CodeGen_Private::FObjectPropertyParams Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_Blueprint = { "Blueprint", nullptr, (EPropertyFlags)0x0010000000000014, UE4CodeGen_Private::EPropertyGenFlags::Object, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(UPEBlueprintAsset, Blueprint), Z_Construct_UClass_UBlueprint_NoRegister, METADATA_PARAMS(Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_Blueprint_MetaData, UE_ARRAY_COUNT(Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_Blueprint_MetaData)) };
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_Package_MetaData[] = {
		{ "Category", "PEBlueprintAsset" },
		{ "ModuleRelativePath", "Public/PEBlueprintAsset.h" },
	};
#endif
	const UE4CodeGen_Private::FObjectPropertyParams Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_Package = { "Package", nullptr, (EPropertyFlags)0x0010000000000014, UE4CodeGen_Private::EPropertyGenFlags::Object, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(UPEBlueprintAsset, Package), Z_Construct_UClass_UPackage, METADATA_PARAMS(Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_Package_MetaData, UE_ARRAY_COUNT(Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_Package_MetaData)) };
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_NeedSave_MetaData[] = {
		{ "Category", "PEBlueprintAsset" },
		{ "ModuleRelativePath", "Public/PEBlueprintAsset.h" },
	};
#endif
	void Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_NeedSave_SetBit(void* Obj)
	{
		((UPEBlueprintAsset*)Obj)->NeedSave = 1;
	}
	const UE4CodeGen_Private::FBoolPropertyParams Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_NeedSave = { "NeedSave", nullptr, (EPropertyFlags)0x0010000000000014, UE4CodeGen_Private::EPropertyGenFlags::Bool | UE4CodeGen_Private::EPropertyGenFlags::NativeBool, RF_Public|RF_Transient|RF_MarkAsNative, 1, sizeof(bool), sizeof(UPEBlueprintAsset), &Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_NeedSave_SetBit, METADATA_PARAMS(Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_NeedSave_MetaData, UE_ARRAY_COUNT(Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_NeedSave_MetaData)) };
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_HasConstructor_MetaData[] = {
		{ "ModuleRelativePath", "Public/PEBlueprintAsset.h" },
	};
#endif
	void Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_HasConstructor_SetBit(void* Obj)
	{
		((UPEBlueprintAsset*)Obj)->HasConstructor = 1;
	}
	const UE4CodeGen_Private::FBoolPropertyParams Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_HasConstructor = { "HasConstructor", nullptr, (EPropertyFlags)0x0010000000000000, UE4CodeGen_Private::EPropertyGenFlags::Bool | UE4CodeGen_Private::EPropertyGenFlags::NativeBool, RF_Public|RF_Transient|RF_MarkAsNative, 1, sizeof(bool), sizeof(UPEBlueprintAsset), &Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_HasConstructor_SetBit, METADATA_PARAMS(Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_HasConstructor_MetaData, UE_ARRAY_COUNT(Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_HasConstructor_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UClass_UPEBlueprintAsset_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_GeneratedClass,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_Blueprint,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_Package,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_NeedSave,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UClass_UPEBlueprintAsset_Statics::NewProp_HasConstructor,
	};
	const FCppClassTypeInfoStatic Z_Construct_UClass_UPEBlueprintAsset_Statics::StaticCppClassTypeInfo = {
		TCppClassTypeTraits<UPEBlueprintAsset>::IsAbstract,
	};
	const UE4CodeGen_Private::FClassParams Z_Construct_UClass_UPEBlueprintAsset_Statics::ClassParams = {
		&UPEBlueprintAsset::StaticClass,
		nullptr,
		&StaticCppClassTypeInfo,
		DependentSingletons,
		FuncInfo,
		Z_Construct_UClass_UPEBlueprintAsset_Statics::PropPointers,
		nullptr,
		UE_ARRAY_COUNT(DependentSingletons),
		UE_ARRAY_COUNT(FuncInfo),
		UE_ARRAY_COUNT(Z_Construct_UClass_UPEBlueprintAsset_Statics::PropPointers),
		0,
		0x001000A0u,
		METADATA_PARAMS(Z_Construct_UClass_UPEBlueprintAsset_Statics::Class_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UClass_UPEBlueprintAsset_Statics::Class_MetaDataParams))
	};
	UClass* Z_Construct_UClass_UPEBlueprintAsset()
	{
		static UClass* OuterClass = nullptr;
		if (!OuterClass)
		{
			UE4CodeGen_Private::ConstructUClass(OuterClass, Z_Construct_UClass_UPEBlueprintAsset_Statics::ClassParams);
		}
		return OuterClass;
	}
	IMPLEMENT_CLASS(UPEBlueprintAsset, 1632560801);
	template<> PUERTSEDITOR_API UClass* StaticClass<UPEBlueprintAsset>()
	{
		return UPEBlueprintAsset::StaticClass();
	}
	static FCompiledInDefer Z_CompiledInDefer_UClass_UPEBlueprintAsset(Z_Construct_UClass_UPEBlueprintAsset, &UPEBlueprintAsset::StaticClass, TEXT("/Script/PuertsEditor"), TEXT("UPEBlueprintAsset"), false, nullptr, nullptr, nullptr);
	DEFINE_VTABLE_PTR_HELPER_CTOR(UPEBlueprintAsset);
PRAGMA_ENABLE_DEPRECATION_WARNINGS
#ifdef _MSC_VER
#pragma warning (pop)
#endif
