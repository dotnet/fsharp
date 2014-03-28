// #Regression #Conformance #ObjectOrientedTypes #Classes #LetBindings #NoMono 
// Regression test for FSharp1.0:4613
// Title: Implicit introduction of lambdas at some constructs leads to "Can't reference protected internals from base class" problems

open System
open System.Windows
open System.Windows.Controls


type TilePanel() =
    inherit Panel()
    
    override x.MeasureOverride(availableSize) =

        let childSize = Size(1.0,1.0);
        
        for child in base.InternalChildren do 
            child.Measure(childSize)
        Size(1.0,1.0)

exit 0
