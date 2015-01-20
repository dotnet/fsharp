// #Conformance #LexicalAnalysis

let success = 1
let failure = -1

#if DEFINED
let e0 = success
#else
let e0 = failure
#endif

#if UNDEFINED
let e1 = failure
#else
let e1 = success
#endif

#if DEFINED && UNDEFINED
let e2 = failure
#else
let e2 = success
#endif

#if UNDEFINED && DEFINED
let e3 = failure
#else
let e3 = success
#endif

#if DEFINED || UNDEFINED
let e4 = success
#else
let e4 = failure
#endif

#if UNDEFINED || DEFINED
let e5 = success
#else
let e5 = failure
#endif

#if !UNDEFINED
let e6 = success
#else
let e6 = failure
#endif

#if !DEFINED
let e7 = failure
#else
let e7 = success
#endif

#if !UNDEFINED || DEFINED
let e8 = success
#else
let e8 = failure
#endif

#if !DEFINED && DEFINED
let e9 = failure
#else
let e9 = success
#endif

#if DEFINED && DEFINED && UNDEFINED
let e10 = failure
#else
let e10 = success
#endif

#if UNDEFINED || UNDEFINED || DEFINED
let e11 = success
#else
let e11 = failure
#endif

#if DEFINED || DEFINED && UNDEFINED
let e12 = success
#else
let e12 = failure
#endif

#if UNDEFINED && DEFINED || DEFINED
let e13 = success
#else
let e13 = failure
#endif

#if (DEFINED)
let e14 = success
#else
let e14 = failure
#endif

#if (DEFINED || DEFINED) && UNDEFINED
let e15 = failure
#else
let e15 = success
#endif

#if UNDEFINED && (DEFINED || DEFINED)
let e16 = failure
#else
let e16 = success
#endif

#if DEFINED // A test comment
let e17 = success
#else
let e17 = failure
#endif

// When it comes to #if true/false are seen as identifiers not values
#if true
let e18 = failure
#else
let e18 = success
#endif

#if false
let e19 = failure
#else
let e19 = success
#endif

#if !!DEFINED
let e20 = success
#else
let e20 = failure
#endif

#if !!!DEFINED
let e21 = failure
#else
let e21 = success
#endif

#if !!UNDEFINED
let e22 = failure
#else
let e22 = success
#endif

#if !!!UNDEFINED
let e23 = success
#else
let e23 = failure
#endif

let verify r e = if r = success then 0 else e

let result =
    0
    +   verify e0   0x000001
    +   verify e1   0x000002
    +   verify e2   0x000004
    +   verify e3   0x000008
    +   verify e4   0x000010
    +   verify e5   0x000020
    +   verify e6   0x000040
    +   verify e7   0x000080
    +   verify e8   0x000100
    +   verify e9   0x000200
    +   verify e10  0x000400
    +   verify e11  0x000800
    +   verify e12  0x001000
    +   verify e13  0x002000
    +   verify e14  0x004000
    +   verify e15  0x008000
    +   verify e16  0x010000
    +   verify e17  0x020000
    +   verify e18  0x040000
    +   verify e19  0x080000
    +   verify e20  0x100000
    +   verify e21  0x200000
    +   verify e22  0x400000
    +   verify e23  0x800000

if result <> 0 then printfn "ExtendedIfGrammar failed: 0x%X" result

exit result
