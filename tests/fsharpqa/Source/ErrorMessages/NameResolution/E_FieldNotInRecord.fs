// #ErrorMessages #NameResolution 
//<Expects status="error" span="(11,31)" id="FS1129">The record type 'F' does not contain a label 'Wall'\.</Expects>

type A = { Hello:string; World:string }
type B = { Size:int; Height:int }
type C = { Wheels:int }
type D = { Size:int; Height:int; Walls:int }
type E = { Unknown:string }
type F = { Wallis:int; Size:int; Height:int; }

let r:F = { Size=3; Height=4; Wall=1 }

exit 0
