// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.CodeAnalysis.PatternMatching
{
    /// <summary>
    /// Note(cyrusn): this enum is ordered from strongest match type to weakest match type.
    /// </summary>
    internal enum PatternMatchKind
    {
        /// <summary>
        /// The candidate string matched the pattern exactly.
        /// </summary>
        Exact,

        /// <summary>
        /// The pattern was a prefix of the candidate string.
        /// </summary>
        Prefix,

        /// <summary>
        /// The pattern was a substring of the candidate string, but in a way that wasn't a CamelCase match.
        /// </summary>
        Substring,

        /// <summary>
        /// The pattern matched the CamelCased candidate string.
        /// </summary>
        CamelCase,

        /// <summary>
        /// The pattern matches the candidate in a fuzzy manner.  Fuzzy matching allows for 
        /// misspellings, missing words, etc.
        /// </summary>
        Fuzzy
    }
}
