// #Regression #Conformance #TypesAndModules #Modules 
// Productions
// Compile without: --mlcompatibility
//<Expects id="FS0062" span="(14,13-14,19)" status="warning">This construct is for ML compatibility\. The syntax 'module \.\.\. = struct \.\. end' is not used in F# code\. Consider using 'module \.\.\. = begin \.\. end'</Expects>
//<Expects id="FS0062" span="(18,13-18,19)" status="warning">This construct is for ML compatibility\. The syntax 'module \.\.\. = struct \.\. end' is not used in F# code\. Consider using 'module \.\.\. = begin \.\. end'</Expects>
//<Expects id="FS0062" span="(22,13-22,19)" status="warning">This construct is for ML compatibility\. The syntax 'module \.\.\. = struct \.\. end' is not used in F# code\. Consider using 'module \.\.\. = begin \.\. end'</Expects>
//<Expects id="FS0062" span="(26,13-26,19)" status="warning">This construct is for ML compatibility\. The syntax 'module \.\.\. = struct \.\. end' is not used in F# code\. Consider using 'module \.\.\. = begin \.\. end'</Expects>
//<Expects id="FS0062" span="(30,13-30,19)" status="warning">This construct is for ML compatibility\. The syntax 'module \.\.\. = struct \.\. end' is not used in F# code\. Consider using 'module \.\.\. = begin \.\. end'</Expects>
//<Expects id="FS0062" span="(35,13-35,19)" status="warning">This construct is for ML compatibility\. The syntax 'module \.\.\. = struct \.\. end' is not used in F# code\. Consider using 'module \.\.\. = begin \.\. end'</Expects>
//<Expects id="FS0062" span="(39,13-39,19)" status="warning">This construct is for ML compatibility\. The syntax 'module \.\.\. = struct \.\. end' is not used in F# code\. Consider using 'module \.\.\. = begin \.\. end'</Expects>

#light

module N1 = struct
                let f x = x + 1
            end

module N2 = struct
                ()
            end

module N3 = struct
                type T = | A = 1
            end

module N4 = struct
                exception E of string
            end

module N5 = struct
                module M5 = begin
                            end
            end

module N6 = struct 
                module M6 = Microsoft.FSharp.Collections.Array
            end
           
module N7 = struct
                open Microsoft.FSharp.Control
            end
