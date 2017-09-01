// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

using System.Collections.Immutable;

namespace Roslyn.Utilities
{
    internal static class ImmutableArrayExtensions
    {
        public static ImmutableArray<T> NullToEmpty<T>(this ImmutableArray<T> array)
        {
            return array.IsDefault ? ImmutableArray<T>.Empty : array;
        }
    }
}