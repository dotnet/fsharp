// #Regression #const #TypeProvider #StaticParams
// Trying to use 'const' on an expression with the + requires using parens (see DevDiv:161603)
//<Expects status="warning" span="(5,14-5,26)" id="FS1190">Type arguments must be placed directly adjacent to the type name, e\.g\. "C<'T>", not "C  <'T>"$</Expects>

type f = N1.T< const +1 >
