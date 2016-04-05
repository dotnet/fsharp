module PrintfDecimalUnitOfMeasureTests

[<Measure>] type usd

let f x = 
    let _ = sprintf "%M" x
    exit (match box x with :? decimal -> 0 | _ -> 1)
    
f 1.m<usd>