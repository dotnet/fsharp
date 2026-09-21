const engine = require("./engine.cjs");
const { confirmBoundary } = require("./attribution.cjs");
const { initializeCheckpoint, loadCheckpoint, authenticateDecodedRecord, serializeWorkflowRecord } = require("./state.cjs");
const { readIssueSnapshot } = require("./intake.cjs");
const { preparePublication, completePublication } = require("./report.cjs");
const { fingerprintInput: fingerprint } = require("./model.cjs");
const { requireValue, boundedWait } = require("./toolchain-io.cjs");

function validateEntry(input, context) {
    requireValue(input && Object.keys(input).sort().join(" ") === "issueNumber repository" &&
        input.repository === "dotnet/fsharp" && Number.isSafeInteger(input.issueNumber) && input.issueNumber > 0 &&
        context.repository === input.repository && context.issueNumber === input.issueNumber,
    "invalid controller entry or selected issue");
}

async function bootstrapController(input, deps) {
    try {
        validateEntry(input, deps.context);
        requireValue(typeof deps.withLock === "function" && typeof deps.approval?.readApproval === "function" &&
            typeof deps.approval.readSource === "function", "trusted bootstrap lock and pinned approval readers required");
        return await deps.withLock(async () => {
            const context = { ...deps.context, now: (deps.clock ?? (() => new Date()))().toISOString() };
            const { authorization: a } = context;
            requireValue(a?.approvalPath === `.github/regression-bisect/approved/${input.issueNumber}.json` &&
                a.approvalRevision === context.policyRevision, "bootstrap approval location mismatch");
            const bytes = await deps.approval.readApproval({ path: a.approvalPath, revision: a.approvalRevision });
            requireValue(Buffer.isBuffer(bytes) && bytes.length > 0 && bytes.length < 48 * 1024, "bounded raw approval bytes required");
            const approval = JSON.parse(new TextDecoder("utf-8", { fatal: true }).decode(bytes));
            requireValue(approval && Object.keys(approval).sort().join(" ") === "bootstrap manifest schemaVersion" &&
                approval.schemaVersion === 1 && fingerprint(approval.manifest) === fingerprint(context.manifest) &&
                a.manifestDigest === fingerprint(approval.manifest) && a.inputDigest === approval.manifest.approvedInputDigest,
            "reviewed bootstrap manifest mismatch");
            const b = approval.bootstrap;
            requireValue(b && Object.keys(b).sort().join(" ") ===
                "allowanceId checkpointAnchorSha checkpointBootstrapDigest checkpointCommentId deadline resultBootstrapDigest resultCommentId" &&
                typeof b.allowanceId === "string" && /^[a-zA-Z0-9_-]{1,128}$/.test(b.allowanceId) &&
                typeof b.deadline === "string" && /^\d{4}-\d\d-\d\dT.*Z$/.test(b.deadline) && Number.isFinite(Date.parse(b.deadline)) &&
                b.checkpointCommentId === context.commentId && b.checkpointAnchorSha === context.checkpointAnchorSha &&
                b.checkpointBootstrapDigest === context.bootstrapDigest &&
                [b.resultBootstrapDigest, b.checkpointBootstrapDigest].every(v => typeof v === "string" && /^[a-f0-9]{64}$/.test(v)) &&
                Number.isSafeInteger(b.resultCommentId) && b.resultCommentId > 0 && b.resultCommentId !== context.commentId,
            "invalid reviewed bootstrap registration");
            const checkpointComment = await deps.transport.getComment(context.commentId);
            const checkpointRecord = await authenticateDecodedRecord(deps.transport, context, checkpointComment);
            requireValue(checkpointRecord, "checkpoint bootstrap slot authentication failed");
            const resultContext = { ...context, commentId: b.resultCommentId };
            const resultComment = await deps.transport.getComment(b.resultCommentId);
            const resultRecord = await authenticateDecodedRecord(deps.transport, resultContext, resultComment);
            requireValue(resultRecord, "result bootstrap slot authentication failed");
            // Scan the issue prefix, not just the currently registered comment ID.
            let historyComplete = false, hasReceipt = false;
            const prefix = `regression-bisect/${context.issueNumber}/`;
            for (let page = 1; page <= 10; page++) {
                const response = await deps.transport.request("GET /repos/{owner}/{repo}/commits/{ref}/statuses", {
                    owner: "dotnet", repo: "fsharp", ref: context.checkpointAnchorSha, per_page: 100, page,
                });
                requireValue(response && (!response.status || response.status === 200) &&
                    Array.isArray(response.data) && response.data.length <= 100, "incomplete bootstrap history response");
                for (const status of response.data) {
                    requireValue(status && typeof status.context === "string", "malformed bootstrap history");
                    if (!status.context.startsWith(prefix)) continue;
                    requireValue(status.context === `${prefix}${context.commentId}`, "prior issue allowance registration exists");
                    hasReceipt = true;
                }
                const next = response.headers?.link === undefined ? response.data.length === 100 :
                    /;\s*rel="?next"?/.test(response.headers.link);
                if (!next) { historyComplete = true; break; }
            }
            requireValue(historyComplete, "bootstrap history page limit exceeded; prior investigation lookup incomplete");
            if (hasReceipt) {
                const { checkpoint: cp } = await loadCheckpoint(deps.transport, context);
                requireValue(cp.state.budget.allowanceId === b.allowanceId && cp.deadline === b.deadline,
                    "existing allowance does not match bootstrap registration");
                requireValue(resultRecord.kind === "result" && cp.state.publication.commentId === b.resultCommentId ||
                    resultRecord.kind === "proposal" && fingerprint(resultComment.body) === b.resultBootstrapDigest,
                "existing result slot changed");
                return { status: "loaded", generation: cp.generation, allowanceId: b.allowanceId, budget: cp.state.budget.consumed };
            }
            requireValue(resultRecord.kind === "proposal" && fingerprint(resultComment.body) === b.resultBootstrapDigest &&
                fingerprint(resultRecord.payload) === fingerprint({
                    schemaVersion: 1, purpose: "result-slot", repository: context.repository,
                    issueNumber: context.issueNumber, checkpointCommentId: context.commentId,
                }), "result bootstrap association mismatch");
            requireValue(checkpointRecord.kind === "checkpoint" || checkpointRecord.kind === "proposal" &&
                fingerprint(checkpointComment.body) === b.checkpointBootstrapDigest && fingerprint(checkpointRecord.payload) === fingerprint({
                    schemaVersion: 1, purpose: "checkpoint-slot", repository: context.repository, issueNumber: context.issueNumber,
                    checkpointAnchorSha: context.checkpointAnchorSha, allowanceId: b.allowanceId, deadline: b.deadline,
                }), "checkpoint bootstrap association mismatch");
            for (const record of [checkpointRecord, resultRecord]) {
                if (fingerprint(record.owner) === fingerprint(context.owner)) continue;
                const run = await deps.transport.getRun(record.owner.runId, record.owner.attempt);
                requireValue(run.status === "completed", "bootstrap slot owning run is still active");
            }
            if (checkpointRecord.kind !== "checkpoint") {
                const snapshot = await readIssueSnapshot({
                    transport: deps.intakeTransport ?? deps.transport, repository: context.repository, issueNumber: context.issueNumber,
                    now: context.now, reviewedManifest: approval.manifest, readSource: deps.approval.readSource,
                    authenticateRecord: async comment => [context.commentId, b.resultCommentId].includes(comment.id) &&
                        (await authenticateDecodedRecord(deps.transport, { ...context, commentId: comment.id }, comment)) !== null,
                    readTrustedCheckpoints: async () => [],
                });
                requireValue(snapshot.status === "complete", `bootstrap intake blocked: ${snapshot.reason}`);
                requireValue(snapshot.snapshot.investigations.active.length === 0, "bootstrap blocked by active linked investigation");
                context.issue = snapshot.snapshot.input.discussion.issue;
                context.issueFingerprint = snapshot.snapshot.discussionFingerprint;
            }
            context.now = (deps.clock ?? (() => new Date()))().toISOString();
            const cp = await initializeCheckpoint(deps.transport, context, { allowanceId: b.allowanceId, deadline: b.deadline });
            return { status: "initialized", generation: cp.generation, allowanceId: b.allowanceId, budget: cp.state.budget.consumed };
        });
    } catch (error) { return { status: "blocked", reason: error.message }; }
}

