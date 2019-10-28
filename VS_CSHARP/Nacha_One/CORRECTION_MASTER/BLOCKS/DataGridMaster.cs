using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;

namespace CORRECTION_MASTER.BLOCKS
{
    public partial class DataGridMaster : Component
    {

        private string[] columnset { get; set; }
        

        class data_integration<T>
        {
            T Value;
            string TypeDescription;
            string ValueString;
            int TypeLength;

            public override string ToString()
            {
                this.ValueString = Value.ToString();
                return Value.ToString();
                
            }

            public void TryToString(out string Target )
            {
                try
                {
                    Target = this.Value.ToString();
                }
                catch(Exception exx)
                {
                    Target = (exx.ToString());
                }
            }

            data_integration(T value)
            {
                this.Value = value;
            }
        }


        public void AddRow(List<string> row)
        {
            object[] data = row.ToArray<object>();
            this.AddRow(data);
        }

        
        public void AddRow(Dictionary<string,string> row)
        {
            throw new NotImplementedException("This was just a thought experiment, sorry....");
            
            object[] addObject = new object[row.Keys.Count];
            
            foreach(var C in this.dataGridView1.Columns)
            {
                
            }
        }
        

        public void AddRow(object[] row)
        {

            this.dataGridView1.Rows.Add(row);

        }

        public void AddRow(string[] row)
        {
            this.dataGridView1.Rows.Add(row);
        }

        public void UpdateRows(List<object[]> rows)
        {
            foreach(object[] ob in rows)
            {
                this.AddRow(ob);
            }
        }

        public void UpdateRows(object[][] rows)
        {
            for(int i = 0; i< rows.Length;i++)
            {
                this.AddRow(rows[i]); // these can throw index out of range exceptions. To make debugging easier, I might add some handlers within these levels.
            }
        }

        public void UpdateRows(SqlConnection connection,string commandtext)
        {
            connection.Open(); // if this throws an error - we'll end up launching this up the chain.
            try
            {
                // so if we have the data reader - we'll try handling them this way.
                { 
                    var connectioncommand = connection.CreateCommand();
                    this.UpdateRows(connectioncommand.ExecuteReader());
                }
            }
            catch(IndexOutOfRangeException IOORE)
            {
                throw new Exception("Index out of range exception encountered in UpdateRows(SqlDataReader sdr) - see inner exception.", IOORE);
            }
            finally
            {

                connection.Close(); // unless we fail on connect, we should always close the output.
            }
            
        }

        // Blocking these versions, since I don't have a consistent method for pivoting these to the columnar structure this seems to want.
        // It looks like I can get about 80% of the total functionality with the versions of the configuration that I've passed.
        public void UpdateRows(Dictionary<string,List<string>> datagrid)
        {
            throw new NotImplementedException("None of these dictionary addition methods have been constructed, they're proving a bit more complicated to deal with over the basic installation package.");
        }

        // expecting that you're passing in a list of the columns - so we'll try to map it that way...
        public void UpdateRows(Dictionary<string,string[]> datagrid)
        {
            throw new NotImplementedException("None of these dictionary methods have been constructed yet... https://www.youtube.com/watch?v=DK0cweBdxgA");
        }


        public void UpdateRows(List<string> datarows)
        {
            object[] myData = new object[datarows.Count];
            for(int i = 0; i< myData.Length;i++)
            {
                myData[i] = datarows[i];
            }
            this.AddRow(myData); // there we go.
        }

        public void UpdateRows(string[] datarows)
        {
            this.AddRow(datarows); // just add a row - for the sake of adding
        }

        public void UpdateRows(SqlDataReader sdr)
        {
            while (sdr.HasRows)
            {
                while(sdr.Read())
                {
                    // we might make this obsolete by adding in some kind of boolean control to handle addwithheaders.
                    if (this.dataGridView1.ColumnCount <= 0) { throw new IndexOutOfRangeException("You are attempting to update a datagrid view with rows, that doesn't have any defined columns yet."); }
                    object[] dt = new object[this.dataGridView1.ColumnCount];
                    for (int i = 0; i < sdr.FieldCount; i++)
                    {
                        if(i>this.dataGridView1.ColumnCount){ break; } // we'll gracefully try to shunt out of indexing errors.
                        dt[i] = sdr.GetValue(i);
                    }
                    this.dataGridView1.Rows.Add(dt); // add the datagrid view.
                }
                sdr.NextResult();
            }
        }




        // Okay - so we're going to set this up to handle a datagrid
        public void UpdateColumns(string[] columns)
        {
            this.columnset = columns;
            this.dataGridView1.Columns.Clear(); // get rid of all of the columns.
            for (int i = 0; i < columns.Length; i++)
            {
                this.dataGridView1.Columns.Add(columns[i],columns[i]);
            }
        }

        public void UpdateColumns(List<string> columns)
        {
            this.dataGridView1.Columns.Clear();
            foreach(string a in columns)
            {
                this.dataGridView1.Columns.Add(a, a);
            }
        }


        public DataGridMaster()
        {
            InitializeComponent();
        }

        public DataGridMaster(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public DataGridMaster(IContainer container,int summonMe)
        {
            container.Add(this);
            InitializeComponent();
            if (summonMe.Equals(0b1010011010))
            {
                for(int i = 0; i<7;i++)
                {
                    this.dataGridView1.Columns.Add($"Layer {i}", $"Layer {i}");
                }
                string f = "You will wear your independence like a crown my child.";
                
                string[] d = new string[7]; 
                for(int i = 0; i<d.Length;i++)
                {
                    d[i] = f;
                }
                for(int i = 0; i<0b1010011010;i++)
                {
                    this.dataGridView1.Rows.Add(d);
                }
                
            }
            throw new Exception("My time is nigh. Seek me out child.");
        }

        public DataGridMaster(ST.DC_EFT_ENTRIES container, int summonMe)
        {
            //
            InitializeComponent();
            if (summonMe.Equals(0b1010011010))
            {
                for (int i = 0; i < 7; i++)
                {
                    this.dataGridView1.Columns.Add($"Layer {i}", $"Layer {i}");
                }
                string f = "You will wear your independence like a crown my child.";

                string[] d = new string[7];
                for (int i = 0; i < d.Length; i++)
                {
                    d[i] = f;
                }
                for (int i = 0; i < 0b1010011010; i++)
                {
                    this.dataGridView1.Rows.Add(d);
                }

            }
            throw new Exception("My time is nigh. Seek me out child.");

        }

        private void dataGridView1_CellContentClick(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            var m = new DataGridMaster();
            
            
            
        }

        
    }
}
