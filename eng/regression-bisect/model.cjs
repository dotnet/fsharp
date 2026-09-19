const { createHash } = require("node:crypto");
const { readFileSync } = require("node:fs");
const trustedPolicy = JSON.parse(readFileSync(require.resolve("../../.github/policies/regression-bisect.json"), "utf8"));

class ContractError extends Error {}

function check(condition, path, message) {
    if (!condition) throw new ContractError(`${path}: ${message}`);
}

const text = (v, p) => check(typeof v === "string" && v.length > 0 && v.length <= 1024, p, "expected bounded nonempty string");
const integer = (v, p) => check(Number.isSafeInteger(v) && v >= 0, p, "expected nonnegative safe integer");
const positive = (v, p) => { integer(v, p); check(v > 0, p, "expected positive integer"); };
const bool = (v, p) => check(typeof v === "boolean", p, "expected boolean");
const literal = expected => (v, p) => check(v === expected, p, `expected ${expected}`);
const oneOf = values => (v, p) => check(values.includes(v), p, `expected one of ${values.join(", ")}`);
const matching = regex => (v, p) => check(typeof v === "string" && regex.test(v), p, "invalid format");
const digest = matching(/^[0-9a-f]{64}$/);
const sourceSha = matching(/^[0-9a-f]{40}$/);
const version = matching(/^\d+\.\d+\.\d+(?:\.\d+)?$/);
const pinnedVersion = matching(/^\d+\.\d+\.\d+(?:\.\d+)?(?:-[0-9A-Za-z]+(?:[.-][0-9A-Za-z]+)*)?$/);
const nullable = validate => (v, p) => { if (v !== null) validate(v, p); };

function object(value, fields, path) {
    check(value !== null && typeof value === "object" && Object.getPrototypeOf(value) === Object.prototype, path, "expected JSON object");
    check(Object.keys(value).length === Object.keys(fields).length && Object.keys(value).every(k => Object.hasOwn(fields, k)),
        path, "missing or unknown fields");
    for (const [key, validate] of Object.entries(fields)) validate(value[key], `${path}.${key}`);
}

const shape = fields => (v, p) => object(v, fields, p);
const list = (validate, min = 0, max = 256) => (v, p) => {
    check(Array.isArray(v) && v.length >= min && v.length <= max, p, `expected array with ${min}..${max} entries`);
    for (let i = 0; i < v.length; i++) validate(v[i], `${p}[${i}]`);
};
const unique = (values, p) => check(new Set(values).size === values.length, p, "duplicate entries");

function fingerprintInput(value) {
    const ancestors = new Set();
    function canonical(v) {
        if (v === null || typeof v === "string" || typeof v === "boolean") return JSON.stringify(v);
        if (typeof v === "number") {
            check(Number.isFinite(v), "JSON", "nonfinite number");
            return JSON.stringify(v);
        }
        check(typeof v === "object" && v !== null && !ancestors.has(v), "JSON", "unsupported or cyclic value");
        ancestors.add(v);
        let result;
        if (Array.isArray(v)) {
            check(Object.keys(v).length === v.length, "JSON", "sparse array or extra fields");
            result = `[${Array.from(v, canonical).join(",")}]`;
        } else {
            check(Object.getPrototypeOf(v) === Object.prototype, "JSON", "expected plain object");
            check(Reflect.ownKeys(v).length === Object.keys(v).length, "JSON", "non-JSON properties");
            result = `{${Object.keys(v).sort().map(k => `${JSON.stringify(k)}:${canonical(v[k])}`).join(",")}}`;
        }
        ancestors.delete(v);
        return result;
    }
    return createHash("sha256").update(canonical(value)).digest("hex");
}

const equal = (a, b) => fingerprintInput(a) === fingerprintInput(b);

function validTimestamp(value) {
    if (typeof value !== "string") return false;
    const match = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})(?:\.\d{1,3})?(Z|[+-]\d{2}:\d{2})$/.exec(value);
    if (!match) return false;
    const [, y, mo, d, h, mi, s, zone] = match;
    const days = new Date(Date.UTC(Number(y), Number(mo), 0)).getUTCDate();
    return Number(mo) >= 1 && Number(mo) <= 12 && Number(d) >= 1 && Number(d) <= days &&
        Number(h) < 24 && Number(mi) < 60 && Number(s) < 60 &&
        (zone === "Z" || (Number(zone.slice(1, 3)) <= 23 && Number(zone.slice(4)) < 60)) &&
        Number.isFinite(Date.parse(value));
}

