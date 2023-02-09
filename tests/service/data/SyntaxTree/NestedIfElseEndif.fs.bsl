ImplFile
  (ParsedImplFileInput
     ("/root/NestedIfElseEndif.fs", false, QualifiedNameOfFile NestedIfElseEndif,
      [], [],
      [SynModuleOrNamespace
         ([NestedIfElseEndif], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named
                    (SynIdent (v, None), false, None,
                     /root/NestedIfElseEndif.fs (1,4--1,5)), None,
                  Const (Int32 3, /root/NestedIfElseEndif.fs (9,8--9,9)),
                  /root/NestedIfElseEndif.fs (1,4--1,5),
                  Yes /root/NestedIfElseEndif.fs (1,0--9,9),
                  { LeadingKeyword = Let /root/NestedIfElseEndif.fs (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange = Some /root/NestedIfElseEndif.fs (1,6--1,7) })],
              /root/NestedIfElseEndif.fs (1,0--9,9))], PreXmlDocEmpty, [], None,
          /root/NestedIfElseEndif.fs (1,0--10,10), { LeadingKeyword = None })],
      (true, false),
      { ConditionalDirectives =
         [If (Ident "FOO", /root/NestedIfElseEndif.fs (2,4--2,11));
          If (Ident "MEH", /root/NestedIfElseEndif.fs (3,8--3,15));
          Else /root/NestedIfElseEndif.fs (5,8--5,13);
          EndIf /root/NestedIfElseEndif.fs (7,8--7,14);
          Else /root/NestedIfElseEndif.fs (8,4--8,9);
          EndIf /root/NestedIfElseEndif.fs (10,4--10,10)]
        CodeComments = [] }, set []))