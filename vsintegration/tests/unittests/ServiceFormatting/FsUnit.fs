// Copied from https://github.com/dungpa/fantomas and modified by Vasily Kirichenko

module FSharp.Compiler.Service.Tests.ServiceFormatting.FsUnit

open NUnit.Framework
open NUnit.Framework.Constraints

[<AutoOpen>]
module TopLevelOperators =
    let Null = NullConstraint()
    let Empty = EmptyConstraint()
    let EmptyString = EmptyStringConstraint()
    let True = TrueConstraint()
    let False = FalseConstraint()
    let NaN = NaNConstraint()
    let unique = UniqueItemsConstraint()

    let should (f : 'a -> #Constraint) x (y : obj) =
        let c = f x
        let y =
            match y with
            | :? (unit -> unit) -> box (TestDelegate(y :?> unit -> unit))
            | _ -> y
        Assert.That(y, c)
    
    let equal x = EqualConstraint(x)
    let equalWithin tolerance x = equal(x).Within tolerance
    let contain x = ContainsConstraint(x)
    let haveLength n = Has.Length.EqualTo(n)
    let haveCount n = Has.Count.EqualTo(n)
    let be = id
    let sameAs x = SameAsConstraint(x)
    let throw = Throws.TypeOf
    let throwWithMessage (m:string) (t:System.Type) = Throws.TypeOf(t).And.Message.EqualTo(m)
    let greaterThan x = GreaterThanConstraint(x)
    let greaterThanOrEqualTo x = GreaterThanOrEqualConstraint(x)
    let lessThan x = LessThanConstraint(x)
    let lessThanOrEqualTo x = LessThanOrEqualConstraint(x)
    
    let shouldFail (f : unit -> unit) =
        TestDelegate(f) |> should throw typeof<AssertionException>

    let endWith (s:string) = EndsWithConstraint s
    let startWith (s:string) = StartsWithConstraint s
    let ofExactType<'a> = ExactTypeConstraint(typeof<'a>)
    let instanceOfType<'a> = InstanceOfTypeConstraint(typeof<'a>)
    let ascending = Is.Ordered
    let descending = Is.Ordered.Descending
    let not' x = NotConstraint(x)

    /// Deprecated operators. These will be removed in a future version of FsUnit.
    module FsUnitDepricated =
        [<System.Obsolete>]
        let not x = not' x