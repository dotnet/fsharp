module internal FSharp.Compiler.EnvironmentHelpers

open System

let isEnvVarTruthy (name: string) =
    match Environment.GetEnvironmentVariable(name) with
    | null
    | "" -> false
    | value when String.Equals(value, "1", StringComparison.OrdinalIgnoreCase) -> true
    | value when String.Equals(value, "true", StringComparison.OrdinalIgnoreCase) -> true
    | _ -> false
