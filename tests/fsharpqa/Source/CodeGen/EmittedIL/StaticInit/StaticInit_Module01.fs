// #NoMono #NoMT #CodeGen #EmittedIL   
//
module StaticInit_Module01
module M = 
  let x = "1".Length 
  module N = 
    let y = x + "2".Length 
    let z = y + "3".Length 
