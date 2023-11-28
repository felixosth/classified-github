using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BergendahlsPOS
{
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Transactions
    {

        private TransactionsTransaction[] transactionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Transaction")]
        public TransactionsTransaction[] Transaction
        {
            get
            {
                return this.transactionField;
            }
            set
            {
                this.transactionField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class TransactionsTransaction
    {

        private string tagField;

        private System.DateTime dateField;

        private System.DateTime timeField;

        private string retailStoreField;

        private string retailStoreNameField;

        private string workstationField;

        private string operatorField;

        private string transNumberField;

        private double amountField;

        /// <remarks/>
        public string Tag
        {
            get
            {
                return this.tagField;
            }
            set
            {
                this.tagField = value;
            }
        }

        public DateTime CombinedDateAndTime
        {
            get
            {
                return dateField.Add(timeField.TimeOfDay);
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        [Browsable(false)]
        public System.DateTime Date
        {
            get
            {
                return this.dateField;
            }
            set
            {
                this.dateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "time")]
        [Browsable(false)]
        public System.DateTime Time
        {
            get
            {
                return this.timeField;
            }
            set
            {
                this.timeField = value;
            }
        }

        /// <remarks/>
        [Browsable(false)]
        public string RetailStore
        {
            get
            {
                return this.retailStoreField;
            }
            set
            {
                this.retailStoreField = value;
            }
        }

        /// <remarks/>
        [Browsable(false)]
        public string RetailStoreName
        {
            get
            {
                return this.retailStoreNameField;
            }
            set
            {
                this.retailStoreNameField = value;
            }
        }

        /// <remarks/>
        public string Workstation
        {
            get
            {
                return this.workstationField;
            }
            set
            {
                this.workstationField = value;
            }
        }

        /// <remarks/>
        public string Operator
        {
            get
            {
                return this.operatorField;
            }
            set
            {
                this.operatorField = value;
            }
        }

        /// <remarks/>
        public string TransNumber
        {
            get
            {
                return this.transNumberField;
            }
            set
            {
                this.transNumberField = value;
            }
        }

        /// <remarks/>
        public double Amount
        {
            get
            {
                return this.amountField;
            }
            set
            {
                this.amountField = value;
            }
        }
    }


}
