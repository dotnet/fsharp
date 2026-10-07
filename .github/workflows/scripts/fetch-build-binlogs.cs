#!/usr/bin/env dotnet
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Collects the binary logs of a completed, failed Azure Pipelines `fsharp-ci`
// PR build so the analysis agent can read them. Only published artifacts are
// downloaded; nothing here builds or executes PR code.
//
// Any validation or completeness gap emits `binlog-found=false`, which leaves
// the rest of the workflow inert. BINLOG_DIR must not already exist.
//
// Environment: RESOLVE_MODE, PR_NUMBER, CHECK_HEAD_SHA, CHECK_DETAILS_URL,
// DISPATCH_BUILD_ID, GH_TOKEN, GH_AW_REPO, BINLOG_DIR, GITHUB_OUTPUT.
//
// Usage: dotnet run --file ./fetch-build-binlogs.cs
//        dotnet run --file ./fetch-build-binlogs.cs -- --extract <archive> <dest> <prefix> <budget> [label]
//        dotnet run --file ./fetch-build-binlogs.cs -- --validate-url <url>

using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

if (args.Length > 0 && args[0] == "--extract")
{
    return RunExtractOnly(args[1..]);
}

if (args.Length > 0 && args[0] == "--validate-url")
{
    return args.Length == 2 && IsTrustedArtifactUrl(args[1]) ? 0 : 1;
}

var githubOutput = Environment.GetEnvironmentVariable("GITHUB_OUTPUT") ?? string.Empty;
if (githubOutput.Length == 0 || !TryAppendOutput(string.Empty))
{
    Console.Error.WriteLine("::error::GITHUB_OUTPUT is unset or not writable; refusing to run without a way to emit step outputs.");
    return 1;
}

var repo = Env("GH_AW_REPO");

// fsharp-ci in dnceng-public/public (public project; read anonymously).
const string AdoApi = "https://dev.azure.com/dnceng-public/public/_apis";
const string AdoBuildUi = "https://dev.azure.com/dnceng-public/public/_build/results";
const string AdoDefinitionId = "90";
const string ExpectedBuildRepository = "dotnet/fsharp";

var binlogDir = Env("BINLOG_DIR");
if (binlogDir.Length == 0 || Directory.Exists(binlogDir) || File.Exists(binlogDir))
{
    Console.Error.WriteLine("::error::BINLOG_DIR is unset or already exists; refusing to run.");
    return 1;
}

Directory.CreateDirectory(binlogDir);

using var github = new HttpClient();
github.DefaultRequestHeaders.UserAgent.ParseAdd("fsharp-build-failure-analysis");
github.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");
github.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
var token = Env("GH_TOKEN");
if (token.Length != 0)
{
    github.DefaultRequestHeaders.Authorization = new("Bearer", token);
}

using var ado = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false });
ado.DefaultRequestHeaders.UserAgent.ParseAdd("fsharp-build-failure-analysis");

// --- 1. Resolve and validate the PR number ---------------------------------
// Azure check payloads may omit pull_requests for fork PRs. The GitHub search
// index can resolve the head SHA, but only one open PR still at that SHA is
// accepted so an analysis cannot be posted to the wrong PR.
var prNumber = Env("PR_NUMBER");
var checkHeadSha = Env("CHECK_HEAD_SHA");
if (prNumber.Length == 0 && Regex.IsMatch(checkHeadSha, "^[0-9a-f]{40}$"))
{
    var search = await GitHubGet($"search/issues?q=repo:{repo}+is:pr+is:open+sha:{checkHeadSha}");
    if (!int.TryParse(search.At("total_count").Text(), out var matches))
    {
        matches = 0;
    }

    EmitNoneIf(matches > 1, $"Head {checkHeadSha} matches {matches} open PRs; refusing to guess which to analyze.");
    if (matches == 1)
    {
        var candidate = search.At("items").Items().FirstOrDefault().At("number").Text();
        var candidateHead = candidate.Length == 0
            ? string.Empty
            : (await GitHubGet($"repos/{repo}/pulls/{candidate}")).At("head", "sha").Text();
        EmitNoneIf(candidate.Length == 0 || candidateHead != checkHeadSha,
            $"PR #{candidate} is no longer at {checkHeadSha}; skipping a stale check run.");
        prNumber = candidate;
        Console.WriteLine($"Resolved PR #{prNumber} from check run head {checkHeadSha}.");
    }
}

