// #Regression #Conformance #LexFilter 
// Verify error on unclosed let-block
// Regression for FSB 1616

//<Expects id="FS0588" status="error" span="(10,5)">The block following this 'let' is unfinished. Every code block is an expression and must have a result. 'let' cannot be the final code element in a block. Consider giving this block an explicit result.</Expects>

// Note how the compiler is left hanging for what completes the function...

let f x =
    let n = 42
