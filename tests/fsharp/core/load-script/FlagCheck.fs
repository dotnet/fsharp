// #Conformance #FSI 
module FlagCheck 
#nowarn "22"
#if INTERACTIVE
printfn "INTERACTIVE is defined"
#endif
#if COMPILED
printfn "COMPILED is defined"
#endif
