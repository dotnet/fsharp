// #Conformance #LexFilter 
#light

// Sanity check #light functionality

let f x y z = 
    let a = x * y
    let b = y * z
    let c = x * z
    let d = 
         match a with
         | 0 -> "x|y = 0"
         | _ when a < 0 -> "x|y < 0 && x|y > 0"
         | _ when a > 0 -> "x|y > 0 && x|y > 0 || x|y < 0 && x|y < 0"
         | _ -> failwith "?"
    let rec fact x =
        if x < 0 then
            failwith "Invalid"
        elif x = 0 then
            1
        elif x = 1 then
            1
        else
            x * fact (x - 1)
               
    (a, b, c, d, fact 4)

// Without #light there would be a compile error
let results = f 1 2 3
let (a, b, c, d, e) = results
if e <> 24 then exit 1

exit 0
