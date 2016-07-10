// #Regression #Conformance #ObjectOrientedTypes #TypeExtensions 
// Verify error associated with placing type extensions
// inside namespaces. (They must only be placed in modules.)

//<Expects id="FS0644" span="(10,17-10,32)" status="error">Namespaces cannot contain extension members except in the same file and namespace declaration group where the type is defined\. Consider using a module to hold declarations of extension members\.$</Expects>

namespace System

type System.Object with
    member this.ExtensionMethod() = 42

