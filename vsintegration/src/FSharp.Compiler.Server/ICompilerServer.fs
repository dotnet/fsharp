namespace Microsoft.FSharp.Compiler.Server

open System

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices

[<RequireQualifiedAccess>]
type Range =
    {
        StartLine: int
        StartColumn: int
        EndLine: int
        EndColumn: int
    }

    static member FromFSharpRange(m: range) =
        {
            StartLine = m.StartLine
            StartColumn = m.StartColumn
            EndLine = m.EndLine
            EndColumn = m.EndColumn
        }

    member this.ToFSharpRange(filePath) =
        let start = mkPos this.StartLine this.StartColumn
        let en = mkPos this.EndLine this.EndColumn
        mkRange filePath start en

type ErrorSeverity =
    | Warning = 0
    | Error = 1

[<RequireQualifiedAccess>]
type ErrorSeverityOptions =
    {
        WarnLevel: int
        GlobalWarnAsError: bool
        WarnOff: int []
        WarnOn: int []
        WarnAsError: int []
        WarnAsWarn: int []
    }

    static member FromFSharpErrorSeverityOptions(fsErrorSevOpt: FSharpErrorSeverityOptions) : ErrorSeverityOptions =
        {
            WarnLevel = fsErrorSevOpt.WarnLevel
            GlobalWarnAsError = fsErrorSevOpt.GlobalWarnAsError
            WarnOff = fsErrorSevOpt.WarnOff |> Array.ofList
            WarnOn = fsErrorSevOpt.WarnOn |> Array.ofList
            WarnAsError = fsErrorSevOpt.WarnAsError |> Array.ofList
            WarnAsWarn = fsErrorSevOpt.WarnAsWarn |> Array.ofList
        }

    member this.ToFSharpErrorSeverityOptions() : FSharpErrorSeverityOptions =
        {
            WarnLevel = this.WarnLevel
            GlobalWarnAsError = this.GlobalWarnAsError
            WarnOff = this.WarnOff |> List.ofArray
            WarnOn = this.WarnOn |> List.ofArray
            WarnAsError = this.WarnAsError |> List.ofArray
            WarnAsWarn = this.WarnAsWarn |> List.ofArray
        }

[<RequireQualifiedAccess>]
type ErrorInfo =
    {
        Number: int
        Severity: ErrorSeverity
        Subcategory: string
        Message: string

        StartLineAlternate: int
        StartColumn: int
        EndLineAlternate: int
        EndColumn: int
    }

    static member FromFSharpErrorInfo(fsErrorInfo: FSharpErrorInfo) =
        {
            Number = fsErrorInfo.ErrorNumber
            Severity =
                match fsErrorInfo.Severity with
                | FSharpErrorSeverity.Warning -> ErrorSeverity.Warning
                | FSharpErrorSeverity.Error -> ErrorSeverity.Error
            Subcategory = fsErrorInfo.Subcategory
            Message = fsErrorInfo.Message

            StartLineAlternate = fsErrorInfo.StartLineAlternate
            StartColumn = fsErrorInfo.StartColumn
            EndLineAlternate = fsErrorInfo.EndLineAlternate
            EndColumn = fsErrorInfo.EndColumn
        }

[<RequireQualifiedAccess>]
type ProjectOptions =
    {
        ProjectFileName: string
        ProjectId: string option
        SourceFiles: string []
        OtherOptions: string []
        ReferencedProjects: (string * ProjectOptions) []
        IsIncompleteTypeCheckEnvironment : bool
        UseScriptResolutionRules : bool      
        LoadTime : DateTime
        Stamp : int64 option
    }

    static member FromFSharpProjectOptions(options: FSharpProjectOptions) =
        {
            ProjectFileName = options.ProjectFileName
            ProjectId = options.ProjectId
            SourceFiles = options.SourceFiles
            OtherOptions = options.OtherOptions
            ReferencedProjects = options.ReferencedProjects |> Array.map (fun (x, o) -> (x, ProjectOptions.FromFSharpProjectOptions(o)))
            IsIncompleteTypeCheckEnvironment = options.IsIncompleteTypeCheckEnvironment
            UseScriptResolutionRules = options.UseScriptResolutionRules
            LoadTime = options.LoadTime
            Stamp = options.Stamp
        }

    member this.ToFSharpProjectOptions() =
        {
            ProjectFileName = this.ProjectFileName
            ProjectId = this.ProjectId
            SourceFiles = this.SourceFiles
            OtherOptions = this.OtherOptions
            ReferencedProjects = this.ReferencedProjects |> Array.map (fun (x, o) -> (x, o.ToFSharpProjectOptions()))
            IsIncompleteTypeCheckEnvironment = this.IsIncompleteTypeCheckEnvironment
            UseScriptResolutionRules = this.UseScriptResolutionRules
            LoadTime = this.LoadTime
            UnresolvedReferences = None
            OriginalLoadReferences = []
            ExtraProjectInfo = None
            Stamp = this.Stamp
        }

