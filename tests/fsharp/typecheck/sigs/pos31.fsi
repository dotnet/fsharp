module Pos31
val x1 : obj list
val x2 : obj []
val x3 : obj list
val x4 : obj []
val x5 : seq<obj>
val x6 : seq<obj>
val g2 : unit -> System.Reflection.MemberInfo []
val g3 : unit -> System.Reflection.MemberInfo []
val g4 : unit -> System.Reflection.MemberInfo []

// This checks that IsCompatFlex ensures that a more generic type is not inferred
val g5 : xs:seq<System.Reflection.MemberInfo> -> System.Reflection.MemberInfo []