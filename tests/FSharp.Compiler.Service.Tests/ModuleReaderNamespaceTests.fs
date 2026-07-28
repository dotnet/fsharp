module FSharp.Compiler.Service.Tests.ModuleReaderNamespaceTests

open System.Collections.Generic
open System.Reflection
open System.Text
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Test.Compiler
open FSharp.Test.Assert
open Xunit

// These tests exercise how ILPreTypeDef / ILPreNamespace are created when reading .NET metadata: the
// goal is lazy namespace reading, where only imported namespaces realise their pre-type-defs.
//
// Roslyn groups emitted types by namespace and doesn't preserve source order across namespaces, so C#
// can't produce a genuinely "split" namespace (e.g. Ns1, Ns2, Ns1). C# is therefore used only for the
// realistic-shape test (siblings sorted); split-namespace order is asserted with synthetic arrays.

/// Render the namespace/type tree using the structural API.
let private dumpTree (sortSiblings: bool) (typeDefs: ILTypeDefs) : string =
    let sb = StringBuilder()

    let rec go (indent: int) (tds: ILTypeDefs) =
        let pad = String.replicate indent "  "

        let typeNames =
            [ for struct (_, pre) in tds.AsArrayOfPreTypeDefs() do
                // Skip the always-present <Module> pseudo-type.
                if pre.Name <> "<Module>" then yield pre.Name ]
        for name in (if sortSiblings then List.sort typeNames else typeNames) do
            sb.AppendLine($"{pad}{name}") |> ignore

        let namespaces = List.ofArray (tds.AsArrayOfPreNamespaces())
        let namespaces = if sortSiblings then List.sortBy (fun (ns: ILPreNamespace) -> ns.Name) namespaces else namespaces
        for ns in namespaces do
            sb.AppendLine($"{pad}{ns.Name}/") |> ignore
            go (indent + 1) (ns.GetContents())

    sb.AppendLine("global") |> ignore
    go 1 typeDefs
    sb.ToString().Replace("\r\n", "\n").TrimEnd('\n')


// ---- Synthetic pre-type-defs (control the exact metadata order) --------------------------------

/// A pre-type-def carrying only its simple name; the namespace lives in the containing table.
let private mkPreTypeDef (name: string) : ILPreTypeDef =
    { new ILPreTypeDef with
        member _.Name = name
        member _.GetTypeDef() =
            ILTypeDef(name, TypeAttributes.Public, ILTypeDefLayout.Auto, [], [], None,
                mkILMethods [], mkILTypeDefs [], mkILFields [], emptyILMethodImpls, mkILEvents [],
                mkILProperties [], emptyILSecurityDecls, emptyILCustomAttrsStored) }

/// A flat-table entry from a full type name: its namespace paired with the pre-type-def (all types
/// sit at one level).
let private flatEntry (fullName: string) : struct (string list * ILPreTypeDef) =
    let ns, name = splitILTypeName fullName
    struct (ns, mkPreTypeDef name)

/// Group the given full type names (in order) into a namespace tree using the production grouping.
let private mkGroupedTypeDefs (fullNames: string list) : ILTypeDefs =
    mkILTypeDefsGroupedComputed
        (fun () -> [| for n in fullNames -> let ns, name = splitILTypeName n in struct (ns, name) |])
        mkPreTypeDef

/// Wrap the type defs so that forcing any namespace's contents records its full path in `forced`.
let private trackNamespaceForcing (forced: HashSet<string>) (typeDefs: ILTypeDefs) : ILTypeDefs =
    let rec track (path: string) (tds: ILTypeDefs) =
        mkILTypeDefsAndNamespacesComputed
            (fun () -> tds.AsArrayOfPreTypeDefs())
            (fun () ->
                [| for ns in tds.AsArrayOfPreNamespaces() ->
                       let nsPath = if path = "" then ns.Name else $"{path}.{ns.Name}"
                       mkILPreNamespaceComputed(ns.Name, fun () ->
                           forced.Add nsPath |> ignore
                           track nsPath (ns.GetContents())) |])

    track "" typeDefs


