ImplFile
  (ParsedImplFileInput
     ("/root/Member/Let 02.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  ObjectModel
                    (Unspecified,
                     [LetBindings
                        ([SynBinding
                            (None, Normal, false, false, [],
                             PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None, None), Wild (5,8--5,9), None,
                             ArbitraryAfterError ("localBinding1", (5,11--5,11)),
                             (5,8--5,9), Yes (5,8--5,11),
                             { LeadingKeyword = Let (5,4--5,7)
                               InlineKeyword = None
                               EqualsRange = Some (5,10--5,11) })], false, false,
                         (5,4--5,11));
                      LetBindings
                        ([SynBinding
                            (None, Do, false, false, [], PreXmlDocEmpty,
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None, None), Const (Unit, (7,4--7,8)), None,
                             Const (Int32 2, (7,7--7,8)), (7,4--7,8), NoneAtDo,
                             { LeadingKeyword = Do (7,4--7,6)
                               InlineKeyword = None
                               EqualsRange = None })], false, false, (7,4--7,8))],
                     (5,4--7,8)), [], None, (3,5--7,8),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--7,8))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,8), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(7,4)-(7,6) parse error Unexpected syntax or possible incorrect indentation: this token is offside of context started at position (5:5). Try indenting this further.
To continue using non-conforming indentation, pass the '--strict-indentation-' flag to the compiler, or set the language version to F# 7.
(7,4)-(7,6) parse error Incomplete structured construct at or before this point in binding
