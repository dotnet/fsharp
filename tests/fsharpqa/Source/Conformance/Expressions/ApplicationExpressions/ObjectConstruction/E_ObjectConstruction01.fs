// #Regression #Conformance #ApplicationExpressions #ObjectConstructors 
// Verify error for inaccessible constructor
//<Expects id="FS0039" status="error" span="(5,37)">The type 'BitArray' does not define a field, constructor or member 'BitArrayEnumeratorSimple'</Expects>

let y = System.Collections.BitArray.BitArrayEnumeratorSimple
()

