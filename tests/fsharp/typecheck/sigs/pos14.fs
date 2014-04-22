open System
type A<'T>() = static member M([<ParamArray>] args: 'T[]) = args
let m (args: 'T[]) = A<'T>.M(args)
