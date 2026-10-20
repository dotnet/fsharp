const fs = require("node:fs");
const fsp = require("node:fs/promises");
const os = require("node:os");
const path = require("node:path");
const { createHash, createHmac, randomUUID } = require("node:crypto");
const { execFileSync, spawn, spawnSync } = require("node:child_process");
const { setTimeout: delay } = require("node:timers/promises");
const { bootstrapFixture } = require("./execution.cjs");
const f = require("./contracts.cjs");
const state = require("../state.cjs");
const { image } = require("../container.cjs");
const { executionEvidence, readExecutionOutput } = require("../worker.cjs");
const { executionFileNames } = require("../worker-output.cjs");
const { fingerprintInput: fingerprint } = require("../model.cjs");

const json = value => Buffer.from(JSON.stringify(value));
const blob = bytes => createHash("sha1").update(`blob ${bytes.length}\0`).update(bytes).digest("hex");
const indexUrl = "https://builds.dotnet.microsoft.com/dotnet/release-metadata/releases-index.json";
const channelUrl = "https://builds.dotnet.microsoft.com/dotnet/release-metadata/10.0/releases.json";
const archiveUrl = "https://builds.dotnet.microsoft.com/dotnet/Sdk/10.0.100/dotnet-sdk-10.0.100-linux-x64.tar.gz";
const archive = Buffer.from("offline fixture archive; never executable");
const archiveHash = createHash("sha512").update(archive).digest("hex");
const metadata = {
    [indexUrl]: { "releases-index": [{ "channel-version": "10.0", "releases.json": channelUrl }] },
    [channelUrl]: { "channel-version": "10.0", releases: [{ "release-date": "2026-09-01",
        runtime: { version: "10.0.0" }, sdk: { version: "10.0.100", files: [
            { rid: "linux-x64", name: "dotnet-sdk-linux-x64.tar.gz", url: archiveUrl, hash: archiveHash },
        ] } }] },
};

function writeDatabase(file, value) {
    const temporary = `${file}.${randomUUID()}.tmp`;
    try {
        fs.writeFileSync(temporary, json(value), { flag: "wx" });
        for (let attempt = 0; ; attempt++) {
            try { fs.renameSync(temporary, file); break; }
            catch (error) {
                // Windows readers can briefly deny delete sharing, as with container receipts.
                if (process.platform !== "win32" || error.code !== "EPERM" || attempt === 100) throw error;
                Atomics.wait(new Int32Array(new SharedArrayBuffer(4)), 0, 0, 10);
            }
        }
    } finally { fs.rmSync(temporary, { force: true }); }
}

function invokeCli(args, env = {}, started = () => {}, {
    entry = "controller-cli.cjs", kind = "cli-exit", storageFaults = true, preload = [],
} = {}) {
    return new Promise((resolve, reject) => {
        const start = performance.now();
        const storagePreload = storageFaults && env.RB_LOCAL_TEST_FILE
            ? ["--require", path.join(__dirname, "controller-storage-faults.cjs")] : [];
        const child = spawn(process.execPath, [...storagePreload, ...preload.flatMap(file => ["--require", file]),
            path.join(__dirname, "..", entry), ...args], {
            env: { ...process.env, ...env }, shell: false, windowsHide: true,
        });
        let stdout = "", stderr = "";
        child.stdout.on("data", bytes => { stdout += bytes; });
        child.stderr.on("data", bytes => { stderr += bytes; });
        child.once("error", reject);
        child.once("close", async code => {
            try {
                if (env.RB_LOCAL_TEST_FILE) await fsp.appendFile(`${env.RB_LOCAL_TEST_FILE}.ledger`, JSON.stringify({
                    kind, pid: child.pid, args, code, durationMs: performance.now() - start, stdout, stderr,
                }) + "\n");
                resolve({ code, stdout, stderr });
            } catch (error) { reject(error); }
        });
        started(child);
    });
}

