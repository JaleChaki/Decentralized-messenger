using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkUtils {
	/// <summary>
	/// represents methods for generate unique Id
	/// </summary>
	internal static class IdGenerator {

		/// <summary>
		/// length of generated id string
		/// </summary>
		public const int IdLength = 12;

		/// <summary>
		/// set of all generated ids (for avoid generate two equal Id)
		/// </summary>
		private static ICollection<string> Ids = new HashSet<string>();

		/// <summary>
		/// random generator
		/// </summary>
		private static Random Rand = new Random();

		/// <summary>
		/// add id to collection of generated ids
		/// </summary>
		/// <param name="id"></param>
		public static void RegisterId(string id) {
			if (IsIdRegistered(id)) {
				throw new ArgumentException("id " + id + "already registered");
			}
			Ids.Add(id);
		}

		/// <summary>
		/// return true, if id was generated earlier
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static bool IsIdRegistered(string id) {
			return Ids.Contains(id);
		}

		/// <summary>
		/// remove id from set of generated ids
		/// </summary>
		/// <param name="id"></param>
		public static void RemoveId(string id) {
			Ids.Remove(id);
		}

		/// <summary>
		/// generate new id and add remember it
		/// </summary>
		/// <returns> generated id </returns>
		public static string CreateId() {
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < IdLength; ++i) {
				builder.Append(GenerateChar());
			}
			string buildedString = builder.ToString();
			if (IsIdRegistered(buildedString)) {
				return CreateId();
			} else {
				RegisterId(buildedString);
				return buildedString;
			}
		}

		/// <summary>
		/// generate random char from set [A..Z, a..z, 0..9]
		/// </summary>
		/// <returns> generated char </returns>
		private static char GenerateChar() {
			int index = Rand.Next(62);
			if (index < 26) {
				return (char)('a' + index);
			}
			if (index < 52) {
				return (char)('A' + index - 26);
			}
			return (char)('0' + index - 52);
		}

	}
}
