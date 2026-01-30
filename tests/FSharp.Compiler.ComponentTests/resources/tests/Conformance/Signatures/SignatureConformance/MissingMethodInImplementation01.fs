// #Regression #Conformance #SignatureFiles 
// Regression for 5618
// Used to get a bad error message here, LSS doesn't implement member Bar with is in the .fsi but also on another type
//<Expects status="error" span="(7,8-7,13)" id="FS0193">Module 'MyNS\.File2' requires a value 'member File2\.LSS\.Bar: string -> int'$</Expects>
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
