module AllowNullLiteralTest = begin

    //[<AllowNullLiteral>]
    type I = 
        interface
          abstract P : int
        end

    //[<AllowNullLiteral>]
    type C() = 
        member x.P = 1


    [<AllowNullLiteral>]
    type D() = 
        inherit C()
        interface I with 
            member x.P = 2
        member x.P = 1

    let d = (null : D)

    let d2 = ((box null) :?>  D)


    [<AllowNullLiteral>] // expect an error here
    type S(c:int) = struct end
    
    [<AllowNullLiteral>] // expect an error here
    type R = { r : int } 
    
    [<AllowNullLiteral>] // expect an error here
    type U = A | B of int
    
    [<AllowNullLiteral>] // expect an error here
    type E = A = 1 | B = 2
    
    [<AllowNullLiteral>] // expect an error here
    type Del = delegate of int -> int
    
    [<AllowNullLiteral>] // expect an error here
    let x = 1
    
    [<AllowNullLiteral>] // expect an error here
    let f x = 1
         
end

module AllowNullLiteralWithArgumentTest = begin

    type A() = class end

    [<AllowNullLiteral(true)>] // expect an error here
    type B() = inherit A()

end
           