using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CORRECTION_MASTER.FORMS
{
    /// <summary>
    /// Entry form file viewing object - 
    /// contains the field definition for ACH_ENTRY and ACH_ADDENDA
    /// </summary>
    public partial class ENTRY_FORM : Form
    {
        /// <summary>
        /// store the last representation of the entry form.
        /// </summary>
        public static string last_entry = ""; // get the string representation of the last entry form.

        // stor in mem as a string - this will also afford us the opportunity to update with the most recent variation on the file.
        /// <summary>
        /// store a copy of the available entry records, available for manipulation.
        /// </summary>
        public static Dictionary<Tuple<string, string>, Tuple<string,string>> Entry_Records = new Dictionary<Tuple<string, string>, Tuple<string,string>>();

        /// <summary>
        /// Update a global entry - this doesn't appear to be used at all.
        /// </summary>
        /// <param name="okey"></param>
        /// <param name="nkey"></param>
        /// <param name="earex"></param>
#if UPDATE_20190807
        [Obsolete("Appears to never have been implemented.")]
#endif
        public static void UpdateGlobalEntry(Tuple<string,string> okey, Tuple<string,string> nkey, Tuple<string,string> earex)
        {
            // 
        }

        // use this to prepopulate several instances of this form.
        // we'll want to try and separate these out if possible.
        /// <summary>
        /// Add a global entry to the records-  appears to append a new entry/addenda record to the tail.
        /// </summary>
        /// <param name="entry_record"></param>
        /// <param name="addenda_record"></param>
        public static void AddGlobalEntry(string entry_record, string addenda_record = "")
        {
            var a = ENTRY_FORM.field_map["Amount     "];
            var b = ENTRY_FORM.field_map["Rec. Name  "];
            string amt = entry_record.Substring(a.Item1, a.Item2);
            string nme = entry_record.Substring(b.Item1, b.Item2);
#if DEBUG
            Entry_Records.Add(new Tuple<string, string>(amt, nme), new Tuple<string, string>(entry_record, addenda_record)); //new ENTRY_FORM(entry_record, addenda_record, true)); // let's see if this displays a bunch of entries.
#else
            Entry_Records.Add(new Tuple<string, string>(amt, nme), new Tuple<string, string>(entry_record, addenda_record)); //new ENTRY_FORM(entry_record, addenda_record, false));
#endif
        }

#if UPDATE_201909
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> field_map_mapping()
        {
            return new Dictionary<string, string>{
            { "record type","E. Rec Type"}
            ,{ "transaction code","Trans. Code"}
            ,{ "routing number","Routing Num"}
            ,{ "check digit","Check Digit"}
            ,{ "account number","Account Num"}
            ,{ "amount","Amount     "}
            ,{ "entry id","ID Number  "}
            ,{ "record name","Rec. Name  "}
            ,{ "discretionary data","Disc. Data "}
            ,{ "addenda indicator","Addenda Ind"}
            ,{ "trace number","Trace Num. "}

            ,{ "addenda record type","A. Rec Type"}
            ,{ "addenda type","Adden. Type"}
            ,{ "payment info","Paymnt Info"}
            ,{ "addenda sequence number","Add Seq Num"}
            ,{ "entry sequence number","Ent Seq Num"}
        };
        }

#endif

        /// <summary>
        /// This stores how we're going to split these files up, which should simplify the process.
        /// </summary>
        public static Dictionary<string, Tuple<int, int>> field_map = new Dictionary<string, Tuple<int, int>>
        {

                { "E. Rec Type",new Tuple<int, int>(0,1) },
                { "Trans. Code",new Tuple<int, int>(1,2) },
                { "Routing Num",new Tuple<int, int>(3,8) },
                { "Check Digit",new Tuple<int, int>(11,1) },
                { "Account Num",new Tuple<int, int>(12,17) },
                { "Amount     ",new Tuple<int, int>(29,10) },
                { "ID Number  ",new Tuple<int, int>(39,15) },
                { "Rec. Name  ",new Tuple<int, int>(54,22) },
                { "Disc. Data ",new Tuple<int, int>(76,2) },
                { "Addenda Ind",new Tuple<int, int>(78,1) },
                { "Trace Num. ",new Tuple<int, int>(79,15) },// typo was causing this field to error out. we'll try it again.
                { "A. Rec Type",new Tuple<int, int>(0,1) },
                { "Adden. Type",new Tuple<int, int>(1,2) },
                { "Paymnt Info",new Tuple<int, int>(3,80) },
                { "Add Seq Num",new Tuple<int, int>(83,4) },
                { "Ent Seq Num",new Tuple<int, int>(87,7) }

        };

        private static List<string> add_rex = new List<string>{
            "A. Rec Type",
            "Adden. Type",
            "Paymnt Info",
            "Add Seq Num",
            "Ent Seq Num"

        };

        private static List<string> ent_rex = new List<string> {
            "E. Rec Type" ,
            "Trans. Code" ,
            "Routing Num" ,
            "Check Digit" ,
            "Account Num" ,
            "Amount     " ,  // can't believe I almost left this one out. That's pretty important. I'll need to set up some further way of Identifying this output when we need to get back to it.
            "ID Number  " ,
            "Rec. Name  " ,
            "Disc. Data " ,
            "Addenda Ind" ,
            "Trace Num. "
        };

        // these should be the original values for the keys, we'll try to keep track of them against the master file.
        private string source_entry { get; set; }
        private string source_addenda { get; set; }

        // I'm kind of ignoring the addenda components. maybe we should just treat all entries as tuples, and if Item2 in "" , null we just pass it along.
        /// <summary>
        /// 
        /// </summary>
        public string destination_entry { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string destination_addenda { get; set; }

        // use these as the reference keys to the global file.
        private string source_amount { get; set; }
        private string source_name { get; set; }

        // on save, create a new tuple using these as the keys.
        private string destination_amount { get; set; }
        private string destination_name { get; set; }

        // access this from a higher level if you can get into it.
        /// <summary>
        /// 
        /// </summary>
        public string return_entry { get; set;}
        /// <summary>
        /// 
        /// </summary>
        public int entry_number { get; set; }

        // we'll have a special case for display...
        /// <summary>
        /// determine the padding when interfacing with the USER INTERFACE elements.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="display"></param>
        /// <returns></returns>
        public static string padding_router(string field, string value, bool display = false)
        {
            string ot = "";

            // look for this getting triggered when trying to save the output.
            if(field.Contains("Amount") && value.Contains("."))
            {
                string dl = "";
                string cn = "";
                if(value.Contains("."))
                {
                    var a = value.Split('.');
                    cn = a[1];
                    dl = a[0];
                }
                if (dl.Contains("$"))
                {
                    dl = dl.Replace('$', '0'); // we'll simply replace the dollar signs with a zero- though it should get stripped in most cases.
                }
                value = dl + cn; // I forgot to recombine this in the initial run. now it should be good to go.
                ot = value; // this will be reset now.
#if DEBUG
                Console.WriteLine(ot);
#endif 
            }

            switch (field)
            {
                case "E. Rec Type":
                    goto case "APPEND_PAD_ZERO";
#if UPDATE_20190807 
                    #else
                    break;
                    #endif
                case "Trans. Code":
                    goto  case "APPEND_PAD_ZERO";
#if UPDATE_20190807
#else
                    break;
#endif
                case "Routing Num":
                    goto case "APPEND_PAD_ZERO";
#if UPDATE_20190807
#else
                    break;
#endif
                case "Check Digit":
                    goto case "APPEND_PAD_ZERO";
#if UPDATE_20190807
#else
                    break;
#endif
                case "Account Num":
                    goto case "APPEND_PAD_SPCE";
#if UPDATE_20190807
#else
                    break;
#endif
                case "Amount     ":
                    goto case "PREPEND_PAD_ZERO";
#if UPDATE_20190807
#else
                    break;
#endif
                case "ID Number  ":
                    goto case "PREPEND_PAD_SPCE";
#if UPDATE_20190807
#else
                    break;
#endif
                case "Rec. Name  ":
                    goto case "APPEND_PAD_SPCE";
#if UPDATE_20190807
#else
                    break;
#endif
                case "Disc. Data ":
                    goto case "PREPEND_PAD_SPCE";
#if UPDATE_20190807
#else
                    break;
#endif
                case "Addenda Ind":
                    goto case "PREPEND_PAD_SPCE";
#if UPDATE_20190807
#else
                    break;
#endif
                case "Trace Num. ":
                    goto case "PREPEND_PAD_SPCE";
#if UPDATE_20190807
#else
                    break;
#endif
                case "A. Rec Type":
                    goto case "PREPEND_PAD_ZERO";
#if UPDATE_20190807
#else
                    break;
#endif
                case "Adden. Type":
                    goto case "PREPEND_PAD_ZERO";
#if UPDATE_20190807
#else
                    break;
#endif
                case "Paymnt Info":
                    goto case "PREPEND_PAD_SPCE";
#if UPDATE_20190807
#else
                    break;
#endif
                case "Add Seq Num":
                    goto case "PREPEND_PAD_ZERO";
#if UPDATE_20190807
#else
                    break;
#endif
                case "Ent Seq Num":
                    goto case "PREPEND_PAD_ZERO";
#if UPDATE_20190807
#else
                    break;
#endif

                // Right Justify
                case "PREPEND_PAD_ZERO":
                    ot = GLOBALS._trun(GLOBALS._pad(value, field_map[field].Item2,'0'), field_map[field].Item2);
                    break;
                // Right Justify
                case "PREPEND_PAD_SPCE":
                    ot = GLOBALS._trun(GLOBALS._pad(value, field_map[field].Item2), field_map[field].Item2);
                    break;
                // Left Justify
                case "APPEND_PAD_ZERO":
                    ot = GLOBALS.trun_(GLOBALS.pad_(value, field_map[field].Item2,'0'), field_map[field].Item2);
                    break;
                // Left Justify
                case "APPEND_PAD_SPCE":
                    ot = GLOBALS.trun_(GLOBALS.pad_(value, field_map[field].Item2), field_map[field].Item2);
                    break;
                default:
                    ot = field;
                    break;
            }
            // for display purposes, we'll display the dollar amount in a more readable form.
            if(field == "Amount     " && display == true)
            {
                string cn = ot.Substring(ot.Length - 2, 2);

                string dl = ot.Substring(0, ot.Length - 2);
                int conv = 0;
                Int32.TryParse(dl, out conv); // we'll try setting it this way.
                dl = conv.ToString();
                ot = $"${dl}.{cn}";
            }


            return ot;
        }

        /// <summary>
        /// Maintain a public list of accessible labels.
        /// </summary>
        public Dictionary<string, Tuple<Label, TextBox>> label_reference { get; set; }
        /// <summary>
        ///  maintain a public list of accessible outputs.
        /// </summary>
        public Dictionary<string, string> output_reference { get; set; }
        /// <summary>
        /// maintain a public list of accessible inputs.
        /// </summary>
        public Dictionary<string, TextBox> input_reference { get; set; }

        private void button_click(object sender, EventArgs e)
        {
            if (sender.Equals(button1))
            {
                // alright - we'll try writing consolidate entry when clicked out.
#if DEBUG
                Console.WriteLine($"Button1 CLick: {consolidate_entry()}");
                ENTRY_FORM.last_entry = consolidate_entry(); // that should be an update object.
                this.DestroyHandle(); // close the window after clicking save.
#else
                ENTRY_FORM.last_entry = consolidate_entry(); // this became the main update object.
                this.DestroyHandle();
#endif
            }
            else if (sender.Equals(button2))
            {
#if DEBUG
                Console.WriteLine("Button2 CLick");
                Console.WriteLine("Closing window (or would be if not in testing mode.)");

#else
#endif

            }
            else if (sender.Equals(button3))
            {
#if DEBUG
                Console.WriteLine("Button3 CLick");
                Console.WriteLine("Clearing fields:");
                foreach (string a in label_reference.Keys)
                {
                    Console.WriteLine($"Clearing {a}");
                    label_reference[a].Item2.Text = "";
                }

#else
                foreach (string a in label_reference.Keys)
                {
                    label_reference[a].Item2.Text = "";
                }
#endif

            }
            else
            {
                Console.WriteLine("Unknown");
            }
        }

        // hopefully I can use this to access one of the keys...
        private void text_changed_handler(TextBox sender, EventArgs e)
        {
            string key = "";
            if( this.input_reference.ContainsValue(sender))
            {
                foreach(var a in this.input_reference.Keys)
                {
                    if (input_reference[a] == sender) { key = a; break; }
                }
            }
            this.output_reference[key] = sender.Text; // hopefully this will work.
        }

        private string consolidate_entry()
        {
            string mystr = ""; 
            foreach(string a in ent_rex)
            {
                mystr += padding_router(a, label_reference[a].Item2.Text,false); // this should assemble the file.
            }
#if DEBUG
            Console.WriteLine(mystr);
            System.IO.File.WriteAllText("./test_entry.txt", mystr);
#endif

            return mystr;
        }


        // this is only used when reading a file into the display - so this *SHOULD* be the main point of contact for parsing.
        private void split_entry(string entry)
        {
            //foreach(string a in field_map.Keys)
            foreach(string a in ent_rex) // this should simplify the total number of calls.
            {
                string val = "";
                try
                {
                    var sindex = field_map[a];
                    val = entry.Substring(sindex.Item1, sindex.Item2); // simplify the number of times we call substring.
                    if (a== "Amount     ")
                    {
                        // set val to the pretty-print amount.
                        val = padding_router(a, val, true); // this should handle the amount display - setting it to show a dollar sign and decimal point.
                        
                    }
                    this.label_reference[a].Item2.Text = val; // use val so we have a surface to manipulate when necessary.
                }
                catch(Exception echo)
                {
#if DEBUG
                    Console.WriteLine($"Error parsing field: {a}");
#endif
                    Console.WriteLine(echo.ToString());
                }
            }
            if (new string[] { "0"," " }.Contains( this.label_reference["Addenda Ind"].Item2.Text))
            {
                this.panel2.Enabled = false;
            }
            else
            {
                this.panel2.Enabled = true; // we'll try tinkering with how these get triggered.
            }
        }

        // we'll only offer split_addenda if panel2 is enabled.
        private void split_addenda(string addenda)
        {
            foreach(string a in add_rex)
            {
                {
                    string val = "";
                    try
                    {
                        var sindex = field_map[a];
                        val = addenda.Substring(sindex.Item1, sindex.Item2); // simplify the number of times we call substring.
                        if (a == "Amount     ")
                        {
                            // set val to the pretty-print amount.
                            val = padding_router(a, val, true); // this should handle the amount display - setting it to show a dollar sign and decimal point.

                        }
                        this.label_reference[a].Item2.Text = val; // use val so we have a surface to manipulate when necessary.
                    }
                    catch (Exception echo)
                    {
                        Console.WriteLine(echo.ToString());
                    }
                }
            }
        }

        // leaving these here until I run this in a different manner.
        private void pad_() { }
        private void _pad() { }
        private void trunc_() { }
        private void _trunc() { }

        /// <summary>
        /// Entry form constructor - get the full object.
        /// 
        /// Might add a hook to see if we can keep this from becoming visible.
        /// </summary>
        /// <param name="entry">Entry value to split into the entry box.</param>
        /// <param name="addenda">Addenda value to split into the addenda box.</param>
        /// <param name="visible_">determine if the new form is visible</param>
        /// <param name="entry_number">determine where in the sequence the new entry belongs.</param>
        public ENTRY_FORM(string entry = "", string addenda = "", bool visible_ = true,int entry_number = 0)
        {
            // go ahead and store the last entry value at the beginning then update as we go.
            ENTRY_FORM.last_entry = entry;
            InitializeComponent();

            // we'll add this update in each of these.
#if UPDATE_20190725
            foreach (Control c in this.Controls)
            {
                Program.set_font(c); // let's see if this works.
            }
#endif

            this.entry_number = entry_number;
            this.Visible = visible_; // will offer a way to hide these in the background, since I'm going to need to run one for every main object.
            // we'll be trying to search these out by the amount and record name, and if only one is provided, try best matches.
            // we'll want to make sure the objects are set to a consistent font... makes life a bit easieer.
            this.input_reference = new Dictionary<string, TextBox>();
            this.output_reference = new Dictionary<string, string>();
            this.label_reference = new Dictionary<string, Tuple<Label, TextBox>>
            {
                { "E. Rec Type",new Tuple<Label, TextBox>(this.label1,this.textBox1) },
                { "Trans. Code",new Tuple<Label, TextBox>(this.label2,this.textBox2) },
                { "Routing Num",new Tuple<Label, TextBox>(this.label3,this.textBox3) },
                { "Check Digit",new Tuple<Label, TextBox>(this.label4,this.textBox4) },
                { "Account Num",new Tuple<Label, TextBox>(this.label5,this.textBox5) },
                { "Amount     ",new Tuple<Label, TextBox>(this.label6,this.textBox6) },
                { "ID Number  ",new Tuple<Label, TextBox>(this.label7,this.textBox7) },
                { "Rec. Name  ",new Tuple<Label, TextBox>(this.label8,this.textBox8) },
                { "Disc. Data ",new Tuple<Label, TextBox>(this.label9,this.textBox9) },
                { "Addenda Ind",new Tuple<Label, TextBox>(this.label10,this.textBox10) },
                { "Trace Num. ",new Tuple<Label, TextBox>(this.label11,this.textBox11) },
                { "A. Rec Type",new Tuple<Label, TextBox>(this.label12,this.textBox12) },
                { "Adden. Type",new Tuple<Label, TextBox>(this.label13,this.textBox13) },
                { "Paymnt Info",new Tuple<Label, TextBox>(this.label14,this.textBox14) },
                { "Add Seq Num",new Tuple<Label, TextBox>(this.label15,this.textBox15) },
                { "Ent Seq Num",new Tuple<Label, TextBox>(this.label19,this.textBox16) }

            };

            // in the original box, we'll display the original value.
            this.label16.Text = entry; // list the record.
            this.label17.Text = "Entry Values";
            this.label18.Text = "Addenda Values";

            this.button1.Text = "SAVE";
            this.button2.Text = "CANCEL";
#if DEBUG
            this.button3.Visible = true;
            this.button3.Text = "CLEAR";
#else
            this.button3.Visible = false;
            this.button3.Enabled = false;
#endif
            foreach(Control c in this.Controls)
            {
                // not the best work but I think it'll help.
                if(c.Name.Contains("button"))
                {
                    c.Click += button_click; // will this work?
#if DEBUG
                    Console.WriteLine($"Setting {c.Name} to button handle");
#endif 
                }
                
            }

            // If I'm going to adjust the display values for the labels, I can assign it here.
            foreach(string a in this.label_reference.Keys)
            {
                try
                {
                    // I think these will be updated in real time..
                    this.label_reference[a].Item1.Text = a; // set all of the names.
                    this.input_reference.Add(a, label_reference[a].Item2);
                    this.output_reference.Add(a, label_reference[a].Item2.Text);
                    // this should be fine.

                }catch(Exception ECHO)
                {
                    Console.WriteLine(ECHO.ToString()); // not too surprised.

                }
            }


            // last steps in the constructor:
            try
            {
                // I must have left that in the middle of handling the DEBUG call removal
                split_entry(entry);
                // we never included split_addenda - 
                if (Program.EXPERT_MODE || Program.HANDLE_ADDENDA) {
                    try
                    {
                        split_addenda(addenda);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        } // terminates the object constructor.
    }
}
