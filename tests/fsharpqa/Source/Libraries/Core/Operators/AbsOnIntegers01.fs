// #Regression #Libraries #Operators 
// Regression test for FSHARP1.0:3470 - exception on abs of native integer

#light

let res = 
       abs -1y = 1y   // signed byte
    && abs -1s = 1s   // int16
    && abs -1l = 1l   // int32
    && abs -1n = 1n   // nativeint
    && abs -1L = 1L   // int64
    && abs -1I = 1I   // bigint
      
if not res then exit 1
exit 0
