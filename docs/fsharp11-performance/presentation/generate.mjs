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
const sdkLabels = ['Old (SDK 10)', 'SDK 11 RC1', 'New (source)'];
const frameworkArms = ['old-framework', 'new-framework'];
const applicationArms = ['sdk10', 'vmr-rc2-r2r'];
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

function scalarTable(cohort, results, cases, metric, divisor, unit, arms = results.arms) {
    return {
        cohort, metric, unit, divisor, arms,
        rows: cases.map(([id, label]) => ({
            case: id, label,
            values: arms.map(arm => {
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
    schema_version: 2,
    evidence_commit: 'bc90ee6637fc9c7065da173bd36c8b06e2a8d6d4',
    generator_sha256_lf: sha256(lf(readFileSync(resolve(here, 'generate.mjs'), 'utf8'))),
    sources,
    statistic: 'Arm median; reduction = 100 * (1 - newer median / older median). No pooled cohorts or confidence claim.',
    units: { GB: 1e9, MB: 1e6, 'B/op': 'Managed bytes per measured application operation' },
    allocation_chart_scale: 'Bar widths scale separately within each workload from zero; labels are actual GB, never normalized values.',
    identities: { sdk: experiments[0].selection, sdk_payload: experiments[0].payload,
        portable_payloads: experiments[1].payloads, portable_runtimes: experiments[1].runtimes },
    counts: experiments.map(e => ({ campaign: e.campaign, ...e.counts })),
    sdk_allocation: scalarTable('third-wave', sdk, workloads, 'allocated_bytes', 1e9, 'GB'),
    framework_allocation: scalarTable('framework', portable, workloads, 'allocated_bytes', 1e9, 'GB', frameworkArms),
    sdk_heap: scalarTable('third-wave', sdk, graphs, 'retained_heap_bytes', 1e6, 'MB'),
    framework_heap: scalarTable('framework', portable, graphs, 'retained_heap_bytes', 1e6, 'MB', frameworkArms),
    sdk_peak: scalarTable('third-wave', sdk, workloads, 'peak_working_set_bytes', 1e6, 'MB'),
    framework_peak: scalarTable('framework', portable, workloads, 'peak_working_set_bytes', 1e6, 'MB', frameworkArms),
    sdk_cpu: scalarTable('third-wave', sdk, workloads, 'cpu_ns', 1e9, 's'),
    programs: kernels.map(([kernel, label]) => ({
        kernel, label, length: 4,
        values: applicationArms.map(arm => {
            const index = sdk.program_statistics.findIndex(p => p.kernel === kernel && p.length === 4 && p.arm === arm);
            const p = sdk.program_statistics[index];
            const launches = sdk.programs.filter(p => p.kernel === kernel && p.length === 4 && p.arm === arm);
            assert.equal(launches.length, 3);
            assert.equal(p.launches, 3);
            assert.equal(median(launches.map(p => p.bytes_per_op)), p.bytes_per_op.median);
            return { arm, value: p.bytes_per_op.median,
                source: `../third-wave/results.json#/program_statistics/${index}/bytes_per_op/median` };
        }),
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
    row.rc1_reduction_vs_old = reduction(row.values[0].median, row.values[1].median);
    row.new_reduction_vs_old = reduction(row.values[0].median, row.values[2].median);
}
for (const group of [data.framework_allocation, data.framework_heap]) {
    for (const row of group.rows) {
        row.new_reduction_vs_old = reduction(row.values[0].median, row.values[1].median);
        const published = portable.scalar_statistics[row.case].changes[
            'new-framework_vs_old-framework'][group.metric].ratio_of_medians_percent;
        assert(Math.abs(row.new_reduction_vs_old + published) < 1e-10, `${row.case}/framework denominator`);
    }
}
const values = row => row.values.map(v => v.value);
const row = (group, id) => group.rows.find(r => r.case === id);
const prLink = id => {
    const p = prs.find(p => Number(p.number) === id);
    assert(p, `Missing PR ${id}`);
    return `[#${id}](${p.url})`;
};
const contributing = prs.filter(p => p.rc2_source_status === 'ancestor')
    .sort((a, b) => a.merged_at.localeCompare(b.merged_at));
const cutoff = contributing.findIndex(p => p.rc1_source_status !== 'ancestor');
assert(cutoff > 0 && contributing.slice(cutoff).every(p => p.rc1_source_status !== 'ancestor'),
    'Release membership is not a single chronological cut-line');
data.contributing_prs = contributing;
data.contribution_window = {
    start: contributing[0].merged_at, end: contributing.at(-1).merged_at,
    count: contributing.length, in_rc1: cutoff, after_rc1: contributing.length - cutoff,
};
const dateLabel = timestamp => {
    const date = new Date(timestamp);
    return `${['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'][date.getUTCMonth()]} ${date.getUTCDate()}`;
};
const sdkFcsPeak = values(row(data.sdk_peak, 'fsharp-compiler-service'));
const sdkFcsCpu = values(row(data.sdk_cpu, 'fsharp-compiler-service'));
const classicFcsPeak = values(row(data.framework_peak, 'fsharp-compiler-service'));
const monitoringWall = data.monitoring.map(m => m.on_vs_off_percent.wall_ns.median);
const blocks = {
    opening: `F# 11 puts less pressure on the garbage collector and keeps less compiler data alive after IDE project checks. Across six real-world compilation workloads, the source-built compiler allocates **roughly 20-50% less than the compiler released in SDK 10.0.100**. Our multi-project IDE checks retain **about 30% less managed memory**.\n\nBehind this is a late-summer push: **${contributing.length} performance and supporting PRs** from auduchinok and T-Gro, merged between **${dateLabel(contributing[0].merged_at)} and ${dateLabel(contributing.at(-1).merged_at)}, 2026**. The work tackles unnecessary allocations, duplicated compiler data and memory held after typechecking. **${cutoff} changes are already in RC1**; the remaining **${contributing.length - cutoff} are in the RC2 source**, headed for .NET 11 and F# 11 GA.\n\nWe're excited to share those gains with F# developers: less allocation during compilation, less memory retained by IDE project checking, and fewer short-lived objects in your own code. One change reaches beyond the compiler: ${prLink(20422)} inlines higher-order \`FSharp.Core\` List/Array functions and their callbacks, so everyday code using operations such as \`List.fold\` can stop allocating a closure on every call.`,
    compilation: table(['Compilation workload', 'Old GB', 'RC1 GB', 'New GB', 'RC1 vs old: less allocation', 'New vs old: less allocation'],
        data.sdk_allocation.rows.map(r => [r.label, ...values(r).map(v => fmt(v, 3)),
            `${fmt(r.rc1_reduction_vs_old)}%`, `${fmt(r.new_reduction_vs_old)}%`])),
    headline: `RC1 already allocates **${range(data.sdk_allocation.rows.map(r => r.rc1_reduction_vs_old))} less than the released compiler**. The source-built F# 11 compiler takes that to **${range(data.sdk_allocation.rows.map(r => r.new_reduction_vs_old))} less**, depending on the project.`,
    'sdk-caveat': `**Lower allocation is not a promise of lower peak RAM or CPU time.** FCS's median peak resident RAM rises from **${fmt(sdkFcsPeak[0])} to ${fmt(sdkFcsPeak[2])} MB**, and CPU time from ${fmt(sdkFcsCpu[0], 2)} to ${fmt(sdkFcsCpu[2], 2)} seconds. Peak RAM is higher in ${data.sdk_peak.rows.filter(r => r.values[2].median > r.values[0].median).length} of the six workloads. [All RAM, CPU and wall-time values](../third-wave/compilation-matrix.csv) remain published.`,
    framework: table(['Compilation workload', 'Old GB', 'New GB', 'New vs old: less allocation'],
        data.framework_allocation.rows.map(r => [r.label, ...values(r).map(v => fmt(v, 3)),
            `${fmt(r.new_reduction_vs_old)}%`])),
    'framework-caveat': `The source-built compiler allocates **${range(data.framework_allocation.rows.map(r => r.new_reduction_vs_old))} less on .NET Framework**. Peak RAM does not uniformly improve here either: FCS rises from ${fmt(classicFcsPeak[0])} to ${fmt(classicFcsPeak[1])} MB.`,
    ide: table(['Project graph', 'Old MB', 'RC1 MB', 'New MB', 'New vs old: less retained memory'],
        data.sdk_heap.rows.map(r => [r.label, ...values(r).map(v => fmt(v)),
            `${fmt(reduction(r.values[0].median, r.values[2].median))}%`])),
    'portable-ide': `The same old-to-new comparison on .NET Framework gives **${range(data.framework_heap.rows.map(r => r.new_reduction_vs_old))} less retained managed memory**. [The detailed IDE measurements](../framework/ide-matrix.csv) remain available.`,
    programs: table(['Kernel, length 4', 'Old (SDK 10.0.100) B/op', 'New (source-built) B/op'],
        data.programs.map(p => [p.label, ...p.values.map(v => v.value)])),
    mechanism: `The [generated IL inventories](../third-wave/programs/) show function-object construction sites falling from **${data.function_construction_sites[0].count} to ${data.function_construction_sites[2].count}** between old and new output. These are static sites, not allocations per call. Separately collected [Oxpecker compiler profiles](../third-wave/profiles/) estimate generated-function allocation falling from **${fmt(data.profiles[0].verified_function_weighted_bytes / 1e6)} to ${fmt(data.profiles[2].verified_function_weighted_bytes / 1e6)} MB**. Those are weighted sampled bytes, classified by resolved \`FSharpFunc\` inheritance rather than just \`@\` in a name; the accepted traces report zero lost events.`,
    contributions: [
        `These **${contributing.length} PRs** are in the measured source snapshot, ordered by merge time. Each distinct change is listed separately; the cut-line marks RC1's source boundary.`,
        contributing.map((p, index) => `${index === cutoff ? `\n---\n\n**RC1 cut-line: the ${cutoff} changes above are in RC1. The ${contributing.length - cutoff} below are in the RC2 source, headed for GA.**\n\n` : ''}- **${dateLabel(p.merged_at)}** - ${prLink(Number(p.number))}: ${p.title.replace(/^\[MicroPerf\]\s*(Perf:\s*)?/, '')} (${p.author}).`).join('\n'),
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
function groupedChart(group, title, labels, palette, sharedMaximum = null) {
    const step = 50 + labels.length * 26, top = 130, x = 250, plot = 700;
    const bottom = top + group.rows.length * step - 22;
    const body = [text(28, 77, sharedMaximum === null
        ? 'Actual allocation in GB. Bar widths are scaled separately for each project.'
        : 'Live managed heap after full GC, in MB. All projects share the same scale.', 'small')];
    if (sharedMaximum !== null) {
        for (let tick = 0; tick <= sharedMaximum; tick += 100) {
            const tx = x + tick / sharedMaximum * plot;
            body.push(`<line x1="${tx}" y1="${top - 20}" x2="${tx}" y2="${bottom}" class="${tick ? 'grid' : 'axis'}"/>`,
                text(tx, top - 30, `${tick}`, 'small', 'middle'));
        }
    }
    group.rows.forEach((r, i) => {
        const y = top + i * step, maximum = sharedMaximum ?? Math.max(...r.values.map(v => v.value));
        body.push(text(28, y + 6, r.label, 'label'));
        if (sharedMaximum === null) body.push(text(x, y + 6, '0', 'small', 'middle'),
            `<line x1="${x}" y1="${y + 13}" x2="${x}" y2="${y + 18 + labels.length * 26}" class="axis"/>`);
        r.values.forEach((v, j) => {
            const value = v.value;
            assert(value >= 0 && value <= maximum, 'Chart axis would clip a value');
            const by = y + 18 + j * 26;
            body.push(text(x - 12, by + 16, labels[j], 'small', 'end'),
                rect(x, by, value / maximum * plot, 19, palette[j]),
                text(x + value / maximum * plot + 9, by + 16, `${fmt(value, group.unit === 'GB' ? 3 : 1)} ${group.unit}`, 'label'));
        });
    });
    body.push(text(28, bottom + 42, sharedMaximum === null
        ? 'Compare versions within each project; different projects use different bar scales. Every scale starts at zero.'
        : 'Retained managed heap (MB), measured while the checker, options and results remain alive.', 'small'),
        text(28, bottom + 68, 'Old = released SDK 10 generation. New = source-built F# 11. Medians of 12 measured runs.', 'small'));
    return svg(title, `${title}. Actual ${group.unit}; zero baseline. ${group.rows.map(r =>
        `${r.label}: ${r.values.map((v, i) => `${labels[i]} ${fmt(v.value, group.unit === 'GB' ? 3 : 1)} ${group.unit}`).join(', ')}.`).join(' ')}`,
        bottom + 92, body);
}
const output = new Map([
    ['compiler-allocation.svg', groupedChart(data.sdk_allocation, 'Less compiler allocation in every workload', sdkLabels, colors)],
    ['runtime-allocation.svg', groupedChart(data.framework_allocation, 'The allocation gains also reach .NET Framework',
        ['Old (released)', 'New (source)'], [colors[0], colors[2]])],
    ['ide-retained-heap.svg', groupedChart(data.sdk_heap, 'Less managed heap retained after checking', sdkLabels, colors, 600)],
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
