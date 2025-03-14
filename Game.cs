namespace WebApplication10
{
    public class Game
    {
        public string ID { get; set; }
        public string Player_1 { get; set; }
        public string Player_2 { get; set; }
        public string Player_3 { get; set; }
        public string Player_4 { get; set; }
        public string Turn { get; set; }
        public List<Word> Word {  get; set; }
        public string Question { get; set; }
        public string CurrentWord { get; set; } = "";
    }
    public class Word
    {
        public char Char { get; set; }
        public bool Guess { get; set; }
    }
}