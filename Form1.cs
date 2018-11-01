
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.IO;

namespace SystemeDarkhast
{
    public partial class Form1 : Form
    {


        private string beingEditedId = null;

        #region Print Member Variables

        int printType = 0;
        StringFormat strFormat; //Used to format the grid rows.
        ArrayList arrColumnLefts = new ArrayList();//Used to save left coordinates of columns
        ArrayList arrColumnWidths = new ArrayList();//Used to save column widths
        int iCellHeight = 0; //Used to get/set the datagridview cell height
        int iTotalWidth = 0; //
        int iRow = 0;//Used as counter
        bool bFirstPage = false; //Used to check whether we are printing first page
        bool bNewPage = false;// Used to check whether we are printing a new page
        int iHeaderHeight = 0; //Used for the header height

        string headerString1;
        string headerString2;

        DataSet tmpSet;
        DataSet printSet;
        DataGridView printGridView;


        SqlCeConnection printCon;
        SqlCeDataAdapter printAdapter;
        DataTable printTable;
        DataRow printRow;
        DataColumn printColumn;


        #endregion

        string whereClause;

        SqlCeConnection con;
        SqlCeTransaction trans;
        SqlCeCommand com;
        SqlCeDataAdapter adapter;
        DataSet set;
        string printStream;

        System.Collections.Specialized.StringCollection pictureAddresses = new System.Collections.Specialized.StringCollection();

        #region Fonts Problem

        SolidBrush solidBrush = new SolidBrush(Color.Black);
        FontFamily[] fontFamilies;
        PrivateFontCollection privateFontCollection = new PrivateFontCollection();
        Font Mitra, MitraBd, TitrBd, Mitra8, MitraBd8;

        #endregion

        public Form1()
        {
            InitializeComponent();

            privateFontCollection.AddFontFile(Application.StartupPath + @"\Fonts\BMitra.ttf");
            privateFontCollection.AddFontFile(Application.StartupPath + @"\Fonts\BMitraBd.ttf");
            privateFontCollection.AddFontFile(Application.StartupPath + @"\Fonts\BTitrBd.ttf");

            fontFamilies = privateFontCollection.Families;


            Mitra = new System.Drawing.Font(fontFamilies[0], 12, System.Drawing.FontStyle.Regular);
            MitraBd = new System.Drawing.Font(fontFamilies[0], 12, System.Drawing.FontStyle.Bold);
            TitrBd = new System.Drawing.Font(fontFamilies[1], 12, System.Drawing.FontStyle.Bold);
            Mitra8 = new System.Drawing.Font(fontFamilies[0], 8, System.Drawing.FontStyle.Regular);
            MitraBd8 = new System.Drawing.Font(fontFamilies[0], 8, System.Drawing.FontStyle.Bold);

            label1.Font = label7.Font = label13.Font = label19.Font = label25.Font = label35.Font =
            label2.Font = label8.Font = label14.Font = label20.Font = label26.Font = label36.Font =
            label3.Font = label9.Font = label15.Font = label21.Font = label27.Font = 
            label4.Font = label10.Font = label16.Font = label22.Font = label32.Font =
            label5.Font = label11.Font = label17.Font = label23.Font = label33.Font =
            label6.Font = label12.Font = label18.Font = label24.Font = label34.Font =
            label28.Font = label29.Font = label30.Font = label31.Font =
            label37.Font = label38.Font = label39.Font = label40.Font = label41.Font = label42.Font =
            label43.Font = label44.Font = label45.Font = label46.Font = label47.Font = label48.Font =
            label49.Font = label50.Font = label51.Font = label52.Font = label53.Font = label54.Font =
            label55.Font = label56.Font = MitraBd;

            textBox1.Font = textBox8.Font = textBox15.Font = textBox22.Font = textBox31.Font =
            textBox2.Font = textBox9.Font = textBox16.Font = textBox23.Font =
            textBox3.Font = textBox10.Font = textBox17.Font = textBox24.Font =
            textBox4.Font = textBox11.Font = textBox18.Font = textBox27.Font =
            textBox5.Font = textBox12.Font = textBox19.Font = textBox28.Font =
            textBox6.Font = textBox13.Font = textBox20.Font = textBox29.Font =
            textBox7.Font = textBox14.Font = textBox21.Font = textBox30.Font =
            textBox6.Font = textBox13.Font = textBox20.Font = textBox29.Font =
            textBox25.Font = textBox26.Font = textBox32.Font = textBox33.Font =
            textBox34.Font = textBox35.Font = textBox36.Font = textBox37.Font =
            textBox38.Font = textBox39.Font = textBox40.Font = textBox41.Font =
            textBox42.Font = textBox43.Font = textBox44.Font = textBox45.Font =
            textBox46.Font = textBox47.Font = textBox48.Font = textBox49.Font = textBox50.Font = textBox51.Font =
            Mitra;

            button1.Font = button4.Font = button7.Font = button10.Font = 
            button2.Font = button5.Font = button8.Font = button11.Font =
            button3.Font = button9.Font = button13.Font = MitraBd;

            dataGridView1.Font = Mitra;

            tabControl1.Font = MitraBd;

            textBox25.ReadOnly = textBox26.ReadOnly = textBox32.ReadOnly = textBox33.ReadOnly =
            textBox34.ReadOnly = textBox35.ReadOnly = textBox36.ReadOnly = textBox37.ReadOnly =
            textBox38.ReadOnly = textBox39.ReadOnly = textBox40.ReadOnly = textBox41.ReadOnly =
            textBox42.ReadOnly = textBox43.ReadOnly = textBox44.ReadOnly = textBox45.ReadOnly =
            textBox46.ReadOnly = textBox47.ReadOnly = textBox48.ReadOnly = textBox49.ReadOnly = 
            textBox50.ReadOnly = true;


            populateAutoCompleteStringCollections();

            this.ActiveControl = textBox1;

            InitializeOpenFileDialog();

            this.Icon = Properties.Resources.Application;

            this.BackgroundImage = Properties.Resources.Gradient1;

        }


        private void InitializeOpenFileDialog()
        {
            // Set the file dialog to filter for graphics files.
            this.openFileDialog1.Filter =
                "Images (*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|" +
                "All files (*.*)|*.*";

            // Allow the user to select multiple images.
            this.openFileDialog1.Multiselect = true;
            this.openFileDialog1.Title = "انتخاب عکس";
        }


