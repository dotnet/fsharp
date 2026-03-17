// #Regression #Conformance #LexicalAnalysis
// Verify error if preprocessor directive isn't a valid identifier
//<Expects id="FS3182" status="error">Unexpected character '\*' in preprocessor expression</Expects>
//<Expects id="FS3184" status="error">Incomplete preprocessor expression</Expects>

#if *COMPILED*
exit 0
#endif

exit 1
