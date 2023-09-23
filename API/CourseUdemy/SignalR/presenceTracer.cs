namespace CourseUdemy.SignalR
{
    public class presenceTracer
    {
        private static readonly Dictionary<string ,List<string>> OnLineUsers=new Dictionary<string ,List<string>>();
   
    public Task<bool> UserConnected(string username , string connnectedId )
        {
            bool IsOnline=false;
            lock ( OnLineUsers )
            {
                if ( OnLineUsers.ContainsKey (username) )
                {
                    OnLineUsers [ username ].Add (connnectedId);
                }
                else {
                    OnLineUsers.Add (username, new List<string> { connnectedId });
                    IsOnline = true;
                }
            }

            return Task.FromResult(IsOnline);
        }
        public Task<bool> UserDisConnected ( string username, string connnectedId )
        {
            bool IsOfilne=false;
            lock ( OnLineUsers )
            {
                if ( !OnLineUsers.ContainsKey (username) ) return Task.FromResult(IsOfilne);
                OnLineUsers [ username ].Remove (connnectedId);
                if ( OnLineUsers [ username ].Count == 0 ) {
                    OnLineUsers.Remove (username); ;
                    IsOfilne = true;
                }
            }
            return Task.FromResult(IsOfilne);

        }
        public Task<string [ ]> GetOnlineUser ( )
        {
            string[] onlinusers;
            lock ( OnLineUsers )
            {
                onlinusers = OnLineUsers.OrderBy (k => k.Key).Select (k => k.Key).ToArray ();
            }
            return Task.FromResult(onlinusers);
        }
        public static Task<List<string>> GetConnectionForUser ( string Username) {
            List<string>ConnectionIds;
            lock ( OnLineUsers )
            {
                ConnectionIds = OnLineUsers.GetValueOrDefault (Username);

            }
            return Task.FromResult (ConnectionIds);
        }

    }
}
