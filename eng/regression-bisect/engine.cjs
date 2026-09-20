const fs = require("node:fs/promises");
const path = require("node:path");
const {
    isEligibleIssue, validateManifest, validateObservation, classifyAttempts, admitBisection,
    fingerprintInput: fingerprint, fingerprintEpoch, validateState,
} = require("./model.cjs");
const intake = require("./intake.cjs");
const { loadCheckpoint, reserveWork, beginWork, commitObservation, invalidateStaleEvidence, recordSearchProgress } = require("./state.cjs");
const toolchains = require("./toolchains.cjs");
const { runReproduction } = require("./repro.cjs");
const { testSourceRevision } = require("./workspace.cjs");
const { execute } = require("./repro-process.cjs");
const { isolatedEnvironment, requireValue } = require("./toolchain-io.cjs");

const equal = (a, b) => fingerprint(a) === fingerprint(b);
const fullSha = s => /^[a-f0-9]{40}$/.test(s);
const maxHistory = 4096;
const report = (status, reason, extra = {}) => ({ status, reason, requiresFreshConfirmation: true, ...extra });

async function historyWorkspace(options, run, signal) {
    requireValue(path.isAbsolute(options.gitExecutable) && options.mainRef === "refs/heads/main",
        "trusted absolute Git executable and main ref required");
    const parent = await fs.realpath(options.scratchDirectory);
    const mirror = await fs.realpath(options.mirrorDirectory);
    const root = await fs.mkdtemp(path.join(parent, "selection-"));
    const platform = { os: { win32: "windows", linux: "linux", darwin: "macos" }[process.platform], architecture: process.arch };
    const env = { ...isolatedEnvironment(root, platform),
        GIT_CONFIG_NOSYSTEM: "1", GIT_CONFIG_GLOBAL: path.join(root, "gitconfig"), GIT_TERMINAL_PROMPT: "0",
        GCM_INTERACTIVE: "Never", GIT_ALLOW_PROTOCOL: "file", GIT_NO_REPLACE_OBJECTS: "1" };
    env.PATH = `${path.dirname(options.gitExecutable)}${path.delimiter}${env.PATH}`;
    const hooks = path.join(root, "hooks");
    await fs.mkdir(hooks);
    await fs.mkdir(env.HOME); await fs.mkdir(env.TEMP);
    await fs.writeFile(env.GIT_CONFIG_GLOBAL, "", { flag: "wx" });
    const commands = [];
    const git = async (args, allowed = [0], { raw = false } = {}) => {
        const result = await run(options.gitExecutable, [
            "-c", `core.hooksPath=${hooks}`, "-c", "credential.helper=", "-c", "core.fsmonitor=false",
            "-c", "protocol.allow=never", "-c", "protocol.file.allow=always", ...args,
        ], { cwd: root, env, signal, timeoutMs: 60000, maxBytes: 4 * 1024 * 1024 });
        commands.push({ args, exitCode: result.exitCode });
        requireValue(result.complete && result.status === "completed" && allowed.includes(result.exitCode),
            `history ${args[0]} failed (${result.exitCode}): ${result.stderr}`);
        return raw ? result.stdout : result.stdout.trim();
    };
    const dispose = async () => {
        requireValue(path.dirname(root) === parent && path.basename(root).startsWith("selection-") &&
            await fs.realpath(root) === root && !(await fs.lstat(root)).isSymbolicLink(), "unowned history workspace");
        await fs.rm(root, { recursive: true });
    };
    try {
        await git(["init", "--quiet", "--template="]);
        await git(["fetch", "--quiet", "--no-tags", "--no-recurse-submodules", mirror, options.mainRef]);
        requireValue(await git(["rev-parse", "--is-shallow-repository"]) === "false", "shallow history is untestable");
        const main = await git(["rev-parse", "FETCH_HEAD"]);
        requireValue(fullSha(main), "main did not resolve to a full SHA");
        await git(["update-ref", "--no-deref", "HEAD", main]);
        const ancestor = async (a, b) => (await git(["merge-base", a, b], [0, 1])) === a;
        return { git, main, ancestor, commands, dispose };
    } catch (error) { await dispose(); throw error; }
}

