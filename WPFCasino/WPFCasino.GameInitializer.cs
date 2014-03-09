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
    class Initializer
    {
        public MemoryStream[] serizlizedTables = new MemoryStream[10];
        CardStack tempStack = new CardStack();
        int[] values = new[] { 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        Dictionary<char, int> images = new Dictionary<char, int>();
        Dictionary<string, ImageBrush> imageStack = new Dictionary<string, ImageBrush>();
        char[] imagesM = new[] { 'C', 'P', 'B', 'H' };
        
        public CardStack InitCards()
        {
            images['T'] = 11;
            images['V'] = 2;
            images['Q'] = 3;
            images['K'] = 4;

            for (int j = 0; j < imagesM.Length; j++)
            {
                for (int i = 0; i < values.Length; i++)
                {

                    ImageBrush backForCanvas = new ImageBrush() { ImageSource = new BitmapImage(new Uri(String.Format(@"\CardsImage\{0}{1}.png", values[i], imagesM[j]), UriKind.Relative)) };
                    tempStack.AddCard(new Card(values[i], String.Format("{0}{1}", values[i], imagesM[j])));
                }
                foreach (KeyValuePair<char, int> item in images)
                {

                    ImageBrush backForCanvas = new ImageBrush() { ImageSource = new BitmapImage(new Uri(String.Format(@"\CardsImage\{0}{1}.png", item.Key, imagesM[j]), UriKind.Relative)) };
                    tempStack.AddCard(new Card(item.Value, String.Format("{0}{1}", item.Key, imagesM[j])));
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
                    ImageBrush backForCanvas = new ImageBrush() { ImageSource = new BitmapImage(new Uri(String.Format(@"CardsImage\{0}{1}.png", values[i], imagesM[j]), UriKind.Relative)) };
                    imageStack.Add(String.Format("{0}{1}", values[i], imagesM[j]), backForCanvas);
                }
                foreach (KeyValuePair<char, int> item in images)
                {
                    ImageBrush backForCanvas = new ImageBrush() { ImageSource = new BitmapImage(new Uri(String.Format(@"CardsImage\{0}{1}.png", item.Key, imagesM[j]), UriKind.Relative)) };
                    imageStack.Add(String.Format("{0}{1}", item.Key, imagesM[j]), backForCanvas);
                }
            }
            return imageStack;
        }

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
}
