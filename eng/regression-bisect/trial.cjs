const fs = require("node:fs/promises");
const path = require("node:path");
const { createHmac, randomUUID } = require("node:crypto");
const { execFileSync } = require("node:child_process");
const controller = require("./controller-cli.cjs");
const state = require("./state.cjs");
const model = require("./model.cjs");
const { requireValue, hash, inside } = require("./toolchain-io.cjs");
const { connect, writeDatabase } = require("./fixtures/controller-service.cjs");
const { createGithubApi } = require("./github.cjs");
const { createFileEvidenceStore } = require("./evidence-store.cjs");

const root = path.resolve(__dirname, "..", "..");
const purpose = "SYNTHETIC ACCEPTANCE ONLY";
const usage = `Usage:
  node eng\\regression-bisect\\trial.cjs --prepare <absolute-reviewed-config.json> <absolute-new-evidence-directory>
  node eng\\regression-bisect\\trial.cjs --advance <absolute-existing-evidence-directory>
  node eng\\regression-bisect\\trial.cjs --verify <absolute-existing-evidence-directory>
  node eng\\regression-bisect\\trial.cjs --help
One advance performs one controller transition or at most one charged real worker.
No synthetic worker, reset, refund, deadline extension or live GitHub transport.
Harness tests are not worker/product or physical known-history acceptance.`;
const decode = raw => JSON.parse(new TextDecoder("utf-8", { fatal: true }).decode(raw));
const json = file => controller.bytes(file, 4 * 1024 * 1024).then(decode);
const equal = (a, b) => model.fingerprintInput(a) === model.fingerprintInput(b);
const fields = (v, names) => requireValue(v && Object.keys(v).sort().join(" ") === names.split(" ").sort().join(" "),
    `unexpected trial fields (expected ${names})`);
const sign = (value, key) => createHmac("sha256", key).update("regression-trial-v1\0")
    .update(model.fingerprintInput(value)).digest("hex");

function environment(config, directory, supplied, role = "controller") {
    requireValue(!supplied.GITHUB_ACTIONS && !supplied.RB_LOCAL_TEST && !supplied.RB_LOCAL_TEST_FILE &&
        !supplied.NODE_OPTIONS, "trial cannot enable Actions, preloads or synthetic execution");
    return { RB_SIGNING_KEYS: supplied.RB_SIGNING_KEYS, RB_TRIAL_DIRECTORY: directory,
        RB_STATE_DIRECTORY: path.join(directory, "state"), RB_CONTAINMENT_DIRECTORY: config.containmentDirectory,
        RB_TRUSTED_REVISION: config.helperRevision, GITHUB_WORKFLOW_SHA: config.helperRevision,
        GITHUB_REPOSITORY: "dotnet/fsharp", GITHUB_REF: "refs/heads/main",
        GITHUB_RUN_ID: config.runs[role], GITHUB_RUN_ATTEMPT: "1",
        RB_GITHUB_TOKEN: "offline-controller-token", RB_DISPATCH_TOKEN: "offline-separate-dispatch-token" };
}

function git(config, args, raw = false) {
    const output = execFileSync(config.deployment.gitExecutable, ["-c", "core.hooksPath=", "-c", "credential.helper=",
        "-c", "core.fsmonitor=false", ...args], { cwd: config.source.directory, encoding: raw ? undefined : "utf8", timeout: 60000,
        maxBuffer: 4 * 1024 * 1024, shell: false, windowsHide: true,
        env: { ...(process.platform === "win32" ? { SystemRoot: process.env.SystemRoot } : {}),
            GIT_CONFIG_NOSYSTEM: "1", GIT_CONFIG_GLOBAL: path.join(config.source.directory, "trial-no-config"),
            GIT_NO_REPLACE_OBJECTS: "1", GIT_TERMINAL_PROMPT: "0", GIT_ALLOW_PROTOCOL: "file" } });
    return raw ? output : output.trim();
}

async function helpers() {
    const names = [];
    async function visit(directory) {
        for (const entry of await fs.readdir(directory, { withFileTypes: true })) {
            if (entry.name.startsWith(".") || ["node_modules", "bin", "obj"].includes(entry.name)) continue;
            const file = path.join(directory, entry.name);
            if (entry.isDirectory()) await visit(file);
            else if (/\.(cjs|cs|csproj|targets)$/.test(entry.name)) names.push(path.relative(root, file));
        }
    }
    await visit(__dirname);
    names.push(".github\\policies\\regression-bisect.json", ".github\\workflows\\regression-bisect-controller.yml",
        ".github\\workflows\\regression-bisect.lock.yml", "UseLocalCompiler.Directory.Build.props",
        "UseLocalCompiler.Directory.Build.targets", "global.json", "eng\\TargetFrameworks.props",
        "eng\\common\\tools.sh", "eng\\common\\pipeline-logging-functions.sh");
    return Promise.all(names.sort().map(async name => ({ path: name.split(/[\\/]/).join("/"),
        sha256: hash(await controller.bytes(path.join(root, ...name.split(/[\\/]/)), 1024 * 1024)) })));
}

