// #Regression #Conformance #TypeInference 
// FSB 1625, fsc generates assemblies that don't load/peverify
//<Expects span="(19,18-19,19)" status="error" id="FS0361">The override 'M: int -> int' implements more than one abstract slot, e\.g\. 'abstract IB\.M: int -> int' and 'abstract IA\.M: int -> int'</Expects>

type IA = 
    abstract M : int -> int 

type IB = 
    inherit IA
    abstract M : int -> int 

type C() = 

    // By removing this, the dispatch slot IA_M is not filled
    //interface IA with 
     //   member x.M(y) = y+3
     
    interface IB with 
        member x.M(y) = y+3
