'use strict';

const assert = require('node:assert/strict');
const { execFile } = require('node:child_process');
const crypto = require('node:crypto');
const fs = require('node:fs/promises');
const http = require('node:http');
const path = require('node:path');
const { test } = require('node:test');
const { promisify } = require('node:util');
const { installArchive } = require('./toolchain-installer.cjs');

const digest = (bytes, algorithm) => crypto.createHash(algorithm).update(bytes).digest('hex');
const parameters = ['Version', 'InstallDir', 'Architecture', 'Runtime', 'NoPath', 'KeepZip', 'ZipPath', 'AzureFeed', 'SkipNonVersionedFiles'];
const help = '--version --install-dir --architecture --runtime --no-path --keep-zip --zip-path --azure-feed --os --skip-non-versioned-files';
const payload = Buffer.from('verified archive bytes');

function request(url, method = 'GET') {
  return new Promise((resolve, reject) => {
    const req = http.request(url, { method, agent: false }, response => {
      const chunks = [];
      response.on('data', chunk => chunks.push(chunk));
      response.on('end', () => resolve({ status: response.statusCode, bytes: Buffer.concat(chunks) }));
    });
    req.on('error', reject);
    req.end();
  });
}

async function fixture(t, { product = 'sdk', os = 'win', architecture = 'x64', rid = `${os}-${architecture}`, version = '11.0.100-preview.3.26159.112' } = {}) {
  const directory = await fs.mkdtemp(path.join(__dirname, '.installer-test-'));
  t.after(() => fs.rm(directory, { recursive: true, force: true }));
  const windows = os === 'win';
  const filename = `${product === 'sdk' ? 'dotnet-sdk' : product === 'dotnet' ? 'dotnet-runtime' : `${product}-runtime`}-${version}-${rid}.${windows ? 'zip' : 'tar.gz'}`;
  const helperRoot = path.join(directory, 'eng', 'common');
  await fs.mkdir(helperRoot, { recursive: true });
  const helperPath = path.join(helperRoot, windows ? 'tools.ps1' : 'tools.sh');
  await fs.writeFile(helperPath, '# trusted repository helper');
  const options = {
    helperRoot, root: path.join(directory, 'sdk'), archivePath: path.join(directory, filename),
    product, version, platform: { os, rid, architecture },
    archive: { name: `${product}-${rid}`, url: `https://builds.dotnet.microsoft.com/dotnet/${filename}`, sha512: digest(payload, 'sha512') },
    cwd: directory,
    env: {
      SystemRoot: 'C:\\Windows', PATH: 'trusted-system-path',
      HOME: directory, USERPROFILE: directory, TEMP: directory, TMP: directory,
      BASH_ENV: 'untrusted', ENV: 'untrusted', GH_TOKEN: 'secret',
      HTTP_PROXY: 'untrusted', DOTNET_STARTUP_HOOKS: 'untrusted', PSModulePath: 'untrusted',
    },
  };
  const state = { calls: [], requests: [], parameters, help, payload, feed: null, hook: null };
  const script = Buffer.from(windows
    ? `[CmdletBinding()]\nparam(${parameters.map(name => `[string]$${name}`).join(',')})\n`
    : '#!/bin/bash\n# Official script fixture\n');
  const deps = {
    fs,
    http: async (url, transport) => {
      state.requests.push({ url, ...transport });
      if (transport.destination) {
        await fs.writeFile(transport.destination, state.payload);
        return {
          status: 200, sha256: digest(state.payload, 'sha256'), sha512: digest(state.payload, 'sha512'),
          url: 'https://ci.dot.net/public/archive', redirects: ['https://ci.dot.net/public/archive'],
        };
      }
      return { bytes: script, url, redirects: [], status: 200 };
    },
    run: async (executable, args, execution) => {
      state.calls.push({ executable, args, ...execution });
      assert.equal(execution.cwd, directory);
      assert.ok(execution.timeoutMs > 0 && execution.timeoutMs <= 1_200_000);
      for (const key of ['GH_TOKEN', 'BASH_ENV', 'ENV', 'HTTP_PROXY', 'DOTNET_STARTUP_HOOKS'])
        assert.equal(execution.env[key], undefined, key);
      assert.notEqual(execution.env.PSModulePath, 'untrusted');
      const provenance = JSON.parse(await fs.readFile(`${options.archivePath}.provenance.json`, 'utf8'));
      assert.equal(provenance.archive.actualSha512, digest(payload, 'sha512'));
      assert.equal(provenance.archive.redirectUrl, 'https://ci.dot.net/public/archive');
      if (args.some(arg => arg.includes('GetDotNetInstallScript'))) {
        assert.ok(args.some(arg => arg.includes(windows ? '$prepareMachine = $false' : 'prepare_machine=false')));
        assert.ok(args.some(arg => arg.includes(windows ? '$ci = $false' : 'ci=false')));
        assert.ok(!args.some(arg => arg.includes('InitializeDotNetCli')));
        assert.deepEqual(await fs.readFile(execution.env.RB_INSTALLER_PATH), script);
        if (!windows) {
          const stamp = Number(await fs.readFile(path.join(execution.env.RB_INSTALLER_CACHE, '.dotnet-install.timestamp'), 'utf8'));
          assert.ok(Math.abs(stamp - Date.now() / 1000) < 10);
        }
        if (state.mutateScript) await fs.appendFile(execution.env.RB_INSTALLER_PATH, 'changed');
        return {
          exitCode: 0, stderr: '',
          stdout: `helper chatter\nINSTALLER_HELPER_RESULT=${windows ? JSON.stringify({
            path: state.wrongPath || execution.env.RB_INSTALLER_PATH, parameters: state.parameters,
          }) : state.wrongPath || execution.env.RB_INSTALLER_PATH}\n`,
        };
      }
      if (args.includes('--help')) return { exitCode: 0, stdout: state.help, stderr: '' };
      const flag = name => args[args.indexOf(windows ? `-${name}` : `--${name.replace(/[A-Z]/g, (c, i) => `${i ? '-' : ''}${c.toLowerCase()}`)}`) + 1];
      assert.equal(flag('Version'), options.version);
      assert.equal(flag('InstallDir'), options.root);
      assert.equal(flag('Architecture'), options.platform.architecture);
      assert.equal(flag('ZipPath'), options.archivePath);
      assert.ok(args.includes(windows ? '-KeepZip' : '--keep-zip'));
      assert.ok(args.includes(windows ? '-NoPath' : '--no-path'));
      assert.equal(args.includes(windows ? '-SkipNonVersionedFiles' : '--skip-non-versioned-files'), product !== 'sdk');
      assert.equal(args.includes(windows ? '-Runtime' : '--runtime'), product !== 'sdk');
      if (product !== 'sdk') assert.equal(flag('Runtime'), product);
      if (windows) {
        assert.equal(executable, 'C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe');
        assert.ok(args.includes('-NoProfile'));
        assert.equal(execution.env.DOTNET_INSTALL_SKIP_TAR, '1');
      } else {
        assert.equal(executable, '/bin/bash');
        assert.deepEqual(args.slice(0, 2), ['--noprofile', '--norc']);
        assert.equal(flag('Os'), rid.slice(0, -(architecture.length + 1)));
      }
      state.feed = flag('AzureFeed');
      assert.equal(new URL(state.feed).hostname, '127.0.0.1');
      const productDir = product === 'sdk' ? 'Sdk' : product === 'aspnetcore' ? 'aspnetcore/Runtime'
        : product === 'windowsdesktop' && Number(version.split('.')[0]) >= 5 ? 'WindowsDesktop' : 'Runtime';
      const requestedName = filename.replace(rid, windows ? `win-${flag('Architecture')}` : `${flag('Os')}-${flag('Architecture')}`);
      state.archiveUrl = `${state.feed}/${productDir}/${version}/${requestedName}`;
      const productVersion = await request(`${state.feed}/${productDir}/${version}/productVersion.txt`);
      assert.equal(productVersion.status, 200);
      assert.equal(productVersion.bytes.toString().trim(), version);
      assert.equal((await request(state.archiveUrl, 'HEAD')).status, 200);
      assert.deepEqual((await request(state.archiveUrl)).bytes, payload);
      if (state.hook) return state.hook({ execution, args, flag });
      await fs.copyFile(`${options.archivePath}.download`, options.archivePath);
      return { exitCode: 0, stdout: 'installed', stderr: '' };
    },
  };
  return { options, deps, state, script, helperPath };
}