async function fixture(t, options = {}) {
    const directory = await fsp.realpath(await fsp.mkdtemp(path.join(os.tmpdir(), "rb-controller-")));
    t.after(async () => {
        const evidence = process.env.RB_TEST_EVIDENCE_DIRECTORY;
        if (evidence) {
            assertValue(path.isAbsolute(evidence), "absolute test evidence directory required");
            await fsp.mkdir(evidence, { recursive: true });
            const ledger = path.join(directory, "service.json.ledger");
            if (fs.existsSync(ledger)) await fsp.copyFile(ledger, path.join(evidence, `${path.basename(directory)}.jsonl`));
        }
        await fsp.rm(directory, { recursive: true, force: true });
    });
    const original = await bootstrapFixture(options);
    const { h, approval, result, data } = original;
    const context = h.context;
    context.workflowPath = ".github/workflows/regression-bisect-controller.yml";
    context.workflowRevision = context.policyRevision;
    context.trustedWorkflowRevisions = [context.workflowRevision];
    const gitExecutable = process.env.PATH.split(path.delimiter).map(p => path.join(p, process.platform === "win32" ? "git.exe" : "git"))
        .find(p => fs.existsSync(p));
    assertValue(gitExecutable, "Git executable unavailable");
    const mirrorDirectory = path.join(directory, "mirror");
    await fsp.mkdir(mirrorDirectory);
    const git = args => execFileSync(gitExecutable, args, { cwd: mirrorDirectory, encoding: "utf8",
        env: { ...process.env, GIT_CONFIG_NOSYSTEM: "1", GIT_CONFIG_GLOBAL: path.join(directory, "no-config"),
            GIT_AUTHOR_NAME: "Fixture", GIT_AUTHOR_EMAIL: "fixture@example.invalid",
            GIT_COMMITTER_NAME: "Fixture", GIT_COMMITTER_EMAIL: "fixture@example.invalid" } }).trim();
    git(["init", "--quiet", "--initial-branch=main", "--template="]);
    await fsp.writeFile(path.join(mirrorDirectory, "source.txt"), "synthetic inert source");
    git(["add", "source.txt"]); git(["commit", "--quiet", "-m", "fixture"]);
    const olderSha = git(["rev-parse", "HEAD"]);
    await fsp.appendFile(path.join(mirrorDirectory, "source.txt"), "\nnewer inert source");
    git(["commit", "--quiet", "-am", "newer fixture"]);
    context.pinnedMainSha = git(["rev-parse", "HEAD"]);
    context.manifest.producer = { name: "no-producer", version: "0.0.0", sha256: f.digest("") };
    context.manifest.packages = [];
    context.manifest.searchHints.sourceShas = [olderSha];
    f.seal(context.manifest);
    approval.manifest = structuredClone(context.manifest);
    context.authorization.manifestDigest = fingerprint(context.manifest);
    context.authorization.inputDigest = context.manifest.approvedInputDigest;
    const checkpointPayload = (await state.authenticateDecodedRecord(h.transport, {
        ...context, workflowPath: ".github/workflows/regression-bisect.yml",
        trustedWorkflowRevisions: [f.sha("workflow")],
    }, h.comment))?.payload;
    assertValue(checkpointPayload, "fixture checkpoint proposal unavailable");
    h.comment.body = state.serializeWorkflowRecord({ kind: "proposal", status: "Reserved checkpoint slot", payload: checkpointPayload }, context);
    result.body = state.serializeWorkflowRecord({ kind: "proposal", status: "Reserved result slot", payload: {
        schemaVersion: 1, purpose: "result-slot", repository: context.repository,
        issueNumber: context.issueNumber, checkpointCommentId: context.commentId,
    } }, { ...context, commentId: result.id });
    context.bootstrapDigest = approval.bootstrap.checkpointBootstrapDigest = fingerprint(h.comment.body);
    approval.bootstrap.resultBootstrapDigest = fingerprint(result.body);
    const config = {
        schemaVersion: 1, workflowPath: context.workflowPath, actor: context.actor, anchorSha: context.checkpointAnchorSha,
        retainedWorkflowRevisions: [], keyId: context.keyId, keyHashes: { fixture: f.digest(context.keys.fixture) },
        containment: { configSha256: f.digest("synthetic config"), reportSha256: f.digest("not a physical attestation"),
            receiptSha256: Object.fromEntries(["inspect", "disk", "pids", "cpu", "memory", "malformed", "timeout"]
                .map(mode => [mode, f.digest(`synthetic ${mode}`)])) },
        platform: { os: "linux", architecture: "x64", rid: "linux-x64" }, gitExecutable,
        container: { schemaVersion: 1, image, dockerExecutable: "/usr/bin/docker",
            limits: { cpus: 2, memoryMiB: 8192, pids: 256, scratchMiB: 8192, timeoutMs: 6600000 } },
    };
    const configPath = path.join(directory, "deployment.json"), serviceFile = path.join(directory, "service.json");
    const files = {
        ".github/regression-bisect/deployment.json": json(config).toString("base64"),
        ".github/regression-bisect/approved/123.json": json(approval).toString("base64"),
        ".github/regression-bisect/registrations/123.json": json(approval.bootstrap).toString("base64"),
        ".github/regression-bisect/provision/123.json": json({ schemaVersion: 1,
            allowanceId: approval.bootstrap.allowanceId, deadline: approval.bootstrap.deadline }).toString("base64"),
        ".github/policies/regression-bisect.json": fs.readFileSync(path.join(__dirname, "..", "..", "..", ".github", "policies", "regression-bisect.json")).toString("base64"),
        [context.workflowPath]: Buffer.from("fixture workflow bytes").toString("base64"),
        [context.manifest.sources[0].path]: Buffer.from('printfn "ok"').toString("base64"),
    };
    const run = { id: 12345, run_attempt: 1, path: context.workflowPath, head_sha: context.workflowRevision,
        head_branch: "main", repository: { full_name: "dotnet/fsharp", id: 99 },
        head_repository: { full_name: "dotnet/fsharp", id: 99 }, status: "in_progress", conclusion: null };
    const db = { files, issue: data.issue, comments: [h.comment, result], timeline: [], statuses: [],
        runs: { "12345/1": run }, revision: context.workflowRevision, anchorSha: config.anchorSha,
        mirrorDirectory, olderSha, now: context.now, actor: context.actor, nextId: 1000, faults: {} };
    await fsp.writeFile(configPath, json(config));
    writeDatabase(serviceFile, db);
    const env = { GITHUB_ACTIONS: "", RB_LOCAL_TEST: "explicit-offline-fixture", RB_LOCAL_TEST_FILE: serviceFile,
        RB_TRUSTED_REVISION: context.policyRevision, GITHUB_WORKFLOW_SHA: context.workflowRevision,
        GITHUB_REPOSITORY: "dotnet/fsharp", GITHUB_REF: "refs/heads/main", GITHUB_RUN_ID: "12345", GITHUB_RUN_ATTEMPT: "1",
        RB_SIGNING_KEYS: JSON.stringify({ fixture: context.keys.fixture.toString("base64") }),
        RB_GITHUB_TOKEN: "offline-controller-token", RB_PROVISION_TOKEN: "offline-separate-maintainer-token",
        RB_STATE_DIRECTORY: path.join(directory, "state") };
    const read = () => JSON.parse(fs.readFileSync(serviceFile));
    const update = action => { const value = read(); action(value); writeDatabase(serviceFile, value); };
    return { directory, config, configPath, serviceFile, env, context, approval, read, update, git,
        ledger: () => fs.existsSync(`${serviceFile}.ledger`) ? fs.readFileSync(`${serviceFile}.ledger`, "utf8").trim().split("\n").filter(Boolean).map(JSON.parse) : [],
        moveMain() {
            git(["commit", "--quiet", "--allow-empty", "-m", "next fixture main"]);
            return git(["rev-parse", "HEAD"]);
        },
        nextRun() {
            update(db => { const old = db.runs[`${env.GITHUB_RUN_ID}/1`]; old.status = "completed"; old.conclusion = "cancelled";
                env.GITHUB_RUN_ID = String(Number(env.GITHUB_RUN_ID) + 1);
                db.runs[`${env.GITHUB_RUN_ID}/1`] = { ...old, id: Number(env.GITHUB_RUN_ID), status: "in_progress", conclusion: null }; });
        },
    };
}

