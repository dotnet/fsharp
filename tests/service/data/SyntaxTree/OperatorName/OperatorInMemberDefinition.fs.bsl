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
                     false, None,
                     /root/OperatorName/OperatorInMemberDefinition.fs (2,5--2,6)),
                  ObjectModel
                    (Augmentation
                       /root/OperatorName/OperatorInMemberDefinition.fs (2,7--2,11),
                     [],
                     /root/OperatorName/OperatorInMemberDefinition.fs (2,5--3,28)),
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
                              ([_; op_Addition],
                               [/root/OperatorName/OperatorInMemberDefinition.fs (3,12--3,13)],
                               [None;
                                Some
                                  (OriginalNotationWithParen
                                     (/root/OperatorName/OperatorInMemberDefinition.fs (3,13--3,14),
                                      "+",
                                      /root/OperatorName/OperatorInMemberDefinition.fs (3,15--3,16)))]),
                            None, None,
                            Pats
                              [Named
                                 (SynIdent (a, None), false, None,
                                  /root/OperatorName/OperatorInMemberDefinition.fs (3,17--3,18));
                               Named
                                 (SynIdent (b, None), false, None,
                                  /root/OperatorName/OperatorInMemberDefinition.fs (3,19--3,20))],
                            None,
                            /root/OperatorName/OperatorInMemberDefinition.fs (3,11--3,20)),
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
                                  /root/OperatorName/OperatorInMemberDefinition.fs (3,25--3,26)),
                               Ident a,
                               /root/OperatorName/OperatorInMemberDefinition.fs (3,23--3,26)),
                            Ident b,
                            /root/OperatorName/OperatorInMemberDefinition.fs (3,23--3,28)),
                         /root/OperatorName/OperatorInMemberDefinition.fs (3,11--3,20),
                         NoneAtInvisible,
                         { LeadingKeyword =
                            Member
                              /root/OperatorName/OperatorInMemberDefinition.fs (3,4--3,10)
                           InlineKeyword = None
                           EqualsRange =
                            Some
                              /root/OperatorName/OperatorInMemberDefinition.fs (3,21--3,22) }),
                      /root/OperatorName/OperatorInMemberDefinition.fs (3,4--3,28))],
                  None,
                  /root/OperatorName/OperatorInMemberDefinition.fs (2,5--3,28),
                  { LeadingKeyword =
                     Type
                       /root/OperatorName/OperatorInMemberDefinition.fs (2,0--2,4)
                    EqualsRange = None
                    WithKeyword = None })],
              /root/OperatorName/OperatorInMemberDefinition.fs (2,0--3,28))],
          PreXmlDocEmpty, [], None,
          /root/OperatorName/OperatorInMemberDefinition.fs (2,0--4,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))