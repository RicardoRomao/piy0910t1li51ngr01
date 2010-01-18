/// <reference path="jquery-1.3.2-vsdoc.js" />
/// <reference path="HomeView.js" />

/*!
* UTIL: parseQueryString
* Função utilitária para dividir uma query string.
* Suporta apenas URL's simples do tipo gerado pelos "ActionResult" deste projecto.
*/
function parseQueryString(s) {
	var d = {};
	var u = s.split('?');
	if(u[1]) {
		var i;
		var q = u[1].split('&');
		for(var p in q) {
			i=q[p].indexOf('=');
			if(i>0) d[q[p].substring(0,i)]=q[p].substring(++i);
			else d[q[p]]='';
		}
	}
	return {url:u[0],data:d};
}


/*!
* MVC:HomeController
* Controller para a home-page
*/
var HomeController = { init: function(user) {

	//garante unicidade
	if (this.refresh) return;
	
	//PARÂMETROS
	var USER_UPDATE_CICLE = 5000;
	var GAME_UPDATE_CICLE = 5000;
	var GAME_PLAYUP_CICLE = 1000;

	//definir utilizador actual
	var c_User = user;

	//inicializa view
	HomeView.init(user);



	//-------------------------------------
	//AJAX: Update a listas de utilizadores
	//-------------------------------------

	var usersUpdateDLY  = USER_UPDATE_CICLE;
	var usersUpdateFifo = new AJAXFifo();
	var invokeUsersUpdate = function() {
		usersUpdateFifo.add('./Home/UpdateUsers', { UserId: c_User.id }, callbackUsersUpdate);
	};
	var callbackUsersUpdate = function(data) {
		var r = eval('('+data+')');
		HomeView.updateUserList(r);
		setTimeout(invokeUsersUpdate, usersUpdateDLY);			
	}



	//------------------------------
	//AJAX: Update a listas de jogos
	//------------------------------

	var gamesUpdateDLY  = GAME_UPDATE_CICLE;
	var gamesUpdateFifo = new AJAXFifo();
	var invokeGamesUpdate = function() {
		gamesUpdateFifo.add('./Home/UpdateGames', { UserId: c_User.id }, callbackGamesUpdate);
	};
	var callbackGamesUpdate = function(data) {
		var r = eval('('+data+')');
		HomeView.updateGameList(r);
		setTimeout(invokeGamesUpdate, gamesUpdateDLY);
	}


	//INICIALIZAR ACTUALIZAÇÕES
	if(c_User) {
		invokeUsersUpdate();
		invokeGamesUpdate();
	}



	//------------------------------------------
	//AJAX: Request c/Resposta no painel central
	//------------------------------------------
	
	//actualizações da área central interactiva
	var commandFifo = new AJAXFifo(5000,'html');

	//handler de pedidos cuja resposta vai para o painel central
	this.handleCenterRequest = function(url) {
	
	//	alert('ajax:calling....' + url);
	
		var ud = parseQueryString(url);
		if(!ud.data) ud.data = {};
		if(c_User) ud.data.UserId = c_User.id;
		commandFifo.add(ud.url, ud.data, callbackCenterRequest, errorOnCenterRequest);
		return false;
	};


	//callback de pedidos cuja resposta vai para o painel central
	//tem que detectar o caso especial de erro recebido.	
	var callbackCenterRequest = function(data) {
		if(data.indexOf('<div class="error">') > 0) {
			HomeView.error(data);
		} else {
			HomeView.updateCenterPanel(data);
		}
	};
	
	//erro não previsto na comunicação ou no servidor
	var errorOnCenterRequest = function(xhr,msg,exc) {
		HomeView.error(AJAXFormatErrorViaXHR(xhr));
	};



	//-----------------------------------------------------------
	//CONTROLO DE JOGO (ACÇÕES PARA ACTUALIZAR TABULEIRO E JOGAR)
	//-----------------------------------------------------------


	//objectos de controlo
	var gamePlayFifo = new AJAXFifo();
	var c_GameId;
	var c_NextPlayer;
	
	//update ao estado de jogo em curso
	var invokeGamePlayUpdate = function() {
		gamePlayFifo.add('./Game/GetUpdate', {UserId: c_User.id, GameId: c_GameId}, callbackGamePlay, errorOnCenterRequest);
	}
	
	//trigger para inicializar actualizações
	this.triggerGamePlay = function(rows,cols,id) {
		HomeView.buildGameBoard(rows,cols);
		c_GameId = id;
		invokeGamePlayUpdate();
		HomeView.setPlayStatus('Updating...');
	};

	//handler para executar uma jogada
	this.handleGamePlay = function(ev) {
		if (c_NextPlayer != c_User.id) {
			alert('Sorry, next to play is "' + c_NextPlayer + '"...');
			return;
		}
		var data = {UserId: c_User.id, GameId: c_GameId, PlayAt: ev.data}
		gamePlayFifo.add('./Game/Play', data, callbackGamePlay, errorOnCenterRequest);
		return false;
	};

	//callback de execução de jogada
	var callbackGamePlay = function(data) {
		if(!c_GameId) return;
		var r = eval('('+data+')');

		//alert('feedback...');
		
		//erro
		if(r.invalid) {
			HomeView.setStatus(r.invalid[0]);
			HomeView.error(r.invalid[0]);
			return;
		}
				
		//players
		HomeView.updatePlayers(r.players);
		
		//next player
		if (r.game.nextplayer) c_NextPlayer = r.game.nextplayer;

		//celulas destapadas
		if (r.game.cells) HomeView.updateCells(r.game.cells);


		//game status
		var msg = r.game.status + ', ' + r.game.mines + ' remaining, ';
		if (r.game.status == 'ENDED') msg += c_NextPlayer + 'WINS!';
		else  msg += 'Next: ' + c_NextPlayer;
		HomeView.setPlayStatus(msg);
		if(r.reply == 'play' && r.game.status == 'ENDED')
				alert('Game ENDED:\nWinner is ' + c_NextPlayer);
		
		//refresh;
		if(r.reply == 'update') setTimeout(invokeGamePlayUpdate,GAME_PLAYUP_CICLE);
		
	};
	



}};


//===================================
// STARTUP DA PÁGINA (APÓS BODY.LOAD) 
//===================================
//$(document).ready(function() { Controller.init(); });
