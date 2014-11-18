Common.namespace("Studio").Editor = function () {
    var modes = {
        "xml" : ace.require("ace/mode/xml").Mode,
        "json" : ace.require("ace/mode/json").Mode
    }
    var languageTools = ace.require("ace/ext/language_tools");

    var pThis = this;
    var aceEditor;

    pThis.events =
    {
        change: Common.Events.createEventHandler(),
        focus: Common.Events.createEventHandler(),
        blur: Common.Events.createEventHandler()
    };

    pThis.initialize = function (params) {
        pThis.parameters = params;

        aceEditor = ace.edit(pThis.parameters.containerId);
        aceEditor.setTheme("ace/theme/textmate");
        aceEditor.setFontSize("12px");
        aceEditor.setHighlightActiveLine('checked');
        aceEditor.setHighlightSelectedWord(true);
        aceEditor.setDisplayIndentGuides(true);

        aceEditor.getSession().setMode(new modes[pThis.parameters.mode || 'xml']);
        aceEditor.renderer.setShowPrintMargin(false);
        aceEditor.setReadOnly(params.readOnly == undefined ? false : params.readOnly);
        aceEditor.getSession().on('change', function () {

            pThis.events.change.raise({ newValue: pThis.value() });

        });

        if (params.onLiveAutocomplete)
        {
            aceEditor.setOptions({
                enableLiveAutocomplete: true
            });

            aceEditor.completers = [{
                getCompletions: params.onLiveAutocomplete
            }];

            var liveAutocompleter = 
            {
                getCompletions: params.onLiveAutocomplete
            }
            
            languageTools.addCompleter(liveAutocompleter);
        }

        pThis.value('');
        pThis.resize();

        Common.Events.PageResize.attach(function () { pThis.resize(); });
    }

    pThis.dispose = function () {

    }

    pThis.selectedText = function (value) {
        if (arguments.length == 0) {
            return aceEditor.session.getTextRange(aceEditor.getSelectionRange());
        }
        else {
            aceEditor.insert(value);
        }
    }

    pThis.insert = function (value) {
        aceEditor.insert(value);
    }

    pThis.value = function (newValue) {
        if (arguments.length == 0) {
            return aceEditor.getSession().getValue();
        }
        else {
            aceEditor.getSession().setValue(newValue);
        }
    }

    pThis.focus = function () {
        aceEditor.focus();
    }

    pThis.resize = function (width, height) {
        var $container = $('#' + pThis.parameters.containerId);
        var $wrapper = $container.parent();

        $container.width(width || $wrapper.width());
        $container.height(height || $wrapper.height());

        aceEditor.resize();
    }

}