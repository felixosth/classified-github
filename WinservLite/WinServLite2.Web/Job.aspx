<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Job.aspx.cs" Inherits="WinServLite2.Web.Job" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
        <h2><asp:Label runat="server" ID="JobHeader"/></h2>
        <div class="row">
            <div class="col-sm">
                <p><asp:Label runat="server" ID="JobDescriptionLabel"  /></p>
            </div>
            <div class="col-sm">
                <p><asp:Label runat="server" ID="JobContactNameLabel" /></p>
                <p><asp:HyperLink runat="server" ID="JobContactTelLink" Target="_blank"/></p>
                <p><asp:HyperLink runat="server" ID="JobContactEmailLink" Target="_blank"/></p>
                <p><asp:HyperLink runat="server" ID="JobLocationMapsLink" Target="_blank"/></p>
            </div>
        </div>

        <div class="table-responsive">
            <button type="button" class="btn btn-primary" id="new-report-button" style="margin-bottom:1rem;">Add report</button>

            <asp:ListView ID="JobReportsTable" runat="server">
                <LayoutTemplate>
                    <div class="table-responsive">
                        <table runat="server" class="table table-striped" id="table">
                            <tr runat="server">
                                <th runat="server">Technician</th>
                                <th runat="server">Comment</th>
                                <th runat="server">Date</th>
                                <th runat="server">Worktime</th>
                                <th runat="server">Traveltime</th>
                                <th runat="server">JobTimeType</th>
                                <th></th>
                            </tr>
                            <tr runat="server" id="itemPlaceholder" />
                        </table>
                    </div>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr runat="server" reportId="<%# (Container.DataItem as WinServLib.Objects.TimeReport).UniqueID %>">
                        <td><asp:Label runat="server" Text='<%# (Container.DataItem as WinServLib.Objects.TimeReport).Technician %>'/></td>
                        <td><asp:Label runat="server" Text='<%# (Container.DataItem as WinServLib.Objects.TimeReport).Comment %>'/></td>
                        <td><asp:Label runat="server" Text='<%# (Container.DataItem as WinServLib.Objects.TimeReport).Date.ToShortDateString() %>'/></td>
                        <td><asp:Label runat="server" Text='<%# (Container.DataItem as WinServLib.Objects.TimeReport).WorkTime %>'/>h</td>
                        <td><asp:Label runat="server" Text='<%# (Container.DataItem as WinServLib.Objects.TimeReport).TravelTime %>'/>h</td>
                        <td delcode="<%# (Container.DataItem as WinServLib.Objects.TimeReport).DelayCode %>"><asp:Label runat="server" Text='<%# (Container.DataItem as WinServLib.Objects.TimeReport).JobTimeTypeName %>'/></td>
                        <td>
                            <button type="button" class="btn btn-outline-secondary edit-report-button">Edit</button>
                            <asp:Button runat="server" Text="Delete" CommandArgument='<%# (Container.DataItem as WinServLib.Objects.TimeReport).JobID + ";" + (Container.DataItem as WinServLib.Objects.TimeReport).UniqueID %>' OnClientClick="return confirm('Are you sure?')" OnClick="DeleteReport_Click" CssClass="btn btn-outline-danger custom-delete-button"/>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:ListView>
        </div>

    <div class="modal fade" id="addReportModal" tabindex="-1" aria-labelledby="addReportModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="addReportModalLabel">Add time report</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <asp:HiddenField runat="server" ID="ReportTypeMode" />
                    <asp:HiddenField runat="server" ID="NewReportJobID" />
                    <asp:HiddenField runat="server" ID="EditReportID" />
                    <div class="form-group">
                        <label>Technician</label>
                        <asp:DropDownList runat="server" ID="NewReportTech" CssClass="custom-select" />
                    </div>
                    <div class="form-group">
                        <label>Comment</label>
                        <asp:TextBox runat="server" ID="NewReportComment" CssClass="form-control" TextMode="MultiLine" Rows="2" />
                    </div>
                    <div class="form-group">
                        <label>JobTimeType</label>
                        <asp:DropDownList runat="server" ID="NewReportJobTimeType" CssClass="custom-select" />
                    </div>
                    <div class="form-group">
                        <label>Date</label>
                        <asp:TextBox runat="server" ID="NewReportDate" TextMode="Date" CssClass="form-control" />
                    </div>
                    <div class="form-group">
                        <div class="row">
                            <div class="col">
                                <label>Worktime (hours)</label>
                                <asp:TextBox Text="0" runat="server" ID="NewReportWorktime" CssClass="form-control" TextMode="Number" step="any" />
                            </div>
                            <div class="col">
                                <label>Traveltime (hours)</label>
                                <asp:TextBox Text="0" runat="server" ID="NewReportTraveltime" CssClass="form-control" TextMode="Number" step="any" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                    <asp:Button runat="server" Text="Submit" CssClass="btn btn-primary" OnClientClick="return validateReportSubmit();" OnClick="SubmitReport_Click" />
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">


        var defaultTech, defaultDate, defaultJobTimeType;
        $(document).ready(function () {
            defaultTech = $("#MainContent_NewReportTech").val();
            defaultDate = $("#MainContent_NewReportDate").val();
            defaultJobTimeType = $("#MainContent_NewReportJobTimeType").val();


            $("#new-report-button").click(function () {

                $("#MainContent_ReportTypeMode").val("new");
                $("#addReportModalLabel").text("Add time report");
                $("#MainContent_EditReportID").val("");

                $("#MainContent_NewReportTech").val(defaultTech);


                $("#MainContent_NewReportComment").val("");
                $("#MainContent_NewReportDate").val(defaultDate);
                $("#MainContent_NewReportWorktime").val("0");
                $("#MainContent_NewReportTraveltime").val("0");
                $("#MainContent_NewReportJobTimeType").val(defaultJobTimeType);

                $("#addReportModal").modal();
                $("#MainContent_NewReportComment").focus();

            });

            $(".edit-report-button").click(function () {

                $("#MainContent_ReportTypeMode").val("edit");
                $("#addReportModalLabel").text("Edit time report");

                var $tr = $(this).parents().eq(1);
                $("#MainContent_EditReportID").val($tr.attr("reportId"));

                var columns = $tr.find("td");

                console.log(columns);

                $("#MainContent_NewReportTech").val(columns[0].innerText);
                $("#MainContent_NewReportComment").val(columns[1].innerText);
                $("#MainContent_NewReportDate").val(columns[2].innerText);
                console.log(columns[3].innerText.slice(0, -1));
                $("#MainContent_NewReportWorktime").val(parseFloat(columns[3].innerText.slice(0, -1).replace(",", ".")));
                $("#MainContent_NewReportTraveltime").val(parseFloat(columns[4].innerText.slice(0, -1).replace(",", ".")));
                $("#MainContent_NewReportJobTimeType").val(columns[5].getAttribute("delcode"));
                //$("#MainContent_JobReportsTable_table tbody tr")


                $("#addReportModal").modal();
            });
        });

        function validateReportSubmit() {


            if (document.getElementById("MainContent_NewReportJobTimeType").value === "0") {
                alert("Specify job time type!");
                return false;
            }
            else if (document.getElementById("MainContent_NewReportComment").value === "") {
                alert("Enter a comment.");
                return false;
            }

            var travelTime = parseFloat(document.getElementById("MainContent_NewReportTraveltime").value);
            var workTime = parseFloat(document.getElementById("MainContent_NewReportWorktime").value);
            if (workTime <= 0) {
                alert("Enter worktime!");
                return false;
            }
            else if (workTime > 24 || (workTime + travelTime) > 24) {
                alert("You can impossibly work 100% of the day. Please adjust your reported time.");
                return false;
            }

            return true;
        }
    </script>
</asp:Content>