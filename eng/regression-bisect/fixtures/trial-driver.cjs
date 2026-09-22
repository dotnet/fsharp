// SYNTHETIC test preload, supplied only by the test launcher, never trial configuration.
if (!process.env.NODE_TEST_CONTEXT || process.env.GITHUB_ACTIONS) throw new Error("SYNTHETIC Node test runner required");
const fs = require("node:fs/promises");
const path = require("node:path");
const { setTimeout: delay } = require("node:timers/promises");
const { digest } = require("./contracts.cjs");
const state = require("../state.cjs");
const { createFileEvidenceStore } = require("../evidence-store.cjs");
const { image } = require("../container.cjs");

const directory = process.argv.at(-1);
const fault = process.env.RB_SYNTHETIC_TRIAL_FAULT;
const pause = async point => {
    if (fault !== point) return;
    await fs.writeFile(path.join(directory, "synthetic-barrier.json"), JSON.stringify({ point, pid: process.pid }));
    for (;;) await delay(1000);
};
for (const [method, point] of [["reserveWork", "reservation"], ["beginWork", "claim"]]) {
    const original = state[method];
    state[method] = async (...args) => { const result = await original(...args); await pause(point); return result; };
}
const service = require("./controller-service.cjs");
const controller = require("../controller-cli.cjs");
const connect = service.connect;
let validateStable;
service.connect = async (file, options) => {
    const api = await connect(file, { ...options, synthetic: { stable: "product-failure" } });
    api.validateStable = value => validateStable(value);
    if (fault === "stable-metadata") {
        const http = api.toolchainDependencies.http;
        api.toolchainDependencies.http = async url => {
            const response = await http(url);
            if (url.endsWith("/releases-index.json"))
                response.bytes = Buffer.from(JSON.stringify({ ...JSON.parse(response.bytes), updated: true }));
            return response;
        };
    }
    const send = api.send;
    api.send = async (url, options) => {
        const result = await send(url, options);
        if (options.method === "PATCH" && url.endsWith("/issues/comments/457")) await pause("result-write");
        return result;
    };
    const collect = api.collectObservation;
    api.collectObservation = async request => {
        const result = await collect(request), { reservation, context } = request;
        const store = createFileEvidenceStore({ repository: context.repository, runId: context.owner.runId,
            attempt: 1, directory: path.join(directory, "state", "dotnet-fsharp-123", "store") });
        const binding = { workId: reservation.id, launchId: digest(reservation.id),
            unitDigest: digest("SYNTHETIC unit evidence"), imageDigest: image.split(":").at(-1) };
        const roles = reservation.request.kind === "released" ? ["required", "optional"] : ["required"];
        result.evidence = await Promise.all(roles.map(async role => ({
            ...await store.writeEvidence(Buffer.from(`SYNTHETIC ${role}: ${reservation.id}`), binding),
            required: role === "required",
        })));
        await pause("retained-observation");
        return result;
    };
    return api;
};
const writeJson = controller.writeJson;
controller.writeJson = async (file, value) => {
    if (path.basename(file) === "delivery.json") await pause("delivery-before");
    await writeJson(file, value);
    if (path.basename(file) === "delivery.json") await pause("delivery-after");
};
const main = controller.main;
controller.main = async (args, env) => {
    validateStable = (await require("../trial.cjs").adapter(env)).validateStable;
    const fixtureEnv = { RB_LOCAL_TEST: "explicit-offline-fixture", RB_TRIAL_DIRECTORY: "",
        RB_LOCAL_TEST_FILE: path.join(directory, "api.json"), RB_STATE_DIRECTORY: env.RB_STATE_DIRECTORY };
    const previous = Object.fromEntries(Object.keys(fixtureEnv).map(k => [k, process.env[k]]));
    Object.assign(process.env, fixtureEnv);
    try { return await main(args, { ...env, ...fixtureEnv }); }
    finally {
        for (const [key, value] of Object.entries(previous))
            if (value === undefined) delete process.env[key]; else process.env[key] = value;
    }
};
