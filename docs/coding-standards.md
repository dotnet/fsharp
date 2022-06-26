---
title: Coding standards
category: Compiler Internals
categoryindex: 200
index: 200
---
# Coding standards and idioms

The F# compiler code base is slowly being updated to better coding standards. There is a long way to go.

The future work includes

* [ ] Consistent use of fantomas formatting across as much of the codebase as feasible
* [ ] Consistent naming conventions
* [ ] Reduction in line length
* [ ] Reduction in single-character identifiers
* [ ] XML documentation for all types, members and cross-module functions

## Abbreviations

The compiler codebase uses various abbreviations. Here are some of the most common ones.

| Abbreviation        | Meaning                                                                                                                                                                                                                                         |  
|:--------------------|:------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `ad`                | Accessor domain, meaning the permissions the accessing code has to access other constructs                                                                                                                                                      |
| `amap`              | Assembly map, saying how to map IL references to F# CCUs                                                                                                                                                                                        |
| `arg`               | Argument (parameter)                                                                                                                                                                                                                            |
| `argty`             | Argument (parameter) type                                                                                                                                                                                                                       |
| `arginfo`           | Argument (parameter) metadata                                                                                                                                                                                                                   |
| `ccu`               | Reference to an F# compilation unit = an F# DLL (possibly including the DLL being compiled)                                                                                                                                                     |
| `celem`             | Custom attribute element                                                                                                                                                                                                                        |
| `cenv`              | Compilation environment. Means different things in different contexts, but usually a parameter for a single compilation state object being passed through a set of related functions in a single phase. The compilation state is often mutable. |
| `cpath`             | Compilation path, meaning A.B.C for the overall names containing a type or module definition                                                                                                                                                    |
| `css`               | Constraint solver state.                                                                                                                                                                                                                        |
| `denv`              | Display Environment. Parameters guiding the formatting of types                                                                                                                                                                                 |
| `einfo`             | An info object for an event  (whether a .NET event, an F# event or a provided event)                                                                                                                                                            |
| `e`                 | Expression                                                                                                                                                                                                                                      |
| `env`               | Environment. Means different things in different contexts, but usually immutable state being passed and adjusted  through a set of related functions in a single phase.                                                                         |
| `finfo`             | An info object for a field (whether a .NET field or a provided field)                                                                                                                                                                           |
| `fref`              | A reference to an ILFieldRef Abstract IL node for a field reference. Would normally be modernized to `ilFieldRef`                                                                                                                               |
| `g`                 | The TcGlobals value                                                                                                                                                                                                                             |
| `id`                | Identifier                                                                                                                                                                                                                                      |
| `lid`               | Long Identifier                                                                                                                                                                                                                                 |
| `m`                 | A source code range marker                                                                                                                                                                                                                      |
| `mimpl`             | IL interface method implementation                                                                                                                                                                                                              |
| `minfo`             | An info object for a method (whether a .NET method, an F# method or a provided method)                                                                                                                                                          |
| `modul`             | a Typed Tree structure for a namespace or F# module                                                                                                                                                                                             |
| `pat`               | Pattern, a syntactic AST node representing part of a pattern in a pattern match                                                                                                                                                                 |
| `pinfo`             | An info object for a property  (whether a .NET property, an F# property or a provided property)                                                                                                                                                 |
| `rfref`             | Record or class field  reference, a reference to a Typed Tree node for a record or class field                                                                                                                                                  |
| `scoref`            | The scope of a reference in IL metadata, either assembly, `.netmodule` or local                                                                                                                                                                 |
| `sp`                | Sequence points or debug points                                                                                                                                                                                                                 |
| `spat`              | Simple Pattern, a syntactic AST node representing part of a pattern in a pattern match                                                                                                                                                          |
| `tau`               | A type with the "forall" nodes stripped off (i.e. the nodes which represent generic type parameters). Comes from the notation _𝛕_ used in type theory                                                                                          |
| `tcref`             | Type constructor  reference (an `EntityRef`)                                                                                                                                                                                                    |
| `tinst`             | Type instantiation                                                                                                                                                                                                                              |
| `tpenv`             | Type parameter environment, tracks the type parameters in scope during type checking                                                                                                                                                            |
| `ty` (not: `typ`)   | Type, usually a Typed Tree type                                                                                                                                                                                                                 |
| `tys` (not: `typs`) | List of types, usually Typed Tree types                                                                                                                                                                                                         |
| `typar`             | Type Parameter                                                                                                                                                                                                                                  |
| `tyvar`             | Type Variable, usually referring to an IL type variable, the compiled form of an F# type parameter                                                                                                                                              |
| `ucref`             | Union case reference, a reference to a Typed Tree node for a union case                                                                                                                                                                         |
| `vref`              | Value reference, a reference to a Typed Tree node for a value                                                                                                                                                                                   |

| Phase Abbreviation             |   Meaning  |  
|:------------------------------|:-----------|
| `Syn`                  | Abstract Syntax Tree |
| `Tc`                  | Type-checker |
| `IL`                 | Abstract  IL = F# representation of .NET IL |
| `Ilx`                 | Extended Abstract IL = .NET IL plus a couple of contructs that get erased |
