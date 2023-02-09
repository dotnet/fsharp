ImplFile
  (ParsedImplFileInput
     ("/root/NestedIfEndifWithComplexExpressions.fs", false,
      QualifiedNameOfFile NestedIfEndifWithComplexExpressions, [], [],
      [SynModuleOrNamespace
         ([NestedIfEndifWithComplexExpressions], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named
                    (SynIdent (v, None), false, None,
                     /root/NestedIfEndifWithComplexExpressions.fs (1,4--1,5)),
                  None,
                  ArbitraryAfterError
                    ("localBinding1",
                     /root/NestedIfEndifWithComplexExpressions.fs (1,7--1,7)),
                  /root/NestedIfEndifWithComplexExpressions.fs (1,4--1,5),
                  Yes /root/NestedIfEndifWithComplexExpressions.fs (1,4--1,7),
                  { LeadingKeyword =
                     Let /root/NestedIfEndifWithComplexExpressions.fs (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange =
                     Some
                       /root/NestedIfEndifWithComplexExpressions.fs (1,6--1,7) })],
              /root/NestedIfEndifWithComplexExpressions.fs (1,0--1,7))],
          PreXmlDocEmpty, [], None,
          /root/NestedIfEndifWithComplexExpressions.fs (1,0--8,10),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives =
         [If
            (Not (Ident "DEBUG"),
             /root/NestedIfEndifWithComplexExpressions.fs (2,4--2,14));
          If
            (And (Ident "FOO", Ident "BAR"),
             /root/NestedIfEndifWithComplexExpressions.fs (3,8--3,22));
          If
            (Or (Ident "MEH", Ident "HMM"),
             /root/NestedIfEndifWithComplexExpressions.fs (4,12--4,26));
          EndIf /root/NestedIfEndifWithComplexExpressions.fs (6,12--6,18);
          EndIf /root/NestedIfEndifWithComplexExpressions.fs (7,8--7,14);
          EndIf /root/NestedIfEndifWithComplexExpressions.fs (8,4--8,10)]
        CodeComments = [] }, set []))