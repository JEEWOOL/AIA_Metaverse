using FreeNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MServer
{
    internal class Program
    {
        static CProcessPacket procPacket = new CProcessPacket();
        static List<CGameUser> userlist = new List<CGameUser>();

        static void Main(string[] args)
        {
            CPacketBufferManager.initialize(2000);

            CNetworkService service = new CNetworkService();
            service.session_created_callback += on_session_created;
            service.initialize();
            service.listen("0.0.0.0", 7979, 10000);

            while (true)
            {
                string input = Console.ReadLine();
                if (input.Equals("exit"))
                    break;

                System.Threading.Thread.Sleep(1000);
            }

            SDebug.WriteLog("Server End");
        }

        static void on_session_created(CUserToken token)
        {
            CGameUser user = new CGameUser(procPacket, token);
            user.remove_user_callback += on_remove_user;

            SDebug.WriteLog($"{user} Client Connected");

            lock (userlist)
            {
                userlist.Add(user);
            }
        }
        public static void broadcast(CPacket msg)
        {
            lock (userlist)
            {
                foreach (var user in userlist)
                {
                    CPacket clone = new CPacket();
                    msg.copy_to(clone);
                    user.send(clone);
                }
            }
        }
        static void on_remove_user(CGameUser user)
        {
            SDebug.WriteLog($"{user} Client Remove");

            if (user.PLAYER != null)
            {
                CPlace gameRoom = user.PLAYER.GAME_ROOM;
                gameRoom.RemovePlayerList(user.PLAYER);
            }

            lock (userlist)
            {
                userlist.Remove(user);
            }
        }
    }
}
