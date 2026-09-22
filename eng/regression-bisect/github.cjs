const { createHash } = require("node:crypto");
const { validateRepositoryPath } = require("./model.cjs");
const { hash, requireValue } = require("./toolchain-io.cjs");

const sha = value => typeof value === "string" && /^[a-f0-9]{40}$/.test(value);
const positive = value => Number.isSafeInteger(value) && value > 0;
const prefix = "/repos/{owner}/{repo}/";
const routes = new Map([
    ["GET issues", "page per_page state labels sort direction"],
    ["GET issues/{issue_number}", ""],
    ["GET issues/{issue_number}/comments", "page per_page"],
    ["GET issues/{issue_number}/timeline", "page per_page"],
    ["GET pulls/{pull_number}", ""],
    ["GET pulls/{pull_number}/commits", "page per_page"],
    ["GET commits/{commit_sha}/pulls", "page per_page"],
    ["GET commits/{ref}/statuses", "page per_page"],
    ["GET actions/runs/{run_id}/attempts/{attempt_number}", ""],
    ["GET issues/comments/{comment_id}", ""],
    ["PATCH issues/comments/{comment_id}", "body"],
    ["POST statuses/{sha}", "state context description target_url"],
    ["GET git/commits/{commit_sha}", ""],
    ["GET git/trees/{tree_sha}", ""],
    ["GET git/blobs/{file_sha}", ""],
    ["GET commits/{ref}", ""],
]);

function createGithubApi(config, send = fetch) {
    const { repository, issueNumber, checkpointCommentId, resultCommentId, anchorSha, token } = { ...config };
    requireValue(repository === "dotnet/fsharp" && positive(issueNumber) && positive(checkpointCommentId) &&
        positive(resultCommentId) && checkpointCommentId !== resultCommentId && sha(anchorSha) &&
        typeof token === "string" && token.length > 0 && !/\s/.test(token), "invalid trusted GitHub API configuration");
    async function request(route, params) {
        requireValue(typeof route === "string" && params && Object.getPrototypeOf(params) === Object.prototype, "invalid API request");
        const [method, template, extra] = route.split(" ");
        const suffix = template?.startsWith(prefix) ? template.slice(prefix.length) : "";
        const fields = routes.get(`${method} ${suffix}`);
        requireValue(extra === undefined && fields !== undefined, "unsupported GitHub API route");
        const placeholders = [...template.matchAll(/\{(\w+)\}/g)].map(m => m[1]);
        const permitted = [...placeholders, ...fields.split(" ").filter(Boolean)];
        requireValue(Object.keys(params).every(k => permitted.includes(k)), "unexpected GitHub API parameters");
        for (const key of placeholders) {
            const value = params[key];
            const valid = ["owner", "repo"].includes(key) ? typeof value === "string" &&
                /^[a-zA-Z0-9_-][a-zA-Z0-9_.-]{0,99}$/.test(value) && !value.endsWith(".") :
                ["sha", "ref", "commit_sha", "tree_sha", "file_sha"].includes(key) ? sha(value) :
                    key === "run_id" ? typeof value === "string" && /^[1-9]\d{0,19}$/.test(value) : positive(value);
            requireValue(valid, `invalid API path parameter: ${key}`);
        }
        const ownRepository = `${params.owner}/${params.repo}` === repository;
        requireValue(ownRepository || method === "GET" && ["issues/{issue_number}", "pulls/{pull_number}"].includes(suffix),
            "forbidden API repository");
        if (params.comment_id !== undefined)
            requireValue([checkpointCommentId, resultCommentId].includes(params.comment_id), "forbidden API comment destination");
        if (method === "PATCH") requireValue(typeof params.body === "string" && Buffer.byteLength(params.body) < 48 * 1024,
            "invalid bounded comment body");
        if (suffix === "commits/{ref}/statuses") requireValue(params.ref === anchorSha, "forbidden freshness anchor");
        if (method === "POST") requireValue(params.sha === anchorSha && params.state === "success" &&
            params.context === `regression-bisect/${issueNumber}/${checkpointCommentId}` &&
            /^checkpoint:(0|[1-9]\d*):[a-f0-9]{64}$/.test(params.description) &&
            new RegExp(`^https://github\\.com/dotnet/fsharp/issues/${issueNumber}#checkpoint-[\\w-]{1,64}:[a-f0-9]{64}$`).test(params.target_url),
        "invalid freshness status destination or payload");
        const url = new URL(template.replace(/\{(\w+)\}/g, (_, key) => String(params[key])), "https://api.github.com");
        const body = {};
        for (const key of fields.split(" ").filter(Boolean)) {
            if (!Object.hasOwn(params, key)) continue;
            const value = params[key];
            if (method !== "GET") body[key] = value;
            else {
                requireValue(key === "page" ? positive(value) : key === "per_page" ? positive(value) && value <= 100 :
                    value === { state: "open", labels: "Regression", sort: "created", direction: "asc" }[key], "invalid API query parameter");
                url.searchParams.set(key, String(value));
            }
        }
        const signal = AbortSignal.any([AbortSignal.timeout(30000), ...(config.signal ? [config.signal] : [])]);
        signal.throwIfAborted();
        const response = await send(url.href, {
            method, redirect: "error", signal, headers: {
                Authorization: `Bearer ${token}`, Accept: "application/vnd.github+json", "X-GitHub-Api-Version": "2022-11-28",
                "Accept-Encoding": "identity", "User-Agent": "dotnet-fsharp-regression-controller", "Content-Type": "application/json",
            }, ...(method === "GET" ? {} : { body: JSON.stringify(body) }),
        });
        const chunks = [];
        let length = 0;
        try {
            requireValue(response.status === (method === "POST" ? 201 : 200), `GitHub API ${method} ${suffix} failed (${response.status})`);
            requireValue(["identity", null].includes(response.headers.get("content-encoding")), "compressed GitHub API response forbidden");
            const expected = response.headers.get("content-length");
            const maxBytes = 4 * 1024 * 1024;
            requireValue(expected === null || /^\d+$/.test(expected) && Number(expected) <= maxBytes, "invalid API response content length");
            requireValue(response.body, "missing GitHub API response body");
            for await (const bytes of response.body) {
                signal.throwIfAborted();
                length += bytes.length;
                requireValue(length <= maxBytes, "GitHub API response exceeds byte limit");
                chunks.push(Buffer.from(bytes));
            }
            signal.throwIfAborted();
            requireValue(expected === null || length === Number(expected), "GitHub API response content length mismatch");
            return { status: response.status, headers: { link: response.headers.get("link") ?? undefined },
                data: JSON.parse(new TextDecoder("utf-8", { fatal: true }).decode(Buffer.concat(chunks))) };
        } finally {
            if (response.body && !response.body.locked) await response.body.cancel();
        }
    }
    const own = { owner: "dotnet", repo: "fsharp" };
    return {
        request,
        getComment: async id => (await request(`GET ${prefix}issues/comments/{comment_id}`, { ...own, comment_id: id })).data,
        updateComment: async (id, body) => (await request(`PATCH ${prefix}issues/comments/{comment_id}`, { ...own, comment_id: id, body })).data,
        getRun: async (id, attempt) => (await request(`GET ${prefix}actions/runs/{run_id}/attempts/{attempt_number}`, {
            ...own, run_id: id, attempt_number: attempt,
        })).data,
    };
}

