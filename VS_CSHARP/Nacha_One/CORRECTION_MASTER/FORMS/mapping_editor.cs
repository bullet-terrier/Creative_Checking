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
    public partial class mapping_editor : Form
    {
        public mapping_editor()
        {
            InitializeComponent();


            List<Label> EntryFields = new List<Label> {
                label1,
                label2,
                label3,
                label4,
                label5,
                label6,
                label7,
                label8,
                label9,
                label10,
                label11
            };
#if UPDATE_201909
            // update teh output with the apropriate name... we'll test to make sure this is fine.
            for(int i = 0;i<12; i++)
            {
                var z = ENTRY_FORM.field_map_mapping();
                EntryFields[i].Text = z.Keys.ToList<string>()[i];
            }
#endif
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            
        }
    }
}