EmitNoneIf(!Regex.IsMatch(prNumber, "^[0-9]+$"),
    $"Resolved PR number '{prNumber}' is not numeric or empty; refusing.");

// --- 2. Resolve and validate the Azure DevOps build id ----------------------
var resolveMode = Env("RESOLVE_MODE");
var buildId = string.Empty;
switch (resolveMode)
{
    case "dispatch":
        buildId = Env("DISPATCH_BUILD_ID");
        break;

    case "check_run":
        var details = Regex.Match(Env("CHECK_DETAILS_URL"), "buildId=([0-9]+)");
        buildId = details.Success ? details.Groups[1].Value : string.Empty;
        break;

    case "latest":
        // Query the newest build regardless of status. If it is still running,
        // skip instead of pairing an older failure with the current PR.
        var newest = (await AdoGet($"build list for PR #{prNumber}",
            $"{AdoApi}/build/builds?definitions={AdoDefinitionId}&branchName=refs/pull/{prNumber}/merge&queryOrder=queueTimeDescending&$top=1&api-version=7.1"))
            .At("value").Items().FirstOrDefault();
        buildId = newest.At("id").Text();
        var buildStatus = newest.At("status").Text();
        Console.WriteLine($"Newest fsharp-ci build for PR #{prNumber}: id='{buildId}' status='{buildStatus}'");
        EmitNoneIf(buildId.Length != 0 && buildStatus != "completed",
            $"PR #{prNumber}'s newest fsharp-ci build ({buildId}) is still '{buildStatus}'; wait for it to finish.");
        break;

    default:
        EmitNone($"Unknown RESOLVE_MODE '{resolveMode}'; refusing.");
        break;
}

EmitNoneIf(!Regex.IsMatch(buildId, "^[0-9]+$"),
    $"Resolved ADO build id '{buildId}' is not numeric or empty; refusing.");

// --- 3. Validate the build on every trigger path ---------------------------
var buildJson = await AdoGet($"details of build {buildId}", $"{AdoApi}/build/builds/{buildId}?api-version=7.1");
var result = buildJson.At("result").Text();
var definitionId = buildJson.At("definition", "id").Text();
var sourceBranch = buildJson.At("sourceBranch").Text();
var buildRepository = buildJson.At("repository", "id").Text();
var buildPrNumber = buildJson.At("triggerInfo", "pr.number").Text();
Console.WriteLine(
    $"ADO build {buildId}: result='{result}' definition='{definitionId}' sourceBranch='{sourceBranch}' repository='{buildRepository}'");
EmitNoneIf(definitionId != AdoDefinitionId,
    $"ADO build {buildId} is definition '{definitionId}', not fsharp-ci ({AdoDefinitionId}); refusing.");
EmitNoneIf(!buildRepository.Equals(ExpectedBuildRepository, StringComparison.OrdinalIgnoreCase),
    $"ADO build {buildId} belongs to repository '{buildRepository}', not {ExpectedBuildRepository}; refusing.");
EmitNoneIf(result != "failed",
    $"ADO build {buildId} did not fail (result='{result}'); nothing to analyze.");
EmitNoneIf(sourceBranch != $"refs/pull/{prNumber}/merge" || buildPrNumber != prNumber,
    $"ADO build {buildId} does not belong to PR #{prNumber}; refusing to avoid posting to the wrong PR.");

