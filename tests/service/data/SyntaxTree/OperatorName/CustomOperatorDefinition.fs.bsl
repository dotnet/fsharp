ImplFile
  (ParsedImplFileInput
     ("/root/OperatorName/CustomOperatorDefinition.fs", false,
      QualifiedNameOfFile CustomOperatorDefinition, [], [],
      [SynModuleOrNamespace
         ([CustomOperatorDefinition], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some a)];
                         [SynArgInfo ([], false, Some b)]],
                        SynArgInfo ([], false, None)), None, None),
                  LongIdent
                    (SynLongIdent
                       ([op_Addition], [],
                        [Some
                           (OriginalNotationWithParen
                              ((2,4--2,5), "+", (2,6--2,7)))]), None, None,
                     Pats
                       [Named (SynIdent (a, None), false, None, (2,8--2,9));
                        Named (SynIdent (b, None), false, None, (2,10--2,11))],
                     None, (2,4--2,11)), None,
                  App
                    (NonAtomic, false,
                     App
                       (NonAtomic, true,
                        LongIdent
                          (false,
                           SynLongIdent
                             ([op_Addition], [], [Some (OriginalNotation "+")]),
                           None, (2,16--2,17)), Ident a, (2,14--2,17)), Ident b,
                     (2,14--2,19)), (2,4--2,11), NoneAtLet,
                  { LeadingKeyword = Let (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange = Some (2,12--2,13) })], (2,0--2,19))],
          PreXmlDocEmpty, [], None, (2,0--3,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
