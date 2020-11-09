// #Conformance #TypeConstraints #Regression
// Dev11:51952, at one point this was causing a StackOverflow in the compiler and crashing VS

type 
    MyRecord<'X when 'X:> System.IComparable<'X>>() =
      static member P = 1
      static member clone2 (v: MyField<'X>) = MyField.clone(v)
and 
    MyField<'X when 'X:> System.IComparable<'X>> () =
       static member clone (v: MyField<'X>) : MyField<'X> = v

exit 0