// #Regression #NoMT #CompilerOptions #RequiresPowerPack 
// Regression test for FSHARP1.0:4271
// Big mistery: compiler does not produce the expected .dll when the command line argruments are rearranged
//<Expects status="success"></Expects>

module fscArgOrder.fscArgOrder

val v1 : int
