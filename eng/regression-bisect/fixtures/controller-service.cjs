const fs = require("node:fs");
const fsp = require("node:fs/promises");
const os = require("node:os");
const path = require("node:path");
const { createHash, createHmac } = require("node:crypto");
const { execFileSync, spawnSync } = require("node:child_process");
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

async function fixture(t) {
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
    const original = await bootstrapFixture();
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
    await fsp.writeFile(serviceFile, json(db));
    const env = { GITHUB_ACTIONS: "", RB_LOCAL_TEST: "explicit-offline-fixture", RB_LOCAL_TEST_FILE: serviceFile,
        RB_TRUSTED_REVISION: context.policyRevision, GITHUB_WORKFLOW_SHA: context.workflowRevision,
        GITHUB_REPOSITORY: "dotnet/fsharp", GITHUB_REF: "refs/heads/main", GITHUB_RUN_ID: "12345", GITHUB_RUN_ATTEMPT: "1",
        RB_SIGNING_KEYS: JSON.stringify({ fixture: context.keys.fixture.toString("base64") }),
        RB_GITHUB_TOKEN: "offline-controller-token", RB_PROVISION_TOKEN: "offline-separate-maintainer-token",
        RB_STATE_DIRECTORY: path.join(directory, "state") };
    const read = () => JSON.parse(fs.readFileSync(serviceFile));
    const update = action => { const value = read(); action(value); fs.writeFileSync(serviceFile, json(value)); };
    return { directory, config, configPath, serviceFile, env, context, approval, read, update,
        ledger: () => fs.existsSync(`${serviceFile}.ledger`) ? fs.readFileSync(`${serviceFile}.ledger`, "utf8").trim().split("\n").filter(Boolean).map(JSON.parse) : [],
        nextRun() {
            update(db => { const old = db.runs[`${env.GITHUB_RUN_ID}/1`]; old.status = "completed"; old.conclusion = "cancelled";
                env.GITHUB_RUN_ID = String(Number(env.GITHUB_RUN_ID) + 1);
                db.runs[`${env.GITHUB_RUN_ID}/1`] = { ...old, id: Number(env.GITHUB_RUN_ID), status: "in_progress", conclusion: null }; });
        },
    };
}

function assertValue(value, message) { if (!value) throw new Error(message); }

async function connect(file) {
    const read = () => JSON.parse(fs.readFileSync(file));
    const write = db => fs.writeFileSync(file, json(db));
    const record = entry => fs.appendFileSync(`${file}.ledger`, JSON.stringify({ pid: process.pid, ...entry }) + "\n");
    const respond = (data, status = 200, headers = {}) => new Response(json(data), { status, headers });
    const fault = (db, name) => {
        if (!db.faults[name]) return false;
        delete db.faults[name]; write(db); return true;
    };
    return {
        clock: () => new Date(read().now),
        mirrorDirectory: read().mirrorDirectory,
        recordGit: args => record({ kind: "git", args }),
        recordGitResult: (args, result, durationMs) => record({ kind: "git-result", args,
            exitCode: result.exitCode, durationMs }),
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
            const location = new URL(url);
            assertValue(location.origin === "https://api.github.com", "unexpected API authority");
            const route = location.pathname.replace("/repos/dotnet/fsharp/", "");
            const body = options.body ? JSON.parse(options.body) : null;
            record({ kind: "api", method: options.method, route, body });
            const db = read();
            const page = rows => rows.slice((Number(location.searchParams.get("page") ?? 1) - 1) * 100,
                Number(location.searchParams.get("page") ?? 1) * 100);
            if (route.startsWith("actions/runs/")) {
                const [, , id, , attempt] = route.split("/");
                return respond(db.runs[`${id}/${attempt}`] ?? {}, db.runs[`${id}/${attempt}`] ? 200 : 404);
            }
            if (route === "issues/123") return respond(db.issue);
            if (route === "issues/123/timeline") return respond(page(db.timeline), 200, { link: "" });
            if (route === "issues/123/comments") {
                if (options.method === "GET") return respond(page(db.comments), 200, { link: "" });
                const comment = { id: db.nextId++, user: { id: db.actor.id, login: db.actor.login, type: db.actor.type },
                    performed_via_github_app: { id: db.actor.appId },
                    issue_url: "https://api.github.com/repos/dotnet/fsharp/issues/123", body: body.body };
                db.comments.push(comment); write(db);
                if (fault(db, "creationAck")) throw new Error("slot creation acknowledgement lost");
                return respond(comment, 201);
            }
            if (route.startsWith("issues/comments/")) {
                const comment = db.comments.find(c => c.id === Number(route.split("/").at(-1)));
                if (!comment) return respond({}, 404);
                if (options.method === "PATCH") {
                    comment.body = body.body; write(db);
                    if (comment.id === 457 && fault(db, "resultAck")) throw new Error("result acknowledgement lost");
                    if (fault(db, "commentAck")) throw new Error("comment acknowledgement lost");
                }
                return respond(comment);
            }
            if (route.endsWith("/statuses") && options.method === "GET")
                return respond(page(db.statuses), db.faults.history ? 503 : 200, { link: db.faults.pagination ? '<https://api.github.com/next>; rel="next"' : "" });
            if (route.startsWith("statuses/")) {
                const receipt = { id: db.nextId++, creator: { id: db.actor.id, login: db.actor.login, type: db.actor.type }, ...body };
                db.statuses.unshift(receipt); write(db);
                if (fault(db, "statusAck")) throw new Error("status acknowledgement lost");
                return respond(receipt, 201);
            }
            if (route.startsWith("commits/")) return respond({ sha: db.revision });
            const nodes = new Map(), blobs = new Map();
            for (const [name, content] of Object.entries(db.files)) {
                const parts = name.split("/");
                for (let i = 0; i < parts.length; i++) {
                    const prefix = parts.slice(0, i).join("/");
                    const entries = nodes.get(prefix) ?? new Map();
                    const leaf = i === parts.length - 1;
                    const value = Buffer.from(content, "base64");
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
            const launch = { launchId: f.digest(unit.buildInstanceId), unitDigest: fingerprint(unit),
                imageDigest: image.split(":").at(-1) };
            await fsp.writeFile(path.join(options.evidenceDirectory, "fixture-input.json"), json({ unit, launch }));
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
    const { unit, launch } = JSON.parse(await fsp.readFile(path.join(directory, "fixture-input.json")));
    const result = { status: "untestable", outcome: { schemaVersion: 1, outcome: "untestable", reason: "setup-failure" },
        error: { message: "offline synthetic setup failure" } };
    const { files } = await executionEvidence(unit, launch, result, {
        evidenceDirectory: directory, scratchDirectory: directory,
    });
    for (const item of files) await fsp.writeFile(path.join(directory, item.path), item.bytes);
}

if (require.main === module) syntheticWorker().catch(error => { console.error(error.message); process.exitCode = 1; });

module.exports = { fixture, connect };
