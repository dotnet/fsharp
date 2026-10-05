#i "nuget: https://api.nuget.org/v3/index.json"
#r "nuget: Markdig, 1.3.2"
#r "nuget: FsHttp, 15.0.3"

open System.IO
open System.Xml.Linq
open System.Text.RegularExpressions
open Markdig
open FsHttp

// FsHttp logs every request to the console in FSI, which is noise in the docs build.
Fsi.disableDebugLogs ()

let versionProps = Path.Combine(__SOURCE_DIRECTORY__, "../../../eng/Versions.props")
let versionPropsDoc = XDocument.Load(versionProps)

/// Find all published versions of a package on NuGet
let getAvailableNuGetVersions (packageName: string) : Set<string> =
    let packageName = packageName.ToLowerInvariant()

    http { GET $"https://api.nuget.org/v3-flatcontainer/%s{packageName}/index.json" }
    |> Request.send
    |> Response.deserializeJson<{| versions: string array |}>
    |> fun json -> Set.ofArray json.versions

/// How a version of a package stands on NuGet
type NuGetRelease =
    /// The package version does not exist on NuGet
    | Unreleased
    /// The package version exists but the owner unlisted it. NuGet reports 1900-01-01 as its
    /// publish date, so the date is meaningless.
    | Unlisted
    /// Published on the given date (yyyy-MM-dd)
    | Published of date: string

/// Find out whether a version of a package is published on NuGet, and when
let getRelease (packageName: string) (availableVersions: Set<string>) (version: string) : NuGetRelease =
    if not (availableVersions.Contains version) then
        Unreleased
    else
        let packageName = packageName.ToLowerInvariant()

        http { GET $"https://api.nuget.org/v3/registration5-gz-semver2/%s{packageName}/%s{version}.json" }
        |> Request.send
        |> Response.deserializeJson<{| published: string; listed: bool |}>
        |> fun json ->
            if not json.listed then Unlisted
            elif System.String.IsNullOrWhiteSpace json.published then Unreleased
            else Published(json.published.Split('T').[0])

/// The heading for a version: the version and its release date or status
let releaseTitle (version: string) (release: NuGetRelease) : string =
    match release with
    | Unreleased -> $"%s{version} - Not on NuGet"
    | Unlisted -> $"%s{version} - Unlisted on NuGet"
    | Published date -> $"%s{version} - %s{date}"

/// A badge linking to the package version on NuGet
let nugetBadgeLink (packageName: string) (version: string) : string =
    $"<a href=\"https://www.nuget.org/packages/%s{packageName}/%s{version}\" target=\"_blank\"><img alt=\"Nuget\" src=\"https://img.shields.io/badge/NuGet-%s{version}-blue\"></a>"

/// A badge linking to the package version on NuGet, for versions that exist there
let nugetBadge (packageName: string) (version: string) (release: NuGetRelease) : string =
    match release with
    | Unreleased -> System.String.Empty
    | Unlisted
    | Published _ -> nugetBadgeLink packageName version

/// The F# version a package was built from, as FSMajor.FSMinor.FSBuild in eng/Versions.props of
/// the source commit that the nuspec on NuGet records. Packages are built from dotnet/fsharp or,
/// since .NET 10, from the dotnet/dotnet VMR where the repo lives under src/fsharp.
let tryGetSourceFSharpVersion (packageName: string) (version: string) : Async<string option> =
    async {
        try
            let packageName = packageName.ToLowerInvariant()

            let! nuspec =
                http { GET $"https://api.nuget.org/v3-flatcontainer/%s{packageName}/%s{version}/%s{packageName}.nuspec" }
                |> Request.sendAsync

            let nuspec = nuspec |> Response.toText
            let repository = Regex.Match(nuspec, "<repository [^>]*url=\"([^\"]+)\"[^>]*commit=\"([0-9a-f]+)\"")

            if not repository.Success then
                eprintfn "%s %s: no source repository in nuspec" packageName version
                return None
            else
                let url = repository.Groups.[1].Value.TrimEnd('/')
                let commit = repository.Groups.[2].Value

                let versionsProps =
                    if url.EndsWith "dotnet/dotnet" then
                        $"https://raw.githubusercontent.com/dotnet/dotnet/%s{commit}/src/fsharp/eng/Versions.props"
                    else
                        $"https://raw.githubusercontent.com/dotnet/fsharp/%s{commit}/eng/Versions.props"

                let! props = http { GET versionsProps } |> Request.sendAsync
                let doc = XDocument.Parse(props |> Response.toText)
                // The first element wins: FSMinorVersion is redefined further down for FSharp.Core only.
                let value name = (doc.Descendants(XName.Get name) |> Seq.head).Value
                let major = value "FSMajorVersion"
                let minor = value "FSMinorVersion"
                let build = value "FSBuildVersion"
                return Some $"%s{major}.%s{minor}.%s{build}"
        with ex ->
            eprintfn "%s %s: could not determine the source F# version: %s" packageName version ex.Message
            return None
    }

/// The release notes file an F# version belongs to: the highest notes version at or below it in
/// the same hundreds band. So with files 9.0.200 and 9.0.202, F# 9.0.201 belongs to 9.0.200 and
/// F# 9.0.203 to 9.0.202, while 9.0.303 belongs to 9.0.300.
let releaseNotesVersionOf (notesVersions: string seq) (fsharpVersion: string) : string option =
    let version = System.Version.Parse fsharpVersion

    notesVersions
    |> Seq.map System.Version.Parse
    |> Seq.filter (fun notes ->
        notes.Major = version.Major
        && notes.Minor = version.Minor
        && notes.Build / 100 = version.Build / 100
        && notes <= version)
    |> Seq.sortDescending
    |> Seq.tryHead
    |> Option.map string

