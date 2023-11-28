using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace AxisPeopleCounterPlugin
{
    public class PrettyCountData
    {
        public static PrettyCountData FakeCountData(Random random)
        {
            return new PrettyCountData(random.Next(500), random.Next(500));
        }

        public PrettyCountData(int inVal, int outVal)
        {
            this.In = inVal;
            this.Out = outVal;
        }

        public PrettyCountData(CountData boringCountData)
        {
            this.In = boringCountData.cntset.cntgroup.cnt[0].Value;
            this.Out = boringCountData.cntset.cntgroup.cnt[1].Value;
        }

        //public float Compare(PrettyCountData otherPrettyCountData)
        //{
        //    float myInOut = In - Out;
        //    float otherInOut = otherPrettyCountData.In - otherPrettyCountData.Out;
        //    return PercentageDifference(myInOut, otherInOut);
        //}


        public int In { get; private set; }

        public int Out { get; private set; }

        public override string ToString() => $"In: {In}, Out: {Out}";
    }

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class CountData
    {
        public static CountData Deserialize(string s)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CountData));
            return serializer.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(s))) as CountData;
        }

        public PrettyCountData ToPretty()
        {
            return new PrettyCountData(this);
        }

        private countdataType[] typedescField;

        private countdataCntset cntsetField;

        private byte versionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("type", IsNullable = false)]
        public countdataType[] typedesc
        {
            get
            {
                return this.typedescField;
            }
            set
            {
                this.typedescField = value;
            }
        }

        /// <remarks/>
        public countdataCntset cntset
        {
            get
            {
                return this.cntsetField;
            }
            set
            {
                this.cntsetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte version
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
    public partial class countdataType
    {

        private byte typeidField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte typeid
        {
            get
            {
                return this.typeidField;
            }
            set
            {
                this.typeidField = value;
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
    public partial class countdataCntset
    {

        private countdataCntsetCntgroup cntgroupField;

        private string nameField;

        private uint starttimeField;

        private uint deltaField;

        private System.DateTime dateField;

        /// <remarks/>
        public countdataCntsetCntgroup cntgroup
        {
            get
            {
                return this.cntgroupField;
            }
            set
            {
                this.cntgroupField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint starttime
        {
            get
            {
                return this.starttimeField;
            }
            set
            {
                this.starttimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint delta
        {
            get
            {
                return this.deltaField;
            }
            set
            {
                this.deltaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "date")]
        public System.DateTime date
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
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class countdataCntsetCntgroup
    {

        private countdataCntsetCntgroupCnt[] cntField;

        private uint endtimeField;

        private string timeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("cnt")]
        public countdataCntsetCntgroupCnt[] cnt
        {
            get
            {
                return this.cntField;
            }
            set
            {
                this.cntField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint endtime
        {
            get
            {
                return this.endtimeField;
            }
            set
            {
                this.endtimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string time
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
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class countdataCntsetCntgroupCnt
    {

        private byte typeidField;

        private byte valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte typeid
        {
            get
            {
                return this.typeidField;
            }
            set
            {
                this.typeidField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public byte Value
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
