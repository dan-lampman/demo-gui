using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DEMOGUI
{
    public partial class LicenseForm : Form
    {
        public LicenseForm()
        {
            InitializeComponent();
        }

        public Configuration config;
        public bool verified = false;
        public string license = "";

        private void btnSave_Click(object sender, EventArgs e)
        {
            var decoded = Base64Decode(txtInput.Text);

            var split = decoded.Split('*');

            if (split[1] == null)
                MessageBox.Show("Invalid license key");
            else
            {
                var decodedDate = Convert.ToDateTime(split[1]);
                                
                if (Base64Decode(txtInput.Text).Contains("tethys") == true && 
                    decodedDate >= DateTime.Now)
                {
                    verified = true;
                    this.DialogResult = DialogResult.OK;
                    license = txtInput.Text;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid license key");
                }
            }

        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

    }
}
