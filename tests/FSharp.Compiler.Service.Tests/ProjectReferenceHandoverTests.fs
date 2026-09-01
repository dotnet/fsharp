module FSharp.Compiler.Service.Tests.ProjectReferenceHandoverTests

open Xunit
open System.IO
open System.Reflection
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.IO
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Symbols
open FSharp.Compiler.TypedTree
open TestFramework

// A referenced project can hand its contents to a consumer already imported instead of pickling them.
// Unpickling is what rewrites ILScopeRef.Local into the referenced assembly, so the handed-over form
// has to do the same: `internal` is a compilation path rooted at Local, and a consumer whose own paths
// are also Local would otherwise read the reference's internals as its own.
//
// Only the background compiler hands contents over, so these pin it rather than taking the suite default.
let private mkCheckerOn transparent share =
    FSharpChecker.Create(
        // the default is three, and these graphs are larger: an evicted builder is rebuilt with a fresh
        // provider, which would look like a refused handover
        projectCacheSize = 50,
        shareImportedAssemblies = share,
        enablePartialTypeChecking = false,
        useTransparentCompiler = transparent
    )

let private mkChecker share = mkCheckerOn false share

let private writeSourceAs (extension: string) (source: string) =
    let fileName = Path.ChangeExtension(getTemporaryFileName (), extension)
    FileSystem.OpenFileForWriteShim(fileName).Write(source)
    fileName

let private writeSource source = writeSourceAs ".fs" source

/// Options for one project of the given files, referencing the given already-built ones, output under
/// the given assembly name where one is asked for - a project whose contents are offered is registered
/// under that name, and a reference to it from elsewhere has to resolve to it
let private projectOptionsNamed (checker: FSharpChecker) assemblyName fileNames references extraOptions =
    let baseName =
        match assemblyName with
        | Some name -> Path.Combine(Path.GetDirectoryName(getTemporaryFileName ()), name)
        | None -> getTemporaryFileName ()

    let dllName = Path.ChangeExtension(baseName, ".dll")
    let projName = Path.ChangeExtension(baseName, ".fsproj")
    let args = mkProjectCommandLineArgsSilent (dllName, fileNames)
    let options = checker.GetProjectOptionsFromCommandLineArgs(projName, args)

    let options =
        { options with
            SourceFiles = fileNames
            OtherOptions =
                Array.concat
                    [ options.OtherOptions
                      [| for dll, _ in references -> "-r:" + dll |]
                      extraOptions ]
            ReferencedProjects =
                [| for dll, opts in references -> FSharpReferencedProject.FSharpReference(dll, opts) |] }

    dllName, options

let private projectOptionsOfFiles checker fileNames references extraOptions =
    projectOptionsNamed checker None fileNames references extraOptions

/// Options for one project of a single implementation file
let private projectOptions (checker: FSharpChecker) source references extraOptions =
    projectOptionsOfFiles checker [| writeSource source |] references extraOptions

/// FSharpAssembly holds the ccu it wraps but does not expose it. The contents rather than the thunk: each
/// reader holds a thunk of its own around the one imported form, and the form is what is shared.
let private ccuOf (assembly: FSharpAssembly) =
    assembly.GetType().GetFields(BindingFlags.Instance ||| BindingFlags.NonPublic ||| BindingFlags.Public)
    |> Array.pick (fun field ->
        match field.GetValue assembly with
        | :? CcuThunk as ccu -> Some ccu.Contents
        | _ -> None)

/// The ccu the given project ended up with for one of its references
let private referencedCcu (checker: FSharpChecker) (options: FSharpProjectOptions) name =
    let results = checker.ParseAndCheckProject options |> Async.RunSynchronously

    results.ProjectContext.GetReferencedAssemblies()
    |> List.find (fun assembly -> assembly.SimpleName = name)
    |> ccuOf

let private errorsIn (checker: FSharpChecker) (options: FSharpProjectOptions) =
    let results = checker.ParseAndCheckProject options |> Async.RunSynchronously

    results.Diagnostics
    |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)

