using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace com.inova8.resolver
{
    using System.Windows.Forms;
    public partial class frmRegister : Form
    {
        private com.inova8.resolver.LicenserClass resolverLicense;
        public frmRegister(com.inova8.resolver.LicenserClass ResolverLicense)
        {
            InitializeComponent();
            tbID.Text = com.inova8.resolver.LicenserClass.GetComputerId();
            resolverLicense = ResolverLicense;
        }


        private void tbKey_TextChanged(object sender, EventArgs e)
        {
            if (tbKey.Text.Length > 0)
            {
                btnRegister.Enabled = true;
            }
            else
            {
                btnRegister.Enabled = false;
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            switch (resolverLicense.ProduceSingleLicense(tbKey.Text))
            {
                case com.inova8.resolver.LicenserClass.LicenseType.Full:
                    MessageBox.Show("Full license registered", "Resolver Registration");
                    break;
                case com.inova8.resolver.LicenserClass.LicenseType.Trial:
                    MessageBox.Show("Trial license registered", "Resolver Registration");
                    break;
                case com.inova8.resolver.LicenserClass.LicenseType.Null:
                    MessageBox.Show("Not a valid license key", "Resolver Registration");
                    break;
                default:
                    break;
            }
            this.Close();
        }

        private void btnGetKey_Click(object sender, EventArgs e)
        {
            ProcessStartInfo p = new ProcessStartInfo();
            p.FileName = getDefaultBrowser();
            p.Arguments = "http://www.inova8.com/joomla/index.php/software";
            Process.Start(p);
        }
        private string getDefaultBrowser() 
        {
        String browser = String.Empty;
        Microsoft.Win32.RegistryKey key  = null;
        try
        {
            key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey("HTTP\\shell\\open\\command", false);

            //trim off quotes
            browser = key.GetValue(null).ToString().ToLower().Replace("\"", "");
            if (!browser.EndsWith("exe"))
            {
                //get rid of everything after the ".exe"
                browser = browser.Substring(0, browser.LastIndexOf(".exe") + 4);
            }
        }
         finally
        {
            if (!(key == null))
            {
                key.Close();
            }
        }
        return browser;
        }

        private void frmRegister_Load(object sender, EventArgs e)
        {

        }

    }
}
