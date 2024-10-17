// #Regression #Conformance #DeclarationElements #LetBindings 
#light

// Regression test for FSharp1.0:3744 - Unable to apply attributes on individual patterns in a tupled pattern match let binding - Implementation doesn't match spec
// Test against error emitted when attributes applied within pattern






open System

let ([<System.Obsolete()>] venus, earth, [<System.Obsolete()>] mars) = 
        ("too hot","just right", "too cold")
