import assert from "node:assert/strict";
import fs from "node:fs";
import path from "node:path";
import { createHash } from "node:crypto";
import { gunzipSync } from "node:zlib";
import { execFileSync } from "node:child_process";
import { fileURLToPath } from "node:url";

const root = path.resolve(process.argv[2]), directory = path.resolve(process.argv[3]);
const source = path.dirname(fileURLToPath(import.meta.url));
const read = file => JSON.parse(fs.readFileSync(file, "utf8").replace(/^\uFEFF/, ""));
const data = read(path.join(directory, "results.json"));
const evidence = read(path.join(directory, "provenance.json"));
const manifest = read(path.join(directory, "experiment.json"));
const tables = read(path.join(directory, "tables.json"));
const article = fs.readFileSync(path.join(directory, "article.md"), "utf8");
const hashBytes = bytes => createHash("sha256").update(bytes).digest("hex");
const hash = file => hashBytes(fs.readFileSync(file));
const median = values => {
    const a = [...values].sort((x, y) => x - y), mid = Math.floor(a.length / 2);
    return a.length % 2 ? a[mid] : (a[mid - 1] + a[mid]) / 2;
};
const f = (n, digits = 2) => n.toFixed(digits);
const gc = n => Number.isInteger(n) ? String(n) : f(n, 1);
const arms = ["sdk10", "sdk11rc1", "vmr-rc2-r2r"];
const labels = { "10.0.100": "sdk10", "11 RC1": "sdk11rc1", "Local RC2 R2R": "vmr-rc2-r2r" };
const names = { "FSharp.Core": "fsharp-core", "FSharp.Compiler.Service": "fsharp-compiler-service",
    "FsToolkit.ErrorHandling": "fstoolkit", Oxpecker: "oxpecker", Nu: "nu", FsAutoComplete: "fsautocomplete" };
