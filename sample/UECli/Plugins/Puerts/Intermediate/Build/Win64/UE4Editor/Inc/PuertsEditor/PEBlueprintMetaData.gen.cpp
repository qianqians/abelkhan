// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/GeneratedCppIncludes.h"
#include "PuertsEditor/Public/PEBlueprintMetaData.h"
#ifdef _MSC_VER
#pragma warning (push)
#pragma warning (disable : 4883)
#endif
PRAGMA_DISABLE_DEPRECATION_WARNINGS
void EmptyLinkFunctionForGeneratedCodePEBlueprintMetaData() {}
// Cross Module References
	PUERTSEDITOR_API UClass* Z_Construct_UClass_UPEClassMetaData_NoRegister();
	PUERTSEDITOR_API UClass* Z_Construct_UClass_UPEClassMetaData();
	COREUOBJECT_API UClass* Z_Construct_UClass_UObject();
	UPackage* Z_Construct_UPackage__Script_PuertsEditor();
	PUERTSEDITOR_API UClass* Z_Construct_UClass_UPEFunctionMetaData_NoRegister();
	PUERTSEDITOR_API UClass* Z_Construct_UClass_UPEFunctionMetaData();
	PUERTSEDITOR_API UClass* Z_Construct_UClass_UPEParamMetaData_NoRegister();
	PUERTSEDITOR_API UClass* Z_Construct_UClass_UPEParamMetaData();
	PUERTSEDITOR_API UClass* Z_Construct_UClass_UPEPropertyMetaData_NoRegister();
	PUERTSEDITOR_API UClass* Z_Construct_UClass_UPEPropertyMetaData();
