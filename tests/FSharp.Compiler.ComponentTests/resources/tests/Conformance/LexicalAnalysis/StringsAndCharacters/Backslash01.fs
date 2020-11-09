// #Conformance #LexicalAnalysis 
#light

// Test the backslash functionality when defining strings (continues on next line)

let test = "abc\
def"

if test <> "abcdef" then exit 1

exit 0
