<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<MineSweeperFlags.Models.UserRef>" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<% String url_base = System.Web.VirtualPathUtility.ToAbsolute("~"); %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title>MineSweeper Flags/LI51N.PI.G1/Parte III</title>
	<link rel="stylesheet" type="text/css" href="<%= url_base %>Content/home.css" />
	<script type="text/javascript" src="<%= url_base %>Scripts/jquery-1.3.2.js"></script>
	<script type="text/javascript" src="<%= url_base %>Scripts/jquery.form.js"></script>
	<script type="text/javascript" src="<%= url_base %>Scripts/ajax.js"></script>
	<script type="text/javascript" src="<%= url_base %>Scripts/HomeView.js"></script>
	<script type="text/javascript" src="<%= url_base %>Scripts/HomeController.js"></script>
</head>
<body>
	<div id="header">
	<img alt="Site Logo" src="<%= url_base %>Images/Minesweeper.png" width="56" height="56" />
	<span class="title">MineSweeper Flags/LI51N.PI.G1/Parte III</span>
	</div>
	<div id="body">
	<div id="status">Status</div>
	
		<!-- LEFT -->
		<div id="left">
			<!-- conta de utilizador -->
			<div class="content">
				<div class="contentbar">Account</div>
				<div id="account" class="contentdata">
					<%if (Model.UserId == null || Model.HasError) {%>
					<span class="error">
					<%= Html.ValidationMessage("UserId") %>
					</span>
					<% using (Html.BeginForm("Index", "Home")) { %>
						<%=Html.TextBox("UserId", Model.UserId, new Dictionary<string, object> { { "size", "15" }, { "class", "text" } })%>
						<input type="submit" value="Login" />
					<% } %>
					<p>
					or
					<a id="u_newreg" class="link" href="./User/Registration">Register</a>
					</p>
					<%} else {%>
					<div class="avatar">
						<img src="<%=Model.User().Avatar%>" alt="User Avatar" />
						<%=Model.User().Id%>
					</div>
					<div class="nick">
						<%=Model.User().Name%>&nbsp;
						(<a class="link" href="./User/Registration?UserId=<%=Model.User().Id%>">Profile</a>) 
					</div>
					<a class="link" href="./User/Logoff?UserId=<%=Model.User().Id%>">Logoff</a>
					<%}%>
				</div>
			</div>
			
			<!-- utilizadores -->
			<div class="content">
				<div class="contentbar">Users</div>
				<div id="users" class="contentdata"></div>
			</div>
		</div>
		
		<!-- CENTER -->
		<div id="center"></div>
		
		<!-- RIGHT -->
		<div id="right">
		
			<!-- Meus Jogos e jogos activos -->
			<div class="content">
				<div class="contentbar">Games</div>
				<div id="games" class="contentdata"></div>
			</div>
			
			<!-- Convites feitos ao utiliador -->
			<div class="content">
				<div class="contentbar">Invitations</div>
				<div id="invitations" class="contentdata"></div>	
			</div>
			
		</div>

	</div>

	<!-- ERROR PANEL -->
	<div id="error" class="hidden">
		<div class="content"></div>
		<a class="link" href="#">close</a>
	</div>
	
	<%-- Inicializa actualização --%>
	<script type="text/javascript">
		var u;
		<%if(Model.User() != null) {%>
		eval('u={<%=Model.User().ToString()%>}');
		<%}%>
		HomeController.init(u);
	</script>
</body>
</html>
