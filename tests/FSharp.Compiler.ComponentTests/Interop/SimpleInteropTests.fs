// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Interop

open Xunit
open FSharp.Test.Compiler

module ``Simple interop verification`` =

    [<Fact(Skip = "TODO: This is broken RN, since netcoreapp30 is used for C# and 3.1 for F#, should be fixed as part of https://github.com/dotnet/fsharp/issues/9740")>]
    let ``Instantiate C# type from F#`` () =

        let CSLib =
            CSharp """
public class A { }
      """   |> withName "CSLib"

        let FSLib =
             FSharp """
module AMaker
let makeA () : A = A()
         """ |> withName "FSLib" |> withReferences [CSLib]

        let app =
            FSharp """
module ReferenceCSfromFS
let a = AMaker.makeA()
        """ |> withReferences [CSLib; FSLib]

        app
        |> compile
        |> shouldSucceed


    [<Fact(Skip = "TODO: This is broken RN, since netcoreapp30 is used for C# and 3.1 for F#, should be fixed as part of https://github.com/dotnet/fsharp/issues/9740")>]
    let ``Instantiate F# type from C#`` () =
        let FSLib =
            FSharp """
namespace Interop.FS
type Bicycle(manufacturer: string) =
    member this.Manufacturer = manufacturer
        """ |> withName "FSLib"

        let app =
            CSharp """
using Interop.FS;
public class BicycleShop {
    public Bicycle[] cycles;
}
        """ |> withReferences [FSLib]

        app
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Instantiate F# type from C# fails without import`` () =
        let FSLib =
            FSharp """
namespace Interop.FS
type Bicycle(manufacturer: string) =
    member this.Manufacturer = manufacturer
        """ |> withName "FSLib"

        let app =
            CSharp """
public class BicycleShop {
    public Bicycle[] cycles;
}
        """ |> withReferences [FSLib]

        app
        |> compile
        |> shouldFail

    [<Fact>]
    let ``can't mutably set a C#-const field in F#`` () =
        let csLib =
            CSharp """
public static class Holder {
    public const string Label = "label";
}
            """
            |> withName "CsharpConst"

        let fsLib =
            FSharp """
module CannotSetCSharpConst
Holder.Label <- "nope"
            """
            |> withReferences [csLib]

        fsLib
        |> compile
        |> shouldFail

    [<Fact>]
    let ``can't mutably set a C#-readonly field in F#`` () =
        let csLib =
            CSharp """
public static class Holder {
    public static readonly string Label = "label";
}
            """
            |> withName "CsharpReadonly"

        let fsLib =
            FSharp """
module CannotSetCSharpReadonly
Holder.Label <- "nope"
            """
            |> withReferences [csLib]

        fsLib
        |> compile
        |> shouldFail

    // https://github.com/dotnet/fsharp/issues/13519
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Issue 13519 - C# method with optional parameters can be called from F# without specifying defaults`` () =
        let csLib =
            CSharp
                """
using System;

namespace CSharpLib
{
    public class MyClass
    {
        public string DoSomething(string required, string optional1 = "default1", string optional2 = "default2")
        {
            return required + "|" + optional1 + "|" + optional2;
        }
    }
}
            """
            |> withName "CSharpOptionalParams"

        let fsApp =
            FSharp
                """
module TestApp

open CSharpLib

[<EntryPoint>]
let main _ =
    let c = MyClass()
    let result = c.DoSomething("hello")
    if result <> "hello|default1|default2" then
        failwithf "Expected 'hello|default1|default2' but got '%s'" result
    let result2 = c.DoSomething("hello", "custom1")
    if result2 <> "hello|custom1|default2" then
        failwithf "Expected 'hello|custom1|default2' but got '%s'" result2
    0
                """
            |> withReferences [ csLib ]

        fsApp
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed

    // https://github.com/dotnet/fsharp/issues/13519
    // Specifically testing the intersection of optional parameters and ParamArray
    // as identified by Don Syme: https://github.com/dotnet/fsharp/issues/13519#issuecomment-1253808416
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Issue 13519 - C# method with optional parameters and ParamArray can be called from F# without specifying defaults`` () =
        let csLib =
            CSharp
                """
using System;

namespace CSharpParamArrayLib
{
    public class Assertions
    {
        public string BeEquivalentTo(string expected, string because = "", params object[] becauseArgs)
        {
            string formatted = because;
            if (becauseArgs != null && becauseArgs.Length > 0)
            {
                formatted = string.Format(because, becauseArgs);
            }
            return expected + "|" + formatted + "|" + becauseArgs.Length;
        }
    }
}
                """
            |> withName "CSharpParamArrayLib"

        let fsApp =
            FSharp
                """
module TestParamArrayApp

open CSharpParamArrayLib

[<EntryPoint>]
let main _ =
    let a = Assertions()

    // Call with only the required argument - omitting both optional and params
    let result1 = a.BeEquivalentTo("test")
    if result1 <> "test||0" then
        failwithf "Test 1 failed: Expected 'test||0' but got '%s'" result1

    // Call with required + optional, omitting params
    let result2 = a.BeEquivalentTo("test", "because {0}")
    if result2 <> "test|because {0}|0" then
        failwithf "Test 2 failed: Expected 'test|because {0}|0' but got '%s'" result2

    // Call with all arguments including params values
    let result3 = a.BeEquivalentTo("test", "because {0}", "reason")
    if result3 <> "test|because reason|1" then
        failwithf "Test 3 failed: Expected 'test|because reason|1' but got '%s'" result3

    0
                """
            |> withReferences [ csLib ]

        fsApp
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed
