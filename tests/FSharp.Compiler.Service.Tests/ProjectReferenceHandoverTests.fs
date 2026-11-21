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

// Only the background compiler hands contents over, so the compiler is pinned rather than taken from the suite default
let private mkCheckerOn transparent share =
    FSharpChecker.Create(
        // the default of three evicts builders in these graphs, and a rebuilt builder offers afresh, which reads as a refusal
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

/// Options for one project of the given files, referencing the given built ones, output under the given
/// assembly name when one is given
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

let private assemblyNameOf (dll: string, _: FSharpProjectOptions) = Path.GetFileNameWithoutExtension dll

let private errorsOf (diagnostics: FSharpDiagnostic[]) =
    diagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)

let private checkProject (checker: FSharpChecker) (options: FSharpProjectOptions) =
    checker.ParseAndCheckProject options |> Async.RunSynchronously

let private errorsIn checker options = errorsOf (checkProject checker options).Diagnostics

let private referencedAssembly (checkResults: FSharpCheckProjectResults) name =
    checkResults.ProjectContext.GetReferencedAssemblies()
    |> List.find (fun assembly -> assembly.SimpleName = name)

/// The contents of the ccu behind an FSharpAssembly, which does not expose it
let private ccuOf (assembly: FSharpAssembly) =
    assembly.GetType().GetFields(BindingFlags.Instance ||| BindingFlags.NonPublic ||| BindingFlags.Public)
    |> Array.pick (fun field ->
        match field.GetValue assembly with
        // each reader wraps the one imported form in a thunk of its own
        | :? CcuThunk as ccu -> Some ccu.Contents
        | _ -> None)

/// The ccu the given project ended up with for one of its references
let private referencedCcu checker options name =
    referencedAssembly (checkProject checker options) name |> ccuOf

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

let private useInternalValue = "let useValue = Library.secretValue"

let private otherLayerOptions = [| "--langversion:9.0"; "--checknulls+" |]

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
    let libraryName = assemblyNameOf library

    let _, sameLayer =
        projectOptions checker (consumerOf "let usePublic = Library.publicValue + 1") [ library ] [||]

    // reading nullable-reference metadata keys another import layer
    let otherLayer body =
        projectOptions checker (consumerOf body) [ library ] otherLayerOptions |> snd

    let firstOnOtherLayer = otherLayer "let useOnce = Library.publicValue + 1"
    let secondOnOtherLayer = otherLayer "let useTwice = Library.publicValue + 2"

    Assert.Empty(errorsIn checker sameLayer)
    Assert.Empty(errorsIn checker firstOnOtherLayer)

    Assert.Same(
        referencedCcu checker firstOnOtherLayer libraryName,
        referencedCcu checker secondOnOtherLayer libraryName
    )

    // the two layers disagree about FSharp.Core, so one copy cannot serve both
    Assert.NotSame(referencedCcu checker sameLayer libraryName, referencedCcu checker firstOnOtherLayer libraryName)

[<Fact>]
let ``An internal is not visible to a consumer on another layer either`` () =
    let checker = mkChecker true
    let library = projectOptions checker librarySource [] [||]

    let _, otherLayer =
        projectOptions checker (consumerOf useInternalValue) [ library ] otherLayerOptions

    Assert.NotEmpty(errorsIn checker otherLayer)

[<Fact>]
let ``An internal of a handed-over project is visible to the consumers its InternalsVisibleTo names`` () =
    let checker = mkChecker true

    let library =
        projectOptions
            checker
            """
module Library

[<assembly: System.Runtime.CompilerServices.InternalsVisibleTo "NamedConsumer">]
[<assembly: System.Runtime.CompilerServices.InternalsVisibleTo "NamedConsumerOnOtherLayer">]
do ()

let internal secretValue = 2
"""
            []
            [||]

    let consumerNamed name extraOptions =
        projectOptionsNamed checker (Some name) [| writeSource (consumerOf useInternalValue) |] [ library ] extraOptions
        |> snd

    let named = consumerNamed "NamedConsumer" [||]
    let namedOnOtherLayer = consumerNamed "NamedConsumerOnOtherLayer" otherLayerOptions
    let unnamed = consumerNamed "UnnamedConsumer" [||]

    Assert.Empty(errorsIn checker named)
    Assert.Empty(errorsIn checker namedOnOtherLayer)
    Assert.NotEmpty(errorsIn checker unnamed)

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

    // a build passes project references on transitively
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

    Assert.Empty(errorsIn checker consumer)

