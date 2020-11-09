// #Regression #Conformance #UnitsOfMeasure #ObjectOrientedTypes 
// Test error when overloading instance methods that differs only by units of measure attributes.
//<Expects id="FS0438" span="(9,5)" status="error">Duplicate method\. The method '\.ctor' has the same name and signature as another method in type 'Foo<'a>' once tuples, functions, units of measure and/or provided types are erased\.</Expects>

type Foo< [<Measure>] 'a > =

    val m_value : decimal<'a>
    member this.Value = this.m_value
    new(x : decimal<'a>) = { m_value = x }
    new(y : decimal) = { m_value = unbox (box y) }
