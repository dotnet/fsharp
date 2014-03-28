// #Regression #Conformance #LexicalAnalysis 
// Verify error if preprocessor directive isn't a valid identifier
//<Expects id="FS1169" status="error">#if directive should be immediately followed by an identifier</Expects>

#if *COMPILED*
exit 0
#endif

exit 1
