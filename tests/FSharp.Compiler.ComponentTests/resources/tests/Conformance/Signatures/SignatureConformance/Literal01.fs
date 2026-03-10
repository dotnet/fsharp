// Regression test for DevDiv:266717
// "Unable to compile .fs/.fsi with literal values"

module M
[<Literal>]
let test = 1
