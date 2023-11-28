using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinServLite2.Graph
{
    public class PlanTask
    {
        public string odatacontext { get; set; }
        public string odataetag { get; set; }
        public string planId { get; set; }
        public string bucketId { get; set; }
        public string title { get; set; }
        public string orderHint { get; set; }
        public string assigneePriority { get; set; }
        public int percentComplete { get; set; }
        public object startDateTime { get; set; }
        public DateTime createdDateTime { get; set; }
        public object dueDateTime { get; set; }
        public bool hasDescription { get; set; }
        public string previewType { get; set; }
        public object completedDateTime { get; set; }
        public object completedBy { get; set; }
        public int referenceCount { get; set; }
        public int checklistItemCount { get; set; }
        public int activeChecklistItemCount { get; set; }
        public object conversationThreadId { get; set; }
        public string id { get; set; }
        public Createdby createdBy { get; set; }
        public Appliedcategories appliedCategories { get; set; }
        public Assignments assignments { get; set; }

        public string GetUrl() => $"https://tasks.office.com/insupport.se/Home/Task/{id}";
    }

    public class Createdby
    {
        public User user { get; set; }
    }

    public class User
    {
        public object displayName { get; set; }
        public string id { get; set; }
    }

    public class Appliedcategories
    {
    }

    public class Assignments
    {
    }

    public class PlanTaskDetails
    {
        public Dictionary<string, PlannerChecklistItem> checklist { get; set; }

        [JsonProperty("@odata.etag")]
        public string etag { get; set; }

        public string description { get; set; }
        public string id { get; set; }
        public string previewType { get; set; }
    }

    public class PlannerChecklistItem
    {
        [JsonProperty("@odata.type")]
        public string odatatype { get; set; } = "microsoft.graph.plannerChecklistItem";
        public bool isChecked { get; set; }
        //public LastModifiedBy lastModifiedBy { get; set; }
        //public DateTime lastModifiedByDateTime { get; set; }
        public string orderHint { get; set; }
        public string title { get; set; }
    }

    public class PlannerChecklistItemForUpload
    {
        [JsonProperty("@odata.type")]
        public string odatatype { get; set; } = "microsoft.graph.plannerChecklistItem";
        public bool isChecked { get; set; }
        public string title { get; set; }
    }

    public class LastModifiedBy
    {
        public User user { get; set; }
    }
}
