using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;      // Подключаем пространство имён для Windows Forms
using TransportLibrary;          // Подключаем нашу библиотеку с классами Transport

namespace TransportApp
{
    // partial - класс разделён на два файла (логика + дизайнер)
    // Form - наследуемся от стандартного класса окна Windows
    public partial class MainForm : Form
    {
        // ПОЛЯ КЛАССА (хранят данные)

        private Project project;           // Объект, хранящий список всех транспортных средств
        private ListBox transportListBox;  // Список для отображения всех ТС
        private TextBox infoTextBox;       // Текстовое поле для подробной информации
        private Button addButton;          // Кнопка "Добавить"
        private Button editButton;         // Кнопка "Редактировать"
        private Button deleteButton;       // Кнопка "Удалить"
        private Button searchButton;       // Кнопка "Поиск"
        private Button saveButton;         // Кнопка "Сохранить"
        private Button loadButton;         // Кнопка "Загрузить"
        private TableLayoutPanel mainTableLayout;  // Табличный контейнер для всей формы
        private SplitContainer splitContainer;     // Разделитель на две панели

        // КОНСТРУКТОР (вызывается при создании формы)
        public MainForm()
        {
            InitializeComponents();   // Создаём и настраиваем все элементы управления
            project = new Project();  // Создаём пустой проект
            UpdateListBox();          // Заполняем список (сначала он будет пустым)
        }

