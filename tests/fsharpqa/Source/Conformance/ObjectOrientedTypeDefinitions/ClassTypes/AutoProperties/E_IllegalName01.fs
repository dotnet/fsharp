// #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties
// <Expects status="error" id="FS1156" span="(4,16-4,18)">This is not a valid numeric literal\. Sample formats include 4, 0x4, 0b0100, 4L, 4UL, 4u, 4s, 4us, 4y, 4uy, 4\.0, 4\.0f, 4I\.</Expects>
type T() =  
    member val 1P = [] with get,set

exit 1