/// Groups package versions on NuGet by the release notes file they belong to, looking up the
/// F# version each package was built from. Versions within a group are sorted ascending.
let getPackagesByReleaseNotes
    (packageName: string)
    (notesVersions: string seq)
    (versions: string seq)
    : Map<string, string list> =
    let numericPrefix (version: string) =
        System.Version.Parse(version.Split('-').[0])

    versions
    |> Seq.map (fun version ->
        async {
            let! fsharpVersion = tryGetSourceFSharpVersion packageName version
            return fsharpVersion |> Option.bind (releaseNotesVersionOf notesVersions) |> Option.map (fun notes -> notes, version)
        })
    |> fun lookups -> Async.Parallel(lookups, maxDegreeOfParallelism = 8)
    |> Async.RunSynchronously
    |> Seq.choose id
    |> Seq.groupBy fst
    |> Seq.map (fun (notes, packages) ->
        notes, packages |> Seq.map snd |> Seq.sortBy numericPrefix |> List.ofSeq)
    |> Map.ofSeq

/// In order for the heading to appear in the page content menu in fsdocs,
/// they need to follow a specific HTML structure.
let transformH3 (version: string) (input: string) : string =
    let pattern = "<h3>(.*?)</h3>"

    let replacement =
        $"<h3><a name=\"%s{version}-$1\" class=\"anchor\" href=\"#%s{version}-$1\">$1</a></h3>"

    Regex.Replace(input, pattern, replacement)

/// Orders release note file names newest first. Version numbers compare numerically, so that
/// 10.0.100 comes before 9.0.300. Names that are not a version, such as "preview" or "18.vNext",
/// are the upcoming release and go on top.
let compareVersionsDescending (a: string) (b: string) : int =
    let tryParse (name: string) =
        match System.Version.TryParse name with
        | true, version -> Some version
        | _ -> None

    match tryParse a, tryParse b with
    | Some a, Some b -> compare b a
    | Some _, None -> 1
    | None, Some _ -> -1
    | None, None -> compare b a

/// The F# version main is at, and the FCS version that goes with it
let upcomingFSharpVersion, upcomingFcsVersion =
    let value name = (versionPropsDoc.Descendants(XName.Get name) |> Seq.head).Value
    let build = value "FSBuildVersion"
    System.Version.Parse $"""{value "FSMajorVersion"}.{value "FSMinorVersion"}.{build}""",
    System.Version.Parse $"""{value "FCSMajorVersion"}.{value "FCSMinorVersion"}.{build}"""

/// Renders the release notes of a package: one section per notes file in `path`, ordered by
/// package version, newest first. Which package versions belong to a notes file is looked up
/// from the source commit of each package on NuGet, from `minPackageVersion` on.
///
/// Notes without a package are either the upcoming release, placed at the top as `upcomingVersion`,
/// or an old servicing version that never shipped, placed where `packageVersionOfNotes` puts it.
let renderPackageReleaseNotes
    (packageName: string)
    (path: string)
    (minPackageVersion: System.Version)
    (packageVersionOfNotes: System.Version -> System.Version)
    (upcomingVersion: System.Version)
    : string =
    let numericPrefix (version: string) =
        System.Version.Parse(version.Split('-').[0])

    let availableVersions = getAvailableNuGetVersions packageName

    let notesVersions =
        Directory.EnumerateFiles(path, "*.md") |> Seq.map Path.GetFileNameWithoutExtension |> List.ofSeq

    let packagesByReleaseNotes =
        availableVersions
        |> Seq.filter (fun version -> numericPrefix version >= minPackageVersion)
        |> getPackagesByReleaseNotes packageName notesVersions

    notesVersions
    |> List.map (fun notesVersion ->
        let file = Path.Combine(path, notesVersion + ".md")
        let packages = packagesByReleaseNotes.TryFind notesVersion |> Option.defaultValue []
        let stable = packages |> List.filter (fun version -> not (version.Contains '-'))

        // The heading is the first package that shipped for these notes; later ones are servicing
        // rebuilds and get a badge each. Without a stable package, show the latest prerelease.
        let version, title, badges =
            match stable, List.rev packages with
            | first :: _, _ ->
                let release = getRelease packageName availableVersions first
                let badges = stable |> List.map (nugetBadgeLink packageName)
                first, releaseTitle first release, String.concat " " badges
            | [], latestPrerelease :: _ ->
                latestPrerelease, $"%s{latestPrerelease} - Prerelease", nugetBadgeLink packageName latestPrerelease
            | [], [] -> notesVersion, $"F# %s{notesVersion} - Not on NuGet", System.String.Empty

        let sortKey =
            match packages with
            | _ :: _ -> numericPrefix version
            | [] ->
                let notes = System.Version.Parse notesVersion

                if notes >= upcomingFSharpVersion then
                    upcomingVersion
                else
                    packageVersionOfNotes notes

        let content = File.ReadAllText file |> Markdown.ToHtml |> transformH3 version

        sortKey,
        $"""<h2><a name="%s{version}" class="anchor" href="#%s{version}">%s{title}</a></h2>%s{badges}%s{content}""")
    |> List.sortByDescending fst
    |> List.map snd
    |> String.concat "\n"

/// Process all MarkDown files from the given release folder, newest version first
let processFolder (path: string) (processFile: string -> string) : string =
    Directory.EnumerateFiles(path, "*.md")
    |> Seq.sortWith (fun a b ->
        compareVersionsDescending (Path.GetFileNameWithoutExtension a) (Path.GetFileNameWithoutExtension b))
    |> Seq.map processFile
    |> String.concat "\n"
