using System;
using System.Xml.Serialization;

namespace Summer.Batch.Extra.Copybook
{
    /// <summary>
    /// Xml representation of FieldFormat
    /// </summary>
    public class FieldFormat : CopybookElement
    {

        private const int Unset = -2;

        private string _size;
        private int _byteSize;

        /// <summary>
        /// Type attribute.
        /// </summary>
        [XmlAttribute]
        public string Type { get; set; }

        /// <summary>
        /// Decimal attribute.
        /// </summary>
        [XmlAttribute]
        public int Decimal { get; set; }

        /// <summary>
        /// Signed attribute.
        /// </summary>        
        [XmlAttribute]
        public bool Signed { get; set; }
        
        /// <summary>
        /// ImpliedDecimal attribute.
        /// </summary>
        [XmlAttribute]
        public bool ImpliedDecimal { get; set; }

        /// <summary>
        /// Value attribute.
        /// </summary>
        [XmlAttribute]
        public string Value { get; set; }

        /// <summary>
        /// Picture attribute.
        /// </summary>
        [XmlAttribute]
        public string Picture { get; set; }

        /// <summary>
        /// Size attribute.
        /// </summary>
        [XmlAttribute]
        public string Size
        {
            get
            {
                return _size;
            }
            set
            {
                _byteSize = Unset;
                _size = value;
            }
        }

        /// <summary>
        /// Byte size calculation.
        /// </summary>
        public int ByteSize
        {
            get
            {
                if (_byteSize == Unset)
                {
                    try
                    {
                        _byteSize = int.Parse(Size);
                    }
                    catch (FormatException)
                    {
                        if (Size == "VB")
                            _byteSize = -1;
                        else
                            _byteSize = 0;
                    }
                    if (Type == "3" && ByteSize > 0)
                    {
                        _byteSize++;
                        if ((_byteSize % 2) != 0)
                            _byteSize = (_byteSize / 2) + 1;
                        else
                            _byteSize = _byteSize / 2;
                    }
                }
                return _byteSize;
            }
        }


        /// <summary>
        /// Whether this FieldFormat has dependencies or not.
        /// </summary>
        /// <returns></returns>
        public override bool HasDependencies()
        {
            return DependingOn.Length > 0;
        }

    }
}
