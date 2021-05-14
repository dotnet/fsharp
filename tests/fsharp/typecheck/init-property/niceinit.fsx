#r @"niceinit.dll"

let niceinit = NiceInit(Val = 1);;

let niceinit2 = NiceInit();;
niceinit2.Val <- 5;;
niceinit2.set_Val 5;;