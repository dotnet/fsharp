ImplFile
  (ParsedImplFileInput
     ("/root/Type/TupleTypeExtensionRecovery 02.fs", false,
      QualifiedNameOfFile TupleTypeExtensionRecovery 02, [],
      [SynModuleOrNamespace
         ([TupleTypeExtensionRecovery 02], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (1,6--1,11),
                     Some
                       (Tuple
                          (false,
                           [Type (Var (SynTypar (T1, None, false), (1,6--1,9)));
                            Star (1,10--1,11)], (1,6--1,11)))),
                  Simple (None (1,6--1,11), (1,6--1,11)), [], None, (1,6--1,11),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = None
                    WithKeyword = None })], (1,0--1,11))], PreXmlDocEmpty, [],
          None, (1,0--2,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(1,0)-(2,0) parse warning The declarations in this file will be placed in an implicit module 'TupleTypeExtensionRecovery 02' based on the file name 'TupleTypeExtensionRecovery 02.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.
