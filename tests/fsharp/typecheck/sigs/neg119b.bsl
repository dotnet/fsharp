
neg119b.fs(40,9,40,17): typecheck error FS0030: Value restriction: The value 'res2n3n4' has an inferred generic type
    val res2n3n4: ^_a when (^_b or Applicatives.ZipList<int> or ^_a) : (static member (<*>) : ^_b * Applicatives.ZipList<int> -> ^_a) and (^_c or obj or ^_b) : (static member (<*>) : ^_c * obj -> ^_b) and (Applicatives.Ap or ^_c) : (static member Return: ^_c * Applicatives.Ap -> ((int -> int -> int) -> ^_c))
However, values cannot have generic type variables like '_a in "let x: '_a". You can do one of the following:
- Define it as a simple data term like an integer literal, a string literal or a union case like "let x = 1"
- Add an explicit type annotation like "let x : int"
- Use the value as a non-generic type in later code for type inference like "do x"
or if you still want type-dependent results, you can define 'res2n3n4' as a function instead by doing either:
- Add a unit parameter like "let x()"
- Write explicit type parameters like "let x<'a>".
This error is because a let binding without parameters defines a value, not a function. Values cannot be generic because reading a value is assumed to result in the same everywhere but generic type parameters may invalidate this assumption by enabling type-dependent results.
