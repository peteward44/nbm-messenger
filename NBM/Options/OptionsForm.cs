using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;

using NBM.OptionNodes;
using NBM.Plugin;

using WinForms = System.Windows.Forms;

// 12/5/03

namespace NBM
{
	/// <summary>
	/// The main options form.
	/// </summary>
	public class OptionsForm : WinForms.Form
	{
		private MainForm mainForm;
		private ArrayList nodesList = new ArrayList();

		private System.Windows.Forms.TreeView optionsTree;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button applyButton;

		private WinForms.Control currentControl = null;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		/// <summary>
		/// Constructs an OptionsForm.
		/// </summary>
		/// <param name="mainForm"></param>
		public OptionsForm(MainForm mainForm)
			: this(mainForm, null)
		{
		}


		/// <summary>
		/// Constructs an OptionsForm, but starts with the selectedProtocol's options pane.
		/// </summary>
		/// <param name="mainForm"></param>
		/// <param name="selectedProtocol"></param>
		public OptionsForm(MainForm mainForm, Protocol selectedProtocol)
		{
			this.mainForm = mainForm;

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.optionsTree.AfterSelect += new WinForms.TreeViewEventHandler(OnNodeSelect);

			WinForms.TreeNode generalNode = new WinForms.TreeNode("General");

			AddNode(generalNode.Nodes, new BasicOptions());
			AddNode(generalNode.Nodes, new ConnectionOptions());
			AddNode(generalNode.Nodes, new EventOptions());
			AddNode(generalNode.Nodes, new ConversationOptions());

			this.optionsTree.Nodes.Add(generalNode);

			// then put in the protocol nodes
			foreach (Protocol protocol in this.mainForm.Protocols)
			{
				WinForms.TreeNode pnode = new WinForms.TreeNode(protocol.Name);
				pnode.Tag = protocol;
				this.optionsTree.Nodes.Add(pnode);

				AddNode(pnode.Nodes, new BasicProtocolOptions(protocol.Settings));

				if (selectedProtocol == protocol)
				{
					this.optionsTree.SelectedNode = pnode.Nodes[0];
				}

				foreach (IOptions options in protocol.OptionsNodes)
				{
					AddNode(pnode.Nodes, options);
				}
			}

			this.optionsTree.ExpandAll();
		}


		/// <summary>
		/// Adds an options node to the options tree.
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		private int AddNode(WinForms.TreeNodeCollection collection, IOptions options)
		{
			WinForms.TreeNode node = new WinForms.TreeNode(options.NodeName);
			node.Tag = options;
			collection.Add(node);
			return this.nodesList.Add(options);
		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}

				foreach (IOptions options in this.nodesList)
				{
					options.Dispose();
				}
			}
			base.Dispose( disposing );
		}


		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.optionsTree = new System.Windows.Forms.TreeView();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.applyButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// optionsTree
			// 
			this.optionsTree.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left);
			this.optionsTree.FullRowSelect = true;
			this.optionsTree.HideSelection = false;
			this.optionsTree.ImageIndex = -1;
			this.optionsTree.Location = new System.Drawing.Point(8, 8);
			this.optionsTree.Name = "optionsTree";
			this.optionsTree.SelectedImageIndex = -1;
			this.optionsTree.ShowPlusMinus = false;
			this.optionsTree.Size = new System.Drawing.Size(120, 360);
			this.optionsTree.TabIndex = 0;
			// 
			// okButton
			// 
			this.okButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(328, 344);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(64, 24);
			this.okButton.TabIndex = 2;
			this.okButton.Text = "OK";
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(400, 344);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(64, 24);
			this.cancelButton.TabIndex = 3;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// applyButton
			// 
			this.applyButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.applyButton.Location = new System.Drawing.Point(472, 344);
			this.applyButton.Name = "applyButton";
			this.applyButton.Size = new System.Drawing.Size(64, 24);
			this.applyButton.TabIndex = 4;
			this.applyButton.Text = "Apply";
			this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
			// 
			// OptionsForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(546, 376);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.applyButton,
																																	this.cancelButton,
																																	this.okButton,
																																	this.optionsTree});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "OptionsForm";
			this.Text = "Options";
			this.ResumeLayout(false);

		}
		#endregion


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void okButton_Click(object sender, System.EventArgs e)
		{
			SaveAllSettings();
			this.Close();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cancelButton_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void applyButton_Click(object sender, System.EventArgs e)
		{
			SaveAllSettings();
			this.Activate();
		}


		/// <summary>
		/// Saves all the settings in each of the option's nodes.
		/// </summary>
		private void SaveAllSettings()
		{
			foreach (IOptions options in this.nodesList)
			{
				options.Save();
			}

			this.mainForm.UpdateOptions();
		}


		/// <summary>
		/// When a treenode is selected, display the options node which is assocaited with it.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnNodeSelect(object sender, WinForms.TreeViewEventArgs e)
		{
			// clear the old controls out
			if (currentControl != null)
			{
				this.Controls.Remove(currentControl);
				currentControl = null;
			}

			if (e.Node.Tag is IOptions)
			{
				IOptions options = (IOptions)e.Node.Tag;

				if (options != null)
				{
					WinForms.Control control = (WinForms.Control)options;
					control.Location = new System.Drawing.Point(136, 8);
					this.Controls.Add(control);
					currentControl = control;
				}
			}
		}
	}
}
