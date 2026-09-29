import assert from "node:assert/strict";
import fs from "node:fs";
import path from "node:path";
import { createHash } from "node:crypto";
import { gzipSync } from "node:zlib";
import { fileURLToPath } from "node:url";

const root = path.resolve(process.argv[2]);
const output = path.resolve(process.argv[3]);
const source = path.dirname(fileURLToPath(import.meta.url));
const wave = path.join(root, "third-wave");
const read = file => JSON.parse(fs.readFileSync(file, "utf8").replace(/^\uFEFF/, ""));
const hash = file => createHash("sha256").update(fs.readFileSync(file)).digest("hex");
const write = (name, value) => fs.writeFileSync(path.join(output, name), JSON.stringify(value, null, 2) + "\n");
const arms = ["sdk10", "sdk11rc1", "vmr-rc2-r2r"];
const labels = { sdk10: "10.0.100", sdk11rc1: "11 RC1", "vmr-rc2-r2r": "Local RC2 R2R" };
const names = {
    "fsharp-core": "FSharp.Core", "fsharp-compiler-service": "FSharp.Compiler.Service",
    fstoolkit: "FsToolkit.ErrorHandling", oxpecker: "Oxpecker", nu: "Nu", fsautocomplete: "FsAutoComplete",
    "oxpecker-ide": "Oxpecker", "fsautocomplete-ide": "FsAutoComplete"
};
const kernels = {
    Cart: "Cart-price List.fold", Rules: "Access-rule exists / forall",
    Telemetry: "Weighted Array.fold / fold2", Option: "Partially applied Option.map",
    Nested: "Nested group folds", FilterMap: "Filtering and mapping",
    Escaping: "Escaping callback control", NonCapturing: "Non-capturing callback control"
};
const quantile = (values, p) => {
    assert.ok(values.length > 0);
    const sorted = [...values].sort((a, b) => a - b);
    const index = (sorted.length - 1) * p, lower = Math.floor(index), fraction = index - lower;
    return sorted[lower] + fraction * (sorted[Math.ceil(index)] - sorted[lower]);
};
const median = values => quantile(values, 0.5);
const stats = values => ({ median: median(values), q1: quantile(values, 0.25), q3: quantile(values, 0.75),
    min: Math.min(...values), max: Math.max(...values) });
function pairedChange(values) {
    let seed = 110100;
    const random = () => {
        seed ^= seed << 13; seed ^= seed >>> 17; seed ^= seed << 5;
        return (seed >>> 0) / 4294967296;
    };
    const samples = Array.from({ length: 10000 }, () =>
        median(Array.from({ length: values.length }, () => values[Math.floor(random() * values.length)])));
    return { median_percent: median(values), low95: quantile(samples, 0.025), high95: quantile(samples, 0.975) };
}
const f = (value, digits = 2) => value.toFixed(digits);
const count = value => Number.isInteger(value) ? String(value) : f(value, 1);
const table = (headers, rows) => [
    `| ${headers.join(" | ")} |`, `| ${headers.map((_, i) => i < 2 ? "---" : "---:").join(" | ")} |`,
    ...rows.map(row => `| ${row.join(" | ")} |`)
].join("\n");
const csv = (name, headers, rows) => fs.writeFileSync(path.join(output, name),
    [headers, ...rows].map(row => row.map(value => `"${String(value).replaceAll('"', '""')}"`).join(",")).join("\n") + "\n");

const observations = fs.readdirSync(path.join(wave, "measurements")).filter(name => name.endsWith(".json"))
    .sort().map(name => read(path.join(wave, "measurements", name)));
assert.equal(observations.length, 336);
assert.equal(new Set(observations.map(r => r.run_id)).size, 336);
const scalarStatistics = {};
const baseMetrics = ["allocated_bytes", "peak_working_set_bytes", "peak_private_commit_bytes",
    "gen0", "gen1", "gen2", "cpu_ns", "wall_ns"];
