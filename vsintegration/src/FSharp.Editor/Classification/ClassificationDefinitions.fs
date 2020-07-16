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
    let [<Literal>] MutableVar = "FSharp.MutableVar"
    let [<Literal>] Disposable = "FSharp.Disposable"

    let getClassificationTypeName = function
        | SemanticClassificationType.MutableRecordField
        | SemanticClassificationType.MutableVar -> MutableVar
        | SemanticClassificationType.DisposableValue
        | SemanticClassificationType.DisposableType -> Disposable
        | SemanticClassificationType.NameSpace -> ClassificationTypeNames.NamespaceName
        | SemanticClassificationType.Exception
        | SemanticClassificationType.Module
        | SemanticClassificationType.Type
        | SemanticClassificationType.TypeDef
        | SemanticClassificationType.ConstructorForReferenceType
        | SemanticClassificationType.Printf
        | SemanticClassificationType.ReferenceType -> ClassificationTypeNames.ClassName
        | SemanticClassificationType.ConstructorForValueType
        | SemanticClassificationType.ValueType -> ClassificationTypeNames.StructName
        | SemanticClassificationType.ComputationExpression
        | SemanticClassificationType.IntrinsicFunction -> ClassificationTypeNames.Keyword
        | SemanticClassificationType.UnionCase
        | SemanticClassificationType.Enumeration -> ClassificationTypeNames.EnumName
        | SemanticClassificationType.Field
        | SemanticClassificationType.UnionCaseField -> ClassificationTypeNames.FieldName
        | SemanticClassificationType.Interface -> ClassificationTypeNames.InterfaceName
        | SemanticClassificationType.TypeArgument -> ClassificationTypeNames.TypeParameterName
        | SemanticClassificationType.Operator -> ClassificationTypeNames.Operator
        | SemanticClassificationType.Function
        | SemanticClassificationType.Method -> ClassificationTypeNames.MethodName
        | SemanticClassificationType.ExtensionMethod -> ClassificationTypeNames.ExtensionMethodName
        | SemanticClassificationType.Literal -> ClassificationTypeNames.ConstantName
        | SemanticClassificationType.Property
        | SemanticClassificationType.RecordFieldAsFunction
        | SemanticClassificationType.RecordField -> ClassificationTypeNames.PropertyName // TODO - maybe pick something that isn't white by default like Property?
        | SemanticClassificationType.NamedArgument -> ClassificationTypeNames.LabelName
        | SemanticClassificationType.Event -> ClassificationTypeNames.EventName
        | SemanticClassificationType.Delegate -> ClassificationTypeNames.DelegateName
        | SemanticClassificationType.Value -> ClassificationTypeNames.Identifier
        | SemanticClassificationType.LocalValue -> ClassificationTypeNames.LocalName

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

        let customColorData = // name,                (light,                            dark)
            [
                FSharpClassificationTypes.MutableVar, (Color.FromRgb(160uy, 128uy, 0uy), Color.FromRgb(255uy, 210uy, 28uy))
                FSharpClassificationTypes.Disposable, (Colors.Green,                     Color.FromRgb(2uy, 183uy, 43uy))
            ]

        let setColors _ =
            let fontAndColorStorage = serviceProvider.GetService(typeof<SVsFontAndColorStorage>) :?> IVsFontAndColorStorage
            let fontAndColorCacheManager = serviceProvider.GetService(typeof<SVsFontAndColorCacheManager>) :?> IVsFontAndColorCacheManager
            fontAndColorCacheManager.CheckCache( ref DefGuidList.guidTextEditorFontCategory) |> ignore
            fontAndColorStorage.OpenCategory(ref DefGuidList.guidTextEditorFontCategory, uint32 __FCSTORAGEFLAGS.FCSF_READONLY) |> ignore

            let formatMap = classificationformatMapService.GetClassificationFormatMap(category = "text")
            try 
                formatMap.BeginBatchUpdate()
                for ctype, (light, dark) in customColorData do
                    // we don't touch the changes made by the user
                    if fontAndColorStorage.GetItem(ctype, Array.zeroCreate 1) <> VSConstants.S_OK  then
                        let ict = classificationTypeRegistry.GetClassificationType(ctype)
                        let oldProps = formatMap.GetTextProperties(ict)
                        let newProps =
                            match getCurrentThemeId() with
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
            let light, dark = customColorData |> Map.ofList |> Map.find ctype
            match getCurrentThemeId() with
            | LightTheme -> Nullable light
            | DarkTheme -> Nullable dark
            | UnknownTheme -> Nullable()

        interface ISetThemeColors with member this.SetColors() = setColors()

    [<Export; Name(FSharpClassificationTypes.MutableVar); BaseDefinition(PredefinedClassificationTypeNames.FormalLanguage)>]
    let FSharpMutableVarClassificationType : ClassificationTypeDefinition = null


    [<Export; Name(FSharpClassificationTypes.Disposable); BaseDefinition(PredefinedClassificationTypeNames.FormalLanguage)>]
    let FSharpDisposableClassificationType : ClassificationTypeDefinition = null

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
    [<ClassificationType(ClassificationTypeNames = FSharpClassificationTypes.Disposable)>]
    [<Name(FSharpClassificationTypes.Disposable)>]
    [<UserVisible(true)>]
    [<Order(After = PredefinedClassificationTypeNames.Keyword)>]
    type internal FSharpDisposableFormat [<ImportingConstructor>](theme: ThemeColors) as self =
        inherit ClassificationFormatDefinition()

        do self.DisplayName <- SR.FSharpDisposablesClassificationType()
           self.ForegroundColor <- theme.GetColor FSharpClassificationTypes.Disposable