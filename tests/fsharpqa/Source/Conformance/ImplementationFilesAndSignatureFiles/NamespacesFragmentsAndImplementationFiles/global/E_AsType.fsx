// #Regression #Conformance #SignatureFiles #Namespaces 
// Regression test for FSHARP1.0:4937, FSHARP1.0:5295
// Usage of 'global' - global is a keyword
//<Expects status="error" id="FS0883" span="(13,6-13,12)">Invalid namespace, module, type or union case name</Expects>



// OK
type ``global``() = class 
                    end

// Err
type global() = class 
                end

