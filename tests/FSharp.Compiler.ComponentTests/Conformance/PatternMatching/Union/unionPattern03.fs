// #Conformance #PatternMatching #Unions 
#light

// Test that when there are two potential matches, they are matched in the order they are defined.
type Foo = A | B | C | D

let test1 x =
    match x with
    | A | B -> 1
    | B | C -> 2
    | C | D -> 3
    | D | A -> 4

// The union of patterns should be considered a single entity, and thus
// their relative ordering shouldn't matter.
let test2 x =
    match x with
    | B | A -> 1
    | C | B -> 2
    | D | C -> 3
    | A | D -> 4

if test1 A <> 1 then exit 1
if test1 B <> 1 then exit 1
if test1 C <> 2 then exit 1
if test1 D <> 3 then exit 1   

if test2 A <> 1 then exit 1
if test2 B <> 1 then exit 1
if test2 C <> 2 then exit 1
if test2 D <> 3 then exit 1   

exit 0
