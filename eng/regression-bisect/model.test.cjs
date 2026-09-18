const test = require("node:test");
const assert = require("node:assert/strict");
const {
    isEligibleIssue, validatePolicy, validateManifest, fingerprintInput, fingerprintEpoch,
    classifyAttempts, validateOutcome, validateObservation, validateState, admitBisection,
} = require("./model.cjs");
const { policy, manifest, attempt, observation, admission, state, seal, sha, digest } = require("./fixtures/contracts.cjs");

const rejects = (fn, pattern = /./) => assert.throws(fn, pattern);

for (const [name, patch, expected] of [
    ["before cutoff", { created_at: "2026-08-31T23:59:59.999Z" }, false],
    ["at cutoff", {}, true],
    ["after cutoff", { created_at: "2026-09-01T00:00:00.001Z" }, true],
    ["equivalent offset", { created_at: "2026-09-01T02:00:00+02:00" }, true],
    ["offset before cutoff", { created_at: "2026-09-01T00:00:00+01:00" }, false],
    ["invalid date", { created_at: "not a date" }, false],
    ["normalized invalid date", { created_at: "2026-09-31T00:00:00Z" }, false],
    ["no timezone", { created_at: "2026-09-01T00:00:00" }, false],
    ["closed", { state: "closed" }, false],
    ["pull request", { pull_request: {} }, false],
    ["null pull request marker", { pull_request: null }, false],
    ["wrong case", { labels: [{ name: "regression" }] }, false],
    ["missing labels", { labels: undefined }, false],
    ["string label", { labels: ["Regression"] }, true],
    ["updated is not created", { created_at: "2025-09-01T00:00:00Z", updated_at: "2026-09-02T00:00:00Z" }, false],
]) {
    test(`eligibility: ${name}`, () => {
        assert.equal(isEligibleIssue({
            state: "open", created_at: "2026-09-01T00:00:00Z", labels: [{ name: "Regression" }], ...patch,
        }), expected);
    });
}

test("policy has finite operational defaults, separate from issue authorization", () => {
    assert.equal(validatePolicy(policy()), true);
    assert.deepEqual(policy().limits, {
        sourceBuilds: 24, workerUnits: 40, reservedMinutes: 720, attemptsPerItem: 2,
        assertionRepetitions: 2, buildTimeoutMinutes: 90, reproductionTimeoutSeconds: 300,
        workerTimeoutMinutes: 110, discoveryIssues: 3, discoveryPages: 2, discoveryPageSize: 100,
    });
});

for (const key of Object.keys(policy().limits)) {
    for (const value of [-1, 0, 1.5, null, "unlimited", Infinity, policy().limits[key] + 1]) {
        test(`policy rejects ${key}=${value}`, () => {
            const p = policy();
            p.limits[key] = value;
            rejects(() => validatePolicy(p), /policy/);
        });
    }
}

for (const [name, edit] of [
    ["schema", m => m.schemaVersion = 2],
    ["unknown field", m => m.instructions = "run issue commands"],
    ["missing approval", m => delete m.approvedInputDigest],
    ["wrong approval digest", m => m.approvedInputDigest = digest("wrong")],
    ["traversal", m => m.sources[0].path = "../payload.fsx"],
    ["encoded traversal", m => m.sources[0].path = "%2e%2e/payload.fsx"],
    ["absolute path", m => m.sources[0].path = "C:\\payload.fsx"],
    ["UNC path", m => m.sources[0].path = "\\\\host\\payload.fsx"],
    ["URL source", m => m.sources[0].path = "https://example.org/code.fsx"],
    ["alternate stream", m => m.sources[0].path = "fixture.fsx:code"],
    ["shell string", m => m.matrix[0].command = "dotnet fsi repro.fsx"],
    ["interpolated argument", m => m.matrix[0].command.push("$(curl evil)")],
    ["unreviewed command source", m => m.matrix[0].command[1] = "other.fsx"],
    ["unsafe hint scheme", m => m.searchHints.urls = ["file:///tmp/code"]],
    ["malformed hint URL", m => m.searchHints.urls = ["not-a-url"]],
    ["URL credentials", m => m.searchHints.urls = ["https://user:secret@example.org/"]],
    ["unbounded timeout", m => m.timeoutSeconds = 301],
    ["package wildcard", m => m.packages[0].version = "1.*"],
    ["identical assertions", m => m.assertions.failure = structuredClone(m.assertions.pass)],
    ["empty matrix", m => m.matrix = []],
    ["duplicate matrix cell", m => m.matrix.push(structuredClone(m.matrix[0]))],
    ["duplicate source", m => m.sources.push(structuredClone(m.sources[0]))],
]) {
    test(`manifest rejects ${name}`, () => {
        const m = manifest();
        edit(m);
        if (!name.includes("approval")) seal(m);
        rejects(() => validateManifest(m), /manifest/);
    });
}

