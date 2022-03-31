#light
[<RequireQualifiedAccess>]
module app.date

open System
open System.Threading

type T = private T of DateTime

let mutable private _today = None

let set_today (dt:DateTime) = Interlocked.Exchange(&_today, Some (T dt.Date))

let today () =
    match _today with
    | Some d -> d
    | None -> T DateTime.Today.Date

let yesterday () =
    let today = DateTime.Today.Date
    T (today - TimeSpan(1, 0, 0, 0))

let inline private unwrap (T d) = d

let fromdt (dt:DateTime) = T dt.Date

let year d = (unwrap d).Year
