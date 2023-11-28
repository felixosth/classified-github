using System.IO;
using System.Xml.Serialization;

namespace InSupport.Drift.Plugins.MilestoneRec.Xml
{

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false, ElementName = "recorderconfig")]
    public partial class RecorderConfig
    {

        public static RecorderConfig Deserialize(string path)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(RecorderConfig));
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                return (RecorderConfig)xmlSerializer.Deserialize(fs);
        }

        private string versionField;

        private recorderconfigRecorder recorderField;

        private recorderconfigServer serverField;

        private recorderconfigWebapi webapiField;

        private recorderconfigPipeline pipelineField;

        private recorderconfigDatabase databaseField;

        private recorderconfigLogmanager logmanagerField;

        private recorderconfigEventlimiter eventlimiterField;

        private recorderconfigWebserver webserverField;

        private recorderconfigTraycontroller traycontrollerField;

        private recorderconfigDriverservices driverservicesField;

        private recorderconfigDriverEventListener driverEventListenerField;

        private recorderconfigFramemediadbconsumer framemediadbconsumerField;

        private recorderconfigRules rulesField;

        private recorderconfigFailoverserver failoverserverField;

        private recorderconfigSmartSearch smartSearchField;

        private recorderconfigHardwareCheck hardwareCheckField;

        private recorderconfigEdgeStorage edgeStorageField;

        private recorderconfigMulticast multicastField;

        private recorderconfigPerformanceCounters performanceCountersField;

        /// <remarks/>
        public string version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        public recorderconfigRecorder recorder
        {
            get
            {
                return this.recorderField;
            }
            set
            {
                this.recorderField = value;
            }
        }

        /// <remarks/>
        public recorderconfigServer server
        {
            get
            {
                return this.serverField;
            }
            set
            {
                this.serverField = value;
            }
        }

        /// <remarks/>
        public recorderconfigWebapi webapi
        {
            get
            {
                return this.webapiField;
            }
            set
            {
                this.webapiField = value;
            }
        }

        /// <remarks/>
        public recorderconfigPipeline pipeline
        {
            get
            {
                return this.pipelineField;
            }
            set
            {
                this.pipelineField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDatabase database
        {
            get
            {
                return this.databaseField;
            }
            set
            {
                this.databaseField = value;
            }
        }

        /// <remarks/>
        public recorderconfigLogmanager logmanager
        {
            get
            {
                return this.logmanagerField;
            }
            set
            {
                this.logmanagerField = value;
            }
        }

        /// <remarks/>
        public recorderconfigEventlimiter eventlimiter
        {
            get
            {
                return this.eventlimiterField;
            }
            set
            {
                this.eventlimiterField = value;
            }
        }

        /// <remarks/>
        public recorderconfigWebserver webserver
        {
            get
            {
                return this.webserverField;
            }
            set
            {
                this.webserverField = value;
            }
        }

        /// <remarks/>
        public recorderconfigTraycontroller traycontroller
        {
            get
            {
                return this.traycontrollerField;
            }
            set
            {
                this.traycontrollerField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDriverservices driverservices
        {
            get
            {
                return this.driverservicesField;
            }
            set
            {
                this.driverservicesField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDriverEventListener DriverEventListener
        {
            get
            {
                return this.driverEventListenerField;
            }
            set
            {
                this.driverEventListenerField = value;
            }
        }

        /// <remarks/>
        public recorderconfigFramemediadbconsumer framemediadbconsumer
        {
            get
            {
                return this.framemediadbconsumerField;
            }
            set
            {
                this.framemediadbconsumerField = value;
            }
        }

        /// <remarks/>
        public recorderconfigRules rules
        {
            get
            {
                return this.rulesField;
            }
            set
            {
                this.rulesField = value;
            }
        }

        /// <remarks/>
        public recorderconfigFailoverserver failoverserver
        {
            get
            {
                return this.failoverserverField;
            }
            set
            {
                this.failoverserverField = value;
            }
        }

        /// <remarks/>
        public recorderconfigSmartSearch SmartSearch
        {
            get
            {
                return this.smartSearchField;
            }
            set
            {
                this.smartSearchField = value;
            }
        }

        /// <remarks/>
        public recorderconfigHardwareCheck HardwareCheck
        {
            get
            {
                return this.hardwareCheckField;
            }
            set
            {
                this.hardwareCheckField = value;
            }
        }

        /// <remarks/>
        public recorderconfigEdgeStorage EdgeStorage
        {
            get
            {
                return this.edgeStorageField;
            }
            set
            {
                this.edgeStorageField = value;
            }
        }

        /// <remarks/>
        public recorderconfigMulticast Multicast
        {
            get
            {
                return this.multicastField;
            }
            set
            {
                this.multicastField = value;
            }
        }

        /// <remarks/>
        public recorderconfigPerformanceCounters PerformanceCounters
        {
            get
            {
                return this.performanceCountersField;
            }
            set
            {
                this.performanceCountersField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigRecorder
    {

        private string idField;

        private string displaynameField;

        private string driverspathField;

        private string mipdriverspathField;

        private string pluginspathField;

        private string ruleactionspathField;

        private ulong storageQuotaLimitField;

        private byte garbagecollectintervalField;

        private ulong memorylogintervalField;

        private ushort unmanagedmemorypressureField;

        private recorderconfigRecorderThreadpool threadpoolField;

        private recorderconfigRecorderHardwarestartup hardwarestartupField;

        private recorderconfigRecorderShutdownProcedure shutdownProcedureField;

        private string clientRegistrationIdField;

        private recorderconfigRecorderServerEncryption serverEncryptionField;

        private string versionField;

        /// <remarks/>
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        public string displayname
        {
            get
            {
                return this.displaynameField;
            }
            set
            {
                this.displaynameField = value;
            }
        }

        /// <remarks/>
        public string driverspath
        {
            get
            {
                return this.driverspathField;
            }
            set
            {
                this.driverspathField = value;
            }
        }

        /// <remarks/>
        public string mipdriverspath
        {
            get
            {
                return this.mipdriverspathField;
            }
            set
            {
                this.mipdriverspathField = value;
            }
        }

        /// <remarks/>
        public string pluginspath
        {
            get
            {
                return this.pluginspathField;
            }
            set
            {
                this.pluginspathField = value;
            }
        }

        /// <remarks/>
        public string ruleactionspath
        {
            get
            {
                return this.ruleactionspathField;
            }
            set
            {
                this.ruleactionspathField = value;
            }
        }

        /// <remarks/>
        public ulong storageQuotaLimit
        {
            get
            {
                return this.storageQuotaLimitField;
            }
            set
            {
                this.storageQuotaLimitField = value;
            }
        }

        /// <remarks/>
        public byte garbagecollectinterval
        {
            get
            {
                return this.garbagecollectintervalField;
            }
            set
            {
                this.garbagecollectintervalField = value;
            }
        }

        /// <remarks/>
        public ulong memoryloginterval
        {
            get
            {
                return this.memorylogintervalField;
            }
            set
            {
                this.memorylogintervalField = value;
            }
        }

        /// <remarks/>
        public ushort unmanagedmemorypressure
        {
            get
            {
                return this.unmanagedmemorypressureField;
            }
            set
            {
                this.unmanagedmemorypressureField = value;
            }
        }

        /// <remarks/>
        public recorderconfigRecorderThreadpool threadpool
        {
            get
            {
                return this.threadpoolField;
            }
            set
            {
                this.threadpoolField = value;
            }
        }

        /// <remarks/>
        public recorderconfigRecorderHardwarestartup hardwarestartup
        {
            get
            {
                return this.hardwarestartupField;
            }
            set
            {
                this.hardwarestartupField = value;
            }
        }

        /// <remarks/>
        public recorderconfigRecorderShutdownProcedure shutdownProcedure
        {
            get
            {
                return this.shutdownProcedureField;
            }
            set
            {
                this.shutdownProcedureField = value;
            }
        }

        /// <remarks/>
        public string ClientRegistrationId
        {
            get
            {
                return this.clientRegistrationIdField;
            }
            set
            {
                this.clientRegistrationIdField = value;
            }
        }

        /// <remarks/>
        public recorderconfigRecorderServerEncryption serverEncryption
        {
            get
            {
                return this.serverEncryptionField;
            }
            set
            {
                this.serverEncryptionField = value;
            }
        }

        /// <remarks/>
        public string version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigRecorderThreadpool
    {

        private byte minWorkerThreadsField;

        private byte minCompletionPortThreadsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte minWorkerThreads
        {
            get
            {
                return this.minWorkerThreadsField;
            }
            set
            {
                this.minWorkerThreadsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte minCompletionPortThreads
        {
            get
            {
                return this.minCompletionPortThreadsField;
            }
            set
            {
                this.minCompletionPortThreadsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigRecorderHardwarestartup
    {

        private byte maxDelayField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte maxDelay
        {
            get
            {
                return this.maxDelayField;
            }
            set
            {
                this.maxDelayField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigRecorderShutdownProcedure
    {

        private byte maxTimeToWaitField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte maxTimeToWait
        {
            get
            {
                return this.maxTimeToWaitField;
            }
            set
            {
                this.maxTimeToWaitField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigRecorderServerEncryption
    {

        private bool enabledField;

        private string certificateHashField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool enabled
        {
            get
            {
                return this.enabledField;
            }
            set
            {
                this.enabledField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string certificateHash
        {
            get
            {
                return this.certificateHashField;
            }
            set
            {
                this.certificateHashField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigServer
    {

        private string addressField;

        private ushort webapiportField;

        private string authorizationserveraddressField;

        private bool logenabledField;

        private ushort logbundleintervalField;

        private byte logbundlemaxcountField;

        private ulong configversioncheckintervalField;

        private ulong sendstatustoserverintervalField;

        /// <remarks/>
        public string address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
            }
        }

        /// <remarks/>
        public ushort webapiport
        {
            get
            {
                return this.webapiportField;
            }
            set
            {
                this.webapiportField = value;
            }
        }

        /// <remarks/>
        public string authorizationserveraddress
        {
            get
            {
                return this.authorizationserveraddressField;
            }
            set
            {
                this.authorizationserveraddressField = value;
            }
        }

        /// <remarks/>
        public bool logenabled
        {
            get
            {
                return this.logenabledField;
            }
            set
            {
                this.logenabledField = value;
            }
        }

        /// <remarks/>
        public ushort logbundleinterval
        {
            get
            {
                return this.logbundleintervalField;
            }
            set
            {
                this.logbundleintervalField = value;
            }
        }

        /// <remarks/>
        public byte logbundlemaxcount
        {
            get
            {
                return this.logbundlemaxcountField;
            }
            set
            {
                this.logbundlemaxcountField = value;
            }
        }

        /// <remarks/>
        public ulong configversioncheckinterval
        {
            get
            {
                return this.configversioncheckintervalField;
            }
            set
            {
                this.configversioncheckintervalField = value;
            }
        }

        /// <remarks/>
        public ulong sendstatustoserverinterval
        {
            get
            {
                return this.sendstatustoserverintervalField;
            }
            set
            {
                this.sendstatustoserverintervalField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigWebapi
    {

        private ushort portField;

        private string publicUriField;

        private byte timeoutfractionField;

        /// <remarks/>
        public ushort port
        {
            get
            {
                return this.portField;
            }
            set
            {
                this.portField = value;
            }
        }

        /// <remarks/>
        public string publicUri
        {
            get
            {
                return this.publicUriField;
            }
            set
            {
                this.publicUriField = value;
            }
        }

        /// <remarks/>
        public byte timeoutfraction
        {
            get
            {
                return this.timeoutfractionField;
            }
            set
            {
                this.timeoutfractionField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigPipeline
    {

        private ushort gopframecountlimitField;

        private ulong gopbytecountlimitField;

        private ulong goptimespanlimitField;

        private ushort audiogoptimespanField;

        private ushort metadatagoptimespanField;

        private byte audioburstdelayField;

        private recorderconfigPipelineLiveThread liveThreadField;

        private recorderconfigPipelineProcessingThread processingThreadField;

        private byte maxframesinqueueField;

        private ulong maxbytesinqueueField;

        private ulong maxactivetimepipeline2Field;

        /// <remarks/>
        public ushort gopframecountlimit
        {
            get
            {
                return this.gopframecountlimitField;
            }
            set
            {
                this.gopframecountlimitField = value;
            }
        }

        /// <remarks/>
        public ulong gopbytecountlimit
        {
            get
            {
                return this.gopbytecountlimitField;
            }
            set
            {
                this.gopbytecountlimitField = value;
            }
        }

        /// <remarks/>
        public ulong goptimespanlimit
        {
            get
            {
                return this.goptimespanlimitField;
            }
            set
            {
                this.goptimespanlimitField = value;
            }
        }

        /// <remarks/>
        public ushort audiogoptimespan
        {
            get
            {
                return this.audiogoptimespanField;
            }
            set
            {
                this.audiogoptimespanField = value;
            }
        }

        /// <remarks/>
        public ushort metadatagoptimespan
        {
            get
            {
                return this.metadatagoptimespanField;
            }
            set
            {
                this.metadatagoptimespanField = value;
            }
        }

        /// <remarks/>
        public byte audioburstdelay
        {
            get
            {
                return this.audioburstdelayField;
            }
            set
            {
                this.audioburstdelayField = value;
            }
        }

        /// <remarks/>
        public recorderconfigPipelineLiveThread liveThread
        {
            get
            {
                return this.liveThreadField;
            }
            set
            {
                this.liveThreadField = value;
            }
        }

        /// <remarks/>
        public recorderconfigPipelineProcessingThread processingThread
        {
            get
            {
                return this.processingThreadField;
            }
            set
            {
                this.processingThreadField = value;
            }
        }

        /// <remarks/>
        public byte maxframesinqueue
        {
            get
            {
                return this.maxframesinqueueField;
            }
            set
            {
                this.maxframesinqueueField = value;
            }
        }

        /// <remarks/>
        public ulong maxbytesinqueue
        {
            get
            {
                return this.maxbytesinqueueField;
            }
            set
            {
                this.maxbytesinqueueField = value;
            }
        }

        /// <remarks/>
        public ulong maxactivetimepipeline2
        {
            get
            {
                return this.maxactivetimepipeline2Field;
            }
            set
            {
                this.maxactivetimepipeline2Field = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigPipelineLiveThread
    {

        private ushort maxStackSizeKBField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort maxStackSizeKB
        {
            get
            {
                return this.maxStackSizeKBField;
            }
            set
            {
                this.maxStackSizeKBField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigPipelineProcessingThread
    {

        private ushort maxStackSizeKBField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort maxStackSizeKB
        {
            get
            {
                return this.maxStackSizeKBField;
            }
            set
            {
                this.maxStackSizeKBField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabase
    {

        private recorderconfigDatabaseDatabase_server database_serverField;

        private recorderconfigDatabaseDatabase_client database_clientField;

        private recorderconfigDatabaseDatabase_default database_defaultField;

        private recorderconfigDatabaseEncryption encryptionField;

        private recorderconfigDatabaseEvent_channel event_channelField;

        private recorderconfigDatabaseFailover failoverField;

        private recorderconfigDatabaseCaching cachingField;

        private recorderconfigDatabaseBankAvailability bankAvailabilityField;

        private bool inprocessField;

        /// <remarks/>
        public recorderconfigDatabaseDatabase_server database_server
        {
            get
            {
                return this.database_serverField;
            }
            set
            {
                this.database_serverField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDatabaseDatabase_client database_client
        {
            get
            {
                return this.database_clientField;
            }
            set
            {
                this.database_clientField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDatabaseDatabase_default database_default
        {
            get
            {
                return this.database_defaultField;
            }
            set
            {
                this.database_defaultField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDatabaseEncryption encryption
        {
            get
            {
                return this.encryptionField;
            }
            set
            {
                this.encryptionField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDatabaseEvent_channel event_channel
        {
            get
            {
                return this.event_channelField;
            }
            set
            {
                this.event_channelField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDatabaseFailover Failover
        {
            get
            {
                return this.failoverField;
            }
            set
            {
                this.failoverField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDatabaseCaching Caching
        {
            get
            {
                return this.cachingField;
            }
            set
            {
                this.cachingField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDatabaseBankAvailability BankAvailability
        {
            get
            {
                return this.bankAvailabilityField;
            }
            set
            {
                this.bankAvailabilityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool inprocess
        {
            get
            {
                return this.inprocessField;
            }
            set
            {
                this.inprocessField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseDatabase_server
    {

        private recorderconfigDatabaseDatabase_serverLogging loggingField;

        private recorderconfigDatabaseDatabase_serverSocket_communication socket_communicationField;

        private recorderconfigDatabaseDatabase_serverServer serverField;

        private recorderconfigDatabaseDatabase_serverTable_split table_splitField;

        private object banksField;

        private recorderconfigDatabaseDatabase_serverThread_pools thread_poolsField;

        private recorderconfigDatabaseDatabase_serverDisk_usage_monitor disk_usage_monitorField;

        private recorderconfigDatabaseDatabase_serverEye_candy eye_candyField;

        private recorderconfigDatabaseDatabase_serverDisk_utilization disk_utilizationField;

        /// <remarks/>
        public recorderconfigDatabaseDatabase_serverLogging logging
        {
            get
            {
                return this.loggingField;
            }
            set
            {
                this.loggingField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDatabaseDatabase_serverSocket_communication socket_communication
        {
            get
            {
                return this.socket_communicationField;
            }
            set
            {
                this.socket_communicationField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDatabaseDatabase_serverServer server
        {
            get
            {
                return this.serverField;
            }
            set
            {
                this.serverField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDatabaseDatabase_serverTable_split table_split
        {
            get
            {
                return this.table_splitField;
            }
            set
            {
                this.table_splitField = value;
            }
        }

        /// <remarks/>
        public object banks
        {
            get
            {
                return this.banksField;
            }
            set
            {
                this.banksField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDatabaseDatabase_serverThread_pools thread_pools
        {
            get
            {
                return this.thread_poolsField;
            }
            set
            {
                this.thread_poolsField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDatabaseDatabase_serverDisk_usage_monitor disk_usage_monitor
        {
            get
            {
                return this.disk_usage_monitorField;
            }
            set
            {
                this.disk_usage_monitorField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDatabaseDatabase_serverEye_candy eye_candy
        {
            get
            {
                return this.eye_candyField;
            }
            set
            {
                this.eye_candyField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDatabaseDatabase_serverDisk_utilization disk_utilization
        {
            get
            {
                return this.disk_utilizationField;
            }
            set
            {
                this.disk_utilizationField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseDatabase_serverLogging
    {

        private object pathField;

        private string prefixField;

        private byte days_to_keepField;

        private ulong max_sizeField;

        /// <remarks/>
        public object path
        {
            get
            {
                return this.pathField;
            }
            set
            {
                this.pathField = value;
            }
        }

        /// <remarks/>
        public string prefix
        {
            get
            {
                return this.prefixField;
            }
            set
            {
                this.prefixField = value;
            }
        }

        /// <remarks/>
        public byte days_to_keep
        {
            get
            {
                return this.days_to_keepField;
            }
            set
            {
                this.days_to_keepField = value;
            }
        }

        /// <remarks/>
        public ulong max_size
        {
            get
            {
                return this.max_sizeField;
            }
            set
            {
                this.max_sizeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseDatabase_serverSocket_communication
    {

        private ushort port_numberField;

        private recorderconfigDatabaseDatabase_serverSocket_communicationAuthentication authenticationField;

        /// <remarks/>
        public ushort port_number
        {
            get
            {
                return this.port_numberField;
            }
            set
            {
                this.port_numberField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDatabaseDatabase_serverSocket_communicationAuthentication authentication
        {
            get
            {
                return this.authenticationField;
            }
            set
            {
                this.authenticationField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseDatabase_serverSocket_communicationAuthentication
    {

        private recorderconfigDatabaseDatabase_serverSocket_communicationAuthenticationAnonymous anonymousField;

        private recorderconfigDatabaseDatabase_serverSocket_communicationAuthenticationBasic basicField;

        /// <remarks/>
        public recorderconfigDatabaseDatabase_serverSocket_communicationAuthenticationAnonymous anonymous
        {
            get
            {
                return this.anonymousField;
            }
            set
            {
                this.anonymousField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDatabaseDatabase_serverSocket_communicationAuthenticationBasic basic
        {
            get
            {
                return this.basicField;
            }
            set
            {
                this.basicField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseDatabase_serverSocket_communicationAuthenticationAnonymous
    {

        private bool enableField;

        /// <remarks/>
        public bool enable
        {
            get
            {
                return this.enableField;
            }
            set
            {
                this.enableField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseDatabase_serverSocket_communicationAuthenticationBasic
    {

        private bool enableField;

        private string passphraseField;

        /// <remarks/>
        public bool enable
        {
            get
            {
                return this.enableField;
            }
            set
            {
                this.enableField = value;
            }
        }

        /// <remarks/>
        public string passphrase
        {
            get
            {
                return this.passphraseField;
            }
            set
            {
                this.passphraseField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseDatabase_serverServer
    {

        private ushort client_shutdown_grace_period_millisecondsField;

        private byte client_shutdown_grace_sleep_millisecondsField;

        /// <remarks/>
        public ushort client_shutdown_grace_period_milliseconds
        {
            get
            {
                return this.client_shutdown_grace_period_millisecondsField;
            }
            set
            {
                this.client_shutdown_grace_period_millisecondsField = value;
            }
        }

        /// <remarks/>
        public byte client_shutdown_grace_sleep_milliseconds
        {
            get
            {
                return this.client_shutdown_grace_sleep_millisecondsField;
            }
            set
            {
                this.client_shutdown_grace_sleep_millisecondsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseDatabase_serverTable_split
    {

        private byte max_minutesField;

        private byte max_size_in_mbField;

        private ushort live_split_table_check_interval_secondsField;

        /// <remarks/>
        public byte max_minutes
        {
            get
            {
                return this.max_minutesField;
            }
            set
            {
                this.max_minutesField = value;
            }
        }

        /// <remarks/>
        public byte max_size_in_mb
        {
            get
            {
                return this.max_size_in_mbField;
            }
            set
            {
                this.max_size_in_mbField = value;
            }
        }

        /// <remarks/>
        public ushort live_split_table_check_interval_seconds
        {
            get
            {
                return this.live_split_table_check_interval_secondsField;
            }
            set
            {
                this.live_split_table_check_interval_secondsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseDatabase_serverThread_pools
    {

        private byte delete_thread_pool_sizeField;

        private byte low_priority_archive_thread_pool_sizeField;

        private byte high_priority_archive_thread_pool_sizeField;

        /// <remarks/>
        public byte delete_thread_pool_size
        {
            get
            {
                return this.delete_thread_pool_sizeField;
            }
            set
            {
                this.delete_thread_pool_sizeField = value;
            }
        }

        /// <remarks/>
        public byte low_priority_archive_thread_pool_size
        {
            get
            {
                return this.low_priority_archive_thread_pool_sizeField;
            }
            set
            {
                this.low_priority_archive_thread_pool_sizeField = value;
            }
        }

        /// <remarks/>
        public byte high_priority_archive_thread_pool_size
        {
            get
            {
                return this.high_priority_archive_thread_pool_sizeField;
            }
            set
            {
                this.high_priority_archive_thread_pool_sizeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseDatabase_serverDisk_usage_monitor
    {

        private byte check_interval_in_secondsField;

        private ushort force_archive_limit_in_mbField;

        private ushort force_delete_limit_in_mbField;

        /// <remarks/>
        public byte check_interval_in_seconds
        {
            get
            {
                return this.check_interval_in_secondsField;
            }
            set
            {
                this.check_interval_in_secondsField = value;
            }
        }

        /// <remarks/>
        public ushort force_archive_limit_in_mb
        {
            get
            {
                return this.force_archive_limit_in_mbField;
            }
            set
            {
                this.force_archive_limit_in_mbField = value;
            }
        }

        /// <remarks/>
        public ushort force_delete_limit_in_mb
        {
            get
            {
                return this.force_delete_limit_in_mbField;
            }
            set
            {
                this.force_delete_limit_in_mbField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseDatabase_serverEye_candy
    {

        private bool use_descriptive_folder_namesField;

        /// <remarks/>
        public bool use_descriptive_folder_names
        {
            get
            {
                return this.use_descriptive_folder_namesField;
            }
            set
            {
                this.use_descriptive_folder_namesField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseDatabase_serverDisk_utilization
    {

        private ulong max_bytes_in_block_filesField;

        private ushort max_records_in_block_filesField;

        private bool truncate_block_filesField;

        private bool precreate_block_filesField;

        private recorderconfigDatabaseDatabase_serverDisk_utilizationPrecreate_sizes precreate_sizesField;

        private recorderconfigDatabaseDatabase_serverDisk_utilizationMedia_block_files media_block_filesField;

        private recorderconfigDatabaseDatabase_serverDisk_utilizationSequence_block_files sequence_block_filesField;

        private recorderconfigDatabaseDatabase_serverDisk_utilizationSignature_block_files signature_block_filesField;

        private recorderconfigDatabaseDatabase_serverDisk_utilizationIndex_files index_filesField;

        private recorderconfigDatabaseDatabase_serverDisk_utilizationChunk_files chunk_filesField;

        /// <remarks/>
        public ulong max_bytes_in_block_files
        {
            get
            {
                return this.max_bytes_in_block_filesField;
            }
            set
            {
                this.max_bytes_in_block_filesField = value;
            }
        }

        /// <remarks/>
        public ushort max_records_in_block_files
        {
            get
            {
                return this.max_records_in_block_filesField;
            }
            set
            {
                this.max_records_in_block_filesField = value;
            }
        }

        /// <remarks/>
        public bool truncate_block_files
        {
            get
            {
                return this.truncate_block_filesField;
            }
            set
            {
                this.truncate_block_filesField = value;
            }
        }

        /// <remarks/>
        public bool precreate_block_files
        {
            get
            {
                return this.precreate_block_filesField;
            }
            set
            {
                this.precreate_block_filesField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDatabaseDatabase_serverDisk_utilizationPrecreate_sizes precreate_sizes
        {
            get
            {
                return this.precreate_sizesField;
            }
            set
            {
                this.precreate_sizesField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDatabaseDatabase_serverDisk_utilizationMedia_block_files media_block_files
        {
            get
            {
                return this.media_block_filesField;
            }
            set
            {
                this.media_block_filesField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDatabaseDatabase_serverDisk_utilizationSequence_block_files sequence_block_files
        {
            get
            {
                return this.sequence_block_filesField;
            }
            set
            {
                this.sequence_block_filesField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDatabaseDatabase_serverDisk_utilizationSignature_block_files signature_block_files
        {
            get
            {
                return this.signature_block_filesField;
            }
            set
            {
                this.signature_block_filesField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDatabaseDatabase_serverDisk_utilizationIndex_files index_files
        {
            get
            {
                return this.index_filesField;
            }
            set
            {
                this.index_filesField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDatabaseDatabase_serverDisk_utilizationChunk_files chunk_files
        {
            get
            {
                return this.chunk_filesField;
            }
            set
            {
                this.chunk_filesField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseDatabase_serverDisk_utilizationPrecreate_sizes
    {

        private ulong regularField;

        private ulong sequenceField;

        private ulong signatureField;

        /// <remarks/>
        public ulong regular
        {
            get
            {
                return this.regularField;
            }
            set
            {
                this.regularField = value;
            }
        }

        /// <remarks/>
        public ulong sequence
        {
            get
            {
                return this.sequenceField;
            }
            set
            {
                this.sequenceField = value;
            }
        }

        /// <remarks/>
        public ulong signature
        {
            get
            {
                return this.signatureField;
            }
            set
            {
                this.signatureField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseDatabase_serverDisk_utilizationMedia_block_files
    {

        private ushort read_buffer_sizeField;

        private ushort write_buffer_sizeField;

        /// <remarks/>
        public ushort read_buffer_size
        {
            get
            {
                return this.read_buffer_sizeField;
            }
            set
            {
                this.read_buffer_sizeField = value;
            }
        }

        /// <remarks/>
        public ushort write_buffer_size
        {
            get
            {
                return this.write_buffer_sizeField;
            }
            set
            {
                this.write_buffer_sizeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseDatabase_serverDisk_utilizationSequence_block_files
    {

        private ushort read_buffer_sizeField;

        private ushort write_buffer_sizeField;

        /// <remarks/>
        public ushort read_buffer_size
        {
            get
            {
                return this.read_buffer_sizeField;
            }
            set
            {
                this.read_buffer_sizeField = value;
            }
        }

        /// <remarks/>
        public ushort write_buffer_size
        {
            get
            {
                return this.write_buffer_sizeField;
            }
            set
            {
                this.write_buffer_sizeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseDatabase_serverDisk_utilizationSignature_block_files
    {

        private ushort read_buffer_sizeField;

        private ushort write_buffer_sizeField;

        /// <remarks/>
        public ushort read_buffer_size
        {
            get
            {
                return this.read_buffer_sizeField;
            }
            set
            {
                this.read_buffer_sizeField = value;
            }
        }

        /// <remarks/>
        public ushort write_buffer_size
        {
            get
            {
                return this.write_buffer_sizeField;
            }
            set
            {
                this.write_buffer_sizeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseDatabase_serverDisk_utilizationIndex_files
    {

        private ushort read_buffer_sizeField;

        private ushort write_buffer_sizeField;

        /// <remarks/>
        public ushort read_buffer_size
        {
            get
            {
                return this.read_buffer_sizeField;
            }
            set
            {
                this.read_buffer_sizeField = value;
            }
        }

        /// <remarks/>
        public ushort write_buffer_size
        {
            get
            {
                return this.write_buffer_sizeField;
            }
            set
            {
                this.write_buffer_sizeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseDatabase_serverDisk_utilizationChunk_files
    {

        private ulong read_buffer_sizeField;

        private ulong write_buffer_sizeField;

        /// <remarks/>
        public ulong read_buffer_size
        {
            get
            {
                return this.read_buffer_sizeField;
            }
            set
            {
                this.read_buffer_sizeField = value;
            }
        }

        /// <remarks/>
        public ulong write_buffer_size
        {
            get
            {
                return this.write_buffer_sizeField;
            }
            set
            {
                this.write_buffer_sizeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseDatabase_client
    {

        private string addressField;

        private ushort portField;

        private recorderconfigDatabaseDatabase_clientAuthentication authenticationField;

        /// <remarks/>
        public string address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
            }
        }

        /// <remarks/>
        public ushort port
        {
            get
            {
                return this.portField;
            }
            set
            {
                this.portField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDatabaseDatabase_clientAuthentication authentication
        {
            get
            {
                return this.authenticationField;
            }
            set
            {
                this.authenticationField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseDatabase_clientAuthentication
    {

        private recorderconfigDatabaseDatabase_clientAuthenticationAnonymous anonymousField;

        private recorderconfigDatabaseDatabase_clientAuthenticationBasic basicField;

        /// <remarks/>
        public recorderconfigDatabaseDatabase_clientAuthenticationAnonymous anonymous
        {
            get
            {
                return this.anonymousField;
            }
            set
            {
                this.anonymousField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDatabaseDatabase_clientAuthenticationBasic basic
        {
            get
            {
                return this.basicField;
            }
            set
            {
                this.basicField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseDatabase_clientAuthenticationAnonymous
    {

        private bool enableField;

        /// <remarks/>
        public bool enable
        {
            get
            {
                return this.enableField;
            }
            set
            {
                this.enableField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseDatabase_clientAuthenticationBasic
    {

        private bool enableField;

        private string passphraseField;

        /// <remarks/>
        public bool enable
        {
            get
            {
                return this.enableField;
            }
            set
            {
                this.enableField = value;
            }
        }

        /// <remarks/>
        public string passphrase
        {
            get
            {
                return this.passphraseField;
            }
            set
            {
                this.passphraseField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseDatabase_default
    {

        private string nameField;

        private string pathField;

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string path
        {
            get
            {
                return this.pathField;
            }
            set
            {
                this.pathField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseEncryption
    {

        private recorderconfigDatabaseEncryptionLight lightField;

        /// <remarks/>
        public recorderconfigDatabaseEncryptionLight light
        {
            get
            {
                return this.lightField;
            }
            set
            {
                this.lightField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseEncryptionLight
    {

        private ushort bytesField;

        /// <remarks/>
        public ushort bytes
        {
            get
            {
                return this.bytesField;
            }
            set
            {
                this.bytesField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseEvent_channel
    {

        private ulong max_queue_sizeField;

        private ulong event_timeout_msField;

        /// <remarks/>
        public ulong max_queue_size
        {
            get
            {
                return this.max_queue_sizeField;
            }
            set
            {
                this.max_queue_sizeField = value;
            }
        }

        /// <remarks/>
        public ulong event_timeout_ms
        {
            get
            {
                return this.event_timeout_msField;
            }
            set
            {
                this.event_timeout_msField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseFailover
    {

        private byte maxRetentionMinutesField;

        private byte maxSizeMbField;

        /// <remarks/>
        public byte MaxRetentionMinutes
        {
            get
            {
                return this.maxRetentionMinutesField;
            }
            set
            {
                this.maxRetentionMinutesField = value;
            }
        }

        /// <remarks/>
        public byte MaxSizeMb
        {
            get
            {
                return this.maxSizeMbField;
            }
            set
            {
                this.maxSizeMbField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseCaching
    {

        private recorderconfigDatabaseCachingDiskUsageInformationBank diskUsageInformationBankField;

        private recorderconfigDatabaseCachingListFileSystemDiskUsage listFileSystemDiskUsageField;

        /// <remarks/>
        public recorderconfigDatabaseCachingDiskUsageInformationBank DiskUsageInformationBank
        {
            get
            {
                return this.diskUsageInformationBankField;
            }
            set
            {
                this.diskUsageInformationBankField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDatabaseCachingListFileSystemDiskUsage ListFileSystemDiskUsage
        {
            get
            {
                return this.listFileSystemDiskUsageField;
            }
            set
            {
                this.listFileSystemDiskUsageField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseCachingDiskUsageInformationBank
    {

        private byte timeoutSecondsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte TimeoutSeconds
        {
            get
            {
                return this.timeoutSecondsField;
            }
            set
            {
                this.timeoutSecondsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseCachingListFileSystemDiskUsage
    {

        private byte timeoutSecondsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte TimeoutSeconds
        {
            get
            {
                return this.timeoutSecondsField;
            }
            set
            {
                this.timeoutSecondsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDatabaseBankAvailability
    {

        private byte checkIntervalSecondsField;

        private bool keepFailingRecordingStorageOfflineField;

        private bool allowStartWithOfflineRecordingStorageField;

        /// <remarks/>
        public byte CheckIntervalSeconds
        {
            get
            {
                return this.checkIntervalSecondsField;
            }
            set
            {
                this.checkIntervalSecondsField = value;
            }
        }

        /// <remarks/>
        public bool KeepFailingRecordingStorageOffline
        {
            get
            {
                return this.keepFailingRecordingStorageOfflineField;
            }
            set
            {
                this.keepFailingRecordingStorageOfflineField = value;
            }
        }

        /// <remarks/>
        public bool AllowStartWithOfflineRecordingStorage
        {
            get
            {
                return this.allowStartWithOfflineRecordingStorageField;
            }
            set
            {
                this.allowStartWithOfflineRecordingStorageField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigLogmanager
    {

        private ushort timerecurrenceField;

        /// <remarks/>
        public ushort timerecurrence
        {
            get
            {
                return this.timerecurrenceField;
            }
            set
            {
                this.timerecurrenceField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigEventlimiter
    {

        private ushort disk_full_eventField;

        /// <remarks/>
        public ushort disk_full_event
        {
            get
            {
                return this.disk_full_eventField;
            }
            set
            {
                this.disk_full_eventField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigWebserver
    {

        private string hostField;

        private ushort portField;

        private recorderconfigWebserverEncryption encryptionField;

        private recorderconfigWebserverRequestThread requestThreadField;

        private bool allowConnectIfPrivacyMaskNotSupportedField;

        private bool useRelaxedTokenCheckField;

        private recorderconfigWebserverImageServerLiveThread imageServerLiveThreadField;

        /// <remarks/>
        public string host
        {
            get
            {
                return this.hostField;
            }
            set
            {
                this.hostField = value;
            }
        }

        /// <remarks/>
        public ushort port
        {
            get
            {
                return this.portField;
            }
            set
            {
                this.portField = value;
            }
        }

        /// <remarks/>
        public recorderconfigWebserverEncryption encryption
        {
            get
            {
                return this.encryptionField;
            }
            set
            {
                this.encryptionField = value;
            }
        }

        /// <remarks/>
        public recorderconfigWebserverRequestThread requestThread
        {
            get
            {
                return this.requestThreadField;
            }
            set
            {
                this.requestThreadField = value;
            }
        }

        /// <remarks/>
        public bool allowConnectIfPrivacyMaskNotSupported
        {
            get
            {
                return this.allowConnectIfPrivacyMaskNotSupportedField;
            }
            set
            {
                this.allowConnectIfPrivacyMaskNotSupportedField = value;
            }
        }

        /// <remarks/>
        public bool useRelaxedTokenCheck
        {
            get
            {
                return this.useRelaxedTokenCheckField;
            }
            set
            {
                this.useRelaxedTokenCheckField = value;
            }
        }

        /// <remarks/>
        public recorderconfigWebserverImageServerLiveThread imageServerLiveThread
        {
            get
            {
                return this.imageServerLiveThreadField;
            }
            set
            {
                this.imageServerLiveThreadField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigWebserverEncryption
    {

        private bool enabledField;

        private string certificateHashField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool enabled
        {
            get
            {
                return this.enabledField;
            }
            set
            {
                this.enabledField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string certificateHash
        {
            get
            {
                return this.certificateHashField;
            }
            set
            {
                this.certificateHashField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigWebserverRequestThread
    {

        private ushort maxStackSizeKBField;

        private ushort maxRequestSizeKbField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort maxStackSizeKB
        {
            get
            {
                return this.maxStackSizeKBField;
            }
            set
            {
                this.maxStackSizeKBField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort maxRequestSizeKb
        {
            get
            {
                return this.maxRequestSizeKbField;
            }
            set
            {
                this.maxRequestSizeKbField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigWebserverImageServerLiveThread
    {

        private ushort maxStackSizeKBField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort maxStackSizeKB
        {
            get
            {
                return this.maxStackSizeKBField;
            }
            set
            {
                this.maxStackSizeKBField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigTraycontroller
    {

        private ushort portField;

        /// <remarks/>
        public ushort port
        {
            get
            {
                return this.portField;
            }
            set
            {
                this.portField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDriverservices
    {

        private recorderconfigDriverservicesHosts hostsField;

        private recorderconfigDriverservicesAlert alertField;

        private recorderconfigDriverservicesSmtp smtpField;

        /// <remarks/>
        public recorderconfigDriverservicesHosts hosts
        {
            get
            {
                return this.hostsField;
            }
            set
            {
                this.hostsField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDriverservicesAlert alert
        {
            get
            {
                return this.alertField;
            }
            set
            {
                this.alertField = value;
            }
        }

        /// <remarks/>
        public recorderconfigDriverservicesSmtp smtp
        {
            get
            {
                return this.smtpField;
            }
            set
            {
                this.smtpField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDriverservicesHosts
    {

        private object hostField;

        /// <remarks/>
        public object host
        {
            get
            {
                return this.hostField;
            }
            set
            {
                this.hostField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDriverservicesAlert
    {

        private bool enabledField;

        private ushort portField;

        private byte maxsessionsField;

        /// <remarks/>
        public bool enabled
        {
            get
            {
                return this.enabledField;
            }
            set
            {
                this.enabledField = value;
            }
        }

        /// <remarks/>
        public ushort port
        {
            get
            {
                return this.portField;
            }
            set
            {
                this.portField = value;
            }
        }

        /// <remarks/>
        public byte maxsessions
        {
            get
            {
                return this.maxsessionsField;
            }
            set
            {
                this.maxsessionsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDriverservicesSmtp
    {

        private bool enabledField;

        private byte portField;

        private bool logcommandsField;

        private byte maxsessionsField;

        private ulong maxmessagesizeField;

        /// <remarks/>
        public bool enabled
        {
            get
            {
                return this.enabledField;
            }
            set
            {
                this.enabledField = value;
            }
        }

        /// <remarks/>
        public byte port
        {
            get
            {
                return this.portField;
            }
            set
            {
                this.portField = value;
            }
        }

        /// <remarks/>
        public bool logcommands
        {
            get
            {
                return this.logcommandsField;
            }
            set
            {
                this.logcommandsField = value;
            }
        }

        /// <remarks/>
        public byte maxsessions
        {
            get
            {
                return this.maxsessionsField;
            }
            set
            {
                this.maxsessionsField = value;
            }
        }

        /// <remarks/>
        public ulong maxmessagesize
        {
            get
            {
                return this.maxmessagesizeField;
            }
            set
            {
                this.maxmessagesizeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigDriverEventListener
    {

        private ushort portField;

        /// <remarks/>
        public ushort Port
        {
            get
            {
                return this.portField;
            }
            set
            {
                this.portField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigFramemediadbconsumer
    {

        private decimal framerateconstantField;

        /// <remarks/>
        public decimal framerateconstant
        {
            get
            {
                return this.framerateconstantField;
            }
            set
            {
                this.framerateconstantField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigRules
    {

        private ushort maxruleactionexecutioncommanddelayField;

        /// <remarks/>
        public ushort maxruleactionexecutioncommanddelay
        {
            get
            {
                return this.maxruleactionexecutioncommanddelayField;
            }
            set
            {
                this.maxruleactionexecutioncommanddelayField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigFailoverserver
    {

        private ushort portField;

        private ushort tokenmaxsizeField;

        private string installedasfailoverserverField;

        /// <remarks/>
        public ushort port
        {
            get
            {
                return this.portField;
            }
            set
            {
                this.portField = value;
            }
        }

        /// <remarks/>
        public ushort tokenmaxsize
        {
            get
            {
                return this.tokenmaxsizeField;
            }
            set
            {
                this.tokenmaxsizeField = value;
            }
        }

        /// <remarks/>
        public string installedasfailoverserver
        {
            get
            {
                return this.installedasfailoverserverField;
            }
            set
            {
                this.installedasfailoverserverField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigSmartSearch
    {

        private bool useSecondaryColumnsField;

        /// <remarks/>
        public bool UseSecondaryColumns
        {
            get
            {
                return this.useSecondaryColumnsField;
            }
            set
            {
                this.useSecondaryColumnsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigHardwareCheck
    {

        private ushort checkFrequenceField;

        /// <remarks/>
        public ushort CheckFrequence
        {
            get
            {
                return this.checkFrequenceField;
            }
            set
            {
                this.checkFrequenceField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigEdgeStorage
    {

        private byte startupRetrieveSecondsField;

        private byte failoverSkipRetrieveSecondsField;

        private ushort maxStackSizeKBField;

        private byte jobMinSecondsField;

        private byte maxSimultaneousJobsField;

        private byte maxJobsAcrossHardwareField;

        private byte periodicUpdateSecondsField;

        private byte writeFinishedJobsXmlMinutesField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte StartupRetrieveSeconds
        {
            get
            {
                return this.startupRetrieveSecondsField;
            }
            set
            {
                this.startupRetrieveSecondsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte FailoverSkipRetrieveSeconds
        {
            get
            {
                return this.failoverSkipRetrieveSecondsField;
            }
            set
            {
                this.failoverSkipRetrieveSecondsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort MaxStackSizeKB
        {
            get
            {
                return this.maxStackSizeKBField;
            }
            set
            {
                this.maxStackSizeKBField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte JobMinSeconds
        {
            get
            {
                return this.jobMinSecondsField;
            }
            set
            {
                this.jobMinSecondsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte MaxSimultaneousJobs
        {
            get
            {
                return this.maxSimultaneousJobsField;
            }
            set
            {
                this.maxSimultaneousJobsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte MaxJobsAcrossHardware
        {
            get
            {
                return this.maxJobsAcrossHardwareField;
            }
            set
            {
                this.maxJobsAcrossHardwareField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte PeriodicUpdateSeconds
        {
            get
            {
                return this.periodicUpdateSecondsField;
            }
            set
            {
                this.periodicUpdateSecondsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte WriteFinishedJobsXmlMinutes
        {
            get
            {
                return this.writeFinishedJobsXmlMinutesField;
            }
            set
            {
                this.writeFinishedJobsXmlMinutesField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigMulticast
    {

        private byte sessionLingerSecondsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte SessionLingerSeconds
        {
            get
            {
                return this.sessionLingerSecondsField;
            }
            set
            {
                this.sessionLingerSecondsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class recorderconfigPerformanceCounters
    {

        private byte deviceDiskUsageIncrementIntervalInSecondsField;

        private byte mediaDBDiskUsageIncrementIntervalInSecondsField;

        /// <remarks/>
        public byte DeviceDiskUsageIncrementIntervalInSeconds
        {
            get
            {
                return this.deviceDiskUsageIncrementIntervalInSecondsField;
            }
            set
            {
                this.deviceDiskUsageIncrementIntervalInSecondsField = value;
            }
        }

        /// <remarks/>
        public byte MediaDBDiskUsageIncrementIntervalInSeconds
        {
            get
            {
                return this.mediaDBDiskUsageIncrementIntervalInSecondsField;
            }
            set
            {
                this.mediaDBDiskUsageIncrementIntervalInSecondsField = value;
            }
        }
    }


}
