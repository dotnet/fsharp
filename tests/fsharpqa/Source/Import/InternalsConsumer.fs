// #Regression #NoMT #Import 
#light

// Regression test for FSharp1.0:3882 - C# internals are not visible to F# when C# uses the InternalsVisibleTo attribute
// Greetings and Calc classes are exposed by a referenced C# class library

let greetings = new Greetings()

let calc = new Calc()

if calc.Add(1,1) <> calc.Mult(1,2) then exit 1

greetings.SayHelloTo("Fred") |> ignore
greetings.SayHiTo("Ben") |> ignore

exit 0
