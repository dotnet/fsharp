
module Test

type Input<'T>(_v: 'T) =
    static member op_Implicit(value: 'T): Input<'T> = Input<'T>(value)

type OtherArgs() =
    member val Name: string = Unchecked.defaultof<_> with get,set
type SomeArgs() =
    member val OtherArgs: Input<OtherArgs> = Unchecked.defaultof<_> with get, set
    
let test() =
    SomeArgs(OtherArgs = OtherArgs(Name = "test"))
