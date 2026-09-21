const { discoverEligibleIssues, readIssueSnapshot } = require("./intake.cjs");
const { fingerprintInput: fingerprint, validateManifest, validatePolicy } = require("./model.cjs");
const { requireValue, hash } = require("./toolchain-io.cjs");
const state = require("./state.cjs");

const repository = "dotnet/fsharp";
const workflowPath = ".github/workflows/regression-bisect-discovery.yml";
const controllerWorkflow = ".github/workflows/regression-bisect-controller.yml";
const policy = require("../../.github/policies/regression-bisect.json");
const positive = n => Number.isSafeInteger(n) && n > 0;
const digest = s => typeof s === "string" && /^[a-f0-9]{64}$/.test(s);
const fields = (value, names) => requireValue(value && Object.getPrototypeOf(value) === Object.prototype &&
    Object.keys(value).sort().join(" ") === names.split(" ").sort().join(" "), "invalid routing schema");
const decode = bytes => JSON.parse(new TextDecoder("utf-8", { fatal: true }).decode(bytes));
const same = (a, b) => fingerprint(a) === fingerprint(b);
const query = { repository, ...policy.selection, sort: "created", direction: "asc", perPage: 100 };

function validateRegistration(r) {
    fields(r, "schemaVersion repository issueNumber commentId checkpointAnchorSha bootstrapDigest provisionDeadline workflowPath workflowGeneration");
    requireValue(r.schemaVersion === 1 && r.repository === repository && positive(r.issueNumber) && positive(r.commentId) &&
        /^[a-f0-9]{40}$/.test(r.checkpointAnchorSha) && digest(r.bootstrapDigest) &&
        typeof r.provisionDeadline === "string" && /^\d{4}-\d\d-\d\dT.*Z$/.test(r.provisionDeadline) &&
        Number.isFinite(Date.parse(r.provisionDeadline)) && r.workflowPath === workflowPath && positive(r.workflowGeneration),
    "invalid reviewed discovery registration");
    return r;
}

function dispatchKey({ issueNumber, allowanceId, generation, mode, discussionFingerprint, policyRevision }) {
    return fingerprint({ repository, controllerWorkflow, ref: "refs/heads/main",
        issueNumber, allowanceId, generation, mode, discussionFingerprint, policyRevision });
}

function validateCursor(cp, registration) {
    fields(cp, "schemaVersion purpose query policyDigest workflowGeneration generation previousDigest cursor selected");
    requireValue(cp.schemaVersion === 1 && cp.purpose === "discovery-routing" && same(cp.query, query) &&
        cp.policyDigest === fingerprint(policy) && cp.workflowGeneration === registration.workflowGeneration &&
        Number.isSafeInteger(cp.generation) && cp.generation >= 0 && digest(cp.previousDigest), "routing generation or query mismatch");
    fields(cp.cursor, "version page offset");
    requireValue(cp.cursor.version === 1 && positive(cp.cursor.page) && Number.isSafeInteger(cp.cursor.offset) &&
        cp.cursor.offset >= 0 && cp.cursor.offset < 100, "invalid authenticated discovery cursor");
    requireValue(Array.isArray(cp.selected) && cp.selected.length <= 3 &&
        new Set(cp.selected.map(s => s.issueNumber)).size === cp.selected.length, "invalid bounded selection");
    for (const item of cp.selected) {
        fields(item, "issueNumber status key discussionFingerprint reason dispatch retryAfter acknowledged");
        requireValue(positive(item.issueNumber) && ["pending", "proposal", "blocked", "complete", "ineligible"].includes(item.status) &&
            digest(item.key) && (item.discussionFingerprint === null || digest(item.discussionFingerprint)) &&
            typeof item.reason === "string" && item.reason.length <= 256 && typeof item.acknowledged === "boolean" &&
            Number.isSafeInteger(item.retryAfter) && item.retryAfter >= 0, "invalid routing intent");
        if (item.dispatch !== null) {
            fields(item.dispatch, "issue mode routingGeneration routingKey");
            requireValue(item.dispatch.issue === item.issueNumber && item.dispatch.routingKey === item.key &&
                ["bootstrap", "advance", "publish"].includes(item.dispatch.mode) &&
                /^(?:-1|0|[1-9]\d*)$/.test(item.dispatch.routingGeneration) &&
                Number.isSafeInteger(Number(item.dispatch.routingGeneration)) &&
                (item.dispatch.mode === "bootstrap") === (item.dispatch.routingGeneration === "-1"), "invalid durable dispatch");
        }
        requireValue(item.status !== "pending" || item.dispatch !== null, "pending selection has no dispatch");
    }
}

