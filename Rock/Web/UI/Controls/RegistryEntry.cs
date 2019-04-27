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

using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock.Security;

namespace Rock.Web.UI.Controls
{
    public class RegistryEntry : CompositeControl, IRockControl
    {
        private TextBox tbVolume;
        private TextBox tbPage;
        private TextBox tbLine;

        public string Text
        {
            get
            {
                EnsureChildControls();
                return $"{tbVolume.Text},{tbPage.Text},{tbLine.Text}";
            }
            set
            {
                EnsureChildControls();
                string[] values = value.Split( ',' );
                if ( values.Length != 3 || values[0].AsIntegerOrNull() == null || values[1].AsIntegerOrNull() == null || values[2].AsIntegerOrNull() == null)
                {
                    // Need three numbers and only three numbers.
                    return;
                }

                tbVolume.Text = values[0];
                tbPage.Text = values[1];
                tbLine.Text = values[2];
            }
        }

        public RegistryEntry() : base()
        {
            RequiredFieldValidator = new HiddenFieldValidator();
            HelpBlock = new HelpBlock();
            WarningBlock = new WarningBlock();
        }

        #region IRockControl Implementation

        [Bindable( true )]
        [Category( "Appearance" )]
        [DefaultValue( "" )]
        [Description( "The text for the label.")]
        public string Label
        {
            get
            {
                return ViewState["Label"] as string ?? string.Empty;
            }

            set
            {
                ViewState["Label"] = value;
            }
        }

        [Bindable( true )]
        [Category( "Appearance" )]
        [DefaultValue( "" )]
        [Description( "The help block." )]
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
        [Bindable( true )]
        [Category( "Appearance" )]
        [DefaultValue( "" )]
        [Description( "The warning block." )]
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
        [Bindable( true )]
        [Category( "Behavior" )]
        [DefaultValue( "false" )]
        [Description( "Is the value required?" )]
        public bool Required
        {
            get
            {
                return ViewState["Required"] as bool? ?? false;
            }

            set
            {
                ViewState["Required"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the required error message.  If blank, the LabelName name will be used
        /// </summary>
        /// <value>
        /// The required error message.
        /// </value>
        public string RequiredErrorMessage
        {
            get
            {
                return RequiredFieldValidator != null ? RequiredFieldValidator.ErrorMessage : string.Empty;
            }

            set
            {
                if ( RequiredFieldValidator != null )
                {
                    RequiredFieldValidator.ErrorMessage = value;
                }
            }
        }

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
                return !Required || RequiredFieldValidator == null || RequiredFieldValidator.IsValid;
            }
        }

        /// <summary>
        /// Gets or sets the form group class.
        /// </summary>
        /// <value>
        /// The form group class.
        /// </value>
        [Bindable( true )]
        [Category( "Appearance" )]
        [Description( "The CSS class to add to the form-group div." )]
        public string FormGroupCssClass
        {
            get
            {
                return ViewState["FormGroupCssClass"] as string ?? string.Empty;
            }

            set
            {
                ViewState["FormGroupCssClass"] = value;
            }
        }

        /// <summary>
        /// Gets the help block.
        /// </summary>
        /// <value>
        /// The help block.
        /// </value>
        public HelpBlock HelpBlock { get; set; }

        /// <summary>
        /// Gets the warning block.
        /// </summary>
        /// <value>
        /// The warning block.
        /// </value>
        public WarningBlock WarningBlock { get; set; }

        /// <summary>
        /// Gets the required field validator.
        /// </summary>
        /// <value>
        /// The required field validator.
        /// </value>
        public RequiredFieldValidator RequiredFieldValidator { get; set; }

        /// <summary>
        /// Gets or sets an optional validation group to use.
        /// </summary>
        /// <value>
        /// The validation group.
        /// </value>
        public string ValidationGroup
        {
            get
            {
                return RequiredFieldValidator.ValidationGroup;
            }
            set
            {
                RequiredFieldValidator.ValidationGroup = value;
            }
        }

        /// <summary>
        /// Renders the base control.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void RenderBaseControl( HtmlTextWriter writer )
        {
            writer.AddAttribute( "class", "form-control-group" );
            writer.RenderBeginTag( HtmlTextWriterTag.Div );

            tbVolume.RenderControl( writer );
            tbPage.RenderControl( writer );
            tbLine.RenderControl( writer );

            writer.RenderEndTag();
        }

        #endregion IRockControl Implementation

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            tbVolume = new TextBox
            {
                ID = $"tbVolume_{this.ID}",
                TextMode = TextBoxMode.Number,
                CssClass = "form-control"
            };

            tbVolume.Attributes.Add( "placeholder", "Volume" );

            tbPage = new TextBox
            {
                ID = $"",
                TextMode = TextBoxMode.Number,
                CssClass = "form-control"
            };

            tbPage.Attributes.Add( "placeholder", "Page" );

            tbLine = new TextBox
            {
                ID = $"",
                TextMode = TextBoxMode.Number,
                CssClass = "form-control"
            };

            tbLine.Attributes.Add( "placeholder", "Line" );

            Controls.Add( tbVolume );
            Controls.Add( tbPage );
            Controls.Add( tbLine );

        }
    }
}
