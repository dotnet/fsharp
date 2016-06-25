namespace Test

open System.Runtime.CompilerServices

type Builder() =
    member self.Bind(x, f, [<CallerLineNumber>] ?line : int) =
        (f x, line)

    member self.Return(x, [<CallerLineNumber>] ?line : int) =
        (x, line)

module Program =
    let builder = Builder()

    [<EntryPoint>]
    let main (_:string[]) =
        let result = 
            builder {
                let! x = 1
                let! y = 2
                return x + y
            }

        if result <> (((3, Some 21), Some 20), Some 19) then
            failwith "Unexpected F# CallerLineNumber"
                   
        0