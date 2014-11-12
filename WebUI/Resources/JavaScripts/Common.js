(function () {
    if (!window.Common) {
        window.Common = {};
    }

    var common = window.Common;

    common.$ = function (id) {
        if (document.all) {
            return document.all[id];
        }

        return document.getElementById(id);
    }

    common.formatString = function (template, args) {
        var result = template;

        for (var i = 1; i < arguments.length; i++) {
            var regex = new RegExp('\\{' + (i - 1) + '\\}', 'gi');
            result = result.replace(regex, arguments[i]);
        }

        return result;
    }

    common.formatRegEx = function (clearPattern, formatPattern, formatTemplate) {
        return function (value) {
            if (!value || !value.length) {
                return value;
            }

            var clearRe = new RegExp(clearPattern, 'gi');
            value = value.replace(clearRe, '');

            if (formatPattern && formatTemplate) {
                var formatRe = new RegExp(formatPattern, 'gi');
                value = value.replace(formatRe, formatTemplate);
            }

            return value;
        }
    }

    common.repairNumber = function (value) {
        var isNegativeRegExp = new RegExp('^[\\D ]*-', 'gi');
        var searchFractionRegExp = new RegExp('^(.*)(?:[.,]+(.+))$', 'gi');
        var cleanRegExp = new RegExp('[\\D ]+', 'gi');

        var fractionMatch = searchFractionRegExp.exec(value);
        var isNegative = isNegativeRegExp.test(value);

        if (!fractionMatch) {
            return value.replace(cleanRegExp, '') * (isNegative ? -1 : 1);
        }

        return (fractionMatch[1].replace(cleanRegExp, '') + '.' + fractionMatch[2].replace(cleanRegExp, '')) * (isNegative ? -1 : 1);
    }

    common.newGuid = function () {
        return (parseInt(Math.random() * (new Date()).getTime() * 100000000)).toString();
    }

    common.newInt = function () {
        return Math.round(Math.random() * (new Date()).getTime() / 100000);
    }

    common.encodeHtml = function (html) {
        if (html == undefined || html == null) {
            return '';
        }

        var result = '';

        for (var i = 0; i < html.length; i++) {
            var charCode = html.charCodeAt(i);
            var ch = html.charAt(i);

            if (charCode > 0xA0 && charCode < 0x100) {
                result += '&#' + charCode + ';';
            }
            else if (ch == '&') {
                result += "&amp;";
            }
            else if (ch == '<') {
                result += "&lt;";
            }
            else if (ch == '>') {
                result += "&gt;";
            }
            else if (ch == '"') {
                result += "&quot;";
            }
            else {
                result += ch;
            }
        }

        return result;
    }

    common.decodeHtml = function (input)
    {
        var charToEntityRegex,
            entityToCharRegex,
            charToEntity,
            entityToChar;

        function resetCharacterEntities() {
            charToEntity = {};
            entityToChar = {};
            // add the default set
            addCharacterEntities({
                '&amp;': '&',
                '&gt;': '>',
                '&lt;': '<',
                '&quot;': '"',
                '&#39;': "'"
            });
        }

        function addCharacterEntities(newEntities) {
            var charKeys = [],
                entityKeys = [],
                key, echar;
            for (key in newEntities) {
                echar = newEntities[key];
                entityToChar[key] = echar;
                charToEntity[echar] = key;
                charKeys.push(echar);
                entityKeys.push(key);
            }
            charToEntityRegex = new RegExp('(' + charKeys.join('|') + ')', 'g');
            entityToCharRegex = new RegExp('(' + entityKeys.join('|') + '|&#[0-9]{1,5};' + ')', 'g');
        }

        function htmlEncode(value) {
            var htmlEncodeReplaceFn = function (match, capture) {
                return charToEntity[capture];
            };

            return (!value) ? value : String(value).replace(charToEntityRegex, htmlEncodeReplaceFn);
        }

        function htmlDecode(value) {
            var htmlDecodeReplaceFn = function (match, capture) {
                return (capture in entityToChar) ? entityToChar[capture] : String.fromCharCode(parseInt(capture.substr(2), 10));
            };

            return (!value) ? value : String(value).replace(entityToCharRegex, htmlDecodeReplaceFn);
        }

        resetCharacterEntities();

        return htmlDecode(input);
    }

    common.extend = function (sourceObject, newObject) {
        var typeName = common.getType(newObject);

        if (typeName != 'array' && typeName != 'object') {
            if (typeName == 'date') {
                return new Date(newObject);
            }

            return newObject;
        }

        if (common.getType(sourceObject) != typeName) {
            if (typeName == "array") {
                sourceObject = [];
            }
            else {
                sourceObject = {};
            }
        }

        for (var i in newObject) {
            if (typeName == 'array') {
                sourceObject[i] = common.clone(newObject[i]);
            }
            else {
                sourceObject[i] = common.extend(sourceObject[i], newObject[i]);
            }
        }

        return sourceObject;
    }

    common.clone = function (sourceObject) {
        var typeName = common.getType(sourceObject);

        if (typeName != 'array' && typeName != 'object') {
            if (typeName == 'date') {
                return new Date(sourceObject);
            }

            return sourceObject;
        }

        var result = typeName == 'array' ? [] : {};

        for (var i in sourceObject) {
            result[i] = common.clone(sourceObject[i]);
        }

        return result;
    }

    common.surfaceClone = function (obj, withoutPrototypes) {
        var typeName = Common.getType(obj);

        if (typeName != 'array' && typeName != 'object') {
            return obj;
        }

        var result = (typeName == 'array') ? [] : {};

        for (var i in obj) {
            if (!withoutPrototypes || obj.hasOwnProperty(i)) {
                result[i] = obj[i];
            }
        }

        return result;
    }

    common.isNumericValue = function (value) {
        return value * 1 == value;
    }

    common.getType = function (obj) {
        if (obj == null) {
            return 'null';
        }

        if (obj instanceof Array) {
            return 'array';
        }

        if (obj instanceof Date) {
            return 'date';
        }

        if (obj == undefined) {
            return 'undefined';
        }

        if (typeof (obj) == 'number') {
            if (parseInt(obj) == Number(obj)) {
                return 'integer';
            }

            return 'float';
        }

        if (typeof (obj) == 'string') {
            return 'string';
        }

        if (typeof (obj) == 'object') {
            return 'object';
        }

        return typeof (obj) || 'other';
    }

    common.ensureProperties = function (obj, props) {
        for (var i = 1; i < arguments.length; i++) {
            if (obj[arguments[i]] == undefined) {
                throw (['Propery missing: ', arguments[i], '.\n', arguments.callee.caller]).join('');
            }
        }
    }

    common.getDate = function (date) {
        date = date || new Date();

        return ({

            year: date.getFullYear(),
            month: date.getMonth(),
            day: date.getDate(),
            dayOfWeek: date.getDay(),
            hours: date.getHours(),
            minutes: date.getMinutes(),
            minutesFull: date.getMinutes() < 10 ? '0' + date.getMinutes() : date.getMinutes() + '',
            seconds: date.getSeconds(),
            milliseconds: date.getMilliseconds()

        });
    }

    common.parseMsDate = function (date) {
        return new Date(parseInt(date.substr(6)));
    };

    common.dateHelper = {};

    common.dateHelper.getMaxMonthDay = function (year, month) {
        var yearValue = parseInt(year) || 0;
        var isLeapYear = yearValue % 400 == 0 || (yearValue % 4 == 0 && yearValue % 100 != 0);
        month = parseInt(month);

        switch (month) {
            case 0:
            case 2:
            case 4:
            case 6:
            case 7:
            case 9:
            case 11:
                return 31;

            case 1:
                return isLeapYear ? 29 : 28;

            case 3:
            case 5:
            case 8:
            case 10:
                return 30;
        }
    };

    common.dateHelper.validateYear = function (year) {
        var yearValue = parseInt(year, 10) || 0;

        if (yearValue < 0) {
            return 1900;
        }
        else if (yearValue < 30) {
            return 2000 + yearValue;
        }
        else if (yearValue < 100) {
            return 1900 + yearValue;
        }
        else if (yearValue < 300) {
            return 1800 + yearValue;
        }
        else if (yearValue < 1900) {
            return 1900;
        }
        else if (yearValue > 2100) {
            return 2100;
        }

        return year;
    };

    common.dateHelper.validateDay = function (day) {
        var dayValue = parseInt(day, 10) || 0;

        if (dayValue <= 0) {
            return '01';
        }
        else if (dayValue < 10) {
            return '0' + dayValue.toString();
        }

        return day;
    };

    common.dateHelper.getMonthHtml = function (month) {
        var options =
            [
                { title: "января", id: "0" },
                { title: "февраля", id: "1" },
                { title: "марта", id: "2" },
                { title: "апреля", id: "3" },
                { title: "мая", id: "4" },
                { title: "июня", id: "5" },
                { title: "июля", id: "6" },
                { title: "августа", id: "7" },
                { title: "сентября", id: "8" },
                { title: "октября", id: "9" },
                { title: "ноября", id: "10" },
                { title: "декабря", id: "11" }
            ];

        var select = [];

        select.push('<select class="month">');

        for (var i = 0; i < options.length; i++) {
            select.push('<option');

            if (options[i].id == month) {
                select.push(' selected="selected"');
            }

            select.push(' value="' + options[i].id + '">' + options[i].title + "</option>");
        }

        select.push('</select>');

        return select.join('');
    };

    common.dateHelper.validateDate = function (year, month, day) {
        return day && month && year;
    };

    common.dateHelper.compareDate = function (a, b) {
        return a && b && a.getFullYear() == b.getFullYear() && a.getMonth() == b.getMonth() && a.getDate() == b.getDate();
    };

    common.monthNamesNominative = ["Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"];
    common.monthNamesGinitive = ["Января", "Февраля", "Марта", "Апреля", "Мая", "Июня", "Июля", "Августа", "Сентября", "Октября", "Ноября", "Декабря"];
    common.monthNamesShort = ["Янв", "Фев", "Мар", "Апр", "Май", "Июн", "Июл", "Авг", "Сент", "Окт", "Ноя", "Дек"];
    common.dayNames = ["Воскресение", "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота"];
    common.dayNamesShort = ["Вс", "Пн", "Вт", "Ср", "Чт", "Пт", "Сб"];

    common.stringToIntArray = function (str) {
        var result = [];

        for (var i = 0; i < str.length; i++) {
            var character = str.substring(i, i + 1);
            result.push(parseInt(character));
        }

        return result;
    }

    common.countChars = function (input, text) {
        var start = 0;
        var count = 0;
        var pos = 0;
        var textLength = text.length;

        if (!input) {
            return count;
        }

        pos = input.indexOf(text, start);

        while (pos != -1) {
            count++;
            start = pos + textLength;
            pos = input.indexOf(text, start);
        }

        return count;
    }

    common.addZerosToStart = function (value, needLength) {
        var result = (value || '').toString();

        while (result.length < needLength) {
            result = '0' + result;
        }

        return result;
    }

    common.russianWordsComparer = function (left, right) {
        return wordsComparer(left, right, russianSymbolsComparer);
    }

    common.getWindowScrollTop = function () {
        return filterPositionResults(
            window.pageYOffset ? window.pageYOffset : 0,
            document.documentElement ? document.documentElement.scrollTop : 0,
            document.body ? document.body.scrollTop : 0
        );
    }

    common.namespace = function (space) {
        var current = window;

        space.split('.').customForEach(function (obj) {
            if (current[obj] === undefined) {
                current[obj] = {};
            }

            current = current[obj];
        });

        return current;
    }

    common.getHeight = function (element) {
        return getSize(element, 'Height');
    }

    function getSize(element, name) {
        return Math.max(
            element.documentElement['client' + name],
            element.documentElement['scroll' + name],
            element.documentElement['offset' + name],
            element.body['scroll' + name],
            element.body['offset' + name],
            element.body['client' + name]);
    }

    function wordsComparer(left, right, symbolsComparer) {
        if (!left) {
            return !right ? 0 : -1;
        }

        if (!right) {
            return 1;
        }

        for (var i = 0; i < left.length; ++i) {
            if (i == right.length) {
                return 1;
            }

            var symbolsCompareResult = symbolsComparer(left.charAt(i), right.charAt(i));

            if (symbolsCompareResult != 0) {
                return symbolsCompareResult;
            }
        }

        return (left.length == right.length) ? 0 : -1;
    }

    function russianSymbolsComparer(left, right) {
        if (left == right) {
            return 0;
        }

        if (left != 'ё' && right != 'ё' &&
            left != 'Ё' && right != 'Ё') {
            return symbolsComparer(left, right);
        }

        switch (left) {
            case 'ё':
                return (right == 'е') ? 1 : symbolsComparer('е', right);

            case 'Ё':
                return (right == 'Е') ? 1 : symbolsComparer('Е', right);

            default:
                return -1 * russianSymbolsComparer(right, left);
        }
    }

    function symbolsComparer(a, b) {
        if (a < b) {
            return -1;
        }

        return a > b ? 1 : 0;
    }

    function sign(number) {
        if (number < 0) {
            return -1;
        }

        return number > 0 ? 1 : 0;
    }

    function filterPositionResults(n_win, n_docel, n_body) {
        var n_result = n_win ? n_win : 0;

        if (n_docel && (!n_result || (n_result > n_docel))) {
            n_result = n_docel;
        }

        return n_body && (!n_result || (n_result > n_body)) ? n_body : n_result;
    }

})();


