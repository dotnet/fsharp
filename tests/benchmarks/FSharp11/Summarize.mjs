import fs from "node:fs";
import path from "node:path";

const [input, destination, programRoot, profileRoot] = process.argv.slice(2);
if (!input || !destination) throw new Error("Usage: node Summarize.mjs <observations-directory> <publication-directory>");
const resultPath = path.join(destination, "results.json");
const results = JSON.parse(fs.readFileSync(resultPath, "utf8"));
const observations = fs.readdirSync(input).filter(p => p.endsWith(".json"))
    .map(p => JSON.parse(fs.readFileSync(path.join(input, p), "utf8").replace(/^\uFEFF/, "")))
    .filter(r => r.run_id).sort((a, b) => a.run_id.localeCompare(b.run_id));
if (new Set(observations.map(r => r.run_id)).size !== observations.length)
    throw new Error("Duplicate observation identities");
for (const r of observations) {
    if (r.status !== "accepted" || r.parent_exit_code !== 0 || r.error_count !== 0)
        throw new Error(`Unsuccessful observation: ${r.run_id}`);
    r.cpu_ns = r.cpu_user_ns + r.cpu_kernel_ns;
}
results.compilation = observations.filter(r => r.scope === "cold-fcs-compile" && (!r.comparison || r.comparison === "shipping"));
results.ide = observations.filter(r => r.scope === "check");
results.controls = observations.filter(r => r.scope === "cold-fcs-compile" && r.comparison && r.comparison !== "shipping");
if (programRoot) {
    results.generated_programs = [];
    const programs = path.join(destination, "programs");
    fs.mkdirSync(programs, { recursive: true });
    const drivers = new Set(), runtimes = new Set(), sources = new Set(), processes = new Set();
    for (const arm of ["sdk10", "sdk11rc1"]) {
        const directory = path.join(programRoot, arm);
        const compilation = JSON.parse(fs.readFileSync(path.join(directory, "compilation.json"), "utf8"));
        sources.add(compilation.source_sha256);
        for (let launch = 0; launch < 3; launch++) {
            const launchDirectory = path.join(directory, `launch-${launch}`);
            const provenance = JSON.parse(fs.readFileSync(path.join(launchDirectory, "provenance.json"), "utf8"));
            const raw = fs.readFileSync(path.join(launchDirectory, "results", "Programs-report-full.json"), "utf8");
            const report = JSON.parse(raw);
            drivers.add(provenance.driver_sha256);
            runtimes.add(provenance.runtime);
            processes.add(`${provenance.process_id}:${provenance.started_utc}`);
            if (report.Benchmarks.length !== 48) throw new Error(`Incomplete program matrix: ${arm} launch ${launch}`);
            const prefix = `${arm}-launch${launch}`;
            fs.writeFileSync(path.join(programs, `${prefix}-benchmarkdotnet.json`), raw);
            fs.writeFileSync(path.join(programs, `${prefix}-provenance.json`), JSON.stringify({ ...provenance, compilation }, null, 2) + "\n");
            if (launch === 0) fs.copyFileSync(path.join(launchDirectory, "il-closures.json"), path.join(programs, `${arm}-il-closures.json`));
            for (const b of report.Benchmarks) {
                const parameters = /^Length=(\d+)&Kernel=(\w+)$/.exec(b.Parameters);
                if (!parameters || b.Statistics.N !== 15 || b.Memory.BytesAllocatedPerOperation == null)
                    throw new Error(`Invalid program observations: ${arm} ${b.Parameters}`);
                results.generated_programs.push({
                    toolchain: arm, launch, kernel: parameters[2], length: Number(parameters[1]),
                    allocated_bytes_per_operation: b.Memory.BytesAllocatedPerOperation,
                    median_ns: b.Statistics.Median, mean_ns: b.Statistics.Mean,
                    iterations: b.Statistics.N, standard_deviation_ns: b.Statistics.StandardDeviation,
                    gc: b.Memory, raw_report: `programs/${prefix}-benchmarkdotnet.json`
                });
            }
        }
        for (const suffix of ["benchmarkdotnet.json", "provenance.json"]) {
            const pilot = path.join(programs, `${arm}-${suffix}`);
            if (fs.existsSync(pilot)) fs.unlinkSync(pilot);
        }
    }
    if (drivers.size !== 1 || runtimes.size !== 1 || sources.size !== 1 || processes.size !== 6)
        throw new Error("Program driver/runtime/source differs, or fresh suite processes are missing");
}
const names = {
    "fsharp-core": "FSharp.Core", "fsharp-compiler-service": "FSharp.Compiler.Service",
    fstoolkit: "FsToolkit.ErrorHandling", oxpecker: "Oxpecker", nu: "Nu", fsautocomplete: "FsAutoComplete"
};
const metrics = ["allocated_bytes", "peak_working_set_bytes", "peak_private_commit_bytes",
    "gen0", "gen1", "gen2", "cpu_ns", "wall_ns"];
