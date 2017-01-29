// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open Microsoft.VisualStudio.Language.StandardClassification
//open Microsoft.VisualStudio.PlatformUI
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Classification
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.Text.Formatting
open Microsoft.VisualStudio.Text.Tagging
open Microsoft.VisualStudio.Utilities
open System
open System.Collections.Generic
open System.ComponentModel.Composition
open System.Windows.Media
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.CodeAnalysis.Classification

module internal ClassificationTypes =
    [<Export; Name(FSharpClassificationTypes.UnionCase); BaseDefinition("identifier")>]
    let FSharpPatternCaseClassificationType : ClassificationTypeDefinition = null

    [<Export; Name(FSharpClassificationTypes.Function); BaseDefinition("identifier")>]
    let FSharpFunctionClassificationType : ClassificationTypeDefinition = null

    [<Export; Name(FSharpClassificationTypes.MutableVar); BaseDefinition("identifier")>]
    let FSharpMutableVarClassificationType : ClassificationTypeDefinition = null

    [<Export; Name(FSharpClassificationTypes.Printf); BaseDefinition("identifier")>]
    let FSharpPrintfClassificationType : ClassificationTypeDefinition = null

    [<Export(typeof<EditorFormatDefinition>)>]
    [<ClassificationType(ClassificationTypeNames = FSharpClassificationTypes.UnionCase)>]
    [<Name(FSharpClassificationTypes.UnionCase)>]
    [<UserVisible(true)>]
    [<Order(After = PredefinedClassificationTypeNames.String)>] 
    type internal FSharpUnionCaseFormat [<ImportingConstructor>](colorManager: ClassificationColorManager) as self =
        inherit ClassificationFormatDefinition()
        
        do self.DisplayName <- "F# Unions / Active Patterns"
           self.ForegroundColor <- colorManager.GetDefaultColors(FSharpClassificationTypes.UnionCase)

    [<Export(typeof<EditorFormatDefinition>)>]
    [<ClassificationType(ClassificationTypeNames = FSharpClassificationTypes.Function)>]
    [<Name(FSharpClassificationTypes.Function)>]
    [<UserVisible(true)>]
    [<Order(After = PredefinedClassificationTypeNames.String)>] 
    type internal FSharpFunctionTypeFormat [<ImportingConstructor>](colorManager: ClassificationColorManager) as self =
        inherit ClassificationFormatDefinition()
        
        do self.DisplayName <- "F# Functions / Methods"
           self.ForegroundColor <- colorManager.GetDefaultColors(FSharpClassificationTypes.Function)

    [<Export(typeof<EditorFormatDefinition>)>]
    [<ClassificationType(ClassificationTypeNames = FSharpClassificationTypes.MutableVar)>]
    [<Name(FSharpClassificationTypes.MutableVar)>]
    [<UserVisible(true)>]
    [<Order(After = PredefinedClassificationTypeNames.String)>] 
    type internal FSharpMutableVarTypeFormat [<ImportingConstructor>](colorManager: ClassificationColorManager) as self =
        inherit ClassificationFormatDefinition()
        
        do self.DisplayName <- "F# Mutable Variables / Reference Cells"
           self.ForegroundColor <- colorManager.GetDefaultColors(FSharpClassificationTypes.MutableVar)

    [<Export(typeof<EditorFormatDefinition>)>]
    [<ClassificationType(ClassificationTypeNames = FSharpClassificationTypes.Printf)>]
    [<Name(FSharpClassificationTypes.Printf)>]
    [<UserVisible(true)>]
    [<Order(After = PredefinedClassificationTypeNames.String)>] 
    type internal FSharpPrintfTypeFormat [<ImportingConstructor>](colorManager: ClassificationColorManager) as self =
        inherit ClassificationFormatDefinition()
        
        do self.DisplayName <- "F# Printf Format"
           self.ForegroundColor <- colorManager.GetDefaultColors(FSharpClassificationTypes.Printf)
    
    
    let getClassificationTypeName = function
        | SemanticClassificationType.ReferenceType
        | SemanticClassificationType.Module -> FSharpClassificationTypes.Module
        | SemanticClassificationType.ValueType -> FSharpClassificationTypes.ValueType
        | SemanticClassificationType.UnionCase -> FSharpClassificationTypes.UnionCase
        | SemanticClassificationType.Function -> FSharpClassificationTypes.Function
        | SemanticClassificationType.MutableVar -> FSharpClassificationTypes.MutableVar
        | SemanticClassificationType.Printf -> FSharpClassificationTypes.Printf
        | SemanticClassificationType.ComputationExpression
        | SemanticClassificationType.IntrinsicType -> FSharpClassificationTypes.Keyword
        | SemanticClassificationType.Enumeration -> FSharpClassificationTypes.Enum