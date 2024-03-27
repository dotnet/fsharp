// <testmetadata>
// { "optimization": { "reported_in": "#3660", "reported_by": "@varon", "last_know_version_not_optimizing": "8", "first_known_version_optimizing": null } }
// </testmetadata>
let exampleFunc v = v + 2

let runAll(fArr:(int -> int) array) x =
    let mutable n = 0
    for i = 0 to fArr.Length - 1 do
        n <- n + fArr.[i] x
    n


let runAllNoAlloc(fArr:(int -> int) array) x =
    let mutable n = 0
    for i = 0 to fArr.Length - 1 do
        let z = fArr.[i]
        n <- n + z x
    n

System.Console.WriteLine()