function assertValue(value, message) { if (!value) throw new Error(message); }

async function routingFixture(t, options = {}) {
    const value = await fixture(t, options);
    const workflowPath = ".github/workflows/regression-bisect.lock.yml";
    const registration = {
        schemaVersion: 2, repository: "dotnet/fsharp", issueNumber: 123, commentId: 458, freshnessCommentId: 459,
        checkpointAnchorSha: value.config.anchorSha, bootstrapDigest: f.digest("Reviewed discovery slot"),
        freshnessBootstrapDigest: f.digest("Reviewed discovery freshness slot"),
        provisionDeadline: options.deadline ?? "2026-10-01T00:00:00Z", workflowPath, workflowGeneration: 1,
    };
    value.env.RB_DISPATCH_TOKEN = "offline-separate-dispatch-token";
    value.update(db => {
        db.runs["22345/1"] = { ...db.runs["12345/1"], id: 22345, path: workflowPath };
        db.issues = [db.issue];
        db.comments.push({ ...db.comments[0], id: 458, body: "Reviewed discovery slot" });
        db.comments.push({ ...db.comments[0], id: 459, body: "Reviewed discovery freshness slot" });
        db.files[".github/regression-bisect/discovery.json"] = json(registration).toString("base64");
        db.files[workflowPath] = null;
    });
    value.env.GITHUB_RUN_ID = "22345";
    value.registration = registration;
    value.routingContext = { ...value.context, workflowPath, commentId: 458, owner: { runId: "22345", attempt: 1 } };
    return value;
}

async function checkpoint(fixture) {
    const service = await connect(fixture.serviceFile);
    const transport = require("../github.cjs").createGithubApi({ repository: "dotnet/fsharp", issueNumber: 123,
        checkpointCommentId: 456, resultCommentId: 457, anchorSha: fixture.config.anchorSha,
        token: "offline-controller-token" }, service.send);
    const context = { ...fixture.context, owner: { runId: fixture.env.GITHUB_RUN_ID, attempt: 1 } };
    const cp = (await state.authenticateDecodedRecord(transport, context, await transport.getComment(456))).payload;
    Object.assign(context, { stableSdk: cp.state.budget.stableSdk, pinnedMainSha: cp.state.pinnedMainSha,
        environmentFingerprint: cp.state.budget.environmentFingerprint });
    return { context, cp, transport };
}

