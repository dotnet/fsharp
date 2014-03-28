// #Conformance #ObjectOrientedTypes #TypeExtensions 
// Verify it is legitimate to put an extension method in a namespace
// as long as the type is in the same file and same namespace where the
// they is defined (and, yes, we have to put it in a module)

//<Expects status="success"></Expects>

namespace N
   type T() = class
              end

   module M =
     type T with
        member this.ExtensionMethod() = 42
   
   module MM =
    open M
    (if (new T()).ExtensionMethod() = 42 then exit 0 else 1) |> exit
