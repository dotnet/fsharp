// #Regression #Conformance #DeclarationElements #LetBindings 
#light

// Regression test for FSharp1.0:3744 - Unable to apply attributes on individual patterns in a tupled pattern match let binding - Implementation doesn't match spec
// Test against error emitted when attributes applied within pattern

//<Expects id="FS0683" span="(14,6-14,27)" status="error">Attributes are not allowed within patterns</Expects>
//<Expects id="FS0842" span="(14,8-14,23)" status="error">This attribute is not valid for use on this language element</Expects>
//<Expects id="FS0683" span="(14,42-14,63)" status="error">Attributes are not allowed within patterns</Expects>
//<Expects id="FS0842" span="(14,44-14,59)" status="error">This attribute is not valid for use on this language element</Expects>

open System

let ([<System.Obsolete()>] venus, earth, [<System.Obsolete()>] mars) = 
        ("too hot","just right", "too cold")
