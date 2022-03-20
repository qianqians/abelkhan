// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/ObjectMacros.h"
#include "UObject/ScriptMacros.h"

PRAGMA_DISABLE_DEPRECATION_WARNINGS
struct FPEGraphPinType;
struct FPEGraphTerminalType;
class UPEPropertyMetaData;
class UPEFunctionMetaData;
class UPEParamMetaData;
class UObject;
class UPEClassMetaData;
#ifdef PUERTSEDITOR_PEBlueprintAsset_generated_h
#error "PEBlueprintAsset.generated.h already included, missing '#pragma once' in PEBlueprintAsset.h"
#endif
#define PUERTSEDITOR_PEBlueprintAsset_generated_h

#define UECli_Plugins_Puerts_Source_PuertsEditor_Public_PEBlueprintAsset_h_37_GENERATED_BODY \
	friend struct Z_Construct_UScriptStruct_FPEGraphPinType_Statics; \
	PUERTSEDITOR_API static class UScriptStruct* StaticStruct();


template<> PUERTSEDITOR_API UScriptStruct* StaticStruct<struct FPEGraphPinType>();

#define UECli_Plugins_Puerts_Source_PuertsEditor_Public_PEBlueprintAsset_h_23_GENERATED_BODY \
	friend struct Z_Construct_UScriptStruct_FPEGraphTerminalType_Statics; \
	PUERTSEDITOR_API static class UScriptStruct* StaticStruct();


template<> PUERTSEDITOR_API UScriptStruct* StaticStruct<struct FPEGraphTerminalType>();

#define UECli_Plugins_Puerts_Source_PuertsEditor_Public_PEBlueprintAsset_h_62_SPARSE_DATA
#define UECli_Plugins_Puerts_Source_PuertsEditor_Public_PEBlueprintAsset_h_62_RPC_WRAPPERS \
 \
	DECLARE_FUNCTION(execSave); \
	DECLARE_FUNCTION(execRemoveNotExistedMemberVariable); \
	DECLARE_FUNCTION(execAddMemberVariableWithMetaData); \
	DECLARE_FUNCTION(execAddMemberVariable); \
	DECLARE_FUNCTION(execRemoveNotExistedFunction); \
	DECLARE_FUNCTION(execAddFunctionWithMetaData); \
	DECLARE_FUNCTION(execAddFunction); \
	DECLARE_FUNCTION(execClearParameter); \
	DECLARE_FUNCTION(execAddParameterWithMetaData); \
	DECLARE_FUNCTION(execAddParameter); \
	DECLARE_FUNCTION(execLoadOrCreateWithMetaData); \
	DECLARE_FUNCTION(execLoadOrCreate);


#define UECli_Plugins_Puerts_Source_PuertsEditor_Public_PEBlueprintAsset_h_62_RPC_WRAPPERS_NO_PURE_DECLS \
 \
	DECLARE_FUNCTION(execSave); \
	DECLARE_FUNCTION(execRemoveNotExistedMemberVariable); \
	DECLARE_FUNCTION(execAddMemberVariableWithMetaData); \
	DECLARE_FUNCTION(execAddMemberVariable); \
	DECLARE_FUNCTION(execRemoveNotExistedFunction); \
	DECLARE_FUNCTION(execAddFunctionWithMetaData); \
	DECLARE_FUNCTION(execAddFunction); \
	DECLARE_FUNCTION(execClearParameter); \
	DECLARE_FUNCTION(execAddParameterWithMetaData); \
	DECLARE_FUNCTION(execAddParameter); \
	DECLARE_FUNCTION(execLoadOrCreateWithMetaData); \
	DECLARE_FUNCTION(execLoadOrCreate);


#define UECli_Plugins_Puerts_Source_PuertsEditor_Public_PEBlueprintAsset_h_62_INCLASS_NO_PURE_DECLS \
private: \
	static void StaticRegisterNativesUPEBlueprintAsset(); \
	friend struct Z_Construct_UClass_UPEBlueprintAsset_Statics; \
public: \
	DECLARE_CLASS(UPEBlueprintAsset, UObject, COMPILED_IN_FLAGS(0), CASTCLASS_None, TEXT("/Script/PuertsEditor"), NO_API) \
	DECLARE_SERIALIZER(UPEBlueprintAsset)


#define UECli_Plugins_Puerts_Source_PuertsEditor_Public_PEBlueprintAsset_h_62_INCLASS \
private: \
	static void StaticRegisterNativesUPEBlueprintAsset(); \
	friend struct Z_Construct_UClass_UPEBlueprintAsset_Statics; \
public: \
	DECLARE_CLASS(UPEBlueprintAsset, UObject, COMPILED_IN_FLAGS(0), CASTCLASS_None, TEXT("/Script/PuertsEditor"), NO_API) \
	DECLARE_SERIALIZER(UPEBlueprintAsset)


