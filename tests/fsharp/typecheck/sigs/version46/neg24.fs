module Test
open Microsoft.FSharp.Quotations


let test2 (v : Expr<'a> -> Expr<'b>) = <@ fun (i: 'a) -> %v <@i@> @>

let test (v : 'a -> Expr<'b>) = <@ fun (i: 'a) -> %(v i) @>

// expect warning
module Negative = 
    let v1 = [ if true then 1 else 2 ]  
    let v2 = [ if true then () else () ] 
    let v6 = [ if true then () ] 
    let a1 = [| if true then 1 else 2 |]  
    let a2 = [| if true then () else () |] 
    let a6 = [| if true then () |] 
    let s3 = seq { (if true then 1 else 2) }

// expect no warning
module Positive = 
    let v3 = [ (if true then 1 else 2) ] 
    let v4 = [ if true then yield 1 else yield 2 ] 
    let v5 = [ if true then yield 1 ] 
    let a3 = [| (if true then 1 else 2) |] 
    let a4 = [| if true then yield 1 else yield 2 |] 
    let a5 = [| if true then yield 1 |] 
    let s2 = seq { if true then () else () } 
    let s6 = seq { if true then () } 
    let s4 = seq { if true then yield 1 else yield 2 }
    let s5 = seq { if true then yield 1  }


module BadCurriedExtensionMember = 
        type C() =
            member x.P = 1
            
        module M1 = 
            type C with 
                member x.M1 a b = a + b    
                member x.M2 (a,b) c = a + b + c

        module M2 = 
            type C with 
                member x.M1 a b = a + b    
                member x.M2 (a,b) c = a + b + c

        open M1
        open M2

        let c = C()

        // negative test - error expected here
        let x1 : int = c.M1 3 4
        // negative test - error expected here
        let x2 : int -> int = c.M1 3
        // negative test - error expected here
        let x3 : int -> int -> int = c.M1 
        
        // negative test - error expected here
        let y1 : int = c.M2 (3,4) 4
        // negative test - error expected here
        let y2 : int -> int = c.M2 (3,4)
        // negative test - error expected here
        let y3 : int * int -> int -> int = c.M2

type C() = 
    member x.M(abc:int,def:string) = abc + def.Length

// Check that the error for a named argument/setter that does not exist is located in a good place
let _ = C().M(qez=3)
