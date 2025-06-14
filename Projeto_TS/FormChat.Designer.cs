namespace Projeto_TS
{
    partial class FormChat
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormChat));
            this.labelNomeCliente = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.listBoxMensagens = new System.Windows.Forms.ListBox();
            this.buttonSair = new System.Windows.Forms.Button();
            this.btnEnviar = new System.Windows.Forms.Button();
            this.textBoxMensagem = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // labelNomeCliente
            // 
            this.labelNomeCliente.AutoSize = true;
            this.labelNomeCliente.Font = new System.Drawing.Font("Microsoft Sans Serif", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNomeCliente.Location = new System.Drawing.Point(178, 9);
            this.labelNomeCliente.Name = "labelNomeCliente";
            this.labelNomeCliente.Size = new System.Drawing.Size(252, 38);
            this.labelNomeCliente.TabIndex = 0;
            this.labelNomeCliente.Text = "Nome do cliente";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(-50, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(1573, 29);
            this.label2.TabIndex = 1;
            this.label2.Text = "---------------------------------------------------------------------------------" +
    "--------------------------------------------------------------------------------" +
    "----------------------------------";
            // 
            // listBoxMensagens
            // 
            this.listBoxMensagens.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxMensagens.FormattingEnabled = true;
            this.listBoxMensagens.ItemHeight = 22;
            this.listBoxMensagens.Location = new System.Drawing.Point(12, 74);
            this.listBoxMensagens.Name = "listBoxMensagens";
            this.listBoxMensagens.Size = new System.Drawing.Size(631, 620);
            this.listBoxMensagens.TabIndex = 2;
            this.listBoxMensagens.SelectedIndexChanged += new System.EventHandler(this.listBoxMensagens_SelectedIndexChanged);
            // 
            // buttonSair
            // 
            this.buttonSair.Image = ((System.Drawing.Image)(resources.GetObject("buttonSair.Image")));
            this.buttonSair.Location = new System.Drawing.Point(596, 12);
            this.buttonSair.Name = "buttonSair";
            this.buttonSair.Size = new System.Drawing.Size(57, 38);
            this.buttonSair.TabIndex = 3;
            this.buttonSair.UseVisualStyleBackColor = true;
            this.buttonSair.Click += new System.EventHandler(this.buttonSair_Click);
            // 
            // btnEnviar
            // 
            this.btnEnviar.Image = ((System.Drawing.Image)(resources.GetObject("btnEnviar.Image")));
            this.btnEnviar.Location = new System.Drawing.Point(539, 702);
            this.btnEnviar.Name = "btnEnviar";
            this.btnEnviar.Size = new System.Drawing.Size(104, 49);
            this.btnEnviar.TabIndex = 4;
            this.btnEnviar.UseVisualStyleBackColor = true;
            // 
            // textBoxMensagem
            // 
            this.textBoxMensagem.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxMensagem.Location = new System.Drawing.Point(13, 702);
            this.textBoxMensagem.Multiline = true;
            this.textBoxMensagem.Name = "textBoxMensagem";
            this.textBoxMensagem.Size = new System.Drawing.Size(520, 49);
            this.textBoxMensagem.TabIndex = 5;
            // 
            // FormChat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 763);
            this.Controls.Add(this.textBoxMensagem);
            this.Controls.Add(this.btnEnviar);
            this.Controls.Add(this.buttonSair);
            this.Controls.Add(this.listBoxMensagens);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelNomeCliente);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormChat";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormChat";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelNomeCliente;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listBoxMensagens;
        private System.Windows.Forms.Button buttonSair;
        private System.Windows.Forms.Button btnEnviar;
        private System.Windows.Forms.TextBox textBoxMensagem;
    }
}