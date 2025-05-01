// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module Tests.ServiceAnalysis.UnusedOpens

open System
open Xunit
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text

let private filePath = "C:\\test.fs"

let private projectOptions : FSharpProjectOptions = 
    { ProjectFileName = "C:\\test.fsproj"
      ProjectId = None
      SourceFiles =  [| filePath |]
      ReferencedProjects = [| |]
      OtherOptions = [| |]
      IsIncompleteTypeCheckEnvironment = true
      UseScriptResolutionRules = false
      LoadTime = DateTime.MaxValue
      OriginalLoadReferences = []
      UnresolvedReferences = None
      Stamp = None }

let private checker = FSharpChecker.Create()

let (=>) (source: string) (expectedRanges: ((*line*)int * ((*start column*)int * (*end column*)int)) list) =
    let sourceLines = source.Split ([|"\r\n"; "\n"; "\r"|], StringSplitOptions.None)

    let _, checkFileAnswer = checker.ParseAndCheckFileInProject(filePath, 0, FSharp.Compiler.Text.SourceText.ofString source, projectOptions) |> Async.RunSynchronously
    
    let checkFileResults =
        match checkFileAnswer with
        | FSharpCheckFileAnswer.Aborted -> failwithf "ParseAndCheckFileInProject aborted"
        | FSharpCheckFileAnswer.Succeeded(checkFileResults) -> checkFileResults

    let unusedOpenRanges = UnusedOpens.getUnusedOpens (checkFileResults, fun lineNum -> sourceLines.[Line.toZ lineNum]) |> Async.RunSynchronously
    
    unusedOpenRanges 
    |> List.map (fun x -> x.StartLine, (x.StartColumn, x.EndColumn))
    |> fun actual -> Assert.Equal<(int * (int * int)) list>(expectedRanges, actual)

[<Fact>]
let ``unused open declaration in top level module``() =
    """
module TopModule
open System
open System.IO
let _ = DateTime.Now
"""
    => [ 4, (5, 14) ]

[<Fact>]
let ``unused open declaration in namespace``() =
    """
namespace TopNamespace
open System
open System.IO
module Nested =
    let _ = DateTime.Now
"""
    => [ 4, (5, 14) ]

[<Fact>]
let ``unused open declaration in nested module``() =
    """
namespace TopNamespace
module Nested =
    open System
    open System.IO
    let _ = DateTime.Now
"""
    => [ 5, (9, 18) ]

[<Fact>] 
let ``unused open declaration due to partially qualified symbol``() =
    """
module TopModule
open System
open System.IO
let _ = IO.File.Create ""
"""
    => [ 4, (5, 14) ]

[<Fact>]
let ``unused parent open declaration due to partially qualified symbol``() =
    """
module TopModule
open System
open System.IO
let _ = File.Create ""
"""
    => [ 3, (5, 11) ]

[<Fact>]
let ``open statement duplication in parent module is unused``() =
    """
module TopModule
open System.IO
module Nested =
    open System.IO
    let _ = File.Create ""
"""
    => [ 5, (9, 18) ]

[<Fact>]
let ``open statement duplication in parent module is marked as unused even though it seems to be used in its scope``() =
    """
module TopModule
open System.IO
module Nested =
    open System.IO
    let _ = File.Create ""
let _ = File.Create ""
"""
    => [ 5, (9, 18) ]

[<Fact>]
let ``multiple open declaration in the same line``() =
    """
open System.IO; let _ = File.Create "";; open System.IO
"""
    => [ 2, (46, 55) ]

[<Fact>]
let ``open a nested module inside another one is not unused``() =
    """
module Top
module M1 =
    let x = ()
module M2 =
    open M1
    let y = x
"""
    => []

[<Fact>]
let ``open a nested module inside another one is not unused, complex hierarchy``() =
    """
module Top =
    module M1 =
        module M11 =
            let x = ()
    module M2 =
        module M22 =
            open M1.M11
            let y = x
"""
    => []

[<Fact>]
let ``open a nested module inside another one is not unused, even more complex hierarchy``() =
    """
module Top =
    module M1 =
        module M11 =
            module M111 =
                module M1111 =
                    let x = ()
    module M2 =
        module M22 =
            open M1.M11.M111.M1111
                let y = x
"""
    => []

[<Fact>]
let ``opening auto open module after it's parent module was opened should be marked as unused``() =
    """
module NormalModule =
    [<AutoOpen>]
    module AutoOpenModule1 =
        module NestedNormalModule =
            [<AutoOpen>]
            module AutoOpenModule2 =
                [<AutoOpen>]
                module AutoOpenModule3 =
                    type Class() = class end

open NormalModule.AutoOpenModule1.NestedNormalModule
open NormalModule.AutoOpenModule1.NestedNormalModule.AutoOpenModule2
let _ = Class()
"""
    => [ 13, (5, 68) ]

