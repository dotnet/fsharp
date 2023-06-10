// #Regression #Conformance #TypesAndModules 
// Abbreviation: it is not allowed to drop variable types
// Regression test for FSHARP1.0:3740
//<Expects id="FS0035" span="(6,6-6,10)" status="error">This construct is deprecated: This type abbreviation has one or more declared type parameters that do not appear in the type being abbreviated\. Type abbreviations must use all declared type parameters in the type being abbreviated\. Consider removing one or more type parameters, or use a concrete type definition that wraps an underlying type, such as 'type C<'a> = C of \.\.\.'\.</Expects>

type Drop<'a,'b> = 'a * 'a

exit 1
