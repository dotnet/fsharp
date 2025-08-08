ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang AndBang 19.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (NonAtomic, false, Ident async,
                 ComputationExpr
                   (false,
                    LetOrUse
                      (true, false, true, false,
                       [SynBinding
                          (None, Normal, false, false, [],
                           PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                           SynValData
                             (None,
                              SynValInfo ([], SynArgInfo ([], false, None)),
                              None),
                           Named (SynIdent (a, None), false, None, (4,12--4,13)),
                           None, Const (Int32 3, (4,16--4,17)), (4,12--4,13),
                           Yes (4,4--4,17),
                           { LeadingKeyword = LetRec ((4,4--4,7), (4,8--4,11))
                             InlineKeyword = None
                             EqualsRange = Some (4,14--4,15) });
                        SynBinding
                          (None, Normal, false, false, [],
                           PreXmlDoc ((5,8), FSharp.Compiler.Xml.XmlDocCollector),
                           SynValData
                             (None,
                              SynValInfo ([], SynArgInfo ([], false, None)),
                              None),
                           Named (SynIdent (b, None), false, None, (5,8--5,9)),
                           None, Const (Int32 5, (5,12--5,13)), (5,8--5,9),
                           Yes (5,4--5,13), { LeadingKeyword = And (5,4--5,7)
                                              InlineKeyword = None
                                              EqualsRange = Some (5,10--5,11) })],
                       LetOrUse
                         (false, false, true, true,
                          [SynBinding
                             (None, Normal, false, false, [], PreXmlDocEmpty,
                              SynValData
                                (None,
                                 SynValInfo ([], SynArgInfo ([], false, None)),
                                 None),
                              Named
                                (SynIdent (a, None), false, None, (6,9--6,10)),
                              None, Const (Int32 3, (6,13--6,14)), (6,4--8,13),
                              Yes (6,4--6,14),
                              { LeadingKeyword = Let (6,4--6,8)
                                InlineKeyword = None
                                EqualsRange = Some (6,11--6,12) });
                           SynBinding
                             (None, Normal, false, false, [], PreXmlDocEmpty,
                              SynValData
                                (None,
                                 SynValInfo ([], SynArgInfo ([], false, None)),
                                 None),
                              Named
                                (SynIdent (b, None), false, None, (7,9--7,10)),
                              None, Const (Int32 5, (7,13--7,14)), (7,4--7,14),
                              Yes (7,4--7,14),
                              { LeadingKeyword = And (7,4--7,8)
                                InlineKeyword = None
                                EqualsRange = Some (7,11--7,12) })],
                          YieldOrReturn
                            ((false, true), Const (Unit, (8,11--8,13)),
                             (8,4--8,13), { YieldOrReturnKeyword = (8,4--8,10) }),
                          (6,4--8,13), { LetOrUseKeyword = (6,4--6,8)
                                         InKeyword = None
                                         EqualsRange = Some (6,11--6,12) }),
                       (4,4--8,13), { LetOrUseKeyword = (4,4--4,11)
                                      InKeyword = None
                                      EqualsRange = Some (4,14--4,15) }),
                    (3,6--9,1)), (3,0--9,1)), (3,0--9,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--9,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
