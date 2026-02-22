// #Regression #Conformance #SignatureFiles 
//<Expects status="error" span="(8,1)" id="FS0222">Files in libraries or multiple-file applications must begin with a namespace or module declaration, e\.g\. 'namespace SomeNamespace\.SubNamespace' or 'module SomeNamespace\.SomeModule'\. Only the last source file of an application may omit such a declaration\.$</Expects>

// Test the ability to declare anonymous signature files, that is FSI files without a
// top-level module or namespace. (Defaults to name of the file, first letter capitalized, no extension.)


type TypeInAnonSigFile = class
                         end
                         with
                           new : unit -> TypeInAnonSigFile
                           member Value : int
                         end

