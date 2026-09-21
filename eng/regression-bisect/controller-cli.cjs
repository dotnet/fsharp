const fs = require("node:fs/promises");
const path = require("node:path");
const os = require("node:os");
const { createHmac, randomUUID } = require("node:crypto");
const { createGithubApi, createProvisioningApi, createApprovalReader } = require("./github.cjs");
const { requireValue, hash, createHttp, validateHttpResponse, isolatedEnvironment } = require("./toolchain-io.cjs");
const model = require("./model.cjs");
const state = require("./state.cjs");
const { execute } = require("./repro-process.cjs");
const { bootstrapController, runController, publishController } = require("./controller.cjs");
const { currentInput, historyWorkspace } = require("./engine.cjs");
const { createObservationCollector, commitCollectedObservation } = require("./collector.cjs");
const { createActionsEvidenceStore, createFileEvidenceStore } = require("./evidence-store.cjs");
const { image, validateContainerConfig, inspectRuntime, runContainer, sourceBundleIdentity } = require("./container.cjs");
const { resolveLatestStableSdk } = require("./toolchains.cjs");
const { readIssueSnapshot } = require("./intake.cjs");

const fingerprint = model.fingerprintInput;
const repository = "dotnet/fsharp";
const workflowPath = ".github/workflows/regression-bisect-controller.yml";
const containmentModes = ["inspect", "disk", "pids", "cpu", "memory", "malformed", "timeout"];
const root = path.resolve(__dirname, "..", "..");
const positive = n => Number.isSafeInteger(n) && n > 0;
const sha = s => typeof s === "string" && /^[a-f0-9]{40}$/.test(s);
const digest = s => typeof s === "string" && /^[a-f0-9]{64}$/.test(s);
const usage = "Usage: node controller-cli.cjs <bootstrap|advance|publish|provision> --issue <positive-number> --config <trusted-config-path>";
const own = { owner: "dotnet", repo: "fsharp" };
const decode = bytes => JSON.parse(new TextDecoder("utf-8", { fatal: true }).decode(bytes));

function argumentsFor(args, env = process.env) {
    if (!args.length && env.GITHUB_ACTIONS === "true" && env.RB_ACTION_INPUTS) {
        let selected;
        try { selected = JSON.parse(env.RB_ACTION_INPUTS); }
        catch (error) {
            if (!(error instanceof SyntaxError)) throw error;
            throw new Error("invalid typed Actions input");
        }
        requireValue(selected && Object.getPrototypeOf(selected) === Object.prototype &&
            ["issue mode", "issue mode routingGeneration routingKey"].includes(Object.keys(selected).sort().join(" ")) &&
            positive(selected.issue) && ["bootstrap", "advance", "publish"].includes(selected.mode), "invalid typed Actions input");
        args = [selected.mode, "--issue", String(selected.issue), "--config",
            path.join(root, ".github", "regression-bisect", "deployment.json")];
        if (Object.hasOwn(selected, "routingKey")) {
            requireValue(typeof selected.routingKey === "string" && typeof selected.routingGeneration === "string",
                "invalid typed Actions routing input");
            if (selected.routingKey !== "" || selected.routingGeneration !== "")
                args.push("--routing-key", selected.routingKey, "--routing-generation", selected.routingGeneration);
        }
    }
    requireValue([5, 9].includes(args.length) && ["bootstrap", "advance", "publish", "provision"].includes(args[0]) &&
        args[1] === "--issue" && args[3] === "--config" && /^[1-9]\d*$/.test(args[2]) &&
        positive(Number(args[2])) && args[4].length > 0 && !args[4].startsWith("--"), usage);
    let routing = {};
    if (args.length === 9) {
        requireValue(args[0] !== "provision" && args[5] === "--routing-key" && digest(args[6]) &&
            args[7] === "--routing-generation" && /^(?:-1|0|[1-9]\d*)$/.test(args[8]) &&
            Number.isSafeInteger(Number(args[8])) && (args[0] === "bootstrap") === (args[8] === "-1"), "invalid typed routing dispatch");
        routing = { routingKey: args[6], routingGeneration: Number(args[8]) };
    }
    return { mode: args[0], issueNumber: Number(args[2]), configPath: path.resolve(args[4]), ...routing };
}

async function bytes(file, limit = 48 * 1024 - 1) {
    const stat = await fs.lstat(file);
    requireValue(stat.isFile() && !stat.isSymbolicLink() && stat.nlink === 1 && stat.size <= limit &&
        await fs.realpath(file) === path.resolve(file), "trusted input must be a bounded regular file");
    const result = await fs.readFile(file);
    requireValue(result.length === stat.size, "trusted file changed while reading");
    return result;
}

