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
exception Status of int

let env name =
    Environment.GetEnvironmentVariable name
    |> Option.ofObj
    |> Option.defaultValue ""

let ensure condition message =
    if not condition then
        raise (Skip message)

let require condition message =
    if not condition then
        raise (InvalidDataException message)

let digits (value: string) = Regex.IsMatch(value, "^[0-9]+$")
let sha (value: string) = Regex.IsMatch(value, "^[0-9a-f]{40}$")

let sanitize (value: string) =
    let name = Regex.Replace(value, "[^A-Za-z0-9._-]", "_").Trim('.', '_', '-')
    name.Substring(0, min 80 name.Length)

let (?) (json: JsonValue) name =
    defaultArg (json.TryGetProperty name) JsonValue.Null

let text (json: JsonValue) =
    match json with
    | JsonValue.Record _
    | JsonValue.Array _ -> ""
    | _ -> json.AsString()

let items =
    function
    | JsonValue.Array values -> values
    | JsonValue.Null -> [||]
    | _ -> raise (InvalidDataException "Expected a JSON array")

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
            require
                (redirects < 5 && not (isNull response.headers.Location))
                "Artifact redirect limit or missing location"

            let next = Uri(Uri url, response.headers.Location).AbsoluteUri
            require (trustedUrl next) "Artifact redirect is outside dnceng-public/public"
            response.dispose ()
            send (redirects + 1) next cancellation
        | _ ->
            match Response.toResult response with
            | Ok response -> read response cancellation
            | Error response -> raise (Status(int response.statusCode))

    let rec attempt number =
        if number > 1 then
            Thread.Sleep(2000 * number)

        try
            use cancellation = new CancellationTokenSource(TimeSpan.FromSeconds seconds)
            send 0 url cancellation.Token
        with
        | Status(408 | 429 | 500 | 502 | 503 | 504)
        | :? OperationCanceledException
        | :? HttpRequestException
        | :? IOException when number < 3 -> attempt (number + 1)
        | Status code -> raise (Skip $"HTTP {code} fetching build/PR data")

    attempt 1

let json github url =
    fetch github url (if github then 60. else 20.) (fun response cancellation ->
        response.content.ReadAsStringAsync(cancellation).GetAwaiter().GetResult()
        |> JsonValue.Parse)

let adoApi = "https://dev.azure.com/dnceng-public/public/_apis/build/builds"
let ado path = json false $"{adoApi}{path}"

let copy (total: int64 ref) limit cancellation (source: Stream) (destination: Stream) =
    let buffer = Array.zeroCreate<byte> (1 <<< 20)

    let mutable count =
        source.ReadAsync(buffer, 0, buffer.Length, cancellation).GetAwaiter().GetResult()

    while count > 0 do
        total.Value <- total.Value + int64 count
        require (total.Value <= limit) "Compressed or extracted size budget exceeded"
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
        require (count <= 0 || remaining > 0L) "Archive metadata exceeds 16 MiB"
        let read = source.Read(buffer, offset, int (min (int64 count) remaining))
        remaining <- remaining - int64 read
        read

    override _.Seek(offset, origin) = source.Seek(offset, origin)
    override _.Flush() = ()
    override _.SetLength _ = raise (NotSupportedException())
    override _.Write(_, _, _) = raise (NotSupportedException())

