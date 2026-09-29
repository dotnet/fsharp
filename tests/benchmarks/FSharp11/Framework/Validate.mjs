import assert from "node:assert/strict";
import fs from "node:fs";
import path from "node:path";
import { createHash } from "node:crypto";
import { gunzipSync } from "node:zlib";
import { execFileSync } from "node:child_process";
import { fileURLToPath } from "node:url";

const [artifactRoot, destination] = process.argv.slice(2);
assert.ok(artifactRoot && destination, "Usage: node Validate.mjs <performance-root> <publication-directory>");
const root = path.resolve(artifactRoot), directory = path.resolve(destination);
const source = path.dirname(fileURLToPath(import.meta.url));
const read = file => JSON.parse(fs.readFileSync(file, "utf8").replace(/^\uFEFF/, ""));
const hashBytes = bytes => createHash("sha256").update(bytes).digest("hex");
const hash = file => hashBytes(fs.readFileSync(file));
const data = read(path.join(directory, "results.json")), evidence = read(path.join(directory, "provenance.json"));
const manifest = read(path.join(directory, "experiment.json"));
const arms = ["old-framework", "new-framework", "old-net10", "new-net10", "old-net11", "new-net11"];
const runtime = arm => arm.endsWith("framework") ? ".NET Framework 4.8.9345.0"
    : arm.endsWith("net10") ? ".NET 10.0.0" : ".NET 11.0.0-rc.1.26425.128";
const q = (values, fraction) => {
    const a = values.slice().sort((x, y) => x - y), pos = fraction * (a.length - 1), left = Math.floor(pos);
    return a[left] + (pos - left) * (a[Math.ceil(pos)] - a[left]);
};
const median = values => q(values, 0.5);
const checkStats = (actual, values) => assert.deepEqual(actual, { n: values.length, median: median(values),
    q1: q(values, 0.25), q3: q(values, 0.75), min: Math.min(...values), max: Math.max(...values) });
