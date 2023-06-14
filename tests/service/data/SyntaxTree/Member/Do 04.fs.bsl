ImplFile
  (ParsedImplFileInput
     ("/root/Member/Do 04.fs", false, QualifiedNameOfFile Module, [], [],
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
                            (None, Do, false, false, [], PreXmlDocEmpty,
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None), Const (Unit, (5,4--5,6)), None,
                             ArbitraryAfterError
                               ("hardwhiteDoBinding1", (5,6--5,6)), (5,4--5,6),
                             NoneAtDo, { LeadingKeyword = Do (5,4--5,6)
                                         InlineKeyword = None
                                         EqualsRange = None })], false, false,
                         (5,4--5,6));
                      LetBindings
                        ([SynBinding
                            (None, Do, false, false, [], PreXmlDocEmpty,
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None), Const (Unit, (7,4--7,9)), None,
                             Const (Unit, (7,7--7,9)), (7,4--7,9), NoneAtDo,
                             { LeadingKeyword = Do (7,4--7,6)
                               InlineKeyword = None
                               EqualsRange = None })], false, false, (7,4--7,9))],
                     (5,4--7,9)), [], None, (3,5--7,9),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--7,9))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,9), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(7,4)-(7,6) parse error Possible incorrect indentation: this token is offside of context started at position (5:5). Try indenting this token further or using standard formatting conventions.
(7,4)-(7,6) parse error Expecting expression
