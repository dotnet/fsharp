ImplFile
  (ParsedImplFileInput
     ("/root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs", false,
      QualifiedNameOfFile NestedSynTypeOrInsideSynExprTraitCall, [], [],
      [SynModuleOrNamespace
         ([NestedSynTypeOrInsideSynExprTraitCall], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, true, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some x)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent
                       ([op_BangBang], [],
                        [Some
                           (OriginalNotationWithParen
                              ((2,11--2,12), "!!", (2,14--2,15)))]), None, None,
                     Pats
                       [Paren
                          (Typed
                             (Named
                                (SynIdent (x, None), false, None, (2,17--2,18)),
                              Tuple
                                (false,
                                 [Type
                                    (Var
                                       (SynTypar (a, HeadType, false),
                                        (2,20--2,22))); Star (2,23--2,24);
                                  Type
                                    (Var
                                       (SynTypar (b, HeadType, false),
                                        (2,25--2,27)))], (2,20--2,27)),
                              (2,17--2,27)), (2,16--2,28))], None, (2,11--2,28)),
                  Some
                    (SynBindingReturnInfo
                       (Var (SynTypar (c, HeadType, false), (2,31--2,33)),
                        (2,31--2,33), [], { ColonRange = Some (2,29--2,30) })),
                  Typed
                    (Paren
                       (TraitCall
                          (Paren
                             (Or
                                (Or
                                   (Var
                                      (SynTypar (a, HeadType, false),
                                       (2,38--2,40)),
                                    Var
                                      (SynTypar (b, HeadType, false),
                                       (2,44--2,46)), (2,38--2,46),
                                    { OrKeyword = (2,41--2,43) }),
                                 Var
                                   (SynTypar (c, HeadType, false), (2,50--2,52)),
                                 (2,38--2,52), { OrKeyword = (2,47--2,49) }),
                              (2,37--2,53)),
                           Member
                             (SynValSig
                                ([], SynIdent (op_Implicit, None),
                                 SynValTyparDecls (None, true),
                                 Fun
                                   (Tuple
                                      (false,
                                       [Type
                                          (Var
                                             (SynTypar (a, HeadType, false),
                                              (2,83--2,85))); Star (2,86--2,87);
                                        Type
                                          (Var
                                             (SynTypar (b, HeadType, false),
                                              (2,88--2,90)))], (2,83--2,90)),
                                    Var
                                      (SynTypar (c, HeadType, false),
                                       (2,94--2,96)), (2,83--2,96),
                                    { ArrowRange = (2,91--2,93) }),
                                 SynValInfo
                                   ([[SynArgInfo ([], false, None);
                                      SynArgInfo ([], false, None)]],
                                    SynArgInfo ([], false, None)), false, false,
                                 PreXmlDoc ((2,56), FSharp.Compiler.Xml.XmlDocCollector),
                                 Single None, None, (2,56--2,96),
                                 { LeadingKeyword =
                                    StaticMember ((2,56--2,62), (2,63--2,69))
                                   InlineKeyword = None
                                   WithKeyword = None
                                   EqualsRange = None }),
                              { IsInstance = false
                                IsDispatchSlot = false
                                IsOverrideOrExplicitImpl = false
                                IsFinal = false
                                GetterOrSetterIsCompilerGenerated = false
                                MemberKind = Member }, (2,56--2,96),
                              { GetSetKeywords = None }), Ident x, (2,36--2,100)),
                        (2,36--2,37), Some (2,99--2,100), (2,36--2,100)),
                     Var (SynTypar (c, HeadType, false), (2,31--2,33)),
                     (2,36--2,100)), (2,11--2,28), NoneAtLet,
                  { LeadingKeyword = Let (2,0--2,3)
                    InlineKeyword = Some (2,4--2,10)
                    EqualsRange = Some (2,34--2,35) })], (2,0--2,100))],
          PreXmlDocEmpty, [], None, (2,0--3,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
