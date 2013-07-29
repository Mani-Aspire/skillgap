// Module for clientside caching.
//
// Dummy implementation is pure call through. If we need
// caching in future due to slow operations, this is the
// place to implement it.
//
// Namespace: MF.Cache

ICNamespace("IConnect.Data.Ajax", function (self) {

	var utils = ICNamespace("IConnect.Utils");
	var getQueue = {};
	// Setup the constant headers.
	var headers = {
		"X-Extensions": "MFWA",
		"X-Timezone": -(new Date().getTimezoneOffset())
	};

	// Parse the authentication details.
	var authHeader = utils.getParameterByName("auth");
	var usernameHeader = utils.getParameterByName("username");
	var passwordHeader = utils.getParameterByName("password");
	var setHeader = function (key, val) { if(val) headers[key] = val; };

	if(authHeader) {
		headers["X-Authentication"] = authHeader;
		setHeader("X-Vault", utils.getParameterByName("vault"));
	} else if(usernameHeader && passwordHeader) {
		headers["X-Username"] = usernameHeader;
		headers["X-Password"] = passwordHeader;

		setHeader("X-Domain", utils.getParameterByName("domain"));
		setHeader("X-WindowsUser", utils.getParameterByName("windowsUser"));
		setHeader("X-Vault", utils.getParameterByName("vault"));
	}

	var sanitizeEmptyString = function (data, dataType) {
		if(data === "")
			return "null";
		else
			return data;
	};

	var getErr = function (xhr) {

		try {
			var errObject = JSON.parse(xhr.responseText);
			return errObject;
		} catch(e) {

			// We got an error but no idea what it is. :(
			// We can TRY assuming it is (x)html.
			var text = xhr.responseText;
			var match = /<title>(.*?)<\/title>/.exec(text);

			var message;
			if(match) {
				message = "UnKnown Error"
				match = /<body[^>]*>((\r|\n|.)*)<\/body>/.exec(text);

				var body = null;
				if(match)
					body = match[1];

				//				error.show({
				//					message: utils.escape(message),
				//					rawstack: body
				//				});
				alert(message);

			} else {
				message = "Unknown Error";
				alert(message);
			}
		}

		return null;
	};

	// Creates an error handler for XHR error replies.
	//
	// - status: Optional parameter which is used to communicate whether
	//           the request was canceled and the scripts should consider
	//           it abandoned. An object that contains a cancel field.
	//           When the field is set to true the request is canceled.
	// - errorCb: Optional parameter used to pass an additional error
	//            callback. Passing this prevents the default error
	//            handling from taking place. When an error happens the
	//            errorCb receives the error object as a parameter.
	var createErrorHandler = function (status, errorCb) {

		return function (xhr) {

			// If the thing has been canceled, errors don't matter.
			if(!status || !status.cancel) {

				var errObject = getErr(xhr);
				if(errObject === null)
					return;

				handleError(errObject, errorCb);
			}
		}

	}


	var handleError = function (errObject, errorCb) {

		try {
			errObject.Message = errObject.Message.replace("com.motivesys.mfwa.RESTException:", "");
		}
		catch(e) {
			//swallow the exception and show the original message
		}

		// If the session has timed out, redirect to login page.
		if(((errObject.Status == 403 || errObject.Status == 401) && !errObject.IsLoggedToApplication) || (errObject.Exception && errObject.Exception.InnerException && errObject.Exception.InnerException.ErrorCode == "(0190)")) {

			var viewInfo = {
				ActiveView: "",
				IsSearch: false
			}

			MFWA.Data.Operations.saveActiveView(viewInfo);
			if(errObject.Status == 403)
				window.location = "login.aspx?timeout=true";
			else
				window.location = "login.aspx";
		} else {

			if(errorCb) {
				errorCb(errObject);
			} else {
				// Default handler for stuff that doesn't have their own handlers
				MFWA.Dialogs.Alerts.ShowError({
					Message: utils.escape(errObject.Message),
					Stack: errObject.Stack
				});
			}

		}

	}
	var doMethod = function (url, method, data, callback, errorCb, uploadErrorCb) {

		// See if the url has query parameters.
		if(url.indexOf("?") != -1)
			url += "&_method=" + method.toUpperCase();
		else
			url += "?_method=" + method.toUpperCase();

		var status = { cancel: false };

		//For handling upload file request during html drag and drop
		if(method == "PUT" && uploadErrorCb) {
			url += "&extensions=mfwa";
			jQuery.ajax({
				//type: method.toUpperCase(), IIS doesn't necessarily support the method so we'll use a _method argument instead.
				type: "POST",
				url: url,
				data: data,
				xhr: function () {  // custom xhr 
					myXhr = $.ajaxSettings.xhr();
					if(myXhr.upload) { // check if upload property exists 
						myXhr.upload.addEventListener("error", uploadErrorCb, false);
					}
					return myXhr;
				},
				success: function (data, text) {
					if(!status.cancel && callback)
						callback(data, text);
				},
				error: createErrorHandler(status, errorCb),
				contentType: false,
				processData: false
			});
		}
		else {
			jQuery.ajax({
				//type: method.toUpperCase(), IIS doesn't necessarily support the method so we'll use a _method argument instead.
				type: "POST",
				url: url,
				data: JSON.stringify(data, null, 4),
				dataFilter: sanitizeEmptyString,
				success: function (data, text) {
					if(!status.cancel && callback)
						callback(data, text);
				},
				error: createErrorHandler(status, errorCb),
				dataType: 'json',
				headers: headers
			});
		}

		return status;
	}

	// Handle grouping concurrent requests. Use this only with urls that are
	// expected to return same value with sequential requests.
	var get = function (url, success, errorCb) {

		var status = { cancel: false };

		if(getQueue[url]) {

			// If there is a pending request, push the callback method to the queue.
			getQueue[url].push({ success: success, status: status, error: errorCb });

		} else {

			// If there isn't a pending request, create a new one and initialize it
			// with the current callback.
			getQueue[url] = [{ success: success, status: status, error: errorCb}];
			jQuery.ajax({
				//type: method.toUpperCase(), IIS doesn't necessarily support the method so we'll use a _method argument instead.
				type: "GET",
				url: url,
				success: function (json) {

					// Call all the pending callbacks.
					for(var i = 0; i < getQueue[url].length; i++) {
						var request = getQueue[url][i];
						if(!request.status.cancel && request.success)
							request.success(json);
					}

					// Reset the queue array to mark that there are no pending request for this list.
					getQueue[url] = null;
				},
				error: createErrorHandler(null, function (errObject) {

					// Call all the pending callbacks.
					var invokeDefault = false;
					for(var i = 0; i < getQueue[url].length; i++) {
						var request = getQueue[url][i];
						if(!request.status.cancel) {
							// Invoke request specific handler. If there are requests
							// which do not have specific handlers, invoke default one for these.
							if(request.error)
								request.error(errObject);
							else
								invokeDefault = true;
						}
					}

					if(invokeDefault) {
						// Default handler for stuff that doesn't have their own handlers
						MFWA.Dialogs.Alerts.ShowError(errObject);
					}

					// Reset the queue array to mark that there are no pending request for this list.
					getQueue[url] = null;
				}),
				dataType: 'json',
				headers: headers
			});
		}

		return status;
	}

	var put = function (url, data, callback, errorCb, uploadErrorCb) {
		return doMethod(url, 'PUT', data, callback, errorCb, uploadErrorCb);
	}

	var post = function (url, data, callback, errorCb) {
		return doMethod(url, 'POST', data, callback, errorCb);
	}

	var del = function (url, callback, errorCb) {
		return doMethod(url, 'DELETE', null, callback, errorCb);
	}

	return {
		get: get,
		put: put,
		post: post,
		del: del,
		createErrorHandler: createErrorHandler,
		handleError: handleError,
		headers: headers
	}
});
