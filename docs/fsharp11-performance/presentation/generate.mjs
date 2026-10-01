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
const functionInlining = input('presentation', 'function-inlining.json');
assert.equal(functionInlining.baseline_fsharp_source, experiments[1].payloads.old.fsharp_source);
assert.equal(functionInlining.candidate_fsharp_source, experiments[0].selection.rc2_and_main_fsharp_source);
for (const group of functionInlining.groups) {
    assert(group.functions.length > 0 && group.functions.every(name => /^[a-z]\w*$/.test(name)));
    assert.equal(new Set(group.functions).size, group.functions.length, 'Duplicate function in inventory');
}
const workloads = [
    ['fsharp-core', 'FSharp.Core'], ['fsharp-compiler-service', 'FSharp.Compiler.Service'],
    ['fstoolkit', 'FsToolkit.ErrorHandling'], ['oxpecker', 'Oxpecker'],
    ['nu', 'Nu'], ['fsautocomplete', 'FsAutoComplete'],
];
const graphs = [['fsautocomplete-ide', 'FsAutoComplete'], ['oxpecker-ide', 'Oxpecker']];
const operations = [
    ['Cart', 'Cart-price List.fold',
        'List.fold (fun s struct (p, q) -> s + p * q * (100 - d) / 100) 0 items'],
    ['Rules', 'Access-rule exists / forall',
        'let any = List.exists (fun x -> x > lo) xs\nList.forall (fun x -> x < hi) xs && any'],
    ['Telemetry', 'Weighted Array.fold / fold2',
        'Array.fold (fun s x -> s + x * k) 0 xs + Array.fold2 (fun s x w -> s + x * w * k) 0 xs ws'],
    ['Option', 'Partially applied Option.map',
        'Option.defaultValue 0 (Option.map (addOffset k) opt)'],
    ['Nested', 'Nested group folds',
        'List.fold (fun s xs -> s + List.fold (fun t x -> t + x * k) 0 xs) 0 groups'],
    ['FilterMap', 'Filtering and mapping',
        'List.sum (List.map (fun x -> x + k) (List.filter (fun x -> x > lo) xs))'],
    ['Escaping', 'Escaping closure', 'saved <- fun x -> x + offset\nsaved n'],
    ['NonCapturing', 'Non-capturing lambda', 'List.fold (+) seed xs'],
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
const reduction = (older, newer) => (1 - newer / older) * 100;
const range = values => `${fmt(Math.min(...values))}%-${fmt(Math.max(...values))}%`;
const table = (headers, rows, rightAlignValues = true) => [
    `| ${headers.join(' | ')} |`,
    `| ${headers.map((_, i) => i && rightAlignValues ? '---:' : '---').join(' | ')} |`,
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
    programs: operations.map(([kernel, label, code]) => ({
        kernel, label, code, length: 4,
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
    function_inlining: functionInlining,
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
const prLink = (id, label = `#${id}`) => {
    const p = prs.find(p => Number(p.number) === id) ??
        functionInlining.changes.find(p => Number(p.number) === id);
    assert(p, `Missing PR ${id}`);
    return `[${label}](${p.url})`;
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
const blocks = {
    opening: `F# 11 brings **roughly 20-50% less allocation during compilation** across six real-world projects and **about 30% less memory retained after IDE project checks**. **And this is a win for your F# code, too!** Better ${prLink(20422, 'inlining of `FSharp.Core` calls')} eliminates closure allocations in everyday functional code. Recompile with the new compiler and \`FSharp.Core\`, and these optimizations reach your applications and libraries, not just the compiler.\n\nBehind these gains are **${contributing.length} performance and supporting PRs** from auduchinok and T-Gro, merged between **${dateLabel(contributing[0].merged_at)} and ${dateLabel(contributing.at(-1).merged_at)}, 2026**. **${cutoff} are already in RC1**; the remaining **${contributing.length - cutoff} are in the RC2 source**, headed for .NET 11 and F# 11 GA.`,
    compilation: table(['Compilation workload', 'Old GB', 'RC1 GB', 'New GB', 'RC1 vs old: less allocation', 'New vs old: less allocation'],
        data.sdk_allocation.rows.map(r => [r.label, ...values(r).map(v => fmt(v, 3)),
            `${fmt(r.rc1_reduction_vs_old)}%`, `${fmt(r.new_reduction_vs_old)}%`])),
    'sdk-caveat': `Peak compilation RAM rose in ${data.sdk_peak.rows.filter(r => r.values[2].median > r.values[0].median).length} of six SDK workloads, and FCS used more CPU time.`,
    framework: table(['Compilation workload', 'Old GB', 'New GB', 'New vs old: less allocation'],
        data.framework_allocation.rows.map(r => [r.label, ...values(r).map(v => fmt(v, 3)),
            `${fmt(r.new_reduction_vs_old)}%`])),
    ide: table(['Project graph', 'Old MB', 'RC1 MB', 'New MB', 'New vs old: less retained memory'],
        data.sdk_heap.rows.map(r => [r.label, ...values(r).map(v => fmt(v)),
            `${fmt(reduction(r.values[0].median, r.values[2].median))}%`])),
    'portable-ide': `On .NET Framework, retained memory falls by **${range(data.framework_heap.rows.map(r => r.new_reduction_vs_old))}** too.`,
    programs: table(['F# code (simplified)', 'Old B/op', 'New B/op'],
        data.programs.map(p => [p.code.split('\n').map(line => `\`${line}\``).join('<br>'),
            ...p.values.map(v => v.value)])),
    'function-inlining': `These existing functions gained explicit lambda inlining in ${prLink(20422)} and the earlier ${prLink(19869, '`Array.init` change')}:\n\n` +
        table(['Module', 'Functions gaining explicit lambda inlining'],
            functionInlining.groups.map(group => [`\`${group.module}\` (${group.functions.length})`,
                group.functions.map(name => `\`${name}\``).join(', ')]), false),
    contributions: [
        '**Already in RC1**',
        contributing.map((p, index) => `${index === cutoff ? '\n---\n\n**In the RC2 source, headed for GA**\n\n' : ''}- **${dateLabel(p.merged_at)}** - ${prLink(Number(p.number))}: ${p.title.replace(/^\[MicroPerf\]\s*(Perf:\s*)?/, '').replace(/\bopaque callbacks\b/g, 'function arguments')} (${p.author}${Number(p.number) === 20506 ? '; concurrency benefit not measured here' : ''}).`).join('\n'),
    ].join('\n\n'),
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
