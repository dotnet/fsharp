// Regression test for 847132
//<Expects status="success"></Expects>

open System
open System.Security
open System.Security.Permissions
open Microsoft.FSharp.Reflection

// Just a class to hold my tests
type PartialTrust() = 
    inherit MarshalByRefObject()
    member this.Repro1() = 
        let emptySet : Set<decimal> = Set.empty
        let result = Set.fold (fun _ _ -> -1I) 0I emptySet
        ()

    member this.Repro2() = 
        let implementationInt (x:obj) = box( unbox<int>(x) + 1)
        let resultFuncIntObj  = FSharpValue.MakeFunction(typeof<int -> int>, implementationInt )
        let resultFuncInt = resultFuncIntObj :?> (int -> int)
        ()

// Create sandbox with limited trust: in here I'll run my tests
let setup = new AppDomainSetup()
setup.ApplicationBase <- Environment.CurrentDirectory

let permissions = new PermissionSet(null)
permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution)) |> ignore
permissions.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess)) |> ignore
let appDomain = AppDomain.CreateDomain("Partial Trust AppDomain", null, setup, permissions)
let p = appDomain.CreateInstanceAndUnwrap(typeof<PartialTrust>.Assembly.FullName, typeof<PartialTrust>.FullName) :?> PartialTrust

// p is my class instantiated in the sandbox...
// No exception should be thrown
try
    p.Repro1() |> ignore
    p.Repro2() |> ignore
    exit 0
with
| _ -> exit 1
