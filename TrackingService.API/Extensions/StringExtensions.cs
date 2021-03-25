using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrackingService.API.Extensions {
	public static class StringExtensions {

        /// <summary>
        /// Inserts a <paramref name="character"/> after every <paramref name="n"/>-th character in <paramref name="str"/>.
        /// </summary>
        public static string InsertEvery(this string str, int n, char character) {
            // https://stackoverflow.com/a/410058

            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (n <= 0)
                throw new ArgumentException("Part length has to be positive.", nameof(n));

            var result = new List<string>();
            for (var i = 0; i < str.Length; i += n) {
                result.Add(str.Substring(i, Math.Min(n, str.Length - i)));
			}

            return string.Join(character, result);
        }
    }
}
