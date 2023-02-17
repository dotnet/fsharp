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
                              (/root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,11--2,12),
                               "!!",
                               /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,14--2,15)))]),
                     None, None,
                     Pats
                       [Paren
                          (Typed
                             (Named
                                (SynIdent (x, None), false, None,
                                 /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,17--2,18)),
                              Tuple
                                (false,
                                 [Type
                                    (Var
                                       (SynTypar (a, HeadType, false),
                                        /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,20--2,22)));
                                  Star
                                    /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,23--2,24);
                                  Type
                                    (Var
                                       (SynTypar (b, HeadType, false),
                                        /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,25--2,27)))],
                                 /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,20--2,27)),
                              /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,17--2,27)),
                           /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,16--2,28))],
                     None,
                     /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,11--2,28)),
                  Some
                    (SynBindingReturnInfo
                       (Var
                          (SynTypar (c, HeadType, false),
                           /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,31--2,33)),
                        /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,31--2,33),
                        [],
                        { ColonRange =
                           Some
                             /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,29--2,30) })),
                  Typed
                    (Paren
                       (TraitCall
                          (Paren
                             (Or
                                (Or
                                   (Var
                                      (SynTypar (a, HeadType, false),
                                       /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,38--2,40)),
                                    Var
                                      (SynTypar (b, HeadType, false),
                                       /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,44--2,46)),
                                    /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,38--2,46),
                                    { OrKeyword =
                                       /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,41--2,43) }),
                                 Var
                                   (SynTypar (c, HeadType, false),
                                    /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,50--2,52)),
                                 /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,38--2,52),
                                 { OrKeyword =
                                    /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,47--2,49) }),
                              /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,37--2,53)),
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
                                              /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,83--2,85)));
                                        Star
                                          /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,86--2,87);
                                        Type
                                          (Var
                                             (SynTypar (b, HeadType, false),
                                              /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,88--2,90)))],
                                       /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,83--2,90)),
                                    Var
                                      (SynTypar (c, HeadType, false),
                                       /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,94--2,96)),
                                    /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,83--2,96),
                                    { ArrowRange =
                                       /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,91--2,93) }),
                                 SynValInfo
                                   ([[SynArgInfo ([], false, None);
                                      SynArgInfo ([], false, None)]],
                                    SynArgInfo ([], false, None)), false, false,
                                 PreXmlDoc ((2,56), FSharp.Compiler.Xml.XmlDocCollector),
                                 None, None,
                                 /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,56--2,96),
                                 { LeadingKeyword =
                                    StaticMember
                                      (/root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,56--2,62),
                                       /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,63--2,69))
                                   InlineKeyword = None
                                   WithKeyword = None
                                   EqualsRange = None }),
                              { IsInstance = false
                                IsDispatchSlot = false
                                IsOverrideOrExplicitImpl = false
                                IsFinal = false
                                GetterOrSetterIsCompilerGenerated = false
                                MemberKind = Member },
                              /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,56--2,96),
                              { GetSetKeywords = None }), Ident x,
                           /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,36--2,100)),
                        /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,36--2,37),
                        Some
                          /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,99--2,100),
                        /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,36--2,100)),
                     Var
                       (SynTypar (c, HeadType, false),
                        /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,31--2,33)),
                     /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,36--2,100)),
                  /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,11--2,28),
                  NoneAtLet,
                  { LeadingKeyword =
                     Let
                       /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,0--2,3)
                    InlineKeyword =
                     Some
                       /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,4--2,10)
                    EqualsRange =
                     Some
                       /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,34--2,35) })],
              /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,0--2,100))],
          PreXmlDocEmpty, [], None,
          /root/SynType/NestedSynTypeOrInsideSynExprTraitCall.fs (2,0--3,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