// End Cross Module References
	DEFINE_FUNCTION(UPEClassMetaData::execAddSparseDataType)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_InType);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->AddSparseDataType(Z_Param_InType);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEClassMetaData::execAddClassGroup)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_InGroupName);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->AddClassGroup(Z_Param_InGroupName);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEClassMetaData::execAddDontAutoCollapseCategory)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_InCategory);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->AddDontAutoCollapseCategory(Z_Param_InCategory);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEClassMetaData::execAddAutoCollapseCategory)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_InCategory);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->AddAutoCollapseCategory(Z_Param_InCategory);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEClassMetaData::execAddAutoExpandCategory)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_InCategory);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->AddAutoExpandCategory(Z_Param_InCategory);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEClassMetaData::execAddShowFunction)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_InFunctionName);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->AddShowFunction(Z_Param_InFunctionName);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEClassMetaData::execAddHideFunction)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_InFunctionName);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->AddHideFunction(Z_Param_InFunctionName);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEClassMetaData::execAddShowSubCategory)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_InCategory);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->AddShowSubCategory(Z_Param_InCategory);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEClassMetaData::execAddShowCategory)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_InCategory);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->AddShowCategory(Z_Param_InCategory);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEClassMetaData::execAddHideCategory)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_InCategory);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->AddHideCategory(Z_Param_InCategory);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEClassMetaData::execSetConfig)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_InConfigName);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->SetConfig(Z_Param_InConfigName);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEClassMetaData::execSetClassWithIn)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_InClassName);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->SetClassWithIn(Z_Param_InClassName);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEClassMetaData::execSetMetaData)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_InName);
		P_GET_PROPERTY(FStrProperty,Z_Param_InValue);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->SetMetaData(Z_Param_InName,Z_Param_InValue);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEClassMetaData::execSetClassFlags)
	{
		P_GET_PROPERTY(FIntProperty,Z_Param_InFlags);
		P_GET_UBOOL(Z_Param_bInPlaceable);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->SetClassFlags(Z_Param_InFlags,Z_Param_bInPlaceable);
		P_NATIVE_END;
	}
	void UPEClassMetaData::StaticRegisterNativesUPEClassMetaData()
	{
		UClass* Class = UPEClassMetaData::StaticClass();
		static const FNameNativePtrPair Funcs[] = {
			{ "AddAutoCollapseCategory", &UPEClassMetaData::execAddAutoCollapseCategory },
			{ "AddAutoExpandCategory", &UPEClassMetaData::execAddAutoExpandCategory },
			{ "AddClassGroup", &UPEClassMetaData::execAddClassGroup },
			{ "AddDontAutoCollapseCategory", &UPEClassMetaData::execAddDontAutoCollapseCategory },
			{ "AddHideCategory", &UPEClassMetaData::execAddHideCategory },
			{ "AddHideFunction", &UPEClassMetaData::execAddHideFunction },
			{ "AddShowCategory", &UPEClassMetaData::execAddShowCategory },
			{ "AddShowFunction", &UPEClassMetaData::execAddShowFunction },
			{ "AddShowSubCategory", &UPEClassMetaData::execAddShowSubCategory },
			{ "AddSparseDataType", &UPEClassMetaData::execAddSparseDataType },
			{ "SetClassFlags", &UPEClassMetaData::execSetClassFlags },
			{ "SetClassWithIn", &UPEClassMetaData::execSetClassWithIn },
			{ "SetConfig", &UPEClassMetaData::execSetConfig },
			{ "SetMetaData", &UPEClassMetaData::execSetMetaData },
		};
		FNativeFunctionRegistrar::RegisterFunctions(Class, Funcs, UE_ARRAY_COUNT(Funcs));
	}
	struct Z_Construct_UFunction_UPEClassMetaData_AddAutoCollapseCategory_Statics
	{
		struct PEClassMetaData_eventAddAutoCollapseCategory_Parms
		{
			FString InCategory;
		};
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InCategory_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InCategory;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_AddAutoCollapseCategory_Statics::NewProp_InCategory_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEClassMetaData_AddAutoCollapseCategory_Statics::NewProp_InCategory = { "InCategory", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEClassMetaData_eventAddAutoCollapseCategory_Parms, InCategory), METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_AddAutoCollapseCategory_Statics::NewProp_InCategory_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddAutoCollapseCategory_Statics::NewProp_InCategory_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEClassMetaData_AddAutoCollapseCategory_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEClassMetaData_AddAutoCollapseCategory_Statics::NewProp_InCategory,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_AddAutoCollapseCategory_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief add a category to auto collapse in blueprint\n     * @param InCategory\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief add a category to auto collapse in blueprint\n@param InCategory" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEClassMetaData_AddAutoCollapseCategory_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEClassMetaData, nullptr, "AddAutoCollapseCategory", nullptr, nullptr, sizeof(PEClassMetaData_eventAddAutoCollapseCategory_Parms), Z_Construct_UFunction_UPEClassMetaData_AddAutoCollapseCategory_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddAutoCollapseCategory_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_AddAutoCollapseCategory_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddAutoCollapseCategory_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEClassMetaData_AddAutoCollapseCategory()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEClassMetaData_AddAutoCollapseCategory_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEClassMetaData_AddAutoExpandCategory_Statics
	{
		struct PEClassMetaData_eventAddAutoExpandCategory_Parms
		{
			FString InCategory;
		};
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InCategory_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InCategory;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_AddAutoExpandCategory_Statics::NewProp_InCategory_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEClassMetaData_AddAutoExpandCategory_Statics::NewProp_InCategory = { "InCategory", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEClassMetaData_eventAddAutoExpandCategory_Parms, InCategory), METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_AddAutoExpandCategory_Statics::NewProp_InCategory_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddAutoExpandCategory_Statics::NewProp_InCategory_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEClassMetaData_AddAutoExpandCategory_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEClassMetaData_AddAutoExpandCategory_Statics::NewProp_InCategory,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_AddAutoExpandCategory_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief add a category to auto expand in blueprint\n     * @param InCategory\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief add a category to auto expand in blueprint\n@param InCategory" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEClassMetaData_AddAutoExpandCategory_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEClassMetaData, nullptr, "AddAutoExpandCategory", nullptr, nullptr, sizeof(PEClassMetaData_eventAddAutoExpandCategory_Parms), Z_Construct_UFunction_UPEClassMetaData_AddAutoExpandCategory_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddAutoExpandCategory_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_AddAutoExpandCategory_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddAutoExpandCategory_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEClassMetaData_AddAutoExpandCategory()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEClassMetaData_AddAutoExpandCategory_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEClassMetaData_AddClassGroup_Statics
	{
		struct PEClassMetaData_eventAddClassGroup_Parms
		{
			FString InGroupName;
		};
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InGroupName_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InGroupName;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_AddClassGroup_Statics::NewProp_InGroupName_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEClassMetaData_AddClassGroup_Statics::NewProp_InGroupName = { "InGroupName", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEClassMetaData_eventAddClassGroup_Parms, InGroupName), METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_AddClassGroup_Statics::NewProp_InGroupName_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddClassGroup_Statics::NewProp_InGroupName_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEClassMetaData_AddClassGroup_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEClassMetaData_AddClassGroup_Statics::NewProp_InGroupName,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_AddClassGroup_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief add a class group\n     * @param InGroupName\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief add a class group\n@param InGroupName" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEClassMetaData_AddClassGroup_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEClassMetaData, nullptr, "AddClassGroup", nullptr, nullptr, sizeof(PEClassMetaData_eventAddClassGroup_Parms), Z_Construct_UFunction_UPEClassMetaData_AddClassGroup_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddClassGroup_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_AddClassGroup_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddClassGroup_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEClassMetaData_AddClassGroup()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEClassMetaData_AddClassGroup_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEClassMetaData_AddDontAutoCollapseCategory_Statics
	{
		struct PEClassMetaData_eventAddDontAutoCollapseCategory_Parms
		{
			FString InCategory;
		};
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InCategory_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InCategory;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_AddDontAutoCollapseCategory_Statics::NewProp_InCategory_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEClassMetaData_AddDontAutoCollapseCategory_Statics::NewProp_InCategory = { "InCategory", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEClassMetaData_eventAddDontAutoCollapseCategory_Parms, InCategory), METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_AddDontAutoCollapseCategory_Statics::NewProp_InCategory_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddDontAutoCollapseCategory_Statics::NewProp_InCategory_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEClassMetaData_AddDontAutoCollapseCategory_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEClassMetaData_AddDontAutoCollapseCategory_Statics::NewProp_InCategory,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_AddDontAutoCollapseCategory_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief forbid a category to auto collapse in blueprint\n     * @param InCategory\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief forbid a category to auto collapse in blueprint\n@param InCategory" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEClassMetaData_AddDontAutoCollapseCategory_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEClassMetaData, nullptr, "AddDontAutoCollapseCategory", nullptr, nullptr, sizeof(PEClassMetaData_eventAddDontAutoCollapseCategory_Parms), Z_Construct_UFunction_UPEClassMetaData_AddDontAutoCollapseCategory_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddDontAutoCollapseCategory_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_AddDontAutoCollapseCategory_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddDontAutoCollapseCategory_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEClassMetaData_AddDontAutoCollapseCategory()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEClassMetaData_AddDontAutoCollapseCategory_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEClassMetaData_AddHideCategory_Statics
	{
		struct PEClassMetaData_eventAddHideCategory_Parms
		{
			FString InCategory;
		};
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InCategory_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InCategory;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_AddHideCategory_Statics::NewProp_InCategory_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEClassMetaData_AddHideCategory_Statics::NewProp_InCategory = { "InCategory", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEClassMetaData_eventAddHideCategory_Parms, InCategory), METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_AddHideCategory_Statics::NewProp_InCategory_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddHideCategory_Statics::NewProp_InCategory_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEClassMetaData_AddHideCategory_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEClassMetaData_AddHideCategory_Statics::NewProp_InCategory,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_AddHideCategory_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief add a category to hide in blueprint\n     * @param InCategory\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief add a category to hide in blueprint\n@param InCategory" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEClassMetaData_AddHideCategory_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEClassMetaData, nullptr, "AddHideCategory", nullptr, nullptr, sizeof(PEClassMetaData_eventAddHideCategory_Parms), Z_Construct_UFunction_UPEClassMetaData_AddHideCategory_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddHideCategory_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_AddHideCategory_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddHideCategory_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEClassMetaData_AddHideCategory()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEClassMetaData_AddHideCategory_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEClassMetaData_AddHideFunction_Statics
	{
		struct PEClassMetaData_eventAddHideFunction_Parms
		{
			FString InFunctionName;
		};
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InFunctionName_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InFunctionName;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_AddHideFunction_Statics::NewProp_InFunctionName_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEClassMetaData_AddHideFunction_Statics::NewProp_InFunctionName = { "InFunctionName", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEClassMetaData_eventAddHideFunction_Parms, InFunctionName), METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_AddHideFunction_Statics::NewProp_InFunctionName_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddHideFunction_Statics::NewProp_InFunctionName_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEClassMetaData_AddHideFunction_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEClassMetaData_AddHideFunction_Statics::NewProp_InFunctionName,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_AddHideFunction_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief add a function to hide in blueprint\n     * @param InFunctionName\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief add a function to hide in blueprint\n@param InFunctionName" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEClassMetaData_AddHideFunction_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEClassMetaData, nullptr, "AddHideFunction", nullptr, nullptr, sizeof(PEClassMetaData_eventAddHideFunction_Parms), Z_Construct_UFunction_UPEClassMetaData_AddHideFunction_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddHideFunction_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_AddHideFunction_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddHideFunction_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEClassMetaData_AddHideFunction()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEClassMetaData_AddHideFunction_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEClassMetaData_AddShowCategory_Statics
	{
		struct PEClassMetaData_eventAddShowCategory_Parms
		{
			FString InCategory;
		};
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InCategory_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InCategory;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_AddShowCategory_Statics::NewProp_InCategory_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEClassMetaData_AddShowCategory_Statics::NewProp_InCategory = { "InCategory", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEClassMetaData_eventAddShowCategory_Parms, InCategory), METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_AddShowCategory_Statics::NewProp_InCategory_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddShowCategory_Statics::NewProp_InCategory_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEClassMetaData_AddShowCategory_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEClassMetaData_AddShowCategory_Statics::NewProp_InCategory,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_AddShowCategory_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief add a category to show in blueprint\n     * @param InCategory\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief add a category to show in blueprint\n@param InCategory" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEClassMetaData_AddShowCategory_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEClassMetaData, nullptr, "AddShowCategory", nullptr, nullptr, sizeof(PEClassMetaData_eventAddShowCategory_Parms), Z_Construct_UFunction_UPEClassMetaData_AddShowCategory_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddShowCategory_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_AddShowCategory_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddShowCategory_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEClassMetaData_AddShowCategory()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEClassMetaData_AddShowCategory_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEClassMetaData_AddShowFunction_Statics
	{
		struct PEClassMetaData_eventAddShowFunction_Parms
		{
			FString InFunctionName;
		};
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InFunctionName_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InFunctionName;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_AddShowFunction_Statics::NewProp_InFunctionName_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEClassMetaData_AddShowFunction_Statics::NewProp_InFunctionName = { "InFunctionName", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEClassMetaData_eventAddShowFunction_Parms, InFunctionName), METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_AddShowFunction_Statics::NewProp_InFunctionName_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddShowFunction_Statics::NewProp_InFunctionName_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEClassMetaData_AddShowFunction_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEClassMetaData_AddShowFunction_Statics::NewProp_InFunctionName,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_AddShowFunction_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief add a function to show in blueprint\n     * @param InFunctionName\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief add a function to show in blueprint\n@param InFunctionName" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEClassMetaData_AddShowFunction_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEClassMetaData, nullptr, "AddShowFunction", nullptr, nullptr, sizeof(PEClassMetaData_eventAddShowFunction_Parms), Z_Construct_UFunction_UPEClassMetaData_AddShowFunction_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddShowFunction_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_AddShowFunction_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddShowFunction_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEClassMetaData_AddShowFunction()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEClassMetaData_AddShowFunction_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEClassMetaData_AddShowSubCategory_Statics
	{
		struct PEClassMetaData_eventAddShowSubCategory_Parms
		{
			FString InCategory;
		};
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InCategory_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InCategory;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_AddShowSubCategory_Statics::NewProp_InCategory_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEClassMetaData_AddShowSubCategory_Statics::NewProp_InCategory = { "InCategory", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEClassMetaData_eventAddShowSubCategory_Parms, InCategory), METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_AddShowSubCategory_Statics::NewProp_InCategory_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddShowSubCategory_Statics::NewProp_InCategory_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEClassMetaData_AddShowSubCategory_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEClassMetaData_AddShowSubCategory_Statics::NewProp_InCategory,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_AddShowSubCategory_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief add a sub category to show in blueprint\n     * @param InCategory\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief add a sub category to show in blueprint\n@param InCategory" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEClassMetaData_AddShowSubCategory_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEClassMetaData, nullptr, "AddShowSubCategory", nullptr, nullptr, sizeof(PEClassMetaData_eventAddShowSubCategory_Parms), Z_Construct_UFunction_UPEClassMetaData_AddShowSubCategory_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddShowSubCategory_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_AddShowSubCategory_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddShowSubCategory_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEClassMetaData_AddShowSubCategory()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEClassMetaData_AddShowSubCategory_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEClassMetaData_AddSparseDataType_Statics
	{
		struct PEClassMetaData_eventAddSparseDataType_Parms
		{
			FString InType;
		};
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InType_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InType;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_AddSparseDataType_Statics::NewProp_InType_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEClassMetaData_AddSparseDataType_Statics::NewProp_InType = { "InType", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEClassMetaData_eventAddSparseDataType_Parms, InType), METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_AddSparseDataType_Statics::NewProp_InType_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddSparseDataType_Statics::NewProp_InType_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEClassMetaData_AddSparseDataType_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEClassMetaData_AddSparseDataType_Statics::NewProp_InType,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_AddSparseDataType_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief add a sparse data type\n     * @param InType\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief add a sparse data type\n@param InType" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEClassMetaData_AddSparseDataType_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEClassMetaData, nullptr, "AddSparseDataType", nullptr, nullptr, sizeof(PEClassMetaData_eventAddSparseDataType_Parms), Z_Construct_UFunction_UPEClassMetaData_AddSparseDataType_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddSparseDataType_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_AddSparseDataType_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_AddSparseDataType_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEClassMetaData_AddSparseDataType()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEClassMetaData_AddSparseDataType_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEClassMetaData_SetClassFlags_Statics
	{
		struct PEClassMetaData_eventSetClassFlags_Parms
		{
			int32 InFlags;
			bool bInPlaceable;
		};
		static const UE4CodeGen_Private::FIntPropertyParams NewProp_InFlags;
		static void NewProp_bInPlaceable_SetBit(void* Obj);
		static const UE4CodeGen_Private::FBoolPropertyParams NewProp_bInPlaceable;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
	const UE4CodeGen_Private::FIntPropertyParams Z_Construct_UFunction_UPEClassMetaData_SetClassFlags_Statics::NewProp_InFlags = { "InFlags", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Int, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEClassMetaData_eventSetClassFlags_Parms, InFlags), METADATA_PARAMS(nullptr, 0) };
	void Z_Construct_UFunction_UPEClassMetaData_SetClassFlags_Statics::NewProp_bInPlaceable_SetBit(void* Obj)
	{
		((PEClassMetaData_eventSetClassFlags_Parms*)Obj)->bInPlaceable = 1;
	}
	const UE4CodeGen_Private::FBoolPropertyParams Z_Construct_UFunction_UPEClassMetaData_SetClassFlags_Statics::NewProp_bInPlaceable = { "bInPlaceable", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Bool | UE4CodeGen_Private::EPropertyGenFlags::NativeBool, RF_Public|RF_Transient|RF_MarkAsNative, 1, sizeof(bool), sizeof(PEClassMetaData_eventSetClassFlags_Parms), &Z_Construct_UFunction_UPEClassMetaData_SetClassFlags_Statics::NewProp_bInPlaceable_SetBit, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEClassMetaData_SetClassFlags_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEClassMetaData_SetClassFlags_Statics::NewProp_InFlags,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEClassMetaData_SetClassFlags_Statics::NewProp_bInPlaceable,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_SetClassFlags_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief set the class flags, ant notify if placeable specifier is set\n     * @param InFlags\n     * @param bInPlaceable\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief set the class flags, ant notify if placeable specifier is set\n@param InFlags\n@param bInPlaceable" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEClassMetaData_SetClassFlags_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEClassMetaData, nullptr, "SetClassFlags", nullptr, nullptr, sizeof(PEClassMetaData_eventSetClassFlags_Parms), Z_Construct_UFunction_UPEClassMetaData_SetClassFlags_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_SetClassFlags_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_SetClassFlags_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_SetClassFlags_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEClassMetaData_SetClassFlags()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEClassMetaData_SetClassFlags_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEClassMetaData_SetClassWithIn_Statics
	{
		struct PEClassMetaData_eventSetClassWithIn_Parms
		{
			FString InClassName;
		};
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InClassName_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InClassName;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_SetClassWithIn_Statics::NewProp_InClassName_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEClassMetaData_SetClassWithIn_Statics::NewProp_InClassName = { "InClassName", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEClassMetaData_eventSetClassWithIn_Parms, InClassName), METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_SetClassWithIn_Statics::NewProp_InClassName_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_SetClassWithIn_Statics::NewProp_InClassName_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEClassMetaData_SetClassWithIn_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEClassMetaData_SetClassWithIn_Statics::NewProp_InClassName,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_SetClassWithIn_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief set the class should be with in\n     * @param InClassName\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief set the class should be with in\n@param InClassName" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEClassMetaData_SetClassWithIn_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEClassMetaData, nullptr, "SetClassWithIn", nullptr, nullptr, sizeof(PEClassMetaData_eventSetClassWithIn_Parms), Z_Construct_UFunction_UPEClassMetaData_SetClassWithIn_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_SetClassWithIn_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_SetClassWithIn_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_SetClassWithIn_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEClassMetaData_SetClassWithIn()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEClassMetaData_SetClassWithIn_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEClassMetaData_SetConfig_Statics
	{
		struct PEClassMetaData_eventSetConfig_Parms
		{
			FString InConfigName;
		};
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InConfigName_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InConfigName;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_SetConfig_Statics::NewProp_InConfigName_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEClassMetaData_SetConfig_Statics::NewProp_InConfigName = { "InConfigName", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEClassMetaData_eventSetConfig_Parms, InConfigName), METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_SetConfig_Statics::NewProp_InConfigName_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_SetConfig_Statics::NewProp_InConfigName_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEClassMetaData_SetConfig_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEClassMetaData_SetConfig_Statics::NewProp_InConfigName,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_SetConfig_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief set the configuration name\n     * @param InConfigName\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief set the configuration name\n@param InConfigName" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEClassMetaData_SetConfig_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEClassMetaData, nullptr, "SetConfig", nullptr, nullptr, sizeof(PEClassMetaData_eventSetConfig_Parms), Z_Construct_UFunction_UPEClassMetaData_SetConfig_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_SetConfig_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_SetConfig_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_SetConfig_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEClassMetaData_SetConfig()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEClassMetaData_SetConfig_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEClassMetaData_SetMetaData_Statics
	{
		struct PEClassMetaData_eventSetMetaData_Parms
		{
			FString InName;
			FString InValue;
		};
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InName_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InName;
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InValue_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InValue;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_SetMetaData_Statics::NewProp_InName_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEClassMetaData_SetMetaData_Statics::NewProp_InName = { "InName", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEClassMetaData_eventSetMetaData_Parms, InName), METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_SetMetaData_Statics::NewProp_InName_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_SetMetaData_Statics::NewProp_InName_MetaData)) };
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_SetMetaData_Statics::NewProp_InValue_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEClassMetaData_SetMetaData_Statics::NewProp_InValue = { "InValue", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEClassMetaData_eventSetMetaData_Parms, InValue), METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_SetMetaData_Statics::NewProp_InValue_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_SetMetaData_Statics::NewProp_InValue_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEClassMetaData_SetMetaData_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEClassMetaData_SetMetaData_Statics::NewProp_InName,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEClassMetaData_SetMetaData_Statics::NewProp_InValue,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEClassMetaData_SetMetaData_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief set the specific meta data\n     * @param InName\n     * @param InValue\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief set the specific meta data\n@param InName\n@param InValue" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEClassMetaData_SetMetaData_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEClassMetaData, nullptr, "SetMetaData", nullptr, nullptr, sizeof(PEClassMetaData_eventSetMetaData_Parms), Z_Construct_UFunction_UPEClassMetaData_SetMetaData_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_SetMetaData_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEClassMetaData_SetMetaData_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEClassMetaData_SetMetaData_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEClassMetaData_SetMetaData()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEClassMetaData_SetMetaData_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	UClass* Z_Construct_UClass_UPEClassMetaData_NoRegister()
	{
		return UPEClassMetaData::StaticClass();
	}
	struct Z_Construct_UClass_UPEClassMetaData_Statics
	{
		static UObject* (*const DependentSingletons[])();
		static const FClassFunctionLinkInfo FuncInfo[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Class_MetaDataParams[];
#endif
		static const FCppClassTypeInfoStatic StaticCppClassTypeInfo;
		static const UE4CodeGen_Private::FClassParams ClassParams;
	};
	UObject* (*const Z_Construct_UClass_UPEClassMetaData_Statics::DependentSingletons[])() = {
		(UObject* (*)())Z_Construct_UClass_UObject,
		(UObject* (*)())Z_Construct_UPackage__Script_PuertsEditor,
	};
	const FClassFunctionLinkInfo Z_Construct_UClass_UPEClassMetaData_Statics::FuncInfo[] = {
		{ &Z_Construct_UFunction_UPEClassMetaData_AddAutoCollapseCategory, "AddAutoCollapseCategory" }, // 4169262472
		{ &Z_Construct_UFunction_UPEClassMetaData_AddAutoExpandCategory, "AddAutoExpandCategory" }, // 2769011013
		{ &Z_Construct_UFunction_UPEClassMetaData_AddClassGroup, "AddClassGroup" }, // 247219165
		{ &Z_Construct_UFunction_UPEClassMetaData_AddDontAutoCollapseCategory, "AddDontAutoCollapseCategory" }, // 3818837133
		{ &Z_Construct_UFunction_UPEClassMetaData_AddHideCategory, "AddHideCategory" }, // 1901581746
		{ &Z_Construct_UFunction_UPEClassMetaData_AddHideFunction, "AddHideFunction" }, // 297986305
		{ &Z_Construct_UFunction_UPEClassMetaData_AddShowCategory, "AddShowCategory" }, // 2869714127
		{ &Z_Construct_UFunction_UPEClassMetaData_AddShowFunction, "AddShowFunction" }, // 2818578537
		{ &Z_Construct_UFunction_UPEClassMetaData_AddShowSubCategory, "AddShowSubCategory" }, // 3213226930
		{ &Z_Construct_UFunction_UPEClassMetaData_AddSparseDataType, "AddSparseDataType" }, // 383609094
		{ &Z_Construct_UFunction_UPEClassMetaData_SetClassFlags, "SetClassFlags" }, // 3695746550
		{ &Z_Construct_UFunction_UPEClassMetaData_SetClassWithIn, "SetClassWithIn" }, // 1150799315
		{ &Z_Construct_UFunction_UPEClassMetaData_SetConfig, "SetConfig" }, // 1654637806
		{ &Z_Construct_UFunction_UPEClassMetaData_SetMetaData, "SetMetaData" }, // 2917992886
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UPEClassMetaData_Statics::Class_MetaDataParams[] = {
		{ "Comment", "/**\n * @brief a helper structure used to store the meta data of a class, @see ClassDeclarationMetaData.h/cpp\n *\x09\x09""currently, we don't need remove methods, also the caller should ensure the internal data consistence\n *\x09\x09""e.g. the show category added should never be added in hide categories...\n */" },
		{ "IncludePath", "PEBlueprintMetaData.h" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief a helper structure used to store the meta data of a class, @see ClassDeclarationMetaData.h/cpp\n            currently, we don't need remove methods, also the caller should ensure the internal data consistence\n            e.g. the show category added should never be added in hide categories..." },
	};
#endif
	const FCppClassTypeInfoStatic Z_Construct_UClass_UPEClassMetaData_Statics::StaticCppClassTypeInfo = {
		TCppClassTypeTraits<UPEClassMetaData>::IsAbstract,
	};
	const UE4CodeGen_Private::FClassParams Z_Construct_UClass_UPEClassMetaData_Statics::ClassParams = {
		&UPEClassMetaData::StaticClass,
		nullptr,
		&StaticCppClassTypeInfo,
		DependentSingletons,
		FuncInfo,
		nullptr,
		nullptr,
		UE_ARRAY_COUNT(DependentSingletons),
		UE_ARRAY_COUNT(FuncInfo),
		0,
		0,
		0x001000A0u,
		METADATA_PARAMS(Z_Construct_UClass_UPEClassMetaData_Statics::Class_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UClass_UPEClassMetaData_Statics::Class_MetaDataParams))
	};
	UClass* Z_Construct_UClass_UPEClassMetaData()
	{
		static UClass* OuterClass = nullptr;
		if (!OuterClass)
		{
			UE4CodeGen_Private::ConstructUClass(OuterClass, Z_Construct_UClass_UPEClassMetaData_Statics::ClassParams);
		}
		return OuterClass;
	}
	IMPLEMENT_CLASS(UPEClassMetaData, 648608103);
	template<> PUERTSEDITOR_API UClass* StaticClass<UPEClassMetaData>()
	{
		return UPEClassMetaData::StaticClass();
	}
	static FCompiledInDefer Z_CompiledInDefer_UClass_UPEClassMetaData(Z_Construct_UClass_UPEClassMetaData, &UPEClassMetaData::StaticClass, TEXT("/Script/PuertsEditor"), TEXT("UPEClassMetaData"), false, nullptr, nullptr, nullptr);
	DEFINE_VTABLE_PTR_HELPER_CTOR(UPEClassMetaData);
	DEFINE_FUNCTION(UPEFunctionMetaData::execSetForceBlueprintImpure)
	{
		P_GET_UBOOL(Z_Param_bInForceBlueprintImpure);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->SetForceBlueprintImpure(Z_Param_bInForceBlueprintImpure);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEFunctionMetaData::execSetIsSealedEvent)
	{
		P_GET_UBOOL(Z_Param_bInSealedEvent);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->SetIsSealedEvent(Z_Param_bInSealedEvent);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEFunctionMetaData::execSetRPCResponseId)
	{
		P_GET_PROPERTY(FIntProperty,Z_Param_InRPCResponseId);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->SetRPCResponseId(Z_Param_InRPCResponseId);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEFunctionMetaData::execSetRPCId)
	{
		P_GET_PROPERTY(FIntProperty,Z_Param_InRPCId);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->SetRPCId(Z_Param_InRPCId);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEFunctionMetaData::execSetEndpointName)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_InEndpointName);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->SetEndpointName(Z_Param_InEndpointName);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEFunctionMetaData::execSetCppValidationImplName)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_InName);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->SetCppValidationImplName(Z_Param_InName);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEFunctionMetaData::execSetCppImplName)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_InName);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->SetCppImplName(Z_Param_InName);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEFunctionMetaData::execSetMetaData)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_InName);
		P_GET_PROPERTY(FStrProperty,Z_Param_InValue);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->SetMetaData(Z_Param_InName,Z_Param_InValue);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEFunctionMetaData::execSetFunctionExportFlags)
	{
		P_GET_PROPERTY(FIntProperty,Z_Param_InFlags);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->SetFunctionExportFlags(Z_Param_InFlags);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEFunctionMetaData::execSetFunctionFlags)
	{
		P_GET_PROPERTY(FIntProperty,Z_Param_InHighBits);
		P_GET_PROPERTY(FIntProperty,Z_Param_InLowBits);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->SetFunctionFlags(Z_Param_InHighBits,Z_Param_InLowBits);
		P_NATIVE_END;
	}
	void UPEFunctionMetaData::StaticRegisterNativesUPEFunctionMetaData()
	{
		UClass* Class = UPEFunctionMetaData::StaticClass();
		static const FNameNativePtrPair Funcs[] = {
			{ "SetCppImplName", &UPEFunctionMetaData::execSetCppImplName },
			{ "SetCppValidationImplName", &UPEFunctionMetaData::execSetCppValidationImplName },
			{ "SetEndpointName", &UPEFunctionMetaData::execSetEndpointName },
			{ "SetForceBlueprintImpure", &UPEFunctionMetaData::execSetForceBlueprintImpure },
			{ "SetFunctionExportFlags", &UPEFunctionMetaData::execSetFunctionExportFlags },
			{ "SetFunctionFlags", &UPEFunctionMetaData::execSetFunctionFlags },
			{ "SetIsSealedEvent", &UPEFunctionMetaData::execSetIsSealedEvent },
			{ "SetMetaData", &UPEFunctionMetaData::execSetMetaData },
			{ "SetRPCId", &UPEFunctionMetaData::execSetRPCId },
			{ "SetRPCResponseId", &UPEFunctionMetaData::execSetRPCResponseId },
		};
		FNativeFunctionRegistrar::RegisterFunctions(Class, Funcs, UE_ARRAY_COUNT(Funcs));
	}
	struct Z_Construct_UFunction_UPEFunctionMetaData_SetCppImplName_Statics
	{
		struct PEFunctionMetaData_eventSetCppImplName_Parms
		{
			FString InName;
		};
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InName_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InName;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEFunctionMetaData_SetCppImplName_Statics::NewProp_InName_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEFunctionMetaData_SetCppImplName_Statics::NewProp_InName = { "InName", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEFunctionMetaData_eventSetCppImplName_Parms, InName), METADATA_PARAMS(Z_Construct_UFunction_UPEFunctionMetaData_SetCppImplName_Statics::NewProp_InName_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEFunctionMetaData_SetCppImplName_Statics::NewProp_InName_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEFunctionMetaData_SetCppImplName_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEFunctionMetaData_SetCppImplName_Statics::NewProp_InName,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEFunctionMetaData_SetCppImplName_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief set the cpp implementation function, is this meaningful for blueprint function?\n     * @param InName\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief set the cpp implementation function, is this meaningful for blueprint function?\n@param InName" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEFunctionMetaData_SetCppImplName_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEFunctionMetaData, nullptr, "SetCppImplName", nullptr, nullptr, sizeof(PEFunctionMetaData_eventSetCppImplName_Parms), Z_Construct_UFunction_UPEFunctionMetaData_SetCppImplName_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEFunctionMetaData_SetCppImplName_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEFunctionMetaData_SetCppImplName_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEFunctionMetaData_SetCppImplName_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEFunctionMetaData_SetCppImplName()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEFunctionMetaData_SetCppImplName_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEFunctionMetaData_SetCppValidationImplName_Statics
	{
		struct PEFunctionMetaData_eventSetCppValidationImplName_Parms
		{
			FString InName;
		};
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InName_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InName;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEFunctionMetaData_SetCppValidationImplName_Statics::NewProp_InName_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEFunctionMetaData_SetCppValidationImplName_Statics::NewProp_InName = { "InName", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEFunctionMetaData_eventSetCppValidationImplName_Parms, InName), METADATA_PARAMS(Z_Construct_UFunction_UPEFunctionMetaData_SetCppValidationImplName_Statics::NewProp_InName_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEFunctionMetaData_SetCppValidationImplName_Statics::NewProp_InName_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEFunctionMetaData_SetCppValidationImplName_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEFunctionMetaData_SetCppValidationImplName_Statics::NewProp_InName,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEFunctionMetaData_SetCppValidationImplName_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief set the cpp implementation validation function\n     * @param InName\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief set the cpp implementation validation function\n@param InName" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEFunctionMetaData_SetCppValidationImplName_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEFunctionMetaData, nullptr, "SetCppValidationImplName", nullptr, nullptr, sizeof(PEFunctionMetaData_eventSetCppValidationImplName_Parms), Z_Construct_UFunction_UPEFunctionMetaData_SetCppValidationImplName_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEFunctionMetaData_SetCppValidationImplName_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEFunctionMetaData_SetCppValidationImplName_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEFunctionMetaData_SetCppValidationImplName_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEFunctionMetaData_SetCppValidationImplName()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEFunctionMetaData_SetCppValidationImplName_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEFunctionMetaData_SetEndpointName_Statics
	{
		struct PEFunctionMetaData_eventSetEndpointName_Parms
		{
			FString InEndpointName;
		};
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InEndpointName_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InEndpointName;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEFunctionMetaData_SetEndpointName_Statics::NewProp_InEndpointName_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEFunctionMetaData_SetEndpointName_Statics::NewProp_InEndpointName = { "InEndpointName", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEFunctionMetaData_eventSetEndpointName_Parms, InEndpointName), METADATA_PARAMS(Z_Construct_UFunction_UPEFunctionMetaData_SetEndpointName_Statics::NewProp_InEndpointName_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEFunctionMetaData_SetEndpointName_Statics::NewProp_InEndpointName_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEFunctionMetaData_SetEndpointName_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEFunctionMetaData_SetEndpointName_Statics::NewProp_InEndpointName,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEFunctionMetaData_SetEndpointName_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief set the end point name,\n     * @param InEndpointName\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief set the end point name,\n@param InEndpointName" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEFunctionMetaData_SetEndpointName_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEFunctionMetaData, nullptr, "SetEndpointName", nullptr, nullptr, sizeof(PEFunctionMetaData_eventSetEndpointName_Parms), Z_Construct_UFunction_UPEFunctionMetaData_SetEndpointName_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEFunctionMetaData_SetEndpointName_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEFunctionMetaData_SetEndpointName_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEFunctionMetaData_SetEndpointName_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEFunctionMetaData_SetEndpointName()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEFunctionMetaData_SetEndpointName_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEFunctionMetaData_SetForceBlueprintImpure_Statics
	{
		struct PEFunctionMetaData_eventSetForceBlueprintImpure_Parms
		{
			bool bInForceBlueprintImpure;
		};
		static void NewProp_bInForceBlueprintImpure_SetBit(void* Obj);
		static const UE4CodeGen_Private::FBoolPropertyParams NewProp_bInForceBlueprintImpure;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
	void Z_Construct_UFunction_UPEFunctionMetaData_SetForceBlueprintImpure_Statics::NewProp_bInForceBlueprintImpure_SetBit(void* Obj)
	{
		((PEFunctionMetaData_eventSetForceBlueprintImpure_Parms*)Obj)->bInForceBlueprintImpure = 1;
	}
	const UE4CodeGen_Private::FBoolPropertyParams Z_Construct_UFunction_UPEFunctionMetaData_SetForceBlueprintImpure_Statics::NewProp_bInForceBlueprintImpure = { "bInForceBlueprintImpure", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Bool | UE4CodeGen_Private::EPropertyGenFlags::NativeBool, RF_Public|RF_Transient|RF_MarkAsNative, 1, sizeof(bool), sizeof(PEFunctionMetaData_eventSetForceBlueprintImpure_Parms), &Z_Construct_UFunction_UPEFunctionMetaData_SetForceBlueprintImpure_Statics::NewProp_bInForceBlueprintImpure_SetBit, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEFunctionMetaData_SetForceBlueprintImpure_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEFunctionMetaData_SetForceBlueprintImpure_Statics::NewProp_bInForceBlueprintImpure,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEFunctionMetaData_SetForceBlueprintImpure_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief set if the function is force blueprint impure\n     * @param bInForceBlueprintImpure\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief set if the function is force blueprint impure\n@param bInForceBlueprintImpure" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEFunctionMetaData_SetForceBlueprintImpure_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEFunctionMetaData, nullptr, "SetForceBlueprintImpure", nullptr, nullptr, sizeof(PEFunctionMetaData_eventSetForceBlueprintImpure_Parms), Z_Construct_UFunction_UPEFunctionMetaData_SetForceBlueprintImpure_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEFunctionMetaData_SetForceBlueprintImpure_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEFunctionMetaData_SetForceBlueprintImpure_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEFunctionMetaData_SetForceBlueprintImpure_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEFunctionMetaData_SetForceBlueprintImpure()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEFunctionMetaData_SetForceBlueprintImpure_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionExportFlags_Statics
	{
		struct PEFunctionMetaData_eventSetFunctionExportFlags_Parms
		{
			int32 InFlags;
		};
		static const UE4CodeGen_Private::FIntPropertyParams NewProp_InFlags;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
	const UE4CodeGen_Private::FIntPropertyParams Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionExportFlags_Statics::NewProp_InFlags = { "InFlags", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Int, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEFunctionMetaData_eventSetFunctionExportFlags_Parms, InFlags), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionExportFlags_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionExportFlags_Statics::NewProp_InFlags,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionExportFlags_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief set the function export flags\n     * @param InFlags\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief set the function export flags\n@param InFlags" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionExportFlags_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEFunctionMetaData, nullptr, "SetFunctionExportFlags", nullptr, nullptr, sizeof(PEFunctionMetaData_eventSetFunctionExportFlags_Parms), Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionExportFlags_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionExportFlags_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionExportFlags_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionExportFlags_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionExportFlags()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionExportFlags_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionFlags_Statics
	{
		struct PEFunctionMetaData_eventSetFunctionFlags_Parms
		{
			int32 InHighBits;
			int32 InLowBits;
		};
		static const UE4CodeGen_Private::FIntPropertyParams NewProp_InHighBits;
		static const UE4CodeGen_Private::FIntPropertyParams NewProp_InLowBits;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
	const UE4CodeGen_Private::FIntPropertyParams Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionFlags_Statics::NewProp_InHighBits = { "InHighBits", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Int, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEFunctionMetaData_eventSetFunctionFlags_Parms, InHighBits), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FIntPropertyParams Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionFlags_Statics::NewProp_InLowBits = { "InLowBits", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Int, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEFunctionMetaData_eventSetFunctionFlags_Parms, InLowBits), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionFlags_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionFlags_Statics::NewProp_InHighBits,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionFlags_Statics::NewProp_InLowBits,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionFlags_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief set the function flags, since this will called from js, so divide into high and low parts\n     * @param InHighBits\n     * @param InLowBits\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief set the function flags, since this will called from js, so divide into high and low parts\n@param InHighBits\n@param InLowBits" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionFlags_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEFunctionMetaData, nullptr, "SetFunctionFlags", nullptr, nullptr, sizeof(PEFunctionMetaData_eventSetFunctionFlags_Parms), Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionFlags_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionFlags_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionFlags_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionFlags_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionFlags()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionFlags_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEFunctionMetaData_SetIsSealedEvent_Statics
	{
		struct PEFunctionMetaData_eventSetIsSealedEvent_Parms
		{
			bool bInSealedEvent;
		};
		static void NewProp_bInSealedEvent_SetBit(void* Obj);
		static const UE4CodeGen_Private::FBoolPropertyParams NewProp_bInSealedEvent;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
	void Z_Construct_UFunction_UPEFunctionMetaData_SetIsSealedEvent_Statics::NewProp_bInSealedEvent_SetBit(void* Obj)
	{
		((PEFunctionMetaData_eventSetIsSealedEvent_Parms*)Obj)->bInSealedEvent = 1;
	}
	const UE4CodeGen_Private::FBoolPropertyParams Z_Construct_UFunction_UPEFunctionMetaData_SetIsSealedEvent_Statics::NewProp_bInSealedEvent = { "bInSealedEvent", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Bool | UE4CodeGen_Private::EPropertyGenFlags::NativeBool, RF_Public|RF_Transient|RF_MarkAsNative, 1, sizeof(bool), sizeof(PEFunctionMetaData_eventSetIsSealedEvent_Parms), &Z_Construct_UFunction_UPEFunctionMetaData_SetIsSealedEvent_Statics::NewProp_bInSealedEvent_SetBit, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEFunctionMetaData_SetIsSealedEvent_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEFunctionMetaData_SetIsSealedEvent_Statics::NewProp_bInSealedEvent,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEFunctionMetaData_SetIsSealedEvent_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief set if the function is sealed event\n     * @param bInSealedEvent\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief set if the function is sealed event\n@param bInSealedEvent" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEFunctionMetaData_SetIsSealedEvent_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEFunctionMetaData, nullptr, "SetIsSealedEvent", nullptr, nullptr, sizeof(PEFunctionMetaData_eventSetIsSealedEvent_Parms), Z_Construct_UFunction_UPEFunctionMetaData_SetIsSealedEvent_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEFunctionMetaData_SetIsSealedEvent_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEFunctionMetaData_SetIsSealedEvent_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEFunctionMetaData_SetIsSealedEvent_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEFunctionMetaData_SetIsSealedEvent()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEFunctionMetaData_SetIsSealedEvent_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEFunctionMetaData_SetMetaData_Statics
	{
		struct PEFunctionMetaData_eventSetMetaData_Parms
		{
			FString InName;
			FString InValue;
		};
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InName_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InName;
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InValue_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InValue;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEFunctionMetaData_SetMetaData_Statics::NewProp_InName_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEFunctionMetaData_SetMetaData_Statics::NewProp_InName = { "InName", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEFunctionMetaData_eventSetMetaData_Parms, InName), METADATA_PARAMS(Z_Construct_UFunction_UPEFunctionMetaData_SetMetaData_Statics::NewProp_InName_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEFunctionMetaData_SetMetaData_Statics::NewProp_InName_MetaData)) };
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEFunctionMetaData_SetMetaData_Statics::NewProp_InValue_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEFunctionMetaData_SetMetaData_Statics::NewProp_InValue = { "InValue", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEFunctionMetaData_eventSetMetaData_Parms, InValue), METADATA_PARAMS(Z_Construct_UFunction_UPEFunctionMetaData_SetMetaData_Statics::NewProp_InValue_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEFunctionMetaData_SetMetaData_Statics::NewProp_InValue_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEFunctionMetaData_SetMetaData_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEFunctionMetaData_SetMetaData_Statics::NewProp_InName,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEFunctionMetaData_SetMetaData_Statics::NewProp_InValue,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEFunctionMetaData_SetMetaData_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief set the specific meta data\n     * @param InName\n     * @param InValue\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief set the specific meta data\n@param InName\n@param InValue" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEFunctionMetaData_SetMetaData_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEFunctionMetaData, nullptr, "SetMetaData", nullptr, nullptr, sizeof(PEFunctionMetaData_eventSetMetaData_Parms), Z_Construct_UFunction_UPEFunctionMetaData_SetMetaData_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEFunctionMetaData_SetMetaData_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEFunctionMetaData_SetMetaData_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEFunctionMetaData_SetMetaData_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEFunctionMetaData_SetMetaData()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEFunctionMetaData_SetMetaData_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEFunctionMetaData_SetRPCId_Statics
	{
		struct PEFunctionMetaData_eventSetRPCId_Parms
		{
			int32 InRPCId;
		};
		static const UE4CodeGen_Private::FIntPropertyParams NewProp_InRPCId;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
	const UE4CodeGen_Private::FIntPropertyParams Z_Construct_UFunction_UPEFunctionMetaData_SetRPCId_Statics::NewProp_InRPCId = { "InRPCId", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Int, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEFunctionMetaData_eventSetRPCId_Parms, InRPCId), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEFunctionMetaData_SetRPCId_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEFunctionMetaData_SetRPCId_Statics::NewProp_InRPCId,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEFunctionMetaData_SetRPCId_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief set the rpc id\n     * @param InRPCId\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief set the rpc id\n@param InRPCId" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEFunctionMetaData_SetRPCId_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEFunctionMetaData, nullptr, "SetRPCId", nullptr, nullptr, sizeof(PEFunctionMetaData_eventSetRPCId_Parms), Z_Construct_UFunction_UPEFunctionMetaData_SetRPCId_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEFunctionMetaData_SetRPCId_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEFunctionMetaData_SetRPCId_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEFunctionMetaData_SetRPCId_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEFunctionMetaData_SetRPCId()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEFunctionMetaData_SetRPCId_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEFunctionMetaData_SetRPCResponseId_Statics
	{
		struct PEFunctionMetaData_eventSetRPCResponseId_Parms
		{
			int32 InRPCResponseId;
		};
		static const UE4CodeGen_Private::FIntPropertyParams NewProp_InRPCResponseId;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
	const UE4CodeGen_Private::FIntPropertyParams Z_Construct_UFunction_UPEFunctionMetaData_SetRPCResponseId_Statics::NewProp_InRPCResponseId = { "InRPCResponseId", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Int, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEFunctionMetaData_eventSetRPCResponseId_Parms, InRPCResponseId), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEFunctionMetaData_SetRPCResponseId_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEFunctionMetaData_SetRPCResponseId_Statics::NewProp_InRPCResponseId,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEFunctionMetaData_SetRPCResponseId_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief set the rpc response id\n     * @param InRPCResponseId\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief set the rpc response id\n@param InRPCResponseId" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEFunctionMetaData_SetRPCResponseId_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEFunctionMetaData, nullptr, "SetRPCResponseId", nullptr, nullptr, sizeof(PEFunctionMetaData_eventSetRPCResponseId_Parms), Z_Construct_UFunction_UPEFunctionMetaData_SetRPCResponseId_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEFunctionMetaData_SetRPCResponseId_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEFunctionMetaData_SetRPCResponseId_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEFunctionMetaData_SetRPCResponseId_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEFunctionMetaData_SetRPCResponseId()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEFunctionMetaData_SetRPCResponseId_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	UClass* Z_Construct_UClass_UPEFunctionMetaData_NoRegister()
	{
		return UPEFunctionMetaData::StaticClass();
	}
	struct Z_Construct_UClass_UPEFunctionMetaData_Statics
	{
		static UObject* (*const DependentSingletons[])();
		static const FClassFunctionLinkInfo FuncInfo[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Class_MetaDataParams[];
#endif
		static const FCppClassTypeInfoStatic StaticCppClassTypeInfo;
		static const UE4CodeGen_Private::FClassParams ClassParams;
	};
	UObject* (*const Z_Construct_UClass_UPEFunctionMetaData_Statics::DependentSingletons[])() = {
		(UObject* (*)())Z_Construct_UClass_UObject,
		(UObject* (*)())Z_Construct_UPackage__Script_PuertsEditor,
	};
	const FClassFunctionLinkInfo Z_Construct_UClass_UPEFunctionMetaData_Statics::FuncInfo[] = {
		{ &Z_Construct_UFunction_UPEFunctionMetaData_SetCppImplName, "SetCppImplName" }, // 3202350120
		{ &Z_Construct_UFunction_UPEFunctionMetaData_SetCppValidationImplName, "SetCppValidationImplName" }, // 3346845866
		{ &Z_Construct_UFunction_UPEFunctionMetaData_SetEndpointName, "SetEndpointName" }, // 625948647
		{ &Z_Construct_UFunction_UPEFunctionMetaData_SetForceBlueprintImpure, "SetForceBlueprintImpure" }, // 4096888877
		{ &Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionExportFlags, "SetFunctionExportFlags" }, // 2439672544
		{ &Z_Construct_UFunction_UPEFunctionMetaData_SetFunctionFlags, "SetFunctionFlags" }, // 3471823919
		{ &Z_Construct_UFunction_UPEFunctionMetaData_SetIsSealedEvent, "SetIsSealedEvent" }, // 2419127231
		{ &Z_Construct_UFunction_UPEFunctionMetaData_SetMetaData, "SetMetaData" }, // 130737550
		{ &Z_Construct_UFunction_UPEFunctionMetaData_SetRPCId, "SetRPCId" }, // 1779533830
		{ &Z_Construct_UFunction_UPEFunctionMetaData_SetRPCResponseId, "SetRPCResponseId" }, // 220819095
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UPEFunctionMetaData_Statics::Class_MetaDataParams[] = {
		{ "Comment", "/**\n * @brief a helper structure used to store the meta data of a function, @see ParseHelper.h\n */" },
		{ "IncludePath", "PEBlueprintMetaData.h" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief a helper structure used to store the meta data of a function, @see ParseHelper.h" },
	};
#endif
	const FCppClassTypeInfoStatic Z_Construct_UClass_UPEFunctionMetaData_Statics::StaticCppClassTypeInfo = {
		TCppClassTypeTraits<UPEFunctionMetaData>::IsAbstract,
	};
	const UE4CodeGen_Private::FClassParams Z_Construct_UClass_UPEFunctionMetaData_Statics::ClassParams = {
		&UPEFunctionMetaData::StaticClass,
		nullptr,
		&StaticCppClassTypeInfo,
		DependentSingletons,
		FuncInfo,
		nullptr,
		nullptr,
		UE_ARRAY_COUNT(DependentSingletons),
		UE_ARRAY_COUNT(FuncInfo),
		0,
		0,
		0x001000A0u,
		METADATA_PARAMS(Z_Construct_UClass_UPEFunctionMetaData_Statics::Class_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UClass_UPEFunctionMetaData_Statics::Class_MetaDataParams))
	};
	UClass* Z_Construct_UClass_UPEFunctionMetaData()
	{
		static UClass* OuterClass = nullptr;
		if (!OuterClass)
		{
			UE4CodeGen_Private::ConstructUClass(OuterClass, Z_Construct_UClass_UPEFunctionMetaData_Statics::ClassParams);
		}
		return OuterClass;
	}
	IMPLEMENT_CLASS(UPEFunctionMetaData, 3249597513);
	template<> PUERTSEDITOR_API UClass* StaticClass<UPEFunctionMetaData>()
	{
		return UPEFunctionMetaData::StaticClass();
	}
	static FCompiledInDefer Z_CompiledInDefer_UClass_UPEFunctionMetaData(Z_Construct_UClass_UPEFunctionMetaData, &UPEFunctionMetaData::StaticClass, TEXT("/Script/PuertsEditor"), TEXT("UPEFunctionMetaData"), false, nullptr, nullptr, nullptr);
	DEFINE_VTABLE_PTR_HELPER_CTOR(UPEFunctionMetaData);
	DEFINE_FUNCTION(UPEParamMetaData::execSetMetaData)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_InName);
		P_GET_PROPERTY(FStrProperty,Z_Param_InValue);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->SetMetaData(Z_Param_InName,Z_Param_InValue);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEParamMetaData::execSetParamFlags)
	{
		P_GET_PROPERTY(FIntProperty,Z_Param_InHighBits);
		P_GET_PROPERTY(FIntProperty,Z_Param_InLowBits);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->SetParamFlags(Z_Param_InHighBits,Z_Param_InLowBits);
		P_NATIVE_END;
	}
	void UPEParamMetaData::StaticRegisterNativesUPEParamMetaData()
	{
		UClass* Class = UPEParamMetaData::StaticClass();
		static const FNameNativePtrPair Funcs[] = {
			{ "SetMetaData", &UPEParamMetaData::execSetMetaData },
			{ "SetParamFlags", &UPEParamMetaData::execSetParamFlags },
		};
		FNativeFunctionRegistrar::RegisterFunctions(Class, Funcs, UE_ARRAY_COUNT(Funcs));
	}
	struct Z_Construct_UFunction_UPEParamMetaData_SetMetaData_Statics
	{
		struct PEParamMetaData_eventSetMetaData_Parms
		{
			FString InName;
			FString InValue;
		};
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InName_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InName;
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InValue_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InValue;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEParamMetaData_SetMetaData_Statics::NewProp_InName_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEParamMetaData_SetMetaData_Statics::NewProp_InName = { "InName", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEParamMetaData_eventSetMetaData_Parms, InName), METADATA_PARAMS(Z_Construct_UFunction_UPEParamMetaData_SetMetaData_Statics::NewProp_InName_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEParamMetaData_SetMetaData_Statics::NewProp_InName_MetaData)) };
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEParamMetaData_SetMetaData_Statics::NewProp_InValue_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEParamMetaData_SetMetaData_Statics::NewProp_InValue = { "InValue", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEParamMetaData_eventSetMetaData_Parms, InValue), METADATA_PARAMS(Z_Construct_UFunction_UPEParamMetaData_SetMetaData_Statics::NewProp_InValue_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEParamMetaData_SetMetaData_Statics::NewProp_InValue_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEParamMetaData_SetMetaData_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEParamMetaData_SetMetaData_Statics::NewProp_InName,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEParamMetaData_SetMetaData_Statics::NewProp_InValue,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEParamMetaData_SetMetaData_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief set specific meta data\n     * @param InName\n     * @param InValue\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief set specific meta data\n@param InName\n@param InValue" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEParamMetaData_SetMetaData_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEParamMetaData, nullptr, "SetMetaData", nullptr, nullptr, sizeof(PEParamMetaData_eventSetMetaData_Parms), Z_Construct_UFunction_UPEParamMetaData_SetMetaData_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEParamMetaData_SetMetaData_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEParamMetaData_SetMetaData_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEParamMetaData_SetMetaData_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEParamMetaData_SetMetaData()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEParamMetaData_SetMetaData_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEParamMetaData_SetParamFlags_Statics
	{
		struct PEParamMetaData_eventSetParamFlags_Parms
		{
			int32 InHighBits;
			int32 InLowBits;
		};
		static const UE4CodeGen_Private::FIntPropertyParams NewProp_InHighBits;
		static const UE4CodeGen_Private::FIntPropertyParams NewProp_InLowBits;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
	const UE4CodeGen_Private::FIntPropertyParams Z_Construct_UFunction_UPEParamMetaData_SetParamFlags_Statics::NewProp_InHighBits = { "InHighBits", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Int, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEParamMetaData_eventSetParamFlags_Parms, InHighBits), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FIntPropertyParams Z_Construct_UFunction_UPEParamMetaData_SetParamFlags_Statics::NewProp_InLowBits = { "InLowBits", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Int, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEParamMetaData_eventSetParamFlags_Parms, InLowBits), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEParamMetaData_SetParamFlags_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEParamMetaData_SetParamFlags_Statics::NewProp_InHighBits,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEParamMetaData_SetParamFlags_Statics::NewProp_InLowBits,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEParamMetaData_SetParamFlags_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief set the parameter's property flags\n     * @param InHighBits\n     * @param InLowBits\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief set the parameter's property flags\n@param InHighBits\n@param InLowBits" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEParamMetaData_SetParamFlags_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEParamMetaData, nullptr, "SetParamFlags", nullptr, nullptr, sizeof(PEParamMetaData_eventSetParamFlags_Parms), Z_Construct_UFunction_UPEParamMetaData_SetParamFlags_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEParamMetaData_SetParamFlags_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEParamMetaData_SetParamFlags_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEParamMetaData_SetParamFlags_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEParamMetaData_SetParamFlags()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEParamMetaData_SetParamFlags_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	UClass* Z_Construct_UClass_UPEParamMetaData_NoRegister()
	{
		return UPEParamMetaData::StaticClass();
	}
	struct Z_Construct_UClass_UPEParamMetaData_Statics
	{
		static UObject* (*const DependentSingletons[])();
		static const FClassFunctionLinkInfo FuncInfo[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Class_MetaDataParams[];
#endif
		static const FCppClassTypeInfoStatic StaticCppClassTypeInfo;
		static const UE4CodeGen_Private::FClassParams ClassParams;
	};
	UObject* (*const Z_Construct_UClass_UPEParamMetaData_Statics::DependentSingletons[])() = {
		(UObject* (*)())Z_Construct_UClass_UObject,
		(UObject* (*)())Z_Construct_UPackage__Script_PuertsEditor,
	};
	const FClassFunctionLinkInfo Z_Construct_UClass_UPEParamMetaData_Statics::FuncInfo[] = {
		{ &Z_Construct_UFunction_UPEParamMetaData_SetMetaData, "SetMetaData" }, // 2587630116
		{ &Z_Construct_UFunction_UPEParamMetaData_SetParamFlags, "SetParamFlags" }, // 4127044004
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UPEParamMetaData_Statics::Class_MetaDataParams[] = {
		{ "Comment", "/**\n * @brief a helper structure used to store the meta data of a function parameter\n */" },
		{ "IncludePath", "PEBlueprintMetaData.h" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief a helper structure used to store the meta data of a function parameter" },
	};
#endif
	const FCppClassTypeInfoStatic Z_Construct_UClass_UPEParamMetaData_Statics::StaticCppClassTypeInfo = {
		TCppClassTypeTraits<UPEParamMetaData>::IsAbstract,
	};
	const UE4CodeGen_Private::FClassParams Z_Construct_UClass_UPEParamMetaData_Statics::ClassParams = {
		&UPEParamMetaData::StaticClass,
		nullptr,
		&StaticCppClassTypeInfo,
		DependentSingletons,
		FuncInfo,
		nullptr,
		nullptr,
		UE_ARRAY_COUNT(DependentSingletons),
		UE_ARRAY_COUNT(FuncInfo),
		0,
		0,
		0x001000A0u,
		METADATA_PARAMS(Z_Construct_UClass_UPEParamMetaData_Statics::Class_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UClass_UPEParamMetaData_Statics::Class_MetaDataParams))
	};
	UClass* Z_Construct_UClass_UPEParamMetaData()
	{
		static UClass* OuterClass = nullptr;
		if (!OuterClass)
		{
			UE4CodeGen_Private::ConstructUClass(OuterClass, Z_Construct_UClass_UPEParamMetaData_Statics::ClassParams);
		}
		return OuterClass;
	}
	IMPLEMENT_CLASS(UPEParamMetaData, 3604128902);
	template<> PUERTSEDITOR_API UClass* StaticClass<UPEParamMetaData>()
	{
		return UPEParamMetaData::StaticClass();
	}
	static FCompiledInDefer Z_CompiledInDefer_UClass_UPEParamMetaData(Z_Construct_UClass_UPEParamMetaData, &UPEParamMetaData::StaticClass, TEXT("/Script/PuertsEditor"), TEXT("UPEParamMetaData"), false, nullptr, nullptr, nullptr);
	DEFINE_VTABLE_PTR_HELPER_CTOR(UPEParamMetaData);
	DEFINE_FUNCTION(UPEPropertyMetaData::execSetRepCallbackName)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_InName);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->SetRepCallbackName(Z_Param_InName);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEPropertyMetaData::execSetMetaData)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_InName);
		P_GET_PROPERTY(FStrProperty,Z_Param_InValue);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->SetMetaData(Z_Param_InName,Z_Param_InValue);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UPEPropertyMetaData::execSetPropertyFlags)
	{
		P_GET_PROPERTY(FIntProperty,Z_Param_InHighBits);
		P_GET_PROPERTY(FIntProperty,Z_Param_InLowBits);
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->SetPropertyFlags(Z_Param_InHighBits,Z_Param_InLowBits);
		P_NATIVE_END;
	}
	void UPEPropertyMetaData::StaticRegisterNativesUPEPropertyMetaData()
	{
		UClass* Class = UPEPropertyMetaData::StaticClass();
		static const FNameNativePtrPair Funcs[] = {
			{ "SetMetaData", &UPEPropertyMetaData::execSetMetaData },
			{ "SetPropertyFlags", &UPEPropertyMetaData::execSetPropertyFlags },
			{ "SetRepCallbackName", &UPEPropertyMetaData::execSetRepCallbackName },
		};
		FNativeFunctionRegistrar::RegisterFunctions(Class, Funcs, UE_ARRAY_COUNT(Funcs));
	}
	struct Z_Construct_UFunction_UPEPropertyMetaData_SetMetaData_Statics
	{
		struct PEPropertyMetaData_eventSetMetaData_Parms
		{
			FString InName;
			FString InValue;
		};
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InName_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InName;
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InValue_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InValue;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEPropertyMetaData_SetMetaData_Statics::NewProp_InName_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEPropertyMetaData_SetMetaData_Statics::NewProp_InName = { "InName", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEPropertyMetaData_eventSetMetaData_Parms, InName), METADATA_PARAMS(Z_Construct_UFunction_UPEPropertyMetaData_SetMetaData_Statics::NewProp_InName_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEPropertyMetaData_SetMetaData_Statics::NewProp_InName_MetaData)) };
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEPropertyMetaData_SetMetaData_Statics::NewProp_InValue_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEPropertyMetaData_SetMetaData_Statics::NewProp_InValue = { "InValue", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEPropertyMetaData_eventSetMetaData_Parms, InValue), METADATA_PARAMS(Z_Construct_UFunction_UPEPropertyMetaData_SetMetaData_Statics::NewProp_InValue_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEPropertyMetaData_SetMetaData_Statics::NewProp_InValue_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEPropertyMetaData_SetMetaData_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEPropertyMetaData_SetMetaData_Statics::NewProp_InName,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEPropertyMetaData_SetMetaData_Statics::NewProp_InValue,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEPropertyMetaData_SetMetaData_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief set the meta data of the property\n     * @param InName\n     * @param InValue\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief set the meta data of the property\n@param InName\n@param InValue" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEPropertyMetaData_SetMetaData_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEPropertyMetaData, nullptr, "SetMetaData", nullptr, nullptr, sizeof(PEPropertyMetaData_eventSetMetaData_Parms), Z_Construct_UFunction_UPEPropertyMetaData_SetMetaData_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEPropertyMetaData_SetMetaData_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEPropertyMetaData_SetMetaData_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEPropertyMetaData_SetMetaData_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEPropertyMetaData_SetMetaData()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEPropertyMetaData_SetMetaData_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEPropertyMetaData_SetPropertyFlags_Statics
	{
		struct PEPropertyMetaData_eventSetPropertyFlags_Parms
		{
			int32 InHighBits;
			int32 InLowBits;
		};
		static const UE4CodeGen_Private::FIntPropertyParams NewProp_InHighBits;
		static const UE4CodeGen_Private::FIntPropertyParams NewProp_InLowBits;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
	const UE4CodeGen_Private::FIntPropertyParams Z_Construct_UFunction_UPEPropertyMetaData_SetPropertyFlags_Statics::NewProp_InHighBits = { "InHighBits", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Int, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEPropertyMetaData_eventSetPropertyFlags_Parms, InHighBits), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FIntPropertyParams Z_Construct_UFunction_UPEPropertyMetaData_SetPropertyFlags_Statics::NewProp_InLowBits = { "InLowBits", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Int, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEPropertyMetaData_eventSetPropertyFlags_Parms, InLowBits), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEPropertyMetaData_SetPropertyFlags_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEPropertyMetaData_SetPropertyFlags_Statics::NewProp_InHighBits,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEPropertyMetaData_SetPropertyFlags_Statics::NewProp_InLowBits,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEPropertyMetaData_SetPropertyFlags_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief set the property flags\n     * @param InHighBits\n     * @param InLowBits\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief set the property flags\n@param InHighBits\n@param InLowBits" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEPropertyMetaData_SetPropertyFlags_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEPropertyMetaData, nullptr, "SetPropertyFlags", nullptr, nullptr, sizeof(PEPropertyMetaData_eventSetPropertyFlags_Parms), Z_Construct_UFunction_UPEPropertyMetaData_SetPropertyFlags_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEPropertyMetaData_SetPropertyFlags_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEPropertyMetaData_SetPropertyFlags_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEPropertyMetaData_SetPropertyFlags_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEPropertyMetaData_SetPropertyFlags()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEPropertyMetaData_SetPropertyFlags_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UPEPropertyMetaData_SetRepCallbackName_Statics
	{
		struct PEPropertyMetaData_eventSetRepCallbackName_Parms
		{
			FString InName;
		};
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_InName_MetaData[];
#endif
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_InName;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEPropertyMetaData_SetRepCallbackName_Statics::NewProp_InName_MetaData[] = {
		{ "NativeConst", "" },
	};
#endif
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UPEPropertyMetaData_SetRepCallbackName_Statics::NewProp_InName = { "InName", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(PEPropertyMetaData_eventSetRepCallbackName_Parms, InName), METADATA_PARAMS(Z_Construct_UFunction_UPEPropertyMetaData_SetRepCallbackName_Statics::NewProp_InName_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEPropertyMetaData_SetRepCallbackName_Statics::NewProp_InName_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UPEPropertyMetaData_SetRepCallbackName_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UPEPropertyMetaData_SetRepCallbackName_Statics::NewProp_InName,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UPEPropertyMetaData_SetRepCallbackName_Statics::Function_MetaDataParams[] = {
		{ "Comment", "/**\n     * @brief set the rep callback name\n     * @param InName\n     */" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief set the rep callback name\n@param InName" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UPEPropertyMetaData_SetRepCallbackName_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UPEPropertyMetaData, nullptr, "SetRepCallbackName", nullptr, nullptr, sizeof(PEPropertyMetaData_eventSetRepCallbackName_Parms), Z_Construct_UFunction_UPEPropertyMetaData_SetRepCallbackName_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEPropertyMetaData_SetRepCallbackName_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x00020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UPEPropertyMetaData_SetRepCallbackName_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UPEPropertyMetaData_SetRepCallbackName_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UPEPropertyMetaData_SetRepCallbackName()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UPEPropertyMetaData_SetRepCallbackName_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	UClass* Z_Construct_UClass_UPEPropertyMetaData_NoRegister()
	{
		return UPEPropertyMetaData::StaticClass();
	}
	struct Z_Construct_UClass_UPEPropertyMetaData_Statics
	{
		static UObject* (*const DependentSingletons[])();
		static const FClassFunctionLinkInfo FuncInfo[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Class_MetaDataParams[];
#endif
		static const FCppClassTypeInfoStatic StaticCppClassTypeInfo;
		static const UE4CodeGen_Private::FClassParams ClassParams;
	};
	UObject* (*const Z_Construct_UClass_UPEPropertyMetaData_Statics::DependentSingletons[])() = {
		(UObject* (*)())Z_Construct_UClass_UObject,
		(UObject* (*)())Z_Construct_UPackage__Script_PuertsEditor,
	};
	const FClassFunctionLinkInfo Z_Construct_UClass_UPEPropertyMetaData_Statics::FuncInfo[] = {
		{ &Z_Construct_UFunction_UPEPropertyMetaData_SetMetaData, "SetMetaData" }, // 1918233523
		{ &Z_Construct_UFunction_UPEPropertyMetaData_SetPropertyFlags, "SetPropertyFlags" }, // 1949859589
		{ &Z_Construct_UFunction_UPEPropertyMetaData_SetRepCallbackName, "SetRepCallbackName" }, // 2242595584
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UPEPropertyMetaData_Statics::Class_MetaDataParams[] = {
		{ "Comment", "/**\n * @brief a helper function used to store the meta data of a class's member variable\n */" },
		{ "IncludePath", "PEBlueprintMetaData.h" },
		{ "ModuleRelativePath", "Public/PEBlueprintMetaData.h" },
		{ "ToolTip", "@brief a helper function used to store the meta data of a class's member variable" },
	};
#endif
	const FCppClassTypeInfoStatic Z_Construct_UClass_UPEPropertyMetaData_Statics::StaticCppClassTypeInfo = {
		TCppClassTypeTraits<UPEPropertyMetaData>::IsAbstract,
	};
	const UE4CodeGen_Private::FClassParams Z_Construct_UClass_UPEPropertyMetaData_Statics::ClassParams = {
		&UPEPropertyMetaData::StaticClass,
		nullptr,
		&StaticCppClassTypeInfo,
		DependentSingletons,
		FuncInfo,
		nullptr,
		nullptr,
		UE_ARRAY_COUNT(DependentSingletons),
		UE_ARRAY_COUNT(FuncInfo),
		0,
		0,
		0x001000A0u,
		METADATA_PARAMS(Z_Construct_UClass_UPEPropertyMetaData_Statics::Class_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UClass_UPEPropertyMetaData_Statics::Class_MetaDataParams))
	};
	UClass* Z_Construct_UClass_UPEPropertyMetaData()
	{
		static UClass* OuterClass = nullptr;
		if (!OuterClass)
		{
			UE4CodeGen_Private::ConstructUClass(OuterClass, Z_Construct_UClass_UPEPropertyMetaData_Statics::ClassParams);
		}
		return OuterClass;
	}
	IMPLEMENT_CLASS(UPEPropertyMetaData, 1834704079);
	template<> PUERTSEDITOR_API UClass* StaticClass<UPEPropertyMetaData>()
	{
		return UPEPropertyMetaData::StaticClass();
	}
	static FCompiledInDefer Z_CompiledInDefer_UClass_UPEPropertyMetaData(Z_Construct_UClass_UPEPropertyMetaData, &UPEPropertyMetaData::StaticClass, TEXT("/Script/PuertsEditor"), TEXT("UPEPropertyMetaData"), false, nullptr, nullptr, nullptr);
	DEFINE_VTABLE_PTR_HELPER_CTOR(UPEPropertyMetaData);
PRAGMA_ENABLE_DEPRECATION_WARNINGS
#ifdef _MSC_VER
#pragma warning (pop)
#endif
