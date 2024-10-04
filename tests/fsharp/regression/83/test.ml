// #Regression 
let failures = ref false
let report_failure () = 
  System.Console.Error.WriteLine " NO"; failures := true

open System
open System.Windows.Forms

let form = new Form()

let lblHello = new Label()
let btnSay = new Button()

let btnSay_Click(sender, e) =
    lblHello.Text <- "Hello"

let form_Load(sender, e) =
    btnSay.add_Click(new EventHandler(fun sender e -> btnSay_Click(sender, e)))



let _ = lblHello.Location <- new System.Drawing.Point(16, 16)
let _ = lblHello.Name <- "lblHello"
let _ = lblHello.Size <- new System.Drawing.Size(72, 23)
let _ = lblHello.TabIndex <- 0

let _ = btnSay.Location <- new System.Drawing.Point(216, 16)
let _ = btnSay.Name <- "btnApply"
let _ = btnSay.TabIndex <- 1
let _ = btnSay.Text <- "Apply"

let _ = form.Text <- "1st F# App"
let _ = form.add_Load(new EventHandler(fun sender e -> form_Load(sender, e)))
let _ = form.Controls.AddRange(Array.ofList [(upcast (lblHello) : Control);
                                            (upcast (btnSay) : Control);
                                                        ])
 
(* let _ = Application.Run(form) *)

let _ = 
  if !failures then (System.Console.Out.WriteLine "Test Failed"; exit 1) 

do (System.Console.Out.WriteLine "Test Passed"; 
    printf "TEST PASSED OK"; 
    exit 0)