[<RequireQualifiedAccess>]
type ParsingOptions =
    {
        SourceFiles: string []
        ConditionalCompilationDefines: string []
        ErrorSeverityOptions: ErrorSeverityOptions
        IsInteractive: bool
        LightSyntax: bool option
        CompilingFsLib: bool
        IsExe: bool
    }

    static member FromFSharpParsingOptions(options: FSharpParsingOptions) =
        {
            SourceFiles = options.SourceFiles
            ConditionalCompilationDefines = options.ConditionalCompilationDefines |> Array.ofList
            ErrorSeverityOptions = ErrorSeverityOptions.FromFSharpErrorSeverityOptions(options.ErrorSeverityOptions)
            IsInteractive = options.IsInteractive
            LightSyntax = options.LightSyntax
            CompilingFsLib = options.CompilingFsLib
            IsExe = options.IsExe
        }

    member this.ToFSharpParsingOptions() : FSharpParsingOptions =
        {
            SourceFiles = this.SourceFiles
            ConditionalCompilationDefines = this.ConditionalCompilationDefines |> List.ofArray
            ErrorSeverityOptions = this.ErrorSeverityOptions.ToFSharpErrorSeverityOptions()
            IsInteractive = this.IsInteractive
            LightSyntax = this.LightSyntax
            CompilingFsLib = this.CompilingFsLib
            IsExe = this.IsExe
        }

[<RequireQualifiedAccess>]
type CheckerOptions =
    {
        FilePath: string
        TextVersionHash: int
        SourceText: string
        ProjectOptions: ProjectOptions
        UserOpName: string option
    }

type SemanticClassificationItemType =
    | ReferenceType = 0
    | ValueType = 1
    | UnionCase = 2
    | Function = 3
    | Property = 4
    | MutableVar = 5
    | Module = 6
    | Printf = 7
    | ComputationExpression = 8
    | IntrinsicFunction = 9
    | Enumeration = 10
    | Interface = 11
    | TypeArgument = 12
    | Operator = 13
    | Disposable = 14
    
[<RequireQualifiedAccess>]
type SemanticClassificationItem =
    {
        Range: Range
        Type: SemanticClassificationItemType
    }

    static member FromFSharp(m: range, typ: SemanticClassificationType) =
        {
            Range = Range.FromFSharpRange(m)
            Type =
                match typ with
                | SemanticClassificationType.ReferenceType -> SemanticClassificationItemType.ReferenceType
                | SemanticClassificationType.ValueType -> SemanticClassificationItemType.ValueType
                | SemanticClassificationType.UnionCase -> SemanticClassificationItemType.UnionCase
                | SemanticClassificationType.Function -> SemanticClassificationItemType.Function
                | SemanticClassificationType.Property -> SemanticClassificationItemType.Property
                | SemanticClassificationType.MutableVar -> SemanticClassificationItemType.MutableVar
                | SemanticClassificationType.Module -> SemanticClassificationItemType.Module
                | SemanticClassificationType.Printf -> SemanticClassificationItemType.Printf
                | SemanticClassificationType.ComputationExpression -> SemanticClassificationItemType.ComputationExpression
                | SemanticClassificationType.IntrinsicFunction -> SemanticClassificationItemType.IntrinsicFunction
                | SemanticClassificationType.Enumeration -> SemanticClassificationItemType.Enumeration
                | SemanticClassificationType.Interface -> SemanticClassificationItemType.Interface
                | SemanticClassificationType.TypeArgument -> SemanticClassificationItemType.TypeArgument
                | SemanticClassificationType.Operator -> SemanticClassificationItemType.Operator
                | SemanticClassificationType.Disposable -> SemanticClassificationItemType.Disposable
        }

type GetErrorInfosCommandType =
    | Syntax = 0
    | Semantic = 1

[<RequireQualifiedAccess>]
module Command =

    type GetErrorInfos =
        {
            ParsingOptions: ParsingOptions
            CheckerOptions: CheckerOptions
            Type: GetErrorInfosCommandType
        }

        static member Create(parsingOptions, checkerOptions, typ) =
            {
                ParsingOptions = parsingOptions
                CheckerOptions = checkerOptions
                Type = typ
            }

[<RequireQualifiedAccess>]
module CommandResult =

    type GetErrorInfos =
        {
            Items: ErrorInfo []
        }

        static member Create(items) =
            {
                Items = items
            }

type ICompilerServer =
    inherit IDisposable

    abstract GetSemanticClassificationAsync : checkerOptions: CheckerOptions * classifyRange: Range -> Async<SemanticClassificationItem []>

    abstract GetErrorInfosAsync : Command.GetErrorInfos -> Async<CommandResult.GetErrorInfos option>