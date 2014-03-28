// #Regression #Conformance #LexFilter 
// Verify error on unclosed let-block
// Regression for FSB 1616

//<Expects id="FS0588" status="error" span="(10,5)">Block following this 'let' is unfinished\. Expect an expression</Expects>

// Note how the compiler is left hanging for what completes the function...

let f x =
    let n = 42