/* URL BUILDER */

(function () {

    var common = window.Common;

    if (!common.UrlBuilder) {
        common.UrlBuilder = {};
    }

    var urlBuilder = common.UrlBuilder;

    urlBuilder.create = function (url) {
        return new UrlBuilder(url);
    }

    function UrlBuilder(url) {

        var pThis = this;
        pThis.url = url || '';

        pThis.setQueryParam = function (name, value) {
            var paramRe = new RegExp('([&?])' + name + '(?:=([^&?=]+)?)?', 'gi');
            var newParam = createParam(name, value);

            if (pThis.url.indexOf('?') == -1) {
                pThis.url = pThis.url + '?' + newParam;
                return pThis.url;
            }

            var urlMatch = paramRe.exec(pThis.url);

            if (urlMatch) {
                pThis.url = pThis.url.replace(paramRe, urlMatch[1] + newParam);
                return pThis.url;
            }

            if (pThis.url.substring(pThis.url.length - 1, 1) != '&') {
                pThis.url = pThis.url + '&';
            }

            pThis.url = pThis.url + newParam;
            return pThis.url;
        }

        pThis.getQueryParamValue = function (name) {
            var paramRe = new RegExp('([&?])' + name + '(?:=([^&?=]+)?)?', 'gi');
            var urlMatch = paramRe.exec(pThis.url);
            return urlMatch && urlMatch[2];
        }

        function createParam(name, value) {
            return name + '=' + encodeURIComponent(value);
        }

        function getQueryString() {
            var queryStart = pThis.url.indexOf('?');

            if (queryStart == -1) {
                return '';
            }

            return pThis.url.substring(queryStart + 1);
        }
    }


})();


/* CONFIGURATION */

(function () {

    var common = window.Common;

    if (!common.Config) {
        common.Config =
        {
            urlBase: '/',
            asyncUrlBaseV2: '/Async/v2/',
            isWidgetMode: isWidgetMode(),
            navigationBar:
            {
                minWidthWidget: 937,
                minWidthRegular: 980
            },
            getDialogMaxWidth: function (desiredWidth) {
                return config.isWidgetMode ? Math.min(document.body.offsetWidth - 130, desiredWidth) : desiredWidth;
            },
            getDialogFieldMinWidth: function (desiredWidth) {
                return config.isWidgetMode ? 100 : desiredWidth;
            },
            getDialogFieldMaxWidth: function (desiredWidth) {
                return config.isWidgetMode ? Math.min(document.body.offsetWidth / 5 * 3, desiredWidth) : desiredWidth;
            },
            getWizardFieldMaxWidth: function () {
                return Math.max(config.isWidgetMode ? (document.body.offsetWidth / 3) : (screen.width - 860), 150);
            }
        };
    }

    function isWidgetMode() {
        return window.location.host.toLowerCase().indexOf('widget.') == 0 || window.location.host.toLowerCase().indexOf('www.widget.') == 0;
    }

    var config = common.Config;

})();

/* JSON */

(function () {

    var common = window.Common;

    if (!common.Json) {
        common.Json = {};
    }

    var json = common.Json;

    json.serialize = function (obj) {
        return JSON.stringify(obj);
    }

    json.deserialize = function (str) {
        return str ? JSON.parse(str) : undefined;
    }

})();

/* UI */

(function () {

    var common = window.Common;

    if (!common.UI) {
        common.UI = {};
    }

    var ui = common.UI;

    ui.renderButton = function (id, title, onClick, icon) {
        return ([
            '<span class="buttonContainer">',
                '<a', (id ? ' id="' + id + '"' : ''), (onClick ? ' onClick="' + onClick + '; return false;"' : ''), ' class="button" href="javascript:">\
                    <span class="bl">\
                        <span class="br">\
                            <span class="bm">',
                                (!icon ? '' : '<img src="/Resources/Layout/Images/Icons/' + icon + '" /> '),
                                '<span class="title">', title, '</span> \
                            </span>\
                        </span>\
                    </span>\
                </a> \
             </span>'
        ]).join('');
    }

    ui.renderRoundedBoxWithBorder = function (boxContent, properties) {
        return ([
            '<div ', (properties && properties.boxId ? 'id="' + properties.boxId + '" ' : ''), 'class="rounderCorners', (properties && ' ' + properties.boxClass ? properties.boxClass : ''), '">',
                '<div class="withBorders ' + (properties && properties.boxColor ? properties.boxColor : '') + ' ">\
                    <div class="line1"></div>\
                    <div class="line2"></div>\
                    <div class="line3"></div>\
                    <div class="line4"></div>\
                    <div class="line5"></div>\
                        <div class="content">',
                            boxContent,
                        '</div>\
                    <div class="line5"></div>\
                    <div class="line4"></div>\
                    <div class="line3"></div>\
                    <div class="line2"></div>\
                    <div class="line1"></div>\
                </div>',
            '</div>'
        ]).join('');
    }

    ui.renderBoxContent = function (header, text) {
        return (['<strong>', header, '</strong><br>', text, '']).join('');
    }

})();


/* CLIENT INFO */

(function () {

    var common = window.Common;

    if (!common.ClientInfo) {
        common.ClientInfo = {};
    }

    var client = common.ClientInfo;

    var ua = navigator.userAgent;

    client.isIE = (document.all && window.clientInformation) ? parseInt(window.clientInformation.userAgent.substr(window.clientInformation.userAgent.indexOf('MSIE ') + 5, 3)) : 0;
    client.isMSIE = (navigator.appName == "Microsoft Internet Explorer");
    client.isMSIE5 = client.isMSIE && (ua.indexOf('MSIE 5') != -1);
    client.isMSIE50 = client.isMSIE && (ua.indexOf('MSIE 5.0') != -1);
    client.isMSIE55 = client.isMSIE && (ua.indexOf('MSIE 5.5') != -1);
    client.isMSIE60 = client.isMSIE && (ua.indexOf('MSIE 6.0') != -1);
    client.isMSIE70 = client.isMSIE && (ua.indexOf('MSIE 7') != -1);
    client.isMSIE80 = client.isMSIE && (ua.indexOf('MSIE 8') != -1);
    client.isMSIE90 = client.isMSIE && (ua.indexOf('MSIE 9') != -1); // lol
    client.isGecko = ua.indexOf('Gecko') != -1;
    client.isGoogleChrome = ua.indexOf('Chrome') != -1;
    client.isSafari = ua.indexOf('Safari') != -1;
    client.isOpera = ua.indexOf('Opera') != -1;
    client.isWebKit = (!client.isIE && !client.isGecko && window.devicePixelRatio) ? true : false;
    client.isMac = ua.indexOf('Mac') != -1;
    client.isFireFox = ua.indexOf('Firefox') != -1;

    client.isNS7 = ua.indexOf('Netscape/7') != -1;
    client.isNS71 = ua.indexOf('Netscape/7.1') != -1;

    if (client.isOpera) {
        client.isMSIE = true;
        client.isGecko = false;
        client.isSafari = false;
    }

    client.isIE = client.isMSIE;
    client.isRealIE = client.isMSIE && !client.isOpera;
    client.execCommand = (typeof (document.execCommand) != 'undefined');

    client.isBrowserModelCompatible = client.isMSIE80 || client.isMSIE90 || client.isSafari || client.isOpera || client.isWebKit || client.isFireFox || client.isGoogleChrome;
    client.isBrowserVersionCompatible = true;

    if (client.isOpera) {
        var versionRegExp = new RegExp('(\\d+)\\.(\\d+)', 'gi');
        var match = versionRegExp.exec(window.opera.version());

        client.operaMajorVersion = match[1];
        client.operaMinorVersion = match[2];

        client.isBrowserVersionCompatible = match[1] >= 11;
    }

    if (client.isFireFox) {
        var reVersion = new RegExp('Firefox\\/(\\d+)*\\.', 'i');
        var match = reVersion.exec(ua);
        client.fireFoxMajorVersion = match[1];
        client.isBrowserVersionCompatible = client.fireFoxMajorVersion > 2;
    }

    client.isBrowserCompatible = client.isBrowserModelCompatible && client.isBrowserVersionCompatible;

})();


/* EVENTS */

