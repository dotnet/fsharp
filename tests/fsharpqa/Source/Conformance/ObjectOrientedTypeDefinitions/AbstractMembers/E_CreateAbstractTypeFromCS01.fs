// #Regression #Conformance #ObjectOrientedTypes #MethodsAndProperties #MemberDefinitions 
// Dev10:921995/Dev11:15622: Used to be no compile time error when instantiating abstract class defined in C#
//<Expects status="error" span="(14,10-14,25)" id="FS0759">Instances of this type cannot be created since it has been marked abstract or not all methods have been given implementations\. Consider using an object expression '{ new \.\.\. with \.\.\. }' instead\.$</Expects>
//<Expects status="error" span="(15,10-15,21)" id="FS0759">Instances of this type cannot be created since it has been marked abstract or not all methods have been given implementations\. Consider using an object expression '{ new \.\.\. with \.\.\. }' instead\.$</Expects>
//<Expects status="error" span="(16,10-16,26)" id="FS0759">Instances of this type cannot be created since it has been marked abstract or not all methods have been given implementations\. Consider using an object expression '{ new \.\.\. with \.\.\. }' instead\.$</Expects>
//<Expects status="error" span="(17,10-17,33)" id="FS0759">Instances of this type cannot be created since it has been marked abstract or not all methods have been given implementations\. Consider using an object expression '{ new \.\.\. with \.\.\. }' instead\.$</Expects>

let a = { new TestLib.A() with
          member this.CompareTo x = 0 }

let b = { new TestLib.B<string>() with
          member this.CompareTo x = 0 }

let x1 = new TestLib.A()
let x2 = TestLib.A()
let x3 = TestLib.B<int>()
let x4 = new TestLib.B<string>()

exit 1