async function loadCursor(deps, { repair = false } = {}) {
    const { transport, context, registration, journal } = deps;
    let committedGeneration;
    let comment = await transport.getComment(context.commentId);
    if (journal && repair) {
        const marker = await state.authenticateDecodedRecord(transport, context,
            { ...comment, body: await journal.read("committed") });
        if (marker?.kind === "proposal" && marker.payload.purpose === "discovery-provisioning") {
            await provisionDiscovery(deps);
            comment = await transport.getComment(context.commentId);
        }
    }
    let record = await state.authenticateDecodedRecord(transport, context, comment);
    requireValue(record?.kind === "proposal", "discovery cursor authentication failed");
    validateCursor(record.payload, registration);
    if (journal) {
        const saved = {};
        for (const name of ["intent", "committed"]) {
            const body = await journal.read(name);
            const decoded = await state.authenticateDecodedRecord(transport, context, { ...comment, body });
            requireValue(decoded?.kind === "proposal", `required discovery ${name} authentication failed`);
            validateCursor(decoded.payload, registration);
            saved[name] = { body, cp: decoded.payload };
        }
        committedGeneration = saved.committed.cp.generation;
        requireValue(record.payload.generation >= saved.committed.cp.generation &&
            (record.payload.generation !== saved.committed.cp.generation || comment.body === saved.committed.body),
        "discovery cursor rolled back behind durable history");
        if (comment.body !== saved.intent.body) {
            requireValue(repair && comment.body === saved.committed.body &&
                saved.intent.cp.generation === record.payload.generation + 1 &&
                saved.intent.cp.previousDigest === hash(comment.body), "discovery intent predecessor mismatch");
            const head = await state.readFreshness(transport, context);
            requireValue(head?.generation === record.payload.generation && head.bodyDigest === hash(comment.body),
                "required discovery freshness missing or rolled back");
            comment = await transport.updateComment(context.commentId, saved.intent.body);
            requireValue(comment.body === saved.intent.body, "discovery recovery write mismatch");
            record = await state.authenticateDecodedRecord(transport, context, comment);
            requireValue(record?.kind === "proposal", "discovery recovery authentication failed");
        }
    }
    const head = await state.readFreshness(transport, context), bodyDigest = hash(comment.body);
    requireValue(head !== null, "discovery freshness receipt missing; operator recovery required");
    requireValue(committedGeneration === undefined || head.generation >= committedGeneration,
        "discovery freshness rolled back behind durable history");
    if (record.payload.generation !== head.generation || bodyDigest !== head.bodyDigest) {
        requireValue(repair && record.payload.generation === head.generation + 1 && record.payload.previousDigest === head.bodyDigest,
            "discovery cursor rollback or torn predecessor");
        await state.writeFreshness(transport, context, record.payload.generation, bodyDigest);
    }
    if (journal && repair) await journal.write("committed", comment.body);
    return { cursor: record.payload, bodyDigest };
}

async function saveCursor(deps, cp, previousDigest) {
    cp.previousDigest = previousDigest;
    validateCursor(cp, deps.registration);
    const body = state.serializeWorkflowRecord({
        kind: "proposal", status: "Regression discovery routing", payload: cp,
    }, deps.context);
    if (deps.journal) await deps.journal.write("intent", body);
    const written = await deps.transport.updateComment(deps.context.commentId, body);
    requireValue(written.body === body && await state.authenticateWorkflowRecord(deps.transport, deps.context, written),
        "discovery write acknowledgement authentication failed");
    await state.writeFreshness(deps.transport, deps.context, cp.generation, hash(body));
    if (deps.journal) await deps.journal.write("committed", body);
    return { cursor: cp, bodyDigest: hash(body) };
}

