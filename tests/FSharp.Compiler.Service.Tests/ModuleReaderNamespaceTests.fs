module FSharp.Compiler.Service.Tests.ModuleReaderNamespaceTests

open System.Collections.Generic
open System.Reflection
open System.Text
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Service.Tests.Common
// The synthetic ILModuleReader harness (ModuleReader, referenceReaderProjectWithTypeDefs).
open FSharp.Compiler.Service.Tests.ModuleReaderCancellationTests
open FSharp.Test.Compiler
open FSharp.Test.Assert
open Xunit

// How ILPreTypeDef / ILPreNamespace are created when reading metadata: only imported namespaces should
// realise their pre-type-defs. Roslyn can't emit a genuinely split namespace, so C# is used only for the
// realistic-shape test and split order is asserted with synthetic arrays.

let private dumpTree (sortSiblings: bool) (typeDefs: ILTypeDefs) : string =
    let sb = StringBuilder()

    let rec go (indent: int) (types: ILPreTypeDef[]) (namespaces: ILPreNamespace[]) =
        let pad = String.replicate indent "  "

        let typeNames =
            [ for pre in types do
                // Skip the always-present <Module> pseudo-type.
                if pre.Name <> "<Module>" then yield pre.Name ]
        for name in (if sortSiblings then List.sort typeNames else typeNames) do
            sb.AppendLine($"{pad}{name}") |> ignore

        let namespaces = List.ofArray namespaces
        let namespaces = if sortSiblings then List.sortBy (fun (ns: ILPreNamespace) -> ns.Name) namespaces else namespaces
        for ns in namespaces do
            sb.AppendLine($"{pad}{ns.Name}/") |> ignore
            go (indent + 1) (ns.GetTypes()) (ns.GetNamespaces())

    sb.AppendLine("global") |> ignore
    go 1 (typeDefs.AsArrayOfPreTypeDefs()) (typeDefs.AsArrayOfPreNamespaces())
    sb.ToString().Replace("\r\n", "\n").TrimEnd('\n')


// ---- Synthetic pre-type-defs (control the exact metadata order) --------------------------------

/// Carries only the simple name; the namespace lives in the containing table.
let private mkPreTypeDef (name: string) : ILPreTypeDef =
    { new ILPreTypeDef with
        member _.Name = name
        member _.GetTypeDef() =
            ILTypeDef(name, TypeAttributes.Public, ILTypeDefLayout.Auto, [], [], None,
                mkILMethods [], mkILTypeDefs [], mkILFields [], emptyILMethodImpls, mkILEvents [],
                mkILProperties [], emptyILSecurityDecls, emptyILCustomAttrsStored) }

let private entryOf (fullName: string) : struct (string list * ILPreTypeDef) =
    let ns, name = splitILTypeName fullName
    struct (ns, mkPreTypeDef name)

/// Full type names, in order, through the production grouping.
let private mkGroupedTypeDefs (fullNames: string list) : ILTypeDefs =
    mkILTypeDefsGroupedComputed (fun () -> [| for n in fullNames -> entryOf n |]) (fun () -> Array.empty)

/// Wrap the type defs so that reading either half of any namespace records its full path in `forced`.
let private trackNamespaceForcing (forced: HashSet<string>) (typeDefs: ILTypeDefs) : ILTypeDefs =
    let rec track (path: string) (ns: ILPreNamespace) =
        let childPath (child: ILPreNamespace) =
            if path = "" then child.Name else $"{path}.{child.Name}"

        mkILPreNamespaceComputed(
            ns.Name,
            (fun () ->
                forced.Add path |> ignore
                ns.GetTypes()),
            (fun () ->
                forced.Add path |> ignore
                [| for child in ns.GetNamespaces() -> track (childPath child) child |])
        )

    // The level itself is the table handed in; only the namespaces below it are tracked.
    mkILTypeDefsOfNamespace (
        mkILPreNamespaceComputed(
            "",
            (fun () -> typeDefs.AsArrayOfPreTypeDefs()),
            (fun () -> [| for ns in typeDefs.AsArrayOfPreNamespaces() -> track ns.Name ns |])
        )
    )


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
    typeDefs.AsArrayOfPreTypeDefs() |> Array.map _.Name |> shouldEqual [| "Type1" |]
    typeDefs.AsArrayOfPreNamespaces() |> Array.map _.Name |> shouldEqual [| "Ns1"; "Ns2" |]
    forced.Count |> shouldEqual 0

    // Importing a single namespace forces only that one (not its siblings, not deeper levels).
    let ns1 = typeDefs.AsArrayOfPreNamespaces() |> Array.find (fun ns -> ns.Name = "Ns1")
    ns1.GetTypes() |> Array.map _.Name |> shouldEqual [| "T1" |]
    forced.Contains "Ns1" |> shouldEqual true
    forced.Contains "Ns2" |> shouldEqual false
    forced.Contains "Ns2.Inner" |> shouldEqual false


