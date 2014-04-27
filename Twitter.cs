using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Web;

namespace Twitter
{
    /// <summary>
    /// Bard/Brad
    /// https://github.com/KingBARD
    /// 4.0 Framework
    /// System.Web Reference must be added.
    /// System.Drawing Reference must be added.
    /// </summary>
    class Twitter
    {

        public OpenFileDialog OFD = new OpenFileDialog();
        CookieContainer CC = new CookieContainer();
        CookieContainer logincookie;
        UTF8Encoding encodings = new UTF8Encoding();

        const string UserA = "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0";

        public bool UserExists = false;
        public bool SignedIn;
        public bool Deleted;
        public bool TweetFavorited;
        public bool Tweeted;
        public bool ChangedPassword;
        public bool DMWorked;
        public bool ChangedSN;
        public bool FollowedUser;
        public bool Retweeted;
        public bool ChangedImaged;
        public bool ChangedDesc;
        public bool ChangedLoc;
        public bool ChangedProfileURL;
        public bool SignedUp;
        public bool ChangedTrends;
        public bool GotTrends;
        public bool ChangedEmail;
    

        public string Token;
        public string ScreenName;
        public string Cap;
        public string CapPicture;

        string Password;
        string MToken;

        public string GetTrends()
        {
            GotTrends = false;
            string Result = null;

            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("https://twitter.com/trends");
            Request.KeepAlive = true;
            Request.Method = "GET";
            Request.UserAgent = UserA;
            HttpWebResponse Response = null;
            Response = (HttpWebResponse)Request.GetResponse();
            StreamReader RequestReader = new StreamReader(Response.GetResponseStream());
            Result = RequestReader.ReadToEnd();

            Result = Result.Replace("\\u003c", "<");
            Result = Result.Replace("\\u003e", ">");
            Result = Result.Replace("\\n", Environment.NewLine);
            Result = Result.Replace("\\\"", "\"");
            Result = Result.Replace("\\/", "/");
            string Test = null;

            foreach (Match Matches in Regex.Matches(Result, "<li class=\"trend-item js-trend-item  \" data-trend-name=\"(.*)\" >"))
            {

                Test = Test + '|' + Matches.Groups[1].Value + Environment.NewLine;
            }

            GotTrends = true;
            
            return Test;

        }

        public bool UserExist(string User)
        {
            try
            {

                HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create("http://www.twitter.com/" + User);
                Request.KeepAlive = true;
                Request.Method = "GET";
                Request.UserAgent = UserA;
                HttpWebResponse Response = null;
                Response = (HttpWebResponse)Request.GetResponse();
                StreamReader Reader = new StreamReader(Response.GetResponseStream());
                string Result = Reader.ReadToEnd();

                // This could change.
                if (Result.Contains("Sorry, that page doesn’t exist!")) { }
                else
                {
                    UserExists = true;

                    return true;
                }
            }
            catch (System.Net.WebException)
            {
            }

            return false;
        }

        public object Login(string User, string Pass)
        {

            ScreenName = User;
            Password = Pass;

            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("https://www.twitter.com");
            Request.KeepAlive = true;
            Request.Method = "GET";
            Request.CookieContainer = CC;
            Request.UserAgent = UserA;
            HttpWebResponse Response = default(HttpWebResponse);
            Response = (HttpWebResponse)Request.GetResponse();

            foreach (Cookie Cookie in Response.Cookies)
            {
                CC.Add(Cookie);
            }

            StreamReader RequestReader = new StreamReader(Response.GetResponseStream());
            string Result = RequestReader.ReadToEnd();
            Match Match = Regex.Match(Result, "<input type=\"hidden\" name=\"authenticity_token\" value=\"(.+)\">");
            Token = Match.Groups[1].Value;

            string PostData = "session%5Busername_or_email%5D=" + ScreenName + "&session%5Bpassword%5D=" + Password + "&authenticity_token=" + Token + "&scribe_log=&redirect_after_login=&authenticity_token=" + Token;
            byte[] byteData = encodings.GetBytes(PostData);

            HttpWebRequest GetRequest = (HttpWebRequest)WebRequest.Create("https://twitter.com/sessions");
            GetRequest.CookieContainer = CC;
            GetRequest.Method = "POST";
            GetRequest.Referer = "https://twitter.com/login";
            GetRequest.KeepAlive = true;
            GetRequest.Host = "twitter.com";
            GetRequest.ContentType = "application/x-www-form-urlencoded";
            GetRequest.UserAgent = UserA;
            GetRequest.ContentLength = byteData.Length;

