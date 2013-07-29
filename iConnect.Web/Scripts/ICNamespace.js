
// Declare namespace.
//
// Returns the specified namespace. Also defines the namespace contents if the
// definition is specified.
//
// Parameters:
// ns: Namespace to declare as a string
// definition: Either a JSON object which defines the public members of the
// namespace or a function which returns a JSON object defining the public 
// fields of the namespace. (Optional parameter)
function ICNamespace(ns, definition) {

    // Split the namespace around dots.
    ns = ns.split(".");

    // Get the root namespace from global scope and assign this to the topNS
    var topNS;
    var nsStack = ns[0];
    eval("if (typeof " + ns[0] + " == 'undefined') " + ns[0] + " = { _namespace: '" + nsStack + "' };");
    eval("topNS = " + ns[0]);

    // If there were more than one namespace level, start navigating the hierarchy from
    // the current top onwards.
    for (var i = 1; i < ns.length; i++) {
        nsStack += "." + ns[i];
        if (typeof topNS[ns[i]] == 'undefined')
            topNS[ns[i]] = { _namespace: nsStack };

        topNS = topNS[ns[i]];
    }

    // If there was a namespace definition, copy its contents to the topNS object.
    if (definition) {

        // If the definition is a function, use its return value as the real definition.
        if (typeof definition == 'function')
            definition = definition(topNS);

        // Declare a recursive utility method that adds the definition to the
        // current NS.
        var addObjectToNS = function(nsObj, defObj, nsStack) {

            for (var memberName in defObj) {
                var member = defObj[memberName];
                var currentNsStack = nsStack + "." + memberName;

                if (typeof member == 'object' && member.constructor != Array) {
                    // The current member is an object so we'll add stuff recursively which
                    // lets us set the _namespace field.

                    if (typeof(nsObj[memberName]) != 'object')
                        nsObj[memberName] = { _namespace: currentNsStack };

                    addObjectToNS(nsObj[memberName], member, currentNsStack)
                } else {
                    // If the member isn't an object or no one has referenced the 
                    // namespace we can safely just assign a new reference.                    
                    nsObj[memberName] = member;
                }
            }
        }

        addObjectToNS(topNS, definition, nsStack);

        // If there is an initialize method, execute it.
        if (definition['initialize'])
            definition['initialize'](topNS);
    }

    return topNS;
}

