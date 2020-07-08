namespace Messenger.Front {
	partial class MainForm {
		/// <summary>
		/// Обязательная переменная конструктора.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Код, автоматически созданный конструктором форм Windows

		/// <summary>
		/// Требуемый метод для поддержки конструктора — не изменяйте 
		/// содержимое этого метода с помощью редактора кода.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.ClientsContainer = new System.Windows.Forms.FlowLayoutPanel();
			this.DialogueContainer = new System.Windows.Forms.FlowLayoutPanel();
			this.MessageTextBox = new System.Windows.Forms.TextBox();
			this.FileAttachButton = new System.Windows.Forms.Button();
			this.NetworkUpdateTimer = new System.Windows.Forms.Timer(this.components);
			this.DeliverTo = new System.Windows.Forms.Label();
			this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.SuspendLayout();
			// 
			// ClientsContainer
			// 
			this.ClientsContainer.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.ClientsContainer.Location = new System.Drawing.Point(13, 13);
			this.ClientsContainer.Name = "ClientsContainer";
			this.ClientsContainer.Size = new System.Drawing.Size(245, 375);
			this.ClientsContainer.TabIndex = 0;
			// 
			// DialogueContainer
			// 
			this.DialogueContainer.AutoScroll = true;
			this.DialogueContainer.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.DialogueContainer.Location = new System.Drawing.Point(264, 13);
			this.DialogueContainer.Name = "DialogueContainer";
			this.DialogueContainer.Size = new System.Drawing.Size(524, 375);
			this.DialogueContainer.TabIndex = 1;
			this.DialogueContainer.WrapContents = false;
			// 
			// MessageTextBox
			// 
			this.MessageTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.MessageTextBox.Location = new System.Drawing.Point(265, 395);
			this.MessageTextBox.Name = "MessageTextBox";
			this.MessageTextBox.Size = new System.Drawing.Size(478, 26);
			this.MessageTextBox.TabIndex = 2;
			this.MessageTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MessageTextBox_KeyDown);
			// 
			// FileAttachButton
			// 
			this.FileAttachButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.FileAttachButton.Location = new System.Drawing.Point(750, 395);
			this.FileAttachButton.Name = "FileAttachButton";
			this.FileAttachButton.Size = new System.Drawing.Size(38, 26);
			this.FileAttachButton.TabIndex = 3;
			this.FileAttachButton.Text = "+";
			this.FileAttachButton.UseVisualStyleBackColor = true;
			this.FileAttachButton.Click += new System.EventHandler(this.FileAttachButton_Click);
			// 
			// NetworkUpdateTimer
			// 
			this.NetworkUpdateTimer.Enabled = true;
			this.NetworkUpdateTimer.Interval = 1000;
			this.NetworkUpdateTimer.Tick += new System.EventHandler(this.NetworkUpdateTimer_Tick);
			// 
			// DeliverTo
			// 
			this.DeliverTo.AutoSize = true;
			this.DeliverTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.DeliverTo.Location = new System.Drawing.Point(13, 395);
			this.DeliverTo.Name = "DeliverTo";
			this.DeliverTo.Size = new System.Drawing.Size(174, 20);
			this.DeliverTo.TabIndex = 4;
			this.DeliverTo.Text = "Выберите получателя";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 433);
			this.Controls.Add(this.DeliverTo);
			this.Controls.Add(this.FileAttachButton);
			this.Controls.Add(this.MessageTextBox);
			this.Controls.Add(this.DialogueContainer);
			this.Controls.Add(this.ClientsContainer);
			this.Name = "MainForm";
			this.Text = "MainForm";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel ClientsContainer;
		private System.Windows.Forms.FlowLayoutPanel DialogueContainer;
		private System.Windows.Forms.TextBox MessageTextBox;
		private System.Windows.Forms.Button FileAttachButton;
		private System.Windows.Forms.Timer NetworkUpdateTimer;
		private System.Windows.Forms.Label DeliverTo;
		private System.Windows.Forms.OpenFileDialog OpenFileDialog;
	}
}