test("both reviewed fsi and project manifests validate", () => {
    assert.equal(validateManifest(manifest()), true);
    const m = manifest();
    m.kind = "project";
    m.sources.push({ path: "fixtures/repro.fsproj", sha256: digest("project") });
    m.matrix[0].project = "fixtures/repro.fsproj";
    m.matrix[0].command = ["run-project", "fixtures/repro.fsproj"];
    seal(m);
    assert.equal(validateManifest(m), true);
});

for (const [name, results, edit, expected, reason] of [
    ["passes", ["pass", "pass"], () => {}, "pass", "positive-assertion"],
    ["failures", ["product-failure", "product-failure"], () => {}, "product-failure", "failure-assertion"],
    ["flake", ["pass", "product-failure"], () => {}, "untestable", "flaky"],
    ["restore", ["pass", "pass"], a => a.checks.restore = false, "untestable", "restore-failure"],
    ["build", ["pass", "pass"], a => a.checks.build = false, "untestable", "build-failure"],
    ["setup", ["pass", "pass"], a => a.checks.setup = false, "untestable", "setup-failure"],
    ["identity", ["pass", "pass"], a => a.checks.identity = false, "untestable", "wrong-identity"],
    ["SDK", ["pass", "pass"], a => a.checks.sdk = false, "untestable", "unavailable-sdk"],
    ["runtime", ["pass", "pass"], a => a.checks.runtime = false, "untestable", "unavailable-runtime"],
    ["timeout", ["pass", "pass"], a => a.execution.status = "timeout", "untestable", "timeout"],
    ["exception", ["pass", "pass"], a => a.execution.status = "exception", "untestable", "unrelated-exception"],
    ["nonzero is not assertion", ["product-failure", "product-failure"], a => a.execution.stderr = "unrelated", "untestable", "assertion-mismatch"],
    ["incomplete output", ["pass", "pass"], a => a.execution.complete = false, "untestable", "incomplete-evidence"],
    ["missing evidence", ["pass", "pass"], a => delete a.execution, "untestable", "incomplete-evidence"],
    ["one repetition", ["pass"], () => {}, "untestable", "incomplete-evidence"],
]) {
    test(`classification: ${name}`, () => {
        const attempts = results.map(r => attempt(r));
        edit(attempts[0]);
        assert.deepEqual(classifyAttempts(attempts, manifest()), { schemaVersion: 1, outcome: expected, reason });
    });
}

test("outcome versions and reason combinations are validated", () => {
    assert.equal(validateOutcome({ schemaVersion: 1, outcome: "untestable", reason: "flaky" }), true);
    for (const value of [
        { schemaVersion: 2, outcome: "pass", reason: "positive-assertion" },
        { schemaVersion: 1, outcome: "green", reason: "positive-assertion" },
        { schemaVersion: 1, outcome: "pass", reason: "restore-failure" },
        { schemaVersion: 1, outcome: "product-failure", reason: "exit-code" },
    ]) rejects(() => validateOutcome(value), /outcome/);
});

test("neither repeated passes nor expected failures survive failed required checks", () => {
    for (const result of ["pass", "product-failure"]) {
        for (const check of Object.keys(attempt(result).checks)) {
            const attempts = [attempt(result), attempt(result)];
            attempts[1].checks[check] = false;
            assert.equal(classifyAttempts(attempts, manifest()).outcome, "untestable", `${result}: ${check}`);
        }
    }
});

const outcomes = ["pass", "product-failure", "untestable"];
for (const stable of outcomes) {
    for (const main of outcomes) {
        for (const good of outcomes) {
            test(`admission: ${stable}/${main}/${good}`, () => {
                const args = admission(stable, main, good);
                const result = admitBisection(args);
                assert.equal(result.decision === "admit", stable === "product-failure" && main === "product-failure" && good === "pass");
                assert.equal(typeof result.reason, "string");
                if ([stable, main, good].includes("untestable")) assert.equal(result.decision, "blocked");
            });
        }
    }
}

