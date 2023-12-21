#i "nuget: https://api.nuget.org/v3/index.json"
#r "nuget: Markdig, 0.33.0"
#r "nuget: FsHttp, 12.1.0"

open System.IO
open System.Xml.Linq
open System.Text.RegularExpressions
open FsHttp

let versionProps = Path.Combine(__SOURCE_DIRECTORY__, "../../../eng/Versions.props")
let versionPropsDoc = XDocument.Load(versionProps)

/// Find all published versions of a package on NuGet
let getAvailableNuGetVersions (packageName: string) : Set<string> =
    let packageName = packageName.ToLowerInvariant()

    http { GET $"https://api.nuget.org/v3-flatcontainer/%s{packageName}/index.json" }
    |> Request.send
    |> Response.deserializeJson<{| versions: string array |}>
    |> fun json -> Set.ofArray json.versions

/// Try and find the publish date on NuGet
let tryGetReleaseDate (packageName: string) (version: string) : string option =
    let packageName = packageName.ToLowerInvariant()

    http { GET $"https://api.nuget.org/v3/registration5-gz-semver2/%s{packageName}/%s{version}.json" }
    |> Request.send
    |> Response.deserializeJson<{| published: string |}>
    |> fun json ->
        if System.String.IsNullOrWhiteSpace json.published then
            None
        else
            Some(json.published.Split('T').[0])

/// In order for the heading to appear in the page content menu in fsdocs,
/// they need to follow a specific HTML structure.
let transformH3 (version: string) (input: string) : string =
    let pattern = "<h3>(.*?)</h3>"

    let replacement =
        $"<h3><a name=\"%s{version}-$1\" class=\"anchor\" href=\"#%s{version}-$1\">$1</a></h3>"

    Regex.Replace(input, pattern, replacement)

/// Process all MarkDown files from the given release folder
let processFolder (path: string) (processFile: string -> string) : string =
    Directory.EnumerateFiles(path, "*.md")
    |> Seq.sortByDescending Path.GetFileNameWithoutExtension
    |> Seq.map processFile
    |> String.concat "\n"
