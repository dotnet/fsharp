// <testmetadata>
// { "optimization": { "reported_in": "#9548", "reported_by": "@teo-tsirpanis", "last_know_version_not_optimizing": "8", "first_known_version_optimizing": null } }
// </testmetadata>

open System

let g() =
  for x in 0 .. 2 .. 15 do
    Console.WriteLine x
g()