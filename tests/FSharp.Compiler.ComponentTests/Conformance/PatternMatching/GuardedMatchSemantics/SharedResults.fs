module Test

type Context = A | B | C | D | E | F | G | H
let mutable calls = 0

{{DECLARATIONS}}

let {{INLINE}}run (kind, x, y) =
    {{LOCALS}}
    let result: {{TYPE}} = {{WRAP}} (
        match kind, x, y with
{{PREFIX_CLAUSES}}
        | (A | B | C | D | E | F | G | H), _, 0 when (
            calls <- calls + 1
            let value =
                match x % 13 with
                | 0 -> x + 1 | 1 -> x * 3 | 2 -> x - 5
                | 3 -> x / 7 | 4 -> x ^^^ 11 | 5 -> x + 13
                | 6 -> x * 17 | 7 -> x - 19 | 8 -> x / 23
                | 9 -> x ^^^ 29 | 10 -> x + 31 | 11 -> x * 37
                | _ -> x - 41
            value + y > 0) -> {{SUCCESS}}
{{TAILS}}){{APPLY}}
    {{OBSERVE}}

let check () =
    for i, kind in List.indexed [A; B; C; D; E; F; G; H] do
        for x, y, accepted in [0, 0, true; -8, 0, false; 3, 0, false; 4, 0, true; 8, 0, false; 12, 0, false; 1, 1, false; 99, 99, false] do
            calls <- 0
            if run (kind, x, y) <> ({{EXPECTED}}) then failwith "result continuation"
            if calls <> (if y = 0 then 1 else 0) + {{EXTRA_CALLS}} then failwith "guard observations"
    0

{{MAIN}}
