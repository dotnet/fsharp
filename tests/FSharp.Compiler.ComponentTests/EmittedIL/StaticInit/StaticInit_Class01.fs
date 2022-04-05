// #NoMono #NoMT #CodeGen #EmittedIL   
// 
module StaticInit_ClassS01
type C(s:System.DateTime) = 
  class 
    static let x = "1".Length 
    static let f () = x + "2".Length
  end