function isEligibleIssue(issue) {
    return !!issue && typeof issue === "object" && !Object.hasOwn(issue, "pull_request") &&
        issue.state === trustedPolicy.selection.state && validTimestamp(issue.created_at) &&
        Date.parse(issue.created_at) >= Date.parse(trustedPolicy.selection.createdAtOrAfter) &&
        Array.isArray(issue.labels) && issue.labels.some(label =>
            (typeof label === "string" ? label : label?.name) === trustedPolicy.selection.label);
}

function validatePolicy(policy) {
    check(policy !== null && typeof policy === "object", "policy", "expected policy object");
    check(policy.limits !== null && typeof policy.limits === "object", "policy.limits", "missing limits");
    for (const name of Object.keys(trustedPolicy.limits)) positive(policy.limits[name], `policy.limits.${name}`);
    check(equal(policy, trustedPolicy), "policy", "must match the policy loaded from the pinned trusted revision; overrides are forbidden");
    return true;
}

function repositoryPath(value, path) {
    text(value, path);
    check(value.split("/").every(part =>
        /^[A-Za-z0-9_.-]+$/.test(part) && part !== "." && part !== ".." && !part.endsWith(".") &&
        !/^(con|prn|aux|nul|com[1-9]|lpt[1-9])(?:\.|$)/i.test(part)),
    path, "expected safe repository-relative path");
}

function httpsUrl(value, path) {
    text(value, path);
    let url;
    try { url = new URL(value); } catch (error) {
        if (!(error instanceof TypeError)) throw error;
        throw new ContractError(`${path}: invalid URL`);
    }
    check(url.protocol === "https:" && !url.username && !url.password && !url.hash, path, "expected credential-free HTTPS URL");
}

function evidenceText(value, path) {
    check(typeof value === "string" && Buffer.byteLength(value, "utf8") <= trustedPolicy.evidenceBytesPerStream,
        path, "missing or oversized assertion evidence");
}

const exitCode = (v, p) => check(Number.isSafeInteger(v) && v >= -2147483648 && v <= 4294967295, p, "invalid exit code");
const assertion = shape({ exitCode, stdout: evidenceText, stderr: evidenceText });
const identity = shape({ name: text, version: pinnedVersion, sha256: digest });
const os = oneOf(["linux", "windows", "macos"]);
const architecture = oneOf(["x64", "arm64"]);
const matrixCell = shape({
    id: text, os, architecture, project: nullable(repositoryPath),
    tfm: matching(/^net(?:standard|coreapp)?\d+\.\d+(?:-[a-z0-9.]+)?$/),
    configuration: oneOf(["Debug", "Release"]),
    languageVersion: matching(/^(?:\d+\.\d+|preview)$/),
    command: list(text, 2, 2),
});

function validateManifest(manifest) {
    object(manifest, {
        schemaVersion: literal(1), repository: literal(trustedPolicy.repository), issueNumber: positive,
        discussionFingerprint: digest, kind: oneOf(["fsi", "project"]),
        sources: list(shape({ path: repositoryPath, sha256: digest }), 1),
        assertions: shape({ pass: assertion, failure: assertion }),
        matrix: list(matrixCell, 1, 64), timeoutSeconds: positive,
        producer: identity, packages: list(shape({ id: text, version: pinnedVersion, sha256: digest })),
        searchHints: shape({ sdkVersions: list(version), sourceShas: list(sourceSha), urls: list(httpsUrl) }),
        approvedInputDigest: digest,
    }, "manifest");
    check(manifest.timeoutSeconds <= trustedPolicy.limits.reproductionTimeoutSeconds, "manifest.timeoutSeconds", "exceeds policy");
    unique(manifest.sources.map(s => s.path.toLowerCase()), "manifest.sources");
    unique(manifest.packages.map(p => p.id.toLowerCase()), "manifest.packages");
    unique(manifest.matrix.map(c => c.id), "manifest.matrix");
    check(!equal(manifest.assertions.pass, manifest.assertions.failure), "manifest.assertions", "pass and failure must differ");
    const paths = new Set(manifest.sources.map(s => s.path));
    for (const cell of manifest.matrix) {
        const operation = manifest.kind === "fsi" ? "fsi" : "run-project";
        check(cell.command[0] === operation && paths.has(cell.command[1]), "manifest.matrix.command", "expected allowed operation and reviewed source");
        check(manifest.kind === "fsi"
            ? cell.project === null && cell.command[1].endsWith(".fsx")
            : cell.project === cell.command[1] && cell.project.endsWith(".fsproj"),
        "manifest.matrix.project", "command does not match reproduction kind");
    }
    const { approvedInputDigest, ...input } = manifest;
    check(approvedInputDigest === fingerprintInput(input), "manifest.approvedInputDigest", "digest mismatch");
    return true;
}

