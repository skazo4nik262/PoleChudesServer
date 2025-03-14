using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualBasic;

namespace WebApplication10
{
    public class MyHub : Hub
    {
        private Room Room { get; }
        static Dictionary<string, ISingleClientProxy> ClientsByNickname { get; set; } = new Dictionary<string, ISingleClientProxy>();

        public MyHub(Room room)
        {
            Room = room;
            room.SetStart(async (p1, p2, p3, p4, id) =>
            {
                await ClientsByNickname[p1].SendAsync("opponents", p2, p3, p4, id);
                await ClientsByNickname[p2].SendAsync("opponents", p1, p3, p4, id);
                await ClientsByNickname[p3].SendAsync("opponents", p2, p1, p4, id);
                await ClientsByNickname[p4].SendAsync("opponents", p2, p3, p1, id);
                await Clients.All.SendAsync("gameWord", Room.GetGameWord(id));
                await ClientsByNickname[p1].SendAsync("maketurn");
            });
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("hello", "Придумай ник");
            await base.OnConnectedAsync();
        }

        public async Task Nickname(string nick)
        {
            var check = ClientsByNickname.Keys.FirstOrDefault(s => s == nick);
            if (check != null)
            {
                await Clients.Caller.SendAsync("hello", "Придумай другой ник");
                return;
            }
            else
            {
                ClientsByNickname.Add(nick, Clients.Caller);
                Room.AddNewClient(nick);
            }
        }

        public async void MakeTurn(Turn turn)
        {
            bool turnResult = Room.MakeTurn(turn);
            bool winner = Room.Winner(turn.GameId);
            if (!turnResult)
            {
                if (winner)
                    await EndGame(turn);
                else
                {
                    string next = Room.GetNextPlayer(turn).CurrentPlayer;
                    try
                    {
                        await ClientsByNickname[next].SendAsync("maketurn");
                        await Clients.All.SendAsync("currentWord", Room.GetCurrentGameWord(turn.GameId));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            else
            {
                if (winner)
                    await EndGame(turn);
                else
                {
                    await ClientsByNickname[turn.CurrentPlayer].SendAsync("maketurn");
                    await Clients.All.SendAsync("currentWord", Room.GetCurrentGameWord(turn.GameId));
                }
            }
        }

        private async Task EndGame(Turn turn)
        {
            var dict = Room.GetWinPlAndLosePl(turn.CurrentPlayer);
            await ClientsByNickname[turn.CurrentPlayer].SendAsync("gameresult", "win");
            foreach (var p in dict)
                await ClientsByNickname[p].SendAsync("gameresult", "lose");
            Room.ClearGame(turn.GameId);
        }

        public void NextGame(string answer, string nick)
        {
            if (answer == "yeap")
                Room.AddNewClient(nick);
            else
                ClientsByNickname.Remove(nick);
        }

        public List<Word> GameWord(string gameId)
        {
            return Room.GetGameWord(gameId);
        }
    }
}