        private void button1_Click(object sender, EventArgs e)
        {

            con = new SqlCeConnection("Data Source=|DataDirectory|\\SystemeDarkhast.sdf");
            con.Open();
            com = con.CreateCommand();
            com.Connection = con;
            string insString = System.String.Empty;

            bool emptyfields = true;

            if (pictureAddresses.Count != 0)
                emptyfields = false;

            if (textBox27.Text != System.String.Empty) { insString += ("\'" + textBox27.Text + "\', "); emptyfields = false; }
            else insString += "NULL, ";

            if (textBox29.Text != System.String.Empty) { insString += ("\'" + textBox29.Text + "\', "); emptyfields = false; }
            else insString += "NULL, ";

            if (textBox30.Text != System.String.Empty) { insString += ("\'" + textBox30.Text + "\', "); emptyfields = false; }
            else insString += "NULL, ";

            if (textBox31.Text != System.String.Empty) { insString += ("\'" + textBox31.Text + "\', "); emptyfields = false; }
            else insString += "NULL, ";

            if (textBox1.Text != System.String.Empty) { insString += ("\'" + textBox1.Text + "\', "); emptyfields = false; }
            else insString += "NULL, ";

            if (textBox2.Text != System.String.Empty) { insString += ("\'" + textBox2.Text + "\', "); emptyfields = false; }
            else insString += "NULL, ";

            if (textBox3.Text != System.String.Empty) { insString += ("\'" + textBox3.Text + "\', "); emptyfields = false; }
            else insString += "NULL, ";

            if (textBox4.Text != System.String.Empty) { insString += ("\'" + textBox4.Text + "\', "); emptyfields = false; }
            else insString += "NULL, ";

            if (textBox5.Text != System.String.Empty) { insString += ("\'" + textBox5.Text + "\', "); emptyfields = false; }
            else insString += "NULL, ";

            if (textBox6.Text != System.String.Empty) { insString += ("\'" + textBox6.Text + "\', "); emptyfields = false; }
            else insString += "NULL, ";

            if (textBox7.Text != System.String.Empty) { insString += ("\'" + textBox7.Text + "\', "); emptyfields = false; }
            else insString += "NULL, ";

            if (textBox8.Text != System.String.Empty) { insString += ("\'" + textBox8.Text + "\', "); emptyfields = false; }
            else insString += "NULL, ";

            if (textBox9.Text != System.String.Empty) { insString += ("\'" + textBox9.Text + "\', "); emptyfields = false; }
            else insString += "NULL, ";

            if (textBox10.Text != System.String.Empty) { insString += ("\'" + textBox10.Text + "\', "); emptyfields = false; }
            else insString += "NULL, ";

            if (textBox11.Text != System.String.Empty) { insString += ("\'" + textBox11.Text + "\', "); emptyfields = false; }
            else insString += "NULL, ";

            if (textBox12.Text != System.String.Empty) { insString += ("\'" + textBox12.Text + "\', "); emptyfields = false; }
            else insString += "NULL, ";

            if (textBox13.Text != System.String.Empty) { insString += ("\'" + textBox13.Text + "\', "); emptyfields = false; }
            else insString += "NULL, ";

            if (textBox14.Text != System.String.Empty) { insString += ("\'" + textBox14.Text + "\', "); emptyfields = false; }
            else insString += "NULL, ";

            if (textBox15.Text != System.String.Empty) { insString += ("\'" + textBox15.Text + "\', "); emptyfields = false; }
            else insString += "NULL, ";

            if (textBox16.Text != System.String.Empty) { insString += ("\'" + textBox16.Text + "\', "); emptyfields = false; }
            else insString += "NULL, ";

            if (textBox17.Text != System.String.Empty) { insString += ("\'" + textBox17.Text + "\'"); emptyfields = false; }
            else insString += "NULL";


            com.CommandText = "INSERT INTO Darkhast (NameEkhtesari, GheymateForoush, EtebareGheymat, EstelameBaha, NameDarkhast, ShomarehTaghaza, ShomarehTarh, ShomarehTabaghebandi_MESC, PartNumber, IranKode, Size, NameGhete, MaterialeGhete, Rang, Sakhti, Tedad, TarikheTolid, TolidKonandeh, KodeGhaleb, Gheymat, KodeNaghshe) VALUES (" + insString + ");";

            if (emptyfields == false)
            {
                try
                {


                    com.ExecuteNonQuery();
                    con.Close();

                    con = new SqlCeConnection("Data Source=|DataDirectory|\\SystemeDarkhast.sdf");
                    con.Open();
                    string cmdText = "SELECT DISTINCT MAX(InternalID) FROM Darkhast";
                    adapter = new SqlCeDataAdapter(cmdText, con);
                    set = new DataSet();
                    adapter.Fill(set);

                    con.Close();

                    string internalID = set.Tables[0].Rows[0][0].ToString();

                    if (pictureAddresses.Count > 0)
                    {
                        try
                        {
                            Directory.CreateDirectory(Application.StartupPath + @"\Images\" + internalID + @"\");

                            for (int i = 0; i < pictureAddresses.Count; i++)
                            {

                                string oldPath = pictureAddresses[i];
                                string extension = Path.GetExtension(oldPath);
                                System.IO.File.Copy(oldPath, Application.StartupPath + @"\Images\" + internalID + @"\" + i + extension);

                            }
                        }
                        catch (Exception)
                        {
                            
                        }
                    }



                    string message = System.String.Empty;



                    if (set.Tables[0].Rows.Count != 0)
                        message = "ثبت انجام شد، شماره ثبت داخلی: " + internalID;




                    populateAutoCompleteStringCollections();

                    textBox1.ReadOnly = true;
                    textBox2.ReadOnly = true;
                    textBox3.ReadOnly = true;
                    textBox4.ReadOnly = true;
                    textBox5.ReadOnly = true;
                    textBox6.ReadOnly = true;
                    textBox7.ReadOnly = true;
                    textBox8.ReadOnly = true;
                    textBox9.ReadOnly = true;
                    textBox10.ReadOnly = true;
                    textBox11.ReadOnly = true;
                    textBox12.ReadOnly = true;
                    textBox13.ReadOnly = true;
                    textBox14.ReadOnly = true;
                    textBox15.ReadOnly = true;
                    textBox16.ReadOnly = true;
                    textBox17.ReadOnly = true;
                    textBox27.ReadOnly = true;
                    textBox29.ReadOnly = true;
                    textBox30.ReadOnly = true;
                    textBox31.ReadOnly = true;

                    button2.Enabled = false;
                    button8.Enabled = false;

                    textBox1.Update();
                    textBox2.Update();
                    textBox3.Update();
                    textBox4.Update();
                    textBox5.Update();
                    textBox6.Update();
                    textBox7.Update();
                    textBox8.Update();
                    textBox9.Update();
                    textBox10.Update();
                    textBox11.Update();
                    textBox12.Update();
                    textBox13.Update();
                    textBox14.Update();
                    textBox15.Update();
                    textBox16.Update();
                    textBox17.Update();
                    textBox27.Update();
                    textBox29.Update();
                    textBox30.Update();
                    textBox31.Update();

                    button2.Update();
                    button8.Update();




                    MessageBox.Show(message, "موفقیت", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);


                }
                catch (Exception e1)
                {
                    con.Close();




                    MessageBox.Show("خطا در ثبت درخواست!", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);

                    textBox1.Text = System.String.Empty;
                    textBox2.Text = System.String.Empty;
                    textBox3.Text = System.String.Empty;
                    textBox4.Text = System.String.Empty;
                    textBox5.Text = System.String.Empty;
                    textBox6.Text = System.String.Empty;
                    textBox7.Text = System.String.Empty;
                    textBox8.Text = System.String.Empty;
                    textBox9.Text = System.String.Empty;
                    textBox10.Text = System.String.Empty;
                    textBox11.Text = System.String.Empty;
                    textBox12.Text = System.String.Empty;
                    textBox13.Text = System.String.Empty;
                    textBox14.Text = System.String.Empty;
                    textBox15.Text = System.String.Empty;
                    textBox16.Text = System.String.Empty;
                    textBox17.Text = System.String.Empty;
                    textBox27.Text = System.String.Empty;
                    textBox29.Text = System.String.Empty;
                    textBox30.Text = System.String.Empty;
                    textBox31.Text = System.String.Empty;
                    flowLayoutPanel1.Controls.Clear();
                    pictureAddresses.Clear();




                    textBox1.ReadOnly = false;
                    textBox2.ReadOnly = false;
                    textBox3.ReadOnly = false;
                    textBox4.ReadOnly = false;
                    textBox5.ReadOnly = false;
                    textBox6.ReadOnly = false;
                    textBox7.ReadOnly = false;
                    textBox8.ReadOnly = false;
                    textBox9.ReadOnly = false;
                    textBox10.ReadOnly = false;
                    textBox11.ReadOnly = false;
                    textBox12.ReadOnly = false;
                    textBox13.ReadOnly = false;
                    textBox14.ReadOnly = false;
                    textBox15.ReadOnly = false;
                    textBox16.ReadOnly = false;
                    textBox17.ReadOnly = false;
                    textBox27.ReadOnly = false;
                    textBox29.ReadOnly = false;
                    textBox30.ReadOnly = false;
                    textBox31.ReadOnly = false;

                    button2.Enabled = true;
                    button8.Enabled = true;




                    throw e1;

                }



            }

            textBox1.Text = System.String.Empty;
            textBox2.Text = System.String.Empty;
            textBox3.Text = System.String.Empty;
            textBox4.Text = System.String.Empty;
            textBox5.Text = System.String.Empty;
            textBox6.Text = System.String.Empty;
            textBox7.Text = System.String.Empty;
            textBox8.Text = System.String.Empty;
            textBox9.Text = System.String.Empty;
            textBox10.Text = System.String.Empty;
            textBox11.Text = System.String.Empty;
            textBox12.Text = System.String.Empty;
            textBox13.Text = System.String.Empty;
            textBox14.Text = System.String.Empty;
            textBox15.Text = System.String.Empty;
            textBox16.Text = System.String.Empty;
            textBox17.Text = System.String.Empty;
            textBox27.Text = System.String.Empty;
            textBox29.Text = System.String.Empty;
            textBox30.Text = System.String.Empty;
            textBox31.Text = System.String.Empty;

            flowLayoutPanel1.Controls.Clear();
            pictureAddresses.Clear();



            textBox1.ReadOnly = false;
            textBox2.ReadOnly = false;
            textBox3.ReadOnly = false;
            textBox4.ReadOnly = false;
            textBox5.ReadOnly = false;
            textBox6.ReadOnly = false;
            textBox7.ReadOnly = false;
            textBox8.ReadOnly = false;
            textBox9.ReadOnly = false;
            textBox10.ReadOnly = false;
            textBox11.ReadOnly = false;
            textBox12.ReadOnly = false;
            textBox13.ReadOnly = false;
            textBox14.ReadOnly = false;
            textBox15.ReadOnly = false;
            textBox16.ReadOnly = false;
            textBox17.ReadOnly = false;
            textBox27.ReadOnly = false;
            textBox29.ReadOnly = false;
            textBox30.ReadOnly = false;
            textBox31.ReadOnly = false;

            button2.Enabled = true;
            button8.Enabled = true;


        }

        private void jostojoo()
        {

            con = new SqlCeConnection("Data Source=|DataDirectory|\\SystemeDarkhast.sdf");
            con.Open();

            whereClause = " WHERE (";
            bool appendAnd = false;

            if (textBox18.Text != System.String.Empty)
            {
                whereClause += "NameDarkhast=\'" + textBox18.Text + "\' ";
                appendAnd = true;
            }

            if (textBox28.Text != System.String.Empty)
            {
                if (appendAnd == true)
                    whereClause += "AND ";

                whereClause += ("NameEkhtesari=\'" + textBox28.Text + "\' ");
                appendAnd = true;
            }


            if (textBox19.Text != System.String.Empty)
            {
                if (appendAnd == true)
                    whereClause += "AND ";

                whereClause += ("ShomarehTaghaza=\'" + textBox19.Text + "\' ");
                appendAnd = true;
            }

            if (textBox20.Text != System.String.Empty)
            {
                if (appendAnd == true)
                    whereClause += "AND ";

                whereClause += ("ShomarehTarh=\'" + textBox20.Text + "\' ");
                appendAnd = true;
            }

            if (textBox21.Text != System.String.Empty)
            {
                if (appendAnd == true)
                    whereClause += "AND ";

                whereClause += ("ShomarehTabaghebandi_MESC=\'" + textBox21.Text + "\' ");
                appendAnd = true;
            }

            if (textBox22.Text != System.String.Empty)
            {
                if (appendAnd == true)
                    whereClause += "AND ";

                whereClause += ("PartNumber=\'" + textBox22.Text + "\' ");
                appendAnd = true;
            }

            if (textBox23.Text != System.String.Empty)
            {
                if (appendAnd == true)
                    whereClause += "AND ";

                whereClause += ("IranKode=\'" + textBox23.Text + "\' ");
                appendAnd = true;
            }

            if (textBox24.Text != System.String.Empty)
            {
                if (appendAnd == true)
                    whereClause += "AND ";

                whereClause += ("Size=\'" + textBox24.Text + "\' ");
                appendAnd = true;
            }

            if (whereClause == " WHERE (")
                whereClause = System.String.Empty;

            else
                whereClause += ");";

            string cmdText = "SELECT * FROM Darkhast" + whereClause;
            adapter = new SqlCeDataAdapter(cmdText, con);
            set = new DataSet();
            adapter.Fill(set);
            con.Close();

            if (set.Tables[0].Rows.Count != 0)
            {


                dataGridView1.DataSource = set;
                dataGridView1.DataMember = set.Tables[0].TableName;
                dataGridView1.AutoGenerateColumns = true;
                dataGridView1.Columns[0].HeaderText = "نام/شرکت";
                dataGridView1.Columns[0].DisplayIndex = 1;
                dataGridView1.Columns[1].HeaderText = "شماره تقاضا";
                dataGridView1.Columns[1].DisplayIndex = 3;
                dataGridView1.Columns[2].HeaderText = "شماره طرح";
                dataGridView1.Columns[2].DisplayIndex = 4;
                dataGridView1.Columns[3].HeaderText = "شماره طبقه بندی (MESC)";
                dataGridView1.Columns[3].DisplayIndex = 5;
                dataGridView1.Columns[4].HeaderText = "پارت نامبر";
                dataGridView1.Columns[4].DisplayIndex = 6;
                dataGridView1.Columns[5].HeaderText = "ایران کد";
                dataGridView1.Columns[5].DisplayIndex = 7;
                dataGridView1.Columns[6].HeaderText = "سایز";
                dataGridView1.Columns[6].DisplayIndex = 8;
                dataGridView1.Columns[7].HeaderText = "نام قطعه";
                dataGridView1.Columns[7].DisplayIndex = 9;
                dataGridView1.Columns[8].HeaderText = "متریال قطعه";
                dataGridView1.Columns[8].DisplayIndex = 10;
                dataGridView1.Columns[9].HeaderText = "رنگ";
                dataGridView1.Columns[9].DisplayIndex = 11;
                dataGridView1.Columns[10].HeaderText = "سختی";
                dataGridView1.Columns[10].DisplayIndex = 12;
                dataGridView1.Columns[11].HeaderText = "تعداد";
                dataGridView1.Columns[11].DisplayIndex = 13;
                dataGridView1.Columns[12].HeaderText = "تاریخ تولید";
                dataGridView1.Columns[12].DisplayIndex = 14;
                dataGridView1.Columns[13].HeaderText = "تولیدکننده";
                dataGridView1.Columns[13].DisplayIndex = 15;
                dataGridView1.Columns[14].HeaderText = "کد قالب";
                dataGridView1.Columns[14].DisplayIndex = 16;
                dataGridView1.Columns[15].HeaderText = "قیمت خرید";
                dataGridView1.Columns[15].DisplayIndex = 17;
                dataGridView1.Columns[16].HeaderText = "کد نقشه";
                dataGridView1.Columns[16].DisplayIndex = 21;
                dataGridView1.Columns[17].HeaderText = "شماره داخلی";
                dataGridView1.Columns[17].DisplayIndex = 0;
                dataGridView1.Columns[18].HeaderText = "نام اختصاری";
                dataGridView1.Columns[18].DisplayIndex = 2;
                dataGridView1.Columns[19].HeaderText = "قیمت فروش";
                dataGridView1.Columns[19].DisplayIndex = 18;
                dataGridView1.Columns[20].HeaderText = "اعتبار قیمت";
                dataGridView1.Columns[20].DisplayIndex = 19;
                dataGridView1.Columns[21].HeaderText = "استعلام بها";
                dataGridView1.Columns[21].DisplayIndex = 20;
            }

            else
            {
                dataGridView1.DataMember = null;
                dataGridView1.DataSource = null;
                MessageBox.Show("موردی یافت نشد!", "جستجو", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
            }



        }

        private void button4_Click(object sender, EventArgs e)
        {

            jostojoo();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            textBox1.Text = System.String.Empty;
            textBox2.Text = System.String.Empty;
            textBox3.Text = System.String.Empty;
            textBox4.Text = System.String.Empty;
            textBox5.Text = System.String.Empty;
            textBox6.Text = System.String.Empty;
            textBox7.Text = System.String.Empty;
            textBox8.Text = System.String.Empty;
            textBox9.Text = System.String.Empty;
            textBox10.Text = System.String.Empty;
            textBox11.Text = System.String.Empty;
            textBox12.Text = System.String.Empty;
            textBox13.Text = System.String.Empty;
            textBox14.Text = System.String.Empty;
            textBox15.Text = System.String.Empty;
            textBox16.Text = System.String.Empty;
            textBox17.Text = System.String.Empty;
            textBox27.Text = System.String.Empty;
            textBox29.Text = System.String.Empty;
            textBox30.Text = System.String.Empty;
            textBox31.Text = System.String.Empty;
            flowLayoutPanel1.Controls.Clear();
            pictureAddresses.Clear();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox18.Text = System.String.Empty;
            textBox19.Text = System.String.Empty;
            textBox20.Text = System.String.Empty;
            textBox21.Text = System.String.Empty;
            textBox22.Text = System.String.Empty;
            textBox23.Text = System.String.Empty;
            textBox24.Text = System.String.Empty;
            textBox28.Text = System.String.Empty;
        }


        private void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            float rightMargin = ev.MarginBounds.Right;
            float topMargin = ev.MarginBounds.Top;
            StringFormat stringFormatting = new StringFormat(StringFormatFlags.DirectionRightToLeft);

            stringFormatting.Alignment = System.Drawing.StringAlignment.Near;

            ev.Graphics.DrawString("مشخصات درخواست", Mitra, Brushes.Black, rightMargin, 0, stringFormatting);
            ev.Graphics.DrawString(printStream, Mitra, Brushes.Black, rightMargin, 70, stringFormatting);
        }


        private void button6_Click(object sender, EventArgs e)
        {


        }

        private void tabControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                tabControl1.SelectTab(0);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F3)
            {
                tabControl1.SelectTab(1);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F4)
            {
                tabControl1.SelectTab(2);
                e.Handled = true;
            }

        }



