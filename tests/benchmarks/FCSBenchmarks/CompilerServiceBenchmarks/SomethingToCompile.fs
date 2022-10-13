
let SomethingToCompile f =

    let inline map
        (mapper: 'okInput -> 'okOutput)
        (input: Result<'okInput, 'error>)
        =
        match input with
        | Ok x -> Ok(f (mapper x))
        | Error e -> Error e

    let inline mapError
        (errorMapper: 'errorInput -> 'errorOutput)
        (input: Result<'ok, 'errorInput>)
        : Result<'ok, 'errorOutput> =
        match input with
        | Ok x -> Ok x
        | Error e -> Error(errorMapper e)

    let inline bind
        (binder: 'okInput -> Result<'okOutput, 'error>)
        (input: Result<'okInput, 'error>)
        : Result<'okOutput, 'error> =
        match input with
        | Ok x -> binder x
        | Error e -> Error e

    let inline isOk (value: Result<'ok, 'error>) : bool =
        match value with
        | Ok _ -> true
        | Error _ -> false

    let inline isError (value: Result<'ok, 'error>) : bool =
        match value with
        | Ok _ -> false
        | Error _ -> true

    let inline either
        (onOk: 'okInput -> 'output)
        (onError: 'errorInput -> 'output)
        (input: Result<'okInput, 'errorInput>)
        : 'output =
        match input with
        | Ok x -> onOk x
        | Error err -> onError err

    let inline eitherMap
        (onOk: 'okInput -> 'okOutput)
        (onError: 'errorInput -> 'errorOutput)
        (input: Result<'okInput, 'errorInput>)
        : Result<'okOutput, 'errorOutput> =
        match input with
        | Ok x -> Ok(onOk x)
        | Error err -> Error(onError err)

    let inline apply
        (applier: Result<'okInput -> 'okOutput, 'error>)
        (input: Result<'okInput, 'error>)
        : Result<'okOutput, 'error> =
        match (applier, input) with
        | Ok f, Ok x -> Ok(f x)
        | Error e, _
        | _, Error e -> Error e

    let inline map2
        (mapper: 'okInput1 -> 'okInput2 -> 'okOutput)
        (input1: Result<'okInput1, 'error>)
        (input2: Result<'okInput2, 'error>)
        : Result<'okOutput, 'error> =
        match (input1, input2) with
        | Ok x, Ok y -> Ok(mapper x y)
        | Error e, _
        | _, Error e -> Error e


    let inline map3
        (mapper: 'okInput1 -> 'okInput2 -> 'okInput3 -> 'okOutput)
        (input1: Result<'okInput1, 'error>)
        (input2: Result<'okInput2, 'error>)
        (input3: Result<'okInput3, 'error>)
        : Result<'okOutput, 'error> =
        match (input1, input2, input3) with
        | Ok x, Ok y, Ok z -> Ok(mapper x y z)
        | Error e, _, _
        | _, Error e, _
        | _, _, Error e -> Error e

    let inline fold
        (onOk: 'okInput -> 'output)
        (onError: 'errorInput -> 'output)
        (input: Result<'okInput, 'errorInput>)
        : 'output =
        match input with
        | Ok x -> onOk x
        | Error err -> onError err

    let inline ofChoice (input: Choice<'ok, 'error>) : Result<'ok, 'error> =
        match input with
        | Choice1Of2 x -> Ok x
        | Choice2Of2 e -> Error e

    let inline tryCreate (fieldName: string) (x: 'a) : Result< ^b, (string * 'c) > =
        let tryCreate' x =
            (^b: (static member TryCreate: 'a -> Result< ^b, 'c >) x)

        tryCreate' x |> mapError (fun z -> (fieldName, z))


    let inline orElse (ifError: Result<'ok, 'errorOutput>) (result: Result<'ok, 'error>) : Result<'ok, 'errorOutput> =
        match result with
        | Ok x -> Ok x
        | Error e -> ifError


    let inline orElseWith
        (ifErrorFunc: 'error -> Result<'ok, 'errorOutput>)
        (result: Result<'ok, 'error>)
        : Result<'ok, 'errorOutput> =
        match result with
        | Ok x -> Ok x
        | Error e -> ifErrorFunc e

    let inline ignore (result: Result<'ok, 'error>) : Result<unit, 'error> =
        match result with
        | Ok _ -> Ok()
        | Error e -> Error e

    let inline requireTrue (error: 'error) (value: bool) : Result<unit, 'error> = if value then Ok() else Error error

    let inline requireFalse (error: 'error) (value: bool) : Result<unit, 'error> =
        if not value then Ok() else Error error

    let inline requireSome (error: 'error) (option: 'ok option) : Result<'ok, 'error> =
        match option with
        | Some x -> Ok x
        | None -> Error error

    let inline requireNone (error: 'error) (option: 'value option) : Result<unit, 'error> =
        match option with
        | Some _ -> Error error
        | None -> Ok()

    let inline requireNotNull (error: 'error) (value: 'ok) : Result<'ok, 'error> =
        match value with
        | null -> Error error
        | nonnull -> Ok nonnull

    let inline requireEqualTo (other: 'value) (error: 'error) (this: 'value) : Result<unit, 'error> =
        if this = other then
            Ok()
        else
            Error error

    let inline requireEqual (x1: 'value) (x2: 'value) (error: 'error) : Result<unit, 'error> =
        if x1 = x2 then Ok() else Error error

    let inline requireEmpty (error: 'error) (xs: #seq<'value>) : Result<unit, 'error> =
        if Seq.isEmpty xs then
            Ok()
        else
            Error error

    let inline requireNotEmpty (error: 'error) (xs: #seq<'value>) : Result<unit, 'error> =
        if Seq.isEmpty xs then
            Error error
        else
            Ok()

    let inline requireHead (error: 'error) (xs: #seq<'ok>) : Result<'ok, 'error> =
        match Seq.tryHead xs with
        | Some x -> Ok x
        | None -> Error error

    let inline setError (error: 'error) (result: Result<'ok, 'errorIgnored>) : Result<'ok, 'error> =
        result |> mapError (fun _ -> error)

    let inline withError (error: 'error) (result: Result<'ok, unit>) : Result<'ok, 'error> =
        result |> mapError (fun () -> error)

    let inline defaultValue (ifError: 'ok) (result: Result<'ok, 'error>) : 'ok =
        match result with
        | Ok x -> x
        | Error _ -> ifError

    let inline defaultError (ifOk: 'error) (result: Result<'ok, 'error>) : 'error =
        match result with
        | Error error -> error
        | Ok _ -> ifOk

    let inline defaultWith (ifErrorThunk: unit -> 'ok) (result: Result<'ok, 'error>) : 'ok =
        match result with
        | Ok x -> x
        | Error _ -> ifErrorThunk ()

    let inline ignoreError (result: Result<unit, 'error>) : unit = defaultValue () result

    let inline teeIf
        (predicate: 'ok -> bool)
        (inspector: 'ok -> unit)
        (result: Result<'ok, 'error>)
        : Result<'ok, 'error> =
        match result with
        | Ok x -> if predicate x then inspector x
        | Error _ -> ()

        result

    let inline teeErrorIf
        (predicate: 'error -> bool)
        (inspector: 'error -> unit)
        (result: Result<'ok, 'error>)
        : Result<'ok, 'error> =
        match result with
        | Ok _ -> ()
        | Error x -> if predicate x then inspector x

        result

    let inline tee (inspector: 'ok -> unit) (result: Result<'ok, 'error>) : Result<'ok, 'error> =
        teeIf (fun _ -> true) inspector result

    let inline teeError
        (inspector: 'error -> unit)
        (result: Result<'ok, 'error>)
        : Result<'ok, 'error> =
        teeErrorIf (fun _ -> true) inspector result

    let inline valueOr (f: 'error -> 'ok) (res: Result<'ok, 'error>) : 'ok =
        match res with
        | Ok x -> x
        | Error x -> f x

    let zip (left: Result<'leftOk, 'error>) (right: Result<'rightOk, 'error>) : Result<'leftOk * 'rightOk, 'error> =
        match left, right with
        | Ok x1res, Ok x2res -> Ok(x1res, x2res)
        | Error e, _ -> Error e
        | _, Error e -> Error e

    let zipError
        (left: Result<'ok, 'leftError>)
        (right: Result<'ok, 'rightError>)
        : Result<'ok, 'leftError * 'rightError> =
        match left, right with
        | Error x1res, Error x2res -> Error(x1res, x2res)
        | Ok e, _ -> Ok e
        | _, Ok e -> Ok e

    map id

let whatever x = SomethingToCompile f x

