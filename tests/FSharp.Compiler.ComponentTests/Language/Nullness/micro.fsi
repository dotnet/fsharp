
module M
val isNotNull: value: 'T -> bool when 'T : null

val test1: string | null -> unit
val test2: string -> unit


val iRejectNulls: string -> string

type GenericContainer<'T> =

    member GetNull : unit -> ('T|null)