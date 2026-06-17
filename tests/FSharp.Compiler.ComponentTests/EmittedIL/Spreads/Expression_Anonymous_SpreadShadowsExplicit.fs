let r1 = {| A = 1; B = 2 |}

let r2 : {| A : int; B : int |} = {| A = "A"; ...r1 |}
let r2' : {| A : int; B : int |} = {| {||} with A = "A"; ...r1 |}
