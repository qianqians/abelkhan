// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/GeneratedCppIncludes.h"
#include "DeclarationGenerator/Public/CodeGenerator.h"
#ifdef _MSC_VER
#pragma warning (push)
#pragma warning (disable : 4883)
#endif
PRAGMA_DISABLE_DEPRECATION_WARNINGS
void EmptyLinkFunctionForGeneratedCodeCodeGenerator() {}
// Cross Module References
	DECLARATIONGENERATOR_API UClass* Z_Construct_UClass_UCodeGenerator_NoRegister();
	DECLARATIONGENERATOR_API UClass* Z_Construct_UClass_UCodeGenerator();
	COREUOBJECT_API UClass* Z_Construct_UClass_UInterface();
	UPackage* Z_Construct_UPackage__Script_DeclarationGenerator();
// End Cross Module References
	DEFINE_FUNCTION(ICodeGenerator::execGen)
	{
		P_FINISH;
		P_NATIVE_BEGIN;
		P_THIS->Gen_Implementation();
		P_NATIVE_END;
	}
	void ICodeGenerator::Gen() const
	{
		check(0 && "Do not directly call Event functions in Interfaces. Call Execute_Gen instead.");
	}
	void UCodeGenerator::StaticRegisterNativesUCodeGenerator()
	{
		UClass* Class = UCodeGenerator::StaticClass();
		static const FNameNativePtrPair Funcs[] = {
			{ "Gen", &ICodeGenerator::execGen },
		};
		FNativeFunctionRegistrar::RegisterFunctions(Class, Funcs, UE_ARRAY_COUNT(Funcs));
	}
	struct Z_Construct_UFunction_UCodeGenerator_Gen_Statics
	{
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UCodeGenerator_Gen_Statics::Function_MetaDataParams[] = {
		{ "ModuleRelativePath", "Public/CodeGenerator.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UCodeGenerator_Gen_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UCodeGenerator, nullptr, "Gen", nullptr, nullptr, 0, nullptr, 0, RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x48020C00, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UCodeGenerator_Gen_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UCodeGenerator_Gen_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UCodeGenerator_Gen()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UCodeGenerator_Gen_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	UClass* Z_Construct_UClass_UCodeGenerator_NoRegister()
	{
		return UCodeGenerator::StaticClass();
	}
	struct Z_Construct_UClass_UCodeGenerator_Statics
	{
		static UObject* (*const DependentSingletons[])();
		static const FClassFunctionLinkInfo FuncInfo[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Class_MetaDataParams[];
#endif
		static const FCppClassTypeInfoStatic StaticCppClassTypeInfo;
		static const UE4CodeGen_Private::FClassParams ClassParams;
	};
	UObject* (*const Z_Construct_UClass_UCodeGenerator_Statics::DependentSingletons[])() = {
		(UObject* (*)())Z_Construct_UClass_UInterface,
		(UObject* (*)())Z_Construct_UPackage__Script_DeclarationGenerator,
	};
	const FClassFunctionLinkInfo Z_Construct_UClass_UCodeGenerator_Statics::FuncInfo[] = {
		{ &Z_Construct_UFunction_UCodeGenerator_Gen, "Gen" }, // 4094526227
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UCodeGenerator_Statics::Class_MetaDataParams[] = {
		{ "ModuleRelativePath", "Public/CodeGenerator.h" },
	};
#endif
	const FCppClassTypeInfoStatic Z_Construct_UClass_UCodeGenerator_Statics::StaticCppClassTypeInfo = {
		TCppClassTypeTraits<ICodeGenerator>::IsAbstract,
	};
	const UE4CodeGen_Private::FClassParams Z_Construct_UClass_UCodeGenerator_Statics::ClassParams = {
		&UCodeGenerator::StaticClass,
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
		0x000840A1u,
		METADATA_PARAMS(Z_Construct_UClass_UCodeGenerator_Statics::Class_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UClass_UCodeGenerator_Statics::Class_MetaDataParams))
	};
	UClass* Z_Construct_UClass_UCodeGenerator()
	{
		static UClass* OuterClass = nullptr;
		if (!OuterClass)
		{
			UE4CodeGen_Private::ConstructUClass(OuterClass, Z_Construct_UClass_UCodeGenerator_Statics::ClassParams);
		}
		return OuterClass;
	}
	IMPLEMENT_CLASS(UCodeGenerator, 2666912600);
	template<> DECLARATIONGENERATOR_API UClass* StaticClass<UCodeGenerator>()
	{
		return UCodeGenerator::StaticClass();
	}
	static FCompiledInDefer Z_CompiledInDefer_UClass_UCodeGenerator(Z_Construct_UClass_UCodeGenerator, &UCodeGenerator::StaticClass, TEXT("/Script/DeclarationGenerator"), TEXT("UCodeGenerator"), false, nullptr, nullptr, nullptr);
	DEFINE_VTABLE_PTR_HELPER_CTOR(UCodeGenerator);
	static FName NAME_UCodeGenerator_Gen = FName(TEXT("Gen"));
	void ICodeGenerator::Execute_Gen(const UObject* O)
	{
		check(O != NULL);
		check(O->GetClass()->ImplementsInterface(UCodeGenerator::StaticClass()));
		UFunction* const Func = O->FindFunction(NAME_UCodeGenerator_Gen);
		if (Func)
		{
			const_cast<UObject*>(O)->ProcessEvent(Func, NULL);
		}
		else if (auto I = (const ICodeGenerator*)(O->GetNativeInterfaceAddress(UCodeGenerator::StaticClass())))
		{
			I->Gen_Implementation();
		}
	}
PRAGMA_ENABLE_DEPRECATION_WARNINGS
#ifdef _MSC_VER
#pragma warning (pop)
#endif
