#light
module app.main

open System

let private init_dates rdate pdate =
    match rdate with
    | Some d -> env.set_irs_received_dt d |> ignore
    | None -> ()

    match pdate with
    | Some d -> env.set_processing_date d |> ignore
    | None -> ()

[<EntryPoint>]
let main argv =
    try
        
        let rdate = Some (date.yesterday ()) 
        let pdate = Some (date.today () )
        init_dates rdate pdate
    with
     | ex -> eprintfn "Unhandled exception: %s" (ex.ToString()) ; exit 1

    0