for (const r of observations) {
    assert.equal(r.status, "accepted");
    assert.equal(r.exit_code, 0);
    assert.equal(r.parent_exit_code, 0);
    assert.equal(r.error_count, 0);
    assert.equal(r.gc_server, true);
    assert.equal(r.gc_configuration.GCDynamicAdaptationMode, 1);
    assert.equal(r.runtime, r.toolchain === "sdk10" ? ".NET 10.0.0" : ".NET 11.0.0-rc.1.26425.128");
    assert.equal(r.input_sha256.toLowerCase(), hash(path.join(root, `case-${r.case}.json`)));
    assert.equal(r.collector_sha256.toLowerCase(), hash(path.join(root, "collector", "Collector.dll")));
    r.cpu_ns = r.cpu_user_ns + r.cpu_kernel_ns;
}
for (const id of Object.keys(names)) {
    const rows = observations.filter(r => r.case === id);
    assert.equal(rows.length, 42, id);
    const operation = id.endsWith("-ide") ? "check" : "compile";
    const metrics = operation === "check"
        ? [...baseMetrics, "idle_10s_working_set_bytes", "retained_heap_bytes"] : baseMetrics;
    const summaries = {};
    for (const arm of arms) {
        const measured = rows.filter(r => r.toolchain === arm && !r.warmup).sort((a, b) => a.round - b.round);
        assert.deepEqual(measured.map(r => r.round), Array.from({ length: 12 }, (_, i) => i));
        summaries[arm] = { n: measured.length, metrics: Object.fromEntries(metrics.map(metric =>
            [metric, stats(measured.map(r => { assert.ok(Number.isFinite(r[metric])); return r[metric]; }))])) };
    }
    const changes = {};
    for (const [candidate, baseline] of [["sdk11rc1", "sdk10"], ["vmr-rc2-r2r", "sdk10"], ["vmr-rc2-r2r", "sdk11rc1"]]) {
        changes[`${candidate}_vs_${baseline}`] = Object.fromEntries(
            ["allocated_bytes", "peak_working_set_bytes", ...(operation === "check" ? ["retained_heap_bytes"] : [])].map(metric => {
                const ratios = Array.from({ length: 12 }, (_, round) => {
                    const a = rows.find(r => r.round === round && r.toolchain === baseline)[metric];
                    const b = rows.find(r => r.round === round && r.toolchain === candidate)[metric];
                    return 100 * (b / a - 1);
                });
                return [metric, pairedChange(ratios)];
            }));
    }
    scalarStatistics[id] = { operation, arms: summaries, changes };
}

