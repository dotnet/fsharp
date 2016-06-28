// #Warnings
//<Expects status="Error" span="(6,5)" id="FS0588">The block following this 'let' is unfinished. Every code block is an expression and must have a result. 'let' cannot be the final code element in a block. Consider giving this block an explicit result.</Expects>

let sum = 0
for x in 0 .. 10 do
    let sum = sum + x
    
exit 0