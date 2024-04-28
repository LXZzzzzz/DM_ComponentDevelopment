using System;

namespace DefaultRole
{
    public class DataBase
    {
        public Action<string> OnDataChanged;

        public virtual void ToData(string str) { }
    }
}
