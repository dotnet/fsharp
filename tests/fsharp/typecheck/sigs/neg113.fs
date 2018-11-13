
module M

    // Check we get compile-time errors
    let negTypeTest1() = ({| a = 1+1; b = 2 |} = {| a = 2 |}) 

    let negTypeTest2() = ({| b = 2 |} = {| a = 2 |} )

    // no subsumption
    let negTypeTest3() = ({| b = 2 |} :> {| a : int |} )

    // no subsumption
    let negTypeTest4() = ({| b = 2; a = 1 |} :> {| a : int |} )

    let posgTypeTest5() = ({| b = 2; a = 1 |} = {| a = 1; b = 2 |} )

    // Comparison is not possible if structural elements are comparable
    let negTypeTest6() = ({| a = id |} > {| a = id |})

    let negTypeTest7() = (compare {| a = id |} {| a = id |})