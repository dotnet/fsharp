// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Tests.ProjectSystem

open System
open System.Collections.Generic
open System.IO
open System.Reflection
open System.Text.RegularExpressions
open System.Xml.Linq
open NUnit.Framework
open Salsa
open UnitTests.TestLib.Utils.Asserts
open UnitTests.TestLib.Utils.FilesystemHelpers
open UnitTests.TestLib.ProjectSystem
open Microsoft.VisualStudio.FSharp.ProjectSystem


[<TestFixture>]
type MultiTargeting() = 
    inherit TheTests()
    
    // Multitargeting tests.
    // For these test cases, we basically test adding a reference and checking the icon state.
    // It's worth pointing out that while we need a valid assembly, the version of the assembly
    // is returned by the VsTargetFrameworkAssemblies service. Therefore, to test, we provide
    // a mock that returns the requested version, and ignore the real version of the assembly.

    member private this.prepTest(projFile) =
        let dirName = Path.GetDirectoryName(projFile)
        let libDirName = Directory.CreateDirectory(Path.Combine(dirName, "lib")).FullName
        let codeBase = (new Uri(Assembly.GetExecutingAssembly().EscapedCodeBase)).LocalPath |> Path.GetDirectoryName
        let refLibPath = Path.Combine(libDirName, "VisualFSharp.Unittests.dll")
        File.Copy(Path.Combine(codeBase, "VisualFSharp.Unittests.dll"), refLibPath)
        File.AppendAllText(projFile, TheTests.FsprojTextWithProjectReferencesAndOtherFlags([], [refLibPath], [], null, "", "v4.0"))
        refLibPath

    [<Test>]
    member public this.``Multitargeting.CheckIconForMismatchedAssemblyReference`` () =
        DoWithTempFile "Test.fsproj" (fun projFile ->
            let sp, ccn = VsMocks.MakeMockServiceProviderAndConfigChangeNotifier40()
            let refLibPath = this.prepTest(projFile)
            use project = TheTests.CreateProject(projFile, "true", ccn, sp)
            let l = new List<AssemblyReferenceNode>()
            project.FindNodesOfType(l)
            Assert.AreEqual(1, l.Count)
            Assert.AreEqual(refLibPath, l.[0].Url)
            Assert.AreEqual(refLibPath, l.[0].Caption)
            let ref = l.[0]
            Assert.AreEqual(true, ref.CanShowDefaultIcon())
        )

    [<Test>]
    member public this.``Multitargeting.CheckIconForMatchedAssemblyReference20`` () =
        DoWithTempFile "Test.fsproj" (fun projFile ->
            let sp, ccn = VsMocks.MakeMockServiceProviderAndConfigChangeNotifier20()
            let refLibPath = this.prepTest(projFile)
            use project = TheTests.CreateProject(projFile, "true", ccn, sp)
            let l = new List<AssemblyReferenceNode>()
            project.FindNodesOfType(l)
            Assert.AreEqual(1, l.Count)
            Assert.AreEqual(refLibPath, l.[0].Url)
            Assert.AreEqual(refLibPath, l.[0].Caption)
            let ref = l.[0]
            Assert.AreEqual(true, ref.CanShowDefaultIcon())
        )
        
    [<Test>]
    member public this.``Multitargeting.DetermineRuntimeAndSKU`` () =
        DoWithTempFile "Test.fsproj" (fun projFile ->
            let sp, ccn = VsMocks.MakeMockServiceProviderAndConfigChangeNotifier40()
            let refLibPath = this.prepTest(projFile)
            use project = TheTests.CreateProject(projFile, "true", ccn, sp)
            
            let validate (fn : System.Runtime.Versioning.FrameworkName) eR eS =
                let (runtime, sku) = project.DetermineRuntimeAndSKU(fn.ToString())
                Assert.AreEqual(eR, runtime)
                Assert.AreEqual(eS, sku)
            validate (new System.Runtime.Versioning.FrameworkName(".NETFramework", new System.Version(4, 0))) "v4.0" ".NETFramework,Version=v4.0"
            validate (new System.Runtime.Versioning.FrameworkName(".NETFramework", new System.Version(2, 0))) "v2.0.50727" null
        )
        
    [<Test>]
    member public this.``Multitargeting.AppConfigNoStartupNode`` () =
        let root = XElement.Parse("<Configuration></Configuration>")
        let dirty = LangConfigFile.PatchUpXml(root, "version", "sku")
        Assert.IsTrue(dirty)
        Assert.IsTrue(root.ToString().Contains("<supportedRuntime version=\"version\" sku=\"sku\" />"))

    [<Test>]
    member public this.``Multitargeting.AppConfigVersionExistsAddNewSku`` () =
        let root = XElement.Parse("<Configuration><startup><supportedRuntime version=\"version\" /></startup></Configuration>")
        let dirty = LangConfigFile.PatchUpXml(root, "version", "sku")
        Assert.IsTrue(dirty)
        Assert.IsTrue(root.ToString().Contains("<supportedRuntime version=\"version\" sku=\"sku\" />"))

    [<Test>]
    member public this.``Multitargeting.AppConfigVersionExistsReplaceSku`` () =
        let root = XElement.Parse("<Configuration><startup><supportedRuntime version=\"version\" sku=\"oldsku\" /></startup></Configuration>")
        let dirty = LangConfigFile.PatchUpXml(root, "version", "sku")
        Assert.IsTrue(dirty)
        Assert.IsTrue(root.ToString().Contains("<supportedRuntime version=\"version\" sku=\"sku\" />"))

    [<Test>]
    member public this.``Multitargeting.AppConfigVersionExistsRemoveSku`` () =
        let root = XElement.Parse("<Configuration><startup><supportedRuntime version=\"version\" sku=\"oldsku\" /></startup></Configuration>")
        let dirty = LangConfigFile.PatchUpXml(root, "version", null) 
        Assert.IsTrue(dirty)
        Assert.IsTrue(root.ToString().Contains("<supportedRuntime version=\"version\" />"))

    [<Test>]
    member public this.``Multitargeting.AppConfigVersionReplaceOldRuntime`` () =
        let root = XElement.Parse("<Configuration><startup><supportedRuntime version=\"versionold\" sku=\"oldsku\" /></startup></Configuration>")
        let dirty = LangConfigFile.PatchUpXml(root, "version", "sku")
        Assert.IsTrue(dirty)
        Assert.IsTrue(root.ToString().Contains("<supportedRuntime version=\"version\" sku=\"sku\" />"))
        Assert.IsFalse(root.ToString().Contains("<supportedRuntime version=\"versionold\" sku=\"oldsku\" />"))

    [<Test>]
    member public this.``Multitargeting.AppConfigVersionReplaceOldRuntimes`` () =
        let root = XElement.Parse("<Configuration><startup><supportedRuntime version=\"versionold\" sku=\"oldsku\" /><supportedRuntime version=\"versionold2\" /></startup></Configuration>")
        let dirty = LangConfigFile.PatchUpXml(root, "version", "sku")
        Assert.IsTrue(dirty)
        Assert.IsTrue(root.ToString().Contains("<supportedRuntime version=\"version\" sku=\"sku\" />"))
        Assert.IsFalse(root.ToString().Contains("<supportedRuntime version=\"versionold\" sku=\"oldsku\" />"))
        Assert.IsFalse(root.ToString().Contains("<supportedRuntime version=\"versionold2\" />"))

    [<Test>]
    member public this.``Multitargeting.TestFrameworkNameToVersionString`` () =
        let validatePair name str =
            let res = HierarchyNode.GetFrameworkVersionString(name)
            Assert.AreEqual(str, res)

        validatePair (new System.Runtime.Versioning.FrameworkName(".NETFramework", new System.Version(4,0))) "v4.0"
        validatePair (new System.Runtime.Versioning.FrameworkName(".NETFramework", new System.Version(4,0,5))) "v4.0.5"
        validatePair (new System.Runtime.Versioning.FrameworkName(".NETFramework", new System.Version(4,0,256))) "v4.0"
        validatePair (new System.Runtime.Versioning.FrameworkName(".NETFramework", new System.Version(4,0,50727))) "v4.0"
        validatePair (new System.Runtime.Versioning.FrameworkName(".NETFramework", new System.Version(2,0,50727,0))) "v2.0"
        validatePair (new System.Runtime.Versioning.FrameworkName(".NETFramework", new System.Version(2,0,50727,5))) "v2.0"
        validatePair (new System.Runtime.Versioning.FrameworkName(".NETFramework", new System.Version(2,0,5,5))) "v2.0.5"
        validatePair (new System.Runtime.Versioning.FrameworkName(".NETFramework", new System.Version(2,0,0))) "v2.0"
        validatePair (new System.Runtime.Versioning.FrameworkName(".NETFramework", new System.Version(2,0,0,0))) "v2.0"
        validatePair (new System.Runtime.Versioning.FrameworkName(".NETFramework", new System.Version(2,0,0,1))) "v2.0"

        (*
    [<Test>]
    member public this.``Multitargeting.AddAppConfigIfRetargetTo40Full`` () =
        DoWithTempFile "Test.fsproj" (fun projFile ->
            let sp, ccn = VsMocks.MakeMockServiceProviderAndConfigChangeNotifier20()
            
            // add mock service for SLocalRegistry so that CreateInstance on it will return a text buffer
            sp.AddService (typeof<SLocalRegistry>, box(VsMocks.vsLocalRegistry (fun () -> VsMocks.Vs.MakeTextLines())), false)
            
            let refLibPath = this.prepTest(projFile)
            use project = TheTests.CreateProject(projFile, "true", ccn, sp)
            let fn = new System.Runtime.Versioning.FrameworkName(".NETFramework", new System.Version(4, 0))
            project.FixupAppConfigOnTargetFXChange(fn.ToString(), "4.3.0.0", false) |> ignore
            let appFile = Path.Combine((Path.GetDirectoryName projFile), "app.config")
            let appText = System.IO.File.ReadAllText(appFile)
            Assert.IsTrue(appText.Contains("<supportedRuntime version=\"v4.0\" sku=\".NETFramework,Version=v4.0\" />"))
            //Assert.IsTrue(appText.Contains("<supportedRuntime version=\"v4.0\" sku=\".NETFramework,Version=v4.0,Profile=Client\" />"))
            //Assert.IsTrue(appText.Contains("<supportedRuntime version=\"v2.0.50727\" sku=\"Client\" />"))
            //Assert.IsTrue(appText.Contains("<supportedRuntime version=\"v2.0.50727\" />"))
            ()
        )
        *)
