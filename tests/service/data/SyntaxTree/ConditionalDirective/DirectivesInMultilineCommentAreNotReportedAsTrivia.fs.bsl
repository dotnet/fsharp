ImplFile
  (ParsedImplFileInput
     ("/root/ConditionalDirective/DirectivesInMultilineCommentAreNotReportedAsTrivia.fs",
      false,
      QualifiedNameOfFile DirectivesInMultilineCommentAreNotReportedAsTrivia, [],
      [],
      [SynModuleOrNamespace
         ([DirectivesInMultilineCommentAreNotReportedAsTrivia], false,
          AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (v, None), false, None, (2,4--2,5)), None,
                  Const (Int32 42, (8,0--8,2)), (2,4--2,5), Yes (2,0--8,2),
                  { LeadingKeyword = Let (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange = Some (2,6--2,7) })], (2,0--8,2))],
          PreXmlDocEmpty, [], None, (2,0--9,0), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [BlockComment (3,0--7,2)] }, set []))
