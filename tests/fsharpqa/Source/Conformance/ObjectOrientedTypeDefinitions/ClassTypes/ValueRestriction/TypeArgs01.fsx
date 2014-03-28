// #Regression #Conformance #ObjectOrientedTypes #Classes # ValueRestriction

//<Expects status="success"></Expects>

// This was originally regression test for FSHARP1.0:877 - "implicit class members typars are not the typars of the enclosing class"
// After discussions on 'fsbugs' it was confirmed that the right behavior is to accept this code, i.e. 'xs' is a "obj list"


type 'a fifo() = class
  let mutable xs  = []
  member q.ToList() = xs
end