assert.equal(manifest.status, "measured"); assert.equal(data.status, "measured");
assert.deepEqual(data.arms, arms); assert.deepEqual(manifest.design.arms, arms);
assert.equal(data.observations.length, 672);
assert.equal(data.observations.filter(r => !r.warmup).length, 576);
assert.equal(new Set(data.observations.map(r => r.raw_file)).size, 672);
function checkRecord(r) {
    assert.equal(r.status, "accepted"); assert.equal(r.exit_code, 0);
    assert.equal(r.parent_exit_code, 0); assert.equal(r.error_count, 0);
    assert.equal(r.runtime, runtime(r.arm));
    assert.equal(r.visible_processors, 16); assert.equal(r.gc_server, true);
    assert.equal(r.appdomain_id, 1);
    assert.equal(r.fcs_target_framework, ".NETStandard,Version=v2.0");
    assert.equal(r.core_target_framework, ".NETStandard,Version=v2.0");
    const old = r.arm.startsWith("old-");
    assert.equal(r.fcs_version, old ? "43.10.100-rc2.25523.111+b0f34d51fccc69fd334253924abd8d6853fad7aa"
        : "43.13.101+9cd6167a7265ce7264b22719503b7dfa9eb8f83c");
    assert.equal(r.core_version, old ? "10.0.100-rc2.25523.111+b0f34d51fccc69fd334253924abd8d6853fad7aa"
        : "11.0.101+9cd6167a7265ce7264b22719503b7dfa9eb8f83c");
    assert.equal(r.input_sha256.toLowerCase(), hash(path.join(root, `case-${r.case}.json`)));
    assert.equal(r.fcs_sha256.toLowerCase(), hash(r.fcs_path));
    assert.equal(r.core_sha256.toLowerCase(), hash(r.core_path));
    const classic = r.arm.endsWith("framework");
    const worker = path.join(root, "framework", "collector-bin", "Release", classic ? "net472" : "net10.0",
        classic ? "Collector.exe" : "Collector.dll");
    assert.equal(r.collector_sha256.toLowerCase(), hash(worker));
    assert.ok(r.fsharp_modules.every(m => !m.name.toLowerCase().endsWith(".ni.dll")));
    if (classic) assert.equal(r.gc_configuration.DATAS, "not-supported");
    else {
        assert.equal(r.gc_configuration.GCDynamicAdaptationMode, 1);
        assert.equal(r.gc_configuration.GCHeapHardLimit, 0);
        assert.equal(r.gc_configuration.ConcurrentGC, true);
    }
    if (r.allocation_monitoring) assert.ok(r.allocated_bytes > 0); else assert.equal(r.allocated_bytes, null);
    assert.ok(r.wall_ns > 0 && r.cpu_user_ns >= 0 && r.cpu_kernel_ns >= 0);
    assert.ok(r.gen0 >= r.gen1 && r.gen1 >= r.gen2);
    assert.ok(r.peak_working_set_bytes >= r.working_set_end_bytes);
}
for (const r of data.observations) {
    checkRecord(r);
    const { raw_file, cpu_ns, ...original } = r;
    assert.deepEqual(original, read(path.join(root, "framework", raw_file)));
    assert.equal(cpu_ns, r.cpu_user_ns + r.cpu_kernel_ns);
    assert.equal(r.warmup, r.round < 0); assert.equal(r.allocation_monitoring, true);
}
assert.equal(Object.keys(data.scalar_statistics).length, 8);
for (const [id, summary] of Object.entries(data.scalar_statistics)) {
    const rows = data.observations.filter(r => r.case === id);
    assert.equal(rows.length, 84);
    for (let round = -2; round < 12; round++) {
        const shift = (round % 6 + 6) % 6, expected = [0, 1, 5, 2, 4, 3].map(i => arms[(i + shift) % 6]);
        const selected = rows.filter(r => r.round === round).sort((a, b) => Date.parse(a.started_utc) - Date.parse(b.started_utc));
        assert.deepEqual(selected.map(r => r.arm), expected);
        for (const r of selected) assert.deepEqual(r.order, expected);
    }
    for (const arm of arms) {
        const selected = rows.filter(r => r.arm === arm && !r.warmup);
        assert.equal(selected.length, 12);
        for (const [metric, stats] of Object.entries(summary.arms[arm].metrics))
            checkStats(stats, selected.map(r => r[metric]));
    }
    for (const [comparison, metrics] of Object.entries(summary.changes)) {
        const [candidate, baseline] = comparison.split("_vs_");
        for (const [metric, change] of Object.entries(metrics)) {
            const a = median(rows.filter(r => r.arm === baseline && !r.warmup).map(r => r[metric]));
            const b = median(rows.filter(r => r.arm === candidate && !r.warmup).map(r => r[metric]));
            assert.equal(change.ratio_of_medians_percent, a === 0 ? null : 100 * (b / a - 1));
            const ratios = Array.from({ length: 12 }, (_, round) => {
                const before = rows.find(r => r.arm === baseline && r.round === round)[metric];
                return before === 0 ? null : 100 * (rows.find(r => r.arm === candidate && r.round === round)[metric] / before - 1);
            });
            if (ratios.every(Number.isFinite)) checkStats(change.paired_percent, ratios);
            else assert.equal(change.paired_percent, null);
        }
    }
}
const ordered = [...data.observations].sort((a, b) => Date.parse(a.started_utc) - Date.parse(b.started_utc));
for (let i = 1; i < ordered.length; i++) {
    const previous = ordered[i - 1], end = Date.parse(previous.started_utc) + previous.wall_ns / 1e6;
    assert.ok(Date.parse(ordered[i].started_utc) >= end, "Overlapping primary operations");
}
assert.equal(data.monitoring_controls.length, 36);
for (const r of data.monitoring_controls) {
    checkRecord(r);
    const { raw_file, ...original } = r;
    assert.deepEqual(original, read(path.join(root, "framework", raw_file)));
    assert.equal(r.allocation_monitoring, r.cohort === "monitoring-on");
}
for (const group of data.monitoring_statistics) {
    for (const [metric, stats] of Object.entries(group.on_vs_off_percent)) {
        const selected = data.monitoring_controls.filter(r => r.case === group.case && r.arm === group.arm);
        const on = selected.filter(r => r.allocation_monitoring), off = selected.filter(r => !r.allocation_monitoring);
        assert.equal(on.length, group.pairs); assert.equal(off.length, group.pairs);
        checkStats(stats, on.map(r => 100 * (r[metric] / off.find(o => o.round === r.round)[metric] - 1)));
    }
}
assert.equal(data.programs.length, 864); assert.equal(data.program_statistics.length, 288);
assert.equal(evidence.program_launches.length, 18);
for (const p of evidence.program_launches) {
    assert.equal(p.runtime, runtime(p.arm)); assert.equal(p.gc_server, false);
    assert.equal(p.benchmarkdotnet, "0.14.0.0");
    const payload = p.arm.split("-")[0], classic = p.arm.endsWith("framework");
    assert.equal(p.workload_sha256, hash(path.join(root, "framework", "programs", payload, "Kernels.dll")));
    assert.equal(p.core_sha256, hash(path.join(root, "framework", `${payload}-payload`, "FSharp.Core.dll")));
    assert.equal(p.driver_sha256, hash(path.join(root, "framework", "program-bin", "Release",
        classic ? "net472" : "net10.0", classic ? "Programs.exe" : "Programs.dll")));
    const bytes = gunzipSync(fs.readFileSync(path.join(directory, "programs", `${p.arm}-${p.launch}.json.gz`)));
    assert.equal(hashBytes(bytes), p.raw_report_sha256);
    const report = JSON.parse(bytes);
    assert.equal(report.Benchmarks.length, 48);
    for (const b of report.Benchmarks) {
        const params = new URLSearchParams(b.Parameters);
        const r = data.programs.find(r => r.arm === p.arm && r.launch === p.launch
            && r.kernel === params.get("Kernel") && r.length === Number(params.get("Length")));
        assert.ok(r); assert.equal(b.Statistics.N, 15);
        assert.equal(b.Measurements.filter(m => m.IterationMode === "Workload" && m.IterationStage === "Warmup").length, 6);
        assert.deepEqual(r.actual_iterations, b.Measurements.filter(m => m.IterationMode === "Workload" && m.IterationStage === "Actual"));
        assert.equal(r.actual_iterations.length, 15);
        assert.deepEqual(r.result_ns_per_op, b.Statistics.OriginalValues);
        assert.equal(r.median_ns_per_op, b.Statistics.Median);
        assert.deepEqual(r.memory, b.Memory); assert.equal(r.bytes_per_op, b.Memory.BytesAllocatedPerOperation);
    }
}
for (let launch = 0; launch < 3; launch++) assert.deepEqual(evidence.program_launches.filter(p => p.launch === launch)
    .sort((a, b) => Date.parse(a.started_utc) - Date.parse(b.started_utc)).map(p => p.arm), manifest.design.applications.launch_order[launch]);
