ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Tuple - Recover 03.fs", false, QualifiedNameOfFile Tuple,
      [], [],
      [SynModuleOrNamespace
         ([Tuple], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Paren
                    (Tuple
                       (true,
                        [Named (SynIdent (x, None), false, None, (3,13--3,14));
                         Named (SynIdent (y, None), false, None, (3,15--3,16));
                         Named (SynIdent (z, None), false, None, (3,17--3,18));
                         Wild (3,19--3,19)],
                        [(3,14--3,15); (3,16--3,17); (3,18--3,19)], (3,5--3,20)),
                     (3,4--3,21)), None, Const (Unit, (3,24--3,26)), (3,4--3,21),
                  Yes (3,0--3,26), { LeadingKeyword = Let (3,0--3,3)
                                     InlineKeyword = None
                                     EqualsRange = Some (3,22--3,23) })],
              (3,0--3,26));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Paren
                    (Tuple
                       (true,
                        [Named (SynIdent (a, None), false, None, (4,13--4,14));
                         Named (SynIdent (b, None), false, None, (4,15--4,16));
                         Tuple
                           (false, [Wild (4,17--4,17); Wild (4,18--4,18)],
                            [(4,17--4,18)], (4,17--4,19))],
                        [(4,14--4,15); (4,16--4,17)], (4,5--4,19)), (4,4--4,20)),
                  None, Const (Unit, (4,23--4,25)), (4,4--4,20), Yes (4,0--4,25),
                  { LeadingKeyword = Let (4,0--4,3)
                    InlineKeyword = None
                    EqualsRange = Some (4,21--4,22) })], (4,0--4,25))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,25), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,18)-(3,19) parse error Expecting pattern
(4,17)-(4,18) parse error Expected a pattern after this point
