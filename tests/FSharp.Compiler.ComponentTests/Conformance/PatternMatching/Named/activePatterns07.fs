// #Conformance #PatternMatching #ActivePatterns 
#light

// Verify parameters into active patterns can be anything (as constrained by the pat-param grammar production)
module OtherModule =
    type Foo = A of int | B of int
    let identifier = 42

module ActivePatternTests  = 

    // Null
    let (|StrLen1|) (param : obj) (input : string) = input.Length
    let test1 = match "foo" with StrLen1 null 3 -> true | _ -> false

    // Const
    let (|StrLen2|) (param : string) (input : string) = input.Length + param.Length
    let test2 = match "foo" with StrLen2 "bar" 6 -> true | _ -> false

    let (|StrLen3|) (param : int) (input : string) = input.Length + param
    let test3 = match "foo" with StrLen3 7 10 -> true | _ -> false

    // Long-ident
    let (|StrLen4|) (param : OtherModule.Foo) (input : string) = let OtherModule.A(value)|OtherModule.B(value) = param in input.Length + value
    let test4 = match "foo" with StrLen4 (OtherModule.A(10)) 13 -> true | _ -> false
    
    // List type
    let (|StringLen5|) param (input : string) = (List.length param) + input.Length
    let test5 = match "foo" with StringLen5 [(1,1);(2,2)] 5 -> true | _ -> false
    
    // Tuple type
    let (|StringLen6|) (ip1,ip2,ip3) (input : string) = input.Length + ip1 + ip2 + ip3
    let test6 = match "foo" with StringLen6 (1,2,3) 9 -> true | _ -> false
    
    // Type constraint
    // Not valid code
    //let (|StringLen7|) (param : obj) (input : string) = input.Length + param.ToString().Length
    //let test7 = match "foo" with StringLen7 ("bar" :> obj) 6 -> true | _ -> false
    
    let (|StringLen8|) (param : 'a list) (input : string) = input.Length + List.length param
    let test8 = match "foo" with StringLen8 ([] : int list) 3 -> true | _ -> false


// Actual test
open ActivePatternTests
    
if not (test1 && test2 && test3 && test4 && test5 && test6 && (*test7 &&*) test8) then exit 1
    
exit 0
