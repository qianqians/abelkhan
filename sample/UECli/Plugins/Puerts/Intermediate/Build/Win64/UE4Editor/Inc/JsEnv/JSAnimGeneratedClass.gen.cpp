// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/GeneratedCppIncludes.h"
#include "JsEnv/Private/JSAnimGeneratedClass.h"
#ifdef _MSC_VER
#pragma warning (push)
#pragma warning (disable : 4883)
#endif
PRAGMA_DISABLE_DEPRECATION_WARNINGS
void EmptyLinkFunctionForGeneratedCodeJSAnimGeneratedClass() {}
// Cross Module References
	JSENV_API UClass* Z_Construct_UClass_UJSAnimGeneratedClass_NoRegister();
	JSENV_API UClass* Z_Construct_UClass_UJSAnimGeneratedClass();
	ENGINE_API UClass* Z_Construct_UClass_UAnimBlueprintGeneratedClass();
	UPackage* Z_Construct_UPackage__Script_JsEnv();
// End Cross Module References
	void UJSAnimGeneratedClass::StaticRegisterNativesUJSAnimGeneratedClass()
	{
	}
	UClass* Z_Construct_UClass_UJSAnimGeneratedClass_NoRegister()
	{
		return UJSAnimGeneratedClass::StaticClass();
	}
	struct Z_Construct_UClass_UJSAnimGeneratedClass_Statics
	{
		static UObject* (*const DependentSingletons[])();
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Class_MetaDataParams[];
#endif
		static const FCppClassTypeInfoStatic StaticCppClassTypeInfo;
		static const UE4CodeGen_Private::FClassParams ClassParams;
	};
	UObject* (*const Z_Construct_UClass_UJSAnimGeneratedClass_Statics::DependentSingletons[])() = {
		(UObject* (*)())Z_Construct_UClass_UAnimBlueprintGeneratedClass,
		(UObject* (*)())Z_Construct_UPackage__Script_JsEnv,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UJSAnimGeneratedClass_Statics::Class_MetaDataParams[] = {
		{ "Comment", "/**\n *\n */" },
		{ "IncludePath", "JSAnimGeneratedClass.h" },
		{ "ModuleRelativePath", "Private/JSAnimGeneratedClass.h" },
	};
#endif
	const FCppClassTypeInfoStatic Z_Construct_UClass_UJSAnimGeneratedClass_Statics::StaticCppClassTypeInfo = {
		TCppClassTypeTraits<UJSAnimGeneratedClass>::IsAbstract,
	};
	const UE4CodeGen_Private::FClassParams Z_Construct_UClass_UJSAnimGeneratedClass_Statics::ClassParams = {
		&UJSAnimGeneratedClass::StaticClass,
		nullptr,
		&StaticCppClassTypeInfo,
		DependentSingletons,
		nullptr,
		nullptr,
		nullptr,
		UE_ARRAY_COUNT(DependentSingletons),
		0,
		0,
		0,
		0x008000A0u,
		METADATA_PARAMS(Z_Construct_UClass_UJSAnimGeneratedClass_Statics::Class_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UClass_UJSAnimGeneratedClass_Statics::Class_MetaDataParams))
	};
	UClass* Z_Construct_UClass_UJSAnimGeneratedClass()
	{
		static UClass* OuterClass = nullptr;
		if (!OuterClass)
		{
			UE4CodeGen_Private::ConstructUClass(OuterClass, Z_Construct_UClass_UJSAnimGeneratedClass_Statics::ClassParams);
		}
		return OuterClass;
	}
	IMPLEMENT_CLASS(UJSAnimGeneratedClass, 3264345130);
	template<> JSENV_API UClass* StaticClass<UJSAnimGeneratedClass>()
	{
		return UJSAnimGeneratedClass::StaticClass();
	}
	static FCompiledInDefer Z_CompiledInDefer_UClass_UJSAnimGeneratedClass(Z_Construct_UClass_UJSAnimGeneratedClass, &UJSAnimGeneratedClass::StaticClass, TEXT("/Script/JsEnv"), TEXT("UJSAnimGeneratedClass"), false, nullptr, nullptr, nullptr);
	DEFINE_VTABLE_PTR_HELPER_CTOR(UJSAnimGeneratedClass);
PRAGMA_ENABLE_DEPRECATION_WARNINGS
#ifdef _MSC_VER
#pragma warning (pop)
#endif
