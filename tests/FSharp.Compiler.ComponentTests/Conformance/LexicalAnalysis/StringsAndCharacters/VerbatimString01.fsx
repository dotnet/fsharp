// #Conformance #LexicalAnalysis 
#light

// Test verbatim strings

let test1 = @"\\\\\\"
if test1.Length <> 6 then ignore 1

let test2 = @"\t"
if test2.Length <> 2 then ignore 1
if test2.[0] <> '\\' then ignore 1
if test2.[1] <> 't'  then ignore 1

ignore 0
