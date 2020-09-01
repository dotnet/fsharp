// #NoMono #NoMT #CodeGen #EmittedIL 
// Validate we emit non-generic (=fast) code for comparison involving decimals
// See DevDiv:217807 (1.0M < 2.0M should be fast, not go through generic comparison code path)
let _ = 1.0M < 2.0M
let _ = 1.0M <= 2.0M
let _ = 1.0M > 2.0M
let _ = 1.0M >= 2.0M
let _ = 1.0M = 2.0M
let _ = 1.0M <> 2.0M
let _ = 1.0M = 2.0M
let _ = compare 1.0M 2.0M
