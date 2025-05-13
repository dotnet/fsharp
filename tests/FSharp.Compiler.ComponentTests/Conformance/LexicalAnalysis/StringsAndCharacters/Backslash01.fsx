// #Conformance #LexicalAnalysis 
#light

// Test the backslash functionality when defining strings (continues on next line)

let test = "abc\
def"

if test <> "abcdef" then ignore 1

ignore 0
