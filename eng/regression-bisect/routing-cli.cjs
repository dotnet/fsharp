const fs = require("node:fs/promises");
const path = require("node:path");
const { createGithubApi, createApprovalReader, createDispatcher } = require("./github.cjs");
const { configuration, withIssueLock, bytes, writeJson } = require("./controller-cli.cjs");
const { requireValue, hash } = require("./toolchain-io.cjs");
const { validatePolicy } = require("./model.cjs");
const { route, provisionDiscovery, validateRegistration, workflowPath } = require("./routing.cjs");
const root = path.resolve(__dirname, "..", "..");
const decode = value => JSON.parse(new TextDecoder("utf-8", { fatal: true }).decode(value));
const usage = "Usage: node routing-cli.cjs <schedule|provision|event --issue N|manual --issue N> --config <trusted-deployment-path>";

function argumentsFor(args) {
    requireValue((args.length === 3 && ["schedule", "provision"].includes(args[0]) ||
        args.length === 5 && ["event", "manual"].includes(args[0]) && args[1] === "--issue" &&
        /^[1-9]\d*$/.test(args[2]) && Number.isSafeInteger(Number(args[2]))) &&
        args.at(-2) === "--config" && args.at(-1)?.length > 0 && !args.at(-1).startsWith("--"), usage);
    return { mode: args[0], issueNumber: args.length === 5 ? Number(args[2]) : undefined, configPath: path.resolve(args.at(-1)) };
}

function eventArguments(name, event, typedInputs) {
    requireValue(event?.repository?.full_name === "dotnet/fsharp" && event.repository.default_branch === "main",
        "untrusted routing event repository/default branch");
    const fixed = ["--config", path.join(root, ".github", "regression-bisect", "deployment.json")];
    if (name === "schedule") return ["schedule", ...fixed];
    if (name === "issues") {
        requireValue(["opened", "labeled", "unlabeled", "edited", "closed", "reopened"].includes(event.action) &&
            Number.isSafeInteger(event.issue?.number) && event.issue.number > 0, "invalid typed issue event");
        return ["event", "--issue", String(event.issue.number), ...fixed];
    }
    requireValue(name === "workflow_dispatch" && [undefined, "main", "refs/heads/main"].includes(event.ref) &&
        event.inputs && Object.keys(event.inputs).join() === "issue" &&
        typedInputs && Object.getPrototypeOf(typedInputs) === Object.prototype &&
        Object.keys(typedInputs).join() === "issue" && Number.isSafeInteger(typedInputs.issue) &&
        typedInputs.issue > 0 && String(event.inputs.issue) === String(typedInputs.issue), "invalid typed manual routing input");
    return ["manual", "--issue", String(typedInputs.issue), ...fixed];
}

