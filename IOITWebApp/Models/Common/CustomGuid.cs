using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace IOITWebApp.Models.Common
{
    public enum SequentialGuidType
    {
        SequentialAsString,
        SequentialAsBinary,
        SequentialAtEnd
    }
    public class CustomGuid
    {
        private static readonly RNGCryptoServiceProvider _rng = new RNGCryptoServiceProvider();
        public static Guid NewSequentialId()
        {
            return NewSequentialGuid(SequentialGuidType.SequentialAtEnd);
        }

        //Ý tưởng là Guid có 16 bit, nhưng chỉ dùng 6 bit để sắp xếp --> 1. 10 bit đầu sinh ngẫu nhiên, 2. 6 bit sau sinh theo timestamp
        // ==> trùng thời gian thì vẫn còn 10 bit ngẫu nhiên --> khả năng trùng số là quá thấp
        // ==> đảm bảo là tuần tự (sequence), đã test và phân mảnh index rất ít.
        public static Guid NewSequentialGuid(SequentialGuidType guidType)
        {
            byte[] randomBytes = new byte[10];
            _rng.GetBytes(randomBytes);

            long timestamp = DateTime.UtcNow.Ticks / 10000L;
            byte[] timestampBytes = BitConverter.GetBytes(timestamp);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(timestampBytes);
            }

            byte[] guidBytes = new byte[16];

            switch (guidType)
            {
                case SequentialGuidType.SequentialAsString:
                case SequentialGuidType.SequentialAsBinary:
                    Buffer.BlockCopy(timestampBytes, 2, guidBytes, 0, 6);
                    Buffer.BlockCopy(randomBytes, 0, guidBytes, 6, 10);

                    // If formatting as a string, we have to reverse the order
                    // of the Data1 and Data2 blocks on little-endian systems.
                    if (guidType == SequentialGuidType.SequentialAsString && BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(guidBytes, 0, 4);
                        Array.Reverse(guidBytes, 4, 2);
                    }
                    break;

                case SequentialGuidType.SequentialAtEnd:
                    Buffer.BlockCopy(randomBytes, 0, guidBytes, 0, 10);
                    Buffer.BlockCopy(timestampBytes, 2, guidBytes, 10, 6);
                    break;
            }

            return new Guid(guidBytes);
        }
    }
}
