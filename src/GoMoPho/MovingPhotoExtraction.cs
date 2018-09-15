using System;
using System.Collections.Generic;
using System.Text;

namespace GoMoPho
{
    public class MovingPhotoExtraction
    {

        public static byte[] ToBytes(string[] hexValues)
        {
            byte[] characters = new byte[hexValues.Length];
            for (int i = 0; i < hexValues.Length; i++)
            {
                string hex = hexValues[i];
                var value = Convert.ToByte(hex, 16);
                characters[i] = value;
            }
            return characters;
        }
    }
}
