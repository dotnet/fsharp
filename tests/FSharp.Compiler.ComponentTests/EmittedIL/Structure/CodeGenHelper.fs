// #NoMT #CodeGen #Interop
#light

module CodeGenHelper

open System
open System.Reflection

// ----------------------------------------------------------------------------
// Basics
// ----------------------------------------------------------------------------

/// Syntactic sugar, doesn't do anything
let should f x = f x

/// Syntactic sugar, doesn't do anything
let verify f x = f x

// ----------------------------------------------------------------------------
// Getters
// ----------------------------------------------------------------------------

/// Gets the given type from the assembly (otherwise throws)
let getType typeName (asm : Assembly) =
    match asm.GetType(typeName, false) with
    | null -> 
        let allTypes = 
            asm.GetTypes()
            |> Array.map (fun ty -> ty.Name)
            |> Array.reduce (fun x y -> sprintf "%s\r%s" x y)
        failwithf "Error: Assembly did not contain type %s.\nAll types in asm:\n%s" typeName allTypes
    | ty -> ty

/// Gets a type's property
let getProperty propName (ty : Type) =
    match ty.GetProperty(propName) with
    | null     -> failwithf "Error: Type did not contain property %s" propName
    | propInfo -> propInfo

/// Gets a type's member with the given name (only search for the public members!)
let getMember memberName (ty : Type) =
    match ty.GetMember(memberName) with                                            // GetMember will never return null
    | [||] -> failwithf "Error: Type did not contain member %s" memberName         // - empty array (=not found)
    | [| memberInfo |] -> memberInfo                                               // - array with 1 element (=found)
    | _ -> failwithf "Error: Multiple overloads. Use getMembers instead."          // - array with >1 elements (=multiple overloads)
   
/// Gets all of a type's PUBLIC members 
let getMembers (ty : Type) = ty.GetMembers()

/// Gets a type's method 
let getMethod methodName (ty : Type) =
    match ty.GetMethod(methodName) with
    | null       -> failwithf "Error: Type did not contain member %s" methodName
    | methodInfo -> methodInfo

/// Gets all of a type's PUBLIC methods
let getMethods (ty : Type) = ty.GetMethods()

/// Gets a type's event with the given name
let getEvent eventName (ty : Type) =
    match ty.GetEvent(eventName) with
    | null  -> failwithf "Error: Type did not contain event %s" eventName
    | event -> event

/// Gets all of a type's events
let getEvents (ty : Type) = ty.GetEvents()

// ----------------------------------------------------------------------------
// Testers
// ----------------------------------------------------------------------------

// Type

/// Verify the assembly contains a type with the given name
let containType typeName (assembly : Assembly) =
    match assembly.GetType(typeName, false) with
    | null -> failwithf "Error: Assembly does not contain type %s" typeName
    | _    -> ()
    
/// Verify the assembly does NOT contains a type with the given name
let notContainType typeName (assembly : Assembly) =
    match assembly.GetType(typeName, false) with
    | null -> ()
    | _    -> failwithf "Error: Assembly does, in fact, contain type %s" typeName
 
// Property
 
/// Verify the type contains a property with the given name       
let containProp propName (ty : Type) =
    match ty.GetProperty(propName) with
    | null -> failwithf "Error: Type %s does not contain property %s" ty.Name propName
    | _    -> ()

/// Verify the type does not contain a property with the given name       
let notContainProp propName (ty : Type) =
    match ty.GetProperty(propName) with
    | null -> ()
    | _    -> failwithf "Error: Type %s does contain property %s" ty.Name propName
      
// Member
  
/// Verify the type contains a method with the given name
let containMember membName (ty : Type) =
    match ty.GetMember(membName) with
    | null -> failwithf "Error: Type %s does not contain member %s" ty.Name membName
    | _    -> ()

/// Verify the type does not contain a method with the given name
let notContainMember membName (ty : Type) =
    match ty.GetMember(membName) with
    | null | [| |] -> ()
    | _    -> failwithf "Error: Type %s does contain member %s" ty.Name membName

