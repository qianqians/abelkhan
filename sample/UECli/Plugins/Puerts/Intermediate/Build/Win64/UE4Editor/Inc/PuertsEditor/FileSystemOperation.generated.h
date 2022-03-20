// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/ObjectMacros.h"
#include "UObject/ScriptMacros.h"

PRAGMA_DISABLE_DEPRECATION_WARNINGS
#ifdef PUERTSEDITOR_FileSystemOperation_generated_h
#error "FileSystemOperation.generated.h already included, missing '#pragma once' in FileSystemOperation.h"
#endif
#define PUERTSEDITOR_FileSystemOperation_generated_h

#define UECli_Plugins_Puerts_Source_PuertsEditor_Public_FileSystemOperation_h_15_SPARSE_DATA
#define UECli_Plugins_Puerts_Source_PuertsEditor_Public_FileSystemOperation_h_15_RPC_WRAPPERS \
 \
	DECLARE_FUNCTION(execFileMD5Hash); \
	DECLARE_FUNCTION(execPuertsNotifyChange); \
	DECLARE_FUNCTION(execGetFiles); \
	DECLARE_FUNCTION(execGetDirectories); \
	DECLARE_FUNCTION(execGetCurrentDirectory); \
	DECLARE_FUNCTION(execCreateDirectory); \
	DECLARE_FUNCTION(execDirectoryExists); \
	DECLARE_FUNCTION(execFileExists); \
	DECLARE_FUNCTION(execResolvePath); \
	DECLARE_FUNCTION(execWriteFile); \
	DECLARE_FUNCTION(execReadFile);


#define UECli_Plugins_Puerts_Source_PuertsEditor_Public_FileSystemOperation_h_15_RPC_WRAPPERS_NO_PURE_DECLS \
 \
	DECLARE_FUNCTION(execFileMD5Hash); \
	DECLARE_FUNCTION(execPuertsNotifyChange); \
	DECLARE_FUNCTION(execGetFiles); \
	DECLARE_FUNCTION(execGetDirectories); \
	DECLARE_FUNCTION(execGetCurrentDirectory); \
	DECLARE_FUNCTION(execCreateDirectory); \
	DECLARE_FUNCTION(execDirectoryExists); \
	DECLARE_FUNCTION(execFileExists); \
	DECLARE_FUNCTION(execResolvePath); \
	DECLARE_FUNCTION(execWriteFile); \
	DECLARE_FUNCTION(execReadFile);


#define UECli_Plugins_Puerts_Source_PuertsEditor_Public_FileSystemOperation_h_15_INCLASS_NO_PURE_DECLS \
private: \
	static void StaticRegisterNativesUFileSystemOperation(); \
	friend struct Z_Construct_UClass_UFileSystemOperation_Statics; \
public: \
	DECLARE_CLASS(UFileSystemOperation, UBlueprintFunctionLibrary, COMPILED_IN_FLAGS(0), CASTCLASS_None, TEXT("/Script/PuertsEditor"), NO_API) \
	DECLARE_SERIALIZER(UFileSystemOperation)


#define UECli_Plugins_Puerts_Source_PuertsEditor_Public_FileSystemOperation_h_15_INCLASS \
private: \
	static void StaticRegisterNativesUFileSystemOperation(); \
	friend struct Z_Construct_UClass_UFileSystemOperation_Statics; \
public: \
	DECLARE_CLASS(UFileSystemOperation, UBlueprintFunctionLibrary, COMPILED_IN_FLAGS(0), CASTCLASS_None, TEXT("/Script/PuertsEditor"), NO_API) \
	DECLARE_SERIALIZER(UFileSystemOperation)


#define UECli_Plugins_Puerts_Source_PuertsEditor_Public_FileSystemOperation_h_15_STANDARD_CONSTRUCTORS \
	/** Standard constructor, called after all reflected properties have been initialized */ \
	NO_API UFileSystemOperation(const FObjectInitializer& ObjectInitializer = FObjectInitializer::Get()); \
	DEFINE_DEFAULT_OBJECT_INITIALIZER_CONSTRUCTOR_CALL(UFileSystemOperation) \
	DECLARE_VTABLE_PTR_HELPER_CTOR(NO_API, UFileSystemOperation); \
	DEFINE_VTABLE_PTR_HELPER_CTOR_CALLER(UFileSystemOperation); \