function authenticated(cp, id, environments) {
    const record = cp.observations[id];
    requireValue(record?.valid && cp.state.budget.authenticatedObservationDigests.includes(id), "missing authenticated observation");
    const o = record.data;
    validateObservation(o, cp.state.manifest);
    requireValue(fingerprint(o) === id &&
        o.provenance.workflowRevision === cp.state.budget.workflowRevision &&
        o.provenance.policyRevision === cp.state.budget.policyRevision, "observation provenance mismatch");
    for (const cell of o.cells) {
        requireValue(equal(cell.outcome, classifyAttempts(cell.attempts, cp.state.manifest)), "inconsistent assertion evidence");
        requireValue(equal(cell.environment, environments[cell.id]), "stale worker environment");
        requireValue(o.kind !== "source" || o.outcome.outcome === "untestable" || cell.buildToolchain,
            "source build toolchain evidence missing");
    }
    return o;
}

function capacity(cp, units, now, sourceShas = []) {
    const { consumed, policy } = cp.state.budget;
    return consumed.sourceBuilds + units <= policy.limits.sourceBuilds &&
        consumed.workerUnits + units <= policy.limits.workerUnits &&
        consumed.reservedMinutes + units * policy.limits.workerTimeoutMinutes <= policy.limits.reservedMinutes &&
        Date.parse(now) + units * policy.limits.workerTimeoutMinutes * 60000 <= Date.parse(cp.deadline) &&
        sourceShas.every(sha => cp.records.filter(r => r.inputDigest === cp.inputDigest &&
            r.request.kind === "source" && r.request.target.sourceSha === sha).length +
            sourceShas.filter(s => s === sha).length <= policy.limits.attemptsPerItem);
}

