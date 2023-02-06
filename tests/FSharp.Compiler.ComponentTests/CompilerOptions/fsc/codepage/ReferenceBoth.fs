// #NoMT #CompilerOptions 
// This test is designed to test the --codepage options.
// - The same source file (libCodepage.fs) is compile twice, with different --codepage options
// - The library file is designed so that it gets compiled to slightly different assemblies
// - This file references both assemblies and should compile without problems
//<Expects status="success"></Expects>
#light


let a = N.M.á.M()
let b = N.M.б.M()

if a = b then () else failwith "Failed: 1"