for (const r of data.program_statistics) {
    const rows = data.programs.filter(p => p.arm === r.arm && p.kernel === r.kernel && p.length === r.length);
    assert.deepEqual(rows.map(p => p.launch).sort(), [0, 1, 2]);
    checkStats(r.bytes_per_op, rows.map(p => p.bytes_per_op));
    checkStats(r.ns_per_op, rows.map(p => p.median_ns_per_op));
}
for (const r of data.programs.filter(r => r.length === 4)) {
    const newer = r.arm.startsWith("new-");
    const expected = { Cart: newer ? 0 : 24, Rules: newer ? 0 : 48, Telemetry: newer ? 0 : 48,
        Nested: newer ? 0 : 48, Option: r.arm.endsWith("framework") ? newer ? 24 : 48 : 0,
        NonCapturing: 0, Escaping: 24, FilterMap: 146 };
    assert.equal(r.bytes_per_op, expected[r.kernel]);
}
for (const file of [...evidence.frozen.files, ...evidence.manifest_addendum.files])
    assert.equal(hash(file.path), file.sha256.toLowerCase());
assert.equal(evidence.frozen.calibrations.length, 6);
for (const r of evidence.frozen.calibrations) {
    assert.equal(r.status, "accepted");
    assert.ok(r.threads_before_gc_bytes >= 41920000 && r.threads_before_gc_bytes <= 41920000 * 1.02);
    assert.ok(r.threads_after_gc_bytes >= r.threads_before_gc_bytes);
}
for (const file of evidence.harness) assert.equal(hash(path.join(source, file.name)), file.sha256);
for (const file of evidence.frozen.package_locks) assert.equal(hash(path.join(source, file.name)), file.sha256.toLowerCase());
for (const file of evidence.preparation_logs) assert.equal(hash(file.file), file.sha256);
for (const file of evidence.auxiliary_files) {
    const bytes = gunzipSync(fs.readFileSync(path.join(directory, "evidence", `${file.file}.gz`)));
    assert.equal(hashBytes(bytes), file.sha256);
    assert.equal(hash(path.join(root, "framework", file.file)), file.sha256);
}
const identities = read(path.join(directory, "output-identities.json"));
assert.equal(identities.length, 12);
for (const item of identities) {
    for (const [runtime, hashes] of Object.entries(item.hashes)) {
        const rows = data.observations.filter(r => r.case === item.case && r.arm === `${item.payload}-${runtime}`);
        assert.deepEqual([...new Set(rows.map(r => r.emitted_sha256))], hashes.map(h => h.sha256));
        for (const h of hashes) assert.deepEqual(rows.filter(r => r.emitted_sha256 === h.sha256)
            .map(r => r.round).sort((a, b) => a - b), h.rounds);
    }
    assert.equal(item.modern_outputs_identical, true);
    assert.equal(item.hashes.net10[0].sha256, item.hashes.net11[0].sha256);
}
const names = { "FSharp.Core": "fsharp-core", "FSharp.Compiler.Service": "fsharp-compiler-service",
    "FsToolkit.ErrorHandling": "fstoolkit", Oxpecker: "oxpecker", Nu: "nu", FsAutoComplete: "fsautocomplete" };
