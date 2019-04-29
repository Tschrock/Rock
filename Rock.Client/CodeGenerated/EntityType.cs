//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Rock.CodeGeneration project
//     Changes to this file will be lost when the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
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
using System.Collections.Generic;


namespace Rock.Client
{
    /// <summary>
    /// Base client model for EntityType that only includes the non-virtual fields. Use this for PUT/POSTs
    /// </summary>
    public partial class EntityTypeEntity
    {
        /// <summary />
        public int Id { get; set; }

        /// <summary />
        public string AssemblyName { get; set; }

        /// <summary />
        public bool AttributesSupportPrePostHtml { get; set; }

        /// <summary />
        public bool AttributesSupportShowOnBulk { get; set; }

        /// <summary />
        public Guid? ForeignGuid { get; set; }

        /// <summary />
        public string ForeignKey { get; set; }

        /// <summary />
        public string FriendlyName { get; set; }

        /// <summary />
        public string IndexDocumentUrl { get; set; }

        /// <summary />
        public string IndexResultTemplate { get; set; }

        /// <summary />
        public bool IsCommon { get; set; }

        /// <summary />
        public bool IsEntity { get; set; }

        /// <summary />
        public bool IsIndexingEnabled { get; set; }

        /// <summary />
        public bool IsSecured { get; set; }

        /// <summary />
        public string LinkUrlLavaTemplate { get; set; }

        /// <summary />
        public int? MultiValueFieldTypeId { get; set; }

        /// <summary />
        public string Name { get; set; }

        /// <summary />
        public int? SingleValueFieldTypeId { get; set; }

        /// <summary />
        public Guid Guid { get; set; }

        /// <summary />
        public int? ForeignId { get; set; }

        /// <summary>
        /// Copies the base properties from a source EntityType object
        /// </summary>
        /// <param name="source">The source.</param>
        public void CopyPropertiesFrom( EntityType source )
        {
            this.Id = source.Id;
            this.AssemblyName = source.AssemblyName;
            this.AttributesSupportPrePostHtml = source.AttributesSupportPrePostHtml;
            this.AttributesSupportShowOnBulk = source.AttributesSupportShowOnBulk;
            this.ForeignGuid = source.ForeignGuid;
            this.ForeignKey = source.ForeignKey;
            this.FriendlyName = source.FriendlyName;
            this.IndexDocumentUrl = source.IndexDocumentUrl;
            this.IndexResultTemplate = source.IndexResultTemplate;
            this.IsCommon = source.IsCommon;
            this.IsEntity = source.IsEntity;
            this.IsIndexingEnabled = source.IsIndexingEnabled;
            this.IsSecured = source.IsSecured;
            this.LinkUrlLavaTemplate = source.LinkUrlLavaTemplate;
            this.MultiValueFieldTypeId = source.MultiValueFieldTypeId;
            this.Name = source.Name;
            this.SingleValueFieldTypeId = source.SingleValueFieldTypeId;
            this.Guid = source.Guid;
            this.ForeignId = source.ForeignId;

        }
    }

    /// <summary>
    /// Client model for EntityType that includes all the fields that are available for GETs. Use this for GETs (use EntityTypeEntity for POST/PUTs)
    /// </summary>
    public partial class EntityType : EntityTypeEntity
    {
    }
}
