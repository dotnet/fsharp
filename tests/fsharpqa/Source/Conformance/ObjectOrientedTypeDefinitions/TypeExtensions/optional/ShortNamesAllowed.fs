// #Regression #Conformance #ObjectOrientedTypes #TypeExtensions  
// Verify that type being optional extended may use a short name identifier

// Bug FSharp 1.0:3720.



#light
namespace OE
  open NS
  module M =
    type Lib with
    
    // Extension Methods
          member x.ExtensionMember () = 1
  
  module F =
    open M
    let mutable res = true
    
    let a = new Lib()
    if not (a.ExtensionMember () = 1) then
      printf "Lib.ExtensionMember failed\n"
      res <- false
      
    (if (res) then 0 else 1) |> exit
