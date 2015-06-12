window.stackit = window.stackit || {};
window.stackit.htmlUtils = {
	escape: function(s) {
		return String(s)
			.replace(/&(?!\w+;)/g, '&amp;')
			.replace(/</g, '&lt;')
			.replace(/>/g, '&gt;')
			.replace(/"/g, '&quot;');
	}
};