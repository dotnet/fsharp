module SomeModule =

    type StrChainBuilder() =
        member this.Zero() = ""
        member this.Delay(f) = f ()
        member this.Yield(x: string) = x
        member this.Combine(a, b) = a + b

    let strchain = StrChainBuilder()

    let test =
        // 100 x nesting of 1
        strchain {
            "test0"
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
            strchain { "test1" }
        }
