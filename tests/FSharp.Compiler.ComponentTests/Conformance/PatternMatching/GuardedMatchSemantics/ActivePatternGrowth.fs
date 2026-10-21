module Test

type Context = {{CONTEXTS}}
let guard n value col = col <= value + n
let (|Identity|) value = value
let run token stack col =
    match token, stack with
    | 0, C0 value :: _ when col < value -> -1
{{GUARDS}}
{{TAILS}}
    | (31 | 32), _ :: _ when col >= 0 -> token
    | _ -> -1
