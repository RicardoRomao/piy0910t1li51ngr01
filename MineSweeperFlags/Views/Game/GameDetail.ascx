<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MineSweeperFlags.Models.GameData>" %>
<%@ Import Namespace="MineSweeperFlagsLib" %>
<% if(!Model.HasError) {%>
<% Game m_game = Model.Game(); %>
<div class="contentbar">
Game #<%= m_game.Id %>,
created by <%= m_game.Owner().Id %>,
<%= m_game.State.ToString() %>
<%= (m_game.Ready() ? "(CAN START)" : "") %>
</div>
<div class="panel">
	<table><tbody><tr><td>
		<% if(m_game.State == Game.GameState.ENDED) { %>
			Game Winner: <%=m_game.Engine().NextPlayer().Id %>
			<a class="link" href="./Game/Abandon?GameId=<%= m_game.Id %>">Delete</a>&nbsp;
		<% } else if(m_game.State == Game.GameState.ACTIVE) { %>
			<a class="link" href="./Game/Continue?GameId=<%= m_game.Id %>">Play</a>&nbsp;
		<% } else if(m_game.Challange(Model.User()) == Game.ChallangeState.NEW) { %>
			<a class="link" href="./Game/AcceptChallange?GameId=<%= m_game.Id %>">Accept</a>&nbsp;
			<a class="link" href="./Game/RefuseChallange?GameId=<%= m_game.Id %>">Refuse</a>&nbsp;
		<% } else if(m_game.Owner() == Model.User() && m_game.Ready()) { %>
			<a class="link" href="./Game/Start?GameId=<%= m_game.Id %>">Start</a>&nbsp;
		<% } %>
	</td></tr></tbody></table>
	
	<% if (Model.Candidates() != null) { %>
		<hr />
		<span class="title">
			<b><u>Other Players You can Challange</u></b>
		</span>
		<table>
		<% foreach(User u in Model.Candidates()) { %>
			<tr>
				<td><img src="<%= u.Avatar %>" alt="Avatar for <%= u.Id %>" /></td>
				<td>
					<a class="link" href="./User/UserDetail?DetailId=<%= u.Id %>"><%= u.Id %></a>&nbsp;
					<a class="link" href="./Game/AddChallange?GameId=<%= Model.Game().Id %>&RefUser=<%= u.Id %>">Challange</a>
				</td>
			</tr>
		<%}%>
		</table>
	<% } %>
	
	<hr />
	<span class="title">
		<b><u>Players</u></b>
		<% if (m_game.Owner() == Model.User() && m_game.State == Game.GameState.CHALLANGE && Model.Candidates() == null) { %>
			(&nbsp;<a class="link" href="./Game/InviteOthers?GameId=<%= m_game.Id %>">Invite Others</a>&nbsp;);
		<% } %>
	</span>
	<table>
	<% foreach(User u in m_game.Users()) { %>
		<tr>
			<td><img src="<%= u.Avatar %>" alt="Avatar for <%= u.Id %>" /></td>
			<td>
				<% if(u == Model.User()) { %>
				<%= u.Id %> (you)
				<% } else { %>
				<a class="link" href="./User/UserDetail?DetailId=<%= u.Id %>"><%= u.Id %></a>
				<% } %>
				<br />
				<%= ((m_game.Owner() == u) ? "CREATOR" : m_game.Challange(u).ToString()) %>
			</td>
		</tr>
	<%}%>
	</table>	
</div>

<% } else { %>
<div class="error">
<%= Html.ValidationSummary("Invalid action") %>
</div>
<% } %>
