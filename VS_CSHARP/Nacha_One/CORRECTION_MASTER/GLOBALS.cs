using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORRECTION_MASTER
{
    /// <summary>
    /// Global access class - anything that is object agnostic can probably be put in here.
    /// 
    /// </summary>
    public class GLOBALS
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <param name="length"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        public static string PadString(string val, int length = -1, char padding = ' ')
        {
            if (length < 0) { length = val.Length; }
            while(val.Length<length)
            {
                val += padding;
            }
            if(val.Length>length)
            {
                val = val.Substring(0, length); // so this should be the absolute number of values to return.
            }
            return val;
        }

        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<string, Tuple<int, int>> batch_fields = new Dictionary<string, Tuple<int, int>>
        {
            { "record type ",new Tuple<int,int>(0,1) },
            { "service code",new Tuple<int,int>(1,3) },
            { "entadd count",new Tuple<int,int>(4,6) },
            { "entry hash  ",new Tuple<int,int>(10,10) },
            { "debit contr.",new Tuple<int,int>(20,12) },
            { "credit cont.",new Tuple<int,int>(32,12) },
            { "company iden",new Tuple<int,int>(44,10) },
            { "message auth",new Tuple<int,int>(54,19) },
            { "reserved    ",new Tuple<int,int>(73,6) },
            { "origin route",new Tuple<int,int>(79,8) },
            { "batch number",new Tuple<int,int>(87,7) }
        };

        /// <summary>
        /// 
        /// </summary>
        // alright - these specify where we need to write the output within the batch and control records.
        public static Dictionary<string, Tuple<int, int>> file_fields = new Dictionary<string, Tuple<int, int>>
        {
            { "record type ",new Tuple<int,int>(0,1) },
            { "batch count ",new Tuple<int,int>(1,6) },
            { "block count ",new Tuple<int,int>(7,6) },
            { "entadd count",new Tuple<int,int>(13,8) },
            { "entry hash  ",new Tuple<int,int>(21,10) },
            { "debit  cont.",new Tuple<int,int>(31,12) },
            { "credit cont.",new Tuple<int,int>(43,12) },
            { "reserved    ",new Tuple<int,int>(55,39) }

        };


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        // I'll just try to construct the last two records manually in  one of a few ways.
        // this will generate the output that we need.
        public static string pad_line()
        {
            string s = "";
            while (s.Length < 94)
            {
                s += "9";
            }
            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file_base"></param>
        /// <returns></returns>
        // alright - this version works without overwriting the output. will return a padded file.
        public static string[] padded_file(string[] file_base)
        {
            //
            int n = file_base.Length;
            int m = 0; // offset;
            while ((n + m) % 10 != 0)
            {
                m += 1;
            }
            string[] os = new string[n + m];
            int o = 0;
            for (int i = 0; i < n; i++)
            {
                os[i] = file_base[i];
                o = i;
            }
            for (int i = n; i < n + m; i++)
            {
                os[i] = pad_line(); // append a padded file.
            }
            return os; // we'll want to check this out later.
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pad"></param>
        /// <param name="length"></param>
        /// <param name="padc"></param>
        /// <returns></returns>
        public static string pad_(string pad, int length, char padc = ' ') { while (pad.Length < length) { pad += padc; } return pad; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pad"></param>
        /// <param name="length"></param>
        /// <param name="padc"></param>
        /// <returns></returns>
        public static string _pad(string pad, int length, char padc = ' ') { while (pad.Length < length) { pad = padc + pad; } return pad; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pad"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string trun_(string pad, int length) { if (pad.Length > length) { pad = pad.Substring(0, length); } return pad; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pad"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string _trun(string pad, int length) { if (pad.Length > length) { pad = pad.Substring(pad.Length - length, length); } return pad; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ach_file"></param>
        /// <returns></returns>
        public static string[] remove_sub_batches(string[] ach_file)
        {
            return new string[] { }; // just returns an empty string array for now.
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ach_file"></param>
        /// <returns></returns>
        // once again, I'm returning a raw string, hopefully this won't be too much of an issue.
        public static string entry_hash(string[] ach_file)
        {
            int hash_accum = 0;
            for(int i = 0; i< ach_file.Length; i++)
            {
                if (ach_file[i][0] == '6')
                {
                    var field = FORMS.ENTRY_FORM.field_map["Routing Num"];
                    hash_accum += Int32.Parse(ach_file[i].Substring(field.Item1, field.Item2));
#if DEBUG
                    Console.WriteLine(ach_file[i].Substring(field.Item1, field.Item2));
#endif 
                }
            }
            // we'll set a breakpoint here - I'm going to try to captrue batch information when looking at one or two batches.
            return hash_accum.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ach_file"></param>
        /// <returns></returns>
        // this returns the un-padded version of the entry addenda count.
        public static string entry_addenda_count(string[] ach_file)
        {
            int acc = 0;
            // only count the entries and addenda - we'll handle the line count later.
            for(int i = 0; i<ach_file.Length;i++ )
            {
                if(ach_file[i][0]=='6' || ach_file[i][0] == '7')
                {
                    acc++;
                }
            }
            return acc.ToString();
        }

    }
}
