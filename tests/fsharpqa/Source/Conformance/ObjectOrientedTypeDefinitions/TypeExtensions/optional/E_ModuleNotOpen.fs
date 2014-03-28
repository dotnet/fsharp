// #Regression #Conformance #ObjectOrientedTypes #TypeExtensions  
// Verify that Optional extensions are only in scope if the module containing the extension has been opened
//<Expects id="FS0039" status="error">ExtensionMember</Expects>


namespace OE
  open NS
  module M =
    type NS.Lib with
    
    // Extension Methods
          member x.ExtensionMember () = 1
  
namespace Test
  open NS
  module N =
    let a = new Lib()
    let b = a.ExtensionMember ()
  
    exit 1
