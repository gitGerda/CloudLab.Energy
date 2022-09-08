namespace KZMP_ENERGY
{
    partial class FormPowerProfile
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPowerProfile));
            this.panel1 = new System.Windows.Forms.Panel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.CheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AddrressColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iconPictureBox1 = new FontAwesome.Sharp.IconPictureBox();
            this.InterfaceTypeLbl = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cb_monthEnergy = new System.Windows.Forms.CheckBox();
            this.comboBox_Months = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.iconPictureBox5 = new FontAwesome.Sharp.IconPictureBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.iconPictureBox4 = new FontAwesome.Sharp.IconPictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.iconButton2 = new FontAwesome.Sharp.IconButton();
            this.TimeoutTextBox2 = new System.Windows.Forms.TextBox();
            this.iconButton1 = new FontAwesome.Sharp.IconButton();
            this.StatusTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.iconPictureBox3 = new FontAwesome.Sharp.IconPictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.iconPictureBox2 = new FontAwesome.Sharp.IconPictureBox();
            this.timePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.datePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.timePickerStart = new System.Windows.Forms.DateTimePicker();
            this.datePickerStart = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.richTextBox_conStatus2 = new System.Windows.Forms.RichTextBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox2)).BeginInit();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.richTextBox1);
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.Controls.Add(this.iconPictureBox1);
            this.panel1.Controls.Add(this.InterfaceTypeLbl);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.ForeColor = System.Drawing.Color.White;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(842, 155);
            this.panel1.TabIndex = 0;
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(33)))), ((int)(((byte)(74)))));
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.ForeColor = System.Drawing.Color.Lime;
            this.richTextBox1.Location = new System.Drawing.Point(11, 66);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(230, 57);
            this.richTextBox1.TabIndex = 7;
            this.richTextBox1.Text = "Внимание: если дата последнего измерения больше даты начала интервала, то опрос с" +
    "четчика начнется с даты последнего измерения.";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CheckBoxColumn,
            this.Type,
            this.AddrressColumn,
            this.Column1});
            this.dataGridView1.Location = new System.Drawing.Point(250, 19);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 46;
            this.dataGridView1.Size = new System.Drawing.Size(528, 136);
            this.dataGridView1.TabIndex = 6;
            // 
            // CheckBoxColumn
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.NullValue = false;
            this.CheckBoxColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.CheckBoxColumn.HeaderText = "+";
            this.CheckBoxColumn.MinimumWidth = 6;
            this.CheckBoxColumn.Name = "CheckBoxColumn";
            this.CheckBoxColumn.Width = 30;
            // 
            // Type
            // 
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            this.Type.DefaultCellStyle = dataGridViewCellStyle2;
            this.Type.HeaderText = "Тип счётчика";
            this.Type.MinimumWidth = 6;
            this.Type.Name = "Type";
            this.Type.Width = 130;
            // 
            // AddrressColumn
            // 
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            this.AddrressColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.AddrressColumn.HeaderText = "Сетевой адрес";
            this.AddrressColumn.MinimumWidth = 6;
            this.AddrressColumn.Name = "AddrressColumn";
            this.AddrressColumn.Width = 80;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
            this.Column1.DefaultCellStyle = dataGridViewCellStyle4;
            this.Column1.HeaderText = "Последнее измерение";
            this.Column1.MinimumWidth = 6;
            this.Column1.Name = "Column1";
            // 
            // iconPictureBox1
            // 
            this.iconPictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(33)))), ((int)(((byte)(74)))));
            this.iconPictureBox1.IconChar = FontAwesome.Sharp.IconChar.TachometerAlt;
            this.iconPictureBox1.IconColor = System.Drawing.Color.White;
            this.iconPictureBox1.IconSize = 40;
            this.iconPictureBox1.Location = new System.Drawing.Point(14, 11);
            this.iconPictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.iconPictureBox1.Name = "iconPictureBox1";
            this.iconPictureBox1.Size = new System.Drawing.Size(50, 40);
            this.iconPictureBox1.TabIndex = 5;
            this.iconPictureBox1.TabStop = false;
            // 
            // InterfaceTypeLbl
            // 
            this.InterfaceTypeLbl.AutoSize = true;
            this.InterfaceTypeLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.InterfaceTypeLbl.ForeColor = System.Drawing.Color.White;
            this.InterfaceTypeLbl.Location = new System.Drawing.Point(68, 19);
            this.InterfaceTypeLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.InterfaceTypeLbl.Name = "InterfaceTypeLbl";
            this.InterfaceTypeLbl.Size = new System.Drawing.Size(143, 17);
            this.InterfaceTypeLbl.TabIndex = 3;
            this.InterfaceTypeLbl.Text = "Выберите счетчики:";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.cb_monthEnergy);
            this.panel2.Controls.Add(this.comboBox_Months);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Controls.Add(this.iconPictureBox5);
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Controls.Add(this.textBox2);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.iconPictureBox4);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.iconButton2);
            this.panel2.Controls.Add(this.TimeoutTextBox2);
            this.panel2.Controls.Add(this.iconButton1);
            this.panel2.Controls.Add(this.StatusTextBox);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.iconPictureBox3);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.iconPictureBox2);
            this.panel2.Controls.Add(this.timePickerEnd);
            this.panel2.Controls.Add(this.datePickerEnd);
            this.panel2.Controls.Add(this.timePickerStart);
            this.panel2.Controls.Add(this.datePickerStart);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 155);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(842, 204);
            this.panel2.TabIndex = 1;
            // 
            // cb_monthEnergy
            // 
            this.cb_monthEnergy.AutoSize = true;
            this.cb_monthEnergy.Checked = true;
            this.cb_monthEnergy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_monthEnergy.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cb_monthEnergy.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.cb_monthEnergy.Location = new System.Drawing.Point(70, 118);
            this.cb_monthEnergy.Name = "cb_monthEnergy";
            this.cb_monthEnergy.Size = new System.Drawing.Size(110, 19);
            this.cb_monthEnergy.TabIndex = 27;
            this.cb_monthEnergy.Text = "Использовать";
            this.cb_monthEnergy.UseVisualStyleBackColor = true;
            // 
            // comboBox_Months
            // 
            this.comboBox_Months.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBox_Months.FormattingEnabled = true;
            this.comboBox_Months.Items.AddRange(new object[] {
            "1. Январь",
            "2. Февраль",
            "3. Март",
            "4. Апрель",
            "5. Май",
            "6. Июнь",
            "7. Июль",
            "8. Август",
            "9. Сентябрь",
            "10. Октябрь",
            "11. Ноябрь",
            "12. Декабрь"});
            this.comboBox_Months.Location = new System.Drawing.Point(250, 98);
            this.comboBox_Months.Name = "comboBox_Months";
            this.comboBox_Months.Size = new System.Drawing.Size(230, 24);
            this.comboBox_Months.TabIndex = 26;
            this.comboBox_Months.Text = "Выберите месяц";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(65, 97);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(187, 17);
            this.label10.TabIndex = 25;
            this.label10.Text = "Энергия (кВт*ч) за период:";
            // 
            // iconPictureBox5
            // 
            this.iconPictureBox5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(33)))), ((int)(((byte)(74)))));
            this.iconPictureBox5.IconChar = FontAwesome.Sharp.IconChar.UniversalAccess;
            this.iconPictureBox5.IconColor = System.Drawing.Color.White;
            this.iconPictureBox5.IconSize = 40;
            this.iconPictureBox5.Location = new System.Drawing.Point(11, 96);
            this.iconPictureBox5.Margin = new System.Windows.Forms.Padding(2);
            this.iconPictureBox5.Name = "iconPictureBox5";
            this.iconPictureBox5.Size = new System.Drawing.Size(50, 40);
            this.iconPictureBox5.TabIndex = 24;
            this.iconPictureBox5.TabStop = false;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(33)))), ((int)(((byte)(74)))));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox1.ForeColor = System.Drawing.Color.Yellow;
            this.textBox1.Location = new System.Drawing.Point(626, 122);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(175, 14);
            this.textBox1.TabIndex = 23;
            this.textBox1.Text = "(............)";
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(33)))), ((int)(((byte)(74)))));
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox2.ForeColor = System.Drawing.Color.Plum;
            this.textBox2.Location = new System.Drawing.Point(626, 100);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(175, 14);
            this.textBox2.TabIndex = 22;
            this.textBox2.Text = "(............)";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(544, 122);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 15);
            this.label8.TabIndex = 21;
            this.label8.Text = "Результат:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(544, 99);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(47, 15);
            this.label9.TabIndex = 20;
            this.label9.Text = "Ответ:";
            // 
            // iconPictureBox4
            // 
            this.iconPictureBox4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(33)))), ((int)(((byte)(74)))));
            this.iconPictureBox4.IconChar = FontAwesome.Sharp.IconChar.FreeCodeCamp;
            this.iconPictureBox4.IconColor = System.Drawing.Color.White;
            this.iconPictureBox4.IconSize = 56;
            this.iconPictureBox4.Location = new System.Drawing.Point(483, 92);
            this.iconPictureBox4.Margin = new System.Windows.Forms.Padding(2);
            this.iconPictureBox4.Name = "iconPictureBox4";
            this.iconPictureBox4.Size = new System.Drawing.Size(73, 56);
            this.iconPictureBox4.TabIndex = 19;
            this.iconPictureBox4.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Underline);
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(561, 75);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(194, 17);
            this.label7.TabIndex = 18;
            this.label7.Text = "Проверка хеш-сумм ответов";
            // 
            // iconButton2
            // 
            this.iconButton2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.iconButton2.FlatAppearance.BorderSize = 0;
            this.iconButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconButton2.Flip = FontAwesome.Sharp.FlipOrientation.Normal;
            this.iconButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.iconButton2.ForeColor = System.Drawing.Color.White;
            this.iconButton2.IconChar = FontAwesome.Sharp.IconChar.TimesCircle;
            this.iconButton2.IconColor = System.Drawing.Color.Gainsboro;
            this.iconButton2.IconSize = 38;
            this.iconButton2.Location = new System.Drawing.Point(246, 146);
            this.iconButton2.Margin = new System.Windows.Forms.Padding(2);
            this.iconButton2.Name = "iconButton2";
            this.iconButton2.Padding = new System.Windows.Forms.Padding(8, 0, 15, 0);
            this.iconButton2.Rotation = 0D;
            this.iconButton2.Size = new System.Drawing.Size(233, 46);
            this.iconButton2.TabIndex = 6;
            this.iconButton2.Text = "Отключить";
            this.iconButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.iconButton2.UseVisualStyleBackColor = false;
            this.iconButton2.Click += new System.EventHandler(this.iconButton2_Click);
            // 
            // TimeoutTextBox2
            // 
            this.TimeoutTextBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(33)))), ((int)(((byte)(74)))));
            this.TimeoutTextBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TimeoutTextBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TimeoutTextBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.TimeoutTextBox2.Location = new System.Drawing.Point(624, 50);
            this.TimeoutTextBox2.Name = "TimeoutTextBox2";
            this.TimeoutTextBox2.Size = new System.Drawing.Size(154, 14);
            this.TimeoutTextBox2.TabIndex = 17;
            this.TimeoutTextBox2.Text = "(............)";
            // 
            // iconButton1
            // 
            this.iconButton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.iconButton1.FlatAppearance.BorderSize = 0;
            this.iconButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconButton1.Flip = FontAwesome.Sharp.FlipOrientation.Normal;
            this.iconButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.iconButton1.ForeColor = System.Drawing.Color.White;
            this.iconButton1.IconChar = FontAwesome.Sharp.IconChar.Download;
            this.iconButton1.IconColor = System.Drawing.Color.Gainsboro;
            this.iconButton1.IconSize = 38;
            this.iconButton1.Location = new System.Drawing.Point(8, 146);
            this.iconButton1.Margin = new System.Windows.Forms.Padding(2);
            this.iconButton1.Name = "iconButton1";
            this.iconButton1.Padding = new System.Windows.Forms.Padding(8, 0, 15, 0);
            this.iconButton1.Rotation = 0D;
            this.iconButton1.Size = new System.Drawing.Size(233, 46);
            this.iconButton1.TabIndex = 4;
            this.iconButton1.Text = "Загрузить данные";
            this.iconButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.iconButton1.UseVisualStyleBackColor = false;
            this.iconButton1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.iconButton1_MouseDown);
            // 
            // StatusTextBox
            // 
            this.StatusTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(33)))), ((int)(((byte)(74)))));
            this.StatusTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.StatusTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.StatusTextBox.ForeColor = System.Drawing.Color.LimeGreen;
            this.StatusTextBox.Location = new System.Drawing.Point(624, 28);
            this.StatusTextBox.Name = "StatusTextBox";
            this.StatusTextBox.Size = new System.Drawing.Size(154, 14);
            this.StatusTextBox.TabIndex = 16;
            this.StatusTextBox.Text = "(............)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(542, 50);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 15);
            this.label6.TabIndex = 15;
            this.label6.Text = "Задержка:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(542, 27);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(50, 15);
            this.label5.TabIndex = 14;
            this.label5.Text = "Статус:";
            // 
            // iconPictureBox3
            // 
            this.iconPictureBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(33)))), ((int)(((byte)(74)))));
            this.iconPictureBox3.IconChar = FontAwesome.Sharp.IconChar.PaperPlane;
            this.iconPictureBox3.IconColor = System.Drawing.Color.White;
            this.iconPictureBox3.IconSize = 56;
            this.iconPictureBox3.Location = new System.Drawing.Point(482, 19);
            this.iconPictureBox3.Margin = new System.Windows.Forms.Padding(2);
            this.iconPictureBox3.Name = "iconPictureBox3";
            this.iconPictureBox3.Size = new System.Drawing.Size(73, 56);
            this.iconPictureBox3.TabIndex = 13;
            this.iconPictureBox3.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Underline);
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(577, 0);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(170, 17);
            this.label4.TabIndex = 12;
            this.label4.Text = "Мониторинг соединения";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(321, 48);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Конец интервала";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(314, 7);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Начало интервала";
            // 
            // iconPictureBox2
            // 
            this.iconPictureBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(33)))), ((int)(((byte)(74)))));
            this.iconPictureBox2.IconChar = FontAwesome.Sharp.IconChar.CalendarCheck;
            this.iconPictureBox2.IconColor = System.Drawing.Color.White;
            this.iconPictureBox2.IconSize = 40;
            this.iconPictureBox2.Location = new System.Drawing.Point(12, 9);
            this.iconPictureBox2.Margin = new System.Windows.Forms.Padding(2);
            this.iconPictureBox2.Name = "iconPictureBox2";
            this.iconPictureBox2.Size = new System.Drawing.Size(50, 40);
            this.iconPictureBox2.TabIndex = 9;
            this.iconPictureBox2.TabStop = false;
            // 
            // timePickerEnd
            // 
            this.timePickerEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.timePickerEnd.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.timePickerEnd.Location = new System.Drawing.Point(377, 66);
            this.timePickerEnd.Name = "timePickerEnd";
            this.timePickerEnd.Size = new System.Drawing.Size(103, 23);
            this.timePickerEnd.TabIndex = 8;
            this.timePickerEnd.Value = new System.DateTime(2020, 9, 26, 0, 0, 0, 0);
            // 
            // datePickerEnd
            // 
            this.datePickerEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.datePickerEnd.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.datePickerEnd.Location = new System.Drawing.Point(250, 66);
            this.datePickerEnd.Name = "datePickerEnd";
            this.datePickerEnd.Size = new System.Drawing.Size(121, 23);
            this.datePickerEnd.TabIndex = 7;
            // 
            // timePickerStart
            // 
            this.timePickerStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.timePickerStart.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.timePickerStart.Location = new System.Drawing.Point(377, 25);
            this.timePickerStart.Name = "timePickerStart";
            this.timePickerStart.Size = new System.Drawing.Size(103, 23);
            this.timePickerStart.TabIndex = 6;
            this.timePickerStart.Value = new System.DateTime(2020, 9, 26, 0, 0, 0, 0);
            // 
            // datePickerStart
            // 
            this.datePickerStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.datePickerStart.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.datePickerStart.Location = new System.Drawing.Point(250, 25);
            this.datePickerStart.Name = "datePickerStart";
            this.datePickerStart.Size = new System.Drawing.Size(121, 23);
            this.datePickerStart.TabIndex = 5;
            this.datePickerStart.ValueChanged += new System.EventHandler(this.datePickerStart_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(67, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(158, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Укажите дату и время:";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(0, 614);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(842, 23);
            this.progressBar1.TabIndex = 0;
            // 
            // richTextBox_conStatus2
            // 
            this.richTextBox_conStatus2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(33)))), ((int)(((byte)(74)))));
            this.richTextBox_conStatus2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox_conStatus2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox_conStatus2.Font = new System.Drawing.Font("Consolas", 10.09346F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.richTextBox_conStatus2.ForeColor = System.Drawing.Color.Lime;
            this.richTextBox_conStatus2.Location = new System.Drawing.Point(0, 0);
            this.richTextBox_conStatus2.Margin = new System.Windows.Forms.Padding(2);
            this.richTextBox_conStatus2.Name = "richTextBox_conStatus2";
            this.richTextBox_conStatus2.ReadOnly = true;
            this.richTextBox_conStatus2.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBox_conStatus2.Size = new System.Drawing.Size(842, 249);
            this.richTextBox_conStatus2.TabIndex = 1;
            this.richTextBox_conStatus2.Text = "kzmp_energy:\\power_profile\\logs\n";
            // 
            // panel6
            // 
            this.panel6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel6.Controls.Add(this.richTextBox_conStatus2);
            this.panel6.Location = new System.Drawing.Point(0, 365);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(842, 249);
            this.panel6.TabIndex = 10;
            // 
            // FormPowerProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(33)))), ((int)(((byte)(74)))));
            this.ClientSize = new System.Drawing.Size(842, 637);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormPowerProfile";
            this.Text = "FormPowerProfile";
            this.Load += new System.EventHandler(this.FormPowerProfile_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox2)).EndInit();
            this.panel6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label InterfaceTypeLbl;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DateTimePicker datePickerStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker timePickerStart;
        private System.Windows.Forms.DateTimePicker timePickerEnd;
        private System.Windows.Forms.DateTimePicker datePickerEnd;
        private FontAwesome.Sharp.IconPictureBox iconPictureBox1;
        private FontAwesome.Sharp.IconPictureBox iconPictureBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private FontAwesome.Sharp.IconButton iconButton1;
        private FontAwesome.Sharp.IconButton iconButton2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ProgressBar progressBar1;
        public  System.Windows.Forms.RichTextBox richTextBox_conStatus2;
        private System.Windows.Forms.DataGridViewCheckBoxColumn CheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn AddrressColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Panel panel6;
        private FontAwesome.Sharp.IconPictureBox iconPictureBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TimeoutTextBox2;
        private System.Windows.Forms.TextBox StatusTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private FontAwesome.Sharp.IconPictureBox iconPictureBox4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox cb_monthEnergy;
        private System.Windows.Forms.ComboBox comboBox_Months;
        private System.Windows.Forms.Label label10;
        private FontAwesome.Sharp.IconPictureBox iconPictureBox5;
    }
}