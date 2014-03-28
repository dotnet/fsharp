
namespace MyNS

module File2 =
    type IFoo =
        abstract member Bar : int -> int

    type LSS() =
        member this.Irrelevant() = ()
    and LS() =
        member this.Irrelevant() = ()
        interface IFoo with
            member this.Bar x = x

