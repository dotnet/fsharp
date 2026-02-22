// #Regression #Conformance #Quotations 


// FSB 2384, "Try with in quotations generates a stack overflow

let quote = <@@ try () with _ -> () @@>

// Problem was that we blew up when quoting trywith blocks
exit 0
