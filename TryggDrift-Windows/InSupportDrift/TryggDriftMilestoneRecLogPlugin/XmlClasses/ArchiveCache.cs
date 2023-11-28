using System.IO;
using System.Xml.Serialization;

namespace InSupport.Drift.Plugins.MilestoneRec.Xml
{

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false, ElementName = "bank")]
    //[System.Xml.Serialization.XmlRoot]
    public partial class ArchivesCache
    {

        private bankTable[] tableField;

        private decimal versionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("table")]
        public bankTable[] tables
        {
            get
            {
                return this.tableField;
            }
            set
            {
                this.tableField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal version
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

        public static ArchivesCache Deserialize(string path)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ArchivesCache));
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                return (ArchivesCache)xmlSerializer.Deserialize(fs);
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class bankTable
    {

        private string nameField;

        private ulong sizeField;

        private string descriptionField;

        private string enable_bufferingField;

        private ushort buffered_millisecs_to_keepField;

        private string archive_sourceField;

        private bankTableEntry[] meta_dataField;

        private bankTableColumn[] columnField;

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
        public ulong size
        {
            get
            {
                return this.sizeField;
            }
            set
            {
                this.sizeField = value;
            }
        }

        /// <remarks/>
        public string description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        public string enable_buffering
        {
            get
            {
                return this.enable_bufferingField;
            }
            set
            {
                this.enable_bufferingField = value;
            }
        }

        /// <remarks/>
        public ushort buffered_millisecs_to_keep
        {
            get
            {
                return this.buffered_millisecs_to_keepField;
            }
            set
            {
                this.buffered_millisecs_to_keepField = value;
            }
        }

        /// <remarks/>
        public string archive_source
        {
            get
            {
                return this.archive_sourceField;
            }
            set
            {
                this.archive_sourceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("entry", IsNullable = false)]
        public bankTableEntry[] meta_data
        {
            get
            {
                return this.meta_dataField;
            }
            set
            {
                this.meta_dataField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        public bankTableColumn[] column
        {
            get
            {
                return this.columnField;
            }
            set
            {
                this.columnField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class bankTableEntry
    {

        private string keyField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string key
        {
            get
            {
                return this.keyField;
            }
            set
            {
                this.keyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class bankTableColumn
    {

        private string nameField;

        private ulong begin_timeField;

        private ulong end_timeField;

        private ulong meta_data_first_recordField;

        private ulong meta_data_last_recordField;

        private ulong begin_time_last_recordField;

        private ulong end_time_first_recordField;

        private ulong sizeField;

        private string typeField;

        private string descriptionField;

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
        public ulong begin_time
        {
            get
            {
                return this.begin_timeField;
            }
            set
            {
                this.begin_timeField = value;
            }
        }

        /// <remarks/>
        public ulong end_time
        {
            get
            {
                return this.end_timeField;
            }
            set
            {
                this.end_timeField = value;
            }
        }

        /// <remarks/>
        public ulong meta_data_first_record
        {
            get
            {
                return this.meta_data_first_recordField;
            }
            set
            {
                this.meta_data_first_recordField = value;
            }
        }

        /// <remarks/>
        public ulong meta_data_last_record
        {
            get
            {
                return this.meta_data_last_recordField;
            }
            set
            {
                this.meta_data_last_recordField = value;
            }
        }

        /// <remarks/>
        public ulong begin_time_last_record
        {
            get
            {
                return this.begin_time_last_recordField;
            }
            set
            {
                this.begin_time_last_recordField = value;
            }
        }

        /// <remarks/>
        public ulong end_time_first_record
        {
            get
            {
                return this.end_time_first_recordField;
            }
            set
            {
                this.end_time_first_recordField = value;
            }
        }

        /// <remarks/>
        public ulong size
        {
            get
            {
                return this.sizeField;
            }
            set
            {
                this.sizeField = value;
            }
        }

        /// <remarks/>
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        public string description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }
    }


}
