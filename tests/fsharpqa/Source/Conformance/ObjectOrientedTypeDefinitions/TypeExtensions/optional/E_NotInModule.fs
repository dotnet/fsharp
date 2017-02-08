// #Regression #Conformance #ObjectOrientedTypes #TypeExtensions  
// Verify that optional extension must be inside a module
//<Expects id="FS0644" status="error" span="(9,20-9,35)">Namespaces cannot contain extension members except in the same file and namespace declaration group where the type is defined\. Consider using a module to hold declarations of extension members\.$</Expects>

namespace NS
    type Lib with
    
    // Extension Methods
          member x.ExtensionMember () = 1
  
    module F =
      exit 1
