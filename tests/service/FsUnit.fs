module FsUnit

open System.Diagnostics
open NUnit.Framework
open NUnit.Framework.Constraints

[<DebuggerNonUserCode>]
let should (f : 'a -> #Constraint) x (y : obj) =
    let c = f x
    let y =
        match y with
        | :? (unit -> unit) -> box (TestDelegate(y :?> unit -> unit))
        | _                 -> y
    Assert.That(y, c)

let equal x = EqualConstraint(x)

/// like "should equal", but validates same-type
let shouldEqual (x: 'a) (y: 'a) = Assert.AreEqual(x, y, sprintf "Expected: %A\nActual: %A" x y)

let notEqual x = NotConstraint(EqualConstraint(x))

let contain x = ContainsConstraint(x)

let haveLength n = Has.Length.EqualTo(n)

let haveCount n = Has.Count.EqualTo(n)

let endWith (s:string) = EndsWithConstraint(s)

let startWith (s:string) = StartsWithConstraint(s)

let be = id

let Null = NullConstraint()

let Empty = EmptyConstraint()

let EmptyString = EmptyStringConstraint()

let True = TrueConstraint()

let False = FalseConstraint()

let sameAs x = SameAsConstraint(x)

let throw = Throws.TypeOf