fs.mkdirSync(path.join(output, "programs"), { recursive: true });
const programs = [], programProvenance = [];
for (let launch = 0; launch < 3; launch++) for (const arm of arms) {
    const directory = path.join(wave, "programs", arm, `launch-${launch}`);
    const raw = path.join(directory, "results", "Programs-report-full.json");
    const report = read(raw), provenance = read(path.join(directory, "provenance.json"));
    assert.equal(report.Benchmarks.length, 48);
    assert.equal(provenance.runtime, arm === "sdk10" ? ".NET 10.0.0" : ".NET 11.0.0-rc.1.26425.128");
    const ilInventory = path.join(directory, "il-closures.json");
    programProvenance.push({ arm, launch, ...provenance, raw_report_sha256: hash(raw), il_inventory_sha256: hash(ilInventory) });
    fs.writeFileSync(path.join(output, "programs", `${arm}-${launch}.json.gz`), gzipSync(fs.readFileSync(raw), { level: 9 }));
    if (launch === 0) fs.copyFileSync(ilInventory, path.join(output, "programs", `${arm}-il.json`));
    else assert.equal(hash(ilInventory), hash(path.join(output, "programs", `${arm}-il.json`)));
    for (const benchmark of report.Benchmarks) {
        const parameters = new URLSearchParams(benchmark.Parameters);
        const actual = benchmark.Measurements.filter(m => m.IterationMode === "Workload" && m.IterationStage === "Actual");
        assert.equal(benchmark.Statistics.N, 15);
        assert.equal(actual.length, 15);
        programs.push({ arm, launch, kernel: parameters.get("Kernel"), length: Number(parameters.get("Length")),
            runtime: provenance.runtime, bytes_per_op: benchmark.Memory.BytesAllocatedPerOperation,
            median_ns_per_op: benchmark.Statistics.Median, memory: benchmark.Memory,
            result_ns_per_op: benchmark.Statistics.OriginalValues, actual_iterations: actual });
    }
}
const programStatistics = [];
for (const kernel of Object.keys(kernels)) for (const length of [0, 1, 4, 16, 64, 1024]) for (const arm of arms) {
    const rows = programs.filter(r => r.kernel === kernel && r.length === length && r.arm === arm);
    assert.equal(rows.length, 3);
    programStatistics.push({ kernel, length, arm, launches: 3,
        bytes_per_op: stats(rows.map(r => r.bytes_per_op)), ns_per_op: stats(rows.map(r => r.median_ns_per_op)) });
}
const profilePaths = [
    path.join(root, "profiles", "third-wave-oxpecker-sdk10-summary.json"),
    path.join(root, "profiles", "third-wave-oxpecker-sdk11rc1-summary.json"),
    path.join(wave, "profile", "summary.json")
];
fs.mkdirSync(path.join(output, "profiles"), { recursive: true });
const profiles = profilePaths.map((file, i) => {
    const profile = read(file);
    assert.equal(profile.events_lost, 0);
    fs.copyFileSync(file, path.join(output, "profiles", `${arms[i]}.json`));
    return { arm: arms[i], allocation_ticks: profile.allocation_ticks,
        weighted_allocation_bytes: profile.weighted_allocation_bytes,
        verified_function_weighted_bytes: profile.verified_function_weighted_bytes,
        events_lost: profile.events_lost, raw_trace_sha256: hash(profile.collection.raw_trace) };
});
const cli = read(path.join(wave, "cli", "equivalence.json"));
assert.equal(cli.length, 6);
assert.ok(cli.every(r => r.identical && r.fcs_output_sha256 === r.cli_output_sha256));
const payload = read(path.join(wave, "payload.json"));
for (const image of payload.images) {
    assert.equal(hash(image.native.path), image.native.sha256.toLowerCase());
    assert.equal(image.il.mvid, image.native.mvid);
}
const provenance = { artifact_root: root, payload, program_launches: programProvenance, profiles, cli,
    historical_evidence: "../provenance.json",
    vmr_audit_sha256: hash(path.join(output, "vmr-audit.json")),
    contributions_sha256: hash(path.join(output, "contributions.csv")),
    build_log_scope: "All retained preparation attempts, including failures; the native-image checks identify the final payload.",
    build_logs: fs.readdirSync(root).filter(name => /^rc2-(bootstrap|release|core-net10|r2r(?:-\d+)?)\.binlog$/.test(name))
        .sort().map(name => name.slice(0, -7)).map(name => ({
        name, binlog: path.join(root, `${name}.binlog`), binlog_sha256: hash(path.join(root, `${name}.binlog`)),
        log: path.join(root, `${name}.log`), log_sha256: hash(path.join(root, `${name}.log`))
    })),
    harness: fs.readdirSync(source).filter(name => /\.(ps1|mjs|props|csproj)$/.test(name))
        .map(name => ({ name, sha256: hash(path.join(source, name)) })) };
write("results.json", { status: "measured", arms, observations, scalar_statistics: scalarStatistics,
    programs, program_statistics: programStatistics, profiles });
write("provenance.json", provenance);
write("cli-equivalence.json", cli);

const value = (id, arm, metric) => scalarStatistics[id].arms[arm].metrics[metric].median;
const compileIds = Object.keys(names).filter(id => !id.endsWith("-ide"));
const ideIds = ["fsautocomplete-ide", "oxpecker-ide"];
const compileRows = compileIds.flatMap(id => arms.map(arm => [names[id], labels[arm],
    f(value(id, arm, "allocated_bytes") / 2 ** 30, 3), f(value(id, arm, "peak_working_set_bytes") / 2 ** 20, 1),
    f(value(id, arm, "peak_private_commit_bytes") / 2 ** 20, 1),
    ["gen0", "gen1", "gen2"].map(m => count(value(id, arm, m))).join(" / "),
    f(value(id, arm, "cpu_ns") / 1e9), f(value(id, arm, "wall_ns") / 1e9)]));
const ideRows = ideIds.flatMap(id => arms.map(arm => [names[id], labels[arm],
    String(observations.find(r => r.case === id).project_count),
    f(value(id, arm, "allocated_bytes") / 2 ** 30, 3), f(value(id, arm, "peak_working_set_bytes") / 2 ** 20, 1),
    f(value(id, arm, "idle_10s_working_set_bytes") / 2 ** 20, 1),
    f(value(id, arm, "retained_heap_bytes") / 2 ** 20, 1),
    `${f(value(id, arm, "cpu_ns") / 1e9)} / ${f(value(id, arm, "wall_ns") / 1e9)}`]));
