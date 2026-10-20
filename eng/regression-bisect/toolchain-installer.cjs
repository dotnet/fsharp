'use strict';

const crypto = require('node:crypto');
const http = require('node:http');
const path = require('node:path');
const { Transform } = require('node:stream');
const { pipeline } = require('node:stream/promises');
const { validateArchiveRequest, validateArchiveReceipt } = require('./archive-provenance.cjs');
const { publicUrl, validateHttpResponse } = require('./toolchain-io.cjs');

const marker = 'INSTALLER_HELPER_RESULT=';
const bootstrapPS = `
$ErrorActionPreference = 'Stop'
$PSModuleAutoLoadingPreference = 'None'
foreach ($module in @('Microsoft.PowerShell.Utility', 'Microsoft.PowerShell.Management')) {
  Import-Module ([IO.Path]::Combine($PSHOME, 'Modules', $module, "$module.psd1")) -ErrorAction Stop
}
`;
const helperPS = `${bootstrapPS}
$prepareMachine = $false
$ci = $false
$restore = $false
$runtimeSourceFeed = ''
$runtimeSourceFeedKey = ''
$useInstalledDotNetCli = $false
$disableConfigureToolsetImport = $true
$dotnetInstallScriptVersion = 'v1'
. $env:RB_HELPER_PATH
$scriptPath = GetDotNetInstallScript $env:RB_INSTALLER_CACHE
$tokens = $null
$parseErrors = $null
$ast = [System.Management.Automation.Language.Parser]::ParseFile($scriptPath, [ref]$tokens, [ref]$parseErrors)
if ($parseErrors.Count -ne 0 -or $null -eq $ast.ParamBlock) { throw 'Unsupported installer parameter block' }
$names = @($ast.ParamBlock.Parameters | ForEach-Object { $_.Name.VariablePath.UserPath })
Write-Output ('INSTALLER_HELPER_RESULT=' + (@{ path = [string]$scriptPath; parameters = $names } | ConvertTo-Json -Compress))
`;
const installerPS = `${bootstrapPS}
$parameters = @{}
($env:RB_INSTALLER_PARAMETERS | ConvertFrom-Json).PSObject.Properties | ForEach-Object {
  $parameters[$_.Name] = $_.Value
}
& $env:RB_INSTALLER_PATH @parameters
exit $LASTEXITCODE
`;
const helperSH = `
set -euo pipefail
prepare_machine=false
ci=false
restore=false
runtime_source_feed=''
runtime_source_feed_key=''
use_installed_dotnet_cli=false
disable_configure_toolset_import=true
dotnetInstallScriptVersion=v1
. "$RB_HELPER_PATH"
GetDotNetInstallScript "$RB_INSTALLER_CACHE"
printf 'INSTALLER_HELPER_RESULT=%s\\n' "$_GetDotNetInstallScript"
`;

const hash = (bytes, algorithm) => crypto.createHash(algorithm).update(bytes).digest('hex');

async function hashFile(fs, filename, maxBytes = 1073741824, signal) {
  signal?.throwIfAborted();
  const before = await fs.lstat(filename);
  if (!before.isFile() || before.isSymbolicLink() || before.size > maxBytes)
    throw new Error('Archive must be a bounded regular nonlink file');
  const sha256 = crypto.createHash('sha256');
  const sha512 = crypto.createHash('sha512');
  let size = 0;
  const file = await fs.open(filename, 'r');
  try {
    for await (const chunk of file.createReadStream({ autoClose: false, signal, highWaterMark: 1024 * 1024 })) {
      signal?.throwIfAborted();
      size += chunk.length;
      if (size > maxBytes) throw new Error('Archive exceeds byte limit');
      sha256.update(chunk);
      sha512.update(chunk);
    }
    const after = await fs.lstat(filename);
    if (!after.isFile() || after.isSymbolicLink() || size !== before.size ||
        ['size', 'ino', 'mtimeMs'].some(key => before[key] !== after[key]))
      throw new Error('Archive changed while hashing bytes');
    return { sha256: sha256.digest('hex'), sha512: sha512.digest('hex'), size };
  } finally {
    await file.close();
  }
}

