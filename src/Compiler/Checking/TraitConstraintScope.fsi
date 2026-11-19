// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.TraitConstraintScope

open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.InfoReader
open FSharp.Compiler.Infos
open FSharp.Compiler.NameResolution
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree

/// Capture an extension scope whose identities follow signature remapping.
val CreateTraitContext:
    selectExtensionMethods: (TraitConstraintInfo * range * NameResolutionEnv * InfoReader -> (TType * MethInfo) list) ->
    nenv: Lazy<NameResolutionEnv> ->
    ad: AccessorDomain ->
        ITraitContext<AccessorDomain, MethInfo, InfoReader>
