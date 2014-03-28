// #Regression #Conformance #ApplicationExpressions #ObjectConstructors 
// Verify error for inaccessible constructor
//<Expects id="FS0039" status="error" span="(5,37)">The field, constructor or member 'BitArrayEnumeratorSimple' is not defined</Expects>

let y = System.Collections.BitArray.BitArrayEnumeratorSimple
()

