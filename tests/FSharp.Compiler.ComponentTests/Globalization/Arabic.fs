// #Globalization 
#light

module المطبّقة
    type إجراء =
        | Bar of int
        | Baz of int
        | Zoo of int
        | Yap of string
        | Uتلم of int
        | Uرنت of int
        | Uتسجي of string

    let تلميح = إجراء.Bar(1)

    let تروني = إجراء.Uتلم(1)

    let ارالتسج = إجراء.Uرنت(3)

    let تي =  إجراء.Uتسجي("ئابةتثجحخد!")

    type ةتثجحخ(إجراء : إجراء) =
        let ذر = إجراء

        member ةتثجح.ابةت(testإجراء : إجراء) =
            ذر |> ignore
            match testإجراء with
            | إجراء.Bar(_)  -> "Match1"
            | إجراء.Baz(_)  -> "Match2"
            | إجراء.Zoo(_)  -> "Match3"
            | إجراء.Yap(_)  -> "Match4"
            | إجراء.Uتلم(_)  -> "Match5"
            | إجراء.Uرنت(_)  -> "Match6"
            | إجراء.Uتسجي(_)  -> "Match7"

    let ذرزسشصض = ةتثجحخ(تلميح)
    let ذرزسشص = ةتثجحخ(تروني)
    let ذرزسش = ةتثجحخ(ارالتسج)
    let ذرزس = ةتثجحخ(تي)

    let ذرز() =
        ذرزسشصض.ابةت(تلميح) |> ignore
        ذرزسشص.ابةت(تروني) |> ignore
        ذرزسش.ابةت(ارالتسج) |> ignore
        ذرزس.ابةت(تي) |> ignore
        
        "ؤإئابةتثجحخدذرزسشصضطظع" |> ignore
        """ؤإئابةتثجحخدذرزسشصضطظع"""