let private librarySource =
    """
module Library

let publicValue = 1
let internal secretValue = 2
type internal SecretType = { S: int }
type Colour = internal Red | Green
type internal SecretClass() =
    member _.M = 3
"""

let private consumerOf body = "module Consumer\n" + body + "\n"

/// Enough on its own where the shape of the internal does not matter
let private useInternalValue = "let useValue = Library.secretValue"

[<Theory>]
[<InlineData "let useValue = Library.secretValue">]
[<InlineData "let useType = typeof<Library.SecretType>">]
[<InlineData "let useCase = Library.Red">]
[<InlineData "let useClass = Library.SecretClass().M">]
let ``An internal of a handed-over project is not visible to a consumer`` (useInternal: string) =
    let checker = mkChecker true
    let library = projectOptions checker librarySource [] [||]
    let _, consumer = projectOptions checker (consumerOf useInternal) [ library ] [||]

    Assert.NotEmpty(errorsIn checker consumer)

[<Fact>]
let ``Handed-over contents keep the public surface usable`` () =
    let checker = mkChecker true
    let library = projectOptions checker librarySource [] [||]

    let _, consumer =
        projectOptions checker (consumerOf "let usePublic = Library.publicValue") [ library ] [||]

    Assert.Empty(errorsIn checker consumer)

[<Fact>]
let ``A consumer on another framework import layer takes the contents`` () =
    let checker = mkChecker true
    let library = projectOptions checker librarySource [] [||]
    let libraryName = Path.GetFileNameWithoutExtension(fst library)

    // Reading nullable-reference metadata keys a different import layer, so this consumer's framework
    // members - FSharp.Core's included - are not the ones the library was checked against. It takes the
    // contents anyway: every non-local reference in them is bound to the ccu this consumer resolves that
    // name to, which is what unpickling would have done for it.
    let _, sameLayer =
        projectOptions checker (consumerOf "let usePublic = Library.publicValue + 1") [ library ] [||]

    let otherLayer body =
        projectOptions checker (consumerOf body) [ library ] [| "--langversion:9.0"; "--checknulls+" |]
        |> snd

    let firstOther = otherLayer "let useOnce = Library.publicValue + 1"
    let secondOther = otherLayer "let useTwice = Library.publicValue + 2"

    Assert.Empty(errorsIn checker sameLayer)
    Assert.Empty(errorsIn checker firstOther)

    // Both are on that other layer, so one bound copy serves them: were the contents not taken they would
    // each have unpickled one of their own
    Assert.Same(referencedCcu checker firstOther libraryName, referencedCcu checker secondOther libraryName)

    // And that copy cannot also serve the layer the library was checked on: they disagree about
    // FSharp.Core, and each must see its own
    Assert.NotSame(referencedCcu checker sameLayer libraryName, referencedCcu checker firstOther libraryName)

/// Binding the contents to the reader's own ccus must not make anything visible that a consumer of the
/// compiled assembly would not see
[<Fact>]
let ``An internal is not visible to a consumer on another layer either`` () =
    let checker = mkChecker true
    let library = projectOptions checker librarySource [] [||]

    let _, otherLayer =
        projectOptions checker (consumerOf useInternalValue) [ library ] [| "--langversion:9.0"; "--checknulls+" |]

    Assert.NotEmpty(errorsIn checker otherLayer)

[<Fact>]
let ``Type identity survives a chain of handed-over project references`` () =
    let checker = mkChecker true

    let leaf =
        projectOptions
            checker
            """
module Leaf

let publicLeaf = 1
let internal secretLeaf = 2
type LeafType = { X: int }
"""
            []
            [||]

    let middle =
        projectOptions
            checker
            """
module Middle

let publicMiddle = Leaf.publicLeaf
let makeLeafType () : Leaf.LeafType = { X = 1 }
"""
            [ leaf ]
            [||]

    // As a build passes project references on transitively
    let _, consumer =
        projectOptions
            checker
            """
module Consumer

let useMiddle = Middle.publicMiddle
let useLeafThroughMiddle = (Middle.makeLeafType ()).X
let useLeafDirectly : Leaf.LeafType = { X = 2 }
"""
            [ middle; leaf ]
            [||]

    // The leaf type reached through the middle project must be the one the consumer imports itself
    Assert.Empty(errorsIn checker consumer)

