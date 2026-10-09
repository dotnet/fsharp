type            RefNominalRecd    = { A : int; B : int }
type [<Struct>] StructNominalRecd = { A : int; B : int }

let refAnonRecd                            =        {| A = 1; B = 2 |}
let structAnonRecd                         = struct {| A = 1; B = 2 |}
let refNominalRecd    : RefNominalRecd     = { A = 1; B = 2 }
let structNominalRecd : StructNominalRecd  = { A = 1; B = 2 }

let ``ref nominal src, ref nominal dst``       : RefNominalRecd    = { ...refNominalRecd; B = 3 }
let ``ref nominal src, struct nominal dst``    : StructNominalRecd = { ...refNominalRecd; B = 3 }
let ``struct nominal src, ref nominal dst``    : RefNominalRecd    = { ...structNominalRecd; B = 3 }
let ``struct nominal src, struct nominal dst`` : StructNominalRecd = { ...structNominalRecd; B = 3 }
let ``ref anon src, ref nominal dst``          : RefNominalRecd    = { ...refAnonRecd; B = 3 }
let ``ref anon src, struct nominal dst``       : StructNominalRecd = { ...refAnonRecd; B = 3 }
let ``struct anon src, ref nominal dst``       : RefNominalRecd    = { ...structAnonRecd; B = 3 }
let ``struct anon src, struct nominal dst``    : StructNominalRecd = { ...structAnonRecd; B = 3 }
