module MixedRangeListTests

// This aggregated file is now split into SequenceExpressions02a.fs..02d.fs.
// Keeping a trivial test here to maintain references where needed.

let test = [0]
let expected = [0]

if test <> expected then failwith $"test failed: got {test}"

printfn "All list tests passed!"