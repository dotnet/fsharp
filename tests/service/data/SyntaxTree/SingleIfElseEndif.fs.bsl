ImplFile
  (ParsedImplFileInput
     ("/root/SingleIfElseEndif.fs", false, QualifiedNameOfFile SingleIfElseEndif,
      [], [],
      [SynModuleOrNamespace
         ([SingleIfElseEndif], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named
                    (SynIdent (v, None), false, None,
                     /root/SingleIfElseEndif.fs (1,4--1,5)), None,
                  Const (Int32 42, /root/SingleIfElseEndif.fs (5,4--5,6)),
                  /root/SingleIfElseEndif.fs (1,4--1,5),
                  Yes /root/SingleIfElseEndif.fs (1,0--5,6),
                  { LeadingKeyword = Let /root/SingleIfElseEndif.fs (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange = Some /root/SingleIfElseEndif.fs (1,6--1,7) })],
              /root/SingleIfElseEndif.fs (1,0--5,6))], PreXmlDocEmpty, [], None,
          /root/SingleIfElseEndif.fs (1,0--6,10), { LeadingKeyword = None })],
      (true, false),
      { ConditionalDirectives =
         [If (Ident "DEBUG", /root/SingleIfElseEndif.fs (2,4--2,13));
          Else /root/SingleIfElseEndif.fs (4,4--4,9);
          EndIf /root/SingleIfElseEndif.fs (6,4--6,10)]
        CodeComments = [] }, set []))