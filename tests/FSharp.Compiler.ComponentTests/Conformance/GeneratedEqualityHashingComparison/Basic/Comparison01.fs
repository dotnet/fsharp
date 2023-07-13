// #Conformance #TypesAndModules #GeneratedEqualityAndHashing 
#light

// Test generated comparison on tuples.

// *** Tupes ***

let baseline = (1, 2, 3, 4, 5)

if (1, 2, 3, 4, 5) < baseline then exit 1
if (1, 2, 3, 4, 5) > baseline then exit 1
if (1, 2, 3, 4, 5) <> baseline then exit 1

if (0, 2, 3, 4, 5) >= baseline then exit 1
if (1, 1, 3, 4, 5) >= baseline then exit 1
if (1, 2, 2, 4, 5) >= baseline then exit 1
if (1, 2, 3, 3, 5) >= baseline then exit 1
if (1, 2, 3, 4, 4) >= baseline then exit 1


if (2, 2, 3, 4, 5) <= baseline then exit 1
if (1, 3, 3, 4, 5) <= baseline then exit 1
if (1, 2, 4, 4, 5) <= baseline then exit 1
if (1, 2, 3, 5, 5) <= baseline then exit 1
if (1, 2, 3, 4, 6) <= baseline then exit 1

if ("A", "A")  > ("B", "B") then exit 1
if ("B", "A")  > ("B", "B") then exit 1
if ("A", "B")  > ("B", "B") then exit 1
if ("B", "B") <> ("B", "B") then exit 1

let baseline2 = (("A", "A"), ("B", "B"), ("C", "C"))
if (("A", "A"), ("B", "B"), ("C", "C")) <> baseline2 then exit 1

// All equal but last element of last, nested tuple
if (("A", "A"), ("B", "B"), ("C", "B")) >= baseline2 then exit 1

// All lower, but higher value in beginning
if (("A", "B"), ("A", "A"), ("A", "A")) <= baseline2 then exit 1

// generic function should properly handle NaN (Bug183736)

// helper function to apply various operators and collect the result
let foo x y = [(<);(>);(<=);(>=);(=);(*<>*)] |> List.map (fun f -> f x y) |> List.fold (||) false

// simple type
if foo nan 0.0 then exit 1
if foo 0.0 nan then exit 1
if foo nanf 0.0f then exit 1
if foo 0.0f nanf then exit 1

// tuple
if foo (1,nan,3) (1,0.0,3) then exit 1
if foo (1,0.0,3) (1,nan,3) then exit 1
if foo (1,nanf,3) (1,0.0f,3) then exit 1
if foo (1,0.0f,3) (1,nanf,3) then exit 1

// record
type r1 = {a:float; b:float}
if foo {a=0.0; b=nan} {a=0.0; b=0.0} then exit 1
if foo {a=0.0; b=0.0} {a=0.0; b=nan} then exit 1

type r2 = {a:float32; b:float32}
if foo {a=0.0f; b=nanf} {a=0.0f; b=0.0f} then exit 1
if foo {a=0.0f; b=0.0f} {a=0.0f; b=nanf} then exit 1

// data type
type u1 = A | B of float
if foo (B nan) (B 0.0) then exit 1
if foo (B 0.0) (B nan) then exit 1

type u2 = A | B of float32
if foo (B nanf) (B 0.0f) then exit 1
if foo (B 0.0f) (B nanf) then exit 1

// mutable
if foo (ref 0.0) (ref nan) then exit 1
if foo (ref nan) (ref 0.0) then exit 1
if foo (ref 0.0f) (ref nanf) then exit 1
if foo (ref nanf) (ref 0.0f) then exit 1
 
exit 0