for (const settings of [
  { product: 'sdk' }, { product: 'dotnet' }, { product: 'aspnetcore' }, { product: 'windowsdesktop' },
  { product: 'windowsdesktop', version: '3.1.32' },
  { product: 'sdk', os: 'linux' }, { product: 'dotnet', os: 'linux-musl' },
  { product: 'sdk', os: 'linux', rid: 'linux-musl-x64' },
  { product: 'dotnet', os: 'linux', architecture: 'arm64', rid: 'linux-musl-arm64' },
  { product: 'aspnetcore', os: 'osx' }, { product: 'sdk', architecture: 'arm64' },
]) {
  test(`installs exact ${settings.product} ${settings.rid || settings.os || 'win'} ${settings.architecture || 'x64'} ${settings.version || 'preview'}`, async t => {
    const { options, deps, state, script, helperPath } = await fixture(t, settings);
    const result = await installArchive(options, deps);
    assert.equal(result.archive.sha256, digest(payload, 'sha256'));
    assert.equal(result.archive.actualSha512, options.archive.sha512);
    assert.equal(result.archive.name, options.archive.name);
    assert.equal(result.archive.url, options.archive.url);
    assert.equal(result.archive.redirectUrl, 'https://ci.dot.net/public/archive');
    assert.deepEqual(result.archive.redirects, ['https://ci.dot.net/public/archive']);
    assert.equal(result.installer.sha256, digest(script, 'sha256'));
    assert.equal(result.installer.helperPath, helperPath);
    assert.equal(result.installer.helperSha256, digest(await fs.readFile(helperPath), 'sha256'));
    assert.ok(result.attempts.some(attempt => attempt.url === options.archive.url && attempt.version === options.version && attempt.status === 200));
    assert.equal(state.requests.length, 2);
    assert.ok(state.requests.every(req => req.method === 'GET'));
    assert.ok(state.calls.every(call => call.executable === ((settings.os || 'win') === 'win'
      ? 'C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe' : '/bin/bash')));
    assert.deepEqual(await fs.readFile(options.archivePath), payload);
    await assert.rejects(request(state.archiveUrl), /ECONNREFUSED|ECONNRESET/);
    await assert.rejects(fs.stat(`${options.archivePath}.download`), { code: 'ENOENT' });
  });
}

