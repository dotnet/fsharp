// #Regression #Conformance #DeclarationElements #Events #ReqNOMT 
// Regression test for FSharp1.0:5686
// Title: Need a "protect" in async api, based on code review

Async.TryCancelled (async { while true do printfn "step"; do! Async.Sleep 100 }, (fun _ -> failwith "fail fail fail") )  |> Async.Start
    
Async.CancelDefaultToken()


// Regression test for FSharp1.0:4550
// Title: Make it reasonably easy to remove an event handler [WAS: "Event" combinators issue when adding & removing handler ]

let btn = new System.Windows.Forms.Button()

let mutable counter = 0

let evt = btn.Click |> Observable.map(fun x -> counter <- counter + 1)
let unsubsribe = evt.Subscribe(fun x -> printfn "%A" x)
unsubsribe.Dispose()

btn.PerformClick()

if counter <> 0 then failwith "Failed: 1";;

#q;;
