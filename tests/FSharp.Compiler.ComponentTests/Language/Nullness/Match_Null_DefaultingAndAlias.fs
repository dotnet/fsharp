module Nullness.Match_Null_DefaultingAndAlias

type objnull = obj | null
type stringnull = string | null

// 1) Defaulting case: result unconstrained; null pattern forces a nullable top type.
let getEnvDefault (_: string) = failwith ""

let valueDefault =
    match "ENVVAR" |> getEnvDefault with
    | null -> "missing"
    | x -> x.ToString() // x must be refined to obj (non-null)

// 2) Alias to obj | null
let getEnvAliasObj (_: string) : objnull = failwith "stub"

let valueAliasObj =
    match getEnvAliasObj "ENVVAR" with
    | null -> "missing"
    | x -> x.ToString() // x must be refined to obj (non-null)

// 3) Alias to string | null
let getEnvAliasStr (_: string) : stringnull = failwith "stub"

let valueAliasStr =
    match getEnvAliasStr "ENVVAR" with
    | null -> 0
    | s -> s.Length // s must be refined to string (non-null)