const versions = {
    sdk10: ["43.10.100-rc2.25523.111+b0f34d51fccc69fd334253924abd8d6853fad7aa",
        "10.0.100-rc2.25523.111+b0f34d51fccc69fd334253924abd8d6853fad7aa"],
    sdk11rc1: ["43.13.101-rc1.26425.128+3551975be08744f0418857c5bed8ab1545c5dd47",
        "11.0.101-rc1.26425.128+3551975be08744f0418857c5bed8ab1545c5dd47"],
    "vmr-rc2-r2r": ["43.13.101+9cd6167a7265ce7264b22719503b7dfa9eb8f83c",
        "11.0.101+9cd6167a7265ce7264b22719503b7dfa9eb8f83c"]
};
assert.equal(manifest.status, "measured");
assert.equal(data.status, "measured");
assert.equal(data.observations.length, 336);
assert.equal(data.observations.filter(r => !r.warmup).length, 288);
assert.equal(new Set(data.observations.map(r => r.run_id)).size, 336);
for (const r of data.observations) {
    const raw = read(path.join(root, "third-wave", "measurements", `${r.run_id}.json`));
    const { cpu_ns, ...publishedRaw } = r;
    assert.deepEqual(publishedRaw, raw);
    assert.equal(cpu_ns, r.cpu_user_ns + r.cpu_kernel_ns);
    assert.equal(r.status, "accepted");
    assert.equal(r.parent_exit_code, 0);
    assert.equal(r.exit_code, 0);
    assert.equal(r.error_count, 0);
    assert.equal(r.visible_processors, 16);
    assert.equal(r.gc_server, true);
    assert.equal(r.gc_configuration.ConcurrentGC, true);
    assert.equal(r.gc_configuration.GCDynamicAdaptationMode, 1);
    assert.equal(r.gc_configuration.GCHeapHardLimit, 0);
    assert.deepEqual([r.fcs_version, r.core_version], versions[r.toolchain]);
    assert.equal(r.runtime, r.toolchain === "sdk10" ? ".NET 10.0.0" : ".NET 11.0.0-rc.1.26425.128");
    assert.equal(r.warmup, r.round < 0);
    assert.equal(r.input_sha256.toLowerCase(), hash(path.join(directory, "..", "inputs", `case-${r.case}.json`)));
    assert.ok(r.allocated_bytes > 0 && r.wall_ns > 0 && r.cpu_ns >= 0);
    assert.ok(r.gen0 >= r.gen1 && r.gen1 >= r.gen2);
    assert.ok(r.peak_working_set_bytes >= r.working_set_end_bytes);
}
for (const [id, summary] of Object.entries(data.scalar_statistics)) {
    const rows = data.observations.filter(r => r.case === id);
    assert.equal(rows.length, 42);
    for (let round = -2; round < 12; round++) {
        const order = manifest.measurement.order[((round % 6) + 6) % 6];
        const triplet = rows.filter(r => r.round === round).sort((a, b) => Date.parse(a.started_utc) - Date.parse(b.started_utc));
        assert.deepEqual(triplet.map(r => r.toolchain), order);
        for (const r of triplet) assert.deepEqual(r.order, order);
    }
    for (const arm of arms) {
        const selected = rows.filter(r => r.toolchain === arm && !r.warmup);
        assert.equal(selected.length, 12);
        for (const [metric, stats] of Object.entries(summary.arms[arm].metrics)) {
            assert.equal(stats.median, median(selected.map(r => r[metric])));
            assert.ok(stats.min <= stats.q1 && stats.q1 <= stats.median && stats.median <= stats.q3 && stats.q3 <= stats.max);
        }
    }
    for (const [comparison, metrics] of Object.entries(summary.changes)) {
        const [candidate, baseline] = comparison.split("_vs_");
        for (const [metric, result] of Object.entries(metrics)) {
            const ratios = Array.from({ length: 12 }, (_, round) =>
                100 * (rows.find(r => r.round === round && r.toolchain === candidate)[metric] /
                    rows.find(r => r.round === round && r.toolchain === baseline)[metric] - 1));
            assert.equal(result.median_percent, median(ratios));
            assert.ok(result.low95 <= result.median_percent && result.median_percent <= result.high95);
        }
    }
}
const tableRows = text => text.split("\n").slice(2).map(line => line.split("|").slice(1, -1).map(s => s.trim()));
for (const [name, text] of Object.entries(tables)) {
    assert.ok(article.includes(`<!-- table:${name} -->\n\n${text}\n\n<!-- /table:${name} -->`), name);
}
assert.equal(tableRows(tables.compilation).length, 18);
assert.equal(tableRows(tables.ide).length, 6);
for (const [tableName, ide] of [["compilation", false], ["ide", true]]) {
    for (const row of tableRows(tables[tableName])) {
        const id = names[row[0]] + (ide ? "-ide" : ""), arm = labels[row[1]];
        const selected = data.observations.filter(r => r.case === id && r.toolchain === arm && !r.warmup);
        const m = metric => median(selected.map(r => r[metric]));
        const expected = ide ? [
            String(selected[0].project_count), f(m("allocated_bytes") / 2 ** 30, 3), f(m("peak_working_set_bytes") / 2 ** 20, 1),
            f(m("idle_10s_working_set_bytes") / 2 ** 20, 1), f(m("retained_heap_bytes") / 2 ** 20, 1),
            `${f(m("cpu_ns") / 1e9)} / ${f(m("wall_ns") / 1e9)}`
        ] : [
            f(m("allocated_bytes") / 2 ** 30, 3), f(m("peak_working_set_bytes") / 2 ** 20, 1),
            f(m("peak_private_commit_bytes") / 2 ** 20, 1), ["gen0", "gen1", "gen2"].map(metric => gc(m(metric))).join(" / "),
            f(m("cpu_ns") / 1e9), f(m("wall_ns") / 1e9)
        ];
        assert.deepEqual(row.slice(2), expected);
    }
}

