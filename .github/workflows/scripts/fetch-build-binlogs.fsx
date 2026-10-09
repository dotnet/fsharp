// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the License.txt file in the project root for more information.

#i "nuget: https://api.nuget.org/v3/index.json"
#r "nuget: FsHttp, 15.0.3"
#r "nuget: FSharp.Data, 8.2.0"

open System
open System.Collections.Generic
open System.IO
open System.IO.Compression
open System.Net.Http
open System.Security.Cryptography
open System.Text.RegularExpressions
open System.Threading

open FSharp.Data
open FsHttp

Fsi.disableDebugLogs ()

exception Skip of string

let env name =
    Environment.GetEnvironmentVariable name
    |> Option.ofObj
    |> Option.defaultValue ""

let ensure condition message =
    if not condition then
        raise (Skip message)

let invalid message = raise (InvalidDataException message)
let digits (value: string) = Regex.IsMatch(value, "^[0-9]+$")
let sha (value: string) = Regex.IsMatch(value, "^[0-9a-f]{40}$")

let sanitize (value: string) =
    let name = Regex.Replace(value, "[^A-Za-z0-9._-]", "_").Trim('.', '_', '-')
    name.Substring(0, min 80 name.Length)

let (?) (json: JsonValue) name =
    match json with
    | JsonValue.Record _ -> json.TryGetProperty name |> Option.defaultValue JsonValue.Null
    | _ -> JsonValue.Null

let text (json: JsonValue) =
    match json with
    | JsonValue.Record _
    | JsonValue.Array _ -> ""
    | _ -> json.AsString()

let items =
    function
    | JsonValue.Array values -> values
    | JsonValue.Null -> [||]
    | _ -> invalid "Expected a JSON array"

