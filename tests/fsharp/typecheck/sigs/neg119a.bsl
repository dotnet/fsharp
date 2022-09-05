
neg119a.fs(40,20,40,22): typecheck error FS0071: Type constraint mismatch when applying the default type 'obj' for a type inference variable. No overloads match for method 'Return'.

Known return type: ((int -> int -> int) -> obj)

Known type parameters: < obj , Applicatives.Ap >

Available overloads:
 - static member Applicatives.Ap.Return: ('r -> 'a) * Ap: Applicatives.Ap -> (('a -> 'r -> 'a2) -> 'a3 -> 'a -> 'r -> 'a2) // Argument at index 1 doesn't match
 - static member Applicatives.Ap.Return: System.Tuple<'a> * Ap: Applicatives.Ap -> ('a -> System.Tuple<'a>) // Argument at index 1 doesn't match
 - static member Applicatives.Ap.Return: r: ^R * obj -> ('a1 -> ^R) when ^R: (static member Return: 'a1 -> ^R) // Argument 'r' doesn't match
 - static member Applicatives.Ap.Return: seq<'a> * Ap: Applicatives.Ap -> ('a -> seq<'a>) // Argument at index 1 doesn't match Consider adding further type constraints
