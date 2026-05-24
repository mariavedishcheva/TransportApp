using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace TransportLibrary
{
    // Класс Truck (грузовик) - наследник класса Transport
    public class Truck : Transport
    {
        // Базовая грузоподъёмность (без прицепа)
        private double baseLoadCapacity;
        // Наличие прицепа
        private bool hasTrailer;

        // Свойство BaseLoadCapacity с проверкой 0-100 тонн
        public double BaseLoadCapacity
        {
            get { return baseLoadCapacity; }
            set
            {
                if (value < 0 || value > 100)
                {
                    baseLoadCapacity = 1;
                }
                else
                {
                    baseLoadCapacity = value;
                }
            }
        }

        // Свойство HasTrailer
        public bool HasTrailer
        {
            get { return hasTrailer; }
            set { hasTrailer = value; }
        }

        // Конструктор по умолчанию
        public Truck() : base()
        {
            BaseLoadCapacity = 1;
            HasTrailer = false;
        }

        // Конструктор с параметрами
        public Truck(string brand, string number, double maxSpeed, double baseLoadCapacity, bool hasTrailer)
            : base(brand, number, maxSpeed)
        {
            BaseLoadCapacity = baseLoadCapacity;
            HasTrailer = hasTrailer;
        }

        // Реализация метода GetLoadCapacity
        // Если есть прицеп - грузоподъёмность удваивается
        public override double GetLoadCapacity()
        {
            if (HasTrailer)
                return BaseLoadCapacity * 2;
            else
                return BaseLoadCapacity;
        }

        // Реализация метода GetInfo
        // Выводит базовую и итоговую грузоподъёмность
        public override string GetInfo()
        {
            string trailerStr = HasTrailer ? "с прицепом" : "без прицепа";
            return string.Format("Грузовик | {0} | {1} | {2} км/ч | {3} т (база) | {4} | {5} т (итого)",
                Brand, Number, MaxSpeed, BaseLoadCapacity, trailerStr, GetLoadCapacity());
        }
    }
}
