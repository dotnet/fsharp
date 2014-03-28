namespace MyNS

module File2 =
    type IFoo =
        abstract member Bar : int -> int

    type LSS =
      class
        member Irrelevant : unit -> unit
        member Bar : string -> int
      end
    type LS =
      class
        interface IFoo
        member Irrelevant : unit -> unit
      end
