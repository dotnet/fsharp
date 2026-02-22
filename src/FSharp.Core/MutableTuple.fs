// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Linq.RuntimeHelpers

open System
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open System.Collections.Generic

#nowarn "49" // no warning for uppercase variable names
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
[<Sealed>]
type AnonymousObject<'T1>(Item1: 'T1) =
    member _.Item1 = Item1

    override this.Equals(other: obj) =
        match other with
        | :? AnonymousObject<'T1> as o -> EqualityComparer<'T1>.Default.Equals(this.Item1, o.Item1)
        | _ -> false

    override this.GetHashCode() =
        EqualityComparer<'T1>.Default.GetHashCode(this.Item1)

/// <summary>This type shouldn't be used directly from user code.</summary>
/// <exclude />
[<Sealed>]
type AnonymousObject<'T1, 'T2>(Item1: 'T1, Item2: 'T2) =
    member _.Item1 = Item1
    member _.Item2 = Item2

    override this.Equals(other: obj) =
        match other with
        | :? AnonymousObject<'T1, 'T2> as o ->
            EqualityComparer<'T1>.Default.Equals(this.Item1, o.Item1)
            && EqualityComparer<'T2>.Default.Equals(this.Item2, o.Item2)
        | _ -> false

    override this.GetHashCode() =
        let h1 = EqualityComparer<'T1>.Default.GetHashCode(this.Item1)
        let h2 = EqualityComparer<'T2>.Default.GetHashCode(this.Item2)
        ((h1 <<< 5) + h1) ^^^ h2

/// <summary>This type shouldn't be used directly from user code.</summary>
/// <exclude />
[<Sealed>]
type AnonymousObject<'T1, 'T2, 'T3>(Item1: 'T1, Item2: 'T2, Item3: 'T3) =
    member _.Item1 = Item1
    member _.Item2 = Item2
    member _.Item3 = Item3

    override this.Equals(other: obj) =
        match other with
        | :? AnonymousObject<'T1, 'T2, 'T3> as o ->
            EqualityComparer<'T1>.Default.Equals(this.Item1, o.Item1)
            && EqualityComparer<'T2>.Default.Equals(this.Item2, o.Item2)
            && EqualityComparer<'T3>.Default.Equals(this.Item3, o.Item3)
        | _ -> false

    override this.GetHashCode() =
        let mutable hash = EqualityComparer<'T1>.Default.GetHashCode(this.Item1)
        hash <- ((hash <<< 5) + hash) ^^^ EqualityComparer<'T2>.Default.GetHashCode(this.Item2)
        hash <- ((hash <<< 5) + hash) ^^^ EqualityComparer<'T3>.Default.GetHashCode(this.Item3)
        hash

/// <summary>This type shouldn't be used directly from user code.</summary>
/// <exclude />
[<Sealed>]
type AnonymousObject<'T1, 'T2, 'T3, 'T4>(Item1: 'T1, Item2: 'T2, Item3: 'T3, Item4: 'T4) =
    member _.Item1 = Item1
    member _.Item2 = Item2
    member _.Item3 = Item3
    member _.Item4 = Item4

    override this.Equals(other: obj) =
        match other with
        | :? AnonymousObject<'T1, 'T2, 'T3, 'T4> as o ->
            EqualityComparer<'T1>.Default.Equals(this.Item1, o.Item1)
            && EqualityComparer<'T2>.Default.Equals(this.Item2, o.Item2)
            && EqualityComparer<'T3>.Default.Equals(this.Item3, o.Item3)
            && EqualityComparer<'T4>.Default.Equals(this.Item4, o.Item4)
        | _ -> false

    override this.GetHashCode() =
        let mutable hash = EqualityComparer<'T1>.Default.GetHashCode(this.Item1)
        hash <- ((hash <<< 5) + hash) ^^^ EqualityComparer<'T2>.Default.GetHashCode(this.Item2)
        hash <- ((hash <<< 5) + hash) ^^^ EqualityComparer<'T3>.Default.GetHashCode(this.Item3)
        hash <- ((hash <<< 5) + hash) ^^^ EqualityComparer<'T4>.Default.GetHashCode(this.Item4)
        hash

