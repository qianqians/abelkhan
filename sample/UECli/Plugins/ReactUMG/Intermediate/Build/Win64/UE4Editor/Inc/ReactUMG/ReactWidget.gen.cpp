// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/GeneratedCppIncludes.h"
#include "ReactUMG/ReactWidget.h"
#ifdef _MSC_VER
#pragma warning (push)
#pragma warning (disable : 4883)
#endif
PRAGMA_DISABLE_DEPRECATION_WARNINGS
void EmptyLinkFunctionForGeneratedCodeReactWidget() {}
// Cross Module References
	REACTUMG_API UClass* Z_Construct_UClass_UReactWidget_NoRegister();
	REACTUMG_API UClass* Z_Construct_UClass_UReactWidget();
	UMG_API UClass* Z_Construct_UClass_UUserWidget();
	UPackage* Z_Construct_UPackage__Script_ReactUMG();
	UMG_API UClass* Z_Construct_UClass_UWidget_NoRegister();
	UMG_API UClass* Z_Construct_UClass_UPanelSlot_NoRegister();
// End Cross Module References
	DEFINE_FUNCTION(UReactWidget::execRemoveChild)
	{
		P_GET_OBJECT(UWidget,Z_Param_Content);
		P_FINISH;
		P_NATIVE_BEGIN;
		*(bool*)Z_Param__Result=P_THIS->RemoveChild(Z_Param_Content);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UReactWidget::execAddChild)
	{
		P_GET_OBJECT(UWidget,Z_Param_Content);
		P_FINISH;
		P_NATIVE_BEGIN;
		*(UPanelSlot**)Z_Param__Result=P_THIS->AddChild(Z_Param_Content);
		P_NATIVE_END;
	}
	void UReactWidget::StaticRegisterNativesUReactWidget()
	{
		UClass* Class = UReactWidget::StaticClass();
		static const FNameNativePtrPair Funcs[] = {
			{ "AddChild", &UReactWidget::execAddChild },
			{ "RemoveChild", &UReactWidget::execRemoveChild },
		};
		FNativeFunctionRegistrar::RegisterFunctions(Class, Funcs, UE_ARRAY_COUNT(Funcs));
	}
	struct Z_Construct_UFunction_UReactWidget_AddChild_Statics
	{
		struct ReactWidget_eventAddChild_Parms
		{
			UWidget* Content;
			UPanelSlot* ReturnValue;
		};
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_Content_MetaData[];
#endif
		static const UE4CodeGen_Private::FObjectPropertyParams NewProp_Content;
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_ReturnValue_MetaData[];
#endif
		static const UE4CodeGen_Private::FObjectPropertyParams NewProp_ReturnValue;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UReactWidget_AddChild_Statics::NewProp_Content_MetaData[] = {
		{ "EditInline", "true" },
	};
#endif
	const UE4CodeGen_Private::FObjectPropertyParams Z_Construct_UFunction_UReactWidget_AddChild_Statics::NewProp_Content = { "Content", nullptr, (EPropertyFlags)0x0010000000080080, UE4CodeGen_Private::EPropertyGenFlags::Object, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(ReactWidget_eventAddChild_Parms, Content), Z_Construct_UClass_UWidget_NoRegister, METADATA_PARAMS(Z_Construct_UFunction_UReactWidget_AddChild_Statics::NewProp_Content_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UReactWidget_AddChild_Statics::NewProp_Content_MetaData)) };
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UReactWidget_AddChild_Statics::NewProp_ReturnValue_MetaData[] = {
		{ "EditInline", "true" },
	};
