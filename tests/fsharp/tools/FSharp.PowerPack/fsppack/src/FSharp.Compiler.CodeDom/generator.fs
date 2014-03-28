namespace Microsoft.Test.Compiler.CodeDom.Internal
#nowarn "57" // parametrized active patterns
#nowarn "62" // This construct is for ML compatibility.

open System
open System.IO
open System.Text
open System.Reflection
open System.Collections
open System.Collections.Generic
open System.CodeDom
open System.CodeDom.Compiler

open Microsoft.Test.Compiler.CodeDom.Internal.Visitor


module internal Generator =

    type ResizeArray<'T> = System.Collections.Generic.List<'T> // alias
  
    //---------------------------------------------------------------------------------------------
    // Context and configuration
    
    type AdditionalOptions =
      /// No extra configuration
      | None = 0                      
      
      /// Reference inherited fields using "fld" instead of "this.fld" 
      /// (could be used in the future to allow implicit classes in ASP.NET?)
      | UnknonwFieldsAsLocals = 1     
                                      
      /// Hacking for ASP.NET incorrect array initializers 
      /// They generate "string" where codedom test suite uses "string[]"
      | AspNetArrays = 2              
                                      
    
    type Context = 
      {
        /// Some unique ID for every namespace (so we don't have name clashes)
        UniqueID:string
        
        /// Options, output, ...
        Options:AdditionalOptions
        Writer:IndentedTextWriter
        
        // *** Method/type scope ***
        
        /// Names of all type arguments in scope (need to rename T -> 'T etc.)
        TypeArgumentNames:Set<string>
        /// Types of all local variables in the method
        LocalVariableTypes:Map<string,Type>;
        /// Type of the method 
        CurrentMethodReturnType:CodeTypeReference option;
        /// We use exception for returning value when generating complex
        /// code that returns using imperative "return" statement
        ReturnUsingException:bool;
      
        // *** Information for the current class *** 
        
        CurrentType:CodeTypeDeclaration;
        BaseTypes:CodeTypeReference option * CodeTypeReference list
        AllFields:Map<string,CodeMemberField>;
        AllProps:Map<string,CodeMemberProperty>;
        AllMeths:Map<string,CodeMemberMethod>;
        AllEvents:Map<string,CodeMemberEvent>;
        FieldTypes:Map<string,CodeTypeReference>;
        PropertyTypes:Map<string,CodeTypeReference>;
        DeclaredEvents:CodeMemberEvent list;
          
        // *** Namespace scope ***         
        
        // Renamed types (when flattening nested classes)
        TypeRenames:Map<string,string>
        // Current namespace (can't be used in the type reference expression)
        CurrentNamespace:string; 
        // Set of interface names declared in the current namespace
        DeclaredInterfaces:Set<string>
        // A static Main method declared by one of the classes in this namespace
        MainMethodForCurrentNamespace:(CodeEntryPointMethod * CodeTypeDeclaration) option
      }
    
    /// Create context using specified text writer and options
    let createContext (wr:TextWriter) (opts:CodeGeneratorOptions) (addopts) = 
      { UniqueID = (Guid.NewGuid()).ToString("N")
        Writer = new IndentedTextWriter(wr); TypeRenames = Map.empty; 
        CurrentType = null; CurrentNamespace = ""; 
        DeclaredEvents = []; 
        BaseTypes = (None, []); 
        AllFields = Map.empty;
        AllEvents = Map.empty;
        AllProps = Map.empty;
        AllMeths = Map.empty;
        FieldTypes = Map.empty 
        CurrentMethodReturnType = None; 
        LocalVariableTypes = Map.empty; 
        ReturnUsingException = false; 
        PropertyTypes = Map.empty
        Options = addopts; 
        DeclaredInterfaces = Set.empty; 
        TypeArgumentNames = Set.empty; 
        MainMethodForCurrentNamespace = None }    

    /// Where are we generating member?
    type MemberGenerateType = 
      | InsideInterface = 0
      | InsideStruct = 1
      | InsideClass = 2
      
    //---------------------------------------------------------------------------------------------
    // Collections and combinators for generating

    /// Function composition operator
    let (+>) (ctx:Context -> Context) (foo:Context -> Context) x =
      foo (ctx x);

    /// Print unique id using: "+> uniqid"
    let uniqid (c:Context) =
      c.Writer.Write(c.UniqueID);
      c;

    /// Break-line and append specified string
    let (++) (ctx:Context -> Context) (str:String) x =
      let c = (ctx x)
      c.Writer.WriteLine();
      c.Writer.Write(str);
      c;

    /// Append specified string without line-break
    let (--) (ctx:Context -> Context) (str:String) x =
      let c = (ctx x)
      c.Writer.Write(str);
      c;

    /// Call function, but give it context as an argument      
    let withCtxt f x =
      (f x) x;
      
    /// Identity function
    let id a = a

    /// Print object converted to string
    let str (o: 'T) (ctx:Context) =
      ctx.Writer.Write(o :> obj);
      ctx;

    /// Create closure to do the counting 
    /// (this is usend when we need indexing during collection processing)
    let createCounter() =   
      let i = ref (-1)
      (fun () -> i := (!i) + 1; !i)
      
    /// Perform map and filter operations in one 
    let rec mapFilter f l =
      match l with
      | [] -> [];
      | a::r -> match (f a) with | None -> (mapFilter f r) | Some el -> el::(mapFilter f r)

    /// Process collection - keeps context through the whole processing
    /// calls 'f' for every element in sequence and 'fs' between every two elements 
    /// as a separator
    let col fs (c:IEnumerable) f (ctx:Context) = 
      let mutable tryPick = true in
      let mutable st = ctx
      let e = c.GetEnumerator()
      while (e.MoveNext()) do
        if (tryPick) then tryPick <- false else st <- fs st
        st <- f (unbox e.Current) st
      st
    
    /// Process collection - keeps context through the whole processing
    /// calls 'f' for every element in sequence and 'fs' between every two elements 
    /// as a separator. This is a variant that works on typed collections.
    let colT fs (c:seq<'T>) f (ctx:Context) =
      let mutable tryPick = true in
      let mutable st = ctx
      let e = c.GetEnumerator();
      while (e.MoveNext()) do
        if (tryPick) then tryPick <- false else st <- fs st;
        st <- f (e.Current) st;
      st
        
    /// Call specified function only on elements of specified type.
    /// (performs dynamic type test using x.GetType())
    let colFilterT<'T> fs (c:IEnumerable) (f: 'T -> Context -> Context) ctx =
      let sq : seq<'T>
          = c |> Seq.cast |> Seq.filter (fun (o:obj) -> o.GetType() = typeof<'T>) |> Seq.cast
      colT fs sq f ctx

    let colFilter<'T> fs (c:IEnumerable) (f: 'T -> Context -> Context) ctx =
      let sq = c |> Seq.cast |> Seq.filter (fun (o:obj) -> o.GetType() = typeof<'T>)
      col fs sq f ctx

    // Separator functions        
    let sepDot          = id -- "."
    let sepWordAnd      = id -- " and "      
    let sepSpace        = id -- " "      
    let sepNln          = id ++ ""
    let sepArgs         = id -- ", "
    let sepArgsSemi     = id -- "; "
    let sepNone         = id
    let sepStar         = id -- " * "
    let sepNlnSemiSpace = id -- ";" ++ "  "
    
    //---------------------------------------------------------------------------------------------
    // F# keywords and identifiers and also type resolving for standard .NET libraries
    
    let fsKeyWords = 
      new HashSet<_>
         (["abstract"; "and"; "as"; "assert"; "asr"; "base"; "begin"; "class"; "default"; "delegate"; "do"; "done";
           "downcast"; "downto"; "elif"; "else"; "end"; "exception"; "extern"; "false"; "finally"; "for"; "fun";
           "function"; "if"; "in"; "inherit"; "inline"; "interface"; "internal"; "land"; "lazy"; "let"; "lor"; "lsl"; "lsr"; "lxor";
           "match"; "member"; "method"; "mod"; "module"; "mutable"; "namespace"; "new"; "null"; "of"; "open"; "or"; "override";
           "private"; "public"; "rec"; "return"; "sig"; "static"; "struct"; "then"; "to"; "true"; "try"; "type"; "upcast"; "use"; "val"; "virtual"; "void"; "when"; 
           "while"; "with"; "yield";
           
           "atomic"; "break"; 
           "checked"; "component"; "const"; "constraint"; "constructor"; "continue"; 
           "eager"; 
           "fixed"; "fori"; "functor"; "global";"recursive";"measure"; 
           "include";  (* "instance"; *)
           "mixin"; 
           "object"; "parallel"; "params";  "process"; "protected"; "pure"; (* "pattern"; *)
           "sealed"; "trait";  "tailcall";
           "volatile"; ], HashIdentity.Structural)

    let isValidIdentifier str = 
      not (fsKeyWords.Contains(str))

    let makeEscapedIdentifier str = 
      if (fsKeyWords.Contains(str)) then "i'"+str+"'" else str;

    let makeValidIdentifier str = 
      if (fsKeyWords.Contains(str)) then "_"+str else str;
      
    let freshName = 
      let counter = createCounter ()
      (fun () -> "UnnamedMethod_" + counter().ToString())

    // List of "known" libraries that we try to search when we need to resolve a type
    let coreAssemblies = 
       ["mscorlib"; "System"; "System.Web"; "System.Xml"; 
        "System.Data"; "System.Deployment"; "System.Design"; "System.DirectoryServices"; 
        "System.Drawing.Design"; "System.Drawing"; "System.EnterpriseServices"; 
        "System.Management"; "System.Messaging"; "System.Runtime.Remoting"; 
        "System.Security"; "System.ServiceProcess"; "System.Transactions"; 
        "System.Configuration"; "System.Web.Mobile"; "System.Web.RegularExpressions"; 
        "System.Web.Services"; "System.Windows.Forms"; "System.Core";
        "PresentationCore"; "PresentationFramework"; "WindowsBase"; "WindowsFormsIntegration"]
      |> List.map ( fun n -> lazy(try Some(System.Reflection.Assembly.LoadWithPartialName(n)) with _ -> None); );

    let dict = new Dictionary<string, Type>();

    /// Tries to find .NET type for specified type name
    /// This is used when we need to know type in order to generate something correctly,
    /// but it's just a fallback case
    let (|FoundSystemType|_|) s =
      if (dict.ContainsKey(s)) then Some dict.[s] else
      let ty = coreAssemblies |> Seq.tryPick ( fun lazyAsm -> 
        match lazyAsm.Force() with 
        | None -> None
        | Some asm -> 
            match (try asm.GetType(s) with _ -> null) with
            | null -> None
            | t -> Some t ) 
      match ty with | Some t -> dict.Add(s, t) | _ -> ()
      ty

    //---------------------------------------------------------------------------------------------
    // Interface recognition magic

    // If the name of the type matches a name of interface declared in this file 
    // (stored in a set in the context) than we treat it as an interface, otherwise
    // we rely on .NET naming pattern (starts with I followed by uppercase letter)
    // We could search known DLLs, but that's useless since all DLLs we could find
    // follow this naming pattern...
    let isInterface (t:CodeTypeReference) (ctx:Context) = 
      let tn = t.BaseType.Substring(t.BaseType.LastIndexOf(".") + 1) 
      let decLoc = Set.contains tn ctx.DeclaredInterfaces
      decLoc || (tn.StartsWith("I") && (((tn.ToUpper()).[1]) = (tn.[1])))


    // Splits base types into base class and implemented interfaces
    // using rules described in <c>isInterface</c>
    // Returns optional base class and list of interfaces
    let resolveHierarchy (c:CodeTypeDeclaration) ctx =
      let (interf, bcl) = 
        c.BaseTypes |> Seq.cast |> Seq.toList
          |> List.partition ( fun (r:CodeTypeReference) -> isInterface r ctx )
          
      if (bcl.Length = 0) then 
        // All supertypes all interfaces
        (None, interf)
      elif (bcl.Length = 1) then 
        // Exactly one supertype is class, other were recognized as interfaces
        (Some (List.head bcl), interf) 
      else 
        // Fallback case - we found more than one supertypes that look like a class
        // so we just return the tryPick one and treat other as interfaces
        (Some (List.head bcl), (List.tail bcl)@interf) 
        
    
    //---------------------------------------------------------------------------------------------
    // Generating strings and working with context   
    
    let incIndent (ctx:Context) = 
      ctx.Writer.Indent <- ctx.Writer.Indent + 1
      ctx

    let decIndent (ctx:Context) = 
      ctx.Writer.Indent <- ctx.Writer.Indent - 1
      ctx

    /// Output string as a valid F# identifier
    let (-!) (ctx:Context -> Context) (str:String) x =
      let c = (ctx x)
      c.Writer.Write(makeValidIdentifier str);
      c;
      
    //---------------------------------------------------------------------------------------------
    // Default values, types, generic parameters
    
    
    
    
    
    
    let generateDefaultValue (t:CodeTypeReference) = 
      if (t.ArrayElementType <> null) then
        id -- "Unchecked.defaultof<_>"
      else
        match t.BaseType with 
          | "System.Single" -> id -- "0.0f"
          | "System.Double" -> id -- "0.0"
          | "System.Char" -> id -- "'\000'"
          | "System.Int16" -> id -- "0s"
          | "System.Int32" -> id -- "0"
          | "System.Int64" -> id -- "0L"
          | "System.Byte" -> id -- "0uy"
          | "System.SByte" -> id -- "0y"
          | "System.UInt16" -> id -- "0us"
          | "System.UInt32" -> id -- "0u"
          | "System.UInt64" -> id -- "0UL"
          | "System.String" -> id -- "\"\""
          | "System.Boolean" -> id -- "false"
          | _ -> id -- "Unchecked.defaultof<_>" 
    
    /// Get System.Type of know type (either standard type or resolved)  
    let tryGetSystemType (cr:CodeTypeReference option) =
      match cr with 
        | None -> None
        | Some cr when (cr.ArrayRank = 0) ->
          match cr.BaseType with 
          | "System.Single" -> Some (typeof<float32>)
          | "System.Double" -> Some (typeof<float>)
          | "System.Char" -> Some (typeof<char>)
          | "System.Int16" -> Some (typeof<int16>)
          | "System.Int32" -> Some (typeof<int>)
          | "System.Int64" -> Some (typeof<int64>)
          | "System.UInt16" -> Some (typeof<uint16>)
          | "System.UInt32" -> Some (typeof<uint32>)
          | "System.UInt64" -> Some (typeof<uint64>)
          | "System.String" -> Some (typeof<string>)
          | "System.Boolean" -> Some (typeof<bool>)
          | FoundSystemType t -> Some t
          | _ -> None;      
        | _ -> None

    /// Tries to resolve type of a variable and adds it to the Context dictionary
    let tryAddVariableType (name:string) (cr:CodeTypeReference) (varTypes:Map<string,Type>) =
      let ret t = Map.add name t varTypes
      match tryGetSystemType (Some cr) with 
        | Some t -> ret t; 
        | _ -> varTypes

    // Returns string with type arguments
    let rec getTypeArgs (tya:CodeTypeReferenceCollection) renames ns tyParams fsSyntax =
      if (tya.Count > 0) then
        let sb = new StringBuilder()
        sb.Append("<") |> ignore
        for a in tya do
          let str = (getTypeRef a renames ns tyParams fsSyntax):string
          sb.Append(str).Append(", ") |> ignore
        let s = sb.ToString()
        s.Substring(0, s.Length - 2) + ">"
      else
        ""
        
        // Several standard renaming tricks      
              
    and isKnownSealedType (t:CodeTypeReference) = 
        t.ArrayRank = 0 &&
        match t.BaseType with 
        | "System.String" 
        | "System.Single" 
        | "System.Double" 
        | "System.DateTime" 
        | "System.TimeSpan" 
        | "System.Decimal" 
        | "System.Char" 
        | "System.SByte" 
        | "System.Byte" 
        | "System.Int16" 
        | "System.Int32" 
        | "System.Int64" 
        | "System.UInt16" 
        | "System.UInt32" 
        | "System.UInt64" 
        | "System.Boolean" -> true
        | _ -> false;
    /// Generates type reference (not for arrays)
    and getBaseTypeRef (cr:CodeTypeReference) renames (ns:string) (tyParams:Set<string>) fsSyntax =
      let s = 
      
        // Remove current namespace name, because it can't be used in this scope
        let bst = 
          if (cr.BaseType.StartsWith(ns+".")) then 
            cr.BaseType.Substring(ns.Length+1) 
          elif cr.Options &&& CodeTypeReferenceOptions.GlobalReference <> enum 0 then 
            "global."+cr.BaseType
          else 
            cr.BaseType
      
        // Several standard renaming tricks      
        match Map.tryFind bst renames with
          // Renamed type (former nested type)
          | Some nn -> nn
          
          // It is a type paramter - rename T to 'T
          | None when Set.contains cr.BaseType tyParams ->
              "'" + cr.BaseType
              
          // Try if it's standard F# type
          // This also renames Void to unit, which may not be completly correct, 
          // but it works much better than if we don't do it
          | None when fsSyntax ->
              match cr.BaseType with 
              | "System.Void" -> "unit"
              | "System.Object" -> "obj"
              | "System.String" -> "string"
              | "System.Single" -> "float32"
              | "System.Double" -> "float"
              | "System.Char" -> "char"
              | "System.Int16" -> "int16" 
              | "System.Int32" -> "int" 
              | "System.Int64" -> "int64" 
              | "System.UInt16" -> "uint16" 
              | "System.UInt32" -> "uint32" 
              | "System.UInt64" -> "uint64" 
              | "System.Boolean" -> "bool"
              | _ -> bst;
          | _ -> bst;          
      // drop `xyz, replace "+" for nested classes with "."
      let sb = new StringBuilder()
      let mutable i = 0 
      while i < s.Length do
        let c = s.[i]
        match c with
          | _ when c = '+' || c = '.' -> sb.Append('.') |> ignore;
          | '`' -> i <- i + 1;
                   while (i<s.Length && s.[i]>='0' && s.[i]<='9') do 
                     i <- i + 1
          | _ -> sb.Append(c) |> ignore
        i <- i + 1      
      // generate type arguments
      sb.Append(getTypeArgs cr.TypeArguments renames ns tyParams fsSyntax).ToString()
      
    /// Generate type reference with empty context
    and getBaseTypeRefString (s:string) =
      getBaseTypeRef (CodeTypeReference(s)) Map.empty "" Set.empty true
    
    
    /// Get full type reference using information from context  
    and getTypeRef (c:CodeTypeReference) (rens:Map<string,string>) (ns:string) (tyParams:Set<string>) (fsSyntax:bool) =
      if (c = null) then  
        ""
      elif (c.ArrayRank = 0) then 
        getBaseTypeRef c rens ns tyParams fsSyntax
      else      
        let baseType = (getTypeRef c.ArrayElementType rens ns tyParams fsSyntax)
        baseType + "[" + (System.String.Concat (Array.create (c.ArrayRank - 1) ",")) + "]"

    /// Get full type reference string using empty context
    and getTypeRefSimple (c:CodeTypeReference) = getTypeRef c Map.empty "" Set.empty true
    
    /// Get type reference, but don't rename .NET types to F# types
    /// (this is only needed when calling static methods on the type)
    let generateTypeRefNet (c:CodeTypeReference) =
      id +> withCtxt ( fun ctx -> id -- getTypeRef c ctx.TypeRenames ctx.CurrentNamespace ctx.TypeArgumentNames false )
    
    /// Generate type reference using context
    /// (this is most commonly used method)  
    let generateTypeRef (c:CodeTypeReference) =
      id +> withCtxt ( fun ctx -> id -- getTypeRef c ctx.TypeRenames ctx.CurrentNamespace ctx.TypeArgumentNames true )
             
    /// Generate type arguments using context
    let generateTypeArgs (c:CodeTypeReferenceCollection) =
      id +> withCtxt ( fun ctx -> id -- getTypeArgs c ctx.TypeRenames ctx.CurrentNamespace ctx.TypeArgumentNames true )
             
    
    /// Record specified type parameters in the context, call generating function
    /// and then restore the original type parameters
    /// (this works if someone uses nested type parameters with the same name)
    let usingTyParams tyArgs f (x:Context) =
      let o = x.TypeArgumentNames
      let n = Array.foldBack Set.add (Array.ofSeq tyArgs) o
      let x = f { x with TypeArgumentNames = n }
      { x with TypeArgumentNames = o }      


    /// Preprocess collection with type parameters
    /// Returns array to be used with <c>usingTyParams</c> and
    /// function to be called to generate < ... > code
    let processTypeArgs (args:CodeTypeParameterCollection) =     
      let tyargs = seq { for (p:CodeTypeParameter) in args -> p.Name }       
      let genTyArgs =
        if (args.Count = 0) then id else
        let s = tyargs |> Seq.fold (fun ctx s -> ctx + ", '" + s) ""
        id 
        -- "<" -- s.Substring(2, s.Length-2)
        +> if (args.Count = 0) then id -- ">" else
             let argsWithConstr = args |> Seq.cast |> Seq.filter (fun (p:CodeTypeParameter) -> 
               p.Constraints.Count <> 0 || p.HasConstructorConstraint) |> Seq.cast |> Seq.toList
             if (argsWithConstr.Length <> 0) then
               id -- " when " +>
               col sepWordAnd argsWithConstr (fun (p:CodeTypeParameter) -> 
                 col sepWordAnd p.Constraints (fun impl ->
                   id -- "'" -- p.Name -- " :> " +> generateTypeRef impl)
                 +> if (not p.HasConstructorConstraint) then id else
                      if (p.Constraints.Count <> 0) then id -- " and " else id
                      -- "'" -- p.Name -- " : (new:unit->'" -- p.Name -- ")")
               -- ">"
             else id -- ">"
      tyargs, genTyArgs
    
    //---------------------------------------------------------------------------------------------
    // Binary operators and numeric functions
    
    /// Generates code for binary operator using function for left and right operand
    let binaryOp (op:CodeBinaryOperatorType) fleft fright =
      id -- "(" +>
      match op with    
        | CodeBinaryOperatorType.Add -> fleft -- " + " +> fright;
        | CodeBinaryOperatorType.BitwiseAnd -> fleft -- " &&& " +> fright;
        | CodeBinaryOperatorType.BitwiseOr -> fleft -- " ||| " +> fright;
        | CodeBinaryOperatorType.BooleanAnd -> fleft -- " && " +> fright;
        | CodeBinaryOperatorType.BooleanOr -> fleft -- " || " +> fright;
        | CodeBinaryOperatorType.Divide -> fleft -- " / " +> fright;
        | CodeBinaryOperatorType.GreaterThan -> fleft -- " > " +> fright;
        | CodeBinaryOperatorType.GreaterThanOrEqual -> fleft -- " >= " +> fright;
        | CodeBinaryOperatorType.LessThan -> fleft -- " < " +> fright;
        | CodeBinaryOperatorType.LessThanOrEqual -> fleft -- " <= " +> fright;
        | CodeBinaryOperatorType.Modulus -> fleft -- " % " +> fright;
        | CodeBinaryOperatorType.Multiply -> fleft -- " * " +> fright;
        | CodeBinaryOperatorType.Subtract -> fleft -- " - " +> fright;
        
        // REVIEW: this is not used in any tests and it is not sure what it means
        | CodeBinaryOperatorType.Assign -> fleft -- " <- " +> fright; 
        
        // REVIEW: reference and value equality use C# semantics, so it is not sure what we should generate
        | CodeBinaryOperatorType.ValueEquality -> fleft -- " = " +> fright;  
        | CodeBinaryOperatorType.IdentityEquality -> id -- "System.Object.ReferenceEquals(" +> fleft -- ", " +> fright -- ")"; 
        | CodeBinaryOperatorType.IdentityInequality -> id -- "not (System.Object.ReferenceEquals(" +> fleft -- ", " +> fright -- "))"; 
        | _ -> failwithf "unimplemented binary operator type '%A'" op;
      -- ")"
    
    /// Are both types numerical types where numeric conversion function can be applied?
    let rec isNumericConversion (src:Type) (target:Type) = 
      convertFunc src <> "" && convertFunc target <> ""
    
    
    /// Returns F# conversion function for the specified type (or empty string)
    and convertFunc (ty:Type) = 
      if (ty = (typeof<int16>)) then "int16"
      elif (ty = (typeof<int32>)) then "int32"
      elif (ty = (typeof<int64>)) then "int64"
      elif (ty = (typeof<int16>)) then "uint16"
      elif (ty = (typeof<int32>)) then "uint32"
      elif (ty = (typeof<int64>)) then "uint64"
      elif (ty = (typeof<float>)) then "float"
      elif (ty = (typeof<float32>)) then "float32"
      elif (ty = (typeof<decimal>)) then "decimal"
      elif (ty = (typeof<byte>)) then "byte"
      elif (ty = (typeof<sbyte>)) then "sbyte"
      else ""
    
    
    /// Generate value of primitive expression  
    let generatePrimitiveExpr (reqty:Type option) (c:CodePrimitiveExpression) =
      let (value, typ) = 
        match c.Value with
          | :? Char as c -> (sprintf "%A" c, Some(typeof<Char>))
          | :? String as s -> (sprintf "\"%s\"" (s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\t", "\\t").Replace("\r", "\\r").Replace("\b", "\\b")), Some(typeof<string>)) 
          | :? Boolean as b -> ((if (b) then "true" else "false"), Some(typeof<bool>))
          | :? Single as f -> (sprintf "%A" f, Some(typeof<float32>))
          | :? Double as f -> (sprintf "%A" f, Some(typeof<float>))
          | :? Byte as i -> (sprintf "%A" i, Some(typeof<Byte>))
          | :? SByte as i -> (sprintf "%A" i, Some(typeof<SByte>))
          | :? Int16 as i -> (sprintf "%A" i, Some(typeof<int16>))
          | :? Int32 as i -> (sprintf "%A" i, Some(typeof<int>))
          | :? Int64 as i -> (sprintf "%A" i, Some(typeof<int64>))
          | :? UInt16 as i -> (sprintf "%A" i, Some(typeof<uint16>))
          | :? UInt32 as i -> (sprintf "%A" i, Some(typeof<uint32>))
          | :? UInt64 as i -> (sprintf "%A" i, Some(typeof<uint64>))
          | null -> ("(Unchecked.defaultof<_>)", None)
          | _ -> ("(* Unknown primitive value '"+c.Value.ToString()+"' of type '"+
                  c.Value.GetType().Name+"'. Please report this to the F# team. *)", None) 
      match typ, reqty with        
        | Some t, Some rt when t <> rt -> id -- convertFunc rt -- " (" -- value -- ")"
        | _, _ ->  id -- value
        
        
    /// Generate array initializer. Checks generator options for ASP.NET workaround.
    let rec generateArrayCreateExpr (c:CodeArrayCreateExpression) =
      if (c.Initializers<>null && c.Initializers.Count>0) then
        id
        -- "([| " +> col sepArgsSemi c.Initializers generateExpression -- " |] : "
        +> withCtxt (fun ctx -> 
            generateTypeRef c.CreateType          
            -- if (ctx.Options &&& AdditionalOptions.AspNetArrays <> enum 0) then "[]" else "")
        -- ")" 
      else
        id
        -- "(Array.zeroCreate "
        +> if (c.SizeExpression <> null) then
              id -- "(" +> generateExpression c.SizeExpression -- ")"
           else
              id +> str c.Size
        -- ":"
        +> withCtxt (fun ctx -> 
            generateTypeRef c.CreateType          
            -- if (ctx.Options &&& AdditionalOptions.AspNetArrays <> enum 0) then "[]" else "")
        -- ")";
    
    /// Tries to resolve if type is an array, so we can generate 
    /// appropriate code (it can be either indexer or array, but we need to generate
    /// .Item call for indexers (no overloading is supported by .[]).
    /// Returns: "None" - can't resolve, "Some" resovled (true/false - is it an array?)
    and tryIsExpressionArray c (ctx:Context) = 
      match (c :> CodeExpression) with
        | :? CodeFieldReferenceExpression as ce when 
             (ce.TargetObject :? CodeThisReferenceExpression) -> 
             match Map.tryFind ce.FieldName ctx.FieldTypes with 
               | Some t -> Some (t.ArrayRank > 0)
               | None -> None
        | :? CodePropertyReferenceExpression as ce when 
             (ce.TargetObject :? CodeThisReferenceExpression) -> 
             match Map.tryFind ce.PropertyName ctx.PropertyTypes with 
               | Some t -> Some (t.ArrayRank > 0)
               | None -> None
        | _ -> None    
    
    
    /// Tries to resolve type of an expression using a few tricks:
    /// * Fields of current type may have known type
    /// * Properties of current type as well
    /// * We can also try to resolve other properties (sometimes it helps)
    /// * Resolve type for local variables or argument reference 
    and tryGetExpressionType c (ctx:Context) = 
      match (c :> CodeExpression) with
        | :? CodeFieldReferenceExpression as ce when 
             (ce.TargetObject :? CodeThisReferenceExpression) -> 
             tryGetSystemType (Map.tryFind ce.FieldName ctx.FieldTypes)
        | :? CodePropertyReferenceExpression as ce when 
             (ce.TargetObject :? CodeThisReferenceExpression) -> 
             tryGetSystemType (Map.tryFind ce.PropertyName ctx.PropertyTypes)
        | :? CodePropertyReferenceExpression as ce ->
               match (tryGetExpressionType ce.TargetObject ctx) with
                 | None -> None
                 | Some t ->
                     try 
                       Some (t.GetProperty(ce.PropertyName).PropertyType)
                     with _ ->
                       None
        | :? CodeArgumentReferenceExpression as ce ->                              
               Map.tryFind ce.ParameterName ctx.LocalVariableTypes  
               // NOTE:
               // XSD generates incorrect referenece (uses argument ref where it should be variable ref)
               // and unfortunately it is followed by wrong numeric type, so we need to workaround this
        | :? CodeVariableReferenceExpression as ce ->
               Map.tryFind ce.VariableName ctx.LocalVariableTypes 
        | _ -> None
      
    //---------------------------------------------------------------------------------------------
    // Generating code for expressions
          
    /// Generates a "this" or "CurrentType" reference depending on whether a reference
    /// is static or not. Used for "ambiguous" references without a type or object qualifier.
    ///
    /// Unfortunately the Code tree isn't so kind as to tell us whether a reference is static
    /// or not up front. Instead we predetermine a set of some static members and 
    /// assume all other references are instance references. 
    ///
    and generateExpressionDefaultThis isKnownStatic c = 
      withCtxt (fun ctx -> 
          match c with 
          | null -> 
              // REVIEW: this is still incorrect if the reference is static and it is a reference from an inherited type
              id -- (if isKnownStatic then ctx.CurrentType.Name else "this" )  
          | _ -> generateExpression c)
        
    /// Matches array or indexer expression and corrects it if the generated CodeDOM is incorrect
    and (|CodeArrayAccessOrIndexer|_|) (ctx:Context) (c:CodeExpression) =
      let noneT b = match b with Some v -> v | _ -> true
      let noneF b = match b with Some v -> v | _ -> false
      match c with 
        | :? CodeArrayIndexerExpression as ce -> 
             Some(true && (noneT (tryIsExpressionArray ce.TargetObject ctx)), ce.TargetObject, ce.Indices)
        | :? CodeIndexerExpression as ce -> 
             Some(false || (noneF (tryIsExpressionArray ce.TargetObject ctx)), ce.TargetObject, ce.Indices)
        | _ -> None
      
    /// Generate expression - with unkonw type
    and generateExpression c = generateExpressionTyped None c    
 
    // Generates code for CodeExpression
    // If the caller knows the expected type of the expression it can be given as an argument,
    // but currently it is used only when generating primitve expression to convert value to the right type
    and generateExpressionTyped ty c ctx = 
      (match c with 
        | :? CodeArgumentReferenceExpression as ce ->
              id -! ce.ParameterName
        | :? CodeArrayCreateExpression as ce ->
              id +> generateArrayCreateExpr ce              
        
        // for indexers we generate get_Item to handle overloading
        | CodeArrayAccessOrIndexer ctx (isArray, target, indices) ->
              id
              +> generateExpression target -- "."
              +> id -- "[" +> col sepArgs indices generateExpression -- "]" 
              
        | :? CodeBaseReferenceExpression as ce ->
              id -- "base"
              
        | :? CodeBinaryOperatorExpression as ce ->
              binaryOp ce.Operator (generateExpressionTyped ty ce.Left) (generateExpressionTyped ty ce.Right)
        
        // casting can also represent numeric conversion - we try to detect that case      
        | :? CodeCastExpression as ce -> 
              id 
              +> withCtxt (fun ctx -> 
                  match tryGetExpressionType (ce.Expression) ctx, tryGetSystemType (Some ce.TargetType) with
                  | Some(t1), Some(t2) when isNumericConversion t1 t2 ->
                      id
                      -- "(" -- (convertFunc t2)
                      -- "(" +> generateExpression ce.Expression -- "))"
                  | _ ->
                    id
                    -- "((" +> generateExpression ce.Expression -- " :> obj) :?> " +> generateTypeRef ce.TargetType -- ")" )
        
        // argument for "ref" or "out" C# parameter - both generated as byref in F#
        | :? CodeDirectionExpression as ce ->
              match ce.Direction with 
              | FieldDirection.Out 
              | FieldDirection.Ref -> 
                  id -- "&" +> generateExpression ce.Expression
              | _ -> 
                  id +> generateExpression ce.Expression
        
        // for delegates, we use 'FuncFromTupled' to get the right function type      
        | :? CodeDelegateCreateExpression as ce ->
              id 
              -- "new " +> generateTypeRef ce.DelegateType -- "(FuncConvert.FuncFromTupled " 
              +> generateExpression ce.TargetObject 
              -- "." -- ce.MethodName -- ")";
        
        | :? CodeDelegateInvokeExpression as ce ->
              id
              +> match ce.TargetObject with 
                  // "this.<DeclaredEventName>( ... )" - will be translated to a raise function returned
                  // by create_DelegateEvent
                  | :? CodeEventReferenceExpression as eref when  
                      (eref.TargetObject :? CodeThisReferenceExpression) 
                      && ((ctx.DeclaredEvents |> List.tryFind (fun e -> e.Name = eref.EventName)) <> None) -> 
                      // F# declared event..
                      id
                      -- "this._invoke_" -- eref.EventName -- " [| "
                      +> col sepArgsSemi ce.Parameters (fun (e:CodeExpression) ->
                           id
                           -- " box (" 
                           +> generateExpression e
                           -- ")" ) -- " |]"
                  // other than this.<Event>(). This may not be correct (but works on cases in test suite)
                  | _ -> 
                      generateExpression ce.TargetObject 
                      -- ".Invoke(" +> col sepArgs ce.Parameters generateExpression -- ")"
        
        // this prevents using mutable variable in a way it would escape its scope
        | :? CodeEventReferenceExpression as ce ->
              id -- "let __e = " +> generateExpression ce.TargetObject -- " in __e." -- ce.EventName
                          
        | :? CodeFieldReferenceExpression as ce -> 
              withCtxt (fun ctx ->  
              
                // if 'UnknownFieldsAsLocals' is set than the code will generate
                // "fld" instead of "this.fld" when accessing field that is not known
                let sft =
                  match ce.TargetObject with
                  | :? CodeThisReferenceExpression as t when 
                       (ctx.Options &&& AdditionalOptions.UnknonwFieldsAsLocals <> enum 0) ->
                       Option.isNone (Map.tryFind ce.FieldName ctx.FieldTypes)
                  | _ -> false
                if sft then
                  id -! ce.FieldName
                else
                  id 
                  +> match ce.TargetObject with 
                       | :? CodeTypeReferenceExpression as ct ->
                             id +> generateTypeRefNet ct.Type
                       | _ -> 
                           let isKnownStatic = 
                               match ce.TargetObject, ctx.AllFields.TryFind ce.FieldName with 
                               | null, Some m -> 
                                  (m.Attributes &&& MemberAttributes.ScopeMask = MemberAttributes.Static) 
                               | _ -> false
                           generateExpressionDefaultThis isKnownStatic ce.TargetObject
                 -- "." -- ce.FieldName )
              
        | :? CodeMethodInvokeExpression as ce ->
              id 
              +> generateExpression (ce.Method :> CodeExpression) 
              -- "(" +> col sepArgs ce.Parameters generateExpression -- ")" 
              
        | :? CodeMethodReferenceExpression as ce ->
              id 
              +> match ce.TargetObject with 
                   | :? CodeTypeReferenceExpression as ct ->
                         id +> generateTypeRefNet ct.Type
                   | _ -> 
                       let isKnownStatic = 
                           match ce.TargetObject, ctx.AllMeths.TryFind ce.MethodName with 
                           | null, Some m -> 
                              (m.Attributes &&& MemberAttributes.ScopeMask = MemberAttributes.Static) 
                           | _ -> false
                       generateExpressionDefaultThis isKnownStatic ce.TargetObject
              -- "." -- ce.MethodName 
              +> generateTypeArgs ce.TypeArguments
              
        | :? CodeObjectCreateExpression as ce ->
              id
              -- "new " +> generateTypeRef ce.CreateType 
              -- "(" +> col sepArgs ce.Parameters generateExpression -- ")" 
              
        | :? CodePrimitiveExpression as ce -> 
              id +> generatePrimitiveExpr ty ce 
              
        | :? CodePropertyReferenceExpression as ce ->
              id 
              +> match ce.TargetObject with 
                   | :? CodeTypeReferenceExpression as ct ->
                         id +> generateTypeRefNet ct.Type
                   | _ -> 
                       let isKnownStatic = 
                           match ce.TargetObject, ctx.AllProps.TryFind ce.PropertyName with 
                           | null, Some m -> 
                              (m.Attributes &&& MemberAttributes.ScopeMask = MemberAttributes.Static) 
                           | _ -> false
                       generateExpressionDefaultThis isKnownStatic ce.TargetObject
              -- "." -- ce.PropertyName 
              
        | :? CodePropertySetValueReferenceExpression as ce ->  
              id -- "value"
              
        // we move all lines of "snippets" by 100 columns so it isn't violating #light rules
        | :? CodeSnippetExpression as ce ->
              let strs = 
                ce.Value.Split([| '\r'; '\n' |], StringSplitOptions.RemoveEmptyEntries)
                |> Array.map (fun s -> String(' ',100) + s )
              colT sepNone strs (fun s -> id ++ s)

        | :? CodeThisReferenceExpression as ce ->  
              id -- "this"
              
        | :? CodeTypeOfExpression as ce ->  
              id -- "(typeof<" +> generateTypeRef ce.Type -- ">)"
              
        | :? CodeTypeReferenceExpression as ce ->  
              id +> generateTypeRef ce.Type 
              
        | :? CodeVariableReferenceExpression as ce ->            
              match ty with 
              | Some t when (convertFunc t) <> "" -> id -- "(" -- (convertFunc t) -- " " -! ce.VariableName -- ")"
              | _ -> id -! ce.VariableName
              
        | null ->
            id 
            
        | _ -> id 
               -- "(* Unknown expression type '" -- (c.GetType().Name) 
               -- "' please report this to the F# team. *)") ctx
    
    //---------------------------------------------------------------------------------------------
    // Generating code for statements
      
    and generateVariableDeclStmt (c:CodeVariableDeclarationStatement) =
      id
      +> (fun ctx -> { ctx with LocalVariableTypes = tryAddVariableType c.Name c.Type ctx.LocalVariableTypes } )
      ++ "let mutable (" -! c.Name -- ":" +> generateTypeRef c.Type -- ") = "
      +> if (c.InitExpression <> null) then 
           (generateExpressionTyped (tryGetSystemType (Some c.Type))) c.InitExpression 
         else 
           (generateDefaultValue c.Type);
    
    // REVIEW: Line pragmas don't work with the #light syntax
    let generateLinePragma (l:CodeLinePragma) = 
      if (l = null) then id else
        id 
        ++ "# " +> str l.LineNumber -- " \"" -- l.FileName -- "\""
        
    let rec generateCatchClause (c:CodeCatchClause) =
      id
      ++ "| :? " +> generateTypeRef c.CatchExceptionType 
      -- " as " -- c.LocalName -- " ->" +> incIndent
      +> generateStatements c.Statements +> decIndent

    and generateStatements (sts:CodeStatementCollection) = 
      let fix = 
        if (sts.Count = 0 || (sts.[sts.Count - 1] :? CodeVariableDeclarationStatement))
          then id ++ "()" else id
      col sepNone sts generateStatement +> fix      

    // Generates block of statements which can return a value
    and generateStatementBlock typ (statements:CodeStatementCollection) =        
      // determine if the block uses only "safe" return statements
      // that can be translated to functional returns without using exceptions
      let safeReturns = 
        statements 
        |> codeDomCallbackWithScope (fun rcall safeScope res o -> 
            match o with 
            | :? CodeMethodReturnStatement as ret -> safeScope && res
            | :? CodeTryCatchFinallyStatement as tfs -> rcall (safeScope && (tfs.CatchClauses.Count = 0)) res o
            | :? CodeStatementCollection -> rcall safeScope res o
            | _ -> rcall false res o ) true true
    
      id
      +> incIndent
      +> (fun ctx -> { ctx with CurrentMethodReturnType=typ; 
                                LocalVariableTypes = Map.empty; 
                                ReturnUsingException = not safeReturns })
      // if returning using exception - wrap inside try .. catch
      +> if (not safeReturns) then id ++ "try" +> incIndent else id
      +> generateStatements statements
      +> if (safeReturns) then id else
           match typ with 
           | Some t when t.BaseType <> "System.Void" -> 
               id ++ "failwith \"Code branch didn't return any value!\";"
               +> decIndent
               ++ "with" ++ "    | ReturnException" +> uniqid -- " v -> (v :?> " +> generateTypeRef t -- ")"
           | _ ->  
               id ++ "raise ReturnNoneException" +> uniqid 
               +> decIndent
               ++ "with" ++ "    | ReturnNoneException" +> uniqid -- " -> ()"
      +> (fun ctx -> {ctx with CurrentMethodReturnType=None; 
                               LocalVariableTypes = Map.empty; 
                               ReturnUsingException = false })
      +> decIndent

    and generateComment (c:CodeComment) =
      id 
      -- if c.DocComment then "/// " else "// " 
      -- (c.Text);

    and generateExpressionThenUpCast e (t: CodeTypeReference) = 
        if isKnownSealedType t then 
            generateExpression e
        else
            id -- "((" +> generateExpression e -- " :> obj) :?> " +> generateTypeRef t -- ")" 
          
    and generateStatement (c:CodeStatement) = 
      (generateLinePragma c.LinePragma) +>
      (match c with 
        | :? CodeAssignStatement as cs -> 
              match cs.Left with 
                | :? CodeIndexerExpression as ci ->
                    id ++ "" 
                    +> generateExpressionDefaultThis false ci.TargetObject -- ".set_Item(" 
                    +> col sepArgs ci.Indices generateExpression -- ", "
                    +> withCtxt (fun ctx -> generateExpressionTyped (tryGetExpressionType cs.Left ctx) cs.Right)
                    -- ")"
                | _ ->
                    id ++ "" +> generateExpression cs.Left 
                    -- " <- " 
                    +> withCtxt (fun ctx -> generateExpressionTyped (tryGetExpressionType cs.Left ctx) cs.Right)
                    
        | :? CodeAttachEventStatement as cs ->
              id ++ "" +> generateExpression (cs.Event :> CodeExpression) 
              -- ".AddHandler(" +> generateExpression cs.Listener -- ")"
              
        | :? CodeCommentStatement as cs -> 
              id ++ "" +> generateComment cs.Comment 
              
        | :? CodeConditionStatement as cs ->
              id 
              ++ "if " +> generateExpression cs.Condition -- " then"
              +> incIndent +> col sepNone cs.TrueStatements generateStatement +> decIndent
              +> if (cs.FalseStatements<>null && cs.FalseStatements.Count>0) then 
                   id 
                   ++ "else" +> incIndent 
                   +> col sepNone cs.FalseStatements generateStatement +> decIndent else id                
                   
        | :? CodeExpressionStatement as cs -> 
              id ++ "" +> generateExpression cs.Expression -- " |> ignore";
              
        | :? CodeIterationStatement as cs ->
              id 
              +> generateStatement cs.InitStatement
              ++ "while " +> generateExpression cs.TestExpression -- " do"
              +> incIndent
              +> col sepNone cs.Statements generateStatement 
              +> generateStatement cs.IncrementStatement
              +> decIndent
              
        // Return - either throw "ReturnException" or just generate F# expression with the value
        | :? CodeMethodReturnStatement as cs -> 
              id
              +> withCtxt (fun ctx -> 
                   if (ctx.ReturnUsingException) then
                       id 
                       ++ "raise ("
                       +> match ctx.CurrentMethodReturnType with
                          | Some t when t.BaseType <> "System.Void" -> 
                              id -- "ReturnException" +> uniqid -- "(" +> generateExpressionThenUpCast cs.Expression t -- ")"
                          | _ -> 
                              id -- "ReturnNoneException" +> uniqid
                       -- ")"
                   else 
                       match ctx.CurrentMethodReturnType with
                       | Some t when t.BaseType <> "System.Void" -> 
                           id 
                           ++ "" +> generateExpressionThenUpCast cs.Expression t
                       | _ ->      id ++ "")
                     
        | :? CodeSnippetStatement as cs ->
              let strs = cs.Value.Split([| '\r'; '\n' |], StringSplitOptions.RemoveEmptyEntries);
              colT sepNone strs (fun s -> id ++ s)
              
        | :? CodeVariableDeclarationStatement as cs -> 
              id +> generateVariableDeclStmt cs
              
        | :? CodeThrowExceptionStatement as cs ->            
              id ++ "raise (" +> generateExpression cs.ToThrow -- ")"
        
        // try .. catch .. finaly is generated as try (try .. catch) finally      
        | :? CodeTryCatchFinallyStatement as cs -> 
              let hasCatch = (cs.CatchClauses<>null && cs.CatchClauses.Count>0) 
              let hasFinally = (cs.FinallyStatements<>null && cs.FinallyStatements.Count>0) 
              id 
              ++ "try" +> incIndent
              +> if (hasCatch && hasFinally) then id ++ "try" +> incIndent else id
              +> generateStatements cs.TryStatements 
              +> if (cs.CatchClauses<>null && cs.CatchClauses.Count>0) then 
                   decIndent 
                   ++ "with" +> incIndent 
                   +> col sepNone cs.CatchClauses generateCatchClause
                   +> decIndent else id;
              +> if (cs.FinallyStatements<>null && cs.FinallyStatements.Count>0) then 
                   decIndent
                   ++ "finally" +> incIndent 
                   +> col sepNone cs.FinallyStatements generateStatement
                   +> decIndent else id;
                   
        | _ -> id 
               -- "(* Unknown statement type '" -- (c.GetType().Name) 
               -- "' please report this to the F# team. *)")

    //---------------------------------------------------------------------------------------------
    // Support for class members (Custom attributes, paramters, etc..)
        
    let generateAttributeArg (c:CodeAttributeArgument) =
      id
      +> if (c.Name<> null && c.Name.Length>0) then 
          id -- c.Name -- "=" else id
      +> generateExpression c.Value;
      
    let generateCustomAttrDecl (c:CodeAttributeDeclaration) =
      id
      -- (getBaseTypeRefString c.Name)
      +> if (c.Arguments.Count = 0) then id else
           id -- "(" +> (col sepArgs c.Arguments generateAttributeArg) -- ")" 
          
    let generateCustomAttrDeclsList (c:CodeAttributeDeclaration list) =
      id 
      +> if (c.Length = 0) then id else 
           id ++ "[<" +> (colT sepNlnSemiSpace c generateCustomAttrDecl) -- ">]"
          
    let generateCustomAttrDeclsForType (c:CodeAttributeDeclaration list) (a:Reflection.TypeAttributes) =
      id 
      +> if (c.Length = 0)
            && (a &&& TypeAttributes.Abstract  = enum 0)
            && (a &&& TypeAttributes.Sealed  = enum 0) then id 
         else 
           id ++ "[<" 
                 +> (colT sepNlnSemiSpace [ for x in c do yield generateCustomAttrDecl x
                                            if a &&& TypeAttributes.Abstract <> enum 0 then yield (id -- "Microsoft.FSharp.Core.AbstractClassAttribute" +> sepNlnSemiSpace)
                                            if a &&& TypeAttributes.Sealed   <> enum 0 then yield (id -- "Microsoft.FSharp.Core.SealedAttribute" +> sepNlnSemiSpace) ]
                                          (fun c -> c) )                                           
              -- ">]"
          
(*
VisibilityMask Specifies type visibility information. 
 NotPublic Specifies that the class is not public. 
 Public Specifies that the class is public. 
 NestedPublic Specifies that the class is nested with public visibility. 
 NestedPrivate Specifies that the class is nested with private visibility. 
 NestedFamily Specifies that the class is nested with family visibility, and is thus accessible only by methods within its own type and any subtypes. 
 NestedAssembly Specifies that the class is nested with assembly visibility, and is thus accessible only by methods within its assembly. 
 NestedFamANDAssem Specifies that the class is nested with assembly and family visibility, and is thus accessible only by methods lying in the intersection of its family and assembly. 
 NestedFamORAssem Specifies that the class is nested with family or assembly visibility, and is thus accessible only by methods lying in the union of its family and assembly. 
 LayoutMask Specifies class layout information. 
 AutoLayout Specifies that class fields are automatically laid out by the common language runtime. 
 SequentialLayout Specifies that class fields are laid out sequentially, in the order that the fields were emitted to the metadata. 
 ExplicitLayout Specifies that class fields are laid out at the specified offsets. 
 ClassSemanticsMask Specifies class semantics information; the current class is contextful (else agile). 
 Class Specifies that the type is a class. 
 Interface Specifies that the type is an interface. 
    DONE: Abstract Specifies that the type is abstract. 
    DONE: Sealed Specifies that the class is concrete and cannot be extended. 
 SpecialName Specifies that the class is special in a way denoted by the name. 
 Import Specifies that the class or interface is imported from another module. 
 Serializable Specifies that the class can be serialized. 
 StringFormatMask Used to retrieve string information for native interoperability. 
 AnsiClass LPTSTR is interpreted as ANSI. 
 UnicodeClass LPTSTR is interpreted as UNICODE. 
 AutoClass LPTSTR is interpreted automatically. 
 CustomFormatClass LPSTR is interpreted by some implementation-specific means, which includes the possibility of throwing a NotSupportedException. 
 CustomFormatMask Used to retrieve non-standard encoding information for native interop. The meaning of the values of these 2 bits is unspecified. 
 BeforeFieldInit Specifies that calling static methods of the type does not force the system to initialize the type. 
 ReservedMask Attributes reserved for runtime use. 
 RTSpecialName Runtime should check name encoding. 
 HasSecurity 
*)

    let generateCustomAttrDecls (c:CodeAttributeDeclarationCollection) = 
      generateCustomAttrDeclsList (c |> Seq.cast |> Seq.toList)
      
    // NOTE: may contain custom attributes - this isn't supported
    let generateParamDecl (c:CodeParameterDeclarationExpression) =
      let dir = if (c.Direction <> FieldDirection.In) then " byref" else ""
      id 
      -! c.Name -- ":" +> generateTypeRef c.Type -- dir;

    // NOTE: may contain custom attributes - this isn't supported
    let generateAbstractParamDecl (c:CodeParameterDeclarationExpression) =
      let dir = if (c.Direction <> FieldDirection.In) then " byref" else ""
      id +> generateTypeRef c.Type -- dir

    // Find all overloads of the method, so we can produce [<OverloadID>]
    let getMethodOverloads (membs:CodeTypeMemberCollection) = 
      let getMethodOverload map (n:CodeMemberMethod) = 
        let n = (n.Name, getTypeRefSimple n.PrivateImplementationType)
        match Map.tryFind n map with 
          | Some v -> v 
          | None -> 0
      let incMethodOverload (n:CodeMemberMethod) map = 
        let n = (n.Name, getTypeRefSimple n.PrivateImplementationType)
        match Map.tryFind n map with 
          | Some v -> Map.add n (v+1) map
          | None -> Map.add n 1 map          
      let m,a = 
        membs 
        |> codeDomCallBackNoScope 
            (fun rcall (res,mlst) o -> 
                match o with 
                  | :? CodeMemberMethod as meth when meth.GetType() = (typeof<CodeMemberMethod>) -> 
                      // we have found another method
                      (incMethodOverload meth res, 
                       ( meth, 
                         getMethodOverload res meth,
                         getTypeRefSimple meth.PrivateImplementationType
                       )::mlst)
                  | :? CodeTypeMemberCollection -> 
                       // recursively walk through member collection
                       rcall (res,mlst) o
                  | _ -> (res,mlst))
            (Map.empty, [])
      getMethodOverload m, a
      
    //---------------------------------------------------------------------------------------------
    // Fields, properties, constructors, methods
    
    /// fields 
    let generateField (c:CodeMemberField) =    
      id
      +> generateCustomAttrDecls c.CustomAttributes
      +> if ((c.Attributes &&& MemberAttributes.ScopeMask) = MemberAttributes.Static) then
            id
            ++ "[<Microsoft.FSharp.Core.DefaultValueAttribute(false)>]"
            ++ "static val mutable private " -- c.Name -- ":" +> generateTypeRef c.Type
            //++ (match c.InitExpression with
                 
         elif ((c.Attributes &&& MemberAttributes.ScopeMask) = MemberAttributes.Const) then
            id
            ++ "static member " -- c.Name -- " = " +> generateExpression c.InitExpression // should have initial value!
         else
             id ++ "[<Microsoft.FSharp.Core.DefaultValueAttribute(false)>]"
                ++ "val mutable " -- c.Name -- ":" +> generateTypeRef c.Type
    
    /// Abstract property in the interface 
    let generateInterfaceMemberProperty (c:CodeMemberProperty) =    
      id 
      ++ "abstract " -- c.Name -- " : " 
      +> (if c.Parameters.Count  > 0 then col sepStar c.Parameters generateAbstractParamDecl -- " -> " else id) 
      +> generateTypeRef c.Type -- " with " -- (if c.HasGet && not c.HasSet then "get" elif c.HasGet && c.HasSet then "get,set" else "set")

    // REVIEW: this is not correct, it should follow same abstract/default/override logic
    // as methods. Unfortunately it isn't possible to declare "abstract" property with "default" implementation
    let generateClassProperty (typ:MemberGenerateType)  (p:CodeMemberProperty) =    
    
      (if typ = MemberGenerateType.InsideStruct ||
          p.Attributes &&& MemberAttributes.ScopeMask = MemberAttributes.Override ||
          p.Attributes &&& MemberAttributes.ScopeMask = MemberAttributes.Static 
       then id
       else (id 
             ++ ""
             +> generateInterfaceMemberProperty p))
      +> generateCustomAttrDecls p.CustomAttributes
      ++ if typ = MemberGenerateType.InsideStruct then "member this."
         elif (p.Attributes &&& MemberAttributes.ScopeMask = MemberAttributes.Override) then "override  this." 
         elif (p.Attributes &&& MemberAttributes.ScopeMask = MemberAttributes.Static) then "static member " 
         else "default this."
      -- p.Name  

      +> if (not p.HasGet) then id else 
         incIndent
         ++ "with get("
         +> col sepArgs p.Parameters generateParamDecl
         -- ") : " +> generateTypeRef p.Type -- " =" 
         +> generateStatementBlock (Some p.Type) p.GetStatements 
         +> decIndent
      +> if (not p.HasSet) then id else 
         incIndent
         ++ (if p.HasGet then "and" else "with") -- " set(" 
         +> col sepNone p.Parameters (fun p -> (generateParamDecl p) -- ", ")
         -- "value:" +> generateTypeRef p.Type 
         -- ") : unit =" 
         +> generateStatementBlock None p.SetStatements 
         +> decIndent
    
    // The argument 'c' can be null when generating default ctor 
    // (which is not generated by the compiler as in C#)
    let generateConstructor (c:CodeConstructor) =    
      // Find all (non-static) fields
      withCtxt (fun ctx -> 
          let fields = 
            ctx.CurrentType.Members
              |> codeDomFlatFilter (fun o -> 
                   match o with 
                     | :? CodeMemberField as fld -> 
                       let keep = 
                         (fld.Attributes &&& MemberAttributes.ScopeMask <> MemberAttributes.Static) &&
                         (fld.Attributes &&& MemberAttributes.ScopeMask <> MemberAttributes.Const)  &&
                         (match fld.InitExpression with null -> false | _ -> true)

                       (keep, false) 
                     | _ -> 
                       (false, false) )
              |> List.map ( fun f -> f :?> CodeMemberField )
          id
          +> (if c <> null then generateCustomAttrDecls c.CustomAttributes else id)
          ++ "new(" 
          +> if (c <> null) then (col sepArgs c.Parameters generateParamDecl) else id
          -- ") as this ="
          +> incIndent
          ++ "{"
          +> incIndent
          // Calling base constructor?
          +> if (c = null || c.BaseConstructorArgs = null || c.BaseConstructorArgs.Count = 0) then id else
                 let (b, i) = ctx.BaseTypes
                 match b with 
                   | None -> failwith "Calling constructor of nonexisting base?"
                   | Some t -> 
                      id 
                      ++ "inherit " +> generateTypeRef t -- "("
                      +> col sepArgs c.BaseConstructorArgs generateExpression
                      --");"; 
          // Generate events
          +> decIndent
          ++ "}"
          +> if ((c <> null && c.Statements.Count > 0) || not fields.IsEmpty || not ctx.DeclaredEvents.IsEmpty) then
               id
               -- " then"
               +> incIndent
               +> incIndent
               // Initialize events
               +> colT sepNone ctx.DeclaredEvents ( fun e -> 
                           id 
                           ++ "let t_event_" -- e.Name -- " = new DelegateEvent<" +> generateTypeRef e.Type -- ">();"
                           ++ "this._event_" -- e.Name -- " <- t_event_" -- e.Name -- ".Publish;"
                           ++ "this._invoke_" -- e.Name -- " <- t_event_" -- e.Name -- ".Trigger;" ) 
               // Initialize fields
               +> colT sepNone fields (fun fld -> 
                        id ++ "this." -- fld.Name -- " <- " +> generateExpression fld.InitExpression-- ";" )
               // Run other initialization code
               +> (if c <> null && c.Statements.Count > 0 then 
                     id
                     ++ "begin"
                     +> generateStatementBlock (None) c.Statements 
                     ++ "end"
                   else 
                      id)
               +> decIndent
               +> decIndent
             else 
               id
          +> decIndent)

    /// Abstract method in the interface
    let generateInterfaceMemberMethod (c:CodeMemberMethod, overloadId:int) =
      let custAttrs = (c.CustomAttributes |> Seq.cast |> Seq.toList)

      let tyargs, genTyArgs = processTypeArgs c.TypeParameters       
      usingTyParams tyargs 
        (id
        +> col sepNone c.Comments generateStatement
        +> generateCustomAttrDeclsList custAttrs
        ++ "abstract "
        -- c.Name 
        +> genTyArgs
        -- " : "
        +> if (c.Parameters.Count > 0) then
             id +> col sepStar c.Parameters generateAbstractParamDecl
           else
             id -- "unit"
        -- " -> "
        +> generateTypeRef c.ReturnType)
      
    /// By default all CodeDOM generated methods are 'virtual' which means that 
    /// we have to generate "abstract and default" (unless we're in struct or
    /// we're implementing an interface, or the method is overriden)
    /// (NOTE: the same logic isn't properly implemented for properties)
    let generateMethod (typ:MemberGenerateType) (c:CodeMemberMethod) genAttrFunc =    
      
      let prefx, mnm =
        if (typ = MemberGenerateType.InsideInterface) then
          id, "member this."
        elif (typ = MemberGenerateType.InsideStruct) then
          id, "member this."
        elif (c.Attributes &&& MemberAttributes.ScopeMask = MemberAttributes.Static) then 
          id, "static member "
        elif (c :? CodeEntryPointMethod) then
          id, "static member "
        elif (c.Attributes &&& MemberAttributes.ScopeMask = MemberAttributes.Abstract) then 
          (id +> generateInterfaceMemberMethod (c, -1)),
          ""
        elif (c.Attributes &&& MemberAttributes.ScopeMask = MemberAttributes.Override) then 
          id, "override this." 
        else
          (id +> generateInterfaceMemberMethod (c, -1)),
          "default this."

      //REVIEW: This is mutating the CodeMemberMethod which is a little questionable
      if c.Name = "" then c.Name <- freshName ()
      if (mnm = "") then prefx else
      let tyargs, genTyArgs = processTypeArgs c.TypeParameters       
      usingTyParams tyargs 
        (prefx
        +> genAttrFunc
        ++ mnm -- c.Name +> genTyArgs -- " "
        -- " (" +> col sepArgs c.Parameters generateParamDecl -- ")"
        -- " ="
        
        // We need to create mutable copy of all arguments except for "byref" arguments which are mutable
        +> incIndent
        +> col sepNone c.Parameters (fun (c:CodeParameterDeclarationExpression) ->  
             if (c.Direction <> FieldDirection.In) then id else
               id ++ "let mutable " -- c.Name -- " = " -- c.Name ) 
        +> decIndent     
        +> generateStatementBlock (Some c.ReturnType) c.Statements)

    /// Generates method code
    /// Generates comments and than calls 'generatMethod'
    let generateClassMemberMethod (typ:MemberGenerateType) (c:CodeMemberMethod, overloadId:int) =
      let custAttrs = (c.CustomAttributes |> Seq.cast |> Seq.toList)
      id
      +> col sepNone c.Comments generateStatement
      +> generateMethod typ c (generateCustomAttrDeclsList custAttrs)
    
    let generateEntryPointMethod (typ:MemberGenerateType) (c:CodeEntryPointMethod)  = 
      id
      +> (fun ctx -> {ctx with MainMethodForCurrentNamespace = Some (c, ctx.CurrentType)})
      +> (generateClassMemberMethod typ ((c :> CodeMemberMethod), -1))
    
    let generateEvent (c:CodeMemberEvent) = 
      id
      +> generateCustomAttrDecls c.CustomAttributes
      ++ "[<CLIEvent>]"
      ++ "member this." -- c.Name -- " ="
      +> incIndent
      ++ "this._event_" -- c.Name
      +> decIndent
    
    let generateEventField (c:CodeMemberEvent) =
      id
      +> (fun ctx -> { ctx with DeclaredEvents = c::ctx.DeclaredEvents })
      ++ "[<Microsoft.FSharp.Core.DefaultValueAttribute(false)>]"
      ++ "val mutable _event_" -- c.Name -- " : IDelegateEvent<" +> generateTypeRef c.Type -- ">;"
      ++ "[<Microsoft.FSharp.Core.DefaultValueAttribute(false)>]"
      ++ "val mutable _invoke_" -- c.Name -- " : obj[] -> unit;";
    

    let generateCodeSnippetMember (c:CodeSnippetTypeMember) =
      
      // Remove additional spaces to make sure that the code aligns with the rest
      // CONSIDER: what to do with '\t' ?
      let countSpaces (s:string) =
        let rec countSpacesAux (s:string) i n = 
          if i >= s.Length then n
          elif s.[i] = ' ' then countSpacesAux s (i + 1) (n + 1)
          else n
        countSpacesAux s 0 0
              
      let lines = c.Text.Split([| '\n'; '\r' |], StringSplitOptions.RemoveEmptyEntries)
      if lines.Length > 0 then 
          let spaces = Array.foldBack (countSpaces >> min) lines Int32.MaxValue
          let lines = lines |> Array.map (fun s -> s.[spaces..])
          
          // ASP.NET doesn’t use any comments or custom attributes, 
          // but I assume this would be the right order
          id 
          +> col sepNone c.Comments generateStatement
          +> generateLinePragma c.LinePragma
          +> generateCustomAttrDecls c.CustomAttributes
          +> colT sepNone lines ((++) id)
      else 
          id

            
    //---------------------------------------------------------------------------------------------
    // Interfaces and classes and other types

    let generateInterfaceImplementation (ifcnfo:KeyValuePair<_, _>) =
      let name = ifcnfo.Key
      let membs = ifcnfo.Value
      id 
      ++ "interface " -- name -- " with"
      +> incIndent
      +> colT sepNln membs (generateClassMemberMethod MemberGenerateType.InsideInterface)
      +> decIndent
      ++ "end"

    let generateClassMember typ (c:CodeTypeMember) =
      match c with 
      | :? CodeTypeDeclaration -> id 
      | :? CodeMemberField 
      | :? CodeMemberEvent 
      | :? CodeConstructor 
      | :? CodeMemberProperty ->
            id
            +> col sepNone c.Comments generateStatement
            +> match c with 
                 | :? CodeMemberField as cm -> generateField cm
                 | :? CodeMemberEvent as cm -> generateEvent cm
                 | :? CodeConstructor as cm -> generateConstructor cm
                 | :? CodeMemberProperty as cm -> generateClassProperty typ cm
                 | _ -> failwithf "unimplemented CodeTypeMember '%A'" c
      | _ ->
            id ++ "(* Member of type '" +> str (c.GetType().Name) --  "' is not supported by the CodeDOM provider and was omitted *)" 

      
    let generateClassOrStruct structOrCls (scope:string list) (c:CodeTypeDeclaration) ctx =
      // affects members
      let typ = 
        if (structOrCls = "struct") then MemberGenerateType.InsideStruct 
          else MemberGenerateType.InsideClass
        
      // Find all constructors
      let ctors = c |> codeDomFlatFilter (fun o -> 
            match o with 
              | :? CodeTypeDeclaration as dc -> (false, dc = c)
              | :? CodeConstructor as c -> (true, true)
              | _ -> (false, true); ) 
      let anyCtor = ctors.Length > 0;
      
      // Find base classes
      let (baseClass, interfaces) = resolveHierarchy c ctx
      
      // Find fields and their types
      let (ft, pt, flds, props, meths, events) = 
          c.Members |> codeDomCallBackNoScope (fun rcall ((ft, pt,flds, props, meths, events) as acc) o -> 
              match o with 
                | :? CodeMemberField as fld -> (Map.add fld.Name fld.Type ft, pt, Map.add fld.Name  fld flds, props, meths, events)
                | :? CodeMemberProperty as prop -> (ft, Map.add prop.Name prop.Type pt, flds, Map.add prop.Name prop props, meths, events)
                | :? CodeMemberMethod as meth -> (ft, pt, flds, props, Map.add meth.Name meth meths, events)
                | :? CodeMemberEvent as ev -> (ft, pt, flds, props, meths, Map.add ev.Name ev events)
                | :? CodeTypeMemberCollection -> rcall acc o
                | _ -> acc; ) (Map.empty, Map.empty, Map.empty, Map.empty, Map.empty, Map.empty)
                          
      // Find all overloads of the method, so we can produce [<OverloadID>]
      let (getOverload, allmeths) = getMethodOverloads(c.Members)
      
      // Get tripple with method info, overload id and name of the interface where
      // it belongs (if it's "PrivateImplementationType")
      let allmeths = allmeths |> List.map ( fun (cm, ovIdx, intrfcName) -> 
        match getOverload cm with | 1 -> (cm, -1, intrfcName) | _ -> (cm, ovIdx, intrfcName) )
        
      // Split between methods of the class
      // and methods that implemnet some interface
      let ifcTable = new Dictionary<string, ResizeArray<CodeMemberMethod*int>>()
      let allmeths = 
        allmeths |> mapFilter (fun (m, idx, ifn) -> 
          match m.PrivateImplementationType, m.ImplementationTypes.Count with
          | null, 0 -> Some((m,idx))
          | _ , 0 -> 
            let b,v = ifcTable.TryGetValue(ifn)
            let v = 
              if (not b) then 
                let rs = new ResizeArray<CodeMemberMethod*int>()
                ifcTable.Add(ifn, rs)
                rs 
              else v
            v.Add((m,idx))
            None
          | null, n -> 
            for implementedInterface in m.ImplementationTypes do
                let b,v = ifcTable.TryGetValue(getTypeRefSimple implementedInterface)
                let v =
                  if (not b) then
                    let rs = new ResizeArray<CodeMemberMethod*int>()
                    ifcTable.Add(getTypeRefSimple implementedInterface, rs)
                    rs
                  else v
                v.Add((m, idx))
            Some((m,idx))
          | _, _ -> failwith "CodeMethodMember must not have both ImplementationTypes and PrivateImplementationType set.")


      // NOTE: we ignore class visibility and also IsPartial property
      // Declare type arguments and generate class 
      let tyargs, genTyArgs = processTypeArgs c.TypeParameters       
      (usingTyParams tyargs 
        (id  
        +> (fun ctx -> { ctx with BaseTypes = (baseClass, interfaces); FieldTypes = ft; PropertyTypes = pt; AllFields=flds; AllProps=props; AllMeths=meths; AllEvents=events  })   
        ++ ""    
        ++ (if c.IsPartial then "(* partial *)" else "")    
        +> col sepNone scope (fun s -> id -- s -- "_") -- c.Name 
        +> genTyArgs
        -- " = " -- structOrCls
        +> incIndent
        +> match (baseClass) with
             | Some (bc) -> id ++ "inherit " +> generateTypeRef bc -- " "
             | _ -> id
        
        // Filter and generate members
        +> colFilterT<CodeMemberEvent>     sepNln c.Members generateEventField
        +> colFilter<CodeMemberField>     sepNln c.Members (generateClassMember typ)
        +> colFilter<CodeTypeConstructor> sepNln c.Members (generateClassMember typ)
        +> colFilter<CodeMemberEvent>     sepNln c.Members (generateClassMember typ)
        
        // Generate default empty constructor for classes 
        // without constructors (but not for structs!)
        +> if (anyCtor) then
             colFilter<CodeConstructor> sepNln c.Members (generateClassMember typ)
           elif (structOrCls = "class" && not c.IsPartial) then
             generateConstructor null
           else
            id 
            
        // User code
        +> colFilterT<CodeSnippetTypeMember> sepNln c.Members generateCodeSnippetMember
        // Properties, methods, interface implementations
        +> colFilter<CodeMemberProperty> sepNln c.Members (generateClassMember typ)
        +> colT sepNln allmeths (generateClassMemberMethod typ)
        +> colT sepNln ifcTable generateInterfaceImplementation
        +> colFilterT<CodeEntryPointMethod> sepNln c.Members (generateEntryPointMethod typ)
        +> decIndent
        ++ "end")) ctx
        
    let generateInterface (scope:string list) (c:CodeTypeDeclaration) =
      // handle overloads
      let (getOverload, allmeths) = getMethodOverloads c.Members 
      let allmeths = allmeths |> List.map ( fun (cm, ovIdx, _) -> 
        match getOverload cm with | 1 -> (cm, -1) | _ -> (cm, ovIdx) )

      let castToProp (a:CodeTypeMember) = (a :?> CodeMemberProperty)        

      // NOTE: visibility is ignored
      let tyargs, genTyArgs = processTypeArgs c.TypeParameters       
      usingTyParams tyargs 
        (id  
        ++ ""    
        +> col sepNone scope (fun s -> id -- s -- "_") -- c.Name 
        +> genTyArgs
        -- " = interface" 
        +> incIndent
        +> col sepNln c.BaseTypes (fun (cr:CodeTypeReference) -> id ++ "inherit " +> generateTypeRef cr)
        +> colFilter<CodeMemberProperty> sepNln c.Members (castToProp >> generateInterfaceMemberProperty)
        ++ ""
        +> colT sepNln allmeths generateInterfaceMemberMethod
        +> decIndent
        ++ "end")
      
    let generateDelegate (scope:string list) (c:CodeTypeDelegate) =
      let tyargs, genTyArgs = processTypeArgs c.TypeParameters       
      usingTyParams tyargs 
        (id
        ++ ""
        +> col sepNone scope (fun s -> id -- s -- "_") -- c.Name 
        +> genTyArgs
        -- " = delegate of "
        +> if (c.Parameters.Count = 0) then
             id -- "unit"
           else
             col sepStar c.Parameters (fun (p:CodeParameterDeclarationExpression) ->
               id +> generateTypeRef p.Type )
        -- " -> "
        +> match c.ReturnType with 
             | null -> id -- "unit"
             | rt -> generateTypeRef rt)
      
    let generateEnumField (index:int) (c:CodeMemberField) =    
      id 
      ++ "| " -- c.Name -- " = " 
      +> match c.InitExpression with
           | null -> str index
           | :? CodePrimitiveExpression as p -> generatePrimitiveExpr None p
           | _ -> failwith "Invalid enum !";
                     
    let generateEnum (scope:string list) (c:CodeTypeDeclaration) =
      let counter = createCounter()
      id     
      ++ "" 
      +> col sepNone scope (fun s -> id -- s -- "_") -- c.Name 
      -- " =" 
      +> incIndent
      +> col sepNone c.Members (fun c -> generateEnumField (counter()) c)
      +> decIndent
    
    let generateTypeDecl index (scope:string list, c:CodeTypeDeclaration) =      
      id
      ++ if (index = 0) then "type" else "and"
      +> incIndent
      +> col sepNone c.Comments generateStatement 
      +> generateCustomAttrDeclsForType (c.CustomAttributes |> Seq.cast |> Seq.toList) c.TypeAttributes
      +> (fun ctx -> { ctx with CurrentType = c })
      +> match c with 
           | :? CodeTypeDelegate as cd -> generateDelegate scope cd
           | c when c.IsClass -> generateClassOrStruct "class" scope c
           | c when c.IsInterface -> generateInterface scope c
           | c when c.IsEnum -> generateEnum scope c
           | c when c.IsStruct -> generateClassOrStruct "struct" scope c
           | _ -> 
            // NOTE: I believe this is full match..
            id ++ "(* Type '" -- (c.Name) --  "' is not supported by the CodeDOM provider and was omitted. *)"
      +> decIndent
      +> (fun ctx -> { ctx with DeclaredEvents = []; CurrentType = null; BaseTypes = (None, []); FieldTypes = Map.empty; PropertyTypes = Map.empty; })
      
    /// Generates a main method.
    let generateMainMethod (c:CodeEntryPointMethod, t:CodeTypeDeclaration) (ns:CodeNamespace) =
      let retType = getTypeRefSimple c.ReturnType 
      let custAttrs = 
        CodeAttributeDeclaration("EntryPoint", [||]) :: (c.CustomAttributes |> Seq.cast |> Seq.toList)
      
      if ((c.Parameters.Count = 0) || (c.Parameters.Count = 1 && (getTypeRefSimple c.Parameters.[0].Type) = "string[]" ))
         && (retType = "int" || retType = "unit")
      then
        id
        ++ "[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]"
        ++ "module __EntryPoint ="
        +> incIndent
        +>   (generateCustomAttrDeclsList custAttrs)  
        ++   "let Main (args:string[]) ="
        +> incIndent
        // REVIEW: Do we need to pass this through the "rename" table?  Could use '(generateTypeRef t)', but we don't have a CodeTypeReference
        ++ t.Name -- "." -- (c.Name) 
        +> if c.Parameters.Count = 1 
           then id -- "(args)"
           else id -- "()"
        // F# only supports main methods returning int.  If we're asked to emit one that returns unit, just return 1.
        +> if retType = "unit" then id ++ "0" else id
        +> decIndent 
        +> decIndent
      else
        id ++ "(* Could not generate entry point for method '" -- (c.Name) -- "'. *)"
      
    //---------------------------------------------------------------------------------------------
    // Namespaces and compilation units
    
    /// Returns CodeNamespace, list of classes with scope (which includes class names 
    /// of containing classes and sequence of class renames)
    let preprocessNamespace (c:CodeNamespace) =
        
        // Extract flat class structure
        let flatClasses = 
            c 
            |> codeDomCallbackWithScope (fun rcall scope acc o -> 
                  match o with 
                    | :? CodeTypeDeclaration as dc -> 
                        //sprintf "preprocessNamespace: rcall for type c.Name = %s\n" dc.Name |> System.Windows.Forms.MessageBox.Show |> ignore
                        rcall (dc.Name::scope) ((scope, dc)::acc) (box dc.Members)
                    | _ -> rcall scope acc o) [] [];
        let flatClasses = flatClasses |> List.rev
        
        // Get all renamed classes - this changes file structure, but at least it works
        let addNameWithScope n (scope:string list) acc = 
            let scn = String.Join("_",Array.ofList scope) + "_" + n
            let (_, res) = 
                scope |> List.fold ( fun (prefix,st) e ->
                  let npref = e + prefix
                  let nmap = Map.add (npref + n) scn st 
                  ("." + npref, nmap) ) (".", Map.add n scn acc)
            res                

        //sprintf "c.Name = %s, #flatClasses = %d\n" c.Name flatClasses.Length |> System.Windows.Forms.MessageBox.Show |> ignore

        let renames = 
            flatClasses 
            |> List.fold ( fun acc ((scope:string list), ty) ->
                  if (scope.Length = 0) then acc 
                  else addNameWithScope ty.Name scope acc ) Map.empty

        //if (renames |> Seq.length) > 0 then
        //    sprintf "#renames = %d\n" (renames |> Seq.length) |> System.Windows.Forms.MessageBox.Show |> ignore

        (c, flatClasses, renames |> Map.toSeq);        
        
    let generateImport (c:CodeNamespaceImport) = 
      id  ++ "open " -- c.Namespace
      
    /// Generates namespace code - takes output from 'preprocessNamespace'
    let generateNamespaceInternal ((c:CodeNamespace, flatClasses, _), containing) =
      let counter = createCounter()
      let ifcSet = 
        flatClasses 
          |> List.fold (fun st (scope, (c:CodeTypeDeclaration)) -> 
                if (c.IsInterface) then 
                  let st = Set.add c.Name st 
                  Set.add (String.Join(".",Array.ofList(scope@[c.Name]))) st
                else st) Set.empty
      
      id
      +> ( fun ctx -> { ctx with CurrentNamespace = c.Name; DeclaredInterfaces = ifcSet } )
      +> col sepNone c.Comments generateStatement
      +> ((if String.IsNullOrEmpty c.Name then id ++ "namespace global" else id ++ "namespace " -! c.Name) +> incIndent) 
      ++ "// Generated by F# CodeDom"
      ++ "#nowarn \"49\" // uppercase argument names"
      ++ "#nowarn \"67\" // this type test or downcast will always hold"
      ++ "#nowarn \"66\" // this upcast is unnecessary - the types are identical"
      ++ "#nowarn \"58\" // possible incorrect indentation.." // (when using CodeSnippets ie. in ASP.NET)
      ++ "#nowarn \"57\" // do not use create_DelegateEvent"
      ++ "#nowarn \"51\" // address-of operator can occur in the code"
      ++ "#nowarn \"1183\" // unused 'this' reference"
      +> colT sepNone containing (fun s -> id ++ "open " -- s)
      +> col sepNone c.Imports generateImport
      ++ ""              
      ++ "exception ReturnException" +> uniqid -- " of obj"
      ++ "exception ReturnNoneException" +> uniqid 

      ++ "[<AutoOpen>]"
      ++ "module FuncConvertFinalOverload" +> uniqid -- " ="
      ++ "  // This extension member adds to the FuncConvert type and is the last resort member in the method overloading rules. "
      ++ "  type global.Microsoft.FSharp.Core.FuncConvert with"
      ++ "      /// A utility function to convert function values from tupled to curried form"
      ++ "      static member FuncFromTupled (f:'T -> 'Res) = f"
      ++ ""
      +> colT sepNln flatClasses (fun c -> generateTypeDecl (counter()) c)
      +> withCtxt (fun ctx -> match ctx.MainMethodForCurrentNamespace with None -> id | Some mainMethod -> (generateMainMethod mainMethod c))
      +> if (c.Name<>null && c.Name.Length>0) then decIndent else id
      +> ( fun ctx -> { ctx with CurrentNamespace = ""; MainMethodForCurrentNamespace = None } )
    
    
    /// Generate code for namespace without compilation unit  
    let generateNamespace (c:CodeNamespace) = 
        generateNamespaceInternal ((preprocessNamespace c), [])
        
    /// Generate code for type declaration (not included in namespace)                 
    let generateTypeDeclOnly (c:CodeTypeDeclaration) =
        let ns = new CodeNamespace()
        ns.Types.Add(c) |> ignore
        let ((_, flatClasses, _), containing) = (preprocessNamespace ns, [])
        let counter = createCounter()
        id
        ++ ""              
        ++ "exception ReturnException" +> uniqid -- " of obj"
        ++ "exception ReturnNoneException" +> uniqid 
        ++ ""
        +> colT sepNln flatClasses (fun c -> generateTypeDecl (counter()) c)

    /// Generate code for compile unit (file)                
    let generateCompileUnit (c:CodeCompileUnit) (preprocHacks:CodeCompileUnit -> unit) =
      
      // Generate code for the compilation unit
      preprocHacks c;
      match c with 
        | :? CodeSnippetCompileUnit as cs -> 
          id +> generateLinePragma cs.LinePragma ++ cs.Value
        | _ -> 
          let preprocNs = c.Namespaces |> Seq.cast |> Seq.map preprocessNamespace
          let renames = preprocNs |> Seq.collect (fun (_, _, renames) -> renames) 
          let getContainingNamespaces (c:CodeNamespace) nslist =
            nslist |> List.filter ( fun (n:string) -> c.Name.StartsWith(n) )
          let (namespacesWithPrev, _) = 
            preprocNs |> Seq.fold (fun (res, tmpNames) (c, cls, renames) ->
              (((c, cls, renames), getContainingNamespaces c tmpNames)::res, c.Name::tmpNames) ) ([], [])
          let namespacesWithPrev = namespacesWithPrev |> Seq.toList |> List.rev

          // renames |> Seq.map (fun (s, t) -> sprintf "%s --> %s\n" s t) |> Seq.toList |> String.concat "\n" |> System.Windows.Forms.MessageBox.Show |> ignore
          
          (fun ctx -> { ctx with TypeRenames = Map.ofSeq renames; } )        
          ++ "//------------------------------------------------------------------------------"
          ++ "// <autogenerated>"
          ++ "//     This code was generated by a tool."
          ++ "//     Runtime Version: " +> (str System.Environment.Version)
          ++ "//"
          ++ "//     Changes to this file may cause incorrect behavior and will be lost if "
          ++ "//     the code is regenerated."
          ++ "// </autogenerated>"
          ++ "//------------------------------------------------------------------------------"
          ++ ""
          +> colT sepNln namespacesWithPrev generateNamespaceInternal;

    //---------------------------------------------------------------------------------------------
