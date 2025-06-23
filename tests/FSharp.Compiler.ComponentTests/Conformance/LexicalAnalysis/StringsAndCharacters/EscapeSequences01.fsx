// #Conformance #LexicalAnalysis 
#light

// Verify String Escape sequences

// Quote
if '\"'B <> 34uy then ignore 1
if " \" "B <> [|32uy; 34uy; 32uy|] then ignore 1

// Backslash
if '\\'B <> 92uy then ignore 1
if " \\ "B <> [|32uy; 92uy; 32uy|] then ignore 1

// Apostrophe
if '\''B <> 39uy then ignore 1
if " \' "B <> [|32uy; 39uy; 32uy|] then ignore 1

// Newline
if '\n'B <> 10uy then ignore 1
if " \n "B <> [|32uy; 10uy; 32uy|] then ignore 1

// Carriage Return
if '\r'B <> 13uy then ignore 1
if " \r "B <> [|32uy; 13uy; 32uy|] then ignore 1

// Tab
if '\t'B <> 9uy then ignore 1
if " \t "B <> [|32uy; 9uy; 32uy|] then ignore 1

// Bell
if '\b'B <> 8uy then ignore 1
if " \b "B <> [|32uy; 8uy; 32uy|] then ignore 1

// Character trigraph
if "\001"B <> [|1uy|] then ignore 1

// Unicode (short)
if "\u2660".Length <> 1 then ignore 1

// Unicode (long)
if "\U00002660".Length <> 1 then ignore 1

ignore 0
