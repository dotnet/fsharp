// #Conformance #LexicalAnalysis
#light                                  // 2
                                        // 3
// Test __LINE__ directive              // 4
if __LINE__ <> "5" then ignore 1        // 5
                                        // 6
let f x =                               // 7
    let line = __LINE__                 // 8
    line                                // 9
                                        // 10
type Foo() =                            // 11
    override this.ToString() =          // 12
                               __LINE__ // 13
                                        // 14
if f 5 <> "8" then ignore 2             // 15
                                        // 16
let t = new Foo()                       // 17
if t.ToString() <> "13" then ignore 3   // 18
                                        // 19
ignore 0                                // 20
