// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Lobby
{
    public class AttributeDataValue : ISettable
    {
        private long? m_AsInt64;
        private double? m_AsDouble;
        private bool? m_AsBool;
        private string m_AsUtf8;
        private AttributeType m_ValueType;

        /// <summary>
        /// Stored as an 8 byte integer
        /// </summary>
        public long? AsInt64
        {
            get
            {
                long? value;
                Helper.TryMarshalGet(m_AsInt64, out value, m_ValueType, AttributeType.Int64);
                return value;
            }

            set
            {
                Helper.TryMarshalSet(ref m_AsInt64, value, ref m_ValueType, AttributeType.Int64);
            }
        }

        /// <summary>
        /// Stored as a double precision floating point
        /// </summary>
        public double? AsDouble
        {
            get
            {
                double? value;
                Helper.TryMarshalGet(m_AsDouble, out value, m_ValueType, AttributeType.Double);
                return value;
            }

            set
            {
                Helper.TryMarshalSet(ref m_AsDouble, value, ref m_ValueType, AttributeType.Double);
            }
        }

        /// <summary>
        /// Stored as a boolean
        /// </summary>
        public bool? AsBool
        {
            get
            {
                bool? value;
                Helper.TryMarshalGet(m_AsBool, out value, m_ValueType, AttributeType.Boolean);
                return value;
            }

            set
            {
                Helper.TryMarshalSet(ref m_AsBool, value, ref m_ValueType, AttributeType.Boolean);
            }
        }

        /// <summary>
        /// Stored as a null terminated UTF8 string
        /// </summary>
        public string AsUtf8
        {
            get
            {
                string value;
                Helper.TryMarshalGet(m_AsUtf8, out value, m_ValueType, AttributeType.String);
                return value;
            }

            set
            {
                Helper.TryMarshalSet(ref m_AsUtf8, value, ref m_ValueType, AttributeType.String);
            }
        }

        /// <summary>
        /// Type of value stored in the union
        /// </summary>
        public AttributeType ValueType
        {
            get
            {
                return m_ValueType;
            }

            private set
            {
                m_ValueType = value;
            }
        }

        public static implicit operator AttributeDataValue(long? value)
        {
            return new AttributeDataValue() { AsInt64 = value };
        }

        public static implicit operator AttributeDataValue(double? value)
        {
            return new AttributeDataValue() { AsDouble = value };
        }

        public static implicit operator AttributeDataValue(bool? value)
        {
            return new AttributeDataValue() { AsBool = value };
        }

        public static implicit operator AttributeDataValue(string value)
        {
            return new AttributeDataValue() { AsUtf8 = value };
        }

        public static implicit operator AttributeDataValue(string[] value)
        {
            string newValue = "[";
            foreach (string val in value)
            {
                if (!"[".Equals(newValue))
                {
                    newValue += ",";
                }
				newValue += val;
            }
            newValue += "]";
            return new AttributeDataValue() { AsUtf8 = newValue };
        }

        internal void Set(AttributeDataValueInternal? other)
        {
            if (other != null)
            {
                AsInt64 = other.Value.AsInt64;
                AsDouble = other.Value.AsDouble;
                AsBool = other.Value.AsBool;
                AsUtf8 = other.Value.AsUtf8;
            }
        }

        public void Set(object other)
        {
            Set(other as AttributeDataValueInternal?);
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Pack = 8)]
    internal struct AttributeDataValueInternal : ISettable, System.IDisposable
    {
        [System.Runtime.InteropServices.FieldOffset(0)]
        private long m_AsInt64;
        [System.Runtime.InteropServices.FieldOffset(0)]
        private double m_AsDouble;
        [System.Runtime.InteropServices.FieldOffset(0)]
        private int m_AsBool;
        [System.Runtime.InteropServices.FieldOffset(0)]
        private System.IntPtr m_AsUtf8;
        [System.Runtime.InteropServices.FieldOffset(8)]
        private AttributeType m_ValueType;

        public long? AsInt64
        {
            get
            {
                long? value;
                Helper.TryMarshalGet(m_AsInt64, out value, m_ValueType, AttributeType.Int64);
                return value;
            }

            set
            {
                Helper.TryMarshalSet(ref m_AsInt64, value, ref m_ValueType, AttributeType.Int64, this);
            }
        }

        public double? AsDouble
        {
            get
            {
                double? value;
                Helper.TryMarshalGet(m_AsDouble, out value, m_ValueType, AttributeType.Double);
                return value;
            }

            set
            {
                Helper.TryMarshalSet(ref m_AsDouble, value, ref m_ValueType, AttributeType.Double, this);
            }
        }

        public bool? AsBool
        {
            get
            {
                bool? value;
                Helper.TryMarshalGet(m_AsBool, out value, m_ValueType, AttributeType.Boolean);
                return value;
            }

            set
            {
                Helper.TryMarshalSet(ref m_AsBool, value, ref m_ValueType, AttributeType.Boolean, this);
            }
        }

        public string AsUtf8
        {
            get
            {
                string value;
                Helper.TryMarshalGet(m_AsUtf8, out value, m_ValueType, AttributeType.String);
                return value;
            }

            set
            {
                Helper.TryMarshalSet(ref m_AsUtf8, value, ref m_ValueType, AttributeType.String, this);
            }
        }

        public void Set(AttributeDataValue other)
        {
            if (other != null)
            {
                AsInt64 = other.AsInt64;
                AsDouble = other.AsDouble;
                AsBool = other.AsBool;
                AsUtf8 = other.AsUtf8;
            }
        }

        public void Set(object other)
        {
            Set(other as AttributeDataValue);
        }

        public void Dispose()
        {
            Helper.TryMarshalDispose(ref m_AsUtf8, m_ValueType, AttributeType.String);
        }
    }
}