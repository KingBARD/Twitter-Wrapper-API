using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TwitterExample
{
    public partial class LoginFrm : Form
    {
        Twitter.Twitter Client = new Twitter.Twitter();

        public LoginFrm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Client.Login(Username.Text, Password.Text);

                if(Client.SignedIn == true)
                {
                    if(Client.Token == null)
                    {
                        return;
                    }
                    else
                    {
                        textBox3.Text = Client.Token;
                        string Time = DateTime.Now.TimeOfDay.ToString();
                        MessageBox.Show(string.Format("Signed into {0} at {1}", Client.ScreenName, Time));
                    }
                }
                else
                {
                    MessageBox.Show("Sign in failed");
                }
            }
            catch (Exception)
            {
                
                throw;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void LoginFrm_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Password_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Username_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
