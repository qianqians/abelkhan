// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/GeneratedCppIncludes.h"
#include "JsEnv/Private/JSGeneratedClass.h"
#ifdef _MSC_VER
#pragma warning (push)
#pragma warning (disable : 4883)
#endif
PRAGMA_DISABLE_DEPRECATION_WARNINGS
void EmptyLinkFunctionForGeneratedCodeJSGeneratedClass() {}
// Cross Module References
	JSENV_API UClass* Z_Construct_UClass_UJSGeneratedClass_NoRegister();
	JSENV_API UClass* Z_Construct_UClass_UJSGeneratedClass();
	ENGINE_API UClass* Z_Construct_UClass_UBlueprintGeneratedClass();
	UPackage* Z_Construct_UPackage__Script_JsEnv();
// End Cross Module References
	void UJSGeneratedClass::StaticRegisterNativesUJSGeneratedClass()
	{
	}
	UClass* Z_Construct_UClass_UJSGeneratedClass_NoRegister()
	{
		return UJSGeneratedClass::StaticClass();
	}
	struct Z_Construct_UClass_UJSGeneratedClass_Statics
	{
		static UObject* (*const DependentSingletons[])();
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Class_MetaDataParams[];
#endif
		static const FCppClassTypeInfoStatic StaticCppClassTypeInfo;
		static const UE4CodeGen_Private::FClassParams ClassParams;
	};
	UObject* (*const Z_Construct_UClass_UJSGeneratedClass_Statics::DependentSingletons[])() = {
		(UObject* (*)())Z_Construct_UClass_UBlueprintGeneratedClass,
		(UObject* (*)())Z_Construct_UPackage__Script_JsEnv,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UJSGeneratedClass_Statics::Class_MetaDataParams[] = {
		{ "Comment", "/**\n *\n */" },
		{ "IncludePath", "JSGeneratedClass.h" },
		{ "ModuleRelativePath", "Private/JSGeneratedClass.h" },
	};
#endif
	const FCppClassTypeInfoStatic Z_Construct_UClass_UJSGeneratedClass_Statics::StaticCppClassTypeInfo = {
		TCppClassTypeTraits<UJSGeneratedClass>::IsAbstract,
	};
	const UE4CodeGen_Private::FClassParams Z_Construct_UClass_UJSGeneratedClass_Statics::ClassParams = {
		&UJSGeneratedClass::StaticClass,
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
		METADATA_PARAMS(Z_Construct_UClass_UJSGeneratedClass_Statics::Class_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UClass_UJSGeneratedClass_Statics::Class_MetaDataParams))
	};
	UClass* Z_Construct_UClass_UJSGeneratedClass()
	{
		static UClass* OuterClass = nullptr;
		if (!OuterClass)
		{
			UE4CodeGen_Private::ConstructUClass(OuterClass, Z_Construct_UClass_UJSGeneratedClass_Statics::ClassParams);
		}
		return OuterClass;
	}
	IMPLEMENT_CLASS(UJSGeneratedClass, 1314038337);
	template<> JSENV_API UClass* StaticClass<UJSGeneratedClass>()
	{
		return UJSGeneratedClass::StaticClass();
	}
	static FCompiledInDefer Z_CompiledInDefer_UClass_UJSGeneratedClass(Z_Construct_UClass_UJSGeneratedClass, &UJSGeneratedClass::StaticClass, TEXT("/Script/JsEnv"), TEXT("UJSGeneratedClass"), false, nullptr, nullptr, nullptr);
	DEFINE_VTABLE_PTR_HELPER_CTOR(UJSGeneratedClass);
PRAGMA_ENABLE_DEPRECATION_WARNINGS
#ifdef _MSC_VER
#pragma warning (pop)
#endif
