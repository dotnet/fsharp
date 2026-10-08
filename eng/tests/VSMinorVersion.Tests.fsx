open System
open System.Diagnostics
open System.Globalization
open System.IO
open System.Text.Json

let root = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "../.."))

let run minor arguments =
    let start = ProcessStartInfo(Environment.ProcessPath, RedirectStandardOutput = true, RedirectStandardError = true)
    start.Environment["VSMinorVersion"] <- minor
    for argument in arguments do
        start.ArgumentList.Add argument
    use child = Process.Start start
    let output, errors = child.StandardOutput.ReadToEndAsync(), child.StandardError.ReadToEndAsync()
    child.WaitForExit()
    child.ExitCode, output.Result.Trim(), errors.Result

for minor, text in
    [ 0, "2025-08-29"; 1, "2025-10-03"; 2, "2025-10-31"; 3, "2025-11-28"
      4, "2026-01-02"; 5, "2026-01-30"; 6, "2026-02-27"; 7, "2026-04-03"
      8, "2026-05-01"; 9, "2026-05-29"; 10, "2026-07-03"; 11, "2026-07-31"
      12, "2026-08-28"; 13, "2026-10-02"; 14, "2026-10-30"; 15, "2026-11-27" ] do
    let friday = DateOnly.Parse(text, CultureInfo.InvariantCulture)
    for days in (if minor = 0 then [ 0; 4 ] else [ -1; 0; 4 ]) do
        let date = friday.AddDays(days).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
        let expected = minor - (if days < 0 then 1 else 0)
        let code, output, errors = run "" [ "fsi"; Path.Combine(root, "eng/scripts/GetVSMinorVersion.fsx"); date ]
        if code <> 0 || output <> string expected then
            failwith $"{date}: expected {expected}, got {output}. {errors}"

let fixture = Path.Combine(Path.GetTempPath(), $"fsharp-vs-version-{Guid.NewGuid():N}")
Directory.CreateDirectory fixture |> ignore
try
    let project = Path.Combine(fixture, "Version.fsproj")
    let source = Path.Combine(fixture, "Assembly.fsx")
    File.WriteAllText(project, $"""
<Project>
  <PropertyGroup><RepoRoot>{root}/</RepoRoot><DISABLE_ARCADE>true</DISABLE_ARCADE><Language>F#</Language></PropertyGroup>
  <Import Project="{root}/eng/Versions.props" />
  <Import Project="{root}/FSharpBuild.Directory.Build.props" />
</Project>
    """)
    for usesVS, minor, shouldFail in [ false, "", false; true, "", true; true, "UNSET", true; true, "13", false ] do
        let code, output, errors =
            run minor [ "msbuild"; project; "-nologo"; $"-p:UseVsMicroBuildAssemblyVersion={usesVS}"
                        "-getProperty:AssemblyVersion,NoWarn,WarningsAsErrors" ]
        if code <> 0 then failwith errors
        use data = JsonDocument.Parse output
        let property (name: string) = data.RootElement.GetProperty("Properties").GetProperty(name).GetString()
        let assemblyVersion = property "AssemblyVersion"
        let noWarn = (property "NoWarn").Replace(';', ',')
        let warningsAsErrors = (property "WarningsAsErrors").Replace(';', ',')
        File.WriteAllText(source, $"open System.Reflection\n[<assembly: AssemblyVersion(\"{assemblyVersion}\")>]\ndo ()")
        let code, _, errors =
            run "" [ "fsi"
                     if noWarn <> "" then $"--nowarn:{noWarn}"
                     $"--warnaserror:{warningsAsErrors}"; source ]
        if (code <> 0) <> shouldFail || (shouldFail && not (errors.Contains("FS2003", StringComparison.Ordinal))) then
            failwith $"VS consumer={usesVS}, minor='{minor}': unexpected exit {code}. {errors}"
finally
    Directory.Delete(fixture, true)

printfn "VS minor version tests passed."
