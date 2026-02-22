// #Regression #Conformance #DeclarationElements #ObjectConstructors 

//<Expects status="error" span="(6,5)" id="FS0963">This definition may only be used in a type with a primary constructor\. Consider adding arguments to your type definition, e\.g\. 'type X\(args\) = \.\.\.'\.$</Expects>

type NoObjCtor =
    let m_value = 42
    override this.ToString() = "ToString"
    
exit 1