async function runController(input, deps) {
    try {
        validateEntry(input, deps.context);
        requireValue(["withLock", "runIsolated", "collectObservation"].every(k => typeof deps[k] === "function"),
            "controller requires whole-issue lock, isolated runner and trusted collector");
        let cleanupPending;
        const pending = new Promise(resolve => { cleanupPending = resolve; });
        const locked = deps.withLock(async () => {
            try {
                const now = () => (deps.clock ?? (() => new Date()))().toISOString();
                // A missing checkpoint is never a first-use authorization.
                await loadCheckpoint(deps.transport, { ...deps.context, now: now() });
                let intent = await engine.advanceInvestigation({ now: now() }, deps);
                if (["confirmation", "complete"].includes(intent.status) || intent.status === "blocked" && intent.interval)
                    intent = await confirmBoundary({ intent, now: now() }, deps);
                if (intent.status !== "work") return intent;
                const cp = await engine.executeInvestigationWork(intent, deps);
                if (cp.status === "blocked") return cp;
                const record = cp.records.find(r => r.id === intent.reservation.id);
                return { status: "observed", workId: record.id, generation: cp.generation,
                    outcome: cp.observations[record.observationDigest].data.outcome, budget: cp.state.budget.consumed };
            } finally {
                if (deps.collectObservation.settleQuota) {
                    const settled = deps.collectObservation.settleQuota();
                    try { await boundedWait(settled, AbortSignal.timeout(5000)); }
                    catch (error) {
                        cleanupPending({ status: "blocked", reason: `evidence ownership cleanup pending: ${error.message}` });
                        // A bounded response is not permission to release the issue lock.
                        await settled;
                    }
                }
            }
        });
        return await Promise.race([locked, pending]);
    } catch (error) { return { status: "blocked", reason: error.message }; }
}

