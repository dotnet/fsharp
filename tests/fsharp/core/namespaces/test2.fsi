// Check recursive name resolution
module rec Test2

open Test2.M

module N = 
    val x : C

module M = 
    type C

