// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/GeneratedCppIncludes.h"
#include "ReactUMG/UMGManager.h"
#ifdef _MSC_VER
#pragma warning (push)
#pragma warning (disable : 4883)
#endif
PRAGMA_DISABLE_DEPRECATION_WARNINGS
void EmptyLinkFunctionForGeneratedCodeUMGManager() {}
// Cross Module References
	REACTUMG_API UClass* Z_Construct_UClass_UUMGManager_NoRegister();
	REACTUMG_API UClass* Z_Construct_UClass_UUMGManager();
	ENGINE_API UClass* Z_Construct_UClass_UBlueprintFunctionLibrary();
	UPackage* Z_Construct_UPackage__Script_ReactUMG();
	ENGINE_API UClass* Z_Construct_UClass_UWorld_NoRegister();
	REACTUMG_API UClass* Z_Construct_UClass_UReactWidget_NoRegister();
	COREUOBJECT_API UClass* Z_Construct_UClass_UClass();
	COREUOBJECT_API UClass* Z_Construct_UClass_UObject_NoRegister();
	UMG_API UClass* Z_Construct_UClass_UUserWidget_NoRegister();
	UMG_API UClass* Z_Construct_UClass_UPanelSlot_NoRegister();
	UMG_API UClass* Z_Construct_UClass_UWidget_NoRegister();
