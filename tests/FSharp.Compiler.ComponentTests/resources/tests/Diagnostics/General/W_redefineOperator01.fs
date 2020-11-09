// #Regression #Diagnostics 
// Regression test for FSHARP1.0:3292
// Give a warning when the user attempts to redefine "=", "<", ">", ">=", "<=" or "<>" or define static members with these names
//<Expects status="warning" span="(8,6-8,7)" id="FS0086">The '<' operator should not normally be redefined\. To define overloaded comparison semantics for a particular type, implement the 'System.IComparable' interface in the definition of that type\.</Expects>
//<Expects status="warning" span="(9,8-9,9)" id="FS0086">The '<' operator should not normally be redefined\. To define overloaded comparison semantics for a particular type, implement the 'System.IComparable' interface in the definition of that type\.</Expects>
//<Expects status="warning" span="(11,20-11,21)" id="FS0086">The name '\(<\)' should not be used as a member name\. To define comparison semantics for a type, implement the 'System\.IComparable' interface\. If defining a static member for use from other CLI languages then use the name 'op_LessThan' instead\.</Expects>
module M
let (<) x y = x + y
let f (<)  = 1 < 2
type C() =
    static member (<) (x:C,y:C) = true
