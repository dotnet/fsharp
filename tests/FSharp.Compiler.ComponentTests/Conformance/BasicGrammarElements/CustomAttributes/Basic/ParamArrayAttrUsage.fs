// #Conformance #DeclarationElements #Attributes 
#light
//Regression test for FSHARP bug 102 Import uses of ParamArray attribute

open System

String.Format("{0},{1},{2}","foo","bar","baz")
String.Format("{0},{1},{2},{3}","foo","bar","baz","womble")
System.String.Format("{0},{1},{2},{3}",[|box "foo";box "bar";box "baz";box "womble"|])
