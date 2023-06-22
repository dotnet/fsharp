module Foo

open System
open System.Threading.Tasks

let mapWithAdditionalDependenies
    (mapping: 'a -> 'b * #seq<#IDisposable>) = 0
