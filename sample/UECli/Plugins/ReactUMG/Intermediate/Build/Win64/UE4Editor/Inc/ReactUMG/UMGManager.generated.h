// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/ObjectMacros.h"
#include "UObject/ScriptMacros.h"

PRAGMA_DISABLE_DEPRECATION_WARNINGS
class UPanelSlot;
class UWidget;
class UWorld;
class UObject;
class UUserWidget;
class UReactWidget;
#ifdef REACTUMG_UMGManager_generated_h
#error "UMGManager.generated.h already included, missing '#pragma once' in UMGManager.h"
#endif
#define REACTUMG_UMGManager_generated_h

#define UECli_Plugins_ReactUMG_Source_ReactUMG_UMGManager_h_26_SPARSE_DATA
#define UECli_Plugins_ReactUMG_Source_ReactUMG_UMGManager_h_26_RPC_WRAPPERS \
 \
	DECLARE_FUNCTION(execSynchronizeSlotProperties); \
	DECLARE_FUNCTION(execSynchronizeWidgetProperties); \
	DECLARE_FUNCTION(execCreateWidget); \
	DECLARE_FUNCTION(execCreateReactWidget);


#define UECli_Plugins_ReactUMG_Source_ReactUMG_UMGManager_h_26_RPC_WRAPPERS_NO_PURE_DECLS \
 \
	DECLARE_FUNCTION(execSynchronizeSlotProperties); \
	DECLARE_FUNCTION(execSynchronizeWidgetProperties); \
	DECLARE_FUNCTION(execCreateWidget); \
	DECLARE_FUNCTION(execCreateReactWidget);


#define UECli_Plugins_ReactUMG_Source_ReactUMG_UMGManager_h_26_INCLASS_NO_PURE_DECLS \
private: \
	static void StaticRegisterNativesUUMGManager(); \
	friend struct Z_Construct_UClass_UUMGManager_Statics; \
public: \
	DECLARE_CLASS(UUMGManager, UBlueprintFunctionLibrary, COMPILED_IN_FLAGS(0), CASTCLASS_None, TEXT("/Script/ReactUMG"), NO_API) \
	DECLARE_SERIALIZER(UUMGManager)


#define UECli_Plugins_ReactUMG_Source_ReactUMG_UMGManager_h_26_INCLASS \
private: \
	static void StaticRegisterNativesUUMGManager(); \
	friend struct Z_Construct_UClass_UUMGManager_Statics; \
public: \
	DECLARE_CLASS(UUMGManager, UBlueprintFunctionLibrary, COMPILED_IN_FLAGS(0), CASTCLASS_None, TEXT("/Script/ReactUMG"), NO_API) \
	DECLARE_SERIALIZER(UUMGManager)


#define UECli_Plugins_ReactUMG_Source_ReactUMG_UMGManager_h_26_STANDARD_CONSTRUCTORS \
	/** Standard constructor, called after all reflected properties have been initialized */ \
	NO_API UUMGManager(const FObjectInitializer& ObjectInitializer = FObjectInitializer::Get()); \
	DEFINE_DEFAULT_OBJECT_INITIALIZER_CONSTRUCTOR_CALL(UUMGManager) \
	DECLARE_VTABLE_PTR_HELPER_CTOR(NO_API, UUMGManager); \
	DEFINE_VTABLE_PTR_HELPER_CTOR_CALLER(UUMGManager); \
private: \
	/** Private move- and copy-constructors, should never be used */ \
	NO_API UUMGManager(UUMGManager&&); \
	NO_API UUMGManager(const UUMGManager&); \
public:


#define UECli_Plugins_ReactUMG_Source_ReactUMG_UMGManager_h_26_ENHANCED_CONSTRUCTORS \
	/** Standard constructor, called after all reflected properties have been initialized */ \
	NO_API UUMGManager(const FObjectInitializer& ObjectInitializer = FObjectInitializer::Get()) : Super(ObjectInitializer) { }; \
private: \
	/** Private move- and copy-constructors, should never be used */ \
	NO_API UUMGManager(UUMGManager&&); \
	NO_API UUMGManager(const UUMGManager&); \
public: \
	DECLARE_VTABLE_PTR_HELPER_CTOR(NO_API, UUMGManager); \
	DEFINE_VTABLE_PTR_HELPER_CTOR_CALLER(UUMGManager); \
	DEFINE_DEFAULT_OBJECT_INITIALIZER_CONSTRUCTOR_CALL(UUMGManager)


#define UECli_Plugins_ReactUMG_Source_ReactUMG_UMGManager_h_26_PRIVATE_PROPERTY_OFFSET
#define UECli_Plugins_ReactUMG_Source_ReactUMG_UMGManager_h_23_PROLOG
#define UECli_Plugins_ReactUMG_Source_ReactUMG_UMGManager_h_26_GENERATED_BODY_LEGACY \
PRAGMA_DISABLE_DEPRECATION_WARNINGS \
public: \
	UECli_Plugins_ReactUMG_Source_ReactUMG_UMGManager_h_26_PRIVATE_PROPERTY_OFFSET \
	UECli_Plugins_ReactUMG_Source_ReactUMG_UMGManager_h_26_SPARSE_DATA \
	UECli_Plugins_ReactUMG_Source_ReactUMG_UMGManager_h_26_RPC_WRAPPERS \
	UECli_Plugins_ReactUMG_Source_ReactUMG_UMGManager_h_26_INCLASS \
	UECli_Plugins_ReactUMG_Source_ReactUMG_UMGManager_h_26_STANDARD_CONSTRUCTORS \
public: \
PRAGMA_ENABLE_DEPRECATION_WARNINGS


#define UECli_Plugins_ReactUMG_Source_ReactUMG_UMGManager_h_26_GENERATED_BODY \
PRAGMA_DISABLE_DEPRECATION_WARNINGS \
public: \
	UECli_Plugins_ReactUMG_Source_ReactUMG_UMGManager_h_26_PRIVATE_PROPERTY_OFFSET \
	UECli_Plugins_ReactUMG_Source_ReactUMG_UMGManager_h_26_SPARSE_DATA \
	UECli_Plugins_ReactUMG_Source_ReactUMG_UMGManager_h_26_RPC_WRAPPERS_NO_PURE_DECLS \
	UECli_Plugins_ReactUMG_Source_ReactUMG_UMGManager_h_26_INCLASS_NO_PURE_DECLS \
	UECli_Plugins_ReactUMG_Source_ReactUMG_UMGManager_h_26_ENHANCED_CONSTRUCTORS \
private: \
PRAGMA_ENABLE_DEPRECATION_WARNINGS


template<> REACTUMG_API UClass* StaticClass<class UUMGManager>();

#undef CURRENT_FILE_ID
#define CURRENT_FILE_ID UECli_Plugins_ReactUMG_Source_ReactUMG_UMGManager_h


PRAGMA_ENABLE_DEPRECATION_WARNINGS
