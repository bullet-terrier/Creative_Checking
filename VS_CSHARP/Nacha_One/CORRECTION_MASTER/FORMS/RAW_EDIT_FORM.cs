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
    public partial class RAW_EDIT_FORM : Form
    {
        string editFile { get; set; }// raw file string that we're planning on editing.



        public static void load_file(string target_file)
        {
            //
            try
            {
                // I'm not going to keep mucking with this one today - I need to set it up, but I can also use notepad for now.
            }
            catch(Exception e)
            {
                return;
            }
        }

        public void save_file()
        {
            // we'll do this to set out the save_files.
        }
        

        public RAW_EDIT_FORM()
        {
            
            InitializeComponent();
            this.tableLayoutPanel1.Dock = DockStyle.Fill;
            this.panel1.Dock = DockStyle.Fill;
            this.panel2.Dock = DockStyle.Fill;
            this.textBox1.Dock = DockStyle.Fill;
            
        }

        private List<Control> traverseControls(Control parent)
        {
            List<Control> myCon = new List<Control> { parent };
            if (parent.Controls.Count > 0)
            {
                foreach (Control c in parent.Controls)
                {
                    foreach (Control C in traverseControls(c)) ; // there we go...
                }
            }
            return myCon;
        }
    }
}
