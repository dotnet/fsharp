// #Regression #TypeInference 
// Regression test for https://github.com/Microsoft/visualfsharp/issues/1749
// Type Inference
// Explicit program entry point: [<EntryPoint>]
//<Expects status="success"></Expects>


let run1 (f: unit -> string) = f()
let run2 (f:System.Func<unit,string>) = f.Invoke()
let run3 (f:System.Func<string>) = f.Invoke()
let run4 (f:System.Func<_>) = f.Invoke()
let mutable exitCode = 0

[<EntryPoint>]
let main (argsz:string []) = 
  
    let a = run1 (fun () -> "hello world")
    let a' = run1 (fun _ -> "hello world")
    let a'' = run1 (fun x -> "hello world")


    if a <> a' || a' <> a'' then 
        exitCode <- 1
        failwith "bug in a"


    let b = run2 (System.Func<unit,string>(fun () -> "hello world 2"))
    let b' = run2 (System.Func<unit,string>(fun _ -> "hello world 2"))
    let b'' = run2 (System.Func<unit,string>(fun x -> "hello world 2"))


    if b <> b' || b' <> b'' then
        exitCode <- 1
        failwith "bug in b"


    let c = run3 (System.Func<string>(fun () -> "hello world 3"))
    let c' = run3 (System.Func<string>(fun _ -> "hello world 3"))
    let c'' = run3 (System.Func<string>(fun x -> "hello world 3"))


    if c <> c' || c' <> c'' then
        exitCode <- 1
        failwith "bug in c"


    let d = run4 (System.Func<_>(fun () -> "hello world 4"))
    let d' = run4 (System.Func<_>(fun _ -> "hello world 4"))
    let d'' = run4 (System.Func<_>(fun x -> "hello world 4"))


    if d <> d' || d' <> d'' then
        exitCode <- 1
        failwith "bug in d"

    exit exitCode