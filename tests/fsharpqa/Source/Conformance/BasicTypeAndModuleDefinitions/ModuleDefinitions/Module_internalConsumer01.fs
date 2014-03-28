// #Regression #AccessorModifiers #Module
// Regression test for DevDiv:175204 
// It should be possible to access internal modules
//<Expects status="success"></Expects>

module Module2

open Module1

let y = f 1

exit <| if y = 1 then 0 else 1