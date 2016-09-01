using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AyreSocket.Utilities
{
    public class AyreEnc
    {
        private string key{ get; set; }
        private List<string> keys { get; set; }
        private int count { get; set; }

        public AyreEnc(string key)
        {
            if(string.IsNullOrWhiteSpace(key) || key.Trim().Length < 30) throw new Exception(string.Format(Messager.ERR_ENC_KEY_INVALID, key ?? "NULL"));
            this.key = key.Trim();
            this.keys = new List<string>();
            this.count = 5;

            Initial();
        }

        private void Initial()
        {
            var length = (this.key.Length - (this.key.Length % this.count)) / this.count;
            var i = 1;
            var from = 0;
            var to = length;
            this.keys.Clear();
            do
            {
                this.keys.Add(this.key.Substring(from, to - from));
                from += length;
                to = ++i < this.count ? to + length : this.key.Length;
            } while (i <= this.count);
        }

        public string Encrypt(string str, string key)
        {
            if (str == null) return null;
            if (str.Length <= 0 || key.Length < this.count) return null;
            //if (!UT.isPureNumber(key)) return null;

            //take the last 5 number of key
            var part = key.Substring(key.Length - this.count);
            var data = string.Empty;
            var zero = "00000";
            var array = new List<int>();
            for (var idx = 0; idx < part.Length; idx++) array.Add(Convert.ToInt32(part[idx]) % this.keys[idx].Length);

            for (var idx = 0; idx < str.Length; idx++) {
                var aid = idx % array.Count;
                var src = (int)(str[idx]);
                var weight = (int)(this.keys[aid][array[aid]]);
                var res = (src + weight).ToString();
                if (res.Length < zero.Length) res = zero.Substring(0, zero.Length - res.Length) + res;

                data += res;
                array[aid] = (array[aid] + 1) % this.keys[aid].Length;
            }
            var temp = data;
            data = string.Empty;
            while (temp.Length > 0) {
                string part1 = null;
                string part2 = null;
                if (temp.Length > 4)
                {
                    part1 = temp.Substring(0, 4);
                    part2 = temp.Substring(4);
                }
                else
                {
                    part1 = temp.Substring(0);
                    part2 = string.Empty;
                }

                var chr = (char)Convert.ToInt32(part1);
                data += chr.ToString();
                temp = part2;
            };
            return data;
        }

        public string Decrypt(string str, string key)
        {
            if (str == null) return null;
            if (str.Length <= 0 || key.Length < this.count) return null;
            //if (!UT.isPureNumber(key)) return null;

            //take the last 5 number of key
            var part = key.Substring(key.Length - this.count);
            var data = string.Empty;
            var zero = "00000";
            var array = new List<int>();
            for (var idx = 0; idx < part.Length; idx++) array.Add(Convert.ToInt32(part[idx]) % this.keys[idx].Length);

            for (var idx = 0; idx < str.Length; idx++) {
                var chr = ((int)str[idx]).ToString();
                if (idx == str.Length - 1)
                {
                    var len = (data.Length + chr.Length) % 5;
                    if (len > 0) chr = zero.Substring(0, 5 - len) + chr;
                }
                else
                {
                    chr = zero.Substring(0, 4 - chr.Length) + chr;
                }
                data += chr;
            }
            if (data.Length % 5 != 0) return null;
            var temp = data;
            data = string.Empty;
            var i = 0;
            while (temp.Length > 0) {
                var aid = i % array.Count;
                var weight = (int)this.keys[aid][array[aid]];

                var first = Convert.ToInt32(temp.Substring(0, 5));
                var last = temp.Substring(5);
                var ichr = first - weight;
                var schr = ((char)(ichr)).ToString();
                data += schr;

                array[aid] = (array[aid] + 1) % this.keys[aid].Length;
                temp = last;
                i++;
            };

            return data;
        }
    }
}
