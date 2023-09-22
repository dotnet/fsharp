// #Regression #Conformance #PatternMatching 
#light

// FSB 1750, Bad codegen in Pattern Matching

let f p x =
    match
        match x with
            | 1 when try p x finally ()
                -> 0
            | _ -> 1
        with
            | 1 -> 0
            | _ -> 1
            
let rec loop x = if x = 0 then 1 else loop (x - 1)

let g p x =
  let test = 
    match x with
      | 1  when (try p x finally ()) -> 3 
      | _ -> 5
  loop test
  
let h p x =
    let test = 
        match x with
        | 1  when (for x = 1 to 100 do printf "" done; true) -> 3
        | _ -> 5
    loop test

let (|E|O|) x = if x % 2 = 0 then E else O

let i p x = 
    let test = 
        match x with
        | E when match x with
                 | E -> true
                 | O -> false
                 -> 3
        | O when match x with
                 | E -> false
                 | O -> true
                 -> 3
        | _ -> 5
    loop test
    
// This test should just compile and PEVerify
exit 0