async function provisionDiscovery(deps) {
    const { transport, context, registration, now, journal } = deps;
    requireValue(Date.parse(registration.provisionDeadline) > now(), "discovery provisioning authorization expired");
    requireValue(journal, "durable discovery provisioning journal required");
    const committed = await journal.read("committed"), head = await state.readFreshness(transport, context);
    const comment = await transport.getComment(context.commentId);
    requireValue(comment.id === context.commentId && comment.user?.id === context.actor.id &&
        comment.user?.login === context.actor.login && comment.user?.type === context.actor.type &&
        comment.performed_via_github_app?.id === context.actor.appId &&
        comment.issue_url === `https://api.github.com/repos/${repository}/issues/${context.issueNumber}`,
    "discovery provisioning writer or issue mismatch");
    const original = hash(comment.body) === registration.bootstrapDigest;
    const marker = { schemaVersion: 1, purpose: "discovery-provisioning", registration };
    if (committed === null) requireValue(original && head === null,
        "required discovery provisioning history missing; cannot replace existing history");
    else {
        const record = await state.authenticateDecodedRecord(transport, context, { ...comment, body: committed });
        requireValue(record?.kind === "proposal" && same(record.payload, marker),
            "discovery already committed or provisioning history authentication failed");
    }
    const initial = {
        schemaVersion: 1, purpose: "discovery-routing", query, policyDigest: fingerprint(policy),
        workflowGeneration: registration.workflowGeneration, generation: 0, previousDigest: registration.bootstrapDigest,
        cursor: { version: 1, page: 1, offset: 0 }, selected: [],
    };
    let body = await journal.read("intent");
    if (body === null) {
        requireValue(committed === null, "required discovery provisioning intent missing");
        body = state.serializeWorkflowRecord({ kind: "proposal", status: "Regression discovery routing", payload: initial }, context);
        await journal.write("intent", body);
    }
    const record = await state.authenticateDecodedRecord(transport, context, { ...comment, body });
    requireValue(record?.kind === "proposal" && same(record.payload, initial),
        "required discovery provisioning intent changed or unauthenticated");
    requireValue((original || comment.body === body) && (head === null ||
        !original && head.generation === 0 && head.bodyDigest === hash(body)), "discovery provisioning predecessor mismatch");
    if (committed === null) await journal.write("committed", state.serializeWorkflowRecord({
        kind: "proposal", status: "Regression discovery provisioning", payload: marker,
    }, context));
    if (original) {
        const written = await transport.updateComment(context.commentId, body);
        requireValue(written.body === body && await state.authenticateWorkflowRecord(transport, context, written),
            "discovery provisioning write acknowledgement authentication failed");
    }
    if (head === null) await state.writeFreshness(transport, context, 0, hash(body));
    await journal.write("committed", body);
    return { status: "provisioned", generation: 0 };
}

