ImplFile
  (ParsedImplFileInput
     ("/root/Binding/ConditionalDirectiveAroundInlineKeyword.fs", false,
      QualifiedNameOfFile ConditionalDirectiveAroundInlineKeyword, [], [],
      [SynModuleOrNamespace
         ([ConditionalDirectiveAroundInlineKeyword], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, true, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some f)];
                         [SynArgInfo ([], false, Some ar)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([map], [], [None]), None, None,
                     Pats
                       [Named (SynIdent (f, None), false, None, (6,8--6,9));
                        Named (SynIdent (ar, None), false, None, (6,10--6,12))],
                     None, (6,4--6,12)), None,
                  App
                    (NonAtomic, false,
                     App
                       (NonAtomic, false,
                        LongIdent
                          (false,
                           SynLongIdent
                             ([Async; map], [(6,20--6,21)], [None; None]), None,
                           (6,15--6,24)),
                        Paren
                          (App
                             (NonAtomic, false,
                              LongIdent
                                (false,
                                 SynLongIdent
                                   ([Result; map], [(6,32--6,33)], [None; None]),
                                 None, (6,26--6,36)), Ident f, (6,26--6,38)),
                           (6,25--6,26), Some (6,38--6,39), (6,25--6,39)),
                        (6,15--6,39)), Ident ar, (6,15--6,42)), (6,4--6,12),
                  NoneAtLet, { LeadingKeyword = Let (2,0--2,3)
                               InlineKeyword = Some (4,4--4,10)
                               EqualsRange = Some (6,13--6,14) })], (2,0--6,42))],
          PreXmlDocEmpty, [], None, (2,0--7,0), { LeadingKeyword = None })],
      (true, false),
      { ConditionalDirectives =
         [If (Not (Ident "FOO"), (3,0--3,8)); EndIf (5,0--5,6)]
        CodeComments = [] }, set []))
