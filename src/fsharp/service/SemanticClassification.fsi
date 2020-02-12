// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SourceCodeServices

open FSharp.Compiler
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.Tastops
open FSharp.Compiler.Range
open FSharp.Compiler.NameResolution
open FSharp.Compiler.TcGlobals

[<RequireQualifiedAccess>]
type SemanticClassificationType =
    | ReferenceType
    | ValueType
    | UnionCase
    | Function
    | Property
    | MutableVar
    | Module
    | Printf
    | ComputationExpression
    | IntrinsicFunction
    | Enumeration
    | Interface
    | TypeArgument
    | Operator
    | Disposable

/// Extension methods for the TcResolutions type.
[<AutoOpen>]
module internal TcResolutionsExtensions =

    val (|CNR|) : cnr: CapturedNameResolution -> (pos * Item * ItemOccurence * DisplayEnv * NameResolutionEnv * AccessorDomain * range)

    type TcResolutions with

        member GetSemanticClassification: g: TcGlobals * amap: Import.ImportMap * formatSpecifierLocations: (range * int) [] * range: range option -> struct(range * SemanticClassificationType) []