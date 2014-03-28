// Erasure tests for units-of-measure
module Neg51

[<Measure>] type kg
[<Measure>] type s

open System.Collections.Generic

// Emit warning if we try to test against a type that contains a unit-of-measure
let testtype1 (x:obj) = x :? float<kg>
let testtype2 (x:obj) = x :? List<float<kg>>
let testtype3 (x:obj,y:float<'u>) = x :? float<'u>

// Emit warning if we try to cast to type that contains a unit-of-measure
let casttype1 (x:obj) = x :?> float<kg>
let casttype2 (x:obj) = x :?> List<float<kg>>
let casttype3 (x:obj,y:float<'u>) = x :?> float<'u>

// Likewise for matching
let matchtesttype1 (x:obj) =
  match x with
    :? float<kg> -> "yes"
  | _ -> "no"

let matchtesttype2 (x:obj) =
  match x with
    :? List<float<kg>> -> "yes"
  | _ -> "no"

let matchtesttype3 (x:obj,y:float<'u>) =
  match x with
    :? float<'u> -> "yes"
  | _ -> "no"

// But we should not get a warning for dimensionless types
let matchtesttype4 (x:obj) =
  match x with
    :? float<1> -> "yes"
  | _ -> "no"

let matchcasttype1 (x:obj) =
  match x with
    :? float<kg> as x -> Some x
  | _ -> None

let matchcasttype2 (x:obj) =
  match x with
    :? List<float<kg>> as x -> Some x
  | _ -> None

let matchcasttype3 (x:obj,y:float<'u>) =
  match x with
    :? float<'u> as x -> Some x
  | _ -> None