private: \
	/** Private move- and copy-constructors, should never be used */ \
	NO_API UFileSystemOperation(UFileSystemOperation&&); \
	NO_API UFileSystemOperation(const UFileSystemOperation&); \
public:


#define UECli_Plugins_Puerts_Source_PuertsEditor_Public_FileSystemOperation_h_15_ENHANCED_CONSTRUCTORS \
	/** Standard constructor, called after all reflected properties have been initialized */ \
	NO_API UFileSystemOperation(const FObjectInitializer& ObjectInitializer = FObjectInitializer::Get()) : Super(ObjectInitializer) { }; \
private: \
	/** Private move- and copy-constructors, should never be used */ \
	NO_API UFileSystemOperation(UFileSystemOperation&&); \
	NO_API UFileSystemOperation(const UFileSystemOperation&); \
public: \
	DECLARE_VTABLE_PTR_HELPER_CTOR(NO_API, UFileSystemOperation); \
	DEFINE_VTABLE_PTR_HELPER_CTOR_CALLER(UFileSystemOperation); \
	DEFINE_DEFAULT_OBJECT_INITIALIZER_CONSTRUCTOR_CALL(UFileSystemOperation)


#define UECli_Plugins_Puerts_Source_PuertsEditor_Public_FileSystemOperation_h_15_PRIVATE_PROPERTY_OFFSET
#define UECli_Plugins_Puerts_Source_PuertsEditor_Public_FileSystemOperation_h_12_PROLOG
#define UECli_Plugins_Puerts_Source_PuertsEditor_Public_FileSystemOperation_h_15_GENERATED_BODY_LEGACY \
PRAGMA_DISABLE_DEPRECATION_WARNINGS \
public: \
	UECli_Plugins_Puerts_Source_PuertsEditor_Public_FileSystemOperation_h_15_PRIVATE_PROPERTY_OFFSET \
	UECli_Plugins_Puerts_Source_PuertsEditor_Public_FileSystemOperation_h_15_SPARSE_DATA \
	UECli_Plugins_Puerts_Source_PuertsEditor_Public_FileSystemOperation_h_15_RPC_WRAPPERS \
	UECli_Plugins_Puerts_Source_PuertsEditor_Public_FileSystemOperation_h_15_INCLASS \
	UECli_Plugins_Puerts_Source_PuertsEditor_Public_FileSystemOperation_h_15_STANDARD_CONSTRUCTORS \
public: \
PRAGMA_ENABLE_DEPRECATION_WARNINGS


#define UECli_Plugins_Puerts_Source_PuertsEditor_Public_FileSystemOperation_h_15_GENERATED_BODY \
PRAGMA_DISABLE_DEPRECATION_WARNINGS \
public: \
	UECli_Plugins_Puerts_Source_PuertsEditor_Public_FileSystemOperation_h_15_PRIVATE_PROPERTY_OFFSET \
	UECli_Plugins_Puerts_Source_PuertsEditor_Public_FileSystemOperation_h_15_SPARSE_DATA \
	UECli_Plugins_Puerts_Source_PuertsEditor_Public_FileSystemOperation_h_15_RPC_WRAPPERS_NO_PURE_DECLS \
	UECli_Plugins_Puerts_Source_PuertsEditor_Public_FileSystemOperation_h_15_INCLASS_NO_PURE_DECLS \
	UECli_Plugins_Puerts_Source_PuertsEditor_Public_FileSystemOperation_h_15_ENHANCED_CONSTRUCTORS \
private: \
PRAGMA_ENABLE_DEPRECATION_WARNINGS


template<> PUERTSEDITOR_API UClass* StaticClass<class UFileSystemOperation>();

#undef CURRENT_FILE_ID
#define CURRENT_FILE_ID UECli_Plugins_Puerts_Source_PuertsEditor_Public_FileSystemOperation_h


PRAGMA_ENABLE_DEPRECATION_WARNINGS
