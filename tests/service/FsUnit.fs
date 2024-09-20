module FsUnit

open System.Diagnostics
open Xunit
open NUnit.Framework.Constraints

[<DebuggerNonUserCode>]
let should (f: 'a -> objnull -> unit) x (y: obj) =
    let y =
        match y with
        | :? (unit -> unit) -> box (fun () -> (y :?> unit -> unit))
        | _                 -> y
    f x y

/// Note, xunit does check types by default.
/// These are artifacts of nunit and not necessary now, just used in many places.
let equal (expected: 'a) (actual: 'a) = 
    Assert.Equal<'a>(expected, actual)

let shouldEqual (x: 'a) (y: 'a) =
    Assert.Equal<'a>(x, y)

/// Same as 'shouldEqual' but goes pairwise over the collections. Lengths must be equal.
let shouldPairwiseEqual (x: seq<_>) (y: seq<_>) =
    // using enumerators, because Seq.iter2 allows different lengths silently
    let ex = x.GetEnumerator()
    let ey = y.GetEnumerator()
    let mutable countx = 0
    let mutable county = 0
    while ex.MoveNext() do
        countx <- countx + 1
        if ey.MoveNext() then
            county <- county + 1
            ey.Current |> shouldEqual ex.Current

    while ex.MoveNext() do countx <- countx + 1
    while ey.MoveNext() do county <- county + 1
    if countx <> county then        
        Assert.Fail($"Collections are of unequal lengths, expected length {countx}, actual length is {county}.")

let sameAs x = SameAsConstraint(x)
