ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Named 08.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named
                    (SynIdent
                       (op_MultiplyMultiplyMultiplyMultiply,
                        Some (HasParenthesis ((3,12--3,13), (3,17--3,18)))),
                     false, Some (Private (3,4--3,11)), (3,4--3,18)), None,
                  Const (Int32 0, (3,21--3,22)), (3,4--3,18), Yes (3,0--3,22),
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,19--3,20) })], (3,0--3,22))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,22), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(3,13)-(3,14) parse error Unexpected integer literal in binding
(3,12)-(3,18) parse error Attempted to parse this as an operator name, but failed