async function seed(fixture, endpoint, outcome, target, attempts) {
    const { context, cp, transport } = await checkpoint(fixture);
    const kind = endpoint === "stable" ? "released" : "source";
    const sourceSha = target ?? (endpoint === "good" ? fixture.read().olderSha : context.pinnedMainSha);
    const reservation = await state.reserveWork(transport, context, { generation: cp.generation,
        attemptId: `seed-${cp.sequence}`, request: { kind, endpoint, phase: endpoint === "good" ? "good-search" : endpoint ?? "bisect",
            target: kind === "released" ? { sdk: context.stableSdk } : { sourceSha } } });
    const observation = syntheticObservation(reservation, context, outcome, attempts);
    return state.commitObservation(transport, context, { generation: reservation.generation,
        attemptId: reservation.id, observation, evidence: [] });
}

function syntheticObservation(reservation, context, outcome, attempts) {
    const kind = reservation.request.kind, sourceSha = reservation.request.target.sourceSha;
    const observation = f.observation(kind, outcome, context.manifest, sourceSha);
    observation.buildInstanceId = reservation.id;
    observation.provenance = { workerId: "synthetic-seed", runId: context.owner.runId,
        workflowRevision: context.workflowRevision, policyRevision: context.policyRevision };
    if (kind === "released") observation.target.sdk = context.stableSdk;
    for (const cell of observation.cells) {
        cell.environment.imageDigest = image.split(":").at(-1);
        cell.sdk.sha256 = context.stableSdk.archives[0].sha256;
        for (const artifact of Object.values(cell.artifacts)) artifact.buildInstanceId = reservation.id;
        if (kind === "source") cell.buildToolchain = f.buildToolchain(sourceSha, cell.environment.os);
        if (attempts) {
            cell.attempts = structuredClone(attempts);
            cell.outcome = observation.outcome = require("../model.cjs").classifyAttempts(attempts, context.manifest);
        }
    }
    return observation;
}