function configuration(c, env) {
    requireValue(c && Object.keys(c).sort().join(" ") ===
        "actor anchorSha container containment gitExecutable keyHashes keyId platform retainedWorkflowRevisions schemaVersion workflowPath" &&
        c.schemaVersion === 1 && c.workflowPath === workflowPath && sha(c.anchorSha) &&
        Array.isArray(c.retainedWorkflowRevisions) && c.retainedWorkflowRevisions.every(sha) &&
        path.isAbsolute(c.gitExecutable) &&
        fingerprint(c.platform) === fingerprint({ os: "linux", architecture: "x64", rid: "linux-x64" }),
    "invalid reviewed deployment configuration");
    requireValue(Object.keys(c.actor).sort().join(" ") === "appId id login type" &&
        positive(c.actor.id) && positive(c.actor.appId) && c.actor.type === "Bot" &&
        typeof c.actor.login === "string" && /^[\w-]+\[bot\]$/.test(c.actor.login), "invalid reviewed writer identity");
    validateContainerConfig(c.container);
    requireValue(c.containment && Object.keys(c.containment).sort().join(" ") === "configSha256 receiptSha256 reportSha256" &&
        digest(c.containment.configSha256) && digest(c.containment.reportSha256) &&
        c.containment.receiptSha256 && Object.keys(c.containment.receiptSha256).sort().join() === [...containmentModes].sort().join() &&
        Object.values(c.containment.receiptSha256).every(digest), "reviewed containment attestation required");
    requireValue(sha(env.RB_TRUSTED_REVISION) && /^[1-9]\d{0,19}$/.test(env.GITHUB_RUN_ID) &&
        /^[1-9]\d*$/.test(env.GITHUB_RUN_ATTEMPT) && positive(Number(env.GITHUB_RUN_ATTEMPT)) &&
        env.GITHUB_REPOSITORY === repository && env.GITHUB_REF === "refs/heads/main" &&
        env.GITHUB_WORKFLOW_SHA === env.RB_TRUSTED_REVISION, "untrusted workflow/run/attempt identity");
    requireValue(c.keyHashes && Object.keys(c.keyHashes).length > 0 &&
        Object.entries(c.keyHashes).every(([id, h]) => /^[\w-]{1,64}$/.test(id) && digest(h)) &&
        Object.hasOwn(c.keyHashes, c.keyId), "invalid key registration");
    let supplied;
    try { supplied = JSON.parse(env.RB_SIGNING_KEYS ?? "{}"); }
    catch (error) {
        if (!(error instanceof SyntaxError)) throw error;
        throw new Error("invalid signing-key JSON");
    }
    requireValue(supplied && Object.getPrototypeOf(supplied) === Object.prototype, "signing keys must be a JSON object");
    requireValue(Object.keys(supplied).sort().join() === Object.keys(c.keyHashes).sort().join(), "missing or extra signing keys");
    const keys = Object.fromEntries(Object.entries(supplied).map(([id, value]) => {
        requireValue(typeof value === "string", "invalid signing key");
        const key = Buffer.from(value, "base64");
        requireValue(key.length >= 32 && key.toString("base64") === value && hash(key) === c.keyHashes[id],
            "missing or swapped signing key");
        return [id, key];
    }));
    return { keys, keyId: c.keyId, repository, issueNumber: null, actor: c.actor, workflowPath,
        workflowRef: "refs/heads/main", workflowRevision: env.RB_TRUSTED_REVISION,
        policyRevision: env.RB_TRUSTED_REVISION,
        trustedWorkflowRevisions: [...new Set([env.RB_TRUSTED_REVISION, ...c.retainedWorkflowRevisions])],
        checkpointAnchorSha: c.anchorSha, owner: { runId: env.GITHUB_RUN_ID, attempt: Number(env.GITHUB_RUN_ATTEMPT) } };
}

async function containment(config, directory, runtime) {
    requireValue(path.isAbsolute(directory ?? ""), "trusted containment evidence directory required");
    const raw = await bytes(path.join(directory, "containment.json"));
    requireValue(hash(raw) === config.containment.reportSha256, "containment attestation digest mismatch");
    const report = decode(raw);
    requireValue(report.status === "passed" && report.image === config.container.image &&
        report.configSha256 === config.containment.configSha256 &&
        report.sourceSha256 === hash(await fs.readFile(path.join(__dirname, "containment.smoke.cjs"))) &&
        fingerprint(report.cases?.map(c => c.mode)) === fingerprint(containmentModes) && report.cases.every(c => c.passed === true),
    "containment attestation is incomplete or stale");
    const configurationBytes = await bytes(path.join(directory, "container.json"));
    requireValue(hash(configurationBytes) === config.containment.configSha256 &&
        fingerprint(decode(configurationBytes)) === fingerprint(config.container), "containment configuration has changed");
    for (const mode of containmentModes) {
        const receiptBytes = await bytes(path.join(directory, mode, "receipt.json"), 1024 * 1024);
        requireValue(hash(receiptBytes) === config.containment.receiptSha256[mode], "containment receipt digest mismatch");
        const receipt = decode(receiptBytes);
        requireValue(receipt.workerStarted === true && receipt.image === config.container.image &&
            receipt.resources?.length === 4 && receipt.cleanup?.length === 4 && receipt.cleanup.every(r => r.removed) &&
            ["ID", "ServerVersion", "KernelVersion", "OperatingSystem", "CgroupVersion", "SecurityOptions"].every(k =>
                runtime[k] !== undefined && fingerprint(receipt.runtime?.[k]) === fingerprint(runtime[k])) &&
            receipt.helperDigest === fingerprint(receipt.helpers), "containment runtime or cleanup attestation mismatch");
        for (const helper of receipt.helpers) {
            model.validateRepositoryPath(helper.path);
            requireValue(hash(await bytes(path.join(root, helper.path), 1024 * 1024)) === helper.sha256,
                "containment helper bytes have changed");
        }
    }
}

