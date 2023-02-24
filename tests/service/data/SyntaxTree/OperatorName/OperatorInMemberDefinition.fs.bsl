ImplFile
  (ParsedImplFileInput
     ("/root/OperatorName/OperatorInMemberDefinition.fs", false,
      QualifiedNameOfFile OperatorInMemberDefinition, [], [],
      [SynModuleOrNamespace
         ([OperatorInMemberDefinition], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,6)),
                  ObjectModel (Augmentation (2,7--2,11), [], (2,5--3,28)),
                  [Member
                     (SynBinding
                        (None, Normal, false, false, [],
                         PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                         SynValData
                           (Some { IsInstance = true
                                   IsDispatchSlot = false
                                   IsOverrideOrExplicitImpl = false
                                   IsFinal = false
                                   GetterOrSetterIsCompilerGenerated = false
                                   MemberKind = Member },
                            SynValInfo
                              ([[SynArgInfo ([], false, None)];
                                [SynArgInfo ([], false, Some a)];
                                [SynArgInfo ([], false, Some b)]],
                               SynArgInfo ([], false, None)), None),
                         LongIdent
                           (SynLongIdent
                              ([_; op_Addition], [(3,12--3,13)],
                               [None;
                                Some
                                  (OriginalNotationWithParen
                                     ((3,13--3,14), "+", (3,15--3,16)))]), None,
                            None,
                            Pats
                              [Named
                                 (SynIdent (a, None), false, None, (3,17--3,18));
                               Named
                                 (SynIdent (b, None), false, None, (3,19--3,20))],
                            None, (3,11--3,20)), None,
                         App
                           (NonAtomic, false,
                            App
                              (NonAtomic, true,
                               LongIdent
                                 (false,
                                  SynLongIdent
                                    ([op_Addition], [],
                                     [Some (OriginalNotation "+")]), None,
                                  (3,25--3,26)), Ident a, (3,23--3,26)), Ident b,
                            (3,23--3,28)), (3,11--3,20), NoneAtInvisible,
                         { LeadingKeyword = Member (3,4--3,10)
                           InlineKeyword = None
                           EqualsRange = Some (3,21--3,22) }), (3,4--3,28))],
                  None, (2,5--3,28), { LeadingKeyword = Type (2,0--2,4)
                                       EqualsRange = None
                                       WithKeyword = None })], (2,0--3,28))],
          PreXmlDocEmpty, [], None, (2,0--4,0), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))
