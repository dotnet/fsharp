'use strict';

const assert = require('node:assert/strict');
const { execFile } = require('node:child_process');
const crypto = require('node:crypto');
const fileSystem = require('node:fs');
const fs = require('node:fs/promises');
const http = require('node:http');
const path = require('node:path');
const { test } = require('node:test');
const { promisify } = require('node:util');
const { installArchive, verifyRetainedArchive } = require('./toolchain-installer.cjs');
const { createHttp, publicUrl } = require('./toolchain-io.cjs');
const { policy } = require('./fixtures/contracts.cjs');
const { fingerprintInput } = require('./model.cjs');

const digest = (bytes, algorithm) => crypto.createHash(algorithm).update(bytes).digest('hex');
const parameters = ['Version', 'InstallDir', 'Architecture', 'Runtime', 'NoPath', 'KeepZip', 'ZipPath', 'AzureFeed', 'SkipNonVersionedFiles'];
const help = '--version --install-dir --architecture --runtime --no-path --keep-zip --zip-path --azure-feed --os --skip-non-versioned-files';
const payload = Buffer.from('verified archive bytes');
const approval = {
  sourceSha: '5a8b9adb034f420a5c2c1f577670c5eeeb454b7e', product: 'sdk', version: '11.0.100-rc.1.26420.103',
  rid: 'win-x64', origin: 'https://builds.dotnet.microsoft.com',
  url: 'https://builds.dotnet.microsoft.com/dotnet/Sdk/11.0.100-rc.1.26420.103/dotnet-sdk-11.0.100-rc.1.26420.103-win-x64.zip',
  redirectUrls: [], maxBytes: 1073741824, timeoutMs: 300000,
  approvalRevision: '2ec6de6837b5b367e1f1186e850b8faa2d188b16',
};

for (const oversized of [false, true]) {
  test(`HTTP cleanup failure is visible after ${oversized ? 'bounded rejection' : 'complete download'}`, async t => {
    const response = new Response(payload);
    t.mock.method(response.body, 'cancel', async () => { throw new Error('controlled response cleanup failure'); });
    t.mock.method(globalThis, 'fetch', async () => response);
    await assert.rejects(createHttp(policy())(approval.url, {
      constraints: { allowedUrls: [approval.url], maxBytes: oversized ? 1 : 100, timeoutMs: 1000 },
    }), error => {
      assert.match(error.message, /controlled response cleanup failure/);
      if (oversized) assert.match(error.message, /download exceeds byte limit/);
      return true;
    });
  });
}

