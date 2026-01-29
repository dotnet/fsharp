// #Regression #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #MemberDefinitions 
// Regression test for FSHARP1.0:5173
// Title: individual get/set properties cannot be marked as "inline"
// Descr: verify that with auto-#light-on, property getters/setters can be marked as inline

type A() = 
    member this.x = 1
    member this.X
        with inline get () = this.x + 1
        and  inline set y  = this.x + y |> ignore
         
    member this.X2
        with inline get (i : int) = this.x + i
        
    member this.X3
        with inline set (i : int) (y : string) = i + this.x |> ignore
        
let a = new A()
(a.X, a.X2(2), a.X3(5) <- "hi") |> ignore

exit 0
