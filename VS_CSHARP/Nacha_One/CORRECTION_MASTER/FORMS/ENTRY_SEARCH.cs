using System;
using System.Collections.Generic;
using System.Windows.Forms;

// changes:
//   - 2019-07-25: added function to interrupt direct call of editor form.
//   - 2019-07-25: added mechanism where listbox1 will initialize with the current list of keys - so that we don't have an unsightly empty box.
//   - 2019-07-25: added option for case-insensitive search option.
//
//
// todo:
//   - clean up and document this file, then lock it for future editing.
//   - everything in this file is currently subject to the UPDATE
//   - make searching case insensitive.
//
namespace CORRECTION_MASTER.FORMS
{
    /// <summary>
    /// Entry searching mechanism. This will assist in correcting large and otherwise unwieldly files.
    /// </summary>
    public partial class ENTRY_SEARCH : Form
    {
        // selectsearch shoudl be the mapping from the radiobutton
        private Dictionary<RadioButton, Tuple<string,string,string>> selectSearch = new Dictionary<RadioButton, Tuple<string,string,string>>();

        // foundSearch will be the subset of matching values found in the file.
        private Dictionary<string,Tuple<string,string>> foundSearch = new Dictionary<string, Tuple<string, string>>();

        // sourceSearch should be the original setup.
        private Dictionary<string, Tuple<string, string>> sourceSearch = new Dictionary<string, Tuple<string, string>>();
        private Dictionary<Tuple<string, string>,Tuple<string,string>> sourceKeyVals = new Dictionary<Tuple<string, string>, Tuple<string, string>>();

        // this tuple will become the selected key - and should match one of the keys for the main output.
        public Tuple<string, string> selectedKey = new Tuple<string, string>("","");


        /// <summary>
        /// This will allow the expansion of key information as necessary.
        /// </summary>
        /// <param name="sourceKeys"></param>
        public void setSourceSearch(Dictionary<string,Tuple<string,string>> sourceKeys)
        {
            this.sourceSearch = sourceKeys; // I must be pretty tired - since I keep trying to put parenthesis on everything like a function.
            foreach(var a in sourceKeys)
            {
                this.listBox1.Items.Add(a);
            }
        }

        /// <summary>
        /// I've got to get these names under control
        /// </summary>
        /// <param name="source"></param>
        public void setSourceKeyVals(Dictionary<Tuple<string,string>,Tuple<string,string>> source)
        {
            this.sourceKeyVals = source;
        }


        public void dislpayCurrentKeyVals()
        {
            //
            foreach(var b in this.sourceSearch.Keys)
            {
                this.listBox1.Items.Add(b);
            }
        }

        

        //
        public string selectedSearch = "";

        // we'll need to check that the dictionary contains the key when we get through here.
        // something is getting mangled in this output.
        private string getSearchKey(Tuple<string, string> value)
        {
            string key = "";
            //foreach (string a in this.sourceSearch.Keys)
            // get the first key that matches the values.
            // this should help to reduce issues from duplicates as well.
            foreach (var z in sourceSearch.Keys)
            {
                if (sourceSearch[z] == value) // this should more accurately reflect what we need.
                {
#if DEBUG
                    // this should happen once for every line - might cause a slowdown on exceptionally
                    // large files.
                    // I'm going to need to add some benchmarking and optimization for this.
                    Console.WriteLine($"{sourceSearch[z]}:{value}");
                        #endif
                    key = z;
                    break;
                }
            }
            return key;
        }

