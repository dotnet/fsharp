// #Conformance #SignatureFiles 
#light

// Test the ability to declare anonymous signature files, that is FSI files without a
// top-level module or namespace. (Defaults to name of the file, first letter capitalized, no extension.)


type TypeInAnonSigFile = class
                         end
                         with
                           new : unit -> TypeInAnonSigFile
                           member Value : int
                         end

