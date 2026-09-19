const fs = require("node:fs/promises");
const path = require("node:path");
const { createFixture } = require("./fixtures/smoke.cjs");
const { fingerprintInput } = require("./model.cjs");
const { requireValue } = require("./toolchain-io.cjs");
const { testSourceRevision } = require("./workspace.cjs");

async function main() {
    const [optIn, sha, mirror, storage, gitExecutable] = process.argv.slice(2);
    requireValue(optIn === "--run" && /^[a-f0-9]{40}$/.test(sha) && mirror && storage && gitExecutable && process.argv.length === 7,
        "usage: node workspace.smoke.cjs --run <full-source-sha> <trusted-mirror> <existing-storage> <absolute-git-executable>");
    const root = await fs.mkdtemp(path.join(await fs.realpath(storage), "fsharp-workspace-smoke-"));
    const os = { win32: "windows", linux: "linux", darwin: "macos" }[process.platform];
    const platform = { os, architecture: process.arch, rid: `${{ windows: "win", linux: "linux", macos: "osx" }[os]}-${process.arch}` };
    const sourceDirectory = path.join(root, "fixture");
    const manifest = await createFixture(sourceDirectory, { kind: "fsi", tfm: "net8.0" });
    const scratchDirectory = path.join(root, "scratch"), evidenceDirectory = path.join(root, "evidence");
    await fs.mkdir(scratchDirectory);
    await fs.mkdir(evidenceDirectory);
    console.log(`Pinned clean-build smoke: ${sha}\nEvidence: ${evidenceDirectory}`);
    const result = await testSourceRevision({
        request: {
            schemaVersion: 1, repository: "dotnet/fsharp", sourceSha: sha, phase: "confirmation",
            manifestDigest: fingerprintInput(manifest), approvalDigest: manifest.approvedInputDigest,
            platform, allocation: { buildTimeoutMs: 90 * 60000, workerTimeoutMs: 110 * 60000 },
        },
        manifest, sourceDirectory, mirrorDirectory: mirror, scratchDirectory, evidenceDirectory, gitExecutable,
    });
    await fs.writeFile(path.join(root, "result.json"), JSON.stringify(result, null, 2), { flag: "wx" });
    console.log(JSON.stringify({ outcome: result.outcome, buildInstanceId: result.buildInstanceId,
        actualSourceSha: result.actualSourceSha, archiveProvenance: result.toolchain?.archive?.provenance,
        error: result.error, result: path.join(root, "result.json") }, null, 2));
    process.exitCode = result.outcome.outcome === "pass" ? 0 : 2;
}

if (require.main === module) main().catch(error => { console.error(error.message); process.exitCode = 2; });
