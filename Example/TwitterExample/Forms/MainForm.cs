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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SignUp SignUp = new SignUp();
            SignUp.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoginFrm Login = new LoginFrm();
            Login.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            TweetExample Tweet = new TweetExample();
            Tweet.Show();
        }
    }
}