[<Fact>]
let ``Grouping - un-imported namespaces never have their type names read`` () =
    // Grouping needs an entry's namespace, never its name - and a name is a string-heap read for the
    // metadata reader.
    let read = HashSet<string>()

    let entry (fullName: string) : struct (string list * ILPreTypeDef) =
        let ns, name = splitILTypeName fullName

        let pre =
            { new ILPreTypeDef with
                member _.Name =
                    read.Add fullName |> ignore
                    name

                member _.GetTypeDef() = (mkPreTypeDef name).GetTypeDef() }

        struct (ns, pre)

    let typeDefs =
        mkILTypeDefsGroupedComputed
            (fun () -> [| entry "Type1"; entry "Ns1.T1"; entry "Ns2.Inner.T2" |])
            (fun () -> Array.empty)

    // Child namespaces are named by the grouping, not by the types in them, so enumerating reads nothing.
    typeDefs.AsArrayOfPreNamespaces() |> Array.map _.Name |> shouldEqual [| "Ns1"; "Ns2" |]
    read |> shouldEqual (HashSet<string>())

    typeDefs.AsArrayOfPreTypeDefs() |> Array.map _.Name |> shouldEqual [| "Type1" |]
    read |> shouldEqual (HashSet [ "Type1" ])

    // Importing Ns1 reads only Ns1's; Ns2's remain untouched.
    let ns1 = typeDefs.AsArrayOfPreNamespaces() |> Array.find (fun ns -> ns.Name = "Ns1")
    ns1.GetTypes() |> Array.map _.Name |> shouldEqual [| "T1" |]
    read |> shouldEqual (HashSet [ "Type1"; "Ns1.T1" ])


[<Fact>]
let ``Lookup - by name works for a deeply nested namespace`` () =
    let typeDefs =
        mkILTypeDefsGroupedComputed (fun () -> [| entryOf "Ns1.Ns2.T"; entryOf "GlobalType" |]) (fun () -> Array.empty)

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
let ``Mixed level - a namespace named by both an entry and a pre-namespace becomes one child`` () =
    // Children from BOTH grouped entries and supplied pre-namespaces, sharing a name: they must be one
    // child at every depth, so an importer never sees two namespaces of one name.
    let ns2 =
        mkILPreNamespaceComputed("Ns2", (fun () -> [| mkPreTypeDef "TDeepSupplied" |]), (fun () -> Array.empty))

    let ns1 =
        mkILPreNamespaceComputed("Ns1", (fun () -> [| mkPreTypeDef "TSupplied" |]), (fun () -> [| ns2 |]))

    let typeDefs =
        mkILTypeDefsGroupedComputed
            (fun () ->
                [| struct ([ "Ns1" ], mkPreTypeDef "TGrouped")
                   struct ([ "Ns1"; "Ns2" ], mkPreTypeDef "TDeepGrouped") |])
            (fun () -> [| ns1 |])

    typeDefs.AsArrayOfPreNamespaces() |> Array.map _.Name |> shouldEqual [| "Ns1" |]

    typeDefs.ExistsByName "Ns1.TGrouped" |> shouldEqual true
    typeDefs.ExistsByName "Ns1.TSupplied" |> shouldEqual true
    typeDefs.ExistsByName "Ns1.Ns2.TDeepGrouped" |> shouldEqual true
    typeDefs.ExistsByName "Ns1.Ns2.TDeepSupplied" |> shouldEqual true
    typeDefs.ExistsByName "Ns1.Missing" |> shouldEqual false

    // Flattening a merged child takes the grouped side first, then the supplied one.
    typeDefs.AllPreTypeDefs()
    |> Array.map _.Name
    |> shouldEqual [| "TGrouped"; "TSupplied"; "TDeepGrouped"; "TDeepSupplied" |]


