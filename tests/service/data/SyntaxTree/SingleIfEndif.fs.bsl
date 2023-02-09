ImplFile
  (ParsedImplFileInput
     ("/root/SingleIfEndif.fs", false, QualifiedNameOfFile SingleIfEndif, [], [],
      [SynModuleOrNamespace
         ([SingleIfEndif], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named
                    (SynIdent (v, None), false, None,
                     /root/SingleIfEndif.fs (1,4--1,5)), None,
                  Const (Int32 42, /root/SingleIfEndif.fs (5,4--5,6)),
                  /root/SingleIfEndif.fs (1,4--1,5),
                  Yes /root/SingleIfEndif.fs (1,0--5,6),
                  { LeadingKeyword = Let /root/SingleIfEndif.fs (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange = Some /root/SingleIfEndif.fs (1,6--1,7) })],
              /root/SingleIfEndif.fs (1,0--5,6))], PreXmlDocEmpty, [], None,
          /root/SingleIfEndif.fs (1,0--5,6), { LeadingKeyword = None })],
      (true, false),
      { ConditionalDirectives =
         [If (Ident "DEBUG", /root/SingleIfEndif.fs (2,4--2,13));
          EndIf /root/SingleIfEndif.fs (4,4--4,10)]
        CodeComments = [] }, set []))