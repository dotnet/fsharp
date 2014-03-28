// #Libraries #Operators 
#light

// Test the round function

// Identity
for i in [1 .. 10000] do
    if i |> float   |> round <> float   i then exit 1
    if i |> float32 |> round <> float32 i then exit 1
    if i |> decimal |> round <> decimal i then exit 1

// Round down
if round 1.1 <> 1.0 then exit 1
if round 1.2 <> 1.0 then exit 1
if round 1.3 <> 1.0 then exit 1
if round 1.4 <> 1.0 then exit 1

if round 1.1f <> 1.0f then exit 1
if round 1.2f <> 1.0f then exit 1
if round 1.3f <> 1.0f then exit 1
if round 1.4f <> 1.0f then exit 1

if round 1.1m <> 1.0m then exit 1
if round 1.2m <> 1.0m then exit 1
if round 1.3m <> 1.0m then exit 1
if round 1.4m <> 1.0m then exit 1

// Round down
if round 1.6 <> 2.0 then exit 1
if round 1.7 <> 2.0 then exit 1
if round 1.8 <> 2.0 then exit 1
if round 1.9 <> 2.0 then exit 1

if round 1.6f <> 2.0f then exit 1
if round 1.7f <> 2.0f then exit 1
if round 1.8f <> 2.0f then exit 1
if round 1.9f <> 2.0f then exit 1

if round 1.6m <> 2.0m then exit 1
if round 1.7m <> 2.0m then exit 1
if round 1.8m <> 2.0m then exit 1
if round 1.9m <> 2.0m then exit 1

// Midpoint rounding. If between two numbers, round to the 'even' one.

if round 1.5  <> 2.0  then exit 1
if round 1.5f <> 2.0f then exit 1
if round 1.5m <> 2.0m then exit 1

if round 2.5  <> 2.0  then exit 1
if round 2.5f <> 2.0f then exit 1
if round 2.5m <> 2.0m then exit 1

// If not midpoint, round to nearest as usual

if round 2.500001  <> 3.0  then exit 1
if round 2.500001f <> 3.0f then exit 1
if round 2.500001m <> 3.0m then exit 1

exit 0
