import assert from 'node:assert/strict';
import { createHash } from 'node:crypto';
import { readFileSync, writeFileSync } from 'node:fs';
import { dirname, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';

const here = dirname(fileURLToPath(import.meta.url));
const check = process.argv.slice(2).includes('--check');
assert(process.argv.slice(2).every(arg => arg === '--check'), 'Usage: node generate.mjs [--check]');
const lf = text => text.replace(/\r\n/g, '\n');
const sha256 = bytes => createHash('sha256').update(bytes).digest('hex');
const sources = [];
function input(cohort, name) {
    const bytes = readFileSync(resolve(here, '..', cohort, ...name.split('/')));
    sources.push({ path: `../${cohort}/${name}`, sha256: sha256(bytes) });
    return name.endsWith('.json') ? JSON.parse(bytes) : bytes;
}
const sdk = input('third-wave', 'results.json');
const portable = input('framework', 'results.json');
const experiments = [input('third-wave', 'experiment.json'), input('framework', 'experiment.json')];
const provenance = input('third-wave', 'provenance.json');
const prs = input('third-wave', 'contributions.json');
const audit = input('third-wave', 'vmr-audit.json');
for (const cohort of ['third-wave', 'framework']) {
    for (const name of ['README.md', 'compilation-matrix.csv', 'ide-matrix.csv', 'program-matrix.csv'])
        input(cohort, name);
}
for (const name of ['article.md', 'tables.json', 'contributions.csv']) input('third-wave', name);
for (const name of ['provenance.json', 'output-identities.json', 'tables.md']) input('framework', name);
const il = sdk.arms.map(arm => input('third-wave', `programs/${arm}-il.json`));
const workloads = [
    ['fsharp-core', 'FSharp.Core'], ['fsharp-compiler-service', 'FSharp.Compiler.Service'],
    ['fstoolkit', 'FsToolkit.ErrorHandling'], ['oxpecker', 'Oxpecker'],
    ['nu', 'Nu'], ['fsautocomplete', 'FsAutoComplete'],
];
const graphs = [['fsautocomplete-ide', 'FsAutoComplete'], ['oxpecker-ide', 'Oxpecker']];
const kernels = [
    ['Cart', 'Cart-price List.fold'], ['Rules', 'Access-rule exists / forall'],
    ['Telemetry', 'Weighted Array.fold / fold2'], ['Option', 'Partially applied Option.map'],
    ['Nested', 'Nested group folds'], ['FilterMap', 'Filtering and mapping'],
    ['Escaping', 'Escaping callback control'], ['NonCapturing', 'Non-capturing control'],
];
const runtimes = ['framework', 'net10', 'net11'];
const sdkLabels = ['SDK 10', 'SDK 11 RC1', 'Local F# 11'];
const runtimeLabels = ['.NET Framework 4.8.1', '.NET 10.0.0', '.NET 11 RC1'];
const median = values => {
    const sorted = [...values].sort((a, b) => a - b);
    assert(sorted.length && sorted.every(Number.isFinite), 'Missing or non-finite observations');
    return (sorted[Math.floor((sorted.length - 1) / 2)] + sorted[Math.floor(sorted.length / 2)]) / 2;
};
const fmt = (value, digits = 1) => value.toFixed(digits);
const signed = value => `${value >= 0 ? '+' : ''}${fmt(value, 2)}%`;
const reduction = (older, newer) => (1 - newer / older) * 100;
const range = values => `${fmt(Math.min(...values))}%-${fmt(Math.max(...values))}%`;
const table = (headers, rows) => [
    `| ${headers.join(' | ')} |`,
    `| ${headers.map((_, i) => i ? '---:' : '---').join(' | ')} |`,
    ...rows.map(row => `| ${row.join(' | ')} |`),
].join('\n');

function scalarTable(cohort, results, cases, metric, divisor, unit) {
    return {
        cohort, metric, unit, divisor, arms: results.arms,
        rows: cases.map(([id, label]) => ({
            case: id, label,
            values: results.arms.map(arm => {
                const stats = results.scalar_statistics[id];
                const published = stats.arms[arm].metrics[metric].median;
                const measured = results.observations.filter(o =>
                    !o.warmup && (o.arm ?? o.toolchain) === arm && o.scope === stats.operation &&
                    o.case.replace(/-ide$/, '') === id.replace(/-ide$/, ''));
                assert.equal(measured.length, 12, `${cohort}/${id}/${arm} sample count`);
                assert.equal(median(measured.map(o => o[metric])), published, `${id}/${arm}/${metric}`);
                return {
                    arm, median: published, value: published / divisor,
                    source: `../${cohort}/results.json#/scalar_statistics/${id}/arms/${arm}/metrics/${metric}/median`,
                };
            }),
        })),
    };
}
const data = {
    schema_version: 1,
    evidence_commit: 'bc90ee6637fc9c7065da173bd36c8b06e2a8d6d4',
    generator_sha256_lf: sha256(lf(readFileSync(resolve(here, 'generate.mjs'), 'utf8'))),
    sources,
    statistic: 'Arm median; reduction = 100 * (1 - newer median / older median). No pooled cohorts or confidence claim.',
    units: { GiB: 1073741824, MiB: 1048576, 'B/op': 'Managed bytes per measured application operation' },
    identities: { sdk: experiments[0].selection, sdk_payload: experiments[0].payload,
        portable_payloads: experiments[1].payloads, portable_runtimes: experiments[1].runtimes },
    counts: experiments.map(e => ({ campaign: e.campaign, ...e.counts })),
    sdk_allocation: scalarTable('third-wave', sdk, workloads, 'allocated_bytes', 2 ** 30, 'GiB'),
    portable_allocation: scalarTable('framework', portable, workloads, 'allocated_bytes', 2 ** 30, 'GiB'),
    sdk_heap: scalarTable('third-wave', sdk, graphs, 'retained_heap_bytes', 2 ** 20, 'MiB'),
    portable_heap: scalarTable('framework', portable, graphs, 'retained_heap_bytes', 2 ** 20, 'MiB'),
    sdk_peak: scalarTable('third-wave', sdk, workloads, 'peak_working_set_bytes', 2 ** 20, 'MiB'),
    portable_peak: scalarTable('framework', portable, workloads, 'peak_working_set_bytes', 2 ** 20, 'MiB'),
    sdk_cpu: scalarTable('third-wave', sdk, workloads, 'cpu_ns', 1e9, 's'),
    portable_cpu: scalarTable('framework', portable, workloads, 'cpu_ns', 1e9, 's'),
    programs: kernels.map(([kernel, label]) => ({
        kernel, label, length: 4,
        cohorts: [sdk, portable].map((results, cohort) => results.arms.map(arm => {
            const index = results.program_statistics.findIndex(p => p.kernel === kernel && p.length === 4 && p.arm === arm);
            const p = results.program_statistics[index];
            const launches = results.programs.filter(p => p.kernel === kernel && p.length === 4 && p.arm === arm);
            assert.equal(launches.length, 3);
            assert.equal(p.launches, 3);
            assert.equal(median(launches.map(p => p.bytes_per_op)), p.bytes_per_op.median);
            return { arm, value: p.bytes_per_op.median,
                source: `../${cohort ? 'framework' : 'third-wave'}/results.json#/program_statistics/${index}/bytes_per_op/median` };
        })),
    })),
    function_construction_sites: il.map((inventory, i) => ({
        arm: sdk.arms[i], count: inventory.newobj_sites.filter(site => site.fsharp_function).length,
        source: `../third-wave/programs/${sdk.arms[i]}-il.json#/newobj_sites`,
    })),
    profiles: provenance.profiles,
    monitoring: portable.monitoring_statistics,
};
assert.equal(audit[1].production_tree, audit[2].production_tree);
for (const [i, results] of [sdk, portable].entries()) {
    const counts = experiments[i].counts;
    assert.equal(results.observations.length, counts.scalar);
    assert.equal(results.observations.filter(o => !o.warmup).length, counts.measured_scalar);
    assert.equal(results.programs.length, counts.program_launch_cases);
    assert.equal(results.programs.reduce((n, p) => n + p.actual_iterations.length, 0), counts.measured_program_iterations);
}
for (const row of data.sdk_allocation.rows) {
    row.baseline100 = row.values.map(v => v.median / row.values[0].median * 100);
    row.local_reduction_vs_sdk10 = reduction(row.values[0].median, row.values[2].median);
    row.local_reduction_vs_rc1 = reduction(row.values[1].median, row.values[2].median);
}
for (const group of [data.portable_allocation, data.portable_heap]) {
    for (const row of group.rows) {
        row.comparisons = runtimes.map((runtime, i) => {
            const older = row.values[2 * i], newer = row.values[2 * i + 1];
            const delta = reduction(older.median, newer.median);
            const published = portable.scalar_statistics[row.case].changes[
                `new-${runtime}_vs_old-${runtime}`][group.metric].ratio_of_medians_percent;
            assert(Math.abs(delta + published) < 1e-10, `${row.case}/${runtime} denominator`);
            return { runtime, baseline100: newer.median / older.median * 100, reduction_percent: delta };
        });
    }
}
const values = row => row.values.map(v => v.value);
const row = (group, id) => group.rows.find(r => r.case === id);
const prLink = id => {
    const p = prs.find(p => Number(p.number) === id);
    assert(p, `Missing PR ${id}`);
    return `[#${id}](${p.url})`;
};
const prGroups = [
    ['auduchinok', 'Avoid eager metadata traversal and unnecessary tables or retention', [20090, 20092, 20249, 20250], true],
    ['auduchinok', 'Share calling conventions, type references and pickled references', [20254, 20259, 20301], true],
    ['auduchinok', 'Share imported assemblies across projects and prevent repeated background work', [20296, 20481], false],
    ['T-Gro', 'Reduce hot-path and optimizer allocation, including constraint-solver and stack-guard closures', [20348, 20363, 20367, 20368], false],
    ['T-Gro', 'Inline higher-order List/Array functions and eliminate partial-application closures', [20422, 20487], false],
    ['T-Gro', 'Stabilize closure optimization for F# 11 and select net10.0 Core for SDK tools', [20571, 20555], false],
];
for (const [, , ids, inRc1] of prGroups) for (const id of ids) {
    const p = prs.find(p => Number(p.number) === id);
    assert.equal(p.rc2_source_status, 'ancestor');
    assert.equal(p.rc1_source_status === 'ancestor', inRc1);
}
data.contributing_prs = prGroups.map(([author, mechanism, ids]) => ({ author, mechanism, prs: ids }));
const sdkFcsPeak = values(row(data.sdk_peak, 'fsharp-compiler-service'));
const sdkFcsCpu = values(row(data.sdk_cpu, 'fsharp-compiler-service'));
const classicFcsPeak = values(row(data.portable_peak, 'fsharp-compiler-service'));
const classicFcsAllocation = values(row(data.portable_allocation, 'fsharp-compiler-service'));
const classicCorePeak = values(row(data.portable_peak, 'fsharp-core'));
const monitoringWall = data.monitoring.map(m => m.on_vs_off_percent.wall_ns.median);
const blocks = {
    compilation: table(['Compilation workload', 'SDK 10 GiB', 'SDK 11 RC1 GiB', 'Local F# 11 GiB', 'Less vs SDK 10', 'Less vs RC1'],
        data.sdk_allocation.rows.map(r => [r.label, ...values(r).map(v => fmt(v, 3)),
            `${fmt(r.local_reduction_vs_sdk10)}%`, `${fmt(r.local_reduction_vs_rc1)}%`])),
    headline: `Across all six workloads, the local F# 11 payload allocates **${range(data.sdk_allocation.rows.map(r => r.local_reduction_vs_sdk10))} less than SDK 10**, and **${range(data.sdk_allocation.rows.map(r => r.local_reduction_vs_rc1))} less than RC1**. The later changes matter: RC1 is not the endpoint of this work.`,
    'sdk-caveat': `**Less allocation does not mean every memory or CPU metric improves.** FCS's median peak resident RAM rises from **${fmt(sdkFcsPeak[0])} to ${fmt(sdkFcsPeak[2])} MiB**, while its CPU time rises from ${fmt(sdkFcsCpu[0], 2)} to ${fmt(sdkFcsCpu[2], 2)} seconds. The median RAM peak is higher than SDK 10 in ${data.sdk_peak.rows.filter(r => r.values[2].median > r.values[0].median).length} of the six workloads. Those regressions are why the headline is allocation, not simply "less memory" or "everything is faster." [All RAM, CPU and wall-time values](../third-wave/compilation-matrix.csv) remain published.`,
    framework: table(['Compilation workload', 'Framework old GiB', 'Framework new GiB', 'Less on Framework', 'Less on .NET 10', 'Less on .NET 11'],
        data.portable_allocation.rows.map(r => [r.label, ...values(r).slice(0, 2).map(v => fmt(v, 3)),
            ...r.comparisons.map(c => `${fmt(c.reduction_percent)}%`)])),
    'framework-caveat': `On .NET Framework, allocation falls by **${range(data.portable_allocation.rows.map(r => r.comparisons[0].reduction_percent))}**. The RAM qualification still applies: FCS allocation falls from **${fmt(classicFcsAllocation[0], 3)} to ${fmt(classicFcsAllocation[1], 3)} GiB**, but peak RAM rises from **${fmt(classicFcsPeak[0])} to ${fmt(classicFcsPeak[1])} MiB**. Core's peak also rises (${fmt(classicCorePeak[0])} to ${fmt(classicCorePeak[1])} MiB on Framework, and it rises on both modern runtimes).`,
    ide: table(['Project graph', 'SDK 10 MiB', 'SDK 11 RC1 MiB', 'Local F# 11 MiB', 'Less vs SDK 10'],
        data.sdk_heap.rows.map(r => [r.label, ...values(r).map(v => fmt(v)),
            `${fmt(reduction(r.values[0].median, r.values[2].median))}%`])),
    'portable-ide': `The separate portable experiment repeats the result on Framework: ${data.portable_heap.rows.map(r =>
        `**${r.label}: ${fmt(r.values[0].value)} to ${fmt(r.values[1].value)} MiB**`).join('; ')}. Across both graphs and all three runtimes, retained managed heap is **${range(data.portable_heap.rows.flatMap(r => r.comparisons.map(c => c.reduction_percent)))} lower** with the newer payload. [All six-arm IDE values](../framework/ide-matrix.csv) are available.`,
    programs: table(['Kernel, length 4', 'SDK 10 / RC1 / local B/op', 'Framework old -> new B/op', '.NET 10 and .NET 11 old -> new B/op'],
        data.programs.map(p => {
            const [s, f] = p.cohorts.map(c => c.map(v => v.value));
            assert.deepEqual(f.slice(2, 4), f.slice(4, 6), `${p.kernel} modern allocations`);
            return [p.label, s.join(' / '), f.slice(0, 2).join(' -> '), f.slice(2, 4).join(' -> ')];
        })),
    mechanism: `The [generated IL inventories](../third-wave/programs/) provide a second view: function-object construction sites fall from **${data.function_construction_sites.map(s => s.count).join(' / ')}** in SDK 10 / RC1 / local output. These are static construction sites, not objects allocated per call. Separately collected [Oxpecker compiler profiles](../third-wave/profiles/) estimate **${data.profiles.map(p => fmt(p.verified_function_weighted_bytes / 2 ** 20)).join(' / ')} MiB** of generated-function allocation in that same order. These are **weighted sampled bytes**, not exact object counts or the scalar totals above. Resolved \`FSharpFunc\` inheritance, not an \`@\` in a type name, determines the classification; all three accepted traces report zero lost events.`,
    contributions: [
        `Of the ${prs.length} surveyed PRs, **${prs.filter(p => p.rc1_source_status === 'ancestor').length} are in RC1's source ancestry and ${prs.filter(p => p.rc2_source_status === 'ancestor').length} in the selected later source**. Selected mechanisms:`,
        ...prGroups.map(([author, mechanism, ids, inRc1]) =>
            `- **${mechanism}** (${author}): ${ids.map(prLink).join(', ')}. ${inRc1 ? 'Already in RC1.' : 'After RC1; present in the local payload.'}`),
        `The experimental-branch merges ${[20424, 20425, 20439].map(prLink).join(', ')} are **absent from the selected mainline ancestry** and are not credited as shipped here. ${prLink(20506)} adds MSBuild concurrency support, but these serial compiler-only runs do **not** measure its benefit.`,
        `The [full PR inventory](../third-wave/contributions.csv), [VMR audit](../third-wave/vmr-audit.json) and [exact source selection](../third-wave/experiment.json) retain the release boundaries. The later compiler source is \`${experiments[0].selection.rc2_and_main_fsharp_source}\`; the pinned RC2 and main production tree is \`${audit[1].production_tree}\`.`,
    ].join('\n\n'),
    instrumentation: `Allocation accounting is also explicit: modern .NET uses \`GC.GetTotalAllocatedBytes(true)\`; Framework uses \`AppDomain.MonitoringTotalAllocatedMemorySize\` for the compiler domain, including retired threads. Known-allocation calibration agreed within 2%. Framework monitoring-on/off controls have median paired wall changes from **${signed(Math.min(...monitoringWall))} to ${signed(Math.max(...monitoringWall))}**, but individual peak-RAM changes reach **${signed(Math.max(...data.monitoring.map(m => m.on_vs_off_percent.peak_working_set_bytes.max)))}**. This is not proof of zero instrumentation overhead. See [counter calibration and controls](../framework/README.md#boundaries-and-stability).`,
    evidence: table(['Separate campaign', 'Scalar observations (measured + warmup)', 'Application launch/cases', 'Measured application iterations'],
        experiments.map((e, i) => [`[${i ? 'Portable / Framework' : 'SDK / local R2R'}](../${i ? 'framework' : 'third-wave'}/README.md)`,
            `${e.counts.scalar} (${e.counts.measured_scalar} + ${e.counts.warmup_scalar})`,
            e.counts.program_launch_cases, e.counts.measured_program_iterations])) +
        `\n\nThe portable campaign additionally retains ${portable.monitoring_controls.length} instrumentation-control records. [SDK raw results](../third-wave/results.json) and [portable raw results](../framework/results.json) include observations, warmups and all metric distributions.`,
};

const colors = ['#64748b', '#2563a6', '#087f73'];
const escape = value => String(value).replace(/[&<>"']/g, ch =>
    ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&apos;' })[ch]);
const text = (x, y, value, cls = '', anchor = 'start') =>
    `<text x="${x}" y="${y}" class="${cls}" text-anchor="${anchor}">${escape(value)}</text>`;
const rect = (x, y, width, height, color) =>
    `<rect x="${x}" y="${y}" width="${width.toFixed(3)}" height="${height}" fill="${color}"/>`;
const svg = (title, description, height, body) =>
    `<svg xmlns="http://www.w3.org/2000/svg" width="1100" height="${height}" viewBox="0 0 1100 ${height}" role="img" aria-labelledby="title desc">
<title id="title">${escape(title)}</title><desc id="desc">${escape(description)}</desc>
<style>text{font-family:Segoe UI,Arial,sans-serif;font-size:16px;fill:#172b3a}.title{font-size:27px;font-weight:700}.label{font-weight:600}.small{font-size:14px;fill:#405466}.grid{stroke:#d7e0e6;stroke-width:1}.axis{stroke:#6b7d8a;stroke-width:1}</style>
<rect width="1100" height="${height}" fill="#fff"/>
${text(28, 44, title, 'title')}\n${body.join('\n')}\n</svg>\n`;
function groupedChart(group, title, unit, maximum, ticks, normalized) {
    const step = 128, top = 142, x = 230, plot = 730;
    const bottom = top + group.rows.length * step - 22;
    const body = [text(28, 77, normalized ? 'Allocation index: each project has its own SDK 10 baseline of 100.' :
        'Live managed heap after full GC; checker, project options and results remain rooted.', 'small')];
    ticks.forEach(tick => {
        const tx = x + tick / maximum * plot;
        body.push(`<line x1="${tx}" y1="${top - 20}" x2="${tx}" y2="${bottom}" class="${tick ? 'grid' : 'axis'}"/>`,
            text(tx, top - 30, tick, 'small', 'middle'));
    });
    group.rows.forEach((r, i) => {
        const y = top + i * step;
        body.push(text(28, y + 6, r.label, 'label'));
        r.values.forEach((v, j) => {
            const value = normalized ? r.baseline100[j] : v.value;
            assert(value >= 0 && value <= maximum, 'Chart axis would clip a value');
            const by = y + 18 + j * 26;
            body.push(text(x - 12, by + 16, sdkLabels[j], 'small', 'end'),
                rect(x, by, value / maximum * plot, 19, colors[j]),
                text(x + value / maximum * plot + 9, by + 16, fmt(value), 'label'));
        });
    });
    body.push(text(x + plot / 2, bottom + 35, unit, 'label', 'middle'),
        text(28, bottom + 66, 'Lower is better. Medians of 12 measured runs per arm; no confidence interval is implied.', 'small'),
        text(28, bottom + 91, 'Local F# 11 = self-hosted RC2-source Release/R2R payload on .NET 11 RC1, not an official RC2 SDK.', 'small'));
    return svg(title, `${title}. ${unit}. Zero baseline. ${group.rows.map(r =>
        `${r.label}: ${r.values.map((v, i) => `${sdkLabels[i]} ${fmt(normalized ? r.baseline100[i] : v.value)}`).join(', ')}.`).join(' ')}`,
        bottom + 116, body);
}
function runtimeChart() {
    const body = [text(28, 77, 'IL-only netstandard2.0 payloads. Each runtime/project normalizes its older payload to 100.', 'small'),
        rect(28, 98, 18, 14, colors[0]), text(54, 111, 'Old = 100', 'small'),
        rect(175, 98, 18, 14, colors[2]), text(201, 111, 'New; directly labeled as % of old', 'small')];
    const top = 198, step = 52, width = 205;
    runtimeLabels.forEach((label, i) => {
        const x = 270 + i * 275;
        body.push(text(x + width / 2, 149, label, 'label', 'middle'));
        for (const tick of [0, 50, 100]) {
            const tx = x + tick / 100 * width;
            body.push(`<line x1="${tx}" y1="185" x2="${tx}" y2="500" class="${tick ? 'grid' : 'axis'}"/>`,
                text(tx, 174, tick, 'small', 'middle'));
        }
        data.portable_allocation.rows.forEach((r, j) => {
            const value = r.comparisons[i].baseline100, y = top + j * step;
            assert(value >= 0 && value <= 100, 'Runtime chart axis would clip a value');
            if (i === 0) body.push(text(28, y + 20, r.label, 'label'));
            body.push(rect(x, y, width, 27, colors[0]), rect(x, y, value / 100 * width, 27, colors[2]),
                text(x + width + 9, y + 20, fmt(value), 'label'));
        });
    });
    body.push(text(28, 540, 'Allocation index (% of older payload); all axes start at zero. Lower is better.', 'label'),
        text(28, 570, '12 measured runs per arm/workload. Separate from SDK/R2R: do not pool the two experiments.', 'small'));
    return svg('The allocation gains also reach .NET Framework',
        `New allocation as percent of the older payload, normalized separately within each runtime. ${data.portable_allocation.rows.map(r =>
            `${r.label}: ${r.comparisons.map((c, i) => `${runtimeLabels[i]} ${fmt(c.baseline100)}`).join(', ')}.`).join(' ')}`,
        598, body);
}
const output = new Map([
    ['compiler-allocation.svg', groupedChart(data.sdk_allocation, 'Less compiler allocation in every workload', 'Allocation index (SDK 10 = 100)', 100, [0, 25, 50, 75, 100], true)],
    ['runtime-allocation.svg', runtimeChart()],
    ['ide-retained-heap.svg', groupedChart(data.sdk_heap, 'Less managed heap retained after checking', 'Retained managed heap (MiB)', 600, [0, 100, 200, 300, 400, 500, 600], false)],
    ['chart-data.json', `${JSON.stringify(data, null, 2)}\n`],
]);
let article = lf(readFileSync(resolve(here, 'article.md'), 'utf8'));
for (const [name, content] of Object.entries(blocks)) {
    const pattern = new RegExp(`<!-- generated:${name} -->[\\s\\S]*?<!-- /generated:${name} -->`, 'g');
    assert.equal([...article.matchAll(pattern)].length, 1, `Missing or duplicate article block: ${name}`);
    article = article.replace(pattern, `<!-- generated:${name} -->\n${content}\n<!-- /generated:${name} -->`);
}
output.set('article.md', article);
for (const [name, content] of output) {
    if (check) assert.equal(lf(readFileSync(resolve(here, name), 'utf8')), content, `Stale output: ${name}`);
    else writeFileSync(resolve(here, name), content);
}
console.log(`${check ? 'Verified' : 'Generated'} article, three SVGs and chart-data.json; medians checked against raw observations.`);