[<Fact>]
let ``Grouping - split namespace preserves metadata order`` () =
    // Ns1 is split in the metadata: Ns1.T1, then Ns2.T2, then back to Ns1.T3.
    let typeDefs = mkGroupedTypeDefs [ "Ns1.T1"; "Ns2.T2"; "Ns1.T3" ]

    // Ns1 must come before Ns2 (first-seen), and within Ns1 the members keep order: T1 then T3.
    dumpTree false typeDefs |> shouldEqual (
        "global\n" +
        "  Ns1/\n" +
        "    T1\n" +
        "    T3\n" +
        "  Ns2/\n" +
        "    T2"
    )


[<Fact>]
let ``Grouping - nested namespaces and global types`` () =
    let typeDefs =
        mkGroupedTypeDefs [ "Type1"; "Namespace1.Type2"; "Namespace2.Inner.Type4"; "Namespace2.Type3" ]

    dumpTree false typeDefs |> shouldEqual (
        "global\n" +
        "  Type1\n" +
        "  Namespace1/\n" +
        "    Type2\n" +
        "  Namespace2/\n" +
        "    Type3\n" +
        "    Inner/\n" +
        "      Type4"
    )


[<Fact>]
let ``Grouping - flat compat members flatten across namespaces`` () =
    let typeDefs = mkGroupedTypeDefs [ "Type1"; "Ns1.T1"; "Ns2.T2"; "Ns1.T3" ]

    // AllPreTypeDefs flattens the whole subtree (local types first, then per-namespace in order).
    typeDefs.AllPreTypeDefs() |> Array.map _.Name |> shouldEqual [| "Type1"; "T1"; "T3"; "T2" |]

    typeDefs.ExistsByName "Ns1.T3" |> shouldEqual true
    typeDefs.ExistsByName "Ns2.T2" |> shouldEqual true
    typeDefs.ExistsByName "Missing" |> shouldEqual false
    (typeDefs.FindByName "Ns1.T1").Name |> shouldEqual "T1"


[<Fact>]
let ``Grouping - namespaces are realised lazily`` () =
    let forced = HashSet<string>()

    let typeDefs =
        trackNamespaceForcing forced (mkGroupedTypeDefs [ "Type1"; "Ns1.T1"; "Ns2.Inner.T2" ])

    // Reading global-namespace types and enumerating child namespaces must not force any contents.
    typeDefs.AsArrayOfPreTypeDefs() |> Array.map (fun struct (_, p) -> p.Name) |> shouldEqual [| "Type1" |]
    typeDefs.AsArrayOfPreNamespaces() |> Array.map _.Name |> shouldEqual [| "Ns1"; "Ns2" |]
    forced.Count |> shouldEqual 0

    // Importing a single namespace forces only that one (not its siblings, not deeper levels).
    let ns1 = typeDefs.AsArrayOfPreNamespaces() |> Array.find (fun ns -> ns.Name = "Ns1")
    ns1.GetContents().AsArrayOfPreTypeDefs() |> Array.map (fun struct (_, p) -> p.Name) |> shouldEqual [| "T1" |]
    forced.Contains "Ns1" |> shouldEqual true
    forced.Contains "Ns2" |> shouldEqual false
    forced.Contains "Ns2.Inner" |> shouldEqual false


