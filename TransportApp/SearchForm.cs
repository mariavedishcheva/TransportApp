using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;      // Пространство имён для Windows Forms
using TransportLibrary;          // Наша библиотека с классами Transport и Project

namespace TransportApp
{
    // Форма для поиска транспортных средств по грузоподъёмности
    // partial - класс разделён на два файла
    // Form - наследуемся от стандартного окна Windows
    public partial class SearchForm : Form
    {
        // ПОЛЯ КЛАССА

        private Project project;           // Ссылка на проект (список всех транспортных средств)
        private TextBox minLoadTextBox;    // Поле ввода минимальной грузоподъёмности
        private Button searchButton;       // Кнопка "Найти"
        private ListBox resultListBox;     // Список для отображения результатов поиска
        private Label errorLabel;          // Метка для сообщений об ошибке

        // КОНСТРУКТОР 
        // Принимает объект Project, в котором будем искать
        public SearchForm(Project project)
        {
            this.project = project;           // Сохраняем ссылку на проект
            InitializeComponents();           // Создаём и настраиваем все элементы управления
        }

        //ИНИЦИАЛИЗАЦИЯ ВСЕХ ЭЛЕМЕНТОВ УПРАВЛЕНИЯ
        private void InitializeComponents()
        {
            // НАСТРОЙКА САМОЙ ФОРМЫ
            this.Text = "Поиск по грузоподъёмности";                    // Заголовок окна
            this.Size = new System.Drawing.Size(500, 400);              // Размер окна (ширина, высота)
            this.StartPosition = FormStartPosition.CenterParent;        // Открывать по центру родительского окна
            this.MinimumSize = new System.Drawing.Size(400, 300);       // Минимальный размер (нельзя сделать меньше)

            // ГЛАВНЫЙ КОНТЕЙНЕР TABLELAYOUTPANEL
            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;           // Растянуть на всю форму
            layout.Padding = new Padding(10);       // Отступы от краёв формы (10 пикселей со всех сторон)
            layout.ColumnCount = 2;                 // 2 колонки: слева подписи, справа поля ввода
            layout.RowCount = 4;                    // 4 строки: поле ввода, кнопка, ошибка, результаты

            // Настройка ширины колонок: 40% на подписи, 60% на поля ввода
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));

            // СТРОКА 0: ПОДПИСЬ И ПОЛЕ ВВОДА
            Label minLoadLabel = new Label();
            minLoadLabel.Text = "Минимальная грузоподъёмность (т):";    // Текст подписи
            minLoadLabel.Dock = DockStyle.Fill;                         // Растянуть на ячейку
            minLoadLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight; // Выравнивание вправо
            layout.Controls.Add(minLoadLabel, 0, 0);   // Добавляем подпись (колонка 0, строка 0)

            minLoadTextBox = new TextBox();
            minLoadTextBox.Dock = DockStyle.Fill;                       // Растянуть на ячейку
            minLoadTextBox.TextChanged += ValidateInput;                // Подписка на событие "текст изменён"
            layout.Controls.Add(minLoadTextBox, 1, 0);   // Добавляем поле ввода (колонка 1, строка 0)

            // СТРОКА 1: КНОПКА "НАЙТИ"
            searchButton = new Button();
            searchButton.Text = "Найти";                 // Текст на кнопке
            searchButton.Dock = DockStyle.Fill;          // Растянуть на ячейку
            searchButton.Enabled = false;                // Изначально кнопка неактивна (пока не введено число)
            searchButton.Click += SearchButton_Click;    // Подписка на событие клика
            layout.Controls.Add(searchButton, 1, 1);     // Добавляем кнопку (колонка 1, строка 1)

            // СТРОКА 2: МЕТКА ДЛЯ СООБЩЕНИЙ ОБ ОШИБКЕ
            errorLabel = new Label();
            errorLabel.ForeColor = System.Drawing.Color.Red;   // Красный цвет текста
            errorLabel.Text = "";                               // Изначально пустая
            errorLabel.Dock = DockStyle.Fill;
            errorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter; // Выравнивание по центру
            layout.SetColumnSpan(errorLabel, 2);                // Растянуть на обе колонки (через весь макет)
            layout.Controls.Add(errorLabel, 0, 2);   // Добавляем метку (колонка 0, строка 2)

            // СТРОКА 3: СПИСОК ДЛЯ РЕЗУЛЬТАТОВ
            resultListBox = new ListBox();
            resultListBox.Dock = DockStyle.Fill;                // Растянуть на ячейку
            resultListBox.Font = new System.Drawing.Font("Consolas", 9F);  // Моноширинный шрифт
            layout.SetColumnSpan(resultListBox, 2);             // Растянуть на обе колонки
            layout.Controls.Add(resultListBox, 0, 3);           // Добавляем список (колонка 0, строка 3)

            // Добавляем главный контейнер на форму
            this.Controls.Add(layout);
        }

        // ВАЛИДАЦИЯ ВВОДА 
        // Проверяет, что в поле ввода введено положительное число
        private void ValidateInput(object sender, EventArgs e)
        {
            // double.TryParse - пытается преобразовать текст в число
            // out double value - результат преобразования (если успешно)
            // value >= 0 - проверка, что число неотрицательное
            if (double.TryParse(minLoadTextBox.Text, out double value) && value >= 0)
            {
                // Если введено корректное положительное число:
                minLoadTextBox.BackColor = System.Drawing.Color.White;   // Белый фон (всё правильно)
                errorLabel.Text = "";                                    // Очищаем сообщение об ошибке
                searchButton.Enabled = true;                             // Включаем кнопку "Найти"
            }
            else
            {
                // Если введено не число или отрицательное число:
                minLoadTextBox.BackColor = System.Drawing.Color.LightSalmon; // Красный фон (ошибка)
                errorLabel.Text = "Введите положительное число";             // Показываем сообщение
                searchButton.Enabled = false;                                // Отключаем кнопку
            }
        }

        // ОБРАБОТЧИК КНОПКИ "НАЙТИ"
        // Выполняет поиск и отображает результаты
        private void SearchButton_Click(object sender, EventArgs e)
        {
            // Получаем минимальную грузоподъёмность из текстового поля
            // Ошибки не будет, так как кнопка активна только при корректном вводе
            double minLoad = double.Parse(minLoadTextBox.Text);

            // Вызываем метод поиска из класса Project
            // SearchByLoadCapacity возвращает список транспортных средств с грузоподъёмностью >= minLoad
            var results = project.SearchByLoadCapacity(minLoad);

            // Очищаем список результатов от предыдущего поиска
            resultListBox.Items.Clear();

            // Проверяем, найдены ли результаты
            if (results.Count == 0)
            {
                // Если ничего не найдено - выводим сообщение
                resultListBox.Items.Add("Транспортные средства не найдены");
            }
            else
            {
                // Выводим заголовок с количеством найденных элементов
                resultListBox.Items.Add($"Найдено {results.Count} транспортных средств с грузоподъёмностью >= {minLoad} т");
                resultListBox.Items.Add("");   // Пустая строка для разделения

                // Проходим по всем найденным объектам
                foreach (Transport t in results)
                {
                    // Добавляем информацию о каждом транспортном средстве
                    // t.GetInfo() - полиморфный метод (разный для Car, Motorcycle, Truck)
                    resultListBox.Items.Add(t.GetInfo());
                }
            }
        }
    }
}