using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace CORRECTION_MASTER.FORMS
{
    public partial class NACHA_TO_XL : Form
    {
        public static Dictionary<string, Tuple<int, int>> Entry_Fields = new Dictionary<string, Tuple<int, int>>
        {
            { "Record Type", ENTRY_FORM.field_map["E. Rec Type"] },
            {"Transaction Code", ENTRY_FORM.field_map["Trans. Code"] },
            {"Routing Number" , ENTRY_FORM.field_map["Routing Num"] },
            {"Check Digit" , ENTRY_FORM.field_map["Check Digit"] },
            {"Account Number", ENTRY_FORM.field_map["Account Num"] },
            {"Amount", ENTRY_FORM.field_map["Amount     "] },
            {"ID Number", ENTRY_FORM.field_map["ID Number  "] },
            {"Record Name", ENTRY_FORM.field_map["Rec. Name  "] },
            {"Discretionary Data",ENTRY_FORM.field_map["Disc. Data "] },
            {"Addenda Indicator", ENTRY_FORM.field_map["Addenda Ind"] },
            {"Trace Number", ENTRY_FORM.field_map["Trace Num. "] }

        };


        public List<string> Dynamic_File = new List<string>();

        public FileInfo current_file = new FileInfo(".");
        
        
        private void write_to_csv()
        {
            var z = new SaveFileDialog();
            z.AddExtension = true;
            z.DefaultExt = "csv";
            z.ShowDialog(); // we'll then allow this to choose the file output.
            FileInfo fin = new FileInfo(z.FileName);

            List<string> output_list = new List<string>();

            string b_string = "";
            foreach(string a in Entry_Fields.Keys)
            {
                b_string += $"{a}\t";
            }
            output_list.Add(b_string);

            foreach(string a in Dynamic_File)
            {
                string t_string = "";

                foreach(string b in Entry_Fields.Keys)
                {
                    t_string += a.Substring(Entry_Fields[b].Item1, Entry_Fields[b].Item2) + "\t";
                }
                output_list.Add(t_string);
            }
            // 
            this.textBox1.Text = "";
            foreach(string a in output_list)
            {
                this.textBox1.Text += $"{a}\r\n";
            }
            if (z.FileName != "")
            {
                File.WriteAllLines(z.FileName, output_list.ToArray());
            }

            //

        }

        private List<string> deduplicate_file(List<string> input_lines)
        {
            List<string> output_lines = new List<string>();

            foreach(string ln in input_lines)
            {
                if (output_lines.Contains(ln))
                {
                    continue;
                }
                else
                {
                    output_lines.Add(ln);
                }
            }

            return output_lines;
        }

        private void process_new_file(string file)
        {
            this.textBox1.Text = "";
            var y = File.ReadAllLines(file);
            for (int i = 0; i < y.Length; i++)
            {
                // skip the output if it contains a header when rewriting these.
                if (Program.REMOVE_HEADERS_FOR_CSV)
                {
                    if (y[i].Length < 0)
                    {
                        break;
                    }
                    try
                    {


                        if (new List<string> { "1", "5", "8", "9" }.Contains(y[i].Substring(0, 1)))
                        {
                            continue;
                        }
                    }
                    catch (ArgumentOutOfRangeException AOORE)
                    {
                        continue; ; // we could attempt a continue; We'll see what happens.
                    }
                    catch (IndexOutOfRangeException IOORE)
                    {
                        break;
                    }
                }
                // we'll strip the headers under certain cases.
                this.Dynamic_File.Add(y[i]); // this should be fine now.
                if (this.textBox1.Visible)
                {
                    this.textBox1.Text += $"{y[i]}\r\n";
                }
            }

        }


        public void button_click_handler(object sender, EventArgs e)
        {
            string sender_text = "";
            foreach (Control b in this.Controls)
            {
                if (sender.Equals(b))
                {
                    sender_text = b.Text; // this will allow us to switch based on names.
                }
            }


            switch (sender_text)
            {
                case "Open":
                    var x = new OpenFileDialog();
                    x.ShowDialog();
                    if(x.FileName is null || x.FileName=="")
                    {
#if DEBUG
                        goto default;
#endif
                        break;
                    }
                    try
                    {
                        process_new_file(x.FileName);
                    }
                    catch(NullReferenceException NRE)
                    {
                        Console.WriteLine(NRE);
                    }
                    
                    break;
                case "Deduplicate":
                    Dynamic_File = deduplicate_file(Dynamic_File); // this should deduplicate the output.
                    this.textBox1.Text = "";
                    foreach(string a in Dynamic_File)
                    {
                        this.textBox1.Text += $"{a}\r\n";
                    }
                    break;
                case "Process":
                    //
                    write_to_csv();
                    break;
                default:
                    // I don't know if this would even work.
#if DEBUG
                    new Landing_Page().Show();
#endif
                    break;
            }
        }

        public void drag_event_handler(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;
            }
            else
            {
                e.Effect = DragDropEffects.None; // I guess prevent unhandled drops.
            }
        }

        public void drop_event_handler(object sender, DragEventArgs e)
        {
            // handle dropping file data?
            var f = e.Data;//.GetData("string", true);
            DataObject d_o = new DataObject();
            d_o.SetData(f); // painful
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                // Might add a mechanism to handle the data from text file.
            }
            // filedrop - we should trigger the process handle, just like filedialog.
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var obj = e.Data.GetData(DataFormats.FileDrop); // does this work?
                string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop); // gotcha - that's why my cast was screwy.
                bool proc_on_open = true; // this was handy - changing for production mode.
#if DEBUG
                proc_on_open = true;
#endif
                if (proc_on_open)
                {
                    // we'll hit the button event args, or the open  function.
                    process_new_file(filenames[0]);
                }
            }
        }


        public NACHA_TO_XL()
        {
            InitializeComponent();

            // we'll add an alternative version that uses a datagrid view rather than a text view.
            // make sure to incorporate all of the methods to keep them compatible with our older NACHA parsers.
            this.textBox1.ReadOnly = true;

            this.button1.Text = "Open";
            this.button2.Text = "Deduplicate";
            this.button3.Text = "Process";

            foreach(Control c in this.Controls)
            {
                if(c.Name.Contains("button"))
                {
                    c.Click += button_click_handler;
                }
            }

            
            this.panel1.AllowDrop = true;
            this.panel1.DragEnter += drag_event_handler;
            this.panel1.DragDrop += drop_event_handler;
            this.textBox1.BringToFront();
            this.textBox1.ScrollBars = ScrollBars.Both;

            this.checkedListBox1.Items.Add("Deduplicate");
            this.checkedListBox1.Items.Add("ProcessToCSV");
            this.checkedListBox1.Items.Add("Automatically Save");

            this.checkedListBox1.Visible = false;


            
            
        }
    }
}
