// #Regression #Conformance #DeclarationElements #Import 
module openModInFun

let f x y =
    let result = x + y
    open System
    Console.WriteLine(result.ToString())
    
    open type Console
    WriteLine(result.ToString())

let top x y =
    let r1 = x + y
    let r2 = x * y
    let nested x y =
        open System
        Console.WriteLine(r1.ToString())
    nested r1 r2
    
type Foo() =
    do
        open System
        open type Console
        WriteLine 123
    member public this.PrintHello() =
        open System
        Console.WriteLine("Hello!")


(
    open type
        System.Console
    WriteLine()
)


// In `match`
match Some 1 with
| Some 1 when open System; Int32.MinValue < 0 -> 
    open type System.Console
    WriteLine "Is 1"
| _ -> ()

// In `for`
for _ in open System.Linq; Enumerable.Range(0, 10) do
    open type System.Console
    WriteLine "Hello, World!"

// In `while`
while
    (open type System.Int32
     MaxValue < 0) do
    open type System.Console
    WriteLine "MaxValue is negative"

// In `if`
if (open type System.Int32; MaxValue <> MinValue) then
    open type System.Console
    WriteLine "MaxValue is not equal to MinValue"
elif (open type System.Int32; MaxValue < 0) then
    open type System.Console
    WriteLine "MaxValue is negative"
else
    open type System.Console
    WriteLine "MaxValue is positive"

// In `try`
try
    open type System.Int32
    open Checked
    ignore(MaxValue + 1)
with | exn -> open type System.Console; WriteLine exn.Message

// In lambdas
let fun1 = fun x -> open System; x + 1
let fun2 = function x -> open type System.Int32; x + MinValue

// In computation expressions
let res = async {
    open System
    Console.WriteLine("Hello, World!")
    let! x = Async.Sleep 1000
    return x
}

// In `query`
let q = 
    query {
        open type System.Linq.Enumerable
        for i in Range(1, 10) do
            open type int
            yield MinValue + i
    } |> Seq.toArray