// --- 4. Require the build to describe the PR's current revision ------------
// sourceVersion is the GitHub merge ref commit. Checking it as well as the PR
// head detects a base-branch advance while the head stays unchanged.
var prJson = await GitHubGet($"repos/{repo}/pulls/{prNumber}");
var buildPrSha = buildJson.At("triggerInfo", "pr.sourceSha").Text();
var buildMergeSha = buildJson.At("sourceVersion").Text();
var currentHead = prJson.At("head", "sha").Text();
var currentMerge = prJson.At("merge_commit_sha").Text();
var currentState = prJson.At("state").Text();
EmitNoneIf(currentState != "open", $"PR #{prNumber} is not open; skipping.");
EmitNoneIf(buildPrSha.Length == 0 || currentHead.Length == 0 || buildMergeSha.Length == 0 || currentMerge.Length == 0,
    "Could not resolve all build/current head and merge revisions; skipping to avoid analyzing a stale binlog.");
EmitNoneIf(buildPrSha != currentHead,
    $"Build {buildId} analyzed '{buildPrSha}' but PR #{prNumber} head is now '{currentHead}'; skipping stale build.");
EmitNoneIf(buildMergeSha != currentMerge,
    $"Build {buildId} merge revision '{buildMergeSha}' but PR #{prNumber} current merge is '{currentMerge}' (base advanced); skipping stale merge.");
var headSha = currentHead;
Console.WriteLine($"Analyzing build {buildId} at PR head revision '{headSha}'.");

// --- 5. Select build-log artifacts produced by failed/canceled jobs --------
// Artifact `source` is the exact Azure timeline job GUID. This avoids guessing
// from FSharp's heterogeneous artifact names. Prefer only known build-log
// shapes: Arcade's automatic *_AttemptN artifact, explicit *binlogs artifacts,
// and regression-test *_BinaryLogs artifacts. Other same-job artifacts are
// test results, dumps, packages, or evidence and are intentionally ignored.
var records = (await AdoGet($"timeline of build {buildId}",
        $"{AdoApi}/build/builds/{buildId}/timeline?api-version=7.1"))
    .At("records").Items().ToList();
var failedJobs = records
    .Where(record => record.At("type").Text() == "Job" && record.At("result").Text() is "failed" or "canceled")
    .Select(record => (
        Name: record.At("name").Text(),
        Id: record.At("id").Text()))
    .Where(job => job.Name.Trim().Length != 0 && Regex.IsMatch(job.Id, "^[0-9a-fA-F-]{36}$"))
    .ToList();
EmitNoneIf(failedJobs.Count == 0, $"No failed or canceled jobs in the timeline for build {buildId}.");

var allArtifacts = (await AdoGet($"artifact list of build {buildId}",
        $"{AdoApi}/build/builds/{buildId}/artifacts?api-version=7.1"))
    .At("value").Items()
    .Select(artifact => (
        Node: artifact,
        Name: artifact.At("name").Text(),
        Source: artifact.At("source").Text(),
        Type: artifact.At("resource", "type").Text()))
    .Where(artifact => artifact.Name.Trim().Length != 0)
    .ToList();

static bool IsBuildLogArtifact(string name, string type)
    => (type == "PipelineArtifact" && Regex.IsMatch(name, @"_Attempt[0-9]+$", RegexOptions.IgnoreCase))
        || Regex.IsMatch(name, @"(?:binlogs|binarylogs)$", RegexOptions.IgnoreCase);

var selectedArtifacts = failedJobs
    .SelectMany(job => allArtifacts
        .Where(artifact =>
            artifact.Source.Equals(job.Id, StringComparison.OrdinalIgnoreCase)
            && IsBuildLogArtifact(artifact.Name, artifact.Type))
        .Select(artifact => (
            artifact.Node,
            artifact.Name,
            JobId: job.Id,
            JobName: job.Name)))
    .DistinctBy(artifact => artifact.Name, StringComparer.Ordinal)
    .OrderBy(artifact => artifact.Name, StringComparer.Ordinal)
    .ToList();

