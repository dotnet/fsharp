
module Test

type DoBindingInClassWithoutImplicitCtor =
  static do printfn "hello"

type StaticLetBindingInClassWithoutImplicitCtor =
  static let x = 1

type LetBindingInClassWithoutImplicitCtor =
  let x = 1

type DoBindingInClassWithoutImplicitCtor =
  do printfn "hello"

type LetAfterMember() =
  member x.P  = 1
  let x = 1

type Bad1() = 
    member val X = 1 + 1
    let x = 1
    member x.P = 1

type Bad2() = 
    interface System.ICloneable with 
        member x.Clone() = obj()
    let X = 1 + 1

type Bad4 = 
    static member val X = 1 + 1

type Bad3 = 
    member val X = 1 + 1

type Ok1() = 
    let p = 1
    member val X = 1 + 1

type Ok2() = 
    let p = 1
    member val X = 1 + 1

[<Struct>]
type S = 
    val mutable X : int 

[<Struct>]
type StructWithValBinding() = 
    member val S = 1 with get,set

type Bad5() = 
    let p = 1
    member val X = 1 + 1 with set // 'set' alone not allowed

type Ok3() = 
    let p = 1
    member val X = 1 + 1 with get,set // 'set' alone not allowed

module GeneralizationBug_220377 = 
    type F<'T> = 
      abstract Invoke<'S> : 'S -> 'T

    let foo (f:F<'T>) =
      f.Invoke(42), f.Invoke("hi")

    let res() = 
      foo { new F<_> with
              member x.Invoke<'S>(s:'S) = s } // 'S should not generalize, error should be reported here

    let n : int * int = res()

[<AbstractClass>]
type Bad10 = 
    abstract inline X : int


[<AbstractClass>]
type Bad11 = 
    abstract inline X : int -> int


module LeafExpressionsInQueries  = 

   let f1 (xs:seq<int>) = query { for x in xs do select (fun x -> x + 1) }
   let f2 (xs:seq<int>) = query { for x in xs do select ((); x) }
   let f3 (xs:seq<int>) = query { for x in xs do select ((); x) }

