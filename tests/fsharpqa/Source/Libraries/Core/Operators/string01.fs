// #Regression #Libraries #Operators 
open System

// Regression test for FSHARP1.0:3977 - Make "string" function work for decimal and user-defined IFormattable types.

type CalcSum(x : int, y: int) = 
    let mutable x = x
    let mutable y = y
    
    member this.Sum () = x + y
   
    interface IFormattable with
        member x.ToString (format: string, provider : IFormatProvider) = 
            match format with
                | null | ""
                | "g" | "G" ->
                    String.Format("X + Y = {0}", x.Sum())
                | "s" | "S" ->
                    // Short form
                    x.Sum().ToString()
                | _ ->
                    invalidArg format "Format is wrong!"

    override x.ToString() = (x :> IFormattable).ToString(null, null)
                    
let calc = CalcSum(10, 20)

// test string function
match string calc with
| "X + Y = 30" -> ()
| _            -> exit 1

// test with Console.WriteLine
try
    printfn "%s" (calc.ToString())
    Console.WriteLine("{0:S}", calc)
    Console.Write("{0} {1} {2:D}", 10, 20, calc)
with 
    | :? ArgumentException as e ->
        match e.ParamName with
        | "D" -> exit 0
        | _   -> exit 2
        
exit 1