async function selectSource(cp, context, deps, history) {
    const s = cp.state;
    const get = id => authenticated(cp, id, deps.environments);
    const stable = get(s.endpointEvidence.stable), main = get(s.endpointEvidence.main);
    const revisions = s.testedRevisions.map(r => ({ ...r, data: get(r.observationDigest) }));
    const all = (await history.git(["rev-list", "--topo-order", `--max-count=${maxHistory + 1}`, history.main])).split("\n");
    const older = all.slice(1, maxHistory + 1);
    for (const r of revisions) if ((all.length <= maxHistory && !all.includes(r.sourceSha)) || !(await history.ancestor(r.sourceSha, history.main)))
        return report("blocked", "observed source is outside main ancestry", { historyEvidence: revisions.map(r => r.sourceSha) });
    for (const bad of revisions.filter(r => r.data.outcome.outcome === "product-failure"))
        for (const good of revisions.filter(r => r.data.outcome.outcome === "pass"))
            if (await history.ancestor(bad.sourceSha, good.sourceSha))
                return report("blocked", "nonmonotonic observed descendant pass after failure",
                    { historyEvidence: { failure: bad.sourceSha, descendantPass: good.sourceSha }, interval: s.interval });
    const passing = revisions.find(r => r.sourceSha !== history.main && r.data.outcome.outcome === "pass");
    if (!passing) {
        const candidates = [...new Set([...s.manifest.searchHints.sourceShas, ...older.toReversed()])]
            .filter(sha => older.includes(sha) && !revisions.some(r => r.sourceSha === sha))
            .slice(0, s.budget.policy.limits.sourceBuilds);
        if (!candidates.length) return report("stopped", "no verified starting interval", { skips: s.skips });
        if (!capacity(cp, 3, context.now, [candidates[0]])) return report("blocked", "no verified starting interval; confirmation budget exhausted",
            { budget: s.budget.consumed, skips: s.skips });
        return { request: { kind: "source", target: { sourceSha: candidates[0] }, endpoint: "good", phase: "good-search" } };
    }
    const ancestry = { goodSha: passing.sourceSha, badSha: history.main, pinnedMainSha: history.main, isAncestor: true };
    const good = passing.data;
    const intervalShas = (await history.git(["rev-list", "--topo-order", `--max-count=${maxHistory + 1}`,
        history.main, `^${passing.sourceSha}`])).split("\n");
    requireValue(intervalShas.length <= maxHistory, "history bound exceeded; interval uncertainty");
    const initialInterval = { goodSha: passing.sourceSha, badSha: history.main, gitLog: [], decisions: [], remaining: intervalShas };
    const parents = await history.git(["rev-list", "--parents", history.main, `^${passing.sourceSha}`]);
    if (parents.split("\n").some(line => line.split(" ").length > 2))
        return report("blocked", "merge/side-branch interval requires explicit parent analysis", { interval: initialInterval });
    const messages = await history.git(["log", "--format=%B%x00", history.main, `^${passing.sourceSha}`]);
    if (/\b(?:revert(?:ed|ing)?|reintroduc\w*|cherry[ -]pick\w*)\b/i.test(messages))
        return report("blocked", "explicit revert/reintroduction/backport history; monotonicity uncertain", { interval: initialInterval });
    const decisions = [...new Map(revisions.filter(r => r.sourceSha !== history.main && intervalShas.includes(r.sourceSha))
        .map(r => [r.sourceSha, r])).values()]
        .map(r => {
            const o = r.data;
            for (const c of o.cells) {
                const m = main.cells.find(m => m.id === c.id);
                requireValue(o.outcome.outcome === "untestable" || equal(c.sdk, m.sdk) && equal(c.runtime, m.runtime),
                    "candidate reproduction toolchain changed");
            }
            return { sha: o.target.sourceSha, outcome: o.outcome.outcome, observationDigest: r.observationDigest };
        });
    // Git has not selected a next item; the ledger's last completed item is not that item.
    const admission = admitBisection({ manifest: s.manifest, stable, main, good, ancestry,
        budget: { ...s.budget, nextWorkItemId: s.budget.pendingWork?.workItemId ?? "none" } });
    if (admission.decision !== "admit") return report(admission.decision === "stop" ? "stopped" : "blocked", admission.reason);
    // No bisect command, including reset, is permitted before this gate.
    await history.git(["bisect", "start", "--no-checkout", history.main, passing.sourceSha]);
    let ambiguous = false;
    for (const d of decisions) {
        const output = await history.git(["bisect", { pass: "good", "product-failure": "bad", untestable: "skip" }[d.outcome], d.sha], [0, 2]);
        ambiguous ||= output.includes("only 'skip'ped commits left");
    }
    const badSha = await history.git(["rev-parse", "refs/bisect/bad"]);
    const goods = (await history.git(["for-each-ref", "--format=%(objectname)", "refs/bisect/good-*"])).split("\n");
    requireValue(goods.every(fullSha), "missing Git good boundary");
    const remaining = (await history.git(["rev-list", "--topo-order", badSha, ...goods.map(sha => `^${sha}`)])).split("\n");
    let goodSha = passing.sourceSha;
    for (const sha of goods) if (await history.ancestor(goodSha, sha)) goodSha = sha;
    const interval = { goodSha, badSha, gitLog: [], decisions, remaining };
    if (ambiguous) return report("blocked", "all-skipped ambiguous interval", { interval, skips: s.skips });
    if (remaining.length === 1) {
        const relevantParents = (await history.git(["rev-list", "--parents", "-n", "1", badSha])).split(" ").slice(1);
        if (!capacity(cp, 1 + relevantParents.length, context.now, [badSha, ...relevantParents]))
            return report("blocked", "unconfirmed interval; insufficient fresh confirmation budget", { interval, budget: s.budget.consumed });
        return report("confirmation", "candidate requires fresh candidate and parent assertions", {
            candidate: badSha, parents: relevantParents, interval, toolchainTransitions: admission.toolchainTransitions ?? [],
        });
    }
    if (!capacity(cp, 3, context.now)) return report("blocked", "unconfirmed interval; preserving confirmation budget",
        { interval, skips: s.skips, budget: s.budget.consumed });
    const sha = await history.git(["rev-parse", "BISECT_HEAD"]);
    requireValue(fullSha(sha) && remaining.includes(sha) && !decisions.some(d => d.sha === sha), "invalid repeated Git selection");
    if (!capacity(cp, 3, context.now, [sha])) return report("blocked", "unconfirmed interval; candidate attempt budget exhausted",
        { interval, skips: s.skips, budget: s.budget.consumed });
    return { interval, goodDigest: passing.observationDigest,
        request: { kind: "source", target: { sourceSha: sha }, endpoint: null, phase: "bisect" } };
}

