using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace WPFCasino
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Table displayedTable;
        Player player = new Player();
        Initializer myGame = new Initializer();
        Dictionary<string, ImageBrush> cardsImages;
        List<Label> playerCardLabels = new List<Label>();
        //Initialize components on Program start
        public MainWindow()
        {
            InitializeComponent();
            playerCardLabels.Add(firstCardLbl);
            playerCardLabels.Add(secondCardLbl);
            playerCardLabels.Add(thirdCardLbl);
            playerCardLabels.Add(fourtCardLbl);
        }
        //EvenHandler for Game Start
        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            cardsImages = myGame.GetImageStack();
            myCanvas.Children.RemoveRange(0, 2);
            ImageBrush backForCanvas = new ImageBrush() { ImageSource = new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "Images/cardBack.jpg")) };
            myCanvas.Background = backForCanvas;
            for (int i = 0; i < 10; i++)
            {
                CardStack temp = (new Initializer()).InitCards();
                temp.Shuffle();
                myGame.serizlizedTables[i] = Initializer.SerializeTable(new Table(temp));
            }
            displayedTable = Initializer.DeserializeTable(myGame.serizlizedTables[0]);
            InitTableButton();
            InitSwitchButton();
            InitBetBox();
            dealBtn.Visibility = System.Windows.Visibility.Visible;
            anotherCardBtn.Visibility = System.Windows.Visibility.Visible;
            pasBtn.Visibility = System.Windows.Visibility.Visible;
            playerCardsGrid.Visibility = System.Windows.Visibility.Visible;

        }
       

        //EventHandler for Switch button
        private void switchButton_Click(object sender, RoutedEventArgs e)
        {
            if (player.CurrentTable  < 9)
            {
                player.CurrentTable += 1;
                myGame.serizlizedTables[player.CurrentTable - 1] = Initializer.SerializeTable(displayedTable);
                displayedTable = Initializer.DeserializeTable(myGame.serizlizedTables[player.CurrentTable]);
                InitTableButton();
                DrowCards();
                winLoseText.Text = displayedTable.Message;
                if (!displayedTable.IsDealed)
                {
                    displayedTable.ShuffleCards();
                }
            }
            else
            {
                player.CurrentTable = 0;
                myGame.serizlizedTables[9] = Initializer.SerializeTable(displayedTable);
                displayedTable = Initializer.DeserializeTable(myGame.serizlizedTables[player.CurrentTable]);
                InitTableButton();
                DrowCards();
                winLoseText.Text = displayedTable.Message;
                if (!displayedTable.IsDealed)
                {
                    displayedTable.ShuffleCards();
                }
                
            }
            
            
        }
        //EventHandler for Deal button
        private void dealBtn__Click(object sender, RoutedEventArgs e)
            {
                if (!displayedTable.IsDealed && displayedTable.Bank > 0)
                {
                    displayedTable.RefreshStack();
                    displayedTable.GiveCardsToPlayer();
                    DrowCards();
                    displayedTable.IsDealed = true;
                    winLoseText.Visibility = System.Windows.Visibility.Hidden;
                    InitTableButton();
                }
                
            }
        //EventHandler for Another card button
        private void anotherCardBtn_Click(object sender, RoutedEventArgs e)
            {
                if (displayedTable.IsDealed == true && displayedTable.ShowCards(true).Count < 4)
                {
                    displayedTable.GiveCardsToPlayer();
                    if (displayedTable.ShowCards(true).Count == 3)
                    {
                        DrowCards();
                    }
                    else if (displayedTable.ShowCards(true).Count == 4)
                    {
                        DrowCards();
                    }
                    InitTableButton();
                }


            }
        //EvenHandler for Pass button
        
        private void pasBtn_Click(object sender, RoutedEventArgs e)
            {
                if (displayedTable.IsDealed)
                {
                    displayedTable.IsDealed = false;
                    int playerResult = displayedTable.GetResult(displayedTable.ShowCards(true));
                    int pcResult = 0;
                    displayedTable.GiveCardsToPC();
                    pcResult = displayedTable.GetResult(displayedTable.ShowCards(false));
                    if (pcResult < 21)
                    {
                        displayedTable.GiveCardsToPC();
                     
                    }
                    pcResult = displayedTable.GetResult(displayedTable.ShowCards(false));
                    if (pcResult < 18)
                    {
                        displayedTable.GiveCardsToPC();
                    }
                    pcResult = displayedTable.GetResult(displayedTable.ShowCards(false));
                    int gameResult = displayedTable.CalculateResult(playerResult, pcResult);
                    switch (gameResult)
                    { 
                        case(1):
                            winLoseText.Visibility = System.Windows.Visibility.Visible;
                            winLoseText.Text = displayedTable.Message;

                            break;
                        case(-1):
                            winLoseText.Visibility = System.Windows.Visibility.Visible;
                            winLoseText.Text = displayedTable.Message;

                            break;
                        case(0):
                            winLoseText.Visibility = System.Windows.Visibility.Visible;
                            winLoseText.Text = displayedTable.Message;

                            break;
                    }
                    InitTableButton();
                    displayedTable.ShowCards(true).Clear();
                    displayedTable.ShowCards(false).Clear();
                    DrowCards();
                    DrowCards();
                    
                }
            }
        
        //Methods to initialize state for buttons objects
        private void InitTableButton()
        {
            if (currentTable.Visibility == System.Windows.Visibility.Visible)
            {
                currentTable.Text = String.Format("My table is - {0}\nMy money is - {1}\nCurrent computer money - {2}\nCurrent bank is - {3}\nMy points is - {4}\nPC Points is - {5}",
                    player.CurrentTable, displayedTable.CurrentPlayerMoney, displayedTable.CurrentTableMoney, displayedTable.Bank, displayedTable.GetResult(displayedTable.ShowCards(true)), displayedTable.GetResult(displayedTable.ShowCards(false)));
            }
            else
            {
                currentTable.Visibility = System.Windows.Visibility.Visible;
                currentTable.Text = String.Format("My table is - {0}\nMy money is - {1}\nCurrent computer money - {2}\nCurrent bank is - {3}\nMy points is - {4}\nPC Points is - {5}",
                    player.CurrentTable, displayedTable.CurrentPlayerMoney, displayedTable.CurrentTableMoney, displayedTable.Bank, displayedTable.GetResult(displayedTable.ShowCards(true)), displayedTable.GetResult(displayedTable.ShowCards(false)));
            }
         
        }
        private void InitSwitchButton()
        {
            switchButton.Visibility = System.Windows.Visibility.Visible;
            switchButton.Content = WPFCasinoConstants.switchText;

        }
        private void InitBetBox()
        {
            betBox.Visibility = System.Windows.Visibility.Visible;
            betBox.Text = "0";
        }
        private void betBox_KeyDown_1(object sender, KeyEventArgs e)
        
        {
            if (e.Key == Key.Enter && !displayedTable.IsDealed)
            {
                int amount = Convert.ToInt32(((TextBox)sender).Text);
                displayedTable.MakeBet(amount);
                InitTableButton();
            }
        }

        //Allow user to enter digits only in Bet box 
        private void betBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }
        //Draw cards of player
        public void DrowCards()
        {
            if (displayedTable.ShowCards(true).Count > 0)
            {
                for (int i = 0; i < displayedTable.ShowCards(true).Count; i++)
                {
                    if (i < 4)
                    {
                        playerCardLabels[i].Background = cardsImages[displayedTable.ShowCards(true)[i].CardName];
                    }
                }
            }
            else
            {
                foreach (Label item in playerCardLabels)
                {
                    item.Background = new ImageBrush();                    
                }
            }
        }



        



        



        
    }
}