#endif
	const UE4CodeGen_Private::FObjectPropertyParams Z_Construct_UFunction_UReactWidget_AddChild_Statics::NewProp_ReturnValue = { "ReturnValue", nullptr, (EPropertyFlags)0x0010000000080588, UE4CodeGen_Private::EPropertyGenFlags::Object, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(ReactWidget_eventAddChild_Parms, ReturnValue), Z_Construct_UClass_UPanelSlot_NoRegister, METADATA_PARAMS(Z_Construct_UFunction_UReactWidget_AddChild_Statics::NewProp_ReturnValue_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UReactWidget_AddChild_Statics::NewProp_ReturnValue_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UReactWidget_AddChild_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UReactWidget_AddChild_Statics::NewProp_Content,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UReactWidget_AddChild_Statics::NewProp_ReturnValue,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UReactWidget_AddChild_Statics::Function_MetaDataParams[] = {
		{ "Category", "Scripting | Javascript" },
		{ "ModuleRelativePath", "ReactWidget.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UReactWidget_AddChild_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UReactWidget, nullptr, "AddChild", nullptr, nullptr, sizeof(ReactWidget_eventAddChild_Parms), Z_Construct_UFunction_UReactWidget_AddChild_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UReactWidget_AddChild_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UReactWidget_AddChild_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UReactWidget_AddChild_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UReactWidget_AddChild()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UReactWidget_AddChild_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UReactWidget_RemoveChild_Statics
	{
		struct ReactWidget_eventRemoveChild_Parms
		{
			UWidget* Content;
			bool ReturnValue;
		};
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_Content_MetaData[];
#endif
		static const UE4CodeGen_Private::FObjectPropertyParams NewProp_Content;
		static void NewProp_ReturnValue_SetBit(void* Obj);
		static const UE4CodeGen_Private::FBoolPropertyParams NewProp_ReturnValue;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UReactWidget_RemoveChild_Statics::NewProp_Content_MetaData[] = {
		{ "EditInline", "true" },
	};
#endif
	const UE4CodeGen_Private::FObjectPropertyParams Z_Construct_UFunction_UReactWidget_RemoveChild_Statics::NewProp_Content = { "Content", nullptr, (EPropertyFlags)0x0010000000080080, UE4CodeGen_Private::EPropertyGenFlags::Object, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(ReactWidget_eventRemoveChild_Parms, Content), Z_Construct_UClass_UWidget_NoRegister, METADATA_PARAMS(Z_Construct_UFunction_UReactWidget_RemoveChild_Statics::NewProp_Content_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UReactWidget_RemoveChild_Statics::NewProp_Content_MetaData)) };
	void Z_Construct_UFunction_UReactWidget_RemoveChild_Statics::NewProp_ReturnValue_SetBit(void* Obj)
	{
		((ReactWidget_eventRemoveChild_Parms*)Obj)->ReturnValue = 1;
	}
	const UE4CodeGen_Private::FBoolPropertyParams Z_Construct_UFunction_UReactWidget_RemoveChild_Statics::NewProp_ReturnValue = { "ReturnValue", nullptr, (EPropertyFlags)0x0010000000000580, UE4CodeGen_Private::EPropertyGenFlags::Bool | UE4CodeGen_Private::EPropertyGenFlags::NativeBool, RF_Public|RF_Transient|RF_MarkAsNative, 1, sizeof(bool), sizeof(ReactWidget_eventRemoveChild_Parms), &Z_Construct_UFunction_UReactWidget_RemoveChild_Statics::NewProp_ReturnValue_SetBit, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UReactWidget_RemoveChild_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UReactWidget_RemoveChild_Statics::NewProp_Content,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UReactWidget_RemoveChild_Statics::NewProp_ReturnValue,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UReactWidget_RemoveChild_Statics::Function_MetaDataParams[] = {
		{ "Category", "Scripting | Javascript" },
		{ "ModuleRelativePath", "ReactWidget.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UReactWidget_RemoveChild_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UReactWidget, nullptr, "RemoveChild", nullptr, nullptr, sizeof(ReactWidget_eventRemoveChild_Parms), Z_Construct_UFunction_UReactWidget_RemoveChild_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UReactWidget_RemoveChild_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04020401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UReactWidget_RemoveChild_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UReactWidget_RemoveChild_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UReactWidget_RemoveChild()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UReactWidget_RemoveChild_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	UClass* Z_Construct_UClass_UReactWidget_NoRegister()
	{
		return UReactWidget::StaticClass();
	}
	struct Z_Construct_UClass_UReactWidget_Statics
	{
		static UObject* (*const DependentSingletons[])();
		static const FClassFunctionLinkInfo FuncInfo[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Class_MetaDataParams[];
#endif
		static const FCppClassTypeInfoStatic StaticCppClassTypeInfo;
		static const UE4CodeGen_Private::FClassParams ClassParams;
	};
	UObject* (*const Z_Construct_UClass_UReactWidget_Statics::DependentSingletons[])() = {
		(UObject* (*)())Z_Construct_UClass_UUserWidget,
		(UObject* (*)())Z_Construct_UPackage__Script_ReactUMG,
	};
	const FClassFunctionLinkInfo Z_Construct_UClass_UReactWidget_Statics::FuncInfo[] = {
		{ &Z_Construct_UFunction_UReactWidget_AddChild, "AddChild" }, // 4106864398
		{ &Z_Construct_UFunction_UReactWidget_RemoveChild, "RemoveChild" }, // 3114763487
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UReactWidget_Statics::Class_MetaDataParams[] = {
		{ "Comment", "/**\n *\n */" },
		{ "IncludePath", "ReactWidget.h" },
		{ "ModuleRelativePath", "ReactWidget.h" },
		{ "ObjectInitializerConstructorDeclared", "" },
	};
#endif
	const FCppClassTypeInfoStatic Z_Construct_UClass_UReactWidget_Statics::StaticCppClassTypeInfo = {
		TCppClassTypeTraits<UReactWidget>::IsAbstract,
	};
	const UE4CodeGen_Private::FClassParams Z_Construct_UClass_UReactWidget_Statics::ClassParams = {
		&UReactWidget::StaticClass,
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
		0x00B010A0u,
		METADATA_PARAMS(Z_Construct_UClass_UReactWidget_Statics::Class_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UClass_UReactWidget_Statics::Class_MetaDataParams))
	};
	UClass* Z_Construct_UClass_UReactWidget()
	{
		static UClass* OuterClass = nullptr;
		if (!OuterClass)
		{
			UE4CodeGen_Private::ConstructUClass(OuterClass, Z_Construct_UClass_UReactWidget_Statics::ClassParams);
		}
		return OuterClass;
	}
	IMPLEMENT_CLASS(UReactWidget, 16902977);
	template<> REACTUMG_API UClass* StaticClass<UReactWidget>()
	{
		return UReactWidget::StaticClass();
	}
	static FCompiledInDefer Z_CompiledInDefer_UClass_UReactWidget(Z_Construct_UClass_UReactWidget, &UReactWidget::StaticClass, TEXT("/Script/ReactUMG"), TEXT("UReactWidget"), false, nullptr, nullptr, nullptr);
	DEFINE_VTABLE_PTR_HELPER_CTOR(UReactWidget);
PRAGMA_ENABLE_DEPRECATION_WARNINGS
#ifdef _MSC_VER
#pragma warning (pop)
#endif