// A successful "Publish ... Logs" task means the job intended to expose its
// logs. If its matching build-log artifact is absent, analyzing the remaining
// jobs could produce a confident-looking diagnosis from incomplete evidence.
var publishedLogJobs = records
    .Where(record =>
        record.At("type").Text() == "Task"
        && record.At("result").Text() is "succeeded" or "succeededWithIssues")
    .Select(record => (
        Name: record.At("name").Text(),
        Parent: record.At("parentId").Text()))
    .Where(record =>
        record.Name.StartsWith("Publish", StringComparison.Ordinal)
        && record.Name.EndsWith("Logs", StringComparison.Ordinal))
    .Select(record => record.Parent)
    .ToHashSet(StringComparer.OrdinalIgnoreCase);
var uncoveredJobs = failedJobs
    .Where(job =>
        publishedLogJobs.Contains(job.Id)
        && !selectedArtifacts.Any(artifact =>
            artifact.JobId.Equals(job.Id, StringComparison.OrdinalIgnoreCase)))
    .ToList();
EmitNoneIf(uncoveredJobs.Count != 0,
    $"Build {buildId} is missing a build-log artifact for {uncoveredJobs.Count} failed jobs that published logs "
        + $"({string.Join(", ", uncoveredJobs.Take(3).Select(job => Extractor.Sanitize(job.Name)))}); skipping incomplete failed-job data.");

var expectedJobs = selectedArtifacts
    .Select(artifact => artifact.JobId)
    .ToHashSet(StringComparer.OrdinalIgnoreCase);
EmitNoneIf(selectedArtifacts.Count == 0,
    $"No build-log artifacts matched the failed or canceled jobs in build {buildId}; the failure is likely outside a build leg.");
Console.WriteLine(
    $"Selected {selectedArtifacts.Count} build-log artifacts for {expectedJobs.Count} of {failedJobs.Count} failed or canceled jobs.");

// --- 6. Download and safely extract regular binlogs ------------------------
const long MaxZipBytes = 2147483648;       // 2 GB compressed per artifact
const long MaxTotalBytes = 4294967296;     // 4 GB extracted across all artifacts
const long MaxTotalZipBytes = 3221225472;  // 3 GB compressed across all artifacts
var totalZipBytes = 0L;
var remainingBytes = MaxTotalBytes;
var stagedJobs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

var zipDir = Directory.CreateTempSubdirectory("binlog-fetch-").FullName;
var zipTmp = Path.Combine(zipDir, "artifact.zip");

var artifactIndex = 0;
foreach (var (node, name, jobId, jobName) in selectedArtifacts)
{
    artifactIndex++;
    var safeName = Extractor.Sanitize(name);
    var safeJob = Extractor.Sanitize(jobName);
    var url = node.At("resource", "downloadUrl").Text();
    if (url.Length == 0)
    {
        Console.WriteLine($"::warning::Skipping {safeName}: no download URL.");
        continue;
    }

    if (!IsTrustedArtifactUrl(url))
    {
        Console.WriteLine($"::warning::Skipping {safeName}: download URL is not a dnceng-public/public artifact URL.");
        continue;
    }

    var zipCap = Math.Min(MaxZipBytes, MaxTotalZipBytes - totalZipBytes);
    if (zipCap <= 0)
    {
        Console.WriteLine(
            $"::warning::Cumulative compressed download budget {MaxTotalZipBytes} is exhausted before {safeName}; stopping downloads.");
        break;
    }

    var (zipBytes, downloadError) = await Download(url, zipTmp, zipCap);
    totalZipBytes += zipBytes;
    if (downloadError is not null || zipBytes == 0)
    {
        Console.WriteLine($"::warning::Skipping {safeName}: download failed or was empty ({downloadError ?? "empty body"}).");
        continue;
    }

    int extracted;
    long written;
    try
    {
        (extracted, written) = Extractor.Extract(
            zipTmp, binlogDir, artifactIndex.ToString(), remainingBytes, safeName);
    }
    catch (Exception ex)
    {
        DeletePartials(artifactIndex);
        Console.WriteLine(
            $"::warning::Skipping {safeName}: extraction failed ({ex.Message.ReplaceLineEndings(" ")}).");
        continue;
    }

    if (extracted == 0)
    {
        DeletePartials(artifactIndex);
        Console.WriteLine($"::warning::Skipping {safeName}: no regular binlogs found in the artifact.");
        continue;
    }

    remainingBytes -= written;
    stagedJobs.Add(jobId);
    Console.WriteLine($"Extracted {extracted} binlog(s) ({written} bytes) from {safeName} for {safeJob}.");
}

