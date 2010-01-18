/// <reference path="jquery-1.3.2-vsdoc.js" />
/*!
* Wrapper para chamadas AJAX: 
* As chamadas a funçõe de actualização no servidor para
* cada área distinta do View devem ser todas executadas
* usando o método add de um objecto AJAXFifo especifico
* dessa área de actualização.
*/

//propriedades ajax por omissão
$.ajaxSetup({
	dataType: 'text',
	timeout : 5000,
	global  : false
});

//Fifo para chamadas ao servidor
function AJAXFifo(timeout, rtnType) {
	var _fifo = [];
	var _work = false;
	var _call = function(c) {
		var op = {complete: _done};
		if(timeout) op.timeout = timeout;
		if(rtnType) op.dataType = rtnType;
		if(c.callback) op.success = c.callback;
		if(c.errorfn) op.error = c.errorfn;
		if(c.$form) {
			if(c.beforefn) op.beforeSubmit = c.beforefn;
			c.$form.ajaxSubmit(op);
		} else {
			op.url = c.url;
			op.type = c.method;
			if(c.data) op.data = c.data;
			$.ajax(op);
		}
	}
	var _done = function(xhr, msg) {
		_work = false;
//		if(msg=='error') {
//			alert('ajax.js:Request error:' + xhr.status + ':' + xhr.statusText);
//		}
		_next();
	}
	var _next = function() {
		if(_work) return;
		if(_fifo.length > 0) {
			_work = true;
			_call(_fifo.shift());
		}
	}
	//executar uma chamada normal
	this.add = function(url, data, callback, errorfn, method) {
		if(!url) return;
		_fifo.push({url:url, data:data, callback:callback, errorfn:errorfn, method:(method?method:'GET')});
		_next();
	}
	//executar uma chamada para submissão de form
	this.addForm = function($form, callback, errorfn, beforefn) {
		if(!$form || !$form[0]) return;
		_fifo.push({$form:$form, callback:callback, errorfn:errorfn, beforefn:beforefn});
		_next();
	}
}


//Auxiliar para formatar erro via XHR
function AJAXFormatErrorViaXHR(xhr) {
	if(!xhr) return 'xhr is null...';
	return ('<p><b>' + xhr.status + ':' + xhr.statusText + '</b></p>'
	+ '<div>' + xhr.responseText + '</div>');
}
