open System
open System.IO

type ProjectType = FSharp | CSharp

type ProjectRef =
    {
        Name : string
        Guid : Guid
        RelativePath : string
    }

type BinaryRef =
    {
        Name : string
        RelativePath : string
    }

type Solution =
    {
        Projects : (ProjectRef * ProjectType) list
    }

type Project =
    {
        Name : string
        Guid : Guid
        Files : string list
        References : ProjectRef list
        BinaryReferences : BinaryRef list
    }

let printGuidL (g : Guid) = (g.ToString "B")
let printGuidU (g : Guid) = (printGuidL g).ToUpper ()

let safeWrite path text =
    let dir = (FileInfo path).Directory
    if not <| dir.Exists then dir.Create ()
    File.WriteAllText(path, text)

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Solution =

    let private projectTypeGuid =
        function
        | FSharp -> Guid.Parse "{F2A71F9B-5D33-465A-A702-920D77279786}"
        | CSharp -> Guid.Parse "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"

    let write path (solution : Solution) =

        let projects =
            let printProject (p : ProjectRef, t : ProjectType) =
                sprintf "Project(\"%s\") = \"%s\", \"%s\", \"%s\"\nEndProject"
                    (t |> projectTypeGuid |> printGuidU) p.Name p.RelativePath (p.Guid |> printGuidU)
            solution.Projects |> List.map printProject |> String.concat "\n"

        let platforms =
            let printPlatforms (p : ProjectRef, _) =
                seq {
                    for t in [ "Debug" ; "Release" ] do
                    for k in [ "ActiveCfg" ; "Build.0" ] do
                    yield sprintf "\t\t%s.%s|Any CPU.%s = %s|Any CPU" (p.Guid |> printGuidU) t k t
                }
            solution.Projects |> Seq.collect printPlatforms |> String.concat "\n"

        let text =
            File.ReadAllText(Path.Combine(__SOURCE_DIRECTORY__,@"Templates\sln.template"))
                .Replace("[PROJECTS]", projects)
                .Replace("[PLATFORMS]", platforms)

        safeWrite path text

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module FSharpProject =

    let write path (project : Project) =

        let files =
            let printFileReference = sprintf "    <Compile Include=\"%s\" />"
            project.Files |> List.map printFileReference |> String.concat "\n"

        for f in project.Files do 
            let filePath = Path.Combine(Path.GetDirectoryName(path),f)
            let fileContents = sprintf "module %s\n\nlet x = 1" (Path.GetFileNameWithoutExtension(f))
            safeWrite filePath fileContents

        let references =
            let printProjectReference (r : ProjectRef) =
                sprintf "    <ProjectReference Include=\"%s\">\n      <Name>%s</Name>\n      <Project>%s</Project>\n      <Private>True</Private>\n    </ProjectReference>"
                    r.RelativePath r.Name (r.Guid |> printGuidL)
            project.References |> List.map printProjectReference |> String.concat "\n"

        let binaryReferences =
            let printBinaryReference (r : BinaryRef) =
                sprintf "    <Reference Include=\"%s\">\n      <HintPath>%s</HintPath>\n    </Reference>" r.Name r.RelativePath
            project.BinaryReferences |> List.map printBinaryReference |> String.concat "\n"

        let text =
            File.ReadAllText(Path.Combine(__SOURCE_DIRECTORY__,@"Templates\fsproj.template"))
                .Replace("[NAME]", project.Name)
                .Replace("[GUID]", project.Guid.ToString ())
                .Replace("[FILES]", files)
                .Replace("[REFERENCES]", references)
                .Replace("[BINARYREFERENCES]", binaryReferences)

        safeWrite path text

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module CSharpProject =

    let write path (project : Project) =

        let files =
            let printFileReference = sprintf "    <Compile Include=\"%s\" />"
            project.Files |> List.map printFileReference |> String.concat "\n"

        let references =
            let printProjectReference (r : ProjectRef) =
                sprintf "    <ProjectReference Include=\"%s\">\n      <Project>%s</Project>\n      <Name>%s</Name>\n    </ProjectReference>"
                    r.RelativePath (r.Guid |> printGuidL) r.Name
            project.References |> List.map printProjectReference |> String.concat "\n"

        let binaryReferences =
            let printBinaryReference (r : BinaryRef) =
                sprintf "    <Reference Include=\"%s\">\n      <HintPath>%s</HintPath>\n    </Reference>" r.Name r.RelativePath
            project.BinaryReferences |> List.map printBinaryReference |> String.concat "\n"

        let text =
            File.ReadAllText(Path.Combine(__SOURCE_DIRECTORY__,@"Templates\csproj.template"))
                .Replace("[NAME]", project.Name)
                .Replace("[GUID]", project.Guid |> printGuidU)
                .Replace("[FILES]", files)
                .Replace("[REFERENCES]", references)
                .Replace("[BINARYREFERENCES]", binaryReferences)

        safeWrite path text

module Testing =

    let testWrite () =

        let sol1 =
            [
                { Name = "CSharpExample"  ; Guid = Guid.Parse "{9020FD53-19B2-4D26-84BE-17F318AA8089}" ; RelativePath = "CSharpExample\CSharpExample.csproj"   }, CSharp
                { Name = "CSharpExample2" ; Guid = Guid.Parse "{3F13D629-E4D0-4FB0-A6BC-3A61CAEA4EC9}" ; RelativePath = "CSharpExample2\CSharpExample2.csproj" }, CSharp
            ]
            |> (fun prs -> { Projects = prs })

        let sol2 =
            [
                { Name = "FSharpExample"  ; Guid = Guid.Parse "{58C702E1-6256-499A-AA61-03ABDE4148D4}" ; RelativePath = "FSharpExample\FSharpExample.fsproj"   }, FSharp
                { Name = "FSharpExample2" ; Guid = Guid.Parse "{972203B9-CDA3-45F6-9E55-9A31DFF36722}" ; RelativePath = "FSharpExample2\FSharpExample2.fsproj" }, FSharp
            ]
            |> (fun prs -> { Projects = prs })

        Solution.write @"Test\csharp.sln" sol1
        Solution.write @"Test\fsharp.sln" sol2

        let fsproj =
            let ref1 = { Name = "FSharpExample" ; Guid = Guid.Parse "{58c702e1-6256-499a-aa61-03abde4148d4}" ; RelativePath = "..\FSharpExample\FSharpExample.fsproj" }
            { Name = "FSharpExample2" ; Guid = Guid.Parse "972203b9-cda3-45f6-9e55-9a31dff36722" ; Files = [ "AssemblyInfo.fs" ; "Library1.fs" ] ; References = [ ref1 ] ; BinaryReferences = [] }

        FSharpProject.write @"Test\fsharp.fsproj" fsproj

        let csproj =
            let ref1 = { Name = "CSharpExample" ; Guid = Guid.Parse "{9020fd53-19b2-4d26-84be-17f318aa8089}" ; RelativePath = "..\CSharpExample\CSharpExample.csproj" }
            { Name = "CSharpExample2" ; Guid = Guid.Parse "{3F13D629-E4D0-4FB0-A6BC-3A61CAEA4EC9}" ; Files = [ "Class1.cs" ; @"Properties\AssemblyInfo.cs" ] ; References = [ ref1 ] ; BinaryReferences = [] }

        CSharpProject.write @"Test\csharp.csproj" csproj
