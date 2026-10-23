const { test } = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const YAML = require("yaml");

const root = path.resolve(__dirname, "..", "..");
const read = file => fs.readFileSync(path.join(root, file), "utf8");
const events = ["opened", "labeled", "unlabeled", "edited", "closed", "reopened"];
const secrets = value => [...JSON.stringify(value).matchAll(/secrets(?:\.([A-Z_][A-Z_0-9]*)|\['([A-Z_][A-Z_0-9]*)'\])/g)]
    .map(m => m[1] ?? m[2]);
const dependencies = job => [job.needs ?? []].flat();
const routingSecrets = ["REGRESSION_ROUTING_TOKEN", "REGRESSION_ROUTING_SIGNING_KEYS", "REGRESSION_DISPATCH_TOKEN"];

function immutableCheckout(job) {
    const checkout = job.steps.find(s => s.uses?.startsWith("actions/checkout@"));
    const guard = job.steps.find(s => s.name === "Require the reviewed immutable workflow revision");
    assert.ok(guard && job.steps.indexOf(guard) < job.steps.indexOf(checkout),
        "validate the SHA before executing checkout helpers");
    assert.equal(guard.env.RB_TRUSTED_REVISION, checkout.with.ref);
    assert.match(guard.run, /\^\[0-9a-f\]\{40\}\$/);
    assert.match(guard.run, /== "\$GITHUB_WORKFLOW_SHA"/);
    assert.doesNotMatch(guard.run, /\$\{\{/);
    assert.equal(checkout.with.ref, "${{ vars.REGRESSION_CONTROLLER_REVISION }}");
    assert.equal(checkout.with["persist-credentials"], false);
}

function routingRuntime(job) {
    const setup = job.steps.find(s => s.uses?.startsWith("actions/setup-node@"));
    assert.ok(setup, "routing must provision Node explicitly on the self-hosted runner");
    assert.equal(setup.uses, "actions/setup-node@48b55a011bda9f5d6aeb4c2d9c7362e8dae4041e",
        "routing runtime setup must use the reviewed action SHA");
    assert.deepEqual(setup.with, { "node-version": "24", "package-manager-cache": false },
        "routing requires Node 24 without package-manager caching");
    assert.equal(setup.if, undefined, "routing runtime setup cannot be conditional");
    assert.equal(setup["continue-on-error"] ?? false, false, "routing runtime setup must fail closed");
    assert.ok(job.steps.indexOf(setup) < job.steps.findIndex(s => s.id === "route"),
        "routing must provision Node before invoking the CLI");
}

function graph(lock) {
    const router = lock.jobs.route;
    assert.ok(router, "trusted routing job is required");
    assert.deepEqual(dependencies(router), [], "routing cannot depend on any model job");
    assert.equal(router["timeout-minutes"], 5);
    assert.deepEqual(router.concurrency, { group: "regression-bisect-discovery", "cancel-in-progress": false });
    assert.match(router.if, /github\.repository == 'dotnet\/fsharp'/);
    assert.match(router.if, /github\.ref == 'refs\/heads\/main'/);
    assert.match(router.if, /vars\.REGRESSION_ROUTING_ENABLED == 'true'/);
    assert.deepEqual(router.permissions, { contents: "read" });
    assert.equal(router.environment, "regression-routing");
    assert.equal(lock.concurrency.group, "regression-bisect-${{ github.run_id }}",
        "a model run must not serialize later deterministic sweeps");
    const checkout = router.steps.find(s => s.uses?.startsWith("actions/checkout@"));
    assert.ok(checkout, "routing requires an immutable helper checkout");
    assert.match(checkout.uses, /@[a-f0-9]{40}$/);
    assert.equal(checkout.with.repository, "dotnet/fsharp");
    assert.equal(checkout.with.ref, "${{ vars.REGRESSION_CONTROLLER_REVISION }}");
    assert.equal(checkout.with["persist-credentials"], false);
    immutableCheckout(router);
    routingRuntime(router);
    const route = router.steps.find(s => s.id === "route");
    assert.equal(route.run, "node eng/regression-bisect/routing-cli.cjs");
    assert.equal(route.env.RB_ACTION_INPUTS, "${{ toJSON(inputs) }}");
    assert.equal(route.env.RB_TRUSTED_REVISION, checkout.with.ref);
    assert.deepEqual(new Set(secrets(router)), new Set(routingSecrets));
    assert.deepEqual(new Set(secrets(route)), new Set(routingSecrets));
    assert.deepEqual(secrets({ env: router.env, steps: router.steps.filter(s => s !== route) }), [],
        "only the routing CLI step receives routing secrets");
    assert.equal(router.outputs.proposals, "${{ steps.route.outputs.proposals }}");
    assert.ok(dependencies(lock.jobs.activation).includes("route"));
    assert.match(lock.jobs.activation.if, /needs\.route\.outputs\.proposals != '\[\]'/);
    assert.ok(dependencies(lock.jobs.agent).includes("route"));
    assert.deepEqual(lock.permissions, {});
    assert.deepEqual(secrets({ env: lock.env, secrets: lock.secrets }), [],
        "workflow-level secrets must not reach the model through inheritance");
    assert.ok(/"GITHUB_READ_ONLY":\s*"1"/.test(lock.jobs.agent.steps.find(s => s.id === "start-mcp-gateway").run),
        "the model's GitHub MCP server must enforce read-only access");
    const agentRun = lock.jobs.agent.steps.find(s => s.name === "Execute GitHub Copilot CLI").run;
    assert.ok(!agentRun.includes("--allow-all-tools") && !agentRun.includes("--allow-tool shell"),
        "the analysis model must not execute shell commands");
    for (const name of ["agent", "detection"]) {
        const run = lock.jobs[name].steps.find(s => s.name === "Execute GitHub Copilot CLI").run;
        for (const flag of ["--deny-tool shell", "--deny-tool task", "--no-custom-instructions"])
            assert.ok(run.includes(flag), `${name} must constrain shell, delegation and inherited instructions`);
    }
    assert.deepEqual(lock.jobs.agent.steps.filter(s => s["timeout-minutes"]).map(s => s["timeout-minutes"]), [15]);
    for (const [name, job] of Object.entries(lock.jobs)) {
        // v0.76.1 bounds the engine step; framework jobs use Actions' finite 360-minute default.
        const timeout = job["timeout-minutes"] ?? 360;
        assert.ok(Number.isInteger(timeout) && timeout > 0 && timeout <= 360, `${name} requires a finite timeout`);
        const permissions = job.permissions ?? lock.permissions;
        assert.ok(permissions && typeof permissions === "object", `${name} cannot inherit broad permissions`);
        assert.ok(Object.values(permissions).every(p => ["read", "none"].includes(p)), `${name} must be read-only`);
        if (name !== "route") {
            const allowed = { activation: ["COPILOT_GITHUB_TOKEN"], agent: ["COPILOT_GITHUB_TOKEN", "GITHUB_TOKEN"],
                detection: ["COPILOT_GITHUB_TOKEN"], conclusion: ["GITHUB_TOKEN"], safe_outputs: ["GITHUB_TOKEN"] };
            assert.deepEqual(new Set(secrets(job)), new Set(allowed[name] ?? []),
                `${name} must not receive write or signing secrets`);
            assert.doesNotMatch(JSON.stringify(job), /RB_|ACTIONS_RUNTIME_TOKEN/);
            assert.ok(["ubuntu-latest", "ubuntu-slim"].includes(job["runs-on"]),
                `${name} must not share the trusted persistent runner`);
            assert.equal(job.environment, undefined, `${name} cannot receive deployment environment secrets`);
        }
        for (const step of job.steps ?? []) {
            if (step.uses?.startsWith("actions/checkout@")) assert.equal(step.with["persist-credentials"], false);
            if (step.uses && !step.uses.startsWith("./")) assert.match(step.uses, /@[a-f0-9]{40}$/);
        }
    }
    assert.doesNotMatch(JSON.stringify(lock), /secrets:inherit|REGRESSION_CONTROLLER_TOKEN|REGRESSION_CONTROLLER_SIGNING_KEYS/);
    const handlers = lock.jobs.safe_outputs.steps.find(s => s.id === "process_safe_outputs");
    assert.deepEqual(Object.keys(JSON.parse(handlers.env.GH_AW_SAFE_OUTPUTS_HANDLER_CONFIG)).sort(),
        ["missing_data", "missing_tool", "noop", "report_incomplete", "upload_artifact"]);
    for (const step of lock.jobs.conclusion.steps)
        for (const [key, value] of Object.entries(step.env ?? {}))
            if (/(?:CREATE_ISSUE|REPORT_AS_ISSUE)$/.test(key)) assert.equal(value, "false", `${key} cannot publish`);
}

test("dedicated source and genuine matching-version lock enforce the routing DAG", () => {
    const text = read(".github/workflows/regression-bisect.md");
    const source = YAML.parse(text.split(/^---\s*$/m)[1]);
    const compiled = read(".github/workflows/regression-bisect.lock.yml");
    const lock = YAML.parse(compiled);
    assert.match(compiled, /"compiler_version":"v0\.76\.1","strict":true/);
    const defaults = YAML.parse(read(".github/workflows/shared/model-defaults.md").split(/^---\s*$/m)[1]);
    assert.deepEqual(source.models, defaults.models);
    assert.equal(source.engine.id, defaults.engine.id);
    assert.equal(source.engine.model, defaults.engine.model);
    assert.deepEqual(source.engine.env, defaults.engine.env);
    assert.deepEqual(source.engine.args, [...defaults.engine.args,
        "--deny-tool", "shell", "--deny-tool", "task", "--no-custom-instructions"]);
    for (const workflow of [source, lock]) {
        assert.deepEqual(workflow.on.issues.types, events);
        assert.deepEqual(Object.keys(workflow.on.workflow_dispatch.inputs).filter(k => k !== "aw_context"), ["issue"]);
        assert.equal(workflow.on.workflow_dispatch.inputs.issue.type, "number");
        assert.equal(workflow.on.workflow_dispatch.inputs.issue.required, true);
        assert.deepEqual(workflow.on.schedule, [{ cron: "17 * * * *" }]);
    }
    assert.equal(source.checkout, false);
    assert.equal(source.on.roles, "all");
    assert.equal(source.on["status-comment"], false);
    assert.equal(source.on.reaction, "none");
    assert.deepEqual(lock.on.workflow_dispatch.inputs.aw_context,
        { default: "", description: "Agent caller context (used internally by Agentic Workflows).", required: false, type: "string" });
    assert.deepEqual(Object.keys(source.tools), ["github", "edit", "bash", "cli-proxy"]);
    assert.deepEqual(source.tools.bash, []);
    assert.equal(source.tools["cli-proxy"], false);
    assert.deepEqual(source.tools.github.toolsets, ["issues", "repos", "pull_requests"]);
    assert.equal(source.tools.github["github-token"], "${{ secrets.GITHUB_TOKEN }}");
    assert.equal(source["safe-outputs"]["report-failure-as-issue"], false);
    assert.equal(source["safe-outputs"].noop["report-as-issue"], false);
    for (const type of ["missing-tool", "missing-data", "report-incomplete"])
        assert.equal(source["safe-outputs"][type]["create-issue"], false);
    assert.deepEqual(source["safe-outputs"]["upload-artifact"],
        { "max-uploads": 1, "retention-days": 7, "max-size-bytes": 16384, "allowed-paths": ["proposal.md"] });
    assert.ok(Object.keys(source["safe-outputs"]).every(k =>
        ["activation-comments", "report-failure-as-issue", "noop", "github-token", "missing-tool",
            "missing-data", "report-incomplete", "upload-artifact"].includes(k)));
    routingRuntime(source.jobs.route);
    graph(lock);
    assert.equal(require("./routing.cjs").workflowPath, ".github/workflows/regression-bisect.lock.yml");
    const schema = JSON.parse(read(".github/regression-bisect/discovery.schema.json"));
    assert.equal(schema.properties.workflowPath.const, require("./routing.cjs").workflowPath);
    for (const file of ["eng/regression-bisect/routing.cjs", "eng/regression-bisect/routing-cli.cjs",
        "eng/regression-bisect/fixtures/routing-actions.cjs"])
        assert.doesNotMatch(read(file), /regression-bisect-discovery\.yml/);
});

test("unsafe DAG mutations fail specific structural assertions", () => {
    const lock = YAML.parse(read(".github/workflows/regression-bisect.lock.yml"));
    for (const [mutate, reason] of [
        [w => { delete w.jobs.route; }, /trusted routing job is required/],
        [w => { w.jobs.route.needs = ["agent"]; }, /routing cannot depend on any model job/],
        [w => { w.jobs.route.steps = w.jobs.route.steps.filter(s => !s.uses?.startsWith("actions/setup-node@")); },
            /routing must provision Node explicitly/],
        [w => { const steps = w.jobs.route.steps;
            steps.push(...steps.splice(steps.findIndex(s => s.uses?.startsWith("actions/setup-node@")), 1)); },
            /routing must provision Node before invoking the CLI/],
        [w => { w.jobs.route.steps.find(s => s.uses?.startsWith("actions/setup-node@")).uses = "actions/setup-node@v6"; },
            /routing runtime setup must use the reviewed action SHA/],
        [w => { w.jobs.route.steps.find(s => s.uses?.startsWith("actions/setup-node@")).with["node-version"] = "22"; },
            /routing requires Node 24 without package-manager caching/],
        [w => { w.jobs.route.steps.find(s => s.uses?.startsWith("actions/setup-node@")).if = "false"; },
            /routing runtime setup cannot be conditional/],
        [w => { w.jobs.route.steps.find(s => s.uses?.startsWith("actions/setup-node@"))["continue-on-error"] = true; },
            /routing runtime setup must fail closed/],
        [w => { w.jobs.agent.env = { TOKEN: "${{ secrets.REGRESSION_DISPATCH_TOKEN }}" }; }, /agent must not receive write or signing secrets/],
        [w => { w.jobs.agent.env = { TOKEN: "${{ secrets['REGRESSION_ROUTING_TOKEN'] }}" }; }, /agent must not receive write or signing secrets/],
        [w => { w.jobs.worker = { "timeout-minutes": 10, permissions: {}, env: {
            TOKEN: "${{ secrets.REGRESSION_CONTROLLER_TOKEN }}" } }; }, /worker must not receive write or signing secrets/],
        [w => { w.jobs.agent.permissions.issues = "write"; }, /agent must be read-only/],
        [w => { w.jobs.agent["runs-on"] = w.jobs.route["runs-on"]; }, /agent must not share the trusted persistent runner/],
        [w => { const step = w.jobs.agent.steps.find(s => s.name === "Execute GitHub Copilot CLI");
            step.run = step.run.replace("--deny-tool shell", "--allow-tool shell"); }, /analysis model must not execute shell commands/],
        [w => { w.env = { TOKEN: "${{ secrets.REGRESSION_ROUTING_TOKEN }}" }; }, /workflow-level secrets must not reach the model/],
    ]) {
        const changed = structuredClone(lock);
        mutate(changed);
        assert.throws(() => graph(changed), reason);
    }
    for (const status of ["failure", "skipped", "noop", "missing"]) {
        const changed = structuredClone(lock);
        if (status === "missing") delete changed.jobs.agent;
        else changed.jobs.agent.if = "false";
        assert.deepEqual(dependencies(changed.jobs.route), []);
        assert.equal(changed.jobs.route.steps.find(s => s.id === "route").run,
            "node eng/regression-bisect/routing-cli.cjs");
    }
});

test("controller remains the sole one-unit executor and fixed-slot publisher", () => {
    const controller = YAML.parse(read(".github/workflows/regression-bisect-controller.yml"));
    assert.deepEqual(Object.keys(controller.on), ["workflow_dispatch"]);
    assert.deepEqual(controller.on.workflow_dispatch.inputs.mode.options, ["bootstrap", "advance", "publish"]);
    assert.equal(controller.concurrency.group, "regression-bisect-dotnet-fsharp-${{ inputs.issue }}");
    assert.equal(controller.concurrency["cancel-in-progress"], false);
    assert.equal(controller.jobs.controller.environment, "regression-controller");
    assert.match(controller.jobs.controller.if, /REGRESSION_CONTROLLER_ENABLED == 'true'/);
    assert.deepEqual(new Set(secrets(controller)), new Set(["REGRESSION_CONTROLLER_TOKEN", "REGRESSION_CONTROLLER_SIGNING_KEYS"]));
    immutableCheckout(controller.jobs.controller);
    assert.equal(controller.jobs.controller.steps.at(-1).uses, "./eng/regression-bisect");
    assert.equal(YAML.parse(read("eng/regression-bisect/action.yml")).runs.main, "controller-cli.cjs");
});

test("scoped validation is not a second production scheduler and has no execution secrets", () => {
    const validation = YAML.parse(read(".github/workflows/regression-bisect-validation.yml"));
    assert.deepEqual(Object.keys(validation.on), ["pull_request", "push"]);
    assert.ok(validation.on.pull_request.paths.includes(".github/workflows/regression-bisect*"));
    assert.ok(validation.on.pull_request.paths.includes("eng/regression-bisect/**"));
    assert.deepEqual(validation.on.push.branches, ["idea/dm-912"]);
    assert.deepEqual(validation.on.push.paths, validation.on.pull_request.paths);
    assert.deepEqual(validation.permissions, { contents: "read" });
    assert.deepEqual(secrets(validation), []);
    assert.equal(validation.concurrency.group,
        "regression-bisect-validation-${{ github.event.pull_request.number || github.ref }}");
    assert.equal(validation.jobs.validate["timeout-minutes"], 45);
    assert.equal(validation.jobs.validate.steps.find(s => s.uses?.startsWith("actions/checkout@")).with["fetch-depth"], 0,
        "controlled source fixtures need the reviewed historical base, not a shallow checkout");
    assert.deepEqual(validation.jobs.validate.steps.filter(s => s["timeout-minutes"]).map(s => s["timeout-minutes"]), [15, 30]);
});

test("isolated fleet compilation preserves the shared action registry and full generated-file guard", () => {
    const validation = YAML.parse(read(".github/workflows/regression-bisect-validation.yml"));
    const steps = validation.jobs.validate.steps;
    const compile = steps.find(step => step.name === "Compile twice with the isolated fleet compiler").run;
    assert.match(compile, /\.github\\aw\\actions-lock\.json/);
    assert.match(compile, /\$actionsLockBytes = \[IO\.File\]::ReadAllBytes\(\$actionsLock\)/);
    assert.match(compile, /finally\s*\{\s*\[IO\.File\]::WriteAllBytes\(\$actionsLock, \$actionsLockBytes\)/);
    assert.ok(compile.indexOf("ReadAllBytes") < compile.indexOf("foreach ($pass in 1, 2)"));
    assert.match(compile, /Compiled lock differs/);
    const suite = steps.find(step => step.name === "Offline scoped suite").run;
    assert.match(suite, /git diff --exit-code\s*\n/);
    assert.match(suite, /git diff --check/);
});

test("validation runs the production containment smoke on an ephemeral Linux runner", () => {
    const validation = YAML.parse(read(".github/workflows/regression-bisect-validation.yml"));
    const linux = validation.jobs.containment;
    assert.ok(linux, "physical containment needs a Linux CI job, not an operator-provisioned VM");
    assert.equal(linux["runs-on"], "ubuntu-24.04");
    assert.equal(linux["continue-on-error"] ?? false, false);
    assert.equal(linux.steps.find(s => s.uses?.startsWith("actions/checkout@")).with["persist-credentials"], false);
    assert.equal(linux.steps.find(s => s.uses?.startsWith("actions/setup-node@")).with["node-version"], "24");
    const image = read("eng/regression-bisect/container.cjs").match(/^const image = "([^"]+)";/m)[1];
    assert.ok(linux.steps.find(s => s.name === "Provision the pinned image").run.includes(image));
    assert.ok(linux.steps.find(s => s.name === "Write smoke configuration").run.includes(image));
    const smoke = linux.steps.find(s => s.name === "Run real Linux containment");
    assert.equal(smoke.run.trim(),
        'node eng/regression-bisect/containment.smoke.cjs --run "$RUNNER_TEMP/regression-containment-config.json" "$RUNNER_TEMP/regression-containment"');
    assert.equal(smoke["continue-on-error"] ?? false, false);
    assert.deepEqual(secrets(linux), []);
    const artifacts = linux.steps.find(s => s.uses?.startsWith("actions/upload-artifact@"));
    assert.equal(artifacts.if, "always()");
    assert.equal(artifacts.with["if-no-files-found"], "error");
    assert.ok(artifacts.with.path.includes("${{ runner.temp }}/regression-containment"));
});

test("physical source acceptance is opt-in, isolated and distinct from the investigation", () => {
    const validation = YAML.parse(read(".github/workflows/regression-bisect-validation.yml"));
    const job = validation.jobs["source-worker"];
    assert.equal(job.if,
        "github.event_name == 'push' && (contains(github.event.head_commit.message, '[source-acceptance]') || contains(github.event.head_commit.message, '[worker-acceptance]'))");
    assert.deepEqual(job.needs, ["validate", "containment"]);
    assert.equal(job["runs-on"], "ubuntu-24.04");
    assert.equal(job["timeout-minutes"], 130);
    assert.equal(job["continue-on-error"] ?? false, false);
    assert.deepEqual(secrets(job), []);
    const checkout = job.steps.find(s => s.uses?.startsWith("actions/checkout@"));
    assert.equal(checkout.with["persist-credentials"], false);
    assert.equal(checkout.with["fetch-depth"], 0);
    const containment = job.steps.findIndex(s => s.name === "Verify this source worker's containment");
    const source = job.steps.findIndex(s => s.name === "Build and test the frozen source inside the production worker");
    assert.ok(containment >= 0 && source > containment);
    assert.ok(job.steps[source].run.includes(
        'node eng/regression-bisect/source-worker.smoke.cjs --controller "$RUNNER_TEMP/regression-source-config.json" "$RUNNER_TEMP/regression-source-worker" "$RUNNER_TEMP/regression-source-containment"'));
    assert.deepEqual(job.steps[source].env, {
        RB_CONTROLLER_ACCEPTANCE: "${{ contains(github.event.head_commit.message, '[worker-acceptance]') }}",
    });
    const artifacts = job.steps.find(s => s.uses?.startsWith("actions/upload-artifact@"));
    assert.equal(artifacts.if, "always()");
    assert.equal(artifacts.with["if-no-files-found"], "error");
    assert.ok(artifacts.with.path.includes("${{ runner.temp }}/regression-source-worker"));
    assert.ok(job.steps.every(s => s["continue-on-error"] !== true));
});

test("controlled delivery uses the existing one-unit CLI with six explicit reservations after physical acceptance", () => {
    const job = YAML.parse(read(".github/workflows/regression-bisect-validation.yml")).jobs["source-worker"];
    const steps = job.steps.filter(s => s.if?.includes("[trial-acceptance]"));
    assert.equal(steps.length, 11);
    assert.ok(steps.every(s => s.if === "contains(github.event.head_commit.message, '[trial-acceptance]')"));
    const setup = steps[0].run;
    for (const gate of ["accepted.status", "accepted.revision", "accepted.controllerAcceptance",
        "accepted.projectMatrixAcceptance", "accepted.immutableProducerAcceptance"]) assert.ok(setup.includes(gate));
    assert.ok(setup.includes("a23fa5514d49566559bdcc7479fbe00ceb01b577"));
    const mask = setup.indexOf('console.log(`::add-mask::${key.toString("base64")}`)');
    assert.ok(mask >= 0 && mask < setup.indexOf("fs.appendFile(process.env.GITHUB_ENV"),
        "generated signing keys must be masked before Actions logs later step environments");
    assert.ok(!setup.includes("--advance"), "input preparation must not execute a worker");
    const units = steps.filter(s => s.name.startsWith("Trial unit "));
    assert.equal(units.length, 6);
    for (const unit of units) {
        assert.equal(unit.run.match(/trial\.cjs --advance/g)?.length, 2, unit.name);
        assert.doesNotMatch(unit.run, /\b(?:for|while|until)\b/);
    }
    const commands = steps.map(s => s.run).join("\n");
    assert.equal(commands.match(/trial\.cjs --prepare/g)?.length, 1);
    assert.equal(commands.match(/trial\.cjs --advance/g)?.length, 22);
    assert.ok(commands.includes("resultAck=true"));
    assert.ok(commands.includes('test "$code" -eq 1'));
    assert.equal(commands.match(/trial\.cjs --verify/g)?.length, 1);
    assert.doesNotMatch(commands, /RB_LOCAL_TEST|NODE_OPTIONS|--restart|git bisect/);
});
