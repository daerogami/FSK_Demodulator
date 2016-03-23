using System;
using System.IO;
using System.Text;

namespace DSP4DotNET
{
    /// <summary>
    /// Wav file class
    /// </summary>
    public class Wave
    {
        private string _FilePath;
        public string m_FilePath
        {
            get { return _FilePath; }
            set { _FilePath = value; }
        }
        //RIFF WAVE Chunk
        //File mark，"RIFF"
        private byte[] _RIFF = new byte[4];
        public string m_RIFF
        {
            get { return Encoding.ASCII.GetString(_RIFF); }
        }
        //The length of the file from the beginning of the address, the total file size = m_FileSize + 8,4Byte, long integer
        private byte[] _FileSize = new byte[4];
        public Int32 m_FileSize
        {
            get { return BitConverter.ToInt32(_FileSize, 0); }
        }
        //WAV file flags, "WAVE", 4 Byte
        private byte[] _WaveFlag = new byte[4];
        public string m_WaveFlag
        {
            get { return Encoding.ASCII.GetString(_WaveFlag); }
        }

        //Format Chunk
        //Wave format signs, 4 Byte, "fmt"
        private byte[] _WaveFormatFlag = new byte[4];
        public string m_WaveFormatFlag
        {
            get { return Encoding.ASCII.GetString(_WaveFormatFlag); }
        }
        //Filtration byte for byte indicating whether there are additional, no additional information is 16, 18:00 there are 2 bytes of additional information, 4Byte, integer
        private byte[] _FilterBytes = new byte[4];
        public Int32 m_FilterBytes
        {
            get { return BitConverter.ToInt32(_FilterBytes, 0); }
        }
        //Format type, when taken as a PCM, 2Byte, integer
        private byte[] _FormatType = new byte[2];
        public Int16 m_FormatType
        {
            get { return BitConverter.ToInt16(_FormatType, 0); }
        }
        //Number of channels, 1 for mono, two-channel as 2,2Byte, integer
        private byte[] _ChannelCount = new byte[2];
        public Int16 m_ChannelCount
        {
            get { return BitConverter.ToInt16(_ChannelCount, 0); }
        }
        //Sampling frequency, 4Byte, long integer
        private byte[] _SampleFrequence = new byte[4];
        public Int32 m_SampleFrequence
        {
            get { return BitConverter.ToInt32(_SampleFrequence, 0); }
        }
        //The average number of bytes per second, long integer, 4Byte
        private byte[] _AvgBytesPerSecond = new byte[4];
        public Int32 m_AvgBytesPerSecond
        {
            get { return BitConverter.ToInt32(_AvgBytesPerSecond, 0); }
        }
        //The number of bytes per sample required, 2Byte, plastic
        private byte[] _SampleBytes = new byte[2];
        public Int16 m_SampleBytes
        {
            get { return BitConverter.ToInt16(_SampleBytes, 0); }
        }
        //Bit bits per sample required, 2bit, integer
        private byte[] _SampleBits = new byte[2];
        public Int16 m_SampleBits
        {
            get { return BitConverter.ToInt16(_SampleBits, 0); }
        }
        //Additional information, 2 Bytes, determined by m_Filter Bytes
        public byte[] m_OptionBytes = new byte[2];

        //Fact Chunk   ID="fact"
        private byte[] _FactID = new byte[4];
        public string m_FactID
        {
            get { return Encoding.ASCII.GetString(_FactID); }
        }
        private byte[] _FactSize = new byte[4];
        public Int32 m_FactSize
        {
            get { return BitConverter.ToInt32(_FactSize, 0); }
        }
        private byte[] _FactData = new byte[4];

        //Data Chunk   ID="data"
        //Data Chunk header structure
        private byte[] _DataID = new byte[4];
        public string m_DataID
        {
            get { return Encoding.ASCII.GetString(_DataID); }
        }
        //Returns data block length
        private byte[] _DataSize;
        public Int32 m_DataSize
        {
            get { return BitConverter.ToInt32(_DataSize, 0); }
        }

