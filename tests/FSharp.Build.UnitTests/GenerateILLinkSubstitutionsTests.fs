// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build.UnitTests

open System
open System.IO
open System.Xml.Linq
open Microsoft.Build.Framework
open FSharp.Build
open Xunit

type GenerateILLinkSubstitutionsTests() =

    let newTask () =
        GenerateILLinkSubstitutions(BuildEngine = MockEngine())

    let createTempDirectory () =
        let tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())
        Directory.CreateDirectory(tempDir) |> ignore
        tempDir

    let cleanupTempDirectory (tempDir: string) =
        if Directory.Exists(tempDir) then
            Directory.Delete(tempDir, true)

    [<Fact>]
    member _.``Task executes successfully with valid inputs``() =
        let tempDir = createTempDirectory ()

        try
            let task = newTask ()
            task.AssemblyName <- "TestAssembly"
            task.IntermediateOutputPath <- tempDir

            let result = task.Execute()

            Assert.True(result, "Task should execute successfully")
        finally
            cleanupTempDirectory tempDir

    [<Fact>]
    member _.``Generated file is created in the correct path``() =
        let tempDir = createTempDirectory ()

        try
            let task = newTask ()
            task.AssemblyName <- "TestAssembly"
            task.IntermediateOutputPath <- tempDir

            let result = task.Execute()

            Assert.True(result, "Task should execute successfully")

            let expectedPath = Path.Combine(tempDir, "ILLink.Substitutions.xml")
            Assert.True(File.Exists(expectedPath), $"File should exist at {expectedPath}")
        finally
            cleanupTempDirectory tempDir

    [<Fact>]
    member _.``Generated TaskItem has correct LogicalName metadata``() =
        let tempDir = createTempDirectory ()

        try
            let task = newTask ()
            task.AssemblyName <- "TestAssembly"
            task.IntermediateOutputPath <- tempDir

            let result = task.Execute()

            Assert.True(result, "Task should execute successfully")
            Assert.Equal(1, task.GeneratedItems.Length)

            let item = task.GeneratedItems.[0]
            let logicalName = item.GetMetadata("LogicalName")
            Assert.Equal("ILLink.Substitutions.xml", logicalName)
        finally
            cleanupTempDirectory tempDir

    [<Fact>]
    member _.``Generated TaskItem has correct ItemSpec``() =
        let tempDir = createTempDirectory ()

        try
            let task = newTask ()
            task.AssemblyName <- "TestAssembly"
            task.IntermediateOutputPath <- tempDir

            let result = task.Execute()

            Assert.True(result, "Task should execute successfully")
            Assert.Equal(1, task.GeneratedItems.Length)

            let item = task.GeneratedItems.[0]
            let expectedPath = Path.Combine(tempDir, "ILLink.Substitutions.xml")
            Assert.Equal(expectedPath, item.ItemSpec)
        finally
            cleanupTempDirectory tempDir

    [<Fact>]
    member _.``Generated file contains proper XML structure``() =
        let tempDir = createTempDirectory ()

        try
            let task = newTask ()
            task.AssemblyName <- "TestAssembly"
            task.IntermediateOutputPath <- tempDir

            let result = task.Execute()

            Assert.True(result, "Task should execute successfully")

            let filePath = Path.Combine(tempDir, "ILLink.Substitutions.xml")
            let doc = XDocument.Load(filePath)

            // Verify root element is <linker>
            Assert.Equal("linker", doc.Root.Name.LocalName)

            // Verify assembly element exists with correct fullname attribute
            let assemblyElement = doc.Root.Element(XName.Get("assembly"))
            Assert.NotNull(assemblyElement)
            let fullnameAttr = assemblyElement.Attribute(XName.Get("fullname"))
            Assert.NotNull(fullnameAttr)
            Assert.Equal("TestAssembly", fullnameAttr.Value)
        finally
            cleanupTempDirectory tempDir

    [<Fact>]
    member _.``Generated XML contains all expected F# metadata resource prefixes``() =
        let tempDir = createTempDirectory ()

        try
            let task = newTask ()
            task.AssemblyName <- "TestAssembly"
            task.IntermediateOutputPath <- tempDir

            let result = task.Execute()

            Assert.True(result, "Task should execute successfully")

            let filePath = Path.Combine(tempDir, "ILLink.Substitutions.xml")
            let doc = XDocument.Load(filePath)

            // Expected resource prefixes
            let expectedPrefixes =
                [|
                    // Signature variants
                    "FSharpSignatureData"
                    "FSharpSignatureDataB"
                    "FSharpSignatureCompressedData"
                    "FSharpSignatureCompressedDataB"
                    // Optimization variants
                    "FSharpOptimizationData"
                    "FSharpOptimizationDataB"
                    "FSharpOptimizationCompressedData"
                    "FSharpOptimizationCompressedDataB"
                    // Info variants
                    "FSharpOptimizationInfo"
                    "FSharpSignatureInfo"
                |]

            let assemblyElement = doc.Root.Element(XName.Get("assembly"))
            let resourceElements = assemblyElement.Elements(XName.Get("resource")) |> Seq.toArray

            // Verify each expected prefix is present
            for prefix in expectedPrefixes do
                let expectedResourceName = $"{prefix}.TestAssembly"
                let found =
                    resourceElements
                    |> Array.exists (fun elem ->
                        let nameAttr = elem.Attribute(XName.Get("name"))
                        nameAttr <> null && nameAttr.Value = expectedResourceName)

                Assert.True(found, $"Expected resource prefix '{expectedResourceName}' not found in generated XML")

            // Verify total count matches
            Assert.Equal(expectedPrefixes.Length, resourceElements.Length)
        finally
            cleanupTempDirectory tempDir

    [<Fact>]
    member _.``Generated XML resource entries have remove action``() =
        let tempDir = createTempDirectory ()

        try
            let task = newTask ()
            task.AssemblyName <- "TestAssembly"
            task.IntermediateOutputPath <- tempDir

            let result = task.Execute()

            Assert.True(result, "Task should execute successfully")

            let filePath = Path.Combine(tempDir, "ILLink.Substitutions.xml")
            let doc = XDocument.Load(filePath)

            let assemblyElement = doc.Root.Element(XName.Get("assembly"))
            let resourceElements = assemblyElement.Elements(XName.Get("resource")) |> Seq.toArray

            // Verify all resource elements have action="remove"
            for elem in resourceElements do
                let actionAttr = elem.Attribute(XName.Get("action"))
                Assert.NotNull(actionAttr)
                Assert.Equal("remove", actionAttr.Value)
        finally
            cleanupTempDirectory tempDir

    [<Fact>]
    member _.``Task creates intermediate output directory if it does not exist``() =
        let rootTempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())
        let tempDir = Path.Combine(rootTempDir, "nested", "path")

        try
            Assert.False(Directory.Exists(tempDir), "Temp directory should not exist initially")

            let task = newTask ()
            task.AssemblyName <- "TestAssembly"
            task.IntermediateOutputPath <- tempDir

            let result = task.Execute()

            Assert.True(result, "Task should execute successfully")
            Assert.True(Directory.Exists(tempDir), "Temp directory should be created")

            let expectedPath = Path.Combine(tempDir, "ILLink.Substitutions.xml")
            Assert.True(File.Exists(expectedPath), $"File should exist at {expectedPath}")
        finally
            cleanupTempDirectory rootTempDir

    [<Fact>]
    member _.``Task works with assembly names containing special characters``() =
        let tempDir = createTempDirectory ()

        try
            let task = newTask ()
            task.AssemblyName <- "My.Special.Assembly"
            task.IntermediateOutputPath <- tempDir

            let result = task.Execute()

            Assert.True(result, "Task should execute successfully")

            let filePath = Path.Combine(tempDir, "ILLink.Substitutions.xml")
            let doc = XDocument.Load(filePath)

            let assemblyElement = doc.Root.Element(XName.Get("assembly"))
            let fullnameAttr = assemblyElement.Attribute(XName.Get("fullname"))
            Assert.NotNull(fullnameAttr)
            Assert.Equal("My.Special.Assembly", fullnameAttr.Value)

            // Verify resource names contain the full assembly name
            let resourceElements = assemblyElement.Elements(XName.Get("resource")) |> Seq.toArray

            for elem in resourceElements do
                let nameAttr = elem.Attribute(XName.Get("name"))
                Assert.NotNull(nameAttr)
                Assert.True(nameAttr.Value.EndsWith(".My.Special.Assembly"), $"Resource name '{nameAttr.Value}' should end with assembly name")
        finally
            cleanupTempDirectory tempDir