async function main(args = process.argv.slice(2), env = process.env) {
    if (args.length === 1 && args[0] === "--help") return { help: usage };
    const signal = AbortSignal.timeout(300000);
    if (!args.length) {
        requireValue(env.GITHUB_ACTIONS === "true" && path.isAbsolute(env.GITHUB_EVENT_PATH ?? ""), usage);
        let event, typedInputs;
        try {
            event = decode(await bytes(env.GITHUB_EVENT_PATH, 4 * 1024 * 1024));
            if (env.GITHUB_EVENT_NAME === "workflow_dispatch") typedInputs = JSON.parse(env.RB_ACTION_INPUTS);
        } catch (error) {
            if (!(error instanceof SyntaxError)) throw error;
            throw new Error("invalid routing event JSON");
        }
        args = eventArguments(env.GITHUB_EVENT_NAME, event, typedInputs);
    }
    const input = argumentsFor(args);
    requireValue(!env.RB_LOCAL_TEST_FILE || !env.GITHUB_ACTIONS && env.RB_LOCAL_TEST === "explicit-offline-fixture",
        "local test injection forbidden in Actions or without explicit local opt-in");
    const local = env.RB_LOCAL_TEST_FILE
        ? await require("./fixtures/controller-service.cjs").connect(path.resolve(env.RB_LOCAL_TEST_FILE)) : null;
    const configBytes = await bytes(input.configPath), config = decode(configBytes);
    const base = configuration(config, env);
    const apiFor = (issueNumber, registration) => createGithubApi({
        repository: base.repository, issueNumber,
        checkpointCommentId: registration?.checkpointCommentId ?? 1,
        resultCommentId: registration?.resultCommentId ?? 2, anchorSha: config.anchorSha,
        token: env.RB_GITHUB_TOKEN, signal,
    }, local?.send);
    const initial = apiFor(1);
    const readerFor = issueNumber => createApprovalReader({ api: initial, revision: base.policyRevision, issueNumber });
    const reader = readerFor(1);
    requireValue(hash(await reader.readTrustedFile(".github/regression-bisect/deployment.json", 48 * 1024 - 1)) === hash(configBytes),
        "configuration differs from pinned trusted bytes");
    const registration = validateRegistration(decode(await reader.readTrustedFile(".github/regression-bisect/discovery.json", 4096)));
    requireValue(registration.checkpointAnchorSha === config.anchorSha, "discovery anchor changed");
    const issueRegistration = await reader.readOptionalTrustedFile(
        `.github/regression-bisect/registrations/${registration.issueNumber}.json`, 4096);
    if (issueRegistration !== null) {
        const { checkpointCommentId, resultCommentId } = decode(issueRegistration);
        requireValue([checkpointCommentId, resultCommentId].every(id => Number.isSafeInteger(id) && id > 0 &&
            ![registration.commentId, registration.freshnessCommentId].includes(id)),
        "discovery slot conflicts with issue registration");
    }
    const policyBytes = await reader.readTrustedFile(".github/policies/regression-bisect.json", 48 * 1024 - 1);
    validatePolicy(decode(policyBytes));
    requireValue(hash(policyBytes) === hash(await fs.readFile(path.join(root, ".github", "policies", "regression-bisect.json"))),
        "stale discovery policy checkout");
    const workflow = await reader.readTrustedFile(workflowPath, 48 * 1024 - 1);
    if (!local) {
        requireValue(hash(workflow) === hash(await bytes(path.join(root, workflowPath))), "stale discovery workflow checkout");
        const { execFileSync } = require("node:child_process");
        const git = args => execFileSync(config.gitExecutable, args, { cwd: root, encoding: "utf8", timeout: 30000 }).trim();
        requireValue(git(["rev-parse", "HEAD"]) === base.workflowRevision &&
            git(["status", "--porcelain", "--untracked-files=all"]) === "", "trusted routing checkout changed");
    }
    const context = { ...base, issueNumber: registration.issueNumber, commentId: registration.commentId, workflowPath };
    const transport = createGithubApi({ repository: base.repository, issueNumber: context.issueNumber,
        checkpointCommentId: context.commentId, resultCommentId: registration.freshnessCommentId,
        anchorSha: config.anchorSha, token: env.RB_GITHUB_TOKEN, signal }, local?.send);
    const run = await transport.getRun(context.owner.runId, context.owner.attempt);
    requireValue(String(run.id) === context.owner.runId && run.run_attempt === context.owner.attempt &&
        run.path === workflowPath && run.head_sha === context.workflowRevision && run.head_branch === "main" &&
        run.repository?.full_name === base.repository && run.head_repository?.full_name === base.repository &&
        run.status === "in_progress", "current discovery workflow authentication failed");
    requireValue(path.isAbsolute(env.RB_STATE_DIRECTORY ?? ""), "absolute trusted RB_STATE_DIRECTORY required");
    const directory = path.join(env.RB_STATE_DIRECTORY, "dotnet-fsharp-discovery");
    return withIssueLock(directory, context, transport, async () => {
        const deps = { context, registration, transport, readerFor, apiFor, signal,
            journal: {
                read: async name => {
                    try { return decode(await bytes(path.join(directory, `${name}.json`), 64 * 1024)); }
                    catch (error) { if (error.code === "ENOENT") return null; throw error; }
                },
                write: (name, body) => writeJson(path.join(directory, `${name}.json`), body),
            },
            now: () => (local?.clock() ?? new Date()).getTime(),
            dispatch: createDispatcher({ token: env.RB_DISPATCH_TOKEN, signal }, local?.send) };
        return input.mode === "provision" ? provisionDiscovery(deps) : route(deps, input.issueNumber);
    });
}

if (require.main === module) main().then(result => {
    console.log(result.help ?? JSON.stringify(result));
}).catch(error => {
    console.log(JSON.stringify({ status: "blocked", reason: error.message }));
    process.exitCode = 1;
});

module.exports = { main, argumentsFor, eventArguments };
