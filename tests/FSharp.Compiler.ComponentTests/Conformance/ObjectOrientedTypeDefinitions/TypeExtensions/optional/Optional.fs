// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.ObjectOrientedTypeDefinitions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

/// Tests for optional type extensions (cross-assembly extension members)
module TypeExtensionsOptional =

    // ========== Helper Libraries ==========

    /// lib000 - Basic type with public fields for extension testing
    let lib000 =
        FSharp """
namespace NS
    type Lib() =
      class
        [<DefaultValue>]
        val mutable instanceField : int
        static let mutable staticField = 0
        member x.Name () = "Lib"
        member x.DefProp = 1
     end
 
    type LibGen<'a>() =
      class
        [<DefaultValue(false)>]
        val mutable instanceField : 'a 
        static let mutable staticField = Unchecked.defaultof<'a>
        member x.Name ()  = "LibGen" 
        member x.DefProp = Unchecked.defaultof<'a>
     end
        """
        |> withName "lib000"

    /// lib001 - Basic type with private fields
    let lib001 =
        FSharp """
namespace NS
    type Lib() =
      class
        [<DefaultValue>]
        val mutable private instanceField : int
        [<DefaultValue>]
        static val mutable private staticField : int
        member x.Name () = "Lib"
        member x.DefProp = 1
     end
 
    type LibGen<'a>() =
      class
        [<DefaultValue(false)>]
        val mutable private instanceField : 'a 
        [<DefaultValue(false)>]
        static val mutable private staticField : 'a
        member x.Name ()  = "LibGen" 
        member x.DefProp = Unchecked.defaultof<'a>
     end
        """
        |> withName "lib001"

    /// lib003 - Type with interface implementation
    let lib003 =
        FSharp """
namespace NS
    type IM = 
      interface
        abstract M : int -> int
      end
      
    type Lib() =
      class
        interface IM with
          member x.M i = 0
     end
        """
        |> withName "lib003"

    /// lib004 - Type with member M
    let lib004 =
        FSharp """
namespace NS
    type Lib() =
      class
        member x.M i:int = 0
     end
        """
        |> withName "lib004"

    /// lib005 - Type with member M returning 1
    let lib005 =
        FSharp """
namespace NS
    type Lib() =
      class
        member x.M i:int = 1
     end
        """
        |> withName "lib005"

    // ========== Success Tests ==========

    /// typeext_opt001 - Verify that types from a dll can be extended
    [<Fact>]
    let ``typeext_opt001 - Types from dll can be extended`` () =
        FSharp """
namespace NS
  module M = 
    type Lib with
    
    // Extension Methods
          member x.ExtensionMember () = 1
          static member StaticExtensionMember() = 1
          
    // Extension Properties
          member x.ExtensionProperty001 = 1
          member x.ExtensionProperty002 with get() = 2
          member x.ExtensionProperty003 with get() = 3
          member x.ExtensionProperty003 with set(i:int) = ()
          member x.ExtensionProperty004 
            with get () = x.instanceField
            and set (inp : int) = x.instanceField <- inp             
          member x.ExtensionIndexer001 
            with get (i:int, j:int) = 1
            and set (i:int, j:int, value:int) = ()

          static member StaticExtensionProperty001 = 11
          static member StaticExtensionProperty002 with get() = 12
          static member StaticExtensionProperty003 with get() = 13
          static member StaticExtensionProperty003 with set(i:int) = ()
          static member StaticExtensionIndexer001 
            with get (i:int, j:int) = 1
            and set (i:int, j:int, value:int) = ()

  module N =
    type LibGen<'a> with
    // Extension Methods
          member x.ExtensionMember () = Unchecked.defaultof<'a>
          static member StaticExtensionMember() = 1
          
    // Extension Properties
          member x.ExtensionProperty001 = 1
          member x.ExtensionProperty002 with get() = 2
          member x.ExtensionProperty003 with get() = 3
          member x.ExtensionProperty003 with set(i:int) = ()
          member x.ExtensionProperty004 
            with get () = x.instanceField
            and set (inp : 'a) = x.instanceField <- inp             
          member x.ExtensionIndexer001 
            with get (i:int, j:int) = 1
            and set (i:int, j:int, value:int) = ()
              
          static member StaticExtensionProperty001 = 11
          static member StaticExtensionProperty002 with get() = 12
          static member StaticExtensionProperty003 with get() = 13
          static member StaticExtensionProperty003 with set(i:int) = ()
          static member StaticExtensionIndexer001 
            with get (i:int, j:int) = 1
            and set (i:int, j:int, value:int) = ()
  
  module F =
    open M
    let mutable res = true
  
    let a = new Lib()
    if not (a.ExtensionProperty001 = 1) then
      printf "Lib.ExtensionProperty001 failed\n"
      res <- false

    (if (res) then 0 else 1) |> exit
        """
        |> withReferences [lib000]
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    /// typeext_opt003 - Verify that hiding an interface implementation is allowed
    [<Fact>]
    let ``typeext_opt003 - Hiding interface implementation allowed`` () =
        FSharp """
namespace NS
  module M = 
    type Lib with
          member x.M i = i
    
  module F =
    open M
    let mutable res = true
    
    let a = new Lib()
    if not (a.M 1 = 1) then
      printf "Lib.TypeExt_opt003.fs failed\n"
      res <- false

    let b = a :> IM
    if not (b.M 1 = 0) then
      printf "Lib.TypeExt_opt003.fs failed\n"
      res <- false
      
    (if (res) then 0 else 1) |> exit
        """
        |> withReferences [lib003]
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    /// typeext_opt004 - Verify that hiding a member is allowed
    [<Fact>]
    let ``typeext_opt004 - Hiding a member allowed`` () =
        FSharp """
namespace NS
  module M = 
    type Lib with
      member x.M i = i
  
  module F =
    open M
    let mutable res = true
    
    let a = new Lib()
    if not (a.M 1 = 1) then
      printf "Lib.TypeExt_opt004.fs failed\n"
      res <- false
    
    (if (res) then 0 else 1) |> exit
        """
        |> withReferences [lib004]
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    /// typeext_opt005 - Verify that overloading a member is allowed
    [<Fact>]
    let ``typeext_opt005 - Overloading a member allowed`` () =
        FSharp """
namespace NS
  module M = 
    type Lib with
      member x.M(i,j) = i + j
    
  module F =
    open M
    let mutable res = true
    
    let a = new Lib()
    if not (a.M(2, 3) + (a.M 1) = 6) then
      printf "Lib.TypeExt_opt005.fs failed\n"
      res <- false
      
    (if (res) then 0 else 1) |> exit
        """
        |> withReferences [lib005]
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    /// typeext_opt007 - Optional extensions can contain conflicting members as long as they don't get used together
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

    /// typeext_opt008 - Regression test for FSHARP1.0:3593 - Prefer extension members that have been brought into scope by more recent 'open' statements
    [<Fact>]
    let ``typeext_opt008 - Recent open wins for extension resolution`` () =
        FSharp """
namespace NS
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

    /// ShortNamesAllowed - Verify that type being optional extended may use a short name identifier (Bug FSharp 1.0:3720)
    [<Fact>]
    let ``ShortNamesAllowed - Short name identifier in extension`` () =
        FSharp """
