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
        importsInvalidated: Event<string>
        assemblyName: string
        outfile: string
        niceNameGen: NiceNameGenerator
        loadClosureOpt: LoadClosure option
    }

[<RequireQualifiedAccess>]
module internal TcInitial =

    val create: TcInitialOptions -> TcInitial