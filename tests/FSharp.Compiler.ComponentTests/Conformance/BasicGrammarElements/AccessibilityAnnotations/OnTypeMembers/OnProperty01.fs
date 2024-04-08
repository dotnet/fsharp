// #Regression #Conformance #DeclarationElements #Accessibility #MethodsAndProperties #MemberDefinitions 
// Regression test for FSharp1.0:4169
// Title: Accessibility modifier in front of property is ignored if either get() or set() is mentioned explicitly
// Verify 'public' is honored everywhere expected

type T() =
    // public by default, can be accessed
    member this.Foo = 100

    // explicit public
    member public this.Bar = 200
    
    // getters
    member this.test1 with public get () = "test1"
    member this.test2 with get () = "test2"
    member public this.test3 with get () = "test3"
    
    // setters
    member this.test4 with public set (x : int) = ()
    member this.test5 with set (x : int) = ()
    member public this.test6 with set (x : int) = ()
    
    // getters and setters together
    member this.test7 with public get () = "test7_get"
                       and public set (x : string) = ()
    member this.test8 with get () = "test8_get"
                       and set (x : string) = ()
    member public this.test9 with get () = "test9_get"
                              and set (x : string) = ()
                              
    // different accessibility on getter and setter
    member this.test10 with public get () = "test10_get"
                        and private set (x : string) = ()
    member this.test11 with private get () = "test11_get"
                        and public set (x : string) = ()
                        
let a = T()
a.Foo + a.Bar |> ignore
a.test1 + a.test2 + a.test3 |> ignore
a.test4 <- 0
a.test5 <- 0
a.test6 <- 0
(a.test7, a.test7 <- "test7_set") |> ignore
(a.test8, a.test8 <- "test8_set") |> ignore
(a.test9, a.test9 <- "test9_set") |> ignore
(a.test10, a.test11 <- "test11_set") |> ignore
