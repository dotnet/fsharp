// #Conformance #LexicalAnalysis 
// Some basic checks duplicated from fsharp\core\libtest and then some

open System

let nl = Environment.NewLine

let check arg expected =
    if arg <> expected then
        printfn "Expected %A = %A" expected arg
        exit 1

check """Hello world""" "Hello world"
check """Hello "world""" "Hello \"world"
check """Hello ""world""" "Hello \"\"world"

//check newlines
check """Hello 
""world""" ("Hello " + nl + "\"\"world")
check """Hello 
 ""world""" ("Hello " + nl + " \"\"world")
check """Hello 

""world""" ("Hello " + nl + nl + "\"\"world")

// check tabs
check """Hello 	 world""" "Hello \t world"
check """Hello		
world""" ("Hello\t\t" + nl + "world")

// check there is no escaping...
check """Hello \"world""" "Hello \\\"world"
check """Hello \\"world""" "Hello \\\\\"world"
check """Hello \nworld""" "Hello \\nworld"
check """Hello \""" "Hello \\"
check """Hello \\""" "Hello \\\\"
check """Hello \n""" "Hello \\n"

// check some embedded comment terminators
check (* """Hello *) world""" *) """Hello world""" "Hello world"
check (* (* """Hello *) world""" *) *) """Hello world""" "Hello world"
check (* """Hello *) "world""" *) """Hello "world""" "Hello \"world"

// check some identifiers
check """int""" "int"
check """string 1""" "string 1"
check """type
T""" ("type" + nl + "T")

exit 0