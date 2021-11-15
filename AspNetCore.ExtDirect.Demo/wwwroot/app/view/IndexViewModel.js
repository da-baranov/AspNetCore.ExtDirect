Ext.define("ExtDirectDemo.view.IndexViewModel", {
    extend: "Ext.app.ViewModel",
    alias: "viewmodel.IndexViewModel",

    data: {
        name: "John Smith",

        orderedArguments: {
            a: "Some string",
            b: 3.1415926,
            c: new Date()
        },

        namedArguments: {
            a: "Some string",
            b: 3.1415926,
            c: 2022
        },

        personName: {
            prefix: "Mr.",
            firstName: "John",
            lastName: "Doe"
        },

        pollingEnabled: false,

        someRandomData: "2 * 2 = 4"
    }
});