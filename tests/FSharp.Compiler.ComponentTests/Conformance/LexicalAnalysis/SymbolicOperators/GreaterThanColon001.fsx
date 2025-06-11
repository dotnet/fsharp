// #Regression #Conformance #LexicalAnalysis #Operators 
// Regression test for FSHARP1.0:1392
// Space should not be required between : and >
//<Expects status="success"></Expects>

#light 

type Ix = interface
    abstract A<'t> : 't->'t      // space between > and :
    end
