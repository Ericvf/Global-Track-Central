namespace Trials.GTC.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Linq;
    using System.Xml.Serialization;
    using System.Data.Linq.Mapping;

    partial class GTCDataContext
    {
    }

    partial class Track
    {
        public Guid[] Tags
        {
            get
            {
                return (from trackTag in this.TrackTags
                        select trackTag.TagId).ToArray();
            }
        }


        private string submittedBy;
        public string SubmittedBy
        {
            get
            {
                if (this.submittedBy == null && this.aspnet_User != null)
                    this.submittedBy = this.aspnet_User.UserName;

                return submittedBy;
            }
            set
            {
                this.submittedBy = value;
            }
        }
    }
}