[<Fact>]
let ``opening parent module after one of its auto open module was opened should be marked as unused``() =
    """
module NormalModule =
    [<AutoOpen>]
    module AutoOpenModule1 =
        module NestedNormalModule =
            [<AutoOpen>]
            module AutoOpenModule2 =
                [<AutoOpen>]
                module AutoOpenModule3 =
                    type Class() = class end

open NormalModule.AutoOpenModule1.NestedNormalModule.AutoOpenModule2
open NormalModule.AutoOpenModule1.NestedNormalModule
let _ = Class()
"""
    => [ 13, (5, 52) ]
    
[<Fact>]
let ``open declaration is not marked as unused if there is a shortened attribute symbol from it``() =
    """
open System
[<Serializable>]
type Class() = class end
"""
    => []
    
[<Fact>]
let ``open declaration is not marked as unused if an extension property is used``() =
    """
module Module =
    type System.String with
        member _.ExtensionProperty = ()
open Module
let _ = "a long string".ExtensionProperty
"""
    => []

[<Fact>]
let ``open declaration is marked as unused if an extension property is not used``() =
    """
module Module =
    type System.String with
        member _.ExtensionProperty = ()
open Module
let _ = "a long string".Trim()
"""
    => [ 5, (5, 11) ]

[<Fact>]
let ``open declaration is not marked as unused if an extension method is used``() =
    """
type Class() = class end
module Module =
    type Class with
        member _.ExtensionMethod() = ()
open Module
let x = Class()
let _ = x.ExtensionMethod()
"""
    => []

[<Fact>]
let ``open declaration is marked as unused if an extension method is not used``() =
    """
type Class() = class end
module Module =
    type Class with
        member _.ExtensionMethod() = ()
open Module
let x = Class()
"""
    => [ 6, (5, 11) ]

[<Fact>]
let ``open declaration is not marked as unused if one of its types is used in a constructor signature``() =
    """
module M =
    type Class() = class end
open M
type Site (x: Class -> unit) = class end
"""
    => []   

[<Fact>]
let ``open declaration is marked as unused if nothing from it is used``() =
    """
module M =
    type Class() = class end
open M
type Site (x: int -> unit) = class end
"""
    => [ 4, (5, 6) ]

[<Fact>]
let ``static extension method applied to a type results that both namespaces /where the type is declared and where the extension is declared/ is not marked as unused``() =
    """
module Extensions =
    type System.DateTime with
        static member ExtensionMethod() = ()
open System
open Extensions
let _ = DateTime.ExtensionMethod
"""
    => []
    
[<Fact>]
let ``static extension property applied to a type results that both namespaces /where the type is declared and where the extension is declared/ is not marked as unused``() =
    """
module Extensions =
    type System.DateTime with
        static member ExtensionProperty = ()
open System
open Extensions
let _ = DateTime.ExtensionProperty
"""
    => []

[<Fact>]
let ``accessing property on a variable should not force the namespace in which the type is declared to be marked as used``() =
    """
let dt = System.DateTime.Now
module M =
    open System
    let _ = dt.Hour
"""
    => [4, (9, 15) ]

[<Fact>]
let ``either of two open declarations are not marked as unused if symbols from both of them are used``() =
    """
module M1 =
    module M2 =
        let func1 _ = ()
        module M3 =
            let func2 _ = ()
open M1.M2.M3
open M1.M2
let _ = func1()
let _ = func2()
"""
    => []
        
[<Fact>]
let ``open module with ModuleSuffix attribute value applied is not marked as unused if a symbol declared in it is used``() =
    """
[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module M =
    let func _ = ()
open M
let _ = func()
"""
    => []

[<Fact>]
let ``open module all of which symbols are used by qualifier is marked as unused``() =
    """
module M =
    let func _ = ()
open M
let _ = M.func 1
"""
    => [4, (5, 6) ]

[<Fact>]
let ``open module is not marked as unused if a symbol defined in it is used in OCaml-style type annotation``() =
    """
module M =
    type Class() = class end
open M
let func (arg: Class list) = ()
"""
    => []

[<Fact>]
let ``auto open module``() =
    """
module Top =
    [<AutoOpen>]
    module M =
        let func _ = ()
open Top
let _ = func()
"""
    => []