[<Fact>]
let ``Grouping - pre-type-defs of un-imported namespaces are never built`` () =
    // The maker for a type runs only once its namespace level is realised, so grouping and enumerating
    // namespaces never touches an un-imported namespace's types.
    let built = HashSet<string>()

    let entry (fullName: string) : struct (string list * string) =
        struct (fst (splitILTypeName fullName), fullName)

    let mk (fullName: string) =
        built.Add fullName |> ignore
        mkPreTypeDef (snd (splitILTypeName fullName))

    let typeDefs =
        mkILTypeDefsGroupedComputed (fun () -> [| entry "Type1"; entry "Ns1.T1"; entry "Ns2.Inner.T2" |]) mk

    // Enumerating child namespace names builds nothing: types and namespaces realise independently.
    typeDefs.AsArrayOfPreNamespaces() |> Array.map _.Name |> shouldEqual [| "Ns1"; "Ns2" |]
    built |> shouldEqual (HashSet<string>())

    // Reading the global-namespace types builds only those.
    typeDefs.AsArrayOfPreTypeDefs() |> Array.map (fun struct (_, p) -> p.Name) |> shouldEqual [| "Type1" |]
    built |> shouldEqual (HashSet [ "Type1" ])

    // Importing Ns1 builds only Ns1's types; Ns2's remain untouched.
    let ns1 = typeDefs.AsArrayOfPreNamespaces() |> Array.find (fun ns -> ns.Name = "Ns1")
    ns1.GetContents().AsArrayOfPreTypeDefs() |> Array.map (fun struct (_, p) -> p.Name) |> shouldEqual [| "T1" |]
    built |> shouldEqual (HashSet [ "Type1"; "Ns1.T1" ])


[<Fact>]
let ``Lookup - by name works for flat readers (namespaced pre-type-defs)`` () =
    // A flat table: all entries at the top level, each carrying its full namespace.
    let typeDefs = mkILTypeDefsComputed (fun () -> [| flatEntry "Ns1.Ns2.T"; flatEntry "GlobalType" |])

    typeDefs.ExistsByName "Ns1.Ns2.T" |> shouldEqual true
    typeDefs.ExistsByName "GlobalType" |> shouldEqual true
    typeDefs.ExistsByName "Ns1.T" |> shouldEqual false
    (typeDefs.FindByName "Ns1.Ns2.T").Name |> shouldEqual "T"


[<Fact>]
let ``Lookup - by name descends only into the relevant namespace`` () =
    let forced = HashSet<string>()
    let typeDefs = trackNamespaceForcing forced (mkGroupedTypeDefs [ "Ns1.T1"; "Ns2.Inner.T2" ])

    // Finding a type descends only into the namespaces on its path, not its siblings.
    typeDefs.ExistsByName "Ns2.Inner.T2" |> shouldEqual true
    forced.Contains "Ns2" |> shouldEqual true
    forced.Contains "Ns2.Inner" |> shouldEqual true
    forced.Contains "Ns1" |> shouldEqual false

    // A miss under an existing namespace does not force siblings either.
    typeDefs.ExistsByName "Ns2.Nope" |> shouldEqual false
    forced.Contains "Ns1" |> shouldEqual false


[<Fact>]
let ``Lookup - FindByName reports the missing type name`` () =
    let typeDefs = mkGroupedTypeDefs [ "Ns1.T1" ]

    Assert.Throws<KeyNotFoundException>(fun () -> typeDefs.FindByName "Ns1.Missing" |> ignore).Message
    |> shouldEqual "Ns1.Missing"


[<Fact>]
let ``Hybrid level - lookup and flattening see both sources`` () =
    // A level carrying BOTH namespaced flat entries and a child pre-namespace of the same name. No
    // reader produces this - a level names its children one way or the other, see
    // mkILTypeDefsAndNamespacesComputed - but lookup and flattening still take in both sources.
    let ns2 =
        mkILPreNamespaceComputed("Ns2", fun () -> mkILTypeDefsComputed (fun () -> [| struct ([], mkPreTypeDef "TDeepGrouped") |]))

    let ns1 =
        mkILPreNamespaceComputed(
            "Ns1",
            fun () -> mkILTypeDefsAndNamespacesComputed (fun () -> [| struct ([], mkPreTypeDef "TGrouped") |]) (fun () -> [| ns2 |])
        )

    let typeDefs =
        mkILTypeDefsAndNamespacesComputed
            (fun () ->
                [| struct ([ "Ns1" ], mkPreTypeDef "TFlat")
                   struct ([ "Ns1"; "Ns2" ], mkPreTypeDef "TDeepFlat") |])
            (fun () -> [| ns1 |])

    // The flat entries hit this level's own dictionary; the grouped ones are found by descending.
    typeDefs.ExistsByName "Ns1.TFlat" |> shouldEqual true
    typeDefs.ExistsByName "Ns1.TGrouped" |> shouldEqual true
    typeDefs.ExistsByName "Ns1.Ns2.TDeepFlat" |> shouldEqual true
    typeDefs.ExistsByName "Ns1.Ns2.TDeepGrouped" |> shouldEqual true
    typeDefs.ExistsByName "Ns1.Missing" |> shouldEqual false

    // Flattening unions both sources: this level's entries first, then each namespace's subtree.
    typeDefs.AllPreTypeDefs()
    |> Array.map _.Name
    |> shouldEqual [| "TFlat"; "TDeepFlat"; "TGrouped"; "TDeepGrouped" |]


