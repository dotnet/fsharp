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
    let [<Literal>] Interface = ClassificationTypeNames.InterfaceName
    let [<Literal>] TypeArgument = ClassificationTypeNames.TypeParameterName
    let [<Literal>] Operator = ClassificationTypeNames.Operator

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

module internal ClassificationDefinitions =
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
    type internal FSharpFunctionTypeFormat() as self =
        inherit ClassificationFormatDefinition()
        // Not setting any colors here, so it will inherit from "Plain Text" by default
        do self.DisplayName <- SR.FSharpFunctionsOrMethodsClassificationType.Value

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
