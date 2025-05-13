// #Conformance #LexicalAnalysis 
#light

// Test strange but valid identifiers

let _startWithUnderscore            = true
let __startWithTwoUnderscore        = true

let no_8_leading_digit_1234         = true

let ``backTicks``                   = true
let ``backTicks`with`single`ticks`` = true
let ``backticks with spaces``       = true
let ``0 leading number``            = true

if not _startWithUnderscore          then ignore 1
if not __startWithTwoUnderscore      then ignore 1
if not no_8_leading_digit_1234       then ignore 1
if not ``backTicks``                 then ignore 1
if not ``backTicks`with`single`ticks`` then ignore 1
if not ``backticks with spaces``     then ignore 1
if not ``0 leading number``          then ignore 1

ignore 0
