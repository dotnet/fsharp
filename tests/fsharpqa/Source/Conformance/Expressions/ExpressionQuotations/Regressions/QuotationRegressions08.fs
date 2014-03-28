// #Regression #Conformance #Quotations #RequiresPowerPack 
// Regression test for FSHARP1.0:3498
// "Quotation expression compiler reveals bug in Linq"
// This code snipped is not supposed to throw at runtime!
//<Expects status=success></Expects>
#light

open Microsoft.FSharp.Linq.QuotationEvaluation

let q = 
    <@ (fun (x1:double) -> 
           let fwd6 = 
               let y3 = x1 * x1
               (y3, (fun yb4 -> yb4 * 2.0 * x1))
           let rev5 = snd fwd6
           let w0 = fst fwd6

           let fwd14 = 
               let y11 = w0 + 1.0
               (y11, (fun yb12 -> yb12 * 1.0))
           let rev13 = snd fwd14
           let y8 = fst fwd14
           (y8, (fun y8b10 -> 
                      let w0b2 = 0.0 
                      let x1b1 = 0.0 
                      let dxs15 = rev13 y8b10 
                      let w0b2 = w0b2 + dxs15 
                      let dxs7 = rev5 w0b2 
                      let x1b1 = x1b1 + dxs7 
                      x1b1))) @>

let (res,_) = (q.Eval()) 4.0

(if (res = 17.0) then 0 else 1) |> exit
