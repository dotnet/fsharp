// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module CodeGenRegressions_TypeDefs =

    // https://github.com/dotnet/fsharp/issues/16565
    [<Fact>]
    let ``Issue_16565_DefaultAugmentationFalseDuplicateEntry`` () =
        let source = """
module Test

open System

[<DefaultAugmentation(false)>]
type Option<'T> =
    | Some of Value: 'T
    | None

    member x.Value =
        match x with
        | Some x -> x
        | None -> raise (new InvalidOperationException("Option.Value"))

    static member None : Option<'T> = None

and 'T option = Option<'T>

let v = Option.Some 42
printfn "Value: %d" v.Value
let n = Option<int>.None
printfn "None created successfully"
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/14321
    [<Fact>]
    let ``Issue_14321_DuAndIWSAMNames`` () =
        let source = """
module Test

#nowarn "3535"  // IWSAM warning

type EngineError<'e> =
    static abstract Overheated : 'e
    static abstract LowOil : 'e

type CarError =
    | Overheated
    | LowOil
    | DeviceNotPaired

    interface EngineError<CarError> with
        static member Overheated = Overheated
        static member LowOil = LowOil
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/14321
    // Runtime test: type must load without "duplicate entry in method table"
    [<Fact>]
    let ``Issue_14321_DuAndIWSAMNames_Runtime`` () =
        let source = """
module Test

#nowarn "3535"

type EngineError<'e> =
    static abstract Overheated : 'e
    static abstract LowOil : 'e

type CarError =
    | Overheated
    | LowOil
    | DeviceNotPaired

    interface EngineError<CarError> with
        static member Overheated = Overheated
        static member LowOil = LowOil

[<EntryPoint>]
let main _ =
    let err = CarError.Overheated
    match err with
    | Overheated -> printfn "Got Overheated"
    | LowOil -> printfn "Got LowOil"
    | DeviceNotPaired -> printfn "Got DeviceNotPaired"
    0
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/5834
    [<Fact>]
    let ``Issue_5834_EventSpecialname`` () =
        let source = """
module Test

open System
open System.Reflection

type IAbstract1 =
    [<CLIEvent>]
    abstract member Event : IEvent<EventHandler, EventArgs>

type IAbstract2 =
    [<CLIEvent>]
    abstract member Event : IDelegateEvent<EventHandler>

[<AbstractClass>]
type Abstract3() =
    [<CLIEvent>]
    abstract member Event : IDelegateEvent<EventHandler>

type Concrete1() =
    let event = new Event<EventHandler, EventArgs>()
    [<CLIEvent>]
    member this.Event = event.Publish

type Concrete2() =
    [<CLIEvent>]
    member this.Event = { new IDelegateEvent<EventHandler> with
                              member this.AddHandler _ = ()
                              member this.RemoveHandler _ = () }

type ConcreteWithObsolete() =
    let evt = new Event<EventHandler, EventArgs>()
    [<Obsolete("deprecated")>]
    [<CLIEvent>]
    member this.MyEvent = evt.Publish

[<EntryPoint>]
let main _ =
    let mutable failures = 0
    let check (t: Type) =
        t.GetMethods(BindingFlags.Public ||| BindingFlags.Instance ||| BindingFlags.DeclaredOnly)
        |> Array.filter (fun m -> m.Name.Contains("Event"))
        |> Array.iter (fun m ->
            if not m.IsSpecialName then
                printfn "FAIL: %s.%s missing specialname" t.Name m.Name
                failures <- failures + 1)

    check typeof<IAbstract1>
    check typeof<IAbstract2>
    check typeof<Abstract3>
    check typeof<Concrete1>
    check typeof<Concrete2>
    check typeof<ConcreteWithObsolete>

    if failures > 0 then
        failwithf "BUG: %d event accessors missing specialname" failures
    printfn "SUCCESS: All event accessors have specialname"
    0
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/5834
    // IL verification: abstract event accessors must have specialname flag
    [<Fact>]
    let ``Issue_5834_EventSpecialname_IL`` () =
        let source = """
module Test

open System

type IAbstract1 =
    [<CLIEvent>]
    abstract member Event : IEvent<EventHandler, EventArgs>

[<AbstractClass>]
type Abstract2() =
    [<CLIEvent>]
    abstract member Event : IDelegateEvent<EventHandler>
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> verifyIL [
            // Interface abstract event accessors have specialname
            """.method public hidebysig specialname abstract virtual instance void  add_Event(class [runtime]System.EventHandler A_1) cil managed"""

            """.method public hidebysig specialname abstract virtual instance void  remove_Event(class [runtime]System.EventHandler A_1) cil managed"""

            // IAbstract1 event references its accessors
            """.addon instance void Test/IAbstract1::add_Event(class [runtime]System.EventHandler)"""
            """.removeon instance void Test/IAbstract1::remove_Event(class [runtime]System.EventHandler)"""

            // Abstract2 event also references its specialname accessors
            """.addon instance void Test/Abstract2::add_Event(class [runtime]System.EventHandler)"""
            """.removeon instance void Test/Abstract2::remove_Event(class [runtime]System.EventHandler)"""
        ]
        |> ignore

    // https://github.com/dotnet/fsharp/issues/14321
    // IL verification: DU case properties and IWSAM implementations coexist without duplicates
    [<Fact>]
    let ``Issue_14321_DuAndIWSAMNames_IL`` () =
        let source = """
module Test

#nowarn "3535"

type EngineError<'e> =
    static abstract Overheated : 'e
    static abstract LowOil : 'e

type CarError =
    | Overheated
    | LowOil
    | DeviceNotPaired

    interface EngineError<CarError> with
        static member Overheated = Overheated
        static member LowOil = LowOil
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> verifyIL [
            // DU case property getter exists on CarError
            """.method public static class Test/CarError get_Overheated() cil managed"""

            // Explicit IWSAM implementation uses mangled name
            """.method public hidebysig specialname static class Test/CarError  'Test.EngineError<Test.CarError>.get_Overheated'() cil managed"""

            // Explicit IWSAM for LowOil also present
            """.method public hidebysig specialname static class Test/CarError  'Test.EngineError<Test.CarError>.get_LowOil'() cil managed"""
        ]
        |> ignore

    // https://github.com/dotnet/fsharp/issues/16565
    // IL verification: DefaultAugmentation(false) produces no duplicate method table entries
    [<Fact>]
    let ``Issue_16565_DefaultAugmentationFalseDuplicateEntry_IL`` () =
        let source = """
module Test

[<DefaultAugmentation(false)>]
type MyOption<'T> =
    | Some of Value: 'T
    | None

    member x.Value =
        match x with
        | Some x -> x
        | None -> failwith "no value"

    static member None : MyOption<'T> = None
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> verifyIL [
            // The user-defined get_Value method exists on the main type with specialname
            """.method public hidebysig specialname instance !T  get_Value() cil managed"""

            // The static get_None method exists
            """.method public static class Test/MyOption`1<!T> get_None() cil managed"""
        ]
        |> ignore

