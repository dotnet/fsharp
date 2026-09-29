import assert from "node:assert/strict";
import fs from "node:fs";
import path from "node:path";
import { createHash } from "node:crypto";
import { fileURLToPath } from "node:url";

const directory = path.resolve(process.argv[2] ?? "");
const read = name => JSON.parse(fs.readFileSync(path.join(directory, name), "utf8").replace(/^\uFEFF/, ""));
const hash = file => createHash("sha256").update(fs.readFileSync(file)).digest("hex");
const median = values => [...values].sort((a, b) => a - b)[Math.floor(values.length / 2)];
const manifest = read("experiment.json"), data = read("results.json"), evidence = read("provenance.json");
const article = fs.readFileSync(path.join(directory, "article.md"), "utf8");
assert.equal(manifest.status, "measured");
assert.equal(data.status, "measured");
assert.equal(manifest.toolchains[0].sdk, "10.0.100");
assert.equal(manifest.toolchains[1].sdk, "11.0.100-rc.1.26425.128");
assert.equal(data.compilation.length, 168);
assert.equal(data.ide.length, 56);
assert.equal(data.controls.length, 68);
assert.equal(data.generated_programs.length, 288);
assert.equal(data.rejected_observations.length, 34);
assert.equal(data.profiles.length, 2);
const all = [...data.compilation, ...data.ide, ...data.controls];
assert.equal(new Set(all.map(r => r.run_id)).size, all.length);
for (const r of all) {
    assert.equal(r.status, "accepted", r.run_id);
    assert.equal(r.parent_exit_code, 0);
    assert.equal(r.exit_code, 0);
    assert.equal(r.error_count, 0);
    assert.equal(r.warmup, r.pair < 0);
    assert.equal(r.gc_server, true);
    assert.equal(r.visible_processors, 16);
    assert.ok(Number.isSafeInteger(r.allocated_bytes) && r.allocated_bytes > 0);
    assert.ok(r.wall_ns > 0 && r.cpu_ns >= 0);
    assert.ok(r.peak_working_set_bytes >= r.working_set_end_bytes);
    assert.ok(r.gen0 >= r.gen1 && r.gen1 >= r.gen2 && r.gen2 >= 0);
    const baseline = r.toolchain.startsWith("sdk10");
    const tc = manifest.toolchains[baseline ? 0 : 1];
    assert.equal(r.fcs_version, tc.fcs_informational_version);
    assert.equal(r.core_version, tc.core_informational_version);
    const runtime = r.toolchain === "sdk10" ? manifest.toolchains[0].runtime : manifest.toolchains[1].runtime;
    assert.equal(r.runtime, `.NET ${runtime}`);
    assert.equal(r.gc_configuration.GCDynamicAdaptationMode, r.toolchain.endsWith("datas-off") ? 0 : 1);
    assert.equal(r.collector_sha256.toLowerCase(), evidence.collector_sha256.toLowerCase());
    const prefix = r.comparison && r.comparison !== "shipping" ? `${r.case}-${r.comparison}` : r.case;
    const operation = r.scope === "cold-fcs-compile" ? "compile" : "check";
    assert.equal(r.input_sha256.toLowerCase(), hash(path.join(directory, "inputs", `case-${r.case}.json`)));
    assert.equal(r.input_inventory_sha256.toLowerCase(), hash(path.join(directory, "inputs", `${prefix}-${operation}-inputs.json`)));
}
for (const [section, cases] of [
    ["compilation", data.statistics.compilation], ["ide", data.statistics.ide],
    ...Object.entries(data.statistics.controls).map(([name, cases]) => [name, cases])
]) {
    const raw = section === "compilation" ? data.compilation : section === "ide" ? data.ide :
        data.controls.filter(r => r.comparison === section);
    for (const [id, summary] of Object.entries(cases)) {
        const measured = raw.filter(r => r.case === id && !r.warmup);
        const arms = Object.keys(summary.arms).sort();
        assert.equal(measured.length, summary.measured_pairs * 2);
        for (let pair = 0; pair < summary.measured_pairs; pair++)
            assert.deepEqual(measured.filter(r => r.pair === pair).map(r => r.toolchain).sort(), arms);
        for (const arm of arms) {
            assert.equal(raw.filter(r => r.case === id && r.toolchain === arm && r.warmup).length, 2);
            for (const [metric, s] of Object.entries(summary.arms[arm])) {
                assert.equal(s.n, summary.measured_pairs);
                assert.equal(s.median, median(measured.filter(r => r.toolchain === arm).map(r => r[metric])));
                assert.ok(s.q1 <= s.median && s.median <= s.q3);
            }
        }
        for (const [metric, s] of Object.entries(summary.candidate_over_baseline)) {
            const ratios = Array.from({ length: summary.measured_pairs }, (_, pair) =>
                measured.find(r => r.pair === pair && r.toolchain === s.candidate)[metric] /
                measured.find(r => r.pair === pair && r.toolchain === s.baseline)[metric]);
            assert.equal(s.median_paired_ratio, median(ratios));
            assert.ok(s.bootstrap_95[0] <= s.median_paired_ratio && s.median_paired_ratio <= s.bootstrap_95[1]);
        }
    }
}
for (const workload of manifest.compilation.workloads) {
    const s = data.statistics.compilation[workload.id];
    assert.equal(s.measured_pairs, workload.measured_pairs);
    for (const arm of ["sdk10", "sdk11rc1"]) {
        const m = key => s.arms[arm][key].median;
        const row = `| ${workload.name} | ${arm === "sdk10" ? "10.0.100" : "11 RC1"} | ${(m("allocated_bytes") / 2 ** 30).toFixed(3)} | ${(m("peak_working_set_bytes") / 2 ** 20).toFixed(1)} | ${(m("peak_private_commit_bytes") / 2 ** 20).toFixed(1)} | ${m("gen0")} / ${m("gen1")} / ${m("gen2")} | ${(m("cpu_ns") / 1e9).toFixed(2)} | ${(m("wall_ns") / 1e9).toFixed(2)} |`;
        assert.ok(article.includes(row), `Stale article row: ${workload.id} ${arm}`);
    }
    const change = key => {
        const s = data.statistics.compilation[workload.id].candidate_over_baseline[key];
        const format = r => `${r >= 1 ? "+" : ""}${((r - 1) * 100).toFixed(2)}`;
        return `${format(s.median_paired_ratio)} [${format(s.bootstrap_95[0])}, ${format(s.bootstrap_95[1])}]`;
    };
    assert.ok(article.includes(`| ${workload.name} | ${change("allocated_bytes")} | ${change("peak_working_set_bytes")} |`));
}
for (const graph of manifest.ide.graphs) {
    const id = `${graph.workload}-ide`, summary = data.statistics.ide[id];
    assert.equal(summary.measured_pairs, graph.measured_pairs);
    const name = manifest.compilation.workloads.find(w => w.id === graph.workload).name;
    for (const arm of ["sdk10", "sdk11rc1"]) {
        const m = key => summary.arms[arm][key].median;
        const row = `| ${name} | ${arm === "sdk10" ? "10.0.100" : "11 RC1"} | ${graph.checked_projects} | ${(m("allocated_bytes") / 2 ** 30).toFixed(3)} | ${(m("peak_working_set_bytes") / 2 ** 20).toFixed(1)} | ${(m("idle_10s_working_set_bytes") / 2 ** 20).toFixed(1)} | ${(m("retained_heap_bytes") / 2 ** 20).toFixed(1)} | ${(m("cpu_ns") / 1e9).toFixed(2)} / ${(m("wall_ns") / 1e9).toFixed(2)} |`;
        assert.ok(article.includes(row), `Stale IDE row: ${id} ${arm}`);
    }
}
const controls = [
    ["compiler-on-runtime11", "sdk10-on-runtime11", "Old compiler, .NET 11 RC1 runtime"],
    ["compiler-on-runtime11", "sdk11rc1", "RC1 compiler, same runtime"],
    ["datas", "sdk11rc1-datas-off", "RC1 compiler/runtime, DATAS off"],
    ["datas", "sdk11rc1-datas-on", "RC1 compiler/runtime, DATAS on"]
];
for (const [comparison, arm, label] of controls) {
    const s = data.statistics.controls[comparison]["fsharp-compiler-service"].arms[arm];
    const m = key => s[key].median;
    assert.ok(article.includes(`| ${label} | ${(m("allocated_bytes") / 2 ** 30).toFixed(3)} | ${(m("peak_working_set_bytes") / 2 ** 20).toFixed(1)} | ${m("gen0")} / ${m("gen1")} / ${m("gen2")} | ${(m("cpu_ns") / 1e9).toFixed(2)} | ${(m("wall_ns") / 1e9).toFixed(2)} |`));
}
const programKeys = new Set(), programDrivers = new Set(), programRuntimes = new Set();
const reports = new Map();
for (const r of data.generated_programs) {
    const key = `${r.toolchain}/${r.launch}/${r.kernel}/${r.length}`;
    assert.ok(!programKeys.has(key));
    programKeys.add(key);
    assert.equal(r.iterations, 15);
    assert.ok(r.allocated_bytes_per_operation >= 0 && r.median_ns > 0);
    if (!reports.has(r.raw_report)) reports.set(r.raw_report, read(r.raw_report));
    const b = reports.get(r.raw_report).Benchmarks.find(b => b.Parameters === `Length=${r.length}&Kernel=${r.kernel}`);
    assert.equal(b.Memory.BytesAllocatedPerOperation, r.allocated_bytes_per_operation);
    assert.equal(b.Statistics.Median, r.median_ns);
    assert.equal(b.Measurements.filter(m => m.IterationMode === "Workload" && m.IterationStage === "Actual").length, 15);
}
for (const arm of ["sdk10", "sdk11rc1"]) {
    for (let launch = 0; launch < 3; launch++) {
        const p = read(`programs/${arm}-launch${launch}-provenance.json`);
        programDrivers.add(p.driver_sha256);
        programRuntimes.add(p.runtime);
        assert.equal(p.core_version, manifest.toolchains[arm === "sdk10" ? 0 : 1].core_informational_version);
        for (const kernel of manifest.generated_programs.cases)
            for (const size of manifest.generated_programs.sizes)
                assert.ok(programKeys.has(`${arm}/${launch}/${kernel.id}/${size}`));
    }
}
assert.equal(programDrivers.size, 1);
assert.equal([...programDrivers][0], evidence.program_driver_sha256.toLowerCase());
assert.deepEqual([...programRuntimes], [`.NET ${manifest.toolchains[1].runtime}`]);
const programNames = {
    Cart: "Cart-price List.fold", Rules: "Access-rule List.exists / List.forall",
    Telemetry: "Weighted Array.fold / Array.fold2", Option: "Partially applied Option.map",
    Nested: "Nested group folds", FilterMap: "Filtering and mapping a batch",
    Escaping: "Escaping callback control", NonCapturing: "Non-capturing callback control"
};
for (const [kernel, name] of Object.entries(programNames)) {
    const arm = id => data.generated_programs.filter(r => r.kernel === kernel && r.length === 4 && r.toolchain === id);
    const a = arm("sdk10"), b = arm("sdk11rc1");
    assert.ok(article.includes(`| ${name} | ${median(a.map(r => r.allocated_bytes_per_operation))} | ${median(b.map(r => r.allocated_bytes_per_operation))} | ${median(a.map(r => r.median_ns)).toFixed(2)} / ${median(b.map(r => r.median_ns)).toFixed(2)} |`));
}
const cli = read("cli-equivalence.json");
assert.equal(cli.length, 12);
assert.equal(new Set(cli.map(r => `${r.case}/${r.toolchain}`)).size, 12);
for (const r of cli) {
    assert.equal(r.identical, true);
    assert.equal(r.fcs_output_sha256, r.cli_output_sha256);
    assert.equal(r.input_sha256.toLowerCase(), hash(path.join(directory, "inputs", `case-${r.case}.json`)));
}
for (const p of data.profiles) {
    const detail = read(p.details);
    assert.equal(detail.events_lost, 0);
    assert.equal(detail.collection.reader_sha256.toLowerCase(), evidence.profile_reader_sha256.toLowerCase());
    assert.equal(detail.allocations.reduce((sum, r) => sum + r.weighted_bytes, 0), detail.weighted_allocation_bytes);
    const functions = detail.allocations.filter(r => r.verified_fsharp_function);
    assert.ok(functions.length > 0);
    assert.equal(functions.reduce((sum, r) => sum + r.weighted_bytes, 0), detail.verified_function_weighted_bytes);
    assert.ok(functions.every(r => r.closure && r.module && r.type.includes("@") && r.metadata_token.startsWith("0x2")));
    assert.ok(article.includes(`| ${p.toolchain === "sdk10" ? "10.0.100" : "11 RC1"} | ${p.allocation_ticks} | ${(p.weighted_allocation_bytes / 2 ** 20).toFixed(1)} | ${(p.verified_function_weighted_bytes / 2 ** 20).toFixed(1)} | 0 |`));
}
for (const c of Object.values(evidence.calibration)) {
    assert.equal(c.status, "accepted");
    assert.ok(c.allocated_bytes >= c.minimum_payload_bytes);
    assert.ok(c.peak_working_set_bytes > 128 * 2 ** 20);
    assert.ok(c.retained_heap_bytes < 2 ** 20);
}
assert.equal(evidence.source_graph_validation["source-edge-sdk10"].error_count, 0);
assert.equal(evidence.source_graph_validation["source-edge-sdk11rc1"].error_count, 0);
assert.ok(evidence.source_graph_validation["missing-edge-result"].error_count > 0);
const harness = path.dirname(fileURLToPath(import.meta.url));
for (const item of evidence.harness_files)
    assert.equal(hash(path.join(harness, item.file)), item.sha256.toLowerCase(), item.file);
for (const item of evidence.input_manifests)
    assert.equal(hash(path.join(directory, "inputs", item.file)), item.sha256.toLowerCase(), item.file);
for (const file of ["article.md", "README.md", "experiment-design.md"]) {
    const text = fs.readFileSync(path.join(directory, file), "utf8");
    for (const match of text.matchAll(/\[[^\]]*\]\(([^)]+)\)/g)) {
        if (/^https?:/.test(match[1])) continue;
        assert.ok(fs.existsSync(path.resolve(directory, match[1].split("#")[0])), `${file}: ${match[1]}`);
    }
}
for (const [file, count] of [["compilation-matrix.csv", 12], ["ide-matrix.csv", 4], ["controls.csv", 4], ["generated-program-matrix.csv", 288]]) {
    const lines = fs.readFileSync(path.join(directory, file), "utf8").trim().split(/\r?\n/);
    assert.equal(lines.length - 1, count, file);
    assert.ok(lines.every(line => line.split(",").length === lines[0].split(",").length), file);
}
console.log("Validated 292 scalar observations, 288 program launch/cases, 12 CLI equivalences, two profiles, input hashes, all article tables, and matrix row counts.");
