// #Regression #Diagnostics 
namespace N1

let x = 4444444

// blank

namespace N2

// nothing
// nothing
// nothing

// Regression test for FSHARP1.0:4995
// Note that what we are really testing in this case is the span (which was incorrectly (3,1-7-10))
//<Expects span="(4,5-4,6)" status="error" id="FS0201">Namespaces cannot contain values\. Consider using a module to hold your value declarations\.$</Expects>
