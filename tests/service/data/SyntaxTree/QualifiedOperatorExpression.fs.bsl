ImplFile
  (ParsedImplFileInput
     ("/root/QualifiedOperatorExpression.fs", false,
      QualifiedNameOfFile QualifiedOperatorExpression, [], [],
      [SynModuleOrNamespace
         ([QualifiedOperatorExpression], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
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
                                (SynIdent (x, None), false, None,
                                 /root/QualifiedOperatorExpression.fs (1,13--1,14)),
                              LongIdent (SynLongIdent ([byte], [], [None])),
                              /root/QualifiedOperatorExpression.fs (1,13--1,19)),
                           /root/QualifiedOperatorExpression.fs (1,12--1,20));
                        Named
                          (SynIdent (n, None), false, None,
                           /root/QualifiedOperatorExpression.fs (1,21--1,22))],
                     None, /root/QualifiedOperatorExpression.fs (1,4--1,22)),
                  None,
                  App
                    (NonAtomic, false,
                     LongIdent
                       (false,
                        SynLongIdent
                          ([Checked; op_Multiply],
                           [/root/QualifiedOperatorExpression.fs (1,32--1,33)],
                           [None;
                            Some
                              (OriginalNotationWithParen
                                 (/root/QualifiedOperatorExpression.fs (1,33--1,34),
                                  "*",
                                  /root/QualifiedOperatorExpression.fs (1,37--1,38)))]),
                        None, /root/QualifiedOperatorExpression.fs (1,25--1,38)),
                     Ident x, /root/QualifiedOperatorExpression.fs (1,25--1,40)),
                  /root/QualifiedOperatorExpression.fs (1,4--1,22), NoneAtLet,
                  { LeadingKeyword =
                     Let /root/QualifiedOperatorExpression.fs (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange =
                     Some /root/QualifiedOperatorExpression.fs (1,23--1,24) })],
              /root/QualifiedOperatorExpression.fs (1,0--1,40))], PreXmlDocEmpty,
          [], None, /root/QualifiedOperatorExpression.fs (1,0--1,40),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))