async function currentInput(now, deps) {
    const context = { ...deps.context, now, environmentFingerprint: fingerprint(deps.environments) };
    validateManifest(context.manifest);
    requireValue(["readSource", "authenticateRecord", "readTrustedCheckpoints"].every(k => typeof deps.approval?.[k] === "function"),
        "pinned approval assets and authenticated intake readers required");
    const snapshot = await (deps.intake?.readIssueSnapshot ?? intake.readIssueSnapshot)({
        ...deps.approval, transport: deps.intakeTransport ?? deps.transport, repository: context.repository,
        issueNumber: context.issueNumber, reviewedManifest: context.manifest, now,
    });
    if (snapshot.status !== "complete") return report(snapshot.status === "ineligible" ? "stopped" : "blocked", snapshot.reason);
    const current = snapshot.snapshot;
    requireValue(current.investigations?.status === "complete" && current.input.reproduction &&
        equal(current.input.reproduction.manifest, context.manifest) &&
        current.discussionFingerprint === context.manifest.discussionFingerprint, "incomplete or stale approved input");
    context.issue = current.input.discussion.issue;
    context.issueFingerprint = current.discussionFingerprint;
    requireValue(isEligibleIssue(context.issue), "issue is no longer eligible");
    const active = current.investigations.active.filter(a => !(a.basis === "trusted-checkpoint-active-run" &&
        a.commentId === context.commentId && a.owner.runId === context.owner.runId && a.owner.attempt === context.owner.attempt));
    return active.length ? report("blocked", "active linked investigation", { active }) : { context };
}

async function latestStable(context, deps, signal) {
    const resolved = await (deps.toolchains?.resolveLatestStableSdk ?? toolchains.resolveLatestStableSdk)(
        { policy: context.policy, platform: deps.platform, signal },
        { clock: () => new Date(context.now), ...deps.toolchainDependencies });
    requireValue(resolved.status === "resolved", `latest stable resolution blocked: ${resolved.error?.message}`);
    requireValue(resolved.toolchain.sdkVersion === context.stableSdk.version &&
        resolved.toolchain.metadata.some(m => m.url === context.stableSdk.releaseIndexUrl && m.sha256 === context.stableSdk.releaseIndexDigest),
    "latest stable identity changed; authenticated endpoint context must be refreshed");
    return resolved.toolchain;
}

