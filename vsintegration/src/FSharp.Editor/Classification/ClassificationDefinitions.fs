// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel.Composition
open System.Windows.Media

open Microsoft.VisualStudio
open Microsoft.VisualStudio.PlatformUI
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.Internal.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.Language.StandardClassification
open Microsoft.VisualStudio.Text.Classification
open Microsoft.VisualStudio.Utilities
open Microsoft.CodeAnalysis.Classification

open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.VisualStudio.FSharp.LanguageService

[<RequireQualifiedAccess>]
module internal FSharpClassificationTypes =
    let [<Literal>] Function = "FSharp.Function"
    let [<Literal>] MutableVar = "FSharp.MutableVar"
    let [<Literal>] Printf = "FSharp.Printf"
    let [<Literal>] ReferenceType = ClassificationTypeNames.ClassName
    let [<Literal>] Module = ClassificationTypeNames.ModuleName
    let [<Literal>] ValueType = ClassificationTypeNames.StructName
    let [<Literal>] Keyword = ClassificationTypeNames.Keyword
    let [<Literal>] Enum = ClassificationTypeNames.EnumName
    let [<Literal>] Property = "FSharp.Property"
    let [<Literal>] Interface = ClassificationTypeNames.InterfaceName

    let getClassificationTypeName = function
        | SemanticClassificationType.ReferenceType -> ReferenceType
        | SemanticClassificationType.Module -> Module
        | SemanticClassificationType.ValueType -> ValueType
        | SemanticClassificationType.Function -> Function
        | SemanticClassificationType.MutableVar -> MutableVar
        | SemanticClassificationType.Printf -> Printf
        | SemanticClassificationType.ComputationExpression
        | SemanticClassificationType.IntrinsicType -> Keyword
        | SemanticClassificationType.UnionCase
        | SemanticClassificationType.Enumeration -> Enum
        | SemanticClassificationType.Property -> Property
        | SemanticClassificationType.Interface -> Interface

