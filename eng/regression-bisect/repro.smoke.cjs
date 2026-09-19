const fs = require("node:fs/promises");
const path = require("node:path");
const { runReproduction } = require("./repro.cjs");
const { createFixture } = require("./fixtures/smoke.cjs");
const { requireValue, isolatedEnvironment } = require("./toolchain-io.cjs");
const { execute } = require("./repro-process.cjs");

async function freezeProducer(root, config, results) {
    const sourceDirectory = path.join(root, "producer");
    const manifest = await createFixture(sourceDirectory, { kind: "project", tfm: config.released.tfm, scenario: "producer" });
    const result = await runReproduction({
        manifest, sourceDirectory, workDirectory: path.join(root, "executions"),
        product: config.released.product, toolchain: config.released.toolchain, helper: config.helper,
        sampleRuntimes: config.released.sampleRuntimes,
    });
    results.push({ id: "freeze-released-producer", expected: "pass", result });
    requireValue(result.outcome.outcome === "pass", `producer preparation failed: ${result.error ?? result.outcome.reason}`);
    const row = result.cells[0].evidence[0].projects[0];
    const nupkg = path.join(path.dirname(path.dirname(row.targetPath)), "FrozenProducer.1.0.0.nupkg");
    const env = isolatedEnvironment(path.join(root, "helper"), config.released.toolchain.requested.platform);
    for (const folder of [env.HOME, env.TEMP, env.APPDATA, env.LOCALAPPDATA]) await fs.mkdir(folder, { recursive: true });
    const inspected = await execute(config.helper.executable,
        ["exec", "--fx-version", config.helper.runtimeVersion, config.helper.dll, "inspect", row.targetPath, nupkg],
        { cwd: root, env, timeoutMs: 120000, maxBytes: 1024 * 1024 });
    requireValue(inspected.complete && inspected.exitCode === 0, `producer inspection failed: ${inspected.stderr}`);
    const [dll, pkg] = JSON.parse(inspected.stdout);
    const compiler = config.released.product.files.find(f => f.role === "compiler");
    return {
        role: "immutable-producer",
        identity: { name: "FrozenProducer", version: "1.0.0", sha256: dll.sha256 },
        compiler: { version: compiler.assemblyVersion, sha256: compiler.sha256 },
        flags: [row.fsc[0].commandLine], tfm: row.targetFramework,
        packages: [{ id: "FSharp.Core", version: row.core.package.version, sha256: row.core.package.sha256 }],
        dll: { ...dll, originalPath: dll.path, path: "producer/FrozenProducer.dll" },
        package: { id: pkg.id, version: pkg.version, sha256: pkg.sha256,
            originalPath: pkg.path, path: "producer/FrozenProducer.1.0.0.nupkg" },
    };
}

async function main() {
    const [configurationFile, outputDirectory, phase] = process.argv.slice(2);
    requireValue(configurationFile && outputDirectory && process.argv.length <= 5 && (!phase || ["released", "source"].includes(phase)),
        "usage: node repro.smoke.cjs <verified-products.json> <new-output-directory> [released|source]");
    const config = JSON.parse(await fs.readFile(configurationFile, "utf8"));
    const products = phase ? [phase] : ["released", "source"];
    requireValue(config.released && config.helper && products.every(name => config[name]?.product.kind === name),
        "verified products and evidence helper are required (source must be a clean local build)");
    await fs.mkdir(outputDirectory);
    const root = await fs.realpath(outputDirectory);
    const results = [];
    const producer = await freezeProducer(root, config, results);
    await fs.writeFile(path.join(root, "frozen-producer.json"), JSON.stringify(producer, null, 2));
    for (const name of products) {
        const input = config[name];
        for (const [kind, scenario, configuration] of [
            ["fsi", "pass", "Release"],
            ["project", "pass", "Debug"],
            ["project", "core-explicit", "Release"],
            ["project", "core-central", "Release"],
            ["project", "runtime-failure", "Release"],
            ["project", "diagnostic", "Release"],
            ["project", "late-flags", "Release"],
            ["project", "nullable-enabled", "Release"],
            ["project", "nullable-disabled", "Debug"],
            ["project", "late-nullable-enable", "Release"],
            ["project", "late-nullable-disable", "Release"],
            ["project", "late-nullable-explicit", "Release"],
            ["project", "collectible-core", "Release"],
            ["project", "reference-pass", "Release"],
            ["project", "reference-late", "Release"],
            ...(process.platform === "win32" ? [["project", "reference-tfm", "Release"]] : []),
            ["project", "producer-consumer", "Release"],
            ...(process.platform === "win32" ? [["project", "multi-tfm", "Release"]] : []),
            ...(name === "source" ? ["compiler", "tasks", "targets", "core"].map(role => ["project", `fallback-${role}`, "Release"]) : []),
            ["fsi", "producer-control", "Release"],
        ]) {
            const id = `${name}-${kind}-${scenario}-${configuration}`;
            const sourceDirectory = path.join(root, id);
            const manifest = await createFixture(sourceDirectory, {
                kind, scenario, configuration, tfm: input.tfm,
                coreVersion: config.released.product.corePackageVersion,
                ...(scenario === "producer-consumer" ? { producer } : {}),
            });
            const result = await runReproduction({
                manifest, sourceDirectory, workDirectory: path.join(root, "executions"),
                product: input.product, toolchain: input.toolchain, helper: config.helper,
                sampleRuntimes: input.sampleRuntimes,
            });
            const lateOverride = scenario.startsWith("late-") || scenario === "reference-late";
            const fallback = scenario.startsWith("fallback-");
            const expected = lateOverride || fallback ? "untestable"
                : ["runtime-failure", "producer-consumer", "diagnostic"].includes(scenario) ? "product-failure" : "pass";
            results.push({ id, expected, result });
            await fs.writeFile(path.join(root, "smoke-results.json"), JSON.stringify(results, null, 2));
            console.log(`${id}: ${result.outcome.outcome} (${result.outcome.reason})${result.error ? `: ${result.error}` : ""}`);
            requireValue(result.outcome.outcome === expected, `bounded smoke failed: ${id}`);
            if (lateOverride) requireValue(result.outcome.reason === "wrong-identity" &&
                result.error === "late compiler flag override", "late override must be rejected for the intended identity mismatch");
            if (fallback) requireValue(result.outcome.reason === "wrong-identity" &&
                result.pending?.evidence.build?.exitCode === 0,
            "fallback must compile successfully before product identity rejects it");
            if (scenario === "collectible-core") {
                const record = result.cells[0].evidence[0];
                const invocation = record.commands.at(-1);
                const env = isolatedEnvironment(path.join(root, "helper"), input.toolchain.requested.platform);
                const baseline = await execute(invocation.executable, invocation.args, { cwd: root, env, timeoutMs: 30000 });
                results.at(-1).uninstrumented = baseline;
                await fs.writeFile(path.join(root, "smoke-results.json"), JSON.stringify(results, null, 2));
                requireValue(baseline.complete && baseline.exitCode === 0 && baseline.stdout === "PASS\n" && baseline.stderr === "",
                    "collectible context must unload both with and without observation");
            }
        }
    }
}

if (require.main === module) main().catch(error => { console.error(error.message); process.exitCode = 1; });
