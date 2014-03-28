// verify type inference works properly when extending asyncbuilder with Task bind and return methods
module AsyncTaskBind

open System.Threading.Tasks

type Microsoft.FSharp.Control.AsyncBuilder with
    member x.Bind(computation:Task<'T>, binder:'T -> Async<'R>) =  x.Bind(Async.AwaitTask computation, binder)
    member x.ReturnFrom(computation:Task<'T>) = x.ReturnFrom(Async.AwaitTask computation)

let f1a p            = async { let! a = p in return a }
let f1b (p:Task<'T>) = async { let! a = p in return a }

let f2a (p:Async<string>) = async.Delay (fun () -> async.Bind(p, (fun a -> async.Return a)))
let f2b (p:Task<string>) = async.Delay (fun () -> async.Bind(p, (fun a -> async.Return a)))

let f3a (p:Async<string>) = async { let! a = p in return (a.EndsWith("3")) }
let f3b (p:Task<string>) = async { let! a = p in return (a.EndsWith("3")) }

let f5a (p:Async<string>) = async.Bind(p, (fun a -> if a.EndsWith("3") then async.Return 1 else async.Return 2))
let f5b (p:Task<string>) = async.Bind(p, (fun a -> if a.EndsWith("3") then async.Return 1 else async.Return 2))

let f7a (p:Async<string>) = async.Bind(p, (fun a -> async.Return (a.EndsWith("3"))))
let f7b (p:Task<string>) = async.Bind(p, (fun a -> async.Return (a.EndsWith("3"))))

let f7an1 (p:Async<string>) = async.Bind(p, binder=(fun a -> async.Return (a.EndsWith("3"))))
let f7bn2 (p:Task<string>) = async.Bind(computation=p, binder=(fun a -> async.Return (a.EndsWith("3"))))

let f8a (p:Async<string>) = async.Delay (fun () -> async.Bind(p, (fun a -> async.Return (a.EndsWith("3")))))
let f8b (p:Task<string>) = async.Delay (fun () -> async.Bind(p, (fun a -> async.Return (a.EndsWith("3")))))
let f8c (p:Task<string>) = (fun (a:AsyncBuilder) -> a.Delay (fun () -> a.Bind(p, (fun _arg1 -> let v = _arg1 in a.Return (v.EndsWith("3")))))) async