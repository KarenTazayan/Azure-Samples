param nameSuffix string = 's1'
param appNamePrefix string ='poc1'
param location string = resourceGroup().location

@secure()
param accessToken string
@secure()
param organizationUrl string

var vnetName = 'vnet-${appNamePrefix}-${nameSuffix}'
var caeName = 'cae-${appNamePrefix}-${nameSuffix}'

var tags = {
  Purpose: 'Proof of Concepts'
}

resource vnet 'Microsoft.Network/virtualNetworks@2022-05-01' = {
  name: vnetName
  location: location
  tags: tags
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.0.0.0/16'
      ]
    }
    subnets: [
      {
        name: 'cae1'
        properties: {
          addressPrefix: '10.0.0.0/21'
        }
      }
    ]
  }
}

resource log 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: 'log-${appNamePrefix}-${nameSuffix}'
  location: location
  tags: tags
  properties: {
    retentionInDays: 30
  }
}

resource cae 'Microsoft.App/managedEnvironments@2023-05-01' = {
  name: caeName
  location: location
  tags: tags
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: log.properties.customerId
        sharedKey: log.listKeys().primarySharedKey
      }
    }
    vnetConfiguration: {
      infrastructureSubnetId: vnet.properties.subnets[0].id
    }
    zoneRedundant: false
  }
}

resource job 'Microsoft.App/jobs@2023-05-01' = {
  name: 'azure-pipelines-agent'
  location: location
  tags: tags
  properties: {
    environmentId: cae.id
    configuration: {
      triggerType: 'Event'
      replicaTimeout: 1800
      replicaRetryLimit: 0
      secrets: [
        {
          name: 'personal-access-token'
          value: accessToken
        }
        {
          name: 'organization-url'
          value: organizationUrl
        }
      ]
      eventTriggerConfig: {
        replicaCompletionCount: 1
        parallelism: 1
        scale: {
          maxExecutions: 10
          minExecutions: 0
          pollingInterval: 5
          rules: [
            {
              auth: [
                {
                  secretRef: 'personal-access-token'
                  triggerParameter: 'personalAccessToken'
                }
                {
                  secretRef: 'organization-url'
                  triggerParameter: 'organizationURL'
                }
              ]
              metadata: {
                poolName: 'Azure Container Apps'
              }
              name: 'azure-pipelines'
              type: 'azure-pipelines'
            }
          ]
        }
      }
    }
    template: {
      containers: [
        {
          image: 'asynchub/azure-pipelines-agents-playwright-1.x:1.43.0.13062024'
          name: 'azure-pipelines-agent'
          resources: {
            cpu: 2
            memory: '4Gi'
          }
          env: [
            {
              name: 'AZP_TOKEN'
              value: 'secretref:personal-access-token'
            }
            {
              name: 'AZP_URL'
              value: 'secretref:organization-url'
            }
            {
              name: 'AZP_POOL'
              value: 'Azure Container Apps'
            }
            {
              name: 'RUN_AGENT_ONCE'
              value: 'True'
            }
          ]
        }
      ]
    }
  }
}
