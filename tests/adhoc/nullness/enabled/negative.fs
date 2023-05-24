open System

module Basics2 = 
    let f6 () : 'T __withnull when 'T : not struct and 'T : null = null // Expected to give an error about inconistent constraints

