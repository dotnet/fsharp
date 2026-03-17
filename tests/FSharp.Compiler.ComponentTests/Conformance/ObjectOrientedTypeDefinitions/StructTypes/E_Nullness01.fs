// #Regression #Conformance #ObjectOrientedTypes #Structs 
// Verify error struct contains a field which cannot be null.
//<Expects id="FS0688" span="(13,9-13,25)" status="error">The default, zero-initializing constructor of a struct type may only be used if all the fields of the struct type admit default initialization$</Expects>

type DiscUnion = A | B of int

type StructType =
    struct
        val m_x : DiscUnion
        member this.Value = this.m_x
    end

let x = new StructType()
