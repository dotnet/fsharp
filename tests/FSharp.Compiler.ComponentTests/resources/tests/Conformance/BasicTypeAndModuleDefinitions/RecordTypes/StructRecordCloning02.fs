// #Conformance #TypesAndModules #Records 
#light

// Verify ability to clone records when they are returned from an arbitrary expression

[<Struct>] type Person = { Name : string; Age : int }

let createPersonArray n a num = [| 1 .. num |] |> Array.map (fun _ -> { Name = n; Age = a } )

let test = { (createPersonArray "Bob" 30 100).[99] with Age = 0 }

if test.Name <> "Bob" then exit 1
if test.Age  <> 0 then exit 1

exit 0
