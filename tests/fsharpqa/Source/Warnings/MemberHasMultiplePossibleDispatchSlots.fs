// #Warnings
//<Expects status="Error" id="FS3213">The member 'Bar</Expects>
//<Expects>Please restrict it to one of the following:</Expects>
//<Expects>Bar : double -> int</Expects>
//<Expects>Bar : int -> int</Expects>

type IOverload =
    abstract member Bar : int -> int
    abstract member Bar : double -> int

type Overload =
    interface IOverload with
        override __.Bar _ = 1
    
exit 0