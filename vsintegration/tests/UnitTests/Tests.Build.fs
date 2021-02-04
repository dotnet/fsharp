// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests

open NUnit.Framework
open System
open System.IO
open System.Diagnostics
open FSharp.Build
open Microsoft.Build.Framework
open Microsoft.Build.Utilities
open UnitTests.TestLib.Utils.FilesystemHelpers

module HandyExtensions =
    type System.String with
        member s.MatchesPattern(p : string) =
            System.Text.RegularExpressions.Regex.Match(s, p) <> System.Text.RegularExpressions.Match.Empty
        member s.AssertMatchesPattern(p : string) =
            if not (s.MatchesPattern p) then
                let up = System.Text.RegularExpressions.Regex.Unescape p
                let message = sprintf "Expected\n%A\nto match pattern\n%A" s up
                printfn "%s" message
                Assert.Fail()
open HandyExtensions

type MyLogger(f : string -> unit) =
    let mutable verbosity = LoggerVerbosity.Quiet
    let mutable parameters = ""
    interface ILogger with
        member mylogger.Initialize(eventSource : IEventSource) =
            let onCustomEvent (e : CustomBuildEventArgs) = f e.Message
            eventSource.CustomEventRaised.Add onCustomEvent
            ()
        member mylogger.Verbosity = verbosity
        member mylogger.set_Verbosity x = verbosity <- x
        member mylogger.Parameters = parameters
        member mylogger.set_Parameters x = parameters <- x
        member mylogger.Shutdown() = ()

type FauxHostObject() =
    let mutable myFlags : string[] = null
    let mutable mySources : string[] = null
    member x.Compile(compile:System.Func<int>, flags:string[], sources:string[]) =
        myFlags <- flags
        mySources <- sources
        0
    member x.Flags = myFlags
    member x.Sources = mySources
    interface ITaskHost
        // no members