        // I'm getting some object out of range errors - assuming thats happening down in the second block.
        public void SearchAndReturn()
        {
            var key = "";
            RadioButton r = new RadioButton();
            this.foundSearch.Clear();
            foreach(RadioButton rb in this.selectSearch.Keys)
            {
                if (rb.Checked) { r = rb; break; }

            }

            this.listBox1.Items.Clear();// there we go.

            if(r ==this.radioButton4)
            {
                // converted these
                foreach (var a in this.sourceSearch.Values)
                {
                    // use this loop to try to make sure that we have the appropriate output.
                    key = getSearchKey(a);
                    if (this.sourceSearch.ContainsKey(key))
                    {
                        // placeholder could be optimized.
                    }
                    else
                    {
                        continue; // if we don't have the key - then move to the next loop.
                    }
                    // so what I'm going to do is pull in the original dictionary as well.
                    //if (sourceSearch[a].Item1.Contains(this.textBox1.Text))
                    if (Program.CASE_INSENSITIVE_ENTRY_SEARCH)
                    {
                        if (sourceKeyVals[a].Item1.ToLower().Contains(this.textBox1.Text.ToLower()))
                        {
                            // trying to use key - since that is what was meant by a originally.
                            this.foundSearch.Add(key, sourceSearch[key]); // so that should be fine.
                            this.listBox1.Items.Add(key); // show the selected file.
                        }
                    }
                    else
                    {
                        if (sourceKeyVals[a].Item1.Contains(this.textBox1.Text))
                        {
                            // trying to use key - since that is what was meant by a originally.
                            this.foundSearch.Add(key, sourceSearch[key]); // so that should be fine.
                            this.listBox1.Items.Add(key); // show the selected file.
                        }
                    }
                }
            }
            else
            {
                foreach(var a in this.sourceSearch.Values)
                {
                    // replace with the getkey reference here.
                    key = getSearchKey(a);
#if DEBUG
                    Console.WriteLine($"Found key: {key}");
#endif
                    if(this.sourceSearch.ContainsKey(key))
                    {
                        // placeholder could be optimized.
                    }
                    else
                    {
                        continue; // if we don't have the key - then move to the next loop.
                    }
                    // probably getting my exception here - I should add a separate handler for parsing out the amount from the dollar amount entries.
#if DEBUG
                    //var aa = sourceSearch[a].Item1; // string we're scanning.
                    var aa = sourceKeyVals[a].Item1; // that should be fine.
                    var ab = this.selectSearch[r].Item3; // entry form.key
                    var ac = ENTRY_FORM.field_map[ab]; // tuple entry
                    var ad = ac.Item1; // start
                    var ae = ac.Item2; // length

                    Console.WriteLine($"Source Line: {aa}\nsearching by: {ab}\nstart index:{ad} and length:{ae}; Current Object Length:{aa.Length}");
                    // if aa.Length is consistently short - then we'll need to adjust how we're scanning these.
                    // Okay - so these are breaking because the keys we're getting are the output keys...
                    if(ad+ae>aa.Length)
                    {
                        Console.WriteLine($"Cannot proceed: {aa.Length} is less than {ad + ae}");
                        continue; // we'll try to bypass the errors here for now.
                    }
#endif
                    //if (sourceSearch[a].Item1.Substring(ENTRY_FORM.field_map[this.selectSearch[r].Item3].Item1, ENTRY_FORM.field_map[this.selectSearch[r].Item3].Item2).Contains(textBox1.Text))
                    if(Program.CASE_INSENSITIVE_ENTRY_SEARCH)
                    {
                        if (sourceKeyVals[a].Item1.Substring(ENTRY_FORM.field_map[this.selectSearch[r].Item3].Item1, ENTRY_FORM.field_map[this.selectSearch[r].Item3].Item2).ToLower().Contains(textBox1.Text.ToLower()))
                        {
                            // so returning a works - key doesn't...
                            //this.foundSearch.Add(a, sourceSearch[a]);
                            this.foundSearch.Add(key, sourceSearch[key]);  // key should get set foreach version.
                                                                           // this.listBox1.Items.Add(a); // this is the original way that works magically.
                            this.listBox1.Items.Add(key); // let's try it this way.
#if DEBUG
                            Console.WriteLine("Coolio - this was wild.");
#endif
                        }
                    }
                    else
                    {
                        if (sourceKeyVals[a].Item1.Substring(ENTRY_FORM.field_map[this.selectSearch[r].Item3].Item1, ENTRY_FORM.field_map[this.selectSearch[r].Item3].Item2).Contains(textBox1.Text))
                        {
                            // so returning a works - key doesn't...
                            //this.foundSearch.Add(a, sourceSearch[a]);
                            this.foundSearch.Add(key, sourceSearch[key]);  // key should get set foreach version.
                                                                           // this.listBox1.Items.Add(a); // this is the original way that works magically.
                            this.listBox1.Items.Add(key); // let's try it this way.
#if DEBUG
                            Console.WriteLine("Coolio - this was wild.");
#endif
                        }
                    }
                    
                }
            }

        }

