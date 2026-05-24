using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace TransportLibrary
{
    // Класс Project - управляет коллекцией транспорта
    public class Project
    {
        // Список всех транспортных средств
        private List<Transport> transports;

        // Свойство для доступа к списку
        public List<Transport> Transports
        {
            get { return transports; }
            set { transports = value ?? new List<Transport>(); }  // Если value = null, создаём пустой список
        }

        // Конструктор - создаёт пустой список
        public Project()
        {
            transports = new List<Transport>();
        }

        // Добавление транспорта в список
        public void AddTransport(Transport transport)
        {
            if (transport != null)
            {
                transports.Add(transport);
            }
        }

        // Удаление транспорта по индексу
        public void RemoveTransport(int index)
        {
            if (index >= 0 && index < transports.Count)
            {
                transports.RemoveAt(index);
            }
        }

        // Обновление транспорта по индексу
        public void UpdateTransport(int index, Transport transport)
        {
            if (index >= 0 && index < transports.Count && transport != null)
            {
                transports[index] = transport;
            }
        }

        // Сохранение списка в JSON файл
        public void SaveToFile(string filename)
        {
            try
            {
                // Настройки сериализации: красивое форматирование (с отступами)
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                // Добавляем наш конвертер для полиморфной сериализации
                options.Converters.Add(new TransportConverter());
                // Преобразуем список в JSON строку
                string json = JsonSerializer.Serialize(transports, options);
                // Записываем строку в файл
                File.WriteAllText(filename, json);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Ошибка сохранения: {0}", ex.Message));
            }
        }

        // Загрузка списка из JSON файла
        public void LoadFromFile(string filename)
        {
            try
            {
                // Проверяем, существует ли файл
                if (File.Exists(filename))
                {
                    // Читаем JSON строку из файла
                    string json = File.ReadAllText(filename);
                    var options = new JsonSerializerOptions();
                    options.Converters.Add(new TransportConverter());
                    // Преобразуем JSON строку обратно в список
                    transports = JsonSerializer.Deserialize<List<Transport>>(json, options) ?? new List<Transport>();
                }
                else
                {
                    transports = new List<Transport>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Ошибка загрузки: {0}", ex.Message));
            }
        }

        // Поиск транспортных средств с грузоподъёмностью >= minLoad
        public List<Transport> SearchByLoadCapacity(double minLoad)
        {
            List<Transport> result = new List<Transport>();
            foreach (Transport t in transports)
            {
                if (t.GetLoadCapacity() >= minLoad)
                {
                    result.Add(t);
                }
            }
            return result;
        }
    }

    // Конвертер для полиморфной сериализации Transport
    // Позволяет правильно сохранять и загружать объекты разных типов (Car, Motorcycle, Truck)
    public class TransportConverter : JsonConverter<Transport>
    {
        // Проверяет, может ли этот конвертер обработать указанный тип
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(Transport).IsAssignableFrom(typeToConvert);
        }

        // Чтение объекта из JSON (десериализация)
        public override Transport Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Парсим JSON в объект JsonDocument
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                JsonElement root = doc.RootElement;
                // Читаем поле Type, которое мы добавили при сохранении
                string type = root.GetProperty("Type").GetString();

                // В зависимости от значения Type создаём объект соответствующего класса
                if (type == "Car")
                {
                    Car car = new Car();
                    car.Brand = root.GetProperty("Brand").GetString();
                    car.Number = root.GetProperty("Number").GetString();
                    car.MaxSpeed = root.GetProperty("MaxSpeed").GetDouble();
                    car.LoadCapacity = root.GetProperty("LoadCapacity").GetDouble();
                    return car;
                }
                else if (type == "Motorcycle")
                {
                    Motorcycle bike = new Motorcycle();
                    bike.Brand = root.GetProperty("Brand").GetString();
                    bike.Number = root.GetProperty("Number").GetString();
                    bike.MaxSpeed = root.GetProperty("MaxSpeed").GetDouble();
                    bike.HasSidecar = root.GetProperty("HasSidecar").GetBoolean();
                    return bike;
                }
                else if (type == "Truck")
                {
                    Truck truck = new Truck();
                    truck.Brand = root.GetProperty("Brand").GetString();
                    truck.Number = root.GetProperty("Number").GetString();
                    truck.MaxSpeed = root.GetProperty("MaxSpeed").GetDouble();
                    truck.BaseLoadCapacity = root.GetProperty("BaseLoadCapacity").GetDouble();
                    truck.HasTrailer = root.GetProperty("HasTrailer").GetBoolean();
                    return truck;
                }
                else
                {
                    throw new NotSupportedException(string.Format("Тип {0} не поддерживается", type));
                }
            }
        }

        // Запись объекта в JSON (сериализация)
        public override void Write(Utf8JsonWriter writer, Transport value, JsonSerializerOptions options)
        {
            // Записываем начало JSON объекта {
            writer.WriteStartObject();

            if (value is Car car)
            {
                // Записываем поле Type для идентификации типа при загрузке
                writer.WriteString("Type", "Car");
                writer.WriteString("Brand", car.Brand);
                writer.WriteString("Number", car.Number);
                writer.WriteNumber("MaxSpeed", car.MaxSpeed);
                writer.WriteNumber("LoadCapacity", car.LoadCapacity);
            }
            else if (value is Motorcycle bike)
            {
                writer.WriteString("Type", "Motorcycle");
                writer.WriteString("Brand", bike.Brand);
                writer.WriteString("Number", bike.Number);
                writer.WriteNumber("MaxSpeed", bike.MaxSpeed);
                writer.WriteBoolean("HasSidecar", bike.HasSidecar);
            }
            else if (value is Truck truck)
            {
                writer.WriteString("Type", "Truck");
                writer.WriteString("Brand", truck.Brand);
                writer.WriteString("Number", truck.Number);
                writer.WriteNumber("MaxSpeed", truck.MaxSpeed);
                writer.WriteNumber("BaseLoadCapacity", truck.BaseLoadCapacity);
                writer.WriteBoolean("HasTrailer", truck.HasTrailer);
            }
            else
            {
                throw new NotSupportedException(string.Format("Тип {0} не поддерживается", value.GetType().Name));
            }

            // Записываем конец JSON объекта }
            writer.WriteEndObject();
        }
    }
}
