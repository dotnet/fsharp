// #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #Accessibility
//<Expects status="error" span="(12,1-12,24)" id="FS0491">The member or object constructor 'InternalProperty' is not accessible\. Private members may only be accessed from within the declaring type\. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions\.$</Expects>
//<Expects status="error" span="(13,4-13,22)" id="FS0491">The member or object constructor 'InternalProperty' is not accessible\. Private members may only be accessed from within the declaring type\. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions\.$</Expects>
//<Expects status="error" span="(15,1-15,30)" id="FS0491">The member or object constructor 'StaticInternalProperty' is not accessible\. Private members may only be accessed from within the declaring type\. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions\.$</Expects>
//<Expects status="error" span="(16,4-16,28)" id="FS0491">The member or object constructor 'StaticInternalProperty' is not accessible\. Private members may only be accessed from within the declaring type\. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions\.$</Expects>

module M2

open M
let x = T()

x.InternalProperty <- 1
if x.InternalProperty <> 1 then exit 1
    
T.StaticInternalProperty <- 1
if T.StaticInternalProperty <> 1 then exit 1

exit 1