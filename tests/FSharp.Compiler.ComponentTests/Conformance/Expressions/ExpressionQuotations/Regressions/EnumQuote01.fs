// #Regression #Conformance #Quotations 
// Regression for FSHARP1.0:6007 
// Enums were causing ArgumentExceptions when quoted

module T

type MyEnum = Foo = 0 | Bar = 1
let q = <@ MyEnum.Foo @>

type NonIntEnum = Foo = 1L | Bar = 3L
let q2 = <@ NonIntEnum.Bar @>

exit 0
