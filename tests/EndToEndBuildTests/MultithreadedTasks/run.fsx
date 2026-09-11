open System
open System.Diagnostics
open System.IO
open System.Reflection.Metadata
open System.Reflection.PortableExecutable
open System.Runtime.InteropServices
open System.Security
open System.Security.Cryptography
open System.Text.RegularExpressions

let args = fsi.CommandLineArgs |> Array.skip 1
let option name fallback =
    match Array.tryFindIndex ((=) name) args with
    | Some index when index + 1 < args.Length -> args[index + 1]
    | _ -> fallback

let configuration = option "--configuration" "Debug"
let repetitions = int (option "--repetitions" "3")
let projectCount = int (option "--projects" "16")
if projectCount < 4 || repetitions < 1 then failwith "Require --projects >= 4 and --repetitions >= 1"
let repo = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "../../.."))
let dotnet = Path.Combine(repo, ".dotnet", if OperatingSystem.IsWindows() then "dotnet.exe" else "dotnet")
let root = Path.Combine(repo, "artifacts", "MultithreadedTasks", DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff"))
let cwd = Path.Combine(root, "unrelated-cwd")
Directory.CreateDirectory cwd |> ignore
let write (path: string) (text: string) =
    Directory.CreateDirectory(Path.GetDirectoryName path) |> ignore
    File.WriteAllText(path, text)
let xml (text: string) = SecurityElement.Escape text
let hash path = File.ReadAllBytes path |> SHA256.HashData |> Convert.ToHexString
let run label arguments =
    let start = ProcessStartInfo(dotnet, WorkingDirectory = cwd, UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true)
    for argument in arguments do start.ArgumentList.Add argument
    start.Environment["DOTNET_CLI_UI_LANGUAGE"] <- "en-US"
    start.Environment["MSBUILDLOGALLASSEMBLYLOADS"] <- "1"
    // Each comparison selects its own mode, independently of an enclosing CI build.
    start.Environment.Remove("MSBUILDFORCEMULTITHREADED") |> ignore
    use child = Process.Start start
    let pid = child.Id
    let output = child.StandardOutput.ReadToEndAsync()
    let errors = child.StandardError.ReadToEndAsync()
    if not (child.WaitForExit(600_000)) then
        child.Kill(true)
        failwithf "%s exceeded ten minutes" label
    let text = output.Result + errors.Result
    write (Path.Combine(root, label + ".log")) text
    if child.ExitCode <> 0 then printfn "%s: exit %d (see retained log)" label child.ExitCode
    child.ExitCode, pid, text
let succeed label arguments =
    let code, pid, output = run label arguments
    if code <> 0 then failwithf "%s failed. See %s" label root
    pid, output

let _, sdk = succeed "sdk" [ "--version" ]
let _, msbuild = succeed "msbuild" [ "msbuild"; "-version"; "-nologo" ]
let sdkPath = Path.Combine(repo, ".dotnet", "sdk", sdk.Trim())
let product = Path.Combine(repo, "artifacts", "bin", "fsc", configuration, "net11.0")
let taskProduct = Path.Combine(repo, "artifacts", "bin", "FSharp.Build", configuration, "netstandard2.0")
let taskAssembly = Path.Combine(taskProduct, "FSharp.Build.dll")
let compiler = Path.Combine(product, "fsc.dll")
let localFsi = Path.Combine(repo, "artifacts", "bin", "fsi", configuration, "net11.0", "fsi.dll")
for path in [ taskAssembly; compiler; localFsi ] do
    if not (File.Exists path) then failwithf "Build local product bits first: missing %s" path
let taskHash = hash taskAssembly
printfn "SDK %sMSBuild %sLocal tasks: %s\nSHA256 %s\nEvidence: %s" sdk msbuild taskAssembly taskHash root
write (Path.Combine(root, "versions.txt")) $"SDK {sdk}MSBuild {msbuild}Tasks {taskAssembly}\nSHA256 {taskHash}\nCompiler {compiler}\nFsi {localFsi}\n"

// An ordinary SDK helper project uses only assemblies supplied by the running SDK, without NuGet packages.
let helper = Path.Combine(root, "probe")
write (Path.Combine(root, "Directory.Build.props")) "<Project><PropertyGroup><ImportDirectoryPackagesProps>false</ImportDirectoryPackagesProps></PropertyGroup></Project>"
write (Path.Combine(root, "Directory.Build.targets")) "<Project />"
write (Path.Combine(helper, "Probe.cs")) (File.ReadAllText(Path.Combine(__SOURCE_DIRECTORY__, "Probe.cs")))
write (Path.Combine(helper, "Probe.csproj")) $"""<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup><TargetFramework>net11.0</TargetFramework><EnableDefaultCompileItems>false</EnableDefaultCompileItems></PropertyGroup>
  <ItemGroup><Compile Include="Probe.cs" />
    <Reference Include="Microsoft.Build.Framework"><HintPath>{xml sdkPath}/Microsoft.Build.Framework.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="Microsoft.Build.Utilities.Core"><HintPath>{xml sdkPath}/Microsoft.Build.Utilities.Core.dll</HintPath><Private>false</Private></Reference>
  </ItemGroup>
</Project>"""
succeed "probe-build" [ "build"; Path.Combine(helper, "Probe.csproj"); "-nologo"; "-v:minimal"; "-nr:false" ] |> ignore
let probeAssembly = Path.Combine(helper, "bin", "Debug", "net11.0", "Probe.dll")
let graph = Path.Combine(root, "graph")
let projects = [| for index in 1 .. projectCount -> sprintf "P%02d" index |]
write (Path.Combine(graph, "Directory.Build.props")) $"""<Project>
  <PropertyGroup>
    <LoadLocalFSharpBuild>True</LoadLocalFSharpBuild>
    <LocalFSharpCompilerConfiguration>{configuration}</LocalFSharpCompilerConfiguration>
    <LocalFSharpCompilerPath>{xml repo}</LocalFSharpCompilerPath>
    <ProbeAssembly>{xml probeAssembly}</ProbeAssembly><LocalFsi>{xml localFsi}</LocalFsi>
    <NuGetAudit>false</NuGetAudit><ImportDirectoryPackagesProps>false</ImportDirectoryPackagesProps>
  </PropertyGroup>
  <Import Project="{xml repo}/UseLocalCompiler.Directory.Build.props" />
  <PropertyGroup>
    <FSharpBuildAssemblyFile>{xml taskAssembly}</FSharpBuildAssemblyFile>
    <FSharpTargetsPath>{xml taskProduct}/Microsoft.FSharp.Targets</FSharpTargetsPath>
    <FSharpPropsShim>{xml taskProduct}/Microsoft.FSharp.NetSdk.props</FSharpPropsShim>
    <FSharpTargetsShim>{xml taskProduct}/Microsoft.FSharp.NetSdk.targets</FSharpTargetsShim>
    <FSharpOverridesTargetsShim>{xml taskProduct}/Microsoft.FSharp.Overrides.NetSdk.targets</FSharpOverridesTargetsShim>
  </PropertyGroup>
</Project>"""
write (Path.Combine(graph, "Directory.Build.targets")) "<Project />"
for name in projects do
    let directory = Path.Combine(graph, name)
    for source, target in [ "Fixture.fsproj", name + ".fsproj"; "Program.fs", "Program.fs"; "check.fsx", "check.fsx" ] do
        write (Path.Combine(directory, target)) (File.ReadAllText(Path.Combine(__SOURCE_DIRECTORY__, source)))
    write (Path.Combine(directory, "sentinel.txt")) name
    write (Path.Combine(directory, "Messages.txt")) $"marker,\"TEXT_{name}\"\n"
    write (Path.Combine(directory, "Strings.resx")) $"""<root>
  <resheader name="resmimetype"><value>text/microsoft-resx</value></resheader>
  <resheader name="version"><value>2.0</value></resheader>
  <data name="Marker"><value>RESX_{name}</value></data>
</root>"""
    write (Path.Combine(directory, "dependent.txt")) $"DEPENDENT_{name}"
    write (Path.Combine(directory, "template.txt")) "@MARKER@"
let solution = Path.Combine(graph, "Concurrent.slnx")
write solution ("<Solution>\n" + String.concat "\n" [ for name in projects -> $"  <Project Path=\"{name}/{name}.fsproj\" />" ] + "\n</Solution>")
let tasks = set [ "Fsc"; "Fsi"; "WriteCodeFragment"; "FSharpEmbedResourceText"; "FSharpEmbedResXSource"; "CreateFSharpManifestResourceName"; "MapSourceRoots"; "GenerateILLinkSubstitutions"; "SubstituteText" ]
let mutable baseline = Map.empty<string, string>

let clean () =
    for name in projects do
        for child in [ "bin"; "obj" ] do
            let directory = Path.Combine(graph, name, child)
            if Directory.Exists directory then Directory.Delete(directory, true)
    let barrier = Path.Combine(graph, "barrier")
    if Directory.Exists barrier then Directory.Delete(barrier, true)

let validateEvidence label mt incremental pid =
    let rows = File.ReadAllLines(Path.Combine(root, label + ".tsv")) |> Array.map (fun line -> line.Split('\t'))
    let starts = rows |> Array.filter (fun row -> row[0] = "start")
    let fscRuns = starts |> Array.filter (fun row -> row[3] = "Fsc") |> Array.length
    if incremental && fscRuns <> 0 then failwithf "%s recompiled %d unchanged projects" label fscRuns
    let requiredTasks = if incremental then set [ "Fsi"; "SubstituteText" ] else tasks
    let commands = rows |> Array.filter (fun row -> row[0] = "command") |> Array.map (fun row -> row[2], row[5]) |> Map.ofArray
    for task in tasks do
        let matches = starts |> Array.filter (fun row -> row[3] = task)
        let distinct = matches |> Array.map (fun row -> row[4]) |> Array.distinct
        if requiredTasks.Contains task && distinct.Length <> projectCount then
            failwithf "%s: %s ran in %d/%d projects" label task distinct.Length projectCount
        for row in matches do
            if not (String.Equals(Path.GetFullPath(row[5]), taskAssembly, StringComparison.OrdinalIgnoreCase)) then
                failwithf "%s loaded the wrong %s: %s" label task row[5]
            if task = "Fsc" || task = "Fsi" then
                let tool = if task = "Fsc" then compiler else localFsi
                if not (commands[row[2]].Replace('\\', '/').Contains(tool.Replace('\\', '/'), StringComparison.OrdinalIgnoreCase)) then
                    failwithf "%s did not execute local %s" label task
    let routes = rows |> Array.filter (fun row -> row[0] = "route")
    let controls = routes |> Array.filter (fun row -> row[5].Contains("Task \"SerialControl\""))
    if mt then
        if controls.Length <> projectCount then failwithf "%s lacks TaskHost routing control evidence" label
        for row in routes do
            for task in tasks do
                if row[5].Contains($"Task \"{task}\"") then failwithf "%s routed %s out of process: %s" label task row[5]
    let pids =
        projects |> Array.map (fun name ->
            let proof = File.ReadAllText(Path.Combine(graph, name, "environment.txt")).Split('\t')
            if Path.GetFullPath(proof[1]) <> Path.Combine(graph, name) || proof[2] <> name || proof[3] <> taskAssembly then
                failwith "Project environment or loaded assembly proof differs"
            int proof[0]) |> Array.distinct
    let callers = controls |> Array.map (fun row -> int (Regex.Match(row[5], @"caller process (\d+)").Groups[1].Value)) |> Array.distinct
    if mt && (pids.Length <> 1 || pids <> callers) then
        failwithf "MT projects must execute in the routing caller: launcher=%d callers=%A projects=%A" pid callers pids
    if not mt && pids.Length < 2 then failwith "MP did not use multiple processes"
    let hosts = projects |> Array.map (fun name -> File.ReadAllText(Path.Combine(graph, name, "host.txt"))) |> Array.distinct
    if hosts.Length <> 1 then failwithf "%s used inconsistent execution hosts" label
    let host = hosts[0].Split('\t')
    for index, assembly in [ 0, "Microsoft.Build.Framework.dll"; 2, "Microsoft.Build.Utilities.Core.dll" ] do
        if Path.GetFullPath(host[index]) <> Path.Combine(sdkPath, assembly) || String.IsNullOrWhiteSpace(host[index + 1]) then
            failwithf "%s did not execute against the SDK's %s: %A" label assembly host
    write (Path.Combine(root, label + "-host.txt")) hosts[0]
    printfn "%s: execution-host API verified, Framework=%s Utilities=%s" label host[1] host[3]
    let finishes = rows |> Array.filter (fun row -> row[0] = "finish") |> Array.map (fun row -> row[2], int64 row[1]) |> Map.ofArray
    let overlap task =
        let points =
            [| for row in starts do
                   if row[3] = task then
                       yield int64 row[1], 1
                       yield finishes[row[2]], -1 |] |> Array.sort
        points |> Array.scan (fun count (_, delta) -> count + delta) 0 |> Array.max
    let fscOverlap, fsiOverlap = overlap "Fsc", overlap "Fsi"
    if (not incremental && fscOverlap < 2) || fsiOverlap < 4 then
        failwithf "Insufficient overlap: Fsc=%d Fsi=%d" fscOverlap fsiOverlap
    let taskKinds = starts |> Array.map (fun row -> row[3]) |> Set.ofArray |> Set.intersect tasks |> Set.count
    let proof = sprintf "%s: %d projects, %d local task kinds, PIDs=%A, Fsc executions=%d, overlap Fsc=%d Fsi=%d, routing controls=%d" label projectCount taskKinds pids fscRuns fscOverlap fsiOverlap controls.Length
    write (Path.Combine(root, label + "-proof.txt")) proof
    printfn "%s" proof

let validateOutputs label =
    let manifest =
        [ for name in projects do
              let directory = Path.Combine(graph, name)
              for file in [ "fsi.txt"; "obj/substituted/template.txt" ] do
                  if File.ReadAllText(Path.Combine(directory, file)) <> name then failwithf "%s: wrong %s" name file
              for file, marker in [ "Strings.fs", "RESX_" + name; "ILLink.Substitutions.xml", $"fullname=\"{name}\"" ] do
                  if not (File.ReadAllText(Path.Combine(directory, "obj/Release/net11.0", file)).Contains marker) then
                      failwithf "%s: generated %s has the wrong marker" name file
              succeed (label + "-" + name) [ Path.Combine(directory, "bin", "Release", "net11.0", name + ".dll") ] |> ignore
              for child in [ "bin/Release/net11.0"; "obj/Release/net11.0" ] do
                  for file in Directory.GetFiles(Path.Combine(directory, child), "*", SearchOption.AllDirectories) do
                      if [ ".dll"; ".pdb"; ".fs"; ".resx"; ".resources"; ".xml" ] |> List.contains (Path.GetExtension file) then
                          yield Path.GetRelativePath(graph, file), hash file ] |> Map.ofList
    write (Path.Combine(root, label + ".sha256")) (manifest |> Map.toSeq |> Seq.map (fun (name, value) -> $"{value}  {name}") |> String.concat "\n")
    if baseline.IsEmpty then baseline <- manifest
    elif manifest <> baseline then
        let differences = manifest |> Map.filter (fun key value -> Map.tryFind key baseline <> Some value) |> Map.toSeq |> Seq.map fst
        failwithf "%s differs from the first MP build: %A" label (Seq.toList differences)
    printfn "%s: %d deterministic artifacts match" label manifest.Count

let modeSwitch mt = if mt then "-mt" else "-mt:false"

let build label mt =
    run label ([ "msbuild"; solution; "-t:Build"; "-p:Configuration=Release"; "-m:4"; "-nr:false"; "-v:diag"; "-clp:ErrorsOnly;Summary";
                 "-bl:" + Path.Combine(root, label + ".binlog"); "-logger:EvidenceLogger," + probeAssembly + ";" + Path.Combine(root, label + ".tsv");
                 modeSwitch mt ])

for iteration in 1 .. repetitions do
    for mt in [ false; true ] do
        let label = sprintf "%02d-%s" iteration (if mt then "mt" else "mp")
        clean ()
        succeed (label + "-restore") [ "restore"; solution; "-p:Configuration=Release"; "-nr:false"; "-v:minimal" ] |> ignore
        let code, pid, _ = build label mt
        if code <> 0 then failwithf "%s failed. Logs: %s" label root
        validateEvidence label mt false pid
        validateOutputs label
        let noOp = label + "-noop"
        Directory.Delete(Path.Combine(graph, "barrier"), true)
        let code, pid, _ = build noOp mt
        if code <> 0 then failwithf "%s failed. Logs: %s" noOp root
        validateEvidence noOp mt true pid
        validateOutputs noOp

// A broken project must not contaminate its concurrently built siblings.
for mt in [ false; true ] do
    for failure in [ "resource"; "type" ] do
        let label = (if mt then "mt-" else "mp-") + failure
        clean ()
        let file = Path.Combine(graph, projects[0], if failure = "resource" then "Messages.txt" else "Program.fs")
        let original = File.ReadAllText file
        try
            File.WriteAllText(file, if failure = "resource" then "malformed resource\n" else "module Program\nlet value: int = \"wrong\"\n")
            succeed (label + "-restore") [ "restore"; solution; "-p:Configuration=Release"; "-nr:false"; "-v:minimal" ] |> ignore
            let code, _, output = build label mt
            let expected = if failure = "type" then "FS0001" else "After the identifier 'malformed' there should be a comma"
            if code = 0 || not (output.Contains expected) then failwithf "%s did not produce the expected failure" label
            let errors = File.ReadAllLines(Path.Combine(root, label + ".tsv")) |> Array.filter (fun row -> row.StartsWith("error\t"))
            if errors.Length = 0 || errors |> Array.exists (fun row -> row.Split('\t')[4] <> Path.Combine(graph, projects[0], projects[0] + ".fsproj")) then
                failwithf "%s reported errors outside the broken project" label
            if failure = "resource" && errors.Length <> 1 then
                failwithf "%s reported %d errors for one malformed resource" label errors.Length
            for name in projects |> Array.skip 1 do
                let dll = Path.Combine(graph, name, "bin/Release/net11.0", name + ".dll")
                succeed (label + "-" + name) [ dll ] |> ignore
                if hash dll <> baseline[Path.GetRelativePath(graph, dll)] then failwithf "%s contaminated %s" label name
            printfn "%s: expected failure; %d unaffected executables match baseline" label (projectCount - 1)
        finally
            File.WriteAllText(file, original)

// Compare published behavior without changing the existing substitutions target scheduling.
let resourceNames file =
    use stream = File.OpenRead file
    use pe = new PEReader(stream)
    let metadata = pe.GetMetadataReader()
    [ for handle in metadata.ManifestResources -> metadata.GetString(metadata.GetManifestResource(handle).Name) ]
let trimProject = Path.Combine(graph, projects[1])
let metadata names = names |> List.filter (fun (name: string) -> name.StartsWith("FSharpSignature") || name.StartsWith("FSharpOptimization")) |> Set.ofList
let untrimmed = Path.Combine(trimProject, "bin/Release/net11.0", projects[1] + ".dll")
let originalMetadata = metadata (resourceNames untrimmed)
if originalMetadata.IsEmpty then failwith "Trimming control has no F# metadata"
let trimFailures = ResizeArray<string>()
for disabled in [ true; false ] do
    let mutable trimmedHash = ""
    for mt in [ false; true ] do
        let label = (if mt then "publish-mt" else "publish-mp") + (if disabled then "-control" else "")
        let publish = Path.Combine(trimProject, "published")
        for child in [ "published"; "bin"; "obj" ] do
            let directory = Path.Combine(trimProject, child)
            if Directory.Exists directory then Directory.Delete(directory, true)
        succeed label ([ "publish"; Path.Combine(trimProject, projects[1] + ".fsproj"); "-c"; "Release";
                         "-r"; RuntimeInformation.RuntimeIdentifier; "--self-contained"; "true"; "-p:PublishTrimmed=true"; "-p:UseAppHost=true";
                         "-p:DisableILLinkSubstitutions=" + string disabled;
                         "-p:PublishDir=published/"; "-nr:false"; "-v:minimal"; "-bl:" + Path.Combine(root, label + ".binlog");
                         "-m:4"; modeSwitch mt ]) |> ignore
        let dll = Path.Combine(publish, projects[1] + ".dll")
        succeed (label + "-run") [ dll ] |> ignore
        let names = resourceNames dll
        write (Path.Combine(root, label + "-resources.txt")) (String.concat "\n" names)
        let expected = originalMetadata
        let actual = metadata names
        if actual <> expected then trimFailures.Add(sprintf "%s: expected F# metadata %A, found %A" label expected actual)
        if mt && hash dll <> trimmedHash then failwith "MP/MT trimmed assemblies differ"
        trimmedHash <- hash dll
        printfn "%s: executable passed; F# metadata resources=%d; SHA256 %s" label actual.Count trimmedHash
if hash taskAssembly <> taskHash then failwith "Local FSharp.Build changed during the run. Rerun against stable product bits."
let summary = sprintf "%d clean and no-op MP/MT pairs passed, %d projects, %d artifacts per build, four failure/isolation builds, four trimmed publishes (%d substitution failures). Logs: %s" repetitions projectCount baseline.Count trimFailures.Count root
write (Path.Combine(root, "summary.txt")) summary
printfn "%s" summary
if trimFailures.Count <> 0 then failwith (String.concat "\n" trimFailures)
printfn "PASS"