[<Fact>]
let ``auto open module with namespace``() =
    """
namespace Module1Namespace
[<AutoOpen>]
module Module1 =
    module Module2 =
        let x = 1
namespace ConsumerNamespace
open Module1Namespace
module Module3 =
            let y = Module2.x
"""
    => []

[<Fact>]
let ``auto open module in the middle of hierarchy``() =
    """
namespace Ns
module M1 =
    [<AutoOpen>]
    module MA1 = 
        let func _ = ()
open M1
module M2 =
    let _ = func()
"""
    => []

[<Fact>]
let ``open declaration is not marked as unused if a delegate defined in it is used``() =
    """
open System
let _ = Func<int, int>(fun _ -> 1)
"""
    => []

[<Fact>]
let ``open declaration is not marked as unused if a unit of measure defined in it is used``() =
    """
module M = 
    type [<Measure>] m
module N =
    open M
    let _ = 1<m>
"""
    => []

[<Fact>]
let ``open declaration is not marked as unused if an attribute defined in it is applied on an interface member argument``() =
    """
open System.Runtime.InteropServices
type T = abstract M: [<DefaultParameterValue(null)>] ?x: int -> unit
"""
    => []

[<Fact>]
let ``relative module open declaration``() =
    """
module Top =
    module Nested = 
        let x = 1
open Top
open Nested
let _ = x
"""
    => []

[<Fact>]
let ``open declaration is used if a symbol defined in it is used in a module top-level do expression``() =
    """
module Top
open System.IO
File.ReadAllLines ""
|> ignore
"""
    => []

[<Fact>]
let ``redundant opening a module with ModuleSuffix attribute value is marks as unused``() =
    """
[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module InternalModuleWithSuffix =
    let func1 _ = ()
module M =
    open InternalModuleWithSuffix
    let _ = InternalModuleWithSuffix.func1()
"""
    => [ 6, (9, 33) ]
    
[<Fact>]
let ``redundant opening a module is marks as unused``() =
    """
module InternalModuleWithSuffix =
    let func1 _ = ()
module M =
    open InternalModuleWithSuffix
    let _ = InternalModuleWithSuffix.func1()
"""
    => [ 5, (9, 33) ]

[<Fact>]
let ``usage of an unqualified union case doesn't make an opening module where it's defined to be marked as unused``() =
    """
module M =
    type DU = Case1
open M
let _ = Case1
"""
    => []

[<Fact>]
let ``usage of qualified union case doesn't make an opening module where it's defined to be marked as unused``() =
    """
module M =
    type DU = Case1
open M
let _ = DU.Case1
"""
    => []

[<Fact>]
let ``type with different DisplayName``() =
    """
open Microsoft.FSharp.Quotations
let _ = Expr.Coerce (<@@ 1 @@>, typeof<int>)
"""
    => []

[<Fact>]
let ``auto open module with ModuleSuffix attribute value``() =
    """
module Top =
    [<AutoOpen; CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
    module Module =
        let func _ = ()
open Top
module Module1 =
    let _ = func()
"""
    => []

[<Fact>]
let ``a type which has more than one DisplayName causes the namespace it's defined in to be not marked as unused``() =
    """
open System
let _ = IntPtr.Zero
""" 
    => []

[<Fact>]
let ``usage of an operator makes the module it's defined in to be not marked as unused``() =
    """
module M =
    let (++|) x y = ()
open M
let _ = 1 ++| 2
"""
    => []

[<Fact>]
let ``usage of an operator makes the module /with Module suffix/ it's defined in to be not marked as unused``() =
    """
[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module M =
    let (++|) x y = ()
open M
let _ = 1 ++| 2
"""
    => []

[<Fact>]
let ``type used in pattern matching with "as" keyword causes the module in which the type is defined to be not marked as unused``() =
    """
module M = 
    type Class() = class end
open M
let _ = match obj() with 
        | :? Class as c -> ()
        | _ -> ()
"""
    => []

[<Fact>]
let ``a function from printf family prevents Printf module from marking as unused``() =
    """
open Microsoft.FSharp.Core.Printf
open System.Text
let _ = bprintf (StringBuilder()) "%A" 1
"""
    => []

[<Fact>]
let ``assembly level attribute prevents namespace in which it's defined to be marked as unused``() =
    """
open System
[<assembly: Version("1")>]
()
"""
    => []

[<Fact>]
let ``open declaration is not marked as unused if a related type extension is used``() =
    """
module Module =
    open System
    type String with
        member _.Method() = ()
"""
    => []

[<Fact>]
let ``open declaration is not marked as unused if a symbol defined in it is used in type do block``() =
    """
open System.IO.Compression

type OutliningHint() as self =
    do self.E.Add (fun (e: GZipStream) -> ()) 
    member _.E: IEvent<_> = Unchecked.defaultof<_> 
"""
    => []

