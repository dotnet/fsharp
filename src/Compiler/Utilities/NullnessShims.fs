namespace Internal.Utilities.Library

open System

[<AutoOpen>]
module internal NullnessShims =

    let inline isNotNull (x: 'T) = not (isNull x)

#if NO_CHECKNULLS
    type 'T MaybeNull when 'T: not struct = 'T
    type objnull = obj

    let inline (^) (a: 'a) ([<InlineIfLambda>] b: 'a -> 'b) : 'b =
        match a with
        | null -> Unchecked.defaultof<'b>
        | _ -> b a

    let inline (|NonNullQuick|) (x: 'T MaybeNull) =
        match x with
        | null -> raise (NullReferenceException())
        | v -> v

    let inline nonNull (x: 'T MaybeNull) =
        match x with
        | null -> raise (NullReferenceException())
        | v -> v

    let inline (|Null|NonNull|) (x: 'T MaybeNull) : Choice<unit, 'T> =
        match x with
        | null -> Null
        | v -> NonNull v

    let inline nullArgCheck paramName (x: 'T MaybeNull) =
        if isNull (box x) then raise (ArgumentNullException(paramName))
        else x

    let inline (!!) x = x

    let inline defaultIfNull defaultValue arg = match arg with | null -> defaultValue | _ -> arg

    let inline nullSafeEquality (x: MaybeNull<'T>) (y: MaybeNull<'T>) ([<InlineIfLambda>]nonNullEqualityFunc:'T->'T->bool) =
        match box x, box y with
        | null, null -> true
        | null,_ | _, null -> false
        | _,_ -> nonNullEqualityFunc x y
#else
    type 'T MaybeNull when 'T: not null and 'T: not struct = 'T | null

    let inline (^) (a: 'a | null) ([<InlineIfLambda>] b: 'a -> 'b) : ('b | null) =
        match a with
        | Null -> null
        | NonNull v -> b v

    let inline (!!) (x:'T | null) = Unchecked.nonNull x

    let inline nullSafeEquality (x: MaybeNull<'T>) (y: MaybeNull<'T>) ([<InlineIfLambda>]nonNullEqualityFunc:'T->'T->bool) =
        match x, y with
        | null, null -> true
        | null,_ | _, null -> false
        | x, y -> nonNullEqualityFunc !!x !!y

#endif    



    [<return:Struct>]
    let inline (|NonEmptyString|_|) (x: string MaybeNull) =
        match x with
        | null -> ValueNone
        | v -> ValueSome v