(function () {
    var common = window.Common;

    if (!common.Events) {
        common.Events =
        {
            PageInit: createEventHandler(),
            PageParsed: createEventHandler(),
            PageLoaded: createEventHandler(),
            PageUnloaded: createEventHandler(),
            PageResize: createEventHandler(),
            PageOnBeforeUnload: createEventHandler()
        };
    }

    var events = common.Events;

    events.createEventHandler = createEventHandler;

    attachWindowEvents();

    function attachWindowEvents() {
        window.onbeforeunload = common.Events.PageOnBeforeUnload.raise;
        window.onload = common.Events.PageLoaded.raise;
        window.onunload = common.Events.PageUnloaded.raise;
        window.onresize = common.Events.PageResize.raise;
    }

    function checkWindowHeight() {
        var state = {};

        return function () {
            var newHeight = Common.getHeight(document);

            if (newHeight != state.prevHeight) {
                common.Events.PageResize.raise();
                state.prevHeight = newHeight;
            }
        }
    }

    events.PageParsed.attach(function () { setInterval(checkWindowHeight(), 100); });

    function createEventHandler() {
        var event = [];
        initializeEventHandler(event);

        return event;
    }

    function initializeEventHandler(event) {
        event.raise = (function (event) {
            return function () {
                var result = [];

                for (var i = 0; i < event.length; i++) {
                    if (typeof (event[i]) == "function") {
                        var r = event[i].apply(this, arguments);

                        if (typeof r != "undefined") {
                            result.push(r);
                        }
                    }
                }

                if (result.length > 1) {
                    return result;
                }

                return result[0];
            }
        })(event);

        event.raiseAsync = (function (event) {
            return function () {
                setTimeout(event.raise, 1);
            }
        })(event);

        event.attach = (function (event) {
            return function (handler, index) {
                if (typeof index != "undefined") {
                    event.splice(index, 0, handler);
                    return;
                }

                event.push(handler);
            }
        })(event);

        event.detach = (function (event) {
            return function (handler) {
                for (var i = 0; i < event.length; i++) {
                    if (event[i] == handler) {
                        delete event[i];
                        this.splice(i, 1);

                        return;
                    }
                }
            }
        })(event);
    }

    function addWindowEvent(event, action) {
        if (window.addEventListener) {
            window.addEventListener(event, action);
        }
        else if (window.attachEvent) {
            window.attachEvent('on' + event, action);
        }
    }

})();


/* CLIENT STORAGE */

(function () {

    var common = window.Common;

    if (!common.Storage) {
        common.Storage = {};
    }

    var storage = common.Storage;
    var defaultStore;

    storage.saveData = saveData;
    storage.getData = getData;
    storage.getDataAsync = getDataAsync;

    common.Events.PageParsed.attach(function () {

        defaultStore = new Persist.Store('Alpha Data Store');

    });

    function saveData(key, value) {
        defaultStore.set(key, common.Json.serialize(value));
    }

    function getData(key) {
        var result = null;

        function processGet(isSuccess, value) {
            result =
            {
                value: value,
                isSuccess: isSuccess
            }

            return true;
        }

        //small hack for wizard 2.0
        if (!defaultStore) {
            defaultStore = new Persist.Store('Alpha Data Store');
        }

        defaultStore.get(key, processGet);

        var startTime = (new Date()).getTime();

        while (!result) {
            if ((new Date()).getTime() - startTime > 2000) {
                return null;
            }
        }

        return result.isSuccess && result.value != null ? common.Json.deserialize(result.value) : null;
    }


    function getDataAsync(key, callBack) {
        function processGet(isSuccess, value) {
            callBack(isSuccess, isSuccess && value != null ? common.Json.deserialize(value) : null);
        }

        defaultStore.get(key, processGet);
    }

})();


/* BROWSER BEHAVIOR */

(function () {
    var common = window.Common;

    common.Events.PageParsed.attach(initializeBackSpaceHandling);

    function initializeBackSpaceHandling() {
        $(document).keydown(handleBackSpace);
    }

    function handleBackSpace(event) {
        switch (event.target.tagName.toLowerCase()) {
            case 'input':
            case 'textarea':
                // do nothing;
                break;

            default:
                if (event.keyCode == 8) {
                    // prevent navigating back
                    event.stopPropagation();
                    return false;
                }
        }
    }

})();



/* WIDGET */

(function () {

    var common = window.Common;

    if (!common.Widget) {
        common.Widget = {};
    }

    var widget = common.Widget;

    common.Events.PageParsed.attach(onPageParsed);

    function onPageParsed() {
        if (common.Config.isWidgetMode && !common.ClientInfo.isMSIE70) {
            initializeWidgetMode();
        }
    }

    function initializeWidgetMode() {
        document.body.style.overflowY = 'hidden';
        common.Events.PageResize.attach(onResize());

        var urlBuilder = common.UrlBuilder.create(window.location);
        var pageUrl = urlBuilder.getQueryParamValue('page');

        if (pageUrl) {
            common.Storage.saveData('widget.parentUrl', decodeURIComponent(pageUrl));
        }
    }

    function onResize() {
        var state = {}

        function notify(newHeight, parentUrl) {
            state.prevHeight = newHeight;

            if (parentUrl) {
                parent.location.replace(parentUrl + '#dwh=' + newHeight);
            }
        }

        return function () {
            var maxHeight = Common.getHeight(document);
            var actualHeight = document.body.offsetHeight;

            var newHeight = maxHeight - actualHeight > 1000 ? actualHeight : maxHeight;

            if (newHeight == state.prevHeight) {
                return;
            }

            if (state.parentUrl) {
                notify(newHeight, state.parentUrl);
            }

            common.Storage.getDataAsync(
                'widget.parentUrl',
                function (isSuccess, value) {
                    if (isSuccess) {
                        state.parentUrl = value;
                        notify(newHeight, state.parentUrl);
                    }
                }
            );
        }
    }

})();



/* VALIDATION */