TryDelete(zipTmp);
try
{
    Directory.Delete(zipDir);
}
catch (Exception)
{
}

var missingJobs = expectedJobs.Except(stagedJobs, StringComparer.OrdinalIgnoreCase).ToList();
EmitNoneIf(missingJobs.Count != 0,
    $"No usable binlog was recovered for {missingJobs.Count} of {expectedJobs.Count} failed jobs with build-log artifacts; skipping incomplete failed-job data.");

var duplicates = RemoveDuplicateBinlogs(binlogDir);
var staged = Directory.EnumerateFiles(binlogDir, "*.binlog").Order(StringComparer.Ordinal).ToList();
EmitNoneIf(staged.Count == 0, $"No usable *.binlog was found in the selected artifacts of build {buildId}.");

Console.WriteLine(
    $"Staged {staged.Count} unique binlog(s) for {stagedJobs.Count} failed job(s)"
        + (duplicates == 0 ? ":" : $" after removing {duplicates} duplicate(s):"));
foreach (var path in staged)
{
    Console.WriteLine($"  {new FileInfo(path).Length,12}  {Path.GetFileName(path)}");
}

// --- 7. Re-check the revision after downloads that can take minutes --------
var latestPr = await GitHubGet($"repos/{repo}/pulls/{prNumber}");
var latestHead = latestPr.At("head", "sha").Text();
var latestMerge = latestPr.At("merge_commit_sha").Text();
var latestState = latestPr.At("state").Text();
EmitNoneIf(latestState != "open", $"PR #{prNumber} closed during artifact download; skipping.");
EmitNoneIf(latestHead != headSha,
    $"PR #{prNumber} head changed during download ('{headSha}' -> '{latestHead}') or could not be re-resolved; skipping.");
EmitNoneIf(latestMerge != buildMergeSha,
    $"PR #{prNumber} merge revision changed during download ('{buildMergeSha}' -> '{latestMerge}') or could not be re-resolved; skipping.");

TryAppendOutput(
    "binlog-found=true\n" +
    $"pr-number={prNumber}\n" +
    $"pr-head-sha={headSha}\n" +
    $"pr-merge-sha={buildMergeSha}\n" +
    $"ado-build-id={buildId}\n" +
    $"ado-build-url={AdoBuildUi}?buildId={buildId}\n");
return 0;

static string Env(string name) => Environment.GetEnvironmentVariable(name) ?? string.Empty;

bool TryAppendOutput(string text)
{
    try
    {
        File.AppendAllText(githubOutput, text);
        return true;
    }
    catch (Exception)
    {
        return false;
    }
}

void EmitNone(string? reason = null)
{
    if (reason is not null)
    {
        Console.WriteLine($"::warning::{reason}");
    }

    TryAppendOutput("binlog-found=false\n");
    Environment.Exit(0);
}

void EmitNoneIf(bool condition, string reason)
{
    if (condition)
    {
        EmitNone(reason);
    }
}

static void TryDelete(string path)
{
    try
    {
        File.Delete(path);
    }
    catch (Exception)
    {
    }
}

void DeletePartials(int prefix)
{
    foreach (var partial in Directory.EnumerateFiles(binlogDir, $"{prefix}_*.binlog"))
    {
        TryDelete(partial);
    }
}

