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
    public partial class Startup : Form
    {
        public static bool allowContinue = false;
        public Startup()
        {
            InitializeComponent();

            this.label1.Text = $"    NACHA_ONE ACH FILE CORRECTION UTILITY\n" +
    $"Copyright(C) {DateTime.Now.Year}  Benjamin Tiernan\n\n" +

    "This program is free software; you can redistribute it and / or modify\n" +
      "it under the terms of the GNU General Public License as published by\n" +
      "the Free Software Foundation; either version 2 of the License, or\n" +
      "(at your option) any later version.\n\n" +

    "This program is distributed in the hope that it will be useful,\n" +
    "but WITHOUT ANY WARRANTY; without even the implied warranty of\n" +
    "MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the\n" +
    "GNU General Public License for more details.\n\n" +

    "You should have received a copy of the GNU General Public License along\n" +
    "with this program; if not, write to the Free Software Foundation, Inc.,\n" +
    "51 Franklin Street, Fifth Floor, Boston, MA 02110 - 1301 USA.\n";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            allowContinue = true;
            this.DestroyHandle();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(1); // you've declined.
        }
    }
}
