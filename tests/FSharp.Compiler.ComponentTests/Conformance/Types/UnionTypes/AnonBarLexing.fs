// Anonymous Union Types
// Make sure there is no lexing error when using a generic type application in an anonymous union type before a bar character
// Add another expressions with a bar, to test whether bar is working correctly

let x: (list<int>|string) = [42]
let a = true || false
42 |> ignore
let y: (list<int> | string) = [42]
let h (z: (list<list<int>>|string)) = x