function quantile(values, p) {
    const a = [...values].sort((x, y) => x - y);
    const at = (a.length - 1) * p, lower = Math.floor(at);
    return a[lower] + (a[Math.ceil(at)] - a[lower]) * (at - lower);
}
function statistics(a) {
    const median = quantile(a, 0.5), q1 = quantile(a, 0.25), q3 = quantile(a, 0.75);
    return { n: a.length, median, q1, q3, iqr: q3 - q1, relative_iqr: median === 0 ? null : (q3 - q1) / median };
}
let seed = 110100;
function random() {
    seed = (Math.imul(seed, 1664525) + 1013904223) >>> 0;
    return seed / 4294967296;
}
function summarize(data, fields) {
    const output = {};
    for (const id of [...new Set(data.map(r => r.case))].sort()) {
        const rows = data.filter(r => r.case === id && !r.warmup);
        const arms = [...new Set(rows.map(r => r.toolchain))].sort();
        const pairs = [...new Set(rows.map(r => r.pair))].sort((a, b) => a - b);
        if (arms.length !== 2 || pairs.some(p => rows.filter(r => r.pair === p).length !== 2))
            throw new Error(`Incomplete paired dataset: ${id}`);
        if (new Set(rows.map(r => r.input_sha256)).size !== 1 ||
            new Set(rows.map(r => r.input_inventory_sha256)).size !== 1 ||
            new Set(rows.map(r => r.collector_sha256)).size !== 1)
            throw new Error(`Inputs or collector changed inside cohort: ${id}`);
        output[id] = { measured_pairs: pairs.length, arms: {}, candidate_over_baseline: {} };
        for (const arm of arms)
            output[id].arms[arm] = Object.fromEntries(fields.map(m =>
                [m, statistics(rows.filter(r => r.toolchain === arm).map(r => r[m]))]));
        for (const metric of fields) {
            const ratios = pairs.map(p => {
                const a = rows.find(r => r.pair === p && r.toolchain === arms[0])[metric];
                const b = rows.find(r => r.pair === p && r.toolchain === arms[1])[metric];
                return a === 0 ? null : b / a;
            });
            if (ratios.some(r => r === null)) continue;
            const boot = Array.from({ length: 10000 }, () =>
                quantile(ratios.map(() => ratios[Math.floor(random() * ratios.length)]), 0.5));
            output[id].candidate_over_baseline[metric] = {
                baseline: arms[0], candidate: arms[1], median_paired_ratio: quantile(ratios, 0.5),
                bootstrap_95: [quantile(boot, 0.025), quantile(boot, 0.975)]
            };
        }
    }
    return output;
}
results.statistics = {
    bootstrap_seed: 110100, bootstrap_resamples: 10000, quantiles: "linear interpolation, (n-1)*p",
    compilation: summarize(results.compilation, metrics),
    ide: summarize(results.ide, [...metrics, "idle_10s_working_set_bytes", "retained_heap_bytes"]),
    controls: Object.fromEntries([...new Set(results.controls.map(r => r.comparison))].sort().map(comparison =>
        [comparison, summarize(results.controls.filter(r => r.comparison === comparison), metrics)]))
};
results.status = "measured";
if (profileRoot) {
    results.profiles = [];
    fs.mkdirSync(path.join(destination, "profiles"), { recursive: true });
    for (const arm of ["sdk10", "sdk11rc1"]) {
        const name = `oxpecker-allocation-${arm}-summary.json`;
        const raw = fs.readFileSync(path.join(profileRoot, name), "utf8");
        const profile = JSON.parse(raw);
        if (profile.events_lost !== 0 || profile.verified_function_weighted_bytes <= 0)
            throw new Error(`Invalid allocation profile: ${arm}`);
        fs.writeFileSync(path.join(destination, "profiles", name), raw);
        results.profiles.push({
            workload: "oxpecker", toolchain: arm, trace_sha256: profile.trace_sha256,
            allocation_ticks: profile.allocation_ticks, weighted_allocation_bytes: profile.weighted_allocation_bytes,
            verified_function_weighted_bytes: profile.verified_function_weighted_bytes,
            events_lost: profile.events_lost, details: `profiles/${name}`
        });
    }
}
fs.writeFileSync(resultPath, JSON.stringify(results, null, 2) + "\n");
const header = ["workload", "toolchain", "measured_pairs", ...metrics.flatMap(m => [`${m}_median`, `${m}_q1`, `${m}_q3`])];
const csv = [header.join(",")];
const table = ["| Workload | SDK compiler | Allocated GiB | Peak RAM MiB | Peak private commit MiB | G0 / G1 / G2 | CPU s | Wall s |",
    "|---|---|---:|---:|---:|---|---:|---:|"];
