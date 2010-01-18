<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<MineSweeperFlags.Models.UserRegistration>" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<% String url_base = System.Web.VirtualPathUtility.ToAbsolute("~"); %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title>MineSweeper Flags/LI51N.PI.G1/Parte III - Register</title>
	<link rel="stylesheet" type="text/css" href="<%= url_base %>Content/home.css" />
</head>

<body>
	<div id="header">
		<img alt="SiteLogo" src="<%=Url.Content("~/Images/Minesweeper.png")%>" width="56" height="56" />
		<span class="title">MineSweeper Flags/LI51N.PI.G1/Parte III</span>
	</div>
	<div class="contentbar">
		User Registration
		<% if(! Model.IsUpdate.HasValue ||Model.IsUpdate.Value == false) { %>
		(<a class="link" href="<%= url_base %>">Home</a>)
		<% } %>
	</div>
	<div class="registration">
	<%if(!Model.HasError) {%>
		<p><b>REGISTRATION COMPLETE</b></p>
		<p>Nick: <%=Model.Id %></p>
		<p>Name: <%=Model.Name %></p>
		<p>Email: <%=Model.Email %></p>
		<a class="link" href="/">You Can Login To server</a>
	<% } else {%>
		<%= Html.ValidationSummary("Please review:", new Dictionary<string, object>{{ "class", "error" }})%>
		<% using (Html.BeginForm("Registration", "User")) { %>
			<%= Html.Hidden("IsUpdate", (Model.IsUpdate.HasValue ? Model.IsUpdate.Value : false)) %>
			<table><tbody>
				<tr><td class="t">Nick:</td><td class="v"><%= Html.TextBox("Id", Model.Id) %></td></tr>
				<tr><td class="t">Name:</td><td class="v"><%= Html.TextBox("Name", Model.Name) %></td></tr>
				<tr><td class="t">Email:</td><td class="v"><%= Html.TextBox("Email", Model.Email) %></td></tr>
				<tr><td class="t">Email(confirm):</td><td class="v"><%= Html.TextBox("EmailConf", Model.EmailConf) %></td></tr>
				<!-- <tr><td>Avatar:</td><td><input type="file" /></td></tr> -->
				<tr><td class="t">&nbsp;</td><td class="v"><input type="submit" value="Registar" /></td></tr>
			</tbody></table>
			
		<% } %>
	<% } %>
	</div>
</body>
</html>
