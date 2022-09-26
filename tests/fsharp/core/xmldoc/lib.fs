namespace XmlDoc

open System.Collections
open System.Collections.Generic

type InterfaceImpl() =

    interface IEnumerable with
        /// This should not appear
        member this.GetEnumerator() = failwith ""

/// Simple type
type SimpleType
     /// Simple constructor
     () =

    /// Simple getter property
    member this.P = 1

    /// Simple setter property
    member this.SetterProperty with get() = 1 and set (v: int) = ()

    /// Simple method
    member this.M() = 1
