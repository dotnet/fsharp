// #Regression #Conformance #SignatureFiles #Namespaces 
// Regression test for FSHARP1.0:4937
// Usage of 'global' - in open
//<Expects status="success"></Expects>

// OK (kind of redundant)
namespace global

// OK
namespace N
module M = begin end

// OK
namespace global
module M = begin 
             exit 0
           end
