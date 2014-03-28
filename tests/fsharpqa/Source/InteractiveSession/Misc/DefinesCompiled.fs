// #NoMT #FSI 
#light

// Verify COMPILED is defined for all compiled .fs files

let test1 = 
    #if COMPILED
    1
    #else
    0
    #endif

// INTERACTIVE should NOT be defined
let test2 = 
    #if INTERACTIVE
    0
    #else
    1
    #endif

if test1 <> 1 then exit 1
if test2 <> 1 then exit 1

exit 0