// End Cross Module References
	DEFINE_FUNCTION(UUMGManager::execSynchronizeSlotProperties)
	{
		P_GET_OBJECT(UPanelSlot,Z_Param_Slot);
		P_FINISH;
		P_NATIVE_BEGIN;
		UUMGManager::SynchronizeSlotProperties(Z_Param_Slot);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UUMGManager::execSynchronizeWidgetProperties)
	{
		P_GET_OBJECT(UWidget,Z_Param_Widget);
		P_FINISH;
		P_NATIVE_BEGIN;
		UUMGManager::SynchronizeWidgetProperties(Z_Param_Widget);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UUMGManager::execCreateWidget)
	{
		P_GET_OBJECT(UWorld,Z_Param_World);
		P_GET_OBJECT(UClass,Z_Param_Class);
		P_FINISH;
		P_NATIVE_BEGIN;
		*(UUserWidget**)Z_Param__Result=UUMGManager::CreateWidget(Z_Param_World,Z_Param_Class);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UUMGManager::execCreateReactWidget)
	{
		P_GET_OBJECT(UWorld,Z_Param_World);
		P_FINISH;
		P_NATIVE_BEGIN;
		*(UReactWidget**)Z_Param__Result=UUMGManager::CreateReactWidget(Z_Param_World);
		P_NATIVE_END;
	}
	void UUMGManager::StaticRegisterNativesUUMGManager()
	{
		UClass* Class = UUMGManager::StaticClass();
		static const FNameNativePtrPair Funcs[] = {
			{ "CreateReactWidget", &UUMGManager::execCreateReactWidget },
			{ "CreateWidget", &UUMGManager::execCreateWidget },
			{ "SynchronizeSlotProperties", &UUMGManager::execSynchronizeSlotProperties },
			{ "SynchronizeWidgetProperties", &UUMGManager::execSynchronizeWidgetProperties },
		};
		FNativeFunctionRegistrar::RegisterFunctions(Class, Funcs, UE_ARRAY_COUNT(Funcs));
	}
	struct Z_Construct_UFunction_UUMGManager_CreateReactWidget_Statics
	{
		struct UMGManager_eventCreateReactWidget_Parms
		{
			UWorld* World;
			UReactWidget* ReturnValue;
		};
		static const UE4CodeGen_Private::FObjectPropertyParams NewProp_World;
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
	const UE4CodeGen_Private::FObjectPropertyParams Z_Construct_UFunction_UUMGManager_CreateReactWidget_Statics::NewProp_World = { "World", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Object, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(UMGManager_eventCreateReactWidget_Parms, World), Z_Construct_UClass_UWorld_NoRegister, METADATA_PARAMS(nullptr, 0) };
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UUMGManager_CreateReactWidget_Statics::NewProp_ReturnValue_MetaData[] = {
		{ "EditInline", "true" },
	};
#endif
	const UE4CodeGen_Private::FObjectPropertyParams Z_Construct_UFunction_UUMGManager_CreateReactWidget_Statics::NewProp_ReturnValue = { "ReturnValue", nullptr, (EPropertyFlags)0x0010000000080588, UE4CodeGen_Private::EPropertyGenFlags::Object, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(UMGManager_eventCreateReactWidget_Parms, ReturnValue), Z_Construct_UClass_UReactWidget_NoRegister, METADATA_PARAMS(Z_Construct_UFunction_UUMGManager_CreateReactWidget_Statics::NewProp_ReturnValue_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UUMGManager_CreateReactWidget_Statics::NewProp_ReturnValue_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UUMGManager_CreateReactWidget_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UUMGManager_CreateReactWidget_Statics::NewProp_World,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UUMGManager_CreateReactWidget_Statics::NewProp_ReturnValue,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UUMGManager_CreateReactWidget_Statics::Function_MetaDataParams[] = {
		{ "Category", "Widget" },
		{ "ModuleRelativePath", "UMGManager.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UUMGManager_CreateReactWidget_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UUMGManager, nullptr, "CreateReactWidget", nullptr, nullptr, sizeof(UMGManager_eventCreateReactWidget_Parms), Z_Construct_UFunction_UUMGManager_CreateReactWidget_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UUMGManager_CreateReactWidget_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04022409, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UUMGManager_CreateReactWidget_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UUMGManager_CreateReactWidget_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UUMGManager_CreateReactWidget()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UUMGManager_CreateReactWidget_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UUMGManager_CreateWidget_Statics
	{
		struct UMGManager_eventCreateWidget_Parms
		{
			UWorld* World;
			UClass* Class;
			UUserWidget* ReturnValue;
		};
		static const UE4CodeGen_Private::FObjectPropertyParams NewProp_World;
		static const UE4CodeGen_Private::FClassPropertyParams NewProp_Class;
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
	const UE4CodeGen_Private::FObjectPropertyParams Z_Construct_UFunction_UUMGManager_CreateWidget_Statics::NewProp_World = { "World", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Object, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(UMGManager_eventCreateWidget_Parms, World), Z_Construct_UClass_UWorld_NoRegister, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FClassPropertyParams Z_Construct_UFunction_UUMGManager_CreateWidget_Statics::NewProp_Class = { "Class", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Class, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(UMGManager_eventCreateWidget_Parms, Class), Z_Construct_UClass_UObject_NoRegister, Z_Construct_UClass_UClass, METADATA_PARAMS(nullptr, 0) };
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UUMGManager_CreateWidget_Statics::NewProp_ReturnValue_MetaData[] = {
		{ "EditInline", "true" },
	};
#endif
	const UE4CodeGen_Private::FObjectPropertyParams Z_Construct_UFunction_UUMGManager_CreateWidget_Statics::NewProp_ReturnValue = { "ReturnValue", nullptr, (EPropertyFlags)0x0010000000080588, UE4CodeGen_Private::EPropertyGenFlags::Object, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(UMGManager_eventCreateWidget_Parms, ReturnValue), Z_Construct_UClass_UUserWidget_NoRegister, METADATA_PARAMS(Z_Construct_UFunction_UUMGManager_CreateWidget_Statics::NewProp_ReturnValue_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UUMGManager_CreateWidget_Statics::NewProp_ReturnValue_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UUMGManager_CreateWidget_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UUMGManager_CreateWidget_Statics::NewProp_World,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UUMGManager_CreateWidget_Statics::NewProp_Class,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UUMGManager_CreateWidget_Statics::NewProp_ReturnValue,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UUMGManager_CreateWidget_Statics::Function_MetaDataParams[] = {
		{ "Category", "Widget" },
		{ "ModuleRelativePath", "UMGManager.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UUMGManager_CreateWidget_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UUMGManager, nullptr, "CreateWidget", nullptr, nullptr, sizeof(UMGManager_eventCreateWidget_Parms), Z_Construct_UFunction_UUMGManager_CreateWidget_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UUMGManager_CreateWidget_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04022409, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UUMGManager_CreateWidget_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UUMGManager_CreateWidget_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UUMGManager_CreateWidget()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UUMGManager_CreateWidget_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UUMGManager_SynchronizeSlotProperties_Statics
	{
		struct UMGManager_eventSynchronizeSlotProperties_Parms
		{
			UPanelSlot* Slot;
		};
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_Slot_MetaData[];
#endif
		static const UE4CodeGen_Private::FObjectPropertyParams NewProp_Slot;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UUMGManager_SynchronizeSlotProperties_Statics::NewProp_Slot_MetaData[] = {
		{ "EditInline", "true" },
	};
#endif
	const UE4CodeGen_Private::FObjectPropertyParams Z_Construct_UFunction_UUMGManager_SynchronizeSlotProperties_Statics::NewProp_Slot = { "Slot", nullptr, (EPropertyFlags)0x0010000000080080, UE4CodeGen_Private::EPropertyGenFlags::Object, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(UMGManager_eventSynchronizeSlotProperties_Parms, Slot), Z_Construct_UClass_UPanelSlot_NoRegister, METADATA_PARAMS(Z_Construct_UFunction_UUMGManager_SynchronizeSlotProperties_Statics::NewProp_Slot_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UUMGManager_SynchronizeSlotProperties_Statics::NewProp_Slot_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UUMGManager_SynchronizeSlotProperties_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UUMGManager_SynchronizeSlotProperties_Statics::NewProp_Slot,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UUMGManager_SynchronizeSlotProperties_Statics::Function_MetaDataParams[] = {
		{ "Category", "Widget" },
		{ "ModuleRelativePath", "UMGManager.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UUMGManager_SynchronizeSlotProperties_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UUMGManager, nullptr, "SynchronizeSlotProperties", nullptr, nullptr, sizeof(UMGManager_eventSynchronizeSlotProperties_Parms), Z_Construct_UFunction_UUMGManager_SynchronizeSlotProperties_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UUMGManager_SynchronizeSlotProperties_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04022409, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UUMGManager_SynchronizeSlotProperties_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UUMGManager_SynchronizeSlotProperties_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UUMGManager_SynchronizeSlotProperties()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UUMGManager_SynchronizeSlotProperties_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UUMGManager_SynchronizeWidgetProperties_Statics
	{
		struct UMGManager_eventSynchronizeWidgetProperties_Parms
		{
			UWidget* Widget;
		};
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam NewProp_Widget_MetaData[];
#endif
		static const UE4CodeGen_Private::FObjectPropertyParams NewProp_Widget;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UUMGManager_SynchronizeWidgetProperties_Statics::NewProp_Widget_MetaData[] = {
		{ "EditInline", "true" },
	};
#endif
	const UE4CodeGen_Private::FObjectPropertyParams Z_Construct_UFunction_UUMGManager_SynchronizeWidgetProperties_Statics::NewProp_Widget = { "Widget", nullptr, (EPropertyFlags)0x0010000000080080, UE4CodeGen_Private::EPropertyGenFlags::Object, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(UMGManager_eventSynchronizeWidgetProperties_Parms, Widget), Z_Construct_UClass_UWidget_NoRegister, METADATA_PARAMS(Z_Construct_UFunction_UUMGManager_SynchronizeWidgetProperties_Statics::NewProp_Widget_MetaData, UE_ARRAY_COUNT(Z_Construct_UFunction_UUMGManager_SynchronizeWidgetProperties_Statics::NewProp_Widget_MetaData)) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UUMGManager_SynchronizeWidgetProperties_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UUMGManager_SynchronizeWidgetProperties_Statics::NewProp_Widget,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UUMGManager_SynchronizeWidgetProperties_Statics::Function_MetaDataParams[] = {
		{ "Category", "Widget" },
		{ "ModuleRelativePath", "UMGManager.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UUMGManager_SynchronizeWidgetProperties_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UUMGManager, nullptr, "SynchronizeWidgetProperties", nullptr, nullptr, sizeof(UMGManager_eventSynchronizeWidgetProperties_Parms), Z_Construct_UFunction_UUMGManager_SynchronizeWidgetProperties_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UUMGManager_SynchronizeWidgetProperties_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04022409, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UUMGManager_SynchronizeWidgetProperties_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UUMGManager_SynchronizeWidgetProperties_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UUMGManager_SynchronizeWidgetProperties()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UUMGManager_SynchronizeWidgetProperties_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	UClass* Z_Construct_UClass_UUMGManager_NoRegister()
	{
		return UUMGManager::StaticClass();
	}
	struct Z_Construct_UClass_UUMGManager_Statics
	{
		static UObject* (*const DependentSingletons[])();
		static const FClassFunctionLinkInfo FuncInfo[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Class_MetaDataParams[];
#endif
		static const FCppClassTypeInfoStatic StaticCppClassTypeInfo;
		static const UE4CodeGen_Private::FClassParams ClassParams;
	};
	UObject* (*const Z_Construct_UClass_UUMGManager_Statics::DependentSingletons[])() = {
		(UObject* (*)())Z_Construct_UClass_UBlueprintFunctionLibrary,
		(UObject* (*)())Z_Construct_UPackage__Script_ReactUMG,
	};
	const FClassFunctionLinkInfo Z_Construct_UClass_UUMGManager_Statics::FuncInfo[] = {
		{ &Z_Construct_UFunction_UUMGManager_CreateReactWidget, "CreateReactWidget" }, // 1180965884
		{ &Z_Construct_UFunction_UUMGManager_CreateWidget, "CreateWidget" }, // 3069800550
		{ &Z_Construct_UFunction_UUMGManager_SynchronizeSlotProperties, "SynchronizeSlotProperties" }, // 220207721
		{ &Z_Construct_UFunction_UUMGManager_SynchronizeWidgetProperties, "SynchronizeWidgetProperties" }, // 2402289647
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UUMGManager_Statics::Class_MetaDataParams[] = {
		{ "Comment", "/**\n *\n */" },
		{ "IncludePath", "UMGManager.h" },
		{ "ModuleRelativePath", "UMGManager.h" },
	};
#endif
	const FCppClassTypeInfoStatic Z_Construct_UClass_UUMGManager_Statics::StaticCppClassTypeInfo = {
		TCppClassTypeTraits<UUMGManager>::IsAbstract,
	};
	const UE4CodeGen_Private::FClassParams Z_Construct_UClass_UUMGManager_Statics::ClassParams = {
		&UUMGManager::StaticClass,
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
		0x001000A0u,
		METADATA_PARAMS(Z_Construct_UClass_UUMGManager_Statics::Class_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UClass_UUMGManager_Statics::Class_MetaDataParams))
	};
	UClass* Z_Construct_UClass_UUMGManager()
	{
		static UClass* OuterClass = nullptr;
		if (!OuterClass)
		{
			UE4CodeGen_Private::ConstructUClass(OuterClass, Z_Construct_UClass_UUMGManager_Statics::ClassParams);
		}
		return OuterClass;
	}
	IMPLEMENT_CLASS(UUMGManager, 3947671245);
	template<> REACTUMG_API UClass* StaticClass<UUMGManager>()
	{
		return UUMGManager::StaticClass();
	}
	static FCompiledInDefer Z_CompiledInDefer_UClass_UUMGManager(Z_Construct_UClass_UUMGManager, &UUMGManager::StaticClass, TEXT("/Script/ReactUMG"), TEXT("UUMGManager"), false, nullptr, nullptr, nullptr);
	DEFINE_VTABLE_PTR_HELPER_CTOR(UUMGManager);
PRAGMA_ENABLE_DEPRECATION_WARNINGS
#ifdef _MSC_VER
#pragma warning (pop)
#endif
