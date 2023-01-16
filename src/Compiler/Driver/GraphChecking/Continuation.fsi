[<RequireQualifiedAccess>]
module internal Continuation

val sequence<'a, 'ret> : recursions: (('a -> 'ret) -> 'ret) list -> finalContinuation: ('a list -> 'ret) -> 'ret