function request(url, method = 'GET') {
  return new Promise((resolve, reject) => {
    const req = http.request(url, { method, agent: false }, response => {
      const chunks = [];
      response.on('data', chunk => chunks.push(chunk));
      response.on('error', reject);
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
    product, version, platform: { os, rid, architecture }, policy: policy(), kind: 'source', sourceSha: approval.sourceSha,
    archive: {
      name: `${product}-${rid}`, rid, architecture, url: `https://builds.dotnet.microsoft.com/dotnet/${filename}`,
      sha512: digest(payload, 'sha512'), provenance: { kind: 'publisher-checksum' },
    },
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
          size: state.payload.length, url: state.finalUrl || 'https://ci.dot.net/public/archive',
          redirects: state.redirects || [{ from: url, to: 'https://ci.dot.net/public/archive', status: 302 }],
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
      assert.equal(provenance.archive.redirectUrl, state.finalUrl || 'https://ci.dot.net/public/archive');
      if (args.some(arg => arg.includes('GetDotNetInstallScript'))) {
        if (windows) {
          const keys = Object.keys(execution.env).map(key => key.toUpperCase());
          assert.equal(new Set(keys).size, keys.length, 'Windows supervised environment must not contain case aliases');
        }
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
      const named = windows ? JSON.parse(execution.env.RB_INSTALLER_PARAMETERS) : null;
      const flagName = name => `--${name.replace(/[A-Z]/g, (c, i) => `${i ? '-' : ''}${c.toLowerCase()}`)}`;
      const flag = name => windows ? named[name] : args[args.indexOf(flagName(name)) + 1];
      const hasFlag = name => windows ? named[name] === true : args.includes(flagName(name));
      assert.equal(flag('Version'), options.version);
      assert.equal(flag('InstallDir'), options.root);
      assert.equal(flag('Architecture'), options.platform.architecture);
      assert.equal(flag('ZipPath'), options.archivePath);
      assert.ok(hasFlag('KeepZip'));
      assert.ok(hasFlag('NoPath'));
      assert.equal(hasFlag('SkipNonVersionedFiles'), product !== 'sdk');
      assert.equal(windows ? Object.hasOwn(named, 'Runtime') : args.includes('--runtime'), product !== 'sdk');
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

async function measuredFixture(t) {
  const result = await fixture(t, { version: approval.version });
  const { options, state } = result;
  options.archive.url = approval.url;
  delete options.archive.sha512;
  options.archive.provenance = {
    kind: 'approved-official-https-measured', approvalRevision: approval.approvalRevision,
    approvalDigest: fingerprintInput(approval),
  };
  assert.deepEqual(options.policy.toolchains.measuredArchives[0], approval);
  state.finalUrl = approval.url;
  state.redirects = [];
  return result;
}

for (const conflicting of [false, true]) {
  test(`Windows installer canonicalizes environment aliases: conflicting=${conflicting}`, async t => {
    const { options, deps, state } = await fixture(t);
    options.env.SYSTEMROOT = options.env.SystemRoot;
    delete options.env.SystemRoot;
    options.env.path = conflicting ? 'different-path' : options.env.PATH;
    if (conflicting) {
      await assert.rejects(installArchive(options, deps), /Conflicting Windows environment aliases/);
      assert.equal(state.requests.length, 0);
    } else await installArchive(options, deps);
  });
}

test('Windows installer explicitly loads built-in modules in both PowerShell phases', async t => {
  const { options, deps, state } = await fixture(t);
  await installArchive(options, deps);
  assert.equal(state.calls.length, 2);
  for (const call of state.calls) {
    const commandIndex = call.args.indexOf('-Command');
    assert.notEqual(commandIndex, -1);
    const command = call.args[commandIndex + 1];
    assert.match(command, /\$PSModuleAutoLoadingPreference = 'None'/);
    assert.match(command, /Microsoft\.PowerShell\.Utility/);
    assert.match(command, /Microsoft\.PowerShell\.Management/);
    assert.match(command, /Import-Module.*\$PSHOME/);
  }
});

test('approved exact historical preview measures bytes without claiming a publisher SHA512', async t => {
  const { options, deps, state } = await measuredFixture(t);
  options.sources = [{ url: `${approval.url}.sha512`, status: 404, version: approval.version }];
  const result = await installArchive(options, deps);
  assert.equal(Object.hasOwn(result.archive, 'sha512'), false);
  assert.deepEqual(result.archive.provenance, options.archive.provenance);
  assert.equal(result.archive.actualSha512, digest(payload, 'sha512'));
  assert.equal(result.archive.finalOrigin, approval.origin);
  assert.equal(result.archive.bytesReceived, payload.length);
  assert.equal(new Date(result.archive.receivedAt).toISOString(), result.archive.receivedAt);
  assert.deepEqual(result.archive.retained, {
    path: options.archivePath, sha256: digest(payload, 'sha256'), sha512: digest(payload, 'sha512'), size: payload.length,
  });
  assert.deepEqual(state.requests[0].constraints, {
    allowedUrls: [approval.url], maxBytes: approval.maxBytes, timeoutMs: approval.timeoutMs,
  });
  await verifyRetainedArchive(result.archive);
  assert.deepEqual(result.sources, options.sources);
  assert.notEqual(result.sources, options.sources);
  assert.deepEqual(JSON.parse(await fs.readFile(`${options.archivePath}.provenance.json`, 'utf8')).archive, result.archive);
  assert.deepEqual(JSON.parse(await fs.readFile(`${options.archivePath}.provenance.json`, 'utf8')).sources, options.sources);
});

test('released publisher-checksum installation remains available without measured approval', async t => {
  const { options, deps } = await fixture(t, { version: '10.0.401' });
  options.kind = 'released';
  delete options.sourceSha;
  deps.fs = { ...fs, chmod: undefined };
  const result = await installArchive(options, deps);
  assert.deepEqual(result.sources, []);
  assert.equal(result.archive.sha512, digest(payload, 'sha512'));
  assert.deepEqual(result.archive.provenance, { kind: 'publisher-checksum' });
  await verifyRetainedArchive(result.archive);
});

for (const [name, mutate] of [
  ['missing policy', o => { delete o.policy; }],
  ['missing approval', o => { o.policy.toolchains.measuredArchives = []; }],
  ['changed approval revision', o => { o.policy.toolchains.measuredArchives[0].approvalRevision = '0'.repeat(40); }],
  ['widened approval redirects', o => { o.policy.toolchains.measuredArchives[0].redirectUrls.push('https://ci.dot.net/public/other.zip'); }],
  ['changed source', o => { o.sourceSha = '0'.repeat(40); }],
  ['released mode', o => { o.kind = 'released'; }],
  ['missing mode', o => { delete o.kind; }],
  ['wrong version', o => { o.version = '11.0.100'; }],
  ['wrong product', o => { o.product = 'dotnet'; }],
  ['wrong RID', o => { o.archive.rid = 'win-arm64'; }],
  ['wrong architecture', o => { o.archive.architecture = 'arm64'; }],
  ['missing provenance', o => { delete o.archive.provenance; }],
  ['unapproved provenance kind', o => { o.archive.provenance.kind = 'measured'; }],
  ['wrong approval digest', o => { o.archive.provenance.approvalDigest = '0'.repeat(64); }],
  ['wrong provenance revision', o => { o.archive.provenance.approvalRevision = '0'.repeat(40); }],
  ['measured SHA512 relabeled as publisher', o => { o.archive.sha512 = digest(payload, 'sha512'); }],
  ['different URL on same origin', o => { o.archive.url = o.archive.url.replace('/Sdk/', '/Runtime/'); }],
]) {
  test(`measured request fails closed before network: ${name}`, async t => {
    const { options, deps, state } = await measuredFixture(t);
    mutate(options);
    await assert.rejects(installArchive(options, deps));
    assert.equal(state.requests.length, 0);
    assert.equal(state.calls.length, 0);
  });
}

for (const [name, mutate, expectedError] of [
  ['missing sha256', r => { delete r.sha256; }],
  ['missing sha512', r => { delete r.sha512; }],
  ['wrong sha256', r => { r.sha256 = '0'.repeat(64); }, {
    code: 'checksum-provenance', message: /HTTP sha256 digest/,
  }],
  ['wrong sha512', r => { r.sha512 = '0'.repeat(128); }],
  ['missing size', r => { delete r.size; }],
  ['truncated bytes', r => { r.size++; }],
  ['oversized bytes', r => { r.size = approval.maxBytes + 1; }],
  ['partial status', r => { r.status = 206; }],
  ['missing redirects', r => { delete r.redirects; }],
  ['omitted hops', r => { r.url = 'https://ci.dot.net/public/other.zip'; }],
  ['wrong hop source', r => { r.redirects = [{ from: `${approval.url}.other`, to: approval.url, status: 302 }]; }],
  ['invalid hop status', r => { r.redirects = [{ from: approval.url, to: approval.url, status: 200 }]; }],
  ['unapproved intermediate hop', r => {
    const other = 'https://ci.dot.net/public/other.zip';
    r.redirects = [{ from: approval.url, to: other, status: 302 }, { from: other, to: approval.url, status: 302 }];
  }],
  ['downgrade', r => {
    r.url = approval.url.replace('https:', 'http:');
    r.redirects = [{ from: approval.url, to: r.url, status: 302 }];
  }],
]) {
  test(`measured adapter independently rejects HTTP claims: ${name}`, async t => {
    const { options, deps, state } = await measuredFixture(t);
    const transport = deps.http;
    deps.http = async (...args) => {
      const response = await transport(...args);
      if (args[1].destination) mutate(response);
      return response;
    };
    await assert.rejects(installArchive(options, deps), expectedError);
    assert.equal(state.calls.length, 0);
  });
}

for (const phase of ['download', 'helper', 'delivery', 'installer exit', 'retention protection']) {
  test(`measured archive mutation fails closed during ${phase}`, async t => {
    const { options, deps, state } = await measuredFixture(t);
    const changed = Buffer.from(payload);
    changed[0] ^= 1;
    if (phase === 'download') state.payload = Buffer.alloc(0);
    else if (phase === 'delivery') {
      let reads = 0;
      deps.fs = {
        ...fs,
        async open(filename, ...args) {
          if (filename === `${options.archivePath}.download` && ++reads === 3)
            await fs.writeFile(filename, changed);
          return fs.open(filename, ...args);
        },
      };
    } else if (phase === 'retention protection') {
      deps.fs = { ...fs, chmod: filename => fs.writeFile(filename, changed) };
    }
    const run = deps.run;
    deps.run = async (exe, args, execution) => {
      const installer = execution.env.RB_INSTALLER_PARAMETERS && JSON.parse(execution.env.RB_INSTALLER_PARAMETERS);
      if (phase === 'delivery' && installer) {
        const url = `${installer.AzureFeed}/Sdk/${options.version}/${path.basename(options.archivePath)}`;
        await request(url).catch(() => {});
        await fs.writeFile(`${options.archivePath}.download`, payload);
        await fs.writeFile(options.archivePath, payload);
        return { exitCode: 0 };
      }
      const result = await run(exe, args, execution);
      if ((phase === 'helper' && !installer) || (phase === 'installer exit' && installer))
        await fs.writeFile(`${options.archivePath}.download`, changed);
      return result;
    };
    await assert.rejects(installArchive(options, deps),
      phase === 'delivery' ? /changed during installer delivery/ : /changed|digest|empty|delivery/i);
    await assert.rejects(fs.stat(`${options.archivePath}.download`), { code: 'ENOENT' });
  });
}

for (const mutation of ['bytes', 'missing', 'receipt sha256', 'receipt sha512', 'receipt size', 'relative path', 'nonlink', 'directory']) {
  test(`retained archive reuse rejects ${mutation}`, async t => {
    const { options, deps } = await measuredFixture(t);
    const { archive } = await installArchive(options, deps);
    let verifyFs = fs;
    if (mutation === 'bytes') {
      await fs.chmod(options.archivePath, 0o666);
      await fs.writeFile(options.archivePath, Buffer.alloc(payload.length));
    } else if (mutation === 'missing') await fs.rm(options.archivePath, { force: true });
    else if (mutation === 'receipt sha256') archive.sha256 = '0'.repeat(64);
    else if (mutation === 'receipt sha512') archive.actualSha512 = '0'.repeat(128);
    else if (mutation === 'receipt size') archive.bytesReceived++;
    else if (mutation === 'relative path') archive.retained.path = path.basename(options.archivePath);
    else if (mutation === 'directory') archive.retained.path = options.cwd;
    else verifyFs = { ...fs, lstat: async filename => ({ ...await fs.lstat(filename), isFile: () => true, isSymbolicLink: () => true }) };
    await assert.rejects(verifyRetainedArchive(archive, { fs: verifyFs }), /archive|ENOENT|regular/i);
  });
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
    assert.deepEqual(result.archive.redirects, [{ from: options.archive.url, to: 'https://ci.dot.net/public/archive', status: 302 }]);
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

test('archive delivery completes before a slow EOF read after its advertised length', async t => {
  const { options, deps } = await fixture(t);
  let reads = 0;
  deps.fs = {
    ...fs,
    async open(filename, ...args) {
      const file = await fs.open(filename, ...args);
      if (filename !== `${options.archivePath}.download` || ++reads !== 3) return file;
      return {
        createReadStream(settings) {
          return fileSystem.createReadStream(filename, {
            ...settings, fd: file.fd,
            fs: {
              close(_fd, callback) { file.close().then(() => callback(null), callback); },
              read(fd, buffer, offset, length, position, callback) {
                fileSystem.read(fd, buffer, offset, length, position, (error, bytesRead, data) => {
                  if (!error && bytesRead === 0) setTimeout(callback, 100, error, bytesRead, data);
                  else callback(error, bytesRead, data);
                });
              },
            },
          });
        },
        close: () => file.close(),
      };
    },
  };
  const result = await installArchive(options, deps);
  assert.equal(result.archive.actualSha512, options.archive.sha512);
});

for (const os of ['win', 'linux']) {
  for (const completion of ['success', 'stream error', 'close error', 'installer error']) {
    test(`awaits archive delivery after installer exit: ${os}, ${completion}`, async t => {
      const { options, deps, state } = await fixture(t, { os });
      let reads = 0;
      let releaseDelivery;
      let deliveryFinished = false;
      let signalDeliveryReady;
      const deliveryReady = new Promise(resolve => { signalDeliveryReady = resolve; });
      const deliveryAllowed = new Promise(resolve => { releaseDelivery = resolve; });
      let received;
      deps.fs = {
        ...fs,
        async open(filename, ...args) {
          if (filename === options.archivePath) assert.equal(deliveryFinished, true);
          const file = await fs.open(filename, ...args);
          if (filename !== `${options.archivePath}.download` || ++reads !== 3) return file;
          signalDeliveryReady();
          await deliveryAllowed;
          return {
            createReadStream(settings) {
              if (completion === 'stream error') throw new Error('controlled stream failure');
              return file.createReadStream(settings);
            },
            async close() {
              await file.close();
              deliveryFinished = true;
              if (completion === 'close error') throw new Error('controlled close failure');
            },
          };
        },
      };
      const run = deps.run;
      deps.run = async (executable, args, execution) => {
        const installer = execution.env.RB_INSTALLER_PARAMETERS && JSON.parse(execution.env.RB_INSTALLER_PARAMETERS);
        const feedIndex = args.indexOf('--azure-feed');
        if (!installer && feedIndex < 0) return run(executable, args, execution);
        const feed = os === 'win' ? installer.AzureFeed : args[feedIndex + 1];
        state.archiveUrl = `${feed}/Sdk/${options.version}/${path.basename(options.archivePath)}`;
        received = request(state.archiveUrl).then(value => value, error => error);
        await deliveryReady;
        await fs.copyFile(`${options.archivePath}.download`, options.archivePath);
        assert.equal(deliveryFinished, false);
        // Force installer completion before the server's pending delivery settles.
        setImmediate(releaseDelivery);
        return { exitCode: completion === 'installer error' ? 17 : 0, stdout: '', stderr: '' };
      };
      if (completion === 'success') {
        const result = await installArchive(options, deps);
        assert.equal(result.archive.actualSha512, options.archive.sha512);
        assert.deepEqual(await fs.readFile(options.archivePath), payload);
      } else {
        await assert.rejects(installArchive(options, deps),
          completion === 'stream error' ? /Loopback archive delivery failed: controlled stream failure/
            : completion === 'close error' ? /Loopback archive delivery failed: controlled close failure/
              : /Official installer process exited 17/);
      }
      assert.equal(deliveryFinished, true);
      if (completion === 'stream error' || completion === 'installer error')
        assert.match((await received).message, /ECONNRESET|socket hang up/);
      else assert.deepEqual((await received).bytes, payload);
      await assert.rejects(request(state.archiveUrl), /ECONNREFUSED|ECONNRESET/);
      await assert.rejects(fs.stat(`${options.archivePath}.download`), { code: 'ENOENT' });
    });
  }

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

for (const method of [null, 'HEAD']) {
  test(`rejects installer success without archive GET: ${method || 'no request'}`, async t => {
    const { options, deps, state } = await fixture(t);
    const run = deps.run;
    deps.run = async (executable, args, execution) => {
      const installer = execution.env.RB_INSTALLER_PARAMETERS && JSON.parse(execution.env.RB_INSTALLER_PARAMETERS);
      if (!installer) return run(executable, args, execution);
      state.archiveUrl = `${installer.AzureFeed}/Sdk/${options.version}/${path.basename(options.archivePath)}`;
      if (method) assert.equal((await request(state.archiveUrl, method)).status, 200);
      await fs.copyFile(`${options.archivePath}.download`, options.archivePath);
      return { exitCode: 0, stdout: 'already installed', stderr: '' };
    };
    await assert.rejects(installArchive(options, deps), /Official installer did not download the verified archive/);
    await assert.rejects(request(state.archiveUrl), /ECONNREFUSED|ECONNRESET/);
  });
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
  o => { o.env.SystemRoot = 'relative'; }, o => { delete o.kind; }, o => { delete o.sourceSha; },
  o => { delete o.archive.provenance; }, o => { delete o.policy; },
]) {
  test(`rejects invalid input: ${mutation}`, async t => {
    const { options, deps, state } = await fixture(t);
    mutation(options);
    await assert.rejects(installArchive(options, deps));
    assert.equal(state.calls.length, 0);
    assert.equal(state.requests.length, 0);
  });
}

for (const status of [404, 429, 500, 503]) {
  test(`HTTP ${status} acquisition fails explicitly, with no fallback or process execution`, async t => {
    const { options, deps, state } = await fixture(t);
    deps.http = async url => ({ status, url, redirects: [] });
    await assert.rejects(installArchive(options, deps), error => {
      assert.match(error.message, new RegExp(String(status)));
      assert.equal(error.code, status === 404 ? 'archive-unavailable' : 'network-failure');
      assert.equal(error.attempts[0].url, options.archive.url);
      assert.equal(error.attempts.length, 1);
      return true;
    });
    assert.equal(state.calls.length, 0);
  });
}

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
    const isolatedArgs = [...args];
    const commandIndex = isolatedArgs.indexOf('-Command') + 1;
    isolatedArgs[commandIndex] = "$PSModuleAutoLoadingPreference = 'None'\n" + isolatedArgs[commandIndex];
    const { stdout, stderr } = await promisify(execFile)(executable, isolatedArgs, {
      cwd: execution.cwd, env: execution.env, timeout: execution.timeoutMs,
      windowsHide: true, maxBuffer: 1024 * 1024,
    });
    return { stdout, stderr, exitCode: 0 };
  };
  await installArchive(options, deps);
  assert.equal(helperRan, true);
});

for (const product of ['sdk', 'dotnet']) {
  test(`real Windows ${product} installer binds literal named parameters without module discovery`,
    { skip: process.platform !== 'win32' }, async t => {
      const { options, deps } = await fixture(t, { product });
      options.helperRoot = path.resolve(__dirname, '..', 'common');
      options.env.SystemRoot = process.env.SystemRoot;
      options.root = path.join(options.cwd, "SDK 'quoted'; $(throw 'not code')");
      const originalHttp = deps.http;
      const script = Buffer.from(`
[CmdletBinding()]
param(${parameters.map(name =>
  `[${['NoPath', 'KeepZip', 'SkipNonVersionedFiles'].includes(name) ? 'switch' : 'string'}]$${name}`).join(',')})
if ($PSModuleAutoLoadingPreference -ne 'None') { throw 'Automatic module discovery is enabled' }
if ($env:GH_TOKEN -or $env:HTTP_PROXY -or $env:DOTNET_STARTUP_HOOKS) { throw 'Caller environment leaked' }
$bound = @{}
foreach ($name in $PSBoundParameters.Keys) {
  $value = $PSBoundParameters[$name]
  $bound[$name] = if ($value -is [System.Management.Automation.SwitchParameter]) { $value.IsPresent } else { $value }
}
$productDirectory = if ($Runtime) { 'Runtime' } else { 'Sdk' }
$archiveName = [IO.Path]::GetFileName($ZipPath)
$client = [Net.WebClient]::new()
try { $client.DownloadFile("$AzureFeed/$productDirectory/$Version/$archiveName", $ZipPath) }
finally { $client.Dispose() }
Write-Output ('NATIVE_INSTALLER_RESULT=' + ($bound | ConvertTo-Json -Compress))
exit 0
`);
      deps.http = async (url, transport) => {
        const response = await originalHttp(url, transport);
        return transport.destination ? response : { ...response, bytes: script };
      };
      let bound;
      deps.run = async (executable, args, execution) => {
        const { stdout, stderr } = await promisify(execFile)(executable, args, {
          cwd: execution.cwd, env: execution.env, timeout: execution.timeoutMs,
          windowsHide: true, maxBuffer: 1024 * 1024,
        });
        const result = stdout.split(/\r?\n/).find(line => line.startsWith('NATIVE_INSTALLER_RESULT='));
        if (result) {
          assert.equal(bound, undefined);
          bound = JSON.parse(result.slice('NATIVE_INSTALLER_RESULT='.length));
          assert.deepEqual(bound, JSON.parse(execution.env.RB_INSTALLER_PARAMETERS));
        }
        return { stdout, stderr, exitCode: 0 };
      };
      const result = await installArchive(options, deps);
      assert.equal(bound.Version, options.version);
      assert.equal(bound.InstallDir, options.root);
      assert.equal(bound.Architecture, 'x64');
      assert.equal(bound.NoPath, true);
      assert.equal(bound.KeepZip, true);
      assert.equal(bound.ZipPath, options.archivePath);
      assert.equal(Object.hasOwn(bound, 'Runtime'), product !== 'sdk');
      assert.equal(Object.hasOwn(bound, 'SkipNonVersionedFiles'), product !== 'sdk');
      assert.equal(result.archive.retained.sha256, digest(payload, 'sha256'));
    });
}

for (const url of [
  approval.url.replace('https:', 'http:'), approval.url.replace('/Sdk/', '/Runtime/'),
  approval.url.replace('/Sdk/', '/dotnet/../Sdk/'), approval.url.replace('/Sdk/', '/%53dk/'),
  approval.url.replace('https://', 'https://user:pass@'), `${approval.url}?query=1`, `${approval.url}#fragment`,
  approval.url.replace('https://builds.', 'https://BUILDS.'), approval.url.replace('/Sdk/', '\\Sdk\\'),
]) {
  test(`constrained HTTP rejects nonexact archive URL before network: ${url}`, async t => {
    const fetch = t.mock.method(globalThis, 'fetch', () => assert.fail('must reject before network'));
    await assert.rejects(createHttp(policy())(url, {
      constraints: { allowedUrls: [approval.url], maxBytes: 100, timeoutMs: 1000 },
    }), { code: 'checksum-provenance', message: /unapproved|exact approved/ });
    assert.equal(fetch.mock.callCount(), 0);
  });
}

for (const redirect of [
  'http://builds.dotnet.microsoft.com/dotnet/other.zip', 'https://ci.dot.net/public/other.zip',
  `${approval.url}?query=1`, `${approval.url}#fragment`, approval.url.replace('/Sdk/', '/%53dk/'),
  approval.url.replace('/Sdk/', '/nested/../Sdk/'),
]) {
  test(`constrained HTTP rejects redirect before network: ${redirect}`, async t => {
    const fetch = t.mock.method(globalThis, 'fetch', async () => new Response(null, { status: 302, headers: { location: redirect } }));
    await assert.rejects(createHttp(policy())(approval.url, {
      constraints: { allowedUrls: [approval.url], maxBytes: 100, timeoutMs: 1000 },
    }), { code: 'checksum-provenance', message: /unapproved|exact approved/ });
    assert.equal(fetch.mock.callCount(), 1);
  });
}

test('constrained HTTP follows only exact allowed hops with one total deadline and streamed digests', async t => {
  const directory = await fs.mkdtemp(path.join(__dirname, '.installer-http-'));
  t.after(() => fs.rm(directory, { recursive: true, force: true }));
  const redirect = 'https://ci.dot.net/public/approved.zip';
  const signals = [];
  const fetch = t.mock.method(globalThis, 'fetch', async (url, options) => {
    signals.push(options.signal);
    assert.equal(options.redirect, 'manual');
    return url === approval.url ? new Response(null, { status: 307, headers: { location: redirect } })
      : new Response(payload, { headers: { 'content-length': String(payload.length) } });
  });
  const destination = path.join(directory, 'archive.zip');
  const result = await createHttp(policy())(approval.url, {
    destination, constraints: { allowedUrls: [approval.url, redirect], maxBytes: payload.length, timeoutMs: 1000 },
  });
  assert.equal(fetch.mock.callCount(), 2);
  assert.equal(signals[0], signals[1]);
  assert.deepEqual(result.redirects, [{ from: approval.url, to: redirect, status: 307 }]);
  assert.deepEqual(await fs.readFile(destination), payload);
  assert.equal(result.bytes, undefined);
  assert.equal(result.size, payload.length);
  assert.equal(result.sha256, digest(payload, 'sha256'));
  assert.equal(result.sha512, digest(payload, 'sha512'));
});

for (const [name, headers, maxBytes] of [
  ['advertised oversize', { 'content-length': '100' }, 4],
  ['streamed oversize', {}, 4],
  ['truncated body', { 'content-length': String(payload.length + 1) }, 100],
  ['overlong body', { 'content-length': '1' }, 100],
  ['invalid length', { 'content-length': 'invalid' }, 100],
  ['infinite limit', {}, Infinity],
  ['zero limit', {}, 0],
]) {
  test(`constrained HTTP rejects ${name}`, async t => {
    t.mock.method(globalThis, 'fetch', async () => new Response(payload, { headers }));
    await assert.rejects(createHttp(policy())(approval.url, {
      constraints: { allowedUrls: [approval.url], maxBytes, timeoutMs: 1000 },
    }), /limit|Content-Length|constraints/);
  });
}

test('constrained HTTP timeout aborts the download rather than resetting on a redirect', async t => {
  const keepAlive = setTimeout(() => {}, 1000);
  t.after(() => clearTimeout(keepAlive));
  t.mock.method(globalThis, 'fetch', (_url, { signal }) => new Promise((_, reject) => {
    signal.addEventListener('abort', () => reject(signal.reason), { once: true });
  }));
  await assert.rejects(createHttp(policy())(approval.url, {
    constraints: { allowedUrls: [approval.url], maxBytes: 100, timeoutMs: 10 },
  }), { name: 'TimeoutError', code: 'network-failure' });
});

for (const phase of ['request', 'body']) {
  test(`HTTP native ${phase} failure retains network classification and cause`, async t => {
    const cause = new TypeError('fetch failed');
    t.mock.method(globalThis, 'fetch', async () => {
      if (phase === 'request') throw cause;
      return new Response(new ReadableStream({ start(controller) { controller.error(cause); } }));
    });
    await assert.rejects(createHttp(policy())(approval.url), error =>
      error.code === 'network-failure' && error.cause === cause && error.name === 'TypeError');
    await assert.rejects(createHttp(policy())(approval.url.replace('https:', 'http:')), error =>
      error.code !== 'network-failure' && /unapproved/.test(error.message));
  });
}

test('HTTP availability HEAD does not consume a body or allocate an archive file', async t => {
  const body = new Response(payload, { headers: { 'content-length': String(approval.maxBytes + 1) } });
  const cancel = t.mock.method(body.body, 'cancel');
  t.mock.method(globalThis, 'fetch', async (_url, options) => {
    assert.equal(options.method, 'HEAD');
    return body;
  });
  const result = await createHttp(policy(), { open: () => assert.fail('HEAD must not allocate a file') })(approval.url, {
    destination: 'unused.zip', method: 'HEAD',
  });
  assert.equal(cancel.mock.callCount(), 1);
  assert.equal(result.size, 0);
  assert.equal(result.bytes.length, 0);
});

test('public URL canonicalization rejects escaped traversal rather than normalizing it', () => {
  for (const url of [approval.url.replace('/Sdk/', '/x/../Sdk/'), approval.url.replace('/Sdk/', '/%53dk/')])
    assert.throws(() => publicUrl(url, policy()), /unapproved/);
});