/// Has airity (number of arguments and types)
let takeParams (args : Type list) (methInfo : MethodInfo) =
    let parameters = methInfo.GetParameters()
    if parameters.Length <> List.length args then
        failwithf 
            "Error: Method [%s] doesn't have expected airity. Method takes %d params, but expected %d"
            methInfo.Name
            parameters.Length
            (List.length args)
            
    let verifyParamIsType (param : ParameterInfo) (ty : Type)=
        if param.ParameterType <> ty then
            failwithf 
                "Error: Parameter is type [%s] but expected type [%s]"
                param.ParameterType.Name
                ty.Name
                
    args |> List.iteri (fun i ty -> verifyParamIsType (parameters.[i]) ty)

/// Assert the type is a Multicast delegate with the given signature.
let beDelegate parameters (returnType : Type) (delegateType : Type) =
    // Verify the give type is, in fact, a delegate
    if delegateType.BaseType.Name <> "MulticastDelegate" then 
        failwithf 
            "Error: The given type [%s] is not a MulticastDelegate, but expected to be." 
            delegateType.Name
    
    // Now make sure it has the right signature, get the method info for the 'Invoke' method
    // to verify this.
    let invokeMethod = getMethod "Invoke" delegateType
    takeParams parameters invokeMethod
    
    // Check the return type too...
    if invokeMethod.ReturnType <> returnType then
        failwithf 
            "Error: Return types do not match. Expected [%s] but got [%s]"
            returnType.Name
            invokeMethod.ReturnType.Name
    ()
    
/// Verifies the given event uses the specified event handler
let useEventHandler eventHandlerType (eventInfo : EventInfo) =
    if eventInfo.EventHandlerType <> eventHandlerType then
        failwithf 
            "Error: Expected event to use handler [%s] but got [%s]"
            eventHandlerType.Name
            eventInfo.EventHandlerType.Name
    ()  
    
/// Verify the object contains a custom attribute with the given name. E.g. "ObsoleteAttribute"
let haveAttribute attrName thingey =
    let containsAttrWithName attrList =
        attrList
        |> Array.filter (fun att -> att.GetType().Name = attrName)
        |> Array.length > 0
    let containsAttribute =
        match box thingey with
        | :? Type as ty 
            -> ty.GetCustomAttributes(false)
               |> containsAttrWithName
        | :? MethodInfo as mi 
            -> mi.GetCustomAttributes(false)
               |> containsAttrWithName
        | :? PropertyInfo as pi
            -> pi.GetCustomAttributes(false)
               |> containsAttrWithName
        | :? EventInfo as ei
            -> ei.GetCustomAttributes(false)
               |> containsAttrWithName
        | _ -> failwith "Error: Unsuported primitive type, unable to get custom attributes."
    if not containsAttribute then
        failwithf "Error: Unable to locate attribute %s." attrName
    else
        ()

/// Asserts a propety's return value
let haveType ty (propInfo : PropertyInfo) =
    if propInfo.PropertyType <> ty then
        failwithf 
            "Error: Property has type [%s] but expected[%s]"
            propInfo.PropertyType.Name
            ty.Name
    ()

/// Verify the object does not contain any custom attributes.
let doesn'tHaveAttribute attrName thingey =
    let doesn'tContainsAttrWithName attrList =
        attrList
        |> Array.filter (fun att -> att.GetType().Name = attrName)
        |> Array.length = 0
    let containsAttribute =
        match box thingey with
        | :? Type as ty 
            -> ty.GetCustomAttributes(false)
               |> doesn'tContainsAttrWithName
        | :? MethodInfo as mi 
            -> mi.GetCustomAttributes(false)
               |> doesn'tContainsAttrWithName
        | :? PropertyInfo as pi
            -> pi.GetCustomAttributes(false)
               |> doesn'tContainsAttrWithName
        | :? EventInfo as ei
            -> ei.GetCustomAttributes(false)
               |> doesn'tContainsAttrWithName
        | _ -> failwith "Error: Unsuported primitive type, unable to get custom attributes."
    if containsAttribute then
        failwithf "Error: Able to locate attribute %s, didn't expect to find." attrName
    else
        ()
