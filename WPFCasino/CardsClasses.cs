using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace WPFCasino
{
    //Iinitalize card stacks for game
    [Serializable]
    class Initializer
    {
        public MemoryStream[] serizlizedTables = new MemoryStream[10];
        CardStack tempStack = new CardStack();
        int[] values = new [] { 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        Dictionary<char, int> images = new Dictionary<char, int>();
        Dictionary<string, ImageBrush> imageStack = new Dictionary<string,ImageBrush>();
        char[] imagesM = new[] { 'C','P','B','H'};
        public CardStack InitCards()
        {
            images['T'] = 11;
            images['V'] = 2;
            images['Q'] = 3;
            images['K'] = 4;
            
            for(int j =0; j < imagesM.Length;j++)
            {
            for (int i = 0; i < values.Length; i++)
            {
                
                ImageBrush backForCanvas = new ImageBrush() { ImageSource = new BitmapImage(new Uri(String.Format(@"\CardsImage\{0}{1}.png", values[i],imagesM[j]), UriKind.Relative)) };
                tempStack.AddCard(new Card(values[i], String.Format("{0}{1}", values[i],imagesM[j])));
            }
            foreach (KeyValuePair<char,int> item in images)
            {
                
                ImageBrush backForCanvas = new ImageBrush() { ImageSource = new BitmapImage(new Uri(String.Format(@"\CardsImage\{0}{1}.png",item.Key,imagesM[j]),UriKind.Relative))};
                tempStack.AddCard(new Card(item.Value,String.Format("{0}{1}",item.Key,imagesM[j])));
            }
            
        }
            return tempStack;
    }
        public Dictionary<string, ImageBrush> GetImageStack()
        {
            images['T'] = 11;
            images['V'] = 2;
            images['Q'] = 3;
            images['K'] = 4;
            for (int j = 0; j < imagesM.Length; j++)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    ImageBrush backForCanvas = new ImageBrush() { ImageSource = new BitmapImage(new Uri(String.Format(@"CardsImage\{0}{1}.png", values[i],imagesM[j]), UriKind.Relative)) };
                    imageStack.Add(String.Format("{0}{1}", values[i],imagesM[j]), backForCanvas);
                }
                foreach (KeyValuePair<char, int> item in images)
                {
                    ImageBrush backForCanvas = new ImageBrush() { ImageSource = new BitmapImage(new Uri(String.Format(@"CardsImage\{0}{1}.png", item.Key,imagesM[j]), UriKind.Relative)) };
                    imageStack.Add(String.Format("{0}{1}", item.Key,imagesM[j]), backForCanvas);
                }
            }
            return imageStack;
        }
                
        //Methods used to serialize/deserizlize tables - need to be removed to Cards level
        public static MemoryStream SerializeTable(Table table)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream temp = new MemoryStream();
            formatter.Serialize(temp, table);
            return temp;

        }
        public static Table DeserializeTable(MemoryStream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            stream.Seek(0, SeekOrigin.Begin);
            Table temp = (Table)formatter.Deserialize(stream);
            return temp;
        }

    }
    [Serializable]

    class Card
    {
        int cardValue;
        string cardName;


        public int CardValue { get { return cardValue; } private set { cardValue = value;} }
        public string CardName { get { return cardName; } set { cardName = value;} }
        public Card()
        {

            CardValue = 0;
            CardName = "";
        }
        public Card( int valueForCard, string nameForCard)
        {
            CardValue = valueForCard;
            CardName = nameForCard;
        }
    }

    [Serializable]
    class CardStack
    {
        Stack<Card> allCards = new Stack<Card>();
        public Card GiveCard()
        {
            if (allCards.Count > 0)
                return allCards.Pop();
            else 
                throw new Exception("No cards!");
        }
        public void AddCard(Card card)
        {
            allCards.Push(card);
        }
        public void Shuffle()
        { 
            List<Card> temporaryForShuffle = new List<Card>(allCards.ToArray<Card>());
            Random randomizer = new Random();
            for (int i = temporaryForShuffle.Count-1; i > 1; i--)
            {
                int j = randomizer.Next(i+1);
                Card temp = temporaryForShuffle[j];
                temporaryForShuffle[j] = temporaryForShuffle[i];
                temporaryForShuffle[i] = temp;
            }
            temporaryForShuffle.OrderBy(i => randomizer.Next());
            allCards = new Stack<Card>(temporaryForShuffle);
        }
    }
    //Класс для сущности - Игровой Стол
    [Serializable]
    class Table
    {
        CardStack currentStack;
        List<Card> onPlayerHands = new List<Card>();
        List<Card> onComputerHands = new List<Card>();
        int currentPlayerMoney;
        bool inPlay;
        int currentTableMoney;
        int tableNumber;
        int bank = 0;
        bool isDealed = false;
        string message = "";
        public Table(CardStack stack)
        {
            currentStack = stack;
            CurrentTableMoney = 100;
            CurrentPlayerMoney = 100;
        }
        public int TableNumber { get { return tableNumber; } set { tableNumber = value;} }
        public bool InPlay { get { return inPlay; } set { inPlay = value;} }
        public int CurrentPlayerMoney { get { return currentPlayerMoney; } set { currentPlayerMoney = value;} }
        public int CurrentTableMoney { get { return currentTableMoney; } set { currentTableMoney = value;} }
        public int Bank { get { return bank; } set { bank = value;} }
        public bool IsDealed { get { return isDealed; } set { isDealed = value;} }
        public string Message { get { return message; } set { message = value;} }
        public void GiveCardsToPlayer()
        {
            try
            {
                if (!IsDealed)
                {
                    onPlayerHands.Add(currentStack.GiveCard());
                    onPlayerHands.Add(currentStack.GiveCard());
                }
                else
                {
                    onPlayerHands.Add(currentStack.GiveCard());
                }
            }
            catch
            {
                Console.WriteLine();
            }
        }
        public void GiveCardsToPC()
        {
            try
            {
                if (!IsDealed)
                {
                    onComputerHands.Add(currentStack.GiveCard());
                    onComputerHands.Add(currentStack.GiveCard());
                }
                else
                {
                    onComputerHands.Add(currentStack.GiveCard());
                }
            }
            catch
            {
                Console.WriteLine();
            }
        }
        public bool MakeBet(int amount)
        {
            if (CurrentTableMoney >= amount && CurrentPlayerMoney >= amount)
            {
                CurrentTableMoney -= amount;
                CurrentPlayerMoney -= amount;
                Bank += amount * 2;
                return true;
            }
            else
                return false;

        }
        public List<Card> ShowCards(bool playerCards)
        {
            if (playerCards)
            {
                return onPlayerHands;
            }
            else
                return onComputerHands;
        }
        public int GetResult(List<Card> cardsonHand)
        {
            int result = 0;
            foreach (Card item in cardsonHand)
            {
                if (item.CardName == "TC" || item.CardName == "TB" || item.CardName == "TP" || item.CardName == "TH")
                {
                    if (result + 11 <= 21)
                    {
                        result += item.CardValue;
                        break;
                    }
                    else
                    {
                        result += 1;
                        break;
                    }
                }

                result += item.CardValue;
            }
            return result;
        }
        internal int CalculateResult(int playerRes, int pcRes)
        {
            if (playerRes > 21)
            {
                currentTableMoney += Bank;
                Bank = 0;
                message = "You lose!";
                return -1;
            }
            if (pcRes > 21 && playerRes <= 21)
            {
                currentPlayerMoney += Bank;
                Bank = 0;
                message = "You won!";
                return 1;
            }
            if (playerRes > pcRes && pcRes <= 21)
            {
                currentPlayerMoney += Bank;
                Bank = 0;
                message = "You won!";
                return 1;
            }
            else if (pcRes > playerRes && pcRes <= 21)
            {
                currentTableMoney += Bank;
                Bank = 0;
                message = "You lose!";
                return -1;
            }
            else 
            {
                currentTableMoney += Bank/2;
                CurrentPlayerMoney += Bank/2;
                message = "Dead heat!";
                Bank = 0;
                return 0;
            }
        }
        public void RefreshStack()
        { 
            currentStack = (new Initializer()).InitCards();
            currentStack.Shuffle();
        }
        public void ShuffleCards()
        {
            currentStack.Shuffle();
        }

    }

}
