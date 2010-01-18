/// <reference path="jquery-1.3.2-vsdoc.js" />
/// <reference path="HomeController.js" />

/*!
* MVC:HomeView
* View para a home-page
*/
var HomeView = { init: function(user) {

	//garante unicidade
	if (this.updateUserList) return;

	//attach de eventos
	var attachShowUserDetail = function(obj,id) { obj.click(function(ev){ HomeController.handleCenterRequest('./User/UserDetail?DetailId='+id); }); };
	var attachShowGameDetail = function(obj,id) { obj.click(function(ev){ HomeController.handleCenterRequest('./Game/GameDetail?GameId='+id); }); };

	//referências DOM(jquery)
	var $users   = $('#users');
	var $invts   = $('#invitations');
	var $games   = $('#games');
	var $center  = $('#center');

	//definir status
	var $status  = $('#status');
	this.setStatus = function(text) { $('div',$status).html(text); };

	//definir error
	var $error   = $('#error').hide('fast').removeClass('hidden');
	$('a',$error).click(function(){ $error.hide('normal'); return false; });	
	this.error = function(data) { $('div',$error).html(data); $error.show('normal'); };


	//actualizar lista de utilizadores
	this.updateUserList = function(users) {
		$users.empty();
		if(users) {
			for(var i = 0; i < users.length; i++) {
				var $icon = $('<img>')
					.attr('alt',(users[i].online ? '*on' : 'off'))
					.attr('src', users[i].icon);
				var $user = $('<div>').addClass('userItem')
					.append($icon)
					.append($('<span>').append(users[i].id))
					.append('(')
					.append($('<span>').append((users[i].games ? users[i].games : '0')))
					.append(')')
				attachShowUserDetail($user, users[i].id);
				$users.append($user);
			}
		} else $users.text('No users...');
	};

	//actualizar lista de jogos e de convites
	this.updateGameList = function(games) {
		var hinv = false;
		var hgam = false;
		$games.empty();
		$invts.empty();
		if(games) {
			for(var i = 0; i < games.length; i++) {
				var $icon = $('<img>')
					.attr('alt', games[i].state)
					.attr('src', games[i].icon);
				var $game = $('<div>').addClass('gameItem')
					.append($icon)
					.append('#')
					.append($('<span>').append(games[i].id))
					.append('(inv:')
					.append($('<span>').append(games[i].owner))
					.append('):')
					.append($('<span>').append(games[i].state));
				if(games[i].you_next) $game.append(', YOU NEXT'); 
				attachShowGameDetail($game, games[i].id);
				if(games[i].you_ans && !games[i].you_own) {
					hinv = true;
					$invts.append($game);
				} else {
					hgam = true;
					$games.append($game);
				}
			}
		}
		if(!hgam) $games.text('No games...');
		if(!hinv) $invts.text('No invitations...');
	};
	
	
	//actualização (por callback com HTML) do painel central
	this.updateCenterPanel = function(data) {
		$center.html(data);
		$('a.link',$center).click(function() { return HomeController.handleCenterRequest(this.href); });
	};



	//-----------------------------------------------------------
	//CONTROLO DE JOGO (ACÇÕES PARA ACTUALIZAR TABULEIRO E JOGAR)
	//-----------------------------------------------------------


	//a resolver...
	var resolveGameIcon = function(icon) { return ('./Images/MineSweeper/' + icon); };

	//objectos de jogo
	var $play_status;
	var $play_players;
	var $play_board;
	var play_BoardCells = [];

	//construir tabuleiro de jogo (div board).
	this.buildGameBoard = function(rows, cols) {
		$play_status = $('div.panel > div.status', $center);
		$play_players = $('div.panel > div.players', $center);
		$play_board = $('.gameboard', $center);
		play_BoardCells = [];
		var glin = false;
		var rxc = rows * cols;
		for (var i = 0; i < rxc; i++) {
			var cell = $('<div>').addClass('cell');
			var cimg = $('<img>').addClass('cellImage').attr('src', resolveGameIcon('hidden.png'));
			var clnk = $('<a>').addClass('cellLink').bind('click', i, HomeController.handleGamePlay);
			clnk.append(cimg);
			cell.append(clnk);
			play_BoardCells[play_BoardCells.length] = cimg;
			if ((i % cols) == 0) {
				if (glin) $play_board.append(glin);
				glin = $('<div>').addClass('cellLine');
			}
			glin.append(cell);
		}
		$play_board.append(glin);
		

	};
	
	//actualizar tabuleiro após uma jogada
	//também destapa tudo após o jogo terminado
	this.updateCells = function(cells) {
		for (var i = 0; i < cells.length; ++i) {
			if (cells[i].content == 9)
				play_BoardCells[cells[i].index].attr('src', resolveGameIcon('mineBlown.png'));
			else 
				play_BoardCells[cells[i].index].attr('src', resolveGameIcon('marked' + cells[i].content + '.png'));
		}
	};

	//actualizar jogadores (no jogo)
	this.updatePlayers = function(users) {
		$play_players.empty();
		if(!users) return;
		for(var i = 0; i < users.length; i++) {
			var $icon = $('<img>')
				.attr('alt', users[i].id)
				.attr('width','20')
				.attr('height','20')
				.attr('src', users[i].avatar);
			var $user = $('<div>').addClass('userItem')
				.append($icon)
				.append($('<span>').append(users[i].id))
				.append(', score ')
				.append($('<span>').append(users[i].score))
				.append('.');
			if(users[i].next) $user.append(' IS NEXT.'); 
			$play_players.append($user);
		}
	};

	//actualizar status do Jogo...
	this.setPlayStatus = function(text) { $play_status.html(text); }

}};