(function () {
    var common = window.Common;

    var russianSymbolsWithoutSpaces = 'а-яА-ЯёЁ.,;:!?#№%*\\\\/0-9_+\'\"()\\-«»@';

    if (!common.Validation) {
        common.Validation =
        {
            elements: [],
            valueProcessors:
            {
                trimOuterSpaces: function (value) {
                    if (!value || !value.length) {
                        return value;
                    }

                    return value.customTrim();
                },
                trimInnerSpaces: function (value) {
                    if (!value || !value.length) {
                        return value;
                    }

                    var filters =
                    [
                        {
                            regExp: new RegExp('\\s+', 'g'),
                            template: ' '
                        },
                        {
                            regExp: new RegExp("[\\'\"]+", 'g'),
                            template: '"'
                        },
                        {
                            regExp: /\s*(\s)"([^"$]+)"([^"$]+)"+(?:(\s)\s*|$)/img,
                            template: '$1«$2«$3»$4'
                        },
                        {
                            regExp: /(\s)"([^"]+)"(?:(\s)|$)/img,
                            template: '$1«$2»$3'
                        },
                        {
                            regExp: new RegExp('«\\s+', 'g'),
                            template: '«'
                        },
                        {
                            regExp: new RegExp('\\s+»', 'g'),
                            template: '»'
                        }
                    ];

                    for (var i = 0; i < filters.length; i++) {
                        var filter = filters[i];
                        value = value.replace(filter.regExp, filter.template);
                    }

                    return value;
                },
                upperCaseFirst: function (value) {
                    if (!value || !value.length) {
                        return value;
                    }

                    value = value.customTrim();

                    return value.substr(0, 1).toUpperCase() + value.substr(1);
                },
                integerNumber: function (value) {
                    if (!value || !value.length) {
                        return null;
                    }

                    var num = common.repairNumber(value).toString();
                    var numStr = num.toString();

                    if (!numStr) {
                        return null;
                    }

                    return num >= 0 ? Math.floor(num) : Math.ceil(num);
                },
                decimalNumber: function (value, precision) {
                    if (!value || !value.length) {
                        return null;
                    }

                    if (!precision) {
                        precision = 3;
                    }
                    else {
                        precision++;
                    }

                    var result = common.repairNumber(value).toString();

                    if (!result) {
                        return null;
                    }

                    var dotIndex = result.indexOf(".");
                    var length = result.length;

                    if (result <= 0) {
                        result = 0;
                    }
                    else if (dotIndex > 0 && dotIndex + precision < length) {
                        result = result.substr(0, dotIndex + precision);
                    }

                    //return result.toString();
                    return result;
                },
                positiveNumber: function (value) {
                    if (value < 0) {
                        return value * -1;
                    }

                    return value;
                },
                regExReplacer: common.formatRegEx
            },
            rules:
            {
                emptyOrNull: function (value) {
                    return typeof value == 'undefined' || value === undefined || value === null || value == NaN || value === '';
                },
                notEmptyStringOrNull: function (value) {
                    if (typeof value == 'undefined' || value === null) {
                        return false;
                    }

                    return value.toString().length > 0;
                },
                notEmpty: function (value, element) {
                    value = (value || '').toString().customTrim();

                    return value.length > 0;
                },
                exactLength: function (length) {
                    return function (value, element) {
                        value = (value || '').toString().customTrim();

                        return value.length == length || value.length == 0;
                    }
                },
                exactValue: function (targetValue) {
                    return function (value, element) {
                        value = (value || '').toString().customTrim();

                        if (value.length == 0) {
                            return true;
                        }

                        return value == targetValue;
                    }
                },
                russianSentence: function (value, element) {
                    value = (value || '').toString().customTrim();

                    var pattern = '^[' + russianSymbolsWithoutSpaces + '\\s]+$';
                    var re = new RegExp(pattern, 'gi');

                    return value.length == 0 || re.test(value);
                },
                russianWord: function (value, element) {
                    value = (value || '').toString().customTrim();
                    var re = new RegExp('^[а-яё]+(\\-[а-яё]+)*$', 'gi');

                    return value.length == 0 || re.test(value);
                },
                russianLastName: function (value, element) {
                    value = (value || '').toString().customTrim();
                    var re = new RegExp('^[а-яё]+(-[а-яё]+)*$', 'gi');

                    return value.length == 0 || re.test(value);
                },
                russianMiddleName: function (value) {
                    value = (value || '').toString().customTrim();
                    var re = new RegExp('^[а-яё]+((-|\\s+)[а-яё]+)*$', 'gi');

                    return value.length == 0 || re.test(value);
                },
                legalPersonRegistrationNumber: function (value, element) {
                    value = (value || '').toString().customTrim();

                    if (value.length != 13) {
                        return true;
                    }

                    var d = common.stringToIntArray(value);
                    var checkSum = value.substr(0, 12) % 11;
                    var checkSumString = (checkSum || '').toString();

                    if (checkSumString.length == 2) {
                        return checkSumString.substring(1, 1) == d[12];
                    }

                    return checkSumString == d[12];
                },
                legalPersonRegistrationNumberFirstDigitValidator: function (value, element) {
                    value = (value || '').toString().customTrim();

                    if (value.length != 13) {
                        return true;
                    }

                    var d = common.stringToIntArray(value);

                    return d[0] == 1 || d[0] == 2 || d[0] == 3 || d[0] == 5;
                },
                legalPersonRegistrationNumberRegionCodeValidator: function (value, element, isFieldValid) {
                    value = (value || '').toString().customTrim();

                    if (value.length != 13) {
                        return true;
                    }

                    var regionCode = value.substr(3, 2);
                    var addressFieldId = Common.DatabaseService.fieldsDefinition.legalPerson.address.fullAddress;

                    return isFieldValid(addressFieldId, [Common.Validation.rules.regionCodeValidator(regionCode)]).isValid;
                },
                legalPersonRegistrationNumberYearValidator: function (value, element, isFieldValid) {
                    value = (value || '').toString().customTrim();

                    if (value.length != 13) {
                        return true;
                    }

                    var registrationYear = value.substr(1, 2);
                    var registrationDateFieldId = Common.DatabaseService.fieldsDefinition.legalPerson.registrationDate;

                    return isFieldValid(registrationDateFieldId, [Common.Validation.rules.exactYear(registrationYear)]).isValid;
                },
                exactYear: function (targetYear) {
                    return function (value, element) {
                        if (!value) {
                            return true;
                        }

                        var fullYear = value.getFullYear();
                        var shortYear = fullYear.toString().substr(2, 2);

                        return shortYear == targetYear;
                    }
                },
                legalPersonTaxpayerNumber: function (value, element) {
                    value = (value || '').toString().customTrim();

                    if (value.length != 10) {
                        return true;
                    }

                    var d = common.stringToIntArray(value);

                    var checkSumm;
                    checkSumm = 2 * d[0] + 4 * d[1] + 10 * d[2] + 3 * d[3] + 5 * d[4] + 9 * d[5] + 4 * d[6] + 6 * d[7] + 8 * d[8];
                    checkSumm = checkSumm % 11;
                    checkSumm = checkSumm % 10;

                    return checkSumm == d[9];
                },
                naturalPersonTaxpayerNumber: function (value, element, isFieldValid) {
                    value = (value || '').toString().customTrim();

                    if (value.length != 12) {
                        return true;
                    }

                    var d = common.stringToIntArray(value);

                    var checkSumm;
                    checkSumm = 7 * d[0] + 2 * d[1] + 4 * d[2] + 10 * d[3] + 3 * d[4] + 5 * d[5] + 9 * d[6] + 4 * d[7] + 6 * d[8] + 8 * d[9];
                    checkSumm = checkSumm % 11;
                    checkSumm = checkSumm % 10;

                    var d11ok = checkSumm == d[10];

                    checkSumm = 3 * d[0] + 7 * d[1] + 2 * d[2] + 4 * d[3] + 10 * d[4] + 3 * d[5] + 5 * d[6] + 9 * d[7] + 4 * d[8] + 6 * d[9] + 8 * d[10];
                    checkSumm = checkSumm % 11;
                    checkSumm = checkSumm % 10;

                    var d12ok = checkSumm == d[11];

                    return d11ok && d12ok;
                },
                personTaxInfoRegionCodeValidator: function (addressFieldId) {
                    return function (value, element, isFieldValid) {
                        value = (value || '').toString().customTrim();

                        if (value == '') {
                            return true;
                        }

                        var regionCode = value.substr(0, 2);

                        return isFieldValid(addressFieldId, [Common.Validation.rules.regionCodeValidator(regionCode)]).isValid;
                    }
                },
                contains: function (verbs) {
                    var verbs = Common.getType(verbs) != 'array' ? [verbs] : verbs;

                    for (var i = 1; i < arguments.length; i++) {
                        verbs.push(arguments[i]);
                    }

                    return function (value, element) {
                        switch (common.getType(value)) {
                            case 'string':

                                value = (value || '').customTrim();

                                if (value.length == 0) {
                                    return true;
                                }

                                for (var i = 0; i < verbs.length; i++) {
                                    var verb = verbs[i];
                                    var regexPattern = new RegExp("(^|\\s)" + verb + "($|\\s)", "gi");

                                    if (!regexPattern.test(value)) {
                                        return false;
                                    }
                                }

                                return true;

                            default:
                                throw 'invalid field type';
                        }
                    }
                },
                regionCodeValidator: function (targetRegionCode) {
                    return function (value, element) {
                        if ((value || '').length == 0) {
                            return true;
                        }

                        var regionCodeFromAddress = Common.DatabaseService.getRegionCodeByAddress(value);

                        return regionCodeFromAddress == targetRegionCode;
                    }
                },
                containsWhenSet: function (fieldId, targetValue, verbs) {
                    return function (value, element, isFieldValid) {
                        if ((value || '').length == 0) {
                            return true;
                        }

                        if (isFieldValid(fieldId, [Common.Validation.rules.exactValue(targetValue)]).isValid) {
                            var containsValidationRule = Common.Validation.rules.contains(verbs);
                            return containsValidationRule(value, element, isFieldValid);
                        }

                        return true;
                    }
                },
                checkRuleWhenSet: function (fieldId, targetValue, rule) {
                    return function (value, element, isFieldValid) {
                        if ((value || '').length == 0) {
                            return true;
                        }

                        if (isFieldValid(fieldId, [Common.Validation.rules.exactValue(targetValue)]).isValid) {
                            return rule(value, element, isFieldValid);
                        }

                        return true;
                    }
                },
                notEquals: function (verbs) {
                    var verbs = Common.getType(verbs) != 'array' ? [verbs] : verbs;

                    return function (value, element) {
                        switch (common.getType(value)) {
                            case 'string':

                                value = (value || '');

                                if (value.length == 0) {
                                    return true;
                                }

                                for (var i = 0; i < verbs.length; i++) {
                                    var verb = verbs[i];

                                    if (value.toLowerCase() == verb.toLowerCase()) {
                                        return false;
                                    }
                                }

                                return true;

                            default:
                                throw 'invalid field type';
                        }
                    }
                },
                requires: function (fieldId) {
                    return function (value, element, isFieldValid) {
                        if ((value || '') == '') {
                            return true;
                        }

                        return isFieldValid(fieldId, [Common.Validation.rules.notEmpty]).isValid;
                    }
                },
                sameAs: function (fieldId) {
                    return function (value, element, isFieldValid) {
                        if ((value || '') == '') {
                            return true;
                        }

                        return isFieldValid(fieldId, [Common.Validation.rules.exactValue(value)]).isValid;
                    }
                },
                notEmptyWhenExists: function (fieldId) {
                    return function (value, element, isFieldValid) {
                        if ((value || '').length > 0) {
                            return true;
                        }

                        return !isFieldValid(fieldId, [Common.Validation.rules.notEmpty]).isValid;
                    }
                },
                varLength: function (minLength, maxLength) {
                    return function (value, element) {
                        value = value.toString();
                        var length = (value || '').customTrim().length;

                        return length == 0 || (length >= minLength && length <= maxLength);
                    }
                },
                minLength: function (minLength) {
                    return function (value, element) {
                        value = value.toString();
                        var length = (value || '').customTrim().length;

                        return length == 0 || (length >= minLength);
                    }
                },
                email: function (value, element) {
                    return (value || '').match(/^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/gi) ? true : false;
                },
                numeric: function (value, element) {
                    value = value.toString().customTrim();

                    if ((value || '').length == 0) {
                        return true;
                    }

                    return value.match(/^\d+([\.,]\d+)?$/gi) ? true : false;
                },
                regularExpression: function (pattern, properties) {
                    return function (value, element) {
                        value = value.toString();

                        if ((value || '').length == 0) {
                            return true;
                        }

                        var re = new RegExp(pattern, properties || 'i');
                        return re.test(value);
                    }
                },
                legalPersonFullName: function (requiredVerb, fullNameVerb) {
                    return function (value) {
                        value = (value || '').toString();

                        if (value.length == 0) {
                            return true;
                        }

                        if (!common.Validation.rules.russianSentence(value)) {
                            return false;
                        }

                        var shortOPF = new RegExp(requiredVerb, 'i');

                        if (!shortOPF.test(value)) {
                            return false;
                        }

                        var fullOPF = new RegExp('^' + fullNameVerb, 'i');

                        if (fullOPF.test(value)) {
                            return common.Validation.rules.legalPersonRussianNameWithQoutes(value);
                        }
                        else {
                            return true;
                        }
                    }
                },
                legalPersonRussianNameWithQoutes: function (value, isNameStartWithWhiteSpace) {
                    value = (value || '').toString();

                    if (value.length == 0) {
                        return true;
                    }

                    var russianNameRegExp = new RegExp('\\s*[«"](.+)["»]\\s*$', 'g');
                    var russianNameRegExpResult = russianNameRegExp.exec(value);

                    if (!russianNameRegExpResult || !russianNameRegExpResult[1]) {
                        return false;
                    }

                    var symbolsRegexPattern = '^[' + russianSymbolsWithoutSpaces + '\\s]+$';
                    var symbolsRegEx = new RegExp(symbolsRegexPattern);

                    return symbolsRegEx.test(russianNameRegExpResult[1]);
                },
                legalPersonShortName: function (requiredVerb) {
                    return function (value) {
                        value = value.toString();

                        if ((value || '').length == 0) {
                            return true;
                        }

                        if (!common.Validation.rules.russianSentence(value)) {
                            return false;
                        }

                        var shortOPF = new RegExp(requiredVerb);

                        if (!shortOPF.test(value)) {
                            return false;
                        }

                        var prefixForm = new RegExp('^' + requiredVerb + '\\s*');
                        var postfixForm = new RegExp('^[^\\s]+\\s*' + requiredVerb + '$');

                        return (prefixForm.test(value) && common.Validation.rules.legalPersonRussianNameWithQoutes(value)) ||
                                postfixForm.test(value);
                    }
                },
                dateRange: function (lowestDate, highestDate) {
                    return function (value) {
                        if (!highestDate) {
                            highestDate = new Date();
                        }

                        if (!lowestDate) {
                            lowestDate = new Date();
                        }

                        var registrationDate = new Date(value);
                        return registrationDate <= highestDate && registrationDate >= lowestDate;
                    }

                },
                entityValidator: function (dataSourceProvider) {
                    return function (value) {
                        if (!value) {
                            return true;
                        }

                        var dataSource = dataSourceProvider();
                        var entity = dataSource.customFindFirst(function (item) { return item.id == value; });

                        if (!entity) {
                            return true;
                        }

                        validator = Forms.Validator.create({
                            fieldSets: entity.info
                        });

                        return validator.validate().isValid;
                    }
                }
            }
        };

        common.Validation.ruleGroups =
            {
                legalFullNameValidators:
                [
                    {
                        message: 'Поле должно содержать организационно-правовую форму и наименование в кавычках на русском языке.\n \
                                      Например: Общество с ограниченной ответственностью «Ромашка»',
                        messageBoxOnly: false,
                        rule: Common.Validation.rules.legalPersonFullName('с ограниченной ответственностью', 'Общество с ограниченной ответственностью')
                    }
                ],
                legalShortNameValidators:
                [
                    {
                        message: 'Поле должно содержать аббревиатуру ООО и сокращенное наименование в кавычках на русском языке.\n \
                                      Например: ООО «Ромашка» ',
                        messageBoxOnly: false,
                        rule: Common.Validation.rules.legalPersonShortName('ООО')
                    }
                ]
            }
    }

    var validation = common.Validation;

    common.Events.PageParsed.attach(initializeValidation);

    validation.validate = function (parameters) {
        var result =
        {
            success: true,
            invalidControls: []
        }

        var elementsToValidate = parameters.elements || validation.elements;

        for (var i = 0; i < elementsToValidate.length; i++) {
            var elementToValidate = elementsToValidate[i];
            var validationCustomHandlerAttribute = elementToValidate.attributes["validationCustomHandler"];
            var isElementValid = validateElement(elementToValidate);

            result.success = result.success && isElementValid;

            if (!isElementValid) {
                result.invalidControls.push(elementToValidate);

                if (result.invalidControls.length == 1) {
                    try {
                        elementToValidate.focus();
                    }
                    catch (e) { }
                }
            }

            if (validationCustomHandlerAttribute) {
                var validationCustomHandler = eval(validationCustomHandlerAttribute.value);

                validationCustomHandler({
                    isValid: isElementValid,
                    element: elementToValidate,
                    errorMessage: getErrorMessage(elementToValidate)
                });
            }
        }

        if (!result.success && parameters.showSummary) {
            showValidationSummary(result.invalidControls);
        }

        return result;
    }

    validation.createResult = function () {
        function ValidationResult(isValid, messages) {
            var pThis = this;
            pThis.isValid = isValid == undefined ? true : isValid;

            if (pThis.isValid) {
                pThis.messages = [];
            }
            else if (common.getType(messages) == 'string') {
                pThis.messages = [messages];
            }
            else {
                pThis.messages = messages || [];
            }

            pThis.combine = function (validationResult) {
                pThis.isValid = pThis.isValid && validationResult.isValid;

                if (validationResult.messages && !pThis.isValid) {
                    pThis.messages = pThis.messages.concat(validationResult.messages)
                }

                return pThis;
            }

            pThis.clearMessages = function () {
                pThis.messages = [];
                return pThis;
            }
        }

        if (arguments.length == 0) {
            return new ValidationResult();
        }

        if (common.getType(arguments[0]) == 'object') {
            var result = new ValidationResult(true);

            for (var i = 0; i < arguments.length; i++) {
                result.combine(arguments[i]);
            }

            return result;
        }

        return new ValidationResult(arguments[0], arguments[1]);
    }

    function showValidationSummary(invalidControls) {
        var result = "";

        for (var i = 0; i < invalidControls.length; i++) {
            var invalidControl = invalidControls[i];
            var errorMessage = getErrorMessage(invalidControl);

            if (!errorMessage) {
                continue;
            }

            result += common.formatString("- {0}\n", errorMessage);
        }

        if (result) {
            alert(result);
        }
    }

    function getErrorMessage(element) {
        var errorMessageAttribute = element.attributes["errorMessage"];

        if (!errorMessageAttribute) {
            return '';
        }

        return errorMessageAttribute.value;
    }

    function validateElement(element) {
        var validationRuleNameAttribute = element.attributes["validationRule"];

        if (!validationRuleNameAttribute) {
            throw "Element is not supposed to be validated: " + element.tagName + "." + element.id;
        }

        var validationRuleNames = validationRuleNameAttribute.value.customTrim().split(/;\s*/);
        var isValid = true;

        for (var i = 0; i < validationRuleNames.length; i++) {
            var validationRuleParser = new RegExp("([^(]+)(\\(([^)]+)\\))?", "g");
            var validationRuleMatch = validationRuleParser.exec(validationRuleNames[i]);

            if (!validationRuleMatch) {
                continue;
            }

            var name = validationRuleMatch[1];

            var params = null;

            if (validationRuleMatch.length == 4 && validationRuleMatch[3]) {
                params = validationRuleMatch[3].split(/,\s*/);
            }

            var validationRule = getValidationRuleByName(name, params);
            isValid = isValid && validationRule(element.value, element);
        }

        return isValid;
    }

    function getValidationRuleByName(name, params) {
        for (var i in common.Validation.rules) {
            if (i.toString().toLowerCase() == name.toString().toLowerCase()) {
                if (params) {
                    return common.Validation.rules[i].apply(this, params);
                }
                else {
                    return common.Validation.rules[i];
                }
            }
        }

        throw "validation rule " + name + " not found";
    }

    function initializeValidation(containerId) {
        controlSelector(containerId).each(function () { validation.elements.push(this); });
    }

    function removeValidation(containerId) {
        controlSelector(containerId).each(function () { validation.elements.customRemoveFirst(this); });
    }

    function controlSelector(containerId) {
        return $('#' + containerId).find("INPUT[validationRule], TEXTAREA[validationRule], DIV[validationRule]");
    }

})();


