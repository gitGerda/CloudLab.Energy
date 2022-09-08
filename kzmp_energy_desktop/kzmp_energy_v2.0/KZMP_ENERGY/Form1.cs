using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FontAwesome.Sharp;
using System.Runtime.InteropServices;

namespace KZMP_ENERGY
{
    
    public partial class FormMainMenu : Form
    {
        //Fields
        private IconButton currentBtn;
        private Panel leftBorderBtn;
        private Form currentChildForm;
        public FormMainMenu()
        {
            InitializeComponent();
            leftBorderBtn = new Panel();
            leftBorderBtn.Size = new Size(7, 60);
            panelMenu.Controls.Add(leftBorderBtn);
            //Form
            this.Text = string.Empty;
            this.ControlBox = false;
            this.DoubleBuffered = true;
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
            
        }

        //structs
        private struct RGBColors
        {
            public static Color color1 = Color.FromArgb(172,126,241);
            public static Color color2 = Color.FromArgb(249,118,176);
            public static Color color3 = Color.FromArgb(253,138,114);
            public static Color color4 = Color.FromArgb(95,77,221);
            public static Color color5 = Color.FromArgb(249,88,155);
            public static Color color6 = Color.FromArgb(24,161,251);
        }

        //methods
        private void ActivateButton(object senderBtn, Color color)
        {
            DisableButton();
            if (senderBtn != null)
            {
                //button
                currentBtn = (IconButton)senderBtn;
                currentBtn.BackColor = Color.FromArgb(37,36,81);
                currentBtn.ForeColor = color;
                currentBtn.TextAlign = ContentAlignment.MiddleCenter;
                currentBtn.IconColor = color;
                currentBtn.TextImageRelation = TextImageRelation.TextBeforeImage;
                currentBtn.ImageAlign = ContentAlignment.MiddleRight;
                // left border button
                //leftBorderBtn.BackColor = color;
                //leftBorderBtn.Location = new Point(0, currentBtn.Location.Y);
                //leftBorderBtn.Visible = true;
                //leftBorderBtn.BringToFront();

                iconCurrentChildForm.IconChar = currentBtn.IconChar;
                iconCurrentChildForm.IconColor = color;
                
            }
            
        }

        private void DisableButton()
        {
            if (currentBtn != null)
            {
                currentBtn.BackColor = Color.FromArgb(31,30,68);
                currentBtn.ForeColor = Color.Gainsboro;
                currentBtn.TextAlign = ContentAlignment.MiddleLeft;
                currentBtn.IconColor = Color.Gainsboro;
                currentBtn.TextImageRelation = TextImageRelation.ImageBeforeText;
                currentBtn.ImageAlign = ContentAlignment.MiddleLeft;
            }

        }

        private void OpenChildForm(Form childForm)
        {
            if (currentChildForm != null)
            {
                //open only form
                currentChildForm.Close();
            }
            currentChildForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panelDesktop.Controls.Add(childForm);
            panelDesktop.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
            TitleChildForm.Text = currentBtn.Text;
        }
        //power profile button
        private void iconButton1_Click(object sender, EventArgs e)
        {
            try
            {
                ActivateButton(sender, RGBColors.color1);
                OpenChildForm(new FormPowerProfile(this));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Данная вкладка доступна только после установления соединения.");
            }
        }
        //list of electricity meters button
        private void iconButton2_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color2);
            OpenChildForm(new FormListOfMeters2(this));
        }
        //connection parameter button
        private void iconButton3_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color3);
            OpenChildForm(new FormConnectionParameter(this));

        }
        //user support button
        /*private void iconButton4_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color6);
            OpenChildForm(new FormUserSupport());
        }
        */
        private void btnHome_Click(object sender, EventArgs e)
        {
            if (currentChildForm != null)
            {
                currentChildForm.Close();
            }
            Reset();
        }

        private void Reset()
        {
            DisableButton();
            leftBorderBtn.Visible = false;

            iconCurrentChildForm.IconChar = IconChar.Home;
            iconCurrentChildForm.IconColor = Color.MediumPurple;
            TitleChildForm.Text = "Домашняя";
        }
        //drag form
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        private void panelTitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void exitBtn_MouseDown(object sender, MouseEventArgs e)
        {
            Application.Exit();
        }

        private void maximizeBtn_MouseDown(object sender, MouseEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                WindowState = FormWindowState.Maximized;
            }
            else { WindowState = FormWindowState.Normal; }
        }

        private void minimizeBtn_MouseDown(object sender, MouseEventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        private void iconButton5_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color5);
            OpenChildForm(new FormReport());
        }

        private void iconButton6_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color6);
            OpenChildForm(new FormTableMeasure());
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color3);
            OpenChildForm(new InfoForReport());
        }
    }
}
