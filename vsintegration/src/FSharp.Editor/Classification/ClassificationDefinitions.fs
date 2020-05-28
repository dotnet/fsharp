// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel.Composition
open System.Windows.Media

open Microsoft.VisualStudio
open Microsoft.VisualStudio.Editor
open Microsoft.VisualStudio.PlatformUI
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.Internal.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.Language.StandardClassification
open Microsoft.VisualStudio.Text.Classification
open Microsoft.VisualStudio.Utilities
open Microsoft.CodeAnalysis.Classification

open FSharp.Compiler.SourceCodeServices

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
    let [<Literal>] TypeArgument = ClassificationTypeNames.TypeParameterName
    let [<Literal>] Operator = ClassificationTypeNames.Operator
    let [<Literal>] Disposable = "FSharp.Disposable"

    let getClassificationTypeName = function
        | SemanticClassificationType.ReferenceType -> ReferenceType
        | SemanticClassificationType.Module -> Module
        | SemanticClassificationType.ValueType -> ValueType
        | SemanticClassificationType.Function -> Function
        | SemanticClassificationType.MutableVar -> MutableVar
        | SemanticClassificationType.Printf -> Printf
        | SemanticClassificationType.ComputationExpression
        | SemanticClassificationType.IntrinsicFunction -> Keyword
        | SemanticClassificationType.UnionCase
        | SemanticClassificationType.Enumeration -> Enum
        | SemanticClassificationType.Property -> Property
        | SemanticClassificationType.Interface -> Interface
        | SemanticClassificationType.TypeArgument -> TypeArgument
        | SemanticClassificationType.Operator -> Operator 
        | SemanticClassificationType.Disposable -> Disposable

