open Library

[<EntryPoint>]
let main _ =
    let cls = Class1()
    let len = cls.GetLength("123")
    printfn "%O" len
    if len = 3 then 0 else 1