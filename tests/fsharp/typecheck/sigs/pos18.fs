// This test is related to F# 4.0+ design change "https://fslang.uservoice.com/forums/245727-f-language/suggestions/6107641-make-microsoft-prefix-optional-when-using-core-f"
//
// In this test, we check we can do basic operations like "open FSharp.Collections" without needing the "Microsoft" prefix

module Pos18 = 
  // Check we can open 'FSharp.Collections' unqualified, without warning
  open FSharp.Collections

  // Check we can open 'FSharp.Quotations' unqualified, without warning
  open FSharp.Quotations

  let v = Expr.Value 3

  // Check we can access 'FSharp.Quotations' via full path
  let v2 = FSharp.Quotations.Expr.Value 3

  // Check we can access 'FSharp.Quotations' via 'global' 
  let v3 = global.FSharp.Quotations.Expr.Value 3

module Pos18b = 
  open Microsoft // Adding "open Microsoft" shouldn't trigger additional warnings
  
  // Check we can open 'FSharp.Collections' unqualified, without warning
  open FSharp.Collections

  // Check we can open 'FSharp.Quotations' unqualified, without warning
  open FSharp.Quotations

  let v = Expr.Value 3

  // Check we can access 'FSharp.Quotations' via full path
  let v2 = FSharp.Quotations.Expr.Value 3

  // Check we can access 'FSharp.Quotations' via 'global' 
  let v3 = global.FSharp.Quotations.Expr.Value 3
