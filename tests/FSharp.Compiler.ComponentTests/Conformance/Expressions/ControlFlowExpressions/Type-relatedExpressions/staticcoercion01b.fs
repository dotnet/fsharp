// #Conformance #TypeRelatedExpressions #TypeAnnotations 
#light

[<Measure>] type s

(1.0<s> :> obj)
("Hello" :> obj)
([1.0<s>;2.0<s>;3.0<s>] :> seq<_>).GetEnumerator()
(upcast 1.0<s> : obj)
