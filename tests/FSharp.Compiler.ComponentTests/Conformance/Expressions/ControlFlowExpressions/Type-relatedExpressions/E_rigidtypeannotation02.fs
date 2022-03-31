// #Regression #Conformance #TypeRelatedExpressions #TypeAnnotations 
// Regression test for FSHARP1.0:2346
//<Expects id="FS0001" span="(7,2-7,3)" status="error">This expression was expected to have type.    'obj'    .but here has type.    'int'</Expects>
//<Expects id="FS0001" span="(8,2-8,9)" status="error">This expression was expected to have type.    'obj'    .but here has type.    'string'</Expects>
//<Expects id="FS0001" span="(9,2-9,15)" status="error">This expression was expected to have type.    'seq<'a>'    .but here has type.    ''b list'</Expects>

(1 : obj)
("Hello" : obj)
([1.0;2.0;3.0] : seq<_>).GetEnumerator()