/// Handing the contents over is what makes one imported form serve every consumer. Were the handover
/// to stop happening these would still type-check, so this is what holds the feature in place.
[<Theory>]
[<InlineData true>]
[<InlineData false>]
let ``Consumers share one imported ccu only when contents are handed over`` share =
    let checker = mkChecker share
    let library = projectOptions checker librarySource [] [||]
    let libraryName = Path.GetFileNameWithoutExtension(fst library)

    let _, first =
        projectOptions checker (consumerOf "let first = Library.publicValue") [ library ] [||]

    let _, second =
        projectOptions checker (consumerOf "let second = Library.publicValue") [ library ] [||]

    let firstCcu = referencedCcu checker first libraryName
    let secondCcu = referencedCcu checker second libraryName

    if share then
        Assert.Same(firstCcu, secondCcu)
    else
        Assert.NotSame(firstCcu, secondCcu)

/// Importing IL metadata consults the settings in one place only - whether nullable-reference attributes
/// are read into the TAST - so two projects agreeing on that import one framework layer whatever their
/// language versions. Keying the layer on the version itself gave each of them a framework of its own,
/// and with it a second copy of every framework entity.
[<Fact>]
let ``Projects on different language versions share one framework import`` () =
    let checker = mkChecker true

    let ccuOfFSharpCore extraOptions =
        let _, options = projectOptions checker (consumerOf "let value = 1") [] extraOptions
        referencedCcu checker options "FSharp.Core"

    Assert.Same(ccuOfFSharpCore [| "--langversion:8.0" |], ccuOfFSharpCore [| "--langversion:9.0" |])

/// The framework import layer is keyed on what importing depends on, so projects that disagree about
/// nullable-reference metadata genuinely import different entities and must refuse each other. realsig is
/// not part of that key: it changes what a project emits, not what it imports. But a project whose
/// realsig differs from the one its layer was
/// first built for is handed a TcGlobals of its own over those very entities, and comparing the TcGlobals
/// rather than the layer refused it every handover - including from a project with the same setting,
/// which made how much of a solution shared depend on which project reached the layer first.
[<Fact>]
let ``Consumers differing only in realsig share one imported ccu`` () =
    let checker = mkChecker true
    let library = projectOptions checker librarySource [] [| "--realsig+" |]
    let libraryName = Path.GetFileNameWithoutExtension(fst library)

    let _, plain =
        projectOptions checker (consumerOf "let first = Library.publicValue") [ library ] [||]

    let _, real =
        projectOptions checker (consumerOf "let second = Library.publicValue") [ library ] [| "--realsig+" |]

    Assert.Empty(errorsIn checker plain)
    Assert.Empty(errorsIn checker real)
    Assert.Same(referencedCcu checker plain libraryName, referencedCcu checker real libraryName)

/// The memo holds a pessimistic entry while a provider's own answer is being computed, so that a cycle
/// refuses rather than loops. A diamond re-reaches a project without a cycle, and must not read that entry.
[<Fact>]
let ``A project reached twice through a diamond is still taken`` () =
    let checker = mkChecker true
    let leaf = projectOptions checker "module Leaf\nlet leaf = 1\n" [] [||]
    let left = projectOptions checker "module Left\nlet left = Leaf.leaf\n" [ leaf ] [||]
    let right = projectOptions checker "module Right\nlet right = Leaf.leaf\n" [ leaf ] [||]

    let join =
        projectOptions checker "module Join\nlet join = Left.left + Right.right\n" [ left; right; leaf ] [||]

    let joinName = Path.GetFileNameWithoutExtension(fst join)
    let references = [ join; left; right; leaf ]

    let _, first = projectOptions checker "module First\nlet x = Join.join\n" references [||]
    let _, second = projectOptions checker "module Second\nlet y = Join.join\n" references [||]

    // Asking Join reaches Leaf through both Left and Right; the second ask must see the settled answer
    Assert.Same(referencedCcu checker first joinName, referencedCcu checker second joinName)

