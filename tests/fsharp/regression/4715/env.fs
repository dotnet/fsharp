#light
[<RequireQualifiedAccess>]
module app.env

open System
//open System.Runtime.CompilerServices
open System.Threading

let mutable private _processing_date:date.T option = None
let mutable private _irs_received_date:date.T option = None

//[<MethodImpl (MethodImplOptions.NoInlining)>]
let set_processing_date d = Interlocked.Exchange(&_processing_date, Some d)

//[<MethodImpl (MethodImplOptions.NoInlining)>]
let set_irs_received_dt d = Interlocked.Exchange(&_irs_received_date, Some d)

let processing_date () =
    match _processing_date with
    | Some d -> d
    | None -> date.fromdt DateTime.Today

let processing_year () =
    let d = processing_date ()
    date.year d

let tax_year () =
    let yr = processing_year ()
    yr - 1

let irs_received_date () = _irs_received_date

let irs_received_date_exn () =
    match _irs_received_date with
    | Some d -> d
    | None -> failwith "env.irs_received_date uninitialized via set_irs_received_dt"
