// 1020100 test cases
let test n step m =
    let expected = seq { n .. step .. m }
    let actual = ResizeArray()
    for i in n .. step .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

let ns = [ -50 .. 50 ]
let ms = [ -50 .. 50 ]
let steps = [ -50 .. -1 ] @ [ 1 .. 50 ]

for n in ns do
    for step in steps do
        for m in ms do
            let res = test n step m
            if not res then 
                failwith $"FAILED CASE: {n} .. {step} .. {m}"