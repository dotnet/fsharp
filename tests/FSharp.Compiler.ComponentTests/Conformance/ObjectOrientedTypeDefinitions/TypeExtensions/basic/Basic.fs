// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.ObjectOrientedTypeDefinitions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module TypeExtensionsBasic =

    // Error tests

    [<Theory; FileInlineData("E_ProtectedMemberInExtensionMember01.fs")>]
    let ``E_ProtectedMemberInExtensionMember01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 491

    [<Theory; FileInlineData("E_CantExtendTypeAbbrev.fs")>]
    let ``E_CantExtendTypeAbbrev_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 964

    [<Theory; FileInlineData("E_ConflictingMembers.fs")>]
    let ``E_ConflictingMembers_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 438

    [<Theory; FileInlineData("E_InvalidExtensions01.fs")>]
    let ``E_InvalidExtensions01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 912

    [<Theory; FileInlineData("E_InvalidExtensions02.fs")>]
    let ``E_InvalidExtensions02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 912

    [<Theory; FileInlineData("E_InvalidExtensions03.fs")>]
    let ``E_InvalidExtensions03_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 925

    [<Theory; FileInlineData("E_InvalidExtensions04.fs")>]
    let ``E_InvalidExtensions04_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 39

    [<Theory; FileInlineData("E_ExtensionInNamespace01.fs")>]
    let ``E_ExtensionInNamespace01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 644

    [<Theory; FileInlineData("E_ExtendVirtualMethods01.fs")>]
    let ``E_ExtendVirtualMethods01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 854

    [<Theory; FileInlineData("E_InvalidForwardRef01.fs")>]
    let ``E_InvalidForwardRef01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 430

    [<Theory; FileInlineData("E_ExtensionOperator01.fs")>]
    let ``E_ExtensionOperator01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1215

    // Success tests

    [<Theory; FileInlineData("BasicExtensions.fs")>]
    let ``BasicExtensions_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("MultipleExtensions.fs")>]
    let ``MultipleExtensions_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("UnqualifiedName.fs")>]
    let ``UnqualifiedName_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    // Tests temporarily skipped due to 'allows ref struct' constraint mismatch on IEnumerable
    // [<Theory; FileInlineData("ExtendHierarchy01.fs")>]
    // let ``ExtendHierarchy01_fs`` compilation =
    //     compilation
    //     |> getCompilation
    //     |> asExe
    //     |> withLangVersionPreview
    //     |> ignoreWarnings
    //     |> compile
    //     |> shouldSucceed

    [<Theory; FileInlineData("ExtendHierarchy02.fs")>]
    let ``ExtendHierarchy02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withLangVersionPreview
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("ExtensionInNamespace02.fs")>]
    let ``ExtensionInNamespace02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("ExtendWithOperator01.fs")>]
    let ``ExtendWithOperator01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("NonConflictingIntrinsicMembers.fs")>]
    let ``NonConflictingIntrinsicMembers_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("ExtendViaOverloading01.fs")>]
    let ``ExtendViaOverloading01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withLangVersionPreview
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    // Tests temporarily skipped due to 'allows ref struct' constraint mismatch on IEnumerable
    // [<Theory; FileInlineData("ExtendViaOverloading02.fs")>]
    // let ``ExtendViaOverloading02_fs`` compilation =
    //     compilation
    //     |> getCompilation
    //     |> asExe
    //     |> withLangVersionPreview
    //     |> ignoreWarnings
    //     |> compile
    //     |> shouldSucceed

    [<Theory; FileInlineData("fslib.fs")>]
    let ``fslib_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    // ========== Optional Extension Tests (cross-assembly) ==========

    /// Optional extensions can contain conflicting members as long as they don't get used together
    [<Fact>]
    let ``typeext_opt007 - Conflicting extension members in different modules`` () =
        FSharp """
namespace NS
   module M = 
    type Lib() = class end
     
    type Lib with
      member x.ExtensionMember () = 1

    module N =
      type Lib with
        member x.ExtensionMember () = 2
  
   module F =
    let mutable res = true
    open M
  
    let a = new Lib()
    if not (a.ExtensionMember () = 1) then
      res <- false

    (if (res) then 0 else 1) |> exit
        """
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    /// Regression test for FSHARP1.0:3593
    /// "Prefer extension members that have been brought into scope by more recent 'open' statements"
    [<Fact>]
    let ``typeext_opt008 - Recent open wins for extension resolution`` () =
        FSharp """
namespace NS
  // Type defined at namespace level so both modules can see it
  type Lib() = class end
  
  module M = 
    type Lib with
        member x.ExtensionMember () = 1

  module N =
    type Lib with
        member x.ExtensionMember () = 2
  
  module F =
    open M
    open N    // <-- last open wins
  
    let a = new Lib()
    let b = a.ExtensionMember()

    (if b = 2 then 0 else 1) |> exit
        """
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    /// Method overrides are not permitted in optional extensions
    [<Fact>]
    let ``E_CrossModule01 - Override not permitted in extension`` () =
        FSharp """
module M = 
    type R() = class end
module U =
   open M
   type R with override x.ToString() = "hi"
 
open M
open U
let x = R()
printfn "%A" (x.ToString())
        """
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 854

    /// Verify that optional extension must be inside a module
    [<Fact>]
    let ``E_NotInModule - Extension in namespace not allowed`` () =
        FSharp """
namespace NS
    type Lib() = class end
namespace NS
    type Lib with
    
    // Extension Methods
          member x.ExtensionMember () = 1
  
    module F =
      exit 1
        """
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 644
