Ext.define("ExtDirectDemo.view.IndexViewModel", {
    extend: "Ext.app.ViewModel",
    alias: "viewmodel.IndexViewModel",
    requires: "ExtDirectDemo.model.Person",

    data: {
        name: "John Doe",

        calculator: {
            operand1: 1,
            operand2: 99,
            sum: undefined
        },

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

        pollingEnabled: true,

        someRandomData: "2 * 2 = 4",

        chatMessage: null,

        personsView: {
            filter: ""
        }
    },

    stores: {
        chat: {
            proxy: {
                type: "memory"
            },
            fields: [
                {
                    name: "id"
                },
                {
                    name: "date", type: "date", dateFormat: "c"
                },
                {
                    name: "message"
                }
            ]
        },

        persons: {
            autoLoad: true,
            autoSync: true,
            model: "ExtDirectDemo.model.Person",
            proxy: {
                type: "direct",
                reader: {
                    type: "json"
                },
                api: {
                    read: "Test.getPersons",
                    create: "Test.createPersons"
                }
            }
        }
    }
});