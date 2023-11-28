<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Jobs.aspx.cs" Inherits="WinServLite2.Web.Jobs" Title="Jobs" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="table-responsive">
        <div class="form-group">
            <div class="input-group mb-3">
              <input type="text" class="form-control" id="search-query-input" placeholder="Search query" aria-label="Search query" aria-describedby="search-query-button">
              <div class="input-group-append">
                <button class="btn btn-outline-secondary" type="button" id="search-query-button">Search</button>
              </div>
              <div class="input-group-append">
                <button class="btn btn-outline-secondary" type="button" id="search-query-button-clear">Clear</button>
              </div>
            </div>
        </div>
        <asp:ListView ID="jobsTable" runat="server">
            <LayoutTemplate>
                <div class="table-responsive">
                    <table runat="server" id="jobsTable" class="table table-striped">
                        <tr runat="server">
                            <th runat="server">JobID</th>
                            <th runat="server">Description</th>
                            <th runat="server">Site</th>
                        </tr>
                        <tr runat="server" id="itemPlaceholder" />
                    </table>
                </div>
            </LayoutTemplate>
            <ItemTemplate>
                <tr runat="server">
                    <td><a href="Job.aspx?id=<%# (Container.DataItem as WinServLib.Objects.Job).JobID %>"><asp:Label ID="JobIDLabel" runat="server" Text='<%# (Container.DataItem as WinServLib.Objects.Job).JobID %>'/></a></td>
                    <td><asp:Label ID="JobDescriptionLabel" runat="server" Text='<%# (Container.DataItem as WinServLib.Objects.Job).CompleteJobDescription %>'/></td>
                    <td><asp:Label ID="JobSiteLabel" runat="server" Text='<%# (Container.DataItem as WinServLib.Objects.Job).SiteName %>'/></td>
                </tr>
            </ItemTemplate>
        </asp:ListView>
    </div>

    <script>

        $(document).ready(function () {
            $("#search-query-input").on('keydown', searchOnEnter);
            $("#search-query-button").click(searchJobs);
            $("#search-query-button-clear").click(function() {
                $("#search-query-input").val("");
                searchJobs();
            });
            //$("form").on("submit", function (e) {
            //    e.preventDefault();
            //    return false;
            //});

            //$(document).on("keypress", 'form', function (e) {
            //    var code = e.keyCode || e.which;
            //    if (code == 13) {
            //        e.preventDefault();
            //        return false;
            //    }
            //});
        });

        function searchOnEnter(e) {
            console.log(e);
            if (e.keyCode == 13) {
                e.preventDefault();
                searchJobs();
                return false;
            }
        }

        function searchJobs() {
            // Declare variables
            var input, filter, table, tr, td, tds, i, txtValue, showTr;
            //input = document.getElementById("myInput");
            filter = $("#search-query-input").val().toUpperCase();
            table = document.getElementById("MainContent_jobsTable_jobsTable");
            tr = table.getElementsByTagName("tr");

            // Loop through all table rows, and hide those who don't match the search query
            for (i = 1; i < tr.length; i++) {
                tds = tr[i].getElementsByTagName("td");
                showTr = false;

                if (tds.length > 0)
                    for (var tdIndex in tds) {
                        txtValue = tds[tdIndex].textContent || tds[tdIndex].innerText;
                        if (txtValue !== undefined && txtValue.toUpperCase().indexOf(filter) > -1) {
                            //tr[i].style.display = "";
                            showTr = true;
                        } else {
                            //tr[i].style.display = "none";
                        }
                    }

                if (showTr)
                    tr[i].style.display = "";
                else
                    tr[i].style.display = "none";
                //if (td) {
                //    txtValue = td.textContent || td.innerText;
                //    if (txtValue.toUpperCase().indexOf(filter) > -1) {
                //        tr[i].style.display = "";
                //    } else {
                //        tr[i].style.display = "none";
                //    }
                //}
            }

            $("tr:visible").each(function (index) {
                $(this).css("background-color", !!(index & 1) ? "rgba(0,0,0,0)" : "rgba(0,0,0,.05)");
            });
        }
    </script>
</asp:Content>