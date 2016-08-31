// #ErrorMessages #UnitGenericAbstractType 
//<Expects status="error" span="(7,21)" id="FS0017">The member 'Apply : int -> unit' is specialized with 'unit' but 'unit' can't be used as return type of an abstract method parameterized on return type\.</Expects>
type EDF<'S> =
    abstract member Apply : int -> 'S
type SomeEDF () =
    interface EDF<unit> with
        member this.Apply d = 
            // [ERROR] The member 'Apply' does not have the correct type to override the corresponding abstract method.
            ()