async function validate(config, directory, supplied) {
    fields(config, "schemaVersion purpose helperRevision deployment api source stable files containmentDirectory runs signature");
    requireValue(config.schemaVersion === 1 && config.purpose === purpose, "explicit synthetic acceptance purpose required");
    fields(config.source, "directory base good candidate main");
    fields(config.runs, "controller routing supervisor");
    requireValue(new Set(Object.values(config.runs)).size === 3 &&
        Object.values(config.runs).every(r => typeof r === "string" && /^[1-9]\d{0,19}$/.test(r) &&
            Number.isSafeInteger(Number(r))), "distinct trial run identities required");
    const env = environment(config, directory, supplied);
    const context = controller.configuration(config.deployment, env);
    const { signature, ...review } = config;
    requireValue(signature === sign(review, context.keys[context.keyId]), "reviewed config signature mismatch");
    for (const name of [directory, config.source.directory, config.containmentDirectory])
        requireValue(path.isAbsolute(name) && path.resolve(name) === name && !/[\0\r\n]/.test(name),
            "canonical absolute trial paths required");
    requireValue(await fs.realpath(path.dirname(directory)) === path.dirname(directory),
        "trial evidence parent cannot resolve through links");
    requireValue(await fs.realpath(config.source.directory) === config.source.directory &&
        config.source.directory !== root && !inside(root, config.source.directory) &&
        directory !== root && !inside(root, directory) && !inside(config.source.directory, directory) &&
        !inside(directory, config.source.directory), "isolated unlinked source and evidence directories required");
    fields(config.api, "path sha256");
    requireValue(Array.isArray(config.files) && config.files.length >= 4 && config.files.length <= 32 &&
        new Set(config.files.map(f => f.role)).size === config.files.length &&
        ["good-patch", "introducing-patch", "source-toolchain", "stable-toolchain"].every(role =>
            config.files.some(f => f.role === role)), "raw patches and official toolchain metadata required");
    for (const item of config.files) fields(item, "role path sha256");
    for (const item of [config.api, ...config.files]) {
        requireValue(path.isAbsolute(item.path) && /^[a-f0-9]{64}$/.test(item.sha256) &&
            hash(await controller.bytes(item.path, 4 * 1024 * 1024)) === item.sha256, "frozen input bytes mismatch");
    }
    const { base, good, candidate, main } = config.source;
    requireValue([base, good, candidate, main].every(s => /^[a-f0-9]{40}$/.test(s)) &&
        new Set([base, good, candidate, main]).size === 4 &&
        git(config, ["rev-parse", "refs/heads/main"]) === main &&
        git(config, ["rev-list", "--parents", "-n", "1", good]) === `${good} ${base}` &&
        git(config, ["rev-list", "--parents", "-n", "1", candidate]) === `${candidate} ${good}` &&
        git(config, ["rev-list", "--parents", "-n", "1", main]) === `${main} ${candidate}` &&
        git(config, ["rev-parse", `${main}^{tree}`]) === git(config, ["rev-parse", `${candidate}^{tree}`]),
    "expected isolated good -> introducing -> no-op main graph changed");
    for (const [role, from, to] of [["good-patch", base, good], ["introducing-patch", good, candidate]])
        requireValue(config.files.find(f => f.role === role).sha256 ===
            hash(git(config, ["diff", "--no-ext-diff", "--no-textconv", "--binary", from, to], true)),
        "reviewed raw patch differs from Git source graph");
    const policy = decode(await controller.bytes(path.join(root, ".github", "policies", "regression-bisect.json")));
    model.validatePolicy(policy);
    requireValue(policy.limits.workerTimeoutMinutes === 110 && policy.limits.reservedMinutes === 720 &&
        6 * policy.limits.workerTimeoutMinutes === 660 && policy.limits.attemptsPerItem === 2,
    "six-unit policy capacity changed");
    fields(config.stable, "sdkVersion archive metadata");
    require("./archive-provenance.cjs").validateArchiveRequest({ policy, kind: "released", product: "sdk",
        version: config.stable.sdkVersion, platform: config.deployment.platform, archive: config.stable.archive });
    requireValue(config.stable.archive?.provenance?.kind === "publisher-checksum" &&
        Array.isArray(config.stable.metadata) && config.stable.metadata.length > 0, "official stable identity required");
    for (const record of config.stable.metadata) {
        fields(record, "url sha256");
        requireValue(config.files.some(f => f.role.startsWith("metadata-") && f.sha256 === record.sha256),
            "raw official metadata identity missing");
    }
    return { context, env, policy };
}

