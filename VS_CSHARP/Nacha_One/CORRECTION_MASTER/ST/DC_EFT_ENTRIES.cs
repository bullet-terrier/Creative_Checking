using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// this won't be a super well written module, but it might be able to be dropped elsewhere.
// worst case scenario is using a regex.replace...
using System.IO; 
using System.Data.SqlClient;
using System.Configuration;
using CORRECTION_MASTER.BLOCKS;

namespace CORRECTION_MASTER.ST
{
#if ST_BUILD
    public partial class DC_EFT_ENTRIES : Form
    {
        // okay - so we'll wrap this up pretty thoroughly, I really need to learn to make better use of generics.
        private string connectionString { get; set; } 
        private string baseQuery { get; set; }
        private DateTime targetDate { get; set; }
        private delegate void getBaseQuery();

        
        // screw it - going to go with the most direct version with a hard coded query - I'm 
        // too burned out to worry about modularity in the ST specific module.
        public DC_EFT_ENTRIES()
        {
            try
            {
                
                baseQuery = ConfigurationManager.AppSettings["DCT_QUERY_PATH"];
                string conPath = ConfigurationManager.AppSettings["DCT_CONNECTIONSTRING"];
                connectionString = ConfigurationManager.ConnectionStrings[conPath].ToString();
                //connectionString = (ConfigurationManager.ConnectionStrings[(ConfigurationManager.AppSettings["DCT_CONNECTIONSTRING"] ?? "")].ToString() ?? "");
                InitializeComponent();
                //var m = new DataGridMaster(666);
                //m.Site = this.dataGridView1.Site;
                dateTimePicker1.Value = DateTime.Now; // we'll default it to the current datetime.
             
            }
            catch(Exception E)
            {
                Console.Write(E);

                InitializeComponent();
                
                
            }

            string rawcolumns = (ConfigurationManager.AppSettings["DCT_COLUMN_NAMES"]);
            string[] columns = rawcolumns.Split(','); // get the individual column names.
            for (int i = 0; i < columns.Length; i++)
            {
                dataGridView1.Columns.Add(columns[i], columns[i]); // should handle all of the column names.
            }

            // so we can update the datetime picker...
            this.dateTimePicker1.ValueChanged += updateTableGridFromDate; // should update the grid views.
        }

        private void updateTableGridFromDate(object sender, EventArgs e)
        {
            this.dataGridView1.Rows.Clear();
            // gotta love events.
            SqlConnection CONN = new SqlConnection(connectionString);
            CONN.Open();
            try
            {
                // need logic for if @drawdate exists, and if it doesn't.
                var CURS = CONN.CreateCommand();
                CURS.CommandText = File.ReadAllText(baseQuery); // make sure that this is the base file.
                try
                {
                    CURS.Parameters.AddWithValue((ConfigurationManager.AppSettings["DCT_DATE_PARAMETER"] ?? "@drawDate"), this.dateTimePicker1.Value);
                }
                catch (Exception se)
                {
                    CURS.Parameters[("@" + ConfigurationManager.AppSettings["DCT_DATE_PARAMETER"] ?? "@drawDate")].Value = dateTimePicker1.Value;
                    Console.WriteLine(CURS.CommandText);
                }
                
                var RESULTS = CURS.ExecuteReader();
                while (RESULTS.HasRows)
                {
                    while (RESULTS.Read())
                    {
                        // looks like adding the +1 was necessary -but made it work.
                        object[] dt = new object[dataGridView1.ColumnCount];
                        for (int i = 0; i < RESULTS.FieldCount; i++)
                        {
                            dt[i] = RESULTS.GetValue(i); // so this is almost a generic.

                        }
                        dataGridView1.Rows.Add(dt); // add the datagrid output.
                    }
                    RESULTS.NextResult();
                }
            }
            catch (Exception es)
            {
                // I really should integrate these with some kind of windows event log objects.
                Console.WriteLine(es); // need 
            }
            finally
            {
                CONN.Close(); // close the connection - might put this in a try/catch/finally.
            }

        }

        private void DC_EFT_ENTRIES_Load(object sender, EventArgs e)
        {

        }
    }
#endif
}
