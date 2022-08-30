let ``test -20`` n m =
    let expected = seq { n .. -20 .. m }
    let actual = ResizeArray()
    for i in n .. -20 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test -20`` n m |> not then
            failwith $"FAILURE: {n} .. -20 .. {m}"

let ``test -19`` n m =
    let expected = seq { n .. -19 .. m }
    let actual = ResizeArray()
    for i in n .. -19 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test -19`` n m |> not then
            failwith $"FAILURE: {n} .. -19 .. {m}"

let ``test -18`` n m =
    let expected = seq { n .. -18 .. m }
    let actual = ResizeArray()
    for i in n .. -18 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test -18`` n m |> not then
            failwith $"FAILURE: {n} .. -18 .. {m}"

let ``test -17`` n m =
    let expected = seq { n .. -17 .. m }
    let actual = ResizeArray()
    for i in n .. -17 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test -17`` n m |> not then
            failwith $"FAILURE: {n} .. -17 .. {m}"

let ``test -16`` n m =
    let expected = seq { n .. -16 .. m }
    let actual = ResizeArray()
    for i in n .. -16 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test -16`` n m |> not then
            failwith $"FAILURE: {n} .. -16 .. {m}"

let ``test -15`` n m =
    let expected = seq { n .. -15 .. m }
    let actual = ResizeArray()
    for i in n .. -15 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test -15`` n m |> not then
            failwith $"FAILURE: {n} .. -15 .. {m}"

let ``test -14`` n m =
    let expected = seq { n .. -14 .. m }
    let actual = ResizeArray()
    for i in n .. -14 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test -14`` n m |> not then
            failwith $"FAILURE: {n} .. -14 .. {m}"

let ``test -13`` n m =
    let expected = seq { n .. -13 .. m }
    let actual = ResizeArray()
    for i in n .. -13 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test -13`` n m |> not then
            failwith $"FAILURE: {n} .. -13 .. {m}"

let ``test -12`` n m =
    let expected = seq { n .. -12 .. m }
    let actual = ResizeArray()
    for i in n .. -12 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test -12`` n m |> not then
            failwith $"FAILURE: {n} .. -12 .. {m}"

let ``test -11`` n m =
    let expected = seq { n .. -11 .. m }
    let actual = ResizeArray()
    for i in n .. -11 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test -11`` n m |> not then
            failwith $"FAILURE: {n} .. -11 .. {m}"

let ``test -10`` n m =
    let expected = seq { n .. -10 .. m }
    let actual = ResizeArray()
    for i in n .. -10 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test -10`` n m |> not then
            failwith $"FAILURE: {n} .. -10 .. {m}"

let ``test -9`` n m =
    let expected = seq { n .. -9 .. m }
    let actual = ResizeArray()
    for i in n .. -9 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test -9`` n m |> not then
            failwith $"FAILURE: {n} .. -9 .. {m}"

let ``test -8`` n m =
    let expected = seq { n .. -8 .. m }
    let actual = ResizeArray()
    for i in n .. -8 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test -8`` n m |> not then
            failwith $"FAILURE: {n} .. -8 .. {m}"

let ``test -7`` n m =
    let expected = seq { n .. -7 .. m }
    let actual = ResizeArray()
    for i in n .. -7 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test -7`` n m |> not then
            failwith $"FAILURE: {n} .. -7 .. {m}"

let ``test -6`` n m =
    let expected = seq { n .. -6 .. m }
    let actual = ResizeArray()
    for i in n .. -6 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test -6`` n m |> not then
            failwith $"FAILURE: {n} .. -6 .. {m}"

let ``test -5`` n m =
    let expected = seq { n .. -5 .. m }
    let actual = ResizeArray()
    for i in n .. -5 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test -5`` n m |> not then
            failwith $"FAILURE: {n} .. -5 .. {m}"

let ``test -4`` n m =
    let expected = seq { n .. -4 .. m }
    let actual = ResizeArray()
    for i in n .. -4 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test -4`` n m |> not then
            failwith $"FAILURE: {n} .. -4 .. {m}"

let ``test -3`` n m =
    let expected = seq { n .. -3 .. m }
    let actual = ResizeArray()
    for i in n .. -3 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test -3`` n m |> not then
            failwith $"FAILURE: {n} .. -3 .. {m}"

let ``test -2`` n m =
    let expected = seq { n .. -2 .. m }
    let actual = ResizeArray()
    for i in n .. -2 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test -2`` n m |> not then
            failwith $"FAILURE: {n} .. -2 .. {m}"

let ``test -1`` n m =
    let expected = seq { n .. -1 .. m }
    let actual = ResizeArray()
    for i in n .. -1 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test -1`` n m |> not then
            failwith $"FAILURE: {n} .. -1 .. {m}"

let ``test 1`` n m =
    let expected = seq { n .. 1 .. m }
    let actual = ResizeArray()
    for i in n .. 1 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test 1`` n m |> not then
            failwith $"FAILURE: {n} .. 1 .. {m}"

let ``test 2`` n m =
    let expected = seq { n .. 2 .. m }
    let actual = ResizeArray()
    for i in n .. 2 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test 2`` n m |> not then
            failwith $"FAILURE: {n} .. 2 .. {m}"

