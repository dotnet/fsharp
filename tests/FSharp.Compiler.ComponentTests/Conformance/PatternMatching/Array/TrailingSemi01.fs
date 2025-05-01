// #Regression #Conformance #PatternMatching #Arrays 
#light

// Verify ability to match a list, array, or record with trailing semicolon
// (Regression for bug 1190 - minor glitch in record pattern)

let list1 = [1;2;3;4;]
let list2 = [
        5;
        6;
        7;
        8;
    ]

if List.length list1 <> 4 then exit 1
if List.length list2 <> 4 then exit 1

// ----------------------------------------

let array1 = [| 1;2;3;4; |]
let array2 = [| 
        5;
        6;
        7;
        8; 
    |]

if Array.length array1 <> 4 then exit 1
if Array.length array2 <> 4 then exit 1

// ----------------------------------------

type recordType1 = {
    label1 : int;
    label2 : int;
}
type recordType2 = { label1B : int; label2B : int; }

let record1 = { label1 = 0; label2 = 1; }
let record2 = {
        label1 = 2;
        label2 = 3;
    }

// -------------------------------------------------------------------------

let isList1 x =
    match x with
    | [
          1;
          2;
          3;
          4; ] -> true
    | _        -> false

let isList2 x =
    match x with
    | [5; 6; 7; 8;] -> true
    | _             -> false
    
if not (isList1 list1) then exit 1
if not (isList2 list2) then exit 1

// ----------------------------------------

let isArray1 x =
    match x with
    | [|
          1;
          2;
          3;
          4; |] -> true
    | _         -> false

let isArray2 x =
    match x with
    | [| 5; 6; 7; 8; |] -> true
    | _                 -> false
    
if not (isArray1 array1) then exit 1
if not (isArray2 array2) then exit 1

// ----------------------------------------

let isRecord1 x =
    match x with
    | {
        label1 = 0;
        label2 = 1;
      } -> true
    | _ -> false

let isRecord2 x = 
    match x with
    | { label1 = 2; label2 = 3; } -> true
    | _                           -> false
    
    
if not (isRecord1 record1) then exit 1
if not (isRecord2 record2) then exit 1

exit 0