for (const os of ['win', 'linux']) {
  for (const product of ['sdk', 'dotnet']) {
    test(`missing nonversioned-file protection is ${product === 'sdk' ? 'allowed' : 'rejected'} for ${product} on ${os}`, async t => {
      const { options, deps, state } = await fixture(t, { os, product });
      state.parameters = parameters.filter(name => name !== 'SkipNonVersionedFiles');
      state.help = help.replace('--skip-non-versioned-files', '--skip-non-versioned-files-unsupported');
      if (product === 'sdk') await installArchive(options, deps);
      else {
        await assert.rejects(installArchive(options, deps), /support.*SkipNonVersionedFiles|support.*skip-non-versioned-files/i);
        assert.equal(state.feed, null);
      }
    });
  }
}

test('rejects wrong SHA512 before any process runs', async t => {
  const { options, deps, state } = await fixture(t);
  state.payload = Buffer.from('wrong archive');
  await assert.rejects(installArchive(options, deps), /SHA512/i);
  assert.equal(state.calls.length, 0);
});

test('independently hashes streamed bytes instead of trusting digest claims', async t => {
  const { options, deps, state } = await fixture(t);
  const transport = deps.http;
  deps.http = async (...args) => {
    const response = await transport(...args);
    if (args[1].destination) await fs.writeFile(args[1].destination, 'tampered bytes');
    return response;
  };
  await assert.rejects(installArchive(options, deps), /SHA512|digest/i);
  assert.equal(state.calls.length, 0);
});