module internal ClassificationDefinitions =
    let private guidVSLightTheme = Guid "de3dbbcd-f642-433c-8353-8f1df4370aba"
    let private guidVSBlueTheme = Guid "a4d6a176-b948-4b29-8c66-53c97a1ed7d0"
    let private guidVSDarkTheme = Guid "1ded0138-47ce-435e-84ef-9ec1f439b749"

    [<Export>]
    type internal ThemeColors
        [<ImportingConstructor>]
        (
            classificationformatMapService: IClassificationFormatMapService,
            classificationTypeRegistry: IClassificationTypeRegistryService,
            [<Import(typeof<SVsServiceProvider>)>] serviceProvider: IServiceProvider
        ) =

        let (| LightTheme | DarkTheme | UnknownTheme |) id =
            if id = guidVSLightTheme || id = guidVSBlueTheme then LightTheme 
            elif id = guidVSDarkTheme then DarkTheme
            else UnknownTheme
    
        let getCurrentThemeId() =
            let themeService = serviceProvider.GetService(typeof<SVsColorThemeService>) :?> IVsColorThemeService
            themeService.CurrentTheme.ThemeId

        let colorData = // name,                  (light,                            dark)
          [ FSharpClassificationTypes.Function,   (Colors.Black,                     Color.FromRgb(220uy, 220uy, 220uy))
            FSharpClassificationTypes.MutableVar, (Color.FromRgb(160uy, 128uy, 0uy), Color.FromRgb(255uy, 210uy, 28uy))
            FSharpClassificationTypes.Printf,     (Color.FromRgb(43uy, 145uy, 175uy), Color.FromRgb(78uy, 220uy, 176uy))
            FSharpClassificationTypes.Property,   (Colors.Black,                     Color.FromRgb(220uy, 220uy, 220uy)) ]

        let setColors _ =
            let fontAndColorStorage = serviceProvider.GetService(typeof<SVsFontAndColorStorage>) :?> IVsFontAndColorStorage
            let fontAndColorCacheManager = serviceProvider.GetService(typeof<SVsFontAndColorCacheManager>) :?> IVsFontAndColorCacheManager)
            fontAndColorCacheManager.CheckCache( ref VSKnownGuids.textEditorFontCategory) |> ignore
            fontAndColorStorage.OpenCategory(ref VSKnownGuids.textEditorFontCategory, uint32 __FCSTORAGEFLAGS.FCSF_READONLY) |> ignore

            let formatMap = classificationformatMapService.GetClassificationFormatMap(category = "text")
            for ctype, (light, dark) in colorData do
                // we don't touch the changes made by the user
                if fontAndColorStorage.GetItem(ctype, Array.zeroCreate 1) <> VSConstants.S_OK  then
                    let ict = classificationTypeRegistry.GetClassificationType(ctype)
                    let oldProps = formatMap.GetTextProperties(ict)
                    let newProps = match getCurrentThemeId() with
                                    | LightTheme -> oldProps.SetForeground light
                                    | DarkTheme -> oldProps.SetForeground dark
                                    | UnknownTheme -> oldProps
                    formatMap.SetTextProperties(ict, newProps)
            fontAndColorStorage.CloseCategory() |> ignore

        let handler = ThemeChangedEventHandler setColors
        do VSColorTheme.add_ThemeChanged handler
        interface IDisposable with member __.Dispose() = VSColorTheme.remove_ThemeChanged handler

        member __.GetColor(ctype) =
            let light, dark = colorData |> Map.ofList |> Map.find ctype
            match getCurrentThemeId() with
            | LightTheme -> Nullable light
            | DarkTheme -> Nullable dark
            | UnknownTheme -> Nullable()

    [<Export; Name(FSharpClassificationTypes.Function); BaseDefinition(PredefinedClassificationTypeNames.FormalLanguage)>]
    let FSharpFunctionClassificationType : ClassificationTypeDefinition = null

    [<Export; Name(FSharpClassificationTypes.MutableVar); BaseDefinition(PredefinedClassificationTypeNames.FormalLanguage)>]
    let FSharpMutableVarClassificationType : ClassificationTypeDefinition = null

    [<Export; Name(FSharpClassificationTypes.Printf); BaseDefinition(PredefinedClassificationTypeNames.FormalLanguage)>]
    let FSharpPrintfClassificationType : ClassificationTypeDefinition = null

    [<Export; Name(FSharpClassificationTypes.Property); BaseDefinition(PredefinedClassificationTypeNames.FormalLanguage)>]
    let FSharpPropertyClassificationType : ClassificationTypeDefinition = null

    [<Export(typeof<EditorFormatDefinition>)>]
    [<ClassificationType(ClassificationTypeNames = FSharpClassificationTypes.Function)>]
    [<Name(FSharpClassificationTypes.Function)>]
    [<UserVisible(true)>]
    [<Order(After = PredefinedClassificationTypeNames.Keyword)>]
    type internal FSharpFunctionTypeFormat [<ImportingConstructor>](theme: ThemeColors) as self =
        inherit ClassificationFormatDefinition()

        do self.DisplayName <- SR.FSharpFunctionsOrMethodsClassificationType.Value
           self.ForegroundColor <- theme.GetColor FSharpClassificationTypes.Function

    [<Export(typeof<EditorFormatDefinition>)>]
    [<ClassificationType(ClassificationTypeNames = FSharpClassificationTypes.MutableVar)>]
    [<Name(FSharpClassificationTypes.MutableVar)>]
    [<UserVisible(true)>]
    [<Order(After = PredefinedClassificationTypeNames.Keyword)>]
    type internal FSharpMutableVarTypeFormat [<ImportingConstructor>](theme: ThemeColors) as self =
        inherit ClassificationFormatDefinition()

        do self.DisplayName <- SR.FSharpMutableVarsClassificationType.Value
           self.ForegroundColor <- theme.GetColor FSharpClassificationTypes.MutableVar

    [<Export(typeof<EditorFormatDefinition>)>]
    [<ClassificationType(ClassificationTypeNames = FSharpClassificationTypes.Printf)>]
    [<Name(FSharpClassificationTypes.Printf)>]
    [<UserVisible(true)>]
    [<Order(After = PredefinedClassificationTypeNames.String)>]
    type internal FSharpPrintfTypeFormat [<ImportingConstructor>](theme: ThemeColors) as self =
        inherit ClassificationFormatDefinition()

        do self.DisplayName <- SR.FSharpPrintfFormatClassificationType.Value
           self.ForegroundColor <- theme.GetColor FSharpClassificationTypes.Printf

    [<Export(typeof<EditorFormatDefinition>)>]
    [<ClassificationType(ClassificationTypeNames = FSharpClassificationTypes.Property)>]
    [<Name(FSharpClassificationTypes.Property)>]
    [<UserVisible(true)>]
    [<Order(After = PredefinedClassificationTypeNames.Keyword)>]
    type internal FSharpPropertyFormat [<ImportingConstructor>](theme: ThemeColors) as self =
        inherit ClassificationFormatDefinition()

        do self.DisplayName <- SR.FSharpPropertiesClassificationType.Value
           self.ForegroundColor <- theme.GetColor FSharpClassificationTypes.Property
