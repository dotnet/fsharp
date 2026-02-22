// #Conformance #LexicalAnalysis 
// Check how newlines in triple quote strings are interpreted by FSI (depends on feed vs pipe)

let x = """Hello 
""world""";;

if x <> "Hello \r\n\"\"world" then exit 1;;

exit 0;;