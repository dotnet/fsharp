// #Conformance #ObjectOrientedTypes #Structs 
#light

#light

// Verify no PEVerify errors when returning a generic struct

[<Struct>]
type GenStruct<'a> =
    val Val : int
      
let f () =
  let aux = GenStruct<'a>()
  aux
  
let x : GenStruct<unit[]> = f()
if x.Val <> 0 then exit 1

exit 0
