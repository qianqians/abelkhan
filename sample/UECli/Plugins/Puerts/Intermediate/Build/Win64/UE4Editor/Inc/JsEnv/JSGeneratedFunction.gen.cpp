// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/GeneratedCppIncludes.h"
#include "JsEnv/Private/JSGeneratedFunction.h"
#ifdef _MSC_VER
#pragma warning (push)
#pragma warning (disable : 4883)
#endif
PRAGMA_DISABLE_DEPRECATION_WARNINGS
void EmptyLinkFunctionForGeneratedCodeJSGeneratedFunction() {}
// Cross Module References
	JSENV_API UClass* Z_Construct_UClass_UJSGeneratedFunction_NoRegister();
	JSENV_API UClass* Z_Construct_UClass_UJSGeneratedFunction();
	COREUOBJECT_API UClass* Z_Construct_UClass_UFunction();
	UPackage* Z_Construct_UPackage__Script_JsEnv();
// End Cross Module References
	void UJSGeneratedFunction::StaticRegisterNativesUJSGeneratedFunction()
	{
	}
	UClass* Z_Construct_UClass_UJSGeneratedFunction_NoRegister()
	{
		return UJSGeneratedFunction::StaticClass();
	}
	struct Z_Construct_UClass_UJSGeneratedFunction_Statics
	{
		static UObject* (*const DependentSingletons[])();
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Class_MetaDataParams[];
#endif
		static const FCppClassTypeInfoStatic StaticCppClassTypeInfo;
		static const UE4CodeGen_Private::FClassParams ClassParams;
	};
	UObject* (*const Z_Construct_UClass_UJSGeneratedFunction_Statics::DependentSingletons[])() = {
		(UObject* (*)())Z_Construct_UClass_UFunction,
		(UObject* (*)())Z_Construct_UPackage__Script_JsEnv,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UJSGeneratedFunction_Statics::Class_MetaDataParams[] = {
		{ "Comment", "/**\n *\n */" },
		{ "IncludePath", "JSGeneratedFunction.h" },
		{ "ModuleRelativePath", "Private/JSGeneratedFunction.h" },
	};
#endif
	const FCppClassTypeInfoStatic Z_Construct_UClass_UJSGeneratedFunction_Statics::StaticCppClassTypeInfo = {
		TCppClassTypeTraits<UJSGeneratedFunction>::IsAbstract,
	};
	const UE4CodeGen_Private::FClassParams Z_Construct_UClass_UJSGeneratedFunction_Statics::ClassParams = {
		&UJSGeneratedFunction::StaticClass,
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
		0x000000A0u,
		METADATA_PARAMS(Z_Construct_UClass_UJSGeneratedFunction_Statics::Class_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UClass_UJSGeneratedFunction_Statics::Class_MetaDataParams))
	};
	UClass* Z_Construct_UClass_UJSGeneratedFunction()
	{
		static UClass* OuterClass = nullptr;
		if (!OuterClass)
		{
			UE4CodeGen_Private::ConstructUClass(OuterClass, Z_Construct_UClass_UJSGeneratedFunction_Statics::ClassParams);
		}
		return OuterClass;
	}
	IMPLEMENT_CLASS(UJSGeneratedFunction, 2430714080);
	template<> JSENV_API UClass* StaticClass<UJSGeneratedFunction>()
	{
		return UJSGeneratedFunction::StaticClass();
	}
	static FCompiledInDefer Z_CompiledInDefer_UClass_UJSGeneratedFunction(Z_Construct_UClass_UJSGeneratedFunction, &UJSGeneratedFunction::StaticClass, TEXT("/Script/JsEnv"), TEXT("UJSGeneratedFunction"), false, nullptr, nullptr, nullptr);
	DEFINE_VTABLE_PTR_HELPER_CTOR(UJSGeneratedFunction);
PRAGMA_ENABLE_DEPRECATION_WARNINGS
#ifdef _MSC_VER
#pragma warning (pop)
#endif
