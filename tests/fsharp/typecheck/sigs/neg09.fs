
namespace N

let x = 1

let f x = x + x

  
type C = class end


module Bug1433 = begin
    type IFoo = 
        interface
            abstract NamedMeth1 : arg1:int * arg2:int * arg3:int * arg4:int-> float
        end

    type Foo() = class
        interface IFoo with
            member public this.NamedMeth1 (arg1, arg2, arg3, arg4) = 2.718
        end
        member x.Stuff() = printfn "Foo"
    end

    let y = new Foo() :> IFoo
    do y.NamedMeth1(1, arg4=1, arg2=2) 
end

module Bug1462 = begin

  open System
  open Microsoft.FSharp.Quotations


  let rec expand_power (n,x) =
      if n = 0
      then 1 
      else (x) * (failwith "" : Expr<int>)
end

module Bug5534 = begin
   let (?) x y = () // expect NO warning here
   let (?<-) x y = () // expect NO warning here
end

type Translator() = class
   let (===) x y = true  // expect NO warning here
   member x.M(a,b) = (a === b)
   static member (+) x y = 1
end