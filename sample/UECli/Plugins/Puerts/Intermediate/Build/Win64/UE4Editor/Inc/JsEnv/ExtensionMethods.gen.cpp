// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/GeneratedCppIncludes.h"
#include "JsEnv/Public/ExtensionMethods.h"
#ifdef _MSC_VER
#pragma warning (push)
#pragma warning (disable : 4883)
#endif
PRAGMA_DISABLE_DEPRECATION_WARNINGS
void EmptyLinkFunctionForGeneratedCodeExtensionMethods() {}
// Cross Module References
	JSENV_API UClass* Z_Construct_UClass_UExtensionMethods_NoRegister();
	JSENV_API UClass* Z_Construct_UClass_UExtensionMethods();
	ENGINE_API UClass* Z_Construct_UClass_UBlueprintFunctionLibrary();
	UPackage* Z_Construct_UPackage__Script_JsEnv();
// End Cross Module References
	void UExtensionMethods::StaticRegisterNativesUExtensionMethods()
	{
	}
	UClass* Z_Construct_UClass_UExtensionMethods_NoRegister()
	{
		return UExtensionMethods::StaticClass();
	}
	struct Z_Construct_UClass_UExtensionMethods_Statics
	{
		static UObject* (*const DependentSingletons[])();
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Class_MetaDataParams[];
#endif
		static const FCppClassTypeInfoStatic StaticCppClassTypeInfo;
		static const UE4CodeGen_Private::FClassParams ClassParams;
	};
	UObject* (*const Z_Construct_UClass_UExtensionMethods_Statics::DependentSingletons[])() = {
		(UObject* (*)())Z_Construct_UClass_UBlueprintFunctionLibrary,
		(UObject* (*)())Z_Construct_UPackage__Script_JsEnv,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UExtensionMethods_Statics::Class_MetaDataParams[] = {
		{ "Comment", "/**\n *\n */" },
		{ "IncludePath", "ExtensionMethods.h" },
		{ "ModuleRelativePath", "Public/ExtensionMethods.h" },
	};
#endif
	const FCppClassTypeInfoStatic Z_Construct_UClass_UExtensionMethods_Statics::StaticCppClassTypeInfo = {
		TCppClassTypeTraits<UExtensionMethods>::IsAbstract,
	};
	const UE4CodeGen_Private::FClassParams Z_Construct_UClass_UExtensionMethods_Statics::ClassParams = {
		&UExtensionMethods::StaticClass,
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
		0x001000A0u,
		METADATA_PARAMS(Z_Construct_UClass_UExtensionMethods_Statics::Class_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UClass_UExtensionMethods_Statics::Class_MetaDataParams))
	};
	UClass* Z_Construct_UClass_UExtensionMethods()
	{
		static UClass* OuterClass = nullptr;
		if (!OuterClass)
		{
			UE4CodeGen_Private::ConstructUClass(OuterClass, Z_Construct_UClass_UExtensionMethods_Statics::ClassParams);
		}
		return OuterClass;
	}
	IMPLEMENT_CLASS(UExtensionMethods, 735532063);
	template<> JSENV_API UClass* StaticClass<UExtensionMethods>()
	{
		return UExtensionMethods::StaticClass();
	}
	static FCompiledInDefer Z_CompiledInDefer_UClass_UExtensionMethods(Z_Construct_UClass_UExtensionMethods, &UExtensionMethods::StaticClass, TEXT("/Script/JsEnv"), TEXT("UExtensionMethods"), false, nullptr, nullptr, nullptr);
	DEFINE_VTABLE_PTR_HELPER_CTOR(UExtensionMethods);
PRAGMA_ENABLE_DEPRECATION_WARNINGS
#ifdef _MSC_VER
#pragma warning (pop)
#endif
