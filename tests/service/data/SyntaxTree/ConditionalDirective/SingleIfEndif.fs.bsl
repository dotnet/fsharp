ImplFile
  (ParsedImplFileInput
     ("/root/ConditionalDirective/SingleIfEndif.fs", false,
      QualifiedNameOfFile SingleIfEndif, [], [],
      [SynModuleOrNamespace
         ([SingleIfEndif], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (v, None), false, None, (2,4--2,5)), None,
                  Const (Int32 42, (6,4--6,6)), (2,4--2,5), Yes (2,0--6,6),
                  { LeadingKeyword = Let (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange = Some (2,6--2,7) })], (2,0--6,6))],
          PreXmlDocEmpty, [], None, (2,0--7,0), { LeadingKeyword = None })],
      (true, false),
      { ConditionalDirectives =
         [If (Ident "DEBUG", (3,4--3,13)); EndIf (5,4--5,10)]
        CodeComments = [] }, set []))
