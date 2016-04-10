import jobs.generation.Utilities;

def project = GithubProject
def branch = GithubBranchName

def osList = ['Windows_NT'] //'Ubuntu', 'OSX', 'CentOS7.1'

def machineLabelMap = ['Ubuntu':'ubuntu-doc',
                       'OSX':'mac',
                       'Windows_NT':'windows-elevated',
                       'CentOS7.1' : 'centos-71']

def static getBuildJobName(def configuration, def os) {
    return configuration.toLowerCase() + '_' + os.toLowerCase()
}

[true, false].each { isPullRequest ->
    ['Debug', 'Release'].each { configuration ->
        osList.each { os ->

            def lowerConfiguration = configuration.toLowerCase()

            // Calculate job name
            def jobName = getBuildJobName(configuration, os)

            def buildCommand = '';
            if (os == 'Windows_NT') {
                buildCommand = ".\\jenkins-build.cmd ${lowerConfiguration}"
            }
            else {
                buildCommand = "./jenkins-build.sh ${lowerConfiguration}"
            }

            def newJobName = Utilities.getFullJobName(project, jobName, isPullRequest)
            def newJob = job(newJobName) {
                label(machineLabelMap[os])
                steps {
                    if (os == 'Windows_NT') {
                        // Batch
                        batchFile(buildCommand)
                    }
                    else {
                        // Shell
                        shell(buildCommand)
                    }
                }
            }

            // TODO: set to false after tests are fully enabled
            def skipIfNoTestFiles = true

            Utilities.setMachineAffinity(newJob, os, 'latest-or-auto')
            Utilities.standardJobSetup(newJob, project, isPullRequest, "*/${branch}")
            Utilities.addXUnitDotNETResults(newJob, 'tests/TestResults/**/*_Xml.xml', skipIfNoTestFiles)
            //Utilities.addArchival(newJob, "${lowerConfiguration}/**")

            if (isPullRequest) {
                Utilities.addGithubPRTriggerForBranch(newJob, branch, "${os} ${configuration} Build")
            }
            else {
                Utilities.addGithubPushTrigger(newJob)
            }
        }
    }
}
