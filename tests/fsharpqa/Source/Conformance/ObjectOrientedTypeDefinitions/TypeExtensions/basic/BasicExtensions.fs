// #Conformance #ObjectOrientedTypes #TypeExtensions 
#light

// Verify error for invalid type extensions
// Extend primitive types with methods called DoStuff()

// Define Foo
type Foo() =
     class
     end

// Extend Foo
type Foo with
    static member DoStuff() = "Some System.String"

// Extend String
type System.String with
    member this.DoStuff() = true

// Extend Bool
type System.Boolean with
    member this.DoStuff() = 5L

// Extend Int64
type System.Int64 with
    member this.DoStuff() = "success"

if Foo.DoStuff().DoStuff().DoStuff().DoStuff() <> "success" then exit 1

exit 0
