
module Neg49

module Example1 = 
    let rec X = X // expect an error

module Example2 =
    let rec X1 = X2 // expect an error
    and X2 = X1

module Example3 =
    let rec X1 = X3 // expect an error
    and X2 = 1
    and X3 = X1

module Example4 =
    let rec X1 = X3 // expect an error
    and X2 = X1
    and X3 = X2

module Example5 =
    let rec X1 = X3 // expect an error
    and X2() = 1
    and X3 = X1

module Example6 =
    let rec X1 = X3 // expect an error
    and X2() = X2()
    and X3 = X1
