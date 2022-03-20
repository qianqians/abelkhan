// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/GeneratedCppIncludes.h"
#include "JsEnv/Public/TypeScriptBlueprint.h"
#ifdef _MSC_VER
#pragma warning (push)
#pragma warning (disable : 4883)
#endif
PRAGMA_DISABLE_DEPRECATION_WARNINGS
void EmptyLinkFunctionForGeneratedCodeTypeScriptBlueprint() {}
// Cross Module References
	JSENV_API UClass* Z_Construct_UClass_UTypeScriptBlueprint_NoRegister();
	JSENV_API UClass* Z_Construct_UClass_UTypeScriptBlueprint();
	ENGINE_API UClass* Z_Construct_UClass_UBlueprint();
	UPackage* Z_Construct_UPackage__Script_JsEnv();
// End Cross Module References
	void UTypeScriptBlueprint::StaticRegisterNativesUTypeScriptBlueprint()
	{
	}
	UClass* Z_Construct_UClass_UTypeScriptBlueprint_NoRegister()
	{
		return UTypeScriptBlueprint::StaticClass();
	}
	struct Z_Construct_UClass_UTypeScriptBlueprint_Statics
	{
		static UObject* (*const DependentSingletons[])();
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Class_MetaDataParams[];
#endif
		static const FCppClassTypeInfoStatic StaticCppClassTypeInfo;
		static const UE4CodeGen_Private::FClassParams ClassParams;
	};
	UObject* (*const Z_Construct_UClass_UTypeScriptBlueprint_Statics::DependentSingletons[])() = {
		(UObject* (*)())Z_Construct_UClass_UBlueprint,
		(UObject* (*)())Z_Construct_UPackage__Script_JsEnv,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UTypeScriptBlueprint_Statics::Class_MetaDataParams[] = {
		{ "Comment", "/**\n *\n */" },
		{ "IncludePath", "TypeScriptBlueprint.h" },
		{ "ModuleRelativePath", "Public/TypeScriptBlueprint.h" },
	};
#endif
	const FCppClassTypeInfoStatic Z_Construct_UClass_UTypeScriptBlueprint_Statics::StaticCppClassTypeInfo = {
		TCppClassTypeTraits<UTypeScriptBlueprint>::IsAbstract,
	};
	const UE4CodeGen_Private::FClassParams Z_Construct_UClass_UTypeScriptBlueprint_Statics::ClassParams = {
		&UTypeScriptBlueprint::StaticClass,
		"Engine",
		&StaticCppClassTypeInfo,
		DependentSingletons,
		nullptr,
		nullptr,
		nullptr,
		UE_ARRAY_COUNT(DependentSingletons),
		0,
		0,
		0,
		0x009000A4u,
		METADATA_PARAMS(Z_Construct_UClass_UTypeScriptBlueprint_Statics::Class_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UClass_UTypeScriptBlueprint_Statics::Class_MetaDataParams))
	};
	UClass* Z_Construct_UClass_UTypeScriptBlueprint()
	{
		static UClass* OuterClass = nullptr;
		if (!OuterClass)
		{
			UE4CodeGen_Private::ConstructUClass(OuterClass, Z_Construct_UClass_UTypeScriptBlueprint_Statics::ClassParams);
		}
		return OuterClass;
	}
	IMPLEMENT_CLASS(UTypeScriptBlueprint, 1480693231);
	template<> JSENV_API UClass* StaticClass<UTypeScriptBlueprint>()
	{
		return UTypeScriptBlueprint::StaticClass();
	}
	static FCompiledInDefer Z_CompiledInDefer_UClass_UTypeScriptBlueprint(Z_Construct_UClass_UTypeScriptBlueprint, &UTypeScriptBlueprint::StaticClass, TEXT("/Script/JsEnv"), TEXT("UTypeScriptBlueprint"), false, nullptr, nullptr, nullptr);
	DEFINE_VTABLE_PTR_HELPER_CTOR(UTypeScriptBlueprint);
PRAGMA_ENABLE_DEPRECATION_WARNINGS
#ifdef _MSC_VER
#pragma warning (pop)
#endif
