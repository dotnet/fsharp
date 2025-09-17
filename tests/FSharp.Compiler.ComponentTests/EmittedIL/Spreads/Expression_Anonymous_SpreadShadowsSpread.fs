let r1 = {| A = 1; B = 2 |}
let r2 = {| A = "A" |}

let r3 : {| A : string; B : int |} = {| ...r1; ...r2 |}
let r4 : {| A : int; B : int |} = {| ...r2; ...r1 |}

let r3' : {| A : string; B : int |} = {| {||} with ...r1; ...r2 |}
let r4' : {| A : int; B : int |} = {| {||} with ...r2; ...r1 |}
