module ActivePatternSanityCHeckTests1 = begin

  let (|Foo|Bar|) x = "BAD DOG!"  //  expect: type string doesn't match type 'choice'
  let (|Foo2|Bar2|_|) x = "BAD DOG!"   // expect: invalid name for an active pattern
  let (|Foo2b|Bar2b|Baz2b|_|) x = "BAD DOG!"   //  expect: type string doesn't match type 'choice'
  let (|Foo3|_|) x = "BAD DOG!"   //  expect: type string doesn't match type 'option'

end
module ActivePatternSanityCHeckTests2 = begin

  let (|Foo|Bar|) (a:int) x = "BAD DOG!"  //  expect: type string doesn't match type 'choice'
  let (|Foo2|Bar2|_|) (a:int)  x = "BAD DOG!"   // expect: invalid name for an active pattern
  let (|Foo2b|Bar2b|Baz2b|_|) (a:int) x = "BAD DOG!"   //  expect: type string doesn't match type 'choice'
  let (|Foo3|_|) (a:int) x = "BAD DOG!"   //  expect: type string doesn't match type 'option'
end

module ActivePatternSanityCHeckTests3 = begin

  let rec (|Foo|Bar|) (a:int) x = "BAD DOG!"  //  expect: type string doesn't match type 'choice'
  let rec (|Foo2|Bar2|_|) (a:int) x = "BAD DOG!"   // expect: invalid name for an active pattern
  let rec  (|Foo2b|Bar2b|Baz2b|_|) (a:int) x = "BAD DOG!"   //  expect: type string doesn't match type 'choice'
  let rec (|Foo3|_|) (a:int) x = "BAD DOG!"   //  expect: type string doesn't match type 'option'

end

module ActivePatternIllegalPatterns = begin

  let (|``FooA++``|) (x:int) = x
  let (``FooA++``(x)) = 10

  let (|OneA|``TwoA+``|) (x:int) = if x = 0 then OneA else ``TwoA+``

  let (|``FooB++``|_|) (x:int) = if x = 0 then Some(x) else None
  let (``FooB++``(x)) = 10

end