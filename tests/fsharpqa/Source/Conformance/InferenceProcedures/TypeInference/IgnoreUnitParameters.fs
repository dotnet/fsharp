// #Regression #TypeInference 
// Regression test for https://github.com/Microsoft/visualfsharp/issues/1749
// Type Inference
namespace N
module InferUnit = 

    let run1 (f: unit -> string) = f()
    
    let a = run1 (fun () -> "hello world")
    let a' = run1 (fun _ -> "hello world")

    
    if a <> a' then
        failwith "bug in a"
    

    
    let run2 (f:System.Func<unit,string>) = f.Invoke()

    let b = run2 (System.Func<unit,string>(fun () -> "hello world 2"))
    let b' = run2 (System.Func<unit,string>(fun _ -> "hello world 2"))


    if b <> b' then
        failwith "bug in b"



    let run3 (f:System.Func<string>) = f.Invoke()

    let c = run3 (System.Func<string>(fun () -> "hello world 3"))
    let c' = run3 (System.Func<string>(fun _ -> "hello world 3"))


    if c <> c' then
        failwith "bug in c"


    let run4 (f:System.Func<_>) = f.Invoke()

    let d = run4 (System.Func<_>(fun () -> "hello world 4"))
    let d' = run4 (System.Func<_>(fun _ -> "hello world 4"))


    if d <> d' then
        failwith "bug in d"        