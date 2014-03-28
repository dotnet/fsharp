module Neg47
open System.Runtime.InteropServices

type Allowed1() =
    [<DllImport("user32.dll")>]
    static let GetCaretBlinkTime() : int = failwith ""
type Allowed2() = member x.P = 1
type Allowed2 with
    [<DllImport("user32.dll")>]
    static member GetCaretBlinkTime() : int = failwith ""

type System.Object with 
    [<DllImport("user32.dll")>]
    static member Allowed3() = true 

type NotAllowed() =
    [<DllImport("user32.dll")>]
    let GetCaretBlinkTime() : int = failwith ""
    do  
        printf "%d" (GetCaretBlinkTime())   

type NotAllowed2() =
    [<DllImport("user32.dll")>]
    member x.GetCaretBlinkTime() : int = failwith ""

let NotAllowed3() =
    { new obj() with 
        [<DllImport("user32.dll")>]
        member x.Equals(y) = true }

type System.Object with 
    [<DllImport("user32.dll")>]
    member x.NotAllowed4() = true 
