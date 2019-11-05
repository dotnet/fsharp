// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.LanguageServer.UnitTests

open System.IO
open FSharp.Compiler.LanguageServer
open NUnit.Framework

[<TestFixture>]
type MiscTests() =

    [<Test>]
    member __.``Find F# projects in a .sln file``() =
        let slnContent = @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 16
VisualStudioVersion = 16.0.29201.188
MinimumVisualStudioVersion = 10.0.40219.1
Project(""{F2A71F9B-5D33-465A-A702-920D77279786}"") = ""ConsoleApp1"", ""ConsoleApp1\ConsoleApp1.fsproj"", ""{60A4BE67-7E03-4200-AD38-B0E5E8E049C1}""
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{60A4BE67-7E03-4200-AD38-B0E5E8E049C1}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{60A4BE67-7E03-4200-AD38-B0E5E8E049C1}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{60A4BE67-7E03-4200-AD38-B0E5E8E049C1}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{60A4BE67-7E03-4200-AD38-B0E5E8E049C1}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(ExtensibilityGlobals) = postSolution
		SolutionGuid = {80902CFC-54E6-4485-AC17-4516930C8B2B}
	EndGlobalSection
EndGlobal
"
        let testDir = @"C:\Dir\With\Solution" // don't care about the potentially improper directory separators here, it's really just a dumb string
        let foundProjects = Solution.getProjectPaths slnContent testDir
        let expected = Path.Combine(testDir, "ConsoleApp1", "ConsoleApp1.fsproj") // proper directory separator characters will be used at runtime
        Assert.AreEqual([| expected |], foundProjects)
