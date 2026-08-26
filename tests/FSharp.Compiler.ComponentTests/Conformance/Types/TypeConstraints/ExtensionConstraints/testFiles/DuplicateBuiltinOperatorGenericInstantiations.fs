module Test

// Two same-signature extension operators on a GENERIC type (ResizeArray<'T>) in two modules.
// Regression for an instantiation-blind optimizer-replay key: list<int> and list<string> stripped
// to the same nominal tycon, so the first-recorded concrete solution (instantiated at int) was
// wrongly replayed at the string call site -> InvalidCastException at runtime.
module A =
    type System.Collections.Generic.List<'T> with
        static member ( * ) (xs: ResizeArray<'T>, n: int) : ResizeArray<'T> =
            let r = ResizeArray<'T>()
            for _ in 1..n do r.AddRange xs
            r   // A = repeat n times

module B =
    type System.Collections.Generic.List<'T> with
        static member ( * ) (xs: ResizeArray<'T>, n: int) : ResizeArray<'T> =
            let r = ResizeArray<'T>()
            r.AddRange xs
            r   // B = copy once (ignore n)

open A
open B   // B opened last -> B must win at both call sites

// call site 1: ResizeArray<int>
let xi = ResizeArray<int>([1;2])
let ri = xi * 3
if ri.Count <> 2 then failwithf "int site: expected B (Count=2), got %d" ri.Count

// call site 2: ResizeArray<string> -- same nominal tycon as site 1, different instantiation
let xs = ResizeArray<string>(["a";"b";"c"])
let rs = xs * 3
if rs.Count <> 3 then failwithf "string site: expected B (Count=3), got %d" rs.Count
if rs.[0] <> "a" then failwithf "string site: expected \"a\", got %s" rs.[0]
