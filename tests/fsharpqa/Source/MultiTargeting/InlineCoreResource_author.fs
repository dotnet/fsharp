module Test

let inline init1 n f = Array.init n f
let init2 n f = Array.init n f

let inline reduce1 f = Array.reduce f
let reduce2 f = Array.reduce f