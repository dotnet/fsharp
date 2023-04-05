namespace FSharp.Compiler.CodeAnalysis


type internal TransparentCompiler() =

    interface IBackgroundCompiler with
        member this.BeforeBackgroundFileCheck: IEvent<string * FSharpProjectOptions> =
            raise (System.NotImplementedException())
        member this.CheckFileInProject(parseResults: FSharpParseFileResults, fileName: string, fileVersion: int, sourceText: FSharp.Compiler.Text.ISourceText, options: FSharpProjectOptions, userOpName: string): FSharp.Compiler.BuildGraph.NodeCode<FSharpCheckFileAnswer> =
            raise (System.NotImplementedException())
        member this.CheckFileInProjectAllowingStaleCachedResults(parseResults: FSharpParseFileResults, fileName: string, fileVersion: int, sourceText: FSharp.Compiler.Text.ISourceText, options: FSharpProjectOptions, userOpName: string): FSharp.Compiler.BuildGraph.NodeCode<FSharpCheckFileAnswer option> =
            raise (System.NotImplementedException())
        member this.ClearCache(options: seq<FSharpProjectOptions>, userOpName: string): unit =
            raise (System.NotImplementedException())
        member this.ClearCaches(): unit =
            raise (System.NotImplementedException())
        member this.DownsizeCaches(): unit =
            raise (System.NotImplementedException())
        member this.FileChecked: IEvent<string * FSharpProjectOptions> =
            raise (System.NotImplementedException())
        member this.FileParsed: IEvent<string * FSharpProjectOptions> =
            raise (System.NotImplementedException())
        member this.FindReferencesInFile(fileName: string, options: FSharpProjectOptions, symbol: FSharp.Compiler.Symbols.FSharpSymbol, canInvalidateProject: bool, userOpName: string): FSharp.Compiler.BuildGraph.NodeCode<seq<FSharp.Compiler.Text.range>> =
            raise (System.NotImplementedException())
        member this.FrameworkImportsCache: FrameworkImportsCache =
            raise (System.NotImplementedException())
        member this.GetAssemblyData(options: FSharpProjectOptions, userOpName: string): FSharp.Compiler.BuildGraph.NodeCode<FSharp.Compiler.CompilerConfig.ProjectAssemblyDataResult> =
            raise (System.NotImplementedException())
        member this.GetBackgroundCheckResultsForFileInProject(fileName: string, options: FSharpProjectOptions, userOpName: string): FSharp.Compiler.BuildGraph.NodeCode<FSharpParseFileResults * FSharpCheckFileResults> =
            raise (System.NotImplementedException())
        member this.GetBackgroundParseResultsForFileInProject(fileName: string, options: FSharpProjectOptions, userOpName: string): FSharp.Compiler.BuildGraph.NodeCode<FSharpParseFileResults> =
            raise (System.NotImplementedException())
        member this.GetCachedCheckFileResult(builder: IncrementalBuilder, fileName: string, sourceText: FSharp.Compiler.Text.ISourceText, options: FSharpProjectOptions): FSharp.Compiler.BuildGraph.NodeCode<(FSharpParseFileResults * FSharpCheckFileResults) option> =
            raise (System.NotImplementedException())
        member this.GetProjectOptionsFromScript(fileName: string, sourceText: FSharp.Compiler.Text.ISourceText, previewEnabled: bool option, loadedTimeStamp: System.DateTime option, otherFlags: string array option, useFsiAuxLib: bool option, useSdkRefs: bool option, sdkDirOverride: string option, assumeDotNetFramework: bool option, optionsStamp: int64 option, userOpName: string): Async<FSharpProjectOptions * FSharp.Compiler.Diagnostics.FSharpDiagnostic list> =
            raise (System.NotImplementedException())
        member this.GetSemanticClassificationForFile(fileName: string, options: FSharpProjectOptions, userOpName: string): FSharp.Compiler.BuildGraph.NodeCode<FSharp.Compiler.EditorServices.SemanticClassificationView option> =
            raise (System.NotImplementedException())
        member this.InvalidateConfiguration(options: FSharpProjectOptions, userOpName: string): unit =
            raise (System.NotImplementedException())
        member this.NotifyFileChanged(fileName: string, options: FSharpProjectOptions, userOpName: string): FSharp.Compiler.BuildGraph.NodeCode<unit> =
            raise (System.NotImplementedException())
        member this.NotifyProjectCleaned(options: FSharpProjectOptions, userOpName: string): Async<unit> =
            raise (System.NotImplementedException())
        member this.ParseAndCheckFileInProject(fileName: string, fileVersion: int, sourceText: FSharp.Compiler.Text.ISourceText, options: FSharpProjectOptions, userOpName: string): FSharp.Compiler.BuildGraph.NodeCode<FSharpParseFileResults * FSharpCheckFileAnswer> =
            raise (System.NotImplementedException())
        member this.ParseAndCheckProject(options: FSharpProjectOptions, userOpName: string): FSharp.Compiler.BuildGraph.NodeCode<FSharpCheckProjectResults> =
            raise (System.NotImplementedException())
        member this.ParseFile(fileName: string, sourceText: FSharp.Compiler.Text.ISourceText, options: FSharpParsingOptions, cache: bool, userOpName: string): Async<FSharpParseFileResults> =
            raise (System.NotImplementedException())
        member this.ProjectChecked: IEvent<FSharpProjectOptions> =
            raise (System.NotImplementedException())
        member this.TryGetRecentCheckResultsForFile(fileName: string, options: FSharpProjectOptions, sourceText: FSharp.Compiler.Text.ISourceText option, userOpName: string): (FSharpParseFileResults * FSharpCheckFileResults * SourceTextHash) option =
            raise (System.NotImplementedException())