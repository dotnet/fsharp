
neg119b.fs(40,9,40,17): typecheck error FS0030: Value restriction. The value 'res2n3n4' has been inferred to have generic type
    val res2n3n4: ^_a when (^_b or Applicatives.ZipList<int> or ^_a) : (static member (<*>) : ^_b * Applicatives.ZipList<int> -> ^_a) and (^_c or obj or ^_b) : (static member (<*>) : ^_c * obj -> ^_b) and (Applicatives.Ap or ^_c) : (static member Return: ^_c * Applicatives.Ap -> ((int -> int -> int) -> ^_c))    
Either define 'res2n3n4' as a simple data term, make it a function with explicit arguments or, if you do not intend for it to be generic, add a type annotation.
