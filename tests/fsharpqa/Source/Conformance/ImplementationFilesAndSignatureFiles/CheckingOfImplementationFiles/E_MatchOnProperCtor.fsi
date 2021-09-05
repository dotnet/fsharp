// Regression test for bug 6465
//<Expects status="error" span="(4,11-4,22)" id="FS0193">Module 'MyNamespace' requires a value 'new: \(int \* int\) -> MyType'</Expects>
namespace MyNamespace

type MyType =
    new : (int * int) -> MyType

