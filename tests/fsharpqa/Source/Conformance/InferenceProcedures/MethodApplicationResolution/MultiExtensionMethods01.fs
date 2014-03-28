// #Conformance #TypeInference 
// Verify nothing explodes when you have multiple extension methods with the same
// signature in scope.

module ExtensionMethodsTest1 = 

    type System.String with 
        member this.LengthPlusX = this.Length + 1

module ExtensionMethodsTest2 = 
    type System.String with 
        member this.LengthPlusX = this.Length + 2


open ExtensionMethodsTest1
if "abc".LengthPlusX <> 4 then exit 1

open ExtensionMethodsTest2
if "abc".LengthPlusX <> 5 then exit 1

open ExtensionMethodsTest1
if "abc".LengthPlusX <> 4 then exit 1


exit 0


