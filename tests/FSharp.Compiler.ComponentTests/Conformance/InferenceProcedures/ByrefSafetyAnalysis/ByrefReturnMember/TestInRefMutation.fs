open Prelude

module TestInRefMutation = 
    [<Struct>]
    type TestMut =

        val mutable x : int

        member this.AnAction() =
            this.x <- 1

    let testAction (m: inref<TestMut>) =
        m.AnAction()
        check "vleoij" m.x 0

    let test() =
        let x = TestMut()
        //testIn (&x)
        testAction (&x)
        x            
    test()