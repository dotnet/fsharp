// #Conformance #BasicGrammarElements #Operators 
#light

// Test basic operator names

module TestModule

// Unary
let (!) x =
    let rec fact x = if x < 1 then 1 else x * fact (x - 1)
    fact x
if !10 <> 3628800 then failwith "Failed: : 1"

// Binary
let (<<<) x y = x - x * y 
if 10 <<< 3 <> -20 then failwith "Failed: : 2"
