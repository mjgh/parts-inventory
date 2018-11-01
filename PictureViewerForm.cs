using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;


namespace SystemeDarkhast
{
    public partial class PictureViewerForm : Form
    {
        private System.Collections.Specialized.StringCollection pictureAddresses;
        int i;

        public PictureViewerForm(System.Collections.Specialized.StringCollection pa)
        {
            InitializeComponent();

            this.Icon = Properties.Resources.Application;

            if (pa.Count == 0)
                this.Close();

            pictureAddresses = pa;
            i = 0;

            Image loadedImage = Image.FromFile(pictureAddresses[i]);
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pictureBox1.Image = loadedImage;


        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (i < (pictureAddresses.Count - 1))
            {
                i++;
                
                Image loadedImage = Image.FromFile(pictureAddresses[i]);
                pictureBox1.Image = loadedImage;

            }

        }


        private void button3_Click(object sender, EventArgs e)
        {

            this.Close();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            if (i > 0)
            {
                i--;
                Image loadedImage = Image.FromFile(pictureAddresses[i]);
                pictureBox1.Image = loadedImage;

            }



        }

        private void button4_Click(object sender, EventArgs e)
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += PrintPage;

            //Open the print dialog
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = pd;
            printDialog.UseEXDialog = true;

            //Get the document
            if (DialogResult.OK == printDialog.ShowDialog())
            {
                pd.DocumentName = "Picture";
                pd.Print();
            }




        }


        private void PrintPage(object o, PrintPageEventArgs e)
        {

            Image i = pictureBox1.Image;

            float imageAspectRatio = (float)i.Width / i.Height;
            float pageAspectRatio = (float)e.PageBounds.Width / e.PageBounds.Height;

            Rectangle rect = new Rectangle();

            if (imageAspectRatio > pageAspectRatio)
            {
                rect.Height = (int)(e.PageBounds.Width / imageAspectRatio);
                rect.Width = e.PageBounds.Width;
            }
            else
            {
                rect.Width = (int)(e.PageBounds.Height * imageAspectRatio);
                rect.Height = e.PageBounds.Height;
            }

            rect.X = (e.PageBounds.Width - rect.Width) / 2;
            rect.Y = (e.PageBounds.Height - rect.Height) / 2;


            e.Graphics.DrawImage(i, rect);

        }
    }
}
