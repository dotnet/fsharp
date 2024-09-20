// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests.ProjectSystem

open System
open System.Collections.Generic
open System.IO
open System.Reflection

open Xunit
open UnitTests.TestLib.Utils.Asserts
open UnitTests.TestLib.Utils.FilesystemHelpers
open UnitTests.TestLib.ProjectSystem

open Microsoft.VisualStudio.FSharp.ProjectSystem
open Microsoft.VisualStudio.Shell.Interop
open System.Xml.Linq

type References() = 
    inherit TheTests()

    //TODO: look for a way to remove the helper functions
    static let currentFrameworkDirectory = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()

    /////////////////////////////////
    // project helpers
    static let SaveProject(project : UnitTestingFSharpProjectNode) =
        project.Save(null, 1, 0u) |> ignore

    static let GetReferenceContainerNode(project : ProjectNode) =
        let l = new List<ReferenceContainerNode>()
        project.FindNodesOfType(l)
        l.[0]


    /// Create a dummy project named 'Test', build it, and then call k with the full path to the resulting exe
    member this.CreateDummyTestProjectBuildItAndDo(k : string -> unit) =
        this.MakeProjectAndDo(["foo.fs"], [], "", (fun project ->
        // Let's create a run-of-the-mill project just to have a spare assembly around
        let fooPath = Path.Combine(project.ProjectFolder, "foo.fs")
        File.AppendAllText(fooPath, "namespace Foo\nmodule Bar =\n  let x = 42")
        let buildResult = project.Build("Build")
        Assert.True buildResult.IsSuccessful
        let exe = Path.Combine(project.ProjectFolder, "bin\\Debug\\Test.exe")
        k exe))

    [<Fact>]
    member this.``BasicAssemblyReferences1``() =
        this.MakeProjectAndDo([], ["System"], "", (fun proj ->
            let systemRef = proj.FirstChild.FirstChild :?> AssemblyReferenceNode
            Assert.True(systemRef.CanShowDefaultIcon())
        ))

    [<Fact>]
    member this.``BasicAssemblyReferences2``() =
        this.MakeProjectAndDo([], ["System.Net"], "", (fun proj ->
            let systemRef = proj.FirstChild.FirstChild :?> AssemblyReferenceNode
            Assert.True(systemRef.CanShowDefaultIcon())
        ))
            
    [<Fact>]
    member this.``AddReference.StarredAssemblyName`` () = 
        DoWithTempFile "Test.fsproj" (fun projFile ->
            File.AppendAllText(projFile, TheTests.SimpleFsprojText([], [], ""))
            use project = TheTests.CreateProject(projFile) 
            let assemblyName = new AssemblyName(typeof<System.Windows.Forms.Form>.Assembly.FullName)
            let selectorData = new VSCOMPONENTSELECTORDATA(``type`` = VSCOMPONENTTYPE.VSCOMPONENTTYPE_ComPlus, bstrFile = "*" + assemblyName.FullName)
            let refContainer = GetReferenceContainerNode(project)
            refContainer.AddReferenceFromSelectorData(selectorData) |> Assert.NotNull
            let l = new List<AssemblyReferenceNode>()
            project.FindNodesOfType(l)
            Assert.Equal(1, l.Count)
            Assert.Equal("System.Windows.Forms", l.[0].Caption)            
            SaveProject(project)
            let fsprojFileText = File.ReadAllText(project.FileName)
            printfn "%s" fsprojFileText
            let expectedFsprojRegex = @"<Reference Include=""System.Windows.Forms"" />"
            TheTests.HelpfulAssertMatches '<' expectedFsprojRegex fsprojFileText
            )

    [<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
    member this.``References.Bug787899.AddDuplicateUnresolved``() =
        // Let's create a run-of-the-mill project just to have a spare assembly around
        this.CreateDummyTestProjectBuildItAndDo(fun exe ->
            Assert.True(File.Exists exe, "failed to build exe")
            this.MakeProjectAndDoWithProjectFile(["doesNotMatter.fs"], ["mscorlib"; "System"; "System.Core"; "System.Net"], 
                                                    "<ItemGroup><Reference Include=\"Test\"><HintPath>.\\Test.dll</HintPath></Reference></ItemGroup>", "v4.0", (fun project file ->
                let assemRef = TheTests.FindNodeWithCaption(project, "Test") :?> AssemblyReferenceNode
                Assert.False(assemRef.CanShowDefaultIcon(), "reference should be banged out, does not resolve")
                // add reference to Test.exe
                let selectorData = new VSCOMPONENTSELECTORDATA(``type`` = VSCOMPONENTTYPE.VSCOMPONENTTYPE_File, bstrFile = exe)
                let refContainer = GetReferenceContainerNode(project)
                refContainer.AddReferenceFromSelectorData(selectorData) |> (fun x -> Assert.NotNull(x))
                // it should have succeeded (no throw)
                ))
            )

    [<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
    member this.``References.Bug787899.AddDuplicateResolved``() =
        // Let's create a run-of-the-mill project just to have a spare assembly around
        this.CreateDummyTestProjectBuildItAndDo(fun exe ->
            Assert.True(File.Exists exe, "failed to build exe")
            this.MakeProjectAndDoWithProjectFile(["doesNotMatter.fs"], ["mscorlib"; "System"; "System.Core"; "System.Net"], 
                                                    sprintf "<ItemGroup><Reference Include=\"Test\"><HintPath>%s</HintPath></Reference></ItemGroup>" exe, "v4.0", (fun project file ->
                let assemRef = TheTests.FindNodeWithCaption(project, "Test") :?> AssemblyReferenceNode
                Assert.True(assemRef.CanShowDefaultIcon(), "reference should not be banged out, does resolve")
                // add reference to Test.exe
                let selectorData = new VSCOMPONENTSELECTORDATA(``type`` = VSCOMPONENTTYPE.VSCOMPONENTTYPE_File, bstrFile = exe)
                let refContainer = GetReferenceContainerNode(project)
                try
                    refContainer.AddReferenceFromSelectorData(selectorData) |> ignore
                    Assert.Fail("expected AddReference to Fail")
                with :? InvalidOperationException as e ->
                    Assert.Equal("A reference to 'Test' (with assembly name 'Test') could not be added. A reference to the component 'Test' with the same assembly name already exists in the project.", e.Message)
                ))
            )

    [<Fact>]
    member this.``ReferenceResolution.Bug4423.LoadedFsProj.Works``() =
        this.MakeProjectAndDo(["doesNotMatter.fs"], ["mscorlib"; "System"; "System.Core"; "System.Net"], "", "v4.0", (fun project ->
            let expectedRefInfo = [ "mscorlib", true
                                    "System", true
                                    "System.Core", true
                                    "System.Net", true ]
            let refContainer = GetReferenceContainerNode(project)
            let actualRefInfo = [
                let mutable r = (refContainer.FirstChild :?> ReferenceNode)
                while r <> null do
                    yield (r.Caption, (r.CanShowDefaultIcon()))
                    r <- r.NextSibling :?> ReferenceNode
                ]
            AssertEqual expectedRefInfo actualRefInfo
            ))


    [<Fact>]
    member this.``ReferenceResolution.Bug4423.LoadedFsProj.WithExactDuplicates``() =
        this.MakeProjectAndDo(["doesNotMatter.fs"], ["System"; "System"], "", "v4.0", (fun project ->
            let expectedRefInfo = [ "System", true  // In C#, one will be banged out, whereas
                                    "System", true] // one will be ok, but in F# both show up as ok.  Bug?  Not worth the effort to fix.
            let refContainer = GetReferenceContainerNode(project)
            let actualRefInfo = [
                let mutable r = (refContainer.FirstChild :?> ReferenceNode)
                while r <> null do
                    yield (r.Caption, (r.CanShowDefaultIcon()))
                    r <- r.NextSibling :?> ReferenceNode
                ]
            AssertEqual expectedRefInfo actualRefInfo
            ))

    [<Fact>]
    member this.``ReferenceResolution.Bug4423.LoadedFsProj.WithBadDuplicates``() =
        this.MakeProjectAndDo(["doesNotMatter.fs"], ["System"; "System.dll"], "", "v4.0", (fun project ->
            let expectedRefInfo = [ "System", false     // one will be banged out
                                    "System.dll", true] // one will be ok
            let refContainer = GetReferenceContainerNode(project)
            let actualRefInfo = [
                let mutable r = (refContainer.FirstChild :?> ReferenceNode)
                while r <> null do
                    yield (r.Caption, (r.CanShowDefaultIcon()))
                    r <- (r.NextSibling :?> ReferenceNode)
                ]
            AssertEqual expectedRefInfo actualRefInfo
            ))

    [<Fact>]
    member this.``ReferenceResolution.Bug4423.LoadedFsProj.WorksWithFilenames``() =
            let netDir = currentFrameworkDirectory
            let ssmw = Path.Combine(netDir, "System.ServiceModel.Web.dll")
            this.MakeProjectAndDo(["doesNotMatter.fs"], [ssmw], "", "v4.0", (fun project ->
            let expectedRefInfo = [ ssmw, true ]
            let refContainer = GetReferenceContainerNode(project)
            let actualRefInfo = [
              let mutable r = (refContainer.FirstChild :?> ReferenceNode)
              while r <> null do
                  yield (r.Caption, (r.CanShowDefaultIcon()))
                  r <- r.NextSibling :?> ReferenceNode
              ]
            AssertEqual expectedRefInfo actualRefInfo
            ))

    [<Fact>]
    member this.``ReferenceResolution.Bug4423.LoadedFsProj.WeirdCases``() =
        this.MakeProjectAndDo(["doesNotMatter.fs"], ["mscorlib, Version=4.0.0.0"; "System, Version=4.0.0.0"; "System.Core, Version=4.0.0.0"; "System.Net, Version=4.0.0.0"], "", "v4.0", (fun project ->
            let expectedRefInfo = [ "mscorlib", true
                                    "System", true
                                    "System.Core, Version=4.0.0.0", false // msbuild does funny things for System.Core (TODO bug number)
                                    "System.Net", true ]
            let refContainer = GetReferenceContainerNode(project)
            let actualRefInfo = [
                let mutable r = (refContainer.FirstChild :?> ReferenceNode)
                while r <> null do
                    yield (r.Caption, (r.CanShowDefaultIcon()))
                    r <- r.NextSibling :?> ReferenceNode
                ]
            AssertEqual expectedRefInfo actualRefInfo
            ))

    member this.ReferenceResolutionHelper(tab : AddReferenceDialogTab, fullPath : string, expectedFsprojRegex : string) =
        this.ReferenceResolutionHelper(tab, fullPath, expectedFsprojRegex, "v4.0", [])
        
    member this.ReferenceResolutionHelper(tab : AddReferenceDialogTab, fullPath : string, expectedFsprojRegex : string, targetFrameworkVersion : string, originalReferences : string list) =
        // Trace.Log <- "ProjectSystemReferenceResolution" // can be useful
        this.MakeProjectAndDo(["doesNotMatter.fs"], originalReferences, "", targetFrameworkVersion, (fun project ->
            let cType = 
                match tab with
                | AddReferenceDialogTab.DotNetTab -> VSCOMPONENTTYPE.VSCOMPONENTTYPE_ComPlus
                | AddReferenceDialogTab.BrowseTab -> VSCOMPONENTTYPE.VSCOMPONENTTYPE_File
                | _ -> failwith "unexpected"
            let selectorData = new VSCOMPONENTSELECTORDATA(``type`` = cType, bstrFile = fullPath)
            let refContainer = GetReferenceContainerNode(project)
            refContainer.AddReferenceFromSelectorData(selectorData) |> Assert.NotNull
            SaveProject(project)
            let fsprojFileText = File.ReadAllText(project.FileName)
            printfn "%s" fsprojFileText
            TheTests.HelpfulAssertMatches '<' expectedFsprojRegex fsprojFileText
            ))

    [<Fact>]
    member this.``ReferenceResolution.Bug4423.FxAssembly.NetTab.AddDuplicate1``() =
        let netDir = currentFrameworkDirectory
        try
            this.ReferenceResolutionHelper(AddReferenceDialogTab.DotNetTab, 
                                           Path.Combine(netDir, "System.ServiceModel.Web.dll"), 
                                           @"whatever, expectation does not matter, will throw before then",
                                           "v4.0",
                                           ["System.ServiceModel.Web"])  // assembly name
            Assert.Fail("adding a duplicate reference should have failed")
        with e ->                                           
            TheTests.HelpfulAssertMatches ' ' "A reference to '.*' \\(with assembly name '.*'\\) could not be added. A reference to the component '.*' with the same assembly name already exists in the project." e.Message

// see 5491 [<Fact>]
    member this.``ReferenceResolution.Bug4423.FxAssembly.NetTab.AddDuplicate2``() =
        let netDir = currentFrameworkDirectory
        try
            this.ReferenceResolutionHelper(AddReferenceDialogTab.DotNetTab, 
                                           Path.Combine(netDir, "System.ServiceModel.Web.dll"), 
                                           @"whatever, expectation does not matter, will throw before then",
                                           "v4.0",
                                           ["System.ServiceModel.Web.dll"]) // filename
            Assert.Fail("adding a duplicate reference should have failed")
        with e ->
            TheTests.HelpfulAssertMatches ' ' "A reference to '.*' could not be added. A reference to the component '.*' already exists in the project." e.Message

    [<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
    member this.``ReferenceResolution.Bug4423.NonFxAssembly.BrowseTab.RelativeHintPath.InsideProjectDir``() =
        // Let's create a run-of-the-mill project just to have a spare assembly around
        this.CreateDummyTestProjectBuildItAndDo(fun exe ->
            Assert.True(File.Exists exe, "failed to build exe")
            // Now let's create an assembly reference to it and ensure we get expected relative HintPath
            let expectedFsprojRegex = @"<Reference Include=""Test"">"
                                         + @"\s*<HintPath>Test.exe</HintPath>"  // in this directory
                                         + @"\s*</Reference>"
            this.MakeProjectAndDo(["bar.fs"], [], "", "v4.5", (fun project ->
                let exeCopy = Path.Combine(project.ProjectFolder, "Test.exe")
                File.Copy(exe, exeCopy, true)                
                Assert.True(File.Exists exeCopy, "failed to build exe")
                let selectorData = new VSCOMPONENTSELECTORDATA(``type`` = VSCOMPONENTTYPE.VSCOMPONENTTYPE_File, bstrFile = exeCopy)
                let refContainer = GetReferenceContainerNode(project)
                refContainer.AddReferenceFromSelectorData(selectorData) |> Assert.NotNull
                SaveProject(project)
                let fsprojFileText = File.ReadAllText(project.FileName)
                printfn "%s" fsprojFileText
                TheTests.HelpfulAssertMatches '<' expectedFsprojRegex fsprojFileText
                // Finally, ensure that the reference works as expected
                project.Reload()
                let assemRef = TheTests.FindNodeWithCaption(project, "Test") :?> AssemblyReferenceNode
                Assert.True(assemRef.CanShowDefaultIcon(), "the reference could not be resolved")  
                // Use the referenced DLL as a double-check
                let barPath = Path.Combine(project.ProjectFolder, "bar.fs")
                File.AppendAllText(barPath, "printfn \"%d\" Foo.Bar.x")  // code that requires the referenced assembly to successfully compile
                let buildResult = project.Build("Build")
                Assert.True buildResult.IsSuccessful
                ))
        )

    [<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
    member this.``ReferenceResolution.Bug4423.NonFxAssembly.BrowseTab.RelativeHintPath.OutsideProjectDir``() =
        this.MakeProjectAndDo(["foo.fs"], [], "", (fun project ->
            // Let's create a run-of-the-mill 
            let fooPath = Path.Combine(project.ProjectFolder, "foo.fs")
            File.AppendAllText(fooPath, "namespace Foo\nmodule Bar =\n  let x = 42")
            let buildResult = project.Build("Build")
            Assert.True buildResult.IsSuccessful
            let exe = Path.Combine(project.ProjectFolder, "bin\\Debug\\Test.exe")
            Assert.True(File.Exists exe, "failed to build exe")
            // Now let's create an assembly reference to it and ensure we get expected relative HintPath
            let expectedFsprojRegex = @"<Reference Include=""Test"">"
                                         + @"\s*<HintPath>\.\.\\.*?</HintPath>"  // the point is, some path start with "..\", since both projects are rooted somewhere in the temp directory (where unit tests create them)
                                         + @"\s*</Reference>"
            this.MakeProjectAndDo(["bar.fs"], [], "", "v4.5", (fun project ->
                let selectorData = new VSCOMPONENTSELECTORDATA(``type`` = VSCOMPONENTTYPE.VSCOMPONENTTYPE_File, bstrFile = exe)
                let refContainer = GetReferenceContainerNode(project)
                refContainer.AddReferenceFromSelectorData(selectorData) |> Assert.NotNull
                SaveProject(project)
                let fsprojFileText = File.ReadAllText(project.FileName)
                printfn "%s" fsprojFileText
                TheTests.HelpfulAssertMatches '<' expectedFsprojRegex fsprojFileText
                // Finally, ensure that the reference works as expected
                project.Reload()
                let assemRef = TheTests.FindNodeWithCaption(project, "Test") :?> AssemblyReferenceNode
                Assert.True(assemRef.CanShowDefaultIcon(), "the reference could not be resolved")  
                // Use the referenced DLL as a double-check
                let barPath = Path.Combine(project.ProjectFolder, "bar.fs")
                File.AppendAllText(barPath, "printfn \"%d\" Foo.Bar.x")  // code that requires the referenced assembly to successfully compile
                let buildResult = project.Build("Build")
                Assert.True buildResult.IsSuccessful
                ))
        ))

    [<Fact>]
    member this.``ReferenceResolution.Bug4423.NotAValidDll.BrowseTab``() =
        let dirName = Path.GetTempPath()
        let dll = Path.Combine(dirName, "Foo.dll")
        File.AppendAllText(dll, "This is not actually a valid dll")
        try
            this.MakeProjectAndDo(["doesNotMatter.fs"], [], "", "v4.0", (fun project ->
                let selectorData = new VSCOMPONENTSELECTORDATA(``type`` = VSCOMPONENTTYPE.VSCOMPONENTTYPE_File, bstrFile = dll)
                let refContainer = GetReferenceContainerNode(project)
                try
                    refContainer.AddReferenceFromSelectorData(selectorData) |> Assert.NotNull
                    Assert.Fail("this should not have succeeded")
                with e ->
                    AssertContains e.Message "could not be added. Please make sure that the file is accessible, and that it is a valid assembly or COM component."
            ))
        finally
            File.Delete(dll)

    [<Fact>]
    member this.``PathReferences.Existing`` () =
        DoWithTempFile "Test.fsproj"(fun projFile ->
            let dirName = Path.GetDirectoryName(projFile)
            let libDirName = Directory.CreateDirectory(Path.Combine(dirName, "lib")).FullName
            let codeBase = (new Uri(Assembly.GetExecutingAssembly().EscapedCodeBase)).LocalPath |> Path.GetDirectoryName
            let refLibPath = Path.Combine(libDirName, "xunit.core.dll")
            File.Copy(Path.Combine(codeBase, "xunit.core.dll"), refLibPath)
            File.AppendAllText(projFile, TheTests.SimpleFsprojText([], [refLibPath], ""))
            use project = TheTests.CreateProject(projFile) 
            let l = new List<AssemblyReferenceNode>()
            project.FindNodesOfType(l)
            AssertEqual 1 l.Count
            AssertEqual refLibPath l.[0].Url
            AssertEqual refLibPath l.[0].Caption  // when Include is a filename, entirety is caption
            Assert.NotNull(l.[0].ResolvedAssembly)
            let refContainer =
                let l = new List<ReferenceContainerNode>()
                project.FindNodesOfType(l)
                l.[0]
            let mscorlibPath = (new Uri("".GetType().Assembly.EscapedCodeBase)).LocalPath
            let selectorData = new VSCOMPONENTSELECTORDATA(``type`` = VSCOMPONENTTYPE.VSCOMPONENTTYPE_ComPlus, bstrFile = mscorlibPath)
            refContainer.AddReferenceFromSelectorData(selectorData) |> Assert.NotNull
            let l = new List<AssemblyReferenceNode>()
            project.FindNodesOfType(l)
            AssertEqual 2 l.Count
            AssertEqual refLibPath l.[0].Url
            AssertEqual refLibPath l.[0].Caption
            AssertEqual "mscorlib" l.[1].Caption
        )

    [<Fact>]
    member this.``PathReferences.Existing.Captions`` () =
        DoWithTempFile "Test.fsproj"(fun projFile ->
            File.AppendAllText(projFile, TheTests.FsprojTextWithProjectReferences(
                [], // <Compile>
                ["$(LetterS)ystem.dll"; "System.Net.dll"], // <Reference>
                [], // <ProjectReference>
                "<PropertyGroup><LetterS>S</LetterS></PropertyGroup>"))  // other stuff
            use project = TheTests.CreateProject(projFile) 
            let l = new List<AssemblyReferenceNode>()
            project.FindNodesOfType(l)
            AssertEqual 2 l.Count
            AssertEqual "System.dll" l.[0].Caption
            Assert.NotNull(l.[0].ResolvedAssembly)
            AssertEqual "System.Net.dll" l.[1].Caption
            Assert.NotNull(l.[1].ResolvedAssembly)
        )
        
    [<Fact>]
    member this.``PathReferences.NonExistent`` () =
        DoWithTempFile "Test.fsproj"(fun projFile ->
            let refLibPath = @"c:\foo\baz\blahblah.dll"
            File.AppendAllText(projFile, TheTests.SimpleFsprojText([], [refLibPath], ""))
            use project = TheTests.CreateProject(projFile) 
            let l = new List<AssemblyReferenceNode>()
            project.FindNodesOfType(l)
            AssertEqual 1 l.Count
            AssertEqual refLibPath l.[0].Caption
            Assert.Null(l.[0].ResolvedAssembly)
        )

        
    [<Fact>]
    member this.``FsprojPreferencePage.ProjSupportsPrefReadWrite``() =
        let testProp = "AssemblyName"
        let compileItem = [@"foo.fs"]
        
        DoWithTempFile "Test.fsproj" (fun projFile ->
            File.AppendAllText(projFile, TheTests.SimpleFsprojText(compileItem, [], "")) 
            use project = TheTests.CreateProject(projFile) 
            // Read a known property from the project node - AssemblyName
            let propertyVal = project.GetProjectProperty(testProp, false)
            // Set the project property to something different (is currently "MyAssembly")
            let newPropVal = "Foo_PROPVAL_Foo" // hopefully unique?
            project.SetProjectProperty(testProp, newPropVal)
            // get the (hopefully) modified property name
            let propertyVal' = project.GetProjectProperty(testProp, false)
            let newProjFileName = (Path.GetDirectoryName projFile) + "\\" + "fooProj.fsproj"
            
            printfn "%s before modification: %s" testProp propertyVal 
            printfn "%s after modification:  %s" testProp propertyVal' 
            
            // Assert that the value has changed
            AssertNotEqual propertyVal propertyVal'
            // Assert that the new value is what we expect it to be 
            AssertEqual newPropVal propertyVal'
            
            // Save as a new project file
            project.SaveMSBuildProjectFileAs(newProjFileName) ; // cleaned up by parent call to DoWithTempFile
            
            // look for the new property inside of the project file
            let contents = File.ReadAllText(newProjFileName)
            AssertContains contents newPropVal
        )


    // Disabled due to: https://github.com/dotnet/fsharp/issues/1460
    // On DEV 15 Preview 4 the VS IDE Test fails with :
    //     System.InvalidOperationException : Operation is not valid due to the current state of the object.
    // [<Fact>]     // Disabled due to: https://github.com/dotnet/fsharp/issues/1460
    member this.``AddReference.COM`` () = 
        DoWithTempFile "Test.fsproj" (fun projFile ->
            File.AppendAllText(projFile, TheTests.SimpleFsprojText([], [], ""))
            use project = TheTests.CreateProject(projFile)

            let guid = Guid("50a7e9b0-70ef-11d1-b75a-00a0c90564fe")

            let selectorData = VSCOMPONENTSELECTORDATA (
                ``type`` = VSCOMPONENTTYPE.VSCOMPONENTTYPE_Com2,
                guidTypeLibrary = guid,
                wTypeLibraryMinorVersion = 0us,
                wTypeLibraryMajorVersion = 1us,
                bstrTitle = "Microsoft Shell Controls And Automation" )
            let refContainer = GetReferenceContainerNode(project)

            let comReference = refContainer.AddReferenceFromSelectorData(selectorData)

            // check reference node properties
            Assert.NotNull comReference
            Assert.True(comReference :? ComReferenceNode)
            let comRef = comReference :?> ComReferenceNode
            Assert.Equal(1, comRef.MajorVersionNumber)
            Assert.Equal(0, comRef.MinorVersionNumber)
            Assert.Equal(guid, comRef.TypeGuid)
            Assert.Equal("Microsoft Shell Controls And Automation", comRef.Caption)
            let sysDirectory = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86)
            Assert.True(String.Compare(Path.Combine(sysDirectory, "shell32.dll"), comRef.InstalledFilePath, StringComparison.OrdinalIgnoreCase) = 0)

            // check node exists under references
            let l = new List<ComReferenceNode>()
            project.FindNodesOfType(l)

            Assert.Equal(1, l.Count)
            let referenceNode = l.[0]
            Assert.Same(comRef, referenceNode)

            // check saved msbuild item
            SaveProject(project)
            let fsproj = XDocument.Load(project.FileName)
            printfn "%O" fsproj
            let xn s = fsproj.Root.GetDefaultNamespace().GetName(s)
            let comReferencesXml = fsproj.Descendants(xn "COMReference") |> Seq.toList
            
            Assert.Equal(1, comReferencesXml |> List.length)

            let comRefXml = comReferencesXml |> List.head

            Assert.Equal("Microsoft Shell Controls And Automation", comRefXml.Attribute(XName.Get("Include")).Value)
            Assert.Equal(guid, Guid(comRefXml.Element(xn "Guid").Value))
            Assert.Equal("1", comRefXml.Element(xn "VersionMajor").Value)
            Assert.Equal("0", comRefXml.Element(xn "VersionMinor").Value)
            Assert.Equal("0", comRefXml.Element(xn "Lcid").Value)
            )