async function connect(file, { readOnly = false, synthetic } = {}) {
    let snapshot;
    const read = () => {
        const stat = fs.statSync(file, { bigint: true });
        const identity = `${stat.dev}:${stat.ino}:${stat.size}:${stat.mtimeNs}:${stat.ctimeNs}`;
        // Writers replace immutable snapshots; every request still checks the current file.
        if (snapshot?.identity !== identity) snapshot = { identity, bytes: fs.readFileSync(file) };
        return JSON.parse(snapshot.bytes);
    };
    const write = db => writeDatabase(file, db);
    const record = entry => {
        if (!readOnly) fs.appendFileSync(`${file}.ledger`, JSON.stringify({ pid: process.pid, ...entry }) + "\n");
    };
    const respond = (data, status = 200, headers = {}) => new Response(json(data), { status, headers });
    const fault = (db, name) => {
        if (!db.faults[name]) return false;
        delete db.faults[name]; write(db); return true;
    };
    synthetic ??= read().SYNTHETIC;
    return {
        ...(synthetic ? { collectObservation: async ({ reservation, context }) => {
            assertValue(process.env.RB_LOCAL_TEST === "explicit-offline-fixture" && !process.env.GITHUB_ACTIONS &&
                !process.env.RB_TRIAL_DIRECTORY, "SYNTHETIC observations are unit fixtures only");
            const launch = path.join(process.env.RB_STATE_DIRECTORY, "dotnet-fsharp-123", "launches", reservation.id);
            const { SYNTHETIC: outcome } = JSON.parse(await fsp.readFile(path.join(launch, "fixture-input.json")));
            assertValue(["pass", "product-failure", "untestable"].includes(outcome), "unknown SYNTHETIC outcome");
            record({ kind: "SYNTHETIC-observation", reservation: reservation.id, outcome });
            const observation = syntheticObservation(reservation, context, outcome);
            if (reservation.request.kind === "released") for (const cell of observation.cells) {
                cell.releasedToolchain = { sdk: cell.sdk, compilerRuntime: cell.runtime,
                    archive: f.archiveReceipt({ name: "dotnet-sdk-linux-x64.tar.gz", url: archiveUrl,
                        sha512: archiveHash, rid: "linux-x64", architecture: "x64", provenance: { kind: "publisher-checksum" } },
                    "/SYNTHETIC/sdk.tar.gz", archive) };
                cell.artifacts.compiler.assemblyVersion = "14.0.100.0";
                cell.artifacts.core.assemblyVersion = "10.0.0.0";
            }
            return { observation, evidence: [] };
        } } : {}),
        clock: () => new Date(read().now),
        mirrorDirectory: read().mirrorDirectory,
        executeGit: async (executable, args, settings) => {
            const config = JSON.parse(Buffer.from(read().files[".github/regression-bisect/deployment.json"], "base64"));
            assertValue(executable === config.gitExecutable && settings.env?.GIT_ALLOW_PROTOCOL === "file" &&
                !Object.keys(settings.env).some(k => /^(?:RB_|GITHUB_|ACTIONS_|COPILOT_|REGRESSION_)/i.test(k)),
            "only credential-free trusted fixture Git may use the offline adapter");
            // This is trusted Git over synthetic local history, not a historical worker.
            const result = await require("../toolchain-io.cjs").run(executable, args, settings);
            assertValue(Buffer.byteLength(result.stdout) <= (settings.maxBytes ?? 4 * 1024 * 1024) &&
                Buffer.byteLength(result.stderr) <= (settings.maxBytes ?? 4 * 1024 * 1024), "fixture Git output exceeded limit");
            return { ...result, status: "completed", complete: true };
        },
        recordGit: args => record({ kind: "git", args }),
        recordGitResult: (args, result, durationMs) => {
            record({ kind: "git-result", args, exitCode: result.exitCode, durationMs });
            if (read().faults.oversizeBundle && args.includes("bundle") && args.includes("create") && result.exitCode === 0)
                fs.truncateSync(args[args.indexOf("create") + 1], 1024 ** 3 + 1);
        },
        checkAnchor: async anchor => assertValue(anchor === read().anchorSha, "freshness anchor mismatch"),
        inspectRuntime: async () => {
            record({ kind: "runtime" });
            const db = read();
            assertValue(!db.faults.runtime, "unsupported Docker runtime");
            if (db.faults.pauseRuntime) {
                record({ kind: "barrier" });
                for (let i = 0; i < 400 && read().faults.pauseRuntime; i++) await delay(25);
                assertValue(!read().faults.pauseRuntime, "fixture barrier timeout");
            }
        },
        toolchainDependencies: { clock: () => new Date(read().now), http: async url => {
            record({ kind: "metadata", url });
            assertValue(metadata[url], "unexpected metadata destination");
            const value = structuredClone(metadata[url]);
            if (url === indexUrl && read().faults.newMetadata) value.updated = true;
            return { status: 200, url, redirects: [], bytes: json(value) };
        } },
        download: async (url, options) => {
            record({ kind: "download", url });
            assertValue(url === archiveUrl, "unexpected archive destination");
            assertValue(options && typeof options === "object" && path.isAbsolute(options.destination),
                "streaming acquisition requires the existing HTTP destination options contract");
            const { destination } = options;
            await fsp.writeFile(destination, archive, { flag: "wx" });
            return { status: 200, url, redirects: [], sha512: archiveHash, sha256: f.digest(archive), size: archive.length };
        },
        send: async (url, options) => {
            assertValue(!readOnly || options.method === "GET", "read-only service cannot mutate");
            const location = new URL(url);
            assertValue(location.origin === "https://api.github.com", "unexpected API authority");
            const route = location.pathname.replace("/repos/dotnet/fsharp/", "");
            const body = options.body ? JSON.parse(options.body) : null;
            record({ kind: "api", method: options.method, route, body, url, query: Object.fromEntries(location.searchParams) });
            const db = read();
            const page = (rows, size = 100) => rows.slice((Number(location.searchParams.get("page") ?? 1) - 1) * size,
                Number(location.searchParams.get("page") ?? 1) * size);
            if (db.faults.routingRateLimit) return respond({}, 429);
            if (route === "actions/workflows/regression-bisect-controller.yml/dispatches") {
                assertValue(options.method === "POST" && options.headers.Authorization === "Bearer offline-separate-dispatch-token",
                    "dispatch requires separate routing credential");
                db.dispatches ??= [];
                db.dispatches.push(body); write(db);
                if (fault(db, "dispatchAck")) throw new Error("dispatch acknowledgement lost");
                return new Response(null, { status: 204 });
            }
            if (route === "issues") {
                if (db.faults.pauseDiscovery) {
                    record({ kind: "discovery-barrier" });
                    for (let i = 0; i < 400 && read().faults.pauseDiscovery; i++) await delay(25);
                    assertValue(!read().faults.pauseDiscovery, "fixture discovery barrier timeout");
                }
                const list = [db.issue, ...(db.issues ?? []).filter(i => i.number !== db.issue.number)].filter(issue => issue.state === "open" &&
                    issue.labels.some(label => (typeof label === "string" ? label : label.name).toLowerCase() === "regression"))
                    .sort((a, b) => Date.parse(a.created_at) - Date.parse(b.created_at) || a.number - b.number);
                return respond(page(list), db.faults.discoveryPage === Number(location.searchParams.get("page")) ? 503 : 200,
                    { link: Number(location.searchParams.get("page")) * 100 < list.length ? '<https://api.github.com/next>; rel="next"' : "" });
            }
            if (route.startsWith("actions/runs/")) {
                const [, , id, , attempt] = route.split("/");
                return respond(db.runs[`${id}/${attempt}`] ?? {}, db.runs[`${id}/${attempt}`] ? 200 : 404);
            }
            if (route === "issues/123") return respond(db.issue);
            const links = (rows, size = 100) => ({ link: Number(location.searchParams.get("page")) * size < rows.length
                ? '<https://api.github.com/next>; rel="next"' : "" });
            if (route === "issues/123/timeline") return respond(page(db.timeline), 200, links(db.timeline));
            if (route === "issues/123/comments") {
                if (options.method === "GET") return respond(page(db.comments), 200,
                    db.faults.discussionPages ? { link: '<https://api.github.com/next>; rel="next"' } : links(db.comments));
                const comment = { id: db.nextId++, user: { id: db.actor.id, login: db.actor.login, type: db.actor.type },
                    performed_via_github_app: { id: db.actor.appId },
                    issue_url: "https://api.github.com/repos/dotnet/fsharp/issues/123", body: body.body };
                db.comments.push(comment); write(db);
                if (fault(db, "creationAck")) throw new Error("slot creation acknowledgement lost");
                return respond(comment, 201);
            }
            if (route.startsWith("issues/comments/")) {
                const id = Number(route.split("/").at(-1));
                const comment = db.comments.find(c => c.id === id) ??
                    Object.values(db.discussions ?? {}).flatMap(d => d.comments).find(c => c.id === id);
                if (!comment) return respond({}, 404);
                if (options.method === "PATCH") {
                    if (comment.id === 458 && fault(db, "cursorWrite")) throw new Error("cursor write interrupted");
                    if (comment.id === 459 && fault(db, "freshnessWrite")) throw new Error("freshness write interrupted");
                    if (comment.id === 458 && db.dispatches?.length && fault(db, "acknowledgementWrite"))
                        throw new Error("dispatch acknowledgement write interrupted");
                    comment.body = body.body; write(db);
                    if (comment.id === 458 && db.faults.editAfterSelection) {
                        db.issue.body += "\nChanged after selection";
                        delete db.faults.editAfterSelection;
                        write(db);
                    }
                    if (comment.id === 458 && fault(db, "cursorAck")) throw new Error("cursor acknowledgement lost");
                    if (comment.id === 459 && fault(db, "statusAck")) throw new Error("freshness acknowledgement lost");
                    if (comment.id === 457 && fault(db, "resultAck")) throw new Error("result acknowledgement lost");
                    if (comment.id === 456 && body.body.includes('"publicationStatus":"published"') && fault(db, "publishedAck"))
                        throw new Error("published checkpoint acknowledgement lost");
                    if (fault(db, "commentAck")) throw new Error("comment acknowledgement lost");
                }
                return respond(comment);
            }
            const issueRoute = /^issues\/(\d+)(?:\/(comments|timeline))?$/.exec(route);
            if (issueRoute) {
                const issue = db.issues?.find(i => i.number === Number(issueRoute[1]));
                if (!issue) return respond({}, 404);
                const rows = db.discussions?.[issue.number]?.[issueRoute[2]];
                assertValue(!issueRoute[2] || Array.isArray(rows), "missing per-issue discussion fixture");
                return respond(issueRoute[2] ? page(rows) : issue, 200, rows ? links(rows) : {});
            }
            if (route.endsWith("/statuses") && options.method === "GET")
                return respond(page(db.statuses), db.faults.history ? 503 : 200,
                    db.faults.pagination ? { link: '<https://api.github.com/next>; rel="next"' } : links(db.statuses));
            if (route.startsWith("statuses/")) {
                if (db.statuses.filter(s => s.context === body.context).length >= 1000)
                    return respond({ message: "status context limit exceeded" }, 422);
                const receipt = { id: db.nextId++, creator: { id: db.actor.id, login: db.actor.login, type: db.actor.type }, ...body };
                db.statuses.unshift(receipt); write(db);
                if (fault(db, "statusAck")) throw new Error("status acknowledgement lost");
                return respond(receipt, 201);
            }
            const associated = /^commits\/([a-f0-9]{40})\/pulls$/.exec(route);
            if (associated) {
                const rows = db.associations?.[associated[1]] ?? [];
                const size = db.associationPageSize ?? 100;
                return respond(page(rows, size), 200, links(rows, size));
            }
            const pull = /^pulls\/(\d+)(\/commits)?$/.exec(route);
            if (pull) {
                const record = db.pulls?.[pull[1]];
                if (!record) return respond({}, 404);
                return pull[2] ? respond(page(record.commits), 200, links(record.commits)) : respond(record.details);
            }
            if (route.startsWith("commits/")) return respond({ sha: db.revision });
            const nodes = new Map(), blobs = new Map();
            for (const [name, content] of Object.entries(db.files)) {
                // Serve the real lock without copying its large bytes into every mutable API snapshot.
                const value = content === null && name === ".github/workflows/regression-bisect.lock.yml"
                    ? fs.readFileSync(path.resolve(__dirname, "..", "..", "..", name)) : Buffer.from(content, "base64");
                const parts = name.split("/");
                for (let i = 0; i < parts.length; i++) {
                    const prefix = parts.slice(0, i).join("/");
                    const entries = nodes.get(prefix) ?? new Map();
                    const leaf = i === parts.length - 1;
                    const id = leaf ? blob(value) : f.sha(parts.slice(0, i + 1).join("/"));
                    entries.set(parts[i], { path: parts[i], type: leaf ? "blob" : "tree",
                        mode: leaf ? "100644" : "040000", sha: id, ...(leaf ? { size: value.length } : {}) });
                    nodes.set(prefix, entries);
                    if (leaf) blobs.set(id, value);
                }
            }
            if (route === `git/commits/${db.revision}`) return respond({ sha: db.revision, tree: { sha: f.sha("") } });
            if (route.startsWith("git/trees/")) {
                const id = route.split("/").at(-1), entry = [...nodes].find(([key]) => f.sha(key) === id);
                assertValue(entry, "unknown tree");
                return respond({ sha: id, truncated: false, tree: [...entry[1].values()] });
            }
            if (route.startsWith("git/blobs/")) {
                const id = route.split("/").at(-1), value = blobs.get(id);
                assertValue(value, "unknown blob");
                return respond({ sha: id, encoding: "base64", size: value.length, content: value.toString("base64") });
            }
            throw new Error(`unexpected fixture API: ${options.method} ${route}`);
        },
        runContainer: async (config, options) => {
            const db = read(), unit = options.input;
            if (unit.request.kind === "source") {
                assertValue(typeof options.sourceBundle === "string" && path.isAbsolute(options.sourceBundle) &&
                    /^[a-f0-9]{64}$/.test(options.sourceBundleSha256), "source bundle path and digest required");
                const digest = createHash("sha256");
                for await (const chunk of fs.createReadStream(options.sourceBundle)) digest.update(chunk);
                assertValue(digest.digest("hex") === options.sourceBundleSha256, "source bundle digest mismatch");
                const directory = path.join(options.evidenceDirectory, "bundle-check");
                await fsp.mkdir(directory);
                const gitExecutable = JSON.parse(Buffer.from(db.files[".github/regression-bisect/deployment.json"], "base64")).gitExecutable;
                const git = args => {
                    record({ kind: "git", args });
                    const start = Date.now();
                    const result = spawnSync(gitExecutable, ["-c", "credential.helper=", "-c", "core.hooksPath=",
                        ...args], { cwd: directory, encoding: "utf8", shell: false, timeout: 60000,
                        env: { ...(process.platform === "win32" ? { SystemRoot: process.env.SystemRoot } : {}),
                            GIT_CONFIG_NOSYSTEM: "1", GIT_CONFIG_GLOBAL: path.join(directory, "no-config"),
                            GIT_ALLOW_PROTOCOL: "file", GIT_TERMINAL_PROMPT: "0" } });
                    record({ kind: "git-result", args, exitCode: result.status, durationMs: Date.now() - start });
                    assertValue(result.status === 0, `bundle verification failed: ${result.stderr}`);
                    return result.stdout.trim();
                };
                git(["init", "--bare", "--quiet", "--template="]);
                git(["bundle", "verify", options.sourceBundle]);
                const heads = git(["bundle", "list-heads", options.sourceBundle]);
                assertValue(heads === `${unit.request.target.sourceSha} refs/heads/selected`, "source bundle selected ref mismatch");
                git(["fetch", "--quiet", "--no-tags", options.sourceBundle, "refs/heads/selected"]);
                assertValue(git(["rev-parse", "FETCH_HEAD"]) === unit.request.target.sourceSha, "source bundle target mismatch");
                record({ kind: "source-bundle", size: (await fsp.stat(options.sourceBundle)).size,
                    sha256: options.sourceBundleSha256, heads, sourceSha: unit.request.target.sourceSha });
            } else assertValue(options.sourceBundle === undefined && options.sourceBundleSha256 === undefined,
                "released unit must not receive source history");
            const launch = { launchId: f.digest(unit.buildInstanceId), unitDigest: fingerprint(unit),
                imageDigest: image.split(":").at(-1) };
            let SYNTHETIC;
            if (synthetic) {
                assertValue(process.env.RB_LOCAL_TEST === "explicit-offline-fixture" && !process.env.RB_TRIAL_DIRECTORY &&
                    !process.env.GITHUB_ACTIONS, "SYNTHETIC worker is a unit fixture only");
                SYNTHETIC = synthetic.stable;
                if (unit.request.kind === "source") {
                    const args = ["show", `${unit.request.target.sourceSha}:source.txt`];
                    record({ kind: "git", args });
                    const executable = JSON.parse(Buffer.from(db.files[".github/regression-bisect/deployment.json"], "base64")).gitExecutable;
                    SYNTHETIC = execFileSync(executable, args, { cwd: db.mirrorDirectory, encoding: "utf8", timeout: 30000,
                        env: { ...process.env, GIT_CONFIG_NOSYSTEM: "1",
                            GIT_CONFIG_GLOBAL: path.join(db.mirrorDirectory, "no-config") } }).trim();
                    if (["synthetic inert source", "synthetic inert source\nnewer inert source"].includes(SYNTHETIC))
                        SYNTHETIC = "untestable";
                }
            }
            await fsp.writeFile(path.join(options.evidenceDirectory, "fixture-input.json"), json({ unit, launch, SYNTHETIC }));
            const env = process.platform === "win32" ? { SystemRoot: process.env.SystemRoot } : {};
            const started = Date.now();
            const executed = spawnSync(process.execPath, [__filename, "--worker", options.evidenceDirectory], {
                env, encoding: "utf8", timeout: 30000, maxBuffer: 16384, shell: false,
            });
            record({ kind: "worker", unit, exitCode: executed.status, durationMs: Date.now() - started,
                environmentKeys: Object.keys(env), credentialNames: Object.keys(env).filter(k => /token|secret|key|github|actions|cloud/i.test(k)) });
            assertValue(executed.status === 0, `synthetic worker failed: ${executed.error?.message ?? executed.stderr}`);
            const files = await Promise.all(executionFileNames.map(async name => {
                const bytes = await fsp.readFile(path.join(options.evidenceDirectory, name));
                return { path: name, bytes, size: bytes.length, sha256: f.digest(bytes) };
            }));
            const result = readExecutionOutput(files, unit, launch);
            const receipt = { schemaVersion: 1, status: "collected", image, launch, historicalTerminated: true,
                cleanup: [], blobs: [], evidenceDirectory: options.evidenceDirectory,
                files: files.map(({ bytes, ...descriptor }) => descriptor) };
            receipt.signature = createHmac("sha256", options.receiptKey).update(fingerprint(receipt)).digest("hex");
            if (db.faults.maliciousWorker) receipt.signature = "a".repeat(64);
            await fsp.writeFile(path.join(options.evidenceDirectory, "receipt.json"), json(receipt));
            if (fault(db, "workerAck")) throw new Error("worker collection acknowledgement lost");
            return { status: "collected", result };
        },
    };
}

