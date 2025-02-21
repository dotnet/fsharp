module M

val test1: string | null -> unit
val test2: string -> unit


val iRejectNulls: string -> string

[<Class>] 
type GenericContainer<'T when 'T:not null and 'T:not struct> =

    member GetNull : unit -> ('T|null)
    member GetNotNull: unit -> 'T

val GetTextOpt: key:string -> string option
