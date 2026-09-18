const { createHash, createHmac, timingSafeEqual } = require("node:crypto");
const {
    isEligibleIssue, validateManifest, validatePolicy, validateState, validateObservation,
    fingerprintInput: fingerprint, fingerprintEpoch,
} = require("./model.cjs");

const MAX_BYTES = 48 * 1024;
const counterNames = ["sourceBuilds", "workerUnits", "reservedMinutes"];
class StateError extends Error {}
const requireState = (condition, message) => { if (!condition) throw new StateError(message); };
const equal = (a, b) => fingerprint(a) === fingerprint(b);
const digest = value => typeof value === "string" && /^[a-f0-9]{64}$/.test(value);
const positive = value => Number.isSafeInteger(value) && value > 0;
const timestamp = value => typeof value === "string" && /^\d{4}-\d\d-\d\dT.*Z$/.test(value) && Number.isFinite(Date.parse(value));
const hashBytes = bytes => createHash("sha256").update(bytes).digest("hex");
const ownerEqual = (a, b) => a?.runId === b?.runId && a?.attempt === b?.attempt;

function fields(value, names, description) {
    requireState(value && Object.getPrototypeOf(value) === Object.prototype &&
        equal(Object.keys(value).sort(), names.split(" ").sort()), `invalid ${description} schema`);
}

// Intern objects, arrays and long strings, including repeated assertion streams.
// References are tagged arrays; original arrays exist only as table entries.
function pack(value) {
    const table = [], seen = new Map();
    function encode(v) {
        if (v === null || (typeof v !== "object" && !(typeof v === "string" && v.length >= 128))) return v;
        const key = fingerprint(v);
        if (seen.has(key)) return ["@", seen.get(key)];
        const index = table.length;
        seen.set(key, index);
        table.push(null);
        table[index] = typeof v === "string" ? v : Array.isArray(v) ? v.map(encode) :
            Object.fromEntries(Object.entries(v).map(([k, child]) => [k, encode(child)]));
        return ["@", index];
    }
    return { root: encode(value), table };
}

function unpack(packed) {
    fields(packed, "root table", "identity table");
    requireState(Array.isArray(packed.table), "invalid identity table");
    let visits = 0;
    const active = new Set();
    function decode(value, depth = 0) {
        requireState(++visits <= 100000 && depth <= 64, "oversized expanded checkpoint");
        if (!Array.isArray(value)) {
            requireState(value === null || typeof value !== "object", "invalid identity reference");
            return value;
        }
        requireState(value.length === 2 && value[0] === "@" && Number.isSafeInteger(value[1]) &&
            value[1] >= 0 && value[1] < packed.table.length && !active.has(value[1]), "invalid identity reference");
        active.add(value[1]);
        const node = packed.table[value[1]];
        requireState(node !== null && (typeof node === "object" || typeof node === "string"), "invalid identity entry");
        const result = typeof node === "string" ? node : Array.isArray(node) ? node.map(v => decode(v, depth + 1)) :
            Object.fromEntries(Object.entries(node).map(([k, v]) => [k, decode(v, depth + 1)]));
        active.delete(value[1]);
        return result;
    }
    return decode(packed.root);
}

function render(envelope) {
    return `${envelope.status}\n\n\`\`\`json\n${JSON.stringify(envelope)}\n\`\`\``;
}