async function verifyRetainedArchive(archive, deps = {}) {
  const fs = deps.fs || require('node:fs/promises');
  const retained = archive?.retained;
  if (!retained || typeof retained.path !== 'string' || !path.isAbsolute(retained.path) || /[\r\n\0]/.test(retained.path) ||
      !/^[0-9a-f]{64}$/.test(retained.sha256) || !/^[0-9a-f]{128}$/.test(retained.sha512) ||
      !Number.isSafeInteger(retained.size) || retained.size <= 0 ||
      archive.sha256 !== retained.sha256 || archive.actualSha512 !== retained.sha512 || archive.bytesReceived !== retained.size ||
      (Object.hasOwn(archive, 'sha512') && archive.sha512.toLowerCase() !== retained.sha512))
    throw new Error('Retained archive receipt fields do not match');
  if (await fs.realpath(retained.path) !== path.resolve(retained.path))
    throw new Error('Retained archive path must not contain links');
  const actual = await hashFile(fs, retained.path, retained.size);
  if (actual.sha256 !== retained.sha256 || actual.sha512 !== retained.sha512 || actual.size !== retained.size)
    throw new Error('Retained archive digest or size does not match verified download');
  return actual;
}

function isolatedEnvironment(env, windows) {
  if (!env || typeof env !== 'object') throw new Error('An isolated environment is required');
  const allowed = new Set([
    'SYSTEMROOT', 'WINDIR', 'PATH', 'HOME', 'USERPROFILE', 'LOCALAPPDATA', 'APPDATA',
    'TEMP', 'TMP', 'TMPDIR', 'DOTNET_CLI_HOME', 'NUGET_PACKAGES', 'LANG', 'LC_ALL',
  ]);
  const result = {};
  for (const [key, value] of Object.entries(env)) {
    if (!allowed.has(key.toUpperCase()) || typeof value !== 'string') continue;
    const name = windows ? (key.toUpperCase() === 'SYSTEMROOT' ? 'SystemRoot' : key.toUpperCase()) : key;
    if (Object.hasOwn(result, name) && result[name] !== value) throw new Error('Conflicting Windows environment aliases');
    result[name] = value;
  }
  if (windows) {
    const systemRoot = Object.entries(result).find(([key]) => key.toUpperCase() === 'SYSTEMROOT')?.[1];
    if (!systemRoot || !path.win32.isAbsolute(systemRoot)) throw new Error('Absolute env.SystemRoot is required on Windows');
    result.SystemRoot = systemRoot;
    result.PSModulePath = path.win32.join(systemRoot, 'System32', 'WindowsPowerShell', 'v1.0', 'Modules');
  }
  result.NO_PROXY = '127.0.0.1,localhost';
  if (!windows) result.no_proxy = result.NO_PROXY;
  result.DOTNET_CLI_TELEMETRY_OPTOUT = '1';
  result.DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1';
  result.DOTNET_MULTILEVEL_LOOKUP = '0';
  return result;
}

