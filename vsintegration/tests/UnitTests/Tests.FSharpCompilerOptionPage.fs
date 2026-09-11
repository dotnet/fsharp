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
    // unavailable in a headless unit-test host. The UseNetSdkCompiler get/set only touch HKCU and
    // never read base/DialogPage state, so bypass the constructor to exercise the real property.
    let private newPage () =
        FormatterServices.GetUninitializedObject(typeof<FSharpCompilerPropertyPage>) :?> FSharpCompilerPropertyPage

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

    [<Fact>]
    let ``Setting false writes DWORD 0 and get returns false`` () =
        withCleanRegistry (fun () ->
            let page = newPage ()
            page.UseNetSdkCompiler <- false
            Assert.Equal<int option>(Some 0, readRaw () |> Option.map (fun o -> unbox<int> o))
            Assert.False(page.UseNetSdkCompiler))

    [<Fact>]
    let ``Setting true writes DWORD 1 and get returns true`` () =
        withCleanRegistry (fun () ->
            let page = newPage ()
            page.UseNetSdkCompiler <- true
            Assert.Equal<int option>(Some 1, readRaw () |> Option.map (fun o -> unbox<int> o))
            Assert.True(page.UseNetSdkCompiler))

    [<Fact>]
    let ``A get-only cycle does not write to the registry`` () =
        withCleanRegistry (fun () ->
            let page = newPage ()
            page.UseNetSdkCompiler |> ignore
            page.UseNetSdkCompiler |> ignore
            Assert.True(readRaw () |> Option.isNone))