            Stream Requeststream = GetRequest.GetRequestStream();
            Requeststream.Write(byteData, 0, byteData.Length);
            Requeststream.Close();

            HttpWebResponse PostResponse = default(HttpWebResponse);
            PostResponse = (HttpWebResponse)GetRequest.GetResponse();

            foreach (Cookie Cookies in PostResponse.Cookies)
            {
                CC.Add(Cookies);
            }

            logincookie = CC;

            StreamReader Requestreader = new StreamReader(PostResponse.GetResponseStream());
            string PageSource = Requestreader.ReadToEnd();

            HttpWebRequest NewRequest = (HttpWebRequest)WebRequest.Create("https://www.twitter.com");
            NewRequest.KeepAlive = true;
            NewRequest.Method = "GET";
            NewRequest.CookieContainer = logincookie;
            NewRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0";
            HttpWebResponse NewResponse = default(HttpWebResponse);
            NewResponse = (HttpWebResponse)NewRequest.GetResponse();
            StreamReader NewRequestReader = new StreamReader(NewResponse.GetResponseStream());
            string Final = NewRequestReader.ReadToEnd();

            bool contains = Final.IndexOf("body.user-style-" + ScreenName, StringComparison.OrdinalIgnoreCase) >= 0;

            if (contains == true)
                SignedIn = true;

            else
                SignedIn = false;

            return Final;

        }

        public object Follow(string UserLink, bool Unfollow)
        {
            if (SignedIn == false)
                return "not signed in";
            else
            {
                string PostURL;
                if (Unfollow == false)
                    PostURL = "https://twitter.com/i/user/follow";
                else
                    PostURL = "https://twitter.com/i/user/unfollow";

                HttpWebRequest GetRequest = (HttpWebRequest)HttpWebRequest.Create(UserLink);
                GetRequest.Method = "GET";
                GetRequest.KeepAlive = true;
                GetRequest.UserAgent = UserA;

                HttpWebResponse GetResponse = default(HttpWebResponse);
                GetResponse = (HttpWebResponse)GetRequest.GetResponse();

                StreamReader Reader = new StreamReader(GetResponse.GetResponseStream());
                string Result = Reader.ReadToEnd();

                Match UserIDMatch = Regex.Match(Result, "issable&quot;:true,&quot;similar_to_user_id&quot;:(.+)?}");
                string UserId = UserIDMatch.Groups[1].Value;

                UserId = UserId.Substring(0, UserId.IndexOf('}'));
                string PostData = "authenticity_token=" + Token + "&user_id=" + UserId;
                byte[] ByteData = encodings.GetBytes(PostData);

                HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create(PostURL);
                Request.CookieContainer = logincookie;
                Request.Method = "POST";
                Request.Host = "twitter.com";
                Request.KeepAlive = true;
                Request.Referer = "https://www.twitter.com/" + ScreenName;
                Request.ContentType = "application/x-www-form-urlencoded";
                Request.UserAgent = UserA;
                Request.ContentLength = ByteData.Length;
                Stream RequestStream = Request.GetRequestStream();
                RequestStream.Write(ByteData, 0, ByteData.Length);
                RequestStream.Close();
                HttpWebResponse PostResponse = default(HttpWebResponse);
                PostResponse = (HttpWebResponse)Request.GetResponse();
                StreamReader PStreamReader = new StreamReader(PostResponse.GetResponseStream());
                string Final = PStreamReader.ReadToEnd();
                if (Final.Contains("{\"profile_stats\":[{"))
                    FollowedUser = true;
                else
                    FollowedUser = false;

                return Final;

            }
        }

        public object Retweet(string TweetLink, bool Delete)
        {
            string Result;
            Match Matchs = Regex.Match(TweetLink, "https://twitter.com/(.+)?/status/(.+)?");
            string Name = Matchs.Groups[1].Value;
            string ID = Matchs.Groups[2].Value;
            string PostData;
            string URL;

