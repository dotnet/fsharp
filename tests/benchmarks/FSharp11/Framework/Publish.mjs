import assert from "node:assert/strict";
import fs from "node:fs";
import path from "node:path";
import { createHash } from "node:crypto";
import { gzipSync } from "node:zlib";
import { fileURLToPath } from "node:url";

const [artifactRoot, destination] = process.argv.slice(2);
assert.ok(artifactRoot && destination, "Usage: node Publish.mjs <performance-root> <publication-directory>");
const root = path.resolve(artifactRoot), output = path.resolve(destination);
const source = path.dirname(fileURLToPath(import.meta.url)), campaign = path.join(root, "framework");
const read = file => JSON.parse(fs.readFileSync(file, "utf8").replace(/^\uFEFF/, ""));
const hash = file => createHash("sha256").update(fs.readFileSync(file)).digest("hex");
const write = (name, value) => fs.writeFileSync(path.join(output, name), JSON.stringify(value, null, 2) + "\n");
const experiment = read(path.join(output, "experiment.json")), arms = experiment.design.arms;
const names = {
    "fsharp-core": "FSharp.Core", "fsharp-compiler-service": "FSharp.Compiler.Service",
    fstoolkit: "FsToolkit.ErrorHandling", oxpecker: "Oxpecker", nu: "Nu", fsautocomplete: "FsAutoComplete",
    "oxpecker-ide": "Oxpecker", "fsautocomplete-ide": "FsAutoComplete"
};
const quantile = (values, p) => {
    assert.ok(values.length && values.every(Number.isFinite));
    const a = [...values].sort((x, y) => x - y), index = (a.length - 1) * p, lo = Math.floor(index);
    return a[lo] + (a[Math.ceil(index)] - a[lo]) * (index - lo);
};
const stats = values => ({ n: values.length, median: quantile(values, 0.5),
    q1: quantile(values, 0.25), q3: quantile(values, 0.75), min: Math.min(...values), max: Math.max(...values) });
const records = directory => fs.readdirSync(path.join(campaign, directory)).filter(f => f.endsWith(".json"))
    .sort().map(f => ({ ...read(path.join(campaign, directory, f)), raw_file: `${directory}/${f}` }));
const metrics = ["allocated_bytes", "peak_working_set_bytes", "peak_private_commit_bytes",
    "gen0", "gen1", "gen2", "cpu_ns", "wall_ns"];
const observations = records("measurements").map(r => ({ ...r, cpu_ns: r.cpu_user_ns + r.cpu_kernel_ns }));
assert.equal(observations.length, 672);
assert.ok(observations.every(r => r.status === "accepted" && r.exit_code === 0 && r.parent_exit_code === 0 && r.error_count === 0));
const scalarStatistics = {};
for (const id of Object.keys(names)) {
    const rows = observations.filter(r => r.case === id && !r.warmup);
    assert.equal(rows.length, 72);
    const fields = id.endsWith("-ide") ? [...metrics, "idle_10s_working_set_bytes", "retained_heap_bytes", "post_gc_working_set_bytes"] : metrics;
    const summaries = Object.fromEntries(arms.map(arm => {
        const selected = rows.filter(r => r.arm === arm);
        assert.deepEqual(selected.map(r => r.round).sort((a, b) => a - b), Array.from({ length: 12 }, (_, i) => i));
        return [arm, { n: 12, metrics: Object.fromEntries(fields.map(m => [m, stats(selected.map(r => r[m]))])) }];
    }));
    const comparisons = [
        ...["framework", "net10", "net11"].map(runtime => [`new-${runtime}`, `old-${runtime}`]),
        ...["old", "new"].flatMap(payload => [[`${payload}-net11`, `${payload}-net10`], [`${payload}-net10`, `${payload}-framework`]])
    ];
    const changes = Object.fromEntries(comparisons.map(([candidate, baseline]) => [
        `${candidate}_vs_${baseline}`, Object.fromEntries(fields.map(metric => {
            const a = summaries[baseline].metrics[metric].median, b = summaries[candidate].metrics[metric].median;
            const ratios = Array.from({ length: 12 }, (_, round) => {
                const before = rows.find(r => r.arm === baseline && r.round === round)[metric];
                return before === 0 ? null : 100 * (rows.find(r => r.arm === candidate && r.round === round)[metric] / before - 1);
            });
            return [metric, { ratio_of_medians_percent: a === 0 ? null : 100 * (b / a - 1),
                paired_percent: ratios.every(Number.isFinite) ? stats(ratios) : null }];
        }))
    ]));
    scalarStatistics[id] = { operation: id.endsWith("-ide") ? "check" : "compile", arms: summaries, changes };
}

