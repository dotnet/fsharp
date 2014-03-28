// #NoMT #FSI 
#light

// Verify INTERACTIVE is defined for all fsi sessions

let test1 = 
    #if INTERACTIVE
    1
    #else
    0
    #endif

// COMPILED should NOT be defined
let test2 = 
    #if COMPILED
    0
    #else
    1
    #endif

if test1 <> 1 then exit 1
if test2 <> 1 then exit 1

exit 0

// ';;' to end FSI session
;;
