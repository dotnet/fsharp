#light




let internal topv = 1
type MyRecord = 
    { x1 : int;
      x2 : int } 
    member obj.X1 = obj.x1
    member obj.X2 = obj.x2
    member obj.TopV = topv
    member obj.TwiceX = obj.x1 + obj.x1
    static member Create(n) = { x1 = n; x2 = n }
    
