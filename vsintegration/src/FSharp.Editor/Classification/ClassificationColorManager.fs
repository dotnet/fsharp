// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open Microsoft.VisualStudio.Language.StandardClassification
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Classification
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.Text.Formatting
open Microsoft.VisualStudio.Text.Tagging
open Microsoft.VisualStudio.Utilities
open System
open System.Composition
open System.Collections.Generic
open System.ComponentModel.Composition
open System.Windows.Media
open Microsoft.VisualStudio.Shell.Interop
open System.Drawing
open Microsoft.Win32
open EnvDTE
open System.Runtime.InteropServices
open Microsoft.VisualStudio.PlatformUI
open Microsoft.VisualStudio.FSharp.Editor.Reflection
open Microsoft.VisualStudio.FSharp.Editor.Logging

type VisualStudioTheme =
    | Unknown
    | Light
    | Blue
    | Dark

[<Guid("0d915b59-2ed7-472a-9de8-9161737ea1c5")>]
type SVsColorThemeService = interface end

[<Export; Shared>]
type ThemeManager [<ImportingConstructor>] 
    ([<Import(typeof<SVsServiceProvider>)>] serviceProvider: IServiceProvider) =
    static let themes = 
        [ Guid("de3dbbcd-f642-433c-8353-8f1df4370aba"), VisualStudioTheme.Light
          Guid("a4d6a176-b948-4b29-8c66-53c97a1ed7d0"), VisualStudioTheme.Blue
          Guid("1ded0138-47ce-435e-84ef-9ec1f439b749"), VisualStudioTheme.Dark ]
        |> Map.ofList

    let getThemeId() =
        try
            let themeService = serviceProvider.GetService(typeof<SVsColorThemeService>)
            themeService?CurrentTheme?ThemeId: Guid
        with _ -> Guid.Empty

    member __.GetCurrentTheme() =
        let themeGuid = getThemeId()
        themes 
        |> Map.tryFind themeGuid
        |> Option.defaultWith (fun _ ->
            try 
                let color = VSColorTheme.GetThemedColor EnvironmentColors.ToolWindowTextColorKey
                if color.GetBrightness() > 0.5f then
                    VisualStudioTheme.Dark
                else
                    VisualStudioTheme.Light
            with _ -> 
                Logging.logError "Can't read Visual Studio themes from environment colors."
                VisualStudioTheme.Unknown)

    member __.GetEditorTextColors (item: string) =
        let dte = serviceProvider.GetService<EnvDTE.DTE, SDTE>()
        let fontsAndColors = dte.Properties("FontsAndColors", "TextEditor")
        let fontsAndColorsItems = fontsAndColors.Item("FontsAndColorsItems").Object :?> EnvDTE.FontsAndColorsItems
        let selectedItem = fontsAndColorsItems.Item(item)
        let foreColor = int selectedItem.Foreground |> System.Drawing.ColorTranslator.FromOle
        let backColor = int selectedItem.Background |> System.Drawing.ColorTranslator.FromOle
        (foreColor, backColor)

[<Export; Shared>]
type internal ClassificationColorManager
    [<ImportingConstructor>]
    (
        themeManager: ThemeManager,
        classificationFormatMapService: IClassificationFormatMapService,
        classificationTypeRegistry: IClassificationTypeRegistryService
    ) =

    let lightAndBlueColors =
        [ 
          FSharpClassificationTypes.UnionCase, Colors.Purple
          FSharpClassificationTypes.Function, Colors.Navy
          FSharpClassificationTypes.MutableVar, Colors.Red
          FSharpClassificationTypes.Printf, Color.FromRgb(43uy, 145uy, 175uy)
          FSharpClassificationTypes.Property, Colors.Black
        ] |> Map.ofList

    let darkColors =
        [ 
          FSharpClassificationTypes.UnionCase, Color.FromRgb(184uy, 215uy, 163uy)
          FSharpClassificationTypes.Function, Color.FromRgb(220uy, 220uy, 220uy)
          FSharpClassificationTypes.MutableVar, Color.FromRgb(233uy, 122uy, 122uy)
          FSharpClassificationTypes.Printf, Color.FromRgb(78uy, 220uy, 176uy)
          FSharpClassificationTypes.Property, Color.FromRgb(220uy, 220uy, 220uy)
        ] |> Map.ofList

    // Theme changed event may fire even though the same theme is still in use.
    // We save a current theme and skip color updates in these cases. 
    let mutable currentTheme: VisualStudioTheme = themeManager.GetCurrentTheme()

    let updateColors() =
        let newTheme = themeManager.GetCurrentTheme()

        if newTheme <> VisualStudioTheme.Unknown && newTheme <> currentTheme then
            currentTheme <- newTheme
            let colors = if newTheme = VisualStudioTheme.Dark then darkColors else lightAndBlueColors
            let formatMap = classificationFormatMapService.GetClassificationFormatMap(category = "text")
            try
                formatMap.BeginBatchUpdate()
                for KeyValue(ty, color) in colors do
                    let classificationType = classificationTypeRegistry.GetClassificationType(ty)
                    let oldProp = formatMap.GetTextProperties(classificationType)

                    let newProp = 
                        TextFormattingRunProperties.CreateTextFormattingRunProperties(
                            SolidColorBrush(color), oldProp.BackgroundBrush, oldProp.Typeface, Nullable(), Nullable(), oldProp.TextDecorations, 
                            oldProp.TextEffects, oldProp.CultureInfo)

                    formatMap.SetTextProperties(classificationType, newProp)
            finally
                formatMap.EndBatchUpdate()

    let themeChangedHandler = ThemeChangedEventHandler(fun _ -> updateColors())
    do VSColorTheme.add_ThemeChanged(themeChangedHandler)

    member __.GetDefaultColors(classificationType: string) =
        match currentTheme with
        | VisualStudioTheme.Dark -> 
            darkColors |> Map.tryFind classificationType |> Option.defaultValue (Color.FromRgb(220uy, 220uy, 220uy)) |> Nullable
        | VisualStudioTheme.Light
        | VisualStudioTheme.Blue 
        | VisualStudioTheme.Unknown -> 
            lightAndBlueColors |> Map.tryFind classificationType |> Option.defaultValue Colors.Black |> Nullable

    interface IDisposable with
        member __.Dispose() =
            VSColorTheme.remove_ThemeChanged(themeChangedHandler)