            if (SignedIn == false)
                return "not signed in";
            else
            {
                if (Delete == true)
                {
                    URL = "https://twitter.com/i/tweet/unretweet";
                    PostData = "_method=DELETE&authenticity_token=" + Token + "&id=" + ID;
                }
                else
                {
                    PostData = "&authenticity_token=" + Token + "&id=" + ID;
                    URL = "https://twitter.com/i/tweet/retweet";
                }

                byte[] ByteData = encodings.GetBytes(PostData);
                HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create(URL);
                Request.Method = "POST";
                Request.KeepAlive = true;
                Request.UserAgent = UserA;
                Request.ContentType = "application/x-www-form-urlencoded";
                Request.ContentLength = ByteData.Length;
                Request.AllowAutoRedirect = true;
                // Won't post without this.
                Request.Referer = "https://twitter.com/" + Name + "/" + ID;
                Request.Host = "Twitter.com";
                Request.CookieContainer = logincookie;

                Stream PostStream = Request.GetRequestStream();
                PostStream.Write(ByteData, 0, ByteData.Length);
                PostStream.Close();


                HttpWebResponse Response = default(HttpWebResponse);
                Response = (HttpWebResponse)Request.GetResponse();
                StreamReader PStreamReader = new StreamReader(Response.GetResponseStream());
                Result = PStreamReader.ReadToEnd();

                if (Result.Contains("Tweets"))
                    Retweeted = true;
                else
                    Retweeted = false;

                return Result;

            }
        }

        public object FavoriteTweet(string TweetLink, bool Delete)
        {
            Match Matchs = Regex.Match(TweetLink, "https://twitter.com/(.+)?/status/(.+)?");
            string Name = Matchs.Groups[1].Value;
            string ID = Matchs.Groups[2].Value;
            string URL;
            string Result;


            if (SignedIn == false)
                return "Not signed in";
            else
            {
                if (Delete == true)
                    URL = "https://twitter.com/i/tweet/unfavorite";
                else
                    URL = "https://twitter.com/i/tweet/favorite";

                string PostData = "authenticity_token=" + Token + "&id=" + ID;
                byte[] ByteData = encodings.GetBytes(PostData);

                HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create(URL);
                Request.Method = "POST";
                Request.KeepAlive = true;
                Request.UserAgent = UserA;
                Request.ContentLength = ByteData.Length;
                Request.ContentType = "application/x-www-form-urlencoded";
                Request.Referer = "https://www.twitter.com" + Name + '/' + ID;
                Request.CookieContainer = logincookie;

                Stream PostStream = Request.GetRequestStream();
                PostStream.Write(ByteData, 0, ByteData.Length);
                PostStream.Close();

                HttpWebResponse Response = default(HttpWebResponse);
                Response = (HttpWebResponse)Request.GetResponse();
                StreamReader Reader = new StreamReader(Response.GetResponseStream());
                Result = Reader.ReadToEnd();
                if (Result.Contains("Favorited 1 time"))
                    TweetFavorited = true;
                else
                    TweetFavorited = false;

                return Result;

            }
        }

        public object DeleteTweet(string Tweet)
        {
            string Result;
            Match Matchs = Regex.Match(Tweet, "https://twitter.com/(.+)?/status/(.+)?");
            string ID = Matchs.Groups[2].Value;
            string PostData = "_method=DELETE&authenticity_token=" + Token + "&id=" + ID;
            byte[] ByteData = encodings.GetBytes(PostData);

            HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create("https://twitter.com/i/tweet/destroy");
            Request.Method = "POST";
            Request.KeepAlive = true;
            Request.ContentLength = ByteData.Length;
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Referer = "https://www.twitter.com" + ScreenName;
            Request.Host = "twitter.com";
            Request.UserAgent = UserA;
            Request.CookieContainer = logincookie;

            Stream PostStream = Request.GetRequestStream();
            PostStream.Write(ByteData, 0, ByteData.Length);
            PostStream.Close();

            HttpWebResponse Response = default(HttpWebResponse);
            Response = (HttpWebResponse)Request.GetResponse();
            StreamReader Reader = new StreamReader(Response.GetResponseStream());
            Result = Reader.ReadToEnd();

            if (Result.Contains("Your tweet as been deleted."))
                Deleted = true;
            else
                Deleted = false;

            return Result;

        }

