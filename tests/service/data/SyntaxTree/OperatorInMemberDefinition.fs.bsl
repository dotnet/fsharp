ImplFile
  (ParsedImplFileInput
     ("/root/OperatorInMemberDefinition.fs", false,
      QualifiedNameOfFile OperatorInMemberDefinition, [], [],
      [SynModuleOrNamespace
         ([OperatorInMemberDefinition], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, /root/OperatorInMemberDefinition.fs (1,5--1,6)),
                  ObjectModel
                    (Augmentation
                       /root/OperatorInMemberDefinition.fs (1,7--1,11), [],
                     /root/OperatorInMemberDefinition.fs (1,5--2,28)),
                  [Member
                     (SynBinding
                        (None, Normal, false, false, [],
                         PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                              ([_; op_Addition],
                               [/root/OperatorInMemberDefinition.fs (2,12--2,13)],
                               [None;
                                Some
                                  (OriginalNotationWithParen
                                     (/root/OperatorInMemberDefinition.fs (2,13--2,14),
                                      "+",
                                      /root/OperatorInMemberDefinition.fs (2,15--2,16)))]),
                            None, None,
                            Pats
                              [Named
                                 (SynIdent (a, None), false, None,
                                  /root/OperatorInMemberDefinition.fs (2,17--2,18));
                               Named
                                 (SynIdent (b, None), false, None,
                                  /root/OperatorInMemberDefinition.fs (2,19--2,20))],
                            None,
                            /root/OperatorInMemberDefinition.fs (2,11--2,20)),
                         None,
                         App
                           (NonAtomic, false,
                            App
                              (NonAtomic, true,
                               LongIdent
                                 (false,
                                  SynLongIdent
                                    ([op_Addition], [],
                                     [Some (OriginalNotation "+")]), None,
                                  /root/OperatorInMemberDefinition.fs (2,25--2,26)),
                               Ident a,
                               /root/OperatorInMemberDefinition.fs (2,23--2,26)),
                            Ident b,
                            /root/OperatorInMemberDefinition.fs (2,23--2,28)),
                         /root/OperatorInMemberDefinition.fs (2,11--2,20),
                         NoneAtInvisible,
                         { LeadingKeyword =
                            Member
                              /root/OperatorInMemberDefinition.fs (2,4--2,10)
                           InlineKeyword = None
                           EqualsRange =
                            Some
                              /root/OperatorInMemberDefinition.fs (2,21--2,22) }),
                      /root/OperatorInMemberDefinition.fs (2,4--2,28))], None,
                  /root/OperatorInMemberDefinition.fs (1,5--2,28),
                  { LeadingKeyword =
                     Type /root/OperatorInMemberDefinition.fs (1,0--1,4)
                    EqualsRange = None
                    WithKeyword = None })],
              /root/OperatorInMemberDefinition.fs (1,0--2,28))], PreXmlDocEmpty,
          [], None, /root/OperatorInMemberDefinition.fs (1,0--2,28),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))