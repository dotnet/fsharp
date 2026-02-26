// #Conformance #TypeConstraints 

open System

let multipleConstraints<'T when 'T :> System.IDisposable and 
                                'T :> System.IComparable > (x: 'T, y: 'T) = 
    if x.CompareTo(y) < 0 then x.Dispose() else y.Dispose()

type T() =
    interface IDisposable with
        member this.Dispose() = ()
    interface IComparable with
        member this.CompareTo(o) = -1
    override this.Equals(o) = true
    override this.GetHashCode() = 1

multipleConstraints((new T(), new T()))

exit 0