for (const setting of ['missing PS parameter', 'missing Unix help flag', 'wrong helper path', 'changed cached script']) {
  test(`fails closed: ${setting}`, async t => {
    const unix = setting.includes('Unix');
    const { options, deps, state } = await fixture(t, { os: unix ? 'linux' : 'win' });
    if (setting.includes('PS parameter')) state.parameters = parameters.filter(p => p !== 'KeepZip');
    if (unix) state.help = help.replace('--keep-zip', '--keep-zip-unsupported');
    if (setting.includes('path')) state.wrongPath = path.join(options.cwd, 'other.ps1');
    if (setting.includes('script')) state.mutateScript = true;
    const expected = setting.includes('path') ? /path/i : setting.includes('script') ? /changed|digest/i : /support.*KeepZip|support.*keep-zip/i;
    await assert.rejects(installArchive(options, deps), expected);
    assert.equal(state.feed, null);
  });
}

test('loopback feed refuses all other versions, paths, queries and methods', async t => {
  const { options, deps, state } = await fixture(t);
  state.hook = async () => {
    for (const url of [
      `${state.feed}/Sdk/latest.version`, `${state.feed}/Sdk/8.0.100/productVersion.txt`,
      `${state.feed}/Sdk/${options.version}/other.zip`, `${state.archiveUrl}?redirect=https://example.com`,
      `${state.feed}/../../secret`, `${state.feed}/Sdk/${options.version}/%2e%2e/secret`,
    ]) assert.equal((await request(url)).status, 404, url);
    assert.equal((await request(state.archiveUrl, 'POST')).status, 405);
    await fs.copyFile(`${options.archivePath}.download`, options.archivePath);
    return { exitCode: 0, stdout: '', stderr: '' };
  };
  await installArchive(options, deps);
});

for (const failure of ['exit', 'throw', 'retained digest', 'missing archive']) {
  test(`closes feed on ${failure} failure`, async t => {
    const { options, deps, state } = await fixture(t);
    state.hook = async () => {
      if (failure === 'exit') return { exitCode: 17, stdout: '', stderr: 'installer failed' };
      if (failure === 'throw') throw new Error('process timeout');
      if (failure === 'retained digest') await fs.writeFile(options.archivePath, 'corrupted retention');
      return { exitCode: 0, stdout: '', stderr: '' };
    };
    await assert.rejects(installArchive(options, deps), /17|timeout|digest|SHA512|ENOENT|archive/i);
    await assert.rejects(request(state.archiveUrl), /ECONNREFUSED|ECONNRESET/);
  });
}

for (const mutation of [
  o => { o.version = 'latest'; }, o => { o.product = 'invalid'; },
  o => { o.archive.sha512 = ''; }, o => { o.platform.architecture = 'x64; bad'; },
  o => { o.platform.rid = 'win-arm64'; }, o => { o.platform.rid = 'linux-x64'; },
  o => { o.env = undefined; }, o => { o.archive.url = 'http://example.com/file.zip'; },
  o => { o.env.SystemRoot = 'relative'; },
]) {
  test(`rejects invalid input: ${mutation}`, async t => {
    const { options, deps, state } = await fixture(t);
    mutation(options);
    await assert.rejects(installArchive(options, deps));
    assert.equal(state.calls.length, 0);
    assert.equal(state.requests.length, 0);
  });
}

test('unavailable archive is explicit, with no fallback or process execution', async t => {
  const { options, deps, state } = await fixture(t);
  deps.http = async url => ({ status: 404, url, redirects: [] });
  await assert.rejects(installArchive(options, deps), error => {
    assert.match(error.message, /404/);
    assert.equal(error.attempts[0].url, options.archive.url);
    return true;
  });
  assert.equal(state.calls.length, 0);
});

test('real repository helper returns seeded script and parses actual PS parameters', { skip: process.platform !== 'win32' }, async t => {
  const { options, deps } = await fixture(t);
  options.helperRoot = path.resolve(__dirname, '..', 'common');
  options.env.SystemRoot = process.env.SystemRoot;
  const simulatedRun = deps.run;
  let helperRan = false;
  deps.run = async (executable, args, execution) => {
    if (!args.some(arg => arg.includes('GetDotNetInstallScript')))
      return simulatedRun(executable, args, execution);
    helperRan = true;
    const { stdout, stderr } = await promisify(execFile)(executable, args, {
      cwd: execution.cwd, env: execution.env, timeout: execution.timeoutMs,
      windowsHide: true, maxBuffer: 1024 * 1024,
    });
    return { stdout, stderr, exitCode: 0 };
  };
  await installArchive(options, deps);
  assert.equal(helperRan, true);
});
