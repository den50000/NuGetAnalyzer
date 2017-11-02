namespace NuGetAnalyzer.View
{
    partial class MainForm
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.rtbOutput = new System.Windows.Forms.RichTextBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnDuplicates = new System.Windows.Forms.Button();
            this.btnFindIfLatest = new System.Windows.Forms.Button();
            this.tfsHierarchyTree = new System.Windows.Forms.TreeView();
            this.rtbResult = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // rtbOutput
            // 
            this.rtbOutput.BackColor = System.Drawing.Color.Black;
            this.rtbOutput.ForeColor = System.Drawing.Color.Lime;
            this.rtbOutput.Location = new System.Drawing.Point(10, 561);
            this.rtbOutput.Name = "rtbOutput";
            this.rtbOutput.Size = new System.Drawing.Size(1344, 259);
            this.rtbOutput.TabIndex = 2;
            this.rtbOutput.Text = "";
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(598, 12);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(165, 23);
            this.btnGenerate.TabIndex = 5;
            this.btnGenerate.Text = "Build DGML File";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // btnDuplicates
            // 
            this.btnDuplicates.Location = new System.Drawing.Point(598, 98);
            this.btnDuplicates.Name = "btnDuplicates";
            this.btnDuplicates.Size = new System.Drawing.Size(165, 23);
            this.btnDuplicates.TabIndex = 8;
            this.btnDuplicates.Text = "Reveal Version Duplicates";
            this.btnDuplicates.UseVisualStyleBackColor = true;
            this.btnDuplicates.Click += new System.EventHandler(this.btnDuplicates_Click);
            // 
            // btnFindIfLatest
            // 
            this.btnFindIfLatest.Location = new System.Drawing.Point(598, 53);
            this.btnFindIfLatest.Name = "btnFindIfLatest";
            this.btnFindIfLatest.Size = new System.Drawing.Size(165, 27);
            this.btnFindIfLatest.TabIndex = 9;
            this.btnFindIfLatest.Text = "Analyze";
            this.btnFindIfLatest.UseVisualStyleBackColor = true;
            this.btnFindIfLatest.Click += new System.EventHandler(this.btnFindIfLatest_Click);
            // 
            // tfsHierarchyTree
            // 
            this.tfsHierarchyTree.CheckBoxes = true;
            this.tfsHierarchyTree.Location = new System.Drawing.Point(12, 12);
            this.tfsHierarchyTree.Name = "tfsHierarchyTree";
            this.tfsHierarchyTree.Size = new System.Drawing.Size(528, 536);
            this.tfsHierarchyTree.TabIndex = 11;
            this.tfsHierarchyTree.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tfsHierarchyTree_AfterCheck);
            this.tfsHierarchyTree.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.tfsHierarchyTree_AfterExpand);
            // 
            // rtbResult
            // 
            this.rtbResult.BackColor = System.Drawing.Color.Black;
            this.rtbResult.ForeColor = System.Drawing.Color.Lime;
            this.rtbResult.Location = new System.Drawing.Point(827, 12);
            this.rtbResult.Name = "rtbResult";
            this.rtbResult.Size = new System.Drawing.Size(527, 536);
            this.rtbResult.TabIndex = 12;
            this.rtbResult.Text = "";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1366, 832);
            this.Controls.Add(this.rtbResult);
            this.Controls.Add(this.tfsHierarchyTree);
            this.Controls.Add(this.btnFindIfLatest);
            this.Controls.Add(this.btnDuplicates);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.rtbOutput);
            this.Name = "MainForm";
            this.Text = "Nuget Analyzer";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.RichTextBox rtbOutput;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnDuplicates;
        private System.Windows.Forms.Button btnFindIfLatest;
        private System.Windows.Forms.TreeView tfsHierarchyTree;
        private System.Windows.Forms.RichTextBox rtbResult;
    }
}

