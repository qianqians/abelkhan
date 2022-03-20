// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/GeneratedCppIncludes.h"
#include "JsEnv/Public/TypeScriptGeneratedClass.h"
#ifdef _MSC_VER
#pragma warning (push)
#pragma warning (disable : 4883)
#endif
PRAGMA_DISABLE_DEPRECATION_WARNINGS
void EmptyLinkFunctionForGeneratedCodeTypeScriptGeneratedClass() {}
// Cross Module References
	JSENV_API UClass* Z_Construct_UClass_UTypeScriptGeneratedClass_NoRegister();
	JSENV_API UClass* Z_Construct_UClass_UTypeScriptGeneratedClass();
	ENGINE_API UClass* Z_Construct_UClass_UBlueprintGeneratedClass();
	UPackage* Z_Construct_UPackage__Script_JsEnv();
// End Cross Module References
	void UTypeScriptGeneratedClass::StaticRegisterNativesUTypeScriptGeneratedClass()
	{
	}
	UClass* Z_Construct_UClass_UTypeScriptGeneratedClass_NoRegister()
	{
		return UTypeScriptGeneratedClass::StaticClass();
	}
	struct Z_Construct_UClass_UTypeScriptGeneratedClass_Statics
	{
		static UObject* (*const DependentSingletons[])();
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Class_MetaDataParams[];
#endif
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_HasConstructor_MetaData[];
#endif
		static void NewProp_HasConstructor_SetBit(void* Obj);
		static const UE4CodeGen_Private::FBoolPropertyParams NewProp_HasConstructor;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
		static const FCppClassTypeInfoStatic StaticCppClassTypeInfo;
		static const UE4CodeGen_Private::FClassParams ClassParams;
	};
	UObject* (*const Z_Construct_UClass_UTypeScriptGeneratedClass_Statics::DependentSingletons[])() = {
		(UObject* (*)())Z_Construct_UClass_UBlueprintGeneratedClass,
		(UObject* (*)())Z_Construct_UPackage__Script_JsEnv,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UTypeScriptGeneratedClass_Statics::Class_MetaDataParams[] = {
		{ "Comment", "/**\n *\n */" },
		{ "IncludePath", "TypeScriptGeneratedClass.h" },
		{ "ModuleRelativePath", "Public/TypeScriptGeneratedClass.h" },
	};
#endif
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UTypeScriptGeneratedClass_Statics::NewProp_HasConstructor_MetaData[] = {
		{ "ModuleRelativePath", "Public/TypeScriptGeneratedClass.h" },
	};
#endif
	void Z_Construct_UClass_UTypeScriptGeneratedClass_Statics::NewProp_HasConstructor_SetBit(void* Obj)
	{
		((UTypeScriptGeneratedClass*)Obj)->HasConstructor = 1;
	}
	const UE4CodeGen_Private::FBoolPropertyParams Z_Construct_UClass_UTypeScriptGeneratedClass_Statics::NewProp_HasConstructor = { "HasConstructor", nullptr, (EPropertyFlags)0x0010000000000000, UE4CodeGen_Private::EPropertyGenFlags::Bool | UE4CodeGen_Private::EPropertyGenFlags::NativeBool, RF_Public|RF_Transient|RF_MarkAsNative, 1, sizeof(bool), sizeof(UTypeScriptGeneratedClass), &Z_Construct_UClass_UTypeScriptGeneratedClass_Statics::NewProp_HasConstructor_SetBit, METADATA_PARAMS(Z_Construct_UClass_UTypeScriptGeneratedClass_Statics::NewProp_HasConstructor_MetaData, UE_ARRAY_COUNT(Z_Construct_UClass_UTypeScriptGeneratedClass_Statics::NewProp_HasConstructor_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UClass_UTypeScriptGeneratedClass_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UClass_UTypeScriptGeneratedClass_Statics::NewProp_HasConstructor,
	};
	const FCppClassTypeInfoStatic Z_Construct_UClass_UTypeScriptGeneratedClass_Statics::StaticCppClassTypeInfo = {
		TCppClassTypeTraits<UTypeScriptGeneratedClass>::IsAbstract,
	};
	const UE4CodeGen_Private::FClassParams Z_Construct_UClass_UTypeScriptGeneratedClass_Statics::ClassParams = {
		&UTypeScriptGeneratedClass::StaticClass,
		nullptr,
		&StaticCppClassTypeInfo,
		DependentSingletons,
		nullptr,
		Z_Construct_UClass_UTypeScriptGeneratedClass_Statics::PropPointers,
		nullptr,
		UE_ARRAY_COUNT(DependentSingletons),
		0,
		UE_ARRAY_COUNT(Z_Construct_UClass_UTypeScriptGeneratedClass_Statics::PropPointers),
		0,
		0x009000A0u,
		METADATA_PARAMS(Z_Construct_UClass_UTypeScriptGeneratedClass_Statics::Class_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UClass_UTypeScriptGeneratedClass_Statics::Class_MetaDataParams))
	};
	UClass* Z_Construct_UClass_UTypeScriptGeneratedClass()
	{
		static UClass* OuterClass = nullptr;
		if (!OuterClass)
		{
			UE4CodeGen_Private::ConstructUClass(OuterClass, Z_Construct_UClass_UTypeScriptGeneratedClass_Statics::ClassParams);
		}
		return OuterClass;
	}
	IMPLEMENT_CLASS(UTypeScriptGeneratedClass, 1792640945);
	template<> JSENV_API UClass* StaticClass<UTypeScriptGeneratedClass>()
	{
		return UTypeScriptGeneratedClass::StaticClass();
	}
	static FCompiledInDefer Z_CompiledInDefer_UClass_UTypeScriptGeneratedClass(Z_Construct_UClass_UTypeScriptGeneratedClass, &UTypeScriptGeneratedClass::StaticClass, TEXT("/Script/JsEnv"), TEXT("UTypeScriptGeneratedClass"), false, nullptr, nullptr, nullptr);
	DEFINE_VTABLE_PTR_HELPER_CTOR(UTypeScriptGeneratedClass);
PRAGMA_ENABLE_DEPRECATION_WARNINGS
#ifdef _MSC_VER
#pragma warning (pop)
#endif
