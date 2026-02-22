// #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #Accessibility
// <Expects status="error" id="FS0491" span="(10,1-10,23)">The member or object constructor 'PrivateProperty' is not accessible\. Private members may only be accessed from within the declaring type\. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions\.</Expects>
// <Expects status="error" id="FS0491" span="(12,1-12,29)">The member or object constructor 'StaticPrivateProperty' is not accessible\. Private members may only be accessed from within the declaring type\. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions\.</Expects>

module M2

open M
let x = T()

x.PrivateProperty <- 1

T.StaticPrivateProperty <- 1
