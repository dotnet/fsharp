let r1 = {| A = 1; B = 2 |}

let r2 : {| A : string; B : int |} = {| ...r1; A = "A" |}
let r2' : {| A : string; B : int |} = {| {||} with ...r1; A = "A" |}
