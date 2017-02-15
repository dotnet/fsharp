// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Linq;

namespace Microsoft.CodeAnalysis.PatternMatching
{
    /// <summary>
    /// Pattern matching results returned when calling <see cref="PatternMatcher.GetMatches(string, string)"/>
    /// Specifically, this type individually provides the matches produced when matching against the
    /// 'candidate' text and the 'container' text.
    /// </summary>
    internal struct PatternMatches
    {
        public static readonly PatternMatches Empty = new PatternMatches(null, null);

        public readonly PatternMatch[] CandidateMatches;
        public readonly PatternMatch[] ContainerMatches;

        public PatternMatches(PatternMatch[] candidateMatches,
                              PatternMatch[] containerMatches = null)
        {
            CandidateMatches = candidateMatches ?? new PatternMatch[0];
            ContainerMatches = containerMatches ?? new PatternMatch[0];
        }

        public bool IsEmpty => CandidateMatches.Length == 0 && ContainerMatches.Length == 0;

        internal bool All(Func<PatternMatch, bool> predicate)
        {
            return CandidateMatches.All(predicate) && ContainerMatches.All(predicate);
        }

        internal bool Any(Func<PatternMatch, bool> predicate)
        {
            return CandidateMatches.Any(predicate) || ContainerMatches.Any(predicate);
        }
    }
}