
neg_known_return_type_and_known_type_arguments.fsx(90,24,90,28): typecheck error FS0001: No overloads match for method 'Zero'.



Known return type: MonoidSample



Known type parameters: < MonoidSample , Zero >



Available overloads:
 - static member Zero.Zero :  ^t * Default1 ->  ^t when  ^t : (static member get_Zero : ->  ^t)

 - static member Zero.Zero :  ^t * Default1 -> ('a1 -> 'a1) when  ^t : null and  ^t : struct

 - static member Zero.Zero :  ^t * Default2 ->  ^t when (FromInt32 or  ^t) : (static member FromInt32 :  ^t * FromInt32 -> int32 ->  ^t)

 - static member Zero.Zero :  ^t * Default2 -> ('a1 -> 'a1) when  ^t : null and  ^t : struct

 - static member Zero.Zero :  ^t * Default3 ->  ^t when  ^t : (static member get_Empty : ->  ^t)

 - static member Zero.Zero : 'a array * Zero -> 'a array

 - static member Zero.Zero : 'a list * Zero -> 'a list

 - static member Zero.Zero : 'a option * Zero -> 'a option

 - static member Zero.Zero : ('T ->  ^Monoid) * Zero -> ('T ->  ^Monoid) when (Zero or  ^Monoid) : (static member Zero :  ^Monoid * Zero ->  ^Monoid)

 - static member Zero.Zero : Async< ^a> * Zero -> Async< ^a> when (Zero or  ^a) : (static member Zero :  ^a * Zero ->  ^a)

 - static member Zero.Zero : Lazy< ^a> * Zero -> Lazy< ^a> when (Zero or  ^a) : (static member Zero :  ^a * Zero ->  ^a)

 - static member Zero.Zero : Map<'a,'b> * Zero -> Map<'a,'b> when 'a : comparison

 - static member Zero.Zero : ResizeArray<'a> * Zero -> ResizeArray<'a>

 - static member Zero.Zero : Set<'a> * Zero -> Set<'a> when 'a : comparison

 - static member Zero.Zero : System.TimeSpan * Zero -> System.TimeSpan

 - static member Zero.Zero : seq<'a> * Zero -> seq<'a>

 - static member Zero.Zero : string * Zero -> string

 - static member Zero.Zero : unit * Zero -> unit
