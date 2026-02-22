// #Regression #Conformance #LexicalAnalysis 
// Regression test for FSharp1.0:2270 - namespaces, types, modules and union casts should not allow literal DOT in their names
//<Expects status="error" span="(10,11-10,18)" id="FS0883">Invalid namespace, module, type or union case name$</Expects>

// OK
namespace A.B
  type C() = class end

// ERR  
namespace ``A.B``
  type C() = class inherit A.B.C() end
