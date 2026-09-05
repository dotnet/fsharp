module FSharp.Compiler.Service.Tests.UnlinkedEntityTests

open System

open Xunit

open FSharp.Compiler.TypedTree

/// (#20269) A placeholder that unpickling never linked is not a readable entity — its
/// representation, attributes, augmentation and contents are all absent, so no stand-in for a
/// subset of them makes it safe. Resilience lives at the IDE read boundary instead.
[<Fact>]
let ``An unlinked entity is not linked and not readable`` () =
    let entity = Entity.NewUnlinked()

    Assert.False entity.IsLinked

    Assert.Throws<NullReferenceException>(fun () -> entity.ModuleOrNamespaceType |> ignore)
    |> ignore
