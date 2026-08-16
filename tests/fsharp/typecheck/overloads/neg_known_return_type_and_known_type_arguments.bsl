
neg_known_return_type_and_known_type_arguments.fsx(90,24,90,28): typecheck error FS0001: No overloads match for method 'Zero'.

Known return type: MonoidSample

Known type parameters: < MonoidSample , Zero >

Available overloads:
 - static member Zero.Zero: 'a array * Zero -> 'a array // Argument at index 1 doesn't match
 - static member Zero.Zero: 'a list * Zero -> 'a list // Argument at index 1 doesn't match
 - static member Zero.Zero: 'a option * Zero -> 'a option // Argument at index 1 doesn't match
 - static member Zero.Zero: 'a seq * Zero -> 'a seq // Argument at index 1 doesn't match
 - static member Zero.Zero: Map<'a,'b> * Zero -> Map<'a,'b> when 'a: comparison // Argument at index 1 doesn't match
 - static member Zero.Zero: ResizeArray<'a> * Zero -> ResizeArray<'a> // Argument at index 1 doesn't match
 - static member Zero.Zero: Set<'a> * Zero -> Set<'a> when 'a: comparison // Argument at index 1 doesn't match
 - static member Zero.Zero: System.TimeSpan * Zero -> System.TimeSpan // Argument at index 1 doesn't match
 - static member Zero.Zero: string * Zero -> string // Argument at index 1 doesn't match
 - static member Zero.Zero: unit * Zero -> unit // Argument at index 1 doesn't match
 - static member Zero.Zero<'T,^Monoid when (Zero or ^Monoid) : (static member Zero: ^Monoid * Zero -> ^Monoid)> : ('T -> ^Monoid) * Zero -> ('T -> ^Monoid) // Argument at index 1 doesn't match
 - static member Zero.Zero<^a when (Zero or ^a) : (static member Zero: ^a * Zero -> ^a)> : Async<^a> * Zero -> Async<^a> // Argument at index 1 doesn't match
 - static member Zero.Zero<^a when (Zero or ^a) : (static member Zero: ^a * Zero -> ^a)> : Lazy<^a> * Zero -> Lazy<^a> // Argument at index 1 doesn't match
 - static member Zero.Zero<^t when (FromInt32 or ^t) : (static member FromInt32: ^t * FromInt32 -> (int32 -> ^t))> : ^t * Default2 -> ^t // Argument at index 1 doesn't match
 - static member Zero.Zero<^t when ^t: (static member Empty: ^t)> : ^t * Default3 -> ^t // Argument at index 1 doesn't match
 - static member Zero.Zero<^t when ^t: (static member Zero: ^t)> : ^t * Default1 -> ^t // Argument at index 1 doesn't match
 - static member Zero.Zero<^t,'a1 when ^t: null and ^t: struct> : ^t * Default1 -> ('a1 -> 'a1) // Argument at index 1 doesn't match
 - static member Zero.Zero<^t,'a1 when ^t: null and ^t: struct> : ^t * Default2 -> ('a1 -> 'a1) // Argument at index 1 doesn't match
