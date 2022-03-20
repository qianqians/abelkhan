// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/GeneratedCppIncludes.h"
#include "JsEnv/Private/DynamicDelegateProxy.h"
#ifdef _MSC_VER
#pragma warning (push)
#pragma warning (disable : 4883)
#endif
PRAGMA_DISABLE_DEPRECATION_WARNINGS
void EmptyLinkFunctionForGeneratedCodeDynamicDelegateProxy() {}
// Cross Module References
	JSENV_API UClass* Z_Construct_UClass_UDynamicDelegateProxy_NoRegister();
	JSENV_API UClass* Z_Construct_UClass_UDynamicDelegateProxy();
	COREUOBJECT_API UClass* Z_Construct_UClass_UObject();
	UPackage* Z_Construct_UPackage__Script_JsEnv();
// End Cross Module References
	DEFINE_FUNCTION(UDynamicDelegateProxy::execFire)
	{
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->Fire();
		P_NATIVE_END;
	}
	void UDynamicDelegateProxy::StaticRegisterNativesUDynamicDelegateProxy()
	{
		UClass* Class = UDynamicDelegateProxy::StaticClass();
		static const FNameNativePtrPair Funcs[] = {
			{ "Fire", &UDynamicDelegateProxy::execFire },
		};
		FNativeFunctionRegistrar::RegisterFunctions(Class, Funcs, UE_ARRAY_COUNT(Funcs));
	}
	struct Z_Construct_UFunction_UDynamicDelegateProxy_Fire_Statics
	{
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UDynamicDelegateProxy_Fire_Statics::Function_MetaDataParams[] = {
		{ "Category", "TGameJS" },
		{ "ModuleRelativePath", "Private/DynamicDelegateProxy.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UDynamicDelegateProxy_Fire_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UDynamicDelegateProxy, nullptr, "Fire", nullptr, nullptr, 0, nullptr, 0, RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UDynamicDelegateProxy_Fire_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UDynamicDelegateProxy_Fire_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UDynamicDelegateProxy_Fire()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UDynamicDelegateProxy_Fire_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	UClass* Z_Construct_UClass_UDynamicDelegateProxy_NoRegister()
	{
		return UDynamicDelegateProxy::StaticClass();
	}
	struct Z_Construct_UClass_UDynamicDelegateProxy_Statics
	{
		static UObject* (*const DependentSingletons[])();
		static const FClassFunctionLinkInfo FuncInfo[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Class_MetaDataParams[];
#endif
		static const FCppClassTypeInfoStatic StaticCppClassTypeInfo;
		static const UE4CodeGen_Private::FClassParams ClassParams;
	};
	UObject* (*const Z_Construct_UClass_UDynamicDelegateProxy_Statics::DependentSingletons[])() = {
		(UObject* (*)())Z_Construct_UClass_UObject,
		(UObject* (*)())Z_Construct_UPackage__Script_JsEnv,
	};
	const FClassFunctionLinkInfo Z_Construct_UClass_UDynamicDelegateProxy_Statics::FuncInfo[] = {
		{ &Z_Construct_UFunction_UDynamicDelegateProxy_Fire, "Fire" }, // 1976320315
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UDynamicDelegateProxy_Statics::Class_MetaDataParams[] = {
		{ "Comment", "/**\n *\n */" },
		{ "IncludePath", "DynamicDelegateProxy.h" },
		{ "ModuleRelativePath", "Private/DynamicDelegateProxy.h" },
	};
#endif
	const FCppClassTypeInfoStatic Z_Construct_UClass_UDynamicDelegateProxy_Statics::StaticCppClassTypeInfo = {
		TCppClassTypeTraits<UDynamicDelegateProxy>::IsAbstract,
	};
	const UE4CodeGen_Private::FClassParams Z_Construct_UClass_UDynamicDelegateProxy_Statics::ClassParams = {
		&UDynamicDelegateProxy::StaticClass,
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
		0x000000A0u,
		METADATA_PARAMS(Z_Construct_UClass_UDynamicDelegateProxy_Statics::Class_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UClass_UDynamicDelegateProxy_Statics::Class_MetaDataParams))
	};
	UClass* Z_Construct_UClass_UDynamicDelegateProxy()
	{
		static UClass* OuterClass = nullptr;
		if (!OuterClass)
		{
			UE4CodeGen_Private::ConstructUClass(OuterClass, Z_Construct_UClass_UDynamicDelegateProxy_Statics::ClassParams);
		}
		return OuterClass;
	}
	IMPLEMENT_CLASS(UDynamicDelegateProxy, 4115464972);
	template<> JSENV_API UClass* StaticClass<UDynamicDelegateProxy>()
	{
		return UDynamicDelegateProxy::StaticClass();
	}
	static FCompiledInDefer Z_CompiledInDefer_UClass_UDynamicDelegateProxy(Z_Construct_UClass_UDynamicDelegateProxy, &UDynamicDelegateProxy::StaticClass, TEXT("/Script/JsEnv"), TEXT("UDynamicDelegateProxy"), false, nullptr, nullptr, nullptr);
	DEFINE_VTABLE_PTR_HELPER_CTOR(UDynamicDelegateProxy);
PRAGMA_ENABLE_DEPRECATION_WARNINGS
#ifdef _MSC_VER
#pragma warning (pop)
#endif