// This is a transport for already verified bytes, not an extractor or a public-feed
// fallback implementation. Only the official installer unpacks the archive. The
// caller's HTTP boundary validates every public hop and streams the original GET.
async function startFeed(fs, download, archiveName, productDirectory, version, product, digests) {
  const prefix = `/${crypto.randomBytes(24).toString('hex')}`;
  const directory = `${prefix}/${productDirectory}/${version}/`;
  const archiveRoute = directory + archiveName;
  const versionRoutes = new Set([
    `${directory}productVersion.txt`,
    `${directory}${product === 'sdk' ? 'sdk' : product === 'dotnet' ? 'runtime' : product}-productVersion.txt`,
  ]);
  const size = digests.size;
  let requests = 0;
  let active = 0;
  let downloads = 0;
  let error;
  const deliveries = [];
  const server = http.createServer((req, res) => {
    res.setHeader('Connection', 'close');
    if (++requests > 64 || active >= 4) { res.writeHead(429).end(); return; }
    if (!['GET', 'HEAD'].includes(req.method)) { res.writeHead(405).end(); return; }
    if (req.url !== archiveRoute && !versionRoutes.has(req.url)) { res.writeHead(404).end(); return; }
    active++;
    deliveries.push(deliver(req, res));
  });
  async function deliver(req, res) {
    try {
      if (versionRoutes.has(req.url)) {
        res.setHeader('Content-Type', 'text/plain');
        res.end(req.method === 'HEAD' ? undefined : `${version}\n`);
      } else {
        res.setHeader('Content-Type', 'application/octet-stream');
        res.setHeader('Content-Length', size);
        if (req.method === 'HEAD') res.end();
        else {
          const file = await fs.open(download, 'r');
          try {
            const sha256 = crypto.createHash('sha256');
            const sha512 = crypto.createHash('sha512');
            let delivered = 0;
            const verify = new Transform({
              transform(chunk, _encoding, callback) {
                delivered += chunk.length;
                sha256.update(chunk); sha512.update(chunk);
                callback(null, chunk);
              },
              flush(callback) {
                callback(delivered === size && sha256.digest('hex') === digests.sha256 && sha512.digest('hex') === digests.sha512
                  ? null : new Error('Archive bytes changed during installer delivery'));
              },
            });
            await pipeline(file.createReadStream({ autoClose: false, end: size - 1 }), verify, res);
            downloads++;
          } finally {
            await file.close();
          }
        }
      }
    } catch (cause) {
      error = cause;
      res.destroy();
    } finally {
      active--;
    }
  }
  server.requestTimeout = server.headersTimeout = 30_000;
  server.timeout = 60_000;
  server.maxHeadersCount = 32;
  await new Promise((resolve, reject) => {
    server.once('error', reject);
    server.listen(0, '127.0.0.1', resolve);
  });
  return {
    url: `http://127.0.0.1:${server.address().port}${prefix}`,
    async verify() {
      // Installer exit and client receipt can precede server-side stream completion.
      await Promise.all(deliveries);
      if (error) throw new Error(`Loopback archive delivery failed: ${error.message}`, { cause: error });
      if (!downloads) throw new Error('Official installer did not download the verified archive');
    },
    async close() {
      await new Promise((resolve, reject) => {
        server.close(cause => cause ? reject(cause) : resolve());
        server.closeAllConnections();
      });
      await Promise.all(deliveries);
    },
  };
}

/**
 * helperRoot contains Arcade tools.ps1/tools.sh; root is the isolated install directory.
 * fs is node:fs/promises-compatible. http performs policy-checked public GETs (streaming
 * to destination when supplied), and run is the caller's bounded, trusted process
 * boundary. No process.env is inherited. The acquisition receipt is persisted at
 * archivePath + '.provenance.json' before any acquired script executes.
 * policy/kind/sourceSha bind measured archives to an exact approval; archive.sha512
 * is reserved for publisher checksums. Both modes retain independently hashed bytes.
 */