assert.equal(data.programs.length, 432);
assert.equal(data.program_statistics.length, 144);
assert.equal(evidence.program_launches.length, 9);
for (const p of evidence.program_launches) {
    const bytes = gunzipSync(fs.readFileSync(path.join(directory, "programs", `${p.arm}-${p.launch}.json.gz`)));
    assert.equal(hashBytes(bytes), p.raw_report_sha256);
    const raw = JSON.parse(bytes);
    assert.equal(raw.Benchmarks.length, 48);
    assert.equal(p.runtime, p.arm === "sdk10" ? ".NET 10.0.0" : ".NET 11.0.0-rc.1.26425.128");
    assert.equal(p.driver_sha256, hash(path.join(root, "program-driver", "Programs.dll")));
    assert.equal(p.workload_sha256, hash(path.join(root, "third-wave", "programs", p.arm, "Kernels.dll")));
    const ilFile = path.join(directory, "programs", `${p.arm}-il.json`);
    assert.equal(hash(ilFile), p.il_inventory_sha256);
    const sites = read(ilFile).newobj_sites.filter(s => s.fsharp_function);
    assert.equal(sites.length, p.arm === "vmr-rc2-r2r" ? 11 : 19);
    for (const b of raw.Benchmarks) {
        const parameters = new URLSearchParams(b.Parameters), kernel = parameters.get("Kernel"), length = Number(parameters.get("Length"));
        const r = data.programs.find(r => r.arm === p.arm && r.launch === p.launch && r.kernel === kernel && r.length === length);
        assert.ok(r);
        assert.equal(b.Statistics.N, 15);
        assert.equal(b.Measurements.filter(m => m.IterationMode === "Workload" && m.IterationStage === "Warmup").length, 6);
        assert.deepEqual(r.actual_iterations, b.Measurements.filter(m => m.IterationMode === "Workload" && m.IterationStage === "Actual"));
        assert.equal(r.actual_iterations.length, 15);
        assert.deepEqual(r.result_ns_per_op, b.Statistics.OriginalValues);
        assert.equal(r.bytes_per_op, b.Memory.BytesAllocatedPerOperation);
    }
}
for (const r of data.program_statistics) {
    const launches = data.programs.filter(p => p.arm === r.arm && p.kernel === r.kernel && p.length === r.length);
    assert.deepEqual(launches.map(p => p.launch).sort(), [0, 1, 2]);
    assert.equal(r.bytes_per_op.median, median(launches.map(p => p.bytes_per_op)));
    assert.equal(r.ns_per_op.median, median(launches.map(p => p.median_ns_per_op)));
}
const kernelLabels = {
    "Cart-price List.fold": "Cart", "Access-rule exists / forall": "Rules",
    "Weighted Array.fold / fold2": "Telemetry", "Partially applied Option.map": "Option",
    "Nested group folds": "Nested", "Filtering and mapping": "FilterMap",
    "Escaping callback control": "Escaping", "Non-capturing callback control": "NonCapturing"
};
assert.equal(tableRows(tables.programs).length, 8);
for (const row of tableRows(tables.programs)) {
    const selected = arms.map(arm => data.program_statistics.find(r =>
        r.arm === arm && r.kernel === kernelLabels[row[0]] && r.length === 4));
    assert.deepEqual(row.slice(1), [...selected.map(r => gc(r.bytes_per_op.median)),
        selected.map(r => f(r.ns_per_op.median)).join(" / ")]);
}
const interval = r => `${r.median_percent >= 0 ? "+" : ""}${f(r.median_percent)} [${f(r.low95)}, ${f(r.high95)}]`;
assert.equal(tableRows(tables.changes).length, 6);
for (const row of tableRows(tables.changes)) {
    const changes = data.scalar_statistics[names[row[0]]].changes;
    assert.deepEqual(row.slice(1), [
        ...["sdk11rc1_vs_sdk10", "vmr-rc2-r2r_vs_sdk10", "vmr-rc2-r2r_vs_sdk11rc1"].map(c => interval(changes[c].allocated_bytes)),
        interval(changes["vmr-rc2-r2r_vs_sdk10"].peak_working_set_bytes)
    ]);
}
for (const r of data.programs.filter(r => r.length === 4)) {
    const before = { Cart: 24, Rules: 48, Telemetry: 48, Nested: 48, Option: 0, NonCapturing: 0, Escaping: 24, FilterMap: 146 };
    const eliminated = ["Cart", "Rules", "Telemetry", "Nested"].includes(r.kernel) && r.arm === "vmr-rc2-r2r";
    assert.equal(r.bytes_per_op, eliminated ? 0 : before[r.kernel]);
}
assert.equal(evidence.cli.length, 6);
assert.ok(evidence.cli.every(r => r.identical && r.fcs_output_sha256 === r.cli_output_sha256));
for (const arm of arms) {
    const profile = read(path.join(directory, "profiles", `${arm}.json`));
    assert.equal(profile.events_lost, 0);
    assert.ok(profile.allocation_ticks > 0 && profile.verified_function_weighted_bytes <= profile.weighted_allocation_bytes);
    assert.equal(hash(profile.collection.raw_trace), evidence.profiles.find(p => p.arm === arm).raw_trace_sha256);
}
assert.equal(tableRows(tables.profiles).length, 3);
for (const row of tableRows(tables.profiles)) {
    const profile = read(path.join(directory, "profiles", `${labels[row[0]]}.json`));
    assert.deepEqual(row.slice(1), [String(profile.allocation_ticks), f(profile.weighted_allocation_bytes / 2 ** 20, 1),
        f(profile.verified_function_weighted_bytes / 2 ** 20, 1), "0"]);
}
const audit = read(path.join(directory, "vmr-audit.json"));
assert.equal(hash(path.join(directory, "vmr-audit.json")), evidence.vmr_audit_sha256);
assert.equal(hash(path.join(directory, "contributions.csv")), evidence.contributions_sha256);
assert.equal(audit.find(r => r.snapshot === "rc2").production_tree, audit.find(r => r.snapshot === "main").production_tree);
assert.notEqual(audit.find(r => r.snapshot === "rc2").fsharp_tree, audit.find(r => r.snapshot === "main").fsharp_tree);
const contributions = read(path.join(directory, "contributions.json"));
assert.equal(contributions.length, 52);
assert.equal(contributions.filter(r => r.rc1_source_status === "ancestor").length, 12);
assert.equal(contributions.filter(r => r.rc2_source_status === "ancestor").length, 49);
for (const image of evidence.payload.images) {
    assert.equal(hash(image.native.path), image.native.sha256.toLowerCase());
    assert.equal(image.native.disable_optimizations, false);
    assert.ok(image.native.ready_to_run && image.native.machine === "Amd64");
    assert.equal(image.native.mvid, image.il.mvid);
}
for (const file of evidence.harness) assert.equal(hash(path.join(source, file.name)), file.sha256);
for (const build of evidence.build_logs) {
    assert.equal(hash(build.binlog), build.binlog_sha256);
    assert.equal(hash(build.log), build.log_sha256);
}
for (const [file, rows] of [["compilation-matrix.csv", 18], ["ide-matrix.csv", 6], ["program-matrix.csv", 144], ["contributions.csv", 52]]) {
    assert.equal(fs.readFileSync(path.join(directory, file), "utf8").trim().split(/\r?\n/).length, rows + 1);
}
execFileSync(process.execPath, [path.join(source, "..", "Validate.mjs"), path.dirname(directory)], { stdio: "inherit" });
console.log("Validated new runtime/default identities, complete balanced rounds, numeric tables, 6480 program iterations, allocation thresholds, R2R evidence and unchanged historical inputs.");
