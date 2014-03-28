// #Regression #const #TypeProviders #StaticParams
// Trying to use a comma in a static argument string (see DevDiv:274395)
//<Expects status="success"></Expects>

type f = N1.P< "a,b">