async function publishController(input, deps) {
    try {
        validateEntry(input, deps.context);
        requireValue(typeof deps.withLock === "function" && typeof deps.readCurrentContext === "function" &&
            Number.isSafeInteger(deps.resultCommentId) && deps.resultCommentId > 0 &&
            deps.resultCommentId !== deps.context.commentId, "trusted publisher lock, current reader and fixed result slot required");
        return await deps.withLock(async () => {
            const now = () => (deps.clock ?? (() => new Date()))().toISOString();
            const context = { ...deps.context, now: now() };
            const { checkpoint } = await loadCheckpoint(deps.transport, context, { requiredEvidenceOnly: true });
            // Report helpers participate in this already-held whole-issue lock.
            const locked = { ...deps, withLock: action => action() };
            let ready = await preparePublication({ now: now(), generation: checkpoint.generation }, locked);
            if (!["ready", "pending"].includes(ready.status)) {
                requireValue(ready.status !== "published" || ready.commentId === deps.resultCommentId, "unexpected published result slot");
                return ready;
            }
            const slotContext = { ...context, commentId: deps.resultCommentId };
            const slot = await authenticateDecodedRecord(deps.transport, slotContext,
                await deps.transport.getComment(deps.resultCommentId));
            const proposal = slot?.kind === "proposal" && fingerprint(slot.payload) === fingerprint({
                schemaVersion: 1, purpose: "result-slot", repository: context.repository,
                issueNumber: context.issueNumber, checkpointCommentId: context.commentId,
            });
            const previousResult = slot?.kind === "result" &&
                slot.payload.report.state.budget.allowanceId === checkpoint.state.budget.allowanceId &&
                checkpoint.history.some(h => h.epoch === slot.payload.report.state.epoch &&
                    h.publication.digest === slot.payload.digest &&
                    [null, deps.resultCommentId].includes(h.publication.commentId));
            requireValue(proposal || previousResult, "result slot authentication or checkpoint association failed");
            const expectedDigest = ready.digest;
            ready = await preparePublication({ now: now(), generation: ready.generation }, locked);
            if (!["ready", "pending"].includes(ready.status)) return ready;
            requireValue(ready.digest === expectedDigest && ready.repository === input.repository &&
                ready.issueNumber === input.issueNumber, "publication intent changed before write");
            const body = serializeWorkflowRecord({ kind: "result", status: "Regression investigation result",
                payload: { schemaVersion: 1, digest: ready.digest, report: ready.report } }, slotContext);
            const written = await deps.transport.updateComment(deps.resultCommentId, body);
            requireValue(written?.body === body &&
                (await authenticateDecodedRecord(deps.transport, slotContext, written))?.kind === "result",
            "result write authentication failed");
            return completePublication({ now: now(), generation: ready.generation,
                digest: ready.digest, commentId: deps.resultCommentId }, locked);
        });
    } catch (error) { return { status: "blocked", reason: error.message }; }
}

module.exports = { bootstrapController, runController, publishController };
