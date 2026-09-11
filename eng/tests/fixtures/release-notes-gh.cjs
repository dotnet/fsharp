#!/usr/bin/env node
const { readFileSync, writeFileSync, existsSync } = require('node:fs');
const { join } = require('node:path');
const { spawnSync } = require('node:child_process');

const directory = process.env.FIXTURE_DIR;
const fixture = JSON.parse(readFileSync(join(directory, 'fixture.json'), 'utf8'));
const args = process.argv.slice(2);
const endpoint = args.find(arg => arg.startsWith('repos/') || arg.startsWith('https://api.github.com/'));
let result;

if (fixture.apiError) {
    console.error('GitHub API unavailable');
    process.exit(1);
}

if (endpoint === 'repos/dotnet/fsharp/pulls/123') {
    const counter = join(directory, 'reads');
    const index = existsSync(counter) ? Number(readFileSync(counter, 'utf8')) : 0;
    if (index > 0 && fixture.finalApiError) {
        console.error('GitHub API unavailable during final validation');
        process.exit(1);
    }
    result = fixture.prs[Math.min(index, fixture.prs.length - 1)];
    writeFileSync(counter, String(index + 1));
} else if (endpoint === 'repos/dotnet/fsharp/pulls/123/files') {
    result = [fixture.files];
} else if (endpoint?.includes('/contents/eng/Versions.props?ref=')) {
    result = '<Project><VSMajorVersion>18</VSMajorVersion></Project>';
} else if (endpoint?.includes('/contents/docs/release-notes/')) {
    result = fixture.note;
} else {
    console.error(`Unexpected GitHub request: ${args.join(' ')}`);
    process.exit(1);
}

const json = typeof result === 'string' ? result : JSON.stringify(result);
const query = args.indexOf('--jq');
if (query >= 0) {
    const filtered = spawnSync('jq', ['-r', args[query + 1]], { input: json, encoding: 'utf8' });
    process.stdout.write(filtered.stdout);
    process.stderr.write(filtered.stderr);
    process.exit(filtered.status);
}
console.log(json);
