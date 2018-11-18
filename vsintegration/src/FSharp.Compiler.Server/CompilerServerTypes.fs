namespace Microsoft.FSharp.Compiler.Server

[<RequireQualifiedAccess>]
type CompilerCommand =
    | GetSemanticClassification of checkerOptions: CheckerOptions * range: Range
    | GetErrorInfos of Command.GetErrorInfos

[<RequireQualifiedAccess>]
type CompilerResult =
    | GetSemanticClassification of SemanticClassificationItem []
    | GetErrorInfosResult of CommandResult.GetErrorInfos option
