ImplFile
  (ParsedImplFileInput
     ("/root/Expression/NestedSynExprLetOrUseContainsTheRangeOfInKeyword.fs",
      false,
      QualifiedNameOfFile NestedSynExprLetOrUseContainsTheRangeOfInKeyword, [],
      [],
      [SynModuleOrNamespace
         ([NestedSynExprLetOrUseContainsTheRangeOfInKeyword], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([[]], SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([f], [], [None]), None, None,
                     Pats [Paren (Const (Unit, (2,6--2,8)), (2,6--2,8))], None,
                     (2,4--2,8)), None,
                  LetOrUse
                    (false, false,
                     [SynBinding
                        (None, Normal, false, false, [],
                         PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                         SynValData
                           (None, SynValInfo ([], SynArgInfo ([], false, None)),
                            None),
                         Named (SynIdent (x, None), false, None, (3,8--3,9)),
                         None, Const (Int32 1, (3,12--3,13)), (3,8--3,9),
                         Yes (3,4--3,13), { LeadingKeyword = Let (3,4--3,7)
                                            InlineKeyword = None
                                            EqualsRange = Some (3,10--3,11) })],
                     LetOrUse
                       (false, false,
                        [SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (None,
                               SynValInfo ([], SynArgInfo ([], false, None)),
                               None),
                            Named (SynIdent (y, None), false, None, (4,8--4,9)),
                            None, Const (Int32 2, (4,12--4,13)), (4,8--4,9),
                            Yes (4,4--4,13), { LeadingKeyword = Let (4,4--4,7)
                                               InlineKeyword = None
                                               EqualsRange = Some (4,10--4,11) })],
                        App
                          (NonAtomic, false,
                           App
                             (NonAtomic, true,
                              LongIdent
                                (false,
                                 SynLongIdent
                                   ([op_Addition], [],
                                    [Some (OriginalNotation "+")]), None,
                                 (5,6--5,7)), Ident x, (5,4--5,7)), Ident y,
                           (5,4--5,9)), (4,4--5,9),
                        { InKeyword = Some (4,14--4,16) }), (3,4--5,9),
                     { InKeyword = Some (3,14--3,16) }), (2,4--2,8), NoneAtLet,
                  { LeadingKeyword = Let (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange = Some (2,9--2,10) })], (2,0--5,9))],
          PreXmlDocEmpty, [], None, (2,0--6,0), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [LineComment (3,17--3,55)] }, set []))
