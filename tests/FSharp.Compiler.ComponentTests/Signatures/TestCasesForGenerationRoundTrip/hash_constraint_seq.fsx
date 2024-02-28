module Foo

open System
open System.Threading.Tasks

let mapWithAdditionalDependencies
    (mapping: 'a -> 'b * #seq<#IDisposable>) = 0
