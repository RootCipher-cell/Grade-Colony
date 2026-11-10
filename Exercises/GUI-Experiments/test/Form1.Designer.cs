namespace test
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            label1 = new Label();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Font = new Font("Microsoft YaHei Light", 10.8F, FontStyle.Italic);
            button1.Location = new Point(387, 246);
            button1.Name = "button1";
            button1.Size = new Size(276, 62);
            button1.TabIndex = 0;
            button1.Text = "Confirm";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft YaHei Light", 10.8F, FontStyle.Italic);
            label1.Location = new Point(173, 109);
            label1.Name = "label1";
            label1.Size = new Size(144, 24);
            label1.TabIndex = 1;
            label1.Text = "enter your name";
            // 
            // textBox1
            // 
            textBox1.Font = new Font("Microsoft YaHei Light", 10.8F, FontStyle.Italic);
            textBox1.Location = new Point(375, 113);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(125, 31);
            textBox1.TabIndex = 2;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(594, 117);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(125, 27);
            textBox2.TabIndex = 3;
            textBox2.TextChanged += textBox2_TextChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImageLayout = ImageLayout.Center;
            ClientSize = new Size(800, 450);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Controls.Add(button1);
            Cursor = Cursors.IBeam;
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label label1;
        private TextBox textBox1;
        private TextBox textBox2;
    }
}
