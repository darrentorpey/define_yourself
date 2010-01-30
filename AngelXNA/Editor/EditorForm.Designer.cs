#if WINDOWS

namespace AngelXNA.Editor
{
    partial class EditorForm
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
            this._pgObjProperties = new System.Windows.Forms.PropertyGrid();
            this.btnSaveActorDefinition = new System.Windows.Forms.Button();
            this._lblActorDefs = new System.Windows.Forms.Label();
            this._btnAddActor = new System.Windows.Forms.Button();
            this._tcActorDefinitions = new System.Windows.Forms.TabControl();
            this._tpTemplates = new System.Windows.Forms.TabPage();
            this._lbActorDefinitions = new System.Windows.Forms.ListBox();
            this._tpBaseActors = new System.Windows.Forms.TabPage();
            this._lbBaseActors = new System.Windows.Forms.ListBox();
            this._ddlSimulate = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this._tcActorDefinitions.SuspendLayout();
            this._tpTemplates.SuspendLayout();
            this._tpBaseActors.SuspendLayout();
            this.SuspendLayout();
            // 
            // _pgObjProperties
            // 
            this._pgObjProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._pgObjProperties.Location = new System.Drawing.Point(12, 39);
            this._pgObjProperties.Name = "_pgObjProperties";
            this._pgObjProperties.Size = new System.Drawing.Size(309, 362);
            this._pgObjProperties.TabIndex = 0;
            this._pgObjProperties.UseCompatibleTextRendering = true;
            // 
            // btnSaveActorDefinition
            // 
            this.btnSaveActorDefinition.Location = new System.Drawing.Point(12, 407);
            this.btnSaveActorDefinition.Name = "btnSaveActorDefinition";
            this.btnSaveActorDefinition.Size = new System.Drawing.Size(131, 23);
            this.btnSaveActorDefinition.TabIndex = 1;
            this.btnSaveActorDefinition.Text = "Save Actor Definition";
            this.btnSaveActorDefinition.UseVisualStyleBackColor = true;
            this.btnSaveActorDefinition.Click += new System.EventHandler(this.btnSaveActorDefinition_Click);
            // 
            // _lblActorDefs
            // 
            this._lblActorDefs.AutoSize = true;
            this._lblActorDefs.Location = new System.Drawing.Point(9, 440);
            this._lblActorDefs.Name = "_lblActorDefs";
            this._lblActorDefs.Size = new System.Drawing.Size(130, 13);
            this._lblActorDefs.TabIndex = 3;
            this._lblActorDefs.Text = "Available Actor Definitions";
            // 
            // _btnAddActor
            // 
            this._btnAddActor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._btnAddActor.Location = new System.Drawing.Point(15, 679);
            this._btnAddActor.Name = "_btnAddActor";
            this._btnAddActor.Size = new System.Drawing.Size(75, 23);
            this._btnAddActor.TabIndex = 4;
            this._btnAddActor.Text = "Add Actor";
            this._btnAddActor.UseVisualStyleBackColor = true;
            this._btnAddActor.Click += new System.EventHandler(this._btnAddActor_Click);
            // 
            // _tcActorDefinitions
            // 
            this._tcActorDefinitions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tcActorDefinitions.Controls.Add(this._tpTemplates);
            this._tcActorDefinitions.Controls.Add(this._tpBaseActors);
            this._tcActorDefinitions.Location = new System.Drawing.Point(15, 461);
            this._tcActorDefinitions.Name = "_tcActorDefinitions";
            this._tcActorDefinitions.SelectedIndex = 0;
            this._tcActorDefinitions.Size = new System.Drawing.Size(306, 212);
            this._tcActorDefinitions.TabIndex = 5;
            // 
            // _tpTemplates
            // 
            this._tpTemplates.Controls.Add(this._lbActorDefinitions);
            this._tpTemplates.Location = new System.Drawing.Point(4, 22);
            this._tpTemplates.Name = "_tpTemplates";
            this._tpTemplates.Padding = new System.Windows.Forms.Padding(3);
            this._tpTemplates.Size = new System.Drawing.Size(298, 186);
            this._tpTemplates.TabIndex = 0;
            this._tpTemplates.Text = "Actor Templates";
            this._tpTemplates.UseVisualStyleBackColor = true;
            // 
            // _lbActorDefinitions
            // 
            this._lbActorDefinitions.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lbActorDefinitions.FormattingEnabled = true;
            this._lbActorDefinitions.Location = new System.Drawing.Point(3, 3);
            this._lbActorDefinitions.Name = "_lbActorDefinitions";
            this._lbActorDefinitions.Size = new System.Drawing.Size(292, 173);
            this._lbActorDefinitions.TabIndex = 3;
            // 
            // _tpBaseActors
            // 
            this._tpBaseActors.Controls.Add(this._lbBaseActors);
            this._tpBaseActors.Location = new System.Drawing.Point(4, 22);
            this._tpBaseActors.Name = "_tpBaseActors";
            this._tpBaseActors.Padding = new System.Windows.Forms.Padding(3);
            this._tpBaseActors.Size = new System.Drawing.Size(298, 186);
            this._tpBaseActors.TabIndex = 1;
            this._tpBaseActors.Text = "Base Actors";
            this._tpBaseActors.UseVisualStyleBackColor = true;
            // 
            // _lbBaseActors
            // 
            this._lbBaseActors.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lbBaseActors.FormattingEnabled = true;
            this._lbBaseActors.Location = new System.Drawing.Point(3, 3);
            this._lbBaseActors.Name = "_lbBaseActors";
            this._lbBaseActors.Size = new System.Drawing.Size(292, 173);
            this._lbBaseActors.TabIndex = 0;
            // 
            // _ddlSimulate
            // 
            this._ddlSimulate.FormattingEnabled = true;
            this._ddlSimulate.Items.AddRange(new object[] {
            "On",
            "Off"});
            this._ddlSimulate.Location = new System.Drawing.Point(76, 12);
            this._ddlSimulate.Name = "_ddlSimulate";
            this._ddlSimulate.Size = new System.Drawing.Size(67, 21);
            this._ddlSimulate.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Simulation:";
            // 
            // EditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(333, 748);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._ddlSimulate);
            this.Controls.Add(this._tcActorDefinitions);
            this.Controls.Add(this._btnAddActor);
            this.Controls.Add(this._lblActorDefs);
            this.Controls.Add(this.btnSaveActorDefinition);
            this.Controls.Add(this._pgObjProperties);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "EditorForm";
            this.Text = "AngelXNA Edit Window";
            this._tcActorDefinitions.ResumeLayout(false);
            this._tpTemplates.ResumeLayout(false);
            this._tpBaseActors.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid _pgObjProperties;
        private System.Windows.Forms.Button btnSaveActorDefinition;
        private System.Windows.Forms.Label _lblActorDefs;
        private System.Windows.Forms.Button _btnAddActor;
        private System.Windows.Forms.TabControl _tcActorDefinitions;
        private System.Windows.Forms.TabPage _tpTemplates;
        private System.Windows.Forms.ListBox _lbActorDefinitions;
        private System.Windows.Forms.TabPage _tpBaseActors;
        private System.Windows.Forms.ListBox _lbBaseActors;
        private System.Windows.Forms.ComboBox _ddlSimulate;
        private System.Windows.Forms.Label label1;
    }
}

#endif