/* POPUP BOX */


(function () {
    var common = window.Common;

    if (!common.PopupBox) {
        common.PopupBox = {};
    }

    common.PopupBox.create = function (parameters) {
        return new PopupBox(parameters);
    }

    common.PopupBox.createConfirmPopup = function (parameters) {
        var applyHandlerOrClosePopup = function (handler) {
            return function () {
                if (handler) {
                    handler.apply(this);
                }
                else {
                    this.close();
                }
            }
        };

        var popupParams = {
            buttons:
            [
                {
                    title: 'ОК',
                    isDefault: parameters.cancelIsDefault == undefined ? true : !parameters.cancelIsDefault,
                    onclick: applyHandlerOrClosePopup(parameters.okButtonHandler)
                },
                {
                    title: 'Отмена',
                    isDefault: parameters.cancelIsDefault,
                    onclick: applyHandlerOrClosePopup(parameters.cancelButtonHandler)
                }
            ]
        };

        jQuery.extend(popupParams, parameters);

        return common.PopupBox.create(popupParams);
    }

    common.PopupBox.WindowStatus =
    {
        closed: 1,
        visible: 2
    }

    function PopupBox(parameters) {
        var pThis = this;
        var defaultParameters =
        {
            title: '',
            width: 500,
            style: 'grey',
            scrollWindow: true,
            topOffset: 50,
            focusOnDefaultButton: true
        }
        var windowStatus = common.PopupBox.WindowStatus.closed;

        pThis.parameters = jQuery.extend(defaultParameters, parameters);

        pThis.Events =
        {
            beforeClose: Common.Events.createEventHandler(),
            close: Common.Events.createEventHandler(),
            show: Common.Events.createEventHandler()
        }

        pThis.show = show;
        pThis.close = close;
        pThis.dispose = dispose;

        initialize();

        function initialize() {
            pThis.guid = common.formatString("popup{0}", common.newGuid());

            pThis.boxElement = document.createElement("DIV");
            pThis.boxElement.id = getPopupBoxContainerId();
            pThis.boxElement.className = "popup";
            pThis.boxElement.innerHTML = renderPopupBoxContent();

            document.body.appendChild(pThis.boxElement);

            pThis.Controls =
            {
                closeButton: common.$(getCloseButtonId()),
                container: common.$(getPopupBoxContainerId()),
                shade: common.$(getPopupBoxShadeId()),
                box: common.$(getPopupBoxId()),
                windowArea: common.$(getWindowAreaId()),
                buttons: common.$(getButtonsId()),
                header: common.$(getHeaderId()),
                title: common.$(getTitleId()),
                content: common.$(pThis.parameters.contentId),
                contentWrapper: common.$(getWrapperId())
            }

            if (pThis.Controls.content) {
                pThis.Controls.windowArea.appendChild(pThis.Controls.content);
            }

            renderButtons();
            attachButtonEvents();

            if (pThis.Controls.closeButton) {
                $(pThis.Controls.closeButton).bind("click", close);
            }

            $(pThis.Controls.box).hide();
        }

        function dispose() {
            document.body.removeChild(pThis.boxElement);
        }

        function show() {
            if (windowStatus == common.PopupBox.WindowStatus.visible) {
                return;
            }

            $(pThis.Controls.box).show();
            $(pThis.Controls.shade).animate({ opacity: 0.0 }, 0).show().animate({ opacity: 0.8 }, 700);

            var verticalScroll = (pThis.Controls.contentWrapper.clientHeight > pThis.Controls.windowArea.clientHeight);

            $(pThis.Controls.windowArea).css({
                width: pThis.parameters.width > 0 ? (pThis.parameters.width + "px") : "auto",
                height: pThis.parameters.height > 0 ? (pThis.parameters.height + "px") : "auto",
                overflowY: verticalScroll ? 'scroll' : 'auto'
            });

            var newLeft = document.body.offsetWidth / 2 - pThis.Controls.box.offsetWidth / 2;
            var newTop = getPopupTopPositon();
            var marginLeft = -pThis.Controls.box.offsetWidth / 2;

            $(pThis.Controls.box).css({
                left: "50%",
                "margin-left": marginLeft + "px",
                top: (newTop > 0 ? newTop : 0) + "px"
            });

            var oldWindowScrollPosition = Common.getWindowScrollTop();

            focusOnWindowOrDefaultButton();

            windowStatus = common.PopupBox.WindowStatus.visible;
            pThis.Events.show.raise();

            window.scroll(0, oldWindowScrollPosition);
        }

        function close(params) {
            var parameters =
            {
                leaveShade: false
            }

            $.extend(parameters, params);

            if (windowStatus == common.PopupBox.WindowStatus.closed) {
                return;
            }

            var eventArgs =
            {
                cancel: false,
                leaveShade: false
            };

            pThis.Events.beforeClose.raise(eventArgs);

            if (eventArgs.cancel) {
                return;
            }

            $(pThis.Controls.box).hide();

            if (!parameters.leaveShade && !eventArgs.leaveShade) {
                $(pThis.Controls.shade).animate({ opacity: "hide" }, 300);
            }

            windowStatus = common.PopupBox.WindowStatus.closed;
            pThis.Events.close.raise();
        }

        function renderPopupBoxContent() {
            return ([
                '<div id="', getPopupBoxShadeId(), '" class="shade"></div> \
                <table id="', getPopupBoxId(), '"class="border ', pThis.parameters.style, '"> \
                    <thead> \
                        <tr> \
                            <td class="left-border"> \
                                &nbsp; \
                            </td> \
                            <td class="center-border"> \
                                &nbsp; \
                            </td> \
                            <td class="right-border"> \
                                &nbsp; \
                            </td> \
                        </tr> \
                    </thead> \
                    <tbody> \
                        <tr> \
                            <td class="left-border"> \
                                &nbsp; \
                            </td> \
                            <td class="center-border content"> \
                                <div id="', getHeaderId(), '" class="columnControl header"> \
                                    <div id="', getTitleId(), '" class="column title">',
                                        common.encodeHtml(pThis.parameters.title),
                                    '</div>',
                                    parameters.withoutCloseButton ? '' : renderCloseButton(),
                                '</div> \
                                <div id="', getWindowAreaId(), '" class="window">',
                                    '<div id="', getWrapperId(), '">',
                                        pThis.parameters.content,
                                    '</div>',
                                '</div> \
                                <div id="', getButtonsId(), '" class="buttons">',
                                '</div> \
                            </td> \
                            <td class="right-border"> \
                                &nbsp; \
                            </td> \
                        </tr> \
                    </tbody> \
                    <tfoot> \
                        <tr> \
                            <td class="left-border"> \
                                &nbsp; \
                            </td> \
                            <td class="center-border"> \
                                &nbsp; \
                            </td> \
                            <td class="right-border"> \
                                &nbsp; \
                            </td> \
                        </tr> \
                    </tfoot> \
                </table>'
            ]).join('');
        }

        function renderCloseButton() {
            return ([
                '<div class="rightColumn controls"> \
                    <img id="', getCloseButtonId(), '" src="/Resources/Layout/Images/PopupBox/close.png" /> \
                </div>'
            ]).join('');
        }

        function focusOnWindowOrDefaultButton() {
            var defaultButton = pThis.parameters.buttons ? pThis.parameters.buttons.customFindFirst(function (b) { return b.isDefault; }) : null;

            if (defaultButton && pThis.parameters.focusOnDefaultButton) {
                common.$(defaultButton.id).focus();
            }
            else {
                document.body.focus();
            }
        }

        function renderButtons() {
            var buttonsHtml = [];

            if (!pThis.parameters.buttons || pThis.parameters.buttons.length == 0) {
                return;
            }

            for (var i = 0; i < pThis.parameters.buttons.length; i++) {
                var button = pThis.parameters.buttons[i];
                buttonsHtml.push(renderButton(button));
            }

            pThis.Controls.buttons.innerHTML = buttonsHtml.join('');
        }

        function attachButtonEvents() {
            if (!pThis.parameters.buttons || pThis.parameters.buttons.length == 0) {
                return;
            }

            var focusedToButton = false;

            for (var i = 0; i < pThis.parameters.buttons.length; i++) {
                var button = pThis.parameters.buttons[i];

                var jButton = $("#" + button.id);
                jButton.bind("click", getButtonClickHandler(button));
            }
        }

        function getButtonClickHandler(button) {
            return function () {
                button.onclick.apply(pThis);
            }
        }


        function renderButton(button) {
            if (!button.id) {
                button.id = createButtonId();
            }

            return ([
                '<button id="', button.id, '">',
                    common.encodeHtml(button.title),
                '</button>'
            ]).join('');
        }

        function getPopupTopPositon() {
            return pThis.parameters.topOffset + Common.getWindowScrollTop();
        }

        function getPopupBoxId() {
            return pThis.guid + 'Box';
        }

        function getPopupBoxContainerId() {
            return pThis.guid + 'BoxContainer';
        }

        function getPopupBoxShadeId() {
            return pThis.guid + 'Shade';
        }

        function getButtonsId() {
            return pThis.guid + 'Buttons';
        }

        function createButtonId() {
            return common.formatString("button{0}", common.newGuid())
        }

        function getCloseButtonId() {
            return pThis.guid + 'Close';
        }

        function getWindowAreaId() {
            return pThis.guid + 'Window';
        }

        function getHeaderId() {
            return pThis.guid + 'Header';
        }

        function getTitleId() {
            return pThis.guid + 'Title';
        }

        function getWrapperId() {
            return pThis.guid + 'Wrapper';
        }
    }

})();


