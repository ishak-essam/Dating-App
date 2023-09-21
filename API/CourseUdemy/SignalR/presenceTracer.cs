namespace CourseUdemy.SignalR
{
    public class presenceTracer
    {
        private static readonly Dictionary<string ,List<string>> OnLineUsers=new Dictionary<string ,List<string>>();
   
    public Task UserConnected(string username , string connnectedId )
        {
            lock ( OnLineUsers )
            {
                if ( OnLineUsers.ContainsKey (username) )
                {
                    OnLineUsers [ username ].Add (connnectedId);
                }
                else {
                    OnLineUsers.Add (username, new List<string> { connnectedId });
                }
            }

            return Task.CompletedTask;
        }
        public Task UserDisConnected ( string username, string connnectedId )
        {

            lock ( OnLineUsers )
            {
                if ( !OnLineUsers.ContainsKey (username) ) return Task.CompletedTask;
                OnLineUsers [ username ].Remove (connnectedId);
                if ( OnLineUsers [ username ].Count == 0 ) {
                    OnLineUsers.Remove (username); ;
                }
            }
            return Task.CompletedTask;

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
    }
}