// This authority is deliberately unavailable to the execution/publishing transport.
function createProvisioningApi(config, send = fetch) {
    const api = createGithubApi(config, send);
    const write = async (suffix, body) => {
        requireValue(Buffer.byteLength(JSON.stringify(body)) < 48 * 1024, "provisioning body exceeds limit");
        const response = await send(`https://api.github.com/repos/dotnet/fsharp/${suffix}`, {
            method: "POST", redirect: "error", signal: AbortSignal.timeout(30000),
            headers: { Authorization: `Bearer ${config.token}`, Accept: "application/vnd.github+json",
                "X-GitHub-Api-Version": "2022-11-28", "Content-Type": "application/json" },
            body: JSON.stringify(body),
        });
        requireValue(response.status === 201 && response.body, `provisioning write failed (${response.status})`);
        const chunks = [];
        let size = 0;
        for await (const chunk of response.body) {
            size += chunk.length;
            requireValue(size < 48 * 1024, "provisioning response exceeds limit");
            chunks.push(Buffer.from(chunk));
        }
        return JSON.parse(Buffer.concat(chunks));
    };
    return { ...api,
        createSlot: body => write(`issues/${config.issueNumber}/comments`, { body }),
        markProvisioning: (description, target_url) => {
            requireValue(/^provision:[a-f0-9]{64}$/.test(description) &&
                new RegExp(`^https://github\\.com/dotnet/fsharp/issues/${config.issueNumber}#provision-[\\w-]+:[a-f0-9]{64}$`).test(target_url),
            "invalid provisioning receipt");
            return write(`statuses/${config.anchorSha}`, { state: "success",
                context: `regression-provision/${config.issueNumber}`, description, target_url });
        },
    };
}

