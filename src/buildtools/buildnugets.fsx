open System.IO
open System.Diagnostics

let Args = fsi.CommandLineArgs |> Array.skip 1
let usage = @"usage: BuildNuGets.fsx <build-version> <nuspec-path> <binaries-dir> <output-directory>";

if Args.Length <> 4 then
    printfn "%s" usage
    exit(1)

let version = Args.[0]
let nuspec = Path.GetFullPath(Args.[1]).Trim('\\')
let bindir = Path.GetFullPath(Args.[2]).Trim('\\')
let outdir = Path.GetFullPath(Args.[3]).Trim('\\')

let author =     @"Microsoft";
let licenseUrl = @"https://github.com/Microsoft/visualfsharp/blob/master/License.txt";
let projectUrl = @"https://github.com/Microsoft/visualfsharp";
let tags =       @"Visual F# Compiler FSharp coreclr functional programming";

let nugetArgs = sprintf "pack %s -BasePath \"%s\" -OutputDirectory \"%s\" -ExcludeEmptyDirectories -prop licenseUrl=\"%s\" -prop version=\"%s\" -prop authors=\"%s\" -prop projectURL=\"%s\" -prop tags=\"%s\""
                        nuspec
                        bindir
                        outdir
                        licenseUrl
                        version
                        author
                        projectUrl
                        tags

let nugetExePath = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, @"..\..\.nuget\nuget.exe"))
let executeProcess filename arguments =
    let processWriteMessage (chan:TextWriter) (message:string) =
        match message with
        | null -> ()
        | _ as m -> chan.WriteLine(m) |>ignore
    let info = new ProcessStartInfo()
    let p = new Process()
    printfn "%s %s" filename arguments
    info.Arguments <- arguments
    info.UseShellExecute <- false
    info.RedirectStandardOutput <- true
    info.RedirectStandardError <- true
    info.CreateNoWindow <- true
    info.FileName <- filename
    p.StartInfo <- info
    p.OutputDataReceived.Add(fun x -> processWriteMessage stdout x.Data)
    p.ErrorDataReceived.Add(fun x ->  processWriteMessage stderr x.Data)
    if p.Start() then
        p.BeginOutputReadLine()
        p.BeginErrorReadLine()
        p.WaitForExit()
        p.ExitCode
    else
        0

exit (executeProcess nugetExePath nugetArgs) 
