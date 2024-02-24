module Test

type EmptyT<'T> =
    static let cachedName = 
        let name = typeof<'T>.Name
        printfn "Accessing name for %s" name
        name
    static member Name = cachedName


for i=0  to 10 do 
    EmptyT<int>.Name |> ignore
    EmptyT<string>.Name |> ignore
    EmptyT<byte>.Name |> ignore