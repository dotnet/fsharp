ImplFile
  (ParsedImplFileInput
     ("/root/ConditionalDirective/DirectivesInMultilineStringAreNotReportedAsTrivia.fs",
      false,
      QualifiedNameOfFile DirectivesInMultilineStringAreNotReportedAsTrivia, [],
      [],
      [SynModuleOrNamespace
         ([DirectivesInMultilineStringAreNotReportedAsTrivia], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (v, None), false, None, (2,4--2,5)), None,
                  Const
                    (String
                       ("
#if DEBUG
()
#endif
42
", TripleQuote, (2,8--7,3)),
                     (2,8--7,3)), (2,4--2,5), Yes (2,0--7,3),
                  { LeadingKeyword = Let (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange = Some (2,6--2,7) })], (2,0--7,3))],
          PreXmlDocEmpty, [], None, (2,0--8,0), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))
