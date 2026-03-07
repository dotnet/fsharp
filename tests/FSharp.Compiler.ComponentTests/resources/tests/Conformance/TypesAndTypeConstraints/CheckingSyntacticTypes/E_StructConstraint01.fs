// #Regression #Conformance #TypeConstraints 
// Verify error when struct constraint isn't satisfied
//<Expects id="FS0001" status="error">A generic construct requires that the type 'string' is a CLI or F# struct type</Expects>

type S =
    struct
        [<DefaultValue>]
        val x : int
        member this.Value = this.x
    end

[<Struct>]
type StructRecord = { X : int }

let isStruct (x : 'a when 'a : struct) = ()

// Works
let s = new S()
do isStruct s

do isStruct { X = 99 }

// This also works, System.Int32 is a value type
do isStruct 42

// Fails
do isStruct "a reference type"

exit 1
