// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/GeneratedCppIncludes.h"
#include "ReactDeclarationGenerator/Public/ReactDeclarationGenerator.h"
#ifdef _MSC_VER
#pragma warning (push)
#pragma warning (disable : 4883)
#endif
PRAGMA_DISABLE_DEPRECATION_WARNINGS
void EmptyLinkFunctionForGeneratedCodeReactDeclarationGenerator() {}
// Cross Module References
	REACTDECLARATIONGENERATOR_API UClass* Z_Construct_UClass_UReactDeclarationGenerator_NoRegister();
	REACTDECLARATIONGENERATOR_API UClass* Z_Construct_UClass_UReactDeclarationGenerator();
	COREUOBJECT_API UClass* Z_Construct_UClass_UObject();
	UPackage* Z_Construct_UPackage__Script_ReactDeclarationGenerator();
	DECLARATIONGENERATOR_API UClass* Z_Construct_UClass_UCodeGenerator_NoRegister();
// End Cross Module References
	DEFINE_FUNCTION(UReactDeclarationGenerator::execGen)
	{
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->Gen_Implementation();
		P_NATIVE_END;
	}
	static FName NAME_UReactDeclarationGenerator_Gen = FName(TEXT("Gen"));
	void UReactDeclarationGenerator::Gen() const
	{
		const_cast<UReactDeclarationGenerator*>(this)->ProcessEvent(FindFunctionChecked(NAME_UReactDeclarationGenerator_Gen),NULL);
	}
	void UReactDeclarationGenerator::StaticRegisterNativesUReactDeclarationGenerator()
	{
		UClass* Class = UReactDeclarationGenerator::StaticClass();
		static const FNameNativePtrPair Funcs[] = {
			{ "Gen", &UReactDeclarationGenerator::execGen },
		};
		FNativeFunctionRegistrar::RegisterFunctions(Class, Funcs, UE_ARRAY_COUNT(Funcs));
	}
	struct Z_Construct_UFunction_UReactDeclarationGenerator_Gen_Statics
	{
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UReactDeclarationGenerator_Gen_Statics::Function_MetaDataParams[] = {
		{ "ModuleRelativePath", "Public/ReactDeclarationGenerator.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UReactDeclarationGenerator_Gen_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UReactDeclarationGenerator, nullptr, "Gen", nullptr, nullptr, 0, nullptr, 0, RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x48020C00, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UReactDeclarationGenerator_Gen_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UReactDeclarationGenerator_Gen_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UReactDeclarationGenerator_Gen()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UReactDeclarationGenerator_Gen_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	UClass* Z_Construct_UClass_UReactDeclarationGenerator_NoRegister()
	{
		return UReactDeclarationGenerator::StaticClass();
	}
	struct Z_Construct_UClass_UReactDeclarationGenerator_Statics
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
	UObject* (*const Z_Construct_UClass_UReactDeclarationGenerator_Statics::DependentSingletons[])() = {
		(UObject* (*)())Z_Construct_UClass_UObject,
		(UObject* (*)())Z_Construct_UPackage__Script_ReactDeclarationGenerator,
	};
	const FClassFunctionLinkInfo Z_Construct_UClass_UReactDeclarationGenerator_Statics::FuncInfo[] = {
		{ &Z_Construct_UFunction_UReactDeclarationGenerator_Gen, "Gen" }, // 71760011
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UReactDeclarationGenerator_Statics::Class_MetaDataParams[] = {
		{ "Comment", "/**\n *\n */" },
		{ "IncludePath", "ReactDeclarationGenerator.h" },
		{ "ModuleRelativePath", "Public/ReactDeclarationGenerator.h" },
	};
#endif
		const UE4CodeGen_Private::FImplementedInterfaceParams Z_Construct_UClass_UReactDeclarationGenerator_Statics::InterfaceParams[] = {
			{ Z_Construct_UClass_UCodeGenerator_NoRegister, (int32)VTABLE_OFFSET(UReactDeclarationGenerator, ICodeGenerator), false },
		};
	const FCppClassTypeInfoStatic Z_Construct_UClass_UReactDeclarationGenerator_Statics::StaticCppClassTypeInfo = {
		TCppClassTypeTraits<UReactDeclarationGenerator>::IsAbstract,
	};
	const UE4CodeGen_Private::FClassParams Z_Construct_UClass_UReactDeclarationGenerator_Statics::ClassParams = {
		&UReactDeclarationGenerator::StaticClass,
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
		METADATA_PARAMS(Z_Construct_UClass_UReactDeclarationGenerator_Statics::Class_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UClass_UReactDeclarationGenerator_Statics::Class_MetaDataParams))
	};
	UClass* Z_Construct_UClass_UReactDeclarationGenerator()
	{
		static UClass* OuterClass = nullptr;
		if (!OuterClass)
		{
			UE4CodeGen_Private::ConstructUClass(OuterClass, Z_Construct_UClass_UReactDeclarationGenerator_Statics::ClassParams);
		}
		return OuterClass;
	}
	IMPLEMENT_CLASS(UReactDeclarationGenerator, 3981650331);
	template<> REACTDECLARATIONGENERATOR_API UClass* StaticClass<UReactDeclarationGenerator>()
	{
		return UReactDeclarationGenerator::StaticClass();
	}
	static FCompiledInDefer Z_CompiledInDefer_UClass_UReactDeclarationGenerator(Z_Construct_UClass_UReactDeclarationGenerator, &UReactDeclarationGenerator::StaticClass, TEXT("/Script/ReactDeclarationGenerator"), TEXT("UReactDeclarationGenerator"), false, nullptr, nullptr, nullptr);
	DEFINE_VTABLE_PTR_HELPER_CTOR(UReactDeclarationGenerator);
PRAGMA_ENABLE_DEPRECATION_WARNINGS
#ifdef _MSC_VER
#pragma warning (pop)
#endif