[<Theory>]
[<InlineData true>]
[<InlineData false>]
let ``Consumers share one imported ccu only when contents are handed over`` share =
    let checker = mkChecker share
    let library = projectOptions checker librarySource [] [||]
    let libraryName = assemblyNameOf library

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

[<Fact>]
let ``Projects on different language versions share one framework import`` () =
    let checker = mkChecker true

    let ccuOfFSharpCore extraOptions =
        let _, options = projectOptions checker (consumerOf "let value = 1") [] extraOptions
        referencedCcu checker options "FSharp.Core"

    Assert.Same(ccuOfFSharpCore [| "--langversion:8.0" |], ccuOfFSharpCore [| "--langversion:9.0" |])

[<Fact>]
let ``Consumers differing only in realsig share one imported ccu`` () =
    let checker = mkChecker true
    let library = projectOptions checker librarySource [] [| "--realsig+" |]
    let libraryName = assemblyNameOf library

    let _, withoutRealsig =
        projectOptions checker (consumerOf "let first = Library.publicValue") [ library ] [||]

    let _, withRealsig =
        projectOptions checker (consumerOf "let second = Library.publicValue") [ library ] [| "--realsig+" |]

    Assert.Empty(errorsIn checker withoutRealsig)
    Assert.Empty(errorsIn checker withRealsig)
    Assert.Same(referencedCcu checker withoutRealsig libraryName, referencedCcu checker withRealsig libraryName)

[<Fact>]
let ``A project reached twice through a diamond is still taken`` () =
    let checker = mkChecker true
    let leaf = projectOptions checker "module Leaf\nlet leaf = 1\n" [] [||]
    let left = projectOptions checker "module Left\nlet left = Leaf.leaf\n" [ leaf ] [||]
    let right = projectOptions checker "module Right\nlet right = Leaf.leaf\n" [ leaf ] [||]

    let join =
        projectOptions checker "module Join\nlet join = Left.left + Right.right\n" [ left; right; leaf ] [||]

    let joinName = assemblyNameOf join
    let references = [ join; left; right; leaf ]

    let _, first = projectOptions checker "module First\nlet x = Join.join\n" references [||]
    let _, second = projectOptions checker "module Second\nlet y = Join.join\n" references [||]

    Assert.Same(referencedCcu checker first joinName, referencedCcu checker second joinName)

[<Fact>]
let ``A dependency the consumer does not have does not refuse the offer`` () =
    let checker = mkChecker true

    // outside the standard reference set, so only the library imports anything of this name
    let privateDependency = "-r:" + typeof<FactAttribute>.Assembly.Location

    let library = projectOptions checker librarySource [] [| privateDependency |]
    let libraryName = assemblyNameOf library

    let _, first =
        projectOptions checker (consumerOf "let first = Library.publicValue") [ library ] [||]

    let _, second =
        projectOptions checker (consumerOf "let second = Library.publicValue") [ library ] [||]

    Assert.Empty(errorsIn checker first)
    Assert.Same(referencedCcu checker first libraryName, referencedCcu checker second libraryName)

[<Fact>]
let ``Consumers checked at once share one imported ccu`` () =
    let checker = mkChecker true
    let library = projectOptions checker librarySource [] [||]
    let libraryName = assemblyNameOf library

    let consumers =
        [ for i in 1..8 ->
            projectOptions checker (consumerOf $"let use%d{i} = Library.publicValue") [ library ] [||]
            |> snd ]

    let checkResults =
        consumers
        |> List.map checker.ParseAndCheckProject
        |> Async.Parallel
        |> Async.RunSynchronously

    for results in checkResults do
        Assert.Empty(errorsOf results.Diagnostics)

    let ccus =
        checkResults
        |> Array.map (fun results -> referencedAssembly results libraryName |> ccuOf)

    for ccu in ccus do
        Assert.Same(ccus[0], ccu)

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

