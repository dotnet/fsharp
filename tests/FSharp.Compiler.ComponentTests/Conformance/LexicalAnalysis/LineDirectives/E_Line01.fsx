// #Regression #Conformance #LexicalAnalysis 


open System

// Test setting the 'line', which we validate by checking the line/col span in errors

//<Expects id="FS1156" span="(1006,9)" status="error">This is not a valid numeric literal. Valid numeric literals include</Expects>

# 1000 "Line01.fs"
                                        // 1000
let x = 7                               // 1001
                                        // 1002
let _ = "string 1"                      // 1003
let _ = "string 2"                      // 1004
                                        // 1005
let y = 7T                              // 1006