let ``test 3`` n m =
    let expected = seq { n .. 3 .. m }
    let actual = ResizeArray()
    for i in n .. 3 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test 3`` n m |> not then
            failwith $"FAILURE: {n} .. 3 .. {m}"

let ``test 4`` n m =
    let expected = seq { n .. 4 .. m }
    let actual = ResizeArray()
    for i in n .. 4 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test 4`` n m |> not then
            failwith $"FAILURE: {n} .. 4 .. {m}"

let ``test 5`` n m =
    let expected = seq { n .. 5 .. m }
    let actual = ResizeArray()
    for i in n .. 5 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test 5`` n m |> not then
            failwith $"FAILURE: {n} .. 5 .. {m}"

let ``test 6`` n m =
    let expected = seq { n .. 6 .. m }
    let actual = ResizeArray()
    for i in n .. 6 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test 6`` n m |> not then
            failwith $"FAILURE: {n} .. 6 .. {m}"

let ``test 7`` n m =
    let expected = seq { n .. 7 .. m }
    let actual = ResizeArray()
    for i in n .. 7 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test 7`` n m |> not then
            failwith $"FAILURE: {n} .. 7 .. {m}"

let ``test 8`` n m =
    let expected = seq { n .. 8 .. m }
    let actual = ResizeArray()
    for i in n .. 8 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test 8`` n m |> not then
            failwith $"FAILURE: {n} .. 8 .. {m}"

let ``test 9`` n m =
    let expected = seq { n .. 9 .. m }
    let actual = ResizeArray()
    for i in n .. 9 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test 9`` n m |> not then
            failwith $"FAILURE: {n} .. 9 .. {m}"

let ``test 10`` n m =
    let expected = seq { n .. 10 .. m }
    let actual = ResizeArray()
    for i in n .. 10 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test 10`` n m |> not then
            failwith $"FAILURE: {n} .. 10 .. {m}"

let ``test 11`` n m =
    let expected = seq { n .. 11 .. m }
    let actual = ResizeArray()
    for i in n .. 11 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test 11`` n m |> not then
            failwith $"FAILURE: {n} .. 11 .. {m}"

let ``test 12`` n m =
    let expected = seq { n .. 12 .. m }
    let actual = ResizeArray()
    for i in n .. 12 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test 12`` n m |> not then
            failwith $"FAILURE: {n} .. 12 .. {m}"

let ``test 13`` n m =
    let expected = seq { n .. 13 .. m }
    let actual = ResizeArray()
    for i in n .. 13 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test 13`` n m |> not then
            failwith $"FAILURE: {n} .. 13 .. {m}"

let ``test 14`` n m =
    let expected = seq { n .. 14 .. m }
    let actual = ResizeArray()
    for i in n .. 14 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test 14`` n m |> not then
            failwith $"FAILURE: {n} .. 14 .. {m}"

let ``test 15`` n m =
    let expected = seq { n .. 15 .. m }
    let actual = ResizeArray()
    for i in n .. 15 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test 15`` n m |> not then
            failwith $"FAILURE: {n} .. 15 .. {m}"

let ``test 16`` n m =
    let expected = seq { n .. 16 .. m }
    let actual = ResizeArray()
    for i in n .. 16 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test 16`` n m |> not then
            failwith $"FAILURE: {n} .. 16 .. {m}"

let ``test 17`` n m =
    let expected = seq { n .. 17 .. m }
    let actual = ResizeArray()
    for i in n .. 17 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test 17`` n m |> not then
            failwith $"FAILURE: {n} .. 17 .. {m}"

let ``test 18`` n m =
    let expected = seq { n .. 18 .. m }
    let actual = ResizeArray()
    for i in n .. 18 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test 18`` n m |> not then
            failwith $"FAILURE: {n} .. 18 .. {m}"

let ``test 19`` n m =
    let expected = seq { n .. 19 .. m }
    let actual = ResizeArray()
    for i in n .. 19 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test 19`` n m |> not then
            failwith $"FAILURE: {n} .. 19 .. {m}"

let ``test 20`` n m =
    let expected = seq { n .. 20 .. m }
    let actual = ResizeArray()
    for i in n .. 20 .. m do
        actual.Add i
    Seq.length expected = actual.Count && Seq.forall2 (=) actual expected

for n = -20 to 20 do
    for m = -20 to 20 do
        if ``test 20`` n m |> not then
            failwith $"FAILURE: {n} .. 20 .. {m}"

// Generation Code:
// [ -20..-1 ] @ [ 1..20 ]
// |> List.map (fun step ->
//     $"""
// let ``test {step}`` n m =
//     let expected = seq {{ n .. {step} .. m }}
//     let actual = ResizeArray()
//     for i in n .. {step} .. m do
//         actual.Add i
//     Seq.length expected = actual.Count && Seq.forall2 (=) actual expected
//
// for n = -20 to 20 do
//     for m = -20 to 20 do
//         if ``test {step}`` n m |> not then
//             failwith $"FAILURE: {{n}} .. {step} .. {{m}}"
// """)
// |> List.reduce (+)