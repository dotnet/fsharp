// #Conformance #TypesAndModules #Unions 
// By default, Union types implement dispatch slot GetHashCode
// We also minimally verify that the hash codes are different for this simple case
//<Expects status="success"></Expects>
#light

type T1 = | A
          | B 

type T2 = | A
          | C 

if (T1.A.GetHashCode() <> T1.B.GetHashCode()) &&
    (T2.A.GetHashCode() <> T2.C.GetHashCode()) then 0 else failwith "Failed: 1"