namespace OE
  open NS
  module M =
    type Lib with
          member x.ExtensionMember () = 1
  
  module F =
    open M
    let mutable res = true
    
    let a = new Lib()
    if not (a.ExtensionMember () = 1) then
      printf "Lib.ExtensionMember failed\n"
      res <- false
      
    (if (res) then 0 else 1) |> exit
        """
        |> withReferences [lib000]
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    // ========== Error Tests ==========

    /// E_typeext_opt005 - Error when trying to use member overloading when some overloads are specified using curried arguments
    [<Fact>]
    let ``E_typeext_opt005 - Curried arguments error`` () =
        FSharp """
namespace NS
  module M = 
    type Lib with
      member x.M i j = i + j
    
  module F =
    open M
    let mutable res = true
    
    let a = new Lib()
    if not (a.M (2, 3) + (a.M 1) = 3) then
      printf "Lib.TypeExt_opt004.fs failed\n"
      res <- false
        """
        |> withReferences [lib005]
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 816

    /// E_NotInModule - Verify that optional extension must be inside a module
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
        |> compile
        |> shouldFail
        |> withErrorCode 644

    /// E_ModuleNotOpen - Verify that optional extensions are only in scope if the module containing the extension has been opened
    [<Fact>]
    let ``E_ModuleNotOpen - Extension not visible without open`` () =
        FSharp """
namespace OE
  open NS
  module M =
    type NS.Lib with
    
    // Extension Methods
          member x.ExtensionMember () = 1
  
namespace Test
  open NS
  module N =
    let a = new Lib()
    let b = a.ExtensionMember ()
  
    exit 1
        """
        |> withReferences [lib000]
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 39

    /// E_PrivateFields - Verify that private fields cannot be accessed by optional extension members
    [<Fact>]
    let ``E_PrivateFields - Private fields not accessible`` () =
        FSharp """
namespace NS
  module M = 
    type Lib with
    
    // Extension Methods
          member x.ExtensionMember () = 1
          static member StaticExtensionMember() = 1
          
    // Extension Properties - trying to access private fields should fail
          member x.ExtensionProperty004 
            with get () = x.instanceField
            and set (inp : int) = x.instanceField <- inp             
        """
        |> withReferences [lib001]
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 1096

    /// E_CrossModule01 - Method overrides are not permitted in optional extensions
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
        |> compile
        |> shouldFail
        |> withErrorCode 854
