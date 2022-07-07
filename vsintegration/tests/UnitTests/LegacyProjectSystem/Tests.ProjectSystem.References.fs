// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests.ProjectSystem

open System
open System.Collections.Generic
open System.IO
open System.Reflection

open NUnit.Framework
open Salsa
open UnitTests.TestLib.Utils.Asserts
open UnitTests.TestLib.Utils.FilesystemHelpers
open UnitTests.TestLib.ProjectSystem

open Microsoft.VisualStudio
open Microsoft.VisualStudio.FSharp.ProjectSystem
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.Win32
open System.Xml.Linq

[<TestFixture>][<Category "ProjectSystem">]
type References() = 
    inherit TheTests()

    //TODO: look for a way to remove the helper functions
    static let currentFrameworkDirectory = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()
    static let Net20AssemExPath () =
        let key = @"SOFTWARE\Microsoft\.NETFramework\v2.0.50727\AssemblyFoldersEx\Public Assemblies (Common Files)"
        let hklm = Registry.LocalMachine
        let rkey = hklm.OpenSubKey(key)
        let path = rkey.GetValue("") :?> string
        if String.IsNullOrEmpty(path) then None
        else Some(path)

    /////////////////////////////////
    // project helpers
    static let SaveProject(project : UnitTestingFSharpProjectNode) =
        project.Save(null, 1, 0u) |> ignore

    static let DefaultBuildActionOfFilename(filename) : Salsa.BuildAction = 
        match Path.GetExtension(filename) with 
        | ".fsx" -> Salsa.BuildAction.None
        | ".resx"
        | ".resources" -> Salsa.BuildAction.EmbeddedResource
        | _ -> Salsa.BuildAction.Compile            

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
        Assert.IsTrue buildResult.IsSuccessful
        let exe = Path.Combine(project.ProjectFolder, "bin\\Debug\\Test.exe")
        k exe))

    [<Test>]
    member this.``BasicAssemblyReferences1``() =
        this.MakeProjectAndDo([], ["System"], "", (fun proj ->
            let systemRef = proj.FirstChild.FirstChild :?> AssemblyReferenceNode
            Assert.IsTrue(systemRef.CanShowDefaultIcon())
        ))

    [<Test>]
    member this.``BasicAssemblyReferences2``() =
        this.MakeProjectAndDo([], ["System.Net"], "", (fun proj ->
            let systemRef = proj.FirstChild.FirstChild :?> AssemblyReferenceNode
            Assert.IsTrue(systemRef.CanShowDefaultIcon())
        ))
            
    [<Test>]
    member this.``AddReference.StarredAssemblyName`` () = 
        DoWithTempFile "Test.fsproj" (fun projFile ->
            File.AppendAllText(projFile, TheTests.SimpleFsprojText([], [], ""))
            use project = TheTests.CreateProject(projFile) 
            let assemblyName = new AssemblyName(typeof<System.Windows.Forms.Form>.Assembly.FullName)
            let selectorData = new VSCOMPONENTSELECTORDATA(``type`` = VSCOMPONENTTYPE.VSCOMPONENTTYPE_ComPlus, bstrFile = "*" + assemblyName.FullName)
            let refContainer = GetReferenceContainerNode(project)
            refContainer.AddReferenceFromSelectorData(selectorData) |> Assert.IsNotNull
            let l = new List<AssemblyReferenceNode>()
            project.FindNodesOfType(l)
            Assert.AreEqual(1, l.Count)
            Assert.AreEqual("System.Windows.Forms", l.[0].Caption)            
            SaveProject(project)
            let fsprojFileText = File.ReadAllText(project.FileName)
            printfn "%s" fsprojFileText
            let expectedFsprojRegex = @"<Reference Include=""System.Windows.Forms"" />"
            TheTests.HelpfulAssertMatches '<' expectedFsprojRegex fsprojFileText
            )

    [<Test>]
    member this.``References.Bug787899.AddDuplicateUnresolved``() =
        // Let's create a run-of-the-mill project just to have a spare assembly around
        this.CreateDummyTestProjectBuildItAndDo(fun exe ->
            Assert.IsTrue(File.Exists exe, "failed to build exe")
            this.MakeProjectAndDoWithProjectFile(["doesNotMatter.fs"], ["mscorlib"; "System"; "System.Core"; "System.Net"], 
                                                    "<ItemGroup><Reference Include=\"Test\"><HintPath>.\\Test.dll</HintPath></Reference></ItemGroup>", "v4.0", (fun project file ->
                let assemRef = TheTests.FindNodeWithCaption(project, "Test") :?> AssemblyReferenceNode
                Assert.IsFalse(assemRef.CanShowDefaultIcon(), "reference should be banged out, does not resolve")
                // add reference to Test.exe
                let selectorData = new VSCOMPONENTSELECTORDATA(``type`` = VSCOMPONENTTYPE.VSCOMPONENTTYPE_File, bstrFile = exe)
                let refContainer = GetReferenceContainerNode(project)
                refContainer.AddReferenceFromSelectorData(selectorData) |> (fun x -> Assert.IsNotNull(x, "expected AddReference to succeed"))
                // it should have succeeded (no throw)
                ))
            )

    [<Test>]
    member this.``References.Bug787899.AddDuplicateResolved``() =
        // Let's create a run-of-the-mill project just to have a spare assembly around
        this.CreateDummyTestProjectBuildItAndDo(fun exe ->
            Assert.IsTrue(File.Exists exe, "failed to build exe")
            this.MakeProjectAndDoWithProjectFile(["doesNotMatter.fs"], ["mscorlib"; "System"; "System.Core"; "System.Net"], 
                                                    sprintf "<ItemGroup><Reference Include=\"Test\"><HintPath>%s</HintPath></Reference></ItemGroup>" exe, "v4.0", (fun project file ->
                let assemRef = TheTests.FindNodeWithCaption(project, "Test") :?> AssemblyReferenceNode
                Assert.IsTrue(assemRef.CanShowDefaultIcon(), "reference should not be banged out, does resolve")
                // add reference to Test.exe
                let selectorData = new VSCOMPONENTSELECTORDATA(``type`` = VSCOMPONENTTYPE.VSCOMPONENTTYPE_File, bstrFile = exe)
                let refContainer = GetReferenceContainerNode(project)
                try
                    refContainer.AddReferenceFromSelectorData(selectorData) |> ignore
                    Assert.Fail("expected AddReference to Fail")
                with :? InvalidOperationException as e ->
                    Assert.AreEqual("A reference to 'Test' (with assembly name 'Test') could not be added. A reference to the component 'Test' with the same assembly name already exists in the project.", e.Message)
                ))
            )

    [<Test>]
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


    [<Test>]
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

    [<Test>]
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

    [<Test>]
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

    [<Test>]
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
            refContainer.AddReferenceFromSelectorData(selectorData) |> Assert.IsNotNull
            SaveProject(project)
            let fsprojFileText = File.ReadAllText(project.FileName)
            printfn "%s" fsprojFileText
            TheTests.HelpfulAssertMatches '<' expectedFsprojRegex fsprojFileText
            ))

    [<Test>]
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

