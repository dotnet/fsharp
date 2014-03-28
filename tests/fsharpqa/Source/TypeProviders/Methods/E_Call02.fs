// #Regression #TypeProvider #Methods #Inline
// This is regression test for DevDiv:358710 - [Type Providers] Calls of provided methods though member constrains and directly has inconsistent behavior
//<Expects status="error" span="(7,9)" id="FS0501">The member or object constructor 'StaticMethod3' takes 0 argument\(s\) but is here given 1\. The required signature is 'Test\.StaticMethod3\(\) : int'\.$</Expects>

open TPTest

let r = Test.StaticMethod3 5


