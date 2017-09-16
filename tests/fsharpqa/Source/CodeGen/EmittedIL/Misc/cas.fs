// #NoMono #NoMT #CodeGen #EmittedIL   
open CustomSecAttr
open System.Security
open System.Security.Permissions

module AttrTest =


    [<SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)>]
    [<PrincipalPermission(SecurityAction.Demand, Role="test")>]
    type Foo() =
        [<PrincipalPermission(SecurityAction.Demand, Role="test")>]
        [<CustomPermission2(SecurityAction.Assert, SecurityArg=SecurityArgType.B)>]
        member x.someMethod() = 6336

        
    [<assembly: CustomPermission2(SecurityAction.Assert)>]
    do()
