// #Regression #Conformance #ObjectOrientedTypes #Structs 
// Verify error when trying to inherit from a struct type
// Regression test for FSHARP1.0:2803
//<Expects id="FS0945" status="error">Cannot inherit a sealed type</Expects>
#light

type StructType = struct
    new(x : int, y : int) = { X = x; Y = y }

    val X : int
    val Y : int
    end


type InheritFromStruct(x, y) = 
    inherit StructType(x, y)
    override this.ToString() = ""


exit 1
