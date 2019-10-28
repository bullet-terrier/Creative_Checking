#define DRAGDROPENABLED
#define HIGHLIGHT_LEGACY
#define UPDATE_20190718

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

// I might want to add a tool to disable EXPERT mode...
namespace CORRECTION_MASTER
{
    public partial class Landing_Page : Form
    {
        private static ACH_FILE working_file { get; set; }

        // okay - with the file open, and current_rex populated, we can add in an "ACH_FILE" object.
        private static FileInfo current_file { get; set; }

        // okay - this will have the rows listed. I'm going to need somewhere to store the mapped indices.
        private static string[] current_rex { get; set; } // write current_rex - we'll have to update this somehow.
        private static string[] old_rex { get; set; }

        // okay - so the framework is here - I'll need to tweak a few things to make it work here.
#if DRAGDROPENABLED
        public void drag_enter(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.Text))
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

        public void drop_object(object sender, DragEventArgs e)
        {
            var f = e.Data;
            DataObject d_o = new DataObject();
            d_o.SetData(f);
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                // removing these text blocks...
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var obj = e.Data.GetData(DataFormats.FileDrop);
                string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop); 
                // todo: 
                //    add the ability to drop multiple files and open multiple instances... not hard, but time consuming.
                current_file = new FileInfo(filenames[0]); // only get the first entry;
                label2.Visible = true;
                label2.Text = current_file.FullName;
                current_rex = File.ReadAllLines(filenames[0]);
                working_file = new ACH_FILE(current_rex);
                bool proc_on_open = true; 
#if DEBUG
                proc_on_open = true;
#endif
                if (proc_on_open)
                {
                    show_search(working_file);
                }
            }
        }
#endif


        //
        public void SaveAsMenuItemClick(object sender, EventArgs e)
        {
            var old_file = current_file;
            try
            {
                var m = new SaveFileDialog();
                m.ShowDialog();
                current_file = new FileInfo(m.FileName);
                //
                File.WriteAllLines(m.FileName, current_rex); // this should write directly out as desired.
            }
            catch(Exception ex)
            {
                current_file = old_file;
            }

        }

        // added 07-18-2019
        /// <summary>
        /// Utility to create a new ACH file that can be edited and saved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NewMenuItemClick(object sender, EventArgs e)
        {
            //
            //
            //
        }

        // maybe I shouldn't let these be static.
        public void OpenMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                var m = new OpenFileDialog();
                m.ShowDialog();
                current_file = new FileInfo(m.FileName);
                //
                current_rex = File.ReadAllLines(m.FileName); // that should work well.  Might need to handle some kinds of errors.
                // now, we'll try loading the records into an ACH_FILE object.
                working_file = new ACH_FILE(current_rex); // 
#if DEBUG
                foreach(Tuple<string,string> a in working_file.entryAddenda.Keys)
                {
                    // with this, I should be able to manipulate the files as I see fit.
                    Console.WriteLine($"Entry: {a.Item1} Amount: {a.Item2} RECORD: ${working_file.entryAddenda[a].Item1}");
                }
