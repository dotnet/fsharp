// #Conformance #LexicalAnalysis
#light                                                                          // 2
                                                                                // 3
// Test ability for a function to 'return' the current source line directive.   // 4
                                                                                // 5
let currentLineToLower() = __LINE__.ToLower()                                   // 6
                                                                                // 7
if currentLineToLower() <> "6" then ignore 1                                      // 8
ignore 0                                                                          // 9

