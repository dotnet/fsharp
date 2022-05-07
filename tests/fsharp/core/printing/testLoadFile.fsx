#light
let x1 = 1
let x2 = "hello"
let x3 = None
let x4 = None : int option
let x5 = []
let x6 = [1;2;3]
let x7 = new System.Windows.Forms.Form(Text="x7 form")
let x8 = Array2D.init 5 5 (fun i j -> i*10 + j)
let x9 = lazy (exit 999; "this lazy value should not be forced!!")  
type ClassInFile1() = class end