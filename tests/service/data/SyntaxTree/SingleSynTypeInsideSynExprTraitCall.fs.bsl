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
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/SingleSynTypeInsideSynExprTraitCall.fs (2,5--2,6)),
                  ObjectModel
                    (Unspecified,
                     [Member
                        (SynBinding
                           (None, Normal, true, false, [],
                            PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                                                  /root/SingleSynTypeInsideSynExprTraitCall.fs (3,50--3,52)),
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
                                                                  /root/SingleSynTypeInsideSynExprTraitCall.fs (3,78--3,80)));
                                                            Star
                                                              /root/SingleSynTypeInsideSynExprTraitCall.fs (3,81--3,82);
                                                            Type
                                                              (Var
                                                                 (SynTypar
                                                                    (b, HeadType,
                                                                     false),
                                                                  /root/SingleSynTypeInsideSynExprTraitCall.fs (3,83--3,85)))],
                                                           /root/SingleSynTypeInsideSynExprTraitCall.fs (3,78--3,85)),
                                                        Var
                                                          (SynTypar
                                                             (c, HeadType, false),
                                                           /root/SingleSynTypeInsideSynExprTraitCall.fs (3,89--3,91)),
                                                        /root/SingleSynTypeInsideSynExprTraitCall.fs (3,78--3,91),
                                                        { ArrowRange =
                                                           /root/SingleSynTypeInsideSynExprTraitCall.fs (3,86--3,88) }),
                                                     SynValInfo
                                                       ([[SynArgInfo
                                                            ([], false, None);
                                                          SynArgInfo
                                                            ([], false, None)]],
                                                        SynArgInfo
                                                          ([], false, None)),
                                                     false, false,
                                                     PreXmlDoc ((3,55), FSharp.Compiler.Xml.XmlDocCollector),
                                                     None, None,
                                                     /root/SingleSynTypeInsideSynExprTraitCall.fs (3,55--3,91),
                                                     { LeadingKeyword =
                                                        StaticMember
                                                          (/root/SingleSynTypeInsideSynExprTraitCall.fs (3,55--3,61),
                                                           /root/SingleSynTypeInsideSynExprTraitCall.fs (3,62--3,68))
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
                                                  /root/SingleSynTypeInsideSynExprTraitCall.fs (3,55--3,91),
                                                  { GetSetKeywords = None }),
                                               /root/SingleSynTypeInsideSynExprTraitCall.fs (3,50--3,92))],
                                           /root/SingleSynTypeInsideSynExprTraitCall.fs (3,32--3,93))),
                                     false)),
                               Pats
                                 [Paren
                                    (Tuple
                                       (false,
                                        [Typed
                                           (Named
                                              (SynIdent (a, None), false, None,
                                               /root/SingleSynTypeInsideSynExprTraitCall.fs (3,94--3,95)),
                                            Var
                                              (SynTypar (a, HeadType, false),
                                               /root/SingleSynTypeInsideSynExprTraitCall.fs (3,97--3,99)),
                                            /root/SingleSynTypeInsideSynExprTraitCall.fs (3,94--3,99));
                                         Typed
                                           (Named
                                              (SynIdent (f, None), false, None,
                                               /root/SingleSynTypeInsideSynExprTraitCall.fs (3,101--3,102)),
                                            Var
                                              (SynTypar (b, HeadType, false),
                                               /root/SingleSynTypeInsideSynExprTraitCall.fs (3,104--3,106)),
                                            /root/SingleSynTypeInsideSynExprTraitCall.fs (3,101--3,106))],
                                        /root/SingleSynTypeInsideSynExprTraitCall.fs (3,94--3,106)),
                                     /root/SingleSynTypeInsideSynExprTraitCall.fs (3,93--3,107))],
                               None,
                               /root/SingleSynTypeInsideSynExprTraitCall.fs (3,25--3,107)),
                            None,
                            Paren
                              (TraitCall
                                 (Var
                                    (SynTypar (b, HeadType, false),
                                     /root/SingleSynTypeInsideSynExprTraitCall.fs (4,9--4,11)),
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
                                                     /root/SingleSynTypeInsideSynExprTraitCall.fs (4,38--4,40)));
                                               Star
                                                 /root/SingleSynTypeInsideSynExprTraitCall.fs (4,41--4,42);
                                               Type
                                                 (Var
                                                    (SynTypar
                                                       (b, HeadType, false),
                                                     /root/SingleSynTypeInsideSynExprTraitCall.fs (4,43--4,45)))],
                                              /root/SingleSynTypeInsideSynExprTraitCall.fs (4,38--4,45)),
                                           Var
                                             (SynTypar (c, HeadType, false),
                                              /root/SingleSynTypeInsideSynExprTraitCall.fs (4,49--4,51)),
                                           /root/SingleSynTypeInsideSynExprTraitCall.fs (4,38--4,51),
                                           { ArrowRange =
                                              /root/SingleSynTypeInsideSynExprTraitCall.fs (4,46--4,48) }),
                                        SynValInfo
                                          ([[SynArgInfo ([], false, None);
                                             SynArgInfo ([], false, None)]],
                                           SynArgInfo ([], false, None)), false,
                                        false,
                                        PreXmlDoc ((4,15), FSharp.Compiler.Xml.XmlDocCollector),
                                        None, None,
                                        /root/SingleSynTypeInsideSynExprTraitCall.fs (4,15--4,51),
                                        { LeadingKeyword =
                                           StaticMember
                                             (/root/SingleSynTypeInsideSynExprTraitCall.fs (4,15--4,21),
                                              /root/SingleSynTypeInsideSynExprTraitCall.fs (4,22--4,28))
                                          InlineKeyword = None
                                          WithKeyword = None
                                          EqualsRange = None }),
                                     { IsInstance = false
                                       IsDispatchSlot = false
                                       IsOverrideOrExplicitImpl = false
                                       IsFinal = false
                                       GetterOrSetterIsCompilerGenerated = false
                                       MemberKind = Member },
                                     /root/SingleSynTypeInsideSynExprTraitCall.fs (4,15--4,51),
                                     { GetSetKeywords = None }),
                                  Paren
                                    (Tuple
                                       (false, [Ident a; Ident f],
                                        [/root/SingleSynTypeInsideSynExprTraitCall.fs (4,55--4,56)],
                                        /root/SingleSynTypeInsideSynExprTraitCall.fs (4,54--4,58)),
                                     /root/SingleSynTypeInsideSynExprTraitCall.fs (4,53--4,54),
                                     Some
                                       /root/SingleSynTypeInsideSynExprTraitCall.fs (4,58--4,59),
                                     /root/SingleSynTypeInsideSynExprTraitCall.fs (4,53--4,59)),
                                  /root/SingleSynTypeInsideSynExprTraitCall.fs (4,8--4,60)),
                               /root/SingleSynTypeInsideSynExprTraitCall.fs (4,8--4,9),
                               Some
                                 /root/SingleSynTypeInsideSynExprTraitCall.fs (4,59--4,60),
                               /root/SingleSynTypeInsideSynExprTraitCall.fs (4,8--4,60)),
                            /root/SingleSynTypeInsideSynExprTraitCall.fs (3,25--3,107),
                            NoneAtInvisible,
                            { LeadingKeyword =
                               StaticMember
                                 (/root/SingleSynTypeInsideSynExprTraitCall.fs (3,4--3,10),
                                  /root/SingleSynTypeInsideSynExprTraitCall.fs (3,11--3,17))
                              InlineKeyword =
                               Some
                                 /root/SingleSynTypeInsideSynExprTraitCall.fs (3,18--3,24)
                              EqualsRange =
                               Some
                                 /root/SingleSynTypeInsideSynExprTraitCall.fs (3,108--3,109) }),
                         /root/SingleSynTypeInsideSynExprTraitCall.fs (3,4--4,60))],
                     /root/SingleSynTypeInsideSynExprTraitCall.fs (3,4--4,60)),
                  [], None,
                  /root/SingleSynTypeInsideSynExprTraitCall.fs (2,5--4,60),
                  { LeadingKeyword =
                     Type
                       /root/SingleSynTypeInsideSynExprTraitCall.fs (2,0--2,4)
                    EqualsRange =
                     Some
                       /root/SingleSynTypeInsideSynExprTraitCall.fs (2,7--2,8)
                    WithKeyword = None })],
              /root/SingleSynTypeInsideSynExprTraitCall.fs (2,0--4,60))],
          PreXmlDocEmpty, [], None,
          /root/SingleSynTypeInsideSynExprTraitCall.fs (2,0--5,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))