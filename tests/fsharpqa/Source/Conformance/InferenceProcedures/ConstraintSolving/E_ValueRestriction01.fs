// #Regression #Conformance #TypeInference #TypeConstraints 
#light

// Verify error associated with open type variable

//<Expects id="FS0030" status="error">Value restriction: The value 'x' has an inferred generic type\n    val x: '_a list ref\nHowever, values cannot have generic type variables like '_a in "let x: '_a"\. You can do one of the following:\n- Define it as a simple data term like a string literal or a union case\n- Add an explicit type annotation\n- Use the value as a non-generic type in later code for type inference,\nor if you still want type-dependent results, you can define 'x' as a function instead by doing either:\n- Add a unit parameter\n- Write explicit type parameters like "let x<'a>"\.\nThis error is because a let binding without parameters defines a value, not a function\. Values cannot be generic because reading a value is assumed to result in the same everywhere but generic type parameters may invalidate this assumption by enabling type-dependent results\.$</Expects>
let x = ref []

exit 1