const untestableReasons = [
    "restore-failure", "build-failure", "setup-failure", "wrong-identity", "unavailable-sdk", "unavailable-runtime",
    "checksum-provenance", "network-failure",
    "timeout", "incomplete-evidence", "flaky", "unrelated-exception", "assertion-mismatch",
];
function validateOutcome(outcome) {
    object(outcome, {
        schemaVersion: literal(1), outcome: oneOf(["pass", "product-failure", "untestable"]), reason: text,
    }, "outcome");
    check(outcome.outcome === "pass" ? outcome.reason === "positive-assertion"
        : outcome.outcome === "product-failure" ? outcome.reason === "failure-assertion"
            : untestableReasons.includes(outcome.reason), "outcome.reason", "reason does not match outcome");
    return true;
}

const outcome = (value, reason) => ({ schemaVersion: 1, outcome: value, reason });
const untestable = reason => outcome("untestable", reason);
const checks = {
    checksumProvenance: "checksum-provenance",
    restore: "restore-failure", build: "build-failure", setup: "setup-failure", identity: "wrong-identity",
    sdk: "unavailable-sdk", runtime: "unavailable-runtime",
};
const attemptSchema = shape({
    schemaVersion: literal(1),
    checks: shape(Object.fromEntries(Object.keys(checks).map(k => [k, bool]))),
    execution: shape({
        status: oneOf(["completed", "timeout", "exception"]), complete: bool,
        exitCode: nullable(exitCode), stdout: evidenceText, stderr: evidenceText,
    }),
});

function classifyAttempts(attempts, manifest) {
    validateManifest(manifest);
    try {
        list(attemptSchema, trustedPolicy.limits.assertionRepetitions, trustedPolicy.limits.assertionRepetitions)(attempts, "attempts");
    } catch (error) {
        if (!(error instanceof ContractError)) throw error;
        return untestable("incomplete-evidence");
    }
    const results = attempts.map(attempt => {
        for (const [name, reason] of Object.entries(checks)) {
            if (!attempt.checks[name]) return untestable(reason);
        }
        const execution = attempt.execution;
        if (execution.status === "timeout") return untestable("timeout");
        if (execution.status === "exception") return untestable("unrelated-exception");
        if (!execution.complete || execution.exitCode === null) return untestable("incomplete-evidence");
        const actual = { exitCode: execution.exitCode, stdout: execution.stdout, stderr: execution.stderr };
        if (equal(actual, manifest.assertions.pass)) return outcome("pass", "positive-assertion");
        if (equal(actual, manifest.assertions.failure)) return outcome("product-failure", "failure-assertion");
        return untestable("assertion-mismatch");
    });
    const failedSetup = results.find(r => r.outcome === "untestable");
    if (failedSetup) return failedSetup;
    return results.every(r => equal(r, results[0])) ? results[0] : untestable("flaky");
}

function stableSdk(value, path) {
    object(value, {
        version, origin: literal("official-release-index"), latest: literal(true), verified: literal(true),
        releaseIndexUrl: httpsUrl, releaseIndexDigest: digest,
        archives: list(shape({ os, architecture, sha256: digest }), 1, 6),
    }, path);
    unique(value.archives.map(a => `${a.os}-${a.architecture}`), `${path}.archives`);
    const url = new URL(value.releaseIndexUrl);
    check(["builds.dotnet.microsoft.com", "dotnetcli.blob.core.windows.net"].includes(url.hostname) &&
        url.pathname === "/dotnet/release-metadata/releases-index.json" && !url.search && !url.port,
    path, "expected official stable release index");
}

