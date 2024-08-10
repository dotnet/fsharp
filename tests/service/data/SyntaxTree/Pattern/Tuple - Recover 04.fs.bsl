ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Tuple - Recover 04.fs", false, QualifiedNameOfFile Tuple,
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
                  Tuple
                    (false,
                     [Wild (3,4--3,4);
                      Paren
                        (Named (SynIdent (a, None), false, None, (3,6--3,7)),
                         (3,5--3,8))], [(3,4--3,5)], (3,4--3,8)), None,
                  Const (Unit, (3,11--3,13)), (3,4--3,8), Yes (3,0--3,13),
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,9--3,10) })], (3,0--3,13));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Tuple
                    (false,
                     [Paren
                        (Named (SynIdent (a, None), false, None, (5,5--5,6)),
                         (5,4--5,7)); Wild (5,8--5,8)], [(5,7--5,8)],
                     (5,4--5,10)), None, Const (Unit, (5,11--5,13)), (5,4--5,10),
                  Yes (5,0--5,13), { LeadingKeyword = Let (5,0--5,3)
                                     InlineKeyword = None
                                     EqualsRange = Some (5,9--5,10) })],
              (5,0--5,13));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((7,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Tuple
                    (false, [Wild (7,4--7,4); Wild (7,5--7,5)], [(7,4--7,5)],
                     (7,4--7,7)), None, Const (Unit, (7,8--7,10)), (7,4--7,7),
                  Yes (7,0--7,10), { LeadingKeyword = Let (7,0--7,3)
                                     InlineKeyword = None
                                     EqualsRange = Some (7,6--7,7) })],
              (7,0--7,10))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,10), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,4)-(3,5) parse error Expecting pattern
(5,9)-(5,10) parse error Unexpected symbol '=' in binding
(5,7)-(5,8) parse error Expecting pattern
(7,6)-(7,7) parse error Unexpected symbol '=' in binding
