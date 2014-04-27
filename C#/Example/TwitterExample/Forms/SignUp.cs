using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TwitterExample
{

	
    public partial class SignUp : Form
    {

        Twitter.Twitter Client = new Twitter.Twitter();
        public SignUp()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Load(Client.GetCaptcha());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Client.Cap = textBox5.Text;
            Client.SignUp(Email.Text, Password.Text, FullName.Text, User.Text);
        }

        private void SignUp_Load(object sender, EventArgs e)
        {

        }
      
    }
}