        private void populateAutoCompleteStringCollections()
        {

            AutoCompleteStringCollection[] autoComplete = new AutoCompleteStringCollection[21 + 8 + 1];

            for (int i = autoComplete.Length; i > 0; i--)
                autoComplete[i - 1] = new AutoCompleteStringCollection();


            con = new SqlCeConnection("Data Source=|DataDirectory|\\SystemeDarkhast.sdf");
            con.Open();
            string cmdText = "SELECT * FROM Darkhast";
            adapter = new SqlCeDataAdapter(cmdText, con);
            set = new DataSet();
            adapter.Fill(set);
            con.Close();

            for (int i = set.Tables[0].Rows.Count; i > 0; i--)
                for (int j = 0; j < set.Tables[0].Columns.Count; j++)
                    autoComplete[j].Add(set.Tables[0].Rows[i - 1][j].ToString());

            textBox1.AutoCompleteCustomSource = autoComplete[0];
            textBox2.AutoCompleteCustomSource = autoComplete[1];
            textBox3.AutoCompleteCustomSource = autoComplete[2];
            textBox4.AutoCompleteCustomSource = autoComplete[3];
            textBox5.AutoCompleteCustomSource = autoComplete[4];
            textBox6.AutoCompleteCustomSource = autoComplete[5];
            textBox7.AutoCompleteCustomSource = autoComplete[6];
            textBox8.AutoCompleteCustomSource = autoComplete[7];
            textBox9.AutoCompleteCustomSource = autoComplete[8];
            textBox10.AutoCompleteCustomSource = autoComplete[9];
            textBox11.AutoCompleteCustomSource = autoComplete[10];
            textBox12.AutoCompleteCustomSource = autoComplete[11];
            textBox13.AutoCompleteCustomSource = autoComplete[12];
            textBox14.AutoCompleteCustomSource = autoComplete[13];
            textBox15.AutoCompleteCustomSource = autoComplete[14];
            textBox16.AutoCompleteCustomSource = autoComplete[15];
            textBox17.AutoCompleteCustomSource = autoComplete[16];
            textBox27.AutoCompleteCustomSource = autoComplete[18];
            textBox29.AutoCompleteCustomSource = autoComplete[19];
            textBox30.AutoCompleteCustomSource = autoComplete[20];
            textBox31.AutoCompleteCustomSource = autoComplete[21];

            textBox18.AutoCompleteCustomSource = autoComplete[0];
            textBox28.AutoCompleteCustomSource = autoComplete[18];
            textBox19.AutoCompleteCustomSource = autoComplete[1];
            textBox20.AutoCompleteCustomSource = autoComplete[2];
            textBox21.AutoCompleteCustomSource = autoComplete[3];
            textBox22.AutoCompleteCustomSource = autoComplete[4];
            textBox23.AutoCompleteCustomSource = autoComplete[5];
            textBox24.AutoCompleteCustomSource = autoComplete[6];

        }

