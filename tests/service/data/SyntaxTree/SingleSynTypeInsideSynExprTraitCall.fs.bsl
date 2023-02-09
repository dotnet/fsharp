ImplFile
  (ParsedImplFileInput
     ("/root/SingleSynTypeInsideSynExprTraitCall.fs", false,
      QualifiedNameOfFile SingleSynTypeInsideSynExprTraitCall, [], [],
      [SynModuleOrNamespace
         ([SingleSynTypeInsideSynExprTraitCall], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/SingleSynTypeInsideSynExprTraitCall.fs (1,5--1,6)),
                  ObjectModel
                    (Unspecified,
                     [Member
                        (SynBinding
                           (None, Normal, true, false, [],
                            PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = false
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo
                                 ([[SynArgInfo ([], false, Some a);
                                    SynArgInfo ([], false, Some f)]],
                                  SynArgInfo ([], false, None)), None),
                            LongIdent
                              (SynLongIdent ([replace], [], [None]), None,
                               Some
                                 (SynValTyparDecls
                                    (Some
                                       (PostfixList
                                          ([SynTyparDecl
                                              ([], SynTypar (a, HeadType, false));
                                            SynTyparDecl
                                              ([], SynTypar (b, HeadType, false));
                                            SynTyparDecl
                                              ([], SynTypar (c, HeadType, false))],
                                           [WhereTyparSupportsMember
                                              (Var
                                                 (SynTypar (b, HeadType, false),
                                                  /root/SingleSynTypeInsideSynExprTraitCall.fs (2,50--2,52)),
                                               Member
                                                 (SynValSig
                                                    ([],
                                                     SynIdent (replace, None),
                                                     SynValTyparDecls
                                                       (None, true),
                                                     Fun
                                                       (Tuple
                                                          (false,
                                                           [Type
                                                              (Var
                                                                 (SynTypar
                                                                    (a, HeadType,
                                                                     false),
                                                                  /root/SingleSynTypeInsideSynExprTraitCall.fs (2,78--2,80)));
                                                            Star
                                                              /root/SingleSynTypeInsideSynExprTraitCall.fs (2,81--2,82);
                                                            Type
                                                              (Var
                                                                 (SynTypar
                                                                    (b, HeadType,
                                                                     false),
                                                                  /root/SingleSynTypeInsideSynExprTraitCall.fs (2,83--2,85)))],
                                                           /root/SingleSynTypeInsideSynExprTraitCall.fs (2,78--2,85)),
                                                        Var
                                                          (SynTypar
                                                             (c, HeadType, false),
                                                           /root/SingleSynTypeInsideSynExprTraitCall.fs (2,89--2,91)),
                                                        /root/SingleSynTypeInsideSynExprTraitCall.fs (2,78--2,91),
                                                        { ArrowRange =
                                                           /root/SingleSynTypeInsideSynExprTraitCall.fs (2,86--2,88) }),
                                                     SynValInfo
                                                       ([[SynArgInfo
                                                            ([], false, None);
                                                          SynArgInfo
                                                            ([], false, None)]],
                                                        SynArgInfo
                                                          ([], false, None)),
                                                     false, false,
                                                     PreXmlDoc ((2,55), FSharp.Compiler.Xml.XmlDocCollector),
                                                     None, None,
                                                     /root/SingleSynTypeInsideSynExprTraitCall.fs (2,55--2,91),
                                                     { LeadingKeyword =
                                                        StaticMember
                                                          (/root/SingleSynTypeInsideSynExprTraitCall.fs (2,55--2,61),
                                                           /root/SingleSynTypeInsideSynExprTraitCall.fs (2,62--2,68))
                                                       InlineKeyword = None
                                                       WithKeyword = None
                                                       EqualsRange = None }),
                                                  { IsInstance = false
                                                    IsDispatchSlot = false
                                                    IsOverrideOrExplicitImpl =
                                                     false
                                                    IsFinal = false
                                                    GetterOrSetterIsCompilerGenerated =
                                                     false
                                                    MemberKind = Member },
                                                  /root/SingleSynTypeInsideSynExprTraitCall.fs (2,55--2,91),
                                                  { GetSetKeywords = None }),
                                               /root/SingleSynTypeInsideSynExprTraitCall.fs (2,50--2,92))],
                                           /root/SingleSynTypeInsideSynExprTraitCall.fs (2,32--2,93))),
                                     false)),
                               Pats
                                 [Paren
                                    (Tuple
                                       (false,
                                        [Typed
                                           (Named
                                              (SynIdent (a, None), false, None,
                                               /root/SingleSynTypeInsideSynExprTraitCall.fs (2,94--2,95)),
                                            Var
                                              (SynTypar (a, HeadType, false),
                                               /root/SingleSynTypeInsideSynExprTraitCall.fs (2,97--2,99)),
                                            /root/SingleSynTypeInsideSynExprTraitCall.fs (2,94--2,99));
                                         Typed
                                           (Named
                                              (SynIdent (f, None), false, None,
                                               /root/SingleSynTypeInsideSynExprTraitCall.fs (2,101--2,102)),
                                            Var
                                              (SynTypar (b, HeadType, false),
                                               /root/SingleSynTypeInsideSynExprTraitCall.fs (2,104--2,106)),
                                            /root/SingleSynTypeInsideSynExprTraitCall.fs (2,101--2,106))],
                                        /root/SingleSynTypeInsideSynExprTraitCall.fs (2,94--2,106)),
                                     /root/SingleSynTypeInsideSynExprTraitCall.fs (2,93--2,107))],
                               None,
                               /root/SingleSynTypeInsideSynExprTraitCall.fs (2,25--2,107)),
                            None,
                            Paren
                              (TraitCall
                                 (Var
                                    (SynTypar (b, HeadType, false),
                                     /root/SingleSynTypeInsideSynExprTraitCall.fs (3,9--3,11)),
                                  Member
                                    (SynValSig
                                       ([], SynIdent (replace, None),
                                        SynValTyparDecls (None, true),
                                        Fun
                                          (Tuple
                                             (false,
                                              [Type
                                                 (Var
                                                    (SynTypar
                                                       (a, HeadType, false),
                                                     /root/SingleSynTypeInsideSynExprTraitCall.fs (3,38--3,40)));
                                               Star
                                                 /root/SingleSynTypeInsideSynExprTraitCall.fs (3,41--3,42);
                                               Type
                                                 (Var
                                                    (SynTypar
                                                       (b, HeadType, false),
                                                     /root/SingleSynTypeInsideSynExprTraitCall.fs (3,43--3,45)))],
                                              /root/SingleSynTypeInsideSynExprTraitCall.fs (3,38--3,45)),
                                           Var
                                             (SynTypar (c, HeadType, false),
                                              /root/SingleSynTypeInsideSynExprTraitCall.fs (3,49--3,51)),
                                           /root/SingleSynTypeInsideSynExprTraitCall.fs (3,38--3,51),
                                           { ArrowRange =
                                              /root/SingleSynTypeInsideSynExprTraitCall.fs (3,46--3,48) }),
                                        SynValInfo
                                          ([[SynArgInfo ([], false, None);
                                             SynArgInfo ([], false, None)]],
                                           SynArgInfo ([], false, None)), false,
                                        false,
                                        PreXmlDoc ((3,15), FSharp.Compiler.Xml.XmlDocCollector),
                                        None, None,
                                        /root/SingleSynTypeInsideSynExprTraitCall.fs (3,15--3,51),
                                        { LeadingKeyword =
                                           StaticMember
                                             (/root/SingleSynTypeInsideSynExprTraitCall.fs (3,15--3,21),
                                              /root/SingleSynTypeInsideSynExprTraitCall.fs (3,22--3,28))
                                          InlineKeyword = None
                                          WithKeyword = None
                                          EqualsRange = None }),
                                     { IsInstance = false
                                       IsDispatchSlot = false
                                       IsOverrideOrExplicitImpl = false
                                       IsFinal = false
                                       GetterOrSetterIsCompilerGenerated = false
                                       MemberKind = Member },
                                     /root/SingleSynTypeInsideSynExprTraitCall.fs (3,15--3,51),
                                     { GetSetKeywords = None }),
                                  Paren
                                    (Tuple
                                       (false, [Ident a; Ident f],
                                        [/root/SingleSynTypeInsideSynExprTraitCall.fs (3,55--3,56)],
                                        /root/SingleSynTypeInsideSynExprTraitCall.fs (3,54--3,58)),
                                     /root/SingleSynTypeInsideSynExprTraitCall.fs (3,53--3,54),
                                     Some
                                       /root/SingleSynTypeInsideSynExprTraitCall.fs (3,58--3,59),
                                     /root/SingleSynTypeInsideSynExprTraitCall.fs (3,53--3,59)),
                                  /root/SingleSynTypeInsideSynExprTraitCall.fs (3,8--3,60)),
                               /root/SingleSynTypeInsideSynExprTraitCall.fs (3,8--3,9),
                               Some
                                 /root/SingleSynTypeInsideSynExprTraitCall.fs (3,59--3,60),
                               /root/SingleSynTypeInsideSynExprTraitCall.fs (3,8--3,60)),
                            /root/SingleSynTypeInsideSynExprTraitCall.fs (2,25--2,107),
                            NoneAtInvisible,
                            { LeadingKeyword =
                               StaticMember
                                 (/root/SingleSynTypeInsideSynExprTraitCall.fs (2,4--2,10),
                                  /root/SingleSynTypeInsideSynExprTraitCall.fs (2,11--2,17))
                              InlineKeyword =
                               Some
                                 /root/SingleSynTypeInsideSynExprTraitCall.fs (2,18--2,24)
                              EqualsRange =
                               Some
                                 /root/SingleSynTypeInsideSynExprTraitCall.fs (2,108--2,109) }),
                         /root/SingleSynTypeInsideSynExprTraitCall.fs (2,4--3,60))],
                     /root/SingleSynTypeInsideSynExprTraitCall.fs (2,4--3,60)),
                  [], None,
                  /root/SingleSynTypeInsideSynExprTraitCall.fs (1,5--3,60),
                  { LeadingKeyword =
                     Type
                       /root/SingleSynTypeInsideSynExprTraitCall.fs (1,0--1,4)
                    EqualsRange =
                     Some
                       /root/SingleSynTypeInsideSynExprTraitCall.fs (1,7--1,8)
                    WithKeyword = None })],
              /root/SingleSynTypeInsideSynExprTraitCall.fs (1,0--3,60))],
          PreXmlDocEmpty, [], None,
          /root/SingleSynTypeInsideSynExprTraitCall.fs (1,0--3,60),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))