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
                              (/root/SynTypeOrInsideSynExprTraitCall.fs (2,11--2,12),
                               "!!",
                               /root/SynTypeOrInsideSynExprTraitCall.fs (2,14--2,15)))]),
                     None, None,
                     Pats
                       [Paren
                          (Typed
                             (Named
                                (SynIdent (x, None), false, None,
                                 /root/SynTypeOrInsideSynExprTraitCall.fs (2,17--2,18)),
                              Var
                                (SynTypar (a, HeadType, false),
                                 /root/SynTypeOrInsideSynExprTraitCall.fs (2,20--2,22)),
                              /root/SynTypeOrInsideSynExprTraitCall.fs (2,17--2,22)),
                           /root/SynTypeOrInsideSynExprTraitCall.fs (2,16--2,23))],
                     None, /root/SynTypeOrInsideSynExprTraitCall.fs (2,11--2,23)),
                  Some
                    (SynBindingReturnInfo
                       (Var
                          (SynTypar (b, HeadType, false),
                           /root/SynTypeOrInsideSynExprTraitCall.fs (2,26--2,28)),
                        /root/SynTypeOrInsideSynExprTraitCall.fs (2,26--2,28),
                        [],
                        { ColonRange =
                           Some
                             /root/SynTypeOrInsideSynExprTraitCall.fs (2,24--2,25) })),
                  Typed
                    (Paren
                       (TraitCall
                          (Paren
                             (Or
                                (Var
                                   (SynTypar (a, HeadType, false),
                                    /root/SynTypeOrInsideSynExprTraitCall.fs (2,33--2,35)),
                                 Var
                                   (SynTypar (b, HeadType, false),
                                    /root/SynTypeOrInsideSynExprTraitCall.fs (2,39--2,41)),
                                 /root/SynTypeOrInsideSynExprTraitCall.fs (2,33--2,41),
                                 { OrKeyword =
                                    /root/SynTypeOrInsideSynExprTraitCall.fs (2,36--2,38) }),
                              /root/SynTypeOrInsideSynExprTraitCall.fs (2,32--2,42)),
                           Member
                             (SynValSig
                                ([], SynIdent (op_Implicit, None),
                                 SynValTyparDecls (None, true),
                                 Fun
                                   (Var
                                      (SynTypar (a, HeadType, false),
                                       /root/SynTypeOrInsideSynExprTraitCall.fs (2,72--2,74)),
                                    Var
                                      (SynTypar (b, HeadType, false),
                                       /root/SynTypeOrInsideSynExprTraitCall.fs (2,78--2,80)),
                                    /root/SynTypeOrInsideSynExprTraitCall.fs (2,72--2,80),
                                    { ArrowRange =
                                       /root/SynTypeOrInsideSynExprTraitCall.fs (2,75--2,77) }),
                                 SynValInfo
                                   ([[SynArgInfo ([], false, None)]],
                                    SynArgInfo ([], false, None)), false, false,
                                 PreXmlDoc ((2,45), FSharp.Compiler.Xml.XmlDocCollector),
                                 None, None,
                                 /root/SynTypeOrInsideSynExprTraitCall.fs (2,45--2,80),
                                 { LeadingKeyword =
                                    StaticMember
                                      (/root/SynTypeOrInsideSynExprTraitCall.fs (2,45--2,51),
                                       /root/SynTypeOrInsideSynExprTraitCall.fs (2,52--2,58))
                                   InlineKeyword = None
                                   WithKeyword = None
                                   EqualsRange = None }),
                              { IsInstance = false
                                IsDispatchSlot = false
                                IsOverrideOrExplicitImpl = false
                                IsFinal = false
                                GetterOrSetterIsCompilerGenerated = false
                                MemberKind = Member },
                              /root/SynTypeOrInsideSynExprTraitCall.fs (2,45--2,80),
                              { GetSetKeywords = None }), Ident x,
                           /root/SynTypeOrInsideSynExprTraitCall.fs (2,31--2,84)),
                        /root/SynTypeOrInsideSynExprTraitCall.fs (2,31--2,32),
                        Some
                          /root/SynTypeOrInsideSynExprTraitCall.fs (2,83--2,84),
                        /root/SynTypeOrInsideSynExprTraitCall.fs (2,31--2,84)),
                     Var
                       (SynTypar (b, HeadType, false),
                        /root/SynTypeOrInsideSynExprTraitCall.fs (2,26--2,28)),
                     /root/SynTypeOrInsideSynExprTraitCall.fs (2,31--2,84)),
                  /root/SynTypeOrInsideSynExprTraitCall.fs (2,11--2,23),
                  NoneAtLet,
                  { LeadingKeyword =
                     Let /root/SynTypeOrInsideSynExprTraitCall.fs (2,0--2,3)
                    InlineKeyword =
                     Some /root/SynTypeOrInsideSynExprTraitCall.fs (2,4--2,10)
                    EqualsRange =
                     Some /root/SynTypeOrInsideSynExprTraitCall.fs (2,29--2,30) })],
              /root/SynTypeOrInsideSynExprTraitCall.fs (2,0--2,84))],
          PreXmlDocEmpty, [], None,
          /root/SynTypeOrInsideSynExprTraitCall.fs (2,0--3,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))