// Only the trusted routing process receives this separate actions-write credential.
function createDispatcher({ token, signal }, send = fetch) {
    requireValue(typeof token === "string" && token.length > 0 && !/\s/.test(token), "invalid dispatcher credential");
    return async input => {
        requireValue(input && Object.keys(input).sort().join(" ") === "issue mode routingGeneration routingKey" &&
            positive(input.issue) && ["bootstrap", "advance", "publish"].includes(input.mode) &&
            typeof input.routingKey === "string" && /^[a-f0-9]{64}$/.test(input.routingKey) &&
            typeof input.routingGeneration === "string" && /^(?:-1|0|[1-9]\d*)$/.test(input.routingGeneration) &&
            Number.isSafeInteger(Number(input.routingGeneration)) &&
            (input.mode === "bootstrap") === (input.routingGeneration === "-1"), "invalid fixed dispatch input");
        const deadline = AbortSignal.any([AbortSignal.timeout(30000), ...(signal ? [signal] : [])]);
        deadline.throwIfAborted();
        const response = await send("https://api.github.com/repos/dotnet/fsharp/actions/workflows/regression-bisect-controller.yml/dispatches", {
            method: "POST", redirect: "error", signal: deadline,
            headers: { Authorization: `Bearer ${token}`, Accept: "application/vnd.github+json",
                "X-GitHub-Api-Version": "2022-11-28", "Content-Type": "application/json" },
            body: JSON.stringify({ ref: "main", inputs: input }),
        });
        try {
            deadline.throwIfAborted();
            requireValue(response.status === 204, `fixed controller dispatch failed (${response.status})`);
        } finally { if (response.body && !response.body.locked) await response.body.cancel(); }
    };
}

function createApprovalReader({ api, revision, issueNumber }) {
    requireValue(typeof api?.request === "function" && sha(revision) && positive(issueNumber), "invalid trusted approval reader configuration");
    const get = async (route, params) => {
        const response = await api.request(`GET ${prefix}git/${route}`, { owner: "dotnet", repo: "fsharp", ...params });
        requireValue(response && (!response.status || response.status === 200) && response.data, "incomplete pinned Git object response");
        return response.data;
    };
    const read = async (file, maxBytes, optional = false) => {
        validateRepositoryPath(file);
        const commit = await get("commits/{commit_sha}", { commit_sha: revision });
        requireValue(commit.sha === revision && sha(commit.tree?.sha), "pinned approval commit identity mismatch");
        let id = commit.tree.sha;
        const parts = file.split("/");
        for (const [index, part] of parts.entries()) {
            const tree = await get("trees/{tree_sha}", { tree_sha: id });
            requireValue(tree.sha === id && tree.truncated === false && Array.isArray(tree.tree), "truncated or substituted approval tree");
            const entries = tree.tree.filter(e => e.path === part);
            if (optional && entries.length === 0) return null;
            requireValue(entries.length === 1 && sha(entries[0].sha), "missing or ambiguous reviewed Git asset");
            const entry = entries[0];
            if (index < parts.length - 1) {
                requireValue(entry.type === "tree" && entry.mode === "040000", "reviewed path is not a regular Git tree");
                id = entry.sha;
                continue;
            }
            requireValue(entry.type === "blob" && ["100644", "100755"].includes(entry.mode), "reviewed asset is not a regular Git blob");
            requireValue(Number.isSafeInteger(entry.size) && entry.size >= 0 && entry.size <= maxBytes, "reviewed Git asset exceeds size limit");
            const blob = await get("blobs/{file_sha}", { file_sha: entry.sha });
            requireValue(blob.sha === entry.sha && blob.encoding === "base64" && blob.size === entry.size &&
                typeof blob.content === "string" && blob.content.length <= maxBytes * 2 + 4, "invalid reviewed Git blob identity or size");
            const encoded = blob.content.replace(/\r?\n/g, "");
            const bytes = Buffer.from(encoded, "base64");
            requireValue(bytes.length === blob.size && bytes.toString("base64") === encoded &&
                createHash("sha1").update(`blob ${bytes.length}\0`).update(bytes).digest("hex") === entry.sha,
            "reviewed Git blob content identity mismatch");
            return bytes;
        }
    };
    return {
        readTrustedFile: read,
        readOptionalTrustedFile: (file, maxBytes) => read(file, maxBytes, true),
        readApproval: async reference => {
            requireValue(reference && Object.keys(reference).sort().join(" ") === "path revision" &&
                reference.revision === revision && reference.path === `.github/regression-bisect/approved/${issueNumber}.json`,
            "approval path or immutable revision mismatch");
            return read(reference.path, 48 * 1024 - 1);
        },
        readSource: async ({ path, sha256 }) => {
            requireValue(typeof sha256 === "string" && /^[a-f0-9]{64}$/.test(sha256), "invalid reviewed asset digest");
            const bytes = await read(path, 1024 * 1024);
            requireValue(hash(bytes) === sha256, "reviewed raw asset digest mismatch");
            return bytes;
        },
    };
}

module.exports = { createGithubApi, createProvisioningApi, createApprovalReader, createDispatcher };
