// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.CodeAnalysis

/// Classifies symbols that are related to a name resolution but are not the direct resolution result.
/// Used to report additional symbols (e.g., union case testers, copy-and-update record types)
/// via a separate sink so they don't corrupt colorization or symbol info.
[<System.Flags>]
type RelatedSymbolUseKind =
    /// No related symbols
    | None = 0
    /// Union case via tester property (e.g., .IsCaseA → CaseA)
    | UnionCaseTester = 1
    /// Record type via copy-and-update expression (e.g., { r with ... } → RecordType)
    | CopyAndUpdateRecord = 2
    /// Parameter or type parameter via the `name` of a `param`, `paramref`, `typeparam` or `typeparamref`
    /// tag in the declaration's XML doc
    | XmlDocParameter = 4
    /// Every related use in code; leaves out the names inside XML doc comments, which only a rename needs
    | AllInCode = 3
    /// Every related use, the names inside XML doc comments included
    | AllInCodeAndDocs = 7
    /// Every related use in code, the same as AllInCode
    | [<System.Obsolete("Use AllInCode, which this equals, or AllInCodeAndDocs to also get the names inside XML doc comments.")>] All = 3
