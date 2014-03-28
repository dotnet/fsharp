// #Regression #Stress #ReqNOCov #ReqRetail #STRESS 
// See FSHARP1.0:5283

(*

Test capacity for a function's ability to have sequential expressions. Having a 
large number of sequential expressions has caused stack overflows in the past when
doing various optimizations.

E.g.:

let f() =
	expr;
	expr;
	expr;
	...
	expr

*)

open System
open System.IO

let writer = new StreamWriter("SeqExprCapacity.fs")

writer.WriteLine("// Testcase should compile, execute, and return 0")


writer.WriteLine("let f () = ")

// Header
writer.WriteLine("    let i = 0")
writer.WriteLine("    let nestedFunction() = 0")

let mutable i = 0

while i < 6000 do                         // <- is this supposed to be 100000? See FSHARP1.0:5283
    writer.Write("    ")

    let exprBody = 
        match i % 20 with
        | 0 -> "printfn \"Hello, World\""
        | 1 -> "1 + 3 * (int 4.3) |> ignore"
        | 2 -> "let nestedFunction() = i + nestedFunction()"
        | _ -> "do ()"

    writer.WriteLine(exprBody)
    i <- i + 1

writer.WriteLine("let _ = f()")
writer.WriteLine("exit 0")

writer.Close()
