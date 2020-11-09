// #Regression #Conformance #LexicalAnalysis 
#light

// Define trigraph character literals

// NOTE: These values are incorrect based on the spec.
// See FSB #3701

// Trigraph
if '\065' <> 'A' then exit 1
if '\063' <> '?' then exit 2
if '\165' <> '¥' then exit 3
if '\201' <> 'É' then exit 4
if '\255' <> 'ÿ' then exit 5

exit 0
