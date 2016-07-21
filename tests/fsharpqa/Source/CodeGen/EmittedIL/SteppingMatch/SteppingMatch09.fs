// #NoMono #NoMT #CodeGen #EmittedIL 
module SteppingMatch09
open System
let public funcA n =   
        match n with                                    
        | 1  ->
            Some(10)  // debug range should cover all of "Some(10)"
        | 2  ->            
            None
        | _ ->
                   Some(   22   )  // debug range should cover all of "Some(   22   )"

// Test case from https://github.com/Microsoft/visualfsharp/issues/105
let OuterWithGenericInner list =
  let GenericInner (list: 'T list) = 
     match list with 
     | [] -> 1 
     | _ -> 2

  GenericInner list

// Test case from https://github.com/Microsoft/visualfsharp/issues/105
let OuterWithNonGenericInner list =
  let NonGenericInner (list: int list) = 
     match list with 
     | [] -> 1 
     | _ -> 2

  NonGenericInner list

// Test case from https://github.com/Microsoft/visualfsharp/issues/105
let OuterWithNonGenericInnerWithCapture x list =
  let NonGenericInnerWithCapture (list: int list) = 
     match list with 
     | [] -> 1 
     | _ -> x

  NonGenericInnerWithCapture list

//let _ = OuterWithGenericInner [1;2;3;4;5;6]
//let _ = OuterWithNonGenericInner [1;2;3;4;5;6]
//let _ = OuterWithNonGenericInnerWithCapture 5 [1;2;3;4;5;6]

