open type System.Int32

// Test looping behavior near Int32.MaxValue.

let test n step m =
    let expected = seq { n .. step .. m }
    let actual = ResizeArray()
    for i in n .. step .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

let ns = [ MaxValue - 10 .. MaxValue ]
let ms = [ MaxValue - 10 .. MaxValue ]
let steps = [ -25 .. -1 ] @ [ 1 .. 25 ]

for n in ns do
    for step in steps do
        for m in ms do
            let res = test n step m
            if not res then 
                failwith $"FAILED CASE: {n} .. {step} .. {m}"

do
    let n = MaxValue - 1
    let m = MaxValue
    let expected = seq { n .. 10 .. m }
    let actual = ResizeArray()
    for i in n .. 10 .. m do
        actual.Add i
    let res = Seq.length expected = actual.Count && Seq.forall2 (=) actual expected
    if not res then 
        failwith $"FAILED CASE: {n} .. 10 .. {m}"