#define UECli_Plugins_Puerts_Source_PuertsEditor_Public_PEBlueprintAsset_h_62_STANDARD_CONSTRUCTORS \
	/** Standard constructor, called after all reflected properties have been initialized */ \
	NO_API UPEBlueprintAsset(const FObjectInitializer& ObjectInitializer = FObjectInitializer::Get()); \
	DEFINE_DEFAULT_OBJECT_INITIALIZER_CONSTRUCTOR_CALL(UPEBlueprintAsset) \
	DECLARE_VTABLE_PTR_HELPER_CTOR(NO_API, UPEBlueprintAsset); \
	DEFINE_VTABLE_PTR_HELPER_CTOR_CALLER(UPEBlueprintAsset); \
private: \
	/** Private move- and copy-constructors, should never be used */ \
	NO_API UPEBlueprintAsset(UPEBlueprintAsset&&); \
	NO_API UPEBlueprintAsset(const UPEBlueprintAsset&); \
public:


#define UECli_Plugins_Puerts_Source_PuertsEditor_Public_PEBlueprintAsset_h_62_ENHANCED_CONSTRUCTORS \
	/** Standard constructor, called after all reflected properties have been initialized */ \
	NO_API UPEBlueprintAsset(const FObjectInitializer& ObjectInitializer = FObjectInitializer::Get()) : Super(ObjectInitializer) { }; \
private: \
	/** Private move- and copy-constructors, should never be used */ \
	NO_API UPEBlueprintAsset(UPEBlueprintAsset&&); \
	NO_API UPEBlueprintAsset(const UPEBlueprintAsset&); \
public: \
	DECLARE_VTABLE_PTR_HELPER_CTOR(NO_API, UPEBlueprintAsset); \
	DEFINE_VTABLE_PTR_HELPER_CTOR_CALLER(UPEBlueprintAsset); \
	DEFINE_DEFAULT_OBJECT_INITIALIZER_CONSTRUCTOR_CALL(UPEBlueprintAsset)


#define UECli_Plugins_Puerts_Source_PuertsEditor_Public_PEBlueprintAsset_h_62_PRIVATE_PROPERTY_OFFSET
#define UECli_Plugins_Puerts_Source_PuertsEditor_Public_PEBlueprintAsset_h_59_PROLOG
#define UECli_Plugins_Puerts_Source_PuertsEditor_Public_PEBlueprintAsset_h_62_GENERATED_BODY_LEGACY \
PRAGMA_DISABLE_DEPRECATION_WARNINGS \
public: \
	UECli_Plugins_Puerts_Source_PuertsEditor_Public_PEBlueprintAsset_h_62_PRIVATE_PROPERTY_OFFSET \
	UECli_Plugins_Puerts_Source_PuertsEditor_Public_PEBlueprintAsset_h_62_SPARSE_DATA \
	UECli_Plugins_Puerts_Source_PuertsEditor_Public_PEBlueprintAsset_h_62_RPC_WRAPPERS \
	UECli_Plugins_Puerts_Source_PuertsEditor_Public_PEBlueprintAsset_h_62_INCLASS \
	UECli_Plugins_Puerts_Source_PuertsEditor_Public_PEBlueprintAsset_h_62_STANDARD_CONSTRUCTORS \
public: \
PRAGMA_ENABLE_DEPRECATION_WARNINGS


#define UECli_Plugins_Puerts_Source_PuertsEditor_Public_PEBlueprintAsset_h_62_GENERATED_BODY \
PRAGMA_DISABLE_DEPRECATION_WARNINGS \
public: \
	UECli_Plugins_Puerts_Source_PuertsEditor_Public_PEBlueprintAsset_h_62_PRIVATE_PROPERTY_OFFSET \
	UECli_Plugins_Puerts_Source_PuertsEditor_Public_PEBlueprintAsset_h_62_SPARSE_DATA \
	UECli_Plugins_Puerts_Source_PuertsEditor_Public_PEBlueprintAsset_h_62_RPC_WRAPPERS_NO_PURE_DECLS \
	UECli_Plugins_Puerts_Source_PuertsEditor_Public_PEBlueprintAsset_h_62_INCLASS_NO_PURE_DECLS \
	UECli_Plugins_Puerts_Source_PuertsEditor_Public_PEBlueprintAsset_h_62_ENHANCED_CONSTRUCTORS \
private: \
PRAGMA_ENABLE_DEPRECATION_WARNINGS


template<> PUERTSEDITOR_API UClass* StaticClass<class UPEBlueprintAsset>();

#undef CURRENT_FILE_ID
#define CURRENT_FILE_ID UECli_Plugins_Puerts_Source_PuertsEditor_Public_PEBlueprintAsset_h


PRAGMA_ENABLE_DEPRECATION_WARNINGS
