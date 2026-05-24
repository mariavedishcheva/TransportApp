using System;
using System.Windows.Forms;      // Пространство имён для Windows Forms (формы, кнопки, события)
using TransportLibrary;          // Наша библиотека с классами Transport, Car, Motorcycle, Truck

namespace TransportApp
{
    // Форма для добавления и редактирования транспортного средства
    // partial - класс разделён на два файла (логика + дизайнер)
    // Form - наследуемся от стандартного окна Windows
    public partial class EditForm : Form
    {
        // ПОЛЯ КЛАССА

        private Transport editingTransport;   // Транспорт, который редактируем (null если создаём новый)
        private Transport resultTransport;    // Результат работы формы (новый или отредактированный объект)

        // Элементы управления (все создаются программно)
        private ComboBox typeComboBox;        // Выпадающий список для выбора типа (легковая/мотоцикл/грузовик)
        private TextBox brandTextBox;         // Поле ввода марки
        private TextBox numberTextBox;        // Поле ввода номера
        private TextBox speedTextBox;         // Поле ввода скорости
        private TextBox loadTextBox;          // Поле ввода грузоподъёмности (для легковой)
        private CheckBox sidecarCheckBox;     // Флажок "наличие коляски" (для мотоцикла)
        private CheckBox trailerCheckBox;     // Флажок "наличие прицепа" (для грузовика)
        private TextBox baseLoadTextBox;      // Поле ввода базовой грузоподъёмности (для грузовика)

        // Подписи (Label) для динамических полей
        private Label loadLabel;              // Подпись "Грузоподъёмность (т):"
        private Label baseLoadLabel;          // Подпись "Базовая грузоподъёмность (т):"
        private Label sidecarLabel;           // Подпись "Наличие коляски:"
        private Label trailerLabel;           // Подпись "Наличие прицепа:"

        private Button okButton;              // Кнопка OK (сохранить и закрыть)
        private Button cancelButton;          // Кнопка Отмена (закрыть без сохранения)
        private TableLayoutPanel mainLayout;  // Табличный контейнер для всей формы
        private Label errorLabel;             // Метка для отображения сообщения об ошибке

        // СВОЙСТВО
        // Публичное свойство для получения результата извне (из MainForm)
        public Transport ResultTransport
        {
            get { return resultTransport; }   // Только чтение - установить можно только внутри формы
        }

        // КОНСТРУКТОР
        // transport - если null, то создаём новый объект; если не null - редактируем существующий
        public EditForm(Transport transport)
        {
            InitializeComponents();           // Создаём и настраиваем все элементы управления
            editingTransport = transport;     // Сохраняем редактируемый объект

            if (editingTransport != null)     // Если передан существующий объект
            {
                this.Text = "Редактирование транспортного средства";  // Заголовок окна
                LoadTransportData();          // Загружаем данные объекта в поля формы
            }
            else                              // Если создаётся новый объект
            {
                this.Text = "Добавление транспортного средства";      // Заголовок окна
                typeComboBox.SelectedIndex = 0;   // По умолчанию выбираем "Легковая машина"
            }
        }

        //  ИНИЦИАЛИЗАЦИЯ ВСЕХ ЭЛЕМЕНТОВ УПРАВЛЕНИЯ 
        private void InitializeComponents()
        {
            // НАСТРОЙКА САМОЙ ФОРМЫ
            this.Size = new System.Drawing.Size(480, 500);        // Размер окна
            this.MinimumSize = new System.Drawing.Size(450, 450); // Минимальный размер
            this.StartPosition = FormStartPosition.CenterParent;  // Открывать по центру родительского окна
            this.FormBorderStyle = FormBorderStyle.FixedDialog;   // Фиксированный размер (нельзя растягивать)
            this.MaximizeBox = false;                             // Отключаем кнопку "Развернуть"

            // ГЛАВНЫЙ КОНТЕЙНЕР TABLELAYOUTPANEL
            mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;           // Растянуть на всю форму
            mainLayout.Padding = new Padding(10);       // Отступы от краёв формы (10 пикселей со всех сторон)
            mainLayout.ColumnCount = 2;                 // 2 колонки: слева подписи, справа поля ввода
            mainLayout.RowCount = 12;                   // 12 строк (запас на все поля)

            // Настройка ширины колонок: 35% на подписи, 65% на поля ввода
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));

