// #Regression #Conformance #ObjectOrientedTypes #TypeExtensions #NoMono 
// Regression test for FSHARP1.0:5053
// Trying to access a protected member in an extension member should give an error
// (and not just fail at runtime!)
//<Expects status="error" id="FS0491" span="(8,27-8,58)">The member or object constructor 'SetWaitNotificationRequired' is not accessible\. Private members may only be accessed from within the declaring type\. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions\.$</Expects>
module Extensions =
    type System.Threading.SynchronizationContext with 
        member x.Test() = x.SetWaitNotificationRequired() // SetWaitNotificationRequired is protected