[<Fact>]
let ``Duplicate namespace nodes - two supplied children of one name merge`` () =
    let mkNs name typeName =
        mkILPreNamespaceComputed(name, (fun () -> [| mkPreTypeDef typeName |]), (fun () -> Array.empty))

    let typeDefs =
        mkILTypeDefsGroupedComputed (fun () -> [||]) (fun () -> [| mkNs "Ns" "First"; mkNs "Ns" "Second" |])

    typeDefs.AsArrayOfPreNamespaces() |> Array.map _.Name |> shouldEqual [| "Ns" |]
    typeDefs.ExistsByName "Ns.First" |> shouldEqual true
    typeDefs.ExistsByName "Ns.Second" |> shouldEqual true
    typeDefs.AllPreTypeDefs() |> Array.map _.Name |> shouldEqual [| "First"; "Second" |]

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

    let typeDefs = mkILTypeDefsGroupedComputed (fun () -> [| struct ([ "Ns" ], outer) |]) (fun () -> Array.empty)

    // Outer sits in namespace Ns; Inner is not a top-level type or namespace.
    dumpTree false typeDefs |> shouldEqual (
        "global\n" +
        "  Ns/\n" +
        "    Outer"
    )

    let ns: ILPreNamespace = typeDefs.AsArrayOfPreNamespaces() |> Array.exactlyOne
    let outerPre = ns.GetTypes() |> Array.exactlyOne
    outerPre.GetTypeDef().NestedTypes.AsArray() |> Array.map (fun td -> td.Name) |> shouldEqual [| "Inner" |]


// ---- End-to-end: what checking a file actually reads out of a reference ------------------------
//
// The tests above pin the reader API in isolation. These pin the guarantee it exists for: checking a file
// must pull only the namespaces it names. The regression is easy to introduce far from the reader -
// anything that walks a CCU's whole ModuleOrNamespaceType realises every namespace of it, as
// addConstraintSources did.

/// A type to put in the synthetic reference assembly.
type private TypeShape =
    { Name: string
      Namespace: string list
      /// Names of the types nested in it (leaves themselves).
      Nested: string list
      /// Full name of a base type in the same assembly.
      Extends: string option }

let private shape name ns =
    { Name = name; Namespace = ns; Nested = []; Extends = None }

let private fullName ns name = String.concat "." (ns @ [ name ])

/// Resolved by simple name against the project's references, so System.Object can be named.
let private systemRuntimeScopeRef =
    ILScopeRef.Assembly(ILAssemblyRef.Create("System.Runtime", None, None, false, None, None))

/// What a check pulled out of the reference, by full type name ("Ns.T", nested as "Ns.T+Inner").
type private ReadLog() =
    member val TypeDefs = HashSet<string>()
    member val Members = HashSet<string>()
    member val NestedTypes = HashSet<string>()
    member val CustomAttrs = HashSet<string>()

let private sorted (names: HashSet<string>) = List.ofSeq names |> List.sort

