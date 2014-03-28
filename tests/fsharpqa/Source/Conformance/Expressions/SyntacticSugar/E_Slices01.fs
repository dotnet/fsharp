// #Regression #Conformance #SyntacticSugar 
#light

// Verify errors related to ambiguous slicing overloads
//<Expects id="FS0041" status="error" span="(22,9)">A unique overload for method 'GetSlice' could not be determined based on type information prior to this program point\. A type annotation may be needed\. Candidates: member Foo\.GetSlice : x:int \* y1:int option \* y2:float option -> unit, member Foo\.GetSlice : x:int \* y1:int option \* y2:int option -> unit</Expects>
//<Expects id="FS0041" status="error" span="(23,9)">A unique overload for method 'GetSlice' could not be determined based on type information prior to this program point\. A type annotation may be needed\. Candidates: member Foo\.GetSlice : x:int \* y1:int option \* y2:float option -> unit, member Foo\.GetSlice : x:int \* y1:int option \* y2:int option -> unit</Expects>
//<Expects id="FS0041" status="error" span="(24,9)">A unique overload for method 'GetSlice' could not be determined based on type information prior to this program point\. A type annotation may be needed\. Candidates: member Foo\.GetSlice : x1:float option \* x2:int option \* y:int -> unit, member Foo\.GetSlice : x1:int option \* x2:int option \* y:int -> unit</Expects>
//<Expects id="FS0041" status="error" span="(25,9)">A unique overload for method 'GetSlice' could not be determined based on type information prior to this program point\. A type annotation may be needed\. Candidates: member Foo\.GetSlice : x1:float option \* x2:int option \* y:int -> unit, member Foo\.GetSlice : x1:int option \* x2:int option \* y:int -> unit</Expects>
//<Expects id="FS0039" status="error" span="(26,9)">The field, constructor or member 'Item' is not defined</Expects>
//<Expects id="FS0503" status="error" span="(27,9)">The member or object constructor 'GetSlice' taking 3 arguments are not accessible from this code location\. All accessible versions of method 'GetSlice' take 3 arguments\.</Expects>
//<Expects id="FS0503" status="error" span="(28,9)">The member or object constructor 'GetSlice' taking 3 arguments are not accessible from this code location\. All accessible versions of method 'GetSlice' take 3 arguments\.</Expects>

type Foo<'a>() =
    member this.GetSlice(x : int, y1 : int option, y2 : int option) = ()
    member this.GetSlice(x : int, y1 : int option, y2 : float option) = ()
    member this.GetSlice(x1 : float option, x2 : int option, y : int) = ()
    member this.GetSlice(x1 : int option,   x2 : int option, y : int) = ()


let f = new Foo<char>()

let _ = f.[2, 1..]
let _ = f.[2, *]
let _ = f.[..1, 2]
let _ = f.[*, 2]
let _ = f.[3, 4]
let _ = f.[1, *, Some(5)]
let _ = f.[1, *, *]

exit 1