const csv = name => fs.readFileSync(path.join(directory, name), "utf8").trim().split(/\r?\n/).slice(1)
    .map(line => line.slice(1, -1).split('","'));
for (const [file, ide, count] of [["compilation-matrix.csv", false, 36], ["ide-matrix.csv", true, 12]]) {
    const rows = csv(file); assert.equal(rows.length, count);
    for (const [name, arm, ...values] of rows) {
        const id = names[name] + (ide ? "-ide" : "");
        const selected = data.observations.filter(r => r.case === id && r.arm === arm && !r.warmup);
        const m = metric => median(selected.map(r => r[metric]));
        assert.deepEqual(values, [(m("allocated_bytes") / 2 ** 30).toFixed(3), (m("peak_working_set_bytes") / 2 ** 20).toFixed(1),
            (m("peak_private_commit_bytes") / 2 ** 20).toFixed(1), ...["gen0", "gen1", "gen2"].map(metric => String(m(metric))),
            (m("cpu_ns") / 1e9).toFixed(2), (m("wall_ns") / 1e9).toFixed(2),
            ...(ide ? [(m("idle_10s_working_set_bytes") / 2 ** 20).toFixed(1), (m("retained_heap_bytes") / 2 ** 20).toFixed(1)] : [])]);
        assert.ok(fs.readFileSync(path.join(directory, "tables.md"), "utf8").includes(`| ${[name, arm, ...values].join(" | ")} |`));
    }
}
assert.equal(csv("program-matrix.csv").length, 288);
for (const [kernel, length, arm, ...values] of csv("program-matrix.csv")) {
    const r = data.program_statistics.find(r => r.kernel === kernel && r.length === Number(length) && r.arm === arm);
    assert.deepEqual(values.map(Number), [r.bytes_per_op.median, r.bytes_per_op.min, r.bytes_per_op.max,
        r.ns_per_op.median, r.ns_per_op.min, r.ns_per_op.max]);
}
execFileSync(process.execPath, [path.join(source, "..", "ThirdWave", "Validate.mjs"), root, path.join(directory, "..", "third-wave")],
    { stdio: "inherit" });
console.log("Validated 672 serial scalar records, runtime defaults, frozen artifacts, 36 controls, 12960 application iterations and all prior evidence.");
