param nameSuffix string = 'd1'
param appNamePrefix string ='acsemailevents'

@description('Name of the Azure Communication Service.')
param acsName string

@description('Location for the Azure Communication Service.')
param location string = 'global'

@description('Data location for the Azure Communication Service.')
param dataLocation string

@description('Name of the Event Grid system topic.')
param systemTopicName string

@description('Name of the SQL server.')
param sqlServerName string = 'sql-${appNamePrefix}-${nameSuffix}'

@description('Administrator username for the SQL server.')
param sqlAdminUsername string = 'a1'

@description('Administrator password for the SQL server.')
@secure()
param sqlAdminPassword string

@description('Name of the SQL database.')
param sqlDatabaseName string = 'AcsEmailEvents'

resource communicationService 'Microsoft.Communication/communicationServices@2023-06-01-preview' = {
  name: acsName
  location: location
  properties: {
    dataLocation: dataLocation
  }
}

resource systemTopic 'Microsoft.EventGrid/systemTopics@2024-12-15-preview' = {
  name: systemTopicName
  location: location
  properties: {
    source: communicationService.id
    topicType: 'Microsoft.Communication.CommunicationServices'
  }
}

resource sqlServer 'Microsoft.Sql/servers@2022-05-01-preview' = {
  name: sqlServerName
  location: 'northeurope'
  properties: {
    administratorLogin: sqlAdminUsername
    administratorLoginPassword: sqlAdminPassword
  }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2022-05-01-preview' = {
  parent: sqlServer
  name: sqlDatabaseName
  location: 'northeurope'
  sku: {
    name: 'Basic'
    tier: 'Basic'
  }
}
