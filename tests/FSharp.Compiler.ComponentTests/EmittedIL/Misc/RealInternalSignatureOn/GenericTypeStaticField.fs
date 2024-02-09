// #NoMono #NoMT #CodeGen #EmittedIL 
type Foo<'a>() =
  static let theInstance = new Foo<'a>()
  static member Instance = theInstance
  
type Bar<'a,'b>() =
  static let theInstance = new Bar<'a,'b>()
  static member Instance = theInstance
