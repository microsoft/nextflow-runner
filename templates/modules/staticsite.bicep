// This template is no longer used by the project, but is kept for reference
param location string = resourceGroup().location
param swaSiteName string
param repositoryUrl string = 'https://github.com/microsoft/nextflow-runner'
param repositoryBranch string = 'main'
@secure()
param repositoryToken string
param appLocation string = 'NextflowRunnerClient'
param outputLocation string = 'wwwroot'

resource swa 'Microsoft.Web/staticSites@2021-03-01' = {
  name: swaSiteName
  location: location
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
  properties: {
    repositoryUrl: repositoryUrl
    repositoryToken: repositoryToken
    branch: repositoryBranch
    stagingEnvironmentPolicy: 'Enabled'
    allowConfigFileUpdates: true
    provider: 'GitHub'
    buildProperties: {
      appLocation: appLocation
      outputLocation: outputLocation
    }
  }
}
