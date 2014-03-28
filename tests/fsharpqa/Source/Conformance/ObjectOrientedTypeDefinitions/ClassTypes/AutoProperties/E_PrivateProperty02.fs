// #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #Accessibility
// <Expects status="error" id="FS0491" span="(14,1-14,23)">The member or object constructor 'PrivateProperty' is not accessible\. Private members may only be accessed from within the declaring type\. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions\.</Expects>
type T() =
    member val public PublicProperty = 0 with get, set
    member val private PrivateProperty = 0 with get, set
    member val internal InternalProperty = 0 with get, set
    static member val public StaticPublicProperty = 0 with get, set
    static member val private StaticPrivateProperty = 0 with get, set
    static member val internal StaticInternalProperty = 0 with get, set

    member this.DoStuff() = this.PrivateProperty <- 1

let x = T()
x.PrivateProperty <- 1
 
exit 1