// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/GeneratedCppIncludes.h"
#include "PuertsEditor/Public/FileSystemOperation.h"
#ifdef _MSC_VER
#pragma warning (push)
#pragma warning (disable : 4883)
#endif
PRAGMA_DISABLE_DEPRECATION_WARNINGS
void EmptyLinkFunctionForGeneratedCodeFileSystemOperation() {}
// Cross Module References
	PUERTSEDITOR_API UClass* Z_Construct_UClass_UFileSystemOperation_NoRegister();
	PUERTSEDITOR_API UClass* Z_Construct_UClass_UFileSystemOperation();
	ENGINE_API UClass* Z_Construct_UClass_UBlueprintFunctionLibrary();
	UPackage* Z_Construct_UPackage__Script_PuertsEditor();
// End Cross Module References
	DEFINE_FUNCTION(UFileSystemOperation::execFileMD5Hash)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_Path);
		P_FINISH;
		P_NATIVE_BEGIN;
		*(FString*)Z_Param__Result=UFileSystemOperation::FileMD5Hash(Z_Param_Path);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UFileSystemOperation::execPuertsNotifyChange)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_Path);
		P_GET_PROPERTY(FStrProperty,Z_Param_Source);
		P_FINISH;
		P_NATIVE_BEGIN;
		UFileSystemOperation::PuertsNotifyChange(Z_Param_Path,Z_Param_Source);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UFileSystemOperation::execGetFiles)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_Path);
		P_FINISH;
		P_NATIVE_BEGIN;
		*(TArray<FString>*)Z_Param__Result=UFileSystemOperation::GetFiles(Z_Param_Path);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UFileSystemOperation::execGetDirectories)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_Path);
		P_FINISH;
		P_NATIVE_BEGIN;
		*(TArray<FString>*)Z_Param__Result=UFileSystemOperation::GetDirectories(Z_Param_Path);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UFileSystemOperation::execGetCurrentDirectory)
	{
		P_FINISH;
		P_NATIVE_BEGIN;
		*(FString*)Z_Param__Result=UFileSystemOperation::GetCurrentDirectory();
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UFileSystemOperation::execCreateDirectory)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_Path);
		P_FINISH;
		P_NATIVE_BEGIN;
		UFileSystemOperation::CreateDirectory(Z_Param_Path);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UFileSystemOperation::execDirectoryExists)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_Path);
		P_FINISH;
		P_NATIVE_BEGIN;
		*(bool*)Z_Param__Result=UFileSystemOperation::DirectoryExists(Z_Param_Path);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UFileSystemOperation::execFileExists)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_Path);
		P_FINISH;
		P_NATIVE_BEGIN;
		*(bool*)Z_Param__Result=UFileSystemOperation::FileExists(Z_Param_Path);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UFileSystemOperation::execResolvePath)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_Path);
		P_FINISH;
		P_NATIVE_BEGIN;
		*(FString*)Z_Param__Result=UFileSystemOperation::ResolvePath(Z_Param_Path);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UFileSystemOperation::execWriteFile)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_Path);
		P_GET_PROPERTY(FStrProperty,Z_Param_Data);
		P_FINISH;
		P_NATIVE_BEGIN;
		UFileSystemOperation::WriteFile(Z_Param_Path,Z_Param_Data);
		P_NATIVE_END;
	}
	DEFINE_FUNCTION(UFileSystemOperation::execReadFile)
	{
		P_GET_PROPERTY(FStrProperty,Z_Param_Path);
		P_GET_PROPERTY_REF(FStrProperty,Z_Param_Out_Data);
		P_FINISH;
		P_NATIVE_BEGIN;
		*(bool*)Z_Param__Result=UFileSystemOperation::ReadFile(Z_Param_Path,Z_Param_Out_Data);
		P_NATIVE_END;
	}
	void UFileSystemOperation::StaticRegisterNativesUFileSystemOperation()
	{
		UClass* Class = UFileSystemOperation::StaticClass();
		static const FNameNativePtrPair Funcs[] = {
			{ "CreateDirectory", &UFileSystemOperation::execCreateDirectory },
			{ "DirectoryExists", &UFileSystemOperation::execDirectoryExists },
			{ "FileExists", &UFileSystemOperation::execFileExists },
			{ "FileMD5Hash", &UFileSystemOperation::execFileMD5Hash },
			{ "GetCurrentDirectory", &UFileSystemOperation::execGetCurrentDirectory },
			{ "GetDirectories", &UFileSystemOperation::execGetDirectories },
			{ "GetFiles", &UFileSystemOperation::execGetFiles },
			{ "PuertsNotifyChange", &UFileSystemOperation::execPuertsNotifyChange },
			{ "ReadFile", &UFileSystemOperation::execReadFile },
			{ "ResolvePath", &UFileSystemOperation::execResolvePath },
			{ "WriteFile", &UFileSystemOperation::execWriteFile },
		};
		FNativeFunctionRegistrar::RegisterFunctions(Class, Funcs, UE_ARRAY_COUNT(Funcs));
	}
	struct Z_Construct_UFunction_UFileSystemOperation_CreateDirectory_Statics
	{
		struct FileSystemOperation_eventCreateDirectory_Parms
		{
			FString Path;
		};
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_Path;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UFileSystemOperation_CreateDirectory_Statics::NewProp_Path = { "Path", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(FileSystemOperation_eventCreateDirectory_Parms, Path), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UFileSystemOperation_CreateDirectory_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UFileSystemOperation_CreateDirectory_Statics::NewProp_Path,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UFileSystemOperation_CreateDirectory_Statics::Function_MetaDataParams[] = {
		{ "Category", "File" },
		{ "ModuleRelativePath", "Public/FileSystemOperation.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UFileSystemOperation_CreateDirectory_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UFileSystemOperation, nullptr, "CreateDirectory", nullptr, nullptr, sizeof(FileSystemOperation_eventCreateDirectory_Parms), Z_Construct_UFunction_UFileSystemOperation_CreateDirectory_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UFileSystemOperation_CreateDirectory_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04042401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UFileSystemOperation_CreateDirectory_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UFileSystemOperation_CreateDirectory_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UFileSystemOperation_CreateDirectory()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UFileSystemOperation_CreateDirectory_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UFileSystemOperation_DirectoryExists_Statics
	{
		struct FileSystemOperation_eventDirectoryExists_Parms
		{
			FString Path;
			bool ReturnValue;
		};
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_Path;
		static void NewProp_ReturnValue_SetBit(void* Obj);
		static const UE4CodeGen_Private::FBoolPropertyParams NewProp_ReturnValue;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UFileSystemOperation_DirectoryExists_Statics::NewProp_Path = { "Path", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(FileSystemOperation_eventDirectoryExists_Parms, Path), METADATA_PARAMS(nullptr, 0) };
	void Z_Construct_UFunction_UFileSystemOperation_DirectoryExists_Statics::NewProp_ReturnValue_SetBit(void* Obj)
	{
		((FileSystemOperation_eventDirectoryExists_Parms*)Obj)->ReturnValue = 1;
	}
	const UE4CodeGen_Private::FBoolPropertyParams Z_Construct_UFunction_UFileSystemOperation_DirectoryExists_Statics::NewProp_ReturnValue = { "ReturnValue", nullptr, (EPropertyFlags)0x0010000000000580, UE4CodeGen_Private::EPropertyGenFlags::Bool | UE4CodeGen_Private::EPropertyGenFlags::NativeBool, RF_Public|RF_Transient|RF_MarkAsNative, 1, sizeof(bool), sizeof(FileSystemOperation_eventDirectoryExists_Parms), &Z_Construct_UFunction_UFileSystemOperation_DirectoryExists_Statics::NewProp_ReturnValue_SetBit, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UFileSystemOperation_DirectoryExists_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UFileSystemOperation_DirectoryExists_Statics::NewProp_Path,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UFileSystemOperation_DirectoryExists_Statics::NewProp_ReturnValue,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UFileSystemOperation_DirectoryExists_Statics::Function_MetaDataParams[] = {
		{ "Category", "File" },
		{ "ModuleRelativePath", "Public/FileSystemOperation.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UFileSystemOperation_DirectoryExists_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UFileSystemOperation, nullptr, "DirectoryExists", nullptr, nullptr, sizeof(FileSystemOperation_eventDirectoryExists_Parms), Z_Construct_UFunction_UFileSystemOperation_DirectoryExists_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UFileSystemOperation_DirectoryExists_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04042401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UFileSystemOperation_DirectoryExists_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UFileSystemOperation_DirectoryExists_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UFileSystemOperation_DirectoryExists()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UFileSystemOperation_DirectoryExists_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UFileSystemOperation_FileExists_Statics
	{
		struct FileSystemOperation_eventFileExists_Parms
		{
			FString Path;
			bool ReturnValue;
		};
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_Path;
		static void NewProp_ReturnValue_SetBit(void* Obj);
		static const UE4CodeGen_Private::FBoolPropertyParams NewProp_ReturnValue;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UFileSystemOperation_FileExists_Statics::NewProp_Path = { "Path", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(FileSystemOperation_eventFileExists_Parms, Path), METADATA_PARAMS(nullptr, 0) };
	void Z_Construct_UFunction_UFileSystemOperation_FileExists_Statics::NewProp_ReturnValue_SetBit(void* Obj)
	{
		((FileSystemOperation_eventFileExists_Parms*)Obj)->ReturnValue = 1;
	}
	const UE4CodeGen_Private::FBoolPropertyParams Z_Construct_UFunction_UFileSystemOperation_FileExists_Statics::NewProp_ReturnValue = { "ReturnValue", nullptr, (EPropertyFlags)0x0010000000000580, UE4CodeGen_Private::EPropertyGenFlags::Bool | UE4CodeGen_Private::EPropertyGenFlags::NativeBool, RF_Public|RF_Transient|RF_MarkAsNative, 1, sizeof(bool), sizeof(FileSystemOperation_eventFileExists_Parms), &Z_Construct_UFunction_UFileSystemOperation_FileExists_Statics::NewProp_ReturnValue_SetBit, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UFileSystemOperation_FileExists_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UFileSystemOperation_FileExists_Statics::NewProp_Path,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UFileSystemOperation_FileExists_Statics::NewProp_ReturnValue,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UFileSystemOperation_FileExists_Statics::Function_MetaDataParams[] = {
		{ "Category", "File" },
		{ "ModuleRelativePath", "Public/FileSystemOperation.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UFileSystemOperation_FileExists_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UFileSystemOperation, nullptr, "FileExists", nullptr, nullptr, sizeof(FileSystemOperation_eventFileExists_Parms), Z_Construct_UFunction_UFileSystemOperation_FileExists_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UFileSystemOperation_FileExists_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04042401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UFileSystemOperation_FileExists_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UFileSystemOperation_FileExists_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UFileSystemOperation_FileExists()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UFileSystemOperation_FileExists_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UFileSystemOperation_FileMD5Hash_Statics
	{
		struct FileSystemOperation_eventFileMD5Hash_Parms
		{
			FString Path;
			FString ReturnValue;
		};
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_Path;
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_ReturnValue;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UFileSystemOperation_FileMD5Hash_Statics::NewProp_Path = { "Path", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(FileSystemOperation_eventFileMD5Hash_Parms, Path), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UFileSystemOperation_FileMD5Hash_Statics::NewProp_ReturnValue = { "ReturnValue", nullptr, (EPropertyFlags)0x0010000000000580, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(FileSystemOperation_eventFileMD5Hash_Parms, ReturnValue), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UFileSystemOperation_FileMD5Hash_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UFileSystemOperation_FileMD5Hash_Statics::NewProp_Path,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UFileSystemOperation_FileMD5Hash_Statics::NewProp_ReturnValue,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UFileSystemOperation_FileMD5Hash_Statics::Function_MetaDataParams[] = {
		{ "Category", "File" },
		{ "ModuleRelativePath", "Public/FileSystemOperation.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UFileSystemOperation_FileMD5Hash_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UFileSystemOperation, nullptr, "FileMD5Hash", nullptr, nullptr, sizeof(FileSystemOperation_eventFileMD5Hash_Parms), Z_Construct_UFunction_UFileSystemOperation_FileMD5Hash_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UFileSystemOperation_FileMD5Hash_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04042401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UFileSystemOperation_FileMD5Hash_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UFileSystemOperation_FileMD5Hash_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UFileSystemOperation_FileMD5Hash()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UFileSystemOperation_FileMD5Hash_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UFileSystemOperation_GetCurrentDirectory_Statics
	{
		struct FileSystemOperation_eventGetCurrentDirectory_Parms
		{
			FString ReturnValue;
		};
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_ReturnValue;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UFileSystemOperation_GetCurrentDirectory_Statics::NewProp_ReturnValue = { "ReturnValue", nullptr, (EPropertyFlags)0x0010000000000580, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(FileSystemOperation_eventGetCurrentDirectory_Parms, ReturnValue), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UFileSystemOperation_GetCurrentDirectory_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UFileSystemOperation_GetCurrentDirectory_Statics::NewProp_ReturnValue,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UFileSystemOperation_GetCurrentDirectory_Statics::Function_MetaDataParams[] = {
		{ "Category", "File" },
		{ "ModuleRelativePath", "Public/FileSystemOperation.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UFileSystemOperation_GetCurrentDirectory_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UFileSystemOperation, nullptr, "GetCurrentDirectory", nullptr, nullptr, sizeof(FileSystemOperation_eventGetCurrentDirectory_Parms), Z_Construct_UFunction_UFileSystemOperation_GetCurrentDirectory_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UFileSystemOperation_GetCurrentDirectory_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04042401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UFileSystemOperation_GetCurrentDirectory_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UFileSystemOperation_GetCurrentDirectory_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UFileSystemOperation_GetCurrentDirectory()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UFileSystemOperation_GetCurrentDirectory_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UFileSystemOperation_GetDirectories_Statics
	{
		struct FileSystemOperation_eventGetDirectories_Parms
		{
			FString Path;
			TArray<FString> ReturnValue;
		};
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_Path;
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_ReturnValue_Inner;
		static const UE4CodeGen_Private::FArrayPropertyParams NewProp_ReturnValue;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UFileSystemOperation_GetDirectories_Statics::NewProp_Path = { "Path", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(FileSystemOperation_eventGetDirectories_Parms, Path), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UFileSystemOperation_GetDirectories_Statics::NewProp_ReturnValue_Inner = { "ReturnValue", nullptr, (EPropertyFlags)0x0000000000000000, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, 0, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FArrayPropertyParams Z_Construct_UFunction_UFileSystemOperation_GetDirectories_Statics::NewProp_ReturnValue = { "ReturnValue", nullptr, (EPropertyFlags)0x0010000000000580, UE4CodeGen_Private::EPropertyGenFlags::Array, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(FileSystemOperation_eventGetDirectories_Parms, ReturnValue), EArrayPropertyFlags::None, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UFileSystemOperation_GetDirectories_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UFileSystemOperation_GetDirectories_Statics::NewProp_Path,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UFileSystemOperation_GetDirectories_Statics::NewProp_ReturnValue_Inner,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UFileSystemOperation_GetDirectories_Statics::NewProp_ReturnValue,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UFileSystemOperation_GetDirectories_Statics::Function_MetaDataParams[] = {
		{ "Category", "File" },
		{ "ModuleRelativePath", "Public/FileSystemOperation.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UFileSystemOperation_GetDirectories_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UFileSystemOperation, nullptr, "GetDirectories", nullptr, nullptr, sizeof(FileSystemOperation_eventGetDirectories_Parms), Z_Construct_UFunction_UFileSystemOperation_GetDirectories_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UFileSystemOperation_GetDirectories_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04042401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UFileSystemOperation_GetDirectories_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UFileSystemOperation_GetDirectories_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UFileSystemOperation_GetDirectories()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UFileSystemOperation_GetDirectories_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UFileSystemOperation_GetFiles_Statics
	{
		struct FileSystemOperation_eventGetFiles_Parms
		{
			FString Path;
			TArray<FString> ReturnValue;
		};
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_Path;
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_ReturnValue_Inner;
		static const UE4CodeGen_Private::FArrayPropertyParams NewProp_ReturnValue;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UFileSystemOperation_GetFiles_Statics::NewProp_Path = { "Path", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(FileSystemOperation_eventGetFiles_Parms, Path), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UFileSystemOperation_GetFiles_Statics::NewProp_ReturnValue_Inner = { "ReturnValue", nullptr, (EPropertyFlags)0x0000000000000000, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, 0, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FArrayPropertyParams Z_Construct_UFunction_UFileSystemOperation_GetFiles_Statics::NewProp_ReturnValue = { "ReturnValue", nullptr, (EPropertyFlags)0x0010000000000580, UE4CodeGen_Private::EPropertyGenFlags::Array, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(FileSystemOperation_eventGetFiles_Parms, ReturnValue), EArrayPropertyFlags::None, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UFileSystemOperation_GetFiles_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UFileSystemOperation_GetFiles_Statics::NewProp_Path,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UFileSystemOperation_GetFiles_Statics::NewProp_ReturnValue_Inner,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UFileSystemOperation_GetFiles_Statics::NewProp_ReturnValue,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UFileSystemOperation_GetFiles_Statics::Function_MetaDataParams[] = {
		{ "Category", "File" },
		{ "ModuleRelativePath", "Public/FileSystemOperation.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UFileSystemOperation_GetFiles_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UFileSystemOperation, nullptr, "GetFiles", nullptr, nullptr, sizeof(FileSystemOperation_eventGetFiles_Parms), Z_Construct_UFunction_UFileSystemOperation_GetFiles_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UFileSystemOperation_GetFiles_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04042401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UFileSystemOperation_GetFiles_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UFileSystemOperation_GetFiles_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UFileSystemOperation_GetFiles()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UFileSystemOperation_GetFiles_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UFileSystemOperation_PuertsNotifyChange_Statics
	{
		struct FileSystemOperation_eventPuertsNotifyChange_Parms
		{
			FString Path;
			FString Source;
		};
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_Path;
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_Source;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UFileSystemOperation_PuertsNotifyChange_Statics::NewProp_Path = { "Path", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(FileSystemOperation_eventPuertsNotifyChange_Parms, Path), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UFileSystemOperation_PuertsNotifyChange_Statics::NewProp_Source = { "Source", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(FileSystemOperation_eventPuertsNotifyChange_Parms, Source), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UFileSystemOperation_PuertsNotifyChange_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UFileSystemOperation_PuertsNotifyChange_Statics::NewProp_Path,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UFileSystemOperation_PuertsNotifyChange_Statics::NewProp_Source,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UFileSystemOperation_PuertsNotifyChange_Statics::Function_MetaDataParams[] = {
		{ "Category", "File" },
		{ "ModuleRelativePath", "Public/FileSystemOperation.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UFileSystemOperation_PuertsNotifyChange_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UFileSystemOperation, nullptr, "PuertsNotifyChange", nullptr, nullptr, sizeof(FileSystemOperation_eventPuertsNotifyChange_Parms), Z_Construct_UFunction_UFileSystemOperation_PuertsNotifyChange_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UFileSystemOperation_PuertsNotifyChange_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04042401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UFileSystemOperation_PuertsNotifyChange_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UFileSystemOperation_PuertsNotifyChange_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UFileSystemOperation_PuertsNotifyChange()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UFileSystemOperation_PuertsNotifyChange_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UFileSystemOperation_ReadFile_Statics
	{
		struct FileSystemOperation_eventReadFile_Parms
		{
			FString Path;
			FString Data;
			bool ReturnValue;
		};
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_Path;
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_Data;
		static void NewProp_ReturnValue_SetBit(void* Obj);
		static const UE4CodeGen_Private::FBoolPropertyParams NewProp_ReturnValue;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UFileSystemOperation_ReadFile_Statics::NewProp_Path = { "Path", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(FileSystemOperation_eventReadFile_Parms, Path), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UFileSystemOperation_ReadFile_Statics::NewProp_Data = { "Data", nullptr, (EPropertyFlags)0x0010000000000180, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(FileSystemOperation_eventReadFile_Parms, Data), METADATA_PARAMS(nullptr, 0) };
	void Z_Construct_UFunction_UFileSystemOperation_ReadFile_Statics::NewProp_ReturnValue_SetBit(void* Obj)
	{
		((FileSystemOperation_eventReadFile_Parms*)Obj)->ReturnValue = 1;
	}
	const UE4CodeGen_Private::FBoolPropertyParams Z_Construct_UFunction_UFileSystemOperation_ReadFile_Statics::NewProp_ReturnValue = { "ReturnValue", nullptr, (EPropertyFlags)0x0010000000000580, UE4CodeGen_Private::EPropertyGenFlags::Bool | UE4CodeGen_Private::EPropertyGenFlags::NativeBool, RF_Public|RF_Transient|RF_MarkAsNative, 1, sizeof(bool), sizeof(FileSystemOperation_eventReadFile_Parms), &Z_Construct_UFunction_UFileSystemOperation_ReadFile_Statics::NewProp_ReturnValue_SetBit, METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UFileSystemOperation_ReadFile_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UFileSystemOperation_ReadFile_Statics::NewProp_Path,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UFileSystemOperation_ReadFile_Statics::NewProp_Data,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UFileSystemOperation_ReadFile_Statics::NewProp_ReturnValue,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UFileSystemOperation_ReadFile_Statics::Function_MetaDataParams[] = {
		{ "Category", "File" },
		{ "ModuleRelativePath", "Public/FileSystemOperation.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UFileSystemOperation_ReadFile_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UFileSystemOperation, nullptr, "ReadFile", nullptr, nullptr, sizeof(FileSystemOperation_eventReadFile_Parms), Z_Construct_UFunction_UFileSystemOperation_ReadFile_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UFileSystemOperation_ReadFile_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04442401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UFileSystemOperation_ReadFile_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UFileSystemOperation_ReadFile_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UFileSystemOperation_ReadFile()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UFileSystemOperation_ReadFile_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UFileSystemOperation_ResolvePath_Statics
	{
		struct FileSystemOperation_eventResolvePath_Parms
		{
			FString Path;
			FString ReturnValue;
		};
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_Path;
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_ReturnValue;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UFileSystemOperation_ResolvePath_Statics::NewProp_Path = { "Path", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(FileSystemOperation_eventResolvePath_Parms, Path), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UFileSystemOperation_ResolvePath_Statics::NewProp_ReturnValue = { "ReturnValue", nullptr, (EPropertyFlags)0x0010000000000580, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(FileSystemOperation_eventResolvePath_Parms, ReturnValue), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UFileSystemOperation_ResolvePath_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UFileSystemOperation_ResolvePath_Statics::NewProp_Path,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UFileSystemOperation_ResolvePath_Statics::NewProp_ReturnValue,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UFileSystemOperation_ResolvePath_Statics::Function_MetaDataParams[] = {
		{ "Category", "File" },
		{ "ModuleRelativePath", "Public/FileSystemOperation.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UFileSystemOperation_ResolvePath_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UFileSystemOperation, nullptr, "ResolvePath", nullptr, nullptr, sizeof(FileSystemOperation_eventResolvePath_Parms), Z_Construct_UFunction_UFileSystemOperation_ResolvePath_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UFileSystemOperation_ResolvePath_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04042401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UFileSystemOperation_ResolvePath_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UFileSystemOperation_ResolvePath_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UFileSystemOperation_ResolvePath()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UFileSystemOperation_ResolvePath_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	struct Z_Construct_UFunction_UFileSystemOperation_WriteFile_Statics
	{
		struct FileSystemOperation_eventWriteFile_Parms
		{
			FString Path;
			FString Data;
		};
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_Path;
		static const UE4CodeGen_Private::FStrPropertyParams NewProp_Data;
		static const UE4CodeGen_Private::FPropertyParamsBase* const PropPointers[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Function_MetaDataParams[];
#endif
		static const UE4CodeGen_Private::FFunctionParams FuncParams;
	};
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UFileSystemOperation_WriteFile_Statics::NewProp_Path = { "Path", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(FileSystemOperation_eventWriteFile_Parms, Path), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FStrPropertyParams Z_Construct_UFunction_UFileSystemOperation_WriteFile_Statics::NewProp_Data = { "Data", nullptr, (EPropertyFlags)0x0010000000000080, UE4CodeGen_Private::EPropertyGenFlags::Str, RF_Public|RF_Transient|RF_MarkAsNative, 1, STRUCT_OFFSET(FileSystemOperation_eventWriteFile_Parms, Data), METADATA_PARAMS(nullptr, 0) };
	const UE4CodeGen_Private::FPropertyParamsBase* const Z_Construct_UFunction_UFileSystemOperation_WriteFile_Statics::PropPointers[] = {
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UFileSystemOperation_WriteFile_Statics::NewProp_Path,
		(const UE4CodeGen_Private::FPropertyParamsBase*)&Z_Construct_UFunction_UFileSystemOperation_WriteFile_Statics::NewProp_Data,
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UFunction_UFileSystemOperation_WriteFile_Statics::Function_MetaDataParams[] = {
		{ "Category", "File" },
		{ "ModuleRelativePath", "Public/FileSystemOperation.h" },
	};
#endif
	const UE4CodeGen_Private::FFunctionParams Z_Construct_UFunction_UFileSystemOperation_WriteFile_Statics::FuncParams = { (UObject*(*)())Z_Construct_UClass_UFileSystemOperation, nullptr, "WriteFile", nullptr, nullptr, sizeof(FileSystemOperation_eventWriteFile_Parms), Z_Construct_UFunction_UFileSystemOperation_WriteFile_Statics::PropPointers, UE_ARRAY_COUNT(Z_Construct_UFunction_UFileSystemOperation_WriteFile_Statics::PropPointers), RF_Public|RF_Transient|RF_MarkAsNative, (EFunctionFlags)0x04042401, 0, 0, METADATA_PARAMS(Z_Construct_UFunction_UFileSystemOperation_WriteFile_Statics::Function_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UFunction_UFileSystemOperation_WriteFile_Statics::Function_MetaDataParams)) };
	UFunction* Z_Construct_UFunction_UFileSystemOperation_WriteFile()
	{
		static UFunction* ReturnFunction = nullptr;
		if (!ReturnFunction)
		{
			UE4CodeGen_Private::ConstructUFunction(ReturnFunction, Z_Construct_UFunction_UFileSystemOperation_WriteFile_Statics::FuncParams);
		}
		return ReturnFunction;
	}
	UClass* Z_Construct_UClass_UFileSystemOperation_NoRegister()
	{
		return UFileSystemOperation::StaticClass();
	}
	struct Z_Construct_UClass_UFileSystemOperation_Statics
	{
		static UObject* (*const DependentSingletons[])();
		static const FClassFunctionLinkInfo FuncInfo[];
#if WITH_METADATA
		static const UE4CodeGen_Private::FMetaDataPairParam Class_MetaDataParams[];
#endif
		static const FCppClassTypeInfoStatic StaticCppClassTypeInfo;
		static const UE4CodeGen_Private::FClassParams ClassParams;
	};
	UObject* (*const Z_Construct_UClass_UFileSystemOperation_Statics::DependentSingletons[])() = {
		(UObject* (*)())Z_Construct_UClass_UBlueprintFunctionLibrary,
		(UObject* (*)())Z_Construct_UPackage__Script_PuertsEditor,
	};
	const FClassFunctionLinkInfo Z_Construct_UClass_UFileSystemOperation_Statics::FuncInfo[] = {
		{ &Z_Construct_UFunction_UFileSystemOperation_CreateDirectory, "CreateDirectory" }, // 2794591451
		{ &Z_Construct_UFunction_UFileSystemOperation_DirectoryExists, "DirectoryExists" }, // 415027838
		{ &Z_Construct_UFunction_UFileSystemOperation_FileExists, "FileExists" }, // 1646878541
		{ &Z_Construct_UFunction_UFileSystemOperation_FileMD5Hash, "FileMD5Hash" }, // 1967031895
		{ &Z_Construct_UFunction_UFileSystemOperation_GetCurrentDirectory, "GetCurrentDirectory" }, // 3645822137
		{ &Z_Construct_UFunction_UFileSystemOperation_GetDirectories, "GetDirectories" }, // 1075843593
		{ &Z_Construct_UFunction_UFileSystemOperation_GetFiles, "GetFiles" }, // 1558383437
		{ &Z_Construct_UFunction_UFileSystemOperation_PuertsNotifyChange, "PuertsNotifyChange" }, // 3226471977
		{ &Z_Construct_UFunction_UFileSystemOperation_ReadFile, "ReadFile" }, // 1398953178
		{ &Z_Construct_UFunction_UFileSystemOperation_ResolvePath, "ResolvePath" }, // 1581592443
		{ &Z_Construct_UFunction_UFileSystemOperation_WriteFile, "WriteFile" }, // 2471603734
	};
#if WITH_METADATA
	const UE4CodeGen_Private::FMetaDataPairParam Z_Construct_UClass_UFileSystemOperation_Statics::Class_MetaDataParams[] = {
		{ "Comment", "/**\n *\n */" },
		{ "IncludePath", "FileSystemOperation.h" },
		{ "ModuleRelativePath", "Public/FileSystemOperation.h" },
	};
#endif
	const FCppClassTypeInfoStatic Z_Construct_UClass_UFileSystemOperation_Statics::StaticCppClassTypeInfo = {
		TCppClassTypeTraits<UFileSystemOperation>::IsAbstract,
	};
	const UE4CodeGen_Private::FClassParams Z_Construct_UClass_UFileSystemOperation_Statics::ClassParams = {
		&UFileSystemOperation::StaticClass,
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
		METADATA_PARAMS(Z_Construct_UClass_UFileSystemOperation_Statics::Class_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UClass_UFileSystemOperation_Statics::Class_MetaDataParams))
	};
	UClass* Z_Construct_UClass_UFileSystemOperation()
	{
		static UClass* OuterClass = nullptr;
		if (!OuterClass)
		{
			UE4CodeGen_Private::ConstructUClass(OuterClass, Z_Construct_UClass_UFileSystemOperation_Statics::ClassParams);
		}
		return OuterClass;
	}
	IMPLEMENT_CLASS(UFileSystemOperation, 3025242183);
	template<> PUERTSEDITOR_API UClass* StaticClass<UFileSystemOperation>()
	{
		return UFileSystemOperation::StaticClass();
	}
	static FCompiledInDefer Z_CompiledInDefer_UClass_UFileSystemOperation(Z_Construct_UClass_UFileSystemOperation, &UFileSystemOperation::StaticClass, TEXT("/Script/PuertsEditor"), TEXT("UFileSystemOperation"), false, nullptr, nullptr, nullptr);
	DEFINE_VTABLE_PTR_HELPER_CTOR(UFileSystemOperation);
PRAGMA_ENABLE_DEPRECATION_WARNINGS
#ifdef _MSC_VER
#pragma warning (pop)
#endif