[<Fact>]
let ``Duplicate namespace nodes - lookup sees the first, flattening sees all`` () =
    // mkILTypeDefsGroupedComputed merges a split namespace into one node, so no reader should hand the
    // same level two namespaces with one name. This pins the documented fallback if one ever does.
    let mkNs name typeName =
        mkILPreNamespaceComputed(name, fun () -> mkILTypeDefsComputed (fun () -> [| struct ([], mkPreTypeDef typeName) |]))

    let typeDefs =
        mkILTypeDefsAndNamespacesComputed (fun () -> [||]) (fun () -> [| mkNs "Ns" "First"; mkNs "Ns" "Second" |])

    typeDefs.ExistsByName "Ns.First" |> shouldEqual true
    typeDefs.ExistsByName "Ns.Second" |> shouldEqual false

    typeDefs.AllPreTypeDefs() |> Array.map _.Name |> shouldEqual [| "First"; "Second" |]


// ---- C# realistic-shape path (reads real metadata via ILModuleReader) --------------------------

let private readCSharpModule (source: string) : ILModuleDef =
    let dllPath =
        CSharp source
        |> withName "NamespaceReaderTest"
        |> compile
        |> shouldSucceed
        |> fun result ->
            match result.OutputPath with
            | Some path -> path
            | None -> failwith "Expected an output path from the C# compilation"

    let options =
        { pdbDirPath = None
          reduceMemoryUsage = ReduceMemoryFlag.Yes
          metadataOnly = MetadataOnlyFlag.Yes
          tryGetMetadataSnapshot = (fun _ -> None) }

    (OpenILModuleReader dllPath options).ILModuleDef


