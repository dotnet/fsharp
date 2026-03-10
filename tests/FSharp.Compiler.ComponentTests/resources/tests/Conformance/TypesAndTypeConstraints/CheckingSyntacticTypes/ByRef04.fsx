// #ByRef #Regression #inline
// Regression test for DevDiv:122445 ("Internal compiler error when evaluating code with inline/byref")
//<Expects status="success">val inline f:</Expects>
//<Expects status="success">  x: string -> y: nativeptr<\^a> -> bool</Expects>
//<Expects status="success">    when \^a: unmanaged and</Expects>
//<Expects status="success">         \^a: \(static member TryParse: string \* nativeptr<\^a> -> bool\)</Expects>

// Should compile just fine
let inline f x (y:_ nativeptr) = (^a : (static member TryParse : string * ^a nativeptr -> bool)(x,y))

#q;; 
