// #Regression #Conformance #LexicalAnalysis 
// Regression test for FSHARP1.0:1419
//<Expects id="FS1169" span="(7,13-7,16)" status="error">#if directive should be immediately followed by an identifier</Expects>
//<Expects id="FS0010" span="(8,12-8,19)" status="error">#endif has no matching #if in pattern</Expects>
//<Expects id="FS0583" span="(8,8-8,9)" status="error">Unmatched '\('</Expects>
#light
let t8 (x : #if_) = ()
let t7 (x : #endif_) = ()

exit 1
