// #Regression #Conformance #BasicGrammarElements #Constants 
#light

// Regression test for FSharp1.0: 2956 - F# should have parity with C# wrt escape characters

let isEscape =
    true
    && ('\a' = char 7 ) // alert
    && ('\b' = char 8 ) // backspace
    && ('\t' = char 9 ) // horizontal tab
    && ('\n' = char 10) // new line
    && ('\v' = char 11) // vertical tab
    && ('\f' = char 12) // form feed
    && ('\r' = char 13) // return
    && ('\"' = char 34) // double quote
    && ('\'' = char 39) // single quote
    && ('\\' = char 92) // backslash

if not isEscape then exit 1

exit 0