const productIdentity = shape({ version: pinnedVersion, sha256: digest, verified: literal(true) });
const artifact = shape({
    sha256: digest, origin: oneOf(["local-build", "released-sdk"]),
    sourceSha: nullable(sourceSha), buildInstanceId: text,
});
const archiveReceipt = (v, p) => object(v, {
    name: text, url: httpsUrl, rid: text, architecture,
    provenance: (v, p) => object(v, v?.kind === "publisher-checksum"
        ? { kind: literal("publisher-checksum") }
        : { kind: literal("approved-official-https-measured"), approvalRevision: sourceSha, approvalDigest: digest }, p),
    ...(v?.provenance?.kind === "publisher-checksum" ? { sha512: matching(/^[a-f0-9]{128}$/) } : {}),
    sha256: digest, actualSha512: matching(/^[a-f0-9]{128}$/), redirectUrl: httpsUrl, finalOrigin: httpsUrl,
    redirects: list(shape({ from: httpsUrl, to: httpsUrl, status: oneOf([301, 302, 303, 307, 308]) }), 0, 5),
    bytesReceived: positive, receivedAt: (v, p) => check(validTimestamp(v), p, "invalid receipt time"),
    retained: shape({ path: text, sha256: digest, sha512: matching(/^[a-f0-9]{128}$/), size: positive }),
}, p);
const buildToolchain = shape({
    sourceSha, globalJsonDigest: digest, evidenceDigest: digest,
    requestedSdk: pinnedVersion, requestedTools: pinnedVersion, requestedCompilerRuntime: pinnedVersion,
    sdk: productIdentity, compilerRuntime: productIdentity, archive: archiveReceipt,
    verified: literal(true), supported: literal(true),
});
const observationCell = (v, p) => object(v, {
    id: text, command: list(text, 2, 2),
    environment: shape({ os, architecture, imageDigest: digest }),
    sdk: nullable(productIdentity), runtime: nullable(productIdentity),
    artifacts: shape({ compiler: nullable(artifact), tasks: nullable(artifact), targets: nullable(artifact), core: nullable(artifact) }),
    producer: identity, attempts: list(attemptSchema, 1, trustedPolicy.limits.assertionRepetitions), outcome: validateOutcome,
    ...(v && Object.hasOwn(v, "buildToolchain") ? { buildToolchain } : {}),
}, p);

