// #Regression #Conformance #ObjectOrientedTypes #Classes 
// Regression test for FSB 3661, 'Properties, obj and de-condensation have awkward intersection'

type Foo() =
    let mutable data : obj = null
    
    member this.Data1 = data
    
    member this.Data2 
        with get () = data
        and set s = data <- s

// ----------------------------
        
type Bar(f : obj -> string) = 
    member this.F = f
    
// ----------------------------

type INodeMeta = obj 
type IScenarioMeta = obj 
type NodeIndex = int 
type NodeValue = obj 
type NodeType = 
    | DataNode 
    | FunctionNode 
    | ScenarioNode 

type ScenarioPerturbFunc = IScenarioMeta -> NodeIndex -> INodeMeta -> NodeValue -> (IScenarioMeta * NodeValue option) 

type Scenario ( scmeta: IScenarioMeta, name: string, sfunc: ScenarioPerturbFunc ) = 
    class 
        let mutable scmeta = scmeta 

        member this.Name = name 
        member this.PerturbF = sfunc 
        member this.ScMeta 
            with get () = scmeta 
            and set s = scmeta <- s 

        member this.xxPerturbF:ScenarioPerturbFunc = sfunc 
        member this.xxScMeta 
            with get ():IScenarioMeta = scmeta 
            and set (s:IScenarioMeta) = scmeta <- s 
    end 

// --------------------------------    

// Bug was about none of these code snippets compiling
// (Converting obj to 'a, and then getting a value restriction)
exit 0