[<Fact>]
let ``Grouping - reader groups real metadata into namespaces (C#)`` () =
    let source = """
public class Type1 { }
namespace Namespace1 { public class Type2 { } }
namespace Namespace2 { public class Type3 { } }
namespace Namespace2.Inner { public class Type4 { } }
"""

    // Sibling order is normalised: Roslyn does not preserve source order across namespaces.
    dumpTree true (readCSharpModule source).TypeDefs |> shouldEqual (
        "global\n" +
        "  Type1\n" +
        "  Namespace1/\n" +
        "    Type2\n" +
        "  Namespace2/\n" +
        "    Type3\n" +
        "    Inner/\n" +
        "      Type4"
    )


[<Fact>]
let ``Nested types - live under their declaring type, not as namespaces (C#)`` () =
    let source = """
namespace Ns { public class Outer { public class Inner { public class Innermost { } } } }
"""

    let moduleDef = readCSharpModule source

    // The tree only exposes namespaces and top-level types: nested types are not namespaces.
    dumpTree true moduleDef.TypeDefs |> shouldEqual (
        "global\n" +
        "  Ns/\n" +
        "    Outer"
    )

    // Nested types are reachable through their declaring type's NestedTypes, keyed by simple name.
    let outer = moduleDef.TypeDefs.FindByName "Ns.Outer"
    outer.NestedTypes.AsArray() |> Array.map (fun td -> td.Name) |> shouldEqual [| "Inner" |]
    outer.NestedTypes.AsArrayOfPreNamespaces() |> shouldEqual [||]

    let inner = outer.NestedTypes.FindByName "Inner"
    inner.NestedTypes.AsArray() |> Array.map (fun td -> td.Name) |> shouldEqual [| "Innermost" |]


[<Fact>]
let ``Nested types - grouping keeps them under the declaring type`` () =
    // A top-level type in a namespace, carrying a nested type in its (namespace-free) NestedTypes.
    let inner =
        ILTypeDef("Inner", TypeAttributes.NestedPublic, ILTypeDefLayout.Auto, [], [], None,
            mkILMethods [], mkILTypeDefs [], mkILFields [], emptyILMethodImpls, mkILEvents [],
            mkILProperties [], emptyILSecurityDecls, emptyILCustomAttrsStored)

    let outer : ILPreTypeDef =
        { new ILPreTypeDef with
            member _.Name = "Outer"
            member _.GetTypeDef() =
                ILTypeDef("Outer", TypeAttributes.Public, ILTypeDefLayout.Auto, [], [], None,
                    mkILMethods [], mkILTypeDefs [ inner ], mkILFields [], emptyILMethodImpls, mkILEvents [],
                    mkILProperties [], emptyILSecurityDecls, emptyILCustomAttrsStored) }

    let typeDefs = mkILTypeDefsGroupedComputed (fun () -> [| struct ([ "Ns" ], outer) |]) id

    // Outer sits in namespace Ns; Inner is not a top-level type or namespace.
    dumpTree false typeDefs |> shouldEqual (
        "global\n" +
        "  Ns/\n" +
        "    Outer"
    )

    let nsContents = (typeDefs.AsArrayOfPreNamespaces() |> Array.exactlyOne).GetContents()
    let struct (_, outerPre) = nsContents.AsArrayOfPreTypeDefs() |> Array.exactlyOne
    outerPre.GetTypeDef().NestedTypes.AsArray() |> Array.map (fun td -> td.Name) |> shouldEqual [| "Inner" |]


// ---- Row indices let a flattened read module be put back into metadata order -------------------

/// The full names of a module's top-level types, in raw metadata TypeDef table order.
let private metadataTypeDefOrder (path: string) =
    use fs = System.IO.File.OpenRead path
    use pe = new System.Reflection.PortableExecutable.PEReader(fs)
    let md = System.Reflection.Metadata.PEReaderExtensions.GetMetadataReader pe

    [ for handle in md.TypeDefinitions do
        let td: System.Reflection.Metadata.TypeDefinition = md.GetTypeDefinition handle
        // Nested types have their own rows; ILTypeDefs only holds top-level ones.
        if td.GetDeclaringType().IsNil then
            let ns = md.GetString td.Namespace
            let name = md.GetString td.Name
            yield (if ns = "" then name else ns + "." + name) ]

[<Fact>]
let ``Reading - sorting a flattened module by row index gives the metadata TypeDef order`` () =
    // Flattening (AsArray/AsList/the enumerator) walks namespace by namespace, so it does *not* reproduce
    // the TypeDef table order - a namespace can be split across the table. Consumers that need the
    // reader's order sort by MetadataIndex; static linking does, so that --standalone emits the types of
    // a dependent assembly in its original order. Pin both halves of that contract here.
    //
    // FSharp.Core is the right subject: F#-compiled assemblies routinely split a namespace across the
    // TypeDef table (Roslyn-compiled ones group it, which would make this test vacuous).
    let path = typeof<int list>.Assembly.Location
    let metadataOrder = metadataTypeDefOrder path

    let options =
        { pdbDirPath = None
          reduceMemoryUsage = ReduceMemoryFlag.Yes
          metadataOnly = MetadataOnlyFlag.Yes
          tryGetMetadataSnapshot = (fun _ -> None) }

    let moduleDef = (OpenILModuleReader path options).ILModuleDef
    let typeDefs = moduleDef.TypeDefs.AsList()

    // ILTypeDef.Name for a top-level type read from metadata is the full "Namespace.Name".
    // Every row is there, but the namespace walk hands them back in a different order.
    let names = typeDefs |> List.map _.Name
    List.sort names |> shouldEqual (List.sort metadataOrder)
    Assert.True(names <> metadataOrder, "flattening happened to match row order, so the sort below proves nothing")

    // List.sortBy is stable, so row indices alone restore the order the rows were read in.
    typeDefs |> List.sortBy (fun td -> td.MetadataIndex) |> List.map _.Name |> shouldEqual metadataOrder