function validateObservation(observation, manifest, options = { finalConfirmation: null }) {
    validateManifest(manifest);
    object(observation, {
        schemaVersion: literal(1), kind: oneOf(["source", "released"]),
        target: (v, p) => object(v, observation.kind === "source" ? { sourceSha } : { sdk: stableSdk }, p),
        manifestDigest: digest, inputDigest: digest, discussionFingerprint: digest,
        matrixFingerprint: digest, producerFingerprint: digest, packageFingerprint: digest,
        buildInstanceId: text, cached: bool, cells: list(observationCell, 1, 64), outcome: validateOutcome,
        provenance: shape({ workerId: text, runId: text, workflowRevision: sourceSha, policyRevision: sourceSha }),
    }, "observation");
    object(options, {
        finalConfirmation: nullable(shape({ runId: text, buildInstanceId: text })),
    }, "observation.fresh");
    const final = options.finalConfirmation;
    check(final === null || (!observation.cached && observation.provenance.runId === final.runId &&
        observation.buildInstanceId === final.buildInstanceId), "observation", "final confirmation requires fresh execution");
    for (const [name, expected] of Object.entries({
        manifestDigest: fingerprintInput(manifest), inputDigest: manifest.approvedInputDigest,
        discussionFingerprint: manifest.discussionFingerprint, matrixFingerprint: fingerprintInput(manifest.matrix),
        producerFingerprint: fingerprintInput(manifest.producer), packageFingerprint: fingerprintInput(manifest.packages),
    })) check(observation[name] === expected, `observation.${name}`, "input mismatch");
    unique(observation.cells.map(c => c.id), "observation.cells");
    check(observation.cells.length === manifest.matrix.length, "observation.cells", "incomplete matrix");
    for (const cell of observation.cells) {
        const expected = manifest.matrix.find(c => c.id === cell.id);
        check(expected && equal(cell.command, expected.command) && cell.environment.os === expected.os &&
            cell.environment.architecture === expected.architecture, "observation.cells", "matrix or command mismatch");
        check(equal(cell.producer, manifest.producer), "observation.cells.producer", "producer mismatch");
        if (cell.buildToolchain) {
            const build = cell.buildToolchain;
            check(observation.kind === "source" && build.sourceSha === observation.target.sourceSha &&
                build.requestedSdk === build.sdk.version && build.requestedTools === build.sdk.version &&
                build.requestedCompilerRuntime === build.compilerRuntime.version,
            "observation.cells.buildToolchain", "wrong source declaration or resolved build SDK");
            const { validateArchiveReceipt } = require("./archive-provenance.cjs");
            try {
                validateArchiveReceipt({
                    policy: trustedPolicy, kind: observation.kind, sourceSha: build.sourceSha, product: "sdk",
                    version: build.requestedSdk, platform: { ...cell.environment, rid: build.archive.rid }, archive: build.archive,
                }, build.archive);
            } catch (error) { throw new ContractError(`observation.cells.buildToolchain.archive: ${error.message}`); }
            const rid = `${{ windows: "win", linux: "linux", macos: "osx" }[cell.environment.os]}-${cell.environment.architecture}`;
            check(build.archive.sha256 === build.sdk.sha256 && build.archive.architecture === cell.environment.architecture &&
                (build.archive.rid === rid || cell.environment.os === "linux" && build.archive.rid === `linux-musl-${cell.environment.architecture}`),
            "observation.cells.buildToolchain.archive", "archive does not match build SDK or platform");
        }
        for (const [name, missing] of Object.entries({
            sdk: cell.sdk === null, runtime: cell.runtime === null,
            identity: Object.values(cell.artifacts).some(a => a === null),
        })) {
            check(!missing || (cell.outcome.outcome === "untestable" && cell.attempts.every(a => !a.checks[name])),
                "observation.cells.identity", "absent identities require untestable checks");
        }
        for (const value of Object.values(cell.artifacts)) {
            if (value === null) continue;
            check(value.origin === (observation.kind === "source" ? "local-build" : "released-sdk") &&
                value.sourceSha === (observation.kind === "source" ? observation.target.sourceSha : null) &&
                value.buildInstanceId === observation.buildInstanceId, "observation.cells.artifacts", "wrong source or build identity");
        }
        if (observation.kind === "released" && cell.sdk !== null) {
            const archive = observation.target.sdk.archives.find(a =>
                a.os === cell.environment.os && a.architecture === cell.environment.architecture);
            check(cell.sdk.version === observation.target.sdk.version && cell.sdk.sha256 === archive?.sha256,
                "observation.cells.sdk", "released SDK identity mismatch");
        }
        check(equal(cell.outcome, classifyAttempts(cell.attempts, manifest)), "observation.cells.outcome", "does not match repeated evidence");
    }
    const results = observation.cells.map(c => c.outcome);
    const aggregate = results.find(r => r.outcome === "untestable") ??
        (results.every(r => equal(r, results[0])) ? results[0] : untestable("flaky"));
    check(equal(observation.outcome, aggregate), "observation.outcome", "does not match matrix evidence");
    return true;
}

