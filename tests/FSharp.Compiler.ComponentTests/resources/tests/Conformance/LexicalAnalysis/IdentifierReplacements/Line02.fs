// #Conformance #LexicalAnalysis
#light                                                                          // 2
                                                                                // 3
// Test ability for a function to 'return' the current source line directive.   // 4
                                                                                // 5
let currentLineToLower() = __LINE__.ToLower()                                   // 6
                                                                                // 7
if currentLineToLower() <> "6" then exit 1                                      // 8
exit 0                                                                          // 9