static int RemoveDuplicateBinlogs(string directory)
{
    var hashes = new HashSet<string>(StringComparer.Ordinal);
    var removed = 0;
    foreach (var path in Directory.EnumerateFiles(directory, "*.binlog").Order(StringComparer.Ordinal))
    {
        using var stream = File.OpenRead(path);
        var hash = Convert.ToHexString(SHA256.HashData(stream));
        if (hashes.Add(hash))
        {
            continue;
        }

        File.Delete(path);
        removed++;
    }

    return removed;
}

async Task<JsonNode?> GitHubGet(string path)
{
    var (body, error) = await Fetch(
        github,
        $"https://api.github.com/{path}",
        TimeSpan.FromSeconds(60),
        (response, cancellation) => response.Content.ReadAsStringAsync(cancellation));
    return error is null ? Parse(body!) : null;
}

async Task<JsonNode?> AdoGet(string what, string url)
{
    var (body, error) = await Fetch(
        ado,
        url,
        TimeSpan.FromSeconds(20),
        (response, cancellation) => response.Content.ReadAsStringAsync(cancellation));
    var document = error is null && body!.Length != 0 ? Parse(body!) : null;
    if (document is null)
    {
        EmitNone(
            $"Could not fetch a usable {what} from Azure DevOps ({error ?? "empty or non-JSON body"}); treating as a data-resolution failure.");
    }

    return document;
}

static JsonNode? Parse(string body)
{
    try
    {
        return JsonNode.Parse(body);
    }
    catch (JsonException)
    {
        return null;
    }
}

static bool IsTransient(HttpStatusCode status)
    => status is HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests
        or HttpStatusCode.InternalServerError or HttpStatusCode.BadGateway
        or HttpStatusCode.ServiceUnavailable or HttpStatusCode.GatewayTimeout;