for (const [id, name] of Object.entries(names)) {
    const cohort = results.statistics.compilation[id];
    if (!cohort) continue;
    for (const arm of ["sdk10", "sdk11rc1"]) {
        const s = cohort.arms[arm], m = key => s[key].median;
        csv.push([id, arm, cohort.measured_pairs, ...metrics.flatMap(key => [s[key].median, s[key].q1, s[key].q3])].join(","));
        table.push(`| ${name} | ${arm === "sdk10" ? "10.0.100" : "11 RC1"} | ${(m("allocated_bytes") / 2 ** 30).toFixed(3)} | ${(m("peak_working_set_bytes") / 2 ** 20).toFixed(1)} | ${(m("peak_private_commit_bytes") / 2 ** 20).toFixed(1)} | ${m("gen0")} / ${m("gen1")} / ${m("gen2")} | ${(m("cpu_ns") / 1e9).toFixed(2)} | ${(m("wall_ns") / 1e9).toFixed(2)} |`);
    }
}
fs.writeFileSync(path.join(destination, "compilation-matrix.csv"), csv.join("\n") + "\n");
const articlePath = path.join(destination, "article.md");
let article = fs.readFileSync(articlePath, "utf8");
article = article.replace(/\| Workload \| SDK compiler \|[\s\S]*?(?=\r?\n\r?\nAllocated bytes)/, table.join("\n"));
article = article.replace(/FSharp\.Core has \*\*15 measured pairs\*\*[^\r\n]*/,
    "Core, FCS, and Nu have **15 measured pairs** each; FsToolkit, Oxpecker, and FsAutoComplete have **nine** each. Each cohort follows two warmup processes per compiler, with AB/BA order alternating. Allocation reductions and peak-memory changes are reported independently: allocating less does not guarantee a lower peak.");
