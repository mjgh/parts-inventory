using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Text;
using System.Windows.Forms;

namespace SystemeDarkhast
{
    public partial class LoginForm : Form
    {

        #region Fonts Problem

        SolidBrush solidBrush = new SolidBrush(Color.Black);
        FontFamily[] fontFamilies;
        PrivateFontCollection privateFontCollection = new PrivateFontCollection();
        Font Mitra;
        Font MitraBd;
        Font TitrBd;

        #endregion



        public LoginForm()
        {
            InitializeComponent();


            privateFontCollection.AddFontFile(Application.StartupPath + @"\Fonts\BMitra.ttf");
            privateFontCollection.AddFontFile(Application.StartupPath + @"\Fonts\BMitraBd.ttf");
            privateFontCollection.AddFontFile(Application.StartupPath + @"\Fonts\BTitrBd.ttf");

            fontFamilies = privateFontCollection.Families;


            Mitra = new System.Drawing.Font(fontFamilies[0], 12, System.Drawing.FontStyle.Regular);
            MitraBd = new System.Drawing.Font(fontFamilies[0], 12, System.Drawing.FontStyle.Bold);
            TitrBd = new System.Drawing.Font(fontFamilies[1], 16, System.Drawing.FontStyle.Bold);

            label1.Font = MitraBd;
            textBox1.Font = Mitra;
            button_login.Font = TitrBd;

            this.Icon = Properties.Resources.User;
        }

        private void button_login_Click(object sender, EventArgs e)
        {
            if (string.Equals(textBox1.Text, "mjgh"))
            {
                Hide();
                new Form1().ShowDialog();
            }

            else
            {
                MessageBox.Show("رمز ورود اشتباه است!", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                Application.Exit();
            }

        }
    }
}
