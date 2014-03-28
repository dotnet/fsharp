// #Regression #const #TypeProviders #StaticParams
// Trying to use 'const' on an expression with the + requires using parens (see DevDiv:161603)
//<Expects status="success"></Expects>

type f = N1.T<const (+1)>
