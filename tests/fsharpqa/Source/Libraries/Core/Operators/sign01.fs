// #Libraries #Operators 
#light

// Test the 'sign' library function

// Positive sign
if sign 1y   <> 1 then exit 1  // byte
if sign 1s   <> 1 then exit 1  // int16
if sign 1    <> 1 then exit 1  // int32
if sign 1L   <> 1 then exit 1  // int64
if sign 1.0f <> 1 then exit 1  // float
if sign 1.0  <> 1 then exit 1  // double
if sign 1.0m <> 1 then exit 1  // decimal

// Zero
if sign 0y   <> 0 then exit 1  // byte
if sign 0s   <> 0 then exit 1  // int16
if sign 0    <> 0 then exit 1  // int32
if sign 0L   <> 0 then exit 1  // int64
if sign 0.0f <> 0 then exit 1  // float
if sign 0.0  <> 0 then exit 1  // double
if sign 0.0m <> 0 then exit 1  // decimal

// Negative sign
if sign -1y   <> -1 then exit 1  // byte
if sign -1s   <> -1 then exit 1  // int16
if sign -1    <> -1 then exit 1  // int32
if sign -1L   <> -1 then exit 1  // int64
if sign -1.0f <> -1 then exit 1  // float
if sign -1.0  <> -1 then exit 1  // double
if sign -1.0m <> -1 then exit 1  // decimal

// All clear!
exit 0