function serializeWorkflowRecord({ kind, status, payload }, context) {
    requireState(["checkpoint", "proposal", "result"].includes(kind) &&
        typeof status === "string" && status.length <= 256 && !/[\r\n`]/.test(status), "invalid record status");
    const key = context.keys[context.keyId];
    requireState(Buffer.isBuffer(key) && key.length >= 32, "authentication key must contain at least 32 bytes");
    const envelope = {
        schemaVersion: 1, kind, status,
        binding: {
            repository: context.repository, issueNumber: context.issueNumber, commentId: context.commentId,
            workflowPath: context.workflowPath, workflowRef: context.workflowRef,
            workflowRevision: context.workflowRevision, runId: context.owner.runId,
            runAttempt: context.owner.attempt, keyId: context.keyId,
        },
        payload: pack(payload),
    };
    envelope.signature = createHmac("sha256", key).update(fingerprint(envelope)).digest("hex");
    const body = render(envelope);
    requireState(Buffer.byteLength(body) < MAX_BYTES, "checkpoint exceeds 48 KiB size limit; execution blocked");
    return body;
}

function parseRecord(body) {
    if (typeof body !== "string" || Buffer.byteLength(body) >= MAX_BYTES) return null;
    const match = /^[^\r\n`]*\n\n```json\n([^\r\n]+)\n```$/.exec(body);
    if (!match) return null;
    try { return JSON.parse(match[1]); } catch (error) {
        if (error instanceof SyntaxError) return null;
        throw error;
    }
}

async function verifyRun(transport, context, binding) {
    const run = await transport.getRun(binding.runId, binding.runAttempt);
    requireState(String(run.id) === binding.runId && run.run_attempt === binding.runAttempt &&
        run.repository?.full_name === context.repository && run.head_repository?.full_name === context.repository &&
        run.path === context.workflowPath && `refs/heads/${run.head_branch}` === context.workflowRef &&
        run.head_sha === binding.workflowRevision && context.trustedWorkflowRevisions.includes(run.head_sha),
    "workflow run provenance authentication failed");
    return run;
}

async function authenticatedRecord(transport, context, comment) {
    const record = parseRecord(comment?.body);
    if (!record || record.schemaVersion !== 1 || !["checkpoint", "proposal", "result"].includes(record.kind) ||
        typeof record.status !== "string" || !digest(record.signature) ||
        !equal(Object.keys(record).sort(), ["binding", "kind", "payload", "schemaVersion", "signature", "status"])) return null;
    const b = record.binding;
    if (!b || !equal(Object.keys(b).sort(), [
        "commentId", "issueNumber", "keyId", "repository", "runAttempt", "runId",
        "workflowPath", "workflowRef", "workflowRevision",
    ])) return null;
    if (comment.id !== context.commentId || b.commentId !== context.commentId ||
        b.issueNumber !== context.issueNumber || b.repository !== context.repository ||
        b.workflowPath !== context.workflowPath || b.workflowRef !== context.workflowRef ||
        !context.trustedWorkflowRevisions.includes(b.workflowRevision) ||
        !/^\d+$/.test(b.runId) || !positive(b.runAttempt) ||
        comment.issue_url !== `https://api.github.com/repos/${context.repository}/issues/${context.issueNumber}` ||
        comment.user?.id !== context.actor.id || comment.user?.login !== context.actor.login ||
        comment.user?.type !== context.actor.type || comment.performed_via_github_app?.id !== context.actor.appId) return null;
    const key = context.keys[b.keyId];
    if (!Buffer.isBuffer(key) || key.length < 32) return null;
    const { signature, ...signed } = record;
    const expected = createHmac("sha256", key).update(fingerprint(signed)).digest();
    if (!timingSafeEqual(Buffer.from(signature, "hex"), expected) || render(record) !== comment.body) return null;
    await verifyRun(transport, context, b);
    return record;
}

async function authenticateWorkflowRecord(transport, context, comment) {
    return (await authenticatedRecord(transport, context, comment)) !== null;
}

function validateEvidence(evidence) {
    requireState(Array.isArray(evidence) && evidence.length <= 16, "invalid evidence list");
    for (const item of evidence) {
        fields(item, "required sha256 url", "external evidence");
        let url;
        try { url = new URL(item.url); } catch (error) {
            if (!(error instanceof TypeError)) throw error;
            throw new StateError("invalid evidence URL");
        }
        requireState(typeof item.required === "boolean" && digest(item.sha256) &&
            url.protocol === "https:" && !url.username && !url.password && !url.hash, "invalid external evidence");
    }
}

function validateCheckpoint(cp) {
    fields(cp, "schemaVersion generation previousDigest state inputDigest reproductionDigest policyDigest sequence pending records observations history deadline publicationStatus", "checkpoint");
    requireState(cp.schemaVersion === 1, "unsupported checkpoint schema");
    validateState(cp.state);
    requireState(Number.isSafeInteger(cp.generation) && cp.generation >= 0 && digest(cp.previousDigest) && timestamp(cp.deadline) &&
        cp.inputDigest === cp.state.manifest.approvedInputDigest &&
        cp.reproductionDigest === reproductionDigest(cp.state.manifest) &&
        cp.policyDigest === fingerprint(cp.state.budget.policy), "checkpoint digest or generation mismatch");
    requireState(Array.isArray(cp.records) && cp.records.length <= cp.state.budget.policy.limits.workerUnits &&
        cp.sequence === cp.records.length && Array.isArray(cp.history) &&
        ["unpublished", "pending", "published", "blocked"].includes(cp.publicationStatus), "invalid checkpoint ledger");
    const totals = { sourceBuilds: 0, workerUnits: 0, reservedMinutes: 0 };
    const ids = new Set();
    for (const [index, r] of cp.records.entries()) {
        fields(r, "id workItemId request requestDigest owner sequence attempt generation deadline inputDigest epoch reserved status observationDigest", "work record");
        requireState(typeof r.id === "string" && r.id.length > 0 && r.id.length <= 128 && !ids.has(r.id) &&
            r.sequence === index + 1 && positive(r.attempt) && positive(r.generation) &&
            timestamp(r.deadline) && digest(r.inputDigest) && digest(r.epoch) &&
            r.requestDigest === fingerprint(r.request) && digest(r.workItemId) &&
            ["reserved", "committed", "cancelled", "invalidated"].includes(r.status), "invalid work record identity");
        fields(r.owner, "runId attempt", "work owner");
        requireState(/^\d+$/.test(r.owner.runId) && positive(r.owner.attempt), "invalid work owner");
        fields(r.reserved, "sourceBuilds workerUnits reservedMinutes", "reservation counters");
        requireState(r.reserved.workerUnits === 1 && r.reserved.sourceBuilds === (r.request.kind === "source" ? 1 : 0) &&
            r.reserved.reservedMinutes === cp.state.budget.policy.limits.workerTimeoutMinutes, "invalid reservation charge");
        ids.add(r.id);
        for (const name of counterNames) totals[name] += r.reserved[name];
        requireState(r.status === "committed" ? digest(r.observationDigest) : r.observationDigest === null, "invalid observation identity");
    }
    requireState(equal(totals, cp.state.budget.consumed), "consumed budgets must equal the durable reservation ledger");
    const counts = new Map();
    for (const r of cp.records) {
        const count = (counts.get(r.workItemId) ?? 0) + 1;
        requireState(r.attempt === count, "nonsequential work attempt");
        counts.set(r.workItemId, count);
    }
    requireState(equal([...counts].map(([workItemId, count]) => ({ workItemId, count })), cp.state.budget.attempts),
        "attempt accounting mismatch");
    const reserved = cp.records.filter(r => r.status === "reserved");
    requireState(cp.pending === null ? reserved.length === 0 && cp.state.budget.pendingWork === null :
        reserved.length === 1 && equal(reserved[0], cp.pending) &&
        cp.state.budget.pendingWork?.id === cp.pending.id, "pending reservation mismatch");
    requireState(cp.observations && Object.getPrototypeOf(cp.observations) === Object.prototype, "invalid observation table");
    for (const [id, observation] of Object.entries(cp.observations)) {
        fields(observation, "data manifest attemptId evidence valid", "observation record");
        validateObservation(observation.data, observation.manifest);
        validateEvidence(observation.evidence);
        requireState(fingerprint(observation.data) === id && typeof observation.valid === "boolean" &&
            cp.records.some(r => r.id === observation.attemptId && r.observationDigest === id), "observation identity mismatch");
        if (observation.valid) requireState(observation.data.manifestDigest === cp.state.approvalFingerprint &&
            observation.data.provenance.workflowRevision === cp.state.budget.workflowRevision &&
            observation.data.provenance.policyRevision === cp.state.budget.policyRevision, "stale observation marked valid");
    }
    requireState(cp.records.every(r => r.status !== "committed" || cp.observations[r.observationDigest]?.attemptId === r.id),
        "missing durable observation");
    requireState(equal(Object.entries(cp.observations).filter(([, o]) => o.valid).map(([id]) => id),
        cp.state.budget.authenticatedObservationDigests), "authenticated observation table mismatch");
}

function reproductionDigest(manifest) {
    const { discussionFingerprint, approvedInputDigest, searchHints, ...reproduction } = manifest;
    return fingerprint(reproduction);
}

function currentState(cp, context) {
    const reasons = [];
    if (!isEligibleIssue(context.issue) || context.issue.number !== context.issueNumber) reasons.push("issue not eligible");
    let state = null;
    try {
        validatePolicy(context.policy);
        validateManifest(context.manifest);
        requireState(context.repository === "dotnet/fsharp" && context.manifest.issueNumber === context.issueNumber &&
            context.issueFingerprint === context.manifest.discussionFingerprint, "approval does not match complete issue snapshot");
        const budget = {
            ...cp.state.budget, policy: context.policy, policyRevision: context.policyRevision,
            workflowRevision: context.workflowRevision, authorization: context.authorization, stableSdk: context.stableSdk,
        };
        state = {
            ...cp.state, manifest: context.manifest, budget,
            issueFingerprint: context.issueFingerprint, approvalFingerprint: fingerprint(context.manifest),
            pinnedMainSha: context.pinnedMainSha,
            epoch: fingerprintEpoch(context.manifest, budget, context.pinnedMainSha),
        };
    } catch (error) {
        if (!(error instanceof StateError) && error.constructor.name !== "ContractError") throw error;
        reasons.push(`approval blocked: ${error.message}`);
    }
    const stale = state !== null && state.epoch !== cp.state.epoch;
    if (stale) reasons.push("stale inputs or current endpoints");
    if (Date.parse(context.now) >= Date.parse(cp.deadline)) reasons.push("allowance deadline exhausted");
    requireState(timestamp(context.now), "invalid controller time");
    return { state, reasons, stale };
}

async function evidenceWarnings(transport, observations) {
    const warnings = [];
    let missingRequired = false;
    for (const record of observations) {
        for (const evidence of record.evidence) {
            // The transport returns null only for confirmed unavailable/expired content.
            // API/authentication errors propagate, rather than becoming missing evidence.
            const bytes = await transport.readEvidence(evidence.url);
            requireState(bytes === null || Buffer.isBuffer(bytes), "evidence transport must return bytes or null");
            if (bytes === null || hashBytes(bytes) !== evidence.sha256) {
                warnings.push(`${evidence.required ? "required" : "optional"} missing or expired evidence: ${evidence.url}`);
                if (evidence.required) missingRequired = true;
            }
        }
    }
    return { warnings, missingRequired };
}

function freshnessStatus(context, generation, bodyDigest, keyId = context.keyId) {
    requireState(context.repository === "dotnet/fsharp" && positive(context.issueNumber) && positive(context.commentId) &&
        /^[a-f0-9]{40}$/.test(context.checkpointAnchorSha), "invalid trusted freshness anchor");
    const key = context.keys[keyId];
    requireState(/^[\w-]{1,64}$/.test(keyId) && Buffer.isBuffer(key) && key.length >= 32, "freshness authentication key unavailable");
    const name = `regression-bisect/${context.issueNumber}/${context.commentId}`;
    const description = `checkpoint:${generation}:${bodyDigest}`;
    const url = `https://github.com/${context.repository}/issues/${context.issueNumber}#checkpoint-${keyId}:`;
    const signature = createHmac("sha256", key).update(fingerprint({
        anchor: context.checkpointAnchorSha, name, description, url,
    })).digest("hex");
    return { state: "success", context: name, description, target_url: url + signature };
}

async function readFreshness(transport, context) {
    const expected = freshnessStatus(context, 0, "");
    let head = null;
    for (let page = 1; page <= 10; page++) {
        const response = await transport.request("GET /repos/{owner}/{repo}/commits/{ref}/statuses", {
            owner: "dotnet", repo: "fsharp", ref: context.checkpointAnchorSha, per_page: 100, page,
        });
        requireState(response && (!response.status || response.status === 200) &&
            Array.isArray(response.data) && response.data.length <= 100, "incomplete freshness response");
        for (const status of response.data) {
            if (status.context !== expected.context || status.creator?.id !== context.actor.id) continue;
            const parts = /^checkpoint:(0|[1-9]\d*):([a-f0-9]{64})$/.exec(status.description);
            const key = /#checkpoint-([\w-]{1,64}):[a-f0-9]{64}$/.exec(status.target_url);
            requireState(parts && key && Number.isSafeInteger(Number(parts[1])) &&
                positive(status.id) && status.creator.login === context.actor.login &&
                status.creator.type === context.actor.type, "freshness receipt authentication failed");
            const receipt = { generation: Number(parts[1]), bodyDigest: parts[2] };
            const signed = freshnessStatus(context, receipt.generation, receipt.bodyDigest, key[1]);
            requireState(Object.entries(signed).every(([k, v]) => status[k] === v), "freshness receipt authentication failed");
            requireState(head?.generation !== receipt.generation || head.bodyDigest === receipt.bodyDigest,
                "conflicting freshness receipts");
            if (head === null || receipt.generation > head.generation) head = receipt;
        }
        const next = response.headers?.link === undefined ? response.data.length === 100 :
            /;\s*rel="?next"?/.test(response.headers.link);
        if (!next) return head;
    }
    throw new StateError("freshness page limit exceeded; history incomplete");
}

async function writeFreshness(transport, context, generation, bodyDigest) {
    const run = await verifyRun(transport, context, {
        runId: context.owner.runId, runAttempt: context.owner.attempt, workflowRevision: context.workflowRevision,
    });
    requireState(run.status === "in_progress", "freshness writer must be the active owning run");
    const status = freshnessStatus(context, generation, bodyDigest);
    const response = await transport.request("POST /repos/{owner}/{repo}/statuses/{sha}", {
        owner: "dotnet", repo: "fsharp", sha: context.checkpointAnchorSha, ...status,
    });
    const written = response?.data;
    requireState((!response?.status || response.status === 201) && positive(written?.id) &&
        written.creator?.id === context.actor.id && written.creator?.login === context.actor.login &&
        written.creator?.type === context.actor.type &&
        Object.entries(status).every(([k, v]) => written[k] === v), "freshness write authentication failed");
}

async function loadCheckpoint(transport, context) {
    const comment = await transport.getComment(context.commentId);
    const record = await authenticatedRecord(transport, context, comment);
    requireState(record?.kind === "checkpoint", "checkpoint authentication failed");
    const checkpoint = unpack(record.payload);
    validateCheckpoint(checkpoint);
    requireState(checkpoint.state.repository === context.repository && checkpoint.state.issueNumber === context.issueNumber,
        "checkpoint issue authentication failed");
    const head = await readFreshness(transport, context);
    const bodyDigest = hashBytes(comment.body);
    requireState(head !== null, "checkpoint freshness receipt missing; initialization or repair required");
    if (checkpoint.generation !== head.generation || bodyDigest !== head.bodyDigest) {
        requireState(checkpoint.generation === head.generation + 1 && checkpoint.previousDigest === head.bodyDigest,
            "checkpoint freshness rollback or predecessor mismatch");
        // Complete an interrupted comment -> receipt write, under the controller lock.
        await writeFreshness(transport, context, checkpoint.generation, bodyDigest);
    }
    const current = currentState(checkpoint, context);
    const evidence = await evidenceWarnings(transport, Object.values(checkpoint.observations).filter(o => o.valid));
    const blockers = [...current.reasons, ...(evidence.missingRequired ? ["required evidence unavailable; blocked"] : [])];
    return {
        checkpoint, bodyDigest, commentId: context.commentId,
        owner: { runId: record.binding.runId, attempt: record.binding.runAttempt },
        executionAllowed: blockers.length === 0,
        publicationAllowed: blockers.length === 0 && checkpoint.state.phase !== "stopped",
        blockers, warnings: evidence.warnings,
    };
}

async function saveCheckpoint(transport, context, checkpoint, previousDigest = checkpoint.previousDigest) {
    checkpoint.previousDigest = previousDigest;
    validateCheckpoint(checkpoint);
    const owner = await verifyRun(transport, context, {
        runId: context.owner.runId, runAttempt: context.owner.attempt, workflowRevision: context.workflowRevision,
    });
    requireState(owner.status === "in_progress", "checkpoint writer must be the active owning run");
    const body = serializeWorkflowRecord({
        kind: "checkpoint", status: `Regression investigation: ${checkpoint.state.phase} (generation ${checkpoint.generation}).`,
        payload: checkpoint,
    }, context);
    const written = await transport.updateComment(context.commentId, body);
    requireState(written?.body === body && await authenticateWorkflowRecord(transport, context, written),
        "checkpoint write authentication failed");
    await writeFreshness(transport, context, checkpoint.generation, hashBytes(body));
    return checkpoint;
}

async function initializeCheckpoint(transport, context, { allowanceId, deadline }) {
    requireState(await readFreshness(transport, context) === null, "initialization forbidden: freshness history already exists");
    const existing = await transport.getComment(context.commentId);
    requireState(digest(context.bootstrapDigest), "initialization requires the trusted bootstrap digest");
    requireState(existing.id === context.commentId && existing.user?.id === context.actor.id &&
        existing.user?.login === context.actor.login && existing.user?.type === context.actor.type &&
        existing.performed_via_github_app?.id === context.actor.appId &&
        existing.issue_url === `https://api.github.com/repos/${context.repository}/issues/${context.issueNumber}`,
    "bootstrap author authentication failed");
    requireState(timestamp(deadline) && Date.parse(deadline) > Date.parse(context.now), "invalid allowance deadline");
    const budget = {
        schemaVersion: 1, policy: context.policy, policyRevision: context.policyRevision, workflowRevision: context.workflowRevision,
        allowanceId, authorization: context.authorization, stableSdk: context.stableSdk,
        authenticatedObservationDigests: [], consumed: { sourceBuilds: 0, workerUnits: 0, reservedMinutes: 0 },
        attempts: [], nextWorkItemId: "none", pendingWork: null,
    };
    const state = {
        schemaVersion: 1, repository: context.repository, issueNumber: context.issueNumber,
        issueFingerprint: context.issueFingerprint, approvalFingerprint: fingerprint(context.manifest),
        manifest: context.manifest, budget, pinnedMainSha: context.pinnedMainSha,
        epoch: fingerprintEpoch(context.manifest, budget, context.pinnedMainSha), phase: "discovery",
        endpointEvidence: { stable: null, main: null, good: null }, testedRevisions: [], skips: [], interval: null,
        publication: { digest: null, commentId: null },
    };
    const cp = {
        schemaVersion: 1, generation: 0, previousDigest: context.bootstrapDigest, state, inputDigest: context.manifest.approvedInputDigest,
        reproductionDigest: reproductionDigest(context.manifest), policyDigest: fingerprint(context.policy),
        sequence: 0, pending: null, records: [], observations: {}, history: [], deadline, publicationStatus: "unpublished",
    };
    requireState(currentState(cp, context).reasons.length === 0, "initialization blocked by current eligibility or approval");
    if (fingerprint(existing.body) !== context.bootstrapDigest || parseRecord(existing.body) !== null) {
        const record = await authenticatedRecord(transport, context, existing);
        requireState(record?.kind === "checkpoint" && equal(unpack(record.payload), cp),
            "initialization requires the exact trusted bootstrap placeholder or interrupted initial checkpoint");
        await writeFreshness(transport, context, 0, hashBytes(existing.body));
        return cp;
    }
    return saveCheckpoint(transport, context, structuredClone(cp));
}

function checkGeneration(cp, generation) {
    requireState(cp.generation === generation, "stale checkpoint generation; controller lock required");
}

async function retirePending(transport, context, cp, status) {
    if (!cp.pending) return;
    const run = await verifyRun(transport, context, {
        runId: cp.pending.owner.runId, runAttempt: cp.pending.owner.attempt,
        workflowRevision: cp.state.budget.workflowRevision,
    });
    requireState(run.status === "completed" && typeof run.conclusion === "string", "pending work has an active or unknown owning run");
    cp.records.find(r => r.id === cp.pending.id).status = status;
    cp.pending = null;
    cp.state.budget.pendingWork = null;
}

function validateRequest(request, state) {
    fields(request, "kind target endpoint phase", "work request");
    requireState(["source", "released"].includes(request.kind) &&
        [null, "stable", "main", "good"].includes(request.endpoint) &&
        ["stable", "main", "good-search", "admission", "bisect", "confirmation"].includes(request.phase), "invalid work request");
    fields(request.target, request.kind === "source" ? "sourceSha" : "sdk", "work target");
    requireState(request.kind === "source" ? /^[a-f0-9]{40}$/.test(request.target.sourceSha) :
        equal(request.target.sdk, state.budget.stableSdk), "invalid work target identity");
    requireState(request.endpoint !== "stable" || request.kind === "released", "stable requires released identity");
    requireState(!["main", "good"].includes(request.endpoint) || request.kind === "source", "source endpoint requires source identity");
    requireState(request.endpoint !== "main" || request.target.sourceSha === state.pinnedMainSha, "main request identity mismatch");
}

async function reserveWork(transport, context, { generation, attemptId, request }) {
    const loaded = await loadCheckpoint(transport, context);
    requireState(loaded.executionAllowed, `execution blocked: ${loaded.blockers.join("; ")}`);
    const cp = loaded.checkpoint;
    validateRequest(request, cp.state);
    requireState(typeof attemptId === "string" && attemptId.length > 0 && attemptId.length <= 128, "invalid attempt identity");
    const previous = cp.records.find(r => r.id === attemptId);
    if (previous) {
        requireState(ownerEqual(previous.owner, context.owner) && previous.requestDigest === fingerprint(request),
            "attempt identity reuse conflict");
        requireState(previous.status === "reserved" && cp.pending?.id === attemptId, "attempt already committed, cancelled or invalidated");
        requireState(generation === previous.generation - 1 || generation === cp.generation, "stale checkpoint generation");
        return { ...structuredClone(previous), execute: false };
    }
    checkGeneration(cp, generation);
    const budget = cp.state.budget;
    const workItemId = fingerprint({ kind: request.kind, target: request.target, inputDigest: cp.inputDigest });
    const attempt = (budget.attempts.find(a => a.workItemId === workItemId)?.count ?? 0) + 1;
    requireState(attempt <= budget.policy.limits.attemptsPerItem, "work item attempt budget exhausted");
    const reserved = { sourceBuilds: request.kind === "source" ? 1 : 0, workerUnits: 1, reservedMinutes: budget.policy.limits.workerTimeoutMinutes };
    requireState(counterNames.every(name => budget.consumed[name] + reserved[name] <= budget.policy.limits[name]), "cumulative budget exhausted");
    const deadline = new Date(Date.parse(context.now) + reserved.reservedMinutes * 60000).toISOString();
    requireState(Date.parse(deadline) <= Date.parse(cp.deadline), "insufficient time before allowance deadline");
    await retirePending(transport, context, cp, "cancelled");
    for (const name of counterNames) budget.consumed[name] += reserved[name];
    const counter = budget.attempts.find(a => a.workItemId === workItemId);
    if (counter) counter.count = attempt;
    else budget.attempts.push({ workItemId, count: attempt });
    cp.generation++;
    cp.sequence++;
    const pending = {
        id: attemptId, workItemId, request: structuredClone(request), requestDigest: fingerprint(request),
        owner: structuredClone(context.owner), sequence: cp.sequence, attempt, generation: cp.generation,
        deadline, inputDigest: cp.inputDigest, epoch: cp.state.epoch, reserved, status: "reserved", observationDigest: null,
    };
    cp.pending = pending;
    cp.records.push(pending);
    budget.nextWorkItemId = workItemId;
    budget.pendingWork = { id: attemptId, allowanceId: budget.allowanceId, workItemId, attempt, reserved, status: "reserved" };
    cp.state.phase = request.phase;
    await saveCheckpoint(transport, context, cp, loaded.bodyDigest);
    return { ...structuredClone(pending), execute: true };
}

async function commitObservation(transport, context, { generation, attemptId, observation, evidence }) {
    const loaded = await loadCheckpoint(transport, context);
    const cp = loaded.checkpoint;
    const record = cp.records.find(r => r.id === attemptId);
    requireState(record && ownerEqual(record.owner, context.owner), "observation owner mismatch");
    const observationDigest = fingerprint(observation);
    validateEvidence(evidence);
    if (record.status === "committed") {
        requireState(generation === record.generation, "stale completion generation");
        requireState(record.observationDigest === observationDigest &&
            equal(cp.observations[observationDigest].evidence, evidence), "observation identity conflict");
        return cp;
    }
    checkGeneration(cp, generation);
    requireState(loaded.executionAllowed, `completion blocked: ${loaded.blockers.join("; ")}`);
    requireState(cp.pending?.id === attemptId && record.status === "reserved", "cancelled or invalidated work");
    requireState(Date.parse(context.now) <= Date.parse(record.deadline), "observation arrived after reservation deadline");
    validateObservation(observation, cp.state.manifest);
    requireState(observation.kind === record.request.kind && equal(observation.target, record.request.target) &&
        observation.buildInstanceId === record.id && !observation.cached &&
        observation.provenance.runId === record.owner.runId &&
        observation.provenance.workflowRevision === cp.state.budget.workflowRevision &&
        observation.provenance.policyRevision === cp.state.budget.policyRevision, "observation request or build identity mismatch");
    const availability = await evidenceWarnings(transport, [{ evidence }]);
    requireState(!availability.missingRequired, "required evidence missing or digest mismatch");
    cp.observations[observationDigest] = {
        data: structuredClone(observation), manifest: structuredClone(cp.state.manifest),
        attemptId, evidence: structuredClone(evidence), valid: true,
    };
    record.status = "committed";
    record.observationDigest = observationDigest;
    cp.state.budget.authenticatedObservationDigests.push(observationDigest);
    if (record.request.endpoint !== null) cp.state.endpointEvidence[record.request.endpoint] = observationDigest;
    if (observation.kind === "source") {
        const tested = { sourceSha: observation.target.sourceSha, observationDigest };
        cp.state.testedRevisions.push(tested);
        if (observation.outcome.outcome === "untestable") cp.state.skips.push({ ...tested, reason: observation.outcome.reason });
    }
    cp.pending = null;
    cp.state.budget.pendingWork = null;
    cp.generation++;
    return saveCheckpoint(transport, context, cp, loaded.bodyDigest);
}

async function invalidateStaleEvidence(transport, context, { generation }) {
    const loaded = await loadCheckpoint(transport, context);
    const cp = loaded.checkpoint;
    checkGeneration(cp, generation);
    const current = currentState(cp, context);
    const eligible = isEligibleIssue(context.issue) && context.issue.number === context.issueNumber;
    if (!current.stale && current.state !== null && eligible && cp.state.phase !== "stopped") return cp;
    const phase = !eligible ? "stopped" : current.state === null ? "blocked" : "discovery";
    if (!current.stale && cp.pending === null && cp.state.phase === phase &&
        (current.state !== null || Object.values(cp.observations).every(o => !o.valid))) return cp;
    await retirePending(transport, context, cp, "invalidated");
    const old = cp.state;
    if (current.stale || current.state === null) {
        const sameInputs = current.state !== null && old.approvalFingerprint === current.state.approvalFingerprint &&
            old.budget.workflowRevision === current.state.budget.workflowRevision &&
            old.budget.policyRevision === current.state.budget.policyRevision;
        cp.history.push({
            epoch: old.epoch, inputDigest: cp.inputDigest, approvalDigest: old.approvalFingerprint,
            policyDigest: cp.policyDigest, workflowRevision: old.budget.workflowRevision,
            pinnedMainSha: old.pinnedMainSha, stableSdk: old.budget.stableSdk,
            endpointEvidence: old.endpointEvidence, interval: old.interval, publication: old.publication,
            reason: current.state === null ? "current approval unavailable" :
                sameInputs ? "current endpoints changed" : "approved inputs changed",
        });
        cp.state = structuredClone(current.state ?? old);
        cp.state.budget.pendingWork = null;
        for (const observation of Object.values(cp.observations)) observation.valid &&= sameInputs;
        cp.state.budget.authenticatedObservationDigests = Object.entries(cp.observations).filter(([, o]) => o.valid).map(([id]) => id);
        cp.state.endpointEvidence = {
            stable: sameInputs && equal(old.budget.stableSdk, cp.state.budget.stableSdk) ? old.endpointEvidence.stable : null,
            main: sameInputs && old.pinnedMainSha === cp.state.pinnedMainSha ? old.endpointEvidence.main : null,
            good: sameInputs ? old.endpointEvidence.good : null,
        };
        if (!sameInputs) { cp.state.testedRevisions = []; cp.state.skips = []; }
        cp.state.interval = null;
        cp.inputDigest = cp.state.manifest.approvedInputDigest;
        cp.reproductionDigest = reproductionDigest(cp.state.manifest);
        cp.policyDigest = fingerprint(cp.state.budget.policy);
        cp.state.publication = { digest: null, commentId: null };
    }
    cp.state.phase = phase;
    cp.publicationStatus = !eligible || current.state === null ? "blocked" :
        cp.state.publication.commentId !== null ? "published" : "unpublished";
    cp.generation++;
    return saveCheckpoint(transport, context, cp, loaded.bodyDigest);
}

module.exports = {
    initializeCheckpoint, serializeWorkflowRecord, authenticateWorkflowRecord,
    loadCheckpoint, reserveWork, commitObservation, invalidateStaleEvidence,
};
