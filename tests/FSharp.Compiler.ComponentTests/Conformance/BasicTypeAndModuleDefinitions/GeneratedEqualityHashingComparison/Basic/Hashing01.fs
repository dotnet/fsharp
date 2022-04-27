// #Conformance #TypesAndModules #GeneratedEqualityAndHashing 
#light

// Sanity check generated equality

// *** Tupes ***

let tupeA1 = (1, "foo")
let tupeA2 = tupeA1

let tupeB1 = ("foo", 1)
let tupeB2 = ("foo", 1)

// Referential equality
if tupeA1.GetHashCode() <> tupeA2.GetHashCode() then exit 1

// Structural equality
if tupeB1 <> tupeB2 then exit 2

// *** Lists ***
if [1;2;3;4;5].GetHashCode() <> [1..5].GetHashCode() then exit 3

// *** Options ***
if Some(40 + 1).GetHashCode() <> Some(39 + 2).GetHashCode() then exit 4
if None <> None then exit 5

// *** Arrays ***
// We don't generate GetHashCode for System.Array
if hash ([| |]) <> hash([| |]) then exit 6

// *** Records ***
type RecType = {A : int * string; B : int list option}
let a = {A = (1, "one"); B = Some([1;2;3])}
let b = {A = (1, "one"); B = Some([1..3])}

if a.GetHashCode() <> b.GetHashCode() then exit 7

// *** Union types ***
type DUType =
    | A of int * DUType
    | B

if A(1, B).GetHashCode() <> A(2 - 1, B).GetHashCode() then exit 8
if A(1, A(2, B)).GetHashCode() = A(0, B).GetHashCode() then exit 9

exit 0
