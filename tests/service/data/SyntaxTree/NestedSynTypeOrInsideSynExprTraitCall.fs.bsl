ImplFile
  (ParsedImplFileInput
     ("/root/NestedSynTypeOrInsideSynExprTraitCall.fs", false,
      QualifiedNameOfFile NestedSynTypeOrInsideSynExprTraitCall, [], [],
      [SynModuleOrNamespace
         ([NestedSynTypeOrInsideSynExprTraitCall], false, AnonModule,
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
                              (/root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,11--1,12),
                               "!!",
                               /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,14--1,15)))]),
                     None, None,
                     Pats
                       [Paren
                          (Typed
                             (Named
                                (SynIdent (x, None), false, None,
                                 /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,17--1,18)),
                              Tuple
                                (false,
                                 [Type
                                    (Var
                                       (SynTypar (a, HeadType, false),
                                        /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,20--1,22)));
                                  Star
                                    /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,23--1,24);
                                  Type
                                    (Var
                                       (SynTypar (b, HeadType, false),
                                        /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,25--1,27)))],
                                 /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,20--1,27)),
                              /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,17--1,27)),
                           /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,16--1,28))],
                     None,
                     /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,11--1,28)),
                  Some
                    (SynBindingReturnInfo
                       (Var
                          (SynTypar (c, HeadType, false),
                           /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,31--1,33)),
                        /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,31--1,33),
                        [],
                        { ColonRange =
                           Some
                             /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,29--1,30) })),
                  Typed
                    (Paren
                       (TraitCall
                          (Paren
                             (Or
                                (Or
                                   (Var
                                      (SynTypar (a, HeadType, false),
                                       /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,38--1,40)),
                                    Var
                                      (SynTypar (b, HeadType, false),
                                       /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,44--1,46)),
                                    /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,38--1,46),
                                    { OrKeyword =
                                       /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,41--1,43) }),
                                 Var
                                   (SynTypar (c, HeadType, false),
                                    /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,50--1,52)),
                                 /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,38--1,52),
                                 { OrKeyword =
                                    /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,47--1,49) }),
                              /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,37--1,53)),
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
                                              /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,83--1,85)));
                                        Star
                                          /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,86--1,87);
                                        Type
                                          (Var
                                             (SynTypar (b, HeadType, false),
                                              /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,88--1,90)))],
                                       /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,83--1,90)),
                                    Var
                                      (SynTypar (c, HeadType, false),
                                       /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,94--1,96)),
                                    /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,83--1,96),
                                    { ArrowRange =
                                       /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,91--1,93) }),
                                 SynValInfo
                                   ([[SynArgInfo ([], false, None);
                                      SynArgInfo ([], false, None)]],
                                    SynArgInfo ([], false, None)), false, false,
                                 PreXmlDoc ((1,56), FSharp.Compiler.Xml.XmlDocCollector),
                                 None, None,
                                 /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,56--1,96),
                                 { LeadingKeyword =
                                    StaticMember
                                      (/root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,56--1,62),
                                       /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,63--1,69))
                                   InlineKeyword = None
                                   WithKeyword = None
                                   EqualsRange = None }),
                              { IsInstance = false
                                IsDispatchSlot = false
                                IsOverrideOrExplicitImpl = false
                                IsFinal = false
                                GetterOrSetterIsCompilerGenerated = false
                                MemberKind = Member },
                              /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,56--1,96),
                              { GetSetKeywords = None }), Ident x,
                           /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,36--1,100)),
                        /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,36--1,37),
                        Some
                          /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,99--1,100),
                        /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,36--1,100)),
                     Var
                       (SynTypar (c, HeadType, false),
                        /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,31--1,33)),
                     /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,36--1,100)),
                  /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,11--1,28),
                  NoneAtLet,
                  { LeadingKeyword =
                     Let
                       /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,0--1,3)
                    InlineKeyword =
                     Some
                       /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,4--1,10)
                    EqualsRange =
                     Some
                       /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,34--1,35) })],
              /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,0--1,100))],
          PreXmlDocEmpty, [], None,
          /root/NestedSynTypeOrInsideSynExprTraitCall.fs (1,0--1,100),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))