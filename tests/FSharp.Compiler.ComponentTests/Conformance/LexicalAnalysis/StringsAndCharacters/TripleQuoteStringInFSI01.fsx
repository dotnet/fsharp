// #Conformance #LexicalAnalysis 
// Check how newlines in triple quote strings are interpreted by FSI (depends on feed vs pipe)

let x = """Hello 
""world""";;

if x <> "Hello \n\"\"world" then ignore 1;;

ignore 0;;