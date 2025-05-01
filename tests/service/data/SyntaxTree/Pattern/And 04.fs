module Module

let f x = 
    match x with
    | Error ((_: exn) & (:? System.NullReferenceException)) -> ()
    | _ -> ()
