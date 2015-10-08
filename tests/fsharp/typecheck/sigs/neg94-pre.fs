namespace Neg94Pre

type Class1() =
  static member inline ($) (r:'R, _) = fun (x:'T) -> ((^R) : (static member method2: ^T -> ^R) x)
  static member inline method1 x = Unchecked.defaultof<'r> $ Class1()