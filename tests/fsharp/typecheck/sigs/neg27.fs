module Test

type NonComp() = //class end

//[<StructuralEquality(true)>]
//[<StructuralComparison(true)>]
type MyAlg =


    | Foo of NonComp
