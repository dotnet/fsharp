// #Regression #Diagnostics 
// To ease up the transition path to the new F#, we (used to) emit diagnostic that is
// useful to the user to figure out what to do to upgrade.
//<Expects id="FS0039" span="(7,10-7,16)" status="error">The type 'Matrix' is not defined</Expects>


type t = Matrix<int>
