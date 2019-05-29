namespace FSharp.Compiler.Compilation

open FSharp.Compiler
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.Ast
open FSharp.Compiler.CompileOps
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.AbstractIL.ILBinaryReader

[<NoEquality; NoComparison>]
type internal TcInitialOptions =
    {
        frameworkTcImportsCache: FrameworkImportsCache
        legacyReferenceResolver: ReferenceResolver.Resolver
        defaultFSharpBinariesDir: string
        tryGetMetadataSnapshot: ILReaderTryGetMetadataSnapshot
        suggestNamesForErrors: bool
        sourceFiles: string list
        commandLineArgs: string list
        projectDirectory: string
        projectReferences: IProjectReference list
        useScriptResolutionRules: bool
        assemblyPath: string
        isExecutable: bool
        keepAssemblyContents: bool
        keepAllBackgroundResolutions: bool
    }

/// This represents the initial data and info necessary to build a checker.
[<NoEquality; NoComparison>]
type internal TcInitial =
    {
        tcConfig: TcConfig
        tcConfigP: TcConfigProvider
        tcGlobals: TcGlobals
        frameworkTcImports: TcImports
        nonFrameworkResolutions: AssemblyResolution list
        unresolvedReferences: UnresolvedAssemblyReference list
        importsInvalidated: Event<string>
        assemblyName: string
        outfile: string
        niceNameGen: NiceNameGenerator
        loadClosureOpt: LoadClosure option
        projectDirectory: string
    }

[<RequireQualifiedAccess>]
module internal TcInitial =

    val create: TcInitialOptions -> CompilationThreadToken -> TcInitial