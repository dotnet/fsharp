namespace FSharp.Compiler.SourceCodeServices.ProjectCrackerTool

open System
open System.Reflection
open System.Runtime.Serialization.Json

module Program =

#if !NETCOREAPP
    let addMSBuildv14BackupResolution () =
        let onResolveEvent = new ResolveEventHandler(fun sender evArgs ->
            let requestedAssembly = AssemblyName(evArgs.Name)
            if requestedAssembly.Name.StartsWith("Microsoft.Build", StringComparison.Ordinal) &&
                not (requestedAssembly.Name.EndsWith(".resources", StringComparison.Ordinal)) && 
                not (requestedAssembly.Version.ToString().Contains("12.0.0.0")) 
            then
                // If the version of MSBuild that we're using wasn't present on the machine, then 
                // just revert back to 12.0.0.0 since that's normally installed as part of the .NET 
                // Framework.
                requestedAssembly.Version <- Version("12.0.0.0")
                Assembly.Load requestedAssembly
            else
                null)
        AppDomain.CurrentDomain.add_AssemblyResolve(onResolveEvent)
#endif

    let crackAndSendOutput asText argv =
        let ret, opts = ProjectCrackerTool.crackOpen argv

        if asText then
            printfn "%A" opts
        else
            let ser = new DataContractJsonSerializer(typeof<ProjectOptions>)
            ser.WriteObject(Console.OpenStandardOutput(), opts)
        ret


    [<EntryPoint>][<STAThread>]
    let main argv =
        let asText = Array.exists (fun (s: string) -> s = "--text") argv
        let argv = Array.filter (fun (s: string) -> s <> "--text") argv

#if !NETCOREAPP
        addMSBuildv14BackupResolution ()
#endif
        crackAndSendOutput asText argv
