// #Regression #Conformance #LexicalAnalysis #Constants 
// Verify error when parsing invalid IEEE64 value
// Regression from TFS 715348

//<Expects id="FS1153" status="error" span="(7,20-7,46)">Invalid floating point number$</Expects>

let tooManyZeros = 0x401E000000000000000000LF
