using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.Admin
{
    public partial class PublishDoc : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GVPublishTopicFile.HeaderRow.TableSection = TableRowSection.TableHeader;
            GVPublishDocFile.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
}