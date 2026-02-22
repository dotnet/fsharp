// #Regression #NoMT #FSI 
// Regression test for FSHARP1.0:5629
//<Expects status="error" span="(4,9)" id="FS0588">The block following this 'let' is unfinished. Every code block is an expression and must have a result. 'let' cannot be the final code element in a block. Consider giving this block an explicit result.</Expects>
let x = let y = 2;;
exit 1;;