/// Records reading its type def, and each part of it read afterwards.
///
/// `ilName` follows the reader: a top-level type def carries its full name while the pre-type-def carries
/// the simple one, and a nested type def carries the simple name. Import rebuilds a nested type's
/// ILTypeRef from its declaring type def's name, so a simple name there resolves in the wrong namespace.
let rec private trackedPreTypeDefWith
    (log: ReadLog)
    (attributes: TypeAttributes)
    (ilName: string)
    (path: string)
    (ty: TypeShape)
    : ILPreTypeDef =
    let methods =
        mkILMethodsComputed (fun () ->
            log.Members.Add path |> ignore
            [||])

    let nested =
        mkILTypeDefsComputed (fun () ->
            log.NestedTypes.Add path |> ignore

            [| for name in ty.Nested ->
                   trackedPreTypeDefWith log TypeAttributes.NestedPublic name $"{path}+{name}" (shape name []) |])

    let customAttrs =
        ILAttributesStored.CreateReader(
            0,
            fun _ ->
                log.CustomAttrs.Add path |> ignore
                [||]
        )

    // Without a base type a member lookup has no hierarchy to walk and simply fails.
    let extends =
        let scope, name =
            match ty.Extends with
            | Some name -> ILScopeRef.Local, name
            | None -> systemRuntimeScopeRef, "System.Object"

        Some(mkILBoxedType (mkILNonGenericTySpec (mkILTyRef (scope, name))))

    // One instance, as a real reader hands out: import holds on to the one it was given.
    let typeDef =
        ILTypeDef(ilName, attributes, ILTypeDefLayout.Auto, [], [], extends,
            methods, nested, mkILFields [], emptyILMethodImpls, mkILEvents [],
            mkILProperties [], emptyILSecurityDecls, customAttrs)

    { new ILPreTypeDef with
        member _.Name = ty.Name

        member _.GetTypeDef() =
            log.TypeDefs.Add path |> ignore
            typeDef }

let private trackedPreTypeDef log (ty: TypeShape) =
    let path = fullName ty.Namespace ty.Name
    trackedPreTypeDefWith log TypeAttributes.Public path path ty

/// Check `source` against a reference assembly built from `shapes`, and report what it read.
let private checkAgainstReference (shapes: TypeShape list) (source: string) =
    let log = ReadLog()

    let typeDefs =
        mkILTypeDefsGroupedComputed (fun () -> [| for s in shapes -> struct (s.Namespace, trackedPreTypeDef log s) |]) (fun () ->
            Array.empty)

    let path, options = mkTestFileAndOptions [||]
    let options = referenceReaderProjectWithTypeDefs typeDefs false options

    let _, results = parseAndCheckFile path source options

    results.Diagnostics
    |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
    |> Array.map _.Message
    |> shouldEqual [||]

    log

/// Types at several namespace depths, with a sibling at each and a nested type in Ns1.A.
let private referenceShapes =
    [ shape "G" []
      { shape "A" [ "Ns1" ] with Nested = [ "Inner" ] }
      shape "B" [ "Ns1" ]
      shape "D" [ "Ns1"; "Deep" ]
      shape "X" [ "Ns2" ] ]

let private useNs1A = """
module Module

let f (x: Ns1.A) = x
"""

[<Fact>]
let ``Laziness - checking a file reads only the namespaces it names`` () =
    let log = checkAgainstReference referenceShapes useNs1A

    // Import granularity is the namespace level, not the type, so Ns1.B and G come along. What matters is
    // that the levels off the path - Ns1.Deep and Ns2 - are never read.
    sorted log.TypeDefs |> shouldEqual [ "G"; "Ns1.A"; "Ns1.B" ]

[<Fact>]
let ``Laziness - checking a file reads no type name of an un-named namespace`` () =
    let log = ReadLog()
    let read = HashSet<string>()

    // The isolated test above pins that grouping never reads a name; this pins that it survives a check.
    let typeDefs =
        mkILTypeDefsGroupedComputed
            (fun () ->
                [| for s in referenceShapes ->
                       let path = fullName s.Namespace s.Name
                       let pre = trackedPreTypeDef log s

                       let tracked =
                           { new ILPreTypeDef with
                               member _.Name =
                                   read.Add path |> ignore
                                   pre.Name

                               member _.GetTypeDef() = pre.GetTypeDef() }

                       struct (s.Namespace, tracked) |])
            (fun () -> Array.empty)

    let path, options = mkTestFileAndOptions [||]
    let options = referenceReaderProjectWithTypeDefs typeDefs false options
    parseAndCheckFile path useNs1A options |> ignore

    sorted read |> shouldEqual [ "G"; "Ns1.A"; "Ns1.B" ]
    sorted log.TypeDefs |> shouldEqual [ "G"; "Ns1.A"; "Ns1.B" ]

