module CalculationCE =

    type CalcState = { r: int }

    type CalculationBuilder() =
        member _.Yield _ = { r = 0 }
        member _.Return(x: int) = { r = x }
        member _.ReturnFrom(x) = x

        [<CustomOperation("add", MaintainsVariableSpace = true)>]
        member _.Add(state: CalcState, x) = { r = state.r + x }

        [<CustomOperation("sub", MaintainsVariableSpace = true)>]
        member _.Sub(state: CalcState, x) = { r = state.r - x }

        [<CustomOperation("mul", MaintainsVariableSpace = true)>]
        member _.Mul(state: CalcState, x) = { r = state.r * x }

        [<CustomOperation("div", MaintainsVariableSpace = true)>]
        member _.Div(state: CalcState, x) =
            if x = 0 then
                failwith "can't divide by 0"

            { r = state.r / x }

    let calculation = CalculationBuilder()

open CalculationCE

// 100 x nesting of 5
let c =
    calculation {

        let c1 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c2 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c3 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c4 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c5 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c6 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c7 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c8 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c9 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c10 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c11 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c12 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c13 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c14 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c15 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c16 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c17 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c18 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c19 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c20 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c21 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c22 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c23 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c24 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c25 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c26 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c27 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c28 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c29 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c30 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c31 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c32 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c33 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c34 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c35 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c36 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c37 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c38 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c39 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c40 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c41 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c42 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c43 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c44 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c45 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c46 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c47 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c48 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c49 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c50 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c51 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c52 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c53 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c54 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c55 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c56 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c57 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c58 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c59 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c60 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c61 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c62 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c63 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c64 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c65 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c66 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c67 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c68 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c69 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c70 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c71 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c72 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c73 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c74 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c75 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c76 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c77 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c78 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c79 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c80 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c81 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c82 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c83 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c84 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c85 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c86 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c87 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c88 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c89 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c90 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c91 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c92 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c93 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c94 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c95 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c96 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c97 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c98 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c99 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let c100 =
            calculation {
                let nest2 =
                    calculation {
                        let nest3 =
                            calculation {
                                let nest4 =
                                    calculation {
                                        let nest5 =
                                            calculation {
                                                add 1
                                                sub 1
                                                mul 1
                                                div 1
                                            }

                                        return! nest5
                                    }

                                return! nest4
                            }

                        return! nest3
                    }

                return! nest2
            }

        let s =
            [ c1.r
              c2.r
              c3.r
              c4.r
              c5.r
              c6.r
              c7.r
              c8.r
              c9.r
              c10.r
              c11.r
              c12.r
              c13.r
              c14.r
              c15.r
              c16.r
              c17.r
              c18.r
              c19.r
              c20.r
              c21.r
              c22.r
              c23.r
              c24.r
              c25.r
              c26.r
              c27.r
              c28.r
              c29.r
              c30.r
              c31.r
              c32.r
              c33.r
              c34.r
              c35.r
              c36.r
              c37.r
              c38.r
              c39.r
              c40.r
              c41.r
              c42.r
              c43.r
              c44.r
              c45.r
              c46.r
              c47.r
              c48.r
              c49.r
              c50.r
              c51.r
              c52.r
              c53.r
              c54.r
              c55.r
              c56.r
              c57.r
              c58.r
              c59.r
              c60.r
              c61.r
              c62.r
              c63.r
              c64.r
              c65.r
              c66.r
              c67.r
              c68.r
              c69.r
              c70.r
              c71.r
              c72.r
              c73.r
              c74.r
              c75.r
              c76.r
              c77.r
              c78.r
              c79.r
              c80.r
              c81.r
              c82.r
              c83.r
              c84.r
              c85.r
              c86.r
              c87.r
              c88.r
              c89.r
              c90.r
              c91.r
              c92.r
              c93.r
              c94.r
              c95.r
              c96.r
              c97.r
              c98.r
              c99.r
              c100.r ]

        return! s |> List.sum
    }