[<Fact>]
let ``should not mark open declaration with global prefix``() =
    """
module Module =
    open global.System
    let _ = String("")
"""
    => []

[<Fact>]
let ``record fields should be taken into account``() = 
    """
module M1 =
    type Record = { Field: int }
module M2 =
    open M1
    let x = { Field = 0 }
"""
    => []

[<Fact>]
let ``handle type alias``() = 
    """
module TypeAlias =
    type MyInt = int
module Usage =
    open TypeAlias
    let f (x:MyInt) = x
"""
    => []

[<Fact>]
let ``handle override members``() = 
    """
type IInterface =
    abstract Property: int

type IClass() =
    interface IInterface with
        member _.Property = 0

let f (x: IClass) = (x :> IInterface).Property
"""
    => []

[<Fact>]
let ``active pattern cases should be taken into account``() =
    """
module M = 
    let (|Pattern|_|) _ = Some()
open M
let f (Pattern _) = ()
"""
    => []

[<Fact>]
let ``active patterns applied as a function should be taken into account``() =
    """
module M = 
    let (|Pattern|_|) _ = Some()
open M
let _ = (|Pattern|_|) ()
"""
    => []

[<Fact>]
let ``not used active pattern does not make the module in which it's defined to not mark as unused``() =
    """
module M = 
    let (|Pattern|_|) _ = Some()
open M
let _ = 1
"""
    => [ 4, (5, 6) ]
    
[<Fact>]
let ``type in type parameter constraint should be taken into account``() =
    """
open System
let f (x: 'a when 'a :> IDisposable) = ()
"""
    => []

[<Fact>]
let ``namespace declaration should never be marked as unused``() =
    """
namespace Library2
type T() = class end
"""
    => []

[<Fact>]
let ``auto open module opened before enclosing one is handled correctly``() =
    """
module M =
    let x = 1
    [<AutoOpen>]
    module N =
        let y = 2
open M.N
open M
let _ = x
let _ = y
"""
    => []

[<Fact>]
let ``single relative open declaration opens two independent modules in different parent modules``() =
    """
module M =
    module Xxx =
        let x = 1
module N =
    module Xxx =
        let y = 1
open M
open N
open N.Xxx
open Xxx

let _ = y
let _ = x
"""
    => []

[<Fact>]
let ``C# extension methods are taken into account``() =
    """
open System.Linq

module Test =
    let xs = []
    let _ = xs.ToList()
""" 
    => []      

[<Fact>]
let ``namespace which contains types with C# extension methods is marked as unused if no extension is used``() =
    """
open System.Linq

module Test =
    let xs = []
""" 
    => [ 2, (5, 16) ]      

[<Fact>]
let ``a type from an auto open module is taken into account``() =
    """
module M1 =
    [<AutoOpen>]
    module AutoOpened =
        type T() = class end

module M2 =
    open M1
    let _ = T()
"""
    => []

[<Fact>]
let ``unused open declaration in top level rec module``() =
    """
module rec TopModule
open System
open System.IO
let _ = DateTime.Now
"""
    => [ 4, (5, 14) ]

[<Fact>]
let ``unused open declaration in rec namespace``() =
    """
namespace rec TopNamespace
open System
open System.IO
module Nested =
    let _ = DateTime.Now
"""
    => [ 4, (5, 14) ]

[<Fact>]
let ``unused inner module open declaration in rec module``() =
    """
module rec TopModule

module Nested =
    let x = 1
    let f x = x
    type T() = class end
    type R = { F: int }

open Nested
"""
    => [ 10, (5, 11) ]

[<Fact>]
let ``used inner module open declaration in rec module``() =
    """
module rec TopModule

module Nested =
    let x = 1
    let f x = x
    type T() = class end
    type R = { F: int }

open Nested

let _ = f 1
"""
    => []

[<Fact>]
let ``used open C# type``() =
    """
open type System.Console

WriteLine("Hello World")
    """
    => []
    
[<Fact>]
let ``unused open C# type``() =
    """
open type System.Console
    
printfn "%s" "Hello World"
    """
    => [2, (10, 24)]
    
[<Fact>]
let ``used open type from module``() =
    """
module MyModule =
    type Thingy =
        static member Thing = ()
    
open type MyModule.Thingy

printfn "%A" Thing
    """
    => []
        
[<Fact>]
let ``unused open type from module``() =
    """
module MyModule =
    type Thingy =
        static member Thing = ()
    
open type MyModule.Thingy

printfn "%A" MyModule.Thingy.Thing
    """
    => [6, (10, 25)]

