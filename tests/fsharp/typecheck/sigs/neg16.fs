

namespace Test
open System.Reflection

type System.Reflection.MethodInfo with
  member mi.Bug() = printf "%A" mi.IsGenericMethod 
  
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
module ActivePatternSanityCHeckTests1 = begin

  let (|Foo|Bar|) x = "BAD DOG!"  //  expect: type string doesn't match type 'choice'
  let (|Foo2|Bar2|_|) x = "BAD DOG!"   // expect: invalid name for an active pattern
  let (|Foo2b|Bar2b|Baz2b|_|) x = "BAD DOG!"   //  expect: type string doesn't match type 'choice'
  let (|Foo3|_|) x = "BAD DOG!"   //  expect: type string doesn't match type 'option'

end
module ActivePatternSanityCHeckTests2 = begin

  let (|Foo|Bar|) (a:int) x = "BAD DOG!"  //  expect: type string doesn't match type 'choice'
  let (|Foo2|Bar2|_|) (a:int)  x = "BAD DOG!"   // expect: invalid name for an active pattern
  let (|Foo2b|Bar2b|Baz2b|_|) (a:int) x = "BAD DOG!"   //  expect: type string doesn't match type 'choice'
  let (|Foo3|_|) (a:int) x = "BAD DOG!"   //  expect: type string doesn't match type 'option'
end

module ActivePatternSanityCHeckTests3 = begin

  let rec (|Foo|Bar|) (a:int) x = "BAD DOG!"  //  expect: type string doesn't match type 'choice'
  let rec (|Foo2|Bar2|_|) (a:int) x = "BAD DOG!"   // expect: invalid name for an active pattern
  let rec  (|Foo2b|Bar2b|Baz2b|_|) (a:int) x = "BAD DOG!"   //  expect: type string doesn't match type 'choice'
  let rec (|Foo3|_|) (a:int) x = "BAD DOG!"   //  expect: type string doesn't match type 'option'

end

module ActivePatternIllegalPatterns = begin

  let (|``FooA++``|) (x:int) = x
  let (``FooA++``(x)) = 10

  let (|OneA|``TwoA+``|) (x:int) = if x = 0 then OneA else ``TwoA+``

  let (|``FooB++``|_|) (x:int) = if x = 0 then Some(x) else None
  let (``FooB++``(x)) = 10

end

module VolatileFieldSanityChecks = begin

  [<VolatileField>]
  let mutable x = 1

  [<VolatileField>]
  let rec f x = 1

  [<VolatileField>]
  let x2 = 1

  type C() = 
    [<VolatileField>]
    static let sx2 = 1   // expect an error - not mutable

    [<VolatileField>]
    static let f x2 = 1   // expect an error - not mutable

    [<VolatileField>]
    let x2 = 1   // expect an error - not mutable

    [<VolatileField>]
    let f x2 = 1   // expect an error - not mutable

    [<VolatileField>]
    val mutable x : int // expect an error - not supported

    member x.P = 1

end

module AllowNullLiteralWithArgumentTest = begin

    type A() = class end

    [<AllowNullLiteral(true)>] // expect an error here
    type B() = inherit A()

end
