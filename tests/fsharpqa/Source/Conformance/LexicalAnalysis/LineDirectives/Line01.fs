// #Regression #Conformance #LexicalAnalysis 


open System

// Test setting the 'line', which we validate by checking the line/col span in errors

//<Expects id="FS1156" span="(1006,9)" status="error">This is not a valid numeric literal\. Sample formats include 4, 0x4, 0b0100, 4L, 4UL, 4u, 4s, 4us, 4y, 4uy, 4\.0, 4\.0f, 4I</Expects>

# 1000 "Line01.fs"
                                        // 1000
let x = 7                               // 1001
                                        // 1002
let _ = "string 1"                      // 1003
let _ = "string 2"                      // 1004
                                        // 1005
let y = 7T                              // 1006
