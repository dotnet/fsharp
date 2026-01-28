// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.ImplementationFilesAndSignatureFiles

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open TestFramework

/// Tests for signature files without corresponding implementation files.
/// Migrated from tests/fsharpqa/Source/Conformance/ImplementationFilesAndSignatureFiles/SignatureFiles/
module SignatureFiles =

    let private resourcePath = __SOURCE_DIRECTORY__ ++ ".." ++ ".." ++ "resources" ++ "tests" ++ "Conformance" ++ "ImplementationFilesAndSignatureFiles" ++ "SignatureFiles"

    // SOURCE=E_MissingSourceFile01.fsi
    // <Expects id="FS0240" status="error" span="(6,1)">The signature file 'E_MissingSourceFile01' does not have a corresponding implementation file
    [<Fact>]
    let ``E_MissingSourceFile01 - fsi without fs gives error`` () =
        FsFromPath (resourcePath ++ "E_MissingSourceFile01.fsi")
        |> compile
        |> shouldFail
        |> withErrorCode 0240
        |> withDiagnosticMessageMatches "signature file.*does not have a corresponding implementation file"
        |> ignore

    // SOURCE=E_MissingSourceFile02.fsi
    // <Expects status="error" id="FS0010" span="(47,23)">Unexpected keyword 'lazy' in signature file
    [<Fact>]
    let ``E_MissingSourceFile02 - deprecated lazy syntax in fsi`` () =
        FsFromPath (resourcePath ++ "E_MissingSourceFile02.fsi")
        |> compile
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected keyword 'lazy' in signature file"
        |> ignore
