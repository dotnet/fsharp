const { createHash } = require("node:crypto");
const {
    fingerprintInput: fingerprint, validateObservation, validateConfirmation, validateAssociations, admitBisection,
} = require("./model.cjs");
const engine = require("./engine.cjs");
const state = require("./state.cjs");
const { execute } = require("./repro-process.cjs");
const { requireValue } = require("./toolchain-io.cjs");

const equal = (a, b) => fingerprint(a) === fingerprint(b);
const sha = value => typeof value === "string" && /^[a-f0-9]{40}$/.test(value);
const hash = bytes => createHash("sha256").update(bytes).digest("hex");
const boundedReason = error => String(error.message ?? error).slice(0, 1024) || "confirmation evidence unavailable";
const toolchainPath = /(?:^|\/)(?:global\.json|NuGet\.config|build\.(?:cmd|sh|ps1))$|\.(?:runtimeconfig\.json|props|targets|[fc]sproj|proj)$|^eng\//i;

async function commitEvidence(history, sourceSha) {
    const parents = (await history.git(["rev-list", "--parents", "-n", "1", sourceSha])).split(" ").slice(1);
    requireValue(parents.length <= 16 && parents.every(sha) && new Set(parents).size === parents.length, "invalid Git parent evidence");
    const message = await history.git(["show", "-s", "--format=%B", sourceSha]);
    const output = await history.git(["-c", "core.quotePath=true", "diff-tree", "--root", "-m",
        "--no-commit-id", "--name-only", "-r", sourceSha], [0], { raw: true });
    requireValue(typeof output === "string" && Buffer.byteLength(output) <= 1024 * 1024 &&
        (output === "" || output.endsWith("\n")), "oversized or incomplete Git path evidence");
    const paths = [...new Set(output === "" ? [] : output.slice(0, -1).split("\n"))].sort();
    let toolchainCoupled = false;
    for (const p of paths) {
        requireValue(p.length > 0 && p.length <= 16384 && !/[\u0000-\u001f\u007f-\uffff]/.test(p) &&
            (p.startsWith('"') ? /^"(?:[^"\\]|\\(?:[0-3][0-7]{2}|[abtnvfr"\\]))*"$/.test(p) : !/["\\]/.test(p)),
        "malformed Git-quoted path evidence");
        const decoded = p.startsWith('"') ? p.slice(1, -1).replace(/\\([0-3][0-7]{2}|[abtnvfr"\\])/g,
            (_, code) => /^[0-3]/.test(code) ? String.fromCharCode(Number.parseInt(code, 8)) :
                ({ a: "\x07", b: "\b", t: "\t", n: "\n", v: "\v", f: "\f", r: "\r", '"': '"', "\\": "\\" })[code]) : p;
        toolchainCoupled ||= toolchainPath.test(decoded);
    }
    const changedPaths = [];
    for (const p of paths) {
        if (p.length > 1024 || changedPaths.length === 256 ||
            Buffer.byteLength(JSON.stringify([...changedPaths, p])) > 4096) break;
        changedPaths.push(p);
    }
    // This is only a patch-similarity fingerprint, never proof of provenance.
    const patch = parents.length <= 1 ? await history.git(["show", "--format=", "--no-ext-diff",
        "--no-textconv", "--no-renames", "--root", sourceSha]) : null;
    const patchId = patch === null || patch === "" ? null : createHash("sha1").update(patch
        .replace(/^index .*\n|^@@.*\n/gm, "").replace(/\s/g, "")).digest("hex");
    const references = regex => [...message.matchAll(regex)].map(m => m[1]);
    return {
        sha: sourceSha, parents, messageDigest: hash(message), changedPaths,
        changedPathsSummary: { encoding: "git-quoted", totalCount: paths.length, sha256: fingerprint(paths),
            truncated: changedPaths.length !== paths.length, toolchainCoupled }, patchId,
        reverts: references(/\bThis reverts commit ([a-f0-9]{40})\b/g),
        cherryPicks: references(/\bcherry picked from commit ([a-f0-9]{40})\b/g),
        suspiciousMessage: /\b(?:revert(?:ed|ing)?|reintroduc\w*|cherry[ -]pick\w*|backport\w*)\b/i.test(message),
    };
}

async function resolveCommitAssociations({ repository, sourceSha, pinnedMainSha }, deps) {
    requireValue(repository === "dotnet/fsharp" && sha(sourceSha) && sha(pinnedMainSha), "invalid association target");
    const result = { schemaVersion: 1, repository, sourceSha, status: "incomplete", reason: "association lookup incomplete", pulls: [] };
    let history;
    try {
        history = await engine.historyWorkspace(deps.history, deps.execute ?? execute, deps.signal);
        requireValue(history.main === pinnedMainSha && await history.ancestor(sourceSha, pinnedMainSha),
            "association source is not on tested main");
        const request = async (route, params) => {
            const r = await deps.transport.request(`GET /repos/{owner}/{repo}/${route}`, { owner: "dotnet", repo: "fsharp", ...params });
            requireValue(r && (r.status === undefined || r.status === 200) && r.data !== undefined &&
                Buffer.byteLength(JSON.stringify(r.data)) <= 4 * 1024 * 1024, "incomplete association API response");
            return r;
        };
        const pages = async (route, params, limit = 1000) => {
            const rows = [];
            for (let page = 1; page <= 10; page++) {
                const r = await request(route, { ...params, per_page: 100, page });
                requireValue(Array.isArray(r.data) && r.data.length <= 100, "invalid association page");
                rows.push(...r.data);
                requireValue(rows.length <= limit, "association response bound exceeded");
                requireValue(r.headers?.link === undefined || typeof r.headers.link === "string", "invalid association pagination header");
                const next = r.headers?.link === undefined ? r.data.length === 100 : /;\s*rel="?next"?/.test(r.headers.link);
                if (!next) return rows;
            }
            throw new Error("association pagination incomplete");
        };
        const rows = await pages("commits/{commit_sha}/pulls", { commit_sha: sourceSha }, 100);
        requireValue(rows.length <= 100 && new Set(rows.map(p => p.number)).size === rows.length,
            "duplicate or oversized association set");
        for (const row of rows.sort((a, b) => a.number - b.number)) {
            requireValue(Number.isSafeInteger(row.number) && row.number > 0, "invalid associated PR number");
            const { data: p } = await request("pulls/{pull_number}", { pull_number: row.number });
            requireValue(p.number === row.number && p.html_url === `https://github.com/${repository}/pull/${p.number}` &&
                p.base?.repo?.full_name === repository && ["open", "closed"].includes(p.state) &&
                typeof p.merged === "boolean", "foreign or incomplete PR association");
            if (!p.merged) {
                requireValue(p.merged_at === null, "inconsistent merge status");
                continue;
            }
            requireValue(p.state === "closed" && Number.isFinite(Date.parse(p.merged_at)) &&
                sha(p.merge_commit_sha) && sha(p.base.sha) && sha(p.head?.sha) &&
                typeof p.base.ref === "string" && p.base.ref.length > 0 && p.base.ref.length <= 256, "incomplete merged PR details");
            const commits = await pages("pulls/{pull_number}/commits", { pull_number: p.number });
            requireValue(commits.every(c => sha(c?.sha)) && new Set(commits.map(c => c.sha)).size === commits.length &&
                commits.some(c => c.sha === p.head.sha) &&
                typeof p.head.repo?.full_name === "string", "invalid PR head/commit evidence");
            const relation = p.merge_commit_sha === sourceSha ? "merge-commit" : "contained-commit";
            requireValue(relation === "merge-commit" || commits.some(c => c.sha === sourceSha), "PR has no exact source commit");
            const onLineage = await history.ancestor(p.merge_commit_sha, pinnedMainSha) &&
                await history.ancestor(sourceSha, p.merge_commit_sha) &&
                p.base.sha !== sourceSha && await history.ancestor(p.base.sha, sourceSha);
            result.pulls.push({ number: p.number, url: p.html_url, mergeSha: p.merge_commit_sha,
                baseSha: p.base.sha, headSha: p.head.sha, baseRef: p.base.ref, relation, onLineage });
        }
        result.status = result.pulls.length === 0 ? "none" :
            result.pulls.length === 1 && result.pulls[0].onLineage && result.pulls[0].baseRef === "main" ? "unique" : "ambiguous";
        result.reason = {
            none: "No verified merged PR association; a source boundary does not require a PR.",
            unique: "Exact commit association and Git ancestry establish one merged PR on tested main.",
            ambiguous: "Multiple associations or servicing/off-lineage evidence; no unique introducing PR.",
        }[result.status];
    } catch (error) {
        result.status = "incomplete";
        result.reason = boundedReason(error);
    } finally { if (history) await history.dispose(); }
    validateAssociations(result);
    return result;
}

async function confirmBoundary({ intent, now }, deps) {
    let cp, context, history, confirmation, retained;
    const retain = () => { validateConfirmation(confirmation); retained = structuredClone(confirmation); };
    const fits = () => {
        const phase = confirmation.classification === "unconfirmed-candidate" ? "blocked" : "complete";
        return state.serializeWorkflowRecord({ kind: "checkpoint",
            status: `Regression investigation: ${phase} (generation ${cp.generation + 1}).`,
            payload: { ...cp, generation: cp.generation + 1, previousDigest: hash("size preflight"),
                state: { ...cp.state, phase, confirmation } } }, context);
    };
    const clock = deps.clock ?? (() => new Date());
    const signal = AbortSignal.any([AbortSignal.timeout(300000), ...(deps.signal ? [deps.signal] : [])]);
    try {
        const input = await engine.currentInput(now, deps);
        requireValue(input.context, input.reason);
        context = input.context;
        const loaded = await state.loadCheckpoint(deps.transport, context);
        cp = loaded.checkpoint;
        requireValue(loaded.executionAllowed && cp.pending === null, loaded.blockers.join("; ") || "pending worker");
        requireValue(intent?.requiresFreshConfirmation === true && ["confirmation", "blocked"].includes(intent.status) &&
            intent.generation === cp.generation && cp.state.phase === intent.status &&
            intent.interval && cp.state.interval &&
            equal(cp.state.interval, (({ remaining, ...i }) => i)(intent.interval)), "stale or unauthenticated confirmation intent");
        const candidate = intent.candidate ?? cp.state.interval.badSha;
        requireValue(candidate === cp.state.interval.badSha, "candidate does not match admitted interval");
        confirmation = {
            schemaVersion: 1, epoch: cp.state.epoch, manifestDigest: cp.state.approvalFingerprint,
            startGeneration: cp.generation, candidate, classification: "unconfirmed-candidate", reason: "confirmation not completed",
            interval: { goodSha: cp.state.interval.goodSha, badSha: candidate, remaining: null },
            parents: [], candidateObservationDigest: null, searchObservationDigest: null,
            history: { commits: [], monotonicity: "not-contradicted", toolchainCoupled: false },
            associations: null, latentRootCause: "unproven",
        };
        retain();
        history = await engine.historyWorkspace(deps.history, deps.execute ?? execute, signal);
        requireValue(history.main === cp.state.pinnedMainSha, "pinned main changed");
        const candidateInfo = await commitEvidence(history, candidate);
        confirmation.parents = candidateInfo.parents.map(sourceSha => ({ sourceSha, comparable: false, observationDigest: null }));
        confirmation.history.toolchainCoupled = candidateInfo.changedPathsSummary.toolchainCoupled;
        retain();
        requireValue(intent.parents === undefined || equal(intent.parents, candidateInfo.parents), "claimed parents differ from Git objects");
        requireValue(candidateInfo.parents.length > 0, "root commit has no parent boundary");
        const remaining = (await history.git(["rev-list", "--topo-order", "--max-count=4097", candidate,
            `^${cp.state.interval.goodSha}`])).split("\n").filter(Boolean);
        confirmation.interval.remaining = [...new Set(remaining.filter(sha))].slice(0, 4096);
        confirmation.interval.remainingSummary = { totalCount: remaining.length, sha256: fingerprint(remaining),
            truncated: confirmation.interval.remaining.length !== remaining.length };
        retain();
        requireValue(remaining.length <= 4096 && remaining.every(sha) && new Set(remaining).size === remaining.length &&
            remaining.includes(candidate) && equal(remaining, intent.interval.remaining), "candidate interval differs from Git ancestry");
        for (const parent of confirmation.parents) parent.comparable =
            await history.ancestor(cp.state.interval.goodSha, parent.sourceSha) && await history.ancestor(parent.sourceSha, history.main);
        retain();
        requireValue(confirmation.parents.every(p => p.comparable), "parent outside verified comparable lineage");
        const observations = Object.entries(cp.observations).filter(([, r]) => r.valid).map(([id]) =>
            [id, engine.authenticated(cp, id, deps.environments)]);
        const source = (target, outcome) => observations.find(([, o]) => o.kind === "source" &&
            o.target.sourceSha === target && o.outcome.outcome === outcome);
        const search = source(candidate, "product-failure");
        requireValue(search, "candidate lacks authenticated product failure");
        confirmation.searchObservationDigest = search[0];
        retain();
        const good = source(cp.state.interval.goodSha, "pass");
        requireValue(good, "interval lacks authenticated passing source");
        const admission = admitBisection({
            manifest: cp.state.manifest, stable: engine.authenticated(cp, cp.state.endpointEvidence.stable, deps.environments),
            main: engine.authenticated(cp, cp.state.endpointEvidence.main, deps.environments), good: good[1],
            ancestry: { goodSha: good[1].target.sourceSha, badSha: history.main, pinnedMainSha: history.main,
                isAncestor: await history.ancestor(good[1].target.sourceSha, history.main) },
            budget: { ...cp.state.budget, nextWorkItemId: "none" },
        });
        requireValue(admission.decision === "admit", admission.reason);
        const shas = [...new Set([...remaining, ...candidateInfo.parents])];
        requireValue(shas.length <= 128, "history evidence bound exceeded; parent uncertainty");
        for (const revision of shas) {
            const { suspiciousMessage, ...evidence } = revision === candidate ? candidateInfo : await commitEvidence(history, revision);
            confirmation.history.commits.push(evidence);
            if (suspiciousMessage || evidence.reverts.length || evidence.cherryPicks.length)
                confirmation.history.monotonicity = "uncertain";
            retain();
            for (const referenced of [...evidence.reverts, ...evidence.cherryPicks]) {
                if (shas.includes(referenced)) continue;
                requireValue(shas.length < 128, "related patch history bound exceeded");
                await history.git(["fetch", "--quiet", "--no-tags", "--no-recurse-submodules",
                    deps.history.mirrorDirectory, referenced]);
                shas.push(referenced);
            }
        }
        const patches = confirmation.history.commits.map(c => c.patchId).filter(Boolean);
        if (new Set(patches).size !== patches.length) confirmation.history.monotonicity = "uncertain";
        for (const [, bad] of observations.filter(([, o]) => o.kind === "source" && o.outcome.outcome === "product-failure"))
            for (const [, pass] of observations.filter(([, o]) => o.kind === "source" && o.outcome.outcome === "pass"))
                if (await history.ancestor(bad.target.sourceSha, pass.target.sourceSha))
                    confirmation.history.monotonicity = "contradicted";
        retain();
        requireValue(confirmation.history.monotonicity === "not-contradicted",
            "revert, reintroduction, cherry-pick or nonmonotonic evidence prevents unique attribution");
        requireValue(remaining.length === 1 || candidateInfo.parents.length > 1,
            "unconfirmed interval contains other possible source commits");
        const producer = cp.state.manifest.producer;
        if (producer.name !== "no-producer") {
            const source = cp.state.manifest.sources.find(s => s.sha256 === producer.sha256);
            requireValue(source, "frozen producer must be a reviewed source asset");
            const bytes = await deps.approval.readSource(structuredClone(source));
            requireValue(bytes instanceof Uint8Array && hash(bytes) === producer.sha256, "frozen producer hash mismatch");
        } else requireValue(producer.version === "0.0.0" && producer.sha256 === hash(""), "invalid absent producer identity");
        const targets = [candidate, ...candidateInfo.parents];
        fits();
        requireValue(engine.capacity(cp, targets.length, now, targets), "budget cannot afford independent candidate and all parents");
        const initialBuilds = new Set(observations.map(([, o]) => o.buildInstanceId));
        for (const sourceSha of targets) {
            const current = await engine.currentInput(clock().toISOString(), deps);
            requireValue(current.context, current.reason);
            context = current.context;
            const fresh = await state.loadCheckpoint(deps.transport, context);
            requireValue(fresh.executionAllowed && fresh.checkpoint.generation === cp.generation &&
                fresh.checkpoint.state.epoch === confirmation.epoch, "confirmation checkpoint changed");
            const reservation = await state.reserveWork(deps.transport, context, {
                generation: cp.generation, attemptId: fingerprint({ epoch: confirmation.epoch,
                    startGeneration: confirmation.startGeneration, sourceSha, owner: context.owner }).slice(0, 32),
                request: { kind: "source", target: { sourceSha }, endpoint: null, phase: "confirmation" },
            });
            cp = await engine.executeInvestigationWork({ status: "work", reservation, binding: {
                issue: context.issue, issueFingerprint: context.issueFingerprint,
                pinnedMainSha: context.pinnedMainSha, environmentFingerprint: context.environmentFingerprint,
            } }, deps);
            requireValue(cp.observations, cp.reason ?? "confirmation worker did not commit");
            const record = cp.records.find(r => r.id === reservation.id);
            const o = engine.authenticated(cp, record.observationDigest, deps.environments);
            validateObservation(o, cp.state.manifest, { finalConfirmation: { runId: reservation.owner.runId, buildInstanceId: reservation.id } });
            requireValue(!initialBuilds.has(o.buildInstanceId) && o.target.sourceSha === sourceSha, "reused build or wrong source");
            initialBuilds.add(o.buildInstanceId);
            requireValue(o.cells.every(c => c.outcome.outcome === "untestable" || c.buildToolchain) &&
                o.cells.every(c => {
                    const previous = search[1].cells.find(p => p.id === c.id);
                    return equal(c.environment, previous.environment) && equal(c.sdk, previous.sdk) && equal(c.runtime, previous.runtime);
                }), "incomparable sample environment or missing revision toolchain");
            if (sourceSha === candidate) confirmation.candidateObservationDigest = record.observationDigest;
            else confirmation.parents.find(p => p.sourceSha === sourceSha).observationDigest = record.observationDigest;
            retain();
        }
        const candidateObservation = cp.observations[confirmation.candidateObservationDigest].data;
        requireValue(candidateObservation.outcome.outcome === "product-failure", "candidate no longer deterministically fails");
        requireValue(confirmation.parents.every(p => cp.observations[p.observationDigest].data.outcome.outcome === "pass"),
            "parent failure, skip or flake leaves the boundary unconfirmed");
        confirmation.history.toolchainCoupled = candidateInfo.changedPathsSummary.toolchainCoupled ||
            confirmation.parents.some(p => cp.observations[p.observationDigest].data.cells.some(c => {
                const current = candidateObservation.cells.find(n => n.id === c.id).buildToolchain;
                return !equal(current.sdk, c.buildToolchain.sdk) || !equal(current.compilerRuntime, c.buildToolchain.compilerRuntime);
            }));
        retain();
        requireValue(confirmation.history.commits.every(c => !c.changedPathsSummary.truncated),
            "incomplete changed-path evidence prevents unique attribution");
        confirmation.classification = candidateInfo.parents.length > 1 || confirmation.history.toolchainCoupled ?
            "verified-triggering-change" : "verified-introducing-source-boundary";
        confirmation.reason = candidateInfo.parents.length > 1 ? "Fresh merge failure and all comparable parents pass: integration trigger only." :
            confirmation.history.toolchainCoupled ? "Fresh source boundary; source and build toolchain responsibility remains coupled/unknown." :
                "Fresh independent candidate failure and parent pass establish the introducing source boundary, not a semantic root cause.";
        if (deps.associationTransport) {
            confirmation.associations = await resolveCommitAssociations({ repository: context.repository,
                sourceSha: candidate, pinnedMainSha: context.pinnedMainSha },
            { transport: deps.associationTransport, history: deps.history, execute: deps.execute, signal: deps.signal });
        }
        validateConfirmation(confirmation);
        cp = await state.recordConfirmation(deps.transport, context, { generation: cp.generation, confirmation });
        return { status: "confirmed", reason: confirmation.reason, confirmation, generation: cp.generation, budget: cp.state.budget.consumed };
    } catch (error) {
        const reason = boundedReason(error);
        if (retained) {
            confirmation = { ...retained, classification: "unconfirmed-candidate", reason };
            const loaded = await state.loadCheckpoint(deps.transport, context);
            cp = loaded.checkpoint;
            if (loaded.executionAllowed && cp.pending === null && cp.state.epoch === confirmation.epoch) {
                for (;;) {
                    try { fits(); break; } catch (sizeError) {
                        if (!/48 KiB/.test(sizeError.message)) throw sizeError;
                        const { interval, history } = confirmation;
                        if (history.commits.length) {
                            history.commitsSummary ??= { totalCount: history.commits.length,
                                sha256: fingerprint(history.commits), truncated: false };
                            history.commits = history.commits.slice(0, Math.floor(history.commits.length / 2));
                            history.commitsSummary.truncated = true;
                        } else if (interval.remaining?.length > 1) {
                            interval.remainingSummary ??= { totalCount: interval.remaining.length,
                                sha256: fingerprint(interval.remaining), truncated: false };
                            interval.remaining = interval.remaining.slice(0, Math.ceil(interval.remaining.length / 2));
                            interval.remainingSummary.truncated = true;
                        } else throw sizeError;
                    }
                }
                validateConfirmation(confirmation);
                cp = await state.recordConfirmation(deps.transport, context, { generation: cp.generation, confirmation });
            }
        }
        return { status: confirmation ? "unconfirmed" : "blocked", reason,
            ...(confirmation ? { confirmation } : {}), ...(cp ? { generation: cp.generation, budget: cp.state.budget.consumed } : {}) };
    } finally { if (history) await history.dispose(); }
}

module.exports = { confirmBoundary, resolveCommitAssociations };