/// The offer used to require agreement about every assembly the offering project imports, so a
/// private implementation dependency - one the consumers never resolve, at any layer - refused it.
[<Fact>]
let ``A dependency the consumer does not have does not refuse the offer`` () =
    let checker = mkChecker true

    // outside the standard reference set, so only the library imports anything of this name
    let privateDependency = "-r:" + typeof<FactAttribute>.Assembly.Location

    let library = projectOptions checker librarySource [] [| privateDependency |]
    let libraryName = Path.GetFileNameWithoutExtension(fst library)

    let _, first =
        projectOptions checker (consumerOf "let first = Library.publicValue") [ library ] [||]

    let _, second =
        projectOptions checker (consumerOf "let second = Library.publicValue") [ library ] [||]

    Assert.Empty(errorsIn checker first)
    Assert.Same(referencedCcu checker first libraryName, referencedCcu checker second libraryName)

/// The handed-over ccu is the offering project's own, not an entry in the shared cache, so consumers
/// racing each other must still land on the one object.
[<Fact>]
let ``Consumers checked at once share one imported ccu`` () =
    let checker = mkChecker true
    let library = projectOptions checker librarySource [] [||]
    let libraryName = Path.GetFileNameWithoutExtension(fst library)

    let consumers =
        [ for i in 1..8 ->
            projectOptions checker (consumerOf $"let use%d{i} = Library.publicValue") [ library ] [||]
            |> snd ]

    let results =
        consumers
        |> List.map checker.ParseAndCheckProject
        |> Async.Parallel
        |> Async.RunSynchronously

    for r in results do
        Assert.Empty(r.Diagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error))

    let ccus =
        results
        |> Array.map (fun r ->
            r.ProjectContext.GetReferencedAssemblies()
            |> List.find (fun a -> a.SimpleName = libraryName)
            |> ccuOf)

    for ccu in ccus do
        Assert.Same(ccus[0], ccu)

/// Wide enough for the two paths to disagree somewhere: accessibility, abbreviations, generics, members,
/// fields, union cases, literals, inlining and documentation all travel differently between them.
let private surfaceSource =
    """
module Surface

open System

/// A documented literal
[<Literal>]
let Answer = 42

let mutable counter = 0

/// A record with an internal field
type Record =
    { Name: string
      mutable Count: int
      internal Hidden: int }

    /// A documented property
    member this.Doubled = this.Count * 2

    static member Create name = { Name = name; Count = 0; Hidden = 0 }

/// A union with an internal case
type Shape =
    | Circle of radius: float
    | Rect of width: float * height: float
    | internal Secret

type Alias = Record

type Generic<'T when 'T :> IComparable>(value: 'T) =
    member _.Value = value
    member _.Boxed = box value

[<AbstractClass>]
type Base() =
    abstract Describe: unit -> string
    default _.Describe() = "base"

exception CustomError of code: int

type internal HiddenRecord = { X: int }

type Mixed() =
    member _.Public = 1
    member internal _.Internal = 2

module internal HiddenModule =
    let value = 1

module Nested =
    /// Inside a nested module
    let helper (x: int) = x + 1

    type Inner = { Value: int }

[<AutoOpen>]
module Auto =
    let inline addThem a b = a + b
"""

/// Anything that throws is rendered rather than swallowed, so the two paths must also agree about that
let private safe (f: unit -> string) =
    try
        f ()
    with e ->
        "<" + e.GetType().Name + ">"

/// The text itself, not just XmlDocSig: the signature file's doc reaches a value through a different
/// field than its own, and only the text shows whether that one survived.
let private describeXmlDoc (doc: FSharpXmlDoc) =
    match doc with
    | FSharpXmlDoc.FromXmlText text -> "doc=" + String.concat " " text.UnprocessedLines
    | FSharpXmlDoc.FromXmlFile(_, xmlSig) -> "docfile=" + xmlSig
    | FSharpXmlDoc.None -> ""

