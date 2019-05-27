// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rock.Web.UI.Controls
{
    /// <summary>
    /// A <see cref="T:System.Web.UI.WebControls.Repeater"/> control with an optional EmptyDataTemplate.
    /// </summary>
    [ToolboxData( "<{0}:RockRepeater runat=server>" )]
    public class RockRepeater : Repeater, IRockControl, IHasValidationGroup
    {

        #region IRockControl implementation

        /// <summary>
        /// Gets or sets the label text.
        /// </summary>
        /// <value>
        /// The label text.
        /// </value>
        [
        Bindable( true ),
        Category( "Appearance" ),
        DefaultValue( "" ),
        Description( "The text for the label." )
        ]
        public string Label
        {
            get { return ViewState["Label"] as string ?? string.Empty; }
            set { ViewState["Label"] = value; }
        }

        /// <summary>
        /// Gets or sets the form group class.
        /// </summary>
        /// <value>
        /// The form group class.
        /// </value>
        [
        Bindable( true ),
        Category( "Appearance" ),
        Description( "The CSS class to add to the form-group div." )
        ]
        public string FormGroupCssClass
        {
            get { return ViewState["FormGroupCssClass"] as string ?? string.Empty; }
            set { ViewState["FormGroupCssClass"] = value; }
        }

        /// <summary>
        /// Gets or sets the help text.
        /// </summary>
        /// <value>
        /// The help text.
        /// </value>
        [
        Bindable( true ),
        Category( "Appearance" ),
        DefaultValue( "" ),
        Description( "The help block." )
        ]
        public string Help
        {
            get
            {
                return HelpBlock != null ? HelpBlock.Text : string.Empty;
            }
            set
            {
                if ( HelpBlock != null )
                {
                    HelpBlock.Text = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the warning text.
        /// </summary>
        /// <value>
        /// The warning text.
        /// </value>
        [
        Bindable( true ),
        Category( "Appearance" ),
        DefaultValue( "" ),
        Description( "The warning block." )
        ]
        public string Warning
        {
            get
            {
                return WarningBlock != null ? WarningBlock.Text : string.Empty;
            }
            set
            {
                if ( WarningBlock != null )
                {
                    WarningBlock.Text = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RockTextBox"/> is required.
        /// </summary>
        /// <value>
        ///   <c>true</c> if required; otherwise, <c>false</c>.
        /// </value>
        [
        Bindable( true ),
        Category( "Behavior" ),
        DefaultValue( "false" ),
        Description( "Is the value required?" )
        ]
        public virtual bool Required
        {
            get { return ViewState["Required"] as bool? ?? false; }
            set { ViewState["Required"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the Required indicator when Required=true
        /// </summary>
        /// <value>
        /// <c>true</c> if [display required indicator]; otherwise, <c>false</c>.
        /// </value>
        public bool DisplayRequiredIndicator
        {
            get { return ViewState["DisplayRequiredIndicator"] as bool? ?? true; }
            set { ViewState["DisplayRequiredIndicator"] = value; }
        }

        /// <summary>
        /// Gets or sets the required error message.  If blank, the LabelName name will be used
        /// </summary>
        /// <value>
        /// The required error message.
        /// </value>
        //public string RequiredErrorMessage
        //{
        //    get
        //    {
        //        return RequiredFieldValidator != null ? RequiredFieldValidator.ErrorMessage : string.Empty;
        //    }
        //    set
        //    {
        //        if ( RequiredFieldValidator != null )
        //        {
        //            RequiredFieldValidator.ErrorMessage = value;
        //        }
        //    }
        //}

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsValid
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the help block.
        /// </summary>
        /// <value>
        /// The help block.
        /// </value>
        public HelpBlock HelpBlock { get; set; }

        /// <summary>
        /// Gets or sets the warning block.
        /// </summary>
        /// <value>
        /// The warning block.
        /// </value>
        public WarningBlock WarningBlock { get; set; }

        /// <summary>
        /// Gets or sets the required field validator.
        /// </summary>
        /// <value>
        /// The required field validator.
        /// </value>
        //public RequiredFieldValidator RequiredFieldValidator { get; set; }

        /// <summary>
        /// Gets or sets the group of controls for which the <see cref="T:System.Web.UI.WebControls.TextBox" /> control causes validation when it posts back to the server.
        /// </summary>
        /// <returns>The group of controls for which the <see cref="T:System.Web.UI.WebControls.TextBox" /> control causes validation when it posts back to the server. The default value is an empty string ("").</returns>
        public string ValidationGroup
        {
            get { return ViewState["ValidationGroup"] as string; }
            set { ViewState["ValidationGroup"] = value; }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Is the Header Template visible if the repeater has no items?
        /// </summary>
        [PersistenceMode( PersistenceMode.Attribute )]
        public bool IsHeaderVisibleIfEmpty
        {
            get;
            set;
        }

        /// <summary>
        /// Is the Footer Template visible if the repeater has no items?
        /// </summary>
        [PersistenceMode( PersistenceMode.Attribute )]
        public bool IsFooterVisibleIfEmpty
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the data template to show when the repeater has no items.
        /// </summary>
        /// <value>The empty template.</value>
        [PersistenceMode( PersistenceMode.InnerProperty )]
        public ITemplate EmptyDataTemplate
        {
            get;
            set;
        }
        //string IRockControl.Label { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //string IRockControl.Help { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //string IRockControl.Warning { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //bool IRockControl.Required { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        string IRockControl.RequiredErrorMessage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        //bool IRockControl.IsValid => throw new NotImplementedException();

        //string IRockControl.ID => throw new NotImplementedException();

        //string IRockControl.ClientID => throw new NotImplementedException();

        //string IRockControl.FormGroupCssClass { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //HelpBlock IRockControl.HelpBlock { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //WarningBlock IRockControl.WarningBlock { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        RequiredFieldValidator IRockControl.RequiredFieldValidator { get { return null; }
            set
            {
                //
            } }
        //bool IHasRequired.Required { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        string IHasValidationGroup.ValidationGroup
        {
            get
            {
                return string.Empty;
            }
            set
            {
                //
            }
        }
    
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RockTextBox" /> class.
        /// </summary>
        public RockRepeater()
            : base()
        {
            RockControlHelper.Init( this );
        }

        #region Control Methods

            /// <summary>
            /// Creates the child controls.
            /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            Controls.Clear();

            //return;

            //RockControlHelper.CreateChildControls( this, Controls );

            this.SetDataTemplateStates();
        }

        /// <summary>
        /// Outputs server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter" /> object and stores tracing information about the control if tracing is enabled.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> object that receives the control content.</param>
        public override void RenderControl( HtmlTextWriter writer )
        {
            if ( this.Visible )
            {
                // Use the helper to render the standard Rock control elements.
                // The non-standard portion of the control is rendered in the RenderBaseControl() callback method.
                RockControlHelper.RenderControl( this, writer );
            }
        }

        public void RenderBaseControl( HtmlTextWriter writer )
        {
            // Render the repeater controls.
            base.RenderControl( writer );
        }

        /// <summary>
        /// Raises the DataBinding event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> 
        /// object that contains the event data.
        /// </param>
        protected override void OnDataBinding( EventArgs e )
        {
            base.OnDataBinding( e );

            SetDataTemplateStates();
        }

        /// <summary>
        /// Sets the visibility of the data templates based on the content of the repeater.
        /// </summary>
        private void SetDataTemplateStates()
        {
            if ( this.Items == null
                 || this.Items.Count > 0
                 || EmptyDataTemplate == null )
            {
                return;
            }

            //this.Controls.Clear();

            if ( IsHeaderVisibleIfEmpty
                 && HeaderTemplate != null )
            {
                HeaderTemplate.InstantiateIn( this );
            }

            EmptyDataTemplate.InstantiateIn( this );

            if ( IsFooterVisibleIfEmpty
                 && FooterTemplate != null )
            {
                FooterTemplate.InstantiateIn( this );
            }

        }

        #endregion
    }
}