// Read-only with respect to issue allowances: the issue controller alone repairs,
// charges, executes and publishes under its own independent lock.
async function issueIntent(issueNumber, deps) {
    const reader = deps.readerFor(issueNumber);
    const registrationBytes = await reader.readOptionalTrustedFile(`.github/regression-bisect/registrations/${issueNumber}.json`, 4096);
    const registration = registrationBytes === null ? null : decode(registrationBytes);
    if (registration) {
        fields(registration, "allowanceId deadline checkpointAnchorSha checkpointCommentId checkpointBootstrapDigest resultCommentId resultBootstrapDigest");
        requireValue(/^[\w-]{1,128}$/.test(registration.allowanceId) && positive(registration.checkpointCommentId) &&
            positive(registration.resultCommentId) && registration.checkpointCommentId !== registration.resultCommentId &&
            [registration.checkpointBootstrapDigest, registration.resultBootstrapDigest].every(digest) &&
            registration.checkpointAnchorSha === deps.context.checkpointAnchorSha &&
            Number.isFinite(Date.parse(registration.deadline)), "invalid allowance registration");
    }
    const transport = deps.apiFor(issueNumber, registration);
    const context = { ...deps.context, issueNumber, workflowPath: controllerWorkflow,
        commentId: registration?.checkpointCommentId, bootstrapDigest: registration?.checkpointBootstrapDigest };
    let checkpoint = null;
    if (registration) {
        const comment = await transport.getComment(registration.checkpointCommentId);
        const record = await state.authenticateDecodedRecord(transport, context, comment);
        requireValue(record, "registered issue checkpoint authentication failed");
        if (record.kind === "checkpoint") {
            checkpoint = await state.readAuthenticatedCheckpoint(transport, context, { recoveryRegistration: registration });
            requireValue(checkpoint.checkpoint.state.budget.allowanceId === registration.allowanceId &&
                checkpoint.checkpoint.deadline === registration.deadline, "allowance registration changed");
        } else requireValue(record.kind === "proposal" && fingerprint(comment.body) === registration.checkpointBootstrapDigest &&
            await state.readFreshness(transport, context) === null, "registered bootstrap changed or rolled back");
    }
    const callbacks = {
        authenticateRecord: async comment => {
            if (issueNumber === deps.context.issueNumber && comment.id === deps.context.commentId) {
                requireValue(await state.authenticateWorkflowRecord(deps.transport, deps.context, comment),
                    "discovery record changed during intake");
                return true;
            }
            const record = await state.authenticateDecodedRecord(transport, { ...context, commentId: comment.id }, comment);
            if (!record) return false;
            requireValue(registration && [registration.checkpointCommentId, registration.resultCommentId].includes(comment.id),
                "unregistered authenticated issue state");
            return true;
        },
        readTrustedCheckpoints: async () => checkpoint ? [{
            status: "loaded", state: checkpoint.checkpoint.state, pending: checkpoint.checkpoint.pending,
            commentId: checkpoint.commentId, owner: checkpoint.owner,
        }] : [],
    };
    const options = { ...callbacks, transport, repository, issueNumber, now: deps.now(), readSource: reader.readSource };
    const intake = await readIssueSnapshot(options);
    requireValue(intake.status !== "blocked", `discussion incomplete: ${intake.reason}`);
    const discussionFingerprint = intake.snapshot?.discussionFingerprint ?? null;
    const intent = (status, reason) => ({ issueNumber, status, reason, discussionFingerprint,
        key: fingerprint({ repository, issueNumber, discussionFingerprint, reason }), dispatch: null,
        retryAfter: 0, acknowledged: false });
    if (intake.status === "ineligible") return intent("ineligible", "issue-ineligible");
    if (intake.snapshot.investigations.active.length) return intent("blocked", "active-investigation");
    const approvedBytes = await reader.readOptionalTrustedFile(`.github/regression-bisect/approved/${issueNumber}.json`, 48 * 1024 - 1);
    if (approvedBytes === null) return intent("proposal", "approval-required");
    const approval = decode(approvedBytes);
    fields(approval, "schemaVersion bootstrap manifest");
    requireValue(approval.schemaVersion === 1, "invalid reviewed approval");
    validateManifest(approval.manifest);
    requireValue(approval.manifest.repository === repository && approval.manifest.issueNumber === issueNumber, "approval issue mismatch");
    if (approval.manifest.discussionFingerprint !== discussionFingerprint) return intent("proposal", "approval-stale");
    if (!registration) return intent("proposal", "registration-required");
    requireValue(same(registration, approval.bootstrap), "approval allowance mismatch");
    const approved = await readIssueSnapshot({ ...options, reviewedManifest: approval.manifest });
    requireValue(approved.status === "complete" && approved.snapshot.discussionFingerprint === discussionFingerprint &&
        approved.snapshot.investigations.active.length === 0, "approved discussion or immutable samples changed");
    if (Date.parse(registration.deadline) <= deps.now()) return intent("blocked", "allowance-expired");
    const cp = checkpoint?.checkpoint;
    const generation = checkpoint?.freshnessGeneration ?? -1;
    if (cp?.publicationStatus === "published" && cp.generation === generation) return intent("complete", "controller-published");
    const mode = generation === -1 ? "bootstrap" : cp.publicationStatus === "requeue" ? "advance" :
        ["pending", "published"].includes(cp.publicationStatus) ||
        ["complete", "blocked", "stopped"].includes(cp.state.phase) ? "publish" : "advance";
    const key = dispatchKey({ issueNumber, allowanceId: registration.allowanceId, generation, mode,
        discussionFingerprint, policyRevision: deps.context.policyRevision });
    return { ...intent("pending", "controller-required"), key,
        dispatch: { issue: issueNumber, mode, routingKey: key, routingGeneration: String(generation) } };
}

