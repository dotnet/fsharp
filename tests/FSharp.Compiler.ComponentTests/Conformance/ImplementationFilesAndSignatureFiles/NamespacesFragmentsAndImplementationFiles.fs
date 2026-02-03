// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.ImplementationFilesAndSignatureFiles

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open TestFramework

/// Tests for namespaces, fragments, and implementation files - basic scenarios.
/// Migrated from tests/fsharpqa/Source/Conformance/ImplementationFilesAndSignatureFiles/NamespacesFragmentsAndImplementationFiles/basic/
module NamespacesBasic =

    let private resourcePath = __SOURCE_DIRECTORY__ ++ ".." ++ ".." ++ "resources" ++ "tests" ++ "Conformance" ++ "ImplementationFilesAndSignatureFiles" ++ "NamespacesFragmentsAndImplementationFiles" ++ "basic"

    // SOURCE=E_NamespaceAndModule01.fs SCFLAGS=--test:ErrorRanges
    // <Expects status="error" span="(9,1-9,1)" id="FS0010">Incomplete structured construct at or before this point in definition
    [<Fact>]
    let ``E_NamespaceAndModule01 - cannot have both namespace and module`` () =
        FsFromPath (resourcePath ++ "E_NamespaceAndModule01.fs")
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Incomplete structured construct"
        |> ignore

    // SOURCE="E_NamespaceAndModule02.fsi E_NamespaceAndModule02.fs" SCFLAGS=--test:ErrorRanges
    // <Expects status="error" span="(9,1-9,1)" id="FS0010">Incomplete structured construct at or before this point in signature file
    [<Fact>]
    let ``E_NamespaceAndModule02 - cannot have both namespace and module in signature`` () =
        FsFromPath (resourcePath ++ "E_NamespaceAndModule02.fsi")
        |> withAdditionalSourceFile (SourceFromPath (resourcePath ++ "E_NamespaceAndModule02.fs"))
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Incomplete structured construct"
        |> ignore

    // SOURCE="E_AnonModule01.fs AnonModule01Main.fs"
    // <Expects status="error" span="(6,1)" id="FS0222">Files in libraries or multiple-file applications must begin with a namespace or module declaration
    [<Fact>]
    let ``E_AnonModule01 - anonymous module in multi-file application error`` () =
        FsFromPath (resourcePath ++ "E_AnonModule01.fs")
        |> withAdditionalSourceFile (SourceFromPath (resourcePath ++ "AnonModule01Main.fs"))
        |> compile
        |> shouldFail
        |> withErrorCode 0222
        |> withDiagnosticMessageMatches "Files in libraries or multiple-file applications must begin with a namespace or module declaration"
        |> ignore

    // SOURCE=hashdirectives01.fs
    // <Expects id="FS0988" status="warning">Main module of program is empty
    [<Fact>]
    let ``hashdirectives01 - empty file with hash directives gives warning`` () =
        FsFromPath (resourcePath ++ "hashdirectives01.fs")
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnosticMessageMatches "Main module of program is empty"
        |> ignore

    // SOURCE=hashdirectives02.fs
    // <Expects id="FS0988" status="warning">Main module of program is empty
    [<Fact>]
    let ``hashdirectives02 - file with only nowarn gives warning`` () =
        FsFromPath (resourcePath ++ "hashdirectives02.fs")
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnosticMessageMatches "Main module of program is empty"
        |> ignore

    // SOURCE=HashDirectives03.fs
    [<Fact>]
    let ``HashDirectives03 - hash directives with code`` () =
        FsFromPath (resourcePath ++ "HashDirectives03.fs")
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> ignore

    // SOURCE=E_HashDirectives01.fs
    // <Expects id="FS0530" status="error" span="(7,1)">Only '#' compiler directives may occur prior to the first 'namespace' declaration
    [<Fact>]
    let ``E_HashDirectives01 - code before namespace gives error`` () =
        FsFromPath (resourcePath ++ "E_HashDirectives01.fs")
        |> compile
        |> shouldFail
        |> withErrorCode 0530
        |> withDiagnosticMessageMatches "Only '#' compiler directives may occur prior to the first 'namespace' declaration"
        |> ignore

    // SOURCE=E_HashDirectives02.fs
    // <Expects id="FS0530" status="error" span="(7,1)">Only '#' compiler directives may occur prior to the first 'namespace' declaration
    [<Fact>]
    let ``E_HashDirectives02 - code before namespace gives error`` () =
        FsFromPath (resourcePath ++ "E_HashDirectives02.fs")
        |> compile
        |> shouldFail
        |> withErrorCode 0530
        |> withDiagnosticMessageMatches "Only '#' compiler directives may occur prior to the first 'namespace' declaration"
        |> ignore

    // SOURCE=E_NamespaceCollision01.fs
    // <Expects id="FS0249" status="error" span="(13,6)">Two type definitions named 'DU' occur in namespace
    [<Fact>]
    let ``E_NamespaceCollision01 - duplicate type in same namespace`` () =
        FsFromPath (resourcePath ++ "E_NamespaceCollision01.fs")
        |> compile
        |> shouldFail
        |> withErrorCode 0249
        |> withDiagnosticMessageMatches "Two type definitions named 'DU' occur in namespace"
        |> ignore

    // SOURCE=E_ModuleCollision01.fs
    // <Expects id="FS0037" status="error" span="(13,1)">Duplicate definition of type, exception or module 'B'
    [<Fact>]
    let ``E_ModuleCollision01 - duplicate module definition`` () =
        FsFromPath (resourcePath ++ "E_ModuleCollision01.fs")
        |> compile
        |> shouldFail
        |> withErrorCode 0037
        |> withDiagnosticMessageMatches "Duplicate definition of type, exception or module 'B'"
        |> ignore

    // SOURCE=E_NamespaceModuleCollision01.fs
    // <Expects id="FS0247" status="error" span="(14,13)">A namespace and a module named 'A.B' both occur
    [<Fact>]
    let ``E_NamespaceModuleCollision01 - namespace and module collision`` () =
        FsFromPath (resourcePath ++ "E_NamespaceModuleCollision01.fs")
        |> compile
        |> shouldFail
        |> withErrorCode 0247
        |> withDiagnosticMessageMatches "A namespace and a module named"
        |> ignore

    // SOURCE="LastFileExeCanBeAnona.fs LastFileExeCanBeAnonb.fs"
    // <Expects status="notin">namespace</Expects> - should compile without warnings about namespace/module
    [<Fact>]
    let ``LastFileExeCanBeAnon - last file in exe can be anonymous`` () =
        FsFromPath (resourcePath ++ "LastFileExeCanBeAnona.fs")
        |> withAdditionalSourceFile (SourceFromPath (resourcePath ++ "LastFileExeCanBeAnonb.fs"))
        |> asExe
        |> compile
        |> shouldSucceed
        |> ignore

    // SOURCE=NoWarningForOneCompiland.fs
    // <Expects status="notin">namespace</Expects> - should compile without warnings
    [<Fact>]
    let ``NoWarningForOneCompiland - single file compile no warning`` () =
        FsFromPath (resourcePath ++ "NoWarningForOneCompiland.fs")
        |> asExe
        |> compile
        |> shouldSucceed
        |> ignore

    // SOURCE="NoWarningWithModNs01a.fs NoWarningWithModNs01b.fs"
    // <Expects status="notin">namespace</Expects> - should compile without warnings
    [<Fact>]
    let ``NoWarningWithModNS01 - files with namespace and module`` () =
        FsFromPath (resourcePath ++ "NoWarningWithModNS01a.fs")
        |> withAdditionalSourceFile (SourceFromPath (resourcePath ++ "NoWarningWithModNS01b.fs"))
        |> compile
        |> shouldSucceed
        |> ignore

    // SOURCE="E_LastFileDllCantBeAnona.fs E_LastFileDllCantBeAnonb.fs" SCFLAGS=-a
    // <Expects status="error" span="(7,1)" id="FS0222"> on first file (anonymous in library)
    // <Expects id="FS0191" status="warning"> on second file
    [<Fact>]
    let ``E_LastFileDllCantBeAnon - anonymous file in library gives error`` () =
        FsFromPath (resourcePath ++ "E_LastFileDllCantBeAnona.fs")
        |> withAdditionalSourceFile (SourceFromPath (resourcePath ++ "E_LastFileDllCantBeAnonb.fs"))
        |> asLibrary
        |> compile
        |> shouldFail
        |> withErrorCode 0222
        |> ignore

    // SOURCE="E_NoNamespaceModuleDec01a.fs E_NoNamespaceModuleDec01b.fs"
    // <Expects status="error" span="(4,1)" id="FS0222">
    [<Fact>]
    let ``E_NoNamespaceModuleDec01 - missing namespace or module in multi-file`` () =
        FsFromPath (resourcePath ++ "E_NoNamespaceModuleDec01a.fs")
        |> withAdditionalSourceFile (SourceFromPath (resourcePath ++ "E_NoNamespaceModuleDec01b.fs"))
        |> compile
        |> shouldFail
        |> withErrorCode 0222
        |> ignore

    // SOURCE="E_NoNamespaceModuleDec02a.fs E_NoNamespaceModuleDec02b.fs"
    // <Expects status="error" span="(7,1)" id="FS0222">...When using a module declaration at the start of a file the '=' sign is not allowed
    [<Fact>]
    let ``E_NoNamespaceModuleDec02 - nested module syntax at top level`` () =
        FsFromPath (resourcePath ++ "E_NoNamespaceModuleDec02a.fs")
        |> withAdditionalSourceFile (SourceFromPath (resourcePath ++ "E_NoNamespaceModuleDec02b.fs"))
        |> compile
        |> shouldFail
        |> withErrorCode 0222
        |> withDiagnosticMessageMatches "'=' sign is not allowed"
        |> ignore


