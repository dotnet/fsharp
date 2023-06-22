// #Conformance #TypeConstraints 
#light

// Type with the null literal

// .Net string
let x1 : System.String = null

// .Net class
let x2 : System.Random = null

// .Net interface
let x3 : System.IComparable = null

// .Net delegate
let x4 : System.Action<int> = null

// F# Array - ok, it's derived from System.Array
let x5 : int array = null