        public object Tweet(string TweetText, bool Reply, string TweetId = null)
        {
            if (SignedIn == false)
                return "Not signed in";
            else
            {
                string PostData;

                if (Reply == true)
                {
                    Match Matchs = Regex.Match(TweetId, "https://twitter.com/(.+)?/status/(.+)?");
                    string One = Matchs.Groups[1].Value;
                    string ID = Matchs.Groups[2].Value;
                    string Name = '@' + One + '+';

                    PostData = "authenticity_token=" + Token + "&in_reply_to_status_id=" + ID + "&place_id=&status=" + Name + TweetText;
                }
                else
                {
                    PostData = "authenticity_token=" + Token + "&place_id=&status=" + TweetText;
                }

                byte[] ByteData = encodings.GetBytes(PostData);

                HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create("https://twitter.com/i/tweet/create");
                Request.Method = "POST";
                Request.Host = "twitter.com";
                Request.CookieContainer = logincookie;
                Request.Referer = "https://twitter.com/";
                Request.UserAgent = UserA;
                Request.KeepAlive = true;
                Request.ContentType = "application/x-www-form-urlencoded";
                Request.ContentLength = ByteData.Length;

                Stream PostStream = Request.GetRequestStream();
                PostStream.Write(ByteData, 0, ByteData.Length);
                PostStream.Close();

                HttpWebResponse Response = default(HttpWebResponse);
                Response = (HttpWebResponse)Request.GetResponse();
                StreamReader Reader = new StreamReader(Response.GetResponseStream());
                string Result = Reader.ReadToEnd();
                if (Result.Contains("Your Tweet was over 140 characters."))
                    Tweeted = false;
                else
                    Tweeted = true;

                return Result;

            }
        }

        public string GetCaptcha()
        {
            HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create("https://mobile.twitter.com/signup");
            Request.KeepAlive = true;
            Request.Method = "GET";
            Request.CookieContainer = CC;
            Request.UserAgent = UserA;

            HttpWebResponse Response = default(HttpWebResponse);
            Response = (HttpWebResponse)Request.GetResponse();

            foreach (Cookie Cookie in Response.Cookies)
            {
                CC.Add(Cookie);
            }

            StreamReader Reader = new StreamReader(Response.GetResponseStream());
            string Result = Reader.ReadToEnd();

            Match Matchs = Regex.Match(Result, "<input name=\"authenticity_token\" type=\"hidden\" value=\"(.+)?\" /></span>");
            Token = Matchs.Groups[1].Value;
            Match Matchs2 = Regex.Match(Result, "<input type=\"hidden\" name=\"captcha_token\" value=\"(.+)?\"/>");
            MToken = Matchs2.Groups[1].Value;
            CapPicture = "https://mobile.twitter.com/signup/captcha/" + MToken;

            return CapPicture;

        }

        public object ChangePassword(string NewPass)
        {
            if (SignedIn == false)
                return "Not signed in";
            else
            {
                string PostData = "_method=PUT&authenticity_token=" + Token + "&current_password=" + Password + "&user_password=" + NewPass + "&user_password_confirmation=" + NewPass;
                byte[] ByteData = encodings.GetBytes(PostData);

                HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create("https://twitter.com/settings/passwords/update");
                Request.Method = "POST";
                Request.KeepAlive = true;
                Request.CookieContainer = logincookie;
                Request.UserAgent = UserA;
                Request.Host = "twitter.com";
                Request.ContentType = "application/x-www-form-urlencoded";
                Request.ContentLength = ByteData.Length;
                Request.Referer = "https://twitter.com/settings/password";

                Stream PostStream = Request.GetRequestStream();
                PostStream.Write(ByteData, 0, ByteData.Length);
                PostStream.Close();

                HttpWebResponse Response = default(HttpWebResponse);
                Response = (HttpWebResponse)Request.GetResponse();

                StreamReader Reader = new StreamReader(Response.GetResponseStream());
                string Result = Reader.ReadToEnd();

                if (Result.Contains("Woo hoo!"))
                    ChangedPassword = true;
                else
                    ChangedPassword = false;

                return Result;

            }

        }

        public object DM(string UserDm, string Message)
        {
            if (SignedIn == false)
                return "Not signed in";
            else
            {
                string PostData = "authenticity_token=" + Token + "&lastMsgId=&screen_name=" + UserDm + "&scribeContext%5Bcomponent%5D=dm_existing_conversation_dialog&text=" + Message;
                byte[] ByteData = encodings.GetBytes(PostData);

                HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create("https://twitter.com/i/direct_messages/new");
                Request.CookieContainer = logincookie;
                Request.Method = "POST";
                Request.Referer = "https://twitter.com/";
                Request.KeepAlive = true;
                Request.ContentType = "application/x-www-form-urlencoded";
                Request.UserAgent = UserA;
                Request.ContentLength = ByteData.Length;

                Stream PostStream = Request.GetRequestStream();
                PostStream.Write(ByteData, 0, ByteData.Length);
                PostStream.Close();
                HttpWebResponse Response = default(HttpWebResponse);
                Response = (HttpWebResponse)Request.GetResponse();
                StreamReader Reader = new StreamReader(Response.GetResponseStream());
                string Result = Reader.ReadToEnd();
                if (Result.Contains("Your account may not be allowed to perform this action. Please"))
                    DMWorked = false;
                else
                    DMWorked = true;

                return Result;

            }
        }

