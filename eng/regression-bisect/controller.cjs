const engine = require("./engine.cjs");
const { confirmBoundary } = require("./attribution.cjs");
const { loadCheckpoint, authenticateDecodedRecord, serializeWorkflowRecord } = require("./state.cjs");
const { preparePublication, completePublication } = require("./report.cjs");
const { fingerprintInput: fingerprint } = require("./model.cjs");
const { requireValue } = require("./toolchain-io.cjs");

function validateEntry(input, context) {
    requireValue(input && Object.keys(input).sort().join(" ") === "issueNumber repository" &&
        input.repository === "dotnet/fsharp" && Number.isSafeInteger(input.issueNumber) && input.issueNumber > 0 &&
        context.repository === input.repository && context.issueNumber === input.issueNumber,
    "invalid controller entry or selected issue");
}

async function runController(input, deps) {
    try {
        validateEntry(input, deps.context);
        requireValue(["withLock", "runIsolated", "collectObservation"].every(k => typeof deps[k] === "function"),
            "controller requires whole-issue lock, isolated runner and trusted collector");
        return await deps.withLock(async () => {
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
        });
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
            requireValue(slot?.kind === "proposal" && fingerprint(slot.payload) === fingerprint({
                schemaVersion: 1, purpose: "result-slot", repository: context.repository,
                issueNumber: context.issueNumber, checkpointCommentId: context.commentId,
            }), "result slot authentication or checkpoint association failed");
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

module.exports = { runController, publishController };
