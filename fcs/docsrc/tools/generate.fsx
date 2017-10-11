// --------------------------------------------------------------------------------------
// Builds the documentation from `.fsx` and `.md` files in the 'docsrc/content' directory
// (the generated documentation is stored in the 'docs' directory)
// --------------------------------------------------------------------------------------

// Binaries that have XML documentation (in a corresponding generated XML file)
let referenceBinaries = [ "FSharp.Compiler.Service.dll" ] 
// Web site location for the generated documentation
let website = "https://fsharp.github.io/FSharp.Compiler.Service"

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

#load "../../packages/FSharp.Formatting/FSharp.Formatting.fsx"
#I "../../packages/FAKE/tools"
#r "../../packages/FAKE/tools/FakeLib.dll"
open Fake
open System.IO
open Fake.FileHelper
open FSharp.Literate
open FSharp.MetadataFormat

let root = "."

// Paths with template/source/output locations
let bin         = __SOURCE_DIRECTORY__ @@ "../../../Release/fcs/net45"
let content     = __SOURCE_DIRECTORY__ @@ "../content"
let output      = __SOURCE_DIRECTORY__ @@ "../../../docs"
let files       = __SOURCE_DIRECTORY__ @@ "../files"
let templates   = __SOURCE_DIRECTORY__ @@ "templates"
let formatting  = __SOURCE_DIRECTORY__ @@ "../../packages/FSharp.Formatting/"
let docTemplate = formatting @@ "templates/docpage.cshtml"

// Where to look for *.csproj templates (in this order)
let layoutRoots =
  [ templates; 
    formatting @@ "templates"
    formatting @@ "templates/reference" ]

// Copy static files and CSS + JS from F# Formatting
let copyFiles () =
  CopyRecursive files output true |> Log "Copying file: "
  ensureDirectory (output @@ "content")
  CopyRecursive (formatting @@ "styles") (output @@ "content") true 
    |> Log "Copying styles and scripts: "

let clr =  System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()
let fsfmt =  __SOURCE_DIRECTORY__ @@ ".." @@ ".." @@ @"packages" @@ "FSharp.Formatting" @@ "lib" @@ "net40"

// Build API reference from XML comments
let buildReference () =
  CleanDir (output @@ "reference")
  for lib in referenceBinaries do
    MetadataFormat.Generate
      ( bin @@ lib, output @@ "reference", layoutRoots, 
        parameters = ("root", root)::info,
        sourceRepo = "https://github.com/fsharp/FSharp.Compiler.Service/tree/master/src",
        sourceFolder = @"..\..\..\src",
        assemblyReferences =
             [clr @@ "System.Runtime.dll"
              clr @@ "System.dll"
              clr @@ "System.Core.dll"
              clr @@ "Microsoft.CSharp.dll"
              clr @@ "System.Linq.dll"
              clr @@ "System.dll"
              bin @@ "System.Reflection.Metadata.dll"
              clr @@ "System.Numerics.dll"
              bin @@ "System.Collections.Immutable.dll"
              clr @@ "System.IO.dll"
              clr @@ "mscorlib.dll"
              fsfmt @@ "FSharp.MetadataFormat.dll"
              fsfmt @@ "RazorEngine.dll"
              bin @@ "FSharp.Core.dll"
              bin @@ "FSharp.Compiler.Service.dll"
             ] )

// Build documentation from `fsx` and `md` files in `docsrc/content`
let buildDocumentation () =
  for dir in [content] do
    let sub = if dir.Length > content.Length then dir.Substring(content.Length + 1) else "."
    Literate.ProcessDirectory
      ( dir, docTemplate, output @@ sub, replacements = ("root", root)::info,
        layoutRoots = layoutRoots, generateAnchors = true, processRecursive=false )

// Generate
copyFiles()
buildDocumentation()
buildReference()

