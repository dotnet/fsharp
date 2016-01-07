import jobs.generation.Utilities;

def project = GithubProject

def testProfiles = [
	'net40,portable7,portable47,portable78,portable259,vs',
	'cambridge_suite,smoke_only',
	'qa_suite,smoke_only'
]

// TODO: run both debug/release builds
	
testProfiles.each { profile ->
	[true, false].each { isPullRequest ->
	        
        def newJobName = Utilities.getFullJobName(project, profile, isPullRequest)
        def buildString = """call appveyor-build.cmd ${profile}"""

        def newJob = job(newJobName) {
            label('windows')
            steps {
                batchFile(buildString)
            }
        }
        
        Utilities.simpleInnerLoopJobSetup(newJob, project, isPullRequest, "Jenkins ${profile}")
        Utilities.addXUnitDotNETResults(newJob, 'tests/TestResults/**/*_Xml.xml')
        Utilities.addArchival(newJob, "Release/**")
    }
}
