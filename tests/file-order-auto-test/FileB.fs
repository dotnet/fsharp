module FileB

// This file references FileA.Person — but is listed BEFORE FileA in the fsproj.
// With manual ordering, this would fail.
// With --file-order-auto, the compiler should resolve it.

let createPerson name age : FileA.Person =
    { FileA.Name = name; FileA.Age = age }

let run () =
    let p = createPerson "Alice" 30
    printfn "%s" (FileA.greet p)