/* EDITABLE SELECT */

(function () {

    var common = window.Common;

    if (!common.EditableSelect) {
        common.EditableSelect = {};
    }

    common.EditableSelect.create = function (parameters) {
        return new EditableSelect(parameters);
    }

    function EditableSelect(parameters) {
        var pThis = this;
        var defaultParameters =
        {
            allowNull: true,
            allowManual: true,
            showTextBoxWhenEmpty: false,
            selectedOptionReturnsText: true,
            ignoreSecondaryEvents: true,
            optionsDisplayLimit: 100,
            itemComparer: itemComparer,
            textBoxBlurFireOnChangeEvent: false
        };

        var onChangeHandler = onChangeHandlerFactory();
        var fireChangeEventOnEmptyTextBoxBlur = false;

        var viewMode =
        {
            select: 0,
            textBox: 1
        };

        var blockChangeEvent = false;

        pThis.parameters = jQuery.extend(defaultParameters, parameters);

        pThis.textBox = prepareControl(pThis.parameters.textBox);
        pThis.select = prepareControl(pThis.parameters.select);
        pThis.Events =
        {
            change: Common.Events.createEventHandler()
        }

        pThis.listItems = [];

        attachEvents();

        pThis.optionSelector = function (listOption) {
            return ({
                value: listOption[0],
                text: listOption[1]
            });
        }

        pThis.setListOptions = function (listOptions, listOptionsCount) {
            blockChangeEvent = true;

            listOptions = listOptions || [];
            pThis.parameters.optionsDisplayLimitExceeded = pThis.parameters.optionsDisplayLimit && listOptionsCount > pThis.parameters.optionsDisplayLimit;

            if (pThis.parameters.optionsDisplayLimitExceeded) {
                listOptions = [];
            }

            while (pThis.select.options.length > 0) {
                pThis.select.options[0] = null;
            }

            if (pThis.parameters.allowNull) {
                appendOption(pThis.select, 0, '');
            }

            if (pThis.parameters.allowManual) {
                appendOption(pThis.select, -1, 'Ввести вручную');
            }

            listOptions.customForEach(function (opt) {

                var option = pThis.optionSelector(opt);
                appendOption(pThis.select, option.value, option.text, false);

            });

            blockChangeEvent = false;
        }

        pThis.isCustomValue = function () {
            if (pThis.textBox) {
                var textBoxValue = getTextBoxValue();

                if (textBoxValue.length > 0) {
                    return true;
                }
            }

            return false;
        }

        pThis.initialize = function (listOptions, value, listOptionsCount) {
            blockChangeEvent = true;

            pThis.setListOptions(listOptions, listOptionsCount);

            if (value != undefined) {
                pThis.setValue(value);
            }

            var hasListOptions = !!listOptions && listOptions.length > 0;

            if ((pThis.parameters.showTextBoxWhenEmpty && !hasListOptions) || pThis.parameters.optionsDisplayLimitExceeded) {
                switchControl(viewMode.textBox, false);
                blockChangeEvent = false;
                return;
            }

            blockChangeEvent = false;
        }

        pThis.setValue = function (value) {
            blockChangeEvent = false;

            var option = findOption(value)

            if (option) {
                option.selected = true;
                onChangeHandler.setOldValue(pThis.getValue());

                if (pThis.textBox) {
                    pThis.textBox.value = '';
                }

                switchControl(viewMode.select, false);

                return;
            }

            if (pThis.textBox) {
                switchControl(viewMode.textBox, false);
                onChangeHandler.setOldValue(value);
                pThis.textBox.value = value;
            }
        }

        pThis.getSelectOptionValue = function () {
            var selectedOption = getSelectedOption();

            if (!selectedOption) {
                return null;
            }

            if (selectedOption.value == -1 && pThis.parameters.allowManual) {
                return null;
            }

            return selectedOption.value;
        }

        pThis.getTextValue = function () {
            return getValue(true);
        }

        pThis.getValue = function () {
            return getValue(pThis.parameters.selectedOptionReturnsText);
        }

        pThis.setEnabled = function (enabled) {

        }

        pThis.reset = function (enabled) {
            if (pThis.textBox) {
                pThis.textBox.value = '';
            }

            pThis.setListOptions([]);
        }

        pThis.isSelectMode = function () {
            return getActiveMode() == viewMode.select;
        }

        function attachEvents() {
            if (pThis.textBox) {
                pThis.textBox.$.blur(textBoxBlur);
                pThis.textBox.$.change(textBoxChange);
            }

            pThis.select.$.change(selectChange);
        }

        function getValue(getAsText) {
            if (pThis.textBox) {
                var textBoxValue = getTextBoxValue();

                if (textBoxValue.length > 0) {
                    return textBoxValue;
                }
            }

            var selectedOption = getSelectedOption();

            if (!selectedOption) {
                return '';
            }

            if (selectedOption.value == -1 && pThis.parameters.allowManual) {
                return '';
            }

            if (getAsText) {
                return selectedOption.text;
            }

            return selectedOption.value;
        }

        function textBoxChange() {
            blockChangeEvent = true;
            pThis.select.selectedIndex = 0;
            blockChangeEvent = false;
            fireChangeEventOnEmptyTextBoxBlur = pThis.textBox.value.customTrim().length == 0;
            onChangeHandler();
        }

        function textBoxBlur() {
            var textBoxValue = getTextBoxValue();

            if (textBoxValue.length == 0) {
                if (!pThis.parameters.optionsDisplayLimitExceeded) {
                    switchControl(viewMode.select, false);
                    onChangeHandler();
                }

                return;
            }

            var option = findOption(textBoxValue)

            if (option) {
                blockChangeEvent = true;
                switchControl(viewMode.select, false);
                option.selected = true;
                blockChangeEvent = false;
            }

            if (!option || pThis.parameters.textBoxBlurFireOnChangeEvent) {
                onChangeHandler();
            }
        }

        function selectChange() {
            if (pThis.textBox) {
                var selectedOption = getSelectedOption();

                if (selectedOption && selectedOption.value == -1 && pThis.parameters.allowManual) {
                    blockChangeEvent = true;
                    switchControl(viewMode.textBox, true);
                    blockChangeEvent = false;

                    return;
                }
            }

            onChangeHandler();
        }

        function getActiveMode() {
            if (pThis.textBox && pThis.textBox.style.display == 'none') {
                return viewMode.select;
            }

            if (pThis.select.style.display == 'none') {
                return viewMode.textBox;
            }

            return null;
        }

        function switchControl(newMode, setFocus) {
            var activeMode = getActiveMode();

            if (newMode == viewMode.textBox && activeMode == viewMode.select) {
                pThis.textBox.style.display = 'block';
                pThis.textBox.value = '';
                pThis.select.style.display = 'none';
                pThis.select.selectedIndex = 0;

                if (setFocus) {
                    setTimeout(function () { pThis.textBox.focus(); }, 100);
                }

            }
            else if (newMode == viewMode.select && activeMode == viewMode.textBox) {
                pThis.select.style.display = 'block';
                pThis.select.selectedIndex = 0;
                pThis.textBox.style.display = 'none';
                pThis.textBox.value = '';

                if (setFocus) {
                    setTimeout(function () { pThis.select.focus(); }, 100);
                }
            }
        }

        function getSelectedOption() {
            if (pThis.select.selectedIndex > -1) {
                return pThis.select.options[pThis.select.selectedIndex];
            }

            return null;
        }

        function getTextBoxValue() {
            return pThis.textBox.value.customTrim();
        }

        function onChangeHandlerFactory() {
            var handlerState = {};

            var handler = function () {
                if (blockChangeEvent && !pThis.parameters.ignoreSecondaryEvents) {
                    handlerState.oldValue = pThis.getValue();
                    handlerState.initialized = true;
                    return;
                }

                var value = pThis.getValue();

                if (handlerState.oldValue != value) {
                    pThis.Events.change.raise({
                        caller: pThis,
                        value: value,
                        oldValue: handlerState.oldValue,
                        mode: getActiveMode(),
                        modes: viewMode
                    });
                }

                handlerState.oldValue = value;
                handlerState.initialized = true;
            }

            handler.setOldValue = function (value) {
                handlerState.oldValue = value;
            }

            return handler;
        }

        function findOption(text) {
            function normalize(inputString) {
                return (inputString || '').toString().customTrim().toLowerCase().replace('ё', 'е').replace('й', 'и');
            }

            for (var i = 0; i < pThis.select.options.length; i++) {
                var option = pThis.select.options[i];

                if (pThis.parameters.itemComparer(normalize(option.text), normalize(text))) {
                    return option;
                }
            }

            return null;
        }

        function itemComparer(firstItem, secondItem) {
            return firstItem == secondItem;
        }

        function appendOption(list, value, text) {
            var option = document.createElement('OPTION');

            option.setAttribute('value', value);
            option.innerHTML = text;

            list.appendChild(option);
        }

        function prepareControl(control) {
            if (!control) {
                return null;
            }

            function getControl(control) {
                if (common.getType(control) == 'string') {
                    return Common.$(control);
                }
                else {
                    return control;
                }
            }

            control = getControl(control);
            control.$ = $(control);

            return control;
        }
    }

})();


