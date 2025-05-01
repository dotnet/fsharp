// #Regression #Conformance #DeclarationElements #Fields #MemberDefinitions 
// Regression for Dev10:840199
// More extensive tests in fsharp\devdiv\src\tests\fsharp\core\members\basics\test-hw.ml

type Foo<'a>() =
  static let theInstance = new Foo<'a>()
  static member Instance = theInstance

type Bar<'a,'b>() =
  static let theInstance = new Bar<'a,'b>()
  static member Instance = theInstance

// Equi-recursive type defs
type Rec1<'a>() = 
  static let rec2Instance = new Rec2<'a>()
  static let rec1Instance = new Rec1<'a>()
  static member Rec2Instance = rec2Instance
  static member Rec1Instance = rec1Instance

and Rec2<'a>() =
  static let rec1Instance = new Rec1<'a>()
  static let rec2Instance = new Rec2<'a>()
  static member Rec1Instance = rec1Instance
  static member Rec2Instance = rec2Instance

// recursive type defs - multi tyargs
type Rec1AB<'a,'b>() = 
  static let rec2Instance = new Rec2B<'a>()
  static let rec1Instance = new Rec1AB<'a,'b>()
  static member Rec2Instance = rec2Instance
  static member Rec1Instance = rec1Instance

and Rec2B<'a>() =
  static let rec2Instance = new Rec2B<'a>()
  static member Rec2Instance = rec2Instance 
