module FSharp.Compiler.Service.Tests.CompletionEventsTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``CLIEvents.DefinedInAssemblies.Bug787438`` () =
    let info =
        Checker.getCompletionInfo
            """let mb = new MailboxProcessor<int>(fun _ -> ())
mb.{caret}"""

    assertHasItemWithNames [ "Error" ] info
    assertHasNoItemsWithNames [ "add_Error"; "remove_Error" ] info

[<Fact>]
let ``Event.NonStandard.PrefixMethods`` () =
    let info =
        Checker.getCompletionInfo
            """System.AppDomain.CurrentDomain.{caret}"""

    assertHasItemWithNames [ "add_AssemblyResolve"; "remove_AssemblyResolve"; "add_ReflectionOnlyAssemblyResolve"; "remove_ReflectionOnlyAssemblyResolve"; "add_ResourceResolve"; "remove_ResourceResolve"; "add_TypeResolve"; "remove_TypeResolve" ] info

[<Fact>]
let ``Event.NonStandard.VerifyLegitimateNameShowUp`` () =
    let info =
        Checker.getCompletionInfo
            """System.AppDomain.CurrentDomain.{caret}"""

    assertHasItemWithNames [ "AssemblyResolve"; "ReflectionOnlyAssemblyResolve"; "ResourceResolve"; "TypeResolve" ] info

[<Fact>]
let ``ReOpenNameSpace.StaticProperties`` () =
    let info =
        Checker.getCompletionInfo
            """
                // Static properties & events
                namespace A
                type TestType =
                  static member Prop = 0
                  static member Event = (new Event<int>()).Publish
                namespace B
                open A
                open A
                TestType.{caret}"""

    assertHasItemWithNames [ "Prop"; "Event" ] info
