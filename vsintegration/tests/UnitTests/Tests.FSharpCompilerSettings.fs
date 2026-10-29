// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

namespace Tests

open Microsoft.Win32
open Microsoft.VisualStudio.FSharp.Interactive
open Xunit

module FSharpCompilerSettingsTests =
    let private subKey = @"Software\Microsoft\VisualStudio\FSharp"
    let private valueName = "UseNetSdkCompiler"

    let private readRaw () =
        use key = Registry.CurrentUser.OpenSubKey(subKey)
        if isNull key then None else Option.ofObj(key.GetValue(valueName))

    let private withCleanRegistry body =
        let original = readRaw ()
        use key = Registry.CurrentUser.CreateSubKey(subKey)
        key.DeleteValue(valueName, false)
        try body ()
        finally
            match original with
            | Some value -> key.SetValue(valueName, value)
            | None -> key.DeleteValue(valueName, false)

    [<Fact>]
    let ``Reading the default compiler selection never creates a registry value`` () =
        withCleanRegistry (fun () ->
            Assert.True(FSharpCompilerSettings.UseNetSdkCompiler)
            Assert.True(FSharpCompilerSettings.UseNetSdkCompiler)
            Assert.Equal<obj option>(None, readRaw ()))

    [<Theory>]
    [<InlineData(0, false)>]
    [<InlineData(1, true)>]
    [<InlineData(42, true)>]
    let ``Compiler selection preserves the registry contract`` initial enabled =
        withCleanRegistry (fun () ->
            use key = Registry.CurrentUser.OpenSubKey(subKey, true)
            key.SetValue(valueName, initial, RegistryValueKind.DWord)
            Assert.Equal(enabled, FSharpCompilerSettings.UseNetSdkCompiler)
            FSharpCompilerSettings.UseNetSdkCompiler <- not enabled
            Assert.Equal(not enabled, FSharpCompilerSettings.UseNetSdkCompiler)
            Assert.Equal<obj option>(Some(box (if enabled then 0 else 1)), readRaw ()))