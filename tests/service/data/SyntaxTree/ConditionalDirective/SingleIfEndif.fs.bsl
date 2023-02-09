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
                  Named
                    (SynIdent (v, None), false, None,
                     /root/ConditionalDirective/SingleIfEndif.fs (2,4--2,5)),
                  None,
                  Const
                    (Int32 42,
                     /root/ConditionalDirective/SingleIfEndif.fs (6,4--6,6)),
                  /root/ConditionalDirective/SingleIfEndif.fs (2,4--2,5),
                  Yes /root/ConditionalDirective/SingleIfEndif.fs (2,0--6,6),
                  { LeadingKeyword =
                     Let /root/ConditionalDirective/SingleIfEndif.fs (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange =
                     Some /root/ConditionalDirective/SingleIfEndif.fs (2,6--2,7) })],
              /root/ConditionalDirective/SingleIfEndif.fs (2,0--6,6))],
          PreXmlDocEmpty, [], None,
          /root/ConditionalDirective/SingleIfEndif.fs (2,0--7,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives =
         [If
            (Ident "DEBUG",
             /root/ConditionalDirective/SingleIfEndif.fs (3,4--3,13));
          EndIf /root/ConditionalDirective/SingleIfEndif.fs (5,4--5,10)]
        CodeComments = [] }, set []))