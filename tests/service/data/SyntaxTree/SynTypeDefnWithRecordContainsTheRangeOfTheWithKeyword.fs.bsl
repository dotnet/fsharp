ImplFile
  (ParsedImplFileInput
     ("/root/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs", false,
      QualifiedNameOfFile SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword,
      [], [],
      [SynModuleOrNamespace
         ([SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword], false,
          AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (1,5--1,8)),
                  Simple
                    (Record
                       (None,
                        [SynField
                           ([], false, Some Bar,
                            LongIdent (SynLongIdent ([int], [], [None])), false,
                            PreXmlDoc ((2,6), FSharp.Compiler.Xml.XmlDocCollector),
                            None,
                            /root/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (2,6--2,15),
                            { LeadingKeyword = None })],
                        /root/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (2,4--2,17)),
                     /root/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (2,4--2,17)),
                  [Member
                     (SynBinding
                        (None, Normal, false, false, [],
                         PreXmlDoc ((4,8), FSharp.Compiler.Xml.XmlDocCollector),
                         SynValData
                           (Some { IsInstance = true
                                   IsDispatchSlot = false
                                   IsOverrideOrExplicitImpl = false
                                   IsFinal = false
                                   GetterOrSetterIsCompilerGenerated = false
                                   MemberKind = Member },
                            SynValInfo
                              ([[SynArgInfo ([], false, None)];
                                [SynArgInfo ([], false, Some v)]],
                               SynArgInfo ([], false, None)), None),
                         LongIdent
                           (SynLongIdent
                              ([this; Meh],
                               [/root/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (4,19--4,20)],
                               [None; None]), None, None,
                            Pats
                              [Paren
                                 (Typed
                                    (Named
                                       (SynIdent (v, None), false, None,
                                        /root/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (4,25--4,26)),
                                     LongIdent
                                       (SynLongIdent ([int], [], [None])),
                                     /root/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (4,25--4,30)),
                                  /root/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (4,24--4,31))],
                            None,
                            /root/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (4,15--4,31)),
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
                                  /root/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (4,43--4,44)),
                               LongIdent
                                 (false,
                                  SynLongIdent
                                    ([this; Bar],
                                     [/root/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (4,38--4,39)],
                                     [None; None]), None,
                                  /root/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (4,34--4,42)),
                               /root/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (4,34--4,44)),
                            Ident v,
                            /root/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (4,34--4,46)),
                         /root/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (4,15--4,31),
                         NoneAtInvisible,
                         { LeadingKeyword =
                            Member
                              /root/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (4,8--4,14)
                           InlineKeyword = None
                           EqualsRange =
                            Some
                              /root/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (4,32--4,33) }),
                      /root/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (4,8--4,46))],
                  None,
                  /root/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (1,5--4,46),
                  { LeadingKeyword =
                     Type
                       /root/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (1,0--1,4)
                    EqualsRange =
                     Some
                       /root/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (1,9--1,10)
                    WithKeyword =
                     Some
                       /root/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (3,4--3,8) })],
              /root/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (1,0--4,46))],
          PreXmlDocEmpty, [], None,
          /root/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (1,0--4,46),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))