async function writeJson(file, value) {
    const temporary = `${file}.${randomUUID()}.tmp`;
    const handle = await fs.open(temporary, "wx", 0o600);
    try { await handle.writeFile(JSON.stringify(value)); await handle.sync(); }
    finally { await handle.close(); }
    await fs.rename(temporary, file);
}

// The guard serializes lock handoff, including stale-owner inspection. An interrupted
// guard is intentionally not guessed away; operators must establish process death.
async function withIssueLock(directory, context, api, action) {
    await fs.mkdir(directory, { recursive: true, mode: 0o700 });
    requireValue(await fs.realpath(directory) === directory, "lock root cannot be a symbolic link");
    const lock = path.join(directory, "owner.json"), guard = path.join(directory, "handoff");
    const ticket = { host: os.hostname(), pid: process.pid, nonce: randomUUID(), owner: context.owner };
    const guarded = async fn => {
        try { await fs.mkdir(guard); }
        catch (error) { if (error.code === "EEXIST") throw new Error("issue lock handoff is busy or requires operator recovery"); throw error; }
        try { return await fn(); } finally { await fs.rmdir(guard); }
    };
    await guarded(async () => {
        let previous;
        try { previous = decode(await bytes(lock)); }
        catch (error) { if (error.code !== "ENOENT") throw error; }
        if (previous) {
            requireValue(previous.host === ticket.host && positive(previous.pid), "issue lock belongs to another host");
            let alive = true;
            try { process.kill(previous.pid, 0); }
            catch (error) { if (error.code === "ESRCH") alive = false; else throw error; }
            requireValue(!alive, "issue lock is held by a live process");
            const run = await api.getRun(previous.owner.runId, previous.owner.attempt);
            requireValue(String(run.id) === previous.owner.runId && run.run_attempt === previous.owner.attempt &&
                run.repository?.full_name === repository && run.head_repository?.full_name === repository &&
                run.path === context.workflowPath && context.trustedWorkflowRevisions.includes(run.head_sha) &&
                run.status === "completed" && typeof run.conclusion === "string", "abandoned issue owner is not safely completed");
        }
        await writeJson(lock, ticket);
    });
    try { return await action(); }
    finally {
        await guarded(async () => {
            requireValue(fingerprint(decode(await bytes(lock))) === fingerprint(ticket), "issue lock ownership changed");
            await fs.unlink(lock);
        });
    }
}

async function pages(api, route, args) {
    const records = [], ids = new Set();
    for (let page = 1; page <= 10; page++) {
        const result = await api.request(route, { ...own, ...args, per_page: 100, page });
        requireValue(Array.isArray(result.data) && result.data.length <= 100, "incomplete provisioning history");
        for (const record of result.data) {
            requireValue(positive(record.id) && !ids.has(record.id), "duplicate or invalid provisioning history");
            ids.add(record.id); records.push(record);
        }
        if (result.headers?.link === undefined ? result.data.length < 100 : !/;\s*rel="?next"?/.test(result.headers.link))
            return records;
    }
    throw new Error("incomplete provisioning history: page limit");
}

