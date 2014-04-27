using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Windows.Forms;

namespace TwitterExample
{
    public partial class TweetExample : Form
    {
       Twitter.Twitter Client = new Twitter.Twitter();
        public TweetExample()
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
            if(textBox1.Text == null)
                {
                    return;
                }
                else
                {
                    try 
	                {
                        string Tweet = textBox1.Text;
                        Client.Tweet(Tweet, false);
                        if (Client.Tweeted == true)
                        {
                            MessageBox.Show("Tweeted!");
                        }
                        else
                        {
                            MessageBox.Show("Tweet failed!" + e.ToString());
                        }
	                }
                    catch (WebException)
	                {
                       MessageBox.Show("You already tweeted that!");
	                }

                }
        }
    }
}
