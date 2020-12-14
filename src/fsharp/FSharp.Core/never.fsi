namespace Microsoft.FSharp.Core
   module TypeRegulate = begin
      [<StructAttribute (); NoComparisonAttribute (); NoEqualityAttribute ()>]
      type Never =
         | Never
      with
         static member NoneNeverId : Never -> unit
         static member NoneNeverId : x:int -> int
         static member NoneNeverId : x:'t -> 't
      end

      val inline nnid : x: ^a ->  ^a when (Never or  ^b or  ^a) : (static member NoneNeverId :  ^b ->  ^a)

      type TypeDiffer<'TBelong,'TAgainst> =
         class
            static member Assign : x:'TBelong -> 'TBelong
            static member Assign : 'TAgainst -> Never
            static member inline Id : x: ^a ->  ^b when (TypeDiffer<'TBelong,'TAgainst> or  ^a or  ^b) : (static  member  Assign :  ^a -> ^b)
         end
  end
