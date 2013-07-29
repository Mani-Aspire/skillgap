// 
// Utilities and helper methods.
//

ICNamespace("IConnect.Utils", function (self) {

    var assert = ICNamespace("IConnect.Assert");

    var uniqueCounter = 0;
    var currentUrl = '';
    //To get the current url at the client side
    function currenturl() {
        var url = window.location.protocol + "//" + window.location.host;
        var pathArray = window.location.pathname.split('/');
        var appPath = "/";
        for (var i = 1; i < pathArray.length - 1; i++) {
            appPath += pathArray[i] + "/";
        }
        url += appPath;
        return url;
    }

    return {
        currentUrl: currenturl(),
        extend: function (cls, base) {
            for (var key in base.prototype) {
                cls.prototype[key] = base.prototype[key];
            }
            cls.prototype.base = base;
        },

        // Unique should be as simple as possible. There are some pieces of
        // code that rely on ids not containing "_" for example. 
        unique: function (base) {
            return base + uniqueCounter++;
        },

        extractPath: function (path, item) {
            // Split the namespace around dots.
            path = path.split(".");

            var current = item;
            for (var i in path) {
                if (!current)
                    break;

                var member = path[i];
                current = current[member];
            }

            return current;
        },

        getUTCTime: function (dateObject) {
            var minutes = dateObject.getTimezoneOffset();
            dateObject.setTime(dateObject.getTime() - minutes * 60 * 1000);
            return dateObject;
        },

        strToDate: function (str, skipUTCConversion) {
            var dateRegEx = /(\d+)-(\d+)-(\d+)T(\d+):(\d+):(\d+)\.?(\d{0,3}d?)\d*\s*(.*)/;
            var dateParts = dateRegEx.exec(str);
            assert.exists(dateParts, "Invalid date " + str);
            assert.isTrue(dateParts[8] == 'Z', "Date must be UTC but instead is '" + dateParts[8] + "'");

            var ms = dateParts[7];
            while (ms.length < 3)
                ms += 0;

            // This is the date in local time with correct numbers.
            var date = new Date(
                +dateParts[1],
                +dateParts[2] - 1,  // Some smart person decided to implement JavaScript months going from 0 to 11.
                +dateParts[3],
                +dateParts[4],
                +dateParts[5],
                +dateParts[6],
                +dateParts[7])

            // Offset to the UTC, ie. offset = utc - local
            if (!skipUTCConversion) {
                // Get milliseconds of the above date.
                var utcMS = date.getTime();
                var utcOffset = date.getTimezoneOffset() * 60000;

                // The time in UTC.
                var localTime = utcMS - utcOffset;
                date.setTime(localTime);
            }

            return date;
        },

        dateToStr: function (date) {
            return date.toJSON();
        },

        copyLocalToUTCDate: function (localDate) {
            var utcDate = new Date();
            utcDate.setUTCDate(localDate.getDate());
            utcDate.setUTCMonth(localDate.getMonth());
            utcDate.setUTCFullYear(localDate.getFullYear());
            utcDate.setUTCHours(localDate.getHours());
            utcDate.setUTCMinutes(localDate.getMinutes());
            utcDate.setUTCSeconds(localDate.getSeconds());
            utcDate.setUTCMilliseconds(localDate.getMilliseconds());
            return utcDate;
        },

        copyUTCToLocalDate: function (utcDate) {
            var localDate = new Date();
            localDate.setDate(utcDate.getUTCDate());
            localDate.setMonth(utcDate.getUTCMonth());
            localDate.setFullYear(utcDate.getUTCFullYear());
            localDate.setHours(utcDate.getUTCHours());
            localDate.setMinutes(utcDate.getUTCMinutes());
            localDate.setSeconds(utcDate.getUTCSeconds());
            localDate.setMilliseconds(utcDate.getUTCMilliseconds());
            return localDate;
        },

        is24HourClock: function () {

            var d = new Date();
            d.setHours(23, 0, 0, 0);
            var s = d.toLocaleTimeString();
            if (s.substr(0, 2) == "23")
                return true;
            else
                return false;
        },

        format: function (str) {
            var pattern = /\{\d+\}/g;
            var args = arguments;
            return str.replace(pattern, function (capture) {
                return args[capture.match(/\d+/) * 1 + 1];
            });
        },

        formatSize: function (size) {
            var exts = {
                'K': Math.pow(1024, 1),
                'M': Math.pow(1024, 2),
                'G': Math.pow(1024, 3),
                'T': Math.pow(1024, 4)
            };

            var ext = '', div = 1;

            for (var e in exts) {
                var d = exts[e];
                if (d > div && size / d > 1) {
                    div = d;
                    ext = e;
                }
            }

            // Round the size to three significant decimals.
            var roundedSize = size / div;
            var decimals = 0;
            if (roundedSize < 100 && div != 1)
                decimals++;
            if (roundedSize < 10 && div != 1)
                decimals++;

            var multiplier = 1;
            if (decimals > 0)
                multiplier = 10 * decimals;

            roundedSize = Math.round(roundedSize * multiplier) / multiplier;

            // Do padding with zero to get the correct precision.
            roundedSize = roundedSize + "";
            while (decimals > 0 && roundedSize.length < 4) {
                if (roundedSize.indexOf('.') == -1)
                    roundedSize = roundedSize + ".";
                roundedSize = roundedSize + "0";
            }

            // Todo: localization.
            return roundedSize + " " + ext + "B";
        },

        copy: function (object) {
            if (typeof object === 'object' && object !== null && object.constructor != Date) {
                var obj;
                if (object.constructor === Array) {
                    obj = [];
                } else {
                    obj = {};
                }

                for (var i in object) {
                    obj[i] = self.copy(object[i]);
                }

                return obj;
            } else {
                return object;
            }
        },

        // Escape string, convert bad html to html-escaped strings.
        // TODO: Can be optimized with char based hash table that can be used to lookup replacements?
        escape: function (str) {
            if (!str)
                return "";

            var arr = new Array(str.length);

            for (var i = 0; i < str.length; i++) {
                var c = str.charAt(i);
                if (c == '<')
                    arr[i] = '&lt;';
                else if (c == '>')
                    arr[i] = '&gt;';
                else if (c == '&')
                    arr[i] = '&amp;';
                else if (c == '"')
                    arr[i] = '&quot;';
                else
                    arr[i] = c;
            }

            var r = arr.join('');
            return r;
        },

        // Encode serialized typed value.
        encodeURIPart: function (value) {

            if (value == "$0")
                return value;

            // First encode the result with URI encoding.
            var uriEncodedValue = encodeURIComponent(value);

            // Then encode % characters in order to prevent any automatic uri decoding.
            // Encode also any characters that are allowd in uri, but not allowed in Windows file names.
            var finalResult = '';
            for (var i = 0; i < uriEncodedValue.length; i++) {
                // Encode reserved characters.
                switch (uriEncodedValue.charAt(i)) {
                    case '_': finalResult = finalResult.concat("_u"); break;
                    case '%': finalResult = finalResult.concat("_p"); break;
                    case '*': finalResult = finalResult.concat("_a"); break;
                    case '.': finalResult = finalResult.concat("_s"); break;
                    case '\'': finalResult = finalResult.concat("_q"); break;
                    default: finalResult = finalResult.concat(uriEncodedValue.charAt(i)); break;
                }
            }

            // Return the result.
            return finalResult;
        },

        // Resolves the locale at browser end.
        getBrowserLocale: function () {
            if (navigator) {

                // Get relevant locale indentifiers. All browsers do not expose all values.
                var navLanguage = navigator.language;
                var navBrowserLanguage = navigator.browserLanguage;
                var navSystemLanguage = navigator.systemLanguage;
                var navUserLanguage = navigator.userLanguage;

                // Todo: investigate the best detection order.
                if (navLanguage) {
                    return navLanguage;
                }
                else if (navBrowserLanguage) {
                    return navBrowserLanguage;
                }
                else if (navSystemLanguage) {
                    return navSystemLanguage;
                }
                else if (navUserLanguage) {
                    return navUserLanguage;
                }
            }
            return '';
        },

        // Get current browser time zone offset to UTC time.
        getBrowserTimeZoneOffset: function () {

            // Get milliseconds of the above date.
            var date = new Date();
            var utcOffset = date.getTimezoneOffset() * 60000;
            return utcOffset;
        },

        // Check TypedValue existence
        typedValueExists: function (typedValue) {

            if (typedValue) {
                // See that the typed value has a non-null value.
                // .Value member can have primitive values, null, undefined and "" should be considered
                // as missing values, 0 and false are not. Values that != null are not missing either.
                if (typedValue.Value || typedValue.Value === 0 || typedValue.Value === false || typedValue.Lookup || typedValue.Lookups)
                    return true;
                else
                    return false;
            }

            return false;
        },

        // Compare ACLs for equality
        ACLequals: function (lhs, rhs) {

            // We shall return false also in the null - null case
            if (!lhs || !rhs)
                return false;

            // Quick reject
            if (lhs.length != rhs.length)
                return false;

            // Sort the lists according to the userorgroupid
            if (lhs.length > 1)
                lhs.sort(function (acl1, acl2) { return acl1.UserOrGroupID - acl2.UserOrGroupID; });
            if (rhs.length > 1)
                rhs.sort(function (acl1, acl2) { return acl1.UserOrGroupID - acl2.UserOrGroupID; });

            for (var i = 0; i < lhs.length; ++i) {
                if ((lhs[i].UserOrGroupID != rhs[i].UserOrGroupID) ||
                    (lhs[i].IsGroup != rhs[i].IsGroup) ||
                    (lhs[i].ChangePermissionsPermission != rhs[i].ChangePermissionsPermission) ||
                    (lhs[i].EditPermission != rhs[i].EditPermission) ||
                    (lhs[i].ReadPermission != rhs[i].ReadPermission))
                    return false;
            }

            return true;
        },

        // Get browser-specific search result limit.
        getSearchResultLimit: function () {

            var browser = this.getBrowserVersion();
            var limit = 500;
            if (browser.msie) {
                if (browser.realIEversion == "IE7")
                    limit = 200;
                else
                    limit = 300;
            }
            return limit;
        },

        // Get browser information.
        // This function adds additional information for
        // jQuery's $.browser object and returns it.
        getBrowserVersion: function () {
            var newInfo = {
                realIEversion: null  // Get real version of IE as string.
            };

            if ($.browser.msie) {
                if ($.browser.version < 8 && !/Trident/.exec(navigator.userAgent))  // For identifying IE7 and IE8 compatibility mode.
                    newInfo.realIEversion = "IE7";
                else if ($.browser.version < 9)
                    newInfo.realIEversion = "IE8";
                else
                    newInfo.realIEversion = "IEnew";
            }

            // Extended $.browser with newInfo.
            return $.extend({}, $.browser, newInfo);
        },

        //Round to two decimal.
        roundToTwoDecimal: function (number, decimals) {
            if (MFWA.Validator.isSignedRealNumber(number)) {
                // Note:
                // Since 0 == False, !0 == True
                // -> roundToTwoDecimal(x, 0) will result in default decimals = 2.
                if (!decimals)
                    decimals = 2;

                return number.toFixed(decimals);
            }
            else
                return number;
        },
        //Round to integer range.
        validateSignedNumberRange: function (number) {
            if (MFWA.Validator.isSignedNumber(number)) {
                if (number > 2147483647)
                    return 2147483647;
                else if (number < -2147483648)
                    return -2147483648;
                else
                    return number;
            }
            else
                return number;
        },
        getTextWidth: function (text) {
            if (!text)
                return 0;

            var dummyText = $("<span style='font-size:12px;'>" + this.escape(text) + "</span>");
            $("body").append(dummyText);
            var textWidth = dummyText.width();
            dummyText.remove();
            return textWidth;
        },
        listProperties: function (object) {
            var propertyName = "";
            for (var prop in object) {
                if (object.hasOwnProperty(prop)) {
                    propertyName += prop + " : " + object[prop] + "\n";
                }
            }
            return propertyName;
        },
        validateURL: function (textval) {
            var urlregex = new RegExp("^(http:\/\/www.|https:\/\/www.|ftp:\/\/www.|www.){1}([0-9A-Za-z]+\.)");
            return urlregex.test(textval);
        },
        replaceAll: function (sourceString, replaceString, withString) {
            if (sourceString)
                return sourceString.replace(new RegExp(replaceString, 'g'), withString)
        },
        isNumber: function (str) {
            if (!str) return false;
            str = str.toString();
            if (str.length == 0) return false;
            var re = /^[-]?\d*\.?\d*$/;
            return str.match(re);
        },
        setInnerHtml: function (control, content) {
            control[0].innerHTML = content;
        },
        getOuterHtml: function (control) {
            return $("<div>").append(control).html();
        },
        //This method gets the local decimal separatorusing java script function. 
        //The following code has no effect in case of chrome, safari browsers. In furture versions have some improvement
        //this will start working fine. 
        getClientNumbericSettings: function () {
            var value = 1.52;
            var separator = {
                decimal: value.toLocaleString().substring(1, 2)
            };
            return separator;
        },
        getParameterByName: function (name) {
            var match = RegExp('[?&]' + name + '=([^&]*)', 'i').exec(window.location.search);
            return match && match[1];
        },
        getAuthUrlString: function (novault) {
            var fields = ['extensions=mfwa']

            var savedFields = ['auth', 'username', 'password', 'domain', 'windowsUser'];
            if (!novault) savedFields.push('vault ');

            for (var k in savedFields) {
                var key = savedFields[k];
                var value = self.getParameterByName(key);
                if (value) {
                    fields.push(key + '=' + value);
                }
            }

            return fields.join('&');
        },
        getRestUrl: function (url) {
            if ($.isArray(url)) {
                url = url.join('/');
            }
            var auth = self.getAuthUrlString();
            if (auth)
                url = url + (/\?/.test(url) ? '&' : '?') + auth;
            return url;
        },

        getSkyDriveLoginURL: function () {
            ///<summary>Gets the skydrive connect url. We follow OAUTH2.0 implicity protocol. This URL retruns the AuthenticationCode directly 
            // to the redirtion URL as a hash value. skydriveRedirect.aspx has the hash value handling mechanism and share the token in localstorage
            //</summary>
            var redirectURL = "http://" + window.location.host + window.location.pathname.replace("/Default.aspx", "") + "/skydriveRedirect.aspx";
            var openURL = "https://oauth.live.com/authorize?client_id=000000004C0DFDF6&redirect_uri=" +
							  redirectURL + "&response_type=token&scope=wl.skydrive_update,wl.offline_access";
            return openURL;
        },

        isGetSetSupported: function () {

            var result;
            try {
                result = (Object.prototype.__defineGetter__ ||
                    Object.defineProperty({}, "x", { get: function () { return true } }).x)
            }
            catch (ex) {
                result = false;
            }

            return result;
        }
    }
});
