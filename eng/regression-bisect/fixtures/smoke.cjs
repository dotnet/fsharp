const fs = require("node:fs/promises");
const path = require("node:path");
const { hash } = require("../toolchain-io.cjs");
const { fingerprintInput } = require("../model.cjs");

async function createFixture(directory, { kind, tfm, configuration = "Release", scenario = "pass", producer, coreVersion }) {
    await fs.mkdir(directory, { recursive: true });
    const contents = new Map();
    const failing = scenario === "runtime-failure";
    const referenced = scenario.startsWith("reference-");
    const nullable = {
        "nullable-enabled": ["enable"],
        "nullable-disabled": ["disable"],
        "late-nullable-enable": ["disable", "enable"],
        "late-nullable-disable": ["enable", "disable"],
        "late-nullable-explicit": ["enable", "disable"],
    }[scenario];
    const projectTfm = scenario === "reference-tfm" ? `${tfm}-windows7.0` : tfm;
    const sdkFSharp = "$(MSBuildSDKsPath)/../FSharp";
    const producerSource = [
        "#if OLD_PRODUCER",
        'let inline value () = "OLD_METADATA"',
        "#else",
        'let inline value () = "NEW_METADATA"',
        "#endif",
    ].join("\n");
    const body = producer
        ? 'if FrozenProducer.value () = "OLD_METADATA" then\n    eprintfn "FROZEN_RUNTIME_FAILURE"\n    1\nelse\n    printfn "PASS"\n    0'
        : failing ? 'eprintfn "FROZEN_RUNTIME_FAILURE"\n1'
        : referenced ? 'printfn "%s" Library.message\n0' : 'printfn "PASS"\n0';
    if (kind === "fsi") contents.set("repro.fsx", scenario === "producer-control"
        ? `${producerSource}\nprintfn "%s" (value ())\n`
        : `let result =\n${body.split("\n").map(l => `    ${l}`).join("\n")}\nexit result\n`);
    else {
        contents.set("Program.fs", `module ${scenario === "producer" ? "FrozenProducer" : "Repro"}
${scenario === "producer" ? `${producerSource}\n` : ""}[<EntryPoint>]
let main _ =
${body.split("\n").map(l => `    ${l}`).join("\n")}
`);
        if (scenario === "diagnostic") contents.set("Program.fs", [
            "module Repro",
            '[<System.Obsolete("FROZEN_DIAGNOSTIC", true)>]',
            "let value = 1",
            "[<EntryPoint>]",
            "let main _ =",
            "    value",
            "",
        ].join("\n"));
        if (scenario === "collectible-core") contents.set("Program.fs", `module Repro
open System
open System.IO
open System.Runtime.CompilerServices
open System.Runtime.Loader

[<MethodImpl(MethodImplOptions.NoInlining)>]
let loadCore () =
    let context = new AssemblyLoadContext("collectible-core", true)
    context.LoadFromAssemblyPath(Path.Combine(AppContext.BaseDirectory, "FSharp.Core.dll")) |> ignore
    let weak = WeakReference(context)
    context.Unload()
    weak

[<EntryPoint>]
let main _ =
    let weak = loadCore ()
    for _ in 1 .. 20 do
        GC.Collect()
        GC.WaitForPendingFinalizers()
        GC.Collect()
    if weak.IsAlive then
        Console.Error.WriteLine("CONTEXT_RETAINED")
        1
    else
        Console.WriteLine("PASS")
        0
`);
        contents.set("Repro.fsproj", `<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    ${scenario === "multi-tfm" ? `<TargetFrameworks>${tfm};${tfm}-windows7.0</TargetFrameworks>` : `<TargetFramework>${projectTfm}</TargetFramework>`}
    <NuGetAudit>false</NuGetAudit>
    ${nullable ? `<Nullable>${nullable[0]}</Nullable>` : ""}
    ${scenario === "late-nullable-explicit" ? "<DefineConstants>$(DefineConstants);NULLABLE</DefineConstants>" : ""}
    ${scenario.startsWith("core-") ? "<DisableImplicitFSharpCoreReference>true</DisableImplicitFSharpCoreReference>" : ""}
    ${scenario === "fallback-compiler" ? `<DotnetFscCompilerPath>${sdkFSharp}/fsc.dll</DotnetFscCompilerPath><Fsc_DotNET_DotnetFscCompilerPath>${sdkFSharp}/fsc.dll</Fsc_DotNET_DotnetFscCompilerPath>` : ""}
    ${scenario === "fallback-tasks" ? `<FSharpBuildAssemblyFile>${sdkFSharp}/FSharp.Build.dll</FSharpBuildAssemblyFile>` : ""}
    ${scenario === "fallback-targets" ? `<FSharpTargetsPath>${sdkFSharp}/Microsoft.FSharp.Targets</FSharpTargetsPath><FSharpTargetsShim>${sdkFSharp}/Microsoft.FSharp.NetSdk.targets</FSharpTargetsShim><FSharpOverridesTargetsShim>${sdkFSharp}/Microsoft.FSharp.Overrides.NetSdk.targets</FSharpOverridesTargetsShim>` : ""}
    ${scenario === "fallback-core" ? "<DisableImplicitFSharpCoreReference>true</DisableImplicitFSharpCoreReference>" : ""}
    ${scenario === "producer" ? "<AssemblyName>FrozenProducer</AssemblyName><PackageId>FrozenProducer</PackageId><Version>1.0.0</Version><DefineConstants>$(DefineConstants);OLD_PRODUCER</DefineConstants><GeneratePackageOnBuild>true</GeneratePackageOnBuild>" : ""}
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="Program.fs" />
    ${scenario === "core-explicit" ? `<PackageReference Include="FSharp.Core" Version="${coreVersion}" />` : ""}
    ${scenario === "core-central" ? '<PackageReference Include="FSharp.Core" />' : ""}
    ${scenario === "fallback-core" ? `<Reference Include="FSharp.Core"><HintPath>${sdkFSharp}/FSharp.Core.dll</HintPath></Reference>` : ""}
    ${producer ? '<Reference Include="FrozenProducer"><HintPath>producer/FrozenProducer.dll</HintPath></Reference>' : ""}
    ${referenced ? '<ProjectReference Include="Library/Library.fsproj" />' : ""}
  </ItemGroup>
  ${scenario === "late-flags" ? '<Target Name="ChangeFlagsAfterEvidence" AfterTargets="_ReproEvidenceSnapshot"><PropertyGroup><OtherFlags>$(OtherFlags) --optimize-</OtherFlags></PropertyGroup></Target>' : ""}
  ${nullable?.[1] ? `<Target Name="ChangeNullableAfterEvidence" AfterTargets="_ReproEvidenceSnapshot"><PropertyGroup><Nullable>${nullable[1]}</Nullable></PropertyGroup></Target>` : ""}
</Project>
`);
        if (referenced) {
            contents.set("Library/Library.fs", `module Library
let message = "PASS"
[<EntryPoint>]
let main _ =
    printfn "%s" message
    0
`);
            contents.set("Library/Library.fsproj", `<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup><OutputType>Exe</OutputType><TargetFramework>${tfm}</TargetFramework><NuGetAudit>false</NuGetAudit></PropertyGroup>
  <ItemGroup><Compile Include="Library.fs" /></ItemGroup>
  ${scenario === "reference-late" ? '<Target Name="ChangeLibraryFlags" AfterTargets="_ReproEvidenceSnapshot"><PropertyGroup><OtherFlags>$(OtherFlags) --optimize-</OtherFlags></PropertyGroup></Target>' : ""}
</Project>
`);
        }
        if (scenario === "core-central") contents.set("Directory.Packages.props", `<Project>
  <PropertyGroup><ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally></PropertyGroup>
  <ItemGroup><PackageVersion Include="FSharp.Core" Version="${coreVersion}" /></ItemGroup>
</Project>
`);
    }
    const sources = [];
    for (const [name, text] of contents) {
        await fs.mkdir(path.dirname(path.join(directory, name)), { recursive: true });
        await fs.writeFile(path.join(directory, name), text);
        sources.push({ path: name, sha256: hash(text) });
    }
    if (producer) {
        await fs.mkdir(path.join(directory, "producer"));
        for (const record of [producer.dll, producer.package]) {
            await fs.copyFile(record.originalPath, path.join(directory, record.path));
            sources.push({ path: record.path, sha256: record.sha256 });
        }
        const descriptor = { ...producer };
        delete descriptor.identity;
        descriptor.dll = { ...producer.dll }; delete descriptor.dll.originalPath;
        descriptor.package = { ...producer.package }; delete descriptor.package.originalPath;
        const bytes = JSON.stringify(descriptor);
        await fs.writeFile(path.join(directory, "frozen.producer.json"), bytes);
        sources.push({ path: "frozen.producer.json", sha256: hash(bytes) });
    }
    const project = kind === "project" ? "Repro.fsproj" : null;
    const manifest = {
        schemaVersion: 1, repository: "dotnet/fsharp", issueNumber: 1,
        discussionFingerprint: hash("synthetic adapter smoke; not an upstream issue"),
        kind, sources,
        assertions: {
            pass: { exitCode: 0, stdout: scenario === "producer-control" ? "NEW_METADATA\n" : "PASS\n", stderr: "" },
            failure: { exitCode: 1, stdout: "", stderr: "FROZEN_RUNTIME_FAILURE\n" },
        },
        matrix: [{
            id: `${kind}-${tfm}-${configuration}`, os: { win32: "windows", linux: "linux", darwin: "macos" }[process.platform],
            architecture: process.arch, project, tfm: projectTfm, configuration, languageVersion: nullable ? "9.0" : "8.0",
            command: [kind === "fsi" ? "fsi" : "run-project", project ?? "repro.fsx"],
        }],
        timeoutSeconds: 120,
        producer: producer?.identity ?? { name: "no-producer", version: "0.0.0", sha256: hash("") },
        packages: producer ? [{ id: producer.package.id, version: producer.package.version, sha256: producer.package.sha256 }] : [],
        searchHints: { sdkVersions: [], sourceShas: [], urls: [] },
    };
    if (scenario === "diagnostic") manifest.assertions = {
        pass: { exitCode: 0, stdout: "", stderr: "" },
        failure: { exitCode: 1, stdout: "", stderr: "Program.fs(6,5): error FS0101: This construct is deprecated. FROZEN_DIAGNOSTIC\n" },
    };
    if (scenario === "collectible-core") manifest.assertions.failure.stderr = "CONTEXT_RETAINED\n";
    if (scenario === "reference-tfm") manifest.matrix.push({
        ...manifest.matrix[0], id: `library-${tfm}-${configuration}`, tfm,
        project: "Library/Library.fsproj", command: ["run-project", "Library/Library.fsproj"],
    });
    if (scenario === "multi-tfm") manifest.matrix.push({
        ...manifest.matrix[0], id: `${kind}-${tfm}-windows7.0-${configuration}`, tfm: `${tfm}-windows7.0`,
    });
    manifest.approvedInputDigest = fingerprintInput(manifest);
    await fs.writeFile(path.join(directory, "manifest.json"), JSON.stringify(manifest, null, 2));
    return manifest;
}

module.exports = { createFixture };
