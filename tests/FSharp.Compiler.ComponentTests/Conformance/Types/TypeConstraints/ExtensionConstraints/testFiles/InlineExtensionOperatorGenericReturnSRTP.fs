module Test

// RFC FS-1043 regression. A generic inline extension operator whose parameter and result types mention
// its own method type parameter (here 'a in 'a list), brought into SRTP scope via `open type`, must not
// be resolved and committed at the definition site of a consuming inline function while the SRTP support
// type is still an abstract typar. Committing there left the operator's method typar undetermined; it
// defaulted to obj and baked an unsound `box ^T; unbox.any List<obj>` coercion into the stored inline
// body, which threw InvalidCastException once the body was specialized at a concrete call site.
[<AbstractClass; Sealed>]
type Ops =
    static member inline (<+>) (a: 'a list, b: 'a list) : 'a list = a @ b

open type Ops

let inline app (a: ^T) (b: ^T) = a <+> b

let ri : int list = app [1; 2] [3; 4]
if ri <> [1; 2; 3; 4] then failwithf "int site: expected [1;2;3;4], got %A" ri

// A second call at a different element type must get its own instantiation, not a shared List<obj>.
let rs : string list = app ["a"] ["b"]
if rs <> ["a"; "b"] then failwithf "string site: expected [\"a\";\"b\"], got %A" rs
