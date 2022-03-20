// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/GeneratedCppIncludes.h"
#include "JsEnv/Public/TypeScriptObject.h"
#ifdef _MSC_VER
#pragma warning (push)
#pragma warning (disable : 4883)
#endif
PRAGMA_DISABLE_DEPRECATION_WARNINGS
void EmptyLinkFunctionForGeneratedCodeTypeScriptObject() {}
// Cross Module References
	JSENV_API UClass* Z_Construct_UClass_UTypeScriptObject_NoRegister();
	JSENV_API UClass* Z_Construct_UClass_UTypeScriptObject();
	COREUOBJECT_API UClass* Z_Construct_UClass_UInterface();
	UPackage* Z_Construct_UPackage__Script_JsEnv();
// End Cross Module References
	void UTypeScriptObject::StaticRegisterNativesUTypeScriptObject()
	{
	}
	UClass* Z_Construct_UClass_UTypeScriptObject_NoRegister()
	{
		return UTypeScriptObject::StaticClass();
	}
	struct Z_Construct_UClass_UTypeScriptObject_Statics
	{
		static UObject* (*const DependentSingletons[])();
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Class_MetaDataParams[];
#endif
		static const FCppClassTypeInfoStatic StaticCppClassTypeInfo;
		static const UE4CodeGen_Private::FClassParams ClassParams;
	};
	UObject* (*const Z_Construct_UClass_UTypeScriptObject_Statics::DependentSingletons[])() = {
		(UObject* (*)())Z_Construct_UClass_UInterface,
		(UObject* (*)())Z_Construct_UPackage__Script_JsEnv,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UTypeScriptObject_Statics::Class_MetaDataParams[] = {
		{ "ModuleRelativePath", "Public/TypeScriptObject.h" },
	};
#endif
	const FCppClassTypeInfoStatic Z_Construct_UClass_UTypeScriptObject_Statics::StaticCppClassTypeInfo = {
		TCppClassTypeTraits<ITypeScriptObject>::IsAbstract,
	};
	const UE4CodeGen_Private::FClassParams Z_Construct_UClass_UTypeScriptObject_Statics::ClassParams = {
		&UTypeScriptObject::StaticClass,
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
		0x000840A1u,
		METADATA_PARAMS(Z_Construct_UClass_UTypeScriptObject_Statics::Class_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UClass_UTypeScriptObject_Statics::Class_MetaDataParams))
	};
	UClass* Z_Construct_UClass_UTypeScriptObject()
	{
		static UClass* OuterClass = nullptr;
		if (!OuterClass)
		{
			UE4CodeGen_Private::ConstructUClass(OuterClass, Z_Construct_UClass_UTypeScriptObject_Statics::ClassParams);
		}
		return OuterClass;
	}
	IMPLEMENT_CLASS(UTypeScriptObject, 1136829889);
	template<> JSENV_API UClass* StaticClass<UTypeScriptObject>()
	{
		return UTypeScriptObject::StaticClass();
	}
	static FCompiledInDefer Z_CompiledInDefer_UClass_UTypeScriptObject(Z_Construct_UClass_UTypeScriptObject, &UTypeScriptObject::StaticClass, TEXT("/Script/JsEnv"), TEXT("UTypeScriptObject"), false, nullptr, nullptr, nullptr);
	DEFINE_VTABLE_PTR_HELPER_CTOR(UTypeScriptObject);
PRAGMA_ENABLE_DEPRECATION_WARNINGS
#ifdef _MSC_VER
#pragma warning (pop)
#endif