let trustedUrl (url: string) =
    match Uri.TryCreate(url, UriKind.Absolute) with
    | true, uri when uri.Scheme = "https" && uri.IsDefaultPort && uri.UserInfo = "" ->
        if uri.Host.Equals("dev.azure.com", StringComparison.OrdinalIgnoreCase) then
            [ "/dnceng-public/public/"
              "/dnceng-public/cbb18261-c48f-4abb-8651-8cdcb5474649/" ]
            |> List.exists (fun prefix -> uri.AbsolutePath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        else
            Regex.IsMatch(
                uri.Host,
                "^artprod(?:[.-]?[a-z0-9]+)\\.artifacts\\.visualstudio\\.com$",
                RegexOptions.IgnoreCase ||| RegexOptions.CultureInvariant
            )
            && uri.AbsolutePath.StartsWith(
                "/A6fcc92e5-73a7-4f88-8d13-d9045b45fb27/cbb18261-c48f-4abb-8651-8cdcb5474649/",
                StringComparison.OrdinalIgnoreCase
            )
    | _ -> false

let fetch github (url: string) seconds read =
    let rec send redirects (url: string) cancellation =
        let request =
            http {
                GET url
                UserAgent "fsharp-build-failure-analysis"
                config_timeoutInSeconds seconds
                config_cancellationToken cancellation

                config_transformHttpClientHandler (fun handler ->
                    handler.AllowAutoRedirect <- false
                    handler)
            }

        let token = env "GH_TOKEN"

        let headers =
            if github then
                [ "Accept", "application/vnd.github+json"
                  "X-GitHub-Api-Version", "2022-11-28"
                  if token <> "" then
                      "Authorization", $"Bearer {token}" ]
            else
                []

        use response = request |> Header.headers headers |> Request.send

        match int response.statusCode with
        | 301
        | 302
        | 303
        | 307
        | 308 when not github ->
            if redirects = 5 || isNull response.headers.Location then
                invalid "Artifact redirect limit or missing location"

            let next = Uri(Uri url, response.headers.Location).AbsoluteUri

            if not (trustedUrl next) then
                invalid "Artifact redirect is outside dnceng-public/public"

            response.dispose ()
            send (redirects + 1) next cancellation
        | _ ->
            response
            |> Response.toResult
            |> Result.map (fun response -> read response cancellation)
            |> Result.mapError (fun response -> int response.statusCode)

    let rec attempt number =
        if number > 1 then
            Thread.Sleep(2000 * number)

        try
            use cancellation = new CancellationTokenSource(TimeSpan.FromSeconds seconds)

            match send 0 url cancellation.Token with
            | Ok value -> value
            | Error(408 | 429 | 500 | 502 | 503 | 504) when number < 3 -> attempt (number + 1)
            | Error status -> raise (Skip $"HTTP {status} fetching build/PR data")
        with
        | :? InvalidDataException -> reraise ()
        | (:? OperationCanceledException | :? HttpRequestException | :? IOException) when number < 3 ->
            attempt (number + 1)

    attempt 1

let json github url =
    fetch github url (if github then 60. else 20.) (fun response cancellation ->
        response.content.ReadAsStringAsync(cancellation).GetAwaiter().GetResult()
        |> JsonValue.Parse)

let adoApi = "https://dev.azure.com/dnceng-public/public/_apis/build/builds"
let ado path = json false $"{adoApi}{path}"

let copy consume cancellation (source: Stream) (destination: Stream) =
    let buffer = Array.zeroCreate<byte> (1 <<< 20)

    let mutable count =
        source.ReadAsync(buffer, 0, buffer.Length, cancellation).GetAwaiter().GetResult()

    while count > 0 do
        consume (int64 count)
        destination.WriteAsync(buffer, 0, count, cancellation).GetAwaiter().GetResult()
        count <- source.ReadAsync(buffer, 0, buffer.Length, cancellation).GetAwaiter().GetResult()

type MetadataStream(source: Stream) =
    inherit Stream()
    let mutable remaining = 16L * 1024L * 1024L
    member _.Complete() = remaining <- Int64.MaxValue
    override _.CanRead = true
    override _.CanSeek = true
    override _.CanWrite = false
    override _.Length = source.Length

    override _.Position
        with get () = source.Position
        and set value = source.Position <- value

    override _.Read(buffer, offset, count) =
        if count > 0 && remaining = 0L then
            invalid "Archive metadata exceeds 16 MiB"

        let read = source.Read(buffer, offset, int (min (int64 count) remaining))
        remaining <- remaining - int64 read
        read

    override _.Seek(offset, origin) = source.Seek(offset, origin)
    override _.Flush() = ()
    override _.SetLength _ = raise (NotSupportedException())
    override _.Write(_, _, _) = raise (NotSupportedException())

let extract archive destination prefix budget label =
    if prefix = "" || sanitize prefix <> prefix then
        invalid "Unsafe output prefix"

    use source = File.OpenRead archive
    use metadata = new MetadataStream(source)
    use zip = new ZipArchive(metadata, ZipArchiveMode.Read)

    if zip.Entries.Count > 65536 then
        invalid "Archive exceeds 65536 entries"

    metadata.Complete()

    for entry in zip.Entries do
        let kind = (entry.ExternalAttributes >>> 16) &&& 0xF000

        if
            Regex.IsMatch(entry.FullName, @"(^[\\/]|(^|[\\/])\.\.([\\/]|$)|^[A-Za-z]:|\x00)")
            || not (List.contains kind [ 0; 0x8000; 0x4000 ])
        then
            invalid "Archive entry has an unsafe path or unsupported type"

    let selected =
        zip.Entries
        |> Seq.filter (fun entry ->
            (entry.ExternalAttributes >>> 16) &&& 0xF000 <> 0x4000
            && entry.Name <> ""
            && not (entry.FullName.EndsWith("/", StringComparison.Ordinal))
            && entry.FullName.EndsWith(".binlog", StringComparison.OrdinalIgnoreCase)
            && not (entry.FullName.EndsWith(".proto.binlog", StringComparison.OrdinalIgnoreCase)))
        |> Seq.toArray

    if selected.Length > 256 then
        invalid "Archive exceeds 256 binlogs"

    Directory.CreateDirectory destination |> ignore
    let created = ResizeArray<string>()
    let mutable written = 0L

    try
        for index, entry in Array.indexed selected do
            let suffix =
                match sanitize label with
                | "" -> ""
                | name -> $"_{name}"

            let path = Path.Combine(destination, $"{prefix}_{index}{suffix}.binlog")
            use input = entry.Open()
            use output = new FileStream(path, FileMode.CreateNew, FileAccess.Write)
            created.Add path

            copy
                (fun count ->
                    written <- written + count

                    if written > budget then
                        invalid "Extracted binlogs exceed the remaining budget")
                CancellationToken.None
                input
                output

        selected.Length, written
    with _ ->
        for path in created do
            File.Delete path

        reraise ()

let validateBuild prNumber buildId build =
    ensure
        (text build?id = buildId
         && text build?definition?id = "90"
         && (text build?repository?id).Equals("dotnet/fsharp", StringComparison.OrdinalIgnoreCase))
        "Build identity, definition, or repository does not match fsharp-ci"

    ensure (text build?status = "completed" && text build?result = "failed") "Build is not completed and failed"

    ensure
        (text build?sourceBranch = $"refs/pull/{prNumber}/merge"
         && text build?triggerInfo?``pr.number`` = prNumber)
        "Build does not belong to the requested PR"

    text build?triggerInfo?``pr.sourceSha``, text build?sourceVersion

let validatePr head merge pr =
    ensure
        (text pr?state = "open"
         && sha head
         && sha merge
         && text pr?head?sha = head
         && text pr?merge_commit_sha = merge)
        "PR closed or head/merge revision is stale or missing"

    let branch, repo = text pr?head?ref, text pr?head?repo?full_name

    ensure
        (branch <> ""
         && not (Regex.IsMatch(branch, @"[\x00-\x20\x7f]"))
         && Regex.IsMatch(repo, "^[A-Za-z0-9_.-]+/[A-Za-z0-9_.-]+$"))
        "Missing or invalid PR branch/repository"

    head, merge, branch, repo

let collect directory =
    let repo = env "GH_AW_REPO"
    ensure (repo = "dotnet/fsharp") "Expected GH_AW_REPO=dotnet/fsharp"

    let github path =
        json true $"https://api.github.com/{path}"

    let pull number = github $"repos/{repo}/pulls/{number}"

    let prNumber =
        match env "PR_NUMBER", env "CHECK_HEAD_SHA" with
        | "", head when sha head ->
            let search = github $"search/issues?q=repo:{repo}+is:pr+is:open+sha:{head}"
            ensure (text search?total_count = "1") "Check head does not identify exactly one open PR"

            let number =
                items search?items |> Array.exactlyOne |> (fun item -> text item?number)

            ensure (digits number && text (pull number)?head?sha = head) "Check head no longer matches the PR"
            number
        | number, _ -> number

    ensure (digits prNumber) "Missing or invalid PR number"

    let buildId =
        match env "RESOLVE_MODE" with
        | "dispatch" -> env "DISPATCH_BUILD_ID"
        | "check_run" -> Regex.Match(env "CHECK_DETAILS_URL", "buildId=([0-9]+)").Groups[1].Value
        | "latest" ->
            let builds =
                ado
                    $"?definitions=90&branchName=refs/pull/{prNumber}/merge&queryOrder=queueTimeDescending&$top=1&api-version=7.1"

            let newest =
                items builds?value |> Array.tryHead |> Option.defaultValue JsonValue.Null

            ensure (text newest?status = "completed") "Newest fsharp-ci build is missing or still running"
            text newest?id
        | mode -> raise (Skip $"Unknown RESOLVE_MODE '{mode}'")

    ensure (digits buildId) "Missing or invalid Azure build id"
    let build = ado $"/{buildId}?api-version=7.1"
    let buildHead, buildMerge = validateBuild prNumber buildId build
    let head, merge, branch, headRepo = validatePr buildHead buildMerge (pull prNumber)

    let records =
        ado $"/{buildId}/timeline?api-version=7.1"
        |> fun timeline -> items timeline?records

    let jobs =
        records
        |> Array.choose (fun record ->
            match text record?``type``, text record?result, Guid.TryParse(text record?id) with
            | "Job", ("failed" | "canceled"), (true, id) when not (String.IsNullOrWhiteSpace(text record?name)) ->
                Some {| Id = id; Name = text record?name |}
            | _ -> None)

    ensure (jobs.Length > 0) "No failed or canceled timeline jobs"

    let artifacts =
        ado $"/{buildId}/artifacts?api-version=7.1" |> fun result -> items result?value

    let selected =
        jobs
        |> Array.collect (fun job ->
            artifacts
            |> Array.choose (fun artifact ->
                let name = text artifact?name
                let matches = Guid.TryParse(text artifact?source) = (true, job.Id)

                let shape =
                    (text artifact?resource?``type`` = "PipelineArtifact"
                     && Regex.IsMatch(name, "_Attempt[0-9]+$", RegexOptions.IgnoreCase))
                    || Regex.IsMatch(name, "(?:binlogs|binarylogs)$", RegexOptions.IgnoreCase)

                if matches && shape && not (String.IsNullOrWhiteSpace name) then
                    Some
                        {| Name = name
                           Job = job
                           Url = text artifact?resource?downloadUrl |}
                else
                    None))
        |> Array.distinctBy _.Name
        |> Array.sortBy _.Name

    let expected = selected |> Seq.map _.Job.Id |> Set.ofSeq

    let published =
        records
        |> Seq.choose (fun record ->
            let name = text record?name

            match text record?``type``, text record?result, Guid.TryParse(text record?parentId) with
            | "Task", ("succeeded" | "succeededWithIssues"), (true, parent) when
                name.StartsWith("Publish", StringComparison.Ordinal)
                && name.EndsWith("Logs", StringComparison.Ordinal)
                ->
                Some parent
            | _ -> None)
        |> Set.ofSeq

    ensure
        (jobs
         |> Array.forall (fun job -> not (Set.contains job.Id published) || Set.contains job.Id expected))
        "A failed job published logs but has no matching build-log artifact"

    ensure (not expected.IsEmpty) "No build-log artifacts from failed jobs"
    let staged = HashSet<Guid>()
    let mutable compressed = 0L
    let mutable extracted = 0L
    let temporary = Directory.CreateTempSubdirectory("binlog-fetch-").FullName
    let archive = Path.Combine(temporary, "artifact.zip")

    try
        for index, artifact in Array.indexed selected do
            try
                if not (trustedUrl artifact.Url) then
                    invalid "Untrusted or missing artifact download URL"

                let mutable artifactBytes = 0L

                fetch false artifact.Url 120. (fun response cancellation ->
                    use source = Response.toStream response
                    use output = File.Create archive

                    copy
                        (fun count ->
                            artifactBytes <- artifactBytes + count
                            compressed <- compressed + count

                            if artifactBytes > 2147483648L || compressed > 3221225472L then
                                invalid "Compressed artifact budget exceeded")
                        cancellation
                        source
                        output)

                let count, written =
                    extract archive directory (string (index + 1)) (4294967296L - extracted) artifact.Name

                if count = 0 then
                    invalid "No regular binlogs in the artifact"

                extracted <- extracted + written
                staged.Add artifact.Job.Id |> ignore

                printfn
                    "Extracted %d binlogs (%d bytes) from %s for %s"
                    count
                    written
                    (sanitize artifact.Name)
                    (sanitize artifact.Job.Name)
            with error ->
                printfn "::warning::%s: %s" (sanitize artifact.Name) (error.Message.ReplaceLineEndings(" "))
    finally
        File.Delete archive
        Directory.Delete temporary

    ensure (expected |> Set.forall staged.Contains) "Incomplete binlogs for failed artifact-producing jobs"
    let hashes = HashSet<string>(StringComparer.Ordinal)

    for path in Directory.GetFiles(directory, "*.binlog") |> Array.sort do
        use file = File.OpenRead path
        let hash = Convert.ToHexString(SHA256.HashData file)
        file.Dispose()

        if not (hashes.Add hash) then
            File.Delete path

    ensure (hashes.Count > 0) "No usable binlogs recovered"

    ensure
        (validatePr head merge (pull prNumber) = (head, merge, branch, headRepo))
        "PR branch/repository changed during download"

    [ "binlog-found=true"
      $"pr-number={prNumber}"
      $"pr-head-sha={head}"
      $"pr-merge-sha={merge}"
      $"pr-head-ref={branch}"
      $"pr-head-repo={headRepo}"
      $"ado-build-id={buildId}"
      $"ado-build-url=https://dev.azure.com/dnceng-public/public/_build/results?buildId={buildId}" ]
    |> String.concat "\n"

let main arguments =
    try
        match arguments with
        | [| "--validate-url"; url |] ->
            if trustedUrl url then
                0
            else
                eprintfn "Untrusted artifact URL"
                1
        | [| "--extract"; archive; destination; prefix; budget |] ->
            let count, written = extract archive destination prefix (Int64.Parse budget) ""
            printfn "%d %d" count written
            0
        | [| "--extract"; archive; destination; prefix; budget; label |] ->
            let count, written = extract archive destination prefix (Int64.Parse budget) label
            printfn "%d %d" count written
            0
        | [||] ->
            let output = env "GITHUB_OUTPUT"
            File.AppendAllText(output, "binlog-found=false\n")
            let directory = env "BINLOG_DIR"

            ensure
                (directory <> "" && not (Directory.Exists directory || File.Exists directory))
                "BINLOG_DIR is unset or already exists"

            Directory.CreateDirectory directory |> ignore
            File.AppendAllText(output, collect directory + "\n")
            0
        | _ -> invalidArg "arguments" "Use --extract <archive> <dest> <prefix> <budget> [label] or --validate-url <url>"
    with
    | Skip message ->
        printfn "::warning::%s" message
        0
    | error ->
        eprintfn "::error::%s" (error.Message.ReplaceLineEndings(" "))
        1

if Path.GetFullPath(fsi.CommandLineArgs[0]) = Path.Combine(__SOURCE_DIRECTORY__, __SOURCE_FILE__) then
    fsi.CommandLineArgs |> Array.skip 1 |> main |> exit
