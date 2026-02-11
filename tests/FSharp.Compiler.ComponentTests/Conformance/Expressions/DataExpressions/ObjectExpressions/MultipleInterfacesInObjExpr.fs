type I1 =
    abstract Foo : unit -> bool
 
type I2 =
    abstract Foo : unit -> bool

let failIfFalse cond fmt  = 
    Printf.kprintf (fun s -> 
        if not cond then
            printfn "%s" s
            exit 1
    ) fmt

let verifyInterfaceImplementation<'T> o = 
    let ty = o.GetType()
    let map = try ty.GetInterfaceMap(typeof<'T>) with e -> failIfFalse false "%A" e; Unchecked.defaultof<_>
    for (ifcMethod, targetMethod) in Seq.zip map.InterfaceMethods map.TargetMethods do
        failIfFalse (ifcMethod.Name <> targetMethod.Name) "MethodImpl of %s should have mangled name" ifcMethod.Name
        failIfFalse targetMethod.IsVirtual "Method %s should be virtual" targetMethod.Name
        failIfFalse targetMethod.IsPrivate "Method %s should be private" targetMethod.Name


let o1 = 
    {
        new I1 with
            member this.Foo() = false
        interface I2 with
            member this.Foo() = false
    }

let o2 =
    { new obj() with
        override this.ToString() = "123"
      interface I1 with
        member this.Foo() = false
 
      interface I2 with
        member this.Foo() = false
    }


verifyInterfaceImplementation<I1> o1
verifyInterfaceImplementation<I2> o1

verifyInterfaceImplementation<I1> o2
verifyInterfaceImplementation<I2> o2

let o2Ty = o2.GetType()
let toString = o2Ty.GetMethod("ToString") 
failIfFalse (toString.DeclaringType = o2Ty) "ToString should be overridden"
failIfFalse toString.IsPublic "ToString should be public"
failIfFalse toString.IsVirtual "ToString should be virtual"