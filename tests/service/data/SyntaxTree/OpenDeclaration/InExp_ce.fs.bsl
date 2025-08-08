ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/InExp_ce.fs", false, QualifiedNameOfFile InExp_ce,
      [],
      [SynModuleOrNamespace
         ([InExp_ce], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (res, None), false, None, (1,4--1,7)), None,
                  App
                    (NonAtomic, false, Ident async,
                     ComputationExpr
                       (false,
                        Open
                          (ModuleOrNamespace
                             (SynLongIdent ([System], [], [None]), (2,9--2,15)),
                           (2,4--2,15), (2,4--5,12),
                           Sequential
                             (SuppressNeither, true,
                              App
                                (Atomic, false,
                                 LongIdent
                                   (false,
                                    SynLongIdent
                                      ([Console; WriteLine], [(3,11--3,12)],
                                       [None; None]), None, (3,4--3,21)),
                                 Paren
                                   (Const
                                      (String
                                         ("Hello, World!", Regular, (3,22--3,37)),
                                       (3,22--3,37)), (3,21--3,22),
                                    Some (3,37--3,38), (3,21--3,38)),
                                 (3,4--3,38)),
                              LetOrUseBang
                                (Yes (4,4--4,29), false, true,
                                 Named
                                   (SynIdent (x, None), false, None, (4,9--4,10)),
                                 App
                                   (NonAtomic, false,
                                    LongIdent
                                      (false,
                                       SynLongIdent
                                         ([Async; Sleep], [(4,18--4,19)],
                                          [None; None]), None, (4,13--4,24)),
                                    Const (Int32 1000, (4,25--4,29)),
                                    (4,13--4,29)), [],
                                 YieldOrReturn
                                   ((false, true), Ident x, (5,4--5,12),
                                    { YieldOrReturnKeyword = (5,4--5,10) }),
                                 (4,4--5,12),
                                 { LetOrUseKeyword = (4,4--4,8)
                                   InKeyword = None
                                   EqualsRange = Some (4,11--4,12) }),
                              (3,4--5,12), { SeparatorRange = None })),
                        (1,16--6,1)), (1,10--6,1)), (1,4--1,7), NoneAtLet,
                  { LeadingKeyword = Let (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange = Some (1,8--1,9) })], (1,0--6,1))],
          PreXmlDocEmpty, [], None, (1,0--6,1), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      WarnDirectives = []
                      CodeComments = [] }, set []))
