ImplFile
  (ParsedImplFileInput
     ("/root/OperatorName/QualifiedOperatorExpression.fs", false,
      QualifiedNameOfFile QualifiedOperatorExpression, [], [],
      [SynModuleOrNamespace
         ([QualifiedOperatorExpression], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some x)];
                         [SynArgInfo ([], false, Some n)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([PowByte], [], [None]), None, None,
                     Pats
                       [Paren
                          (Typed
                             (Named
                                (SynIdent (x, None), false, None, (2,13--2,14)),
                              LongIdent (SynLongIdent ([byte], [], [None])),
                              (2,13--2,19)), (2,12--2,20));
                        Named (SynIdent (n, None), false, None, (2,21--2,22))],
                     None, (2,4--2,22)), None,
                  App
                    (NonAtomic, false,
                     LongIdent
                       (false,
                        SynLongIdent
                          ([Checked; op_Multiply], [(2,32--2,33)],
                           [None;
                            Some
                              (OriginalNotationWithParen
                                 ((2,33--2,34), "*", (2,37--2,38)))]), None,
                        (2,25--2,38)), Ident x, (2,25--2,40)), (2,4--2,22),
                  NoneAtLet, { LeadingKeyword = Let (2,0--2,3)
                               InlineKeyword = None
                               EqualsRange = Some (2,23--2,24) })], (2,0--2,40))],
          PreXmlDocEmpty, [], None, (2,0--3,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
