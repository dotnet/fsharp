module ORPTestRunner

open PriorityTests
open ExtensionPriorityTests

let mutable failures = 0

let test (name: string) (expected: string) (actual: string) =
    if actual <> expected then
        printfn "FAIL: %s - Expected '%s' but got '%s'" name expected actual
        failures <- failures + 1
    else
        printfn "PASS: %s" name

// ============================================================================
// Basic Priority Tests - consuming C# ORP from F#
// ============================================================================

let testBasicPriority () =
    test "Higher priority wins over lower" "priority-2" (BasicPriority.Invoke("test"))
    
    test "Negative priority deprioritizes" "current" (NegativePriority.Legacy("test"))
    
    test "Multiple negative priority levels" "new" (NegativePriority.Obsolete(42))
    
    test "Priority overrides concreteness" "generic-high-priority" (PriorityVsConcreteness.Process(42))
    
    test "Default priority is 0" "mixed-priority" (DefaultPriority.Mixed("test"))

// ============================================================================
// Per-declaring-type Extension Tests
// ============================================================================

let testExtensions () =
    test "Extension type B priority" "TypeB-priority2" (ExtensionTypeB.ExtMethod("hello", 42))
    
    let x = 42
    test "Per-type extension priority" "ModuleB-int-priority2" (x.Transform())

// ============================================================================
// Same Priority - Normal Tiebreakers Apply
// ============================================================================

let testSamePriorityTiebreakers () =
    test "Same priority - int wins by concreteness" "int" (SamePriorityTiebreaker.Process(42))
    
    test "Same priority - string wins by concreteness" "string" (SamePriorityTiebreaker.Process("hello"))
    
    test "Same priority - int[] wins by concreteness" "int-array" (SamePriorityArrayTypes.Handle([|1; 2; 3|]))

// ============================================================================
// Inheritance Tests
// ============================================================================

let testInheritance () =
    let derived = DerivedClassWithNewMethods()
    test "Derived new method highest priority" "DerivedNew-int-priority2" (derived.Method(42))
    
    let derivedBase = DerivedClass()
    test "Base priority respected in derived" "Derived-string" (derivedBase.Method("test"))

// ============================================================================
// Instance Method Priority
// ============================================================================

let testInstanceMethods () =
    let obj = InstanceOnlyClass()
    test "Instance method priority" "object-priority2" (obj.Call("hello"))
    
    let target = TargetClass()
    test "Extension adds new overload" "Extension-int-priority2" (target.DoWork(42))

// ============================================================================
// Explicit vs Implicit Zero Priority
// ============================================================================

let testExplicitVsImplicit () =
    test "No attr direct call" "no-attr" (ExplicitVsImplicitZero.WithoutAttr("test"))
    test "Explicit zero direct call" "explicit-zero" (ExplicitVsImplicitZero.WithExplicitZero(box "test"))

// ============================================================================
// Complex Generics
// ============================================================================

let testComplexGenerics () =
    test "Complex generics - fully generic wins" "fully-generic-priority2" (ComplexGenerics.Process(1, 2))
    
    test "Complex generics - partial match" "fully-generic-priority2" (ComplexGenerics.Process("hello", 42))

// ============================================================================
// F# Code USING the ORP attribute (defining overloads with ORP)
// ============================================================================

type FSharpWithORP =
    [<System.Runtime.CompilerServices.OverloadResolutionPriority(2)>]
    static member Greet(o: obj) = "fsharp-obj-priority2"
    
    [<System.Runtime.CompilerServices.OverloadResolutionPriority(0)>]
    static member Greet(s: string) = "fsharp-string-priority0"
    
    static member Greet(i: int) = "fsharp-int-default"

type FSharpGenericPriority =
    [<System.Runtime.CompilerServices.OverloadResolutionPriority(1)>]
    static member Process<'T>(x: 'T) = "fsharp-generic-priority1"
    
    [<System.Runtime.CompilerServices.OverloadResolutionPriority(0)>]
    static member Process(x: int) = "fsharp-int-priority0"

[<AutoOpen>]
module FSharpExtensions =
    type System.String with
        [<System.Runtime.CompilerServices.OverloadResolutionPriority(1)>]
        member this.FsExtend(x: obj) = "fsharp-ext-obj-priority1"
        
        [<System.Runtime.CompilerServices.OverloadResolutionPriority(0)>]
        member this.FsExtend(x: int) = "fsharp-ext-int-priority0"

let testFSharpUsingORP () =
    test "F# ORP - obj wins by priority" "fsharp-obj-priority2" (FSharpWithORP.Greet("hello"))
    
    test "F# ORP - generic wins by priority" "fsharp-generic-priority1" (FSharpGenericPriority.Process(42))
    
    test "F# extension ORP - obj wins by priority" "fsharp-ext-obj-priority1" ("test".FsExtend(42))

// ============================================================================
// Virtual Base ORPA Inheritance Tests
// ============================================================================

let testVirtualBaseOrpa () =
    // When called via Base type, priority1 object overload should win over priority0 string
    let baseObj = VirtualBaseWithPriority()
    test "Virtual base - object wins by priority" "base-object-priority1" (baseObj.Compute("hello"))

    // When called via Derived instance, base priority should still apply
    // Derived overrides don't change priority - it's read from base declaration
    let derived = DerivedOverridesVirtual()
    test "Derived virtual - base priority respected, object wins" "derived-object" (derived.Compute("hello"))

    // Int has priority -1, should lose to both object(1) and string(0)
    let intResult = derived.Compute(42)
    test "Derived virtual - int with neg priority" "derived-int" intResult

// ============================================================================
// Main entry point
// ============================================================================

[<EntryPoint>]
let main _ =
    printfn "Running OverloadResolutionPriority tests..."
    printfn ""
    
    printfn "=== Basic Priority Tests ==="
    testBasicPriority ()
    printfn ""
    
    printfn "=== Extension Tests ==="
    testExtensions ()
    printfn ""
    
    printfn "=== Same Priority Tiebreaker Tests ==="
    testSamePriorityTiebreakers ()
    printfn ""
    
    printfn "=== Inheritance Tests ==="
    testInheritance ()
    printfn ""
    
    printfn "=== Instance Method Tests ==="
    testInstanceMethods ()
    printfn ""
    
    printfn "=== Explicit vs Implicit Zero Tests ==="
    testExplicitVsImplicit ()
    printfn ""
    
    printfn "=== Complex Generics Tests ==="
    testComplexGenerics ()
    printfn ""
    
    printfn "=== F# Using ORP Attribute Tests ==="
    testFSharpUsingORP ()
    printfn ""
    
    printfn "=== Virtual Base ORPA Tests ==="
    testVirtualBaseOrpa ()
    printfn ""
    
    printfn "========================================"
    if failures = 0 then
        printfn "All tests passed!"
        0
    else
        printfn "FAILED: %d test(s) failed" failures
        1