        public object ChangeEmail(string Email)
        {
            if (SignedIn == false)
                return "Not signed in!";
            else
            {
                string PostData = "_method=PUT&authenticity_token=" + Token + "&user%5Bscreen_name%5D=" + ScreenName + "&user%5Bemail%5D=" + Email + "&auth_password=" + Password;
                byte[] ByteData = encodings.GetBytes(PostData);
                HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create("");
                Request.CookieContainer = logincookie;
                Request.Method = "POST";
                Request.Referer = "https://twitter.com/settings/account";
                Request.KeepAlive = true;
                Request.AllowAutoRedirect = true;
                Request.ContentType = "application/x-www-form-urlencoded";
                Request.ContentLength = ByteData.Length;

                Stream PostStream = Request.GetRequestStream();
                PostStream.Write(ByteData, 0, ByteData.Length);
                PostStream.Close();

                HttpWebResponse Response = default(HttpWebResponse);
                Response = (HttpWebResponse)Request.GetResponse();

                StreamReader Reader = new StreamReader(Response.GetResponseStream());
                string Result = Reader.ReadToEnd();

                if (Result.Contains("Thanks, your settings have been saved."))
                    ChangedEmail = true;
                else
                    ChangedEmail = false;

                return Result;

            }
        }

        public object ChangeScreen(string NewScreenName)
        {
            if (SignedIn == false)
                return "Not signed in";
            else
            {
                string PostData = "_method=PUT&authenticity_token=" + Token + "&user%5Bscreen_name%5D=" + NewScreenName + "&auth_password=" + Password;
                byte[] ByteData = encodings.GetBytes(PostData);

                HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create("");
                Request.CookieContainer = logincookie;
                Request.Method = "POST";
                Request.Referer = "https://twitter.com/settings/account";
                Request.KeepAlive = true;
                Request.AllowAutoRedirect = true;
                Request.ContentLength = ByteData.Length;
                Request.ContentType = "application/x-www-form-urlencoded";
                Request.UserAgent = UserA;
                Stream PostStream = Request.GetRequestStream();
                PostStream.Write(ByteData, 0, ByteData.Length);
                PostStream.Close();

                HttpWebResponse Response = default(HttpWebResponse);
                Response = (HttpWebResponse)Request.GetResponse();

                StreamReader Reader = new StreamReader(Response.GetResponseStream());
                string Result = Reader.ReadToEnd();
                if (Result.Contains("Thanks, your settings have been saved."))
                    ChangedSN = false;
                else
                    ChangedSN = true;

                return Result;

            }
        }

        private string ConvertFileToBase64(string FileName)
        {
            return Convert.ToBase64String(System.IO.File.ReadAllBytes(FileName));
        }

