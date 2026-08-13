namespace LegacyInline

module Library =
    let inline invoke (value: ^T) : int =
        ((^T : (static member Invoke: int -> int) 41))
