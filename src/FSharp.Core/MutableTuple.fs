// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Linq.RuntimeHelpers

#nowarn "49" // no warning for uppercase variable names
open Microsoft.FSharp.Core

// ----------------------------------------------------------------------------
// Mutable Tuples - used when translating queries that use F# tuples
// and records. We replace tuples/records with anonymous types which
// are handled correctly by LINQ to SQL/Entities and other providers.
//
// NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE
//
// The terminology "mutable tuple" is now incorrect in this code -
// "immutable anonymous tuple-like types" are used instead. The key thing in this
// code is that the anonymous types used conform to the shape and style
// expected by LINQ providers, and we pass the correspondence between constructor
// arguments and properties to the magic "members" argument of the Expression.New
// constructor in Linq.fs.
//
// This terminology mistake also runs all the way through Query.fs.
// ----------------------------------------------------------------------------

/// <summary>This type shouldn't be used directly from user code.</summary>
/// <exclude />
type AnonymousObject<'T1> =
    val private item1: 'T1
    member x.Item1 = x.item1

    new(Item1) = { item1 = Item1 }

/// <summary>This type shouldn't be used directly from user code.</summary>
/// <exclude />
type AnonymousObject<'T1, 'T2> =
    val private item1: 'T1
    member x.Item1 = x.item1

    val private item2: 'T2
    member x.Item2 = x.item2

    new(Item1, Item2) = { item1 = Item1; item2 = Item2 }

/// <summary>This type shouldn't be used directly from user code.</summary>
/// <exclude />
type AnonymousObject<'T1, 'T2, 'T3> =
    val private item1: 'T1
    member x.Item1 = x.item1

    val private item2: 'T2
    member x.Item2 = x.item2

    val private item3: 'T3
    member x.Item3 = x.item3

    new(Item1, Item2, Item3) =
        {
            item1 = Item1
            item2 = Item2
            item3 = Item3
        }

/// <summary>This type shouldn't be used directly from user code.</summary>
/// <exclude />
type AnonymousObject<'T1, 'T2, 'T3, 'T4> =
    val private item1: 'T1
    member x.Item1 = x.item1

    val private item2: 'T2
    member x.Item2 = x.item2

    val private item3: 'T3
    member x.Item3 = x.item3

    val private item4: 'T4
    member x.Item4 = x.item4

    new(Item1, Item2, Item3, Item4) =
        {
            item1 = Item1
            item2 = Item2
            item3 = Item3
            item4 = Item4
        }

/// <summary>This type shouldn't be used directly from user code.</summary>
/// <exclude />
type AnonymousObject<'T1, 'T2, 'T3, 'T4, 'T5> =
    val private item1: 'T1
    member x.Item1 = x.item1

    val private item2: 'T2
    member x.Item2 = x.item2

    val private item3: 'T3
    member x.Item3 = x.item3

    val private item4: 'T4
    member x.Item4 = x.item4

    val private item5: 'T5
    member x.Item5 = x.item5

    new(Item1, Item2, Item3, Item4, Item5) =
        {
            item1 = Item1
            item2 = Item2
            item3 = Item3
            item4 = Item4
            item5 = Item5
        }

/// <summary>This type shouldn't be used directly from user code.</summary>
/// <exclude />
type AnonymousObject<'T1, 'T2, 'T3, 'T4, 'T5, 'T6> =
    val private item1: 'T1
    member x.Item1 = x.item1

    val private item2: 'T2
    member x.Item2 = x.item2

    val private item3: 'T3
    member x.Item3 = x.item3

    val private item4: 'T4
    member x.Item4 = x.item4

    val private item5: 'T5
    member x.Item5 = x.item5

    val private item6: 'T6
    member x.Item6 = x.item6

    new(Item1, Item2, Item3, Item4, Item5, Item6) =
        {
            item1 = Item1
            item2 = Item2
            item3 = Item3
            item4 = Item4
            item5 = Item5
            item6 = Item6
        }

/// <summary>This type shouldn't be used directly from user code.</summary>
/// <exclude />
type AnonymousObject<'T1, 'T2, 'T3, 'T4, 'T5, 'T6, 'T7> =
    val private item1: 'T1
    member x.Item1 = x.item1

    val private item2: 'T2
    member x.Item2 = x.item2

    val private item3: 'T3
    member x.Item3 = x.item3

    val private item4: 'T4
    member x.Item4 = x.item4

    val private item5: 'T5
    member x.Item5 = x.item5

    val private item6: 'T6
    member x.Item6 = x.item6

    val private item7: 'T7
    member x.Item7 = x.item7

    new(Item1, Item2, Item3, Item4, Item5, Item6, Item7) =
        {
            item1 = Item1
            item2 = Item2
            item3 = Item3
            item4 = Item4
            item5 = Item5
            item6 = Item6
            item7 = Item7
        }

/// <summary>This type shouldn't be used directly from user code.</summary>
/// <exclude />
type AnonymousObject<'T1, 'T2, 'T3, 'T4, 'T5, 'T6, 'T7, 'T8> =
    val private item1: 'T1
    member x.Item1 = x.item1

    val private item2: 'T2
    member x.Item2 = x.item2

    val private item3: 'T3
    member x.Item3 = x.item3

    val private item4: 'T4
    member x.Item4 = x.item4

    val private item5: 'T5
    member x.Item5 = x.item5

    val private item6: 'T6
    member x.Item6 = x.item6

    val private item7: 'T7
    member x.Item7 = x.item7

    val private item8: 'T8
    member x.Item8 = x.item8

    new(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8) =
        {
            item1 = Item1
            item2 = Item2
            item3 = Item3
            item4 = Item4
            item5 = Item5
            item6 = Item6
            item7 = Item7
            item8 = Item8
        }