[<TestFixture>]
type Build() = 
    (* Asserts ----------------------------------------------------------------------------- *)
    let AssertEqual expected actual =
        if expected<>actual then 
            let message = sprintf "Expected\n%A\nbut got\n%A" expected actual
            printfn "%s" message
            Assert.Fail(message)

    let MakeTaskItem (itemSpec : string) = new TaskItem(itemSpec) :> ITaskItem

    /// Called per test
    [<SetUp>]
    member this.Setup() =
        ()
        
    [<TearDown>]
    member this.TearDown() =
        ()

    [<Test>]
    member public this.TestCodePage() =
        let tool = new FSharp.Build.Fsc()
        printfn "By the way, the registry or app.config tool path is %s" tool.ToolPath
        tool.CodePage <- "65001"
        AssertEqual "65001" tool.CodePage 
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--codepage:65001" + Environment.NewLine +
                     "--optimize+" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd

    [<Test>]
    member public this.TestDebugSymbols() =
        let tool = new FSharp.Build.Fsc()
        tool.DebugSymbols <- true
        AssertEqual true tool.DebugSymbols
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("-g" + Environment.NewLine +
                     "--optimize+" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd

    [<Test>]
    member public this.TestDebugType() =
        let tool = new FSharp.Build.Fsc()
        tool.DebugType <- "pdbONly"
        AssertEqual "pdbONly" tool.DebugType
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--debug:pdbonly" + Environment.NewLine +
                     "--optimize+" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd

    [<Test>]
    member public this.TestDefineConstants() =
        let tool = new FSharp.Build.Fsc()
        tool.DefineConstants <- [| MakeTaskItem "FOO=3"
                                   MakeTaskItem "BAR=4" |]
        AssertEqual 2 tool.DefineConstants.Length 
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--define:FOO=3" + Environment.NewLine +
                     "--define:BAR=4" + Environment.NewLine +
                     "--optimize+" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd

    [<Test>]
    member public this.TestDisabledWarnings1() =
        let tool = new FSharp.Build.Fsc()
        tool.DisabledWarnings <- "52;109"
        AssertEqual "52;109" tool.DisabledWarnings
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--optimize+" + Environment.NewLine +
                     "--nowarn:52,109" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd

    [<Test>]
    member public this.TestDisabledWarnings2() =
        let tool = new FSharp.Build.Fsc()
        tool.DisabledWarnings <- ";"  // e.g. someone may have <NoWarn>$(NoWarn);$(SomeOtherVar)</NoWarn> and both vars are empty
        AssertEqual ";" tool.DisabledWarnings
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--optimize+" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd

    [<Test>]
    member public this.TestWarningsNotAsErrors() =
        let tool = new FSharp.Build.Fsc()
        tool.WarningsNotAsErrors <- "52;109"
        AssertEqual "52;109" tool.WarningsNotAsErrors
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--optimize+" + Environment.NewLine +
                     "--warnaserror-:52,109" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd

    [<Test>]
    member public this.TestVersionFile() =
        let tool = new FSharp.Build.Fsc()
        tool.VersionFile <- "src/version"
        AssertEqual "src/version" tool.VersionFile 
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--optimize+" + Environment.NewLine +
                     "--versionfile:src/version" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd

    [<Test>]
    member public this.TestDocumentationFile() =
        let tool = new FSharp.Build.Fsc()
        tool.DocumentationFile <- "foo.xml"
        AssertEqual "foo.xml" tool.DocumentationFile 
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--doc:foo.xml" + Environment.NewLine +
                     "--optimize+" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd

    [<Test>]
    member public this.TestGenerateInterfaceFile() =
        let tool = new FSharp.Build.Fsc()
        tool.GenerateInterfaceFile <- "foo.fsi"
        AssertEqual "foo.fsi" tool.GenerateInterfaceFile 
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--sig:foo.fsi" + Environment.NewLine +
                     "--optimize+" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd

    [<Test>]
    member public this.TestKeyFile() =
        let tool = new FSharp.Build.Fsc()
        tool.KeyFile <- "key.txt"
        AssertEqual "key.txt" tool.KeyFile 
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--keyfile:key.txt" + Environment.NewLine +
                     "--optimize+" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd

    [<Test>]
    member public this.TestNoFramework() =
        let tool = new FSharp.Build.Fsc()
        tool.NoFramework <- true
        AssertEqual true tool.NoFramework 
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--noframework" + Environment.NewLine +
                     "--optimize+" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd

    [<Test>]
    member public this.TestOptimize() =
        let tool = new FSharp.Build.Fsc()
        tool.Optimize <- false
        AssertEqual false tool.Optimize 
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--optimize-" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd

    [<Test>]
    member public this.TestTailcalls() =
        let tool = new FSharp.Build.Fsc()
        tool.Tailcalls <- true
        AssertEqual true tool.Tailcalls
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        // REVIEW we don't put the default, is that desired?
        AssertEqual ("--optimize+" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd

    [<Test>]
    member public this.TestOtherFlags() =
        let tool = new FSharp.Build.Fsc()
        tool.OtherFlags <- "--yadda yadda"
        AssertEqual "--yadda yadda" tool.OtherFlags 
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--optimize+" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore" + Environment.NewLine +
                     "--yadda" + Environment.NewLine +
                     "yadda")
                    cmd

    [<Test>]
    member public this.TestOutputAssembly() =
        let tool = new FSharp.Build.Fsc()
        tool.OutputAssembly <- "oUt.dll"
        AssertEqual "oUt.dll" tool.OutputAssembly 
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("-o:oUt.dll" + Environment.NewLine +
                     "--optimize+" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd

    [<Test>]
    member public this.TestPdbFile() =
        let tool = new FSharp.Build.Fsc()
        tool.PdbFile <- "out.pdb"
        AssertEqual "out.pdb" tool.PdbFile 
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--optimize+" + Environment.NewLine +
                     "--pdb:out.pdb" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd

    [<Test>]
    member public this.TestPlatform1() =
        let tool = new FSharp.Build.Fsc()
        tool.Platform <- "x64"
        AssertEqual "x64" tool.Platform 
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--optimize+" + Environment.NewLine +
                     "--platform:x64" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd

    [<Test>]
    member public this.TestPlatform3() =
        let tool = new FSharp.Build.Fsc()
        tool.Platform <- "x86"
        AssertEqual "x86" tool.Platform 
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--optimize+" + Environment.NewLine +
                     "--platform:x86" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd

    [<Test>]
    member public this.TestReferences() =
        let tool = new FSharp.Build.Fsc()
        let dll = "c:\\sd\\staging\\tools\\nunit\\nunit.framework.dll"
        tool.References <- [| MakeTaskItem dll |]
        AssertEqual 1 tool.References.Length 
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--optimize+" + Environment.NewLine +
                     "-r:" + dll + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd

    [<Test>]
    member public this.TestReferencePath() =
        let tool = new FSharp.Build.Fsc()
        let path = "c:\\sd\\staging\\tools\\nunit\\;c:\\Foo"
        tool.ReferencePath <- path
        AssertEqual path tool.ReferencePath 
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--optimize+" + Environment.NewLine +
                     "--lib:c:\\sd\\staging\\tools\\nunit\\,c:\\Foo" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd

    [<Test>]
    member public this.TestReferencePathWithSpaces() =
        let tool = new FSharp.Build.Fsc()
        let path = "c:\\program files;c:\\sd\\staging\\tools\\nunit;c:\\Foo"
        tool.ReferencePath <- path
        AssertEqual path tool.ReferencePath 
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--optimize+" + Environment.NewLine +
                     "--lib:c:\\program files,c:\\sd\\staging\\tools\\nunit,c:\\Foo" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd

    [<Test>]
    member public this.TestResources() =
        let tool = new FSharp.Build.Fsc()
        tool.Resources <- [| MakeTaskItem "Foo.resources" |]
        AssertEqual 1 tool.Resources.Length 
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--optimize+" + Environment.NewLine +
                     "--resource:Foo.resources" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd

    [<Test>]
    member public this.TestSources() =
        let tool = new FSharp.Build.Fsc()
        let src = "foo.fs"
        let iti = MakeTaskItem src
        tool.Sources <- [| iti; iti |]
        AssertEqual 2 tool.Sources.Length 
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--optimize+" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore" + Environment.NewLine +
                     src + Environment.NewLine +
                     src)
                    cmd
        ()

    [<Test>]
    member public this.TestTargetType1() =
        let tool = new FSharp.Build.Fsc()
        tool.TargetType <- "Library"
        AssertEqual "Library" tool.TargetType 
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--optimize+" + Environment.NewLine +
                     "--target:library" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd

    [<Test>]
    member public this.TestTargetType2() =
        let tool = new FSharp.Build.Fsc()
        tool.TargetType <- "Winexe"
        AssertEqual "Winexe" tool.TargetType 
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--optimize+" + Environment.NewLine +
                     "--target:winexe" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd

    [<Test>]
    member public this.TestTargetType3() =
        let tool = new FSharp.Build.Fsc()
        tool.TargetType <- "Module"
        AssertEqual "Module" tool.TargetType 
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--optimize+" + Environment.NewLine +
                     "--target:module" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd

    [<Test>]
    member public this.TestUtf8Output() =
        let tool = new FSharp.Build.Fsc()
        tool.Utf8Output <- true
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--optimize+" + Environment.NewLine +
                     "--utf8output" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd

    [<Test>]
    member public this.TestWin32Res() =
        let tool = new FSharp.Build.Fsc()
        tool.Win32ResourceFile <- "foo.res"
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--optimize+" + Environment.NewLine +
                     "--win32res:foo.res" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd

    [<Test>]
    member public this.TestWin32Manifest() =
        let tool = new FSharp.Build.Fsc()
        tool.Win32ManifestFile <- "foo.manifest"
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--optimize+" + Environment.NewLine +
                     "--win32manifest:foo.manifest" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd 

    [<Test>]
    member public this.TestHighEntropyVA() =
        let tool = new FSharp.Build.Fsc()
        tool.HighEntropyVA <- true
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--optimize+" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--highentropyva+" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd 

    [<Test>]
    member public this.TestSubsystemVersion() =
        let tool = new FSharp.Build.Fsc()
        tool.SubsystemVersion <- "6.02"
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd
        AssertEqual ("--optimize+" + Environment.NewLine +
                     "--fullpaths" + Environment.NewLine +
                     "--flaterrors" + Environment.NewLine +
                     "--subsystemversion:6.02" + Environment.NewLine +
                     "--highentropyva-" + Environment.NewLine +
                     "--nocopyfsharpcore")
                    cmd 

    [<Test>]
    member public this.TestAllCombo() =
        let tool = new FSharp.Build.Fsc()
        tool.CodePage <- "65001"
        tool.DebugSymbols <- true
        tool.DebugType <- "full"
        tool.DefineConstants <- [| MakeTaskItem "FOO=3"
                                   MakeTaskItem "BAR=4" |]
        tool.DisabledWarnings <- "52,109"
        tool.VersionFile <- "src/version"
        tool.DocumentationFile <- "foo.xml"
        tool.GenerateInterfaceFile <- "foo.fsi"
        tool.KeyFile <- "key.txt" 
        tool.NoFramework <- true
        tool.Optimize <- true
        tool.Tailcalls <- true
        tool.OtherFlags <- "--yadda:yadda --other:\"internal quote\" blah"
        tool.OutputAssembly <- "out.dll"
        tool.PdbFile <- "out.pdb"
        tool.References <- [| MakeTaskItem "ref.dll"; MakeTaskItem "C:\\Program Files\\SpacesPath.dll" |]
        tool.ReferencePath <- @"c:\foo;c:\bar"
        tool.Resources <- [| MakeTaskItem "MyRes.resources"; MakeTaskItem "OtherRes.resources" |]
        tool.Sources <- [| MakeTaskItem "foo.fs"; MakeTaskItem "C:\\Program Files\\spaces.fs" |]
        tool.WarningLevel <- "4"
        tool.TreatWarningsAsErrors <- true
        tool.TargetType <- "Exe"
        tool.BaseAddress <- "0xBADF00D"
        tool.Platform <- "AnyCPU"
        tool.Utf8Output <- true
        tool.VisualStudioStyleErrors <- true
        tool.SubsystemVersion <- "4.0"
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd

        let expected = 
            "-o:out.dll" + Environment.NewLine +
            "--codepage:65001" + Environment.NewLine +
            "-g" + Environment.NewLine +
            "--debug:full" + Environment.NewLine +
            "--noframework" + Environment.NewLine  +
            "--baseaddress:0xBADF00D" + Environment.NewLine +
            "--define:FOO=3" + Environment.NewLine +
            "--define:BAR=4" + Environment.NewLine +
            "--doc:foo.xml" + Environment.NewLine +
            "--sig:foo.fsi" + Environment.NewLine +
            "--keyfile:key.txt" + Environment.NewLine +
            "--optimize+" + Environment.NewLine +
            "--pdb:out.pdb" + Environment.NewLine +
            "--platform:anycpu" + Environment.NewLine +
            "--resource:MyRes.resources" + Environment.NewLine +
            "--resource:OtherRes.resources" + Environment.NewLine +
            "--versionfile:src/version" + Environment.NewLine +
            "-r:ref.dll" + Environment.NewLine +
            @"-r:C:\Program Files\SpacesPath.dll" + Environment.NewLine +
            @"--lib:c:\foo,c:\bar" + Environment.NewLine +
            "--target:exe" + Environment.NewLine +
            "--nowarn:52,109" + Environment.NewLine +
            "--warn:4" + Environment.NewLine +
            "--warnaserror" + Environment.NewLine +
            "--vserrors" + Environment.NewLine +
            "--utf8output" + Environment.NewLine +
            "--fullpaths" + Environment.NewLine +
            "--flaterrors" + Environment.NewLine +
            "--subsystemversion:4.0" + Environment.NewLine +
            "--highentropyva-" + Environment.NewLine +
            "--nocopyfsharpcore" + Environment.NewLine +
            "--yadda:yadda" + Environment.NewLine +
            "--other:internal quote" + Environment.NewLine +
            "blah" + Environment.NewLine +
            "foo.fs" + Environment.NewLine +
            @"C:\Program Files\spaces.fs"

        AssertEqual expected cmd

        let hostObject = new FauxHostObject()
        tool.HostObject <- hostObject
        tool.InternalExecuteTool("", "", "") |> ignore
        let expectedFlags = [|
            "-o:out.dll"
            "--codepage:65001"
            "-g"
            "--debug:full"
            "--noframework"
            "--baseaddress:0xBADF00D"
            "--define:FOO=3"
            "--define:BAR=4"
            "--doc:foo.xml"
            "--sig:foo.fsi" 
            "--keyfile:key.txt"
            "--optimize+"
            "--pdb:out.pdb"
            "--platform:anycpu"
            "--resource:MyRes.resources"
            "--resource:OtherRes.resources"
            "--versionfile:src/version"
            "-r:ref.dll" 
            "-r:C:\\Program Files\\SpacesPath.dll"  // note no internal quotes
            "--lib:c:\\foo,c:\\bar"
            "--target:exe"
            "--nowarn:52,109"
            "--warn:4"
            "--warnaserror"
            "--vserrors"
            "--utf8output"
            "--fullpaths"
            "--flaterrors"
            "--subsystemversion:4.0"
            "--highentropyva-"
            "--nocopyfsharpcore"
            "--yadda:yadda"
            "--other:internal quote" // note stripped internal quotes
            "blah" |]
        AssertEqual expectedFlags hostObject.Flags 
        let expectedSources = [| "foo.fs"; "C:\\Program Files\\spaces.fs" |]
        AssertEqual expectedSources hostObject.Sources

    [<Test>]
    member public this.``DisabledWarnings build property``() =
        let tool = new FSharp.Build.Fsc()
        tool.DisabledWarnings <- "
        
        \n52,,\n,,,109,110;\r73
        
        ,
        
        ;
        85;
        "
        let cmd = tool.InternalGenerateResponseFileCommands()
        printfn "cmd=\"%s\"" cmd

        let expected =
            "--optimize+" + Environment.NewLine +
            "--nowarn:52,109,110,73,85" + Environment.NewLine +
            "--fullpaths" + Environment.NewLine +
            "--flaterrors" + Environment.NewLine +
            "--highentropyva-" + Environment.NewLine  +
            "--nocopyfsharpcore"

        AssertEqual expected cmd