/* DATA LOSS PREVENTION */

(function () {

    var common = window.Common;

    if (!common.DataLossPrevention) {
        common.DataLossPrevention =
        {
            enabled: true,
            preventAttributeName: 'preventDataLoss',
            browserMessage: 'Несохраненные данные будут потеряны',
            confirmMessage: 'Несохраненные данные будут потеряны. Вы уверены, что хотите покинуть страницу?'
        };
    }

    var dataLossPrevention = common.DataLossPrevention;

    common.Events.PageParsed.attach(dataLossPrevention.updateInitialValues);
    common.Events.PageOnBeforeUnload.attach(cancelNavigateAwayIfRequired);

    dataLossPrevention.updateInitialValues = function ($controls) {
        var $filteredControls = $controls ? filterControls($controls) : getAllPreventLossControls();
        $filteredControls.each(function () { this.setAttribute('initialValue', getElementValue(this)); });
    }

    dataLossPrevention.checkIfDataChanged = function ($controls) {
        var $filteredControls = $controls ? filterControls($controls) : getAllPreventLossControls();

        for (var i = 0; i < $filteredControls.length; ++i) {
            if (isElementValueChanged($filteredControls[i])) {
                return true;
            }
        }

        return false;
    }

    function cancelNavigateAwayIfRequired() {
        if (dataLossPrevention.checkIfDataChanged() && dataLossPrevention.enabled) {
            return common.DataLossPrevention.browserMessage;
        }
    }

    function filterControls($controls) {
        return $controls.filter('[' + dataLossPrevention.preventAttributeName + ']');
    }

    function getAllPreventLossControls() {
        return $('[' + dataLossPrevention.preventAttributeName + ']');
    }

    function getElementValue(element) {
        switch (element.tagName.toLowerCase()) {
            case 'input':
                return getInputElementValue(element);
            case 'textarea':
                return element.value;
            case 'select':
                return getSelectBoxValue(element);
            default:
                return '';
        }
    }

    function getInputElementValue(inputElement) {
        switch (inputElement.type.toLowerCase()) {
            case 'hidden':
                return inputElement.value.replace(/\n$/i, '');
            case 'text':
                return inputElement.value;
            case 'checkbox':
            case 'radio':
                return inputElement.checked + '';
            default:
                return '';
        }
    }

    function getSelectBoxValue(selectElement) {
        return selectElement.selectedIndex + '';
    }

    function getInitialElementValue(element) {
        var initialValueAttribute = element.attributes['initialValue'];
        return !initialValueAttribute ? getElementValue(element) : initialValueAttribute.value;
    }

    function isElementValueChanged(element) {
        var currentValue = getElementValue(element);
        var initialValue = getInitialElementValue(element);

        return (initialValue.customTrim() != currentValue.customTrim());
    }

})();


/* TIMERS */
(function () {
    var common = window.Common;
    var timersStore = {};

    if (!common.Timers) {
        common.Timers = {};
    }

    common.Timers.trigger = function (code, ms, condition) {
        var timer = null;

        function run() {
            if (timer) {
                clearTimeout(timer);
            }

            if (condition()) {
                code();
            }

            timer = setTimeout(run, ms);
        }

        run();
    }

    common.Timers.startWithDelay = function (code, ms, conditionTrigger) {
        var timer = null;

        var run = function () {
            var args = arguments;
            var pThis = this;

            if (timer) {
                clearTimeout(timer);
            }

            timer = setTimeout(
                function () {
                    if (conditionTrigger && !conditionTrigger()) {
                        run.apply(pThis, args);
                        return;
                    }

                    code.apply(pThis, args);
                },
                ms);
        }

        return run;
    }

    common.Timers.createTimer = function (code, delay) {
        function Timer() {
            var timeout;
            var pThis = this;

            pThis.run = function () {
                var args = arguments;
                var pFunction = this;

                pThis.clear();

                timeout = setTimeout(function () { code.apply(pFunction, args); }, delay);
            }

            pThis.clear = function () {
                if (timeout) {
                    clearTimeout(timeout);
                }
            }
        }

        return new Timer();
    }

    common.Timers.createInterval = function (code, delay) {
        function Interval() {
            var interval;
            var pThis = this;

            pThis.start = function () {
                if (!interval) {
                    interval = setInterval(code, delay);
                }
            }

            pThis.stop = function () {
                if (interval) {
                    clearInterval(interval);
                }
            }

            pThis.enabled = function (value) {
                (value ? pThis.start : pThis.stop)();
            }
        }

        return new Interval();
    }

    common.Timers.startSingleTimer = function (code, delay, timerGuid) {
        if (!timersStore[timerGuid]) {
            timersStore[timerGuid] = setTimeout(function () {
                killTimer(timerGuid);
                code();
            }, delay);
        }
        else {
            killTimer(timerGuid);
            common.Timers.startSingleTimer(code, delay, timerGuid);
        }
    }

    function killTimer(timerGuid) {
        if (timersStore[timerGuid]) {
            clearTimeout(timersStore[timerGuid]);
        }

        timersStore[timerGuid] = undefined;
    }

})();



/* PROTOTYPE */
Date.prototype.toUtcDate = function () {
    return new Date(
        this.getUTCFullYear(),
        this.getUTCMonth(),
        this.getUTCDate(),
        this.getUTCHours(),
        this.getUTCMinutes(),
        this.getUTCSeconds(),
        this.getUTCMilliseconds());
}

Date.prototype.toRussianDateString = function (dateWithZeros) {
    var month = '';

    switch (this.getMonth()) {
        case 0:
            month = 'января';
            break;
        case 1:
            month = 'февраля';
            break;
        case 2:
            month = 'марта';
            break;
        case 3:
            month = 'апреля';
            break;
        case 4:
            month = 'мая';
            break;
        case 5:
            month = 'июня';
            break;
        case 6:
            month = 'июля';
            break;
        case 7:
            month = 'августа';
            break;
        case 8:
            month = 'сентября';
            break;
        case 9:
            month = 'октября';
            break;
        case 10:
            month = 'ноября';
            break;
        case 11:
            month = 'декабря';
            break;
    }

    return ([
        dateWithZeros ? Common.addZerosToStart(this.getDate(), 2) : this.getDate(), ' ',
        month, ' ',
        this.getFullYear(), ' г.'
    ]).join('');
}

Date.prototype.toRussianTimeString = function () {
    return ([
        Common.addZerosToStart(this.getHours(), 2),
        ':',
        Common.addZerosToStart(this.getMinutes(), 2)
    ]).join('');
}

Array.prototype.customUnique = function () {
    var a = this.concat();
    for (var i = 0; i < a.length; ++i) {
        for (var j = i + 1; j < a.length; ++j) {
            if (a[i] === a[j])
                a.splice(j, 1);
        }
    }

    return a;
};

Array.prototype.customRemove = function (delegate) {
    var i = 0;

    while (i < this.length) {
        if (delegate(this[i])) {
            delete this[i];
            this.splice(i, 1);
        }
        else {
            i++;
        }
    }

    return this;
}

Array.prototype.customRemoveFirst = function (element, notDelegate) {
    var index = this.customIndexOf(element, notDelegate);

    if (index > -1) {
        delete this[i];
        this.splice(i, 1);
    }

    return this;
}

Array.prototype.customWhere = function (delegate) {
    var result = [];

    for (var i = 0; i < this.length; i++) {
        if (delegate(this[i], i)) {
            result.push(this[i]);
        }
    }

    return result;
}

Array.prototype.customFirst = function (delegate) {
    if (arguments.length == 0) {
        return this[0];
    }

    for (var i = 0; i < this.length; i++) {
        if (delegate(this[i], i)) {
            return this[i];
        }
    }

    return null;
}

Array.prototype.customLast = function (delegate) {
    if (arguments.length == 0) {
        return this[this.length - 1];
    }

    for (var i = this.length - 1; i > 0 ; i--) {
        if (delegate(this[i], i)) {
            return this[i];
        }
    }

    return null;
}

Array.prototype.customSelect = function (delegate) {
    var result = [];

    for (var i = 0; i < this.length; i++) {
        result.push(delegate(this[i], i));
    }

    return result;
}

Array.prototype.customSort = function (delegate) {
    var result = [];

    function comparer(a, b) {
        var bValue = delegate(b);
        var aValue = delegate(a);

        if (aValue < bValue)
            return -1;
        if (aValue > bValue)
            return 1;
        return 0;
    }

    for (var i = 0; i < this.length; i++) {
        result.push(this[i]);
    }

    result.sort(comparer);

    return result;
}


Array.prototype.customSelectMany = function (delegate) {
    var result = [];

    for (var i = 0; i < this.length; i++) {
        result = result.concat(delegate(this[i], i));
    }

    return result;
}

Array.prototype.customForEach = function (delegate) {
    for (var i = 0; i < this.length; i++) {
        delegate(this[i], i, this.length);
    }

    return this;
}

Array.prototype.customSkip = function (count) {
    return this.slice(count);
}


Array.prototype.customJoin = function (arr, delegate) {
    var result = [];

    if (!arr) {
        return [];
    }

    for (var i = 0; i < this.length; i++) {
        for (var j = 0; j < arr.length; j++) {
            if (delegate(this[i], arr[j], i, j)) {
                var joinedItem =
                {
                    left: this[i],
                    right: arr[j]
                }

                result.push(joinedItem);
            }
        }
    }

    return result;
}


Array.prototype.customIndexOf = function (delegate, notDelegate) {
    if (typeof (delegate) == "function" && !notDelegate) {
        for (var i = 0; i < this.length; i++) {
            if (delegate(this[i])) {
                return i;
            }
        }
    }
    else {
        for (var i = 0; i < this.length; i++) {
            if (this[i] == delegate) {
                return i;
            }
        }
    }

    return -1;
}

Array.prototype.customLastIndexOf = function (delegate, notDelegate) {
    if (typeof (delegate) == "function" && !notDelegate) {
        for (var i = this.length - 1; i >= 0; i--) {
            if (delegate(this[i])) {
                return i;
            }
        }
    }
    else {
        for (var i = this.length - 1; i >= 0 ; i--) {
            if (this[i] == delegate) {
                return i;
            }
        }
    }

    return -1;
}

Array.prototype.customMin = function (delegate) {
    var result = 0;
    for (var i = 0; i < this.length; i++) {
        var val = delegate(this[i]);
        if (val < result) {
            result = val;
        }
    }
    return result;
}
Array.prototype.customMax = function (delegate) {
    var result = 0;
    for (var i = 0; i < this.length; i++) {
        var val = delegate(this[i]);
        if (val > result) {
            result = val;
        }
    }
    return result;
}