const programRows = Object.keys(kernels).map(kernel => {
    const selected = arms.map(arm => programStatistics.find(r => r.kernel === kernel && r.length === 4 && r.arm === arm));
    return [kernels[kernel], ...selected.map(r => count(r.bytes_per_op.median)),
        selected.map(r => f(r.ns_per_op.median)).join(" / ")];
});
const interval = result => `${result.median_percent >= 0 ? "+" : ""}${f(result.median_percent)} [${f(result.low95)}, ${f(result.high95)}]`;
const tables = {
    compilation: table(["Workload", "Compiler", "Allocated GiB", "Peak RAM MiB", "Peak private commit MiB", "G0 / G1 / G2", "CPU s", "Wall s"], compileRows),
    changes: table(["Workload", "RC1 vs 10 allocation % [95% CI]", "Local vs 10 allocation % [95% CI]", "Local vs RC1 allocation % [95% CI]", "Local vs 10 peak RAM % [95% CI]"],
        compileIds.map(id => [names[id], ...["sdk11rc1_vs_sdk10", "vmr-rc2-r2r_vs_sdk10", "vmr-rc2-r2r_vs_sdk11rc1"]
            .map(key => interval(scalarStatistics[id].changes[key].allocated_bytes)),
        interval(scalarStatistics[id].changes["vmr-rc2-r2r_vs_sdk10"].peak_working_set_bytes)])),
    ide: table(["Graph", "Compiler", "Projects", "Allocated GiB", "Peak RAM MiB", "RAM after 10 s idle MiB", "Live managed heap after full GC MiB", "CPU / wall s"], ideRows),
    programs: table(["Kernel, length 4", "10.0.100 B/op", "11 RC1 B/op", "Local RC2 B/op", "Median ns/op (10 / RC1 / local)"], programRows),
    profiles: table(["Oxpecker profile", "Allocation ticks", "Weighted allocation MiB", "Weighted generated-function MiB", "Lost events"],
        profiles.map(r => [labels[r.arm], r.allocation_ticks, f(r.weighted_allocation_bytes / 2 ** 20, 1),
            f(r.verified_function_weighted_bytes / 2 ** 20, 1), r.events_lost]))
};
csv("compilation-matrix.csv", ["Workload", "Compiler", "Allocated GiB", "Peak RAM MiB", "Peak private commit MiB", "G0 / G1 / G2", "CPU s", "Wall s"], compileRows);
csv("ide-matrix.csv", ["Graph", "Compiler", "Projects", "Allocated GiB", "Peak RAM MiB", "RAM after 10 s idle MiB", "Live managed heap after full GC MiB", "CPU / wall s"], ideRows);
csv("program-matrix.csv", ["Kernel", "Length", "Compiler", "Launches", "B/op", "B/op min", "B/op max", "Median ns/op", "Launch median min ns/op", "Launch median max ns/op"],
    programStatistics.map(r => [r.kernel, r.length, r.arm, r.launches, r.bytes_per_op.median, r.bytes_per_op.min,
        r.bytes_per_op.max, r.ns_per_op.median, r.ns_per_op.min, r.ns_per_op.max]));
write("tables.json", tables);
const articlePath = path.join(output, "article.md");
let article = fs.readFileSync(articlePath, "utf8");
for (const [name, contents] of Object.entries(tables)) {
    const pattern = new RegExp(`<!-- table:${name} -->[\\s\\S]*?<!-- /table:${name} -->`);
    assert.ok(pattern.test(article), `Missing article table marker: ${name}`);
    article = article.replace(pattern, `<!-- table:${name} -->\n\n${contents}\n\n<!-- /table:${name} -->`);
}
fs.writeFileSync(articlePath, article);
const experiment = read(path.join(output, "experiment.json"));
experiment.status = "measured";
experiment.counts = { scalar: 336, measured_scalar: 288, warmup_scalar: 48,
    program_launches: 9, program_launch_cases: 432, measured_program_iterations: 6480, accepted_profiles: 3 };
write("experiment.json", experiment);
console.log("Published 336 scalar observations, 432 program launch/cases, three fresh profiles, and all article tables.");
