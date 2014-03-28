// #Regression #Stress #RequiresENU #ReqRetail #STRESS 
#light

open System
open System.IO
 
let writer = new StreamWriter("2766.fs")

writer.WriteLine("//Negative Stress test for FSharp1.0#2766 - Internal error on parser when given unbalanced and deeply nested parens")
//<Expects id="FS0010" status="error">Unexpected end of input in expression</Expects>
writer.WriteLine("//<Expects id=\"FS0192\" status=\"error\" span=\"(504,1)\">parse error: unexpected end of file</Expected>")


writer.WriteLine("let x = (1 + ")
 
let mutable i = 0
 
while i < 500 do
    writer.Write("        ")
    for j = 0 to i do
        writer.Write("    ")
    writer.WriteLine("(1 +")
    i <- i + 1
 
writer.Close()