            int row = 0;  // Текущий номер строки (будем увеличивать после добавления каждого поля)

            // СТРОКА 0: ВЫПАДАЮЩИЙ СПИСОК ТИПА ТРАНСПОРТА
            Label typeLabel = new Label();
            typeLabel.Text = "Тип транспорта:";                     // Текст подписи
            typeLabel.Dock = DockStyle.Fill;                        // Растянуть на ячейку
            typeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;  // Выравнивание текста по правому краю
            mainLayout.Controls.Add(typeLabel, 0, row);             // Добавляем подпись (колонка 0, строка row)

            typeComboBox = new ComboBox();
            typeComboBox.Dock = DockStyle.Fill;                     // Растянуть на ячейку
            typeComboBox.DropDownStyle = ComboBoxStyle.DropDownList; // Нельзя вводить свой текст, только выбирать из списка
            typeComboBox.Items.AddRange(new string[] { "Легковая машина", "Мотоцикл", "Грузовик" }); // Варианты
            typeComboBox.SelectedIndexChanged += TypeComboBox_SelectedIndexChanged; // Подписка на событие "выбор изменён"
            mainLayout.Controls.Add(typeComboBox, 1, row);          // Добавляем список (колонка 1, строка row)
            row++;  // Переходим к следующей строке

            // СТРОКА 1: ПОЛЕ "МАРКА"
            Label brandLabel = new Label();
            brandLabel.Text = "Марка:";
            brandLabel.Dock = DockStyle.Fill;
            brandLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            mainLayout.Controls.Add(brandLabel, 0, row);

            brandTextBox = new TextBox();
            brandTextBox.Dock = DockStyle.Fill;
            brandTextBox.TextChanged += ValidateInput;   // Подписка на событие "текст изменён" - проверяем ввод
            mainLayout.Controls.Add(brandTextBox, 1, row);
            row++;

            // СТРОКА 2: ПОЛЕ "НОМЕР"
            Label numberLabel = new Label();
            numberLabel.Text = "Номер:";
            numberLabel.Dock = DockStyle.Fill;
            numberLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            mainLayout.Controls.Add(numberLabel, 0, row);

            numberTextBox = new TextBox();
            numberTextBox.Dock = DockStyle.Fill;
            numberTextBox.TextChanged += ValidateInput;
            mainLayout.Controls.Add(numberTextBox, 1, row);
            row++;

            // СТРОКА 3: ПОЛЕ "МАКСИМАЛЬНАЯ СКОРОСТЬ
            Label speedLabel = new Label();
            speedLabel.Text = "Максимальная скорость (км/ч):";
            speedLabel.Dock = DockStyle.Fill;
            speedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            mainLayout.Controls.Add(speedLabel, 0, row);

            speedTextBox = new TextBox();
            speedTextBox.Dock = DockStyle.Fill;
            speedTextBox.TextChanged += ValidateNumericInput;   // Специальная проверка для чисел
            mainLayout.Controls.Add(speedTextBox, 1, row);
            row++;

            //СТРОКА 4: ПОЛЕ "ГРУЗОПОДЪЁМНОСТЬ" (только для легковой, изначально скрыто)
            loadLabel = new Label();
            loadLabel.Text = "Грузоподъёмность (т):";
            loadLabel.Dock = DockStyle.Fill;
            loadLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            loadLabel.Visible = false;   // Изначально скрыто
            mainLayout.Controls.Add(loadLabel, 0, row);

            loadTextBox = new TextBox();
            loadTextBox.Dock = DockStyle.Fill;
            loadTextBox.TextChanged += ValidateNumericInput;
            loadTextBox.Visible = false;  // Изначально скрыто
            mainLayout.Controls.Add(loadTextBox, 1, row);
            row++;

            // СТРОКА 5: ФЛАЖОК "НАЛИЧИЕ КОЛЯСКИ" (только для мотоцикла, изначально скрыто)
            sidecarLabel = new Label();
            sidecarLabel.Text = "Наличие коляски:";
            sidecarLabel.Dock = DockStyle.Fill;
            sidecarLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            sidecarLabel.Visible = false;
            mainLayout.Controls.Add(sidecarLabel, 0, row);