if (results.ide.length) {
    const ideTable = ["| Production graph | SDK FCS | Projects checked | Allocated GiB | Peak RAM MiB | RAM after 10 s idle MiB | Live managed heap after full GC MiB | CPU / wall s |",
        "|---|---|---:|---:|---:|---:|---:|---|"];
    for (const [id, cohort] of Object.entries(results.statistics.ide)) {
        for (const arm of ["sdk10", "sdk11rc1"]) {
            const m = key => cohort.arms[arm][key].median;
            const count = results.ide.find(r => r.case === id).project_count;
            ideTable.push(`| ${names[id.replace(/-ide$/, "")]} | ${arm === "sdk10" ? "10.0.100" : "11 RC1"} | ${count} | ${(m("allocated_bytes") / 2 ** 30).toFixed(3)} | ${(m("peak_working_set_bytes") / 2 ** 20).toFixed(1)} | ${(m("idle_10s_working_set_bytes") / 2 ** 20).toFixed(1)} | ${(m("retained_heap_bytes") / 2 ** 20).toFixed(1)} | ${(m("cpu_ns") / 1e9).toFixed(2)} / ${(m("wall_ns") / 1e9).toFixed(2)} |`);
        }
    }
    article = article.replace(/\| (?:Solution|Production) graph \| SDK FCS \|[\s\S]*?(?=\r?\n\r?\n)/, ideTable.join("\n"));
    fs.writeFileSync(path.join(destination, "ide-matrix.csv"), [
        "workload,toolchain,measured_pairs,project_count,allocated_bytes,peak_working_set_bytes,idle_10s_working_set_bytes,retained_heap_bytes,cpu_ns,wall_ns",
        ...Object.entries(results.statistics.ide).flatMap(([id, c]) => Object.entries(c.arms).map(([arm, s]) =>
            [id, arm, c.measured_pairs, results.ide.find(r => r.case === id).project_count,
                ...["allocated_bytes", "peak_working_set_bytes", "idle_10s_working_set_bytes", "retained_heap_bytes", "cpu_ns", "wall_ns"].map(m => s[m].median)].join(",")))
    ].join("\n") + "\n");
}
if (results.generated_programs.length) {
    const descriptions = {
        Cart: "Cart-price List.fold", Rules: "Access-rule List.exists / List.forall",
        Telemetry: "Weighted Array.fold / Array.fold2", Option: "Partially applied Option.map",
        Nested: "Nested group folds", FilterMap: "Filtering and mapping a batch",
        Escaping: "Escaping callback control", NonCapturing: "Non-capturing callback control"
    };
    const programTable = ["| Workload, length 4 | 10.0.100 B/op | 11 RC1 B/op | Median ns/op (old / new) |",
        "|---|---:|---:|---|"];
    for (const [kernel, description] of Object.entries(descriptions)) {
        const a = results.generated_programs.filter(r => r.toolchain === "sdk10" && r.length === 4 && r.kernel === kernel);
        const b = results.generated_programs.filter(r => r.toolchain === "sdk11rc1" && r.length === 4 && r.kernel === kernel);
        const median = (rows, field) => quantile(rows.map(r => r[field]), 0.5);
        programTable.push(`| ${description} | ${median(a, "allocated_bytes_per_operation")} | ${median(b, "allocated_bytes_per_operation")} | ${median(a, "median_ns").toFixed(2)} / ${median(b, "median_ns").toFixed(2)} |`);
    }
    article = article.replace(/\| Workload, length 4 \|[\s\S]*?(?=\r?\n\r?\n)/, programTable.join("\n"));
    const fields = ["toolchain", "launch", "kernel", "length", "allocated_bytes_per_operation", "median_ns", "mean_ns", "iterations", "standard_deviation_ns"];
    fs.writeFileSync(path.join(destination, "generated-program-matrix.csv"),
        [fields.join(","), ...results.generated_programs.map(r => fields.map(f => r[f]).join(","))].join("\n") + "\n");
}
const controlRows = ["comparison,workload,toolchain,measured_pairs," + metrics.join(",")];
for (const [comparison, cases] of Object.entries(results.statistics.controls))
    for (const [id, cohort] of Object.entries(cases))
        for (const [arm, s] of Object.entries(cohort.arms))
            controlRows.push([comparison, id, arm, cohort.measured_pairs, ...metrics.map(m => s[m].median)].join(","));
fs.writeFileSync(path.join(destination, "controls.csv"), controlRows.join("\n") + "\n");
fs.writeFileSync(articlePath, article);
console.log(table.join("\n"));
