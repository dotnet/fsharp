module internal CheckIncrementalClasses

open Internal.Utilities.Collections
open FSharp.Compiler.CheckExpressions
open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.Syntax
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.Xml

exception ParameterlessStructCtor of range: range

/// Typechecked info for implicit constructor and it's arguments 
type IncrClassCtorLhs = 
    {
        /// The TyconRef for the type being defined
        TyconRef: TyconRef

        /// The type parameters allocated for the implicit instance constructor. 
        /// These may be equated with other (WillBeRigid) type parameters through equi-recursive inference, and so 
        /// should always be renormalized/canonicalized when used.
        InstanceCtorDeclaredTypars: Typars     

        /// The value representing the static implicit constructor.
        /// Lazy to ensure the static ctor value is only published if needed.
        StaticCtorValInfo: Lazy<Val list * Val * ValScheme>

        /// The value representing the implicit constructor.
        InstanceCtorVal: Val

        /// The type of the implicit constructor, representing as a ValScheme.
        InstanceCtorValScheme: ValScheme
        
        /// The values representing the arguments to the implicit constructor.
        InstanceCtorArgs: Val list

        /// The reference cell holding the 'this' parameter within the implicit constructor so it can be referenced in the
        /// arguments passed to the base constructor
        InstanceCtorSafeThisValOpt: Val option

        /// Data indicating if safe-initialization checks need to be inserted for this type.
        InstanceCtorSafeInitInfo: SafeInitData

        /// The value representing the 'base' variable within the implicit instance constructor.
        InstanceCtorBaseValOpt: Val option

        /// The value representing the 'this' variable within the implicit instance constructor.
        InstanceCtorThisVal: Val

        /// The name generator used to generate the names of fields etc. within the type.
        NameGenerator: NiceNameGenerator
    }

/// Indicates how is a 'let' bound value in a class with implicit construction is represented in
/// the TAST ultimately produced by type checking.    
type IncrClassValRepr = 

    // e.g representation for 'let v = 3' if it is not used in anything given a method representation
    | InVar of isArg: bool 

    // e.g representation for 'let v = 3'
    | InField of isStatic: bool * staticCountForSafeInit: int * fieldRef: RecdFieldRef

    // e.g representation for 'let f x = 3'
    | InMethod of isStatic:bool * value: Val * valReprInfo: ValReprInfo

/// IncrClassReprInfo represents the decisions we make about the representation of 'let' and 'do' bindings in a
/// type defined with implicit class construction.
type IncrClassReprInfo = 
    { 
        /// Indicates the set of field names taken within one incremental class
        TakenFieldNames: Set<string>
          
        RepInfoTcGlobals: TcGlobals
          
        /// vals mapped to representations
        ValReprs: Zmap<Val, IncrClassValRepr> 
          
        /// vals represented as fields or members from this point on 
        ValsWithRepresentation: Zset<Val> 
    }

    static member IsMethodRepr: cenv: TcFileState -> bind: Binding -> bool

    // Publish the fields of the representation to the type 
    member PublishIncrClassFields: 
        cenv : TcFileState * 
        denv : DisplayEnv * 
        cpath : CompilationPath * 
        ctorInfo: IncrClassCtorLhs * 
        safeStaticInitInfo : SafeInitData 
          -> unit    

    /// Given localRep saying how locals have been represented, e.g. as fields.
    /// Given an expr under a given thisVal context.
    member FixupIncrClassExprPhase2C: 
        cenv : TcFileState -> 
        thisValOpt : Val option -> 
        safeStaticInitInfo : SafeInitData -> 
        thisTyInst: TypeInst -> 
        expr : Expr -> 
        Expr 

/// Represents a single group of bindings in a class with an implicit constructor
type IncrClassBindingGroup = 
    | IncrClassBindingGroup of bindings: Binding list * isStatic: bool* isRecursive: bool
    | IncrClassDo of expr: Expr * isStatic: bool * range: Range 

type IncrClassConstructionBindingsPhase2C =
    | Phase2CBindings of IncrClassBindingGroup list
    | Phase2CCtorJustAfterSuperInit     
    | Phase2CCtorJustAfterLastLet

/// Check and elaborate the "left hand side" of the implicit class construction 
/// syntax.
val TcImplicitCtorLhs_Phase2A: 
    cenv: TcFileState * 
    env: TcEnv * 
    tpenv: UnscopedTyparEnv * 
    tcref: TyconRef * 
    vis: SynAccess option * 
    attrs : SynAttribute list * 
    spats : SynSimplePat list * 
    thisIdOpt : Ident option * 
    baseValOpt: Val option * 
    safeInitInfo : SafeInitData * 
    m : range * 
    copyOfTyconTypars : Typar list * 
    objTy : TType * 
    thisTy : TType * 
    xmlDoc: PreXmlDoc 
      -> IncrClassCtorLhs

/// <summary>
/// Given a set of 'let' bindings (static or not, recursive or not) that make up a class,
/// generate their initialization expression(s).
/// </summary>
/// <param name='cenv'></param>
/// <param name='env'></param>
/// <param name='ctorInfo'>The lhs information about the implicit constructor</param>
/// <param name='inheritsExpr'>The call to the super class constructor</param>
/// <param name='inheritsIsVisible'>Should we place a sequence point at the 'inheritedTys call?</param>
/// <param name='decs'>The declarations</param>
/// <param name='memberBinds'></param>
/// <param name='generalizedTyparsForRecursiveBlock'>Record any unconstrained type parameters generalized for the outer members as "free choices" in the let bindings</param>
/// <param name='safeStaticInitInfo'></param>
val MakeCtorForIncrClassConstructionPhase2C:
    cenv: TcFileState *
    env: TcEnv *
    ctorInfo: IncrClassCtorLhs *
    inheritsExpr : Expr *
    inheritsIsVisible : bool *
    decs: IncrClassConstructionBindingsPhase2C list *
    memberBinds: Binding list *
    generalizedTyparsForRecursiveBlock : Typar list *
    safeStaticInitInfo: SafeInitData -> Expr * Expr option * Binding list * IncrClassReprInfo
