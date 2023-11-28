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
    public partial class ArchiveConfig
    {

        private string descriptionField;

        private ulong max_size_in_mbField;

        private ulong max_minutesField;

        private bankPasswords passwordsField;

        private string password_encryptionField;

        private byte table_passwords_are_fips_compliantField;

        private archive archiveField;

        private decimal versionField;

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
        public ulong max_size_in_mb
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
        public ulong max_minutes
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
        public bankPasswords passwords
        {
            get
            {
                return this.passwordsField;
            }
            set
            {
                this.passwordsField = value;
            }
        }

        /// <remarks/>
        public string password_encryption
        {
            get
            {
                return this.password_encryptionField;
            }
            set
            {
                this.password_encryptionField = value;
            }
        }

        /// <remarks/>
        public byte table_passwords_are_fips_compliant
        {
            get
            {
                return this.table_passwords_are_fips_compliantField;
            }
            set
            {
                this.table_passwords_are_fips_compliantField = value;
            }
        }

        /// <remarks/>
        public archive archive
        {
            get
            {
                return this.archiveField;
            }
            set
            {
                this.archiveField = value;
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

        public static ArchiveConfig Deserialize(string path)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ArchiveConfig));
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                return (ArchiveConfig)xmlSerializer.Deserialize(fs);
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class bankPasswords
    {

        private bankPasswordsPassword passwordField;

        /// <remarks/>
        public bankPasswordsPassword password
        {
            get
            {
                return this.passwordField;
            }
            set
            {
                this.passwordField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class bankPasswordsPassword
    {

        private byte selectorField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte selector
        {
            get
            {
                return this.selectorField;
            }
            set
            {
                this.selectorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string value
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


    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class archive
    {

        private archiveLink linkField;

        private ulong skip_minutesField;

        /// <remarks/>
        public archiveLink link
        {
            get
            {
                return this.linkField;
            }
            set
            {
                this.linkField = value;
            }
        }

        /// <remarks/>
        public ulong skip_minutes
        {
            get
            {
                return this.skip_minutesField;
            }
            set
            {
                this.skip_minutesField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class archiveLink
    {

        private string source_bankField;

        private string destination_bankField;

        private bool only_first_subrecordField;

        /// <remarks/>
        public string source_bank
        {
            get
            {
                return this.source_bankField;
            }
            set
            {
                this.source_bankField = value;
            }
        }

        /// <remarks/>
        public string destination_bank
        {
            get
            {
                return this.destination_bankField;
            }
            set
            {
                this.destination_bankField = value;
            }
        }

        /// <remarks/>
        public bool only_first_subrecord
        {
            get
            {
                return this.only_first_subrecordField;
            }
            set
            {
                this.only_first_subrecordField = value;
            }
        }
    }




}
