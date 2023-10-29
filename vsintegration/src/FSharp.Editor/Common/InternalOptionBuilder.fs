module internal Microsoft.VisualStudio.FSharp.Editor.InternalOptionBuilder

type InternalOptionBuilder() =
    member _.Bind(x, f) = Option.bind f x
    member _.Return(x) = Some x
    member _.ReturnFrom(x) = x
    member _.Zero() = Some()

let internalOption = InternalOptionBuilder()
let inline (|>>) v f = Option.map f v
let inline (|>!) v f = Option.bind f v
let inline (>->) f g v = f v |>> g
let inline (>=>) f g v = f v >>= g
