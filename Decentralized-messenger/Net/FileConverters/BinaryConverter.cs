using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Net.FileConverters {
	/// <summary>
	/// represents methods for convert byte array to string for transfer in json packages
	/// </summary>
	public class BinaryConverter : IFileConverter {

		private static char[] AssignedChars;

		private static Dictionary<char, byte> AssignedBytes;

		static BinaryConverter() {
			AssignedChars = new char[16];
			AssignedChars[0] = '0';
			AssignedChars[1] = '1';
			AssignedChars[2] = '2';
			AssignedChars[3] = '3';
			AssignedChars[4] = '4';
			AssignedChars[5] = '5';
			AssignedChars[6] = '6';
			AssignedChars[7] = '7';
			AssignedChars[8] = '8';
			AssignedChars[9] = '9';
			AssignedChars[10] = 'A';
			AssignedChars[11] = 'B';
			AssignedChars[12] = 'C';
			AssignedChars[13] = 'D';
			AssignedChars[14] = 'E';
			AssignedChars[15] = 'F';
			AssignedBytes = new Dictionary<char, byte>();
			for (int i = 0; i < AssignedChars.Length; ++i) {
				AssignedBytes.Add(AssignedChars[i], (byte)i);
			}
		}

		public BinaryConverter() {

		}

		/// <summary>
		/// returns length of specified string, which show this byte array
		/// </summary>
		/// <param name="b"></param>
		/// <returns></returns>
		public static int GetLength(byte[] b) {
			return b == null ? 0 : b.Length * 2;
		}

		/// <summary>
		/// return length of specified string
		/// </summary>
		/// <param name="freeSpace"></param>
		/// <returns></returns>
		public static int GetByteArrayCapacity(int freeSpace) {
			return freeSpace / 2;
		}

		public string EncodeBytes(byte[] data) {
			string result = "";
			foreach (byte b in data) {
				char symbD = AssignedChars[b / 16];
				char symbE = AssignedChars[b % 16];
				result += symbD + "" + symbE;
			}
			return result;
		}

		public byte[] DecodeString(string converted) {
			if (converted == null || converted.Length % 2 == 1) {
				throw new ArgumentException("converted can't be null and should be even length");
			}
			byte[] result = new byte[converted.Length / 2];
			for (int i = 0; i < converted.Length; i += 2) {
				byte bD = AssignedBytes[converted[i]];
				byte bE = AssignedBytes[converted[i + 1]];
				result[i / 2] = (byte)(bD * 16 + bE);
			}
			return result;
		}

		public int GetEncodedSize(byte b) {
			return 2;
		}

	}
}
