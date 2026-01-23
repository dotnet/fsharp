// F# test runner for OverloadResolutionPriority tests
// This file contains all assertions and is run ONCE as an executable

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
    // Higher priority (2) should win over more specific overload (priority 1)
    test "Higher priority wins over lower" "priority-2" (BasicPriority.Invoke("test"))
    
    // Negative priority -1 loses to default priority 0
    test "Negative priority deprioritizes" "current" (NegativePriority.Legacy("test"))
    
    // Multiple negative levels: int(0) beats string(-1) and object(-2)
    test "Multiple negative priority levels" "new" (NegativePriority.Obsolete(42))
    
    // Higher priority generic beats more concrete int with lower priority
    test "Priority overrides concreteness" "generic-high-priority" (PriorityVsConcreteness.Process(42))
    
    // Priority 1 (object) beats default priority 0 (string)
    test "Default priority is 0" "mixed-priority" (DefaultPriority.Mixed("test"))

// ============================================================================
// Per-declaring-type Extension Tests
// ============================================================================

let testExtensions () =
    // Direct call to TypeB - should pick priority 2 overload
    test "Extension type B priority" "TypeB-priority2" (ExtensionTypeB.ExtMethod("hello", 42))
    
    // Per-type filtering: ModuleA offers generic@1, ModuleB offers int@2
    // Between different types, concreteness applies: int beats generic
    let x = 42
    test "Per-type extension priority" "ModuleB-int-priority2" (x.Transform())

// ============================================================================
// Same Priority - Normal Tiebreakers Apply
// ============================================================================

let testSamePriorityTiebreakers () =
    // All have priority 1, int is more concrete
    test "Same priority - int wins by concreteness" "int" (SamePriorityTiebreaker.Process(42))
    
    // All have priority 1, string is more concrete
    test "Same priority - string wins by concreteness" "string" (SamePriorityTiebreaker.Process("hello"))
    
    // Both have priority 1, int[] is more concrete than T[]
    test "Same priority - int[] wins by concreteness" "int-array" (SamePriorityArrayTypes.Handle([|1; 2; 3|]))

// ============================================================================
// Inheritance Tests
// ============================================================================

let testInheritance () =
    // DerivedClassWithNewMethods: int@2 has highest priority
    let derived = DerivedClassWithNewMethods()
    test "Derived new method highest priority" "DerivedNew-int-priority2" (derived.Method(42))
    
    // DerivedClass inherits priorities: string@1 beats object@0
    let derivedBase = DerivedClass()
    test "Base priority respected in derived" "Derived-string" (derivedBase.Method("test"))

// ============================================================================
// Instance Method Priority
// ============================================================================

let testInstanceMethods () =
    // InstanceOnlyClass: object@2 has higher priority than string@0
    let obj = InstanceOnlyClass()
    test "Instance method priority" "object-priority2" (obj.Call("hello"))
    
    // Extension int@2 is the matching overload for int
    let target = TargetClass()
    test "Extension adds new overload" "Extension-int-priority2" (target.DoWork(42))

// ============================================================================
// Explicit vs Implicit Zero Priority
// ============================================================================

let testExplicitVsImplicit () =
    // Direct call works
    test "No attr direct call" "no-attr" (ExplicitVsImplicitZero.WithoutAttr("test"))
    test "Explicit zero direct call" "explicit-zero" (ExplicitVsImplicitZero.WithExplicitZero(box "test"))

// ============================================================================
// Complex Generics
// ============================================================================

let testComplexGenerics () =
    // All match for (int, int), fully-generic@2 wins
    test "Complex generics - fully generic wins" "fully-generic-priority2" (ComplexGenerics.Process(1, 2))
    
    // For (string, int): fully-generic@2 and partial@1 match, priority 2 wins
    test "Complex generics - partial match" "fully-generic-priority2" (ComplexGenerics.Process("hello", 42))

// ============================================================================
// F# Code USING the ORP attribute (defining overloads with ORP)
// ============================================================================

// F# can define methods with ORP attribute
type FSharpWithORP =
    [<System.Runtime.CompilerServices.OverloadResolutionPriority(2)>]
    static member Greet(o: obj) = "fsharp-obj-priority2"
    
    [<System.Runtime.CompilerServices.OverloadResolutionPriority(0)>]
    static member Greet(s: string) = "fsharp-string-priority0"
    
    // No attr - default priority 0
    static member Greet(i: int) = "fsharp-int-default"

// F# class where priority makes generic win over concrete
type FSharpGenericPriority =
    [<System.Runtime.CompilerServices.OverloadResolutionPriority(1)>]
    static member Process<'T>(x: 'T) = "fsharp-generic-priority1"
    
    [<System.Runtime.CompilerServices.OverloadResolutionPriority(0)>]
    static member Process(x: int) = "fsharp-int-priority0"

// F# extension methods with ORP
[<AutoOpen>]
module FSharpExtensions =
    type System.String with
        [<System.Runtime.CompilerServices.OverloadResolutionPriority(1)>]
        member this.FsExtend(x: obj) = "fsharp-ext-obj-priority1"
        
        [<System.Runtime.CompilerServices.OverloadResolutionPriority(0)>]
        member this.FsExtend(x: int) = "fsharp-ext-int-priority0"

let testFSharpUsingORP () =
    // F# method with ORP: obj@2 beats string@0
    test "F# ORP - obj wins by priority" "fsharp-obj-priority2" (FSharpWithORP.Greet("hello"))
    
    // F# method with ORP: generic@1 beats int@0
    test "F# ORP - generic wins by priority" "fsharp-generic-priority1" (FSharpGenericPriority.Process(42))
    
    // F# extension with ORP: obj@1 beats int@0
    test "F# extension ORP - obj wins by priority" "fsharp-ext-obj-priority1" ("test".FsExtend(42))

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
    
    printfn "========================================"
    if failures = 0 then
        printfn "All tests passed!"
        0
    else
        printfn "FAILED: %d test(s) failed" failures
        1