const counters = shape({ sourceBuilds: integer, workerUnits: integer, reservedMinutes: integer });
const reservation = shape({
    id: text, allowanceId: text, workItemId: text, attempt: positive,
    reserved: counters, status: oneOf(["reserved", "running"]),
});
function validateBudget(budget, manifest) {
    object(budget, {
        schemaVersion: literal(1), policy: validatePolicy, policyRevision: sourceSha, workflowRevision: sourceSha,
        allowanceId: text,
        authorization: shape({
            repository: literal(trustedPolicy.repository), issueNumber: positive,
            approvalPath: repositoryPath, approvalRevision: sourceSha, manifestDigest: digest, inputDigest: digest,
        }),
        stableSdk, authenticatedObservationDigests: list(digest),
        consumed: counters, attempts: list(shape({ workItemId: text, count: positive })),
        nextWorkItemId: text, pendingWork: nullable(reservation),
    }, "budget");
    const auth = budget.authorization;
    check(auth.repository === manifest.repository && auth.issueNumber === manifest.issueNumber &&
        auth.approvalPath === `${trustedPolicy.authorization.approvalDirectory}/${manifest.issueNumber}.json` &&
        auth.approvalRevision === budget.policyRevision &&
        auth.manifestDigest === fingerprintInput(manifest) && auth.inputDigest === manifest.approvedInputDigest,
    "budget.authorization", "missing matching pinned approval");
    for (const name of Object.keys(budget.consumed)) {
        check(budget.consumed[name] <= budget.policy.limits[name], `budget.consumed.${name}`, "exceeds allowance");
    }
    unique(budget.attempts.map(a => a.workItemId), "budget.attempts");
    check(budget.attempts.every(a => a.count <= budget.policy.limits.attemptsPerItem), "budget.attempts", "exceeds attempt limit");
    check(budget.attempts.reduce((sum, a) => sum + a.count, 0) <= budget.consumed.workerUnits,
        "budget.attempts", "attempts must be charged");
    if (budget.pendingWork !== null) {
        const pending = budget.pendingWork;
        check(pending.allowanceId === budget.allowanceId && pending.workItemId === budget.nextWorkItemId &&
            pending.attempt === budget.attempts.find(a => a.workItemId === pending.workItemId)?.count &&
            pending.reserved.workerUnits === 1 && pending.reserved.sourceBuilds <= 1 &&
            pending.reserved.reservedMinutes === budget.policy.limits.workerTimeoutMinutes,
        "budget.pendingWork", "reservation must be charged to this allowance and attempt");
        for (const name of Object.keys(pending.reserved)) {
            check(pending.reserved[name] <= budget.consumed[name], "budget.pendingWork", "unaccounted reservation");
        }
    }
}

function fingerprintEpoch(manifest, budget, pinnedMainSha) {
    validateManifest(manifest);
    validateBudget(budget, manifest);
    sourceSha(pinnedMainSha, "epoch.pinnedMainSha");
    return fingerprintInput({
        schemaVersion: 1, manifestDigest: fingerprintInput(manifest),
        policyRevision: budget.policyRevision, workflowRevision: budget.workflowRevision,
        pinnedMainSha, stableSdk: budget.stableSdk,
    });
}

