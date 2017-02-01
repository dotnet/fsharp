// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel.Composition
open System.Windows.Media

open Microsoft.VisualStudio.Language.StandardClassification
open Microsoft.VisualStudio.Text.Classification
open Microsoft.VisualStudio.Utilities
open Microsoft.CodeAnalysis.Classification

open Microsoft.FSharp.Compiler.SourceCodeServices

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

    let getClassificationTypeName = function
        | SemanticClassificationType.ReferenceType
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

module internal ClassificationDefinitions =
    [<Export; Name(FSharpClassificationTypes.Function); BaseDefinition("identifier")>]
    let FSharpFunctionClassificationType : ClassificationTypeDefinition = null

    [<Export; Name(FSharpClassificationTypes.MutableVar); BaseDefinition("identifier")>]
    let FSharpMutableVarClassificationType : ClassificationTypeDefinition = null

    [<Export; Name(FSharpClassificationTypes.Printf); BaseDefinition("identifier")>]
    let FSharpPrintfClassificationType : ClassificationTypeDefinition = null

    [<Export; Name(FSharpClassificationTypes.Property); BaseDefinition("identifier")>]
    let FSharpPropertyClassificationType : ClassificationTypeDefinition = null

    [<Export(typeof<EditorFormatDefinition>)>]
    [<ClassificationType(ClassificationTypeNames = FSharpClassificationTypes.Function)>]
    [<Name(FSharpClassificationTypes.Function)>]
    [<UserVisible(true)>]
    [<Order(After = PredefinedClassificationTypeNames.Keyword)>]
    type internal FSharpFunctionTypeFormat() as self =
        inherit ClassificationFormatDefinition()
        
        do self.DisplayName <- SR.FSharpFunctionsOrMethodsClassificationType.Value
           self.ForegroundColor <- Nullable Colors.Black

    [<Export(typeof<EditorFormatDefinition>)>]
    [<ClassificationType(ClassificationTypeNames = FSharpClassificationTypes.MutableVar)>]
    [<Name(FSharpClassificationTypes.MutableVar)>]
    [<UserVisible(true)>]
    [<Order(After = PredefinedClassificationTypeNames.Keyword)>]
    type internal FSharpMutableVarTypeFormat() as self =
        inherit ClassificationFormatDefinition()
        
        do self.DisplayName <- SR.FSharpMutableVarsClassificationType.Value
           self.ForegroundColor <- Nullable Colors.Red

    [<Export(typeof<EditorFormatDefinition>)>]
    [<ClassificationType(ClassificationTypeNames = FSharpClassificationTypes.Printf)>]
    [<Name(FSharpClassificationTypes.Printf)>]
    [<UserVisible(true)>]
    [<Order(After = PredefinedClassificationTypeNames.String)>]
    type internal FSharpPrintfTypeFormat() as self =
        inherit ClassificationFormatDefinition()
        
        do self.DisplayName <- SR.FSharpPrintfFormatClassificationType.Value
           self.ForegroundColor <- Nullable (Color.FromRgb(43uy, 145uy, 175uy))
    
    [<Export(typeof<EditorFormatDefinition>)>]
    [<ClassificationType(ClassificationTypeNames = FSharpClassificationTypes.Property)>]
    [<Name(FSharpClassificationTypes.Property)>]
    [<UserVisible(true)>]
    [<Order(After = PredefinedClassificationTypeNames.Keyword)>]
    type internal FSharpPropertyFormat() as self =
        inherit ClassificationFormatDefinition()
        
        do self.DisplayName <- SR.FSharpPropertiesClassificationType.Value
           self.ForegroundColor <- Nullable Colors.Black