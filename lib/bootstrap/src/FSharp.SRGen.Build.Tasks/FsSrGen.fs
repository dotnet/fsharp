
namespace Microsoft.FSharp.Build

open System
open Microsoft.Build.Framework
open Microsoft.Build.Utilities

type FsSrGen() =
    inherit ToolTask()
    let mutable inputFile  : string = null
    let mutable toolPath : string = null
    let mutable outputFsFile : string = null
    let mutable outputResxFile : string = null
    [<Required>]
    member this.InputFile
        with get ()  = inputFile
        and  set (s) = inputFile <- s
    [<Required>]
    [<Output>]
    member this.OutputFsFile
        with get ()  = outputFsFile
        and  set (s) = outputFsFile <- s
    [<Required>]
    [<Output>]
    member this.OutputResxFile
        with get ()  = outputResxFile
        and  set (s) = outputResxFile <- s

    // For targeting other versions
    member this.ToolPath
        with get ()  = toolPath
        and  set (s) = toolPath <- s
        
    // ToolTask methods
    override this.ToolName = "fssrgen.exe"

    override this.GenerateFullPathToTool() =
        System.IO.Path.Combine(toolPath, this.ToolExe)

    override this.GenerateCommandLineCommands() =
        let builder = new CommandLineBuilder()
        builder.AppendSwitchIfNotNull(" ", inputFile)
        builder.AppendSwitchIfNotNull(" ", outputFsFile)
        builder.AppendSwitchIfNotNull(" ", outputResxFile)
        let args = builder.ToString()
        args
