namespace Cesto.WinForms
{
    partial class UnexpectedErrorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UnexpectedErrorForm));
            this.titleLabel = new System.Windows.Forms.Label();
            this.problemDescriptionLabel = new System.Windows.Forms.Label();
            this.textBox = new System.Windows.Forms.TextBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.diagnosticInformationLabel = new System.Windows.Forms.Label();
            this.copyLink = new System.Windows.Forms.LinkLabel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.fillPanel = new System.Windows.Forms.Panel();
            this.restartButton = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.fillPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // titleLabel
            // 
            resources.ApplyResources(this.titleLabel, "titleLabel");
            this.titleLabel.Name = "titleLabel";
            // 
            // problemDescriptionLabel
            // 
            resources.ApplyResources(this.problemDescriptionLabel, "problemDescriptionLabel");
            this.flowLayoutPanel2.SetFlowBreak(this.problemDescriptionLabel, true);
            this.problemDescriptionLabel.Name = "problemDescriptionLabel";
            // 
            // textBox
            // 
            resources.ApplyResources(this.textBox, "textBox");
            this.textBox.Name = "textBox";
            this.textBox.ReadOnly = true;
            // 
            // closeButton
            // 
            resources.ApplyResources(this.closeButton, "closeButton");
            this.closeButton.Name = "closeButton";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.CloseButtonClick);
            // 
            // diagnosticInformationLabel
            // 
            resources.ApplyResources(this.diagnosticInformationLabel, "diagnosticInformationLabel");
            this.diagnosticInformationLabel.Name = "diagnosticInformationLabel";
            // 
            // copyLink
            // 
            resources.ApplyResources(this.copyLink, "copyLink");
            this.copyLink.Name = "copyLink";
            this.copyLink.TabStop = true;
            this.copyLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.CopyLinkLinkClicked);
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.closeButton);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // flowLayoutPanel2
            // 
            resources.ApplyResources(this.flowLayoutPanel2, "flowLayoutPanel2");
            this.flowLayoutPanel2.Controls.Add(this.titleLabel);
            this.flowLayoutPanel2.Controls.Add(this.restartButton);
            this.flowLayoutPanel2.Controls.Add(this.problemDescriptionLabel);
            this.flowLayoutPanel2.Controls.Add(this.diagnosticInformationLabel);
            this.flowLayoutPanel2.Controls.Add(this.copyLink);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            // 
            // fillPanel
            // 
            this.fillPanel.Controls.Add(this.textBox);
            resources.ApplyResources(this.fillPanel, "fillPanel");
            this.fillPanel.Name = "fillPanel";
            // 
            // restartButton
            // 
            resources.ApplyResources(this.restartButton, "restartButton");
            this.flowLayoutPanel2.SetFlowBreak(this.restartButton, true);
            this.restartButton.Name = "restartButton";
            this.restartButton.UseVisualStyleBackColor = true;
            this.restartButton.Click += new System.EventHandler(this.restartButton_Click);
            // 
            // UnexpectedErrorForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fillPanel);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "UnexpectedErrorForm";
            this.Load += new System.EventHandler(this.FormLoad);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.fillPanel.ResumeLayout(false);
            this.fillPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label problemDescriptionLabel;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Label diagnosticInformationLabel;
        private System.Windows.Forms.LinkLabel copyLink;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Panel fillPanel;
        private System.Windows.Forms.Button restartButton;
    }
}