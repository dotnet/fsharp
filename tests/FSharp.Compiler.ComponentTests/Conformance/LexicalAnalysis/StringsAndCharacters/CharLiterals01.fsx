// #Regression #Conformance #LexicalAnalysis 
#light

// Define trigraph character literals

// NOTE: These values are incorrect based on the spec.
// See FSB #3701

// Trigraph
if '\065' <> 'A' then ignore 1
if '\063' <> '?' then ignore 2
if '\165' <> '¥' then ignore 3
if '\201' <> 'É' then ignore 4
if '\255' <> 'ÿ' then ignore 5

ignore 0
