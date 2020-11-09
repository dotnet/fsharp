// #Regression #Conformance #TypeConstraints 
// Regression test for FSHARP1.0:1419
// Tokens beginning with # should not match greedily with directives
// The only case where we are still a bit confused is #light, which is missing in
// this source file (see HashContraint02.fs) and #endif.
#light

type R() = class
    end

type r() = class
    inherit R()
    end

type U() = class
    inherit r()
    end
    
type u() = class
    inherit U()
    end

type light_() = class
    inherit u()
    end
type use_() = class
    inherit light_()
    end

type endif_() = class
    inherit use_()
    end

type if_() = class
    inherit endif_()
    end

type define_() = class
    inherit if_()
    end

let t = new define_()

let t1 (x : #R) = ()
let t2 (x : #r) = ()
let t3 (x : #U) = ()
let t4 (x : #u) = ()
let t6 (x : #use_) = ()
//let t7 (x : #endif_) = ()
//let t8 (x : #if_) = ()
let t9 (x : #define_) = ()

// If this doesn't cause a compiler error we are good to go
t1 t; t2 t; t3 t; t4 t; t6 t; (*t7 t; t8 t; *) t9 t

exit 0
