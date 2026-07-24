type            RefNominalRecd    = { A : int }
type [<Struct>] StructNominalRecd = { A : int }

let refAnonRecd                            =        {| A = 1 |}
let structAnonRecd                         = struct {| A = 1 |}
let refNominalRecd    : RefNominalRecd     = { A = 1 }
let structNominalRecd : StructNominalRecd  = { A = 1 }

let ``ref anon src, no explicit target, stays ref``                                              =        {| ...refAnonRecd; B = 2 |}
let ``ref anon src, explicit struct target, becomes struct``                                     = struct {| ...refAnonRecd; B = 2 |}
let ``ref anon src, inferred struct target, becomes struct`` : struct {| A : int; B : int |}     =        {| ...refAnonRecd; B = 2 |}
let ``struct anon src, no explicit target, stays struct``                                        =        {| ...structAnonRecd; B = 2 |}
let ``struct anon src, explicit struct target, stays struct``                                    = struct {| ...structAnonRecd; B = 2 |}
let ``struct anon src, inferred struct target, stays struct`` : struct {| A : int; B : int |}    =        {| ...structAnonRecd; B = 2 |}

let ``ref nominal src, no explicit target, stays ref``                                           =        {| ...refAnonRecd; B = 2 |}
let ``ref nominal src, explicit struct target, becomes struct``                                  = struct {| ...refAnonRecd; B = 2 |}
let ``ref nominal src, inferred struct target, becomes struct`` : struct {| A : int; B : int |}  =        {| ...refAnonRecd; B = 2 |}
let ``struct nominal src, no explicit target, stays struct``                                     =        {| ...structAnonRecd; B = 2 |}
let ``struct nominal src, explicit struct target, stays struct``                                 = struct {| ...structAnonRecd; B = 2 |}
let ``struct nominal src, inferred struct target, stays struct`` : struct {| A : int; B : int |} =        {| ...structAnonRecd; B = 2 |}
