const { createHash } = require("node:crypto");
const policy = () => structuredClone(require("../../../.github/policies/regression-bisect.json"));
const digest = text => createHash("sha256").update(text).digest("hex");
const sha = text => createHash("sha1").update(text).digest("hex");
const model = () => require("../model.cjs");
const seal = m => {
    const { approvedInputDigest, ...input } = m;
    m.approvedInputDigest = model().fingerprintInput(input);
    return m;
};

function manifest() {
    return seal({
        schemaVersion: 1, repository: "dotnet/fsharp", issueNumber: 123,
        discussionFingerprint: digest("complete issue and edited comments"), kind: "fsi",
        sources: [{ path: "fixtures/repro.fsx", sha256: digest("printfn \"ok\"") }],
        assertions: {
            pass: { exitCode: 0, stdout: "ok\n", stderr: "" },
            failure: { exitCode: 1, stdout: "", stderr: "expected regression\n" },
        },
        matrix: [{
            id: "linux-x64", os: "linux", architecture: "x64", project: null,
            tfm: "net10.0", configuration: "Release", languageVersion: "10.0",
            command: ["fsi", "fixtures/repro.fsx"],
        }],
        timeoutSeconds: 300, producer: { name: "repro", version: "1.0.0", sha256: digest("producer") },
        packages: [{ id: "Example", version: "1.0.0", sha256: digest("package") }],
        searchHints: { sdkVersions: ["9.0.100"], sourceShas: [sha("good")], urls: ["https://github.com/dotnet/fsharp"] },
    });
}

function attempt(outcome) {
    return {
        schemaVersion: 1,
        checks: { restore: true, build: true, setup: true, identity: true, sdk: true, runtime: true },
        execution: {
            status: "completed", complete: true,
            ...structuredClone(manifest().assertions[outcome === "pass" ? "pass" : "failure"]),
            ...(outcome === "untestable" ? { stderr: "unexpected exception" } : {}),
        },
    };
}

const stableSdk = (matrix = [{ os: "linux", architecture: "x64" }]) => ({
    version: "10.0.100", origin: "official-release-index", latest: true, verified: true,
    releaseIndexUrl: "https://builds.dotnet.microsoft.com/dotnet/release-metadata/releases-index.json",
    releaseIndexDigest: digest("release index"),
    archives: [...new Map(matrix.map(c => [`${c.os}-${c.architecture}`, {
        os: c.os, architecture: c.architecture, sha256: digest(`SDK archive ${c.os}-${c.architecture}`),
    }])).values()],
});

function observation(kind, outcome, m = manifest(), sourceSha = sha("main")) {
    const fingerprint = model().fingerprintInput;
    const cells = m.matrix.map(cell => ({
        id: cell.id, command: cell.command,
        environment: { os: cell.os, architecture: cell.architecture, imageDigest: digest("runner image") },
        sdk: { version: "10.0.100", sha256: digest(`SDK archive ${cell.os}-${cell.architecture}`), verified: true },
        runtime: { version: "10.0.0", sha256: digest("runtime"), verified: true },
        artifacts: Object.fromEntries(["compiler", "tasks", "targets", "core"].map(name => [name, {
            sha256: digest(`${kind}-${sourceSha}-${name}`), origin: kind === "source" ? "local-build" : "released-sdk",
            sourceSha: kind === "source" ? sourceSha : null, buildInstanceId: `build-${sourceSha}`,
        }])),
        producer: structuredClone(m.producer),
        attempts: [attempt(outcome), attempt(outcome)],
        outcome: model().classifyAttempts([attempt(outcome), attempt(outcome)], m),
    }));
    return {
        schemaVersion: 1, kind, target: kind === "source" ? { sourceSha } : { sdk: stableSdk(m.matrix) },
        manifestDigest: fingerprint(m), inputDigest: m.approvedInputDigest, discussionFingerprint: m.discussionFingerprint,
        matrixFingerprint: fingerprint(m.matrix), producerFingerprint: fingerprint(m.producer), packageFingerprint: fingerprint(m.packages),
        buildInstanceId: `build-${sourceSha}`, cached: false, cells,
        outcome: cells[0].outcome,
        provenance: { workerId: "worker-1", runId: "12345", workflowRevision: sha("workflow"), policyRevision: sha("policy") },
    };
}

function admission(stable = "product-failure", main = "product-failure", good = "pass") {
    const m = manifest();
    const result = {
        manifest: m, stable: observation("released", stable, m), main: observation("source", main, m),
        good: observation("source", good, m, sha("good")),
        ancestry: { goodSha: sha("good"), badSha: sha("main"), pinnedMainSha: sha("main"), isAncestor: true },
        budget: {
            schemaVersion: 1, policy: policy(), policyRevision: sha("policy"), workflowRevision: sha("workflow"),
            allowanceId: "maintainer-approved-investigation-1",
            authorization: {
                repository: m.repository, issueNumber: m.issueNumber,
                approvalPath: ".github/regression-bisect/approved/123.json", approvalRevision: sha("policy"),
                manifestDigest: model().fingerprintInput(m), inputDigest: m.approvedInputDigest,
            },
            stableSdk: stableSdk(), authenticatedObservationDigests: [],
            consumed: { sourceBuilds: 2, workerUnits: 3, reservedMinutes: 330 },
            attempts: [], nextWorkItemId: "first-bisection-build", pendingWork: null,
        },
    };
    result.budget.authenticatedObservationDigests = [result.stable, result.main, result.good].map(model().fingerprintInput);
    return result;
}

function state() {
    const a = admission();
    return {
        schemaVersion: 1, repository: a.manifest.repository, issueNumber: a.manifest.issueNumber,
        issueFingerprint: a.manifest.discussionFingerprint, approvalFingerprint: model().fingerprintInput(a.manifest),
        manifest: a.manifest, budget: a.budget, pinnedMainSha: a.ancestry.pinnedMainSha,
        epoch: model().fingerprintEpoch(a.manifest, a.budget, a.ancestry.pinnedMainSha), phase: "admission",
        endpointEvidence: { stable: model().fingerprintInput(a.stable), main: model().fingerprintInput(a.main), good: model().fingerprintInput(a.good) },
        testedRevisions: [{ sourceSha: sha("good"), observationDigest: model().fingerprintInput(a.good) }],
        skips: [], interval: { goodSha: sha("good"), badSha: sha("main"), gitLog: [] },
        publication: { digest: null, commentId: null },
    };
}

module.exports = { policy, manifest, attempt, observation, admission, state, seal, sha, digest };
