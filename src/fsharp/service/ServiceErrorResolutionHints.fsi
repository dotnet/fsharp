// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Diagnostics

/// Exposes the string distance algorithm used to suggest names for mistyped identifiers.
module ErrorResolutionHints =

    /// Given a set of names, uses and a string representing an unresolved identifier,
    /// returns a list of suggested names if there are any feasible candidates.
    val GetSuggestedNames: suggestionsF: ((string -> unit) -> unit) -> unresolvedIdentifier: string -> seq<string>