            sidecarCheckBox = new CheckBox();
            sidecarCheckBox.Dock = DockStyle.Fill;
            sidecarCheckBox.Text = "";    // Текст не нужен, так как подпись отдельно
            sidecarCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            sidecarCheckBox.Visible = false;
            mainLayout.Controls.Add(sidecarCheckBox, 1, row);
            row++;

            // СТРОКА 6: ПОЛЕ "БАЗОВАЯ ГРУЗОПОДЪЁМНОСТЬ" (только для грузовика, изначально скрыто)
            baseLoadLabel = new Label();
            baseLoadLabel.Text = "Базовая грузоподъёмность (т):";
            baseLoadLabel.Dock = DockStyle.Fill;
            baseLoadLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            baseLoadLabel.Visible = false;
            mainLayout.Controls.Add(baseLoadLabel, 0, row);

            baseLoadTextBox = new TextBox();
            baseLoadTextBox.Dock = DockStyle.Fill;
            baseLoadTextBox.TextChanged += ValidateNumericInput;
            baseLoadTextBox.Visible = false;
            mainLayout.Controls.Add(baseLoadTextBox, 1, row);
            row++;

            // СТРОКА 7: ФЛАЖОК "НАЛИЧИЕ ПРИЦЕПА" (только для грузовика, изначально скрыто)
            trailerLabel = new Label();
            trailerLabel.Text = "Наличие прицепа:";
            trailerLabel.Dock = DockStyle.Fill;
            trailerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            trailerLabel.Visible = false;
            mainLayout.Controls.Add(trailerLabel, 0, row);

            trailerCheckBox = new CheckBox();
            trailerCheckBox.Dock = DockStyle.Fill;
            trailerCheckBox.Text = "";
            trailerCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            trailerCheckBox.Visible = false;
            mainLayout.Controls.Add(trailerCheckBox, 1, row);
            row++;

            // СТРОКА 8: МЕТКА ДЛЯ СООБЩЕНИЙ ОБ ОШИБКЕ
            errorLabel = new Label();
            errorLabel.ForeColor = System.Drawing.Color.Red;   // Красный цвет текста
            errorLabel.Text = "";                               // Изначально пустая
            errorLabel.Dock = DockStyle.Fill;
            errorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            mainLayout.SetColumnSpan(errorLabel, 2);           // Растянуть на обе колонки
            mainLayout.Controls.Add(errorLabel, 0, row);
            row++;

            // СТРОКА 9: ПАНЕЛЬ С КНОПКАМИ OK И ОТМЕНА 
            TableLayoutPanel buttonPanel = new TableLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.ColumnCount = 2;
            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            // Кнопка OK
            okButton = new Button();
            okButton.Text = "OK";
            okButton.Dock = DockStyle.Fill;
            okButton.Click += OkButton_Click;   // Подписка на клик
            buttonPanel.Controls.Add(okButton, 0, 0);

            // Кнопка Отмена (закрывает форму без сохранения)
            cancelButton = new Button();
            cancelButton.Text = "Отмена";
            cancelButton.Dock = DockStyle.Fill;
            // Лямбда-выражение: при клике устанавливаем DialogResult.Cancel и закрываем форму
            cancelButton.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            buttonPanel.Controls.Add(cancelButton, 1, 0);

            mainLayout.SetColumnSpan(buttonPanel, 2);  // Растянуть на обе колонки
            mainLayout.Controls.Add(buttonPanel, 0, row);
            row++;

