module Neg123

type Switcher = Switcher

let inline checker< ^s, ^r when (^s or ^r) : (static member pass : ^r -> unit)> (s : ^s) (r : ^r) = ()

let inline format () : ^r =
    checker Switcher Unchecked.defaultof< ^r>
    () :> obj :?> ^r

type Switcher with
    static member inline pass(_ : string -> ^r) =
        checker Switcher Unchecked.defaultof< ^r>
    static member inline pass(_ : int -> ^r) =
        checker Switcher Unchecked.defaultof< ^r>
    static member inline pass(_ : unit) = ()
    static member inline pass(_ : int) = ()

let res : unit = format () "text" 5 "more text" ()