function admitBisection(input) {
    try {
        object(input, {
            manifest: validateManifest, stable: () => {}, main: () => {}, good: () => {},
            ancestry: shape({ goodSha: sourceSha, badSha: sourceSha, pinnedMainSha: sourceSha, isAncestor: bool }),
            budget: () => {},
        }, "admission");
        const { manifest, stable, main, good, ancestry, budget } = input;
        validateBudget(budget, manifest);
        for (const observation of [stable, main, good]) {
            validateObservation(observation, manifest);
            check(budget.authenticatedObservationDigests.includes(fingerprintInput(observation)),
                "admission", "observation not authenticated by the trusted collector");
            check(observation.provenance.policyRevision === budget.policyRevision &&
                observation.provenance.workflowRevision === budget.workflowRevision, "admission", "provenance mismatch");
        }
        check(stable.kind === "released" && main.kind === "source" && good.kind === "source",
            "admission", "missing source interval or released stable endpoint");
        check(equal(stable.target.sdk, budget.stableSdk), "admission", "not the pinned latest stable SDK");
        check(main.target.sourceSha === ancestry.pinnedMainSha && main.target.sourceSha === ancestry.badSha &&
            good.target.sourceSha === ancestry.goodSha && ancestry.goodSha !== ancestry.badSha && ancestry.isAncestor,
        "admission", "good must be a strict ancestor of pinned main");
        const toolchainTransitions = [];
        for (const cell of main.cells) {
            const older = good.cells.find(c => c.id === cell.id);
            const released = stable.cells.find(c => c.id === cell.id);
            check(equal(cell.environment, older.environment) && equal(cell.environment, released.environment) &&
                equal(cell.sdk, older.sdk) && equal(cell.runtime, older.runtime), "admission", "incomparable environment or source toolchain");
            check(!!cell.buildToolchain === !!older.buildToolchain, "admission", "incomplete revision-specific toolchain evidence");
            if (cell.buildToolchain && (!equal(cell.buildToolchain.sdk, older.buildToolchain.sdk) ||
                !equal(cell.buildToolchain.compilerRuntime, older.buildToolchain.compilerRuntime))) {
                toolchainTransitions.push({ cellId: cell.id, good: older.buildToolchain, main: cell.buildToolchain });
            }
        }
        if ([stable, main, good].some(o => o.outcome.outcome === "untestable")) {
            return { decision: "blocked", reason: "untestable endpoint evidence" };
        }
        if (stable.outcome.outcome !== "product-failure") return { decision: "stop", reason: "not reproduced on latest stable" };
        if (main.outcome.outcome !== "product-failure") return { decision: "stop", reason: "release-specific or already fixed on main" };
        if (good.outcome.outcome !== "pass") return { decision: "stop", reason: "missing passing source endpoint" };
        const limits = budget.policy.limits;
        check(budget.pendingWork !== null ? budget.pendingWork.reserved.sourceBuilds === 1 :
            budget.consumed.sourceBuilds < limits.sourceBuilds && budget.consumed.workerUnits < limits.workerUnits &&
            budget.consumed.reservedMinutes + limits.workerTimeoutMinutes <= limits.reservedMinutes &&
            (budget.attempts.find(a => a.workItemId === budget.nextWorkItemId)?.count ?? 0) < limits.attemptsPerItem,
        "admission", "insufficient remaining budget");
        return {
            decision: "admit", reason: "verified stable failure, pinned-main failure, and comparable ancestor pass",
            ...(toolchainTransitions.length ? { toolchainTransitions } : {}),
        };
    } catch (error) {
        if (!(error instanceof ContractError)) throw error;
        return { decision: "blocked", reason: error.message };
    }
}

function validateState(state) {
    try {
        object(state, {
            schemaVersion: literal(1), repository: literal(trustedPolicy.repository), issueNumber: positive,
            issueFingerprint: digest, approvalFingerprint: digest, manifest: validateManifest,
            budget: () => {}, pinnedMainSha: sourceSha, epoch: digest,
            phase: oneOf(["discovery", "stable", "main", "good-search", "admission", "bisect", "confirmation", "complete", "blocked", "stopped"]),
            endpointEvidence: shape({ stable: nullable(digest), main: nullable(digest), good: nullable(digest) }),
            testedRevisions: list(shape({ sourceSha, observationDigest: digest })),
            skips: list(shape({ sourceSha, reason: oneOf(untestableReasons), observationDigest: digest })),
            interval: nullable(shape({ goodSha: sourceSha, badSha: sourceSha, gitLog: list(text, 0, 1000) })),
            publication: shape({ digest: nullable(digest), commentId: nullable(positive) }),
        }, "state");
        validateBudget(state.budget, state.manifest);
        check(state.repository === state.manifest.repository && state.issueNumber === state.manifest.issueNumber &&
            state.issueFingerprint === state.manifest.discussionFingerprint &&
            state.approvalFingerprint === fingerprintInput(state.manifest) &&
            state.epoch === fingerprintEpoch(state.manifest, state.budget, state.pinnedMainSha),
        "state", "identity or epoch mismatch");
        const budget = state.budget;
        const evidence = [...Object.values(state.endpointEvidence).filter(x => x !== null),
            ...state.testedRevisions.map(r => r.observationDigest), ...state.skips.map(r => r.observationDigest)];
        check(evidence.every(d => budget.authenticatedObservationDigests.includes(d)), "state", "unauthenticated evidence");
        return true;
    } catch (error) {
        if (!(error instanceof ContractError)) throw error;
        throw new ContractError(`state: ${error.message}`);
    }
}

module.exports = {
    isEligibleIssue, validatePolicy, validateManifest, fingerprintInput, classifyAttempts,
    validateObservation, admitBisection, validateOutcome, validateState, fingerprintEpoch,
};
