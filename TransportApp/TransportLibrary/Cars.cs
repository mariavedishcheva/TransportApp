using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace TransportLibrary
{
    // Класс Car (легковая машина) - наследник класса Transport
    public class Car : Transport
    {
        // Закрытое поле для хранения грузоподъёмности
        private double loadCapacity;

        // Свойство LoadCapacity с проверкой диапазона 0-100 тонн
        public double LoadCapacity
        {
            get { return loadCapacity; }
            set
            {
                if (value < 0 || value > 100)
                {
                    loadCapacity = 1;  // Значение по умолчанию
                }
                else
                {
                    loadCapacity = value;
                }
            }
        }

        // Конструктор по умолчанию
        // base() вызывает конструктор базового класса Transport
        public Car() : base()
        {
            LoadCapacity = 1;
        }

        // Конструктор с параметрами
        public Car(string brand, string number, double maxSpeed, double loadCapacity)
            : base(brand, number, maxSpeed)
        {
            LoadCapacity = loadCapacity;
        }

        // Реализация абстрактного метода GetLoadCapacity
        // Для легковой машины просто возвращаем значение LoadCapacity
        public override double GetLoadCapacity()
        {
            return LoadCapacity;
        }

        // Реализация абстрактного метода GetInfo
        // Формирует строку с информацией о легковой машине
        public override string GetInfo()
        {
            // string.Format - форматирует строку с подстановкой значений
            return string.Format("Легковая | {0} | {1} | {2} км/ч | {3} т", Brand, Number, MaxSpeed, LoadCapacity);
        }
    }
}