Array.prototype.shellSort = function (delegate) {
    if (typeof (delegate) != "function") {
        throw "Compare delegate at Shell sort is not a function";
    }

    var stop, swap, limit, temp;
    var x = Math.floor(this.length / 2) - 1;

    while (x > 0) {
        stop = 0;
        limit = this.length - x;
        while (stop == 0) {
            swap = 0;
            for (var k = 0; k < limit; k++) {
                if (delegate(this[k], this[k + x]) > 0) {
                    temp = this[k];
                    this[k] = this[k + x];
                    this[k + x] = temp;
                    swap = k;
                }
            }

            limit = swap - x;

            if (swap == 0) {
                stop = 1;
            }
        }

        x = Math.floor(x / 2);
    }
}


Array.prototype.customFindFirst = function (delegate) {
    return this[this.customIndexOf(delegate)];
}

Array.prototype.customFindLast = function (delegate) {
    return this[this.customLastIndexOf(delegate)];
}

Array.prototype.customContains = function (value, notDelegate) {
    return this.customIndexOf(value, notDelegate) != -1;
}

Array.prototype.customAll = function (delegate) {
    for (var i = 0; i < this.length; i++) {
        if (!delegate(this[i], i)) {
            return false;
        }
    }

    return true;
}

Array.prototype.customAny = function (delegate) {
    for (var i = 0; i < this.length; i++) {
        if (delegate(this[i], i)) {
            return true;
        }
    }

    return false;
}

var stringCustomTrimCache = [];

if (String.prototype.trim) {
    String.prototype.customTrim = String.prototype.trim;
}
else {
    String.prototype.customTrim = function () {
        if (!this) {
            return '';
        }

        var cachedValue = stringCustomTrimCache[this];

        if (cachedValue) {
            return cachedValue;
        }

        var str = this;
        var from;
        var to;
        var trimOccured = false;
        var whitespace = ' \n\r\t\f\x0b\xa0\u2000\u2001\u2002\u2003\u2004\u2005\u2006\u2007\u2008\u2009\u200a\u200b\u2028\u2029\u3000';
        var whiteSpaceOccured = false;

        for (var from = 0; from < str.length; from++) {
            if (whitespace.indexOf(str.charAt(from)) === -1) {
                trimOccured = true;
                break;
            }

            whiteSpaceOccured = true;
        }

        for (to = str.length - 1; to >= from; to--) {
            if (whitespace.indexOf(str.charAt(to)) === -1) {
                trimOccured = true;
                break;
            }
        }

        if (trimOccured > 0) {
            str = str.substring(from, to + 1);
        }
        else if (whiteSpaceOccured) {
            str = '';
        }

        stringCustomTrimCache[this] = str;

        return str;
    }
}

String.prototype.toLowerFirst = function () {
    return this.charAt(0).toLowerCase() + this.slice(1);
}



if (!this.JSON) {
    this.JSON = {};
}

(function () {

    function f(n) {
        return n < 10 ? '0' + n : n;
    }

    if (typeof Date.prototype.toJSON !== 'function') {

        Date.prototype.toJSON = function (key) {

            return isFinite(this.valueOf()) ?
                   this.getUTCFullYear() + '-' +
                 f(this.getUTCMonth() + 1) + '-' +
                 f(this.getUTCDate()) + 'T' +
                 f(this.getUTCHours()) + ':' +
                 f(this.getUTCMinutes()) + ':' +
                 f(this.getUTCSeconds()) + 'Z' : null;
        };

        String.prototype.toJSON =
        Number.prototype.toJSON =
        Boolean.prototype.toJSON = function (key) {
            return this.valueOf();
        };
    }

    var cx = new RegExp('[\u0000\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]', 'g');
    var escapable = new RegExp('[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]', 'g');

    var gap,
        indent,
        meta = {    // table of character substitutions
            '\b': '\\b',
            '\t': '\\t',
            '\n': '\\n',
            '\f': '\\f',
            '\r': '\\r',
            '"': '\\"',
            '\\': '\\\\'
        },
        rep;


    function quote(string) {
        escapable.lastIndex = 0;
        return escapable.test(string) ?
            '"' + string.replace(escapable, function (a) {
                var c = meta[a];
                return typeof c === 'string' ? c :
                    '\\u' + ('0000' + a.charCodeAt(0).toString(16)).slice(-4);
            }) + '"' :
            '"' + string + '"';
    }


    function str(key, holder) {

        var i,          // The loop counter.
            k,          // The member key.
            v,          // The member value.
            length,
            mind = gap,
            partial,
            value = holder[key];

        if (value && typeof value === 'object' &&
                typeof value.toJSON === 'function') {
            value = value.toJSON(key);
        }

        if (typeof rep === 'function') {
            value = rep.call(holder, key, value);
        }

        switch (typeof value) {
            case 'string':
                return quote(value);

            case 'number':

                return isFinite(value) ? String(value) : 'null';

            case 'boolean':
            case 'null':

                return String(value);

            case 'object':

                if (!value) {
                    return 'null';
                }

                gap += indent;
                partial = [];

                if (Object.prototype.toString.apply(value) === '[object Array]') {

                    length = value.length;
                    for (i = 0; i < length; i += 1) {
                        partial[i] = str(i, value) || 'null';
                    }

                    v = partial.length === 0 ? '[]' :
                        gap ? '[\n' + gap +
                                partial.join(',\n' + gap) + '\n' +
                                    mind + ']' :
                              '[' + partial.join(',') + ']';
                    gap = mind;
                    return v;
                }

                if (rep && typeof rep === 'object') {
                    length = rep.length;
                    for (i = 0; i < length; i += 1) {
                        k = rep[i];
                        if (typeof k === 'string') {
                            v = str(k, value);
                            if (v) {
                                partial.push(quote(k) + (gap ? ': ' : ':') + v);
                            }
                        }
                    }
                } else {

                    for (k in value) {
                        if (Object.hasOwnProperty.call(value, k)) {
                            v = str(k, value);
                            if (v) {
                                partial.push(quote(k) + (gap ? ': ' : ':') + v);
                            }
                        }
                    }
                }

                v = partial.length === 0 ? '{}' :
                    gap ? '{\n' + gap + partial.join(',\n' + gap) + '\n' +
                            mind + '}' : '{' + partial.join(',') + '}';
                gap = mind;
                return v;
        }
    }

    if (typeof JSON.stringify !== 'function') {
        JSON.stringify = function (value, replacer, space) {

            var i;
            gap = '';
            indent = '';

            if (typeof space === 'number') {
                for (i = 0; i < space; i += 1) {
                    indent += ' ';
                }

            } else if (typeof space === 'string') {
                indent = space;
            }

            rep = replacer;
            if (replacer && typeof replacer !== 'function' &&
                    (typeof replacer !== 'object' ||
                     typeof replacer.length !== 'number')) {
                throw new Error('JSON.stringify');
            }

            return str('', { '': value });
        };
    }


    if (typeof JSON.parse !== 'function') {
        JSON.parse = function (text, reviver) {

            var j;

            function walk(holder, key) {

                var k, v, value = holder[key];
                if (value && typeof value === 'object') {
                    for (k in value) {
                        if (Object.hasOwnProperty.call(value, k)) {
                            v = walk(value, k);
                            if (v !== undefined) {
                                value[k] = v;
                            } else {
                                delete value[k];
                            }
                        }
                    }
                }
                return reviver.call(holder, key, value);
            }

            text = String(text);
            cx.lastIndex = 0;
            if (cx.test(text)) {
                text = text.replace(cx, function (a) {
                    return '\\u' +
                        ('0000' + a.charCodeAt(0).toString(16)).slice(-4);
                });
            }

            if ((/^[\],:{}\s]*$/).test(text.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, '@').replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']').replace(/(?:^|:|,)(?:\s*\[)+/g, ''))) {

                j = eval('(' + text + ')');


                return typeof reviver === 'function' ?
                    walk({ '': j }, '') : j;
            }


            throw new SyntaxError('JSON.parse');
        };
    }
}());

function y(expression, context) {
    context = context || window;

    var match = expression.split(/\s*=>\s*/);
    var left = match[0];
    var right = match[1];

    if (!(/^\s*return\s+/gi).test(right)) {
        right = 'return ' + right;
    }

    var args = left.split(/\s*,\s*/);
    args.push(right);

    return Function.apply(context, args);
}

var parseBool = function (str) {
    var result;
    result = str === true || str === "true" || str === "True" || str === 1 || str === "1";
    return result;
};

RegExp.escape = function (text) {
    if (!text) {
        return '';
    }

    return text.replace(/[-[\]{}()*+?.,\\^$|#\s]/g, "\\$&");
};

(function () {
    function isEmpty(value) {
        return Common.Validation.rules.emptyOrNull(value);
    }

    Common.Fraction = {};

    Common.Fraction.create = function (numerator, denominator) {
        if (isEmpty(numerator) && isEmpty(denominator)) {
            return null;
        }

        var numerator = Common.Validation.valueProcessors.integerNumber(numerator.toString());
        var denominator = Common.Validation.valueProcessors.integerNumber(denominator.toString());

        return { numerator: numerator, denominator: denominator };
    };

    Common.Fraction.createEmpty = function () {
        return { numerator: '', denominator: '' };
    };

    Common.Fraction.getMaxCommonDivisor = function (a, b) {
        while (b != 0) {
            var remainder = a % b;
            a = b;
            b = remainder;
        }

        return a;
    };

    Common.Fraction.compare = function (a, b) {
        if (!a && !b) {
            return true;
        }

        if (!a || !b) {
            return false;
        }

        return a.numerator == b.numerator && a.denominator == b.denominator;
    };

    Common.Fraction.isEmpty = function (value) {
        return !value || isEmpty(value.numerator) || isEmpty(value.denominator);
    };

    Common.Fraction.convertToDecimal = function (value, precision, multiply) {
        if (Common.Fraction.isEmpty(value)) {
            return '';
        }

        var result = value.numerator / value.denominator;

        if (multiply) {
            result = result * multiply;
        }

        if (!precision) {
            precision = 5;
        }

        result = result.toFixed(precision);
        result = Common.Validation.valueProcessors.decimalNumber(result, precision);

        return result;
    };

    Common.Fraction.convertToPercent = function (value) {
        return Common.Fraction.convertToDecimal(value, 4, 100);
    };

    Common.Fraction.convertFromDecimal = function (value, divide) {
        if (isEmpty(value)) {
            return null;
        }

        if (!value) {
            return Common.Fraction.create(0, 1);
        }

        if (divide) {
            value /= divide;
        }

        var index = value.toString().indexOf('.');

        if (index >= 0) {
            numerator = value.toString().replace(new RegExp('[.]', 'gi'), '');
            numerator = Common.Validation.valueProcessors.integerNumber(numerator);
            numerator = parseInt(numerator) || 0;
            denominator = Math.pow(10, value.toString().length - index - 1);

            var d = Common.Fraction.getMaxCommonDivisor(numerator, denominator);

            numerator = numerator / d;
            denominator = denominator / d;
        }
        else {
            denominator = 1;
            numerator = parseInt(value) || 0;
        }

        return Common.Fraction.create(numerator, denominator);
    };

    Common.Fraction.convertFromPercent = function (value) {
        return Common.Fraction.convertFromDecimal(value, 100);
    };
}());