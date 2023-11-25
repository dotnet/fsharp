ImplFile
  (ParsedImplFileInput
     ("/root/Type/With 03.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  ObjectModel (Augmentation (3,7--3,11), [], (3,5--3,11)), [],
                  None, (3,5--3,11), { LeadingKeyword = Type (3,0--3,4)
                                       EqualsRange = None
                                       WithKeyword = None })], (3,0--3,11));
           Expr (Const (Int32 2, (5,0--5,1)), (5,0--5,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(5,0)-(5,1) parse error Unexpected syntax or possible incorrect indentation: this token is offside of context started at position (3:1). Try indenting this further.
To continue using non-conforming indentation, pass the '--strict-indentation-' flag to the compiler, or set the language version to F# 7.
