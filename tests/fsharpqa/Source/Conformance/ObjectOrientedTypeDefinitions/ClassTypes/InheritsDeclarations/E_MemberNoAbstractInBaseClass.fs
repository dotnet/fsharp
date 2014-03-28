// #Regression #Conformance #ObjectOrientedTypes #Classes #Inheritance 
// Regression test for FSHARP1.0:5421
// Assert/ICE when compiling code with override/default
// Note: There are a few more errors in the code, but the point here is just to validate that we do not ICE
//       This is the same as E_DefaultNoAbstractInBaseClass.fs - the only difference is 'member' instead of 'default' on line 9
//<Expects status="error" span="(22,16)" id="FS0859">No abstract property was found that corresponds to this override$</Expects>
module N.M

type public FrameworkElement() =
                                        member this.VisualChildrenCount
                                            with get () = 1
                                        member this.GetVisualChild(index : int) = 
                                             0
                                        member this.LogicalChildren = 0

type DecoratorTest() as this =
    inherit FrameworkElement()
    let mutable content = 0
    member x.Content 
        with get () = content

    override x.VisualChildrenCount 
        with get () = 1

    override x.GetVisualChild(index) = 
        this.Content :> Visual

    override this.LogicalChildren
        with get () =
            let count =  this.VisualChildrenCount
            let firstchild = this.GetVisualChild(0)
            // Error: Method or object constructor 'GetVisualChild' not found.            
            let child i = this.GetVisualChild(i)
            // Error: Unexpected error: empty property list 
            let elements = seq { for i in 0 .. this.VisualChildrenCount do yield FrameworkElement() } 
            // Error: Method or object constructor 'GetVisualChild' not found.
            let children = seq { for i in 0 .. count do yield this.GetVisualChild(i) } 
            children.GetEnumerator() :> IEnumerator