fs.mkdirSync(path.join(output, "programs"), { recursive: true });
const programs = [], programLaunches = [];
for (let launch = 0; launch < 3; launch++) for (const arm of arms) {
    const directory = path.join(campaign, "programs", arm, `launch-${launch}`);
    const raw = path.join(directory, "results", "Programs-report-full.json"), report = read(raw);
    const provenance = read(path.join(directory, "provenance.json"));
    assert.equal(report.Benchmarks.length, 48);
    programLaunches.push({ arm, launch, ...provenance, raw_report_sha256: hash(raw) });
    fs.writeFileSync(path.join(output, "programs", `${arm}-${launch}.json.gz`), gzipSync(fs.readFileSync(raw), { level: 9 }));
    for (const b of report.Benchmarks) {
        const parameters = new URLSearchParams(b.Parameters);
        const actual = b.Measurements.filter(m => m.IterationMode === "Workload" && m.IterationStage === "Actual");
        assert.equal(actual.length, 15);
        assert.equal(b.Statistics.N, 15);
        programs.push({ arm, launch, kernel: parameters.get("Kernel"), length: Number(parameters.get("Length")),
            bytes_per_op: b.Memory.BytesAllocatedPerOperation, median_ns_per_op: b.Statistics.Median,
            memory: b.Memory, actual_iterations: actual, result_ns_per_op: b.Statistics.OriginalValues });
    }
}
const kernels = ["Cart", "Rules", "Telemetry", "Option", "Nested", "FilterMap", "Escaping", "NonCapturing"];
const programStatistics = kernels.flatMap(kernel => [0, 1, 4, 16, 64, 1024].flatMap(length => arms.map(arm => {
    const selected = programs.filter(r => r.arm === arm && r.kernel === kernel && r.length === length);
    assert.equal(selected.length, 3);
    return { arm, kernel, length, launches: 3, bytes_per_op: stats(selected.map(r => r.bytes_per_op)),
        ns_per_op: stats(selected.map(r => r.median_ns_per_op)) };
})));
const controls = [...records("monitoring-on"), ...records("monitoring-off")];
assert.equal(controls.length, 36);
const monitoring = ["oxpecker", "fsharp-compiler-service"].flatMap(id => ["old-framework", "new-framework"].map(arm => {
    const on = controls.filter(r => r.case === id && r.arm === arm && r.allocation_monitoring);
    const off = controls.filter(r => r.case === id && r.arm === arm && !r.allocation_monitoring);
    const count = id === "oxpecker" ? 6 : 3;
    assert.equal(on.length, count); assert.equal(off.length, count);
    return { case: id, arm, pairs: count, on_vs_off_percent: Object.fromEntries(
        ["wall_ns", "peak_working_set_bytes", "peak_private_commit_bytes"].map(metric => [metric,
            stats(on.map(r => 100 * (r[metric] / off.find(o => o.round === r.round)[metric] - 1)))])) };
}));
const equivalence = Object.keys(names).filter(id => !id.endsWith("-ide")).flatMap(id => ["old", "new"].map(payload => {
    const selected = observations.filter(r => r.case === id && r.arm.startsWith(`${payload}-`));
    const hashes = Object.fromEntries(["framework", "net10", "net11"].map(runtime => {
        const rows = selected.filter(r => r.arm === `${payload}-${runtime}`);
        return [runtime, [...new Set(rows.map(r => r.emitted_sha256))].map(sha256 => ({
            sha256, rounds: rows.filter(r => r.emitted_sha256 === sha256).map(r => r.round).sort((a, b) => a - b)
        }))];
    }));
    return { case: id, payload, hashes,
        modern_outputs_identical: new Set(selected.filter(r => !r.arm.endsWith("framework")).map(r => r.emitted_sha256)).size === 1 };
}));
const frozen = read(path.join(campaign, "frozen.json"));
for (const file of frozen.files) assert.equal(hash(file.path), file.sha256.toLowerCase());
const auxiliary = ["calibration-frozen", "failed-warmup-manifest", "manifest-replay", "output-inspection"];
const auxiliaryFiles = auxiliary.flatMap(directory => fs.readdirSync(path.join(campaign, directory))
    .filter(name => /\.(json|stderr|stdout)$/.test(name)).map(name => {
        const file = path.join(campaign, directory, name), relative = `${directory}/${name}`;
        const target = path.join(output, "evidence", `${relative}.gz`);
        fs.mkdirSync(path.dirname(target), { recursive: true });
        fs.writeFileSync(target, gzipSync(fs.readFileSync(file), { level: 9 }));
        return { file: relative, sha256: hash(file) };
    }));
