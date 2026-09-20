const model = require("./model.cjs");
const state = require("./state.cjs");
const { readIssueSnapshot } = require("./intake.cjs");
const { createHash } = require("node:crypto");

const fingerprint = model.fingerprintInput;
const equal = (a, b) => fingerprint(a) === fingerprint(b);
const requireValue = (value, message) => { if (!value) throw new Error(message); };
const digest = value => typeof value === "string" && /^[a-f0-9]{64}$/.test(value);
const sha = value => typeof value === "string" && /^[a-f0-9]{40}$/.test(value);
const positive = value => Number.isSafeInteger(value) && value > 0;
const hash = bytes => createHash("sha256").update(bytes).digest("hex");
function fields(value, names) {
    requireValue(value && Object.getPrototypeOf(value) === Object.prototype &&
        equal(Object.keys(value).sort(), names.split(" ").sort()), "invalid report schema fields");
}
function reproductionDigest(manifest) {
    const { discussionFingerprint, approvedInputDigest, searchHints, ...reproduction } = manifest;
    return fingerprint(reproduction);
}
function safeEvidenceUrl(url) {
    requireValue(typeof url === "string" && (
        /^https:\/\/api\.github\.com\/repos\/dotnet\/fsharp\/actions\/artifacts\/[1-9]\d*\/zip$/.test(url) ||
        /^https:\/\/github\.com\/dotnet\/fsharp\/actions\/runs\/[1-9]\d*\/artifacts\/[1-9]\d*$/.test(url)),
    "unsafe evidence origin or path");
}
function validateReport(report) {
    fields(report, "schemaVersion kind state observations deadline inputDigest reproductionDigest policyDigest checkedHead");
    requireValue(report.schemaVersion === 1 && report.kind === "regression-investigation", "invalid report kind");
    model.validateState(report.state);
    const s = report.state;
    requireValue(s.publication.digest === null && s.publication.commentId === null &&
        report.inputDigest === s.manifest.approvedInputDigest &&
        report.reproductionDigest === reproductionDigest(s.manifest) &&
        report.policyDigest === fingerprint(s.budget.policy) && sha(report.checkedHead) && report.checkedHead === s.pinnedMainSha &&
        typeof report.deadline === "string" && Number.isFinite(Date.parse(report.deadline)), "report identity mismatch");
    requireValue(Array.isArray(report.observations) && report.observations.length <= s.budget.policy.limits.workerUnits,
        "invalid report observations");
    const identities = [];
    for (const record of report.observations) {
        fields(record, "digest data evidence");
        model.validateObservation(record.data, s.manifest);
        requireValue(record.digest === fingerprint(record.data) &&
            record.data.provenance.workflowRevision === s.budget.workflowRevision &&
            record.data.provenance.policyRevision === s.budget.policyRevision, "report observation identity mismatch");
        requireValue(fingerprint(Object.fromEntries(record.data.cells.map(c => [c.id, c.environment]))) ===
            s.budget.environmentFingerprint, "report observation environment mismatch");
        requireValue(Array.isArray(record.evidence) && record.evidence.length <= 16, "invalid report evidence");
        for (const item of record.evidence) {
            fields(item, "required sha256 url available");
            requireValue(digest(item.sha256) && typeof item.required === "boolean" && typeof item.available === "boolean" &&
                (!item.required || item.available), "invalid or unavailable required report evidence");
            safeEvidenceUrl(item.url);
        }
        identities.push(record.digest);
    }
    requireValue(equal(identities, s.budget.authenticatedObservationDigests), "report evidence authentication mismatch");
    if (s.confirmation) {
        model.validateConfirmation(s.confirmation);
        requireValue(s.confirmation.epoch === s.epoch && s.confirmation.manifestDigest === s.approvalFingerprint,
            "stale report confirmation");
        for (const id of [s.confirmation.candidateObservationDigest, s.confirmation.searchObservationDigest,
            ...s.confirmation.parents.map(p => p.observationDigest)].filter(Boolean)) {
            requireValue(identities.includes(id), "unauthenticated report confirmation");
        }
        if (s.confirmation.classification !== "unconfirmed-candidate") {
            const released = report.observations.find(o => o.digest === s.endpointEvidence.stable)?.data;
            requireValue(released?.kind === "released" && equal(released.target.sdk, s.budget.stableSdk) &&
                released.outcome.outcome === "product-failure" && released.cells.every(cell =>
                    cell.releasedToolchain && cell.runtime && cell.artifacts.compiler?.assemblyVersion &&
                    cell.artifacts.core?.assemblyVersion),
            "verified report requires released endpoint archive, compiler runtime and compiler/Core assembly versions");
        }
    }
    requireValue(Buffer.byteLength(JSON.stringify(report)) <= 2 * 1024 * 1024, "report evidence too large");
    return true;
}

