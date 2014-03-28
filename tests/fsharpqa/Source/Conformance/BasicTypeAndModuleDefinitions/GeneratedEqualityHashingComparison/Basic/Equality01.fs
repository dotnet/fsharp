// #Conformance #TypesAndModules #GeneratedEqualityAndHashing 
#light

// Sanity check generated equality

// *** Tupes ***

let tupeA1 = (1, "foo")
let tupeA2 = tupeA1

let tupeB1 = ("foo", 1)
let tupeB2 = ("foo", 1)

// Referential equality
if tupeA1 <> tupeA2 then exit 1

// Structural equality
if tupeB1 <> tupeB2 then exit 2

// *** Lists ***
if [1;2;3;4;5] <> [1..5] then exit 3

// *** Options ***
if Some(40 + 1) <> Some(39 + 2) then exit 4
if None <> None then exit 5

// *** Arrays ***
if [| |] <> [| |] then exit 6

// *** Records ***
type RecType = {A : int * string; B : int list option}
let a = {A = (1, "one"); B = Some([1;2;3])}
let b = {A = (1, "one"); B = Some([1..3])}

if a <> b then exit 7

// *** Union types ***
type DUType =
    | A of int * DUType
    | B

if A(1, B) <> A(2 - 1, B) then exit 8
if A(1, A(2, B)) = A(0, B) then exit 9

exit 0
