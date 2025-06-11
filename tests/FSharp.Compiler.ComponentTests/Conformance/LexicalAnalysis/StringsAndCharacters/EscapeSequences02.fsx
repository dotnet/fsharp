// #Conformance #LexicalAnalysis 
#light

// Verify behavior of invalid escape sequences

// '\\' escape sequence for single backslash
if "\\"B <> [|92uy|] then ignore 1

// '\G' is not a valid escape sequence, treated as '\' 'G' combo
if "\G"B <> [|92uy; 71uy|] then ignore 1

// \N, \NN are nothing special. \NNN is a trigraph.
if "\0".Length      <> 2 then ignore 1
if "\00".Length     <> 3 then ignore 1
if "\000".Length    <> 1 then ignore 1
if "\0000".Length   <> 2 then ignore 1
if "\00000".Length  <> 3 then ignore 1

ignore 0
