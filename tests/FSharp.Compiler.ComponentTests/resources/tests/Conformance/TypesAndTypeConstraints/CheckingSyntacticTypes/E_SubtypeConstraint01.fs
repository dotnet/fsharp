// #Regression #Conformance #TypeConstraints 
// Verify error if subtype constraint is not met
//<Expects id="FS0001" status="error">The type 'Animal' is not compatible with the type 'Dog'</Expects>

let isSubtype<'baseClass> (x : 'a when 'a :> 'baseClass) = ()

type Animal() = class end

type Dog() =
    inherit Animal()
    member this.Bark() = printfn "Bark"

// Works
do isSubtype<Animal> (new Dog())
do isSubtype<obj> (52)

// Fails
do isSubtype<Dog> (new Animal())

exit 1
