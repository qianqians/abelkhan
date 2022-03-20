// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/ObjectMacros.h"
#include "UObject/ScriptMacros.h"

PRAGMA_DISABLE_DEPRECATION_WARNINGS
#ifdef DECLARATIONGENERATOR_CodeGenerator_generated_h
#error "CodeGenerator.generated.h already included, missing '#pragma once' in CodeGenerator.h"
#endif
#define DECLARATIONGENERATOR_CodeGenerator_generated_h

#define UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_13_SPARSE_DATA
#define UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_13_RPC_WRAPPERS \
	virtual void Gen_Implementation() const {}; \
 \
	DECLARE_FUNCTION(execGen);


#define UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_13_RPC_WRAPPERS_NO_PURE_DECLS \
	virtual void Gen_Implementation() const {}; \
 \
	DECLARE_FUNCTION(execGen);


#define UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_13_EVENT_PARMS
#define UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_13_CALLBACK_WRAPPERS
#define UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_13_STANDARD_CONSTRUCTORS \
	/** Standard constructor, called after all reflected properties have been initialized */ \
	DECLARATIONGENERATOR_API UCodeGenerator(const FObjectInitializer& ObjectInitializer = FObjectInitializer::Get()); \
	DEFINE_ABSTRACT_DEFAULT_OBJECT_INITIALIZER_CONSTRUCTOR_CALL(UCodeGenerator) \
	DECLARE_VTABLE_PTR_HELPER_CTOR(DECLARATIONGENERATOR_API, UCodeGenerator); \
	DEFINE_VTABLE_PTR_HELPER_CTOR_CALLER(UCodeGenerator); \
private: \
	/** Private move- and copy-constructors, should never be used */ \
	DECLARATIONGENERATOR_API UCodeGenerator(UCodeGenerator&&); \
	DECLARATIONGENERATOR_API UCodeGenerator(const UCodeGenerator&); \
public:


#define UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_13_ENHANCED_CONSTRUCTORS \
	/** Standard constructor, called after all reflected properties have been initialized */ \
	DECLARATIONGENERATOR_API UCodeGenerator(const FObjectInitializer& ObjectInitializer = FObjectInitializer::Get()) : Super(ObjectInitializer) { }; \
private: \
	/** Private move- and copy-constructors, should never be used */ \
	DECLARATIONGENERATOR_API UCodeGenerator(UCodeGenerator&&); \
	DECLARATIONGENERATOR_API UCodeGenerator(const UCodeGenerator&); \
public: \
	DECLARE_VTABLE_PTR_HELPER_CTOR(DECLARATIONGENERATOR_API, UCodeGenerator); \
	DEFINE_VTABLE_PTR_HELPER_CTOR_CALLER(UCodeGenerator); \
	DEFINE_ABSTRACT_DEFAULT_OBJECT_INITIALIZER_CONSTRUCTOR_CALL(UCodeGenerator)


#define UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_13_GENERATED_UINTERFACE_BODY() \
private: \
	static void StaticRegisterNativesUCodeGenerator(); \
	friend struct Z_Construct_UClass_UCodeGenerator_Statics; \
public: \
	DECLARE_CLASS(UCodeGenerator, UInterface, COMPILED_IN_FLAGS(CLASS_Abstract | CLASS_Interface), CASTCLASS_None, TEXT("/Script/DeclarationGenerator"), DECLARATIONGENERATOR_API) \
	DECLARE_SERIALIZER(UCodeGenerator)


#define UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_13_GENERATED_BODY_LEGACY \
		PRAGMA_DISABLE_DEPRECATION_WARNINGS \
	UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_13_GENERATED_UINTERFACE_BODY() \
	UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_13_STANDARD_CONSTRUCTORS \
	PRAGMA_ENABLE_DEPRECATION_WARNINGS


#define UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_13_GENERATED_BODY \
	PRAGMA_DISABLE_DEPRECATION_WARNINGS \
	UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_13_GENERATED_UINTERFACE_BODY() \
	UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_13_ENHANCED_CONSTRUCTORS \
private: \
	PRAGMA_ENABLE_DEPRECATION_WARNINGS


#define UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_13_INCLASS_IINTERFACE_NO_PURE_DECLS \
protected: \
	virtual ~ICodeGenerator() {} \
public: \
	typedef UCodeGenerator UClassType; \
	typedef ICodeGenerator ThisClass; \
	static void Execute_Gen(const UObject* O); \
	virtual UObject* _getUObject() const { check(0 && "Missing required implementation."); return nullptr; }


#define UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_13_INCLASS_IINTERFACE \
protected: \
	virtual ~ICodeGenerator() {} \
public: \
	typedef UCodeGenerator UClassType; \
	typedef ICodeGenerator ThisClass; \
	static void Execute_Gen(const UObject* O); \
	virtual UObject* _getUObject() const { check(0 && "Missing required implementation."); return nullptr; }


#define UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_10_PROLOG \
	UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_13_EVENT_PARMS


#define UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_21_GENERATED_BODY_LEGACY \
PRAGMA_DISABLE_DEPRECATION_WARNINGS \
public: \
	UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_13_SPARSE_DATA \
	UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_13_RPC_WRAPPERS \
	UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_13_CALLBACK_WRAPPERS \
	UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_13_INCLASS_IINTERFACE \
public: \
PRAGMA_ENABLE_DEPRECATION_WARNINGS


#define UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_21_GENERATED_BODY \
PRAGMA_DISABLE_DEPRECATION_WARNINGS \
public: \
	UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_13_SPARSE_DATA \
	UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_13_RPC_WRAPPERS_NO_PURE_DECLS \
	UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_13_CALLBACK_WRAPPERS \
	UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h_13_INCLASS_IINTERFACE_NO_PURE_DECLS \
private: \
PRAGMA_ENABLE_DEPRECATION_WARNINGS


template<> DECLARATIONGENERATOR_API UClass* StaticClass<class UCodeGenerator>();

#undef CURRENT_FILE_ID
#define CURRENT_FILE_ID UECli_Plugins_Puerts_Source_DeclarationGenerator_Public_CodeGenerator_h


PRAGMA_ENABLE_DEPRECATION_WARNINGS