/// Renders what the getter throws instead of swallowing it, so the two paths must agree on that too
let private safe (f: unit -> string) =
    try
        f ()
    with e ->
        "<" + e.GetType().Name + ">"

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

/// Everything a host can see of one referenced project
let private referencedSurface checker options name =
    let assembly = referencedAssembly (checkProject checker options) name

    allSymbolsInEntities true assembly.Contents.Entities
    |> List.map describeSymbol
    |> List.sort
    |> Array.ofList

[<Fact>]
let ``A handed-over project presents the same surface as an unpickled one`` () =
    let surfaceOf share =
        let checker = mkChecker share
        let library = projectOptions checker surfaceSource [] [||]

        let _, consumer =
            projectOptions checker (consumerOf "let useIt = Surface.Answer") [ library ] [||]

        Assert.Empty(errorsIn checker consumer)
        referencedSurface checker consumer (assemblyNameOf library)

    let unpickled = surfaceOf false
    let handedOver = surfaceOf true

    Assert.Equal<string[]>(unpickled, handedOver)

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

        let _, consumer =
            projectOptions checker (consumerOf "let useIt = Documented.answer") [ library ] [||]

        Assert.Empty(errorsIn checker consumer)
        referencedSurface checker consumer (assemblyNameOf library)

    let unpickled = surfaceOf false
    let handedOver = surfaceOf true

    Assert.Equal<string[]>(unpickled, handedOver)

    // both paths losing the doc would satisfy the comparison above and pin nothing
    Assert.Contains(handedOver, fun (s: string) -> s.Contains "Doubles it, said in the implementation")

[<Fact>]
let ``The transparent compiler does not hand contents over`` () =
    let checker = mkCheckerOn true true
    let library = projectOptions checker librarySource [] [||]
    let libraryName = assemblyNameOf library

    let _, first =
        projectOptions checker (consumerOf "let first = Library.publicValue") [ library ] [||]

    let _, second =
        projectOptions checker (consumerOf "let second = Library.publicValue") [ library ] [||]

    Assert.Empty(errorsIn checker first)
    Assert.Empty(errorsIn checker second)
    Assert.NotSame(referencedCcu checker first libraryName, referencedCcu checker second libraryName)

/// Compiles the source to an assembly on disk under the given name and returns its path
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
    Assert.Empty(errorsOf diagnostics)
    Assert.True(exn.IsNone, string exn)
    dll

[<Fact>]
let ``A consumer of both an offered project and an assembly built against it still checks`` () =
    let checker = mkChecker true

    let sharedSource =
        """
module Shared

type Carried = { Value: int }

let make v = { Value = v }
"""

    // the same source as a real assembly for Middle to build against, and as a project under that assembly name
    let sharedDll = compileDll checker "Shared" sharedSource []

    let middleDll =
        compileDll
            checker
            "Middle"
            """
module Middle

let carry (v: int) : Shared.Carried = Shared.make v
"""
            [ sharedDll ]

    let shared =
        projectOptionsNamed checker (Some "Shared") [| writeSource sharedSource |] [] [||]

    let _, consumer =
        projectOptionsNamed
            checker
            None
            [| writeSource "module Consumer\nlet used = Middle.carry 1\n" |]
            [ shared ]
            [| "-r:" + middleDll |]

    let checkResults = checkProject checker consumer

    Assert.Empty(errorsOf checkResults.Diagnostics)

    // the builder failing leaves no error behind, so the signature is what says it ran at all
    Assert.NotEmpty(checkResults.AssemblySignature.Entities)
    Assert.NotEmpty(checkResults.ProjectContext.GetReferencedAssemblies())
