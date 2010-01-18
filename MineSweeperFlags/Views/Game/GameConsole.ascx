<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MineSweeperFlags.Models.GameData>" %>
<%@ Import Namespace="MineSweeperFlagsLib" %>
<% if(String.IsNullOrEmpty(Html.ValidationSummary())) {%>
<div class="contentbar">
Game #<%= Model.Game().Id%>,
created by <%= Model.Game().Owner().Id%>,
<%= Model.Game().State.ToString()%>.
</div>
<div class="panel">
	<div class="status">Updating...</div>
	<div class="gameboard"></div>
	<div class="actions">
		<a class="link" href="./Game/Abandon?GameId=<%= Model.Game().Id %>">Quit game (and loose)</a>&nbsp;
	</div>
	<div class="players">
		<% foreach(User u in Model.Game().Users()) { %>
			<div class="userItem"><img width="20" height="20" src="<%= u.Avatar %>" alt="Avatar for <%= u.Id %>" />&nbsp;<%= u.Id %></div>
		<%}%>
	</div>
</div>
<script type="text/javascript">HomeController.triggerGamePlay(<%= Game.GAME_ROWS %>,<%= Game.GAME_COLS %>,<%= Model.Game().Id %>);</script>

<% } else { %>
<div class="error">
<%= Html.ValidationSummary("Invalid action") %>
</div>
<% } %>