function immutableApi(db) {
    const { comments, statuses, runs, dispatches, nextId, now, ...fixed } = db;
    return fixed;
}

async function load(directory, supplied, readOnly = false) {
    requireValue(path.isAbsolute(directory) && await fs.realpath(directory) === directory, "unlinked absolute trial directory required");
    const saved = await json(path.join(directory, "trial.json"));
    const config = await json(path.join(directory, "reviewed.json"));
    const checked = await validate(config, directory, supplied);
    const { signature, ...body } = saved;
    requireValue(signature === sign(body, checked.context.keys[checked.context.keyId]) &&
        saved.directory === directory && saved.configSha256 === hash(await controller.bytes(path.join(directory, "reviewed.json"), 4 * 1024 * 1024)) &&
        equal(saved.helpers, await helpers()), "trial identity or helper drift");
    const db = await json(path.join(directory, "api.json"));
    requireValue(equal(immutableApi(db), saved.api), "fake API identity or immutable inputs changed");
    const service = await connect(path.join(directory, "api.json"), { readOnly });
    const approval = decode(Buffer.from(db.files[".github/regression-bisect/approved/123.json"], "base64"));
    return { ...checked, config, saved, db, service, approval, directory };
}

async function adapter(env) {
    const t = await load(env.RB_TRIAL_DIRECTORY, env);
    const { send, executeGit, recordGit, recordGitResult, checkAnchor } = t.service;
    // Whitelist transport/Git only: fixture outcomes, clock and runtime overrides cannot enter acceptance.
    return { send, executeGit, recordGit, recordGitResult, checkAnchor, mirrorDirectory: t.config.source.directory,
        clock: () => new Date(), validateStable: value => requireValue(equal(t.config.stable, {
            sdkVersion: value.sdkVersion, archive: value.archive,
            metadata: value.metadata.map(({ url, sha256 }) => ({ url, sha256 })),
        }), "frozen official stable metadata changed") };
}

async function prepare(configFile, directory, supplied) {
    const raw = await controller.bytes(configFile, 4 * 1024 * 1024), config = decode(raw);
    const { context } = await validate(config, directory, supplied);
    const db = await json(config.api.path);
    requireValue(db.issue?.number === 123 && model.isEligibleIssue(db.issue) && db.statuses?.length === 0 &&
        !db.dispatches?.length && Object.keys(db.faults ?? {}).length === 0 && !db.SYNTHETIC &&
        db.mirrorDirectory === config.source.directory && db.revision === config.helperRevision &&
        equal(decode(Buffer.from(db.files[".github/regression-bisect/deployment.json"], "base64")), config.deployment),
    "fresh reviewed selected-issue fake API required");
    for (const [role, id] of Object.entries(config.runs)) {
        const run = db.runs[`${id}/1`];
        requireValue(run?.id === Number(id) && run.head_sha === config.helperRevision &&
            run.path === (role === "routing" ? ".github/workflows/regression-bisect.lock.yml" : config.deployment.workflowPath),
        "reviewed fake run identity mismatch");
    }
    const approval = decode(Buffer.from(db.files[".github/regression-bisect/approved/123.json"], "base64"));
    fields(approval, "schemaVersion manifest bootstrap");
    model.validateManifest(approval.manifest);
    const registration = decode(Buffer.from(db.files[".github/regression-bisect/registrations/123.json"], "base64"));
    requireValue(equal(approval.bootstrap, registration) && registration.checkpointAnchorSha === config.deployment.anchorSha,
        "reviewed approval and registration conflict");
    const discovery = require("./routing.cjs").validateRegistration(
        decode(Buffer.from(db.files[".github/regression-bisect/discovery.json"], "base64")));
    requireValue(discovery.issueNumber === 123 && discovery.checkpointAnchorSha === config.deployment.anchorSha &&
        [discovery.commentId, discovery.freshnessCommentId].every(id =>
            ![registration.checkpointCommentId, registration.resultCommentId].includes(id)), "trial discovery slots conflict");
    requireValue(approval.manifest.searchHints.sourceShas.length === 1 &&
        approval.manifest.searchHints.sourceShas[0] === config.source.good &&
        Date.parse(approval.bootstrap.deadline) - Date.now() >= 660 * 60000,
    "frozen good source and fixed six-unit deadline required");
    for (const source of approval.manifest.sources) {
        requireValue(hash(Buffer.from(db.files[source.path], "base64")) === source.sha256, "raw sample/producer hash mismatch");
        for (const sha of [config.source.good, config.source.candidate, config.source.main])
            requireValue(hash(git(config, ["show", `${sha}:${source.path}`], true)) === source.sha256,
                "sample/producer must be immutable in every source revision");
    }
    await fs.mkdir(directory);
    requireValue(await fs.realpath(directory) === directory, "setup directory resolves through a link");
    await fs.writeFile(path.join(directory, "reviewed.json"), raw, { flag: "wx" });
    await fs.writeFile(path.join(directory, "deployment.json"), JSON.stringify(config.deployment), { flag: "wx" });
    db.trialId = randomUUID();
    for (const run of Object.values(db.runs)) { run.status = "completed"; run.conclusion = "success"; }
    writeDatabase(path.join(directory, "api.json"), db);
    const body = { schemaVersion: 1, directory, configSha256: hash(raw), helpers: await helpers(),
        api: immutableApi(db), allowanceId: approval.bootstrap.allowanceId, deadline: approval.bootstrap.deadline,
        plan: { sourceBuilds: 5, workerUnits: 6, reservedMinutes: 660,
            phases: ["stable", "main", "good-search", "bisect", "confirmation", "confirmation"] } };
    await controller.writeJson(path.join(directory, "trial.json"), { ...body, signature: sign(body, context.keys[context.keyId]) });
    return { status: "prepared", purpose, trialId: db.trialId, ...body.plan, physicalAcceptance: false };
}

