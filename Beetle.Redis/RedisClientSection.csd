<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="ca4d9157-3566-4ae1-b8f3-0d7b6c854ce4" namespace="Beetle.Redis" xmlSchemaNamespace="urn:Beetle.Redis" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
  <typeDefinitions>
    <externalType name="String" namespace="System" />
    <externalType name="Boolean" namespace="System" />
    <externalType name="Int32" namespace="System" />
    <externalType name="Int64" namespace="System" />
    <externalType name="Single" namespace="System" />
    <externalType name="Double" namespace="System" />
    <externalType name="DateTime" namespace="System" />
    <externalType name="TimeSpan" namespace="System" />
  </typeDefinitions>
  <configurationElements>
    <configurationSection name="RedisClientSection" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="redisClientSection">
      <attributeProperties>
        <attributeProperty name="DB" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="dB" isReadOnly="false" defaultValue="0">
          <type>
            <externalTypeMoniker name="/ca4d9157-3566-4ae1-b8f3-0d7b6c854ce4/Int32" />
          </type>
        </attributeProperty>
        <attributeProperty name="Cached" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="cached" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/ca4d9157-3566-4ae1-b8f3-0d7b6c854ce4/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <elementProperties>
        <elementProperty name="Reads" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="reads" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/ca4d9157-3566-4ae1-b8f3-0d7b6c854ce4/HostItemCollection" />
          </type>
        </elementProperty>
        <elementProperty name="Writes" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="writes" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/ca4d9157-3566-4ae1-b8f3-0d7b6c854ce4/HostItemCollection" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElement name="HostItem">
      <attributeProperties>
        <attributeProperty name="Host" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="host" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/ca4d9157-3566-4ae1-b8f3-0d7b6c854ce4/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Connections" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="connections" isReadOnly="false" defaultValue="10">
          <type>
            <externalTypeMoniker name="/ca4d9157-3566-4ae1-b8f3-0d7b6c854ce4/Int32" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="HostItemCollection" collectionType="AddRemoveClearMapAlternate" xmlItemName="hostItem" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/ca4d9157-3566-4ae1-b8f3-0d7b6c854ce4/HostItem" />
      </itemType>
    </configurationElementCollection>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>