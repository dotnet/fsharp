#load "BaseLib.fs"
let add3 = BaseLib.add2 >> ((+) 1)