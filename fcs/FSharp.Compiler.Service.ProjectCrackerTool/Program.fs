namespace Microsoft.FSharp.Compiler.SourceCodeServices.ProjectCrackerTool

open System
open System.IO
open System.Runtime.Serialization.Json

module Program =

    [<EntryPoint>]
    let main argv =
        let text = Array.exists (fun (s: string) -> s = "--text") argv
        let argv = Array.filter (fun (s: string) -> s <> "--text") argv

        let ret, opts = ProjectCrackerTool.crackOpen argv

        if text then
            printfn "%A" opts
        else
            let ser = new DataContractJsonSerializer(typeof<ProjectOptions>)
            ser.WriteObject(Console.OpenStandardOutput(), opts)
        ret
