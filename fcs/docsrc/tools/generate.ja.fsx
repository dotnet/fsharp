// --------------------------------------------------------------------------------------
// Builds the documentation from `.fsx` and `.md` files in the 'docsrc/content' directory
// (the generated documentation is stored in the 'docs' directory)
// --------------------------------------------------------------------------------------

// Binaries that have XML documentation (in a corresponding generated XML file)
let referenceBinaries = [ "FSharp.Compiler.Service.dll" ] 
// Web site location for the generated documentation
let website = "/FSharp.Compiler.Service/ja"

// Specify more information about your project
let info =
  [ "project-name", "F# Compiler Services"
    "project-author", "Microsoft Corporation, Dave Thomas, Anh-Dung Phan, Tomas Petricek"
    "project-summary", "F# compiler services for creating IDE tools, language extensions and for F# embedding"
    "project-github", "http://github.com/fsharp/FSharp.Compiler.Service"
    "project-nuget", "https://www.nuget.org/packages/FSharp.Compiler.Service" ]

// --------------------------------------------------------------------------------------
// For typical project, no changes are needed below
// --------------------------------------------------------------------------------------

#I "../../packages/FSharpVSPowerTools.Core/lib/net45"
#I "../../packages/FSharp.Formatting/lib/net40"
#I "../../packages/FSharp.Compiler.Service/lib/net45"
#I "../../packages/FAKE/tools"
#r "FSharpVSPowerTools.Core.dll"
#r "System.Web.Razor.dll"
#r "FakeLib.dll"
#r "FSharp.Compiler.Service.dll"
#r "RazorEngine.dll"
#r "FSharp.Literate.dll"
#r "FSharp.CodeFormat.dll"
#r "FSharp.MetadataFormat.dll"
open Fake
open System.IO
open Fake.FileHelper
open FSharp.Literate
open FSharp.MetadataFormat

// When called from 'build.fsx', use the public project URL as <root>
// otherwise, use the current 'output' directory.
#if RELEASE
let root = website
#else
let root = "file://" + (__SOURCE_DIRECTORY__ @@ "../output/ja")
#endif

// Paths with template/source/output locations
let bin         = __SOURCE_DIRECTORY__ @@ "../../../Release/fcs/net45"
let content     = __SOURCE_DIRECTORY__ @@ "../content/ja"
let output      = __SOURCE_DIRECTORY__ @@ "../../docs"
let outputJa    = __SOURCE_DIRECTORY__ @@ "../../docs/ja"
let files       = __SOURCE_DIRECTORY__ @@ "../files"
let templates   = __SOURCE_DIRECTORY__ @@ "templates/ja"
let reference   = __SOURCE_DIRECTORY__ @@ "reference"
let formatting  = __SOURCE_DIRECTORY__ @@ "../../packages/FSharp.Formatting/"
let docTemplate = formatting @@ "templates/docpage.cshtml"

// Where to look for *.csproj templates (in this order)
let layoutRoots =
  [ templates
    reference
    formatting @@ "templates"
    formatting @@ "templates/reference" ]

// Copy static files and CSS + JS from F# Formatting
// Build documentation from `fsx` and `md` files in `docsrc/content`
let buildDocumentation () =
  let subdirs = Directory.EnumerateDirectories(content, "*", SearchOption.AllDirectories)
                |> Seq.filter (fun x -> x.Contains "ja")
  for dir in Seq.append [content] subdirs do
    let sub = if dir.Length > content.Length then dir.Substring(content.Length + 1) else "."
    Literate.ProcessDirectory
      ( dir, docTemplate, outputJa @@ sub, replacements = ("root", root)::info,
        layoutRoots = layoutRoots, generateAnchors = true )

// Generate
buildDocumentation()
