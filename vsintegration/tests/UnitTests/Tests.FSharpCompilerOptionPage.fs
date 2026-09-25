// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests

open System
open System.Runtime.Serialization
open Xunit
open Microsoft.Win32
open Microsoft.VisualStudio.FSharp.Interactive

module FSharpCompilerOptionPageTests =

    let private subKey = @"Software\Microsoft\VisualStudio\FSharp"
    let private valueName = "UseNetSdkCompiler"

    // The base DialogPage constructor needs live VS services (JoinableTaskContext), which are
    // unavailable in a headless unit-test host. Initialize the page through its storage hook.
    let private newPage () =
        let page = FormatterServices.GetUninitializedObject(typeof<FSharpCompilerPropertyPage>) :?> FSharpCompilerPropertyPage
        page.LoadSettingsFromStorage()
        page

    // Save/restore the real HKCU value so the test never corrupts a developer's setting.
    let private withCleanRegistry (body: unit -> unit) =
        let original =
            use k = Registry.CurrentUser.OpenSubKey(subKey)
            if isNull k then None else Option.ofObj (k.GetValue(valueName))
        // start from "absent"
        (use k = Registry.CurrentUser.OpenSubKey(subKey, true)
         if not (isNull k) then k.DeleteValue(valueName, false))
        try
            body ()
        finally
            use k = Registry.CurrentUser.CreateSubKey(subKey)
            match original with
            | Some v -> k.SetValue(valueName, v)
            | None -> k.DeleteValue(valueName, false)

    let private readRaw () =
        use k = Registry.CurrentUser.OpenSubKey(subKey)
        if isNull k then None else Option.ofObj (k.GetValue(valueName))

    [<Fact>]
    let ``Default is ON when no registry value is present`` () =
        withCleanRegistry (fun () ->
            let page = newPage ()
            Assert.True(page.UseNetSdkCompiler)
            // merely constructing + getting must not write anything
            Assert.Equal<int option>(None, readRaw () |> Option.map (fun o -> unbox<int> o)))

    [<Theory>]
    [<InlineData(-1, false)>]
    [<InlineData(0, false)>]
    [<InlineData(1, false)>]
    [<InlineData(-1, true)>]
    [<InlineData(0, true)>]
    [<InlineData(1, true)>]
    let ``Edits persist only when saved and reload discards unsaved edits`` (initial: int, save: bool) =
        withCleanRegistry (fun () ->
            if initial <> -1 then
                use k = Registry.CurrentUser.CreateSubKey(subKey)
                k.SetValue(valueName, initial, RegistryValueKind.DWord)
            let original = readRaw ()
            let page = newPage ()
            let initialEnabled = initial <> 0
            Assert.Equal(initialEnabled, page.UseNetSdkCompiler)
            page.UseNetSdkCompiler <- not initialEnabled
            Assert.Equal(not initialEnabled, page.UseNetSdkCompiler)
            Assert.Equal<obj option>(original, readRaw ())
            if save then
                page.SaveSettingsToStorage()
                Assert.Equal<int option>(Some (if initialEnabled then 0 else 1), readRaw () |> Option.map unbox<int>)
                page.UseNetSdkCompiler <- initialEnabled
            page.LoadSettingsFromStorage()
            Assert.Equal((if save then not initialEnabled else initialEnabled), page.UseNetSdkCompiler)
            if not save then
                Assert.Equal<obj option>(original, readRaw ()))

    [<Fact>]
    let ``A get-only cycle does not write to the registry`` () =
        withCleanRegistry (fun () ->
            let page = newPage ()
            page.UseNetSdkCompiler |> ignore
            page.UseNetSdkCompiler |> ignore
            Assert.True(readRaw () |> Option.isNone))