            // Добавляем главный контейнер на форму
            this.Controls.Add(mainLayout);
        }

        // ОБРАБОТЧИК: ИЗМЕНЕНИЕ ТИПА ТРАНСПОРТА 
        // Срабатывает, когда пользователь выбирает другой тип в выпадающем списке
        private void TypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateVisibility();   // Показываем/скрываем соответствующие поля
        }

        // УПРАВЛЕНИЕ ВИДИМОСТЬЮ ПОЛЕЙ
        // В зависимости от выбранного типа показываем нужные поля и скрываем ненужные
        private void UpdateVisibility()
        {
            int selectedType = typeComboBox.SelectedIndex;  // 0 - легковая, 1 - мотоцикл, 2 - грузовик

            // Поля для легковой машины (selectedType == 0)
            loadLabel.Visible = (selectedType == 0);
            loadTextBox.Visible = (selectedType == 0);

            // Поля для мотоцикла (selectedType == 1)
            sidecarLabel.Visible = (selectedType == 1);
            sidecarCheckBox.Visible = (selectedType == 1);

            // Поля для грузовика (selectedType == 2)
            baseLoadLabel.Visible = (selectedType == 2);
            baseLoadTextBox.Visible = (selectedType == 2);
            trailerLabel.Visible = (selectedType == 2);
            trailerCheckBox.Visible = (selectedType == 2);

            ValidateAll();   // Перепроверяем валидность всех полей
        }

        //ЗАГРУЗКА ДАННЫХ РЕДАКТИРУЕМОГО ОБЪЕКТА В ФОРМУ
        private void LoadTransportData()
        {
            // Пытаемся преобразовать editingTransport в Car (оператор as возвращает null, если не подходит)
            Car car = editingTransport as Car;
            if (car != null)  // Если это легковая машина
            {
                typeComboBox.SelectedIndex = 0;           // Выбираем тип "Легковая машина"
                brandTextBox.Text = car.Brand;            // Заполняем марку
                numberTextBox.Text = car.Number;          // Заполняем номер
                speedTextBox.Text = car.MaxSpeed.ToString(); // Заполняем скорость
                UpdateVisibility();                       // Показываем нужные поля
                loadTextBox.Text = car.LoadCapacity.ToString(); // Заполняем грузоподъёмность
                return;   // Выходим (дальше не проверяем)
            }

            // Пытаемся преобразовать в Motorcycle
            Motorcycle bike = editingTransport as Motorcycle;
            if (bike != null)  // Если это мотоцикл
            {
                typeComboBox.SelectedIndex = 1;           // Выбираем тип "Мотоцикл"
                brandTextBox.Text = bike.Brand;
                numberTextBox.Text = bike.Number;
                speedTextBox.Text = bike.MaxSpeed.ToString();
                UpdateVisibility();
                sidecarCheckBox.Checked = bike.HasSidecar; // Устанавливаем флажок коляски
                return;
            }

            // Пытаемся преобразовать в Truck
            Truck truck = editingTransport as Truck;
            if (truck != null)  // Если это грузовик
            {
                typeComboBox.SelectedIndex = 2;           // Выбираем тип "Грузовик"
                brandTextBox.Text = truck.Brand;
                numberTextBox.Text = truck.Number;
                speedTextBox.Text = truck.MaxSpeed.ToString();
                UpdateVisibility();
                baseLoadTextBox.Text = truck.BaseLoadCapacity.ToString(); // Заполняем базовую грузоподъёмность
                trailerCheckBox.Checked = truck.HasTrailer;               // Устанавливаем флажок прицепа
            }
        }

        // ВАЛИДАЦИЯ ОБЫЧНОГО ТЕКСТА
        // Проверяет, что текстовое поле не пустое. Если пустое - подсвечивает красным
        private void ValidateInput(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;   // Преобразуем sender (объект, вызвавший событие) в TextBox
            if (tb != null)                   // Если преобразование удалось
            {
                if (string.IsNullOrWhiteSpace(tb.Text))  // Если текст пустой или состоит из пробелов
                {
                    tb.BackColor = System.Drawing.Color.LightSalmon;  // Подсветка красным
                }
                else
                {
                    tb.BackColor = System.Drawing.Color.White;        // Обычный белый фон
                }
            }
            ValidateAll();   // Проверяем все поля вместе
        }

        // ВАЛИДАЦИЯ ЧИСЛОВОГО ВВОДА
        // Проверяет, что в поле введено число. Если не число - подсвечивает красным
        private void ValidateNumericInput(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                double value;   // Переменная для результата
                if (double.TryParse(tb.Text, out value))  // Пытаемся преобразовать текст в число
                {
                    tb.BackColor = System.Drawing.Color.White;   // Успешно - белый фон
                }
                else
                {
                    tb.BackColor = System.Drawing.Color.LightSalmon;  // Ошибка - красный
                }
            }
            ValidateAll();
        }

        // ОБЩАЯ ПРОВЕРКА ВСЕХ ПОЛЕЙ
        // Проверяет все поля формы, включает/выключает кнопку OK
        private void ValidateAll()
        {
            bool isValid = true;      // Флаг: все ли поля заполнены корректно
            string errorMessage = ""; // Сообщение об ошибке

            // Проверка марки
            if (string.IsNullOrWhiteSpace(brandTextBox.Text))
            {
                isValid = false;
                errorMessage = "Марка не может быть пустой";
            }
            // Проверка номера
            else if (string.IsNullOrWhiteSpace(numberTextBox.Text))
            {
                isValid = false;
                errorMessage = "Номер не может быть пустым";
            }
            else
            {
                // Проверка скорости
                double speed;
                if (!double.TryParse(speedTextBox.Text, out speed) || speed < 0 || speed > 400)
                {
                    isValid = false;
                    errorMessage = "Скорость должна быть числом от 0 до 400";
                }
                else
                {
                    int selectedType = typeComboBox.SelectedIndex;

                    // Проверка для легковой машины
                    if (selectedType == 0)
                    {
                        double load;
                        if (string.IsNullOrWhiteSpace(loadTextBox.Text))
                        {
                            isValid = false;
                            errorMessage = "Грузоподъёмность не может быть пустой";
                        }
                        else if (!double.TryParse(loadTextBox.Text, out load) || load < 0 || load > 100)
                        {
                            isValid = false;
                            errorMessage = "Грузоподъёмность должна быть числом от 0 до 100";
                        }
                    }
                    // Проверка для грузовика
                    else if (selectedType == 2)
                    {
                        double baseLoad;
                        if (string.IsNullOrWhiteSpace(baseLoadTextBox.Text))
                        {
                            isValid = false;
                            errorMessage = "Базовая грузоподъёмность не может быть пустой";
                        }
                        else if (!double.TryParse(baseLoadTextBox.Text, out baseLoad) || baseLoad < 0 || baseLoad > 100)
                        {
                            isValid = false;
                            errorMessage = "Базовая грузоподъёмность должна быть числом от 0 до 100";
                        }
                    }
                    // Для мотоцикла (selectedType == 1) дополнительные проверки не нужны
                }
            }

            errorLabel.Text = errorMessage;       // Показываем сообщение об ошибке
            okButton.Enabled = isValid;           // Кнопка OK активна только если все поля корректны
        }

        // ОБРАБОТЧИК КНОПКИ OK
        // Создаёт объект Transport на основе введённых данных и закрывает форму
        private void OkButton_Click(object sender, EventArgs e)
        {
            int selectedType = typeComboBox.SelectedIndex;   // Выбранный тип
            string brand = brandTextBox.Text;                 // Марка
            string number = numberTextBox.Text;               // Номер
            double speed = double.Parse(speedTextBox.Text);   // Скорость (ошибки не будет, т.к. прошли валидацию)

            try
            {
                // Создаём объект нужного типа
                if (selectedType == 0)  // Легковая машина
                {
                    double load = double.Parse(loadTextBox.Text);
                    resultTransport = new Car(brand, number, speed, load);
                }
                else if (selectedType == 1)  // Мотоцикл
                {
                    bool hasSidecar = sidecarCheckBox != null && sidecarCheckBox.Checked;
                    resultTransport = new Motorcycle(brand, number, speed, hasSidecar);
                }
                else  // Грузовик (selectedType == 2)
                {
                    double baseLoad = double.Parse(baseLoadTextBox.Text);
                    bool hasTrailer = trailerCheckBox != null && trailerCheckBox.Checked;
                    resultTransport = new Truck(brand, number, speed, baseLoad, hasTrailer);
                }

                // Устанавливаем результат OK (чтобы MainForm понял, что нужно сохранить данные)
                this.DialogResult = DialogResult.OK;
                this.Close();   // Закрываем форму
            }
            catch (Exception ex)  // Если произошла ошибка (маловероятно, но на всякий случай)
            {
                MessageBox.Show(string.Format("Ошибка: {0}", ex.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}