module Neg03
type myRecord1 = { field1: int; field2: string }
type myRecord2 = { field2: string; field3: int }
let myGoodRecord1 = { new myRecord1 with field1 = 3 and field2 = "3" }  
let myGoodRecord2 = { new myRecord2 with field3 = 3 and field2 = "3" }  
let myGoodRecord3 = { new myRecord2 with field2 = "3" and field3 = 3;  }  
let myBadRecord1 = { new myRecord1 with field1 = 3 and field3 = "3" }  
let myBadRecord2 = { new myRecord2 with field3 = 3 and field1 = "3" }  
let myBadRecord3 = { new myRecord1 with field1 = 1  }  
let myBadRecord4 = { new myRecord1 with field2 = "a"  }  
let myBadRecord5 = { new myRecord2 with field3 = 1  }  

// check location of incomplete and redundant match warnings
let [1] = [2]
  
let f1 [1] = 1

let f2 () = match [2] with [3] -> 4 | _ -> 5

let f3 () = match [2] with | [3] -> 4 | _ -> 5

let f4 () = match [2] with | x -> 4 | [3] -> 5

let (lsl) (a:int) (b:int) =  a <<< b  
let _ = true <<< 4
let _ = true lsl 4
let _ = 4 lsl true
let _ = 4 <<< true


type C() = class
   let t = ref (Map.empty<string,_>)
   let f x = (!t).Add("3",x)
   member x.M() = !t
      
end

let x = C()
let v1 : Map<string,int> = x.M()
let v2 : Map<string,string> = x.M()


// check we're taking notice of RequiresExplicitTypeArgumentsAttribute
module GeneralizationOfTypeFunctionChecks = begin
    let test3257 = typeof
    let test3254 = sizeof

    // check type functions are not generalizeable by default
    let r<'a> = printfn "abc"; ref ([] : 'a list)
    let x1 = r<'a>

    let x2 = (x1 : int list ref)
    let x3 = (x1 : string  list ref)
end



module IncompletePatternWarningTests = begin
    let 1 = 3

    let (1 | 2) = 3
    let (2 | 1) = 3

    let (1,2) = 4,5

    let (Some(1)) = Some(1)

    let (Some _) = Some(1)
    let None = Some(1)
    let (None | None) = Some(1)
    let (None | None | Some(1)) = Some(1)
    let (Some(2) | Some(1)) = Some(1)
    let (Some(2) & Some(1)) = Some(1)
    let ((1,2) | (2,1)) = 2,3

    let [| |] = [| 2 |]
    let [| _;_ |] = [| 2 |]
    let [| 1;2 |] = [| 2 |]
    let [| 1 |] = [| 2 |]

    let "1" = "3"

    let ("1" | "1a") = "3"
    let ("1aa" | "1a") = "3"
    let ("1aa" | "ssssss") = "3"
    let null = "3"
    let _ = match box "3" with :? string  -> 1
    let _ = 
        match box "3" with 
        | :? string  -> 1 
        | :? string  -> 1  // check this rule is marked as 'never be matched'
        | _ -> 2

    let _ = 
        match box "3" with 
        | :? System.IComparable -> 1 
        | :? string  -> 1  // check this rule is marked as 'never be matched'
        | _ -> 2

    let [ ] = [ 2 ]
    let [ _;_ ] = [ 2 ]
    let [ 1;2 ] = [ 2 ]
    let [ 1 ] = [ 2 ]

    let _ =    
        match [] with 
        | [] -> () 
        | [1] -> () 
        | h1::h2::t -> ()
(*
    let _ =    
        match 1N with 
        | 2N -> () 
        | 3N -> ()  // suggests "13N"

    let _ =    
        match 1N with 
        | 2N -> () 
        | 3N -> () 
        | 13N -> () // suggests "113N"

    let _ =    
        match 1N with 
        | 2N -> () 
        | 2N -> ()  // check this rule is marked as 'never be matched'
        | _  -> ()

    let _ =    
        match 1I with 
        | 2I -> () 
        | 3I -> ()  // suggests "13I"


    let _ =    
        match 1I with 
        | 2I -> () 
        | 3I -> () 
        | 13I -> () // suggests "113I"

    let _ =    
        match 1I with 
        | 2I -> () 
        | 2I -> ()  // check this rule is marked as 'never be matched'
        | _  -> ()
  *)      
end

let foo (arr : int) = arr 1

let goo (f : int ) = ()
do goo (fun x -> x + 1)

let goo2 (f : int -> int) = ()
do goo2 1
