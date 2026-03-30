module FileC

// This file references both FileA and FileB — but is listed FIRST in the fsproj.
// Tests a 3-file dependency chain with completely reversed order.

let main () =
    let p = FileB.createPerson "Bob" 25
    let greeting = FileA.greet p
    printfn "FileC says: %s" greeting
    0
