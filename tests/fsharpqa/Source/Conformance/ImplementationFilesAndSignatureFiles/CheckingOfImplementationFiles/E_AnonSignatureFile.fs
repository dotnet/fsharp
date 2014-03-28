// #Conformance #SignatureFiles 
#light

type TypeInAnonSigFile() =
    member this.Value = 10


let test = new TypeInAnonSigFile()
if test.Value <> 10 then exit 1

exit 0
