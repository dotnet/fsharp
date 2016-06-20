// Assume that the following class exists.

type Point = { mutable x : int; mutable y : int }

let pinObject() = 
    let point = { x = 1; y = 2 }
    use p1 = fixed &point.x   // note, fixed is a keyword and would be highlighted
    ()
    
let pinRef() = 
    let point = ref 1
    use p1 = fixed &point.contents   // note, fixed is a keyword and would be highlighted
    ()
    
let pinArray1() = 
    let arr = [| 0.0; 1.5; 2.3; 3.4; 4.0; 5.9 |]
    use p1 = fixed arr
    ()

let pinArray2() = 
    let arr = [| 0.0; 1.5; 2.3; 3.4; 4.0; 5.9 |]
    // You can initialize a pointer by using the address of a variable. 
    use p = fixed &arr.[0]
    ()

let pinString() = 
    let str = "Hello World"
    // The following assignment initializes p by using a string.
    use pChar = fixed str
    ()
    // some code that uses pChar, which has type char*
