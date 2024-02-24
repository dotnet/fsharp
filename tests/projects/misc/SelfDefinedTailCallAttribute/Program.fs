namespace N

    module M =

        open Microsoft.FSharp.Core

        [<TailCall>]
        let rec f x = 1 + f x

        [<EntryPoint>]
        let main argv =
            printfn "Hello from F#"
            0