// see 5491 [<Test>]
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

(*        
    [<Ignore("Legacy test, NRE trying to get UI thread.")>]
    [<Test>]
    member this.``ReferenceResolution.Bug650591.AutomationReference.Add.FullPath``() = 
        match Net20AssemExPath() with
        | Some(net20) ->
          let invoker = 
              {
                  new Microsoft.Internal.VisualStudio.Shell.Interop.IVsInvokerPrivate with
                      member this.Invoke(invokable) = invokable.Invoke()
              }
          let log = 
              {
                  new Microsoft.VisualStudio.Shell.Interop.IVsActivityLog with
                      member this.LogEntry(_, _, _) = VSConstants.S_OK
                      member this.LogEntryGuid(_, _, _, _) = VSConstants.S_OK
                      member this.LogEntryGuidHr(_, _, _, _, _) = VSConstants.S_OK
                      member this.LogEntryGuidHrPath(_, _, _, _, _, _) = VSConstants.S_OK
                      member this.LogEntryGuidPath(_, _, _, _, _) = VSConstants.S_OK
                      member this.LogEntryHr(_, _, _, _) = VSConstants.S_OK
                      member this.LogEntryHrPath(_, _, _, _, _) = VSConstants.S_OK
                      member this.LogEntryPath(_, _, _, _) = VSConstants.S_OK
              }
          let mocks = 
              [
                  typeof<Microsoft.Internal.VisualStudio.Shell.Interop.SVsUIThreadInvokerPrivate>.GUID, box invoker
                  typeof<Microsoft.VisualStudio.Shell.Interop.SVsActivityLog>.GUID, box log
              ] |> dict
          let mockProvider = 
              {
                  new Microsoft.VisualStudio.OLE.Interop.IServiceProvider with
                      member this.QueryService(guidService, riid, punk) =
                          match mocks.TryGetValue guidService with
                          | true, v -> 
                              punk <- System.Runtime.InteropServices.Marshal.GetIUnknownForObject(v)
                              VSConstants.S_OK
                          | _ ->
                              punk <- IntPtr.Zero
                              VSConstants.E_NOINTERFACE
              }
  
          let _ = Microsoft.VisualStudio.Shell.ServiceProvider.CreateFromSetSite(mockProvider)
          let envDte80RefAssemPath = Path.Combine(net20, "EnvDTE80.dll")
          let dirName = Path.GetTempPath()
          let copy = Path.Combine(dirName, "EnvDTE80.dll")
          try
              File.Copy(envDte80RefAssemPath, copy, true)
              this.MakeProjectAndDo
                  (
                      ["DoesNotMatter.fs"], 
                      [], 
                      "",
                      fun proj -> 
                          let refContainer = GetReferenceContainerNode(proj)
                          let automationRefs = refContainer.Object :?> Automation.OAReferences
                          automationRefs.Add(copy) |> ignore
                          SaveProject(proj)
                          let fsprojFileText = File.ReadAllText(proj.FileName)
                          printfn "%s" fsprojFileText
                          let expectedFsProj = 
                              @"<Reference Include=""EnvDTE80"">"
                              + @"\s*<HintPath>\.\.\\EnvDTE80.dll</HintPath>"
                              + @"\s*</Reference>"
                          TheTests.HelpfulAssertMatches '<' expectedFsProj fsprojFileText
                  )
          finally
              File.Delete(copy)
        | _ -> ()
*)

    [<Test; Category("Expensive")>]
    member this.``ReferenceResolution.Bug4423.NonFxAssembly.BrowseTab.RelativeHintPath.InsideProjectDir``() =
        // Let's create a run-of-the-mill project just to have a spare assembly around
        this.CreateDummyTestProjectBuildItAndDo(fun exe ->
            Assert.IsTrue(File.Exists exe, "failed to build exe")
            // Now let's create an assembly reference to it and ensure we get expected relative HintPath
            let expectedFsprojRegex = @"<Reference Include=""Test"">"
                                         + @"\s*<HintPath>Test.exe</HintPath>"  // in this directory
                                         + @"\s*</Reference>"
            this.MakeProjectAndDo(["bar.fs"], [], "", "v4.5", (fun project ->
                let exeCopy = Path.Combine(project.ProjectFolder, "Test.exe")
                File.Copy(exe, exeCopy, true)                
                Assert.IsTrue(File.Exists exeCopy, "failed to build exe")
                let selectorData = new VSCOMPONENTSELECTORDATA(``type`` = VSCOMPONENTTYPE.VSCOMPONENTTYPE_File, bstrFile = exeCopy)
                let refContainer = GetReferenceContainerNode(project)
                refContainer.AddReferenceFromSelectorData(selectorData) |> Assert.IsNotNull
                SaveProject(project)
                let fsprojFileText = File.ReadAllText(project.FileName)
                printfn "%s" fsprojFileText
                TheTests.HelpfulAssertMatches '<' expectedFsprojRegex fsprojFileText
                // Finally, ensure that the reference works as expected
                project.Reload()
                let assemRef = TheTests.FindNodeWithCaption(project, "Test") :?> AssemblyReferenceNode
                Assert.IsTrue(assemRef.CanShowDefaultIcon(), "the reference could not be resolved")  
                // Use the referenced DLL as a double-check
                let barPath = Path.Combine(project.ProjectFolder, "bar.fs")
                File.AppendAllText(barPath, "printfn \"%d\" Foo.Bar.x")  // code that requires the referenced assembly to successfully compile
                let buildResult = project.Build("Build")
                Assert.IsTrue buildResult.IsSuccessful
                ))
        )

    [<Test>]
    member this.``ReferenceResolution.Bug4423.NonFxAssembly.BrowseTab.RelativeHintPath.OutsideProjectDir``() =
        this.MakeProjectAndDo(["foo.fs"], [], "", (fun project ->
            // Let's create a run-of-the-mill 
            let fooPath = Path.Combine(project.ProjectFolder, "foo.fs")
            File.AppendAllText(fooPath, "namespace Foo\nmodule Bar =\n  let x = 42")
            let buildResult = project.Build("Build")
            Assert.IsTrue buildResult.IsSuccessful
            let exe = Path.Combine(project.ProjectFolder, "bin\\Debug\\Test.exe")
            Assert.IsTrue(File.Exists exe, "failed to build exe")
            // Now let's create an assembly reference to it and ensure we get expected relative HintPath
            let expectedFsprojRegex = @"<Reference Include=""Test"">"
                                         + @"\s*<HintPath>\.\.\\.*?</HintPath>"  // the point is, some path start with "..\", since both projects are rooted somewhere in the temp directory (where unit tests create them)
                                         + @"\s*</Reference>"
            this.MakeProjectAndDo(["bar.fs"], [], "", "v4.5", (fun project ->
                let selectorData = new VSCOMPONENTSELECTORDATA(``type`` = VSCOMPONENTTYPE.VSCOMPONENTTYPE_File, bstrFile = exe)
                let refContainer = GetReferenceContainerNode(project)
                refContainer.AddReferenceFromSelectorData(selectorData) |> Assert.IsNotNull
                SaveProject(project)
                let fsprojFileText = File.ReadAllText(project.FileName)
                printfn "%s" fsprojFileText
                TheTests.HelpfulAssertMatches '<' expectedFsprojRegex fsprojFileText
                // Finally, ensure that the reference works as expected
                project.Reload()
                let assemRef = TheTests.FindNodeWithCaption(project, "Test") :?> AssemblyReferenceNode
                Assert.IsTrue(assemRef.CanShowDefaultIcon(), "the reference could not be resolved")  
                // Use the referenced DLL as a double-check
                let barPath = Path.Combine(project.ProjectFolder, "bar.fs")
                File.AppendAllText(barPath, "printfn \"%d\" Foo.Bar.x")  // code that requires the referenced assembly to successfully compile
                let buildResult = project.Build("Build")
                Assert.IsTrue buildResult.IsSuccessful
                ))
        ))

    [<Test>]
    member this.``ReferenceResolution.Bug4423.NotAValidDll.BrowseTab``() =
        let dirName = Path.GetTempPath()
        let dll = Path.Combine(dirName, "Foo.dll")
        File.AppendAllText(dll, "This is not actually a valid dll")
        try
            this.MakeProjectAndDo(["doesNotMatter.fs"], [], "", "v4.0", (fun project ->
                let selectorData = new VSCOMPONENTSELECTORDATA(``type`` = VSCOMPONENTTYPE.VSCOMPONENTTYPE_File, bstrFile = dll)
                let refContainer = GetReferenceContainerNode(project)
                try
                    refContainer.AddReferenceFromSelectorData(selectorData) |> Assert.IsNotNull
                    Assert.Fail("this should not have succeeded")
                with e ->
                    AssertContains e.Message "could not be added. Please make sure that the file is accessible, and that it is a valid assembly or COM component."
            ))
        finally
            File.Delete(dll)

    [<Test>]
    member this.``PathReferences.Existing`` () =
        DoWithTempFile "Test.fsproj"(fun projFile ->
            let dirName = Path.GetDirectoryName(projFile)
            let libDirName = Directory.CreateDirectory(Path.Combine(dirName, "lib")).FullName
            let codeBase = (new Uri(Assembly.GetExecutingAssembly().EscapedCodeBase)).LocalPath |> Path.GetDirectoryName
            let refLibPath = Path.Combine(libDirName, "nunit.framework.dll")
            File.Copy(Path.Combine(codeBase, "nunit.framework.dll"), refLibPath)
            File.AppendAllText(projFile, TheTests.SimpleFsprojText([], [refLibPath], ""))
            use project = TheTests.CreateProject(projFile) 
            let l = new List<AssemblyReferenceNode>()
            project.FindNodesOfType(l)
            AssertEqual 1 l.Count
            AssertEqual refLibPath l.[0].Url
            AssertEqual refLibPath l.[0].Caption  // when Include is a filename, entirety is caption
            Assert.IsNotNull(l.[0].ResolvedAssembly)
            let refContainer =
                let l = new List<ReferenceContainerNode>()
                project.FindNodesOfType(l)
                l.[0]
            let mscorlibPath = (new Uri("".GetType().Assembly.EscapedCodeBase)).LocalPath
            let selectorData = new VSCOMPONENTSELECTORDATA(``type`` = VSCOMPONENTTYPE.VSCOMPONENTTYPE_ComPlus, bstrFile = mscorlibPath)
            refContainer.AddReferenceFromSelectorData(selectorData) |> Assert.IsNotNull
            let l = new List<AssemblyReferenceNode>()
            project.FindNodesOfType(l)
            AssertEqual 2 l.Count
            AssertEqual refLibPath l.[0].Url
            AssertEqual refLibPath l.[0].Caption
            AssertEqual "mscorlib" l.[1].Caption
        )

    [<Test>]
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
            Assert.IsNotNull(l.[0].ResolvedAssembly)
            AssertEqual "System.Net.dll" l.[1].Caption
            Assert.IsNotNull(l.[1].ResolvedAssembly)
        )
        
    [<Test>]
    member this.``PathReferences.NonExistent`` () =
        DoWithTempFile "Test.fsproj"(fun projFile ->
            let refLibPath = @"c:\foo\baz\blahblah.dll"
            File.AppendAllText(projFile, TheTests.SimpleFsprojText([], [refLibPath], ""))
            use project = TheTests.CreateProject(projFile) 
            let l = new List<AssemblyReferenceNode>()
            project.FindNodesOfType(l)
            AssertEqual 1 l.Count
            AssertEqual refLibPath l.[0].Caption
            Assert.IsNull(l.[0].ResolvedAssembly)
        )

        
    [<Test>]
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
    // [<Test>]     // Disabled due to: https://github.com/dotnet/fsharp/issues/1460
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
            Assert.IsNotNull comReference
            Assert.IsTrue(comReference :? ComReferenceNode)
            let comRef = comReference :?> ComReferenceNode
            Assert.AreEqual(1, comRef.MajorVersionNumber)
            Assert.AreEqual(0, comRef.MinorVersionNumber)
            Assert.AreEqual(guid, comRef.TypeGuid)
            Assert.AreEqual("Microsoft Shell Controls And Automation", comRef.Caption)
            let sysDirectory = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86)
            Assert.IsTrue(String.Compare(Path.Combine(sysDirectory, "shell32.dll"), comRef.InstalledFilePath, StringComparison.OrdinalIgnoreCase) = 0)

            // check node exists under references
            let l = new List<ComReferenceNode>()
            project.FindNodesOfType(l)

            Assert.AreEqual(1, l.Count)
            let referenceNode = l.[0]
            Assert.AreSame(comRef, referenceNode)

            // check saved msbuild item
            SaveProject(project)
            let fsproj = XDocument.Load(project.FileName)
            printfn "%O" fsproj
            let xn s = fsproj.Root.GetDefaultNamespace().GetName(s)
            let comReferencesXml = fsproj.Descendants(xn "COMReference") |> Seq.toList
            
            Assert.AreEqual(1, comReferencesXml |> List.length)

            let comRefXml = comReferencesXml |> List.head

            Assert.AreEqual("Microsoft Shell Controls And Automation", comRefXml.Attribute(XName.Get("Include")).Value)
            Assert.AreEqual(guid, Guid(comRefXml.Element(xn "Guid").Value))
            Assert.AreEqual("1", comRefXml.Element(xn "VersionMajor").Value)
            Assert.AreEqual("0", comRefXml.Element(xn "VersionMinor").Value)
            Assert.AreEqual("0", comRefXml.Element(xn "Lcid").Value)
            )