[<Fact>]
let ``Laziness - importing a type reads neither its members nor its nested types`` () =
    let log = checkAgainstReference referenceShapes useNs1A

    // Reading a type def is the whole cost of importing it: what is inside stays behind its own lazies.
    sorted log.Members |> shouldEqual []
    sorted log.NestedTypes |> shouldEqual []

[<Fact>]
let ``Laziness - attributes are read for the types brought into scope, not for all imported ones`` () =
    let log = checkAgainstReference referenceShapes useNs1A

    // Attributes are read when a type enters the name environment or a use of it is resolved. Ns1.B is
    // imported alongside Ns1.A but never enters scope.
    sorted log.CustomAttrs |> shouldEqual [ "G"; "Ns1.A" ]

[<Fact>]
let ``Laziness - a nested type is read only once it is named`` () =
    let source = """
module Module

let f (x: Ns1.A.Inner) = x
"""
    let log = checkAgainstReference referenceShapes source

    // Naming the nested type forces its declaring type's nested table - and only that one.
    sorted log.NestedTypes |> shouldEqual [ "Ns1.A" ]
    sorted log.TypeDefs |> shouldEqual [ "G"; "Ns1.A"; "Ns1.A+Inner"; "Ns1.B" ]
    sorted log.Members |> shouldEqual []

[<Fact>]
let ``Laziness - opening a namespace does not read its child namespaces`` () =
    let source = """
module Module

open Ns1

let f (x: A) = x
"""
    let log = checkAgainstReference referenceShapes source

    // An open imports the namespace's own types, so Ns1.Deep must stay untouched.
    sorted log.TypeDefs |> shouldEqual [ "G"; "Ns1.A"; "Ns1.B" ]

    // An open brings every type of Ns1 into scope, so it reads all their attributes - bounded by Ns1.
    sorted log.CustomAttrs |> shouldEqual [ "G"; "Ns1.A"; "Ns1.B" ]

[<Fact>]
let ``Laziness - a deep type reads only the levels on its path`` () =
    let source = """
module Module

let f (x: Ns1.Deep.D) = x
"""
    let log = checkAgainstReference referenceShapes source

    // Ns1 is on the path so its own types come too; Ns2 is not.
    sorted log.TypeDefs |> shouldEqual [ "G"; "Ns1.A"; "Ns1.B"; "Ns1.Deep.D" ]

[<Fact>]
let ``Laziness - a reference nothing names reads only its root level`` () =
    let source = """
module Module

let x = 1
"""
    let log = checkAgainstReference referenceShapes source

    // The floor: the initial name resolution environment names each reference's root namespaces.
    sorted log.TypeDefs |> shouldEqual [ "G" ]

let private withBaseTypeShapes =
    [ { shape "A" [ "Ns1" ] with Extends = Some "Ns2.Base" }
      shape "Base" [ "Ns2" ]
      shape "X" [ "Ns2" ]
      shape "D" [ "Ns1"; "Deep" ] ]

[<Fact>]
let ``Laziness - the base type of an imported type is not read`` () =
    let log = checkAgainstReference withBaseTypeShapes useNs1A

    // Importing Ns1.A only records its base type as an ILType; nothing here needs the hierarchy, so Ns2
    // stays unread - and with no global type, even the root level costs nothing.
    sorted log.TypeDefs |> shouldEqual [ "Ns1.A" ]

[<Fact>]
let ``Laziness - a member lookup reads the base type's namespace`` () =
    let source = """
module Module

let f (x: Ns1.A) = x.ToString()
"""
    let log = checkAgainstReference withBaseTypeShapes source

    // A member lookup walks the hierarchy, so the base type is imported, realising its namespace level.
    sorted log.TypeDefs |> shouldEqual [ "Ns1.A"; "Ns2.Base"; "Ns2.X" ]
    sorted log.Members |> shouldEqual [ "Ns1.A"; "Ns2.Base" ]


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
    // Flattening walks namespace by namespace, so it does not reproduce the TypeDef table order - a
    // namespace can be split across it. Consumers needing the reader's order sort by MetadataIndex, as
    // static linking does. FSharp.Core is the subject because F# routinely splits a namespace; Roslyn doesn't.
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
