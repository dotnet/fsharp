---
title: Representations
category: Compiler Internals
categoryindex: 200
index: 350
---
# Representation Decisions in the F# Compiler

Consider the following declarations, all of which look very similar.

```fsharp
module M = 
    let z = 1                   
    let f = x + z               
                                

type C(w: int, z: int) =        
                                
    let f x = x + z             
    let f x = f 3 + x           
                                

let g (z: int) =                
    let f x = x + 1             
```
Part of the job of the F# compiler is to "decide" how these declarations are compiled. The following acts as a guide to how these different bindings are represented and where these decisions are made.

First for module-level `let` bindings. These representations are decided by code in `CheckExpressions.fs` and `CheckDeclarations.fs` based on syntax. 

```fsharp
module M = 
    let z = 1              // z --> static property + field, required by spec, compiled name mandated
    let f x = x + z          // f --> static method, required by spec, compiled name mandated
```

Next for class-level `let` bindings.  These representations are decided by code in `CheckIncrementalClasses.fs` based on analysis of use. 
```fsharp
//    Decided in CheckIncrementalClasses.fs based on analysis of use
type C(w: int, z: int) =   // w --> local to object constructor, required by spec
                           // z --> private instance field, required by spec
    let f x = x + z        // f --> private instance method, required by spec, compiled name not mandated
                           // Note: initially uses an ephemeral 'f' Val then creates a member Val with compiled name
                           
    let f x = f 3 + x      // f --> private instance method, required by spec, compiled name not mandated
                           // Note: initially uses an ephemeral 'f' Val then creates a member Val with compiled name
                           
    static let g x = x + 1 // g --> private static method, required by spec, compiled name not mandated, initially uses an ephemeral 'g' Val then creates a member Val with compiled name
    
    static let g x = g 3   // g --> private static method, required by spec, compiled name not mandated, initially uses an ephemeral 'g' Val then creates a member Val with compiled name
```
Next for expression-level `let` bindings.  These representations are decided by code in various optimization phases.
```fsharp
let g (z: int) =          // z --> local + field in closure for 'f', not mandated
    let f x = x + 1       // f --> FSharpFunc value, or maybe a static method, not mandated 
                          //    Decided in various optimization phases
```

> NOTE: The representation decision is implied by the addition of ValReprInfo to the `Val` node.