        public object ChangeImage(bool Header, bool Avatar)
        {
            if (SignedIn == false)
                return "Not signed in";
            else
            {

                OFD.ShowDialog();

                Image image = Image.FromFile(OFD.FileName);
                string FilePath = OFD.FileName;
                string encodedText = ConvertFileToBase64(FilePath);
                encodedText = HttpUtility.UrlEncode(encodedText);

                string PostData;
                string URL;
                if (Header == true)
                {
                    URL = "https://twitter.com/settings/profile/upload_profile_header";
                    PostData = "authenticity_token=" + Token + "&fileData=" + encodedText + "&fileName=" + OFD.FileName + "&height=" + image.Height + "&offsetLeft=0&offsetTop=0&page_context=settings&scribeContext%5Bcomponent%5D=header_image_upload&scribeElement=upload&section_context=profile&uploadType=header&width=" + image.Width;
                }
                else
                {
                    URL = "https://twitter.com/settings/profile/profile_image_update";
                    PostData = "authenticity_token=" + Token + "&fileData=" + encodedText + "fileName=" + OFD.FileName + "&height=" + image.Height + "&offsetLeft=0&offsetTop=0&page_context=me&scribeContext%5Bcomponent%5D=profile_image_upload&scribeElement=upload&section_context=profile&uploadType=avatar&width=" + image.Width;
                }
                byte[] ByteData = encodings.GetBytes(PostData);
                HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create(URL);
                Request.Method = "POST";
                Request.Referer = "https://twitter.com/" + ScreenName + "edit=true";
                Request.UserAgent = UserA;
                Request.Host = "twitter.com";
                Request.AllowAutoRedirect = true;
                Request.ContentLength = ByteData.Length;
                Request.ContentType = "application/x-www-form-urlencoded";

                Stream PostStream = Request.GetRequestStream();
                PostStream.Write(ByteData, 0, ByteData.Length);
                PostStream.Close();
                HttpWebResponse Response = default(HttpWebResponse);
                Response = (HttpWebResponse)Request.GetResponse();
                StreamReader Reader = new StreamReader(Response.GetResponseStream());
                string Result = Reader.ReadToEnd();

                if (Result.Contains("profile_images"))
                    ChangedImaged = true;
                else
                    ChangedImaged = false;

                return Result;

            }

        }

        public object ChangeDesciption(string Desc)
        {
            if (SignedIn == false)
                return "Not signed in";
            else
            {
                Desc.Replace(" ", "+");
                Desc = HttpUtility.UrlEncode(Desc);
                string PostData = "authenticity_token=" + Token + "&page_context=me&section_context=profile&user%5Bdescription%5D=" + Desc;
                byte[] ByteData = encodings.GetBytes(PostData);

                HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create("https://twitter.com/i/profiles/update");
                Request.CookieContainer = logincookie;
                Request.Method = "POST";
                Request.Referer = "https://twitter.com/" + ScreenName + "?edit=true";
                Request.KeepAlive = true;
                Request.AllowAutoRedirect = true;
                Request.ContentType = "application/x-www-form-urlencoded";
                Request.UserAgent = UserA;
                Request.ContentLength = ByteData.Length;

                Stream PostStream = Request.GetRequestStream();
                PostStream.Write(ByteData, 0, ByteData.Length);
                PostStream.Close();

                HttpWebResponse Response = default(HttpWebResponse);
                Response = (HttpWebResponse)Request.GetResponse();

                StreamReader Reader = new StreamReader(Response.GetResponseStream());
                string Result = Reader.ReadToEnd();

                if (Result.Contains("description"))
                    ChangedDesc = true;
                else
                    ChangedDesc = false;

                return Result;
            }

        }

        public object ChangeProfileLocation(string Loc)
        {
            if (SignedIn == false)
                return "Not signed in";
            else
            {
                string PostData = "authenticity_token=" + Token + "&page_context=me&section_context=profile&user%5Blocation%5D=" + Loc;
                byte[] ByteData = encodings.GetBytes(PostData);
                HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create("https://twitter.com/i/profiles/update");

                Request.CookieContainer = logincookie;
                Request.Method = "POST";
                Request.KeepAlive = true;
                Request.Host = "twitter.com";
                Request.Referer = "https://twitter.com/" + ScreenName + "?edit=true";
                Request.ContentType = "application/x-www-form-urlencoded";
                Request.UserAgent = UserA;
                Request.ContentLength = ByteData.Length;

                Stream PostStream = Request.GetRequestStream();
                PostStream.Write(ByteData, 0, ByteData.Length);
                PostStream.Close();

                HttpWebResponse Response = default(HttpWebResponse);
                Response = (HttpWebResponse)Request.GetResponse();
                StreamReader Reader = new StreamReader(Response.GetResponseStream());

                string Result = Reader.ReadToEnd();
                if (Result.Contains("{}"))
                    ChangedLoc = true;
                else
                    ChangedLoc = false;

                return Result;
            }
        }