async function installArchive(options, deps) {
  const { helperRoot, root, archivePath, product, version, platform, archive, cwd, policy, kind, sourceSha } = options;
  const fs = deps.fs || require('node:fs/promises');
  if (typeof deps.http !== 'function' || typeof deps.run !== 'function')
    throw new Error('Policy-constrained http and trusted run boundaries are required');
  const os = platform?.os === 'linux' && platform.rid === `linux-musl-${platform.architecture}` ? 'linux-musl'
    : ({ win32: 'win', windows: 'win', darwin: 'osx', macos: 'osx' })[platform?.os] || platform?.os;
  const windows = os === 'win';
  if (!['win', 'linux', 'linux-musl', 'osx', 'freebsd', 'rhel.6'].includes(os))
    throw new Error('Unsupported installer OS');
  if (!['sdk', 'dotnet', 'aspnetcore', 'windowsdesktop'].includes(product) || (product === 'windowsdesktop' && !windows))
    throw new Error('Unsupported installer product');
  if (!/^\d+\.\d+\.\d+(?:-[0-9A-Za-z.-]+)?$/.test(version))
    throw new Error('An exact SDK/runtime version is required, not a channel or latest');
  if (!['x64', 'x86', 'arm', 'arm64', 's390x', 'ppc64le', 'loongarch64', 'riscv64'].includes(platform.architecture))
    throw new Error('Unsupported installer architecture');
  if (platform.rid !== `${os}-${platform.architecture}` || archive?.rid !== platform.rid || archive.architecture !== platform.architecture)
    throw new Error('Installer archive OS/RID/architecture mismatch');
  if (!['source', 'released'].includes(kind) || (kind === 'source' && !/^[0-9a-f]{40}$/.test(sourceSha)))
    throw new Error('Explicit installer kind and source revision are required');
  const approval = validateArchiveRequest({ policy, kind, sourceSha, product, version, platform, archive });
  const constraints = approval && {
    allowedUrls: [approval.url, ...approval.redirectUrls], maxBytes: approval.maxBytes, timeoutMs: approval.timeoutMs,
  };
  const archiveName = new URL(publicUrl(archive.url, policy)).pathname.split('/').pop();
  if (!/^[A-Za-z0-9][A-Za-z0-9_.-]*\.(zip|tar\.gz)$/.test(archiveName) || (!windows && !archiveName.endsWith('.tar.gz')))
    throw new Error('Unsupported official archive filename');
  for (const [name, value] of Object.entries({ helperRoot, root, archivePath, cwd }))
    if (typeof value !== 'string' || !path.isAbsolute(value) || /[\r\n\0]/.test(value))
      throw new Error(`Absolute ${name} is required`);
  const env = isolatedEnvironment(options.env, windows);
  if (windows) env.DOTNET_INSTALL_SKIP_TAR = archiveName.endsWith('.zip') ? '1' : '0';
  // Archive owner IDs are not valid authority in the container without FOWNER.
  if (os === 'linux' || os === 'linux-musl') env.TAR_OPTIONS = '--no-same-owner';
  const extension = windows ? 'ps1' : 'sh';
  const installerUrl = `https://builds.dotnet.microsoft.com/dotnet/scripts/v1/dotnet-install.${extension}`;
  const helperPath = path.join(helperRoot, `tools.${extension}`);
  const executable = windows
    ? path.win32.join(env.SystemRoot, 'System32', 'WindowsPowerShell', 'v1.0', 'powershell.exe')
    : '/bin/bash';
  const shellArgs = windows ? ['-NoLogo', '-NoProfile', '-NonInteractive', '-ExecutionPolicy', 'Bypass'] : ['--noprofile', '--norc'];
  const attempts = [];
  const logs = [];
  const download = `${archivePath}.download`;
  let feed;
  let receipt;
  let stage = 'checksum-provenance';
  async function get(url, destination, constraints) {
    const attempt = { url, version, status: 'pending' };
    attempts.push(attempt);
    try {
      const response = await deps.http(url, { method: 'GET', ...(destination ? { destination } : {}), ...(constraints ? { constraints } : {}) });
      attempt.status = response.status;
      if (response.status !== 200) throw Object.assign(new Error(`Acquisition HTTP ${response.status}: ${url}`), {
        code: response.status === 404 ? 'archive-unavailable' : 'network-failure',
      });
      return validateHttpResponse(url, response, policy, constraints);
    } catch (error) {
      attempt.error = error.message;
      throw error;
    }
  }
  async function run(args, timeoutMs) {
    const result = await deps.run(executable, [...shellArgs, ...args], { cwd, env: { ...env }, timeoutMs });
    logs.push({ stdout: result.stdout || '', stderr: result.stderr || '', exitCode: result.exitCode });
    if (result.exitCode !== 0) throw new Error(`Official installer process exited ${result.exitCode}: ${result.stderr || result.stdout || ''}`);
    return result.stdout || '';
  }
  try {
    await fs.mkdir(path.dirname(archivePath), { recursive: true });
    await fs.mkdir(root, { recursive: true });
    const response = await get(archive.url, download, constraints);
    const receivedAt = new Date().toISOString();
    const digests = await hashFile(fs, download, approval?.maxBytes);
    if (!digests.size) throw new Error('Downloaded archive is empty');
    if (!approval && digests.sha512 !== archive.sha512.toLowerCase()) throw new Error('Downloaded archive SHA512 does not match official metadata');
    if ((approval || response.size !== undefined) && response.size !== digests.size)
      throw new Error('HTTP byte count does not match downloaded archive bytes');
    for (const name of ['sha256', 'sha512'])
      if ((approval || response[name] !== undefined) &&
          (typeof response[name] !== 'string' || response[name].toLowerCase() !== digests[name]))
        throw new Error(`HTTP ${name} digest does not match downloaded archive bytes`);
    const scriptResponse = await get(installerUrl);
    if (!Buffer.isBuffer(scriptResponse.bytes) || scriptResponse.bytes.length === 0 || scriptResponse.bytes.length > 1_048_576)
      throw new Error('Official installer script response is missing or oversized');
    const scriptDigest = hash(scriptResponse.bytes, 'sha256');
    if (scriptResponse.sha256 && scriptResponse.sha256.toLowerCase() !== scriptDigest)
      throw new Error('Official installer script digest mismatch');
    const helperDigest = hash(await fs.readFile(helperPath), 'sha256');
    const cache = await fs.mkdtemp(path.join(path.dirname(archivePath), '.installer-'));
    const scriptPath = path.join(cache, `dotnet-install.${extension}`);
    await fs.writeFile(scriptPath, scriptResponse.bytes, { flag: 'wx' });
    if (!windows) await fs.writeFile(path.join(cache, '.dotnet-install.timestamp'), `${Math.floor(Date.now() / 1000)}\n`, { flag: 'wx' });
    receipt = {
      archive: {
        ...archive, sha256: digests.sha256, actualSha512: digests.sha512, redirectUrl: response.url, redirects: response.redirects,
        finalOrigin: new URL(response.url).origin, bytesReceived: digests.size, receivedAt, retained: { path: archivePath, ...digests },
      },
      installer: {
        url: installerUrl, sha256: scriptDigest, helperPath, helperSha256: helperDigest,
        path: scriptPath, redirectUrl: scriptResponse.url, redirects: scriptResponse.redirects,
      },
      attempts, sources: structuredClone(options.sources ?? []),
    };
    validateArchiveReceipt(options, receipt.archive);
    await fs.writeFile(`${archivePath}.provenance.json`, JSON.stringify(receipt, null, 2));
    stage = 'installation-failure';
    Object.assign(env, { RB_HELPER_PATH: helperPath, RB_INSTALLER_CACHE: cache, RB_INSTALLER_PATH: scriptPath });
    const helperOutput = await run(windows ? ['-Command', helperPS] : ['-c', helperSH], 60_000);
    const markers = helperOutput.split(/\r?\n/).filter(line => line.startsWith(marker));
    if (markers.length !== 1) throw new Error('Repository helper did not return exactly one installer path');
    const output = markers[0].slice(marker.length);
    const helperResult = windows ? JSON.parse(output) : { path: output };
    if (path.resolve(helperResult.path) !== path.resolve(scriptPath)) throw new Error('Repository helper returned an unexpected installer path');
    if (hash(await fs.readFile(scriptPath), 'sha256') !== scriptDigest ||
        hash(await fs.readFile(helperPath), 'sha256') !== helperDigest)
      throw new Error('Verified installer script or repository helper changed');
    const required = windows
      ? ['Version', 'InstallDir', 'Architecture', 'NoPath', 'KeepZip', 'ZipPath', 'AzureFeed']
      : ['version', 'install-dir', 'architecture', 'no-path', 'keep-zip', 'zip-path', 'azure-feed', 'os'];
    if (product !== 'sdk') required.push(...(windows ? ['Runtime', 'SkipNonVersionedFiles'] : ['runtime', 'skip-non-versioned-files']));
    const supported = windows
      ? new Set(helperResult.parameters.map(name => name.toLowerCase()))
      : new Set((await run([scriptPath, '--help'], 30_000)).match(/--[a-z][a-z-]*/g)?.map(name => name.slice(2)) || []);
    for (const flag of required)
      if (!supported.has(flag.toLowerCase())) throw new Error(`Official installer does not support required flag ${flag}`);
    const productDirectory = product === 'sdk' ? 'Sdk' : product === 'aspnetcore' ? 'aspnetcore/Runtime'
      : product === 'windowsdesktop' && Number(version.split('.')[0]) >= 5 ? 'WindowsDesktop' : 'Runtime';
    stage = 'checksum-provenance';
    const beforeInstall = await hashFile(fs, download, digests.size);
    if (beforeInstall.sha256 !== digests.sha256 || beforeInstall.sha512 !== digests.sha512 || beforeInstall.size !== digests.size)
      throw new Error('Downloaded archive changed before installer execution');
    feed = await startFeed(fs, download, archiveName, productDirectory, version, product, digests);
    let args;
    if (windows) {
      env.RB_INSTALLER_PARAMETERS = JSON.stringify({
        Version: version, InstallDir: root, Architecture: platform.architecture,
        NoPath: true, KeepZip: true, ZipPath: archivePath, AzureFeed: feed.url,
        ...(product !== 'sdk' ? { Runtime: product, SkipNonVersionedFiles: true } : {}),
      });
      args = ['-Command', installerPS];
    } else {
      args = [scriptPath, '--version', version, '--install-dir', root, '--architecture', platform.architecture,
        '--no-path', '--keep-zip', '--zip-path', archivePath, '--azure-feed', feed.url, '--os', os];
      if (product !== 'sdk') args.push('--runtime', product, '--skip-non-versioned-files');
    }
    stage = 'installation-failure';
    await run(args, 1_200_000);
    stage = 'checksum-provenance';
    await feed.verify();
    const afterInstall = await hashFile(fs, download, digests.size);
    if (afterInstall.sha512 !== digests.sha512 || afterInstall.sha256 !== digests.sha256 || afterInstall.size !== digests.size)
      throw new Error('Downloaded archive changed after installer execution');
    await verifyRetainedArchive(receipt.archive, { fs });
    if (typeof fs.chmod === 'function') {
      try { await fs.chmod(archivePath, 0o444); } catch (error) {
        if (!['ENOTSUP', 'EOPNOTSUPP', 'ENOSYS'].includes(error.code)) throw error;
      }
    }
    await verifyRetainedArchive(receipt.archive, { fs });
    await fs.writeFile(`${archivePath}.provenance.json`, JSON.stringify(receipt, null, 2));
    return { ...receipt, logs };
  } catch (error) {
    error.code ??= stage;
    error.attempts = attempts;
    error.logs = logs;
    if (receipt) error.acquisition = receipt;
    throw error;
  } finally {
    if (feed) await feed.close();
    await fs.rm(download, { force: true });
  }
}

module.exports = { installArchive, verifyRetainedArchive, hashFile };