async function syntheticWorker() {
    const [mode, directory, extra] = process.argv.slice(2);
    assertValue(mode === "--worker" && path.isAbsolute(directory) && !extra, "invalid synthetic worker input");
    assertValue(!Object.keys(process.env).some(k => /token|secret|key|github|actions|cloud/i.test(k)),
        "credentials entered the synthetic worker");
    const { unit, launch, SYNTHETIC } = JSON.parse(await fsp.readFile(path.join(directory, "fixture-input.json")));
    let result = { status: "untestable", outcome: { schemaVersion: 1, outcome: "untestable", reason: "setup-failure" },
        error: { message: "offline synthetic setup failure" } };
    if (SYNTHETIC) {
        const attempts = [f.attempt(SYNTHETIC), f.attempt(SYNTHETIC)];
        const reproduction = { outcome: require("../model.cjs").classifyAttempts(attempts, unit.manifest),
            cells: unit.manifest.matrix.map(c => ({ id: c.id, attempts })) };
        result = unit.request.kind === "released" ? reproduction : {
            status: "tested", actualSourceSha: unit.request.target.sourceSha,
            buildInstanceId: unit.buildInstanceId, reproduction, outcome: reproduction.outcome,
        };
    }
    const { files } = await executionEvidence(unit, launch, result, {
        evidenceDirectory: directory, scratchDirectory: directory,
    });
    for (const item of files) await fsp.writeFile(path.join(directory, item.path), item.bytes);
}

if (require.main === module) syntheticWorker().catch(error => { console.error(error.message); process.exitCode = 1; });

module.exports = { fixture, routingFixture, connect, checkpoint, seed, writeDatabase, invokeCli };
