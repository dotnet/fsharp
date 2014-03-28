// #Conformance #ObjectOrientedTypes #TypeExtensions  
//optional extensions can contain conflicting members as long as they don't get used together
namespace NS
   module M = 
    type Lib() = class end
     
    type Lib with
      // Extension Methods
      member x.ExtensionMember () = 1

    module N =
      type Lib with
        // Extension Methods
        member x.ExtensionMember () = 2
  
   module F =
    let mutable res = true
    open M
  
    let a = new Lib()
    if not (a.ExtensionMember () = 1) then
      printf "Lib.ExtensionMember failed\n"
      res <- false

    (if (res) then 0 else 1) |> exit
