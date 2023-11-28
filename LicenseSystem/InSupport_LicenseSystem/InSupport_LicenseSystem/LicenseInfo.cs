using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InSupport_LicenseSystem
{
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class LicenseInfo
    {

        private string customerField;

        private string siteField;

        private string productField;

        private string machineGuidField;

        private int maxCurrentUsers;

        private System.DateTime expirationDateField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Customer
        {
            get
            {
                return this.customerField;
            }
            set
            {
                this.customerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Site
        {
            get
            {
                return this.siteField;
            }
            set
            {
                this.siteField = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int MaxCurrentUsers
        {
            get
            {
                return this.maxCurrentUsers;
            }
            set
            {
                this.maxCurrentUsers = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Product
        {
            get
            {
                return this.productField;
            }
            set
            {
                this.productField = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string MachineGUID
        {
            get
            {
                return this.machineGuidField;
            }
            set
            {
                this.machineGuidField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "date")]
        public System.DateTime ExpirationDate
        {
            get
            {
                return this.expirationDateField;
            }
            set
            {
                this.expirationDateField = value;
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
}