        //МЕТОД НАСТРОЙКИ ВСЕХ ЭЛЕМЕНТОВ УПРАВЛЕНИЯ
        private void InitializeComponents()
        {
            //НАСТРОЙКА САМОЙ ФОРМЫ 
            this.Text = "Система управления транспортом";  // Заголовок окна
            this.Size = new System.Drawing.Size(900, 600); // Размер окна (ширина, высота)
            this.MinimumSize = new System.Drawing.Size(600, 400); // Минимальный размер (нельзя сделать меньше)

            //КОНТЕЙНЕР TABLELAYOUTPANEL (табличная верстка)
            // Он позволяет разбить форму на строки и столбцы
            mainTableLayout = new TableLayoutPanel();
            mainTableLayout.Dock = DockStyle.Fill;  // Растянуть на всю форму
            mainTableLayout.ColumnCount = 1;        // Одна колонка
            mainTableLayout.RowCount = 2;           // Две строки

            // Настройка строк: первая строка - 90% высоты, вторая - 10%
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 90F));
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));

            // РАЗДЕЛИТЕЛЬ SPLITCONTAINER (делит пространство на две изменяемые панели)
            // Пользователь может перетаскивать границу мышью
            splitContainer = new SplitContainer();
            splitContainer.Dock = DockStyle.Fill;       // Растянуть на всю родительскую ячейку
            splitContainer.SplitterDistance = 300;     // Ширина левой панели в пикселях
            splitContainer.SplitterWidth = 5;          // Толщина разделителя

            // ЛЕВАЯ ПАНЕЛЬ - СПИСОК ТРАНСПОРТНЫХ СРЕДСТВ
            transportListBox = new ListBox();
            transportListBox.Dock = DockStyle.Fill;                    // Растянуть на всю панель
            transportListBox.Font = new System.Drawing.Font("Consolas", 10F);  // Моноширинный шрифт
            // Подписываемся на событие "выбран другой элемент" - при клике на элемент списка
            transportListBox.SelectedIndexChanged += TransportListBox_SelectedIndexChanged;

            // ПРАВАЯ ПАНЕЛЬ - ТЕКСТОВОЕ ПОЛЕ ДЛЯ ИНФОРМАЦИИ
            infoTextBox = new TextBox();
            infoTextBox.Dock = DockStyle.Fill;          // Растянуть на всю панель
            infoTextBox.Multiline = true;               // Многострочный режим
            infoTextBox.ReadOnly = true;                // Только для чтения (пользователь не может редактировать)
            infoTextBox.Font = new System.Drawing.Font("Consolas", 10F);
            infoTextBox.ScrollBars = ScrollBars.Vertical;  // Вертикальная полоса прокрутки

            // Размещаем элементы в панелях SplitContainer
            splitContainer.Panel1.Controls.Add(transportListBox);  // Список - в левую панель
            splitContainer.Panel2.Controls.Add(infoTextBox);       // Текст - в правую панель

            // ПАНЕЛЬ КНОПОК (ещё одна таблица для 5 кнопок)
            TableLayoutPanel buttonPanel = new TableLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.ColumnCount = 5;               // 5 колонок (по одной на каждую кнопку)
            buttonPanel.RowCount = 1;                  // Одна строка

            // Каждая колонка занимает 20% ширины
            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));

            //КНОПКА "ДОБАВИТЬ"
            addButton = new Button();
            addButton.Text = "Добавить";               // Текст на кнопке
            addButton.Dock = DockStyle.Fill;           // Растянуть на всю ячейку
            addButton.Click += AddButton_Click;        // Подписка на событие клика

            //КНОПКА "РЕДАКТИРОВАТЬ" 
            editButton = new Button();
            editButton.Text = "Редактировать";
            editButton.Dock = DockStyle.Fill;
            editButton.Click += EditButton_Click;

            //КНОПКА "УДАЛИТЬ" 
            deleteButton = new Button();
            deleteButton.Text = "Удалить";
            deleteButton.Dock = DockStyle.Fill;
            deleteButton.Click += DeleteButton_Click;

            //КНОПКА "ПОИСК"
            searchButton = new Button();
            searchButton.Text = "Поиск по грузоподъёмности";
            searchButton.Dock = DockStyle.Fill;
            searchButton.Click += SearchButton_Click;

            //КНОПКА "СОХРАНИТЬ"
            saveButton = new Button();
            saveButton.Text = "Сохранить";
            saveButton.Dock = DockStyle.Fill;
            saveButton.Click += SaveButton_Click;

            //КНОПКА "ЗАГРУЗИТЬ"
            loadButton = new Button();
            loadButton.Text = "Загрузить";
            loadButton.Dock = DockStyle.Fill;
            loadButton.Click += LoadButton_Click;

            // Добавляем кнопки в таблицу buttonPanel
            // Параметры: (кнопка, колонка, строка)
            buttonPanel.Controls.Add(addButton, 0, 0);
            buttonPanel.Controls.Add(editButton, 1, 0);
            buttonPanel.Controls.Add(deleteButton, 2, 0);
            buttonPanel.Controls.Add(searchButton, 3, 0);
            buttonPanel.Controls.Add(saveButton, 4, 0);
            buttonPanel.Controls.Add(loadButton, 5, 0);

            // Собираем всю структуру: в первую строку mainTableLayout - SplitContainer
            mainTableLayout.Controls.Add(splitContainer, 0, 0);
            // Во вторую строку mainTableLayout - панель с кнопками
            mainTableLayout.Controls.Add(buttonPanel, 0, 1);

            // Добавляем главный контейнер на форму
            this.Controls.Add(mainTableLayout);
        }

        //ОБНОВЛЕНИЕ СПИСКА ТРАНСПОРТНЫХ СРЕДСТВ 
        private void UpdateListBox()
        {
            transportListBox.Items.Clear();  // Очищаем список (удаляем все старые элементы)

            // Проходим по всем объектам в проекте
            foreach (Transport t in project.Transports)
            {
                // Добавляем в список строку, полученную от GetInfo()
                // Полиморфизм: у каждого типа своя реализация GetInfo()
                transportListBox.Items.Add(t.GetInfo());
            }
        }

        // ОБНОВЛЕНИЕ ПАНЕЛИ ИНФОРМАЦИИ 
        // index - индекс выбранного элемента в списке
        private void UpdateInfoTextBox(int index)
        {
            // Проверяем, что индекс не отрицательный и не выходит за границы списка
            if (index >= 0 && index < project.Transports.Count)
            {
                Transport t = project.Transports[index];  // Получаем выбранный объект

                // Формируем текст для отображения
                infoTextBox.Text = $"Транспортное средство №{index + 1}\r\n\r\n";

                // Определяем тип и выводим соответствующую строку
                infoTextBox.Text += $"Тип: ";
                if (t is Car) infoTextBox.Text += "Легковая машина\r\n";
                else if (t is Motorcycle) infoTextBox.Text += "Мотоцикл\r\n";
                else if (t is Truck) infoTextBox.Text += "Грузовик\r\n";

                // Общие поля для всех типов
                infoTextBox.Text += $"\r\nМарка: {t.Brand}\r\n";
                infoTextBox.Text += $"Номер: {t.Number}\r\n";
                infoTextBox.Text += $"Максимальная скорость: {t.MaxSpeed} км/ч\r\n";
                infoTextBox.Text += $"Грузоподъёмность: {t.GetLoadCapacity()} т\r\n";

                // Специфические поля для мотоцикла (если текущий объект - мотоцикл)
                if (t is Motorcycle m)
                {
                    infoTextBox.Text += $"Наличие коляски: {(m.HasSidecar ? "Да" : "Нет")}\r\n";
                }
                // Специфические поля для грузовика (если текущий объект - грузовик)
                else if (t is Truck tr)
                {
                    infoTextBox.Text += $"Базовая грузоподъёмность: {tr.BaseLoadCapacity} т\r\n";
                    infoTextBox.Text += $"Наличие прицепа: {(tr.HasTrailer ? "Да" : "Нет")}\r\n";
                }
            }
            else
            {
                // Если ничего не выбрано - выводим подсказку
                infoTextBox.Text = "Выберите транспортное средство из списка";
            }
        }

        // ОБРАБОТЧИК ВЫБОРА ЭЛЕМЕНТА В СПИСКЕ
        // Срабатывает, когда пользователь кликает по элементу списка
        private void TransportListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Вызываем обновление панели информации для выбранного элемента
            UpdateInfoTextBox(transportListBox.SelectedIndex);
        }

        // ОБРАБОТЧИК КНОПКИ "ДОБАВИТЬ" 
        private void AddButton_Click(object sender, EventArgs e)
        {
            // Создаём окно редактирования. null = создаём новый объект
            EditForm editForm = new EditForm(null);

            // ShowDialog() - показываем окно в диалоговом режиме (блокирует главное окно)
            // Если пользователь нажал OK, результат DialogResult.OK
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                project.AddTransport(editForm.ResultTransport);  // Добавляем в проект
                UpdateListBox();  // Обновляем список на экране
            }
        }

        //ОБРАБОТЧИК КНОПКИ "РЕДАКТИРОВАТЬ"
        private void EditButton_Click(object sender, EventArgs e)
        {
            int index = transportListBox.SelectedIndex;  // Получаем индекс выбранного элемента

            if (index >= 0)  // Если что-то выбрано
            {
                // Создаём окно редактирования, передаём редактируемый объект
                EditForm editForm = new EditForm(project.Transports[index]);

                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    // Обновляем объект в проекте
                    project.UpdateTransport(index, editForm.ResultTransport);
                    UpdateListBox();                     // Обновляем список
                    transportListBox.SelectedIndex = index;  // Восстанавливаем выделение
                }
            }
            else
            {
                // Если ничего не выбрано - показываем предупреждение
                MessageBox.Show("Выберите транспортное средство для редактирования",
                    "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ОБРАБОТЧИК КНОПКИ "УДАЛИТЬ"
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            int index = transportListBox.SelectedIndex;

            if (index >= 0)
            {
                // Запрашиваем подтверждение у пользователя
                DialogResult result = MessageBox.Show(
                    $"Удалить транспортное средство?\n\n{project.Transports[index].GetInfo()}",
                    "Подтверждение удаления",
                    MessageBoxButtons.YesNo,      // Кнопки "Да" и "Нет"
                    MessageBoxIcon.Question);     // Значок вопроса

                if (result == DialogResult.Yes)   // Если пользователь нажал "Да"
                {
                    project.RemoveTransport(index);   // Удаляем из проекта
                    UpdateListBox();                  // Обновляем список
                    infoTextBox.Text = "Выберите транспортное средство из списка";
                }
            }
            else
            {
                MessageBox.Show("Выберите транспортное средство для удаления",
                    "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ОБРАБОТЧИК КНОПКИ "ПОИСК"
        private void SearchButton_Click(object sender, EventArgs e)
        {
            // Создаём окно поиска, передаём проект для поиска
            SearchForm searchForm = new SearchForm(project);
            searchForm.ShowDialog();  // Показываем окно
        }

        // ОБРАБОТЧИК КНОПКИ "СОХРАНИТЬ" 
        private void SaveButton_Click(object sender, EventArgs e)
        {
            // Диалог выбора файла для сохранения
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "JSON файлы (*.json)|*.json|Все файлы (*.*)|*.*";  // Фильтр типов
            saveDialog.DefaultExt = "json";    // Расширение по умолчанию
            saveDialog.FileName = "transport_data.json";  // Имя файла по умолчанию

            // Если пользователь выбрал файл и нажал "Сохранить"
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    project.SaveToFile(saveDialog.FileName);  // Сохраняем в файл
                    MessageBox.Show("Данные успешно сохранены", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)  // Если произошла ошибка
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ОБРАБОТЧИК КНОПКИ "ЗАГРУЗИТЬ" 
        private void LoadButton_Click(object sender, EventArgs e)
        {
            // Диалог выбора файла для открытия
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "JSON файлы (*.json)|*.json|Все файлы (*.*)|*.*";
            openDialog.DefaultExt = "json";
            openDialog.FileName = "transport_data.json";

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    project.LoadFromFile(openDialog.FileName);  // Загружаем из файла
                    UpdateListBox();  // Обновляем список
                    infoTextBox.Text = "Данные загружены. Выберите транспортное средство из списка";
                    MessageBox.Show($"Загружено {project.Transports.Count} транспортных средств",
                        "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
    // Автоматический пустой обработчик
        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}