let private describeAccess (a: FSharpAccessibility) =
    if a.IsPublic then "public"
    elif a.IsInternal then "internal"
    elif a.IsPrivate then "private"
    else "?"

let private describeSymbol (s: FSharpSymbol) =
    let ctx = FSharpDisplayContext.Empty

    let details =
        match s with
        | :? FSharpEntity as e ->
            [ describeAccess e.Accessibility
              safe (fun () -> if e.IsFSharpAbbreviation then "abbrev=" + e.AbbreviatedType.Format ctx else "")
              safe (fun () ->
                  match e.BaseType with
                  | Some b -> "base=" + b.Format ctx
                  | None -> "")
              safe (fun () -> e.DeclaredInterfaces |> Seq.map (fun i -> i.Format ctx) |> String.concat ",")
              safe (fun () -> e.XmlDocSig)
              safe (fun () -> describeXmlDoc e.XmlDoc) ]
        | :? FSharpMemberOrFunctionOrValue as v ->
            [ describeAccess v.Accessibility
              safe (fun () -> v.FullType.Format ctx)
              (if v.IsMutable then "mutable" else "")
              safe (fun () -> string v.InlineAnnotation)
              safe (fun () ->
                  match v.LiteralValue with
                  | Some x -> "literal=" + string x
                  | None -> "")
              safe (fun () -> v.XmlDocSig)
              safe (fun () -> describeXmlDoc v.XmlDoc) ]
        | :? FSharpField as f -> [ describeAccess f.Accessibility; safe (fun () -> f.FieldType.Format ctx) ]
        | :? FSharpUnionCase as c ->
            [ describeAccess c.Accessibility
              safe (fun () -> c.Fields |> Seq.map (fun f -> f.FieldType.Format ctx) |> String.concat ",") ]
        | _ -> []

    String.concat "|" (
        [ s.GetType().Name; safe (fun () -> s.FullName); s.DisplayName ]
        @ details
        @ attribsOfSymbol s)

/// Everything a host can see of a referenced project, however this build of it arrived
let private referencedSurface (checker: FSharpChecker) (options: FSharpProjectOptions) name =
    let results = checker.ParseAndCheckProject options |> Async.RunSynchronously

    let assembly =
        results.ProjectContext.GetReferencedAssemblies()
        |> List.find (fun a -> a.SimpleName = name)

    allSymbolsInEntities true assembly.Contents.Entities
    |> List.map describeSymbol
    |> List.sort
    |> Array.ofList

/// The handed-over tree is meant to be what unpickling produces. The other tests pin single properties
/// of that; this one compares the whole surface a host sees, against a checker that cannot share and so
/// unpickles the same project. A field the pruning forgets shows up here and nowhere else.
[<Fact>]
let ``A handed-over project presents the same surface as an unpickled one`` () =
    let surfaceOf share =
        let checker = mkChecker share
        let library = projectOptions checker surfaceSource [] [||]
        let libraryName = Path.GetFileNameWithoutExtension(fst library)

        let _, consumer =
            projectOptions checker (consumerOf "let useIt = Surface.Answer") [ library ] [||]

        Assert.Empty(errorsIn checker consumer)
        referencedSurface checker consumer libraryName

    let unpickled = surfaceOf false
    let handedOver = surfaceOf true

    Assert.Equal<string[]>(unpickled, handedOver)


/// A project with a signature file exports what the signature says, and signature conformance hands the
/// signature's documentation to the implementation as a doc held apart from the value's own. Neither path
/// carries that second doc to a consumer - the comparison below is what says so - but the implementation's
/// own documentation does travel, and the guard keeps this from passing on two empty surfaces.
[<Fact>]
let ``A handed-over project with a signature file presents the same surface as an unpickled one`` () =
    let signatureSource =
        """
module Documented

/// The answer and where it came from
val answer: int

/// Doubles its argument
val twice: x: int -> int
"""

    let implementationSource =
        """
module Documented

let answer = 42

/// Doubles it, said in the implementation
let twice x = x * 2
"""

    let surfaceOf share =
        let checker = mkChecker share

        let files =
            [| writeSourceAs ".fsi" signatureSource
               writeSourceAs ".fs" implementationSource |]

        let library = projectOptionsOfFiles checker files [] [||]
        let libraryName = Path.GetFileNameWithoutExtension(fst library)

        let _, consumer =
            projectOptions checker (consumerOf "let useIt = Documented.answer") [ library ] [||]

        Assert.Empty(errorsIn checker consumer)
        referencedSurface checker consumer libraryName

    let unpickled = surfaceOf false
    let handedOver = surfaceOf true

    Assert.Equal<string[]>(unpickled, handedOver)

    // Both paths losing the doc would satisfy the comparison above and pin nothing
    Assert.Contains(handedOver, fun (s: string) -> s.Contains "Doubles it, said in the implementation")

