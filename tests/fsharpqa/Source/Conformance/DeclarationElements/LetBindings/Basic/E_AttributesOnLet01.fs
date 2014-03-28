// #Regression #Conformance #DeclarationElements #LetBindings 
#light

// Regression test for FSharp1.0:3744 - Unable to apply attributes on individual patterns in a tupled pattern match let binding - Implementation doesn't match spec
// Test against error emitted when attributes applied within pattern

//<Expects id="FS0683" span="(11,6-11,33)" status="error">Attributes are not allowed within patterns</Expects>

open System

let ([<System.Obsolete()>] venus, earth, [<System.Obsolete()>] mars) = 
        ("too hot","just right", "too cold")

exit 1
