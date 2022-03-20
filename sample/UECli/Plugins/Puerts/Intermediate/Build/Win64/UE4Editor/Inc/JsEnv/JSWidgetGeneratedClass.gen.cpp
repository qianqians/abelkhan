// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/GeneratedCppIncludes.h"
#include "JsEnv/Private/JSWidgetGeneratedClass.h"
#ifdef _MSC_VER
#pragma warning (push)
#pragma warning (disable : 4883)
#endif
PRAGMA_DISABLE_DEPRECATION_WARNINGS
void EmptyLinkFunctionForGeneratedCodeJSWidgetGeneratedClass() {}
// Cross Module References
	JSENV_API UClass* Z_Construct_UClass_UJSWidgetGeneratedClass_NoRegister();
	JSENV_API UClass* Z_Construct_UClass_UJSWidgetGeneratedClass();
	UMG_API UClass* Z_Construct_UClass_UWidgetBlueprintGeneratedClass();
	UPackage* Z_Construct_UPackage__Script_JsEnv();
// End Cross Module References
	void UJSWidgetGeneratedClass::StaticRegisterNativesUJSWidgetGeneratedClass()
	{
	}
	UClass* Z_Construct_UClass_UJSWidgetGeneratedClass_NoRegister()
	{
		return UJSWidgetGeneratedClass::StaticClass();
	}
	struct Z_Construct_UClass_UJSWidgetGeneratedClass_Statics
	{
		static UObject* (*const DependentSingletons[])();
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Class_MetaDataParams[];
#endif
		static const FCppClassTypeInfoStatic StaticCppClassTypeInfo;
		static const UE4CodeGen_Private::FClassParams ClassParams;
	};
	UObject* (*const Z_Construct_UClass_UJSWidgetGeneratedClass_Statics::DependentSingletons[])() = {
		(UObject* (*)())Z_Construct_UClass_UWidgetBlueprintGeneratedClass,
		(UObject* (*)())Z_Construct_UPackage__Script_JsEnv,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UJSWidgetGeneratedClass_Statics::Class_MetaDataParams[] = {
		{ "Comment", "/**\n *\n */" },
		{ "IncludePath", "JSWidgetGeneratedClass.h" },
		{ "ModuleRelativePath", "Private/JSWidgetGeneratedClass.h" },
	};
#endif
	const FCppClassTypeInfoStatic Z_Construct_UClass_UJSWidgetGeneratedClass_Statics::StaticCppClassTypeInfo = {
		TCppClassTypeTraits<UJSWidgetGeneratedClass>::IsAbstract,
	};
	const UE4CodeGen_Private::FClassParams Z_Construct_UClass_UJSWidgetGeneratedClass_Statics::ClassParams = {
		&UJSWidgetGeneratedClass::StaticClass,
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
		METADATA_PARAMS(Z_Construct_UClass_UJSWidgetGeneratedClass_Statics::Class_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UClass_UJSWidgetGeneratedClass_Statics::Class_MetaDataParams))
	};
	UClass* Z_Construct_UClass_UJSWidgetGeneratedClass()
	{
		static UClass* OuterClass = nullptr;
		if (!OuterClass)
		{
			UE4CodeGen_Private::ConstructUClass(OuterClass, Z_Construct_UClass_UJSWidgetGeneratedClass_Statics::ClassParams);
		}
		return OuterClass;
	}
	IMPLEMENT_CLASS(UJSWidgetGeneratedClass, 2555728979);
	template<> JSENV_API UClass* StaticClass<UJSWidgetGeneratedClass>()
	{
		return UJSWidgetGeneratedClass::StaticClass();
	}
	static FCompiledInDefer Z_CompiledInDefer_UClass_UJSWidgetGeneratedClass(Z_Construct_UClass_UJSWidgetGeneratedClass, &UJSWidgetGeneratedClass::StaticClass, TEXT("/Script/JsEnv"), TEXT("UJSWidgetGeneratedClass"), false, nullptr, nullptr, nullptr);
	DEFINE_VTABLE_PTR_HELPER_CTOR(UJSWidgetGeneratedClass);
PRAGMA_ENABLE_DEPRECATION_WARNINGS
#ifdef _MSC_VER
#pragma warning (pop)
#endif
