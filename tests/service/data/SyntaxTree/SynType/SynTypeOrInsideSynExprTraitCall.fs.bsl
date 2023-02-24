ImplFile
  (ParsedImplFileInput
     ("/root/SynType/SynTypeOrInsideSynExprTraitCall.fs", false,
      QualifiedNameOfFile SynTypeOrInsideSynExprTraitCall, [], [],
      [SynModuleOrNamespace
         ([SynTypeOrInsideSynExprTraitCall], false, AnonModule,
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
                              Var (SynTypar (a, HeadType, false), (2,20--2,22)),
                              (2,17--2,22)), (2,16--2,23))], None, (2,11--2,23)),
                  Some
                    (SynBindingReturnInfo
                       (Var (SynTypar (b, HeadType, false), (2,26--2,28)),
                        (2,26--2,28), [], { ColonRange = Some (2,24--2,25) })),
                  Typed
                    (Paren
                       (TraitCall
                          (Paren
                             (Or
                                (Var
                                   (SynTypar (a, HeadType, false), (2,33--2,35)),
                                 Var
                                   (SynTypar (b, HeadType, false), (2,39--2,41)),
                                 (2,33--2,41), { OrKeyword = (2,36--2,38) }),
                              (2,32--2,42)),
                           Member
                             (SynValSig
                                ([], SynIdent (op_Implicit, None),
                                 SynValTyparDecls (None, true),
                                 Fun
                                   (Var
                                      (SynTypar (a, HeadType, false),
                                       (2,72--2,74)),
                                    Var
                                      (SynTypar (b, HeadType, false),
                                       (2,78--2,80)), (2,72--2,80),
                                    { ArrowRange = (2,75--2,77) }),
                                 SynValInfo
                                   ([[SynArgInfo ([], false, None)]],
                                    SynArgInfo ([], false, None)), false, false,
                                 PreXmlDoc ((2,45), FSharp.Compiler.Xml.XmlDocCollector),
                                 None, None, (2,45--2,80),
                                 { LeadingKeyword =
                                    StaticMember ((2,45--2,51), (2,52--2,58))
                                   InlineKeyword = None
                                   WithKeyword = None
                                   EqualsRange = None }),
                              { IsInstance = false
                                IsDispatchSlot = false
                                IsOverrideOrExplicitImpl = false
                                IsFinal = false
                                GetterOrSetterIsCompilerGenerated = false
                                MemberKind = Member }, (2,45--2,80),
                              { GetSetKeywords = None }), Ident x, (2,31--2,84)),
                        (2,31--2,32), Some (2,83--2,84), (2,31--2,84)),
                     Var (SynTypar (b, HeadType, false), (2,26--2,28)),
                     (2,31--2,84)), (2,11--2,23), NoneAtLet,
                  { LeadingKeyword = Let (2,0--2,3)
                    InlineKeyword = Some (2,4--2,10)
                    EqualsRange = Some (2,29--2,30) })], (2,0--2,84))],
          PreXmlDocEmpty, [], None, (2,0--3,0), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))