async function advance(t, supplied) {
    const apiFile = path.join(t.directory, "api.json");
    const supervisor = { ...t.context, owner: { runId: t.config.runs.supervisor, attempt: 1 } };
    return controller.withIssueLock(path.join(t.directory, "supervisor"), supervisor, {
        getRun: async (id, attempt) => (await json(apiFile)).runs[`${id}/${attempt}`],
    }, async () => {
        t = await load(t.directory, supplied);
        const deliveryFile = path.join(t.directory, "delivery.json");
        let delivery = null;
        try { delivery = await json(deliveryFile); } catch (error) { if (error.code !== "ENOENT") throw error; }
        if (delivery) {
            const { signature, ...receipt } = delivery;
            requireValue(signature === sign(receipt, t.context.keys[t.context.keyId]) &&
                receipt.trialId === t.db.trialId, "delivery receipt authentication failed");
        }
        const selected = t.db.dispatches?.at(-1)?.inputs;
        const consume = selected && selected.routingKey !== delivery?.routingKey;
        const role = consume ? "controller" : "routing";
        for (const run of Object.values(t.db.runs)) { run.status = "completed"; run.conclusion = "cancelled"; }
        const runId = String(Math.max(...Object.values(t.db.runs).map(r => r.id)) + 1);
        requireValue(Number.isSafeInteger(Number(runId)), "trial run identity exhausted");
        const current = { ...t.db.runs[`${t.config.runs[role]}/1`],
            id: Number(runId), status: "in_progress", conclusion: null };
        t.db.runs[`${runId}/1`] = current;
        writeDatabase(apiFile, t.db);
        try {
            const configArgs = ["--config", path.join(t.directory, "deployment.json")];
            const env = environment(t.config, t.directory, supplied, role);
            env.GITHUB_RUN_ID = runId;
            const result = consume ? await controller.main([selected.mode, "--issue", "123", ...configArgs,
                "--routing-key", selected.routingKey, "--routing-generation", selected.routingGeneration], env) :
                await require("./routing-cli.cjs").main([
                    ...(await fs.stat(path.join(env.RB_STATE_DIRECTORY, "dotnet-fsharp-discovery", "committed.json"))
                        .then(() => true, error => { if (error.code === "ENOENT") return false; throw error; })
                        ? ["event", "--issue", "123"] : ["provision"]), ...configArgs], env);
            if (consume && !["blocked", "requeue"].includes(result.status)) {
                const receipt = { trialId: t.db.trialId, routingKey: selected.routingKey };
                await controller.writeJson(deliveryFile, { ...receipt, signature: sign(receipt, t.context.keys[t.context.keyId]) });
            }
            await fs.appendFile(path.join(t.directory, "process.jsonl"), JSON.stringify({
                pid: process.pid, role, selected, result, time: new Date().toISOString(),
            }) + "\n");
            return { ...result, purpose, physicalAcceptance: false };
        } finally {
            const db = await json(apiFile);
            db.runs[`${current.id}/1`].status = "completed"; db.runs[`${current.id}/1`].conclusion = "success";
            writeDatabase(apiFile, db);
        }
    });
}

