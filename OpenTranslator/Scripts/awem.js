awem = function ($) {
    var minZindex = 1;
    var maxLookupDropdownHeight = 360;
    var maxDropdownHeight = 420;
    var popSpace = 20;
    var hpSpace = popSpace / 2;
    var reverseDefaultBtns;
    var closePopOnOutClick = 0;
    var $doc = $(document);
    var $win = $(window);
    // keys you can type without opening menu dropdown
    // enter, esc, shift, left arrow, right arrow, tab
    var nonOpenKeys = [13, 27, 16, 37, 39, 9]; // keys that won't open the menu

    // down and up arrow, enter, esc, shift //, left arrow, right arrow
    var controlKeys = [40, 38, 13, 27, 16]; //, 37, 39

    var nonComboSearchKeys = [40, 38, 13, 27, 16, 37, 39, 9];

    var updownKeys = [38, 40];

    var isMobile = function () { return awem.isMobileOrTablet(); };

    var keycode = {
        enter: 13,
        backspace: 8,
        esc: 27,
        down: 40,
        up: 38,
        tab: 9
    };

    var loadingHtml = '<div class="awe-loading"><span></span></div>';
    var sawecolschange = 'awecolschanged';
    var saweinit = 'aweinit';
    var saweinl = 'aweinline';
    var saweinlsave = saweinl + 'save';
    var saweinlinv = saweinl + 'invalid';
    var saweinledit = saweinl + 'edit';
    var saweinlcancel = saweinl + 'cancel';
    var sddpOutClEv = 'mousedown.ddp keyup.ddp';
    var sfocus = 'focus';
    var soddDocClEv = 'mousedown touchstart keydown';

    var cache = {};
    var dpop = {};

    $(function () {
        if (minZindex == 1) {
            var nav = $('.navbar-fixed-top:first');
            if (nav.length) {
                minZindex = nav.css('z-index');
            }
        }
    });

    function cd() {
        return awem.clientDict;
    }

    function K(item) {
        return item.k;
    }

    function C(item) {
        return item.c;
    }

    function format(s, args) {
        return s.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined'
      ? args[number]
      : match;
        });
    };

    function toUpperFirst(s) {
        return s.substr(0, 1).toUpperCase() + s.substr(1);
    }

    function toLowerFirst(s) {
        return s.substr(0, 1).toLowerCase() + s.substr(1);
    }

    function containsVal(itemK, vals) {
        var res = false;
        $.each(vals, function (_, value) {
            if (itemK == escape(value)) {
                res = true;
                return false;
            }
        });

        return res;
    }

    function inArray(val, vals) {
        var res = -1;
        $.each(vals, function (i, value) {
            if (val == value) {
                res = i;
                return false;
            }
        });

        return res;
    }

    function contains(key, keys) {
        return $.inArray(key, keys) != -1;
    }

    function strContainsi(c, squeryUp) {
        return (c || '').toString().toUpperCase().indexOf(squeryUp) != -1;
    }

    function strEqualsi(c, squeryUp) {
        return (c || '').toString().toUpperCase() == squeryUp;
    }

    function pickAvEl(arr) {
        for (var i = 0; i < arr.length; i++) {
            if (arr[i].length) {
                return arr[i];
            }
        }
    }

    function setDisabled(o, s) {
        if (s) {
            o.attr('disabled', 'disabled');
        } else {
            o.removeAttr('disabled');
        }
    }

    function prevDef(e) {
        e.preventDefault();
    }

    var entityMap = {
        "&": "&amp;",
        "<": "&lt;",
        '"': '&quot;',
        "'": "&#39;",
        ">": "&gt;"
    };

    function escape(str) {
        return String(str).replace(/[&<>"']/g, function (s) {
            return entityMap[s];
        });
    }

    function toStr(v) {
        return (v == null) ? '' : v.toString();
    }

    function unesc(str) {
        str = toStr(str);
        for (key in entityMap) {
            str = str.replace(entityMap[key], key);
        }
        return str;
    }

    function outerh(sel, m) {
        return sel.length ? sel.outerHeight(!!m) : 0;
    }

    function readTag(o, prop, nullVal) {
        var res = nullVal;

        if (o.tag && o.tag[prop] != null) {
            res = o.tag[prop];
        }

        return res;
    }

    function dapi(o) {
        return o.data('api');
    }

    function dto(o) {
        return o.data('o');
    }

    function getZIndex(el) {
        var val = el.css('z-index');
        if (val && val > 0) return val;
        var parent = el.parent();
        if (parent && !parent.is($('body'))) return getZIndex(parent);
    }

    function calcZIndex(zIndex, el) {
        if (zIndex < minZindex) zIndex = minZindex;
        var zi = getZIndex(el);
        if (zi && zi > zIndex) {
            zIndex = zi;
        }

        return zIndex;
    }

    function setGridHeight(grid, newh) {
        var go = dto(grid);
        if (go.h != newh) {
            go.h = newh;
            dapi(grid).lay();
        }
    }

    function scrollTo(focused, cont) {
        function y(o) {
            return o.offset().top;
        }

        var fry = y(focused);
        var fh = focused.height();
        var conh = cont.height();
        var miny = y(cont);
        var maxy = miny + conh - fh;
        var scrcont = cont;
        var winmax = $win.height() + $doc.scrollTop() - fh;
        var winmin = $doc.scrollTop();
        if (maxy > winmax && winmax < fry) {
            maxy = winmax;
            scrcont = $win;
        }
        if (miny < winmin && winmin > fry) {
            miny = winmin;
            scrcont = $win;
        }
        var delta = fry < miny ? fry - miny : fry > maxy ? fry - maxy : 0;
        // +1 for ie and ff 
        if (delta > fh + 1 && scrcont != $win) {
            delta += conh / 2;
        }
        scrcont.scrollTop(scrcont.scrollTop() + delta);
    }

    function initgridh(grid) {
        var o = dto(grid);
        o.h = o.ih;
        dapi(grid).lay();
    }

    function cdelta(grid, val) {
        grid.trigger('awerowch', val);
    }

    function movedGridRow(fromgrid, togrid) {
        dto(togrid).lrso = 1;
        dto(fromgrid).lrso = 1;
        cdelta(togrid, 1);
        cdelta(fromgrid, -1);
        if (!fromgrid.find('.awe-row').length && dto(fromgrid).lrs.pc > 1) {
            dapi(fromgrid).load();
        }
    }

    function layDropdownPopup2(o, pop, isFixed, capHeight, $opener, setHeight, keepPos, canShrink, chkfulls, minh, popuph, maxph) {

        if (!keepPos) {
            pop.css('left', 0);
            pop.css('top', 0);
        }

        var winh = $win.height();
        var winw = $win.width();

        var maxw = winw - popSpace;

        var mnw = Math.min(pop.outerWidth(), maxw);

        pop.css('min-height', '');
        pop.css('height', '');
        pop.css('max-width', maxw);
        pop.css('min-width', canShrink ? '' : mnw);
        pop.css('position', '');

        var scrolltop = $win.scrollTop();
        var toppos;
        var left;

        var topd = scrolltop;

        if ($opener) {
            topd = $opener.offset().top;
            capHeight = capHeight || $opener.outerHeight();
        }

        // handle opener overflow
        var botoverflow = topd - (winh + scrolltop);
        if (botoverflow > 0) {
            topd -= botoverflow;
        }

        var topoverflow = scrolltop - (topd + capHeight);

        if (topoverflow > 0) {
            topd += topoverflow;
        }

        var top = topd - scrolltop;
        var bot = winh - (top + capHeight);

        // adjust height
        var poph = popuph || pop.outerHeight();


        if (!o.maxph) o.maxph = poph;
        else if (o.maxph > poph) poph = o.maxph;
        else o.maxph = poph;

        var autofls = chkfulls(poph);

        var valign = 'bot';
        if (autofls) {
            isFixed = 1;
        } else {
            var stop = top - hpSpace;
            var sbot = bot - hpSpace;
            var maxh = sbot;

            if (minh > poph) minh = poph;

            if (sbot > poph) {
                valign = 'bot';
            }
            else if (stop > sbot) {
                valign = 'top';
                if (poph > stop) {
                    poph = stop;
                }

                maxh = stop;
            } else {
                poph = sbot;
            }

            if (poph < minh) {
                maxh = poph = minh;
            }

            if (maxph && poph > maxph) {
                poph = maxph;
            }

            if (poph > winh - popSpace) {
                maxh = poph = winh - popSpace;
            }

            setHeight(poph, maxh, valign);
        }

        if (isFixed) {
            topd = top;
            pop.css('position', 'fixed');
        }

        var w = pop.outerWidth(true);
        var h = pop.outerHeight(true);
        if (o.avh) h = o.avh + o.nph;

        if ($opener) {
            left = $opener.offset().left;
            var lspace = winw - (left + w);
            if (lspace < 0) {

                var ow = $opener.outerWidth(true);
                if (ow < w)
                    left -= (w - ow);

                if (left < 10) {
                    left = 10;
                }
            }

            if (autofls) {
                left = toppos = hpSpace;
            }
            else if (bot < h + hpSpace && (top > h + hpSpace || top > (bot))) {
                //top
                toppos = topd - h;

                if (top < h) {
                    toppos = topd - top;
                    if (h + hpSpace < winh) toppos += hpSpace;
                }
            } else {
                //bot
                toppos = topd + capHeight;

                if (bot < h + hpSpace) {

                    toppos -= (h - bot);

                    if (h + hpSpace < winh) toppos -= hpSpace;
                }
            }
        } else {
            var diff = winh - h;
            toppos = diff / 2;
            if (diff > 200) toppos -= 45;

            left = Math.max((winw - pop.outerWidth()) / 2, 0);
        }

        if (!keepPos || autofls) {
            pop.css('left', left);
            pop.css('top', Math.floor(toppos));
        }
    }

    function buttonGroupRadio(o) {
        return nbuttonGroup(o);
    }

    function buttonGroupCheckbox(o) {
        return nbuttonGroup(o, 1);
    }

    function bootstrapDropdown(o) {
        function render() {
            o.d.empty();
            var caption = cd().Select;
            var items = '';
            $.each(o.lrs, function (i, item) {
                var checked = $.inArray(K(item), awe.val(o.v)) > -1;
                if (checked) caption = C(item);
                items += '<li role="presentation"><input style="display:none;" type="radio" value="' + K(item) + '" name="' + o.nm + '" ' + (checked ? 'checked="checked"' : '') +
                    '" /><a role="menuitem" tabindex="-1" href="#" >' + C(item) + '</a></li>';
            });
            if (!items) items = "<li class='empty'>(empty)</li>";
            var res = '<div class="dropdown"><button class="btn btn-default dropdown-toggle" type="button" data-toggle="dropdown" aria-expanded="true"><span class="caption">'
                + caption +
                '</span> <i class="caret"></i></button><ul class="dropdown-menu" role="menu">' + items + "</ul><div>";

            o.d.append(res);
        };

        dapi(o.v).render = render;

        o.v.on('change', render);

        o.d.on('click', 'a', function (e) {
            prevDef(e);
            $(this).prev().click();//click on the hidden radiobutton
        });
    }

    function nbuttonGroup(o, multiple) {
        var $odisplay;

        function init() {
            $odisplay = o.mo.odisplay;
            o.f.addClass('buttonGroup');

            $odisplay.on('click', '.awe-btn', function () {
                o.api.toggleVal($(this).data('val'));
            });
        }

        function setSelectionDisplay() {
            var val = awe.val(o.v);

            var items = '';
            $.each(o.lrs, function (index, item) {
                var selected = containsVal(K(item), val) ? "awe-selected" : "";
                items += '<button type="button" class="awe-btn awe-il ' + selected + '" data-index="' + index + '" data-val="' + K(item) + '">' + C(item) + '</button>';
            });

            $odisplay.html(items);
        }

        function setSelectionDisplayChange() {
            var vals = awe.val(o.v);
            $odisplay.children('.awe-selected').removeClass('awe-selected');
            $.each(vals, function (i, v) {
                $odisplay.children().filter(function () {
                    return $(this).data('val') == v;
                }).addClass('awe-selected');
            });
        }

        var opt = {
            setSel: setSelectionDisplay,
            setSelChange: setSelectionDisplayChange,
            init: init,
            multiple: multiple,
            noMenu: 1
        };

        return odropdown(o, opt);
    }

    function multiselb(o) {
        o.d.addClass("multiselb");
        function renderCaption() {
            return o.mo.inlabel;
        }

        return odropdown(o, {
            multiple: 1,
            renderCaption: renderCaption
        });
    }

    function multiselect(o) {
        var captionText;
        var $multi = $('<div class="multi"></div>');
        var $searchtxt = $('<input type="text" class="osearch awe-il awe-txt" id="' + o.i + '-awed" />');
        var $dropmenu;
        var $caption;
        var glrs;
        var api;
        o.d.addClass("multiselect");
        var reor = readTag(o, "Reor");

        if (!isMobile())
            $multi.append($searchtxt);

        function init() {
            o.mo.odisplay.append($multi);
            $dropmenu = o.mo.dropmenu;
            captionText = o.mo.caption;

            if (!isMobile()) {
                o.mo.srctxt = $searchtxt;
            }

            api = o.api;
            glrs = api.glrs;
        }

        function handleCaption() {
            if ($multi.find('.multiRem').length)
                $caption.hide();
            else
                $caption.show();
        }

        var setSelectionDisplay = function () {
            var vals = awe.val(o.v);
            $multi.find('.caption').remove();
            $multi.find('.multiItem').remove();
            var items = '';
            var lrs = glrs();

            $.each(vals, function (_, val) {
                $.each(lrs, function (lindex, item) {
                    if (K(item) == escape(val)) {
                        items += renderSelectedItem(item, lindex);
                        return false;
                    }
                });
            });

            $caption = $('<span class="caption">' + captionText + '</span>');

            if (items) {
                autoWidth($searchtxt);
                $caption.hide();
            }

            $multi.prepend(items);
            $multi.prepend($caption);
            $searchtxt.val('');
        };

        function setSelectionDisplayChange() {
            var vals = awe.val(o.v);

            // remove keys and add items
            $multi.find('.multiRem').each(function () {
                var val = $(this).parent().data('val');
                var indexFound = inArray(val, vals);
                if (indexFound > -1) {
                    //remove from vals
                    vals.splice(indexFound, 1);
                } else {
                    $(this).parent().remove();
                }
            });

            //add multiRem for remaining vals
            var items = '';
            $.each(glrs(), function (index, item) {
                if (containsVal(K(item), vals)) {
                    items += renderSelectedItem(item, index);
                }
            });

            $searchtxt.val('');
            autoWidth($searchtxt);

            if (isMobile()) {
                $multi.append(items);
            } else {
                $searchtxt.before(items);
            }

            handleCaption();
        }

        $multi.on('click', function (e) {
            if (!$(e.target).is('.multiRem')) {

                if (!$dropmenu.hasClass('open')) {
                    api.toggleOpen();
                }

                $searchtxt.focus();
            }
        });

        $searchtxt.on('focusin', function () {
            $caption.hide();
            autoWidth($(this));
        });

        function autoWidth(input) {
            input.css('width', Math.min(Math.max((input.val().length + 2) * 10, 21), $multi.width()) + 'px');
        }

        var opt = {
            setSel: setSelectionDisplay,
            setSelChange: setSelectionDisplayChange,
            init: init,
            multiple: 1,
            prerender: function () { },
            afls: 0
        };

        function renderSelectedItem(item, index) {
            return '<div class="multiItem awe-il awe-btn" data-val="' + K(item) + '"><span class="multiCap">' + opt.renSelCap(item) + '</span><span class="multiRem" data-index="' + index + '">×</span></div>';
        }

        $searchtxt.on('keyup', function (e) {
            if (!$dropmenu.hasClass('open') && !contains(e.which, nonOpenKeys)) {
                if (!(e.which == keycode.backspace && !$searchtxt.val()))
                    dapi(o.v).toggleOpen();
            }

            if (!contains(e.which, nonComboSearchKeys)) {
                var query = $(this).val();

                dapi(o.v).search(query);
            }
        });

        $searchtxt.on('keydown', function (e) {
            if (e.which == keycode.backspace && !$searchtxt.val()) {
                $multi.find('.multiRem:last').click();
            }

            autoWidth($searchtxt);
        });

        $searchtxt.on('focusout', function () {
            $searchtxt.val('').change();
            if (!$multi.children('.multiItem').length) {
                $searchtxt.css('width', '0');
                $caption.show();
            }
        });

        $multi.on('click', '.multiRem', function () {

            var val = $(this).parent().data('val');

            api.toggleVal(val);
            api.close();
            $searchtxt.focus();
        });

        if (reor) {
            var placeholder, drgObj, others, last;
            var justmoved, initm;
            function wrap(clone, dragObj) {
                initm = 1;
                placeholder = dragObj.clone().addClass('awe-changing placeh').hide();

                drgObj = dragObj.after(placeholder);
                others = $multi.find('.multiItem:not(.placeh)');
                last = $multi.find('.multiItem').last();
                return clone;
                //return $('</div>');
            }

            function end() {
                placeholder.remove();
                drgObj.show();
            }


            function hoverFunc(dragObj, x, y) {
                if (initm) {
                    drgObj.hide();
                    placeholder.show();
                    initm = 0;
                }
                var hovered;
                others.each(function (_, el) {
                    var mi = $(el);
                    var mix = mi.offset().left;
                    var miy = mi.offset().top;

                    if (x > mix && x < mix + mi.width() &&
                        y > miy && y < miy + mi.height()) {
                        hovered = mi;
                        return false;
                    }
                });

                if (justmoved) {
                    if (!hovered) {
                        justmoved = null;
                    } else if (justmoved.is(hovered)) {
                        hovered = null;
                    }
                }

                if (hovered) {
                    if (hovered.index() < placeholder.index()) {
                        hovered.before(placeholder);
                    } else {
                        hovered.after(placeholder);
                    }

                    justmoved = hovered;
                }
            }

            function dropFunc() {
                placeholder.after(drgObj).remove();
                api.moveVal(drgObj.data('val'), drgObj.prev().data('val'));
                drgObj.show();
            }

            dragAndDrop({
                from: $multi,
                sel: '.multiItem',
                to: [{ c: $('body'), drop: dropFunc, hover: hoverFunc }],
                wrap: wrap,
                end: end
            });
        }

        if (!isMobile()) {
            opt.searchOutside = 1;
            opt.noAutoSearch = 1;
        }

        return odropdown(o, opt);
    }

    function colorDropdown(o) {
        var caption;

        function init() {
            caption = o.mo.caption;
        }

        o.d.addClass("colordd");

        o.df = function () {
            return $.map(['#5484ED', '#A4BDFC', '#7AE7BF', '#51B749', '#FBD75B', '#FFB878', '#FF887C', '#DC2127', '#DBADFF', '#E1E1E1'],
                function (item) { return { k: item, c: item }; });
        };

        var renderCaption = function (selected) {
            var sel = caption;
            if (selected.length) {
                var color = K(selected[0]);
                sel = '<div style="background:' + color + '" class="color">&nbsp;</div>';
            }

            return sel;
        };

        var renderItemDisplay = function (item) {
            return '<span class="colorItem" style="background:' + K(item) + '">&nbsp;</span>';
        };

        var opt = {
            renderItemDisplay: renderItemDisplay,
            renderCaption: renderCaption,
            noAutoSearch: 1,
            menuClass: "colorddmenu",
            init: init
        };

        odropdown(o, opt);
    }

    function imgDropdown(o) {
        var caption;

        o.d.addClass('imgdd');

        function init() {
            caption = o.mo.caption;
        }

        var opt = {
            menuClass: "imgddmenu",
            init: init
        };

        opt.renderItemDisplay = function (item) {
            return '<div class="imgddItem"><img src="' + item.url + '"/> ' + C(item) + '</div>';
        };

        opt.renderCaption = function (selected) {
            var sel = caption;
            if (selected.length)
                sel = '<img src="' + selected[0].url + '"/>' + C(selected[0]);
            return sel;
        };

        odropdown(o, opt);
    }

    function timepicker(o) {
        o.f.addClass("timep");

        function pad(num) {
            var s = "00" + num;
            return s.substr(s.length - 2);
        }

        o.df = function () {
            var step = readTag(o, "Step") || 30;
            var items = [];
            var ampm = o.tag.AmPm;
            for (var i = 0; i < 24 * 60; i += step) {
                var apindx = 0;
                var hour = Math.floor(i / 60);
                var minute = i % 60;

                if (ampm) {

                    if (hour >= 12) {
                        apindx = 1;
                    }

                    if (!hour) {
                        hour = 12;
                    }

                    if (hour > 12) {
                        hour -= 12;
                    }
                }

                var item = ampm ? hour : pad(hour);

                item += ":" + pad(minute);

                if (ampm) item += " " + ampm[apindx];

                items.push(item);
            }

            return $.map(items, function (v) { return { k: v, c: v }; });
        };

        return combobox(o);
    }

    function combobox(o) {
        o.d.addClass('combobox');

        var $v = o.v;
        var $combotxt = $('<input type="text" class="awe-txt combotxt osearch" size="1" autocomplete="off" id="' + o.i + '-awed" />');
        var $openbtn = $('<button type="button" class="cdropbtn odropbtn oddbtn awe-btn" tabindex="-1"><span class="selbtn"><i class="ocaret"></i></span></button>');
        var docClickRegistered = 0;
        var glrs;
        var closeOnEmpty = readTag(o, "CloseOnEmpty");
        var api;
        var dropmenu;
        var vprop;
        var contval = '';

        function init() {
            o.mo.odisplay.append($combotxt).append($openbtn);
            o.mo.srctxt = $combotxt;
            vprop = o.mo.vprop;
            $combotxt.attr('placeholder', o.mo.caption);
            api = o.api;
            glrs = api.glrs;
            dropmenu = o.mo.dropmenu;
        }

        function setSelectionDisplay() {
            var vals = awe.val($v);

            var selected = $.grep(glrs(), function (item) {
                return containsVal(item[vprop], vals);
            });

            var txtval = '';
            if (!selected.length && vals.length) {
                txtval = vals[0];
            }
            else if (selected.length) {
                txtval = unesc(C(selected[0]));
            }

            $combotxt.val(txtval);
        }

        function docClickFocusHandler(e) {
            var $target = $(e.target);
            if (!$target.closest(dropmenu).length && !$target.closest(o.d).length) {
                compval();
                checkComboval();
                docClickRegistered = 0;
                $doc.off('click focusin', docClickFocusHandler);
            }
        }

        $combotxt.on('focusin', function () {
            this.selectionStart = this.selectionEnd = this.value.length;

            if (!docClickRegistered) {
                $doc.on('click focusin', docClickFocusHandler);
                docClickRegistered++;
            }
        });

        $combotxt.on('keydown', function (e) {
            if (e.which == keycode.enter && !dropmenu.hasClass('open')) {
                prevDef(e);
                checkComboval();
            }
        });

        $combotxt.on('keyup', function (e) {
            var val = $combotxt.val();
            var isOpen = dropmenu.hasClass('open');
            var key = e.which;

            if (closeOnEmpty && !val && !contains(key, updownKeys)) {
                if (isOpen) {
                    api.toggleOpen();
                }
            }
            else if (!isOpen) {
                if (!contains(key, nonOpenKeys)) {
                    api.toggleOpen();
                }

                if (key == keycode.enter) {
                    checkComboval();
                }
            }
        });

        function postSearchFunc(which) {
            if (!contains(which, nonComboSearchKeys)) {
                if (!dropmenu.find('.oitem:visible').length) {
                    api.close();
                }

                compval();
            }
        }

        $openbtn.on('click', function () {
            api.search('');
            if (!isMobile())
                $combotxt.focus();
        });

        function compval() {
            var query = $combotxt.val();
            var newVal = query;
            var cval = query;
            var indexFound;
            var itemFound;

            $.each(glrs(), function (i, item) {
                if (strEqualsi(C(item), query.toUpperCase())) {
                    indexFound = i;
                    itemFound = item;
                    newVal = item[vprop];
                    cval = C(item);
                    return false;
                }
            });

            dropmenu.find('.selected').removeClass('selected');

            if (itemFound) {
                dropmenu.find('.oitem').eq(indexFound).addClass('selected');
            }

            $v.data('comboval', newVal);
            contval = cval;
        }

        function checkComboval() {
            if (!$v.parent().length) {
                return;
            }

            var comboval = $v.data('comboval');

            if (comboval != null) {
                api.toggleVal(comboval);
                $combotxt.val(contval);
            }
        }

        odropdown(o, {
            searchOutside: 1,
            noAutoSearch: 1,
            setSel: setSelectionDisplay,
            setSelChange: setSelectionDisplay,
            Combo: 1,
            init: init,
            psf: postSearchFunc,
            prerender: function () { },
            afls: 0
        });
    }

    function menuDropdown(o) {
        o.d.addClass("menudd");
        var opt = {
            menuClass: "menuddmenu"
        };

        opt.renderCaption = function () {
            return o.mo.caption;
        };

        opt.renderItems = function (rs) {
            var res = '';
            $.each(rs, function (i, item) {
                var attrs = ' class="oitem" data-index="' + i + '"';
                if (item.click) attrs += ' data-click="' + item.click + '"';
                res += '<li' + attrs + '>' + opt.renderItemDisplay(item) + '</li>';
            });

            if (!rs.length) {
                res += '<li class="empty">(empty)</li>';
            }

            return res;
        };

        opt.renderItemDisplay = opt.renderItemDisplay || function (item) {
            var res;
            var href = K(item) || item.href;
            if (href && !item.click) {
                res = '<a href="' + href + '">' + C(item) + '</a>';
            } else {
                res = C(item);
            }

            return res;
        };

        opt.onItemClick = function (e) {
            var $trg = $(e.target);
            if ($trg.is('.oitem')) {
                var click = $trg.data('click');

                if (click) {
                    eval(click);
                } else {
                    var $a = $trg.find('a');
                    if ($a.length)
                        $a.get(0).click();
                }
            }

            o.api.close();
        };

        return odropdown(o, opt);
    }

    function odropdown(o, opt) {

        var api = o.api;
        api.render = render;
        api.glrs = glrs;
        api.toggleVal = toggleVal;
        api.moveVal = moveVal;

        opt = opt || {};
        if (opt.afls == null) opt.afls = 1;

        var btnCaption = $('<div class="caption"></div>');
        var btn = $('<button type="button" class="odropbtn oddbtn awe-btn" id="' + o.i + '-awed"><span class="selbtn"><i class="ocaret"></i></span></button>');
        var srcinfo = '<li class="oinfo">' + cd().SearchForRes + '</li>';

        var $odropdown = $('<div class="odropdown"></div>');
        var $odisplay = $('<div class="odisplay oldngp">' + loadingHtml + '</div>');

        var inlabel = readTag(o, 'InLabel', '');
        var caption = readTag(o, 'Caption', cd().Select);
        var autoSelectFirst = readTag(o, 'AutoSelectFirst');
        var noSelClose = readTag(o, 'NoSelClose');
        var minWidth = readTag(o, 'MinWidth');
        var searchFunc = readTag(o, 'SrcFunc');
        var cacheKey = readTag(o, "Key", o.i);
        var itemFunc = readTag(o, 'ItemFunc');
        var captionFunc = readTag(o, 'CaptionFunc');
        var useConVal = readTag(o, "UseConVal");
        var popupClass = readTag(o, "Pc");
        var vprop = useConVal ? 'c' : 'k';

        var valInputType = opt.multiple ? "checkbox" : "radio";

        var $valCont = $('<div class="valCont"></div>').hide();

        var hostc = $('body');
        var modal = $('<div class="opmodal opc" tabindex="-1" data-i="' + o.i + '"></div>');

        var $dropmenu = $('<div class="omenu opc" tabindex="-1" data-i="' + o.i + '"></div>').addClass(opt.menuClass).addClass(popupClass);

        if (o.rtl) $dropmenu.css('direction', 'rtl');
        if (minWidth) $odropdown.css('min-width', minWidth);

        var $menuSearchCont = $('<div class="osrccont oldngp"><input type="text" class="osearch awe-txt" placeholder="' + cd().Searchp + '" size="1"/>' + loadingHtml + '</div>');
        var $menuSearchTxt = $menuSearchCont.find('.osearch');
        var $itemscont = $('<div class="oitemscont"></div>');
        var $menu = $('<ul class="omenuitems" tabindex="-1">' + (searchFunc ? srcinfo : '') + '</ul>');
        var slistctrl = slist($itemscont, { sel: '.oitem' });
        var autofocus = slistctrl.autofocus;

        var isFixed = 0;
        var zIndex = minZindex;

        if (isMobile())
            $dropmenu.addClass('omobile');

        $odropdown.append($odisplay);
        $dropmenu.append($menuSearchCont);
        $dropmenu.append($itemscont);
        $itemscont.append($menu);

        o.d.append($valCont).append($odropdown);
        o.f.addClass('odfield');

        o.mo = { dropmenu: $dropmenu, odisplay: $odisplay, caption: caption, odropdown: $odropdown, inlabel: inlabel, msrctxt: $menuSearchTxt, vprop: vprop };

        opt.renderItemDisplay = opt.renderItemDisplay || function (item) {
            return itemFunc ? eval(itemFunc)(item) : C(item);
        };

        opt.renderCaption = opt.renderCaption || function (selected) {
            var sel = caption;
            if (selected.length) {
                sel = opt.renSelCap(selected[0]);
            }

            return inlabel + sel;
        };

        opt.renSelCap = opt.renSelCap || function (item) {
            return captionFunc ? eval(captionFunc)(item) : C(item);
        }

        opt.setSel = opt.setSel || function () {
            btnCaption.html(getSelectedCaption());
        };

        opt.setSelChange = opt.setSelChange || function () {
            btnCaption.html(getSelectedCaption());
        };

        opt.renderItems = opt.renderItems || function (rs) {
            var res = '';
            $.each(rs, function (i, item) {
                res += '<li class="oitem" data-index="' + i + '" data-val="' + item[vprop] + '">' + opt.renderItemDisplay(item) + '</li>';
            });

            if (!rs.length) {
                res += '<li class="empty">(empty)</li>';
            }

            if (searchFunc) {
                res += srcinfo;
            }

            return res;
        };

        function getSelectedCaption() {
            var vals = awe.val(o.v);
            var selected = $.grep(glrs(), function (item) {
                return containsVal(K(item), vals);
            });

            return opt.renderCaption(selected);
        }

        opt.onItemClick = opt.onItemClick || function () {

            var $clickedItem = $(this);
            var val = $clickedItem.data('val');

            toggleVal(val);

            var $osearch = $odisplay.find('.osearch');

            if ($osearch.length && !isMobile()) {
                $osearch.focus();
            } else {
                $odisplay.find('.odropbtn').focus();
            }


            if (!noSelClose) {
                close();
            }

            $menuSearchTxt.val('');
            filter('', $clickedItem);

            if (noSelClose) {
                lay();
            }
        };

        opt.prerender = opt.prerender || function () {
            btn.append(btnCaption);
            $odisplay.append(btn);
        };

        // get last result + cache
        function glrs() {
            var cacheObj = cache[cacheKey];
            if (cacheObj) {
                var res = cacheObj.Items.slice(0);

                for (var i = 0; i < o.lrs.length; i++) {
                    if (cacheObj.Keys[K(o.lrs[i])] == null) {
                        res.push(o.lrs[i]);
                    }
                }

                return res;
            }

            return o.lrs;
        }

        function findvalinput(val) {
            return $valCont.find('input').filter(function () {
                return $(this).val() == val;
            });
        }

        function toggleVal(val) {
            var valinput = findvalinput(val);

            if (valinput.length) {
                if (opt.multiple) {
                    valinput.click().remove();
                } else if (opt.Combo) {
                    changeHandler();
                }
            } else {

                if (!opt.multiple) {
                    $valCont.empty();
                }

                valinput = $('<input type="' + valInputType + '" value="' + escape(val) + '" name="' + o.nm + '"/>');
                $valCont.append(valinput);
                valinput.click();
            }
        }

        function moveVal(val, afval) {
            var input = findvalinput(val);

            if (afval) {
                findvalinput(afval).after(input);
            } else {
                $valCont.prepend(input);
            }
        }

        function render() {
            opt.setSel();

            if ((searchFunc || glrs().length > 10 && !opt.noAutoSearch) && !opt.searchOutside) {
                $menuSearchCont.show();
            } else {
                $menuSearchCont.hide();
            }

            renderMenu();
        };

        function renderMenu() {
            if (!opt.noMenu) {
                $menu.html(opt.renderItems(glrs()));
            }

            $valCont.html(renderValInputs());

            markMenuSelectedItems();
        }

        function renderValInputs() {
            var res = '';
            var rawvals = awe.val(o.v);

            var vals = [];

            var lrs = glrs();
            for (var i = 0; i < rawvals.length; i++) {
                var val = escape(rawvals[i]);
                var found = 0;
                for (var j = 0; j < lrs.length; j++) {
                    if (val == lrs[j][vprop]) {
                        vals.push(lrs[j][vprop]);
                        found = 1;
                        break;
                    }
                }

                if (opt.Combo && !found) {
                    vals.push(val);
                }
            }

            if (autoSelectFirst && (!vals.length || vals.length == 1 && vals[0] == '')) {

                var allItems = glrs();
                if (allItems.length) {
                    vals = [allItems[0][vprop]];
                }
            }

            $.each(vals, function (_, value) {
                res += '<input type="' + valInputType + '" value="' + value + '" name="' + o.nm + '" checked="checked"/>';
            });

            if (!vals.length && opt.multiple) res = '<input type="checkbox" name="' + o.nm + '" />';

            return res;
        }

        function markMenuSelectedItems() {
            var val = awe.val(o.v);
            var items = glrs();
            $dropmenu.find('.oitem').each(function (i, element) {
                var checked = containsVal(items[i][vprop], val);
                if (checked) {
                    $(element).addClass('selected');
                } else {
                    $(element).removeClass('selected');
                }
            });
        }

        function filter(query, $itemToFocus) {
            var items = glrs(); //o.lrs;
            if (searchFunc) {
                var info = $itemscont.find('.oinfo');
                if (query) {
                    info.hide();
                } else {
                    info.show();
                }
            }

            var squery = query.toUpperCase();

            var count = 0;
            var f = function (s) { return s; }
            if (squery != escape(squery)) {
                f = unesc;
            }

            $dropmenu.find('.oitem').each(function (i, element) {
                if (strContainsi(f(C(items[i])), squery)) {
                    $(element).show();
                    count++;
                } else {
                    $(element).hide();
                }
            });

            autofocus($itemToFocus);

            return count;
        }

        function docClickHandler(e) {
            if ($(e.target).is('.opmodal') && e.type == 'touchstart') return;

            if (!$(e.target).closest($odisplay).length &&
                !$(e.target).closest($dropmenu).length) {
                close();
            }
        };

        function close() {
            if ($dropmenu.hasClass('open')) toggleOpen();
        }

        function toggleOpen() {
            $dropmenu.toggleClass('open');
            if ($dropmenu.hasClass('open')) {
                $odisplay.find('.oddbtn').addClass('awe-focus');
                o.maxph = 0;
                if (zIndex) {
                    zIndex = calcZIndex(zIndex, $odropdown);

                    modal.css('z-index', zIndex + 1);
                    $dropmenu.css('z-index', zIndex + 1);
                }

                $dropmenu.css('min-width', $odisplay.width() + 'px');
                $doc.on(soddDocClEv, docClickHandler);

                // render for searchfunc cache merging
                if (cache[cacheKey]) {
                    renderMenu();
                }

                $dropmenu.show();
                lay();

                if (!opt.searchOutside && !isMobile()) {
                    $menuSearchTxt.focus();
                }

                autofocus();
                dpop[o.i] = $odropdown;
            } else {
                $odisplay.find('.oddbtn').removeClass('awe-focus');
                $dropmenu.hide();
                modal.hide();
                $doc.off(soddDocClEv, docClickHandler);

                $menuSearchTxt.val('');

                filter('');
            }
        }

        function lay() {
            if ($dropmenu.hasClass('open')) {
                var oitemsc = $dropmenu.find('.oitemscont');
                var oitemscst = oitemsc.scrollTop();

                oitemsc.css('max-height', '');
                oitemsc.css('height', '');
                $dropmenu.css('width', '');

                function chkfulls(height) {
                    var winw = $win.width();
                    var winh = $win.height();
                    var limh = 300;
                    var limw = 200;
                    if (!opt.Combo && opt.afls) {
                        if (height > winh - limh - popSpace && $dropmenu.width() > winw - limw - popSpace) {
                            $dropmenu.width(winw - popSpace);
                            setHeight(winh - popSpace, winh - popSpace);

                            modal.show();
                            return 1;
                        } else {
                            modal.hide();
                        }
                    }
                }

                function setHeight(poph, maxh, valign) {
                    var rest = $dropmenu.outerHeight() - oitemsc.height();

                    if (valign == 'top') {
                        oitemsc.css('height', poph - rest);
                    } else {
                        oitemsc.css('max-height', Math.min(maxh - rest, maxDropdownHeight));
                    }
                }

                var opener = o.d.find('.odropbtn');
                if (!opener.length || opt.Combo) opener = o.d;

                layDropdownPopup2(o, $dropmenu, isFixed, null, opener, setHeight, 0, 0, chkfulls, 170, 0, maxDropdownHeight);

                oitemsc.scrollTop(oitemscst);
            }
        }

        opt.init && opt.init();

        var srctxt = o.mo.srctxt || $menuSearchTxt;

        opt.prerender();
        render();

        if (!opt.noMenu) {
            var uidialog = o.d.closest('.awe-uidialog');
            var parPop = o.d.closest('.opcont');

            var id = o.i + '-dropmenu';
            $('#' + id).remove();
            $('#' + id + '-modal').remove();
            $dropmenu.attr('id', id);
            modal.attr('id', id + '-modal');

            isFixed = 1;
            if (uidialog.length) {
                hostc = uidialog;
                zIndex = hostc.css('z-index');
            } else if (o.d.parents('.modal-dialog').length) {
                hostc = o.d.closest('.modal');
                zIndex = hostc.css('z-index');
            } else if (parPop.length) {
                zIndex = parPop.css('z-index');
                if (parPop.css('position') != 'fixed') {
                    isFixed = 0;
                }
            } else {
                isFixed = 0;
            }

            hostc.append(modal.hide());
            hostc.append($dropmenu);

            $dropmenu.on('click', '.oitem', opt.onItemClick)
                     .on('mousemove', '.oitem', function () { slistctrl.focus($(this)); });

            $odropdown.on('click', '.odropbtn', function () {
                toggleOpen();
            });

            $odisplay.on('keydown', function (e) {
                if ($dropmenu.hasClass('open')) {
                    handleMoveSelectKeys(e);
                }
            });

            $dropmenu.on('keydown', function (e) {
                handleMoveSelectKeys(e);
            });

            function loadHandler() {
                if (cache[cacheKey]) {
                    cache[cacheKey] = { Items: [], Keys: {} };
                    renderMenu();
                }
            }

            o.v.on('aweload', loadHandler);

            function handleMoveSelectKeys(e) {
                slistctrl.keyh(e);

                if (e.which == keycode.esc) {
                    $(e.target).closest('.awe-popup').data('esc', 1);
                    close();
                }
            }

            var searchTimerOn;
            var searchTimerTerm;
            var searchTimer;
            var localSearchResCount;
            var itrkc = 0;
            function getSrcTerm() {
                return srctxt.val().trim();
            }

            srctxt.on('keyup', function (e) {
                if (!contains(e.which, nonComboSearchKeys)) {

                    var term = getSrcTerm();
                    localSearchResCount = filter(term);

                    if (searchFunc && term) {
                        // cache can be already set by another odropdown
                        cache[cacheKey] = cache[cacheKey] || { Items: [], Keys: {} };

                        if (searchTimerOn) {
                            itrkc++;
                        }

                        if (!searchTimerOn) {
                            searchTimerOn = 1;
                            searchTimerTerm = term;

                            searchTimer = setInterval(function () {
                                var newTerm = getSrcTerm();

                                if (newTerm == searchTimerTerm && !itrkc) {
                                    clearInterval(searchTimer);
                                    searchTimerOn = 0;

                                    if (searchTimerTerm) {
                                        srctxt.closest('.oldngp').addClass('oldng');

                                        $.when(eval(searchFunc)(o, { term: searchTimerTerm, count: localSearchResCount, cache: cache[cacheKey] }))
                                            .always(function () {
                                                srctxt.closest('.oldngp').removeClass('oldng');
                                            })
                                            .done(function (result) {
                                                if (result.length) {
                                                    $.each(result, function (i, item) {
                                                        var cacheObj = cache[cacheKey];
                                                        var keys = cacheObj.Keys;
                                                        var items = cacheObj.Items;
                                                        if (keys[K(item)] == null) {
                                                            keys[K(item)] = items.length;
                                                            items.push(item);
                                                        } else {
                                                            items[keys[K(item)]] = item;
                                                        }
                                                    });

                                                    renderMenu();
                                                    filter(getSrcTerm());
                                                    lay();
                                                }

                                                opt.psf && opt.psf(e.which);
                                            });
                                    }
                                }

                                searchTimerTerm = newTerm;
                                itrkc = 0;

                            }, 250);
                        }
                    } else {
                        opt.psf && opt.psf(e.which);
                    }
                }
            });

            api.toggleOpen = toggleOpen;
            api.layMenu = lay;
            api.search = filter;
            api.close = close;
        }

        function changeHandler() {
            opt.setSelChange();
            markMenuSelectedItems();
            o.v.data('comboval', null);
        }

        o.v.on('change', changeHandler);

        $win.on('resize domlay', function () {
            lay();
        });
    }

    function slist(cont, opt) {
        var itemsel = opt.sel;
        var onenter = opt.enter;
        var focuscls = opt.fcls || sfocus;
        var selcls = opt.sc || 'selected';

        var itemselector = itemsel + ':visible:first';

        function focus(item) {
            remFocus();
            item.addClass(focuscls);
        }

        function remFocus() {
            cont.find('.' + focuscls).removeClass(focuscls);
        }

        function scrollToFocused() {
            var focused = cont.find('.' + focuscls);

            if (focused.length && focused.is(':visible')) {
                function y(o) {
                    return o.offset().top;
                }

                var fry = y(focused);
                var fh = focused.height();
                var conh = cont.height();
                var miny = y(cont);

                var maxy = miny + conh - fh;

                var scrcont = cont;

                var winmax = $win.height() + $doc.scrollTop() - fh;
                var winmin = $doc.scrollTop();

                if (maxy > winmax && winmax < fry) {
                    maxy = winmax;
                    scrcont = $win;
                }

                if (miny < winmin && winmin > fry) {
                    miny = winmin;
                    scrcont = $win;
                }

                var delta = fry < miny ? fry - miny : fry > maxy ? fry - maxy : 0;

                // +1 for ie and ff 
                if (delta > fh + 1 && scrcont != $win) {
                    delta += conh / 2;
                }

                scrcont.scrollTop(scrcont.scrollTop() + delta);
            }
        }

        function autofocus($itemToFocus) {
            if ($itemToFocus) {
                focus($itemToFocus);
            } else {
                var $selected = cont.find('.' + selcls + ':visible');
                if ($selected.length == 1) {
                    focus($selected);
                } else {
                    focus(cont.find(itemselector));
                }
            }

            scrollToFocused();
        }

        function handleMoveSelectKeys(e) {
            var which = e.which;

            var focused = cont.find('.' + focuscls);

            var select = function (item, f) {
                if (!focused.length) {
                    autofocus();
                }
                else if (item.length) {
                    focus(item);
                    scrollToFocused();
                } else if (f) {
                    f();
                }
            };

            if (contains(which, controlKeys)) {
                if (which == keycode.down) {
                    prevDef(e);
                    var $next = focused.nextAll(itemselector);
                    select($next, opt.botf);
                } else if (which == keycode.up) {
                    prevDef(e);
                    var $prev = focused.prevAll(itemselector);
                    select($prev, opt.topf);
                } else if (which == keycode.enter) {
                    if (onenter) {
                        onenter(e, focused);
                    }
                    else {
                        prevDef(e);
                        focused.click();
                    }
                }

                return 1;
            }

            return 0;
        }

        return {
            focus: focus,
            scrollToFocused: scrollToFocused,
            keyh: handleMoveSelectKeys,
            autofocus: autofocus,
            remf: remFocus
        };
    }

    function notif(text, time, clss) {
        var $popup = $('<div class="onotp"/>').addClass(clss);
        var $content = $('<div class="onotc"/>').html(text || 'error occured');
        var $closeBtn = $('<span class="oclose">×</span>');
        var notifCont = $('#onotifcont');

        if (!notifCont.length) {
            notifCont = $('<div class="onotcont" id="onotifcont"/>');
            notifCont.appendTo($('body'));
        }

        notifCont.prepend($popup);
        $popup.append($content);
        $popup.append($closeBtn);
        $popup.append('<div class="olbrd"/>');

        $closeBtn.on('click', close);

        $content.css('max-height', $win.height() - 50);

        if (time) {
            setTimeout(function () {
                close(1);
            }, time);
        }

        function close(fade) {
            if (fade == 1) {
                $popup.fadeOut(function () { $popup.remove(); });
            } else {
                $popup.remove();
            }
        }
    }

    function dropdownPopup(o) {
        var p = o.p; //opup properties
        var popup = p.d; //popup div
        p.i = p.i || '';
        var wrap = $('<div class="opwrap"><div class="opcont opc" tabindex="-1" data-i="' + p.i + '"></div></div>')
            .hide();
        var itmoved;
        var header;
        var api = function () { };
        var $opener;
        var openerId;

        var fls;

        var outsideClickClose = readTag(o, "Occ");
        var isDropDown = readTag(o, "Dd", !!o.api);
        var showHeader = readTag(o, "Sh");
        var toggle = readTag(o, "Tg");

        if (!isDropDown) showHeader = 1;

        var sopener = o.opener;
        var $dropdownPopup = wrap.find('.opcont').addClass(p.pc);
        p.mlh = 0;

        popup.addClass('opcontent');

        if (p.minw != null) {
            popup.css('min-width', p.minw);
        }

        if (o.rtl) {
            $dropdownPopup.addClass('awe-rtl').css('direction', 'rtl');
        }

        $dropdownPopup.append(popup);

        var modal = $('<div class="opmodal opc" tabindex="-1" data-i="' + p.i + '"></div>');
        modal.on('keyup', closeOnEsc);

        $dropdownPopup.on('keydown',
            function (e) {
                if (e.keyCode == keycode.tab) {
                    var tabbables = $dropdownPopup.find(':tabbable'),
                        first = tabbables.first(),
                        last = tabbables.last();
                    var trg = $(e.target);
                    if (trg.is(last) && !e.shiftKey) {
                        first.focus();
                        return false;
                    } else if (trg.is(first) && e.shiftKey) {
                        last.focus();
                        return false;
                    }
                }
            });

        var isFixed;
        var zIndex = minZindex;

        function layPopup(isResize, canShrink) {
            if (isResize) {
                // reset position changed by dragging popup
                itmoved = 0;
            }

            if (!p.isOpen) return;

            var winavh = $win.height() - popSpace;
            var winavw = $win.width() - popSpace;

            modal.css('z-index', zIndex);
            $dropdownPopup.css('overflow-y', 'auto');
            if (zIndex) {
                $dropdownPopup.css('z-index', zIndex);
            }

            popup.css('width', '');
            popup.css('height', '');
            popup.css('max-height', '');

            var oapi = o.api || {};

            if (oapi.rlay) {
                oapi.rlay();
            }

            var capHeight = o.f ? outerh(o.f.find('.awe-openbtn:first'), 1) : 0;

            fls = p.f;

            if (openerId && !$opener.closest(document).length) {
                $opener = $('#' + openerId);
            }

            var height = p.dh || p.h;

            if (!height) {
                height = Math.max(350, outerh($dropdownPopup));
            }

            var maxph = 0;

            var dpw = $dropdownPopup.outerWidth();
            var pow = popup.outerWidth();

            var nonpopw = dpw - pow;

            var resth = $dropdownPopup.outerHeight() - popup.outerHeight();

            if (oapi.lay) {
                height = p.dh || maxLookupDropdownHeight;
                maxph = p.dh || maxLookupDropdownHeight;
            }

            var limw = winavw;
            if (p.mw) {
                popup.css('max-width', p.mw);
                limw = p.mw;
            }

            if (p.w) {
                if (!isDropDown || p.wws) {
                    var minw = Math.min(p.w, Math.min(limw, winavw)) - nonpopw;
                    popup.css('min-width', minw);
                }
            }

            var minh = height;
            if (!isDropDown || p.hws) {
                if (p.h) {
                    minh = p.h;
                    if (height < minh) height = minh;
                    if (maxph < minh) maxph = minh;
                    popup.css('min-height', '');
                }
            }

            function chkfulls(ph) {
                var pw = $dropdownPopup.outerWidth();
                var h = $dropdownPopup.outerHeight();

                var wlim = 25, hlim = 200;

                if (p.af) {
                    wlim = 200;
                    hlim = 300;

                    h = Math.max(ph, h);
                };

                var condition = pw > winavw - wlim && h > winavh - hlim;

                if (!oapi.lay) {
                    condition = condition && h * .7 > winavh - h;
                }

                if (condition) {
                    fls = 1;
                }

                if (fls) {
                    if (oapi.lay) {
                        o.avh = winavh - resth;
                        o.nph = resth;
                    }

                    popup.css('width', winavw - nonpopw);
                    popup.css('height', winavh - resth);
                }

                if (fls || p.m) {
                    modal.show();
                } else {
                    modal.hide();
                }

                return fls;
            }

            function setmaxheight(poph, maxh, valign) {
                var avh = maxh - resth;
                popup.css('max-height', avh);

                if (oapi.lay) {
                    avh = poph - resth;

                    popup.css('height', avh);

                    o.avh = avh;
                    o.nph = resth;
                }
            }

            layDropdownPopup2(o,
                $dropdownPopup,
                isFixed,
                capHeight,
                isDropDown ? $opener : null,
                setmaxheight,
                itmoved,
                canShrink,
                chkfulls,
                minh,
                height,
                maxph);

            popup.trigger('aweresize');
        }

        function outClickClose(e) {
            var shouldClose;
            if (outsideClickClose != null) {
                shouldClose = outsideClickClose;
            } else {
                shouldClose = closePopOnOutClick || $opener && isDropDown;
            }

            if (shouldClose) {
                var trg = $(e.target);

                function lookForMe(it) {
                    var popup = it.closest('.opc');

                    var pid, mclick = 0;
                    if (it.is('.opmodal')) {
                        mclick = 1;
                    }

                    if (popup.length) {
                        pid = popup.data('i');
                    }

                    if (pid) {
                        if (pid == p.i && !mclick) return 1;

                        var popener = dpop[pid];
                        if (popener)
                            return lookForMe(popener);
                    }
                }

                if (!lookForMe(trg)) {
                    if (!trg.closest($opener).length) {
                        var $omenu = trg.closest('.omenu');
                        if ($omenu.length) {
                            if (!$omenu.data('owner').closest($dropdownPopup).length) {
                                api.close(1);
                            }
                        } else {
                            if (!trg.closest('.ui-datepicker').length) {
                                api.close(1);
                            }
                        }
                    }
                }
            } else {
                $doc.off(sddpOutClEv, outClickClose);
            }
        }

        function loadHandler() {
            layPopup();
        }

        $dropdownPopup.on('aweload awebeginload', loadHandler);

        function resizeHandler() {
            layPopup(1, 1);
        }

        $win.on('resize domlay', resizeHandler);

        api.lay = resizeHandler;

        api.open = function (e) {
            if (toggle) {
                if (p.isOpen) {
                    return api.close();
                }
            }

            if (sopener) {
                $opener = sopener;
            } else {
                if (e && e.target) {
                    $opener = $(e.target);
                    var btn = $opener.closest('button');
                    if (btn.length) $opener = btn;
                }

                if (o.f && o.f.closest('.awe-field').length) {
                    $opener = o.f;
                }

                if ($opener && !$opener.is(':visible')) {//|| p.f
                    $opener = null;
                }
            }

            var hostc = $('body');
            isFixed = 1;
            if ($opener) {
                openerId = $opener.attr('id');
                var uidialog = $opener.closest('.awe-uidialog');
                var parPop = $opener.closest('.opcont');

                if (uidialog.length) {
                    hostc = uidialog;
                    zIndex = hostc.css('z-index');
                } else if ($opener.parents('.modal-dialog').length) {
                    hostc = $opener.closest('.modal');
                    zIndex = hostc.css('z-index');
                } else if (parPop.length) {
                    zIndex = parPop.css('z-index');
                } else {
                    isFixed = 0;
                    zIndex = calcZIndex(zIndex, $opener);
                }

                header.hide();
            }

            if (!isDropDown) {
                hostc = $('body');
                isFixed = 1;
                header.show();
            }

            if (showHeader) {
                header.show();
            }

            hostc.append(modal);
            hostc.append(wrap);
            wrap.show();
            p.isOpen = 1;

            //layPopup(0, 1); // can shrink
            layPopup(0, isDropDown);

            dpop[p.i] = $opener;

            setTimeout(function () {
                $doc.on(sddpOutClEv, outClickClose);
            }, 100);

            if (!isMobile() && !p.nf) {
                setTimeout(function () {
                    var popTab = popup.find(':tabbable:first');
                    if (popTab.length) {
                        popTab.focus();
                    } else {
                        wrap.find(':tabbable:first').focus();
                    }
                },
                    10);
            }
        };

        api.close = function (nofocus) {
            wrap.hide();
            if (modal) modal.hide();
            p.isOpen = 0;
            if (p.cl) {
                p.cl();
            }

            popup.trigger('aweclose');

            if (!p.dntr) {
                wrap.remove();
                if (modal) modal.remove();
            }

            $doc.off(sddpOutClEv, outClickClose);

            if (!nofocus) {
                if ($opener && $opener.length) {
                    (o.ctf || $opener).focus();
                }
            }
        };

        api.destroy = function () {
            api.close();
            wrap.remove();
            if (modal) modal.remove();
            $win.off('resize domlay', resizeHandler);
        };

        popup.data('api', api);

        header = $('<div class="opheader"><div class="optitle">' +
            (p.t || '&nbsp;') +
            '</div><span class="oclose">×</span></div>');
        $dropdownPopup.prepend(header);
        header.find('.oclose').click(api.close);

        function getDragPopup() {
            itmoved = 1;
            return $dropdownPopup;
        }

        dragAndDrop({
            from: header,
            ch: getDragPopup,
            kdh: 1,
            cancel: function () { return fls; }
        });

        addFooter(p.btns, $dropdownPopup, popup, 'opbtns');

        function closeOnEsc(e) {
            if (e.which == keycode.esc) {
                var dtpf = $(e.target).closest('.awe-datepicker-field');
                if (dtpf.length && dtpf.find('.awe-val').datepicker('widget').is(':visible')) {

                } else {
                    if (!popup.data('esc')) {
                        api.close();
                    }
                }

                popup.data('esc', null);
            }
        }

        $dropdownPopup.on('keyup', closeOnEsc);

        return wrap;
    }

    function uiPopup(o) {
        var soption = "option";
        var pp = o.p;
        var popup = pp.d;

        pp.mlh = 0;

        var autoSize = awe.autoSize;
        var fullscreen = pp.f;
        var draggable = true;

        if (!pp.r) pp.r = false;

        if (fullscreen) {
            pp.r = false;
            draggable = false;
            pp.m = true;
        }

        pp.uiw = pp.w;
        if (!pp.uiw) pp.uiw = 700;

        popup.dialog({
            draggable: draggable,
            width: pp.uiw,
            height: pp.h,
            modal: pp.m,
            resizable: pp.r,
            buttons: pp.btns,
            autoOpen: false,
            title: pp.t,
            resizeStop: function () {
                pp.uiw = popup.dialog(soption, 'width');
                pp.h = popup.dialog(soption, 'height');
                pp.p = popup.dialog(soption, 'position');
            },
            dragStop: function () {
                pp.p = popup.dialog(soption, 'position');
            }
        });

        var dialogClass = "awe-uidialog awe-popupw";
        if (o.rtl) {
            dialogClass += ' awe-rtl';
        }

        if (pp.pc) dialogClass = dialogClass + " " + pp.pc;
        popup.dialog(soption, { dialogClass: dialogClass });

        var autoResize = function () { };
        if (fullscreen || autoSize) {
            //autosize
            autoResize = function (e) {
                if (popup.is(':visible'))
                    if (!e || e.target == window || e.target == document) {

                        var wh = $win.height();
                        var ww = $win.width();

                        var sw = pp.uiw > ww - 10 || fullscreen ? ww - 10 : pp.uiw;
                        var sh = pp.h > wh - 5 || fullscreen ? wh - 20 : pp.h;
                        var opt = {
                            height: sh,
                            width: sw
                        };

                        //on ie9 it goes off screen on zoom when setting position
                        if (!fullscreen && pp.p) opt.position = pp.p;
                        popup.dialog(soption, opt).trigger('aweresize');
                    }
            };

            $win.on('resize', autoResize);
            autoResize();
            popup.on('change', autoResize);
        }//end if fullscreen or autoSize 

        popup.on('dialogclose', function () {
            popup.trigger('aweclose');

            pp.isOpen = 0;
            if (pp.cl) {
                pp.cl.call(o);
            }

            if (!pp.dntr) {
                if (autoSize || fullscreen) {
                    $win.off('resize', autoResize);
                }

                popup.find('*').remove();
                popup.remove();
            }


        }).on('dialogresize', function () {
            popup.trigger('aweresize');
        });

        popup.on('aweload awebeginload', function () {
            o.avh = 0;
            popup.trigger('aweresize');
        });

        var api = function () { };
        api.open = function () {
            popup.dialog('open');
            pp.isOpen = 1;
            popup.trigger('aweopen');
            autoResize();
        };
        api.close = function () {
            popup.dialog('close');
        };
        api.destroy = function () {
            api.close();
            $win.off('resize', autoResize);
            popup.remove();
        };

        popup.data('api', api);
    }

    function bootstrapPopup(o) {
        var p = o.p; //popup properties
        var popup = p.d; //popup div
        var modal = $('<div class="modal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">' +
            '<div class="modal-dialog"><div class="modal-content awe-popupw"><div class="modal-header"><button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
            '<h4 class="modal-title"></h4></div></div></div></div>');

        var hasFooter = p.btns && p.btns.length;

        //minimum height of the lookup/multilookup content
        p.mlh = !p.f ? 250 : 0;

        if (!p.t) {
            p.t = "&nbsp;"; //put one space when no title
        }

        popup.addClass("modal-body");
        popup.css('overflow', 'auto');

        modal.find('.modal-content').append(popup);
        modal.find('.modal-title').html(p.t);
        popup.show();

        //use to resize the popup when fullscreen
        function autoResize() {
            var h = $win.height() - 120;
            if (hasFooter) h -= 90;
            if (h < 400) h = 400;
            popup.height(h);
            popup.trigger('aweresize');
        }

        var api = function () { };
        api.open = function () {
            modal.appendTo($('body')); //appendTo each time to prevent modal to show under current top modal
            modal.modal('show');
            p.isOpen = 1;
            popup.trigger('aweopen');
            if (p.f) autoResize();
        };
        api.close = function () {
            modal.modal('hide');

            p.isOpen = 0;
            if (p.cl) {
                p.cl();
            }
            if (!p.dntr) {
                popup.find('*').remove();
                popup.closest('.modal').remove();
            }
        };

        api.destroy = function () {
            api.close();
            $win.off('resize', autoResize);
            popup.closest('.modal').remove();
        };

        popup.data('api', api);

        modal.on('hidden.bs.modal', function () {
            popup.trigger('aweclose');
        });

        $('body').append(modal);

        //fullscreen
        if (p.f) {
            modal.find('.modal-dialog').css('width', 'auto').css('margin', '10px');
            $win.on('resize', autoResize);
        }

        //add buttons if any
        if (hasFooter) {
            var footer = $('<div class="modal-footer"></div>');
            modal.find('.modal-footer').remove();
            modal.find('.modal-content').append(footer);
            $.each(p.btns, function (i, e) {
                var btn = $('<button type="button" class="btn btn-default">' + e.text + '</button>');
                btn.click(function () { e.click.call(popup); });
                footer.append(btn);
            });
        }
    }

    function addFooter(btns, cont, popup, fclass) {
        // add btns if any
        if (btns && btns.length) {
            var btnslen = btns.length;

            var footer = $('<div/>').addClass(fclass);

            if (reverseDefaultBtns && btnslen > 1) {
                if (btns[btnslen - 1].c) {
                    var cbtn = btns.pop();
                    var kbtn = btns.pop();
                    btns.push(cbtn);
                    btns.push(kbtn);
                }
            }

            $.each(btns, function (i, el) {
                var cls = !el.k ? 'awe-sbtn' : 'awe-okbtn';
                var btn = $('<button type="button">' + el.text + '</button>');
                if (el.tag) {
                    var tag = el.tag;
                    if (tag.K)
                        $.each(tag.K, function (indx, key) {
                            btn.attr(key, tag.V[indx]);
                        });
                }

                btn.addClass('awe-btn ' + cls + ' inl-btn');
                btn.click(function () { el.click.call(popup); });
                footer.append(btn);
            });

            cont.append(footer);
        }
    }

    function inlinePopup(o) {
        var p = o.p; //popup properties
        var popup = p.d; //popup div
        var wrap = $('<div class="oinlpop awe-popupw"></div>').hide();

        //minimum height of the lookup/multilookup content
        p.mlh = 250;

        wrap.append(popup);

        //decide where to attach the inline popup
        //tag and tags are set using .Tag(object) .Tags(string)
        if (o.tag && o.tag.target) {
            $('#' + o.tag.target).append(wrap);
        } else if (o.tag && o.tag.cont) {// cont used in grid nesting
            o.tag.cont.prepend(wrap);
        } else if (o.tags) {
            $('#' + o.tags).append(wrap);
        } else if (o.f) { //component field
            o.f.after(wrap);
        } else {
            $('body').prepend(wrap);
        }

        var api = function () { };
        api.open = function () {
            wrap.show();
            p.isOpen = 1;
            popup.trigger('aweopen');
        };
        api.close = function () {
            wrap.hide();
            p.isOpen = 0;
            if (p.cl) {
                p.cl();
            }
            popup.trigger('aweclose');
            if (!p.dntr) {
                wrap.remove();
            }
        };
        api.destroy = function () {
            api.close();
            wrap.remove();
        };

        popup.data('api', api);

        var title = $('<div class="inl-title"></div>');
        var closeBtn = $('<button type="button" class="awe-btn">&nbsp;X&nbsp;</button>').click(api.close);
        title.append(closeBtn).append("<span class='inl-txt'>" + p.t + "</span>");

        if (readTag(o, "Sh", 1))
            wrap.prepend(title);

        addFooter(p.btns, wrap, popup);

        return wrap;
    }

    function gridPageInfo(o) {
        var $grid = o.v;
        var $pageInfo = $('<div class="gridPageInfo"></div>');
        var delta = 0;
        var $footer = $grid.find('.awe-footer');
        if (!$footer.length) return;

        $grid.on('awerowch', function (e, data) {
            delta += data;
            render();
        });

        $grid.find('.awe-footer').append($pageInfo);

        $grid.on('aweload', function (e) {
            if (!$(e.target).is($grid)) return;
            delta = 0;
            render();
        });

        function render() {
			var lrs = dto($grid).lrs;
			var pageSize = lrs.ps;
			if ($grid[0].id == "OriginalTextGrid")
			{
				document.cookie = "PageSize=" + pageSize;
			}
			//document.cookie = "PageSize=" + pageSize;
            var itemsCount = lrs.ic + delta;

            var first = pageSize * (lrs.p - 1) + 1;
            var last = lrs.pgn ? first + pageSize - 1 + delta : itemsCount;
            if (last > itemsCount) last = itemsCount;
            if (!itemsCount || !last) first = 0;

            $pageInfo.html(first + ' - ' + (last) + ' ' + format(cd().GridInfo, [itemsCount]));
        }
    }
	function getCookie(name) {
		var v = document.cookie.match('(^|;) ?' + name + '=([^;]*)(;|$)');
		return v ? v[2] : null;
	}
    function gridPageSize(o) {
        if (isMobile()) return;

        var items = [5, 10, 20, 50];
        function addIfLacks(ni) {
            if (!contains(ni, items)) {
                items.push(ni);
                items.sort(function (a, b) {
                    return a - b;
                });
            }
        }

        var $grid = o.v;

        var $footer = $grid.find('.awe-footer');
        if (!$footer.length) return;
		
		if ($grid[0].id =="OriginalTextGrid")
		{
			$grid.find('.awe-footer').append('<div class="awe-ajaxradiolist-field gridPageSize" ><input id="' + o.i + 'PageSize" class="awe-val" type="hidden" value="' + getCookie("PageSize") + '" /><div class="awe-display"></div></div>');
		}
		else
		{
			$grid.find('.awe-footer').append('<div class="awe-ajaxradiolist-field gridPageSize" ><input id="' + o.i + 'PageSize" class="awe-val" type="hidden" value="' + o.ps + '" /><div class="awe-display"></div></div>');

		}
		addIfLacks(o.ps); gridPageSize

        var psi = o.i + 'PageSize';

        function setPages() {
            return $.map(items, function (val) {
                return { c: val, k: val };
            });
        }

        awe.radioList({ i: psi, nm: psi, df: setPages, l: 1, md: awem.odropdown, tag: { InLabel: "page size: " } });

        o.data.keys.push("pageSize");
        o.data.vals.push(psi);
        o.data.l.push(1);
    }

    function gridInfScroll(o) {
        var $grid = o.v;
        var $content = $grid.find('.awe-content');
        var $tw = $content.children().first();

        function adjustMargin() {
            var diff = (Math.max(($content.height() - $tw.height()) + 20, 20));

            $tw.css('margin-bottom', diff + 'px');
        }

        adjustMargin();

        $content.on('scroll', function () {
            var res = o.lrs;

            var st = $content.scrollTop();
            var sh = $content.prop('scrollHeight') - $content.height();
            var lstv = $content.data('lastsv');

            adjustMargin();

            if (st > sh) {
                st -= awe.scrollw();
            } // deduct horizontal scrollbar height

            if ((lstv + 1) == sh && st == sh) {
                if (res.p < res.pc) {
                    $.when(nextPage()).done(function () {
                        $content.scrollTop(1);
                        st = 1;
                    });
                }
            }
            else if (st == sh) {
                st--;
                $content.scrollTop(st);
            }
            else if ((lstv - 1) == 0 && st == 0) {
                if (res.p > 1) {
                    $.when(prevPage()).done(function () {
                        st = sh - 1;
                        $content.scrollTop(st);
                    });
                }
            } else if (st == 0) {
                st++;
                $content.scrollTop(st);
            }

            $content.data('lastsv', st);

            function nextPage() {
                return dapi($grid).load({ oparams: { page: res.p + 1 } });
            }

            function prevPage() {
                return dapi($grid).load({ oparams: { page: res.p - 1 } });
            }
        });
    }

    function isMobileOrTablet() {
        return false;
    }

    var clientDict = {
        GridInfo: "of {0} items",
        Select: 'please select',
        SearchForRes: 'search for more results',
        Searchp: 'search...',
        NoRecFound: 'no records found',
        Months: [
            "January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December"
        ],
        Days: ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"]
    };

    function gridLoading(o, opt) {
        opt = opt || {};
        opt.lhtm = opt.lhtm || '<div class="spinner"><div class="dot1"></div><div class="dot2"></div></div>';
        var ctm = opt.ctm || 40;

        var $grid = o.v;
        var $mcontent = $grid.find('.awe-content');

        $grid.on('awebeginload', function (e) {
            if ($(e.target).is($grid)) {
                $grid.find('.ogempt').remove();
                var $spin = $('<div class="spinCont">' + opt.lhtm + '</div>').hide();
                $spin.height($mcontent.height());
                $mcontent.prepend($spin);
                $spin.children().first().css('margin-top', ($mcontent.height() / 2 - ctm) + 'px');
                $spin.fadeIn();
            }
        }).on('aweload', function (e, res) {
            if ($(e.target).is($grid)) {
                $mcontent.find('.spinCont:first').fadeOut().remove();
                $mcontent.find('.ogempt').remove();
                if (!res.ic) {
                    $mcontent.prepend($('<div class="ogempt">' + cd().NoRecFound + '</div>')
                        .css('margin-top', Math.max(($mcontent.height() / 2) - 90, 10) + 'px'));
                }
            }
        });
    }

    function gridMovRows(opt) {
        return function (o) {
            var $grid = o.v;
            var placeh;
            var $fromCont = $grid.find('.awe-content');
            var hovered;
            var drgObj;
            var rowmodel;
            var prevIndx;
            var ogrow;
            var hi, di;
            var grids = [o.v.attr('id')];
            var currhovering;

            if (opt && opt.G) {
                grids = opt.G;
            }

            function getRow($c) {
                return dapi($c.closest('.awe-grid')).renderRow(rowmodel);
            }

            function wrap(clone, dragObj) {
                placeh = ogrow = currhovering = null;
                prevIndx = dragObj.index();
                drgObj = dragObj;
                rowmodel = drgObj.data('model');

                var res = $('<div/>').append($('<table/>').append(dragObj.closest('table').find('colgroup').clone()).append(clone))
                                     .prop('class', $grid.prop('class'));

                return res;
            }

            function hoverFunc($c) {
                return function (dragObj, x, y) {

                    if (placeh) {
                        placeh.detach();
                    }

                    if (currhovering != $c) {
                        currhovering = $c;
                        placeh = getRow($c).addClass('awe-changing');
                        ogrow = placeh.clone();
                    }

                    drgObj.show();
                    di = drgObj.index();

                    if (!$c.is($fromCont)) {
                        $c.find('.awe-tbody').prepend(ogrow.show());
                        di = 0;
                    }

                    hovered = null;
                    $c.find('.awe-row').each(function (i, el) {
                        if ($(el).offset().top + $(el).height() > y) {
                            hovered = $(el);
                            return false;
                        }
                    });

                    if (hovered == null) {
                        $c.find('.awe-tbody').append(placeh);
                    } else {
                        hi = hovered.index();
                        if (di > hi) {
                            hovered.before(placeh);
                        } else {
                            hovered.after(placeh);
                        }
                    }

                    drgObj.hide();
                    ogrow.hide();
                }
            }

            function dropFunc($c) {
                return function (dragObj) {
                    var newRow = getRow($c);

                    if (hovered == null) {
                        $c.find('.awe-tbody').append(newRow);
                    } else if (di > hi)
                        hovered.before(newRow);
                    else {
                        hovered.after(newRow);
                    }

                    dragObj.remove();
                    var $toGrid = $c.closest('.awe-grid');
                    $toGrid.trigger('awerowmove', [newRow, prevIndx, $fromCont]);

                    if (!$toGrid.is($grid)) {
                        movedGridRow($grid, $toGrid);
                    }
                };
            }

            // called on move when switching containers and end
            function resetHover() {
                if (placeh) {
                    placeh.detach();
                    ogrow.detach();
                }
            }

            function end() {
                drgObj.show();
            }

            var to = [];
            var scroll = [];

            $.each(grids, function (i, val) {
                var $grid = $('#' + val).find('.awe-content');
                to.push({ c: $grid, drop: dropFunc($grid), hover: hoverFunc($grid) });
                scroll.push({ c: $grid, y: 1 });
            });

            scroll.push({ c: $win, y: 1 });

            dragAndDrop({
                from: $fromCont,
                sel: '.awe-row',
                to: to,
                wrap: wrap,
                reshov: resetHover,
                scroll: scroll,
                cancel: function (isTouch, coords) {
                    var rleft = coords.pageX - $fromCont.offset().left;
                    return isTouch && (($fromCont.width() - rleft < 100 || rleft < 100));
                },
                end: end
            });
        };
    }

    function gridInlineEdit(createUrl, editUrl, oneRow, reloadOnSave, rowClick) {
        return function (o) {
            var $grid = o.v;
            var api = dapi($grid);
            var newic = 1;
            var activeRow;

            function oneRowCheck(action) {
                if (oneRow) {
                    var otherRow = $grid.find('.ginlrow').first();
                    if (otherRow.length && otherRow.data('state') != 3) {
                        checkAndPreventActionUntilSave(otherRow, action);
                        return 1;
                    }
                }
            }

            api.inlineCreate = function inlineCreate(newModel) {
                if (oneRowCheck(function () { inlineCreate(newModel); })) {
                    return;
                }

                newModel = newModel || {};
                var $newRow = api.renderRow(newModel);
                $newRow.addClass('newrow');
                $grid.find('.awe-content .awe-tbody').prepend($newRow);
                inlineEdit($newRow);

            };

            api.inlineEdit = inlineEdit;
            api.inlineCancel = function ($row, focus) { cancelRow($row, focus); };

            $grid.on('click', '.gsavebtn', function () {
                save($(this).closest('.awe-row'), 1);
            })
            .on('click', '.gcancelbtn', function () {
                api.inlineCancel($(this).closest('.awe-row'), 1);
            })
            .on('click', '.geditbtn', function () {
                var $this = $(this);
                var $row = $this.closest('.awe-row');
                inlineEdit($row);
            });

            function inlineEdit($row, td) {
                if (oneRowCheck(function () { inlineEdit($row, td); })) {
                    return;
                }
                
                activeRow = $row;

                $row.addClass('ginlrow awe-nonselect');
                $row.find('.awe-btn').hide();
                $row.find('.gcancelbtn,.gsavebtn').show();

                var $colgroup = $row.closest('.awe-table').find('colgroup');
                var model = $row.data('model');

                var hidden = '';

                var prefix = o.i + (model[o.k] || '');

                if ($row.hasClass('newrow')) {
                    prefix += 'new' + (newic++);
                }

                var needLoading = [];

                $.each(o.columns, function (_, column) {
                    var tdi = $colgroup.find('col[data-i="' + column.i + '"]').index();
                    var tag = column.Tag;
                    if (tag) {
                        function getVal(prop) {
                            var val = model[o.lrs.a ? toLowerFirst(prop) : prop];

                            val = awe.rgv(val);
                            val = val instanceof Array ? JSON.stringify(val) : val;
                            return val;
                        }

                        function parseFormat(format, value) {

                            var boolVal = value ? 'checked' : '';
                            format = format.split('#Value').join(value)
                                .split('#Prefix').join(prefix)
                                .split('#ValChecked').join(boolVal);

                            for (var key in model) {
                                var sval = getVal(key);

                                format = format.split(".(" + key + ")").join(sval)
                                               .split(".(" + toUpperFirst(key) + ")").join(sval)
                                               .split("." + key).join(sval)
                                               .split("." + toUpperFirst(key)).join(sval);
                            }

                            format = format.replace(/\.\(\w+\)/g, "");
                            return format;
                        }

                        var inlelms = tag.Format;

                        if (tag.FormatFunc) {
                            inlelms = eval(tag.FormatFunc)(model, tag.Fpar);
                        }

                        if (inlelms) {
                            for (var j = 0; j < inlelms.length; j++) {
                                var el = inlelms[j];
                                var val = getVal(el.ValProp);

                                var hformat = parseFormat(el.Format, val);

                                if (column.Hid) {
                                    hidden += hformat;
                                } else {
                                    var validstr = el.ModelProp && hformat.indexOf("awe-gvalidmsg") == -1 ? '<div class="awe-gvalidmsg ' + el.ModelProp + '"></div>' : '';
                                    var cellcont = '<div class="oinlc"><div class="oinle">' + hformat + '</div>' + validstr + '</div>';
                                    addHidden($row.children().eq(tdi).empty().append(cellcont));
                                }

                                if (el.JsFormat) {
                                    needLoading.push(parseFormat(el.JsFormat, val));
                                }
                            }
                        }
                    }
                });

                if (hidden) {
                    addHidden($row.children().last());
                }

                function addHidden(cont) {
                    if (hidden) {
                        cont.append($('<div>' + hidden + '</div>').hide());
                        hidden = '';
                    }
                }

                if (needLoading.length) {
                    for (var i = 0; i < needLoading.length; i++) {
                        eval(needLoading[i]);
                    }
                }

                var ins = $row.find(':input').serializeArray();
                $row.data('ins', ins);
                $row.trigger(saweinledit);
                var fsel = ':tabbable:not(.hasDatepicker):first';
                setTimeout(function () {
                    if (td && td.find(fsel).length) {
                        td.find(fsel).focus();
                    } else {
                        $row.find(fsel).focus();
                    }
                });

                if (rowClick) {
                    setTimeout(function () {
                        regOutClick($row);
                    });
                }
            }

            function save($row, isClick) {
                if ($row.data('slock')) {
                    return;
                }

                $row.data('slock', 1);
                $row.data('state', 2);

                var url = $row.hasClass('newrow') ? createUrl : editUrl;
                var sdata = $row.find(':input').serializeArray();

                var diff = 0;
                var ins = $row.data('ins');
                if (ins.length != sdata.length) {
                    diff = 1;
                } else {
                    for (var i = 0; i < ins.length; i++) {
                        if (ins[i].name != sdata[i].name || ins[i].value != sdata[i].value) {
                            diff = 1;
                            break;
                        }
                    }
                }

                if (!diff && !$row.hasClass('newrow')) {
                    cancelRow($row, isClick);
                    return 1;
                }

                o.lrso = 1;
                $.post(url, sdata.concat(awe.params(o, 1)), function (rdata) {
                    $row.find('.awe-gvalidmsg').empty();
                    var errors = rdata.e;
                    if (errors) {
                        $row.data('state', 1);
                        $row.trigger(saweinlinv);

                        for (var k in errors) {
                            var msg = '';
                            for (var i = 0; i < errors[k].length; i++) {
                                msg += '<div class="field-validation-error">' + errors[k][i] + '</div>';
                            }

                            if (!k || !$row.find('.' + k).length) {
                                $grid.find('.awe-gvalidmsg:last').append(msg);
                            } else {
                                $row.find('.' + k).html(msg);
                            }
                        }
                    } else {
                        $row.data('state', 3);
                        $row.trigger(saweinlsave, { r: rdata });

                        if (reloadOnSave) {
                            api.load();
                        } else if (rdata.Item) {
                            var $nrow = api.renderRow(rdata.Item);
                            $row.after($nrow).remove();
                            $nrow.addClass("awe-changing").removeClass("awe-changing", 1000).find('.geditbtn').focus();
                            closeActiveRow($row);
                        } else {
                            var item = $row.data('model');
                            var key = o.k;
                            $.when(api.update(item[key])).done(function () {
                                closeActiveRow($row);
                                if (isClick) focusEditBtn(item, key);
                            });
                        }
                    }
                }).fail(function (p1, p2, p3) {
                    $row.data('state', 0);
                    awe.err(o, p1, p2, p3);
                }).always(function () {
                    $row.data('slock', 0);
                });
            }

            function cancelRow($row, isClick) {
                $row.trigger(saweinlcancel);
                if ($row.hasClass('newrow')) {
                    $row.remove();
                } else {
                    var item = $row.data('model');
                    $row.after(api.renderRow(item)).remove();
                    if (isClick) {
                        focusEditBtn(item, o.k);
                    }
                }

                closeActiveRow($row);
            }

            function closeActiveRow($row) {
                if (activeRow && (activeRow.is($row) || !activeRow.closest(document).length)) {
                    activeRow = null;
                    if ($grid.find('.ginlrow').length) {
                        activeRow = $grid.find('.ginlrow').first();
                    }
                }
            }

            if (rowClick) {
                $grid.on('click focusin',
                    '.awe-row:not(.oinlar) td',
                    function (e) {
                        var td = $(this);
                        var row = td.closest('.awe-row');

                        if ((e.type == 'focusin' || $(e.target).closest('button').length) && !row.hasClass('ginlrow') || !row.closest(document).length) {
                            return;
                        }

                        activeRow = row;
                        if (!row.hasClass('ginlrow')) {
                            inlineEdit(row, td);
                        } else {
                            regOutClick(row);
                        }
                    });
            }

            function regOutClick(row) {
                function outclick(e) {
                    function popupLookFor(src, pivot) {
                        if (pivot.closest(src).length) {
                            return 1;
                        }

                        var popup = pivot.closest('.opc');

                        if (popup.length) {
                            var pid = popup.data('i');
                            var popener = dpop[pid];
                            if (popener) {
                                return popupLookFor(src, popener);
                            }
                        }
                    }

                    var trg = $(e.target);
                    if (!popupLookFor(row, trg) && !trg.closest('.ui-datepicker').length && trg.closest(document).length) {
                        save(row);
                        deregOutclick();
                        row.removeClass('oinlar');
                    }
                }

                var ev = saweinlsave + ' ' + saweinlcancel;

                function deregOutclick() {
                    $(document).off('click', outclick);
                    row.off('keydown', onKeyDown);
                    row.off(ev, deregOutclick);
                }

                function onKeyDown(e) {
                    if (e.which == keycode.tab) {
                        var tabls = row.find(':tabbable');
                        if ($(e.target).is(tabls.last()) && !e.shiftKey) {
                            prevDef(e);
                            row.next().find('td:first').click();
                        }
                        else if ($(e.target).is(tabls.first()) && e.shiftKey) {
                            prevDef(e);
                            row.prev().find('td:first').click();
                        }
                    }
                }

                if (row.hasClass('oinlar')) {
                    return;
                }

                row.addClass('oinlar');
                row.data('state', 0);

                $(document).on('click', outclick);

                row.on('keydown', onKeyDown);
                row.on(ev, deregOutclick);
            }

            function checkAndPreventActionUntilSave(row, action) {
                var state = row.data('state');
                if (state == 3) {
                    action();
                } else {
                    if (!state) {
                        if (save(row)) {
                            action();
                            return;
                        }
                    }

                    if (!state || state == 2) {
                        var ev = saweinlsave + ' ' + saweinlinv;
                        function onSaveFin(e) {
                            row.off(ev, onSaveFin);
                            if (e.type == saweinlsave) {
                                action();
                            } else {
                                scrollTo(row, $grid.find('.awe-content'));
                            }
                        }

                        row.on(ev, onSaveFin);
                    } else if (row.data('state') == 1) {
                        scrollTo(row, $grid.find('.awe-content'));
                    }
                }
            }

            function onGridBeforeLoad(e, aobj) {
                if ($(e.target).is($grid)) {
                    var ierows = $grid.find('.ginlrow').length;

                    if (ierows) {
                        var loadFunc = aobj.load;
                        aobj.load = null;

                        if (ierows == 1) {
                            checkAndPreventActionUntilSave(activeRow, loadFunc);
                        }
                    }
                }
            }

            $grid.on('awebeforeload', onGridBeforeLoad);

            function focusEditBtn(item, key) {
                var row = api.select(item[key])[0];
                row.find('.geditbtn').focus();
            }
        };
    }

    var regHandlers = { ca: {}, mp: {} }; // column autohide; minipager

    function gridColAutohide(o) {
        function isColumnHidden(column) {
            return !o.sgc && column.Gd || column.Hid;
        }

        function autohide(col) {
            return col.Tag && col.Tag.Autohide || 0;
        }

        var $grid = o.v;

        function autohideColumns(isInit) {
            if (!isInit && (!o.lrs || o.ldg)) return;

            var changes = 0;
            var avw = $grid.find('.awe-hcon').width() || $grid.find('.awe-content').width() - awe.scrollw();
            var eo = dto($grid);

            if (avw < 0) return changes;

            if (!eo) {
                removeRegHandle(o, 'ca');
                return changes;
            }

            var ahcols = $.grep(eo.columns, function (col) {
                return autohide(col);
            }).sort(function (a, b) { return autohide(b) - autohide(a); }).reverse();

            // unhide autohidden
            $.each(ahcols, function (_, col) {
                if (col.Hid == 2) {
                    col.Hid = 0;
                    changes++;
                }
            });

            var contentWidth = o.api.conw();
            if (avw < contentWidth) {
                $.each(ahcols, function (_, col) {
                    if (!isColumnHidden(col)) {
                        col.Hid = 2;
                        changes--;
                        contentWidth -= col.W || col.Mw;
                        if (contentWidth <= avw) return false;
                    }
                });
            }

            if (changes) {
                if (!isInit) {
                    dapi($grid).render();
                }

                $grid.trigger(sawecolschange);
            }

            return changes;
        }

        $grid.on(saweinit, function (e) {
            if ($(e.target).is($grid)) {
                autohideColumns(1);
            }
        });

        removeRegHandle(o, 'ca');

        function resizeHandler() {
            autohideColumns();
        }

        $win.on('aweresize resize domlay', resizeHandler);

        regHandlers['ca'][o.i] = {
            h: resizeHandler,
            e: 'aweresize resize domlay'
        };
    }

    function removeRegHandle(o, k) {
        var reghandle = regHandlers[k][o.i];
        if (reghandle) {
            $win.off(reghandle.e, reghandle.h);
        }
    }

    function gridColSel(o) {
        var $grid = o.v;
        var scid = o.i + 'ColSel';

        $grid.find('.awe-footer').append('<div class="awe-ajaxcheckboxlist-field gridColSel" ><input id="' + scid + '" class="awe-val awe-array" type="hidden" /><div class="awe-display"></div></div>');

        function getColumnsDataFunc() {
            var result = [];
            $.each(o.columns, function (i, col) {
                var name = col.H || col.P || "col" + (i + 1);
                if (!(col.Tag && col.Tag.Nohide))
                    result.push({ k: i, c: name });
            });

            return result;
        }

        awe.checkboxList({ i: scid, nm: scid, df: getColumnsDataFunc, l: 0, md: awem.multiselb, tag: { InLabel: "<i class='o'></i><i class='o'></i><i class='o'></i>", NoSelClose: 1 } });
        var colSel = $('#' + scid);

        function setItems() {
            var selColIndx = []; // value
            $.each(o.columns,
                function (i, col) {
                    if (!col.Hid && !(col.Tag && col.Tag.Nohide)) selColIndx.push(i);
                });

            colSel.val(JSON.stringify(selColIndx));
            dapi(colSel).load();
        }

        $grid.on(saweinit + ' ' + sawecolschange + ' aweload', function (e, d) {
            if ($(e.target).is($grid) && !(d && d.c)) {
                setItems();
            }
        });

        colSel.on('change', function () {
            var colIndxs = $.parseJSON($(this).val() || "[]");
            $.each(o.columns, function (i, col) {
                if ($.inArray(i.toString(), colIndxs) == -1 && !(col.Tag && col.Tag.Nohide)) {
                    if (!col.Hid) {
                        col.Hid = 1; //hide column
                        if (col.Gd) {
                            //remove grouped when hiding column
                            col.Gd = 0;
                            o.lrso = 1;
                        }
                    }
                } else {
                    col.Hid = 0;
                }
            });

            var api = dapi($grid);
            api.persist();
            api.render();
            $grid.trigger(sawecolschange, { c: 1 });
        });
    }

    function gridMiniPager(o) {
        return gridAutoMiniPager(o, 1);
    }

    function gridAutoMiniPager(oo, useMiniPager) {
        var $grid = oo.v;
        var $footer = $grid.find('.awe-footer');
        if (!$footer.length) return;
        var api = dapi($grid);
        var original = api.buildPager;
        var miniPager = function (o) {
            var pageCount = o.lrs.pc;
            var page = o.lrs.p || 1;
            if (o.lrs.pgn) {
                var result = '';

                result += renderButton(1, icon('o-arw double left'), 0, page < 2, 'ba');
                result += renderButton(page - 1, icon('o-arw left'), 0, page < 2, 'b');

                result += renderButton(page, page, 1, 0, 'c');

                result += renderButton(page + 1, icon('o-arw right'), 0, pageCount <= page, 'f');
                result += renderButton(pageCount, icon('o-arw double right'), 0, pageCount <= page, 'fa');

                var $pager = $grid.find('.awe-pager');
                $pager.html(result);

                $pager.find('.awe-btn').on('click', function () {
                    var p = $(this).data('p');
                    var act = $(this).data('act');

                    $.when(dapi($grid).load({ start: function () { o.pg = parseInt(p); } })).done(function () {
                        var fbtn = $pager.find('[data-act=' + act + "]");
                        if (fbtn.is(':disabled')) {
                            $pager.find('.awe-btn:not(:disabled)').first().focus();
                        } else {
                            fbtn.focus();
                        }
                    });
                });

                setTimeout(function () {
                    api.lay();
                }, 10);
            }
        };

        decideSwitch();

        if (!useMiniPager) {
            removeRegHandle(oo, 'mp');

            $win.on('resize domlay', tryminipager);

            regHandlers['mp'][oo.i] = {
                h: tryminipager,
                e: 'resize domlay'
            };

            function tryminipager() {
                if (decideSwitch()) {
                    api.buildPager(oo);
                };
            }
        }

        function decideSwitch() {
            var cval = api.buildPager;
            var nval = useMiniPager || $win.width() < 1000 ? miniPager : original;
            api.buildPager = nval;
            return nval != cval;
        }

        function icon(icls) {
            return '<span class="' + icls + '" aria-hidden="true"></span>';
        }

        function renderButton(page, caption, selected, disabled, act) {
            var clss = "awe-btn mpbtn ";
            if (selected) clss += "awe-selected ";
            if (disabled) clss += "awe-disabled ";
            var dis = disabled ? "disabled" : '';
            return '<button type="button" class="' + clss + '" data-p="' + page + '" data-act="' + act + '" ' + dis + '>' + caption + '</button>';
        }
    }

    function gridkeynav(o) {
        var grid = o.v;
        var api = dapi(grid);

        grid.addClass('keynav');
        grid.attr('tabindex', '0');
        var sctrl = slist(grid.find('.awe-content'), { sel: '.awe-row', fcls: sfocus, sc: 'n', topf: topFunc, botf: botFunc, enter: onenter });

        function topFunc() {
            chpage(-1);
        }

        function botFunc() {
            chpage(1);
        }

        function onenter(e, focused) {
            if (focused.length) {
                prevDef(e);
                var shift = e.shiftKey;

                if (!shift && focused.find('.awe-movebtn').length) {

                    var next = pickAvEl([focused.next(), focused.prev()]);

                    focused.removeClass(sfocus);
                    focused.find('.awe-movebtn').click();

                    if (next) {
                        sctrl.focus(next);
                    }

                } else {
                    focused.click();
                }

                if (shift) {
                    if (grid.closest('.awe-lookup-popup').length) {
                        focused.addClass('awe-selected');
                    }

                    var lookupPopup = grid.closest('.awe-lookup-popup, .awe-multilookup-popup');
                    if (lookupPopup.length) {
                        dto(lookupPopup).api.ok();
                    }
                }
            }
        }


        var nofocus;
        grid.keydown(function (e) {
            var trg = $(e.target);
            var which = e.which;
            if ((which == keycode.down || which == keycode.up) && trg.is('.awe-btn:not(.oddbtn)')) {
                trg = grid;
                grid.focus();
            }

            if (trg.is(grid)) {
                var keys = [40, 38, 35, 36, 34, 33];

                sctrl.keyh(e);

                if ($.inArray(which, keys) != -1) {
                    prevDef(e);
                }

                if (which == 34) {
                    // page down
                    chpage(1);
                } else if (which == 33) {
                    // page up
                    chpage(-1);
                } else if (which == 35) {
                    // end
                    sctrl.focus(grid.find('.awe-row').last());
                    sctrl.scrollToFocused();
                } else if (which == 36) {
                    // home
                    sctrl.focus(grid.find('.awe-row').first());
                    sctrl.scrollToFocused();
                } else if (which == 32) {
                    //space
                    onenter(e, grid.find('.' + sfocus));
                }
            }
        })
            .on('mousedown',
                function (e) {
                    nofocus = 1;
                    setTimeout(function () { nofocus = 0; }, 100);
                })
            .on('focusin',
                function (e) {
                    if (!nofocus && !$(e.target).is(':input')) {
                        sctrl.autofocus();
                    }

                    nofocus = 0;
                })
            .on('focusout',
                function () {
                    sctrl.remf();
                })
            .on('aweload', removeTabIndex);

        function removeTabIndex() {
            grid.find('.awe-footer .awe-btn').each(function () {
                var btn = $(this);
                btn.attr('tabindex', -1);
            });
        };

        function chpage(val) {
            var res = o.lrs;
            if (res.p < res.pc && val > 0 || res.p > 1 && val == -1) {
                $.when(api.load({ oparams: { page: res.p + val } })).done(function () {
                    var tof = null;
                    if (val < 0) tof = grid.find('.awe-row').last();
                    sctrl.autofocus(tof);
                });
            }
        }
    }

    function dragAndDrop(opt) {
        var dropContainers = [];
        var dropFuncs = [];
        var dropHoverFuncs = [];

        opt.to && $.each(opt.to, function (i, val) {
            dropContainers.push(val.c);
            dropHoverFuncs.push(val.hover);
            dropFuncs.push(val.drop);
        });

        awe.rdd(opt.from, opt.sel, dropContainers, dropFuncs, opt.dragClass, opt.hide, dropHoverFuncs, opt.end, opt.reshov, opt.scroll, opt.wrap, opt.ch, opt.cancel, opt.kdh, opt.move);
    }

    function multilookupGrid(o) {
        var popup;
        var gridsrl, gridsel;
        var api = o.api;
        o.p.dh = o.p.h;
        api.gsval = getSelectedValue;

        function getSelectedValue() {
            if (gridsel && dto(gridsel).lrs) {
                return gridsel.find('.awe-row').map(function () { return $(this).data('k'); }).get();
            } else {
                return awe.val(dto(popup).v);
            }
        }

        api.lay = function () {
            if (gridsrl && gridsel) {
                //var resth = popup.find('.awe-scon').height() - (gridsrl.outerHeight(true) + gridsel.outerHeight(true));

                var resth = popup.find('.awe-scon').height() -
                    gridsrl.outerHeight() -
                    gridsel.outerHeight() +
                    popup.outerHeight() -
                    popup.height();

                var avh = o.avh || popup.height();
                var newh = (avh - resth - 1) / 2;

                setGridHeight(gridsrl, newh);
                setGridHeight(gridsel, newh);
            }
        };

        api.rlay = function () {
            if (gridsrl) {
                initgridh(gridsrl);
            }

            if (gridsel) {
                initgridh(gridsel);
            }
        };

        api.rload = function () {
            dapi(gridsrl).load();
            dapi(gridsel).load();
        };

        o.v.on('awepopupinit', function () {
            gridsrl = null;
            gridsel = null;
            popup = o.p.d;

            popup.on('click', '.awe-movebtn', function (e) {
                var trg = $(e.target);
                var gridfrom = gridsel, gridto = gridsrl;
                if (trg.closest(gridsrl).length) {
                    gridfrom = gridsrl;
                    gridto = gridsel;
                }

                var trgRow = trg.closest('.awe-row');
                gridto.find('.awe-content .awe-tbody').prepend(dapi(gridto).renderRow(trgRow.data('model')));
                trgRow.remove();
                movedGridRow(gridfrom, gridto);
            });

            popup.on(saweinit, function (e) {

                var it = $(e.target);
                if (it.is('.awe-grid')) {
                    var gdo = dto(it);
                    gdo.pro = dto(popup);

                    var getSelected = function () { return { selected: getSelectedValue() }; };

                    gdo.parf = gdo.parf ? gdo.parf.concat(getSelected) : [getSelected];

                    if (it.is('.awe-srl')) {
                        gridsrl = it;
                    }
                    else if (it.is('.awe-sel')) {
                        gridsel = it;
                        api.lay();
                    }
                }
            });
        });

        o.p.af = 0;
    }

    function lookupKeyNav(o) {
        var popup;
        var api = o.api;
        o.v.on('awepopupinit', function () {
            popup = o.p.d;

            handleCont(o.srl.closest('.awe-list'));

            if (o.sel) {
                handleCont(o.sel.closest('.awe-list'));
            }

            function handleCont(cont) {
                cont.attr('tabindex', 0);

                var sctrl = slist(cont, { sel: '.awe-li', enter: onenter });

                function onenter(e, focused) {
                    prevDef(e);
                    var shift = e.shiftKey;
                    if (focused.is('.awe-morecont')) {
                        var prev = focused.prev();
                        focused.parent()
                            .one('aweload', function () {
                                var tofocus = pickAvEl([prev.next(), prev, cont.find('.awe-li').first()]);

                                sctrl.focus(tofocus);
                            });
                        focused.find('.awe-morebtn').click();
                    } else if (focused.find('.awe-movebtn').length && !shift) {
                        var tofocus = pickAvEl([focused.next(), focused.prev()]);

                        focused.removeClass(sfocus);
                        focused.find('.awe-movebtn').click();

                        if (tofocus) {
                            sctrl.focus(tofocus);
                        }
                    }
                    else {
                        focused.click();
                        if (shift) {
                            focused.addClass('awe-selected');
                        }
                    }

                    if (shift) {
                        api.ok();
                    }
                }

                function handleKeys(e) {
                    var keys = [40, 38, 35, 36, 34, 33];
                    if (sctrl.keyh(e) || $.inArray(e.which, keys) != -1) {
                        prevDef(e);
                    }

                    if (e.which == 32) { // space
                        onenter(e, cont.find('.focus'));
                    }
                }

                cont.keydown(handleKeys);
                cont.on('focusout', function () {
                    cont.find('.focus').removeClass(sfocus);
                }).on('keyup', function (e) {
                    if (e.which == 9)//tab
                        sctrl.autofocus();
                });
            }
        });
    }

    function lookupGrid(o) {
        var popup;
        var grid;
        var api = o.api;

        api.gsval = function () {
            return popup.find('.awe-selected').data('k');
        };

        api.lay = function () {

            if (grid) {
                var resth = popup.find('.awe-scon').height() - grid.outerHeight() + popup.outerHeight() - popup.height();

                var newh = (o.avh || popup.outerHeight()) - resth;

                setGridHeight(grid, newh);
            }
        };

        api.rlay = function () {
            if (grid) {
                initgridh(grid);
            }
        };

        api.rload = function () {
            dapi(grid).load();
        };

        o.v.on('awepopupinit', function () {
            popup = o.p.d;
            grid = null;

            popup.on('dblclick', '.awe-row', function (e) {
                if (!$(e.target).closest('.awe-nonselect').length) {
                    o.api.sval($(this).data('k'));
                }
            });

            popup.on(saweinit, function (e) {
                var g = $(e.target);
                if (g.is('.awe-grid')) {
                    dto(g).pro = dto(popup);
                    grid = g;
                    api.lay();
                }
            });
        });

        o.p.af = 0;
    }

    function tbtns(o) {
        var tabs = $('#' + o.i);

        function laybtns() {
            var av = tabs.width();
            var w = av;
            tabs.find('.awe-tabsbar br').remove();
            var btns = tabs.find('.awe-tab-btn');
            var len = btns.length;
            var isFirst = 1;
            for (var i = len - 1; i >= 0; i--) {
                var btn = btns.eq(i);
                w -= btn.outerWidth();

                if (w < 0) {
                    if (isFirst) continue;
                    btn.after('</br>');
                    isFirst = 1;
                    i++;
                    w = av;
                } else {
                    isFirst = 0;
                }
            }
        }

        tabs.on('awerender', function (e) {
            if (!$(e.target).is(tabs)) return;
            laybtns();
        });

        $win.off('resize domlay', laybtns).on('resize domlay', laybtns);
    }

    function dtp(o) {
        var id = o.i;
        var cmid = id + 'cm';
        var cyid = id + 'cy';
        var popupId = id + '-awepw';

        var monthNames = cd().Months;

        var dayNames = cd().Days.slice(0);

        if (awem.fdw) {
            dayNames.push(dayNames.shift());
        }

        var prm = o.p;
        var input = o.v;
        var openb = o.f.find('.awe-dpbtn');
        var selDate = null;
        var inline = prm.ilc;
        var inlCont = o.f.find('.awe-ilc');
        var rtl = o.rtl;
        var nxtcls = '.o-mnxt';
        var prvcls = '.o-mprv';

        if (rtl) {
            var c = nxtcls;
            nxtcls = prvcls;
            prvcls = c;
        }

        var cmdd;
        var cydd;
        var isOpening;
        var currDate;
        var today;

        var numberOfMonths;
        var defaultDate;
        var dateFormat;
        var changeYear;
        var changeMonth;
        var minDate;
        var maxDate;
        var amaxDate;
        var yearRange;

        var popup, cont;
        var wasOpen;
        var kval;

        init();

        input.attr('autocomplete', 'off');

        input.on('keyup', keyuph)
           .on('keydown', inpkeyd);

        openb.on('keydown', function (e) {
            var key = e.which;
            if (key == keycode.enter) {
                wasOpen = !isOpen();
            }

            if (keynav(key)) {
                prevDef(e);
            }
        }).on('keyup', keyuph);

        if (inline) {
            openDtp();
        } else {
            if (!isMobile()) {
                input.on('click', openDtp);
            }

            openb.on('click', openDtp);
        }

        input.change(function () {
            setVal(tryParse(input.val()));
        });

        function setVal(newVal) {
            if (newVal && (!selDate || newVal.getTime() != selDate.getTime())) {
                selDate = newVal;
                if (cont && inline || isOpen())
                    updateTo(selDate);
            }
        }

        function moveHov(dir) {
            var pivot = getHov();
            var sel = '.o-enb';
            if (cont.find(nxtcls).is(':enabled')) {
                sel = '.o-mnth:first ' + sel;
            }

            var list = cont.find(sel);

            var indx = list.index(pivot) + dir;
            var n = list.eq(indx);

            if (!n.length || indx < 0) {
                n = 0;
                var cls = dir > 0 ? nxtcls : prvcls;
                var fl = dir > 0 ? 'first' : 'last';
                var nbtn = cont.find(cls);

                if (nbtn.is(':enabled')) {
                    cont.find(cls).click();
                    n = cont.find('.o-mnth:first .o-enb:' + fl);
                }
            }

            if (n && n.length) {
                cont.find('.o-hov').removeClass('o-hov');
                n.addClass('o-hov');
            }
        }

        function keynav(key) {
            var res = 0;
            if (isOpen()) {
                if (key == keycode.down) {
                    moveHov(1);
                    res = 1;
                }
                else if (key == keycode.up) {
                    moveHov(-1);
                    res = 1;
                }
            }

            if (res) cont.addClass('o-nhov');
            return res;
        }

        function inpkeyd(e) {
            var key = e.which;

            if (keynav(key)) {
                prevDef(e);
            }

            if (!isOpen()) {
                if (key == keycode.down || key == keycode.up) {
                    openDtp(e);
                }
            }

            // / / . . - -
            awe.pnn(e, [191, 111, 190, 110, 189, 109]);

            kval = input.val();
        }

        function keyuph(e) {
            var which = e.which;

            if (isOpen()) {
                if (which == keycode.enter) {
                    if (!wasOpen) {
                        getHov().click();
                    }
                } else if (!inline && which == keycode.esc) {
                    dapi(popup).close();
                    e.stopPropagation();
                }
                else if (input.val() != kval) {
                    setVal(tryParse(input.val()));
                }
            }

            wasOpen = 0;
        }

        function isOpen() {
            return cont && cont.closest('body').length;
        }

        function getHov() {
            var h = cont.find('.o-hov');
            if (!h.length) h = cont.find('.o-enb:hover');
            if (!h.length) h = cont.find('.o-enb.o-selday');
            if (!h.length) h = cont.find('.o-enb.o-tday');
            if (!h.length) h = cont.find('.o-enb:first');

            return h;
        }

        function tryParse(sval) {
            var val = 0;
            try {
                val = $.datepicker.parseDate(dateFormat, sval);
            }
            catch (err) {
            }

            return val;
        }

        function init() {
            today = cdate();
            remTime(today);

            numberOfMonths = prm.numberOfMonths || 1;
            defaultDate = prm.defaultDate;
            dateFormat = prm.dateFormat;
            changeYear = prm.changeYear;
            changeMonth = prm.changeMonth;
            minDate = prm.minDate;
            maxDate = prm.maxDate;
            yearRange = prm.yearRange;

            if (prm.min) {
                minDate = tryParse(prm.min);
            }

            if (prm.max) {
                maxDate = tryParse(prm.max);
            }

            if (maxDate) {
                amaxDate = cdate(maxDate);
                amaxDate.setMonth(amaxDate.getMonth() - numberOfMonths + 1);
            }
        }

        function openDtp(e) {
            if (isOpen() || isOpening) return;
            isOpening = 1;

            init();

            if ($('#' + popupId).length) {
                dapi($('#' + popupId)).destroy();
            }

            selDate = tryParse(input.val());

            currDate = cdate(selDate || defaultDate || today);

            remTime(currDate);

            cont = $('<div class="o-dtp"/>').hide();
            cont.appendTo($('body'));

            if (inline) {
                cont.addClass('o-inl');
            }

            cont.html(render(currDate));
            changeMonth && awe.radioList({ i: cmid, nm: cmid, df: monthItems, md: awem.odropdown });
            changeYear && awe.radioList({ i: cyid, nm: cyid, df: yearItems, md: awem.odropdown });
            cmdd = $('#' + cmid);
            cydd = $('#' + cyid);

            updateTo(currDate, 1);

            cont.on('mousemove', function () {
                cont.removeClass('o-nhov');
                cont.find('.o-hov').removeClass('o-hov');
            });

            cont.on('click',
                '.o-mday:not(.o-dsb)',
                function () {
                    var td = $(this);
                    cont.find('.o-selday').removeClass('o-selday');
                    td.addClass('o-selday');
                    selDate = new Date(td.data('y'), td.data('m'), td.data('d'));

                    input.val($.datepicker.formatDate(dateFormat, selDate));
                    awe.ach(o);
                    popup && dapi(popup).close();
                });

            cont.on('click',
                nxtcls,
                function () {
                    updateTo(incMonth(currDate, 1));
                });

            cont.on('click',
                prvcls,
                function () {
                    updateTo(incMonth(currDate, -1));
                });

            cont.show();
            if (inline) {
                inlCont.html(cont);
            } else {
                popup = $('<div id="' + popupId + '"/>');
                popup.append(cont);

                var ctf = input;
                if (input.is('[readonly]')) {
                    ctf = openb;
                }

                if (e && $(e.target).closest(openb).length) {
                    ctf = openb;
                }

                awem.dropdownPopup({
                    opener: o.f,
                    ctf: ctf,
                    rtl: rtl,
                    p: { d: popup, i: popupId, minw: 'auto', pc: 'o-dtpp', nf: 1 },
                    tag: { Dd: 1, MinWidth: '150px' }
                });


                dapi(popup).open(e);
            }



            cmdd.change(function () {
                currDate.setDate(1);
                currDate.setMonth(cmdd.val());
                updateTo(currDate);
            });
            cydd.change(function () {
                currDate.setDate(1);
                currDate.setFullYear(cydd.val());
                updateTo(currDate);
            });

            isOpening = 0;
        }

        function updateTo(newDate, init) {
            currDate = newDate;
            if (minDate && newDate < minDate) {
                newDate = cdate(minDate);
            }

            if (amaxDate && newDate > amaxDate) {
                newDate = cdate(amaxDate);
            }

            var monthcs = cont.find('.o-mnth');
            var mlen = monthcs.length;
            monthcs.each(function (i, el) {
                var day = cdate(newDate);
                incMonth(day, i);
                var mc = $(el);
                mc.find('.o-tb').html(renderDaysTable(day, mlen));

                if (i || !changeYear) mc.find('.o-yhd').html(pad(year(day)));
                if (i || !changeMonth) mc.find('.o-mhd').html(pad(month(day)));

                if (mlen == i + 1) {
                    var ldm = lastDayOfMonth(day);
                    setDisabled(cont.find(nxtcls), maxDate && ldm >= maxDate);
                }
            });

            var fdm = firstDayOfMonth(newDate);
            setDisabled(cont.find(prvcls), minDate && fdm <= minDate);

            changeMonth && dapi(cmdd.val(newDate.getMonth())).load();
            changeYear && dapi(cydd.val(newDate.getFullYear())).load();

            if (!init) {
                popup && dapi(popup).lay();
            }
        }

        function yearItems() {
            var y = year(currDate);

            var res = [];
            var startYear = y - 10;
            var endYear = y + 10;

            if (yearRange) {
                var yra = yearRange.split(":");
                startYear = calcYear(yra[0], y, year(today));
                endYear = calcYear(yra[1], y, year(today));
            }

            if (minDate) {
                startYear = Math.max(startYear, year(minDate));
            }

            if (maxDate) {
                endYear = Math.min(endYear, year(maxDate));
            }

            for (var i = startYear; i <= endYear; i++) {
                res.push({ c: i, k: i });
            }

            return res;
        }

        function monthItems() {
            var allowed = null;
            if (minDate || maxDate) {
                var min = null, max = null;
                if (minDate) {
                    min = cdate(minDate);
                    min.setDate(1);
                }

                if (maxDate) {
                    max = cdate(maxDate);
                    max.setDate(1);
                }

                var start = cdate(currDate);
                start.setDate(1);
                start.setMonth(0);
                allowed = {};

                for (var j = 0; j < 12; j++) {
                    if ((!min || start >= min) && (!max || start <= max)) {
                        allowed[j] = 1;
                    }

                    incMonth(start, 1);
                }
            }

            var res = [];
            for (var i = 0; i < monthNames.length; i++) {
                if (!allowed || allowed[i])
                    res.push({ c: monthNames[i], k: i });
            }

            return res;
        }

        function render(currDate) {
            var res = '';
            for (var i = 0; i < numberOfMonths; i++) {
                var day = cdate(currDate);
                incMonth(day, i);

                res += '<div class="o-mnth" data-i="' + i + '">' +
                    renderMonth(day, i == 0, i == numberOfMonths - 1) +
                    '</div>';
            }

            return res;
        }

        function renderDaysTable(pivotDay, mlen) {
            var fdm = firstDayOfMonth(pivotDay);
            var ldm = lastDayOfMonth(pivotDay);

            var pmd0 = startOfWeek(fdm);
            var nm1 = endOfWeek(ldm);

            var day = pmd0;

            var table = '';

            function renderDay(d) {
                var date = d.getDate();
                var m = d.getMonth();
                var y = d.getFullYear();
                var cls = 'o-day';
                var enb = 0;
                var out = 0;
                if (d < fdm || d > ldm) {
                    cls += ' o-outm';
                    out = 1;
                } else {
                    cls += ' o-mday';
                    enb = 1;
                }

                if (d <= today && d >= today && !out && mlen) {
                    cls += ' o-tday';
                }

                if (minDate && d < minDate || maxDate && d > maxDate) {
                    cls += ' o-dsb';
                } else if (enb) {

                    cls += ' o-enb';

                    if (selDate && d <= selDate && d >= selDate) {
                        cls += ' o-selday';
                    }
                }

                return '<td class="' +
                    cls +
                    '" data-d="' +
                    date +
                    '" data-m="' +
                    m +
                    '" data-y="' +
                    y +
                    '" ><div>' +
                    date +
                    '</div></td>';
            }

            table += '<tr class="o-wdays">';
            for (var di = 0; di < dayNames.length; di++) {
                table += '<td>' + dayNames[di] + '</td>';
            }
            table += '</tr>';

            var i = 1;
            var rowstarted = 0;
            var rowCount = 0;
            while (day <= nm1 || rowCount < 6) {
                if (!rowstarted) {
                    table += '<tr>';
                    rowstarted = 1;
                }

                table += renderDay(day);

                if (i == 7) {
                    table += '</tr>';
                    rowstarted = 0;
                    i = 0;
                    rowCount++;
                }

                nextDay(day);
                i++;
            }

            return table;
        }

        function renderMonth(pivotDay, first, last) {

            var mbtn = function (cls, icls) {
                return '<button type="button" class="o-cmbtn ' + cls + '" ' +
                    '><i class="o-arw ' + icls + '"></i></button>';
            }

            var topbar = '<div class="o-mtop">';

            if (first) {
                topbar += mbtn('o-mprv', rtl ? 'right' : 'left');
            }

            var mval = pivotDay.getMonth();
            var yval = year(pivotDay);

            topbar += '<div class="o-ym"><div class="o-mhd">' +
                (first && changeMonth ? radioList(cmid, mval, 'o-cm') : pad(month(pivotDay))) +
                '</div>' +
                '<div class="o-yhd">' +
                (first && changeYear ? radioList(cyid, yval, 'o-cy') : pad(yval)) +
                '</div></div>';

            if (last) {
                topbar += mbtn('o-mnxt', rtl ? 'left' : 'right');
            }

            topbar += '</div>';

            return topbar + '<table class="o-tb"></table>';
        }

        function month(pivotDay) {
            var mval = pivotDay.getMonth();
            return monthNames[mval];
        }

        function year(pivotDay) {
            return pivotDay.getFullYear();
        }

        function pad(s) {
            return "<span class='o-txt'>" + s + "</span>";
        }

        function calcYear(fstr, cy, ty) {
            function f(res, i, fstr, cy, ty) {
                if (fstr[i] == 'c')
                    return f(cy, i + 1, fstr, cy, ty);
                if (fstr[i] == '-' || fstr[i] == '+')
                    if (res)
                        res = res + parseInt(fstr.substr(i));
                    else
                        res = ty + parseInt(fstr.substr(i));
                else return parseInt(fstr);

                return res;
            }

            return f(0, 0, fstr, cy, ty);
        }

        // utils methods

        function cdate(d) {
            return d ? new Date(d) : new Date();
        }

        function radioList(id, val, cls) {
            return '<div class="awe-ajaxradiolist-field ' + cls +
                '" ><input id="' + id +
                '" class="awe-val" type="hidden" value="' + val +
                '" /><div class="awe-display"></div></div>';
        }

        function startOfWeek(date) {
            var dat = cdate(date);

            var day = dat.getDay();
            var diff = dat.getDate() - day;

            if (awem.fdw) {
                diff += (day == 0 ? -6 : 1);
            }

            dat.setDate(diff);
            return dat;
        }

        function endOfWeek(d) {
            var dat = cdate(startOfWeek(d));
            dat.setDate(dat.getDate() + 6);
            return dat;
        }

        function firstDayOfMonth(d) {
            var dat = cdate(d);
            dat.setDate(1);
            return dat;
        }

        function lastDayOfMonth(d) {
            var nd = cdate(d);
            nd.setMonth(d.getMonth() + 1);
            nd.setDate(0);
            return nd;
        }

        function remTime(d) {
            d.setHours(0, 0, 0, 0);
        }

        function nextDay(d) {
            d.setDate(d.getDate() + 1);
        }

        function incMonth(d, m) {
            d.setDate(1);
            d.setMonth(d.getMonth() + m);
            return d;
        }
    }

    return {
        dtp: dtp,
        fdw: 0,
        tbtns: tbtns,
        lookupKeyNav: lookupKeyNav,
        multilookupGrid: multilookupGrid,
        lookupGrid: lookupGrid,
        gridMovRows: gridMovRows,
        dragAndDrop: dragAndDrop,
        clientDict: clientDict,
        gridInlineEdit: gridInlineEdit,
        gridLoading: gridLoading,
        gridInfScroll: gridInfScroll,
        gridPageSize: gridPageSize,
        gridPageInfo: gridPageInfo,
        gridColSel: gridColSel,
        gridColAutohide: gridColAutohide,
        btnGroup: buttonGroupRadio,
        btnGroupChk: buttonGroupCheckbox,
        bootstrapDropdown: bootstrapDropdown,
        multiselect: multiselect,
        colorDropdown: colorDropdown,
        imgDropdown: imgDropdown,
        combobox: combobox,
        timepicker: timepicker,
        menuDropdown: menuDropdown,
        odropdown: odropdown,
        dropdownPopup: dropdownPopup,
        uiPopup: uiPopup,
        bootstrapPopup: bootstrapPopup,
        inlinePopup: inlinePopup,
        isMobileOrTablet: isMobileOrTablet,
        multiselb: multiselb,
        gridAutoMiniPager: gridAutoMiniPager,
        gridMiniPager: gridMiniPager,
        gridKeyNav: gridkeynav,
        notif: notif,
        escape: escape,
        slist: slist
    };
}(jQuery);