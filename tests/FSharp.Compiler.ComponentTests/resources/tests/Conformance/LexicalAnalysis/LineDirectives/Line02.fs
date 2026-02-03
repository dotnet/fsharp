// #Regression #Conformance #LexicalAnalysis 
// Regression test for FSHARP1.0:5855
// The following test (when compiled with --parseonly)
// should yield no errors/warnings

type Foo()=
        [<System.Diagnostics.DebuggerNonUserCodeAttribute>]
        abstract __BuildControlForm1 : unit -> global.System.Web.UI.HtmlControls.HtmlForm
        [<System.Diagnostics.DebuggerNonUserCodeAttribute>]
        default this.__BuildControlForm1  () =
            let mutable (__ctrl:global.System.Web.UI.HtmlControls.HtmlForm) = Unchecked.defaultof<_>
            
            __ctrl <- new global.System.Web.UI.HtmlControls.HtmlForm()
            this.Form1 <- __ctrl
                            
            # 6 "C:\Hello.pif"
            __parser.AddParsedSubObject(new System.Web.UI.LiteralControl("<br />\r\n    ")) |> ignore
            let mutable (__ctrl2:global.System.Web.UI.WebControls.Label) = Unchecked.defaultof<_>
            
            
            __parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n    ")) |> ignore
            ((__ctrl :> obj) :?> global.System.Web.UI.HtmlControls.HtmlForm)