async function provision({ context, config, reader, apiFor, directory, now }) {
    const request = decode(await reader.readTrustedFile(`.github/regression-bisect/provision/${context.issueNumber}.json`, 4096));
    requireValue(Object.keys(request).sort().join(" ") === "allowanceId deadline schemaVersion" &&
        request.schemaVersion === 1 && /^[\w-]{1,128}$/.test(request.allowanceId) &&
        Number.isFinite(Date.parse(request.deadline)) && Date.parse(request.deadline) > Date.parse(now), "invalid reviewed provisioning authorization");
    const api = apiFor(1, 2, true);
    const statuses = await pages(api, "GET /repos/{owner}/{repo}/commits/{ref}/statuses", { ref: config.anchorSha });
    const comments = await pages(api, "GET /repos/{owner}/{repo}/issues/{issue_number}/comments", { issue_number: context.issueNumber });
    const file = path.join(directory, "provision.json");
    let journal;
    try { journal = decode(await bytes(file)); }
    catch (error) { if (error.code !== "ENOENT") throw error; }
    const sign = value => createHmac("sha256", context.keys[context.keyId]).update("regression-provision-v1\0")
        .update(fingerprint(value)).digest("hex");
    const relevant = statuses.filter(s => s.context === `regression-provision/${context.issueNumber}` ||
        s.context?.startsWith(`regression-bisect/${context.issueNumber}/`));
    if (!journal) {
        requireValue(relevant.length === 0 && !comments.some(c => c.body?.includes(`regression-provision:${context.issueNumber}:`)),
            "prior allowance exists or provisioning registration is missing");
        for (const comment of comments)
            requireValue(!(await state.authenticateDecodedRecord(api, { ...context, commentId: comment.id }, comment)),
                "prior signed allowance exists; missing registration cannot be replaced");
        journal = { request, anchorSha: config.anchorSha, issueNumber: context.issueNumber, owner: context.owner,
            workflowRevision: context.workflowRevision, keyId: context.keyId, slots: {}, next: null };
        await writeJson(file, { ...journal, signature: sign(journal) });
    } else {
        const { signature, ...body } = journal;
        requireValue(signature === sign(body) && fingerprint(body.request) === fingerprint(request) &&
            body.anchorSha === config.anchorSha && body.issueNumber === context.issueNumber &&
            context.trustedWorkflowRevisions.includes(body.workflowRevision), "provisioning registration authentication failed");
        journal = body;
    }
    const provisionDigest = fingerprint({ request, issueNumber: context.issueNumber, anchorSha: config.anchorSha });
    const receipt = { description: `provision:${provisionDigest}`,
        target_url: `https://github.com/${repository}/issues/${context.issueNumber}#provision-${context.keyId}:${sign(provisionDigest)}` };
    const prior = relevant.filter(s => s.context === `regression-provision/${context.issueNumber}`);
    requireValue(relevant.every(s => s.context === `regression-provision/${context.issueNumber}` ||
        s.context === `regression-bisect/${context.issueNumber}/${journal.slots.checkpoint?.id}`),
    "different prior allowance exists");
    requireValue(prior.every(s => s.creator?.id === config.actor.id && s.creator.login === config.actor.login &&
        s.creator.type === config.actor.type && s.state === "success" &&
        s.description === receipt.description && s.target_url === receipt.target_url), "forged prior provisioning history");
    if (!prior.length) {
        requireValue(relevant.length === 0 && Object.keys(journal.slots).length === 0 && journal.next === null,
            "provisioning freshness receipt missing");
        const written = await api.markProvisioning(receipt.description, receipt.target_url);
        requireValue(positive(written.id) && written.creator?.id === config.actor.id &&
            written.creator.login === config.actor.login && written.creator.type === config.actor.type &&
            written.context === `regression-provision/${context.issueNumber}` && written.state === "success" &&
            written.description === receipt.description && written.target_url === receipt.target_url,
        "provisioning receipt acknowledgement mismatch");
    }
    const save = () => writeJson(file, { ...journal, signature: sign(journal) });
    for (const kind of ["checkpoint", "result"]) {
        const marker = `regression-provision:${context.issueNumber}:${kind}:${provisionDigest}:${sign({ kind, provisionDigest })}`;
        if (!journal.slots[kind]) {
            const matches = comments.filter(c => c.body === marker);
            requireValue(matches.length <= 1, "ambiguous provisioned slots");
            requireValue(journal.next !== kind || matches.length === 1, "created slot missing; replacement allowance forbidden");
            if (!matches.length) {
                journal.next = kind; await save();
                matches.push(await api.createSlot(marker));
            }
            const slot = matches[0];
            requireValue(positive(slot.id) && slot.user?.id === config.actor.id &&
                slot.user.login === config.actor.login && slot.user.type === config.actor.type &&
                slot.performed_via_github_app?.id === config.actor.appId &&
                slot.issue_url === `https://api.github.com/repos/${repository}/issues/${context.issueNumber}`,
            "provisioned slot writer/destination mismatch");
            journal.slots[kind] = { id: slot.id, marker };
            journal.next = null; await save();
        }
    }
    const transport = apiFor(journal.slots.checkpoint.id, journal.slots.result.id, true);
    for (const kind of ["checkpoint", "result"]) {
        const slot = journal.slots[kind];
        const signing = { ...context, owner: journal.owner, workflowRevision: journal.workflowRevision, commentId: slot.id };
        const payload = { schemaVersion: 1, purpose: `${kind}-slot`, repository, issueNumber: context.issueNumber,
            ...(kind === "checkpoint" ? { checkpointAnchorSha: config.anchorSha, ...request } :
                { checkpointCommentId: journal.slots.checkpoint.id }) };
        const body = state.serializeWorkflowRecord({ kind: "proposal", status: `Reserved ${kind} slot`, payload }, signing);
        const existing = await transport.getComment(slot.id);
        if (!slot.digest) {
            requireValue(existing.body === slot.marker || existing.body === body, "provisioning slot was replaced");
            if (existing.body !== body) await transport.updateComment(slot.id, body);
            requireValue((await state.authenticateDecodedRecord(transport, signing, await transport.getComment(slot.id)))?.kind === "proposal",
                "provisioning signature acknowledgement failed");
            slot.digest = fingerprint(body); await save();
        } else requireValue(await state.authenticateDecodedRecord(transport, signing, existing),
            "registered allowance slot missing or unauthenticated");
    }
    const registration = { allowanceId: request.allowanceId, deadline: request.deadline,
        checkpointAnchorSha: config.anchorSha, checkpointCommentId: journal.slots.checkpoint.id,
        checkpointBootstrapDigest: journal.slots.checkpoint.digest, resultCommentId: journal.slots.result.id,
        resultBootstrapDigest: journal.slots.result.digest };
    await writeJson(path.join(directory, "registration.json"), registration);
    return { status: "provisioned", registration, approvalRequired: true };
}

