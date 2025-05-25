ImplFile
  (ParsedImplFileInput
     ("/root/Expression/ExprInterpolatedString 02.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (a, None), false, None, (3,4--3,5)), None,
                  InterpolatedString
                    ([String ("123", (3,8--3,13))], Regular, (3,7--3,13)),
                  (3,4--3,5), Yes (3,0--3,13), { LeadingKeyword = Let (3,0--3,3)
                                                 InlineKeyword = None
                                                 EqualsRange = Some (3,6--3,7) })],
              (3,0--3,13));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (b, None), false, None, (4,4--4,5)), None,
                  InterpolatedString
                    ([String ("123", (4,8--4,14))], Regular, (4,8--4,14)),
                  (4,4--4,5), Yes (4,0--4,14), { LeadingKeyword = Let (4,0--4,3)
                                                 InlineKeyword = None
                                                 EqualsRange = Some (4,6--4,7) })],
              (4,0--4,14));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (x, None), false, None, (5,4--5,5)), None,
                  InterpolatedString
                    ([String ("hello {name}", (5,8--5,22))], Regular,
                     (5,7--5,22)), (5,4--5,5), Yes (5,0--5,22),
                  { LeadingKeyword = Let (5,0--5,3)
                    InlineKeyword = None
                    EqualsRange = Some (5,6--5,7) })], (5,0--5,22));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((6,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (x, None), false, None, (6,4--6,5)), None,
                  InterpolatedString
                    ([String ("hello ", (6,8--6,17));
                      FillExpr (Ident name, None); String ("", (6,21--6,23))],
                     Regular, (6,8--6,23)), (6,4--6,5), Yes (6,0--6,23),
                  { LeadingKeyword = Let (6,0--6,3)
                    InlineKeyword = None
                    EqualsRange = Some (6,6--6,7) })], (6,0--6,23));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((7,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (y, None), false, None, (7,4--7,5)), None,
                  InterpolatedString
                    ([String ("value: {x + 1}", (7,8--7,24))], Regular,
                     (7,7--7,24)), (7,4--7,5), Yes (7,0--7,24),
                  { LeadingKeyword = Let (7,0--7,3)
                    InlineKeyword = None
                    EqualsRange = Some (7,6--7,7) })], (7,0--7,24));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((8,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (y, None), false, None, (8,4--8,5)), None,
                  InterpolatedString
                    ([String ("value: ", (8,8--8,18));
                      FillExpr
                        (App
                           (NonAtomic, false,
                            App
                              (NonAtomic, true,
                               LongIdent
                                 (false,
                                  SynLongIdent
                                    ([op_Addition], [],
                                     [Some (OriginalNotation "+")]), None,
                                  (8,20--8,21)), Ident x, (8,18--8,21)),
                            Const (Int32 1, (8,22--8,23)), (8,18--8,23)), None);
                      String ("", (8,23--8,25))], Regular, (8,8--8,25)),
                  (8,4--8,5), Yes (8,0--8,25), { LeadingKeyword = Let (8,0--8,3)
                                                 InlineKeyword = None
                                                 EqualsRange = Some (8,6--8,7) })],
              (8,0--8,25))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--8,25), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
