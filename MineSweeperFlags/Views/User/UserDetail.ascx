<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MineSweeperFlags.Models.UserDetail>" %>
<%@ Import Namespace="MineSweeperFlagsLib" %>
<div class="contentbar">User Detail</div>
<div class="panel">
	<table>
		<tr>
			<td rowspan="2"><img src="<%= Model.DetailUser().Avatar %>" alt="User Avatar" /></td>
			<td><%= Model.DetailUser().Id %> ( <a href="mailto:<%= Model.DetailUser().Email %>"><%= Model.DetailUser().Email %></a> )</td>
		</tr>
		<tr>
			<td><a class="link" href="./Game/NewGame?RefUser=<%= Model.DetailUser().Id %>">Challange <%= Model.DetailUser().Id %> to new game...</a></td>
		</tr>
	</table>
	<span class="title">Games:</span>
	<ul>
	<% foreach(Game g in Model.GameList()) { %>
		<li>
		<b>#<%= g.Id %></b>,
		<%= g.State.ToString() %>,
		you: <%= (g.Owner() == Model.User() ? "OWNER" : g.Challange(Model.User()).ToString()) %>.
		<% if(g.State == Game.GameState.ACTIVE) { %>
			<a class="link" href="./Game/Continue?GameId=<%= g.Id %>">Play</a>&nbsp;
		<% } else if(g.Challange(Model.User()) == Game.ChallangeState.NEW) { %>
			<a class="link" href="./Game/AcceptChallange?GameId=<%= g.Id %>">Accept</a>&nbsp;
			<a class="link" href="./Game/RefuseChallange?GameId=<%= g.Id %>">Refuse</a>&nbsp;
		<% } else if(g.Owner() == Model.User() && g.Ready()) { %>
				<a class="link" href="./Game/Start?GameId=<%= g.Id %>">Start</a>&nbsp;
		<% } %>
		</li>
	<% } %>
	</ul>
</div>