async function advanceInvestigation({ now }, deps) {
    let history, cp, context;
    const signal = AbortSignal.any([AbortSignal.timeout(300000), ...(deps.signal ? [deps.signal] : [])]);
    try {
        const input = await currentInput(now, deps);
        if (!input.context) return input;
        context = input.context;
        let loaded = await loadCheckpoint(deps.transport, context);
        cp = loaded.checkpoint;
        if (context.pinnedMainSha !== cp.state.pinnedMainSha) {
            context.pinnedMainSha = cp.state.pinnedMainSha;
            loaded = await loadCheckpoint(deps.transport, context);
        }
        if (!loaded.executionAllowed) {
            await invalidateStaleEvidence(deps.transport, context, { generation: cp.generation });
            loaded = await loadCheckpoint(deps.transport, context);
            cp = loaded.checkpoint;
            if (!loaded.executionAllowed) return report("blocked", loaded.blockers.join("; "));
        }
        validateState(cp.state);
        requireValue(cp.state.epoch === fingerprintEpoch(context.manifest, cp.state.budget, context.pinnedMainSha), "stale epoch");
        const resolved = await latestStable(context, deps, signal);
        if (cp.pending) {
            const owner = cp.pending.owner;
            const run = await deps.transport.getRun(owner.runId, owner.attempt);
            if (run.status !== "completed") return report("blocked", "work already reserved by an active owner",
                { pending: { id: cp.pending.id, owner, deadline: cp.pending.deadline } });
        }
        if (cp.state.confirmation && ["confirmation", "complete"].includes(cp.state.phase)) {
            history = await historyWorkspace(deps.history, deps.execute ?? execute, signal);
            if (history.main !== cp.state.pinnedMainSha) {
                context.pinnedMainSha = history.main;
                cp = await invalidateStaleEvidence(deps.transport, context, { generation: cp.generation });
            } else return report(cp.state.phase, cp.state.confirmation.reason, {
                candidate: cp.state.confirmation.candidate, parents: cp.state.confirmation.parents.map(p => p.sourceSha),
                interval: { ...cp.state.interval, remaining: cp.state.confirmation.interval.remaining },
                generation: cp.generation, commands: history.commands,
            });
        }
        let selected;
        const stableId = cp.state.endpointEvidence.stable;
        if (!stableId) selected = { request: { kind: "released", target: { sdk: context.stableSdk }, endpoint: "stable", phase: "stable" } };
        else {
            const stable = authenticated(cp, stableId, deps.environments);
            requireValue(stable.kind === "released" && equal(stable.target.sdk, context.stableSdk), "wrong stable endpoint identity");
            if (stable.outcome.outcome !== "product-failure") selected = report(
                stable.outcome.outcome === "pass" ? "stopped" : "blocked",
                stable.outcome.outcome === "pass" ? "not reproduced on latest stable" : `stable untestable: ${stable.outcome.reason}`);
            else {
                history ??= await historyWorkspace(deps.history, deps.execute ?? execute, signal);
                if (history.main !== cp.state.pinnedMainSha) {
                    context.pinnedMainSha = history.main;
                    cp = await invalidateStaleEvidence(deps.transport, context, { generation: cp.generation });
                }
                const mainId = cp.state.endpointEvidence.main;
                if (!mainId) selected = { request: { kind: "source", target: { sourceSha: history.main }, endpoint: "main", phase: "main" } };
                else {
                    const main = authenticated(cp, mainId, deps.environments);
                    requireValue(main.kind === "source" && main.target.sourceSha === history.main, "wrong pinned-main endpoint identity");
                    if (main.outcome.outcome !== "product-failure") selected = report(
                        main.outcome.outcome === "pass" ? "stopped" : "blocked",
                        main.outcome.outcome === "pass" ? "release-specific or fixed on main" : `main untestable: ${main.outcome.reason}`);
                    else selected = await selectSource(cp, context, deps, history);
                }
            }
        }
        if ((selected.interval || selected.status) && cp.pending === null) {
            const { remaining, ...interval } = selected.interval ?? cp.state.interval ?? {};
            cp = await recordSearchProgress(deps.transport, context, {
                generation: cp.generation, phase: selected.status ?? "bisect",
                interval: selected.interval || cp.state.interval ? interval : null,
                ...(selected.goodDigest ? { goodDigest: selected.goodDigest } : {}),
            });
        }
        if (selected.status) return { ...selected, generation: cp.generation, commands: history?.commands ?? [] };
        signal.throwIfAborted();
        const attemptId = fingerprint({ owner: context.owner, sequence: cp.sequence + 1, epoch: cp.state.epoch }).slice(0, 32);
        const reservation = await reserveWork(deps.transport, context, { generation: cp.generation, attemptId, request: selected.request });
        return { status: "work", reservation, binding: {
            issue: context.issue, issueFingerprint: context.issueFingerprint, pinnedMainSha: context.pinnedMainSha,
            environmentFingerprint: context.environmentFingerprint,
        }, resolved, commands: history?.commands ?? [] };
    } catch (error) {
        return report("blocked", error.message, { ...(cp ? { budget: cp.state.budget.consumed } : {}), commands: history?.commands ?? [] });
    } finally { if (history) await history.dispose(); }
}

