ImplFile
  (ParsedImplFileInput
     ("/root/ConditionalDirective/SingleIfElseEndif.fs", false,
      QualifiedNameOfFile SingleIfElseEndif, [], [],
      [SynModuleOrNamespace
         ([SingleIfElseEndif], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named
                    (SynIdent (v, None), false, None,
                     /root/ConditionalDirective/SingleIfElseEndif.fs (2,4--2,5)),
                  None,
                  Const
                    (Int32 42,
                     /root/ConditionalDirective/SingleIfElseEndif.fs (6,4--6,6)),
                  /root/ConditionalDirective/SingleIfElseEndif.fs (2,4--2,5),
                  Yes /root/ConditionalDirective/SingleIfElseEndif.fs (2,0--6,6),
                  { LeadingKeyword =
                     Let
                       /root/ConditionalDirective/SingleIfElseEndif.fs (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange =
                     Some
                       /root/ConditionalDirective/SingleIfElseEndif.fs (2,6--2,7) })],
              /root/ConditionalDirective/SingleIfElseEndif.fs (2,0--6,6))],
          PreXmlDocEmpty, [], None,
          /root/ConditionalDirective/SingleIfElseEndif.fs (2,0--8,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives =
         [If
            (Ident "DEBUG",
             /root/ConditionalDirective/SingleIfElseEndif.fs (3,4--3,13));
          Else /root/ConditionalDirective/SingleIfElseEndif.fs (5,4--5,9);
          EndIf /root/ConditionalDirective/SingleIfElseEndif.fs (7,4--7,10)]
        CodeComments = [] }, set []))
