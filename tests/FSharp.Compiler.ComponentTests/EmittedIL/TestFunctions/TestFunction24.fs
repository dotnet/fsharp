open FSharp.NativeInterop
// Assume that the following class exists.

type Point = { mutable x : int; mutable y : int }

let pinObject() = 
    let point = { x = 1; y = 2 }
    use p1 = fixed &point.x   // note, fixed is a keyword and would be highlighted
    NativePtr.get p1 0 + NativePtr.get p1 1
    
let pinRef() = 
    let point = ref 17
    use p1 = fixed &point.contents   // note, fixed is a keyword and would be highlighted
    NativePtr.read p1 + NativePtr.read p1 
    
let pinArray1() = 
    let arr = [| 0.0; 1.5; 2.3; 3.4; 4.1; 5.9 |]
    use p1 = fixed arr
    NativePtr.get p1 0 + NativePtr.get p1 1

let pinArray2() = 
    let arr = [| 0.0; 1.5; 2.3; 3.4; 4.1; 5.9 |]
    // You can initialize a pointer by using the address of a variable. 
    use p = fixed &arr.[0]
    NativePtr.get p 0 + NativePtr.get p 1

let pinString() = 
    let str = "Hello World"
    // The following assignment initializes p by using a string.
    use pChar = fixed str
    NativePtr.get pChar 0,  NativePtr.get pChar 1

