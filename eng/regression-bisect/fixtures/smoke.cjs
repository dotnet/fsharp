const fs = require("node:fs/promises");
const path = require("node:path");
const { hash } = require("../toolchain-io.cjs");
const { fingerprintInput } = require("../model.cjs");

async function createFixture(directory, { kind, tfm, configuration = "Release", scenario = "pass", producer, coreVersion }) {
    await fs.mkdir(directory, { recursive: true });
    const contents = new Map();
    const failing = scenario === "runtime-failure";
    const producerSource = [
        "#if OLD_PRODUCER",
        'let inline value () = "OLD_METADATA"',
        "#else",
        'let inline value () = "NEW_METADATA"',
        "#endif",
    ].join("\n");
    const body = producer
        ? 'if FrozenProducer.value () = "OLD_METADATA" then\n    eprintfn "FROZEN_RUNTIME_FAILURE"\n    1\nelse\n    printfn "PASS"\n    0'
        : failing ? 'eprintfn "FROZEN_RUNTIME_FAILURE"\n1' : 'printfn "PASS"\n0';
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
        contents.set("Repro.fsproj", `<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    ${scenario === "multi-tfm" ? `<TargetFrameworks>${tfm};${tfm}-windows7.0</TargetFrameworks>` : `<TargetFramework>${tfm}</TargetFramework>`}
    <NuGetAudit>false</NuGetAudit>
    ${scenario.startsWith("core-") ? "<DisableImplicitFSharpCoreReference>true</DisableImplicitFSharpCoreReference>" : ""}
    ${scenario === "producer" ? "<AssemblyName>FrozenProducer</AssemblyName><PackageId>FrozenProducer</PackageId><Version>1.0.0</Version><DefineConstants>$(DefineConstants);OLD_PRODUCER</DefineConstants><GeneratePackageOnBuild>true</GeneratePackageOnBuild>" : ""}
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="Program.fs" />
    ${scenario === "core-explicit" ? `<PackageReference Include="FSharp.Core" Version="${coreVersion}" />` : ""}
    ${scenario === "core-central" ? '<PackageReference Include="FSharp.Core" />' : ""}
    ${producer ? '<Reference Include="FrozenProducer"><HintPath>producer/FrozenProducer.dll</HintPath></Reference>' : ""}
  </ItemGroup>
  ${scenario === "late-flags" ? '<Target Name="ChangeFlagsAfterEvidence" AfterTargets="_ReproEvidenceSnapshot"><PropertyGroup><OtherFlags>$(OtherFlags) --optimize-</OtherFlags></PropertyGroup></Target>' : ""}
</Project>
`);
        if (scenario === "core-central") contents.set("Directory.Packages.props", `<Project>
  <PropertyGroup><ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally></PropertyGroup>
  <ItemGroup><PackageVersion Include="FSharp.Core" Version="${coreVersion}" /></ItemGroup>
</Project>
`);
    }
    const sources = [];
    for (const [name, text] of contents) {
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
            architecture: process.arch, project, tfm, configuration, languageVersion: "8.0",
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
    if (scenario === "multi-tfm") manifest.matrix.push({
        ...manifest.matrix[0], id: `${kind}-${tfm}-windows7.0-${configuration}`, tfm: `${tfm}-windows7.0`,
    });
    manifest.approvedInputDigest = fingerprintInput(manifest);
    await fs.writeFile(path.join(directory, "manifest.json"), JSON.stringify(manifest, null, 2));
    return manifest;
}

module.exports = { createFixture };