        private void printDocument1_BeginPrint(object sender, PrintEventArgs e)
        {
            try
            {
                strFormat = new StringFormat(StringFormatFlags.DirectionRightToLeft);
                strFormat.Alignment = StringAlignment.Center;
                strFormat.LineAlignment = StringAlignment.Center;
                strFormat.Trimming = StringTrimming.EllipsisCharacter;

                arrColumnLefts.Clear();
                arrColumnWidths.Clear();
                iCellHeight = 0;
                iRow = 0;
                bFirstPage = true;
                bNewPage = true;

                // Calculating Total Widths
                iTotalWidth = 0;
                
                foreach (DataGridViewColumn dgvGridCol in printGridView.Columns)
                {
                    iTotalWidth += dgvGridCol.Width;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (printType == 0)
            {
                try
                {
                    //Set the left margin
                    int iLeftMargin = e.MarginBounds.Left;
                    //Set the top margin
                    int iTopMargin = e.MarginBounds.Top;
                    //Whether more pages have to print or not
                    bool bMorePagesToPrint = false;
                    int iTmpWidth = 0;

                    //For the first page to print set the cell width and header height
                    if (bFirstPage)
                    {
                        foreach (DataGridViewColumn GridCol in printGridView.Columns)
                        {
                            iTmpWidth = (int)(Math.Floor((double)((double)GridCol.Width /
                                           (double)iTotalWidth * (double)iTotalWidth *
                                           ((double)(e.MarginBounds.Width) / (double)iTotalWidth))));

                            iHeaderHeight = (int)(e.Graphics.MeasureString(GridCol.HeaderText,
                                        GridCol.InheritedStyle.Font, iTmpWidth).Height) + 11;

                            // Save width and height of headres
                            arrColumnLefts.Add(iLeftMargin);
                            arrColumnWidths.Add(iTmpWidth);
                            iLeftMargin += iTmpWidth;
                        }
                    }
                    //Loop till all the grid rows not get printed
                    while (iRow <= printGridView.Rows.Count - 1)
                    {
                        DataGridViewRow GridRow = printGridView.Rows[iRow];
                        //Set the cell height
                        iCellHeight = GridRow.Height + 5;
                        int iCount = 0;
                        //Check whether the current page settings allo more rows to print
                        if (iTopMargin + iCellHeight >= e.MarginBounds.Height + e.MarginBounds.Top)
                        {
                            bNewPage = true;
                            bFirstPage = false;
                            bMorePagesToPrint = true;
                            break;
                        }
                        else
                        {
                            if (bNewPage)
                            {
                                //Draw Logo
                                try
                                {
                                    Image headerLogo = Properties.Resources.Logo;

                                    e.Graphics.DrawImage(headerLogo, e.MarginBounds.Left + (((e.MarginBounds.Width) / 2) -
                                    (headerLogo.Width / 2)), e.MarginBounds.Top - headerLogo.Height - 13);



                                }
                                catch (Exception)
                                {

                                }



                                //Draw Text
                                e.Graphics.DrawString(headerString1, MitraBd,
                                        Brushes.Black, e.MarginBounds.Left, e.MarginBounds.Top -
                                        e.Graphics.MeasureString("Customer Summary", new Font(printGridView.Font,
                                        FontStyle.Bold), (e.MarginBounds.Width)).Height - 50);


                                //Draw Date
                                e.Graphics.DrawString(headerString2, MitraBd,
                                        Brushes.Black, e.MarginBounds.Left + ((e.MarginBounds.Width) - 25 -
                                        e.Graphics.MeasureString(headerString2, new Font(printGridView.Font,
                                        FontStyle.Bold), (e.MarginBounds.Width)).Width) - 10, e.MarginBounds.Top -
                                        e.Graphics.MeasureString("Customer Summary", new Font(new Font(printGridView.Font,
                                        FontStyle.Bold), FontStyle.Bold), (e.MarginBounds.Width)).Height - 50);

                                //Draw Columns                 
                                iTopMargin = e.MarginBounds.Top;
                                foreach (DataGridViewColumn GridCol in printGridView.Columns)
                                {
                                    e.Graphics.FillRectangle(new SolidBrush(Color.LightGray),
                                        new Rectangle((int)arrColumnLefts[iCount], iTopMargin,
                                        (int)arrColumnWidths[iCount], iHeaderHeight));

                                    e.Graphics.DrawRectangle(Pens.Black,
                                        new Rectangle((int)arrColumnLefts[iCount], iTopMargin,
                                        (int)arrColumnWidths[iCount], iHeaderHeight));

                                    e.Graphics.DrawString(GridCol.HeaderText, Mitra8,
                                        new SolidBrush(GridCol.InheritedStyle.ForeColor),
                                        new RectangleF((int)arrColumnLefts[iCount], iTopMargin,
                                        (int)arrColumnWidths[iCount], iHeaderHeight), strFormat);
                                    iCount++;
                                }
                                bNewPage = false;
                                iTopMargin += iHeaderHeight;
                            }
                            iCount = 0;
                            //Draw Columns Contents                
                            foreach (DataGridViewCell Cel in GridRow.Cells)
                            {
                                if (Cel.Value != null)
                                {
                                    e.Graphics.DrawString(Cel.Value.ToString(), Mitra8,
                                                new SolidBrush(Cel.InheritedStyle.ForeColor),
                                                new RectangleF((int)arrColumnLefts[iCount], (float)iTopMargin,
                                                (int)arrColumnWidths[iCount], (float)iCellHeight), strFormat);
                                }
                                //Drawing Cells Borders 
                                e.Graphics.DrawRectangle(Pens.Black, new Rectangle((int)arrColumnLefts[iCount],
                                        iTopMargin, (int)arrColumnWidths[iCount], iCellHeight));

                                iCount++;
                            }
                        }
                        iRow++;
                        iTopMargin += iCellHeight;
                    }

                    //If more lines exist, print another page.
                    if (bMorePagesToPrint)
                        e.HasMorePages = true;
                    else
                        e.HasMorePages = false;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }

            else if (printType == 1)
            {
                try
                {
                    //Set the left margin
                    int iLeftMargin = e.MarginBounds.Left;
                    //Set the top margin
                    int iTopMargin = e.MarginBounds.Top;
                    //Whether more pages have to print or not
                    bool bMorePagesToPrint = false;
                    int iTmpWidth = 0;

                    //For the first page to print set the cell width and header height
                    if (bFirstPage)
                    {
                        foreach (DataGridViewColumn GridCol in printGridView.Columns)
                        {
                            iTmpWidth = (int)(Math.Floor((double)((double)GridCol.Width /
                                           (double)iTotalWidth * (double)iTotalWidth *
                                           ((double)(e.MarginBounds.Width) / (double)iTotalWidth))));

                            iHeaderHeight = (int)(e.Graphics.MeasureString(GridCol.HeaderText,
                                        GridCol.InheritedStyle.Font, iTmpWidth).Height) + 11;

                            // Save width and height of headres
                            arrColumnLefts.Add(iLeftMargin);
                            arrColumnWidths.Add(iTmpWidth);
                            iLeftMargin += iTmpWidth;
                        }
                    }
                    //Loop till all the grid rows not get printed
                    while (iRow <= printGridView.Rows.Count - 1)
                    {
                        DataGridViewRow GridRow = printGridView.Rows[iRow];
                        //Set the cell height
                        iCellHeight = GridRow.Height + 5;
                        int iCount = 0;
                        //Check whether the current page settings allo more rows to print
                        if (iTopMargin + iCellHeight >= e.MarginBounds.Height + e.MarginBounds.Top)
                        {
                            bNewPage = true;
                            bFirstPage = false;
                            bMorePagesToPrint = true;
                            break;
                        }
                        else
                        {
                            if (bNewPage)
                            {
                                //Draw Logo
                                try
                                {
                                    Image headerLogo = Properties.Resources.Logo;

                                    e.Graphics.DrawImage(headerLogo, e.MarginBounds.Left + (((e.MarginBounds.Width) / 2) -
                                    (headerLogo.Width / 2)), e.MarginBounds.Top - headerLogo.Height - 13);



                                }
                                catch (Exception)
                                {

                                }



                                //Draw Text
                                e.Graphics.DrawString(headerString1, MitraBd,
                                        Brushes.Black, e.MarginBounds.Left, e.MarginBounds.Top -
                                        e.Graphics.MeasureString("Customer Summary", new Font(printGridView.Font,
                                        FontStyle.Bold), (e.MarginBounds.Width)).Height - 50);


                                //Draw Date
                                e.Graphics.DrawString(headerString2, MitraBd,
                                        Brushes.Black, e.MarginBounds.Left + ((e.MarginBounds.Width) - 25 -
                                        e.Graphics.MeasureString(headerString2, new Font(printGridView.Font,
                                        FontStyle.Bold), (e.MarginBounds.Width)).Width) - 10, e.MarginBounds.Top -
                                        e.Graphics.MeasureString("Customer Summary", new Font(new Font(printGridView.Font,
                                        FontStyle.Bold), FontStyle.Bold), (e.MarginBounds.Width)).Height - 50);

                                //Draw Columns                 
                                iTopMargin = e.MarginBounds.Top;
                                foreach (DataGridViewColumn GridCol in printGridView.Columns)
                                {
                                    e.Graphics.FillRectangle(new SolidBrush(Color.LightGray),
                                        new Rectangle((int)arrColumnLefts[iCount], iTopMargin,
                                        (int)arrColumnWidths[iCount], iHeaderHeight));

                                    e.Graphics.DrawRectangle(Pens.Black,
                                        new Rectangle((int)arrColumnLefts[iCount], iTopMargin,
                                        (int)arrColumnWidths[iCount], iHeaderHeight));

                                    e.Graphics.DrawString(GridCol.HeaderText, MitraBd,
                                        new SolidBrush(GridCol.InheritedStyle.ForeColor),
                                        new RectangleF((int)arrColumnLefts[iCount], iTopMargin,
                                        (int)arrColumnWidths[iCount], iHeaderHeight), strFormat);
                                    iCount++;
                                }
                                bNewPage = false;
                                iTopMargin += iHeaderHeight;
                            }
                            iCount = 0;
                            //Draw Columns Contents                
                            foreach (DataGridViewCell Cel in GridRow.Cells)
                            {
                                if (Cel.Value != null)
                                {
                                    e.Graphics.DrawString(Cel.Value.ToString(), MitraBd,
                                                new SolidBrush(Cel.InheritedStyle.ForeColor),
                                                new RectangleF((int)arrColumnLefts[iCount], (float)iTopMargin,
                                                (int)arrColumnWidths[iCount], (float)iCellHeight), strFormat);
                                }
                                //Drawing Cells Borders 
                                e.Graphics.DrawRectangle(Pens.Black, new Rectangle((int)arrColumnLefts[iCount],
                                        iTopMargin, (int)arrColumnWidths[iCount], iCellHeight));

                                iCount++;
                            }
                        }
                        iRow++;
                        iTopMargin += iCellHeight;
                    }

                    //If more lines exist, print another page.
                    if (bMorePagesToPrint)
                        e.HasMorePages = true;
                    else
                        e.HasMorePages = false;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                


            }

            else if (printType == 2)
            {
                try
                {
                    //Set the left margin
                    int iLeftMargin = e.MarginBounds.Left;
                    //Set the top margin
                    int iTopMargin = e.MarginBounds.Top;
                    //Whether more pages have to print or not
                    bool bMorePagesToPrint = false;
                    int iTmpWidth = 0;

                    //For the first page to print set the cell width and header height
                    if (bFirstPage)
                    {
                        foreach (DataGridViewColumn GridCol in printGridView.Columns)
                        {
                            iTmpWidth = (int)(Math.Floor((double)((double)GridCol.Width /
                                           (double)iTotalWidth * (double)iTotalWidth *
                                           ((double)(e.MarginBounds.Width) / (double)iTotalWidth))));

                            iHeaderHeight = (int)(e.Graphics.MeasureString(GridCol.HeaderText,
                                        GridCol.InheritedStyle.Font, iTmpWidth).Height) + 11;

                            // Save width and height of headres
                            arrColumnLefts.Add(iLeftMargin);
                            arrColumnWidths.Add(iTmpWidth);
                            iLeftMargin += iTmpWidth;
                        }
                    }
                    //Loop till all the grid rows not get printed
                    while (iRow <= printGridView.Rows.Count - 1)
                    {
                        DataGridViewRow GridRow = printGridView.Rows[iRow];
                        //Set the cell height
                        iCellHeight = GridRow.Height + 5;
                        int iCount = 0;
                        //Check whether the current page settings allo more rows to print
                        if (iTopMargin + iCellHeight >= e.MarginBounds.Height + e.MarginBounds.Top)
                        {
                            bNewPage = true;
                            bFirstPage = false;
                            bMorePagesToPrint = true;
                            break;
                        }
                        else
                        {
                            if (bNewPage)
                            {
                                //Draw Logo
                                try
                                {
                                    Image headerLogo = Properties.Resources.Logo;

                                    e.Graphics.DrawImage(headerLogo, e.MarginBounds.Left + (((e.MarginBounds.Width) / 2) -
                                    (headerLogo.Width / 2)), e.MarginBounds.Top - headerLogo.Height - 13);



                                }
                                catch (Exception)
                                {

                                }



                                //Draw Text
                                e.Graphics.DrawString(headerString1, MitraBd,
                                        Brushes.Black, e.MarginBounds.Left, e.MarginBounds.Top -
                                        e.Graphics.MeasureString("Customer Summary", new Font(printGridView.Font,
                                        FontStyle.Bold), (e.MarginBounds.Width)).Height - 50);


                                //Draw Date
                                e.Graphics.DrawString(headerString2, new Font("Arial", 12, FontStyle.Bold),
                                        Brushes.Black, e.MarginBounds.Left + ((e.MarginBounds.Width) - 25 -
                                        e.Graphics.MeasureString(headerString2, new Font(printGridView.Font,
                                        FontStyle.Bold), (e.MarginBounds.Width)).Width) - 10, e.MarginBounds.Top -
                                        e.Graphics.MeasureString("Customer Summary", new Font(new Font(printGridView.Font,
                                        FontStyle.Bold), FontStyle.Bold), (e.MarginBounds.Width)).Height - 50);

                                //Draw Columns                 
                                iTopMargin = e.MarginBounds.Top;
                                foreach (DataGridViewColumn GridCol in printGridView.Columns)
                                {
                                    e.Graphics.FillRectangle(new SolidBrush(Color.LightGray),
                                        new Rectangle((int)arrColumnLefts[iCount], iTopMargin,
                                        (int)arrColumnWidths[iCount], iHeaderHeight));

                                    e.Graphics.DrawRectangle(Pens.Black,
                                        new Rectangle((int)arrColumnLefts[iCount], iTopMargin,
                                        (int)arrColumnWidths[iCount], iHeaderHeight));

                                    e.Graphics.DrawString(GridCol.HeaderText, new Font("Arial", 12, FontStyle.Bold),
                                        new SolidBrush(GridCol.InheritedStyle.ForeColor),
                                        new RectangleF((int)arrColumnLefts[iCount], iTopMargin,
                                        (int)arrColumnWidths[iCount], iHeaderHeight), strFormat);
                                    iCount++;
                                }
                                bNewPage = false;
                                iTopMargin += iHeaderHeight;
                            }
                            iCount = 0;
                            //Draw Columns Contents                
                            foreach (DataGridViewCell Cel in GridRow.Cells)
                            {
                                if (Cel.Value != null)
                                {
                                    e.Graphics.DrawString(Cel.Value.ToString(), new Font("Arial", 12, FontStyle.Bold),
                                                new SolidBrush(Cel.InheritedStyle.ForeColor),
                                                new RectangleF((int)arrColumnLefts[iCount], (float)iTopMargin,
                                                (int)arrColumnWidths[iCount], (float)iCellHeight), strFormat);
                                }
                                //Drawing Cells Borders 
                                e.Graphics.DrawRectangle(Pens.Black, new Rectangle((int)arrColumnLefts[iCount],
                                        iTopMargin, (int)arrColumnWidths[iCount], iCellHeight));

                                iCount++;
                            }
                        }
                        iRow++;
                        iTopMargin += iCellHeight;
                    }

                    //If more lines exist, print another page.
                    if (bMorePagesToPrint)
                        e.HasMorePages = true;
                    else
                        e.HasMorePages = false;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }




        }

        private void button7_Click(object sender, EventArgs e)
        {
            printType = 0;

            headerString2 = "مشخصات درخواست ها";
            headerString1 = "";


            printGridView = new DataGridView();
            
            printCon = new SqlCeConnection("Data Source=|DataDirectory|\\SystemeDarkhast.sdf");
            printCon.Open();
            string cmdText = "SELECT KodeNaghshe, EstelameBaha, EtebareGheymat, GheymateForoush, Gheymat, KodeGhaleb, TolidKonandeh, TarikheTolid, Tedad, Sakhti, Rang, MaterialeGhete, NameGhete, Size, IranKode, PartNumber, ShomarehTabaghebandi_MESC, ShomarehTarh, ShomarehTaghaza, NameEkhtesari, NameDarkhast, InternalID FROM Darkhast" + whereClause;
            printAdapter = new SqlCeDataAdapter(cmdText, con);
            printSet = new DataSet();
            printAdapter.Fill(printSet);
            printCon.Close();

            dataGridView2.AutoGenerateColumns = true;
            dataGridView2.DataMember = null;
            dataGridView2.DataSource = printSet;
            dataGridView2.DataMember = printSet.Tables[0].TableName;

            dataGridView2.Columns[0].HeaderText = "کد نقشه";
            dataGridView2.Columns[1].HeaderText = "استعلام بها";
            dataGridView2.Columns[2].HeaderText = "اعتبار قیمت";
            dataGridView2.Columns[3].HeaderText = "قیمت فروش";
            dataGridView2.Columns[4].HeaderText = "قیمت خرید";
            dataGridView2.Columns[5].HeaderText = "کد قالب";
            dataGridView2.Columns[6].HeaderText = "تولیدکننده";
            dataGridView2.Columns[7].HeaderText = "تاریخ تولید";
            dataGridView2.Columns[8].HeaderText = "تعداد";
            dataGridView2.Columns[9].HeaderText = "سختی";
            dataGridView2.Columns[10].HeaderText = "رنگ";
            dataGridView2.Columns[11].HeaderText = "متریال قطعه";
            dataGridView2.Columns[12].HeaderText = "نام قطعه";
            dataGridView2.Columns[13].HeaderText = "سایز";
            dataGridView2.Columns[14].HeaderText = "ایران کد";
            dataGridView2.Columns[15].HeaderText = "پارت نامبر";
            dataGridView2.Columns[16].HeaderText = "شماره طبقه بندی (MESC)";
            dataGridView2.Columns[17].HeaderText = "شماره طرح";
            dataGridView2.Columns[18].HeaderText = "شماره تقاضا";
            dataGridView2.Columns[19].HeaderText = "نام اختصاری";
            dataGridView2.Columns[20].HeaderText = "نام/شرکت";
            dataGridView2.Columns[21].HeaderText = "شماره داخلی";

            printGridView = dataGridView2;

            //Open the print dialog
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDocument1;
            printDialog.UseEXDialog = true;

            printDocument1.DefaultPageSettings.Margins.Left = 30;
            printDocument1.DefaultPageSettings.Margins.Right = 70;
            printDocument1.DefaultPageSettings.Margins.Top = 80;
            printDocument1.DefaultPageSettings.Margins.Bottom = 40;



            //Get the document
            if (DialogResult.OK == printDialog.ShowDialog())
            {
                printDocument1.DocumentName = "Page";
                printDocument1.Print();
            }

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                this.ActiveControl = textBox1;
                this.AcceptButton = button1;
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                this.ActiveControl = textBox18;
                this.AcceptButton = button4;
            }
            else if (tabControl1.SelectedIndex == 2)
            {
                if (textBox50.ReadOnly == true)
                {
                    this.ActiveControl = textBox51;
                    this.AcceptButton = button13;
                }
                else
                {
                    this.ActiveControl = textBox50;
                    this.AcceptButton = button12;
                }
                    
            }
            else
                this.AcceptButton = null;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            DialogResult dr = this.openFileDialog1.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                // Read the files
                foreach (String file in openFileDialog1.FileNames)
                {
                    // Create a PictureBox.
                    try
                    {
                        PictureBox pb = new PictureBox();
                        pictureAddresses.Add(file);
                        Image loadedImage = Image.FromFile(file);
                        pb.Height = 95;
                        pb.Width = 95;
                        pb.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
                        pb.Image = loadedImage;
                        flowLayoutPanel1.Controls.Add(pb);
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
            }
            

        }

        private void button9_Click(object sender, EventArgs e)
        {
            printType = 1;

            headerString2 = "مشخصات درخواست";
            headerString1 = "";

            printGridView = new DataGridView();
            
            DataGridViewSelectedRowCollection currentRow = dataGridView1.SelectedRows;

            if (currentRow.Count != 0)
            {
                string internalID = currentRow[0].Cells[17].Value.ToString();

                printCon = new SqlCeConnection("Data Source=|DataDirectory|\\SystemeDarkhast.sdf");
                printCon.Open();
                string cmdText = "SELECT * FROM Darkhast WHERE InternalID=(" + internalID + ");";
                printAdapter = new SqlCeDataAdapter(cmdText, con);
                tmpSet = new DataSet();
                printAdapter.Fill(tmpSet);
                printCon.Close();

                printSet = new DataSet();
                printTable = new DataTable();

                
                printColumn = new DataColumn();
                printColumn.DataType = System.Type.GetType("System.String");
                printColumn.ColumnName = "مقدار";
                printTable.Columns.Add(printColumn);

                printColumn = new DataColumn();
                printColumn.DataType = System.Type.GetType("System.String");
                printColumn.ColumnName = "مورد";
                printTable.Columns.Add(printColumn);

                printRow = printTable.NewRow();
                printRow["مورد"] = "نام/شرکت"; printRow["مقدار"] = tmpSet.Tables[0].Rows[0][0].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["مورد"] = "نام اختصاری"; printRow["مقدار"] = tmpSet.Tables[0].Rows[0][18].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["مورد"] = "شماره تقاضا"; printRow["مقدار"] = tmpSet.Tables[0].Rows[0][1].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["مورد"] = "شماره طرح"; printRow["مقدار"] = tmpSet.Tables[0].Rows[0][2].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["مورد"] = "MESC (شماره طبقه بندی)"; printRow["مقدار"] = tmpSet.Tables[0].Rows[0][3].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["مورد"] = "پارت نامبر"; printRow["مقدار"] = tmpSet.Tables[0].Rows[0][4].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["مورد"] = "ایران کد"; printRow["مقدار"] = tmpSet.Tables[0].Rows[0][5].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["مورد"] = "سایز"; printRow["مقدار"] = tmpSet.Tables[0].Rows[0][6].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["مورد"] = "نام قطعه"; printRow["مقدار"] = tmpSet.Tables[0].Rows[0][7].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["مورد"] = "متریال قطعه"; printRow["مقدار"] = tmpSet.Tables[0].Rows[0][8].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["مورد"] = "رنگ"; printRow["مقدار"] = tmpSet.Tables[0].Rows[0][9].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["مورد"] = "سختی"; printRow["مقدار"] = tmpSet.Tables[0].Rows[0][10].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["مورد"] = "تعداد"; printRow["مقدار"] = tmpSet.Tables[0].Rows[0][11].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["مورد"] = "تاریخ تولید"; printRow["مقدار"] = tmpSet.Tables[0].Rows[0][12].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["مورد"] = "تولید کننده"; printRow["مقدار"] = tmpSet.Tables[0].Rows[0][13].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["مورد"] = "کد قالب"; printRow["مقدار"] = tmpSet.Tables[0].Rows[0][14].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["مورد"] = "قیمت خرید"; printRow["مقدار"] = tmpSet.Tables[0].Rows[0][15].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["مورد"] = "کد نقشه"; printRow["مقدار"] = tmpSet.Tables[0].Rows[0][16].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["مورد"] = "قیمت فروش"; printRow["مقدار"] = tmpSet.Tables[0].Rows[0][19].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["مورد"] = "اعتبار قیمت"; printRow["مقدار"] = tmpSet.Tables[0].Rows[0][20].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["مورد"] = "استعلام بها"; printRow["مقدار"] = tmpSet.Tables[0].Rows[0][21].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["مورد"] = "شماره داخلی"; printRow["مقدار"] = tmpSet.Tables[0].Rows[0][17].ToString();
                printTable.Rows.Add(printRow);

                printSet.Tables.Add(printTable);

                dataGridView2.AutoGenerateColumns = true;
                dataGridView2.DataMember = null;
                dataGridView2.DataSource = printSet;
                dataGridView2.DataMember = printSet.Tables[0].TableName;

                printGridView = dataGridView2;

                //Open the print dialog
                PrintDialog printDialog = new PrintDialog();
                printDialog.Document = printDocument1;
                printDialog.UseEXDialog = true;

                printDocument1.DefaultPageSettings.Margins.Left = 30;
                printDocument1.DefaultPageSettings.Margins.Right = 70;
                printDocument1.DefaultPageSettings.Margins.Top = 80;
                printDocument1.DefaultPageSettings.Margins.Bottom = 40;



                //Get the document
                if (DialogResult.OK == printDialog.ShowDialog())
                {
                    printDocument1.DocumentName = "Page";
                    printDocument1.Print();
                }





            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection currentRow = dataGridView1.SelectedRows;

            if (currentRow.Count != 0)
            {
                string internalID = currentRow[0].Cells[17].Value.ToString();

                if (MessageBox.Show("مطمئن هستید؟", "حذف", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading) == DialogResult.Yes)
                {
                    con = new SqlCeConnection("Data Source=|DataDirectory|\\SystemeDarkhast.sdf");
                    con.Open();
                    string cmdText = "SELECT InternalID FROM Darkhast WHERE InternalID=(" + internalID + ");";
                    adapter = new SqlCeDataAdapter(cmdText, con);
                    set = new DataSet();
                    adapter.Fill(set);
                    con.Close();

                    con = new SqlCeConnection("Data Source=|DataDirectory|\\SystemeDarkhast.sdf");
                    con.Open();
                    com = con.CreateCommand();
                    com.Connection = con;
                    com.CommandText = "DELETE FROM Darkhast WHERE InternalID=(" + internalID + ");";

                    try
                    {
                        com.ExecuteNonQuery();

                        if (Directory.Exists(Application.StartupPath + @"\Images\" + internalID + @"\"))
                            Directory.Delete(Application.StartupPath + @"\Images\" + internalID + @"\", true);

                        MessageBox.Show("حذف انجام شد.", "حذف", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                    }
                    catch (Exception e1)
                    {
                        
                        MessageBox.Show("خطا در حین عملیات حذف", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);

                    }

                    con.Close();

                }


                populateAutoCompleteStringCollections();

                jostojoo();

            }
        }

        private void button11_Click(object sender, EventArgs e)
        {

            DataGridViewSelectedRowCollection currentRow = dataGridView1.SelectedRows;
            if (currentRow.Count != 0)
            {
                string internalID = currentRow[0].Cells[17].Value.ToString();

                string picDirPath = Application.StartupPath + @"\Images\" + internalID + @"\";

                if (Directory.Exists(picDirPath))
                {
                    string[] pictureAddresses = Directory.GetFiles(picDirPath);

                    System.Collections.Specialized.StringCollection strCollection = new System.Collections.Specialized.StringCollection();

                    strCollection.AddRange(pictureAddresses);

                    if (strCollection.Count == 0)
                        MessageBox.Show("این آیتم تصویری ندارد.", "موفقیت", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);

                    else
                        new PictureViewerForm(strCollection).ShowDialog();
                }
                else
                    MessageBox.Show("این آیتم تصویری ندارد.", "موفقیت", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);

                System.GC.Collect();


            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void button5_Click(object sender, EventArgs e)
        {

            printType = 2;

            headerString1 = "Request Details";
            headerString2 = "";
            
            printGridView = new DataGridView();

            DataGridViewSelectedRowCollection currentRow = dataGridView1.SelectedRows;

            if (currentRow.Count != 0)
            {
                string internalID = currentRow[0].Cells[17].Value.ToString();

                printCon = new SqlCeConnection("Data Source=|DataDirectory|\\SystemeDarkhast.sdf");
                printCon.Open();
                string cmdText = "SELECT * FROM Darkhast WHERE InternalID=(" + internalID + ");";
                printAdapter = new SqlCeDataAdapter(cmdText, con);
                tmpSet = new DataSet();
                printAdapter.Fill(tmpSet);
                printCon.Close();

                printSet = new DataSet();
                printTable = new DataTable();


                printColumn = new DataColumn();
                printColumn.DataType = System.Type.GetType("System.String");
                printColumn.ColumnName = "Item";
                printTable.Columns.Add(printColumn);

                printColumn = new DataColumn();
                printColumn.DataType = System.Type.GetType("System.String");
                printColumn.ColumnName = "Value";
                printTable.Columns.Add(printColumn);

                printRow = printTable.NewRow();
                printRow["Item"] = "Name/Company"; printRow["Value"] = tmpSet.Tables[0].Rows[0][0].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["Item"] = "Short Name"; printRow["Value"] = tmpSet.Tables[0].Rows[0][18].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["Item"] = "Req. Number"; printRow["Value"] = tmpSet.Tables[0].Rows[0][1].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["Item"] = "Design Number"; printRow["Value"] = tmpSet.Tables[0].Rows[0][2].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["Item"] = "MESC - Classification Number"; printRow["Value"] = tmpSet.Tables[0].Rows[0][3].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["Item"] = "Part Number"; printRow["Value"] = tmpSet.Tables[0].Rows[0][4].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["Item"] = "IranCode"; printRow["Value"] = tmpSet.Tables[0].Rows[0][5].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["Item"] = "Size"; printRow["Value"] = tmpSet.Tables[0].Rows[0][6].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["Item"] = "Part Name"; printRow["Value"] = tmpSet.Tables[0].Rows[0][7].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["Item"] = "Part Material"; printRow["Value"] = tmpSet.Tables[0].Rows[0][8].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["Item"] = "Color"; printRow["Value"] = tmpSet.Tables[0].Rows[0][9].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["Item"] = "Hardness"; printRow["Value"] = tmpSet.Tables[0].Rows[0][10].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["Item"] = "Count"; printRow["Value"] = tmpSet.Tables[0].Rows[0][11].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["Item"] = "Production Date"; printRow["Value"] = tmpSet.Tables[0].Rows[0][12].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["Item"] = "Producer"; printRow["Value"] = tmpSet.Tables[0].Rows[0][13].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["Item"] = "Mold Code"; printRow["Value"] = tmpSet.Tables[0].Rows[0][14].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["Item"] = "Buying Cost"; printRow["Value"] = tmpSet.Tables[0].Rows[0][15].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["Item"] = "Plot Number"; printRow["Value"] = tmpSet.Tables[0].Rows[0][16].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["Item"] = "Selling Price"; printRow["Value"] = tmpSet.Tables[0].Rows[0][19].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["Item"] = "Price Credit"; printRow["Value"] = tmpSet.Tables[0].Rows[0][20].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["Item"] = "Price Inquiry"; printRow["Value"] = tmpSet.Tables[0].Rows[0][21].ToString();
                printTable.Rows.Add(printRow);

                printRow = printTable.NewRow();
                printRow["Item"] = "Internal ID"; printRow["Value"] = tmpSet.Tables[0].Rows[0][17].ToString();
                printTable.Rows.Add(printRow);

                printSet.Tables.Add(printTable);

                dataGridView2.AutoGenerateColumns = true;
                dataGridView2.DataMember = null;
                dataGridView2.DataSource = printSet;
                dataGridView2.DataMember = printSet.Tables[0].TableName;

                printGridView = dataGridView2;

                //Open the print dialog
                PrintDialog printDialog = new PrintDialog();
                printDialog.Document = printDocument1;
                printDialog.UseEXDialog = true;

                printDocument1.DefaultPageSettings.Margins.Left = 30;
                printDocument1.DefaultPageSettings.Margins.Right = 70;
                printDocument1.DefaultPageSettings.Margins.Top = 80;
                printDocument1.DefaultPageSettings.Margins.Bottom = 40;



                //Get the document
                if (DialogResult.OK == printDialog.ShowDialog())
                {
                    printDocument1.DocumentName = "Page";
                    printDocument1.Print();
                }





            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            new AboutForm().ShowDialog();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            
            
            textBox51.ReadOnly = true;
            button13.Enabled = false;
            beingEditedId = textBox51.Text;

            con = new SqlCeConnection("Data Source=|DataDirectory|\\SystemeDarkhast.sdf");
            con.Open();
            string cmdText = "SELECT * FROM Darkhast WHERE InternalID=(" + textBox51.Text + ");";
            adapter = new SqlCeDataAdapter(cmdText, con);
            set = new DataSet();
            adapter.Fill(set);
            con.Close();

            if (set.Tables[0].Rows.Count != 0)
            {
                textBox50.Text = set.Tables[0].Rows[0]["NameDarkhast"].ToString();
                textBox43.Text = set.Tables[0].Rows[0]["NameEkhtesari"].ToString();
                textBox49.Text = set.Tables[0].Rows[0]["ShomarehTaghaza"].ToString();
                textBox48.Text = set.Tables[0].Rows[0]["ShomarehTarh"].ToString();
                textBox47.Text = set.Tables[0].Rows[0]["ShomarehTabaghebandi_MESC"].ToString();
                textBox46.Text = set.Tables[0].Rows[0]["PartNumber"].ToString();
                textBox45.Text = set.Tables[0].Rows[0]["IranKode"].ToString();
                textBox44.Text = set.Tables[0].Rows[0]["Size"].ToString();
                textBox41.Text = set.Tables[0].Rows[0]["NameGhete"].ToString();
                textBox42.Text = set.Tables[0].Rows[0]["MaterialeGhete"].ToString();
                textBox40.Text = set.Tables[0].Rows[0]["Rang"].ToString();
                textBox39.Text = set.Tables[0].Rows[0]["Sakhti"].ToString();
                textBox38.Text = set.Tables[0].Rows[0]["Tedad"].ToString();
                textBox33.Text = set.Tables[0].Rows[0]["KodeNaghshe"].ToString();
                textBox25.Text = set.Tables[0].Rows[0]["EstelameBaha"].ToString();
                textBox26.Text = set.Tables[0].Rows[0]["EtebareGheymat"].ToString();
                textBox37.Text = set.Tables[0].Rows[0]["TarikheTolid"].ToString();
                textBox36.Text = set.Tables[0].Rows[0]["TolidKonandeh"].ToString();
                textBox35.Text = set.Tables[0].Rows[0]["KodeGhaleb"].ToString();
                textBox34.Text = set.Tables[0].Rows[0]["Gheymat"].ToString();
                textBox32.Text = set.Tables[0].Rows[0]["GheymateForoush"].ToString();



                textBox25.ReadOnly = textBox26.ReadOnly = textBox32.ReadOnly = textBox33.ReadOnly =
                textBox34.ReadOnly = textBox35.ReadOnly = textBox36.ReadOnly = textBox37.ReadOnly =
                textBox38.ReadOnly = textBox39.ReadOnly = textBox40.ReadOnly = textBox41.ReadOnly =
                textBox42.ReadOnly = textBox43.ReadOnly = textBox44.ReadOnly = textBox45.ReadOnly =
                textBox46.ReadOnly = textBox47.ReadOnly = textBox48.ReadOnly = textBox49.ReadOnly =
                textBox50.ReadOnly = false;

                button12.Enabled = button6.Enabled = true;

                this.ActiveControl = textBox50;
                this.AcceptButton = button12;

            }
            else
            {
                beingEditedId = null;
                MessageBox.Show("آیتمی با این شماره وجود ندارد.", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                textBox51.ReadOnly = false;
                button13.Enabled = true;
                this.ActiveControl = textBox51;
                this.AcceptButton = button13;
            }

                


        }

        private void button12_Click(object sender, EventArgs e)
        {

            textBox25.ReadOnly = textBox26.ReadOnly = textBox32.ReadOnly = textBox33.ReadOnly =
            textBox34.ReadOnly = textBox35.ReadOnly = textBox36.ReadOnly = textBox37.ReadOnly =
            textBox38.ReadOnly = textBox39.ReadOnly = textBox40.ReadOnly = textBox41.ReadOnly =
            textBox42.ReadOnly = textBox43.ReadOnly = textBox44.ReadOnly = textBox45.ReadOnly =
            textBox46.ReadOnly = textBox47.ReadOnly = textBox48.ReadOnly = textBox49.ReadOnly =
            textBox50.ReadOnly = true;

            try
            {
                con = new SqlCeConnection("Data Source=|DataDirectory|\\SystemeDarkhast.sdf");
                con.Open();
                com = con.CreateCommand();
                com.Connection = con;
                string updString = "UPDATE Darkhast SET ";



                if (textBox50.Text != System.String.Empty)
                    updString += ("NameDarkhast=\'" + textBox50.Text + "\'");
                else
                    updString += ("NameDarkhast=NULL");

                if (textBox43.Text != System.String.Empty)
                    updString += (", NameEkhtesari=\'" + textBox43.Text + "\'");
                else
                    updString += (", NameEkhtesari=NULL");

                if (textBox49.Text != System.String.Empty)
                    updString += (", ShomarehTaghaza=\'" + textBox49.Text + "\'");
                else
                    updString += (", ShomarehTaghaza=NULL");

                if (textBox48.Text != System.String.Empty)
                    updString += (", ShomarehTarh=\'" + textBox48.Text + "\'");
                else
                    updString += (", ShomarehTarh=NULL");

                if (textBox47.Text != System.String.Empty)
                    updString += (", ShomarehTabaghebandi_MESC=\'" + textBox47.Text + "\'");
                else
                    updString += (", ShomarehTabaghebandi_MESC=NULL");

                if (textBox46.Text != System.String.Empty)
                    updString += (", PartNumber=\'" + textBox46.Text + "\'");
                else
                    updString += (", PartNumber=NULL");

                if (textBox45.Text != System.String.Empty)
                    updString += (", IranKode=\'" + textBox45.Text + "\'");
                else
                    updString += (", IranKode=NULL");

                if (textBox44.Text != System.String.Empty)
                    updString += (", Size=\'" + textBox44.Text + "\'");
                else
                    updString += (", Size=NULL");

                if (textBox41.Text != System.String.Empty)
                    updString += (", NameGhete=\'" + textBox41.Text + "\'");
                else
                    updString += (", NameGhete=NULL");

                if (textBox42.Text != System.String.Empty)
                    updString += (", MaterialeGhete=\'" + textBox42.Text + "\'");
                else
                    updString += (", MaterialeGhete=NULL");

                if (textBox40.Text != System.String.Empty)
                    updString += (", Rang=\'" + textBox40.Text + "\'");
                else
                    updString += (", Rang=NULL");

                if (textBox39.Text != System.String.Empty)
                    updString += (", Sakhti=\'" + textBox39.Text + "\'");
                else
                    updString += (", Sakhti=NULL");

                if (textBox38.Text != System.String.Empty)
                    updString += (", Tedad=\'" + textBox38.Text + "\'");
                else
                    updString += (", Tedad=NULL");

                if (textBox33.Text != System.String.Empty)
                    updString += (", KodeNaghshe=\'" + textBox33.Text + "\'");
                else
                    updString += (", KodeNaghshe=NULL");

                if (textBox25.Text != System.String.Empty)
                    updString += (", EstelameBaha=\'" + textBox25.Text + "\'");
                else
                    updString += (", EstelameBaha=NULL");

                if (textBox26.Text != System.String.Empty)
                    updString += (", EtebareGheymat=\'" + textBox26.Text + "\'");
                else
                    updString += (", EtebareGheymat=NULL");

                if (textBox37.Text != System.String.Empty)
                    updString += (", TarikheTolid=\'" + textBox37.Text + "\'");
                else
                    updString += (", TarikheTolid=NULL");

                if (textBox36.Text != System.String.Empty)
                    updString += (", TolidKonandeh=\'" + textBox36.Text + "\'");
                else
                    updString += (", TolidKonandeh=NULL");

                if (textBox35.Text != System.String.Empty)
                    updString += (", KodeGhaleb=\'" + textBox35.Text + "\'");
                else
                    updString += (", KodeGhaleb=NULL");

                if (textBox34.Text != System.String.Empty)
                    updString += (", Gheymat=\'" + textBox34.Text + "\'");
                else
                    updString += (", Gheymat=NULL");

                if (textBox32.Text != System.String.Empty)
                    updString += (", GheymateForoush=\'" + textBox32.Text + "\'");
                else
                    updString += (", GheymateForoush=NULL");

                updString += (" WHERE InternalId = " + beingEditedId + ";");

                com.CommandText = updString;

                com.ExecuteNonQuery();
                con.Close();

                MessageBox.Show("ویرایش انجام شد.", "موفقیت", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);

            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.ToString());
                MessageBox.Show("خطا در انجام ویرایش!", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
            }

            textBox25.Text = textBox26.Text = textBox32.Text = textBox33.Text =
            textBox34.Text = textBox35.Text = textBox36.Text = textBox37.Text =
            textBox38.Text = textBox39.Text = textBox40.Text = textBox41.Text =
            textBox42.Text = textBox43.Text = textBox44.Text = textBox45.Text =
            textBox46.Text = textBox47.Text = textBox48.Text = textBox49.Text =
            textBox50.Text = System.String.Empty;

            button12.Enabled = button6.Enabled = false;

            beingEditedId = null;
            textBox51.ReadOnly = false;
            button13.Enabled = true;
            this.ActiveControl = textBox51;
            this.AcceptButton = button13;

        }

        private void button6_Click_1(object sender, EventArgs e)
        {


            textBox25.ReadOnly = textBox26.ReadOnly = textBox32.ReadOnly = textBox33.ReadOnly =
            textBox34.ReadOnly = textBox35.ReadOnly = textBox36.ReadOnly = textBox37.ReadOnly =
            textBox38.ReadOnly = textBox39.ReadOnly = textBox40.ReadOnly = textBox41.ReadOnly =
            textBox42.ReadOnly = textBox43.ReadOnly = textBox44.ReadOnly = textBox45.ReadOnly =
            textBox46.ReadOnly = textBox47.ReadOnly = textBox48.ReadOnly = textBox49.ReadOnly =
            textBox50.ReadOnly = true;

            textBox25.Text = textBox26.Text = textBox32.Text = textBox33.Text =
            textBox34.Text = textBox35.Text = textBox36.Text = textBox37.Text =
            textBox38.Text = textBox39.Text = textBox40.Text = textBox41.Text =
            textBox42.Text = textBox43.Text = textBox44.Text = textBox45.Text =
            textBox46.Text = textBox47.Text = textBox48.Text = textBox49.Text =
            textBox50.Text = System.String.Empty;


            button12.Enabled = button6.Enabled = false;

            beingEditedId = null;
            textBox51.ReadOnly = false;
            button13.Enabled = true;
            this.ActiveControl = textBox51;
            this.AcceptButton = button13;


        }




    }

}