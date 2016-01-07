import jobs.generation.Utilities;

def project = GithubProject

def testProfiles = [
	'UnitTests': 'net40,portable7,portable47,portable78,portable259,vs',
	'CambridgeTests': 'cambridge_suite,smoke_only',
	'QASuite': 'qa_suite,smoke_only'
]

// TODO: run both debug/release builds
	
testProfiles.each { profileName, testSuite ->
	[true, false].each { isPullRequest ->
	        
        def newJobName = Utilities.getFullJobName(project, profileName, isPullRequest)
        def buildString = """call appveyor-build.cmd ${testSuite}"""

        def newJob = job(newJobName) {
            label('windows')
            steps {
                batchFile(buildString)
            }
        }
        
        Utilities.simpleInnerLoopJobSetup(newJob, project, isPullRequest, "Jenkins ${profileName}")
        Utilities.addXUnitDotNETResults(newJob, 'tests/TestResults/**/*_Xml.xml')
        Utilities.addArchival(newJob, "Release/**")
    }
}
