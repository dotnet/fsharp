let e0 : int =
#if DEFINED
    0
#else
    failwith "e0"
#endif

let e1 : int =
#if UNDEFINED
    failwith "e1"
#else
    0
#endif

let e2 : int =
#if DEFINED && UNDEFINED
    failwith "e2"
#else
    0
#endif

let e3 : int =
#if UNDEFINED && DEFINED
    failwith "e3"
#else
    0
#endif

let e4 : int =
#if DEFINED || UNDEFINED
    0
#else
    failwith "e4"
#endif

let e5 : int =
#if UNDEFINED || DEFINED
    0
#else
    failwith "e5"
#endif

let e6 : int =
#if !UNDEFINED
    0
#else
    failwith "e6"
#endif

let e7 :int =
#if !DEFINED
    failwith "e7"
#else
    0
#endif

let e8 : int =
#if !UNDEFINED || DEFINED
    0
#else
    failwith "e8"
#endif

let e9 : int =
#if !DEFINED && DEFINED
    failwith "e9"
#else
    0
#endif

let e10 : int =
#if DEFINED && DEFINED && UNDEFINED
    failwith "e10"
#else
    0
#endif

let e11 : int =
#if UNDEFINED || UNDEFINED || DEFINED
    0
#else
    failwith "e11"
#endif

let e12 : int =
#if DEFINED || DEFINED && UNDEFINED
    0
#else
    failwith "e12"
#endif

let e13 : int =
#if UNDEFINED && DEFINED || DEFINED
    0
#else
    failwith "e13"
#endif

let e14 : int =
#if (DEFINED)
    0
#else
    failwith "e14"
#endif

let e15 : int =
#if (DEFINED || DEFINED) && UNDEFINED
    failwith "e15"
#else
    0
#endif

let e16 : int =
#if UNDEFINED && (DEFINED || DEFINED)
    failwith "e16"
#else
    0
#endif

let e17 : int =
#if DEFINED // A test comment
    0
#else
    failwith "e17"
#endif

// When it comes to #if true/false are seen as identifiers not values
let e18 : int =
#if true
    failwith "e18"
#else
    0
#endif

let e19 : int =
#if false
    failwith "e19"
#else
    0
#endif

let e20 : int =
#if !!DEFINED
    0
#else
    failwith "e20"
#endif

let e21 : int =
#if !!!DEFINED
    failwith "e21"
#else
    0
#endif

let e22 : int =
#if !!UNDEFINED
    failwith "e22"
#else
    0
#endif

let e23 : int =
#if !!!UNDEFINED
    0
#else
    failwith "e23"
#endif

exit 0