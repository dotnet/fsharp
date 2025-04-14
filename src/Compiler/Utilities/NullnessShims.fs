namespace Internal.Utilities.Library

open System

[<AutoOpen>]
module internal NullnessShims =

    let inline isNotNull (x: 'T) = not (isNull x)

    type 'T MaybeNull when 'T: not null and 'T: not struct = 'T | null

    let inline (^) (a: 'a | null) ([<InlineIfLambda>] b: 'a -> 'b) : ('b | null) =
        match a with
        | Null -> null
        | NonNull v -> b v

    let inline (!!) (x: 'T | null) = Unchecked.nonNull x

    let inline nullSafeEquality (x: MaybeNull<'T>) (y: MaybeNull<'T>) ([<InlineIfLambda>] nonNullEqualityFunc: 'T -> 'T -> bool) =
        match x, y with
        | null, null -> true
        | null, _
        | _, null -> false
        | x, y -> nonNullEqualityFunc !!x !!y

#if BUILDING_WITH_LKG
    type ActivityDisposable = System.IDisposable
#else
    type ActivityDisposable = System.IDisposable | null
#endif

#if NET5_0_OR_GREATER
    // Argument type for overriding System.Object.Equals(arg)
    // Desktop frameworks as well as netstandard need plain 'obj' and are not annotated, NET5 and higher can use (obj|null)
    type objEqualsArg = objnull
#else
    type objEqualsArg = obj
#endif

    [<return: Struct>]
    let inline (|NonEmptyString|_|) (x: string MaybeNull) =
        match x with
        | null -> ValueNone
        | "" -> ValueNone
        | v -> ValueSome v
