namespace FSharp.Compiler.Service

open System.Collections.Immutable
open FSharp.Compiler
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.Ast
open FSharp.Compiler.CompileOps
open FSharp.Compiler.TcGlobals

type internal ParsingOptions =
    {
        isExecutable: bool
        lexResourceManager: Lexhelp.LexResourceManager
    }

type internal IncrementalCheckerOptions =
    {
        keepAssemblyContents: bool
        keepAllBackgroundResolutions: bool
        parsingOptions: ParsingOptions
    }

/// This is immutable.
/// Its job is to do either full type checking on a collection of sources or to do the least amount of work checking a source to get a check result.
/// Checks are also cached.
[<Sealed>]
type internal IncrementalChecker =

    member Version: VersionStamp

    member AddSources: sources: ImmutableArray<Source> -> IncrementalChecker

    member ReplaceSource: source: Source -> IncrementalChecker

type internal InitialInfo =
    {
        ctok: CompilationThreadToken
        tcConfig: TcConfig
        tcConfigP: TcConfigProvider
        tcGlobals: TcGlobals
        frameworkTcImports: TcImports
        nonFrameworkResolutions: AssemblyResolution list
        unresolvedReferences: UnresolvedAssemblyReference list
        importsInvalidated: Event<string>
        assemblyName: string
        niceNameGen: NiceNameGenerator
        loadClosureOpt: LoadClosure option
        projectDirectory: string
        checkerOptions: IncrementalCheckerOptions
    }

module internal IncrementalChecker =

    val Create: InitialInfo -> Cancellable<IncrementalChecker>    