async function main(args = process.argv.slice(2), env = process.env) {
    if (args.length === 1 && args[0] === "--help") return { help: usage };
    const input = argumentsFor(args, env);
    requireValue(!env.RB_LOCAL_TEST_FILE || !env.GITHUB_ACTIONS && env.RB_LOCAL_TEST === "explicit-offline-fixture",
        "local test injection forbidden in Actions or without explicit local opt-in");
    const local = env.RB_LOCAL_TEST_FILE
        ? await require("./fixtures/controller-service.cjs").connect(path.resolve(env.RB_LOCAL_TEST_FILE)) : null;
    const configBytes = await bytes(input.configPath);
    const config = decode(configBytes);
    const context = { ...configuration(config, env), issueNumber: input.issueNumber };
    const token = input.mode === "provision" ? env.RB_PROVISION_TOKEN : env.RB_GITHUB_TOKEN;
    requireValue(typeof token === "string" && token.length > 0, "missing explicit controller/provisioning credential");
    const apiFor = (checkpointCommentId, resultCommentId, provisioning = false) =>
        (provisioning ? createProvisioningApi : createGithubApi)({
            repository, issueNumber: input.issueNumber, checkpointCommentId, resultCommentId,
            anchorSha: config.anchorSha, token,
        }, local?.send);
    const api = apiFor(1, 2);
    const reader = createApprovalReader({ api, revision: context.policyRevision, issueNumber: input.issueNumber });
    requireValue(hash(await reader.readTrustedFile(".github/regression-bisect/deployment.json", 48 * 1024 - 1)) === hash(configBytes),
        "configuration differs from pinned trusted bytes");
    const run = await api.getRun(context.owner.runId, context.owner.attempt);
    requireValue(String(run.id) === context.owner.runId && run.run_attempt === context.owner.attempt &&
        run.path === workflowPath && run.head_sha === context.workflowRevision && run.head_branch === "main" &&
        run.repository?.full_name === repository && run.head_repository?.full_name === repository &&
        (run.status === "in_progress" || input.mode === "provision" && run.status === "completed"),
    "current workflow run authentication failed");
    requireValue(path.isAbsolute(env.RB_STATE_DIRECTORY ?? ""), "absolute trusted RB_STATE_DIRECTORY required");
    const directory = path.join(env.RB_STATE_DIRECTORY, `dotnet-fsharp-${input.issueNumber}`);
    return withIssueLock(directory, context, api, async () => {
        const clock = local?.clock ?? (() => new Date());
        const git = async (args, cwd = root) => {
            local?.recordGit(args);
            const started = Date.now();
            const result = await execute(config.gitExecutable, ["-c", "credential.helper=", "-c", "core.hooksPath=",
                "-c", "core.fsmonitor=false", ...args], { cwd,
                env: { ...isolatedEnvironment(directory, { os: process.platform === "win32" ? "windows" : "linux",
                    architecture: process.arch }), GIT_CONFIG_NOSYSTEM: "1", GIT_CONFIG_GLOBAL: path.join(directory, "empty.gitconfig"),
                    GIT_NO_REPLACE_OBJECTS: "1", GIT_TERMINAL_PROMPT: "0", GIT_ALLOW_PROTOCOL: local ? "file" : "https" },
                timeoutMs: 120000, maxBytes: 4 * 1024 * 1024 });
            local?.recordGitResult(args, result, Date.now() - started);
            requireValue(result.complete && result.exitCode === 0, `trusted Git ${args[0]} failed: ${result.stderr}`);
            return result.stdout.trim();
        };
        if (!local) {
            requireValue(await git(["rev-parse", "HEAD"]) === context.workflowRevision, "helpers are not at the trusted revision");
            requireValue(await git(["status", "--porcelain", "--untracked-files=all"]) === "", "trusted checkout is modified");
        }
        const head = (await api.request("GET /repos/{owner}/{repo}/commits/{ref}", { ...own,
            ref: context.policyRevision })).data;
        requireValue(head.sha === context.policyRevision, "trusted commit is unavailable");
        const history = { gitExecutable: config.gitExecutable, scratchDirectory: directory,
            mirrorDirectory: local?.mirrorDirectory ?? path.join(directory, "history.git"), mainRef: "refs/heads/main" };
        if (!local) {
            await fs.mkdir(history.mirrorDirectory, { recursive: true });
            await git(["init", "--bare", "--quiet", "--template="], history.mirrorDirectory);
            await git(["fetch", "--quiet", "--no-tags", "--no-recurse-submodules", "https://github.com/dotnet/fsharp.git",
                "+refs/heads/main:refs/heads/main"], history.mirrorDirectory);
            requireValue(await git(["merge-base", context.workflowRevision, "refs/heads/main"], history.mirrorDirectory) ===
                context.workflowRevision, "trusted revision is not reachable from default branch");
            requireValue(await git(["merge-base", config.anchorSha, "refs/heads/main"], history.mirrorDirectory) === config.anchorSha,
                "freshness anchor is not reachable from default branch");
        } else await local.checkAnchor(config.anchorSha);
        if (input.mode === "provision") return provision({ context, config, reader, apiFor, directory, now: clock().toISOString() });
        if (!local) requireValue(process.platform === "linux" && process.arch === "x64", "native Linux x64 controller required");
        const runtime = await (local?.inspectRuntime ?? inspectRuntime)(config.container);
        if (!local) await containment(config, env.RB_CONTAINMENT_DIRECTORY, runtime);
        const policyBytes = await reader.readTrustedFile(".github/policies/regression-bisect.json", 48 * 1024 - 1);
        context.policy = decode(policyBytes);
        model.validatePolicy(context.policy);
        requireValue(hash(policyBytes) === hash(await fs.readFile(path.join(root, ".github", "policies", "regression-bisect.json"))),
            "stale policy checkout");
        const workflow = await reader.readTrustedFile(workflowPath, 48 * 1024 - 1);
        if (!local) requireValue(hash(workflow) === hash(await fs.readFile(path.join(root, workflowPath))), "stale workflow checkout");
        const approval = decode(await reader.readApproval({ path: `.github/regression-bisect/approved/${input.issueNumber}.json`,
            revision: context.policyRevision }));
        const registration = decode(await reader.readTrustedFile(`.github/regression-bisect/registrations/${input.issueNumber}.json`, 4096));
        requireValue(approval.schemaVersion === 1 && Object.keys(approval).sort().join(" ") === "bootstrap manifest schemaVersion" &&
            Object.keys(registration).sort().join(" ") ===
                "allowanceId checkpointAnchorSha checkpointBootstrapDigest checkpointCommentId deadline resultBootstrapDigest resultCommentId" &&
            /^[\w-]{1,128}$/.test(registration.allowanceId) && typeof registration.deadline === "string" &&
            /^\d{4}-\d\d-\d\dT.*Z$/.test(registration.deadline) && Number.isFinite(Date.parse(registration.deadline)) &&
            [registration.checkpointBootstrapDigest, registration.resultBootstrapDigest].every(digest) &&
            fingerprint(registration) === fingerprint(approval.bootstrap) && registration.checkpointAnchorSha === config.anchorSha,
        "reviewed registration/approval mismatch");
        context.commentId = registration.checkpointCommentId;
        context.bootstrapDigest = registration.checkpointBootstrapDigest;
        context.manifest = approval.manifest;
        model.validateManifest(context.manifest);
        context.authorization = { repository, issueNumber: input.issueNumber,
            approvalPath: `.github/regression-bisect/approved/${input.issueNumber}.json`, approvalRevision: context.policyRevision,
            manifestDigest: fingerprint(context.manifest), inputDigest: context.manifest.approvedInputDigest };
        const environments = Object.fromEntries(context.manifest.matrix.map(c =>
            [c.id, { os: c.os, architecture: c.architecture, imageDigest: image.split(":").at(-1) }]));
        requireValue(context.manifest.matrix.every(c => c.os === "linux" && c.architecture === "x64"),
            "unsupported worker matrix; no platform fallback");
        context.environmentFingerprint = fingerprint(environments);
        const transport = apiFor(registration.checkpointCommentId, registration.resultCommentId);
        let routedGeneration;
        if (input.routingKey) {
            const { dispatchKey } = require("./routing.cjs");
            requireValue(input.routingKey === dispatchKey({ issueNumber: input.issueNumber, allowanceId: registration.allowanceId,
                generation: input.routingGeneration, mode: input.mode, discussionFingerprint: context.manifest.discussionFingerprint,
                policyRevision: context.policyRevision }), "routing dispatch identity mismatch");
            const existing = await transport.getComment(context.commentId);
            const record = await state.authenticateDecodedRecord(transport, context, existing);
            requireValue(record, "routing checkpoint authentication failed");
            if (record.kind === "checkpoint") {
                requireValue(input.routingGeneration <= record.payload.generation, "routing generation is ahead of the checkpoint");
                requireValue(record.payload.state.budget.allowanceId === registration.allowanceId &&
                    record.payload.deadline === registration.deadline, "existing allowance differs from reviewed registration");
                routedGeneration = record.payload.generation;
            } else requireValue(record.kind === "proposal" && input.routingGeneration === -1 &&
                fingerprint(existing.body) === registration.checkpointBootstrapDigest,
                "routing requires the registered bootstrap");
        }
        const discoveryBytes = await reader.readOptionalTrustedFile(".github/regression-bisect/discovery.json", 4096);
        const discovery = discoveryBytes === null ? null : require("./routing.cjs").validateRegistration(decode(discoveryBytes));
        if (discovery?.issueNumber === input.issueNumber) {
            requireValue(![registration.checkpointCommentId, registration.resultCommentId].includes(discovery.commentId) &&
                discovery.checkpointAnchorSha === config.anchorSha, "discovery slot conflicts with issue registration");
            await require("./routing.cjs").loadCursor({ transport: apiFor(discovery.commentId, registration.resultCommentId), registration: discovery,
                context: { ...context, commentId: discovery.commentId, workflowPath: discovery.workflowPath } });
        }
        const storeConfig = { repository, runId: context.owner.runId, attempt: context.owner.attempt,
            directory: path.join(directory, "store"), token, runtimeToken: env.ACTIONS_RUNTIME_TOKEN,
            resultsUrl: env.ACTIONS_RESULTS_URL, retentionDays: 90 };
        const store = local ? createFileEvidenceStore(storeConfig) : createActionsEvidenceStore(storeConfig);
        Object.assign(transport, { readEvidence: store.readEvidence, verifyEvidence: store.verifyEvidence });
        if (!local && input.mode === "advance")
            requireValue(typeof env.ACTIONS_RUNTIME_TOKEN === "string" && env.ACTIONS_RUNTIME_TOKEN.length > 0 &&
                ["https://results-receiver.actions.githubusercontent.com", "https://results-receiver.actions.githubusercontent.com/"]
                    .includes(env.ACTIONS_RESULTS_URL), "trusted Actions artifact runtime unavailable");
        const evidenceDirectory = path.join(directory, "launches");
        await fs.mkdir(evidenceDirectory, { recursive: true });
        const collectObservation = createObservationCollector({ evidenceDirectory, store });
        const checkpoints = new Map();
        const approvalReader = { ...reader,
            authenticateRecord: async comment => {
                if (discovery?.issueNumber === input.issueNumber && comment.id === discovery.commentId) {
                    requireValue(await state.authenticateWorkflowRecord(transport, {
                        ...context, commentId: discovery.commentId, workflowPath: discovery.workflowPath,
                    }, comment), "registered discovery record authentication failed");
                    return true;
                }
                const record = await state.authenticateDecodedRecord(transport, { ...context, commentId: comment.id }, comment);
                if (!record) return false;
                if (record.kind === "checkpoint") {
                    requireValue(comment.id === context.commentId, "another authenticated allowance checkpoint exists");
                    checkpoints.set(comment.id, { status: "loaded", state: record.payload.state,
                        commentId: comment.id, owner: record.owner, pending: record.payload.pending });
                }
                return true;
            },
            readTrustedCheckpoints: async () => [...checkpoints.values()],
        };
        const refresh = async () => {
            checkpoints.clear();
            requireValue(hash(await reader.readTrustedFile(".github/regression-bisect/deployment.json", 48 * 1024 - 1)) === hash(configBytes) &&
                hash(await reader.readTrustedFile(".github/policies/regression-bisect.json", 48 * 1024 - 1)) === hash(policyBytes) &&
                hash(await reader.readTrustedFile(workflowPath, 48 * 1024 - 1)) === hash(workflow),
            "current pinned deployment/policy/workflow bytes changed");
            const currentRuntime = await (local?.inspectRuntime ?? inspectRuntime)(config.container);
            if (!local) await containment(config, env.RB_CONTAINMENT_DIRECTORY, currentRuntime);
            const snapshot = await readIssueSnapshot({ ...approvalReader, transport, repository,
                issueNumber: input.issueNumber, now: clock().toISOString(), reviewedManifest: context.manifest });
            requireValue(snapshot.status === "complete", `current intake ${snapshot.status}: ${snapshot.reason}`);
            requireValue(snapshot.snapshot.investigations.active.every(a => a.commentId === context.commentId &&
                a.owner?.runId === context.owner.runId && a.owner?.attempt === context.owner.attempt), "active linked investigation");
            context.issue = snapshot.snapshot.input.discussion.issue;
            context.issueFingerprint = snapshot.snapshot.discussionFingerprint;
            const resolved = await resolveLatestStableSdk({ policy: context.policy, platform: config.platform,
                signal: AbortSignal.timeout(300000) }, local?.toolchainDependencies);
            requireValue(resolved.status === "resolved", `stable metadata unavailable: ${resolved.error?.message}`);
            const toolchain = resolved.toolchain;
            const archive = path.join(directory, `stable-${randomUUID()}.archive`);
            try {
                const downloaded = await (local?.download ?? createHttp(context.policy))(toolchain.archive.url, { destination: archive });
                validateHttpResponse(toolchain.archive.url, downloaded, context.policy);
                requireValue(downloaded.status === 200 && downloaded.sha512 === toolchain.archive.sha512 &&
                    downloaded.size > 0 && downloaded.size <= 1073741824, "stable archive acquisition identity mismatch");
                context.stableSdk = { version: toolchain.sdkVersion, origin: "official-release-index", latest: true, verified: true,
                    releaseIndexUrl: toolchain.metadata[0].url, releaseIndexDigest: toolchain.metadata[0].sha256,
                    archives: [{ os: "linux", architecture: "x64", sha256: downloaded.sha256 }] };
            } finally { await fs.rm(archive, { force: true }); }
            if (!local) await git(["fetch", "--quiet", "--no-tags", "--no-recurse-submodules", "https://github.com/dotnet/fsharp.git",
                "+refs/heads/main:refs/heads/main"], history.mirrorDirectory);
            context.pinnedMainSha = await git(["rev-parse", "refs/heads/main"], history.mirrorDirectory);
            requireValue(sha(context.pinnedMainSha), "invalid pinned main");
            context.now = clock().toISOString();
            return { ...context, readSource: reader.readSource };
        };
        try {
            await refresh();
            const bootstrapJournal = path.join(directory, "bootstrap.json");
            if (input.mode === "bootstrap") {
                const existing = await transport.getComment(context.commentId);
                let body;
                try { body = decode(await bytes(bootstrapJournal, 64 * 1024)); }
                catch (error) { if (error.code !== "ENOENT") throw error; }
                if (body === undefined) {
                    requireValue(fingerprint(existing.body) === registration.checkpointBootstrapDigest &&
                        await state.readFreshness(transport, context) === null,
                    "required bootstrap journal missing; cannot replace existing history");
                    body = existing.body;
                    await writeJson(bootstrapJournal, body);
                }
                const journal = await state.authenticateDecodedRecord(transport, context, { ...existing, body });
                requireValue(journal && (journal.kind === "proposal" && fingerprint(body) === registration.checkpointBootstrapDigest ||
                    journal.kind === "checkpoint" && journal.payload.state.budget.allowanceId === registration.allowanceId &&
                    journal.payload.deadline === registration.deadline), "bootstrap journal authentication failed");
                if (journal.kind === "checkpoint")
                    requireValue(await state.readFreshness(transport, context) !== null,
                        "committed bootstrap freshness missing; operator recovery required");
            }
            const interruptedBootstrap = input.mode === "bootstrap" && routedGeneration === 0 &&
                await state.readFreshness(transport, context) === null;
            if (routedGeneration !== undefined && input.routingGeneration < routedGeneration && !interruptedBootstrap) {
                // A duplicate still requires current intake and authenticated evidence.
                const current = await state.loadCheckpoint(transport, context, { requiredEvidenceOnly: input.mode === "publish" });
                requireValue(current.blockers.length === 0, `routing duplicate blocked: ${current.blockers.join("; ")}`);
                if (input.mode === "bootstrap") await writeJson(bootstrapJournal, (await transport.getComment(context.commentId)).body);
                return { status: "duplicate", generation: current.checkpoint.generation, routingKey: input.routingKey };
            }
            const deps = { context, transport, approval: approvalReader, environments, platform: config.platform, history,
                clock, withLock: action => action(), collectObservation, resultCommentId: registration.resultCommentId,
                discoveryCommentId: discovery?.issueNumber === input.issueNumber ? discovery.commentId : undefined,
                toolchainDependencies: local?.toolchainDependencies, readCurrentContext: refresh,
                execute: async (...args) => {
                    local?.recordGit(args[1]);
                    const started = Date.now(), result = await execute(...args);
                    local?.recordGitResult(args[1], result, Date.now() - started);
                    return result;
                },
                runIsolated: async (unit, { signal }) => {
                    const launchDirectory = path.join(evidenceDirectory, unit.buildInstanceId);
                    await fs.mkdir(launchDirectory);
                    const sources = path.join(launchDirectory, "samples");
                    await fs.mkdir(sources);
                    for (const source of unit.manifest.sources) {
                        const target = path.join(sources, source.path);
                        await fs.mkdir(path.dirname(target), { recursive: true });
                        await fs.writeFile(target, await reader.readSource(source), { flag: "wx" });
                    }
                    let sourceBundle, sourceBundleSha256;
                    if (unit.request.kind === "source") {
                        const selected = await historyWorkspace(history, deps.execute, signal);
                        try {
                            await selected.git(["update-ref", "refs/heads/selected", unit.request.target.sourceSha]);
                            sourceBundle = path.join(launchDirectory, "source.bundle");
                            await selected.git(["bundle", "create", sourceBundle, "refs/heads/selected"]);
                            sourceBundleSha256 = (await sourceBundleIdentity(sourceBundle, signal)).sha256;
                        } finally { await selected.dispose(); }
                    }
                    const output = await (local?.runContainer ?? runContainer)(config.container, {
                        operation: "execution", input: unit, signal, evidenceDirectory: launchDirectory,
                        sourceDirectory: sources, sourceBundle, sourceBundleSha256, receiptKey: context.keys[context.keyId],
                    });
                    requireValue(output.status === "collected", `contained worker blocked: ${output.reason ?? output.error ?? output.status}`);
                    return output.result;
                },
            };
            const entry = { repository, issueNumber: input.issueNumber };
            if (input.mode === "bootstrap") {
                const result = await bootstrapController(entry, deps);
                if (["initialized", "loaded"].includes(result.status))
                    await writeJson(bootstrapJournal, (await transport.getComment(context.commentId)).body);
                return result;
            }
            let loaded = await state.loadCheckpoint(transport, context, { requiredEvidenceOnly: input.mode === "publish" });
            requireValue(loaded.checkpoint.state.budget.allowanceId === registration.allowanceId &&
                loaded.checkpoint.deadline === registration.deadline, "existing allowance differs from reviewed registration");
            if (input.mode === "publish") return await publishController(entry, deps);
            // Stable endpoint stops can return before the engine selects source history.
            if (loaded.checkpoint.state.pinnedMainSha !== context.pinnedMainSha || loaded.checkpoint.publicationStatus === "requeue") {
                await state.invalidateStaleEvidence(transport, context, { generation: loaded.checkpoint.generation });
                loaded = await state.loadCheckpoint(transport, context);
            }
            const pending = loaded.checkpoint.pending;
            if (pending?.executionGeneration !== undefined && loaded.executionAllowed) {
                const previous = await transport.getRun(pending.owner.runId, pending.owner.attempt);
                requireValue(fingerprint(pending.owner) === fingerprint(context.owner) || previous.status === "completed",
                    "claimed worker owner remains active");
                const current = await currentInput(context.now, deps);
                requireValue(current.context, current.reason);
                const cp = await commitCollectedObservation({ reservation: pending, manifest: context.manifest, context },
                    { collectObservation, transport, clock });
                return { status: "observed", workId: pending.id, generation: cp.generation,
                    outcome: cp.observations[cp.records.find(r => r.id === pending.id).observationDigest].data.outcome,
                    budget: cp.state.budget.consumed, recovered: true };
            }
            const result = await runController(entry, deps);
            return ["confirmed", "unconfirmed"].includes(result.status)
                ? { ...result, status: "confirmation", confirmationStatus: result.status } : result;
        } finally {
            // runController may return a bounded cleanup-pending response. Do not
            // hand off this process's lock until its uncancellable quota I/O settles.
            await store.settleQuota();
        }
    });
}

if (require.main === module) main().then(result => {
    console.log(result.help ?? JSON.stringify(result));
    if (result.status === "blocked" || result.status === "requeue") process.exitCode = 1;
}).catch(error => {
    console.log(JSON.stringify({ status: "blocked", reason: error.message }));
    process.exitCode = 1;
});

module.exports = { main, argumentsFor, configuration, withIssueLock, containment, bytes, writeJson };