/// <summary>This type shouldn't be used directly from user code.</summary>
/// <exclude />
[<Sealed>]
type AnonymousObject<'T1, 'T2, 'T3, 'T4, 'T5>(Item1: 'T1, Item2: 'T2, Item3: 'T3, Item4: 'T4, Item5: 'T5) =
    member _.Item1 = Item1
    member _.Item2 = Item2
    member _.Item3 = Item3
    member _.Item4 = Item4
    member _.Item5 = Item5

    override this.Equals(other: obj) =
        match other with
        | :? AnonymousObject<'T1, 'T2, 'T3, 'T4, 'T5> as o ->
            EqualityComparer<'T1>.Default.Equals(this.Item1, o.Item1)
            && EqualityComparer<'T2>.Default.Equals(this.Item2, o.Item2)
            && EqualityComparer<'T3>.Default.Equals(this.Item3, o.Item3)
            && EqualityComparer<'T4>.Default.Equals(this.Item4, o.Item4)
            && EqualityComparer<'T5>.Default.Equals(this.Item5, o.Item5)
        | _ -> false

    override this.GetHashCode() =
        let mutable hash = EqualityComparer<'T1>.Default.GetHashCode(this.Item1)
        hash <- ((hash <<< 5) + hash) ^^^ EqualityComparer<'T2>.Default.GetHashCode(this.Item2)
        hash <- ((hash <<< 5) + hash) ^^^ EqualityComparer<'T3>.Default.GetHashCode(this.Item3)
        hash <- ((hash <<< 5) + hash) ^^^ EqualityComparer<'T4>.Default.GetHashCode(this.Item4)
        hash <- ((hash <<< 5) + hash) ^^^ EqualityComparer<'T5>.Default.GetHashCode(this.Item5)
        hash

/// <summary>This type shouldn't be used directly from user code.</summary>
/// <exclude />
[<Sealed>]
type AnonymousObject<'T1, 'T2, 'T3, 'T4, 'T5, 'T6>
    (Item1: 'T1, Item2: 'T2, Item3: 'T3, Item4: 'T4, Item5: 'T5, Item6: 'T6) =
    member _.Item1 = Item1
    member _.Item2 = Item2
    member _.Item3 = Item3
    member _.Item4 = Item4
    member _.Item5 = Item5
    member _.Item6 = Item6

    override this.Equals(other: obj) =
        match other with
        | :? AnonymousObject<'T1, 'T2, 'T3, 'T4, 'T5, 'T6> as o ->
            EqualityComparer<'T1>.Default.Equals(this.Item1, o.Item1)
            && EqualityComparer<'T2>.Default.Equals(this.Item2, o.Item2)
            && EqualityComparer<'T3>.Default.Equals(this.Item3, o.Item3)
            && EqualityComparer<'T4>.Default.Equals(this.Item4, o.Item4)
            && EqualityComparer<'T5>.Default.Equals(this.Item5, o.Item5)
            && EqualityComparer<'T6>.Default.Equals(this.Item6, o.Item6)
        | _ -> false

    override this.GetHashCode() =
        let mutable hash = EqualityComparer<'T1>.Default.GetHashCode(this.Item1)
        hash <- ((hash <<< 5) + hash) ^^^ EqualityComparer<'T2>.Default.GetHashCode(this.Item2)
        hash <- ((hash <<< 5) + hash) ^^^ EqualityComparer<'T3>.Default.GetHashCode(this.Item3)
        hash <- ((hash <<< 5) + hash) ^^^ EqualityComparer<'T4>.Default.GetHashCode(this.Item4)
        hash <- ((hash <<< 5) + hash) ^^^ EqualityComparer<'T5>.Default.GetHashCode(this.Item5)
        hash <- ((hash <<< 5) + hash) ^^^ EqualityComparer<'T6>.Default.GetHashCode(this.Item6)
        hash

