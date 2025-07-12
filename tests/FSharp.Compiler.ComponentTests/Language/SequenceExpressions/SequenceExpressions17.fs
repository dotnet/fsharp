module MixedRangeVersionTest
        
let f p q r =
    [
        if p then
            if q then 3
        if p then
            if q then
                if r then 3
    ]
    
let f1 p q r =
    [
        if p then
            if q then yield! 1..10
        if p then
            if q then
                if r then yield! 1..10
    ]
    
let f2 x (|P|_|) (|Q|_|) (|R|_|) =
    [
        match x with
        | P _ -> 3
        | _ -> ()

        match x with
        | P y ->
            match y with
            | Q z ->
                match z with
                | R -> 3
                | _ -> ()
            | _ -> ()
        | _ -> ()
    ]
    
let f x (|P|_|) (|Q|_|) (|R|_|) =
    [
        match x with
        | P _ -> yield! 1..10
        | _ -> ()

        match x with
        | P y ->
            match y with
            | Q z ->
                match z with
                | R -> yield! 1..10
                | _ -> ()
            | _ -> ()
        | _ -> ()
    ]

printfn "yield vs yield! tests passed!"