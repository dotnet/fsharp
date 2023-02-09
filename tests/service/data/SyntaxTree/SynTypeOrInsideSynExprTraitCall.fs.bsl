ImplFile
  (ParsedImplFileInput
     ("/root/SynTypeOrInsideSynExprTraitCall.fs", false,
      QualifiedNameOfFile SynTypeOrInsideSynExprTraitCall, [], [],
      [SynModuleOrNamespace
         ([SynTypeOrInsideSynExprTraitCall], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, true, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
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
                              (/root/SynTypeOrInsideSynExprTraitCall.fs (1,11--1,12),
                               "!!",
                               /root/SynTypeOrInsideSynExprTraitCall.fs (1,14--1,15)))]),
                     None, None,
                     Pats
                       [Paren
                          (Typed
                             (Named
                                (SynIdent (x, None), false, None,
                                 /root/SynTypeOrInsideSynExprTraitCall.fs (1,17--1,18)),
                              Var
                                (SynTypar (a, HeadType, false),
                                 /root/SynTypeOrInsideSynExprTraitCall.fs (1,20--1,22)),
                              /root/SynTypeOrInsideSynExprTraitCall.fs (1,17--1,22)),
                           /root/SynTypeOrInsideSynExprTraitCall.fs (1,16--1,23))],
                     None, /root/SynTypeOrInsideSynExprTraitCall.fs (1,11--1,23)),
                  Some
                    (SynBindingReturnInfo
                       (Var
                          (SynTypar (b, HeadType, false),
                           /root/SynTypeOrInsideSynExprTraitCall.fs (1,26--1,28)),
                        /root/SynTypeOrInsideSynExprTraitCall.fs (1,26--1,28),
                        [],
                        { ColonRange =
                           Some
                             /root/SynTypeOrInsideSynExprTraitCall.fs (1,24--1,25) })),
                  Typed
                    (Paren
                       (TraitCall
                          (Paren
                             (Or
                                (Var
                                   (SynTypar (a, HeadType, false),
                                    /root/SynTypeOrInsideSynExprTraitCall.fs (1,33--1,35)),
                                 Var
                                   (SynTypar (b, HeadType, false),
                                    /root/SynTypeOrInsideSynExprTraitCall.fs (1,39--1,41)),
                                 /root/SynTypeOrInsideSynExprTraitCall.fs (1,33--1,41),
                                 { OrKeyword =
                                    /root/SynTypeOrInsideSynExprTraitCall.fs (1,36--1,38) }),
                              /root/SynTypeOrInsideSynExprTraitCall.fs (1,32--1,42)),
                           Member
                             (SynValSig
                                ([], SynIdent (op_Implicit, None),
                                 SynValTyparDecls (None, true),
                                 Fun
                                   (Var
                                      (SynTypar (a, HeadType, false),
                                       /root/SynTypeOrInsideSynExprTraitCall.fs (1,72--1,74)),
                                    Var
                                      (SynTypar (b, HeadType, false),
                                       /root/SynTypeOrInsideSynExprTraitCall.fs (1,78--1,80)),
                                    /root/SynTypeOrInsideSynExprTraitCall.fs (1,72--1,80),
                                    { ArrowRange =
                                       /root/SynTypeOrInsideSynExprTraitCall.fs (1,75--1,77) }),
                                 SynValInfo
                                   ([[SynArgInfo ([], false, None)]],
                                    SynArgInfo ([], false, None)), false, false,
                                 PreXmlDoc ((1,45), FSharp.Compiler.Xml.XmlDocCollector),
                                 None, None,
                                 /root/SynTypeOrInsideSynExprTraitCall.fs (1,45--1,80),
                                 { LeadingKeyword =
                                    StaticMember
                                      (/root/SynTypeOrInsideSynExprTraitCall.fs (1,45--1,51),
                                       /root/SynTypeOrInsideSynExprTraitCall.fs (1,52--1,58))
                                   InlineKeyword = None
                                   WithKeyword = None
                                   EqualsRange = None }),
                              { IsInstance = false
                                IsDispatchSlot = false
                                IsOverrideOrExplicitImpl = false
                                IsFinal = false
                                GetterOrSetterIsCompilerGenerated = false
                                MemberKind = Member },
                              /root/SynTypeOrInsideSynExprTraitCall.fs (1,45--1,80),
                              { GetSetKeywords = None }), Ident x,
                           /root/SynTypeOrInsideSynExprTraitCall.fs (1,31--1,84)),
                        /root/SynTypeOrInsideSynExprTraitCall.fs (1,31--1,32),
                        Some
                          /root/SynTypeOrInsideSynExprTraitCall.fs (1,83--1,84),
                        /root/SynTypeOrInsideSynExprTraitCall.fs (1,31--1,84)),
                     Var
                       (SynTypar (b, HeadType, false),
                        /root/SynTypeOrInsideSynExprTraitCall.fs (1,26--1,28)),
                     /root/SynTypeOrInsideSynExprTraitCall.fs (1,31--1,84)),
                  /root/SynTypeOrInsideSynExprTraitCall.fs (1,11--1,23),
                  NoneAtLet,
                  { LeadingKeyword =
                     Let /root/SynTypeOrInsideSynExprTraitCall.fs (1,0--1,3)
                    InlineKeyword =
                     Some /root/SynTypeOrInsideSynExprTraitCall.fs (1,4--1,10)
                    EqualsRange =
                     Some /root/SynTypeOrInsideSynExprTraitCall.fs (1,29--1,30) })],
              /root/SynTypeOrInsideSynExprTraitCall.fs (1,0--1,84))],
          PreXmlDocEmpty, [], None,
          /root/SynTypeOrInsideSynExprTraitCall.fs (1,0--1,84),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))