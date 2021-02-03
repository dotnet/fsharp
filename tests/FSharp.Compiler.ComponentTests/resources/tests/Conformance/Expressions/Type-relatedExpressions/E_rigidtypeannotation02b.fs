// #Regression #Conformance #TypeRelatedExpressions #TypeAnnotations 
// Regression test for FSHARP1.0:2346
//<Expects id="FS0001" span="(7,2-7,8)" status="error">This expression was expected to have type.    'obj'    .but here has type.    'float<s>'</Expects>
//<Expects id="FS0001" span="(8,2-8,24)" status="error">This expression was expected to have type.    'seq<'a>'    .but here has type.    ''b list'</Expects>

[<Measure>] type s
(1.0<s> : obj)
([1.0<s>;2.0<s>;3.0<s>] : seq<_>).GetEnumerator()