/// Only the background compiler offers contents in imported form: TransparentCompiler builds the same
/// assembly data with nothing to offer, so its consumers each unpickle a copy. Nothing else says so, and
/// a later change that started offering there would be a silent one.
[<Fact>]
let ``The transparent compiler does not hand contents over`` () =
    let checker = mkCheckerOn true true
    let library = projectOptions checker librarySource [] [||]
    let libraryName = Path.GetFileNameWithoutExtension(fst library)

    let _, first =
        projectOptions checker (consumerOf "let first = Library.publicValue") [ library ] [||]

    let _, second =
        projectOptions checker (consumerOf "let second = Library.publicValue") [ library ] [||]

    Assert.Empty(errorsIn checker first)
    Assert.Empty(errorsIn checker second)
    Assert.NotSame(referencedCcu checker first libraryName, referencedCcu checker second libraryName)

/// Compiles a real assembly, so that what a consumer imports from it comes from pickled bytes on disk
/// rather than from another project of the graph.
let private compileDll (checker: FSharpChecker) name source references =
    let dll = Path.Combine(Path.GetDirectoryName(getTemporaryFileName ()), name + ".dll")

    let args =
        [| yield "fsc.exe"
           yield "--target:library"
           yield "--noframework"
           yield "-o:" + dll
           for r in mkStandardProjectReferences () -> "-r:" + r
           for r in references -> "-r:" + r
           yield writeSource source |]

    let diagnostics, exn = checker.Compile(args) |> Async.RunSynchronously
    Assert.Empty(diagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error))
    Assert.True(exn.IsNone, string exn)
    dll

/// An assembly read from disk carries its signature pickled, and unpickling it resolves the names it
/// mentions against what the consumer has registered. One of those names can be a project whose contents
/// were offered, whose ccu stands delayed until it is bound - and pointing at a delayed one is an error,
/// not a wait. So a consumer that references both an offered project and an assembly built against it
/// used to lose its builder outright: no error, no files, no signature, which is why Errors alone did
/// not see it.
[<Fact>]
let ``A consumer of both an offered project and an assembly built against it still checks`` () =
    let checker = mkChecker true

    let librarySource =
        """
module Shared

type Carried = { Value: int }

let make v = { Value = v }
"""

    // Same source, once as a real assembly for Middle to be built against, and once as a project of the
    // graph under that same assembly name, which is the one the consumer offers to take
    let libraryDll = compileDll checker "Shared" librarySource []

    let middleDll =
        compileDll
            checker
            "Middle"
            """
module Middle

/// Mentions Shared.Carried in its own signature, so unpickling Middle has to resolve "Shared"
let carry (v: int) : Shared.Carried = Shared.make v
"""
            [ libraryDll ]

    let library =
        projectOptionsNamed checker (Some "Shared") [| writeSource librarySource |] [] [||]

    let _, consumer =
        projectOptionsNamed
            checker
            None
            [| writeSource "module Consumer\nlet used = Middle.carry 1\n" |]
            [ library ]
            [| "-r:" + middleDll |]

    let results = checker.ParseAndCheckProject consumer |> Async.RunSynchronously

    Assert.Empty(results.Diagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error))

    // The builder failing leaves no error behind, so the signature is what says it ran at all
    Assert.NotEmpty(results.AssemblySignature.Entities)
    Assert.NotEmpty(results.ProjectContext.GetReferencedAssemblies())