async function verify(t) {
    const b = t.approval.bootstrap, m = t.approval.manifest;
    const transport = createGithubApi({ repository: "dotnet/fsharp", issueNumber: 123,
        checkpointCommentId: b.checkpointCommentId, resultCommentId: b.resultCommentId,
        anchorSha: t.config.deployment.anchorSha, token: "offline" }, t.service.send);
    const context = { ...t.context, issueNumber: 123, commentId: b.checkpointCommentId };
    const { checkpoint: cp } = await state.readAuthenticatedCheckpoint(transport, context);
    requireValue(cp.publicationStatus === "published" && cp.state.phase === "complete" && cp.pending === null &&
        cp.deadline === t.saved.deadline && cp.state.budget.allowanceId === t.saved.allowanceId &&
        equal(cp.state.manifest, m) && cp.state.pinnedMainSha === t.config.source.main &&
        equal(cp.state.budget.consumed, { sourceBuilds: 5, workerUnits: 6, reservedMinutes: 660 }),
    "trial incomplete or six-unit allowance identity changed");
    const result = await state.authenticateDecodedRecord(transport, { ...context, commentId: b.resultCommentId },
        await transport.getComment(b.resultCommentId));
    requireValue(result?.kind === "result" && result.payload.digest === cp.state.publication.digest &&
        cp.state.publication.commentId === b.resultCommentId && equal(result.payload.report.state,
            { ...cp.state, publication: { digest: null, commentId: null } }), "authenticated persisted publication required");
    const c = cp.state.confirmation;
    requireValue(c?.classification === "verified-introducing-source-boundary" && c.candidate === t.config.source.candidate &&
        c.parents.length === 1 && c.parents[0].sourceSha === t.config.source.good &&
        c.latentRootCause === "unproven" && equal(cp.records.map(r => r.request.phase), t.saved.plan.phases) &&
        new Set(cp.records.map(r => r.id)).size === 6 && cp.records.every(r => r.status === "committed" &&
            r.executionGeneration !== undefined), "fresh independently reserved expected source boundary required");
    const storeDirectory = path.join(t.directory, "state", "dotnet-fsharp-123", "store");
    requireValue((await fs.stat(storeDirectory)).isDirectory(), "required evidence store missing");
    const store = createFileEvidenceStore({ repository: "dotnet/fsharp", runId: t.config.runs.controller,
        attempt: 1, directory: storeDirectory, token: "offline", retentionDays: 90 }, { readOnly: true });
    for (const observation of Object.values(cp.observations)) {
        requireValue(observation.valid && observation.evidence.some(e => e.required),
            "physical collector evidence required; SYNTHETIC unit observations are not acceptance");
        for (const evidence of observation.evidence.filter(e => e.required))
            requireValue((await store.verifyEvidence(evidence.url))?.sha256 === evidence.sha256, "required evidence missing or changed");
    }
    return { status: "verified", purpose, trialId: t.db.trialId, digest: result.payload.digest,
        commentId: b.resultCommentId, budget: cp.state.budget.consumed, physicalAcceptance: false };
}

async function main(args = process.argv.slice(2), supplied = process.env) {
    if (equal(args, ["--help"])) return { help: usage };
    requireValue((args[0] === "--prepare" && args.length === 3 ||
        ["--advance", "--verify"].includes(args[0]) && args.length === 2) &&
        args.slice(1).every(p => path.isAbsolute(p) && path.resolve(p) === p), usage);
    if (args[0] === "--prepare") return prepare(args[1], args[2], supplied);
    const t = await load(args[1], supplied, args[0] === "--verify");
    return args[0] === "--advance" ? advance(t, supplied) : verify(t);
}

if (require.main === module) main().then(result => {
    console.log(result.help ?? JSON.stringify(result));
    if (["blocked", "requeue"].includes(result.status)) process.exitCode = 1;
}).catch(error => { console.log(JSON.stringify({ status: "blocked", reason: error.message })); process.exitCode = 1; });

module.exports = { main, adapter, helpers, sign };
