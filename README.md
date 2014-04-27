C# & VB.NET Twitter Wrapper/API
=========

---
A simple wrapper for twitter which uses HttpWebRequests. It includes the following features.

  - Login
  - SignUp
  - Tweet
  - Direct Message
  - Follow/Unfollow an account
  - Change account settings(Change Email,ScreenName,Password etc)
  - Check if a user exist
  - Deleting,Favoriting or Retweeting a tweet
  - Get basic information about a selected user(Followers,Following etc)
  
Both C# and VB.NET classes include *full* examples of how to use the class.

Examples
--------------
 - VB.NET
 
```sh
Public WithEvents Client As New Twitter

Private Sub Login()

    With Client
    
        .Login("UserName","Password")
        If Client.SignedInTrue = True Then
            MessageBox.Show("Signed In !")
            //False as we don't want to reply to a tweet.
            .Tweet("This is my Tweet", False)
        Else
            MessageBox.Show("Login Failed")
        End If
        
    End With

End Sub
```

 - C#

```sh
        static void Main(string[] args)
        {
            Twitter.Twitter Client = new Twitter.Twitter();

            Client.Login("User","Password");
            if (Client.SignedIn == true)
            {
                Console.WriteLine("Signed In");
                Client.Tweet("My Message Again", false);
            }
            else
                Console.WriteLine("Login Failed");

            Console.ReadLine();
            
        }
```

##### PLEASE NOTE
The C# project must have these references added

 - System.Web.
 - System.Drawing.
 - System.Windows.Forms.
 - Requires Framework 4.
 
The VB.Net project must have
 - System.Web.
 - Also requires Framework 4.


System.Drawing and Forms are needed for the image uploading. If you do not require that method(ChangeImage) then you may remove it from the class but System.Web has to be added to the project for it to function.


License
----

MIT