async function route(deps, issueNumber) {
    validatePolicy(policy);
    let loaded = await loadCursor(deps, { repair: true });
    const cp = structuredClone(loaded.cursor);
    const found = await discoverEligibleIssues({ transport: deps.transport, repository, cursor: cp.cursor,
        ...(issueNumber === undefined ? {} : { issueNumber }) });
    requireValue(found.status === "complete", `discovery blocked: ${found.reason}`);
    const candidates = found.issues, nextCursor = found.nextCursor ?? cp.cursor;
    const selected = [];
    for (const candidate of candidates) {
        deps.signal.throwIfAborted();
        let current;
        try { current = await issueIntent(candidate.number, deps); }
        catch (error) {
            // An unreadable issue blocks only its own dispatch, not the bounded scan.
            current = { issueNumber: candidate.number, status: "blocked", reason: error.message.slice(0, 256),
                discussionFingerprint: null, key: fingerprint({ issueNumber: candidate.number, error: error.message }),
                dispatch: null, retryAfter: 0, acknowledged: false };
        }
        const previous = cp.selected.find(item => item.issueNumber === candidate.number && item.key === current.key);
        if (previous && current.status === "pending") {
            current.retryAfter = previous.retryAfter;
            current.acknowledged = previous.acknowledged;
        }
        selected.push(current);
    }
    const toDispatch = selected.filter(item => item.status === "pending" && item.retryAfter <= deps.now());
    for (const item of toDispatch) { item.retryAfter = deps.now() + 300000; item.acknowledged = false; }
    const changed = !same(cp.cursor, nextCursor) || !same(cp.selected, selected);
    if (changed) {
        cp.cursor = nextCursor; cp.selected = selected; cp.generation++;
        loaded = await saveCursor(deps, cp, loaded.bodyDigest);
    }
    const dispatched = [], failures = [];
    let invalidated = false;
    for (const item of toDispatch) {
        deps.signal.throwIfAborted();
        const fresh = await issueIntent(item.issueNumber, deps);
        if (fresh.status !== "pending" || fresh.key !== item.key) {
            Object.assign(item, fresh);
            invalidated = true;
            continue;
        }
        try { await deps.dispatch(item.dispatch); }
        catch (error) { failures.push(`issue ${item.issueNumber}: ${error.message}`); continue; }
        item.acknowledged = true;
        dispatched.push(item.issueNumber);
    }
    if (dispatched.length || invalidated) {
        cp.selected = selected; cp.generation++;
        await saveCursor(deps, cp, loaded.bodyDigest);
    }
    requireValue(failures.length === 0, failures.join("; "));
    return { status: "routed", selected: selected.filter(s => s.status !== "ineligible").map(s => s.issueNumber),
        dispatched, generation: cp.generation, nextCursor: cp.cursor,
        intents: selected.map(({ issueNumber, status, reason, key, discussionFingerprint }) =>
            ({ issueNumber, status, reason, key, discussionFingerprint })) };
}

module.exports = { route, provisionDiscovery, validateRegistration, dispatchKey, loadCursor, workflowPath };
