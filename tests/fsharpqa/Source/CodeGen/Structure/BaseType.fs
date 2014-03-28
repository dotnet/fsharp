module A

open System.Security
open System.Security.Permissions

type T() =
    abstract TestBase : unit -> int
    default x.TestBase() = 
#if PASS1
        1
#else
        2
#endif
    //[<SecuritySafeCritical()>] // this won't get inlined either
    [<EnvironmentPermission(SecurityAction.LinkDemand, Read="PATH")>]
    member this.TestCAS() =
#if PASS1
        10
#else
        11
#endif
    [<SecuritySafeCritical()>] // this won't get inlined either
    abstract TetSecuritySafeCritical : unit -> int
    default x.TetSecuritySafeCritical() =
#if PASS1
        1
#else
        2
#endif
 
type U() = class inherit T() end