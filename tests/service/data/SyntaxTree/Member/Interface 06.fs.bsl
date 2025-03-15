ImplFile
  (ParsedImplFileInput
     ("/root/Member/Interface 06.fs", false, QualifiedNameOfFile Module, [],
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
                     [Interface
                        (LongIdent (SynLongIdent ([I], [], [None])),
                         Some (4,16--4,20), Some [], (4,4--4,20));
                      LetBindings
                        ([SynBinding
                            (None, Normal, false, false, [],
                             PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None), Wild (6,8--6,9), None,
                             Const (Unit, (6,12--6,14)), (6,8--6,9),
                             Yes (6,4--6,14),
                             { LeadingKeyword = Let (6,4--6,7)
                               InlineKeyword = None
                               EqualsRange = Some (6,10--6,11) })], false, false,
                         (6,4--6,14))], (4,4--6,14)), [], None, (3,5--6,14),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--6,14))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,14), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(6,4)-(6,14) parse error Unexpected syntax or possible incorrect indentation: this token is offside of context started at position (4:5). Try indenting this further.
To continue using non-conforming indentation, pass the '--strict-indentation-' flag to the compiler, or set the language version to F# 7.
