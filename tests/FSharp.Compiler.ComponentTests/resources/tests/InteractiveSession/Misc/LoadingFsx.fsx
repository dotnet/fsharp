// #NoMT #FSI 
// It is ok to #load an .fsx file
//<Expects status="success"></Expects>

let x1 = 1

#if INTERACTIVE

#load "LoadingFsx.fsx"

#endif

let x2 = 2

#q;;