let extract archive destination prefix budget label =
    require (prefix <> "" && sanitize prefix = prefix) "Unsafe output prefix"
    use source = File.OpenRead archive
    use metadata = new MetadataStream(source)
    use zip = new ZipArchive(metadata, ZipArchiveMode.Read)

    require (zip.Entries.Count <= 65536) "Archive exceeds 65536 entries"
    metadata.Complete()

    let selected =
        [| for entry in zip.Entries do
               let kind = (entry.ExternalAttributes >>> 16) &&& 0xF000

               require
                   (not (Regex.IsMatch(entry.FullName, @"(^[\\/]|(^|[\\/])\.\.([\\/]|$)|^[A-Za-z]:|\x00)"))
                    && List.contains kind [ 0; 0x8000; 0x4000 ])
                   "Archive entry has an unsafe path or unsupported type"

               if
                   kind <> 0x4000
                   && entry.FullName.EndsWith(".binlog", StringComparison.OrdinalIgnoreCase)
                   && not (entry.FullName.EndsWith(".proto.binlog", StringComparison.OrdinalIgnoreCase))
               then
                   entry |]

    require (selected.Length <= 256) "Archive exceeds 256 binlogs"
    Directory.CreateDirectory destination |> ignore
    let created = ResizeArray<string>()
    let written = ref 0L

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
            copy written budget CancellationToken.None input output

        selected.Length, written.Value
    with _ ->
        Seq.iter File.Delete created
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

            match items builds?value |> Array.tryHead with
            | Some newest when text newest?status = "completed" -> text newest?id
            | _ -> raise (Skip "Newest fsharp-ci build is missing or still running")
        | mode -> raise (Skip $"Unknown RESOLVE_MODE '{mode}'")

    ensure (digits buildId) "Missing or invalid Azure build id"
    let build = ado $"/{buildId}?api-version=7.1"
    let buildHead, buildMerge = validateBuild prNumber buildId build
    let head, merge, branch, headRepo = validatePr buildHead buildMerge (pull prNumber)

    let records = items (ado $"/{buildId}/timeline?api-version=7.1")?records

    let jobs =
        records
        |> Array.choose (fun record ->
            match text record?``type``, text record?result, Guid.TryParse(text record?id) with
            | "Job", ("failed" | "canceled"), (true, id) when not (String.IsNullOrWhiteSpace(text record?name)) ->
                Some(id, text record?name)
            | _ -> None)
        |> Map.ofArray

    ensure (not jobs.IsEmpty) "No failed or canceled timeline jobs"

    let selected =
        items (ado $"/{buildId}/artifacts?api-version=7.1")?value
        |> Array.choose (fun artifact ->
            let name = text artifact?name

            let shape =
                (text artifact?resource?``type`` = "PipelineArtifact"
                 && Regex.IsMatch(name, "_Attempt[0-9]+$", RegexOptions.IgnoreCase))
                || Regex.IsMatch(name, "(?:binlogs|binarylogs)$", RegexOptions.IgnoreCase)

            match Guid.TryParse(text artifact?source) with
            | true, job when shape && jobs.ContainsKey job -> Some(name, job, text artifact?resource?downloadUrl)
            | _ -> None)
        |> Array.sort
        |> Array.distinctBy (fun (name, job, _) -> name, job)

    let expected = set [ for _, job, _ in selected -> job ]

    let published =
        records
        |> Seq.choose (fun record ->
            let name = text record?name

            match text record?``type``, text record?result, Guid.TryParse(text record?parentId) with
            | "Task", ("succeeded" | "succeededWithIssues"), (true, parent) when
                jobs.ContainsKey parent
                && name.StartsWith("Publish", StringComparison.Ordinal)
                && name.EndsWith("Logs", StringComparison.Ordinal)
                ->
                Some parent
            | _ -> None)
        |> Set.ofSeq

    ensure (published.IsSubsetOf expected) "A failed job published logs but has no matching build-log artifact"

    ensure (not expected.IsEmpty) "No build-log artifacts from failed jobs"
    let staged = HashSet<Guid>()
    let compressed = ref 0L
    let mutable extracted = 0L
    let archive = Path.GetTempFileName()

    try
        for index, (name, job, url) in Array.indexed selected do
            try
                require (trustedUrl url) "Untrusted or missing artifact download URL"
                let limit = min 3221225472L (compressed.Value + 2147483648L)

                fetch false url 120. (fun response cancellation ->
                    use source = Response.toStream response
                    use output = File.Create archive

                    copy compressed limit cancellation source output)

                let count, written =
                    extract archive directory (string (index + 1)) (4294967296L - extracted) name

                require (count > 0) "No regular binlogs in the artifact"
                extracted <- extracted + written
                staged.Add job |> ignore

                printfn $"Extracted {count} binlogs ({written} bytes) from {sanitize name} for {sanitize jobs[job]}"
            with error ->
                printfn "::warning::%s: %s" (sanitize name) (error.Message.ReplaceLineEndings(" "))
    finally
        File.Delete archive

    ensure (expected |> Set.forall staged.Contains) "Incomplete binlogs for failed artifact-producing jobs"
    let hashes = HashSet<string>()

    for path in Directory.GetFiles(directory, "*.binlog") |> Array.sort do
        if not (hashes.Add(using (File.OpenRead path) SHA256.HashData |> Convert.ToHexString)) then
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
        | [| "--validate-url"; url |] when trustedUrl url -> 0
        | [| "--validate-url"; _ |] ->
            eprintfn "Untrusted artifact URL"
            1
        | [| "--extract"; archive; destination; prefix; budget |]
        | [| "--extract"; archive; destination; prefix; budget; _ |] ->
            let label = defaultArg (Array.tryItem 5 arguments) ""
            let count, written = extract archive destination prefix (Int64.Parse budget) label
            printfn "%d %d" count written
            0
        | [||] ->
            let output = env "GITHUB_OUTPUT"
            File.AppendAllText(output, "binlog-found=false\n")
            let directory = env "BINLOG_DIR"

            ensure (directory <> "" && not (Path.Exists directory)) "BINLOG_DIR is unset or already exists"
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