#endif
                if(Program.PROCESS_ON_OPEN || Program.EXPERT_MODE)
                {
                    show_search(working_file);
                }
                Console.WriteLine(current_file.FullName);
                label2.Visible = true;
                label2.Text = current_file.FullName;
            }
            catch(Exception ex)
            {
                current_rex = new string[0]; // clear rex
                label2.Visible = false;
                label2.Text = "";
                current_file = null;
                Console.WriteLine(ex.StackTrace);
            }
        }

        // after the editor, our fields should be updated with the new records.
        public void show_search(ACH_FILE open_file)
        {
#if UPDATE_20190718

            // we'll be using the following loop only when that record is broken.
            var m = new FORMS.ACCESS_RECORD(); // don't declare open_file here.
            // might add an option to prevent it from continuing after this loop - let's find out how it's going.
            m.InterruptShowDialog(open_file); // allows it to be called with as many arguments.
#else
            // this is being changed based on the new format.
            var m = new FORMS.ACCESS_RECORD(open_file);
            m.ShowDialog();
#endif

            // technically everything from here should be viable to handle in the backgroung, once the new file is built.
#if DEBUG
            string fname = "./Outout_Test_2.txt";
            File.WriteAllText(fname, open_file.file_header+"\n");
            File.AppendAllText(fname, open_file.batch_header + "\n");
            // adding debug hook to try it out.
            foreach(var a in open_file.entryAddenda.Keys)
            {
                var myTup = open_file.entryAddenda[a];
                File.AppendAllText(fname, myTup.Item1+"\n"); // should write the entry files.
                if(!new List<string> { " ", "0", "" }.Contains(myTup.Item2))
                {
                    File.AppendAllText(fname, myTup.Item1+"\n"); // should write the entry files.
                }
                // if this works and matches the record order, then I'll go ahead and add the logic to construct the batch components
                // *NEED TO ENSURE THAT ADDENDA GET HANDLED TOO.
            }
#else
#endif
            old_rex = current_rex;
            current_rex = working_file.get_file(); // this should yield the final product.
#if DEBUG
            for(int i = 0; i< current_rex.Length; i++)
            {
                Console.WriteLine(current_rex[i]);

            }
            File.WriteAllLines("./test_final.ach", current_rex);
#endif
            //
            // add the condition for this one, wehere we automatically begin using save.
            // use Expert mode to minimize keystrokes
            //
            if(Program.SAVE_ON_CLOSE || Program.EXPERT_MODE)
            {
                this.ButtonObjectClick(button3, new EventArgs());
            }            
        }


        public void ButtonObjectClick(object sender, EventArgs e)
        {
            if (sender.Equals(button1))
            {
                OpenMenuItemClick(sender, e);
            }
            if(sender.Equals(button2))
            {
                try
                {
                    show_search(working_file);
                }
                catch(NullReferenceException nre)
                {
                    // we'll need to put in an error log.
                    // I need to add said werror log to all of my applications
#if DEBUG
                    Console.WriteLine("Desired outcome is to simply prevent the application from failing if this is the case - we want it to do nothing if the file hasn't been established yet.");
#endif
                }
            }                    
            if (sender.Equals(button3))
            {
                SaveAsMenuItemClick(sender, e);
            }
            if(sender.GetType() == menuStrip1.Items[0].GetType())
            {
                // we'll show the new output.
                var m = new FORMS.ABOUT_NACHA();
                m.ShowDialog();
                // 
            }
            
        }

        public void ExportDropDownButtonClick(object sender, EventArgs e)
        {
            var x = new FORMS.NACHA_TO_XL().ShowDialog();
        }
        

        // we'll want to tweak a few elements in here.
        public Landing_Page()
        {
            
            InitializeComponent();

            // this can happen right after initialize_component which is fun.
#if UPDATE_20190725
            foreach (Control c in this.Controls)
            {
                Program.set_font(c); // let's see if this works.
            }
#endif

            this.label1.Text = "Drag file here: ";
            this.button1.Text = "Open";
            this.button2.Text = "Edit";
            this.button3.Text = "Save";
            this.label2.Visible = false;
            this.panel1.AllowDrop = true;
            this.Text = "NACHA ONE FILE CORRECTIONS";
            this.Name = "NACHA ONE FILE CORRECTIONS";
            
            this.panel1.DragEnter += drag_enter;
            this.panel1.DragDrop += drop_object; // should allow d&d now.

            //this.button1.Click += OpenMenuItemClick;
            // DON'T FORGET TO ASSIGN TO EVENT - NOT TO THE OBJECT.
            //this.fileMenustripToolItem.Items[0]+= OpenMenuItemClick;
            this.fileToolStripMenuItem.DropDownItems[0].Click += OpenMenuItemClick;
            this.fileToolStripMenuItem.DropDownItems[1].Click += SaveAsMenuItemClick;
            this.fileToolStripMenuItem.DropDownItems[2].Visible = false; // hiding the option for a new file.

            this.menuStrip1.Items.Add("About");
            this.menuStrip1.Items[menuStrip1.Items.Count - 1].Click += ButtonObjectClick;

            this.fileToolStripMenuItem.DropDownItems.Add("Export");
            this.fileToolStripMenuItem.DropDownItems[this.fileToolStripMenuItem.DropDownItems.Count - 1].Click += ExportDropDownButtonClick;
            //this.menuStrip1.Items[1].Click += SaveAsMenuItemClick; // this should *hopefully* allow this to function.
            // this was setting the edit button to saveas.

            if (Program.ALLOW_OLD_EDITOR)
            {
                this.editToolStripMenuItem.Visible = true;
                this.editToolStripMenuItem.DropDownItems.Add("Retro Editor");
                this.editToolStripMenuItem.Click += Program.START_OLD_EDITOR; // this should handle the old editor call...
            }

            foreach(Control a in this.Controls)
            {
                if (a.Name.Contains("button"))
                {
                    a.Click += ButtonObjectClick; // add the click handler.
                }
            }

#if UPDATE_201909
            this.menuStrip1.Items.Add("EFT Files by Date");
            this.menuStrip1.Items[this.menuStrip1.Items.Count - 1].Click += getFilesByResolutionDate;
#endif

        }

#if UPDATE_201909
        private void getFilesByResolutionDate(object sender, EventArgs e)
        {
            var m = new ST.DC_EFT_ENTRIES();
            m.Show();
            return;
        }
#endif
    }
}
