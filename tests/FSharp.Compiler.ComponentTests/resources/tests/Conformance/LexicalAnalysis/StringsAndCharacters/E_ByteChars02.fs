// #Regression #Conformance #LexicalAnalysis 

// Verify getting the byte value of a char works past the first 128 ASCII characters
// It is not valid because it must be <= 127
//<Expects id="FS1157" span="(7,4-7,8)" status="error">This is not a valid byte character literal. The value must be less than or equal to '\\127'B.</Expects>

if 'ú'B <> 250uy  then exit 1

exit 1
