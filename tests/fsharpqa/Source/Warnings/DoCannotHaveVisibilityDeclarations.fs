// #Warnings
//<Expects status="Error" id="FS0531">Accessibility</Expects>
//<Expects status="Error" id="FS0512">Accessibility modifiers are not permitted on 'do' bindings, but 'Private' was given.</Expects>

type X() =
    do ()
    private do ()
    static member Y() = 1 
  
    
exit 0