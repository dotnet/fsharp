let r1 = {| A = 1; B = 2 |}

let r2 : {| A : int ; B : int; C : int |} = {| ...r1; C = 3 |}