async function executeInvestigationWork(work, deps) {
    requireValue(work.status === "work" && work.reservation.execute, "work has no new execution permission");
    const clock = deps.clock ?? (() => new Date());
    const input = await currentInput(clock().toISOString(), deps);
    requireValue(input.context, input.reason);
    const context = input.context;
    let loaded = await loadCheckpoint(deps.transport, context);
    if (context.pinnedMainSha !== loaded.checkpoint.state.pinnedMainSha) {
        context.pinnedMainSha = loaded.checkpoint.state.pinnedMainSha;
        loaded = await loadCheckpoint(deps.transport, context);
    }
    const r = loaded.checkpoint.pending;
    requireValue(loaded.executionAllowed && r?.id === work.reservation.id &&
        r.generation === work.reservation.generation && equal(r.request, work.reservation.request),
    "work reservation no longer current");
    requireValue(equal(work.binding, { issue: context.issue, issueFingerprint: context.issueFingerprint,
        pinnedMainSha: context.pinnedMainSha, environmentFingerprint: context.environmentFingerprint }), "stale work binding");
    requireValue(typeof deps.collectObservation === "function", "trusted observation collector required");
    requireValue(r.request.kind !== "source" || deps.worker.reproductionToolchain,
        "frozen reproduction toolchain required separately from revision build SDK");
    const remaining = Date.parse(r.deadline) - clock().getTime();
    requireValue(remaining > 0, "reservation deadline exhausted");
    const signal = AbortSignal.any([AbortSignal.timeout(remaining), ...(deps.signal ? [deps.signal] : [])]);
    const checkDeadline = () => {
        signal.throwIfAborted();
        requireValue(clock().getTime() < Date.parse(r.deadline), "reservation deadline exhausted");
    };
    checkDeadline();
    const resolved = await latestStable({ ...context, now: clock().toISOString() }, deps, signal);
    if (r.request.kind === "source") {
        const history = await historyWorkspace(deps.history, deps.execute ?? execute, signal);
        try { requireValue(history.main === context.pinnedMainSha, "current main changed; source work must be reselected"); }
        finally { await history.dispose(); }
    }
    checkDeadline();
    const started = await beginWork(deps.transport, { ...context, now: clock().toISOString() },
        { generation: r.generation, attemptId: r.id });
    if (!started.execute) return report("blocked", "work execution already claimed");
    const manifest = loaded.checkpoint.state.manifest;
    checkDeadline();
    let result;
    if (r.request.kind === "source") {
        result = await (deps.testSourceRevision ?? testSourceRevision)({
            ...deps.worker, manifest, signal, buildInstanceId: r.id,
            request: { schemaVersion: 1, repository: context.repository, sourceSha: r.request.target.sourceSha,
                phase: r.request.phase === "good-search" ? "older-search" : r.request.phase,
                manifestDigest: fingerprint(manifest), approvalDigest: manifest.approvedInputDigest,
                platform: deps.platform, allocation: { buildTimeoutMs: context.policy.limits.buildTimeoutMinutes * 60000,
                    workerTimeoutMs: context.policy.limits.workerTimeoutMinutes * 60000 } },
        }, deps.workspaceDependencies);
    } else {
        const acquired = await (deps.acquireToolchain ?? toolchains.acquireToolchain)({
            policy: context.policy, toolchain: resolved, storageDirectory: deps.worker.storageDirectory, signal,
        }, deps.toolchainDependencies);
        checkDeadline();
        if (acquired.status !== "acquired") result = acquired;
        else {
            const verified = (deps.verifyToolchainIdentity ?? toolchains.verifyToolchainIdentity)({
                policy: context.policy, toolchain: acquired.identity.requested, identity: acquired.identity,
            });
            requireValue(verified.status === "verified", "stable acquisition identity rejected");
            requireValue(acquired.identity.archive.sha256 === context.stableSdk.archives.find(a =>
                a.os === deps.platform.os && a.architecture === deps.platform.architecture)?.sha256, "stable archive identity changed");
            checkDeadline();
            const product = await deps.inventoryReleased(acquired.identity, r.id);
            checkDeadline();
            result = await (deps.runReproduction ?? runReproduction)({
                ...deps.worker, manifest, product, toolchain: acquired.identity,
            }, { execute: (exe, args, settings) => (deps.workerExecute ?? execute)(exe, args, { ...settings, signal }) });
        }
    }
    checkDeadline();
    const collected = await deps.collectObservation({ result, reservation: r, manifest, context });
    checkDeadline();
    validateObservation(collected.observation, manifest);
    requireValue(equal(collected.observation.outcome, result.outcome), "collector changed worker outcome");
    if (result.outcome.outcome !== "untestable") {
        const reproduction = r.request.kind === "source" ? result.reproduction : result;
        requireValue(r.request.kind !== "source" || result.status === "tested" &&
            result.actualSourceSha === r.request.target.sourceSha && result.buildInstanceId === r.id, "source result identity mismatch");
        requireValue(reproduction?.cells.length === manifest.matrix.length &&
            collected.observation.cells.every(c => equal(c.attempts, reproduction.cells.find(r => r.id === c.id)?.attempts)),
        "collector omitted original repeated assertions");
    }
    return commitObservation(deps.transport, { ...context, now: clock().toISOString() }, {
        generation: started.generation, attemptId: r.id, observation: collected.observation, evidence: collected.evidence,
    });
}

module.exports = {
    advanceInvestigation, executeInvestigationWork,
    historyWorkspace, currentInput, authenticated, capacity,
};
