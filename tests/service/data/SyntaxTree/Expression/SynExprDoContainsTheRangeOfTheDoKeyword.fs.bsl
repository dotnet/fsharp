ImplFile
  (ParsedImplFileInput
     ("/root/Expression/SynExprDoContainsTheRangeOfTheDoKeyword.fs", false,
      QualifiedNameOfFile SynExprDoContainsTheRangeOfTheDoKeyword, [], [],
      [SynModuleOrNamespace
         ([SynExprDoContainsTheRangeOfTheDoKeyword], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (a, None), false, None, (2,4--2,5)), None,
                  Sequential
                    (SuppressNeither, true, Do (Ident foobar, (3,4--4,14)),
                     DoBang (Ident foobarBang, (5,4--6,18)), (3,4--6,18)),
                  (2,4--2,5), NoneAtLet, { LeadingKeyword = Let (2,0--2,3)
                                           InlineKeyword = None
                                           EqualsRange = Some (2,6--2,7) })],
              (2,0--6,18))], PreXmlDocEmpty, [], None, (2,0--7,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
