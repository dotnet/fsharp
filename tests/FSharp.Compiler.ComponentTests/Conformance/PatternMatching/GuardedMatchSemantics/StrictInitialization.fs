module Test

let run gate token flag =
    let rec value: int =
        match token, flag with
        | (0 | 1), true when gate -> 0
        | _ -> value + 1
    value
