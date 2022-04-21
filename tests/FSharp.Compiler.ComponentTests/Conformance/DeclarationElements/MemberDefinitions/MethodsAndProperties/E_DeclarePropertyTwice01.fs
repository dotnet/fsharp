// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
#light

// Verify error if you declare a setter twice
//<Expects id="FS0037" status="error">Duplicate definition of value 'Vector.set_Length.2'</Expects>

type Vector3 = 
    | Vector3 of float * float * float
    member this.Decompose() = match this with Vector3(x, y, z) -> x, y, z

type Vector =
    { mutable Vector3 : Vector3 }
    member this.Length with set newLen =
                                this.Vector3 <- Vector3(newLen, newLen, newLen)
                         and set newLen =
                                this.Vector3 <- Vector3(newLen, newLen, newLen)
