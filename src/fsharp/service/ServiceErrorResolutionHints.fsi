// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices

/// Exposes the string distance algorithm used to suggest names for mistyped identifiers.
module ErrorResolutionHints =
    /// Given a set of symbol uses and a string representing an unresolved identifier,
    /// returns a list of suggested names if there are any feasible candidates.
    val getSuggestedNames: symbolUses: FSharpSymbolUse[] -> unresolvedIdentifier: string -> string list option