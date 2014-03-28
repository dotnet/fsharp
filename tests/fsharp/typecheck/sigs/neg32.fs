module Neg32
module IncorrectAttributeUsage =
    type DontPressThisButtonAttribute = 
      class 
        inherit System.Attribute
        val v: string 
        val mutable someOtherField: string 
        member x.SomeOtherField 
           with get() = x.someOtherField 
           and  set(v:string) = x.someOtherField <- v
        member x.Message = x.v
        new(s:string) = { inherit System.Attribute(); v=s; someOtherField="" }
      end

    type C = 
      class 
        [<property: DontPressThisButtonAttribute("no!")>]
        val mutable mf3 : int
      end

module Bug5680 = 
    let inline try_parse x = (^a: (static member TryParse: string -> bool * ^a) x)
    let _, x = try_parse "4" 
    let z = x + 1      



type PositiveInterface =
   abstract M<'T> : 'T -> 'T
   abstract M : 'U -> 'U

[<AbstractClass>]
type PositiveClass<'A>() =
   abstract M<'T> : 'T -> 'T
   abstract M2<'T> : 'T -> 'A
   abstract M : 'U -> 'U

type NegativeInterface =
   abstract v : 'T
   abstract M<'T> : 'U 
   abstract M<'T> : 'U -> 'U
   abstract M : ('U -> 'U)

[<AbstractClass>]
type NegativeClass() =
   abstract v : 'T
   abstract M<'T> : 'U 
   abstract M<'T> : 'U -> 'U
   abstract M : ('U -> 'U)

type NegativeRecord =
   { v : 'T }

type NegativeUnion =
   Foo of 'T

type EventDeclarationWithSquiggleThatWasTooLong = 
    [<CLIEvent>]
    abstract member Event1 : IDelegateEvent<System.EventHandler<System.EventArgs<int>>>

