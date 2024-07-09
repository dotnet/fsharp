// <testmetadata>
// { "optimization": { "reported_in": "#16302", "reported_by": "@thorium", "last_know_version_not_optimizing": "8", "first_known_version_optimizing": null } }
// </testmetadata>
let foo a b =
  if a then false
  elif b then false
  else true
  
let bar a b =
  not(a || b)
  
System.Console.WriteLine(foo true false)
System.Console.WriteLine(bar true false)