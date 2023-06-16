// #Conformance #DeclarationElements #LetBindings #TypeAnnotations #TypeInference #TypeConstraints 
#light

let f (x:int) (y:string) = System.String.Concat ((x.ToString()), y)
let res1 = f 42 "abc"
if res1 <> "42abc" then failwith "Failed: 1"

let g (x : string) = System.Int32.Parse(x)
let res2 = g "100"
if res2 <> 100 then failwith "Failed: 2"

type Foo() =
    member this.DoStuff ((x : int), (y: string)) = printfn "%A %A" x y
    member this.DoStuff2 ((x, y) : int * string) = printfn "%A %A" x y
    
let x = new Foo()
x.DoStuff(42, "something")
x.DoStuff2((42, "something"))