/// Tests for 'global' namespace usage.
/// Migrated from tests/fsharpqa/Source/Conformance/ImplementationFilesAndSignatureFiles/NamespacesFragmentsAndImplementationFiles/global/
module NamespacesGlobal =

    let private resourcePath = __SOURCE_DIRECTORY__ ++ ".." ++ ".." ++ "resources" ++ "tests" ++ "Conformance" ++ "ImplementationFilesAndSignatureFiles" ++ "NamespacesFragmentsAndImplementationFiles" ++ "global"

    // SOURCE=MiscNegativeTests.fs SCFLAGS="--test:ErrorRanges"
    // <Expects status="error" id="FS0039" span="(11,20-11,26)">The type 'string' is not defined
    // <Expects status="error" id="FS0039" span="(17,16-17,23)">The value, namespace, type or module 'Array2D' is not defined
    [<Fact>]
    let ``MiscNegativeTests - global keyword misuse`` () =
        FsFromPath (resourcePath ++ "MiscNegativeTests.fs")
        |> withOptions ["--test:ErrorRanges"]
        |> asExe
        |> compile
        |> shouldFail
        |> withErrorCode 0039
        |> ignore

    // SOURCE=AsPrefix.fsx SCFLAGS="--test:ErrorRanges"
    // <Expects status="success">
    [<Fact>]
    let ``AsPrefix - global as prefix in qualified name`` () =
        FsxFromPath (resourcePath ++ "AsPrefix.fsx")
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed
        |> ignore

    // SOURCE=E_Abbreviation.fsx SCFLAGS="--test:ErrorRanges"
    // <Expects status="error" id="FS0883" span="(6,18-6,24)">Invalid namespace, module, type or union case name
    // <Expects status="error" id="FS0053" span="(6,18-6,24)">Discriminated union cases and exception labels must be uppercase identifiers
    [<Fact>]
    let ``E_Abbreviation - global cannot be abbreviated`` () =
        FsxFromPath (resourcePath ++ "E_Abbreviation.fsx")
        |> withLangVersion80
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 0883
        |> withDiagnosticMessageMatches "Invalid namespace, module, type or union case name"
        |> ignore

    // SOURCE=E_AsATypeInFunctionDecl.fsx SCFLAGS="--test:ErrorRanges"
    // <Expects status="error" id="FS1126" span="(10,12-10,18)">'global' may only be used as the first name in a qualified path
    [<Fact>]
    let ``E_AsATypeInFunctionDecl - global cannot be type`` () =
        FsxFromPath (resourcePath ++ "E_AsATypeInFunctionDecl.fsx")
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 1126
        |> withDiagnosticMessageMatches "'global' may only be used as the first name in a qualified path"
        |> ignore

    // SOURCE=E_AsModuleName.fsx SCFLAGS="--test:ErrorRanges"
    // <Expects status="error" span="(6,8-6,14)" id="FS0244">Invalid module or namespace name
    [<Fact>]
    let ``E_AsModuleName - global cannot be module name`` () =
        FsxFromPath (resourcePath ++ "E_AsModuleName.fsx")
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 0244
        |> withDiagnosticMessageMatches "Invalid module or namespace name"
        |> ignore

    // SOURCE=E_AsType.fsx SCFLAGS="--test:ErrorRanges"
    // <Expects status="error" id="FS0883" span="(13,6-13,12)">Invalid namespace, module, type or union case name
    // <Expects status="error" id="FS1126" span="(13,6-13,12)">'global' may only be used as the first name in a qualified path
    [<Fact>]
    let ``E_AsType - global cannot be type name`` () =
        FsxFromPath (resourcePath ++ "E_AsType.fsx")
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 0883
        |> ignore

    // SOURCE=E_InAttribute.fsx SCFLAGS="--test:ErrorRanges"
    // <Expects status="error" id="FS1126" span="(7,19-7,25)">'global' may only be used as the first name in a qualified path
    // <Expects status="error" id="FS0267" span="(7,19-7,25)">This is not a valid constant expression or custom attribute value
    [<Fact>]
    let ``E_InAttribute - global in attribute value`` () =
        FsxFromPath (resourcePath ++ "E_InAttribute.fsx")
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 1126
        |> ignore

    // SOURCE=E_InExceptionDecl.fsx SCFLAGS="--test:ErrorRanges"
    // <Expects status="error" id="FS0010" span="(7,11-7,17)">Unexpected keyword 'global' in exception definition
    [<Fact>]
    let ``E_InExceptionDecl - global in exception definition`` () =
        FsxFromPath (resourcePath ++ "E_InExceptionDecl.fsx")
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected keyword 'global' in exception definition"
        |> ignore

    // SOURCE=E_InOpen.fsx SCFLAGS="--test:ErrorRanges"
    // <Expects status="error" id="FS1126" span="(10,6-10,12)">'global' may only be used as the first name in a qualified path
    [<Fact>]
    let ``E_InOpen - cannot open global by itself`` () =
        FsxFromPath (resourcePath ++ "E_InOpen.fsx")
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 1126
        |> ignore

    // SOURCE=E_IsAKeyword.fsx SCFLAGS="--test:ErrorRanges"
    // <Expects status="error" id="FS0010" span="(6,12-6,13)">Unexpected symbol '=' in binding
    [<Fact>]
    let ``E_IsAKeyword - global is keyword cannot be bound`` () =
        FsxFromPath (resourcePath ++ "E_IsAKeyword.fsx")
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected symbol '=' in binding"
        |> ignore

    // SOURCE=InNamespaceByItself.fsx SCFLAGS="--test:ErrorRanges"
    // <Expects status="success">
    [<Fact>]
    let ``InNamespaceByItself - namespace global is valid`` () =
        FsxFromPath (resourcePath ++ "InNamespaceByItself.fsx")
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed
        |> ignore

    // NOTE: FSharpImportCSharp.fs requires C# compilation (PRECMD="\$CSC_PIPE /t:library CSharpDll.cs")
    // This test is migrated using the C# interop pattern from ComponentTests
    [<Fact>]
    let ``FSharpImportCSharp - F# imports C# type hidden by local type`` () =
        let csLib =
            CSharp """
public class C
{
    public static int M()
    {
        return 1;
    }
}
"""
            |> withName "CSharpDll"

        FsFromPath (resourcePath ++ "FSharpImportCSharp.fs")
        |> withReferences [csLib]
        |> asExe
        |> compile
        |> shouldSucceed
        |> ignore
