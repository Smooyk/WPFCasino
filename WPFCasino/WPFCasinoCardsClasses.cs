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
        public void GiveCardsToPC()
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
            {
                return false;
            }

        }
        public List<Card> ShowCards(bool playerCards)
        {
            if (playerCards)
            {
                return onPlayerHands;
            }
            else
            {
                return onComputerHands;
            }
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
                message = WPFCasinoConstants.loseTest;
                return -1;
            }
            else if (pcRes > 21 && playerRes <= 21)
            {
                currentPlayerMoney += Bank;
                Bank = 0;
                message = WPFCasinoConstants.wonText;
                return 1;
            }
            else if (playerRes > pcRes && pcRes <= 21)
            {
                currentPlayerMoney += Bank;
                Bank = 0;
                message = WPFCasinoConstants.wonText;
                return 1;
            }
            else if (pcRes > playerRes && pcRes <= 21)
            {
                currentTableMoney += Bank;
                Bank = 0;
                message = WPFCasinoConstants.loseTest;
                return -1;
            }
            else 
            {
                currentTableMoney += Bank/2;
                CurrentPlayerMoney += Bank/2;
                message = WPFCasinoConstants.deadHeat;
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