// Markdown is generated only from typed evidence, never supplied by the issue or a worker.
function display(value, limit = 320) {
    const text = typeof value === "string" ? value : JSON.stringify(value);
    return text.slice(0, limit).replace(/[\u0000-\u001f\u007f-\u009f\u2028-\u202e\u2066-\u2069]/g, c =>
        c === "\n" ? "\\n" : c === "\r" ? "\\r" : c === "\t" ? "\\t" : "\uFFFD")
        .replace(/[\\`*_{}\[\]()#+.!|]/g, "\\$&")
        .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/@/g, "&#64;") +
        (text.length > limit ? " … [truncated; see authenticated evidence]" : "");
}
function renderReport(report) {
    validateReport(report);
    const s = report.state, m = s.manifest, b = s.budget, c = s.confirmation;
    const lines = [
        "## Regression investigation",
        `Repository/issue: ${s.repository}#${s.issueNumber}`,
        `Classification: ${c ? c.classification.replaceAll("-", " ") : "unconfirmed candidate"}.`,
        `Stop reason: ${display(c?.reason ?? `checkpoint phase ${s.phase}; no verified source confirmation recorded`)}.`,
        "The latent root cause is unproven. Bisection does not establish semantic causation.",
        `Input: ${report.inputDigest}; discussion: ${s.issueFingerprint}; epoch: ${s.epoch}.`,
        `Approval manifest: ${s.approvalFingerprint}; approval revision: ${b.authorization.approvalRevision}; path: ${display(b.authorization.approvalPath)}.`,
        `Policy: ${report.policyDigest} at ${b.policyRevision}; workflow: ${b.workflowRevision}; reproduction: ${report.reproductionDigest}.`,
        `Tested pinned main: ${s.pinnedMainSha}; current-head check: ${report.checkedHead} (matched at preparation; recheck before writing).`,
        `Sample environment fingerprint: ${b.environmentFingerprint ?? "not recorded"}.`,
        "",
        "### Approved commands and frozen reproduction",
        `Reproduction kind: ${m.kind}; timeout: ${m.timeoutSeconds}s. Operations below are runner commands, not arbitrary shell input.`,
        ...m.sources.map(source => `- Source ${display(source.path)}: SHA-256 ${source.sha256}.`),
        `Retained producer: ${display(m.producer.name)} ${m.producer.version}; SHA-256 ${m.producer.sha256}.`,
        ...m.packages.map(p => `- Package ${display(p.id)} ${p.version}: SHA-256 ${p.sha256}.`),
        ...m.matrix.map(cell => `- ${display(cell.id)}: ${cell.os}/${cell.architecture}; TFM ${cell.tfm}; ${cell.configuration}; language ${cell.languageVersion}; approved operation ${display(cell.command.join(" "))}.`),
        "",
        "### Expected assertions",
        ...Object.entries(m.assertions).map(([name, assertion]) =>
            `- ${name}: exit ${assertion.exitCode}; stdout ${display(JSON.stringify(assertion.stdout), 1024)}; stderr ${display(JSON.stringify(assertion.stderr), 1024)}; assertion digest ${fingerprint(assertion)}.`),
        "",
        "### Released SDK selection",
        `Latest stable selected: ${b.stableSdk.version}; official release index ${display(b.stableSdk.releaseIndexUrl)}; metadata SHA-256 ${b.stableSdk.releaseIndexDigest}.`,
        ...b.stableSdk.archives.map(a => `- ${a.os}/${a.architecture}: archive SHA-256 ${a.sha256}.`),
        ...(s.endpointEvidence.stable ? [] : ["Released endpoint archive/runtime/compiler/Core evidence: not recorded."]),
        "",
        "### Endpoint and candidate evidence",
        ...Object.entries(s.endpointEvidence).map(([name, id]) => `- Endpoint ${name}: ${id ?? "not recorded"}.`),
        `Candidate: ${c?.candidate ?? "not confirmed"}.`,
        `Remaining interval: ${display(c?.interval ?? s.interval ?? "not recorded", 1600)}.`,
        `Parent evidence: ${c?.parents.length ? "" : "not recorded"}`,
        ...(c?.parents ?? []).map(p => `- ${p.sourceSha}: comparable ${p.comparable}; observation ${p.observationDigest ?? "not recorded"}.`),
        ...s.skips.map(skip => `- Skipped ${skip.sourceSha}: ${skip.reason}; observation ${skip.observationDigest}.`),
        `Monotonicity: ${c?.history.monotonicity ?? "uncertain"}; source/toolchain coupled: ${c ? c.history.toolchainCoupled : "not established"}.`,
        ...(c?.history.commits ?? []).map(commit =>
            `- History ${commit.sha}: parents ${commit.parents.join(", ") || "root"}; patch ${commit.patchId ?? "not recorded"}; reverts ${commit.reverts.join(", ") || "none recorded"}; cherry-picks ${commit.cherryPicks.join(", ") || "none recorded"}; changed paths ${display(commit.changedPaths, 512)}; message SHA-256 ${commit.messageDigest}.`),
        `PR association: ${c?.associations?.status ?? "not established"}; ${display(c?.associations?.reason ?? "no verified association evidence")}.`,
        ...(c?.associations?.pulls ?? []).map(p =>
            `- ${p.url}: ${p.relation}; merge ${p.mergeSha}; base ${p.baseSha}; head ${p.headSha}; base ref ${display(p.baseRef)}; tested lineage ${p.onLineage}.`),
        "",
        "### Authenticated observations and logs",
    ];
    const toolchains = new Set(), samples = new Set(), attempts = new Set();
    for (const record of report.observations) {
        const o = record.data;
        lines.push(`- Observation ${record.digest}: ${o.kind} ${o.kind === "source" ? o.target.sourceSha : o.target.sdk.version}; ${o.outcome.outcome}/${o.outcome.reason}; build ID ${display(o.buildInstanceId)}; cached ${o.cached}; run ${display(o.provenance.runId)}; worker ${display(o.provenance.workerId)}.`);
        for (const cell of o.cells) {
            const sample = fingerprint({ environment: cell.environment, sdk: cell.sdk, runtime: cell.runtime });
            lines.push(samples.has(sample)
                ? `  - Matrix ${display(cell.id)}: same sample SDK/runtime/environment ${sample}.`
                : `  - Matrix ${display(cell.id)}: sample ${sample}; ${cell.environment.os}/${cell.environment.architecture}; image ${cell.environment.imageDigest}; SDK ${cell.sdk?.version ?? "not recorded"} (${cell.sdk?.sha256 ?? "unavailable"}); runtime ${cell.runtime?.version ?? "not recorded"} (${cell.runtime?.sha256 ?? "unavailable"}).`);
            samples.add(sample);
            lines.push("  - Artifacts below belong to this observation's source/build ID:");
            for (const [name, artifact] of Object.entries(cell.artifacts)) {
                lines.push(`  - ${name}: ${artifact ? `${artifact.origin}; SHA-256 ${artifact.sha256}` +
                    (name === "targets" ? "" : `; assembly version ${artifact.assemblyVersion ?? "not recorded"}`) : "identity unavailable"}.`);
            }
            const released = o.kind === "released";
            const t = released ? cell.releasedToolchain : cell.buildToolchain;
            const toolchainId = t && fingerprint({ ...t, sourceSha: null });
            if (t && toolchains.has(toolchainId)) {
                lines.push(`  - ${released ? "Released" : "Revision"} toolchain: same identity and archive receipt ${toolchainId}, for this observation.`);
            } else if (t) {
                toolchains.add(toolchainId);
                lines.push(released
                    ? `  - Released toolchain ${toolchainId}: selected SDK ${t.sdk.version}; SHA-256 ${t.sdk.sha256}; Released compiler runtime ${t.compilerRuntime.version}; SHA-256 ${t.compilerRuntime.sha256}.`
                    : `  - Revision toolchain ${toolchainId}: global.json SHA-256 ${t.globalJsonDigest}; evidence ${t.evidenceDigest}; requested SDK/tools/runtime ${t.requestedSdk}/${t.requestedTools}/${t.requestedCompilerRuntime}; resolved SDK/runtime ${t.sdk.version}/${t.compilerRuntime.version}.`);
                const a = t.archive;
                lines.push(`  - ${released ? "Released" : "Source"} archive receipt: ${display(a.url)}; ${a.rid}; SHA-256 ${a.sha256}; actual SHA-512 ${a.actualSha512}; ${a.bytesReceived} bytes; received ${a.receivedAt}; retained SHA-256 ${a.retained.sha256}.`);
                lines.push(a.provenance.kind === "publisher-checksum"
                    ? `  - Publisher-provided SHA-512 ${a.sha512}.`
                    : `  - Approved official-HTTPS measured archive hash, NOT a publisher-provided checksum; approval ${a.provenance.approvalRevision}/${a.provenance.approvalDigest}.`);
                lines.push(`  - Archive final origin ${display(a.finalOrigin)}; final URL ${display(a.redirectUrl)}; redirects ${display(a.redirects)}.`);
            } else lines.push(released ? "  - Released SDK archive receipt and compiler runtime: not recorded." :
                "  - Revision-specific requested/resolved build toolchain and archive receipt: not recorded.");
            cell.attempts.forEach((attempt, i) => {
                const id = fingerprint(attempt);
                lines.push(attempts.has(id)
                    ? `  - Assertion repetition ${i + 1}: identical checks and streams to ${id}; ${attempt.execution.status}; exit ${attempt.execution.exitCode}.`
                    : `  - Assertion repetition ${i + 1}: ${attempt.execution.status}; exit ${attempt.execution.exitCode}; checks ${display(attempt.checks)}; stdout ${display(JSON.stringify(attempt.execution.stdout), 256)}; stderr ${display(JSON.stringify(attempt.execution.stderr), 256)}; full attempt digest ${id}.`);
                attempts.add(id);
            });
        }
        lines.push(...record.evidence.map(e => `  - ${e.required ? "Required" : "Optional"} log/evidence reference: ${display(e.url)}; SHA-256 ${e.sha256}; ${e.available ? "hash verified" : "unavailable or expired (including hash mismatch)"}.`));
        if (!record.evidence.length) lines.push("  - External log references not recorded; assertion streams are retained in the authenticated result envelope.");
    }
    lines.push("", "### Budget and resume",
        `Allowance ${display(b.allowanceId)}; deadline ${report.deadline}; ${b.consumed.sourceBuilds}/${b.policy.limits.sourceBuilds} source builds; ${b.consumed.workerUnits}/${b.policy.limits.workerUnits} worker units; ${b.consumed.reservedMinutes}/${b.policy.limits.reservedMinutes} reserved minutes.`,
        `Charged attempts: ${display(b.attempts, 1600)}. Cancellations and stale inputs do not refund allowance.`,
        `Resume: ${s.phase === "complete" ? "confirmation complete; publication only" : "trusted controller may resume from the authenticated checkpoint after refreshing approval and inputs"}; phase ${s.phase}; no automatic reset of spent budget.`);
    const body = lines.join("\n");
    requireValue(Buffer.byteLength(body) < 32 * 1024, "rendered report exceeds bounded display size");
    return body;
}

function reportFromCheckpoint(cp, checkedHead, evidenceHashes) {
    const snapshot = structuredClone(cp.state);
    snapshot.publication = { digest: null, commentId: null };
    return {
        schemaVersion: 1, kind: "regression-investigation", state: snapshot,
        observations: Object.entries(cp.observations).filter(([, o]) => o.valid)
            .map(([digest, o]) => ({ digest, data: structuredClone(o.data),
                evidence: o.evidence.map(e => ({ ...e, available: evidenceHashes.get(e.url) === e.sha256 })) })),
        deadline: cp.deadline, inputDigest: cp.inputDigest, reproductionDigest: cp.reproductionDigest,
        policyDigest: cp.policyDigest, checkedHead,
    };
}

/**
 * deps.context is the full existing state context, including trusted record
 * identity, keys, owner, signing workflow revision and checkpoint anchor.
 * withLock(action) holds the per-issue controller lock across every read/write.
 * readCurrentContext({now,repository,issueNumber}) must freshly read trusted
 * manifest, authorization, policy, policyRevision, workflowRevision, pinnedMainSha
 * (current main HEAD), stableSdk, environmentFingerprint, and readSource({path,sha256}).
 * readSource enforces approved pinned tracked-blob/symlink policy. No caller
 * snapshot, report body, destination, intake replacement or authentication callback
 * is accepted. transport.request is real paginated intake's Octokit-shaped seam.
 * readEvidence must honor redirect:"error" and maxBytes; redirects are never
 * followed by this wrapper. Prepare updates only checkpoint/receipt, not a result.
 */
async function preparePublication(options, deps) {
    try {
        fields(options, "now generation");
        requireValue(typeof options.now === "string" && Number.isFinite(Date.parse(options.now)) &&
            Number.isSafeInteger(options.generation) && options.generation >= 0, "invalid publication request");
        requireValue(typeof deps.withLock === "function" && typeof deps.readCurrentContext === "function",
            "trusted current context reader and controller lock required");
        return await deps.withLock(async () => {
            const context = { ...deps.context, now: options.now };
            requireValue(context.repository === "dotnet/fsharp" && positive(context.issueNumber), "invalid selected issue");
            const evidenceHashes = new Map();
            const transport = { ...deps.transport, readEvidence: async url => {
                safeEvidenceUrl(url);
                const bytes = await deps.transport.readEvidence(url, { redirect: "error", maxBytes: 4 * 1024 * 1024 });
                requireValue(bytes === null || Buffer.isBuffer(bytes) && bytes.length <= 4 * 1024 * 1024,
                    "oversized or invalid evidence response");
                evidenceHashes.set(url, bytes === null ? null : hash(bytes));
                return bytes;
            } };
            const loaded = await state.loadCheckpoint(transport, context);
            const cp = loaded.checkpoint;
            const originalGeneration = cp.generation - (cp.publicationStatus === "pending" ? 1 : cp.publicationStatus === "published" ? 2 : 0);
            requireValue(options.generation === cp.generation || options.generation === originalGeneration ||
                cp.publicationStatus === "published" && options.generation === cp.generation - 1,
                "stale checkpoint generation");
            const stop = async (reason, status = "blocked") => {
                await state.recordPublication(transport, context, { generation: cp.generation,
                    status: "blocked", digest: cp.state.publication.digest, commentId: cp.state.publication.commentId });
                return { status, reason, budget: structuredClone(cp.state.budget.consumed) };
            };
            let fresh;
            try {
                fresh = await deps.readCurrentContext({ now: options.now, repository: context.repository, issueNumber: context.issueNumber });
                requireValue(fresh && typeof fresh.readSource === "function" && digest(fresh.environmentFingerprint),
                    "fresh approval sources and environment fingerprint required");
                model.validateManifest(fresh.manifest);
                model.validatePolicy(fresh.policy);
            } catch (error) { return stop(error.message); }
            const records = new Map(), checkpoints = new Map([[context.commentId, loaded]]);
            const authenticateRecord = async comment => {
                const decoded = await state.authenticateDecodedRecord(transport, { ...context, commentId: comment.id }, comment);
                if (!decoded) return false;
                records.set(comment.id, decoded);
                if (decoded.kind === "checkpoint" && !checkpoints.has(comment.id)) {
                    checkpoints.set(comment.id, await state.loadCheckpoint(transport, { ...context, commentId: comment.id }));
                }
                return true;
            };
            const snapshot = await readIssueSnapshot({
                repository: context.repository, issueNumber: context.issueNumber, transport, now: options.now,
                reviewedManifest: fresh.manifest, readSource: fresh.readSource, authenticateRecord,
                readTrustedCheckpoints: async () => [...checkpoints.values()].map(v => ({
                    status: "loaded", state: v.checkpoint.state, commentId: v.commentId, owner: v.owner, pending: v.checkpoint.pending,
                })),
            });
            if (snapshot.status !== "complete") return stop(snapshot.reason,
                snapshot.status === "ineligible" ? "blocked" : /stale|mismatch/.test(snapshot.reason) ? "requeue" : "blocked");
            const current = snapshot.snapshot;
            const mismatches = [
                ["discussion", current.discussionFingerprint, cp.state.issueFingerprint],
                ["manifest", fresh.manifest, cp.state.manifest], ["approval", fresh.authorization, cp.state.budget.authorization],
                ["policy", fresh.policy, cp.state.budget.policy], ["policy revision", fresh.policyRevision, cp.state.budget.policyRevision],
                ["workflow revision", fresh.workflowRevision, cp.state.budget.workflowRevision],
                ["main head", fresh.pinnedMainSha, cp.state.pinnedMainSha], ["stable SDK", fresh.stableSdk, cp.state.budget.stableSdk],
                ["environment", fresh.environmentFingerprint, cp.state.budget.environmentFingerprint],
            ].filter(([, actual, expected]) => actual === undefined || expected === undefined || !equal(actual, expected)).map(([name]) => name);
            if (mismatches.length) return stop(`stale ${mismatches.join(", ")}; requeue with spent allowance preserved`, "requeue");
            if (!loaded.publicationAllowed) return stop(loaded.blockers.join("; ") || "publication is stopped");
            if (current.investigations.active.some(active => active.commentId !== context.commentId ||
                active.owner?.runId !== context.owner.runId || active.owner?.attempt !== context.owner.attempt)) {
                return stop("another active investigation owns this issue");
            }
            if (cp.pending || !["complete", "blocked"].includes(cp.state.phase)) {
                return stop("work is pending or search has not reached a reportable state");
            }
            const report = reportFromCheckpoint(cp, fresh.pinnedMainSha, evidenceHashes);
            let body;
            try { body = renderReport(report); } catch (error) { return stop(error.message); }
            const resultDigest = fingerprint(report);
            // Enforce the combined human report + signed envelope limit before issuing an intent.
            state.serializeWorkflowRecord({ kind: "result", status: "Regression investigation result",
                payload: { schemaVersion: 1, digest: resultDigest, report } }, context);
            const matches = [...records.values()].filter(r => r.kind === "result" &&
                r.payload.digest === resultDigest && equal(r.payload.report, report));
            if (matches.length > 1) return stop("conflicting duplicate authenticated results");
            if (cp.state.publication.digest && cp.state.publication.digest !== resultDigest) return stop("pending publication digest changed", "requeue");
            if (matches.length) {
                requireValue(cp.state.publication.digest === resultDigest, "result has no matching publication intent");
                const saved = await state.recordPublication(transport, context, { generation: cp.generation,
                    status: "published", digest: resultDigest, commentId: matches[0].commentId });
                return { status: "published", digest: resultDigest, commentId: matches[0].commentId, generation: saved.generation };
            }
            if (cp.publicationStatus === "published") return stop("authenticated published result is missing");
            const duplicate = cp.publicationStatus === "pending";
            const saved = await state.recordPublication(transport, context, { generation: cp.generation,
                status: "pending", digest: resultDigest, commentId: null });
            return { status: duplicate ? "pending" : "ready", repository: cp.state.repository, issueNumber: cp.state.issueNumber,
                digest: resultDigest, generation: saved.generation, report, body };
        });
    } catch (error) { return { status: "blocked", reason: error.message }; }
}

async function completePublication(options, deps) {
    try {
        fields(options, "now generation digest commentId");
        requireValue(digest(options.digest) && positive(options.commentId), "invalid publication completion");
        const result = await preparePublication({ now: options.now, generation: options.generation }, deps);
        if (result.status === "published") requireValue(result.digest === options.digest &&
            result.commentId === options.commentId, "publication completion identity mismatch");
        return result;
    } catch (error) { return { status: "blocked", reason: error.message }; }
}

module.exports = { renderReport, validateReport, preparePublication, completePublication };