        public object ChangeProfileURL(string URL)
        {
            if (SignedIn == false)
                return "Not signed in";
            {
                URL = HttpUtility.UrlEncode(URL);
                string PostData = "authenticity_token=" + Token + "&page_context=me&section_context=profile&user%5Burl%5D=" + URL;
                byte[] ByteData = encodings.GetBytes(PostData);
                HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create("https://twitter.com/i/profiles/update");
                Request.CookieContainer = logincookie;
                Request.Method = "POST";
                Request.KeepAlive = true;
                Request.ContentType = "application/x-www-form-urlencoded";
                Request.UserAgent = UserA;
                Request.AllowAutoRedirect = true;
                Request.ContentLength = ByteData.Length;
                Stream PostStream = Request.GetRequestStream();
                PostStream.Write(ByteData, 0, ByteData.Length);
                PostStream.Close();
                HttpWebResponse Response = default(HttpWebResponse);
                Response = (HttpWebResponse)Request.GetResponse();
                StreamReader Reader = new StreamReader(Response.GetResponseStream());
                string Result = Reader.ReadToEnd();

                if (Result.Contains("user_url"))
                    ChangedProfileURL = true;
                else
                    ChangedProfileURL = false;

                return Result;

            }
        }

        public object SignUp(string Email, string SPassword, string Name, string User)
        {
            string PostData = "authenticity_token=" + Token + "&oauth_signup_client%5Bfullname%5D=" + Name + "&oauth_signup_client%5Bemail%5D=" + Email + "&oauth_signup_client%5Bscreen_name%5D=" + User + "&oauth_signup_client%5Bpassword%5D=" + SPassword + "&oauth_signup_client%5Buse_cookie_personalization%5D=0&captcha_token=" + MToken + "&captcha_response_field=" + Cap;
            byte[] ByteData = encodings.GetBytes(PostData);

            HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create("https://mobile.twitter.com/signup");
            Request.CookieContainer = CC;
            Request.Method = "POST";
            Request.Referer = "https://mobile.twitter.com/signup";
            Request.KeepAlive = true;
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.UserAgent = UserA;
            Request.ContentLength = ByteData.Length;
            Request.Host = "mobile.twitter.com";

            Stream PostStream = Request.GetRequestStream();
            PostStream.Write(ByteData, 0, ByteData.Length);
            PostStream.Close();

            HttpWebResponse Response = default(HttpWebResponse);
            Response = (HttpWebResponse)Request.GetResponse();

            foreach (Cookie Cookie in Response.Cookies)
            {
                CC.Add(Cookie);
            }

            logincookie = CC;
            StreamReader Reader = new StreamReader(Response.GetResponseStream());
            string Result = Reader.ReadToEnd();
            if (Result.Contains("The words you typed didn't match. Try again!"))
                SignedUp = false;
            else if (Result.Contains("<div class=\"signup-field-hint\">You can also use your email address</div>"))
                SignedUp = false;
            else if (Result.Contains("You've made a few too many attempts. Please try again later."))
                SignedUp = false;
            else
                SignedUp = true;

            return Result;

        }

        public string GetProfileHeader(string user)
        {
            HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create("https://www.twitter.com/" + user);
            Request.Method = "GET";
            Request.KeepAlive = true;
            Request.CookieContainer = CC;
            Request.UserAgent = UserA;
            HttpWebResponse Response = default(HttpWebResponse);
            Response = (HttpWebResponse)Request.GetResponse();
            StreamReader Reader = new StreamReader(Response.GetResponseStream());
            string Result = Reader.ReadToEnd();

            Match Match = Regex.Match(Result,"data-background-image=\"url(.+)?");
            string Header = Match.Groups[1].Value;
            Header = Header.Replace("'", "");
            Header = Header.Replace("(","");
            Header = Header.Replace(")", "");
            Header = Header.Replace("\"", "");
            Header = Header.Replace(">", "");

            return Header;
        }

        public string GetProfilePicture(string user)
        {
            HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create("https://www.twitter.com/" + user);
            Request.Method = "GET";
            Request.KeepAlive = true;
            Request.CookieContainer = CC;
            Request.UserAgent = UserA;
            HttpWebResponse Response = default(HttpWebResponse);
            Response = (HttpWebResponse)Request.GetResponse();
            StreamReader Reader = new StreamReader(Response.GetResponseStream());
            string Result = Reader.ReadToEnd();

            Match PictureMatch = Regex.Match(Result, "<img src=\"(.+)?\" alt=\"(.+)?\" class=\"avatar size73\">");
            string PictureString = PictureMatch.Groups[1].Value;

            return PictureString;


        }

