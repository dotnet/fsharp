// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace UnitTests.Tests

open System.IO
open NUnit.Framework

module U = Microsoft.FSharp.Data.TypeProviders.DesignTime.Utilities
module CF = Microsoft.FSharp.Data.TypeProviders.DesignTime.ConfigFiles
module Util = Microsoft.FSharp.Data.TypeProviders.Utility.Util

[<Parallelizable(ParallelScope.Fixtures)>][<TestFixture>]
type TypeProviderImplTests() = 
    
    [<Test>]
    member public this.``SanitizeFileName``() = 
        let tests = 
            [
                "NorthWind", "Northwind"
                "1NorthWind", "_1northwind"
                "north (wind)", "North_wind_"
                "north wind", "NorthWind"
                "North(Wind)", "North_wind_"
                "(northwind)", "_northwind_"
                "^#$&5wind", "____5wind"
            ]
        for (source, expected) in tests do
            let actual = U.sanitizeDataContextFileName source
            printfn "source: %s, expected %s, actual %s" source expected actual
            Assert.AreEqual(expected, actual)

    [<Test>]
    member public this.``ParseCodegenToolErrors.RecognizeErrorLine``() = 
        let tests =
            [
                // known localization related items 
                [||], [|"error 7015: Parameter switch 'targetversion' is not valid."|], ["error 7015: Parameter switch 'targetversion' is not valid."]
                [||], [|"!stsMv!error 7015: !3eCOn!Parameter switch 'L' is not valid. 表c?字㌍!"|], ["!stsMv!error 7015: !3eCOn!Parameter switch 'L' is not valid. 表c?字㌍!"]
                [||], [|"에러 7015: Parameter switch 'targetversion' is not valid." |], ["에러 7015: Parameter switch 'targetversion' is not valid."]
                [||], [|"!pM1WE!Error:  表c!!i2yz2!Cannot read .. 表c?!"|], ["!pM1WE!Error:  表c!!i2yz2!Cannot read .. 表c?!"]
                [||], [|"エラー: . を読み取れません"|], ["エラー: . を読み取れません"]
                // regular 
                [||], [|"Error: Cannot obtain Metadata from http://bing.com/" |], ["Error: Cannot obtain Metadata from http://bing.com/"]
            ]

        for (stdin, stderr, expectedErrs) in tests do
            try
                U.formatErr stdin stderr
                Assert.Fail("Should throw")
            with
                | :? System.AggregateException as ae -> 
                    let actualErrs = ae.InnerExceptions |> Seq.map (fun e -> e.Message) |> set
                    let expectedErrs = set expectedErrs
                    printfn "Expected: %A" expectedErrs
                    printfn "Actual: %A" actualErrs
                    Assert.IsTrue(actualErrs.Count = expectedErrs.Count)
                    Assert.IsTrue((actualErrs - expectedErrs).Count = 0)
                | e -> sprintf "unexpected exception %A" e |> Assert.Fail

    [<Test>]
    member public this.``ParseCodegenToolErrors.FalsePositives``() = 
        let stdin = 
             [|
                 "Attempting to download metadata from 'http://bing.com/' using WS-Metadata Exchange or DISCO."
                 "Microsoft (R) Service Model Metadata Tool"
                 "[Microsoft (R) Windows (R) Communication Foundation, Version 4.0.30319.17360]"
                 "Copyright (c) Microsoft Corporation.  All rights reserved."
                 ""
                 ""
                 ""
                 "If you would like more help, type \"svcutil /?\""
             |]
        let stderr = 
             [|
                 "Error: Cannot obtain Metadata from http://bing.com/ "
                 ""
                 "If this is a Windows (R) Communication Foundation service to which you have access, please check that you have enabled metadata publishing at the specified address.  For help enabling metadata publishing, please refer to the MSDN documentation at http://go.microsoft.com/fwlink/?LinkId=65455."
                 ""
                 "WS-Metadata Exchange Error"
                 "    URI: http://bing.com/"
                 ""
                 "    Metadata contains a reference that cannot be resolved: 'http://bing.com/'."
                 ""
                 "    The content type text/html; charset=utf-8 of the response message does not match the content type of the binding (application/soap+xml; charset=utf-8). If using a custom encoder, be sure that the IsContentTypeSupported method is implemented properly. The first 194 bytes of the response were: '<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"><html lang=\"en\" xml:lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\"><head>'."
                 ""
                 "HTTP GET Error"
                 "    URI: http://bing.com/"
                 ""
                 "    The HTML document does not contain Web service discovery information."
                 ""
             |]
        try
            U.formatErr stdin stderr
            Assert.Fail("Should throw")
        with
            | :? System.AggregateException as ae -> 
                printfn "%A" ae
                Assert.IsTrue (ae.InnerExceptions.Count = 1, "One exception expected")
                Assert.IsTrue(ae.InnerExceptions.[0].Message = "Error: Cannot obtain Metadata from http://bing.com/")
            | e -> sprintf "unexpected exception %A" e |> Assert.Fail
    
    [<Test>]
    member public this.``PathResolutionUtils.MakeAbsolute``() = 
        use d = Util.TemporaryDirectory()
        let pru = Microsoft.FSharp.Data.TypeProviders.DesignTime.PathResolutionUtils(d.Path)
        
        // NullOrEmpty
        Assert.AreEqual(null, pru.MakeAbsolute(d.Path, null), "null expected")
        Assert.AreEqual("", pru.MakeAbsolute(d.Path, ""), "empty string expected")

        // rooted path
        let subFolder = Path.Combine(d.Path, "Subfolder")
        Assert.AreEqual(subFolder, pru.MakeAbsolute(d.Path, subFolder), "absolute path should be returned as is")

        // relative path
        Assert.AreEqual(subFolder, pru.MakeAbsolute(d.Path, "Subfolder"), "relative path should be converted to absolute")

    [<Test>]
    member public this.``PathResolutionUtils.GetAbsoluteDesignTimeDirectory``() = 
        use d = Util.TemporaryDirectory()
        let pru = Microsoft.FSharp.Data.TypeProviders.DesignTime.PathResolutionUtils(d.Path)
        
        // absolute path
        Assert.AreEqual(d.Path, pru.GetAbsoluteDesignTimeDirectory(d.Path), "absolute path should be returned as is")
        
        // relative path
        let name = "Subfolder"
        let expected = Path.Combine(d.Path, name)
        Assert.AreEqual(expected, pru.GetAbsoluteDesignTimeDirectory(name), "relative path should be converted to absolute")

    [<Test>]
    member public this.``PathResolutionUtils.GetConfigFileWatchSpec.ResolvedPathsInSpecs``() = 
        let (++) a b = Path.Combine(a, b)
        (
            use d = Util.TemporaryDirectory()
            let pru = Microsoft.FSharp.Data.TypeProviders.DesignTime.PathResolutionUtils(d.Path)
        
            // no connection string name set
            pru.GetConfigFileWatchSpec(null, null, null) 
            |> List.length
            |> fun l -> Assert.AreEqual(0, l, "Connection string name not set => not watch specs expected")
        )

        let check caption resFolder configFile f = 
            printfn "Running %s" caption
            use d = Util.TemporaryDirectory()
            let pru = Microsoft.FSharp.Data.TypeProviders.DesignTime.PathResolutionUtils(d.Path)
            match pru.GetConfigFileWatchSpec(resFolder, configFile, "Name") with
            | [r] -> f (d, r)
            | x -> sprintf "%s - unexpected result %A" caption x |> Assert.Fail
            printfn "Finished %s" caption
            

        // NOT FOUND CASES:
        // no custom config file name set - file name should be d.Path + "*.webconfig"
        check "#1" null null <| 
            fun (d, r) ->
                let expectedPath = d.Path  ++ "*.config"
                Assert.AreEqual(expectedPath, r.Path)

        // no custom config file name set + resolutionfolder is specified- result file name should be d.Path + resolutionFolder + "*.webconfig"
        check "#2" "Subfolder" null <| 
            fun (d, r) ->
                let expectedPath = d.Path  ++ "Subfolder" ++"*.config"
                Assert.AreEqual(expectedPath, r.Path)

        // no custom config file name set + absolute resolutionfolder is specified- result file name should be resolutionFolder + "*.webconfig"
        (
            use root = Util.TemporaryDirectory()
            let absResolutionFolder = root.Path ++ "Subfolder2"
            check "#3" absResolutionFolder null <|
                fun (_, r) ->
                    let expectedPath = absResolutionFolder ++"*.config"
                    Assert.AreEqual(expectedPath, r.Path)
        )

        // absolute custom config file name set - result file name should be absolute 
        (
            use root = Util.TemporaryDirectory()
            let absoluteConfigPath = root.Path ++ "somefolder" ++ "custom.config"
            check "#4" null absoluteConfigPath <|
                fun (_, r) -> Assert.AreEqual(absoluteConfigPath, r.Path)
        )

        // absolute custom config file name set - result file name should be absolute 
        (
            use root = Util.TemporaryDirectory()
            let absoluteConfigPath = root.Path ++ "somefolder" ++ "custom.config"
            check "#4.1" "some name" absoluteConfigPath <|
                fun (_, r) -> Assert.AreEqual(absoluteConfigPath, r.Path)
        )

        // resolutionFolder + relative custom config file name set - result file name should be d.Path + resFolder + config file name
        check "#5" "Subfolder" "custom.config" <|
            fun (d, r) -> 
                let expected = d.Path ++ "Subfolder" ++ "custom.config"
                Assert.AreEqual(expected, r.Path)

        // relative custom config file name set - result file name should be d.Path + config file name
        check "#6" null "custom.config" <|
            fun (d, r) -> 
                let expected = d.Path ++  "custom.config"
                Assert.AreEqual(expected, r.Path)

        // resolution folder is absolute and config filename is relative - result file name should be resolution folder + config file name
        (
            use root = Util.TemporaryDirectory()
            check "#7" root.Path "custom.config" <|
                fun (_, r) ->
                    let expected = root.Path ++ "custom.config"
                    Assert.AreEqual(expected, r.Path)
        )

        // FOUND cases
        let checkFound caption resFolder configFile actualConfigPath =
            printfn "Running %s" caption
            use d = Util.TemporaryDirectory()
            let configPath = actualConfigPath d.Path

            Directory.CreateDirectory(Path.GetDirectoryName configPath) 
            |> ignore

            File.WriteAllText(configPath, "")
            let pru = Microsoft.FSharp.Data.TypeProviders.DesignTime.PathResolutionUtils(d.Path)
            match pru.GetConfigFileWatchSpec(resFolder, configFile, "Name") with
            | [r] -> Assert.AreEqual(configPath, r.Path)
            | x -> sprintf "%s, unexpected result %A" caption x |> Assert.Fail
            let di = DirectoryInfo d.Path
            for subdir in di.GetDirectories() do
                subdir.Delete(true)

            printfn "Finished %s" caption
        
        // no res folder\no custom config
        checkFound "#8" null null <| fun d -> d ++ "app.config"
        // relative res folder\no custom config
        checkFound "#9" "subfolder" null <| fun d -> d ++ "subfolder\\web.config"
        // abs res folder\no custom config
        (
            use root = Util.TemporaryDirectory()
            checkFound "#10" root.Path null <| fun _ -> root.Path ++ "app.config"
        )

        // no res folder\rel custom config
        (
            use root = Util.TemporaryDirectory()
            let custom = root.Path ++ "custom.config"
            checkFound "#11" null custom <| fun _ -> custom
        )

        // no res folder\abs custom config
        (
            use root = Util.TemporaryDirectory()
            let custom = root.Path ++ "custom.config"
            checkFound "#11" null custom <| fun _ -> custom
        )

        // relative res folder\relative custom config
        checkFound "#12" "custom" @"sub\custom.config" <| fun d -> d ++ "custom" ++ @"sub\custom.config"

        // relative res folder\absolute custom config
        (
            use root = Util.TemporaryDirectory()
            let custom = root.Path ++ "custom.config"
            checkFound "#13" "custom" custom <| fun _ -> custom
        )

        // abs res folder\absolute custom config
        (
            use resFolder = Util.TemporaryDirectory()
            use configFolder = Util.TemporaryDirectory()
            let custom = configFolder.Path ++ "custom.config"
            checkFound "#14" resFolder.Path custom <| fun _ -> custom
        )

    [<Test>]
    member public this.``ConfigFiles.findConfigFile``() = 
        let withTempFile name f = 
            use d = Util.TemporaryDirectory()
            File.WriteAllText(Path.Combine(d.Path, name), "content")
            f d.Path
        let checkStandard name = 
            withTempFile name <| fun folder ->
                printfn "folder %s" folder
                let r = CF.findConfigFile(folder, null)
                match r with 
                | CF.StandardFound f -> Assert.IsTrue(Path.GetFileName(f) = name)
                | r -> sprintf "findConfigFile: StandardFound (%s)  unexpected result when: %+A" name r |> Assert.Fail
        // check if app.config can be found
        checkStandard "app.config"
        // check if web.config can be found
        checkStandard "web.config"

        // check if proper result is returned is failure case when custom configuration file is not specified 
        (
            use td = Util.TemporaryDirectory()
            let r = CF.findConfigFile(td.Path, null)
            match r with
            | CF.StandardNotFound -> ()
            | r -> sprintf "findConfigFile: StandardNotFound - unexpected result: %+A" r |> Assert.Fail
        )

        // check if custom.config can be found
        let customConfigName = "custom.config"
        withTempFile customConfigName <| fun folder ->
            let r = CF.findConfigFile(folder, Path.Combine(folder, customConfigName))
            match r with 
            | CF.CustomFound f -> Assert.IsTrue(Path.GetFileName(f) = customConfigName)
            | r -> sprintf "findConfigFile: CustomFound (%s)  unexpected result when: %+A" customConfigName r |> Assert.Fail

        // check if proper result is returned is failure case when custom configuration file is specified 
        (
            use td = Util.TemporaryDirectory()
            let r = CF.findConfigFile(td.Path, Path.Combine(td.Path, customConfigName))
            match r with
            | CF.CustomNotFound _ -> ()
            | r -> sprintf "findConfigFile: CustomNotFound - unexpected result: %+A" r |> Assert.Fail
        )

    [<Test>]
    member public this.``ConfigFiles.tryReadConnectionString``() = 
        // 1. no file found
        (
            use dir = Util.TemporaryDirectory()
            let r = CF.tryReadConnectionString(Path.Combine(dir.Path, "app.config"), "F")
            match r with
            | CF.ConnectionStringReadResult.Error (:? FileNotFoundException) -> ()
            | r -> sprintf "no file found: unexpected result - %+A" r |> Assert.Fail
        )

        // 2.  file exists but corrupted
        (
            use f = Util.TemporaryFile "app.config"
            File.WriteAllText(f.Path, "some text")
            let r = CF.tryReadConnectionString(f.Path, "F")
            match r with
            | CF.ConnectionStringReadResult.Error e -> printfn "%A" e
            | r -> sprintf "file exists but corrupted: unexpected result - %+A" r |> Assert.Fail
        )

        // 3. file is correct but doesn't contain connection string
        (
            let configText = """
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.0" />
  </startup>
</configuration>"""
            use f = Util.TemporaryFile "app.config"
            File.WriteAllText(f.Path, configText.Trim())
            let r = CF.tryReadConnectionString(f.Path, "F")
            match r with
            | CF.ConnectionStringReadResult.NotFound -> ()
            | r -> sprintf "file is correct but doesn't contain connection string: unexpected result - %+A" r |> Assert.Fail
        )

        // 4. file is correct and it contains connection string
        (
            let configText = """
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <connectionStrings>
    <add name="S" connectionString="string"/>
  </connectionStrings>
</configuration>"""
            use f = Util.TemporaryFile "app.config"
            File.WriteAllText(f.Path, configText.Trim())
            let r = CF.tryReadConnectionString(f.Path, "S")
            match r with
            | CF.ConnectionStringReadResult.Ok s -> Assert.AreEqual("string", s.ConnectionString)
            | r -> sprintf "file is correct but doesn't contain connection string: unexpected result - %+A" r |> Assert.Fail
        
        )
