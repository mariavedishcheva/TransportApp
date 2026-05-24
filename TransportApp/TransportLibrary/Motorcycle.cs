using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace TransportLibrary
{
    // Класс Motorcycle (мотоцикл) - наследник класса Transport
    public class Motorcycle : Transport
    {
        // Поле: наличие коляски (true - есть, false - нет)
        private bool hasSidecar;

        // Свойство для доступа к полю hasSidecar
        public bool HasSidecar
        {
            get { return hasSidecar; }
            set { hasSidecar = value; }
        }

        // Конструктор по умолчанию
        public Motorcycle() : base()
        {
            HasSidecar = false;
        }

        // Конструктор с параметрами
        public Motorcycle(string brand, string number, double maxSpeed, bool hasSidecar)
            : base(brand, number, maxSpeed)
        {
            HasSidecar = hasSidecar;
        }

        // Реализация метода GetLoadCapacity
        // Если есть коляска - 0.2 тонны, иначе 0
        public override double GetLoadCapacity()
        {
            if (HasSidecar)
                return 0.2;
            else
                return 0;
        }

        // Реализация метода GetInfo
        // Добавляет информацию о наличии коляски
        public override string GetInfo()
        {
            string sidecarStr = HasSidecar ? "с коляской" : "без коляски";
            return string.Format("Мотоцикл | {0} | {1} | {2} км/ч | {3} | {4} т",
                Brand, Number, MaxSpeed, sidecarStr, GetLoadCapacity());
        }
    }
}
