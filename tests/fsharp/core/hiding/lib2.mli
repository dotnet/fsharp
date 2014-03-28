

val f1: Lib.abstractType -> unit  (* looks OK, but optimization data may refer to hidden type *)
val f2: Lib.abstractType * Lib.abstractType -> unit  (* looks OK, but optimization data may refer to hidden type *)


val e2: exn
val e3: exn

exception D1 = Lib.A
exception D2 = Lib.C