        public string GetFollowersTweetFollowingCount(string user)
        {


            HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create("https://www.twitter.com/" + user);

            Request.CookieContainer = CC;
            Request.Method = "GET";
            Request.KeepAlive = true;
            Request.UserAgent = UserA;

            HttpWebResponse Response = default(HttpWebResponse);
            Response = (HttpWebResponse)Request.GetResponse();
            StreamReader Reader = new StreamReader(Response.GetResponseStream());
            string Result = Reader.ReadToEnd();

            string Clear;
            string TweetCount;
            string FollowerCount;
            string FollowingCount;
            string AllCount;

            Match Match = Regex.Match(Result, ";followers_count&quot;:(.+?),&quot;");
            Match Match1 = Regex.Match(Result, ";friends_count&quot;:(.+)?,&quot;");
            Match Match2 = Regex.Match(Result, ";statuses_count&quot;:(.+)?,&quot;");

            int index = Match1.Groups[1].Value.IndexOf(",");Clear = Match1.Groups[1].Value;Clear = Clear.Substring(0, index);
            FollowingCount = Clear;
            int index2 = Match2.Groups[1].Value.IndexOf(",");Clear = Match2.Groups[1].Value; Clear = Clear.Substring(0, index);
            TweetCount = Clear;
            FollowerCount = Match.Groups[1].Value;

            AllCount = TweetCount + '/' + FollowingCount + '/' + FollowerCount;
                // This can be split on the main form by using
                //string[] lol;
                //lol = le.Split('/');
                //Do somthing with lol[1-3];

            return AllCount;

        }

        public object Logout()
        {
            if (SignedIn == false)
                return "Not signed in";
            else
            {
                string PostData = "authenticity_token=" + Token + "&reliability_event=&scribe_log=";
                byte[] ByteData = encodings.GetBytes(PostData);

                HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create("https://twitter.com/logout");
                Request.CookieContainer = logincookie;
                Request.Method = "POST";
                Request.KeepAlive = true;
                Request.ContentType = "application/x-www-form-urlencoded";
                Request.UserAgent = UserA;
                Request.Referer = "https://twitter.com/";
                Request.ContentLength = ByteData.Length;

                Stream PostStream = Request.GetRequestStream();
                PostStream.Write(ByteData, 0, ByteData.Length);
                PostStream.Close();

                HttpWebResponse Response = default(HttpWebResponse);
                Response = (HttpWebResponse)Request.GetResponse();
                StreamReader Reader = new StreamReader(Response.GetResponseStream());
                string Result = Reader.ReadToEnd();

                if (Result.Contains("Sign in to Twitter"))
                    SignedIn = false;

                return Result;
            }
        }

        public object ChangeTrendLocation(string Country)
        {
            string WOEID;
            if (Country == "WorldWide" | Country == "Worldwide" | Country == "worldwide")
                WOEID = "1";
            else
            {
                HttpWebRequest WOIDRequest = (HttpWebRequest)HttpWebRequest.Create("http://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20geo.places%20where%20text%3D%22" + Country + "%22&format=xml");
                WOIDRequest.Method = "GET";
                WOIDRequest.UserAgent = UserA;
                WOIDRequest.KeepAlive = true;

                HttpWebResponse WOIDResponse = default(HttpWebResponse);
                WOIDResponse = (HttpWebResponse)WOIDRequest.GetResponse();
                StreamReader WOIDReader = new StreamReader(WOIDResponse.GetResponseStream());
                string Source = WOIDReader.ReadToEnd();

                Match Matches = Regex.Match(Source, "<country code=\"(.+)?\" type=\"Country\" woeid=\"(.+)?\">New Zealand</country>");

                WOEID = Matches.Groups[2].Value;
            }
            if (SignedIn == false)
                return "Not signed in";
            {

                string PostData = "authenticity_token=" + Token + "&pc=true&woeid=" + WOEID;
                byte[] ByteData = encodings.GetBytes(PostData);

                HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create("https://twitter.com/trends/dialog");
                Request.CookieContainer = logincookie;
                Request.Method = "POST";
                Request.KeepAlive = true;
                Request.ContentType = "application/x-www-form-urlencoded";
                Request.UserAgent = UserA;
                Request.ContentLength = ByteData.Length;
                Stream PostStream = Request.GetRequestStream();
                PostStream.Write(ByteData, 0, ByteData.Length);
                PostStream.Close();

                HttpWebResponse Response = default(HttpWebResponse);
                Response = (HttpWebResponse)Request.GetResponse();
                StreamReader Reader = new StreamReader(Response.GetResponseStream());
                string Result = Reader.ReadToEnd();
                if (Result.Contains("countryName"))
                    ChangedTrends = true;
                else
                    ChangedTrends = false;

                return Result;
            }
        }

    }
}