/// <summary>This type shouldn't be used directly from user code.</summary>
/// <exclude />
[<Sealed>]
type AnonymousObject<'T1, 'T2, 'T3, 'T4, 'T5, 'T6, 'T7>
    (Item1: 'T1, Item2: 'T2, Item3: 'T3, Item4: 'T4, Item5: 'T5, Item6: 'T6, Item7: 'T7) =
    member _.Item1 = Item1
    member _.Item2 = Item2
    member _.Item3 = Item3
    member _.Item4 = Item4
    member _.Item5 = Item5
    member _.Item6 = Item6
    member _.Item7 = Item7

    override this.Equals(other: obj) =
        match other with
        | :? AnonymousObject<'T1, 'T2, 'T3, 'T4, 'T5, 'T6, 'T7> as o ->
            EqualityComparer<'T1>.Default.Equals(this.Item1, o.Item1)
            && EqualityComparer<'T2>.Default.Equals(this.Item2, o.Item2)
            && EqualityComparer<'T3>.Default.Equals(this.Item3, o.Item3)
            && EqualityComparer<'T4>.Default.Equals(this.Item4, o.Item4)
            && EqualityComparer<'T5>.Default.Equals(this.Item5, o.Item5)
            && EqualityComparer<'T6>.Default.Equals(this.Item6, o.Item6)
            && EqualityComparer<'T7>.Default.Equals(this.Item7, o.Item7)
        | _ -> false

    override this.GetHashCode() =
        let mutable hash = EqualityComparer<'T1>.Default.GetHashCode(this.Item1)
        hash <- ((hash <<< 5) + hash) ^^^ EqualityComparer<'T2>.Default.GetHashCode(this.Item2)
        hash <- ((hash <<< 5) + hash) ^^^ EqualityComparer<'T3>.Default.GetHashCode(this.Item3)
        hash <- ((hash <<< 5) + hash) ^^^ EqualityComparer<'T4>.Default.GetHashCode(this.Item4)
        hash <- ((hash <<< 5) + hash) ^^^ EqualityComparer<'T5>.Default.GetHashCode(this.Item5)
        hash <- ((hash <<< 5) + hash) ^^^ EqualityComparer<'T6>.Default.GetHashCode(this.Item6)
        hash <- ((hash <<< 5) + hash) ^^^ EqualityComparer<'T7>.Default.GetHashCode(this.Item7)
        hash

/// <summary>This type shouldn't be used directly from user code.</summary>
/// <exclude />
[<Sealed>]
type AnonymousObject<'T1, 'T2, 'T3, 'T4, 'T5, 'T6, 'T7, 'T8>
    (Item1: 'T1, Item2: 'T2, Item3: 'T3, Item4: 'T4, Item5: 'T5, Item6: 'T6, Item7: 'T7, Item8: 'T8) =
    member _.Item1 = Item1
    member _.Item2 = Item2
    member _.Item3 = Item3
    member _.Item4 = Item4
    member _.Item5 = Item5
    member _.Item6 = Item6
    member _.Item7 = Item7
    member _.Item8 = Item8

    override this.Equals(other: obj) =
        match other with
        | :? AnonymousObject<'T1, 'T2, 'T3, 'T4, 'T5, 'T6, 'T7, 'T8> as o ->
            EqualityComparer<'T1>.Default.Equals(this.Item1, o.Item1)
            && EqualityComparer<'T2>.Default.Equals(this.Item2, o.Item2)
            && EqualityComparer<'T3>.Default.Equals(this.Item3, o.Item3)
            && EqualityComparer<'T4>.Default.Equals(this.Item4, o.Item4)
            && EqualityComparer<'T5>.Default.Equals(this.Item5, o.Item5)
            && EqualityComparer<'T6>.Default.Equals(this.Item6, o.Item6)
            && EqualityComparer<'T7>.Default.Equals(this.Item7, o.Item7)
            && EqualityComparer<'T8>.Default.Equals(this.Item8, o.Item8)
        | _ -> false

    override this.GetHashCode() =
        let mutable hash = EqualityComparer<'T1>.Default.GetHashCode(this.Item1)
        hash <- ((hash <<< 5) + hash) ^^^ EqualityComparer<'T2>.Default.GetHashCode(this.Item2)
        hash <- ((hash <<< 5) + hash) ^^^ EqualityComparer<'T3>.Default.GetHashCode(this.Item3)
        hash <- ((hash <<< 5) + hash) ^^^ EqualityComparer<'T4>.Default.GetHashCode(this.Item4)
        hash <- ((hash <<< 5) + hash) ^^^ EqualityComparer<'T5>.Default.GetHashCode(this.Item5)
        hash <- ((hash <<< 5) + hash) ^^^ EqualityComparer<'T6>.Default.GetHashCode(this.Item6)
        hash <- ((hash <<< 5) + hash) ^^^ EqualityComparer<'T7>.Default.GetHashCode(this.Item7)
        hash <- ((hash <<< 5) + hash) ^^^ EqualityComparer<'T8>.Default.GetHashCode(this.Item8)
        hash
