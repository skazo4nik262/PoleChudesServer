using System.Threading.Tasks.Dataflow;

namespace WebApplication10
{
    public class Room
    {
        public static List<string> Words = new List<string>
        {
            "яблоко",
            "машина",
            "программирование",
            "книга",
            "кошка",
            "солнце",
            "путешествие",
            "музыка",
            "компьютер",
            "друзья"
        };


        public static List<string> Questions = new List<string>
        {
            "Какой фрукт красного цвета?",
            "Какой транспорт используется для передвижения по дороге?",
            "Как называется процесс создания программ?",
            "Что можно читать для получения знаний?",
            "Какое домашнее животное часто ловит мышей?",
            "Что светит нам днём?",
            "Что мы делаем, когда хотим увидеть новые места?",
            "Какой вид искусства может быть классическим или современным?",
            "Какое устройство помогает нам работать и развлекаться?",
            "Кто поддерживает нас в трудные времена?"
        };

        public List<string> Players { get; set; } = new List<string>();
        Dictionary<string, Game> Games { get; set; } = new Dictionary<string, Game>();


        public void AddNewClient(string nick)
        {
            if (Players.Count < 3)
                Players.Add(nick);
            else
            {
                Players.Add(nick);
                StartNewGame(Players);

            }
        }


        Action<string, string, string, string, string> proc;
        public void SetStart(Action<string, string, string, string, string> proc)
        {
            this.proc = proc;
        }

        private void StartNewGame(List<string> players)
        {
            int word = new Random().Next(11);
            List<Word> currentWord = new();
            foreach (var ch in Words[word])
            {
                currentWord.Add(new Word() { Char = ch, Guess = false });
            }

            var game = new Game() { Player_1 = players[0], Player_2 = players[1], Player_3 = players[2], Player_4 = players[3], Word = currentWord, Question = Questions[word], ID = Guid.NewGuid().ToString() };
            Games.Add(game.ID, game);
            proc(game.Player_1, game.Player_2, game.Player_3, game.Player_4, game.ID);
        }

        public List<Word> GetGameWord(string gameId)
        {
            return Games[gameId].Word;
        }

        public Turn GetNextPlayer(Turn turn)
        {
            if (turn.CurrentPlayer == Games[turn.GameId].Player_1)
            {
                turn.CurrentPlayer = Games[turn.GameId].Player_2;
            }
            else if (turn.CurrentPlayer == Games[turn.GameId].Player_2)
            {
                turn.CurrentPlayer = Games[turn.GameId].Player_3;
            }
            else if (turn.CurrentPlayer == Games[turn.GameId].Player_3)
            {
                turn.CurrentPlayer = Games[turn.GameId].Player_4;
            }
            else if (turn.CurrentPlayer == Games[turn.GameId].Player_4)
            {
                turn.CurrentPlayer = Games[turn.GameId].Player_1;
            }
            return turn;
        }

        public List<Word> GetCurrentGameWord(string gameId)
        {
            return Games[gameId].Word;
        }

        public bool MakeTurn(Turn turn)
        {
            var game = Games[turn.GameId];

            bool res = false;

            foreach (var i in game.Word)
            {
                if (i.Char.ToString().ToLower() == turn.Char.ToLower())
                {
                    i.Guess = true;
                    res = true;
                }
                else continue;
            }
            return res;
        }

        public bool Winner(string gameId)
        {
            var game = Games[gameId];
            int i = 0;
            foreach (var word in game.Word)
                if (word.Guess)
                    i++;
            if (game.Word.Count == i)
                return true;
            else
                return false;
        }

        public void ClearGame(string gameId)
        {
            Games.Remove(gameId);
        }

        public List<string> GetWinPlAndLosePl(string currentPlayer)
        {
            List<string> dict = new List<string>();
            foreach (var player in Players)
                if (player != currentPlayer)
                    dict.Add(player);
            return dict;
        }
    }
}