// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/GeneratedCppIncludes.h"
#include "DeclarationGenerator/Public/TemplateBindingGenerator.h"
#ifdef _MSC_VER
#pragma warning (push)
#pragma warning (disable : 4883)
#endif
PRAGMA_DISABLE_DEPRECATION_WARNINGS
void EmptyLinkFunctionForGeneratedCodeTemplateBindingGenerator() {}
// Cross Module References
	DECLARATIONGENERATOR_API UClass* Z_Construct_UClass_UTemplateBindingGenerator_NoRegister();
	DECLARATIONGENERATOR_API UClass* Z_Construct_UClass_UTemplateBindingGenerator();
	COREUOBJECT_API UClass* Z_Construct_UClass_UObject();
	UPackage* Z_Construct_UPackage__Script_DeclarationGenerator();
	DECLARATIONGENERATOR_API UClass* Z_Construct_UClass_UCodeGenerator_NoRegister();
// End Cross Module References
	DEFINE_FUNCTION(UTemplateBindingGenerator::execGen)
	{
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->Gen_Implementation();
		P_NATIVE_END;
	}
	static FName NAME_UTemplateBindingGenerator_Gen = FName(TEXT("Gen"));
	void UTemplateBindingGenerator::Gen() const
	{
		const_cast<UTemplateBindingGenerator*>(this)->ProcessEvent(FindFunctionChecked(NAME_UTemplateBindingGenerator_Gen),NULL);
	}
	void UTemplateBindingGenerator::StaticRegisterNativesUTemplateBindingGenerator()
	{
		UClass* Class = UTemplateBindingGenerator::StaticClass();
		static const FNameNativePtrPair Funcs[] = {
			{ "Gen", &UTemplateBindingGenerator::execGen },
		};
		FNativeFunctionRegistrar::RegisterFunctions(Class, Funcs, UE_ARRAY_COUNT(Funcs));
	}
	struct Z_Construct_UFunction_UTemplateBindingGenerator_Gen_Statics
	{
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UTemplateBindingGenerator_Gen_Statics::Function_MetaDataParams[] = {
		{ "ModuleRelativePath", "Public/TemplateBindingGenerator.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UTemplateBindingGenerator_Gen_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UTemplateBindingGenerator, nullptr, "Gen", nullptr, nullptr, 0, nullptr, 0, RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x48020C00, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UTemplateBindingGenerator_Gen_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UTemplateBindingGenerator_Gen_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UTemplateBindingGenerator_Gen()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UTemplateBindingGenerator_Gen_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	UClass* Z_Construct_UClass_UTemplateBindingGenerator_NoRegister()
	{
		return UTemplateBindingGenerator::StaticClass();
	}
	struct Z_Construct_UClass_UTemplateBindingGenerator_Statics
	{
		static UObject* (*const DependentSingletons[])();
		static const FClassFunctionLinkInfo FuncInfo[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Class_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FImplementedInterfaceParams InterfaceParams[];
		static const FCppClassTypeInfoStatic StaticCppClassTypeInfo;
		static const UE4CodeGen_Private::FClassParams ClassParams;
	};
	UObject* (*const Z_Construct_UClass_UTemplateBindingGenerator_Statics::DependentSingletons[])() = {
		(UObject* (*)())Z_Construct_UClass_UObject,
		(UObject* (*)())Z_Construct_UPackage__Script_DeclarationGenerator,
	};
	const FClassFunctionLinkInfo Z_Construct_UClass_UTemplateBindingGenerator_Statics::FuncInfo[] = {
		{ &Z_Construct_UFunction_UTemplateBindingGenerator_Gen, "Gen" }, // 2828949435
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UTemplateBindingGenerator_Statics::Class_MetaDataParams[] = {
		{ "Comment", "/**\n *\n */" },
		{ "IncludePath", "TemplateBindingGenerator.h" },
		{ "ModuleRelativePath", "Public/TemplateBindingGenerator.h" },
	};
#endif
		const UE4CodeGen_Private::FImplementedInterfaceParams Z_Construct_UClass_UTemplateBindingGenerator_Statics::InterfaceParams[] = {
			{ Z_Construct_UClass_UCodeGenerator_NoRegister, (int32)VTABLE_OFFSET(UTemplateBindingGenerator, ICodeGenerator), false },
		};
	const FCppClassTypeInfoStatic Z_Construct_UClass_UTemplateBindingGenerator_Statics::StaticCppClassTypeInfo = {
		TCppClassTypeTraits<UTemplateBindingGenerator>::IsAbstract,
	};
	const UE4CodeGen_Private::FClassParams Z_Construct_UClass_UTemplateBindingGenerator_Statics::ClassParams = {
		&UTemplateBindingGenerator::StaticClass,
		nullptr,
		&StaticCppClassTypeInfo,
		DependentSingletons,
		FuncInfo,
		nullptr,
		InterfaceParams,
		UE_ARRAY_COUNT(DependentSingletons),
		UE_ARRAY_COUNT(FuncInfo),
		0,
		UE_ARRAY_COUNT(InterfaceParams),
		0x001000A0u,
		METADATA_PARAMS(Z_Construct_UClass_UTemplateBindingGenerator_Statics::Class_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UClass_UTemplateBindingGenerator_Statics::Class_MetaDataParams))
	};
	UClass* Z_Construct_UClass_UTemplateBindingGenerator()
	{
		static UClass* OuterClass = nullptr;
		if (!OuterClass)
		{
			UE4CodeGen_Private::ConstructUClass(OuterClass, Z_Construct_UClass_UTemplateBindingGenerator_Statics::ClassParams);
		}
		return OuterClass;
	}
	IMPLEMENT_CLASS(UTemplateBindingGenerator, 3074477642);
	template<> DECLARATIONGENERATOR_API UClass* StaticClass<UTemplateBindingGenerator>()
	{
		return UTemplateBindingGenerator::StaticClass();
	}
	static FCompiledInDefer Z_CompiledInDefer_UClass_UTemplateBindingGenerator(Z_Construct_UClass_UTemplateBindingGenerator, &UTemplateBindingGenerator::StaticClass, TEXT("/Script/DeclarationGenerator"), TEXT("UTemplateBindingGenerator"), false, nullptr, nullptr, nullptr);
	DEFINE_VTABLE_PTR_HELPER_CTOR(UTemplateBindingGenerator);
PRAGMA_ENABLE_DEPRECATION_WARNINGS
#ifdef _MSC_VER
#pragma warning (pop)
#endif
