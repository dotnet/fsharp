ImplFile
  (ParsedImplFileInput
     ("/root/Type/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs",
      false,
      QualifiedNameOfFile SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword,
      [], [],
      [SynModuleOrNamespace
         ([SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword], false,
          AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/Type/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (2,5--2,8)),
                  Simple
                    (Record
                       (None,
                        [SynField
                           ([], false, Some Bar,
                            LongIdent (SynLongIdent ([int], [], [None])), false,
                            PreXmlDoc ((3,6), FSharp.Compiler.Xml.XmlDocCollector),
                            None,
                            /root/Type/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (3,6--3,15),
                            { LeadingKeyword = None })],
                        /root/Type/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (3,4--3,17)),
                     /root/Type/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (3,4--3,17)),
                  [Member
                     (SynBinding
                        (None, Normal, false, false, [],
                         PreXmlDoc ((5,8), FSharp.Compiler.Xml.XmlDocCollector),
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
                               [/root/Type/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (5,19--5,20)],
                               [None; None]), None, None,
                            Pats
                              [Paren
                                 (Typed
                                    (Named
                                       (SynIdent (v, None), false, None,
                                        /root/Type/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (5,25--5,26)),
                                     LongIdent
                                       (SynLongIdent ([int], [], [None])),
                                     /root/Type/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (5,25--5,30)),
                                  /root/Type/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (5,24--5,31))],
                            None,
                            /root/Type/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (5,15--5,31)),
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
                                  /root/Type/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (5,43--5,44)),
                               LongIdent
                                 (false,
                                  SynLongIdent
                                    ([this; Bar],
                                     [/root/Type/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (5,38--5,39)],
                                     [None; None]), None,
                                  /root/Type/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (5,34--5,42)),
                               /root/Type/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (5,34--5,44)),
                            Ident v,
                            /root/Type/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (5,34--5,46)),
                         /root/Type/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (5,15--5,31),
                         NoneAtInvisible,
                         { LeadingKeyword =
                            Member
                              /root/Type/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (5,8--5,14)
                           InlineKeyword = None
                           EqualsRange =
                            Some
                              /root/Type/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (5,32--5,33) }),
                      /root/Type/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (5,8--5,46))],
                  None,
                  /root/Type/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (2,5--5,46),
                  { LeadingKeyword =
                     Type
                       /root/Type/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (2,0--2,4)
                    EqualsRange =
                     Some
                       /root/Type/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (2,9--2,10)
                    WithKeyword =
                     Some
                       /root/Type/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (4,4--4,8) })],
              /root/Type/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (2,0--5,46))],
          PreXmlDocEmpty, [], None,
          /root/Type/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs (2,0--6,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
