function AutoCompleteMultiSelect() {
    this._elementId = "";
    this._list = [];
    this._load = [];
    this._readOnly = false;

    this.AttachmentId = function (id) {
        if (id) {
            this._elementId = id;
            return;
        }
    }

    this.List = function (list) {
        if (list) {
            this._list = list;
            return;
        }
    }

    this.Load = function (list) {
        if (list) {
            this._load = list;
            return;
        }
    }

    this.ReadOnly = function (flag) {
        this._readOnly = flag ? true : false;
    }
}

AutoCompleteMultiSelect.prototype.Setup = function () {
    var attachmentId = this._elementId;
    var inputId = attachmentId + "-input";
    var list = this._list;
    var load = this._load;
    var readOnly = this._readOnly;

    function split(val) {
        return val.split(/,\s*/);
    }

    function extractLast(term) {
        return split(term).pop();
    }

    function addLozenge(group, input) {
        var span = $("<span>").addClass("autocomplete-multi-select-lozenge").text(group);

        if (!readOnly) {
            var delIcon = $("<span>").addClass("glyphicon glyphicon-remove autocomplete-multi-select-glyphicon-remove").attr({
                title: "Remove " + group
            });

            delIcon.appendTo(span);
        }

        span.insertBefore(input);
        input.val("").css("top", 2);

        //Remove from available groups...
        var idx = list.indexOf(group);
        list.splice(idx, 1);
    }

    $(attachmentId).addClass("form-control autocomplete-multi-select-form-control");

    if (!readOnly) {
        var input = $("<input>").addClass("autocomplete-multi-select-input").attr({
            id: inputId,
            type: "text"
        });

        input
            // don't navigate away from the field on tab when selecting an item
            .bind("keydown", function (event) {
                if (event.keyCode === $.ui.keyCode.TAB &&
                    $(this).autocomplete("instance").menu.active) {
                    event.preventDefault();
                }
            })
            .autocomplete({
                minLength: 0,
                source: function (request, response) {
                    // delegate back to autocomplete, but extract the last term
                    response($.ui.autocomplete.filter(
                        list.sort(), extractLast(request.term)));
                },
                focus: function () {
                    // prevent value inserted on focus
                    return false;
                },
                select: function (event, ui) {
                    var group = ui.item.value;

                    addLozenge(group, input);
                    return false;
                },
                change: function () {
                    input.val("").css("top", 2);
                }
            });

        $(attachmentId).append(input);

        $(attachmentId).on("click", ".autocomplete-multi-select-glyphicon-remove", function (event) {
            var parent = $(event.target).parent();

            parent.remove();

            //Add to available groups...
            list.push(parent.text());
        });
    }

    if (load) {
        load.forEach(function (group) {
            addLozenge(group, input);
        });
    }

}

AutoCompleteMultiSelect.prototype.RemoveAllLozenges = function () {
    var attachmentId = this._elementId;
    var list = this._list;

    $(attachmentId).children("span").each(function () {
        $(this).remove();

        //Add to available groups...
        list.push($(this).text());
    });
}

AutoCompleteMultiSelect.prototype.ListToHiddenFields = function () {
    var attachmentId = this._elementId;

    $(attachmentId).children("span").each(function (idx) {
        var hidden = $("<input>").attr({
            id: "Groups_" + idx,
            type: "hidden",
            value: $(this).text(),
            name: "Groups[" + idx + "]"
        });

        $(attachmentId).append(hidden);
    });
}