for (const [name, edit] of [
    ["missing manifest", a => delete a.manifest],
    ["missing good", a => delete a.good],
    ["missing approval", a => delete a.budget.authorization],
    ["self approved changed manifest", a => { a.manifest.assertions.pass.stdout = "changed"; seal(a.manifest); }],
    ["released-product-only good", a => a.good = observation("released", "pass", a.manifest)],
    ["nonancestor", a => a.ancestry.isAncestor = false],
    ["unknown ancestry", a => delete a.ancestry.isAncestor],
    ["same source", a => a.good.target.sourceSha = a.main.target.sourceSha],
    ["wrong main SHA", a => a.ancestry.pinnedMainSha = sha("other")],
    ["wrong matrix", a => a.good.matrixFingerprint = digest("other")],
    ["wrong producer", a => a.good.producerFingerprint = digest("other")],
    ["wrong digest", a => a.main.manifestDigest = digest("other")],
    ["wrong discussion", a => a.good.discussionFingerprint = digest("other")],
    ["wrong environment", a => a.good.cells[0].environment.imageDigest = digest("other")],
    ["wrong source SDK", a => a.good.cells[0].sdk.version = "9.0.100"],
    ["missing local Core", a => delete a.main.cells[0].artifacts.core],
    ["released compiler in source test", a => a.main.cells[0].artifacts.compiler.origin = "released-sdk"],
    ["wrong build instance", a => a.main.cells[0].artifacts.core.buildInstanceId = "other"],
    ["wrong artifact source", a => a.good.cells[0].artifacts.tasks.sourceSha = sha("other")],
    ["missing cell", a => a.main.cells = []],
    ["unverified evidence", a => a.budget.authenticatedObservationDigests = []],
    ["unverified SDK", a => a.stable.target.sdk.verified = false],
    ["installed SDK", a => a.stable.target.sdk.origin = "installed"],
    ["preview SDK", a => a.stable.target.sdk.version = "11.0.100-preview.1"],
    ["not latest stable", a => a.stable.target.sdk.latest = false],
    ["wrong resolved stable", a => a.budget.stableSdk.version = "9.0.100"],
    ["unofficial release index", a => a.stable.target.sdk.releaseIndexUrl = "https://example.org/releases-index.json"],
    ["wrong workflow", a => a.good.provenance.workflowRevision = sha("other")],
    ["forged outcome", a => a.good.outcome.outcome = "pass"],
    ["source budget", a => a.budget.consumed.sourceBuilds = policy().limits.sourceBuilds],
    ["worker budget", a => a.budget.consumed.workerUnits = policy().limits.workerUnits],
    ["time budget", a => a.budget.consumed.reservedMinutes = policy().limits.reservedMinutes - 109],
    ["attempts exhausted", a => a.budget.attempts = [{ workItemId: a.budget.nextWorkItemId, count: 2 }]],
    ["expanded budget", a => a.budget.policy.limits.workerUnits++],
    ["unknown admission field", a => a.override = true],
]) {
    test(`admission denies ${name}`, () => {
        const args = admission();
        if (name === "forged outcome") args.good = observation("source", "untestable", args.manifest, sha("good"));
        edit(args);
        if (name !== "unverified evidence") {
            args.budget.authenticatedObservationDigests = [args.stable, args.main, args.good]
                .filter(Boolean).map(fingerprintInput);
        }
        assert.notEqual(admitBisection(args).decision, "admit");
    });
}

test("invalid observation evidence is rejected even if the collector authenticated its digest", () => {
    for (const edit of [
        o => o.cells[0].attempts[0].checks.identity = false,
        o => o.cells[0].command = ["fsi", "unreviewed.fsx"],
        o => o.cells[0].attempts[0].execution.stdout = "x".repeat(16385),
        o => o.cells.push(structuredClone(o.cells[0])),
        o => o.provenance.runId = "",
        o => o.schemaVersion = 2,
    ]) {
        const o = observation("source", "pass", manifest(), sha("good"));
        edit(o);
        rejects(() => validateObservation(o, manifest()), /observation/);
    }
});

