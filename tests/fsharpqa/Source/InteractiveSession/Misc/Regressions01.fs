// #Regression #NoMT #FSI #NoMono 
// Regression for FSB 1711
// Generic Interface requires a method implementation that in some cases is not supported by FSI

type IFoo<'a> =
    abstract InterfaceMethod<'b> : 'a -> 'b

type Foo<'a, 'b>() =
    interface IFoo<'a> with
        override this.InterfaceMethod (x : 'a) = (Array.zeroCreate 1).[0]
    override this.ToString() = "Foo"

;;

let test = new Foo<string, float>()

if (test :> IFoo<_>).InterfaceMethod null <> 0.0 then exit 1

exit 0;;
                   
