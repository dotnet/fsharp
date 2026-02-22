// #Regression #Misc
// Regression test for FSHARP1.0:5936 
// This test ensures that you can't apply the CompiledName attribute more than once to a property
//<Expects status="error" span="(29,16-29,39)" id="FS0429">The attribute type 'CompiledNameAttribute' has 'AllowMultiple=false'\. Multiple instances of this attribute cannot be attached to a single language element\.$</Expects>
//<Expects status="error" span="(30,15-30,38)" id="FS0429">The attribute type 'CompiledNameAttribute' has 'AllowMultiple=false'\. Multiple instances of this attribute cannot be attached to a single language element\.$</Expects>

module M
type T() = 
    let mutable bval = "Boo!"
    
    [<CompiledName "P">]
    static member p = 1
    
    [<CompiledName "M">]
    static member m() = 1
    
    [<CompiledName "IP">]
    member this.ip = 1
    
    [<CompiledName "IM">]
    member this.im() = 1
    
    // The additional applications of CompiledName to the getters and setters won't work
    // since CompiledName has already been applied to the property
    // CompiledName can be applied to the getter, setter or property - it will rename the
    // property itself - you can only use it once
    [<CompiledName "B">]
    member this.b 
        with [<CompiledName "WrongGet">] get() = bval
        and [<CompiledName "WrongSet">] set(v) = bval <- v