const preparation = fs.readdirSync(campaign).filter(name => /\.(log|binlog)$/.test(name)).sort().map(name => ({
    file: path.join(campaign, name), sha256: hash(path.join(campaign, name))
}));
write("results.json", { status: "measured", arms, observations, scalar_statistics: scalarStatistics,
    programs, program_statistics: programStatistics, monitoring_controls: controls, monitoring_statistics: monitoring });
write("provenance.json", { artifact_root: root, frozen, manifest_addendum: read(path.join(campaign, "manifest-addendum.json")),
    program_launches: programLaunches, preparation_logs: preparation, auxiliary_files: auxiliaryFiles,
    historical_inputs: "../inputs", harness: fs.readdirSync(source).filter(name => /\.(mjs|ps1|cs|csproj|config|manifest|json)$/.test(name))
        .sort().map(name => ({ name, sha256: hash(path.join(source, name)) })) });
write("output-identities.json", equivalence);
const csv = (name, headers, rows) => fs.writeFileSync(path.join(output, name),
    [headers, ...rows].map(row => row.map(value => `"${String(value).replaceAll('"', '""')}"`).join(",")).join("\n") + "\n");
const f = (v, places = 2) => v.toFixed(places);
const headers = ["Workload", "Arm", "Allocated GiB", "Peak RAM MiB", "Peak private commit MiB", "G0", "G1", "G2", "CPU s", "Wall s"];
const rowsFor = ide => Object.keys(names).filter(id => id.endsWith("-ide") === ide).flatMap(id => arms.map(arm => {
    const m = field => scalarStatistics[id].arms[arm].metrics[field].median;
    return [names[id], arm, f(m("allocated_bytes") / 2 ** 30, 3), f(m("peak_working_set_bytes") / 2 ** 20, 1),
        f(m("peak_private_commit_bytes") / 2 ** 20, 1), ...["gen0", "gen1", "gen2"].map(m),
        f(m("cpu_ns") / 1e9), f(m("wall_ns") / 1e9),
        ...(ide ? [f(m("idle_10s_working_set_bytes") / 2 ** 20, 1), f(m("retained_heap_bytes") / 2 ** 20, 1)] : [])];
}));
const compileRows = rowsFor(false), ideRows = rowsFor(true);
const ideHeaders = [...headers, "RAM after 10 s idle MiB", "Retained managed heap MiB"];
csv("compilation-matrix.csv", headers, compileRows);
csv("ide-matrix.csv", ideHeaders, ideRows);
csv("program-matrix.csv", ["Kernel", "Length", "Arm", "B/op", "B/op min", "B/op max", "Median ns/op", "Launch median min ns/op", "Launch median max ns/op"],
    programStatistics.map(r => [r.kernel, r.length, r.arm, r.bytes_per_op.median, r.bytes_per_op.min, r.bytes_per_op.max,
        r.ns_per_op.median, r.ns_per_op.min, r.ns_per_op.max]));
const table = (head, rows) => [`| ${head.join(" | ")} |`, `| ${head.map(() => "---").join(" | ")} |`,
    ...rows.map(row => `| ${row.join(" | ")} |`)].join("\n");
const programRows = kernels.map(kernel => [kernel, ...arms.map(arm =>
    programStatistics.find(r => r.arm === arm && r.kernel === kernel && r.length === 4).bytes_per_op.median)]);
fs.writeFileSync(path.join(output, "tables.md"), "# Portable-payload runtime comparison\n\n"
    + "Generated medians; all successful primary observations retained. See [methodology](README.md) for arm definitions and uncertainty.\n\n"
    + "## Compilation\n\n" + table(headers, compileRows) + "\n\n## IDE graphs\n\n" + table(ideHeaders, ideRows)
    + "\n\n## Applications: length four, bytes allocated per operation\n\n" + table(["Kernel", ...arms], programRows) + "\n");
experiment.status = "measured";
experiment.counts = { scalar: 672, measured_scalar: 576, warmup_scalar: 96, monitoring_controls: 36,
    program_launches: 18, program_launch_cases: 864, measured_program_iterations: 12960 };
write("experiment.json", experiment);
console.log("Published 672 scalar records, 36 monitoring controls, 864 application launch/cases and full matrices.");
