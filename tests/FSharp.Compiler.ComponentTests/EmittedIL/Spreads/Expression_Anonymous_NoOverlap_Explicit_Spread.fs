let r1 = {| A = 1; B = 2 |}

let r2 : {| A : int ; B : int; C : int |} = {| C = 3; ...r1 |}
let r2' : {| A : int ; B : int; C : int |} = {| {||} with C = 3; ...r1 |}
