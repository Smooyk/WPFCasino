using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace WPFCasino
{
    //Класс-контроллер для текущего игрока
    class Player
    {
        int[] myCash = new int[10];
        int currentTable;
        public Player()
        {
            
        }
        public int CurrentTable { get { return currentTable; } set { currentTable = value;} }
    }
}