static bool IsTrustedArtifactUrl(string url)
{
    const string CollectionId = "6fcc92e5-73a7-4f88-8d13-d9045b45fb27";
    const string ProjectId = "cbb18261-c48f-4abb-8651-8cdcb5474649";

    if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)
        || uri.Scheme != Uri.UriSchemeHttps
        || !uri.IsDefaultPort)
    {
        return false;
    }

    var path = uri.AbsolutePath;
    if (uri.Host.Equals("dev.azure.com", StringComparison.OrdinalIgnoreCase))
    {
        return path.StartsWith("/dnceng-public/public/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith($"/dnceng-public/{ProjectId}/", StringComparison.OrdinalIgnoreCase);
    }

    return Regex.IsMatch(
            uri.Host,
            @"^artprod(?:[.-]?[a-z0-9]+)\.artifacts\.visualstudio\.com$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)
        && path.StartsWith($"/A{CollectionId}/{ProjectId}/", StringComparison.OrdinalIgnoreCase);
}

static bool IsRedirect(HttpStatusCode status)
    => status is HttpStatusCode.MovedPermanently or HttpStatusCode.Found or HttpStatusCode.SeeOther
        or HttpStatusCode.TemporaryRedirect or HttpStatusCode.PermanentRedirect;

static async Task<HttpResponseMessage> Get(
    HttpClient client,
    string url,
    HttpCompletionOption completion,
    CancellationToken cancellation)
{
    const int MaxRedirects = 5;

    var response = await client.GetAsync(url, completion, cancellation);
    for (var redirect = 0; redirect < MaxRedirects && IsRedirect(response.StatusCode); redirect++)
    {
        Uri? next = null;
        try
        {
            var location = response.Headers.Location;
            if (location is not null)
            {
                next = new Uri(new Uri(url), location);
            }
        }
        catch (UriFormatException)
        {
        }

        response.Dispose();
        if (next is null || !IsTrustedArtifactUrl(next.AbsoluteUri))
        {
            Console.WriteLine(
                "::warning::Refusing an artifact redirect outside dnceng-public/public.");
            return new HttpResponseMessage(HttpStatusCode.Forbidden);
        }

        url = next.AbsoluteUri;
        response = await client.GetAsync(url, completion, cancellation);
    }

    if (!IsRedirect(response.StatusCode))
    {
        return response;
    }

    response.Dispose();
    Console.WriteLine($"::warning::Refusing an artifact after more than {MaxRedirects} redirects.");
    return new HttpResponseMessage(HttpStatusCode.Forbidden);
}

static async Task<(T? Value, string? Error)> Fetch<T>(
    HttpClient client,
    string url,
    TimeSpan timeout,
    Func<HttpResponseMessage, CancellationToken, Task<T>> read)
{
    var error = "no attempt was made";
    for (var attempt = 1; attempt <= 3; attempt++)
    {
        if (attempt != 1)
        {
            await Task.Delay(TimeSpan.FromSeconds(2 * attempt));
        }

        try
        {
            using var cts = new CancellationTokenSource(timeout);
            using var response = await Get(
                client,
                url,
                HttpCompletionOption.ResponseHeadersRead,
                cts.Token);
            if (response.IsSuccessStatusCode)
            {
                return (await read(response, cts.Token), null);
            }

            error = $"HTTP {(int)response.StatusCode}";
            if (!IsTransient(response.StatusCode))
            {
                break;
            }
        }
        catch (Exception ex) when (ex is OperationCanceledException or HttpRequestException or IOException)
        {
            error = ex is OperationCanceledException ? "timed out" : ex.GetType().Name;
        }
    }

    return (default, error);
}

async Task<(long Bytes, string? Error)> Download(string url, string path, long cap)
{
    var (bytes, error) = await Fetch(
        ado,
        url,
        TimeSpan.FromMinutes(2),
        async (response, cancellation) =>
        {
            await using var source = await response.Content.ReadAsStreamAsync(cancellation);
            await using var output = File.Create(path);
            var buffer = new byte[1 << 20];
            long written = 0;
            int read;
            while ((read = await source.ReadAsync(buffer, cancellation)) > 0)
            {
                if ((written += read) > cap)
                {
                    return -1;
                }

                await output.WriteAsync(buffer.AsMemory(0, read), cancellation);
            }

            return written;
        });

    return error is not null
        ? (0, error)
        : bytes < 0
            ? (cap, $"exceeded the {cap}-byte size cap")
            : (bytes, null);
}

static int RunExtractOnly(string[] extractArgs)
{
    if (extractArgs.Length is < 4 or > 5 || !long.TryParse(extractArgs[3], out var budgetBytes))
    {
        Console.Error.WriteLine(
            "usage: fetch-build-binlogs.cs --extract <archive> <dest> <prefix> <budget> [label]");
        return 1;
    }

    try
    {
        var (count, written) = Extractor.Extract(
            extractArgs[0],
            extractArgs[1],
            extractArgs[2],
            budgetBytes,
            extractArgs.Length == 5 ? extractArgs[4] : string.Empty);
        Console.Out.WriteLine($"{count} {written}");
        return 0;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine(ex.Message);
        return 1;
    }
}

static class Extractor
{
    private const int MaxEntries = 65536;
    private const int MaxBinlogs = 256;
    private const long MaxMetadataBytes = 16 * 1024 * 1024;

    public static (int Count, long Written) Extract(
        string archivePath,
        string destination,
        string prefix,
        long budgetBytes,
        string label)
    {
        var safeLabel = Sanitize(label);

        using var archive = File.OpenRead(archivePath);
        using var metadata = new MetadataReadStream(archive);
        using var zip = new ZipArchive(metadata, ZipArchiveMode.Read);

        if (zip.Entries.Count > MaxEntries)
        {
            throw new InvalidDataException(
                $"archive holds {zip.Entries.Count} entries, above the {MaxEntries} allowed");
        }

        metadata.CompleteMetadataRead();

        for (var i = 0; i < zip.Entries.Count; i++)
        {
            if (IsUnsafePath(zip.Entries[i].FullName) || IsUnsupportedType(zip.Entries[i]))
            {
                throw new InvalidDataException(
                    $"archive entry {i} has an unsafe path or an unsupported type");
            }
        }

        var selected = zip.Entries
            .Where(entry =>
                !IsDirectoryEntry(entry)
                && entry.FullName.EndsWith(".binlog", StringComparison.OrdinalIgnoreCase)
                && !entry.FullName.EndsWith(".proto.binlog", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        if (selected.Length > MaxBinlogs)
        {
            throw new InvalidDataException(
                $"archive holds {selected.Length} binlogs, above the {MaxBinlogs} allowed");
        }

        Directory.CreateDirectory(destination);

        var written = 0L;
        var buffer = new byte[1024 * 1024];
        var created = new List<string>(selected.Length);
        try
        {
            for (var index = 0; index < selected.Length; index++)
            {
                var stem = safeLabel.Length == 0
                    ? $"{prefix}_{index}"
                    : $"{prefix}_{index}_{safeLabel}";
                var outputPath = Path.Combine(destination, $"{stem}.binlog");

                using var source = selected[index].Open();
                using var output = new FileStream(
                    outputPath,
                    FileMode.CreateNew,
                    FileAccess.Write);
                created.Add(outputPath);

                int read;
                while ((read = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    written += read;
                    if (written > budgetBytes)
                    {
                        throw new InvalidDataException("extracted binlogs exceed the remaining budget");
                    }

                    output.Write(buffer, 0, read);
                }
            }
        }
        catch
        {
            foreach (var path in created)
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception)
                {
                    // Preserve the extraction failure that triggered cleanup.
                }
            }

            throw;
        }

        return (selected.Length, written);
    }

    private sealed class MetadataReadStream(Stream source) : Stream
    {
        private long _remaining = MaxMetadataBytes;

        public void CompleteMetadataRead() => _remaining = long.MaxValue;

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length => source.Length;
        public override long Position
        {
            get => source.Position;
            set => source.Position = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
            => Read(buffer.AsSpan(offset, count));

        public override int Read(Span<byte> buffer)
        {
            if (buffer.Length != 0 && _remaining == 0)
            {
                throw new InvalidDataException(
                    $"archive metadata exceeds the {MaxMetadataBytes}-byte read budget");
            }

            var read = source.Read(buffer[..(int)Math.Min(buffer.Length, _remaining)]);
            _remaining -= read;
            return read;
        }

        public override long Seek(long offset, SeekOrigin origin) => source.Seek(offset, origin);
        public override void Flush()
        {
        }

        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }

    public static string Sanitize(string value)
    {
        var result = Regex.Replace(value, "[^A-Za-z0-9._-]", "_").Trim('.', '_', '-');
        return result.Length > 80 ? result[..80] : result;
    }

    private static bool IsUnsafePath(string name)
    {
        var parts = name.Replace('\\', '/').Split('/');
        var first = parts[0];
        return name.Contains('\0')
            || name.StartsWith('/')
            || name.StartsWith('\\')
            || Array.IndexOf(parts, "..") >= 0
            || (first.Length >= 2 && char.IsAsciiLetter(first[0]) && first[1] == ':');
    }

    private static bool IsUnsupportedType(ZipArchiveEntry entry)
    {
        var fileType = (entry.ExternalAttributes >> 16) & 0xF000;
        return fileType is not (0 or 0x8000 or 0x4000);
    }

    private static bool IsDirectoryEntry(ZipArchiveEntry entry)
        => entry.FullName.EndsWith('/') || entry.Name.Length == 0;
}

static class Json
{
    public static JsonNode? At(this JsonNode? node, params string[] path)
    {
        foreach (var name in path)
        {
            node = (node as JsonObject)?[name];
        }

        return node;
    }

    public static string Text(this JsonNode? node)
        => (node as JsonValue)?.ToString() ?? string.Empty;

    public static IEnumerable<JsonNode?> Items(this JsonNode? node)
        => node as JsonArray ?? [];
}
