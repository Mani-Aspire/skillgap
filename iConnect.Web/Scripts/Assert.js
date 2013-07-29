// 
// assert methods and profiling overrides.
// 
var profiling = false;
ICNamespace("IConnect.Assert", {
    doesNotHaveValue: function(collection, item, message) {
        if (collection[item])
            this.promptAssert(message);
    },
    hasValue: function(collection, item, message) {
        if (!collection[item])
            this.promptAssert(message);
    },
    exists: function(value, message) {
        this.isTrue(value, message);
    },
    doesNotExist: function(value, message) {
        this.isFalse(value, message);
    },
    equals: function(value, expected, message) {
        if (value != expected)
            this.promptAssert(message);
    },
    isTrue: function(variable, message) {
        if (!variable)
            this.promptAssert(message);
    },
    isFalse: function(variable, message) {
        if (variable)
            this.promptAssert(message);
    },
    fail: function(message) {
        this.promptAssert(message);
    },
    promptAssert: function(message) {
        if (!message)
            message = "Assertion failure";

        if (!confirm("Error: " + message + "\nContinue?"))
            throw message;
    }
});