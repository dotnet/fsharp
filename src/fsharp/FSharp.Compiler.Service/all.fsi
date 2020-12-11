namespace FSharp.Compiler
  module internal AccessibilityLogic =
    [<NoEquality; NoComparison>]
    type AccessorDomain =
      | AccessibleFrom of
        TypedTree.CompilationPath list * TypedTree.TyconRef option
      | AccessibleFromEverywhere
      | AccessibleFromSomeFSharpCode
      | AccessibleFromSomewhere
      with
        static member
          CustomEquals: g:TcGlobals.TcGlobals * ad1:AccessorDomain *
                         ad2:AccessorDomain -> bool
        static member CustomGetHashCode: ad:AccessorDomain -> int
    
    val IsAccessible:
      ad:AccessorDomain -> taccess:TypedTree.Accessibility -> bool
    val private IsILMemberAccessible:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          m:Range.range ->
            tcrefOfViewedItem:TypedTree.TyconRef ->
              ad:AccessorDomain -> access:AbstractIL.IL.ILMemberAccess -> bool
    val private IsILTypeDefAccessible:
      amap:Import.ImportMap ->
        m:Range.range ->
          ad:AccessorDomain ->
            encTyconRefOpt:TypedTree.TyconRef option ->
              tdef:AbstractIL.IL.ILTypeDef -> bool
    val private IsTyconAccessibleViaVisibleTo:
      ad:AccessorDomain -> tcrefOfViewedItem:TypedTree.TyconRef -> bool
    val private IsILTypeInfoAccessible:
      amap:Import.ImportMap ->
        m:Range.range ->
          ad:AccessorDomain -> tcrefOfViewedItem:TypedTree.TyconRef -> bool
    val private IsILTypeAndMemberAccessible:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          m:Range.range ->
            adType:AccessorDomain ->
              ad:AccessorDomain ->
                ty:Infos.ILTypeInfo ->
                  access:AbstractIL.IL.ILMemberAccess -> bool
    val IsEntityAccessible:
      amap:Import.ImportMap ->
        m:Range.range -> ad:AccessorDomain -> tcref:TypedTree.TyconRef -> bool
    val CheckTyconAccessible:
      amap:Import.ImportMap ->
        m:Range.range -> ad:AccessorDomain -> tcref:TypedTree.TyconRef -> bool
    val IsTyconReprAccessible:
      amap:Import.ImportMap ->
        m:Range.range -> ad:AccessorDomain -> tcref:TypedTree.TyconRef -> bool
    val CheckTyconReprAccessible:
      amap:Import.ImportMap ->
        m:Range.range -> ad:AccessorDomain -> tcref:TypedTree.TyconRef -> bool
    val IsTypeAccessible:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          m:Range.range -> ad:AccessorDomain -> ty:TypedTree.TType -> bool
    val IsTypeInstAccessible:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          m:Range.range -> ad:AccessorDomain -> tinst:TypedTree.TypeInst -> bool
    val IsProvidedMemberAccessible:
      amap:Import.ImportMap ->
        m:Range.range ->
          ad:AccessorDomain ->
            ty:TypedTree.TType -> access:AbstractIL.IL.ILMemberAccess -> bool
    val ComputeILAccess:
      isPublic:bool ->
        isFamily:bool ->
          isFamilyOrAssembly:bool ->
            isFamilyAndAssembly:bool -> AbstractIL.IL.ILMemberAccess
    val IsILFieldInfoAccessible:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          m:Range.range -> ad:AccessorDomain -> x:Infos.ILFieldInfo -> bool
    val GetILAccessOfILEventInfo:
      Infos.ILEventInfo -> AbstractIL.IL.ILMemberAccess
    val IsILEventInfoAccessible:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          m:Range.range -> ad:AccessorDomain -> einfo:Infos.ILEventInfo -> bool
    val private IsILMethInfoAccessible:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          m:Range.range ->
            adType:AccessorDomain ->
              ad:AccessorDomain -> ilminfo:Infos.ILMethInfo -> bool
    val GetILAccessOfILPropInfo:
      Infos.ILPropInfo -> AbstractIL.IL.ILMemberAccess
    val IsILPropInfoAccessible:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          m:Range.range -> ad:AccessorDomain -> pinfo:Infos.ILPropInfo -> bool
    val IsValAccessible: ad:AccessorDomain -> vref:TypedTree.ValRef -> bool
    val CheckValAccessible:
      m:Range.range -> ad:AccessorDomain -> vref:TypedTree.ValRef -> unit
    val IsUnionCaseAccessible:
      amap:Import.ImportMap ->
        m:Range.range ->
          ad:AccessorDomain -> ucref:TypedTree.UnionCaseRef -> bool
    val CheckUnionCaseAccessible:
      amap:Import.ImportMap ->
        m:Range.range ->
          ad:AccessorDomain -> ucref:TypedTree.UnionCaseRef -> bool
    val IsRecdFieldAccessible:
      amap:Import.ImportMap ->
        m:Range.range ->
          ad:AccessorDomain -> rfref:TypedTree.RecdFieldRef -> bool
    val CheckRecdFieldAccessible:
      amap:Import.ImportMap ->
        m:Range.range ->
          ad:AccessorDomain -> rfref:TypedTree.RecdFieldRef -> bool
    val CheckRecdFieldInfoAccessible:
      amap:Import.ImportMap ->
        m:Range.range -> ad:AccessorDomain -> rfinfo:Infos.RecdFieldInfo -> unit
    val CheckILFieldInfoAccessible:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          m:Range.range -> ad:AccessorDomain -> finfo:Infos.ILFieldInfo -> unit
    val IsTypeAndMethInfoAccessible:
      amap:Import.ImportMap ->
        m:Range.range ->
          accessDomainTy:AccessorDomain ->
            ad:AccessorDomain -> _arg1:Infos.MethInfo -> bool
    val IsMethInfoAccessible:
      amap:Import.ImportMap ->
        m:Range.range -> ad:AccessorDomain -> minfo:Infos.MethInfo -> bool
    val IsPropInfoAccessible:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          m:Range.range -> ad:AccessorDomain -> _arg1:Infos.PropInfo -> bool
    val IsFieldInfoAccessible:
      ad:AccessorDomain -> rfref:Infos.RecdFieldInfo -> bool


namespace FSharp.Compiler
  module internal AttributeChecking =
    exception ObsoleteWarning of string * Range.range
    exception ObsoleteError of string * Range.range
    val fail: unit -> 'a
    val private evalILAttribElem: e:AbstractIL.IL.ILAttribElem -> obj
    val private evalFSharpAttribArg:
      g:TcGlobals.TcGlobals -> e:TypedTree.Expr -> obj
    type AttribInfo =
      | FSAttribInfo of TcGlobals.TcGlobals * TypedTree.Attrib
      | ILAttribInfo of
        TcGlobals.TcGlobals * Import.ImportMap * AbstractIL.IL.ILScopeRef *
        AbstractIL.IL.ILAttribute * Range.range
      with
        member ConstructorArguments: (TypedTree.TType * obj) list
        member NamedArguments: (TypedTree.TType * string * bool * obj) list
        member Range: Range.range
        member TyconRef: TypedTree.TyconRef
    
    val AttribInfosOfIL:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          scoref:AbstractIL.IL.ILScopeRef ->
            m:Range.range ->
              attribs:AbstractIL.IL.ILAttributes -> AttribInfo list
    val AttribInfosOfFS:
      g:TcGlobals.TcGlobals -> attribs:TypedTree.Attrib list -> AttribInfo list
    val GetAttribInfosOfEntity:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          m:Range.range -> tcref:TypedTree.TyconRef -> AttribInfo list
    val GetAttribInfosOfMethod:
      amap:Import.ImportMap ->
        m:Range.range -> minfo:Infos.MethInfo -> AttribInfo list
    val GetAttribInfosOfProp:
      amap:Import.ImportMap ->
        m:Range.range -> pinfo:Infos.PropInfo -> AttribInfo list
    val GetAttribInfosOfEvent:
      amap:Import.ImportMap ->
        m:Range.range -> einfo:Infos.EventInfo -> AttribInfo list
    val TryBindTyconRefAttribute:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          TcGlobals.BuiltinAttribInfo ->
            tcref:TypedTree.TyconRef ->
              f1:(AbstractIL.IL.ILAttribElem list *
                  AbstractIL.IL.ILAttributeNamedArg list -> 'a option) ->
                f2:(TypedTree.Attrib -> 'a option) ->
                  f3:(obj option list * (string * obj option) list -> 'a option) ->
                    'a option
    val BindMethInfoAttributes:
      m:Range.range ->
        minfo:Infos.MethInfo ->
          f1:(AbstractIL.IL.ILAttributes -> 'a) ->
            f2:(TypedTree.Attrib list -> 'a) ->
              f3:(Tainted<ExtensionTyping.IProvidedCustomAttributeProvider> ->
                    'a) -> 'a
    val TryBindMethInfoAttribute:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          TcGlobals.BuiltinAttribInfo ->
            minfo:Infos.MethInfo ->
              f1:(AbstractIL.IL.ILAttribElem list *
                  AbstractIL.IL.ILAttributeNamedArg list -> 'a option) ->
                f2:(TypedTree.Attrib -> 'a option) ->
                  f3:(obj option list * (string * obj option) list -> 'a option) ->
                    'a option
    val TryFindMethInfoStringAttribute:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          attribSpec:TcGlobals.BuiltinAttribInfo ->
            minfo:Infos.MethInfo -> string option
    val MethInfoHasAttribute:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          attribSpec:TcGlobals.BuiltinAttribInfo -> minfo:Infos.MethInfo -> bool
    val private CheckILAttributes:
      g:TcGlobals.TcGlobals ->
        isByrefLikeTyconRef:bool ->
          cattrs:AbstractIL.IL.ILAttributes ->
            m:Range.range -> ErrorLogger.OperationResult<unit>
    val langVersionPrefix: string
    val CheckFSharpAttributes:
      g:TcGlobals.TcGlobals ->
        attribs:TypedTree.Attrib list ->
          m:Range.range -> ErrorLogger.OperationResult<unit>
    val private CheckProvidedAttributes:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          provAttribs:Tainted<ExtensionTyping.IProvidedCustomAttributeProvider> ->
            ErrorLogger.OperationResult<unit>
    val CheckILAttributesForUnseen:
      g:TcGlobals.TcGlobals ->
        cattrs:AbstractIL.IL.ILAttributes -> _m:'a -> bool
    val CheckFSharpAttributesForHidden:
      g:TcGlobals.TcGlobals -> attribs:TypedTree.Attrib list -> bool
    val CheckFSharpAttributesForObsolete:
      g:TcGlobals.TcGlobals -> attribs:TypedTree.Attrib list -> bool
    val CheckFSharpAttributesForUnseen:
      g:TcGlobals.TcGlobals -> attribs:TypedTree.Attrib list -> _m:'a -> bool
    val CheckProvidedAttributesForUnseen:
      provAttribs:Tainted<ExtensionTyping.IProvidedCustomAttributeProvider> ->
        m:Range.range -> bool
    val CheckPropInfoAttributes:
      pinfo:Infos.PropInfo -> m:Range.range -> ErrorLogger.OperationResult<unit>
    val CheckILFieldAttributes:
      g:TcGlobals.TcGlobals -> finfo:Infos.ILFieldInfo -> m:Range.range -> unit
    val CheckMethInfoAttributes:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          tyargsOpt:'a option ->
            minfo:Infos.MethInfo -> ErrorLogger.OperationResult<unit>
    val MethInfoIsUnseen:
      g:TcGlobals.TcGlobals ->
        m:Range.range -> ty:TypedTree.TType -> minfo:Infos.MethInfo -> bool
    val PropInfoIsUnseen: m:'a -> pinfo:Infos.PropInfo -> bool
    val CheckEntityAttributes:
      g:TcGlobals.TcGlobals ->
        x:TypedTree.TyconRef ->
          m:Range.range -> ErrorLogger.OperationResult<unit>
    val CheckUnionCaseAttributes:
      g:TcGlobals.TcGlobals ->
        x:TypedTree.UnionCaseRef ->
          m:Range.range -> ErrorLogger.OperationResult<unit>
    val CheckRecdFieldAttributes:
      g:TcGlobals.TcGlobals ->
        x:TypedTree.RecdFieldRef ->
          m:Range.range -> ErrorLogger.OperationResult<unit>
    val CheckValAttributes:
      g:TcGlobals.TcGlobals ->
        x:TypedTree.ValRef -> m:Range.range -> ErrorLogger.OperationResult<unit>
    val CheckRecdFieldInfoAttributes:
      g:TcGlobals.TcGlobals ->
        x:Infos.RecdFieldInfo ->
          m:Range.range -> ErrorLogger.OperationResult<unit>
    val IsSecurityAttribute:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          casmap:System.Collections.Generic.Dictionary<TypedTree.Stamp,bool> ->
            TypedTree.Attrib -> m:Range.range -> bool
    val IsSecurityCriticalAttribute:
      g:TcGlobals.TcGlobals -> TypedTree.Attrib -> bool


namespace FSharp.Compiler
  module internal TypeRelations =
    val TypeDefinitelySubsumesTypeNoCoercion:
      ndeep:int ->
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap ->
            m:Range.range -> ty1:TypedTree.TType -> ty2:TypedTree.TType -> bool
    type CanCoerce =
      | CanCoerce
      | NoCoerce
    val TypesFeasiblyEquivalent:
      stripMeasures:bool ->
        ndeep:int ->
          g:TcGlobals.TcGlobals ->
            amap:'a ->
              m:Range.range ->
                ty1:TypedTree.TType -> ty2:TypedTree.TType -> bool
    val TypesFeasiblyEquiv:
      ndeep:int ->
        g:TcGlobals.TcGlobals ->
          amap:'a ->
            m:Range.range -> ty1:TypedTree.TType -> ty2:TypedTree.TType -> bool
    val TypesFeasiblyEquivStripMeasures:
      g:TcGlobals.TcGlobals ->
        amap:'a ->
          m:Range.range -> ty1:TypedTree.TType -> ty2:TypedTree.TType -> bool
    val TypeFeasiblySubsumesType:
      ndeep:int ->
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap ->
            m:Range.range ->
              ty1:TypedTree.TType ->
                canCoerce:CanCoerce -> ty2:TypedTree.TType -> bool
    val ChooseTyparSolutionAndRange:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          tp:TypedTree.Typar -> TypedTree.TType * Range.range
    val ChooseTyparSolution:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap -> tp:TypedTree.Typar -> TypedTree.TType
    val IterativelySubstituteTyparSolutions:
      g:TcGlobals.TcGlobals ->
        tps:TypedTree.Typars -> solutions:TypedTree.TTypes -> TypedTree.TypeInst
    val ChooseTyparSolutionsForFreeChoiceTypars:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap -> e:TypedTree.Expr -> TypedTree.Expr
    val tryDestTopLambda:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          TypedTree.ValReprInfo ->
            e:TypedTree.Expr * ty:TypedTree.TType ->
              (TypedTree.Typars * TypedTree.Val option * TypedTree.Val option *
               TypedTree.Val list list * TypedTree.Expr * TypedTree.TType) option
    val destTopLambda:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          topValInfo:TypedTree.ValReprInfo ->
            e:TypedTree.Expr * ty:TypedTree.TType ->
              TypedTree.Typars * TypedTree.Val option * TypedTree.Val option *
              TypedTree.Val list list * TypedTree.Expr * TypedTree.TType
    val IteratedAdjustArityOfLambdaBody:
      g:TcGlobals.TcGlobals ->
        arities:int list ->
          vsl:TypedTree.Val list list ->
            body:TypedTree.Expr -> TypedTree.Val list list * TypedTree.Expr
    val IteratedAdjustArityOfLambda:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          topValInfo:TypedTree.ValReprInfo ->
            e:TypedTree.Expr ->
              TypedTree.Typars * TypedTree.Val option * TypedTree.Val option *
              TypedTree.Val list list * TypedTree.Expr * TypedTree.TType
    val FindUniqueFeasibleSupertype:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          m:Range.range ->
            ty1:TypedTree.TType -> ty2:TypedTree.TType -> TypedTree.TType option


namespace FSharp.Compiler
  module internal InfoReader =
    val SelectImmediateMemberVals:
      g:TcGlobals.TcGlobals ->
        optFilter:string option ->
          f:(TypedTree.ValMemberInfo -> TypedTree.ValRef -> 'a option) ->
            tcref:TypedTree.TyconRef -> 'a list
    val private checkFilter: optFilter:string option -> nm:string -> bool
    val TrySelectMemberVal:
      g:TcGlobals.TcGlobals ->
        optFilter:string option ->
          ty:TypedTree.TType ->
            pri:Infos.ExtensionMethodPriority option ->
              _membInfo:'a -> vref:TypedTree.ValRef -> Infos.MethInfo option
    val GetImmediateIntrinsicMethInfosOfTypeAux:
      optFilter:string option * ad:AccessibilityLogic.AccessorDomain ->
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap ->
            m:Range.range ->
              origTy:TypedTree.TType ->
                metadataTy:TypedTree.TType -> Infos.MethInfo list
    val GetImmediateIntrinsicMethInfosOfType:
      optFilter:string option * ad:AccessibilityLogic.AccessorDomain ->
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap ->
            m:Range.range -> ty:TypedTree.TType -> Infos.MethInfo list
    type PropertyCollector =
  
        new: g:TcGlobals.TcGlobals * amap:Import.ImportMap * m:Range.range *
              ty:TypedTree.TType * optFilter:string option *
              ad:AccessibilityLogic.AccessorDomain -> PropertyCollector
        member Close: unit -> Infos.PropInfo list
        member
          Collect: membInfo:TypedTree.ValMemberInfo * vref:TypedTree.ValRef ->
                      unit
    
    val GetImmediateIntrinsicPropInfosOfTypeAux:
      optFilter:string option * ad:AccessibilityLogic.AccessorDomain ->
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap ->
            m:Range.range ->
              origTy:TypedTree.TType ->
                metadataTy:TypedTree.TType -> Infos.PropInfo list
    val GetImmediateIntrinsicPropInfosOfType:
      optFilter:string option * ad:AccessibilityLogic.AccessorDomain ->
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap ->
            m:Range.range -> ty:TypedTree.TType -> Infos.PropInfo list
    val IsIndexerType:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap -> ty:TypedTree.TType -> bool
    val GetMostSpecificItemsByType:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          f:('a -> (TypedTree.TType * Range.range) option) ->
            xs:'a list -> 'a list
    val GetMostSpecificMethodInfosByMethInfoSig:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          m:Range.range ->
            ty:TypedTree.TType * minfo:Infos.MethInfo ->
              minfos:(TypedTree.TType * Infos.MethInfo) list ->
                (TypedTree.TType * Infos.MethInfo) list
    val FilterMostSpecificMethInfoSets:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          m:Range.range ->
            minfoSets:AbstractIL.Internal.Library.NameMultiMap<TypedTree.TType *
                                                               Infos.MethInfo> ->
              AbstractIL.Internal.Library.NameMultiMap<TypedTree.TType *
                                                       Infos.MethInfo>
    type HierarchyItem =
      | MethodItem of Infos.MethInfo list list
      | PropertyItem of Infos.PropInfo list list
      | RecdFieldItem of Infos.RecdFieldInfo
      | EventItem of Infos.EventInfo list
      | ILFieldItem of Infos.ILFieldInfo list
    type InfoReader =
  
        new: g:TcGlobals.TcGlobals * amap:Import.ImportMap -> InfoReader
        member
          GetEntireTypeHierarchy: allowMultiIntfInst:Infos.AllowMultiIntfInstantiations *
                                   m:Range.range * ty:TypedTree.TType ->
                                     TypedTree.TType list
        member
          GetEventInfosOfType: optFilter:string option *
                                ad:AccessibilityLogic.AccessorDomain *
                                m:Range.range * ty:TypedTree.TType ->
                                  Infos.EventInfo list
        member
          GetILFieldInfosOfType: optFilter:string option *
                                  ad:AccessibilityLogic.AccessorDomain *
                                  m:Range.range * ty:TypedTree.TType ->
                                    Infos.ILFieldInfo list
        member
          GetImmediateIntrinsicEventsOfType: optFilter:string option *
                                              ad:AccessibilityLogic.AccessorDomain *
                                              m:Range.range * ty:TypedTree.TType ->
                                                Infos.EventInfo list
        member
          GetIntrinsicMostSpecificOverrideMethodSetsOfType: optFilter:string option *
                                                             ad:AccessibilityLogic.AccessorDomain *
                                                             allowMultiIntfInst:Infos.AllowMultiIntfInstantiations *
                                                             m:Range.range *
                                                             ty:TypedTree.TType ->
                                                               AbstractIL.Internal.Library.NameMultiMap<TypedTree.TType *
                                                                                                        Infos.MethInfo>
        member
          GetPrimaryTypeHierarchy: allowMultiIntfInst:Infos.AllowMultiIntfInstantiations *
                                    m:Range.range * ty:TypedTree.TType ->
                                      TypedTree.TType list
        member
          GetRawIntrinsicMethodSetsOfType: optFilter:string option *
                                            ad:AccessibilityLogic.AccessorDomain *
                                            allowMultiIntfInst:Infos.AllowMultiIntfInstantiations *
                                            m:Range.range * ty:TypedTree.TType ->
                                              Infos.MethInfo list list
        member
          GetRawIntrinsicPropertySetsOfType: optFilter:string option *
                                              ad:AccessibilityLogic.AccessorDomain *
                                              allowMultiIntfInst:Infos.AllowMultiIntfInstantiations *
                                              m:Range.range * ty:TypedTree.TType ->
                                                Infos.PropInfo list list
        member
          GetRecordOrClassFieldsOfType: optFilter:string option *
                                         ad:AccessibilityLogic.AccessorDomain *
                                         m:Range.range * ty:TypedTree.TType ->
                                           Infos.RecdFieldInfo list
        member
          IsLanguageFeatureRuntimeSupported: langFeature:Features.LanguageFeature ->
                                                bool
        member
          TryFindNamedItemOfType: nm:string *
                                   ad:AccessibilityLogic.AccessorDomain *
                                   m:Range.range * ty:TypedTree.TType ->
                                     HierarchyItem option
        member
          TryFindRecdOrClassFieldInfoOfType: nm:string * m:Range.range *
                                              ty:TypedTree.TType ->
                                                Infos.RecdFieldInfo voption
        member amap: Import.ImportMap
        member g: TcGlobals.TcGlobals
    
    val private tryLanguageFeatureRuntimeErrorAux:
      infoReader:InfoReader ->
        langFeature:Features.LanguageFeature ->
          m:Range.range -> error:(exn -> unit) -> bool
    val checkLanguageFeatureRuntimeError:
      infoReader:InfoReader ->
        langFeature:Features.LanguageFeature -> m:Range.range -> unit
    val checkLanguageFeatureRuntimeErrorRecover:
      infoReader:InfoReader ->
        langFeature:Features.LanguageFeature -> m:Range.range -> unit
    val tryLanguageFeatureRuntimeErrorRecover:
      infoReader:InfoReader ->
        langFeature:Features.LanguageFeature -> m:Range.range -> bool
    val GetIntrinsicConstructorInfosOfTypeAux:
      infoReader:InfoReader ->
        m:Range.range ->
          origTy:TypedTree.TType ->
            metadataTy:TypedTree.TType -> Infos.MethInfo list
    val GetIntrinsicConstructorInfosOfType:
      infoReader:InfoReader ->
        m:Range.range -> ty:TypedTree.TType -> Infos.MethInfo list
    type FindMemberFlag =
      | IgnoreOverrides
      | PreferOverrides
    type private IndexedList<'T> =
  
        new: itemLists:'T list list *
              itemsByName:AbstractIL.Internal.Library.NameMultiMap<'T> ->
                IndexedList<'T>
        member AddItems: items:'T list * nmf:('T -> string) -> IndexedList<'T>
        member
          FilterNewItems: keepTest:('a -> 'T -> bool) ->
                             nmf:('a -> string) -> itemsToAdd:'a list -> 'a list
        member ItemsWithName: nm:string -> 'T list
        member Items: 'T list list
        static member Empty: IndexedList<'T>
    
    val private FilterItemsInSubTypesBasedOnItemsInSuperTypes:
      nmf:('a -> string) ->
        keepTest:('a -> 'a -> bool) -> itemLists:'a list list -> 'a list list
    val private FilterItemsInSuperTypesBasedOnItemsInSubTypes:
      nmf:('a -> string) ->
        keepTest:('a -> 'a list -> bool) ->
          itemLists:'a list list -> 'a list list
    val private ExcludeItemsInSuperTypesBasedOnEquivTestWithItemsInSubTypes:
      nmf:('a -> string) ->
        equivTest:('a -> 'a -> bool) -> itemLists:'a list list -> 'a list list
    val private FilterOverrides:
      findFlag:FindMemberFlag ->
        isVirt:('a -> bool) * isNewSlot:('a -> bool) *
        isDefiniteOverride:('a -> bool) * isFinal:('a -> bool) *
        equivSigs:('a -> 'a -> bool) * nmf:('a -> string) ->
          items:'a list list -> 'a list list
    val private FilterOverridesOfMethInfos:
      findFlag:FindMemberFlag ->
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap ->
            m:Range.range ->
              minfos:Infos.MethInfo list list -> Infos.MethInfo list list
    val private FilterOverridesOfPropInfos:
      findFlag:FindMemberFlag ->
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap ->
            m:Range.range ->
              props:Infos.PropInfo list list -> Infos.PropInfo list list
    val ExcludeHiddenOfMethInfos:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          m:Range.range ->
            minfos:Infos.MethInfo list list -> Infos.MethInfo list
    val ExcludeHiddenOfPropInfos:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          m:Range.range ->
            pinfos:Infos.PropInfo list list -> Infos.PropInfo list
    val GetIntrinsicMethInfoSetsOfType:
      infoReader:InfoReader ->
        optFilter:string option ->
          ad:AccessibilityLogic.AccessorDomain ->
            allowMultiIntfInst:Infos.AllowMultiIntfInstantiations ->
              findFlag:FindMemberFlag ->
                m:Range.range -> ty:TypedTree.TType -> Infos.MethInfo list list
    val GetIntrinsicPropInfoSetsOfType:
      infoReader:InfoReader ->
        optFilter:string option ->
          ad:AccessibilityLogic.AccessorDomain ->
            allowMultiIntfInst:Infos.AllowMultiIntfInstantiations ->
              findFlag:FindMemberFlag ->
                m:Range.range -> ty:TypedTree.TType -> Infos.PropInfo list list
    val GetIntrinsicMethInfosOfType:
      infoReader:InfoReader ->
        optFilter:string option ->
          ad:AccessibilityLogic.AccessorDomain ->
            allowMultiIntfInst:Infos.AllowMultiIntfInstantiations ->
              findFlag:FindMemberFlag ->
                m:Range.range -> ty:TypedTree.TType -> Infos.MethInfo list
    val GetIntrinsicPropInfosOfType:
      infoReader:InfoReader ->
        optFilter:string option ->
          ad:AccessibilityLogic.AccessorDomain ->
            allowMultiIntfInst:Infos.AllowMultiIntfInstantiations ->
              findFlag:FindMemberFlag ->
                m:Range.range -> ty:TypedTree.TType -> Infos.PropInfo list
    val TryFindIntrinsicNamedItemOfType:
      infoReader:InfoReader ->
        nm:string * ad:AccessibilityLogic.AccessorDomain ->
          findFlag:FindMemberFlag ->
            m:Range.range -> ty:TypedTree.TType -> HierarchyItem option
    val TryFindIntrinsicMethInfo:
      infoReader:InfoReader ->
        m:Range.range ->
          ad:AccessibilityLogic.AccessorDomain ->
            nm:string -> ty:TypedTree.TType -> Infos.MethInfo list
    val TryFindPropInfo:
      infoReader:InfoReader ->
        m:Range.range ->
          ad:AccessibilityLogic.AccessorDomain ->
            nm:string -> ty:TypedTree.TType -> Infos.PropInfo list
    val GetIntrinisicMostSpecificOverrideMethInfoSetsOfType:
      infoReader:InfoReader ->
        m:Range.range ->
          ty:TypedTree.TType ->
            AbstractIL.Internal.Library.NameMultiMap<TypedTree.TType *
                                                     Infos.MethInfo>
    [<NoEquality; NoComparison>]
    type SigOfFunctionForDelegate =
      | SigOfFunctionForDelegate of
        Infos.MethInfo * TypedTree.TType list * TypedTree.TType *
        TypedTree.TType
    val GetSigOfFunctionForDelegate:
      infoReader:InfoReader ->
        delty:TypedTree.TType ->
          m:Range.range ->
            ad:AccessibilityLogic.AccessorDomain -> SigOfFunctionForDelegate
    val TryDestStandardDelegateType:
      infoReader:InfoReader ->
        m:Range.range ->
          ad:AccessibilityLogic.AccessorDomain ->
            delTy:TypedTree.TType -> (TypedTree.TType * TypedTree.TType) option
    val IsStandardEventInfo:
      infoReader:InfoReader ->
        m:Range.range ->
          ad:AccessibilityLogic.AccessorDomain -> einfo:Infos.EventInfo -> bool
    val ArgsTypOfEventInfo:
      infoReader:InfoReader ->
        m:Range.range ->
          ad:AccessibilityLogic.AccessorDomain ->
            einfo:Infos.EventInfo -> TypedTree.TType
    val PropTypOfEventInfo:
      infoReader:InfoReader ->
        m:Range.range ->
          ad:AccessibilityLogic.AccessorDomain ->
            einfo:Infos.EventInfo -> TypedTree.TType


namespace FSharp.Compiler
  module internal NicePrint =
    module PrintUtilities =
      val bracketIfL:
        x:bool ->
          lyt:Internal.Utilities.StructuredFormat.Layout ->
            Internal.Utilities.StructuredFormat.Layout
      val squareAngleL:
        x:Internal.Utilities.StructuredFormat.Layout ->
          Internal.Utilities.StructuredFormat.Layout
      val angleL:
        x:Internal.Utilities.StructuredFormat.Layout ->
          Internal.Utilities.StructuredFormat.Layout
      val braceL:
        x:Internal.Utilities.StructuredFormat.Layout ->
          Internal.Utilities.StructuredFormat.Layout
      val braceBarL:
        x:Internal.Utilities.StructuredFormat.Layout ->
          Internal.Utilities.StructuredFormat.Layout
      val comment: str:string -> Internal.Utilities.StructuredFormat.Layout
      val layoutsL: ls:Layout.layout list -> Layout.layout
      val suppressInheritanceAndInterfacesForTyInSimplifiedDisplays:
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap -> m:Range.range -> ty:TypedTree.TType -> bool
      val applyMaxMembers:
        maxMembers:int option ->
          allDecls:Internal.Utilities.StructuredFormat.Layout list ->
            Internal.Utilities.StructuredFormat.Layout list
      val adjustILName: n:string -> string
      val shrinkOverloads:
        layoutFunction:('a -> Internal.Utilities.StructuredFormat.Layout) ->
          resultFunction:('a -> Internal.Utilities.StructuredFormat.Layout -> 'b) ->
            group:'a list -> 'b list
      val layoutTyconRefImpl:
        isAttribute:bool ->
          denv:TypedTreeOps.DisplayEnv ->
            tcref:TypedTree.TyconRef ->
              Internal.Utilities.StructuredFormat.Layout
      val layoutBuiltinAttribute:
        denv:TypedTreeOps.DisplayEnv ->
          attrib:TcGlobals.BuiltinAttribInfo ->
            Internal.Utilities.StructuredFormat.Layout
  
    module private PrintIL =
      val fullySplitILTypeRef: tref:AbstractIL.IL.ILTypeRef -> string list
      val layoutILTypeRefName:
        denv:TypedTreeOps.DisplayEnv ->
          path:string list -> Internal.Utilities.StructuredFormat.Layout
      val layoutILTypeRef:
        denv:TypedTreeOps.DisplayEnv ->
          tref:AbstractIL.IL.ILTypeRef ->
            Internal.Utilities.StructuredFormat.Layout
      val layoutILArrayShape:
        AbstractIL.IL.ILArrayShape -> Internal.Utilities.StructuredFormat.Layout
      val paramsL: ps:Layout.layout list -> Layout.layout
      val pruneParams:
    Name:string ->
          ilTyparSubst:Layout.layout list -> Layout.layout list
      val layoutILType:
        denv:TypedTreeOps.DisplayEnv ->
          ilTyparSubst:Layout.layout list ->
            ty:AbstractIL.IL.ILType -> Layout.layout
      val layoutILCallingSignature:
        denv:TypedTreeOps.DisplayEnv ->
          ilTyparSubst:Layout.layout list ->
            cons:string option ->
              signature:AbstractIL.IL.ILCallingSignature -> Layout.layout
      val layoutILFieldInit:
        x:AbstractIL.IL.ILFieldInit option ->
          Internal.Utilities.StructuredFormat.Layout
      val layoutILEnumDefParts:
        nm:string ->
          litVal:AbstractIL.IL.ILFieldInit option ->
            Internal.Utilities.StructuredFormat.Layout
  
    module private PrintTypes =
      val layoutConst:
        g:TcGlobals.TcGlobals ->
          ty:TypedTree.TType ->
            c:TypedTree.Const -> Internal.Utilities.StructuredFormat.Layout
      val layoutAccessibility:
        denv:TypedTreeOps.DisplayEnv ->
          accessibility:TypedTree.Accessibility ->
            itemL:Internal.Utilities.StructuredFormat.Layout ->
              Internal.Utilities.StructuredFormat.Layout
      val layoutTyconRef:
        denv:TypedTreeOps.DisplayEnv ->
          tycon:TypedTree.TyconRef -> Internal.Utilities.StructuredFormat.Layout
      val layoutMemberFlags:
        memFlags:SyntaxTree.MemberFlags ->
          Internal.Utilities.StructuredFormat.Layout
      val layoutAttribArg:
        denv:TypedTreeOps.DisplayEnv ->
          arg:TypedTree.Expr -> Internal.Utilities.StructuredFormat.Layout
      val layoutAttribArgs:
        denv:TypedTreeOps.DisplayEnv ->
          args:TypedTree.AttribExpr list ->
            Internal.Utilities.StructuredFormat.Layout
      val layoutAttrib:
        denv:TypedTreeOps.DisplayEnv ->
          TypedTree.Attrib -> Internal.Utilities.StructuredFormat.Layout
      val layoutILAttribElement:
        denv:TypedTreeOps.DisplayEnv ->
          arg:AbstractIL.IL.ILAttribElem ->
            Internal.Utilities.StructuredFormat.Layout
      val layoutILAttrib:
        denv:TypedTreeOps.DisplayEnv ->
          ty:AbstractIL.IL.ILType * args:AbstractIL.IL.ILAttribElem list ->
            Internal.Utilities.StructuredFormat.Layout
      val layoutAttribs:
        denv:TypedTreeOps.DisplayEnv ->
          isValue:bool ->
            ty:TypedTree.TType ->
              kind:TypedTree.TyparKind ->
                attrs:TypedTree.Attrib list ->
                  restL:Internal.Utilities.StructuredFormat.Layout ->
                    Internal.Utilities.StructuredFormat.Layout
      val layoutTyparAttribs:
        denv:TypedTreeOps.DisplayEnv ->
          kind:TypedTree.TyparKind ->
            attrs:TypedTree.Attrib list ->
              restL:Internal.Utilities.StructuredFormat.Layout ->
                Internal.Utilities.StructuredFormat.Layout
      val layoutTyparRef:
        denv:TypedTreeOps.DisplayEnv ->
          typar:TypedTree.Typar -> Internal.Utilities.StructuredFormat.Layout
      val layoutTyparRefWithInfo:
        denv:TypedTreeOps.DisplayEnv ->
          env:TypedTreeOps.SimplifyTypes.TypeSimplificationInfo ->
            typar:TypedTree.Typar -> Internal.Utilities.StructuredFormat.Layout
      val layoutConstraintsWithInfo:
        denv:TypedTreeOps.DisplayEnv ->
          env:TypedTreeOps.SimplifyTypes.TypeSimplificationInfo ->
            cxs:(TypedTree.Typar * TypedTree.TyparConstraint) list ->
              Internal.Utilities.StructuredFormat.Layout
      val layoutConstraintWithInfo:
        denv:TypedTreeOps.DisplayEnv ->
          env:TypedTreeOps.SimplifyTypes.TypeSimplificationInfo ->
            tp:TypedTree.Typar * tpc:TypedTree.TyparConstraint ->
              Internal.Utilities.StructuredFormat.Layout list
      val layoutTraitWithInfo:
        denv:TypedTreeOps.DisplayEnv ->
          env:TypedTreeOps.SimplifyTypes.TypeSimplificationInfo ->
            TypedTree.TraitConstraintInfo ->
              Internal.Utilities.StructuredFormat.Layout
      val layoutMeasure:
        denv:TypedTreeOps.DisplayEnv ->
          unt:TypedTree.Measure -> Internal.Utilities.StructuredFormat.Layout
      val layoutTypeAppWithInfoAndPrec:
        denv:TypedTreeOps.DisplayEnv ->
          env:TypedTreeOps.SimplifyTypes.TypeSimplificationInfo ->
            tcL:Internal.Utilities.StructuredFormat.Layout ->
              prec:int ->
                prefix:bool ->
                  args:TypedTree.TType list ->
                    Internal.Utilities.StructuredFormat.Layout
      val layoutTypeWithInfoAndPrec:
        denv:TypedTreeOps.DisplayEnv ->
          env:TypedTreeOps.SimplifyTypes.TypeSimplificationInfo ->
            prec:int ->
              ty:TypedTree.TType -> Internal.Utilities.StructuredFormat.Layout
      val layoutTypesWithInfoAndPrec:
        denv:TypedTreeOps.DisplayEnv ->
          env:TypedTreeOps.SimplifyTypes.TypeSimplificationInfo ->
            prec:int ->
              sep:Internal.Utilities.StructuredFormat.Layout ->
                typl:TypedTree.TType list ->
                  Internal.Utilities.StructuredFormat.Layout
      val layoutReturnType:
        denv:TypedTreeOps.DisplayEnv ->
          env:TypedTreeOps.SimplifyTypes.TypeSimplificationInfo ->
            rty:TypedTree.TType -> Internal.Utilities.StructuredFormat.Layout
      val layoutTypeWithInfo:
        denv:TypedTreeOps.DisplayEnv ->
          env:TypedTreeOps.SimplifyTypes.TypeSimplificationInfo ->
            ty:TypedTree.TType -> Internal.Utilities.StructuredFormat.Layout
      val layoutType:
        denv:TypedTreeOps.DisplayEnv ->
          ty:TypedTree.TType -> Internal.Utilities.StructuredFormat.Layout
      val layoutArgInfos:
        denv:TypedTreeOps.DisplayEnv ->
          env:TypedTreeOps.SimplifyTypes.TypeSimplificationInfo ->
            argInfos:(TypedTree.TType * TypedTree.ArgReprInfo) list list ->
              Internal.Utilities.StructuredFormat.Layout list
      val layoutGenericParameterTypes:
        denv:TypedTreeOps.DisplayEnv ->
          env:TypedTreeOps.SimplifyTypes.TypeSimplificationInfo ->
            _arg1:TypedTree.TType list ->
              Internal.Utilities.StructuredFormat.Layout
      val layoutTopType:
        denv:TypedTreeOps.DisplayEnv ->
          env:TypedTreeOps.SimplifyTypes.TypeSimplificationInfo ->
            argInfos:(TypedTree.TType * TypedTree.ArgReprInfo) list list ->
              rty:TypedTree.TType ->
                cxs:(TypedTree.Typar * TypedTree.TyparConstraint) list ->
                  Internal.Utilities.StructuredFormat.Layout
      val layoutTyparDecls:
        denv:TypedTreeOps.DisplayEnv ->
          nmL:Internal.Utilities.StructuredFormat.Layout ->
            prefix:bool ->
              typars:TypedTree.Typars ->
                Internal.Utilities.StructuredFormat.Layout
      val layoutTyparConstraint:
        denv:TypedTreeOps.DisplayEnv ->
          tp:TypedTree.Typar * tpc:TypedTree.TyparConstraint ->
            Internal.Utilities.StructuredFormat.Layout
      val prettyLayoutOfInstAndSig:
        denv:TypedTreeOps.DisplayEnv ->
          typarInst:TypedTreeOps.TyparInst * tys:TypedTree.TTypes *
          retTy:TypedTree.TType ->
            TypedTreeOps.TyparInst * (TypedTree.TTypes * TypedTree.TType) *
            (Internal.Utilities.StructuredFormat.Layout list *
             Internal.Utilities.StructuredFormat.Layout) *
            Internal.Utilities.StructuredFormat.Layout
      val prettyLayoutOfTopTypeInfoAux:
        denv:TypedTreeOps.DisplayEnv ->
          prettyArgInfos:(TypedTree.TType * TypedTree.ArgReprInfo) list list ->
            prettyRetTy:TypedTree.TType ->
              cxs:TypedTreeOps.TyparConstraintsWithTypars ->
                Internal.Utilities.StructuredFormat.Layout
      val prettyLayoutOfUncurriedSig:
        denv:TypedTreeOps.DisplayEnv ->
          typarInst:TypedTreeOps.TyparInst ->
            argInfos:TypedTreeOps.UncurriedArgInfos ->
              retTy:TypedTree.TType ->
                TypedTreeOps.TyparInst *
                Internal.Utilities.StructuredFormat.Layout
      val prettyLayoutOfCurriedMemberSig:
        denv:TypedTreeOps.DisplayEnv ->
          typarInst:TypedTreeOps.TyparInst ->
            argInfos:TypedTreeOps.CurriedArgInfos ->
              retTy:TypedTree.TType ->
                parentTyparTys:TypedTree.TTypes ->
                  TypedTreeOps.TyparInst *
                  Internal.Utilities.StructuredFormat.Layout
      val prettyArgInfos:
        denv:TypedTreeOps.DisplayEnv ->
          allTyparInst:TypedTreeOps.TyparInst ->
            _arg1:(TypedTree.TType * TypedTree.ArgReprInfo) list ->
              (TypedTree.TType * TypedTree.ArgReprInfo) list
      val prettyLayoutOfMemberSigCore:
        denv:TypedTreeOps.DisplayEnv ->
          memberToParentInst:(TypedTree.Typar * TypedTree.TType) list ->
            typarInst:TypedTreeOps.TyparInst * methTypars:TypedTree.Typars *
            argInfos:(TypedTree.TType * TypedTree.ArgReprInfo) list list *
            retTy:TypedTree.TType ->
              TypedTreeOps.TyparInst * TypedTree.Typars *
              Internal.Utilities.StructuredFormat.Layout
      val prettyLayoutOfMemberType:
        denv:TypedTreeOps.DisplayEnv ->
          v:TypedTree.ValRef ->
            typarInst:TypedTreeOps.TyparInst ->
              argInfos:(TypedTree.TType * TypedTree.ArgReprInfo) list list ->
                retTy:TypedTree.TType ->
                  TypedTreeOps.TyparInst * TypedTree.Typars *
                  Internal.Utilities.StructuredFormat.Layout
      val prettyLayoutOfMemberSig:
        denv:TypedTreeOps.DisplayEnv ->
          memberToParentInst:(TypedTree.Typar * TypedTree.TType) list *
          nm:string * methTypars:TypedTree.Typars *
          argInfos:(TypedTree.TType * TypedTree.ArgReprInfo) list list *
          retTy:TypedTree.TType -> Internal.Utilities.StructuredFormat.Layout
      val prettyLayoutsOfUnresolvedOverloading:
        denv:TypedTreeOps.DisplayEnv ->
          argInfos:(TypedTree.TType * TypedTree.ArgReprInfo) list ->
            retTy:TypedTree.TType ->
              genParamTys:seq<TypedTree.TType> ->
                Internal.Utilities.StructuredFormat.Layout *
                Internal.Utilities.StructuredFormat.Layout *
                Internal.Utilities.StructuredFormat.Layout
      val prettyLayoutOfType:
        denv:TypedTreeOps.DisplayEnv ->
          ty:TypedTree.TType -> Internal.Utilities.StructuredFormat.Layout
      val prettyLayoutOfTypeNoConstraints:
        denv:TypedTreeOps.DisplayEnv ->
          ty:TypedTree.TType -> Internal.Utilities.StructuredFormat.Layout
      val layoutAssemblyName: _denv:'a -> ty:TypedTree.TType -> string
  
    module private PrintTastMemberOrVals =
      val prettyLayoutOfMemberShortOption:
        denv:TypedTreeOps.DisplayEnv ->
          typarInst:TypedTreeOps.TyparInst ->
            v:TypedTree.Val ->
              short:bool ->
                TypedTreeOps.TyparInst *
                Internal.Utilities.StructuredFormat.Layout
      val prettyLayoutOfMember:
        denv:TypedTreeOps.DisplayEnv ->
          typarInst:TypedTreeOps.TyparInst ->
            v:TypedTree.Val ->
              TypedTreeOps.TyparInst *
              Internal.Utilities.StructuredFormat.Layout
      val prettyLayoutOfMemberNoInstShort:
        denv:TypedTreeOps.DisplayEnv ->
          v:TypedTree.Val -> Internal.Utilities.StructuredFormat.Layout
      val layoutOfLiteralValue:
        literalValue:TypedTree.Const ->
          Internal.Utilities.StructuredFormat.Layout
      val layoutNonMemberVal:
        denv:TypedTreeOps.DisplayEnv ->
          tps:TypedTree.Typar list * v:TypedTree.Val * tau:TypedTree.TType *
          cxs:TypedTreeOps.TyparConstraintsWithTypars ->
            Internal.Utilities.StructuredFormat.Layout
      val prettyLayoutOfValOrMember:
        denv:TypedTreeOps.DisplayEnv ->
          typarInst:TypedTreeOps.TyparInst ->
            v:TypedTree.Val ->
              TypedTreeOps.TyparInst *
              Internal.Utilities.StructuredFormat.Layout
      val prettyLayoutOfValOrMemberNoInst:
        denv:TypedTreeOps.DisplayEnv ->
          v:TypedTree.Val -> Internal.Utilities.StructuredFormat.Layout
  
    val layoutTyparConstraint:
      denv:TypedTreeOps.DisplayEnv ->
        TypedTree.Typar * TypedTree.TyparConstraint ->
          Internal.Utilities.StructuredFormat.Layout
    val outputType:
      denv:TypedTreeOps.DisplayEnv ->
        os:System.Text.StringBuilder -> x:TypedTree.TType -> unit
    val layoutType:
      denv:TypedTreeOps.DisplayEnv ->
        x:TypedTree.TType -> Internal.Utilities.StructuredFormat.Layout
    val outputTypars:
      denv:TypedTreeOps.DisplayEnv ->
        nm:Layout.TaggedText ->
          os:System.Text.StringBuilder -> x:TypedTree.Typars -> unit
    val outputTyconRef:
      denv:TypedTreeOps.DisplayEnv ->
        os:System.Text.StringBuilder -> x:TypedTree.TyconRef -> unit
    val layoutTyconRef:
      denv:TypedTreeOps.DisplayEnv ->
        x:TypedTree.TyconRef -> Internal.Utilities.StructuredFormat.Layout
    val layoutConst:
      g:TcGlobals.TcGlobals ->
        ty:TypedTree.TType ->
          c:TypedTree.Const -> Internal.Utilities.StructuredFormat.Layout
    val prettyLayoutOfMemberSig:
      denv:TypedTreeOps.DisplayEnv ->
        (TypedTree.Typar * TypedTree.TType) list * string * TypedTree.Typars *
        (TypedTree.TType * TypedTree.ArgReprInfo) list list * TypedTree.TType ->
          Internal.Utilities.StructuredFormat.Layout
    val prettyLayoutOfUncurriedSig:
      denv:TypedTreeOps.DisplayEnv ->
        argInfos:TypedTreeOps.TyparInst ->
          tau:TypedTreeOps.UncurriedArgInfos ->
            (TypedTree.TType ->
               TypedTreeOps.TyparInst *
               Internal.Utilities.StructuredFormat.Layout)
    val prettyLayoutsOfUnresolvedOverloading:
      denv:TypedTreeOps.DisplayEnv ->
        argInfos:(TypedTree.TType * TypedTree.ArgReprInfo) list ->
          retTy:TypedTree.TType ->
            genericParameters:seq<TypedTree.TType> ->
              Internal.Utilities.StructuredFormat.Layout *
              Internal.Utilities.StructuredFormat.Layout *
              Internal.Utilities.StructuredFormat.Layout
    module InfoMemberPrinting =
      val layoutParamData:
        denv:TypedTreeOps.DisplayEnv ->
          Infos.ParamData -> Internal.Utilities.StructuredFormat.Layout
      val formatParamDataToBuffer:
        denv:TypedTreeOps.DisplayEnv ->
          os:System.Text.StringBuilder -> pd:Infos.ParamData -> unit
      val private layoutMethInfoFSharpStyleCore:
        amap:Import.ImportMap ->
          m:Range.range ->
            denv:TypedTreeOps.DisplayEnv ->
              minfo:Infos.MethInfo ->
                minst:TypedTree.TType list ->
                  Internal.Utilities.StructuredFormat.Layout
      val private layoutMethInfoCSharpStyle:
        amap:Import.ImportMap ->
          m:Range.range ->
            denv:TypedTreeOps.DisplayEnv ->
              minfo:Infos.MethInfo ->
                minst:TypedTree.TType list ->
                  Internal.Utilities.StructuredFormat.Layout
      val prettifyILMethInfo:
        amap:Import.ImportMap ->
          m:Range.range ->
            minfo:Infos.MethInfo ->
              typarInst:TypedTreeOps.TyparInst ->
                ilMethInfo:Infos.ILMethInfo ->
                  TypedTreeOps.TyparInst * Infos.MethInfo * TypedTree.TType list
      val prettyLayoutOfMethInfoFreeStyle:
        amap:Import.ImportMap ->
          m:Range.range ->
            denv:TypedTreeOps.DisplayEnv ->
              typarInst:TypedTreeOps.TyparInst ->
                methInfo:Infos.MethInfo ->
                  TypedTreeOps.TyparInst *
                  Internal.Utilities.StructuredFormat.Layout
      val prettyLayoutOfPropInfoFreeStyle:
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap ->
            m:Range.range ->
              denv:TypedTreeOps.DisplayEnv ->
                pinfo:Infos.PropInfo ->
                  Internal.Utilities.StructuredFormat.Layout
      val formatMethInfoToBufferFreeStyle:
        amap:Import.ImportMap ->
          m:Range.range ->
            denv:TypedTreeOps.DisplayEnv ->
              os:System.Text.StringBuilder -> minfo:Infos.MethInfo -> unit
      val layoutMethInfoFSharpStyle:
        amap:Import.ImportMap ->
          m:Range.range ->
            denv:TypedTreeOps.DisplayEnv ->
              minfo:Infos.MethInfo -> Internal.Utilities.StructuredFormat.Layout
  
    module private TastDefinitionPrinting =
      val layoutExtensionMember:
        denv:TypedTreeOps.DisplayEnv ->
          v:TypedTree.Val -> Internal.Utilities.StructuredFormat.Layout
      val layoutExtensionMembers:
        denv:TypedTreeOps.DisplayEnv ->
          vs:TypedTree.Val list -> Internal.Utilities.StructuredFormat.Layout
      val layoutRecdField:
        addAccess:bool ->
          denv:TypedTreeOps.DisplayEnv ->
            fld:TypedTree.RecdField ->
              Internal.Utilities.StructuredFormat.Layout
      val layoutUnionOrExceptionField:
        denv:TypedTreeOps.DisplayEnv ->
          isGenerated:('a -> TypedTree.RecdField -> bool) ->
            i:'a ->
              fld:TypedTree.RecdField ->
                Internal.Utilities.StructuredFormat.Layout
      val isGeneratedUnionCaseField: pos:int -> f:TypedTree.RecdField -> bool
      val isGeneratedExceptionField: pos:'a -> f:TypedTree.RecdField -> bool
      val layoutUnionCaseFields:
        denv:TypedTreeOps.DisplayEnv ->
          isUnionCase:bool ->
            fields:TypedTree.RecdField list ->
              Internal.Utilities.StructuredFormat.Layout
      val layoutUnionCase:
        denv:TypedTreeOps.DisplayEnv ->
          prefixL:Internal.Utilities.StructuredFormat.Layout ->
            ucase:TypedTree.UnionCase ->
              Internal.Utilities.StructuredFormat.Layout
      val layoutUnionCases:
        denv:TypedTreeOps.DisplayEnv ->
          ucases:TypedTree.UnionCase list ->
            Internal.Utilities.StructuredFormat.Layout list
      val breakTypeDefnEqn: repr:TypedTree.TyconRepresentation -> bool
      val layoutILFieldInfo:
        denv:TypedTreeOps.DisplayEnv ->
          amap:Import.ImportMap ->
            m:Range.range ->
              e:Infos.ILFieldInfo -> Internal.Utilities.StructuredFormat.Layout
      val layoutEventInfo:
        denv:TypedTreeOps.DisplayEnv ->
          amap:Import.ImportMap ->
            m:Range.range ->
              e:Infos.EventInfo -> Internal.Utilities.StructuredFormat.Layout
      val layoutPropInfo:
        denv:TypedTreeOps.DisplayEnv ->
          amap:Import.ImportMap ->
            m:Range.range ->
              p:Infos.PropInfo -> Internal.Utilities.StructuredFormat.Layout
      val layoutTycon:
        denv:TypedTreeOps.DisplayEnv ->
          infoReader:InfoReader.InfoReader ->
            ad:AccessibilityLogic.AccessorDomain ->
              m:Range.range ->
                simplified:bool ->
                  typewordL:Internal.Utilities.StructuredFormat.Layout ->
                    tycon:TypedTree.Tycon ->
                      Internal.Utilities.StructuredFormat.Layout
      val layoutExnDefn:
        denv:TypedTreeOps.DisplayEnv ->
          exnc:TypedTree.Entity -> Internal.Utilities.StructuredFormat.Layout
      val layoutTyconDefns:
        denv:TypedTreeOps.DisplayEnv ->
          infoReader:InfoReader.InfoReader ->
            ad:AccessibilityLogic.AccessorDomain ->
              m:Range.range ->
                tycons:TypedTree.Tycon list ->
                  Internal.Utilities.StructuredFormat.Layout
  
    module private InferredSigPrinting =
      val layoutInferredSigOfModuleExpr:
        showHeader:bool ->
          denv:TypedTreeOps.DisplayEnv ->
            infoReader:InfoReader.InfoReader ->
              ad:AccessibilityLogic.AccessorDomain ->
                m:Range.range ->
                  expr:TypedTree.ModuleOrNamespaceExprWithSig ->
                    Internal.Utilities.StructuredFormat.Layout
  
    module private PrintData =
      val dataExprL:
        denv:TypedTreeOps.DisplayEnv ->
          expr:TypedTree.Expr -> Internal.Utilities.StructuredFormat.Layout
      val dataExprWrapL:
        denv:TypedTreeOps.DisplayEnv ->
          isAtomic:bool ->
            expr:TypedTree.Expr -> Internal.Utilities.StructuredFormat.Layout
      val dataExprsL:
        denv:TypedTreeOps.DisplayEnv ->
          xs:TypedTree.Exprs -> Internal.Utilities.StructuredFormat.Layout list
  
    val dataExprL:
      denv:TypedTreeOps.DisplayEnv ->
        expr:TypedTree.Expr -> Internal.Utilities.StructuredFormat.Layout
    val outputValOrMember:
      denv:TypedTreeOps.DisplayEnv ->
        os:System.Text.StringBuilder -> x:TypedTree.Val -> unit
    val stringValOrMember:
      denv:TypedTreeOps.DisplayEnv -> x:TypedTree.Val -> string
    val layoutQualifiedValOrMember:
      denv:TypedTreeOps.DisplayEnv ->
        typarInst:TypedTreeOps.TyparInst ->
          v:TypedTree.Val ->
            TypedTreeOps.TyparInst * Internal.Utilities.StructuredFormat.Layout
    val outputQualifiedValOrMember:
      denv:TypedTreeOps.DisplayEnv ->
        os:System.Text.StringBuilder -> v:TypedTree.Val -> unit
    val outputQualifiedValSpec:
      denv:TypedTreeOps.DisplayEnv ->
        os:System.Text.StringBuilder -> v:TypedTree.Val -> unit
    val stringOfQualifiedValOrMember:
      denv:TypedTreeOps.DisplayEnv -> v:TypedTree.Val -> string
    val formatMethInfoToBufferFreeStyle:
      amap:Import.ImportMap ->
        m:Range.range ->
          denv:TypedTreeOps.DisplayEnv ->
            buf:System.Text.StringBuilder -> d:Infos.MethInfo -> unit
    val prettyLayoutOfMethInfoFreeStyle:
      amap:Import.ImportMap ->
        m:Range.range ->
          denv:TypedTreeOps.DisplayEnv ->
            typarInst:TypedTreeOps.TyparInst ->
              minfo:Infos.MethInfo ->
                TypedTreeOps.TyparInst *
                Internal.Utilities.StructuredFormat.Layout
    val prettyLayoutOfPropInfoFreeStyle:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          m:Range.range ->
            denv:TypedTreeOps.DisplayEnv ->
              d:Infos.PropInfo -> Internal.Utilities.StructuredFormat.Layout
    val stringOfMethInfo:
      amap:Import.ImportMap ->
        m:Range.range ->
          denv:TypedTreeOps.DisplayEnv -> d:Infos.MethInfo -> string
    val stringOfParamData:
      denv:TypedTreeOps.DisplayEnv -> paramData:Infos.ParamData -> string
    val layoutOfParamData:
      denv:TypedTreeOps.DisplayEnv ->
        paramData:Infos.ParamData -> Internal.Utilities.StructuredFormat.Layout
    val outputExnDef:
      denv:TypedTreeOps.DisplayEnv ->
        os:System.Text.StringBuilder -> x:TypedTree.Entity -> unit
    val layoutExnDef:
      denv:TypedTreeOps.DisplayEnv ->
        x:TypedTree.Entity -> Internal.Utilities.StructuredFormat.Layout
    val stringOfTyparConstraints:
      denv:TypedTreeOps.DisplayEnv ->
        x:(TypedTree.Typar * TypedTree.TyparConstraint) list -> string
    val outputTycon:
      denv:TypedTreeOps.DisplayEnv ->
        infoReader:InfoReader.InfoReader ->
          ad:AccessibilityLogic.AccessorDomain ->
            m:Range.range ->
              os:System.Text.StringBuilder -> x:TypedTree.Tycon -> unit
    val layoutTycon:
      denv:TypedTreeOps.DisplayEnv ->
        infoReader:InfoReader.InfoReader ->
          ad:AccessibilityLogic.AccessorDomain ->
            m:Range.range ->
              x:TypedTree.Tycon -> Internal.Utilities.StructuredFormat.Layout
    val layoutUnionCases:
      denv:TypedTreeOps.DisplayEnv ->
        x:TypedTree.RecdField list -> Internal.Utilities.StructuredFormat.Layout
    val outputUnionCases:
      denv:TypedTreeOps.DisplayEnv ->
        os:System.Text.StringBuilder -> x:TypedTree.RecdField list -> unit
    val isGeneratedUnionCaseField: pos:int -> f:TypedTree.RecdField -> bool
    val isGeneratedExceptionField: pos:'a -> f:TypedTree.RecdField -> bool
    val stringOfTyparConstraint:
      denv:TypedTreeOps.DisplayEnv ->
        TypedTree.Typar * TypedTree.TyparConstraint -> string
    val stringOfTy: denv:TypedTreeOps.DisplayEnv -> x:TypedTree.TType -> string
    val prettyLayoutOfType:
      denv:TypedTreeOps.DisplayEnv ->
        x:TypedTree.TType -> Internal.Utilities.StructuredFormat.Layout
    val prettyLayoutOfTypeNoCx:
      denv:TypedTreeOps.DisplayEnv ->
        x:TypedTree.TType -> Internal.Utilities.StructuredFormat.Layout
    val prettyStringOfTy:
      denv:TypedTreeOps.DisplayEnv -> x:TypedTree.TType -> string
    val prettyStringOfTyNoCx:
      denv:TypedTreeOps.DisplayEnv -> x:TypedTree.TType -> string
    val stringOfRecdField:
      denv:TypedTreeOps.DisplayEnv -> x:TypedTree.RecdField -> string
    val stringOfUnionCase:
      denv:TypedTreeOps.DisplayEnv -> x:TypedTree.UnionCase -> string
    val stringOfExnDef:
      denv:TypedTreeOps.DisplayEnv -> x:TypedTree.Entity -> string
    val stringOfFSAttrib:
      denv:TypedTreeOps.DisplayEnv -> x:TypedTree.Attrib -> string
    val stringOfILAttrib:
      denv:TypedTreeOps.DisplayEnv ->
        AbstractIL.IL.ILType * AbstractIL.IL.ILAttribElem list -> string
    val layoutInferredSigOfModuleExpr:
      showHeader:bool ->
        denv:TypedTreeOps.DisplayEnv ->
          infoReader:InfoReader.InfoReader ->
            ad:AccessibilityLogic.AccessorDomain ->
              m:Range.range ->
                expr:TypedTree.ModuleOrNamespaceExprWithSig ->
                  Internal.Utilities.StructuredFormat.Layout
    val prettyLayoutOfValOrMember:
      denv:TypedTreeOps.DisplayEnv ->
        typarInst:TypedTreeOps.TyparInst ->
          v:TypedTree.Val ->
            TypedTreeOps.TyparInst * Internal.Utilities.StructuredFormat.Layout
    val prettyLayoutOfValOrMemberNoInst:
      denv:TypedTreeOps.DisplayEnv ->
        v:TypedTree.Val -> Internal.Utilities.StructuredFormat.Layout
    val prettyLayoutOfMemberNoInstShort:
      denv:TypedTreeOps.DisplayEnv ->
        v:TypedTree.Val -> Internal.Utilities.StructuredFormat.Layout
    val prettyLayoutOfInstAndSig:
      denv:TypedTreeOps.DisplayEnv ->
        TypedTreeOps.TyparInst * TypedTree.TTypes * TypedTree.TType ->
          TypedTreeOps.TyparInst * (TypedTree.TTypes * TypedTree.TType) *
          (Internal.Utilities.StructuredFormat.Layout list *
           Internal.Utilities.StructuredFormat.Layout) *
          Internal.Utilities.StructuredFormat.Layout
    val minimalStringsOfTwoTypes:
      denv:TypedTreeOps.DisplayEnv ->
        t1:TypedTree.TType -> t2:TypedTree.TType -> string * string * string
    val minimalStringsOfTwoValues:
      denv:TypedTreeOps.DisplayEnv ->
        v1:TypedTree.Val -> v2:TypedTree.Val -> string * string
    val minimalStringOfType:
      denv:TypedTreeOps.DisplayEnv -> ty:TypedTree.TType -> string


namespace FSharp.Compiler
  module internal AugmentWithHashCompare =
    val mkIComparableCompareToSlotSig:
      g:TcGlobals.TcGlobals -> TypedTree.SlotSig
    val mkGenericIComparableCompareToSlotSig:
      g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> TypedTree.SlotSig
    val mkIStructuralComparableCompareToSlotSig:
      g:TcGlobals.TcGlobals -> TypedTree.SlotSig
    val mkGenericIEquatableEqualsSlotSig:
      g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> TypedTree.SlotSig
    val mkIStructuralEquatableEqualsSlotSig:
      g:TcGlobals.TcGlobals -> TypedTree.SlotSig
    val mkIStructuralEquatableGetHashCodeSlotSig:
      g:TcGlobals.TcGlobals -> TypedTree.SlotSig
    val mkGetHashCodeSlotSig: g:TcGlobals.TcGlobals -> TypedTree.SlotSig
    val mkEqualsSlotSig: g:TcGlobals.TcGlobals -> TypedTree.SlotSig
    val mkThisTy:
      g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> TypedTree.TType
    val mkCompareObjTy:
      g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> TypedTree.TType
    val mkCompareTy:
      g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> TypedTree.TType
    val mkCompareWithComparerTy:
      g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> TypedTree.TType
    val mkEqualsObjTy:
      g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> TypedTree.TType
    val mkEqualsTy:
      g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> TypedTree.TType
    val mkEqualsWithComparerTy:
      g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> TypedTree.TType
    val mkHashTy:
      g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> TypedTree.TType
    val mkHashWithComparerTy:
      g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> TypedTree.TType
    val mkRelBinOp:
      g:TcGlobals.TcGlobals ->
        op:AbstractIL.IL.ILInstr ->
          m:Range.range ->
            e1:TypedTree.Expr -> e2:TypedTree.Expr -> TypedTree.Expr
    val mkClt:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          e1:TypedTree.Expr -> e2:TypedTree.Expr -> TypedTree.Expr
    val mkCgt:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          e1:TypedTree.Expr -> e2:TypedTree.Expr -> TypedTree.Expr
    val mkILLangPrimTy: g:TcGlobals.TcGlobals -> AbstractIL.IL.ILType
    val mkILCallGetComparer:
      g:TcGlobals.TcGlobals -> m:Range.range -> TypedTree.Expr
    val mkILCallGetEqualityComparer:
      g:TcGlobals.TcGlobals -> m:Range.range -> TypedTree.Expr
    val mkThisVar:
      g:TcGlobals.TcGlobals ->
        m:Range.range -> ty:TypedTree.TType -> TypedTree.Val * TypedTree.Expr
    val mkShl:
      g:TcGlobals.TcGlobals ->
        m:Range.range -> acce:TypedTree.Expr -> n:int -> TypedTree.Expr
    val mkShr:
      g:TcGlobals.TcGlobals ->
        m:Range.range -> acce:TypedTree.Expr -> n:int -> TypedTree.Expr
    val mkAdd:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          e1:TypedTree.Expr -> e2:TypedTree.Expr -> TypedTree.Expr
    val mkAddToHashAcc:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          e:TypedTree.Expr ->
            accv:TypedTree.ValRef -> acce:TypedTree.Expr -> TypedTree.Expr
    val mkCombineHashGenerators:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          exprs:TypedTree.Expr list ->
            accv:TypedTree.ValRef -> acce:TypedTree.Expr -> TypedTree.Expr
    val mkThatAddrLocal:
      g:TcGlobals.TcGlobals ->
        m:Range.range -> ty:TypedTree.TType -> TypedTree.Val * TypedTree.Expr
    val mkThatAddrLocalIfNeeded:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          tcve:TypedTree.Expr ->
            ty:TypedTree.TType -> TypedTree.Val option * TypedTree.Expr
    val mkThisVarThatVar:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          ty:TypedTree.TType ->
            TypedTree.Val * TypedTree.Val * TypedTree.Expr * TypedTree.Expr
    val mkThatVarBind:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          ty:TypedTree.TType ->
            thataddrv:TypedTree.Val ->
              expr:TypedTree.Expr -> TypedTree.Val * TypedTree.Expr
    val mkBindThatAddr:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          ty:TypedTree.TType ->
            thataddrv:TypedTree.Val ->
              thatv:TypedTree.Val ->
                thate:TypedTree.Expr -> expr:TypedTree.Expr -> TypedTree.Expr
    val mkBindThatAddrIfNeeded:
      m:Range.range ->
        thataddrvOpt:TypedTree.Val option ->
          thatv:TypedTree.Val -> expr:TypedTree.Expr -> TypedTree.Expr
    val mkDerefThis:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          thisv:TypedTree.Val -> thise:TypedTree.Expr -> TypedTree.Expr
    val mkCompareTestConjuncts:
      g:TcGlobals.TcGlobals ->
        m:Range.range -> exprs:TypedTree.Expr list -> TypedTree.Expr
    val mkEqualsTestConjuncts:
      g:TcGlobals.TcGlobals ->
        m:Range.range -> exprs:TypedTree.Expr list -> TypedTree.Expr
    val mkMinimalTy:
      g:TcGlobals.TcGlobals ->
        tcref:TypedTree.TyconRef -> TypedTree.TType list * TypedTree.TType
    val mkBindNullComparison:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          thise:TypedTree.Expr ->
            thate:TypedTree.Expr -> expr:TypedTree.Expr -> TypedTree.Expr
    val mkBindThisNullEquals:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          thise:TypedTree.Expr ->
            thate:TypedTree.Expr -> expr:TypedTree.Expr -> TypedTree.Expr
    val mkBindThatNullEquals:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          thise:TypedTree.Expr ->
            thate:TypedTree.Expr -> expr:TypedTree.Expr -> TypedTree.Expr
    val mkBindNullHash:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          thise:TypedTree.Expr -> expr:TypedTree.Expr -> TypedTree.Expr
    val mkRecdCompare:
      g:TcGlobals.TcGlobals ->
        tcref:TypedTree.TyconRef ->
          tycon:TypedTree.Tycon ->
            TypedTree.Val * TypedTree.Val * TypedTree.Expr
    val mkRecdCompareWithComparer:
      g:TcGlobals.TcGlobals ->
        tcref:TypedTree.TyconRef ->
          tycon:TypedTree.Tycon ->
            _thisv:'a * thise:TypedTree.Expr ->
              'b * thate:TypedTree.Expr ->
                compe:TypedTree.Expr -> TypedTree.Expr
    val mkRecdEquality:
      g:TcGlobals.TcGlobals ->
        tcref:TypedTree.TyconRef ->
          tycon:TypedTree.Tycon ->
            TypedTree.Val * TypedTree.Val * TypedTree.Expr
    val mkRecdEqualityWithComparer:
      g:TcGlobals.TcGlobals ->
        tcref:TypedTree.TyconRef ->
          tycon:TypedTree.Tycon ->
            _thisv:'a * thise:TypedTree.Expr ->
              thatobje:TypedTree.Expr ->
                thatv:TypedTree.Val * thate:TypedTree.Expr ->
                  compe:TypedTree.Expr -> TypedTree.Expr
    val mkExnEquality:
      g:TcGlobals.TcGlobals ->
        exnref:TypedTree.TyconRef ->
          exnc:TypedTree.Tycon -> TypedTree.Val * TypedTree.Val * TypedTree.Expr
    val mkExnEqualityWithComparer:
      g:TcGlobals.TcGlobals ->
        exnref:TypedTree.TyconRef ->
          exnc:TypedTree.Tycon ->
            _thisv:'a * thise:TypedTree.Expr ->
              thatobje:TypedTree.Expr ->
                thatv:TypedTree.Val * thate:TypedTree.Expr ->
                  compe:TypedTree.Expr -> TypedTree.Expr
    val mkUnionCompare:
      g:TcGlobals.TcGlobals ->
        tcref:TypedTree.TyconRef ->
          tycon:TypedTree.Tycon ->
            TypedTree.Val * TypedTree.Val * TypedTree.Expr
    val mkUnionCompareWithComparer:
      g:TcGlobals.TcGlobals ->
        tcref:TypedTree.TyconRef ->
          tycon:TypedTree.Tycon ->
            _thisv:'a * thise:TypedTree.Expr ->
              _thatobjv:'b * thatcaste:TypedTree.Expr ->
                compe:TypedTree.Expr -> TypedTree.Expr
    val mkUnionEquality:
      g:TcGlobals.TcGlobals ->
        tcref:TypedTree.TyconRef ->
          tycon:TypedTree.Tycon ->
            TypedTree.Val * TypedTree.Val * TypedTree.Expr
    val mkUnionEqualityWithComparer:
      g:TcGlobals.TcGlobals ->
        tcref:TypedTree.TyconRef ->
          tycon:TypedTree.Tycon ->
            _thisv:'a * thise:TypedTree.Expr ->
              thatobje:TypedTree.Expr ->
                thatv:TypedTree.Val * thate:TypedTree.Expr ->
                  compe:TypedTree.Expr -> TypedTree.Expr
    val mkRecdHashWithComparer:
      g:TcGlobals.TcGlobals ->
        tcref:TypedTree.TyconRef ->
          tycon:TypedTree.Tycon ->
            compe:TypedTree.Expr -> TypedTree.Val * TypedTree.Expr
    val mkExnHashWithComparer:
      g:TcGlobals.TcGlobals ->
        exnref:TypedTree.TyconRef ->
          exnc:TypedTree.Tycon ->
            compe:TypedTree.Expr -> TypedTree.Val * TypedTree.Expr
    val mkUnionHashWithComparer:
      g:TcGlobals.TcGlobals ->
        tcref:TypedTree.TyconRef ->
          tycon:TypedTree.Tycon ->
            compe:TypedTree.Expr -> TypedTree.Val * TypedTree.Expr
    val isNominalExnc: exnc:TypedTree.Tycon -> bool
    val isTrueFSharpStructTycon: _g:'a -> tycon:TypedTree.Tycon -> bool
    val canBeAugmentedWithEquals: g:'a -> tycon:TypedTree.Tycon -> bool
    val canBeAugmentedWithCompare: g:'a -> tycon:TypedTree.Tycon -> bool
    val getAugmentationAttribs:
      g:TcGlobals.TcGlobals ->
        tycon:TypedTree.Tycon ->
          bool * bool * bool option * bool option * bool option * bool option *
          bool option * bool option * bool option
    val CheckAugmentationAttribs:
      bool -> TcGlobals.TcGlobals -> Import.ImportMap -> TypedTree.Tycon -> unit
    val TyconIsCandidateForAugmentationWithCompare:
      TcGlobals.TcGlobals -> TypedTree.Tycon -> bool
    val TyconIsCandidateForAugmentationWithEquals:
      TcGlobals.TcGlobals -> TypedTree.Tycon -> bool
    val TyconIsCandidateForAugmentationWithHash:
      TcGlobals.TcGlobals -> TypedTree.Tycon -> bool
    val slotImplMethod:
      final:bool * c:TypedTree.TyconRef * slotsig:TypedTree.SlotSig ->
        TypedTree.ValMemberInfo
    val nonVirtualMethod: c:TypedTree.TyconRef -> TypedTree.ValMemberInfo
    val unitArg: TypedTree.ArgReprInfo list list
    val unaryArg: TypedTree.ArgReprInfo list list
    val tupArg: TypedTree.ArgReprInfo list list
    val mkValSpec:
      g:TcGlobals.TcGlobals ->
        tcref:TypedTree.TyconRef ->
          tmty:TypedTree.TType ->
            vis:TypedTree.Accessibility ->
              slotsig:TypedTree.SlotSig option ->
                methn:string ->
                  ty:TypedTree.TType ->
                    argData:TypedTree.ArgReprInfo list list -> TypedTree.Val
    val MakeValsForCompareAugmentation:
      TcGlobals.TcGlobals -> TypedTree.TyconRef -> TypedTree.Val * TypedTree.Val
    val MakeValsForCompareWithComparerAugmentation:
      TcGlobals.TcGlobals -> TypedTree.TyconRef -> TypedTree.Val
    val MakeValsForEqualsAugmentation:
      TcGlobals.TcGlobals -> TypedTree.TyconRef -> TypedTree.Val * TypedTree.Val
    val MakeValsForEqualityWithComparerAugmentation:
      TcGlobals.TcGlobals ->
        TypedTree.TyconRef -> TypedTree.Val * TypedTree.Val * TypedTree.Val
    val MakeBindingsForCompareAugmentation:
      TcGlobals.TcGlobals -> TypedTree.Tycon -> TypedTree.Binding list
    val MakeBindingsForCompareWithComparerAugmentation:
      TcGlobals.TcGlobals -> TypedTree.Tycon -> TypedTree.Binding list
    val MakeBindingsForEqualityWithComparerAugmentation:
      TcGlobals.TcGlobals -> TypedTree.Tycon -> TypedTree.Binding list
    val MakeBindingsForEqualsAugmentation:
      TcGlobals.TcGlobals -> TypedTree.Tycon -> TypedTree.Binding list
    val TypeDefinitelyHasEquality:
      TcGlobals.TcGlobals -> TypedTree.TType -> bool


namespace FSharp.Compiler
  module internal NameResolution =
    type NameResolver =
  
        new: g:TcGlobals.TcGlobals * amap:Import.ImportMap *
              infoReader:InfoReader.InfoReader *
              instantiationGenerator:(Range.range -> TypedTree.Typars ->
                                        TypedTree.TypeInst) -> NameResolver
        member InfoReader: InfoReader.InfoReader
        member
          InstantiationGenerator: (Range.range -> TypedTree.Typars ->
                                      TypedTree.TypeInst)
        member amap: Import.ImportMap
        member g: TcGlobals.TcGlobals
        member languageSupportsNameOf: bool
    
    val UnionCaseRefsInTycon:
      modref:TypedTree.ModuleOrNamespaceRef ->
        tycon:TypedTree.Tycon -> TypedTree.UnionCaseRef list
    val UnionCaseRefsInModuleOrNamespace:
      modref:TypedTree.ModuleOrNamespaceRef -> TypedTree.UnionCaseRef list
    val TryFindTypeWithUnionCase:
      modref:TypedTree.ModuleOrNamespaceRef ->
        id:SyntaxTree.Ident -> TypedTree.Entity option
    val TryFindTypeWithRecdField:
      modref:TypedTree.ModuleOrNamespaceRef ->
        id:SyntaxTree.Ident -> TypedTree.Entity option
    val ActivePatternElemsOfValRef:
      vref:TypedTree.ValRef -> TypedTree.ActivePatternElemRef list
    val TryMkValRefInModRef:
      modref:TypedTree.EntityRef ->
        vspec:TypedTree.Val -> TypedTree.ValRef option
    val ActivePatternElemsOfVal:
      modref:TypedTree.EntityRef ->
        vspec:TypedTree.Val -> TypedTree.ActivePatternElemRef list
    val ActivePatternElemsOfModuleOrNamespace:
      TypedTree.ModuleOrNamespaceRef ->
        AbstractIL.Internal.Library.NameMap<TypedTree.ActivePatternElemRef>
    val ( |AbbrevOrAppTy|_| ): TypedTree.TType -> TypedTree.TyconRef option
    [<NoEquality; NoComparison ();
      RequireQualifiedAccessAttribute>]
    type ArgumentContainer =
      | Method of Infos.MethInfo
      | Type of TypedTree.TyconRef
    val emptyTypeInst: TypedTree.TypeInst
    type EnclosingTypeInst = TypedTree.TypeInst
    val emptyEnclosingTypeInst: EnclosingTypeInst
    [<NoEquality; NoComparison ();
      RequireQualifiedAccessAttribute>]
    type Item =
      | Value of TypedTree.ValRef
      | UnionCase of Infos.UnionCaseInfo * hasRequireQualifiedAccessAttr: bool
      | ActivePatternResult of
        PrettyNaming.ActivePatternInfo * TypedTree.TType * int * Range.range
      | ActivePatternCase of TypedTree.ActivePatternElemRef
      | ExnCase of TypedTree.TyconRef
      | RecdField of Infos.RecdFieldInfo
      | UnionCaseField of Infos.UnionCaseInfo * fieldIndex: int
      | AnonRecdField of
        TypedTree.AnonRecdTypeInfo * TypedTree.TTypes * int * Range.range
      | NewDef of SyntaxTree.Ident
      | ILField of Infos.ILFieldInfo
      | Event of Infos.EventInfo
      | Property of string * Infos.PropInfo list
      | MethodGroup of
        displayName: string * methods: Infos.MethInfo list *
        uninstantiatedMethodOpt: Infos.MethInfo option
      | CtorGroup of string * Infos.MethInfo list
      | FakeInterfaceCtor of TypedTree.TType
      | DelegateCtor of TypedTree.TType
      | Types of string * TypedTree.TType list
      | CustomOperation of
        string * (unit -> string option) * Infos.MethInfo option
      | CustomBuilder of string * TypedTree.ValRef
      | TypeVar of string * TypedTree.Typar
      | ModuleOrNamespaces of TypedTree.ModuleOrNamespaceRef list
      | ImplicitOp of SyntaxTree.Ident * TypedTree.TraitConstraintSln option ref
      | ArgName of SyntaxTree.Ident * TypedTree.TType * ArgumentContainer option
      | SetterArg of SyntaxTree.Ident * Item
      | UnqualifiedType of TypedTree.TyconRef list
      with
        static member
          MakeCtorGroup: nm:string * minfos:Infos.MethInfo list -> Item
        static member
          MakeMethGroup: nm:string * minfos:Infos.MethInfo list -> Item
        member DisplayName: string
    
    val valRefHash: vref:TypedTree.ValRef -> int
    [<RequireQualifiedAccessAttribute>]
    type ItemWithInst =
      { Item: Item
        TyparInst: TypedTreeOps.TyparInst }
    val ItemWithNoInst: Item -> ItemWithInst
    val ( |ItemWithInst| ): ItemWithInst -> Item * TypedTreeOps.TyparInst
    type FieldResolution = | FieldResolution of Infos.RecdFieldInfo * bool
    type ExtensionMember =
      | FSExtMem of TypedTree.ValRef * Infos.ExtensionMethodPriority
      | ILExtMem of
        TypedTree.TyconRef * Infos.MethInfo * Infos.ExtensionMethodPriority
      with
        static member
          Comparer: g:TcGlobals.TcGlobals ->
                       System.Collections.Generic.IEqualityComparer<ExtensionMember>
        static member
          Equality: g:TcGlobals.TcGlobals ->
                       e1:ExtensionMember -> e2:ExtensionMember -> bool
        static member Hash: e1:ExtensionMember -> int
        member Priority: Infos.ExtensionMethodPriority
    
    type FullyQualifiedFlag =
      | FullyQualified
      | OpenQualified
    type UnqualifiedItems = AbstractIL.Internal.Library.LayeredMap<string,Item>
    [<NoEquality; NoComparison>]
    type NameResolutionEnv =
      { eDisplayEnv: TypedTreeOps.DisplayEnv
        eUnqualifiedItems: UnqualifiedItems
        eUnqualifiedEnclosingTypeInsts:
          TypedTreeOps.TyconRefMap<EnclosingTypeInst>
        ePatItems: AbstractIL.Internal.Library.NameMap<Item>
        eModulesAndNamespaces:
          AbstractIL.Internal.Library.NameMultiMap<TypedTree.ModuleOrNamespaceRef>
        eFullyQualifiedModulesAndNamespaces:
          AbstractIL.Internal.Library.NameMultiMap<TypedTree.ModuleOrNamespaceRef>
        eFieldLabels:
          AbstractIL.Internal.Library.NameMultiMap<TypedTree.RecdFieldRef>
        eUnqualifiedRecordOrUnionTypeInsts:
          TypedTreeOps.TyconRefMap<TypedTree.TypeInst>
        eTyconsByAccessNames:
          AbstractIL.Internal.Library.LayeredMultiMap<string,TypedTree.TyconRef>
        eFullyQualifiedTyconsByAccessNames:
          AbstractIL.Internal.Library.LayeredMultiMap<string,TypedTree.TyconRef>
        eTyconsByDemangledNameAndArity:
          AbstractIL.Internal.Library.LayeredMap<PrettyNaming.NameArityPair,
                                                 TypedTree.TyconRef>
        eFullyQualifiedTyconsByDemangledNameAndArity:
          AbstractIL.Internal.Library.LayeredMap<PrettyNaming.NameArityPair,
                                                 TypedTree.TyconRef>
        eIndexedExtensionMembers: TypedTreeOps.TyconRefMultiMap<ExtensionMember>
        eUnindexedExtensionMembers: ExtensionMember list
        eTypars: AbstractIL.Internal.Library.NameMap<TypedTree.Typar> }
      with
        static member Empty: g:TcGlobals.TcGlobals -> NameResolutionEnv
        member FindUnqualifiedItem: string -> Item
        member
          ModulesAndNamespaces: fq:FullyQualifiedFlag ->
                                   AbstractIL.Internal.Library.NameMultiMap<TypedTree.ModuleOrNamespaceRef>
        member
          TyconsByAccessNames: fq:FullyQualifiedFlag ->
                                  AbstractIL.Internal.Library.LayeredMultiMap<string,
                                                                              TypedTree.TyconRef>
        member
          TyconsByDemangledNameAndArity: fq:FullyQualifiedFlag ->
                                            AbstractIL.Internal.Library.LayeredMap<PrettyNaming.NameArityPair,
                                                                                   TypedTree.TyconRef>
        member DisplayEnv: TypedTreeOps.DisplayEnv
    
    [<RequireQualifiedAccessAttribute>]
    type ResultCollectionSettings =
      | AllResults
      | AtMostOneResult
    val NextExtensionMethodPriority: unit -> uint64
    val IsTyconRefUsedForCSharpStyleExtensionMembers:
      g:TcGlobals.TcGlobals -> m:Range.range -> tcref:TypedTree.TyconRef -> bool
    val IsTypeUsedForCSharpStyleExtensionMembers:
      g:TcGlobals.TcGlobals -> m:Range.range -> ty:TypedTree.TType -> bool
    val IsMethInfoPlainCSharpStyleExtensionMember:
      g:TcGlobals.TcGlobals ->
        m:Range.range -> isEnclExtTy:bool -> minfo:Infos.MethInfo -> bool
    val private GetCSharpStyleIndexedExtensionMembersForTyconRef:
      amap:Import.ImportMap ->
        m:Range.range ->
          tcrefOfStaticClass:TypedTree.TyconRef ->
            Choice<(TypedTree.TyconRef * ExtensionMember),ExtensionMember> list
    val IntrinsicPropInfosOfTypeInScope:
      infoReader:InfoReader.InfoReader ->
        optFilter:string option ->
          ad:AccessibilityLogic.AccessorDomain ->
            findFlag:InfoReader.FindMemberFlag ->
              m:Range.range -> ty:TypedTree.TType -> Infos.PropInfo list
    val SelectPropInfosFromExtMembers:
      infoReader:InfoReader.InfoReader ->
        ad:AccessibilityLogic.AccessorDomain ->
          optFilter:string option ->
            declaringTy:TypedTree.TType ->
              m:Range.range ->
                extMemInfos:seq<ExtensionMember> -> Infos.PropInfo list
    val ExtensionPropInfosOfTypeInScope:
      ResultCollectionSettings ->
        InfoReader.InfoReader ->
          NameResolutionEnv ->
            string option ->
              AccessibilityLogic.AccessorDomain ->
                Range.range -> TypedTree.TType -> Infos.PropInfo list
    val AllPropInfosOfTypeInScope:
      ResultCollectionSettings ->
        InfoReader.InfoReader ->
          NameResolutionEnv ->
            string option ->
              AccessibilityLogic.AccessorDomain ->
                InfoReader.FindMemberFlag ->
                  Range.range -> TypedTree.TType -> Infos.PropInfo list
    val IntrinsicMethInfosOfType:
      infoReader:InfoReader.InfoReader ->
        optFilter:string option ->
          ad:AccessibilityLogic.AccessorDomain ->
            allowMultiIntfInst:Infos.AllowMultiIntfInstantiations ->
              findFlag:InfoReader.FindMemberFlag ->
                m:Range.range -> ty:TypedTree.TType -> Infos.MethInfo list
    val TrySelectExtensionMethInfoOfILExtMem:
      Range.range ->
        Import.ImportMap ->
          TypedTree.TType ->
            TypedTree.TyconRef * Infos.MethInfo * Infos.ExtensionMethodPriority ->
              Infos.MethInfo option
    val SelectMethInfosFromExtMembers:
      infoReader:InfoReader.InfoReader ->
        optFilter:string option ->
          apparentTy:TypedTree.TType ->
            m:Range.range ->
              extMemInfos:seq<ExtensionMember> -> Infos.MethInfo list
    val ExtensionMethInfosOfTypeInScope:
      collectionSettings:ResultCollectionSettings ->
        infoReader:InfoReader.InfoReader ->
          nenv:NameResolutionEnv ->
            optFilter:string option ->
              m:Range.range -> ty:TypedTree.TType -> Infos.MethInfo list
    val AllMethInfosOfTypeInScope:
      ResultCollectionSettings ->
        InfoReader.InfoReader ->
          NameResolutionEnv ->
            string option ->
              AccessibilityLogic.AccessorDomain ->
                InfoReader.FindMemberFlag ->
                  Range.range -> TypedTree.TType -> Infos.MethInfo list
    [<RequireQualifiedAccessAttribute>]
    type BulkAdd =
      | Yes
      | No
    val AddValRefsToItems:
      bulkAddMode:BulkAdd ->
        eUnqualifiedItems:UnqualifiedItems ->
          vrefs:TypedTree.ValRef [] -> UnqualifiedItems
    val AddValRefToExtensionMembers:
      pri:Infos.ExtensionMethodPriority ->
        eIndexedExtensionMembers:TypedTreeOps.TyconRefMultiMap<ExtensionMember> ->
          vref:TypedTree.ValRef ->
            TypedTreeOps.TyconRefMultiMap<ExtensionMember>
    val AddFakeNamedValRefToNameEnv:
      string -> NameResolutionEnv -> TypedTree.ValRef -> NameResolutionEnv
    val AddFakeNameToNameEnv:
      string -> NameResolutionEnv -> Item -> NameResolutionEnv
    val AddValRefsToActivePatternsNameEnv:
      ePatItems:Map<string,Item> -> vref:TypedTree.ValRef -> Map<string,Item>
    val AddValRefsToNameEnvWithPriority:
      bulkAddMode:BulkAdd ->
        pri:Infos.ExtensionMethodPriority ->
          nenv:NameResolutionEnv ->
            vrefs:TypedTree.ValRef [] -> NameResolutionEnv
    val AddValRefToNameEnv:
      NameResolutionEnv -> TypedTree.ValRef -> NameResolutionEnv
    val AddActivePatternResultTagsToNameEnv:
      PrettyNaming.ActivePatternInfo ->
        NameResolutionEnv -> TypedTree.TType -> Range.range -> NameResolutionEnv
    val GeneralizeUnionCaseRef:
      ucref:TypedTree.UnionCaseRef -> Infos.UnionCaseInfo
    val AddTyconsByDemangledNameAndArity:
      bulkAddMode:BulkAdd ->
        tcrefs:TypedTree.TyconRef [] ->
          tab:AbstractIL.Internal.Library.LayeredMap<PrettyNaming.NameArityPair,
                                                     TypedTree.TyconRef> ->
            AbstractIL.Internal.Library.LayeredMap<PrettyNaming.NameArityPair,
                                                   TypedTree.TyconRef>
    val AddTyconByAccessNames:
      bulkAddMode:BulkAdd ->
        tcrefs:TypedTree.TyconRef [] ->
          tab:AbstractIL.Internal.Library.LayeredMultiMap<string,
                                                          TypedTree.TyconRef> ->
            AbstractIL.Internal.Library.LayeredMultiMap<string,
                                                        TypedTree.TyconRef>
    val AddRecdField:
      rfref:TypedTree.RecdFieldRef ->
        tab:AbstractIL.Internal.Library.NameMultiMap<TypedTree.RecdFieldRef> ->
          Map<string,TypedTree.RecdFieldRef list>
    val AddUnionCases1:
      tab:Map<string,Item> ->
        ucrefs:TypedTree.UnionCaseRef list -> Map<string,Item>
    val AddUnionCases2:
      bulkAddMode:BulkAdd ->
        eUnqualifiedItems:UnqualifiedItems ->
          ucrefs:TypedTree.UnionCaseRef list -> Map<string,Item>
    type TypeNameResolutionFlag =
      | ResolveTypeNamesToCtors
      | ResolveTypeNamesToTypeRefs
    [<SealedAttribute (); NoEquality; NoComparison ();
      RequireQualifiedAccessAttribute>]
    type TypeNameResolutionStaticArgsInfo =
      | Indefinite
      | Definite of int
      with
        static member
          FromTyArgs: numTyArgs:int -> TypeNameResolutionStaticArgsInfo
        member MangledNameForType: nm:string -> string
        member HasNoStaticArgsInfo: bool
        member NumStaticArgs: int
        static member DefiniteEmpty: TypeNameResolutionStaticArgsInfo
    
    [<NoEquality; NoComparison>]
    type TypeNameResolutionInfo =
      | TypeNameResolutionInfo of
        TypeNameResolutionFlag * TypeNameResolutionStaticArgsInfo
      with
        static member
          ResolveToTypeRefs: TypeNameResolutionStaticArgsInfo ->
                                TypeNameResolutionInfo
        member DropStaticArgsInfo: TypeNameResolutionInfo
        member ResolutionFlag: TypeNameResolutionFlag
        member StaticArgsInfo: TypeNameResolutionStaticArgsInfo
        static member Default: TypeNameResolutionInfo
    
    [<RequireQualifiedAccessAttribute>]
    type PermitDirectReferenceToGeneratedType =
      | Yes
      | No
    val CheckForDirectReferenceToGeneratedType:
      tcref:TypedTree.TyconRef * genOk:PermitDirectReferenceToGeneratedType *
      m:Range.range -> unit
    val AddEntityForProvidedType:
      amap:Import.ImportMap * modref:TypedTree.ModuleOrNamespaceRef *
      resolutionEnvironment:ExtensionTyping.ResolutionEnvironment *
      st:Tainted<ExtensionTyping.ProvidedType> * m:Range.range ->
        TypedTree.EntityRef
    val ResolveProvidedTypeNameInEntity:
      amap:Import.ImportMap * m:Range.range * typeName:string *
      modref:TypedTree.ModuleOrNamespaceRef -> TypedTree.EntityRef list
    val LookupTypeNameInEntityHaveArity:
      nm:string ->
        staticResInfo:TypeNameResolutionStaticArgsInfo ->
          mty:TypedTree.ModuleOrNamespaceType -> TypedTree.Tycon option
    val LookupTypeNameNoArity:
      nm:string ->
        byDemangledNameAndArity:AbstractIL.Internal.Library.LayeredMap<PrettyNaming.NameArityPair,
                                                                       'a> ->
          byAccessNames:AbstractIL.Internal.Library.LayeredMultiMap<string,'a> ->
            'a list
    val LookupTypeNameInEntityNoArity:
      _m:'a ->
        nm:string ->
          mtyp:TypedTree.ModuleOrNamespaceType -> TypedTree.Tycon list
    val LookupTypeNameInEntityMaybeHaveArity:
      amap:Import.ImportMap * m:Range.range *
      ad:AccessibilityLogic.AccessorDomain * nm:string *
      staticResInfo:TypeNameResolutionStaticArgsInfo *
      modref:TypedTree.ModuleOrNamespaceRef -> TypedTree.EntityRef list
    val GetNestedTyconRefsOfType:
      infoReader:InfoReader.InfoReader ->
        amap:Import.ImportMap ->
          ad:AccessibilityLogic.AccessorDomain * optFilter:string option *
          staticResInfo:TypeNameResolutionStaticArgsInfo *
          checkForGenerated:bool * m:Range.range ->
            ty:TypedTree.TType -> TypedTree.TypeInst * TypedTree.EntityRef list
    val MakeNestedType:
      ncenv:NameResolver ->
        tinst:TypedTree.TType list ->
          m:Range.range -> tcrefNested:TypedTree.TyconRef -> TypedTree.TType
    val GetNestedTypesOfType:
      ad:AccessibilityLogic.AccessorDomain * ncenv:NameResolver *
      optFilter:string option * staticResInfo:TypeNameResolutionStaticArgsInfo *
      checkForGenerated:bool * m:Range.range ->
        ty:TypedTree.TType -> TypedTree.TType list
    val ChooseMethInfosForNameEnv:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          ty:TypedTree.TType ->
            minfos:Infos.MethInfo list ->
              System.Collections.Generic.KeyValuePair<string,Item> list
    val ChoosePropInfosForNameEnv:
      g:TcGlobals.TcGlobals ->
        ty:TypedTree.TType ->
          pinfos:Infos.PropInfo list ->
            System.Collections.Generic.KeyValuePair<string,Item> list
    val ChooseFSharpFieldInfosForNameEnv:
      g:TcGlobals.TcGlobals ->
        ty:TypedTree.TType ->
          rfinfos:Infos.RecdFieldInfo list ->
            System.Collections.Generic.KeyValuePair<string,Item> list
    val ChooseILFieldInfosForNameEnv:
      g:TcGlobals.TcGlobals ->
        ty:TypedTree.TType ->
          finfos:Infos.ILFieldInfo list ->
            System.Collections.Generic.KeyValuePair<string,Item> list
    val ChooseEventInfosForNameEnv:
      g:TcGlobals.TcGlobals ->
        ty:TypedTree.TType ->
          einfos:Infos.EventInfo list ->
            System.Collections.Generic.KeyValuePair<string,Item> list
    val AddStaticContentOfTypeToNameEnv:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          ad:AccessibilityLogic.AccessorDomain ->
            m:Range.range ->
              nenv:NameResolutionEnv -> ty:TypedTree.TType -> NameResolutionEnv
    val private AddNestedTypesOfTypeToNameEnv:
      infoReader:InfoReader.InfoReader ->
        amap:Import.ImportMap ->
          ad:AccessibilityLogic.AccessorDomain ->
            m:Range.range ->
              nenv:NameResolutionEnv -> ty:TypedTree.TType -> NameResolutionEnv
    val private AddTyconRefsWithEnclosingTypeInstToNameEnv:
      bulkAddMode:BulkAdd ->
        ownDefinition:bool ->
          g:TcGlobals.TcGlobals ->
            amap:Import.ImportMap ->
              ad:AccessibilityLogic.AccessorDomain ->
                m:Range.range ->
                  root:bool ->
                    nenv:NameResolutionEnv ->
                      tinstEnclosing:TypedTree.TypeInst *
                      tcrefs:TypedTree.TyconRef list -> NameResolutionEnv
    val private AddStaticPartsOfTypeToNameEnv:
      amap:Import.ImportMap ->
        m:Range.range ->
          nenv:NameResolutionEnv -> ty:TypedTree.TType -> NameResolutionEnv
    val private AddStaticPartsOfTyconRefToNameEnv:
      bulkAddMode:BulkAdd ->
        ownDefinition:bool ->
          g:TcGlobals.TcGlobals ->
            amap:Import.ImportMap ->
              m:Range.range ->
                nenv:NameResolutionEnv ->
                  tinstOpt:TypedTree.TypeInst option ->
                    tcref:TypedTree.TyconRef -> NameResolutionEnv
    val private CanAutoOpenTyconRef:
      g:TcGlobals.TcGlobals -> m:Range.range -> tcref:TypedTree.TyconRef -> bool
    val private AddPartsOfTyconRefToNameEnv:
      bulkAddMode:BulkAdd ->
        ownDefinition:bool ->
          g:TcGlobals.TcGlobals ->
            amap:Import.ImportMap ->
              ad:AccessibilityLogic.AccessorDomain ->
                m:Range.range ->
                  nenv:NameResolutionEnv ->
                    tcref:TypedTree.TyconRef -> NameResolutionEnv
    val AddTyconRefsToNameEnv:
      BulkAdd ->
        bool ->
          TcGlobals.TcGlobals ->
            Import.ImportMap ->
              AccessibilityLogic.AccessorDomain ->
                Range.range ->
                  bool ->
                    NameResolutionEnv ->
                      TypedTree.TyconRef list -> NameResolutionEnv
    val AddExceptionDeclsToNameEnv:
      BulkAdd -> NameResolutionEnv -> TypedTree.TyconRef -> NameResolutionEnv
    val AddModuleAbbrevToNameEnv:
      SyntaxTree.Ident ->
        NameResolutionEnv ->
          TypedTree.ModuleOrNamespaceRef list -> NameResolutionEnv
    val MakeNestedModuleRefs:
      modref:TypedTree.ModuleOrNamespaceRef -> TypedTree.EntityRef list
    val AddModuleOrNamespaceRefsToNameEnv:
      TcGlobals.TcGlobals ->
        Import.ImportMap ->
          Range.range ->
            bool ->
              AccessibilityLogic.AccessorDomain ->
                NameResolutionEnv ->
                  TypedTree.ModuleOrNamespaceRef list -> NameResolutionEnv
    val AddModuleOrNamespaceContentsToNameEnv:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          ad:AccessibilityLogic.AccessorDomain ->
            m:Range.range ->
              root:bool ->
                nenv:NameResolutionEnv ->
                  modref:TypedTree.ModuleOrNamespaceRef -> NameResolutionEnv
    val AddModuleOrNamespaceRefsContentsToNameEnv:
      TcGlobals.TcGlobals ->
        Import.ImportMap ->
          AccessibilityLogic.AccessorDomain ->
            Range.range ->
              bool ->
                NameResolutionEnv ->
                  TypedTree.EntityRef list -> NameResolutionEnv
    val AddTypeContentsToNameEnv:
      TcGlobals.TcGlobals ->
        Import.ImportMap ->
          AccessibilityLogic.AccessorDomain ->
            Range.range ->
              NameResolutionEnv -> TypedTree.TType -> NameResolutionEnv
    val AddModuleOrNamespaceRefContentsToNameEnv:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          ad:AccessibilityLogic.AccessorDomain ->
            m:Range.range ->
              root:bool ->
                nenv:NameResolutionEnv ->
                  modref:TypedTree.EntityRef -> NameResolutionEnv
    val AddModuleOrNamespaceRefToNameEnv:
      TcGlobals.TcGlobals ->
        Import.ImportMap ->
          Range.range ->
            bool ->
              AccessibilityLogic.AccessorDomain ->
                NameResolutionEnv -> TypedTree.EntityRef -> NameResolutionEnv
    type CheckForDuplicateTyparFlag =
      | CheckForDuplicateTypars
      | NoCheckForDuplicateTypars
    val AddDeclaredTyparsToNameEnv:
      CheckForDuplicateTyparFlag ->
        NameResolutionEnv -> TypedTree.Typar list -> NameResolutionEnv
    val FreshenTycon:
      ncenv:NameResolver ->
        m:Range.range -> tcref:TypedTree.TyconRef -> TypedTree.TType
    val FreshenTyconWithEnclosingTypeInst:
      ncenv:NameResolver ->
        m:Range.range ->
          tinstEnclosing:TypedTree.TypeInst ->
            tcref:TypedTree.TyconRef -> TypedTree.TType
    val FreshenUnionCaseRef:
      ncenv:NameResolver ->
        m:Range.range -> ucref:TypedTree.UnionCaseRef -> Infos.UnionCaseInfo
    val FreshenRecdFieldRef:
      NameResolver ->
        Range.range -> TypedTree.RecdFieldRef -> Infos.RecdFieldInfo
    val ResolveUnqualifiedItem:
      ncenv:NameResolver ->
        nenv:NameResolutionEnv -> m:Range.range -> res:Item -> Item
    val OneResult:
      res:AbstractIL.Internal.Library.ResultOrException<'a> ->
        AbstractIL.Internal.Library.ResultOrException<'a list>
    val OneSuccess:
      x:'a -> AbstractIL.Internal.Library.ResultOrException<'a list>
    val AddResults:
      res1:AbstractIL.Internal.Library.ResultOrException<'a list> ->
        res2:AbstractIL.Internal.Library.ResultOrException<'a list> ->
          AbstractIL.Internal.Library.ResultOrException<'a list>
    val NoResultsOrUsefulErrors:
      AbstractIL.Internal.Library.ResultOrException<'a list>
    val CollectResults:
      f:('a -> AbstractIL.Internal.Library.ResultOrException<'b>) ->
        _arg1:'a list -> AbstractIL.Internal.Library.ResultOrException<'b list>
    val CollectAtMostOneResult:
      f:('a -> AbstractIL.Internal.Library.ResultOrException<'b>) ->
        inputs:'a list -> AbstractIL.Internal.Library.ResultOrException<'b list>
    val CollectResults2:
      resultCollectionSettings:ResultCollectionSettings ->
        f:('a -> AbstractIL.Internal.Library.ResultOrException<'b>) ->
          ('a list -> AbstractIL.Internal.Library.ResultOrException<'b list>)
    val MapResults:
      f:('a -> 'b) ->
        _arg1:AbstractIL.Internal.Library.ResultOrException<'a list> ->
          AbstractIL.Internal.Library.ResultOrException<'b list>
    val AtMostOneResult:
      m:Range.range ->
        res:AbstractIL.Internal.Library.ResultOrException<'a list> ->
          AbstractIL.Internal.Library.ResultOrException<'a>
    val AtMostOneResultQuery:
      query2:(unit -> AbstractIL.Internal.Library.ResultOrException<'a list>) ->
        res1:AbstractIL.Internal.Library.ResultOrException<'a list> ->
          AbstractIL.Internal.Library.ResultOrException<'a list>
    val inline ( +++ ):
      res1:AbstractIL.Internal.Library.ResultOrException<'a list> ->
        query2:(unit -> AbstractIL.Internal.Library.ResultOrException<'a list>) ->
          AbstractIL.Internal.Library.ResultOrException<'a list>
    val LookupTypeNameInEnvHaveArity:
      fq:FullyQualifiedFlag ->
        nm:string ->
          numTyArgs:int -> nenv:NameResolutionEnv -> TypedTree.TyconRef option
    val LookupTypeNameInEnvNoArity:
      FullyQualifiedFlag ->
        string -> NameResolutionEnv -> TypedTree.TyconRef list
    val LookupTypeNameInEnvMaybeHaveArity:
      fq:FullyQualifiedFlag ->
        nm:string ->
          typeNameResInfo:TypeNameResolutionInfo ->
            nenv:NameResolutionEnv -> TypedTree.TyconRef list
    [<RequireQualifiedAccessAttribute (); StructAttribute>]
    type ItemOccurence =
      | Binding
      | Use
      | UseInType
      | UseInAttribute
      | Pattern
      | Implemented
      | RelatedText
      | Open
    type OpenDeclaration =
      { Target: SyntaxTree.SynOpenDeclTarget
        Range: Range.range option
        Modules: TypedTree.ModuleOrNamespaceRef list
        Types: TypedTree.TType list
        AppliedScope: Range.range
        IsOwnNamespace: bool }
      with
        static member
          Create: target:SyntaxTree.SynOpenDeclTarget *
                   modules:TypedTree.ModuleOrNamespaceRef list *
                   types:TypedTree.TType list * appliedScope:Range.range *
                   isOwnNamespace:bool -> OpenDeclaration
    
    type FormatStringCheckContext =
      { SourceText: Text.ISourceText
        LineStartPositions: int [] }
    type ITypecheckResultsSink =
      interface
        abstract member
          NotifyEnvWithScope: Range.range * NameResolutionEnv *
                               AccessibilityLogic.AccessorDomain -> unit
        abstract member
          NotifyExprHasType: TypedTree.TType * NameResolutionEnv *
                              AccessibilityLogic.AccessorDomain * Range.range ->
                                unit
        abstract member
          NotifyFormatSpecifierLocation: Range.range * int -> unit
        abstract member
          NotifyMethodGroupNameResolution: Range.pos * Item * Item *
                                            TypedTreeOps.TyparInst *
                                            ItemOccurence * NameResolutionEnv *
                                            AccessibilityLogic.AccessorDomain *
                                            Range.range * bool -> unit
        abstract member
          NotifyNameResolution: Range.pos * Item * TypedTreeOps.TyparInst *
                                 ItemOccurence * NameResolutionEnv *
                                 AccessibilityLogic.AccessorDomain * Range.range *
                                 bool -> unit
        abstract member NotifyOpenDeclaration: OpenDeclaration -> unit
        abstract member CurrentSourceText: Text.ISourceText option
        abstract member
          FormatStringCheckContext: FormatStringCheckContext option
    
    val ( |ValRefOfProp|_| ): pi:Infos.PropInfo -> TypedTree.ValRef option
    val ( |ValRefOfMeth|_| ): mi:Infos.MethInfo -> TypedTree.ValRef option
    val ( |ValRefOfEvent|_| ): evt:Infos.EventInfo -> TypedTree.ValRef option
    val ( |RecordFieldUse|_| ):
      item:Item -> (string * TypedTree.TyconRef) option
    val ( |UnionCaseFieldUse|_| ):
      item:Item -> (int * TypedTree.UnionCaseRef) option
    val ( |ILFieldUse|_| ): item:Item -> Infos.ILFieldInfo option
    val ( |PropertyUse|_| ): item:Item -> Infos.PropInfo option
    val ( |FSharpPropertyUse|_| ): item:Item -> TypedTree.ValRef option
    val ( |MethodUse|_| ): item:Item -> Infos.MethInfo option
    val ( |FSharpMethodUse|_| ): item:Item -> TypedTree.ValRef option
    val ( |EntityUse|_| ): item:Item -> TypedTree.TyconRef option
    val ( |EventUse|_| ): item:Item -> Infos.EventInfo option
    val ( |FSharpEventUse|_| ): item:Item -> TypedTree.ValRef option
    val ( |UnionCaseUse|_| ): item:Item -> TypedTree.UnionCaseRef option
    val ( |ValUse|_| ): item:Item -> TypedTree.ValRef option
    val ( |ActivePatternCaseUse|_| ):
      item:Item -> (Range.range * Range.range * int) option
    val tyconRefDefnHash:
      _g:TcGlobals.TcGlobals -> eref1:TypedTree.EntityRef -> int
    val tyconRefDefnEq:
      g:TcGlobals.TcGlobals ->
        eref1:TypedTree.EntityRef -> eref2:TypedTree.EntityRef -> bool
    val valRefDefnHash: _g:TcGlobals.TcGlobals -> vref1:TypedTree.ValRef -> int
    val valRefDefnEq:
      g:TcGlobals.TcGlobals ->
        vref1:TypedTree.ValRef -> vref2:TypedTree.ValRef -> bool
    val unionCaseRefDefnEq:
      g:TcGlobals.TcGlobals ->
        uc1:TypedTree.UnionCaseRef -> uc2:TypedTree.UnionCaseRef -> bool
    val ItemsAreEffectivelyEqual: TcGlobals.TcGlobals -> Item -> Item -> bool
    val ItemsAreEffectivelyEqualHash: TcGlobals.TcGlobals -> Item -> int
    [<ClassAttribute ();
      System.Diagnostics.DebuggerDisplay ("{DebugToString()}")>]
    type CapturedNameResolution =
  
        new: i:Item * tpinst:TypedTreeOps.TyparInst * io:ItemOccurence *
              nre:NameResolutionEnv * ad:AccessibilityLogic.AccessorDomain *
              m:Range.range -> CapturedNameResolution
        member DebugToString: unit -> string
        member AccessorDomain: AccessibilityLogic.AccessorDomain
        member DisplayEnv: TypedTreeOps.DisplayEnv
        member Item: Item
        member ItemOccurence: ItemOccurence
        member ItemWithInst: ItemWithInst
        member NameResolutionEnv: NameResolutionEnv
        member Pos: Range.pos
        member Range: Range.range
    
    [<ClassAttribute>]
    type TcResolutions =
  
        new: capturedEnvs:ResizeArray<Range.range * NameResolutionEnv *
                                       AccessibilityLogic.AccessorDomain> *
              capturedExprTypes:ResizeArray<TypedTree.TType * NameResolutionEnv *
                                            AccessibilityLogic.AccessorDomain *
                                            Range.range> *
              capturedNameResolutions:ResizeArray<CapturedNameResolution> *
              capturedMethodGroupResolutions:ResizeArray<CapturedNameResolution> ->
                TcResolutions
        member
          CapturedEnvs: ResizeArray<Range.range * NameResolutionEnv *
                                     AccessibilityLogic.AccessorDomain>
        member
          CapturedExpressionTypings: ResizeArray<TypedTree.TType *
                                                  NameResolutionEnv *
                                                  AccessibilityLogic.AccessorDomain *
                                                  Range.range>
        member
          CapturedMethodGroupResolutions: ResizeArray<CapturedNameResolution>
        member CapturedNameResolutions: ResizeArray<CapturedNameResolution>
        static member Empty: TcResolutions
    
    [<StructAttribute>]
    type TcSymbolUseData =
      { Item: Item
        ItemOccurence: ItemOccurence
        DisplayEnv: TypedTreeOps.DisplayEnv
        Range: Range.range }
    [<ClassAttribute>]
    type TcSymbolUses =
  
        new: g:TcGlobals.TcGlobals *
              capturedNameResolutions:ResizeArray<CapturedNameResolution> *
              formatSpecifierLocations:(Range.range * int) [] -> TcSymbolUses
        member
          GetFormatSpecifierLocationsAndArity: unit -> (Range.range * int) []
        member GetUsesOfSymbol: Item -> TcSymbolUseData []
        member AllUsesOfSymbols: TcSymbolUseData [] []
        static member Empty: TcSymbolUses
    
    type TcResultsSinkImpl =
  
        interface ITypecheckResultsSink
        new: tcGlobals:TcGlobals.TcGlobals * ?sourceText:Text.ISourceText ->
                TcResultsSinkImpl
        member GetFormatSpecifierLocations: unit -> (Range.range * int) []
        member GetOpenDeclarations: unit -> OpenDeclaration []
        member GetResolutions: unit -> TcResolutions
        member GetSymbolUses: unit -> TcSymbolUses
    
    type TcResultsSink =
      { mutable CurrentSink: ITypecheckResultsSink option }
      with
        static member WithSink: ITypecheckResultsSink -> TcResultsSink
        static member NoSink: TcResultsSink
    
    val WithNewTypecheckResultsSink:
      ITypecheckResultsSink * TcResultsSink -> System.IDisposable
    val TemporarilySuspendReportingTypecheckResultsToSink:
      TcResultsSink -> System.IDisposable
    val CallEnvSink:
      TcResultsSink ->
        Range.range * NameResolutionEnv * AccessibilityLogic.AccessorDomain ->
          unit
    val CallNameResolutionSink:
      TcResultsSink ->
        Range.range * NameResolutionEnv * Item * TypedTreeOps.TyparInst *
        ItemOccurence * AccessibilityLogic.AccessorDomain -> unit
    val CallMethodGroupNameResolutionSink:
      TcResultsSink ->
        Range.range * NameResolutionEnv * Item * Item * TypedTreeOps.TyparInst *
        ItemOccurence * AccessibilityLogic.AccessorDomain -> unit
    val CallNameResolutionSinkReplacing:
      TcResultsSink ->
        Range.range * NameResolutionEnv * Item * TypedTreeOps.TyparInst *
        ItemOccurence * AccessibilityLogic.AccessorDomain -> unit
    val CallExprHasTypeSink:
      TcResultsSink ->
        Range.range * NameResolutionEnv * TypedTree.TType *
        AccessibilityLogic.AccessorDomain -> unit
    val CallOpenDeclarationSink: TcResultsSink -> OpenDeclaration -> unit
    type ResultTyparChecker = | ResultTyparChecker of (unit -> bool)
    val CheckAllTyparsInferrable:
      amap:Import.ImportMap -> m:Range.range -> item:Item -> bool
    type ResolutionInfo =
      | ResolutionInfo of
        (Range.range * TypedTree.EntityRef) list * (ResultTyparChecker -> unit) *
        tinstEnclosing: EnclosingTypeInst
      with
        static member
          SendEntityPathToSink: sink:TcResultsSink * ncenv:NameResolver *
                                 nenv:NameResolutionEnv * occ:ItemOccurence *
                                 ad:AccessibilityLogic.AccessorDomain *
                                 ResolutionInfo *
                                 typarChecker:ResultTyparChecker -> unit
        member
          AddEntity: info:(Range.range * TypedTree.EntityRef) -> ResolutionInfo
        member AddWarning: f:(ResultTyparChecker -> unit) -> ResolutionInfo
        member
          WithEnclosingTypeInst: tinstEnclosing:EnclosingTypeInst ->
                                    ResolutionInfo
        member EnclosingTypeInst: EnclosingTypeInst
        static member Empty: ResolutionInfo
    
    val CheckForTypeLegitimacyAndMultipleGenericTypeAmbiguities:
      tcrefs:(ResolutionInfo * TypedTree.TyconRef) list *
      typeNameResInfo:TypeNameResolutionInfo *
      genOk:PermitDirectReferenceToGeneratedType * m:Range.range ->
        (ResolutionInfo * TypedTree.TyconRef) list
    val ResolveLongIdentAsModuleOrNamespace:
      TcResultsSink ->
        ResultCollectionSettings ->
          Import.ImportMap ->
            Range.range ->
              first:bool ->
                FullyQualifiedFlag ->
                  NameResolutionEnv ->
                    AccessibilityLogic.AccessorDomain ->
                      SyntaxTree.Ident ->
                        SyntaxTree.Ident list ->
                          isOpenDecl:bool ->
                            AbstractIL.Internal.Library.ResultOrException<(int *
                                                                           TypedTree.ModuleOrNamespaceRef *
                                                                           TypedTree.ModuleOrNamespaceType) list>
    val ResolveLongIdentAsModuleOrNamespaceThen:
      sink:TcResultsSink ->
        atMostOne:ResultCollectionSettings ->
          amap:Import.ImportMap ->
            m:Range.range ->
              fullyQualified:FullyQualifiedFlag ->
                nenv:NameResolutionEnv ->
                  ad:AccessibilityLogic.AccessorDomain ->
                    id:SyntaxTree.Ident ->
                      rest:SyntaxTree.Ident list ->
                        isOpenDecl:bool ->
                          f:(ResolutionInfo -> int -> Range.range ->
                               TypedTree.ModuleOrNamespaceRef ->
                               TypedTree.ModuleOrNamespaceType ->
                               SyntaxTree.Ident -> SyntaxTree.Ident list ->
                               AbstractIL.Internal.Library.ResultOrException<'a>) ->
                            AbstractIL.Internal.Library.ResultOrException<'a list>
    val private ResolveObjectConstructorPrim:
      ncenv:NameResolver ->
        edenv:TypedTreeOps.DisplayEnv ->
          resInfo:'a ->
            m:Range.range ->
              ad:AccessibilityLogic.AccessorDomain ->
                ty:TypedTree.TType ->
                  AbstractIL.Internal.Library.ResultOrException<'a * Item>
    val ResolveObjectConstructor:
      NameResolver ->
        TypedTreeOps.DisplayEnv ->
          Range.range ->
            AccessibilityLogic.AccessorDomain ->
              TypedTree.TType ->
                AbstractIL.Internal.Library.ResultOrException<Item>
    exception IndeterminateType of Range.range
    [<RequireQualifiedAccessAttribute>]
    type LookupKind =
      | RecdField
      | Pattern
      | Expr
      | Type
      | Ctor
    val TryFindUnionCaseOfType:
      g:TcGlobals.TcGlobals ->
        ty:TypedTree.TType -> nm:string -> Infos.UnionCaseInfo voption
    val TryFindAnonRecdFieldOfType:
      TcGlobals.TcGlobals -> TypedTree.TType -> string -> Item option
    val CoreDisplayName: pinfo:Infos.PropInfo -> string
    val DecodeFSharpEvent:
      pinfos:Infos.PropInfo list ->
        ad:AccessibilityLogic.AccessorDomain ->
          g:TcGlobals.TcGlobals ->
            ncenv:NameResolver -> m:Range.range -> Item option
    val GetRecordLabelsForType:
      g:TcGlobals.TcGlobals ->
        nenv:NameResolutionEnv ->
          ty:TypedTree.TType -> System.Collections.Generic.HashSet<string>
    val CheckNestedTypesOfType:
      ncenv:NameResolver ->
        resInfo:ResolutionInfo ->
          ad:AccessibilityLogic.AccessorDomain ->
            nm:string ->
              typeNameResInfo:TypeNameResolutionInfo ->
                m:Range.range -> ty:TypedTree.TType -> TypedTree.TType list
    val ResolveLongIdentInTypePrim:
      ncenv:NameResolver ->
        nenv:NameResolutionEnv ->
          lookupKind:LookupKind ->
            resInfo:ResolutionInfo ->
              depth:int ->
                m:Range.range ->
                  ad:AccessibilityLogic.AccessorDomain ->
                    id:SyntaxTree.Ident ->
                      rest:SyntaxTree.Ident list ->
                        findFlag:InfoReader.FindMemberFlag ->
                          typeNameResInfo:TypeNameResolutionInfo ->
                            ty:TypedTree.TType ->
                              AbstractIL.Internal.Library.ResultOrException<(ResolutionInfo *
                                                                             Item *
                                                                             SyntaxTree.Ident list) list>
    val ResolveLongIdentInNestedTypes:
      ncenv:NameResolver ->
        nenv:NameResolutionEnv ->
          lookupKind:LookupKind ->
            resInfo:ResolutionInfo ->
              depth:int ->
                id:SyntaxTree.Ident ->
                  m:Range.range ->
                    ad:AccessibilityLogic.AccessorDomain ->
                      id2:SyntaxTree.Ident ->
                        rest:SyntaxTree.Ident list ->
                          findFlag:InfoReader.FindMemberFlag ->
                            typeNameResInfo:TypeNameResolutionInfo ->
                              tys:TypedTree.TType list ->
                                AbstractIL.Internal.Library.ResultOrException<(ResolutionInfo *
                                                                               Item *
                                                                               SyntaxTree.Ident list) list>
    val ResolveLongIdentInType:
      TcResultsSink ->
        NameResolver ->
          NameResolutionEnv ->
            LookupKind ->
              Range.range ->
                AccessibilityLogic.AccessorDomain ->
                  SyntaxTree.Ident ->
                    InfoReader.FindMemberFlag ->
                      TypeNameResolutionInfo ->
                        TypedTree.TType -> Item * SyntaxTree.Ident list
    val private ResolveLongIdentInTyconRef:
      ncenv:NameResolver ->
        nenv:NameResolutionEnv ->
          lookupKind:LookupKind ->
            resInfo:ResolutionInfo ->
              depth:int ->
                m:Range.range ->
                  ad:AccessibilityLogic.AccessorDomain ->
                    id:SyntaxTree.Ident ->
                      rest:SyntaxTree.Ident list ->
                        typeNameResInfo:TypeNameResolutionInfo ->
                          tcref:TypedTree.TyconRef ->
                            AbstractIL.Internal.Library.ResultOrException<(ResolutionInfo *
                                                                           Item *
                                                                           SyntaxTree.Ident list) list>
    val private ResolveLongIdentInTyconRefs:
      atMostOne:ResultCollectionSettings ->
        ncenv:NameResolver ->
          nenv:NameResolutionEnv ->
            lookupKind:LookupKind ->
              depth:int ->
                m:Range.range ->
                  ad:AccessibilityLogic.AccessorDomain ->
                    id:SyntaxTree.Ident ->
                      rest:SyntaxTree.Ident list ->
                        typeNameResInfo:TypeNameResolutionInfo ->
                          idRange:Range.range ->
                            tcrefs:(ResolutionInfo * TypedTree.EntityRef) list ->
                              AbstractIL.Internal.Library.ResultOrException<(ResolutionInfo *
                                                                             Item *
                                                                             SyntaxTree.Ident list) list>
    val ( |AccessibleEntityRef|_| ):
      amap:Import.ImportMap ->
        m:Range.range ->
          ad:AccessibilityLogic.AccessorDomain ->
            modref:TypedTree.ModuleOrNamespaceRef ->
              mspec:TypedTree.Entity -> TypedTree.EntityRef option
    val ResolveExprLongIdentInModuleOrNamespace:
      ncenv:NameResolver ->
        nenv:NameResolutionEnv ->
          typeNameResInfo:TypeNameResolutionInfo ->
            ad:AccessibilityLogic.AccessorDomain ->
              resInfo:ResolutionInfo ->
                depth:int ->
                  m:Range.range ->
                    modref:TypedTree.EntityRef ->
                      mty:TypedTree.ModuleOrNamespaceType ->
                        id:SyntaxTree.Ident ->
                          rest:SyntaxTree.Ident list ->
                            AbstractIL.Internal.Library.ResultOrException<ResolutionInfo *
                                                                          Item *
                                                                          SyntaxTree.Ident list>
    val ChooseTyconRefInExpr:
      ncenv:NameResolver * m:Range.range * ad:AccessibilityLogic.AccessorDomain *
      nenv:NameResolutionEnv * id:SyntaxTree.Ident *
      typeNameResInfo:TypeNameResolutionInfo *
      tcrefs:(ResolutionInfo * TypedTree.TyconRef) list ->
        AbstractIL.Internal.Library.ResultOrException<(ResolutionInfo * Item) list>
    val ResolveUnqualifiedTyconRefs:
      nenv:NameResolutionEnv ->
        tcrefs:TypedTree.TyconRef list ->
          (ResolutionInfo * TypedTree.TyconRef) list
    val ResolveExprLongIdentPrim:
      sink:TcResultsSink ->
        ncenv:NameResolver ->
          first:bool ->
            fullyQualified:FullyQualifiedFlag ->
              m:Range.range ->
                ad:AccessibilityLogic.AccessorDomain ->
                  nenv:NameResolutionEnv ->
                    typeNameResInfo:TypeNameResolutionInfo ->
                      id:SyntaxTree.Ident ->
                        rest:SyntaxTree.Ident list ->
                          isOpenDecl:bool ->
                            AbstractIL.Internal.Library.ResultOrException<EnclosingTypeInst *
                                                                          Item *
                                                                          SyntaxTree.Ident list>
    val ResolveExprLongIdent:
      TcResultsSink ->
        NameResolver ->
          Range.range ->
            AccessibilityLogic.AccessorDomain ->
              NameResolutionEnv ->
                TypeNameResolutionInfo ->
                  SyntaxTree.Ident list ->
                    AbstractIL.Internal.Library.ResultOrException<EnclosingTypeInst *
                                                                  Item *
                                                                  SyntaxTree.Ident list>
    val ResolvePatternLongIdentInModuleOrNamespace:
      ncenv:NameResolver ->
        nenv:NameResolutionEnv ->
          numTyArgsOpt:TypeNameResolutionInfo ->
            ad:AccessibilityLogic.AccessorDomain ->
              resInfo:ResolutionInfo ->
                depth:int ->
                  m:Range.range ->
                    modref:TypedTree.ModuleOrNamespaceRef ->
                      mty:TypedTree.ModuleOrNamespaceType ->
                        id:SyntaxTree.Ident ->
                          rest:SyntaxTree.Ident list ->
                            AbstractIL.Internal.Library.ResultOrException<ResolutionInfo *
                                                                          Item *
                                                                          SyntaxTree.Ident list>
    exception UpperCaseIdentifierInPattern of Range.range
    type WarnOnUpperFlag =
      | WarnOnUpperCase
      | AllIdsOK
    val ResolvePatternLongIdentPrim:
      sink:TcResultsSink ->
        ncenv:NameResolver ->
          fullyQualified:FullyQualifiedFlag ->
            warnOnUpper:WarnOnUpperFlag ->
              newDef:bool ->
                m:Range.range ->
                  ad:AccessibilityLogic.AccessorDomain ->
                    nenv:NameResolutionEnv ->
                      numTyArgsOpt:TypeNameResolutionInfo ->
                        id:SyntaxTree.Ident ->
                          rest:SyntaxTree.Ident list -> Item
    val ResolvePatternLongIdent:
      TcResultsSink ->
        NameResolver ->
          WarnOnUpperFlag ->
            bool ->
              Range.range ->
                AccessibilityLogic.AccessorDomain ->
                  NameResolutionEnv ->
                    TypeNameResolutionInfo -> SyntaxTree.Ident list -> Item
    val ResolveNestedTypeThroughAbbreviation:
      ncenv:NameResolver ->
        tcref:TypedTree.TyconRef -> m:Range.range -> TypedTree.TyconRef
    val ResolveTypeLongIdentInTyconRefPrim:
      ncenv:NameResolver ->
        typeNameResInfo:TypeNameResolutionInfo ->
          ad:AccessibilityLogic.AccessorDomain ->
            resInfo:ResolutionInfo ->
              genOk:PermitDirectReferenceToGeneratedType ->
                depth:int ->
                  m:Range.range ->
                    tcref:TypedTree.TyconRef ->
                      id:SyntaxTree.Ident ->
                        rest:SyntaxTree.Ident list ->
                          AbstractIL.Internal.Library.ResultOrException<ResolutionInfo *
                                                                        TypedTree.TyconRef>
    val ResolveTypeLongIdentInTyconRef:
      TcResultsSink ->
        NameResolver ->
          NameResolutionEnv ->
            TypeNameResolutionInfo ->
              AccessibilityLogic.AccessorDomain ->
                Range.range ->
                  TypedTree.TyconRef ->
                    SyntaxTree.Ident list -> TypedTree.TyconRef
    val SuggestTypeLongIdentInModuleOrNamespace:
      depth:int ->
        modref:TypedTree.ModuleOrNamespaceRef ->
          amap:Import.ImportMap ->
            ad:AccessibilityLogic.AccessorDomain ->
              m:Range.range -> id:SyntaxTree.Ident -> exn
    val private ResolveTypeLongIdentInModuleOrNamespace:
      sink:TcResultsSink ->
        nenv:NameResolutionEnv ->
          ncenv:NameResolver ->
            typeNameResInfo:TypeNameResolutionInfo ->
              ad:AccessibilityLogic.AccessorDomain ->
                genOk:PermitDirectReferenceToGeneratedType ->
                  resInfo:ResolutionInfo ->
                    depth:int ->
                      m:Range.range ->
                        modref:TypedTree.ModuleOrNamespaceRef ->
                          _mty:TypedTree.ModuleOrNamespaceType ->
                            id:SyntaxTree.Ident ->
                              rest:SyntaxTree.Ident list ->
                                AbstractIL.Internal.Library.ResultOrException<(ResolutionInfo *
                                                                               TypedTree.EntityRef) list>
    val ResolveTypeLongIdentPrim:
      sink:TcResultsSink ->
        ncenv:NameResolver ->
          occurence:ItemOccurence ->
            first:bool ->
              fullyQualified:FullyQualifiedFlag ->
                m:Range.range ->
                  nenv:NameResolutionEnv ->
                    ad:AccessibilityLogic.AccessorDomain ->
                      id:SyntaxTree.Ident ->
                        rest:SyntaxTree.Ident list ->
                          staticResInfo:TypeNameResolutionStaticArgsInfo ->
                            genOk:PermitDirectReferenceToGeneratedType ->
                              AbstractIL.Internal.Library.ResultOrException<ResolutionInfo *
                                                                            TypedTree.TyconRef>
    val ResolveTypeLongIdentAux:
      sink:TcResultsSink ->
        ncenv:NameResolver ->
          occurence:ItemOccurence ->
            fullyQualified:FullyQualifiedFlag ->
              nenv:NameResolutionEnv ->
                ad:AccessibilityLogic.AccessorDomain ->
                  lid:SyntaxTree.Ident list ->
                    staticResInfo:TypeNameResolutionStaticArgsInfo ->
                      genOk:PermitDirectReferenceToGeneratedType ->
                        AbstractIL.Internal.Library.ResultOrException<ResolutionInfo *
                                                                      TypedTree.TyconRef>
    val ResolveTypeLongIdent:
      TcResultsSink ->
        NameResolver ->
          ItemOccurence ->
            FullyQualifiedFlag ->
              NameResolutionEnv ->
                AccessibilityLogic.AccessorDomain ->
                  SyntaxTree.Ident list ->
                    TypeNameResolutionStaticArgsInfo ->
                      PermitDirectReferenceToGeneratedType ->
                        AbstractIL.Internal.Library.ResultOrException<EnclosingTypeInst *
                                                                      TypedTree.TyconRef>
    val ResolveFieldInModuleOrNamespace:
      ncenv:NameResolver ->
        nenv:NameResolutionEnv ->
          ad:AccessibilityLogic.AccessorDomain ->
            resInfo:ResolutionInfo ->
              depth:int ->
                m:Range.range ->
                  modref:TypedTree.ModuleOrNamespaceRef ->
                    _mty:TypedTree.ModuleOrNamespaceType ->
                      id:SyntaxTree.Ident ->
                        rest:SyntaxTree.Ident list ->
                          AbstractIL.Internal.Library.ResultOrException<ResolutionInfo *
                                                                        FieldResolution *
                                                                        SyntaxTree.Ident list>
    val SuggestOtherLabelsOfSameRecordType:
      g:TcGlobals.TcGlobals ->
        nenv:NameResolutionEnv ->
          ty:TypedTree.TType ->
            id:SyntaxTree.Ident ->
              allFields:SyntaxTree.Ident list ->
                System.Collections.Generic.HashSet<string>
    val SuggestLabelsOfRelatedRecords:
      g:TcGlobals.TcGlobals ->
        nenv:NameResolutionEnv ->
          id:SyntaxTree.Ident -> allFields:SyntaxTree.Ident list -> exn
    val ResolveFieldPrim:
      sink:TcResultsSink ->
        ncenv:NameResolver ->
          nenv:NameResolutionEnv ->
            ad:AccessibilityLogic.AccessorDomain ->
              ty:TypedTree.TType ->
                mp:SyntaxTree.Ident list * id:SyntaxTree.Ident ->
                  allFields:SyntaxTree.Ident list ->
                    (ResolutionInfo * FieldResolution) list
    val ResolveField:
      TcResultsSink ->
        NameResolver ->
          NameResolutionEnv ->
            AccessibilityLogic.AccessorDomain ->
              TypedTree.TType ->
                SyntaxTree.Ident list * SyntaxTree.Ident ->
                  SyntaxTree.Ident list -> FieldResolution list
    val private ResolveExprDotLongIdent:
      ncenv:NameResolver ->
        m:Range.range ->
          ad:AccessibilityLogic.AccessorDomain ->
            nenv:NameResolutionEnv ->
              ty:TypedTree.TType ->
                id:SyntaxTree.Ident ->
                  rest:SyntaxTree.Ident list ->
                    typeNameResInfo:TypeNameResolutionInfo ->
                      findFlag:InfoReader.FindMemberFlag ->
                        ResolutionInfo * Item * SyntaxTree.Ident list
    val ComputeItemRange:
      wholem:Range.range ->
        lid:SyntaxTree.Ident list -> rest:'a list -> Range.range
    val FilterMethodGroups:
      ncenv:NameResolver ->
        itemRange:Range.range -> item:Item -> staticOnly:bool -> Item
    val NeedsWorkAfterResolution: namedItem:Item -> bool
    [<RequireQualifiedAccessAttribute>]
    type AfterResolution =
      | DoNothing
      | RecordResolution of
        Item option * (TypedTreeOps.TyparInst -> unit) *
        (Infos.MethInfo * Infos.PropInfo option * TypedTreeOps.TyparInst -> unit) *
        (unit -> unit)
    val ResolveLongIdentAsExprAndComputeRange:
      TcResultsSink ->
        NameResolver ->
          Range.range ->
            AccessibilityLogic.AccessorDomain ->
              NameResolutionEnv ->
                TypeNameResolutionInfo ->
                  SyntaxTree.Ident list ->
                    AbstractIL.Internal.Library.ResultOrException<EnclosingTypeInst *
                                                                  Item *
                                                                  Range.range *
                                                                  SyntaxTree.Ident list *
                                                                  AfterResolution>
    val ( |NonOverridable|_| ): namedItem:Item -> unit option
    val ResolveExprDotLongIdentAndComputeRange:
      TcResultsSink ->
        NameResolver ->
          Range.range ->
            AccessibilityLogic.AccessorDomain ->
              NameResolutionEnv ->
                TypedTree.TType ->
                  SyntaxTree.Ident list ->
                    TypeNameResolutionInfo ->
                      InfoReader.FindMemberFlag ->
                        bool ->
                          Item * Range.range * SyntaxTree.Ident list *
                          AfterResolution
    val FakeInstantiationGenerator:
      Range.range -> TypedTree.Typar list -> TypedTree.TType list
    val ItemForModuleOrNamespaceRef: v:TypedTree.ModuleOrNamespaceRef -> Item
    val ItemForPropInfo: pinfo:Infos.PropInfo -> Item
    val IsTyconUnseenObsoleteSpec:
      ad:AccessibilityLogic.AccessorDomain ->
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap ->
            m:Range.range -> x:TypedTree.TyconRef -> allowObsolete:bool -> bool
    val IsTyconUnseen:
      ad:AccessibilityLogic.AccessorDomain ->
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap -> m:Range.range -> x:TypedTree.TyconRef -> bool
    val IsValUnseen:
      ad:AccessibilityLogic.AccessorDomain ->
        g:TcGlobals.TcGlobals -> m:'a -> v:TypedTree.ValRef -> bool
    val IsUnionCaseUnseen:
      ad:AccessibilityLogic.AccessorDomain ->
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap ->
            m:Range.range -> ucref:TypedTree.UnionCaseRef -> bool
    val ItemIsUnseen:
      ad:AccessibilityLogic.AccessorDomain ->
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap -> m:Range.range -> item:Item -> bool
    val ItemOfTyconRef:
      ncenv:NameResolver -> m:Range.range -> x:TypedTree.TyconRef -> Item
    val ItemOfTy: g:TcGlobals.TcGlobals -> x:TypedTree.TType -> Item
    val IsInterestingModuleName: nm:string -> bool
    val PartialResolveLookupInModuleOrNamespaceAsModuleOrNamespaceThen:
      f:(TypedTree.ModuleOrNamespaceRef -> 'a list) ->
        plid:string list -> modref:TypedTree.ModuleOrNamespaceRef -> 'a list
    val PartialResolveLongIdentAsModuleOrNamespaceThen:
      nenv:NameResolutionEnv ->
        plid:string list ->
          f:(TypedTree.ModuleOrNamespaceRef -> 'a list) -> 'a list
    val ResolveRecordOrClassFieldsOfType:
      NameResolver ->
        Range.range ->
          AccessibilityLogic.AccessorDomain ->
            TypedTree.TType -> bool -> Item list
    [<RequireQualifiedAccessAttribute>]
    type ResolveCompletionTargets =
      | All of (Infos.MethInfo -> TypedTree.TType -> bool)
      | SettablePropertiesAndFields
      with
        member ResolveAll: bool
    
    val ResolveCompletionsInType:
      NameResolver ->
        NameResolutionEnv ->
          ResolveCompletionTargets ->
            Range.range ->
              AccessibilityLogic.AccessorDomain ->
                bool -> TypedTree.TType -> Item list
    val ResolvePartialLongIdentInType:
      ncenv:NameResolver ->
        nenv:NameResolutionEnv ->
          isApplicableMeth:ResolveCompletionTargets ->
            m:Range.range ->
              ad:AccessibilityLogic.AccessorDomain ->
                statics:bool ->
                  plid:string list -> ty:TypedTree.TType -> Item list
    val InfosForTyconConstructors:
      ncenv:NameResolver ->
        m:Range.range ->
          ad:AccessibilityLogic.AccessorDomain ->
            tcref:TypedTree.TyconRef -> Item list
    val inline notFakeContainerModule:
      tyconNames:System.Collections.Generic.HashSet<'a> -> nm:'a -> bool
    val getFakeContainerModulesFromTycons:
      tycons:#seq<TypedTree.Tycon> -> System.Collections.Generic.HashSet<string>
    val getFakeContainerModulesFromTyconRefs:
      tyconRefs:#seq<TypedTree.TyconRef> ->
        System.Collections.Generic.HashSet<string>
    val private EntityRefContainsSomethingAccessible:
      ncenv:NameResolver ->
        m:Range.range ->
          ad:AccessibilityLogic.AccessorDomain ->
            modref:TypedTree.ModuleOrNamespaceRef -> bool
    val ResolvePartialLongIdentInModuleOrNamespace:
      ncenv:NameResolver ->
        nenv:NameResolutionEnv ->
          isApplicableMeth:ResolveCompletionTargets ->
            m:Range.range ->
              ad:AccessibilityLogic.AccessorDomain ->
                modref:TypedTree.ModuleOrNamespaceRef ->
                  plid:string list -> allowObsolete:bool -> Item list
    val TryToResolveLongIdentAsType:
      NameResolver ->
        NameResolutionEnv ->
          Range.range -> string list -> TypedTree.TType option
    val ResolvePartialLongIdentPrim:
      ncenv:NameResolver ->
        nenv:NameResolutionEnv ->
          isApplicableMeth:ResolveCompletionTargets ->
            fullyQualified:FullyQualifiedFlag ->
              m:Range.range ->
                ad:AccessibilityLogic.AccessorDomain ->
                  plid:string list -> allowObsolete:bool -> Item list
    val ResolvePartialLongIdent:
      NameResolver ->
        NameResolutionEnv ->
          (Infos.MethInfo -> TypedTree.TType -> bool) ->
            Range.range ->
              AccessibilityLogic.AccessorDomain ->
                string list -> bool -> Item list
    val ResolvePartialLongIdentInModuleOrNamespaceForRecordFields:
      ncenv:NameResolver ->
        nenv:'a ->
          m:Range.range ->
            ad:AccessibilityLogic.AccessorDomain ->
              modref:TypedTree.ModuleOrNamespaceRef ->
                plid:string list -> allowObsolete:bool -> Item list
    val ResolvePartialLongIdentToClassOrRecdFields:
      NameResolver ->
        NameResolutionEnv ->
          Range.range ->
            AccessibilityLogic.AccessorDomain ->
              string list -> bool -> Item list
    val ResolvePartialLongIdentToClassOrRecdFieldsImpl:
      ncenv:NameResolver ->
        nenv:NameResolutionEnv ->
          fullyQualified:FullyQualifiedFlag ->
            m:Range.range ->
              ad:AccessibilityLogic.AccessorDomain ->
                plid:string list -> allowObsolete:bool -> Item list
    val ResolveCompletionsInTypeForItem:
      ncenv:NameResolver ->
        nenv:NameResolutionEnv ->
          m:Range.range ->
            ad:AccessibilityLogic.AccessorDomain ->
              statics:bool -> ty:TypedTree.TType -> item:Item -> seq<Item>
    val ResolvePartialLongIdentInTypeForItem:
      ncenv:NameResolver ->
        nenv:NameResolutionEnv ->
          m:Range.range ->
            ad:AccessibilityLogic.AccessorDomain ->
              statics:bool ->
                plid:string list -> item:Item -> ty:TypedTree.TType -> seq<Item>
    val ResolvePartialLongIdentInModuleOrNamespaceForItem:
      ncenv:NameResolver ->
        nenv:NameResolutionEnv ->
          m:Range.range ->
            ad:AccessibilityLogic.AccessorDomain ->
              modref:TypedTree.ModuleOrNamespaceRef ->
                plid:string list -> item:Item -> seq<Item>
    val PartialResolveLookupInModuleOrNamespaceAsModuleOrNamespaceThenLazy:
      f:(TypedTree.ModuleOrNamespaceRef -> seq<'a>) ->
        plid:string list -> modref:TypedTree.ModuleOrNamespaceRef -> seq<'a>
    val PartialResolveLongIdentAsModuleOrNamespaceThenLazy:
      nenv:NameResolutionEnv ->
        plid:string list ->
          f:(TypedTree.ModuleOrNamespaceRef -> seq<'a>) -> seq<'a>
    val GetCompletionForItem:
      ncenv:NameResolver ->
        nenv:NameResolutionEnv ->
          m:Range.range ->
            ad:AccessibilityLogic.AccessorDomain ->
              plid:string list -> item:Item -> seq<Item>
    val IsItemResolvable:
      NameResolver ->
        NameResolutionEnv ->
          Range.range ->
            AccessibilityLogic.AccessorDomain -> string list -> Item -> bool
    val GetVisibleNamespacesAndModulesAtPoint:
      NameResolver ->
        NameResolutionEnv ->
          Range.range ->
            AccessibilityLogic.AccessorDomain ->
              TypedTree.ModuleOrNamespaceRef list


namespace FSharp.Compiler
  module internal SignatureConformance =
    exception RequiredButNotSpecified of
                                        TypedTreeOps.DisplayEnv *
                                        TypedTree.ModuleOrNamespaceRef * string *
                                        (System.Text.StringBuilder -> unit) *
                                        Range.range
    exception ValueNotContained of
                                  TypedTreeOps.DisplayEnv *
                                  TypedTree.ModuleOrNamespaceRef * TypedTree.Val *
                                  TypedTree.Val *
                                  (string * string * string -> string)
    exception ConstrNotContained of
                                   TypedTreeOps.DisplayEnv * TypedTree.UnionCase *
                                   TypedTree.UnionCase *
                                   (string * string -> string)
    exception ExnconstrNotContained of
                                      TypedTreeOps.DisplayEnv * TypedTree.Tycon *
                                      TypedTree.Tycon *
                                      (string * string -> string)
    exception FieldNotContained of
                                  TypedTreeOps.DisplayEnv * TypedTree.RecdField *
                                  TypedTree.RecdField *
                                  (string * string -> string)
    exception InterfaceNotRevealed of
                                     TypedTreeOps.DisplayEnv * TypedTree.TType *
                                     Range.range
    type Checker =
  
        new: g:TcGlobals.TcGlobals * amap:Import.ImportMap *
              denv:TypedTreeOps.DisplayEnv *
              remapInfo:TypedTreeOps.SignatureRepackageInfo * checkingSig:bool ->
                Checker
        member
          CheckSignature: aenv:TypedTreeOps.TypeEquivEnv ->
                             implModRef:TypedTree.ModuleOrNamespaceRef ->
                               signModType:TypedTree.ModuleOrNamespaceType ->
                                 bool
        member
          CheckTypars: m:Range.range ->
                          aenv:TypedTreeOps.TypeEquivEnv ->
                            implTypars:TypedTree.Typars ->
                              signTypars:TypedTree.Typars -> bool
    
    val CheckNamesOfModuleOrNamespaceContents:
      denv:TypedTreeOps.DisplayEnv ->
        implModRef:TypedTree.ModuleOrNamespaceRef ->
          signModType:TypedTree.ModuleOrNamespaceType -> bool
    val CheckNamesOfModuleOrNamespace:
      denv:TypedTreeOps.DisplayEnv ->
        implModRef:TypedTree.ModuleOrNamespaceRef ->
          signModType:TypedTree.ModuleOrNamespaceType -> bool


namespace FSharp.Compiler
  module internal MethodOverrides =
    type OverrideCanImplement =
      | CanImplementAnyInterfaceSlot
      | CanImplementAnyClassHierarchySlot
      | CanImplementAnySlot
      | CanImplementNoSlots
    type OverrideInfo =
      | Override of
        OverrideCanImplement * TypedTree.TyconRef * SyntaxTree.Ident *
        (TypedTree.Typars * TypedTreeOps.TyparInst) * TypedTree.TType list list *
        TypedTree.TType option * bool * bool
      with
        member ArgTypes: TypedTree.TType list list
        member BoundingTyconRef: TypedTree.TyconRef
        member CanImplement: OverrideCanImplement
        member IsCompilerGenerated: bool
        member IsFakeEventProperty: bool
        member LogicalName: string
        member Range: Range.range
        member ReturnType: TypedTree.TType option
    
    type RequiredSlot =
      | RequiredSlot of Infos.MethInfo * isOptional: bool
      | DefaultInterfaceImplementationSlot of
        Infos.MethInfo * isOptional: bool * possiblyNoMostSpecific: bool
      with
        member HasDefaultInterfaceImplementation: bool
        member IsOptional: bool
        member MethodInfo: Infos.MethInfo
        member PossiblyNoMostSpecificImplementation: bool
    
    type SlotImplSet =
      | SlotImplSet of
        RequiredSlot list *
        AbstractIL.Internal.Library.NameMultiMap<RequiredSlot> *
        OverrideInfo list * Infos.PropInfo list
    exception TypeIsImplicitlyAbstract of Range.range
    exception OverrideDoesntOverride of
                                       TypedTreeOps.DisplayEnv * OverrideInfo *
                                       Infos.MethInfo option *
                                       TcGlobals.TcGlobals * Import.ImportMap *
                                       Range.range
    module DispatchSlotChecking =
      val PrintOverrideToBuffer:
        denv:TypedTreeOps.DisplayEnv ->
          os:System.Text.StringBuilder -> OverrideInfo -> unit
      val PrintMethInfoSigToBuffer:
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap ->
            m:Range.range ->
              denv:TypedTreeOps.DisplayEnv ->
                os:System.Text.StringBuilder -> minfo:Infos.MethInfo -> unit
      val FormatOverride:
        denv:TypedTreeOps.DisplayEnv -> d:OverrideInfo -> string
      val FormatMethInfoSig:
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap ->
            m:Range.range ->
              denv:TypedTreeOps.DisplayEnv -> d:Infos.MethInfo -> string
      val GetInheritedMemberOverrideInfo:
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap ->
            m:Range.range ->
              parentType:OverrideCanImplement ->
                minfo:Infos.MethInfo -> OverrideInfo
      val GetTypeMemberOverrideInfo:
        g:TcGlobals.TcGlobals ->
          reqdTy:TypedTree.TType -> overrideBy:TypedTree.ValRef -> OverrideInfo
      val GetObjectExprOverrideInfo:
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap ->
            implty:TypedTree.TType * id:SyntaxTree.Ident *
            memberFlags:SyntaxTree.MemberFlags * ty:TypedTree.TType *
            arityInfo:TypedTree.ValReprInfo * bindingAttribs:TypedTree.Attribs *
            rhsExpr:TypedTree.Expr ->
              OverrideInfo *
              (TypedTree.Val option * TypedTree.Val * TypedTree.Val list list *
               TypedTree.Attribs * TypedTree.Expr)
      val IsNameMatch:
        dispatchSlot:Infos.MethInfo -> overrideBy:OverrideInfo -> bool
      val IsImplMatch:
        g:TcGlobals.TcGlobals ->
          dispatchSlot:Infos.MethInfo -> overrideBy:OverrideInfo -> bool
      val IsTyparKindMatch: Infos.CompiledSig -> OverrideInfo -> bool
      val IsSigPartialMatch:
        g:TcGlobals.TcGlobals ->
          dispatchSlot:Infos.MethInfo ->
            compiledSig:Infos.CompiledSig -> OverrideInfo -> bool
      val IsPartialMatch:
        g:TcGlobals.TcGlobals ->
          dispatchSlot:Infos.MethInfo ->
            compiledSig:Infos.CompiledSig -> overrideBy:OverrideInfo -> bool
      val ReverseTyparRenaming:
        g:TcGlobals.TcGlobals ->
          tinst:(TypedTree.Typar * TypedTree.TType) list ->
            (TypedTree.Typar * TypedTree.TType) list
      val ComposeTyparInsts:
        inst1:('a * TypedTree.TType) list ->
          inst2:TypedTreeOps.TyparInst -> ('a * TypedTree.TType) list
      val IsSigExactMatch:
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap ->
            m:Range.range -> dispatchSlot:Infos.MethInfo -> OverrideInfo -> bool
      val IsExactMatch:
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap ->
            m:Range.range ->
              dispatchSlot:Infos.MethInfo -> overrideBy:OverrideInfo -> bool
      val OverrideImplementsDispatchSlot:
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap ->
            m:Range.range ->
              dispatchSlot:Infos.MethInfo ->
                availPriorOverride:OverrideInfo -> bool
      val DispatchSlotIsAlreadyImplemented:
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap ->
            m:Range.range ->
              availPriorOverridesKeyed:AbstractIL.Internal.Library.NameMultiMap<OverrideInfo> ->
                dispatchSlot:Infos.MethInfo -> bool
      val CheckDispatchSlotsAreImplemented:
        denv:TypedTreeOps.DisplayEnv * infoReader:InfoReader.InfoReader *
        m:Range.range * nenv:NameResolution.NameResolutionEnv *
        sink:NameResolution.TcResultsSink * isOverallTyAbstract:bool *
        reqdTy:TypedTree.TType * dispatchSlots:RequiredSlot list *
        availPriorOverrides:OverrideInfo list * overrides:OverrideInfo list ->
          bool
      val GetMostSpecificOverrideInterfaceMethodSets:
        infoReader:InfoReader.InfoReader ->
          allReqdTys:(TypedTree.TType * Range.range) list ->
            Map<string,(TypedTree.TType * Infos.MethInfo) list>
      val GetMostSpecificOverrideInterfaceMethodsByMethod:
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap ->
            m:Range.range ->
              mostSpecificOverrides:AbstractIL.Internal.Library.NameMultiMap<TypedTree.TType *
                                                                             Infos.MethInfo> ->
                minfo:Infos.MethInfo -> (TypedTree.TType * Infos.MethInfo) list
      val GetInterfaceDispatchSlots:
        infoReader:InfoReader.InfoReader ->
          ad:AccessibilityLogic.AccessorDomain ->
            m:Range.range ->
              availImpliedInterfaces:TypedTree.TType list ->
                mostSpecificOverrides:AbstractIL.Internal.Library.NameMultiMap<TypedTree.TType *
                                                                               Infos.MethInfo> ->
                  interfaceTy:TypedTree.TType -> RequiredSlot list
      val GetClassDispatchSlots:
        infoReader:InfoReader.InfoReader ->
          ad:AccessibilityLogic.AccessorDomain ->
            m:Range.range -> reqdTy:TypedTree.TType -> RequiredSlot list
      val GetDispatchSlotSet:
        infoReader:InfoReader.InfoReader ->
          ad:AccessibilityLogic.AccessorDomain ->
            m:Range.range ->
              availImpliedInterfaces:TypedTree.TType list ->
                mostSpecificOverrides:AbstractIL.Internal.Library.NameMultiMap<TypedTree.TType *
                                                                               Infos.MethInfo> ->
                  reqdTy:TypedTree.TType ->
                    impliedTys:seq<TypedTree.TType> ->
                      (TypedTree.TType * RequiredSlot list) list
      val CheckOverridesAreAllUsedOnce:
        denv:TypedTreeOps.DisplayEnv * g:TcGlobals.TcGlobals *
        amap:Import.ImportMap * isObjExpr:bool * reqdTy:TypedTree.TType *
        dispatchSlotsKeyed:AbstractIL.Internal.Library.NameMultiMap<RequiredSlot> *
        availPriorOverrides:OverrideInfo list * overrides:OverrideInfo list ->
          unit
      val GetSlotImplSets:
        infoReader:InfoReader.InfoReader ->
          denv:TypedTreeOps.DisplayEnv ->
            ad:AccessibilityLogic.AccessorDomain ->
              isObjExpr:bool ->
                allReqdTys:(TypedTree.TType * Range.range) list ->
                  SlotImplSet list
      val CheckImplementationRelationAtEndOfInferenceScope:
        infoReader:InfoReader.InfoReader * denv:TypedTreeOps.DisplayEnv *
        nenv:NameResolution.NameResolutionEnv *
        sink:NameResolution.TcResultsSink * tycon:TypedTree.Tycon *
        isImplementation:bool -> unit
  
    val FinalTypeDefinitionChecksAtEndOfInferenceScope:
      infoReader:InfoReader.InfoReader * nenv:NameResolution.NameResolutionEnv *
      sink:NameResolution.TcResultsSink * isImplementation:bool *
      denv:TypedTreeOps.DisplayEnv -> tycon:TypedTree.Tycon -> unit
    val GetAbstractMethInfosForSynMethodDecl:
      infoReader:InfoReader.InfoReader * ad:AccessibilityLogic.AccessorDomain *
      memberName:SyntaxTree.Ident * bindm:Range.range *
      typToSearchForAbstractMembers:(TypedTree.TType * SlotImplSet option) *
      valSynData:SyntaxTree.SynValInfo ->
        Infos.MethInfo list * Infos.MethInfo list
    val GetAbstractPropInfosForSynPropertyDecl:
      infoReader:InfoReader.InfoReader * ad:AccessibilityLogic.AccessorDomain *
      memberName:SyntaxTree.Ident * bindm:Range.range *
      typToSearchForAbstractMembers:(TypedTree.TType * SlotImplSet option) *
      _k:'a * _valSynData:'b -> Infos.PropInfo list


namespace FSharp.Compiler
  module internal MethodCalls =
    type CallerArg<'T> =
      | CallerArg of
        ty: TypedTree.TType * range: Range.range * isOpt: bool * exprInfo: 'T
      with
        member CallerArgumentType: TypedTree.TType
        member Expr: 'T
        member IsExplicitOptional: bool
        member Range: Range.range
    
    type CalledArg =
      { Position: struct (int * int)
        IsParamArray: bool
        OptArgInfo: Infos.OptionalArgInfo
        CallerInfo: Infos.CallerInfo
        IsInArg: bool
        IsOutArg: bool
        ReflArgInfo: Infos.ReflectedArgInfo
        NameOpt: SyntaxTree.Ident option
        CalledArgumentType: TypedTree.TType }
    val CalledArg:
      pos:struct (int * int) * isParamArray:bool *
      optArgInfo:Infos.OptionalArgInfo * callerInfo:Infos.CallerInfo *
      isInArg:bool * isOutArg:bool * nameOpt:SyntaxTree.Ident option *
      reflArgInfo:Infos.ReflectedArgInfo * calledArgTy:TypedTree.TType ->
        CalledArg
    type AssignedCalledArg<'T> =
      { NamedArgIdOpt: SyntaxTree.Ident option
        CalledArg: CalledArg
        CallerArg: CallerArg<'T> }
      with
        member Position: struct (int * int)
    
    type AssignedItemSetterTarget =
      | AssignedPropSetter of
        Infos.PropInfo * Infos.MethInfo * TypedTree.TypeInst
      | AssignedILFieldSetter of Infos.ILFieldInfo
      | AssignedRecdFieldSetter of Infos.RecdFieldInfo
    type AssignedItemSetter<'T> =
      | AssignedItemSetter of
        SyntaxTree.Ident * AssignedItemSetterTarget * CallerArg<'T>
    type CallerNamedArg<'T> =
      | CallerNamedArg of SyntaxTree.Ident * CallerArg<'T>
      with
        member CallerArg: CallerArg<'T>
        member Ident: SyntaxTree.Ident
        member Name: string
    
    [<StructAttribute>]
    type CallerArgs<'T> =
      { Unnamed: CallerArg<'T> list list
        Named: CallerNamedArg<'T> list list }
      with
        member ArgumentNamesAndTypes: (string option * TypedTree.TType) list
        member CallerArgCounts: int * int
        member
          CurriedCallerArgs: (CallerArg<'T> list * CallerNamedArg<'T> list) list
        static member Empty: CallerArgs<'T>
    
    val AdjustCalledArgTypeForLinqExpressionsAndAutoQuote:
      infoReader:InfoReader.InfoReader ->
        callerArgTy:TypedTree.TType ->
          calledArg:CalledArg -> m:Range.range -> TypedTree.TType
    val AdjustCalledArgTypeForOptionals:
      g:TcGlobals.TcGlobals ->
        enforceNullableOptionalsKnownTypes:bool ->
          calledArg:CalledArg ->
            calledArgTy:TypedTree.TType ->
              callerArg:CallerArg<'a> -> TypedTree.TType
    val AdjustCalledArgType:
      infoReader:InfoReader.InfoReader ->
        isConstraint:bool ->
          enforceNullableOptionalsKnownTypes:bool ->
            calledArg:CalledArg -> callerArg:CallerArg<'a> -> TypedTree.TType
    type CalledMethArgSet<'T> =
      { UnnamedCalledArgs: CalledArg list
        UnnamedCallerArgs: CallerArg<'T> list
        ParamArrayCalledArgOpt: CalledArg option
        ParamArrayCallerArgs: CallerArg<'T> list
        AssignedNamedArgs: AssignedCalledArg<'T> list }
      with
        member NumAssignedNamedArgs: int
        member NumUnnamedCalledArgs: int
        member NumUnnamedCallerArgs: int
    
    val MakeCalledArgs:
      amap:Import.ImportMap ->
        m:Range.range ->
          minfo:Infos.MethInfo ->
            minst:TypedTree.TType list -> CalledArg list list
    type CalledMeth<'T> =
  
        new: infoReader:InfoReader.InfoReader *
              nameEnv:NameResolution.NameResolutionEnv option *
              isCheckingAttributeCall:bool *
              freshenMethInfo:(Range.range -> Infos.MethInfo ->
                                 TypedTree.TypeInst) * m:Range.range *
              ad:AccessibilityLogic.AccessorDomain * minfo:Infos.MethInfo *
              calledTyArgs:TypedTree.TType list *
              callerTyArgs:TypedTree.TType list * pinfoOpt:Infos.PropInfo option *
              callerObjArgTys:TypedTree.TType list * callerArgs:CallerArgs<'T> *
              allowParamArgs:bool * allowOutAndOptArgs:bool *
              tyargsOpt:TypedTree.TType option -> CalledMeth<'T>
        static member GetMethod: x:CalledMeth<'T> -> Infos.MethInfo
        member CalledObjArgTys: m:Range.range -> TypedTree.TType list
        member GetParamArrayElementType: unit -> TypedTree.TType
        member HasCorrectObjArgs: m:Range.range -> bool
        member
          IsAccessible: m:Range.range * ad:AccessibilityLogic.AccessorDomain ->
                           bool
        member
          IsCandidate: m:Range.range * ad:AccessibilityLogic.AccessorDomain ->
                          bool
        override ToString: unit -> string
        member AllCalledArgs: CalledArg list list
        member AllUnnamedCalledArgs: CalledArg list
        member ArgSets: CalledMethArgSet<'T> list
        member AssignedItemSetters: AssignedItemSetter<'T> list
        member AssignedNamedArgs: AssignedCalledArg<'T> list list
        member AssignedUnnamedArgs: AssignedCalledArg<'T> list list
        member AssignsAllNamedArgs: bool
        member AssociatedPropertyInfo: Infos.PropInfo option
        member AttributeAssignedNamedArgs: CallerNamedArg<'T> list
        member CalledReturnTypeAfterByrefDeref: TypedTree.TType
        member CalledReturnTypeAfterOutArgTupling: TypedTree.TType
        member CalledTyArgs: TypedTree.TType list
        member CalledTyparInst: TypedTreeOps.TyparInst
        member CallerObjArgTys: TypedTree.TType list
        member CallerTyArgs: TypedTree.TType list
        member HasCorrectArity: bool
        member HasCorrectGenericArity: bool
        member HasOptArgs: bool
        member HasOutArgs: bool
        member Method: Infos.MethInfo
        member NumArgSets: int
        member NumAssignedProps: int
        member NumCalledTyArgs: int
        member NumCallerTyArgs: int
        member ParamArrayCalledArgOpt: CalledArg option
        member ParamArrayCallerArgs: CallerArg<'T> list option
        member TotalNumAssignedNamedArgs: int
        member TotalNumUnnamedCalledArgs: int
        member TotalNumUnnamedCallerArgs: int
        member UnassignedNamedArgs: CallerNamedArg<'T> list
        member UnnamedCalledOptArgs: CalledArg list
        member UnnamedCalledOutArgs: CalledArg list
        member UsesParamArrayConversion: bool
        member amap: Import.ImportMap
        member infoReader: InfoReader.InfoReader
    
    val NamesOfCalledArgs: calledArgs:CalledArg list -> SyntaxTree.Ident list
    type ArgumentAnalysis =
      | NoInfo
      | ArgDoesNotMatch
      | CallerLambdaHasArgTypes of TypedTree.TType list
      | CalledArgMatchesType of TypedTree.TType
    val InferLambdaArgsForLambdaPropagation:
      origRhsExpr:SyntaxTree.SynExpr -> int
    val ExamineArgumentForLambdaPropagation:
      infoReader:InfoReader.InfoReader ->
        arg:AssignedCalledArg<SyntaxTree.SynExpr> -> ArgumentAnalysis
    val ExamineMethodForLambdaPropagation:
      x:CalledMeth<SyntaxTree.SynExpr> ->
        (ArgumentAnalysis list list *
         (SyntaxTree.Ident * ArgumentAnalysis) list list) option
    val IsBaseCall: objArgs:TypedTree.Expr list -> bool
    val ComputeConstrainedCallInfo:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          m:Range.range ->
            objArgs:TypedTree.Expr list * minfo:Infos.MethInfo ->
              TypedTree.TType option
    val TakeObjAddrForMethodCall:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          minfo:Infos.MethInfo ->
            isMutable:TypedTreeOps.Mutates ->
              m:Range.range ->
                objArgs:TypedTree.Expr list ->
                  f:(TypedTree.TType option -> TypedTree.Expr list ->
                       TypedTree.Expr * 'a) -> TypedTree.Expr * 'a
    val BuildILMethInfoCall:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          m:Range.range ->
            isProp:bool ->
              minfo:Infos.ILMethInfo ->
                valUseFlags:TypedTree.ValUseFlag ->
                  minst:TypedTree.TType list ->
                    direct:bool ->
                      args:TypedTree.Exprs -> TypedTree.Expr * TypedTree.TType
    val BuildFSharpMethodApp:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          vref:TypedTree.ValRef ->
            vexp:TypedTree.Expr ->
              vexprty:TypedTree.TType ->
                args:TypedTree.Exprs -> TypedTree.Expr * TypedTree.TType
    val BuildFSharpMethodCall:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          ty:TypedTree.TType * vref:TypedTree.ValRef ->
            valUseFlags:TypedTree.ValUseFlag ->
              minst:TypedTree.TType list ->
                args:TypedTree.Exprs -> TypedTree.Expr * TypedTree.TType
    val MakeMethInfoCall:
      amap:Import.ImportMap ->
        m:Range.range ->
          minfo:Infos.MethInfo ->
            minst:TypedTree.TType list -> args:TypedTree.Exprs -> TypedTree.Expr
    val TryImportProvidedMethodBaseAsLibraryIntrinsic:
      amap:Import.ImportMap * m:Range.range *
      mbase:Tainted<ExtensionTyping.ProvidedMethodBase> ->
        TypedTree.ValRef option
    val BuildMethodCall:
      tcVal:(TypedTree.ValRef -> TypedTree.ValUseFlag -> TypedTree.TType list ->
               Range.range -> TypedTree.Expr * TypedTree.TType) ->
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap ->
            isMutable:TypedTreeOps.Mutates ->
              m:Range.range ->
                isProp:bool ->
                  minfo:Infos.MethInfo ->
                    valUseFlags:TypedTree.ValUseFlag ->
                      minst:TypedTree.TType list ->
                        objArgs:TypedTree.Expr list ->
                          args:TypedTree.Expr list ->
                            TypedTree.Expr * TypedTree.TType
    val BuildObjCtorCall:
      g:TcGlobals.TcGlobals -> m:Range.range -> TypedTree.Expr
    val BuildNewDelegateExpr:
      eventInfoOpt:Infos.EventInfo option * g:TcGlobals.TcGlobals *
      amap:Import.ImportMap * delegateTy:TypedTree.TType *
      invokeMethInfo:Infos.MethInfo * delArgTys:TypedTree.TType list *
      f:TypedTree.Expr * fty:TypedTree.TType * m:Range.range -> TypedTree.Expr
    val CoerceFromFSharpFuncToDelegate:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          infoReader:InfoReader.InfoReader ->
            ad:AccessibilityLogic.AccessorDomain ->
              callerArgTy:TypedTree.TType ->
                m:Range.range ->
                  callerArgExpr:TypedTree.Expr ->
                    delegateTy:TypedTree.TType -> TypedTree.Expr
    val AdjustCallerArgExprForCoercions:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          infoReader:InfoReader.InfoReader ->
            ad:AccessibilityLogic.AccessorDomain ->
              isOutArg:bool ->
                calledArgTy:TypedTree.TType ->
                  reflArgInfo:Infos.ReflectedArgInfo ->
                    callerArgTy:TypedTree.TType ->
                      m:Range.range ->
                        callerArgExpr:TypedTree.Expr ->
                          'a option * TypedTree.Expr
    val emptyPreBinder: e:TypedTree.Expr -> TypedTree.Expr
    val GetDefaultExpressionForCallerSideOptionalArg:
      tcFieldInit:(Range.range -> AbstractIL.IL.ILFieldInit -> TypedTree.Const) ->
        g:TcGlobals.TcGlobals ->
          calledArg:CalledArg ->
            currCalledArgTy:TypedTree.TType ->
              currDfltVal:Infos.OptionalArgCallerSideValue ->
                eCallerMemberName:string option ->
                  mMethExpr:Range.range ->
                    (TypedTree.Expr -> TypedTree.Expr) * TypedTree.Expr
    val GetDefaultExpressionForCalleeSideOptionalArg:
      g:TcGlobals.TcGlobals ->
        calledArg:CalledArg ->
          eCallerMemberName:string option ->
            mMethExpr:Range.range -> TypedTree.Expr
    val GetDefaultExpressionForOptionalArg:
      tcFieldInit:(Range.range -> AbstractIL.IL.ILFieldInit -> TypedTree.Const) ->
        g:TcGlobals.TcGlobals ->
          calledArg:CalledArg ->
            eCallerMemberName:string option ->
              mItem:Range.range ->
                mMethExpr:Range.range ->
                  (TypedTree.Expr -> TypedTree.Expr) *
                  AssignedCalledArg<TypedTree.Expr>
    val MakeNullableExprIfNeeded:
      infoReader:InfoReader.InfoReader ->
        calledArgTy:TypedTree.TType ->
          callerArgTy:TypedTree.TType ->
            callerArgExpr:TypedTree.Expr -> m:Range.range -> TypedTree.Expr
    val AdjustCallerArgForOptional:
      tcFieldInit:(Range.range -> AbstractIL.IL.ILFieldInit -> TypedTree.Const) ->
        eCallerMemberName:string option ->
          infoReader:InfoReader.InfoReader ->
            assignedArg:AssignedCalledArg<TypedTree.Expr> ->
              AssignedCalledArg<TypedTree.Expr>
    val AdjustCallerArgsForOptionals:
      tcFieldInit:(Range.range -> AbstractIL.IL.ILFieldInit -> TypedTree.Const) ->
        eCallerMemberName:string option ->
          infoReader:InfoReader.InfoReader ->
            calledMeth:CalledMeth<TypedTree.Expr> ->
              mItem:Range.range ->
                mMethExpr:Range.range ->
                  AssignedCalledArg<TypedTree.Expr> list *
                  (TypedTree.Expr -> TypedTree.Expr) *
                  AssignedCalledArg<TypedTree.Expr> list *
                  AssignedCalledArg<TypedTree.Expr> list
    val AdjustOutCallerArgs:
      g:TcGlobals.TcGlobals ->
        calledMeth:CalledMeth<'a> ->
          mMethExpr:Range.range ->
            AssignedCalledArg<TypedTree.Expr> list * TypedTree.Expr list *
            TypedTree.Binding list
    val AdjustParamArrayCallerArgs:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          infoReader:InfoReader.InfoReader ->
            ad:AccessibilityLogic.AccessorDomain ->
              calledMeth:CalledMeth<TypedTree.Expr> ->
                mMethExpr:Range.range ->
                  'a option list * AssignedCalledArg<TypedTree.Expr> list
    val AdjustCallerArgs:
      tcFieldInit:(Range.range -> AbstractIL.IL.ILFieldInit -> TypedTree.Const) ->
        eCallerMemberName:string option ->
          infoReader:InfoReader.InfoReader ->
            ad:AccessibilityLogic.AccessorDomain ->
              calledMeth:CalledMeth<TypedTree.Expr> ->
                objArgs:TypedTree.Expr list ->
                  lambdaVars:'a option ->
                    mItem:Range.range ->
                      mMethExpr:Range.range ->
                        (TypedTree.Expr -> TypedTree.Expr) * TypedTree.Expr list *
                        'b option list * AssignedCalledArg<TypedTree.Expr> list *
                        TypedTree.Expr list * (TypedTree.Expr -> TypedTree.Expr) *
                        'c option list * TypedTree.Expr list *
                        TypedTree.Binding list
    module ProvidedMethodCalls =
      val private convertConstExpr:
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap ->
            m:Range.range ->
              constant:Tainted<obj * ExtensionTyping.ProvidedType> ->
                TypedTree.Expr
      val eraseSystemType:
        amap:Import.ImportMap * m:Range.range *
        inputType:Tainted<ExtensionTyping.ProvidedType> ->
          Tainted<ExtensionTyping.ProvidedType>
      val convertProvidedExpressionToExprAndWitness:
        tcVal:(TypedTree.ValRef -> TypedTree.ValUseFlag ->
                 TypedTree.TType list -> Range.range ->
                 TypedTree.Expr * TypedTree.TType) ->
          thisArg:TypedTree.Expr option * allArgs:TypedTree.Exprs *
          paramVars:Tainted<ExtensionTyping.ProvidedVar> [] *
          g:TcGlobals.TcGlobals * amap:Import.ImportMap *
          mut:TypedTreeOps.Mutates * isProp:bool *
          isSuperInit:TypedTree.ValUseFlag * m:Range.range *
          expr:Tainted<ExtensionTyping.ProvidedExpr> ->
            Tainted<ExtensionTyping.ProvidedMethodInfo> option *
            (TypedTree.Expr * TypedTree.TType)
      val TranslateInvokerExpressionForProvidedMethodCall:
        tcVal:(TypedTree.ValRef -> TypedTree.ValUseFlag ->
                 TypedTree.TType list -> Range.range ->
                 TypedTree.Expr * TypedTree.TType) ->
          g:TcGlobals.TcGlobals * amap:Import.ImportMap *
          mut:TypedTreeOps.Mutates * isProp:bool *
          isSuperInit:TypedTree.ValUseFlag *
          mi:Tainted<ExtensionTyping.ProvidedMethodBase> *
          objArgs:TypedTree.Expr list * allArgs:TypedTree.Exprs * m:Range.range ->
            Tainted<ExtensionTyping.ProvidedMethodInfo> option *
            (TypedTree.Expr * TypedTree.TType)
      val BuildInvokerExpressionForProvidedMethodCall:
        tcVal:(TypedTree.ValRef -> TypedTree.ValUseFlag ->
                 TypedTree.TType list -> Range.range ->
                 TypedTree.Expr * TypedTree.TType) ->
          g:TcGlobals.TcGlobals * amap:Import.ImportMap *
          mi:Tainted<ExtensionTyping.ProvidedMethodBase> *
          objArgs:TypedTree.Expr list * mut:TypedTreeOps.Mutates * isProp:bool *
          isSuperInit:TypedTree.ValUseFlag * allArgs:TypedTree.Exprs *
          m:Range.range ->
            Tainted<ExtensionTyping.ProvidedMethodInfo> option * TypedTree.Expr *
            TypedTree.TType
  
    val RecdFieldInstanceChecks:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          ad:AccessibilityLogic.AccessorDomain ->
            m:Range.range -> rfinfo:Infos.RecdFieldInfo -> unit
    val ILFieldStaticChecks:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          infoReader:InfoReader.InfoReader ->
            ad:AccessibilityLogic.AccessorDomain ->
              m:Range.range -> finfo:Infos.ILFieldInfo -> unit
    val ILFieldInstanceChecks:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          ad:AccessibilityLogic.AccessorDomain ->
            m:Range.range -> finfo:Infos.ILFieldInfo -> unit
    val MethInfoChecks:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          isInstance:bool ->
            tyargsOpt:'a option ->
              objArgs:TypedTree.Expr list ->
                ad:AccessibilityLogic.AccessorDomain ->
                  m:Range.range -> minfo:Infos.MethInfo -> unit
    exception FieldNotMutable of
                                TypedTreeOps.DisplayEnv * TypedTree.RecdFieldRef *
                                Range.range
    val CheckRecdFieldMutation:
      m:Range.range ->
        denv:TypedTreeOps.DisplayEnv -> rfinfo:Infos.RecdFieldInfo -> unit
    val GenWitnessExpr:
      amap:Import.ImportMap ->
        g:TcGlobals.TcGlobals ->
          m:Range.range ->
            traitInfo:TypedTree.TraitConstraintInfo ->
              argExprs:TypedTree.Expr list -> TypedTree.Expr option
    val GenWitnessExprLambda:
      amap:Import.ImportMap ->
        g:TcGlobals.TcGlobals ->
          m:Range.range ->
            traitInfo:TypedTree.TraitConstraintInfo ->
              Choice<TypedTree.TraitConstraintInfo,TypedTree.Expr>
    val GenWitnessArgs:
      amap:Import.ImportMap ->
        g:TcGlobals.TcGlobals ->
          m:Range.range ->
            traitInfos:TypedTree.TraitConstraintInfo list ->
              Choice<TypedTree.TraitConstraintInfo,TypedTree.Expr> list


namespace FSharp.Compiler
  module internal PatternMatchCompilation =
    exception MatchIncomplete of bool * (string * bool) option * Range.range
    exception RuleNeverMatched of Range.range
    exception EnumMatchIncomplete of bool * (string * bool) option * Range.range
    type ActionOnFailure =
      | ThrowIncompleteMatchException
      | IgnoreWithWarning
      | Throw
      | Rethrow
      | FailFilter
    [<NoEquality; NoComparison>]
    type Pattern =
      | TPat_const of TypedTree.Const * Range.range
      | TPat_wild of Range.range
      | TPat_as of Pattern * PatternValBinding * Range.range
      | TPat_disjs of Pattern list * Range.range
      | TPat_conjs of Pattern list * Range.range
      | TPat_query of
        (TypedTree.Expr * TypedTree.TType list *
         (TypedTree.ValRef * TypedTree.TypeInst) option * int *
         PrettyNaming.ActivePatternInfo) * Pattern * Range.range
      | TPat_unioncase of
        TypedTree.UnionCaseRef * TypedTree.TypeInst * Pattern list * Range.range
      | TPat_exnconstr of TypedTree.TyconRef * Pattern list * Range.range
      | TPat_tuple of
        TypedTree.TupInfo * Pattern list * TypedTree.TType list * Range.range
      | TPat_array of Pattern list * TypedTree.TType * Range.range
      | TPat_recd of
        TypedTree.TyconRef * TypedTree.TypeInst * Pattern list * Range.range
      | TPat_range of char * char * Range.range
      | TPat_null of Range.range
      | TPat_isinst of
        TypedTree.TType * TypedTree.TType * PatternValBinding option *
        Range.range
      | TPat_error of Range.range
      with
        member Range: Range.range
    
    and PatternValBinding = | PBind of TypedTree.Val * TypedTreeOps.TypeScheme
    and TypedMatchClause =
      | TClause of
        Pattern * TypedTree.Expr option * TypedTree.DecisionTreeTarget *
        Range.range
      with
        member BoundVals: TypedTree.Val list
        member GuardExpr: TypedTree.Expr option
        member Pattern: Pattern
        member Range: Range.range
        member Target: TypedTree.DecisionTreeTarget
    
    val debug: bool
    type SubExprOfInput =
      | SubExpr of
        (TypedTreeOps.TyparInst -> TypedTree.Expr -> TypedTree.Expr) *
        (TypedTree.Expr * TypedTree.Val)
    val BindSubExprOfInput:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          gtps:TypedTree.Typar list ->
            PatternValBinding ->
              m:Range.range -> SubExprOfInput -> TypedTree.Val * TypedTree.Expr
    val GetSubExprOfInput:
      g:TcGlobals.TcGlobals ->
        gtps:'a list * tyargs:TypedTree.TType list *
        tinst:TypedTreeOps.TyparInst -> SubExprOfInput -> TypedTree.Expr
    type Path =
      | PathQuery of Path * CompilerGlobalState.Unique
      | PathConj of Path * int
      | PathTuple of Path * TypedTree.TypeInst * int
      | PathRecd of Path * TypedTree.TyconRef * TypedTree.TypeInst * int
      | PathUnionConstr of
        Path * TypedTree.UnionCaseRef * TypedTree.TypeInst * int
      | PathArray of Path * TypedTree.TType * int * int
      | PathExnConstr of Path * TypedTree.TyconRef * int
      | PathEmpty of TypedTree.TType
    val pathEq: p1:Path -> p2:Path -> bool
    type RefutedSet =
      | RefutedInvestigation of Path * TypedTree.DecisionTreeTest list
      | RefutedWhenClause
    val notNullText: string
    val otherSubtypeText: string
    val ilFieldToTastConst: AbstractIL.IL.ILFieldInit -> TypedTree.Const
    exception CannotRefute
    val RefuteDiscrimSet:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          path:Path ->
            discrims:TypedTree.DecisionTreeTest list -> TypedTree.Expr * bool
    val CombineRefutations:
      g:TcGlobals.TcGlobals ->
        r1:TypedTree.Expr -> r2:TypedTree.Expr -> TypedTree.Expr
    val ShowCounterExample:
      g:TcGlobals.TcGlobals ->
        denv:TypedTreeOps.DisplayEnv ->
          m:Range.range ->
            refuted:RefutedSet list -> (string * bool * bool) option
    type RuleNumber = int
    type Active = | Active of Path * SubExprOfInput * Pattern
    type Actives = Active list
    type Frontier =
      | Frontier of RuleNumber * Actives * TypedTreeOps.ValMap<TypedTree.Expr>
    type InvestigationPoint =
      | Investigation of RuleNumber * TypedTree.DecisionTreeTest * Path
    val isMemOfActives: p1:Path -> actives:Active list -> bool
    val lookupActive: x:Path -> l:Active list -> SubExprOfInput * Pattern
    val removeActive: x:Path -> l:Active list -> Active list
    val getDiscrimOfPattern:
      g:TcGlobals.TcGlobals ->
        tpinst:TypedTreeOps.TyparInst ->
          t:Pattern -> TypedTree.DecisionTreeTest option
    val constOfDiscrim: discrim:TypedTree.DecisionTreeTest -> TypedTree.Const
    val constOfCase: c:TypedTree.DecisionTreeCase -> TypedTree.Const
    val discrimsEq:
      g:TcGlobals.TcGlobals ->
        d1:TypedTree.DecisionTreeTest -> d2:TypedTree.DecisionTreeTest -> bool
    val isDiscrimSubsumedBy:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          m:Range.range ->
            d1:TypedTree.DecisionTreeTest ->
              d2:TypedTree.DecisionTreeTest -> bool
    val chooseSimultaneousEdgeSet:
      prevOpt:'a option ->
        f:('a option -> 'b -> 'a option) -> l:'b list -> 'a list * 'b list
    val canCompactConstantClass: c:TypedTree.Const -> bool
    val discrimsHaveSameSimultaneousClass:
      g:TcGlobals.TcGlobals ->
        d1:TypedTree.DecisionTreeTest -> d2:TypedTree.DecisionTreeTest -> bool
    val canInvestigate: pat:Pattern -> bool
    val ChooseInvestigationPointLeftToRight: frontiers:Frontier list -> Active
    val ( |ConstNeedsDefaultCase|_| ): c:TypedTree.Const -> unit option
    val BuildSwitch:
      inpExprOpt:'a option ->
        g:TcGlobals.TcGlobals ->
          expr:TypedTree.Expr ->
            edges:TypedTree.DecisionTreeCase list ->
              dflt:TypedTree.DecisionTree option ->
                m:Range.range -> TypedTree.DecisionTree
    val layoutPat: pat:Pattern -> Internal.Utilities.StructuredFormat.Layout
    val layoutPath: _p:'a -> Internal.Utilities.StructuredFormat.Layout
    val layoutActive: Active -> Internal.Utilities.StructuredFormat.Layout
    val layoutFrontier: Frontier -> Internal.Utilities.StructuredFormat.Layout
    val mkFrontiers:
      investigations:(Actives * TypedTreeOps.ValMap<TypedTree.Expr>) list ->
        i:RuleNumber -> Frontier list
    val getRuleIndex: Frontier -> RuleNumber
    val isPatternPartial: p:Pattern -> bool
    val erasePartialPatterns: inpp:Pattern -> Pattern
    val erasePartials: inps:Pattern list -> Pattern list
    type EdgeDiscrim =
      | EdgeDiscrim of int * TypedTree.DecisionTreeTest * Range.range
    val getDiscrim: EdgeDiscrim -> TypedTree.DecisionTreeTest
    val CompilePatternBasic:
      g:TcGlobals.TcGlobals ->
        denv:TypedTreeOps.DisplayEnv ->
          amap:Import.ImportMap ->
            tcVal:(TypedTree.ValRef -> TypedTree.ValUseFlag ->
                     TypedTree.TType list -> Range.range ->
                     TypedTree.Expr * TypedTree.TType) ->
              infoReader:InfoReader.InfoReader ->
                exprm:Range.range ->
                  matchm:Range.range ->
                    warnOnUnused:bool ->
                      warnOnIncomplete:bool ->
                        actionOnFailure:ActionOnFailure ->
                          origInputVal:TypedTree.Val *
                          origInputValTypars:TypedTree.Typar list *
                          _origInputExprOpt:TypedTree.Expr option ->
                            typedClauses:TypedMatchClause list ->
                              inputTy:TypedTree.TType ->
                                resultTy:TypedTree.TType ->
                                  TypedTree.DecisionTree *
                                  TypedTree.DecisionTreeTarget list
    val isPartialOrWhenClause: c:TypedMatchClause -> bool
    val CompilePattern:
      TcGlobals.TcGlobals ->
        TypedTreeOps.DisplayEnv ->
          Import.ImportMap ->
            (TypedTree.ValRef -> TypedTree.ValUseFlag -> TypedTree.TType list ->
               Range.range -> TypedTree.Expr * TypedTree.TType) ->
              InfoReader.InfoReader ->
                Range.range ->
                  Range.range ->
                    bool ->
                      ActionOnFailure ->
                        TypedTree.Val * TypedTree.Typar list *
                        TypedTree.Expr option ->
                          TypedMatchClause list ->
                            TypedTree.TType ->
                              TypedTree.TType ->
                                TypedTree.DecisionTree *
                                TypedTree.DecisionTreeTarget list


namespace FSharp.Compiler
  module internal ConstraintSolver =
    val compgenId: SyntaxTree.Ident
    val NewCompGenTypar:
      kind:TypedTree.TyparKind * rigid:TypedTree.TyparRigidity *
      staticReq:SyntaxTree.TyparStaticReq * dynamicReq:TypedTree.TyparDynamicReq *
      error:bool -> TypedTree.Typar
    val AnonTyparId: m:Range.range -> SyntaxTree.Ident
    val NewAnonTypar:
      TypedTree.TyparKind * Range.range * TypedTree.TyparRigidity *
      SyntaxTree.TyparStaticReq * TypedTree.TyparDynamicReq -> TypedTree.Typar
    val NewNamedInferenceMeasureVar:
      _m:'a * rigid:TypedTree.TyparRigidity * var:SyntaxTree.TyparStaticReq *
      id:SyntaxTree.Ident -> TypedTree.Typar
    val NewInferenceMeasurePar: unit -> TypedTree.Typar
    val NewErrorTypar: unit -> TypedTree.Typar
    val NewErrorMeasureVar: unit -> TypedTree.Typar
    val NewInferenceType: unit -> TypedTree.TType
    val NewErrorType: unit -> TypedTree.TType
    val NewErrorMeasure: unit -> TypedTree.Measure
    val NewByRefKindInferenceType:
      TcGlobals.TcGlobals -> Range.range -> TypedTree.TType
    val NewInferenceTypes: 'a list -> TypedTree.TType list
    val FreshenAndFixupTypars:
      Range.range ->
        TypedTree.TyparRigidity ->
          TypedTree.Typars ->
            TypedTree.TType list ->
              TypedTree.Typar list ->
                TypedTree.Typar list * TypedTreeOps.TyparInst * TypedTree.TTypes
    val FreshenTypeInst:
      Range.range ->
        TypedTree.Typar list ->
          TypedTree.Typar list * TypedTreeOps.TyparInst * TypedTree.TTypes
    val FreshMethInst:
      m:Range.range ->
        fctps:TypedTree.Typars ->
          tinst:TypedTree.TType list ->
            tpsorig:TypedTree.Typar list ->
              TypedTree.Typar list * TypedTreeOps.TyparInst * TypedTree.TTypes
    val FreshenTypars:
      Range.range -> TypedTree.Typar list -> TypedTree.TType list
    val FreshenMethInfo: Range.range -> Infos.MethInfo -> TypedTree.TTypes
    [<RequireQualifiedAccessAttribute>]
    type ContextInfo =
      | NoContext
      | IfExpression of Range.range
      | OmittedElseBranch of Range.range
      | ElseBranchResult of Range.range
      | RecordFields
      | TupleInRecordFields
      | CollectionElement of bool * Range.range
      | ReturnInComputationExpression
      | YieldInComputationExpression
      | RuntimeTypeTest of bool
      | DowncastUsedInsteadOfUpcast of bool
      | FollowingPatternMatchClause of Range.range
      | PatternMatchGuard of Range.range
      | SequenceExpression of TypedTree.TType
    type OverloadInformation =
      { methodSlot: MethodCalls.CalledMeth<TypedTree.Expr>
        amap: Import.ImportMap
        error: exn }
    type OverloadResolutionFailure =
      | NoOverloadsFound of
        methodName: string * candidates: OverloadInformation list *
        cx: TypedTree.TraitConstraintInfo option
      | PossibleCandidates of
        methodName: string * candidates: OverloadInformation list *
        cx: TypedTree.TraitConstraintInfo option
    exception ConstraintSolverTupleDiffLengths of
                                                 displayEnv:
                                                   TypedTreeOps.DisplayEnv *
                                                 TypedTree.TType list *
                                                 TypedTree.TType list *
                                                 Range.range * Range.range
    exception ConstraintSolverInfiniteTypes of
                                              displayEnv:
                                                TypedTreeOps.DisplayEnv *
                                              contextInfo: ContextInfo *
                                              TypedTree.TType * TypedTree.TType *
                                              Range.range * Range.range
    exception ConstraintSolverTypesNotInEqualityRelation of
                                                           displayEnv:
                                                             TypedTreeOps.DisplayEnv *
                                                           TypedTree.TType *
                                                           TypedTree.TType *
                                                           Range.range *
                                                           Range.range *
                                                           ContextInfo
    exception ConstraintSolverTypesNotInSubsumptionRelation of
                                                              displayEnv:
                                                                TypedTreeOps.DisplayEnv *
                                                              argTy:
                                                                TypedTree.TType *
                                                              paramTy:
                                                                TypedTree.TType *
                                                              callRange:
                                                                Range.range *
                                                              parameterRange:
                                                                Range.range
    exception ConstraintSolverMissingConstraint of
                                                  displayEnv:
                                                    TypedTreeOps.DisplayEnv *
                                                  TypedTree.Typar *
                                                  TypedTree.TyparConstraint *
                                                  Range.range * Range.range
    exception ConstraintSolverError of string * Range.range * Range.range
    exception ConstraintSolverRelatedInformation of
                                                   string option * Range.range *
                                                   exn
    exception ErrorFromApplyingDefault of
                                         tcGlobals: TcGlobals.TcGlobals *
                                         displayEnv: TypedTreeOps.DisplayEnv *
                                         TypedTree.Typar * TypedTree.TType * exn *
                                         Range.range
    exception ErrorFromAddingTypeEquation of
                                            tcGlobals: TcGlobals.TcGlobals *
                                            displayEnv: TypedTreeOps.DisplayEnv *
                                            actualTy: TypedTree.TType *
                                            expectedTy: TypedTree.TType * exn *
                                            Range.range
    exception ErrorsFromAddingSubsumptionConstraint of
                                                      tcGlobals:
                                                        TcGlobals.TcGlobals *
                                                      displayEnv:
                                                        TypedTreeOps.DisplayEnv *
                                                      actualTy: TypedTree.TType *
                                                      expectedTy:
                                                        TypedTree.TType * exn *
                                                      ContextInfo *
                                                      parameterRange:
                                                        Range.range
    exception ErrorFromAddingConstraint of
                                          displayEnv: TypedTreeOps.DisplayEnv *
                                          exn * Range.range
    exception UnresolvedOverloading of
                                      displayEnv: TypedTreeOps.DisplayEnv *
                                      callerArgs:
                                        MethodCalls.CallerArgs<TypedTree.Expr> *
                                      failure: OverloadResolutionFailure *
                                      Range.range
    exception UnresolvedConversionOperator of
                                             displayEnv: TypedTreeOps.DisplayEnv *
                                             TypedTree.TType * TypedTree.TType *
                                             Range.range
    type TcValF =
      TypedTree.ValRef -> TypedTree.ValUseFlag -> TypedTree.TType list ->
        Range.range -> TypedTree.Expr * TypedTree.TType
    [<SealedAttribute>]
    type ConstraintSolverState =
      { g: TcGlobals.TcGlobals
        amap: Import.ImportMap
        InfoReader: InfoReader.InfoReader
        TcVal: TcValF
        mutable ExtraCxs:
          Internal.Utilities.Collections.HashMultiMap<TypedTree.Stamp,
                                                      (TypedTree.TraitConstraintInfo *
                                                       Range.range)> }
      with
        static member
          New: TcGlobals.TcGlobals * Import.ImportMap * InfoReader.InfoReader *
                TcValF -> ConstraintSolverState
    
    type ConstraintSolverEnv =
      { SolverState: ConstraintSolverState
        eContextInfo: ContextInfo
        MatchingOnly: bool
        m: Range.range
        EquivEnv: TypedTreeOps.TypeEquivEnv
        DisplayEnv: TypedTreeOps.DisplayEnv }
      with
        override ToString: unit -> string
        member InfoReader: InfoReader.InfoReader
        member amap: Import.ImportMap
        member g: TcGlobals.TcGlobals
    
    val MakeConstraintSolverEnv:
      contextInfo:ContextInfo ->
        css:ConstraintSolverState ->
          m:Range.range -> denv:TypedTreeOps.DisplayEnv -> ConstraintSolverEnv
    val occursCheck:
      g:TcGlobals.TcGlobals -> un:TypedTree.Typar -> ty:TypedTree.TType -> bool
    type PermitWeakResolution =
      | Yes
      | No
      with
        member Permit: bool
    
    val isNativeIntegerTy: g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> bool
    val isSignedIntegerTy: g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> bool
    val isUnsignedIntegerTy:
      g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> bool
    val IsIntegerOrIntegerEnumTy:
      g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> bool
    val isIntegerTy: g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> bool
    val isStringTy: g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> bool
    val isCharTy: g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> bool
    val isBoolTy: g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> bool
    val isFpTy: g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> bool
    val isDecimalTy: g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> bool
    val IsNonDecimalNumericOrIntegralEnumType:
      g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> bool
    val IsNumericOrIntegralEnumType:
      g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> bool
    val IsNonDecimalNumericType:
      g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> bool
    val IsNumericType: g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> bool
    val IsRelationalType: g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> bool
    val IsCharOrStringType: g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> bool
    val IsAddSubModType:
      nm:string -> g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> bool
    val IsBitwiseOpType: g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> bool
    val IsBinaryOpOtherArgType:
      g:TcGlobals.TcGlobals ->
        permitWeakResolution:PermitWeakResolution -> ty:TypedTree.TType -> bool
    val IsSignType: g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> bool
    type TraitConstraintSolution =
      | TTraitUnsolved
      | TTraitBuiltIn
      | TTraitSolved of Infos.MethInfo * TypedTree.TypeInst
      | TTraitSolvedRecdProp of Infos.RecdFieldInfo * bool
      | TTraitSolvedAnonRecdProp of
        TypedTree.AnonRecdTypeInfo * TypedTree.TypeInst * int
    val BakedInTraitConstraintNames: Set<string>
    [<SealedAttribute (); NoEquality; NoComparison>]
    type Trace =
      { mutable actions: ((unit -> unit) * (unit -> unit)) list }
      with
        static member New: unit -> Trace
        member Push: f:(unit -> unit) -> undo:(unit -> unit) -> unit
        member Undo: unit -> unit
    
    type OptionalTrace =
      | NoTrace
      | WithTrace of Trace
      with
        member AddFromReplay: source:Trace -> unit
        member
          CollectThenUndoOrCommit: predicate:('a -> bool) ->
                                      f:(Trace -> 'a) -> 'a
        member Exec: f:(unit -> unit) -> undo:(unit -> unit) -> unit
        member HasTrace: bool
    
    val CollectThenUndo: f:(Trace -> 'a) -> 'a
    val FilterEachThenUndo:
      f:(Trace -> 'a -> ErrorLogger.OperationResult<'b>) ->
        meths:'a list -> ('a * exn list * Trace) list
    val ShowAccessDomain: ad:AccessibilityLogic.AccessorDomain -> string
    exception NonRigidTypar of
                              displayEnv: TypedTreeOps.DisplayEnv *
                              string option * Range.range * TypedTree.TType *
                              TypedTree.TType * Range.range
    exception AbortForFailedOverloadResolution
    val inline TryD_IgnoreAbortForFailedOverloadResolution:
      f1:(unit -> ErrorLogger.OperationResult<unit>) ->
        f2:(exn -> ErrorLogger.OperationResult<unit>) ->
          ErrorLogger.OperationResult<unit>
    exception ArgDoesNotMatchError of
                                     error:
                                       ErrorsFromAddingSubsumptionConstraint *
                                     calledMeth:
                                       MethodCalls.CalledMeth<TypedTree.Expr> *
                                     calledArg: MethodCalls.CalledArg *
                                     callerArg:
                                       MethodCalls.CallerArg<TypedTree.Expr>
    exception LocallyAbortOperationThatLosesAbbrevs
    val localAbortD: ErrorLogger.OperationResult<unit>
    val PreferUnifyTypar: v1:TypedTree.Typar -> v2:TypedTree.Typar -> bool
    val FindPreferredTypar:
      vs:(TypedTree.Typar * 'a) list -> (TypedTree.Typar * 'a) list
    val SubstMeasure: r:TypedTree.Typar -> ms:TypedTree.Measure -> unit
    val TransactStaticReq:
      csenv:ConstraintSolverEnv ->
        trace:OptionalTrace ->
          tpr:TypedTree.Typar ->
            req:SyntaxTree.TyparStaticReq -> ErrorLogger.OperationResult<unit>
    val SolveTypStaticReqTypar:
      csenv:ConstraintSolverEnv ->
        trace:OptionalTrace ->
          req:SyntaxTree.TyparStaticReq ->
            tpr:TypedTree.Typar -> ErrorLogger.OperationResult<unit>
    val SolveTypStaticReq:
      csenv:ConstraintSolverEnv ->
        trace:OptionalTrace ->
          req:SyntaxTree.TyparStaticReq ->
            ty:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val TransactDynamicReq:
      trace:OptionalTrace ->
        tpr:TypedTree.Typar ->
          req:TypedTree.TyparDynamicReq -> ErrorLogger.OperationResult<unit>
    val SolveTypDynamicReq:
      csenv:ConstraintSolverEnv ->
        trace:OptionalTrace ->
          req:TypedTree.TyparDynamicReq ->
            ty:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val TransactIsCompatFlex:
      trace:OptionalTrace ->
        tpr:TypedTree.Typar -> req:bool -> ErrorLogger.OperationResult<unit>
    val SolveTypIsCompatFlex:
      csenv:ConstraintSolverEnv ->
        trace:OptionalTrace ->
          req:bool -> ty:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val SubstMeasureWarnIfRigid:
      csenv:ConstraintSolverEnv ->
        trace:OptionalTrace ->
          v:TypedTree.Typar ->
            ms:TypedTree.Measure -> ErrorLogger.OperationResult<unit>
    val UnifyMeasureWithOne:
      csenv:ConstraintSolverEnv ->
        trace:OptionalTrace ->
          ms:TypedTree.Measure -> ErrorLogger.OperationResult<unit>
    val UnifyMeasures:
      csenv:ConstraintSolverEnv ->
        trace:OptionalTrace ->
          ms1:TypedTree.Measure ->
            ms2:TypedTree.Measure -> ErrorLogger.OperationResult<unit>
    val SimplifyMeasure:
      g:TcGlobals.TcGlobals ->
        vars:TypedTree.Typar list ->
          ms:TypedTree.Measure -> TypedTree.Typar list * TypedTree.Typar option
    val SimplifyMeasuresInType:
      g:TcGlobals.TcGlobals ->
        resultFirst:bool ->
          TypedTree.Typar list * TypedTree.Typar list ->
            ty:TypedTree.TType -> TypedTree.Typar list * TypedTree.Typar list
    val SimplifyMeasuresInTypes:
      g:TcGlobals.TcGlobals ->
        TypedTree.Typar list * TypedTree.Typar list ->
          tys:TypedTree.TypeInst -> TypedTree.Typar list * TypedTree.Typar list
    val SimplifyMeasuresInConstraint:
      g:TcGlobals.TcGlobals ->
        TypedTree.Typar list * TypedTree.Typar list ->
          c:TypedTree.TyparConstraint ->
            TypedTree.Typar list * TypedTree.Typar list
    val SimplifyMeasuresInConstraints:
      g:TcGlobals.TcGlobals ->
        TypedTree.Typar list * TypedTree.Typar list ->
          cs:TypedTree.TyparConstraint list ->
            TypedTree.Typar list * TypedTree.Typar list
    val GetMeasureVarGcdInType:
      v:TypedTree.Typar -> ty:TypedTree.TType -> Rational.Rational
    val GetMeasureVarGcdInTypes:
      v:TypedTree.Typar -> tys:TypedTree.TypeInst -> Rational.Rational
    val NormalizeExponentsInTypeScheme:
      uvars:TypedTree.Typar list -> ty:TypedTree.TType -> TypedTree.Typar list
    val SimplifyMeasuresInTypeScheme:
      TcGlobals.TcGlobals ->
        bool ->
          TypedTree.Typar list ->
            TypedTree.TType ->
              TypedTree.TyparConstraint list -> TypedTree.Typar list
    val freshMeasure: unit -> TypedTree.Measure
    val CheckWarnIfRigid:
      csenv:ConstraintSolverEnv ->
        ty1:TypedTree.TType ->
          r:TypedTree.Typar ->
            ty:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val SolveTyparEqualsTypePart1:
      csenv:ConstraintSolverEnv ->
        m2:Range.range ->
          trace:OptionalTrace ->
            ty1:TypedTree.TType ->
              r:TypedTree.Typar ->
                ty:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val SolveTyparEqualsTypePart2:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m2:Range.range ->
            trace:OptionalTrace ->
              r:TypedTree.Typar ->
                ty:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val solveTypMeetsTyparConstraints:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m2:Range.range ->
            trace:OptionalTrace ->
              ty:TypedTree.TType ->
                r:TypedTree.Typar -> ErrorLogger.OperationResult<unit>
    val SolveTyparEqualsType:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m2:Range.range ->
            trace:OptionalTrace ->
              ty1:TypedTree.TType ->
                ty:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val SolveTyparsEqualTypes:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m2:Range.range ->
            trace:OptionalTrace ->
              tptys:TypedTree.TType list ->
                tys:TypedTree.TType list -> ErrorLogger.OperationResult<unit>
    val SolveAnonInfoEqualsAnonInfo:
      csenv:ConstraintSolverEnv ->
        m2:Range.range ->
          anonInfo1:TypedTree.AnonRecdTypeInfo ->
            anonInfo2:TypedTree.AnonRecdTypeInfo ->
              ErrorLogger.OperationResult<unit>
    val SolveTypeEqualsType:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m2:Range.range ->
            trace:OptionalTrace ->
              cxsln:(TypedTree.TraitConstraintInfo *
                     TypedTree.TraitConstraintSln) option ->
                ty1:TypedTree.TType ->
                  ty2:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val SolveTypeEqualsTypeKeepAbbrevs:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m2:Range.range ->
            trace:OptionalTrace ->
              ty1:TypedTree.TType ->
                ty2:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val private SolveTypeEqualsTypeKeepAbbrevsWithCxsln:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m2:Range.range ->
            trace:OptionalTrace ->
              cxsln:(TypedTree.TraitConstraintInfo *
                     TypedTree.TraitConstraintSln) option ->
                ty1:TypedTree.TType ->
                  ty2:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val SolveTypeEqualsTypeEqns:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m2:Range.range ->
            trace:OptionalTrace ->
              cxsln:(TypedTree.TraitConstraintInfo *
                     TypedTree.TraitConstraintSln) option ->
                origl1:TypedTree.TypeInst ->
                  origl2:TypedTree.TypeInst -> ErrorLogger.OperationResult<unit>
    val SolveFunTypeEqn:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m2:Range.range ->
            trace:OptionalTrace ->
              cxsln:(TypedTree.TraitConstraintInfo *
                     TypedTree.TraitConstraintSln) option ->
                d1:TypedTree.TType ->
                  d2:TypedTree.TType ->
                    r1:TypedTree.TType ->
                      r2:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val SolveTypeSubsumesType:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m2:Range.range ->
            trace:OptionalTrace ->
              cxsln:(TypedTree.TraitConstraintInfo *
                     TypedTree.TraitConstraintSln) option ->
                ty1:TypedTree.TType ->
                  ty2:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val SolveTypeSubsumesTypeKeepAbbrevs:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m2:Range.range ->
            trace:OptionalTrace ->
              cxsln:(TypedTree.TraitConstraintInfo *
                     TypedTree.TraitConstraintSln) option ->
                ty1:TypedTree.TType ->
                  ty2:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val SolveTyparSubtypeOfType:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m2:Range.range ->
            trace:OptionalTrace ->
              tp:TypedTree.Typar ->
                ty1:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val DepthCheck:
      ndeep:int -> m:Range.range -> ErrorLogger.OperationResult<unit>
    val SolveDimensionlessNumericType:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m2:Range.range ->
            trace:OptionalTrace ->
              ty:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val SolveMemberConstraint:
      csenv:ConstraintSolverEnv ->
        ignoreUnresolvedOverload:bool ->
          permitWeakResolution:PermitWeakResolution ->
            ndeep:int ->
              m2:Range.range ->
                trace:OptionalTrace ->
                  traitInfo:TypedTree.TraitConstraintInfo ->
                    ErrorLogger.OperationResult<bool>
    val RecordMemberConstraintSolution:
      css:ConstraintSolverState ->
        m:Range.range ->
          trace:OptionalTrace ->
            traitInfo:TypedTree.TraitConstraintInfo ->
              res:TraitConstraintSolution -> ErrorLogger.OperationResult<bool>
    val MemberConstraintSolutionOfMethInfo:
      css:ConstraintSolverState ->
        m:Range.range ->
          minfo:Infos.MethInfo ->
            minst:TypedTree.TypeInst -> TypedTree.TraitConstraintSln
    val TransactMemberConstraintSolution:
      traitInfo:TypedTree.TraitConstraintInfo ->
        trace:OptionalTrace -> sln:TypedTree.TraitConstraintSln -> unit
    val GetRelevantMethodsForTrait:
      csenv:ConstraintSolverEnv ->
        permitWeakResolution:PermitWeakResolution ->
          nm:string -> TypedTree.TraitConstraintInfo -> Infos.MethInfo list
    val GetSupportOfMemberConstraint:
      csenv:ConstraintSolverEnv ->
        TypedTree.TraitConstraintInfo -> TypedTree.Typar list
    val SupportOfMemberConstraintIsFullySolved:
      csenv:ConstraintSolverEnv -> TypedTree.TraitConstraintInfo -> bool
    val GetFreeTyparsOfMemberConstraint:
      csenv:ConstraintSolverEnv ->
        TypedTree.TraitConstraintInfo -> TypedTree.Typar list
    val MemberConstraintIsReadyForWeakResolution:
      csenv:ConstraintSolverEnv ->
        traitInfo:TypedTree.TraitConstraintInfo -> bool
    val MemberConstraintIsReadyForStrongResolution:
      csenv:ConstraintSolverEnv ->
        traitInfo:TypedTree.TraitConstraintInfo -> bool
    val MemberConstraintSupportIsReadyForDeterminingOverloads:
      csenv:ConstraintSolverEnv ->
        traitInfo:TypedTree.TraitConstraintInfo -> bool
    val SolveRelevantMemberConstraints:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          permitWeakResolution:PermitWeakResolution ->
            trace:OptionalTrace ->
              tps:TypedTree.Typar list -> ErrorLogger.OperationResult<unit>
    val SolveRelevantMemberConstraintsForTypar:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          permitWeakResolution:PermitWeakResolution ->
            trace:OptionalTrace ->
              tp:TypedTree.Typar -> ErrorLogger.OperationResult<bool>
    val CanonicalizeRelevantMemberConstraints:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          trace:OptionalTrace ->
            tps:TypedTree.Typar list -> ErrorLogger.OperationResult<unit>
    val AddMemberConstraint:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m2:Range.range ->
            trace:OptionalTrace ->
              traitInfo:TypedTree.TraitConstraintInfo ->
                support:TypedTree.Typar list ->
                  frees:TypedTree.Typar list ->
                    ErrorLogger.OperationResult<unit>
    val AddConstraint:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m2:Range.range ->
            trace:OptionalTrace ->
              tp:TypedTree.Typar ->
                newConstraint:TypedTree.TyparConstraint ->
                  ErrorLogger.OperationResult<unit>
    val SolveTypeSupportsNull:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m2:Range.range ->
            trace:OptionalTrace ->
              ty:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val SolveTypeSupportsComparison:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m2:Range.range ->
            trace:OptionalTrace ->
              ty:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val SolveTypeSupportsEquality:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m2:Range.range ->
            trace:OptionalTrace ->
              ty:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val SolveTypeIsEnum:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m2:Range.range ->
            trace:OptionalTrace ->
              ty:TypedTree.TType ->
                underlying:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val SolveTypeIsDelegate:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m2:Range.range ->
            trace:OptionalTrace ->
              ty:TypedTree.TType ->
                aty:TypedTree.TType ->
                  bty:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val SolveTypeIsNonNullableValueType:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m2:Range.range ->
            trace:OptionalTrace ->
              ty:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val SolveTypeIsUnmanaged:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m2:Range.range ->
            trace:OptionalTrace ->
              ty:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val SolveTypeChoice:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m2:Range.range ->
            trace:OptionalTrace ->
              ty:TypedTree.TType ->
                tys:TypedTree.TTypes -> ErrorLogger.OperationResult<unit>
    val SolveTypeIsReferenceType:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m2:Range.range ->
            trace:OptionalTrace ->
              ty:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val SolveTypeRequiresDefaultConstructor:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m2:Range.range ->
            trace:OptionalTrace ->
              origTy:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val SolveTypeRequiresDefaultValue:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m2:Range.range ->
            trace:OptionalTrace ->
              origTy:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val CanMemberSigsMatchUpToCheck:
      csenv:ConstraintSolverEnv ->
        permitOptArgs:bool ->
          alwaysCheckReturn:bool ->
            unifyTypes:(TypedTree.TType -> TypedTree.TType ->
                          ErrorLogger.OperationResult<unit>) ->
              subsumeTypes:(TypedTree.TType -> TypedTree.TType ->
                              ErrorLogger.OperationResult<unit>) ->
                subsumeArg:(MethodCalls.CalledArg ->
                              MethodCalls.CallerArg<'a> ->
                              ErrorLogger.OperationResult<unit>) ->
                  reqdRetTyOpt:TypedTree.TType option ->
                    calledMeth:MethodCalls.CalledMeth<'a> ->
                      ErrorLogger.ImperativeOperationResult
    val private SolveTypeSubsumesTypeWithWrappedContextualReport:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m:Range.range ->
            trace:OptionalTrace ->
              cxsln:(TypedTree.TraitConstraintInfo *
                     TypedTree.TraitConstraintSln) option ->
                ty1:TypedTree.TType ->
                  ty2:TypedTree.TType ->
                    wrapper:(exn -> #exn) -> ErrorLogger.OperationResult<unit>
    val private SolveTypeSubsumesTypeWithReport:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m:Range.range ->
            trace:OptionalTrace ->
              cxsln:(TypedTree.TraitConstraintInfo *
                     TypedTree.TraitConstraintSln) option ->
                ty1:TypedTree.TType ->
                  ty2:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val private SolveTypeEqualsTypeWithReport:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          m:Range.range ->
            trace:OptionalTrace ->
              cxsln:(TypedTree.TraitConstraintInfo *
                     TypedTree.TraitConstraintSln) option ->
                actual:TypedTree.TType ->
                  expected:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val ArgsMustSubsumeOrConvert:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          trace:OptionalTrace ->
            cxsln:(TypedTree.TraitConstraintInfo * TypedTree.TraitConstraintSln) option ->
              isConstraint:bool ->
                enforceNullableOptionalsKnownTypes:bool ->
                  calledArg:MethodCalls.CalledArg ->
                    callerArg:MethodCalls.CallerArg<'T> ->
                      ErrorLogger.OperationResult<unit>
    val MustUnify:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          trace:OptionalTrace ->
            cxsln:(TypedTree.TraitConstraintInfo * TypedTree.TraitConstraintSln) option ->
              ty1:TypedTree.TType ->
                ty2:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val MustUnifyInsideUndo:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          trace:Trace ->
            cxsln:(TypedTree.TraitConstraintInfo * TypedTree.TraitConstraintSln) option ->
              ty1:TypedTree.TType ->
                ty2:TypedTree.TType -> ErrorLogger.OperationResult<unit>
    val ArgsMustSubsumeOrConvertInsideUndo:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          trace:Trace ->
            cxsln:(TypedTree.TraitConstraintInfo * TypedTree.TraitConstraintSln) option ->
              isConstraint:bool ->
                calledMeth:MethodCalls.CalledMeth<TypedTree.Expr> ->
                  calledArg:MethodCalls.CalledArg ->
                    MethodCalls.CallerArg<TypedTree.Expr> ->
                      ErrorLogger.OperationResult<unit>
    val TypesMustSubsumeOrConvertInsideUndo:
      csenv:ConstraintSolverEnv ->
        ndeep:int ->
          trace:OptionalTrace ->
            cxsln:(TypedTree.TraitConstraintInfo * TypedTree.TraitConstraintSln) option ->
              m:Range.range ->
                calledArgTy:TypedTree.TType ->
                  callerArgTy:TypedTree.TType ->
                    ErrorLogger.OperationResult<unit>
    val ArgsEquivInsideUndo:
      csenv:ConstraintSolverEnv ->
        isConstraint:bool ->
          calledArg:MethodCalls.CalledArg ->
            MethodCalls.CallerArg<'c> -> ErrorLogger.OperationResult<unit>
    val ReportNoCandidatesError:
      csenv:ConstraintSolverEnv ->
        nUnnamedCallerArgs:System.Int32 * nNamedCallerArgs:int ->
          methodName:System.String ->
            ad:AccessibilityLogic.AccessorDomain ->
              calledMethGroup:MethodCalls.CalledMeth<'d> list ->
                isSequential:('d -> bool) -> ErrorLogger.OperationResult<'e>
    val ReportNoCandidatesErrorExpr:
      csenv:ConstraintSolverEnv ->
        System.Int32 * int ->
          methodName:System.String ->
            ad:AccessibilityLogic.AccessorDomain ->
              calledMethGroup:MethodCalls.CalledMeth<TypedTree.Expr> list ->
                ErrorLogger.OperationResult<'f>
    val ReportNoCandidatesErrorSynExpr:
      csenv:ConstraintSolverEnv ->
        System.Int32 * int ->
          methodName:System.String ->
            ad:AccessibilityLogic.AccessorDomain ->
              calledMethGroup:MethodCalls.CalledMeth<SyntaxTree.SynExpr> list ->
                ErrorLogger.OperationResult<'g>
    val ResolveOverloading:
      csenv:ConstraintSolverEnv ->
        trace:OptionalTrace ->
          methodName:string ->
            ndeep:int ->
              cx:TypedTree.TraitConstraintInfo option ->
                callerArgs:MethodCalls.CallerArgs<TypedTree.Expr> ->
                  ad:AccessibilityLogic.AccessorDomain ->
                    calledMethGroup:MethodCalls.CalledMeth<TypedTree.Expr> list ->
                      permitOptArgs:bool ->
                        reqdRetTyOpt:TypedTree.TType option ->
                          MethodCalls.CalledMeth<TypedTree.Expr> option *
                          ErrorLogger.OperationResult<unit>
    val ResolveOverloadingForCall:
      TypedTreeOps.DisplayEnv ->
        ConstraintSolverState ->
          Range.range ->
            methodName:string ->
              ndeep:int ->
                cx:TypedTree.TraitConstraintInfo option ->
                  callerArgs:MethodCalls.CallerArgs<TypedTree.Expr> ->
                    AccessibilityLogic.AccessorDomain ->
                      calledMethGroup:MethodCalls.CalledMeth<TypedTree.Expr> list ->
                        permitOptArgs:bool ->
                          reqdRetTyOpt:TypedTree.TType option ->
                            MethodCalls.CalledMeth<TypedTree.Expr> option *
                            ErrorLogger.OperationResult<unit>
    val UnifyUniqueOverloading:
      TypedTreeOps.DisplayEnv ->
        ConstraintSolverState ->
          Range.range ->
            System.Int32 * int ->
              System.String ->
                AccessibilityLogic.AccessorDomain ->
                  MethodCalls.CalledMeth<SyntaxTree.SynExpr> list ->
                    TypedTree.TType -> ErrorLogger.OperationResult<bool>
    val EliminateConstraintsForGeneralizedTypars:
      TypedTreeOps.DisplayEnv ->
        ConstraintSolverState ->
          Range.range -> OptionalTrace -> TypedTree.Typars -> unit
    val AddCxTypeEqualsType:
      ContextInfo ->
        TypedTreeOps.DisplayEnv ->
          ConstraintSolverState ->
            Range.range -> TypedTree.TType -> TypedTree.TType -> unit
    val UndoIfFailed: f:(Trace -> ErrorLogger.OperationResult<'a>) -> bool
    val UndoIfFailedOrWarnings:
      f:(Trace -> ErrorLogger.OperationResult<'a>) -> bool
    val AddCxTypeEqualsTypeUndoIfFailed:
      TypedTreeOps.DisplayEnv ->
        ConstraintSolverState ->
          Range.range -> TypedTree.TType -> TypedTree.TType -> bool
    val AddCxTypeEqualsTypeUndoIfFailedOrWarnings:
      TypedTreeOps.DisplayEnv ->
        ConstraintSolverState ->
          Range.range -> TypedTree.TType -> TypedTree.TType -> bool
    val AddCxTypeEqualsTypeMatchingOnlyUndoIfFailed:
      TypedTreeOps.DisplayEnv ->
        ConstraintSolverState ->
          Range.range -> TypedTree.TType -> TypedTree.TType -> bool
    val AddCxTypeMustSubsumeTypeUndoIfFailed:
      TypedTreeOps.DisplayEnv ->
        ConstraintSolverState ->
          Range.range -> TypedTree.TType -> TypedTree.TType -> bool
    val AddCxTypeMustSubsumeTypeMatchingOnlyUndoIfFailed:
      TypedTreeOps.DisplayEnv ->
        ConstraintSolverState ->
          Range.range -> TypedTree.TType -> TypedTree.TType -> bool
    val AddCxTypeMustSubsumeType:
      ContextInfo ->
        TypedTreeOps.DisplayEnv ->
          ConstraintSolverState ->
            Range.range ->
              OptionalTrace -> TypedTree.TType -> TypedTree.TType -> unit
    val AddCxMethodConstraint:
      TypedTreeOps.DisplayEnv ->
        ConstraintSolverState ->
          Range.range -> OptionalTrace -> TypedTree.TraitConstraintInfo -> unit
    val AddCxTypeMustSupportNull:
      TypedTreeOps.DisplayEnv ->
        ConstraintSolverState ->
          Range.range -> OptionalTrace -> TypedTree.TType -> unit
    val AddCxTypeMustSupportComparison:
      TypedTreeOps.DisplayEnv ->
        ConstraintSolverState ->
          Range.range -> OptionalTrace -> TypedTree.TType -> unit
    val AddCxTypeMustSupportEquality:
      TypedTreeOps.DisplayEnv ->
        ConstraintSolverState ->
          Range.range -> OptionalTrace -> TypedTree.TType -> unit
    val AddCxTypeMustSupportDefaultCtor:
      TypedTreeOps.DisplayEnv ->
        ConstraintSolverState ->
          Range.range -> OptionalTrace -> TypedTree.TType -> unit
    val AddCxTypeIsReferenceType:
      TypedTreeOps.DisplayEnv ->
        ConstraintSolverState ->
          Range.range -> OptionalTrace -> TypedTree.TType -> unit
    val AddCxTypeIsValueType:
      TypedTreeOps.DisplayEnv ->
        ConstraintSolverState ->
          Range.range -> OptionalTrace -> TypedTree.TType -> unit
    val AddCxTypeIsUnmanaged:
      TypedTreeOps.DisplayEnv ->
        ConstraintSolverState ->
          Range.range -> OptionalTrace -> TypedTree.TType -> unit
    val AddCxTypeIsEnum:
      TypedTreeOps.DisplayEnv ->
        ConstraintSolverState ->
          Range.range ->
            OptionalTrace -> TypedTree.TType -> TypedTree.TType -> unit
    val AddCxTypeIsDelegate:
      TypedTreeOps.DisplayEnv ->
        ConstraintSolverState ->
          Range.range ->
            OptionalTrace ->
              TypedTree.TType -> TypedTree.TType -> TypedTree.TType -> unit
    val AddCxTyparDefaultsTo:
      TypedTreeOps.DisplayEnv ->
        ConstraintSolverState ->
          Range.range ->
            ContextInfo -> TypedTree.Typar -> int -> TypedTree.TType -> unit
    val SolveTypeAsError:
      TypedTreeOps.DisplayEnv ->
        ConstraintSolverState -> Range.range -> TypedTree.TType -> unit
    val ApplyTyparDefaultAtPriority:
      TypedTreeOps.DisplayEnv ->
        ConstraintSolverState -> priority:int -> TypedTree.Typar -> unit
    val CreateCodegenState:
      tcVal:TcValF ->
        g:TcGlobals.TcGlobals -> amap:Import.ImportMap -> ConstraintSolverState
    val CodegenWitnessExprForTraitConstraint:
      TcValF ->
        TcGlobals.TcGlobals ->
          Import.ImportMap ->
            Range.range ->
              TypedTree.TraitConstraintInfo ->
                TypedTree.Expr list ->
                  ErrorLogger.OperationResult<TypedTree.Expr option>
    val CodegenWitnessesForTyparInst:
      TcValF ->
        TcGlobals.TcGlobals ->
          Import.ImportMap ->
            Range.range ->
              TypedTree.Typar list ->
                TypedTree.TType list ->
                  ErrorLogger.OperationResult<Choice<TypedTree.TraitConstraintInfo,
                                                     TypedTree.Expr> list>
    val CodegenWitnessArgForTraitConstraint:
      TcValF ->
        TcGlobals.TcGlobals ->
          Import.ImportMap ->
            Range.range ->
              TypedTree.TraitConstraintInfo ->
                ErrorLogger.OperationResult<Choice<TypedTree.TraitConstraintInfo,
                                                   TypedTree.Expr>>
    val ChooseTyparSolutionAndSolve:
      ConstraintSolverState ->
        TypedTreeOps.DisplayEnv -> TypedTree.Typar -> unit
    val CheckDeclaredTypars:
      TypedTreeOps.DisplayEnv ->
        ConstraintSolverState ->
          Range.range -> TypedTree.Typar list -> TypedTree.Typar list -> unit
    val CanonicalizePartialInferenceProblem:
      ConstraintSolverState ->
        TypedTreeOps.DisplayEnv -> Range.range -> TypedTree.Typar list -> unit
    val IsApplicableMethApprox:
      TcGlobals.TcGlobals ->
        Import.ImportMap ->
          Range.range -> Infos.MethInfo -> TypedTree.TType -> bool


namespace FSharp.Compiler
  module internal CheckFormatStrings =
    type FormatItem =
      | Simple of TypedTree.TType
      | FuncAndVal
    val copyAndFixupFormatTypar:
      m:Range.range -> tp:TypedTree.Typar -> TypedTree.TType
    val lowestDefaultPriority: int
    val mkFlexibleFormatTypar:
      m:Range.range ->
        tys:TypedTree.TTypes -> dflt:TypedTree.TType -> TypedTree.TType
    val mkFlexibleIntFormatTypar:
      g:TcGlobals.TcGlobals -> m:Range.range -> TypedTree.TType
    val mkFlexibleDecimalFormatTypar:
      g:TcGlobals.TcGlobals -> m:Range.range -> TypedTree.TType
    val mkFlexibleFloatFormatTypar:
      g:TcGlobals.TcGlobals -> m:Range.range -> TypedTree.TType
    type FormatInfoRegister =
      { mutable leftJustify: bool
        mutable numPrefixIfPos: char option
        mutable addZeros: bool
        mutable precision: bool }
    val newInfo: unit -> FormatInfoRegister
    val parseFormatStringInternal:
      m:Range.range ->
        fragRanges:Range.range list ->
          g:TcGlobals.TcGlobals ->
            isInterpolated:bool ->
              isFormattableString:bool ->
                context:NameResolution.FormatStringCheckContext option ->
                  fmt:string ->
                    printerArgTy:TypedTree.TType ->
                      printerResidueTy:TypedTree.TType ->
                        TypedTree.TType list * (Range.range * int) list * string *
                        TypedTree.TType []
    val ParseFormatString:
      m:Range.range ->
        fragmentRanges:Range.range list ->
          g:TcGlobals.TcGlobals ->
            isInterpolated:bool ->
              isFormattableString:bool ->
                formatStringCheckContext:NameResolution.FormatStringCheckContext option ->
                  fmt:string ->
                    printerArgTy:TypedTree.TType ->
                      printerResidueTy:TypedTree.TType ->
                        printerResultTy:TypedTree.TType ->
                          TypedTree.TType list * TypedTree.TType *
                          TypedTree.TType * TypedTree.TType [] *
                          (Range.range * int) list * string
    val TryCountFormatStringArguments:
      m:Range.range ->
        g:TcGlobals.TcGlobals ->
          isInterpolated:bool ->
            fmt:string ->
              printerArgTy:TypedTree.TType ->
                printerResidueTy:TypedTree.TType -> int option


namespace FSharp.Compiler
  module internal FindUnsolved =
    type env = | Nix
    type cenv =
      { g: TcGlobals.TcGlobals
        amap: Import.ImportMap
        denv: TypedTreeOps.DisplayEnv
        mutable unsolved: TypedTree.Typars }
      with
        override ToString: unit -> string
    
    val accTy: cenv:cenv -> _env:'a -> ty:TypedTree.TType -> unit
    val accTypeInst: cenv:cenv -> env:'a -> tyargs:TypedTree.TType list -> unit
    val accExpr: cenv:cenv -> env:env -> expr:TypedTree.Expr -> unit
    val accMethods:
      cenv:cenv ->
        env:env ->
          baseValOpt:TypedTree.Val option ->
            l:TypedTree.ObjExprMethod list -> unit
    val accMethod:
      cenv:cenv ->
        env:env ->
          _baseValOpt:TypedTree.Val option -> TypedTree.ObjExprMethod -> unit
    val accIntfImpls:
      cenv:cenv ->
        env:env ->
          baseValOpt:TypedTree.Val option ->
            l:(TypedTree.TType * TypedTree.ObjExprMethod list) list -> unit
    val accIntfImpl:
      cenv:cenv ->
        env:env ->
          baseValOpt:TypedTree.Val option ->
            ty:TypedTree.TType * overrides:TypedTree.ObjExprMethod list -> unit
    val accOp:
      cenv:cenv ->
        env:env ->
          op:TypedTree.TOp * tyargs:TypedTree.TypeInst * args:TypedTree.Exprs *
          _m:Range.range -> unit
    val accTraitInfo:
      cenv:cenv -> env:env -> TypedTree.TraitConstraintInfo -> unit
    val accLambdas:
      cenv:cenv ->
        env:env ->
          topValInfo:TypedTree.ValReprInfo ->
            e:TypedTree.Expr -> ety:TypedTree.TType -> unit
    val accExprs: cenv:cenv -> env:env -> exprs:TypedTree.Expr list -> unit
    val accTargets:
      cenv:cenv ->
        env:env ->
          m:Range.range ->
            ty:TypedTree.TType ->
              targets:TypedTree.DecisionTreeTarget array -> unit
    val accTarget:
      cenv:cenv ->
        env:env ->
          _m:Range.range ->
            _ty:TypedTree.TType -> TypedTree.DecisionTreeTarget -> unit
    val accDTree: cenv:cenv -> env:env -> x:TypedTree.DecisionTree -> unit
    val accSwitch:
      cenv:cenv ->
        env:env ->
          e:TypedTree.Expr * cases:TypedTree.DecisionTreeCase list *
          dflt:TypedTree.DecisionTree option * _m:Range.range -> unit
    val accDiscrim:
      cenv:cenv -> env:env -> d:TypedTree.DecisionTreeTest -> unit
    val accAttrib: cenv:cenv -> env:env -> TypedTree.Attrib -> unit
    val accAttribs:
      cenv:cenv -> env:env -> attribs:TypedTree.Attrib list -> unit
    val accValReprInfo: cenv:cenv -> env:env -> TypedTree.ValReprInfo -> unit
    val accArgReprInfo:
      cenv:cenv -> env:env -> argInfo:TypedTree.ArgReprInfo -> unit
    val accVal: cenv:cenv -> env:env -> v:TypedTree.Val -> unit
    val accBind: cenv:cenv -> env:env -> bind:TypedTree.Binding -> unit
    val accBinds: cenv:cenv -> env:env -> xs:TypedTree.Bindings -> unit
    val accTyconRecdField:
      cenv:cenv -> env:env -> _tycon:'a -> rfield:TypedTree.RecdField -> unit
    val accTycon: cenv:cenv -> env:env -> tycon:TypedTree.Tycon -> unit
    val accTycons: cenv:cenv -> env:env -> tycons:TypedTree.Tycon list -> unit
    val accModuleOrNamespaceExpr:
      cenv:cenv -> env:env -> x:TypedTree.ModuleOrNamespaceExprWithSig -> unit
    val accModuleOrNamespaceDefs:
      cenv:cenv -> env:env -> x:TypedTree.ModuleOrNamespaceExpr list -> unit
    val accModuleOrNamespaceDef:
      cenv:cenv -> env:env -> x:TypedTree.ModuleOrNamespaceExpr -> unit
    val accModuleOrNamespaceBinds:
      cenv:cenv -> env:env -> xs:TypedTree.ModuleOrNamespaceBinding list -> unit
    val accModuleOrNamespaceBind:
      cenv:cenv -> env:env -> x:TypedTree.ModuleOrNamespaceBinding -> unit
    val UnsolvedTyparsOfModuleDef:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          denv:TypedTreeOps.DisplayEnv ->
            mdef:TypedTree.ModuleOrNamespaceExpr *
            extraAttribs:TypedTree.Attrib list -> TypedTree.Typar list


namespace FSharp.Compiler
  module internal QuotationTranslator =
    val verboseCReflect: bool
    [<RequireQualifiedAccessAttribute>]
    type IsReflectedDefinition =
      | Yes
      | No
    [<RequireQualifiedAccessAttribute>]
    type QuotationSerializationFormat =
      { SupportsWitnesses: bool
        SupportsDeserializeEx: bool }
    [<SealedAttribute>]
    type QuotationGenerationScope =
      { g: TcGlobals.TcGlobals
        amap: Import.ImportMap
        scope: TypedTree.CcuThunk
        tcVal: ConstraintSolver.TcValF
        referencedTypeDefs: ResizeArray<AbstractIL.IL.ILTypeRef>
        referencedTypeDefsTable:
          System.Collections.Generic.Dictionary<AbstractIL.IL.ILTypeRef,int>
        typeSplices: ResizeArray<TypedTree.Typar * Range.range>
        exprSplices: ResizeArray<TypedTree.Expr * Range.range>
        isReflectedDefinition: IsReflectedDefinition
        quotationFormat: QuotationSerializationFormat
        mutable emitDebugInfoInQuotations: bool }
      with
        static member
          ComputeQuotationFormat: TcGlobals.TcGlobals ->
                                     QuotationSerializationFormat
        static member
          Create: TcGlobals.TcGlobals * Import.ImportMap * TypedTree.CcuThunk *
                   ConstraintSolver.TcValF * IsReflectedDefinition ->
                     QuotationGenerationScope
        member
          Close: unit ->
                    AbstractIL.IL.ILTypeRef list *
                    (TypedTree.TType * Range.range) list *
                    (TypedTree.Expr * Range.range) list
    
    type QuotationTranslationEnv =
      { vs: TypedTreeOps.ValMap<int>
        numValsInScope: int
        tyvs: TypedTree.StampMap<int>
        suppressWitnesses: bool
        witnessesInScope: TypedTreeOps.TraitWitnessInfoHashMap<int>
        isinstVals: TypedTreeOps.ValMap<TypedTree.TType * TypedTree.Expr>
        substVals: TypedTreeOps.ValMap<TypedTree.Expr> }
      with
        static member
          CreateEmpty: g:TcGlobals.TcGlobals -> QuotationTranslationEnv
        member BindTypar: v:TypedTree.Typar -> QuotationTranslationEnv
        member BindTypars: vs:TypedTree.Typar list -> QuotationTranslationEnv
        member
          BindWitnessInfo: witnessInfo:TypedTree.TraitWitnessInfo ->
                              QuotationTranslationEnv
        member
          BindWitnessInfos: witnessInfos:TypedTree.TraitWitnessInfo list ->
                               QuotationTranslationEnv
    
    val BindFormalTypars:
      env:QuotationTranslationEnv ->
        vs:TypedTree.Typar list -> QuotationTranslationEnv
    val BindVal:
      env:QuotationTranslationEnv -> v:TypedTree.Val -> QuotationTranslationEnv
    val BindIsInstVal:
      env:QuotationTranslationEnv ->
        v:TypedTree.Val ->
          ty:TypedTree.TType * e:TypedTree.Expr -> QuotationTranslationEnv
    val BindSubstVal:
      env:QuotationTranslationEnv ->
        v:TypedTree.Val -> e:TypedTree.Expr -> QuotationTranslationEnv
    val BindVals:
      env:QuotationTranslationEnv ->
        vs:TypedTree.Val list -> QuotationTranslationEnv
    val BindFlatVals:
      env:QuotationTranslationEnv ->
        vs:TypedTree.Val list -> QuotationTranslationEnv
    exception InvalidQuotedTerm of exn
    exception IgnoringPartOfQuotedTermWarning of string * Range.range
    val wfail: e:exn -> 'a
    val ( |ModuleValueOrMemberUse|_| ):
      TcGlobals.TcGlobals ->
        TypedTree.Expr ->
          (TypedTree.ValRef * TypedTree.ValUseFlag * TypedTree.Expr *
           TypedTree.TType * TypedTree.TypeInst * TypedTree.Expr list) option
    val ( |SimpleArrayLoopUpperBound|_| ): TypedTree.Expr -> unit option
    val ( |SimpleArrayLoopBody|_| ):
      TcGlobals.TcGlobals ->
        TypedTree.Expr ->
          (TypedTree.Expr * TypedTree.TType * TypedTree.Expr) option
    val ( |ObjectInitializationCheck|_| ):
      TcGlobals.TcGlobals -> TypedTree.Expr -> unit option
    val isSplice: TcGlobals.TcGlobals -> TypedTree.ValRef -> bool
    val EmitDebugInfoIfNecessary:
      cenv:QuotationGenerationScope ->
        env:QuotationTranslationEnv ->
          m:Range.range ->
            astExpr:QuotationPickler.ExprData -> QuotationPickler.ExprData
    val ConvExpr:
      cenv:QuotationGenerationScope ->
        env:QuotationTranslationEnv ->
          expr:TypedTree.Expr -> QuotationPickler.ExprData
    val GetWitnessArgs:
      cenv:QuotationGenerationScope ->
        env:QuotationTranslationEnv ->
          m:Range.range ->
            tps:TypedTree.Typars ->
              tyargs:TypedTree.TType list -> QuotationPickler.ExprData list
    val ConvWitnessInfo:
      cenv:QuotationGenerationScope ->
        env:QuotationTranslationEnv ->
          m:Range.range ->
            traitInfo:TypedTree.TraitConstraintInfo -> QuotationPickler.ExprData
    val private ConvExprCore:
      cenv:QuotationGenerationScope ->
        env:QuotationTranslationEnv ->
          expr:TypedTree.Expr -> QuotationPickler.ExprData
    val ConvLdfld:
      cenv:QuotationGenerationScope ->
        env:QuotationTranslationEnv ->
          m:Range.range ->
            fspec:AbstractIL.IL.ILFieldSpec ->
              enclTypeArgs:TypedTree.TypeInst ->
                args:TypedTree.Exprs -> QuotationPickler.ExprData
    val ConvUnionFieldGet:
      cenv:QuotationGenerationScope ->
        env:QuotationTranslationEnv ->
          m:Range.range ->
            ucref:TypedTree.UnionCaseRef ->
              n:int ->
                tyargs:TypedTree.TypeInst ->
                  e:TypedTree.Expr -> QuotationPickler.ExprData
    val ConvClassOrRecdFieldGet:
      cenv:QuotationGenerationScope ->
        env:QuotationTranslationEnv ->
          m:Range.range ->
            rfref:TypedTree.RecdFieldRef ->
              tyargs:TypedTree.TypeInst ->
                args:TypedTree.Exprs -> QuotationPickler.ExprData
    val private ConvClassOrRecdFieldGetCore:
      cenv:QuotationGenerationScope ->
        env:QuotationTranslationEnv ->
          m:Range.range ->
            rfref:TypedTree.RecdFieldRef ->
              tyargs:TypedTree.TypeInst ->
                args:TypedTree.Exprs -> QuotationPickler.ExprData
    val ConvLetBind:
      cenv:QuotationGenerationScope ->
        env:QuotationTranslationEnv ->
          bind:TypedTree.Binding ->
            (QuotationPickler.VarData * QuotationPickler.ExprData) option *
            QuotationTranslationEnv
    val ConvLValueArgs:
      cenv:QuotationGenerationScope ->
        env:QuotationTranslationEnv ->
          args:TypedTree.Exprs -> QuotationPickler.ExprData list
    val ConvLValueExpr:
      cenv:QuotationGenerationScope ->
        env:QuotationTranslationEnv ->
          expr:TypedTree.Expr -> QuotationPickler.ExprData
    val ConvLValueExprCore:
      cenv:QuotationGenerationScope ->
        env:QuotationTranslationEnv ->
          expr:TypedTree.Expr -> QuotationPickler.ExprData
    val ConvObjectModelCall:
      cenv:QuotationGenerationScope ->
        env:QuotationTranslationEnv ->
          m:Range.range ->
            bool * bool * bool * QuotationPickler.NamedTypeData *
            QuotationPickler.TypeData list * QuotationPickler.TypeData list *
            QuotationPickler.TypeData * string * TypedTree.TypeInst * int *
            TypedTree.Expr list * QuotationPickler.ExprData list *
            TypedTree.Expr list list -> QuotationPickler.ExprData
    val ConvObjectModelCallCore:
      cenv:QuotationGenerationScope ->
        env:QuotationTranslationEnv ->
          m:Range.range ->
            isPropGet:bool * isPropSet:bool * isNewObj:bool *
            parentTyconR:QuotationPickler.NamedTypeData *
            witnessArgTypesR:QuotationPickler.TypeData list *
            methArgTypesR:QuotationPickler.TypeData list *
            methRetTypeR:QuotationPickler.TypeData * methName:string *
            tyargs:TypedTree.TypeInst * numGenericArgs:int *
            objArgs:TypedTree.Expr list *
            witnessArgsR:QuotationPickler.ExprData list *
            untupledCurriedArgs:TypedTree.Expr list list ->
              QuotationPickler.ExprData
    val ConvModuleValueApp:
      cenv:QuotationGenerationScope ->
        env:QuotationTranslationEnv ->
          m:Range.range ->
            vref:TypedTree.ValRef ->
              tyargs:TypedTree.TypeInst ->
                witnessArgs:QuotationPickler.ExprData list ->
                  args:TypedTree.Expr list list -> QuotationPickler.ExprData
    val ConvModuleValueAppCore:
      cenv:QuotationGenerationScope ->
        env:QuotationTranslationEnv ->
          m:Range.range ->
            vref:TypedTree.ValRef ->
              tyargs:TypedTree.TypeInst ->
                witnessArgsR:QuotationPickler.ExprData list ->
                  curriedArgs:TypedTree.Expr list list ->
                    QuotationPickler.ExprData
    val ConvExprs:
      cenv:QuotationGenerationScope ->
        env:QuotationTranslationEnv ->
          args:TypedTree.Exprs -> QuotationPickler.ExprData list
    val ConvValRef:
      holeOk:bool ->
        cenv:QuotationGenerationScope ->
          env:QuotationTranslationEnv ->
            m:Range.range ->
              vref:TypedTree.ValRef ->
                tyargs:TypedTree.TypeInst -> QuotationPickler.ExprData
    val private ConvValRefCore:
      holeOk:bool ->
        cenv:QuotationGenerationScope ->
          env:QuotationTranslationEnv ->
            m:Range.range ->
              vref:TypedTree.ValRef ->
                tyargs:TypedTree.TypeInst -> QuotationPickler.ExprData
    val ConvUnionCaseRef:
      cenv:QuotationGenerationScope ->
        ucref:TypedTree.UnionCaseRef ->
          m:Range.range -> QuotationPickler.NamedTypeData * string
    val ConvRecdFieldRef:
      cenv:QuotationGenerationScope ->
        rfref:TypedTree.RecdFieldRef ->
          m:Range.range -> QuotationPickler.NamedTypeData * string
    val ConvVal:
      cenv:QuotationGenerationScope ->
        env:QuotationTranslationEnv ->
          v:TypedTree.Val -> QuotationPickler.VarData
    val ConvTyparRef:
      cenv:QuotationGenerationScope ->
        env:QuotationTranslationEnv ->
          m:Range.range -> tp:TypedTree.Typar -> int
    val FilterMeasureTyargs: tys:TypedTree.TType list -> TypedTree.TType list
    val ConvType:
      cenv:QuotationGenerationScope ->
        env:QuotationTranslationEnv ->
          m:Range.range -> ty:TypedTree.TType -> QuotationPickler.TypeData
    val ConvTypes:
      cenv:QuotationGenerationScope ->
        env:QuotationTranslationEnv ->
          m:Range.range ->
            tys:TypedTree.TType list -> QuotationPickler.TypeData list
    val ConvConst:
      cenv:QuotationGenerationScope ->
        env:QuotationTranslationEnv ->
          m:Range.range ->
            c:TypedTree.Const -> ty:TypedTree.TType -> QuotationPickler.ExprData
    val ConvDecisionTree:
      cenv:QuotationGenerationScope ->
        env:QuotationTranslationEnv ->
          tgs:TypedTree.DecisionTreeTarget array ->
            typR:QuotationPickler.TypeData ->
              x:TypedTree.DecisionTree -> QuotationPickler.ExprData
    val IsILTypeRefStaticLinkLocal:
      cenv:QuotationGenerationScope ->
        m:Range.range -> tr:AbstractIL.IL.ILTypeRef -> bool
    val ConvILTypeRefUnadjusted:
      cenv:QuotationGenerationScope ->
        m:Range.range ->
          tr:AbstractIL.IL.ILTypeRef -> QuotationPickler.NamedTypeData
    val ConvILTypeRef:
      cenv:QuotationGenerationScope ->
        tr:AbstractIL.IL.ILTypeRef -> QuotationPickler.NamedTypeData
    val ConvVoidType:
      cenv:QuotationGenerationScope ->
        m:Range.range -> QuotationPickler.TypeData
    val ConvILType:
      cenv:QuotationGenerationScope ->
        env:QuotationTranslationEnv ->
          m:Range.range -> ty:AbstractIL.IL.ILType -> QuotationPickler.TypeData
    val TryElimErasableTyconRef:
      cenv:QuotationGenerationScope ->
        m:Range.range -> tcref:TypedTree.TyconRef -> TypedTree.TType option
    val ConvTyconRef:
      cenv:QuotationGenerationScope ->
        tcref:TypedTree.TyconRef ->
          m:Range.range -> QuotationPickler.NamedTypeData
    val ConvReturnType:
      cenv:QuotationGenerationScope ->
        envinner:QuotationTranslationEnv ->
          m:Range.range ->
            retTy:TypedTree.TType option -> QuotationPickler.TypeData
    val ConvExprPublic:
      QuotationGenerationScope ->
        suppressWitnesses:bool -> TypedTree.Expr -> QuotationPickler.ExprData
    val ConvMethodBase:
      cenv:QuotationGenerationScope ->
        env:QuotationTranslationEnv ->
          methName:string * v:TypedTree.Val -> QuotationPickler.MethodBaseData
    val ConvReflectedDefinition:
      QuotationGenerationScope ->
        string ->
          TypedTree.Val ->
            TypedTree.Expr ->
              QuotationPickler.MethodBaseData * QuotationPickler.ExprData


namespace FSharp.Compiler
  module internal PostTypeCheckSemanticChecks =
    type env =
      { boundTyparNames: string list
        boundTypars: TypedTreeOps.TyparMap<unit>
        argVals: TypedTreeOps.ValMap<unit>
        sigToImplRemapInfo:
          (TypedTreeOps.Remap * TypedTreeOps.SignatureHidingInfo) list
        ctorLimitedZone: bool
        quote: bool
        reflect: bool
        external: bool
        returnScope: int
        isInAppExpr: bool }
      with
        override ToString: unit -> string
    
    val BindTypar: env:env -> tp:TypedTree.Typar -> env
    val BindTypars:
      g:TcGlobals.TcGlobals -> env:env -> tps:TypedTree.Typar list -> env
    val BindArgVals: env:env -> vs:TypedTree.Val list -> env
    [<System.Flags>]
    type LimitFlags =
      | None = 0
      | ByRef = 1
      | ByRefOfSpanLike = 3
      | ByRefOfStackReferringSpanLike = 5
      | SpanLike = 8
      | StackReferringSpanLike = 16
    [<StructAttribute>]
    type Limit =
      { scope: int
        flags: LimitFlags }
      with
        member IsLocal: bool
    
    val inline HasLimitFlag: targetLimit:LimitFlags -> limit:Limit -> bool
    val NoLimit: Limit
    val CombineTwoLimits: limit1:Limit -> limit2:Limit -> Limit
    val CombineLimits: limits:Limit list -> Limit
    type cenv =
      { boundVals: System.Collections.Generic.Dictionary<TypedTree.Stamp,int>
        limitVals: System.Collections.Generic.Dictionary<TypedTree.Stamp,Limit>
        mutable potentialUnboundUsesOfVals: TypedTree.StampMap<Range.range>
        mutable anonRecdTypes: TypedTree.StampMap<TypedTree.AnonRecdTypeInfo>
        g: TcGlobals.TcGlobals
        amap: Import.ImportMap
        infoReader: InfoReader.InfoReader
        internalsVisibleToPaths: TypedTree.CompilationPath list
        denv: TypedTreeOps.DisplayEnv
        viewCcu: TypedTree.CcuThunk
        reportErrors: bool
        isLastCompiland: bool * bool
        isInternalTestSpanStackReferring: bool
        mutable usesQuotations: bool
        mutable entryPointGiven: bool
        tcVal: ConstraintSolver.TcValF }
      with
        override ToString: unit -> string
    
    val IsValArgument: env:env -> v:TypedTree.Val -> bool
    val IsValLocal: env:env -> v:TypedTree.Val -> bool
    val GetLimitVal:
      cenv:cenv -> env:env -> m:Range.range -> v:TypedTree.Val -> Limit
    val GetLimitValByRef:
      cenv:cenv -> env:env -> m:Range.range -> v:TypedTree.Val -> Limit
    val LimitVal: cenv:cenv -> v:TypedTree.Val -> limit:Limit -> unit
    val BindVal: cenv:cenv -> env:env -> v:TypedTree.Val -> unit
    val BindVals: cenv:cenv -> env:env -> vs:TypedTree.Val list -> unit
    val RecordAnonRecdInfo:
      cenv:cenv -> anonInfo:TypedTree.AnonRecdTypeInfo -> unit
    val CheckTypeDeep:
      cenv:cenv ->
        (TypedTree.TType -> unit) * (bool -> TypedTree.TyconRef -> unit) option *
        (TypedTree.TyconRef * TypedTree.TypeInst -> unit) option *
        (TypedTree.TraitConstraintSln -> unit) option *
        (env * TypedTree.Typar -> unit) option ->
          g:TcGlobals.TcGlobals ->
            env:env -> isInner:bool -> ty:TypedTree.TType -> unit
    val CheckTypesDeep:
      cenv:cenv ->
        (TypedTree.TType -> unit) * (bool -> TypedTree.TyconRef -> unit) option *
        (TypedTree.TyconRef * TypedTree.TypeInst -> unit) option *
        (TypedTree.TraitConstraintSln -> unit) option *
        (env * TypedTree.Typar -> unit) option ->
          g:TcGlobals.TcGlobals -> env:env -> tys:TypedTree.TypeInst -> unit
    val CheckTypesDeepNoInner:
      cenv:cenv ->
        (TypedTree.TType -> unit) * (bool -> TypedTree.TyconRef -> unit) option *
        (TypedTree.TyconRef * TypedTree.TypeInst -> unit) option *
        (TypedTree.TraitConstraintSln -> unit) option *
        (env * TypedTree.Typar -> unit) option ->
          g:TcGlobals.TcGlobals -> env:env -> tys:TypedTree.TypeInst -> unit
    val CheckTypeConstraintDeep:
      cenv:cenv ->
        (TypedTree.TType -> unit) * (bool -> TypedTree.TyconRef -> unit) option *
        (TypedTree.TyconRef * TypedTree.TypeInst -> unit) option *
        (TypedTree.TraitConstraintSln -> unit) option *
        (env * TypedTree.Typar -> unit) option ->
          g:TcGlobals.TcGlobals ->
            env:env -> x:TypedTree.TyparConstraint -> unit
    val CheckTraitInfoDeep:
      cenv:cenv ->
        (TypedTree.TType -> unit) * (bool -> TypedTree.TyconRef -> unit) option *
        (TypedTree.TyconRef * TypedTree.TypeInst -> unit) option *
        (TypedTree.TraitConstraintSln -> unit) option *
        (env * TypedTree.Typar -> unit) option ->
          g:TcGlobals.TcGlobals ->
            env:env -> TypedTree.TraitConstraintInfo -> unit
    val CheckForByrefLikeType:
      cenv:cenv ->
        env:env ->
          m:Range.range -> ty:TypedTree.TType -> check:(unit -> unit) -> unit
    val CheckForByrefType:
      cenv:cenv -> env:env -> ty:TypedTree.TType -> check:(unit -> unit) -> unit
    val CheckEscapes:
      cenv:cenv ->
        allowProtected:bool ->
          m:Range.range ->
            syntacticArgs:TypedTree.Val list ->
              body:TypedTree.Expr -> TypedTree.FreeVars option
    val AccessInternalsVisibleToAsInternal:
      thisCompPath:TypedTree.CompilationPath ->
        internalsVisibleToPaths:TypedTree.CompilationPath list ->
          access:TypedTree.Accessibility -> TypedTree.Accessibility
    val CheckTypeForAccess:
      cenv:cenv ->
        env:env ->
          objName:(unit -> System.String) ->
            valAcc:TypedTree.Accessibility ->
              m:Range.range -> ty:TypedTree.TType -> unit
    val WarnOnWrongTypeForAccess:
      cenv:cenv ->
        env:env ->
          objName:(unit -> System.String) ->
            valAcc:TypedTree.Accessibility ->
              m:Range.range -> ty:TypedTree.TType -> unit
    [<RequireQualifiedAccessAttribute>]
    type PermitByRefType =
      | None
      | NoInnerByRefLike
      | SpanLike
      | All
    [<RequireQualifiedAccessAttribute>]
    type PermitByRefExpr =
      | YesTupleOfArgs of int
      | Yes
      | YesReturnable
      | YesReturnableNonLocal
      | No
      with
        member Disallow: bool
        member PermitOnlyReturnable: bool
        member PermitOnlyReturnableNonLocal: bool
    
    val inline IsLimitEscapingScope:
      env:env -> context:PermitByRefExpr -> limit:Limit -> bool
    val mkArgsPermit: n:int -> PermitByRefExpr
    val mkArgsForAppliedVal:
      isBaseCall:bool ->
        vref:TypedTree.ValRef -> argsl:'a list -> PermitByRefExpr list
    val mkArgsForAppliedExpr:
      isBaseCall:bool ->
        argsl:'a list -> x:TypedTree.Expr -> PermitByRefExpr list
    val CheckTypeAux:
      permitByRefLike:PermitByRefType ->
        cenv:cenv ->
          env:env ->
            m:Range.range ->
              ty:TypedTree.TType -> onInnerByrefError:(unit -> unit) -> unit
    val CheckType:
      permitByRefLike:PermitByRefType ->
        cenv:cenv -> env:env -> m:Range.range -> ty:TypedTree.TType -> unit
    val CheckTypeNoByrefs:
      cenv:cenv -> env:env -> m:Range.range -> ty:TypedTree.TType -> unit
    val CheckTypePermitSpanLike:
      cenv:cenv -> env:env -> m:Range.range -> ty:TypedTree.TType -> unit
    val CheckTypePermitAllByrefs:
      cenv:cenv -> env:env -> m:Range.range -> ty:TypedTree.TType -> unit
    val CheckTypeNoInnerByrefs:
      cenv:cenv -> env:env -> m:Range.range -> ty:TypedTree.TType -> unit
    val CheckTypeInstNoByrefs:
      cenv:cenv ->
        env:env -> m:Range.range -> tyargs:TypedTree.TType list -> unit
    val CheckTypeInstPermitAllByrefs:
      cenv:cenv ->
        env:env -> m:Range.range -> tyargs:TypedTree.TType list -> unit
    val CheckTypeInstNoInnerByrefs:
      cenv:cenv ->
        env:env -> m:Range.range -> tyargs:TypedTree.TType list -> unit
    val ( |OptionalCoerce| ): _arg1:TypedTree.Expr -> TypedTree.Expr
    val CheckNoReraise:
      cenv:cenv ->
        freesOpt:TypedTree.FreeVars option -> body:TypedTree.Expr -> unit
    val isSpliceOperator: g:TcGlobals.TcGlobals -> v:TypedTree.ValRef -> bool
    type TTypeEquality =
      | ExactlyEqual
      | FeasiblyEqual
      | NotEqual
    val compareTypesWithRegardToTypeVariablesAndMeasures:
      g:TcGlobals.TcGlobals ->
        amap:'a ->
          m:Range.range ->
            typ1:TypedTree.TType -> typ2:TypedTree.TType -> TTypeEquality
    val CheckMultipleInterfaceInstantiations:
      cenv:cenv ->
        typ:TypedTree.TType ->
          interfaces:TypedTree.TType list ->
            isObjectExpression:bool -> m:Range.range -> unit
    val CheckExprNoByrefs: cenv:cenv -> env:env -> expr:TypedTree.Expr -> unit
    val CheckValRef:
      cenv:cenv ->
        env:env ->
          v:TypedTree.ValRef -> m:Range.range -> context:PermitByRefExpr -> unit
    val CheckValUse:
      cenv:cenv ->
        env:env ->
          vref:TypedTree.ValRef * vFlags:TypedTree.ValUseFlag * m:Range.range ->
            context:PermitByRefExpr -> Limit
    val CheckForOverAppliedExceptionRaisingPrimitive:
      cenv:cenv -> expr:TypedTree.Expr -> unit
    val CheckCallLimitArgs:
      cenv:cenv ->
        env:env ->
          m:Range.range ->
            returnTy:TypedTree.TType ->
              limitArgs:Limit -> context:PermitByRefExpr -> Limit
    val CheckCall:
      cenv:cenv ->
        env:env ->
          m:Range.range ->
            returnTy:TypedTree.TType ->
              args:TypedTree.Expr list ->
                contexts:PermitByRefExpr list ->
                  context:PermitByRefExpr -> Limit
    val CheckCallWithReceiver:
      cenv:cenv ->
        env:env ->
          m:Range.range ->
            returnTy:TypedTree.TType ->
              args:TypedTree.Expr list ->
                contexts:PermitByRefExpr list ->
                  context:PermitByRefExpr -> Limit
    val CheckExprLinear:
      cenv:cenv ->
        env:env ->
          expr:TypedTree.Expr ->
            context:PermitByRefExpr -> contf:(Limit -> Limit) -> Limit
    val CheckExpr:
      cenv:cenv ->
        env:env -> origExpr:TypedTree.Expr -> context:PermitByRefExpr -> Limit
    val CheckMethods:
      cenv:cenv ->
        env:env ->
          baseValOpt:TypedTree.Val option ->
            methods:TypedTree.ObjExprMethod list -> unit
    val CheckMethod:
      cenv:cenv ->
        env:env ->
          baseValOpt:TypedTree.Val option -> TypedTree.ObjExprMethod -> unit
    val CheckInterfaceImpls:
      cenv:cenv ->
        env:env ->
          baseValOpt:TypedTree.Val option ->
            l:(TypedTree.TType * TypedTree.ObjExprMethod list) list -> unit
    val CheckInterfaceImpl:
      cenv:cenv ->
        env:env ->
          baseValOpt:TypedTree.Val option ->
            _ty:TypedTree.TType * overrides:TypedTree.ObjExprMethod list -> unit
    val CheckExprOp:
      cenv:cenv ->
        env:env ->
          op:TypedTree.TOp * tyargs:TypedTree.TypeInst * args:TypedTree.Exprs *
          m:Range.range ->
            context:PermitByRefExpr -> expr:TypedTree.Expr -> Limit
    val CheckLambdas:
      isTop:bool ->
        memInfo:TypedTree.ValMemberInfo option ->
          cenv:cenv ->
            env:env ->
              inlined:bool ->
                topValInfo:TypedTree.ValReprInfo ->
                  alwaysCheckNoReraise:bool ->
                    e:TypedTree.Expr ->
                      mOrig:Range.range ->
                        ety:TypedTree.TType -> context:PermitByRefExpr -> Limit
    val CheckExprs:
      cenv:cenv ->
        env:env ->
          exprs:TypedTree.Expr list -> contexts:PermitByRefExpr list -> Limit
    val CheckExprsNoByRefLike:
      cenv:cenv -> env:env -> exprs:TypedTree.Expr list -> Limit
    val CheckExprsPermitByRefLike:
      cenv:cenv -> env:env -> exprs:TypedTree.Expr list -> Limit
    val CheckExprsPermitReturnableByRef:
      cenv:cenv -> env:env -> exprs:TypedTree.Expr list -> Limit
    val CheckExprPermitByRefLike:
      cenv:cenv -> env:env -> expr:TypedTree.Expr -> Limit
    val CheckExprPermitReturnableByRef:
      cenv:cenv -> env:env -> expr:TypedTree.Expr -> Limit
    val CheckDecisionTreeTargets:
      cenv:cenv ->
        env:env ->
          targets:TypedTree.DecisionTreeTarget array ->
            context:PermitByRefExpr -> Limit
    val CheckDecisionTreeTarget:
      cenv:cenv ->
        env:env ->
          context:PermitByRefExpr -> TypedTree.DecisionTreeTarget -> Limit
    val CheckDecisionTree:
      cenv:cenv -> env:env -> x:TypedTree.DecisionTree -> unit
    val CheckDecisionTreeSwitch:
      cenv:cenv ->
        env:env ->
          e:TypedTree.Expr * cases:TypedTree.DecisionTreeCase list *
          dflt:TypedTree.DecisionTree option * m:Range.range -> unit
    val CheckDecisionTreeTest:
      cenv:cenv ->
        env:env -> m:Range.range -> discrim:TypedTree.DecisionTreeTest -> unit
    val CheckAttrib: cenv:cenv -> env:env -> TypedTree.Attrib -> unit
    val CheckAttribExpr: cenv:cenv -> env:env -> TypedTree.AttribExpr -> unit
    val CheckAttribArgExpr: cenv:cenv -> env:env -> expr:TypedTree.Expr -> unit
    val CheckAttribs: cenv:cenv -> env:env -> attribs:TypedTree.Attribs -> unit
    val CheckValInfo: cenv:cenv -> env:env -> TypedTree.ValReprInfo -> unit
    val CheckArgInfo:
      cenv:cenv -> env:env -> argInfo:TypedTree.ArgReprInfo -> unit
    val CheckValSpecAux:
      permitByRefLike:PermitByRefType ->
        cenv:cenv ->
          env:env -> v:TypedTree.Val -> onInnerByrefError:(unit -> unit) -> unit
    val CheckValSpec:
      permitByRefLike:PermitByRefType ->
        cenv:cenv -> env:env -> v:TypedTree.Val -> unit
    val AdjustAccess:
      isHidden:bool ->
        cpath:(unit -> TypedTree.CompilationPath) ->
          access:TypedTree.Accessibility -> TypedTree.Accessibility
    val CheckBinding:
      cenv:cenv ->
        env:env ->
          alwaysCheckNoReraise:bool ->
            context:PermitByRefExpr -> TypedTree.Binding -> Limit
    val CheckBindings: cenv:cenv -> env:env -> xs:TypedTree.Bindings -> unit
    val CheckModuleBinding: cenv:cenv -> env:env -> TypedTree.Binding -> unit
    val CheckModuleBindings:
      cenv:cenv -> env:env -> binds:TypedTree.Binding list -> unit
    val CheckRecdField:
      isUnion:bool ->
        cenv:cenv ->
          env:env -> tycon:TypedTree.Tycon -> rfield:TypedTree.RecdField -> unit
    val CheckEntityDefn: cenv:cenv -> env:env -> tycon:TypedTree.Entity -> unit
    val CheckEntityDefns:
      cenv:cenv -> env:env -> tycons:TypedTree.Entity list -> unit
    val CheckModuleExpr:
      cenv:cenv -> env:env -> x:TypedTree.ModuleOrNamespaceExprWithSig -> unit
    val CheckDefnsInModule:
      cenv:cenv -> env:env -> x:TypedTree.ModuleOrNamespaceExpr list -> unit
    val CheckNothingAfterEntryPoint: cenv:cenv -> m:Range.range -> unit
    val CheckDefnInModule:
      cenv:cenv -> env:env -> x:TypedTree.ModuleOrNamespaceExpr -> unit
    val CheckModuleSpec:
      cenv:cenv -> env:env -> x:TypedTree.ModuleOrNamespaceBinding -> unit
    val CheckTopImpl:
      g:TcGlobals.TcGlobals * amap:Import.ImportMap * reportErrors:bool *
      infoReader:InfoReader.InfoReader *
      internalsVisibleToPaths:TypedTree.CompilationPath list *
      viewCcu:TypedTree.CcuThunk * tcValF:ConstraintSolver.TcValF *
      denv:TypedTreeOps.DisplayEnv *
      mexpr:TypedTree.ModuleOrNamespaceExprWithSig *
      extraAttribs:TypedTree.Attribs * (bool * bool) *
      isInternalTestSpanStackReferring:bool ->
        bool * TypedTree.StampMap<TypedTree.AnonRecdTypeInfo>


namespace FSharp.Compiler
  module internal CheckExpressions =
    val mkNilListPat:
      g:TcGlobals.TcGlobals ->
        m:Range.range -> ty:TypedTree.TType -> PatternMatchCompilation.Pattern
    val mkConsListPat:
      g:TcGlobals.TcGlobals ->
        ty:TypedTree.TType ->
          ph:PatternMatchCompilation.Pattern ->
            pt:PatternMatchCompilation.Pattern ->
              PatternMatchCompilation.Pattern
    exception BakedInMemberConstraintName of string * Range.range
    exception FunctionExpected of
                                 TypedTreeOps.DisplayEnv * TypedTree.TType *
                                 Range.range
    exception NotAFunction of
                             TypedTreeOps.DisplayEnv * TypedTree.TType *
                             Range.range * Range.range
    exception NotAFunctionButIndexer of
                                       TypedTreeOps.DisplayEnv * TypedTree.TType *
                                       string option * Range.range * Range.range
    exception Recursion of
                          TypedTreeOps.DisplayEnv * SyntaxTree.Ident *
                          TypedTree.TType * TypedTree.TType * Range.range
    exception RecursiveUseCheckedAtRuntime of
                                             TypedTreeOps.DisplayEnv *
                                             TypedTree.ValRef * Range.range
    exception LetRecEvaluatedOutOfOrder of
                                          TypedTreeOps.DisplayEnv *
                                          TypedTree.ValRef * TypedTree.ValRef *
                                          Range.range
    exception LetRecCheckedAtRuntime of Range.range
    exception LetRecUnsound of
                              TypedTreeOps.DisplayEnv * TypedTree.ValRef list *
                              Range.range
    exception TyconBadArgs of
                             TypedTreeOps.DisplayEnv * TypedTree.TyconRef * int *
                             Range.range
    exception UnionCaseWrongArguments of
                                        TypedTreeOps.DisplayEnv * int * int *
                                        Range.range
    exception UnionCaseWrongNumberOfArgs of
                                           TypedTreeOps.DisplayEnv * int * int *
                                           Range.range
    exception FieldsFromDifferentTypes of
                                         TypedTreeOps.DisplayEnv *
                                         TypedTree.RecdFieldRef *
                                         TypedTree.RecdFieldRef * Range.range
    exception FieldGivenTwice of
                                TypedTreeOps.DisplayEnv * TypedTree.RecdFieldRef *
                                Range.range
    exception MissingFields of string list * Range.range
    exception FunctionValueUnexpected of
                                        TypedTreeOps.DisplayEnv *
                                        TypedTree.TType * Range.range
    exception UnitTypeExpected of
                                 TypedTreeOps.DisplayEnv * TypedTree.TType *
                                 Range.range
    exception UnitTypeExpectedWithEquality of
                                             TypedTreeOps.DisplayEnv *
                                             TypedTree.TType * Range.range
    exception UnitTypeExpectedWithPossibleAssignment of
                                                       TypedTreeOps.DisplayEnv *
                                                       TypedTree.TType * bool *
                                                       string * Range.range
    exception UnitTypeExpectedWithPossiblePropertySetter of
                                                           TypedTreeOps.DisplayEnv *
                                                           TypedTree.TType *
                                                           string * string *
                                                           Range.range
    exception UnionPatternsBindDifferentNames of Range.range
    exception VarBoundTwice of SyntaxTree.Ident
    exception ValueRestriction of
                                 TypedTreeOps.DisplayEnv * bool * TypedTree.Val *
                                 TypedTree.Typar * Range.range
    exception ValNotMutable of
                              TypedTreeOps.DisplayEnv * TypedTree.ValRef *
                              Range.range
    exception ValNotLocal of
                            TypedTreeOps.DisplayEnv * TypedTree.ValRef *
                            Range.range
    exception InvalidRuntimeCoercion of
                                       TypedTreeOps.DisplayEnv * TypedTree.TType *
                                       TypedTree.TType * Range.range
    exception IndeterminateRuntimeCoercion of
                                             TypedTreeOps.DisplayEnv *
                                             TypedTree.TType * TypedTree.TType *
                                             Range.range
    exception IndeterminateStaticCoercion of
                                            TypedTreeOps.DisplayEnv *
                                            TypedTree.TType * TypedTree.TType *
                                            Range.range
    exception RuntimeCoercionSourceSealed of
                                            TypedTreeOps.DisplayEnv *
                                            TypedTree.TType * Range.range
    exception CoercionTargetSealed of
                                     TypedTreeOps.DisplayEnv * TypedTree.TType *
                                     Range.range
    exception UpcastUnnecessary of Range.range
    exception TypeTestUnnecessary of Range.range
    exception StaticCoercionShouldUseBox of
                                           TypedTreeOps.DisplayEnv *
                                           TypedTree.TType * TypedTree.TType *
                                           Range.range
    exception SelfRefObjCtor of bool * Range.range
    exception VirtualAugmentationOnNullValuedType of Range.range
    exception NonVirtualAugmentationOnNullValuedType of Range.range
    exception UseOfAddressOfOperator of Range.range
    exception DeprecatedThreadStaticBindingWarning of Range.range
    exception IntfImplInIntrinsicAugmentation of Range.range
    exception IntfImplInExtrinsicAugmentation of Range.range
    exception OverrideInIntrinsicAugmentation of Range.range
    exception OverrideInExtrinsicAugmentation of Range.range
    exception NonUniqueInferredAbstractSlot of
                                              TcGlobals.TcGlobals *
                                              TypedTreeOps.DisplayEnv * string *
                                              Infos.MethInfo * Infos.MethInfo *
                                              Range.range
    exception StandardOperatorRedefinitionWarning of string * Range.range
    exception InvalidInternalsVisibleToAssemblyName of string * string option
    type SafeInitData =
      | SafeInitField of TypedTree.RecdFieldRef * TypedTree.RecdField
      | NoSafeInitInfo
    [<SealedAttribute>]
    type CtorInfo =
      { ctorShapeCounter: int
        safeThisValOpt: TypedTree.Val option
        safeInitInfo: SafeInitData
        ctorIsImplicit: bool }
    [<NoEquality; NoComparison (); SealedAttribute>]
    type UngeneralizableItem =
  
        new: computeFreeTyvars:(unit -> TypedTree.FreeTyvars) ->
                UngeneralizableItem
        member GetFreeTyvars: unit -> TypedTree.FreeTyvars
        member CachedFreeLocalTycons: TypedTree.FreeTycons
        member CachedFreeTraitSolutions: TypedTree.FreeLocals
        member WillNeverHaveFreeTypars: bool
    
    [<NoEquality; NoComparison>]
    type TcEnv =
      { eNameResEnv: NameResolution.NameResolutionEnv
        eUngeneralizableItems: UngeneralizableItem list
        ePath: SyntaxTree.Ident list
        eCompPath: TypedTree.CompilationPath
        eAccessPath: TypedTree.CompilationPath
        eAccessRights: AccessibilityLogic.AccessorDomain
        eInternalsVisibleCompPaths: TypedTree.CompilationPath list
        eModuleOrNamespaceTypeAccumulator: TypedTree.ModuleOrNamespaceType ref
        eContextInfo: ConstraintSolver.ContextInfo
        eFamilyType: TypedTree.TyconRef option
        eCtorInfo: CtorInfo option
        eCallerMemberName: string option }
      with
        override ToString: unit -> string
        member AccessRights: AccessibilityLogic.AccessorDomain
        member DisplayEnv: TypedTreeOps.DisplayEnv
        member NameEnv: NameResolution.NameResolutionEnv
    
    val ComputeAccessRights:
      eAccessPath:TypedTree.CompilationPath ->
        eInternalsVisibleCompPaths:TypedTree.CompilationPath list ->
          eFamilyType:TypedTree.TyconRef option ->
            AccessibilityLogic.AccessorDomain
    val InitialExplicitCtorInfo:
      safeThisValOpt:TypedTree.Val option * safeInitInfo:SafeInitData ->
        CtorInfo
    val InitialImplicitCtorInfo: unit -> CtorInfo
    val EnterFamilyRegion: tcref:TypedTree.TyconRef -> env:TcEnv -> TcEnv
    val ExitFamilyRegion: env:TcEnv -> TcEnv
    val AreWithinCtorShape: env:TcEnv -> bool
    val AreWithinImplicitCtor: env:TcEnv -> bool
    val GetCtorShapeCounter: env:TcEnv -> int
    val GetRecdInfo: env:TcEnv -> TypedTree.RecordConstructionInfo
    val AdjustCtorShapeCounter: f:(int -> int) -> env:TcEnv -> TcEnv
    val ExitCtorShapeRegion: env:TcEnv -> TcEnv
    val addFreeItemOfTy:
      ty:TypedTree.TType ->
        eUngeneralizableItems:UngeneralizableItem list ->
          UngeneralizableItem list
    val addFreeItemOfModuleTy:
      TypedTree.ModuleOrNamespaceType ->
        UngeneralizableItem list -> UngeneralizableItem list
    val AddValMapToNameEnv:
      vs:AbstractIL.Internal.Library.NameMap<TypedTree.Val> ->
        nenv:NameResolution.NameResolutionEnv ->
          NameResolution.NameResolutionEnv
    val AddValListToNameEnv:
      vs:TypedTree.Val list ->
        nenv:NameResolution.NameResolutionEnv ->
          NameResolution.NameResolutionEnv
    val AddLocalValPrimitive: v:TypedTree.Val -> TcEnv -> TcEnv
    val AddLocalValMap:
      tcSink:NameResolution.TcResultsSink ->
        scopem:Range.range ->
          vals:AbstractIL.Internal.Library.NameMap<TypedTree.Val> ->
            env:TcEnv -> TcEnv
    val AddLocalVals:
      tcSink:NameResolution.TcResultsSink ->
        scopem:Range.range -> vals:TypedTree.Val list -> env:TcEnv -> TcEnv
    val AddLocalVal:
      NameResolution.TcResultsSink ->
        scopem:Range.range -> v:TypedTree.Val -> TcEnv -> TcEnv
    val AddDeclaredTypars:
      check:NameResolution.CheckForDuplicateTyparFlag ->
        typars:TypedTree.Typar list -> env:TcEnv -> TcEnv
    type UnscopedTyparEnv =
      | UnscopedTyparEnv of AbstractIL.Internal.Library.NameMap<TypedTree.Typar>
    val emptyUnscopedTyparEnv: UnscopedTyparEnv
    val AddUnscopedTypar:
      n:string -> p:TypedTree.Typar -> UnscopedTyparEnv -> UnscopedTyparEnv
    val TryFindUnscopedTypar:
      n:string -> UnscopedTyparEnv -> TypedTree.Typar option
    val HideUnscopedTypars:
      typars:TypedTree.Typar list -> UnscopedTyparEnv -> UnscopedTyparEnv
    [<NoEquality; NoComparison>]
    type TcFileState =
      { g: TcGlobals.TcGlobals
        mutable recUses:
          TypedTreeOps.ValMultiMap<TypedTree.Expr ref * Range.range * bool>
        mutable postInferenceChecks: ResizeArray<(unit -> unit)>
        mutable createsGeneratedProvidedTypes: bool
        isScript: bool
        amap: Import.ImportMap
        synArgNameGenerator: SyntaxTreeOps.SynArgNameGenerator
        tcSink: NameResolution.TcResultsSink
        topCcu: TypedTree.CcuThunk
        css: ConstraintSolver.ConstraintSolverState
        compilingCanonicalFslibModuleType: bool
        isSig: bool
        haveSig: bool
        niceNameGen: CompilerGlobalState.NiceNameGenerator
        infoReader: InfoReader.InfoReader
        nameResolver: NameResolution.NameResolver
        conditionalDefines: string list option
        isInternalTestSpanStackReferring: bool
        TcSequenceExpressionEntry:
          TcFileState -> TcEnv -> TypedTree.TType -> UnscopedTyparEnv ->
            bool * bool ref * SyntaxTree.SynExpr -> Range.range ->
            TypedTree.Expr * UnscopedTyparEnv
        TcArrayOrListSequenceExpression:
          TcFileState -> TcEnv -> TypedTree.TType -> UnscopedTyparEnv ->
            bool * SyntaxTree.SynExpr -> Range.range ->
            TypedTree.Expr * UnscopedTyparEnv
        TcComputationExpression:
          TcFileState -> TcEnv -> TypedTree.TType -> UnscopedTyparEnv ->
            Range.range * TypedTree.Expr * TypedTree.TType * SyntaxTree.SynExpr ->
            TypedTree.Expr * UnscopedTyparEnv }
      with
        static member
          Create: g:TcGlobals.TcGlobals * isScript:bool *
                   niceNameGen:CompilerGlobalState.NiceNameGenerator *
                   amap:Import.ImportMap * topCcu:TypedTree.CcuThunk *
                   isSig:bool * haveSig:bool *
                   conditionalDefines:string list option *
                   tcSink:NameResolution.TcResultsSink *
                   tcVal:ConstraintSolver.TcValF *
                   isInternalTestSpanStackReferring:bool *
                   tcSequenceExpressionEntry:(TcFileState -> TcEnv ->
                                                TypedTree.TType ->
                                                UnscopedTyparEnv ->
                                                bool * bool ref *
                                                SyntaxTree.SynExpr ->
                                                Range.range ->
                                                TypedTree.Expr *
                                                UnscopedTyparEnv) *
                   tcArrayOrListSequenceExpression:(TcFileState -> TcEnv ->
                                                      TypedTree.TType ->
                                                      UnscopedTyparEnv ->
                                                      bool * SyntaxTree.SynExpr ->
                                                      Range.range ->
                                                      TypedTree.Expr *
                                                      UnscopedTyparEnv) *
                   tcComputationExpression:(TcFileState -> TcEnv ->
                                              TypedTree.TType ->
                                              UnscopedTyparEnv ->
                                              Range.range * TypedTree.Expr *
                                              TypedTree.TType *
                                              SyntaxTree.SynExpr ->
                                              TypedTree.Expr * UnscopedTyparEnv) ->
                     TcFileState
        override ToString: unit -> string
    
    type cenv = TcFileState
    val CopyAndFixupTypars:
      m:Range.range ->
        rigid:TypedTree.TyparRigidity ->
          tpsorig:TypedTree.Typars ->
            TypedTree.Typars * TypedTreeOps.TyparInst * TypedTree.TType list
    val UnifyTypes:
      cenv:TcFileState ->
        env:TcEnv ->
          m:Range.range ->
            actualTy:TypedTree.TType -> expectedTy:TypedTree.TType -> unit
    val MakeInnerEnvWithAcc:
      addOpenToNameEnv:bool ->
        env:TcEnv ->
          nm:SyntaxTree.Ident ->
            mtypeAcc:TypedTree.ModuleOrNamespaceType ref ->
              modKind:TypedTree.ModuleOrNamespaceKind -> TcEnv
    val MakeInnerEnv:
      addOpenToNameEnv:bool ->
        env:TcEnv ->
          nm:SyntaxTree.Ident ->
            modKind:TypedTree.ModuleOrNamespaceKind ->
              TcEnv * TypedTree.ModuleOrNamespaceType ref
    val MakeInnerEnvForTyconRef:
      env:TcEnv ->
        tcref:TypedTree.TyconRef -> isExtrinsicExtension:bool -> TcEnv
    val MakeInnerEnvForMember: env:TcEnv -> v:TypedTree.Val -> TcEnv
    val GetCurrAccumulatedModuleOrNamespaceType:
      env:TcEnv -> TypedTree.ModuleOrNamespaceType
    val SetCurrAccumulatedModuleOrNamespaceType:
      env:TcEnv -> x:TypedTree.ModuleOrNamespaceType -> unit
    val LocateEnv:
      ccu:TypedTree.CcuThunk ->
        env:TcEnv -> enclosingNamespacePath:SyntaxTree.Ident list -> TcEnv
    val ShrinkContext:
      env:TcEnv -> oldRange:Range.range -> newRange:Range.range -> TcEnv
    val UnifyRefTupleType:
      contextInfo:ConstraintSolver.ContextInfo ->
        cenv:TcFileState ->
          denv:TypedTreeOps.DisplayEnv ->
            m:Range.range ->
              ty:TypedTree.TType -> ps:'a list -> TypedTree.TTypes
    val UnifyTupleTypeAndInferCharacteristics:
      contextInfo:ConstraintSolver.ContextInfo ->
        cenv:TcFileState ->
          denv:TypedTreeOps.DisplayEnv ->
            m:Range.range ->
              knownTy:TypedTree.TType ->
                isExplicitStruct:bool ->
                  ps:'a list -> TypedTree.TupInfo * TypedTree.TTypes
    val UnifyAnonRecdTypeAndInferCharacteristics:
      contextInfo:ConstraintSolver.ContextInfo ->
        cenv:TcFileState ->
          denv:TypedTreeOps.DisplayEnv ->
            m:Range.range ->
              ty:TypedTree.TType ->
                isExplicitStruct:bool ->
                  unsortedNames:SyntaxTree.Ident [] ->
                    TypedTree.AnonRecdTypeInfo * TypedTree.TType list
    val UnifyFunctionTypeUndoIfFailed:
      cenv:TcFileState ->
        denv:TypedTreeOps.DisplayEnv ->
          m:Range.range ->
            ty:TypedTree.TType -> (TypedTree.TType * TypedTree.TType) voption
    val UnifyFunctionType:
      extraInfo:Range.range option ->
        cenv:TcFileState ->
          denv:TypedTreeOps.DisplayEnv ->
            mFunExpr:Range.range ->
              ty:TypedTree.TType -> TypedTree.TType * TypedTree.TType
    val ReportImplicitlyIgnoredBoolExpression:
      denv:TypedTreeOps.DisplayEnv ->
        m:Range.range -> ty:TypedTree.TType -> expr:TypedTree.Expr -> exn
    val UnifyUnitType:
      cenv:TcFileState ->
        env:TcEnv ->
          m:Range.range -> ty:TypedTree.TType -> expr:TypedTree.Expr -> bool
    val TryUnifyUnitTypeWithoutWarning:
      cenv:TcFileState ->
        env:TcEnv -> m:Range.range -> ty:TypedTree.TType -> bool
    module AttributeTargets =
      val FieldDecl: System.AttributeTargets
      val FieldDeclRestricted: System.AttributeTargets
      val UnionCaseDecl: System.AttributeTargets
      val TyconDecl: System.AttributeTargets
      val ExnDecl: System.AttributeTargets
      val ModuleDecl: System.AttributeTargets
      val Top: System.AttributeTargets
  
    val ForNewConstructors:
      tcSink:NameResolution.TcResultsSink ->
        env:TcEnv ->
          mObjTy:Range.range ->
            methodName:string ->
              meths:Infos.MethInfo list -> NameResolution.AfterResolution
    val TcSynRationalConst: c:SyntaxTree.SynRationalConst -> Rational.Rational
    val TcConst:
      cenv:TcFileState ->
        ty:TypedTree.TType ->
          m:Range.range -> env:TcEnv -> c:SyntaxTree.SynConst -> TypedTree.Const
    val TcFieldInit:
      Range.range -> AbstractIL.IL.ILFieldInit -> TypedTree.Const
    val AdjustValSynInfoInSignature:
      g:TcGlobals.TcGlobals ->
        ty:TypedTree.TType -> SyntaxTree.SynValInfo -> SyntaxTree.SynValInfo
    type PartialValReprInfo =
      | PartialValReprInfo of
        curriedArgInfos: TypedTree.ArgReprInfo list list *
        returnInfo: TypedTree.ArgReprInfo
    val TranslateTopArgSynInfo:
      isArg:bool ->
        m:Range.range ->
          tcAttributes:(SyntaxTree.SynAttribute list -> TypedTree.Attribs) ->
            SyntaxTree.SynArgInfo -> TypedTree.ArgReprInfo
    val TranslateTopValSynInfo:
      Range.range ->
        tcAttributes:(System.AttributeTargets -> SyntaxTree.SynAttribute list ->
                        TypedTree.Attribs) ->
          synValInfo:SyntaxTree.SynValInfo -> PartialValReprInfo
    val TranslatePartialArity:
      tps:TypedTree.Typar list -> PartialValReprInfo -> TypedTree.ValReprInfo
    val ComputeLogicalName:
      id:SyntaxTree.Ident -> memberFlags:SyntaxTree.MemberFlags -> string
    type PreValMemberInfo =
      | PreValMemberInfo of
        memberInfo: TypedTree.ValMemberInfo * logicalName: string *
        compiledName: string
    val MakeMemberDataAndMangledNameForMemberVal:
      g:TcGlobals.TcGlobals * tcref:TypedTree.TyconRef * isExtrinsic:bool *
      attrs:TypedTree.Attribs * optImplSlotTys:TypedTree.TType list *
      memberFlags:SyntaxTree.MemberFlags * valSynData:SyntaxTree.SynValInfo *
      id:SyntaxTree.Ident * isCompGen:bool -> PreValMemberInfo
    type OverridesOK =
      | OverridesOK
      | WarnOnOverrides
      | ErrorOnOverrides
    type ExplicitTyparInfo =
      | ExplicitTyparInfo of
        rigidCopyOfDeclaredTypars: TypedTree.Typars *
        declaredTypars: TypedTree.Typars * infer: bool
    val permitInferTypars: ExplicitTyparInfo
    val dontInferTypars: ExplicitTyparInfo
    type ArgAndRetAttribs =
      | ArgAndRetAttribs of TypedTree.Attribs list list * TypedTree.Attribs
    val noArgOrRetAttribs: ArgAndRetAttribs
    type DeclKind =
      | ModuleOrMemberBinding
      | IntrinsicExtensionBinding
      | ExtrinsicExtensionBinding
      | ClassLetBinding of isStatic: bool
      | ObjectExpressionOverrideBinding
      | ExpressionBinding
      with
        static member
          AllowedAttribTargets: SyntaxTree.MemberFlags option ->
                                   DeclKind -> System.AttributeTargets
        static member CanGeneralizeConstrainedTypars: DeclKind -> bool
        static member CanOverrideOrImplement: DeclKind -> OverridesOK
        static member ConvertToLinearBindings: DeclKind -> bool
        static member ImplicitlyStatic: DeclKind -> bool
        static member IsAccessModifierPermitted: DeclKind -> bool
        static member IsModuleOrMemberOrExtensionBinding: DeclKind -> bool
        static member MustHaveArity: DeclKind -> bool
        member CanBeDllImport: bool
    
    [<SealedAttribute>]
    type PrelimValScheme1 =
      | PrelimValScheme1 of
        id: SyntaxTree.Ident * explicitTyparInfo: ExplicitTyparInfo *
        TypedTree.TType * PartialValReprInfo option * PreValMemberInfo option *
        bool * TypedTree.ValInline * TypedTree.ValBaseOrThisInfo *
        ArgAndRetAttribs * SyntaxTree.SynAccess option * bool
      with
        member Ident: SyntaxTree.Ident
        member Type: TypedTree.TType
    
    type PrelimValScheme2 =
      | PrelimValScheme2 of
        SyntaxTree.Ident * TypedTreeOps.TypeScheme * PartialValReprInfo option *
        PreValMemberInfo option * bool * TypedTree.ValInline *
        TypedTree.ValBaseOrThisInfo * ArgAndRetAttribs *
        SyntaxTree.SynAccess option * bool * bool
    type ValScheme =
      | ValScheme of
        id: SyntaxTree.Ident * typeScheme: TypedTreeOps.TypeScheme *
        topValInfo: TypedTree.ValReprInfo option *
        memberInfo: PreValMemberInfo option * isMutable: bool *
        inlineInfo: TypedTree.ValInline *
        baseOrThisInfo: TypedTree.ValBaseOrThisInfo *
        visibility: SyntaxTree.SynAccess option * compgen: bool *
        isIncrClass: bool * isTyFunc: bool * hasDeclaredTypars: bool
      with
        member GeneralizedTypars: TypedTree.Typars
        member TypeScheme: TypedTreeOps.TypeScheme
        member ValReprInfo: TypedTree.ValReprInfo option
    
    type TcPatPhase2Input =
      | TcPatPhase2Input of
        AbstractIL.Internal.Library.NameMap<TypedTree.Val *
                                            TypedTreeOps.TypeScheme> * bool
      with
        member RightPath: TcPatPhase2Input
    
    [<SealedAttribute>]
    type CheckedBindingInfo =
      | CheckedBindingInfo of
        inlineFlag: TypedTree.ValInline * valAttribs: TypedTree.Attribs *
        xmlDoc: XmlDoc.XmlDoc *
        tcPatPhase2: TcPatPhase2Input -> PatternMatchCompilation.Pattern *
        exlicitTyparInfo: ExplicitTyparInfo *
        nameToPrelimValSchemeMap:
          AbstractIL.Internal.Library.NameMap<PrelimValScheme1> *
        rhsExprChecked: TypedTree.Expr * argAndRetAttribs: ArgAndRetAttribs *
        overallPatTy: TypedTree.TType * mBinding: Range.range *
        spBind: SyntaxTree.DebugPointForBinding * isCompilerGenerated: bool *
        literalValue: TypedTree.Const option * isFixed: bool
      with
        member Expr: TypedTree.Expr
        member SeqPoint: SyntaxTree.DebugPointForBinding
    
    val GeneralizedTypeForTypeScheme:
      typeScheme:TypedTreeOps.TypeScheme -> TypedTree.TType
    val NonGenericTypeScheme: ty:TypedTree.TType -> TypedTreeOps.TypeScheme
    val UpdateAccModuleOrNamespaceType:
      cenv:TcFileState ->
        env:TcEnv ->
          f:(bool -> TypedTree.ModuleOrNamespaceType ->
               TypedTree.ModuleOrNamespaceType) -> unit
    val PublishModuleDefn:
      cenv:TcFileState -> env:TcEnv -> mspec:TypedTree.Tycon -> unit
    val PublishTypeDefn:
      cenv:TcFileState -> env:TcEnv -> mspec:TypedTree.Tycon -> unit
    val PublishValueDefnPrim:
      cenv:TcFileState -> env:TcEnv -> vspec:TypedTree.Val -> unit
    val PublishValueDefn:
      cenv:TcFileState ->
        env:TcEnv -> declKind:DeclKind -> vspec:TypedTree.Val -> unit
    val CombineVisibilityAttribs:
      vis1:'a option -> vis2:'a option -> m:Range.range -> 'a option
    val ComputeAccessAndCompPath:
      env:TcEnv ->
        declKindOpt:DeclKind option ->
          m:Range.range ->
            vis:SyntaxTree.SynAccess option ->
              overrideVis:TypedTree.Accessibility option ->
                actualParent:TypedTree.ParentRef ->
                  TypedTree.Accessibility * TypedTree.CompilationPath option
    val CheckForAbnormalOperatorNames:
      cenv:TcFileState ->
        idRange:Range.range ->
          coreDisplayName:string ->
            memberInfoOpt:TypedTree.ValMemberInfo option -> unit
    val MakeAndPublishVal:
      cenv:TcFileState ->
        env:TcEnv ->
          altActualParent:TypedTree.ParentRef * inSig:bool * declKind:DeclKind *
          vrec:TypedTree.ValRecursiveScopeInfo * vscheme:ValScheme *
          attrs:TypedTree.Attribs * doc:XmlDoc.XmlDoc *
          konst:TypedTree.Const option * isGeneratedEventVal:bool ->
            TypedTree.Val
    val MakeAndPublishVals:
      cenv:TcFileState ->
        env:TcEnv ->
          altActualParent:TypedTree.ParentRef * inSig:bool * declKind:DeclKind *
          vrec:TypedTree.ValRecursiveScopeInfo * valSchemes:Map<'a,ValScheme> *
          attrs:TypedTree.Attribs * doc:XmlDoc.XmlDoc *
          literalValue:TypedTree.Const option ->
            Map<'a,(TypedTree.Val * TypedTreeOps.TypeScheme)>
        when 'a: comparison
    val MakeAndPublishBaseVal:
      cenv:TcFileState ->
        env:TcEnv ->
          SyntaxTree.Ident option -> TypedTree.TType -> TypedTree.Val option
    val MakeAndPublishSafeThisVal:
      cenv:TcFileState ->
        env:TcEnv ->
          thisIdOpt:SyntaxTree.Ident option ->
            thisTy:TypedTree.TType -> TypedTree.Val option
    val AdjustAndForgetUsesOfRecValue:
      cenv:TcFileState ->
        vrefTgt:TypedTree.ValRef -> valScheme:ValScheme -> unit
    val AdjustRecType: vspec:TypedTree.Val -> vscheme:ValScheme -> unit
    val RecordUseOfRecValue:
      cenv:TcFileState ->
        vrec:TypedTree.ValRecursiveScopeInfo ->
          vrefTgt:TypedTree.ValRef ->
            vexp:TypedTree.Expr -> m:Range.range -> TypedTree.Expr
    [<SealedAttribute>]
    type RecursiveUseFixupPoints =
      | RecursiveUseFixupPoints of (TypedTree.Expr ref * Range.range) list
    val GetAllUsesOfRecValue:
      cenv:TcFileState -> vrefTgt:TypedTree.Val -> RecursiveUseFixupPoints
    val ChooseCanonicalDeclaredTyparsAfterInference:
      g:TcGlobals.TcGlobals ->
        denv:TypedTreeOps.DisplayEnv ->
          declaredTypars:TypedTree.Typar list ->
            m:Range.range -> TypedTree.Typars
    val ChooseCanonicalValSchemeAfterInference:
      g:TcGlobals.TcGlobals ->
        denv:TypedTreeOps.DisplayEnv ->
          vscheme:ValScheme -> m:Range.range -> ValScheme
    val PlaceTyparsInDeclarationOrder:
      declaredTypars:TypedTree.Typar list ->
        generalizedTypars:TypedTree.Typar list -> TypedTree.Typar list
    val SetTyparRigid:
      TypedTreeOps.DisplayEnv -> Range.range -> TypedTree.Typar -> unit
    val GeneralizeVal:
      cenv:TcFileState ->
        denv:TypedTreeOps.DisplayEnv ->
          enclosingDeclaredTypars:TypedTree.Typar list ->
            generalizedTyparsForThisBinding:TypedTree.Typar list ->
              PrelimValScheme1 -> PrelimValScheme2
    val GeneralizeVals:
      cenv:TcFileState ->
        denv:TypedTreeOps.DisplayEnv ->
          enclosingDeclaredTypars:TypedTree.Typar list ->
            generalizedTypars:TypedTree.Typar list ->
              types:AbstractIL.Internal.Library.NameMap<PrelimValScheme1> ->
                Map<string,PrelimValScheme2>
    val DontGeneralizeVals:
      types:AbstractIL.Internal.Library.NameMap<PrelimValScheme1> ->
        Map<string,PrelimValScheme2>
    val InferGenericArityFromTyScheme:
      TypedTreeOps.TypeScheme ->
        partialValReprInfo:PartialValReprInfo -> TypedTree.ValReprInfo
    val ComputeIsTyFunc:
      id:SyntaxTree.Ident * hasDeclaredTypars:bool *
      arityInfo:TypedTree.ValReprInfo option -> bool
    val UseSyntacticArity:
      declKind:DeclKind ->
        typeScheme:TypedTreeOps.TypeScheme ->
          partialValReprInfo:PartialValReprInfo -> TypedTree.ValReprInfo option
    val CombineSyntacticAndInferredArities:
      g:TcGlobals.TcGlobals ->
        declKind:DeclKind ->
          rhsExpr:TypedTree.Expr ->
            prelimScheme:PrelimValScheme2 -> PartialValReprInfo option
    val BuildValScheme:
      declKind:DeclKind ->
        partialArityInfoOpt:PartialValReprInfo option ->
          prelimScheme:PrelimValScheme2 -> ValScheme
    val UseCombinedArity:
      g:TcGlobals.TcGlobals ->
        declKind:DeclKind ->
          rhsExpr:TypedTree.Expr -> prelimScheme:PrelimValScheme2 -> ValScheme
    val UseNoArity: prelimScheme:PrelimValScheme2 -> ValScheme
    val MakeAndPublishSimpleVals:
      cenv:TcFileState ->
        env:TcEnv ->
          names:AbstractIL.Internal.Library.NameMap<PrelimValScheme1> ->
            Map<string,(TypedTree.Val * TypedTreeOps.TypeScheme)> *
            Map<string,TypedTree.Val>
    val MakeAndPublishSimpleValsForMergedScope:
      cenv:TcFileState ->
        env:TcEnv ->
          m:Range.range ->
            names:AbstractIL.Internal.Library.NameMap<PrelimValScheme1> ->
              TcEnv * Map<string,(TypedTree.Val * TypedTreeOps.TypeScheme)> *
              Map<string,TypedTree.Val>
    val FreshenTyconRef:
      m:Range.range ->
        rigid:TypedTree.TyparRigidity ->
          tcref:TypedTree.TyconRef ->
            declaredTyconTypars:TypedTree.Typar list ->
              TypedTree.TType * TypedTree.Typar list * TypedTreeOps.TyparInst *
              TypedTree.TType
    val FreshenPossibleForallTy:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          rigid:TypedTree.TyparRigidity ->
            ty:TypedTree.TType ->
              TypedTree.Typar list * TypedTree.Typar list * TypedTree.TType list *
              TypedTree.TType
    val FreshenTyconRef2:
      m:Range.range ->
        tcref:TypedTree.TyconRef ->
          TypedTree.Typars * TypedTreeOps.TyparInst * TypedTree.TType list *
          TypedTree.TType
    val FreshenAbstractSlot:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          m:Range.range ->
            synTyparDecls:SyntaxTree.SynValTyparDecls ->
              absMethInfo:Infos.MethInfo ->
                bool * TypedTree.Typars * TypedTree.TType list list *
                TypedTree.TType
    val BuildFieldMap:
      cenv:TcFileState ->
        env:TcEnv ->
          isPartial:bool ->
            ty:TypedTree.TType ->
              flds:((SyntaxTree.Ident list * SyntaxTree.Ident) * 'a) list ->
                m:Range.range ->
                  TypedTree.TypeInst * TypedTree.TyconRef * Map<string,'a> *
                  (string * 'a) list
    val ApplyUnionCaseOrExn:
      makerForUnionCase:(TypedTree.UnionCaseRef * TypedTree.TypeInst -> 'a) *
      makerForExnTag:(TypedTree.TyconRef -> 'a) ->
        m:Range.range ->
          cenv:TcFileState ->
            env:TcEnv ->
              overallTy:TypedTree.TType ->
                item:NameResolution.Item ->
                  'a * TypedTree.TType list * SyntaxTree.Ident list
    val ApplyUnionCaseOrExnTypes:
      m:Range.range ->
        cenv:TcFileState ->
          env:TcEnv ->
            overallTy:TypedTree.TType ->
              c:NameResolution.Item ->
                (Range.range -> TypedTree.Exprs -> TypedTree.Expr) *
                TypedTree.TType list * SyntaxTree.Ident list
    val ApplyUnionCaseOrExnTypesForPat:
      m:Range.range ->
        cenv:TcFileState ->
          env:TcEnv ->
            overallTy:TypedTree.TType ->
              c:NameResolution.Item ->
                (Range.range -> PatternMatchCompilation.Pattern list ->
                   PatternMatchCompilation.Pattern) * TypedTree.TType list *
                SyntaxTree.Ident list
    val UnionCaseOrExnCheck:
      env:TcEnv -> numArgTys:int -> numArgs:int -> m:Range.range -> unit
    val TcUnionCaseOrExnField:
      cenv:TcFileState ->
        env:TcEnv ->
          ty1:TypedTree.TType ->
            m:Range.range ->
              c:SyntaxTree.Ident list ->
                n:int ->
                  (TypedTree.UnionCaseRef * TypedTree.TypeInst -> 'a) *
                  (TypedTree.TyconRef -> 'a) -> 'a * TypedTree.TType
    type GeneralizeConstrainedTyparOptions =
      | CanGeneralizeConstrainedTypars
      | DoNotGeneralizeConstrainedTypars
    module GeneralizationHelpers =
      val ComputeUngeneralizableTypars:
        env:TcEnv ->
          Internal.Utilities.Collections.Tagged.Set<TypedTree.Typar,
                                                    System.Collections.Generic.IComparer<TypedTree.Typar>>
      val ComputeUnabstractableTycons: env:TcEnv -> TypedTree.FreeTycons
      val ComputeUnabstractableTraitSolutions:
        env:TcEnv -> TypedTree.FreeLocals
      val IsGeneralizableValue:
        g:TcGlobals.TcGlobals -> t:TypedTree.Expr -> bool
      val CanGeneralizeConstrainedTyparsForDecl:
        declKind:DeclKind -> GeneralizeConstrainedTyparOptions
      val TrimUngeneralizableTypars:
        genConstrainedTyparFlag:GeneralizeConstrainedTyparOptions ->
          inlineFlag:TypedTree.ValInline ->
            generalizedTypars:TypedTree.Typar list ->
              freeInEnv:AbstractIL.Internal.Zset<TypedTree.Typar> ->
                TypedTree.Typar list * AbstractIL.Internal.Zset<TypedTree.Typar>
      val CondenseTypars:
        cenv:TcFileState * denv:TypedTreeOps.DisplayEnv *
        generalizedTypars:TypedTree.Typars * tauTy:TypedTree.TType *
        m:Range.range -> TypedTree.Typar list
      val ComputeAndGeneralizeGenericTypars:
        cenv:TcFileState * denv:TypedTreeOps.DisplayEnv * m:Range.range *
        freeInEnv:TypedTree.FreeTypars * canInferTypars:bool *
        genConstrainedTyparFlag:GeneralizeConstrainedTyparOptions *
        inlineFlag:TypedTree.ValInline * exprOpt:TypedTree.Expr option *
        allDeclaredTypars:TypedTree.Typars * maxInferredTypars:TypedTree.Typars *
        tauTy:TypedTree.TType * resultFirst:bool -> TypedTree.Typars
      val CheckDeclaredTyparsPermitted:
        memFlagsOpt:SyntaxTree.MemberFlags option * declaredTypars:'a list *
        m:Range.range -> unit
      val ComputeCanInferExtraGeneralizableTypars:
        parentRef:TypedTree.ParentRef * canInferTypars:bool *
        memFlagsOpt:SyntaxTree.MemberFlags option -> bool
  
    val ComputeInlineFlag:
      memFlagsOption:SyntaxTree.MemberFlags option ->
        isInline:bool -> isMutable:bool -> m:Range.range -> TypedTree.ValInline
    type NormalizedBindingRhs =
      | NormalizedBindingRhs of
        simplePats: SyntaxTree.SynSimplePats list *
        returnTyOpt: SyntaxTree.SynBindingReturnInfo option *
        rhsExpr: SyntaxTree.SynExpr
    val PushOnePatternToRhs:
      cenv:cenv ->
        isMember:bool ->
          p:SyntaxTree.SynPat -> NormalizedBindingRhs -> NormalizedBindingRhs
    type NormalizedBindingPatternInfo =
      | NormalizedBindingPat of
        SyntaxTree.SynPat * NormalizedBindingRhs * SyntaxTree.SynValData *
        SyntaxTree.SynValTyparDecls
    type NormalizedBinding =
      | NormalizedBinding of
        visibility: SyntaxTree.SynAccess option *
        kind: SyntaxTree.SynBindingKind * mustInline: bool * isMutable: bool *
        attribs: SyntaxTree.SynAttribute list * xmlDoc: XmlDoc.XmlDoc *
        typars: SyntaxTree.SynValTyparDecls * valSynData: SyntaxTree.SynValData *
        pat: SyntaxTree.SynPat * rhsExpr: NormalizedBindingRhs *
        mBinding: Range.range * spBinding: SyntaxTree.DebugPointForBinding
    type IsObjExprBinding =
      | ObjExprBinding
      | ValOrMemberBinding
    module BindingNormalization =
      val private PushMultiplePatternsToRhs:
        cenv:cenv ->
          isMember:bool ->
            ps:SyntaxTree.SynPat list ->
              NormalizedBindingRhs -> NormalizedBindingRhs
      val private MakeNormalizedStaticOrValBinding:
        cenv:cenv ->
          isObjExprBinding:IsObjExprBinding ->
            id:SyntaxTree.Ident ->
              vis:SyntaxTree.SynAccess option ->
                typars:SyntaxTree.SynValTyparDecls ->
                  args:SyntaxTree.SynPat list ->
                    rhsExpr:NormalizedBindingRhs ->
                      valSynData:SyntaxTree.SynValData ->
                        NormalizedBindingPatternInfo
      val private MakeNormalizedInstanceMemberBinding:
        cenv:cenv ->
          thisId:SyntaxTree.Ident ->
            memberId:SyntaxTree.Ident ->
              toolId:SyntaxTree.Ident option ->
                vis:SyntaxTree.SynAccess option ->
                  m:Range.range ->
                    typars:SyntaxTree.SynValTyparDecls ->
                      args:SyntaxTree.SynPat list ->
                        rhsExpr:NormalizedBindingRhs ->
                          valSynData:SyntaxTree.SynValData ->
                            NormalizedBindingPatternInfo
      val private NormalizeStaticMemberBinding:
        cenv:cenv ->
          memberFlags:SyntaxTree.MemberFlags ->
            valSynData:SyntaxTree.SynValData ->
              id:SyntaxTree.Ident ->
                vis:SyntaxTree.SynAccess option ->
                  typars:SyntaxTree.SynValTyparDecls ->
                    args:SyntaxTree.SynPat list ->
                      m:Range.range ->
                        rhsExpr:NormalizedBindingRhs ->
                          NormalizedBindingPatternInfo
      val private NormalizeInstanceMemberBinding:
        cenv:cenv ->
          memberFlags:SyntaxTree.MemberFlags ->
            valSynData:SyntaxTree.SynValData ->
              thisId:SyntaxTree.Ident ->
                memberId:SyntaxTree.Ident ->
                  toolId:SyntaxTree.Ident option ->
                    vis:SyntaxTree.SynAccess option ->
                      typars:SyntaxTree.SynValTyparDecls ->
                        args:SyntaxTree.SynPat list ->
                          m:Range.range ->
                            rhsExpr:NormalizedBindingRhs ->
                              NormalizedBindingPatternInfo
      val private NormalizeBindingPattern:
        cenv:TcFileState ->
          nameResolver:NameResolution.NameResolver ->
            isObjExprBinding:IsObjExprBinding ->
              env:TcEnv ->
                valSynData:SyntaxTree.SynValData ->
                  pat:SyntaxTree.SynPat ->
                    rhsExpr:NormalizedBindingRhs -> NormalizedBindingPatternInfo
      val NormalizeBinding:
        isObjExprBinding:IsObjExprBinding ->
          cenv:TcFileState ->
            env:TcEnv -> binding:SyntaxTree.SynBinding -> NormalizedBinding
  
    module EventDeclarationNormalization =
      val ConvertSynInfo:
        m:Range.range -> SyntaxTree.SynValInfo -> SyntaxTree.SynValInfo
      val ConvertMemberFlags:
        memberFlags:SyntaxTree.MemberFlags -> SyntaxTree.MemberFlags
      val private ConvertMemberFlagsOpt:
        m:Range.range ->
          memberFlagsOpt:SyntaxTree.MemberFlags option ->
            SyntaxTree.MemberFlags option
      val private ConvertSynData:
        m:Range.range ->
          valSynData:SyntaxTree.SynValData -> SyntaxTree.SynValData
      val private RenameBindingPattern:
        f:(string -> string) ->
          declPattern:SyntaxTree.SynPat -> SyntaxTree.SynPat
      val GenerateExtraBindings:
        cenv:TcFileState ->
          bindingAttribs:TypedTree.Attribs * binding:NormalizedBinding ->
            NormalizedBinding list
  
    val FreshenObjectArgType:
      cenv:TcFileState ->
        m:Range.range ->
          rigid:TypedTree.TyparRigidity ->
            tcref:TypedTree.TyconRef ->
              isExtrinsic:bool ->
                declaredTyconTypars:TypedTree.Typar list ->
                  TypedTree.TType * TypedTree.Typar list *
                  TypedTreeOps.TyparInst * TypedTree.TType * TypedTree.TType
    val TcValEarlyGeneralizationConsistencyCheck:
      cenv:TcFileState ->
        env:TcEnv ->
          v:TypedTree.Val * vrec:TypedTree.ValRecursiveScopeInfo *
          tinst:TypedTree.TType list * vty:TypedTree.TType * tau:TypedTree.TType *
          m:Range.range -> unit
    val TcVal:
      checkAttributes:bool ->
        cenv:TcFileState ->
          env:TcEnv ->
            tpenv:'a ->
              vref:TypedTree.ValRef ->
                optInst:(TypedTree.ValUseFlag *
                         ('a -> TypedTree.TyparKind list ->
                            TypedTree.TypeInst * 'a)) option ->
                  optAfterResolution:NameResolution.AfterResolution option ->
                    m:Range.range ->
                      TypedTree.Typar list * TypedTree.Expr * bool *
                      TypedTree.TType * TypedTree.TType list * 'a
    val LightweightTcValForUsingInBuildMethodCall:
      g:TcGlobals.TcGlobals ->
        vref:TypedTree.ValRef ->
          vrefFlags:TypedTree.ValUseFlag ->
            vrefTypeInst:TypedTree.TTypes ->
              m:Range.range -> TypedTree.Expr * TypedTree.TType
    type ApplicableExpr =
      | ApplicableExpr of cenv * TypedTree.Expr * bool
      with
        member
          SupplyArgument: e2:TypedTree.Expr * m:Range.range -> ApplicableExpr
        member Expr: TypedTree.Expr
        member Range: Range.range
        member Type: TypedTree.TType
    
    val MakeApplicableExprNoFlex:
      cenv:cenv -> expr:TypedTree.Expr -> ApplicableExpr
    val MakeApplicableExprWithFlex:
      cenv:TcFileState -> env:TcEnv -> expr:TypedTree.Expr -> ApplicableExpr
    val TcRuntimeTypeTest:
      isCast:bool ->
        isOperator:bool ->
          cenv:TcFileState ->
            denv:TypedTreeOps.DisplayEnv ->
              m:Range.range ->
                tgtTy:TypedTree.TType -> srcTy:TypedTree.TType -> unit
    val TcStaticUpcast:
      cenv:TcFileState ->
        denv:TypedTreeOps.DisplayEnv ->
          m:Range.range ->
            tgtTy:TypedTree.TType -> srcTy:TypedTree.TType -> unit
    val BuildPossiblyConditionalMethodCall:
      cenv:TcFileState ->
        env:TcEnv ->
          isMutable:TypedTreeOps.Mutates ->
            m:Range.range ->
              isProp:bool ->
                minfo:Infos.MethInfo ->
                  valUseFlags:TypedTree.ValUseFlag ->
                    minst:TypedTree.TType list ->
                      objArgs:TypedTree.Expr list ->
                        args:TypedTree.Exprs -> TypedTree.Expr * TypedTree.TType
    val TryFindIntrinsicOrExtensionMethInfo:
      collectionSettings:NameResolution.ResultCollectionSettings ->
        cenv:cenv ->
          env:TcEnv ->
            m:Range.range ->
              ad:AccessibilityLogic.AccessorDomain ->
                nm:string -> ty:TypedTree.TType -> Infos.MethInfo list
    val TryFindFSharpSignatureInstanceGetterProperty:
      cenv:cenv ->
        env:TcEnv ->
          m:Range.range ->
            nm:string ->
              ty:TypedTree.TType ->
                sigTys:TypedTree.TType list -> Infos.PropInfo option
    val BuildDisposableCleanup:
      cenv:cenv ->
        env:TcEnv -> m:Range.range -> v:TypedTree.Val -> TypedTree.Expr
    val BuildOffsetToStringData:
      cenv:cenv -> env:TcEnv -> m:Range.range -> TypedTree.Expr
    val BuildILFieldGet:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          m:Range.range ->
            objExpr:TypedTree.Expr -> finfo:Infos.ILFieldInfo -> TypedTree.Expr
    val private CheckFieldLiteralArg:
      finfo:Infos.ILFieldInfo -> argExpr:TypedTree.Expr -> m:Range.range -> unit
    val BuildILFieldSet:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          objExpr:TypedTree.Expr ->
            finfo:Infos.ILFieldInfo -> argExpr:TypedTree.Expr -> TypedTree.Expr
    val BuildILStaticFieldSet:
      m:Range.range ->
        finfo:Infos.ILFieldInfo -> argExpr:TypedTree.Expr -> TypedTree.Expr
    val BuildRecdFieldSet:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          objExpr:TypedTree.Expr ->
            rfinfo:Infos.RecdFieldInfo ->
              argExpr:TypedTree.Expr -> TypedTree.Expr
    val ( |BinOpExpr|_| ):
      SyntaxTree.SynExpr ->
        (SyntaxTree.Ident * SyntaxTree.SynExpr * SyntaxTree.SynExpr) option
    val ( |SimpleEqualsExpr|_| ):
      e:SyntaxTree.SynExpr -> (SyntaxTree.SynExpr * SyntaxTree.SynExpr) option
    val TryGetNamedArg:
      e:SyntaxTree.SynExpr ->
        (bool * SyntaxTree.Ident * SyntaxTree.SynExpr) option
    val inline IsNamedArg: e:SyntaxTree.SynExpr -> bool
    val GetMethodArgs:
      arg:SyntaxTree.SynExpr ->
        SyntaxTree.SynExpr list *
        (bool * SyntaxTree.Ident * SyntaxTree.SynExpr) list
    val CompilePatternForMatch:
      cenv:TcFileState ->
        env:TcEnv ->
          mExpr:Range.range ->
            matchm:Range.range ->
              warnOnUnused:bool ->
                actionOnFailure:PatternMatchCompilation.ActionOnFailure ->
                  inputVal:TypedTree.Val * generalizedTypars:TypedTree.Typars *
                  inputExprOpt:TypedTree.Expr option ->
                    clauses:PatternMatchCompilation.TypedMatchClause list ->
                      inputTy:TypedTree.TType ->
                        resultTy:TypedTree.TType -> TypedTree.Expr
    val CompilePatternForMatchClauses:
      cenv:TcFileState ->
        env:TcEnv ->
          mExpr:Range.range ->
            matchm:Range.range ->
              warnOnUnused:bool ->
                actionOnFailure:PatternMatchCompilation.ActionOnFailure ->
                  inputExprOpt:TypedTree.Expr option ->
                    inputTy:TypedTree.TType ->
                      resultTy:TypedTree.TType ->
                        tclauses:PatternMatchCompilation.TypedMatchClause list ->
                          TypedTree.Val * TypedTree.Expr
    val AnalyzeArbitraryExprAsEnumerable:
      cenv:cenv ->
        env:TcEnv ->
          localAlloc:bool ->
            m:Range.range ->
              exprty:TypedTree.TType ->
                expr:TypedTree.Expr ->
                  TypedTree.Val * TypedTree.Expr * TypedTree.TType *
                  TypedTree.TType * TypedTree.Expr * TypedTree.TType *
                  TypedTree.Expr * TypedTree.TType * TypedTree.Expr
    val ConvertArbitraryExprToEnumerable:
      cenv:cenv ->
        ty:TypedTree.TType ->
          env:TcEnv -> expr:TypedTree.Expr -> TypedTree.Expr * TypedTree.TType
    type InitializationGraphAnalysisState =
      | Top
      | InnerTop
      | DefinitelyStrict
      | MaybeLazy
      | DefinitelyLazy
    type PreInitializationGraphEliminationBinding =
      { FixupPoints: RecursiveUseFixupPoints
        Binding: TypedTree.Binding }
    val EliminateInitializationGraphs:
      g:TcGlobals.TcGlobals ->
        mustHaveArity:bool ->
          denv:TypedTreeOps.DisplayEnv ->
            bindings:'Binding list ->
              iterBindings:((PreInitializationGraphEliminationBinding list ->
                               unit) -> 'Binding list -> unit) ->
                buildLets:(TypedTree.Binding list -> 'Result) ->
                  mapBindings:((PreInitializationGraphEliminationBinding list ->
                                  TypedTree.Binding list) -> 'Binding list ->
                                 'Result list) ->
                    bindsm:Range.range -> 'Result list
    val CheckAndRewriteObjectCtor:
      g:TcGlobals.TcGlobals ->
        env:TcEnv -> ctorLambdaExpr:TypedTree.Expr -> TypedTree.Expr
    val buildApp:
      cenv:TcFileState ->
        expr:ApplicableExpr ->
          resultTy:TypedTree.TType ->
            arg:TypedTree.Expr ->
              m:Range.range -> ApplicableExpr * TypedTree.TType
    type DelayedItem =
      | DelayedTypeApp of SyntaxTree.SynType list * Range.range * Range.range
      | DelayedApp of
        SyntaxTree.ExprAtomicFlag * SyntaxTree.SynExpr * Range.range
      | DelayedDotLookup of SyntaxTree.Ident list * Range.range
      | DelayedDot
      | DelayedSet of SyntaxTree.SynExpr * Range.range
    val MakeDelayedSet: e:SyntaxTree.SynExpr * m:Range.range -> DelayedItem
    type NewSlotsOK =
      | NewSlotsOK
      | NoNewSlots
    type ImplicitlyBoundTyparsAllowed =
      | NewTyparsOKButWarnIfNotRigid
      | NewTyparsOK
      | NoNewTypars
    type CheckConstraints =
      | CheckCxs
      | NoCheckCxs
    type MemberOrValContainerInfo =
      | MemberOrValContainerInfo of
        tcref: TypedTree.TyconRef *
        optIntfSlotTy: (TypedTree.TType * MethodOverrides.SlotImplSet) option *
        baseValOpt: TypedTree.Val option * safeInitInfo: SafeInitData *
        declaredTyconTypars: TypedTree.Typars
    type ContainerInfo =
      | ContainerInfo of TypedTree.ParentRef * MemberOrValContainerInfo option
      with
        member ParentRef: TypedTree.ParentRef
    
    val ExprContainerInfo: ContainerInfo
    type NormalizedRecBindingDefn =
      | NormalizedRecBindingDefn of
        containerInfo: ContainerInfo * newslotsOk: NewSlotsOK *
        declKind: DeclKind * binding: NormalizedBinding
    type ValSpecResult =
      | ValSpecResult of
        altActualParent: TypedTree.ParentRef *
        memberInfoOpt: PreValMemberInfo option * id: SyntaxTree.Ident *
        enclosingDeclaredTypars: TypedTree.Typars *
        declaredTypars: TypedTree.Typars * ty: TypedTree.TType *
        partialValReprInfo: PartialValReprInfo * declKind: DeclKind
    type RecDefnBindingInfo =
      | RecDefnBindingInfo of
        containerInfo: ContainerInfo * newslotsOk: NewSlotsOK *
        declKind: DeclKind * synBinding: SyntaxTree.SynBinding
    type RecursiveBindingInfo =
      | RecursiveBindingInfo of
        recBindIndex: int * containerInfo: ContainerInfo *
        enclosingDeclaredTypars: TypedTree.Typars *
        inlineFlag: TypedTree.ValInline * vspec: TypedTree.Val *
        explicitTyparInfo: ExplicitTyparInfo *
        partialValReprInfo: PartialValReprInfo *
        memberInfoOpt: PreValMemberInfo option *
        baseValOpt: TypedTree.Val option * safeThisValOpt: TypedTree.Val option *
        safeInitInfo: SafeInitData * visibility: SyntaxTree.SynAccess option *
        ty: TypedTree.TType * declKind: DeclKind
      with
        member ContainerInfo: ContainerInfo
        member DeclKind: DeclKind
        member DeclaredTypars: TypedTree.Typars
        member EnclosingDeclaredTypars: TypedTree.Typars
        member ExplicitTyparInfo: ExplicitTyparInfo
        member Index: int
        member Val: TypedTree.Val
    
    type PreCheckingRecursiveBinding =
      { SyntacticBinding: NormalizedBinding
        RecBindingInfo: RecursiveBindingInfo }
    type PreGeneralizationRecursiveBinding =
      { ExtraGeneralizableTypars: TypedTree.Typars
        CheckedBinding: CheckedBindingInfo
        RecBindingInfo: RecursiveBindingInfo }
    type PostGeneralizationRecursiveBinding =
      { ValScheme: ValScheme
        CheckedBinding: CheckedBindingInfo
        RecBindingInfo: RecursiveBindingInfo }
      with
        member GeneralizedTypars: TypedTree.Typars
    
    type PostSpecialValsRecursiveBinding =
      { ValScheme: ValScheme
        Binding: TypedTree.Binding }
    val CanInferExtraGeneralizedTyparsForRecBinding:
      pgrbind:PreGeneralizationRecursiveBinding -> bool
    val GetInstanceMemberThisVariable:
      vspec:TypedTree.Val * expr:TypedTree.Expr -> TypedTree.Val option
    val TcTyparConstraint:
      ridx:int ->
        cenv:TcFileState ->
          newOk:ImplicitlyBoundTyparsAllowed ->
            checkCxs:CheckConstraints ->
              occ:NameResolution.ItemOccurence ->
                env:TcEnv ->
                  tpenv:UnscopedTyparEnv ->
                    c:SyntaxTree.SynTypeConstraint -> UnscopedTyparEnv
    val TcPseudoMemberSpec:
      cenv:TcFileState ->
        newOk:ImplicitlyBoundTyparsAllowed ->
          env:TcEnv ->
            synTypes:SyntaxTree.SynType list ->
              tpenv:UnscopedTyparEnv ->
                memSpfn:SyntaxTree.SynMemberSig ->
                  m:Range.range ->
                    TypedTree.TraitConstraintInfo * UnscopedTyparEnv
    val TcValSpec:
      cenv:TcFileState ->
        TcEnv ->
          DeclKind ->
            ImplicitlyBoundTyparsAllowed ->
              ContainerInfo ->
                SyntaxTree.MemberFlags option ->
                  thisTyOpt:TypedTree.TType option ->
                    UnscopedTyparEnv ->
                      SyntaxTree.SynValSig ->
                        TypedTree.Attrib list ->
                          ValSpecResult list * UnscopedTyparEnv
    val TcTyparOrMeasurePar:
      optKind:TypedTree.TyparKind option ->
        cenv:TcFileState ->
          env:TcEnv ->
            newOk:ImplicitlyBoundTyparsAllowed ->
              tpenv:UnscopedTyparEnv ->
                SyntaxTree.SynTypar -> TypedTree.Typar * UnscopedTyparEnv
    val TcTypar:
      cenv:TcFileState ->
        env:TcEnv ->
          newOk:ImplicitlyBoundTyparsAllowed ->
            tpenv:UnscopedTyparEnv ->
              tp:SyntaxTree.SynTypar -> TypedTree.Typar * UnscopedTyparEnv
    val TcTyparDecl:
      cenv:TcFileState ->
        env:TcEnv -> SyntaxTree.SynTyparDecl -> TypedTree.Typar
    val TcTyparDecls:
      cenv:TcFileState ->
        env:TcEnv ->
          synTypars:SyntaxTree.SynTyparDecl list -> TypedTree.Typar list
    val TcTypeOrMeasure:
      optKind:TypedTree.TyparKind option ->
        cenv:TcFileState ->
          newOk:ImplicitlyBoundTyparsAllowed ->
            checkCxs:CheckConstraints ->
              occ:NameResolution.ItemOccurence ->
                env:TcEnv ->
                  tpenv:UnscopedTyparEnv ->
                    ty:SyntaxTree.SynType -> TypedTree.TType * UnscopedTyparEnv
    val TcType:
      cenv:TcFileState ->
        newOk:ImplicitlyBoundTyparsAllowed ->
          checkCxs:CheckConstraints ->
            occ:NameResolution.ItemOccurence ->
              env:TcEnv ->
                tpenv:UnscopedTyparEnv ->
                  ty:SyntaxTree.SynType -> TypedTree.TType * UnscopedTyparEnv
    val TcMeasure:
      cenv:TcFileState ->
        newOk:ImplicitlyBoundTyparsAllowed ->
          checkCxs:CheckConstraints ->
            occ:NameResolution.ItemOccurence ->
              env:TcEnv ->
                tpenv:UnscopedTyparEnv ->
                  SyntaxTree.SynType ->
                    m:Range.range -> TypedTree.Measure * UnscopedTyparEnv
    val TcAnonTypeOrMeasure:
      optKind:TypedTree.TyparKind option ->
        _cenv:TcFileState ->
          rigid:TypedTree.TyparRigidity ->
            dyn:TypedTree.TyparDynamicReq ->
              newOk:ImplicitlyBoundTyparsAllowed ->
                m:Range.range -> TypedTree.Typar
    val TcTypes:
      cenv:TcFileState ->
        newOk:ImplicitlyBoundTyparsAllowed ->
          checkCxs:CheckConstraints ->
            occ:NameResolution.ItemOccurence ->
              env:TcEnv ->
                tpenv:UnscopedTyparEnv ->
                  args:SyntaxTree.SynType list ->
                    TypedTree.TType list * UnscopedTyparEnv
    val TcTypesAsTuple:
      cenv:TcFileState ->
        newOk:ImplicitlyBoundTyparsAllowed ->
          checkCxs:CheckConstraints ->
            occ:NameResolution.ItemOccurence ->
              env:TcEnv ->
                tpenv:UnscopedTyparEnv ->
                  args:(bool * SyntaxTree.SynType) list ->
                    m:Range.range -> TypedTree.TTypes * UnscopedTyparEnv
    val TcMeasuresAsTuple:
      cenv:TcFileState ->
        newOk:ImplicitlyBoundTyparsAllowed ->
          checkCxs:CheckConstraints ->
            occ:NameResolution.ItemOccurence ->
              env:TcEnv ->
                tpenv:UnscopedTyparEnv ->
                  args:(bool * SyntaxTree.SynType) list ->
                    m:Range.range -> TypedTree.Measure * UnscopedTyparEnv
    val TcTypesOrMeasures:
      optKinds:TypedTree.TyparKind list option ->
        cenv:TcFileState ->
          newOk:ImplicitlyBoundTyparsAllowed ->
            checkCxs:CheckConstraints ->
              occ:NameResolution.ItemOccurence ->
                env:TcEnv ->
                  tpenv:UnscopedTyparEnv ->
                    args:SyntaxTree.SynType list ->
                      m:Range.range -> TypedTree.TType list * UnscopedTyparEnv
    val TcTyparConstraints:
      cenv:TcFileState ->
        newOk:ImplicitlyBoundTyparsAllowed ->
          checkCxs:CheckConstraints ->
            occ:NameResolution.ItemOccurence ->
              env:TcEnv ->
                tpenv:UnscopedTyparEnv ->
                  synConstraints:SyntaxTree.SynTypeConstraint list ->
                    UnscopedTyparEnv
    val TcStaticConstantParameter:
      cenv:TcFileState ->
        env:TcEnv ->
          tpenv:UnscopedTyparEnv ->
            kind:TypedTree.TType ->
              SyntaxTree.SynType ->
                idOpt:SyntaxTree.Ident option ->
                  container:NameResolution.ArgumentContainer ->
                    obj * UnscopedTyparEnv
    val CrackStaticConstantArgs:
      cenv:TcFileState ->
        env:TcEnv ->
          tpenv:UnscopedTyparEnv ->
            staticParameters:Tainted<ExtensionTyping.ProvidedParameterInfo> [] *
            args:SyntaxTree.SynType list *
            container:NameResolution.ArgumentContainer *
            containerName:System.String * m:Range.range -> obj []
    val TcProvidedTypeAppToStaticConstantArgs:
      cenv:TcFileState ->
        env:TcEnv ->
          optGeneratedTypePath:string list option ->
            tpenv:UnscopedTyparEnv ->
              tcref:TypedTree.TyconRef ->
                args:SyntaxTree.SynType list ->
                  m:Range.range ->
                    bool * Tainted<ExtensionTyping.ProvidedType> *
                    (unit -> unit)
    val TryTcMethodAppToStaticConstantArgs:
      cenv:TcFileState ->
        env:TcEnv ->
          tpenv:UnscopedTyparEnv ->
            minfos:Infos.MethInfo list *
            argsOpt:(SyntaxTree.SynType list * 'a) option *
            mExprAndArg:Range.range * mItem:Range.range -> Infos.MethInfo option
    val TcProvidedMethodAppToStaticConstantArgs:
      cenv:TcFileState ->
        env:TcEnv ->
          tpenv:UnscopedTyparEnv ->
            minfo:Infos.MethInfo *
            methBeforeArguments:Tainted<ExtensionTyping.ProvidedMethodBase> *
            staticParams:Tainted<ExtensionTyping.ProvidedParameterInfo> [] *
            args:SyntaxTree.SynType list * m:Range.range ->
              Tainted<ExtensionTyping.ProvidedMethodBase>
    val TcProvidedTypeApp:
      cenv:TcFileState ->
        env:TcEnv ->
          tpenv:UnscopedTyparEnv ->
            tcref:TypedTree.TyconRef ->
              args:SyntaxTree.SynType list ->
                m:Range.range -> TypedTree.TType * UnscopedTyparEnv
    val TcTypeApp:
      cenv:TcFileState ->
        newOk:ImplicitlyBoundTyparsAllowed ->
          checkCxs:CheckConstraints ->
            occ:NameResolution.ItemOccurence ->
              env:TcEnv ->
                tpenv:UnscopedTyparEnv ->
                  m:Range.range ->
                    tcref:TypedTree.TyconRef ->
                      pathTypeArgs:NameResolution.EnclosingTypeInst ->
                        synArgTys:SyntaxTree.SynType list ->
                          TypedTree.TType * UnscopedTyparEnv
    val TcTypeOrMeasureAndRecover:
      optKind:TypedTree.TyparKind option ->
        cenv:TcFileState ->
          newOk:ImplicitlyBoundTyparsAllowed ->
            checkCxs:CheckConstraints ->
              occ:NameResolution.ItemOccurence ->
                env:TcEnv ->
                  tpenv:UnscopedTyparEnv ->
                    ty:SyntaxTree.SynType -> TypedTree.TType * UnscopedTyparEnv
    val TcTypeAndRecover:
      cenv:TcFileState ->
        newOk:ImplicitlyBoundTyparsAllowed ->
          checkCxs:CheckConstraints ->
            occ:NameResolution.ItemOccurence ->
              env:TcEnv ->
                tpenv:UnscopedTyparEnv ->
                  ty:SyntaxTree.SynType -> TypedTree.TType * UnscopedTyparEnv
    val TcNestedTypeApplication:
      cenv:TcFileState ->
        newOk:ImplicitlyBoundTyparsAllowed ->
          checkCxs:CheckConstraints ->
            occ:NameResolution.ItemOccurence ->
              env:TcEnv ->
                tpenv:UnscopedTyparEnv ->
                  mWholeTypeApp:Range.range ->
                    ty:TypedTree.TType ->
                      pathTypeArgs:NameResolution.EnclosingTypeInst ->
                        tyargs:SyntaxTree.SynType list ->
                          TypedTree.TType * UnscopedTyparEnv
    val TryAdjustHiddenVarNameToCompGenName:
      cenv:TcFileState ->
        env:TcEnv ->
          id:SyntaxTree.Ident ->
            altNameRefCellOpt:Ref<SyntaxTree.SynSimplePatAlternativeIdInfo> option ->
              SyntaxTree.Ident option
    val TcSimplePat:
      optArgsOK:bool ->
        checkCxs:CheckConstraints ->
          cenv:TcFileState ->
            ty:TypedTree.TType ->
              env:TcEnv ->
                tpenv:UnscopedTyparEnv * names:Map<string,PrelimValScheme1> *
                takenNames:Set<string> ->
                  p:SyntaxTree.SynSimplePat ->
                    string *
                    (UnscopedTyparEnv * Map<string,PrelimValScheme1> *
                     Set<string>)
    val ValidateOptArgOrder: spats:SyntaxTree.SynSimplePats -> unit
    val TcSimplePats:
      cenv:TcFileState ->
        optArgsOK:bool ->
          checkCxs:CheckConstraints ->
            ty:TypedTree.TType ->
              env:TcEnv ->
                tpenv:UnscopedTyparEnv * names:Map<string,PrelimValScheme1> *
                takenNames:Set<string> ->
                  p:SyntaxTree.SynSimplePats ->
                    string list *
                    (UnscopedTyparEnv * Map<string,PrelimValScheme1> *
                     Set<string>)
    val TcSimplePatsOfUnknownType:
      cenv:TcFileState ->
        optArgsOK:bool ->
          checkCxs:CheckConstraints ->
            env:TcEnv ->
              tpenv:UnscopedTyparEnv ->
                spats:SyntaxTree.SynSimplePats ->
                  string list *
                  (UnscopedTyparEnv * Map<string,PrelimValScheme1> * Set<string>)
    val TcPatBindingName:
      cenv:TcFileState ->
        env:TcEnv ->
          id:SyntaxTree.Ident ->
            ty:TypedTree.TType ->
              isMemberThis:bool ->
                vis1:SyntaxTree.SynAccess option ->
                  topValData:PartialValReprInfo option ->
                    inlineFlag:TypedTree.ValInline *
                    declaredTypars:ExplicitTyparInfo *
                    argAttribs:ArgAndRetAttribs * isMutable:bool *
                    vis2:SyntaxTree.SynAccess option * compgen:bool ->
                      names:Map<string,PrelimValScheme1> *
                      takenNames:Set<string> ->
                        (TcPatPhase2Input ->
                           PatternMatchCompilation.PatternValBinding) *
                        Map<string,PrelimValScheme1> * Set<string>
    val TcPatAndRecover:
      warnOnUpper:NameResolution.WarnOnUpperFlag ->
        cenv:TcFileState ->
          env:TcEnv ->
            topValInfo:PartialValReprInfo option ->
              TypedTree.ValInline * ExplicitTyparInfo * ArgAndRetAttribs * bool *
              SyntaxTree.SynAccess option * bool ->
                tpenv:UnscopedTyparEnv * names:Map<string,PrelimValScheme1> *
                takenNames:Set<string> ->
                  ty:TypedTree.TType ->
                    pat:SyntaxTree.SynPat ->
                      (TcPatPhase2Input -> PatternMatchCompilation.Pattern) *
                      (UnscopedTyparEnv * Map<string,PrelimValScheme1> *
                       Set<string>)
    val TcPat:
      warnOnUpper:NameResolution.WarnOnUpperFlag ->
        cenv:TcFileState ->
          env:TcEnv ->
            topValInfo:PartialValReprInfo option ->
              TypedTree.ValInline * ExplicitTyparInfo * ArgAndRetAttribs * bool *
              SyntaxTree.SynAccess option * bool ->
                tpenv:UnscopedTyparEnv * names:Map<string,PrelimValScheme1> *
                takenNames:Set<string> ->
                  ty:TypedTree.TType ->
                    pat:SyntaxTree.SynPat ->
                      (TcPatPhase2Input -> PatternMatchCompilation.Pattern) *
                      (UnscopedTyparEnv * Map<string,PrelimValScheme1> *
                       Set<string>)
    val TcPatterns:
      warnOnUpper:NameResolution.WarnOnUpperFlag ->
        cenv:TcFileState ->
          env:TcEnv ->
            TypedTree.ValInline * ExplicitTyparInfo * ArgAndRetAttribs * bool *
            SyntaxTree.SynAccess option * bool ->
              UnscopedTyparEnv * Map<string,PrelimValScheme1> * Set<string> ->
                argTys:TypedTree.TType list ->
                  args:SyntaxTree.SynPat list ->
                    (TcPatPhase2Input -> PatternMatchCompilation.Pattern) list *
                    (UnscopedTyparEnv * Map<string,PrelimValScheme1> *
                     Set<string>)
    val solveTypAsError:
      cenv:TcFileState ->
        denv:TypedTreeOps.DisplayEnv ->
          m:Range.range -> ty:TypedTree.TType -> unit
    val RecordNameAndTypeResolutions_IdeallyWithoutHavingOtherEffects:
      cenv:TcFileState ->
        env:TcEnv -> tpenv:UnscopedTyparEnv -> expr:SyntaxTree.SynExpr -> unit
    val RecordNameAndTypeResolutions_IdeallyWithoutHavingOtherEffects_Delayed:
      cenv:TcFileState ->
        env:TcEnv -> tpenv:UnscopedTyparEnv -> delayed:DelayedItem list -> unit
    val UnifyTypesAndRecover:
      cenv:TcFileState ->
        env:TcEnv ->
          m:Range.range ->
            expectedTy:TypedTree.TType -> actualTy:TypedTree.TType -> unit
    val TcExprOfUnknownType:
      cenv:TcFileState ->
        env:TcEnv ->
          tpenv:UnscopedTyparEnv ->
            expr:SyntaxTree.SynExpr ->
              TypedTree.Expr * TypedTree.TType * UnscopedTyparEnv
    val TcExprFlex:
      cenv:TcFileState ->
        flex:bool ->
          compat:bool ->
            ty:TypedTree.TType ->
              env:TcEnv ->
                tpenv:UnscopedTyparEnv ->
                  e:SyntaxTree.SynExpr -> TypedTree.Expr * UnscopedTyparEnv
    val TcExpr:
      cenv:TcFileState ->
        ty:TypedTree.TType ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              expr:SyntaxTree.SynExpr -> TypedTree.Expr * UnscopedTyparEnv
    val TcExprNoRecover:
      cenv:TcFileState ->
        ty:TypedTree.TType ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              expr:SyntaxTree.SynExpr -> TypedTree.Expr * UnscopedTyparEnv
    val TcExprOfUnknownTypeThen:
      cenv:TcFileState ->
        env:TcEnv ->
          tpenv:UnscopedTyparEnv ->
            expr:SyntaxTree.SynExpr ->
              delayed:DelayedItem list ->
                TypedTree.Expr * TypedTree.TType * UnscopedTyparEnv
    val TcExprThatIsCtorBody:
      TypedTree.Val option * SafeInitData ->
        cenv:TcFileState ->
          overallTy:TypedTree.TType ->
            env:TcEnv ->
              tpenv:UnscopedTyparEnv ->
                expr:SyntaxTree.SynExpr -> TypedTree.Expr * UnscopedTyparEnv
    val TcExprThatCanBeCtorBody:
      cenv:TcFileState ->
        overallTy:TypedTree.TType ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              expr:SyntaxTree.SynExpr -> TypedTree.Expr * UnscopedTyparEnv
    val TcExprThatCantBeCtorBody:
      cenv:TcFileState ->
        overallTy:TypedTree.TType ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              expr:SyntaxTree.SynExpr -> TypedTree.Expr * UnscopedTyparEnv
    val TcStmtThatCantBeCtorBody:
      cenv:TcFileState ->
        env:TcEnv ->
          tpenv:UnscopedTyparEnv ->
            expr:SyntaxTree.SynExpr -> TypedTree.Expr * UnscopedTyparEnv
    val TcStmt:
      cenv:TcFileState ->
        env:TcEnv ->
          tpenv:UnscopedTyparEnv ->
            synExpr:SyntaxTree.SynExpr -> TypedTree.Expr * UnscopedTyparEnv
    val TryTcStmt:
      cenv:TcFileState ->
        env:TcEnv ->
          tpenv:UnscopedTyparEnv ->
            synExpr:SyntaxTree.SynExpr ->
              bool * TypedTree.Expr * UnscopedTyparEnv
    val TcExprThen:
      cenv:TcFileState ->
        overallTy:TypedTree.TType ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              synExpr:SyntaxTree.SynExpr ->
                delayed:DelayedItem list -> TypedTree.Expr * UnscopedTyparEnv
    val TcExprs:
      cenv:TcFileState ->
        env:TcEnv ->
          m:Range.range ->
            tpenv:UnscopedTyparEnv ->
              flexes:bool list ->
                argTys:TypedTree.TType list ->
                  args:SyntaxTree.SynExpr list ->
                    TypedTree.Expr list * UnscopedTyparEnv
    val CheckSuperInit:
      cenv:TcFileState -> objTy:TypedTree.TType -> m:Range.range -> unit
    val TcExprUndelayedNoType:
      cenv:TcFileState ->
        env:TcEnv ->
          tpenv:UnscopedTyparEnv ->
            synExpr:SyntaxTree.SynExpr ->
              TypedTree.Expr * TypedTree.TType * UnscopedTyparEnv
    val TcExprUndelayed:
      cenv:TcFileState ->
        overallTy:TypedTree.TType ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              synExpr:SyntaxTree.SynExpr -> TypedTree.Expr * UnscopedTyparEnv
    val TcIteratedLambdas:
      cenv:TcFileState ->
        isFirst:bool ->
          env:TcEnv ->
            overallTy:TypedTree.TType ->
              takenNames:Set<string> ->
                tpenv:UnscopedTyparEnv ->
                  e:SyntaxTree.SynExpr -> TypedTree.Expr * UnscopedTyparEnv
    val TcIndexerThen:
      cenv:TcFileState ->
        env:TcEnv ->
          overallTy:TypedTree.TType ->
            mWholeExpr:Range.range ->
              mDot:Range.range ->
                tpenv:UnscopedTyparEnv ->
                  wholeExpr:SyntaxTree.SynExpr ->
                    e1:SyntaxTree.SynExpr ->
                      indexArgs:SyntaxTree.SynIndexerArg list ->
                        delayed:DelayedItem list ->
                          TypedTree.Expr * UnscopedTyparEnv
    val TcNewExpr:
      cenv:TcFileState ->
        env:TcEnv ->
          tpenv:UnscopedTyparEnv ->
            objTy:TypedTree.TType ->
              mObjTyOpt:Range.range option ->
                superInit:bool ->
                  arg:SyntaxTree.SynExpr ->
                    mWholeExprOrObjTy:Range.range ->
                      TypedTree.Expr * UnscopedTyparEnv
    val TcCtorCall:
      isNaked:bool ->
        cenv:TcFileState ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              overallTy:TypedTree.TType ->
                objTy:TypedTree.TType ->
                  mObjTyOpt:Range.range option ->
                    item:NameResolution.Item ->
                      superInit:bool ->
                        args:SyntaxTree.SynExpr list ->
                          mWholeCall:Range.range ->
                            delayed:DelayedItem list ->
                              afterTcOverloadResolutionOpt:NameResolution.AfterResolution option ->
                                TypedTree.Expr * UnscopedTyparEnv
    val TcRecordConstruction:
      cenv:TcFileState ->
        overallTy:TypedTree.TType ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              optOrigExprInfo:(TypedTree.Expr * TypedTree.Val * TypedTree.Expr) option ->
                objTy:TypedTree.TType ->
                  fldsList:seq<string * SyntaxTree.SynExpr> ->
                    m:Range.range -> TypedTree.Expr * UnscopedTyparEnv
    val GetNameAndArityOfObjExprBinding:
      _cenv:'b ->
        _env:'c -> b:NormalizedBinding -> string * SyntaxTree.SynValInfo
    val FreshenObjExprAbstractSlot:
      cenv:TcFileState ->
        env:TcEnv ->
          implty:TypedTree.TType ->
            virtNameAndArityPairs:((string * 'd) * Infos.MethInfo) list ->
              bind:NormalizedBinding * bindAttribs:TypedTree.Attribs *
              bindName:string * absSlots:('e * Infos.MethInfo) list ->
                (bool * TypedTree.Typars * TypedTree.TType) option
    val TcObjectExprBinding:
      cenv:cenv ->
        env:TcEnv ->
          implty:TypedTree.TType ->
            tpenv:UnscopedTyparEnv ->
              absSlotInfo:(bool * TypedTree.Typars * TypedTree.TType) option *
              bind:NormalizedBinding ->
                (SyntaxTree.Ident * SyntaxTree.MemberFlags * TypedTree.TType *
                 TypedTree.Attribs * TypedTree.Expr) * UnscopedTyparEnv
    val ComputeObjectExprOverrides:
      cenv:TcFileState ->
        env:TcEnv ->
          tpenv:UnscopedTyparEnv ->
            impls:(Range.range * TypedTree.TType * SyntaxTree.SynBinding list) list ->
              (Range.range * TypedTree.TType * MethodOverrides.RequiredSlot list *
               AbstractIL.Internal.Library.NameMultiMap<MethodOverrides.RequiredSlot> *
               MethodOverrides.OverrideInfo list *
               (MethodOverrides.OverrideInfo *
                (TypedTree.Val option * TypedTree.Val * TypedTree.Val list list *
                 TypedTree.Attribs * TypedTree.Expr)) list) list *
              UnscopedTyparEnv
    val CheckSuperType:
      cenv:TcFileState -> ty:TypedTree.TType -> m:Range.range -> unit
    val TcObjectExpr:
      cenv:TcFileState ->
        overallTy:TypedTree.TType ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              synObjTy:SyntaxTree.SynType *
              argopt:(SyntaxTree.SynExpr * SyntaxTree.Ident option) option *
              binds:SyntaxTree.SynBinding list *
              extraImpls:SyntaxTree.SynInterfaceImpl list * mNewExpr:Range.range *
              mWholeExpr:Range.range -> TypedTree.Expr * UnscopedTyparEnv
    val TcConstStringExpr:
      cenv:TcFileState ->
        overallTy:TypedTree.TType ->
          env:TcEnv ->
            m:Range.range ->
              tpenv:UnscopedTyparEnv ->
                s:string -> TypedTree.Expr * UnscopedTyparEnv
    val TcFormatStringExpr:
      cenv:TcFileState ->
        overallTy:TypedTree.TType ->
          env:TcEnv ->
            m:Range.range ->
              tpenv:UnscopedTyparEnv ->
                fmtString:string -> TypedTree.Expr * UnscopedTyparEnv
    val TcInterpolatedStringExpr:
      cenv:TcFileState ->
        overallTy:TypedTree.TType ->
          env:TcEnv ->
            m:Range.range ->
              tpenv:UnscopedTyparEnv ->
                parts:SyntaxTree.SynInterpolatedStringPart list ->
                  TypedTree.Expr * UnscopedTyparEnv
    val TcConstExpr:
      cenv:TcFileState ->
        overallTy:TypedTree.TType ->
          env:TcEnv ->
            m:Range.range ->
              tpenv:UnscopedTyparEnv ->
                c:SyntaxTree.SynConst -> TypedTree.Expr * UnscopedTyparEnv
    val TcAssertExpr:
      cenv:TcFileState ->
        overallTy:TypedTree.TType ->
          env:TcEnv ->
            m:Range.range ->
              tpenv:UnscopedTyparEnv ->
                x:SyntaxTree.SynExpr -> TypedTree.Expr * UnscopedTyparEnv
    val TcRecdExpr:
      cenv:TcFileState ->
        overallTy:TypedTree.TType ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              inherits:(SyntaxTree.SynType * SyntaxTree.SynExpr * Range.range *
                        SyntaxTree.BlockSeparator option * Range.range) option *
              optOrigExpr:(SyntaxTree.SynExpr * SyntaxTree.BlockSeparator) option *
              flds:(SyntaxTree.RecordFieldName * SyntaxTree.SynExpr option *
                    SyntaxTree.BlockSeparator option) list *
              mWholeExpr:Range.range -> TypedTree.Expr * UnscopedTyparEnv
    val TcAnonRecdExpr:
      cenv:TcFileState ->
        overallTy:TypedTree.TType ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              isStruct:bool *
              optOrigSynExpr:(SyntaxTree.SynExpr * SyntaxTree.BlockSeparator) option *
              unsortedFieldIdsAndSynExprsGiven:(SyntaxTree.Ident *
                                                SyntaxTree.SynExpr) list *
              mWholeExpr:Range.range -> TypedTree.Expr * UnscopedTyparEnv
    val TcForEachExpr:
      cenv:TcFileState ->
        overallTy:TypedTree.TType ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              pat:SyntaxTree.SynPat * enumSynExpr:SyntaxTree.SynExpr *
              bodySynExpr:SyntaxTree.SynExpr * mWholeExpr:Range.range *
              spForLoop:SyntaxTree.DebugPointAtFor ->
                TypedTree.Expr * UnscopedTyparEnv
    val TcQuotationExpr:
      cenv:TcFileState ->
        overallTy:TypedTree.TType ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              _oper:SyntaxTree.SynExpr * raw:bool * ast:SyntaxTree.SynExpr *
              isFromQueryExpression:bool * m:Range.range ->
                TypedTree.Expr * UnscopedTyparEnv
    val Propagate:
      cenv:TcFileState ->
        overallTy:TypedTree.TType ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              expr:ApplicableExpr ->
                exprty:TypedTree.TType -> delayed:DelayedItem list -> unit
    val PropagateThenTcDelayed:
      cenv:TcFileState ->
        overallTy:TypedTree.TType ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              mExpr:Range.range ->
                expr:ApplicableExpr ->
                  exprty:TypedTree.TType ->
                    atomicFlag:SyntaxTree.ExprAtomicFlag ->
                      delayed:DelayedItem list ->
                        TypedTree.Expr * UnscopedTyparEnv
    val TcDelayed:
      cenv:TcFileState ->
        overallTy:TypedTree.TType ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              mExpr:Range.range ->
                expr:ApplicableExpr ->
                  exprty:TypedTree.TType ->
                    atomicFlag:SyntaxTree.ExprAtomicFlag ->
                      delayed:DelayedItem list ->
                        TypedTree.Expr * UnscopedTyparEnv
    val delayRest:
      rest:SyntaxTree.Ident list ->
        mPrior:Range.range -> delayed:DelayedItem list -> DelayedItem list
    val TcNameOfExpr:
      cenv:TcFileState ->
        env:TcEnv ->
          tpenv:UnscopedTyparEnv -> synArg:SyntaxTree.SynExpr -> TypedTree.Expr
    val TcNameOfExprResult:
      cenv:TcFileState ->
        lastIdent:SyntaxTree.Ident -> m:Range.range -> TypedTree.Expr
    val TcFunctionApplicationThen:
      cenv:TcFileState ->
        overallTy:TypedTree.TType ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              mExprAndArg:Range.range ->
                expr:ApplicableExpr ->
                  exprty:TypedTree.TType ->
                    synArg:SyntaxTree.SynExpr ->
                      atomicFlag:SyntaxTree.ExprAtomicFlag ->
                        delayed:DelayedItem list ->
                          TypedTree.Expr * UnscopedTyparEnv
    val GetLongIdentTypeNameInfo:
      delayed:DelayedItem list -> NameResolution.TypeNameResolutionInfo
    val TcLongIdentThen:
      cenv:TcFileState ->
        overallTy:TypedTree.TType ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              SyntaxTree.LongIdentWithDots ->
                delayed:DelayedItem list -> TypedTree.Expr * UnscopedTyparEnv
    val TcItemThen:
      cenv:TcFileState ->
        overallTy:TypedTree.TType ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              tinstEnclosing:NameResolution.EnclosingTypeInst *
              item:NameResolution.Item * mItem:Range.range *
              rest:SyntaxTree.Ident list *
              afterResolution:NameResolution.AfterResolution ->
                delayed:DelayedItem list -> TypedTree.Expr * UnscopedTyparEnv
    val GetSynMemberApplicationArgs:
      delayed:DelayedItem list ->
        tpenv:'f ->
          SyntaxTree.ExprAtomicFlag *
          (SyntaxTree.SynType list * Range.range) option *
          SyntaxTree.SynExpr list * DelayedItem list * 'f
    val TcMemberTyArgsOpt:
      cenv:TcFileState ->
        env:TcEnv ->
          tpenv:UnscopedTyparEnv ->
            tyargsOpt:(SyntaxTree.SynType list * Range.range) option ->
              TypedTree.TType list option * UnscopedTyparEnv
    val GetMemberApplicationArgs:
      delayed:DelayedItem list ->
        cenv:TcFileState ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              SyntaxTree.ExprAtomicFlag * TypedTree.TType list option *
              SyntaxTree.SynExpr list * DelayedItem list * UnscopedTyparEnv
    val TcLookupThen:
      cenv:TcFileState ->
        overallTy:TypedTree.TType ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              mObjExpr:Range.range ->
                objExpr:TypedTree.Expr ->
                  objExprTy:TypedTree.TType ->
                    longId:SyntaxTree.Ident list ->
                      delayed:DelayedItem list ->
                        mExprAndLongId:Range.range ->
                          TypedTree.Expr * UnscopedTyparEnv
    val TcEventValueThen:
      cenv:TcFileState ->
        overallTy:TypedTree.TType ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              mItem:Range.range ->
                mExprAndItem:Range.range ->
                  objDetails:(TypedTree.Expr * TypedTree.TType) option ->
                    einfo:Infos.EventInfo ->
                      delayed:DelayedItem list ->
                        TypedTree.Expr * UnscopedTyparEnv
    val TcMethodApplicationThen:
      cenv:TcFileState ->
        env:TcEnv ->
          overallTy:TypedTree.TType ->
            objTyOpt:TypedTree.TType option ->
              tpenv:UnscopedTyparEnv ->
                callerTyArgs:TypedTree.TType list option ->
                  objArgs:TypedTree.Expr list ->
                    m:Range.range ->
                      mItem:Range.range ->
                        methodName:string ->
                          ad:AccessibilityLogic.AccessorDomain ->
                            mut:TypedTreeOps.Mutates ->
                              isProp:bool ->
                                meths:(Infos.MethInfo * Infos.PropInfo option) list ->
                                  afterResolution:NameResolution.AfterResolution ->
                                    isSuperInit:TypedTree.ValUseFlag ->
                                      args:SyntaxTree.SynExpr list ->
                                        atomicFlag:SyntaxTree.ExprAtomicFlag ->
                                          delayed:DelayedItem list ->
                                            TypedTree.Expr * UnscopedTyparEnv
    val GetNewInferenceTypeForMethodArg:
      cenv:TcFileState ->
        env:'g -> tpenv:'h -> x:SyntaxTree.SynExpr -> TypedTree.TType
    val TcMethodApplication:
      isCheckingAttributeCall:bool ->
        cenv:TcFileState ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              tyargsOpt:TypedTree.TType list option ->
                objArgs:TypedTree.Expr list ->
                  mMethExpr:Range.range ->
                    mItem:Range.range ->
                      methodName:string ->
                        objTyOpt:TypedTree.TType option ->
                          ad:AccessibilityLogic.AccessorDomain ->
                            mut:TypedTreeOps.Mutates ->
                              isProp:bool ->
                                calledMethsAndProps:(Infos.MethInfo *
                                                     Infos.PropInfo option) list ->
                                  afterResolution:NameResolution.AfterResolution ->
                                    isSuperInit:TypedTree.ValUseFlag ->
                                      curriedCallerArgs:SyntaxTree.SynExpr list ->
                                        exprTy:TypedTree.TType ->
                                          delayed:DelayedItem list ->
                                            (TypedTree.Expr *
                                             MethodCalls.CallerNamedArg<TypedTree.Expr> list *
                                             DelayedItem list) *
                                            UnscopedTyparEnv
    val TcSetterArgExpr:
      cenv:TcFileState ->
        env:TcEnv ->
          denv:TypedTreeOps.DisplayEnv ->
            objExpr:TypedTree.Expr ->
              ad:AccessibilityLogic.AccessorDomain ->
                MethodCalls.AssignedItemSetter<TypedTree.Expr> ->
                  (TypedTree.Expr -> TypedTree.Expr) option * TypedTree.Expr *
                  Range.range
    val TcUnnamedMethodArgs:
      cenv:TcFileState ->
        env:TcEnv ->
          lambdaPropagationInfo:(MethodCalls.ArgumentAnalysis [] [] *
                                 (SyntaxTree.Ident *
                                  MethodCalls.ArgumentAnalysis) [] []) [] ->
            tpenv:UnscopedTyparEnv ->
              args:MethodCalls.CallerArg<SyntaxTree.SynExpr> list list ->
                MethodCalls.CallerArg<TypedTree.Expr> list list *
                ((MethodCalls.ArgumentAnalysis [] [] *
                  (SyntaxTree.Ident * MethodCalls.ArgumentAnalysis) [] []) [] *
                 UnscopedTyparEnv)
    val TcUnnamedMethodArg:
      cenv:TcFileState ->
        env:TcEnv ->
          lambdaPropagationInfo:(MethodCalls.ArgumentAnalysis [] [] *
                                 (SyntaxTree.Ident *
                                  MethodCalls.ArgumentAnalysis) [] []) [] *
          tpenv:UnscopedTyparEnv ->
            i:int * j:int * MethodCalls.CallerArg<SyntaxTree.SynExpr> ->
              MethodCalls.CallerArg<TypedTree.Expr> *
              ((MethodCalls.ArgumentAnalysis [] [] *
                (SyntaxTree.Ident * MethodCalls.ArgumentAnalysis) [] []) [] *
               UnscopedTyparEnv)
    val TcMethodNamedArgs:
      cenv:TcFileState ->
        env:TcEnv ->
          lambdaPropagationInfo:(MethodCalls.ArgumentAnalysis [] [] *
                                 (SyntaxTree.Ident *
                                  MethodCalls.ArgumentAnalysis) [] []) [] ->
            tpenv:UnscopedTyparEnv ->
              args:MethodCalls.CallerNamedArg<SyntaxTree.SynExpr> list list ->
                MethodCalls.CallerNamedArg<TypedTree.Expr> list list *
                ((MethodCalls.ArgumentAnalysis [] [] *
                  (SyntaxTree.Ident * MethodCalls.ArgumentAnalysis) [] []) [] *
                 UnscopedTyparEnv)
    val TcMethodNamedArg:
      cenv:TcFileState ->
        env:TcEnv ->
          lambdaPropagationInfo:(MethodCalls.ArgumentAnalysis [] [] *
                                 (SyntaxTree.Ident *
                                  MethodCalls.ArgumentAnalysis) [] []) [] *
          tpenv:UnscopedTyparEnv ->
            MethodCalls.CallerNamedArg<SyntaxTree.SynExpr> ->
              MethodCalls.CallerNamedArg<TypedTree.Expr> *
              ((MethodCalls.ArgumentAnalysis [] [] *
                (SyntaxTree.Ident * MethodCalls.ArgumentAnalysis) [] []) [] *
               UnscopedTyparEnv)
    val TcMethodArg:
      cenv:TcFileState ->
        env:TcEnv ->
          lambdaPropagationInfo:(MethodCalls.ArgumentAnalysis [] [] *
                                 (SyntaxTree.Ident *
                                  MethodCalls.ArgumentAnalysis) [] []) [] *
          tpenv:UnscopedTyparEnv ->
            lambdaPropagationInfoForArg:MethodCalls.ArgumentAnalysis [] *
            MethodCalls.CallerArg<SyntaxTree.SynExpr> ->
              MethodCalls.CallerArg<TypedTree.Expr> *
              ((MethodCalls.ArgumentAnalysis [] [] *
                (SyntaxTree.Ident * MethodCalls.ArgumentAnalysis) [] []) [] *
               UnscopedTyparEnv)
    val TcNewDelegateThen:
      cenv:TcFileState ->
        overallTy:TypedTree.TType ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              mDelTy:Range.range ->
                mExprAndArg:Range.range ->
                  delegateTy:TypedTree.TType ->
                    arg:SyntaxTree.SynExpr ->
                      atomicFlag:SyntaxTree.ExprAtomicFlag ->
                        delayed:DelayedItem list ->
                          TypedTree.Expr * UnscopedTyparEnv
    val bindLetRec:
      binds:TypedTree.Bindings ->
        m:Range.range -> e:TypedTree.Expr -> TypedTree.Expr
    val CheckRecursiveBindingIds: binds:seq<SyntaxTree.SynBinding> -> unit
    val TcLinearExprs:
      bodyChecker:(TypedTree.TType -> TcEnv -> UnscopedTyparEnv ->
                     SyntaxTree.SynExpr -> TypedTree.Expr * UnscopedTyparEnv) ->
        cenv:TcFileState ->
          env:TcEnv ->
            overallTy:TypedTree.TType ->
              tpenv:UnscopedTyparEnv ->
                isCompExpr:bool ->
                  expr:SyntaxTree.SynExpr ->
                    cont:(TypedTree.Expr * UnscopedTyparEnv ->
                            TypedTree.Expr * UnscopedTyparEnv) ->
                      TypedTree.Expr * UnscopedTyparEnv
    val TcAndPatternCompileMatchClauses:
      mExpr:Range.range ->
        matchm:Range.range ->
          actionOnFailure:PatternMatchCompilation.ActionOnFailure ->
            cenv:TcFileState ->
              inputExprOpt:TypedTree.Expr option ->
                inputTy:TypedTree.TType ->
                  resultTy:TypedTree.TType ->
                    env:TcEnv ->
                      tpenv:UnscopedTyparEnv ->
                        synClauses:SyntaxTree.SynMatchClause list ->
                          TypedTree.Val * TypedTree.Expr * UnscopedTyparEnv
    val TcMatchPattern:
      cenv:TcFileState ->
        inputTy:TypedTree.TType ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              pat:SyntaxTree.SynPat * optWhenExpr:SyntaxTree.SynExpr option ->
                PatternMatchCompilation.Pattern * TypedTree.Expr option *
                TypedTree.Val list * TcEnv * UnscopedTyparEnv
    val TcMatchClauses:
      cenv:TcFileState ->
        inputTy:TypedTree.TType ->
          resultTy:TypedTree.TType ->
            env:TcEnv ->
              tpenv:UnscopedTyparEnv ->
                clauses:SyntaxTree.SynMatchClause list ->
                  PatternMatchCompilation.TypedMatchClause list *
                  UnscopedTyparEnv
    val TcMatchClause:
      cenv:TcFileState ->
        inputTy:TypedTree.TType ->
          resultTy:TypedTree.TType ->
            env:TcEnv ->
              isFirst:bool ->
                tpenv:UnscopedTyparEnv ->
                  SyntaxTree.SynMatchClause ->
                    PatternMatchCompilation.TypedMatchClause * UnscopedTyparEnv
    val TcStaticOptimizationConstraint:
      cenv:TcFileState ->
        env:TcEnv ->
          tpenv:UnscopedTyparEnv ->
            c:SyntaxTree.SynStaticOptimizationConstraint ->
              TypedTree.StaticOptimization * UnscopedTyparEnv
    val mkConvToNativeInt:
      g:TcGlobals.TcGlobals ->
        e:TypedTree.Expr -> m:Range.range -> TypedTree.Expr
    val TcAndBuildFixedExpr:
      cenv:TcFileState ->
        env:TcEnv ->
          overallPatTy:TypedTree.TType * fixedExpr:TypedTree.Expr *
          overallExprTy:TypedTree.TType * mBinding:Range.range -> TypedTree.Expr
    val TcNormalizedBinding:
      declKind:DeclKind ->
        cenv:cenv ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              overallTy:TypedTree.TType ->
                safeThisValOpt:TypedTree.Val option ->
                  safeInitInfo:SafeInitData ->
                    enclosingDeclaredTypars:TypedTree.Typar list *
                    ExplicitTyparInfo ->
                      bind:NormalizedBinding ->
                        CheckedBindingInfo * UnscopedTyparEnv
    val TcLiteral:
      cenv:cenv ->
        overallTy:TypedTree.TType ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              attrs:TypedTree.Attribs * synLiteralValExpr:SyntaxTree.SynExpr ->
                bool * TypedTree.Const option
    val TcBindingTyparDecls:
      alwaysRigid:bool ->
        cenv:TcFileState ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              SyntaxTree.SynValTyparDecls ->
                ExplicitTyparInfo * UnscopedTyparEnv
    val TcNonrecBindingTyparDecls:
      cenv:cenv ->
        env:TcEnv ->
          tpenv:UnscopedTyparEnv ->
            bind:NormalizedBinding -> ExplicitTyparInfo * UnscopedTyparEnv
    val TcNonRecursiveBinding:
      declKind:DeclKind ->
        cenv:TcFileState ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              ty:TypedTree.TType ->
                b:SyntaxTree.SynBinding -> CheckedBindingInfo * UnscopedTyparEnv
    val TcAttribute:
      canFail:bool ->
        cenv:TcFileState ->
          env:TcEnv ->
            attrTgt:System.AttributeTargets ->
              synAttr:SyntaxTree.SynAttribute ->
                (System.AttributeTargets * TypedTree.Attrib) list * bool
    val TcAttributesWithPossibleTargets:
      canFail:bool ->
        cenv:TcFileState ->
          env:TcEnv ->
            attrTgt:System.AttributeTargets ->
              synAttribs:SyntaxTree.SynAttribute list ->
                (System.AttributeTargets * TypedTree.Attrib) list * bool
    val TcAttributesMaybeFail:
      canFail:bool ->
        cenv:TcFileState ->
          env:TcEnv ->
            attrTgt:System.AttributeTargets ->
              synAttribs:SyntaxTree.SynAttribute list ->
                TypedTree.Attrib list * bool
    val TcAttributesCanFail:
      cenv:TcFileState ->
        env:TcEnv ->
          attrTgt:System.AttributeTargets ->
            synAttribs:SyntaxTree.SynAttribute list ->
              TypedTree.Attrib list * (unit -> TypedTree.Attribs)
    val TcAttributes:
      cenv:TcFileState ->
        env:TcEnv ->
          attrTgt:System.AttributeTargets ->
            synAttribs:SyntaxTree.SynAttribute list -> TypedTree.Attribs
    val TcLetBinding:
      cenv:TcFileState ->
        isUse:bool ->
          env:TcEnv ->
            containerInfo:ContainerInfo ->
              declKind:DeclKind ->
                tpenv:UnscopedTyparEnv ->
                  synBinds:SyntaxTree.SynBinding list *
                  synBindsRange:Range.range * scopem:Range.range ->
                    (TypedTree.Expr * TypedTree.TType ->
                       TypedTree.Expr * TypedTree.TType) * TcEnv *
                    UnscopedTyparEnv
    val TcLetBindings:
      cenv:TcFileState ->
        env:TcEnv ->
          containerInfo:ContainerInfo ->
            declKind:DeclKind ->
              tpenv:UnscopedTyparEnv ->
                binds:SyntaxTree.SynBinding list * bindsm:Range.range *
                scopem:Range.range ->
                  TypedTree.ModuleOrNamespaceExpr list * TcEnv *
                  UnscopedTyparEnv
    val CheckMemberFlags:
      optIntfSlotTy:'a option ->
        newslotsOK:NewSlotsOK ->
          overridesOK:OverridesOK ->
            memberFlags:SyntaxTree.MemberFlags -> m:Range.range -> unit
    val ApplyTypesFromArgumentPatterns:
      cenv:TcFileState * env:TcEnv * optArgsOK:bool * ty:TypedTree.TType *
      m:Range.range * tpenv:UnscopedTyparEnv * NormalizedBindingRhs *
      memberFlagsOpt:SyntaxTree.MemberFlags option -> unit
    val ComputeIsComplete:
      enclosingDeclaredTypars:TypedTree.Typar list ->
        declaredTypars:TypedTree.Typar list -> ty:TypedTree.TType -> bool
    val ApplyAbstractSlotInference:
      cenv:cenv ->
        envinner:TcEnv ->
          bindingTy:TypedTree.TType * m:Range.range *
          synTyparDecls:SyntaxTree.SynValTyparDecls *
          declaredTypars:TypedTree.Typars * memberId:SyntaxTree.Ident *
          tcrefObjTy:TypedTree.TType * renaming:TypedTreeOps.TyparInst *
          _objTy:'i *
          optIntfSlotTy:(TypedTree.TType * MethodOverrides.SlotImplSet) option *
          valSynData:SyntaxTree.SynValInfo * memberFlags:SyntaxTree.MemberFlags *
          attribs:TypedTree.Attribs -> TypedTree.TType list * TypedTree.Typars
    val CheckForNonAbstractInterface:
      declKind:DeclKind ->
        tcref:TypedTree.TyconRef ->
          memberFlags:SyntaxTree.MemberFlags -> m:Range.range -> unit
    val AnalyzeRecursiveStaticMemberOrValDecl:
      cenv:TcFileState * envinner:TcEnv * tpenv:'j * declKind:DeclKind *
      newslotsOK:NewSlotsOK * overridesOK:OverridesOK *
      tcrefContainerInfo:MemberOrValContainerInfo option * vis1:'k option *
      id:SyntaxTree.Ident * vis2:'k option * declaredTypars:'l *
      memberFlagsOpt:SyntaxTree.MemberFlags option *
      thisIdOpt:SyntaxTree.Ident option * bindingAttribs:TypedTree.Attribs *
      valSynInfo:SyntaxTree.SynValInfo * ty:TypedTree.TType * bindingRhs:'m *
      mBinding:Range.range * explicitTyparInfo:'n ->
        TcEnv * 'j * SyntaxTree.Ident * 'o option * PreValMemberInfo option *
        'k option * 'k option * TypedTree.Val option * TypedTree.Typar list *
        TypedTree.Val option * 'n * 'm * 'l
    val AnalyzeRecursiveInstanceMemberDecl:
      cenv:cenv * envinner:TcEnv * tpenv:'p * declKind:DeclKind *
      synTyparDecls:SyntaxTree.SynValTyparDecls *
      valSynInfo:SyntaxTree.SynValInfo * explicitTyparInfo:ExplicitTyparInfo *
      newslotsOK:NewSlotsOK * overridesOK:OverridesOK * vis1:'q option *
      thisId:SyntaxTree.Ident * memberId:SyntaxTree.Ident *
      toolId:SyntaxTree.Ident option * bindingAttribs:TypedTree.Attribs *
      vis2:'q option * tcrefContainerInfo:MemberOrValContainerInfo option *
      memberFlagsOpt:SyntaxTree.MemberFlags option * ty:TypedTree.TType *
      bindingRhs:NormalizedBindingRhs * mBinding:Range.range ->
        TcEnv * 'p * SyntaxTree.Ident * SyntaxTree.Ident option *
        PreValMemberInfo option * 'q option * 'q option * 'r option *
        TypedTree.Typar list * TypedTree.Val option * ExplicitTyparInfo *
        NormalizedBindingRhs * TypedTree.Typars
    val AnalyzeRecursiveDecl:
      cenv:TcFileState * envinner:TcEnv * tpenv:UnscopedTyparEnv *
      declKind:DeclKind * synTyparDecls:SyntaxTree.SynValTyparDecls *
      declaredTypars:TypedTree.Typars * thisIdOpt:SyntaxTree.Ident option *
      valSynInfo:SyntaxTree.SynValInfo * explicitTyparInfo:ExplicitTyparInfo *
      newslotsOK:NewSlotsOK * overridesOK:OverridesOK *
      vis1:SyntaxTree.SynAccess option * declPattern:SyntaxTree.SynPat *
      bindingAttribs:TypedTree.Attribs *
      tcrefContainerInfo:MemberOrValContainerInfo option *
      memberFlagsOpt:SyntaxTree.MemberFlags option * ty:TypedTree.TType *
      bindingRhs:NormalizedBindingRhs * mBinding:Range.range ->
        TcEnv * UnscopedTyparEnv * SyntaxTree.Ident * SyntaxTree.Ident option *
        PreValMemberInfo option * SyntaxTree.SynAccess option *
        SyntaxTree.SynAccess option * TypedTree.Val option *
        TypedTree.Typar list * TypedTree.Val option * ExplicitTyparInfo *
        NormalizedBindingRhs * TypedTree.Typars
    val AnalyzeAndMakeAndPublishRecursiveValue:
      overridesOK:OverridesOK ->
        isGeneratedEventVal:bool ->
          cenv:TcFileState ->
            env:TcEnv ->
              tpenv:UnscopedTyparEnv * recBindIdx:int ->
                NormalizedRecBindingDefn ->
                  (PreCheckingRecursiveBinding list * TypedTree.Val list) *
                  (UnscopedTyparEnv * int)
    val AnalyzeAndMakeAndPublishRecursiveValues:
      overridesOK:OverridesOK ->
        cenv:TcFileState ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              binds:NormalizedRecBindingDefn list ->
                PreCheckingRecursiveBinding list * TypedTree.Val list *
                (UnscopedTyparEnv * int)
    val TcLetrecBinding:
      cenv:TcFileState * envRec:TcEnv * scopem:Range.range *
      extraGeneralizableTypars:TypedTree.Typars *
      reqdThisValTyOpt:TypedTree.TType option ->
        envNonRec:TcEnv *
        generalizedRecBinds:PostGeneralizationRecursiveBinding list *
        preGeneralizationRecBinds:PreGeneralizationRecursiveBinding list *
        tpenv:UnscopedTyparEnv *
        uncheckedRecBindsTable:Map<TypedTree.Stamp,PreCheckingRecursiveBinding> ->
          rbind:PreCheckingRecursiveBinding ->
            TcEnv * PostGeneralizationRecursiveBinding list *
            PreGeneralizationRecursiveBinding list * UnscopedTyparEnv *
            Map<TypedTree.Stamp,PreCheckingRecursiveBinding>
    val TcIncrementalLetRecGeneralization:
      cenv:TcFileState ->
        scopem:Range.range ->
          envNonRec:TcEnv *
          generalizedRecBinds:PostGeneralizationRecursiveBinding list *
          preGeneralizationRecBinds:PreGeneralizationRecursiveBinding list *
          tpenv:UnscopedTyparEnv *
          uncheckedRecBindsTable:Map<TypedTree.Stamp,PreCheckingRecursiveBinding> ->
            TcEnv * PostGeneralizationRecursiveBinding list *
            PreGeneralizationRecursiveBinding list * UnscopedTyparEnv *
            Map<TypedTree.Stamp,PreCheckingRecursiveBinding>
    val TcLetrecComputeAndGeneralizeGenericTyparsForBinding:
      cenv:TcFileState ->
        denv:TypedTreeOps.DisplayEnv ->
          freeInEnv:Internal.Utilities.Collections.Tagged.Set<TypedTree.Typar,
                                                              System.Collections.Generic.IComparer<TypedTree.Typar>> ->
            pgrbind:PreGeneralizationRecursiveBinding -> TypedTree.Typar list
    val TcLetrecComputeSupportForBinding:
      cenv:TcFileState ->
        pgrbind:PreGeneralizationRecursiveBinding -> TypedTree.Typar list
    val TcLetrecGeneralizeBinding:
      cenv:TcFileState ->
        denv:TypedTreeOps.DisplayEnv ->
          generalizedTypars:TypedTree.Typar list ->
            pgrbind:PreGeneralizationRecursiveBinding ->
              PostGeneralizationRecursiveBinding
    val TcLetrecComputeCtorSafeThisValBind:
      cenv:TcFileState ->
        safeThisValOpt:TypedTree.Val option -> TypedTree.Binding option
    val MakeCheckSafeInitField:
      g:TcGlobals.TcGlobals ->
        tinst:TypedTree.TypeInst ->
          thisValOpt:TypedTree.Val option ->
            rfref:TypedTree.RecdFieldRef ->
              reqExpr:TypedTree.Expr -> expr:TypedTree.Expr -> TypedTree.Expr
    val MakeCheckSafeInit:
      g:TcGlobals.TcGlobals ->
        tinst:TypedTree.TypeInst ->
          safeInitInfo:SafeInitData ->
            reqExpr:TypedTree.Expr -> expr:TypedTree.Expr -> TypedTree.Expr
    val TcLetrecAdjustMemberForSpecialVals:
      cenv:TcFileState ->
        pgrbind:PostGeneralizationRecursiveBinding ->
          PostSpecialValsRecursiveBinding
    val FixupLetrecBind:
      cenv:TcFileState ->
        denv:TypedTreeOps.DisplayEnv ->
          generalizedTyparsForRecursiveBlock:TypedTree.Typars ->
            bind:PostSpecialValsRecursiveBinding ->
              PreInitializationGraphEliminationBinding
    val unionGeneralizedTypars:
      typarSets:TypedTree.Typar list list -> TypedTree.Typar list
    val TcLetrec:
      overridesOK:OverridesOK ->
        cenv:TcFileState ->
          env:TcEnv ->
            tpenv:UnscopedTyparEnv ->
              binds:RecDefnBindingInfo list * bindsm:Range.range *
              scopem:Range.range ->
                TypedTree.Bindings * TcEnv * UnscopedTyparEnv
    val TcAndPublishValSpec:
      cenv:TcFileState * env:TcEnv * containerInfo:ContainerInfo *
      declKind:DeclKind * memFlagsOpt:SyntaxTree.MemberFlags option *
      tpenv:UnscopedTyparEnv * valSpfn:SyntaxTree.SynValSig ->
        TypedTree.Val list * UnscopedTyparEnv


namespace FSharp.Compiler
  module internal CheckComputationExpressions =
    type cenv = CheckExpressions.TcFileState
    type CompExprTranslationPass =
      | Initial
      | Subsequent
    type CustomOperationsMode =
      | Allowed
      | Denied
    val TryFindIntrinsicOrExtensionMethInfo:
      collectionSettings:NameResolution.ResultCollectionSettings ->
        cenv:cenv ->
          env:CheckExpressions.TcEnv ->
            m:Range.range ->
              ad:AccessibilityLogic.AccessorDomain ->
                nm:string -> ty:TypedTree.TType -> Infos.MethInfo list
    val IgnoreAttribute: 'a -> 'b option
    val ( |ExprAsPat|_| ): f:SyntaxTree.SynExpr -> SyntaxTree.SynPat option
    val ( |JoinRelation|_| ):
      cenv:CheckExpressions.TcFileState ->
        env:CheckExpressions.TcEnv ->
          e:SyntaxTree.SynExpr ->
            (SyntaxTree.SynExpr * SyntaxTree.SynExpr) option
    val elimFastIntegerForLoop:
      spBind:SyntaxTree.DebugPointAtFor * id:SyntaxTree.Ident *
      start:SyntaxTree.SynExpr * dir:bool * finish:SyntaxTree.SynExpr *
      innerExpr:SyntaxTree.SynExpr * m:Range.range -> SyntaxTree.SynExpr
    val YieldFree: cenv:cenv -> expr:SyntaxTree.SynExpr -> bool
    val ( |SimpleSemicolonSequence|_| ):
      cenv:cenv ->
        acceptDeprecated:bool ->
          cexpr:SyntaxTree.SynExpr -> SyntaxTree.SynExpr list option
    val RecordNameAndTypeResolutions_IdeallyWithoutHavingOtherEffects:
      cenv:CheckExpressions.TcFileState ->
        env:CheckExpressions.TcEnv ->
          tpenv:CheckExpressions.UnscopedTyparEnv ->
            expr:SyntaxTree.SynExpr -> unit
    val TcComputationExpression:
      cenv:CheckExpressions.TcFileState ->
        env:CheckExpressions.TcEnv ->
          overallTy:TypedTree.TType ->
            tpenv:CheckExpressions.UnscopedTyparEnv ->
              mWhole:Range.range * interpExpr:TypedTree.Expr *
              builderTy:TypedTree.TType * comp:SyntaxTree.SynExpr ->
                TypedTree.Expr * CheckExpressions.UnscopedTyparEnv
    val mkSeqEmpty:
      cenv:cenv ->
        env:CheckExpressions.TcEnv ->
          m:Range.range -> genTy:TypedTree.TType -> TypedTree.Expr
    val mkSeqCollect:
      cenv:cenv ->
        env:CheckExpressions.TcEnv ->
          m:Range.range ->
            enumElemTy:TypedTree.TType ->
              genTy:TypedTree.TType ->
                lam:TypedTree.Expr -> enumExpr:TypedTree.Expr -> TypedTree.Expr
    val mkSeqUsing:
      cenv:cenv ->
        env:CheckExpressions.TcEnv ->
          m:Range.range ->
            resourceTy:TypedTree.TType ->
              genTy:TypedTree.TType ->
                resourceExpr:TypedTree.Expr ->
                  lam:TypedTree.Expr -> TypedTree.Expr
    val mkSeqDelay:
      cenv:cenv ->
        env:CheckExpressions.TcEnv ->
          m:Range.range ->
            genTy:TypedTree.TType -> lam:TypedTree.Expr -> TypedTree.Expr
    val mkSeqAppend:
      cenv:cenv ->
        env:CheckExpressions.TcEnv ->
          m:Range.range ->
            genTy:TypedTree.TType ->
              e1:TypedTree.Expr -> e2:TypedTree.Expr -> TypedTree.Expr
    val mkSeqFromFunctions:
      cenv:cenv ->
        env:CheckExpressions.TcEnv ->
          m:Range.range ->
            genTy:TypedTree.TType ->
              e1:TypedTree.Expr -> e2:TypedTree.Expr -> TypedTree.Expr
    val mkSeqFinally:
      cenv:cenv ->
        env:CheckExpressions.TcEnv ->
          m:Range.range ->
            genTy:TypedTree.TType ->
              e1:TypedTree.Expr -> e2:TypedTree.Expr -> TypedTree.Expr
    val mkSeqExprMatchClauses:
      pat':PatternMatchCompilation.Pattern * vspecs:TypedTree.Val list ->
        innerExpr:TypedTree.Expr ->
          PatternMatchCompilation.TypedMatchClause list
    val compileSeqExprMatchClauses:
      cenv:cenv ->
        env:CheckExpressions.TcEnv ->
          inputExprMark:Range.range ->
            pat:PatternMatchCompilation.Pattern * vspecs:TypedTree.Val list ->
              innerExpr:TypedTree.Expr ->
                inputExprOpt:TypedTree.Expr option ->
                  bindPatTy:TypedTree.TType ->
                    genInnerTy:TypedTree.TType -> TypedTree.Val * TypedTree.Expr
    val TcSequenceExpression:
      cenv:cenv ->
        env:CheckExpressions.TcEnv ->
          tpenv:CheckExpressions.UnscopedTyparEnv ->
            comp:SyntaxTree.SynExpr ->
              overallTy:TypedTree.TType ->
                m:Range.range ->
                  TypedTree.Expr * CheckExpressions.UnscopedTyparEnv
    val TcSequenceExpressionEntry:
      cenv:cenv ->
        env:CheckExpressions.TcEnv ->
          overallTy:TypedTree.TType ->
            tpenv:CheckExpressions.UnscopedTyparEnv ->
              isArrayOrList:bool * isNotNakedRefCell:bool ref *
              comp:SyntaxTree.SynExpr ->
                m:Range.range ->
                  TypedTree.Expr * CheckExpressions.UnscopedTyparEnv
    val TcArrayOrListSequenceExpression:
      cenv:cenv ->
        env:CheckExpressions.TcEnv ->
          overallTy:TypedTree.TType ->
            tpenv:CheckExpressions.UnscopedTyparEnv ->
              isArray:bool * comp:SyntaxTree.SynExpr ->
                m:Range.range ->
                  TypedTree.Expr * CheckExpressions.UnscopedTyparEnv


namespace FSharp.Compiler
  module internal CheckDeclarations =
    type cenv = CheckExpressions.TcFileState
    type MutRecDataForOpen =
      | MutRecDataForOpen of
        SyntaxTree.SynOpenDeclTarget * Range.range * appliedScope: Range.range
    type MutRecDataForModuleAbbrev =
      | MutRecDataForModuleAbbrev of
        SyntaxTree.Ident * SyntaxTree.LongIdent * Range.range
    [<RequireQualifiedAccessAttribute>]
    type MutRecShape<'TypeData,'LetsData,'ModuleData> =
      | Tycon of 'TypeData
      | Lets of 'LetsData
      | Module of 'ModuleData * MutRecShapes<'TypeData,'LetsData,'ModuleData>
      | ModuleAbbrev of MutRecDataForModuleAbbrev
      | Open of MutRecDataForOpen
    and MutRecShapes<'TypeData,'LetsData,'ModuleData> =
      MutRecShape<'TypeData,'LetsData,'ModuleData> list
    module MutRecShapes =
      val map:
        f1:('a -> 'b) ->
          f2:('c -> 'd) ->
            f3:('e -> 'f) ->
              x:MutRecShape<'a,'c,'e> list -> MutRecShape<'b,'d,'f> list
      val mapTycons:
        f1:('a -> 'b) ->
          xs:MutRecShape<'a,'c,'d> list -> MutRecShape<'b,'c,'d> list
      val mapTyconsAndLets:
        f1:('a -> 'b) ->
          f2:('c -> 'd) ->
            xs:MutRecShape<'a,'c,'e> list -> MutRecShape<'b,'d,'e> list
      val mapLets:
        f2:('a -> 'b) ->
          xs:MutRecShape<'c,'a,'d> list -> MutRecShape<'c,'b,'d> list
      val mapModules:
        f1:('a -> 'b) ->
          xs:MutRecShape<'c,'d,'a> list -> MutRecShape<'c,'d,'b> list
      val mapWithEnv:
        fTycon:('Env -> 'a -> 'b) ->
          fLets:('Env -> 'c -> 'd) ->
            env:'Env ->
              x:MutRecShape<'a,'c,('e * 'Env)> list ->
                MutRecShape<'b,'d,('e * 'Env)> list
      val mapTyconsWithEnv:
        f1:('a -> 'b -> 'c) ->
          env:'a ->
            xs:MutRecShape<'b,'d,('e * 'a)> list ->
              MutRecShape<'c,'d,('e * 'a)> list
      val mapWithParent:
        parent:'a ->
          f1:('a -> 'b -> MutRecShapes<'c,'d,'b> -> 'e * 'a) ->
            f2:('a -> 'c -> 'f) ->
              f3:('a -> 'd -> 'g) ->
                xs:MutRecShape<'c,'d,'b> list -> MutRecShape<'f,'g,'e> list
      val computeEnvs:
        f1:('a -> 'b -> 'Env) ->
          f2:('Env -> MutRecShape<'c,'d,'b> list -> 'a) ->
            env:'Env ->
              xs:MutRecShape<'c,'d,'b> list ->
                'a * MutRecShape<'c,'d,('b * 'a)> list
      val extendEnvs:
        f1:('Env -> MutRecShape<'a,'b,('c * 'Env)> list -> 'Env) ->
          env:'Env ->
            xs:MutRecShape<'a,'b,('c * 'Env)> list ->
              'Env * MutRecShape<'a,'b,('c * 'Env)> list
      val dropEnvs:
        xs:MutRecShape<'a,'b,('c * 'd)> list -> MutRecShape<'a,'b,'c> list
      val expandTyconsWithEnv:
        f1:('a -> 'b -> 'c list * 'c list) ->
          env:'a ->
            xs:MutRecShape<'b,'c list,('d * 'a)> list ->
              MutRecShape<'b,'c list,('d * 'a)> list
      val mapFoldWithEnv:
        f1:('a -> 'b -> MutRecShape<'c,'d,('e * 'b)> ->
              MutRecShape<'f,'g,('e * 'b)> * 'a) ->
          z:'a ->
            env:'b ->
              xs:MutRecShape<'c,'d,('e * 'b)> list ->
                MutRecShape<'f,'g,('e * 'b)> list * 'a
      val collectTycons: x:MutRecShape<'a,'b,'c> list -> 'a list
      val topTycons: x:MutRecShape<'a,'b,'c> list -> 'a list
      val iter:
        f1:('a -> unit) ->
          f2:('b -> unit) ->
            f3:('c -> unit) ->
              f4:(MutRecDataForOpen -> unit) ->
                f5:(MutRecDataForModuleAbbrev -> unit) ->
                  x:MutRecShape<'a,'b,'c> list -> unit
      val iterTycons: f1:('a -> unit) -> x:MutRecShape<'a,'b,'c> list -> unit
      val iterTyconsAndLets:
        f1:('a -> unit) ->
          f2:('b -> unit) -> x:MutRecShape<'a,'b,'c> list -> unit
      val iterModules: f1:('a -> unit) -> x:MutRecShape<'b,'c,'a> list -> unit
      val iterWithEnv:
        f1:('a -> 'b -> unit) ->
          f2:('a -> 'c -> unit) ->
            f3:('a -> MutRecDataForOpen -> unit) ->
              f4:('a -> MutRecDataForModuleAbbrev -> unit) ->
                env:'a -> x:MutRecShape<'b,'c,('d * 'a)> list -> unit
      val iterTyconsWithEnv:
        f1:('a -> 'b -> unit) ->
          env:'a -> xs:MutRecShape<'b,'c,('d * 'a)> list -> unit
  
    val ModuleOrNamespaceContainerInfo:
      modref:TypedTree.EntityRef -> CheckExpressions.ContainerInfo
    val TyconContainerInfo:
      parent:TypedTree.ParentRef * tcref:TypedTree.TyconRef *
      declaredTyconTypars:TypedTree.Typars *
      safeInitInfo:CheckExpressions.SafeInitData ->
        CheckExpressions.ContainerInfo
    type TyconBindingDefn =
      | TyconBindingDefn of
        CheckExpressions.ContainerInfo * CheckExpressions.NewSlotsOK *
        CheckExpressions.DeclKind * SyntaxTree.SynMemberDefn * Range.range
    type MutRecSigsInitialData =
      MutRecShape<SyntaxTree.SynTypeDefnSig,SyntaxTree.SynValSig,
                  SyntaxTree.SynComponentInfo> list
    type MutRecDefnsInitialData =
      MutRecShape<SyntaxTree.SynTypeDefn,SyntaxTree.SynBinding list,
                  SyntaxTree.SynComponentInfo> list
    type MutRecDefnsPhase1DataForTycon =
      | MutRecDefnsPhase1DataForTycon of
        SyntaxTree.SynComponentInfo * SyntaxTree.SynTypeDefnSimpleRepr *
        (SyntaxTree.SynType * Range.range) list *
        preEstablishedHasDefaultCtor: bool * hasSelfReferentialCtor: bool *
        isAtOriginalTyconDefn: bool
    type MutRecDefnsPhase1Data =
      MutRecShape<(MutRecDefnsPhase1DataForTycon * SyntaxTree.SynMemberDefn list),
                  CheckExpressions.RecDefnBindingInfo list,
                  SyntaxTree.SynComponentInfo> list
    type MutRecDefnsPhase2DataForTycon =
      | MutRecDefnsPhase2DataForTycon of
        TypedTree.Tycon option * TypedTree.ParentRef * CheckExpressions.DeclKind *
        TypedTree.TyconRef * TypedTree.Val option *
        CheckExpressions.SafeInitData * TypedTree.Typars *
        SyntaxTree.SynMemberDefn list * Range.range *
        CheckExpressions.NewSlotsOK * fixupFinalAttribs: unit -> unit
    type MutRecDefnsPhase2DataForModule =
      | MutRecDefnsPhase2DataForModule of
        TypedTree.ModuleOrNamespaceType ref * TypedTree.ModuleOrNamespace
    type MutRecDefnsPhase2Data =
      MutRecShape<MutRecDefnsPhase2DataForTycon,
                  CheckExpressions.RecDefnBindingInfo list,
                  (MutRecDefnsPhase2DataForModule * CheckExpressions.TcEnv)> list
    type MutRecDefnsPhase2InfoForTycon =
      | MutRecDefnsPhase2InfoForTycon of
        TypedTree.Tycon option * TypedTree.TyconRef * TypedTree.Typars *
        CheckExpressions.DeclKind * TyconBindingDefn list *
        fixupFinalAttrs: unit -> unit
    type MutRecDefnsPhase2Info =
      MutRecShape<MutRecDefnsPhase2InfoForTycon,
                  CheckExpressions.RecDefnBindingInfo list,
                  (MutRecDefnsPhase2DataForModule * CheckExpressions.TcEnv)> list
    val AddLocalExnDefnAndReport:
      tcSink:NameResolution.TcResultsSink ->
        scopem:Range.range ->
          env:CheckExpressions.TcEnv ->
            exnc:TypedTree.Tycon -> CheckExpressions.TcEnv
    val AddLocalTyconRefs:
      ownDefinition:bool ->
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap ->
            m:Range.range ->
              tcrefs:TypedTree.TyconRef list ->
                env:CheckExpressions.TcEnv -> CheckExpressions.TcEnv
    val AddLocalTycons:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          m:Range.range ->
            tycons:TypedTree.Tycon list ->
              env:CheckExpressions.TcEnv -> CheckExpressions.TcEnv
    val AddLocalTyconsAndReport:
      tcSink:NameResolution.TcResultsSink ->
        scopem:Range.range ->
          g:TcGlobals.TcGlobals ->
            amap:Import.ImportMap ->
              m:Range.range ->
                tycons:TypedTree.Tycon list ->
                  env:CheckExpressions.TcEnv -> CheckExpressions.TcEnv
    val AddLocalSubModule:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          m:Range.range ->
            env:CheckExpressions.TcEnv ->
              modul:TypedTree.ModuleOrNamespace -> CheckExpressions.TcEnv
    val AddLocalSubModuleAndReport:
      tcSink:NameResolution.TcResultsSink ->
        scopem:Range.range ->
          g:TcGlobals.TcGlobals ->
            amap:Import.ImportMap ->
              m:Range.range ->
                env:CheckExpressions.TcEnv ->
                  modul:TypedTree.ModuleOrNamespace -> CheckExpressions.TcEnv
    val BuildRootModuleType:
      enclosingNamespacePath:SyntaxTree.Ident list ->
        cpath:TypedTree.CompilationPath ->
          mtyp:TypedTree.ModuleOrNamespaceType ->
            TypedTree.ModuleOrNamespaceType * TypedTree.ModuleOrNamespace list
    val BuildRootModuleExpr:
      enclosingNamespacePath:SyntaxTree.Ident list ->
        cpath:TypedTree.CompilationPath ->
          mexpr:TypedTree.ModuleOrNamespaceExpr ->
            TypedTree.ModuleOrNamespaceExpr
    val TryStripPrefixPath:
      g:TcGlobals.TcGlobals ->
        enclosingNamespacePath:SyntaxTree.Ident list ->
          (SyntaxTree.Ident * SyntaxTree.Ident list) option
    val AddModuleAbbreviationAndReport:
      tcSink:NameResolution.TcResultsSink ->
        scopem:Range.range ->
          id:SyntaxTree.Ident ->
            modrefs:TypedTree.ModuleOrNamespaceRef list ->
              env:CheckExpressions.TcEnv -> CheckExpressions.TcEnv
    val OpenModuleOrNamespaceRefs:
      tcSink:NameResolution.TcResultsSink ->
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap ->
            scopem:Range.range ->
              root:bool ->
                env:CheckExpressions.TcEnv ->
                  mvvs:TypedTree.ModuleOrNamespaceRef list ->
                    openDeclaration:NameResolution.OpenDeclaration ->
                      CheckExpressions.TcEnv
    val OpenTypeContent:
      tcSink:NameResolution.TcResultsSink ->
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap ->
            scopem:Range.range ->
              env:CheckExpressions.TcEnv ->
                typ:TypedTree.TType ->
                  openDeclaration:NameResolution.OpenDeclaration ->
                    CheckExpressions.TcEnv
    val AddRootModuleOrNamespaceRefs:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          m:Range.range ->
            env:CheckExpressions.TcEnv ->
              modrefs:TypedTree.ModuleOrNamespaceRef list ->
                CheckExpressions.TcEnv
    val addInternalsAccessibility:
      env:CheckExpressions.TcEnv ->
        ccu:TypedTree.CcuThunk -> CheckExpressions.TcEnv
    val AddNonLocalCcu:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          scopem:Range.range ->
            env:CheckExpressions.TcEnv ->
              assemblyName:string ->
                ccu:TypedTree.CcuThunk *
                internalsVisibleToAttributes:string list ->
                  CheckExpressions.TcEnv
    val AddLocalRootModuleOrNamespace:
      NameResolution.TcResultsSink ->
        TcGlobals.TcGlobals ->
          Import.ImportMap ->
            Range.range ->
              CheckExpressions.TcEnv ->
                TypedTree.ModuleOrNamespaceType -> CheckExpressions.TcEnv
    val ImplicitlyOpenOwnNamespace:
      tcSink:NameResolution.TcResultsSink ->
        g:TcGlobals.TcGlobals ->
          amap:Import.ImportMap ->
            scopem:Range.range ->
              enclosingNamespacePath:SyntaxTree.Ident list ->
                env:CheckExpressions.TcEnv -> CheckExpressions.TcEnv
    exception NotUpperCaseConstructor of Range.range
    val CheckNamespaceModuleOrTypeName:
      g:TcGlobals.TcGlobals -> id:SyntaxTree.Ident -> unit
    val CheckDuplicates:
      idf:('a -> SyntaxTree.Ident) -> k:string -> elems:'a list -> 'a list
    module TcRecdUnionAndEnumDeclarations =
      val CombineReprAccess:
        parent:TypedTree.ParentRef ->
          vis:TypedTree.Accessibility -> TypedTree.Accessibility
      val MakeRecdFieldSpec:
        _cenv:'a ->
          env:CheckExpressions.TcEnv ->
            parent:TypedTree.ParentRef ->
              isStatic:bool * konst:TypedTree.Const option * ty':TypedTree.TType *
              attrsForProperty:TypedTree.Attribs *
              attrsForField:TypedTree.Attribs * id:SyntaxTree.Ident *
              nameGenerated:bool * isMutable:bool * vol:bool *
              xmldoc:XmlDoc.XmlDoc * vis:SyntaxTree.SynAccess option *
              m:Range.range -> TypedTree.RecdField
      val TcFieldDecl:
        cenv:CheckExpressions.TcFileState ->
          env:CheckExpressions.TcEnv ->
            parent:TypedTree.ParentRef ->
              isIncrClass:bool ->
                tpenv:CheckExpressions.UnscopedTyparEnv ->
                  isStatic:bool * synAttrs:SyntaxTree.SynAttribute list *
                  id:SyntaxTree.Ident * nameGenerated:bool *
                  ty:SyntaxTree.SynType * isMutable:bool * xmldoc:XmlDoc.XmlDoc *
                  vis:SyntaxTree.SynAccess option * m:Range.range ->
                    TypedTree.RecdField
      val TcAnonFieldDecl:
        cenv:CheckExpressions.TcFileState ->
          env:CheckExpressions.TcEnv ->
            parent:TypedTree.ParentRef ->
              tpenv:CheckExpressions.UnscopedTyparEnv ->
                nm:string -> SyntaxTree.SynField -> TypedTree.RecdField
      val TcNamedFieldDecl:
        cenv:CheckExpressions.TcFileState ->
          env:CheckExpressions.TcEnv ->
            parent:TypedTree.ParentRef ->
              isIncrClass:bool ->
                tpenv:CheckExpressions.UnscopedTyparEnv ->
                  SyntaxTree.SynField -> TypedTree.RecdField
      val TcNamedFieldDecls:
        cenv:CheckExpressions.TcFileState ->
          env:CheckExpressions.TcEnv ->
            parent:TypedTree.ParentRef ->
              isIncrClass:bool ->
                tpenv:CheckExpressions.UnscopedTyparEnv ->
                  fields:SyntaxTree.SynField list -> TypedTree.RecdField list
      val CheckUnionCaseName: cenv:cenv -> id:SyntaxTree.Ident -> unit
      val ValidateFieldNames:
        synFields:SyntaxTree.SynField list * tastFields:TypedTree.RecdField list ->
          unit
      val TcUnionCaseDecl:
        cenv:CheckExpressions.TcFileState ->
          env:CheckExpressions.TcEnv ->
            parent:TypedTree.ParentRef ->
              thisTy:TypedTree.TType ->
                thisTyInst:TypedTree.TypeInst ->
                  tpenv:CheckExpressions.UnscopedTyparEnv ->
                    SyntaxTree.SynUnionCase -> TypedTree.UnionCase
      val TcUnionCaseDecls:
        cenv:CheckExpressions.TcFileState ->
          env:CheckExpressions.TcEnv ->
            parent:TypedTree.ParentRef ->
              thisTy:TypedTree.TType ->
                thisTyInst:TypedTree.TypeInst ->
                  tpenv:CheckExpressions.UnscopedTyparEnv ->
                    unionCases:SyntaxTree.SynUnionCase list ->
                      TypedTree.UnionCase list
      val TcEnumDecl:
        cenv:CheckExpressions.TcFileState ->
          env:CheckExpressions.TcEnv ->
            parent:TypedTree.ParentRef ->
              thisTy:TypedTree.TType ->
                fieldTy:TypedTree.TType ->
                  SyntaxTree.SynEnumCase -> TypedTree.RecdField
      val TcEnumDecls:
        cenv:CheckExpressions.TcFileState ->
          env:CheckExpressions.TcEnv ->
            parent:TypedTree.ParentRef ->
              thisTy:TypedTree.TType ->
                enumCases:SyntaxTree.SynEnumCase list ->
                  TypedTree.TType * TypedTree.RecdField list
  
    val PublishInterface:
      cenv:cenv ->
        denv:TypedTreeOps.DisplayEnv ->
          tcref:TypedTree.TyconRef ->
            m:Range.range -> compgen:bool -> ty':TypedTree.TType -> unit
    val TcAndPublishMemberSpec:
      cenv:CheckExpressions.TcFileState ->
        env:CheckExpressions.TcEnv ->
          containerInfo:CheckExpressions.ContainerInfo ->
            declKind:CheckExpressions.DeclKind ->
              tpenv:CheckExpressions.UnscopedTyparEnv ->
                memb:SyntaxTree.SynMemberSig ->
                  TypedTree.Val list * CheckExpressions.UnscopedTyparEnv
    val TcTyconMemberSpecs:
      cenv:CheckExpressions.TcFileState ->
        env:CheckExpressions.TcEnv ->
          containerInfo:CheckExpressions.ContainerInfo ->
            declKind:CheckExpressions.DeclKind ->
              tpenv:CheckExpressions.UnscopedTyparEnv ->
                augSpfn:SyntaxTree.SynMemberSig list ->
                  TypedTree.Val list * CheckExpressions.UnscopedTyparEnv
    val TcOpenLidAndPermitAutoResolve:
      tcSink:NameResolution.TcResultsSink ->
        env:CheckExpressions.TcEnv ->
          amap:Import.ImportMap ->
            longId:SyntaxTree.Ident list ->
              (int * TypedTree.ModuleOrNamespaceRef *
               TypedTree.ModuleOrNamespaceType) list
    val TcOpenModuleOrNamespaceDecl:
      NameResolution.TcResultsSink ->
        TcGlobals.TcGlobals ->
          Import.ImportMap ->
            Range.range ->
              CheckExpressions.TcEnv ->
                (SyntaxTree.Ident list * Range.range) -> CheckExpressions.TcEnv
    val TcOpenTypeDecl:
      cenv:cenv ->
        mOpenDecl:Range.range ->
          scopem:Range.range ->
            env:CheckExpressions.TcEnv ->
              synType:SyntaxTree.SynType * m:Range.range ->
                CheckExpressions.TcEnv
    val TcOpenDecl:
      cenv:CheckExpressions.TcFileState ->
        mOpenDecl:Range.range ->
          scopem:Range.range ->
            env:CheckExpressions.TcEnv ->
              target:SyntaxTree.SynOpenDeclTarget -> CheckExpressions.TcEnv
    exception ParameterlessStructCtor of Range.range
    val MakeSafeInitField:
      g:TcGlobals.TcGlobals ->
        env:CheckExpressions.TcEnv ->
          m:Range.range -> isStatic:bool -> TypedTree.RecdField
    module IncrClassChecking =
      type IncrClassBindingGroup =
        | IncrClassBindingGroup of TypedTree.Binding list * bool * bool
        | IncrClassDo of TypedTree.Expr * bool
      type IncrClassCtorLhs =
        { TyconRef: TypedTree.TyconRef
          InstanceCtorDeclaredTypars: TypedTree.Typars
          StaticCtorValInfo:
            System.Lazy<TypedTree.Val list * TypedTree.Val *
                        CheckExpressions.ValScheme>
          InstanceCtorVal: TypedTree.Val
          InstanceCtorValScheme: CheckExpressions.ValScheme
          InstanceCtorArgs: TypedTree.Val list
          InstanceCtorSafeThisValOpt: TypedTree.Val option
          InstanceCtorSafeInitInfo: CheckExpressions.SafeInitData
          InstanceCtorBaseValOpt: TypedTree.Val option
          InstanceCtorThisVal: TypedTree.Val
          NameGenerator: CompilerGlobalState.NiceNameGenerator }
        with
          member
            GetNormalizedInstanceCtorDeclaredTypars: cenv:cenv ->
                                                        denv:TypedTreeOps.DisplayEnv ->
                                                          m:Range.range ->
                                                            TypedTree.Typar list
      
      val TcImplicitCtorLhs_Phase2A:
        cenv:cenv * env:CheckExpressions.TcEnv *
        tpenv:CheckExpressions.UnscopedTyparEnv * tcref:TypedTree.TyconRef *
        vis:SyntaxTree.SynAccess option * attrs:SyntaxTree.SynAttribute list *
        spats:SyntaxTree.SynSimplePat list * thisIdOpt:SyntaxTree.Ident option *
        baseValOpt:TypedTree.Val option *
        safeInitInfo:CheckExpressions.SafeInitData * m:Range.range *
        copyOfTyconTypars:TypedTree.Typar list * objTy:TypedTree.TType *
        thisTy:TypedTree.TType * doc:XmlDoc.PreXmlDoc -> IncrClassCtorLhs
      val private MakeIncrClassField:
        g:TcGlobals.TcGlobals * cpath:TypedTree.CompilationPath *
        formalTyparInst:TypedTreeOps.TyparInst * v:TypedTree.Val * isStatic:bool *
        rfref:TypedTree.RecdFieldRef -> TypedTree.RecdField
      type IncrClassValRepr =
        | InVar of bool
        | InField of bool * int * TypedTree.RecdFieldRef
        | InMethod of bool * TypedTree.Val * TypedTree.ValReprInfo
      type IncrClassReprInfo =
        { TakenFieldNames: Set<string>
          RepInfoTcGlobals: TcGlobals.TcGlobals
          ValReprs: AbstractIL.Internal.Zmap<TypedTree.Val,IncrClassValRepr>
          ValsWithRepresentation: AbstractIL.Internal.Zset<TypedTree.Val> }
        with
          static member
            Empty: g:TcGlobals.TcGlobals * names:string list ->
                      IncrClassReprInfo
          static member
            IsMethodRepr: cenv:cenv -> bind:TypedTree.Binding -> bool
          member
            ChooseAndAddRepresentation: cenv:cenv * env:CheckExpressions.TcEnv *
                                         isStatic:bool * isCtorArg:bool *
                                         ctorInfo:IncrClassCtorLhs *
                                         staticForcedFieldVars:TypedTree.FreeLocals *
                                         instanceForcedFieldVars:TypedTree.FreeLocals *
                                         bind:TypedTree.Binding ->
                                           IncrClassReprInfo
          member
            ChooseRepresentation: cenv:cenv * env:CheckExpressions.TcEnv *
                                   isStatic:bool * isCtorArg:bool *
                                   ctorInfo:IncrClassCtorLhs *
                                   staticForcedFieldVars:TypedTree.FreeLocals *
                                   instanceForcedFieldVars:TypedTree.FreeLocals *
                                   takenFieldNames:Set<string> *
                                   bind:TypedTree.Binding ->
                                     IncrClassValRepr * Set<string>
          member
            FixupIncrClassExprPhase2C: cenv:CheckExpressions.TcFileState ->
                                          thisValOpt:TypedTree.Val option ->
                                            safeStaticInitInfo:CheckExpressions.SafeInitData ->
                                              thisTyInst:TypedTree.TypeInst ->
                                                expr:TypedTree.Expr ->
                                                  TypedTree.Expr
          member IsValRepresentedAsLocalVar: v:TypedTree.Val -> bool
          member IsValRepresentedAsMethod: v:TypedTree.Val -> bool
          member IsValWithRepresentation: v:TypedTree.Val -> bool
          member LookupRepr: v:TypedTree.Val -> IncrClassValRepr
          member
            MakeValueAssign: thisValOpt:TypedTree.Val option ->
                                tinst:TypedTree.TypeInst ->
                                  safeStaticInitInfo:CheckExpressions.SafeInitData ->
                                    v:TypedTree.Val ->
                                      expr:TypedTree.Expr ->
                                        m:Range.range -> TypedTree.Expr
          member
            MakeValueGetAddress: readonly:bool ->
                                    thisValOpt:TypedTree.Val option ->
                                      tinst:TypedTree.TypeInst ->
                                        safeStaticInitInfo:CheckExpressions.SafeInitData ->
                                          v:TypedTree.Val ->
                                            m:Range.range -> TypedTree.Expr
          member
            MakeValueLookup: thisValOpt:TypedTree.Val option ->
                                tinst:TypedTree.TypeInst ->
                                  safeStaticInitInfo:CheckExpressions.SafeInitData ->
                                    v:TypedTree.Val ->
                                      tyargs:TypedTree.TType list ->
                                        m:Range.range -> TypedTree.Expr
          member
            PublishIncrClassFields: cenv:cenv * denv:TypedTreeOps.DisplayEnv *
                                     cpath:TypedTree.CompilationPath *
                                     ctorInfo:IncrClassCtorLhs *
                                     safeStaticInitInfo:CheckExpressions.SafeInitData ->
                                       unit
          member ValNowWithRepresentation: v:TypedTree.Val -> IncrClassReprInfo
      
      type IncrClassConstructionBindingsPhase2C =
        | Phase2CBindings of IncrClassBindingGroup list
        | Phase2CCtorJustAfterSuperInit
        | Phase2CCtorJustAfterLastLet
      val MakeCtorForIncrClassConstructionPhase2C:
        cenv:cenv * env:CheckExpressions.TcEnv * ctorInfo:IncrClassCtorLhs *
        inheritsExpr:TypedTree.Expr * inheritsIsVisible:bool *
        decs:IncrClassConstructionBindingsPhase2C list *
        memberBinds:TypedTree.Binding list *
        generalizedTyparsForRecursiveBlock:TypedTree.Typar list *
        safeStaticInitInfo:CheckExpressions.SafeInitData ->
          TypedTree.Expr * TypedTree.Expr option * TypedTree.Binding list *
          IncrClassReprInfo
  
    module MutRecBindingChecking =
      type TyconBindingPhase2A =
        | Phase2AIncrClassCtor of IncrClassChecking.IncrClassCtorLhs
        | Phase2AInherit of
          SyntaxTree.SynType * SyntaxTree.SynExpr * TypedTree.Val option *
          Range.range
        | Phase2AIncrClassBindings of
          TypedTree.TyconRef * SyntaxTree.SynBinding list * bool * bool *
          Range.range
        | Phase2AMember of CheckExpressions.PreCheckingRecursiveBinding
        | Phase2AIncrClassCtorJustAfterSuperInit
        | Phase2AIncrClassCtorJustAfterLastLet
      type TyconBindingsPhase2A =
        | TyconBindingsPhase2A of
          TypedTree.Tycon option * CheckExpressions.DeclKind *
          TypedTree.Val list * TypedTree.TyconRef * TypedTree.Typar list *
          TypedTree.TType * TyconBindingPhase2A list
      type MutRecDefnsPhase2AData =
        MutRecShape<TyconBindingsPhase2A,
                    CheckExpressions.PreCheckingRecursiveBinding list,
                    (MutRecDefnsPhase2DataForModule * CheckExpressions.TcEnv)> list
      type TyconBindingPhase2B =
        | Phase2BIncrClassCtor of
          IncrClassChecking.IncrClassCtorLhs * TypedTree.Binding option
        | Phase2BInherit of TypedTree.Expr * TypedTree.Val option
        | Phase2BIncrClassBindings of
          IncrClassChecking.IncrClassBindingGroup list
        | Phase2BMember of int
        | Phase2BIncrClassCtorJustAfterSuperInit
        | Phase2BIncrClassCtorJustAfterLastLet
      type TyconBindingsPhase2B =
        | TyconBindingsPhase2B of
          TypedTree.Tycon option * TypedTree.TyconRef * TyconBindingPhase2B list
      type MutRecDefnsPhase2BData =
        MutRecShape<TyconBindingsPhase2B,int list,
                    (MutRecDefnsPhase2DataForModule * CheckExpressions.TcEnv)> list
      type TyconBindingPhase2C =
        | Phase2CIncrClassCtor of
          IncrClassChecking.IncrClassCtorLhs * TypedTree.Binding option
        | Phase2CInherit of TypedTree.Expr * TypedTree.Val option
        | Phase2CIncrClassBindings of
          IncrClassChecking.IncrClassBindingGroup list
        | Phase2CMember of
          CheckExpressions.PreInitializationGraphEliminationBinding
        | Phase2CIncrClassCtorJustAfterSuperInit
        | Phase2CIncrClassCtorJustAfterLastLet
      type TyconBindingsPhase2C =
        | TyconBindingsPhase2C of
          TypedTree.Tycon option * TypedTree.TyconRef * TyconBindingPhase2C list
      type MutRecDefnsPhase2CData =
        MutRecShape<TyconBindingsPhase2C,
                    CheckExpressions.PreInitializationGraphEliminationBinding list,
                    (MutRecDefnsPhase2DataForModule * CheckExpressions.TcEnv)> list
      val TcMutRecBindings_Phase2A_CreateRecursiveValuesAndCheckArgumentPatterns
           :
        cenv:cenv ->
          tpenv:CheckExpressions.UnscopedTyparEnv ->
            envMutRec:CheckExpressions.TcEnv * mutRecDefns:MutRecDefnsPhase2Info ->
              MutRecDefnsPhase2AData *
              CheckExpressions.PreCheckingRecursiveBinding list *
              CheckExpressions.UnscopedTyparEnv
      val TcMutRecBindings_Phase2B_TypeCheckAndIncrementalGeneralization:
        cenv:cenv ->
          tpenv:CheckExpressions.UnscopedTyparEnv ->
            envInitial:CheckExpressions.TcEnv ->
              envMutRec:CheckExpressions.TcEnv * defnsAs:MutRecDefnsPhase2AData *
              uncheckedRecBinds:CheckExpressions.PreCheckingRecursiveBinding list *
              scopem:Range.range ->
                MutRecDefnsPhase2BData *
                CheckExpressions.PostGeneralizationRecursiveBinding list *
                CheckExpressions.UnscopedTyparEnv
      val TcMutRecBindings_Phase2C_FixupRecursiveReferences:
        cenv:cenv ->
          denv:TypedTreeOps.DisplayEnv * defnsBs:MutRecDefnsPhase2BData *
          generalizedTyparsForRecursiveBlock:TypedTree.Typar list *
          generalizedRecBinds:CheckExpressions.PostGeneralizationRecursiveBinding list *
          scopem:Range.range ->
            MutRecShape<TyconBindingsPhase2C,
                        CheckExpressions.PreInitializationGraphEliminationBinding list,
                        (MutRecDefnsPhase2DataForModule * CheckExpressions.TcEnv)> list
      val TcMutRecBindings_Phase2D_ExtractImplicitFieldAndMethodBindings:
        cenv:cenv ->
          envMutRec:CheckExpressions.TcEnv ->
            tpenv:CheckExpressions.UnscopedTyparEnv ->
              denv:TypedTreeOps.DisplayEnv *
              generalizedTyparsForRecursiveBlock:TypedTree.Typar list *
              defnsCs:MutRecDefnsPhase2CData ->
                MutRecShape<(TypedTree.Tycon option *
                             CheckExpressions.PreInitializationGraphEliminationBinding list *
                             TypedTree.Binding list),
                            CheckExpressions.PreInitializationGraphEliminationBinding list,
                            (MutRecDefnsPhase2DataForModule *
                             CheckExpressions.TcEnv)> list
      val TcModuleAbbrevDecl:
        cenv:cenv ->
          scopem:Range.range ->
            env:CheckExpressions.TcEnv ->
              id:SyntaxTree.Ident * p:SyntaxTree.Ident list * m:Range.range ->
                CheckExpressions.TcEnv
      val TcMutRecDefns_UpdateNSContents:
        mutRecNSInfo:(TypedTree.ModuleOrNamespace option *
                      TypedTree.ModuleOrNamespaceType ref) option -> unit
      val TcMutRecDefns_UpdateModuleContents:
        mutRecNSInfo:(TypedTree.ModuleOrNamespace option *
                      TypedTree.ModuleOrNamespaceType ref) option ->
          defns:MutRecShape<'a,'b,(MutRecDefnsPhase2DataForModule * 'c)> list ->
            unit
      val TcMutRecDefns_ComputeEnvs:
        getTyconOpt:('a -> TypedTree.Tycon option) ->
          getVals:('b -> TypedTree.Val list) ->
            cenv:cenv ->
              report:bool ->
                scopem:Range.range ->
                  m:Range.range ->
                    envInitial:CheckExpressions.TcEnv ->
                      mutRecShape:MutRecShape<'a,'b,
                                              MutRecDefnsPhase2DataForModule> list ->
                        CheckExpressions.TcEnv *
                        MutRecShape<'a,'b,
                                    (MutRecDefnsPhase2DataForModule *
                                     CheckExpressions.TcEnv)> list
      val TcMutRecDefns_Phase2_Bindings:
        cenv:cenv ->
          envInitial:CheckExpressions.TcEnv ->
            tpenv:CheckExpressions.UnscopedTyparEnv ->
              bindsm:Range.range ->
                scopem:Range.range ->
                  mutRecNSInfo:(TypedTree.ModuleOrNamespace option *
                                TypedTree.ModuleOrNamespaceType ref) option ->
                    envMutRecPrelimWithReprs:CheckExpressions.TcEnv ->
                      mutRecDefns:MutRecDefnsPhase2Info ->
                        MutRecShape<(TypedTree.Tycon option *
                                     TypedTree.Binding list),
                                    TypedTree.Binding list,
                                    (MutRecDefnsPhase2DataForModule *
                                     CheckExpressions.TcEnv)> list *
                        CheckExpressions.TcEnv
  
    val TcMutRecDefns_Phase2:
      cenv:cenv ->
        envInitial:CheckExpressions.TcEnv ->
          bindsm:Range.range ->
            scopem:Range.range ->
              mutRecNSInfo:(TypedTree.ModuleOrNamespace option *
                            TypedTree.ModuleOrNamespaceType ref) option ->
                envMutRec:CheckExpressions.TcEnv ->
                  mutRecDefns:MutRecDefnsPhase2Data ->
                    MutRecShape<(TypedTree.Tycon option * TypedTree.Binding list),
                                TypedTree.Binding list,
                                (MutRecDefnsPhase2DataForModule *
                                 CheckExpressions.TcEnv)> list *
                    CheckExpressions.TcEnv
    module AddAugmentationDeclarations =
      val tcaugHasNominalInterface:
        g:TcGlobals.TcGlobals ->
          tcaug:TypedTree.TyconAugmentation -> tcref:TypedTree.TyconRef -> bool
      val AddGenericCompareDeclarations:
        cenv:cenv ->
          env:CheckExpressions.TcEnv ->
            scSet:Set<TypedTree.Stamp> -> tycon:TypedTree.Tycon -> unit
      val AddGenericEqualityWithComparerDeclarations:
        cenv:cenv ->
          env:CheckExpressions.TcEnv ->
            seSet:Set<TypedTree.Stamp> -> tycon:TypedTree.Tycon -> unit
      val AddGenericCompareBindings:
        cenv:cenv -> tycon:TypedTree.Tycon -> TypedTree.Binding list
      val AddGenericCompareWithComparerBindings:
        cenv:cenv -> tycon:TypedTree.Tycon -> TypedTree.Binding list
      val AddGenericEqualityWithComparerBindings:
        cenv:cenv -> tycon:TypedTree.Tycon -> TypedTree.Binding list
      val AddGenericHashAndComparisonDeclarations:
        cenv:cenv ->
          env:CheckExpressions.TcEnv ->
            scSet:Set<TypedTree.Stamp> ->
              seSet:Set<TypedTree.Stamp> -> tycon:TypedTree.Tycon -> unit
      val AddGenericHashAndComparisonBindings:
        cenv:cenv -> tycon:TypedTree.Tycon -> TypedTree.Binding list
      val AddGenericEqualityBindings:
        cenv:cenv ->
          env:CheckExpressions.TcEnv ->
            tycon:TypedTree.Tycon -> TypedTree.Binding list
  
    module TyconConstraintInference =
      val InferSetOfTyconsSupportingComparable:
        cenv:cenv ->
          denv:TypedTreeOps.DisplayEnv ->
            tyconsWithStructuralTypes:(TypedTree.Tycon *
                                       (TypedTree.TType * 'a) list) list ->
              Set<TypedTree.Stamp>
      val InferSetOfTyconsSupportingEquatable:
        cenv:cenv ->
          denv:TypedTreeOps.DisplayEnv ->
            tyconsWithStructuralTypes:(TypedTree.Tycon *
                                       (TypedTree.TType * 'a) list) list ->
              Set<TypedTree.Stamp>
  
    val ComputeModuleName: longPath:SyntaxTree.Ident list -> SyntaxTree.Ident
    val CheckForDuplicateConcreteType:
      env:CheckExpressions.TcEnv -> nm:string -> m:Range.range -> unit
    val CheckForDuplicateModule:
      env:CheckExpressions.TcEnv -> nm:string -> m:Range.range -> unit
    module TcExceptionDeclarations =
      val TcExnDefnCore_Phase1A:
        cenv:CheckExpressions.TcFileState ->
          env:CheckExpressions.TcEnv ->
            parent:TypedTree.ParentRef ->
              SyntaxTree.SynExceptionDefnRepr -> TypedTree.Entity
      val TcExnDefnCore_Phase1G_EstablishRepresentation:
        cenv:cenv ->
          env:CheckExpressions.TcEnv ->
            parent:TypedTree.ParentRef ->
              exnc:TypedTree.Entity ->
                SyntaxTree.SynExceptionDefnRepr -> TypedTree.RecdField list
      val private TcExnDefnCore:
        cenv:CheckExpressions.TcFileState ->
          env:CheckExpressions.TcEnv ->
            parent:TypedTree.ParentRef ->
              synExnDefnRepr:SyntaxTree.SynExceptionDefnRepr ->
                TypedTree.Binding list * TypedTree.Entity
      val TcExnDefn:
        cenv:CheckExpressions.TcFileState ->
          envInitial:CheckExpressions.TcEnv ->
            parent:TypedTree.ParentRef ->
              SyntaxTree.SynExceptionDefn * scopem:Range.range ->
                TypedTree.Binding list * TypedTree.Entity *
                CheckExpressions.TcEnv
      val TcExnSignature:
        cenv:CheckExpressions.TcFileState ->
          envInitial:CheckExpressions.TcEnv ->
            parent:TypedTree.ParentRef ->
              tpenv:CheckExpressions.UnscopedTyparEnv ->
                SyntaxTree.SynExceptionSig * scopem:Range.range ->
                  TypedTree.Binding list * TypedTree.Val list *
                  TypedTree.EntityRef * CheckExpressions.TcEnv
  
    module EstablishTypeDefinitionCores =
      type TypeRealizationPass =
        | FirstPass
        | SecondPass
      val private ComputeTyconName:
        longPath:SyntaxTree.Ident list * doErase:bool * typars:TypedTree.Typars ->
          SyntaxTree.Ident
      val private GetTyconAttribs:
        g:TcGlobals.TcGlobals ->
          attrs:TypedTree.Attribs -> bool * bool * bool * bool * bool
      val private InferTyconKind:
        g:TcGlobals.TcGlobals ->
          kind:SyntaxTree.SynTypeDefnKind * attrs:TypedTree.Attribs *
          slotsigs:'a list * fields:'b list * inSig:bool * isConcrete:bool *
          m:Range.range -> SyntaxTree.SynTypeDefnKind
      val private ( |TyconCoreAbbrevThatIsReallyAUnion|_| ):
        hasMeasureAttr:bool * envinner:CheckExpressions.TcEnv *
        id:SyntaxTree.Ident ->
          synTyconRepr:SyntaxTree.SynTypeDefnSimpleRepr ->
            (SyntaxTree.Ident * Range.range) option
      val private GetStructuralElementsOfTyconDefn:
        cenv:CheckExpressions.TcFileState ->
          env:CheckExpressions.TcEnv ->
            tpenv:CheckExpressions.UnscopedTyparEnv ->
              MutRecDefnsPhase1DataForTycon ->
                tycon:Lib.NonNullSlot<TypedTree.Entity> ->
                  (TypedTree.TType * Range.range) list
      val ComputeModuleOrNamespaceKind:
        g:TcGlobals.TcGlobals ->
          isModule:bool ->
            typeNames:Set<'a> ->
              attribs:TypedTree.Attribs ->
                nm:'a -> TypedTree.ModuleOrNamespaceKind when 'a: comparison
      val AdjustModuleName:
        modKind:TypedTree.ModuleOrNamespaceKind -> nm:string -> string
      val InstanceMembersNeedSafeInitCheck:
        cenv:cenv -> m:Range.range -> thisTy:TypedTree.TType -> bool
      val ComputeInstanceSafeInitInfo:
        cenv:cenv ->
          env:CheckExpressions.TcEnv ->
            m:Range.range ->
              thisTy:TypedTree.TType -> CheckExpressions.SafeInitData
      val TypeNamesInMutRecDecls:
        cenv:CheckExpressions.TcFileState ->
          env:CheckExpressions.TcEnv ->
            compDecls:MutRecShapes<(MutRecDefnsPhase1DataForTycon * 'MemberInfo),
                                   'LetInfo,SyntaxTree.SynComponentInfo> ->
              Set<string>
      val TypeNamesInNonMutRecDecls:
        defs:seq<SyntaxTree.SynModuleDecl> -> Set<string>
      val TypeNamesInNonMutRecSigDecls:
        defs:seq<SyntaxTree.SynModuleSigDecl> -> Set<string>
      val TcTyconDefnCore_Phase1A_BuildInitialModule:
        cenv:CheckExpressions.TcFileState ->
          envInitial:CheckExpressions.TcEnv ->
            parent:TypedTree.ParentRef ->
              typeNames:Set<string> ->
                compInfo:SyntaxTree.SynComponentInfo ->
                  decls:MutRecShapes<(MutRecDefnsPhase1DataForTycon * 'a),'b,
                                     SyntaxTree.SynComponentInfo> ->
                    MutRecDefnsPhase2DataForModule *
                    (TypedTree.ParentRef * Set<string> * CheckExpressions.TcEnv)
      val private TcTyconDefnCore_Phase1A_BuildInitialTycon:
        cenv:cenv ->
          env:CheckExpressions.TcEnv ->
            parent:TypedTree.ParentRef ->
              MutRecDefnsPhase1DataForTycon -> TypedTree.Entity
      val private TcTyconDefnCore_Phase1B_EstablishBasicKind:
        cenv:cenv ->
          inSig:bool ->
            envinner:CheckExpressions.TcEnv ->
              MutRecDefnsPhase1DataForTycon ->
                tycon:TypedTree.Tycon ->
                  TypedTree.Attrib list * (unit -> TypedTree.Attribs)
      val private TcTyconDefnCore_GetGenerateDeclaration_Rhs:
        SyntaxTree.SynType ->
          (SyntaxTree.LongIdent * SyntaxTree.SynType list * Range.range) option
      val private TcTyconDefnCore_TryAsGenerateDeclaration:
        cenv:cenv ->
          envinner:CheckExpressions.TcEnv ->
            tpenv:CheckExpressions.UnscopedTyparEnv ->
              tycon:TypedTree.Tycon * rhsType:SyntaxTree.SynType ->
                (TypedTree.TyconRef * Tainted<ExtensionTyping.ProvidedType> *
                 (unit -> unit) * SyntaxTree.SynType list * Range.range) option
      val private
          TcTyconDefnCore_Phase1C_EstablishDeclarationForGeneratedSetOfTypes:
        cenv:cenv ->
          inSig:bool ->
            tycon:TypedTree.Tycon * rhsType:SyntaxTree.SynType *
            tcrefForContainer:TypedTree.TyconRef *
            theRootType:Tainted<ExtensionTyping.ProvidedType> *
            checkTypeName:(unit -> unit) * args:'a list * m:Range.range -> unit
      val private TcTyconDefnCore_Phase1C_Phase1E_EstablishAbbreviations:
        cenv:cenv ->
          envinner:CheckExpressions.TcEnv ->
            inSig:bool ->
              tpenv:CheckExpressions.UnscopedTyparEnv ->
                pass:TypeRealizationPass ->
                  MutRecDefnsPhase1DataForTycon ->
                    tycon:TypedTree.Tycon -> attrs:TypedTree.Attribs -> unit
      val private
          TcTyconDefnCore_Phase1D_Phase1F_EstablishSuperTypesAndInterfaceTypes:
        cenv:CheckExpressions.TcFileState ->
          tpenv:CheckExpressions.UnscopedTyparEnv ->
            inSig:bool ->
              pass:TypeRealizationPass ->
                envMutRec:CheckExpressions.TcEnv *
                mutRecDefns:MutRecShape<((MutRecDefnsPhase1DataForTycon * 'a *
                                          'b) *
                                         (TypedTree.Tycon *
                                          (TypedTree.Attribs * 'c)) option),'d,
                                        ('e * CheckExpressions.TcEnv)> list ->
                  unit
      val private TcTyconDefnCore_Phase1G_EstablishRepresentation:
        cenv:cenv ->
          envinner:CheckExpressions.TcEnv ->
            tpenv:CheckExpressions.UnscopedTyparEnv ->
              inSig:bool ->
                MutRecDefnsPhase1DataForTycon ->
                  tycon:TypedTree.Tycon ->
                    attrs:TypedTree.Attribs ->
                      TypedTree.Val option * CheckExpressions.SafeInitData
      val private TcTyconDefnCore_CheckForCyclicAbbreviations:
        tycons:TypedTree.Entity list -> unit
      val TcTyconDefnCore_CheckForCyclicStructsAndInheritance:
        cenv:cenv -> tycons:TypedTree.Entity list -> unit
      val TcMutRecDefns_CheckExplicitConstraints:
        cenv:CheckExpressions.TcFileState ->
          tpenv:CheckExpressions.UnscopedTyparEnv ->
            m:Range.range ->
              checkCxs:CheckExpressions.CheckConstraints ->
                envMutRecPrelim:CheckExpressions.TcEnv ->
                  withEnvs:MutRecShape<((MutRecDefnsPhase1DataForTycon * 'a * 'b) *
                                        TypedTree.Tycon option),'c,
                                       ('d * CheckExpressions.TcEnv)> list ->
                    unit
      val TcMutRecDefns_Phase1:
        mkLetInfo:(CheckExpressions.ContainerInfo -> 'LetInfo -> 'a) ->
          cenv:cenv ->
            envInitial:CheckExpressions.TcEnv ->
              parent:TypedTree.ParentRef ->
                typeNames:Set<string> ->
                  inSig:bool ->
                    tpenv:CheckExpressions.UnscopedTyparEnv ->
                      m:Range.range ->
                        scopem:Range.range ->
                          mutRecNSInfo:(TypedTree.ModuleOrNamespace option *
                                        TypedTree.ModuleOrNamespaceType ref) option ->
                            mutRecDefns:MutRecShapes<(MutRecDefnsPhase1DataForTycon *
                                                      'MemberInfo),'LetInfo,
                                                     SyntaxTree.SynComponentInfo> ->
                              TypedTree.Entity list * CheckExpressions.TcEnv *
                              MutRecShape<((MutRecDefnsPhase1DataForTycon *
                                            'MemberInfo * TypedTree.ParentRef) *
                                           TypedTree.Entity option *
                                           (unit -> unit) *
                                           (TypedTree.Val option *
                                            CheckExpressions.SafeInitData)),'a,
                                          (MutRecDefnsPhase2DataForModule *
                                           CheckExpressions.TcEnv)> list
  
    module TcDeclarations =
      val private ComputeTyconDeclKind:
        cenv:cenv ->
          envForDecls:CheckExpressions.TcEnv ->
            tyconOpt:Lib.NonNullSlot<TypedTree.Entity> option ->
              isAtOriginalTyconDefn:bool ->
                inSig:bool ->
                  m:Range.range ->
                    synTypars:SyntaxTree.SynTyparDecl list ->
                      synTyparCxs:SyntaxTree.SynTypeConstraint list ->
                        longPath:SyntaxTree.Ident list ->
                          CheckExpressions.DeclKind * TypedTree.EntityRef *
                          TypedTree.Typars
      val private isAugmentationTyconDefnRepr:
        _arg1:SyntaxTree.SynTypeDefnSimpleRepr -> bool
      val private isAutoProperty: _arg1:SyntaxTree.SynMemberDefn -> bool
      val private isMember: _arg1:SyntaxTree.SynMemberDefn -> bool
      val private isImplicitCtor: _arg1:SyntaxTree.SynMemberDefn -> bool
      val private isImplicitInherit: _arg1:SyntaxTree.SynMemberDefn -> bool
      val private isAbstractSlot: _arg1:SyntaxTree.SynMemberDefn -> bool
      val private isInterface: _arg1:SyntaxTree.SynMemberDefn -> bool
      val private isInherit: _arg1:SyntaxTree.SynMemberDefn -> bool
      val private isField: _arg1:SyntaxTree.SynMemberDefn -> bool
      val private isTycon: _arg1:SyntaxTree.SynMemberDefn -> bool
      val private allFalse: ps:('a -> bool) list -> x:'a -> bool
      val private CheckMembersForm: ds:SyntaxTree.SynMemberDefn list -> unit
      val private SplitTyconDefn:
        SyntaxTree.SynTypeDefn ->
          MutRecDefnsPhase1DataForTycon * SyntaxTree.SynMemberDefn list
      val TcMutRecDefinitions:
        cenv:cenv ->
          envInitial:CheckExpressions.TcEnv ->
            parent:TypedTree.ParentRef ->
              typeNames:Set<string> ->
                tpenv:CheckExpressions.UnscopedTyparEnv ->
                  m:Range.range ->
                    scopem:Range.range ->
                      mutRecNSInfo:(TypedTree.ModuleOrNamespace option *
                                    TypedTree.ModuleOrNamespaceType ref) option ->
                        mutRecDefns:MutRecDefnsInitialData ->
                          MutRecShape<(TypedTree.Tycon option *
                                       TypedTree.Binding list),
                                      TypedTree.Binding list,
                                      (MutRecDefnsPhase2DataForModule *
                                       CheckExpressions.TcEnv)> list *
                          CheckExpressions.TcEnv
      val private SplitTyconSignature:
        SyntaxTree.SynTypeDefnSig ->
          MutRecDefnsPhase1DataForTycon *
          (SyntaxTree.SynComponentInfo * SyntaxTree.SynMemberSig list)
      val private TcMutRecSignatureDecls_Phase2:
        cenv:cenv ->
          scopem:Range.range ->
            envMutRec:CheckExpressions.TcEnv ->
              mutRecDefns:MutRecShape<((MutRecDefnsPhase1DataForTycon *
                                        (SyntaxTree.SynComponentInfo *
                                         SyntaxTree.SynMemberSig list) *
                                        TypedTree.ParentRef) *
                                       Lib.NonNullSlot<TypedTree.Entity> option *
                                       'a * 'b),
                                      (CheckExpressions.ContainerInfo *
                                       SyntaxTree.SynValSig),
                                      ('c * CheckExpressions.TcEnv)> list ->
                MutRecShape<(TypedTree.Val list *
                             CheckExpressions.UnscopedTyparEnv),
                            CheckExpressions.TcEnv,('c * CheckExpressions.TcEnv)> list
      val TcMutRecSignatureDecls:
        cenv:cenv ->
          envInitial:CheckExpressions.TcEnv ->
            parent:TypedTree.ParentRef ->
              typeNames:Set<string> ->
                tpenv:CheckExpressions.UnscopedTyparEnv ->
                  m:Range.range ->
                    scopem:Range.range ->
                      mutRecNSInfo:(TypedTree.ModuleOrNamespace option *
                                    TypedTree.ModuleOrNamespaceType ref) option ->
                        mutRecSigs:MutRecSigsInitialData ->
                          CheckExpressions.TcEnv
  
    val TcSignatureElementNonMutRec:
      cenv:CheckExpressions.TcFileState ->
        parent:TypedTree.ParentRef ->
          typeNames:Set<string> ->
          m:Range.range ->
              env:CheckExpressions.TcEnv ->
                synSigDecl:SyntaxTree.SynModuleSigDecl ->
                  AbstractIL.Internal.Library.Eventually<CheckExpressions.TcEnv>
    val TcSignatureElements:
      cenv:CheckExpressions.TcFileState ->
        parent:TypedTree.ParentRef ->
        m:Range.range ->
            env:CheckExpressions.TcEnv ->
              xml:XmlDoc.PreXmlDoc ->
                mutRecNSInfo:(TypedTree.ModuleOrNamespace option *
                              TypedTree.ModuleOrNamespaceType ref) option ->
                  defs:SyntaxTree.SynModuleSigDecl list ->
                    AbstractIL.Internal.Library.Eventually<CheckExpressions.TcEnv>
    val TcSignatureElementsNonMutRec:
      cenv:CheckExpressions.TcFileState ->
        parent:TypedTree.ParentRef ->
          typeNames:Set<string> ->
          m:Range.range ->
              env:CheckExpressions.TcEnv ->
                defs:SyntaxTree.SynModuleSigDecl list ->
                  AbstractIL.Internal.Library.Eventually<CheckExpressions.TcEnv>
    val TcSignatureElementsMutRec:
      cenv:CheckExpressions.TcFileState ->
        parent:TypedTree.ParentRef ->
          typeNames:Set<string> ->
            m:Range.range ->
              mutRecNSInfo:(TypedTree.ModuleOrNamespace option *
                            TypedTree.ModuleOrNamespaceType ref) option ->
                envInitial:CheckExpressions.TcEnv ->
                  defs:SyntaxTree.SynModuleSigDecl list ->
                    AbstractIL.Internal.Library.Eventually<CheckExpressions.TcEnv>
    val TcModuleOrNamespaceSignatureElementsNonMutRec:
      cenv:CheckExpressions.TcFileState ->
        parent:TypedTree.ParentRef ->
          env:CheckExpressions.TcEnv ->
            id:SyntaxTree.Ident * modKind:TypedTree.ModuleOrNamespaceKind *
            defs:SyntaxTree.SynModuleSigDecl list * m:Range.range *
            xml:XmlDoc.PreXmlDoc ->
              AbstractIL.Internal.Library.Eventually<TypedTree.ModuleOrNamespaceType *
                                                     CheckExpressions.TcEnv>
    val ElimModuleDoBinding:
      bind:SyntaxTree.SynModuleDecl -> SyntaxTree.SynModuleDecl
    val TcMutRecDefnsEscapeCheck:
      binds:MutRecShapes<(TypedTree.Tycon option * TypedTree.Binding list),
                         TypedTree.Binding list,'a> ->
        env:CheckExpressions.TcEnv -> unit
    val CheckLetOrDoInNamespace:
      binds:SyntaxTree.SynBinding list -> m:Range.range -> unit
    val TcModuleOrNamespaceElementNonMutRec:
      cenv:cenv ->
        parent:TypedTree.ParentRef ->
          typeNames:Set<string> ->
            scopem:Range.range ->
              env:CheckExpressions.TcEnv ->
                synDecl:SyntaxTree.SynModuleDecl ->
                  AbstractIL.Internal.Library.Eventually<((TypedTree.ModuleOrNamespaceExpr list ->
                                                             TypedTree.ModuleOrNamespaceExpr list) *
                                                          (System.AttributeTargets *
                                                           TypedTree.Attrib) list) *
                                                         CheckExpressions.TcEnv *
                                                         CheckExpressions.TcEnv>
    val TcModuleOrNamespaceElementsNonMutRec:
      cenv:cenv ->
        parent:TypedTree.ParentRef ->
          typeNames:Set<string> ->
          m:Range.range ->
              defsSoFar:((TypedTree.ModuleOrNamespaceExpr list ->
                            TypedTree.ModuleOrNamespaceExpr list) *
                         (System.AttributeTargets * TypedTree.Attrib) list) list *
              env:CheckExpressions.TcEnv * envAtEnd:CheckExpressions.TcEnv ->
                moreDefs:SyntaxTree.SynModuleDecl list ->
                  AbstractIL.Internal.Library.Eventually<((TypedTree.ModuleOrNamespaceExpr list ->
                                                             TypedTree.ModuleOrNamespaceExpr list) *
                                                          (System.AttributeTargets *
                                                           TypedTree.Attrib) list) list *
                                                         CheckExpressions.TcEnv>
    val TcModuleOrNamespaceElementsMutRec:
      cenv:cenv ->
        parent:TypedTree.ParentRef ->
          typeNames:Set<string> ->
            m:Range.range ->
              envInitial:CheckExpressions.TcEnv ->
                mutRecNSInfo:(TypedTree.ModuleOrNamespace option *
                              TypedTree.ModuleOrNamespaceType ref) option ->
                  defs:SyntaxTree.SynModuleDecl list ->
                    AbstractIL.Internal.Library.Eventually<((TypedTree.ModuleOrNamespaceExpr list ->
                                                               TypedTree.ModuleOrNamespaceExpr list) *
                                                            (System.AttributeTargets *
                                                             TypedTree.Attrib) list) *
                                                           CheckExpressions.TcEnv *
                                                           CheckExpressions.TcEnv>
    val TcMutRecDefsFinish:
      cenv:cenv ->
        defs:MutRecShape<(TypedTree.Tycon option * TypedTree.Binding list),
                         TypedTree.Binding list,
                         (MutRecDefnsPhase2DataForModule *
                          CheckExpressions.TcEnv)> list ->
          m:Range.range -> TypedTree.ModuleOrNamespaceExpr
    val TcModuleOrNamespaceElements:
      cenv:cenv ->
        parent:TypedTree.ParentRef ->
        m:Range.range ->
            env:CheckExpressions.TcEnv ->
              xml:XmlDoc.PreXmlDoc ->
                mutRecNSInfo:(TypedTree.ModuleOrNamespace option *
                              TypedTree.ModuleOrNamespaceType ref) option ->
                  defs:SyntaxTree.SynModuleDecl list ->
                    AbstractIL.Internal.Library.Eventually<TypedTree.ModuleOrNamespaceExpr *
                                                           (System.AttributeTargets *
                                                            TypedTree.Attrib) list *
                                                           CheckExpressions.TcEnv>
    val ApplyAssemblyLevelAutoOpenAttributeToTcEnv:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          ccu:TypedTree.CcuThunk ->
            scopem:Range.range ->
              env:CheckExpressions.TcEnv ->
                p:System.String * root:bool -> CheckExpressions.TcEnv
    val AddCcuToTcEnv:
      TcGlobals.TcGlobals * Import.ImportMap * Range.range *
      CheckExpressions.TcEnv * assemblyName:string * ccu:TypedTree.CcuThunk *
      autoOpens:string list * internalsVisibleToAttributes:string list ->
        CheckExpressions.TcEnv
    val emptyTcEnv: g:TcGlobals.TcGlobals -> CheckExpressions.TcEnv
    val CreateInitialTcEnv:
      TcGlobals.TcGlobals * Import.ImportMap * Range.range * assemblyName:string *
      (TypedTree.CcuThunk * string list * string list) list ->
        CheckExpressions.TcEnv
    type ConditionalDefines = string list
    type TopAttribs =
      { mainMethodAttrs: TypedTree.Attribs
        netModuleAttrs: TypedTree.Attribs
        assemblyAttrs: TypedTree.Attribs }
    val EmptyTopAttrs: TopAttribs
    val CombineTopAttrs: TopAttribs -> TopAttribs -> TopAttribs
    val IterTyconsOfModuleOrNamespaceType:
      f:(TypedTree.Entity -> unit) ->
        mty:TypedTree.ModuleOrNamespaceType -> unit
    val ApplyDefaults:
      cenv:cenv ->
        g:TcGlobals.TcGlobals ->
          denvAtEnd:TypedTreeOps.DisplayEnv ->
            m:Range.range ->
              mexpr:TypedTree.ModuleOrNamespaceExpr ->
                extraAttribs:TypedTree.Attrib list -> unit
    val CheckValueRestriction:
      denvAtEnd:TypedTreeOps.DisplayEnv ->
        rootSigOpt:'a option ->
          implFileTypePriorToSig:TypedTree.ModuleOrNamespaceType ->
            m:Range.range -> unit
    val SolveInternalUnknowns:
      g:TcGlobals.TcGlobals ->
        cenv:cenv ->
          denvAtEnd:TypedTreeOps.DisplayEnv ->
            mexpr:TypedTree.ModuleOrNamespaceExpr ->
              extraAttribs:TypedTree.Attrib list -> unit
    val CheckModuleSignature:
      g:TcGlobals.TcGlobals ->
        cenv:cenv ->
          m:Range.range ->
            denvAtEnd:TypedTreeOps.DisplayEnv ->
              rootSigOpt:TypedTree.ModuleOrNamespaceType option ->
                implFileTypePriorToSig:TypedTree.ModuleOrNamespaceType ->
                  implFileSpecPriorToSig:Lib.NonNullSlot<TypedTree.Entity> ->
                    mexpr:TypedTree.ModuleOrNamespaceExpr ->
                      TypedTree.ModuleOrNamespaceExprWithSig
    val MakeInitialEnv:
      env:CheckExpressions.TcEnv ->
        CheckExpressions.TcEnv * TypedTree.ModuleOrNamespaceType ref
    val TypeCheckOneImplFile:
      TcGlobals.TcGlobals * CompilerGlobalState.NiceNameGenerator *
      Import.ImportMap * TypedTree.CcuThunk * (unit -> bool) *
      string list option * NameResolution.TcResultsSink * bool ->
        CheckExpressions.TcEnv ->
          TypedTree.ModuleOrNamespaceType option ->
            SyntaxTree.ParsedImplFileInput ->
              AbstractIL.Internal.Library.Eventually<TopAttribs *
                                                     TypedTree.TypedImplFile *
                                                     TypedTree.ModuleOrNamespaceType *
                                                     CheckExpressions.TcEnv *
                                                     bool>
    val TypeCheckOneSigFile:
      TcGlobals.TcGlobals * CompilerGlobalState.NiceNameGenerator *
      Import.ImportMap * TypedTree.CcuThunk * (unit -> bool) *
      string list option * NameResolution.TcResultsSink * bool ->
        CheckExpressions.TcEnv ->
          SyntaxTree.ParsedSigFileInput ->
            AbstractIL.Internal.Library.Eventually<CheckExpressions.TcEnv *
                                                   TypedTree.ModuleOrNamespaceType *
                                                   bool>


namespace FSharp.Compiler
  module internal Optimizer =
    val verboseOptimizationInfo: bool
    val verboseOptimizations: bool
    val i_ldlen: AbstractIL.IL.ILInstr list
    [<LiteralAttribute>]
    val callSize: int = 1
    [<LiteralAttribute>]
    val forAndWhileLoopSize: int = 5
    [<LiteralAttribute>]
    val tryWithSize: int = 5
    [<LiteralAttribute>]
    val tryFinallySize: int = 5
    [<LiteralAttribute>]
    val closureTotalSize: int = 10
    [<LiteralAttribute>]
    val methodDefnTotalSize: int = 1
    type TypeValueInfo = | UnknownTypeValue
    type ExprValueInfo =
      | UnknownValue
      | SizeValue of size: int * ExprValueInfo
      | ValValue of TypedTree.ValRef * ExprValueInfo
      | TupleValue of ExprValueInfo []
      | RecdValue of TypedTree.TyconRef * ExprValueInfo []
      | UnionCaseValue of TypedTree.UnionCaseRef * ExprValueInfo []
      | ConstValue of TypedTree.Const * TypedTree.TType
      | CurriedLambdaValue of
        id: CompilerGlobalState.Unique * arity: int * size: int *
        value: TypedTree.Expr * TypedTree.TType
      | ConstExprValue of size: int * value: TypedTree.Expr
    type ValInfo =
      { ValMakesNoCriticalTailcalls: bool
        ValExprInfo: ExprValueInfo }
    type ValInfos =
  
        new: entries:seq<TypedTree.ValRef * ValInfo> -> ValInfos
        member Filter: f:(TypedTree.ValRef * ValInfo -> bool) -> ValInfos
        member
          Map: f:(TypedTree.ValRef * ValInfo -> TypedTree.ValRef * ValInfo) ->
                  ValInfos
        member
          TryFind: v:TypedTree.ValRef -> (TypedTree.ValRef * ValInfo) option
        member
          TryFindForFslib: g:TcGlobals.TcGlobals * vref:TypedTree.ValRef ->
                              bool * (TypedTree.ValRef * ValInfo)
        member Entries: seq<TypedTree.ValRef * ValInfo>
    
    type ModuleInfo =
      { ValInfos: ValInfos
        ModuleOrNamespaceInfos:
          AbstractIL.Internal.Library.NameMap<LazyModuleInfo> }
    and LazyModuleInfo = Lazy<ModuleInfo>
    type ImplFileOptimizationInfo = LazyModuleInfo
    type CcuOptimizationInfo = LazyModuleInfo
    val braceL:
      x:Internal.Utilities.StructuredFormat.Layout ->
        Internal.Utilities.StructuredFormat.Layout
    val seqL:
      xL:('a -> Internal.Utilities.StructuredFormat.Layout) ->
        xs:seq<'a> -> Internal.Utilities.StructuredFormat.Layout
    val namemapL:
      xL:(string -> 'a -> Internal.Utilities.StructuredFormat.Layout) ->
        xmap:AbstractIL.Internal.Library.NameMap<'a> ->
          Internal.Utilities.StructuredFormat.Layout
    val exprValueInfoL:
      g:TcGlobals.TcGlobals ->
        exprVal:ExprValueInfo -> Internal.Utilities.StructuredFormat.Layout
    val exprValueInfosL:
      g:TcGlobals.TcGlobals ->
        vinfos:ExprValueInfo [] -> Internal.Utilities.StructuredFormat.Layout
    val moduleInfoL:
      TcGlobals.TcGlobals ->
        LazyModuleInfo -> Internal.Utilities.StructuredFormat.Layout
    val valInfoL:
      g:TcGlobals.TcGlobals ->
        x:ValInfo -> Internal.Utilities.StructuredFormat.Layout
    type Summary<'Info> =
      { Info: 'Info
        FunctionSize: int
        TotalSize: int
        HasEffect: bool
        MightMakeCriticalTailcall: bool }
    val SizeOfValueInfos: arr:ExprValueInfo [] -> int
    val SizeOfValueInfo: x:ExprValueInfo -> int
    [<LiteralAttribute>]
    val minDepthForASizeNode: int = 5
    val MakeValueInfoWithCachedSize:
      vdepth:int -> v:ExprValueInfo -> ExprValueInfo
    val MakeSizedValueInfo: v:ExprValueInfo -> ExprValueInfo
    val BoundValueInfoBySize: vinfo:ExprValueInfo -> ExprValueInfo
    [<LiteralAttribute>]
    val jitOptDefault: bool = true
    [<LiteralAttribute>]
    val localOptDefault: bool = true
    [<LiteralAttribute>]
    val crossModuleOptDefault: bool = true
    type OptimizationSettings =
      { abstractBigTargets: bool
        jitOptUser: bool option
        localOptUser: bool option
        crossModuleOptUser: bool option
        bigTargetSize: int
        veryBigExprSize: int
        lambdaInlineThreshold: int
        reportingPhase: bool
        reportNoNeedToTailcall: bool
        reportFunctionSizes: bool
        reportHasEffect: bool
        reportTotalSizes: bool }
      with
        member EliminateImmediatelyConsumedLocals: unit -> bool
        member EliminateRecdFieldGet: unit -> bool
        member EliminateSequential: unit -> bool
        member EliminateSwitch: unit -> bool
        member EliminateTryWithAndTryFinally: unit -> bool
        member EliminateTupleFieldGet: unit -> bool
        member EliminateUnionCaseFieldGet: unit -> bool
        member EliminateUnusedBindings: unit -> bool
        member ExpandStructuralValues: unit -> bool
        member InlineLambdas: unit -> bool
        member KeepOptimizationValues: unit -> bool
        member crossModuleOpt: unit -> bool
        member jitOpt: unit -> bool
        member localOpt: unit -> bool
        static member Defaults: OptimizationSettings
    
    type cenv =
      { g: TcGlobals.TcGlobals
        TcVal: ConstraintSolver.TcValF
        amap: Import.ImportMap
        optimizing: bool
        scope: TypedTree.CcuThunk
        localInternalVals:
          System.Collections.Generic.Dictionary<TypedTree.Stamp,ValInfo>
        settings: OptimizationSettings
        emitTailcalls: bool
        casApplied: System.Collections.Generic.Dictionary<TypedTree.Stamp,bool> }
      with
        override ToString: unit -> string
    
    [<SealedAttribute>]
    type IncrementalOptimizationEnv =
      { latestBoundId: SyntaxTree.Ident option
        dontInline: AbstractIL.Internal.Zset<CompilerGlobalState.Unique>
        dontSplitVars: TypedTreeOps.ValMap<unit>
        disableMethodSplitting: bool
        functionVal: (TypedTree.Val * TypedTree.ValReprInfo) option
        typarInfos: (TypedTree.Typar * TypeValueInfo) list
        localExternalVals:
          AbstractIL.Internal.Library.LayeredMap<TypedTree.Stamp,ValInfo>
        globalModuleInfos:
          AbstractIL.Internal.Library.LayeredMap<string,LazyModuleInfo> }
      with
        override ToString: unit -> string
        static member Empty: IncrementalOptimizationEnv
    
    val IsPartialExprVal: x:ExprValueInfo -> bool
    val CheckInlineValueIsComplete:
      v:TypedTree.Val -> res:ExprValueInfo -> unit
    val check:
      vref:TypedTree.ValRef -> res:ValInfo -> TypedTree.ValRef * ValInfo
    val EmptyModuleInfo: System.Lazy<ModuleInfo>
    val UnionOptimizationInfos: seq<LazyModuleInfo> -> System.Lazy<ModuleInfo>
    val FindOrCreateModuleInfo:
      n:'a -> ss:Map<'a,System.Lazy<ModuleInfo>> -> System.Lazy<ModuleInfo>
        when 'a: comparison
    val FindOrCreateGlobalModuleInfo:
      n:'a ->
        ss:AbstractIL.Internal.Library.LayeredMap<'a,System.Lazy<ModuleInfo>> ->
          System.Lazy<ModuleInfo> when 'a: comparison
    val BindValueInSubModuleFSharpCore:
      mp:string [] ->
        i:int -> v:TypedTree.Val -> vval:ValInfo -> ss:ModuleInfo -> ModuleInfo
    val BindValueInModuleForFslib:
      n:string ->
        mp:string [] ->
          i:int ->
            v:TypedTree.Val ->
              vval:ValInfo ->
                ss:AbstractIL.Internal.Library.NameMap<LazyModuleInfo> ->
                  AbstractIL.Internal.Library.NameMap<LazyModuleInfo>
    val BindValueInGlobalModuleForFslib:
      n:'a ->
        mp:string [] ->
          i:int ->
            v:TypedTree.Val ->
              vval:ValInfo ->
                ss:AbstractIL.Internal.Library.LayeredMap<'a,
                                                          System.Lazy<ModuleInfo>> ->
                  Map<'a,System.Lazy<ModuleInfo>> when 'a: comparison
    val BindValueForFslib:
      nlvref:TypedTree.NonLocalValOrMemberRef ->
        v:TypedTree.Val ->
          vval:ValInfo ->
            env:IncrementalOptimizationEnv -> IncrementalOptimizationEnv
    val UnknownValInfo: ValInfo
    val mkValInfo: info:Summary<ExprValueInfo> -> v:TypedTree.Val -> ValInfo
    val BindInternalLocalVal:
      cenv:cenv -> v:TypedTree.Val -> vval:ValInfo -> env:'a -> 'a
    val BindExternalLocalVal:
      cenv:cenv ->
        v:TypedTree.Val ->
          vval:ValInfo ->
            env:IncrementalOptimizationEnv -> IncrementalOptimizationEnv
    val BindValsInModuleOrNamespace:
      cenv:cenv ->
        mval:LazyModuleInfo ->
          env:IncrementalOptimizationEnv -> IncrementalOptimizationEnv
    val inline BindInternalValToUnknown: cenv:'a -> v:'b -> env:'c -> 'c
    val inline BindInternalValsToUnknown: cenv:'a -> vs:'b -> env:'c -> 'c
    val BindTypeVar:
      tyv:TypedTree.Typar ->
        typeinfo:TypeValueInfo ->
          env:IncrementalOptimizationEnv -> IncrementalOptimizationEnv
    val BindTypeVarsToUnknown:
      tps:TypedTree.Typar list ->
        env:IncrementalOptimizationEnv -> IncrementalOptimizationEnv
    val BindCcu:
      TypedTree.CcuThunk ->
        LazyModuleInfo ->
          IncrementalOptimizationEnv ->
            TcGlobals.TcGlobals -> IncrementalOptimizationEnv
    val GetInfoForLocalValue:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          v:TypedTree.Val -> m:Range.range -> ValInfo
    val TryGetInfoForCcu:
      env:IncrementalOptimizationEnv ->
        ccu:TypedTree.CcuThunk -> LazyModuleInfo option
    val TryGetInfoForEntity: sv:ModuleInfo -> n:string -> ModuleInfo option
    val TryGetInfoForPath:
      sv:ModuleInfo -> p:string [] -> i:int -> ModuleInfo option
    val TryGetInfoForNonLocalEntityRef:
      env:IncrementalOptimizationEnv ->
        nleref:TypedTree.NonLocalEntityRef -> ModuleInfo option
    val GetInfoForNonLocalVal:
      cenv:cenv ->
        env:IncrementalOptimizationEnv -> vref:TypedTree.ValRef -> ValInfo
    val GetInfoForVal:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          m:Range.range -> vref:TypedTree.ValRef -> ValInfo
    val stripValue: _arg1:ExprValueInfo -> ExprValueInfo
    val ( |StripConstValue|_| ): ev:ExprValueInfo -> TypedTree.Const option
    val ( |StripLambdaValue|_| ):
      ev:ExprValueInfo ->
        (CompilerGlobalState.Unique * int * int * TypedTree.Expr *
         TypedTree.TType) option
    val destTupleValue: ev:ExprValueInfo -> ExprValueInfo [] option
    val destRecdValue: ev:ExprValueInfo -> ExprValueInfo [] option
    val ( |StripUnionCaseValue|_| ):
      ev:ExprValueInfo -> (TypedTree.UnionCaseRef * ExprValueInfo []) option
    val mkBoolVal: g:TcGlobals.TcGlobals -> n:bool -> ExprValueInfo
    val mkInt8Val: g:TcGlobals.TcGlobals -> n:sbyte -> ExprValueInfo
    val mkInt16Val: g:TcGlobals.TcGlobals -> n:int16 -> ExprValueInfo
    val mkInt32Val: g:TcGlobals.TcGlobals -> n:int32 -> ExprValueInfo
    val mkInt64Val: g:TcGlobals.TcGlobals -> n:int64 -> ExprValueInfo
    val mkUInt8Val: g:TcGlobals.TcGlobals -> n:byte -> ExprValueInfo
    val mkUInt16Val: g:TcGlobals.TcGlobals -> n:uint16 -> ExprValueInfo
    val mkUInt32Val: g:TcGlobals.TcGlobals -> n:uint32 -> ExprValueInfo
    val mkUInt64Val: g:TcGlobals.TcGlobals -> n:uint64 -> ExprValueInfo
    val ( |StripInt32Value|_| ): _arg1:ExprValueInfo -> int32 option
    val MakeValueInfoForValue:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          vref:TypedTree.ValRef -> vinfo:ExprValueInfo -> ExprValueInfo
    val MakeValueInfoForRecord:
      tcref:TypedTree.TyconRef -> argvals:ExprValueInfo [] -> ExprValueInfo
    val MakeValueInfoForTuple: argvals:ExprValueInfo [] -> ExprValueInfo
    val MakeValueInfoForUnionCase:
      cspec:TypedTree.UnionCaseRef -> argvals:ExprValueInfo [] -> ExprValueInfo
    val MakeValueInfoForConst:
      c:TypedTree.Const -> ty:TypedTree.TType -> ExprValueInfo
    val inline IntegerUnaryOp:
      g:TcGlobals.TcGlobals ->
        f8:(sbyte -> sbyte) ->
          f16:(int16 -> int16) ->
            f32:(int -> int) ->
              f64:(int64 -> int64) ->
                fu8:(byte -> byte) ->
                  fu16:(uint16 -> uint16) ->
                    fu32:(uint32 -> uint32) ->
                      fu64:(uint64 -> uint64) ->
                        a:ExprValueInfo -> ExprValueInfo option
    val inline SignedIntegerUnaryOp:
      g:TcGlobals.TcGlobals ->
        f8:(sbyte -> sbyte) ->
          f16:(int16 -> int16) ->
            f32:(int32 -> int32) ->
              f64:(int64 -> int64) -> a:ExprValueInfo -> ExprValueInfo option
    val inline IntegerBinaryOp:
      g:TcGlobals.TcGlobals ->
        f8:(sbyte -> sbyte -> sbyte) ->
          f16:(int16 -> int16 -> int16) ->
            f32:(int -> int -> int) ->
              f64:(int64 -> int64 -> int64) ->
                fu8:(byte -> byte -> byte) ->
                  fu16:(uint16 -> uint16 -> uint16) ->
                    fu32:(uint32 -> uint32 -> uint32) ->
                      fu64:(uint64 -> uint64 -> uint64) ->
                        a:ExprValueInfo ->
                          b:ExprValueInfo -> ExprValueInfo option
    val mkAssemblyCodeValueInfo:
      g:TcGlobals.TcGlobals ->
        instrs:AbstractIL.IL.ILInstr list ->
          argvals:ExprValueInfo list ->
            tys:TypedTree.TType list -> ExprValueInfo
    [<LiteralAttribute>]
    val localVarSize: int = 1
    val AddTotalSizes: l:Summary<'a> list -> int
    val AddFunctionSizes: l:Summary<'a> list -> int
    val OrEffects: l:Summary<'a> list -> bool
    val OrTailcalls: l:Summary<'a> list -> bool
    val OptimizeList: f:('a -> 'b * 'c) -> l:'a list -> 'b list * 'c list
    val NoExprs: TypedTree.Expr list * Summary<ExprValueInfo> list
    val CombineValueInfos: einfos:Summary<'a> list -> res:'b -> Summary<'b>
    val CombineValueInfosUnknown:
      einfos:Summary<'a> list -> Summary<ExprValueInfo>
    val AbstractLazyModulInfoByHiding:
      isAssemblyBoundary:bool ->
        mhi:TypedTreeOps.SignatureHidingInfo ->
          (LazyModuleInfo -> LazyModuleInfo)
    val AbstractOptimizationInfoToEssentials:
      (System.Lazy<ModuleInfo> -> System.Lazy<ModuleInfo>)
    val AbstractExprInfoByVars:
      boundVars:TypedTree.Val list * boundTyVars:TypedTree.Typar list ->
        ivalue:ExprValueInfo -> ExprValueInfo
    val RemapOptimizationInfo:
      TcGlobals.TcGlobals ->
        TypedTreeOps.Remap -> (LazyModuleInfo -> LazyModuleInfo)
    val AbstractAndRemapModulInfo:
      msg:string ->
        g:TcGlobals.TcGlobals ->
          m:Range.range ->
            repackage:TypedTreeOps.SignatureRepackageInfo *
            hidden:TypedTreeOps.SignatureHidingInfo ->
              info:LazyModuleInfo -> LazyModuleInfo
    [<LiteralAttribute>]
    val suffixForVariablesThatMayNotBeEliminated: string = "$cont"
    val IsTyFuncValRefExpr: _arg1:TypedTree.Expr -> bool
    val IsSmallConstExpr: x:TypedTree.Expr -> bool
    val ValueOfExpr: expr:TypedTree.Expr -> ExprValueInfo
    val IsDiscardableEffectExpr: expr:TypedTree.Expr -> bool
    val ValueIsUsedOrHasEffect:
      cenv:cenv ->
        fvs:(unit -> AbstractIL.Internal.Zset<TypedTree.Val>) ->
          b:TypedTree.Binding * binfo:Summary<'a> -> bool
    val SplitValuesByIsUsedOrHasEffect:
      cenv:cenv ->
        fvs:(unit -> AbstractIL.Internal.Zset<TypedTree.Val>) ->
          x:(TypedTree.Binding * Summary<'a>) list ->
            TypedTree.Binding list * Summary<'a> list
    val IlAssemblyCodeInstrHasEffect: i:AbstractIL.IL.ILInstr -> bool
    val IlAssemblyCodeHasEffect: instrs:AbstractIL.IL.ILInstr list -> bool
    val ExprHasEffect: TcGlobals.TcGlobals -> TypedTree.Expr -> bool
    val ExprsHaveEffect: g:TcGlobals.TcGlobals -> exprs:TypedTree.Exprs -> bool
    val BindingsHaveEffect:
      g:TcGlobals.TcGlobals -> binds:TypedTree.Bindings -> bool
    val BindingHasEffect:
      g:TcGlobals.TcGlobals -> bind:TypedTree.Binding -> bool
    val OpHasEffect:
      g:TcGlobals.TcGlobals -> m:Range.range -> op:TypedTree.TOp -> bool
    val TryEliminateBinding:
      cenv:cenv ->
        _env:'a ->
          TypedTree.Binding ->
            e2:TypedTree.Expr -> _m:'b -> TypedTree.Expr option
    val TryEliminateLet:
      cenv:cenv ->
        env:'a ->
          bind:TypedTree.Binding ->
            e2:TypedTree.Expr -> m:Range.range -> TypedTree.Expr * int
    val ( |KnownValApp|_| ):
      expr:TypedTree.Expr ->
        (TypedTree.ValRef * TypedTree.TType list * TypedTree.Expr list) option
    val ( |TDBoolSwitch|_| ):
      dtree:TypedTree.DecisionTree ->
        (TypedTree.Expr * bool * TypedTree.DecisionTree * TypedTree.DecisionTree *
         Range.range) option
    val ( |ConstantBoolTarget|_| ):
      target:TypedTree.DecisionTreeTarget -> bool option
    val CountBoolLogicTree:
      TypedTree.DecisionTreeTarget [] * int * int * bool ->
        tree:TypedTree.DecisionTree -> int * int
    val RewriteBoolLogicTree:
      TypedTree.DecisionTreeTarget [] * TypedTree.DecisionTree *
      TypedTree.DecisionTree * bool ->
        tree:TypedTree.DecisionTree -> TypedTree.DecisionTree
    val RewriteBoolLogicCase:
      TypedTree.DecisionTreeTarget [] * TypedTree.DecisionTree *
      TypedTree.DecisionTree * bool ->
        TypedTree.DecisionTreeCase -> TypedTree.DecisionTreeCase
    val CombineBoolLogic: expr:TypedTree.Expr -> TypedTree.Expr
    val CanExpandStructuralBinding: v:TypedTree.Val -> bool
    val ExprIsValue: _arg1:TypedTree.Expr -> bool
    val MakeStructuralBindingTemp:
      v:TypedTree.Val ->
        i:'a ->
          arg:TypedTree.Expr ->
            argTy:TypedTree.TType -> TypedTree.Expr * TypedTree.Binding
    val ExpandStructuralBindingRaw:
      cenv:cenv -> expr:TypedTree.Expr -> TypedTree.Expr
    val RearrangeTupleBindings:
      expr:TypedTree.Expr ->
        fin:(TypedTree.Expr -> TypedTree.Expr) -> TypedTree.Expr option
    val ExpandStructuralBinding:
      cenv:cenv -> expr:TypedTree.Expr -> TypedTree.Expr
    val ( |QueryRun|_| ):
      g:TcGlobals.TcGlobals ->
        expr:TypedTree.Expr -> (TypedTree.Expr * TypedTree.TType option) option
    val ( |MaybeRefTupled| ): e:TypedTree.Expr -> TypedTree.Exprs
    val ( |AnyInstanceMethodApp|_| ):
      e:TypedTree.Expr ->
        (TypedTree.ValRef * TypedTree.TypeInst * TypedTree.Expr *
         TypedTree.Exprs) option
    val ( |InstanceMethodApp|_| ):
      g:TcGlobals.TcGlobals ->
        expectedValRef:TypedTree.ValRef ->
          e:TypedTree.Expr ->
            (TypedTree.TypeInst * TypedTree.Expr * TypedTree.Exprs) option
    val ( |QuerySourceEnumerable|_| ):
      g:TcGlobals.TcGlobals ->
        _arg1:TypedTree.Expr -> (TypedTree.TType * TypedTree.Expr) option
    val ( |QueryFor|_| ):
      g:TcGlobals.TcGlobals ->
        _arg1:TypedTree.Expr ->
          (TypedTree.TType * TypedTree.TType * TypedTree.TType * TypedTree.Expr *
           TypedTree.Expr) option
    val ( |QueryYield|_| ):
      g:TcGlobals.TcGlobals ->
        _arg1:TypedTree.Expr ->
          (TypedTree.TType * TypedTree.TType * TypedTree.Expr) option
    val ( |QueryYieldFrom|_| ):
      g:TcGlobals.TcGlobals ->
        _arg1:TypedTree.Expr ->
          (TypedTree.TType * TypedTree.TType * TypedTree.Expr) option
    val ( |QuerySelect|_| ):
      g:TcGlobals.TcGlobals ->
        _arg1:TypedTree.Expr ->
          (TypedTree.TType * TypedTree.TType * TypedTree.TType * TypedTree.Expr *
           TypedTree.Expr) option
    val ( |QueryZero|_| ):
      g:TcGlobals.TcGlobals ->
        _arg1:TypedTree.Expr -> (TypedTree.TType * TypedTree.TType) option
    val ( |AnyRefTupleTrans| ):
      e:TypedTree.Expr -> TypedTree.Exprs * (TypedTree.Exprs -> TypedTree.Expr)
    val ( |AnyQueryBuilderOpTrans|_| ):
      g:TcGlobals.TcGlobals ->
        _arg1:TypedTree.Expr ->
          (TypedTree.Expr * (TypedTree.Expr -> TypedTree.Expr)) option
    val tryRewriteToSeqCombinators:
      g:TcGlobals.TcGlobals -> e:TypedTree.Expr -> TypedTree.Expr option
    val TryDetectQueryQuoteAndRun:
      cenv:cenv -> expr:TypedTree.Expr -> TypedTree.Expr option
    val IsILMethodRefSystemStringConcat: mref:AbstractIL.IL.ILMethodRef -> bool
    val IsILMethodRefSystemStringConcatArray:
      mref:AbstractIL.IL.ILMethodRef -> bool
    val OptimizeExpr:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          expr:TypedTree.Expr -> TypedTree.Expr * Summary<ExprValueInfo>
    val OptimizeObjectExpr:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          ty:TypedTree.TType * baseValOpt:TypedTree.Val option *
          basecall:TypedTree.Expr * overrides:TypedTree.ObjExprMethod list *
          iimpls:(TypedTree.TType * TypedTree.ObjExprMethod list) list *
          m:Range.range -> TypedTree.Expr * Summary<ExprValueInfo>
    val OptimizeMethods:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          baseValOpt:TypedTree.Val option ->
            methods:TypedTree.ObjExprMethod list ->
              TypedTree.ObjExprMethod list * Summary<ExprValueInfo> list
    val OptimizeMethod:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          baseValOpt:TypedTree.Val option ->
            TypedTree.ObjExprMethod ->
              TypedTree.ObjExprMethod * Summary<ExprValueInfo>
    val OptimizeInterfaceImpls:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          baseValOpt:TypedTree.Val option ->
            iimpls:(TypedTree.TType * TypedTree.ObjExprMethod list) list ->
              (TypedTree.TType * TypedTree.ObjExprMethod list) list *
              Summary<ExprValueInfo> list
    val OptimizeInterfaceImpl:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          baseValOpt:TypedTree.Val option ->
            ty:TypedTree.TType * overrides:TypedTree.ObjExprMethod list ->
              (TypedTree.TType * TypedTree.ObjExprMethod list) *
              Summary<ExprValueInfo>
    val MakeOptimizedSystemStringConcatCall:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          m:Range.range ->
            args:TypedTree.Exprs -> TypedTree.Expr * Summary<ExprValueInfo>
    val OptimizeExprOp:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          op:TypedTree.TOp * tyargs:TypedTree.TypeInst * args:TypedTree.Exprs *
          m:Range.range -> TypedTree.Expr * Summary<ExprValueInfo>
    val OptimizeExprOpReductions:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          op:TypedTree.TOp * tyargs:TypedTree.TypeInst * args:TypedTree.Exprs *
          m:Range.range -> TypedTree.Expr * Summary<ExprValueInfo>
    val OptimizeExprOpReductionsAfter:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          op:TypedTree.TOp * tyargs:TypedTree.TypeInst * argsR:TypedTree.Exprs *
          arginfos:Summary<ExprValueInfo> list * m:Range.range ->
            TypedTree.Expr * Summary<ExprValueInfo>
    val OptimizeExprOpFallback:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          op:TypedTree.TOp * tyargs:TypedTree.TypeInst * argsR:TypedTree.Exprs *
          m:Range.range ->
            arginfos:Summary<ExprValueInfo> list ->
              valu:ExprValueInfo -> TypedTree.Expr * Summary<ExprValueInfo>
    val OptimizeConst:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          expr:TypedTree.Expr ->
            c:TypedTree.Const * m:Range.range * ty:TypedTree.TType ->
              TypedTree.Expr * Summary<ExprValueInfo>
    val TryOptimizeRecordFieldGet:
      cenv:cenv ->
        _env:IncrementalOptimizationEnv ->
          e1info:Summary<ExprValueInfo> * TypedTree.RecdFieldRef *
          _tinst:TypedTree.TypeInst * m:Range.range -> ExprValueInfo option
    val TryOptimizeTupleFieldGet:
      cenv:cenv ->
        _env:IncrementalOptimizationEnv ->
          _tupInfo:TypedTree.TupInfo * e1info:Summary<ExprValueInfo> *
          tys:TypedTree.TypeInst * n:int * m:Range.range -> ExprValueInfo option
    val TryOptimizeUnionCaseGet:
      cenv:cenv ->
        _env:IncrementalOptimizationEnv ->
          e1info:Summary<ExprValueInfo> * cspec:TypedTree.UnionCaseRef *
          _tys:TypedTree.TypeInst * n:int * m:Range.range ->
            ExprValueInfo option
    val OptimizeFastIntegerForLoop:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          spStart:SyntaxTree.DebugPointAtFor * v:TypedTree.Val *
          e1:TypedTree.Expr * dir:TypedTree.ForLoopStyle * e2:TypedTree.Expr *
          e3:TypedTree.Expr * m:Range.range ->
            TypedTree.Expr * Summary<ExprValueInfo>
    val OptimizeLetRec:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          binds:TypedTree.Bindings * bodyExpr:TypedTree.Expr * m:Range.range ->
            TypedTree.Expr * Summary<ExprValueInfo>
    val OptimizeLinearExpr:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          expr:TypedTree.Expr ->
            contf:(TypedTree.Expr * Summary<ExprValueInfo> ->
                     TypedTree.Expr * Summary<ExprValueInfo>) ->
              TypedTree.Expr * Summary<ExprValueInfo>
    val OptimizeTryFinally:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          spTry:SyntaxTree.DebugPointAtTry *
          spFinally:SyntaxTree.DebugPointAtFinally * e1:TypedTree.Expr *
          e2:TypedTree.Expr * m:Range.range * ty:TypedTree.TType ->
            TypedTree.Expr * Summary<ExprValueInfo>
    val OptimizeTryWith:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          e1:TypedTree.Expr * vf:TypedTree.Val * ef:TypedTree.Expr *
          vh:TypedTree.Val * eh:TypedTree.Expr * m:Range.range *
          ty:TypedTree.TType * spTry:SyntaxTree.DebugPointAtTry *
          spWith:SyntaxTree.DebugPointAtWith ->
            TypedTree.Expr * Summary<ExprValueInfo>
    val OptimizeWhileLoop:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          spWhile:SyntaxTree.DebugPointAtWhile *
          marker:TypedTree.SpecialWhileLoopMarker * e1:TypedTree.Expr *
          e2:TypedTree.Expr * m:Range.range ->
            TypedTree.Expr * Summary<ExprValueInfo>
    val OptimizeTraitCall:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          traitInfo:TypedTree.TraitConstraintInfo * args:TypedTree.Exprs *
          m:Range.range -> TypedTree.Expr * Summary<ExprValueInfo>
    val TryOptimizeVal:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          mustInline:bool * valInfoForVal:ExprValueInfo * m:Range.range ->
            TypedTree.Expr option
    val TryOptimizeValInfo:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          m:Range.range -> vinfo:Summary<ExprValueInfo> -> TypedTree.Expr option
    val AddValEqualityInfo:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          v:TypedTree.ValRef ->
            info:Summary<ExprValueInfo> -> Summary<ExprValueInfo>
    val OptimizeVal:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          expr:TypedTree.Expr ->
            v:TypedTree.ValRef * m:Range.range ->
              TypedTree.Expr * Summary<ExprValueInfo>
    val StripToNominalTyconRef:
      cenv:cenv -> ty:TypedTree.TType -> TypedTree.TyconRef * TypedTree.TypeInst
    val CanDevirtualizeApplication:
      cenv:cenv ->
        v:TypedTree.ValRef ->
          vref:TypedTree.ValRef -> ty:TypedTree.TType -> args:'a list -> bool
    val TakeAddressOfStructArgumentIfNeeded:
      cenv:cenv ->
        vref:TypedTree.ValRef ->
          ty:TypedTree.TType ->
            args:TypedTree.Expr list ->
              m:Range.range ->
                (TypedTree.Expr -> TypedTree.Expr) * TypedTree.Expr list
    val DevirtualizeApplication:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          vref:TypedTree.ValRef ->
            ty:TypedTree.TType ->
              tyargs:TypedTree.TType list ->
                args:TypedTree.Expr list ->
                  m:Range.range -> TypedTree.Expr * Summary<ExprValueInfo>
    val TryDevirtualizeApplication:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          f:TypedTree.Expr * tyargs:TypedTree.TType list *
          args:TypedTree.Expr list * m:Range.range ->
            (TypedTree.Expr * Summary<ExprValueInfo>) option
    val TryInlineApplication:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          finfo:Summary<ExprValueInfo> ->
            tyargs:TypedTree.TType list * args:TypedTree.Expr list *
            m:Range.range -> (TypedTree.Expr * Summary<ExprValueInfo>) option
    val OptimizeApplication:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          f0:TypedTree.Expr * f0ty:TypedTree.TType * tyargs:TypedTree.TypeInst *
          args:TypedTree.Exprs * m:Range.range ->
            TypedTree.Expr * Summary<ExprValueInfo>
    val OptimizeLambdas:
      vspec:TypedTree.Val option ->
        cenv:cenv ->
          env:IncrementalOptimizationEnv ->
            topValInfo:TypedTree.ValReprInfo ->
              e:TypedTree.Expr ->
                ety:TypedTree.TType -> TypedTree.Expr * Summary<ExprValueInfo>
    val OptimizeExprsThenReshapeAndConsiderSplits:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          exprs:(ExprValueInfo * TypedTree.Expr) list ->
            TypedTree.Exprs * Summary<ExprValueInfo> list
    val OptimizeExprsThenConsiderSplits:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          exprs:TypedTree.Exprs -> TypedTree.Exprs * Summary<ExprValueInfo> list
    val OptimizeExprThenReshapeAndConsiderSplit:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          shape:ExprValueInfo * e:TypedTree.Expr ->
            TypedTree.Expr * Summary<ExprValueInfo>
    val OptimizeDecisionTreeTargets:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          m:Range.range ->
            targets:TypedTree.DecisionTreeTarget [] ->
              TypedTree.DecisionTreeTarget list * Summary<ExprValueInfo> list
    val ReshapeExpr:
      cenv:cenv -> shape:ExprValueInfo * e:TypedTree.Expr -> TypedTree.Expr
    val OptimizeExprThenConsiderSplit:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          e:TypedTree.Expr -> TypedTree.Expr * Summary<ExprValueInfo>
    val ComputeSplitToMethodCondition:
      flag:bool ->
        threshold:int ->
          cenv:cenv ->
            env:IncrementalOptimizationEnv ->
              e:TypedTree.Expr * einfo:Summary<'b> -> bool
    val ConsiderSplitToMethod:
      flag:bool ->
        threshold:int ->
          cenv:cenv ->
            env:IncrementalOptimizationEnv ->
              e:TypedTree.Expr * einfo:Summary<ExprValueInfo> ->
                TypedTree.Expr * Summary<ExprValueInfo>
    val OptimizeMatch:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          spMatch:SyntaxTree.DebugPointForBinding * exprm:Range.range *
          dtree:TypedTree.DecisionTree *
          targets:TypedTree.DecisionTreeTarget array * m:Range.range *
          ty:TypedTree.TType -> TypedTree.Expr * Summary<ExprValueInfo>
    val OptimizeMatchPart2:
      cenv:cenv ->
        spMatch:SyntaxTree.DebugPointForBinding * exprm:Range.range *
        dtreeR:TypedTree.DecisionTree *
        targetsR:TypedTree.DecisionTreeTarget list *
        dinfo:Summary<ExprValueInfo> * tinfos:Summary<ExprValueInfo> list *
        m:Range.range * ty:TypedTree.TType ->
          TypedTree.Expr * Summary<ExprValueInfo>
    val CombineMatchInfos:
      dinfo:Summary<'c> -> tinfo:Summary<'d> -> Summary<ExprValueInfo>
    val RebuildOptimizedMatch:
      spMatch:SyntaxTree.DebugPointForBinding * exprm:Range.range *
      m:Range.range * ty:TypedTree.TType * dtree:TypedTree.DecisionTree *
      tgs:TypedTree.DecisionTreeTarget list * dinfo:Summary<ExprValueInfo> *
      tinfos:Summary<ExprValueInfo> list ->
        TypedTree.Expr * Summary<ExprValueInfo>
    val OptimizeDecisionTreeTarget:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          _m:Range.range ->
            TypedTree.DecisionTreeTarget ->
              TypedTree.DecisionTreeTarget * Summary<ExprValueInfo>
    val OptimizeDecisionTree:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          m:Range.range ->
            x:TypedTree.DecisionTree ->
              TypedTree.DecisionTree * Summary<ExprValueInfo>
    val TryOptimizeDecisionTreeTest:
      cenv:cenv ->
        test:TypedTree.DecisionTreeTest -> vinfo:ExprValueInfo -> bool option
    val OptimizeSwitch:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          e:TypedTree.Expr * cases:TypedTree.DecisionTreeCase list *
          dflt:TypedTree.DecisionTree option * m:Range.range ->
            TypedTree.DecisionTree * Summary<ExprValueInfo>
    val OptimizeSwitchFallback:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          eR:TypedTree.Expr * einfo:Summary<ExprValueInfo> *
          cases:TypedTree.DecisionTreeCase list *
          dflt:TypedTree.DecisionTree option * m:Range.range ->
            TypedTree.DecisionTree * Summary<ExprValueInfo>
    val OptimizeBinding:
      cenv:cenv ->
        isRec:bool ->
          env:IncrementalOptimizationEnv ->
            TypedTree.Binding ->
              (TypedTree.Binding * Summary<ExprValueInfo>) *
              IncrementalOptimizationEnv
    val OptimizeBindings:
      cenv:cenv ->
        isRec:bool ->
          env:IncrementalOptimizationEnv ->
            xs:TypedTree.Bindings ->
              (TypedTree.Binding * Summary<ExprValueInfo>) list *
              IncrementalOptimizationEnv
    val OptimizeModuleExpr:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          x:TypedTree.ModuleOrNamespaceExprWithSig ->
            TypedTree.ModuleOrNamespaceExprWithSig * LazyModuleInfo
    val mkValBind: bind:TypedTree.Binding -> info:'e -> TypedTree.ValRef * 'e
    val OptimizeModuleDef:
      cenv:cenv ->
        env:IncrementalOptimizationEnv *
        bindInfosColl:(TypedTree.Binding * Summary<ExprValueInfo>) list list ->
          x:TypedTree.ModuleOrNamespaceExpr ->
            (TypedTree.ModuleOrNamespaceExpr * System.Lazy<ModuleInfo>) *
            (IncrementalOptimizationEnv *
             (TypedTree.Binding * Summary<ExprValueInfo>) list list)
    val OptimizeModuleBindings:
      cenv:cenv ->
        env:IncrementalOptimizationEnv *
        bindInfosColl:(TypedTree.Binding * Summary<ExprValueInfo>) list list ->
          xs:TypedTree.ModuleOrNamespaceBinding list ->
            (TypedTree.ModuleOrNamespaceBinding *
             Choice<(TypedTree.Binding * Summary<ExprValueInfo>),
                    (string * LazyModuleInfo)>) list *
            (IncrementalOptimizationEnv *
             (TypedTree.Binding * Summary<ExprValueInfo>) list list)
    val OptimizeModuleBinding:
      cenv:cenv ->
        env:IncrementalOptimizationEnv *
        bindInfosColl:(TypedTree.Binding * Summary<ExprValueInfo>) list list ->
          x:TypedTree.ModuleOrNamespaceBinding ->
            (TypedTree.ModuleOrNamespaceBinding *
             Choice<(TypedTree.Binding * Summary<ExprValueInfo>),
                    (string * LazyModuleInfo)>) *
            (IncrementalOptimizationEnv *
             (TypedTree.Binding * Summary<ExprValueInfo>) list list)
    val OptimizeModuleDefs:
      cenv:cenv ->
        env:IncrementalOptimizationEnv *
        bindInfosColl:(TypedTree.Binding * Summary<ExprValueInfo>) list list ->
          defs:TypedTree.ModuleOrNamespaceExpr list ->
            (TypedTree.ModuleOrNamespaceExpr list * System.Lazy<ModuleInfo>) *
            (IncrementalOptimizationEnv *
             (TypedTree.Binding * Summary<ExprValueInfo>) list list)
    val OptimizeImplFileInternal:
      cenv:cenv ->
        env:IncrementalOptimizationEnv ->
          isIncrementalFragment:bool ->
            hidden:TypedTreeOps.SignatureHidingInfo ->
              TypedTree.TypedImplFile ->
                IncrementalOptimizationEnv * TypedTree.TypedImplFile *
                LazyModuleInfo * TypedTreeOps.SignatureHidingInfo
    val OptimizeImplFile:
      OptimizationSettings * TypedTree.CcuThunk * TcGlobals.TcGlobals *
      ConstraintSolver.TcValF * Import.ImportMap * IncrementalOptimizationEnv *
      isIncrementalFragment:bool * emitTailcalls:bool *
      TypedTreeOps.SignatureHidingInfo * TypedTree.TypedImplFile ->
        (IncrementalOptimizationEnv * TypedTree.TypedImplFile * LazyModuleInfo *
         TypedTreeOps.SignatureHidingInfo) *
        (bool -> TypedTree.Expr -> TypedTree.Expr)
    val p_ExprValueInfo:
      x:ExprValueInfo -> st:TypedTreePickle.WriterState -> unit
    val p_ValInfo: v:ValInfo -> st:TypedTreePickle.WriterState -> unit
    val p_ModuleInfo: x:ModuleInfo -> st:TypedTreePickle.WriterState -> unit
    val p_LazyModuleInfo:
      x:LazyModuleInfo -> st:TypedTreePickle.WriterState -> unit
    val p_CcuOptimizationInfo:
      LazyModuleInfo -> TypedTreePickle.WriterState -> unit
    val u_ExprInfo: st:TypedTreePickle.ReaderState -> ExprValueInfo
    val u_ValInfo: st:TypedTreePickle.ReaderState -> ValInfo
    val u_ModuleInfo: st:TypedTreePickle.ReaderState -> ModuleInfo
    val u_LazyModuleInfo: st:TypedTreePickle.ReaderState -> LazyModuleInfo
    val u_CcuOptimizationInfo: TypedTreePickle.ReaderState -> LazyModuleInfo


namespace FSharp.Compiler
  module internal Detuple =
    val ( |TyappAndApp|_| ):
      e:TypedTree.Expr ->
        (TypedTree.Expr * TypedTree.TType * TypedTree.TType list *
         TypedTree.Exprs * Range.range) option
    module GlobalUsageAnalysis =
      val bindAccBounds:
        vals:AbstractIL.Internal.Zset<'a> ->
          _isInDTree:'b * v:'a -> AbstractIL.Internal.Zset<'a>
      val GetValsBoundInExpr:
        TypedTree.Expr -> AbstractIL.Internal.Zset<TypedTree.Val>
      type accessor = | TupleGet of int * TypedTree.TType list
      type Results =
        { Uses:
            AbstractIL.Internal.Zmap<TypedTree.Val,
                                     (accessor list * TypedTree.TType list *
                                      TypedTree.Expr list) list>
          Defns: AbstractIL.Internal.Zmap<TypedTree.Val,TypedTree.Expr>
          DecisionTreeBindings: AbstractIL.Internal.Zset<TypedTree.Val>
          RecursiveBindings:
            AbstractIL.Internal.Zmap<TypedTree.Val,(bool * TypedTree.Vals)>
          TopLevelBindings: AbstractIL.Internal.Zset<TypedTree.Val>
          IterationIsAtTopLevel: bool }
      val z0: Results
      val logUse:
        f:TypedTree.Val ->
          accessor list * TypedTree.TType list * TypedTree.Expr list ->
            z:Results -> Results
      val logBinding: z:Results -> isInDTree:bool * v:TypedTree.Val -> Results
      val logNonRecBinding: z:Results -> bind:TypedTree.Binding -> Results
      val logRecBindings: z:Results -> binds:TypedTree.Bindings -> Results
      val foldUnderLambda:
        f:(Results -> 'a -> Results) -> z:Results -> x:'a -> Results
      val UsageFolders:
        g:TcGlobals.TcGlobals -> TypedTreeOps.ExprFolder<Results>
      val GetUsageInfoOfImplFile:
        TcGlobals.TcGlobals -> TypedTree.TypedImplFile -> Results
  
    val internalError: str:string -> 'a
    val mkLocalVal:
      m:Range.range ->
        name:string ->
          ty:TypedTree.TType ->
            topValInfo:TypedTree.ValReprInfo option -> TypedTree.Val
    type TupleStructure =
      | UnknownTS
      | TupleTS of TupleStructure list
    val ValReprInfoForTS: ts:TupleStructure -> TypedTree.ArgReprInfo list list
    val andTS: ts:TupleStructure -> tsB:TupleStructure -> TupleStructure
    val checkTS: _arg1:TupleStructure -> TupleStructure
    val uncheckedExprTS: expr:TypedTree.Expr -> TupleStructure
    val uncheckedTypeTS:
      g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> TupleStructure
    val exprTS: exprs:TypedTree.Expr -> TupleStructure
    val typeTS: g:TcGlobals.TcGlobals -> tys:TypedTree.TType -> TupleStructure
    val rebuildTS:
      g:TcGlobals.TcGlobals ->
        m:Range.range ->
          ts:TupleStructure -> vs:TypedTree.Val list -> TypedTree.Expr
    type CallPattern = TupleStructure list
    val callPatternOrder: (CallPattern -> CallPattern -> int)
    val argsCP: exprs:TypedTree.Expr list -> TupleStructure list
    val noArgsCP: 'a list
    val inline isTrivialCP: xs:'a list -> bool
    val minimalCallPattern:
      callPattern:TupleStructure list -> TupleStructure list
    val commonCallPattern:
      callPatterns:TupleStructure list list -> TupleStructure list
    val siteCP:
      _accessors:'a * _inst:'b * args:TypedTree.Expr list -> TupleStructure list
    val sitesCPs:
      sites:('a * 'b * TypedTree.Expr list) list -> TupleStructure list list
    type TransformedFormal =
      | SameArg
      | NewArgs of TypedTree.Val list * TypedTree.Expr
    type Transform =
      { transformCallPattern: CallPattern
        transformedFormals: TransformedFormal list
        transformedVal: TypedTree.Val }
    val mkTransform:
      g:TcGlobals.TcGlobals ->
        f:TypedTree.Val ->
          m:Range.range ->
            tps:TypedTree.Typar list ->
              x1Ntys:TypedTree.TType list ->
                rty:TypedTree.TType ->
                  callPattern:TupleStructure list *
                  tyfringes:(TypedTree.TType list * TypedTree.Val list) list ->
                    Transform
    val zipTupleStructureAndType:
      g:TcGlobals.TcGlobals ->
        ts:TupleStructure ->
          ty:TypedTree.TType -> TupleStructure * TypedTree.TType list
    val zipTupleStructuresAndTypes:
      g:TcGlobals.TcGlobals ->
        tss:TupleStructure list ->
          tys:TypedTree.TTypes -> TupleStructure list * TypedTree.TType list
    val zipCallPatternArgTys:
      m:Range.range ->
        g:TcGlobals.TcGlobals ->
          callPattern:TupleStructure list ->
            vss:TypedTree.Val list list ->
              TupleStructure list *
              (TypedTree.TType list * TypedTree.Val list) list
    val decideFormalSuggestedCP:
      g:TcGlobals.TcGlobals ->
        z:GlobalUsageAnalysis.Results ->
          tys:TypedTree.TType list ->
            vss:TypedTree.Val list list -> TupleStructure list
    val decideTransform:
      g:TcGlobals.TcGlobals ->
        z:GlobalUsageAnalysis.Results ->
          v:TypedTree.Val ->
            callPatterns:TupleStructure list list ->
              m:Range.range * tps:TypedTree.Typar list *
              vss:TypedTree.Val list list * rty:TypedTree.TType ->
                (TypedTree.Val * Transform) option
    val eligibleVal:
      g:TcGlobals.TcGlobals -> m:Range.range -> v:TypedTree.Val -> bool
    val determineTransforms:
      g:TcGlobals.TcGlobals ->
        z:GlobalUsageAnalysis.Results ->
          AbstractIL.Internal.Zmap<TypedTree.Val,Transform>
    type penv =
      { transforms: AbstractIL.Internal.Zmap<TypedTree.Val,Transform>
        ccu: TypedTree.CcuThunk
        g: TcGlobals.TcGlobals }
    val hasTransfrom: penv:penv -> f:TypedTree.Val -> Transform option
    type env =
      { eg: TcGlobals.TcGlobals
        prefix: string
        m: Range.range }
      with
        override ToString: unit -> string
    
    val suffixE: env:env -> s:string -> env
    val rangeE: env:env -> m:Range.range -> env
    val push: b:'a -> bs:'a list -> 'a list
    val pushL: xs:'a list -> bs:'a list -> 'a list
    val newLocal:
      env:env -> ty:TypedTree.TType -> TypedTree.Val * TypedTree.Expr
    val newLocalN:
      env:env -> i:'a -> ty:TypedTree.TType -> TypedTree.Val * TypedTree.Expr
    val noEffectExpr:
      env:env ->
        bindings:TypedTree.Binding list ->
          x:TypedTree.Expr -> TypedTree.Binding list * TypedTree.Expr
    val buildProjections:
      env:env ->
        bindings:TypedTree.Binding list ->
          x:TypedTree.Expr ->
            xtys:TypedTree.TType list ->
              TypedTree.Binding list * TypedTree.Expr list
    val collapseArg:
      env:env ->
        bindings:TypedTree.Binding list ->
          ts:TupleStructure ->
            x:TypedTree.Expr -> TypedTree.Binding list * TypedTree.Expr list
    val collapseArgs:
      env:env ->
        bindings:TypedTree.Binding list ->
          n:int ->
            callPattern:TupleStructure list ->
              args:TypedTree.Exprs ->
                TypedTree.Binding list * TypedTree.Expr list
    val mkLets:
      binds:TypedTree.Binding list -> body:TypedTree.Expr -> TypedTree.Expr
    val fixupApp:
      penv:penv ->
        fx:TypedTree.Expr * fty:TypedTree.TType * tys:TypedTree.TypeInst *
        args:TypedTree.Exprs * m:Range.range -> TypedTree.Expr
    val transFormal:
      ybi:TransformedFormal -> xi:TypedTree.Val list -> TypedTree.Val list list
    val transRebind:
      ybi:TransformedFormal -> xi:TypedTree.Val list -> TypedTree.Binding list
    val passBind: penv:penv -> TypedTree.Binding -> TypedTree.Binding
    val passBinds:
      penv:penv -> binds:TypedTree.Binding list -> TypedTree.Binding list
    val passBindRhs:
      conv:(TypedTree.Expr -> TypedTree.Expr) ->
        TypedTree.Binding -> TypedTree.Binding
    val preInterceptExpr:
      penv:penv ->
        conv:(TypedTree.Expr -> TypedTree.Expr) ->
          expr:TypedTree.Expr -> TypedTree.Expr option
    val postTransformExpr:
      penv:penv -> expr:TypedTree.Expr -> TypedTree.Expr option
    val passImplFile:
      penv:penv -> assembly:TypedTree.TypedImplFile -> TypedTree.TypedImplFile
    val DetupleImplFile:
      TypedTree.CcuThunk ->
        TcGlobals.TcGlobals ->
          TypedTree.TypedImplFile -> TypedTree.TypedImplFile


namespace FSharp.Compiler
  module internal InnerLambdasToTopLevelFuncs =
    val verboseTLR: bool
    val internalError: str:string -> 'a
    module Zmap =
      val force:
        k:'a ->
          mp:AbstractIL.Internal.Zmap<'a,'b> ->
            str:string * soK:('a -> string) -> 'b
  
    type Tree<'T> =
      | TreeNode of Tree<'T> list
      | LeafNode of 'T
    val fringeTR: tr:Tree<'a> -> 'a list
    val emptyTR: Tree<'a>
    val destApp:
      f:TypedTree.Expr * fty:TypedTree.TType * tys:TypedTree.TType list *
      args:'a * m:'b ->
        TypedTree.Expr * TypedTree.TType * TypedTree.TType list * 'a * 'b
    val showTyparSet: tps:AbstractIL.Internal.Zset<TypedTree.Typar> -> string
    val isDelayedRepr: f:TypedTree.Val -> e:TypedTree.Expr -> bool
    val mkLocalNameTypeArity:
      compgen:bool ->
        m:Range.range ->
          name:string ->
            ty:TypedTree.TType ->
              topValInfo:TypedTree.ValReprInfo option -> TypedTree.Val
    val GetValsBoundUnderMustInline:
      xinfo:Detuple.GlobalUsageAnalysis.Results ->
        AbstractIL.Internal.Zset<TypedTree.Val>
    val IsRefusedTLR: g:TcGlobals.TcGlobals -> f:TypedTree.Val -> bool
    val IsMandatoryTopLevel: f:TypedTree.Val -> bool
    val IsMandatoryNonTopLevel:
      g:TcGlobals.TcGlobals -> f:TypedTree.Val -> bool
    module Pass1_DetermineTLRAndArities =
      val GetMaxNumArgsAtUses:
        xinfo:Detuple.GlobalUsageAnalysis.Results -> f:TypedTree.Val -> int
      val SelectTLRVals:
        g:TcGlobals.TcGlobals ->
          xinfo:Detuple.GlobalUsageAnalysis.Results ->
            f:TypedTree.Val -> e:TypedTree.Expr -> (TypedTree.Val * int) option
      val IsValueRecursionFree:
        xinfo:Detuple.GlobalUsageAnalysis.Results -> f:TypedTree.Val -> bool
      val DumpArity: arityM:AbstractIL.Internal.Zmap<TypedTree.Val,int> -> unit
      val DetermineTLRAndArities:
        g:TcGlobals.TcGlobals ->
          expr:TypedTree.TypedImplFile ->
            AbstractIL.Internal.Zset<TypedTree.Val> *
            AbstractIL.Internal.Zset<TypedTree.Val> *
            AbstractIL.Internal.Zmap<TypedTree.Val,int>
  
    type BindingGroupSharingSameReqdItems =
  
        new: bindings:TypedTree.Bindings -> BindingGroupSharingSameReqdItems
        member Contains: v:TypedTree.Val -> bool
        override ToString: unit -> string
        member IsEmpty: bool
        member Pairs: (TypedTree.Val * BindingGroupSharingSameReqdItems) list
        member Vals: TypedTree.Vals
    
    val fclassOrder:
      System.Collections.Generic.IComparer<BindingGroupSharingSameReqdItems>
    type ReqdItem =
      | ReqdSubEnv of TypedTree.Val
      | ReqdVal of TypedTree.Val
      with
        override ToString: unit -> string
    
    val reqdItemOrder: System.Collections.Generic.IComparer<ReqdItem>
    type ReqdItemsForDefn =
      { reqdTypars: AbstractIL.Internal.Zset<TypedTree.Typar>
        reqdItems: AbstractIL.Internal.Zset<ReqdItem>
        m: Range.range }
      with
        static member
          Initial: typars:TypedTree.Typar list ->
                      m:Range.range -> ReqdItemsForDefn
        member
          Extend: typars:TypedTree.Typar list * items:ReqdItem list ->
                     ReqdItemsForDefn
        override ToString: unit -> string
        member ReqdSubEnvs: TypedTree.Val list
        member ReqdVals: TypedTree.Val list
    
    type Generators = AbstractIL.Internal.Zset<TypedTree.Val>
    val IsArityMet:
      vref:TypedTree.ValRef ->
        wf:int -> tys:TypedTree.TypeInst -> args:'a list -> bool
    module Pass2_DetermineReqdItems =
      type state =
        { stack:
            (BindingGroupSharingSameReqdItems * Generators * ReqdItemsForDefn) list
          reqdItemsMap:
            AbstractIL.Internal.Zmap<BindingGroupSharingSameReqdItems,
                                     ReqdItemsForDefn>
          fclassM:
            AbstractIL.Internal.Zmap<TypedTree.Val,
                                     BindingGroupSharingSameReqdItems>
          revDeclist: BindingGroupSharingSameReqdItems list
          recShortCallS: AbstractIL.Internal.Zset<TypedTree.Val> }
      val state0: state
      val PushFrame:
        fclass:BindingGroupSharingSameReqdItems ->
          reqdTypars0:TypedTree.Typar list * reqdVals0:Generators *
          m:Range.range -> state:state -> state
      val SaveFrame:
        fclass:BindingGroupSharingSameReqdItems -> state:state -> state
      val LogRequiredFrom:
        gv:TypedTree.Val -> items:ReqdItem list -> state:state -> state
      val LogShortCall: gv:TypedTree.Val -> state:state -> state
      val FreeInBindings: bs:TypedTree.Binding list -> TypedTree.FreeVars
      val ExprEnvIntercept:
        tlrS:AbstractIL.Internal.Zset<TypedTree.Val> *
        arityM:AbstractIL.Internal.Zmap<TypedTree.Val,int> ->
          recurseF:(state -> TypedTree.Expr -> state) ->
            noInterceptF:(state -> TypedTree.Expr -> state) ->
              z:state -> expr:TypedTree.Expr -> state
      val CloseReqdTypars:
        fclassM:AbstractIL.Internal.Zmap<TypedTree.Val,'a> ->
          reqdItemsMap:AbstractIL.Internal.Zmap<'a,ReqdItemsForDefn> ->
            AbstractIL.Internal.Zmap<'a,ReqdItemsForDefn>
      val DumpReqdValMap:
        reqdItemsMap:seq<System.Collections.Generic.KeyValuePair<'a,'b>> -> unit
      val DetermineReqdItems:
        tlrS:AbstractIL.Internal.Zset<TypedTree.Val> *
        arityM:AbstractIL.Internal.Zmap<TypedTree.Val,int> ->
          expr:TypedTree.TypedImplFile ->
            AbstractIL.Internal.Zmap<BindingGroupSharingSameReqdItems,
                                     ReqdItemsForDefn> *
            AbstractIL.Internal.Zmap<TypedTree.Val,
                                     BindingGroupSharingSameReqdItems> *
            BindingGroupSharingSameReqdItems list *
            AbstractIL.Internal.Zset<TypedTree.Val>
  
    type PackedReqdItems =
      { ep_etps: TypedTree.Typars
        ep_aenvs: TypedTree.Val list
        ep_pack: TypedTree.Bindings
        ep_unpack: TypedTree.Bindings }
    exception AbortTLR of Range.range
    val FlatEnvPacks:
      g:TcGlobals.TcGlobals ->
        fclassM:AbstractIL.Internal.Zmap<TypedTree.Val,
                                         BindingGroupSharingSameReqdItems> ->
          topValS:AbstractIL.Internal.Zset<TypedTree.Val> ->
            declist:#BindingGroupSharingSameReqdItems list ->
              reqdItemsMap:AbstractIL.Internal.Zmap<BindingGroupSharingSameReqdItems,
                                                    ReqdItemsForDefn> ->
                AbstractIL.Internal.Zmap<BindingGroupSharingSameReqdItems,
                                         PackedReqdItems>
    val DumpEnvPackM:
      g:TcGlobals.TcGlobals ->
        envPackM:seq<System.Collections.Generic.KeyValuePair<'a,PackedReqdItems>> ->
          unit
    val ChooseReqdItemPackings:
      g:TcGlobals.TcGlobals ->
        fclassM:AbstractIL.Internal.Zmap<TypedTree.Val,
                                         BindingGroupSharingSameReqdItems> ->
          topValS:AbstractIL.Internal.Zset<TypedTree.Val> ->
            declist:#BindingGroupSharingSameReqdItems list ->
              reqdItemsMap:AbstractIL.Internal.Zmap<BindingGroupSharingSameReqdItems,
                                                    ReqdItemsForDefn> ->
                AbstractIL.Internal.Zmap<BindingGroupSharingSameReqdItems,
                                         PackedReqdItems>
    val MakeSimpleArityInfo:
      tps:TypedTree.Typar list -> n:int -> TypedTree.ValReprInfo
    val CreateNewValuesForTLR:
      g:TcGlobals.TcGlobals ->
        tlrS:AbstractIL.Internal.Zset<TypedTree.Val> ->
          arityM:AbstractIL.Internal.Zmap<TypedTree.Val,int> ->
            fclassM:AbstractIL.Internal.Zmap<TypedTree.Val,'a> ->
              envPackM:AbstractIL.Internal.Zmap<'a,PackedReqdItems> ->
                AbstractIL.Internal.Zmap<TypedTree.Val,TypedTree.Val>
    module Pass4_RewriteAssembly =
      [<NoEquality; NoComparison>]
      type RewriteContext =
        { ccu: TypedTree.CcuThunk
          g: TcGlobals.TcGlobals
          tlrS: AbstractIL.Internal.Zset<TypedTree.Val>
          topValS: AbstractIL.Internal.Zset<TypedTree.Val>
          arityM: AbstractIL.Internal.Zmap<TypedTree.Val,int>
          fclassM:
            AbstractIL.Internal.Zmap<TypedTree.Val,
                                     BindingGroupSharingSameReqdItems>
          recShortCallS: AbstractIL.Internal.Zset<TypedTree.Val>
          envPackM:
            AbstractIL.Internal.Zmap<BindingGroupSharingSameReqdItems,
                                     PackedReqdItems>
          fHatM: AbstractIL.Internal.Zmap<TypedTree.Val,TypedTree.Val> }
      type IsRecursive =
        | IsRec
        | NotRec
      type LiftedDeclaration = IsRecursive * TypedTree.Bindings
      type RewriteState =
        { rws_mustinline: bool
          rws_innerLevel: int
          rws_preDecs: Tree<LiftedDeclaration> }
      val rewriteState0: RewriteState
      val EnterInner: z:RewriteState -> RewriteState
      val ExitInner: z:RewriteState -> RewriteState
      val EnterMustInline:
        b:bool ->
          z:RewriteState ->
            f:(RewriteState -> 'a * RewriteState) -> RewriteState * 'a
      val ExtractPreDecs:
        z:RewriteState -> LiftedDeclaration list * RewriteState
      val PopPreDecs: z:RewriteState -> RewriteState * Tree<LiftedDeclaration>
      val SetPreDecs:
        z:RewriteState -> pdt:Tree<LiftedDeclaration> -> RewriteState
      val LiftTopBinds: _isRec:'a -> _penv:'b -> z:'c -> binds:'d -> 'c * 'd
      val MakePreDec:
        m:Range.range ->
          isRec:IsRecursive * binds:TypedTree.Bindings ->
            expr:TypedTree.Expr -> TypedTree.Expr
      val MakePreDecs:
        m:Range.range ->
          preDecs:(IsRecursive * TypedTree.Bindings) list ->
            expr:TypedTree.Expr -> TypedTree.Expr
      val RecursivePreDecs:
        pdsA:Tree<'a * 'b list> ->
          pdsB:Tree<'a * 'b list> -> Tree<IsRecursive * 'b list>
      val ConvertBind:
        g:TcGlobals.TcGlobals -> TypedTree.Binding -> TypedTree.Binding
      val TransTLRBindings:
        penv:RewriteContext ->
          binds:TypedTree.Bindings ->
            TypedTree.Binding list * TypedTree.Binding list
      val GetAEnvBindings:
        penv:RewriteContext ->
          fc:BindingGroupSharingSameReqdItems -> TypedTree.Binding list
      val TransBindings:
        xisRec:IsRecursive ->
          penv:RewriteContext ->
            binds:TypedTree.Bindings ->
              TypedTree.Binding list * TypedTree.Binding list
      val TransApp:
        penv:RewriteContext ->
          fx:TypedTree.Expr * fty:TypedTree.TType * tys:TypedTree.TypeInst *
          args:TypedTree.Expr list * m:Range.range -> TypedTree.Expr
      val TransExpr:
        penv:RewriteContext ->
          z:RewriteState -> expr:TypedTree.Expr -> TypedTree.Expr * RewriteState
      val TransLinearExpr:
        penv:RewriteContext ->
          z:RewriteState ->
            expr:TypedTree.Expr ->
              contf:(TypedTree.Expr * RewriteState ->
                       TypedTree.Expr * RewriteState) ->
                TypedTree.Expr * RewriteState
      val TransMethod:
        penv:RewriteContext ->
          z:RewriteState ->
            TypedTree.ObjExprMethod -> TypedTree.ObjExprMethod * RewriteState
      val TransBindingRhs:
        penv:RewriteContext ->
          z:RewriteState ->
            TypedTree.Binding -> TypedTree.Binding * RewriteState
      val TransDecisionTree:
        penv:RewriteContext ->
          z:RewriteState ->
            x:TypedTree.DecisionTree -> TypedTree.DecisionTree * RewriteState
      val TransDecisionTreeTarget:
        penv:RewriteContext ->
          z:RewriteState ->
            TypedTree.DecisionTreeTarget ->
              TypedTree.DecisionTreeTarget * RewriteState
      val TransValBinding:
        penv:RewriteContext ->
          z:RewriteState ->
            bind:TypedTree.Binding -> TypedTree.Binding * RewriteState
      val TransValBindings:
        penv:RewriteContext ->
          z:RewriteState ->
            binds:TypedTree.Binding list ->
              TypedTree.Binding list * RewriteState
      val TransModuleExpr:
        penv:RewriteContext ->
          z:RewriteState ->
            x:TypedTree.ModuleOrNamespaceExprWithSig ->
              TypedTree.ModuleOrNamespaceExprWithSig * RewriteState
      val TransModuleDefs:
        penv:RewriteContext ->
          z:RewriteState ->
            x:TypedTree.ModuleOrNamespaceExpr list ->
              TypedTree.ModuleOrNamespaceExpr list * RewriteState
      val TransModuleDef:
        penv:RewriteContext ->
          z:RewriteState ->
            x:TypedTree.ModuleOrNamespaceExpr ->
              TypedTree.ModuleOrNamespaceExpr * RewriteState
      val TransModuleBindings:
        penv:RewriteContext ->
          z:RewriteState ->
            binds:TypedTree.ModuleOrNamespaceBinding list ->
              TypedTree.ModuleOrNamespaceBinding list * RewriteState
      val TransModuleBinding:
        penv:RewriteContext ->
          z:RewriteState ->
            x:TypedTree.ModuleOrNamespaceBinding ->
              TypedTree.ModuleOrNamespaceBinding * RewriteState
      val TransImplFile:
        penv:RewriteContext ->
          z:RewriteState ->
            TypedTree.TypedImplFile -> TypedTree.TypedImplFile * RewriteState
  
    val RecreateUniqueBounds:
      g:TcGlobals.TcGlobals ->
        expr:TypedTree.TypedImplFile -> TypedTree.TypedImplFile
    val MakeTLRDecisions:
      TypedTree.CcuThunk ->
        TcGlobals.TcGlobals ->
          TypedTree.TypedImplFile -> TypedTree.TypedImplFile


namespace FSharp.Compiler
  module internal LowerCallsAndSeqs =
    val InterceptExpr:
      g:TcGlobals.TcGlobals ->
        cont:(TypedTree.Expr -> TypedTree.Expr) ->
          expr:TypedTree.Expr -> TypedTree.Expr option
    val LowerImplFile:
      g:TcGlobals.TcGlobals ->
        assembly:TypedTree.TypedImplFile -> TypedTree.TypedImplFile
    val mkLambdaNoType:
      g:TcGlobals.TcGlobals ->
        m:Range.range -> uv:TypedTree.Val -> e:TypedTree.Expr -> TypedTree.Expr
    val callNonOverloadedMethod:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          m:Range.range ->
            methName:string ->
              ty:TypedTree.TType -> args:TypedTree.Exprs -> TypedTree.Expr
    type LoweredSeqFirstPhaseResult =
      { phase2:
          TypedTree.ValRef * TypedTree.ValRef * TypedTree.ValRef *
          Map<AbstractIL.IL.ILCodeLabel,int> ->
            TypedTree.Expr * TypedTree.Expr * TypedTree.Expr
        entryPoints: int list
        significantClose: bool
        stateVars: TypedTree.ValRef list
        asyncVars: TypedTree.FreeVars }
    val isVarFreeInExpr: v:TypedTree.Val -> e:TypedTree.Expr -> bool
    val ( |Seq|_| ):
      g:TcGlobals.TcGlobals ->
        expr:TypedTree.Expr -> (TypedTree.Expr * TypedTree.TType) option
    val IsPossibleSequenceExpr:
      g:TcGlobals.TcGlobals -> overallExpr:TypedTree.Expr -> bool
    val ConvertSequenceExprToObject:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          overallExpr:TypedTree.Expr ->
            (TypedTree.ValRef * TypedTree.ValRef * TypedTree.ValRef *
             TypedTree.ValRef list * TypedTree.Expr * TypedTree.Expr *
             TypedTree.Expr * TypedTree.TType * Range.range) option


namespace FSharp.Compiler
  module internal AutoBox =
    type cenv =
      { g: TcGlobals.TcGlobals
        amap: Import.ImportMap }
      with
        override ToString: unit -> string
    
    val DecideEscapes:
      syntacticArgs:TypedTree.Val list ->
        body:TypedTree.Expr -> AbstractIL.Internal.Zset<TypedTree.Val>
    val DecideLambda:
      exprF:(AbstractIL.Internal.Zset<TypedTree.Val> -> TypedTree.Expr ->
               AbstractIL.Internal.Zset<TypedTree.Val>) option ->
        cenv:cenv ->
          topValInfo:TypedTree.ValReprInfo ->
            expr:TypedTree.Expr ->
              ety:TypedTree.TType ->
                z:AbstractIL.Internal.Zset<TypedTree.Val> ->
                  AbstractIL.Internal.Zset<TypedTree.Val>
    val DecideExprOp:
      exprF:(AbstractIL.Internal.Zset<TypedTree.Val> -> TypedTree.Expr ->
               AbstractIL.Internal.Zset<TypedTree.Val>) ->
        noInterceptF:(AbstractIL.Internal.Zset<TypedTree.Val> ->
                        TypedTree.Expr ->
                        AbstractIL.Internal.Zset<TypedTree.Val>) ->
          z:AbstractIL.Internal.Zset<TypedTree.Val> ->
            expr:TypedTree.Expr ->
              op:TypedTree.TOp * tyargs:'a list * args:TypedTree.Expr list ->
                AbstractIL.Internal.Zset<TypedTree.Val>
    val DecideExpr:
      cenv:cenv ->
        exprF:(AbstractIL.Internal.Zset<TypedTree.Val> -> TypedTree.Expr ->
                 AbstractIL.Internal.Zset<TypedTree.Val>) ->
          noInterceptF:(AbstractIL.Internal.Zset<TypedTree.Val> ->
                          TypedTree.Expr ->
                          AbstractIL.Internal.Zset<TypedTree.Val>) ->
            z:AbstractIL.Internal.Zset<TypedTree.Val> ->
              expr:TypedTree.Expr -> AbstractIL.Internal.Zset<TypedTree.Val>
    val DecideBinding:
      cenv:cenv ->
        z:AbstractIL.Internal.Zset<TypedTree.Val> ->
          TypedTree.Binding -> AbstractIL.Internal.Zset<TypedTree.Val>
    val DecideBindings:
      cenv:cenv ->
        z:AbstractIL.Internal.Zset<TypedTree.Val> ->
          binds:TypedTree.Binding list ->
            AbstractIL.Internal.Zset<TypedTree.Val>
    val DecideImplFile:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          implFile:TypedTree.TypedImplFile ->
            AbstractIL.Internal.Zset<TypedTree.Val>
    val TransformExpr:
      g:TcGlobals.TcGlobals ->
        nvs:TypedTreeOps.ValMap<'a * TypedTree.Expr> ->
          exprF:(TypedTree.Expr -> TypedTree.Expr) ->
            expr:TypedTree.Expr -> TypedTree.Expr option
    val TransformBinding:
      g:TcGlobals.TcGlobals ->
        nvs:TypedTreeOps.ValMap<TypedTree.Val * 'a> ->
          exprF:(TypedTree.Expr -> TypedTree.Expr) ->
            TypedTree.Binding -> TypedTree.Binding option
    val TransformImplFile:
      g:TcGlobals.TcGlobals ->
        amap:Import.ImportMap ->
          implFile:TypedTree.TypedImplFile -> TypedTree.TypedImplFile


namespace FSharp.Compiler
  module internal IlxGen =
    val IsNonErasedTypar: tp:TypedTree.Typar -> bool
    val DropErasedTypars: tps:TypedTree.Typar list -> TypedTree.Typar list
    val DropErasedTyargs: tys:TypedTree.TType list -> TypedTree.TType list
    val AddNonUserCompilerGeneratedAttribs:
      g:TcGlobals.TcGlobals ->
        mdef:AbstractIL.IL.ILMethodDef -> AbstractIL.IL.ILMethodDef
    val debugDisplayMethodName: string
    val useHiddenInitCode: bool
    val iLdcZero: AbstractIL.IL.ILInstr
    val iLdcInt64: i:int64 -> AbstractIL.IL.ILInstr
    val iLdcDouble: i:double -> AbstractIL.IL.ILInstr
    val iLdcSingle: i:single -> AbstractIL.IL.ILInstr
    val mkLdfldMethodDef:
      ilMethName:string * reprAccess:AbstractIL.IL.ILMemberAccess *
      isStatic:bool * ilTy:AbstractIL.IL.ILType * ilFieldName:string *
      ilPropType:AbstractIL.IL.ILType -> AbstractIL.IL.ILMethodDef
    val ChooseParamNames:
      fieldNamesAndTypes:(string * string * 'a) list ->
        (string * string * 'a) list
    val CheckCodeDoesSomething: code:AbstractIL.IL.ILCode -> bool
    val ChooseFreeVarNames:
      takenNames:string list -> ts:string list -> string list
    val IsILTypeByref: _arg1:AbstractIL.IL.ILType -> bool
    val mainMethName: string
    type AttributeDecoder =
  
        new: namedArgs:TypedTree.AttribNamedArg list -> AttributeDecoder
        member FindBool: x:string -> dflt:bool -> bool
        member FindInt16: x:string -> dflt:int16 -> int16
        member FindInt32: x:string -> dflt:int32 -> int32
        member FindString: x:string -> dflt:string -> string
        member FindTypeName: x:string -> dflt:string -> string
    
    val mutable reports: (System.IO.TextWriter -> unit)
    val AddReport: f:(System.IO.TextWriter -> unit) -> unit
    val ReportStatistics: System.IO.TextWriter -> unit
    val NewCounter: nm:string -> (unit -> unit)
    val CountClosure: (unit -> unit)
    val CountMethodDef: (unit -> unit)
    val CountStaticFieldDef: (unit -> unit)
    val CountCallFuncInstructions: (unit -> unit)
    type IlxGenIntraAssemblyInfo =
      { StaticFieldInfo:
          System.Collections.Generic.Dictionary<AbstractIL.IL.ILMethodRef,
                                                AbstractIL.IL.ILFieldSpec> }
    type FakeUnit = | Fake
    type IlxGenBackend =
      | IlWriteBackend
      | IlReflectBackend
    [<NoEquality; NoComparison>]
    type IlxGenOptions =
      { fragName: string
        generateFilterBlocks: bool
        workAroundReflectionEmitBugs: bool
        emitConstantArraysUsingStaticDataBlobs: bool
        mainMethodInfo: TypedTree.Attribs option
        localOptimizationsAreOn: bool
        generateDebugSymbols: bool
        testFlagEmitFeeFeeAs100001: bool
        ilxBackend: IlxGenBackend
        isInteractive: bool
        isInteractiveItExpr: bool
        alwaysCallVirt: bool }
    [<NoEquality; NoComparison>]
    type cenv =
      { g: TcGlobals.TcGlobals
        amap: Import.ImportMap
        tcVal: ConstraintSolver.TcValF
        viewCcu: TypedTree.CcuThunk
        opts: IlxGenOptions
        mutable ilUnitTy: AbstractIL.IL.ILType option
        intraAssemblyInfo: IlxGenIntraAssemblyInfo
        casApplied: System.Collections.Generic.Dictionary<TypedTree.Stamp,bool>
        mutable optimizeDuringCodeGen: bool -> TypedTree.Expr -> TypedTree.Expr
        mutable exprRecursionDepth: int
        delayedGenMethods: System.Collections.Generic.Queue<(cenv -> unit)> }
      with
        override ToString: unit -> string
    
    val mkTypeOfExpr:
      cenv:cenv -> m:Range.range -> ilty:AbstractIL.IL.ILType -> TypedTree.Expr
    val mkGetNameExpr:
      cenv:cenv -> ilt:AbstractIL.IL.ILType -> m:Range.range -> TypedTree.Expr
    val useCallVirt:
      cenv:cenv ->
        boxity:AbstractIL.IL.ILBoxity ->
          mspec:AbstractIL.IL.ILMethodSpec -> isBaseCall:bool -> bool
    type CompileLocation =
      { Scope: AbstractIL.IL.ILScopeRef
        TopImplQualifiedName: string
        Namespace: string option
        Enclosing: string list
        QualifiedNameOfFile: string }
    val mkTopName: ns:string option -> n:string -> string
    val CompLocForFragment:
      fragName:string -> ccu:TypedTree.CcuThunk -> CompileLocation
    val CompLocForCcu: ccu:TypedTree.CcuThunk -> CompileLocation
    val CompLocForSubModuleOrNamespace:
      cloc:CompileLocation ->
        submod:TypedTree.ModuleOrNamespace -> CompileLocation
    val CompLocForFixedPath:
      fragName:string ->
        qname:string -> TypedTree.CompilationPath -> CompileLocation
    val CompLocForFixedModule:
      fragName:string ->
        qname:string -> mspec:TypedTree.ModuleOrNamespace -> CompileLocation
    val NestedTypeRefForCompLoc:
      cloc:CompileLocation -> n:string -> AbstractIL.IL.ILTypeRef
    val CleanUpGeneratedTypeName: nm:string -> string
    val TypeNameForInitClass: cloc:CompileLocation -> string
    val TypeNameForImplicitMainMethod: cloc:CompileLocation -> string
    val TypeNameForPrivateImplementationDetails: cloc:CompileLocation -> string
    val CompLocForInitClass: cloc:CompileLocation -> CompileLocation
    val CompLocForImplicitMainMethod: cloc:CompileLocation -> CompileLocation
    val CompLocForPrivateImplementationDetails:
      cloc:CompileLocation -> CompileLocation
    val TypeRefForCompLoc: cloc:CompileLocation -> AbstractIL.IL.ILTypeRef
    val mkILTyForCompLoc: cloc:CompileLocation -> AbstractIL.IL.ILType
    val ComputeMemberAccess: hidden:bool -> AbstractIL.IL.ILMemberAccess
    val ComputePublicTypeAccess: unit -> AbstractIL.IL.ILTypeDefAccess
    val ComputeTypeAccess:
      tref:AbstractIL.IL.ILTypeRef ->
        hidden:bool -> AbstractIL.IL.ILTypeDefAccess
    [<NoEquality; NoComparison>]
    type TypeReprEnv =
  
        new: reprs:Map<TypedTree.Stamp,uint16> * count:int -> TypeReprEnv
        static member ForTycon: tycon:TypedTree.Tycon -> TypeReprEnv
        static member ForTyconRef: tycon:TypedTree.TyconRef -> TypeReprEnv
        static member ForTypars: tps:TypedTree.Typar list -> TypeReprEnv
        member Add: tps:TypedTree.Typar list -> TypeReprEnv
        member AddOne: tp:TypedTree.Typar -> TypeReprEnv
        member Item: tp:TypedTree.Typar * m:Range.range -> uint16
        member Count: int
        static member Empty: TypeReprEnv
    
    val GenTyconRef: tcref:TypedTree.TyconRef -> TypedTree.CompiledTypeRepr
    type VoidNotOK =
      | VoidNotOK
      | VoidOK
    val voidCheck:
      m:Range.range ->
        g:TcGlobals.TcGlobals -> permits:VoidNotOK -> ty:TypedTree.TType -> unit
    type PtrsOK =
      | PtrTypesOK
      | PtrTypesNotOK
    val GenReadOnlyAttributeIfNecessary:
      g:TcGlobals.TcGlobals ->
        ty:TypedTree.TType -> AbstractIL.IL.ILAttribute option
    val GenReadOnlyModReqIfNecessary:
      g:TcGlobals.TcGlobals ->
        ty:TypedTree.TType -> ilTy:AbstractIL.IL.ILType -> AbstractIL.IL.ILType
    val GenTypeArgAux:
      amap:Import.ImportMap ->
        m:Range.range ->
          tyenv:TypeReprEnv -> tyarg:TypedTree.TType -> AbstractIL.IL.ILType
    val GenTypeArgsAux:
      amap:Import.ImportMap ->
        m:Range.range ->
          tyenv:TypeReprEnv ->
            tyargs:TypedTree.TType list -> AbstractIL.IL.ILType list
    val GenTyAppAux:
      amap:Import.ImportMap ->
        m:Range.range ->
          tyenv:TypeReprEnv ->
            repr:TypedTree.CompiledTypeRepr ->
              tinst:TypedTree.TType list -> AbstractIL.IL.ILType
    val GenILTyAppAux:
      amap:Import.ImportMap ->
        m:Range.range ->
          tyenv:TypeReprEnv ->
            tref:AbstractIL.IL.ILTypeRef * boxity:AbstractIL.IL.ILBoxity *
            ilTypeOpt:AbstractIL.IL.ILType option ->
              tinst:TypedTree.TType list -> AbstractIL.IL.ILType
    val GenNamedTyAppAux:
      amap:Import.ImportMap ->
        m:Range.range ->
          tyenv:TypeReprEnv ->
            ptrsOK:PtrsOK ->
              tcref:TypedTree.TyconRef ->
                tinst:TypedTree.TType list -> AbstractIL.IL.ILType
    val GenTypeAux:
      amap:Import.ImportMap ->
        m:Range.range ->
          tyenv:TypeReprEnv ->
            voidOK:VoidNotOK ->
              ptrsOK:PtrsOK -> ty:TypedTree.TType -> AbstractIL.IL.ILType
    val GenUnionCaseRef:
      amap:Import.ImportMap ->
        m:Range.range ->
          tyenv:TypeReprEnv ->
            i:int ->
              fspecs:TypedTree.RecdField [] ->
                AbstractIL.Extensions.ILX.Types.IlxUnionField []
    val GenUnionRef:
      amap:Import.ImportMap ->
        m:Range.range ->
          tcref:TypedTree.TyconRef ->
            AbstractIL.Extensions.ILX.Types.IlxUnionRef
    val ComputeUnionHasHelpers:
      g:TcGlobals.TcGlobals ->
        tcref:TypedTree.TyconRef ->
          AbstractIL.Extensions.ILX.Types.IlxUnionHasHelpers
    val GenUnionSpec:
      amap:Import.ImportMap ->
        m:Range.range ->
          tyenv:TypeReprEnv ->
            tcref:TypedTree.TyconRef ->
              tyargs:TypedTree.TypeInst ->
                AbstractIL.Extensions.ILX.Types.IlxUnionSpec
    val GenUnionCaseSpec:
      amap:Import.ImportMap ->
        m:Range.range ->
          tyenv:TypeReprEnv ->
            ucref:TypedTree.UnionCaseRef ->
              tyargs:TypedTree.TypeInst ->
                AbstractIL.Extensions.ILX.Types.IlxUnionSpec * int
    val GenType:
      amap:Import.ImportMap ->
        m:Range.range ->
          tyenv:TypeReprEnv -> ty:TypedTree.TType -> AbstractIL.IL.ILType
    val GenTypes:
      amap:Import.ImportMap ->
        m:Range.range ->
          tyenv:TypeReprEnv ->
            tys:TypedTree.TType list -> AbstractIL.IL.ILType list
    val GenTypePermitVoid:
      amap:Import.ImportMap ->
        m:Range.range ->
          tyenv:TypeReprEnv -> ty:TypedTree.TType -> AbstractIL.IL.ILType
    val GenTypesPermitVoid:
      amap:Import.ImportMap ->
        m:Range.range ->
          tyenv:TypeReprEnv ->
            tys:TypedTree.TType list -> AbstractIL.IL.ILType list
    val GenTyApp:
      amap:Import.ImportMap ->
        m:Range.range ->
          tyenv:TypeReprEnv ->
            repr:TypedTree.CompiledTypeRepr ->
              tyargs:TypedTree.TType list -> AbstractIL.IL.ILType
    val GenNamedTyApp:
      amap:Import.ImportMap ->
        m:Range.range ->
          tyenv:TypeReprEnv ->
            tcref:TypedTree.TyconRef ->
              tinst:TypedTree.TType list -> AbstractIL.IL.ILType
    val GenReturnType:
      amap:Import.ImportMap ->
        m:Range.range ->
          tyenv:TypeReprEnv ->
            returnTyOpt:TypedTree.TType option -> AbstractIL.IL.ILType
    val GenParamType:
      amap:Import.ImportMap ->
        m:Range.range ->
          tyenv:TypeReprEnv ->
            isSlotSig:bool -> ty:TypedTree.TType -> AbstractIL.IL.ILType
    val GenParamTypes:
      amap:Import.ImportMap ->
        m:Range.range ->
          tyenv:TypeReprEnv ->
            isSlotSig:bool ->
              tys:TypedTree.TType list -> AbstractIL.IL.ILType list
    val GenTypeArgs:
      amap:Import.ImportMap ->
        m:Range.range ->
          tyenv:TypeReprEnv ->
            tyargs:TypedTree.TypeInst -> AbstractIL.IL.ILGenericArgs
    val GenTypePermitVoidAux:
      amap:Import.ImportMap ->
        m:Range.range ->
          tyenv:TypeReprEnv -> ty:TypedTree.TType -> AbstractIL.IL.ILType
    val GenFieldSpecForStaticField:
      isInteractive:bool * g:TcGlobals.TcGlobals *
      ilContainerTy:AbstractIL.IL.ILType * vspec:TypedTree.Val * nm:string *
      m:Range.range * cloc:CompileLocation * ilTy:AbstractIL.IL.ILType ->
        AbstractIL.IL.ILFieldSpec
    val GenRecdFieldRef:
      m:Range.range ->
        cenv:cenv ->
          tyenv:TypeReprEnv ->
            rfref:TypedTree.RecdFieldRef ->
              tyargs:TypedTree.TType list -> AbstractIL.IL.ILFieldSpec
    val GenExnType:
      amap:Import.ImportMap ->
        m:Range.range ->
          tyenv:TypeReprEnv -> ecref:TypedTree.TyconRef -> AbstractIL.IL.ILType
    type ArityInfo = int list
    [<NoEquality; NoComparison>]
    type IlxClosureInfo =
      { cloExpr: TypedTree.Expr
        cloName: string
        cloArityInfo: ArityInfo
        ilCloFormalReturnTy: AbstractIL.IL.ILType
        ilCloAllFreeVars: AbstractIL.Extensions.ILX.Types.IlxClosureFreeVar []
        cloSpec: AbstractIL.Extensions.ILX.Types.IlxClosureSpec
        cloAttribs: TypedTree.Attribs
        cloILGenericParams: AbstractIL.IL.ILGenericParameterDefs
        cloFreeVars: TypedTree.Val list
        cloFreeTyvars: TypedTree.Typars
        cloWitnessInfos: TypedTreeOps.TraitWitnessInfos
        ilCloLambdas: AbstractIL.Extensions.ILX.Types.IlxClosureLambdas
        localTypeFuncILGenericArgs: AbstractIL.IL.ILType list
        localTypeFuncContractFreeTypars: TypedTree.Typar list
        localTypeFuncDirectILGenericParams: AbstractIL.IL.ILGenericParameterDefs
        localTypeFuncInternalFreeTypars: TypedTree.Typar list }
    [<NoEquality; NoComparison>]
    type ValStorage =
      | Null
      | StaticField of
        AbstractIL.IL.ILFieldSpec * TypedTree.ValRef * bool *
        AbstractIL.IL.ILType * string * AbstractIL.IL.ILType *
        AbstractIL.IL.ILMethodRef * AbstractIL.IL.ILMethodRef *
        OptionalShadowLocal
      | StaticProperty of AbstractIL.IL.ILMethodSpec * OptionalShadowLocal
      | Method of
        TypedTree.ValReprInfo * TypedTree.ValRef * AbstractIL.IL.ILMethodSpec *
        AbstractIL.IL.ILMethodSpec * Range.range * TypedTree.Typars *
        TypedTree.Typars * TypedTreeOps.CurriedArgInfos *
        TypedTree.ArgReprInfo list * TypedTreeOps.TraitWitnessInfos *
        TypedTree.TType list * TypedTree.ArgReprInfo
      | Env of
        AbstractIL.IL.ILType * AbstractIL.IL.ILFieldSpec *
        NamedLocalIlxClosureInfo ref option
      | Arg of int
      | Local of idx: int * realloc: bool * NamedLocalIlxClosureInfo ref option
    and OptionalShadowLocal =
      | NoShadowLocal
      | ShadowLocal of ValStorage
    and NamedLocalIlxClosureInfo =
      | NamedLocalIlxClosureInfoGenerator of (IlxGenEnv -> IlxClosureInfo)
      | NamedLocalIlxClosureInfoGenerated of IlxClosureInfo
      with
        override ToString: unit -> string
    
    and ModuleStorage =
      { Vals: Lazy<AbstractIL.Internal.Library.NameMap<ValStorage>>
        SubModules: Lazy<AbstractIL.Internal.Library.NameMap<ModuleStorage>> }
      with
        override ToString: unit -> string
    
    and BranchCallItem =
      | BranchCallClosure of ArityInfo
      | BranchCallMethod of
        ArityInfo * (TypedTree.TType * TypedTree.ArgReprInfo) list list *
        TypedTree.Typars * int * int * int
      with
        override ToString: unit -> string
    
    and Mark =
      | Mark of AbstractIL.IL.ILCodeLabel
      with
        member CodeLabel: AbstractIL.IL.ILCodeLabel
    
    and sequel =
      | EndFilter
      | LeaveHandler of (bool * int * Mark)
      | Br of Mark
      | CmpThenBrOrContinue of Pops * AbstractIL.IL.ILInstr list
      | Continue
      | DiscardThen of sequel
      | Return
      | EndLocalScope of sequel * Mark
      | ReturnVoid
    and Pushes = AbstractIL.IL.ILType list
    and Pops = int
    and IlxGenEnv =
      { tyenv: TypeReprEnv
        someTypeInThisAssembly: AbstractIL.IL.ILType
        isFinalFile: bool
        cloc: CompileLocation
        sigToImplRemapInfo:
          (TypedTreeOps.Remap * TypedTreeOps.SignatureHidingInfo) list
        valsInScope: TypedTreeOps.ValMap<Lazy<ValStorage>>
        witnessesInScope: TypedTreeOps.TraitWitnessInfoHashMap<ValStorage>
        suppressWitnesses: bool
        innerVals: (TypedTree.ValRef * (BranchCallItem * Mark)) list
        letBoundVars: TypedTree.ValRef list
        liveLocals: Lib.IntMap<unit>
        withinSEH: bool
        isInLoop: bool }
      with
        override ToString: unit -> string
    
    val discard: sequel
    val discardAndReturnVoid: sequel
    val SetIsInLoop: isInLoop:bool -> eenv:IlxGenEnv -> IlxGenEnv
    val ReplaceTyenv: tyenv:TypeReprEnv -> eenv:IlxGenEnv -> IlxGenEnv
    val EnvForTypars: tps:TypedTree.Typar list -> eenv:IlxGenEnv -> IlxGenEnv
    val AddTyparsToEnv:
      typars:TypedTree.Typar list -> eenv:IlxGenEnv -> IlxGenEnv
    val AddSignatureRemapInfo:
      _msg:'a ->
        rpi:TypedTreeOps.SignatureRepackageInfo *
        mhi:TypedTreeOps.SignatureHidingInfo -> eenv:IlxGenEnv -> IlxGenEnv
    val OutputStorage: pps:System.IO.TextWriter -> s:ValStorage -> unit
    val AddStorageForVal:
      g:TcGlobals.TcGlobals ->
        v:TypedTree.Val * s:Lazy<ValStorage> -> eenv:IlxGenEnv -> IlxGenEnv
    val AddStorageForLocalVals:
      g:TcGlobals.TcGlobals ->
        vals:(TypedTree.Val * ValStorage) list -> eenv:IlxGenEnv -> IlxGenEnv
    val AddStorageForLocalWitness:
      eenv:IlxGenEnv -> w:TypedTree.TraitWitnessInfo * s:ValStorage -> IlxGenEnv
    val AddStorageForLocalWitnesses:
      witnesses:(TypedTree.TraitWitnessInfo * ValStorage) list ->
        eenv:IlxGenEnv -> IlxGenEnv
    val StorageForVal:
      g:TcGlobals.TcGlobals ->
        m:Range.range -> v:TypedTree.Val -> eenv:IlxGenEnv -> ValStorage
    val StorageForValRef:
      g:TcGlobals.TcGlobals ->
        m:Range.range -> v:TypedTree.ValRef -> eenv:IlxGenEnv -> ValStorage
    val ComputeGenerateWitnesses:
      g:TcGlobals.TcGlobals -> eenv:IlxGenEnv -> bool
    val TryStorageForWitness:
      _g:TcGlobals.TcGlobals ->
        eenv:IlxGenEnv -> w:TypedTree.TraitWitnessInfo -> ValStorage option
    val IsValRefIsDllImport:
      g:TcGlobals.TcGlobals -> vref:TypedTree.ValRef -> bool
    val GetMethodSpecForMemberVal:
      amap:Import.ImportMap ->
        g:TcGlobals.TcGlobals ->
          memberInfo:TypedTree.ValMemberInfo ->
            vref:TypedTree.ValRef ->
              AbstractIL.IL.ILMethodSpec * AbstractIL.IL.ILMethodSpec *
              TypedTree.Typar list * TypedTree.Typar list *
              TypedTreeOps.CurriedArgInfos * TypedTree.ArgReprInfo list *
              TypedTree.ArgReprInfo * TypedTreeOps.TraitWitnessInfos *
              TypedTree.TType list * TypedTree.TType option
    val ComputeFieldSpecForVal:
      optIntraAssemblyInfo:IlxGenIntraAssemblyInfo option * isInteractive:bool *
      g:TcGlobals.TcGlobals * ilTyForProperty:AbstractIL.IL.ILType *
      vspec:TypedTree.Val * nm:string * m:Range.range * cloc:CompileLocation *
      ilTy:AbstractIL.IL.ILType * ilGetterMethRef:AbstractIL.IL.ILMethodRef ->
        AbstractIL.IL.ILFieldSpec
    val ComputeStorageForFSharpValue:
      amap:Import.ImportMap ->
        g:TcGlobals.TcGlobals ->
          cloc:CompileLocation ->
            optIntraAssemblyInfo:IlxGenIntraAssemblyInfo option ->
              optShadowLocal:OptionalShadowLocal ->
                isInteractive:bool ->
                  returnTy:TypedTree.TType ->
                    vref:TypedTree.ValRef -> m:Range.range -> ValStorage
    val ComputeStorageForFSharpMember:
      amap:Import.ImportMap ->
        g:TcGlobals.TcGlobals ->
          topValInfo:TypedTree.ValReprInfo ->
            memberInfo:TypedTree.ValMemberInfo ->
              vref:TypedTree.ValRef -> m:Range.range -> ValStorage
    val ComputeStorageForFSharpFunctionOrFSharpExtensionMember:
      amap:Import.ImportMap ->
        g:TcGlobals.TcGlobals ->
          cloc:CompileLocation ->
            topValInfo:TypedTree.ValReprInfo ->
              vref:TypedTree.ValRef -> m:Range.range -> ValStorage
    val IsFSharpValCompiledAsMethod:
      TcGlobals.TcGlobals -> TypedTree.Val -> bool
    val ComputeStorageForTopVal:
      amap:Import.ImportMap * g:TcGlobals.TcGlobals *
      optIntraAssemblyInfo:IlxGenIntraAssemblyInfo option * isInteractive:bool *
      optShadowLocal:OptionalShadowLocal * vref:TypedTree.ValRef *
      cloc:CompileLocation -> ValStorage
    val ComputeAndAddStorageForLocalTopVal:
      amap:Import.ImportMap * g:TcGlobals.TcGlobals *
      intraAssemblyFieldTable:IlxGenIntraAssemblyInfo * isInteractive:bool *
      optShadowLocal:OptionalShadowLocal ->
        cloc:CompileLocation -> v:TypedTree.Val -> eenv:IlxGenEnv -> IlxGenEnv
    val ComputeStorageForNonLocalTopVal:
      amap:Import.ImportMap ->
        g:TcGlobals.TcGlobals ->
          cloc:CompileLocation ->
            modref:TypedTree.EntityRef -> v:TypedTree.Val -> ValStorage
    val AddStorageForNonLocalModuleOrNamespaceRef:
      amap:Import.ImportMap ->
        g:TcGlobals.TcGlobals ->
          cloc:CompileLocation ->
            acc:IlxGenEnv ->
              modref:TypedTree.ModuleOrNamespaceRef ->
                modul:TypedTree.ModuleOrNamespace -> IlxGenEnv
    val AddStorageForExternalCcu:
      amap:Import.ImportMap ->
        g:TcGlobals.TcGlobals ->
          eenv:IlxGenEnv -> ccu:TypedTree.CcuThunk -> IlxGenEnv
    val AddBindingsForLocalModuleType:
      allocVal:(CompileLocation -> TypedTree.Val -> 'a -> 'a) ->
        cloc:CompileLocation ->
          eenv:'a -> mty:TypedTree.ModuleOrNamespaceType -> 'a
    val AddExternalCcusToIlxGenEnv:
      amap:Import.ImportMap ->
        g:TcGlobals.TcGlobals ->
          eenv:IlxGenEnv -> ccus:TypedTree.CcuThunk list -> IlxGenEnv
    val AddBindingsForTycon:
      allocVal:(CompileLocation -> TypedTree.Val -> 'a -> 'a) ->
        cloc:CompileLocation -> tycon:TypedTree.Tycon -> eenv:'a -> 'a
    val AddBindingsForModuleDefs:
      allocVal:(CompileLocation -> TypedTree.Val -> 'a -> 'a) ->
        cloc:CompileLocation ->
          eenv:'a -> mdefs:TypedTree.ModuleOrNamespaceExpr list -> 'a
    val AddBindingsForModuleDef:
      allocVal:(CompileLocation -> TypedTree.Val -> 'a -> 'a) ->
        cloc:CompileLocation ->
          eenv:'a -> x:TypedTree.ModuleOrNamespaceExpr -> 'a
    val AddBindingsForModule:
      allocVal:(CompileLocation -> TypedTree.Val -> 'a -> 'a) ->
        cloc:CompileLocation ->
          x:TypedTree.ModuleOrNamespaceBinding -> eenv:'a -> 'a
    val AddBindingsForModuleTopVals:
      _g:'a ->
        allocVal:('b -> 'c -> 'c) -> _cloc:'d -> eenv:'c -> vs:'b list -> 'c
    val AddIncrementalLocalAssemblyFragmentToIlxGenEnv:
      amap:Import.ImportMap * isIncrementalFragment:bool * g:TcGlobals.TcGlobals *
      ccu:TypedTree.CcuThunk * fragName:string *
      intraAssemblyInfo:IlxGenIntraAssemblyInfo * eenv:IlxGenEnv *
      typedImplFiles:TypedTree.TypedImplFile list -> IlxGenEnv
    val GenILSourceMarker:
      g:TcGlobals.TcGlobals -> m:Range.range -> AbstractIL.IL.ILSourceMarker
    val GenPossibleILSourceMarker:
      cenv:cenv -> m:Range.range -> AbstractIL.IL.ILSourceMarker option
    val HashRangeSorted:
      ht:System.Collections.Generic.IDictionary<'a,(int * 'b)> -> 'b list
    val MergeOptions:
      m:Range.range -> o1:'a option -> o2:'a option -> 'a option
    val MergePropertyPair:
      m:Range.range ->
        pd:AbstractIL.IL.ILPropertyDef ->
          pdef:AbstractIL.IL.ILPropertyDef -> AbstractIL.IL.ILPropertyDef
    type PropKey =
      | PropKey of
        string * AbstractIL.IL.ILTypes * AbstractIL.IL.ILThisConvention
    val AddPropertyDefToHash:
      m:Range.range ->
        ht:System.Collections.Generic.Dictionary<PropKey,
                                                 (int *
                                                  AbstractIL.IL.ILPropertyDef)> ->
          pdef:AbstractIL.IL.ILPropertyDef -> unit
    val MergePropertyDefs:
      m:Range.range ->
        ilPropertyDefs:#AbstractIL.IL.ILPropertyDef list ->
          AbstractIL.IL.ILPropertyDef list
    type TypeDefBuilder =
  
        new: tdef:AbstractIL.IL.ILTypeDef *
              tdefDiscards:((AbstractIL.IL.ILMethodDef -> bool) *
                            (AbstractIL.IL.ILPropertyDef -> bool)) option ->
                TypeDefBuilder
        member AddEventDef: edef:AbstractIL.IL.ILEventDef -> unit
        member AddFieldDef: ilFieldDef:AbstractIL.IL.ILFieldDef -> unit
        member AddMethodDef: ilMethodDef:AbstractIL.IL.ILMethodDef -> unit
        member
          AddOrMergePropertyDef: pdef:AbstractIL.IL.ILPropertyDef *
                                  m:Range.range -> unit
        member Close: unit -> AbstractIL.IL.ILTypeDef
        member GetCurrentFields: unit -> seq<AbstractIL.IL.ILFieldDef>
        member
          PrependInstructionsToSpecificMethodDef: cond:(AbstractIL.IL.ILMethodDef ->
                                                           bool) *
                                                   instrs:AbstractIL.IL.ILInstr list *
                                                   tag:AbstractIL.IL.ILSourceMarker option ->
                                                     unit
        member NestedTypeDefs: TypeDefsBuilder
    
    and TypeDefsBuilder =
  
        new: unit -> TypeDefsBuilder
        member
          AddTypeDef: tdef:AbstractIL.IL.ILTypeDef * eliminateIfEmpty:bool *
                       addAtEnd:bool *
                       tdefDiscards:((AbstractIL.IL.ILMethodDef -> bool) *
                                     (AbstractIL.IL.ILPropertyDef -> bool)) option ->
                         unit
        member Close: unit -> AbstractIL.IL.ILTypeDef list
        member
          FindNestedTypeDefBuilder: tref:AbstractIL.IL.ILTypeRef ->
                                       TypeDefBuilder
        member FindNestedTypeDefsBuilder: path:string list -> TypeDefsBuilder
        member FindTypeDefBuilder: nm:string -> TypeDefBuilder
    
    type AnonTypeGenerationTable =
  
        new: unit -> AnonTypeGenerationTable
        member
          Table: System.Collections.Generic.Dictionary<TypedTree.Stamp,
                                                        (AbstractIL.IL.ILMethodRef *
                                                         AbstractIL.IL.ILMethodRef [] *
                                                         AbstractIL.IL.ILType)>
    
    type AssemblyBuilder =
  
        new: cenv:cenv * anonTypeTable:AnonTypeGenerationTable ->
                AssemblyBuilder
        member
          AddEventDef: tref:AbstractIL.IL.ILTypeRef *
                        edef:AbstractIL.IL.ILEventDef -> unit
        member
          AddExplicitInitToSpecificMethodDef: cond:(AbstractIL.IL.ILMethodDef ->
                                                       bool) *
                                               tref:AbstractIL.IL.ILTypeRef *
                                               fspec:AbstractIL.IL.ILFieldSpec *
                                               sourceOpt:AbstractIL.IL.ILSourceMarker option *
                                               feefee:AbstractIL.IL.ILInstr list *
                                               seqpt:AbstractIL.IL.ILInstr list ->
                                                 unit
        member
          AddFieldDef: tref:AbstractIL.IL.ILTypeRef *
                        ilFieldDef:AbstractIL.IL.ILFieldDef -> unit
        member AddInitializeScriptsInOrderToEntryPoint: unit -> unit
        member
          AddMethodDef: tref:AbstractIL.IL.ILTypeRef *
                         ilMethodDef:AbstractIL.IL.ILMethodDef -> unit
        member
          AddOrMergePropertyDef: tref:AbstractIL.IL.ILTypeRef *
                                  pdef:AbstractIL.IL.ILPropertyDef *
                                  m:Range.range -> unit
        member
          AddReflectedDefinition: vspec:TypedTree.Val * expr:TypedTree.Expr ->
                                     unit
        member
          AddScriptInitFieldSpec: fieldSpec:AbstractIL.IL.ILFieldSpec *
                                   range:Range.range -> unit
        member
          AddTypeDef: tref:AbstractIL.IL.ILTypeRef *
                       tdef:AbstractIL.IL.ILTypeDef * eliminateIfEmpty:bool *
                       addAtEnd:bool *
                       tdefDiscards:((AbstractIL.IL.ILMethodDef -> bool) *
                                     (AbstractIL.IL.ILPropertyDef -> bool)) option ->
                         unit
        member
          Close: unit ->
                    AbstractIL.IL.ILTypeDef list *
                    ((string * TypedTree.Val) * TypedTree.Expr) list
        member
          GenerateAnonType: genToStringMethod:(AbstractIL.IL.ILType ->
                                                  #seq<AbstractIL.IL.ILMethodDef>) *
                             anonInfo:TypedTree.AnonRecdTypeInfo -> unit
        member
          GenerateRawDataValueType: cloc:CompileLocation * size:int ->
                                       AbstractIL.IL.ILTypeSpec
        member
          GetCurrentFields: tref:AbstractIL.IL.ILTypeRef ->
                               seq<AbstractIL.IL.ILFieldDef>
        member
          GetExplicitEntryPointInfo: unit -> AbstractIL.IL.ILTypeRef option
        member GrabExtraBindingsToGenerate: unit -> TypedTree.Binding list
        member
          LookupAnonType: genToStringMethod:(AbstractIL.IL.ILType ->
                                                #seq<AbstractIL.IL.ILMethodDef>) *
                           anonInfo:TypedTree.AnonRecdTypeInfo ->
                             AbstractIL.IL.ILMethodRef *
                             AbstractIL.IL.ILMethodRef [] * AbstractIL.IL.ILType
        member
          ReplaceNameOfReflectedDefinition: vspec:TypedTree.Val *
                                             newName:string -> unit
        member cenv: cenv
    
    val pop: i:int -> Pops
    val Push: tys:Pushes -> Pushes
    val Push0: Pushes
    val FeeFee: cenv:cenv -> int
    val FeeFeeInstr:
      cenv:cenv -> doc:AbstractIL.IL.ILSourceDocument -> AbstractIL.IL.ILInstr
    type CodeGenBuffer =
  
        new: m:Range.range * mgbuf:AssemblyBuilder * methodName:string *
              alreadyUsedArgs:int -> CodeGenBuffer
        member
          AllocLocal: ranges:(string * (Mark * Mark)) list *
                       ty:AbstractIL.IL.ILType * isFixed:bool -> int
        member AssertEmptyStack: unit -> unit
        member
          Close: unit ->
                    ((string * (Mark * Mark)) list * AbstractIL.IL.ILType * bool) list *
                    int *
                    System.Collections.Generic.Dictionary<AbstractIL.IL.ILCodeLabel,
                                                          int> *
                    AbstractIL.IL.ILInstr [] *
                    AbstractIL.IL.ILExceptionSpec list * bool
        member DoPops: n:Pops -> unit
        member DoPushes: pushes:Pushes -> unit
        member
          EmitExceptionClause: clause:AbstractIL.IL.ILExceptionSpec -> unit
        member
          EmitInstr: pops:Pops * pushes:Pushes * i:AbstractIL.IL.ILInstr ->
                        unit
        member
          EmitInstrs: pops:Pops * pushes:Pushes * is:AbstractIL.IL.ILInstr list ->
                         unit
        member EmitSeqPoint: src:Range.range -> unit
        member EmitStartOfHiddenCode: unit -> unit
        member private EnsureNopBetweenDebugPoints: unit -> unit
        member GenerateDelayMark: _nm:'b -> Mark
        member GetCurrentStack: unit -> AbstractIL.IL.ILType list
        member GetLastDebugPoint: unit -> Range.range option
        member Mark: s:'a -> Mark
        member
          ReallocLocal: cond:(int ->
                                 (string * (Mark * Mark)) list *
                                 AbstractIL.IL.ILType * bool -> bool) *
                         ranges:(string * (Mark * Mark)) list *
                         ty:AbstractIL.IL.ILType * isFixed:bool -> int * bool
        member
          SetCodeLabelToCodeLabel: lab1:AbstractIL.IL.ILCodeLabel *
                                    lab2:AbstractIL.IL.ILCodeLabel -> unit
        member SetCodeLabelToPC: lab:AbstractIL.IL.ILCodeLabel * pc:int -> unit
        member SetMark: mark1:Mark * mark2:Mark -> unit
        member SetMarkToHere: Mark -> unit
        member SetStack: s:AbstractIL.IL.ILType list -> unit
        member MethodName: string
        member PreallocatedArgCount: int
        member mgbuf: AssemblyBuilder
    
    module CG =
      val EmitInstr:
        cgbuf:CodeGenBuffer ->
          pops:Pops -> pushes:Pushes -> i:AbstractIL.IL.ILInstr -> unit
      val EmitInstrs:
        cgbuf:CodeGenBuffer ->
          pops:Pops -> pushes:Pushes -> is:AbstractIL.IL.ILInstr list -> unit
      val EmitSeqPoint: cgbuf:CodeGenBuffer -> src:Range.range -> unit
      val GenerateDelayMark: cgbuf:CodeGenBuffer -> nm:'a -> Mark
      val SetMark: cgbuf:CodeGenBuffer -> m1:Mark -> m2:Mark -> unit
      val SetMarkToHere: cgbuf:CodeGenBuffer -> m1:Mark -> unit
      val SetStack: cgbuf:CodeGenBuffer -> s:AbstractIL.IL.ILType list -> unit
      val GenerateMark: cgbuf:CodeGenBuffer -> s:'a -> Mark
  
    val GenString: cenv:cenv -> cgbuf:CodeGenBuffer -> s:string -> unit
    val GenConstArray:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            ilElementType:AbstractIL.IL.ILType ->
              data:'a [] ->
                write:(AbstractIL.Internal.ByteBuffer -> 'a -> unit) -> unit
    val CodeGenThen:
      cenv:cenv ->
        mgbuf:AssemblyBuilder ->
          entryPointInfo:(TypedTree.ValRef * BranchCallItem) list *
          methodName:string * eenv:IlxGenEnv * alreadyUsedArgs:int *
          codeGenFunction:(CodeGenBuffer -> IlxGenEnv -> unit) * m:Range.range ->
            AbstractIL.IL.ILLocal list * int *
            System.Collections.Generic.Dictionary<AbstractIL.IL.ILCodeLabel,int> *
            AbstractIL.IL.ILInstr [] * AbstractIL.IL.ILExceptionSpec list *
            AbstractIL.IL.ILLocalDebugInfo list * bool
    val CodeGenMethod:
      cenv:cenv ->
        mgbuf:AssemblyBuilder ->
          entryPointInfo:(TypedTree.ValRef * BranchCallItem) list *
          methodName:string * eenv:IlxGenEnv * alreadyUsedArgs:int *
          codeGenFunction:(CodeGenBuffer -> IlxGenEnv -> unit) * m:Range.range ->
            AbstractIL.IL.ILInstr [] * AbstractIL.IL.ILMethodBody
    val StartDelayedLocalScope: nm:string -> cgbuf:CodeGenBuffer -> Mark * Mark
    val StartLocalScope: nm:string -> cgbuf:CodeGenBuffer -> Mark * Mark
    val LocalScope:
      nm:string -> cgbuf:CodeGenBuffer -> f:(Mark * Mark -> 'a) -> 'a
    val compileSequenceExpressions: bool
    type EmitDebugPointState =
      | SPAlways
      | SPSuppress
    val BindingEmitsNoCode:
      g:TcGlobals.TcGlobals -> b:TypedTree.Binding -> bool
    val ComputeDebugPointForBinding:
      g:TcGlobals.TcGlobals ->
        TypedTree.Binding -> bool * Range.range option * EmitDebugPointState
    val BindingEmitsDebugPoint:
      g:TcGlobals.TcGlobals -> bind:TypedTree.Binding -> bool
    val BindingIsInvisible: TypedTree.Binding -> bool
    val BindingEmitsHiddenCode: TypedTree.Binding -> bool
    val FirstEmittedCodeWillBeDebugPoint:
      g:TcGlobals.TcGlobals ->
        sp:EmitDebugPointState -> expr:TypedTree.Expr -> bool
    val EmitDebugPointForWholeExpr:
      g:TcGlobals.TcGlobals ->
        sp:EmitDebugPointState -> expr:TypedTree.Expr -> bool
    val EmitHiddenCodeMarkerForWholeExpr:
      g:TcGlobals.TcGlobals ->
        sp:EmitDebugPointState -> expr:TypedTree.Expr -> bool
    val RangeOfDebugPointForWholeExpr:
      g:TcGlobals.TcGlobals -> expr:TypedTree.Expr -> Range.range
    val DoesGenExprStartWithDebugPoint:
      g:TcGlobals.TcGlobals ->
        sp:EmitDebugPointState -> expr:TypedTree.Expr -> bool
    val ProcessDebugPointForExpr:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          sp:EmitDebugPointState -> expr:TypedTree.Expr -> unit
    val GenExpr:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            sp:EmitDebugPointState ->
              expr:TypedTree.Expr -> sequel:sequel -> unit
    val ProcessDelayedGenMethods: cenv:cenv -> unit
    val GenExprWithStackGuard:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            sp:EmitDebugPointState ->
              expr:TypedTree.Expr -> sequel:sequel -> unit
    val GenExprPreSteps:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            sp:EmitDebugPointState ->
              expr:TypedTree.Expr -> sequel:sequel -> bool
    val GenExprAux:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            sp:EmitDebugPointState ->
              expr:TypedTree.Expr -> sequel:sequel -> unit
    val GenExprs:
      cenv:cenv ->
        cgbuf:CodeGenBuffer -> eenv:IlxGenEnv -> es:TypedTree.Expr list -> unit
    val CodeGenMethodForExpr:
      cenv:cenv ->
        mgbuf:AssemblyBuilder ->
          spReq:EmitDebugPointState *
          entryPointInfo:(TypedTree.ValRef * BranchCallItem) list *
          methodName:string * eenv:IlxGenEnv * alreadyUsedArgs:int *
          expr0:TypedTree.Expr * sequel0:sequel -> AbstractIL.IL.ILMethodBody
    val sequelAfterDiscard: sequel:sequel -> sequel option
    val sequelIgnoringEndScopesAndDiscard: sequel:sequel -> sequel
    val sequelIgnoreEndScopes: sequel:sequel -> sequel
    val GenSequelEndScopes: cgbuf:CodeGenBuffer -> sequel:sequel -> unit
    val StringOfSequel: sequel:sequel -> string
    val GenSequel:
      cenv:cenv ->
        cloc:CompileLocation -> cgbuf:CodeGenBuffer -> sequel:sequel -> unit
    val GenConstant:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            c:TypedTree.Const * m:Range.range * ty:TypedTree.TType ->
              sequel:sequel -> unit
    val GenUnitTy:
      cenv:cenv -> eenv:IlxGenEnv -> m:Range.range -> AbstractIL.IL.ILType
    val GenUnit:
      cenv:cenv ->
        eenv:IlxGenEnv -> m:Range.range -> cgbuf:CodeGenBuffer -> unit
    val GenUnitThenSequel:
      cenv:cenv ->
        eenv:IlxGenEnv ->
          m:Range.range ->
            cloc:CompileLocation -> cgbuf:CodeGenBuffer -> sequel:sequel -> unit
    val GenAllocTuple:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            tupInfo:TypedTree.TupInfo * args:TypedTree.Exprs *
            argtys:TypedTree.TypeInst * m:Range.range -> sequel:sequel -> unit
    val GenGetTupleField:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            tupInfo:TypedTree.TupInfo * e:TypedTree.Expr *
            tys:TypedTree.TypeInst * n:int * m:Range.range ->
              sequel:sequel -> unit
    val GenAllocExn:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            c:TypedTree.TyconRef * args:TypedTree.Exprs * m:Range.range ->
              sequel:sequel -> unit
    val GenAllocUnionCaseCore:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            c:TypedTree.UnionCaseRef * tyargs:TypedTree.TypeInst * n:int *
            m:Range.range -> unit
    val GenAllocUnionCase:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            c:TypedTree.UnionCaseRef * tyargs:TypedTree.TypeInst *
            args:TypedTree.Exprs * m:Range.range -> sequel:sequel -> unit
    val GenLinearExpr:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            sp:EmitDebugPointState ->
              expr:TypedTree.Expr ->
                sequel:sequel ->
                  preSteps:bool -> contf:(FakeUnit -> FakeUnit) -> FakeUnit
    val GenAllocRecd:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            ctorInfo:TypedTree.RecordConstructionInfo ->
              tcref:TypedTree.TyconRef * argtys:TypedTree.TypeInst *
              args:TypedTree.Exprs * m:Range.range -> sequel:sequel -> unit
    val GenAllocAnonRecd:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            anonInfo:TypedTree.AnonRecdTypeInfo * tyargs:TypedTree.TypeInst *
            args:TypedTree.Exprs * m:Range.range -> sequel:sequel -> unit
    val GenGetAnonRecdField:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            anonInfo:TypedTree.AnonRecdTypeInfo * e:TypedTree.Expr *
            tyargs:TypedTree.TypeInst * n:int * m:Range.range ->
              sequel:sequel -> unit
    val GenNewArraySimple:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            elems:TypedTree.Expr list * elemTy:TypedTree.TType * m:Range.range ->
              sequel:sequel -> unit
    val GenNewArray:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            elems:TypedTree.Expr list * elemTy:TypedTree.TType * m:Range.range ->
              sequel:sequel -> unit
    val GenCoerce:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            e:TypedTree.Expr * tgty:TypedTree.TType * m:Range.range *
            srcty:TypedTree.TType -> sequel:sequel -> unit
    val GenReraise:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            rtnty:TypedTree.TType * m:Range.range -> sequel:sequel -> unit
    val GenGetExnField:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            e:TypedTree.Expr * ecref:TypedTree.TyconRef * fieldNum:int *
            m:Range.range -> sequel:sequel -> unit
    val GenSetExnField:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            e:TypedTree.Expr * ecref:TypedTree.TyconRef * fieldNum:int *
            e2:TypedTree.Expr * m:Range.range -> sequel:sequel -> unit
    val UnionCodeGen:
      cgbuf:CodeGenBuffer ->
        AbstractIL.Extensions.ILX.EraseUnions.ICodeGen<Mark>
    val GenUnionCaseProof:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            e:TypedTree.Expr * ucref:TypedTree.UnionCaseRef *
            tyargs:TypedTree.TypeInst * m:Range.range -> sequel:sequel -> unit
    val GenGetUnionCaseField:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            e:TypedTree.Expr * ucref:TypedTree.UnionCaseRef *
            tyargs:TypedTree.TypeInst * n:int * m:Range.range ->
              sequel:sequel -> unit
    val GenGetUnionCaseFieldAddr:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            e:TypedTree.Expr * ucref:TypedTree.UnionCaseRef *
            tyargs:TypedTree.TypeInst * n:int * m:Range.range ->
              sequel:sequel -> unit
    val GenGetUnionCaseTag:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            e:TypedTree.Expr * tcref:TypedTree.TyconRef *
            tyargs:TypedTree.TypeInst * m:Range.range -> sequel:sequel -> unit
    val GenSetUnionCaseField:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            e:TypedTree.Expr * ucref:TypedTree.UnionCaseRef *
            tyargs:TypedTree.TypeInst * n:int * e2:TypedTree.Expr *
            m:Range.range -> sequel:sequel -> unit
    val GenGetRecdFieldAddr:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            e:TypedTree.Expr * f:TypedTree.RecdFieldRef *
            tyargs:TypedTree.TypeInst * m:Range.range -> sequel:sequel -> unit
    val GenGetStaticFieldAddr:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            f:TypedTree.RecdFieldRef * tyargs:TypedTree.TypeInst * m:Range.range ->
              sequel:sequel -> unit
    val GenGetRecdField:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            e:TypedTree.Expr * f:TypedTree.RecdFieldRef *
            tyargs:TypedTree.TypeInst * m:Range.range -> sequel:sequel -> unit
    val GenSetRecdField:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            e1:TypedTree.Expr * f:TypedTree.RecdFieldRef *
            tyargs:TypedTree.TypeInst * e2:TypedTree.Expr * m:Range.range ->
              sequel:sequel -> unit
    val GenGetStaticField:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            f:TypedTree.RecdFieldRef * tyargs:TypedTree.TypeInst * m:Range.range ->
              sequel:sequel -> unit
    val GenSetStaticField:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            f:TypedTree.RecdFieldRef * tyargs:TypedTree.TypeInst *
            e2:TypedTree.Expr * m:Range.range -> sequel:sequel -> unit
    val mk_field_pops: isStatic:bool -> n:int -> Pops
    val GenFieldGet:
      isStatic:bool ->
        cenv:cenv ->
          cgbuf:CodeGenBuffer ->
            eenv:IlxGenEnv ->
              rfref:TypedTree.RecdFieldRef * tyargs:TypedTree.TypeInst *
              m:Range.range -> unit
    val GenFieldStore:
      isStatic:bool ->
        cenv:cenv ->
          cgbuf:CodeGenBuffer ->
            eenv:IlxGenEnv ->
              rfref:TypedTree.RecdFieldRef * tyargs:TypedTree.TypeInst *
              m:Range.range -> sequel:sequel -> unit
    val GenUntupledArgsDiscardingLoneUnit:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            m:Range.range ->
              numObjArgs:int ->
                curriedArgInfos:(TypedTree.TType * TypedTree.ArgReprInfo) list list ->
                  args:TypedTree.Expr list -> unit
    val GenUntupledArgExpr:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            m:Range.range ->
              argInfos:(TypedTree.TType * TypedTree.ArgReprInfo) list ->
                expr:TypedTree.Expr -> unit
    val GenWitnessArgFromTraitInfo:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            m:Range.range -> traitInfo:TypedTree.TraitConstraintInfo -> unit
    val GenWitnessArgFromWitnessInfo:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            m:Range.range -> witnessInfo:TypedTree.TraitWitnessInfo -> unit
    val GenWitnessArgsFromWitnessInfos:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            m:Range.range ->
              witnessInfos:seq<TypedTree.TraitWitnessInfo> -> unit
    val GenWitnessArgs:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            m:Range.range ->
              tps:TypedTree.Typars -> tyargs:TypedTree.TType list -> unit
    val GenApp:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            f:TypedTree.Expr * fty:TypedTree.TType * tyargs:TypedTree.TypeInst *
            curriedArgs:TypedTree.Exprs * m:Range.range -> sequel:sequel -> unit
    val CanTailcall:
      hasStructObjArg:bool * ccallInfo:TypedTree.TType option * withinSEH:bool *
      hasByrefArg:bool * mustGenerateUnitAfterCall:bool * isDllImport:bool *
      isSelfInit:bool * makesNoCriticalTailcalls:bool * sequel:sequel ->
        AbstractIL.IL.ILTailcall
    val GenNamedLocalTyFuncCall:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            ty:TypedTree.TType ->
              cloinfo:IlxClosureInfo ->
                tyargs:TypedTree.TypeInst -> m:Range.range -> TypedTree.TType
    val GenCurriedArgsAndIndirectCall:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            functy:TypedTree.TType * tyargs:TypedTree.TypeInst *
            curriedArgs:TypedTree.Exprs * m:Range.range -> sequel:sequel -> unit
    val GenIndirectCall:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            functy:TypedTree.TType * tyargs:TypedTree.TType list *
            curriedArgs:TypedTree.Expr list * m:Range.range ->
              sequel:sequel -> unit
    val GenTry:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            Mark * Mark ->
              e1:TypedTree.Expr * m:Range.range * resty:TypedTree.TType *
              spTry:SyntaxTree.DebugPointAtTry ->
                int * IlxGenEnv * (AbstractIL.IL.ILType list * int list) *
                (AbstractIL.IL.ILCodeLabel * AbstractIL.IL.ILCodeLabel) * Mark *
                AbstractIL.IL.ILType
    val GenTryWith:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            e1:TypedTree.Expr * vf:TypedTree.Val * ef:TypedTree.Expr *
            vh:TypedTree.Val * eh:TypedTree.Expr * m:Range.range *
            resty:TypedTree.TType * spTry:SyntaxTree.DebugPointAtTry *
            spWith:SyntaxTree.DebugPointAtWith -> sequel:sequel -> unit
    val GenTryFinally:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            bodyExpr:TypedTree.Expr * handlerExpr:TypedTree.Expr * m:Range.range *
            resty:TypedTree.TType * spTry:SyntaxTree.DebugPointAtTry *
            spFinally:SyntaxTree.DebugPointAtFinally -> sequel:sequel -> unit
    val GenForLoop:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            spFor:SyntaxTree.DebugPointAtFor * v:TypedTree.Val *
            e1:TypedTree.Expr * dir:TypedTree.ForLoopStyle * e2:TypedTree.Expr *
            loopBody:TypedTree.Expr * m:Range.range -> sequel:sequel -> unit
    val GenWhileLoop:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            spWhile:SyntaxTree.DebugPointAtWhile * e1:TypedTree.Expr *
            e2:TypedTree.Expr * m:Range.range -> sequel:sequel -> unit
    val GenAsmCode:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            il:AbstractIL.IL.ILInstr list * tyargs:TypedTree.TypeInst *
            args:TypedTree.Exprs * returnTys:TypedTree.TTypes * m:Range.range ->
              sequel:sequel -> unit
    val GenQuotation:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            ast:TypedTree.Expr *
            qdataCell:((AbstractIL.IL.ILTypeRef list * TypedTree.TTypes *
                        TypedTree.Exprs * QuotationPickler.ExprData) *
                       (AbstractIL.IL.ILTypeRef list * TypedTree.TTypes *
                        TypedTree.Exprs * QuotationPickler.ExprData)) option ref *
            m:Range.range * ety:TypedTree.TType -> sequel:sequel -> unit
    val GenILCall:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            virt:bool * valu:bool * newobj:bool *
            valUseFlags:TypedTree.ValUseFlag * isDllImport:bool *
            ilMethRef:AbstractIL.IL.ILMethodRef * enclArgTys:TypedTree.TypeInst *
            methArgTys:TypedTree.TypeInst * argExprs:TypedTree.Exprs *
            returnTys:TypedTree.TTypes * m:Range.range -> sequel:sequel -> unit
    val CommitCallSequel:
      cenv:cenv ->
        eenv:IlxGenEnv ->
          m:Range.range ->
            cloc:CompileLocation ->
              cgbuf:CodeGenBuffer ->
                mustGenerateUnitAfterCall:bool -> sequel:sequel -> unit
    val MakeNotSupportedExnExpr:
      cenv:cenv ->
        eenv:IlxGenEnv ->
          argExpr:TypedTree.Expr * m:Range.range -> TypedTree.Expr
    val GenTraitCall:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            traitInfo:TypedTree.TraitConstraintInfo * argExprs:TypedTree.Exprs *
            m:Range.range -> expr:TypedTree.Expr -> sequel:sequel -> unit
    val GenGetAddrOfRefCellField:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            e:TypedTree.Expr * ty:TypedTree.TType * m:Range.range ->
              sequel:sequel -> unit
    val GenGetValAddr:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            v:TypedTree.ValRef * m:Range.range -> sequel:sequel -> unit
    val GenGetByref:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            v:TypedTree.ValRef * m:Range.range -> sequel:sequel -> unit
    val GenSetByref:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            v:TypedTree.ValRef * e:TypedTree.Expr * m:Range.range ->
              sequel:sequel -> unit
    val GenDefaultValue:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv -> ty:TypedTree.TType * m:Range.range -> unit
    val GenGenericParam:
      cenv:cenv ->
        eenv:IlxGenEnv ->
          tp:TypedTree.Typar -> AbstractIL.IL.ILGenericParameterDef
    val GenSlotParam:
      m:Range.range ->
        cenv:cenv ->
          eenv:IlxGenEnv -> TypedTree.SlotParam -> AbstractIL.IL.ILParameter
    val GenFormalSlotsig:
      m:Range.range ->
        cenv:cenv ->
          eenv:IlxGenEnv ->
            TypedTree.SlotSig ->
              AbstractIL.IL.ILType * AbstractIL.IL.ILParameter list *
              AbstractIL.IL.ILReturn
    val instSlotParam:
      inst:TypedTreeOps.TyparInst -> TypedTree.SlotParam -> TypedTree.SlotParam
    val GenActualSlotsig:
      m:Range.range ->
        cenv:cenv ->
          eenv:IlxGenEnv ->
            TypedTree.SlotSig ->
              methTyparsOfOverridingMethod:TypedTree.Typars ->
                methodParams:TypedTree.Val list ->
                  AbstractIL.IL.ILParameter list * AbstractIL.IL.ILReturn
    val GenNameOfOverridingMethod:
      cenv:cenv -> useMethodImpl:bool * slotsig:TypedTree.SlotSig -> string
    val GenMethodImpl:
      cenv:cenv ->
        eenv:IlxGenEnv ->
          useMethodImpl:bool * TypedTree.SlotSig ->
            m:Range.range ->
              string *
              (AbstractIL.IL.ILType * TypedTree.Typar list ->
                 AbstractIL.IL.ILMethodImplDef)
    val bindBaseOrThisVarOpt:
      cenv:cenv ->
        eenv:IlxGenEnv -> baseValOpt:TypedTree.Val option -> IlxGenEnv
    val fixupVirtualSlotFlags:
      mdef:AbstractIL.IL.ILMethodDef -> AbstractIL.IL.ILMethodDef
    val renameMethodDef:
      nameOfOverridingMethod:string ->
        mdef:AbstractIL.IL.ILMethodDef -> AbstractIL.IL.ILMethodDef
    val fixupMethodImplFlags:
      mdef:AbstractIL.IL.ILMethodDef -> AbstractIL.IL.ILMethodDef
    val GenObjectMethod:
      cenv:cenv ->
        eenvinner:IlxGenEnv ->
          cgbuf:CodeGenBuffer ->
            useMethodImpl:bool ->
              tmethod:TypedTree.ObjExprMethod ->
                ((bool *
                  (AbstractIL.IL.ILType * TypedTree.Typar list ->
                     AbstractIL.IL.ILMethodImplDef) * TypedTree.Typars) *
                 AbstractIL.IL.ILMethodDef) list
    val GenObjectExpr:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenvouter:IlxGenEnv ->
            expr:TypedTree.Expr ->
              baseType:TypedTree.TType * baseValOpt:TypedTree.Val option *
              basecall:TypedTree.Expr * overrides:TypedTree.ObjExprMethod list *
              interfaceImpls:(TypedTree.TType * TypedTree.ObjExprMethod list) list *
              m:Range.range -> sequel:sequel -> unit
    val GenSequenceExpr:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenvouter:IlxGenEnv ->
            nextEnumeratorValRef:TypedTree.ValRef * pcvref:TypedTree.ValRef *
            currvref:TypedTree.ValRef * stateVars:TypedTree.ValRef list *
            generateNextExpr:TypedTree.Expr * closeExpr:TypedTree.Expr *
            checkCloseExpr:TypedTree.Expr * seqElemTy:TypedTree.TType *
            m:Range.range -> sequel:sequel -> unit
    val GenClosureTypeDefs:
      cenv:cenv ->
        tref:AbstractIL.IL.ILTypeRef *
        ilGenParams:AbstractIL.IL.ILGenericParameterDefs *
        attrs:AbstractIL.IL.ILAttribute list *
        ilCloAllFreeVars:AbstractIL.Extensions.ILX.Types.IlxClosureFreeVar [] *
        ilCloLambdas:AbstractIL.Extensions.ILX.Types.IlxClosureLambdas *
        ilCtorBody:AbstractIL.IL.ILMethodBody *
        mdefs:AbstractIL.IL.ILMethodDef list *
        mimpls:AbstractIL.IL.ILMethodImplDef list * ext:AbstractIL.IL.ILType *
        ilIntfTys:AbstractIL.IL.ILType list *
        cloSpec:AbstractIL.Extensions.ILX.Types.IlxClosureSpec option ->
          AbstractIL.IL.ILTypeDef list
    val GenStaticDelegateClosureTypeDefs:
      cenv:cenv ->
        tref:AbstractIL.IL.ILTypeRef *
        ilGenParams:AbstractIL.IL.ILGenericParameterDefs *
        attrs:AbstractIL.IL.ILAttribute list *
        ilCloAllFreeVars:AbstractIL.Extensions.ILX.Types.IlxClosureFreeVar [] *
        ilCloLambdas:AbstractIL.Extensions.ILX.Types.IlxClosureLambdas *
        ilCtorBody:AbstractIL.IL.ILMethodBody *
        mdefs:AbstractIL.IL.ILMethodDef list *
        mimpls:AbstractIL.IL.ILMethodImplDef list * ext:AbstractIL.IL.ILType *
        ilIntfTys:AbstractIL.IL.ILType list *
        staticCloInfo:AbstractIL.Extensions.ILX.Types.IlxClosureSpec option ->
          AbstractIL.IL.ILTypeDef list
    val GenGenericParams:
      cenv:cenv ->
        eenv:IlxGenEnv ->
          tps:TypedTree.Typar list -> AbstractIL.IL.ILGenericParameterDefs
    val GenGenericArgs:
      m:Range.range ->
        tyenv:TypeReprEnv ->
          tps:TypedTree.Typar list -> AbstractIL.IL.ILGenericArgs
    val GenLambdaClosure:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            isLocalTypeFunc:bool ->
              thisVars:TypedTree.ValRef list ->
                expr:TypedTree.Expr -> IlxClosureInfo * Range.range
    val GenClosureAlloc:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv -> cloinfo:IlxClosureInfo * m:Range.range -> unit
    val GenLambda:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            isLocalTypeFunc:bool ->
              thisVars:TypedTree.ValRef list ->
                expr:TypedTree.Expr -> sequel:sequel -> unit
    val GenTypeOfVal:
      cenv:cenv -> eenv:IlxGenEnv -> v:TypedTree.Val -> AbstractIL.IL.ILType
    val GenFreevar:
      cenv:cenv ->
        m:Range.range ->
          eenvouter:IlxGenEnv ->
            tyenvinner:TypeReprEnv -> fv:TypedTree.Val -> AbstractIL.IL.ILType
    val GetIlxClosureFreeVars:
      cenv:cenv ->
        m:Range.range ->
          thisVars:TypedTree.ValRef list ->
            eenvouter:IlxGenEnv ->
              takenNames:string list ->
                expr:TypedTree.Expr ->
                  TypedTree.Attribs * TypedTree.Typar list *
                  TypedTree.Typar list * TypedTree.Typar list *
                  TypedTreeOps.TraitWitnessInfos * TypedTree.Val list *
                  AbstractIL.IL.ILTypeRef *
                  AbstractIL.Extensions.ILX.Types.IlxClosureFreeVar [] *
                  IlxGenEnv
    val GetIlxClosureInfo:
      cenv:cenv ->
        m:Range.range ->
          isLocalTypeFunc:bool ->
            canUseStaticField:bool ->
              thisVars:TypedTree.ValRef list ->
                eenvouter:IlxGenEnv ->
                  expr:TypedTree.Expr ->
                    IlxClosureInfo * TypedTree.Expr * IlxGenEnv
    val IsNamedLocalTypeFuncVal:
      g:TcGlobals.TcGlobals -> v:TypedTree.Val -> expr:TypedTree.Expr -> bool
    val GenNamedLocalTypeFuncContractInfo:
      cenv:cenv ->
        eenv:IlxGenEnv ->
          m:Range.range ->
            cloinfo:IlxClosureInfo ->
              AbstractIL.IL.ILGenericParameterDef list *
              AbstractIL.IL.ILGenericParameterDefs * AbstractIL.IL.ILTypeSpec *
              AbstractIL.IL.ILType
    val GenDelegateExpr:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenvouter:IlxGenEnv ->
            expr:TypedTree.Expr ->
              TypedTree.ObjExprMethod * m:Range.range -> sequel:sequel -> unit
    val GenStaticOptimization:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            constraints:TypedTree.StaticOptimization list * e2:TypedTree.Expr *
            e3:TypedTree.Expr * _m:Range.range -> sequel:sequel -> unit
    val IsSequelImmediate: sequel:sequel -> bool
    val GenJoinPoint:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          pos:string ->
            eenv:IlxGenEnv ->
              ty:TypedTree.TType ->
                m:Range.range ->
                  sequel:sequel ->
                    sequel * Mark * AbstractIL.IL.ILType list * sequel
    val GenDecisionTreeAndTargets:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          stackAtTargets:AbstractIL.IL.ILType list ->
            eenv:IlxGenEnv ->
              tree:TypedTree.DecisionTree ->
                targets:TypedTree.DecisionTreeTarget array ->
                  repeatSP:(unit -> unit) ->
                    sequel:sequel -> contf:(FakeUnit -> FakeUnit) -> FakeUnit
    val GenPostponedDecisionTreeTargets:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          targetInfos:System.Collections.Generic.KeyValuePair<int,
                                                              ((Mark * Mark *
                                                                IlxGenEnv *
                                                                TypedTree.Expr *
                                                                SyntaxTree.DebugPointForTarget *
                                                                (unit -> unit) *
                                                                TypedTree.Val list *
                                                                TypedTree.Bindings *
                                                                Mark * Mark) *
                                                               bool)> list ->
            stackAtTargets:AbstractIL.IL.ILType list ->
              sequel:sequel -> contf:(FakeUnit -> FakeUnit) -> FakeUnit
    val TryFindTargetInfo:
      targetInfos:Lib.IntMap<'a * 'b> -> n:int -> 'a option
    val GenDecisionTreeAndTargetsInner:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          inplabOpt:Mark option ->
            stackAtTargets:AbstractIL.IL.ILType list ->
              eenv:IlxGenEnv ->
                tree:TypedTree.DecisionTree ->
                  targets:TypedTree.DecisionTreeTarget array ->
                    repeatSP:(unit -> unit) ->
                      targetInfos:AbstractIL.Internal.Zmap<int,
                                                           ((Mark * Mark *
                                                             IlxGenEnv *
                                                             TypedTree.Expr *
                                                             SyntaxTree.DebugPointForTarget *
                                                             (unit -> unit) *
                                                             TypedTree.Val list *
                                                             TypedTree.Bindings *
                                                             Mark * Mark) * bool)> ->
                        sequel:sequel ->
                          contf:(AbstractIL.Internal.Zmap<int,
                                                          ((Mark * Mark *
                                                            IlxGenEnv *
                                                            TypedTree.Expr *
                                                            SyntaxTree.DebugPointForTarget *
                                                            (unit -> unit) *
                                                            TypedTree.Val list *
                                                            TypedTree.Bindings *
                                                            Mark * Mark) * bool)> ->
                                   FakeUnit) -> FakeUnit
    val GetTarget: targets:'c [] -> n:int -> 'c
    val GenDecisionTreeSuccess:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          inplabOpt:Mark option ->
            stackAtTargets:AbstractIL.IL.ILType list ->
              eenv:IlxGenEnv ->
                es:TypedTree.Exprs ->
                  targetIdx:int ->
                    targets:TypedTree.DecisionTreeTarget array ->
                      repeatSP:(unit -> unit) ->
                        targetInfos:AbstractIL.Internal.Zmap<int,
                                                             ((Mark * Mark *
                                                               IlxGenEnv *
                                                               TypedTree.Expr *
                                                               SyntaxTree.DebugPointForTarget *
                                                               (unit -> unit) *
                                                               TypedTree.Val list *
                                                               TypedTree.Bindings *
                                                               Mark * Mark) *
                                                              bool)> ->
                          sequel:sequel ->
                            AbstractIL.Internal.Zmap<int,
                                                     ((Mark * Mark * IlxGenEnv *
                                                       TypedTree.Expr *
                                                       SyntaxTree.DebugPointForTarget *
                                                       (unit -> unit) *
                                                       TypedTree.Val list *
                                                       TypedTree.Bindings * Mark *
                                                       Mark) * bool)> *
                            (IlxGenEnv * EmitDebugPointState * TypedTree.Expr *
                             sequel) option
    val GenDecisionTreeTarget:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          stackAtTargets:AbstractIL.IL.ILType list ->
            targetMarkBeforeBinds:Mark * targetMarkAfterBinds:Mark *
            eenvAtTarget:IlxGenEnv * successExpr:TypedTree.Expr *
            spTarget:SyntaxTree.DebugPointForTarget * repeatSP:(unit -> unit) *
            vs:TypedTree.Val list * binds:TypedTree.Bindings * startScope:Mark *
          Scope:Mark ->
              sequel:sequel ->
                IlxGenEnv * EmitDebugPointState * TypedTree.Expr * sequel
    val GenDecisionTreeSwitch:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          inplabOpt:Mark option ->
            stackAtTargets:AbstractIL.IL.ILType list ->
              eenv:IlxGenEnv ->
                e:TypedTree.Expr ->
                  cases:TypedTree.DecisionTreeCase list ->
                    defaultTargetOpt:TypedTree.DecisionTree option ->
                      switchm:Range.range ->
                        targets:TypedTree.DecisionTreeTarget array ->
                          repeatSP:(unit -> unit) ->
                            targetInfos:AbstractIL.Internal.Zmap<int,
                                                                 ((Mark * Mark *
                                                                   IlxGenEnv *
                                                                   TypedTree.Expr *
                                                                   SyntaxTree.DebugPointForTarget *
                                                                   (unit -> unit) *
                                                                   TypedTree.Val list *
                                                                   TypedTree.Bindings *
                                                                   Mark * Mark) *
                                                                  bool)> ->
                              sequel:sequel ->
                                contf:(AbstractIL.Internal.Zmap<int,
                                                                ((Mark * Mark *
                                                                  IlxGenEnv *
                                                                  TypedTree.Expr *
                                                                  SyntaxTree.DebugPointForTarget *
                                                                  (unit -> unit) *
                                                                  TypedTree.Val list *
                                                                  TypedTree.Bindings *
                                                                  Mark * Mark) *
                                                                 bool)> ->
                                         FakeUnit) -> FakeUnit
    val GenDecisionTreeCases:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          stackAtTargets:AbstractIL.IL.ILType list ->
            eenv:IlxGenEnv ->
              defaultTargetOpt:TypedTree.DecisionTree option ->
                targets:TypedTree.DecisionTreeTarget array ->
                  repeatSP:(unit -> unit) ->
                    targetInfos:AbstractIL.Internal.Zmap<int,
                                                         ((Mark * Mark *
                                                           IlxGenEnv *
                                                           TypedTree.Expr *
                                                           SyntaxTree.DebugPointForTarget *
                                                           (unit -> unit) *
                                                           TypedTree.Val list *
                                                           TypedTree.Bindings *
                                                           Mark * Mark) * bool)> ->
                      sequel:sequel ->
                        caseLabels:Mark list ->
                          cases:TypedTree.DecisionTreeCase list ->
                            contf:(AbstractIL.Internal.Zmap<int,
                                                            ((Mark * Mark *
                                                              IlxGenEnv *
                                                              TypedTree.Expr *
                                                              SyntaxTree.DebugPointForTarget *
                                                              (unit -> unit) *
                                                              TypedTree.Val list *
                                                              TypedTree.Bindings *
                                                              Mark * Mark) *
                                                             bool)> -> FakeUnit) ->
                              FakeUnit
    val ( |BoolExpr|_| ): _arg42:TypedTree.Expr -> bool option
    val GenDecisionTreeTest:
      cenv:cenv ->
        cloc:CompileLocation ->
          cgbuf:CodeGenBuffer ->
            stackAtTargets:AbstractIL.IL.ILType list ->
              e:TypedTree.Expr ->
                tester:(Pops * Pushes *
                        Choice<(bool *
                                AbstractIL.Extensions.ILX.Types.IlxUnionSpec *
                                int),AbstractIL.IL.ILInstr>) option ->
                  eenv:IlxGenEnv ->
                    successTree:TypedTree.DecisionTree ->
                      failureTree:TypedTree.DecisionTree ->
                        targets:TypedTree.DecisionTreeTarget array ->
                          repeatSP:(unit -> unit) ->
                            targetInfos:AbstractIL.Internal.Zmap<int,
                                                                 ((Mark * Mark *
                                                                   IlxGenEnv *
                                                                   TypedTree.Expr *
                                                                   SyntaxTree.DebugPointForTarget *
                                                                   (unit -> unit) *
                                                                   TypedTree.Val list *
                                                                   TypedTree.Bindings *
                                                                   Mark * Mark) *
                                                                  bool)> ->
                              sequel:sequel ->
                                contf:(AbstractIL.Internal.Zmap<int,
                                                                ((Mark * Mark *
                                                                  IlxGenEnv *
                                                                  TypedTree.Expr *
                                                                  SyntaxTree.DebugPointForTarget *
                                                                  (unit -> unit) *
                                                                  TypedTree.Val list *
                                                                  TypedTree.Bindings *
                                                                  Mark * Mark) *
                                                                 bool)> ->
                                         FakeUnit) -> FakeUnit
    val GenLetRecFixup:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            ilxCloSpec:AbstractIL.Extensions.ILX.Types.IlxClosureSpec *
            e:TypedTree.Expr * ilField:AbstractIL.IL.ILFieldSpec *
            e2:TypedTree.Expr * _m:'d -> unit
    val GenLetRecBindings:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv -> allBinds:TypedTree.Bindings * m:Range.range -> unit
    val GenLetRec:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            binds:TypedTree.Bindings * body:TypedTree.Expr * m:Range.range ->
              sequel:sequel -> unit
    val GenDebugPointForBind:
      cenv:cenv ->
        cgbuf:CodeGenBuffer -> bind:TypedTree.Binding -> EmitDebugPointState
    val GenBinding:
      cenv:cenv ->
        cgbuf:CodeGenBuffer -> eenv:IlxGenEnv -> bind:TypedTree.Binding -> unit
    val ComputeMemberAccessRestrictedBySig:
      eenv:IlxGenEnv -> vspec:TypedTree.Val -> AbstractIL.IL.ILMemberAccess
    val ComputeMethodAccessRestrictedBySig:
      eenv:IlxGenEnv -> vspec:TypedTree.Val -> AbstractIL.IL.ILMemberAccess
    val GenBindingAfterDebugPoint:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            sp:EmitDebugPointState ->
              TypedTree.Binding -> startScopeMarkOpt:Mark option -> unit
    val GenMarshal:
      cenv:cenv ->
        attribs:TypedTree.Attrib list ->
          AbstractIL.IL.ILNativeType option * TypedTree.Attrib list
    val GenParamAttribs:
      cenv:cenv ->
        paramTy:TypedTree.TType ->
          attribs:TypedTree.Attribs ->
            bool * bool * bool * AbstractIL.IL.ILFieldInit option *
            AbstractIL.IL.ILNativeType option * TypedTree.Attribs
    val GenParams:
      cenv:cenv ->
        eenv:IlxGenEnv ->
          m:Range.range ->
            mspec:AbstractIL.IL.ILMethodSpec ->
              witnessInfos:TypedTreeOps.TraitWitnessInfos ->
                argInfos:TypedTree.ArgReprInfo list ->
                  methArgTys:TypedTree.TType list ->
                    implValsOpt:TypedTree.Val list option ->
                      AbstractIL.IL.ILParameter list
    val GenReturnInfo:
      cenv:cenv ->
        eenv:IlxGenEnv ->
          returnTy:TypedTree.TType option ->
            ilRetTy:AbstractIL.IL.ILType ->
              retInfo:TypedTree.ArgReprInfo -> AbstractIL.IL.ILReturn
    val GenPropertyForMethodDef:
      compileAsInstance:bool ->
        tref:AbstractIL.IL.ILTypeRef ->
          mdef:AbstractIL.IL.ILMethodDef ->
            v:TypedTree.Val ->
              memberInfo:TypedTree.ValMemberInfo ->
                ilArgTys:AbstractIL.IL.ILTypes ->
                  ilPropTy:AbstractIL.IL.ILType ->
                    ilAttrs:AbstractIL.IL.ILAttributes ->
                      compiledName:string option -> AbstractIL.IL.ILPropertyDef
    val GenEventForProperty:
      cenv:cenv ->
        eenvForMeth:IlxGenEnv ->
          mspec:AbstractIL.IL.ILMethodSpec ->
            v:TypedTree.Val ->
              ilAttrsThatGoOnPrimaryItem:AbstractIL.IL.ILAttribute list ->
                m:Range.range ->
                  returnTy:TypedTree.TType -> AbstractIL.IL.ILEventDef
    val ComputeUseMethodImpl:
      cenv:cenv -> v:TypedTree.Val * slotsig:TypedTree.SlotSig -> bool
    val ComputeMethodImplNameFixupForMemberBinding:
      cenv:cenv ->
        v:TypedTree.Val * memberInfo:TypedTree.ValMemberInfo -> string option
    val ComputeFlagFixupsForMemberBinding:
      cenv:cenv ->
        v:TypedTree.Val * memberInfo:TypedTree.ValMemberInfo ->
          (#AbstractIL.IL.ILMethodDef -> AbstractIL.IL.ILMethodDef) list
    val ComputeMethodImplAttribs:
      cenv:cenv ->
        _v:TypedTree.Val ->
          attrs:TypedTree.Attribs ->
            bool * bool * bool * bool * TypedTree.Attrib list
    val DelayGenMethodForBinding:
      cenv:cenv ->
        mgbuf:AssemblyBuilder ->
          eenv:IlxGenEnv ->
            TypedTree.Val * AbstractIL.IL.ILMethodSpec * bool * bool *
            AbstractIL.IL.ILMemberAccess * TypedTree.Typars * TypedTree.Typars *
            TypedTree.TraitWitnessInfo list * TypedTreeOps.CurriedArgInfos *
            TypedTree.ArgReprInfo list * TypedTree.TType list *
            TypedTree.ArgReprInfo * TypedTree.ValReprInfo * TypedTree.Val option *
            TypedTree.Val option * TypedTree.Typars * TypedTree.Val list *
            TypedTree.Expr * TypedTree.TType -> unit
    val GenMethodForBinding:
      cenv:cenv ->
        mgbuf:AssemblyBuilder ->
          eenv:IlxGenEnv ->
            v:TypedTree.Val * mspec:AbstractIL.IL.ILMethodSpec *
            hasWitnessEntry:bool * generateWitnessArgs:bool *
            access:AbstractIL.IL.ILMemberAccess * ctps:TypedTree.Typars *
            mtps:TypedTree.Typars * witnessInfos:TypedTree.TraitWitnessInfo list *
            curriedArgInfos:TypedTreeOps.CurriedArgInfos *
            paramInfos:TypedTree.ArgReprInfo list * argTys:TypedTree.TType list *
            retInfo:TypedTree.ArgReprInfo * topValInfo:TypedTree.ValReprInfo *
            ctorThisValOpt:TypedTree.Val option *
            baseValOpt:TypedTree.Val option * methLambdaTypars:TypedTree.Typars *
            methLambdaVars:TypedTree.Val list * methLambdaBody:TypedTree.Expr *
            returnTy:TypedTree.TType -> unit
    val GenPInvokeMethod:
      nm:string * dll:string * namedArgs:TypedTree.AttribNamedArg list ->
        bool * AbstractIL.IL.MethodBody
    val GenBindings:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv -> binds:TypedTree.Bindings -> unit
    val GenSetVal:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            vref:TypedTree.ValRef * e:TypedTree.Expr * m:Range.range ->
              sequel:sequel -> unit
    val GenGetValRefAndSequel:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            m:Range.range ->
              v:TypedTree.ValRef ->
                storeSequel:(TypedTree.TypeInst * TypedTree.Exprs * Range.range *
                             sequel) option -> unit
    val GenGetVal:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            v:TypedTree.ValRef * m:Range.range -> sequel:sequel -> unit
    val GenBindingRhs:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            sp:EmitDebugPointState ->
              vspec:TypedTree.Val -> e:TypedTree.Expr -> unit
    val CommitStartScope:
      cgbuf:CodeGenBuffer -> startScopeMarkOpt:Mark option -> unit
    val EmitInitLocal:
      cgbuf:CodeGenBuffer -> ty:AbstractIL.IL.ILType -> idx:int -> unit
    val EmitSetLocal: cgbuf:CodeGenBuffer -> idx:int -> unit
    val EmitGetLocal:
      cgbuf:CodeGenBuffer -> ty:AbstractIL.IL.ILType -> idx:int -> unit
    val EmitSetStaticField:
      cgbuf:CodeGenBuffer -> fspec:AbstractIL.IL.ILFieldSpec -> unit
    val EmitGetStaticFieldAddr:
      cgbuf:CodeGenBuffer ->
        ty:AbstractIL.IL.ILType -> fspec:AbstractIL.IL.ILFieldSpec -> unit
    val EmitGetStaticField:
      cgbuf:CodeGenBuffer ->
        ty:AbstractIL.IL.ILType -> fspec:AbstractIL.IL.ILFieldSpec -> unit
    val GenSetStorage:
      m:Range.range -> cgbuf:CodeGenBuffer -> storage:ValStorage -> unit
    val CommitGetStorageSequel:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            m:Range.range ->
              ty:TypedTree.TType ->
                localCloInfo:Ref<NamedLocalIlxClosureInfo> option ->
                  storeSequel:(TypedTree.TType list * TypedTree.Expr list *
                               Range.range * sequel) option -> unit
    val GenGetStorageAndSequel:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            m:Range.range ->
              ty:TypedTree.TType * ilTy:AbstractIL.IL.ILType ->
                storage:ValStorage ->
                  storeSequel:(TypedTree.TType list * TypedTree.Expr list *
                               Range.range * sequel) option -> unit
    val GenGetLocalVals:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenvouter:IlxGenEnv -> m:Range.range -> fvs:TypedTree.Val list -> unit
    val GenGetLocalVal:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            m:Range.range ->
              vspec:TypedTree.Val ->
                storeSequel:(TypedTree.TType list * TypedTree.Expr list *
                             Range.range * sequel) option -> unit
    val GenGetLocalVRef:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            m:Range.range ->
              vref:TypedTree.ValRef ->
                storeSequel:(TypedTree.TType list * TypedTree.Expr list *
                             Range.range * sequel) option -> unit
    val GenStoreVal:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv -> m:Range.range -> vspec:TypedTree.Val -> unit
    val AllocLocal:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            compgen:bool ->
              v:string * ty:AbstractIL.IL.ILType * isFixed:bool ->
                Mark * Mark -> int * bool * IlxGenEnv
    val AllocLocalVal:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          v:TypedTree.Val ->
            eenv:IlxGenEnv ->
              repr:TypedTree.Expr option ->
                Mark * Mark -> ValStorage option * IlxGenEnv
    val AllocStorageForBind:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          Mark * Mark -> eenv:IlxGenEnv -> bind:TypedTree.Binding -> IlxGenEnv
    val AllocStorageForBinds:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          Mark * Mark -> eenv:IlxGenEnv -> binds:TypedTree.Bindings -> IlxGenEnv
    val AllocValForBind:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          Mark * Mark ->
            eenv:IlxGenEnv -> TypedTree.Binding -> ValStorage option * IlxGenEnv
    val AllocTopValWithinExpr:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          cloc:CompileLocation ->
            Mark * Mark -> v:TypedTree.Val -> eenv:IlxGenEnv -> IlxGenEnv
    val EmitSaveStack:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          eenv:IlxGenEnv ->
            m:Range.range ->
              Mark * Mark -> (AbstractIL.IL.ILType list * int list) * IlxGenEnv
    val EmitRestoreStack:
      cgbuf:CodeGenBuffer ->
        savedStack:AbstractIL.IL.ILType list * savedStackLocals:int list -> unit
    val GenAttribArg:
      amap:Import.ImportMap ->
        g:TcGlobals.TcGlobals ->
          eenv:IlxGenEnv ->
            x:TypedTree.Expr ->
              ilArgTy:AbstractIL.IL.ILType -> AbstractIL.IL.ILAttribElem
    val GenAttr:
      amap:Import.ImportMap ->
        g:TcGlobals.TcGlobals ->
          eenv:IlxGenEnv -> TypedTree.Attrib -> AbstractIL.IL.ILAttribute
    val GenAttrs:
      cenv:cenv ->
        eenv:IlxGenEnv ->
          attrs:TypedTree.Attribs -> AbstractIL.IL.ILAttribute list
    val GenCompilationArgumentCountsAttr:
      cenv:cenv -> v:TypedTree.Val -> AbstractIL.IL.ILAttribute list
    val CreatePermissionSets:
      cenv:cenv ->
        eenv:IlxGenEnv ->
          securityAttributes:TypedTree.Attrib list ->
            AbstractIL.IL.ILSecurityDecl list
    val GenTypeDefForCompLoc:
      cenv:cenv * eenv:IlxGenEnv * mgbuf:AssemblyBuilder * cloc:CompileLocation *
      hidden:bool * attribs:TypedTree.Attribs *
      initTrigger:AbstractIL.IL.ILTypeInit * eliminateIfEmpty:bool *
      addAtEnd:bool -> unit
    val GenModuleExpr:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          qname:SyntaxTree.QualifiedNameOfFile ->
            lazyInitInfo:ResizeArray<(AbstractIL.IL.ILFieldSpec ->
                                        AbstractIL.IL.ILInstr list ->
                                        AbstractIL.IL.ILInstr list -> unit)> ->
              eenv:IlxGenEnv -> x:TypedTree.ModuleOrNamespaceExprWithSig -> unit
    val GenModuleDefs:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          qname:SyntaxTree.QualifiedNameOfFile ->
            lazyInitInfo:ResizeArray<(AbstractIL.IL.ILFieldSpec ->
                                        AbstractIL.IL.ILInstr list ->
                                        AbstractIL.IL.ILInstr list -> unit)> ->
              eenv:IlxGenEnv ->
                mdefs:TypedTree.ModuleOrNamespaceExpr list -> unit
    val GenModuleDef:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          qname:SyntaxTree.QualifiedNameOfFile ->
            lazyInitInfo:ResizeArray<(AbstractIL.IL.ILFieldSpec ->
                                        AbstractIL.IL.ILInstr list ->
                                        AbstractIL.IL.ILInstr list -> unit)> ->
              eenv:IlxGenEnv -> x:TypedTree.ModuleOrNamespaceExpr -> unit
    val GenModuleBinding:
      cenv:cenv ->
        cgbuf:CodeGenBuffer ->
          qname:SyntaxTree.QualifiedNameOfFile ->
            lazyInitInfo:ResizeArray<(AbstractIL.IL.ILFieldSpec ->
                                        AbstractIL.IL.ILInstr list ->
                                        AbstractIL.IL.ILInstr list -> unit)> ->
              eenv:IlxGenEnv ->
                m:Range.range -> x:TypedTree.ModuleOrNamespaceBinding -> unit
    val GenImplFile:
      cenv:cenv ->
        mgbuf:AssemblyBuilder ->
          mainInfoOpt:TypedTree.Attribs option ->
            eenv:IlxGenEnv ->
              implFile:TypedTree.TypedImplFileAfterOptimization -> IlxGenEnv
    val GenForceWholeFileInitializationAsPartOfCCtor:
      cenv:cenv ->
        mgbuf:AssemblyBuilder ->
          lazyInitInfo:ResizeArray<(AbstractIL.IL.ILFieldSpec ->
                                      AbstractIL.IL.ILInstr list ->
                                      AbstractIL.IL.ILInstr list -> unit)> ->
            tref:AbstractIL.IL.ILTypeRef -> m:Range.range -> unit
    val GenEqualsOverrideCallingIComparable:
      cenv:cenv ->
        tcref:TypedTree.TyconRef * ilThisTy:AbstractIL.IL.ILType * _ilThatTy:'f ->
          AbstractIL.IL.ILMethodDef
    val GenFieldInit:
      m:Range.range -> c:TypedTree.Const -> AbstractIL.IL.ILFieldInit
    val GenWitnessParams:
      cenv:cenv ->
        eenv:IlxGenEnv ->
          m:Range.range ->
            witnessInfos:TypedTreeOps.TraitWitnessInfos ->
              AbstractIL.IL.ILParameter list
    val GenAbstractBinding:
      cenv:cenv ->
        eenv:IlxGenEnv ->
          tref:AbstractIL.IL.ILTypeRef ->
            vref:TypedTree.ValRef ->
              AbstractIL.IL.ILMethodDef list * AbstractIL.IL.ILPropertyDef list *
              AbstractIL.IL.ILEventDef list
    val GenToStringMethod:
      cenv:cenv ->
        eenv:IlxGenEnv ->
          ilThisTy:AbstractIL.IL.ILType ->
            m:Range.range -> AbstractIL.IL.ILMethodDef list
    val GenTypeDef:
      cenv:cenv ->
        mgbuf:AssemblyBuilder ->
          lazyInitInfo:ResizeArray<(AbstractIL.IL.ILFieldSpec ->
                                      AbstractIL.IL.ILInstr list ->
                                      AbstractIL.IL.ILInstr list -> unit)> ->
            eenv:IlxGenEnv -> m:Range.range -> tycon:TypedTree.Tycon -> unit
    val GenExnDef:
      cenv:cenv ->
        mgbuf:AssemblyBuilder ->
          eenv:IlxGenEnv -> m:Range.range -> exnc:TypedTree.Tycon -> unit
    val CodegenAssembly:
      cenv:cenv ->
        eenv:IlxGenEnv ->
          mgbuf:AssemblyBuilder ->
            implFiles:TypedTree.TypedImplFileAfterOptimization list -> unit
    val GetEmptyIlxGenEnv:
      g:TcGlobals.TcGlobals -> ccu:TypedTree.CcuThunk -> IlxGenEnv
    type IlxGenResults =
      { ilTypeDefs: AbstractIL.IL.ILTypeDef list
        ilAssemAttrs: AbstractIL.IL.ILAttribute list
        ilNetModuleAttrs: AbstractIL.IL.ILAttribute list
        topAssemblyAttrs: TypedTree.Attribs
        permissionSets: AbstractIL.IL.ILSecurityDecl list
        quotationResourceInfo: (AbstractIL.IL.ILTypeRef list * byte []) list }
    val GenerateCode:
      cenv:cenv * anonTypeTable:AnonTypeGenerationTable * eenv:IlxGenEnv *
      TypedTree.TypedAssemblyAfterOptimization * assemAttribs:TypedTree.Attribs *
      moduleAttribs:TypedTree.Attribs -> IlxGenResults
    type ExecutionContext =
      { LookupFieldRef: AbstractIL.IL.ILFieldRef -> System.Reflection.FieldInfo
        LookupMethodRef:
          AbstractIL.IL.ILMethodRef -> System.Reflection.MethodInfo
        LookupTypeRef: AbstractIL.IL.ILTypeRef -> System.Type
        LookupType: AbstractIL.IL.ILType -> System.Type }
    val defaultOf: (System.Type -> obj)
    val LookupGeneratedValue:
      amap:Import.ImportMap ->
        ctxt:ExecutionContext ->
          eenv:IlxGenEnv -> v:TypedTree.Val -> (obj * System.Type) option
    val SetGeneratedValue:
      ctxt:ExecutionContext ->
        g:TcGlobals.TcGlobals ->
          eenv:IlxGenEnv ->
            isForced:bool -> v:TypedTree.Val -> value:obj -> unit
    val ClearGeneratedValue:
      ctxt:ExecutionContext ->
        g:TcGlobals.TcGlobals -> eenv:IlxGenEnv -> v:TypedTree.Val -> unit
    type IlxAssemblyGenerator =
  
        new: Import.ImportMap * TcGlobals.TcGlobals * ConstraintSolver.TcValF *
              TypedTree.CcuThunk -> IlxAssemblyGenerator
        member AddExternalCcus: TypedTree.CcuThunk list -> unit
        member
          AddIncrementalLocalAssemblyFragment: isIncrementalFragment:bool *
                                                fragName:string *
                                                typedImplFiles:TypedTree.TypedImplFile list ->
                                                  unit
        member ClearGeneratedValue: ExecutionContext * TypedTree.Val -> unit
        member
          ForceSetGeneratedValue: ExecutionContext * TypedTree.Val * obj ->
                                     unit
        member
          GenerateCode: IlxGenOptions *
                         TypedTree.TypedAssemblyAfterOptimization *
                         TypedTree.Attribs * TypedTree.Attribs -> IlxGenResults
        member
          LookupGeneratedValue: ExecutionContext * TypedTree.Val ->
                                   (obj * System.Type) option
    


namespace FSharp.Compiler
  module internal DotNetFrameworkDependencies =
    type private TypeInThisAssembly
    val fSharpCompilerLocation: string
    val inline ifEmptyUse: alternative:string -> filename:string -> string
    val getFSharpCoreLibraryName: string
    val getFsiLibraryName: string
    val getDefaultFSharpCoreLocation: string
    val getDefaultFsiLibraryLocation: string
    val implementationAssemblyDir: string
    val getDefaultSystemValueTupleReference: unit -> string option
    val zeroVersion: System.Version
    val version: string option
    val frameworkRefsPackDirectoryRoot: string option
    val netcoreTfm: string option
    val getWindowsDesktopTfm: unit -> string
    val executionTfm: string
    val executionRid: string
    val isInReferenceAssemblyPackDirectory: filename:string -> bool
    val frameworkRefsPackDirectory: string option
    val getDependenciesOf:
      assemblyReferences:string list ->
        System.Collections.Generic.Dictionary<string,string>
    val getDesktopDefaultReferences: useFsiAuxLib:bool -> string list
    val fetchPathsForDefaultReferencesForScriptsAndOutOfProjectSources:
      useFsiAuxLib:bool ->
        useSdkRefs:bool -> assumeDotNetFramework:bool -> string list
    val defaultReferencesForScriptsAndOutOfProjectSources:
      useFsiAuxLib:bool ->
        assumeDotNetFramework:bool -> useSdkRefs:bool -> string list
    val systemAssemblies: System.Collections.Generic.HashSet<string>
    val basicReferencesForScriptLoadClosure:
      useFsiAuxLib:bool ->
        useSdkRefs:bool -> assumeDotNetFramework:bool -> string list


namespace Microsoft.DotNet.DependencyManager
  type AssemblyResolutionProbe =
    delegate of Unit -> seq<string>
  type AssemblyResolveHandlerCoreclr =

      interface System.IDisposable
      new: assemblyProbingPaths:AssemblyResolutionProbe ->
              AssemblyResolveHandlerCoreclr
  
  type AssemblyResolveHandlerDeskTop =

      interface System.IDisposable
      new: assemblyProbingPaths:AssemblyResolutionProbe ->
              AssemblyResolveHandlerDeskTop
  
  type AssemblyResolveHandler =

      interface System.IDisposable
      new: assemblyProbingPaths:AssemblyResolutionProbe ->
              AssemblyResolveHandler
  

namespace Microsoft.DotNet.DependencyManager
  type NativeResolutionProbe =
    delegate of Unit -> seq<string>
  type NativeAssemblyLoadContext =

      inherit System.Runtime.Loader.AssemblyLoadContext
      new: unit -> NativeAssemblyLoadContext
      override
        Load: _path:System.Reflection.AssemblyName ->
                 System.Reflection.Assembly
      member LoadNativeLibrary: path:string -> System.IntPtr
      static member NativeLoadContext: NativeAssemblyLoadContext
  
  type NativeDllResolveHandlerCoreClr =

      interface System.IDisposable
      new: nativeProbingRoots:NativeResolutionProbe ->
              NativeDllResolveHandlerCoreClr
  
  type NativeDllResolveHandler =

      interface System.IDisposable
      new: _nativeProbingRoots:NativeResolutionProbe -> NativeDllResolveHandler
      member internal RefreshPathsInEnvironment: seq<string> -> unit
  

namespace Microsoft.DotNet.DependencyManager
  module ReflectionHelper =
    val dependencyManagerPattern: string
    val dependencyManagerAttributeName: string
    val resolveDependenciesMethodName: string
    val namePropertyName: string
    val keyPropertyName: string
    val helpMessagesPropertyName: string
    val arrEmpty: string []
    val seqEmpty: seq<string>
    val assemblyHasAttribute:
      theAssembly:System.Reflection.Assembly -> attributeName:string -> bool
    val getAttributeNamed:
      theType:System.Type -> attributeName:string -> obj option
    val getInstanceProperty<'treturn>:
      theType:System.Type ->
        propertyName:string -> System.Reflection.PropertyInfo option
    val getInstanceMethod<'treturn>:
      theType:System.Type ->
        parameterTypes:System.Type array ->
          methodName:string -> System.Reflection.MethodInfo option
    val stripTieWrapper: e:System.Exception -> exn

  [<RequireQualifiedAccessAttribute>]
  type ErrorReportType =
    | Warning
    | Error
  type ResolvingErrorReport =
    delegate of ErrorReportType * int * string -> unit
  type IResolveDependenciesResult =
    interface
      abstract member Resolutions: seq<string>
      abstract member Roots: seq<string>
      abstract member SourceFiles: seq<string>
      abstract member StdError: string []
      abstract member StdOut: string []
      abstract member Success: bool
  
  [<AllowNullLiteralAttribute>]
  type IDependencyManagerProvider =
    interface
      abstract member
        ResolveDependencies: scriptDir:string * mainScriptName:string *
                              scriptName:string * scriptExt:string *
                              packageManagerTextLines:seq<string * string> *
                              tfm:string * rid:string ->
                                IResolveDependenciesResult
      abstract member HelpMessages: string []
      abstract member Key: string
      abstract member Name: string
  
  type ReflectionDependencyManagerProvider =

      interface IDependencyManagerProvider
      new: theType:System.Type * nameProperty:System.Reflection.PropertyInfo *
            keyProperty:System.Reflection.PropertyInfo *
            helpMessagesProperty:System.Reflection.PropertyInfo option *
            resolveDeps:System.Reflection.MethodInfo option *
            resolveDepsEx:System.Reflection.MethodInfo option *
            outputDir:string option -> ReflectionDependencyManagerProvider
      static member
        InstanceMaker: theType:System.Type * outputDir:string option ->
                          (unit -> IDependencyManagerProvider) option
      static member
        MakeResultFromFields: success:bool * stdOut:string array *
                               stdError:string array * resolutions:seq<string> *
                               sourceFiles:seq<string> * roots:seq<string> ->
                                 IResolveDependenciesResult
      static member
        MakeResultFromObject: result:obj -> IResolveDependenciesResult
  
  type DependencyProvider =

      interface System.IDisposable
      new: unit -> DependencyProvider
      new: nativeProbingRoots:NativeResolutionProbe -> DependencyProvider
      new: assemblyProbingPaths:AssemblyResolutionProbe *
            nativeProbingRoots:NativeResolutionProbe -> DependencyProvider
      member
        CreatePackageManagerUnknownError: seq<string> * string * string *
                                           ResolvingErrorReport -> int * string
      member
        GetRegisteredDependencyManagerHelpText: seq<string> * string *
                                                 ResolvingErrorReport ->
                                                   string []
      member
        Resolve: packageManager:IDependencyManagerProvider * scriptExt:string *
                  packageManagerTextLines:seq<string * string> *
                  reportError:ResolvingErrorReport * executionTfm:string *
                  executionRid:string * implicitIncludeDir:string *
                  mainScriptName:string * fileName:string ->
                    IResolveDependenciesResult
      member
        TryFindDependencyManagerByKey: compilerTools:seq<string> *
                                        outputDir:string *
                                        reportError:ResolvingErrorReport *
                                        key:string -> IDependencyManagerProvider
      member
        TryFindDependencyManagerInPath: compilerTools:seq<string> *
                                         outputDir:string *
                                         reportError:ResolvingErrorReport *
                                         path:string ->
                                           string * IDependencyManagerProvider
  

namespace FSharp.Compiler
  module internal CompilerConfig =
    val ( ++ ): x:'a list -> s:'a -> 'a list
    val FSharpSigFileSuffixes: string list
    val mlCompatSuffixes: string list
    val FSharpImplFileSuffixes: string list
    val FSharpScriptFileSuffixes: string list
    val doNotRequireNamespaceOrModuleSuffixes: string list
    val FSharpLightSyntaxFileSuffixes: string list
    exception FileNameNotResolved of string * string * Range.range
    exception LoadedSourceNotFoundIgnoring of string * Range.range
    val TryResolveFileUsingPaths:
      paths:string list * m:Range.range * name:string -> string option
    val ResolveFileUsingPaths:
      paths:string list * m:Range.range * name:string -> string
    val GetWarningNumber: m:Range.range * warningNumber:string -> int32 option
    val ComputeMakePathAbsolute:
      implicitIncludeDir:string -> path:string -> string
    [<RequireQualifiedAccessAttribute>]
    type CompilerTarget =
      | WinExe
      | ConsoleExe
      | Dll
      | Module
      with
        member IsExe: bool
    
    [<RequireQualifiedAccessAttribute>]
    type ResolveAssemblyReferenceMode =
      | Speculative
      | ReportErrors
    [<RequireQualifiedAccessAttribute>]
    type CopyFSharpCoreFlag =
      | Yes
      | No
    type VersionFlag =
      | VersionString of string
      | VersionFile of string
      | VersionNone
      with
        member
          GetVersionInfo: implicitIncludeDir:string ->
                             AbstractIL.IL.ILVersionInfo
        member GetVersionString: implicitIncludeDir:string -> string
    
    type IRawFSharpAssemblyData =
      interface
        abstract member
          GetAutoOpenAttributes: AbstractIL.IL.ILGlobals -> string list
        abstract member
          GetInternalsVisibleToAttributes: AbstractIL.IL.ILGlobals ->
                                              string list
        abstract member
          GetRawFSharpOptimizationData: Range.range * ilShortAssemName:string *
                                         fileName:string ->
                                           (string *
                                            (unit ->
                                               AbstractIL.Internal.ReadOnlyByteMemory)) list
        abstract member
          GetRawFSharpSignatureData: Range.range * ilShortAssemName:string *
                                      fileName:string ->
                                        (string *
                                         (unit ->
                                            AbstractIL.Internal.ReadOnlyByteMemory)) list
        abstract member
          GetRawTypeForwarders: unit ->
                                   AbstractIL.IL.ILExportedTypesAndForwarders
        abstract member
          HasMatchingFSharpSignatureDataAttribute: AbstractIL.IL.ILGlobals ->
                                                      bool
        abstract member
          TryGetILModuleDef: unit -> AbstractIL.IL.ILModuleDef option
        abstract member HasAnyFSharpSignatureDataAttribute: bool
        abstract member ILAssemblyRefs: AbstractIL.IL.ILAssemblyRef list
        abstract member ILScopeRef: AbstractIL.IL.ILScopeRef
        abstract member ShortAssemblyName: string
    
    type TimeStampCache =
  
        new: defaultTimeStamp:System.DateTime -> TimeStampCache
        member GetFileTimeStamp: string -> System.DateTime
        member
          GetProjectReferenceTimeStamp: IProjectReference *
                                         AbstractIL.Internal.Library.CompilationThreadToken ->
                                           System.DateTime
    
    and IProjectReference =
      interface
        abstract member
          EvaluateRawContents: AbstractIL.Internal.Library.CompilationThreadToken ->
                                  AbstractIL.Internal.Library.Cancellable<IRawFSharpAssemblyData option>
        abstract member
          TryGetLogicalTimeStamp: TimeStampCache *
                                   AbstractIL.Internal.Library.CompilationThreadToken ->
                                     System.DateTime option
        abstract member FileName: string
    
    type AssemblyReference =
      | AssemblyReference of Range.range * string * IProjectReference option
      with
        member SimpleAssemblyNameIs: string -> bool
        override ToString: unit -> string
        member ProjectReference: IProjectReference option
        member Range: Range.range
        member Text: string
    
    type UnresolvedAssemblyReference =
      | UnresolvedAssemblyReference of string * AssemblyReference list
    type ResolvedExtensionReference =
      | ResolvedExtensionReference of
        string * AssemblyReference list *
        Tainted<CompilerServices.ITypeProvider> list
    type ImportedAssembly =
      { ILScopeRef: AbstractIL.IL.ILScopeRef
        FSharpViewOfMetadata: TypedTree.CcuThunk
        AssemblyAutoOpenAttributes: string list
        AssemblyInternalsVisibleToAttributes: string list
        IsProviderGenerated: bool
        mutable TypeProviders: Tainted<CompilerServices.ITypeProvider> list
        FSharpOptimizationData: Lazy<Option<Optimizer.LazyModuleInfo>> }
    type AvailableImportedAssembly =
      | ResolvedImportedAssembly of ImportedAssembly
      | UnresolvedImportedAssembly of string
    type CcuLoadFailureAction =
      | RaiseError
      | ReturnNone
    type Directive =
      | Resolution
      | Include
    type LStatus =
      | Unprocessed
      | Processed
    type PackageManagerLine =
      { Directive: Directive
        LineStatus: LStatus
        Line: string
        Range: Range.range }
      with
        static member
          AddLineWithKey: string ->
                             Directive ->
                               string ->
                                 Range.range ->
                                   Map<string,PackageManagerLine list> ->
                                     Map<string,PackageManagerLine list>
        static member
          RemoveUnprocessedLines: string ->
                                     Map<string,PackageManagerLine list> ->
                                       Map<string,PackageManagerLine list>
        static member
          SetLinesAsProcessed: string ->
                                  Map<string,PackageManagerLine list> ->
                                    Map<string,PackageManagerLine list>
        static member StripDependencyManagerKey: string -> string -> string
    
    [<NoEquality; NoComparison>]
    type TcConfigBuilder =
      { mutable primaryAssembly: AbstractIL.IL.PrimaryAssembly
        mutable noFeedback: bool
        mutable stackReserveSize: int32 option
        mutable implicitIncludeDir: string
        mutable openDebugInformationForLaterStaticLinking: bool
        defaultFSharpBinariesDir: string
        mutable compilingFslib: bool
        mutable useIncrementalBuilder: bool
        mutable includes: string list
        mutable implicitOpens: string list
        mutable useFsiAuxLib: bool
        mutable framework: bool
        mutable resolutionEnvironment: ReferenceResolver.ResolutionEnvironment
        mutable implicitlyResolveAssemblies: bool
        mutable light: bool option
        mutable conditionalCompilationDefines: string list
        mutable loadedSources: (Range.range * string * string) list
        mutable compilerToolPaths: string list
        mutable referencedDLLs: AssemblyReference list
        mutable packageManagerLines: Map<string,PackageManagerLine list>
        mutable projectReferences: IProjectReference list
        mutable knownUnresolvedReferences: UnresolvedAssemblyReference list
        reduceMemoryUsage: AbstractIL.ILBinaryReader.ReduceMemoryFlag
        mutable subsystemVersion: int * int
        mutable useHighEntropyVA: bool
        mutable inputCodePage: int option
        mutable embedResources: string list
        mutable errorSeverityOptions: ErrorLogger.FSharpErrorSeverityOptions
        mutable mlCompatibility: bool
        mutable checkOverflow: bool
        mutable showReferenceResolutions: bool
        mutable outputDir: string option
        mutable outputFile: string option
        mutable platform: AbstractIL.IL.ILPlatform option
        mutable prefer32Bit: bool
        mutable useSimpleResolution: bool
        mutable target: CompilerTarget
        mutable debuginfo: bool
        mutable testFlagEmitFeeFeeAs100001: bool
        mutable dumpDebugInfo: bool
        mutable debugSymbolFile: string option
        mutable typeCheckOnly: bool
        mutable parseOnly: bool
        mutable importAllReferencesOnly: bool
        mutable simulateException: string option
        mutable printAst: bool
        mutable tokenizeOnly: bool
        mutable testInteractionParser: bool
        mutable reportNumDecls: bool
        mutable printSignature: bool
        mutable printSignatureFile: string
        mutable xmlDocOutputFile: string option
        mutable stats: bool
        mutable generateFilterBlocks: bool
        mutable signer: string option
        mutable container: string option
        mutable delaysign: bool
        mutable publicsign: bool
        mutable version: VersionFlag
        mutable metadataVersion: string option
        mutable standalone: bool
        mutable extraStaticLinkRoots: string list
        mutable noSignatureData: bool
        mutable onlyEssentialOptimizationData: bool
        mutable useOptimizationDataFile: bool
        mutable jitTracking: bool
        mutable portablePDB: bool
        mutable embeddedPDB: bool
        mutable embedAllSource: bool
        mutable embedSourceList: string list
        mutable sourceLink: string
        mutable ignoreSymbolStoreSequencePoints: bool
        mutable internConstantStrings: bool
        mutable extraOptimizationIterations: int
        mutable win32res: string
        mutable win32manifest: string
        mutable includewin32manifest: bool
        mutable linkResources: string list
        mutable legacyReferenceResolver: ReferenceResolver.Resolver
        mutable showFullPaths: bool
        mutable errorStyle: ErrorLogger.ErrorStyle
        mutable utf8output: bool
        mutable flatErrors: bool
        mutable maxErrors: int
        mutable abortOnError: bool
        mutable baseAddress: int32 option
        mutable checksumAlgorithm: AbstractIL.ILPdbWriter.HashAlgorithm
        mutable showOptimizationData: bool
        mutable showTerms: bool
        mutable writeTermsToFiles: bool
        mutable doDetuple: bool
        mutable doTLR: bool
        mutable doFinalSimplify: bool
        mutable optsOn: bool
        mutable optSettings: Optimizer.OptimizationSettings
        mutable emitTailcalls: bool
        mutable deterministic: bool
        mutable preferredUiLang: string option
        mutable lcid: int option
        mutable productNameForBannerText: string
        mutable showBanner: bool
        mutable showTimes: bool
        mutable showLoadedAssemblies: bool
        mutable continueAfterParseFailure: bool
        mutable showExtensionTypeMessages: bool
        mutable pause: bool
        mutable alwaysCallVirt: bool
        mutable noDebugData: bool
        isInteractive: bool
        isInvalidationSupported: bool
        mutable emitDebugInfoInQuotations: bool
        mutable exename: string option
        mutable copyFSharpCore: CopyFSharpCoreFlag
        mutable shadowCopyReferences: bool
        mutable useSdkRefs: bool
        mutable tryGetMetadataSnapshot:
          AbstractIL.ILBinaryReader.ILReaderTryGetMetadataSnapshot
        mutable internalTestSpanStackReferring: bool
        mutable noConditionalErasure: bool
        mutable pathMap: Internal.Utilities.PathMap
        mutable langVersion: Features.LanguageVersion }
      with
        static member
          CreateNew: legacyReferenceResolver:ReferenceResolver.Resolver *
                      defaultFSharpBinariesDir:string *
                      reduceMemoryUsage:AbstractIL.ILBinaryReader.ReduceMemoryFlag *
                      implicitIncludeDir:string * isInteractive:bool *
                      isInvalidationSupported:bool *
                      defaultCopyFSharpCore:CopyFSharpCoreFlag *
                      tryGetMetadataSnapshot:AbstractIL.ILBinaryReader.ILReaderTryGetMetadataSnapshot ->
                        TcConfigBuilder
        static member
          SplitCommandLineResourceInfo: string ->
                                           string * string *
                                           AbstractIL.IL.ILResourceAccess
        member AddCompilerToolsByPath: string -> unit
        member
          AddDependencyManagerText: packageManager:Microsoft.DotNet.DependencyManager.IDependencyManagerProvider *
                                     lt:Directive * m:Range.range * path:string ->
                                       unit
        member AddEmbeddedResource: string -> unit
        member AddEmbeddedSourceFile: string -> unit
        member AddIncludePath: Range.range * string * string -> unit
        member
          AddLoadedSource: m:Range.range * originalPath:string *
                            pathLoadedFrom:string -> unit
        member AddPathMapping: oldPrefix:string * newPrefix:string -> unit
        member
          AddReferenceDirective: dependencyProvider:Microsoft.DotNet.DependencyManager.DependencyProvider *
                                  m:Range.range * path:string *
                                  directive:Directive -> unit
        member AddReferencedAssemblyByPath: Range.range * string -> unit
        member DecideNames: string list -> string * string option * string
        member GetNativeProbingRoots: unit -> seq<string>
        member RemoveReferencedAssemblyByPath: Range.range * string -> unit
        member
          ResolveSourceFile: m:Range.range * nm:string * pathLoadedFrom:string ->
                                string
        member TurnWarningOff: Range.range * string -> unit
        member TurnWarningOn: Range.range * string -> unit
        static member Initial: TcConfigBuilder
    
    [<SealedAttribute>]
    type TcConfig =
  
        private new: data:TcConfigBuilder * validate:bool -> TcConfig
        static member Create: TcConfigBuilder * validate:bool -> TcConfig
        member CloneToBuilder: unit -> TcConfigBuilder
        member
          ComputeCanContainEntryPoint: sourceFiles:string list ->
                                          bool list * bool
        member ComputeLightSyntaxInitialStatus: string -> bool
        member CoreLibraryDllReference: unit -> AssemblyReference
        member GetAvailableLoadedSources: unit -> (Range.range * string) list
        member GetNativeProbingRoots: unit -> seq<string>
        member GetSearchPathsForLibraryFiles: unit -> string list
        member GetTargetFrameworkDirectories: unit -> string list
        member IsSystemAssembly: string -> bool
        member MakePathAbsolute: string -> string
        member PrimaryAssemblyDllReference: unit -> AssemblyReference
        member
          ResolveSourceFile: Range.range * filename:string *
                              pathLoadedFrom:string -> string
        member GenerateOptimizationData: bool
        member GenerateSignatureData: bool
        member alwaysCallVirt: bool
        member baseAddress: int32 option
        member checkOverflow: bool
        member checksumAlgorithm: AbstractIL.ILPdbWriter.HashAlgorithm
        member clrRoot: string option
        member compilerToolPaths: string list
        member compilingFslib: bool
        member conditionalCompilationDefines: string list
        member container: string option
        member continueAfterParseFailure: bool
        member copyFSharpCore: CopyFSharpCoreFlag
        member debugSymbolFile: string option
        member debuginfo: bool
        member delaysign: bool
        member deterministic: bool
        member doDetuple: bool
        member doFinalSimplify: bool
        member doTLR: bool
        member dumpDebugInfo: bool
        member embedAllSource: bool
        member embedResources: string list
        member embedSourceList: string list
        member embeddedPDB: bool
        member emitDebugInfoInQuotations: bool
        member emitTailcalls: bool
        member errorSeverityOptions: ErrorLogger.FSharpErrorSeverityOptions
        member errorStyle: ErrorLogger.ErrorStyle
        member extraOptimizationIterations: int
        member extraStaticLinkRoots: string list
        member flatErrors: bool
        member framework: bool
        member fsharpBinariesDir: string
        member generateFilterBlocks: bool
        member ignoreSymbolStoreSequencePoints: bool
        member implicitIncludeDir: string
        member implicitOpens: string list
        member implicitlyResolveAssemblies: bool
        member importAllReferencesOnly: bool
        member includes: string list
        member includewin32manifest: bool
        member inputCodePage: int option
        member internConstantStrings: bool
        member internalTestSpanStackReferring: bool
        member isInteractive: bool
        member isInvalidationSupported: bool
        member jitTracking: bool
        member knownUnresolvedReferences: UnresolvedAssemblyReference list
        member langVersion: Features.LanguageVersion
        member lcid: int option
        member legacyReferenceResolver: ReferenceResolver.Resolver
        member light: bool option
        member linkResources: string list
        member loadedSources: (Range.range * string * string) list
        member maxErrors: int
        member metadataVersion: string option
        member mlCompatibility: bool
        member noConditionalErasure: bool
        member noDebugData: bool
        member noFeedback: bool
        member noSignatureData: bool
        member onlyEssentialOptimizationData: bool
        member openDebugInformationForLaterStaticLinking: bool
        member optSettings: Optimizer.OptimizationSettings
        member optsOn: bool
        member outputDir: string option
        member outputFile: string option
        member packageManagerLines: Map<string,PackageManagerLine list>
        member parseOnly: bool
        member pathMap: Internal.Utilities.PathMap
        member pause: bool
        member platform: AbstractIL.IL.ILPlatform option
        member portablePDB: bool
        member prefer32Bit: bool
        member preferredUiLang: string option
        member primaryAssembly: AbstractIL.IL.PrimaryAssembly
        member printAst: bool
        member printSignature: bool
        member printSignatureFile: string
        member productNameForBannerText: string
        member publicsign: bool
        member reduceMemoryUsage: AbstractIL.ILBinaryReader.ReduceMemoryFlag
        member referencedDLLs: AssemblyReference list
        member reportNumDecls: bool
        member resolutionEnvironment: ReferenceResolver.ResolutionEnvironment
        member shadowCopyReferences: bool
        member showBanner: bool
        member showExtensionTypeMessages: bool
        member showFullPaths: bool
        member showLoadedAssemblies: bool
        member showOptimizationData: bool
        member showReferenceResolutions: bool
        member showTerms: bool
        member showTimes: bool
        member signer: string option
        member simulateException: string option
        member sourceLink: string
        member stackReserveSize: int32 option
        member standalone: bool
        member stats: bool
        member subsystemVersion: int * int
        member target: CompilerTarget
        member targetFrameworkVersion: string
        member testFlagEmitFeeFeeAs100001: bool
        member testInteractionParser: bool
        member tokenizeOnly: bool
        member
          tryGetMetadataSnapshot: AbstractIL.ILBinaryReader.ILReaderTryGetMetadataSnapshot
        member typeCheckOnly: bool
        member useFsiAuxLib: bool
        member useHighEntropyVA: bool
        member useIncrementalBuilder: bool
        member useOptimizationDataFile: bool
        member useSdkRefs: bool
        member useSimpleResolution: bool
        member utf8output: bool
        member version: VersionFlag
        member win32manifest: string
        member win32res: string
        member writeTermsToFiles: bool
        member xmlDocOutputFile: string option
    
    [<SealedAttribute>]
    type TcConfigProvider =
      | TcConfigProvider of
        (AbstractIL.Internal.Library.CompilationThreadToken -> TcConfig)
      with
        static member
          BasedOnMutableBuilder: TcConfigBuilder -> TcConfigProvider
        static member Constant: TcConfig -> TcConfigProvider
        member
          Get: AbstractIL.Internal.Library.CompilationThreadToken -> TcConfig
    
    val GetFSharpCoreLibraryName: unit -> string


namespace FSharp.Compiler
  module internal CompilerImports =
    val ( ++ ): x:'a list -> s:'a -> 'a list
    val IsSignatureDataResource: AbstractIL.IL.ILResource -> bool
    val IsOptimizationDataResource: AbstractIL.IL.ILResource -> bool
    val GetSignatureDataResourceName: AbstractIL.IL.ILResource -> string
    val GetOptimizationDataResourceName: r:AbstractIL.IL.ILResource -> string
    val IsReflectedDefinitionsResource: AbstractIL.IL.ILResource -> bool
    val MakeILResource:
      rName:string -> bytes:byte [] -> AbstractIL.IL.ILResource
    val PickleToResource:
      inMem:bool ->
        file:string ->
          g:TcGlobals.TcGlobals ->
            scope:TypedTree.CcuThunk ->
              rName:string ->
                p:TypedTreePickle.pickler<'a> ->
                  x:'a -> AbstractIL.IL.ILResource
    val GetSignatureData:
      file:string * ilScopeRef:AbstractIL.IL.ILScopeRef *
      ilModule:AbstractIL.IL.ILModuleDef option *
      byteReader:(unit -> AbstractIL.Internal.ReadOnlyByteMemory) ->
        TypedTreePickle.PickledDataWithReferences<TypedTree.PickledCcuInfo>
    val WriteSignatureData:
      CompilerConfig.TcConfig * TcGlobals.TcGlobals * TypedTreeOps.Remap *
      TypedTree.CcuThunk * filename:string * inMem:bool ->
        AbstractIL.IL.ILResource
    val GetOptimizationData:
      file:string * ilScopeRef:AbstractIL.IL.ILScopeRef *
      ilModule:AbstractIL.IL.ILModuleDef option *
      byteReader:(unit -> AbstractIL.Internal.ReadOnlyByteMemory) ->
        TypedTreePickle.PickledDataWithReferences<Optimizer.CcuOptimizationInfo>
    val WriteOptimizationData:
      TcGlobals.TcGlobals * filename:string * inMem:bool * TypedTree.CcuThunk *
      Optimizer.CcuOptimizationInfo -> AbstractIL.IL.ILResource
    exception AssemblyNotResolved of string * Range.range
    exception MSBuildReferenceResolutionWarning of string * string * Range.range
    exception MSBuildReferenceResolutionError of string * string * Range.range
    val OpenILBinary:
      filename:string *
      reduceMemoryUsage:AbstractIL.ILBinaryReader.ReduceMemoryFlag *
      pdbDirPath:string option * shadowCopyReferences:bool *
      tryGetMetadataSnapshot:AbstractIL.ILBinaryReader.ILReaderTryGetMetadataSnapshot ->
        AbstractIL.ILBinaryReader.ILModuleReader
    [<RequireQualifiedAccessAttribute>]
    type ResolveAssemblyReferenceMode =
      | Speculative
      | ReportErrors
    type ResolvedExtensionReference =
      | ResolvedExtensionReference of
        string * CompilerConfig.AssemblyReference list *
        Tainted<CompilerServices.ITypeProvider> list
    [<System.Diagnostics.DebuggerDisplay ("AssemblyResolution({resolvedPath})")>]
    type AssemblyResolution =
      { originalReference: CompilerConfig.AssemblyReference
        resolvedPath: string
        prepareToolTip: unit -> string
        sysdir: bool
        mutable ilAssemblyRef: AbstractIL.IL.ILAssemblyRef option }
      with
        member
          GetILAssemblyRef: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                             reduceMemoryUsage:AbstractIL.ILBinaryReader.ReduceMemoryFlag *
                             tryGetMetadataSnapshot:AbstractIL.ILBinaryReader.ILReaderTryGetMetadataSnapshot ->
                               AbstractIL.Internal.Library.Cancellable<AbstractIL.IL.ILAssemblyRef>
        override ToString: unit -> string
        member ProjectReference: CompilerConfig.IProjectReference option
    
    [<RequireQualifiedAccessAttribute>]
    type ImportedBinary =
      { FileName: string
        RawMetadata: CompilerConfig.IRawFSharpAssemblyData
        ProviderGeneratedAssembly: System.Reflection.Assembly option
        IsProviderGenerated: bool
        ProviderGeneratedStaticLinkMap:
          ExtensionTyping.ProvidedAssemblyStaticLinkingMap option
        ILAssemblyRefs: AbstractIL.IL.ILAssemblyRef list
        ILScopeRef: AbstractIL.IL.ILScopeRef }
    [<RequireQualifiedAccessAttribute>]
    type ImportedAssembly =
      { ILScopeRef: AbstractIL.IL.ILScopeRef
        FSharpViewOfMetadata: TypedTree.CcuThunk
        AssemblyAutoOpenAttributes: string list
        AssemblyInternalsVisibleToAttributes: string list
        IsProviderGenerated: bool
        mutable TypeProviders: Tainted<CompilerServices.ITypeProvider> list
        FSharpOptimizationData: Lazy<Option<Optimizer.LazyModuleInfo>> }
    type AvailableImportedAssembly =
      | ResolvedImportedAssembly of ImportedAssembly
      | UnresolvedImportedAssembly of string
    type CcuLoadFailureAction =
      | RaiseError
      | ReturnNone
    type TcConfig with
      member
        TryResolveLibWithDirectories: r:CompilerConfig.AssemblyReference ->
                                         AssemblyResolution option
    type TcConfig with
      member
        ResolveLibWithDirectories: ccuLoadFailureAction:CcuLoadFailureAction *
                                    r:CompilerConfig.AssemblyReference ->
                                      AssemblyResolution option
    type TcConfig with
      member
        MsBuildResolve: references:(string * string) [] *
                         mode:ResolveAssemblyReferenceMode *
                         errorAndWarningRange:Range.range * showMessages:bool ->
                           ReferenceResolver.ResolvedFile []
    type TcConfig with
      static member
        TryResolveLibsUsingMSBuildRules: tcConfig:CompilerConfig.TcConfig *
                                          originalReferences:CompilerConfig.AssemblyReference list *
                                          errorAndWarningRange:Range.range *
                                          mode:ResolveAssemblyReferenceMode ->
                                            AssemblyResolution list *
                                            CompilerConfig.UnresolvedAssemblyReference list
    [<SealedAttribute>]
    type TcAssemblyResolutions =
  
        new: tcConfig:CompilerConfig.TcConfig * results:AssemblyResolution list *
              unresolved:CompilerConfig.UnresolvedAssemblyReference list ->
                TcAssemblyResolutions
        static member
          BuildFromPriorResolutions: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                                      tcConfig:CompilerConfig.TcConfig *
                                      AssemblyResolution list *
                                      CompilerConfig.UnresolvedAssemblyReference list ->
                                        TcAssemblyResolutions
        static member
          GetAllDllReferences: tcConfig:CompilerConfig.TcConfig ->
                                  CompilerConfig.AssemblyReference list
        static member
          GetAssemblyResolutionInformation: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                                             tcConfig:CompilerConfig.TcConfig ->
                                               AssemblyResolution list *
                                               CompilerConfig.UnresolvedAssemblyReference list
        static member
          ResolveAssemblyReferences: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                                      tcConfig:CompilerConfig.TcConfig *
                                      assemblyList:CompilerConfig.AssemblyReference list *
                                      knownUnresolved:CompilerConfig.UnresolvedAssemblyReference list ->
                                        TcAssemblyResolutions
        static member
          SplitNonFoundationalResolutions: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                                            tcConfig:CompilerConfig.TcConfig ->
                                              AssemblyResolution list *
                                              AssemblyResolution list *
                                              CompilerConfig.UnresolvedAssemblyReference list
        member
          AddResolutionResults: newResults:AssemblyResolution list ->
                                   TcAssemblyResolutions
        member
          AddUnresolvedReferences: newUnresolved:CompilerConfig.UnresolvedAssemblyReference list ->
                                      TcAssemblyResolutions
        member GetAssemblyResolutions: unit -> AssemblyResolution list
        member
          GetUnresolvedReferences: unit ->
                                      CompilerConfig.UnresolvedAssemblyReference list
        member
          TryFindByExactILAssemblyRef: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                                        assemblyRef:AbstractIL.IL.ILAssemblyRef ->
                                          AssemblyResolution option
        member
          TryFindByOriginalReference: assemblyReference:CompilerConfig.AssemblyReference ->
                                         AssemblyResolution option
        member
          TryFindByOriginalReferenceText: nm:string ->
                                             AssemblyResolution option
        member TryFindByResolvedPath: nm:string -> AssemblyResolution option
        member
          TryFindBySimpleAssemblyName: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                                        simpleAssemName:string ->
                                          AssemblyResolution option
    
    val GetNameOfILModule: m:AbstractIL.IL.ILModuleDef -> string
    val MakeScopeRefForILModule:
      ilModule:AbstractIL.IL.ILModuleDef -> AbstractIL.IL.ILScopeRef
    val GetCustomAttributesOfILModule:
      ilModule:AbstractIL.IL.ILModuleDef -> AbstractIL.IL.ILAttribute list
    val GetAutoOpenAttributes:
      ilg:AbstractIL.IL.ILGlobals ->
        ilModule:AbstractIL.IL.ILModuleDef -> string list
    val GetInternalsVisibleToAttributes:
      ilg:AbstractIL.IL.ILGlobals ->
        ilModule:AbstractIL.IL.ILModuleDef -> string list
    type RawFSharpAssemblyDataBackedByFileOnDisk =
  
        interface CompilerConfig.IRawFSharpAssemblyData
        new: ilModule:AbstractIL.IL.ILModuleDef *
              ilAssemblyRefs:AbstractIL.IL.ILAssemblyRef list ->
                RawFSharpAssemblyDataBackedByFileOnDisk
    
    [<SealedAttribute>]
    type TcImportsSafeDisposal =
  
        interface System.IDisposable
        new: disposeActions:ResizeArray<(unit -> unit)> *
              disposeTypeProviderActions:ResizeArray<(unit -> unit)> ->
                TcImportsSafeDisposal
        override Finalize: unit -> unit
    
    type TcImportsDllInfoHack =
      { FileName: string }
    and TcImportsWeakHack =
  
        new: tcImports:System.WeakReference<TcImports> -> TcImportsWeakHack
        member SetDllInfos: value:ImportedBinary list -> unit
        member SystemRuntimeContainsType: typeName:string -> bool
        member Base: TcImportsWeakHack option
    
    [<SealedAttribute>]
    and TcImports =
  
        interface System.IDisposable
        new: tcConfigP:CompilerConfig.TcConfigProvider *
              initialResolutions:TcAssemblyResolutions *
              importsBase:TcImports option *
              ilGlobalsOpt:AbstractIL.IL.ILGlobals option *
              dependencyProviderOpt:Microsoft.DotNet.DependencyManager.DependencyProvider option ->
                TcImports
        static member
          BuildFrameworkTcImports: AbstractIL.Internal.Library.CompilationThreadToken *
                                    CompilerConfig.TcConfigProvider *
                                    AssemblyResolution list *
                                    AssemblyResolution list ->
                                      AbstractIL.Internal.Library.Cancellable<TcGlobals.TcGlobals *
                                                                              TcImports>
        static member
          BuildNonFrameworkTcImports: AbstractIL.Internal.Library.CompilationThreadToken *
                                       CompilerConfig.TcConfigProvider *
                                       TcGlobals.TcGlobals * TcImports *
                                       AssemblyResolution list *
                                       CompilerConfig.UnresolvedAssemblyReference list *
                                       Microsoft.DotNet.DependencyManager.DependencyProvider ->
                                         AbstractIL.Internal.Library.Cancellable<TcImports>
        static member
          BuildTcImports: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                           tcConfigP:CompilerConfig.TcConfigProvider *
                           dependencyProvider:Microsoft.DotNet.DependencyManager.DependencyProvider ->
                             AbstractIL.Internal.Library.Cancellable<TcGlobals.TcGlobals *
                                                                     TcImports>
        member AllAssemblyResolutions: unit -> AssemblyResolution list
        member private AttachDisposeAction: action:(unit -> unit) -> unit
        member
          private AttachDisposeTypeProviderAction: action:(unit -> unit) ->
                                                      unit
        member
          FindCcu: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                    m:Range.range * assemblyName:string * lookupOnly:bool ->
                      TypedTree.CcuResolutionResult
        member
          FindCcuFromAssemblyRef: AbstractIL.Internal.Library.CompilationThreadToken *
                                   Range.range * AbstractIL.IL.ILAssemblyRef ->
                                     TypedTree.CcuResolutionResult
        member
          FindCcuInfo: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                        m:Range.range * assemblyName:string * lookupOnly:bool ->
                          AvailableImportedAssembly
        member
          FindDllInfo: AbstractIL.Internal.Library.CompilationThreadToken *
                        Range.range * string -> ImportedBinary
        member GetCcusExcludingBase: unit -> TypedTree.CcuThunk list
        member GetCcusInDeclOrder: unit -> TypedTree.CcuThunk list
        member GetDllInfos: unit -> ImportedBinary list
        member GetImportMap: unit -> Import.ImportMap
        member GetImportedAssemblies: unit -> ImportedAssembly list
        member
          GetProvidedAssemblyInfo: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                                    m:Range.range *
                                    assembly:Tainted<ExtensionTyping.ProvidedAssembly> ->
                                      bool *
                                      ExtensionTyping.ProvidedAssemblyStaticLinkingMap option
        member GetTcGlobals: unit -> TcGlobals.TcGlobals
        member
          ImplicitLoadIfAllowed: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                                  m:Range.range * assemblyName:string *
                                  lookupOnly:bool -> unit
        member
          ImportTypeProviderExtensions: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                                         tcConfig:CompilerConfig.TcConfig *
                                         fileNameOfRuntimeAssembly:string *
                                         ilScopeRefOfRuntimeAssembly:AbstractIL.IL.ILScopeRef *
                                         runtimeAssemblyAttributes:AbstractIL.IL.ILAttribute list *
                                         entityToInjectInto:TypedTree.Entity *
                                         invalidateCcu:Event<string> *
                                         m:Range.range ->
                                           Tainted<CompilerServices.ITypeProvider> list
        member
          private InjectProvidedNamespaceOrTypeIntoEntity: typeProviderEnvironment:ExtensionTyping.ResolutionEnvironment *
                                                            tcConfig:CompilerConfig.TcConfig *
                                                            m:Range.range *
                                                            entity:TypedTree.Entity *
                                                            injectedNamespace:string list *
                                                            remainingNamespace:string list *
                                                            provider:Tainted<CompilerServices.ITypeProvider> *
                                                            st:Tainted<ExtensionTyping.ProvidedType> option ->
                                                              unit
        member IsAlreadyRegistered: nm:string -> bool
        member
          MkLoaderForMultiModuleILAssemblies: ctok:AbstractIL.Internal.Library.CompilationThreadToken ->
                                                 m:Range.range ->
                                                   (AbstractIL.IL.ILScopeRef ->
                                                      AbstractIL.IL.ILModuleDef)
        member
          OpenILBinaryModule: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                               filename:string * m:Range.range ->
                                 AbstractIL.IL.ILModuleDef *
                                 AbstractIL.IL.ILAssemblyRef list
        member
          PrepareToImportReferencedFSharpAssembly: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                                                    m:Range.range *
                                                    filename:string *
                                                    dllinfo:ImportedBinary ->
                                                      (unit ->
                                                         AvailableImportedAssembly list)
        member
          PrepareToImportReferencedILAssembly: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                                                m:Range.range * filename:string *
                                                dllinfo:ImportedBinary ->
                                                  (unit ->
                                                     AvailableImportedAssembly list)
        member
          RecordGeneratedTypeRoot: root:ExtensionTyping.ProviderGeneratedType ->
                                      unit
        member
          RegisterAndImportReferencedAssemblies: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                                                  nms:AssemblyResolution list ->
                                                    AbstractIL.Internal.Library.Cancellable<ImportedBinary list *
                                                                                            AvailableImportedAssembly list>
        member
          RegisterAndPrepareToImportReferencedDll: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                                                    r:AssemblyResolution ->
                                                      AbstractIL.Internal.Library.Cancellable<ImportedBinary *
                                                                                              (unit ->
                                                                                                 AvailableImportedAssembly list)>
        member RegisterCcu: ccuInfo:ImportedAssembly -> unit
        member RegisterDll: dllInfo:ImportedBinary -> unit
        member
          ReportUnresolvedAssemblyReferences: CompilerConfig.UnresolvedAssemblyReference list ->
                                                 unit
        member
          ResolveAssemblyReference: AbstractIL.Internal.Library.CompilationThreadToken *
                                     CompilerConfig.AssemblyReference *
                                     ResolveAssemblyReferenceMode ->
                                       AssemblyResolution list
        member private SetILGlobals: ilg:AbstractIL.IL.ILGlobals -> unit
        member private SetTcGlobals: g:TcGlobals.TcGlobals -> unit
        member SystemRuntimeContainsType: string -> bool
        override ToString: unit -> string
        member
          TryFindDllInfo: AbstractIL.Internal.Library.CompilationThreadToken *
                           Range.range * string * lookupOnly:bool ->
                             ImportedBinary option
        member
          TryFindExistingFullyQualifiedPathByExactAssemblyRef: AbstractIL.Internal.Library.CompilationThreadToken *
                                                                AbstractIL.IL.ILAssemblyRef ->
                                                                  string option
        member
          TryFindExistingFullyQualifiedPathBySimpleAssemblyName: AbstractIL.Internal.Library.CompilationThreadToken *
                                                                  string ->
                                                                    string option
        member
          TryFindProviderGeneratedAssemblyByName: AbstractIL.Internal.Library.CompilationThreadToken *
                                                   assemblyName:string ->
                                                     System.Reflection.Assembly option
        member
          TryResolveAssemblyReference: AbstractIL.Internal.Library.CompilationThreadToken *
                                        CompilerConfig.AssemblyReference *
                                        ResolveAssemblyReferenceMode ->
                                          ErrorLogger.OperationResult<AssemblyResolution list>
        member Base: TcImports option
        member CcuTable: AbstractIL.Internal.Library.NameMap<ImportedAssembly>
        member
          DependencyProvider: Microsoft.DotNet.DependencyManager.DependencyProvider
        member DllTable: AbstractIL.Internal.Library.NameMap<ImportedBinary>
        member
          ProviderGeneratedTypeRoots: ExtensionTyping.ProviderGeneratedType list
        member Weak: TcImportsWeakHack
    
    val RequireDLL:
      ctok:AbstractIL.Internal.Library.CompilationThreadToken *
      tcImports:TcImports * tcEnv:CheckExpressions.TcEnv *
      thisAssemblyName:string * referenceRange:Range.range * file:string ->
        CheckExpressions.TcEnv * (ImportedBinary list * ImportedAssembly list)
    val DefaultReferencesForScriptsAndOutOfProjectSources: bool -> string list


namespace FSharp.Compiler
  module internal CompilerDiagnostics =
    module CompilerService =
      val showAssertForUnexpectedException: bool ref
  
    exception HashIncludeNotAllowedInNonScript of Range.range
    exception HashReferenceNotAllowedInNonScript of Range.range
    exception HashLoadedSourceHasIssues of exn list * exn list * Range.range
    exception HashLoadedScriptConsideredSource of Range.range
    exception HashDirectiveNotAllowedInNonScript of Range.range
    exception DeprecatedCommandLineOptionFull of string * Range.range
    exception DeprecatedCommandLineOptionForHtmlDoc of string * Range.range
    exception DeprecatedCommandLineOptionSuggestAlternative of
                                                              string * string *
                                                              Range.range
    exception DeprecatedCommandLineOptionNoDescription of string * Range.range
    exception InternalCommandLineOption of string * Range.range
    val GetRangeOfDiagnostic:
      ErrorLogger.PhasedDiagnostic -> Range.range option
    val GetDiagnosticNumber: ErrorLogger.PhasedDiagnostic -> int
    val GetWarningLevel: err:ErrorLogger.PhasedDiagnostic -> int
    val warningOn:
      err:ErrorLogger.PhasedDiagnostic ->
        level:int -> specificWarnOn:int list -> bool
    val SplitRelatedDiagnostics:
      ErrorLogger.PhasedDiagnostic ->
        ErrorLogger.PhasedDiagnostic * ErrorLogger.PhasedDiagnostic list
    val DeclareMessage:
      (string * Printf.StringFormat<'a> -> DiagnosticMessage.ResourceString<'a>)
    val SeeAlsoE: unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val ConstraintSolverTupleDiffLengthsE:
      unit -> DiagnosticMessage.ResourceString<(int -> int -> string)>
    val ConstraintSolverInfiniteTypesE:
      unit -> DiagnosticMessage.ResourceString<(string -> string -> string)>
    val ConstraintSolverMissingConstraintE:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val ConstraintSolverTypesNotInEqualityRelation1E:
      unit -> DiagnosticMessage.ResourceString<(string -> string -> string)>
    val ConstraintSolverTypesNotInEqualityRelation2E:
      unit -> DiagnosticMessage.ResourceString<(string -> string -> string)>
    val ConstraintSolverTypesNotInSubsumptionRelationE:
      unit ->
        DiagnosticMessage.ResourceString<(string -> string -> string -> string)>
    val ErrorFromAddingTypeEquation1E:
      unit ->
        DiagnosticMessage.ResourceString<(string -> string -> string -> string)>
    val ErrorFromAddingTypeEquation2E:
      unit ->
        DiagnosticMessage.ResourceString<(string -> string -> string -> string)>
    val ErrorFromApplyingDefault1E:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val ErrorFromApplyingDefault2E:
      unit -> DiagnosticMessage.ResourceString<string>
    val ErrorsFromAddingSubsumptionConstraintE:
      unit ->
        DiagnosticMessage.ResourceString<(string -> string -> string -> string)>
    val UpperCaseIdentifierInPatternE:
      unit -> DiagnosticMessage.ResourceString<string>
    val NotUpperCaseConstructorE:
      unit -> DiagnosticMessage.ResourceString<string>
    val FunctionExpectedE: unit -> DiagnosticMessage.ResourceString<string>
    val BakedInMemberConstraintNameE:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val BadEventTransformationE:
      unit -> DiagnosticMessage.ResourceString<string>
    val ParameterlessStructCtorE:
      unit -> DiagnosticMessage.ResourceString<string>
    val InterfaceNotRevealedE:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val TyconBadArgsE:
      unit -> DiagnosticMessage.ResourceString<(string -> int -> int -> string)>
    val IndeterminateTypeE: unit -> DiagnosticMessage.ResourceString<string>
    val NameClash1E:
      unit -> DiagnosticMessage.ResourceString<(string -> string -> string)>
    val NameClash2E:
      unit ->
        DiagnosticMessage.ResourceString<(string -> string -> string ->
                                            string -> string -> string)>
    val Duplicate1E:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val Duplicate2E:
      unit -> DiagnosticMessage.ResourceString<(string -> string -> string)>
    val UndefinedName2E: unit -> DiagnosticMessage.ResourceString<string>
    val FieldNotMutableE: unit -> DiagnosticMessage.ResourceString<string>
    val FieldsFromDifferentTypesE:
      unit -> DiagnosticMessage.ResourceString<(string -> string -> string)>
    val VarBoundTwiceE:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val RecursionE:
      unit ->
        DiagnosticMessage.ResourceString<(string -> string -> string ->
                                            string -> string)>
    val InvalidRuntimeCoercionE:
      unit ->
        DiagnosticMessage.ResourceString<(string -> string -> string -> string)>
    val IndeterminateRuntimeCoercionE:
      unit -> DiagnosticMessage.ResourceString<(string -> string -> string)>
    val IndeterminateStaticCoercionE:
      unit -> DiagnosticMessage.ResourceString<(string -> string -> string)>
    val StaticCoercionShouldUseBoxE:
      unit -> DiagnosticMessage.ResourceString<(string -> string -> string)>
    val TypeIsImplicitlyAbstractE:
      unit -> DiagnosticMessage.ResourceString<string>
    val NonRigidTypar1E:
      unit -> DiagnosticMessage.ResourceString<(string -> string -> string)>
    val NonRigidTypar2E:
      unit -> DiagnosticMessage.ResourceString<(string -> string -> string)>
    val NonRigidTypar3E:
      unit -> DiagnosticMessage.ResourceString<(string -> string -> string)>
    val OBlockEndSentenceE: unit -> DiagnosticMessage.ResourceString<string>
    val UnexpectedEndOfInputE: unit -> DiagnosticMessage.ResourceString<string>
    val UnexpectedE:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val NONTERM_interactionE: unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_hashDirectiveE:
      unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_fieldDeclE: unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_unionCaseReprE:
      unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_localBindingE: unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_hardwhiteLetBindingsE:
      unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_classDefnMemberE:
      unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_defnBindingsE: unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_classMemberSpfnE:
      unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_valSpfnE: unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_tyconSpfnE: unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_anonLambdaExprE:
      unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_attrUnionCaseDeclE:
      unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_cPrototypeE: unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_objectImplementationMembersE:
      unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_ifExprCasesE: unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_openDeclE: unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_fileModuleSpecE:
      unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_patternClausesE:
      unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_beginEndExprE: unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_recdExprE: unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_tyconDefnE: unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_exconCoreE: unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_typeNameInfoE: unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_attributeListE:
      unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_quoteExprE: unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_typeConstraintE:
      unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_Category_ImplementationFileE:
      unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_Category_DefinitionE:
      unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_Category_SignatureFileE:
      unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_Category_PatternE:
      unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_Category_ExprE:
      unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_Category_TypeE:
      unit -> DiagnosticMessage.ResourceString<string>
    val NONTERM_typeArgsActualE:
      unit -> DiagnosticMessage.ResourceString<string>
    val TokenName1E:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val TokenName1TokenName2E:
      unit -> DiagnosticMessage.ResourceString<(string -> string -> string)>
    val TokenName1TokenName2TokenName3E:
      unit ->
        DiagnosticMessage.ResourceString<(string -> string -> string -> string)>
    val RuntimeCoercionSourceSealed1E:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val RuntimeCoercionSourceSealed2E:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val CoercionTargetSealedE:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val UpcastUnnecessaryE: unit -> DiagnosticMessage.ResourceString<string>
    val TypeTestUnnecessaryE: unit -> DiagnosticMessage.ResourceString<string>
    val OverrideDoesntOverride1E:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val OverrideDoesntOverride2E:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val OverrideDoesntOverride3E:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val OverrideDoesntOverride4E:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val UnionCaseWrongArgumentsE:
      unit -> DiagnosticMessage.ResourceString<(int -> int -> string)>
    val UnionPatternsBindDifferentNamesE:
      unit -> DiagnosticMessage.ResourceString<string>
    val RequiredButNotSpecifiedE:
      unit ->
        DiagnosticMessage.ResourceString<(string -> string -> string -> string)>
    val UseOfAddressOfOperatorE:
      unit -> DiagnosticMessage.ResourceString<string>
    val DefensiveCopyWarningE:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val DeprecatedThreadStaticBindingWarningE:
      unit -> DiagnosticMessage.ResourceString<string>
    val FunctionValueUnexpectedE:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val UnitTypeExpectedE:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val UnitTypeExpectedWithEqualityE:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val UnitTypeExpectedWithPossiblePropertySetterE:
      unit ->
        DiagnosticMessage.ResourceString<(string -> string -> string -> string)>
    val UnitTypeExpectedWithPossibleAssignmentE:
      unit -> DiagnosticMessage.ResourceString<(string -> string -> string)>
    val UnitTypeExpectedWithPossibleAssignmentToMutableE:
      unit -> DiagnosticMessage.ResourceString<(string -> string -> string)>
    val RecursiveUseCheckedAtRuntimeE:
      unit -> DiagnosticMessage.ResourceString<string>
    val LetRecUnsound1E:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val LetRecUnsound2E:
      unit -> DiagnosticMessage.ResourceString<(string -> string -> string)>
    val LetRecUnsoundInnerE:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val LetRecEvaluatedOutOfOrderE:
      unit -> DiagnosticMessage.ResourceString<string>
    val LetRecCheckedAtRuntimeE:
      unit -> DiagnosticMessage.ResourceString<string>
    val SelfRefObjCtor1E: unit -> DiagnosticMessage.ResourceString<string>
    val SelfRefObjCtor2E: unit -> DiagnosticMessage.ResourceString<string>
    val VirtualAugmentationOnNullValuedTypeE:
      unit -> DiagnosticMessage.ResourceString<string>
    val NonVirtualAugmentationOnNullValuedTypeE:
      unit -> DiagnosticMessage.ResourceString<string>
    val NonUniqueInferredAbstractSlot1E:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val NonUniqueInferredAbstractSlot2E:
      unit -> DiagnosticMessage.ResourceString<string>
    val NonUniqueInferredAbstractSlot3E:
      unit -> DiagnosticMessage.ResourceString<(string -> string -> string)>
    val NonUniqueInferredAbstractSlot4E:
      unit -> DiagnosticMessage.ResourceString<string>
    val Failure3E: unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val Failure4E: unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val MatchIncomplete1E: unit -> DiagnosticMessage.ResourceString<string>
    val MatchIncomplete2E:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val MatchIncomplete3E:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val MatchIncomplete4E: unit -> DiagnosticMessage.ResourceString<string>
    val RuleNeverMatchedE: unit -> DiagnosticMessage.ResourceString<string>
    val EnumMatchIncomplete1E: unit -> DiagnosticMessage.ResourceString<string>
    val ValNotMutableE:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val ValNotLocalE: unit -> DiagnosticMessage.ResourceString<string>
    val Obsolete1E: unit -> DiagnosticMessage.ResourceString<string>
    val Obsolete2E:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val ExperimentalE:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val PossibleUnverifiableCodeE:
      unit -> DiagnosticMessage.ResourceString<string>
    val DeprecatedE:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val LibraryUseOnlyE: unit -> DiagnosticMessage.ResourceString<string>
    val MissingFieldsE:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val ValueRestriction1E:
      unit ->
        DiagnosticMessage.ResourceString<(string -> string -> string -> string)>
    val ValueRestriction2E:
      unit ->
        DiagnosticMessage.ResourceString<(string -> string -> string -> string)>
    val ValueRestriction3E:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val ValueRestriction4E:
      unit ->
        DiagnosticMessage.ResourceString<(string -> string -> string -> string)>
    val ValueRestriction5E:
      unit ->
        DiagnosticMessage.ResourceString<(string -> string -> string -> string)>
    val RecoverableParseErrorE:
      unit -> DiagnosticMessage.ResourceString<string>
    val ReservedKeywordE:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val IndentationProblemE:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val OverrideInIntrinsicAugmentationE:
      unit -> DiagnosticMessage.ResourceString<string>
    val OverrideInExtrinsicAugmentationE:
      unit -> DiagnosticMessage.ResourceString<string>
    val IntfImplInIntrinsicAugmentationE:
      unit -> DiagnosticMessage.ResourceString<string>
    val IntfImplInExtrinsicAugmentationE:
      unit -> DiagnosticMessage.ResourceString<string>
    val UnresolvedReferenceNoRangeE:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val UnresolvedPathReferenceNoRangeE:
      unit -> DiagnosticMessage.ResourceString<(string -> string -> string)>
    val HashIncludeNotAllowedInNonScriptE:
      unit -> DiagnosticMessage.ResourceString<string>
    val HashReferenceNotAllowedInNonScriptE:
      unit -> DiagnosticMessage.ResourceString<string>
    val HashDirectiveNotAllowedInNonScriptE:
      unit -> DiagnosticMessage.ResourceString<string>
    val FileNameNotResolvedE:
      unit -> DiagnosticMessage.ResourceString<(string -> string -> string)>
    val AssemblyNotResolvedE:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val HashLoadedSourceHasIssues1E:
      unit -> DiagnosticMessage.ResourceString<string>
    val HashLoadedSourceHasIssues2E:
      unit -> DiagnosticMessage.ResourceString<string>
    val HashLoadedScriptConsideredSourceE:
      unit -> DiagnosticMessage.ResourceString<string>
    val InvalidInternalsVisibleToAssemblyName1E:
      unit -> DiagnosticMessage.ResourceString<(string -> string -> string)>
    val InvalidInternalsVisibleToAssemblyName2E:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val LoadedSourceNotFoundIgnoringE:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val MSBuildReferenceResolutionErrorE:
      unit -> DiagnosticMessage.ResourceString<(string -> string -> string)>
    val TargetInvocationExceptionWrapperE:
      unit -> DiagnosticMessage.ResourceString<(string -> string)>
    val getErrorString: key:string -> string
    val ( |InvalidArgument|_| ): exn:exn -> string option
    val OutputPhasedErrorR:
      os:System.Text.StringBuilder ->
        err:ErrorLogger.PhasedDiagnostic -> canSuggestNames:bool -> unit
    val OutputPhasedDiagnostic:
      System.Text.StringBuilder ->
        ErrorLogger.PhasedDiagnostic ->
          flattenErrors:bool -> suggestNames:bool -> unit
    val SanitizeFileName:
      fileName:string -> implicitIncludeDir:string -> string
    [<RequireQualifiedAccessAttribute>]
    type DiagnosticLocation =
      { Range: Range.range
        File: string
        TextRepresentation: string
        IsEmpty: bool }
    [<RequireQualifiedAccessAttribute>]
    type DiagnosticCanonicalInformation =
      { ErrorNumber: int
        Subcategory: string
        TextRepresentation: string }
    [<RequireQualifiedAccessAttribute>]
    type DiagnosticDetailedInfo =
      { Location: DiagnosticLocation option
        Canonical: DiagnosticCanonicalInformation
        Message: string }
    [<RequireQualifiedAccessAttribute>]
    type Diagnostic =
      | Short of bool * string
      | Long of bool * DiagnosticDetailedInfo
    val CollectDiagnostic:
      implicitIncludeDir:string * showFullPaths:bool * flattenErrors:bool *
      errorStyle:ErrorLogger.ErrorStyle * isError:bool *
      ErrorLogger.PhasedDiagnostic * suggestNames:bool -> seq<Diagnostic>
    val OutputDiagnostic:
      implicitIncludeDir:string * showFullPaths:bool * flattenErrors:bool *
      errorStyle:ErrorLogger.ErrorStyle * isError:bool ->
        System.Text.StringBuilder -> ErrorLogger.PhasedDiagnostic -> unit
    val OutputDiagnosticContext:
      prefix:string ->
        fileLineFunction:(string -> int -> string) ->
          System.Text.StringBuilder -> ErrorLogger.PhasedDiagnostic -> unit
    val ReportWarning:
      ErrorLogger.FSharpErrorSeverityOptions ->
        ErrorLogger.PhasedDiagnostic -> bool
    val ReportWarningAsError:
      ErrorLogger.FSharpErrorSeverityOptions ->
        ErrorLogger.PhasedDiagnostic -> bool
    type ErrorLoggerFilteringByScopedPragmas =
  
        inherit ErrorLogger.ErrorLogger
        new: checkFile:bool * scopedPragmas:SyntaxTree.ScopedPragma list *
              errorLogger:ErrorLogger.ErrorLogger ->
                ErrorLoggerFilteringByScopedPragmas
        override
          DiagnosticSink: phasedError:ErrorLogger.PhasedDiagnostic *
                           isError:bool -> unit
        override ErrorCount: int
    
    val GetErrorLoggerFilteringByScopedPragmas:
      checkFile:bool * SyntaxTree.ScopedPragma list * ErrorLogger.ErrorLogger ->
        ErrorLogger.ErrorLogger


namespace FSharp.Compiler
  module internal ParseAndCheckInputs =
    val CanonicalizeFilename: filename:string -> string
    val IsScript: string -> bool
    val QualFileNameOfModuleName:
      m:Range.range ->
        filename:string ->
          modname:SyntaxTree.Ident list -> SyntaxTree.QualifiedNameOfFile
    val QualFileNameOfFilename:
      m:Range.range -> filename:string -> SyntaxTree.QualifiedNameOfFile
    val ComputeQualifiedNameOfFileFromUniquePath:
      Range.range * string list -> SyntaxTree.QualifiedNameOfFile
    val QualFileNameOfSpecs:
      filename:string ->
        specs:SyntaxTree.SynModuleOrNamespaceSig list ->
          SyntaxTree.QualifiedNameOfFile
    val QualFileNameOfImpls:
      filename:string ->
        specs:SyntaxTree.SynModuleOrNamespace list ->
          SyntaxTree.QualifiedNameOfFile
    val PrependPathToQualFileName:
      x:SyntaxTree.Ident list ->
        SyntaxTree.QualifiedNameOfFile -> SyntaxTree.QualifiedNameOfFile
    val PrependPathToImpl:
      x:SyntaxTree.Ident list ->
        SyntaxTree.SynModuleOrNamespace -> SyntaxTree.SynModuleOrNamespace
    val PrependPathToSpec:
      x:SyntaxTree.Ident list ->
        SyntaxTree.SynModuleOrNamespaceSig -> SyntaxTree.SynModuleOrNamespaceSig
    val PrependPathToInput:
      SyntaxTree.Ident list -> SyntaxTree.ParsedInput -> SyntaxTree.ParsedInput
    val ComputeAnonModuleName:
      check:bool ->
        defaultNamespace:string option ->
          filename:string -> m:Range.range -> SyntaxTree.Ident list
    val PostParseModuleImpl:
      _i:'a * defaultNamespace:string option * isLastCompiland:(bool * bool) *
      filename:string * impl:SyntaxTree.ParsedImplFileFragment ->
        SyntaxTree.SynModuleOrNamespace
    val PostParseModuleSpec:
      _i:'a * defaultNamespace:string option * isLastCompiland:(bool * bool) *
      filename:string * intf:SyntaxTree.ParsedSigFileFragment ->
        SyntaxTree.SynModuleOrNamespaceSig
    val GetScopedPragmasForInput:
      input:SyntaxTree.ParsedInput -> SyntaxTree.ScopedPragma list
    val GetScopedPragmasForHashDirective:
      hd:SyntaxTree.ParsedHashDirective -> SyntaxTree.ScopedPragma list
    val PostParseModuleImpls:
      defaultNamespace:string option * filename:string *
      isLastCompiland:(bool * bool) * SyntaxTree.ParsedImplFile ->
        SyntaxTree.ParsedInput
    val PostParseModuleSpecs:
      defaultNamespace:string option * filename:string *
      isLastCompiland:(bool * bool) * SyntaxTree.ParsedSigFile ->
        SyntaxTree.ParsedInput
    type ModuleNamesDict =
      Map<string,Map<string,SyntaxTree.QualifiedNameOfFile>>
    val DeduplicateModuleName:
      moduleNamesDict:ModuleNamesDict ->
        fileName:string ->
          qualNameOfFile:SyntaxTree.QualifiedNameOfFile ->
            SyntaxTree.QualifiedNameOfFile * ModuleNamesDict
    val DeduplicateParsedInputModuleName:
      ModuleNamesDict ->
        SyntaxTree.ParsedInput -> SyntaxTree.ParsedInput * ModuleNamesDict
    val ParseInput:
      (Internal.Utilities.Text.Lexing.LexBuffer<char> -> Parser.token) *
      ErrorLogger.ErrorLogger * UnicodeLexing.Lexbuf * string option * string *
      isLastCompiland:(bool * bool) -> SyntaxTree.ParsedInput
    val ShowAllTokensAndExit:
      shortFilename:string * tokenizer:LexFilter.LexFilter *
      lexbuf:Internal.Utilities.Text.Lexing.LexBuffer<char> -> unit
    val TestInteractionParserAndExit:
      tokenizer:LexFilter.LexFilter *
      lexbuf:Internal.Utilities.Text.Lexing.LexBuffer<char> -> 'a
    val ReportParsingStatistics: res:SyntaxTree.ParsedInput -> unit
    val ParseOneInputLexbuf:
      tcConfig:CompilerConfig.TcConfig *
      lexResourceManager:Lexhelp.LexResourceManager *
      conditionalCompilationDefines:string list * lexbuf:UnicodeLexing.Lexbuf *
      filename:string * isLastCompiland:(bool * bool) *
      errorLogger:ErrorLogger.ErrorLogger -> SyntaxTree.ParsedInput option
    val ValidSuffixes: string list
    val ParseOneInputFile:
      CompilerConfig.TcConfig * Lexhelp.LexResourceManager * string list *
      string * isLastCompiland:(bool * bool) * ErrorLogger.ErrorLogger * bool ->
        SyntaxTree.ParsedInput option
    val ProcessMetaCommandsFromInput:
      (('T -> Range.range * string -> 'T) *
       ('T -> Range.range * string * CompilerConfig.Directive -> 'T) *
       ('T -> Range.range * string -> unit)) ->
        CompilerConfig.TcConfigBuilder * SyntaxTree.ParsedInput * string * 'T ->
          'T
    val ApplyNoWarnsToTcConfig:
      CompilerConfig.TcConfig * SyntaxTree.ParsedInput * string ->
        CompilerConfig.TcConfig
    val ApplyMetaCommandsFromInputToTcConfig:
      CompilerConfig.TcConfig * SyntaxTree.ParsedInput * string *
      Microsoft.DotNet.DependencyManager.DependencyProvider ->
        CompilerConfig.TcConfig
    val GetInitialTcEnv:
      assemblyName:string * Range.range * CompilerConfig.TcConfig *
      CompilerImports.TcImports * TcGlobals.TcGlobals -> CheckExpressions.TcEnv
    val CheckSimulateException: tcConfig:CompilerConfig.TcConfig -> unit
    type RootSigs =
      AbstractIL.Internal.Zmap<SyntaxTree.QualifiedNameOfFile,
                               TypedTree.ModuleOrNamespaceType>
    type RootImpls = AbstractIL.Internal.Zset<SyntaxTree.QualifiedNameOfFile>
    val qnameOrder:
      System.Collections.Generic.IComparer<SyntaxTree.QualifiedNameOfFile>
    [<SealedAttribute>]
    type TcState =
      { tcsCcu: TypedTree.CcuThunk
        tcsCcuType: TypedTree.ModuleOrNamespace
        tcsNiceNameGen: CompilerGlobalState.NiceNameGenerator
        tcsTcSigEnv: CheckExpressions.TcEnv
        tcsTcImplEnv: CheckExpressions.TcEnv
        tcsCreatesGeneratedProvidedTypes: bool
        tcsRootSigs: RootSigs
        tcsRootImpls: RootImpls
        tcsCcuSig: TypedTree.ModuleOrNamespaceType }
      with
        member
          NextStateAfterIncrementalFragment: CheckExpressions.TcEnv -> TcState
        member Ccu: TypedTree.CcuThunk
        member CcuSig: TypedTree.ModuleOrNamespaceType
        member CcuType: TypedTree.ModuleOrNamespace
        member CreatesGeneratedProvidedTypes: bool
        member NiceNameGenerator: CompilerGlobalState.NiceNameGenerator
        member TcEnvFromImpls: CheckExpressions.TcEnv
        member TcEnvFromSignatures: CheckExpressions.TcEnv
    
    val GetInitialTcState:
      Range.range * string * CompilerConfig.TcConfig * TcGlobals.TcGlobals *
      CompilerImports.TcImports * CompilerGlobalState.NiceNameGenerator *
      CheckExpressions.TcEnv -> TcState
    val TypeCheckOneInputEventually:
      checkForErrors:(unit -> bool) * CompilerConfig.TcConfig *
      CompilerImports.TcImports * TcGlobals.TcGlobals *
      SyntaxTree.LongIdent option * NameResolution.TcResultsSink * TcState *
      SyntaxTree.ParsedInput * skipImplIfSigExists:bool ->
        AbstractIL.Internal.Library.Eventually<(CheckExpressions.TcEnv *
                                                CheckDeclarations.TopAttribs *
                                                TypedTree.TypedImplFile option *
                                                TypedTree.ModuleOrNamespaceType) *
                                               TcState>
    val TypeCheckOneInput:
      ctok:AbstractIL.Internal.Library.CompilationThreadToken *
      checkForErrors:(unit -> bool) * tcConfig:CompilerConfig.TcConfig *
      tcImports:CompilerImports.TcImports * tcGlobals:TcGlobals.TcGlobals *
      prefixPathOpt:SyntaxTree.LongIdent option ->
        tcState:TcState ->
          inp:SyntaxTree.ParsedInput ->
            (CheckExpressions.TcEnv * CheckDeclarations.TopAttribs *
             TypedTree.TypedImplFile option * TypedTree.ModuleOrNamespaceType) *
            TcState
    val TypeCheckMultipleInputsFinish:
      (CheckExpressions.TcEnv * CheckDeclarations.TopAttribs * 'T option * 'U) list *
      TcState ->
        (CheckExpressions.TcEnv * CheckDeclarations.TopAttribs * 'T list *
         'U list) * TcState
    val TypeCheckOneInputAndFinishEventually:
      checkForErrors:(unit -> bool) * CompilerConfig.TcConfig *
      CompilerImports.TcImports * TcGlobals.TcGlobals *
      SyntaxTree.LongIdent option * NameResolution.TcResultsSink * TcState *
      SyntaxTree.ParsedInput ->
        AbstractIL.Internal.Library.Eventually<(CheckExpressions.TcEnv *
                                                CheckDeclarations.TopAttribs *
                                                TypedTree.TypedImplFile list *
                                                TypedTree.ModuleOrNamespaceType list) *
                                               TcState>
    val TypeCheckClosedInputSetFinish:
      TypedTree.TypedImplFile list * TcState ->
        TcState * TypedTree.TypedImplFile list
    val TypeCheckClosedInputSet:
      AbstractIL.Internal.Library.CompilationThreadToken *
      checkForErrors:(unit -> bool) * CompilerConfig.TcConfig *
      CompilerImports.TcImports * TcGlobals.TcGlobals *
      SyntaxTree.LongIdent option * TcState * SyntaxTree.ParsedInput list ->
        TcState * CheckDeclarations.TopAttribs * TypedTree.TypedImplFile list *
        CheckExpressions.TcEnv


namespace FSharp.Compiler
  module internal ScriptClosure =
    [<RequireQualifiedAccessAttribute>]
    type LoadClosureInput =
      { FileName: string
        SyntaxTree: SyntaxTree.ParsedInput option
        ParseDiagnostics: (ErrorLogger.PhasedDiagnostic * bool) list
        MetaCommandDiagnostics: (ErrorLogger.PhasedDiagnostic * bool) list }
    [<RequireQualifiedAccessAttribute>]
    type LoadClosure =
      { SourceFiles: (string * Range.range list) list
        References: (string * CompilerImports.AssemblyResolution list) list
        PackageReferences: (Range.range * string list) []
        UnresolvedReferences: CompilerConfig.UnresolvedAssemblyReference list
        Inputs: LoadClosureInput list
        OriginalLoadReferences: (Range.range * string * string) list
        NoWarns: (string * Range.range list) list
        ResolutionDiagnostics: (ErrorLogger.PhasedDiagnostic * bool) list
        AllRootFileDiagnostics: (ErrorLogger.PhasedDiagnostic * bool) list
        LoadClosureRootFileDiagnostics:
          (ErrorLogger.PhasedDiagnostic * bool) list }
      with
        static member
          ComputeClosureOfScriptFiles: AbstractIL.Internal.Library.CompilationThreadToken *
                                        tcConfig:CompilerConfig.TcConfig *
                                        (string * Range.range) list *
                                        implicitDefines:CodeContext *
                                        lexResourceManager:Lexhelp.LexResourceManager *
                                        dependencyProvider:Microsoft.DotNet.DependencyManager.DependencyProvider ->
                                          LoadClosure
        static member
          ComputeClosureOfScriptText: AbstractIL.Internal.Library.CompilationThreadToken *
                                       legacyReferenceResolver:ReferenceResolver.Resolver *
                                       defaultFSharpBinariesDir:string *
                                       filename:string *
                                       sourceText:Text.ISourceText *
                                       implicitDefines:CodeContext *
                                       useSimpleResolution:bool *
                                       useFsiAuxLib:bool * useSdkRefs:bool *
                                       lexResourceManager:Lexhelp.LexResourceManager *
                                       applyCompilerOptions:(CompilerConfig.TcConfigBuilder ->
                                                               unit) *
                                       assumeDotNetFramework:bool *
                                       tryGetMetadataSnapshot:AbstractIL.ILBinaryReader.ILReaderTryGetMetadataSnapshot *
                                       reduceMemoryUsage:AbstractIL.ILBinaryReader.ReduceMemoryFlag *
                                       dependencyProvider:Microsoft.DotNet.DependencyManager.DependencyProvider ->
                                         LoadClosure
    
    [<RequireQualifiedAccessAttribute>]
    type CodeContext =
      | CompilationAndEvaluation
      | Compilation
      | Editing
    module ScriptPreprocessClosure =
      type ClosureSource =
        | ClosureSource of
          filename: string * referenceRange: Range.range *
          sourceText: Text.ISourceText * parseRequired: bool
      type ClosureFile =
        | ClosureFile of
          string * Range.range * SyntaxTree.ParsedInput option *
          (ErrorLogger.PhasedDiagnostic * bool) list *
          (ErrorLogger.PhasedDiagnostic * bool) list *
          (string * Range.range) list
      type Observed =
    
          new: unit -> Observed
          member HaveSeen: check:string -> bool
          member SetSeen: check:string -> unit
      
      val ParseScriptText:
        filename:string * sourceText:Text.ISourceText *
        tcConfig:CompilerConfig.TcConfig * codeContext:CodeContext *
        lexResourceManager:Lexhelp.LexResourceManager *
        errorLogger:ErrorLogger.ErrorLogger -> SyntaxTree.ParsedInput option
      val CreateScriptTextTcConfig:
        legacyReferenceResolver:ReferenceResolver.Resolver *
        defaultFSharpBinariesDir:string * filename:string *
        codeContext:CodeContext * useSimpleResolution:bool * useFsiAuxLib:bool *
        basicReferences:#seq<Range.range * string> option *
        applyCommandLineArgs:(CompilerConfig.TcConfigBuilder -> unit) *
        assumeDotNetFramework:bool * useSdkRefs:bool *
        tryGetMetadataSnapshot:AbstractIL.ILBinaryReader.ILReaderTryGetMetadataSnapshot *
        reduceMemoryUsage:AbstractIL.ILBinaryReader.ReduceMemoryFlag ->
          CompilerConfig.TcConfig
      val ClosureSourceOfFilename:
        filename:string * m:Range.range * inputCodePage:int option *
        parseRequired:bool -> ClosureSource list
      val ApplyMetaCommandsFromInputToTcConfigAndGatherNoWarn:
        tcConfig:CompilerConfig.TcConfig * inp:SyntaxTree.ParsedInput *
        pathOfMetaCommandSource:string *
        dependencyProvider:Microsoft.DotNet.DependencyManager.DependencyProvider ->
          CompilerConfig.TcConfig * (string * Range.range) list
      val FindClosureFiles:
        mainFile:string * _m:'a * closureSources:ClosureSource list *
        origTcConfig:CompilerConfig.TcConfig * codeContext:CodeContext *
        lexResourceManager:Lexhelp.LexResourceManager *
        dependencyProvider:Microsoft.DotNet.DependencyManager.DependencyProvider ->
          ClosureFile list * CompilerConfig.TcConfig *
          (Range.range * string list) []
      val GetLoadClosure:
        ctok:AbstractIL.Internal.Library.CompilationThreadToken *
        rootFilename:string * closureFiles:ClosureFile list *
        tcConfig:CompilerConfig.TcConfig * codeContext:CodeContext *
        packageReferences:(Range.range * string list) [] -> LoadClosure
      val GetFullClosureOfScriptText:
        ctok:AbstractIL.Internal.Library.CompilationThreadToken *
        legacyReferenceResolver:ReferenceResolver.Resolver *
        defaultFSharpBinariesDir:string * filename:string *
        sourceText:Text.ISourceText * codeContext:CodeContext *
        useSimpleResolution:bool * useFsiAuxLib:bool * useSdkRefs:bool *
        lexResourceManager:Lexhelp.LexResourceManager *
        applyCommandLineArgs:(CompilerConfig.TcConfigBuilder -> unit) *
        assumeDotNetFramework:bool *
        tryGetMetadataSnapshot:AbstractIL.ILBinaryReader.ILReaderTryGetMetadataSnapshot *
        reduceMemoryUsage:AbstractIL.ILBinaryReader.ReduceMemoryFlag *
        dependencyProvider:Microsoft.DotNet.DependencyManager.DependencyProvider ->
          LoadClosure
      val GetFullClosureOfScriptFiles:
        ctok:AbstractIL.Internal.Library.CompilationThreadToken *
        tcConfig:CompilerConfig.TcConfig * files:(string * Range.range) list *
        codeContext:CodeContext * lexResourceManager:Lexhelp.LexResourceManager *
        dependencyProvider:Microsoft.DotNet.DependencyManager.DependencyProvider ->
          LoadClosure
  


namespace FSharp.Compiler
  module internal CompilerOptions =
    module Attributes =
  
    [<RequireQualifiedAccessAttribute>]
    type OptionSwitch =
      | On
      | Off
    type OptionSpec =
      | OptionClear of bool ref
      | OptionFloat of (float -> unit)
      | OptionInt of (int -> unit)
      | OptionSwitch of (OptionSwitch -> unit)
      | OptionIntList of (int -> unit)
      | OptionIntListSwitch of (int -> OptionSwitch -> unit)
      | OptionRest of (string -> unit)
      | OptionSet of bool ref
      | OptionString of (string -> unit)
      | OptionStringList of (string -> unit)
      | OptionStringListSwitch of (string -> OptionSwitch -> unit)
      | OptionUnit of (unit -> unit)
      | OptionHelp of (CompilerOptionBlock list -> unit)
      | OptionGeneral of (string list -> bool) * (string list -> string list)
    and CompilerOption =
      | CompilerOption of
        string * string * OptionSpec * Option<exn> * string option
    and CompilerOptionBlock =
      | PublicOptions of string * CompilerOption list
      | PrivateOptions of CompilerOption list
    val GetOptionsOfBlock: block:CompilerOptionBlock -> CompilerOption list
    val FilterCompilerOptionBlock:
      (CompilerOption -> bool) -> CompilerOptionBlock -> CompilerOptionBlock
    val compilerOptionUsage: CompilerOption -> string
    val PrintCompilerOption: CompilerOption -> unit
    val PrintPublicOptions: heading:string * opts:CompilerOption list -> unit
    val PrintCompilerOptionBlocks: CompilerOptionBlock list -> unit
    val dumpCompilerOption: prefix:string -> CompilerOption -> unit
    val dumpCompilerOptionBlock: _arg1:CompilerOptionBlock -> unit
    val DumpCompilerOptionBlocks: CompilerOptionBlock list -> unit
    val isSlashOpt: opt:string -> bool
    module ResponseFile =
      type ResponseFileData = ResponseFileLine list
      and ResponseFileLine =
        | CompilerOptionSpec of string
        | Comment of string
      val parseFile: path:string -> Choice<ResponseFileData,System.Exception>
  
    val ParseCompilerOptions:
      (string -> unit) * CompilerOptionBlock list * string list -> unit
    val lexFilterVerbose: bool
    val mutable enableConsoleColoring: bool
    val setFlag: r:(bool -> 'a) -> n:int -> 'a
    val SetOptimizeOff: tcConfigB:CompilerConfig.TcConfigBuilder -> unit
    val SetOptimizeOn: tcConfigB:CompilerConfig.TcConfigBuilder -> unit
    val SetOptimizeSwitch:
      CompilerConfig.TcConfigBuilder -> OptionSwitch -> unit
    val SetTailcallSwitch:
      CompilerConfig.TcConfigBuilder -> OptionSwitch -> unit
    val SetDeterministicSwitch:
      tcConfigB:CompilerConfig.TcConfigBuilder -> switch:OptionSwitch -> unit
    val AddPathMapping:
      tcConfigB:CompilerConfig.TcConfigBuilder -> pathPair:string -> unit
    val jitoptimizeSwitch:
      tcConfigB:CompilerConfig.TcConfigBuilder -> switch:OptionSwitch -> unit
    val localoptimizeSwitch:
      tcConfigB:CompilerConfig.TcConfigBuilder -> switch:OptionSwitch -> unit
    val crossOptimizeSwitch:
      tcConfigB:CompilerConfig.TcConfigBuilder -> switch:OptionSwitch -> unit
    val splittingSwitch:
      tcConfigB:CompilerConfig.TcConfigBuilder -> switch:OptionSwitch -> unit
    val callVirtSwitch:
      tcConfigB:CompilerConfig.TcConfigBuilder -> switch:OptionSwitch -> unit
    val useHighEntropyVASwitch:
      tcConfigB:CompilerConfig.TcConfigBuilder -> switch:OptionSwitch -> unit
    val subSystemVersionSwitch:
      tcConfigB:CompilerConfig.TcConfigBuilder -> text:string -> unit
    val SetUseSdkSwitch:
      tcConfigB:CompilerConfig.TcConfigBuilder -> switch:OptionSwitch -> unit
    val ( ++ ): x:'a list -> s:'a -> 'a list
    val SetTarget: tcConfigB:CompilerConfig.TcConfigBuilder -> s:string -> unit
    val SetDebugSwitch:
      CompilerConfig.TcConfigBuilder -> string option -> OptionSwitch -> unit
    val SetEmbedAllSourceSwitch:
      tcConfigB:CompilerConfig.TcConfigBuilder -> switch:OptionSwitch -> unit
    val setOutFileName:
      tcConfigB:CompilerConfig.TcConfigBuilder -> path:string -> unit
    val setSignatureFile:
      tcConfigB:CompilerConfig.TcConfigBuilder -> s:string -> unit
    val tagString: string
    val tagExe: string
    val tagWinExe: string
    val tagLibrary: string
    val tagModule: string
    val tagFile: string
    val tagFileList: string
    val tagDirList: string
    val tagPathList: string
    val tagResInfo: string
    val tagFullPDBOnlyPortable: string
    val tagWarnList: string
    val tagSymbolList: string
    val tagAddress: string
    val tagAlgorithm: string
    val tagInt: string
    val tagPathMap: string
    val tagNone: string
    val tagLangVersionValues: string
    val PrintOptionInfo: CompilerConfig.TcConfigBuilder -> unit
    val inputFileFlagsBoth:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption list
    val referenceFlagAbbrev:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption
    val compilerToolFlagAbbrev:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption
    val inputFileFlagsFsc:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption list
    val inputFileFlagsFsiBase:
      _tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption list
    val inputFileFlagsFsi:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption list
    val errorsAndWarningsFlags:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption list
    val outputFileFlagsFsi:
      _tcConfigB:CompilerConfig.TcConfigBuilder -> 'a list
    val outputFileFlagsFsc:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption list
    val resourcesFlagsFsi: _tcConfigB:CompilerConfig.TcConfigBuilder -> 'a list
    val resourcesFlagsFsc:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption list
    val codeGenerationFlags:
      isFsi:bool ->
        tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption list
    val defineSymbol:
      tcConfigB:CompilerConfig.TcConfigBuilder -> s:string -> unit
    val mlCompatibilityFlag:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption
    val setLanguageVersion: specifiedVersion:string -> Features.LanguageVersion
    val languageFlags:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption list
    val libFlag: tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption
    val codePageFlag:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption
    val preferredUiLang:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption
    val utf8OutputFlag:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption
    val fullPathsFlag:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption
    val cliRootFlag:
      _tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption
    val SetTargetProfile: CompilerConfig.TcConfigBuilder -> string -> unit
    val advancedFlagsBoth:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption list
    val noFrameworkFlag:
      isFsc:bool -> tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption
    val advancedFlagsFsi:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption list
    val advancedFlagsFsc:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption list
    val testFlag: tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption
    val editorSpecificFlags:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption list
    val internalFlags:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption list
    val compilingFsLibFlag:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption
    val compilingFsLib20Flag: CompilerOption
    val compilingFsLib40Flag: CompilerOption
    val compilingFsLibNoBigIntFlag: CompilerOption
    val mlKeywordsFlag: CompilerOption
    val gnuStyleErrorsFlag:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption
    val deprecatedFlagsBoth:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption list
    val deprecatedFlagsFsi:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption list
    val deprecatedFlagsFsc:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption list
    val DisplayBannerText: CompilerConfig.TcConfigBuilder -> unit
    val displayHelpFsc:
      tcConfigB:CompilerConfig.TcConfigBuilder ->
        blocks:CompilerOptionBlock list -> 'a
    val displayVersion: tcConfigB:CompilerConfig.TcConfigBuilder -> 'a
    val miscFlagsBoth:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption list
    val miscFlagsFsc:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption list
    val miscFlagsFsi:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption list
    val abbreviatedFlagsBoth:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption list
    val abbreviatedFlagsFsi:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption list
    val abbreviatedFlagsFsc:
      tcConfigB:CompilerConfig.TcConfigBuilder -> CompilerOption list
    val GetAbbrevFlagSet: CompilerConfig.TcConfigBuilder -> bool -> Set<string>
    val PostProcessCompilerArgs: Set<string> -> string [] -> string list
    val testingAndQAFlags: _tcConfigB:'a -> CompilerOption list
    val GetCoreFscCompilerOptions:
      CompilerConfig.TcConfigBuilder -> CompilerOptionBlock list
    val GetCoreServiceCompilerOptions:
      CompilerConfig.TcConfigBuilder -> CompilerOptionBlock list
    val GetCoreFsiCompilerOptions:
      CompilerConfig.TcConfigBuilder -> CompilerOptionBlock list
    val ApplyCommandLineArgs:
      tcConfigB:CompilerConfig.TcConfigBuilder * sourceFiles:string list *
      argv:string list -> string list
    val mutable showTermFileCount: int
    val PrintWholeAssemblyImplementation:
      g:TcGlobals.TcGlobals ->
        tcConfig:CompilerConfig.TcConfig ->
          outfile:string ->
            header:string -> expr:TypedTree.TypedImplFile list -> unit
    val mutable tPrev: (float * int []) option
    val mutable nPrev: string option
    val ReportTime: CompilerConfig.TcConfig -> string -> unit
    val ignoreFailureOnMono1_1_16: (unit -> unit) -> unit
    val foreBackColor:
      unit -> (System.ConsoleColor * System.ConsoleColor) option
    val DoWithColor: System.ConsoleColor -> (unit -> 'a) -> 'a
    val DoWithErrorColor: bool -> (unit -> 'a) -> 'a


namespace FSharp.Compiler
  module internal OptimizeInputs =
    val mutable showTermFileCount: int
    val PrintWholeAssemblyImplementation:
      g:TcGlobals.TcGlobals ->
        tcConfig:CompilerConfig.TcConfig ->
          outfile:string ->
            header:string -> expr:TypedTree.TypedImplFile list -> unit
    val AddExternalCcuToOptimizationEnv:
      TcGlobals.TcGlobals ->
        Optimizer.IncrementalOptimizationEnv ->
          CompilerImports.ImportedAssembly ->
            Optimizer.IncrementalOptimizationEnv
    val GetInitialOptimizationEnv:
      CompilerImports.TcImports * TcGlobals.TcGlobals ->
        Optimizer.IncrementalOptimizationEnv
    val ApplyAllOptimizations:
      CompilerConfig.TcConfig * TcGlobals.TcGlobals * ConstraintSolver.TcValF *
      string * Import.ImportMap * bool * Optimizer.IncrementalOptimizationEnv *
      TypedTree.CcuThunk * TypedTree.TypedImplFile list ->
        TypedTree.TypedAssemblyAfterOptimization * Optimizer.CcuOptimizationInfo *
        Optimizer.IncrementalOptimizationEnv
    val CreateIlxAssemblyGenerator:
      CompilerConfig.TcConfig * CompilerImports.TcImports * TcGlobals.TcGlobals *
      ConstraintSolver.TcValF * TypedTree.CcuThunk ->
        IlxGen.IlxAssemblyGenerator
    val GenerateIlxCode:
      IlxGen.IlxGenBackend * isInteractiveItExpr:bool * isInteractiveOnMono:bool *
      CompilerConfig.TcConfig * CheckDeclarations.TopAttribs *
      TypedTree.TypedAssemblyAfterOptimization * fragName:string *
      IlxGen.IlxAssemblyGenerator -> IlxGen.IlxGenResults
    val NormalizeAssemblyRefs:
      AbstractIL.Internal.Library.CompilationThreadToken *
      AbstractIL.IL.ILGlobals * CompilerImports.TcImports ->
        (AbstractIL.IL.ILScopeRef -> AbstractIL.IL.ILScopeRef)
    val GetGeneratedILModuleName:
      CompilerConfig.CompilerTarget -> string -> string


namespace FSharp.Compiler
  module internal XmlDocFileWriter =
    module XmlDocWriter =
      val hasDoc: doc:XmlDoc.XmlDoc -> bool
      val ComputeXmlDocSigs:
        tcGlobals:TcGlobals.TcGlobals * generatedCcu:TypedTree.CcuThunk -> unit
      val WriteXmlDocFile:
        assemblyName:string * generatedCcu:TypedTree.CcuThunk * xmlfile:string ->
          unit
  


namespace FSharp.Compiler
  module internal BinaryResourceFormats =
    module BinaryGenerationUtilities =
      val b0: n:int -> byte
      val b1: n:int -> byte
      val b2: n:int -> byte
      val b3: n:int -> byte
      val i16: i:int32 -> byte []
      val i32: i:int32 -> byte []
      val Padded: initialAlignment:int -> v:byte [] -> byte []
  
    module ResFileFormat =
      val ResFileNode:
        dwTypeID:int32 * dwNameID:int32 * wMemFlags:int32 * wLangID:int32 *
        data:byte [] -> byte []
      val ResFileHeader: unit -> byte []
  
    module VersionResourceFormat =
      val VersionInfoNode: data:byte [] -> byte []
      val VersionInfoElement:
        wType:int32 * szKey:byte [] * valueOpt:byte [] option *
        children:byte [] [] * isString:bool -> byte []
      val Version: version:AbstractIL.IL.ILVersionInfo -> byte []
      val String: string:string * value:string -> byte []
      val StringTable:
        language:string * strings:seq<string * string> -> byte []
      val StringFileInfo:
        stringTables:#seq<string * 'b> -> byte []
          when 'b:> seq<string * string>
      val VarFileInfo: vars:#seq<int32 * int32> -> byte []
      val VS_FIXEDFILEINFO:
        fileVersion:AbstractIL.IL.ILVersionInfo *
        productVersion:AbstractIL.IL.ILVersionInfo * dwFileFlagsMask:int32 *
        dwFileFlags:int32 * dwFileOS:int32 * dwFileType:int32 *
        dwFileSubtype:int32 * lwFileDate:int64 -> byte []
      val VS_VERSION_INFO:
        fixedFileInfo:(AbstractIL.IL.ILVersionInfo * AbstractIL.IL.ILVersionInfo *
                       int32 * int32 * int32 * int32 * int32 * int64) *
        stringFileInfo:seq<string * #seq<string * string>> *
        varFileInfo:seq<int32 * int32> -> byte []
      val VS_VERSION_INFO_RESOURCE:
        (AbstractIL.IL.ILVersionInfo * AbstractIL.IL.ILVersionInfo * int32 *
         int32 * int32 * int32 * int32 * int64) *
        seq<string * #seq<string * string>> * seq<int32 * int32> -> byte []
  
    module ManifestResourceFormat =
      val VS_MANIFEST_RESOURCE: data:byte [] * isLibrary:bool -> byte []
  


namespace FSharp.Compiler
  module internal StaticLinking =
    type TypeForwarding =
  
        new: tcImports:CompilerImports.TcImports -> TypeForwarding
        member
          TypeForwardILTypeRef: tref:AbstractIL.IL.ILTypeRef ->
                                   AbstractIL.IL.ILTypeRef
    
    val debugStaticLinking: bool
    val StaticLinkILModules:
      tcConfig:CompilerConfig.TcConfig * ilGlobals:AbstractIL.IL.ILGlobals *
      tcImports:CompilerImports.TcImports *
      ilxMainModule:AbstractIL.IL.ILModuleDef *
      dependentILModules:(TypedTree.CcuThunk option * AbstractIL.IL.ILModuleDef) list ->
        AbstractIL.IL.ILModuleDef *
        (AbstractIL.IL.ILScopeRef -> AbstractIL.IL.ILScopeRef)
    [<NoEquality; NoComparison>]
    type Node =
      { name: string
        data: AbstractIL.IL.ILModuleDef
        ccu: TypedTree.CcuThunk option
        refs: AbstractIL.IL.ILReferences
        mutable edges: Node list
        mutable visited: bool }
    val FindDependentILModulesForStaticLinking:
      ctok:AbstractIL.Internal.Library.CompilationThreadToken *
      tcConfig:CompilerConfig.TcConfig * tcImports:CompilerImports.TcImports *
      ilGlobals:AbstractIL.IL.ILGlobals *
      ilxMainModule:AbstractIL.IL.ILModuleDef ->
        (TypedTree.CcuThunk option * AbstractIL.IL.ILModuleDef) list
    val FindProviderGeneratedILModules:
      ctok:AbstractIL.Internal.Library.CompilationThreadToken *
      tcImports:CompilerImports.TcImports *
      providerGeneratedAssemblies:(CompilerImports.ImportedBinary * 'a) list ->
        ((TypedTree.CcuThunk option * AbstractIL.IL.ILScopeRef *
          AbstractIL.IL.ILModuleDef) * (string * 'a)) list
    val StaticLink:
      ctok:AbstractIL.Internal.Library.CompilationThreadToken *
      tcConfig:CompilerConfig.TcConfig * tcImports:CompilerImports.TcImports *
      ilGlobals:AbstractIL.IL.ILGlobals ->
        (AbstractIL.IL.ILModuleDef -> AbstractIL.IL.ILModuleDef)


namespace FSharp.Compiler
  module internal CreateILModule =
    module AttributeHelpers =
      val TryFindStringAttribute:
        g:TcGlobals.TcGlobals ->
          attrib:string -> attribs:TypedTree.Attribs -> string option
      val TryFindIntAttribute:
        g:TcGlobals.TcGlobals ->
          attrib:string -> attribs:TypedTree.Attribs -> int32 option
      val TryFindBoolAttribute:
        g:TcGlobals.TcGlobals ->
          attrib:string -> attribs:TypedTree.Attribs -> bool option
      val ( |ILVersion|_| ):
        versionString:string -> AbstractIL.IL.ILVersionInfo option
  
    type StrongNameSigningInfo =
      | StrongNameSigningInfo of
        delaysign: bool * publicsign: bool * signer: string option *
        container: string option
    val ValidateKeySigningAttributes:
      tcConfig:CompilerConfig.TcConfig * tcGlobals:TcGlobals.TcGlobals *
      CheckDeclarations.TopAttribs -> StrongNameSigningInfo
    val GetStrongNameSigner:
      signingInfo:StrongNameSigningInfo ->
        AbstractIL.Internal.StrongNameSign.ILStrongNameSigner option
    module MainModuleBuilder =
      val injectedCompatTypes: Set<string>
      val typesForwardedToMscorlib: Set<string>
      val typesForwardedToSystemNumerics: Set<string>
      val createMscorlibExportList:
        tcGlobals:TcGlobals.TcGlobals ->
          AbstractIL.IL.ILExportedTypeOrForwarder list
      val createSystemNumericsExportList:
        tcConfig:CompilerConfig.TcConfig ->
          tcImports:CompilerImports.TcImports ->
            AbstractIL.IL.ILExportedTypeOrForwarder list
      val fileVersion:
        findStringAttr:(string -> string option) ->
          assemblyVersion:AbstractIL.IL.ILVersionInfo ->
            AbstractIL.IL.ILVersionInfo
      val productVersion:
        findStringAttr:(string -> string option) ->
          fileVersion:AbstractIL.IL.ILVersionInfo -> string
      val productVersionToILVersionInfo: string -> AbstractIL.IL.ILVersionInfo
      val CreateMainModule:
        ctok:AbstractIL.Internal.Library.CompilationThreadToken *
        tcConfig:CompilerConfig.TcConfig * tcGlobals:TcGlobals.TcGlobals *
        tcImports:CompilerImports.TcImports * pdbfile:'t option *
        assemblyName:string * outfile:string *
        topAttrs:CheckDeclarations.TopAttribs *
        sigDataAttributes:AbstractIL.IL.ILAttribute list *
        sigDataResources:AbstractIL.IL.ILResource list *
        optDataResources:AbstractIL.IL.ILResource list *
        codegenResults:IlxGen.IlxGenResults *
        assemVerFromAttrib:AbstractIL.IL.ILVersionInfo option *
        metadataVersion:string * secDecls:AbstractIL.IL.ILSecurityDecls ->
          AbstractIL.IL.ILModuleDef
  


namespace FSharp.Compiler
  module internal Driver =
    [<AbstractClassAttribute>]
    type ErrorLoggerUpToMaxErrors =
  
        inherit ErrorLogger.ErrorLogger
        new: tcConfigB:CompilerConfig.TcConfigBuilder *
              exiter:ErrorLogger.Exiter * nameForDebugging:string ->
                ErrorLoggerUpToMaxErrors
        override
          DiagnosticSink: err:ErrorLogger.PhasedDiagnostic * isError:bool ->
                             unit
        abstract member
          HandleIssue: tcConfigB:CompilerConfig.TcConfigBuilder *
                        error:ErrorLogger.PhasedDiagnostic * isError:bool ->
                          unit
        abstract member HandleTooManyErrors: text:string -> unit
        override ErrorCount: int
    
    val ConsoleErrorLoggerUpToMaxErrors:
      tcConfigB:CompilerConfig.TcConfigBuilder * exiter:ErrorLogger.Exiter ->
        ErrorLogger.ErrorLogger
    type DelayAndForwardErrorLogger =
  
        inherit ErrorLogger.CapturingErrorLogger
        new: exiter:ErrorLogger.Exiter *
              errorLoggerProvider:ErrorLoggerProvider ->
                DelayAndForwardErrorLogger
        member
          ForwardDelayedDiagnostics: tcConfigB:CompilerConfig.TcConfigBuilder ->
                                        unit
    
    [<AbstractClassAttribute>]
    and ErrorLoggerProvider =
  
        new: unit -> ErrorLoggerProvider
        member
          CreateDelayAndForwardLogger: exiter:ErrorLogger.Exiter ->
                                          DelayAndForwardErrorLogger
        abstract member
          CreateErrorLoggerUpToMaxErrors: tcConfigBuilder:CompilerConfig.TcConfigBuilder *
                                           exiter:ErrorLogger.Exiter ->
                                             ErrorLogger.ErrorLogger
    
    type InProcErrorLoggerProvider =
  
        new: unit -> InProcErrorLoggerProvider
        member CapturedErrors: CompilerDiagnostics.Diagnostic []
        member CapturedWarnings: CompilerDiagnostics.Diagnostic []
        member Provider: ErrorLoggerProvider
    
    type ConsoleLoggerProvider =
  
        inherit ErrorLoggerProvider
        new: unit -> ConsoleLoggerProvider
        override
          CreateErrorLoggerUpToMaxErrors: tcConfigBuilder:CompilerConfig.TcConfigBuilder *
                                           exiter:ErrorLogger.Exiter ->
                                             ErrorLogger.ErrorLogger
    
    val AbortOnError:
      errorLogger:ErrorLogger.ErrorLogger * exiter:ErrorLogger.Exiter -> unit
    val TypeCheck:
      ctok:AbstractIL.Internal.Library.CompilationThreadToken *
      tcConfig:CompilerConfig.TcConfig * tcImports:CompilerImports.TcImports *
      tcGlobals:TcGlobals.TcGlobals * errorLogger:ErrorLogger.ErrorLogger *
      assemblyName:string * niceNameGen:CompilerGlobalState.NiceNameGenerator *
      tcEnv0:CheckExpressions.TcEnv * inputs:SyntaxTree.ParsedInput list *
      exiter:ErrorLogger.Exiter ->
        ParseAndCheckInputs.TcState * CheckDeclarations.TopAttribs *
        TypedTree.TypedImplFile list * CheckExpressions.TcEnv
    val AdjustForScriptCompile:
      ctok:AbstractIL.Internal.Library.CompilationThreadToken *
      tcConfigB:CompilerConfig.TcConfigBuilder *
      commandLineSourceFiles:string list *
      lexResourceManager:Lexhelp.LexResourceManager *
      dependencyProvider:Microsoft.DotNet.DependencyManager.DependencyProvider ->
        string list
    val SetProcessThreadLocals:
      tcConfigB:CompilerConfig.TcConfigBuilder -> unit
    val ProcessCommandLineFlags:
      tcConfigB:CompilerConfig.TcConfigBuilder * lcidFromCodePage:int option *
      argv:string [] -> string list
    val EncodeSignatureData:
      tcConfig:CompilerConfig.TcConfig * tcGlobals:TcGlobals.TcGlobals *
      exportRemapping:TypedTreeOps.Remap * generatedCcu:TypedTree.CcuThunk *
      outfile:string * isIncrementalBuild:bool ->
        AbstractIL.IL.ILAttribute list * AbstractIL.IL.ILResource list
    val EncodeOptimizationData:
      tcGlobals:TcGlobals.TcGlobals * tcConfig:CompilerConfig.TcConfig *
      outfile:string * exportRemapping:TypedTreeOps.Remap *
      data:(TypedTree.CcuThunk * #Optimizer.CcuOptimizationInfo) *
      isIncrementalBuild:bool -> AbstractIL.IL.ILResource list
    module InterfaceFileWriter =
      val BuildInitialDisplayEnvForSigFileGeneration:
        tcGlobals:TcGlobals.TcGlobals -> TypedTreeOps.DisplayEnv
      val WriteInterfaceFile:
        tcGlobals:TcGlobals.TcGlobals * tcConfig:CompilerConfig.TcConfig *
        infoReader:InfoReader.InfoReader *
        declaredImpls:seq<TypedTree.TypedImplFile> -> unit
  
    val CopyFSharpCore:
      outFile:string * referencedDlls:CompilerConfig.AssemblyReference list ->
        unit
    val TryFindVersionAttribute:
      g:TcGlobals.TcGlobals ->
        attrib:string ->
          attribName:System.String ->
            attribs:TypedTree.Attribs ->
              deterministic:bool -> AbstractIL.IL.ILVersionInfo option
    [<NoEquality; NoComparison>]
    type Args<'T> = | Args of 'T
    val main1:
      ctok:'a * argv:string [] *
      legacyReferenceResolver:ReferenceResolver.Resolver *
      bannerAlreadyPrinted:bool *
      reduceMemoryUsage:AbstractIL.ILBinaryReader.ReduceMemoryFlag *
      defaultCopyFSharpCore:CompilerConfig.CopyFSharpCoreFlag *
      exiter:ErrorLogger.Exiter * errorLoggerProvider:ErrorLoggerProvider *
      disposables:Lib.DisposablesTracker ->
        Args<'a * TcGlobals.TcGlobals * CompilerImports.TcImports *
             CompilerImports.TcImports * TypedTree.CcuThunk *
             TypedTree.TypedImplFile list * CheckDeclarations.TopAttribs *
             CompilerConfig.TcConfig * string * string option * string *
             ErrorLogger.ErrorLogger * ErrorLogger.Exiter>
        when 'a:> AbstractIL.Internal.Library.CompilationThreadToken
    val main1OfAst:
      ctok:'a * legacyReferenceResolver:ReferenceResolver.Resolver *
      reduceMemoryUsage:AbstractIL.ILBinaryReader.ReduceMemoryFlag *
      assemblyName:string * target:CompilerConfig.CompilerTarget * outfile:'b *
      pdbFile:'c option * dllReferences:string list * noframework:bool *
      exiter:ErrorLogger.Exiter * errorLoggerProvider:ErrorLoggerProvider *
      disposables:Lib.DisposablesTracker * inputs:SyntaxTree.ParsedInput list ->
        Args<'a * TcGlobals.TcGlobals * CompilerImports.TcImports *
             CompilerImports.TcImports * TypedTree.CcuThunk *
             TypedTree.TypedImplFile list * CheckDeclarations.TopAttribs *
             CompilerConfig.TcConfig * 'b * 'c option * string *
             ErrorLogger.ErrorLogger * ErrorLogger.Exiter>
        when 'a:> AbstractIL.Internal.Library.CompilationThreadToken
    val main2:
      Args<'a * 'b * CompilerImports.TcImports * 'c * TypedTree.CcuThunk * 'd *
           CheckDeclarations.TopAttribs * CompilerConfig.TcConfig * 'e * 'f *
           string * #ErrorLogger.ErrorLogger * ErrorLogger.Exiter> ->
        Args<'a * CompilerConfig.TcConfig * CompilerImports.TcImports * 'c * 'b *
             ErrorLogger.ErrorLogger * TypedTree.CcuThunk * 'e * 'd *
             CheckDeclarations.TopAttribs * 'f * string *
             AbstractIL.IL.ILVersionInfo option *
             CreateILModule.StrongNameSigningInfo * ErrorLogger.Exiter>
        when 'b:> TcGlobals.TcGlobals and 'd:> seq<TypedTree.TypedImplFile>
    val main3:
      Args<'a * CompilerConfig.TcConfig * CompilerImports.TcImports *
           CompilerImports.TcImports * 'b * ErrorLogger.ErrorLogger *
           TypedTree.CcuThunk * string * TypedTree.TypedImplFile list * 'c * 'd *
           'e * 'f * 'g * ErrorLogger.Exiter> ->
        Args<'a * CompilerConfig.TcConfig * CompilerImports.TcImports * 'b *
             ErrorLogger.ErrorLogger * TypedTree.CcuThunk * string *
             TypedTree.TypedAssemblyAfterOptimization * 'c * 'd * 'e *
             AbstractIL.IL.ILAttribute list * AbstractIL.IL.ILResource list *
             AbstractIL.IL.ILResource list * 'f * 'g * string *
             ErrorLogger.Exiter> when 'b:> TcGlobals.TcGlobals
    val main4:
      tcImportsCapture:(CompilerImports.TcImports -> unit) option *
      dynamicAssemblyCreator:'a option ->
        Args<'b * CompilerConfig.TcConfig * CompilerImports.TcImports *
             TcGlobals.TcGlobals * 'c * TypedTree.CcuThunk * string *
             TypedTree.TypedAssemblyAfterOptimization *
             CheckDeclarations.TopAttribs * 'd option * string *
             AbstractIL.IL.ILAttribute list * AbstractIL.IL.ILResource list *
             AbstractIL.IL.ILResource list * AbstractIL.IL.ILVersionInfo option *
             'e * string * ErrorLogger.Exiter> ->
          Args<'b * CompilerConfig.TcConfig * CompilerImports.TcImports *
               TcGlobals.TcGlobals * 'c *
               (AbstractIL.IL.ILModuleDef -> AbstractIL.IL.ILModuleDef) * string *
               'd option * AbstractIL.IL.ILModuleDef * 'e * ErrorLogger.Exiter>
        when 'b:> AbstractIL.Internal.Library.CompilationThreadToken and
             'c:> ErrorLogger.ErrorLogger
    val main5:
      Args<'a * 'b * 'c * 'd * ErrorLogger.ErrorLogger * ('e -> 'f) * 'g * 'h *
           'e * 'i * ErrorLogger.Exiter> ->
        Args<'a * 'b * 'c * 'd * ErrorLogger.ErrorLogger * 'f * 'g * 'h * 'i *
             ErrorLogger.Exiter>
    val main6:
      dynamicAssemblyCreator:(TcGlobals.TcGlobals * string *
                              AbstractIL.IL.ILModuleDef -> unit) option ->
        Args<#AbstractIL.Internal.Library.CompilationThreadToken *
             CompilerConfig.TcConfig * CompilerImports.TcImports *
             TcGlobals.TcGlobals * ErrorLogger.ErrorLogger *
             AbstractIL.IL.ILModuleDef * string * string option *
             CreateILModule.StrongNameSigningInfo * ErrorLogger.Exiter> -> unit
    val mainCompile:
      ctok:AbstractIL.Internal.Library.CompilationThreadToken * argv:string [] *
      legacyReferenceResolver:ReferenceResolver.Resolver *
      bannerAlreadyPrinted:bool *
      reduceMemoryUsage:AbstractIL.ILBinaryReader.ReduceMemoryFlag *
      defaultCopyFSharpCore:CompilerConfig.CopyFSharpCoreFlag *
      exiter:ErrorLogger.Exiter * loggerProvider:ErrorLoggerProvider *
      tcImportsCapture:(CompilerImports.TcImports -> unit) option *
      dynamicAssemblyCreator:(TcGlobals.TcGlobals * string *
                              AbstractIL.IL.ILModuleDef -> unit) option -> unit
    val compileOfAst:
      ctok:AbstractIL.Internal.Library.CompilationThreadToken *
      legacyReferenceResolver:ReferenceResolver.Resolver *
      reduceMemoryUsage:AbstractIL.ILBinaryReader.ReduceMemoryFlag *
      assemblyName:string * target:CompilerConfig.CompilerTarget *
      targetDll:string * targetPdb:string option * dependencies:string list *
      noframework:bool * exiter:ErrorLogger.Exiter *
      loggerProvider:ErrorLoggerProvider * inputs:SyntaxTree.ParsedInput list *
      tcImportsCapture:(CompilerImports.TcImports -> unit) option *
      dynamicAssemblyCreator:(TcGlobals.TcGlobals * string *
                              AbstractIL.IL.ILModuleDef -> unit) option -> unit


namespace FSharp.Compiler.SourceCodeServices
  module EnvMisc2 =
    val maxMembers: int

  [<RequireQualifiedAccessAttribute>]
  type FSharpErrorSeverity =
    | Warning
    | Error
  module FSharpErrorInfo =
    [<LiteralAttribute>]
    val ObsoleteMessage: string
    = "Use FSharpErrorInfo.Range. This API will be removed in a future update."

  [<ClassAttribute>]
  type FSharpErrorInfo =

      new: m:Range.range * severity:FSharpErrorSeverity * message:string *
            subcategory:string * errorNum:int -> FSharpErrorInfo
      static member
        CreateFromException: ErrorLogger.PhasedDiagnostic * isError:bool *
                              Range.range * suggestNames:bool -> FSharpErrorInfo
      static member
        CreateFromExceptionAndAdjustEof: ErrorLogger.PhasedDiagnostic *
                                          isError:bool * Range.range *
                                          lastPosInFile:(int * int) *
                                          suggestNames:bool -> FSharpErrorInfo
      override ToString: unit -> string
      member WithEnd: newEnd:Range.pos -> FSharpErrorInfo
      member WithStart: newStart:Range.pos -> FSharpErrorInfo
      [<System.Obsolete
        ("Use FSharpErrorInfo.Range. This API will be removed in a future update.")>]
      member End: Range.pos
      [<System.Obsolete
        ("Use FSharpErrorInfo.Range. This API will be removed in a future update.")>]
      member EndColumn: int
      [<System.Obsolete
        ("Use FSharpErrorInfo.Range. This API will be removed in a future update.")>]
      member EndLine: Range.Line0
      [<System.Obsolete
        ("Use FSharpErrorInfo.Range. This API will be removed in a future update.")>]
      member EndLineAlternate: int
      member ErrorNumber: int
      [<System.Obsolete
        ("Use FSharpErrorInfo.Range. This API will be removed in a future update.")>]
      member FileName: string
      member Message: string
      member Range: Range.range
      member Severity: FSharpErrorSeverity
      [<System.Obsolete
        ("Use FSharpErrorInfo.Range. This API will be removed in a future update.")>]
      member Start: Range.pos
      [<System.Obsolete
        ("Use FSharpErrorInfo.Range. This API will be removed in a future update.")>]
      member StartColumn: int
      [<System.Obsolete
        ("Use FSharpErrorInfo.Range. This API will be removed in a future update.")>]
      member StartLine: Range.Line0
      [<System.Obsolete
        ("Use FSharpErrorInfo.Range. This API will be removed in a future update.")>]
      member StartLineAlternate: int
      member Subcategory: string
  
  [<SealedAttribute>]
  type ErrorScope =

      interface System.IDisposable
      new: unit -> ErrorScope
      static member
        Protect: Range.range -> (unit -> 'a) -> (string -> 'a) -> 'a
      member TryGetFirstErrorText: unit -> string option
      member Diagnostics: FSharpErrorInfo list
      member Errors: FSharpErrorInfo list
      member FirstError: string option
      member Warnings: FSharpErrorInfo list
  
  type internal CompilationErrorLogger =

      inherit ErrorLogger.ErrorLogger
      new: debugName:string * options:ErrorLogger.FSharpErrorSeverityOptions ->
              CompilationErrorLogger
      override
        DiagnosticSink: exn:ErrorLogger.PhasedDiagnostic * isError:bool -> unit
      member
        GetErrors: unit ->
                      (ErrorLogger.PhasedDiagnostic * FSharpErrorSeverity) []
      override ErrorCount: int
  
  type CompilationGlobalsScope =

      interface System.IDisposable
      new: ErrorLogger.ErrorLogger * ErrorLogger.BuildPhase ->
              CompilationGlobalsScope
  
  module ErrorHelpers =
    val ReportError:
      ErrorLogger.FSharpErrorSeverityOptions * allErrors:bool *
      mainInputFileName:string * fileInfo:(int * int) *
      (ErrorLogger.PhasedDiagnostic * FSharpErrorSeverity) * suggestNames:bool ->
        FSharpErrorInfo list
    val CreateErrorInfos:
      ErrorLogger.FSharpErrorSeverityOptions * allErrors:bool *
      mainInputFileName:string *
      seq<ErrorLogger.PhasedDiagnostic * FSharpErrorSeverity> *
      suggestNames:bool -> FSharpErrorInfo []

  type Layout = Internal.Utilities.StructuredFormat.Layout
  [<RequireQualifiedAccessAttribute>]
  type FSharpXmlDoc =
    | None
    | Text of unprocessedLines: string [] * elaboratedXmlLines: string []
    | XmlDocFileSignature of string * string
  [<RequireQualifiedAccessAttribute>]
  type FSharpToolTipElementData<'T> =
    { MainDescription: 'T
      XmlDoc: FSharpXmlDoc
      TypeMapping: 'T list
      Remarks: 'T option
      ParamName: string option }
    with
      static member
        Create: layout:'T * xml:FSharpXmlDoc * ?typeMapping:'T list *
                 ?paramName:string * ?remarks:'T -> FSharpToolTipElementData<'T>
  
  [<RequireQualifiedAccessAttribute>]
  type FSharpToolTipElement<'T> =
    | None
    | Group of FSharpToolTipElementData<'T> list
    | CompositionError of string
    with
      static member
        Single: 'T * FSharpXmlDoc * ?typeMapping:'T list * ?paramName:string *
                 ?remarks:'T -> FSharpToolTipElement<'T>
  
  type FSharpToolTipElement = FSharpToolTipElement<string>
  type FSharpStructuredToolTipElement = FSharpToolTipElement<Layout>
  type FSharpToolTipText<'T> =
    | FSharpToolTipText of FSharpToolTipElement<'T> list
  type FSharpToolTipText = FSharpToolTipText<string>
  type FSharpStructuredToolTipText = FSharpToolTipText<Layout>
  module Tooltips =
    val ToFSharpToolTipElement:
      FSharpToolTipElement<Layout> -> FSharpToolTipElement<string>
    val ToFSharpToolTipText:
      FSharpToolTipText<Layout> -> FSharpToolTipText<string>

  [<RequireQualifiedAccessAttribute>]
  type CompletionItemKind =
    | Field
    | Property
    | Method of isExtension: bool
    | Event
    | Argument
    | CustomOperation
    | Other
  type UnresolvedSymbol =
    { FullName: string
      DisplayName: string
      Namespace: string [] }
  type CompletionItem =
    { ItemWithInst: NameResolution.ItemWithInst
      Kind: CompletionItemKind
      IsOwnMember: bool
      MinorPriority: int
      Type: TypedTree.TyconRef option
      Unresolved: UnresolvedSymbol option }
    with
      member Item: NameResolution.Item
  
  module internal SymbolHelpers =
    val OutputFullName:
      isListItem:bool ->
        ppF:('a -> 'b option) ->
          fnF:('a -> Internal.Utilities.StructuredFormat.Layout) ->
            r:'a -> Internal.Utilities.StructuredFormat.Layout
    val rangeOfValRef:
      preferFlag:bool option -> vref:TypedTree.ValRef -> Range.range
    val rangeOfEntityRef:
      preferFlag:bool option -> eref:TypedTree.EntityRef -> Range.range
    val rangeOfPropInfo:
      preferFlag:bool option -> pinfo:Infos.PropInfo -> Range.range option
    val rangeOfMethInfo:
      g:TcGlobals.TcGlobals ->
        preferFlag:bool option -> minfo:Infos.MethInfo -> Range.range option
    val rangeOfEventInfo:
      preferFlag:bool option -> einfo:Infos.EventInfo -> Range.range option
    val rangeOfUnionCaseInfo:
      preferFlag:bool option -> ucinfo:Infos.UnionCaseInfo -> Range.range
    val rangeOfRecdField:
      preferFlag:bool option -> rField:TypedTree.RecdField -> Range.range
    val rangeOfRecdFieldInfo:
      preferFlag:bool option -> rfinfo:Infos.RecdFieldInfo -> Range.range
    val rangeOfItem:
      TcGlobals.TcGlobals ->
        bool option -> NameResolution.Item -> Range.range option
    val computeCcuOfTyconRef:
      tcref:TypedTree.TyconRef -> TypedTree.CcuThunk option
    val ccuOfMethInfo:
      g:TcGlobals.TcGlobals -> minfo:Infos.MethInfo -> TypedTree.CcuThunk option
    val ccuOfItem:
      TcGlobals.TcGlobals -> NameResolution.Item -> TypedTree.CcuThunk option
    val fileNameOfItem:
      TcGlobals.TcGlobals ->
        string option -> Range.range -> NameResolution.Item -> string
    val cutFileName: s:string -> string
    val libFileOfEntityRef: x:TypedTree.EntityRef -> string option
    val ParamNameAndTypesOfUnaryCustomOperation:
      TcGlobals.TcGlobals -> Infos.MethInfo -> Infos.ParamNameAndType list
    val metaInfoOfEntityRef:
      infoReader:InfoReader.InfoReader ->
        m:Range.range ->
          tcref:TypedTree.EntityRef ->
            (string option * TypedTree.Typars * Infos.ILTypeInfo) option
    val mkXmlComment: thing:(string option * string) option -> FSharpXmlDoc
    val GetXmlDocSigOfEntityRef:
      InfoReader.InfoReader ->
        Range.range -> TypedTree.EntityRef -> (string option * string) option
    val GetXmlDocSigOfScopedValRef:
      TcGlobals.TcGlobals ->
        TypedTree.TyconRef ->
          TypedTree.ValRef -> (string option * string) option
    val GetXmlDocSigOfRecdFieldInfo:
      Infos.RecdFieldInfo -> (string option * string) option
    val GetXmlDocSigOfUnionCaseInfo:
      Infos.UnionCaseInfo -> (string option * string) option
    val GetXmlDocSigOfMethInfo:
      InfoReader.InfoReader ->
        Range.range -> Infos.MethInfo -> (string option * string) option
    val GetXmlDocSigOfValRef:
      TcGlobals.TcGlobals -> TypedTree.ValRef -> (string option * string) option
    val GetXmlDocSigOfProp:
      InfoReader.InfoReader ->
        Range.range -> Infos.PropInfo -> (string option * string) option
    val GetXmlDocSigOfEvent:
      InfoReader.InfoReader ->
        Range.range -> Infos.EventInfo -> (string option * string) option
    val GetXmlDocSigOfILFieldInfo:
      InfoReader.InfoReader ->
        Range.range -> Infos.ILFieldInfo -> (string option * string) option
    val GetXmlDocHelpSigOfItemForLookup:
      infoReader:InfoReader.InfoReader ->
        m:Range.range -> d:NameResolution.Item -> FSharpXmlDoc
    val GetXmlCommentForItemAux:
      xmlDoc:XmlDoc.XmlDoc option ->
        infoReader:InfoReader.InfoReader ->
          m:Range.range -> d:NameResolution.Item -> FSharpXmlDoc
    val mutable ToolTipFault: string option
    val GetXmlCommentForMethInfoItem:
      infoReader:InfoReader.InfoReader ->
        m:Range.range ->
          d:NameResolution.Item -> minfo:Infos.MethInfo -> FSharpXmlDoc
    val FormatTyparMapping:
      denv:TypedTreeOps.DisplayEnv ->
        prettyTyparInst:TypedTreeOps.TyparInst ->
          Internal.Utilities.StructuredFormat.Layout list
    val FormatOverloadsToList:
      infoReader:InfoReader.InfoReader ->
        m:Range.range ->
          denv:TypedTreeOps.DisplayEnv ->
            item:NameResolution.ItemWithInst ->
              minfos:seq<Infos.MethInfo> -> FSharpStructuredToolTipElement
    val pubpathOfValRef: v:TypedTree.ValRef -> TypedTree.ValPublicPath option
    val pubpathOfTyconRef: x:TypedTree.TyconRef -> TypedTree.PublicPath option
    val ( |ItemWhereTypIsPreferred|_| ):
      item:NameResolution.Item -> TypedTree.TType option
    val ItemDisplayPartialEquality:
      g:TcGlobals.TcGlobals ->
        AbstractIL.Internal.Library.IPartialEqualityComparer<NameResolution.Item>
    val CompletionItemDisplayPartialEquality:
      g:TcGlobals.TcGlobals ->
        AbstractIL.Internal.Library.IPartialEqualityComparer<CompletionItem>
    val ItemWithTypeDisplayPartialEquality:
      g:TcGlobals.TcGlobals ->
        AbstractIL.Internal.Library.IPartialEqualityComparer<NameResolution.Item *
                                                             'a>
    val RemoveDuplicateModuleRefs:
      modrefs:TypedTree.ModuleOrNamespaceRef list ->
        TypedTree.ModuleOrNamespaceRef list
    val RemoveDuplicateItems:
      TcGlobals.TcGlobals ->
        NameResolution.ItemWithInst list -> NameResolution.ItemWithInst list
    val RemoveDuplicateCompletionItems:
      TcGlobals.TcGlobals -> CompletionItem list -> CompletionItem list
    val IsExplicitlySuppressed:
      TcGlobals.TcGlobals -> NameResolution.Item -> bool
    val RemoveExplicitlySuppressed:
      TcGlobals.TcGlobals ->
        NameResolution.ItemWithInst list -> NameResolution.ItemWithInst list
    val RemoveExplicitlySuppressedCompletionItems:
      TcGlobals.TcGlobals -> CompletionItem list -> CompletionItem list
    val SimplerDisplayEnv: TypedTreeOps.DisplayEnv -> TypedTreeOps.DisplayEnv
    val FullNameOfItem: TcGlobals.TcGlobals -> NameResolution.Item -> string
    val GetXmlCommentForItem:
      InfoReader.InfoReader ->
        Range.range -> NameResolution.Item -> FSharpXmlDoc
    val IsAttribute: InfoReader.InfoReader -> NameResolution.Item -> bool
    val FormatItemDescriptionToToolTipElement:
      isListItem:bool ->
        infoReader:InfoReader.InfoReader ->
          m:Range.range ->
            denv:TypedTreeOps.DisplayEnv ->
              item:NameResolution.ItemWithInst -> FSharpToolTipElement<Layout>
    val ( |ItemIsProvidedType|_| ):
      TcGlobals.TcGlobals -> NameResolution.Item -> TypedTree.TyconRef option
    val ( |ItemIsProvidedTypeWithStaticArguments|_| ):
      Range.range ->
        TcGlobals.TcGlobals ->
          NameResolution.Item ->
            Tainted<ExtensionTyping.ProvidedParameterInfo> [] option
    val ( |ItemIsProvidedMethodWithStaticArguments|_| ):
      item:NameResolution.Item ->
        Tainted<ExtensionTyping.ProvidedParameterInfo> [] option
    val ( |ItemIsWithStaticArguments|_| ):
      Range.range ->
        TcGlobals.TcGlobals ->
          NameResolution.Item ->
            Tainted<ExtensionTyping.ProvidedParameterInfo> [] option
    val GetF1Keyword:
      TcGlobals.TcGlobals -> NameResolution.Item -> string option
    val FormatStructuredDescriptionOfItem:
      isDecl:bool ->
        InfoReader.InfoReader ->
          Range.range ->
            TypedTreeOps.DisplayEnv ->
              NameResolution.ItemWithInst -> FSharpToolTipElement<Layout>
    val FlattenItems:
      TcGlobals.TcGlobals ->
        Range.range -> NameResolution.Item -> NameResolution.Item list


namespace FSharp.Compiler.SourceCodeServices
  type FSharpAccessibility =

      new: TypedTree.Accessibility * ?isProtected:bool -> FSharpAccessibility
      override ToString: unit -> string
      member internal Contents: TypedTree.Accessibility
      member IsInternal: bool
      member IsPrivate: bool
      member IsProtected: bool
      member IsPublic: bool
  
  and SymbolEnv =

      new: TcGlobals.TcGlobals * thisCcu:TypedTree.CcuThunk *
            thisCcuTyp:TypedTree.ModuleOrNamespaceType option *
            tcImports:CompilerImports.TcImports -> SymbolEnv
      new: TcGlobals.TcGlobals * thisCcu:TypedTree.CcuThunk *
            thisCcuTyp:TypedTree.ModuleOrNamespaceType option *
            tcImports:CompilerImports.TcImports * amap:Import.ImportMap *
            infoReader:InfoReader.InfoReader -> SymbolEnv
      member amap: Import.ImportMap
      member g: TcGlobals.TcGlobals
      member infoReader: InfoReader.InfoReader
      member tcImports: CompilerImports.TcImports
      member
        tcValF: (TypedTree.ValRef -> TypedTree.ValUseFlag ->
                    TypedTree.TTypes -> Range.range ->
                    TypedTree.Expr * TypedTree.TType)
      member thisCcu: TypedTree.CcuThunk
      member thisCcuTy: TypedTree.ModuleOrNamespaceType option
  
  and FSharpDisplayContext =

      new: denv:(TcGlobals.TcGlobals -> TypedTreeOps.DisplayEnv) ->
              FSharpDisplayContext
      member Contents: g:TcGlobals.TcGlobals -> TypedTreeOps.DisplayEnv
      member WithShortTypeNames: bool -> FSharpDisplayContext
      static member Empty: FSharpDisplayContext
  
  [<ClassAttribute>]
  and FSharpSymbol =

      new: cenv:SymbolEnv * item:(unit -> NameResolution.Item) *
            access:(FSharpSymbol -> TypedTree.CcuThunk ->
                      AccessibilityLogic.AccessorDomain -> bool) -> FSharpSymbol
      static member
        Create: cenv:SymbolEnv * item:NameResolution.Item -> FSharpSymbol
      static member
        Create: g:TcGlobals.TcGlobals * thisCcu:TypedTree.CcuThunk *
                 thisCcuTyp:TypedTree.ModuleOrNamespaceType *
                 tcImports:CompilerImports.TcImports * item:NameResolution.Item ->
                   FSharpSymbol
      static member
        GetAccessibility: FSharpSymbol -> FSharpAccessibility option
      override Equals: other:obj -> bool
      member GetEffectivelySameAsHash: unit -> int
      override GetHashCode: unit -> int
      member IsAccessible: FSharpAccessibilityRights -> bool
      member IsEffectivelySameAs: other:FSharpSymbol -> bool
      override ToString: unit -> string
      member Assembly: FSharpAssembly
      member DeclarationLocation: Range.range option
      member DisplayName: string
      member FullName: string
      member ImplementationLocation: Range.range option
      member IsExplicitlySuppressed: bool
      member internal Item: NameResolution.Item
      member SignatureLocation: Range.range option
  
  and FSharpEntity =

      inherit FSharpSymbol
      new: SymbolEnv * TypedTree.EntityRef -> FSharpEntity
      override Equals: other:obj -> bool
      override GetHashCode: unit -> int
      override ToString: unit -> string
      member AbbreviatedType: FSharpType
      member AccessPath: string
      member Accessibility: FSharpAccessibility
      member ActivePatternCases: FSharpActivePatternCase list
      member AllCompilationPaths: string list
      member AllInterfaces: System.Collections.Generic.IList<FSharpType>
      member ArrayRank: int
      member Attributes: System.Collections.Generic.IList<FSharpAttribute>
      member BaseType: FSharpType option
      member CompiledName: string
      member DeclarationLocation: Range.range
      member DeclaredInterfaces: System.Collections.Generic.IList<FSharpType>
      member DeclaringEntity: FSharpEntity option
      member DisplayName: string
      member ElaboratedXmlDoc: System.Collections.Generic.IList<string>
      member Entity: TypedTree.EntityRef
      member FSharpDelegateSignature: FSharpDelegateSignature
      member FSharpFields: System.Collections.Generic.IList<FSharpField>
      member FullName: string
      member
        GenericParameters: System.Collections.Generic.IList<FSharpGenericParameter>
      member HasAssemblyCodeRepresentation: bool
      member HasFSharpModuleSuffix: bool
      member IsAbstractClass: bool
      member IsArrayType: bool
      member IsAttributeType: bool
      member IsByRef: bool
      member IsClass: bool
      member IsDelegate: bool
      member IsDisposableType: bool
      member IsEnum: bool
      member IsFSharp: bool
      member IsFSharpAbbreviation: bool
      member IsFSharpExceptionDeclaration: bool
      member IsFSharpModule: bool
      member IsFSharpRecord: bool
      member IsFSharpUnion: bool
      member IsInterface: bool
      member IsMeasure: bool
      member IsNamespace: bool
      member IsOpaque: bool
      member IsProvided: bool
      member IsProvidedAndErased: bool
      member IsProvidedAndGenerated: bool
      member IsStaticInstantiation: bool
      member IsUnresolved: bool
      member IsValueType: bool
      member LogicalName: string
      member
        MembersFunctionsAndValues: System.Collections.Generic.IList<FSharpMemberOrFunctionOrValue>
      [<System.Obsolete ("Renamed to MembersFunctionsAndValues")>]
      member
        MembersOrValues: System.Collections.Generic.IList<FSharpMemberOrFunctionOrValue>
      member Namespace: string option
      member NestedEntities: System.Collections.Generic.IList<FSharpEntity>
      member QualifiedName: string
      [<System.Obsolete ("Renamed to FSharpFields")>]
      member RecordFields: System.Collections.Generic.IList<FSharpField>
      member RepresentationAccessibility: FSharpAccessibility
      member
        StaticParameters: System.Collections.Generic.IList<FSharpStaticParameter>
      member TryFullName: string option
      member UnionCases: System.Collections.Generic.IList<FSharpUnionCase>
      member UsesPrefixDisplay: bool
      member XmlDoc: System.Collections.Generic.IList<string>
      member XmlDocSig: string
  
  [<ClassAttribute>]
  and FSharpUnionCase =

      inherit FSharpSymbol
      new: SymbolEnv * TypedTree.UnionCaseRef -> FSharpUnionCase
      override Equals: other:obj -> bool
      override GetHashCode: unit -> int
      override ToString: unit -> string
      member Accessibility: FSharpAccessibility
      member Attributes: System.Collections.Generic.IList<FSharpAttribute>
      member CompiledName: string
      member DeclarationLocation: Range.range
      member ElaboratedXmlDoc: System.Collections.Generic.IList<string>
      member HasFields: bool
      member IsUnresolved: bool
      member Name: string
      member ReturnType: FSharpType
      member UnionCaseFields: System.Collections.Generic.IList<FSharpField>
      member private V: TypedTree.UnionCaseRef
      member XmlDoc: System.Collections.Generic.IList<string>
      member XmlDocSig: string
  
  and FSharpFieldData =
    | AnonField of
      TypedTree.AnonRecdTypeInfo * TypedTree.TTypes * int * Range.range
    | ILField of Infos.ILFieldInfo
    | RecdOrClass of TypedTree.RecdFieldRef
    | Union of TypedTree.UnionCaseRef * int
    with
      member TryDeclaringTyconRef: TypedTree.TyconRef option
      member
        TryRecdField: Choice<TypedTree.RecdField,Infos.ILFieldInfo,
                              (TypedTree.AnonRecdTypeInfo * TypedTree.TTypes *
                               int * Range.range)>
  
  [<ClassAttribute>]
  and FSharpAnonRecordTypeDetails =

      new: cenv:SymbolEnv * anonInfo:TypedTree.AnonRecdTypeInfo ->
              FSharpAnonRecordTypeDetails
      member Assembly: FSharpAssembly
      member CompiledName: string
      member EnclosingCompiledTypeNames: string list
      member SortedFieldNames: string []
  
  [<ClassAttribute>]
  and FSharpField =

      inherit FSharpSymbol
      new: SymbolEnv * TypedTree.RecdFieldRef -> FSharpField
      new: SymbolEnv * TypedTree.UnionCaseRef * int -> FSharpField
      new: cenv:SymbolEnv * d:FSharpFieldData -> FSharpField
      override Equals: other:obj -> bool
      override GetHashCode: unit -> int
      override ToString: unit -> string
      member Accessibility: FSharpAccessibility
      member
        AnonRecordFieldDetails: FSharpAnonRecordTypeDetails * FSharpType [] *
                                 int
      member DeclarationLocation: Range.range
      member DeclaringEntity: FSharpEntity option
      member DeclaringUnionCase: FSharpUnionCase option
      member ElaboratedXmlDoc: System.Collections.Generic.IList<string>
      member FieldAttributes: System.Collections.Generic.IList<FSharpAttribute>
      member FieldType: FSharpType
      member IsAnonRecordField: bool
      member IsCompilerGenerated: bool
      member IsDefaultValue: bool
      member IsLiteral: bool
      member IsMutable: bool
      member IsNameGenerated: bool
      member IsStatic: bool
      member IsUnionCaseField: bool
      member IsUnresolved: bool
      member IsVolatile: bool
      member LiteralValue: obj option
      member Name: string
      member
        PropertyAttributes: System.Collections.Generic.IList<FSharpAttribute>
      member private V: FSharpFieldData
      member XmlDoc: System.Collections.Generic.IList<string>
      member XmlDocSig: string
  
  [<System.Obsolete ("Renamed to FSharpField")>]
  and FSharpRecordField = FSharpField
  [<ClassAttribute>]
  and FSharpAccessibilityRights =

      new: TypedTree.CcuThunk * AccessibilityLogic.AccessorDomain ->
              FSharpAccessibilityRights
      member internal Contents: AccessibilityLogic.AccessorDomain
      member internal ThisCcu: TypedTree.CcuThunk
  
  [<ClassAttribute>]
  and FSharpActivePatternCase =

      inherit FSharpSymbol
      new: cenv:SymbolEnv * apinfo:PrettyNaming.ActivePatternInfo *
            ty:TypedTree.TType * n:int * valOpt:TypedTree.ValRef option *
            item:NameResolution.Item -> FSharpActivePatternCase
      member DeclarationLocation: Range.range
      member ElaboratedXmlDoc: System.Collections.Generic.IList<string>
      member Group: FSharpActivePatternGroup
      member Index: int
      member Name: string
      member XmlDoc: System.Collections.Generic.IList<string>
      member XmlDocSig: string
  
  [<ClassAttribute>]
  and FSharpActivePatternGroup =

      new: cenv:SymbolEnv * apinfo:PrettyNaming.ActivePatternInfo *
            ty:TypedTree.TType * valOpt:TypedTree.ValRef option ->
              FSharpActivePatternGroup
      member DeclaringEntity: FSharpEntity option
      member IsTotal: bool
      member Name: string option
      member Names: System.Collections.Generic.IList<string>
      member OverallType: FSharpType
  
  [<ClassAttribute>]
  and FSharpGenericParameter =

      inherit FSharpSymbol
      new: SymbolEnv * TypedTree.Typar -> FSharpGenericParameter
      override Equals: other:obj -> bool
      override GetHashCode: unit -> int
      override ToString: unit -> string
      member Attributes: System.Collections.Generic.IList<FSharpAttribute>
      member
        Constraints: System.Collections.Generic.IList<FSharpGenericParameterConstraint>
      member DeclarationLocation: Range.range
      member ElaboratedXmlDoc: System.Collections.Generic.IList<string>
      member IsCompilerGenerated: bool
      member IsMeasure: bool
      member IsSolveAtCompileTime: bool
      member Name: string
      member internal V: TypedTree.Typar
      member XmlDoc: System.Collections.Generic.IList<string>
  
  [<ClassAttribute>]
  and FSharpDelegateSignature =

      new: cenv:SymbolEnv * info:TypedTree.SlotSig -> FSharpDelegateSignature
      override ToString: unit -> string
      member
        DelegateArguments: System.Collections.Generic.IList<string option *
                                                             FSharpType>
      member DelegateReturnType: FSharpType
  
  [<ClassAttribute>]
  and FSharpAbstractParameter =

      new: cenv:SymbolEnv * info:TypedTree.SlotParam -> FSharpAbstractParameter
      member Attributes: System.Collections.Generic.IList<FSharpAttribute>
      member IsInArg: bool
      member IsOptionalArg: bool
      member IsOutArg: bool
      member Name: string option
      member Type: FSharpType
  
  [<ClassAttribute>]
  and FSharpAbstractSignature =

      new: SymbolEnv * TypedTree.SlotSig -> FSharpAbstractSignature
      member
        AbstractArguments: System.Collections.Generic.IList<System.Collections.Generic.IList<FSharpAbstractParameter>>
      member AbstractReturnType: FSharpType
      member DeclaringType: FSharpType
      member
        DeclaringTypeGenericParameters: System.Collections.Generic.IList<FSharpGenericParameter>
      member
        MethodGenericParameters: System.Collections.Generic.IList<FSharpGenericParameter>
      member Name: string
  
  [<ClassAttribute (); NoEquality; NoComparison>]
  and FSharpGenericParameterMemberConstraint =

      new: cenv:SymbolEnv * info:TypedTree.TraitConstraintInfo ->
              FSharpGenericParameterMemberConstraint
      override ToString: unit -> string
      member MemberArgumentTypes: System.Collections.Generic.IList<FSharpType>
      member MemberIsStatic: bool
      member MemberName: string
      member MemberReturnType: FSharpType
      member MemberSources: System.Collections.Generic.IList<FSharpType>
  
  [<ClassAttribute (); NoEquality; NoComparison>]
  and FSharpGenericParameterDelegateConstraint =

      new: cenv:SymbolEnv * tupledArgTy:TypedTree.TType * rty:TypedTree.TType ->
              FSharpGenericParameterDelegateConstraint
      override ToString: unit -> string
      member DelegateReturnType: FSharpType
      member DelegateTupledArgumentType: FSharpType
  
  [<ClassAttribute (); NoEquality; NoComparison>]
  and FSharpGenericParameterDefaultsToConstraint =

      new: cenv:SymbolEnv * pri:int * ty:TypedTree.TType ->
              FSharpGenericParameterDefaultsToConstraint
      override ToString: unit -> string
      member DefaultsToPriority: int
      member DefaultsToTarget: FSharpType
  
  [<ClassAttribute (); NoEquality; NoComparison>]
  and FSharpGenericParameterConstraint =

      new: cenv:SymbolEnv * cx:TypedTree.TyparConstraint ->
              FSharpGenericParameterConstraint
      override ToString: unit -> string
      member CoercesToTarget: FSharpType
      member
        DefaultsToConstraintData: FSharpGenericParameterDefaultsToConstraint
      member DelegateConstraintData: FSharpGenericParameterDelegateConstraint
      member EnumConstraintTarget: FSharpType
      member IsCoercesToConstraint: bool
      member IsComparisonConstraint: bool
      member IsDefaultsToConstraint: bool
      member IsDelegateConstraint: bool
      member IsEnumConstraint: bool
      member IsEqualityConstraint: bool
      member IsMemberConstraint: bool
      member IsNonNullableValueTypeConstraint: bool
      member IsReferenceTypeConstraint: bool
      member IsRequiresDefaultConstructorConstraint: bool
      member IsSimpleChoiceConstraint: bool
      member IsSupportsNullConstraint: bool
      member IsUnmanagedConstraint: bool
      member MemberConstraintData: FSharpGenericParameterMemberConstraint
      member SimpleChoices: System.Collections.Generic.IList<FSharpType>
  
  [<RequireQualifiedAccessAttribute>]
  and FSharpInlineAnnotation =
    | PseudoValue
    | AlwaysInline
    | OptionalInline
    | NeverInline
    | AggressiveInline
  and FSharpMemberOrValData =
    | E of Infos.EventInfo
    | P of Infos.PropInfo
    | M of Infos.MethInfo
    | C of Infos.MethInfo
    | V of TypedTree.ValRef
  and FSharpMemberOrVal = FSharpMemberOrFunctionOrValue
  and FSharpMemberFunctionOrValue = FSharpMemberOrFunctionOrValue
  [<ClassAttribute>]
  and FSharpMemberOrFunctionOrValue =

      inherit FSharpSymbol
      new: SymbolEnv * Infos.MethInfo -> FSharpMemberOrFunctionOrValue
      new: SymbolEnv * TypedTree.ValRef -> FSharpMemberOrFunctionOrValue
      new: cenv:SymbolEnv * d:FSharpMemberOrValData * item:NameResolution.Item ->
              FSharpMemberOrFunctionOrValue
      override Equals: other:obj -> bool
      member
        FormatLayout: context:FSharpDisplayContext ->
                         Internal.Utilities.StructuredFormat.Layout
      override GetHashCode: unit -> int
      member
        GetWitnessPassingInfo: unit ->
                                  (string *
                                   System.Collections.Generic.IList<FSharpParameter>) option
      member
        Overloads: bool ->
                      System.Collections.Generic.IList<FSharpMemberOrFunctionOrValue> option
      override ToString: unit -> string
      member Accessibility: FSharpAccessibility
      member ApparentEnclosingEntity: FSharpEntity
      member Attributes: System.Collections.Generic.IList<FSharpAttribute>
      member CompiledName: string
      member
        CurriedParameterGroups: System.Collections.Generic.IList<System.Collections.Generic.IList<FSharpParameter>>
      member Data: FSharpMemberOrValData
      member DeclarationLocation: Range.range
      member DeclarationLocationOpt: Range.range option
      member DeclaringEntity: FSharpEntity option
      member DisplayName: string
      member ElaboratedXmlDoc: System.Collections.Generic.IList<string>
      member EventAddMethod: FSharpMemberOrFunctionOrValue
      member EventDelegateType: FSharpType
      member EventForFSharpProperty: FSharpMemberOrFunctionOrValue option
      member EventIsStandard: bool
      member EventRemoveMethod: FSharpMemberOrFunctionOrValue
      member FullType: FSharpType
      member
        GenericParameters: System.Collections.Generic.IList<FSharpGenericParameter>
      member GetterMethod: FSharpMemberOrFunctionOrValue
      member HasGetterMethod: bool
      member HasSetterMethod: bool
      member
        ImplementedAbstractSignatures: System.Collections.Generic.IList<FSharpAbstractSignature>
      member InlineAnnotation: FSharpInlineAnnotation
      member IsActivePattern: bool
      member IsBaseValue: bool
      member IsCompilerGenerated: bool
      member IsConstructor: bool
      member IsConstructorThisValue: bool
      member IsDispatchSlot: bool
      member IsEvent: bool
      member IsEventAddMethod: bool
      member IsEventRemoveMethod: bool
      member IsExplicitInterfaceImplementation: bool
      member IsExtensionMember: bool
      [<System.Obsolete
        ("Renamed to IsPropertyGetterMethod, which returns 'true' only for method symbols, not for property symbols")>]
      member IsGetterMethod: bool
      member IsImplicitConstructor: bool
      member IsInstanceMember: bool
      member IsInstanceMemberInCompiledCode: bool
      member IsMember: bool
      member IsMemberThisValue: bool
      member IsModuleValueOrMember: bool
      member IsMutable: bool
      member IsOverrideOrExplicitInterfaceImplementation: bool
      [<System.Obsolete
        ("Renamed to IsOverrideOrExplicitInterfaceImplementation")>]
      member IsOverrideOrExplicitMember: bool
      member IsProperty: bool
      member IsPropertyGetterMethod: bool
      member IsPropertySetterMethod: bool
      [<System.Obsolete
        ("Renamed to IsPropertySetterMethod, which returns 'true' only for method symbols, not for property symbols")>]
      member IsSetterMethod: bool
      member IsTypeFunction: bool
      member IsUnresolved: bool
      member IsValCompiledAsMethod: bool
      member IsValue: bool
      member LiteralValue: obj option
      member LogicalName: string
      member ReturnParameter: FSharpParameter
      member SetterMethod: FSharpMemberOrFunctionOrValue
      member XmlDoc: System.Collections.Generic.IList<string>
      member XmlDocSig: string
  
  [<ClassAttribute>]
  and FSharpType =

      new: g:TcGlobals.TcGlobals * thisCcu:TypedTree.CcuThunk *
            thisCcuTyp:TypedTree.ModuleOrNamespaceType *
            tcImports:CompilerImports.TcImports * ty:TypedTree.TType ->
              FSharpType
      new: SymbolEnv * ty:TypedTree.TType -> FSharpType
      static member
        Prettify: parameters:System.Collections.Generic.IList<System.Collections.Generic.IList<FSharpParameter>> ->
                     System.Collections.Generic.IList<System.Collections.Generic.IList<FSharpParameter>>
      static member
        Prettify: parameters:System.Collections.Generic.IList<FSharpParameter> ->
                     System.Collections.Generic.IList<FSharpParameter>
      static member Prettify: parameter:FSharpParameter -> FSharpParameter
      static member
        Prettify: types:System.Collections.Generic.IList<FSharpType> ->
                     System.Collections.Generic.IList<FSharpType>
      static member Prettify: ty:FSharpType -> FSharpType
      static member
        Prettify: parameters:System.Collections.Generic.IList<System.Collections.Generic.IList<FSharpParameter>> *
                   returnParameter:FSharpParameter ->
                     System.Collections.Generic.IList<System.Collections.Generic.IList<FSharpParameter>> *
                     FSharpParameter
      member private AdjustType: t:TypedTree.TType -> FSharpType
      override Equals: other:obj -> bool
      member Format: context:FSharpDisplayContext -> string
      member
        FormatLayout: context:FSharpDisplayContext ->
                         Internal.Utilities.StructuredFormat.Layout
      override GetHashCode: unit -> int
      member
        Instantiate: (FSharpGenericParameter * FSharpType) list -> FSharpType
      override ToString: unit -> string
      member AbbreviatedType: FSharpType
      member AllInterfaces: System.Collections.Generic.IList<FSharpType>
      member AnonRecordTypeDetails: FSharpAnonRecordTypeDetails
      member BaseType: FSharpType option
      member GenericArguments: System.Collections.Generic.IList<FSharpType>
      member GenericParameter: FSharpGenericParameter
      member HasTypeDefinition: bool
      member IsAbbreviation: bool
      member IsAnonRecordType: bool
      member IsFunctionType: bool
      member IsGenericParameter: bool
      [<System.Obsolete ("Renamed to HasTypeDefinition")>]
      member IsNamedType: bool
      member IsStructTupleType: bool
      member IsTupleType: bool
      member IsUnresolved: bool
      [<System.Obsolete ("Renamed to TypeDefinition")>]
      member NamedEntity: FSharpEntity
      member TypeDefinition: FSharpEntity
      member private V: TypedTree.TType
      member private cenv: SymbolEnv
  
  [<ClassAttribute>]
  and FSharpAttribute =

      new: cenv:SymbolEnv * attrib:AttributeChecking.AttribInfo ->
              FSharpAttribute
      member Format: context:FSharpDisplayContext -> string
      override ToString: unit -> string
      member AttributeType: FSharpEntity
      member
        ConstructorArguments: System.Collections.Generic.IList<FSharpType * obj>
      member IsUnresolved: bool
      member
        NamedArguments: System.Collections.Generic.IList<FSharpType * string *
                                                          bool * obj>
      member Range: Range.range
  
  [<ClassAttribute>]
  and FSharpStaticParameter =

      inherit FSharpSymbol
      new: cenv:SymbolEnv * sp:Tainted<ExtensionTyping.ProvidedParameterInfo> *
            m:Range.range -> FSharpStaticParameter
      override Equals: other:obj -> bool
      override GetHashCode: unit -> int
      override ToString: unit -> string
      member DeclarationLocation: Range.range
      member DefaultValue: obj
      [<System.Obsolete
        ("This member is no longer used, use IsOptional instead")>]
      member HasDefaultValue: bool
      member IsOptional: bool
      member Kind: FSharpType
      member Name: string
      member Range: Range.range
  
  [<ClassAttribute>]
  and FSharpParameter =

      inherit FSharpSymbol
      new: cenv:SymbolEnv * ty:TypedTree.TType * argInfo:TypedTree.ArgReprInfo *
            ownerRangeOpt:Range.range option -> FSharpParameter
      new: cenv:SymbolEnv * id:SyntaxTree.Ident * ty:TypedTree.TType *
            container:NameResolution.ArgumentContainer option -> FSharpParameter
      new: cenv:SymbolEnv * paramTy:TypedTree.TType *
            topArgInfo:TypedTree.ArgReprInfo *
            ownerOpt:NameResolution.ArgumentContainer option *
            ownerRangeOpt:Range.range option * isParamArrayArg:bool *
            isInArg:bool * isOutArg:bool * isOptionalArg:bool *
            isWitnessArg:bool -> FSharpParameter
      member AdjustType: ty:TypedTree.TType -> FSharpParameter
      override Equals: other:obj -> bool
      override GetHashCode: unit -> int
      override ToString: unit -> string
      member Attributes: System.Collections.Generic.IList<FSharpAttribute>
      member DeclarationLocation: Range.range
      member IsInArg: bool
      member IsOptionalArg: bool
      member IsOutArg: bool
      member IsParamArrayArg: bool
      member IsWitnessArg: bool
      member Name: string option
      member Owner: FSharpSymbol option
      member Type: FSharpType
      member V: TypedTree.TType
      member private ValReprInfo: TypedTree.ArgReprInfo
      member cenv: SymbolEnv
  
  and FSharpAssemblySignature =

      new: tcGlobals:TcGlobals.TcGlobals * thisCcu:TypedTree.CcuThunk *
            thisCcuTyp:TypedTree.ModuleOrNamespaceType *
            tcImports:CompilerImports.TcImports *
            topAttribs:CheckDeclarations.TopAttribs option *
            contents:TypedTree.ModuleOrNamespaceType -> FSharpAssemblySignature
      new: cenv:SymbolEnv * ccu:TypedTree.CcuThunk -> FSharpAssemblySignature
      new: cenv:SymbolEnv * topAttribs:CheckDeclarations.TopAttribs option *
            optViewedCcu:TypedTree.CcuThunk option *
            mtyp:TypedTree.ModuleOrNamespaceType -> FSharpAssemblySignature
      member FindEntityByPath: string list -> FSharpEntity option
      override ToString: unit -> string
      member Attributes: System.Collections.Generic.IList<FSharpAttribute>
      member Entities: System.Collections.Generic.IList<FSharpEntity>
  
  and FSharpAssembly =

      new: tcGlobals:TcGlobals.TcGlobals * tcImports:CompilerImports.TcImports *
            ccu:TypedTree.CcuThunk -> FSharpAssembly
      internal new: cenv:SymbolEnv * ccu:TypedTree.CcuThunk -> FSharpAssembly
      override ToString: unit -> string
      [<System.Obsolete ("This item is obsolete, it is not useful")>]
      member CodeLocation: string
      member Contents: FSharpAssemblySignature
      member FileName: string option
      member IsProviderGenerated: bool
      member QualifiedName: string
      member RawCcuThunk: TypedTree.CcuThunk
      member SimpleName: string
  
  [<SealedAttribute>]
  and FSharpOpenDeclaration =

      new: target:SyntaxTree.SynOpenDeclTarget * range:Range.range option *
            modules:FSharpEntity list * types:FSharpType list *
            appliedScope:Range.range * isOwnNamespace:bool ->
              FSharpOpenDeclaration
      member AppliedScope: Range.range
      member IsOwnNamespace: bool
      member LongId: SyntaxTree.LongIdent
      member Modules: FSharpEntity list
      member Range: Range.range option
      member Target: SyntaxTree.SynOpenDeclTarget
      member Types: FSharpType list
  
  [<SealedAttribute>]
  and FSharpSymbolUse =

      new: g:TcGlobals.TcGlobals * denv:TypedTreeOps.DisplayEnv *
            symbol:FSharpSymbol * itemOcc:NameResolution.ItemOccurence *
            range:Range.range -> FSharpSymbolUse
      override ToString: unit -> string
      member DisplayContext: FSharpDisplayContext
      member FileName: string
      member IsDefinition: bool
      member IsFromAttribute: bool
      member IsFromComputationExpression: bool
      member IsFromDefinition: bool
      member IsFromDispatchSlotImplementation: bool
      member IsFromOpenStatement: bool
      member IsFromPattern: bool
      member IsFromType: bool
      member IsPrivateToFile: bool
      member Range: Range.Range01
      member RangeAlternate: Range.range
      member Symbol: FSharpSymbol
  
  module Impl =
    val protect: f:(unit -> 'a) -> 'a
    val makeReadOnlyCollection:
      arr:seq<'T> -> System.Collections.Generic.IList<'T>
    val makeXmlDoc:
      doc:XmlDoc.XmlDoc -> System.Collections.Generic.IList<string>
    val makeElaboratedXmlDoc:
      doc:XmlDoc.XmlDoc -> System.Collections.Generic.IList<string>
    val rescopeEntity:
      optViewedCcu:TypedTree.CcuThunk option ->
        entity:TypedTree.Entity -> TypedTree.EntityRef
    val entityIsUnresolved: entity:TypedTree.EntityRef -> bool
    val checkEntityIsResolved: entity:TypedTree.EntityRef -> unit
    val checkForCrossProjectAccessibility:
      ilg:AbstractIL.IL.ILGlobals ->
        thisCcu2:TypedTree.CcuThunk * ad2:AccessibilityLogic.AccessorDomain ->
          thisCcu1:TypedTree.CcuThunk * taccess1:TypedTree.Accessibility -> bool
    val getApproxFSharpAccessibilityOfMember:
      declaringEntity:TypedTree.EntityRef ->
        ilAccess:AbstractIL.IL.ILMemberAccess -> TypedTree.Accessibility
    val getApproxFSharpAccessibilityOfEntity:
      entity:TypedTree.EntityRef -> TypedTree.Accessibility
    val getLiteralValue: _arg6:TypedTree.Const option -> obj option
    val getXmlDocSigForEntity:
      cenv:SymbolEnv -> ent:TypedTree.EntityRef -> string


namespace FSharp.Compiler.SourceCodeServices
  module ExprTranslationImpl =
    val nonNil: x:'a list -> bool
    type ExprTranslationEnv =
      { vs: TypedTreeOps.ValMap<unit>
        tyvs: TypedTree.StampMap<FSharpGenericParameter>
        isinstVals: TypedTreeOps.ValMap<TypedTree.TType * TypedTree.Expr>
        substVals: TypedTreeOps.ValMap<TypedTree.Expr>
        suppressWitnesses: bool
        witnessesInScope: TypedTreeOps.TraitWitnessInfoHashMap<int> }
      with
        static member Empty: g:TcGlobals.TcGlobals -> ExprTranslationEnv
        member
          BindCurriedVals: vsl:TypedTree.Val list list -> ExprTranslationEnv
        member
          BindIsInstVal: v:TypedTree.Val ->
                            ty:TypedTree.TType * e:TypedTree.Expr ->
                              ExprTranslationEnv
        member
          BindSubstVal: v:TypedTree.Val ->
                           e:TypedTree.Expr -> ExprTranslationEnv
        member
          BindTypar: v:TypedTree.Typar * gp:FSharpGenericParameter ->
                        ExprTranslationEnv
        member
          BindTypars: vs:(TypedTree.Typar * #FSharpGenericParameter) list ->
                         ExprTranslationEnv
        member BindVal: v:TypedTree.Val -> ExprTranslationEnv
        member BindVals: vs:TypedTree.Val list -> ExprTranslationEnv
    
    exception IgnoringPartOfQuotedTermWarning of string * Range.range
    val wfail: msg:string * m:Range.range -> 'a

  type E =
    | Value of FSharpMemberOrFunctionOrValue
    | ThisValue of FSharpType
    | BaseValue of FSharpType
    | Application of FSharpExpr * FSharpType list * FSharpExpr list
    | Lambda of FSharpMemberOrFunctionOrValue * FSharpExpr
    | TypeLambda of FSharpGenericParameter list * FSharpExpr
    | Quote of FSharpExpr
    | IfThenElse of FSharpExpr * FSharpExpr * FSharpExpr
    | DecisionTree of
      FSharpExpr * (FSharpMemberOrFunctionOrValue list * FSharpExpr) list
    | DecisionTreeSuccess of int * FSharpExpr list
    | Call of
      FSharpExpr option * FSharpMemberOrFunctionOrValue * FSharpType list *
      FSharpType list * FSharpExpr list * FSharpExpr list
    | NewObject of
      FSharpMemberOrFunctionOrValue * FSharpType list * FSharpExpr list
    | LetRec of (FSharpMemberOrFunctionOrValue * FSharpExpr) list * FSharpExpr
    | Let of (FSharpMemberOrFunctionOrValue * FSharpExpr) * FSharpExpr
    | NewRecord of FSharpType * FSharpExpr list
    | ObjectExpr of
      FSharpType * FSharpExpr * FSharpObjectExprOverride list *
      (FSharpType * FSharpObjectExprOverride list) list
    | FSharpFieldGet of FSharpExpr option * FSharpType * FSharpField
    | FSharpFieldSet of
      FSharpExpr option * FSharpType * FSharpField * FSharpExpr
    | NewUnionCase of FSharpType * FSharpUnionCase * FSharpExpr list
    | NewAnonRecord of FSharpType * FSharpExpr list
    | AnonRecordGet of FSharpExpr * FSharpType * int
    | UnionCaseGet of FSharpExpr * FSharpType * FSharpUnionCase * FSharpField
    | UnionCaseSet of
      FSharpExpr * FSharpType * FSharpUnionCase * FSharpField * FSharpExpr
    | UnionCaseTag of FSharpExpr * FSharpType
    | UnionCaseTest of FSharpExpr * FSharpType * FSharpUnionCase
    | TraitCall of
      FSharpType list * string * SyntaxTree.MemberFlags * FSharpType list *
      FSharpType list * FSharpExpr list
    | NewTuple of FSharpType * FSharpExpr list
    | TupleGet of FSharpType * int * FSharpExpr
    | Coerce of FSharpType * FSharpExpr
    | NewArray of FSharpType * FSharpExpr list
    | TypeTest of FSharpType * FSharpExpr
    | AddressSet of FSharpExpr * FSharpExpr
    | ValueSet of FSharpMemberOrFunctionOrValue * FSharpExpr
    | Unused
    | DefaultValue of FSharpType
    | Const of obj * FSharpType
    | AddressOf of FSharpExpr
    | Sequential of FSharpExpr * FSharpExpr
    | FastIntegerForLoop of FSharpExpr * FSharpExpr * FSharpExpr * bool
    | WhileLoop of FSharpExpr * FSharpExpr
    | TryFinally of FSharpExpr * FSharpExpr
    | TryWith of
      FSharpExpr * FSharpMemberOrFunctionOrValue * FSharpExpr *
      FSharpMemberOrFunctionOrValue * FSharpExpr
    | NewDelegate of FSharpType * FSharpExpr
    | ILFieldGet of FSharpExpr option * FSharpType * string
    | ILFieldSet of FSharpExpr option * FSharpType * string * FSharpExpr
    | ILAsm of string * FSharpType list * FSharpExpr list
    | WitnessArg of int
  [<SealedAttribute>]
  and FSharpObjectExprOverride =

      new: sgn:FSharpAbstractSignature * gps:FSharpGenericParameter list *
            args:FSharpMemberOrFunctionOrValue list list * body:FSharpExpr ->
              FSharpObjectExprOverride
      member Body: FSharpExpr
      member CurriedParameterGroups: FSharpMemberOrFunctionOrValue list list
      member GenericParameters: FSharpGenericParameter list
      member Signature: FSharpAbstractSignature
  
  [<SealedAttribute>]
  and FSharpExpr =

      new: cenv:SymbolEnv * f:(unit -> FSharpExpr) option * e:E * m:Range.range *
            ty:TypedTree.TType -> FSharpExpr
      override ToString: unit -> string
      member E: E
      member ImmediateSubExpressions: FSharpExpr list
      member Range: Range.range
      member Type: FSharpType
      member cenv: SymbolEnv
  
  module FSharpExprConvert =
    val IsStaticInitializationField: rfref:TypedTree.RecdFieldRef -> bool
    val ( |StaticInitializationCheck|_| ): e:TypedTree.Expr -> unit option
    val ( |StaticInitializationCount|_| ): e:TypedTree.Expr -> unit option
    val ( |ILUnaryOp|_| ):
      e:AbstractIL.IL.ILInstr ->
        (#TcGlobals.TcGlobals -> Range.range -> TypedTree.TType ->
           TypedTree.Expr -> TypedTree.Expr) option
    val ( |ILMulDivOp|_| ):
      e:AbstractIL.IL.ILInstr ->
        ((#TcGlobals.TcGlobals -> Range.range -> TypedTree.TType ->
            TypedTree.TType -> TypedTree.TType -> TypedTree.Expr ->
            TypedTree.Expr -> TypedTree.Expr) * bool) option
    val ( |ILBinaryOp|_| ):
      e:AbstractIL.IL.ILInstr ->
        (#TcGlobals.TcGlobals -> Range.range -> TypedTree.TType ->
           TypedTree.Expr -> TypedTree.Expr -> TypedTree.Expr) option
    val ( |ILConvertOp|_| ):
      e:AbstractIL.IL.ILInstr ->
        (#TcGlobals.TcGlobals -> Range.range -> TypedTree.TType ->
           TypedTree.Expr -> TypedTree.Expr) option
    val ( |TTypeConvOp|_| ):
      cenv:SymbolEnv ->
        ty:TypedTree.TType ->
          (#TcGlobals.TcGlobals -> Range.range -> TypedTree.TType ->
             TypedTree.Expr -> TypedTree.Expr) option
    val ConvType: cenv:SymbolEnv -> ty:TypedTree.TType -> FSharpType
    val ConvTypes:
      cenv:SymbolEnv -> tys:TypedTree.TType list -> FSharpType list
    val ConvILTypeRefApp:
      cenv:SymbolEnv ->
        m:Range.range ->
          tref:AbstractIL.IL.ILTypeRef ->
            tyargs:TypedTree.TypeInst -> FSharpType
    val ConvUnionCaseRef:
      cenv:SymbolEnv -> ucref:TypedTree.UnionCaseRef -> FSharpUnionCase
    val ConvRecdFieldRef:
      cenv:SymbolEnv -> rfref:TypedTree.RecdFieldRef -> FSharpField
    val exprOfExprAddr: cenv:SymbolEnv -> expr:TypedTree.Expr -> TypedTree.Expr
    val Mk:
      cenv:SymbolEnv -> m:Range.range -> ty:TypedTree.TType -> e:E -> FSharpExpr
    val Mk2: cenv:SymbolEnv -> orig:TypedTree.Expr -> e:E -> FSharpExpr
    val ConvLValueExpr:
      cenv:SymbolEnv ->
        env:ExprTranslationImpl.ExprTranslationEnv ->
          expr:TypedTree.Expr -> FSharpExpr
    val ConvExpr:
      cenv:SymbolEnv ->
        env:ExprTranslationImpl.ExprTranslationEnv ->
          expr:TypedTree.Expr -> FSharpExpr
    val ConvExprLinear:
      cenv:SymbolEnv ->
        env:ExprTranslationImpl.ExprTranslationEnv ->
          expr:TypedTree.Expr -> contF:(FSharpExpr -> E) -> E
    val ConvExprPrimLinear:
      cenv:SymbolEnv ->
        env:ExprTranslationImpl.ExprTranslationEnv ->
          expr:TypedTree.Expr -> contF:(E -> E) -> E
    val ConvModuleValueOrMemberUseLinear:
      cenv:SymbolEnv ->
        env:ExprTranslationImpl.ExprTranslationEnv ->
          expr:TypedTree.Expr * vref:TypedTree.ValRef *
          vFlags:TypedTree.ValUseFlag * tyargs:TypedTree.TypeInst *
          curriedArgs:TypedTree.Expr list -> contF:(E -> E) -> E
    val GetWitnessArgs:
      cenv:SymbolEnv ->
        env:ExprTranslationImpl.ExprTranslationEnv ->
          vref:TypedTree.ValRef ->
            m:Range.range ->
              tps:TypedTree.Typars ->
                tyargs:TypedTree.TypeInst -> FSharpExpr list
    val ConvExprPrim:
      cenv:SymbolEnv ->
        env:ExprTranslationImpl.ExprTranslationEnv -> expr:TypedTree.Expr -> E
    val ConvWitnessInfoPrim:
      _cenv:SymbolEnv ->
        env:ExprTranslationImpl.ExprTranslationEnv ->
          traitInfo:TypedTree.TraitConstraintInfo -> E
    val ConvWitnessInfo:
      cenv:SymbolEnv ->
        env:ExprTranslationImpl.ExprTranslationEnv ->
          m:Range.range -> traitInfo:TypedTree.TraitConstraintInfo -> FSharpExpr
    val ConvLetBind:
      cenv:SymbolEnv ->
        env:ExprTranslationImpl.ExprTranslationEnv ->
          bind:TypedTree.Binding ->
            (FSharpMemberOrFunctionOrValue * FSharpExpr) option *
            ExprTranslationImpl.ExprTranslationEnv
    val ConvILCall:
      cenv:SymbolEnv ->
        env:ExprTranslationImpl.ExprTranslationEnv ->
          isNewObj:bool * valUseFlags:TypedTree.ValUseFlag *
          ilMethRef:AbstractIL.IL.ILMethodRef * enclTypeArgs:TypedTree.TypeInst *
          methTypeArgs:TypedTree.TypeInst * callArgs:TypedTree.Exprs *
          m:Range.range -> E
    val ConvObjectModelCallLinear:
      cenv:SymbolEnv ->
        env:ExprTranslationImpl.ExprTranslationEnv ->
          isNewObj:bool * v:FSharpMemberOrFunctionOrValue *
          enclTyArgs:TypedTree.TType list * methTyArgs:TypedTree.TType list *
          witnessArgsR:FSharpExpr list * callArgs:TypedTree.Expr list ->
            contF:(E -> E) -> E
    val ConvExprs:
      cenv:SymbolEnv ->
        env:ExprTranslationImpl.ExprTranslationEnv ->
          args:TypedTree.Exprs -> FSharpExpr list
    val ConvExprsLinear:
      cenv:SymbolEnv ->
        env:ExprTranslationImpl.ExprTranslationEnv ->
          args:TypedTree.Expr list -> contF:(FSharpExpr list -> E) -> E
    val ConvTargetsLinear:
      cenv:SymbolEnv ->
        env:ExprTranslationImpl.ExprTranslationEnv ->
          tgs:TypedTree.DecisionTreeTarget list ->
            contF:((FSharpMemberOrFunctionOrValue list * FSharpExpr) list -> E) ->
              E
    val ConvValRef:
      cenv:SymbolEnv ->
        env:ExprTranslationImpl.ExprTranslationEnv ->
          m:Range.range -> vref:TypedTree.ValRef -> E
    val ConvVal:
      cenv:SymbolEnv -> v:TypedTree.Val -> FSharpMemberOrFunctionOrValue
    val ConvConst:
      cenv:SymbolEnv ->
        env:ExprTranslationImpl.ExprTranslationEnv ->
          m:Range.range -> c:TypedTree.Const -> ty:TypedTree.TType -> E
    val ConvDecisionTree:
      cenv:SymbolEnv ->
        env:ExprTranslationImpl.ExprTranslationEnv ->
          dtreeRetTy:TypedTree.TType ->
            x:TypedTree.DecisionTree -> m:Range.range -> FSharpExpr
    val ConvDecisionTreePrim:
      cenv:SymbolEnv ->
        env:ExprTranslationImpl.ExprTranslationEnv ->
          dtreeRetTy:TypedTree.TType -> x:TypedTree.DecisionTree -> E
    val ConvExprOnDemand:
      cenv:SymbolEnv ->
        env:ExprTranslationImpl.ExprTranslationEnv ->
          expr:TypedTree.Expr -> FSharpExpr

  type FSharpAssemblyContents =

      new: tcGlobals:TcGlobals.TcGlobals * thisCcu:TypedTree.CcuThunk *
            thisCcuType:TypedTree.ModuleOrNamespaceType option *
            tcImports:CompilerImports.TcImports *
            mimpls:TypedTree.TypedImplFile list -> FSharpAssemblyContents
      new: cenv:SymbolEnv * mimpls:TypedTree.TypedImplFile list ->
              FSharpAssemblyContents
      member ImplementationFiles: FSharpImplementationFileContents list
  
  and FSharpImplementationFileDeclaration =
    | Entity of FSharpEntity * FSharpImplementationFileDeclaration list
    | MemberOrFunctionOrValue of
      FSharpMemberOrFunctionOrValue * FSharpMemberOrFunctionOrValue list list *
      FSharpExpr
    | InitAction of FSharpExpr
  and FSharpImplementationFileContents =

      new: cenv:SymbolEnv * mimpl:TypedTree.TypedImplFile ->
              FSharpImplementationFileContents
      member Declarations: FSharpImplementationFileDeclaration list
      member FileName: string
      member HasExplicitEntryPoint: bool
      member IsScript: bool
      member QualifiedName: string
  
  module BasicPatterns =
    val ( |Value|_| ): FSharpExpr -> FSharpMemberOrFunctionOrValue option
    val ( |Const|_| ): FSharpExpr -> (obj * FSharpType) option
    val ( |TypeLambda|_| ):
      FSharpExpr -> (FSharpGenericParameter list * FSharpExpr) option
    val ( |Lambda|_| ):
      FSharpExpr -> (FSharpMemberOrFunctionOrValue * FSharpExpr) option
    val ( |Application|_| ):
      FSharpExpr -> (FSharpExpr * FSharpType list * FSharpExpr list) option
    val ( |IfThenElse|_| ):
      FSharpExpr -> (FSharpExpr * FSharpExpr * FSharpExpr) option
    val ( |Let|_| ):
      FSharpExpr ->
        ((FSharpMemberOrFunctionOrValue * FSharpExpr) * FSharpExpr) option
    val ( |LetRec|_| ):
      FSharpExpr ->
        ((FSharpMemberOrFunctionOrValue * FSharpExpr) list * FSharpExpr) option
    val ( |NewRecord|_| ): FSharpExpr -> (FSharpType * FSharpExpr list) option
    val ( |NewAnonRecord|_| ):
      FSharpExpr -> (FSharpType * FSharpExpr list) option
    val ( |NewUnionCase|_| ):
      FSharpExpr -> (FSharpType * FSharpUnionCase * FSharpExpr list) option
    val ( |NewTuple|_| ): FSharpExpr -> (FSharpType * FSharpExpr list) option
    val ( |TupleGet|_| ): FSharpExpr -> (FSharpType * int * FSharpExpr) option
    val ( |Call|_| ):
      FSharpExpr ->
        (FSharpExpr option * FSharpMemberOrFunctionOrValue * FSharpType list *
         FSharpType list * FSharpExpr list) option
    val ( |CallWithWitnesses|_| ):
      FSharpExpr ->
        (FSharpExpr option * FSharpMemberOrFunctionOrValue * FSharpType list *
         FSharpType list * FSharpExpr list * FSharpExpr list) option
    val ( |NewObject|_| ):
      FSharpExpr ->
        (FSharpMemberOrFunctionOrValue * FSharpType list * FSharpExpr list) option
    val ( |FSharpFieldGet|_| ):
      FSharpExpr -> (FSharpExpr option * FSharpType * FSharpField) option
    val ( |AnonRecordGet|_| ):
      FSharpExpr -> (FSharpExpr * FSharpType * int) option
    val ( |FSharpFieldSet|_| ):
      FSharpExpr ->
        (FSharpExpr option * FSharpType * FSharpField * FSharpExpr) option
    val ( |UnionCaseGet|_| ):
      FSharpExpr ->
        (FSharpExpr * FSharpType * FSharpUnionCase * FSharpField) option
    val ( |UnionCaseTag|_| ): FSharpExpr -> (FSharpExpr * FSharpType) option
    val ( |UnionCaseTest|_| ):
      FSharpExpr -> (FSharpExpr * FSharpType * FSharpUnionCase) option
    val ( |NewArray|_| ): FSharpExpr -> (FSharpType * FSharpExpr list) option
    val ( |Coerce|_| ): FSharpExpr -> (FSharpType * FSharpExpr) option
    val ( |Quote|_| ): FSharpExpr -> FSharpExpr option
    val ( |TypeTest|_| ): FSharpExpr -> (FSharpType * FSharpExpr) option
    val ( |Sequential|_| ): FSharpExpr -> (FSharpExpr * FSharpExpr) option
    val ( |FastIntegerForLoop|_| ):
      FSharpExpr -> (FSharpExpr * FSharpExpr * FSharpExpr * bool) option
    val ( |WhileLoop|_| ): FSharpExpr -> (FSharpExpr * FSharpExpr) option
    val ( |TryFinally|_| ): FSharpExpr -> (FSharpExpr * FSharpExpr) option
    val ( |TryWith|_| ):
      FSharpExpr ->
        (FSharpExpr * FSharpMemberOrFunctionOrValue * FSharpExpr *
         FSharpMemberOrFunctionOrValue * FSharpExpr) option
    val ( |NewDelegate|_| ): FSharpExpr -> (FSharpType * FSharpExpr) option
    val ( |DefaultValue|_| ): FSharpExpr -> FSharpType option
    val ( |AddressSet|_| ): FSharpExpr -> (FSharpExpr * FSharpExpr) option
    val ( |ValueSet|_| ):
      FSharpExpr -> (FSharpMemberOrFunctionOrValue * FSharpExpr) option
    val ( |AddressOf|_| ): FSharpExpr -> FSharpExpr option
    val ( |ThisValue|_| ): FSharpExpr -> FSharpType option
    val ( |BaseValue|_| ): FSharpExpr -> FSharpType option
    val ( |ILAsm|_| ):
      FSharpExpr -> (string * FSharpType list * FSharpExpr list) option
    val ( |ILFieldGet|_| ):
      FSharpExpr -> (FSharpExpr option * FSharpType * string) option
    val ( |ILFieldSet|_| ):
      FSharpExpr ->
        (FSharpExpr option * FSharpType * string * FSharpExpr) option
    val ( |ObjectExpr|_| ):
      FSharpExpr ->
        (FSharpType * FSharpExpr * FSharpObjectExprOverride list *
         (FSharpType * FSharpObjectExprOverride list) list) option
    val ( |DecisionTree|_| ):
      FSharpExpr ->
        (FSharpExpr * (FSharpMemberOrFunctionOrValue list * FSharpExpr) list) option
    val ( |DecisionTreeSuccess|_| ):
      FSharpExpr -> (int * FSharpExpr list) option
    val ( |UnionCaseSet|_| ):
      FSharpExpr ->
        (FSharpExpr * FSharpType * FSharpUnionCase * FSharpField * FSharpExpr) option
    val ( |TraitCall|_| ):
      FSharpExpr ->
        (FSharpType list * string * SyntaxTree.MemberFlags * FSharpType list *
         FSharpType list * FSharpExpr list) option
    val ( |WitnessArg|_| ): FSharpExpr -> int option


namespace FSharp.Compiler.SourceCodeServices
  module Symbol =
    val isAttribute<'T>: FSharpAttribute -> bool
    val tryGetAttribute<'T>: seq<FSharpAttribute> -> FSharpAttribute option
    module Option =
      val attempt: f:(unit -> 'a) -> 'a option
  
    val hasModuleSuffixAttribute: FSharpEntity -> bool
    val isOperator: name:string -> bool
    val UnnamedUnionFieldRegex: System.Text.RegularExpressions.Regex
    val isUnnamedUnionCaseField: FSharpField -> bool
    val ( |AbbreviatedType|_| ): FSharpEntity -> FSharpType option
    val ( |TypeWithDefinition|_| ): FSharpType -> FSharpEntity option
    val getEntityAbbreviatedType:
      FSharpEntity -> FSharpEntity * FSharpType option
    val getAbbreviatedType: FSharpType -> FSharpType
    val ( |Attribute|_| ): FSharpEntity -> unit option
    val hasAttribute<'T>: seq<FSharpAttribute> -> bool
    val ( |ValueType|_| ): FSharpEntity -> unit option
    val ( |Class|_| ):
      original:FSharpEntity * abbreviated:FSharpEntity * 'a -> unit option
    val ( |Record|_| ): FSharpEntity -> unit option
    val ( |UnionType|_| ): FSharpEntity -> unit option
    val ( |Delegate|_| ): FSharpEntity -> unit option
    val ( |FSharpException|_| ): FSharpEntity -> unit option
    val ( |Interface|_| ): FSharpEntity -> unit option
    val ( |AbstractClass|_| ): FSharpEntity -> unit option
    val ( |FSharpType|_| ): FSharpEntity -> unit option
    val ( |ProvidedType|_| ): FSharpEntity -> unit option
    val ( |ByRef|_| ): FSharpEntity -> unit option
    val ( |Array|_| ): FSharpEntity -> unit option
    val ( |FSharpModule|_| ): FSharpEntity -> unit option
    val ( |Namespace|_| ): FSharpEntity -> unit option
    val ( |ProvidedAndErasedType|_| ): FSharpEntity -> unit option
    val ( |Enum|_| ): FSharpEntity -> unit option
    val ( |Tuple|_| ): FSharpType -> unit option
    val ( |RefCell|_| ): FSharpType -> unit option
    val ( |FunctionType|_| ): FSharpType -> unit option
    val ( |Pattern|_| ): FSharpSymbol -> unit option
    val ( |Field|_| ): FSharpSymbol -> (FSharpField * FSharpType) option
    val ( |MutableVar|_| ): FSharpSymbol -> unit option
    val ( |FSharpEntity|_| ):
      FSharpSymbol -> (FSharpEntity * FSharpEntity * FSharpType option) option
    val ( |Parameter|_| ): FSharpSymbol -> unit option
    val ( |UnionCase|_| ): FSharpSymbol -> FSharpUnionCase option
    val ( |RecordField|_| ): FSharpSymbol -> FSharpField option
    val ( |ActivePatternCase|_| ):
      FSharpSymbol -> FSharpActivePatternCase option
    val ( |MemberFunctionOrValue|_| ):
      FSharpSymbol -> FSharpMemberOrFunctionOrValue option
    val ( |Constructor|_| ):
      FSharpMemberOrFunctionOrValue -> FSharpEntity option
    val ( |Function|_| ):
      excluded:bool -> FSharpMemberOrFunctionOrValue -> unit option
    val ( |ExtensionMember|_| ): FSharpMemberOrFunctionOrValue -> unit option
    val ( |Event|_| ): FSharpMemberOrFunctionOrValue -> unit option


namespace FSharp.Compiler.SourceCodeServices
  type internal IReactorOperations =
    interface
      abstract member
        EnqueueAndAwaitOpAsync: userOpName:string * opName:string *
                                 opArg:string *
                                 action:(AbstractIL.Internal.Library.CompilationThreadToken ->
                                           AbstractIL.Internal.Library.Cancellable<'T>) ->
                                   Async<'T>
      abstract member
        EnqueueOp: userOpName:string * opName:string * opArg:string *
                    action:(AbstractIL.Internal.Library.CompilationThreadToken ->
                              unit) -> unit
  
  [<NoEquality; NoComparison>]
  type internal ReactorCommands =
    | SetBackgroundOp of
      (string * string * string *
       (AbstractIL.Internal.Library.CompilationThreadToken ->
          System.Threading.CancellationToken -> bool)) option
    | Op of
      userOpName: string * opName: string * opArg: string *
      System.Threading.CancellationToken *
      (AbstractIL.Internal.Library.CompilationThreadToken -> unit) *
      (unit -> unit)
    | WaitForBackgroundOpCompletion of AsyncReplyChannel<unit>
    | CompleteAllQueuedOps of AsyncReplyChannel<unit>
  [<SealedAttribute>]
  type Reactor =

      new: unit -> Reactor
      member CancelBackgroundOp: unit -> unit
      member CompleteAllQueuedOps: unit -> unit
      member
        EnqueueAndAwaitOpAsync: userOpName:string * opName:string *
                                 opArg:string *
                                 (AbstractIL.Internal.Library.CompilationThreadToken ->
                                    AbstractIL.Internal.Library.Cancellable<'T>) ->
                                   Async<'T>
      member
        EnqueueOp: userOpName:string * opName:string * opArg:string *
                    op:(AbstractIL.Internal.Library.CompilationThreadToken ->
                          unit) -> unit
      member
        EnqueueOpPrim: userOpName:string * opName:string * opArg:string *
                        ct:System.Threading.CancellationToken *
                        op:(AbstractIL.Internal.Library.CompilationThreadToken ->
                              unit) * ccont:(unit -> unit) -> unit
      member
        SetBackgroundOp: (string * string * string *
                           (AbstractIL.Internal.Library.CompilationThreadToken ->
                              System.Threading.CancellationToken -> bool)) option ->
                            unit
      member SetPreferredUILang: string option -> unit
      member WaitForBackgroundOpCompletion: unit -> unit
      member CurrentQueueLength: int
      member PauseBeforeBackgroundWork: int
      static member Singleton: Reactor
  

namespace FSharp.Compiler.SourceCodeServices
  [<RequireQualifiedAccessAttribute>]
  type SemanticClassificationType =
    | ReferenceType
    | ValueType
    | UnionCase
    | UnionCaseField
    | Function
    | Property
    | MutableVar
    | Module
    | Namespace
    | Printf
    | ComputationExpression
    | IntrinsicFunction
    | Enumeration
    | Interface
    | TypeArgument
    | Operator
    | DisposableType
    | DisposableTopLevelValue
    | DisposableLocalValue
    | Method
    | ExtensionMethod
    | ConstructorForReferenceType
    | ConstructorForValueType
    | Literal
    | RecordField
    | MutableRecordField
    | RecordFieldAsFunction
    | Exception
    | Field
    | Event
    | Delegate
    | NamedArgument
    | Value
    | LocalValue
    | Type
    | TypeDef
    | Plaintext
  module TcResolutionsExtensions =
    val ( |CNR| ):
      cnr:NameResolution.CapturedNameResolution ->
        NameResolution.Item * NameResolution.ItemOccurence *
        TypedTreeOps.DisplayEnv * NameResolution.NameResolutionEnv *
        AccessibilityLogic.AccessorDomain * Range.range
    type internal TcResolutions with
      member
        GetSemanticClassification: g:TcGlobals.TcGlobals *
                                    amap:Import.ImportMap *
                                    formatSpecifierLocations:(Range.range * int) [] *
                                    range:Range.range option ->
                                
                                        (Range.range *
                                         SemanticClassificationType) []


namespace FSharp.Compiler.SourceCodeServices
  module ItemKeyTags =
    [<LiteralAttribute>]
    val entityRef: string = "#E#"
    [<LiteralAttribute>]
    val typeTuple: string = "#T#"
    [<LiteralAttribute>]
    val typeAnonymousRecord: string = "#N#"
    [<LiteralAttribute>]
    val typeFunction: string = "#F#"
    [<LiteralAttribute>]
    val typeMeasure: string = "#M#"
    [<LiteralAttribute>]
    val typeUnionCase: string = "#U#"
    [<LiteralAttribute>]
    val typeMeasureVar: string = "#p#"
    [<LiteralAttribute>]
    val typeMeasureCon: string = "#c#"
    [<LiteralAttribute>]
    val itemValueMember: string = "m$"
    [<LiteralAttribute>]
    val itemValue: string = "v$"
    [<LiteralAttribute>]
    val itemUnionCase: string = "u$"
    [<LiteralAttribute>]
    val itemActivePattern: string = "r$"
    [<LiteralAttribute>]
    val itemExnCase: string = "e$"
    [<LiteralAttribute>]
    val itemRecordField: string = "d$"
    [<LiteralAttribute>]
    val itemAnonymousRecordField: string = "a$"
    [<LiteralAttribute>]
    val itemNewDef: string = "n$"
    [<LiteralAttribute>]
    val itemILField: string = "l$"
    [<LiteralAttribute>]
    val itemEvent: string = "t$"
    [<LiteralAttribute>]
    val itemProperty: string = "p$"
    [<LiteralAttribute>]
    val itemTypeVar: string = "y$"
    [<LiteralAttribute>]
    val itemModuleOrNamespace: string = "o$"
    [<LiteralAttribute>]
    val itemDelegateCtor: string = "g$"
    [<LiteralAttribute>]
    val parameters: string = "p$p$"

  [<SealedAttribute>]
  type ItemKeyStore =

      interface System.IDisposable
      new: mmf:System.IO.MemoryMappedFiles.MemoryMappedFile * length:int64 ->
              ItemKeyStore
      member FindAll: NameResolution.Item -> seq<Range.range>
      member ReadFirstKeyString: unit -> System.ReadOnlySpan<byte>
      member
        ReadKeyString: reader:byref<System.Reflection.Metadata.BlobReader> ->
                          System.ReadOnlySpan<byte>
      member
        ReadRange: reader:byref<System.Reflection.Metadata.BlobReader> ->
                      Range.range
  
  [<SealedAttribute>]
  and ItemKeyStoreBuilder =

      new: unit -> ItemKeyStoreBuilder
      member TryBuildAndReset: unit -> ItemKeyStore option
      member Write: Range.range * NameResolution.Item -> unit
  

namespace FSharp.Compiler
  module internal IncrementalBuild =
    val mutable injectCancellationFault: bool
    val LocallyInjectCancellationFault: unit -> System.IDisposable

  module IncrementalBuilderEventTesting =
    type internal FixedLengthMRU<'T> =
  
        new: unit -> FixedLengthMRU<'T>
        member Add: filename:'T -> unit
        member MostRecentList: n:int -> 'T list
        member CurrentEventNum: int
    
    type IBEvent =
      | IBEParsed of string
      | IBETypechecked of string
      | IBECreated
    val MRU: FixedLengthMRU<IBEvent>
    val GetMostRecentIncrementalBuildEvents: int -> IBEvent list
    val GetCurrentIncrementalBuildEventNum: unit -> int

  module IncrementalBuildSyntaxTree =
    [<SealedAttribute>]
    type SyntaxTree =
  
        new: tcConfig:CompilerConfig.TcConfig * fileParsed:Event<string> *
              lexResourceManager:Lexhelp.LexResourceManager *
              sourceRange:Range.range * filename:string *
              isLastCompiland:(bool * bool) -> SyntaxTree
        member Invalidate: unit -> unit
        member
          Parse: sigNameOpt:SyntaxTree.QualifiedNameOfFile option ->
                    SyntaxTree.ParsedInput option * Range.range * string *
                    (ErrorLogger.PhasedDiagnostic *
                     SourceCodeServices.FSharpErrorSeverity) []
        member FileName: string
    

  [<NoEquality; NoComparison>]
  type TcInfo =
    { tcState: ParseAndCheckInputs.TcState
      tcEnvAtEndOfFile: CheckExpressions.TcEnv
      moduleNamesDict: ParseAndCheckInputs.ModuleNamesDict
      topAttribs: CheckDeclarations.TopAttribs option
      latestCcuSigForFile: TypedTree.ModuleOrNamespaceType option
      tcErrorsRev:
        (ErrorLogger.PhasedDiagnostic * SourceCodeServices.FSharpErrorSeverity) [] list
      tcDependencyFiles: string list
      sigNameOpt: (string * SyntaxTree.QualifiedNameOfFile) option }
    with
      member
        TcErrors: (ErrorLogger.PhasedDiagnostic *
                    SourceCodeServices.FSharpErrorSeverity) []
  
  [<NoEquality; NoComparison>]
  type TcInfoOptional =
    { tcResolutionsRev: NameResolution.TcResolutions list
      tcSymbolUsesRev: NameResolution.TcSymbolUses list
      tcOpenDeclarationsRev: NameResolution.OpenDeclaration [] list
      latestImplFile: TypedTree.TypedImplFile option
      itemKeyStore: SourceCodeServices.ItemKeyStore option
      semanticClassification:
   (Range.range * SourceCodeServices.SemanticClassificationType) [] }
    with
      member TcSymbolUses: NameResolution.TcSymbolUses list
  
  [<NoEquality; NoComparison>]
  type TcInfoState =
    | PartialState of TcInfo
    | FullState of TcInfo * TcInfoOptional
    with
      member Partial: TcInfo
  
  [<SealedAttribute>]
  type SemanticModel =

      private new: tcConfig:CompilerConfig.TcConfig *
                    tcGlobals:TcGlobals.TcGlobals *
                    tcImports:CompilerImports.TcImports *
                    keepAssemblyContents:bool *
                    keepAllBackgroundResolutions:bool *
                    maxTimeShareMilliseconds:int64 *
                    keepAllBackgroundSymbolUses:bool *
                    enableBackgroundItemKeyStoreAndSemanticClassification:bool *
                    enablePartialTypeChecking:bool *
                    beforeFileChecked:Event<string> * fileChecked:Event<string> *
                    prevTcInfo:TcInfo *
                    prevTcInfoOptional:AbstractIL.Internal.Library.Eventually<TcInfoOptional option> *
                    syntaxTreeOpt:IncrementalBuildSyntaxTree.SyntaxTree option *
                    lazyTcInfoState:TcInfoState option ref -> SemanticModel
      static member
        Create: tcConfig:CompilerConfig.TcConfig *
                 tcGlobals:TcGlobals.TcGlobals *
                 tcImports:CompilerImports.TcImports * keepAssemblyContents:bool *
                 keepAllBackgroundResolutions:bool *
                 maxTimeShareMilliseconds:int64 *
                 keepAllBackgroundSymbolUses:bool *
                 enableBackgroundItemKeyStoreAndSemanticClassification:bool *
                 enablePartialTypeChecking:bool *
                 beforeFileChecked:Event<string> * fileChecked:Event<string> *
                 prevTcInfo:TcInfo *
                 prevTcInfoOptional:AbstractIL.Internal.Library.Eventually<TcInfoOptional option> *
                 syntaxTreeOpt:IncrementalBuildSyntaxTree.SyntaxTree option ->
                   SemanticModel
      member
        Finish: finalTcErrorsRev:(ErrorLogger.PhasedDiagnostic *
                                   SourceCodeServices.FSharpErrorSeverity) [] list *
                 finalTopAttribs:CheckDeclarations.TopAttribs option ->
                   AbstractIL.Internal.Library.Eventually<SemanticModel>
      member
        GetState: partialCheck:bool ->
                     AbstractIL.Internal.Library.Eventually<TcInfoState>
      member Invalidate: unit -> unit
      member
        Next: syntaxTree:IncrementalBuildSyntaxTree.SyntaxTree ->
                 AbstractIL.Internal.Library.Eventually<SemanticModel>
      member
        private TypeCheck: partialCheck:bool ->
                              AbstractIL.Internal.Library.Eventually<TcInfoState>
      member BackingSignature: SyntaxTree.QualifiedNameOfFile option
      member TcConfig: CompilerConfig.TcConfig
      member TcGlobals: TcGlobals.TcGlobals
      member TcImports: CompilerImports.TcImports
      member TcInfo: AbstractIL.Internal.Library.Eventually<TcInfo>
      member
        TcInfoWithOptional: AbstractIL.Internal.Library.Eventually<TcInfo *
                                                                    TcInfoOptional>
  
  type FrameworkImportsCacheKey =
    string list * string * string list * string * decimal
  type FrameworkImportsCache =

      new: size:int -> FrameworkImportsCache
      member Clear: AbstractIL.Internal.Library.CompilationThreadToken -> unit
      member
        Downsize: AbstractIL.Internal.Library.CompilationThreadToken -> unit
      member
        Get: AbstractIL.Internal.Library.CompilationThreadToken *
              CompilerConfig.TcConfig ->
                AbstractIL.Internal.Library.Cancellable<TcGlobals.TcGlobals *
                                                        CompilerImports.TcImports *
                                                        CompilerImports.AssemblyResolution list *
                                                        CompilerConfig.UnresolvedAssemblyReference list>
  
  [<SealedAttribute>]
  type PartialCheckResults =

      private new: semanticModel:SemanticModel * timeStamp:System.DateTime ->
                      PartialCheckResults
      static member
        Create: semanticModel:SemanticModel * timestamp:System.DateTime ->
                   PartialCheckResults
      member
        GetSemanticClassification: AbstractIL.Internal.Library.CompilationThreadToken ->
                                
                                        (Range.range *
                                         SourceCodeServices.SemanticClassificationType) []
      member
        TcInfo: AbstractIL.Internal.Library.CompilationThreadToken -> TcInfo
      member
        TcInfoWithOptional: AbstractIL.Internal.Library.CompilationThreadToken ->
                               TcInfo * TcInfoOptional
      member
        TryGetItemKeyStore: AbstractIL.Internal.Library.CompilationThreadToken ->
                               SourceCodeServices.ItemKeyStore option
      member TcConfig: CompilerConfig.TcConfig
      member TcGlobals: TcGlobals.TcGlobals
      member TcImports: CompilerImports.TcImports
      member TimeStamp: System.DateTime
  
  module Utilities =
    val TryFindFSharpStringAttribute:
      tcGlobals:TcGlobals.TcGlobals ->
        attribSpec:TcGlobals.BuiltinAttribInfo ->
          attribs:TypedTree.Attribs -> string option

  type RawFSharpAssemblyDataBackedByLanguageService =

      interface CompilerConfig.IRawFSharpAssemblyData
      new: tcConfig:CompilerConfig.TcConfig * tcGlobals:TcGlobals.TcGlobals *
            tcState:ParseAndCheckInputs.TcState * outfile:string *
            topAttrs:CheckDeclarations.TopAttribs * assemblyName:string *
            ilAssemRef:AbstractIL.IL.ILAssemblyRef ->
              RawFSharpAssemblyDataBackedByLanguageService
  
  [<ClassAttribute>]
  type IncrementalBuilder =

      new: tcGlobals:TcGlobals.TcGlobals *
            frameworkTcImports:CompilerImports.TcImports *
            nonFrameworkAssemblyInputs:(Choice<string,
                                               CompilerConfig.IProjectReference> *
                                        (CompilerConfig.TimeStampCache ->
                                           AbstractIL.Internal.Library.CompilationThreadToken ->
                                           System.DateTime)) list *
            nonFrameworkResolutions:CompilerImports.AssemblyResolution list *
            unresolvedReferences:CompilerConfig.UnresolvedAssemblyReference list *
            tcConfig:CompilerConfig.TcConfig * projectDirectory:string *
            outfile:string * assemblyName:string *
            niceNameGen:CompilerGlobalState.NiceNameGenerator *
            lexResourceManager:Lexhelp.LexResourceManager *
            sourceFiles:string list *
            loadClosureOpt:ScriptClosure.LoadClosure option *
            keepAssemblyContents:bool * keepAllBackgroundResolutions:bool *
            maxTimeShareMilliseconds:int64 * keepAllBackgroundSymbolUses:bool *
            enableBackgroundItemKeyStoreAndSemanticClassification:bool *
            enablePartialTypeChecking:bool *
            dependencyProviderOpt:Microsoft.DotNet.DependencyManager.DependencyProvider option ->
              IncrementalBuilder
      static member
        TryCreateIncrementalBuilderForProjectOptions: AbstractIL.Internal.Library.CompilationThreadToken *
                                                       ReferenceResolver.Resolver *
                                                       defaultFSharpBinariesDir:string *
                                                       FrameworkImportsCache *
                                                       loadClosureOpt:ScriptClosure.LoadClosure option *
                                                       sourceFiles:string list *
                                                       commandLineArgs:string list *
                                                       projectReferences:CompilerConfig.IProjectReference list *
                                                       projectDirectory:string *
                                                       useScriptResolutionRules:bool *
                                                       keepAssemblyContents:bool *
                                                       keepAllBackgroundResolutions:bool *
                                                       maxTimeShareMilliseconds:int64 *
                                                       tryGetMetadataSnapshot:AbstractIL.ILBinaryReader.ILReaderTryGetMetadataSnapshot *
                                                       suggestNamesForErrors:bool *
                                                       keepAllBackgroundSymbolUses:bool *
                                                       enableBackgroundItemKeyStoreAndSemanticClassification:bool *
                                                       enablePartialTypeChecking:bool *
                                                       dependencyProvider:Microsoft.DotNet.DependencyManager.DependencyProvider option ->
                                                         AbstractIL.Internal.Library.Cancellable<IncrementalBuilder option *
                                                                                                 SourceCodeServices.FSharpErrorInfo []>
      member AreCheckResultsBeforeFileInProjectReady: filename:string -> bool
      member ContainsFile: filename:string -> bool
      member
        GetCheckResultsAfterFileInProject: AbstractIL.Internal.Library.CompilationThreadToken *
                                            filename:string ->
                                              AbstractIL.Internal.Library.Cancellable<PartialCheckResults>
      member
        GetCheckResultsAfterLastFileInProject: AbstractIL.Internal.Library.CompilationThreadToken ->
                                                  AbstractIL.Internal.Library.Cancellable<PartialCheckResults>
      member
        GetCheckResultsAndImplementationsForProject: AbstractIL.Internal.Library.CompilationThreadToken ->
                                                        AbstractIL.Internal.Library.Cancellable<PartialCheckResults *
                                                                                                AbstractIL.IL.ILAssemblyRef *
                                                                                                CompilerConfig.IRawFSharpAssemblyData option *
                                                                                                TypedTree.TypedImplFile list option>
      member
        GetCheckResultsBeforeFileInProject: AbstractIL.Internal.Library.CompilationThreadToken *
                                             filename:string ->
                                               AbstractIL.Internal.Library.Cancellable<PartialCheckResults>
      member
        GetCheckResultsBeforeFileInProjectEvenIfStale: filename:string ->
                                                          PartialCheckResults option
      member
        GetCheckResultsBeforeSlotInProject: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                                             slotOfFile:int ->
                                               AbstractIL.Internal.Library.Cancellable<PartialCheckResults>
      member
        GetFullCheckResultsAfterFileInProject: AbstractIL.Internal.Library.CompilationThreadToken *
                                                filename:string ->
                                                  AbstractIL.Internal.Library.Cancellable<PartialCheckResults>
      member
        GetFullCheckResultsAndImplementationsForProject: AbstractIL.Internal.Library.CompilationThreadToken ->
                                                            AbstractIL.Internal.Library.Cancellable<PartialCheckResults *
                                                                                                    AbstractIL.IL.ILAssemblyRef *
                                                                                                    CompilerConfig.IRawFSharpAssemblyData option *
                                                                                                    TypedTree.TypedImplFile list option>
      member
        GetLogicalTimeStampForProject: CompilerConfig.TimeStampCache *
                                        AbstractIL.Internal.Library.CompilationThreadToken ->
                                          System.DateTime
      member
        GetParseResultsForFile: AbstractIL.Internal.Library.CompilationThreadToken *
                                 filename:string ->
                                   AbstractIL.Internal.Library.Cancellable<SyntaxTree.ParsedInput option *
                                                                           Range.range *
                                                                           string *
                                                                           (ErrorLogger.PhasedDiagnostic *
                                                                            SourceCodeServices.FSharpErrorSeverity) []>
      member GetSlotOfFileName: filename:string -> int
      member GetSlotsCount: unit -> int
      member
        Step: AbstractIL.Internal.Library.CompilationThreadToken ->
                 AbstractIL.Internal.Library.Cancellable<bool>
      member TryGetCurrentTcImports: unit -> CompilerImports.TcImports option
      member TryGetSlotOfFileName: filename:string -> int option
      member AllDependenciesDeprecated: string []
      member BeforeFileChecked: IEvent<string>
      member FileChecked: IEvent<string>
      member FileParsed: IEvent<string>
      member ImportsInvalidatedByTypeProvider: IEvent<string>
      member ProjectChecked: IEvent<unit>
      member SourceFiles: string list
      member TcConfig: CompilerConfig.TcConfig
  

namespace FSharp.Compiler.SourceCodeServices
  [<RequireQualifiedAccessAttribute>]
  type FSharpGlyph =
    | Class
    | Constant
    | Delegate
    | Enum
    | EnumMember
    | Event
    | Exception
    | Field
    | Interface
    | Method
    | OverridenMethod
    | Module
    | NameSpace
    | Property
    | Struct
    | Typedef
    | Type
    | Union
    | Variable
    | ExtensionMethod
    | Error

namespace FSharp.Compiler.SourceCodeServices
  [<SealedAttribute>]
  type FSharpMethodGroupItemParameter =

      new: name:string * canonicalTypeTextForSorting:string *
            display:Layout.layout * isOptional:bool ->
              FSharpMethodGroupItemParameter
      member CanonicalTypeTextForSorting: string
      member Display: string
      member IsOptional: bool
      member ParameterName: string
      member StructuredDisplay: Layout.layout
  
  module internal DescriptionListsImpl =
    val isFunction: g:TcGlobals.TcGlobals -> ty:TypedTree.TType -> bool
    val printCanonicalizedTypeName:
      g:TcGlobals.TcGlobals ->
        denv:TypedTreeOps.DisplayEnv -> tau:TypedTree.TType -> string
    val PrettyParamOfRecdField:
      g:TcGlobals.TcGlobals ->
        denv:TypedTreeOps.DisplayEnv ->
          f:TypedTree.RecdField -> FSharpMethodGroupItemParameter
    val PrettyParamOfUnionCaseField:
      g:TcGlobals.TcGlobals ->
        denv:TypedTreeOps.DisplayEnv ->
          isGenerated:(int -> TypedTree.RecdField -> bool) ->
            i:int -> f:TypedTree.RecdField -> FSharpMethodGroupItemParameter
    val ParamOfParamData:
      g:TcGlobals.TcGlobals ->
        denv:TypedTreeOps.DisplayEnv ->
          Infos.ParamData -> FSharpMethodGroupItemParameter
    val PrettyParamsOfParamDatas:
      g:TcGlobals.TcGlobals ->
        denv:TypedTreeOps.DisplayEnv ->
          typarInst:TypedTreeOps.TyparInst ->
            paramDatas:Infos.ParamData list ->
              rty:TypedTree.TType ->
                TypedTreeOps.TyparInst * FSharpMethodGroupItemParameter list *
                Internal.Utilities.StructuredFormat.Layout *
                Internal.Utilities.StructuredFormat.Layout
    val PrettyParamsOfTypes:
      g:TcGlobals.TcGlobals ->
        denv:TypedTreeOps.DisplayEnv ->
          typarInst:TypedTreeOps.TyparInst ->
            paramTys:TypedTree.TTypes ->
              retTy:TypedTree.TType ->
                TypedTreeOps.TyparInst * FSharpMethodGroupItemParameter list *
                Internal.Utilities.StructuredFormat.Layout *
                Internal.Utilities.StructuredFormat.Layout
    val StaticParamsOfItem:
      infoReader:InfoReader.InfoReader ->
        m:Range.range ->
          denv:TypedTreeOps.DisplayEnv ->
            item:NameResolution.Item -> FSharpMethodGroupItemParameter []
    val PrettyParamsAndReturnTypeOfItem:
      infoReader:InfoReader.InfoReader ->
        m:Range.range ->
          denv:TypedTreeOps.DisplayEnv ->
            item:NameResolution.ItemWithInst ->
              FSharpMethodGroupItemParameter list *
              Internal.Utilities.StructuredFormat.Layout
    val GlyphOfItem:
      denv:TypedTreeOps.DisplayEnv * item:NameResolution.Item -> FSharpGlyph
    val AnotherFlattenItems:
      g:TcGlobals.TcGlobals ->
        m:Range.range -> item:NameResolution.Item -> NameResolution.Item list

  [<SealedAttribute>]
  type FSharpDeclarationListItem =

      new: name:string * nameInCode:string * fullName:string *
            glyph:FSharpGlyph *
            info:Choice<(CompletionItem list * InfoReader.InfoReader *
                         Range.range * TypedTreeOps.DisplayEnv),
                        FSharpToolTipText<Layout>> *
            accessibility:FSharpAccessibility option * kind:CompletionItemKind *
            isOwnMember:bool * priority:int * isResolved:bool *
            namespaceToOpen:string option -> FSharpDeclarationListItem
      member Accessibility: FSharpAccessibility option
      member DescriptionText: FSharpToolTipText
      [<System.Obsolete
        ("This operation is no longer asynchronous, please use the non-async version")>]
      member DescriptionTextAsync: Async<FSharpToolTipText>
      member FullName: string
      member Glyph: FSharpGlyph
      member IsOwnMember: bool
      member IsResolved: bool
      member Kind: CompletionItemKind
      member MinorPriority: int
      member Name: string
      member NameInCode: string
      member NamespaceToOpen: string option
      member StructuredDescriptionText: FSharpToolTipText<Layout>
      [<System.Obsolete
        ("This operation is no longer asynchronous, please use the non-async version")>]
      member StructuredDescriptionTextAsync: Async<FSharpToolTipText<Layout>>
  
  [<SealedAttribute>]
  type FSharpDeclarationListInfo =

      new: declarations:FSharpDeclarationListItem [] * isForType:bool *
            isError:bool -> FSharpDeclarationListInfo
      static member
        Create: infoReader:InfoReader.InfoReader * m:Range.range *
                 denv:TypedTreeOps.DisplayEnv *
                 getAccessibility:(NameResolution.Item ->
                                     FSharpAccessibility option) *
                 items:CompletionItem list * currentNamespace:string [] option *
                 isAttributeApplicationContext:bool -> FSharpDeclarationListInfo
      static member Error: message:string -> FSharpDeclarationListInfo
      member IsError: bool
      member IsForType: bool
      member Items: FSharpDeclarationListItem []
      static member Empty: FSharpDeclarationListInfo
  
  [<SealedAttribute (); NoEquality; NoComparison>]
  type FSharpMethodGroupItem =

      new: description:FSharpToolTipText<Layout.layout> * xmlDoc:FSharpXmlDoc *
            returnType:Layout.layout *
            parameters:FSharpMethodGroupItemParameter [] * hasParameters:bool *
            hasParamArrayArg:bool *
            staticParameters:FSharpMethodGroupItemParameter [] ->
              FSharpMethodGroupItem
      member Description: FSharpToolTipText
      member HasParamArrayArg: bool
      member HasParameters: bool
      member Parameters: FSharpMethodGroupItemParameter []
      member ReturnTypeText: string
      member StaticParameters: FSharpMethodGroupItemParameter []
      member StructuredDescription: FSharpToolTipText<Layout.layout>
      member StructuredReturnTypeText: Layout.layout
      member XmlDoc: FSharpXmlDoc
  
  [<SealedAttribute>]
  type FSharpMethodGroup =

      new: string * FSharpMethodGroupItem [] -> FSharpMethodGroup
      static member
        Create: InfoReader.InfoReader * Range.range * TypedTreeOps.DisplayEnv *
                 NameResolution.ItemWithInst list -> FSharpMethodGroup
      member MethodName: string
      member Methods: FSharpMethodGroupItem []
  

namespace FSharp.Compiler.SourceCodeServices
  type Position = int * int
  type Range = Position * Position
  module FSharpTokenTag =
    val Identifier: int
    val String: int
    val IDENT: int
    val STRING: int
    val INTERP_STRING_BEGIN_END: int
    val INTERP_STRING_BEGIN_PART: int
    val INTERP_STRING_PART: int
    val INTERP_STRING_END: int
    val LPAREN: int
    val RPAREN: int
    val LBRACK: int
    val RBRACK: int
    val LBRACE: int
    val RBRACE: int
    val LBRACK_LESS: int
    val GREATER_RBRACK: int
    val LESS: int
    val GREATER: int
    val LBRACK_BAR: int
    val BAR_RBRACK: int
    val PLUS_MINUS_OP: int
    val MINUS: int
    val STAR: int
    val INFIX_STAR_DIV_MOD_OP: int
    val PERCENT_OP: int
    val INFIX_AT_HAT_OP: int
    val QMARK: int
    val COLON: int
    val EQUALS: int
    val SEMICOLON: int
    val COMMA: int
    val DOT: int
    val DOT_DOT: int
    val DOT_DOT_HAT: int
    val INT32_DOT_DOT: int
    val UNDERSCORE: int
    val BAR: int
    val COLON_GREATER: int
    val COLON_QMARK_GREATER: int
    val COLON_QMARK: int
    val INFIX_BAR_OP: int
    val INFIX_COMPARE_OP: int
    val COLON_COLON: int
    val AMP_AMP: int
    val PREFIX_OP: int
    val COLON_EQUALS: int
    val BAR_BAR: int
    val RARROW: int
    val LARROW: int
    val QUOTE: int
    val WHITESPACE: int
    val COMMENT: int
    val LINE_COMMENT: int
    val BEGIN: int
    val DO: int
    val FUNCTION: int
    val THEN: int
    val ELSE: int
    val STRUCT: int
    val CLASS: int
    val TRY: int
    val NEW: int
    val WITH: int
    val OWITH: int

  type FSharpTokenColorKind =
    | Default = 0
    | Text = 0
    | Keyword = 1
    | Comment = 2
    | Identifier = 3
    | String = 4
    | UpperIdentifier = 5
    | InactiveCode = 7
    | PreprocessorKeyword = 8
    | Number = 9
    | Operator = 10
    | Punctuation = 11
  type FSharpTokenTriggerClass =
    | None = 0
    | MemberSelect = 1
    | MatchBraces = 2
    | ChoiceSelect = 4
    | MethodTip = 240
    | ParamStart = 16
    | ParamNext = 32
    | ParamEnd = 64
  type FSharpTokenCharKind =
    | Default = 0
    | Text = 0
    | Keyword = 1
    | Identifier = 2
    | String = 3
    | Literal = 4
    | Operator = 5
    | Delimiter = 6
    | WhiteSpace = 8
    | LineComment = 9
    | Comment = 10
  type FSharpTokenInfo =
    { LeftColumn: int
      RightColumn: int
      ColorClass: FSharpTokenColorKind
      CharClass: FSharpTokenCharKind
      FSharpTokenTriggerClass: FSharpTokenTriggerClass
      Tag: int
      TokenName: string
      FullMatchedLength: int }
  module internal TokenClassifications =
    val tokenInfo:
      token:Parser.token ->
        FSharpTokenColorKind * FSharpTokenCharKind * FSharpTokenTriggerClass

  module internal TestExpose =
    val TokenInfo:
      Parser.token ->
        FSharpTokenColorKind * FSharpTokenCharKind * FSharpTokenTriggerClass

  [<StructAttribute (); CustomEqualityAttribute (); NoComparison>]
  type FSharpTokenizerLexState =
    { PosBits: int64
      OtherBits: int64 }
    with
      override Equals: obj:obj -> bool
      member Equals: FSharpTokenizerLexState -> bool
      override GetHashCode: unit -> int
      static member Initial: FSharpTokenizerLexState
  
  type FSharpTokenizerColorState =
    | Token = 1
    | IfDefSkip = 3
    | String = 4
    | Comment = 5
    | StringInComment = 6
    | VerbatimStringInComment = 7
    | CamlOnly = 8
    | VerbatimString = 9
    | SingleLineComment = 10
    | EndLineThenSkip = 11
    | EndLineThenToken = 12
    | TripleQuoteString = 13
    | TripleQuoteStringInComment = 14
    | InitialState = 0
  module internal LexerStateEncoding =
    val computeNextLexState:
      token:Parser.token ->
        prevLexcont:ParseHelpers.LexerContinuation ->
          ParseHelpers.LexerContinuation
    val revertToDefaultLexCont: ParseHelpers.LexerContinuation
    val lexstateNumBits: int
    val ncommentsNumBits: int
    val hardwhiteNumBits: int
    val ifdefstackCountNumBits: int
    val ifdefstackNumBits: int
    val stringKindBits: int
    val nestingBits: int
    val lexstateStart: int
    val ncommentsStart: int
    val hardwhitePosStart: int
    val ifdefstackCountStart: int
    val ifdefstackStart: int
    val stringKindStart: int
    val nestingStart: int
    val lexstateMask: int64
    val ncommentsMask: int64
    val hardwhitePosMask: int64
    val ifdefstackCountMask: int64
    val ifdefstackMask: int64
    val stringKindMask: int64
    val nestingMask: int64
    val bitOfBool: b:bool -> int
    val boolOfBit: n:int64 -> bool
    val colorStateOfLexState:
      state:FSharpTokenizerLexState -> FSharpTokenizerColorState
    val lexStateOfColorState: state:FSharpTokenizerColorState -> int64
    val encodeStringStyle: kind:ParseHelpers.LexerStringStyle -> int
    val decodeStringStyle: kind:int -> ParseHelpers.LexerStringStyle
    val encodeLexCont:
      colorState:FSharpTokenizerColorState * numComments:int64 * b:Range.pos *
      ifdefStack:seq<ParseHelpers.LexerIfdefStackEntry * 'a> * light:bool *
      stringKind:ParseHelpers.LexerStringKind *
      stringNest:(int * ParseHelpers.LexerStringStyle * 'b) list ->
        FSharpTokenizerLexState
    val decodeLexCont:
      state:FSharpTokenizerLexState ->
        FSharpTokenizerColorState * int32 * Range.pos *
        (ParseHelpers.LexerIfdefStackEntry * Range.range) list * bool *
        ParseHelpers.LexerStringKind *
        ParseHelpers.LexerInterpolatedStringNesting
    val encodeLexInt:
      lightStatus:bool ->
        lexcont:ParseHelpers.LexerContinuation -> FSharpTokenizerLexState
    val decodeLexInt:
      state:FSharpTokenizerLexState -> bool * ParseHelpers.LexerContinuation

  type SingleLineTokenState =
    | BeforeHash = 0
    | NoFurtherMatchPossible = 1
  [<SealedAttribute>]
  type FSharpLineTokenizer =

      new: lexbuf:UnicodeLexing.Lexbuf * maxLength:int option *
            filename:string option * lexargs:Lexhelp.LexArgs ->
              FSharpLineTokenizer
      static member
        ColorStateOfLexState: FSharpTokenizerLexState ->
                                 FSharpTokenizerColorState
      static member
        LexStateOfColorState: FSharpTokenizerColorState ->
                                 FSharpTokenizerLexState
      member
        ScanToken: lexState:FSharpTokenizerLexState ->
                      FSharpTokenInfo option * FSharpTokenizerLexState
  
  [<SealedAttribute>]
  type FSharpSourceTokenizer =

      new: conditionalDefines:string list * filename:string option ->
              FSharpSourceTokenizer
      member
        CreateBufferTokenizer: bufferFiller:(char [] * int * int -> int) ->
                                  FSharpLineTokenizer
      member CreateLineTokenizer: lineText:string -> FSharpLineTokenizer
  
  module Keywords =
    val DoesIdentifierNeedQuotation: string -> bool
    val QuoteIdentifierIfNeeded: string -> string
    val NormalizeIdentifierBackticks: string -> string
    val KeywordsWithDescription: (string * string) list

  module Lexer =
    [<System.Flags ();
      ExperimentalAttribute
      ("This FCS API is experimental and subject to change.")>]
    type FSharpLexerFlags =
      | Default = 69649
      | LightSyntaxOn = 1
      | Compiling = 16
      | CompilingFSharpCore = 272
      | SkipTrivia = 4096
      | UseLexFilter = 65536
    [<RequireQualifiedAccessAttribute ();
      ExperimentalAttribute
      ("This FCS API is experimental and subject to change.")>]
    type FSharpSyntaxTokenKind =
      | None
      | HashIf
      | HashElse
      | HashEndIf
      | CommentTrivia
      | WhitespaceTrivia
      | HashLine
      | HashLight
      | InactiveCode
      | LineCommentTrivia
      | StringText
      | Fixed
      | OffsideInterfaceMember
      | OffsideBlockEnd
      | OffsideRightBlockEnd
      | OffsideDeclEnd
      | OffsideEnd
      | OffsideBlockSep
      | OffsideBlockBegin
      | OffsideReset
      | OffsideFun
      | OffsideFunction
      | OffsideWith
      | OffsideElse
      | OffsideThen
      | OffsideDoBang
      | OffsideDo
      | OffsideBinder
      | OffsideLet
      | HighPrecedenceTypeApp
      | HighPrecedenceParenthesisApp
      | HighPrecedenceBracketApp
      | Extern
      | Void
      | Public
      | Private
      | Internal
      | Global
      | Static
      | Member
      | Class
      | Abstract
      | Override
      | Default
      | Constructor
      | Inherit
      | GreaterRightBracket
      | Struct
      | Sig
      | Bar
      | RightBracket
      | RightBrace
      | Minus
      | Dollar
      | BarRightBracket
      | BarRightBrace
      | Underscore
      | Semicolon
      | SemicolonSemicolon
      | LeftArrow
      | Equals
      | LeftBracket
      | LeftBracketBar
      | LeftBraceBar
      | LeftBracketLess
      | LeftBrace
      | QuestionMark
      | QuestionMarkQuestionMark
      | Dot
      | Colon
      | ColonColon
      | ColonGreater
      | ColonQuestionMark
      | ColonQuestionMarkGreater
      | ColonEquals
      | When
      | While
      | With
      | Hash
      | Ampersand
      | AmpersandAmpersand
      | Quote
      | LeftParenthesis
      | RightParenthesis
      | Star
      | Comma
      | RightArrow
      | GreaterBarRightBracket
      | LeftParenthesisStarRightParenthesis
      | Open
      | Or
      | Rec
      | Then
      | To
      | True
      | Try
      | Type
      | Val
      | Inline
      | Interface
      | Instance
      | Const
      | Lazy
      | OffsideLazy
      | Match
      | MatchBang
      | Mutable
      | New
      | Of
      | Exception
      | False
      | For
      | Fun
      | Function
      | If
      | In
      | JoinIn
      | Finally
      | DoBang
      | And
      | As
      | Assert
      | OffsideAssert
      | Begin
      | Do
      | Done
      | DownTo
      | Else
      | Elif
      | End
      | DotDot
      | DotDotHat
      | BarBar
      | Upcast
      | Downcast
      | Null
      | Reserved
      | Module
      | Namespace
      | Delegate
      | Constraint
      | Base
      | LeftQuote
      | RightQuote
      | RightQuoteDot
      | PercentOperator
      | Binder
      | Less
      | Greater
      | Let
      | Yield
      | YieldBang
      | BigNumber
      | Decimal
      | Char
      | Ieee64
      | Ieee32
      | NativeInt
      | UNativeInt
      | UInt64
      | UInt32
      | UInt16
      | UInt8
      | Int64
      | Int32
      | Int32DotDot
      | Int16
      | Int8
      | FunkyOperatorName
      | AdjacentPrefixOperator
      | PlusMinusOperator
      | InfixAmpersandOperator
      | InfixStarDivideModuloOperator
      | PrefixOperator
      | InfixBarOperator
      | InfixAtHatOperator
      | InfixCompareOperator
      | InfixStarStarOperator
      | Identifier
      | KeywordString
      | String
      | ByteArray
      | Asr
      | InfixAsr
      | InfixLand
      | InfixLor
      | InfixLsl
      | InfixLsr
      | InfixLxor
      | InfixMod
    [<StructAttribute (); NoComparison (); NoEquality;
      ExperimentalAttribute
      ("This FCS API is experimental and subject to change.")>]
    type FSharpSyntaxToken =

        new: tok:Parser.token * tokRange:Range.range -> FSharpSyntaxToken
        val private tok: Parser.token
        val private tokRange: Range.range
        member IsCommentTrivia: bool
        member IsIdentifier: bool
        member IsKeyword: bool
        member IsNumericLiteral: bool
        member IsStringLiteral: bool
        member Kind: FSharpSyntaxTokenKind
        member Range: Range.range
    
    val lexWithErrorLogger:
      text:Text.ISourceText ->
        conditionalCompilationDefines:string list ->
          flags:FSharpLexerFlags ->
            supportsFeature:(Features.LanguageFeature -> bool) ->
              errorLogger:ErrorLogger.ErrorLogger ->
                onToken:(Parser.token -> Range.range -> unit) ->
                  pathMap:Internal.Utilities.PathMap ->
                    ct:System.Threading.CancellationToken -> unit
    val lex:
      text:Text.ISourceText ->
        conditionalCompilationDefines:string list ->
          flags:FSharpLexerFlags ->
            supportsFeature:(Features.LanguageFeature -> bool) ->
              lexCallback:(Parser.token -> Range.range -> unit) ->
                pathMap:Internal.Utilities.PathMap ->
                  ct:System.Threading.CancellationToken -> unit
    [<AbstractClassAttribute (); SealedAttribute ();
      ExperimentalAttribute
      ("This FCS API is experimental and subject to change.")>]
    type FSharpLexer =
  
        [<ExperimentalAttribute
          ("This FCS API is experimental and subject to change.")>]
        static member
          Lex: text:Text.ISourceText *
                tokenCallback:(FSharpSyntaxToken -> unit) * ?langVersion:string *
                ?filePath:string * ?conditionalCompilationDefines:string list *
                ?flags:FSharpLexerFlags * ?pathMap:Map<string,string> *
                ?ct:System.Threading.CancellationToken -> unit
    


namespace FSharp.Compiler.SourceCodeServices
  module AstTraversal =
    val rangeContainsPosLeftEdgeInclusive:
      m1:Range.range -> p:Range.pos -> bool
    val rangeContainsPosEdgesExclusive: m1:Range.range -> p:Range.pos -> bool
    val rangeContainsPosLeftEdgeExclusiveAndRightEdgeInclusive:
      m1:Range.range -> p:Range.pos -> bool
    [<RequireQualifiedAccessAttribute>]
    type TraverseStep =
      | Expr of SyntaxTree.SynExpr
      | Module of SyntaxTree.SynModuleDecl
      | ModuleOrNamespace of SyntaxTree.SynModuleOrNamespace
      | TypeDefn of SyntaxTree.SynTypeDefn
      | MemberDefn of SyntaxTree.SynMemberDefn
      | MatchClause of SyntaxTree.SynMatchClause
      | Binding of SyntaxTree.SynBinding
    type TraversePath = TraverseStep list
    [<AbstractClassAttribute>]
    type AstVisitorBase<'T> =
  
        new: unit -> AstVisitorBase<'T>
        override
          VisitBinding: defaultTraverse:(SyntaxTree.SynBinding -> 'T option) *
                         binding:SyntaxTree.SynBinding -> 'T option
        abstract member
          VisitBinding: (SyntaxTree.SynBinding -> 'T option) *
                         SyntaxTree.SynBinding -> 'T option
        override VisitComponentInfo: SyntaxTree.SynComponentInfo -> 'T option
        abstract member
          VisitComponentInfo: SyntaxTree.SynComponentInfo -> 'T option
        abstract member
          VisitExpr: TraversePath * (SyntaxTree.SynExpr -> 'T option) *
                      (SyntaxTree.SynExpr -> 'T option) * SyntaxTree.SynExpr ->
                        'T option
        override VisitHashDirective: Range.range -> 'T option
        abstract member VisitHashDirective: Range.range -> 'T option
        override
          VisitImplicitInherit: defaultTraverse:(SyntaxTree.SynExpr ->
                                                    'T option) *
                                 _ty:SyntaxTree.SynType *
                                 expr:SyntaxTree.SynExpr * _m:Range.range ->
                                   'T option
        abstract member
          VisitImplicitInherit: (SyntaxTree.SynExpr -> 'T option) *
                                 SyntaxTree.SynType * SyntaxTree.SynExpr *
                                 Range.range -> 'T option
        override
          VisitInheritSynMemberDefn: _componentInfo:SyntaxTree.SynComponentInfo *
                                      _typeDefnKind:SyntaxTree.SynTypeDefnKind *
                                      _synType:SyntaxTree.SynType *
                                      _members:SyntaxTree.SynMemberDefns *
                                      _range:Range.range -> 'T option
        abstract member
          VisitInheritSynMemberDefn: SyntaxTree.SynComponentInfo *
                                      SyntaxTree.SynTypeDefnKind *
                                      SyntaxTree.SynType *
                                      SyntaxTree.SynMemberDefns * Range.range ->
                                        'T option
        override
          VisitInterfaceSynMemberDefnType: _synType:SyntaxTree.SynType ->
                                              'T option
        abstract member
          VisitInterfaceSynMemberDefnType: SyntaxTree.SynType -> 'T option
        override
          VisitLetOrUse: TraversePath * (SyntaxTree.SynBinding -> 'T option) *
                          SyntaxTree.SynBinding list * Range.range -> 'T option
        abstract member
          VisitLetOrUse: TraversePath * (SyntaxTree.SynBinding -> 'T option) *
                          SyntaxTree.SynBinding list * Range.range -> 'T option
        override
          VisitMatchClause: defaultTraverse:(SyntaxTree.SynMatchClause ->
                                                'T option) *
                             mc:SyntaxTree.SynMatchClause -> 'T option
        abstract member
          VisitMatchClause: (SyntaxTree.SynMatchClause -> 'T option) *
                             SyntaxTree.SynMatchClause -> 'T option
        override
          VisitModuleDecl: defaultTraverse:(SyntaxTree.SynModuleDecl ->
                                               'T option) *
                            decl:SyntaxTree.SynModuleDecl -> 'T option
        abstract member
          VisitModuleDecl: (SyntaxTree.SynModuleDecl -> 'T option) *
                            SyntaxTree.SynModuleDecl -> 'T option
        override
          VisitModuleOrNamespace: SyntaxTree.SynModuleOrNamespace -> 'T option
        abstract member
          VisitModuleOrNamespace: SyntaxTree.SynModuleOrNamespace -> 'T option
        override
          VisitPat: defaultTraverse:(SyntaxTree.SynPat -> 'T option) *
                     pat:SyntaxTree.SynPat -> 'T option
        abstract member
          VisitPat: (SyntaxTree.SynPat -> 'T option) * SyntaxTree.SynPat ->
                       'T option
        override
          VisitRecordField: _path:TraversePath *
                             _copyOpt:SyntaxTree.SynExpr option *
                             _recordField:SyntaxTree.LongIdentWithDots option ->
                               'T option
        abstract member
          VisitRecordField: TraversePath * SyntaxTree.SynExpr option *
                             SyntaxTree.LongIdentWithDots option -> 'T option
        override VisitSimplePats: SyntaxTree.SynSimplePat list -> 'T option
        abstract member
          VisitSimplePats: SyntaxTree.SynSimplePat list -> 'T option
        override
          VisitType: defaultTraverse:(SyntaxTree.SynType -> 'T option) *
                      ty:SyntaxTree.SynType -> 'T option
        abstract member
          VisitType: (SyntaxTree.SynType -> 'T option) * SyntaxTree.SynType ->
                        'T option
        override
          VisitTypeAbbrev: _ty:SyntaxTree.SynType * _m:Range.range -> 'T option
        abstract member
          VisitTypeAbbrev: SyntaxTree.SynType * Range.range -> 'T option
    
    val dive: node:'a -> range:'b -> project:('a -> 'c) -> 'b * (unit -> 'c)
    val pick:
      pos:Range.pos ->
        outerRange:Range.range ->
          _debugObj:obj ->
            diveResults:(Range.range * (unit -> 'a option)) list -> 'a option
    val Traverse:
      pos:Range.pos * parseTree:SyntaxTree.ParsedInput *
      visitor:AstVisitorBase<'T> -> 'T option


namespace FSharp.Compiler.SourceCodeServices
  type FSharpNavigationDeclarationItemKind =
    | NamespaceDecl
    | ModuleFileDecl
    | ExnDecl
    | ModuleDecl
    | TypeDecl
    | MethodDecl
    | PropertyDecl
    | FieldDecl
    | OtherDecl
  [<RequireQualifiedAccessAttribute>]
  type FSharpEnclosingEntityKind =
    | Namespace
    | Module
    | Class
    | Exception
    | Interface
    | Record
    | Enum
    | DU
  [<SealedAttribute>]
  type FSharpNavigationDeclarationItem =

      new: uniqueName:string * name:string *
            kind:FSharpNavigationDeclarationItemKind * glyph:FSharpGlyph *
            range:Range.range * bodyRange:Range.range * singleTopLevel:bool *
            enclosingEntityKind:FSharpEnclosingEntityKind * isAbstract:bool *
            access:SyntaxTree.SynAccess option ->
              FSharpNavigationDeclarationItem
      static member
        Create: name:string * kind:FSharpNavigationDeclarationItemKind *
                 glyph:FSharpGlyph * range:Range.range * bodyRange:Range.range *
                 singleTopLevel:bool *
                 enclosingEntityKind:FSharpEnclosingEntityKind * isAbstract:bool *
                 access:SyntaxTree.SynAccess option ->
                   FSharpNavigationDeclarationItem
      member
        WithUniqueName: uniqueName:string -> FSharpNavigationDeclarationItem
      member Access: SyntaxTree.SynAccess option
      member BodyRange: Range.range
      member EnclosingEntityKind: FSharpEnclosingEntityKind
      member Glyph: FSharpGlyph
      member IsAbstract: bool
      member IsSingleTopLevel: bool
      member Kind: FSharpNavigationDeclarationItemKind
      member Name: string
      member Range: Range.range
      member UniqueName: string
      member bodyRange: Range.range
  
  [<NoEquality; NoComparison>]
  type FSharpNavigationTopLevelDeclaration =
    { Declaration: FSharpNavigationDeclarationItem
      Nested: FSharpNavigationDeclarationItem [] }
  [<SealedAttribute>]
  type FSharpNavigationItems =

      new: declarations:FSharpNavigationTopLevelDeclaration [] ->
              FSharpNavigationItems
      member Declarations: FSharpNavigationTopLevelDeclaration []
  
  module NavigationImpl =
    val unionRangesChecked: r1:Range.range -> r2:Range.range -> Range.range
    val rangeOfDecls2:
      f:('a -> FSharpNavigationDeclarationItem) -> decls:'a list -> Range.range
    val rangeOfDecls:
      ((FSharpNavigationDeclarationItem * int) list -> Range.range)
    val moduleRange:
      idm:Range.range ->
        others:(FSharpNavigationDeclarationItem * 'a * 'b) list -> Range.range
    val fldspecRange: fldspec:SyntaxTree.SynUnionCaseType -> Range.range
    val bodyRange:
      mb:Range.range ->
        decls:(FSharpNavigationDeclarationItem * int) list -> Range.range
    val getNavigationFromImplFile:
      modules:SyntaxTree.SynModuleOrNamespace list -> FSharpNavigationItems
    val getNavigationFromSigFile:
      modules:SyntaxTree.SynModuleOrNamespaceSig list -> FSharpNavigationItems

  module FSharpNavigation =
    val getNavigation: SyntaxTree.ParsedInput -> FSharpNavigationItems
    val empty: FSharpNavigationItems

  module NavigateTo =
    [<RequireQualifiedAccessAttribute>]
    type NavigableItemKind =
      | Module
      | ModuleAbbreviation
      | Exception
      | Type
      | ModuleValue
      | Field
      | Property
      | Constructor
      | Member
      | EnumCase
      | UnionCase
      with
        override ToString: unit -> string
    
    [<RequireQualifiedAccessAttribute>]
    type ContainerType =
      | File
      | Namespace
      | Module
      | Type
      | Exception
    type Container =
      { Type: ContainerType
        Name: string }
    type NavigableItem =
      { Name: string
        Range: Range.range
        IsSignature: bool
        Kind: NavigableItemKind
        Container: Container }
    val getNavigableItems: SyntaxTree.ParsedInput -> NavigableItem []


namespace FSharp.Compiler.SourceCodeServices
  [<SealedAttribute>]
  type FSharpNoteworthyParamInfoLocations =

      new: longId:string list * longIdRange:Range.range *
            openParenLocation:Range.pos * tupleEndLocations:Range.pos list *
            isThereACloseParen:bool * namedParamNames:string option list ->
              FSharpNoteworthyParamInfoLocations
      static member
        Find: Range.pos * SyntaxTree.ParsedInput ->
                 FSharpNoteworthyParamInfoLocations option
      member IsThereACloseParen: bool
      member LongId: string list
      member LongIdEndLocation: Range.pos
      member LongIdStartLocation: Range.pos
      member NamedParamNames: string option []
      member OpenParenLocation: Range.pos
      member TupleEndLocations: Range.pos []
  
  module internal NoteworthyParamInfoLocationsImpl =
    val isStaticArg: SyntaxTree.SynType -> bool
    val digOutIdentFromFuncExpr:
      synExpr:SyntaxTree.SynExpr -> (string list * Range.range) option
    type FindResult =
      | Found of
        openParen: Range.pos *
        commasAndCloseParen: (Range.pos * string option) list *
        hasClosedParen: bool
      | NotFound
    val digOutIdentFromStaticArg: SyntaxTree.SynType -> string option
    val getNamedParamName: e:SyntaxTree.SynExpr -> string option
    val getTypeName: synType:SyntaxTree.SynType -> string list
    val handleSingleArg:
      traverseSynExpr:(SyntaxTree.SynExpr -> 'a option) ->
        pos:Range.pos * synExpr:SyntaxTree.SynExpr * parenRange:Range.range *
        rpRangeOpt:'b option -> FindResult * 'c option
    val searchSynArgExpr:
      traverseSynExpr:(SyntaxTree.SynExpr -> 'a option) ->
        pos:Range.pos ->
          expr:SyntaxTree.SynExpr -> FindResult * 'a option option
    val ( |StaticParameters|_| ):
      pos:Range.pos ->
        SyntaxTree.SynType -> FSharpNoteworthyParamInfoLocations option
    val traverseInput:
      pos:Range.pos * parseTree:SyntaxTree.ParsedInput ->
        FSharpNoteworthyParamInfoLocations option

  module internal SynExprAppLocationsImpl =
    val private searchSynArgExpr:
      traverseSynExpr:(SyntaxTree.SynExpr -> 'a option) ->
        expr:SyntaxTree.SynExpr ->
          ranges:Range.range list -> Range.range list option * 'a option option
    val getAllCurriedArgsAtPosition:
      pos:Range.pos ->
        parseTree:SyntaxTree.ParsedInput -> Range.range list option


namespace FSharp.Compiler.SourceCodeServices
  module SourceFile =
    val private compilableExtensions: string list
    val private singleFileProjectExtensions: string list
    val IsCompilable: string -> bool
    val MustBeSingleFileProject: string -> bool

  module SourceFileImpl =
    val IsInterfaceFile: string -> bool
    val AdditionalDefinesForUseInEditor: isInteractive:bool -> string list

  type CompletionPath = string list * string option
  [<RequireQualifiedAccessAttribute>]
  type InheritanceOrigin =
    | Class
    | Interface
    | Unknown
  [<RequireQualifiedAccessAttribute>]
  type InheritanceContext =
    | Class
    | Interface
    | Unknown
  [<RequireQualifiedAccessAttribute>]
  type RecordContext =
    | CopyOnUpdate of Range.range * CompletionPath
    | Constructor of string
    | New of CompletionPath
  [<RequireQualifiedAccessAttribute>]
  type CompletionContext =
    | Invalid
    | Inherit of InheritanceContext * CompletionPath
    | RecordField of RecordContext
    | RangeOperator
    | ParameterList of Range.pos * System.Collections.Generic.HashSet<string>
    | AttributeApplication
    | OpenDeclaration of isOpenType: bool
    | PatternType
  [<SealedAttribute>]
  type FSharpParseFileResults =

      new: errors:FSharpErrorInfo [] * input:SyntaxTree.ParsedInput option *
            parseHadErrors:bool * dependencyFiles:string [] ->
              FSharpParseFileResults
      member
        FindNoteworthyParamInfoLocations: pos:Range.pos ->
                                             FSharpNoteworthyParamInfoLocations option
      member
        GetAllArgumentsForFunctionApplicationAtPostion: pos:Range.pos ->
                                                           Range.range list option
      member GetNavigationItems: unit -> FSharpNavigationItems
      member private GetNavigationItemsImpl: unit -> FSharpNavigationItems
      member IsPosContainedInApplication: pos:Range.pos -> bool
      member IsPositionContainedInACurriedParameter: pos:Range.pos -> bool
      member
        TryIdentOfPipelineContainingPosAndNumArgsApplied: pos:Range.pos ->
                                                             (SyntaxTree.Ident *
                                                              int) option
      member TryRangeOfExprInYieldOrReturn: pos:Range.pos -> Range.range option
      member
        TryRangeOfFunctionOrMethodBeingApplied: pos:Range.pos ->
                                                   Range.range option
      member
        TryRangeOfParenEnclosingOpEqualsGreaterUsage: opGreaterEqualPos:Range.pos ->
                                                         (Range.range *
                                                          Range.range *
                                                          Range.range) option
      member
        TryRangeOfRecordExpressionContainingPos: pos:Range.pos ->
                                                    Range.range option
      member
        TryRangeOfRefCellDereferenceContainingPos: expressionPos:Range.pos ->
                                                      Range.range option
      member ValidateBreakpointLocation: pos:Range.pos -> Range.range option
      member
        private ValidateBreakpointLocationImpl: pos:Range.pos ->
                                                   Range.range option
      member DependencyFiles: string []
      member Errors: FSharpErrorInfo []
      member FileName: string
      member ParseHadErrors: bool
      member ParseTree: SyntaxTree.ParsedInput option
  
  type ModuleKind =
    { IsAutoOpen: bool
      HasModuleSuffix: bool }
  [<RequireQualifiedAccessAttribute>]
  type EntityKind =
    | Attribute
    | Type
    | FunctionOrValue of isActivePattern: bool
    | Module of ModuleKind
    with
      override ToString: unit -> string
  
  module UntypedParseImpl =
    val emptyStringSet: System.Collections.Generic.HashSet<string>
    val GetRangeOfExprLeftOfDot:
      Range.pos * SyntaxTree.ParsedInput option -> Range.range option
    val TryFindExpressionIslandInPosition:
      Range.pos * SyntaxTree.ParsedInput option -> string option
    val TryFindExpressionASTLeftOfDotLeftOfCursor:
      Range.pos * SyntaxTree.ParsedInput option -> (Range.pos * bool) option
    val GetEntityKind: Range.pos * SyntaxTree.ParsedInput -> EntityKind option
    type internal TS = AstTraversal.TraverseStep
    val insideAttributeApplicationRegex: System.Text.RegularExpressions.Regex
    val TryGetCompletionContext:
      Range.pos * SyntaxTree.ParsedInput * lineStr:string ->
        CompletionContext option
    val GetFullNameOfSmallestModuleOrNamespaceAtPoint:
      SyntaxTree.ParsedInput * Range.pos -> string []


namespace FSharp.Compiler.SourceCodeServices
  type ShortIdent = string
  type Idents = ShortIdent []
  type MaybeUnresolvedIdent =
    { Ident: ShortIdent
      Resolved: bool }
  type MaybeUnresolvedIdents = MaybeUnresolvedIdent []
  type IsAutoOpen = bool
  module Extensions =
    type FSharpEntity with
      member TryGetFullName: unit -> string option
    type FSharpEntity with
      member TryGetFullDisplayName: unit -> string option
    type FSharpEntity with
      member TryGetFullCompiledName: unit -> string option
    type FSharpEntity with
      member PublicNestedEntities: seq<FSharpEntity>
    type FSharpEntity with
      member
        TryGetMembersFunctionsAndValues: System.Collections.Generic.IList<FSharpMemberOrFunctionOrValue>
    type FSharpMemberOrFunctionOrValue with
      member FullTypeSafe: FSharpType option
    type FSharpMemberOrFunctionOrValue with
      member TryGetFullDisplayName: unit -> string option
    type FSharpMemberOrFunctionOrValue with
      member TryGetFullCompiledOperatorNameIdents: unit -> Idents option
    type FSharpAssemblySignature with
      member TryGetEntities: unit -> seq<FSharpEntity>

  [<RequireQualifiedAccessAttribute>]
  type LookupType =
    | Fuzzy
    | Precise
  [<NoComparison (); NoEqualityAttribute>]
  type AssemblySymbol =
    { FullName: string
      CleanedIdents: Idents
      Namespace: Idents option
      NearestRequireQualifiedAccessParent: Idents option
      TopRequireQualifiedAccessParent: Idents option
      AutoOpenParent: Idents option
      Symbol: FSharpSymbol
      Kind: LookupType -> EntityKind
      UnresolvedSymbol: UnresolvedSymbol }
    with
      override ToString: unit -> string
  
  type AssemblyPath = string
  type AssemblyContentType =
    | Public
    | Full
  type Parent =
    { Namespace: Idents option
      ThisRequiresQualifiedAccess: bool -> Idents option
      TopRequiresQualifiedAccess: bool -> Idents option
      AutoOpen: Idents option
      WithModuleSuffix: Idents option
      IsModule: bool }
    with
      static member
        RewriteParentIdents: parentIdents:Idents option ->
                                idents:Idents -> Idents
      member FixParentModuleSuffix: idents:Idents -> Idents
      member
        FormatEntityFullName: entity:FSharpEntity ->
                                 (string * ShortIdent []) option
      static member Empty: Parent
  
  type AssemblyContentCacheEntry =
    { FileWriteTime: System.DateTime
      ContentType: AssemblyContentType
      Symbols: AssemblySymbol list }
  [<NoComparison (); NoEqualityAttribute>]
  type IAssemblyContentCache =
    interface
      abstract member Set: AssemblyPath -> AssemblyContentCacheEntry -> unit
      abstract member TryGet: AssemblyPath -> AssemblyContentCacheEntry option
  
  module AssemblyContentProvider =
    val unresolvedSymbol:
      topRequireQualifiedAccessParent:Idents option ->
        cleanedIdents:Idents -> fullName:string -> UnresolvedSymbol
    val createEntity:
      ns:Idents option ->
        parent:Parent -> entity:FSharpEntity -> AssemblySymbol option
    val traverseMemberFunctionAndValues:
      ns:Idents option ->
        parent:Parent ->
          membersFunctionsAndValues:seq<FSharpMemberOrFunctionOrValue> ->
            seq<AssemblySymbol>
    val traverseEntity:
      contentType:AssemblyContentType ->
        parent:Parent -> entity:FSharpEntity -> seq<AssemblySymbol>
    val getAssemblySignatureContent:
      AssemblyContentType -> FSharpAssemblySignature -> AssemblySymbol list
    val getAssemblySignaturesContent:
      contentType:AssemblyContentType ->
        assemblies:FSharpAssembly list -> AssemblySymbol list
    val getAssemblyContent:
      withCache:((IAssemblyContentCache -> AssemblySymbol list) ->
                   AssemblySymbol list) ->
        contentType:AssemblyContentType ->
          fileName:string option ->
            assemblies:FSharpAssembly list -> AssemblySymbol list

  type EntityCache =

      interface IAssemblyContentCache
      new: unit -> EntityCache
      member Clear: unit -> unit
      member Locking: (IAssemblyContentCache -> 'T) -> 'T
  
  type StringLongIdent = string
  type Entity =
    { FullRelativeName: StringLongIdent
      Qualifier: StringLongIdent
      Namespace: StringLongIdent option
      Name: StringLongIdent
      LastIdent: string }
    with
      override ToString: unit -> string
  
  module Entity =
    val getRelativeNamespace: targetNs:Idents -> sourceNs:Idents -> Idents
    val cutAutoOpenModules:
      autoOpenParent:Idents option -> candidateNs:Idents -> ShortIdent []
    val tryCreate:
      targetNamespace:Idents option * targetScope:Idents *
      partiallyQualifiedName:MaybeUnresolvedIdents *
      requiresQualifiedAccessParent:Idents option * autoOpenParent:Idents option *
      candidateNamespace:Idents option * candidate:Idents -> Entity []

  type ScopeKind =
    | Namespace
    | TopModule
    | NestedModule
    | OpenDeclaration
    | HashDirective
    with
      override ToString: unit -> string
  
  type InsertContext =
    { ScopeKind: ScopeKind
      Pos: Range.pos }
  type Module =
    { Idents: Idents
      Range: Range.range }
  type OpenStatementInsertionPoint =
    | TopLevel
    | Nearest
  module ParsedInput =
    val ( |Sequentials|_| ):
      _arg1:SyntaxTree.SynExpr -> SyntaxTree.SynExpr list option
    val ( |ConstructorPats| ):
      _arg1:SyntaxTree.SynArgPats -> SyntaxTree.SynPat list
    val getLongIdents:
      input:SyntaxTree.ParsedInput option ->
        System.Collections.Generic.IDictionary<Range.pos,SyntaxTree.LongIdent>
    val getLongIdentAt:
      ast:SyntaxTree.ParsedInput -> pos:Range.pos -> SyntaxTree.LongIdent option
    type Scope =
      { Idents: Idents
        Kind: ScopeKind }
    val tryFindNearestPointAndModules:
      currentLine:int ->
        ast:SyntaxTree.ParsedInput ->
          insertionPoint:OpenStatementInsertionPoint ->
            (Scope * string [] option * Range.pos) option * Module list
    val findBestPositionToInsertOpenDeclaration:
      modules:Module list ->
        scope:Scope -> pos:Range.pos -> entity:Idents -> InsertContext
    val tryFindInsertionContext:
      currentLine:int ->
        ast:SyntaxTree.ParsedInput ->
          MaybeUnresolvedIdents ->
            insertionPoint:OpenStatementInsertionPoint ->
              (Idents option * Idents option * Idents option * Idents ->
                 (Entity * InsertContext) [])
    val adjustInsertionPoint:
      getLineStr:(int -> string) -> ctx:InsertContext -> Range.pos
    val findNearestPointToInsertOpenDeclaration:
      currentLine:int ->
        ast:SyntaxTree.ParsedInput ->
          entity:Idents ->
            insertionPoint:OpenStatementInsertionPoint -> InsertContext


namespace FSharp.Compiler.SourceCodeServices
  type XmlDocable =
    | XmlDocable of line: int * indent: int * paramNames: string list
  module XmlDocParsing =
    val ( |ConstructorPats| ):
      _arg1:SyntaxTree.SynArgPats -> SyntaxTree.SynPat list
    val digNamesFrom: pat:SyntaxTree.SynPat -> string list
    val getXmlDocablesImpl:
      sourceText:Text.ISourceText * input:SyntaxTree.ParsedInput option ->
        XmlDocable list

  module XmlDocComment =
    val private ws: s:string * pos:int -> (string * int) option
    val private str:
      prefix:string -> s:string * pos:int -> (string * int) option
    val private eol: s:string * pos:'a -> (string * 'a) option
    val inline private ( >=> ):
      f:('a -> 'b option) -> g:('b -> 'c option) -> ('a -> 'c option)
    val isBlank: string -> int option

  module XmlDocParser =
    val getXmlDocables:
      Text.ISourceText * input:SyntaxTree.ParsedInput option -> XmlDocable list


namespace FSharp.Compiler.SourceCodeServices
  module private Option =
    val ofOptionList: xs:'a option list -> 'a list option

  [<RequireQualifiedAccessAttribute>]
  type ExternalType =
    | Type of fullName: string * genericArgs: ExternalType list
    | Array of inner: ExternalType
    | Pointer of inner: ExternalType
    | TypeVar of typeName: string
    with
      override ToString: unit -> string
  
  module ExternalType =
    val internal tryOfILType:
      string array -> AbstractIL.IL.ILType -> ExternalType option

  [<RequireQualifiedAccessAttribute>]
  type ParamTypeSymbol =
    | Param of ExternalType
    | Byref of ExternalType
    with
      override ToString: unit -> string
  
  module ParamTypeSymbol =
    val internal tryOfILType:
      string array -> AbstractIL.IL.ILType -> ParamTypeSymbol option
    val internal tryOfILTypes:
      string array -> AbstractIL.IL.ILType list -> ParamTypeSymbol list option

  [<RequireQualifiedAccessAttribute ();
    System.Diagnostics.DebuggerDisplay ("{ToDebuggerDisplay(),nq}")>]
  type ExternalSymbol =
    | Type of fullName: string
    | Constructor of typeName: string * args: ParamTypeSymbol list
    | Method of
      typeName: string * name: string * paramSyms: ParamTypeSymbol list *
      genericArity: int
    | Field of typeName: string * name: string
    | Event of typeName: string * name: string
    | Property of typeName: string * name: string
    with
      member internal ToDebuggerDisplay: unit -> string
      override ToString: unit -> string
  

namespace FSharp.Compiler
  type PartialLongName =
    { QualifyingIdents: string list
      PartialIdent: string
      EndColumn: int
      LastDotPos: int option }
    with
      static member Empty: endColumn:int -> PartialLongName
  
  module QuickParse =
    val MagicalAdjustmentConstant: int
    val CorrectIdentifierToken: tokenText:string -> tokenTag:int -> int
    val isValidStrippedName: name:System.ReadOnlySpan<char> -> idx:int -> bool
    val private isValidActivePatternName: name:string -> bool
    val GetCompleteIdentifierIslandImpl:
      lineStr:string -> index:int -> (string * int * bool) option
    val GetCompleteIdentifierIsland:
      tolerateJustAfter:bool ->
        tokenText:string -> index:int -> (string * int * bool) option
    val private defaultName: 'a list * string
    val GetPartialLongName: lineStr:string * index:int -> string list * string
    type private EatCommentCallContext =
      | SkipWhiteSpaces of
        ident: string * current: string list * throwAwayNext: bool
      | StartIdentifier of current: string list * throwAway: bool
    val GetPartialLongNameEx: lineStr:string * index:int -> PartialLongName
    val TokenNameEquals:
      tokenInfo:SourceCodeServices.FSharpTokenInfo -> token2:string -> bool
    val private expected: string [] list
    val TestMemberOrOverrideDeclaration:
      tokens:SourceCodeServices.FSharpTokenInfo [] -> bool


namespace FSharp.Compiler.SourceCodeServices
  module internal FSharpCheckerResultsSettings =
    val getToolTipTextSize: int
    val maxTypeCheckErrorsOutOfProjectContext: int
    val maxTimeShareMilliseconds: int64
    val defaultFSharpBinariesDir: string

  [<RequireQualifiedAccessAttribute>]
  type FSharpFindDeclFailureReason =
    | Unknown of message: string
    | NoSourceCode
    | ProvidedType of string
    | ProvidedMember of string
  [<RequireQualifiedAccessAttribute>]
  type FSharpFindDeclResult =
    | DeclNotFound of FSharpFindDeclFailureReason
    | DeclFound of Range.range
    | ExternalDecl of assembly: string * externalSym: ExternalSymbol
  [<RequireQualifiedAccessAttribute (); NoEquality;
    NoComparison>]
  type internal NameResResult =
    | Members of
      (NameResolution.ItemWithInst list * TypedTreeOps.DisplayEnv * Range.range)
    | Cancel of TypedTreeOps.DisplayEnv * Range.range
    | Empty
  [<RequireQualifiedAccessAttribute>]
  type ResolveOverloads =
    | Yes
    | No
  [<RequireQualifiedAccessAttribute>]
  type GetPreciseCompletionListFromExprTypingsResult =
    | NoneBecauseTypecheckIsStaleAndTextChanged
    | NoneBecauseThereWereTypeErrors
    | None
    | Some of
      (NameResolution.ItemWithInst list * TypedTreeOps.DisplayEnv * Range.range) *
      TypedTree.TType
  type Names = string list
  [<SealedAttribute>]
  type internal TypeCheckInfo =

      new: _sTcConfig:CompilerConfig.TcConfig * g:TcGlobals.TcGlobals *
            ccuSigForFile:TypedTree.ModuleOrNamespaceType *
            thisCcu:TypedTree.CcuThunk * tcImports:CompilerImports.TcImports *
            tcAccessRights:AccessibilityLogic.AccessorDomain *
            projectFileName:string * mainInputFileName:string *
            sResolutions:NameResolution.TcResolutions *
            sSymbolUses:NameResolution.TcSymbolUses *
            sFallback:NameResolution.NameResolutionEnv *
            loadClosure:ScriptClosure.LoadClosure option *
            implFileOpt:TypedTree.TypedImplFile option *
            openDeclarations:NameResolution.OpenDeclaration [] -> TypeCheckInfo
      member
        GetBestDisplayEnvForPos: cursorPos:Range.pos ->
                                    (NameResolution.NameResolutionEnv *
                                     AccessibilityLogic.AccessorDomain) *
                                    Range.range
      member
        GetDeclarationListSymbols: parseResultsOpt:FSharpParseFileResults option *
                                    line:int * lineStr:string *
                                    partialName:PartialLongName *
                                    getAllEntities:(unit -> AssemblySymbol list) ->
                                      FSharpSymbolUse list list
      member
        GetDeclarationLocation: line:int * lineStr:string * colAtEndOfNames:int *
                                 names:string list * preferFlag:bool option ->
                                   FSharpFindDeclResult
      member
        GetDeclarations: parseResultsOpt:FSharpParseFileResults option *
                          line:int * lineStr:string *
                          partialName:PartialLongName *
                          getAllEntities:(unit -> AssemblySymbol list) ->
                            FSharpDeclarationListInfo
      member
        GetF1Keyword: line:int * lineStr:string * colAtEndOfNames:int *
                       names:string list -> string option
      member
        GetFormatSpecifierLocationsAndArity: unit -> (Range.range * int) []
      member
        GetMethods: line:int * lineStr:string * colAtEndOfNames:int *
                     namesOpt:string list option -> FSharpMethodGroup
      member
        GetMethodsAsSymbols: line:int * lineStr:string * colAtEndOfNames:int *
                              names:string list ->
                                (FSharpSymbol list * TypedTreeOps.DisplayEnv *
                                 Range.range) option
      member
        GetReferenceResolutionStructuredToolTipText: line:int * col:int ->
                                                        FSharpToolTipText<Layout>
      member GetReferencedAssemblies: unit -> FSharpAssembly list
      member
        GetSemanticClassification: range:Range.range option ->
                                
                                        (Range.range *
                                         SemanticClassificationType) []
      member
        GetStructuredToolTipText: line:int * lineStr:string *
                                   colAtEndOfNames:int * names:string list ->
                                     FSharpToolTipText<Layout>
      member
        GetSymbolUseAtLocation: line:int * lineStr:string * colAtEndOfNames:int *
                                 names:string list ->
                                   (FSharpSymbol * TypedTreeOps.DisplayEnv *
                                    Range.range) option
      member
        GetVisibleNamespacesAndModulesAtPosition: cursorPos:Range.pos ->
                                                     TypedTree.ModuleOrNamespaceRef list
      member
        IsRelativeNameResolvable: cursorPos:Range.pos * plid:string list *
                                   item:NameResolution.Item -> bool
      member
        IsRelativeNameResolvableFromSymbol: cursorPos:Range.pos *
                                             plid:string list *
                                             symbol:FSharpSymbol -> bool
      override ToString: unit -> string
      member AccessRights: AccessibilityLogic.AccessorDomain
      member CcuSigForFile: TypedTree.ModuleOrNamespaceType
      member ImplementationFile: TypedTree.TypedImplFile option
      member OpenDeclarations: NameResolution.OpenDeclaration []
      member PartialAssemblySignatureForFile: FSharpAssemblySignature
      member ScopeResolutions: NameResolution.TcResolutions
      member ScopeSymbolUses: NameResolution.TcSymbolUses
      member SymbolEnv: SymbolEnv
      member TcGlobals: TcGlobals.TcGlobals
      member TcImports: CompilerImports.TcImports
      member ThisCcu: TypedTree.CcuThunk
  
  type FSharpParsingOptions =
    { SourceFiles: string []
      ConditionalCompilationDefines: string list
      ErrorSeverityOptions: ErrorLogger.FSharpErrorSeverityOptions
      IsInteractive: bool
      LightSyntax: bool option
      CompilingFsLib: bool
      IsExe: bool }
    with
      static member
        FromTcConfig: tcConfig:CompilerConfig.TcConfig * sourceFiles:string [] *
                       isInteractive:bool -> FSharpParsingOptions
      static member
        FromTcConfigBuilder: tcConfigB:CompilerConfig.TcConfigBuilder *
                              sourceFiles:string [] * isInteractive:bool ->
                                FSharpParsingOptions
      member LastFileName: string
      static member Default: FSharpParsingOptions
  
  module internal ParseAndCheckFile =
    type ErrorHandler =
  
        new: reportErrors:bool * mainInputFileName:string *
              errorSeverityOptions:ErrorLogger.FSharpErrorSeverityOptions *
              sourceText:Text.ISourceText * suggestNamesForErrors:bool ->
                ErrorHandler
        member AnyErrors: bool
        member CollectedDiagnostics: FSharpErrorInfo []
        member ErrorCount: int
        member ErrorLogger: ErrorLogger.ErrorLogger
        member
          ErrorSeverityOptions: ErrorLogger.FSharpErrorSeverityOptions with set
    
    val getLightSyntaxStatus:
      fileName:string ->
        options:FSharpParsingOptions -> Lexhelp.LightSyntaxStatus
    val createLexerFunction:
      fileName:string ->
        options:FSharpParsingOptions ->
          lexbuf:Internal.Utilities.Text.Lexing.LexBuffer<char> ->
            errHandler:ErrorHandler -> ('a -> Parser.token)
    val isFeatureSupported: _featureId:Features.LanguageFeature -> bool
    val createLexbuf: sourceText:Text.ISourceText -> UnicodeLexing.Lexbuf
    val matchBraces:
      sourceText:Text.ISourceText * fileName:string *
      options:FSharpParsingOptions * userOpName:string *
      suggestNamesForErrors:bool -> (Range.range * Range.range) []
    val parseFile:
      sourceText:Text.ISourceText * fileName:string *
      options:FSharpParsingOptions * userOpName:string *
      suggestNamesForErrors:bool ->
        FSharpErrorInfo [] * SyntaxTree.ParsedInput option * bool
    val ApplyLoadClosure:
      tcConfig:CompilerConfig.TcConfig * parsedMainInput:SyntaxTree.ParsedInput *
      mainInputFileName:string * loadClosure:ScriptClosure.LoadClosure option *
      tcImports:CompilerImports.TcImports *
      backgroundDiagnostics:(ErrorLogger.PhasedDiagnostic * FSharpErrorSeverity) [] ->
        unit
    val CheckOneFile:
      parseResults:FSharpParseFileResults * sourceText:Text.ISourceText *
      mainInputFileName:string * projectFileName:string *
      tcConfig:CompilerConfig.TcConfig * tcGlobals:TcGlobals.TcGlobals *
      tcImports:CompilerImports.TcImports * tcState:ParseAndCheckInputs.TcState *
      moduleNamesDict:ParseAndCheckInputs.ModuleNamesDict *
      loadClosure:ScriptClosure.LoadClosure option *
      backgroundDiagnostics:(ErrorLogger.PhasedDiagnostic * FSharpErrorSeverity) [] *
      reactorOps:IReactorOperations * userOpName:string *
      suggestNamesForErrors:bool ->
        Async<FSharpErrorInfo [] * Result<TypeCheckInfo,unit>>

  [<SealedAttribute>]
  type FSharpProjectContext =

      new: thisCcu:TypedTree.CcuThunk * assemblies:FSharpAssembly list *
            ad:AccessibilityLogic.AccessorDomain -> FSharpProjectContext
      member GetReferencedAssemblies: unit -> FSharpAssembly list
      member AccessibilityRights: FSharpAccessibilityRights
  
  [<SealedAttribute>]
  type FSharpCheckFileResults =

      new: filename:string * errors:FSharpErrorInfo [] *
            scopeOptX:TypeCheckInfo option * dependencyFiles:string [] *
            builderX:IncrementalBuilder option * keepAssemblyContents:bool ->
              FSharpCheckFileResults
      static member
        CheckOneFile: parseResults:FSharpParseFileResults *
                       sourceText:Text.ISourceText * mainInputFileName:string *
                       projectFileName:string * tcConfig:CompilerConfig.TcConfig *
                       tcGlobals:TcGlobals.TcGlobals *
                       tcImports:CompilerImports.TcImports *
                       tcState:ParseAndCheckInputs.TcState *
                       moduleNamesDict:ParseAndCheckInputs.ModuleNamesDict *
                       loadClosure:ScriptClosure.LoadClosure option *
                       backgroundDiagnostics:(ErrorLogger.PhasedDiagnostic *
                                              FSharpErrorSeverity) [] *
                       reactorOps:IReactorOperations * userOpName:string *
                       isIncompleteTypeCheckEnvironment:bool *
                       builder:IncrementalBuilder * dependencyFiles:string [] *
                       creationErrors:FSharpErrorInfo [] *
                       parseErrors:FSharpErrorInfo [] *
                       keepAssemblyContents:bool * suggestNamesForErrors:bool ->
                         Async<FSharpCheckFileAnswer>
      static member
        JoinErrors: isIncompleteTypeCheckEnvironment:bool *
                     creationErrors:FSharpErrorInfo [] *
                     parseErrors:FSharpErrorInfo [] *
                     tcErrors:FSharpErrorInfo [] -> FSharpErrorInfo []
      static member
        Make: mainInputFileName:string * projectFileName:string *
               tcConfig:CompilerConfig.TcConfig * tcGlobals:TcGlobals.TcGlobals *
               isIncompleteTypeCheckEnvironment:bool *
               builder:IncrementalBuilder * dependencyFiles:string [] *
               creationErrors:FSharpErrorInfo [] *
               parseErrors:FSharpErrorInfo [] * tcErrors:FSharpErrorInfo [] *
               keepAssemblyContents:bool *
               ccuSigForFile:TypedTree.ModuleOrNamespaceType *
               thisCcu:TypedTree.CcuThunk * tcImports:CompilerImports.TcImports *
               tcAccessRights:AccessibilityLogic.AccessorDomain *
               sResolutions:NameResolution.TcResolutions *
               sSymbolUses:NameResolution.TcSymbolUses *
               sFallback:NameResolution.NameResolutionEnv *
               loadClosure:ScriptClosure.LoadClosure option *
               implFileOpt:TypedTree.TypedImplFile option *
               openDeclarations:NameResolution.OpenDeclaration [] ->
                 FSharpCheckFileResults
      static member
        MakeEmpty: filename:string * creationErrors:FSharpErrorInfo [] *
                    keepAssemblyContents:bool -> FSharpCheckFileResults
      member
        GetAllUsesOfAllSymbolsInFile: ?cancellationToken:System.Threading.CancellationToken ->
                                         seq<FSharpSymbolUse>
      member
        GetDeclarationListInfo: parsedFileResults:FSharpParseFileResults option *
                                 line:int * lineText:string *
                                 partialName:PartialLongName *
                                 ?getAllEntities:(unit -> AssemblySymbol list) ->
                                   FSharpDeclarationListInfo
      member
        GetDeclarationListSymbols: parsedFileResults:FSharpParseFileResults option *
                                    line:int * lineText:string *
                                    partialName:PartialLongName *
                                    ?getAllEntities:(unit -> AssemblySymbol list) ->
                                      FSharpSymbolUse list list
      member
        GetDeclarationLocation: line:int * colAtEndOfNames:int *
                                 lineText:string * names:string list *
                                 ?preferFlag:bool -> FSharpFindDeclResult
      member
        GetDisplayContextForPos: cursorPos:Range.pos ->
                                    FSharpDisplayContext option
      member
        GetF1Keyword: line:int * colAtEndOfNames:int * lineText:string *
                       names:string list -> string option
      [<System.Obsolete
        ("This member has been replaced by GetFormatSpecifierLocationsAndArity, which returns both range and arity of specifiers")>]
      member GetFormatSpecifierLocations: unit -> Range.range []
      member
        GetFormatSpecifierLocationsAndArity: unit -> (Range.range * int) []
      member
        GetMethods: line:int * colAtEndOfNames:int * lineText:string *
                     names:string list option -> FSharpMethodGroup
      member
        GetMethodsAsSymbols: line:int * colAtEndOfNames:int * lineText:string *
                              names:string list -> FSharpSymbolUse list option
      member
        GetSemanticClassification: Range.range option ->
                                
                                        (Range.range *
                                         SemanticClassificationType) []
      member
        GetStructuredToolTipText: line:int * colAtEndOfNames:int *
                                   lineText:string * names:string list *
                                   tokenTag:int -> FSharpToolTipText<Layout>
      member
        GetSymbolAtLocation: line:int * colAtEndOfNames:int * lineStr:string *
                              names:string list -> FSharpSymbol option
      member
        GetSymbolUseAtLocation: line:int * colAtEndOfNames:int *
                                 lineText:string * names:string list ->
                                   FSharpSymbolUse option
      member
        GetToolTipText: line:int * colAtEndOfNames:int * lineText:string *
                         names:string list * tokenTag:int -> FSharpToolTipText
      member
        GetUsesOfSymbolInFile: symbol:FSharpSymbol *
                                ?cancellationToken:System.Threading.CancellationToken ->
                                  FSharpSymbolUse []
      member
        GetVisibleNamespacesAndModulesAtPoint: Range.pos ->
                                                  TypedTree.ModuleOrNamespaceRef []
      member
        IsRelativeNameResolvable: cursorPos:Range.pos * plid:string list *
                                   item:NameResolution.Item -> bool
      member
        IsRelativeNameResolvableFromSymbol: cursorPos:Range.pos *
                                             plid:string list *
                                             symbol:FSharpSymbol -> bool
      override ToString: unit -> string
      member TryGetCurrentTcImports: unit -> CompilerImports.TcImports option
      member DependencyFiles: string []
      member Errors: FSharpErrorInfo []
      member HasFullTypeCheckInfo: bool
      member ImplementationFile: FSharpImplementationFileContents option
      member OpenDeclarations: FSharpOpenDeclaration []
      member PartialAssemblySignature: FSharpAssemblySignature
      member ProjectContext: FSharpProjectContext
  
  [<RequireQualifiedAccessAttribute (); NoComparison>]
  and FSharpCheckFileAnswer =
    | Aborted
    | Succeeded of FSharpCheckFileResults
  [<SealedAttribute>]
  type FSharpCheckProjectResults =

      new: projectFileName:string *
            tcConfigOption:CompilerConfig.TcConfig option *
            keepAssemblyContents:bool * errors:FSharpErrorInfo [] *
            details:(TcGlobals.TcGlobals * CompilerImports.TcImports *
                     TypedTree.CcuThunk * TypedTree.ModuleOrNamespaceType *
                     NameResolution.TcSymbolUses list *
                     CheckDeclarations.TopAttribs option *
                     CompilerConfig.IRawFSharpAssemblyData option *
                     AbstractIL.IL.ILAssemblyRef *
                     AccessibilityLogic.AccessorDomain *
                     TypedTree.TypedImplFile list option * string []) option ->
              FSharpCheckProjectResults
      member
        GetAllUsesOfAllSymbols: ?cancellationToken:System.Threading.CancellationToken ->
                                   FSharpSymbolUse []
      member GetOptimizedAssemblyContents: unit -> FSharpAssemblyContents
      member
        GetUsesOfSymbol: symbol:FSharpSymbol *
                          ?cancellationToken:System.Threading.CancellationToken ->
                            FSharpSymbolUse []
      override ToString: unit -> string
      member AssemblyContents: FSharpAssemblyContents
      member AssemblyFullName: string
      member AssemblySignature: FSharpAssemblySignature
      member DependencyFiles: string []
      member Errors: FSharpErrorInfo []
      member HasCriticalErrors: bool
      member ProjectContext: FSharpProjectContext
      member
        RawFSharpAssemblyData: CompilerConfig.IRawFSharpAssemblyData option
      member
        TypedImplementationFiles: TcGlobals.TcGlobals * TypedTree.CcuThunk *
                                   CompilerImports.TcImports *
                                   TypedTree.TypedImplFile list
  
  type FsiInteractiveChecker =

      new: ReferenceResolver.Resolver * ops:IReactorOperations *
            tcConfig:CompilerConfig.TcConfig * tcGlobals:TcGlobals.TcGlobals *
            tcImports:CompilerImports.TcImports *
            tcState:ParseAndCheckInputs.TcState -> FsiInteractiveChecker
      member
        ParseAndCheckInteraction: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                                   sourceText:Text.ISourceText *
                                   ?userOpName:string ->
                                     Async<FSharpParseFileResults *
                                           FSharpCheckFileResults *
                                           FSharpCheckProjectResults>
  

namespace FSharp.Compiler.SourceCodeServices
  type internal Layout = Internal.Utilities.StructuredFormat.Layout
  module EnvMisc =
    val braceMatchCacheSize: int
    val parseFileCacheSize: int
    val checkFileInProjectCacheSize: int
    val projectCacheSizeDefault: int
    val frameworkTcImportsCacheStrongSize: int
    val maxMBDefault: int

  type UnresolvedReferencesSet =
    | UnresolvedReferencesSet of CompilerConfig.UnresolvedAssemblyReference list
  type FSharpProjectOptions =
    { ProjectFileName: string
      ProjectId: string option
      SourceFiles: string []
      OtherOptions: string []
      ReferencedProjects: (string * FSharpProjectOptions) []
      IsIncompleteTypeCheckEnvironment: bool
      UseScriptResolutionRules: bool
      LoadTime: System.DateTime
      UnresolvedReferences: UnresolvedReferencesSet option
      OriginalLoadReferences: (Range.range * string * string) list
      ExtraProjectInfo: obj option
      Stamp: int64 option }
    with
      static member
        AreSameForChecking: options1:FSharpProjectOptions *
                             options2:FSharpProjectOptions -> bool
      static member
        UseSameProject: options1:FSharpProjectOptions *
                         options2:FSharpProjectOptions -> bool
      override ToString: unit -> string
      member ProjectDirectory: string
      member ProjectOptions: string []
  
  [<NoComparison (); NoEqualityAttribute>]
  type IsResultObsolete = | IsResultObsolete of (unit -> bool)
  module Helpers =
    val AreSameForChecking2:
      (string * FSharpProjectOptions) * (string * FSharpProjectOptions) -> bool
    val AreSubsumable2:
      (string * FSharpProjectOptions) * (string * FSharpProjectOptions) -> bool
    val AreSameForParsing:
      (string * int * 'a) * (string * int * 'a) -> bool when 'a: equality
    val AreSimilarForParsing:
      ('a * 'b * 'c) * ('a * 'd * 'e) -> bool when 'a: equality
    val AreSameForChecking3:
      (string * int * FSharpProjectOptions) *
      (string * int * FSharpProjectOptions) -> bool
    val AreSubsumable3:
      (string * 'a * FSharpProjectOptions) *
      (string * 'b * FSharpProjectOptions) -> bool

  module CompileHelpers =
    val mkCompilationErrorHandlers:
      unit ->
        ResizeArray<FSharpErrorInfo> * ErrorLogger.ErrorLogger *
        Driver.ErrorLoggerProvider
    val tryCompile:
      errorLogger:ErrorLogger.ErrorLogger ->
        f:(ErrorLogger.Exiter -> unit) -> int
    val compileFromArgs:
      ctok:AbstractIL.Internal.Library.CompilationThreadToken * argv:string [] *
      legacyReferenceResolver:ReferenceResolver.Resolver *
      tcImportsCapture:(CompilerImports.TcImports -> unit) option *
      dynamicAssemblyCreator:(TcGlobals.TcGlobals * string *
                              AbstractIL.IL.ILModuleDef -> unit) option ->
        FSharpErrorInfo [] * int
    val compileFromAsts:
      ctok:AbstractIL.Internal.Library.CompilationThreadToken *
      legacyReferenceResolver:ReferenceResolver.Resolver *
      asts:SyntaxTree.ParsedInput list * assemblyName:string * outFile:string *
      dependencies:string list * noframework:bool * pdbFile:string option *
      executable:bool option *
      tcImportsCapture:(CompilerImports.TcImports -> unit) option *
      dynamicAssemblyCreator:(TcGlobals.TcGlobals * string *
                              AbstractIL.IL.ILModuleDef -> unit) option ->
        FSharpErrorInfo [] * int
    val createDynamicAssembly:
      ctok:AbstractIL.Internal.Library.CompilationThreadToken * debugInfo:bool *
      tcImportsRef:CompilerImports.TcImports option ref * execute:bool *
      assemblyBuilderRef:System.Reflection.Emit.AssemblyBuilder option ref ->
        tcGlobals:TcGlobals.TcGlobals * outfile:string *
        ilxMainModule:AbstractIL.IL.ILModuleDef -> unit
    val setOutputStreams:
      execute:(#System.IO.TextWriter * #System.IO.TextWriter) option -> unit

  type SourceTextHash = int
  type FileName = string
  type FilePath = string
  type ProjectPath = string
  type FileVersion = int
  type ParseCacheLockToken =

      interface AbstractIL.Internal.Library.LockToken
      new: unit -> ParseCacheLockToken
  
  type ScriptClosureCacheToken =

      interface AbstractIL.Internal.Library.LockToken
      new: unit -> ScriptClosureCacheToken
  
  type BackgroundCompiler =

      new: legacyReferenceResolver:ReferenceResolver.Resolver *
            projectCacheSize:int * keepAssemblyContents:bool *
            keepAllBackgroundResolutions:bool *
            tryGetMetadataSnapshot:AbstractIL.ILBinaryReader.ILReaderTryGetMetadataSnapshot *
            suggestNamesForErrors:bool * keepAllBackgroundSymbolUses:bool *
            enableBackgroundItemKeyStoreAndSemanticClassification:bool *
            enablePartialTypeChecking:bool -> BackgroundCompiler
      member
        CheckFileInProject: parseResults:FSharpParseFileResults *
                             filename:string * fileVersion:int *
                             sourceText:Text.ISourceText *
                             options:FSharpProjectOptions * userOpName:string ->
                               Async<FSharpCheckFileAnswer>
      member
        CheckFileInProjectAllowingStaleCachedResults: parseResults:FSharpParseFileResults *
                                                       filename:string *
                                                       fileVersion:int *
                                                       sourceText:Text.ISourceText *
                                                       options:FSharpProjectOptions *
                                                       userOpName:string ->
                                                         Async<FSharpCheckFileAnswer option>
      member
        private CheckOneFileImpl: parseResults:FSharpParseFileResults *
                                   sourceText:Text.ISourceText * fileName:string *
                                   options:FSharpProjectOptions *
                                   fileVersion:int * builder:IncrementalBuilder *
                                   tcConfig:CompilerConfig.TcConfig *
                                   tcGlobals:TcGlobals.TcGlobals *
                                   tcImports:CompilerImports.TcImports *
                                   tcDependencyFiles:string list *
                                   timeStamp:System.DateTime *
                                   prevTcState:ParseAndCheckInputs.TcState *
                                   prevModuleNamesDict:ParseAndCheckInputs.ModuleNamesDict *
                                   prevTcErrors:(ErrorLogger.PhasedDiagnostic *
                                                 FSharpErrorSeverity) [] *
                                   creationErrors:FSharpErrorInfo [] *
                                   userOpName:string ->
                                     Async<FSharpCheckFileAnswer>
      member
        CheckProjectInBackground: options:FSharpProjectOptions *
                                   userOpName:string -> unit
      member
        ClearCache: options:seq<FSharpProjectOptions> * userOpName:string ->
                       unit
      member ClearCachesAsync: userOpName:string -> Async<unit>
      member CompleteAllQueuedOps: unit -> unit
      member DownsizeCaches: userOpName:string -> Async<unit>
      member
        FindReferencesInFile: filename:string * options:FSharpProjectOptions *
                               symbol:FSharpSymbol * canInvalidateProject:bool *
                               userOpName:string -> Async<seq<Range.range>>
      member
        GetAssemblyData: options:FSharpProjectOptions *
                          ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                          userOpName:string ->
                            AbstractIL.Internal.Library.Cancellable<CompilerConfig.IRawFSharpAssemblyData option>
      member
        GetBackgroundCheckResultsForFileInProject: filename:string *
                                                    options:FSharpProjectOptions *
                                                    userOpName:string ->
                                                      Async<FSharpParseFileResults *
                                                            FSharpCheckFileResults>
      member
        GetBackgroundParseResultsForFileInProject: filename:string *
                                                    options:FSharpProjectOptions *
                                                    userOpName:string ->
                                                      Async<FSharpParseFileResults>
      member
        GetCachedCheckFileResult: builder:IncrementalBuilder *
                                   filename:FileName *
                                   sourceText:Text.ISourceText *
                                   options:FSharpProjectOptions ->
                                     (FSharpParseFileResults *
                                      FSharpCheckFileResults) option
      member
        GetProjectOptionsFromScript: filename:string *
                                      sourceText:Text.ISourceText *
                                      previewEnabled:bool option *
                                      loadedTimeStamp:System.DateTime option *
                                      otherFlags:string [] option *
                                      useFsiAuxLib:bool option *
                                      useSdkRefs:bool option *
                                      assumeDotNetFramework:bool option *
                                      extraProjectInfo:obj option *
                                      optionsStamp:int64 option *
                                      userOpName:string ->
                                        Async<FSharpProjectOptions *
                                              FSharpErrorInfo list>
      member
        GetSemanticClassificationForFile: filename:string *
                                           options:FSharpProjectOptions *
                                           userOpName:string ->
                                             Async<struct
                                                     (Range.range *
                                                      SemanticClassificationType) []>
      member
        ImplicitlyStartCheckProjectInBackground: options:FSharpProjectOptions *
                                                  userOpName:string -> unit
      member
        InvalidateConfiguration: options:FSharpProjectOptions *
                                  startBackgroundCompileIfAlreadySeen:bool option *
                                  userOpName:string -> unit
      member
        NotifyProjectCleaned: options:FSharpProjectOptions * userOpName:string ->
                                 Async<unit>
      member
        ParseAndCheckFileInProject: filename:string * fileVersion:int *
                                     sourceText:Text.ISourceText *
                                     options:FSharpProjectOptions *
                                     userOpName:string ->
                                       Async<FSharpParseFileResults *
                                             FSharpCheckFileAnswer>
      member
        ParseAndCheckProject: options:FSharpProjectOptions * userOpName:string ->
                                 Async<FSharpCheckProjectResults>
      member
        private ParseAndCheckProjectImpl: options:FSharpProjectOptions *
                                           ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                                           userOpName:string ->
                                             AbstractIL.Internal.Library.Cancellable<FSharpCheckProjectResults>
      member
        ParseFile: filename:string * sourceText:Text.ISourceText *
                    options:FSharpParsingOptions * userOpName:string ->
                      Async<FSharpParseFileResults>
      member
        ParseFileNoCache: filename:string * sourceText:Text.ISourceText *
                           options:FSharpParsingOptions * userOpName:string ->
                             Async<FSharpParseFileResults>
      member
        RecordTypeCheckFileInProjectResults: filename:string *
                                              options:FSharpProjectOptions *
                                              parsingOptions:FSharpParsingOptions *
                                              parseResults:FSharpParseFileResults *
                                              fileVersion:int *
                                              priorTimeStamp:System.DateTime *
                                              checkAnswer:FSharpCheckFileAnswer option *
                                              sourceText:SourceTextHash -> unit
      member StopBackgroundCompile: unit -> unit
      member
        private TryGetLogicalTimeStampForProject: cache:CompilerConfig.TimeStampCache *
                                                   ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                                                   options:FSharpProjectOptions *
                                                   userOpName:string ->
                                                     System.DateTime option
      member
        TryGetRecentCheckResultsForFile: filename:string *
                                          options:FSharpProjectOptions *
                                          sourceText:Text.ISourceText option *
                                          _userOpName:string ->
                                            (FSharpParseFileResults *
                                             FSharpCheckFileResults *
                                             FileVersion) option
      member WaitForBackgroundCompile: unit -> unit
      member BeforeBackgroundFileCheck: IEvent<string * obj option>
      member CurrentQueueLength: int
      member FileChecked: IEvent<string * obj option>
      member FileParsed: IEvent<string * obj option>
      member FrameworkImportsCache: FrameworkImportsCache
      member ImplicitlyStartBackgroundWork: bool
      member ProjectChecked: IEvent<string * obj option>
      member Reactor: Reactor
      member ReactorOps: IReactorOperations
      static member GlobalForegroundParseCountStatistic: int
      static member GlobalForegroundTypeCheckCountStatistic: int
  
  [<SealedAttribute>]
  type FSharpChecker =

      new: legacyReferenceResolver:ReferenceResolver.Resolver *
            projectCacheSize:int * keepAssemblyContents:bool *
            keepAllBackgroundResolutions:bool *
            tryGetMetadataSnapshot:AbstractIL.ILBinaryReader.ILReaderTryGetMetadataSnapshot *
            suggestNamesForErrors:bool * keepAllBackgroundSymbolUses:bool *
            enableBackgroundItemKeyStoreAndSemanticClassification:bool *
            enablePartialTypeChecking:bool -> FSharpChecker
      static member
        Create: ?projectCacheSize:int * ?keepAssemblyContents:bool *
                 ?keepAllBackgroundResolutions:bool *
                 ?legacyReferenceResolver:ReferenceResolver.Resolver *
                 ?tryGetMetadataSnapshot:(string * System.DateTime ->
                                            AbstractIL.ILBinaryReader.ILReaderMetadataSnapshot option) *
                 ?suggestNamesForErrors:bool * ?keepAllBackgroundSymbolUses:bool *
                 ?enableBackgroundItemKeyStoreAndSemanticClassification:bool *
                 ?enablePartialTypeChecking:bool -> FSharpChecker
      member
        CheckFileInProject: parseResults:FSharpParseFileResults *
                             filename:string * fileVersion:int *
                             sourceText:Text.ISourceText *
                             options:FSharpProjectOptions * ?userOpName:string ->
                               Async<FSharpCheckFileAnswer>
      [<System.Obsolete
        ("This member should no longer be used, please use 'CheckFileInProject'")>]
      member
        CheckFileInProjectAllowingStaleCachedResults: parseResults:FSharpParseFileResults *
                                                       filename:string *
                                                       fileVersion:int *
                                                       source:string *
                                                       options:FSharpProjectOptions *
                                                       ?userOpName:string ->
                                                         Async<FSharpCheckFileAnswer option>
      member CheckMaxMemoryReached: unit -> unit
      member
        CheckProjectInBackground: options:FSharpProjectOptions *
                                   ?userOpName:string -> unit
      member
        ClearCache: options:seq<FSharpProjectOptions> * ?userOpName:string ->
                       unit
      member ClearCaches: ?userOpName:string -> unit
      member ClearCachesAsync: ?userOpName:string -> Async<unit>
      member
        ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients: unit ->
                                                                             unit
      member
        Compile: argv:string [] * ?userOpName:string ->
                    Async<FSharpErrorInfo [] * int>
      member
        Compile: ast:SyntaxTree.ParsedInput list * assemblyName:string *
                  outFile:string * dependencies:string list * ?pdbFile:string *
                  ?executable:bool * ?noframework:bool * ?userOpName:string ->
                    Async<FSharpErrorInfo [] * int>
      member
        CompileToDynamicAssembly: otherFlags:string [] *
                                   execute:(System.IO.TextWriter *
                                            System.IO.TextWriter) option *
                                   ?userOpName:string ->
                                     Async<FSharpErrorInfo [] * int *
                                           System.Reflection.Assembly option>
      member
        CompileToDynamicAssembly: ast:SyntaxTree.ParsedInput list *
                                   assemblyName:string *
                                   dependencies:string list *
                                   execute:(System.IO.TextWriter *
                                            System.IO.TextWriter) option *
                                   ?debug:bool * ?noframework:bool *
                                   ?userOpName:string ->
                                     Async<FSharpErrorInfo [] * int *
                                           System.Reflection.Assembly option>
      member
        FindBackgroundReferencesInFile: filename:string *
                                         options:FSharpProjectOptions *
                                         symbol:FSharpSymbol *
                                         ?canInvalidateProject:bool *
                                         ?userOpName:string ->
                                           Async<seq<Range.range>>
      member
        GetBackgroundCheckResultsForFileInProject: filename:string *
                                                    options:FSharpProjectOptions *
                                                    ?userOpName:string ->
                                                      Async<FSharpParseFileResults *
                                                            FSharpCheckFileResults>
      member
        GetBackgroundParseResultsForFileInProject: filename:string *
                                                    options:FSharpProjectOptions *
                                                    ?userOpName:string ->
                                                      Async<FSharpParseFileResults>
      member
        GetBackgroundSemanticClassificationForFile: filename:string *
                                                     options:FSharpProjectOptions *
                                                     ?userOpName:string ->
                                                       Async<struct
                                                               (Range.range *
                                                                SemanticClassificationType) []>
      member
        GetParsingOptionsFromCommandLineArgs: argv:string list *
                                               ?isInteractive:bool ->
                                                 FSharpParsingOptions *
                                                 FSharpErrorInfo list
      member
        GetParsingOptionsFromCommandLineArgs: sourceFiles:string list *
                                               argv:string list *
                                               ?isInteractive:bool ->
                                                 FSharpParsingOptions *
                                                 FSharpErrorInfo list
      member
        GetParsingOptionsFromProjectOptions: options:FSharpProjectOptions ->
                                                FSharpParsingOptions *
                                                FSharpErrorInfo list
      member
        GetProjectOptionsFromCommandLineArgs: projectFileName:string *
                                               argv:string [] *
                                               ?loadedTimeStamp:System.DateTime *
                                               ?extraProjectInfo:obj ->
                                                 FSharpProjectOptions
      member
        GetProjectOptionsFromScript: filename:string * source:Text.ISourceText *
                                      ?previewEnabled:bool *
                                      ?loadedTimeStamp:System.DateTime *
                                      ?otherFlags:string [] * ?useFsiAuxLib:bool *
                                      ?useSdkRefs:bool *
                                      ?assumeDotNetFramework:bool *
                                      ?extraProjectInfo:obj *
                                      ?optionsStamp:int64 * ?userOpName:string ->
                                        Async<FSharpProjectOptions *
                                              FSharpErrorInfo list>
      member InvalidateAll: unit -> unit
      member
        InvalidateConfiguration: options:FSharpProjectOptions *
                                  ?startBackgroundCompile:bool *
                                  ?userOpName:string -> unit
      [<System.Obsolete
        ("Please pass FSharpParsingOptions to MatchBraces. If necessary generate FSharpParsingOptions from FSharpProjectOptions by calling checker.GetParsingOptionsFromProjectOptions(options)")>]
      member
        MatchBraces: filename:string * source:string *
                      options:FSharpProjectOptions * ?userOpName:string ->
                        Async<(Range.range * Range.range) []>
      member
        MatchBraces: filename:string * sourceText:Text.ISourceText *
                      options:FSharpParsingOptions * ?userOpName:string ->
                        Async<(Range.range * Range.range) []>
      member
        NotifyProjectCleaned: options:FSharpProjectOptions * ?userOpName:string ->
                                 Async<unit>
      member
        ParseAndCheckFileInProject: filename:string * fileVersion:int *
                                     sourceText:Text.ISourceText *
                                     options:FSharpProjectOptions *
                                     ?userOpName:string ->
                                       Async<FSharpParseFileResults *
                                             FSharpCheckFileAnswer>
      member
        ParseAndCheckProject: options:FSharpProjectOptions * ?userOpName:string ->
                                 Async<FSharpCheckProjectResults>
      member
        ParseFile: filename:string * sourceText:Text.ISourceText *
                    options:FSharpParsingOptions * ?userOpName:string ->
                      Async<FSharpParseFileResults>
      [<System.Obsolete
        ("Please call checker.ParseFile instead.  To do this, you must also pass FSharpParsingOptions instead of FSharpProjectOptions. If necessary generate FSharpParsingOptions from FSharpProjectOptions by calling checker.GetParsingOptionsFromProjectOptions(options)")>]
      member
        ParseFileInProject: filename:string * source:string *
                             options:FSharpProjectOptions * ?userOpName:string ->
                               Async<FSharpParseFileResults>
      member
        ParseFileNoCache: filename:string * sourceText:Text.ISourceText *
                           options:FSharpParsingOptions * ?userOpName:string ->
                             Async<FSharpParseFileResults>
      member
        StartBackgroundCompile: options:FSharpProjectOptions *
                                 ?userOpName:string -> unit
      member StopBackgroundCompile: unit -> unit
      member TokenizeFile: source:string -> FSharpTokenInfo [] []
      member
        TokenizeLine: line:string * state:FSharpTokenizerLexState ->
                         FSharpTokenInfo [] * FSharpTokenizerLexState
      member
        TryGetRecentCheckResultsForFile: filename:string *
                                          options:FSharpProjectOptions *
                                          ?sourceText:Text.ISourceText *
                                          ?userOpName:string ->
                                            (FSharpParseFileResults *
                                             FSharpCheckFileResults *
                                             FileVersion) option
      member WaitForBackgroundCompile: unit -> unit
      member BeforeBackgroundFileCheck: IEvent<string * obj option>
      member CurrentQueueLength: int
      member FileChecked: IEvent<string * obj option>
      member FileParsed: IEvent<string * obj option>
      member internal FrameworkImportsCache: FrameworkImportsCache
      member ImplicitlyStartBackgroundWork: bool
      member MaxMemory: int
      member MaxMemoryReached: IEvent<unit>
      member PauseBeforeBackgroundWork: int
      member ProjectChecked: IEvent<string * obj option>
      member ReactorOps: IReactorOperations
      member ReferenceResolver: ReferenceResolver.Resolver
      static member GlobalForegroundParseCountStatistic: int
      static member GlobalForegroundTypeCheckCountStatistic: int
      [<System.Obsolete
        ("Please create an instance of FSharpChecker using FSharpChecker.Create")>]
      static member Instance: FSharpChecker
  
  [<ClassAttribute>]
  type CompilerEnvironment =

      static member
        BinFolderOfDefaultFSharpCompiler: ?probePoint:string -> string option
  
  module CompilerEnvironment =
    val DefaultReferencesForOrphanSources:
      assumeDotNetFramework:bool -> string list
    val GetCompilationDefinesForEditing:
      parsingOptions:FSharpParsingOptions -> string list
    val IsCheckerSupportedSubcategory: string -> bool

  module DebuggerEnvironment =
    val GetLanguageID: unit -> System.Guid

  module PrettyNaming =
    val IsIdentifierPartCharacter: char -> bool
    val IsLongIdentifierPartCharacter: char -> bool
    val IsOperatorName: string -> bool
    val GetLongNameFromString: string -> string list
    val FormatAndOtherOverloadsString: System.Int32 -> string
    val QuoteIdentifierIfNeeded: string -> string
    val KeywordNames: string list

  module FSharpFileUtilities =
    val isScriptFile: string -> bool




namespace FSharp.Compiler.SourceCodeServices
  module Structure =
    module Range =
      val endToEnd: r1:Range.range -> r2:Range.range -> Range.range
      val endToStart: r1:Range.range -> r2:Range.range -> Range.range
      val startToEnd: r1:Range.range -> r2:Range.range -> Range.range
      val startToStart: r1:Range.range -> r2:Range.range -> Range.range
      val modStart: m:int -> r:Range.range -> Range.range
      val modEnd: m:int -> r:Range.range -> Range.range
      val modBoth: modStart:int -> modEnd:int -> r:Range.range -> Range.range
  
    val longIdentRange: longId:SyntaxTree.LongIdent -> Range.range
    val rangeOfTypeArgsElse:
      other:Range.range -> typeArgs:SyntaxTree.SynTyparDecl list -> Range.range
    val rangeOfSynPatsElse:
      other:Range.range -> synPats:SyntaxTree.SynSimplePat list -> Range.range
    [<RequireQualifiedAccessAttribute>]
    type Collapse =
      | Below
      | Same
    [<RequireQualifiedAccessAttribute>]
    type Scope =
      | Open
      | Namespace
      | Module
      | Type
      | Member
      | LetOrUse
      | Val
      | CompExpr
      | IfThenElse
      | ThenInIfThenElse
      | ElseInIfThenElse
      | TryWith
      | TryInTryWith
      | WithInTryWith
      | TryFinally
      | TryInTryFinally
      | FinallyInTryFinally
      | ArrayOrList
      | ObjExpr
      | For
      | While
      | Match
      | MatchBang
      | MatchLambda
      | MatchClause
      | Lambda
      | CompExprInternal
      | Quote
      | Record
      | SpecialFunc
      | Do
      | New
      | Attribute
      | Interface
      | HashDirective
      | LetOrUseBang
      | TypeExtension
      | YieldOrReturn
      | YieldOrReturnBang
      | Tuple
      | UnionCase
      | EnumCase
      | RecordField
      | RecordDefn
      | UnionDefn
      | Comment
      | XmlDocComment
      with
        override ToString: unit -> string
    
    [<NoComparison>]
    type ScopeRange =
      { Scope: Scope
        Collapse: Collapse
        Range: Range.range
        CollapseRange: Range.range }
    type LineNumber = int
    type LineStr = string
    type CommentType =
      | SingleLine
      | XmlDoc
    [<NoComparison>]
    type CommentList =
      { Lines: ResizeArray<LineNumber * LineStr>
        Type: CommentType }
      with
        static member
          New: ty:CommentType -> lineStr:(LineNumber * LineStr) -> CommentList
    
    val getOutliningRanges:
      sourceLines:string [] ->
        parsedInput:SyntaxTree.ParsedInput -> seq<ScopeRange>


namespace FSharp.Compiler.SourceCodeServices
  module UnusedOpens =
    val symbolHash: System.Collections.Generic.IEqualityComparer<FSharpSymbol>
    type OpenedModule =
  
        new: entity:FSharpEntity * isNestedAutoOpen:bool -> OpenedModule
        member RevealedSymbolsContains: symbol:FSharpSymbol -> bool
        member Entity: FSharpEntity
        member IsNestedAutoOpen: bool
    
    type OpenedModuleGroup =
      { OpenedModules: OpenedModule [] }
      with
        static member Create: modul:FSharpEntity -> OpenedModuleGroup
    
    type OpenStatement =
      { OpenedGroups: OpenedModuleGroup list
        Range: Range.range
        AppliedScope: Range.range }
    val getOpenStatements:
      openDeclarations:FSharpOpenDeclaration [] -> OpenStatement []
    val filterSymbolUses:
      getSourceLineStr:(int -> string) ->
        symbolUses:seq<FSharpSymbolUse> -> FSharpSymbolUse []
    val splitSymbolUses:
      symbolUses:FSharpSymbolUse [] -> FSharpSymbolUse [] * FSharpSymbolUse []
    val isOpenStatementUsed:
      symbolUses2:FSharpSymbolUse [] ->
        symbolUsesRangesByDeclaringEntity:System.Collections.Generic.Dictionary<FSharpEntity,
                                                                                Range.range list> ->
          usedModules:System.Collections.Generic.Dictionary<FSharpEntity,
                                                            Range.range list> ->
            openStatement:OpenStatement -> bool
    val filterOpenStatementsIncremental:
      symbolUses2:FSharpSymbolUse [] ->
        symbolUsesRangesByDeclaringEntity:System.Collections.Generic.Dictionary<FSharpEntity,
                                                                                Range.range list> ->
          openStatements:OpenStatement list ->
            usedModules:System.Collections.Generic.Dictionary<FSharpEntity,
                                                              Range.range list> ->
              acc:OpenStatement list -> Async<OpenStatement list>
    val entityHash: System.Collections.Generic.IEqualityComparer<FSharpEntity>
    val filterOpenStatements:
      symbolUses1:FSharpSymbolUse [] * symbolUses2:FSharpSymbolUse [] ->
        openStatements:OpenStatement [] -> Async<Range.range list>
    val getUnusedOpens:
      checkFileResults:FSharpCheckFileResults * getSourceLineStr:(int -> string) ->
        Async<Range.range list>

  module SimplifyNames =
    type SimplifiableRange =
      { Range: Range.range
        RelativeName: string }
    val getPlidLength: plid:string list -> int
    val getSimplifiableNames:
      checkFileResults:FSharpCheckFileResults * getSourceLineStr:(int -> string) ->
        Async<seq<SimplifiableRange>>

  module UnusedDeclarations =
    val isPotentiallyUnusedDeclaration: symbol:FSharpSymbol -> bool
    val getUnusedDeclarationRanges:
      symbolsUses:seq<FSharpSymbolUse> -> isScript:bool -> seq<Range.range>
    val getUnusedDeclarations:
      checkFileResults:FSharpCheckFileResults * isScriptFile:bool ->
        Async<seq<Range.range>>


namespace FSharp.Compiler.Interactive
  module Shell =
    [<ClassAttribute>]
    type FsiValue =
  
        new: reflectionValue:obj * reflectionType:System.Type *
              fsharpType:SourceCodeServices.FSharpType -> FsiValue
        member FSharpType: SourceCodeServices.FSharpType
        member ReflectionType: System.Type
        member ReflectionValue: obj
    
    [<SealedAttribute>]
    type FsiBoundValue =
  
        new: name:string * value:FsiValue -> FsiBoundValue
        member Name: string
        member Value: FsiValue
    
    module internal Utilities =
      type IAnyToLayoutCall =
        interface
          abstract member
            AnyToLayout: Internal.Utilities.StructuredFormat.FormatOptions *
                          obj * System.Type ->
                            Internal.Utilities.StructuredFormat.Layout
          abstract member
            FsiAnyToLayout: Internal.Utilities.StructuredFormat.FormatOptions *
                             obj * System.Type ->
                               Internal.Utilities.StructuredFormat.Layout
      
      type private AnyToLayoutSpecialization<'T> =
    
          interface IAnyToLayoutCall
          new: unit -> AnyToLayoutSpecialization<'T>
      
      val getAnyToLayoutCall: ty:System.Type -> IAnyToLayoutCall
      val callStaticMethod:
        ty:System.Type -> name:string -> args:obj list -> obj
      val ignoreAllErrors: f:(unit -> unit) -> unit
      val getMember:
        name:string ->
          memberType:System.Reflection.MemberTypes ->
            attr:System.Reflection.BindingFlags ->
              declaringType:System.Type -> System.Reflection.MemberInfo []
      val tryFindMember:
        name:string ->
          memberType:System.Reflection.MemberTypes ->
            declaringType:System.Type -> System.Reflection.MemberInfo option
      val getInstanceProperty: obj:obj -> nm:string -> 'a
      val setInstanceProperty: obj:obj -> nm:string -> v:obj -> 'a
      val callInstanceMethod0:
        obj:obj -> typeArgs:System.Type [] -> nm:string -> 'a
      val callInstanceMethod1:
        obj:obj -> typeArgs:System.Type [] -> nm:string -> v:obj -> 'a
      val callInstanceMethod3:
        obj:obj ->
          typeArgs:System.Type [] ->
            nm:string -> v1:obj -> v2:obj -> v3:obj -> 'a
      val colorPrintL:
        outWriter:System.IO.TextWriter ->
          opts:Internal.Utilities.StructuredFormat.FormatOptions ->
            layout:Internal.Utilities.StructuredFormat.Layout -> unit
      val reportError:
        m:Range.range -> Microsoft.DotNet.DependencyManager.ResolvingErrorReport
      val getOutputDir: tcConfigB:CompilerConfig.TcConfigBuilder -> string
  
    type internal FsiTimeReporter =
  
        new: outWriter:System.IO.TextWriter -> FsiTimeReporter
        member TimeOp: f:(unit -> 'b) -> 'b
        member TimeOpIf: flag:bool -> f:(unit -> 'a) -> 'a
    
    type internal FsiValuePrinterMode =
      | PrintExpr
      | PrintDecl
    [<ClassAttribute>]
    type EvaluationEventArgs =
  
        inherit System.EventArgs
        new: fsivalue:FsiValue option *
              symbolUse:SourceCodeServices.FSharpSymbolUse *
              decl:SourceCodeServices.FSharpImplementationFileDeclaration ->
                EvaluationEventArgs
        member FsiValue: FsiValue option
        member
          ImplementationDeclaration: SourceCodeServices.FSharpImplementationFileDeclaration
        member Name: string
        member Symbol: SourceCodeServices.FSharpSymbol
        member SymbolUse: SourceCodeServices.FSharpSymbolUse
    
    [<AbstractClassAttribute>]
    type FsiEvaluationSessionHostConfig =
  
        new: unit -> FsiEvaluationSessionHostConfig
        abstract member EventLoopInvoke: codeToRun:(unit -> 'T) -> 'T
        abstract member EventLoopRun: unit -> bool
        abstract member EventLoopScheduleRestart: unit -> unit
        abstract member
          GetOptionalConsoleReadLine: probeToSeeIfConsoleWorks:bool ->
                                         (unit -> string) option
        abstract member ReportUserCommandLineArgs: string [] -> unit
        abstract member StartServer: fsiServerName:string -> unit
        member
          internal TriggerEvaluation: value:FsiValue option *
                                       symbolUse:SourceCodeServices.FSharpSymbolUse *
                                       decl:SourceCodeServices.FSharpImplementationFileDeclaration ->
                                         unit
        abstract member
          AddedPrinters: Choice<(System.Type * (obj -> string)),
                                 (System.Type * (obj -> obj))> list
        abstract member FloatingPointFormat: string
        abstract member FormatProvider: System.IFormatProvider
        member OnEvaluation: IEvent<EvaluationEventArgs>
        abstract member PrintDepth: int
        abstract member PrintLength: int
        abstract member PrintSize: int
        abstract member PrintWidth: int
        abstract member ShowDeclarationValues: bool
        abstract member ShowIEnumerable: bool
        abstract member ShowProperties: bool
        abstract member UseFsiAuxLib: bool
    
    type internal FsiValuePrinter =
  
        new: fsi:FsiEvaluationSessionHostConfig * g:TcGlobals.TcGlobals *
              generateDebugInfo:bool *
              resolveAssemblyRef:(AbstractIL.IL.ILAssemblyRef ->
                                    Choice<string,System.Reflection.Assembly> option) *
              outWriter:System.IO.TextWriter -> FsiValuePrinter
        member FormatValue: obj:obj * objTy:System.Type -> string
        member
          GetEvaluationContext: emEnv:AbstractIL.ILRuntimeWriter.emEnv ->
                                   IlxGen.ExecutionContext
        member
          GetFsiPrintOptions: unit ->
                                 Internal.Utilities.StructuredFormat.FormatOptions
        member
          InvokeDeclLayout: emEnv:AbstractIL.ILRuntimeWriter.emEnv *
                             ilxGenerator:IlxGen.IlxAssemblyGenerator *
                             v:TypedTree.Val ->
                               Internal.Utilities.StructuredFormat.Layout option
        member
          InvokeExprPrinter: denv:TypedTreeOps.DisplayEnv *
                              emEnv:AbstractIL.ILRuntimeWriter.emEnv *
                              ilxGenerator:IlxGen.IlxAssemblyGenerator *
                              vref:TypedTree.Val -> unit
        member
          PrintValue: printMode:FsiValuePrinterMode *
                       opts:Internal.Utilities.StructuredFormat.FormatOptions *
                       x:obj * ty:System.Type ->
                         Internal.Utilities.StructuredFormat.Layout
    
    type internal FsiStdinSyphon =
  
        new: errorWriter:System.IO.TextWriter -> FsiStdinSyphon
        member Add: str:string -> unit
        member GetLine: filename:string -> i:int -> string
        member
          PrintError: tcConfig:CompilerConfig.TcConfigBuilder *
                       err:ErrorLogger.PhasedDiagnostic -> unit
        member Reset: unit -> unit
    
    type internal FsiConsoleOutput =
  
        new: tcConfigB:CompilerConfig.TcConfigBuilder *
              outWriter:System.IO.TextWriter * errorWriter:System.IO.TextWriter ->
                FsiConsoleOutput
        member uprintf: fmt:Printf.TextWriterFormat<'f> -> 'f
        member uprintfn: fmt:Printf.TextWriterFormat<'e> -> 'e
        member uprintfnn: fmt:Printf.TextWriterFormat<'d,unit> -> 'd
        member uprintnf: fmt:Printf.TextWriterFormat<'c> -> 'c
        member uprintnfn: fmt:Printf.TextWriterFormat<'b> -> 'b
        member uprintnfnn: fmt:Printf.TextWriterFormat<'a,unit> -> 'a
        member Error: System.IO.TextWriter
        member Out: System.IO.TextWriter
    
    type internal ErrorLoggerThatStopsOnFirstError =
  
        inherit ErrorLogger.ErrorLogger
        new: tcConfigB:CompilerConfig.TcConfigBuilder *
              fsiStdinSyphon:FsiStdinSyphon * fsiConsoleOutput:FsiConsoleOutput ->
                ErrorLoggerThatStopsOnFirstError
        override
          DiagnosticSink: err:ErrorLogger.PhasedDiagnostic * isError:bool ->
                             unit
        member ResetErrorCount: unit -> unit
        member SetError: unit -> unit
        override ErrorCount: int
    
    type ErrorLogger with
      member CheckForErrors: unit -> bool
    type ErrorLogger with
      member AbortOnError: fsiConsoleOutput:FsiConsoleOutput -> unit
    val internal directoryName: s:string -> string
    type internal FsiCommandLineOptions =
  
        new: fsi:FsiEvaluationSessionHostConfig * argv:string [] *
              tcConfigB:CompilerConfig.TcConfigBuilder *
              fsiConsoleOutput:FsiConsoleOutput -> FsiCommandLineOptions
        member ShowBanner: unit -> unit
        member ShowHelp: m:Range.range -> unit
        member
          DependencyProvider: Microsoft.DotNet.DependencyManager.DependencyProvider
        member EnableConsoleKeyProcessing: bool
        member FsiLCID: int option
        member FsiServerInputCodePage: int option
        member FsiServerName: string
        member FsiServerOutputCodePage: int option
        member Gui: bool
        member Interact: bool
        member IsInteractiveServer: bool
        member PeekAheadOnConsoleToPermitTyping: bool
        member ProbeToSeeIfConsoleWorks: bool
        member ShowILCode: bool
        member ShowTypes: bool
        member SourceFiles: (string * bool) list
    
    val internal SetCurrentUICultureForThread:
      lcid:int option -> System.IDisposable
    val internal InstallErrorLoggingOnThisThread:
      errorLogger:ErrorLogger.ErrorLogger -> unit
    val internal SetServerCodePages: fsiOptions:FsiCommandLineOptions -> unit
    type internal FsiConsolePrompt =
  
        new: fsiOptions:FsiCommandLineOptions *
              fsiConsoleOutput:FsiConsoleOutput -> FsiConsolePrompt
        member Print: unit -> unit
        member PrintAhead: unit -> unit
        member SkipNext: unit -> unit
        member FsiOptions: FsiCommandLineOptions
    
    type internal FsiConsoleInput =
  
        new: fsi:FsiEvaluationSessionHostConfig *
              fsiOptions:FsiCommandLineOptions * inReader:System.IO.TextReader *
              outWriter:System.IO.TextWriter -> FsiConsoleInput
        member TryGetConsole: unit -> (unit -> string) option
        member TryGetFirstLine: unit -> string option
        member WaitForInitialConsoleInput: unit -> unit
        member In: System.IO.TextReader
    
    type internal FsiInteractionStepStatus =
      | CtrlC
      | EndOfFile
      | Completed of FsiValue option
      | CompletedWithAlreadyReportedError
      | CompletedWithReportedError of exn
    [<NoEquality; NoComparison>]
    type internal FsiDynamicCompilerState =
      { optEnv: Optimizer.IncrementalOptimizationEnv
        emEnv: AbstractIL.ILRuntimeWriter.emEnv
        tcGlobals: TcGlobals.TcGlobals
        tcState: ParseAndCheckInputs.TcState
        tcImports: CompilerImports.TcImports
        ilxGenerator: IlxGen.IlxAssemblyGenerator
        boundValues: AbstractIL.Internal.Library.NameMap<TypedTree.Val>
        timing: bool
        debugBreak: bool }
    val internal WithImplicitHome:
      tcConfigB:CompilerConfig.TcConfigBuilder * dir:string ->
        f:(unit -> 'a) -> 'a
    val internal convertReflectionTypeToILTypeRef:
      reflectionTy:System.Type -> AbstractIL.IL.ILTypeRef
    val internal convertReflectionTypeToILType:
      reflectionTy:System.Type -> AbstractIL.IL.ILType
    val internal mkBoundValueTypedImpl:
      tcGlobals:TcGlobals.TcGlobals ->
        m:Range.range ->
          moduleName:string ->
            name:string ->
              ty:TypedTree.TType ->
                TypedTree.ModuleOrNamespace * TypedTree.Val *
                TypedTree.TypedImplFile
    type internal FsiDynamicCompiler =
  
        new: fsi:FsiEvaluationSessionHostConfig * timeReporter:FsiTimeReporter *
              tcConfigB:CompilerConfig.TcConfigBuilder * tcLockObject:obj *
              outWriter:System.IO.TextWriter *
              tcImports:CompilerImports.TcImports *
              tcGlobals:TcGlobals.TcGlobals * fsiOptions:FsiCommandLineOptions *
              fsiConsoleOutput:FsiConsoleOutput * fsiCollectible:bool *
              niceNameGen:CompilerGlobalState.NiceNameGenerator *
              resolveAssemblyRef:(AbstractIL.IL.ILAssemblyRef ->
                                    Choice<string,System.Reflection.Assembly> option) ->
                FsiDynamicCompiler
        member
          AddBoundValue: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                          errorLogger:ErrorLogger.ErrorLogger *
                          istate:FsiDynamicCompilerState * name:string *
                          value:obj ->
                            FsiDynamicCompilerState * FsiInteractionStepStatus
        member
          BuildItBinding: expr:SyntaxTree.SynExpr ->
                             SyntaxTree.SynModuleDecl list
        member
          CommitDependencyManagerText: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                                        istate:FsiDynamicCompilerState *
                                        lexResourceManager:Lexhelp.LexResourceManager *
                                        errorLogger:ErrorLogger.ErrorLogger ->
                                          FsiDynamicCompilerState
        member CreateDebuggerBreak: m:Range.range -> SyntaxTree.SynModuleDecl
        member
          CurrentPartialAssemblySignature: istate:FsiDynamicCompilerState ->
                                              SourceCodeServices.FSharpAssemblySignature
        member
          EvalDependencyManagerTextFragment: packageManager:Microsoft.DotNet.DependencyManager.IDependencyManagerProvider *
                                              lt:CompilerConfig.Directive *
                                              m:Range.range * path:string ->
                                                unit
        member
          EvalParsedDefinitions: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                                  errorLogger:ErrorLogger.ErrorLogger *
                                  istate:FsiDynamicCompilerState *
                                  showTypes:bool * isInteractiveItExpr:bool *
                                  defs:SyntaxTree.SynModuleDecl list ->
                                    FsiDynamicCompilerState *
                                    FsiInteractionStepStatus
        member
          EvalParsedExpression: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                                 errorLogger:ErrorLogger.ErrorLogger *
                                 istate:FsiDynamicCompilerState *
                                 expr:SyntaxTree.SynExpr ->
                                   FsiDynamicCompilerState *
                                   FsiInteractionStepStatus
        member
          EvalParsedSourceFiles: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                                  errorLogger:ErrorLogger.ErrorLogger *
                                  istate:FsiDynamicCompilerState *
                                  inputs:SyntaxTree.ParsedInput list ->
                                    FsiDynamicCompilerState
        member
          EvalRequireReference: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                                 istate:FsiDynamicCompilerState * m:Range.range *
                                 path:string ->
                                   CompilerImports.AssemblyResolution list *
                                   FsiDynamicCompilerState
        member
          EvalSourceFiles: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                            istate:FsiDynamicCompilerState * m:Range.range *
                            sourceFiles:string list *
                            lexResourceManager:Lexhelp.LexResourceManager *
                            errorLogger:ErrorLogger.ErrorLogger ->
                              FsiDynamicCompilerState
        member FormatValue: obj:obj * objTy:System.Type -> string
        member
          GetBoundValues: istate:FsiDynamicCompilerState -> FsiBoundValue list
        member GetInitialInteractiveState: unit -> FsiDynamicCompilerState
        member
          ProcessMetaCommandsFromInputAsInteractiveCommands: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                                                              istate:FsiDynamicCompilerState *
                                                              sourceFile:string *
                                                              inp:SyntaxTree.ParsedInput ->
                                                                FsiDynamicCompilerState
        member
          TryFindBoundValue: istate:FsiDynamicCompilerState * nm:string ->
                                FsiBoundValue option
        member DynamicAssembly: System.Reflection.Assembly
        member DynamicAssemblyName: string
        member ValueBound: IEvent<obj * System.Type * string>
    
    type ControlEventHandler =
      delegate of int -> bool
    type internal FsiInterruptStdinState =
      | StdinEOFPermittedBecauseCtrlCRecentlyPressed
      | StdinNormal
    type internal FsiInterruptControllerState =
      | InterruptCanRaiseException
      | InterruptIgnored
    type internal FsiInterruptControllerKillerThreadRequest =
      | ThreadAbortRequest
      | NoRequest
      | ExitRequest
      | PrintInterruptRequest
    type internal FsiInterruptController =
  
        new: fsiOptions:FsiCommandLineOptions *
              fsiConsoleOutput:FsiConsoleOutput -> FsiInterruptController
        member ClearInterruptRequest: unit -> unit
        member Exit: unit -> 'a
        member
          InstallKillThread: threadToKill:System.Threading.Thread *
                              pauseMilliseconds:int -> unit
        member Interrupt: unit -> unit
        member PosixInvoke: n:int -> unit
        member EventHandlers: ControlEventHandler list
        member FsiInterruptStdinState: FsiInterruptStdinState
        member InterruptAllowed: FsiInterruptControllerState with set
    
    module internal MagicAssemblyResolution =
      val private assemblyLoadFrom: path:string -> System.Reflection.Assembly
      val Install:
        tcConfigB:CompilerConfig.TcConfigBuilder *
        tcImports:CompilerImports.TcImports *
        fsiDynamicCompiler:FsiDynamicCompiler *
        fsiConsoleOutput:FsiConsoleOutput -> System.IDisposable
  
    type internal FsiStdinLexerProvider =
  
        new: tcConfigB:CompilerConfig.TcConfigBuilder *
              fsiStdinSyphon:FsiStdinSyphon * fsiConsoleInput:FsiConsoleInput *
              fsiConsoleOutput:FsiConsoleOutput *
              fsiOptions:FsiCommandLineOptions *
              lexResourceManager:Lexhelp.LexResourceManager ->
                FsiStdinLexerProvider
        member
          CreateBufferLexer: sourceFileName:string *
                              lexbuf:UnicodeLexing.Lexbuf *
                              errorLogger:ErrorLogger.ErrorLogger ->
                                LexFilter.LexFilter
        member
          CreateIncludedScriptLexer: sourceFileName:string *
                                      reader:System.IO.StreamReader *
                                      errorLogger:ErrorLogger.ErrorLogger ->
                                        LexFilter.LexFilter
        member
          CreateStdinLexer: errorLogger:ErrorLogger.ErrorLogger ->
                               LexFilter.LexFilter
        member
          CreateStringLexer: sourceFileName:string * source:string *
                              errorLogger:ErrorLogger.ErrorLogger ->
                                LexFilter.LexFilter
        member ConsoleInput: FsiConsoleInput
    
    type internal FsiInteractionProcessor =
  
        new: fsi:FsiEvaluationSessionHostConfig *
              tcConfigB:CompilerConfig.TcConfigBuilder *
              fsiOptions:FsiCommandLineOptions *
              fsiDynamicCompiler:FsiDynamicCompiler *
              fsiConsolePrompt:FsiConsolePrompt *
              fsiConsoleOutput:FsiConsoleOutput *
              fsiInterruptController:FsiInterruptController *
              fsiStdinLexerProvider:FsiStdinLexerProvider *
              lexResourceManager:Lexhelp.LexResourceManager *
              initialInteractiveState:FsiDynamicCompilerState ->
                FsiInteractionProcessor
        member
          AddBoundValue: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                          errorLogger:ErrorLogger.ErrorLogger * name:string *
                          value:obj -> Choice<FsiValue option,exn option>
        member
          CompletionsForPartialLID: istate:FsiDynamicCompilerState *
                                     prefix:string -> string list
        member
          EvalExpression: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                           sourceText:string * scriptFileName:string *
                           errorLogger:ErrorLogger.ErrorLogger ->
                             Choice<FsiValue option,exn option>
        member
          EvalIncludedScript: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                               istate:FsiDynamicCompilerState *
                               sourceFile:string * m:Range.range *
                               errorLogger:ErrorLogger.ErrorLogger ->
                                 FsiDynamicCompilerState *
                                 FsiInteractionStepStatus
        member
          EvalIncludedScripts: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                                istate:FsiDynamicCompilerState *
                                sourceFiles:string list *
                                errorLogger:ErrorLogger.ErrorLogger ->
                                  FsiDynamicCompilerState
        member
          EvalInteraction: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                            sourceText:string * scriptFileName:string *
                            errorLogger:ErrorLogger.ErrorLogger *
                            ?cancellationToken:System.Threading.CancellationToken ->
                              Choice<FsiValue option,exn option>
        member
          EvalScript: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                       scriptPath:string * errorLogger:ErrorLogger.ErrorLogger ->
                         Choice<FsiValue option,exn option>
        member
          LoadDummyInteraction: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                                 errorLogger:ErrorLogger.ErrorLogger -> unit
        member
          LoadInitialFiles: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                             errorLogger:ErrorLogger.ErrorLogger -> unit
        member
          ParseAndCheckInteraction: ctok:AbstractIL.Internal.Library.CompilationThreadToken *
                                     legacyReferenceResolver:ReferenceResolver.Resolver *
                                     checker:SourceCodeServices.IReactorOperations *
                                     istate:FsiDynamicCompilerState *
                                     text:string ->
                                       Async<SourceCodeServices.FSharpParseFileResults *
                                             SourceCodeServices.FSharpCheckFileResults *
                                             SourceCodeServices.FSharpCheckProjectResults>
        member
          ParseAndExecOneSetOfInteractionsFromLexbuf: runCodeOnMainThread:((#AbstractIL.Internal.Library.CompilationThreadToken ->
                                                                               FsiDynamicCompilerState ->
                                                                               FsiDynamicCompilerState *
                                                                               FsiInteractionStepStatus) ->
                                                                              FsiDynamicCompilerState ->
                                                                              FsiDynamicCompilerState *
                                                                              FsiInteractionStepStatus) *
                                                       istate:FsiDynamicCompilerState *
                                                       tokenizer:LexFilter.LexFilter *
                                                       errorLogger:ErrorLogger.ErrorLogger *
                                                       ?cancellationToken:System.Threading.CancellationToken ->
                                                         FsiDynamicCompilerState *
                                                         FsiInteractionStepStatus
        member
          StartStdinReadAndProcessThread: errorLogger:ErrorLogger.ErrorLogger ->
                                             unit
        member CurrentState: FsiDynamicCompilerState
        member PartialAssemblySignatureUpdated: IEvent<unit>
    
    val internal SpawnThread: name:string -> f:(unit -> unit) -> unit
    val internal SpawnInteractiveServer:
      fsi:FsiEvaluationSessionHostConfig * fsiOptions:FsiCommandLineOptions *
      fsiConsoleOutput:FsiConsoleOutput -> unit
    val internal DriveFsiEventLoop:
      fsi:FsiEvaluationSessionHostConfig * fsiConsoleOutput:FsiConsoleOutput ->
        unit
    [<ClassAttribute>]
    type FsiCompilationException =
  
        inherit System.Exception
        new: string * SourceCodeServices.FSharpErrorInfo [] option ->
                FsiCompilationException
        member ErrorInfos: SourceCodeServices.FSharpErrorInfo [] option
    
    [<ClassAttribute>]
    type FsiEvaluationSession =
  
        interface System.IDisposable
        new: fsi:FsiEvaluationSessionHostConfig * argv:string [] *
              inReader:System.IO.TextReader * outWriter:System.IO.TextWriter *
              errorWriter:System.IO.TextWriter * fsiCollectible:bool *
              legacyReferenceResolver:ReferenceResolver.Resolver option ->
                FsiEvaluationSession
        static member
          Create: fsiConfig:FsiEvaluationSessionHostConfig * argv:string [] *
                   inReader:System.IO.TextReader *
                   outWriter:System.IO.TextWriter *
                   errorWriter:System.IO.TextWriter * ?collectible:bool *
                   ?legacyReferenceResolver:ReferenceResolver.Resolver ->
                     FsiEvaluationSession
        static member
          GetDefaultConfiguration: unit -> FsiEvaluationSessionHostConfig
        static member
          GetDefaultConfiguration: fsiObj:obj -> FsiEvaluationSessionHostConfig
        static member
          GetDefaultConfiguration: fsiObj:obj * useFsiAuxLib:bool ->
                                      FsiEvaluationSessionHostConfig
        member AddBoundValue: name:string * value:obj -> unit
        member EvalExpression: code:string -> FsiValue option
        member
          EvalExpressionNonThrowing: code:string ->
                                        Choice<FsiValue option,exn> *
                                        SourceCodeServices.FSharpErrorInfo []
        member
          EvalInteraction: code:string *
                            ?cancellationToken:System.Threading.CancellationToken ->
                              unit
        member
          EvalInteractionNonThrowing: code:string *
                                       ?cancellationToken:System.Threading.CancellationToken ->
                                         Choice<FsiValue option,exn> *
                                         SourceCodeServices.FSharpErrorInfo []
        member EvalScript: filePath:string -> unit
        member
          EvalScriptNonThrowing: filePath:string ->
                                    Choice<unit,exn> *
                                    SourceCodeServices.FSharpErrorInfo []
        member
          FormatValue: reflectionValue:obj * reflectionType:System.Type ->
                          string
        member GetBoundValues: unit -> FsiBoundValue list
        member GetCompletions: longIdent:string -> seq<string>
        member Interrupt: unit -> unit
        member
          ParseAndCheckInteraction: code:string ->
                                       Async<SourceCodeServices.FSharpParseFileResults *
                                             SourceCodeServices.FSharpCheckFileResults *
                                             SourceCodeServices.FSharpCheckProjectResults>
        member ReportUnhandledException: exn:exn -> unit
        member Run: unit -> unit
        member TryFindBoundValue: name:string -> FsiBoundValue option
        member
          CurrentPartialAssemblySignature: SourceCodeServices.FSharpAssemblySignature
        member DynamicAssembly: System.Reflection.Assembly
        member InteractiveChecker: SourceCodeServices.FSharpChecker
        member IsGui: bool
        member LCID: int option
        member PartialAssemblySignatureUpdated: IEvent<unit>
        member ValueBound: IEvent<obj * System.Type * string>
    
    module Settings =
      type IEventLoop =
        interface
          abstract member Invoke: (unit -> 'T) -> 'T
          abstract member Run: unit -> bool
          abstract member ScheduleRestart: unit -> unit
      
      type internal SimpleEventLoop =
    
          interface System.IDisposable
          interface IEventLoop
          new: unit -> SimpleEventLoop
      
      [<SealedAttribute>]
      type InteractiveSettings =
    
          new: unit -> InteractiveSettings
          member AddPrintTransformer: ('T -> obj) -> unit
          member AddPrinter: ('T -> string) -> unit
          member
            AddedPrinters: Choice<(System.Type * (obj -> string)),
                                   (System.Type * (obj -> obj))> list
          member CommandLineArgs: string []
          member EventLoop: IEventLoop
          member FloatingPointFormat: string
          member FormatProvider: System.IFormatProvider
          member PrintDepth: int
          member PrintLength: int
          member PrintSize: int
          member PrintWidth: int
          member ShowDeclarationValues: bool
          member ShowIDictionary: bool
          member ShowIEnumerable: bool
          member ShowProperties: bool
      
      val fsi: InteractiveSettings
  
    [<AllowNullLiteralAttribute>]
    type CompilerInputStream =
  
        inherit System.IO.Stream
        new: unit -> CompilerInputStream
        member Add: str:string -> unit
        override Flush: unit -> unit
        override Read: buffer:byte [] * offset:int * count:int -> int
        override Seek: _offset:int64 * _origin:System.IO.SeekOrigin -> int64
        override SetLength: _value:int64 -> unit
        override Write: _buffer:byte [] * _offset:int * _count:int -> unit
        override CanRead: bool
        override CanSeek: bool
        override CanWrite: bool
        override Length: int64
        override Position: int64
    
    [<AllowNullLiteralAttribute>]
    type CompilerOutputStream =
  
        inherit System.IO.Stream
        new: unit -> CompilerOutputStream
        override Flush: unit -> unit
        member Read: unit -> string
        override Read: _buffer:byte [] * _offset:int * _count:int -> int
        override Seek: _offset:int64 * _origin:System.IO.SeekOrigin -> int64
        override SetLength: _value:int64 -> unit
        override Write: buffer:byte [] * offset:int * count:int -> unit
        override CanRead: bool
        override CanSeek: bool
        override CanWrite: bool
        override Length: int64
        override Position: int64
    


