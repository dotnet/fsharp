SigFile
  (ParsedSigFileInput
     ("/root/Member/Inherit - Missing type 02.fsi", QualifiedNameOfFile Module,
      [], [],
      [SynModuleOrNamespaceSig
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefnSig
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  ObjectModel
                    (Unspecified,
                     [Inherit (FromParseError (4,11--4,11), (4,4--4,11))],
                     (4,4--4,11)), [], (3,5--4,11),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--4,11))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,11), { LeadingKeyword = Module (1,0--1,6) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,12)-(4,14) parse error Unexpected keyword 'as' in member signature
(7,0)-(7,0) parse error Incomplete structured construct at or before this point in signature file
