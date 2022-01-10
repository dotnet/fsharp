// #Globalization 
#light

module ꁫꁬꁭꁮ
    type Foo =
        | Bar of int
        | Baz of int
        | Zoo of int
        | Yap of string
        

    let ꁸꁹꁺꁻ = Foo.Bar(1)

    let ꅀꅁꅂꅃꅄ = Foo.Baz(1)

    let ꅩꅪꅫꅬꅭ = Foo.Zoo(3)

    let ꂖꂗꂘꂙꂚꂛꂜ = Foo.Yap("ꁘꁙꁚꁛꁜꁝꁞ")

    type ꃅꃆꃇꃈꃉ(ꃷꃸꃹꃺꃻꃼꃽꃾꃿ : Foo) =
        let ꂽꂾꂿꃀꃁꃂ = ꃷꃸꃹꃺꃻꃼꃽꃾꃿ
        member ꃜꃝꃞꃟꃠ.ꄴꄵꄶꄷꄸꄹꄺꄻ(ꄃꄄꄅꄆꄇꄈꄉ : Foo) =
            ꂽꂾꂿꃀꃁꃂ |> ignore
            match ꄃꄄꄅꄆꄇꄈꄉ with
            | Foo.Bar(_) -> "ꅥꅦꅧꅨꅩꅪꅫꅬꅭꅮ"
            | Foo.Baz(_) -> "ꅯꅰꅱꅲꅳꅴꅵꅶꅷꅸ"
            | Foo.Zoo(_) -> "ꆍꆎꆏꆐꆑꆒꆓꆔꆕꆖ"
            | Foo.Yap(_) -> "ꃙꃚꃛꃜꃝꃞꃟꃠꃡꃢ"
        
    let ꂽꂾꂿꃀꃁꃂꃃ = ꃅꃆꃇꃈꃉ(ꁸꁹꁺꁻ)
    let ꂍꂎꂏꂐꂑꂒ = ꃅꃆꃇꃈꃉ(ꅀꅁꅂꅃꅄ)
    let ꄋꄌꄍꄎꄏꄐꄑꄒꄓ = ꃅꃆꃇꃈꃉ(ꅩꅪꅫꅬꅭ)
    let ꄴꄵꄶꄷꄸꄹ = ꃅꃆꃇꃈꃉ(ꂖꂗꂘꂙꂚꂛꂜ)

    let ꆙꆚꆛꆜꆝꆞꆟꆠ() =
        ꂽꂾꂿꃀꃁꃂꃃ.ꄴꄵꄶꄷꄸꄹꄺꄻ(ꁸꁹꁺꁻ) |> ignore
        ꂍꂎꂏꂐꂑꂒ.ꄴꄵꄶꄷꄸꄹꄺꄻ(ꅀꅁꅂꅃꅄ) |> ignore
        ꄋꄌꄍꄎꄏꄐꄑꄒꄓ.ꄴꄵꄶꄷꄸꄹꄺꄻ(ꅩꅪꅫꅬꅭ) |> ignore
        ꄴꄵꄶꄷꄸꄹ.ꄴꄵꄶꄷꄸꄹꄺꄻ(ꂖꂗꂘꂙꂚꂛꂜ) |> ignore
        
        "ꆿꇀꇁꇂꇃꇄꇅꇆꇇꇈ" |> ignore
        """ꆿꇀꇁꇂꇃꇄꇅꇆꇇꇈ"""