module internal ClassificationDefinitions =

    [<Export>]
    [<Export(typeof<ISetThemeColors>)>]
    type internal ThemeColors
        [<ImportingConstructor>]
        (
            classificationformatMapService: IClassificationFormatMapService,
            classificationTypeRegistry: IClassificationTypeRegistryService,
            [<Import(typeof<SVsServiceProvider>)>] serviceProvider: IServiceProvider
        ) =

        let (| LightTheme | DarkTheme | UnknownTheme |) id =
            if id = KnownColorThemes.Light || id = KnownColorThemes.Blue || id = Guids.blueHighContrastThemeId then LightTheme 
            elif id = KnownColorThemes.Dark then DarkTheme
            else UnknownTheme
    
        let getCurrentThemeId() =
            let themeService = serviceProvider.GetService(typeof<SVsColorThemeService>) :?> IVsColorThemeService
            themeService.CurrentTheme.ThemeId

        let colorData = // name,                  (light,                            dark)
          [ FSharpClassificationTypes.Function,   (Colors.Black,                     Color.FromRgb(220uy, 220uy, 220uy))
            FSharpClassificationTypes.MutableVar, (Color.FromRgb(160uy, 128uy, 0uy), Color.FromRgb(255uy, 210uy, 28uy))
            FSharpClassificationTypes.Printf,     (Color.FromRgb(43uy, 145uy, 175uy), Color.FromRgb(78uy, 220uy, 176uy))
            FSharpClassificationTypes.Property,   (Colors.Black,                     Color.FromRgb(220uy, 220uy, 220uy)) 
            FSharpClassificationTypes.Disposable, (Color.FromRgb(43uy, 145uy, 175uy), Color.FromRgb(78uy, 220uy, 176uy)) ]


        let setColors _ =
            let fontAndColorStorage = serviceProvider.GetService(typeof<SVsFontAndColorStorage>) :?> IVsFontAndColorStorage
            let fontAndColorCacheManager = serviceProvider.GetService(typeof<SVsFontAndColorCacheManager>) :?> IVsFontAndColorCacheManager
            fontAndColorCacheManager.CheckCache( ref DefGuidList.guidTextEditorFontCategory) |> ignore
            fontAndColorStorage.OpenCategory(ref DefGuidList.guidTextEditorFontCategory, uint32 __FCSTORAGEFLAGS.FCSF_READONLY) |> ignore

            let formatMap = classificationformatMapService.GetClassificationFormatMap(category = "text")
            try 
                formatMap.BeginBatchUpdate()
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
            finally formatMap.EndBatchUpdate()

        let handler = ThemeChangedEventHandler setColors
        do VSColorTheme.add_ThemeChanged handler
        interface IDisposable with member __.Dispose() = VSColorTheme.remove_ThemeChanged handler

        member __.GetColor(ctype) =
            let light, dark = colorData |> Map.ofList |> Map.find ctype
            match getCurrentThemeId() with
            | LightTheme -> Nullable light
            | DarkTheme -> Nullable dark
            | UnknownTheme -> Nullable()

        interface ISetThemeColors with member this.SetColors() = setColors()


    [<Export; Name(FSharpClassificationTypes.Function); BaseDefinition(PredefinedClassificationTypeNames.FormalLanguage)>]
    let FSharpFunctionClassificationType : ClassificationTypeDefinition = null

    [<Export; Name(FSharpClassificationTypes.MutableVar); BaseDefinition(PredefinedClassificationTypeNames.FormalLanguage)>]
    let FSharpMutableVarClassificationType : ClassificationTypeDefinition = null

    [<Export; Name(FSharpClassificationTypes.Printf); BaseDefinition(PredefinedClassificationTypeNames.FormalLanguage)>]
    let FSharpPrintfClassificationType : ClassificationTypeDefinition = null

    [<Export; Name(FSharpClassificationTypes.Property); BaseDefinition(PredefinedClassificationTypeNames.FormalLanguage)>]
    let FSharpPropertyClassificationType : ClassificationTypeDefinition = null

    [<Export; Name(FSharpClassificationTypes.Disposable); BaseDefinition(PredefinedClassificationTypeNames.FormalLanguage)>]
    let FSharpDisposableClassificationType : ClassificationTypeDefinition = null

    [<Export(typeof<EditorFormatDefinition>)>]
    [<ClassificationType(ClassificationTypeNames = FSharpClassificationTypes.Function)>]
    [<Name(FSharpClassificationTypes.Function)>]
    [<UserVisible(true)>]
    [<Order(After = PredefinedClassificationTypeNames.Keyword)>]
    type internal FSharpFunctionTypeFormat() as self =
        inherit ClassificationFormatDefinition()

        do self.DisplayName <- SR.FSharpFunctionsOrMethodsClassificationType()

    [<Export(typeof<EditorFormatDefinition>)>]
    [<ClassificationType(ClassificationTypeNames = FSharpClassificationTypes.MutableVar)>]
    [<Name(FSharpClassificationTypes.MutableVar)>]
    [<UserVisible(true)>]
    [<Order(After = PredefinedClassificationTypeNames.Keyword)>]
    type internal FSharpMutableVarTypeFormat [<ImportingConstructor>](theme: ThemeColors) as self =
        inherit ClassificationFormatDefinition()

        do self.DisplayName <- SR.FSharpMutableVarsClassificationType()
           self.ForegroundColor <- theme.GetColor FSharpClassificationTypes.MutableVar

    [<Export(typeof<EditorFormatDefinition>)>]
    [<ClassificationType(ClassificationTypeNames = FSharpClassificationTypes.Printf)>]
    [<Name(FSharpClassificationTypes.Printf)>]
    [<UserVisible(true)>]
    [<Order(After = PredefinedClassificationTypeNames.String)>]
    type internal FSharpPrintfTypeFormat [<ImportingConstructor>](theme: ThemeColors) as self =
        inherit ClassificationFormatDefinition()

        do self.DisplayName <- SR.FSharpPrintfFormatClassificationType()
           self.ForegroundColor <- theme.GetColor FSharpClassificationTypes.Printf

    [<Export(typeof<EditorFormatDefinition>)>]
    [<ClassificationType(ClassificationTypeNames = FSharpClassificationTypes.Property)>]
    [<Name(FSharpClassificationTypes.Property)>]
    [<UserVisible(true)>]
    [<Order(After = PredefinedClassificationTypeNames.Keyword)>]
    type internal FSharpPropertyFormat() as self =
        inherit ClassificationFormatDefinition()

        do self.DisplayName <- SR.FSharpPropertiesClassificationType()

    [<Export(typeof<EditorFormatDefinition>)>]
    [<ClassificationType(ClassificationTypeNames = FSharpClassificationTypes.Disposable)>]
    [<Name(FSharpClassificationTypes.Disposable)>]
    [<UserVisible(true)>]
    [<Order(After = PredefinedClassificationTypeNames.Keyword)>]
    type internal FSharpDisposableFormat [<ImportingConstructor>](theme: ThemeColors) as self =
        inherit ClassificationFormatDefinition()

        do self.DisplayName <- SR.FSharpDisposablesClassificationType()
           self.ForegroundColor <- theme.GetColor FSharpClassificationTypes.Disposable