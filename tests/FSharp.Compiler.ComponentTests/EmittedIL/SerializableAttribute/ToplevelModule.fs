// Regression test for DevDiv:327356
// top-level module

module ABC
        (* type *)
        type Expr = Num of int
        exception MyExn of int
        type A(x:string) = member __.X = x

        (* value *)
        let add x y = x + y
        let greeting = "hello"

        module ABC =
            (* type *)
            type Expr = Num of int
            exception MyExn of int
            type A(x:string) = member __.X = x

            (* value *)
            let add x y = x + y
            let greeting = "hello"
