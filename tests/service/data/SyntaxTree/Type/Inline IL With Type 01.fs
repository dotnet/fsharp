// Expected: No warning - parentheses context skips validation
module Module
let inline unbox (x: obj) : 'T = 
    (# "unbox.any !0" type ('T) x : 'T #)