test("fresh final confirmations cannot use cached endpoint evidence", () => {
    const o = observation("source", "pass", manifest(), sha("good"));
    const finalConfirmation = { runId: o.provenance.runId, buildInstanceId: o.buildInstanceId };
    assert.equal(validateObservation(o, manifest(), { finalConfirmation }), true);
    rejects(() => validateObservation(o, manifest(), {
        finalConfirmation: { ...finalConfirmation, runId: "new-run" },
    }), /fresh/);
    rejects(() => validateObservation(o, manifest(), {
        finalConfirmation: { ...finalConfirmation, buildInstanceId: "new-build" },
    }), /fresh/);
    rejects(() => validateObservation(o, manifest(), { finalConfirmation: true }), /fresh/);
    o.cached = true;
    assert.equal(validateObservation(o, manifest()), true);
    rejects(() => validateObservation(o, manifest(), { finalConfirmation }), /fresh/);
});

test("multi-platform stable release identities bind each platform's archive", () => {
    const m = manifest();
    m.matrix.push({ ...m.matrix[0], id: "windows-arm64", os: "windows", architecture: "arm64" });
    seal(m);
    const o = observation("released", "product-failure", m);
    assert.equal(validateObservation(o, m), true);
    o.cells[1].sdk.sha256 = o.cells[0].sdk.sha256;
    rejects(() => validateObservation(o, m), /released SDK identity/);
});

test("source builds may use pinned prerelease toolchains without treating them as released stable", () => {
    const a = admission();
    for (const o of [a.main, a.good]) {
        o.cells[0].sdk.version = "11.0.100-rc.1.26420.103";
        o.cells[0].runtime.version = "11.0.0-rc.1.26420.103";
    }
    a.budget.authenticatedObservationDigests = [a.stable, a.main, a.good].map(fingerprintInput);
    assert.equal(admitBisection(a).decision, "admit");
});

test("assertions preserve signed process exit codes", () => {
    const m = manifest();
    m.assertions.failure.exitCode = -1;
    seal(m);
    const attempts = [attempt("product-failure"), attempt("product-failure")];
    for (const a of attempts) a.execution.exitCode = -1;
    assert.equal(classifyAttempts(attempts, m).outcome, "product-failure");
});

test("every matrix cell must reproduce consistently, not just the first cell", () => {
    const a = admission();
    a.manifest.matrix.push({ ...a.manifest.matrix[0], id: "debug", configuration: "Debug" });
    seal(a.manifest);
    a.stable = observation("released", "product-failure", a.manifest);
    a.main = observation("source", "product-failure", a.manifest);
    a.good = observation("source", "pass", a.manifest, sha("good"));
    a.budget.authorization.manifestDigest = fingerprintInput(a.manifest);
    a.budget.authorization.inputDigest = a.manifest.approvedInputDigest;
    a.budget.authenticatedObservationDigests = [a.stable, a.main, a.good].map(fingerprintInput);
    assert.equal(admitBisection(a).decision, "admit");
    a.good.cells[1].attempts[1] = attempt("product-failure");
    a.budget.authenticatedObservationDigests = [a.stable, a.main, a.good].map(fingerprintInput);
    assert.equal(admitBisection(a).decision, "blocked");
});

test("budget boundary includes the whole worker reservation and retry cost", () => {
    const a = admission();
    a.budget.consumed = { sourceBuilds: 23, workerUnits: 39, reservedMinutes: 610 };
    a.budget.attempts = [{ workItemId: a.budget.nextWorkItemId, count: 1 }];
    assert.equal(admitBisection(a).decision, "admit");
    a.budget.consumed.reservedMinutes++;
    assert.match(admitBisection(a).reason, /budget/);
});

test("a charged reservation remains usable at the allowance boundary, but cannot cover another item", () => {
    const a = admission();
    a.budget.consumed = { sourceBuilds: 24, workerUnits: 40, reservedMinutes: 720 };
    a.budget.attempts = [{ workItemId: a.budget.nextWorkItemId, count: 2 }];
    a.budget.pendingWork = {
        id: "last-reservation", allowanceId: a.budget.allowanceId, workItemId: a.budget.nextWorkItemId,
        attempt: 2, reserved: { sourceBuilds: 1, workerUnits: 1, reservedMinutes: 110 }, status: "reserved",
    };
    assert.equal(admitBisection(a).decision, "admit");
    a.budget.pendingWork.workItemId = "unreserved-item";
    assert.equal(admitBisection(a).decision, "blocked");
});

