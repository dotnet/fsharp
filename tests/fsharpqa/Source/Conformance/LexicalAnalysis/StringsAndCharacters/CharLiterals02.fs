// #Conformance #LexicalAnalysis 
#light

// Character escape sequences

if '\"'B <> 34uy then exit 1
if '\\'B <> 92uy then exit 1
if '\''B <> 39uy then exit 1
if '\n'B <> 10uy then exit 1
if '\r'B <> 13uy then exit 1
if '\t'B <> 9uy then exit 1
if '\b'B <> 8uy then exit 1

if '\001'B <> 1uy then exit 1

exit 0
