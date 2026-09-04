module FSharp.Compiler.Service.Tests.UnlinkedEntityTests

open Xunit

open FSharp.Compiler.TypedTree

/// (#20269) An entity whose data was never linked has no contents. The IDE reaches these while a
/// file is mid-edit, so reading the contents must yield empty stand-ins rather than throwing a
/// NullReferenceException that takes down classification for the whole file.
[<Fact>]
let ``Unlinked entity exposes empty contents instead of throwing`` () =
    let entity = Entity.NewUnlinked()

    Assert.Empty(entity.ModuleOrNamespaceType.AllEntities)
    Assert.Empty(entity.ModuleOrNamespaceType.AllValsAndMembers)
    Assert.Empty(entity.TypeContents.tcaug_interfaces)

/// Reading the contents of an unlinked entity must not fill them in: the getters are hit from
/// parallel checking, and writing shared typed-tree state from a getter is a race.
[<Fact>]
let ``Reading the contents of an unlinked entity does not link them in`` () =
    let entity = Entity.NewUnlinked()

    entity.ModuleOrNamespaceType |> ignore
    entity.TypeContents |> ignore

    match entity.entity_modul_type with
    | null -> ()
    | _ -> failwith "reading ModuleOrNamespaceType must not write the entity's contents"

    match entity.entity_tycon_tcaug with
    | null -> ()
    | _ -> failwith "reading TypeContents must not write the entity's augmentation"
