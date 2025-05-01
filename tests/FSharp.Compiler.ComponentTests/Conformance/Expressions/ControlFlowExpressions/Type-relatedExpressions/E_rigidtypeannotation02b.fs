// #Regression #Conformance #TypeRelatedExpressions #TypeAnnotations 
// Regression test for FSHARP1.0:2346



[<Measure>] type s
(1.0<s> : obj)
([1.0<s>;2.0<s>;3.0<s>] : seq<_>).GetEnumerator()