        /// <summary>
        /// Bit rate, since the communication 1K is not equal to 1024 but 1000, because the bit rate of the average number of bytes per second * 8/1000
        /// </summary>
        public string m_BitRate
        {
            get { return m_AvgBytesPerSecond * 8.0d / 1000 + "kbps"; }
        }

        //Byte data of both channels
        public byte[] m_PCMBytesData;

        //Left channel sample data
        public Int16[] m_LeftPCMInt16Data;

        //Right channel sample data
        public Int16[] m_RightPCMInt16Data;

        //Left channel sample data conversion float
        public double[] m_LeftPCMFloatData;

        //Right channel sample data is converted to floating point
        public double[] m_RightPCMFloatData;

        public Wave(string m_FilePath)
        {
            _FilePath = m_FilePath;
            if (m_FilePath.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
            {
                FileStream m_FileStream = new FileStream(_FilePath, FileMode.Open);
                BinaryReader m_BinaryReader = new BinaryReader(m_FileStream);
                _RIFF = m_BinaryReader.ReadBytes(4);
                _FileSize = m_BinaryReader.ReadBytes(4);
                _WaveFlag = m_BinaryReader.ReadBytes(4);
                _WaveFormatFlag = m_BinaryReader.ReadBytes(4);
                _FilterBytes = m_BinaryReader.ReadBytes(4);
                _FormatType = m_BinaryReader.ReadBytes(2);
                _ChannelCount = m_BinaryReader.ReadBytes(2);
                _SampleFrequence = m_BinaryReader.ReadBytes(4);
                _AvgBytesPerSecond = m_BinaryReader.ReadBytes(4);
                _SampleBytes = m_BinaryReader.ReadBytes(2);
                _SampleBits = m_BinaryReader.ReadBytes(2);
                //If there is additional information, the additional information is read
                if (m_FilterBytes == 18)
                {
                    m_OptionBytes = m_BinaryReader.ReadBytes(2);
                }

                //Read the next ID
                byte[] m_Temp = new byte[4];
                m_Temp = m_BinaryReader.ReadBytes(4);
                //If the block is a fact
                if (Encoding.ASCII.GetString(m_Temp) == "fact")
                {
                    _FactID = m_Temp;
                    _FactSize = m_BinaryReader.ReadBytes(4);
                    _FactData = m_BinaryReader.ReadBytes(4);
                    // ID data
                    _DataID = m_BinaryReader.ReadBytes(4);
                    // Data block size, the number of bytes
                    _DataSize = m_BinaryReader.ReadBytes(4);
                }
                else// data Chunk
                {
                    // ID data
                    _DataID = m_Temp;
                    // Data block size, the number of bytes
                    _DataSize = m_BinaryReader.ReadBytes(4);
                }

                // Reads the data of both channels
                m_PCMBytesData = new byte[m_DataSize];
                for (int i = 0; i < m_DataSize; i++)
                {
                    m_PCMBytesData[i] = m_BinaryReader.ReadByte();
                }

                // Split two data channels
                FnExtractLeftChannel();

                // Two channels of data conversion
                FnConvertLeftChannelFloat();

                m_BinaryReader.Close();
                m_FileStream.Close();
            }
        }

        /// <summary>
        /// Extracting the data of each channel
        /// </summary>
        public void FnExtractLeftChannel()
        {
            //int i = 0;
            //Mono
            if (m_ChannelCount == 1)
            {
                switch (m_SampleBits)
                {
                    case 8: FnSingleChannel8Bits(); break;
                    case 16: FnSingleChannel16Bits(); break;
                    default: break;
                }
            }
            //Stereo
            else
            {
                switch (m_SampleBits)
                {
                    case 8: FnDoubleChannel8Bits(); break;
                    case 16: FnDoubleChannel16Bits(); break;
                    default: break;
                }
            }
        }

        /// <summary>
        /// Mono 8bit samples, use a value of left and right channels
        /// </summary>
        public void FnSingleChannel8Bits()
        {
            m_LeftPCMInt16Data = new short[m_PCMBytesData.Length];
            m_RightPCMInt16Data = new short[m_PCMBytesData.Length];
            for (int i = 0; i < m_PCMBytesData.Length; i++)
            {
                m_LeftPCMInt16Data[i] = m_RightPCMInt16Data[i] = m_PCMBytesData[i];
            }
        }

        /// <summary>
        /// 8Bit two-channel sampling, separate left and right channels
        /// </summary>
        public void FnDoubleChannel8Bits()
        {
            m_RightPCMInt16Data = new short[m_PCMBytesData.Length / 2];
            m_LeftPCMInt16Data = new short[m_PCMBytesData.Length / 2];

            for (int i = 0; i < m_LeftPCMInt16Data.Length; i++)
            {
                m_LeftPCMInt16Data[i] = m_PCMBytesData[2 * i];
                m_RightPCMInt16Data[i] = m_PCMBytesData[2 * i + 1];
            }
        }

        /// <summary>
        /// Mono 16bit sampling, the same left and right channels
        /// </summary>
        public void FnSingleChannel16Bits()
        {
            m_RightPCMInt16Data = new short[m_PCMBytesData.Length / 4];
            m_LeftPCMInt16Data = new short[m_PCMBytesData.Length / 4];

            for (int i = 0; i < m_LeftPCMInt16Data.Length; i++)
            {
                m_LeftPCMInt16Data[i] = m_RightPCMInt16Data[i] = BitConverter.ToInt16(m_PCMBytesData, i * 2);
            }
        }

        /// <summary>
        /// Two-channel 16Bit samples, separate left and right channels
        /// </summary>
        public void FnDoubleChannel16Bits()
        {
            m_RightPCMInt16Data = new short[m_PCMBytesData.Length / 4];
            m_LeftPCMInt16Data = new short[m_PCMBytesData.Length / 4];

            for (int i = 0; i < m_LeftPCMInt16Data.Length; i++)
            {
                m_LeftPCMInt16Data[i] = BitConverter.ToInt16(m_PCMBytesData, 4 * i);
                m_RightPCMInt16Data[i] = BitConverter.ToInt16(m_PCMBytesData, 4 * i + 2);
            }
        }

        /// <summary>
        /// The left and right data is converted to floating point, PCM sampling raw data of plus or minus two-way data
        /// How kind of data there are both positive and negative, positive and negative bidirectional PCM quantization
        /// coding, according to the median Bit sampling decisions, the highest bit is 0 for positive, 1 negative, divided
        /// by different denominators depending on the number of bits to convert , 16, said -32768 to 32767
        /// Therefore, a positive number is divided by 32767, 32768 divided by the need to get a negative result
        /// </summary>
        public void FnConvertLeftChannelFloat()
        {
            //8bit: -128 to 127, 16Bit: -32768 to 32767
            double m_PositiveConvertBase = Math.Pow(2, m_SampleBits - 1) - 1;
            double m_DePositiveConverBase = Math.Pow(2, m_SampleBits - 1);

            m_LeftPCMFloatData = new double[m_LeftPCMInt16Data.Length];
            m_RightPCMFloatData = new double[m_RightPCMInt16Data.Length];

            //Perform the conversion
            for (int i = 0; i < m_LeftPCMInt16Data.Length; i++)
            {
                m_LeftPCMFloatData[i] = m_LeftPCMInt16Data[i] > 0 ? (m_LeftPCMInt16Data[i] * 1.0d / m_PositiveConvertBase) : (m_LeftPCMInt16Data[i] * 1.0d / m_DePositiveConverBase);
                m_RightPCMFloatData[i] = m_RightPCMInt16Data[i] > 0 ? (m_RightPCMInt16Data[i] * 1.0d / m_PositiveConvertBase) : (m_RightPCMInt16Data[i] * 1.0d / m_DePositiveConverBase);
            }
        }
    }
}