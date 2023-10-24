module Neg60
// Tests that we get an error when custom operations are overloaded
module QuerySyntaxForAnFSharpType = 

    open System
    open Microsoft.FSharp.Quotations

    type EventBuilder() = 
        member __.For(ev:IObservable<'T>, loop:('T -> #IObservable<'U>)) : IObservable<'U> = failwith ""
        member __.Yield(v:'T) : IObservable<'T> = failwith ""
        member __.Quote(v:Quotations.Expr<'T>) : Expr<'T> = v
        member __.Run(x:Expr<'T>) = Microsoft.FSharp.Linq.RuntimeHelpers.LeafExpressionConverter.EvaluateQuotation x :?> 'T
         
        [<CustomOperation("where",MaintainsVariableSpace=true)>]
        member __.Where (x, [<ProjectionParameter>] f) = Observable.filter f x
         
        [<CustomOperation("notValid1",MaintainsVariableSpace=true,MaintainsVariableSpaceUsingBind=true)>]
        member __.NotValid1 (x, [<ProjectionParameter>] f) = Observable.filter f x
         
        [<CustomOperation("notValid2",IsLikeZip=true,IsLikeJoin=true)>]
        member __.NotValid2 (x, [<ProjectionParameter>] f) = Observable.filter f x
         
        [<CustomOperation("notValid3",IsLikeZip=true,IsLikeGroupJoin=true)>]
        member __.NotValid3 (x, [<ProjectionParameter>] f) = Observable.filter f x
         
        [<CustomOperation("notValid4",IsLikeJoin=true,IsLikeGroupJoin=true)>]
        member __.NotValid4 (x, [<ProjectionParameter>] f) = Observable.filter f x
         
        // Note: overloaded, this gives the error further below
        [<CustomOperation("where")>]
        member __.Select (x, [<ProjectionParameter>] f) = Observable.map f x

        [<CustomOperation("choose")>]
        member __.Choose (x, [<ProjectionParameter>] f) = Observable.choose f x

    // intrinsic extension member
    type EventBuilder with 
        [<CustomOperation("action",MaintainsVariableSpace=true)>]
        member __.Action (source:IObservable<_>, [<ProjectionParameter>] f)  = Observable.add f source; source

    // extrinsic extension member
    /// Check that a custom operator defined by an extension member works
    [<AutoOpen>]
    module ExtensionMembers = 
        type EventBuilder with 
            [<CustomOperation("scanSumBy")>]
            member inline __.ScanSumBy (source, [<ProjectionParameter>] f : 'T -> 'U) : IObservable<'U> = Observable.scan (fun a b -> a + f b) LanguagePrimitives.GenericZero<'U> source


    let myquery = EventBuilder()
    let mybuilder = EventBuilder()

    //---------------------------------------------------------------

    type PretendForm() = 
    
       let event = new Event<int * int >()
       member x.Tick = event.Trigger(10,10)
       member x.MouseClick = event.Publish
    //let f = new System.Windows.Forms.Form()
    let f = new PretendForm()

    let e1 =     
      myquery { for x in f.MouseClick do 
                  where (fst x < 100)
                  scanSumBy (snd x) }

module NullableOperators1 = 


    let x : System.Nullable<int> = null


module NullableOperators2 = 
 
    let iq = System.Nullable<int>(10)
    let _ = iq ?>= iq 
    let _ = iq ?>= 10 
    let _ = 1 + (1,2) 
    let _ = (2,3) + (1,2) 
    let _ = (2,3) + (fun () -> ())

module NullableOperators3 = 
 
    let a = new System.Nullable<int>(0)
    let b = new System.Nullable<int>(1)
    if a ?=? b then printfn "Same" else printfn "Different"

module QuerySyntaxWithValidOverloading = 

    open System

    type Content = ArraySegment<byte> list

    type ContentBuilder() =
        member this.Run(c: Content) =
            let crlf = "\r\n"B
            [|for part in List.rev c do
                yield! part.Array.[part.Offset..(part.Count+part.Offset-1)]
                yield! crlf |]

        member this.Yield(_) = []

        [<CustomOperation("body")>]
        member this.Body(c: Content, segment: ArraySegment<byte>)  =
            segment::c

        [<CustomOperation("body")>]
        member this.Body(c: Content, bytes: byte[], offset, count) =
            ArraySegment<byte>(bytes, offset, count)::c

        [<CustomOperation("body")>]
        member this.Body(c: Content, [<ParamArray>] contents: string[]) =
            let rec loop acc (contents: string list) =
                match contents with
                | [] -> acc
                | content::rest ->
                    let bytes = Text.Encoding.ASCII.GetBytes(content)
                    loop (this.Body(c, bytes, 0, bytes.Length)) rest
            loop c (List.ofArray contents)

    let content = ContentBuilder()

    //---------------------------------------------------------------

    let values =
      content {
        body "Name"
        body (ArraySegment<_>("Email"B, 0, 5))
        body "Password"B 2 4
        body "Description" "of" "content"
      }

module QuerySyntaxWithOptionalParamAndParamsArray = 

    open System

    type InputKind =
        | Text of placeholder:string option
        | Password of placeholder: string option

    type InputOptions =
      { Label: string option
        Kind : InputKind
        Validators : (string -> bool) array }

    type InputBuilder() =

        member t.Yield(_) = 
          { Label = None
            Kind = Text None
            Validators = [||] }
            
        [<CustomOperation("text")>]
        member this.Text(io,?placeholder) =
            { io with Kind = Text placeholder }
            
        [<CustomOperation("password")>]
        member this.Password(io,?placeholder) =
            { io with Kind = Password placeholder }
            
        [<CustomOperation("label")>]
        member this.Label(io,label) = 
            { io with Label = Some label }
            
        [<CustomOperation("with_validators")>]
        member this.Validators(io, [<ParamArray>] validators) =
            { io with Validators = validators }
        
    let input = InputBuilder()

    //---------------------------------------------------------------

    let name =
      input {
        label "Name"
        text
        with_validators
            (String.IsNullOrWhiteSpace >> not)
      }
            
    let email =
      input {
        label "Email"
        text "Your email"
        with_validators
            (String.IsNullOrWhiteSpace >> not)
            (fun s -> s.Contains "@")
      }
            
    let password =
      input {
        label "Password"
        password "Must contains at least 6 characters, one number and one uppercase"
        with_validators
            (String.exists Char.IsUpper)
            (String.exists Char.IsDigit)
            (fun s -> s.Length >= 6)
      }
