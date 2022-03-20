// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/ObjectMacros.h"
#include "UObject/ScriptMacros.h"

PRAGMA_DISABLE_DEPRECATION_WARNINGS
#ifdef JSENV_TypeScriptObject_generated_h
#error "TypeScriptObject.generated.h already included, missing '#pragma once' in TypeScriptObject.h"
#endif
#define JSENV_TypeScriptObject_generated_h

#define UECli_Plugins_Puerts_Source_JsEnv_Public_TypeScriptObject_h_19_SPARSE_DATA
#define UECli_Plugins_Puerts_Source_JsEnv_Public_TypeScriptObject_h_19_RPC_WRAPPERS
#define UECli_Plugins_Puerts_Source_JsEnv_Public_TypeScriptObject_h_19_RPC_WRAPPERS_NO_PURE_DECLS
#define UECli_Plugins_Puerts_Source_JsEnv_Public_TypeScriptObject_h_19_STANDARD_CONSTRUCTORS \
	/** Standard constructor, called after all reflected properties have been initialized */ \
	JSENV_API UTypeScriptObject(const FObjectInitializer& ObjectInitializer = FObjectInitializer::Get()); \
	DEFINE_ABSTRACT_DEFAULT_OBJECT_INITIALIZER_CONSTRUCTOR_CALL(UTypeScriptObject) \
	DECLARE_VTABLE_PTR_HELPER_CTOR(JSENV_API, UTypeScriptObject); \
	DEFINE_VTABLE_PTR_HELPER_CTOR_CALLER(UTypeScriptObject); \
private: \
	/** Private move- and copy-constructors, should never be used */ \
	JSENV_API UTypeScriptObject(UTypeScriptObject&&); \
	JSENV_API UTypeScriptObject(const UTypeScriptObject&); \
public:


#define UECli_Plugins_Puerts_Source_JsEnv_Public_TypeScriptObject_h_19_ENHANCED_CONSTRUCTORS \
	/** Standard constructor, called after all reflected properties have been initialized */ \
	JSENV_API UTypeScriptObject(const FObjectInitializer& ObjectInitializer = FObjectInitializer::Get()) : Super(ObjectInitializer) { }; \
private: \
	/** Private move- and copy-constructors, should never be used */ \
	JSENV_API UTypeScriptObject(UTypeScriptObject&&); \
	JSENV_API UTypeScriptObject(const UTypeScriptObject&); \
public: \
	DECLARE_VTABLE_PTR_HELPER_CTOR(JSENV_API, UTypeScriptObject); \
	DEFINE_VTABLE_PTR_HELPER_CTOR_CALLER(UTypeScriptObject); \
	DEFINE_ABSTRACT_DEFAULT_OBJECT_INITIALIZER_CONSTRUCTOR_CALL(UTypeScriptObject)


#define UECli_Plugins_Puerts_Source_JsEnv_Public_TypeScriptObject_h_19_GENERATED_UINTERFACE_BODY() \
private: \
	static void StaticRegisterNativesUTypeScriptObject(); \
	friend struct Z_Construct_UClass_UTypeScriptObject_Statics; \
public: \
	DECLARE_CLASS(UTypeScriptObject, UInterface, COMPILED_IN_FLAGS(CLASS_Abstract | CLASS_Interface), CASTCLASS_None, TEXT("/Script/JsEnv"), JSENV_API) \
	DECLARE_SERIALIZER(UTypeScriptObject)


#define UECli_Plugins_Puerts_Source_JsEnv_Public_TypeScriptObject_h_19_GENERATED_BODY_LEGACY \
		PRAGMA_DISABLE_DEPRECATION_WARNINGS \
	UECli_Plugins_Puerts_Source_JsEnv_Public_TypeScriptObject_h_19_GENERATED_UINTERFACE_BODY() \
	UECli_Plugins_Puerts_Source_JsEnv_Public_TypeScriptObject_h_19_STANDARD_CONSTRUCTORS \
	PRAGMA_ENABLE_DEPRECATION_WARNINGS


#define UECli_Plugins_Puerts_Source_JsEnv_Public_TypeScriptObject_h_19_GENERATED_BODY \
	PRAGMA_DISABLE_DEPRECATION_WARNINGS \
	UECli_Plugins_Puerts_Source_JsEnv_Public_TypeScriptObject_h_19_GENERATED_UINTERFACE_BODY() \
	UECli_Plugins_Puerts_Source_JsEnv_Public_TypeScriptObject_h_19_ENHANCED_CONSTRUCTORS \
private: \
	PRAGMA_ENABLE_DEPRECATION_WARNINGS


#define UECli_Plugins_Puerts_Source_JsEnv_Public_TypeScriptObject_h_19_INCLASS_IINTERFACE_NO_PURE_DECLS \
protected: \
	virtual ~ITypeScriptObject() {} \
public: \
	typedef UTypeScriptObject UClassType; \
	typedef ITypeScriptObject ThisClass; \
	virtual UObject* _getUObject() const { check(0 && "Missing required implementation."); return nullptr; }


#define UECli_Plugins_Puerts_Source_JsEnv_Public_TypeScriptObject_h_19_INCLASS_IINTERFACE \
protected: \
	virtual ~ITypeScriptObject() {} \
public: \
	typedef UTypeScriptObject UClassType; \
	typedef ITypeScriptObject ThisClass; \
	virtual UObject* _getUObject() const { check(0 && "Missing required implementation."); return nullptr; }


#define UECli_Plugins_Puerts_Source_JsEnv_Public_TypeScriptObject_h_16_PROLOG
#define UECli_Plugins_Puerts_Source_JsEnv_Public_TypeScriptObject_h_27_GENERATED_BODY_LEGACY \
PRAGMA_DISABLE_DEPRECATION_WARNINGS \
public: \
	UECli_Plugins_Puerts_Source_JsEnv_Public_TypeScriptObject_h_19_SPARSE_DATA \
	UECli_Plugins_Puerts_Source_JsEnv_Public_TypeScriptObject_h_19_RPC_WRAPPERS \
	UECli_Plugins_Puerts_Source_JsEnv_Public_TypeScriptObject_h_19_INCLASS_IINTERFACE \
public: \
PRAGMA_ENABLE_DEPRECATION_WARNINGS


#define UECli_Plugins_Puerts_Source_JsEnv_Public_TypeScriptObject_h_27_GENERATED_BODY \
PRAGMA_DISABLE_DEPRECATION_WARNINGS \
public: \
	UECli_Plugins_Puerts_Source_JsEnv_Public_TypeScriptObject_h_19_SPARSE_DATA \
	UECli_Plugins_Puerts_Source_JsEnv_Public_TypeScriptObject_h_19_RPC_WRAPPERS_NO_PURE_DECLS \
	UECli_Plugins_Puerts_Source_JsEnv_Public_TypeScriptObject_h_19_INCLASS_IINTERFACE_NO_PURE_DECLS \
private: \
PRAGMA_ENABLE_DEPRECATION_WARNINGS


template<> JSENV_API UClass* StaticClass<class UTypeScriptObject>();

#undef CURRENT_FILE_ID
#define CURRENT_FILE_ID UECli_Plugins_Puerts_Source_JsEnv_Public_TypeScriptObject_h


PRAGMA_ENABLE_DEPRECATION_WARNINGS