        // return a list of matching names;
        void getName() { }

        // return a list of matching amounts;
        void getAmount() { }

        // return a list of matching account numbers;
        void getAccount() { }

        void get___Something___I_guess____() { } //

        private void button1Click(object sender, EventArgs e)
        {
            string message = "";
            foreach(var a in this.selectSearch.Keys)
            {
                if(a.Checked)
                {
                    message = selectSearch[a].Item1 + ":::" + selectSearch[a].Item2 + "==" + textBox1.Text ; // this should be fine.
                }
            }
            Console.WriteLine($"Current selected value is {message}");
            SearchAndReturn(); // this should be fine here.
        }

        private void button2Click(object sender, EventArgs e)
        {
            // cancel and close;
            this.DestroyHandle(); // return to the caller by removing the instance of this object.
        }

        // return the selected item in the listbox.
        private void button3Click(object sender, EventArgs e)
        {
            try
            {
                // we'll try using this to pass the string through.
                Console.WriteLine(this.listBox1.SelectedItem.ToString()); // so is something preventing it here.
                this.selectedKey = this.sourceSearch[this.listBox1.SelectedItem.ToString()];
                // submit and close
                this.Close(); // better - simply close the form.
            }
            catch {
                Console.WriteLine("Incorrect key: available keys as follows...");
                foreach(var key in this.sourceSearch.Keys)
                {
                    Console.WriteLine($"{key}: {key.GetType()}");
                }
            }
            finally
            {

            }
            
        }

        private void radioButtonSelect(object sender,EventArgs e)
        {
            // we'll depend on the sender to define where we are from.
            // resource intensive placeholder.
            RadioButton rb = new RadioButton();
            // get all of the radiobuttons.
            foreach(RadioButton a in new List<RadioButton> { this.radioButton1,this.radioButton2,this.radioButton3,this.radioButton4 })
            {
                if(sender.Equals(a))
                {
                    rb = a; // assign rb.
                    break; // exit the loop
                }
            }
            string s = selectSearch[rb].Item2;
#if DEBUG
            Console.WriteLine($"Search by: {selectSearch[rb].Item1}");
                #endif
            switch (s)
            {

            }

        }

        /// <summary>
        /// Entry search constructor
        /// </summary>
        public ENTRY_SEARCH()
        {
            // modified constructs - member objects are now listed as public with docstrings.
            InitializeComponent();
            this.selectSearch = new Dictionary<RadioButton, Tuple<string,string,string>>
            {
                { this.radioButton1,new Tuple<string,string,string>("Account Name","name","Rec. Name  ") },
                { this.radioButton2,new Tuple<string,string,string>("Entry Amount", "amount","Amount     ") },
                { this.radioButton3,new Tuple<string,string,string>("Account Number", "account","Account Num") },
                { this.radioButton4,new Tuple<string,string,string>("Entry Substring", "entry","") }
            };

#if UPDATE_20190725
            foreach (Control c in this.Controls)
            {
                Program.set_font(c); // let's see if this works.
            }
#endif

            this.button1.Text = "Search";
            this.button2.Text = "Cancel";
            this.button3.Text = "Submit";

            this.label1.Text = "Search By:";
            this.label2.Text = "Search For:";
            // button3 should return and close the form.
            // listBox1 should show the matching entries.

            // I should start utilizing the recursive control searching mechansim...
            foreach(Control c in this.panel1.Controls)
            {
                if(c.Name.Contains("radioButton")||c.Name.Contains("RadioButton"))
                {
                    Console.WriteLine("Assigning button control");
                    c.Click += radioButtonSelect;
                    foreach(RadioButton r in selectSearch.Keys)
                    {
                        if (c.Equals(r))
                        {
                            //r.CheckedChanged += radioButtonSelect;
                            c.Text = selectSearch[r].Item1; // 
                        }
                    }
                }
                c.Click += null; // setting to null for now.
            }
            this.button1.Click += button1Click;
            this.button2.Click += button2Click;
            this.button3.Click += button3Click;
        }
    }
}
