using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TransportLibrary
{
    // Абстрактный класс Transport - базовый для всех видов транспорта
    public abstract class Transport
    {
        // Создаем поля
        protected string brand;
        protected string number;
        protected double maxSpeed;

        // Свойство Brand (марка) с проверкой на пустое значение
        // get читает значение
        // set устанавливает значение
        public string Brand
        {
            get { return brand; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    brand = "Неизвестно";  // Устанавливаем значение по умолчанию
                }
                else
                {
                    brand = value;  // Устанавливаем переданное значение
                }
            }
        }

        public string Number
        {
            get { return number; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    number = "0000";
                }
                else
                {
                    number = value;
                }
            }
        }

        public double MaxSpeed
        {
            get { return maxSpeed; }
            set
            {
                if (value < 0 || value > 400)
                {
                    maxSpeed = 50;  // Значение по умолчанию при ошибке
                }
                else
                {
                    maxSpeed = value;
                }
            }
        }

        // Конструктор по умолчанию - вызывается при создании объекта без параметров
        // Устанавливает значения по умолчанию
        public Transport()
        {
            Brand = "Неизвестно";
            Number = "0000";
            MaxSpeed = 50;
        }

        // Конструктор с параметрами - вызывается при создании объекта с данными
        // Сначала задаются значения по умолчанию, затем через свойства - переданные
        public Transport(string brand, string number, double maxSpeed)
        {
            this.brand = "Неизвестно";
            this.number = "0000";
            this.maxSpeed = 50;
            Brand = brand;
            Number = number;
            MaxSpeed = maxSpeed;
        }

        // Абстрактные методы
        // Возвращает грузоподъёмность с учётом особенностей транспорта
        public abstract double GetLoadCapacity();
        // Возвращает строку с полной информацией о транспорте
        public abstract string GetInfo();
    }
}