test("unavailable runtime can be recorded as untestable without fabricating its identity", () => {
    const o = observation("source", "pass", manifest());
    o.cells[0].runtime = null;
    for (const a of o.cells[0].attempts) a.checks.runtime = false;
    o.cells[0].outcome = o.outcome = classifyAttempts(o.cells[0].attempts, manifest());
    assert.equal(o.outcome.reason, "unavailable-runtime");
    assert.equal(validateObservation(o, manifest()), true);
    o.cells[0].attempts[0].checks.runtime = true;
    rejects(() => validateObservation(o, manifest()), /identity/);
});

test("canonical hashes preserve array order, exact source bytes, and diagnostic whitespace", () => {
    assert.equal(fingerprintInput({ b: 2, a: { d: 4, c: 3 } }), fingerprintInput({ a: { c: 3, d: 4 }, b: 2 }));
    for (const [a, b] of [
        [["a", "b"], ["b", "a"]], ["source\r\n", "source\n"], ["FS0001: a b", "FS0001: a  b"],
        [{ body: "old" }, { body: "corrected" }],
        [{ comments: [{ id: 1, body: "old", updatedAt: "a" }] }, { comments: [{ id: 1, body: "edited", updatedAt: "b" }] }],
    ]) assert.notEqual(fingerprintInput(a), fingerprintInput(b));
    for (const value of [undefined, NaN, Infinity, { a: undefined }, new Date(), () => {}]) {
        rejects(() => fingerprintInput(value), /JSON/);
    }
    const cycle = {};
    cycle.self = cycle;
    rejects(() => fingerprintInput(cycle), /JSON/);
    rejects(() => fingerprintInput(Array(1)), /JSON/);
});

test("pending reservations and cancelled attempts remain charged in state", () => {
    const s = state();
    s.budget.attempts = [{ workItemId: s.budget.nextWorkItemId, count: 1 }];
    s.budget.pendingWork = {
        id: "reservation-1", allowanceId: s.budget.allowanceId, workItemId: s.budget.nextWorkItemId,
        attempt: 1, reserved: { sourceBuilds: 1, workerUnits: 1, reservedMinutes: 110 }, status: "reserved",
    };
    assert.equal(validateState(s), true);
    s.budget.pendingWork.status = "running";
    assert.equal(validateState(s), true);
    const consumed = structuredClone(s.budget.consumed);
    s.budget.pendingWork = null;
    assert.equal(validateState(s), true);
    assert.deepEqual(s.budget.consumed, consumed);
    s.budget.consumed.workerUnits = 0;
    rejects(() => validateState(s), /charged/);
});

test("workflow-looking text never erases discussion records", () => {
    const input = { issue: { body: "repro" }, comments: [{ id: 1, body: "<!-- regression-bisect -->", author: "github-actions[bot]" }] };
    assert.notEqual(fingerprintInput(input), fingerprintInput({ ...input, comments: [] }));
    input.comments[0].authenticated = true;
    assert.notEqual(fingerprintInput(input), fingerprintInput({ ...input, comments: [] }));
});

test("approval and input are independent of main; epoch binds main and stable", () => {
    const args = admission();
    const first = fingerprintEpoch(args.manifest, args.budget, args.ancestry.pinnedMainSha);
    const approvalDigest = args.manifest.approvedInputDigest;
    assert.notEqual(first, fingerprintEpoch(args.manifest, args.budget, sha("new-main")));
    args.budget.stableSdk.version = "10.0.101";
    assert.notEqual(first, fingerprintEpoch(args.manifest, args.budget, args.ancestry.pinnedMainSha));
    assert.equal(args.manifest.approvedInputDigest, approvalDigest);
    for (const edit of [m => m.sources[0].sha256 = digest("edit"), m => m.assertions.failure.stderr += " "]) {
        const m = manifest();
        edit(m);
        rejects(() => validateManifest(m), /digest/);
    }
});

test("state contract carries durable allowance, reservations, evidence, interval and publication", () => {
    assert.equal(validateState(state()), true);
    for (const edit of [
        s => delete s.budget.pendingWork,
        s => s.schemaVersion = 2,
        s => s.epoch = digest("wrong"),
        s => s.budget.consumed.workerUnits = -1,
        s => s.budget.pendingWork = { id: "unaccounted" },
        s => s.budget.allowanceId = "",
        s => s.phase = "anything",
        s => s.endpointEvidence.main = "not a digest",
        s => s.publication.commentId = -1,
        s => s.budget.authorization.approvalPath = ".github/regression-bisect/approved/other.json",
    ]) {
        const s = state();
        edit(s);
        rejects(() => validateState(s), /state/);
    }
});
